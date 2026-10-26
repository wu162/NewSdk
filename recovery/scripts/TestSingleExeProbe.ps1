param([Parameter(Mandatory=$true)][string]$OutputDirectory,[string]$FoundationDirectory,[switch]$Minimal)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
if (Test-Path $OutputDirectory) { throw "Refusing to overwrite probe evidence: $OutputDirectory" }
New-Item -ItemType Directory -Force $OutputDirectory | Out-Null
$out = (Resolve-Path $OutputDirectory).Path
$tempRoot = if ($env:RUNNER_TEMP) { $env:RUNNER_TEMP } else { [IO.Path]::GetTempPath() }
$work = Join-Path $tempRoot ('single-exe-' + [Guid]::NewGuid().ToString('N'))
$oracle = Join-Path $work 'original'
New-Item -ItemType Directory -Force $oracle | Out-Null
$state = @{ sourceCommit=$null;runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';minimal=[bool]$Minimal;fullG1Passed=$false;configurations=@() }
Push-Location $root
try {
    $state.sourceCommit=(git rev-parse HEAD).Trim()
    $state.originalInputs = @(@('SDK/tools/BinaryAssetBuilder.Core.dll','SDK/tools/BinaryAssetBuilder.Tokenizer.dll','SDK/tools/EALA.Hash.dll') | ForEach-Object { @{path=$_;sha256=(Get-FileHash $_ -Algorithm SHA256).Hash.ToLowerInvariant()} })
    Copy-Item SDK/tools/*.dll $oracle
    $csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    & $csc /nologo /codepage:65001 /platform:x86 /define:ORACLE /r:System.Xml.dll "/r:$oracle/BinaryAssetBuilder.Core.dll" "/r:$oracle/EALA.Hash.dll" "/out:$oracle/Original.exe" (Resolve-Path 'recovery/probes/SingleExe/Probe.cs').Path *> "$out/oracle-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/oracle-build.log" -Tail 80; throw 'Original probe build failed' }
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$oracle/Original.exe.config"
    Push-Location $oracle
    try { & "$oracle/Original.exe" "$out/original.tsv" "$oracle/BinaryAssetBuilder.Tokenizer.dll" *> "$out/original.log"; $code=$LASTEXITCODE }
    finally { Pop-Location }
    if ($code -ne 0) { Get-Content "$out/original.log" -Tail 80; throw 'Original probe fixture failed' }
    $state.rows = @(Get-Content "$out/original.tsv").Count
    if ($state.rows -ne 610) { throw "Unexpected baseline fixture row count: $($state.rows)" }
    $baseline = (Get-FileHash "$out/original.tsv" -Algorithm SHA256).Hash
    foreach ($mode in @('Debug','Release')) {
        $build = Join-Path $work $mode
        $modeEvidence = Join-Path $out $mode
        New-Item -ItemType Directory -Force $modeEvidence | Out-Null
        try { & "$PSScriptRoot/BuildSingleExeProbe.ps1" -Configuration $mode -OutputDirectory $build -FoundationDirectory $FoundationDirectory -Minimal:$Minimal | Out-Null }
        finally {
            Get-ChildItem $build -File -ErrorAction SilentlyContinue | Where-Object { $_.Extension -in @('.log','.json') } | Copy-Item -Destination $modeEvidence
        }
        $bin = Join-Path $build 'bin'
        $allowedFiles = @('NewModCompiler.exe','NewModCompiler.exe.config')
        if (!$Minimal) { $allowedFiles += 'antlr.runtime.dll' }
        $unexpected = @(Get-ChildItem $bin -File | Where-Object { $_.Name -notin $allowedFiles })
        if ($unexpected.Count -ne 0) { throw 'Unexpected file in isolated probe runtime directory' }
        # If a link retained module file dependencies, removing them must fail the test.
        $quarantine = Join-Path $work "quarantined-$mode"
        Move-Item "$build/obj" $quarantine
        Push-Location $bin
        try { & "$bin/NewModCompiler.exe" "$modeEvidence/recovered.tsv" *> "$modeEvidence/runtime.log"; $code=$LASTEXITCODE }
        finally { Pop-Location }
        if ($code -ne 0) { Get-Content "$modeEvidence/runtime.log" -Tail 100; throw "$mode isolated unified-link probe failed" }
        if ((Get-FileHash "$modeEvidence/recovered.tsv" -Algorithm SHA256).Hash -ne $baseline) {
            $a=Get-Content "$out/original.tsv";$b=Get-Content "$modeEvidence/recovered.tsv"
            for($i=0;$i -lt [Math]::Max($a.Count,$b.Count);$i++){if($a[$i] -cne $b[$i]){Write-Output "Difference row $i";break}}
            throw "$mode original/unified-link transcript mismatch"
        }
        $resourceHashes = @{}
        foreach($line in (Get-Content "$modeEvidence/recovered.tsv.resources")) {
            $parts=$line -split "`t",2
            if($parts.Length -ne 2 -or $resourceHashes.ContainsKey($parts[0])){throw 'Invalid/duplicate resource audit row'}
            $resourceHashes[$parts[0]]=$parts[1]
        }
        $buildReceipt=Get-Content "$build/build.json" -Raw | ConvertFrom-Json
        if($resourceHashes.Count -ne @($buildReceipt.resources).Count){throw 'Resource count differs from manifest'}
        foreach($resource in $buildReceipt.resources){
            $expected=(Get-FileHash $resource.path -Algorithm SHA256).Hash.ToLowerInvariant()
            if($resourceHashes[$resource.logicalName] -cne $expected){throw "Embedded resource bytes differ: $($resource.logicalName)"}
        }
        Get-Content "$modeEvidence/runtime.log"
        $state.configurations += @{configuration=$mode;passed=$true;transcriptSha256=$baseline.ToLowerInvariant();exeSha256=(Get-FileHash "$bin/NewModCompiler.exe" -Algorithm SHA256).Hash.ToLowerInvariant();moduleInputsQuarantined=$true;runtimeFiles=$allowedFiles}
    }
    $state.status='passed'
    $state.scope=if($Minimal){'minimal unified-link G1 probe only; full Foundation metadata/resources, G2 and SDK gates remain'}else{'unified Foundation/Armor link and separate-process original differential; metadata acceptance is recorded separately'}
    Write-Output 'PASS isolated unified-link Debug/Release and original Armor/Hash transcript differential'
} catch { $state.status='failed';$state.error=$_.Exception.ToString();throw }
finally {
    $state | ConvertTo-Json -Depth 8 | Set-Content "$out/validation.json"
    Pop-Location
}
