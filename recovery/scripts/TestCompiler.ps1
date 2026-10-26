param([Parameter(Mandatory=$true)][string]$FoundationDirectory,[Parameter(Mandatory=$true)][string]$OutputDirectory,[string]$BuildDirectory)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
if(Test-Path $OutputDirectory){throw "Refusing to overwrite product validation $OutputDirectory"}
New-Item -ItemType Directory -Force $OutputDirectory|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
if(!$BuildDirectory){$BuildDirectory=Join-Path $root ('recovery/artifacts/compiler-tests/'+[Guid]::NewGuid().ToString('N'))}
$buildRoot=[IO.Path]::GetFullPath($BuildDirectory)
$state=@{schemaVersion=1;sourceCommit=$null;runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='P3 clean production CLI/Foundation deployment; asset backend intentionally unavailable';modes=@()}
Push-Location $root
try {
    $state.sourceCommit=(git rev-parse HEAD).Trim()
    $FoundationDirectory=(Resolve-Path $FoundationDirectory).Path
    $work=Join-Path ([IO.Path]::GetTempPath()) ('compiler-test-'+[Guid]::NewGuid().ToString('N'))
    New-Item -ItemType Directory -Force $work|Out-Null
    $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    $inspector=Join-Path $work 'CompilerDeployment.exe'
    & $csc /nologo /codepage:65001 /platform:x86 /r:System.Web.Extensions.dll "/out:$inspector" (Resolve-Path 'recovery/tests/CompilerDeployment.cs').Path *> "$out/inspector-build.log"
    if($LASTEXITCODE -ne 0){Get-Content "$out/inspector-build.log";throw 'Deployment metadata inspector compilation failed'}
    foreach($mode in @('Debug','Release')) {
        $modeOut=Join-Path $out $mode;New-Item -ItemType Directory -Force $modeOut|Out-Null
        $build=Join-Path $buildRoot $mode
        try {& "$PSScriptRoot/BuildCompiler.ps1" -Configuration $mode -FoundationDirectory $FoundationDirectory -OutputDirectory $build|Out-Host}
        finally {Get-ChildItem $build -File -ErrorAction SilentlyContinue|Where-Object{$_.Extension -in @('.log','.json')}|Copy-Item -Destination $modeOut}
        $clean=Join-Path $work "$mode/clean"
        New-Item -ItemType Directory -Force $clean|Out-Null
        Copy-Item "$build/bin/*" $clean
        $allowed=@('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll')
        if(@(Get-ChildItem $clean -File|Where-Object{$_.Name -notin $allowed}).Count -ne 0){throw 'Unexpected file in clean production directory'}
        $exe=Join-Path $clean 'NewModCompiler.exe'
        & $inspector $exe (Join-Path $FoundationDirectory "$mode/Foundation.types.txt") (Join-Path $FoundationDirectory "$mode/resources.json") *> "$modeOut/metadata-runtime.log"
        if($LASTEXITCODE -ne 0){Get-Content "$modeOut/metadata-runtime.log";throw 'Clean product metadata validation failed'}
        & $exe --help *> "$modeOut/help.log"
        if($LASTEXITCODE -ne 0 -or !(Select-String -Path "$modeOut/help.log" -SimpleMatch '--mod' -Quiet)){throw 'Clean product help failed'}
        $fixture=Join-Path $work "$mode/MOD 中文 fixture"
        $sdk=Join-Path $work "$mode/SDK fixture"
        New-Item -ItemType Directory -Force "$fixture/Data/AptUI","$fixture/Data/AdditionalMaps",$sdk|Out-Null
        '<Mod/>'|Set-Content "$fixture/Data/Mod.xml"
        '<Apt/>'|Set-Content "$fixture/Data/AptUI/One.xml"
        '<Map/>'|Set-Content "$fixture/Data/AdditionalMaps/MapMetaData_One.xml"
        & $exe doctor --sdk-root $sdk *> "$modeOut/doctor.log"
        if($LASTEXITCODE -ne 3 -or !(Select-String -Path "$modeOut/doctor.log" -SimpleMatch 'BACKEND_NOT_INTEGRATED' -Quiet)){throw 'Doctor must disclose unavailable backend with exit 3'}
        foreach($command in @('plan','build')) {
            $destination=Join-Path $work "$mode/$command-output"
            & $exe $command --mod $fixture --sdk-root $sdk --out $destination --package-content base,apt,maps *> "$modeOut/$command.log"
            $code=$LASTEXITCODE;$expected=if($command -eq 'plan'){0}else{3}
            if($code -ne $expected){Get-Content "$modeOut/$command.log";throw "$mode $command exit $code differs from $expected"}
            $report=Get-Content "$destination/build-report.json" -Raw|ConvertFrom-Json
            $events=@(Get-Content "$destination/build.events.jsonl"|ForEach-Object{$_|ConvertFrom-Json})
            if(@($report.plan.Tasks).Count -ne 3){throw 'Clean production CLI plan must contain all three selected content groups'}
            if($report.exitCode -ne $code -or @($events|Where-Object{$_.eventType -eq 'BuildFinished'}).Count -ne 1){throw 'Production event/report terminal status differs'}
            if($command -eq 'build' -and @($report.diagnostics|Where-Object{$_.code -eq 'BACKEND_NOT_INTEGRATED'}).Count -ne 1){throw 'Build did not explicitly reject missing backend'}
            if(@($report.artifacts).Count -ne 0){throw 'P3 skeleton claimed compiled assets'}
            Copy-Item "$destination/build-report.json" "$modeOut/$command-report.json"
            Copy-Item "$destination/build.events.jsonl" "$modeOut/$command-events.jsonl"
        }
        $state.modes+=@{configuration=$mode;passed=$true;exeSha256=(Get-FileHash $exe -Algorithm SHA256).Hash.ToLowerInvariant();runtimeFiles=$allowed;typeInventoryVerified=$true;exactResourcesVerified=$true;productContainsTests=$false;planExit=0;doctorExit=3;buildExit=3;modCompilationSupported=$false}
        Write-Host "PASS production $mode clean directory: metadata/types/resources, help, plan/events/report, explicit unavailable backend"
    }
    $state.status='passed'
} catch {$state.status='failed';$state.error=$_.Exception.ToString();throw}
finally {$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json";Pop-Location}
