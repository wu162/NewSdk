$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/dib/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-dib-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$false;sdkValidated=$false;scope='Independent PC raw DIB Decode/Load APIs; actual original raw-DIB/BMP full byte equivalence; core/INFO/V2/V3/OS2/V4/V5 and known NPOT/metadata/errors; explicit uncertain bitfields refusals; no Plugin or SDK route';configurations=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 40;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS '}}
try{
 $header='recovery/evidence/texture/bmp-header-load/40799686293-1'
 $h=Get-Content "$header/validation.json" -Raw|ConvertFrom-Json
 $headerHash='08715c9547eb6f9fb8ba1745ab3f38b954522439848caa031889c2f73deb069b'
 if($h.status -ne 'probe-only' -or $h.sourceCommit -ne '511896747c32c7863078b36ad781caae4f3763c1' -or $h.fixtureCount -ne 2616 -or $h.samplesSha256 -ne $headerHash -or (Get-FileHash "$header/samples.zip").Hash.ToLowerInvariant() -ne $headerHash){throw 'Wrong immutable BMP header source gold'}
 $os2='recovery/evidence/texture/bmp-os2/40813121025-1'
 $o=Get-Content "$os2/validation.json" -Raw|ConvertFrom-Json
 if($o.status -ne 'passed' -or $o.sourceCommit -ne '0dca148f4c43419ab53d97f132f764a06140abd3' -or @($o.configurations|Where-Object passed -eq $true).Count -ne 2 -or (Get-FileHash "$os2/corpus.zip").Hash.ToLowerInvariant() -ne $o.corpusSha256){throw 'OS2 source group must have complete acceptance before DIB reuse'}
 foreach($source in @($h.sourceCommit,$o.sourceCommit)){git merge-base --is-ancestor $source HEAD;if($LASTEXITCODE -ne 0){throw 'DIB source gold must be an ancestor'}}
 $r.originalBaselines=@(@{sourceCommit=$h.sourceCommit;runId=$h.runId;archiveSha256=$headerHash},@{sourceCommit=$o.sourceCommit;runId=$o.runId;archiveSha256=$o.corpusSha256})
 Expand-Archive "$header/samples.zip" "$work/header";Expand-Archive "$os2/corpus.zip" "$work/os2"
 Copy-Item SDK/tools/*.dll $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 foreach($mode in @('Debug','Release')){
  Checked "$mode-build" {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c $mode -o "$work/build-$mode" --nologo}
  Copy-Item "$work/build-$mode/Recovered.BinaryAssetBuilder.TextureCompiler.dll" $work -Force
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  Checked "$mode-oracle-build" {& $csc /nologo /unsafe /platform:x86 $opt /main:TextureDibOracle /r:System.Drawing.dll /r:"$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /out:"$work/TextureDibOracle.exe" (Resolve-Path recovery/tests/TextureDibOracle.cs).Path (Resolve-Path recovery/tests/TexturePngOracle.cs).Path}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureDibOracle.exe.config"
  Checked "$mode-oracle" {& "$work/TextureDibOracle.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$work/header" "$work/os2/Debug-direct.replay" "$work/cases-$mode" "$out/$mode-original.tsv"}
  $isolated=Join-Path $work "isolated-$mode";New-Item -ItemType Directory -Force $isolated|Out-Null
  Get-ChildItem "$work/*.dll"|Where-Object Name -ne 'BinaryAssetBuilder.TextureCompiler.dll'|Copy-Item -Destination $isolated
  Copy-Item "$work/TextureDibOracle.exe","$work/TextureDibOracle.exe.config" $isolated
  Checked "$mode-no-original" {& "$isolated/TextureDibOracle.exe" "$work/header" "$work/os2/Debug-direct.replay" "$isolated/cases" "$out/$mode-no-original.tsv"}
  if((Get-FileHash "$out/$mode-original.tsv").Hash -ne (Get-FileHash "$out/$mode-no-original.tsv").Hash){throw 'Original and isolated complete DIB source/outcome transcripts differ'}
  $r.configurations+=@{configuration=$mode;passed=$true;withoutOriginalTextureCompiler=$true;summary=@(Get-Content "$out/$mode-oracle.log","$out/$mode-no-original.log"|Where-Object{$_ -match '^PASS '})}
 }
 $r.status='passed';$r.fixtureCount=4560;$r.originalRawDibCallsPerConfiguration=4272;$r.originalInitializationCallsPerConfiguration=1
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"}
