param([switch]$OriginalOnly)
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if (!$env:RUNNER_TEMP) { $env:RUNNER_TEMP=[IO.Path]::GetTempPath() }
if (!$env:GITHUB_RUN_ID -or !$env:GITHUB_RUN_ATTEMPT) { throw 'MPEG batch requires an identified Actions run' }
$out="recovery/evidence/audio/mpeg-boundary-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
if (Test-Path $out) { throw "Refusing to overwrite evidence: $out" }
New-Item -ItemType Directory $out -Force | Out-Null
$out=(Resolve-Path $out).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-mpeg-'+[Guid]::NewGuid().ToString('N'))
$original=Join-Path $work 'original'; $fixtures=Join-Path $work 'fixtures'
New-Item -ItemType Directory $original,$fixtures -Force | Out-Null
Copy-Item SDK/tools/*.dll $original
$csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$receipt=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;originalSha256=(Get-FileHash SDK/tools/BinaryAssetBuilder.AudioCompiler.dll).Hash.ToLowerInvariant();status='started';sdkValidated=$false;configurations=@();scope='Win32 MPEG Layer3 prefix/suffix/truncation/synchronization/container-priority acceptance; experimental cases recorded separately from established baseline';originalCases=@()}
function Run-Corpus([string]$hostDirectory,[string]$library,[string]$side) {
    $outputs=@()
    foreach ($case in $cases) {
        $caseOutput=Join-Path $out "$side-$case.txt"
        $log=Join-Path $out "$side-$case.log"
        $arguments=@(('"'+$library+'"'),('"'+$fixtures+'"'),('"'+$caseOutput+'"'),$(if ($side -eq 'original') { 'original' } else { 'recovered' }),$case)
        $process=Start-Process -FilePath "$hostDirectory/AudioMpegBoundaryOracle.exe" -ArgumentList $arguments -PassThru -NoNewWindow -RedirectStandardOutput $log -RedirectStandardError "$log.stderr"
        if (!$process.WaitForExit(15000)) {
            $process.Kill(); $process.WaitForExit()
            $outputs+=@{name=$case;status='timeout';records=0}; continue
        }
        $process.WaitForExit()
        if ($process.ExitCode -ne 0) { $outputs+=@{name=$case;status='process-failed';exitCode=$process.ExitCode;records=0}; continue }
        $lines=@(Get-Content $caseOutput)
        if ($lines.Count -ne 4) { throw "Incomplete $side MPEG case $case" }
        $outputs+=@{name=$case;status='recorded';records=4;transcriptSha256=(Get-FileHash $caseOutput).Hash.ToLowerInvariant();accepted=@($lines|Where-Object { $_ -match ':BUFFER:' }).Count}
    }
    return $outputs
}
try {
    & $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Xml.dll /r:"$original/BinaryAssetBuilder.Core.dll" /out:"$original/AudioMpegBoundaryOracle.exe" (Resolve-Path recovery/tests/AudioMpegBoundaryOracle.cs).Path (Resolve-Path recovery/tests/AudioMp3Fixtures.cs).Path *> "$out/oracle-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/oracle-build.log";throw 'MPEG oracle build failed' }
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$original/AudioMpegBoundaryOracle.exe.config"
    & "$original/AudioMpegBoundaryOracle.exe" "$original/BinaryAssetBuilder.AudioCompiler.dll" $fixtures "$out/cases.txt" list *> "$out/fixture-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/fixture-build.log";throw 'MPEG fixture creation failed' }
    $cases=@(Get-Content "$out/cases.txt")
    if ($cases.Count -lt 60) { throw 'Incomplete MPEG corpus' }
    $receipt.fixtureCount=$cases.Count
    $receipt.fixtures=@(Get-ChildItem $fixtures -Filter '*.mp3'|ForEach-Object { @{name=$_.BaseName;length=$_.Length;sha256=(Get-FileHash $_.FullName).Hash.ToLowerInvariant()} })
    $receipt.originalCases=@(Run-Corpus $original "$original/BinaryAssetBuilder.AudioCompiler.dll" original)
    $receipt.originalProcessFailures=@($receipt.originalCases|Where-Object { $_.status -ne 'recorded' }).Count
    if ($OriginalOnly) { $receipt.status='original-evidence-only';return }
    foreach ($mode in @('Debug','Release')) {
        $candidate=Join-Path $work $mode;New-Item -ItemType Directory $candidate|Out-Null
        Get-ChildItem SDK/tools -Filter '*.dll'|Where-Object Name -ne 'BinaryAssetBuilder.AudioCompiler.dll'|Copy-Item -Destination $candidate
        dotnet build src/AudioCompiler/BinaryAssetBuilder.AudioCompiler.csproj -c $mode -o $candidate -p:AssemblyName=Recovered.AudioCompiler --nologo *> "$out/$mode-build.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-build.log" -Tail 50;throw "$mode MPEG project build failed" }
        Copy-Item "$original/AudioMpegBoundaryOracle.exe*" $candidate
        $actual=@(Run-Corpus $candidate "$candidate/Recovered.AudioCompiler.dll" $mode)
        $comparisons=@()
        foreach ($expected in $receipt.originalCases) {
            $row=@($actual|Where-Object { $_.name -eq $expected.name })[0]
            $equal=($expected.status -eq 'recorded' -and $row.status -eq 'recorded' -and $expected.transcriptSha256 -eq $row.transcriptSha256)
            $comparisons+=@{name=$expected.name;equal=$equal;originalStatus=$expected.status;candidateStatus=$row.status;records=$row.records;originalAccepted=$expected.accepted;candidateAccepted=$row.accepted}
            if (!$equal -and $expected.status -eq 'recorded' -and $row.status -eq 'recorded') {
                Compare-Object (Get-Content "$out/original-$($row.name).txt") (Get-Content "$out/$mode-$($row.name).txt") | Out-String -Width 10000 | Set-Content "$out/$mode-$($row.name)-difference.txt"
            }
        }
        $differences=@($comparisons|Where-Object { !$_.equal }).Count
        $receipt.configurations+=@{configuration=$mode;passed=($differences -eq 0);differences=$differences;cases=$comparisons;originalAbsentFromRecoveredHost=$true;recoveredSha256=(Get-FileHash "$candidate/Recovered.AudioCompiler.dll").Hash.ToLowerInvariant()}
        Write-Host "MPEG $mode`: $($cases.Count-$differences)/$($cases.Count) matching isolated cases; $differences differences"
    }
    if (@($receipt.configurations|Where-Object { !$_.passed }).Count -gt 0) { $receipt.status='differences-found';throw 'MPEG boundary differences found; inspect per-case receipts before changing production support' }
    $receipt.status='passed'
} catch { if ($receipt.status -eq 'started') { $receipt.status='failed' };$receipt.error=$_.Exception.Message;throw }
finally { $receipt|ConvertTo-Json -Depth 10|Set-Content "$out/validation.json" }
