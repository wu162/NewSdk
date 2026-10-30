param([switch]$RequireMatch,[string]$EvidenceGroup='public-d3dx-bc')
$ErrorActionPreference='Stop';$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/$EvidenceGroup/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT";$work=Join-Path $env:RUNNER_TEMP ('texture-public-bc-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$scope=if($RequireMatch){'strict public BC1/2/3 fixed XG/file codecs and default DDS file consumer, all supplied counts/small/NPOT/POT, complete original bytes, D/R and per-call NoOriginal; policy sweeps remain diagnostic'}else{'public D3DX BC1/2/3 diagnostic: full block/file-codec/default DDS resource/VM/mip/lock/release/actual-copy; eight fixed encoder policies and wrap/clamp aggregates; no production acceptance'}
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=(!$RequireMatch);productionAccepted=$false;sdkValidated=$false;scope=$scope;configurations=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 60;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^(PROBE |POLICY |FILE_POLICY |PASS )'}}
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
  $configuration=@{configuration=$mode;comparisonCompleted=$true;summary=@(Get-Content "$out/$mode-diagnostic.log"|Where-Object{$_ -match '^(PROBE |POLICY |FILE_POLICY )'});captureSha256=(Get-FileHash "$out/$mode.capture").Hash.ToLowerInvariant();captureBytes=(Get-Item "$out/$mode.capture").Length;candidateSha256=(Get-FileHash "$out/$mode-Recovered.TexturePublicBc.dll").Hash.ToLowerInvariant()}
  if($RequireMatch){
   Move-Item "$work/BinaryAssetBuilder.TextureCompiler.dll" "$work/original.hidden"
   Push-Location $work
   try{Checked "$mode-no-original" {& "$work/$mode/TexturePublicBcDiagnostic.exe" --no-original "$work/$mode" "$out/$mode.capture"}}
   finally{Pop-Location;Move-Item "$work/original.hidden" "$work/BinaryAssetBuilder.TextureCompiler.dll"}
   $configuration.passed=$true;$configuration.noOriginalPassed=$true;$configuration.withoutOriginalTextureCompiler=$true
  }
  $r.configurations+=$configuration
 }
 if($RequireMatch -and $r.configurations[0].captureSha256 -ne $r.configurations[1].captureSha256){throw 'Debug/Release complete BC captures differ'}
 $r.status=if($RequireMatch){'passed'}else{'probe-only'}
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{
 # Preserve failures too: compress every recorded byte before the normal push.
 $captures=@(Get-ChildItem "$out/*.capture" -ErrorAction SilentlyContinue)
 if($captures.Count){
  $archive=if($r.status -eq 'failed'){'failure-captures.zip'}else{'captures.zip'}
  Compress-Archive -Path @($captures.FullName) -DestinationPath "$out/$archive" -Force
  $r.archiveSha256=(Get-FileHash "$out/$archive").Hash.ToLowerInvariant()
  $r.captureMembers=@($captures|ForEach-Object{@{name=$_.Name;bytes=$_.Length;sha256=(Get-FileHash $_.FullName).Hash.ToLowerInvariant()}})
  Remove-Item @($captures.FullName)
 }
 Remove-Item "$out/*-input.dds" -ErrorAction SilentlyContinue
 $r|ConvertTo-Json -Depth 15|Set-Content "$out/validation.json"
}
