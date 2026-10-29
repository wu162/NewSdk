$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/bmp-headers/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-bmp-headers-replay-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$false;sdkValidated=$false;scope='PC BMP core/DIB/OS2/gamma/ICC/gaps complete immutable original bytes/errors; direct/public BMP decoding, per-call NoOriginal; 1896 original decode/432 original rejection matches and 288 explicit unrecovered bitfields refusals; plugin/SDK separate';configurations=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 40;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS '}}
try{
 $loadEvidence='recovery/evidence/texture/bmp-header-load/40799686293-1'
 $receipt=Get-Content "$loadEvidence/validation.json" -Raw|ConvertFrom-Json
 if($receipt.status -ne 'probe-only' -or $receipt.sourceCommit -ne '511896747c32c7863078b36ad781caae4f3763c1' -or $receipt.fixtureCount -ne 2616 -or $receipt.decodedCount -ne 2184 -or $receipt.rejectedCount -ne 432 -or $receipt.unstableOriginals -ne 49){throw 'Wrong immutable original BMP header corpus'}
 if($receipt.originalSha256 -ne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e' -or $receipt.samplesSha256 -ne '08715c9547eb6f9fb8ba1745ab3f38b954522439848caa031889c2f73deb069b' -or (Get-FileHash "$loadEvidence/samples.zip").Hash.ToLowerInvariant() -ne '08715c9547eb6f9fb8ba1745ab3f38b954522439848caa031889c2f73deb069b'){throw 'Original metadata baseline hash changed'}
 git merge-base --is-ancestor $receipt.sourceCommit HEAD;if($LASTEXITCODE -ne 0){throw 'Baseline source is not an ancestor'}
 $r.originalBaseline=@{sourceCommit=$receipt.sourceCommit;runId=$receipt.runId;samplesSha256=$receipt.samplesSha256}
 Expand-Archive "$loadEvidence/samples.zip" "$work/samples"
 Get-ChildItem SDK/tools/*.dll|Where-Object Name -ne 'BinaryAssetBuilder.TextureCompiler.dll'|Copy-Item -Destination $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 foreach($mode in @('Debug','Release')){
  Checked "$mode-managed-build" {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c $mode -o "$work/build-$mode" --nologo}
  Copy-Item "$work/build-$mode/Recovered.BinaryAssetBuilder.TextureCompiler.dll" $work -Force
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  Checked "$mode-replay-build" {& $csc /nologo /platform:x86 $opt /r:"$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /out:"$work/TextureBmpHeaderReplay.exe" (Resolve-Path recovery/tests/TextureBmpHeaderReplay.cs).Path}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureBmpHeaderReplay.exe.config"
  Checked "$mode-replay" {& "$work/TextureBmpHeaderReplay.exe" "$work/samples" 2616}
  $r.configurations+=@{configuration=$mode;passed=$true;withoutOriginalTextureCompiler=$true;summary=@(Get-Content "$out/$mode-replay.log"|Where-Object{$_ -match '^PASS '})}
 }
 $r.status='passed'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"}
