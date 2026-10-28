param([Parameter(Mandatory=$true)][string]$Destination,[ValidateSet('Debug','Release')][string]$Configuration='Release',[switch]$BuildEncoder)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
New-Item -ItemType Directory -Force $Destination|Out-Null;$Destination=(Resolve-Path $Destination).Path
$source=(Resolve-Path "$root/recovery/reference/ijg6b").Path
$manifest=Get-Content "$source/PROVENANCE.json" -Raw|ConvertFrom-Json
foreach($file in $manifest.files.PSObject.Properties){if((Get-FileHash "$source/$($file.Name)").Hash.ToLowerInvariant() -ne $file.Value){throw "IJG source changed: $($file.Name)"}}
Copy-Item "$source/*" $Destination -Force
Copy-Item "$source/jconfig.vc" "$Destination/jconfig.h"
Copy-Item "$root/src/TextureCompiler/Jpeg/TextureJpeg.c" $Destination
$previous=Get-Location
try{
 Set-Location $Destination
 $common=@('jcomapi.c','jutils.c','jerror.c','jmemmgr.c','jmemnobs.c')
 $encoder=@('jcapimin.c','jcapistd.c','jctrans.c','jcparam.c','jdatadst.c','jcinit.c','jcmaster.c','jcmarker.c','jcmainct.c','jcprepct.c','jccoefct.c','jccolor.c','jcsample.c','jchuff.c','jcphuff.c','jcdctmgr.c','jfdctfst.c','jfdctflt.c','jfdctint.c')
 $decoder=@('jdapimin.c','jdapistd.c','jdtrans.c','jdatasrc.c','jdmaster.c','jdinput.c','jdmarker.c','jdhuff.c','jdphuff.c','jdmainct.c','jdcoefct.c','jdpostct.c','jddctmgr.c','jidctfst.c','jidctflt.c','jidctint.c','jidctred.c','jdsample.c','jdcolor.c','jquant1.c','jquant2.c','jdmerge.c')
 $files=@($common+$decoder);if($BuildEncoder){$files+= $encoder}
 $opt=if($Configuration -eq 'Release'){'/O2'}else{'/Od'}
 & cl /nologo /TC /MT $opt /fp:precise /D_CRT_SECURE_NO_WARNINGS /I. /c @files
 if($LASTEXITCODE -ne 0){throw 'IJG source compilation failed'}
 & lib /nologo /out:jpeg.lib @($files|ForEach-Object{[IO.Path]::ChangeExtension($_,'obj')})
 if($LASTEXITCODE -ne 0){throw 'IJG library failed'}
 & cl /nologo /TC /MT $opt /fp:precise /LD /D_CRT_SECURE_NO_WARNINGS /I. TextureJpeg.c /link jpeg.lib /OUT:Recovered.TextureJpeg.dll /MACHINE:X86
 if($LASTEXITCODE -ne 0){throw 'PC JPEG decoder build failed'}
 & dumpbin /exports Recovered.TextureJpeg.dll
 $bytes=[IO.File]::ReadAllBytes("$Destination/Recovered.TextureJpeg.dll");$pe=[BitConverter]::ToInt32($bytes,0x3c)
 if([BitConverter]::ToUInt16($bytes,$pe+4) -ne 0x14c){throw 'JPEG decoder must be I386'}
 if($BuildEncoder){
  & cl /nologo /TC /MT $opt /D_CRT_SECURE_NO_WARNINGS /I. /Fe:cjpeg.exe cjpeg.c rdppm.c rdgif.c rdtarga.c rdrle.c rdbmp.c rdswitch.c cdjpeg.c /link jpeg.lib /MACHINE:X86
  if($LASTEXITCODE -ne 0){throw 'IJG fixture encoder failed'}
 }
}finally{Set-Location $previous}
