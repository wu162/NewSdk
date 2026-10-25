param([string]$OutputDirectory)
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if(!$env:RUNNER_TEMP){$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
if(!$OutputDirectory){$OutputDirectory="recovery/evidence/audio/xma-pcm-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if(Test-Path $OutputDirectory){throw 'Refusing to overwrite XMA PCM evidence'}
New-Item -ItemType Directory $OutputDirectory -Force|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-xma-pcm-'+[Guid]::NewGuid().ToString('N'))
$original=Join-Path $work 'original'
New-Item -ItemType Directory $original -Force|Out-Null
$csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='XMA PCM preprocessing RVA e9b0 only';xmaEncoderRecovered=$false;sdkValidated=$false;recordsPerConfiguration=124;exhaustiveSigned16InputValues=65536;modes=@()}
try{
 Copy-Item SDK/tools/*.dll $original
 $state.originalDllSha256=(Get-FileHash "$original/BinaryAssetBuilder.AudioCompiler.dll").Hash.ToLowerInvariant()
 if($state.originalDllSha256 -ne 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139'){throw 'Original DLL fingerprint changed'}
 & $csc /nologo /platform:x86 /r:System.Core.dll /out:"$original/AudioXmaPcmOracle.exe" (Resolve-Path recovery/tests/AudioXmaPcmOracle.cs).Path *> "$out/oracle-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/oracle-build.log";throw 'PCM oracle build failed'}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$original/AudioXmaPcmOracle.exe.config"
 $inputPath=Join-Path $out 'original.tsv.gz'
 & "$original/AudioXmaPcmOracle.exe" original "$original/BinaryAssetBuilder.AudioCompiler.dll" $inputPath *> "$out/original-cpu.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/original-cpu.log";throw 'Original CPU PCM oracle failed'}
 Get-Content "$out/original-cpu.log"
 $state.originalCpuValidated=$true;$state.oracleSha256=(Get-FileHash $inputPath).Hash.ToLowerInvariant()
 Copy-Item "$original/AudioXmaPcmOracle.exe*" $work
 foreach($mode in @('Debug','Release')){
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  $library=Join-Path $work "Recovered.Audio.XmaPcm.$mode.dll"
  & $csc /nologo /target:library /platform:x86 $opt /out:"$library" (Resolve-Path src/AudioCompiler/Native/XmaPcmConverter.cs).Path *> "$out/$mode-build.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode-build.log";throw "$mode PCM candidate build failed"}
  & "$work/AudioXmaPcmOracle.exe" candidate $library $inputPath *> "$out/$mode.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode.log";throw "$mode PCM candidate mismatch"}
  Get-Content "$out/$mode.log"
  $state.modes+=@{configuration=$mode;passed=$true;records=124;safetyRefusals=9;originalAbsentFromRecoveredHost=$true;recoveredSha256=(Get-FileHash $library).Hash.ToLowerInvariant();summary=(Get-Content "$out/$mode.log" -Tail 1)}
 }
 $state.status='passed'
}catch{$state.status='failed';$state.error=$_.Exception.Message;throw}
finally{$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json"}
