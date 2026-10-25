param([string]$OutputDirectory,[string]$RawEvidenceDirectory)
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if(!$env:RUNNER_TEMP){$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
if(!$OutputDirectory){$OutputDirectory="recovery/evidence/audio/xma-rewrite-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if(Test-Path $OutputDirectory){throw "Refusing to overwrite XMA output rewrite evidence: $OutputDirectory"}
New-Item -ItemType Directory $OutputDirectory -Force|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-xma-rewrite-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $work|Out-Null
$csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='Bounded XMA raw-packet to EA-output rewriter RVA 0xee00 only; real original CPU full-write comparison';xmaEncoderRecovered=$false;sdkValidated=$false;audioFileXmaRouteEnabled=$false;syntheticRecords=793;rawCodecBlocks=0;historicalPluginClosedLoopBlocks=0;modes=@();inputs=@()}
try {
 $original=Join-Path $work 'original'
 New-Item -ItemType Directory $original|Out-Null
 Copy-Item SDK/tools/*.dll $original
 $originalDll=Join-Path $original 'BinaryAssetBuilder.AudioCompiler.dll'
 $state.originalDllSha256=(Get-FileHash $originalDll).Hash.ToLowerInvariant()
 if($state.originalDllSha256 -ne 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139'){throw 'Pinned original DLL fingerprint changed'}
 & $csc /nologo /platform:x86 /r:System.Core.dll /out:"$original/AudioXmaRewriteOracle.exe" (Resolve-Path recovery/tests/AudioXmaRewriteOracle.cs).Path *> "$out/oracle-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/oracle-build.log";throw 'XMA rewrite oracle host build failed'}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$original/AudioXmaRewriteOracle.exe.config"
 $rawManifest=Join-Path $out 'raw-inputs.tsv'
 $rawLines=New-Object 'System.Collections.Generic.List[string]'
 $rawLines.Add('# case-block streamCount rateFlags rawPacketBytesBase64 historicalPluginInnerEaBase64; raw bytes from pinned native codec, before RVA ee00')
 if($RawEvidenceDirectory){
  $rawRoot=(Resolve-Path $RawEvidenceDirectory).Path
  $receipt=Get-Content (Join-Path $rawRoot 'validation.json') -Raw|ConvertFrom-Json
  if($receipt.status -ne 'original-evidence-only' -or !$receipt.rawBeforeOuterRewriter -or $receipt.cases -ne 8 -or $receipt.nativeEncodeCalls -ne 16 -or $receipt.xmaEncodingRecovered){throw 'Incomplete or overstated original raw codec receipt'}
  $rawReceiptPath=Join-Path $rawRoot 'raw-codec.json'
  if((Get-FileHash $rawReceiptPath).Hash.ToLowerInvariant() -ne $receipt.rawCodecReceiptSha256){throw 'Raw codec receipt fingerprint differs'}
  $rawReceipt=Get-Content $rawReceiptPath -Raw|ConvertFrom-Json
  if($rawReceipt.status -ne 'original-evidence-only' -or !$rawReceipt.rawBeforeOuterRewriter -or $rawReceipt.caseCount -ne 8 -or $rawReceipt.nativeEncodeCalls -ne 16 -or $rawReceipt.xmaEncodingRecovered -or $rawReceipt.originalDllSha256 -ne $state.originalDllSha256){throw 'Raw codec provenance does not match the pinned native oracle'}
  $state.rawCodecReceiptSha256=$receipt.rawCodecReceiptSha256
  $state.rawSourceCommit=$receipt.sourceCommit
  $state.rawRunId=$receipt.runId
  $state.rawRunAttempt=$receipt.runAttempt
  $historicalPath=(Resolve-Path 'recovery/evidence/audio/plugin-validation/40199000936-1/xma-original.txt').Path
  $historicalText=[IO.File]::ReadAllText($historicalPath).Replace("`r`n","`n")
  $historyHash=[Security.Cryptography.SHA256]::Create()
  try{$historicalSha=([BitConverter]::ToString($historyHash.ComputeHash([Text.Encoding]::UTF8.GetBytes($historicalText)))).Replace('-','').ToLowerInvariant()}finally{$historyHash.Dispose()}
  if($historicalSha -ne 'e7b2befa6c59fc0cce64ed7959d1b65f83cb1adeed5e6cb100758d68583a35d0'){throw 'Historical original plugin transcript fingerprint differs'}
  $historicalLines=$historicalText.Split("`n")
  $state.historicalPluginTranscriptSha256=$historicalSha
  $state.historicalPluginRunId='40199000936'
  foreach($case in $rawReceipt.cases){
   if(!$case.deterministic -or $case.repeatCount -ne 2 -or $case.streamCount -lt 1 -or $case.blockBytes -lt 2048 -or $case.blockBytes % 2048 -ne 0){throw 'Invalid or unconfirmed raw codec metadata'}
   $raw=Get-Item (Join-Path (Join-Path (Join-Path $rawRoot 'cases') $case.name) 'raw-packets.bin')
   $rawFile=@($case.files|Where-Object {$_.path -eq 'raw-packets.bin'})
   if($rawFile.Count -ne 1 -or $raw.Length -ne $rawFile[0].length -or (Get-FileHash $raw.FullName).Hash.ToLowerInvariant() -ne $rawFile[0].sha256){throw 'Raw packet bytes differ from the pinned codec receipt'}
   $bytes=[IO.File]::ReadAllBytes($raw.FullName)
   if($bytes.Length -eq 0 -or $bytes.Length % 2048 -ne 0){throw 'Raw codec output is not an integral positive packet block'}
   if($bytes.Length -ne 2048 -or $case.sampleRate -ne 22050){throw 'Historical plugin closed loop currently pins the eight original single-packet cases'}
   $label='xma-evidence-'+$case.channels+'-'+$case.frames+'-'+([bool]$case.streamed).ToString()
   $historicalCase=@($historicalLines|Where-Object {$_.StartsWith($label+':')})
   if($historicalCase.Count -ne 1){throw 'Historical original plugin case missing or duplicated'}
   $customAt=$historicalCase[0].IndexOf(':CUSTOM:')
   if($customAt -lt 0){throw 'Historical CustomData missing'}
   $custom=[Convert]::FromBase64String($historicalCase[0].Substring($customAt+8))
   $innerOffset=if($case.streamed){8}else{16}
   $innerLength=(([uint32]$custom[$innerOffset] -shl 24) -bor ([uint32]$custom[$innerOffset+1] -shl 16) -bor ([uint32]$custom[$innerOffset+2] -shl 8) -bor [uint32]$custom[$innerOffset+3]) -shr 2
   if($innerLength -lt 8 -or $innerOffset+$innerLength -gt $custom.Length){throw 'Historical inner EA block exceeds CustomData'}
   $historicalEa=New-Object byte[] $innerLength
   [Array]::Copy($custom,$innerOffset,$historicalEa,0,$innerLength)
   $flags=if($case.sampleRate -le 24000){0}elseif($case.sampleRate -le 32000){1}elseif($case.sampleRate -le 44100){2}else{3}
   for($offset=0;$offset -lt $bytes.Length;$offset+=$case.blockBytes){
    $length=[Math]::Min([int]$case.blockBytes,$bytes.Length-$offset)
    $block=New-Object byte[] $length
    [Array]::Copy($bytes,$offset,$block,0,$length)
    $rawLines.Add(($raw.Directory.Name+'-block'+$offset+"`t"+$case.streamCount+"`t"+$flags+"`t"+[Convert]::ToBase64String($block)+"`t"+[Convert]::ToBase64String($historicalEa)))
    $state.rawCodecBlocks++
    $state.historicalPluginClosedLoopBlocks++
   }
   $state.inputs+=@{case=$raw.Directory.Name;rawPacketSha256=(Get-FileHash $raw.FullName).Hash.ToLowerInvariant();streamCount=$case.streamCount;blockBytes=$case.blockBytes;sampleRate=$case.sampleRate;sourceCommit=$receipt.sourceCommit;runId=$receipt.runId;runAttempt=$receipt.runAttempt}
  }
 }
 [IO.File]::WriteAllLines($rawManifest,$rawLines,[Text.Encoding]::ASCII)
 $oracleInput=Join-Path $out 'original.tsv.gz'
 & "$original/AudioXmaRewriteOracle.exe" original $originalDll $oracleInput $rawManifest *> "$out/original-cpu.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/original-cpu.log";throw 'Real original CPU rewrite oracle failed'}
 Get-Content "$out/original-cpu.log"
 $state.originalCpuValidated=$true
 $state.oracleSha256=(Get-FileHash $oracleInput).Hash.ToLowerInvariant()
 Copy-Item "$original/AudioXmaRewriteOracle.exe*" $work
 if(Test-Path (Join-Path $work 'BinaryAssetBuilder.AudioCompiler.dll')){throw 'Candidate host contains original target DLL'}
 foreach($mode in @('Debug','Release')){
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  $library=Join-Path $work "Recovered.Audio.XmaRewrite.$mode.dll"
  & $csc /nologo /target:library /platform:x86 $opt /out:"$library" (Resolve-Path src/AudioCompiler/Native/XmaOutputRewriter.cs).Path *> "$out/$mode-build.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode-build.log";throw "$mode XMA rewrite source build failed"}
  & "$work/AudioXmaRewriteOracle.exe" candidate $library $oracleInput *> "$out/$mode.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode.log";throw "$mode XMA rewrite differs from original"}
  Get-Content "$out/$mode.log"
  $state.modes+=@{configuration=$mode;passed=$true;summary=(Get-Content "$out/$mode.log" -Tail 1);records=($state.syntheticRecords+$state.rawCodecBlocks);candidateSafetyRejections=12;recoveredSha256=(Get-FileHash $library).Hash.ToLowerInvariant();originalAbsentFromRecoveredHost=$true}
 }
 $state.status='passed'
}catch{$state.status='failed';$state.error=$_.Exception.Message;throw}
finally{$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json"}
