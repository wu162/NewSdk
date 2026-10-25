param()
$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if (!$env:RUNNER_TEMP) { $env:RUNNER_TEMP = [IO.Path]::GetTempPath() }
$out = "recovery/evidence/audio/adp4-decoder-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
if (Test-Path $out) { throw "Refusing to overwrite evidence: $out" }
New-Item -ItemType Directory $out -Force | Out-Null; $out = (Resolve-Path $out).Path
$work = Join-Path $env:RUNNER_TEMP ('audio-adp4-decoder-' + [Guid]::NewGuid().ToString('N'))
$original = Join-Path $work 'original'; New-Item -ItemType Directory $original -Force | Out-Null
Copy-Item SDK/tools/*.dll $original
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$receipt = @{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';originalSha256=(Get-FileHash SDK/tools/BinaryAssetBuilder.AudioCompiler.dll).Hash.ToLowerInvariant();scope='Bounded ADP4 raw-nibble integer decoder; original native0x48500 and audited mono/stereo nibble order';pluginValidated=$false;sdkValidated=$false;aifcAdp4RouteEnabled=$false;configurations=@()}
try {
    if ($receipt.originalSha256 -ne 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139') { throw 'Original DLL SHA mismatch' }
    & $csc /nologo /platform:x86 /r:System.Core.dll /out:"$original/AudioAdp4DecoderOracle.exe" (Resolve-Path recovery/tests/AudioAdp4DecoderOracle.cs).Path *> "$out/oracle-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/oracle-build.log"; throw 'ADP4 decoder oracle build failed' }
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$original/AudioAdp4DecoderOracle.exe.config"
    & "$original/AudioAdp4DecoderOracle.exe" original "$original/BinaryAssetBuilder.AudioCompiler.dll" "$out/original.tsv.gz" *> "$out/original-cpu.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/original-cpu.log"; throw 'Original CPU ADP4 decoder failed' }
    Get-Content "$out/original-cpu.log"
    $receipt.originalCpuValidated=$true;$receipt.oracleSha256=(Get-FileHash "$out/original.tsv.gz").Hash.ToLowerInvariant()
    Copy-Item "$original/AudioAdp4DecoderOracle.exe*" $work
    foreach ($mode in @('Debug','Release')) {
        $opt = if ($mode -eq 'Release') { '/optimize+' } else { '/optimize-' }
        $library = Join-Path $work "Recovered.Adp4.$mode.dll"
        & $csc /nologo /target:library /platform:x86 $opt /out:"$library" (Resolve-Path src/AudioCompiler/Native/Adp4Decoder.cs).Path *> "$out/$mode-build.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-build.log"; throw "$mode ADP4 decoder build failed" }
        & "$work/AudioAdp4DecoderOracle.exe" candidate $library "$out/original.tsv.gz" *> "$out/$mode.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode.log"; throw "$mode ADP4 decoder differs" }
        Get-Content "$out/$mode.log"
        $receipt.configurations += @{configuration=$mode;passed=$true;records=18243;scalarRecords=18067;initializedStreamSequences=176;safetyRefusals=11;originalAbsentFromRecoveredHost=(!(Test-Path "$work/BinaryAssetBuilder.AudioCompiler.dll"));recoveredSha256=(Get-FileHash $library).Hash.ToLowerInvariant()}
    }
    $receipt.status='passed'
} catch {$receipt.status='failed';$receipt.error=$_.Exception.Message;throw}
finally {$receipt | ConvertTo-Json -Depth 8 | Set-Content "$out/validation.json"}
