param([switch]$OriginalOnly)
$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if (!$env:RUNNER_TEMP) { $env:RUNNER_TEMP = [IO.Path]::GetTempPath() }
if (!$env:GITHUB_RUN_ID -or !$env:GITHUB_RUN_ATTEMPT) { throw 'Loop metadata batch requires an identified Actions run' }
$out = "recovery/evidence/audio/marker-range-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
if (Test-Path $out) { throw "Refusing to overwrite evidence: $out" }
New-Item -ItemType Directory $out -Force | Out-Null
$out = (Resolve-Path $out).Path
$work = Join-Path $env:RUNNER_TEMP ('audio-loop-meta-' + [Guid]::NewGuid().ToString('N'))
$original = Join-Path $work 'original'; $fixtures = Join-Path $work 'fixtures'
New-Item -ItemType Directory $original,$fixtures -Force | Out-Null
Copy-Item SDK/tools/*.dll $original
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$receipt = @{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;originalSha256=(Get-FileHash SDK/tools/BinaryAssetBuilder.AudioCompiler.dll).Hash.ToLowerInvariant();status='started';sdkValidated=$false;configurations=@();scope='Signed COMM-declared frame filtering before SSND correction; last COMM wins; selected out-of-range/negative marker skip preserves earlier valid duplicates; AIFF/AIFC NONE/raw ADP4 PCM8/16/24 mono/stereo PC NONE/XAS RAM/streamed and both rate directions; complete canonical comparisons'}
try {
    if ($receipt.originalSha256 -ne 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139') { throw 'Original DLL SHA mismatch' }
    & $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Xml.dll /r:"$original/BinaryAssetBuilder.Core.dll" /out:"$original/AudioAiffMarkerRangeOracle.exe" (Resolve-Path recovery/tests/AudioAiffMarkerRangeOracle.cs).Path (Resolve-Path recovery/tests/AudioAiffMarkerRangeCases.cs).Path (Resolve-Path recovery/tests/AudioAdp4InputCases.cs).Path (Resolve-Path recovery/tests/AudioAifcInputCases.cs).Path (Resolve-Path recovery/tests/AudioInputFormatCases.cs).Path *> "$out/oracle-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/oracle-build.log"; throw 'Loop metadata plugin oracle build failed' }
    & $csc /nologo /platform:x86 /out:"$original/AudioAiffMarkerRangeReaderOracle.exe" (Resolve-Path recovery/tests/AudioAiffMarkerRangeReaderOracle.cs).Path (Resolve-Path recovery/tests/AudioAiffMarkerRangeCases.cs).Path (Resolve-Path recovery/tests/AudioAdp4InputCases.cs).Path (Resolve-Path recovery/tests/AudioAifcInputCases.cs).Path (Resolve-Path recovery/tests/AudioInputFormatCases.cs).Path *> "$out/reader-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/reader-build.log"; throw 'Loop metadata reader oracle build failed' }
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$original/AudioAiffMarkerRangeOracle.exe.config"
    & "$original/AudioAiffMarkerRangeOracle.exe" "$original/BinaryAssetBuilder.AudioCompiler.dll" $fixtures "$out/original.txt" original *> "$out/original.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/original.log" -Tail 40; throw 'Original loop metadata probe failed' }
    if (@(Get-Content "$out/original.txt").Count -ne 7776) { throw 'Expected all7776 original records' }
    Get-ChildItem $fixtures -File | Where-Object { $_.Name -ne 'markrange-custom.bin' } | Copy-Item -Destination $out
    $receipt.originalRecords = 7776
    if ($OriginalOnly) { $receipt.status='original-evidence-only'; return }
    foreach ($mode in @('Debug','Release')) {
        $candidate = Join-Path $work $mode; New-Item -ItemType Directory $candidate | Out-Null
        Get-ChildItem SDK/tools -Filter '*.dll' | Where-Object Name -ne 'BinaryAssetBuilder.AudioCompiler.dll' | Copy-Item -Destination $candidate
        dotnet build src/AudioCompiler/BinaryAssetBuilder.AudioCompiler.csproj -c $mode -o $candidate -p:AssemblyName=Recovered.AudioCompiler --nologo *> "$out/$mode-build.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-build.log" -Tail 60; throw "$mode loop metadata build failed" }
        Copy-Item "$original/AudioAiffMarkerRangeOracle.exe*" $candidate
        Copy-Item "$original/AudioAiffMarkerRangeReaderOracle.exe" $candidate
        & "$candidate/AudioAiffMarkerRangeReaderOracle.exe" "$candidate/Recovered.AudioCompiler.dll" $fixtures *> "$out/$mode-reader.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-reader.log"; throw "$mode loop metadata parser assertions failed" }
        Get-Content "$out/$mode-reader.log" -Tail 1
        & "$candidate/AudioAiffMarkerRangeOracle.exe" "$candidate/Recovered.AudioCompiler.dll" $fixtures "$out/$mode.txt" recovered *> "$out/$mode.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode.log" -Tail 40; throw "$mode loop metadata oracle failed" }
        if ((Get-FileHash "$out/original.txt").Hash -ne (Get-FileHash "$out/$mode.txt").Hash) {
            Compare-Object (Get-Content "$out/original.txt") (Get-Content "$out/$mode.txt") | Out-String -Width 2000 | Set-Content "$out/$mode-difference.txt"
            throw "$mode loop metadata transcript differs"
        }
        $receipt.configurations += @{configuration=$mode;passed=$true;records=7776;transcriptSha256=(Get-FileHash "$out/$mode.txt").Hash.ToLowerInvariant();recoveredSha256=(Get-FileHash "$candidate/Recovered.AudioCompiler.dll").Hash.ToLowerInvariant();originalAbsentFromRecoveredHost=$true;canonicalPairInvariants=3888;readerAssertions=720}
        Write-Host "PASS loop metadata $mode`:7776 complete plugin records identical"
    }
    $receipt.status='passed'
} catch { $receipt.status='failed';$receipt.error=$_.Exception.Message;throw }
finally { $receipt | ConvertTo-Json -Depth 8 | Set-Content "$out/validation.json" }
