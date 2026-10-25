param()
$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if (!$env:RUNNER_TEMP) { $env:RUNNER_TEMP = [IO.Path]::GetTempPath() }
$out = "recovery/evidence/audio/adp4-probe/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
if (Test-Path $out) { throw "Refusing to overwrite evidence: $out" }
New-Item -ItemType Directory $out -Force | Out-Null; $out = (Resolve-Path $out).Path
$work = Join-Path $env:RUNNER_TEMP ('audio-adp4-probe-' + [Guid]::NewGuid().ToString('N'))
$hostDirectory = Join-Path $work 'original'; $fixtures = Join-Path $work 'fixtures'
New-Item -ItemType Directory $hostDirectory,$fixtures -Force | Out-Null
Copy-Item SDK/tools/*.dll $hostDirectory
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$receipt = @{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';originalOnly=$true;candidateValidated=$false;sdkValidated=$false;originalSha256=(Get-FileHash SDK/tools/BinaryAssetBuilder.AudioCompiler.dll).Hash.ToLowerInvariant();scope='Original-only raw-nibble AIFC ADP4 plugin acceptance/determinism; not standard Apple ima4 packets or recovered support'}
try {
    if ($receipt.originalSha256 -ne 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139') { throw 'Original DLL SHA mismatch' }
    & $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Xml.dll /r:"$hostDirectory/BinaryAssetBuilder.Core.dll" /out:"$hostDirectory/AudioAdp4Probe.exe" (Resolve-Path recovery/tests/AudioAdp4Probe.cs).Path (Resolve-Path recovery/tests/AudioAifcInputCases.cs).Path (Resolve-Path recovery/tests/AudioInputFormatCases.cs).Path *> "$out/oracle-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/oracle-build.log"; throw 'ADP4 original oracle build failed' }
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$hostDirectory/AudioAdp4Probe.exe.config"
    foreach ($iteration in @(1,2)) {
        & "$hostDirectory/AudioAdp4Probe.exe" "$hostDirectory/BinaryAssetBuilder.AudioCompiler.dll" $fixtures "$out/original-$iteration.txt" original *> "$out/original-$iteration.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/original-$iteration.log" -Tail 40; throw "Original ADP4 probe iteration$iteration failed" }
        if (@(Get-Content "$out/original-$iteration.txt").Count -ne 96) { throw 'Expected96 complete original ADP4 records' }
    }
    if ((Get-FileHash "$out/original-1.txt").Hash -ne (Get-FileHash "$out/original-2.txt").Hash) { throw 'Original ADP4 repeated process transcripts differ; investigate initialization/undefined memory' }
    $frameCounts = @(Get-Content "$out/original-1.txt" | ForEach-Object {
        $fields = $_.Split(':'); $instance = [Convert]::FromBase64String($fields[3])
        if ($fields[0] -notmatch '^adp4-(\d+)-(\d+)-(\d+)-') { throw 'Unrecognized ADP4 source label' }
        $channels = [int]$Matches[1]; $declared = [int]$Matches[2]
        "$($fields[0])`t$channels`t$declared`t$($channels*2*$declared)`t$([BitConverter]::ToUInt32($instance,12))"
    })
    $frameCounts | Set-Content "$out/frame-counts.tsv"
    Get-ChildItem $fixtures -Filter '*.aifc' | Copy-Item -Destination $out
    $inputs = @(Get-ChildItem $fixtures -Filter '*.aifc' | ForEach-Object {@{name=$_.Name;bytes=$_.Length;sha256=(Get-FileHash $_.FullName).Hash.ToLowerInvariant();declaredBits=16;compression='ADP4';rawNibblePayloadOffset=60}})
    $inputs | ConvertTo-Json -Depth 5 | Set-Content "$out/inputs.json"
    $receipt.originalRecords=96; $receipt.repeatedProcesses=2; $receipt.transcriptSha256=(Get-FileHash "$out/original-1.txt").Hash.ToLowerInvariant();$receipt.status='original-evidence-only'
    Write-Host 'PASS original-only ADP4:96 complete outputs, two independent-process transcripts identical; no candidate support'
} catch {$receipt.status='failed';$receipt.error=$_.Exception.Message;throw}
finally {$receipt | ConvertTo-Json -Depth 8 | Set-Content "$out/validation.json"}
