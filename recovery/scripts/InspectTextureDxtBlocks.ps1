$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/dxt-blocks/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('dxt-blocks-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$true;evidenceOnly=$true;sdkValidated=$false;scope='Original-only PC/PS3 4x4 no-mip DXT1/DXT1A/DXT3/DXT5 samples; not recovery acceptance'}
function Run([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 30;throw "$name failed"}}
try{
 Copy-Item SDK/tools/*.dll $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 Run compile {& $csc /nologo /unsafe /platform:x86 /out:"$work/TextureDxtBlockProbe.exe" (Resolve-Path recovery/tests/TextureDxtBlockProbe.cs).Path}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureDxtBlockProbe.exe.config"
 Run probe {& "$work/TextureDxtBlockProbe.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$out/samples"}
 $dds=@(Get-ChildItem "$out/samples/dds" -File)
 if($dds.Count -ne 64){throw "Expected 64 original DDS samples; got $($dds.Count)"}
 $r.fixtures=$dds.Count;$r.summary=@(Get-Content "$out/probe.log"|Where-Object{$_ -match '^PROBE Texture'})
 Copy-Item "$out/samples/transcript.tsv" "$out/transcript.tsv"
 Compress-Archive -Path "$out/samples/*" -DestinationPath "$out/samples.zip"
 $r.sampleArchiveSha256=(Get-FileHash "$out/samples.zip").Hash.ToLowerInvariant()
 Remove-Item "$out/samples" -Recurse -Force
 $r.status='probe-only'
}finally{$r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"}
