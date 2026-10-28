$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/png-metadata-load/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-png-metadata-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$true;evidenceOnly=$true;sdkValidated=$false;scope='original PC LoadImageA ICC/APNG full RGBA/errors and per-input repeat determinism; no recovered/plugin/SDK acceptance'}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 40;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PROBE '}}
try{
 Copy-Item SDK/tools/*.dll $work
 $r.originalSha256=(Get-FileHash "$work/BinaryAssetBuilder.TextureCompiler.dll").Hash.ToLowerInvariant()
 $r.profiles=Get-Content recovery/reference/compact-icc/PROVENANCE.json -Raw|ConvertFrom-Json
 Checked support-build {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c Release -o "$work/build" --nologo}
 Copy-Item "$work/build/Recovered.BinaryAssetBuilder.TextureCompiler.dll" $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 Checked build {& $csc /nologo /unsafe /platform:x86 /optimize+ /main:TexturePngMetadataProbe /r:System.Drawing.dll /r:"$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /out:"$work/TexturePngMetadataProbe.exe" (Resolve-Path recovery/tests/TexturePngMetadataProbe.cs).Path (Resolve-Path recovery/tests/TexturePngMetadataFixtures.cs).Path (Resolve-Path recovery/tests/TexturePngOracle.cs).Path}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TexturePngMetadataProbe.exe.config"
 Checked oracle {& "$work/TexturePngMetadataProbe.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$work/samples" (Resolve-Path recovery/reference/compact-icc).Path}
 Copy-Item "$work/samples/transcript.tsv" "$out/transcript.tsv"
 $rows=Import-Csv "$out/transcript.tsv" -Delimiter "`t";if($rows.Count -ne 4680){throw 'Incomplete PNG metadata corpus'}
 $r.fixtureCount=$rows.Count;$r.decodedCount=@($rows|Where-Object error -eq '').Count;$r.rejectedCount=@($rows|Where-Object error -ne '').Count;$r.originalCalls=$rows.Count*2
 Compress-Archive -Path "$work/samples/*" -DestinationPath "$out/samples.zip"
 $r.samplesSha256=(Get-FileHash "$out/samples.zip").Hash.ToLowerInvariant();$r.status='probe-only'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{
 if($r.status -ne 'probe-only' -and (Test-Path "$work/samples")){Compress-Archive -Path "$work/samples/*" -DestinationPath "$out/partial-samples.zip" -Force}
 $r|ConvertTo-Json -Depth 14|Set-Content "$out/validation.json"
}
