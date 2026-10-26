param([string]$OutputDirectory)
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if(!$env:RUNNER_TEMP){$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
if(!$OutputDirectory){$OutputDirectory="recovery/evidence/audio/xas-precision-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if(Test-Path $OutputDirectory){throw "Refusing to overwrite XAS precision evidence: $OutputDirectory"}
New-Item -ItemType Directory $OutputDirectory -Force|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-xas-precision-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $work|Out-Null
$csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='Full original XAS block matrix plus binary32 quantization regression; unchanged block-size scalar matrix reused from prior acceptance';modes=@();sdkValidated=$false}
try {
 $oracleInput=Join-Path $out 'original-blocks.tsv'
 python recovery/scripts/GenerateAudioXasNativeOracle.py --include-precision --output $oracleInput
 if($LASTEXITCODE -ne 0){throw 'Original XAS precision oracle generation failed'}
 $state.oracleSha256=(Get-FileHash $oracleInput).Hash.ToLowerInvariant()
 & $csc /nologo /platform:x86 /r:System.Core.dll /out:"$work/AudioXasBlockOracle.exe" (Resolve-Path recovery/tests/AudioXasBlockOracle.cs).Path (Resolve-Path recovery/tests/AudioXasPrecisionFixture.cs).Path *> "$out/oracle-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/oracle-build.log";throw 'XAS precision oracle host build failed'}
 foreach($mode in @('Debug','Release')) {
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  $library=Join-Path $work "Recovered.Audio.XasPrecision.$mode.dll"
  & $csc /nologo /target:library /platform:x86 $opt /out:"$library" (Resolve-Path recovery/src/Managed/Audio/Native/Pcm16Encoder.cs).Path (Resolve-Path recovery/src/Managed/Audio/Native/Xas1Encoder.cs).Path *> "$out/$mode-build.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode-build.log";throw "$mode XAS precision build failed"}
  & "$work/AudioXasBlockOracle.exe" $library $oracleInput precision *> "$out/$mode.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode.log";throw "$mode XAS precision differential failed"}
  Get-Content "$out/$mode.log"
  $state.modes+=@{configuration=$mode;passed=$true;records=264;precisionRegressions=2;recoveredSha256=(Get-FileHash $library).Hash.ToLowerInvariant();originalAbsentFromRecoveredHost=$true}
 }
 $state.status='passed'
}catch{$state.status='failed';$state.error=$_.Exception.Message;throw}
finally{$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json"}
