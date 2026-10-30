param([Parameter(Mandatory=$true)][string]$Destination,[ValidateSet('Debug','Release')][string]$Configuration='Release')
$ErrorActionPreference='Stop';$root=(Resolve-Path "$PSScriptRoot/../..").Path
New-Item -ItemType Directory -Force $Destination|Out-Null;$destinationPath=(Resolve-Path $Destination).Path
foreach($kind in @('Resource','Surface','Copy')){& "$PSScriptRoot/BuildTextureNative$kind.ps1" -Destination $destinationPath -Configuration $Configuration;if($LASTEXITCODE -ne 0){throw "Loader dependency $kind build failed"}}
$opt=if($Configuration -eq 'Release'){'/O2'}else{'/Od'}
Push-Location $destinationPath
try{
 & cl /nologo /LD /MT /EHsc /std:c++17 $opt /fp:strict "$root/src/TextureCompiler/Native/TextureNativeLoaderApi.cpp" /link "$destinationPath/native-resource.lib" "$destinationPath/native-surface.lib" "$destinationPath/native-copy.lib" /DEF:"$root/src/TextureCompiler/Native/TextureNativeLoaderApi.def" /OUT:"$destinationPath/Recovered.TextureNativeLoader.dll" /IMPLIB:"$destinationPath/native-loader.lib" /MACHINE:X86
 if($LASTEXITCODE -ne 0){throw 'Native x86 PC loader build failed'}
 $data=[IO.File]::ReadAllBytes("$destinationPath/Recovered.TextureNativeLoader.dll");$pe=[BitConverter]::ToInt32($data,0x3c)
 if([BitConverter]::ToUInt16($data,$pe+4) -ne 0x14c){throw 'Loader ABI requires I386'}
 $imports=@(& dumpbin /imports "$destinationPath/Recovered.TextureNativeLoader.dll");if($LASTEXITCODE -ne 0 -or ($imports -join "`n") -match 'BinaryAssetBuilder[.]TextureCompiler|d3d9[.]dll|d3dx'){throw 'Loader ABI imports original compiler or system D3D'}
 & dumpbin /exports "$destinationPath/Recovered.TextureNativeLoader.dll"
 if($LASTEXITCODE -ne 0){throw 'Native loader export inspection failed'}
}finally{Pop-Location}
