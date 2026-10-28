param([int]$TimeoutSeconds=600)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$out="recovery/evidence/texture/sdk-atlas/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-sdk-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work | Out-Null
$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';sdkValidated=$false;scope="Dedicated PC TerrainTextureAtlas MOD through real Core: tile-count atlas shapes, normals256/128, duplicate IDs/Cell, metadata, NPOT and24-bit source; four artifacts/full buffer traces in three modes; not complete production MOD";profile="PcAtlas";targetPlatform='Win32';modes=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 60;throw "$name failed"}}
try{
 Checked recovered-build {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c Release -o "$work/build" --nologo}
 Checked colour-asm {& ml /nologo /c /coff /safeseh /Fo"$work/colour.obj" (Resolve-Path src/TextureCompiler/Colour/TextureColour.asm).Path}
 Checked colour-native {& cl /nologo /LD /MT /fp:strict /GS- /O2 /Fo"$work/bridge.obj" (Resolve-Path src/TextureCompiler/Colour/TextureColourBridge.cpp).Path "$work/colour.obj" /link /NODEFAULTLIB /NOENTRY /OUT:"$work/Recovered.TextureDxtColour.dll" /IMPLIB:"$work/colour.lib"}
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
 $baseline='recovery/evidence/texture/atlas-load/40599993769-1'
 $receipt=Get-Content "$baseline/validation.json" -Raw|ConvertFrom-Json
 if($receipt.status -ne 'probe-only' -or $receipt.sourceCommit -ne '056937b13fa39431ec9171b0d990e6a5c15a013c' -or $receipt.fixtureCount -ne 25 -or $receipt.originalCalls -ne 50 -or $receipt.originalSha256 -ne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e' -or (Get-FileHash "$baseline/samples.zip").Hash.ToLowerInvariant() -ne $receipt.samplesSha256){throw 'Wrong immutable original PC atlas SDK inputs'}
 git merge-base --is-ancestor $receipt.sourceCommit HEAD;if($LASTEXITCODE -ne 0){throw 'Atlas SDK baseline is not an ancestor'}
 Expand-Archive "$baseline/samples.zip" "$work/fixtures"
 $r.originalBaseline=@{sourceCommit=$receipt.sourceCommit;runId=$receipt.runId;samplesSha256=$receipt.samplesSha256}
 $wrapper=(Resolve-Path recovery/tests/TextureDxtSdkTracePlugin.cs).Path
 foreach($mode in @('Original','Recovered','NoOriginal')){
  $sdk=Join-Path $work $mode;Copy-Item SDK $sdk -Recurse
  Remove-Item "$sdk/builtmods" -Recurse -Force -ErrorAction SilentlyContinue
  $moddir=Join-Path $sdk 'Mods\RecoveryTexture';New-Item -ItemType Directory -Force "$moddir/Data","$moddir/Art"|Out-Null
  Get-ChildItem "$work/fixtures" -File|Where-Object {$_.Extension -in @('.tga','.png')}|Copy-Item -Destination "$moddir/Art"
  $nodes=@()
  $labels=@('count-1','count-2','count-3','count-4','count-7','count-8','count-9','count-10','count-21','count-22','count-49','metadata-zero-false','metadata-max-true','metadata-default-false','base-npot','base-24')
  foreach($label in $labels){
   [xml]$fixture=Get-Content "$work/fixtures/$label.input.xml" -Raw
   $fixture.DocumentElement.SetAttribute('id','AtlasSdk_'+$label)
   foreach($tile in $fixture.DocumentElement.ChildNodes){
    if($tile.LocalName -eq 'Tile'){
     $tile.SetAttribute('BaseTexture','..\Art\'+$tile.GetAttribute('BaseTexture'))
     $tile.SetAttribute('NormalTexture','..\Art\'+$tile.GetAttribute('NormalTexture'))
    }
   }
   $nodes+=$fixture.DocumentElement.OuterXml
  }
  $expectedAssets=$nodes.Count
  if($expectedAssets -ne 16){throw 'Expected16 complete PC atlas assets'}
  $r.fixtureLabels=$labels
  "<?xml version='1.0'?><AssetDeclaration xmlns='uri:ea.com:eala:asset'><Tags />$($nodes -join "`n")</AssetDeclaration>"|Set-Content "$moddir/Data/Mod.xml" -Encoding utf8
  $trace="$out/$mode-trace";New-Item -ItemType Directory -Force $trace|Out-Null
  $cfg="$sdk/tools/BinaryAssetBuilder.exe.config";[xml]$xml=Get-Content $cfg -Raw
  $plugin=@($xml.configuration.assetbuilder.plugins.plugin|Where-Object {$_.assetTypes.Split(',') -contains 'Texture'})
  if($plugin.Count -ne 1){throw 'Texture plugin route is ambiguous'}
  if($mode -eq 'Original'){
   Checked "$mode-wrapper-build" {& $csc /nologo /target:library /platform:x86 /define:ORIGINAL /r:"$sdk/tools/BinaryAssetBuilder.Core.dll" /r:"$sdk/tools/BinaryAssetBuilder.Utility.dll" /r:"$sdk/tools/BinaryAssetBuilder.TextureCompiler.dll" /out:"$sdk/tools/TextureSdkTrace.dll" $wrapper}
  }else{
   $wrapperDefines=@();if($mode -eq 'NoOriginal'){$wrapperDefines=@('/define:NO_ORIGINAL')}
   Copy-Item "$work/build/Recovered.BinaryAssetBuilder.TextureCompiler.dll","$work/Recovered.TextureDxtColour.dll","$work/Recovered.TextureDxtAlpha.dll" "$sdk/tools"
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
 Write-Output 'PASS Texture PC atlas SDK original/recovered/no-original: four stream files, actual Core calls, every buffer and full cdata identical'
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




