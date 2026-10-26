param([switch]$OriginalOnly,[switch]$SkipSdk)
$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if (!$env:RUNNER_TEMP) { $env:RUNNER_TEMP = [IO.Path]::GetTempPath() }
if (!$env:GITHUB_RUN_ID -or !$env:GITHUB_RUN_ATTEMPT) { throw 'AIFC batch requires an identified Actions run' }
$out = "recovery/evidence/audio/aifc-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
if (Test-Path $out) { throw "Refusing to overwrite evidence: $out" }
New-Item -ItemType Directory $out -Force | Out-Null
$out = (Resolve-Path $out).Path
$work = Join-Path $env:RUNNER_TEMP ('audio-aifc-' + [Guid]::NewGuid().ToString('N'))
$original = Join-Path $work 'original'; $fixtures = Join-Path $work 'fixtures'
New-Item -ItemType Directory $original,$fixtures -Force | Out-Null
Copy-Item SDK/tools/*.dll $original
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$receipt = @{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;originalSha256=(Get-FileHash SDK/tools/BinaryAssetBuilder.AudioCompiler.dll).Hash.ToLowerInvariant();status='started';sdkValidated=$false;configurations=@();scope='AIFC NONE integral PCM8/16/24; COMM names, FVER, chunk order; mono/stereo; PC NONE/XAS; RAM/streamed'}
try {
    & $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Xml.dll /r:"$original/BinaryAssetBuilder.Core.dll" /out:"$original/AudioAifcPluginOracle.exe" (Resolve-Path recovery/tests/AudioAifcPluginOracle.cs).Path (Resolve-Path recovery/tests/AudioAifcInputCases.cs).Path (Resolve-Path recovery/tests/AudioInputFormatCases.cs).Path *> "$out/oracle-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/oracle-build.log"; throw 'AIFC plugin oracle build failed' }
    & $csc /nologo /platform:x86 /out:"$original/AudioAifcReaderOracle.exe" (Resolve-Path recovery/tests/AudioAifcReaderOracle.cs).Path (Resolve-Path recovery/tests/AudioAifcInputCases.cs).Path (Resolve-Path recovery/tests/AudioInputFormatCases.cs).Path *> "$out/reader-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/reader-build.log"; throw 'AIFC reader oracle build failed' }
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$original/AudioAifcPluginOracle.exe.config"
    & "$original/AudioAifcPluginOracle.exe" "$original/BinaryAssetBuilder.AudioCompiler.dll" $fixtures "$out/original.txt" original *> "$out/original.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/original.log" -Tail 40; throw 'Original AIFC probe failed' }
    if (@(Get-Content "$out/original.txt").Count -ne 720) { throw 'Expected all720 original records' }
    Get-ChildItem $fixtures -Filter '*.aifc' | Copy-Item -Destination $out
    $receipt.originalRecords = 720
    if ($OriginalOnly) { $receipt.status='original-evidence-only'; return }
    foreach ($mode in @('Debug','Release')) {
        $candidate = Join-Path $work $mode; New-Item -ItemType Directory $candidate | Out-Null
        Get-ChildItem SDK/tools -Filter '*.dll' | Where-Object Name -ne 'BinaryAssetBuilder.AudioCompiler.dll' | Copy-Item -Destination $candidate
        dotnet build src/AudioCompiler/BinaryAssetBuilder.AudioCompiler.csproj -c $mode -o $candidate -p:AssemblyName=Recovered.AudioCompiler --nologo *> "$out/$mode-build.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-build.log" -Tail 60; throw "$mode AIFC build failed" }
        Copy-Item "$original/AudioAifcPluginOracle.exe*" $candidate
        Copy-Item "$original/AudioAifcReaderOracle.exe" $candidate
        & "$candidate/AudioAifcReaderOracle.exe" "$candidate/Recovered.AudioCompiler.dll" $fixtures *> "$out/$mode-reader.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-reader.log" -Tail 40; throw "$mode AIFC reader boundaries failed" }
        Get-Content "$out/$mode-reader.log" -Tail 1
        & "$candidate/AudioAifcPluginOracle.exe" "$candidate/Recovered.AudioCompiler.dll" $fixtures "$out/$mode.txt" recovered *> "$out/$mode.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode.log" -Tail 40; throw "$mode AIFC oracle failed" }
        if ((Get-FileHash "$out/original.txt").Hash -ne (Get-FileHash "$out/$mode.txt").Hash) {
            Compare-Object (Get-Content "$out/original.txt") (Get-Content "$out/$mode.txt") | Out-String -Width 2000 | Set-Content "$out/$mode-difference.txt"
            throw "$mode AIFC transcript differs"
        }
        $receipt.configurations += @{configuration=$mode;passed=$true;records=720;transcriptSha256=(Get-FileHash "$out/$mode.txt").Hash.ToLowerInvariant();recoveredSha256=(Get-FileHash "$candidate/Recovered.AudioCompiler.dll").Hash.ToLowerInvariant();originalAbsentFromRecoveredHost=$true;sampleBitInvariants=60;safetyRefusals=10}
        Write-Host "PASS AIFC/NONE $mode`:720 complete plugin records identical"
    }
    $receipt.status='passed'
    if (!$SkipSdk) {
        & "$PSScriptRoot/TestAudioSdk.ps1" -AifcOnly
        $sdk = Get-Content "recovery/evidence/audio/sdk-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT/validation.json" -Raw | ConvertFrom-Json
        if ($sdk.status -ne 'passed' -or !$sdk.succeeded -or $sdk.profile -ne 'create-aifc') { throw 'Missing AIFC SDK acceptance' }
        $sdkDirectory = "recovery/evidence/audio/sdk-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
        if (@(Get-Content "$sdkDirectory/expected-assets.tsv").Count -ne 120) { throw 'Missing120 expected AIFC SDK assets' }
        foreach ($side in @('Original','Recovered')) {
            if (@(Get-Content "$sdkDirectory/$side-calls.tsv").Count -ne 120) { throw "Expected120 actual $side AIFC SDK calls" }
        }
        $receipt.sdkValidated=$true
    }
} catch { $receipt.status='failed';$receipt.error=$_.Exception.Message;throw }
finally { $receipt | ConvertTo-Json -Depth 8 | Set-Content "$out/validation.json" }
