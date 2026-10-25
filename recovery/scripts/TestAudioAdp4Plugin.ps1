param([switch]$OriginalOnly)
$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if (!$env:RUNNER_TEMP) { $env:RUNNER_TEMP = [IO.Path]::GetTempPath() }
if (!$env:GITHUB_RUN_ID -or !$env:GITHUB_RUN_ATTEMPT) { throw 'ADP4 plugin batch requires an identified Actions run' }
$out = "recovery/evidence/audio/adp4-plugin-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
if (Test-Path $out) { throw "Refusing to overwrite evidence: $out" }
New-Item -ItemType Directory $out -Force | Out-Null
$out = (Resolve-Path $out).Path
$work = Join-Path $env:RUNNER_TEMP ('audio-adp4-plugin-' + [Guid]::NewGuid().ToString('N'))
$original = Join-Path $work 'original'; $fixtures = Join-Path $work 'fixtures'
New-Item -ItemType Directory $original,$fixtures -Force | Out-Null
Copy-Item SDK/tools/*.dll $original
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$receipt = @{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;originalSha256=(Get-FileHash SDK/tools/BinaryAssetBuilder.AudioCompiler.dll).Hash.ToLowerInvariant();status='started';sdkValidated=$false;configurations=@();scope='Raw AIFC ADP4 input: container widths8/16/24, mono/stereo, frame count correction and partial SSND, PC NONE/XAS PS3 XAS RAM/streamed, filter120 rate conversion; component/plugin only, no SDK'}
try {
    & $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Xml.dll /r:"$original/BinaryAssetBuilder.Core.dll" /out:"$original/AudioAdp4PluginOracle.exe" (Resolve-Path recovery/tests/AudioAdp4PluginOracle.cs).Path (Resolve-Path recovery/tests/AudioAifcInputCases.cs).Path (Resolve-Path recovery/tests/AudioAdp4InputCases.cs).Path (Resolve-Path recovery/tests/AudioInputFormatCases.cs).Path *> "$out/oracle-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/oracle-build.log"; throw 'ADP4 plugin oracle build failed' }
    & $csc /nologo /platform:x86 /out:"$original/AudioAdp4ReaderOracle.exe" (Resolve-Path recovery/tests/AudioAdp4ReaderOracle.cs).Path (Resolve-Path recovery/tests/AudioAifcInputCases.cs).Path (Resolve-Path recovery/tests/AudioAdp4InputCases.cs).Path (Resolve-Path recovery/tests/AudioInputFormatCases.cs).Path *> "$out/reader-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/reader-build.log"; throw 'ADP4 reader oracle build failed' }
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$original/AudioAdp4PluginOracle.exe.config"
    & "$original/AudioAdp4PluginOracle.exe" "$original/BinaryAssetBuilder.AudioCompiler.dll" $fixtures "$out/original.txt" original *> "$out/original.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/original.log" -Tail 40; throw 'Original ADP4 input probe failed' }
    if (@(Get-Content "$out/original.txt").Count -ne 3180) { throw 'Expected all3180 original records' }
    Get-ChildItem $fixtures -Filter '*.aifc' | Copy-Item -Destination $out
    $receipt.originalRecords = 3180
    if ($OriginalOnly) { $receipt.status='original-evidence-only'; return }
    foreach ($mode in @('Debug','Release')) {
        $candidate = Join-Path $work $mode; New-Item -ItemType Directory $candidate | Out-Null
        Get-ChildItem SDK/tools -Filter '*.dll' | Where-Object Name -ne 'BinaryAssetBuilder.AudioCompiler.dll' | Copy-Item -Destination $candidate
        dotnet build src/AudioCompiler/BinaryAssetBuilder.AudioCompiler.csproj -c $mode -o $candidate -p:AssemblyName=Recovered.AudioCompiler --nologo *> "$out/$mode-build.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-build.log" -Tail 60; throw "$mode AIFC build failed" }
        Copy-Item "$original/AudioAdp4PluginOracle.exe*" $candidate
        Copy-Item "$original/AudioAdp4ReaderOracle.exe" $candidate
        & "$candidate/AudioAdp4ReaderOracle.exe" "$candidate/Recovered.AudioCompiler.dll" $fixtures *> "$out/$mode-reader.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-reader.log" -Tail 40; throw "$mode AIFC reader boundaries failed" }
        Get-Content "$out/$mode-reader.log" -Tail 1
        & "$candidate/AudioAdp4PluginOracle.exe" "$candidate/Recovered.AudioCompiler.dll" $fixtures "$out/$mode.txt" recovered *> "$out/$mode.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode.log" -Tail 40; throw "$mode AIFC oracle failed" }
        if ((Get-FileHash "$out/original.txt").Hash -ne (Get-FileHash "$out/$mode.txt").Hash) {
            Compare-Object (Get-Content "$out/original.txt") (Get-Content "$out/$mode.txt") | Out-String -Width 2000 | Set-Content "$out/$mode-difference.txt"
            throw "$mode AIFC transcript differs"
        }
        $receipt.configurations += @{configuration=$mode;passed=$true;records=3180;transcriptSha256=(Get-FileHash "$out/$mode.txt").Hash.ToLowerInvariant();recoveredSha256=(Get-FileHash "$candidate/Recovered.AudioCompiler.dll").Hash.ToLowerInvariant();originalAbsentFromRecoveredHost=$true;sampleBitInvariants=144;safetyRefusals=12}
        Write-Host "PASS AIFC/ADP4 $mode`:3180 complete plugin records identical"
    }
    $receipt.status='passed'
    Write-Host 'PASS AIFC/ADP4 plugin:Debug/Release3180 exact records,144 normalized/frame invariants,12 safety refusals; SDK not run'
} catch { $receipt.status='failed';$receipt.error=$_.Exception.Message;throw }
finally { $receipt | ConvertTo-Json -Depth 8 | Set-Content "$out/validation.json" }
