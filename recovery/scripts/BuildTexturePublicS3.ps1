param([Parameter(Mandatory=$true)][string]$Destination,[ValidateSet('Debug','Release')][string]$Configuration='Release')
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
New-Item -ItemType Directory -Force $Destination|Out-Null;$Destination=(Resolve-Path $Destination).Path
$source=(Resolve-Path "$root/src/TextureCompiler/PublicS3").Path
$manifest=Get-Content "$source/PROVENANCE.json" -Raw|ConvertFrom-Json
foreach($file in $manifest.files){if((Get-FileHash "$source/$($file.file)").Hash.ToLowerInvariant() -ne $file.sha256){throw "S3 source fingerprint changed: $($file.file)"}}
$build=Join-Path $Destination "s3-$Configuration";New-Item -ItemType Directory -Force $build|Out-Null
$opt=if($Configuration -eq 'Release'){'/O2'}else{'/Od'}
Push-Location $build
try{
 & cl /nologo /LD /MT /EHsc /std:c++17 /arch:SSE2 /fp:strict $opt /I"$source" "$source/s3_quant.cpp" "$source/TexturePublicS3.cpp" /link /OUT:"$Destination/Recovered.TexturePublicS3.dll" /MACHINE:X86
 if($LASTEXITCODE -ne 0){throw 'Public S3 codec build failed'}
 $bytes=[IO.File]::ReadAllBytes("$Destination/Recovered.TexturePublicS3.dll");$pe=[BitConverter]::ToInt32($bytes,0x3c)
 if([BitConverter]::ToUInt16($bytes,$pe+4) -ne 0x14c){throw 'Public S3 codec must be I386'}
 $imports=@(& dumpbin /imports "$Destination/Recovered.TexturePublicS3.dll");if($LASTEXITCODE -ne 0){throw 'S3 imports inspection failed'}
 if(($imports -join "`n") -match 'BinaryAssetBuilder[.]TextureCompiler|Recovered[.]TextureDxtColour|Recovered[.]TextureDxtAlpha'){throw 'S3 must not import another encoder'}
 $imports|Write-Output
 @{configuration=$Configuration;machine='I386';sha256=(Get-FileHash "$Destination/Recovered.TexturePublicS3.dll").Hash.ToLowerInvariant();sourceManifestSha256=(Get-FileHash "$source/PROVENANCE.json").Hash.ToLowerInvariant();sourceFiles=$manifest.files;importsOtherEncoders=$false}|ConvertTo-Json -Depth 12|Set-Content "$Destination/PublicS3-native.json"
 Write-Output "PASS Public S3 native $Configuration I386 SHA256=$((Get-FileHash "$Destination/Recovered.TexturePublicS3.dll").Hash.ToLowerInvariant()); copied source only, no original/recovered encoder callback"
}finally{Pop-Location}
