param([string]$OutputDirectory,[string]$CorpusDirectory='recovery/evidence/audio/xma-corpus-validation/40233712043-1')
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if(!$env:RUNNER_TEMP){$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
if(!$OutputDirectory){$OutputDirectory="recovery/evidence/audio/xma-raw-packet-state-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if(Test-Path $OutputDirectory){throw 'Refusing to overwrite actual raw packet state evidence'}
New-Item -ItemType Directory $OutputDirectory -Force|Out-Null
$out=(Resolve-Path $OutputDirectory).Path;$baseline=(Resolve-Path $CorpusDirectory).Path
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='Actual raw XMA packet helper entry-return states from two accepted long corpus encodes; isolated candidate replay';actualEncoderStates=0;syntheticStateRecords=0;xmaEncoderRecovered=$false;sdkValidated=$false;audioFileXmaRouteEnabled=$false;modes=@()}
try {
 $receipt=Get-Content "$baseline/validation.json" -Raw|ConvertFrom-Json
 if($receipt.sourceCommit -ne '579c95661d90f067ab414c4133f033c2571b96e1' -or [string]$receipt.runId -ne '40233712043' -or [int]$receipt.runAttempt -ne 1 -or $receipt.status -ne 'original-evidence-only' -or $receipt.cases -ne 8 -or $receipt.nativeEncodeCalls -ne 16 -or $receipt.multiPacketCases -le 0 -or (Get-FileHash "$baseline/corpus.json").Hash.ToLowerInvariant() -ne $receipt.corpusSha256){throw 'Accepted long corpus provenance differs'}
 $corpus=Get-Content "$baseline/corpus.json" -Raw|ConvertFrom-Json
 if($corpus.originalDllSha256 -ne 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139'){throw 'Accepted long corpus codec fingerprint differs'}
 foreach($name in @('seeded-wide-stereo-ram','seeded-wide-stereo-stream')){
  $case=@($corpus.cases|Where-Object {$_.name -eq $name});if($case.Count -ne 1){throw 'Accepted long corpus case missing'}
  $localCase=Get-Content (Join-Path (Join-Path "$baseline/cases" $name) 'case.json') -Raw|ConvertFrom-Json
  foreach($field in @('name','channels','frames','sampleRate','blockBytes')){if($localCase.$field -ne $case[0].$field){throw 'Case metadata differs from pinned corpus manifest'}}
  if(@($localCase.files).Count -ne @($case[0].files).Count){throw 'Case file mapping differs from pinned corpus manifest'}
  foreach($entry in $case[0].files){$mapped=@($localCase.files|Where-Object {$_.path -eq $entry.path});if($mapped.Count -ne 1 -or $mapped[0].bytes -ne $entry.bytes -or $mapped[0].sha256 -ne $entry.sha256){throw 'Case file fingerprint mapping differs from pinned corpus manifest'}}
  foreach($entry in $case[0].files){$path=Join-Path (Join-Path "$baseline/cases" $name) $entry.path;if((Get-Item $path).Length -ne $entry.bytes -or (Get-FileHash $path).Hash.ToLowerInvariant() -ne $entry.sha256){throw 'Accepted long corpus bytes differ'}}
 }
 $state.baselineSourceCommit=$receipt.sourceCommit;$state.baselineRunId=$receipt.runId;$state.baselineRunAttempt=$receipt.runAttempt;$state.baselineCorpusSha256=$receipt.corpusSha256
 $work=Join-Path $env:RUNNER_TEMP ('audio-xma-raw-packet-state-'+[Guid]::NewGuid().ToString('N'));$original=Join-Path $work 'original';New-Item -ItemType Directory $original -Force|Out-Null;Copy-Item SDK/tools/*.dll $original
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 & $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Web.Extensions.dll /main:AudioXmaRawPacketStateOracle /out:"$original/AudioXmaRawPacketStateOracle.exe" (Resolve-Path recovery/tests/AudioXmaRawPacketStateOracle.cs).Path (Resolve-Path recovery/tests/AudioXmaRawOracle.cs).Path *> "$out/oracle-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/oracle-build.log";throw 'Actual raw packet state host build failed'}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$original/AudioXmaRawPacketStateOracle.exe.config"
 & "$original/AudioXmaRawPacketStateOracle.exe" original "$original/BinaryAssetBuilder.AudioCompiler.dll" $baseline $out *> "$out/original-cpu.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/original-cpu.log";throw 'Actual original raw packet state capture failed'}
 Get-Content "$out/original-cpu.log"
 $captured=Get-Content "$out/packet-state.json" -Raw|ConvertFrom-Json
 if($captured.status -ne 'original-evidence-only' -or $captured.nativeEncodeCalls -ne 2 -or $captured.headerStates -le 0 -or $captured.nextPacketStates -le 0 -or !$captured.entryPointsRestored -or $captured.instrumentation.managedCallbacks -ne 0 -or $captured.instrumentation.usesVeh){throw 'Actual packet state journal provenance incomplete'}
 foreach($case in $captured.cases){if(!$case.threeAllocationsEqualAcceptedBaseline -or !$case.inputEqualAcceptedBaseline){throw 'Instrumentation changed accepted corpus behavior'}}
 $state.actualEncoderStates=$captured.actualEncoderStates;$state.headerStates=$captured.headerStates;$state.nextPacketStates=$captured.nextPacketStates;$state.paddingStates=$captured.paddingStates;$state.originalCpuValidated=$true;$state.originalCodecAllocationsUnchanged=$true
 $transcript=Join-Path $out 'actual-packet-states.tsv.gz';$state.oracleSha256=(Get-FileHash $transcript).Hash.ToLowerInvariant()
 Copy-Item "$original/AudioXmaRawPacketStateOracle.exe*" $work
 foreach($mode in @('Debug','Release')){
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'};$library=Join-Path $work "Recovered.Audio.XmaRawPacketActual.$mode.dll"
  & $csc /nologo /target:library /platform:x86 $opt /out:"$library" (Resolve-Path src/AudioCompiler/Native/XmaRawPacketWriter.cs).Path *> "$out/$mode-build.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode-build.log";throw "$mode actual raw packet candidate build failed"}
  & "$work/AudioXmaRawPacketStateOracle.exe" candidate $library $transcript *> "$out/$mode.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode.log";throw "$mode actual raw packet states differ"}
  Get-Content "$out/$mode.log"
  $state.modes+=@{configuration=$mode;passed=$true;summary=(Get-Content "$out/$mode.log" -Tail 1);actualEncoderStates=$captured.actualEncoderStates;recoveredSha256=(Get-FileHash $library).Hash.ToLowerInvariant();originalAbsentFromRecoveredHost=$true}
 }
 $state.status='passed'
}catch{$state.status='failed';$state.error=$_.Exception.Message;throw}
finally{$state|ConvertTo-Json -Depth 9|Set-Content "$out/validation.json"}
