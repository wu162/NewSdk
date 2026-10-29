param([int]$TimeoutSeconds=300)
$Profile='PcBmpHeaders'
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$evidenceFolder='sdk-bmp-headers'
$out="recovery/evidence/texture/$evidenceFolder/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-sdk-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work | Out-Null
$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';sdkValidated=$false;scope='Dedicated PC BMP core/OS2/V2/V3/V4/V5 BI_RGB and metadata MOD: immutable POT/NPOT raw3/DXT3/5 standard/volume complete mip, transparent DXT1A, and BMP atlas base/normal; three modes/four outputs/complete actual traces; not full production MOD';profile='PcBmpHeaders';targetPlatform='Win32';modes=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 60;throw "$name failed"}}
try{
 Checked recovered-build {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c Release -o "$work/build" --nologo}
 Checked colour-asm {& ml /nologo /c /coff /safeseh /Fo"$work/colour.obj" (Resolve-Path src/TextureCompiler/Colour/TextureColour.asm).Path}
 Checked colour-native {& cl /nologo /LD /MT /fp:strict /GS- /O2 /Fo"$work/bridge.obj" (Resolve-Path src/TextureCompiler/Colour/TextureColourBridge.cpp).Path "$work/colour.obj" /link /NODEFAULTLIB /NOENTRY /OUT:"$work/Recovered.TextureDxtColour.dll" /IMPLIB:"$work/colour.lib"}
 Checked public-s3 {& "$PSScriptRoot/BuildTexturePublicS3.ps1" -Destination $work -Configuration 'Release'}
  $r.publicS3Native=@($r.publicS3Native|Where-Object{$_})+@((Get-Content "$work/PublicS3-native.json" -Raw|ConvertFrom-Json))
 Checked alpha-native {& cl /nologo /LD /MT /fp:strict /GS- /O2 /Fo"$work/alpha.obj" (Resolve-Path src/TextureCompiler/Dxt/TextureAlphaQuantizer.cpp).Path /link /NODEFAULTLIB /NOENTRY /OUT:"$work/Recovered.TextureDxtAlpha.dll" /IMPLIB:"$work/alpha.lib"}
 $r.nativeBinaries=@()
 foreach($name in @('Recovered.TextureDxtColour.dll','Recovered.TextureDxtAlpha.dll')){
  $bytes=[IO.File]::ReadAllBytes("$work/$name");$pe=[BitConverter]::ToInt32($bytes,0x3c)
  if([BitConverter]::ToUInt16($bytes,$pe+4) -ne 0x14c -or [BitConverter]::ToUInt32($bytes,$pe+40) -ne 0 -or [BitConverter]::ToUInt32($bytes,$pe+128) -ne 0){throw 'Owned native helpers must be x86 with no entry point or imports'}
  $r.nativeBinaries+=@{name=$name;sha256=(Get-FileHash "$work/$name").Hash.ToLowerInvariant();machine='I386';entryPoint=0;importDirectory=0}
 }
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 Copy-Item SDK/tools/*.dll $work
 Copy-Item "$work/build/Recovered.BinaryAssetBuilder.TextureCompiler.dll" $work
 Checked asset-id-build {& $csc /nologo /platform:x86 /r:"$work/BinaryAssetBuilder.Core.dll" /out:"$work/TextureSdkAssetIds.exe" (Resolve-Path recovery/tests/TextureSdkAssetIds.cs).Path}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureSdkAssetIds.exe.config"
 $baseline='recovery/evidence/texture/bmp-header-load/40799686293-1'
 $receipt=Get-Content "$baseline/validation.json" -Raw|ConvertFrom-Json
 $hash='08715c9547eb6f9fb8ba1745ab3f38b954522439848caa031889c2f73deb069b'
 if($receipt.status -ne 'probe-only' -or $receipt.sourceCommit -ne '511896747c32c7863078b36ad781caae4f3763c1' -or $receipt.fixtureCount -ne 2616 -or $receipt.decodedCount -ne 2184 -or $receipt.rejectedCount -ne 432 -or $receipt.unstableOriginals -ne 49 -or $receipt.originalSha256 -ne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e' -or $receipt.samplesSha256 -ne $hash -or (Get-FileHash "$baseline/samples.zip").Hash.ToLowerInvariant() -ne $hash){throw 'Wrong immutable BMP header SDK source corpus'}
 git merge-base --is-ancestor $receipt.sourceCommit HEAD;if($LASTEXITCODE -ne 0){throw 'BMP source not ancestor'}
 Expand-Archive "$baseline/samples.zip" "$work/samples";New-Item -ItemType Directory -Force "$work/fixtures"|Out-Null
 foreach($row in Import-Csv "$work/samples/transcript.tsv" -Delimiter "`t"){
  # Every supported DIB/depth/direction/metadata/gap combination at both focused
  # POT/NPOT shapes. Rejections and all bitfields are outside this positive SDK.
  if($row.compression -eq '0' -and $row.label -match '-(16x8|7x5)-top' -and ($row.label -match '-p0-' -or ($row.depth -eq '32' -and $row.profile -eq 'plain' -and $row.gap -eq '0'))){
   if($row.error -ne '' -or $row.originalStable -ne 'True'){throw 'BMP SDK focused input is not a repeat-equal original success'}
   $file="$work/samples/$($row.label).bmp";if((Get-FileHash $file).Hash.ToLowerInvariant() -ne $row.inputSha256){throw 'BMP input hash mismatch'};Copy-Item $file "$work/fixtures"
  }
 }
 if(@(Get-ChildItem "$work/fixtures" -File).Count -ne 356){throw 'Expected356 immutable BMP header SDK inputs'}
 $r.originalBaseline=@{sourceCommit=$receipt.sourceCommit;runId=$receipt.runId;samplesSha256=$hash;selected=356;selection='compression0,16x8-or-7x5,pattern0 plus32bit/plain/gap0 all alpha patterns; all DIB/depth/direction/profile/gap combinations'}
 Checked fixture-writer-build {& $csc /nologo /unsafe /platform:x86 /r:System.Drawing.dll /r:"$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /main:TextureBmpHeaderSdkFixtures /out:"$work/TextureBmpHeaderSdkFixtures.exe" (Resolve-Path recovery/tests/TextureBmpHeaderSdkFixtures.cs).Path (Resolve-Path recovery/tests/TextureBmpHeaderConsumerFixtures.cs).Path (Resolve-Path recovery/tests/TextureBmpHeaderFixtures.cs).Path (Resolve-Path recovery/tests/TextureBmpOracle.cs).Path}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureBmpHeaderSdkFixtures.exe.config"
 Checked fixture-write {& "$work/TextureBmpHeaderSdkFixtures.exe" "$work/generated" (Resolve-Path recovery/reference/compact-icc).Path}
 $wrapper=(Resolve-Path recovery/tests/TextureDxtSdkTracePlugin.cs).Path
 foreach($mode in @('Original','Recovered','NoOriginal')){
  $sdk=Join-Path $work $mode;Copy-Item SDK $sdk -Recurse
  Remove-Item "$sdk/builtmods" -Recurse -Force -ErrorAction SilentlyContinue
  $moddir=Join-Path $sdk 'Mods\RecoveryTexture';New-Item -ItemType Directory -Force "$moddir/Data","$moddir/Art"|Out-Null
  $nodes=@();$index=0
  foreach($fixture in Get-ChildItem "$work/fixtures" -File|Sort-Object Name){
   Copy-Item $fixture.FullName "$moddir/Art/$($fixture.Name)";$name=$fixture.Name
   foreach($kind in @('StandardTexture','VolumeTexture')){foreach($generate in @('false','true')){foreach($type in @('Texture','OnDemandTexture')){foreach($format in @('A8R8G8B8','X8R8G8B8','R5G6B5','DXT3','DXT5')){
    $nodes+="<$type id='BmpHeader_$index' File='..\Art\$name' Type='$kind' OutputFormat='$format' GenerateMipMaps='$generate' AllowAutomaticResize='false' />";$index++
   }}}}
  }
  if($nodes.Count -ne 14240){throw 'Expected14240 BMP headers raw/DXT3/5 assets'};$r.textureAssets=14240
  foreach($fixture in Get-ChildItem "$work/generated" -File -Filter 'transparent-*'|Sort-Object Name){
   Copy-Item $fixture.FullName "$moddir/Art/$($fixture.Name)";$name=$fixture.Name
   foreach($kind in @('StandardTexture','VolumeTexture')){foreach($generate in @('false','true')){foreach($type in @('Texture','OnDemandTexture')){
    $nodes+="<$type id='BmpHeader_$index' File='..\Art\$name' Type='$kind' OutputFormat='DXT1A' GenerateMipMaps='$generate' AllowAutomaticResize='false' />";$index++
   }}}
  }
  if($nodes.Count -ne 14752){throw 'Expected512 BMP headers transparent DXT1A assets'};$r.transparentDxt1aAssets=512
  foreach($fixture in Get-ChildItem "$work/generated" -File -Filter 'atlas-*'|Sort-Object Name){
   # The original packer requires "tga" in BOTH path strings, independently
   # of file magic. Keep the immutable/generated BMP payload, with a legal
   # atlas path; renaming does not convert or replace the image bytes.
   $name=[IO.Path]::ChangeExtension($fixture.Name,'.tga');Copy-Item $fixture.FullName "$moddir/Art/$name"
   foreach($normal in @('missing-normal.tga',$name)){
    $nodes+="<TerrainTextureAtlas id='BmpHeaderAtlas_$index' AtlasSize='2048' AllowLossyCompression='true'><Tile TextureID='42' BaseTexture='..\Art\$name' NormalTexture='..\Art\$normal' /></TerrainTextureAtlas>";$index++
   }
  }
  $expectedAssets=$nodes.Count;if($expectedAssets -ne 14852){throw 'Expected14852 BMP headers texture/atlas assets'};$r.atlasAssets=100

  "<?xml version='1.0'?><AssetDeclaration xmlns='uri:ea.com:eala:asset'><Tags />$($nodes -join "`n")</AssetDeclaration>"|Set-Content "$moddir/Data/Mod.xml" -Encoding utf8
  Checked "$mode-asset-ids" {& "$work/TextureSdkAssetIds.exe" "$moddir/Data/Mod.xml" "$out/$mode-asset-ids.tsv" $expectedAssets}
  $idHash=(Get-FileHash "$out/$mode-asset-ids.tsv").Hash.ToLowerInvariant()
  $modHash=(Get-FileHash "$moddir/Data/Mod.xml").Hash.ToLowerInvariant()
  if($mode -eq 'Original'){$r.assetIdManifestSha256=$idHash;$r.inputModSha256=$modHash}
  elseif($idHash -ne $r.assetIdManifestSha256 -or $modHash -ne $r.inputModSha256){throw 'Three SDK modes must use identical checked IDs and input XML'}
  $trace="$out/$mode-trace";New-Item -ItemType Directory -Force $trace|Out-Null
  $cfg="$sdk/tools/BinaryAssetBuilder.exe.config";[xml]$xml=Get-Content $cfg -Raw
  $plugin=@($xml.configuration.assetbuilder.plugins.plugin|Where-Object {$_.assetTypes.Split(',') -contains 'Texture'})
  if($plugin.Count -ne 1){throw 'Texture plugin route is ambiguous'}
  if($mode -eq 'Original'){
   Checked "$mode-wrapper-build" {& $csc /nologo /target:library /platform:x86 /define:ORIGINAL /r:"$sdk/tools/BinaryAssetBuilder.Core.dll" /r:"$sdk/tools/BinaryAssetBuilder.Utility.dll" /r:"$sdk/tools/BinaryAssetBuilder.TextureCompiler.dll" /out:"$sdk/tools/TextureSdkTrace.dll" $wrapper}
  }else{
   $wrapperDefines=@();if($mode -eq 'NoOriginal'){$wrapperDefines=@('/define:NO_ORIGINAL')}
   Copy-Item "$work/build/Recovered.BinaryAssetBuilder.TextureCompiler.dll","$work/Recovered.TextureDxtColour.dll","$work/Recovered.TextureDxtAlpha.dll","$work/Recovered.TexturePublicS3.dll" "$sdk/tools"
   Checked "$mode-wrapper-build" {& $csc /nologo /target:library /platform:x86 $wrapperDefines /r:"$sdk/tools/BinaryAssetBuilder.Core.dll" /r:"$sdk/tools/BinaryAssetBuilder.Utility.dll" /r:"$sdk/tools/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /out:"$sdk/tools/TextureSdkTrace.dll" $wrapper}
   if($mode -eq 'NoOriginal'){Remove-Item "$sdk/tools/BinaryAssetBuilder.TextureCompiler.dll" -Force}
  }
  $plugin[0].SetAttribute('targetType','TextureSdkTracePlugin, TextureSdkTrace');$plugin[0].SetAttribute('useBuildCache','false')
  $startup=$xml.CreateElement('startup');$startup.SetAttribute('useLegacyV2RuntimeActivationPolicy','true')
  $runtime=$xml.CreateElement('supportedRuntime');$runtime.SetAttribute('version','v4.0');[void]$startup.AppendChild($runtime);[void]$xml.configuration.AppendChild($startup);$xml.Save($cfg)
  Copy-Item $cfg "$out/$mode.config"
  if($mode -eq 'Original'){Copy-Item "$moddir/Data/Mod.xml" "$out/input-Mod.xml";$r.fixtureFiles=@(Get-ChildItem "$moddir/Art" -File|Sort-Object Name|ForEach-Object {@{name=$_.Name;bytes=$_.Length;sha256=(Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()}})}
  if($mode -eq 'NoOriginal' -and (Test-Path "$sdk/tools/BinaryAssetBuilder.TextureCompiler.dll")){throw 'Original TextureCompiler present in isolated SDK'}
  $env:RA3_TEXTURE_SDK_TRACE=$trace
  $output=Join-Path $sdk 'builtmods';New-Item -ItemType Directory -Force $output|Out-Null
  $mod=Join-Path $moddir 'Data\Mod.xml'
  $arguments=@("`"$mod`"","/od:`"$output`"","/iod:`"$output`"","/tp:$($r.targetPlatform)",'/csc:false','/ls:true','/osh:false','/pc:true','/res:true','/slowclean:true','/ss:true','/art:".;.\Mods\RecoveryTexture\Art;.\Mods;.\Art"','/audio:".;.\Mods\RecoveryTexture\Audio;.\Mods;.\Audio"','/data:".;.\Mods\RecoveryTexture\Data;.\Mods;.\SageXml"')
  $p=Start-Process "$sdk/tools/BinaryAssetBuilder.exe" -ArgumentList ($arguments -join ' ') -WorkingDirectory $sdk -PassThru -RedirectStandardOutput "$out/$mode-stdout.log" -RedirectStandardError "$out/$mode-stderr.log"
  if(!$p.WaitForExit($TimeoutSeconds*1000)){Stop-Process -Id $p.Id -Force;throw "$mode SDK timeout"};$p.Refresh()
  if($p.ExitCode -ne 0){Get-Content "$out/$mode-stdout.log" -Tail 70;Get-Content "$out/$mode-stderr.log" -Tail 30;throw "$mode SDK exit=$($p.ExitCode)"}
  $files=@()
  foreach($ext in @('bin','manifest','imp','relo')){
   $f="$output/mods/recoverytexture/data/mod.$ext";if(!(Test-Path $f)){Get-ChildItem $output -File -Recurse | Select-Object -First 25 FullName | Out-String | Write-Output;throw "$mode missing $f"}
   Copy-Item $f "$out/$mode-mod.$ext";$files+=@{file="mod.$ext";bytes=(Get-Item $f).Length;sha256=(Get-FileHash $f -Algorithm SHA256).Hash.ToLowerInvariant()}
  }
  $calls=Import-Csv "$trace/texture.calls.tsv" -Delimiter "`t"
  if(@($calls|Where-Object Stage -eq 'failure').Count -ne 0){throw "$mode plugin failure trace"}
  $success=@($calls|Where-Object Stage -eq 'success');$unique=@($success|ForEach-Object {"$($_.TypeId):$($_.InstanceId)"}|Sort-Object -Unique)
  if($unique.Count -ne $expectedAssets -or $success.Count -lt $expectedAssets){throw "$mode expected $expectedAssets unique actual Core texture calls; found $($unique.Count)"}
  $r.modes+=@{mode=$mode;passed=$true;uniqueAssets=$unique.Count;actualCalls=$success.Count;withoutOriginalTextureCompiler=($mode -eq 'NoOriginal');assertedOriginalNotLoadedDuringEveryCall=($mode -eq 'NoOriginal');files=$files}
  Write-Output "PASS $mode real SDK: assets=$($unique.Count) calls=$($success.Count)"
 }
 foreach($mode in @('Recovered','NoOriginal')){
  $expectedNames=@(Get-ChildItem "$out/Original-trace" -File|ForEach-Object Name|Sort-Object)
  $actualNames=@(Get-ChildItem "$out/$mode-trace" -File|ForEach-Object Name|Sort-Object)
  if(@(Compare-Object $expectedNames $actualNames).Count -ne 0){throw "$mode complete trace file set differs"}
  foreach($ext in @('bin','manifest','imp','relo')){if((Get-FileHash "$out/Original-mod.$ext").Hash -ne (Get-FileHash "$out/$mode-mod.$ext").Hash){throw "$mode SDK mod.$ext differs"}}
  foreach($file in Get-ChildItem "$out/Original-trace" -File){
   $other="$out/$mode-trace/$($file.Name)";if(!(Test-Path $other)){throw "$mode missing trace $($file.Name)"}
   if((Get-FileHash $file.FullName).Hash -ne (Get-FileHash $other).Hash){throw "$mode actual plugin trace differs: $($file.Name)"}
  }
 }
 $r.status='passed';$r.sdkValidated=$true
 $r.expectedAssets=$expectedAssets;$r.sourceFiles=@(Get-ChildItem src/TextureCompiler/Managed,src/TextureCompiler/Colour,src/TextureCompiler/Dxt -File|Sort-Object Name|ForEach-Object{@{path=$_.FullName.Substring($root.Length+1).Replace('\','/');sha256=(Get-FileHash $_.FullName).Hash.ToLowerInvariant()}})
 Write-Output 'PASS Texture BMP headers SDK original/recovered/no-original: four stream files, actual Core calls, every buffer and full cdata identical'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{
 $env:RA3_TEXTURE_SDK_TRACE=$null
 $traces=@(Get-ChildItem $out -Directory -Filter '*-trace')
 try{if($traces.Count -gt 0){
  Compress-Archive -Path $traces.FullName -DestinationPath "$out/traces.zip"
  $r.traceArchiveSha256=(Get-FileHash "$out/traces.zip").Hash.ToLowerInvariant()
  Remove-Item $traces.FullName -Recurse -Force
 }}catch{$r.traceArchiveError=$_.Exception.Message}finally{$r|ConvertTo-Json -Depth 14|Set-Content "$out/validation.json"}
}




