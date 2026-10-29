$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/tga-metadata/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-tga-metadata-replay-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$false;sdkValidated=$false;scope='PC TGA extension/gamma/alpha/developer/tables/origin complete immutable original bytes/errors; direct/public TGA decoding, per-call NoOriginal; plugin/SDK separate';configurations=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 40;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS '}}
try{
 $loadEvidence='recovery/evidence/texture/tga-metadata-load/40793184986-1'
 $receipt=Get-Content "$loadEvidence/validation.json" -Raw|ConvertFrom-Json
 if($receipt.status -ne 'probe-only' -or $receipt.sourceCommit -ne '63dcfd2b527284ff153e07e7badc997a3d2bfb49' -or $receipt.fixtureCount -ne 12480 -or $receipt.decodedCount -ne 12480 -or $receipt.rejectedCount -ne 0){throw 'Wrong immutable original TGA metadata corpus'}
 if($receipt.originalSha256 -ne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e' -or $receipt.samplesSha256 -ne 'e55d7f704fd1d592593843e7444fa231ff05a3824ea3895a08ba50bb4b149993' -or (Get-FileHash "$loadEvidence/samples.zip").Hash.ToLowerInvariant() -ne 'e55d7f704fd1d592593843e7444fa231ff05a3824ea3895a08ba50bb4b149993'){throw 'Original metadata baseline hash changed'}
 git merge-base --is-ancestor $receipt.sourceCommit HEAD;if($LASTEXITCODE -ne 0){throw 'Baseline source is not an ancestor'}
 $r.originalBaseline=@{sourceCommit=$receipt.sourceCommit;runId=$receipt.runId;samplesSha256=$receipt.samplesSha256}
 Expand-Archive "$loadEvidence/samples.zip" "$work/samples"
 Get-ChildItem SDK/tools/*.dll|Where-Object Name -ne 'BinaryAssetBuilder.TextureCompiler.dll'|Copy-Item -Destination $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 foreach($mode in @('Debug','Release')){
  Checked "$mode-managed-build" {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c $mode -o "$work/build-$mode" --nologo}
  Copy-Item "$work/build-$mode/Recovered.BinaryAssetBuilder.TextureCompiler.dll" $work -Force
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  Checked "$mode-replay-build" {& $csc /nologo /platform:x86 $opt /r:"$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /out:"$work/TextureTgaMetadataReplay.exe" (Resolve-Path recovery/tests/TextureTgaMetadataReplay.cs).Path}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureTgaMetadataReplay.exe.config"
  Checked "$mode-replay" {& "$work/TextureTgaMetadataReplay.exe" "$work/samples" 12480}
  $r.configurations+=@{configuration=$mode;passed=$true;withoutOriginalTextureCompiler=$true;summary=@(Get-Content "$out/$mode-replay.log"|Where-Object{$_ -match '^PASS '})}
 }
 $r.status='passed'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"}
