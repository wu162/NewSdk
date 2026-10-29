$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/public-ppm/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-ppm-replay-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$false;sdkValidated=$false;productionAccepted=$false;scope='Independent adapted IJG P3/P6 full immutable original corpus, Decode/Load, malformed binary/numeric safety, per-call NoOriginal; production loader/plugin/SDK not enabled';configurations=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 50;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS '}}
try{
 # All cheap immutable input checks precede native/managed builds.
 $baseline='recovery/evidence/texture/ppm-load/40836345313-1'
 $receipt=Get-Content "$baseline/validation.json" -Raw|ConvertFrom-Json
 $gold='00c9854e3f026723d378afa05c1b1241e30c23b7a37f40b718ced2885e7ee339'
 if($receipt.status -ne 'probe-only' -or $receipt.sourceCommit -ne '7ab333561a9c4fee7e37eb98d3693469b905855e' -or $receipt.fixtureCount -ne 8438 -or $receipt.decodedCount -ne 6615 -or $receipt.rejectedCount -ne 1823 -or $receipt.unstableOriginals -ne 0){throw 'Wrong immutable original PPM corpus'}
 if($receipt.originalSha256 -ne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e' -or $receipt.samplesSha256 -ne $gold -or (Get-FileHash "$baseline/samples.zip").Hash.ToLowerInvariant() -ne $gold){throw 'Original PPM baseline fingerprint changed'}
 git merge-base --is-ancestor $receipt.sourceCommit HEAD;if($LASTEXITCODE -ne 0){throw 'Original PPM source is not ancestor'}
 $r.originalBaseline=@{sourceCommit=$receipt.sourceCommit;runId=$receipt.runId;samplesSha256=$gold}
 $r.publicSource=Get-Content src/TextureCompiler/Ppm/PROVENANCE.json -Raw|ConvertFrom-Json
 Expand-Archive "$baseline/samples.zip" "$work/samples"
 Get-ChildItem SDK/tools/*.dll|Where-Object Name -ne 'BinaryAssetBuilder.TextureCompiler.dll'|Copy-Item -Destination $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 foreach($mode in @('Debug','Release')){
  Checked "$mode-native-build" {& "$PSScriptRoot/BuildTexturePpm.ps1" -Destination "$work/native-$mode" -Configuration $mode}
  Copy-Item "$work/native-$mode/Recovered.TexturePpm.dll" $work -Force
  Checked "$mode-managed-build" {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c $mode -o "$work/build-$mode" --nologo}
  Copy-Item "$work/build-$mode/Recovered.BinaryAssetBuilder.TextureCompiler.dll" $work -Force
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  $references=@("/r:$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll","/r:$work/BinaryAssetBuilder.Core.dll")
  Checked "$mode-candidate-build" {& $csc /nologo /target:library /platform:x86 $opt @references /out:"$work/Recovered.TexturePpm.Candidate.dll" (Resolve-Path src/TextureCompiler/Ppm/TexturePpmDecoder.cs).Path}
  Checked "$mode-replay-build" {& $csc /nologo /platform:x86 $opt @references /r:"$work/Recovered.TexturePpm.Candidate.dll" /out:"$work/TexturePpmReplay.exe" (Resolve-Path recovery/tests/TexturePpmReplay.cs).Path}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TexturePpmReplay.exe.config"
  Checked "$mode-replay" {& "$work/TexturePpmReplay.exe" "$work/samples"}
  $r.configurations+=@{configuration=$mode;passed=$true;withoutOriginalTextureCompiler=$true;summary=@(Get-Content "$out/$mode-replay.log"|Where-Object{$_ -match '^PASS '})}
 }
 $r.status='passed'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 16|Set-Content "$out/validation.json"}
