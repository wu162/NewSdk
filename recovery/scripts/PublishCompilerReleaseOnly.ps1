#requires -Version 7.0
# Release-only packaging of the accepted SuperCompilerCache Win32 runtime.
# Keeps the two-configuration PublishCompiler.ps1 contract unchanged.
[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$CompilerDirectory,
    [Parameter(Mandatory=$true)][string]$AcceptanceReceipt,
    [Parameter(Mandatory=$true)][string]$OutputDirectory,
    [ValidateRange(15,600)][int]$TimeoutSeconds=180
)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$CompilerDirectory=(Resolve-Path -LiteralPath $CompilerDirectory).Path
$AcceptanceReceipt=(Resolve-Path -LiteralPath $AcceptanceReceipt).Path
if(Test-Path -LiteralPath $OutputDirectory){throw "Refusing to overwrite release output: $OutputDirectory"}
$out=(New-Item -ItemType Directory -Path $OutputDirectory).FullName
$allowed=@('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll')
$short=$null
$state=[ordered]@{schemaVersion=1;succeeded=$false;configuration='Release';sourceCommit=(git -C $root rev-parse HEAD).Trim();candidateSourceCommit='52d44c6267385055df6da91ecdc6ee2b44f467a9';binarySourceCommit='f8e8f4a024072a892177c7200fd02fc5e8c3d62f';acceptedRunId=41275643703;runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;scope='Release-only three-file Win32/net48 runtime; prior targeted mymod/Upheaval/FLA acceptance retained with original identities; fresh independent ZIP doctor and Armor loose raw output equivalence. No new Debug, complete original SDK differential, full APT/maps/cache matrix or game acceptance.'}
function Need([bool]$Condition,[string]$Message){if(!$Condition){throw $Message}}
function Hash([string]$Path){(Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()}
function Within([string]$Directory,[string]$Path){
    $base=[IO.Path]::GetFullPath($Directory).TrimEnd([char[]]'\/');$full=[IO.Path]::GetFullPath($Path)
    $full.StartsWith($base+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)
}
function Inventory([string]$Directory){
    $items=@(Get-ChildItem -LiteralPath $Directory -Force)
    Need ($items.Count -eq $allowed.Count) "Runtime does not contain exactly three files: $Directory"
    foreach($item in $items){Need (!$item.PSIsContainer -and $item.Name -cin $allowed -and ($item.Attributes -band [IO.FileAttributes]::ReparsePoint) -eq 0) "Unexpected runtime entry: $($item.Name)"}
    return @($allowed|ForEach-Object{$file=Get-Item -LiteralPath (Join-Path $Directory $_);@{name=$_;bytes=$file.Length;sha256=(Hash $file.FullName)}})
}
function AssertInventory([string]$Directory,$Expected){
    $actual=Inventory $Directory
    for($i=0;$i -lt $Expected.Count;$i++){Need ($actual[$i].name -ceq $Expected[$i].name -and $actual[$i].bytes -eq $Expected[$i].bytes -and $actual[$i].sha256 -ceq $Expected[$i].sha256) "Runtime bytes changed: $Directory/$($Expected[$i].name)"}
}
function Controlled([string]$Name,[string]$Exe,[string]$Cwd,[string[]]$Arguments,[string]$Destination,[string]$Evidence){
    New-Item -ItemType Directory -Path $Evidence|Out-Null
    $request=@{exe=$Exe;cwd=$Cwd;arguments=$Arguments;stdout=(Join-Path $Evidence 'stdout.log');stderr=(Join-Path $Evidence 'stderr.log');events=(Join-Path $Destination 'build.events.jsonl');cancel=$false;timeoutSeconds=$TimeoutSeconds}
    $requestPath=Join-Path $Evidence 'request.json';$controlPath=Join-Path $Evidence 'control.json'
    $request|ConvertTo-Json -Depth 5|Set-Content -LiteralPath $requestPath
    & $script:control $requestPath $controlPath *> (Join-Path $Evidence 'controller.log')
    $controlExit=$LASTEXITCODE;$audit=Get-Content -LiteralPath $controlPath -Raw|ConvertFrom-Json
    if($controlExit -ne 0 -or !$audit.succeeded -or $audit.processExitCode -ne 0){
        foreach($log in @('stdout.log','stderr.log','controller.log')){Get-Content -LiteralPath (Join-Path $Evidence $log) -Tail 25|Out-Host}
        throw "Release validation process failed: $Name"
    }
    Need ($audit.singleProcessJobEnforced -and $audit.singleProcessJobLimit -eq 1 -and @($audit.descendants).Count -eq 0) "$Name launched a worker or descendant"
    return $audit
}
function BuildOutputs([string]$Runtime,[string]$Sdk,[string]$Mod,[string]$Destination,[string]$Evidence){
    New-Item -ItemType Directory -Path $Destination|Out-Null
    $audit=Controlled 'build' (Join-Path $Runtime 'NewModCompiler.exe') $Runtime @('build','--mod',$Mod,'--sdk-root',$Sdk,'--out',$Destination,'--package-format','loose','--package-content','base','/sc:false','/bc:false') $Destination $Evidence
    $reportPath=Join-Path $Destination 'build-report.json';$eventPath=Join-Path $Destination 'build.events.jsonl'
    $report=Get-Content -LiteralPath $reportPath -Raw|ConvertFrom-Json
    $events=@(Get-Content -LiteralPath $eventPath|Where-Object{$_}|ForEach-Object{$_|ConvertFrom-Json})
    Need ($report.exitCode -eq 0 -and $report.status -ceq 'succeeded' -and $report.executionMode -ceq 'InProcess' -and $report.publicationStatus -ceq 'PublishedDirectory' -and @($report.externalTools).Count -eq 0) 'Packaged build did not confirm in-process successful publication'
    Need (@($report.plan.Tasks).Count -eq 1 -and $report.plan.Tasks[0].Id -ceq 'base/Mod') 'Release probe selected an unexpected task'
    Need ($events.Count -gt 0 -and @($events|Where-Object{$_.eventType -ceq 'BuildStarted'}).Count -eq 1 -and @($events|Where-Object{$_.eventType -ceq 'BuildFinished'}).Count -eq 1 -and @($events|Where-Object{$_.eventType -ceq 'StreamStarted'}).Count -eq 1 -and @($events|Where-Object{$_.eventType -ceq 'StreamCompleted'}).Count -eq 1 -and $events[-1].eventType -ceq 'BuildFinished') 'Release protocol lifecycle differs'
    Need ($events[0].payload.processId -eq $audit.processId -and $events[-1].payload.exitCode -eq 0 -and $events[-1].payload.status -ceq $report.status) 'Release terminal protocol/process identity differs'
    for($i=0;$i -lt $events.Count;$i++){Need ($events[$i].buildId -ceq $report.buildId -and $events[$i].sequence -eq $i+1) 'Release event sequence/build correlation differs'}
    $published=Join-Path $Destination 'published';Need ($report.publicationDirectory -ceq $published) 'Release publication root differs'
    $files=@(Get-ChildItem -LiteralPath $published -File -Recurse -Force)
    Need ($files.Count -eq 4 -and @($report.artifacts).Count -eq 4) 'Release probe must produce exactly four linked outputs'
    $outputs=@{}
    foreach($artifact in $report.artifacts){
        Need ((Within $published $artifact.Path) -and (Test-Path -LiteralPath $artifact.Path -PathType Leaf)) 'Release artifact escaped its publication'
        $file=Get-Item -LiteralPath $artifact.Path
        Need ($file.Length -eq $artifact.Size) 'Release artifact differs from report'
        $relative=[IO.Path]::GetRelativePath($published,$file.FullName).Replace('\','/')
        Need ([IO.Path]::GetExtension($relative) -cin @('.manifest','.bin','.imp','.relo') -and !$outputs.ContainsKey($relative)) 'Release probe returned an unexpected linked output'
        $outputs[$relative]=@{path=$file.FullName;bytes=$file.Length;sha256=(Hash $file.FullName)}
    }
    Need (@($outputs.Keys|ForEach-Object{[IO.Path]::GetExtension($_)}|Sort-Object -Unique).Count -eq 4 -and @($outputs.Keys|ForEach-Object{$_.Substring(0,$_.Length-[IO.Path]::GetExtension($_).Length)}|Sort-Object -Unique).Count -eq 1) 'Release probe did not return one complete four-file stream'
    Need (@(Get-ChildItem -LiteralPath $Destination -File -Recurse -Filter '*.tmp').Count -eq 0 -and !(Test-Path -LiteralPath (Join-Path $Destination ('.build/'+$report.buildId)))) 'Release left uncommitted build staging or temporary reports'
    Copy-Item -LiteralPath $Destination -Destination (Join-Path $Evidence 'output') -Recurse
    foreach($relative in @($outputs.Keys)){
        $file=$outputs[$relative];$file.sourcePath=$file.path
        $file.path=Join-Path (Join-Path $Evidence 'output/published') $relative
        Need ((Test-Path -LiteralPath $file.path -PathType Leaf) -and (Get-Item -LiteralPath $file.path).Length -eq $file.bytes -and (Hash $file.path) -ceq $file.sha256) 'Archived release output differs from its original bytes'
    }
    return @{audit=$audit;outputs=$outputs;report=$report;events=$events.Count;evidenceDirectory=$Evidence}
}
try {
    # A packaging change may not change any production, SDK or build input.
    git -C $root merge-base --is-ancestor $state.binarySourceCommit HEAD
    Need ($LASTEXITCODE -eq 0) 'Binary source is not an ancestor'
    git -C $root diff --exit-code $state.binarySourceCommit HEAD -- recovery/src SDK ui recovery/build 'recovery/scripts/Build*.ps1' recovery/scripts/RestoreUiDependencies.ps1
    Need ($LASTEXITCODE -eq 0) 'Production inputs changed; runtime reuse rejected'
    $build=Get-Content (Join-Path $CompilerDirectory 'build.json') -Raw|ConvertFrom-Json
    $accept=Get-Content $AcceptanceReceipt -Raw|ConvertFrom-Json
    Need ($accept.status -ceq 'passed' -and $accept.binarySourceCommit -ceq $state.binarySourceCommit -and $accept.sourceCommit -ceq '476a78787bb026d43902e25d0b006154f14d8448') 'Targeted CLI receipt identity differs'
    Need ($build.schemaVersion -eq 2 -and $build.configuration -ceq 'Release' -and $build.sourceCommit -ceq $state.binarySourceCommit -and $build.runtime -ceq 'net48/Win32' -and $build.entryPoint -ceq 'NewModCompiler.Hosting.CompilerEntry.Main' -and $build.containsProbeOrTestSources -eq $false -and $build.backendIntegrated -eq $true -and ($build.runtimeFiles -join "`n") -ceq ($allowed -join "`n")) 'Production receipt differs'
    Need ($build.texturePlugin.implementation -ceq 'built-in-source' -and @($build.texturePlugin.externalRuntimeFiles).Count -eq 0) 'Unexpected external texture dependency'
    $sourceBin=Join-Path $CompilerDirectory 'bin'
    $inventory=Inventory $sourceBin
    Need ($inventory[0].sha256 -ceq $build.exeSha256 -and $inventory[0].sha256 -ceq '704a0b1b54d9faa99c536d433be848e8e0b678e3ab5f59d4697917a1d88fde93' -and $inventory[2].sha256 -ceq 'febdc86d6299a550c6ec87050b0cbf5205e821851e75494c699c17ce8ba2c62a') 'Accepted runtime identity differs'
    Need ((Hash (Join-Path $sourceBin 'NewModCompiler.exe.config')) -ceq (Hash (Join-Path $root 'recovery/src/App/App.config')) -and (Hash (Join-Path $sourceBin 'antlr.runtime.dll')) -ceq (Hash (Join-Path $root 'SDK/tools/antlr.runtime.dll'))) 'Runtime dependency differs from current source'
    foreach($name in @('mymod-loose-cold','mymod-loose-hot-newout','mymod-big-hot-newout','upheaval-loose-cold','upheaval-loose-hot-newout','upheaval-big-hot-newout','fla-direct-cold','fla-direct-hot-newout')){
        Need (@($accept.samples|Where-Object{$_.label -ceq $name -and $_.status -ceq 'passed' -and $_.exitCode -eq 0}).Count -eq 1) "Missing accepted CLI case: $name"
    }
    $short=(New-Item -ItemType Directory -Path (Join-Path ([IO.Path]::GetPathRoot($env:RUNNER_TEMP)) ('pkg'+$env:GITHUB_RUN_ID))).FullName
    $sdk=(New-Item -ItemType Directory -Path (Join-Path $short 's')).FullName
    New-Item -ItemType Directory -Path (Join-Path $sdk 'Schemas'),(Join-Path $sdk 'tools')|Out-Null
    Copy-Item -LiteralPath (Join-Path $root 'SDK/Schemas/xsd') -Destination (Join-Path $sdk 'Schemas') -Recurse
    Copy-Item -LiteralPath (Join-Path $root 'SDK/tools/BinaryAssetBuilder.exe.config') -Destination (Join-Path $sdk 'tools')
    $mod=Join-Path $short 'm';New-Item -ItemType Directory -Path (Join-Path $mod 'Data')|Out-Null
    Copy-Item -LiteralPath (Join-Path $root 'SDK/Mods/mymod/Data/RecoveryArmorProbe.xml') -Destination (Join-Path $mod 'Data/Mod.xml')
    $script:control=& "$PSScriptRoot/BuildCompilerCliControl.ps1" -OutputDirectory (Join-Path $out 'evidence/controller')
    $package=Join-Path $out 'NewModCompiler-Win32-Release.candidate.zip'
    Compress-Archive -LiteralPath @($allowed|ForEach-Object{Join-Path $sourceBin $_}) -DestinationPath $package
    $runtime=Join-Path $short 'deployed'
    Expand-Archive -LiteralPath $package -DestinationPath $runtime
    AssertInventory $runtime $inventory
    $doctor=Controlled 'doctor' (Join-Path $runtime 'NewModCompiler.exe') $runtime @('doctor','--sdk-root',$sdk) (Join-Path $short 'doctor-unused') (Join-Path $out 'evidence/doctor')
    Need ((Get-Content (Join-Path $out 'evidence/doctor/stdout.log') -Raw).Contains('CORE_ENVIRONMENT_OK')) 'Deployed doctor did not verify runtime'
    $baseline=BuildOutputs $sourceBin $sdk $mod (Join-Path $short 'b') (Join-Path $out 'evidence/baseline')
    $deployed=BuildOutputs $runtime $sdk $mod (Join-Path $short 'p') (Join-Path $out 'evidence/deployed')
    Need ($baseline.outputs.Count -eq $deployed.outputs.Count) 'Output count differs'
    foreach($key in $baseline.outputs.Keys){
        Need ($deployed.outputs.ContainsKey($key) -and $baseline.outputs[$key].bytes -eq $deployed.outputs[$key].bytes -and $baseline.outputs[$key].sha256 -ceq $deployed.outputs[$key].sha256) "Deployed output differs: $key"
        $left=[IO.File]::ReadAllBytes($baseline.outputs[$key].path);$right=[IO.File]::ReadAllBytes($deployed.outputs[$key].path)
        for($i=0;$i -lt $left.Length;$i++){Need ($left[$i] -eq $right[$i]) "Deployed raw byte differs: $key at $i"}
    }
    AssertInventory $sourceBin $inventory;AssertInventory $runtime $inventory
    $final=Join-Path $out 'NewModCompiler-Win32-Release.zip'
    Move-Item -LiteralPath $package -Destination $final
    $state.runtimeFiles=$inventory;$state.zipSha256=Hash $final;$state.zipBytes=(Get-Item $final).Length
    $state.doctor=$doctor;$state.rawOutputEquality=$true;$state.runtimeUnchanged=$true;$state.succeeded=$true
    $state|ConvertTo-Json -Depth 8|Set-Content (Join-Path $out 'NewModCompiler-Win32-Release.manifest.json')
    [IO.File]::WriteAllText((Join-Path $out 'SHA256SUMS.txt'),($state.zipSha256+'  NewModCompiler-Win32-Release.zip'+"`n"),[Text.UTF8Encoding]::new($false))
} finally {
    $state|ConvertTo-Json -Depth 8|Set-Content (Join-Path $out 'validation.json')
    if($short -and (Test-Path $short)){Remove-Item -LiteralPath $short -Recurse -Force}
}
