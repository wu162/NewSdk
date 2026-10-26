param([string]$OutputDirectory,[ValidateSet("AIFF","AIFC")][string[]]$Containers=@("AIFF","AIFC"),[int[]]$Channels=@(1,2,4,6),[int[]]$Frames=@(129),[int[]]$BitDepths=@(8,16,24),[ValidateSet(2)][int]$Repetitions=2,[ValidateRange(5,60)][int]$TimeoutSeconds=30)
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if (!$env:RUNNER_TEMP) { $env:RUNNER_TEMP=[IO.Path]::GetTempPath() }
if (!$OutputDirectory) { $OutputDirectory="recovery/evidence/audio/multichannel-aiff-original/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT" }
if (Test-Path $OutputDirectory) { throw 'Refusing to overwrite original multichannel evidence' }
if (@($BitDepths | Where-Object { $_ -notin @(8,16,24) }).Count) { throw 'Only integral PCM8/16/24 is in this probe' }
if (@($Channels | Where-Object { $_ -notin @(1,2,4,6) }).Count -or @($Frames | Where-Object { $_ -ne 129 }).Count) { throw 'Probe bounds exceeded' }
New-Item -ItemType Directory $OutputDirectory -Force | Out-Null; $out=(Resolve-Path $OutputDirectory).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-multichannel-aiff-original-'+[Guid]::NewGuid().ToString('N'))
$hostDirectory=Join-Path $work 'host'; New-Item -ItemType Directory $hostDirectory -Force | Out-Null
Copy-Item SDK/tools/*.dll $hostDirectory
$library=Join-Path $hostDirectory 'BinaryAssetBuilder.AudioCompiler.dll'
$receipt=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;originalOnly=$true;candidateValidated=$false;sdkValidated=$false;originalSha256=(Get-FileHash $library).Hash.ToLowerInvariant();scope='Isolated original Win32 AIFF/AIFC NONE PCM8/16/24 1/2 controls+4/6 discovery, NONE/XAS RAM/streamed,129 frames,two independent processes per case; no candidate or SDK acceptance';status='started';cases=@()}
try {
    if ($receipt.originalSha256 -ne 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139') { throw 'Original AudioCompiler fingerprint changed' }
    $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"; $hostExe=Join-Path $hostDirectory 'AudioMultichannelAiffOriginalProbe.exe'
    & $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Xml.dll /r:System.Web.Extensions.dll /r:"$hostDirectory/BinaryAssetBuilder.Core.dll" /out:$hostExe (Resolve-Path recovery/tests/AudioMultichannelAiffOriginalProbe.cs).Path *> "$out/build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/build.log"; throw 'Multichannel discovery host build failed' }
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$hostExe.config"
    foreach ($container in $Containers) { foreach ($channel in $Channels) { foreach ($frame in $Frames) { foreach ($bits in $BitDepths) { foreach ($signal in @('constant','impulse')) { foreach ($codec in @('NONE','XAS')) { foreach ($streamed in @($false,$true)) {
        $label="$container-ch$channel-b$bits-f$frame-$signal-$codec-$streamed"; $fixture=Join-Path $work $label
        $attempts=@()
        foreach ($iteration in 1..$Repetitions) {
            $record="$out/$label-$iteration.json"
            $arguments=@(('"'+$library+'"'),('"'+$fixture+'"'),('"'+$record+'"'),$channel,$frame,$codec,$streamed,$bits,$signal,$container)
            $p=Start-Process $hostExe -ArgumentList ($arguments -join ' ') -WorkingDirectory $hostDirectory -PassThru -RedirectStandardOutput "$out/$label-$iteration.stdout.log" -RedirectStandardError "$out/$label-$iteration.stderr.log"
            $timedOut=!$p.WaitForExit($TimeoutSeconds*1000)
            if ($timedOut) { Stop-Process -Id $p.Id -Force } else { $p.WaitForExit(); $p.Refresh() }
            $exitCode=if ($timedOut) { $null } else { $p.ExitCode }
            $row=@{iteration=$iteration;exitCode=$exitCode;timedOut=$timedOut;recordPresent=(Test-Path $record)}
            if (Test-Path $record) {
                $r=Get-Content $record -Raw | ConvertFrom-Json; $row.status=$r.status; $row.outputChannels=$r.outputChannels; $row.outputFrames=$r.outputFrames; $row.customPresent=$r.customPresent; $row.inputSha256=$r.inputSha256
                $row.recordSha256=(Get-FileHash $record).Hash.ToLowerInvariant()
                $row.completeOutput=(!$timedOut -and $exitCode -eq 0 -and $r.status -eq 'BUFFER' -and $r.customPresent -and $r.outputFrames -eq $frame -and $r.outputChannels -eq $channel -and $r.outputSampleRate -eq 22050 -and $null -ne $r.instance -and $null -ne $r.custom)
            } else { $row.status=if ($timedOut) {'TIMEOUT'} else {'PROCESS_FAILED'}; $row.completeOutput=$false }
            $attempts+=$row
        }
        $hashes=@($attempts | Where-Object recordPresent | ForEach-Object recordSha256 | Select-Object -Unique)
        $case=@{label=$label;container=$container;channels=$channel;bits=$bits;signal=$signal;frames=$frame;codec=$codec;streamed=$streamed;attempts=$attempts;allComplete=(@($attempts | Where-Object { !$_.completeOutput }).Count -eq 0);repeatedBytesIdentical=($hashes.Count -eq 1 -and @($attempts | Where-Object { !$_.recordPresent }).Count -eq 0)}
        $receipt.cases+=$case
        Write-Host "DISCOVERY $label complete=$($case.allComplete) repeatedBytesIdentical=$($case.repeatedBytesIdentical)"
        $receipt | ConvertTo-Json -Depth 10 | Set-Content "$out/validation.json"
    } } } } } } }
    $receipt.status='completed-original-only'; $receipt.caseCount=$receipt.cases.Count
    $expectedCount=$Containers.Count*$Channels.Count*$Frames.Count*$BitDepths.Count*2*2*2
    if ($receipt.caseCount -ne $expectedCount) { throw "Incomplete discovery matrix: expected$expectedCount got$($receipt.caseCount)" }
    $receipt.completeCases=@($receipt.cases | Where-Object allComplete).Count
    $receipt.stableCases=@($receipt.cases | Where-Object repeatedBytesIdentical).Count
    $receipt.controlsComplete=(@($receipt.cases | Where-Object { $_.channels -in @(1,2) -and !$_.allComplete }).Count -eq 0)
    New-Item -ItemType Directory "$out/inputs" -Force | Out-Null
    Get-ChildItem $work -Recurse -File | Where-Object { $_.Name -in @('input.aiff','input.aifc') } | ForEach-Object { $saved=Join-Path "$out/inputs" ($_.Directory.Name+$_.Extension);Copy-Item $_.FullName $saved; @{case=$_.Directory.Name;file=('inputs/'+[IO.Path]::GetFileName($saved));bytes=$_.Length;sha256=(Get-FileHash $_.FullName).Hash.ToLowerInvariant()} } | ConvertTo-Json -Depth 4 | Set-Content "$out/inputs.json"
} catch { $receipt.status='failed';$receipt.error=$_.Exception.ToString();throw }
finally { $receipt | ConvertTo-Json -Depth 10 | Set-Content "$out/validation.json" }
