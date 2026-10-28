param([int]$TimeoutSeconds=300)
$Profile='PcPngMetadata'
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$evidenceFolder='sdk-png-metadata'
$out="recovery/evidence/texture/$evidenceFolder/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-sdk-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work | Out-Null
$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';sdkValidated=$false;scope='Dedicated PC PNG ICC/APNG/sRGB MOD: immutable RGBA8 POT/NPOT raw3/DXT3/5 standard/volume complete mip, transparent DXT1A, and PNG-coded atlas base/normal; three modes/four outputs/complete actual traces; not full production MOD';profile='PcPngMetadata';targetPlatform='Win32';modes=@()}
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
 $baselines=@(
  @{name='metadata';path='recovery/evidence/texture/png-metadata-load/40779895712-1';source='c812b8094a032dba0a29475d90e563cef211116e';count=4680;hash='5e698ae274086468e3efa5520847fa9127acc4de62c1b96060a5ac0cbab0cac6';selected=52},
  @{name='order';path='recovery/evidence/texture/png-metadata-order/40780410178-1';source='2cab493118bb33992a762ee191af392d10fe30eb';count=3240;hash='e804e2d5099f73c6be676a87153b02806fc0237827dc0da657c48503cdbd964f';selected=36}
 )
 New-Item -ItemType Directory -Force "$work/fixtures"|Out-Null;$r.originalBaselines=@()
 foreach($b in $baselines){
  $receipt=Get-Content "$($b.path)/validation.json" -Raw|ConvertFrom-Json
  if($receipt.status -ne 'probe-only' -or $receipt.sourceCommit -ne $b.source -or $receipt.fixtureCount -ne $b.count -or $receipt.originalSha256 -ne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e' -or $receipt.samplesSha256 -ne $b.hash -or (Get-FileHash "$($b.path)/samples.zip").Hash.ToLowerInvariant() -ne $b.hash){throw 'Wrong immutable PNG metadata SDK source corpus'}
  git merge-base --is-ancestor $receipt.sourceCommit HEAD;if($LASTEXITCODE -ne 0){throw 'PNG SDK inputs not ancestor'}
  Expand-Archive "$($b.path)/samples.zip" "$work/$($b.name)";$selected=0
  foreach($row in Import-Csv "$work/$($b.name)/transcript.tsv" -Delimiter "`t"){
   if($row.label -match '^c6d8-(16x8|7x5)-i0-p0-'){
    if($row.error -ne ''){throw 'PNG metadata SDK focused input is an original rejection'}
    $file="$work/$($b.name)/$($row.label).png";if((Get-FileHash $file).Hash.ToLowerInvariant() -ne $row.inputSha256){throw 'PNG input hash mismatch'}
    Copy-Item $file "$work/fixtures";$selected++
   }
  }
  if($selected -ne $b.selected){throw 'Incomplete focused PNG SDK source family'}
  $r.originalBaselines+=@{sourceCommit=$receipt.sourceCommit;runId=$receipt.runId;samplesSha256=$receipt.samplesSha256;selected=$selected}
 }
 if(@(Get-ChildItem "$work/fixtures" -File).Count -ne 88){throw 'Expected88 immutable metadata input files'}
 Checked fixture-writer-build {& $csc /nologo /unsafe /platform:x86 /main:TexturePngMetadataSdkFixtures /r:System.Drawing.dll /r:"$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /out:"$work/TexturePngMetadataSdkFixtures.exe" (Resolve-Path recovery/tests/TexturePngMetadataSdkFixtures.cs).Path (Resolve-Path recovery/tests/TexturePngMetadataFixtures.cs).Path (Resolve-Path recovery/tests/TexturePngOracle.cs).Path}
 Checked fixture-write {& "$work/TexturePngMetadataSdkFixtures.exe" "$work/generated" (Resolve-Path recovery/reference/compact-icc).Path}
 $wrapper=(Resolve-Path recovery/tests/TextureDxtSdkTracePlugin.cs).Path
 foreach($mode in @('Original','Recovered','NoOriginal')){
  $sdk=Join-Path $work $mode;Copy-Item SDK $sdk -Recurse
  Remove-Item "$sdk/builtmods" -Recurse -Force -ErrorAction SilentlyContinue
  $moddir=Join-Path $sdk 'Mods\RecoveryTexture';New-Item -ItemType Directory -Force "$moddir/Data","$moddir/Art"|Out-Null
  $nodes=@();$index=0
  foreach($fixture in Get-ChildItem "$work/fixtures" -File|Sort-Object Name){
   Copy-Item $fixture.FullName "$moddir/Art/$($fixture.Name)";$name=$fixture.Name
   foreach($kind in @('StandardTexture','VolumeTexture')){foreach($generate in @('false','true')){foreach($type in @('Texture','OnDemandTexture')){foreach($format in @('A8R8G8B8','X8R8G8B8','R5G6B5','DXT3','DXT5')){
    $nodes+="<$type id='PngMetadata_$index' File='..\Art\$name' Type='$kind' OutputFormat='$format' GenerateMipMaps='$generate' AllowAutomaticResize='false' />";$index++
   }}}}
  }
  if($nodes.Count -ne 3520){throw 'Expected3520 PNG metadata raw/DXT3/5 assets'};$r.textureAssets=3520
  foreach($fixture in Get-ChildItem "$work/generated" -File -Filter 'transparent-*'|Sort-Object Name){
   Copy-Item $fixture.FullName "$moddir/Art/$($fixture.Name)";$name=$fixture.Name
   foreach($kind in @('StandardTexture','VolumeTexture')){foreach($generate in @('false','true')){foreach($type in @('Texture','OnDemandTexture')){
    $nodes+="<$type id='PngMetadata_$index' File='..\Art\$name' Type='$kind' OutputFormat='DXT1A' GenerateMipMaps='$generate' AllowAutomaticResize='false' />";$index++
   }}}
  }
  if($nodes.Count -ne 4224){throw 'Expected704 PNG metadata transparent DXT1A assets'};$r.transparentDxt1aAssets=704
  foreach($fixture in Get-ChildItem "$work/generated" -File -Filter 'atlas-*'|Sort-Object Name){
   Copy-Item $fixture.FullName "$moddir/Art/$($fixture.Name)";$name=$fixture.Name
   foreach($normal in @('missing-normal.tga',$name)){
    $nodes+="<TerrainTextureAtlas id='PngMetadataAtlas_$index' AtlasSize='2048' AllowLossyCompression='true'><Tile TextureID='42' BaseTexture='..\Art\$name' NormalTexture='..\Art\$normal' /></TerrainTextureAtlas>";$index++
   }
  }
  $expectedAssets=$nodes.Count;if($expectedAssets -ne 4240){throw 'Expected4240 PNG metadata texture/atlas assets'};$r.atlasAssets=16

  "<?xml version='1.0'?><AssetDeclaration xmlns='uri:ea.com:eala:asset'><Tags />$($nodes -join "`n")</AssetDeclaration>"|Set-Content "$moddir/Data/Mod.xml" -Encoding utf8
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
 Write-Output 'PASS Texture PNG metadata SDK original/recovered/no-original: four stream files, actual Core calls, every buffer and full cdata identical'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{
 $env:RA3_TEXTURE_SDK_TRACE=$null
 $traces=@(Get-ChildItem $out -Directory -Filter '*-trace')
 if($traces.Count -gt 0){
  Compress-Archive -Path $traces.FullName -DestinationPath "$out/traces.zip"
  $r.traceArchiveSha256=(Get-FileHash "$out/traces.zip").Hash.ToLowerInvariant()
  Remove-Item $traces.FullName -Recurse -Force
 }
 $r|ConvertTo-Json -Depth 14|Set-Content "$out/validation.json"
}




