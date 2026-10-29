param([Parameter(Mandatory=$true)][string]$Destination,[ValidateSet('Debug','Release')][string]$Configuration='Release')
$ErrorActionPreference='Stop';$root=(Resolve-Path "$PSScriptRoot/../..").Path
New-Item -ItemType Directory -Force $Destination|Out-Null;$destinationPath=(Resolve-Path $Destination).Path
$opt=if($Configuration -eq 'Release'){'/O2'}else{'/Od'}
Push-Location $destinationPath
try{
 & cl /nologo /LD /MT /EHsc /std:c++17 $opt "$root/src/TextureCompiler/Native/TextureNativeDeviceApi.cpp" /link /DEF:"$root/src/TextureCompiler/Native/TextureNativeDeviceApi.def" /OUT:"$destinationPath/Recovered.TextureNativeDevice.dll" /IMPLIB:"$destinationPath/native-device.lib" /MACHINE:X86
 if($LASTEXITCODE -ne 0){throw 'Native x86 PC device build failed'}
 $data=[IO.File]::ReadAllBytes("$destinationPath/Recovered.TextureNativeDevice.dll");$pe=[BitConverter]::ToInt32($data,0x3c)
 if([BitConverter]::ToUInt16($data,$pe+4) -ne 0x14c){throw 'Device ABI requires I386'}
 $imports=@(& dumpbin /imports "$destinationPath/Recovered.TextureNativeDevice.dll");if($LASTEXITCODE -ne 0 -or ($imports -join "`n") -match 'BinaryAssetBuilder[.]TextureCompiler|d3d9[.]dll|d3dx'){throw 'Device ABI imports original compiler or system D3D'}
 & dumpbin /exports "$destinationPath/Recovered.TextureNativeDevice.dll"
 if($LASTEXITCODE -ne 0){throw 'Native device export inspection failed'}
}finally{Pop-Location}
