$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/dxt-volume-boundaries/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-dxt-boundaries-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$true;evidenceOnly=$true;sdkValidated=$false;scope='original-only PC DXT3/5 small/aligned volume DDS headers, callback framing and complete mip chains; no recovered acceptance'}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 40;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PROBE '}}
try{
 Copy-Item SDK/tools/*.dll $work
 $r.originalSha256=(Get-FileHash "$work/BinaryAssetBuilder.TextureCompiler.dll").Hash.ToLowerInvariant()
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 Checked build {& $csc /nologo /unsafe /platform:x86 /optimize+ /out:"$work/TextureDxtVolumeBoundaries.exe" (Resolve-Path recovery/tests/TextureDxtVolumeBoundaries.cs).Path}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureDxtVolumeBoundaries.exe.config"
 Checked oracle {& "$work/TextureDxtVolumeBoundaries.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$work/samples"}
 Copy-Item "$work/samples/transcript.tsv" "$out/transcript.tsv"
 $rows=Import-Csv "$out/transcript.tsv" -Delimiter "`t";if($rows.Count -ne 416){throw 'Incomplete boundary transcript'}
 $r.fixtureCount=$rows.Count
 $r.headerLayouts=@($rows|Select-Object format,width,height,depth,mipmaps,bytes,flags,linearSize,mipCount,caps,caps2,callbackFrames -Unique)
 Compress-Archive -Path "$work/samples/*" -DestinationPath "$out/samples.zip"
 $r.samplesSha256=(Get-FileHash "$out/samples.zip").Hash.ToLowerInvariant()
 $r.status='probe-only'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"}

