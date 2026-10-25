param()
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if(!$env:RUNNER_TEMP){$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
$out="recovery/evidence/audio/xma-corpus-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
if(Test-Path $out){throw 'Refusing to overwrite XMA corpus evidence'}
New-Item -ItemType Directory $out -Force|Out-Null;$out=(Resolve-Path $out).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-xma-corpus-'+[Guid]::NewGuid().ToString('N'));New-Item -ItemType Directory $work|Out-Null
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='Original-only longer non-impulse XMA corpus from pinned real main audio and seeded stereo';xmaEncoderRecovered=$false;sdkValidated=$false}
try{
 Copy-Item SDK/tools/*.dll $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 & $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Web.Extensions.dll /main:AudioXmaCorpusOracle /out:"$work/AudioXmaCorpusOracle.exe" (Resolve-Path recovery/tests/AudioXmaRawOracle.cs).Path (Resolve-Path recovery/tests/AudioXmaCorpusOracle.cs).Path *> "$out/oracle-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/oracle-build.log";throw 'Corpus host build failed'}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/AudioXmaCorpusOracle.exe.config"
 & "$work/AudioXmaCorpusOracle.exe" "$work/BinaryAssetBuilder.AudioCompiler.dll" (Get-Location).Path $out *> "$out/original-cpu.log"
 $state.nativeExitCode=$LASTEXITCODE
 if($LASTEXITCODE -ne 0){Get-Content "$out/original-cpu.log";throw 'Original corpus failed'}
 Get-Content "$out/original-cpu.log"
 $data=Get-Content "$out/corpus.json" -Raw|ConvertFrom-Json
 if($data.status -ne 'original-evidence-only' -or $data.caseCount -ne 8 -or $data.nativeEncodeCalls -ne 16 -or $data.multiPacketCases -le 0 -or $data.multiBlockCases -le 0 -or $data.xmaEncoderRecovered){throw 'Incomplete corpus scope'}
 foreach($case in $data.cases){foreach($file in $case.files){$path=Join-Path (Join-Path "$out/cases" $case.name) $file.path;if((Get-Item $path).Length -ne $file.bytes -or (Get-FileHash $path).Hash.ToLowerInvariant() -ne $file.sha256){throw 'Corpus file fingerprint mismatch'}}}
 $state.status='original-evidence-only';$state.cases=8;$state.nativeEncodeCalls=16;$state.totalPackets=$data.totalPackets;$state.multiPacketCases=$data.multiPacketCases;$state.multiBlockCases=$data.multiBlockCases;$state.corpusSha256=(Get-FileHash "$out/corpus.json").Hash.ToLowerInvariant()
}catch{$state.status='failed';$state.error=$_.Exception.Message;throw}
finally{$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json"}
