param([string]$OutputDirectory)
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if(!$env:RUNNER_TEMP){$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
if(!$OutputDirectory){$OutputDirectory="recovery/evidence/audio/xma-noise-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if(Test-Path $OutputDirectory){throw "Refusing to overwrite XMA noise evidence: $OutputDirectory"}
New-Item -ItemType Directory $OutputDirectory -Force|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-xma-noise-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $work|Out-Null
$csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='XMA numeric noise/state helper group 0x64cd5 and 0x65a0b only';xmaEncoderRecovered=$false;sdkValidated=$false;audioFileXmaRouteEnabled=$false;originalDllSha256=(Get-FileHash SDK/tools/BinaryAssetBuilder.AudioCompiler.dll).Hash.ToLowerInvariant();modes=@();physicalX87ControlWord='0x027f';recordsPerConfiguration=6360;prngSequences=200;noiseFills=6160}
try {
 if($state.originalDllSha256 -ne 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139'){throw 'Original DLL fingerprint changed'}
 $oracleInput=Join-Path $out 'original.tsv.gz'
 $original=Join-Path $work 'original'
 New-Item -ItemType Directory $original|Out-Null
 Copy-Item SDK/tools/*.dll $original
 & $csc /nologo /platform:x86 /r:System.Core.dll /out:"$original/AudioXmaNoiseOracle.exe" (Resolve-Path recovery/tests/AudioXmaNoiseOracle.cs).Path *> "$out/oracle-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/oracle-build.log";throw 'XMA noise oracle host build failed'}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$original/AudioXmaNoiseOracle.exe.config"
 & "$original/AudioXmaNoiseOracle.exe" original "$original/BinaryAssetBuilder.AudioCompiler.dll" $oracleInput *> "$out/original-cpu.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/original-cpu.log";throw 'Actual original CPU noise oracle failed'}
 Get-Content "$out/original-cpu.log"
 $state.oracleSha256=(Get-FileHash $oracleInput).Hash.ToLowerInvariant()
 $state.originalCpuValidated=$true
 Copy-Item "$original/AudioXmaNoiseOracle.exe*" $work
 if(Test-Path (Join-Path $work 'BinaryAssetBuilder.AudioCompiler.dll')){throw 'Original DLL copied into candidate host'}
 foreach($mode in @('Debug','Release')) {
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  $library=Join-Path $work "Recovered.Audio.XmaNoise.$mode.dll"
  & $csc /nologo /target:library /platform:x86 $opt /out:"$library" (Resolve-Path src/AudioCompiler/Native/XmaNoiseGenerator.cs).Path *> "$out/$mode-build.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode-build.log";throw "$mode XMA noise source build failed"}
  & "$work/AudioXmaNoiseOracle.exe" candidate $library $oracleInput *> "$out/$mode.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode.log";throw "$mode XMA noise helpers differ from original"}
  Get-Content "$out/$mode.log"
  $state.modes+=@{configuration=$mode;passed=$true;summary=(Get-Content "$out/$mode.log" -Tail 1);records=6360;candidateSafetyRejections=8;recoveredSha256=(Get-FileHash $library).Hash.ToLowerInvariant();originalAbsentFromRecoveredHost=(!(Test-Path (Join-Path $work 'BinaryAssetBuilder.AudioCompiler.dll')))}
 }
 $state.status='passed'
}catch{$state.status='failed';$state.error=$_.Exception.Message;throw}
finally{$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json"}
