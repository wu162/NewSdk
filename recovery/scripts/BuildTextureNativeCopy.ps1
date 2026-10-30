param([Parameter(Mandatory=$true)][string]$Destination,[ValidateSet('Debug','Release')][string]$Configuration='Release')
$ErrorActionPreference='Stop';$root=(Resolve-Path "$PSScriptRoot/../..").Path
New-Item -ItemType Directory -Force $Destination|Out-Null;$destinationPath=(Resolve-Path $Destination).Path
$opt=if($Configuration -eq 'Release'){'/O2'}else{'/Od'}
$publicBc="$root/src/TextureCompiler/PublicD3dxBc"
$manifest=Get-Content "$publicBc/PROVENANCE.json" -Raw|ConvertFrom-Json
foreach($file in $manifest.files){$path=Join-Path $publicBc $file.path;if((Get-Item $path).Length -ne $file.bytes -or (Get-FileHash $path).Hash.ToLowerInvariant() -ne $file.sha256){throw "Public BC source changed: $($file.path)"}}
Push-Location $destinationPath
try{
 # Keep accepted raw conversion's compiler options unchanged. Only the
 # embedded public codec uses its required scalar IA32/x87 arithmetic.
 $bcObjects=@()
 foreach($name in @('BC','TexturePublicBcApi','TexturePublicBcCopy')){
  $object="$destinationPath/copy-public-$name.obj"
  & cl /nologo /c /MT /EHsc /std:c++17 $opt /arch:IA32 /fp:strict /D_XM_NO_INTRINSICS_ /DTEXTURE_PUBLIC_BC_STATIC /DTEXTURE_PUBLIC_BC_EMBEDDED_COPY "$publicBc/$name.cpp" "/Fo$object"
  if($LASTEXITCODE -ne 0){throw "Embedded copy BC build failed: $name"};$bcObjects+=$object
 }
 & cl /nologo /LD /MT /EHsc /std:c++17 $opt /fp:strict "$root/src/TextureCompiler/Native/TextureNativeCopyApi.cpp" "$root/src/TextureCompiler/Native/TextureNativeConvertApi.cpp" /link $bcObjects /DEF:"$root/src/TextureCompiler/Native/TextureNativeCopyApi.def" /OUT:"$destinationPath/Recovered.TextureNativeCopy.dll" /IMPLIB:"$destinationPath/native-copy.lib" /MACHINE:X86
 if($LASTEXITCODE -ne 0){throw 'Native x86 PC copy build failed'}
 $data=[IO.File]::ReadAllBytes("$destinationPath/Recovered.TextureNativeCopy.dll");$pe=[BitConverter]::ToInt32($data,0x3c)
 if([BitConverter]::ToUInt16($data,$pe+4) -ne 0x14c){throw 'Copy ABI requires I386'}
 $imports=@(& dumpbin /imports "$destinationPath/Recovered.TextureNativeCopy.dll");if($LASTEXITCODE -ne 0 -or ($imports -join "`n") -match 'BinaryAssetBuilder[.]TextureCompiler|d3d9[.]dll|d3dx'){throw 'Copy ABI imports original compiler or system D3D'}
 & dumpbin /exports "$destinationPath/Recovered.TextureNativeCopy.dll"
 if($LASTEXITCODE -ne 0){throw 'Native copy export inspection failed'}
}finally{Pop-Location}
