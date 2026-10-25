param([string]$OutputDirectory)
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if(!$env:RUNNER_TEMP){$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
if(!$OutputDirectory){$OutputDirectory="recovery/evidence/audio/xma-numerical-dispatch-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if(Test-Path $OutputDirectory){throw "Refusing to overwrite XMA numerical dispatch evidence: $OutputDirectory"}
New-Item -ItemType Directory $OutputDirectory -Force|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-xma-numerical-dispatch-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $work|Out-Null
$csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='Real x86 CPU original numerical XMA memory-indirect dispatch';xmaEncodingRecovered=$false;sdkValidated=$false;baselineSourceCommit='cd66d9a2c821bc41f1f975aac8ac0a5bac6a6499';baselineRunId='40232525056';tracedCases=0;callSiteRva='0x97d97'}
try {
 $baseline=(Resolve-Path recovery/evidence/audio/xma-raw-validation/40232525056-1).Path
 $validation=Get-Content "$baseline/validation.json" -Raw|ConvertFrom-Json
 if($validation.status -ne 'original-evidence-only' -or $validation.sourceCommit -ne $state.baselineSourceCommit -or $validation.cases -ne 8 -or $validation.nativeEncodeCalls -ne 16){throw 'Accepted raw baseline receipt differs'}
 $state.stage='compile';$state|ConvertTo-Json -Depth 8|Set-Content "$out/progress.json"
 Copy-Item SDK/tools/*.dll $work
 & $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Web.Extensions.dll /main:AudioXmaNumericalDispatchOracle /out:"$work/AudioXmaNumericalDispatchOracle.exe" (Resolve-Path recovery/tests/AudioXmaRawOracle.cs).Path (Resolve-Path recovery/tests/AudioXmaNumericalDispatchOracle.cs).Path *> "$out/oracle-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/oracle-build.log";throw 'XMA numerical dispatch oracle build failed'}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$work/AudioXmaNumericalDispatchOracle.exe.config"
 $state.stage='native-execution';$state|ConvertTo-Json -Depth 8|Set-Content "$out/progress.json"
 & "$work/AudioXmaNumericalDispatchOracle.exe" "$work/BinaryAssetBuilder.AudioCompiler.dll" $baseline $out *> "$out/original-cpu.log"
 $state.nativeExitCode=$LASTEXITCODE
 $state|ConvertTo-Json -Depth 8|Set-Content "$out/progress.json"
 if($state.nativeExitCode -ne 0){Get-Content "$out/original-cpu.log";throw "Original numerical dispatch oracle failed exit=$($state.nativeExitCode)"}
 Get-Content "$out/original-cpu.log"
 $trace=Get-Content "$out/numerical-dispatch.json" -Raw|ConvertFrom-Json
 if($trace.status -ne 'original-evidence-only' -or $trace.caseCount -ne 2 -or $trace.nativeEncodeCalls -ne 2 -or $trace.hitCount -lt 2 -or $trace.numericalEncoderRecovered){throw 'Incomplete numerical dispatch receipt'}
 $state.status='original-evidence-only';$state.tracedCases=2;$state.actualCallObservations=$trace.hitCount;$state.acceptedRawCasesMatched=2;$state.receipt='numerical-dispatch.json';$state.receiptSha256=(Get-FileHash "$out/numerical-dispatch.json").Hash.ToLowerInvariant()
}catch{$state.status='failed';$state.error=$_.Exception.Message;throw}
finally{$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json"}
