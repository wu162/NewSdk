$ErrorActionPreference='Stop';$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/native-loader/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT";$work=Join-Path $env:RUNNER_TEMP ('texture-native-loader-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';sdkValidated=$false;scope='PC owned 2D CPU resource creation and actual raw DDS partial/complete-chain/BOX missing-mip and POT/NPOT TGA24/32 raw/RLE plus raw DDS per-provided-level DirectXTex triangle file loading; 105 compressed DDS original successes remain explicitly unsupported with separate full original snapshots; full original native headers/VM/packed tails/BOX mips/descriptors/locks/copies/release, Debug/Release and independent NoOriginal; compressed missing mip/additional codecs/production SDK/mixed-mode identity excluded';configurations=@();sourceFixtures=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 70;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS '}}
try{
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
  $r.configurations+=@{configuration=$mode;passed=$true;withoutOriginalTextureCompiler=$true;summary=@(Get-Content "$out/$mode-differential.log","$out/$mode-no-original.log"|Where-Object{$_ -match '^PASS '});captureSha256=(Get-FileHash "$out/$mode.capture").Hash.ToLowerInvariant();captureBytes=(Get-Item "$out/$mode.capture").Length;unsupportedOriginalSha256=(Get-FileHash "$out/$mode.unsupported-original").Hash.ToLowerInvariant();nativeFiles=$dlls}
 }
 $r.unsupportedOriginalDebugReleaseIdentical=(Get-FileHash "$out/Debug.unsupported-original").Hash -eq (Get-FileHash "$out/Release.unsupported-original").Hash
 # Original compressed snapshots are probe-only boundaries, not restored
 # outputs; record repeat differences without weakening the supported matrix.
 if((Get-FileHash "$out/Debug.capture").Hash -ne (Get-FileHash "$out/Release.capture").Hash){throw 'Complete original Debug/Release loader capture differs'}
 # Preserve every original byte losslessly, with a receipt hash for each raw
 # capture. Compression is storage only, never comparison normalization.
 Compress-Archive -Path "$out/Debug.capture","$out/Release.capture","$out/Debug.unsupported-original","$out/Release.unsupported-original" -DestinationPath "$out/captures.zip"
 $r.captureArchiveSha256=(Get-FileHash "$out/captures.zip").Hash.ToLowerInvariant()
 Remove-Item "$out/Debug.capture","$out/Release.capture","$out/Debug.unsupported-original","$out/Release.unsupported-original";Get-ChildItem $out -Directory -Filter '*-inputs'|Remove-Item -Recurse
 $r.status='passed'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{
 # Failed full-byte traces are also kept compressed, avoiding oversized Git
 # objects while retaining the exact mismatch for diagnosis.
 if($r.status -eq 'failed'){$traces=@(Get-ChildItem $out -File|Where-Object{$_.Extension -in '.capture','.actual','.expected','.unsupported-original'});if($traces.Count){Compress-Archive -Path $traces.FullName -DestinationPath "$out/failure-captures.zip";Remove-Item $traces.FullName};Get-ChildItem $out -Directory -Filter '*-inputs'|Remove-Item -Recurse}
 $r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"
}
