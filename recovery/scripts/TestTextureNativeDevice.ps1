$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/native-device/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-native-device-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';sdkValidated=$false;scope='Native x86 stdcall PC type!=2 device factory/refcount/final release, complete object/command bytes, real thread ID/allocation semantics and isolated replay; no type2/resource-populated/OOM/CLI identity or production package claim';configurations=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 50;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS '}}
try{
 Copy-Item SDK/tools/*.dll $work
 $r.originalSha256=(Get-FileHash "$work/BinaryAssetBuilder.TextureCompiler.dll").Hash.ToLowerInvariant()
 foreach($mode in @('Debug','Release')){
  Checked "$mode-native-build" {& "$PSScriptRoot/BuildTextureNativeDevice.ps1" -Destination "$work/$mode" -Configuration $mode}
  $opt=if($mode -eq 'Release'){'/O2'}else{'/Od'}
  Checked "$mode-static-link-build" {& cl /nologo /MT /EHsc /std:c++17 $opt (Resolve-Path recovery/tests/TextureNativeDeviceLink.cpp).Path /Fo"$work/$mode/device-link.obj" /Fe:"$work/$mode/TextureNativeDeviceLink.exe" /link "$work/$mode/native-device.lib"}
  Checked "$mode-static-link" {& "$work/$mode/TextureNativeDeviceLink.exe"}
  Checked "$mode-oracle-build" {& cl /nologo /clr /EHa /std:c++17 $opt /Fo"$work/$mode/oracle.obj" (Resolve-Path recovery/tests/TextureNativeDeviceOracle.cpp).Path /Fe:"$work/$mode/TextureNativeDeviceOracle.exe"}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/$mode/TextureNativeDeviceOracle.exe.config"
  Checked "$mode-differential" {& "$work/$mode/TextureNativeDeviceOracle.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$work/$mode/Recovered.TextureNativeDevice.dll" "$out/$mode.capture"}
  $isolated=Join-Path $work "isolated-$mode";New-Item -ItemType Directory -Force $isolated|Out-Null
  Copy-Item "$work/$mode/TextureNativeDeviceOracle.exe","$work/$mode/TextureNativeDeviceOracle.exe.config","$work/$mode/Recovered.TextureNativeDevice.dll" $isolated
  Push-Location $isolated
  try{Checked "$mode-no-original" {& "$isolated/TextureNativeDeviceOracle.exe" isolated "$isolated/Recovered.TextureNativeDevice.dll" "$out/$mode.capture"}}finally{Pop-Location}
  Copy-Item "$work/$mode/Recovered.TextureNativeDevice.dll" "$out/$mode-Recovered.TextureNativeDevice.dll"
  $r.configurations+=@{configuration=$mode;passed=$true;withoutOriginalTextureCompiler=$true;summary=@(Get-Content "$out/$mode-differential.log","$out/$mode-no-original.log"|Where-Object{$_ -match '^PASS '});captureSha256=(Get-FileHash "$out/$mode.capture").Hash.ToLowerInvariant();nativeSha256=(Get-FileHash "$out/$mode-Recovered.TextureNativeDevice.dll").Hash.ToLowerInvariant()}
 }
 if((Get-FileHash "$out/Debug.capture").Hash -ne (Get-FileHash "$out/Release.capture").Hash){throw 'Complete original Debug/Release device captures differ'}
 $r.status='passed'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 10|Set-Content "$out/validation.json"}
