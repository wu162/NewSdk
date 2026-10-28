$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/jpeg-load/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-jpeg-load-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$true;evidenceOnly=$true;sdkValidated=$false;scope='original PC LoadImageA JPEG full pixels and repeat determinism; IJG6b six decoder option candidates; no public plugin or SDK acceptance'}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 50;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PROBE '}}
try{
 Copy-Item SDK/tools/*.dll $work
 $r.originalSha256=(Get-FileHash "$work/BinaryAssetBuilder.TextureCompiler.dll").Hash.ToLowerInvariant()
 Checked candidate-build {& "$PSScriptRoot/BuildTextureJpegCandidate.ps1" -Destination "$work/native"}
 Copy-Item "$work/native/TextureJpegCandidate.dll" $work
 $r.candidateSha256=(Get-FileHash "$work/TextureJpegCandidate.dll").Hash.ToLowerInvariant()
 $r.referenceSource=Get-Content recovery/reference/ijg6b/PROVENANCE.json -Raw|ConvertFrom-Json
 Checked fixture-support-build {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c Release -o "$work/build" --nologo}
 Copy-Item "$work/build/Recovered.BinaryAssetBuilder.TextureCompiler.dll" $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 Checked build {& $csc /nologo /unsafe /platform:x86 /optimize+ /r:"$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /out:"$work/TextureJpegLoadProbe.exe" (Resolve-Path recovery/tests/TextureJpegLoadProbe.cs).Path}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureJpegLoadProbe.exe.config"
 Checked oracle {& "$work/TextureJpegLoadProbe.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$work/samples" "$work/native/cjpeg.exe"}
 Copy-Item "$work/samples/transcript.tsv" "$out/transcript.tsv"
 $rows=Import-Csv "$out/transcript.tsv" -Delimiter "`t";if($rows.Count -ne 1284){throw 'Incomplete JPEG option corpus'}
 $r.fixtureCount=$rows.Count;$r.decodedCount=@($rows|Where-Object error -eq '').Count;$r.rejectedCount=@($rows|Where-Object error -ne '').Count;$r.originalCalls=$rows.Count*2
 $r.exactByMode=@{};foreach($mode in @('islow-box','ifast-box','float-box','islow-fancy','ifast-fancy','float-fancy')){$r.exactByMode[$mode]=@($rows|Where-Object{$_.$mode.StartsWith('same:')}).Count}
 Compress-Archive -Path "$work/samples/*" -DestinationPath "$out/samples.zip"
 $r.samplesSha256=(Get-FileHash "$out/samples.zip").Hash.ToLowerInvariant();$r.status='probe-only'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{
 if($r.status -ne 'probe-only' -and (Test-Path "$work/samples")){Compress-Archive -Path "$work/samples/*" -DestinationPath "$out/partial-samples.zip" -Force}
 $r|ConvertTo-Json -Depth 14|Set-Content "$out/validation.json"
}
