param([Parameter(Mandatory=$true)][string]$Destination,[ValidateSet('Debug','Release')][string]$Configuration='Release')
$ErrorActionPreference='Stop';$root=(Resolve-Path "$PSScriptRoot/../..").Path
New-Item -ItemType Directory -Force $Destination|Out-Null;$destinationPath=(Resolve-Path $Destination).Path
$opt=if($Configuration -eq 'Release'){'/O2'}else{'/Od'}
Push-Location $destinationPath
try{
 & cl /nologo /LD /MT /EHsc /std:c++17 $opt "$root/src/TextureCompiler/Native/TextureNativeCopyApi.cpp" /link /DEF:"$root/src/TextureCompiler/Native/TextureNativeCopyApi.def" /OUT:"$destinationPath/Recovered.TextureNativeCopy.dll" /IMPLIB:"$destinationPath/native-copy.lib" /MACHINE:X86
 if($LASTEXITCODE -ne 0){throw 'Native x86 PC copy build failed'}
 $data=[IO.File]::ReadAllBytes("$destinationPath/Recovered.TextureNativeCopy.dll");$pe=[BitConverter]::ToInt32($data,0x3c)
 if([BitConverter]::ToUInt16($data,$pe+4) -ne 0x14c){throw 'Copy ABI requires I386'}
 $imports=@(& dumpbin /imports "$destinationPath/Recovered.TextureNativeCopy.dll");if($LASTEXITCODE -ne 0 -or ($imports -join "`n") -match 'BinaryAssetBuilder[.]TextureCompiler|d3d9[.]dll|d3dx'){throw 'Copy ABI imports original compiler or system D3D'}
 & dumpbin /exports "$destinationPath/Recovered.TextureNativeCopy.dll"
 if($LASTEXITCODE -ne 0){throw 'Native copy export inspection failed'}
}finally{Pop-Location}
