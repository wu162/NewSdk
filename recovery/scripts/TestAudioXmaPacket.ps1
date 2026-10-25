param([string]$OutputDirectory)
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if(!$env:RUNNER_TEMP){$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
if(!$OutputDirectory){$OutputDirectory="recovery/evidence/audio/xma-packet-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if(Test-Path $OutputDirectory){throw "Refusing to overwrite XMA packet evidence: $OutputDirectory"}
New-Item -ItemType Directory $OutputDirectory -Force|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-xma-packets-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $work|Out-Null
$csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='Bounded integer XMA packet helpers only, original real CPU and candidate exact-byte comparison';xmaEncoderRecovered=$false;sdkValidated=$false;audioFileXmaRouteEnabled=$false;realEaBlocks=8;rawCodecPackets=0;modes=@()}
try {
 $oracleInput=Join-Path $out 'original.tsv.gz'
 python recovery/scripts/GenerateAudioXmaPacketOracle.py --output $oracleInput
 if($LASTEXITCODE -ne 0){throw 'Original bounded integer packet oracle generation failed'}
 $state.oracleSha256=(Get-FileHash $oracleInput).Hash.ToLowerInvariant()
 if($state.oracleSha256 -ne '8f64dfaf0c179e00613fd397c81122fb0cd78c6fb9eeac3e5d1b3005db01e0bf'){throw 'Pinned XMA packet oracle fingerprint changed'}
 $original=Join-Path $work 'original'
 New-Item -ItemType Directory $original|Out-Null
 Copy-Item SDK/tools/*.dll $original
 & $csc /nologo /platform:x86 /r:System.Core.dll /out:"$original/AudioXmaPacketOracle.exe" (Resolve-Path recovery/tests/AudioXmaPacketOracle.cs).Path *> "$out/oracle-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/oracle-build.log";throw 'XMA packet oracle host build failed'}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$original/AudioXmaPacketOracle.exe.config"
 & "$original/AudioXmaPacketOracle.exe" original "$original/BinaryAssetBuilder.AudioCompiler.dll" $oracleInput *> "$out/original-cpu.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/original-cpu.log";throw 'Real original CPU packet helpers differ from machine-code oracle'}
 Get-Content "$out/original-cpu.log"
 $state.originalCpuValidated=$true
 Copy-Item "$original/AudioXmaPacketOracle.exe*" $work
 foreach($mode in @('Debug','Release')) {
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  $library=Join-Path $work "Recovered.Audio.XmaPackets.$mode.dll"
  & $csc /nologo /target:library /platform:x86 $opt /out:"$library" (Resolve-Path src/AudioCompiler/Native/XmaPacketBits.cs).Path (Resolve-Path src/AudioCompiler/Native/XmaPacketScanner.cs).Path *> "$out/$mode-build.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode-build.log";throw "$mode XMA packet source build failed"}
  & "$work/AudioXmaPacketOracle.exe" candidate $library $oracleInput *> "$out/$mode.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode.log";throw "$mode XMA packet helpers differ from original"}
  Get-Content "$out/$mode.log"
  $state.modes+=@{configuration=$mode;passed=$true;summary=(Get-Content "$out/$mode.log" -Tail 1);records=1025;candidateSafetyRejections=6;recoveredSha256=(Get-FileHash $library).Hash.ToLowerInvariant();originalAbsentFromRecoveredHost=$true}
 }
 $state.status='passed'
}catch{$state.status='failed';$state.error=$_.Exception.Message;throw}
finally{$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json"}
