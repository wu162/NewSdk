param([string]$OutputDirectory)
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if(!$env:RUNNER_TEMP){$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
if(!$OutputDirectory){$OutputDirectory="recovery/evidence/audio/xma-dct-snapshot-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if(Test-Path $OutputDirectory){throw "Refusing to overwrite XMA DCT snapshot evidence: $OutputDirectory"}
New-Item -ItemType Directory $OutputDirectory -Force|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-xma-dct-snapshot-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $work|Out-Null
$csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='Real x86 CPU actual original XMA DCT entry/return pairs';xmaEncodingRecovered=$false;sdkValidated=$false;baselineSourceCommit='cd66d9a2c821bc41f1f975aac8ac0a5bac6a6499';baselineRunId='40232525056';tracedCases=0;callSiteRva='0x97bb0'}
try {
 $baseline=(Resolve-Path recovery/evidence/audio/xma-raw-validation/40232525056-1).Path
 $validation=Get-Content "$baseline/validation.json" -Raw|ConvertFrom-Json
 if($validation.status -ne 'original-evidence-only' -or $validation.sourceCommit -ne $state.baselineSourceCommit -or $validation.cases -ne 8 -or $validation.nativeEncodeCalls -ne 16){throw 'Accepted raw baseline receipt differs'}
 $state.stage='compile';$state|ConvertTo-Json -Depth 8|Set-Content "$out/progress.json"
 Copy-Item SDK/tools/*.dll $work
 if(!(Get-Command cl.exe -ErrorAction SilentlyContinue)){throw 'Configure x86 MSVC environment before TestAudioXmaDctSnapshot.ps1'}
 & cl.exe /nologo /LD /O2 /MT /GS- /arch:SSE2 "/Fo$work/AudioXmaDctSnapshotNative.obj" "/Fe$work/AudioXmaDctSnapshotNative.dll" (Resolve-Path recovery/tests/AudioXmaDctSnapshotNative.cpp).Path /link "/IMPLIB:$work/AudioXmaDctSnapshotNative.lib" *> "$out/native-helper-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/native-helper-build.log";throw 'Native DCT snapshot helper build failed'}
 & dumpbin.exe /nologo /exports "$work/AudioXmaDctSnapshotNative.dll" *> "$out/native-helper-exports.log"
 if($LASTEXITCODE -ne 0){throw 'Native snapshot helper export audit failed'}
 foreach($entry in @('DctConfigure','DctHook','DctSetCase','DctCount','DctRecord','DctSize')){if(!(Select-String -Path "$out/native-helper-exports.log" -SimpleMatch $entry -Quiet)){throw "Missing exact x86 C export $entry"}}
 $state.nativeHelperSha256=(Get-FileHash "$work/AudioXmaDctSnapshotNative.dll").Hash.ToLowerInvariant()
 & $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Web.Extensions.dll /main:AudioXmaDctSnapshotOracle /out:"$work/AudioXmaDctSnapshotOracle.exe" (Resolve-Path recovery/tests/AudioXmaRawOracle.cs).Path (Resolve-Path recovery/tests/AudioXmaDctSnapshotOracle.cs).Path *> "$out/oracle-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/oracle-build.log";throw 'XMA DCT snapshot oracle build failed'}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$work/AudioXmaDctSnapshotOracle.exe.config"
 $state.stage='native-execution';$state|ConvertTo-Json -Depth 8|Set-Content "$out/progress.json"
 & "$work/AudioXmaDctSnapshotOracle.exe" "$work/BinaryAssetBuilder.AudioCompiler.dll" $baseline $out *> "$out/original-cpu.log"
 $state.nativeExitCode=$LASTEXITCODE
 $state|ConvertTo-Json -Depth 8|Set-Content "$out/progress.json"
 if($state.nativeExitCode -ne 0){Get-Content "$out/original-cpu.log";throw "Original DCT snapshot oracle failed exit=$($state.nativeExitCode)"}
 Get-Content "$out/original-cpu.log"
 $trace=Get-Content "$out/dct-snapshots.json" -Raw|ConvertFrom-Json
 if($trace.status -ne 'original-evidence-only' -or $trace.caseCount -ne 2 -or $trace.nativeEncodeCalls -ne 2 -or $trace.snapshotCount -lt 2 -or $trace.numericalEncoderRecovered){throw 'Incomplete DCT snapshot receipt'}
 foreach($transform in $trace.transforms){
  $dir=Join-Path "$out/transforms" $transform.name
  if((Get-FileHash "$dir/input.bin").Hash.ToLowerInvariant() -ne $transform.inputSha256 -or (Get-FileHash "$dir/output.bin").Hash.ToLowerInvariant() -ne $transform.outputSha256){throw 'DCT transform fingerprint differs'}
 }
 $state.status='original-evidence-only';$state.tracedCases=2;$state.snapshots=$trace.snapshotCount;$state.acceptedRawCasesMatched=2;$state.receipt='dct-snapshots.json';$state.receiptSha256=(Get-FileHash "$out/dct-snapshots.json").Hash.ToLowerInvariant()
}catch{$state.status='failed';$state.error=$_.Exception.Message;throw}
finally{$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json"}
