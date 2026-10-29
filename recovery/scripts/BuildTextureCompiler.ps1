param(
 [Parameter(Mandatory=$true)][string]$Destination,
 [ValidateSet('Debug','Release')][string]$Configuration='Release'
)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$destinationPath=[IO.Path]::GetFullPath($Destination)
$sdkPath=[IO.Path]::GetFullPath((Join-Path $root 'SDK'))
if($destinationPath.TrimEnd('\','/') -eq $sdkPath -or $destinationPath.StartsWith($sdkPath+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)){throw 'Build into a separate package directory, outside the source SDK'}
if(Test-Path $destinationPath){if(@(Get-ChildItem $destinationPath -Force).Count -ne 0){throw 'Package destination must be empty; use a new directory to avoid stale native dependencies'}}
foreach($command in 'dotnet','cl','ml','dumpbin'){if(-not(Get-Command $command -ErrorAction SilentlyContinue)){throw "Missing $command; use a Windows x86 MSVC developer shell with .NET 8 SDK and .NET Framework 4.8 targeting support"}}
$work=Join-Path $env:TEMP ('texture-package-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $work,$destinationPath|Out-Null
$previous=Get-Location
$r=@{sourceCommit='';configuration=$Configuration;targetPlatform='Win32';status='started';originalTextureCompilerDistributed=$false;pluginType='Recovered.Compiler.Texture.Plugin, Recovered.BinaryAssetBuilder.TextureCompiler';assetTypes=@('TerrainTextureAtlas','Texture','OnDemandTexture');runtimeFiles=@();sdkDependencies=@();sourceFiles=@();nativeBinaries=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$work/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$work/$name.log" -Tail 40;throw "$name failed"}}
function FileRecord([string]$path,[string]$name){@{file=$name;bytes=(Get-Item $path).Length;sha256=(Get-FileHash $path -Algorithm SHA256).Hash.ToLowerInvariant()}}
try{
 Set-Location $root
 $r.sourceCommit=(git rev-parse HEAD).Trim();if($LASTEXITCODE -ne 0){throw 'Cannot determine package source commit'}
 $sourcePaths=@('src/TextureCompiler','recovery/reference/ijg6b','recovery/scripts/BuildTextureCompiler.ps1','recovery/scripts/BuildTexturePublicS3.ps1','recovery/scripts/BuildTextureJpeg.ps1','recovery/scripts/InstallTextureCompiler.ps1','recovery/scripts/RestoreTextureCompiler.ps1')
 # dotnet's default obj/bin directories are generated compilation products,
 # not source inputs. Keep tracked changes and all other untracked source
 # visible, including newly added .cs/native files outside those directories.
 $r.uncommittedSource=@(git status --porcelain --untracked-files=all -- $sourcePaths|Where-Object{$_.Substring(3).Trim('"') -notmatch '^src/TextureCompiler/Managed/(?:obj|bin)/'})
 if($LASTEXITCODE -ne 0){throw 'Cannot inspect package source status'}
 $r.dirtySource=$r.uncommittedSource.Count -ne 0
 Checked managed {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c $Configuration -o "$work/managed" --nologo}
 Checked public-s3 {& "$PSScriptRoot/BuildTexturePublicS3.ps1" -Destination "$work/s3" -Configuration $Configuration}
 Checked jpeg {& "$PSScriptRoot/BuildTextureJpeg.ps1" -Destination "$work/jpeg" -Configuration $Configuration}
 Checked colour-asm {& ml /nologo /c /coff /safeseh /Fo"$work/colour.obj" (Resolve-Path src/TextureCompiler/Colour/TextureColour.asm).Path}
 $opt=if($Configuration -eq 'Release'){'/O2'}else{'/Od'}
 Checked colour {& cl /nologo /LD /MT /fp:strict /GS- $opt /Fo"$work/colour-bridge.obj" (Resolve-Path src/TextureCompiler/Colour/TextureColourBridge.cpp).Path "$work/colour.obj" /link /NODEFAULTLIB /NOENTRY /OUT:"$work/Recovered.TextureDxtColour.dll" /IMPLIB:"$work/colour.lib"}
 Checked alpha {& cl /nologo /LD /MT /fp:strict /GS- $opt /Fo"$work/alpha.obj" (Resolve-Path src/TextureCompiler/Dxt/TextureAlphaQuantizer.cpp).Path /link /NODEFAULTLIB /NOENTRY /OUT:"$work/Recovered.TextureDxtAlpha.dll" /IMPLIB:"$work/alpha.lib"}
 $runtime=@{
  'Recovered.BinaryAssetBuilder.TextureCompiler.dll'="$work/managed/Recovered.BinaryAssetBuilder.TextureCompiler.dll"
  'Recovered.TexturePublicS3.dll'="$work/s3/Recovered.TexturePublicS3.dll"
  'Recovered.TextureJpeg.dll'="$work/jpeg/Recovered.TextureJpeg.dll"
  'Recovered.TextureDxtColour.dll'="$work/Recovered.TextureDxtColour.dll"
  'Recovered.TextureDxtAlpha.dll'="$work/Recovered.TextureDxtAlpha.dll"
 }
 foreach($name in @($runtime.Keys|Sort-Object)){
  $path=$runtime[$name];$bytes=[IO.File]::ReadAllBytes($path);$pe=[BitConverter]::ToInt32($bytes,0x3c)
  if([BitConverter]::ToUInt16($bytes,$pe+4) -ne 0x14c){throw "$name must be I386"}
  if($name -ne 'Recovered.BinaryAssetBuilder.TextureCompiler.dll'){
   $imports=@(& dumpbin /imports $path);if($LASTEXITCODE -ne 0){throw "Cannot inspect $name imports"}
   if(($imports -join "`n") -match 'BinaryAssetBuilder[.]TextureCompiler'){throw "$name imports the original TextureCompiler"}
   $entry=[BitConverter]::ToUInt32($bytes,$pe+40);$importDirectory=[BitConverter]::ToUInt32($bytes,$pe+128)
   if($name -in @('Recovered.TextureDxtColour.dll','Recovered.TextureDxtAlpha.dll') -and ($entry -ne 0 -or $importDirectory -ne 0)){throw "$name must have no entry point or imports"}
   $r.nativeBinaries+=@{file=$name;machine='I386';entryPoint=$entry;importDirectory=$importDirectory;imports=@($imports|Where-Object{$_ -match '^\s+\S+[.]dll\s*$'}|ForEach-Object{$_.Trim()})}
  }
  Copy-Item $path (Join-Path $destinationPath $name)
  $r.runtimeFiles+=FileRecord (Join-Path $destinationPath $name) $name
 }
 foreach($name in @('BinaryAssetBuilder.Core.dll','BinaryAssetBuilder.Utility.dll')){$r.sdkDependencies+=FileRecord (Join-Path "$root/SDK/tools" $name) $name}
 $sources=@(git ls-files -- $sourcePaths);if($LASTEXITCODE -ne 0){throw 'Cannot enumerate source fingerprints'}
 foreach($file in $sources){$r.sourceFiles+=FileRecord (Join-Path $root $file) $file}
 $r.publicS3=Get-Content "$work/s3/PublicS3-native.json" -Raw|ConvertFrom-Json
 New-Item -ItemType Directory -Force "$destinationPath/upstream/IJG6b","$destinationPath/upstream/PublicS3","$destinationPath/build-logs"|Out-Null
 Copy-Item "$root/recovery/reference/ijg6b/README","$root/recovery/reference/ijg6b/NOTICE.md","$root/recovery/reference/ijg6b/PROVENANCE.json" "$destinationPath/upstream/IJG6b"
 Copy-Item "$root/src/TextureCompiler/PublicS3/*" "$destinationPath/upstream/PublicS3"
 Copy-Item "$PSScriptRoot/InstallTextureCompiler.ps1","$PSScriptRoot/RestoreTextureCompiler.ps1" $destinationPath
 @'
PC TextureCompiler source build

Install this package into a matching SDK copy using:
./InstallTextureCompiler.ps1 -PackageDirectory . -SdkDirectory <SDK>
The installer verifies every runtime fingerprint, saves prior file states and
prints a backup directory. To restore:
./RestoreTextureCompiler.ps1 -ReceiptPath <backup>/installation.json
Restore refuses to overwrite files edited after installation.

The five Recovered.*.dll files go beside the existing SDK/tools host. The SDK's
Core/Utility and other compiler plugins remain. The three Texture roots use:
Recovered.Compiler.Texture.Plugin, Recovered.BinaryAssetBuilder.TextureCompiler

The CLR4 host startup must set useLegacyV2RuntimeActivationPolicy="true".
Use Win32 only. Default DXT1 and opaque DXT1A blocks remain explicitly rejected;
see recovery/RESUME.md at the manifest's sourceCommit for the accepted scope.
The colour/alpha DLLs serve the separately exposed low-level recovered APIs.
The complete production DXT route uses PublicS3; JPEG uses the pinned IJG6b DLL.

This bundle includes original upstream notices, provenance and adapted PublicS3
source. It does not include the original TextureCompiler, Core or Utility DLLs.
manifest.json records the exact source and runtime fingerprints; a successful
build alone is not component/SDK acceptance or production installation.
'@|Set-Content "$destinationPath/README.txt"
 Copy-Item "$work/*.log" "$destinationPath/build-logs"
 $r.status='built';$r|ConvertTo-Json -Depth 16|Set-Content "$destinationPath/manifest.json"
 Write-Output "PASS TextureCompiler PC package $Configuration files=$($r.runtimeFiles.Count) source=$($r.sourceCommit) dirty=$($r.dirtySource); managed/PublicS3/IJG6b/independent colour+alpha; build only, acceptance separate"
}catch{
 $r.status='failed';$r.error=$_.Exception.Message
 New-Item -ItemType Directory -Force "$destinationPath/build-logs"|Out-Null
 Copy-Item "$work/*.log" "$destinationPath/build-logs" -ErrorAction SilentlyContinue
 $r|ConvertTo-Json -Depth 16|Set-Content "$destinationPath/manifest.json"
 throw
}finally{Set-Location $previous;Remove-Item $work -Recurse -Force -ErrorAction SilentlyContinue}
