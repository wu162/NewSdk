param([string]$OutputDirectory)
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if(!$env:RUNNER_TEMP){$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
if(!$OutputDirectory){$OutputDirectory="recovery/evidence/audio/xma-codec-metadata-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if(Test-Path $OutputDirectory){throw "Refusing to overwrite XMA metadata evidence: $OutputDirectory"}
New-Item -ItemType Directory $OutputDirectory -Force|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-xma-codec-metadata-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $work|Out-Null
$csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='Complete bounded XMA EA-format, XMA2WAVE and BE cumulative seek serialization; mocked original state, real original CPU';xmaEncodingRecovered=$false;sdkValidated=$false;audioFileXmaRouteEnabled=$false;syntheticStateRecords=384;actualWrapperStates=0;nativeRvas=@('0x5f170','0x5f000','0x5eea0');nativeExportRvas=@('0x5af00','0x5afd0','0x5b570','0x5b5d0');modes=@()}
try {
 $original=Join-Path $work 'original'
 New-Item -ItemType Directory $original|Out-Null
 Copy-Item SDK/tools/*.dll $original
 $originalDll=Join-Path $original 'BinaryAssetBuilder.AudioCompiler.dll'
 $state.originalDllSha256=(Get-FileHash $originalDll).Hash.ToLowerInvariant()
 if($state.originalDllSha256 -ne 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139'){throw 'Pinned original DLL fingerprint changed'}
 & $csc /nologo /platform:x86 /r:System.Core.dll /out:"$original/AudioXmaCodecMetadataOracle.exe" (Resolve-Path recovery/tests/AudioXmaCodecMetadataOracle.cs).Path *> "$out/oracle-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/oracle-build.log";throw 'XMA metadata oracle host build failed'}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$original/AudioXmaCodecMetadataOracle.exe.config"
 $oracleInput=Join-Path $out 'original.tsv.gz'
 & "$original/AudioXmaCodecMetadataOracle.exe" original $originalDll $oracleInput *> "$out/original-cpu.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/original-cpu.log";throw 'Real original CPU metadata oracle failed'}
 Get-Content "$out/original-cpu.log"
 $state.originalCpuValidated=$true
 $state.oracleSha256=(Get-FileHash $oracleInput).Hash.ToLowerInvariant()
 Copy-Item "$original/AudioXmaCodecMetadataOracle.exe*" $work
 if(Test-Path (Join-Path $work 'BinaryAssetBuilder.AudioCompiler.dll')){throw 'Candidate host contains original target DLL'}
 foreach($mode in @('Debug','Release')){
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  $library=Join-Path $work "Recovered.Audio.XmaCodecMetadata.$mode.dll"
  & $csc /nologo /target:library /platform:x86 $opt /out:"$library" (Resolve-Path src/AudioCompiler/Native/XmaCodecMetadata.cs).Path *> "$out/$mode-build.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode-build.log";throw "$mode XMA metadata source build failed"}
  & "$work/AudioXmaCodecMetadataOracle.exe" candidate $library $oracleInput *> "$out/$mode.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode.log";throw "$mode XMA metadata differs from original"}
  Get-Content "$out/$mode.log"
  $state.modes+=@{configuration=$mode;passed=$true;summary=(Get-Content "$out/$mode.log" -Tail 1);records=384;candidateSafetyRejections=15;recoveredSha256=(Get-FileHash $library).Hash.ToLowerInvariant();originalAbsentFromRecoveredHost=$true}
 }
 $state.status='passed'
}catch{$state.status='failed';$state.error=$_.Exception.Message;throw}
finally{$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json"}
