param([int]$TimeoutSeconds=300)
$Profile='PcDib'
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$evidenceFolder='sdk-dib'
$out="recovery/evidence/texture/$evidenceFolder/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-sdk-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work | Out-Null
$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';sdkValidated=$false;scope='Dedicated PC BMP/bare DIB routes and truecolour tables, core/INFO/V2/V3/OS2/V4/V5 metadata; POT/NPOT/partial raw3/DXT3/5 standard/volume complete mip, transparent DXT1A and positive atlas base/normal; three modes/four outputs/complete actual traces; not full production MOD';profile='PcDib';targetPlatform='Win32';modes=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 60;throw "$name failed"}}
try{
 # A forced SDK group still requires complete component acceptance. Re-read
 # receipts written earlier in this same batch as well as saved ancestors;
 # failed/missing groups must stop before package build or any SDK invocation.
 $componentPlan=Join-Path $work 'component-readiness.json'
 Checked component-readiness {python "$PSScriptRoot/SelectTextureGroups.py" --out $componentPlan}
 Copy-Item $componentPlan "$out/component-readiness.json"
 $readiness=Get-Content $componentPlan -Raw|ConvertFrom-Json
 if($readiness.sourceCommit -ne $r.sourceCommit -or @($readiness.selected).Count -ne 0 -or @($readiness.reused).Count -lt 46){throw "DIB SDK requires complete current component acceptance: $($readiness.selected -join ', ')"}
 $r.componentGroupsVerified=@($readiness.reused).Count
 Checked package-build {& "$PSScriptRoot/BuildTextureCompiler.ps1" -Destination "$work/package" -Configuration Release}
 $package=Get-Content "$work/package/manifest.json" -Raw|ConvertFrom-Json
 Copy-Item "$work/package/manifest.json" "$out/package-manifest.json"
 if($package.status -ne 'built' -or $package.dirtySource -or $package.runtimeFiles.Count -ne 5 -or $package.sourceCommit -ne $r.sourceCommit -or $package.originalTextureCompilerDistributed){throw "Wrong or dirty complete PC package: status=$($package.status) dirty=$($package.dirtySource) runtimeFiles=$($package.runtimeFiles.Count) uncommitted=$($package.uncommittedSource -join ', ')"}
 foreach($file in $package.runtimeFiles){if((Get-FileHash "$work/package/$($file.file)").Hash.ToLowerInvariant() -ne $file.sha256){throw 'Package runtime fingerprint mismatch'}}
 Compress-Archive -Path "$work/package/*" -DestinationPath "$out/pc-plugin-package.zip"
 $r.packageArchiveSha256=(Get-FileHash "$out/pc-plugin-package.zip").Hash.ToLowerInvariant();$r.publicS3Native=@($package.publicS3);$r.nativeBinaries=$package.nativeBinaries
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 Copy-Item SDK/tools/*.dll $work
 Copy-Item "$work/package/*.dll" $work
 Checked asset-id-build {& $csc /nologo /platform:x86 /r:"$work/BinaryAssetBuilder.Core.dll" /out:"$work/TextureSdkAssetIds.exe" (Resolve-Path recovery/tests/TextureSdkAssetIds.cs).Path}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureSdkAssetIds.exe.config"
 $fixtureSources=@('TextureDibSdkFixtures.cs','TextureDibConsumerFixtures.cs','TextureBmpOs2Fixtures.cs','TextureBmpHeaderConsumerFixtures.cs','TextureBmpHeaderFixtures.cs','TextureBmpOracle.cs')|ForEach-Object{(Resolve-Path "recovery/tests/$_").Path}
 Checked fixture-writer-build {& $csc /nologo /unsafe /platform:x86 /r:System.Drawing.dll /r:"$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /main:TextureDibSdkFixtures /out:"$work/TextureDibSdkFixtures.exe" $fixtureSources}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureDibSdkFixtures.exe.config"
 Checked fixture-write {& "$work/TextureDibSdkFixtures.exe" "$work/generated" (Resolve-Path recovery/reference/compact-icc).Path}
 New-Item -ItemType Directory -Force "$work/fixtures"|Out-Null
 Get-ChildItem "$work/generated" -File|Where-Object{$_.Name -notmatch '^(atlas|transparent)-'}|Copy-Item -Destination "$work/fixtures"
 if(@(Get-ChildItem "$work/fixtures" -File).Count -ne 498){throw 'Expected498 positive BMP/DIB raw inputs'}
 $wrapper=(Resolve-Path recovery/tests/TextureDxtSdkTracePlugin.cs).Path
 foreach($mode in @('Original','Recovered','NoOriginal')){
  $sdk=Join-Path $work $mode;Copy-Item SDK $sdk -Recurse
  Remove-Item "$sdk/builtmods" -Recurse -Force -ErrorAction SilentlyContinue
  $moddir=Join-Path $sdk 'Mods\RecoveryTexture';New-Item -ItemType Directory -Force "$moddir/Data","$moddir/Art"|Out-Null
  $nodes=@();$index=0
  foreach($fixture in Get-ChildItem "$work/fixtures" -File|Sort-Object Name){
   Copy-Item $fixture.FullName "$moddir/Art/$($fixture.Name)";$name=$fixture.Name
   foreach($kind in @('StandardTexture','VolumeTexture')){foreach($generate in @('false','true')){foreach($type in @('Texture','OnDemandTexture')){foreach($format in @('A8R8G8B8','X8R8G8B8','R5G6B5','DXT3','DXT5')){
    $nodes+="<$type id='Dib_$index' File='..\Art\$name' Type='$kind' OutputFormat='$format' GenerateMipMaps='$generate' AllowAutomaticResize='false' />";$index++
   }}}}
  }
  if($nodes.Count -ne 19920){throw 'Expected19920 DIB/BMP tables raw/DXT3/5 assets'};$r.textureAssets=19920
  foreach($fixture in Get-ChildItem "$work/generated" -File -Filter 'transparent-*'|Sort-Object Name){
   Copy-Item $fixture.FullName "$moddir/Art/$($fixture.Name)";$name=$fixture.Name
   foreach($kind in @('StandardTexture','VolumeTexture')){foreach($generate in @('false','true')){foreach($type in @('Texture','OnDemandTexture')){
    $nodes+="<$type id='Dib_$index' File='..\Art\$name' Type='$kind' OutputFormat='DXT1A' GenerateMipMaps='$generate' AllowAutomaticResize='false' />";$index++
   }}}
  }
  if($nodes.Count -ne 21248){throw 'Expected1328 DIB/BMP tables transparent DXT1A assets'};$r.transparentDxt1aAssets=1328
  foreach($fixture in Get-ChildItem "$work/generated" -File -Filter 'atlas-*'|Sort-Object Name){
   # The original packer requires "tga" in BOTH path strings, independently
   # of file magic. Keep the immutable/generated BMP payload, with a legal
   # atlas path; renaming does not convert or replace the image bytes.
   $name=[IO.Path]::ChangeExtension($fixture.Name,'.tga');Copy-Item $fixture.FullName "$moddir/Art/$name"
   foreach($normal in @('missing-normal.tga',$name)){
    $nodes+="<TerrainTextureAtlas id='DibAtlas_$index' AtlasSize='2048' AllowLossyCompression='true'><Tile TextureID='42' BaseTexture='..\Art\$name' NormalTexture='..\Art\$normal' /></TerrainTextureAtlas>";$index++
   }
  }
  $expectedAssets=$nodes.Count;if($expectedAssets -ne 21464){throw 'Expected21464 DIB/BMP tables texture/atlas assets'};$r.atlasAssets=216

  "<?xml version='1.0'?><AssetDeclaration xmlns='uri:ea.com:eala:asset'><Tags />$($nodes -join "`n")</AssetDeclaration>"|Set-Content "$moddir/Data/Mod.xml" -Encoding utf8
  Checked "$mode-asset-ids" {& "$work/TextureSdkAssetIds.exe" "$moddir/Data/Mod.xml" "$out/$mode-asset-ids.tsv" $expectedAssets}
  $idHash=(Get-FileHash "$out/$mode-asset-ids.tsv").Hash.ToLowerInvariant()
  $modHash=(Get-FileHash "$moddir/Data/Mod.xml").Hash.ToLowerInvariant()
  if($mode -eq 'Original'){$r.assetIdManifestSha256=$idHash;$r.inputModSha256=$modHash}
  elseif($idHash -ne $r.assetIdManifestSha256 -or $modHash -ne $r.inputModSha256){throw 'Three SDK modes must use identical checked IDs and input XML'}
  $trace="$out/$mode-trace";New-Item -ItemType Directory -Force $trace|Out-Null
  if($mode -ne 'Original'){
   $beforeInstallHash=(Get-FileHash "$sdk/tools/BinaryAssetBuilder.exe.config").Hash
   Checked "$mode-package-install" {& "$work/package/InstallTextureCompiler.ps1" -PackageDirectory "$work/package" -SdkDirectory $sdk -ReceiptPath "$out/$mode-installation.json"}
   if($mode -eq 'Recovered'){
    Checked package-restore {& "$work/package/RestoreTextureCompiler.ps1" -ReceiptPath "$out/$mode-installation.json"}
    if((Get-FileHash "$sdk/tools/BinaryAssetBuilder.exe.config").Hash -ne $beforeInstallHash){throw 'Package restore changed prior SDK configuration'}
    Checked package-reinstall {& "$work/package/InstallTextureCompiler.ps1" -PackageDirectory "$work/package" -SdkDirectory $sdk -ReceiptPath "$out/$mode-installation.json"}
    $r.packageRestoreValidated=$true
   }
  }
  $cfg="$sdk/tools/BinaryAssetBuilder.exe.config";[xml]$xml=Get-Content $cfg -Raw
  $plugin=@($xml.configuration.assetbuilder.plugins.plugin|Where-Object {$_.assetTypes.Split(',') -contains 'Texture'})
  if($plugin.Count -ne 1){throw 'Texture plugin route is ambiguous'}
  if($mode -eq 'Original'){
   Checked "$mode-wrapper-build" {& $csc /nologo /target:library /platform:x86 /define:ORIGINAL /r:"$sdk/tools/BinaryAssetBuilder.Core.dll" /r:"$sdk/tools/BinaryAssetBuilder.Utility.dll" /r:"$sdk/tools/BinaryAssetBuilder.TextureCompiler.dll" /out:"$sdk/tools/TextureSdkTrace.dll" $wrapper}
  }else{
   $wrapperDefines=@();if($mode -eq 'NoOriginal'){$wrapperDefines=@('/define:NO_ORIGINAL')}
   if($plugin[0].targetType -ne $package.pluginType){throw 'Package installer did not set the actual Texture plugin route'}
   Checked "$mode-wrapper-build" {& $csc /nologo /target:library /platform:x86 $wrapperDefines /r:"$sdk/tools/BinaryAssetBuilder.Core.dll" /r:"$sdk/tools/BinaryAssetBuilder.Utility.dll" /r:"$sdk/tools/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /out:"$sdk/tools/TextureSdkTrace.dll" $wrapper}
   if($mode -eq 'NoOriginal'){Remove-Item "$sdk/tools/BinaryAssetBuilder.TextureCompiler.dll" -Force}
  }
  $plugin[0].SetAttribute('targetType','TextureSdkTracePlugin, TextureSdkTrace');$plugin[0].SetAttribute('useBuildCache','false')
  $startup=$xml.configuration.startup;if($null -eq $startup){$startup=$xml.CreateElement('startup');[void]$xml.configuration.AppendChild($startup)};$startup.SetAttribute('useLegacyV2RuntimeActivationPolicy','true')
  $runtime=$startup.supportedRuntime;if($null -eq $runtime){$runtime=$xml.CreateElement('supportedRuntime');[void]$startup.AppendChild($runtime)};$runtime.SetAttribute('version','v4.0');$xml.Save($cfg)
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
  $atlasCalls=@($success|Where-Object TypeName -eq 'TerrainTextureAtlas'|ForEach-Object InstanceId|Sort-Object -Unique).Count
  if($atlasCalls -ne 216){throw "$mode expected216 successful unique atlas calls; found $atlasCalls"}
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
 Write-Output 'PASS Texture DIB/BMP tables SDK original/recovered/no-original: four stream files, actual Core calls, every buffer and full cdata identical'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{
 $env:RA3_TEXTURE_SDK_TRACE=$null
 $traces=@(Get-ChildItem $out -Directory -Filter '*-trace')
 try{if($traces.Count -gt 0){
  Compress-Archive -Path $traces.FullName -DestinationPath "$out/traces.zip"
  $r.traceArchiveSha256=(Get-FileHash "$out/traces.zip").Hash.ToLowerInvariant()
  Remove-Item $traces.FullName -Recurse -Force
 }}catch{$r.traceArchiveError=$_.Exception.Message}finally{$r|ConvertTo-Json -Depth 14|Set-Content "$out/validation.json"}
}
