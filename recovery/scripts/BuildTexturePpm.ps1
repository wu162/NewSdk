param([Parameter(Mandatory=$true)][string]$Destination,[ValidateSet('Debug','Release')][string]$Configuration='Release')
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
New-Item -ItemType Directory -Force $Destination|Out-Null;$Destination=(Resolve-Path $Destination).Path
$manifest=Get-Content "$root/src/TextureCompiler/Ppm/PROVENANCE.json" -Raw|ConvertFrom-Json
foreach($pair in @(@($manifest.upstreamFile,$manifest.upstreamSha256),@($manifest.adaptedFile,$manifest.adaptedSha256))){
 if((Get-FileHash "$root/$($pair[0])").Hash.ToLowerInvariant() -ne $pair[1]){throw "PPM source fingerprint changed: $($pair[0])"}
}
& "$PSScriptRoot/BuildTextureJpeg.ps1" -Destination "$Destination/ijg" -Configuration $Configuration -BuildEncoder
if($LASTEXITCODE -ne 0){throw 'Pinned IJG library build failed'}
$opt=if($Configuration -eq 'Release'){'/O2'}else{'/Od'}
& cl /nologo /TC /MT $opt /fp:precise /LD /D_CRT_SECURE_NO_WARNINGS /I"$Destination/ijg" /I"$root/src/TextureCompiler/Ppm" /Fo"$Destination/TexturePpm.obj" "$root/src/TextureCompiler/Ppm/TexturePpm.c" /link "$Destination/ijg/jpeg.lib" /OUT:"$Destination/Recovered.TexturePpm.dll" /IMPLIB:"$Destination/ppm.lib" /MACHINE:X86
if($LASTEXITCODE -ne 0){throw 'Adapted public PPM candidate build failed'}
& dumpbin /exports "$Destination/Recovered.TexturePpm.dll"
if($LASTEXITCODE -ne 0){throw 'Cannot inspect PPM candidate exports'}
