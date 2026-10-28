$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/jpeg-metadata-load/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-jpeg-metadata-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$true;evidenceOnly=$true;sdkValidated=$false;productionAccepted=$false;scope='Original PC RGB/CMYK/YCCK JPEG and APP14/ICC/Exif/XMP/comment complete RGBA/errors; compare current pinned public IJG6b implementation; capture only, not acceptance'}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 40;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PROBE '}}
try{
 Copy-Item SDK/tools/*.dll $work
 $r.originalSha256=(Get-FileHash "$work/BinaryAssetBuilder.TextureCompiler.dll").Hash.ToLowerInvariant()
 Checked jpeg-build {& "$PSScriptRoot/BuildTextureJpeg.ps1" -Destination "$work/jpeg" -Configuration Release -BuildEncoder}
 Copy-Item "$work/jpeg/Recovered.TextureJpeg.dll" $work
 Checked colour-encoder-build {& cl /nologo /TC /MT /O2 /D_CRT_SECURE_NO_WARNINGS /I"$work/jpeg" /Fo"$work/colour-fixtures.obj" /Fe:"$work/TextureJpegColourFixtures.exe" (Resolve-Path recovery/tests/TextureJpegColourFixtures.c).Path /link "$work/jpeg/jpeg.lib" /MACHINE:X86}
 Checked support-build {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c Release -o "$work/build" --nologo}
 Copy-Item "$work/build/Recovered.BinaryAssetBuilder.TextureCompiler.dll" $work
 $baseline='recovery/evidence/texture/jpeg-load/40625499544-1';$receipt=Get-Content "$baseline/validation.json" -Raw|ConvertFrom-Json
 $hash='b758465f88b831f5952e4700531ffe5b83a9696be6889a8b9d0defdc1eee24a1'
 if($receipt.status -ne 'probe-only' -or $receipt.sourceCommit -ne '3f2351f2d0b0adccf416ba6dda7e97d49b5c92f6' -or $receipt.fixtureCount -ne 1284 -or $receipt.samplesSha256 -ne $hash -or (Get-FileHash "$baseline/samples.zip").Hash.ToLowerInvariant() -ne $hash){throw 'Wrong immutable JPEG metadata source corpus'}
 git merge-base --is-ancestor $receipt.sourceCommit HEAD;if($LASTEXITCODE -ne 0){throw 'Original JPEG source is not ancestor'}
 Expand-Archive "$baseline/samples.zip" "$work/jpeg-source";New-Item -ItemType Directory -Force "$work/base-fixtures"|Out-Null
 foreach($row in Import-Csv "$work/jpeg-source/transcript.tsv" -Delimiter "`t"){
  if($row.profile -like 'p3-q75-s2x2-progressive*'){
   if($row.error -ne ''){throw 'Expected successful JPEG metadata base input'};Copy-Item "$work/jpeg-source/$($row.label).jpg" "$work/base-fixtures"
  }
 }
 if(@(Get-ChildItem "$work/base-fixtures" -File).Count -ne 16){throw 'Expected16 focused original JPEG inputs'}
 $r.originalBaseline=@{sourceCommit=$receipt.sourceCommit;runId=$receipt.runId;samplesSha256=$hash;selected=16}
 $r.ijgProvenance=Get-Content recovery/reference/ijg6b/PROVENANCE.json -Raw|ConvertFrom-Json
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 Checked build {& $csc /nologo /unsafe /platform:x86 /optimize+ /main:TextureJpegMetadataProbe /r:System.Drawing.dll /r:"$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /out:"$work/TextureJpegMetadataProbe.exe" (Resolve-Path recovery/tests/TextureJpegMetadataProbe.cs).Path (Resolve-Path recovery/tests/TexturePngMetadataFixtures.cs).Path (Resolve-Path recovery/tests/TexturePngOracle.cs).Path}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureJpegMetadataProbe.exe.config"
 Checked oracle {& "$work/TextureJpegMetadataProbe.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$work/samples" "$work/TextureJpegColourFixtures.exe" (Resolve-Path recovery/reference/compact-icc).Path "$work/base-fixtures"}
 Copy-Item "$work/samples/transcript.tsv" "$out/transcript.tsv"
 $rows=Import-Csv "$out/transcript.tsv" -Delimiter "`t";if($rows.Count -ne 4000){throw 'Incomplete JPEG colour/metadata corpus'}
 $r.fixtureCount=$rows.Count;$r.decodedCount=@($rows|Where-Object error -eq '').Count;$r.rejectedCount=@($rows|Where-Object error -ne '').Count;$r.originalCalls=$rows.Count*2
 $r.candidateMatches=@($rows|Where-Object candidateEqual -eq 'True').Count;$r.candidateDifferences=@($rows|Where-Object candidateEqual -eq 'False').Count
 Compress-Archive -Path "$work/samples/*" -DestinationPath "$out/samples.zip"
 $r.samplesSha256=(Get-FileHash "$out/samples.zip").Hash.ToLowerInvariant();$r.status='probe-only'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{
 if($r.status -ne 'probe-only' -and (Test-Path "$work/samples")){Compress-Archive -Path "$work/samples/*" -DestinationPath "$out/partial-samples.zip" -Force}
 $r|ConvertTo-Json -Depth 16|Set-Content "$out/validation.json"
}
