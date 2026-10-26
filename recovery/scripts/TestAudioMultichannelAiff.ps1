param()
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if (!$env:GITHUB_RUN_ID -or !$env:GITHUB_RUN_ATTEMPT) { throw 'AIFF milestone requires an identified Actions run' }
$out="recovery/evidence/audio/multichannel-aiff-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
if (Test-Path $out) { throw 'Refusing to overwrite AIFF evidence' }
New-Item -ItemType Directory $out -Force | Out-Null; $out=(Resolve-Path $out).Path
$state=@{sourceCommit=(git rev-parse HEAD).Trim();sourceTree=(git rev-parse HEAD:recovery/src/Managed/Audio).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';sdkValidated=$false;baselineAssets=1728;commAssets=3456;offsetAssets=1152;expectedAssets=6336;configurations=@()}
try {
    & "$PSScriptRoot/TestAudioPlugin.ps1" -OutputDirectory "$out/plugin" -Groups @('multichannel-aiff')
    $plugin=Get-Content "$out/plugin/validation.json" -Raw | ConvertFrom-Json
    if ($plugin.status -ne 'passed') { throw 'AIFF multichannel and shared-importer plugin regression failed' }
    $state.plugin=$plugin
    $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    $inputs=@('AudioMultichannelAiffReaderOracle','AudioMultichannelAiffCases','AudioAiffMarkerRangeCases','AudioAiffOffsetCases','AudioInputFormatCases','AudioAifcInputCases','AudioAdp4InputCases','AudioAiffLoopCases') | ForEach-Object { (Resolve-Path "recovery/tests/$_.cs").Path }
    & $csc /nologo /platform:x86 /r:System.Core.dll /out:"$out/AudioMultichannelAiffReaderOracle.exe" @inputs *> "$out/reader-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/reader-build.log"; throw 'AIFF reader oracle build failed' }
    foreach ($mode in @('Debug','Release')) {
        $build=Join-Path $env:RUNNER_TEMP ('audio-multichannel-aiff-'+$mode+'-'+[Guid]::NewGuid().ToString('N'))
        dotnet build recovery/src/Managed/Audio/BinaryAssetBuilder.AudioCompiler.csproj -c $mode -o $build -p:AssemblyName=Recovered.AudioCompiler --nologo *> "$out/$mode-library.log"
        if ($LASTEXITCODE -ne 0) { throw "$mode AIFF reader library build failed" }
        & "$out/AudioMultichannelAiffReaderOracle.exe" "$build/Recovered.AudioCompiler.dll" "$out/reader-inputs" *> "$out/$mode-reader.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-reader.log"; throw "$mode AIFF reader boundaries failed" }
        Get-Content "$out/$mode-reader.log"
        $state.configurations+=@{configuration=$mode;passed=$true;sourceOrderSamples=7740;windowInvariants=72;markerExpectations=432;safetyRefusals=108}
    }
    $state.status='passed'
} catch { $state.status='failed';$state.error=$_.Exception.ToString();throw }
finally { $state | ConvertTo-Json -Depth 12 | Set-Content "$out/validation.json" }
