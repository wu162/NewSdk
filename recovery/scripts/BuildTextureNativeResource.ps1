param([Parameter(Mandatory=$true)][string]$Destination,[ValidateSet('Debug','Release')][string]$Configuration='Release')
$ErrorActionPreference='Stop';$root=(Resolve-Path "$PSScriptRoot/../..").Path
New-Item -ItemType Directory -Force $Destination|Out-Null;$destinationPath=(Resolve-Path $Destination).Path
$opt=if($Configuration -eq 'Release'){'/O2'}else{'/Od'}
Push-Location $destinationPath
try{
 & cl /nologo /LD /MT /EHsc /std:c++17 $opt "$root/src/TextureCompiler/Native/TextureNativeResourceApi.cpp" /link /DEF:"$root/src/TextureCompiler/Native/TextureNativeResourceApi.def" /OUT:"$destinationPath/Recovered.TextureNativeResource.dll" /IMPLIB:"$destinationPath/native-resource.lib" /MACHINE:X86
 if($LASTEXITCODE -ne 0){throw 'Native x86 PC resource build failed'}
 $data=[IO.File]::ReadAllBytes("$destinationPath/Recovered.TextureNativeResource.dll");$pe=[BitConverter]::ToInt32($data,0x3c)
 if([BitConverter]::ToUInt16($data,$pe+4) -ne 0x14c){throw 'Resource ABI requires I386'}
 $imports=@(& dumpbin /imports "$destinationPath/Recovered.TextureNativeResource.dll");if($LASTEXITCODE -ne 0 -or ($imports -join "`n") -match 'BinaryAssetBuilder[.]TextureCompiler|d3d9[.]dll|d3dx'){throw 'Resource ABI imports original compiler or system D3D'}
 & dumpbin /exports "$destinationPath/Recovered.TextureNativeResource.dll"
 if($LASTEXITCODE -ne 0){throw 'Native resource export inspection failed'}
}finally{Pop-Location}
