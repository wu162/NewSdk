$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/png-metadata/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-png-metadata-replay-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$false;sdkValidated=$false;scope='PC PNG ICC/APNG default image and sRGB/gAMA precedence; full immutable RGBA/errors through direct/public decoder; original absent/unloaded per call; plugin/SDK separate';configurations=@();originalBaselines=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 40;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS '}}
try{
 $baselines=@(
  @{name='metadata';path='recovery/evidence/texture/png-metadata-load/40779895712-1';source='c812b8094a032dba0a29475d90e563cef211116e';count=4680;decoded=3120;rejected=1560;hash='5e698ae274086468e3efa5520847fa9127acc4de62c1b96060a5ac0cbab0cac6'},
  @{name='order';path='recovery/evidence/texture/png-metadata-order/40780410178-1';source='2cab493118bb33992a762ee191af392d10fe30eb';count=3240;decoded=2160;rejected=1080;hash='e804e2d5099f73c6be676a87153b02806fc0237827dc0da657c48503cdbd964f'}
 )
 foreach($b in $baselines){
  $receipt=Get-Content "$($b.path)/validation.json" -Raw|ConvertFrom-Json
  if($receipt.status -ne 'probe-only' -or $receipt.sourceCommit -ne $b.source -or $receipt.fixtureCount -ne $b.count -or $receipt.decodedCount -ne $b.decoded -or $receipt.rejectedCount -ne $b.rejected){throw 'Wrong immutable PNG metadata corpus'}
  if($receipt.originalSha256 -ne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e' -or $receipt.samplesSha256 -ne $b.hash -or (Get-FileHash "$($b.path)/samples.zip").Hash.ToLowerInvariant() -ne $b.hash){throw 'Original metadata baseline hash changed'}
  git merge-base --is-ancestor $receipt.sourceCommit HEAD;if($LASTEXITCODE -ne 0){throw 'Baseline source is not an ancestor'}
  $r.originalBaselines+=@{sourceCommit=$receipt.sourceCommit;runId=$receipt.runId;samplesSha256=$receipt.samplesSha256;fixtureCount=$receipt.fixtureCount}
  Expand-Archive "$($b.path)/samples.zip" "$work/$($b.name)"
 }
 Get-ChildItem SDK/tools/*.dll|Where-Object Name -ne 'BinaryAssetBuilder.TextureCompiler.dll'|Copy-Item -Destination $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 foreach($mode in @('Debug','Release')){
  Checked "$mode-managed-build" {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c $mode -o "$work/build-$mode" --nologo}
  Copy-Item "$work/build-$mode/Recovered.BinaryAssetBuilder.TextureCompiler.dll" $work -Force
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  Checked "$mode-replay-build" {& $csc /nologo /platform:x86 $opt /r:"$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /out:"$work/TexturePngMetadataReplay.exe" (Resolve-Path recovery/tests/TexturePngMetadataReplay.cs).Path}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TexturePngMetadataReplay.exe.config"
  foreach($b in $baselines){Checked "$mode-$($b.name)-replay" {& "$work/TexturePngMetadataReplay.exe" "$work/$($b.name)" "$($b.count)"}}
  $r.configurations+=@{configuration=$mode;passed=$true;withoutOriginalTextureCompiler=$true;summary=@(Get-Content "$out/$mode-*-replay.log"|Where-Object{$_ -match '^PASS '})}
 }
 $r.status='passed'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"}
