param([string]$OutputDirectory, [switch]$OriginalOnly, [switch]$IncludeMp3)
$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if (!$env:RUNNER_TEMP) { $env:RUNNER_TEMP = [IO.Path]::GetTempPath() }
if (!$OutputDirectory) {
    $suffix = if ($env:GITHUB_RUN_ID) { "$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT" } else { 'local-' + [Guid]::NewGuid().ToString('N') }
    $OutputDirectory = "recovery/evidence/audio/plugin-validation/$suffix"
}
if (Test-Path $OutputDirectory) { throw "Refusing to overwrite evidence: $OutputDirectory" }
New-Item -ItemType Directory $OutputDirectory -Force | Out-Null
$out = (Resolve-Path $OutputDirectory).Path
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$work = Join-Path $env:RUNNER_TEMP ('audio-plugin-' + [Guid]::NewGuid().ToString('N'))
$original = Join-Path $work 'original'; $fixtures = Join-Path $work 'fixtures'
New-Item -ItemType Directory $original, $fixtures -Force | Out-Null
Copy-Item SDK/tools/*.dll $original
$core = Join-Path $original 'BinaryAssetBuilder.Core.dll'
$hostExe = Join-Path $original 'AudioPluginOracle.exe'
& $csc /nologo /unsafe /platform:x86 /r:System.Core.dll /r:System.Xml.dll /r:"$core" /out:"$hostExe" (Resolve-Path 'recovery/tests/AudioPluginOracle.cs').Path (Resolve-Path 'recovery/tests/AudioMp3Fixtures.cs').Path *> "$out/oracle-build.log"
if ($LASTEXITCODE -ne 0) { Get-Content "$out/oracle-build.log"; throw 'Audio plugin oracle build failed' }
'<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$hostExe.config"
$sourceCommit = (git rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0) { throw 'Cannot identify source commit' }
$receipt = @{sourceCommit=$sourceCommit; runId=$env:GITHUB_RUN_ID; runAttempt=$env:GITHUB_RUN_ATTEMPT; originalSha256=(Get-FileHash SDK/tools/BinaryAssetBuilder.AudioCompiler.dll).Hash.ToLowerInvariant(); status='started'; configurations=@(); sdkValidated=$false; nativeAudioFileValidated=$false; scope='PathMusic map/track/event ProcessInstance/direct-handler component differential; MP3 original evidence only'}
try {
    $businessScope = if ($OriginalOnly -or $IncludeMp3) { 'business' } else { 'pathmusic' }
    $receipt.mp3Validated = [bool]$IncludeMp3
    if ($IncludeMp3) { $receipt.scope = 'PathMusic map/track/event and MP3 passthrough ProcessInstance/direct-handler component differential' }
    & $hostExe "$original/BinaryAssetBuilder.AudioCompiler.dll" original $fixtures "$out/original.txt" $businessScope *> "$out/original.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/original.log"; throw 'Original business oracle failed' }
    # Full raw PCM native evidence is retained separately and is not counted as recovered acceptance.
    & $hostExe "$original/BinaryAssetBuilder.AudioCompiler.dll" original $fixtures "$out/pcm-original.txt" pcm *> "$out/pcm-original.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/pcm-original.log"; throw 'Original PCM probe failed' }
    & $hostExe "$original/BinaryAssetBuilder.AudioCompiler.dll" original $fixtures "$out/xas-original.txt" xas *> "$out/xas-original.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/xas-original.log"; throw 'Original XAS probe failed' }
    & $hostExe "$original/BinaryAssetBuilder.AudioCompiler.dll" original $fixtures "$out/defaults-original.txt" defaults *> "$out/defaults-original.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/defaults-original.log"; throw 'Original AudioFile defaults probe failed' }
    & $hostExe "$original/BinaryAssetBuilder.AudioCompiler.dll" original $fixtures "$out/rates-original.txt" rates *> "$out/rates-original.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/rates-original.log"; throw 'Original rate boundary probe failed' }
    $env:AUDIO_TEST_REAL_DIRECTORY = (Resolve-Path recovery/fixtures/audio/main).Path
    foreach ($extraGroup in @('main','pcm8')) {
        & $hostExe "$original/BinaryAssetBuilder.AudioCompiler.dll" original $fixtures "$out/$extraGroup-original.txt" $extraGroup *> "$out/$extraGroup-original.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$extraGroup-original.log"; throw "Original $extraGroup probe failed" }
    }
    $receipt.realMainProvenanceSha256=(Get-FileHash recovery/fixtures/audio/main/provenance.json).Hash.ToLowerInvariant()
    Get-ChildItem $fixtures -Filter '*.wav' | Copy-Item -Destination $out
    if ($OriginalOnly) { $receipt.status='original-evidence-only'; return }
    $baseline = (Get-FileHash "$out/original.txt").Hash
    foreach ($mode in @('Debug', 'Release')) {
        $candidate = Join-Path $work $mode; New-Item -ItemType Directory $candidate | Out-Null
        Get-ChildItem SDK/tools -Filter '*.dll' | Where-Object Name -ne 'BinaryAssetBuilder.AudioCompiler.dll' | Copy-Item -Destination $candidate
        $library = Join-Path $candidate 'Recovered.AudioCompiler.dll'
        $sources = @(Get-ChildItem src/AudioCompiler -Filter '*.cs' -Recurse | Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' } | ForEach-Object FullName)
        if ($sources.Count -lt 2) { throw 'Recovered AudioCompiler implementation is missing' }
        $optimize = if ($mode -eq 'Release') { '/optimize+' } else { '/optimize-' }
        & dotnet build src/AudioCompiler/BinaryAssetBuilder.AudioCompiler.csproj --configuration $mode --output $candidate -p:AssemblyName=Recovered.AudioCompiler *> "$out/$mode-build.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-build.log"; throw "$mode AudioCompiler build failed" }
        Copy-Item "$original/AudioPluginOracle.exe*" $candidate
        foreach ($group in @('business','pcm','xas','defaults','rates','main','pcm8')) {
            $scope = if ($group -eq 'business') { $businessScope } else { $group }
            $expectedFile = if ($group -eq 'business') { "$out/original.txt" } else { "$out/$group-original.txt" }
            $actualFile = "$out/$mode-$group.txt"
            & "$candidate/AudioPluginOracle.exe" $library recovered $fixtures $actualFile $scope *> "$out/$mode-$group.log"
            if ($LASTEXITCODE -ne 0) {
                Get-Content "$out/$mode-$group.log" -Tail 25
                $receipt.configurations += @{configuration=$mode;group=$group;passed=$false;error='oracle failed'}
                continue
            }
            $actual = (Get-FileHash $actualFile).Hash
            $expectedHash = (Get-FileHash $expectedFile).Hash
            if ($actual -ne $expectedHash) {
                Compare-Object (Get-Content $expectedFile) (Get-Content $actualFile) | Out-String -Width 2000 | Set-Content "$out/$mode-$group-difference.txt"
                Get-Content "$out/$mode-$group-difference.txt" -First 25
                $receipt.configurations += @{configuration=$mode;group=$group;passed=$false;error='transcript differs'}
                continue
            }
            $count = @(Get-Content $actualFile).Count
            $receipt.configurations += @{configuration=$mode;group=$group;passed=$true;records=$count;transcriptSha256=$actual.ToLowerInvariant();recoveredSha256=(Get-FileHash $library).Hash.ToLowerInvariant();originalAbsentFromRecoveredHost=$true}
            Write-Host "PASS Audio plugin $mode $group`: $count identical three-buffer/custom-data records"
        }
    }
    if (@($receipt.configurations | Where-Object { !$_.passed }).Count -gt 0) { $receipt.status='failed'; throw 'Audio plugin groups failed; see individual difference files' }
    $receipt.nativeAudioFileValidated=$true
    $receipt.status='passed'
} finally {
    $receipt | ConvertTo-Json -Depth 8 | Set-Content "$out/validation.json"
}
