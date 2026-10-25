param([string]$OutputDirectory)
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if(!$env:RUNNER_TEMP){$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
if(!$OutputDirectory){$OutputDirectory="recovery/evidence/audio/xma-dispatch-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if(Test-Path $OutputDirectory){throw "Refusing to overwrite XMA dispatch evidence: $OutputDirectory"}
New-Item -ItemType Directory $OutputDirectory -Force|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-xma-dispatch-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $work|Out-Null
$csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='Real x86 CPU dynamic original XMA indirect dispatch';xmaEncodingRecovered=$false;sdkValidated=$false;baselineSourceCommit='cd66d9a2c821bc41f1f975aac8ac0a5bac6a6499';baselineRunId='40232525056';tracedCases=0}
try {
 $baseline=(Resolve-Path recovery/evidence/audio/xma-raw-validation/40232525056-1).Path
 $validation=Get-Content "$baseline/validation.json" -Raw|ConvertFrom-Json
 if($validation.status -ne 'original-evidence-only' -or $validation.sourceCommit -ne $state.baselineSourceCommit -or $validation.cases -ne 8 -or $validation.nativeEncodeCalls -ne 16){throw 'Accepted raw baseline receipt differs'}
 Copy-Item SDK/tools/*.dll $work
 & $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Web.Extensions.dll /main:AudioXmaDispatchOracle /out:"$work/AudioXmaDispatchOracle.exe" (Resolve-Path recovery/tests/AudioXmaRawOracle.cs).Path (Resolve-Path recovery/tests/AudioXmaDispatchOracle.cs).Path *> "$out/oracle-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/oracle-build.log";throw 'XMA dispatch oracle host build failed'}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$work/AudioXmaDispatchOracle.exe.config"
 & "$work/AudioXmaDispatchOracle.exe" "$work/BinaryAssetBuilder.AudioCompiler.dll" $baseline $out *> "$out/original-cpu.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/original-cpu.log";throw 'Original XMA dynamic dispatch oracle failed'}
 Get-Content "$out/original-cpu.log"
 $trace=Get-Content "$out/dispatch.json" -Raw|ConvertFrom-Json
 if($trace.status -ne 'original-evidence-only' -or $trace.caseCount -ne 3 -or $trace.nativeEncodeCalls -ne 4 -or $trace.hitCount -lt 7 -or $trace.numericalEncoderRecovered){throw 'Incomplete dynamic dispatch evidence'}
 $state.status='original-evidence-only';$state.tracedCases=3;$state.nativeEncodeCalls=4;$state.actualCallObservations=$trace.hitCount;$state.originalRawOutputsEqualAccepted=$true;$state.dispatchReceipt='dispatch.json';$state.dispatchReceiptSha256=(Get-FileHash "$out/dispatch.json").Hash.ToLowerInvariant()
}catch{$state.status='failed';$state.error=$_.Exception.Message;throw}
finally{$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json"}
