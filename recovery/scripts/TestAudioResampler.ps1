param([string]$OutputDirectory)
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if(!$env:RUNNER_TEMP){$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
if(!$OutputDirectory){$OutputDirectory="recovery/evidence/audio/resampler-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if(Test-Path $OutputDirectory){throw "Refusing to overwrite resampler evidence: $OutputDirectory"}
New-Item -ItemType Directory $OutputDirectory -Force|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-resampler-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $work|Out-Null
$csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
python recovery/scripts/GenerateAudioResamplerWindow.py
if($LASTEXITCODE -ne 0){throw 'Original window constants differ from source'}
$oracleInput=Join-Path $out 'original.tsv.gz'
python recovery/scripts/GenerateAudioResamplerOracle.py --output $oracleInput
if($LASTEXITCODE -ne 0){throw 'Original native oracle generation failed'}
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='Isolated resampling kernel exact-byte differential, including original Windows x86 CPU cross-check';sdkValidated=$false;audioFileRouteEnabled=$false;nativeOracleSha256=(Get-FileHash $oracleInput).Hash.ToLowerInvariant();modes=@()}
try {
 $original=Join-Path $work 'original'
 New-Item -ItemType Directory $original|Out-Null
 Copy-Item SDK/tools/*.dll $original
 & $csc /nologo /platform:x86 /r:System.Core.dll /out:"$original/AudioNativeResamplerOracle.exe" /main:AudioNativeResamplerOracle (Resolve-Path recovery/tests/AudioResamplerKernelOracle.cs).Path (Resolve-Path recovery/tests/AudioNativeResamplerOracle.cs).Path *> "$out/native-oracle-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/native-oracle-build.log";throw 'Original CPU resampler host build failed'}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$original/AudioNativeResamplerOracle.exe.config"
 & "$original/AudioNativeResamplerOracle.exe" "$original/BinaryAssetBuilder.AudioCompiler.dll" $oracleInput *> "$out/original-cpu.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/original-cpu.log";throw 'Real original CPU resampler differs from machine-code oracle'}
 Get-Content "$out/original-cpu.log"
 $state.originalCpuValidated=$true
 & $csc /nologo /platform:x86 /r:System.Core.dll /out:"$work/AudioResamplerKernelOracle.exe" (Resolve-Path recovery/tests/AudioResamplerKernelOracle.cs).Path *> "$out/oracle-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/oracle-build.log";throw 'Resampler oracle host build failed'}
 foreach($mode in @('Debug','Release')) {
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  $library=Join-Path $work "Recovered.Audio.Resampler.$mode.dll"
  $sources=@('ResamplerFft.cs','ResamplerFilter.cs','ResamplerCoefficients.cs','ResamplerScheduler.cs') | ForEach-Object {(Resolve-Path "src/AudioCompiler/Native/$_").Path}
  & $csc /nologo /target:library /platform:x86 $opt /out:"$library" $sources *> "$out/$mode-build.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode-build.log";throw "$mode resampler source build failed"}
  & "$work/AudioResamplerKernelOracle.exe" $library $oracleInput *> "$out/$mode.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode.log";throw "$mode resampler differs from original machine code"}
  Get-Content "$out/$mode.log"
  $state.modes+=@{configuration=$mode;passed=$true;summary=(Get-Content "$out/$mode.log" -Tail 1);recoveredSha256=(Get-FileHash $library).Hash.ToLowerInvariant();originalAbsentFromRecoveredHost=$true}
 }
 $state.status='passed'
}catch{$state.status='failed';$state.error=$_.Exception.Message;throw}
finally{$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json"}
