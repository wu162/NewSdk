$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/ppm-load/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-ppm-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
# Scope byte preservation to this new run, never alter historical gold checkout.
'*.tsv -text'|Set-Content "$out/.gitattributes"
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$true;evidenceOnly=$true;sdkValidated=$false;productionAccepted=$false;scope='Bounded original PC P3/P6/PGM syntax, maxval, NPOT and full repeated RGBA/errors; adapted pinned IJG candidate; capture only'}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 50;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PROBE '}}
try{
 Copy-Item SDK/tools/*.dll $work
 $r.originalSha256=(Get-FileHash "$work/BinaryAssetBuilder.TextureCompiler.dll").Hash.ToLowerInvariant()
 if($r.originalSha256 -ne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'){throw 'Original PPM baseline changed'}
 $r.publicSource=Get-Content src/TextureCompiler/Ppm/PROVENANCE.json -Raw|ConvertFrom-Json
 Checked ppm-build {& "$PSScriptRoot/BuildTexturePpm.ps1" -Destination "$work/ppm" -Configuration Release}
 Copy-Item "$work/ppm/Recovered.TexturePpm.dll" $work
 Checked support-build {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c Release -o "$work/managed" --nologo}
 Copy-Item "$work/managed/Recovered.BinaryAssetBuilder.TextureCompiler.dll" $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 $references=@("/r:$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll","/r:$work/BinaryAssetBuilder.Core.dll")
 Checked candidate-build {& $csc /nologo /target:library /platform:x86 /optimize+ @references /out:"$work/Recovered.TexturePpm.Candidate.dll" (Resolve-Path src/TextureCompiler/Ppm/TexturePpmDecoder.cs).Path}
 $sources=@('TexturePpmProbe.cs','TexturePpmFixtures.cs','TexturePngOracle.cs')|ForEach-Object{(Resolve-Path "recovery/tests/$_").Path}
 Checked probe-build {& $csc /nologo /unsafe /platform:x86 /optimize+ /main:TexturePpmProbe /r:System.Drawing.dll @references /r:"$work/Recovered.TexturePpm.Candidate.dll" /out:"$work/TexturePpmProbe.exe" $sources}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TexturePpmProbe.exe.config"
 Checked oracle {& "$work/TexturePpmProbe.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$work/samples"}
 Copy-Item "$work/samples/transcript.tsv" "$out/transcript.tsv"
 $rows=@(Import-Csv "$out/transcript.tsv" -Delimiter "`t");if($rows.Count -ne 8438){throw 'Incomplete original PPM corpus'}
 $r.fixtureCount=$rows.Count;$r.originalCalls=$rows.Count*2
 $r.decodedCount=@($rows|Where-Object error -eq '').Count;$r.rejectedCount=@($rows|Where-Object error -ne '').Count
 $r.candidateMatches=@($rows|Where-Object candidateEqual -eq 'True').Count;$r.candidateDifferences=@($rows|Where-Object candidateEqual -eq 'False').Count
 $r.unstableOriginals=@($rows|Where-Object originalStable -ne 'True').Count
 Compress-Archive -Path "$work/samples/*" -DestinationPath "$out/samples.zip"
 $r.samplesSha256=(Get-FileHash "$out/samples.zip").Hash.ToLowerInvariant();$r.status='probe-only'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{
 if($r.status -ne 'probe-only' -and (Test-Path "$work/samples")){Compress-Archive -Path "$work/samples/*" -DestinationPath "$out/partial-samples.zip" -Force}
 $r|ConvertTo-Json -Depth 16|Set-Content "$out/validation.json"
}
