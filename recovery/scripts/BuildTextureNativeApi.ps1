param([Parameter(Mandatory=$true)][string]$Destination,[ValidateSet('Debug','Release')][string]$Configuration='Release')
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
New-Item -ItemType Directory -Force $Destination|Out-Null;$destinationPath=(Resolve-Path $Destination).Path
$opt=if($Configuration -eq 'Release'){'/O2'}else{'/Od'}
& cl /nologo /LD /MT /GS- $opt /Fo"$destinationPath/errors.obj" "$root/src/TextureCompiler/Native/TextureErrorStrings.cpp" /link /NOENTRY /NODEFAULTLIB /DEF:"$root/src/TextureCompiler/Native/TextureNativeApi.def" /OUT:"$destinationPath/Recovered.TextureNativeApi.dll" /IMPLIB:"$destinationPath/native-api.lib" /MACHINE:X86
if($LASTEXITCODE -ne 0){throw 'Native PC status API build failed'}
$data=[IO.File]::ReadAllBytes("$destinationPath/Recovered.TextureNativeApi.dll");$pe=[BitConverter]::ToInt32($data,0x3c)
if([BitConverter]::ToUInt16($data,$pe+4) -ne 0x14c -or [BitConverter]::ToUInt32($data,$pe+40) -ne 0 -or [BitConverter]::ToUInt32($data,$pe+128) -ne 0){throw 'Native status API requires I386, zero entry and zero imports'}
& dumpbin /exports "$destinationPath/Recovered.TextureNativeApi.dll"
if($LASTEXITCODE -ne 0){throw 'Native PC status API export inspection failed'}
