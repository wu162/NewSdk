$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/dxt1-boundary/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-dxt1-boundary-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$true;evidenceOnly=$true;sdkValidated=$false;scope='Original-only PC default71 DXT1/1A stack-boundary with DXT3/5 controls; no binary patch or alternate codec, not production acceptance';processes=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 50;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PROBE '}}
try{
 Copy-Item SDK/tools/*.dll $work
 $r.originalSha256=(Get-FileHash "$work/BinaryAssetBuilder.TextureCompiler.dll").Hash.ToLowerInvariant()
 if($r.originalSha256 -ne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'){throw 'Original DLL mismatch'}
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 Checked native-build {& cl /nologo /LD /MT /O2 /GS- /Fo"$work/boundary.obj" (Resolve-Path recovery/tests/TextureDxt1BoundaryNative.cpp).Path /link /OUT:"$work/TextureDxt1BoundaryNative.dll" /MACHINE:X86}
 Checked probe-build {& $csc /nologo /unsafe /platform:x86 /out:"$work/TextureDxt1BoundaryProbe.exe" (Resolve-Path recovery/tests/TextureDxt1BoundaryProbe.cs).Path}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureDxt1BoundaryProbe.exe.config"
 for($process=0;$process -lt 3;$process++){
  Checked "process-$process" {& "$work/TextureDxt1BoundaryProbe.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$out/samples/process-$process"}
  if(@(Get-ChildItem "$out/samples/process-$process/dds" -File).Count -ne 1280){throw 'Incomplete original process corpus'}
  Copy-Item "$out/samples/process-$process/transcript.tsv","$out/samples/process-$process/variations.tsv" $out -ErrorAction SilentlyContinue
  $r.processes+=@{process=$process;fixtureCount=1280;summary=@(Get-Content "$out/process-$process.log"|Where-Object{$_ -match '^PROBE '})}
 }
 $r.fixtureCount=3840
 Compress-Archive -Path "$out/samples/*" -DestinationPath "$out/samples.zip"
 $r.samplesSha256=(Get-FileHash "$out/samples.zip").Hash.ToLowerInvariant()
 Remove-Item "$out/samples" -Recurse -Force
 $r.status='probe-only'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"}
