param([Parameter(Mandatory=$true)][string]$Destination,[ValidateSet('Debug','Release')][string]$Configuration='Release')
$ErrorActionPreference='Stop';$root=(Resolve-Path "$PSScriptRoot/../..").Path
$source="$root/src/TextureCompiler/PublicD3dxBc"
$manifest=Get-Content "$source/PROVENANCE.json" -Raw|ConvertFrom-Json
foreach($file in $manifest.files){$path=Join-Path $source $file.path;if((Get-Item $path).Length -ne $file.bytes -or (Get-FileHash $path).Hash.ToLowerInvariant() -ne $file.sha256){throw "Public BC source changed: $($file.path)"}}
New-Item -ItemType Directory -Force $Destination|Out-Null;$destinationPath=(Resolve-Path $Destination).Path
& "$PSScriptRoot/BuildTextureNativeLoader.ps1" -Destination $destinationPath -Configuration $Configuration
if($LASTEXITCODE -ne 0){throw 'Public BC resource consumer dependencies failed'}
$opt=if($Configuration -eq 'Release'){'/O2'}else{'/Od'}
Push-Location $destinationPath
try{
 & cl /nologo /LD /MT /EHsc /std:c++17 $opt /arch:IA32 /fp:strict /D_XM_NO_INTRINSICS_ "$source/BC.cpp" "$source/TexturePublicBcApi.cpp" "$source/TexturePublicBcDds.cpp" "$source/TexturePublicBcCopy.cpp" "$source/TexturePublicBcResize.cpp" /link "$destinationPath/native-loader.lib" "$destinationPath/native-resource.lib" "$destinationPath/native-surface.lib" "$destinationPath/native-copy.lib" /DEF:"$source/TexturePublicBcApi.def" /OUT:"$destinationPath/Recovered.TexturePublicBc.dll" /IMPLIB:"$destinationPath/public-bc.lib" /MACHINE:X86
 if($LASTEXITCODE -ne 0){throw 'Public D3DX BC candidate build failed'}
 $bytes=[IO.File]::ReadAllBytes("$destinationPath/Recovered.TexturePublicBc.dll");$pe=[BitConverter]::ToInt32($bytes,0x3c)
 if([BitConverter]::ToUInt16($bytes,$pe+4) -ne 0x14c){throw 'Public BC requires I386'}
 $imports=@(& dumpbin /imports "$destinationPath/Recovered.TexturePublicBc.dll");if($LASTEXITCODE -ne 0 -or ($imports -join "`n") -match 'BinaryAssetBuilder[.]TextureCompiler|d3d9[.]dll|d3dx'){throw 'Public BC candidate imports forbidden runtime backend'}
 & dumpbin /exports "$destinationPath/Recovered.TexturePublicBc.dll"
 if($LASTEXITCODE -ne 0){throw 'Public BC candidate export inspection failed'}
}finally{Pop-Location}
