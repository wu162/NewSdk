param(
 [Parameter(Mandatory=$true)][string]$PackageDirectory,
 [Parameter(Mandatory=$true)][string]$SdkDirectory,
 [string]$ReceiptPath
)
$ErrorActionPreference='Stop'
$package=(Resolve-Path $PackageDirectory).Path;$sdk=(Resolve-Path $SdkDirectory).Path
$tools=Join-Path $sdk 'tools';$config=Join-Path $tools 'BinaryAssetBuilder.exe.config'
$manifest=Get-Content "$package/manifest.json" -Raw|ConvertFrom-Json
$expected=@('Recovered.BinaryAssetBuilder.TextureCompiler.dll','Recovered.TextureDxtAlpha.dll','Recovered.TextureDxtColour.dll','Recovered.TextureJpeg.dll','Recovered.TexturePpm.dll','Recovered.TexturePublicS3.dll')
if($manifest.status -ne 'built' -or $manifest.targetPlatform -ne 'Win32' -or $manifest.originalTextureCompilerDistributed -or $manifest.runtimeFiles.Count -ne 6 -or @((Compare-Object $expected @($manifest.runtimeFiles.file|Sort-Object))).Count -ne 0){throw 'Not a complete PC TextureCompiler package'}
if($manifest.sdkDependencies.Count -ne 2 -or @(Compare-Object @('BinaryAssetBuilder.Core.dll','BinaryAssetBuilder.Utility.dll') @($manifest.sdkDependencies.file|Sort-Object)).Count -ne 0){throw 'Package must identify both SDK Core/Utility dependencies'}
foreach($file in @($manifest.runtimeFiles)){
 if((Get-FileHash "$package/$($file.file)").Hash.ToLowerInvariant() -ne $file.sha256 -or (Get-Item "$package/$($file.file)").Length -ne $file.bytes){throw "Package fingerprint mismatch: $($file.file)"}
}
foreach($file in @($manifest.sdkDependencies)){
 if($file.file -notin @('BinaryAssetBuilder.Core.dll','BinaryAssetBuilder.Utility.dll') -or (Get-FileHash "$tools/$($file.file)").Hash.ToLowerInvariant() -ne $file.sha256){throw 'SDK Core/Utility do not match this source build'}
}
if($manifest.pluginType -ne 'Recovered.Compiler.Texture.Plugin, Recovered.BinaryAssetBuilder.TextureCompiler'){throw 'Unexpected plugin type'}
$xml=New-Object Xml.XmlDocument;$xml.PreserveWhitespace=$true;$xml.Load($config)
$roots=@('TerrainTextureAtlas','Texture','OnDemandTexture');$routes=@()
foreach($root in $roots){
 $nodes=@($xml.configuration.assetbuilder.plugins.plugin|Where-Object{$_.assetTypes.Split(',') -contains $root})
 if($nodes.Count -ne 1){throw "SDK $root route is missing or ambiguous"}
 if($routes -notcontains $nodes[0]){$routes+=$nodes[0]}
}
foreach($route in $routes){if(@($route.assetTypes.Split(',')|Where-Object{$_ -notin $roots}).Count -ne 0){throw 'A Texture route also owns another compiler; split its assetTypes before installing'}}
$backup=Join-Path $sdk ('RecoveryTextureBackups/'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $backup|Out-Null
$r=@{status='started';sdkDirectory=$sdk;backupDirectory=$backup;packageSourceCommit=$manifest.sourceCommit;packageManifestSha256=(Get-FileHash "$package/manifest.json").Hash.ToLowerInvariant();files=@()}
foreach($name in @('BinaryAssetBuilder.exe.config')+$expected){
 $path=Join-Path $tools $name;$existed=Test-Path $path
 $row=@{file=$name;existed=$existed;beforeSha256=$null;installedSha256=$null}
 if($existed){Copy-Item $path (Join-Path $backup $name);$row.beforeSha256=(Get-FileHash $path).Hash.ToLowerInvariant()}
 $r.files+=$row
}
$temporaryConfig=Join-Path $tools ('texture-install-'+[Guid]::NewGuid().ToString('N')+'.tmp')
try{
 foreach($route in $routes){$route.SetAttribute('targetType',$manifest.pluginType);$route.SetAttribute('useBuildCache','false')}
 $startup=$xml.configuration.startup
 if($null -eq $startup){$startup=$xml.CreateElement('startup');[void]$xml.configuration.AppendChild($startup)}
 $startup.SetAttribute('useLegacyV2RuntimeActivationPolicy','true')
 $runtime=$startup.supportedRuntime
 if($null -eq $runtime){$runtime=$xml.CreateElement('supportedRuntime');[void]$startup.AppendChild($runtime)}
 $runtime.SetAttribute('version','v4.0')
 foreach($name in $expected){Copy-Item "$package/$name" "$tools/$name" -Force}
 $xml.Save($temporaryConfig);Move-Item $temporaryConfig $config -Force
 foreach($file in $r.files){$file.installedSha256=(Get-FileHash "$tools/$($file.file)").Hash.ToLowerInvariant()}
 $r.status='installed';$r|ConvertTo-Json -Depth 8|Set-Content "$backup/installation.json"
 if($ReceiptPath){$r|ConvertTo-Json -Depth 8|Set-Content $ReceiptPath}
 Write-Output "PASS TextureCompiler PC package installed: source=$($manifest.sourceCommit) backup=$backup; three Texture roots, six runtime DLLs; other compiler routes retained"
}catch{
 foreach($file in $r.files){$path=Join-Path $tools $file.file;if($file.existed){Copy-Item "$backup/$($file.file)" $path -Force}else{Remove-Item $path -Force -ErrorAction SilentlyContinue}}
 $r.status='rolled-back';$r.error=$_.Exception.Message;$r|ConvertTo-Json -Depth 8|Set-Content "$backup/installation.json"
 throw
}finally{Remove-Item $temporaryConfig -Force -ErrorAction SilentlyContinue}
