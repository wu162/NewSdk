$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/native-bc-build/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-native-bc-build-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';compilationOnly=$true;differentialExecuted=$false;noOriginalExecuted=$false;productionAccepted=$false;sdkValidated=$false;configurations=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 70;throw "$name failed"}}
try{
 foreach($mode in @('Debug','Release')){
  foreach($kind in @('Device','Descriptor')){Checked "$mode-$kind-build" {& "$PSScriptRoot/BuildTextureNative$kind.ps1" -Destination "$work/$mode" -Configuration $mode}}
  # PublicBc also builds formal Loader/Copy/Resource/Surface. This verifies
  # both embedded cores and the independent consumer, without running DLLs.
  Checked "$mode-public-and-native-build" {& "$PSScriptRoot/BuildTexturePublicBc.ps1" -Destination "$work/$mode" -Configuration $mode}
  $opt=if($mode -eq 'Release'){'/O2'}else{'/Od'}
  foreach($hostName in @('TextureNativeLoaderOracle','TexturePublicBcDiagnostic')){
   Checked "$mode-$hostName-build" {& cl /nologo /clr /EHa /std:c++17 $opt /Fo"$work/$mode/$hostName.obj" (Resolve-Path "recovery/tests/$hostName.cpp").Path /Fe:"$work/$mode/$hostName.exe"}
  }
  $files=@();foreach($dll in Get-ChildItem "$work/$mode/Recovered.Texture*.dll"){
   $bytes=[IO.File]::ReadAllBytes($dll.FullName);$pe=[BitConverter]::ToInt32($bytes,0x3c)
   if([BitConverter]::ToUInt16($bytes,$pe+4) -ne 0x14c){throw "Not I386: $($dll.Name)"}
   Copy-Item $dll.FullName "$out/$mode-$($dll.Name)"
   $files+=@{file=$dll.Name;sha256=(Get-FileHash $dll.FullName).Hash.ToLowerInvariant();machine='I386'}
  }
  if($files.Count -ne 7){throw 'All six native DLLs and independent BC DLL required'}
  $r.configurations+=@{configuration=$mode;compiled=$true;hostCompilationCompleted=$true;files=$files}
 }
 $r.status='compiled-only'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 12|Set-Content "$out/compilation.json"}
