$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/native-surface/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-native-surface-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';sdkValidated=$false;scope='Native x86 PC type3 complete mip/packed/border/tiled/array format layout and public LockRect, full state/owned-storage differential and isolation; no mixed-mode identity/nontexture descriptors or production package claim';configurations=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 50;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS '}}
try{
 Copy-Item SDK/tools/*.dll $work
 $r.originalSha256=(Get-FileHash "$work/BinaryAssetBuilder.TextureCompiler.dll").Hash.ToLowerInvariant()
 foreach($mode in @('Debug','Release')){
  Checked "$mode-native-build" {& "$PSScriptRoot/BuildTextureNativeSurface.ps1" -Destination "$work/$mode" -Configuration $mode}
  $opt=if($mode -eq 'Release'){'/O2'}else{'/Od'}
  Checked "$mode-static-link-build" {& cl /nologo /MT /EHsc /std:c++17 $opt (Resolve-Path recovery/tests/TextureNativeSurfaceLink.cpp).Path /Fo"$work/$mode/surface-link.obj" /Fe:"$work/$mode/TextureNativeSurfaceLink.exe" /link "$work/$mode/native-surface.lib"}
  Checked "$mode-static-link" {& "$work/$mode/TextureNativeSurfaceLink.exe"}
  Checked "$mode-oracle-build" {& cl /nologo /clr /EHa /std:c++17 $opt /Fo"$work/$mode/oracle.obj" (Resolve-Path recovery/tests/TextureNativeSurfaceOracle.cpp).Path /Fe:"$work/$mode/TextureNativeSurfaceOracle.exe"}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/$mode/TextureNativeSurfaceOracle.exe.config"
  Checked "$mode-differential" {& "$work/$mode/TextureNativeSurfaceOracle.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$work/$mode/Recovered.TextureNativeSurface.dll" "$out/$mode.capture"}
  $isolated=Join-Path $work "isolated-$mode";New-Item -ItemType Directory -Force $isolated|Out-Null
  Copy-Item "$work/$mode/TextureNativeSurfaceOracle.exe","$work/$mode/TextureNativeSurfaceOracle.exe.config","$work/$mode/Recovered.TextureNativeSurface.dll" $isolated
  Push-Location $isolated
  try{Checked "$mode-no-original" {& "$isolated/TextureNativeSurfaceOracle.exe" isolated "$isolated/Recovered.TextureNativeSurface.dll" "$out/$mode.capture"}}finally{Pop-Location}
  Copy-Item "$work/$mode/Recovered.TextureNativeSurface.dll" "$out/$mode-Recovered.TextureNativeSurface.dll"
  $r.configurations+=@{configuration=$mode;passed=$true;withoutOriginalTextureCompiler=$true;summary=@(Get-Content "$out/$mode-differential.log","$out/$mode-no-original.log"|Where-Object{$_ -match '^PASS '});captureSha256=(Get-FileHash "$out/$mode.capture").Hash.ToLowerInvariant();nativeSha256=(Get-FileHash "$out/$mode-Recovered.TextureNativeSurface.dll").Hash.ToLowerInvariant()}
 }
 if((Get-FileHash "$out/Debug.capture").Hash -ne (Get-FileHash "$out/Release.capture").Hash){throw 'Complete original Debug/Release surface captures differ'}
 $r.status='passed'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 10|Set-Content "$out/validation.json"}
