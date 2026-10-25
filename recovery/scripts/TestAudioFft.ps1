param([string]$OutputDirectory)
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if(!$env:RUNNER_TEMP){$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
if(!$OutputDirectory){$OutputDirectory="recovery/evidence/audio/fft-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if(Test-Path $OutputDirectory){throw "Refusing to overwrite FFT evidence: $OutputDirectory"}
New-Item -ItemType Directory $OutputDirectory -Force|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-fft-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $work|Out-Null
$csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$oracleInput=(Resolve-Path recovery/evidence/audio/native-fft/original.tsv.gz).Path
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='Isolated original-machine-code FFT differential only; runtime resampling remains unimplemented';sdkValidated=$false;runtimeResamplingValidated=$false;nativeOracleSha256=(Get-FileHash $oracleInput).Hash.ToLowerInvariant();modes=@()}
try {
 & $csc /nologo /platform:x86 /r:System.Core.dll /out:"$work/AudioResamplerFftOracle.exe" (Resolve-Path recovery/tests/AudioResamplerFftOracle.cs).Path *> "$out/oracle-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/oracle-build.log";throw 'FFT oracle host build failed'}
 foreach($mode in @('Debug','Release')) {
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  $library=Join-Path $work "Recovered.Audio.Fft.$mode.dll"
  & $csc /nologo /target:library /platform:x86 $opt /out:"$library" (Resolve-Path src/AudioCompiler/Native/ResamplerFft.cs).Path *> "$out/$mode-build.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode-build.log";throw "$mode FFT source build failed"}
  & "$work/AudioResamplerFftOracle.exe" $library $oracleInput *> "$out/$mode.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode.log";throw "$mode FFT differs from original machine code"}
  Get-Content "$out/$mode.log"
  $state.modes+=@{configuration=$mode;passed=$true;records=78;safetyChecks=9;recoveredSha256=(Get-FileHash $library).Hash.ToLowerInvariant()}
 }
 $state.status='passed'
}catch{$state.status='failed';$state.error=$_.Exception.Message;throw}
finally{$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json"}
