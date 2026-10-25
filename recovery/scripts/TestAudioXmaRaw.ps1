param([string]$OutputDirectory)
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if(!$env:RUNNER_TEMP){$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
if(!$OutputDirectory){$OutputDirectory="recovery/evidence/audio/xma-raw-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if(Test-Path $OutputDirectory){throw "Refusing to overwrite raw XMA evidence: $OutputDirectory"}
New-Item -ItemType Directory $OutputDirectory -Force|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-xma-raw-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $work|Out-Null
$csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='Original real x86 CPU raw XMA codec before outer packet rewriting';xmaEncodingRecovered=$false;sdkValidated=$false;audioFileXmaRouteEnabled=$false;rawBeforeOuterRewriter=$true;cases=0;nativeEncodeCalls=0}
try {
 Copy-Item SDK/tools/*.dll $work
 & $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Web.Extensions.dll /out:"$work/AudioXmaRawOracle.exe" (Resolve-Path recovery/tests/AudioXmaRawOracle.cs).Path *> "$out/oracle-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/oracle-build.log";throw 'Raw XMA oracle host build failed'}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$work/AudioXmaRawOracle.exe.config"
 $fixtures=(Resolve-Path recovery/evidence/audio/plugin-validation/40199000936-1).Path
 & "$work/AudioXmaRawOracle.exe" "$work/BinaryAssetBuilder.AudioCompiler.dll" $fixtures $out *> "$out/original-cpu.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/original-cpu.log";throw 'Original raw XMA CPU oracle failed'}
 Get-Content "$out/original-cpu.log"
 $raw=Get-Content "$out/raw-codec.json" -Raw|ConvertFrom-Json
 if($raw.status -ne 'original-evidence-only' -or $raw.caseCount -ne 8 -or $raw.nativeEncodeCalls -ne 16 -or !$raw.rawBeforeOuterRewriter -or $raw.xmaEncodingRecovered){throw 'Incomplete or overstated raw codec receipt'}
 foreach($case in $raw.cases){
  if(!$case.deterministic -or $case.repeatCount -ne 2 -or $case.rawLength -le 0){throw 'Missing raw codec repeat comparison'}
  foreach($entry in $case.files){
   $path=Join-Path (Join-Path "$out/cases" $case.name) $entry.path
   if((Get-Item $path).Length -ne $entry.length -or (Get-FileHash $path).Hash.ToLowerInvariant() -ne $entry.sha256){throw 'Raw codec file fingerprint differs'}
  }
 }
 $state.status='original-evidence-only';$state.cases=8;$state.nativeEncodeCalls=16;$state.rawCodecPacketsCaptured=$true;$state.rawCodecReceipt='raw-codec.json';$state.rawCodecReceiptSha256=(Get-FileHash "$out/raw-codec.json").Hash.ToLowerInvariant()
}catch{$state.status='failed';$state.error=$_.Exception.Message;throw}
finally{$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json"}
