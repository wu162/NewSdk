$ErrorActionPreference='Stop';$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/native-consumers/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT";$work=Join-Path $env:RUNNER_TEMP ('texture-native-consumers-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';sdkValidated=$false;scope='Four independent PC native ABI libraries consumed together on complete original-loader raw resources for all12 fixed real DDS/TGA inputs; image loader remains oracle-only, not recovered loader/mixed-mode identity/SDK or production-package acceptance';configurations=@();sourceFixtures=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 60;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS '}}
try{
 Copy-Item SDK/tools/*.dll $work
 $r.originalSha256=(Get-FileHash "$work/BinaryAssetBuilder.TextureCompiler.dll").Hash.ToLowerInvariant()
 $fixtures=(Resolve-Path recovery/tests/fixtures/texture-main).Path;$manifest=Get-Content "$fixtures/manifest.json" -Raw|ConvertFrom-Json
 if($manifest.files.Count -ne 12){throw 'All12 actual source inputs required'}
 foreach($f in $manifest.files){$name=[IO.Path]::GetFileName($f.fixture);$path=Join-Path $fixtures $name;$hash=(Get-FileHash $path).Hash.ToLowerInvariant();if($hash -ne $f.sha256 -or (Get-Item $path).Length -ne $f.bytes){throw "Source fixture hash/bytes changed: $name"};$r.sourceFixtures+=@{file=$name;sha256=$hash;bytes=$f.bytes}}
 foreach($mode in @('Debug','Release')){
  foreach($kind in @('Device','Resource','Surface','Descriptor')){Checked "$mode-$kind-build" {& "$PSScriptRoot/BuildTextureNative$kind.ps1" -Destination "$work/$mode" -Configuration $mode}}
  $opt=if($mode -eq 'Release'){'/O2'}else{'/Od'}
  Checked "$mode-oracle-build" {& cl /nologo /clr /EHa /std:c++17 $opt /Fo"$work/$mode/consumer.obj" (Resolve-Path recovery/tests/TextureNativeConsumerOracle.cpp).Path /Fe:"$work/$mode/TextureNativeConsumerOracle.exe"}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/$mode/TextureNativeConsumerOracle.exe.config"
  Checked "$mode-differential" {& "$work/$mode/TextureNativeConsumerOracle.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$work/$mode" $fixtures "$out/$mode"}
  $isolated=Join-Path $work "isolated-$mode";New-Item -ItemType Directory -Force $isolated|Out-Null
  Copy-Item "$work/$mode/TextureNativeConsumerOracle.exe","$work/$mode/TextureNativeConsumerOracle.exe.config","$work/$mode/Recovered.TextureNative*.dll" $isolated
  Push-Location $isolated
  try{Checked "$mode-no-original" {& "$isolated/TextureNativeConsumerOracle.exe" isolated $isolated '-' "$out/$mode"}}finally{Pop-Location}
  $dlls=@();foreach($kind in @('Device','Resource','Surface','Descriptor')){$name="Recovered.TextureNative$kind.dll";Copy-Item "$work/$mode/$name" "$out/$mode-$name";$dlls+=@{file=$name;sha256=(Get-FileHash "$out/$mode-$name").Hash.ToLowerInvariant()}}
  $r.configurations+=@{configuration=$mode;passed=$true;withoutOriginalTextureCompiler=$true;summary=@(Get-Content "$out/$mode-differential.log","$out/$mode-no-original.log"|Where-Object{$_ -match '^PASS '});captureSha256=(Get-FileHash "$out/$mode.capture").Hash.ToLowerInvariant();fixtureSha256=(Get-FileHash "$out/$mode.fixtures").Hash.ToLowerInvariant();nativeFiles=$dlls}
 }
 foreach($extension in @('capture','fixtures')){if((Get-FileHash "$out/Debug.$extension").Hash -ne (Get-FileHash "$out/Release.$extension").Hash){throw "Complete original Debug/Release consumer $extension differs"}}
 $r.status='passed'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"}
