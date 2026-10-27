$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/atlas-load/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-atlas-load-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$true;evidenceOnly=$true;sdkValidated=$false;scope='original-only PC TerrainTextureAtlas ProcessInstance full runtime buffers, embedded base/normal DDS, input XML/images, repeated determinism, size-count boundaries 0..49/50, normals and defined file errors; no recovered or SDK acceptance'}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 45;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PROBE '}}
try{
 Copy-Item SDK/tools/*.dll $work
 $r.originalSha256=(Get-FileHash "$work/BinaryAssetBuilder.TextureCompiler.dll").Hash.ToLowerInvariant()
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 Checked build {& $csc /nologo /platform:x86 /optimize+ /r:System.Xml.dll /r:"$work/BinaryAssetBuilder.Core.dll" /r:"$work/BinaryAssetBuilder.Utility.dll" /out:"$work/TextureAtlasLoadProbe.exe" (Resolve-Path recovery/tests/TextureAtlasLoadProbe.cs).Path}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureAtlasLoadProbe.exe.config"
 Checked oracle {& "$work/TextureAtlasLoadProbe.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$work/samples"}
 Copy-Item "$work/samples/transcript.tsv" "$out/transcript.tsv"
 $rows=Import-Csv "$out/transcript.tsv" -Delimiter "`t";if($rows.Count -ne 25){throw 'Incomplete PC atlas original ProcessInstance corpus'}
 $r.fixtureCount=$rows.Count;$r.decodedCount=@($rows|Where-Object error -eq '').Count;$r.rejectedCount=@($rows|Where-Object error -ne '').Count;$r.originalCalls=50
 Compress-Archive -Path "$work/samples/*" -DestinationPath "$out/samples.zip"
 $r.samplesSha256=(Get-FileHash "$out/samples.zip").Hash.ToLowerInvariant();$r.status='probe-only'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{
 if(Test-Path "$work/samples/current.txt"){Copy-Item "$work/samples/current.txt" "$out/current.txt" -Force}
 if($r.status -ne 'probe-only' -and (Test-Path "$work/samples")){Compress-Archive -Path "$work/samples/*" -DestinationPath "$out/partial-samples.zip" -Force;$r.partialSamplesSha256=(Get-FileHash "$out/partial-samples.zip").Hash.ToLowerInvariant()}
 $r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"
}
