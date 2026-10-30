$ErrorActionPreference='Stop';$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/public-d3dx-bc/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT";$work=Join-Path $env:RUNNER_TEMP ('texture-public-bc-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$true;productionAccepted=$false;sdkValidated=$false;scope='public D3DX BC1/2/3 diagnostic: complete float/block bytes and complete owned DDS resource/VM/mip/lock/release; eight fixed encoder policies and wrap/clamp aggregates, not per-sample policy selection; no NoOriginal or production acceptance';configurations=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 60;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^(PROBE |POLICY )'}}
try{
 Copy-Item SDK/tools/*.dll $work
 foreach($mode in @('Debug','Release')){
  Checked "$mode-public-build" {& "$PSScriptRoot/BuildTexturePublicBc.ps1" -Destination "$work/$mode" -Configuration $mode}
  foreach($kind in @('Device','Descriptor')){Checked "$mode-$kind-build" {& "$PSScriptRoot/BuildTextureNative$kind.ps1" -Destination "$work/$mode" -Configuration $mode}}
  $opt=if($mode -eq 'Release'){'/O2'}else{'/Od'}
  Checked "$mode-oracle-build" {& cl /nologo /clr /EHa /std:c++17 $opt /Fo"$work/$mode/bc-diagnostic.obj" (Resolve-Path recovery/tests/TexturePublicBcDiagnostic.cpp).Path /Fe:"$work/$mode/TexturePublicBcDiagnostic.exe"}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/$mode/TexturePublicBcDiagnostic.exe.config"
  Checked "$mode-diagnostic" {& "$work/$mode/TexturePublicBcDiagnostic.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$work/$mode" "$out/$mode"}
  Copy-Item "$work/$mode/Recovered.TexturePublicBc.dll" "$out/$mode-Recovered.TexturePublicBc.dll"
  $r.configurations+=@{configuration=$mode;comparisonCompleted=$true;summary=@(Get-Content "$out/$mode-diagnostic.log"|Where-Object{$_ -match '^(PROBE |POLICY )'});captureSha256=(Get-FileHash "$out/$mode.capture").Hash.ToLowerInvariant();captureBytes=(Get-Item "$out/$mode.capture").Length;candidateSha256=(Get-FileHash "$out/$mode-Recovered.TexturePublicBc.dll").Hash.ToLowerInvariant()}
 }
 Compress-Archive -Path "$out/Debug.capture","$out/Release.capture" -DestinationPath "$out/captures.zip";$r.archiveSha256=(Get-FileHash "$out/captures.zip").Hash.ToLowerInvariant();Remove-Item "$out/Debug.capture","$out/Release.capture","$out/*-input.dds"
 $r.status='probe-only'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 15|Set-Content "$out/validation.json"}
