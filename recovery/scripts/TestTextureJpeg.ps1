$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/jpeg/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-jpeg-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$false;sdkValidated=$false;scope='PC public JPEG ImageLoader: full immutable 1284-case original pixels/metadata/errors, JPEG6b IFAST/fancy and grayscale L8 NPOT store, source invariance and per-call isolated Debug/Release';configurations=@();nativeBinaries=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 45;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS '}}
try{
 $baseline='recovery/evidence/texture/jpeg-load/40625499544-1'
 $receipt=Get-Content "$baseline/validation.json" -Raw|ConvertFrom-Json
 if($receipt.status -ne 'probe-only' -or $receipt.sourceCommit -ne '3f2351f2d0b0adccf416ba6dda7e97d49b5c92f6' -or $receipt.fixtureCount -ne 1284 -or $receipt.originalCalls -ne 2568 -or $receipt.originalSha256 -ne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e' -or (Get-FileHash "$baseline/samples.zip").Hash.ToLowerInvariant() -ne $receipt.samplesSha256){throw 'Wrong immutable JPEG original corpus'}
 git merge-base --is-ancestor $receipt.sourceCommit HEAD;if($LASTEXITCODE -ne 0){throw 'JPEG baseline not ancestor'}
 $r.originalBaseline=@{sourceCommit=$receipt.sourceCommit;runId=$receipt.runId;samplesSha256=$receipt.samplesSha256}
 Expand-Archive "$baseline/samples.zip" "$work/samples"
 Get-ChildItem SDK/tools/*.dll|Where-Object Name -ne 'BinaryAssetBuilder.TextureCompiler.dll'|Copy-Item -Destination $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 foreach($mode in @('Debug','Release')){
  Checked "$mode-native" {& "$PSScriptRoot/BuildTextureJpeg.ps1" -Destination "$work/native-$mode" -Configuration $mode}
  Copy-Item "$work/native-$mode/Recovered.TextureJpeg.dll" $work -Force
  $r.nativeBinaries+=@{configuration=$mode;machine='I386';sha256=(Get-FileHash "$work/Recovered.TextureJpeg.dll").Hash.ToLowerInvariant();sourceArchiveSha256='75c3ec241e9996504fe02a9ed4d12f16b74ade713972f3db9e65ce95cd27e35d'}
  Checked "$mode-managed-build" {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c $mode -o "$work/build-$mode" --nologo}
  Copy-Item "$work/build-$mode/*.dll" $work -Force
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  Checked "$mode-replay-build" {& $csc /nologo /platform:x86 $opt /r:"$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /out:"$work/TextureJpegReplay.exe" (Resolve-Path recovery/tests/TextureJpegReplay.cs).Path}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureJpegReplay.exe.config"
  Checked "$mode-replay" {& "$work/TextureJpegReplay.exe" "$work/samples"}
  $r.configurations+=@{configuration=$mode;passed=$true;withoutOriginalTextureCompiler=$true;summary=@(Get-Content "$out/$mode-replay.log"|Where-Object{$_ -match '^PASS '})}
 }
 $r.status='passed'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"}
