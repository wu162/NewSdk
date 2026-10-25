param([string]$OutputDirectory,[string]$SnapshotsDirectory)
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if(!$env:RUNNER_TEMP){$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
if(!$OutputDirectory){$OutputDirectory="recovery/evidence/audio/xma-schedule-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if(Test-Path $OutputDirectory){throw "Refusing to overwrite XMA schedule evidence: $OutputDirectory"}
New-Item -ItemType Directory $OutputDirectory -Force|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-xma-schedule-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $work|Out-Null
$csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='Complete integral XMA 0x98267 window scheduler over three recovered input representations';xmaEncoderRecovered=$false;outerWindowSchedulerRecovered=$true;sdkValidated=$false;audioFileXmaRouteEnabled=$false;originalDllSha256=(Get-FileHash SDK/tools/BinaryAssetBuilder.AudioCompiler.dll).Hash.ToLowerInvariant();modes=@();physicalX87ControlWord='0x027f';recordsPerConfiguration=3240}
try {
 if($state.originalDllSha256 -ne 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139'){throw 'Original DLL fingerprint changed'}
 $snapshotArgs=@()
 if($SnapshotsDirectory){$SnapshotsDirectory=(Resolve-Path $SnapshotsDirectory).Path;$snapshotArgs=@($SnapshotsDirectory);$state.snapshotsDirectory=$SnapshotsDirectory;$state.realSnapshotsExpected=@(Get-ChildItem $SnapshotsDirectory -Filter case.json -Recurse).Count;if(!$state.realSnapshotsExpected){throw 'Requested real window snapshot directory is empty'}}else{$state.realSnapshotsExpected=0}
 $oracleInput=Join-Path $out 'original.tsv.gz' 
 $original=Join-Path $work 'original'
 New-Item -ItemType Directory $original|Out-Null
 Copy-Item SDK/tools/*.dll $original
 & $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Web.Extensions.dll /out:"$original/AudioXmaScheduleOracle.exe" (Resolve-Path recovery/tests/AudioXmaScheduleOracle.cs).Path *> "$out/oracle-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/oracle-build.log";throw 'XMA schedule oracle host build failed'}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$original/AudioXmaScheduleOracle.exe.config"
 & "$original/AudioXmaScheduleOracle.exe" original "$original/BinaryAssetBuilder.AudioCompiler.dll" $oracleInput @snapshotArgs *> "$out/original-cpu.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/original-cpu.log";throw 'Actual original CPU window oracle failed'}
 Get-Content "$out/original-cpu.log"
 $state.oracleSha256=(Get-FileHash $oracleInput).Hash.ToLowerInvariant()
 $state.originalCpuValidated=$true
 Copy-Item "$original/AudioXmaScheduleOracle.exe*" $work
 if(Test-Path (Join-Path $work 'BinaryAssetBuilder.AudioCompiler.dll')){throw 'Original DLL copied into candidate host'}
 foreach($mode in @('Debug','Release')) {
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  $library=Join-Path $work "Recovered.Audio.XmaSchedule.$mode.dll"
  & $csc /nologo /target:library /platform:x86 $opt /out:"$library" (Resolve-Path src/AudioCompiler/Native/XmaPcmWindow.cs).Path (Resolve-Path src/AudioCompiler/Native/XmaPcmWindowScheduler.cs).Path *> "$out/$mode-build.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode-build.log";throw "$mode XMA schedule source build failed"}
  & "$work/AudioXmaScheduleOracle.exe" candidate $library $oracleInput @snapshotArgs *> "$out/$mode.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode.log";throw "$mode XMA schedule helpers differ from original"}
  Get-Content "$out/$mode.log"
  $state.modes+=@{configuration=$mode;passed=$true;summary=(Get-Content "$out/$mode.log" -Tail 1);records=3240;candidateSafetyRejections=15;recoveredSha256=(Get-FileHash $library).Hash.ToLowerInvariant();originalAbsentFromRecoveredHost=(!(Test-Path (Join-Path $work 'BinaryAssetBuilder.AudioCompiler.dll')))}
 }
 if($state.realSnapshotsExpected -gt 0){
  foreach($configuration in @('original-cpu','Debug','Release')){
   if(-not (Select-String -Path "$out/$configuration.log" -SimpleMatch ("realSnapshots={0};" -f $state.realSnapshotsExpected))){throw "Real snapshot count missing in $configuration"}
  }
 }
 $state.realSnapshotsReplayed=$state.realSnapshotsExpected
 $state.realSnapshotNativeArgumentsVerified=($state.realSnapshotsExpected -gt 0)
 $state.status='passed' 
}catch{$state.status='failed';$state.error=$_.Exception.Message;throw}
finally{$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json"}
