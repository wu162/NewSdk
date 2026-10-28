param([switch]$OnlyAmd,[switch]$Patched,[switch]$OnlyS3,[switch]$X87)
$ErrorActionPreference='Stop'
if($X87 -and !$OnlyS3){throw 'X87 experiment must select the complete S3 family'}
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/public-codecs/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-public-codecs-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$family=if($OnlyS3){'s3'}elseif($Patched){'patched'}elseif($OnlyAmd){'amd'}else{'stb-squish-icbc'}
$fpMode=if($X87){'IA32-x87-scalar'}else{'SSE2'}
$architecture=@('/arch:SSE2');if($X87){$architecture=@('/arch:IA32','/DSQUISH_USE_SSE=0')}
$r=@{floatingPointMode=$fpMode;candidateFamily=$family;sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';comparisonOnly=$true;productionAccepted=$false;sdkValidated=$false;scope='copied public DXT codec complete original10240-block differential; selected candidate family only, full payload/RGB/alpha counts, Debug/Release and original DLL absent/unloaded';configurations=@();eligibleCandidates=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 45;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^(PASS|COMPARE) '}}
try{
 $manifest=Get-Content recovery/reference/public-codecs/PROVENANCE.json -Raw|ConvertFrom-Json
 foreach($file in $manifest.files){if((Get-FileHash $file.path).Hash.ToLowerInvariant() -ne $file.sha256 -or (git hash-object $file.path).Trim() -ne $file.blobSha){throw "Upstream source fingerprint mismatch $($file.path)"}}
 $r.upstreamSources=$manifest
 $block='recovery/evidence/texture/dxt-blocks/40500898688-1';$blockReceipt=Get-Content "$block/validation.json" -Raw|ConvertFrom-Json
 if($blockReceipt.status -ne 'passed' -or $blockReceipt.sourceCommit -ne 'b02787056a655a9710117e05e4b771740d3b67fe'){throw 'Wrong complete original DXT block baseline'}
 git merge-base --is-ancestor $blockReceipt.sourceCommit HEAD;if($LASTEXITCODE -ne 0){throw 'Block baseline not ancestor'}
 Expand-Archive "$block/transcripts.zip" "$work/blocks"
 # Debug and Release must have saved the exact same immutable original payloads.
 if((Get-FileHash "$work/blocks/Debug-transcript.txt").Hash -ne (Get-FileHash "$work/blocks/Release-transcript.txt").Hash){throw 'Original block transcripts differ by configuration'}
 $r.blockBaseline=@{sourceCommit=$blockReceipt.sourceCommit;runId=$blockReceipt.runId;sha256=(Get-FileHash "$work/blocks/Debug-transcript.txt").Hash.ToLowerInvariant()}
 $legacy='recovery/evidence/texture/dxt-blocks/40470267556-1';Expand-Archive "$legacy/samples.zip" "$work/legacy"
 $jpeg='recovery/evidence/texture/jpeg-load/40625499544-1';$jpegReceipt=Get-Content "$jpeg/validation.json" -Raw|ConvertFrom-Json
 if($jpegReceipt.status -ne 'probe-only' -or $jpegReceipt.fixtureCount -ne 1284 -or (Get-FileHash "$jpeg/samples.zip").Hash.ToLowerInvariant() -ne $jpegReceipt.samplesSha256){throw 'Wrong original JPEG corpus'}
 git merge-base --is-ancestor $jpegReceipt.sourceCommit HEAD;if($LASTEXITCODE -ne 0){throw 'JPEG baseline not ancestor'}
 Expand-Archive "$jpeg/samples.zip" "$work/jpeg"
 $r.jpegBaseline=@{sourceCommit=$jpegReceipt.sourceCommit;runId=$jpegReceipt.runId;sha256=$jpegReceipt.samplesSha256}
 Get-ChildItem SDK/tools/*.dll|Where-Object Name -ne 'BinaryAssetBuilder.TextureCompiler.dll'|Copy-Item -Destination $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 $sources=@('recovery/tests/TexturePublicCodecsReplay.cs','src/TextureCompiler/Managed/TexturePacker.cs','src/TextureCompiler/Managed/TextureNpotRgba8Resampler.cs','src/TextureCompiler/Managed/TextureNpotStorageResampler.cs')|ForEach-Object{(Resolve-Path $_).Path}
 $squish=(Resolve-Path recovery/reference/public-codecs/squish).Path
 $stb=(Resolve-Path recovery/reference/public-codecs/stb).Path;$nvtt=(Resolve-Path recovery/reference/public-codecs/nvtt).Path
 $amd=(Resolve-Path recovery/reference/public-codecs/amd).Path;$compat=(Resolve-Path recovery/tests/public-amd-compat).Path
 $s3=(Resolve-Path recovery/reference/public-codecs/s3).Path
 $native=@((Resolve-Path recovery/tests/TexturePublicS3Bridge.cpp).Path,(Resolve-Path recovery/reference/public-codecs/s3/s3_quant.cpp).Path,(Resolve-Path recovery/tests/TexturePublicCodecBridge.cpp).Path,(Resolve-Path recovery/tests/TexturePublicAmdBridge.cpp).Path,(Resolve-Path recovery/reference/public-codecs/amd/compressonatorxcodec.cpp).Path)+@(Get-ChildItem "$squish/*.cpp"|ForEach-Object FullName)
 foreach($mode in @('Debug','Release')){
  $build=Join-Path $work $mode;New-Item -ItemType Directory -Force $build|Out-Null
  $opt=if($mode -eq 'Release'){'/O2'}else{'/Od'};$managedOpt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  Checked "$mode-native" {Push-Location $build;try{& cl /nologo /LD /MT /EHsc /std:c++17 @architecture /fp:strict $opt /I"$squish" /I"$stb" /I"$nvtt" /I"$compat" /I"$amd" /I"$s3" @native /link /OUT:"$work/TexturePublicCodecs.dll" /MACHINE:X86}finally{Pop-Location}}
  $bytes=[IO.File]::ReadAllBytes("$work/TexturePublicCodecs.dll");$pe=[BitConverter]::ToInt32($bytes,0x3c);if([BitConverter]::ToUInt16($bytes,$pe+4) -ne 0x14c){throw 'Public codec DLL must be x86'}
  Checked "$mode-host-build" {& $csc /nologo /platform:x86 $managedOpt /r:"$work/BinaryAssetBuilder.Core.dll" /r:"$work/BinaryAssetBuilder.Utility.dll" /out:"$work/TexturePublicCodecsReplay.exe" $sources}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TexturePublicCodecsReplay.exe.config"
  New-Item -ItemType Directory -Force "$out/$mode"|Out-Null
  Checked "$mode-replay" {& "$work/TexturePublicCodecsReplay.exe" "$work/blocks/Debug-transcript.txt" "$work/legacy" "$work/jpeg" "$out/$mode" $family}
  Compress-Archive "$out/$mode/dxt-blocks.bin" "$out/$mode/dxt-blocks.zip"
  Remove-Item "$out/$mode/dxt-blocks.bin"
  $rows=Import-Csv "$out/$mode/dxt-summary.tsv" -Delimiter "`t";if($rows.Count -ne $(if($OnlyS3){32}elseif($Patched){84}elseif($OnlyAmd){24}else{64})){throw 'Incomplete candidate matrix'}
  $r.configurations+=@{configuration=$mode;comparisonCompleted=$true;withoutOriginalTextureCompiler=$true;nativeSha256=(Get-FileHash "$work/TexturePublicCodecs.dll").Hash.ToLowerInvariant();candidates=@($rows)}
 }
 foreach($row in $r.configurations[0].candidates){
  $other=@($r.configurations[1].candidates|Where-Object{ $_.backend -eq $row.backend -and $_.variant -eq $row.variant -and $_.format -eq $row.format })
  if($row.eligibleForIntegration -eq 'True' -and $other.Count -eq 1 -and $other[0].eligibleForIntegration -eq 'True'){$r.eligibleCandidates+=@{backend=$row.backend;variant=$row.variant;format=$row.format;blockAcceptanceOnly=$true;sdkStillRequired=$true}}
 }
 $r.status='comparison-completed'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 16|Set-Content "$out/validation.json"}
