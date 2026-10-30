$ErrorActionPreference='Stop';$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/native-loader/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT";$work=Join-Path $env:RUNNER_TEMP ('texture-native-loader-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';sdkValidated=$false;scope='PC formal owned 2D CPU resource/file loader: all old raw DDS/TGA/BMP matrices plus DirectXTex BC1/2/3 small/POT/NPOT, provided partial tails and BOX missing mips; 16180 successful files including1679 compressed,6860 constructors and19 explicit rejections; full original header/VM/mips/three locks/masked and physical RGBA/ownership, Debug/Release and per-call NoOriginal; not production package/SDK/mixed-mode identity';configurations=@();sourceFixtures=@();publicSources=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 70;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS '}}
try{
 foreach($provider in @('PublicDirectXTex','PublicStbBmp','PublicD3dxBc')){
  $providerPath="src/TextureCompiler/$provider";$provenance=Get-Content "$providerPath/PROVENANCE.json" -Raw|ConvertFrom-Json
  foreach($file in $provenance.files){$path=Join-Path $providerPath $file.path;if((Get-Item $path).Length -ne $file.bytes -or (Get-FileHash $path).Hash.ToLowerInvariant() -ne $file.sha256){throw "Public source fingerprint mismatch: $provider/$($file.path)"}}
  $r.publicSources+=@(@{repository=$provenance.repository;commit=$provenance.commit;files=$provenance.files})
 }
 Copy-Item SDK/tools/*.dll $work
 $r.originalSha256=(Get-FileHash "$work/BinaryAssetBuilder.TextureCompiler.dll").Hash.ToLowerInvariant()
 if($r.originalSha256 -ne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'){throw 'Original loader SHA mismatch'}
 $fixtures=(Resolve-Path recovery/tests/fixtures/texture-main).Path;$manifest=Get-Content "$fixtures/manifest.json" -Raw|ConvertFrom-Json
 if($manifest.files.Count -ne 12){throw 'All12 actual source inputs required'}
 foreach($f in $manifest.files){$name=[IO.Path]::GetFileName($f.fixture);$path=Join-Path $fixtures $name;$hash=(Get-FileHash $path).Hash.ToLowerInvariant();if($hash -ne $f.sha256 -or (Get-Item $path).Length -ne $f.bytes){throw "Source fixture hash/bytes changed: $name"};$r.sourceFixtures+=@{file=$name;sha256=$hash;bytes=$f.bytes}}
 foreach($mode in @('Debug','Release')){
  foreach($kind in @('Device','Descriptor','Loader')){Checked "$mode-$kind-build" {& "$PSScriptRoot/BuildTextureNative$kind.ps1" -Destination "$work/$mode" -Configuration $mode}}
  $opt=if($mode -eq 'Release'){'/O2'}else{'/Od'}
  Checked "$mode-oracle-build" {& cl /nologo /clr /EHa /std:c++17 $opt /Fo"$work/$mode/loader-oracle.obj" (Resolve-Path recovery/tests/TextureNativeLoaderOracle.cpp).Path /Fe:"$work/$mode/TextureNativeLoaderOracle.exe"}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/$mode/TextureNativeLoaderOracle.exe.config"
  Checked "$mode-differential" {& "$work/$mode/TextureNativeLoaderOracle.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$work/$mode" $fixtures "$out/$mode"}
  $isolated=Join-Path $work "isolated-$mode";New-Item -ItemType Directory -Force $isolated|Out-Null
  Copy-Item "$work/$mode/TextureNativeLoaderOracle.exe","$work/$mode/TextureNativeLoaderOracle.exe.config","$work/$mode/Recovered.TextureNative*.dll" $isolated
  Push-Location $isolated
  try{Checked "$mode-no-original" {& "$isolated/TextureNativeLoaderOracle.exe" isolated $isolated $fixtures "$out/$mode"}}finally{Pop-Location}
  $dlls=@();foreach($kind in @('Device','Resource','Surface','Descriptor','Copy','Loader')){$name="Recovered.TextureNative$kind.dll";Copy-Item "$work/$mode/$name" "$out/$mode-$name";$dlls+=@{file=$name;sha256=(Get-FileHash "$out/$mode-$name").Hash.ToLowerInvariant()}}
  $r.configurations+=@{configuration=$mode;passed=$true;withoutOriginalTextureCompiler=$true;summary=@(Get-Content "$out/$mode-differential.log","$out/$mode-no-original.log"|Where-Object{$_ -match '^PASS '});captureSha256=(Get-FileHash "$out/$mode.capture").Hash.ToLowerInvariant();captureBytes=(Get-Item "$out/$mode.capture").Length;nativeFiles=$dlls}
 }
 if((Get-FileHash "$out/Debug.capture").Hash -ne (Get-FileHash "$out/Release.capture").Hash){throw 'Complete original Debug/Release loader capture differs'}
 # Preserve every original byte losslessly, with a receipt hash for each raw
 # capture. Compression is storage only, never comparison normalization.
 Compress-Archive -Path "$out/Debug.capture","$out/Release.capture" -DestinationPath "$out/captures.zip"
 $r.captureArchiveSha256=(Get-FileHash "$out/captures.zip").Hash.ToLowerInvariant()
 Remove-Item "$out/Debug.capture","$out/Release.capture";Get-ChildItem $out -Directory -Filter '*-inputs'|Remove-Item -Recurse
 $r.matrix=@{constructors=6860;successfulFiles=16180;compressedSuccessfulFiles=1679;formalBcFiles=1572;rawNpotFiles=5665;bmpFiles=6152;explicitRejections=19};$r.status='passed'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{
 # Failed full-byte traces are also kept compressed, avoiding oversized Git
 # objects while retaining the exact mismatch for diagnosis.
 if($r.status -eq 'failed'){$traces=@(Get-ChildItem $out -File|Where-Object{$_.Extension -in '.capture','.actual','.expected','.unsupported-original'});if($traces.Count){Compress-Archive -Path $traces.FullName -DestinationPath "$out/failure-captures.zip";Remove-Item $traces.FullName};Get-ChildItem $out -Directory -Filter '*-inputs'|Remove-Item -Recurse}
 $r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"
}
