param([string]$OutputDirectory,[string]$RawEvidenceDirectory='recovery/evidence/audio/xma-raw-validation/40232525056-1')
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if(!$env:RUNNER_TEMP){$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
if(!$OutputDirectory){$OutputDirectory="recovery/evidence/audio/xma-metadata-state-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if(Test-Path $OutputDirectory){throw "Refusing to overwrite actual XMA metadata state evidence: $OutputDirectory"}
New-Item -ItemType Directory $OutputDirectory -Force|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-xma-metadata-state-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $work|Out-Null
$csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='Actual original XMA metadata state captured by native-only entry journal, replayed against original codec EA/seek allocations and isolated candidate';xmaEncodingRecovered=$false;sdkValidated=$false;audioFileXmaRouteEnabled=$false;actualWrapperStates=0;syntheticStateRecords=0;modes=@()}
try {
 $baseline=(Resolve-Path $RawEvidenceDirectory).Path
 $receipt=Get-Content "$baseline/validation.json" -Raw|ConvertFrom-Json
 $rawPath=Join-Path $baseline 'raw-codec.json'
 if($receipt.status -ne 'original-evidence-only' -or !$receipt.rawBeforeOuterRewriter -or $receipt.cases -ne 8 -or $receipt.nativeEncodeCalls -ne 16 -or (Get-FileHash $rawPath).Hash.ToLowerInvariant() -ne $receipt.rawCodecReceiptSha256){throw 'Accepted raw baseline provenance differs'}
 $state.baselineSourceCommit=$receipt.sourceCommit;$state.baselineRunId=$receipt.runId;$state.baselineRunAttempt=$receipt.runAttempt;$state.baselineReceiptSha256=$receipt.rawCodecReceiptSha256
 $raw=Get-Content $rawPath -Raw|ConvertFrom-Json
 if($raw.originalDllSha256 -ne 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139' -or $raw.caseCount -ne 8 -or !$raw.rawBeforeOuterRewriter){throw 'Accepted original codec fingerprint differs'}
 foreach($case in $raw.cases){
  foreach($entry in $case.files){$path=Join-Path (Join-Path "$baseline/cases" $case.name) $entry.path;if((Get-Item $path).Length -ne $entry.length -or (Get-FileHash $path).Hash.ToLowerInvariant() -ne $entry.sha256){throw 'Accepted raw baseline file fingerprint differs'}}
 }
 $original=Join-Path $work 'original';New-Item -ItemType Directory $original|Out-Null;Copy-Item SDK/tools/*.dll $original
 $originalDll=Join-Path $original 'BinaryAssetBuilder.AudioCompiler.dll'
 & $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Web.Extensions.dll /main:AudioXmaMetadataStateOracle /out:"$original/AudioXmaMetadataStateOracle.exe" (Resolve-Path recovery/tests/AudioXmaMetadataStateOracle.cs).Path (Resolve-Path recovery/tests/AudioXmaRawOracle.cs).Path (Resolve-Path recovery/tests/AudioXmaCodecMetadataOracle.cs).Path *> "$out/oracle-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/oracle-build.log";throw 'Actual metadata state oracle build failed'}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$original/AudioXmaMetadataStateOracle.exe.config"
 & "$original/AudioXmaMetadataStateOracle.exe" original $originalDll $baseline $out *> "$out/original-cpu.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/original-cpu.log";throw 'Original actual-state capture/replay failed'}
 Get-Content "$out/original-cpu.log"
 $captured=Get-Content "$out/actual-states.json" -Raw|ConvertFrom-Json
 if($captured.status -ne 'original-evidence-only' -or $captured.actualWrapperStates -ne 8 -or $captured.nativeEncodeCalls -ne 8 -or !$captured.entryRestoredBeforeDirectReplays -or $captured.instrumentation.managedCallbacks -ne 0 -or $captured.instrumentation.usesVeh){throw 'Actual-state capture provenance incomplete'}
 foreach($case in $captured.cases){
  if(!$case.actualStateCapturedBeforeMetadataBuilder -or !$case.originalOutputsEqualAcceptedBaseline -or !$case.eaSeekEqualActualCodecAllocations){throw 'Actual-state real codec closed loop missing'}
  foreach($entry in $case.files){$path=Join-Path (Join-Path "$out/cases" $case.name) $entry.path;if((Get-Item $path).Length -ne $entry.length -or (Get-FileHash $path).Hash.ToLowerInvariant() -ne $entry.sha256){throw 'Captured state/output fingerprint differs'}}
 }
 $state.actualWrapperStates=8;$state.originalCpuValidated=$true;$state.originalCodecAllocationsUnchanged=$true;$state.originalMetadataReplayMatchesRealCodecAllocations=$true
 $transcript=Join-Path $out 'actual-states.tsv.gz';$state.oracleSha256=(Get-FileHash $transcript).Hash.ToLowerInvariant()
 Copy-Item "$original/AudioXmaMetadataStateOracle.exe*" $work
 foreach($mode in @('Debug','Release')){
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'};$library=Join-Path $work "Recovered.Audio.XmaMetadataActual.$mode.dll"
  & $csc /nologo /target:library /platform:x86 $opt /out:"$library" (Resolve-Path src/AudioCompiler/Native/XmaCodecMetadata.cs).Path *> "$out/$mode-build.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode-build.log";throw "$mode actual-state candidate build failed"}
  & "$work/AudioXmaMetadataStateOracle.exe" candidate $library $transcript *> "$out/$mode.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode.log";throw "$mode actual-state metadata differs from original codec outputs"}
  Get-Content "$out/$mode.log"
  $state.modes+=@{configuration=$mode;passed=$true;summary=(Get-Content "$out/$mode.log" -Tail 1);actualWrapperStates=8;recoveredSha256=(Get-FileHash $library).Hash.ToLowerInvariant();originalAbsentFromRecoveredHost=$true}
 }
 $state.status='passed'
}catch{$state.status='failed';$state.error=$_.Exception.Message;throw}
finally{$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json"}
