param([Parameter(Mandatory=$true)][string]$Destination,[ValidateSet('Debug','Release')][string]$Configuration='Release')
$ErrorActionPreference='Stop';$root=(Resolve-Path "$PSScriptRoot/../..").Path
New-Item -ItemType Directory -Force $Destination|Out-Null;$destinationPath=(Resolve-Path $Destination).Path
foreach($kind in @('Resource','Surface','Copy')){& "$PSScriptRoot/BuildTextureNative$kind.ps1" -Destination $destinationPath -Configuration $Configuration;if($LASTEXITCODE -ne 0){throw "Loader dependency $kind build failed"}}
$opt=if($Configuration -eq 'Release'){'/O2'}else{'/Od'}
# Reuse the accepted public JPEG/PPM sources and their fixed IJG6b library.
# All codecs are linked into Loader; generated helper DLLs in this temporary
# build directory are not runtime dependencies of the native file ABI.
$ppmManifest=Get-Content "$root/src/TextureCompiler/Ppm/PROVENANCE.json" -Raw|ConvertFrom-Json
foreach($pair in @(@($ppmManifest.upstreamFile,$ppmManifest.upstreamSha256),@($ppmManifest.adaptedFile,$ppmManifest.adaptedSha256))){
 if((Get-FileHash "$root/$($pair[0])").Hash.ToLowerInvariant() -ne $pair[1]){throw "PPM source fingerprint changed: $($pair[0])"}
}
$rasterIjg="$destinationPath/loader-raster-ijg"
# The PPM reader uses IJG's compressor allocation/error API as well as the
# decoder used by JPEG, so retain the pinned library's encoder objects too.
& "$PSScriptRoot/BuildTextureJpeg.ps1" -Destination $rasterIjg -Configuration $Configuration -BuildEncoder
if($LASTEXITCODE -ne 0){throw 'Loader pinned IJG build failed'}
Push-Location $destinationPath
try{
 $rasterObjects=@()
 foreach($codec in @(@('Jpeg','TextureJpeg'),@('Ppm','TexturePpm'))){
  $object="$destinationPath/loader-raster-$($codec[1]).obj"
  & cl /nologo /TC /c /MT $opt /fp:precise /D_CRT_SECURE_NO_WARNINGS "/I$rasterIjg" "/I$root/src/TextureCompiler/$($codec[0])" "$root/src/TextureCompiler/$($codec[0])/$($codec[1]).c" "/Fo$object"
  if($LASTEXITCODE -ne 0){throw "Embedded raster wrapper build failed: $($codec[1])"};$rasterObjects+=$object
 }
 $publicBc="$root/src/TextureCompiler/PublicD3dxBc";$bcObjects=@()
 foreach($name in @('BC','TexturePublicBcApi','TexturePublicBcDds','TexturePublicBcResize')){
  $object="$destinationPath/loader-public-$name.obj"
  & cl /nologo /c /MT /EHsc /std:c++17 $opt /arch:IA32 /fp:strict /D_XM_NO_INTRINSICS_ /DTEXTURE_PUBLIC_BC_STATIC /DRECOVERED_TEXTURE_NATIVE_LOADER_API_BUILD "$publicBc/$name.cpp" "/Fo$object"
  if($LASTEXITCODE -ne 0){throw "Embedded loader BC build failed: $name"};$bcObjects+=$object
 }
 & cl /nologo /LD /MT /EHsc /std:c++17 $opt /fp:strict /DTEXTURE_PUBLIC_BC_STATIC "$root/src/TextureCompiler/Native/TextureNativeLoaderApi.cpp" "$root/src/TextureCompiler/Native/TextureNativeRaster.cpp" "$root/src/TextureCompiler/PublicDirectXTex/TexturePublicTriangle.cpp" "$root/src/TextureCompiler/PublicStbBmp/TexturePublicBmp.cpp" /link $bcObjects $rasterObjects "$rasterIjg/jpeg.lib" "$destinationPath/native-resource.lib" "$destinationPath/native-surface.lib" "$destinationPath/native-copy.lib" /DEF:"$root/src/TextureCompiler/Native/TextureNativeLoaderApi.def" /OUT:"$destinationPath/Recovered.TextureNativeLoader.dll" /IMPLIB:"$destinationPath/native-loader.lib" /MACHINE:X86
 if($LASTEXITCODE -ne 0){throw 'Native x86 PC loader build failed'}
 $data=[IO.File]::ReadAllBytes("$destinationPath/Recovered.TextureNativeLoader.dll");$pe=[BitConverter]::ToInt32($data,0x3c)
 if([BitConverter]::ToUInt16($data,$pe+4) -ne 0x14c){throw 'Loader ABI requires I386'}
 $imports=@(& dumpbin /imports "$destinationPath/Recovered.TextureNativeLoader.dll");if($LASTEXITCODE -ne 0 -or ($imports -join "`n") -match 'BinaryAssetBuilder[.]TextureCompiler|d3d9[.]dll|d3dx'){throw 'Loader ABI imports original compiler or system D3D'}
 & dumpbin /exports "$destinationPath/Recovered.TextureNativeLoader.dll"
 if($LASTEXITCODE -ne 0){throw 'Native loader export inspection failed'}
}finally{Pop-Location}
