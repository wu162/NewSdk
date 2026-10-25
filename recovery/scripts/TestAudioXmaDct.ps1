param([string]$OutputDirectory,[string]$SnapshotsDirectory,[switch]$RealOnly)
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if(!$env:RUNNER_TEMP){$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
if(!$OutputDirectory){$group=if($RealOnly){'xma-dct-replay-validation'}else{'xma-dct-validation'};$OutputDirectory="recovery/evidence/audio/$group/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if(Test-Path $OutputDirectory){throw "Refusing to overwrite XMA table-backed DCT-IV evidence: $OutputDirectory"}
New-Item -ItemType Directory $OutputDirectory -Force|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-xma-dct-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $work|Out-Null
$csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='Complete table-backed XMA DCT-IV 0xa81db sizes64..2048 powers of two';xmaEncoderRecovered=$false;sdkValidated=$false;audioFileXmaRouteEnabled=$false;originalDllSha256=(Get-FileHash SDK/tools/BinaryAssetBuilder.AudioCompiler.dll).Hash.ToLowerInvariant();modes=@();physicalX87ControlWord='0x027f';recordsPerConfiguration=$(if($RealOnly){0}else{360});sizes=6;scales=10;patterns=6;realOnly=[bool]$RealOnly}
try {
 if($state.originalDllSha256 -ne 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139'){throw 'Original DLL fingerprint changed'}
 $snapshotArgs=@()
 if($RealOnly -and !$SnapshotsDirectory){throw 'Real-only DCT replay requires SnapshotsDirectory'}
 if($SnapshotsDirectory){$SnapshotsDirectory=(Resolve-Path $SnapshotsDirectory).Path;$snapshotArgs=@($SnapshotsDirectory);$state.snapshotsDirectory=$SnapshotsDirectory;$state.realSnapshotsExpected=@(Get-ChildItem $SnapshotsDirectory -Filter case.json -Recurse).Count;if(!$state.realSnapshotsExpected){throw 'Requested real DCT snapshot directory is empty'}}else{$state.realSnapshotsExpected=0}
 if($RealOnly){$snapshotArgs+=@('--real-only')}
 $oracleInput=Join-Path $out 'original.tsv.gz' 
 $original=Join-Path $work 'original'
 New-Item -ItemType Directory $original|Out-Null
 Copy-Item SDK/tools/*.dll $original
 & $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Web.Extensions.dll /out:"$original/AudioXmaDctOracle.exe" (Resolve-Path recovery/tests/AudioXmaDctOracle.cs).Path *> "$out/oracle-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/oracle-build.log";throw 'XMA table-backed DCT-IV oracle host build failed'}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$original/AudioXmaDctOracle.exe.config"
 & "$original/AudioXmaDctOracle.exe" original "$original/BinaryAssetBuilder.AudioCompiler.dll" $oracleInput @snapshotArgs *> "$out/original-cpu.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/original-cpu.log";throw 'Actual original CPU DCT oracle failed'}
 Get-Content "$out/original-cpu.log"
 $state.oracleSha256=(Get-FileHash $oracleInput).Hash.ToLowerInvariant()
 $state.originalCpuValidated=$true
 Copy-Item "$original/AudioXmaDctOracle.exe*" $work
 if(Test-Path (Join-Path $work 'BinaryAssetBuilder.AudioCompiler.dll')){throw 'Original DLL copied into candidate host'}
 foreach($mode in @('Debug','Release')) {
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  $library=Join-Path $work "Recovered.Audio.XmaDct.$mode.dll"
  & $csc /nologo /target:library /platform:x86 $opt /out:"$library" (Resolve-Path src/AudioCompiler/Native/XmaComplexFft.cs).Path (Resolve-Path src/AudioCompiler/Native/XmaDct4.cs).Path *> "$out/$mode-build.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode-build.log";throw "$mode XMA table-backed DCT-IV source build failed"}
  & "$work/AudioXmaDctOracle.exe" candidate $library $oracleInput @snapshotArgs *> "$out/$mode.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode.log";throw "$mode XMA table-backed DCT-IV results differ from original"}
  Get-Content "$out/$mode.log"
  $state.modes+=@{configuration=$mode;passed=$true;summary=(Get-Content "$out/$mode.log" -Tail 1);records=$state.recordsPerConfiguration;candidateSafetyRejections=9;recoveredSha256=(Get-FileHash $library).Hash.ToLowerInvariant();originalAbsentFromRecoveredHost=(!(Test-Path (Join-Path $work 'BinaryAssetBuilder.AudioCompiler.dll')))}
 }
 if($state.realSnapshotsExpected -gt 0){foreach($configuration in @('original-cpu','Debug','Release')){if(-not (Select-String -Path "$out/$configuration.log" -SimpleMatch ("realSnapshots={0};" -f $state.realSnapshotsExpected))){throw "Real snapshot count missing in $configuration"}}}
 $state.realSnapshotsReplayed=$state.realSnapshotsExpected
 $state.realSnapshotInputsAndOutputsVerified=($state.realSnapshotsExpected -gt 0)
 $state.status='passed' 
}catch{$state.status='failed';$state.error=$_.Exception.Message;throw}
finally{$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json"}
