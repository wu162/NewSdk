param([Parameter(Mandatory=$true)][string]$Destination,[ValidateSet('Debug','Release')][string]$Configuration='Release')
$ErrorActionPreference='Stop';$root=(Resolve-Path "$PSScriptRoot/../..").Path
New-Item -ItemType Directory -Force $Destination|Out-Null;$destinationPath=(Resolve-Path $Destination).Path
$opt=if($Configuration -eq 'Release'){'/O2'}else{'/Od'}
Push-Location $destinationPath
try{
 & cl /nologo /LD /MT /EHsc /std:c++17 $opt "$root/src/TextureCompiler/Native/TextureNativeSurfaceApi.cpp" /link /DEF:"$root/src/TextureCompiler/Native/TextureNativeSurfaceApi.def" /OUT:"$destinationPath/Recovered.TextureNativeSurface.dll" /IMPLIB:"$destinationPath/native-surface.lib" /MACHINE:X86
 if($LASTEXITCODE -ne 0){throw 'Native x86 PC surface build failed'}
 $data=[IO.File]::ReadAllBytes("$destinationPath/Recovered.TextureNativeSurface.dll");$pe=[BitConverter]::ToInt32($data,0x3c)
 if([BitConverter]::ToUInt16($data,$pe+4) -ne 0x14c){throw 'Surface ABI requires I386'}
 $imports=@(& dumpbin /imports "$destinationPath/Recovered.TextureNativeSurface.dll");if($LASTEXITCODE -ne 0 -or ($imports -join "`n") -match 'BinaryAssetBuilder[.]TextureCompiler|d3d9[.]dll|d3dx'){throw 'Surface ABI imports original compiler or system D3D'}
 & dumpbin /exports "$destinationPath/Recovered.TextureNativeSurface.dll"
 if($LASTEXITCODE -ne 0){throw 'Native surface export inspection failed'}
}finally{Pop-Location}
