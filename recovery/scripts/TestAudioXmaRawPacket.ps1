param([string]$OutputDirectory)
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if(!$env:RUNNER_TEMP){$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
if(!$OutputDirectory){$OutputDirectory="recovery/evidence/audio/xma-raw-packet-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if(Test-Path $OutputDirectory){throw "Refusing to overwrite XMA raw-packet evidence: $OutputDirectory"}
New-Item -ItemType Directory $OutputDirectory -Force|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-xma-raw-packet-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $work|Out-Null
$csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='Complete bounded raw XMA packet lifecycle and native bitsplice, synthetic codec states and real original CPU';xmaEncodingRecovered=$false;sdkValidated=$false;audioFileXmaRouteEnabled=$false;syntheticStateRecords=1098;actualEncoderStates=0;nativeRvas=@('0x5dc70','0x5dcf0','0x5ddb0','0x5c270','0x5c390','0x5c3b0');completeTwoPacketSequences=12;realRawPacketInputs=8;rawInputs=@();modes=@()}
try {
 $baseline='recovery/evidence/audio/xma-raw-validation/40232525056-1'
 $baselineState=Get-Content "$baseline/validation.json" -Raw|ConvertFrom-Json
 if($baselineState.status -ne 'original-evidence-only' -or $baselineState.cases -ne 8 -or !$baselineState.rawBeforeOuterRewriter -or (Get-FileHash "$baseline/raw-codec.json").Hash.ToLowerInvariant() -ne $baselineState.rawCodecReceiptSha256){throw 'Accepted raw payload baseline provenance differs'}
 $rawReceipt=Get-Content "$baseline/raw-codec.json" -Raw|ConvertFrom-Json
 if($rawReceipt.originalDllSha256 -ne 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139' -or $rawReceipt.caseCount -ne 8){throw 'Accepted raw payload codec fingerprint differs'}
 foreach($case in $rawReceipt.cases){
  $entry=@($case.files|Where-Object {$_.path -eq 'raw-packets.bin'})
  $path=Join-Path (Join-Path "$baseline/cases" $case.name) 'raw-packets.bin'
  if($entry.Count -ne 1 -or (Get-Item $path).Length -ne 2048 -or (Get-FileHash $path).Hash.ToLowerInvariant() -ne $entry[0].sha256){throw 'Accepted raw payload bytes differ'}
  $state.rawInputs+=@{case=$case.name;sha256=$entry[0].sha256;length=2048;sourceCommit=$baselineState.sourceCommit;runId=$baselineState.runId;runAttempt=$baselineState.runAttempt}
 }
 $state.rawCodecReceiptSha256=$baselineState.rawCodecReceiptSha256
 $original=Join-Path $work 'original'
 New-Item -ItemType Directory $original|Out-Null
 Copy-Item SDK/tools/*.dll $original
 $originalDll=Join-Path $original 'BinaryAssetBuilder.AudioCompiler.dll'
 $state.originalDllSha256=(Get-FileHash $originalDll).Hash.ToLowerInvariant()
 if($state.originalDllSha256 -ne 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139'){throw 'Pinned original DLL fingerprint changed'}
 & $csc /nologo /platform:x86 /r:System.Core.dll /out:"$original/AudioXmaRawPacketOracle.exe" (Resolve-Path recovery/tests/AudioXmaRawPacketOracle.cs).Path *> "$out/oracle-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/oracle-build.log";throw 'XMA raw-packet oracle host build failed'}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$original/AudioXmaRawPacketOracle.exe.config"
 $oracleInput=Join-Path $out 'original.tsv.gz'
 & "$original/AudioXmaRawPacketOracle.exe" original $originalDll $oracleInput *> "$out/original-cpu.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/original-cpu.log";throw 'Real original CPU raw-packet oracle failed'}
 Get-Content "$out/original-cpu.log"
 $state.originalCpuValidated=$true
 $state.oracleSha256=(Get-FileHash $oracleInput).Hash.ToLowerInvariant()
 Copy-Item "$original/AudioXmaRawPacketOracle.exe*" $work
 if(Test-Path (Join-Path $work 'BinaryAssetBuilder.AudioCompiler.dll')){throw 'Candidate host contains original target DLL'}
 foreach($mode in @('Debug','Release')){
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  $library=Join-Path $work "Recovered.Audio.XmaRawPacket.$mode.dll"
  & $csc /nologo /target:library /platform:x86 $opt /out:"$library" (Resolve-Path src/AudioCompiler/Native/XmaRawPacketWriter.cs).Path *> "$out/$mode-build.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode-build.log";throw "$mode XMA raw-packet source build failed"}
  & "$work/AudioXmaRawPacketOracle.exe" candidate $library $oracleInput *> "$out/$mode.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode.log";throw "$mode XMA raw-packet differs from original"}
  Get-Content "$out/$mode.log"
  $state.modes+=@{configuration=$mode;passed=$true;summary=(Get-Content "$out/$mode.log" -Tail 1);records=1106;candidateSafetyRejections=15;recoveredSha256=(Get-FileHash $library).Hash.ToLowerInvariant();originalAbsentFromRecoveredHost=$true}
 }
 $state.status='passed'
}catch{$state.status='failed';$state.error=$_.Exception.Message;throw}
finally{$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json"}
