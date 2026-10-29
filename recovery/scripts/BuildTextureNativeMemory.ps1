param([Parameter(Mandatory=$true)][string]$Destination,[ValidateSet('Debug','Release')][string]$Configuration='Release')
$ErrorActionPreference='Stop';$root=(Resolve-Path "$PSScriptRoot/../..").Path
New-Item -ItemType Directory -Force $Destination|Out-Null;$destinationPath=(Resolve-Path $Destination).Path
$opt=if($Configuration -eq 'Release'){'/O2'}else{'/Od'}
Push-Location $destinationPath
try{
 & cl /nologo /LD /MT /EHsc /std:c++17 $opt "$root/src/TextureCompiler/Native/TextureSupport.cpp" "$root/src/TextureCompiler/Native/TextureNativeMemoryApi.cpp" /link /DEF:"$root/src/TextureCompiler/Native/TextureNativeMemoryApi.def" /OUT:"$destinationPath/Recovered.TextureNativeMemory.dll" /IMPLIB:"$destinationPath/native-memory.lib" /MACHINE:X86
 if($LASTEXITCODE -ne 0){& dumpbin /symbols "$destinationPath/TextureNativeMemoryApi.obj"|Select-String 'TextureBuffer|TextureAllocate|TextureFree|TextureReverse|TextureDirect3D';throw 'Native x86 memory ABI build failed'}
 $data=[IO.File]::ReadAllBytes("$destinationPath/Recovered.TextureNativeMemory.dll");$pe=[BitConverter]::ToInt32($data,0x3c)
 if([BitConverter]::ToUInt16($data,$pe+4) -ne 0x14c){throw 'Memory ABI requires I386'}
 $imports=@(& dumpbin /imports "$destinationPath/Recovered.TextureNativeMemory.dll");if($LASTEXITCODE -ne 0 -or ($imports -join "`n") -match 'BinaryAssetBuilder[.]TextureCompiler'){throw 'Memory ABI imports the original compiler'}
 & dumpbin /exports "$destinationPath/Recovered.TextureNativeMemory.dll"
 if($LASTEXITCODE -ne 0){throw 'Native memory export inspection failed'}
}finally{Pop-Location}
