param([string]$RunId=$env:GITHUB_RUN_ID,[string]$RunAttempt=$env:GITHUB_RUN_ATTEMPT)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$source=(git rev-parse HEAD).Trim()
$out=Join-Path $root "recovery/evidence/texture/deployment/$RunId-$RunAttempt"
$sdk=Join-Path $root 'SDK';$destination=Join-Path $sdk 'RecoveryTexture'
if(Test-Path $destination){throw 'SDK already has a deployment; restore or explicitly upgrade it before replacing its rollback receipt'}
New-Item -ItemType Directory -Force $out|Out-Null
function Hash([string]$p){(Get-FileHash $p -Algorithm SHA256).Hash.ToLowerInvariant()}
$receipts=@{}
foreach($group in @('sdk-ppm','sdk-mymod')){
 $directory=Join-Path $root "recovery/evidence/texture/$group/$RunId-$RunAttempt"
 $r=Get-Content "$directory/validation.json" -Raw|ConvertFrom-Json
 if($r.status -ne 'passed' -or !$r.sdkValidated -or $r.sourceCommit -ne $source -or $r.runId -ne $RunId -or $r.runAttempt -ne $RunAttempt){throw "Incomplete or wrong-source deployment gate: $group"}
 if($r.modes.Count -ne 3 -or @(Compare-Object @('NoOriginal','Original','Recovered') @($r.modes.mode|Sort-Object)).Count -ne 0 -or @($r.modes|Where-Object {!$_.passed}).Count -ne 0){throw "Missing three-mode acceptance: $group"}
 $isolated=@($r.modes|Where-Object mode -eq 'NoOriginal')
 if($isolated.Count -ne 1 -or !$isolated[0].withoutOriginalTextureCompiler -or !$isolated[0].assertedOriginalNotLoadedDuringEveryCall){throw "Missing per-call isolation gate: $group"}
 if((Hash "$directory/pc-plugin-package.zip") -ne $r.packageArchiveSha256){throw "Tested package bytes changed: $group"}
 $receipts[$group]=$r
}
if(!$receipts['sdk-mymod'].packageRestoreValidated -or !$receipts['sdk-mymod'].portableRestoreValidated -or !$receipts['sdk-mymod'].reusedPpmPackage -or $receipts['sdk-mymod'].packageArchiveSha256 -ne $receipts['sdk-ppm'].packageArchiveSha256){throw 'Deploy only the same PPM/full-MOD tested package, including relocated rollback'}
$baseline=@($receipts['sdk-mymod'].baselineModes)
if($baseline.Count -ne 3 -or @($baseline|Where-Object {!$_.passed -or $_.actualCalls -ne 0 -or $_.uniqueAssets -ne 0}).Count -ne 0){throw 'Complete unmodified mymod baseline gate missing'}
$ready=Get-Content "$root/recovery/evidence/texture/sdk-ppm/$RunId-$RunAttempt/component-readiness.json" -Raw|ConvertFrom-Json
if($ready.sourceCommit -ne $source -or @($ready.selected).Count -ne 0 -or @($ready.reused).Count -lt 49){throw 'Current complete component readiness gate missing'}
$work=Join-Path $env:RUNNER_TEMP ('texture-deployment-'+[Guid]::NewGuid().ToString('N'))
Expand-Archive "$root/recovery/evidence/texture/sdk-ppm/$RunId-$RunAttempt/pc-plugin-package.zip" $work
$manifest=Get-Content "$work/manifest.json" -Raw|ConvertFrom-Json
if($manifest.sourceCommit -ne $source -or $manifest.dirtySource -or $manifest.status -ne 'built'){throw 'Wrong deployment source package'}
foreach($file in $manifest.sourceFiles){if((Hash "$root/$($file.file)") -ne $file.sha256){throw "Build input changed since acceptance: $($file.file)"}}
$original=Hash "$sdk/tools/BinaryAssetBuilder.TextureCompiler.dll"
$beforeConfig=Hash "$sdk/tools/BinaryAssetBuilder.exe.config"
try{
 & "$work/InstallTextureCompiler.ps1" -PackageDirectory $work -SdkDirectory $sdk -ReceiptPath "$out/installation.json"
 $installation=Get-Content "$out/installation.json" -Raw|ConvertFrom-Json
 New-Item -ItemType Directory -Force $destination|Out-Null
 # Only this new deployment directory and the installed config are byte-fixed;
 # no historical gold or unrelated SDK text is renormalized.
 '* -text'|Set-Content "$destination/.gitattributes" -Encoding ascii
 $attributes=Join-Path "$sdk/tools" '.gitattributes'
 $rule='BinaryAssetBuilder.exe.config -text'
 if(!(Test-Path $attributes) -or $rule -notin @(Get-Content $attributes)){Add-Content $attributes $rule -Encoding ascii}
 Copy-Item $installation.backupDirectory "$destination/backup" -Recurse
 $installation.sdkDirectory='..';$installation.backupDirectory='backup'
 $installation|ConvertTo-Json -Depth 8|Set-Content "$destination/installation.json"
 Remove-Item "$destination/backup/installation.json"
 Copy-Item "$work/RestoreTextureCompiler.ps1","$work/manifest.json" $destination
 '& "$PSScriptRoot/RestoreTextureCompiler.ps1" -ReceiptPath "$PSScriptRoot/installation.json"'|Set-Content "$destination/Restore.ps1"
 foreach($file in $manifest.runtimeFiles){if((Hash "$sdk/tools/$($file.file)") -ne $file.sha256){throw 'Installed bytes differ from the fully tested package'}}
 if((Hash "$sdk/tools/BinaryAssetBuilder.TextureCompiler.dll") -ne $original){throw 'Original immutable oracle DLL was changed'}
 [xml]$config=Get-Content "$sdk/tools/BinaryAssetBuilder.exe.config" -Raw
 foreach($type in @('Texture','OnDemandTexture','TerrainTextureAtlas')){
  $routes=@($config.configuration.assetbuilder.plugins.plugin|Where-Object {$_.assetTypes.Split(',') -contains $type})
  if($routes.Count -ne 1 -or $routes[0].targetType -ne $manifest.pluginType){throw "Production route was not activated: $type"}
 }
 @'
Recovered TextureCompiler is active for Win32 Texture, OnDemandTexture and TerrainTextureAtlas.
Six runtime DLLs exactly match the package tested by the PPM SDK and complete mymod.
Core/Utility and all other compiler routes retain their existing binaries.
To restore the original configuration and prior runtime file states, run in PowerShell:
  ./SDK/RecoveryTexture/Restore.ps1
The receipt is relative to this directory and works after moving the checkout.
Restore verifies every installed file and backup before changing any file.
Default DXT1, fully opaque DXT1A blocks and uncertain extended BMP bitfields remain rejected.
'@|Set-Content "$destination/README.txt"
 $deployment=@{status='installed';sourceCommit=$source;runId=$RunId;runAttempt=$RunAttempt;packageArchiveSha256=$receipts['sdk-ppm'].packageArchiveSha256;componentGroupsVerified=@($ready.reused).Count;fullMymodValidated=$true;ppmSdkValidated=$true;portableRestoreValidated=$true;originalCompilerSha256=$original;beforeConfigSha256=$beforeConfig;installedConfigSha256=(Hash "$sdk/tools/BinaryAssetBuilder.exe.config");runtimeFiles=@($manifest.runtimeFiles);targetPlatform='Win32';originalOracleRetained=$true;postDeploymentComponentValidation='required because SDK/tools inputs changed'}
 $deployment|ConvertTo-Json -Depth 12|Set-Content "$destination/deployment.json"
 $deployment|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"
 Compress-Archive -Path "$destination/*" -DestinationPath "$out/installed-metadata.zip"
 Write-Output "PASS TextureCompiler deployed: source=$source run=$RunId; exact PPM/full-MOD tested six-DLL package; portable seven-file rollback"
}catch{
 # An installation/packaging failure must leave the working SDK usable.
 if(Test-Path "$destination/installation.json"){
  & "$work/RestoreTextureCompiler.ps1" -ReceiptPath "$out/installation.json"
  Remove-Item $destination -Recurse -Force
 }elseif(Test-Path "$out/installation.json"){
  & "$work/RestoreTextureCompiler.ps1" -ReceiptPath "$out/installation.json"
 }
 throw
}finally{Remove-Item $work -Recurse -Force -ErrorAction SilentlyContinue}
