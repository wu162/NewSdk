param([Parameter(Mandatory=$true)][string]$Destination,[ValidateSet('Debug','Release')][string]$Configuration='Release')
$ErrorActionPreference='Stop';$root=(Resolve-Path "$PSScriptRoot/../..").Path
New-Item -ItemType Directory -Force $Destination|Out-Null;$destinationPath=(Resolve-Path $Destination).Path
$opt=if($Configuration -eq 'Release'){'/O2'}else{'/Od'}
Push-Location $destinationPath
try{
 & cl /nologo /LD /MT /EHsc /std:c++17 $opt "$root/src/TextureCompiler/Native/TextureNativeDescriptorApi.cpp" /link /DEF:"$root/src/TextureCompiler/Native/TextureNativeDescriptorApi.def" /OUT:"$destinationPath/Recovered.TextureNativeDescriptor.dll" /IMPLIB:"$destinationPath/native-descriptor.lib" /MACHINE:X86
 if($LASTEXITCODE -ne 0){throw 'Native x86 PC descriptor build failed'}
 $data=[IO.File]::ReadAllBytes("$destinationPath/Recovered.TextureNativeDescriptor.dll");$pe=[BitConverter]::ToInt32($data,0x3c)
 if([BitConverter]::ToUInt16($data,$pe+4) -ne 0x14c){throw 'Descriptor ABI requires I386'}
 $imports=@(& dumpbin /imports "$destinationPath/Recovered.TextureNativeDescriptor.dll");if($LASTEXITCODE -ne 0 -or ($imports -join "`n") -match 'BinaryAssetBuilder[.]TextureCompiler|d3d9[.]dll|d3dx'){throw 'Descriptor ABI imports original compiler or system D3D'}
 & dumpbin /exports "$destinationPath/Recovered.TextureNativeDescriptor.dll"
 if($LASTEXITCODE -ne 0){throw 'Native descriptor export inspection failed'}
}finally{Pop-Location}
