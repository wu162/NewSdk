param()
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if (!$env:GITHUB_RUN_ID -or !$env:GITHUB_RUN_ATTEMPT) { throw 'Multichannel milestone requires an identified Actions run' }
$out="recovery/evidence/audio/multichannel-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
if (Test-Path $out) { throw 'Refusing to overwrite multichannel evidence' }
New-Item -ItemType Directory $out -Force | Out-Null; $out=(Resolve-Path $out).Path
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';sdkValidated=$false;scope='WAVE PCM8/16/24 4/6 channels only; standard/extensible NONE/XAS RAM/streamed 127/128/129/4096 frames, four sample rates, no-loop/start0/long-intro129';configurations=@()}
try {
    & "$PSScriptRoot/TestAudioPlugin.ps1" -OutputDirectory "$out/plugin" -Groups @('multichannel','pcm','xas','defaults','rates','main','pcm8','formats','loops','aiffloops','resample-full')
    $plugin=Get-Content "$out/plugin/validation.json" -Raw | ConvertFrom-Json
    if ($plugin.status -ne 'passed') { throw 'Multichannel and affected ordinary plugin regression failed' }
    $state.plugin=$plugin
    & "$PSScriptRoot/TestAudioPcm.ps1" -OriginalEvidenceDirectory "$out/plugin"
    & "$PSScriptRoot/TestAudioPcm.ps1" -Codec XAS -OriginalEvidenceDirectory "$out/plugin"
    $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    $hostExe="$out/AudioMultichannelBoundaryOracle.exe"
    & $csc /nologo /platform:x86 /out:$hostExe (Resolve-Path recovery/tests/AudioMultichannelBoundaryOracle.cs).Path (Resolve-Path recovery/tests/AudioMultichannelCases.cs).Path (Resolve-Path recovery/tests/AudioInputFormatCases.cs).Path (Resolve-Path recovery/tests/AudioAifcInputCases.cs).Path (Resolve-Path recovery/tests/AudioAdp4InputCases.cs).Path *> "$out/boundary-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/boundary-build.log"; throw 'Multichannel refusal oracle build failed' }
    $directExe="$out/AudioMultichannelDirectOracle.exe"
    & $csc /nologo /platform:x86 /r:System.Core.dll /out:$directExe (Resolve-Path recovery/tests/AudioMultichannelDirectOracle.cs).Path *> "$out/direct-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/direct-build.log"; throw 'Direct multichannel oracle build failed' }
    foreach ($mode in @('Debug','Release')) {
        $build=Join-Path $env:RUNNER_TEMP ('audio-multichannel-boundary-'+$mode+'-'+[Guid]::NewGuid().ToString('N'))
        dotnet build src/AudioCompiler/BinaryAssetBuilder.AudioCompiler.csproj -c $mode -o $build -p:AssemblyName=Recovered.AudioCompiler --nologo *> "$out/$mode-boundary-library.log"
        if ($LASTEXITCODE -ne 0) { throw "$mode refusal library build failed" }
        & $hostExe "$build/Recovered.AudioCompiler.dll" "$out/boundary-inputs" *> "$out/$mode-boundaries.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-boundaries.log"; throw "$mode multichannel refusal boundaries failed" }
        & $directExe "$build/Recovered.AudioCompiler.dll" "$out/plugin/multichannel-original.txt" "$out/plugin" *> "$out/$mode-direct.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-direct.log"; throw "$mode multichannel direct-entry differential failed" }
        $state.configurations+=@{configuration=$mode;refusals=36;directOriginalCases=64;directEncoderComparisons=128;directInvalidChannelRefusals=15;passed=$true}
    }
    $state.status='passed'
} catch { $state.status='failed';$state.error=$_.Exception.ToString();throw }
finally { $state | ConvertTo-Json -Depth 12 | Set-Content "$out/validation.json" }
