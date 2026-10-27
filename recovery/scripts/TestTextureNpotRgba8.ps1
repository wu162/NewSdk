$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/npot-rgba8/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-npot-rgba8-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$false;sdkValidated=$false;scope='isolated PC D3DX RGBA8 NPOT triangle component only; immutable original LoadImageA corpus and coefficient bits; plugin/SDK NPOT remains rejected';configurations=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 40;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS '}}
try{
 $loadEvidence='recovery/evidence/texture/npot-load/40528237009-1'
 $numericEvidence='recovery/evidence/texture/npot-numeric/40529918922-1'
 $loadReceipt=Get-Content "$loadEvidence/validation.json" -Raw|ConvertFrom-Json
 $numericReceipt=Get-Content "$numericEvidence/validation.json" -Raw|ConvertFrom-Json
 if($loadReceipt.status -ne 'probe-only' -or $loadReceipt.sourceCommit -ne 'a19bb879704cd80ad55202760a160c8af57bbe2d' -or $loadReceipt.runId -ne '40528237009' -or $loadReceipt.fixtureCount -ne 1012 -or $loadReceipt.decodedCount -ne 1012){throw 'Wrong immutable original load corpus'}
 if($numericReceipt.status -ne 'probe-only' -or $numericReceipt.sourceCommit -ne '678db0127a2ea3620700b44e46093dd6dc2050d1' -or $numericReceipt.runId -ne '40529918922'){throw 'Wrong immutable original numeric corpus'}
 if((Get-FileHash "$loadEvidence/samples.zip").Hash.ToLowerInvariant() -ne $loadReceipt.samplesSha256){throw 'Original samples archive hash changed'}
 foreach($receipt in @($loadReceipt,$numericReceipt)){
  if($receipt.originalSha256 -ne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'){throw 'Original baseline DLL hash changed'}
  git merge-base --is-ancestor $receipt.sourceCommit HEAD;if($LASTEXITCODE -ne 0){throw 'Baseline source is not an ancestor'}
 }
 $weightFile="$numericEvidence/tables/weights.tsv";$weightRecord=@($numericReceipt.tableFiles|Where-Object name -eq 'weights.tsv');if($weightRecord.Count -ne 1 -or (Get-FileHash $weightFile).Hash.ToLowerInvariant() -ne $weightRecord[0].sha256){throw 'Original coefficient transcript hash changed'}
 $r.originalBaselines=@(@{sourceCommit=$loadReceipt.sourceCommit;runId=$loadReceipt.runId;samplesSha256=$loadReceipt.samplesSha256},@{sourceCommit=$numericReceipt.sourceCommit;runId=$numericReceipt.runId;weightsSha256=$weightRecord[0].sha256})
 Expand-Archive "$loadEvidence/samples.zip" "$work/samples"
 Get-ChildItem SDK/tools/*.dll|Where-Object Name -ne 'BinaryAssetBuilder.TextureCompiler.dll'|Copy-Item -Destination $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 foreach($mode in @('Debug','Release')){
  Checked "$mode-managed-build" {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c $mode -o "$work/build-$mode" --nologo}
  Copy-Item "$work/build-$mode/Recovered.BinaryAssetBuilder.TextureCompiler.dll" $work -Force
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  Checked "$mode-replay-build" {& $csc /nologo /platform:x86 $opt /r:"$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /out:"$work/TextureNpotRgba8Replay.exe" (Resolve-Path recovery/tests/TextureNpotRgba8Replay.cs).Path}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureNpotRgba8Replay.exe.config"
  Checked "$mode-replay" {& "$work/TextureNpotRgba8Replay.exe" "$work/samples" (Resolve-Path $weightFile).Path}
  $r.configurations+=@{configuration=$mode;passed=$true;withoutOriginalTextureCompiler=$true;summary=@(Get-Content "$out/$mode-replay.log"|Where-Object{$_ -match '^PASS '})}
 }
 $r.status='passed'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"}
