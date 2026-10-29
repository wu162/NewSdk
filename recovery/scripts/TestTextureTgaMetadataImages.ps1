$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/tga-metadata-images/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-colour-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$false;sdkValidated=$false;scope='actual Texture/OnDemandTexture TGA extension/gamma/alpha/developer/tables/origin; PC POT/NPOT raw3/DXT3/5 standard/volume full mip; three buffers/full cdata and NoOriginal; SDK separate';configurations=@();nativeBinaries=@()}
$r.sourceFiles=@(Get-ChildItem src/TextureCompiler/Colour -File|Sort-Object FullName|ForEach-Object{@{path=[IO.Path]::GetRelativePath($root,$_.FullName).Replace('\','/');sha256=(Get-FileHash $_.FullName).Hash.ToLowerInvariant()}})
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 40;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS '}}
try {
 $baseline='recovery/evidence/texture/tga-metadata-load/40793184986-1'
 $receipt=Get-Content "$baseline/validation.json" -Raw|ConvertFrom-Json
 $hash='e55d7f704fd1d592593843e7444fa231ff05a3824ea3895a08ba50bb4b149993'
 if($receipt.status -ne 'probe-only' -or $receipt.sourceCommit -ne '63dcfd2b527284ff153e07e7badc997a3d2bfb49' -or $receipt.fixtureCount -ne 12480 -or $receipt.decodedCount -ne 12480 -or $receipt.rejectedCount -ne 0 -or $receipt.originalSha256 -ne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e' -or $receipt.samplesSha256 -ne $hash -or (Get-FileHash "$baseline/samples.zip").Hash.ToLowerInvariant() -ne $hash){throw 'Wrong immutable TGA metadata consumer source corpus'}
 git merge-base --is-ancestor $receipt.sourceCommit HEAD;if($LASTEXITCODE -ne 0){throw 'Original TGA source is not ancestor'}
 $r.originalBaseline=@{sourceCommit=$receipt.sourceCommit;runId=$receipt.runId;samplesSha256=$hash}
 Expand-Archive "$baseline/samples.zip" "$work/samples"
 Copy-Item SDK/tools/*.dll $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 foreach($mode in @('Debug','Release')) {
  Checked "$mode-asm" {& ml /nologo /c /coff /safeseh /Fo"$work/colour-$mode.obj" (Resolve-Path src/TextureCompiler/Colour/TextureColour.asm).Path}
  $nativeOpt=if($mode -eq 'Release'){'/O2'}else{'/Od'}
  Checked "$mode-native" {& cl /nologo /LD /MT /fp:strict /GS- $nativeOpt /Fo"$work/bridge-$mode.obj" (Resolve-Path src/TextureCompiler/Colour/TextureColourBridge.cpp).Path "$work/colour-$mode.obj" /link /NODEFAULTLIB /NOENTRY /OUT:"$work/Recovered.TextureDxtColour.dll" /IMPLIB:"$work/colour-$mode.lib"}
  Checked "$mode-public-s3" {& "$PSScriptRoot/BuildTexturePublicS3.ps1" -Destination $work -Configuration $mode}
  $r.publicS3Native=@($r.publicS3Native|Where-Object{$_})+@((Get-Content "$work/PublicS3-native.json" -Raw|ConvertFrom-Json))
  $bytes=[IO.File]::ReadAllBytes("$work/Recovered.TextureDxtColour.dll");$pe=[BitConverter]::ToInt32($bytes,0x3c)
  if([BitConverter]::ToUInt16($bytes,$pe+4) -ne 0x14c -or [BitConverter]::ToUInt32($bytes,$pe+40) -ne 0 -or [BitConverter]::ToUInt32($bytes,$pe+128) -ne 0){throw 'Owned colour DLL must be x86 with no entry point or imports'}
  $r.nativeBinaries+=@{configuration=$mode;sha256=(Get-FileHash "$work/Recovered.TextureDxtColour.dll").Hash.ToLowerInvariant();machine='I386';entryPoint=0;importDirectory=0}
  Checked "$mode-managed-build" {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c $mode -o "$work/build-$mode" --nologo}
  Copy-Item "$work/build-$mode/*.dll" $work -Force
  Checked "$mode-alpha-native" {& cl /nologo /LD /MT /fp:strict /GS- $nativeOpt /Fo"$work/alpha-$mode.obj" (Resolve-Path src/TextureCompiler/Dxt/TextureAlphaQuantizer.cpp).Path /link /NODEFAULTLIB /NOENTRY /OUT:"$work/Recovered.TextureDxtAlpha.dll" /IMPLIB:"$work/alpha-$mode.lib"}
  $dll="$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll"
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  Checked "$mode-oracle-build" {& $csc /nologo /unsafe /platform:x86 $opt /main:TextureTgaMetadataPipelineOracle /r:System.Drawing.dll /r:System.Xml.dll /r:$dll /r:"$work/BinaryAssetBuilder.Core.dll" /r:"$work/BinaryAssetBuilder.Utility.dll" /out:"$work/TextureTgaMetadataPipelineOracle.exe" (Resolve-Path recovery/tests/TextureTgaMetadataPipelineOracle.cs).Path (Resolve-Path recovery/tests/TextureTgaMetadataFixtures.cs).Path}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureTgaMetadataPipelineOracle.exe.config"
  Checked "$mode-oracle" {& "$work/TextureTgaMetadataPipelineOracle.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" $dll "$work/fixtures-$mode" "$out/$mode-corpus.replay" "$work/samples"}
  $isolated=Join-Path $work "isolated-$mode";New-Item -ItemType Directory -Force $isolated|Out-Null
  Get-ChildItem "$work/*.dll"|Where-Object{$_.Name -ne 'BinaryAssetBuilder.TextureCompiler.dll'}|Copy-Item -Destination $isolated
  Copy-Item "$work/TextureTgaMetadataPipelineOracle.exe","$work/TextureTgaMetadataPipelineOracle.exe.config" $isolated
  Checked "$mode-no-original" {& "$isolated/TextureTgaMetadataPipelineOracle.exe" "$isolated/Recovered.BinaryAssetBuilder.TextureCompiler.dll" "$isolated/fixtures" "$out/$mode-corpus.replay"}
  $r.configurations+=@{configuration=$mode;passed=$true;withoutOriginalTextureCompiler=$true;summary=@(Get-Content "$out/$mode-oracle.log","$out/$mode-no-original.log"|Where-Object{$_ -match '^PASS '})}
 }
 $files=@(Get-ChildItem "$out/*-corpus.replay");Compress-Archive -Path $files.FullName -DestinationPath "$out/corpus.zip";$files|Remove-Item
 $r.status='passed'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"}

