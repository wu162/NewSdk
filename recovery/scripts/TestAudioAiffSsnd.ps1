param([switch]$OriginalOnly, [switch]$SkipLoopRegression)
$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if (!$env:RUNNER_TEMP) { $env:RUNNER_TEMP = [IO.Path]::GetTempPath() }
if (!$env:GITHUB_RUN_ID -or !$env:GITHUB_RUN_ATTEMPT) { throw 'SSND blockSize batch requires an identified Actions run' }
$out = "recovery/evidence/audio/ssnd-block-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
if (Test-Path $out) { throw "Refusing to overwrite evidence: $out" }
New-Item -ItemType Directory $out -Force | Out-Null
$out = (Resolve-Path $out).Path
$work = Join-Path $env:RUNNER_TEMP ('audio-ssnd-block-' + [Guid]::NewGuid().ToString('N'))
$original = Join-Path $work 'original'; $fixtures = Join-Path $work 'fixtures'
New-Item -ItemType Directory $original,$fixtures -Force | Out-Null
Copy-Item SDK/tools/*.dll $original
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$receipt = @{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;originalSha256=(Get-FileHash SDK/tools/BinaryAssetBuilder.AudioCompiler.dll).Hash.ToLowerInvariant();status='started';sdkValidated=$false;configurations=@();scope='SSND ignored uint32 blockSize:AIFF/AIFC NONE/raw ADP4/AIFF sustain loops; canonical0 and six nonzero values; full plugin comparison, no SDK'}
try {
    if ($receipt.originalSha256 -ne 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139') { throw 'Original DLL SHA mismatch' }
    & $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Xml.dll /r:"$original/BinaryAssetBuilder.Core.dll" /out:"$original/AudioAiffSsndOracle.exe" (Resolve-Path recovery/tests/AudioAiffSsndOracle.cs).Path (Resolve-Path recovery/tests/AudioAifcInputCases.cs).Path (Resolve-Path recovery/tests/AudioAdp4InputCases.cs).Path (Resolve-Path recovery/tests/AudioAiffSsndCases.cs).Path (Resolve-Path recovery/tests/AudioAiffLoopCases.cs).Path (Resolve-Path recovery/tests/AudioInputFormatCases.cs).Path *> "$out/oracle-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/oracle-build.log"; throw 'SSND oracle build failed' }
    & $csc /nologo /platform:x86 /out:"$original/AudioAiffSsndReaderOracle.exe" (Resolve-Path recovery/tests/AudioAiffSsndReaderOracle.cs).Path (Resolve-Path recovery/tests/AudioAifcInputCases.cs).Path (Resolve-Path recovery/tests/AudioAdp4InputCases.cs).Path (Resolve-Path recovery/tests/AudioAiffSsndCases.cs).Path (Resolve-Path recovery/tests/AudioAiffLoopCases.cs).Path (Resolve-Path recovery/tests/AudioInputFormatCases.cs).Path *> "$out/reader-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/reader-build.log"; throw 'SSND reader oracle build failed' }
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$original/AudioAiffSsndOracle.exe.config"
    & "$original/AudioAiffSsndOracle.exe" "$original/BinaryAssetBuilder.AudioCompiler.dll" $fixtures "$out/original.txt" original *> "$out/original.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/original.log" -Tail 40; throw 'Original SSND block probe failed' }
    if (@(Get-Content "$out/original.txt").Count -ne 2436) { throw 'Expected all2436 original records' }
    Get-ChildItem $fixtures -File | Where-Object { $_.Extension -in @('.aiff','.aifc') } | Copy-Item -Destination $out
    $receipt.originalRecords = 2436
    if ($OriginalOnly) { $receipt.status='original-evidence-only'; return }
    foreach ($mode in @('Debug','Release')) {
        $candidate = Join-Path $work $mode; New-Item -ItemType Directory $candidate | Out-Null
        Get-ChildItem SDK/tools -Filter '*.dll' | Where-Object Name -ne 'BinaryAssetBuilder.AudioCompiler.dll' | Copy-Item -Destination $candidate
        dotnet build src/AudioCompiler/BinaryAssetBuilder.AudioCompiler.csproj -c $mode -o $candidate -p:AssemblyName=Recovered.AudioCompiler --nologo *> "$out/$mode-build.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-build.log" -Tail 60; throw "$mode SSND build failed" }
        Copy-Item "$original/AudioAiffSsndOracle.exe*" $candidate
        Copy-Item "$original/AudioAiffSsndReaderOracle.exe" $candidate
        & "$candidate/AudioAiffSsndReaderOracle.exe" "$candidate/Recovered.AudioCompiler.dll" $fixtures *> "$out/$mode-reader.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-reader.log" -Tail 40; throw "$mode SSND reader boundaries failed" }
        Get-Content "$out/$mode-reader.log" -Tail 1
        & "$candidate/AudioAiffSsndOracle.exe" "$candidate/Recovered.AudioCompiler.dll" $fixtures "$out/$mode.txt" recovered *> "$out/$mode.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode.log" -Tail 40; throw "$mode SSND oracle failed" }
        if ((Get-FileHash "$out/original.txt").Hash -ne (Get-FileHash "$out/$mode.txt").Hash) {
            Compare-Object (Get-Content "$out/original.txt") (Get-Content "$out/$mode.txt") | Out-String -Width 2000 | Set-Content "$out/$mode-difference.txt"
            throw "$mode SSND transcript differs"
        }
        $receipt.configurations += @{configuration=$mode;passed=$true;records=2436;transcriptSha256=(Get-FileHash "$out/$mode.txt").Hash.ToLowerInvariant();recoveredSha256=(Get-FileHash "$candidate/Recovered.AudioCompiler.dll").Hash.ToLowerInvariant();originalAbsentFromRecoveredHost=$true;sampleBitInvariants=168;safetyRefusals=168}
        Write-Host "PASS SSND $mode`:2436 complete plugin records identical"
    }
    if (!$SkipLoopRegression) {
        & "$PSScriptRoot/TestAudioPlugin.ps1" -Groups aiffloops
        $receipt.affectedAiffLoopRegressionRecordsPerConfiguration=200
    } else {
        $receipt.affectedAiffLoopRegressionRecordsPerConfiguration=0
        $receipt.loopRegressionProvidedByCaller=$true
    }
    $receipt.zeroBlockCanonicalRecordsPerConfiguration=348
    $receipt.nonzeroVsZeroComparisonsPerConfiguration=2088
    $receipt.status='passed'
    Write-Host 'PASS SSND blockSize:Debug/Release2436 exact records and2088 nonzero-vs-zero comparisons;168 sample invariants/offset refusals; SDK not run'
} catch { $receipt.status='failed';$receipt.error=$_.Exception.Message;throw }
finally { $receipt | ConvertTo-Json -Depth 8 | Set-Content "$out/validation.json" }
