param([int]$TimeoutSeconds=300,[ValidateSet('PcDxtNoMip','PcDxtMips','PcDxtVolumes','PcNpotRgba8','PcNpotStorage')][string]$Profile='PcDxtNoMip')
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$evidenceFolder=if($Profile -eq 'PcNpotStorage'){'sdk-npot-storage'}elseif($Profile -eq 'PcNpotRgba8'){'sdk-npot-rgba8'}elseif($Profile -eq 'PcDxtVolumes'){'sdk-dxt-volumes'}elseif($Profile -eq 'PcDxtMips'){'sdk-dxt-mips'}else{'sdk-dxt'}
$out="recovery/evidence/texture/$evidenceFolder/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-sdk-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work | Out-Null
$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';sdkValidated=$false;scope="EALAModStudio case5 BAB build of dedicated PC standard/volume raw+DXT3/5 POT/NPOT TGA/PNG/BMP texture MOD: $Profile";profile=$Profile;targetPlatform='Win32';modes=@()}
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
 $generatorSources=@('recovery/tests/TexturePngOracle.cs','recovery/tests/TextureBmpOracle.cs','recovery/tests/TextureDxtSdkFixtures.cs')|ForEach-Object{(Resolve-Path $_).Path}
 Checked fixture-writer-build {& $csc /nologo /unsafe /platform:x86 /main:TextureDxtSdkFixtures /r:System.Drawing.dll /r:"$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /out:"$work/TextureDxtSdkFixtures.exe" $generatorSources}
 $fixtureArgs=@();if($Profile -eq 'PcDxtVolumes'){$fixtureArgs=@('volume')}
 if($Profile -eq 'PcNpotStorage'){
  & "$PSScriptRoot/SelectTextureNpotStorageFixtures.ps1" -Destination "$work/fixtures" -Extraction "$work/npot-storage-source"
  $r.originalNpotStorageBaseline=@{sourceCommit='c6459c7f0d2a88d3680e882bb0713bc95cb985a5';runId='40592911746';fixtures=64}
 }elseif($Profile -eq 'PcNpotRgba8'){
 $npotEvidence='recovery/evidence/texture/npot-load/40528237009-1'
 $npotReceipt=Get-Content "$npotEvidence/validation.json" -Raw|ConvertFrom-Json
 if($npotReceipt.status -ne 'probe-only' -or $npotReceipt.sourceCommit -ne 'a19bb879704cd80ad55202760a160c8af57bbe2d' -or (Get-FileHash "$npotEvidence/samples.zip").Hash.ToLowerInvariant() -ne $npotReceipt.samplesSha256){throw 'NPOT source corpus mismatch'}
 Expand-Archive "$npotEvidence/samples.zip" "$work/npot-source"
 New-Item -ItemType Directory -Force "$work/npot-fixtures"|Out-Null
 foreach($row in Import-Csv "$work/npot-source/transcript.tsv" -Delimiter "`t"){
  if(("$($row.sourceWidth)x$($row.sourceHeight)" -in @('3x1','3x2','5x3','7x5')) -and ($row.pattern -in @('0','2'))){
   $ext=if($row.profile.StartsWith('tga-')){'tga'}elseif($row.profile.StartsWith('png-')){'png'}else{'bmp'}
   Copy-Item "$work/npot-source/$($row.label).$ext" "$work/npot-fixtures"
  }
 }
 if(@(Get-ChildItem "$work/npot-fixtures" -File).Count -ne 80){throw 'Expected 80 focused NPOT inputs'}
  Copy-Item "$work/npot-fixtures" "$work/fixtures" -Recurse
  $r.originalNpotBaseline=@{sourceCommit=$npotReceipt.sourceCommit;runId=$npotReceipt.runId;samplesSha256=$npotReceipt.samplesSha256}
 }else{Checked fixture-write {& "$work/TextureDxtSdkFixtures.exe" "$work/fixtures" @fixtureArgs}}
 $wrapper=(Resolve-Path recovery/tests/TextureDxtSdkTracePlugin.cs).Path
 foreach($mode in @('Original','Recovered','NoOriginal')){
  $sdk=Join-Path $work $mode;Copy-Item SDK $sdk -Recurse
  Remove-Item "$sdk/builtmods" -Recurse -Force -ErrorAction SilentlyContinue
  $moddir=Join-Path $sdk 'Mods\RecoveryTexture';New-Item -ItemType Directory -Force "$moddir/Data","$moddir/Art"|Out-Null
  $nodes=@();$index=0
  foreach($fixture in Get-ChildItem "$work/fixtures" -File|Sort-Object Name){
    Copy-Item $fixture.FullName "$moddir/Art/$($fixture.Name)"
    $name=$fixture.Name;$generate=if($Profile -eq 'PcDxtMips'){'true'}else{'false'}
    $kinds=if($Profile -in @('PcNpotRgba8','PcNpotStorage')){@('StandardTexture','VolumeTexture')}elseif($Profile -eq 'PcDxtVolumes'){@('VolumeTexture')}else{@('StandardTexture')}
    $mipmapOptions=if($Profile -in @('PcDxtVolumes','PcNpotRgba8','PcNpotStorage')){@('false','true')}else{@($generate)}
    foreach($kind in $kinds){foreach($generate in $mipmapOptions){
     foreach($type in @('Texture','OnDemandTexture')){foreach($format in $(if($Profile -in @('PcNpotRgba8','PcNpotStorage')){@('A8R8G8B8','X8R8G8B8','R5G6B5','DXT3','DXT5')}else{@('DXT3','DXT5')})){
      $nodes+="<$type id='RecoveredRaster_$index' File='..\Art\$name' Type='$kind' OutputFormat='$format' GenerateMipMaps='$generate' AllowAutomaticResize='false' />";$index++
     }}
    }}
  }
  $expectedAssets=$nodes.Count
  $requiredAssets=if($Profile -eq 'PcNpotStorage'){2560}elseif($Profile -eq 'PcNpotRgba8'){3200}elseif($Profile -eq 'PcDxtVolumes'){1488}else{712}
  if($expectedAssets -ne $requiredAssets){throw "Expected $requiredAssets compressed texture assets, got $expectedAssets"}

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
  foreach($ext in @('bin','manifest','imp','relo')){if((Get-FileHash "$out/Original-mod.$ext").Hash -ne (Get-FileHash "$out/$mode-mod.$ext").Hash){throw "$mode SDK mod.$ext differs"}}
  foreach($file in Get-ChildItem "$out/Original-trace" -File){
   $other="$out/$mode-trace/$($file.Name)";if(!(Test-Path $other)){throw "$mode missing trace $($file.Name)"}
   if((Get-FileHash $file.FullName).Hash -ne (Get-FileHash $other).Hash){throw "$mode actual plugin trace differs: $($file.Name)"}
  }
 }
 $r.status='passed';$r.sdkValidated=$true
 $r.expectedAssets=$expectedAssets;$r.sourceFiles=@(Get-ChildItem src/TextureCompiler/Managed,src/TextureCompiler/Colour,src/TextureCompiler/Dxt -File|Sort-Object Name|ForEach-Object{@{path=$_.FullName.Substring($root.Length+1).Replace('\','/');sha256=(Get-FileHash $_.FullName).Hash.ToLowerInvariant()}})
 Write-Output 'PASS Texture DXT SDK original/recovered/no-original: four stream files, actual Core calls, every buffer and full cdata identical'
}finally{
 $env:RA3_TEXTURE_SDK_TRACE=$null
 $traces=@(Get-ChildItem $out -Directory -Filter '*-trace')
 if($traces.Count -gt 0){
  Compress-Archive -Path $traces.FullName -DestinationPath "$out/traces.zip"
  $r.traceArchiveSha256=(Get-FileHash "$out/traces.zip").Hash.ToLowerInvariant()
  Remove-Item $traces.FullName -Recurse -Force
 }
 $r|ConvertTo-Json -Depth 14|Set-Content "$out/validation.json"
}




