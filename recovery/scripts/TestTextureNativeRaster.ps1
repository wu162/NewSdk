$ErrorActionPreference='Stop';$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/native-raster/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-native-raster-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';sdkValidated=$false;productionAccepted=$false;scope='PC formal native JPEG/PPM resource loader: complete immutable 13722 inputs,9335 original successes and4387 original errors; original HRESULT/header/VM/all mips/three locks/L8+X8 masked and physical RGBA/ownership, Debug/Release and per-call NoOriginal; raw/DDS/TGA/BMP/BC base regression is a separate unchanged full group; not production package or SDK acceptance';configurations=@();sourceCorpora=@();publicSources=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 70;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS '}}
try{
 # Read the exact historical archives, not a regenerated candidate corpus.
 # The source-only extraction retains all known original errors, and never
 # adds new binary truncations to the original parser's unsafe paths.
 $sources=@(
  @{name='ppm';directory='ppm-load/40836345313-1';sourceCommit='7ab333561a9c4fee7e37eb98d3693469b905855e';runId='40836345313';sha256='00c9854e3f026723d378afa05c1b1241e30c23b7a37f40b718ced2885e7ee339';extension='.ppm';count=8438;decoded=6615;rejected=1823},
  @{name='jpeg';directory='jpeg-load/40625499544-1';sourceCommit='3f2351f2d0b0adccf416ba6dda7e97d49b5c92f6';runId='40625499544';sha256='b758465f88b831f5952e4700531ffe5b83a9696be6889a8b9d0defdc1eee24a1';extension='.jpg';count=1284;decoded=1280;rejected=4},
  @{name='jpeg-metadata';directory='jpeg-metadata-load/40783041481-1';sourceCommit='7ff6dc66e8d847718fcaf4926b7eecdce46c05ac';runId='40783041481';sha256='ef7313c2061dcc28b580e8447c9dc8e01a8dfe7b08025fd19bab8ffe5ee5acca';extension='.jpg';count=4000;decoded=1440;rejected=2560}
 )
 $originalSha='4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
 foreach($source in $sources){
  $baseline="recovery/evidence/texture/$($source.directory)";$receipt=Get-Content "$baseline/validation.json" -Raw|ConvertFrom-Json
  if($receipt.status -ne 'probe-only' -or $receipt.sourceCommit -ne $source.sourceCommit -or $receipt.runId -ne $source.runId -or $receipt.fixtureCount -ne $source.count -or $receipt.decodedCount -ne $source.decoded -or $receipt.rejectedCount -ne $source.rejected -or $receipt.originalCalls -ne 2*$source.count -or $receipt.originalSha256 -ne $originalSha -or $receipt.samplesSha256 -ne $source.sha256){throw "Wrong immutable native raster baseline: $($source.name)"}
  if((Get-FileHash "$baseline/samples.zip").Hash.ToLowerInvariant() -ne $source.sha256){throw "Raster source archive changed: $($source.name)"}
  git merge-base --is-ancestor $source.sourceCommit HEAD;if($LASTEXITCODE -ne 0){throw "Raster baseline not ancestor: $($source.name)"}
 }
 foreach($provider in @('PublicDirectXTex','PublicStbBmp','PublicD3dxBc')){
  $providerPath="src/TextureCompiler/$provider";$provenance=Get-Content "$providerPath/PROVENANCE.json" -Raw|ConvertFrom-Json
  foreach($file in $provenance.files){$path=Join-Path $providerPath $file.path;if((Get-Item $path).Length -ne $file.bytes -or (Get-FileHash $path).Hash.ToLowerInvariant() -ne $file.sha256){throw "Public source fingerprint mismatch: $provider/$($file.path)"}}
  $r.publicSources+=@{provider=$provider;repository=$provenance.repository;commit=$provenance.commit;files=$provenance.files}
 }
 $ijg=Get-Content recovery/reference/ijg6b/PROVENANCE.json -Raw|ConvertFrom-Json
 foreach($file in $ijg.files.PSObject.Properties){if((Get-FileHash "recovery/reference/ijg6b/$($file.Name)").Hash.ToLowerInvariant() -ne $file.Value){throw "IJG source changed: $($file.Name)"}}
 $ppm=Get-Content src/TextureCompiler/Ppm/PROVENANCE.json -Raw|ConvertFrom-Json
 foreach($pair in @(@($ppm.upstreamFile,$ppm.upstreamSha256),@($ppm.adaptedFile,$ppm.adaptedSha256))){if((Get-FileHash "$root/$($pair[0])").Hash.ToLowerInvariant() -ne $pair[1]){throw "PPM source changed: $($pair[0])"}}
 $r.ijgProvenance=$ijg;$r.ppmProvenance=$ppm
 Add-Type -AssemblyName System.IO.Compression.FileSystem
 Add-Type -AssemblyName System.IO.Compression
 $corpus=Join-Path $work 'corpus';New-Item -ItemType Directory -Force $corpus|Out-Null
 $manifest=[Collections.Generic.List[object]]::new()
 foreach($source in $sources){
  $destination=Join-Path $corpus $source.name;New-Item -ItemType Directory -Force $destination|Out-Null
  $archive=[IO.Compression.ZipFile]::OpenRead("$root/recovery/evidence/texture/$($source.directory)/samples.zip")
  try{
   $entries=@($archive.Entries|Where-Object{[IO.Path]::GetExtension($_.Name) -eq $source.extension -and $_.Name -ne 'encoded.jpg'}|Sort-Object FullName)
   if($entries.Count -ne $source.count){throw "Complete source-only archive count: $($source.name)"}
   $seen=@{}
   foreach($entry in $entries){
    if($entry.FullName -ne $entry.Name -or $seen.ContainsKey($entry.Name)){throw 'Nested or duplicate raster source name'};$seen[$entry.Name]=$true
    $target=Join-Path $destination $entry.Name;[IO.Compression.ZipFileExtensions]::ExtractToFile($entry,$target,$false)
    $manifest.Add(@{corpus=$source.name;file=$entry.Name;bytes=$entry.Length;sha256=(Get-FileHash $target).Hash.ToLowerInvariant()})
   }
  }finally{$archive.Dispose()}
  $r.sourceCorpora+=@{name=$source.name;sourceCommit=$source.sourceCommit;runId=$source.runId;archiveSha256=$source.sha256;files=$source.count;originalDecoded=$source.decoded;originalRejected=$source.rejected;sourceOnly=$true;temporaryEncodedJpegExcluded=$source.extension -eq '.jpg'}
 }
 if($manifest.Count -ne 13722){throw 'Full native raster source count changed'}
 $manifest|ConvertTo-Json -Depth 6|Set-Content "$out/source-manifest.json"
 $r.sourceManifestSha256=(Get-FileHash "$out/source-manifest.json").Hash.ToLowerInvariant()
 Copy-Item SDK/tools/*.dll $work
 $r.originalSha256=(Get-FileHash "$work/BinaryAssetBuilder.TextureCompiler.dll").Hash.ToLowerInvariant()
 if($r.originalSha256 -ne $originalSha){throw 'Original native raster DLL SHA mismatch'}
 $fixtures=(Resolve-Path recovery/tests/fixtures/texture-main).Path
 foreach($mode in @('Debug','Release')){
  foreach($kind in @('Device','Descriptor','Loader')){Checked "$mode-$kind-build" {& "$PSScriptRoot/BuildTextureNative$kind.ps1" -Destination "$work/$mode" -Configuration $mode}}
  $opt=if($mode -eq 'Release'){'/O2'}else{'/Od'}
  Checked "$mode-oracle-build" {& cl /nologo /clr /EHa /std:c++17 $opt /Fo"$work/$mode/loader-oracle.obj" (Resolve-Path recovery/tests/TextureNativeLoaderOracle.cpp).Path /Fe:"$work/$mode/TextureNativeLoaderOracle.exe"}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/$mode/TextureNativeLoaderOracle.exe.config"
  Checked "$mode-differential" {& "$work/$mode/TextureNativeLoaderOracle.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$work/$mode" $fixtures "$out/$mode" $corpus}
  $isolated=Join-Path $work "isolated-$mode";New-Item -ItemType Directory -Force $isolated|Out-Null
  # Only the six formal native DLLs are copied. Neither the original compiler
  # nor separate JPEG/PPM DLLs may supply decoding during isolated replay.
  Copy-Item "$work/$mode/TextureNativeLoaderOracle.exe","$work/$mode/TextureNativeLoaderOracle.exe.config","$work/$mode/Recovered.TextureNative*.dll" $isolated
  Push-Location $isolated
  try{Checked "$mode-no-original" {& "$isolated/TextureNativeLoaderOracle.exe" isolated $isolated $fixtures "$out/$mode" $corpus}}finally{Pop-Location}
  $dlls=@();foreach($kind in @('Device','Resource','Surface','Descriptor','Copy','Loader')){$name="Recovered.TextureNative$kind.dll";Copy-Item "$work/$mode/$name" "$out/$mode-$name";$dlls+=@{file=$name;sha256=(Get-FileHash "$out/$mode-$name").Hash.ToLowerInvariant()}}
  $r.configurations+=@{configuration=$mode;passed=$true;withoutOriginalTextureCompiler=$true;summary=@(Get-Content "$out/$mode-differential.log","$out/$mode-no-original.log"|Where-Object{$_ -match '^PASS '});captureSha256=(Get-FileHash "$out/$mode.capture").Hash.ToLowerInvariant();captureBytes=(Get-Item "$out/$mode.capture").Length;nativeFiles=$dlls}
 }
 if((Get-FileHash "$out/Debug.capture").Hash -ne (Get-FileHash "$out/Release.capture").Hash){throw 'Complete original Debug/Release raster capture differs'}
 Compress-Archive -Path "$out/Debug.capture","$out/Release.capture" -DestinationPath "$out/captures.zip"
 $r.captureArchiveSha256=(Get-FileHash "$out/captures.zip").Hash.ToLowerInvariant()
 Remove-Item "$out/Debug.capture","$out/Release.capture";Get-ChildItem $out -Directory -Filter '*-inputs'|Remove-Item -Recurse
 $r.matrix=@{files=13722;originalSuccesses=9335;originalErrors=4387;ppm=8438;jpeg=1284;jpegMetadata=4000;separateBaseConstructors=6860;separateBaseFiles=16180;separateBaseRejections=19};$r.status='passed'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{
 if($r.status -eq 'failed'){$traces=@(Get-ChildItem $out -File|Where-Object{$_.Extension -in '.capture','.actual','.expected'});if($traces.Count){Compress-Archive -Path $traces.FullName -DestinationPath "$out/failure-captures.zip";Remove-Item $traces.FullName};Get-ChildItem $out -Directory -Filter '*-inputs'|Remove-Item -Recurse}
 $r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"
}
