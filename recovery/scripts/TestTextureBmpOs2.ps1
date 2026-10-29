$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/bmp-os2/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-bmp-os2-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$false;sdkValidated=$false;scope='PC core12/OS2-64 BI_RGB16/24/32 and indexed original rejection; direct/public full RGBA/errors and real plugin raw/DXT/transparent DXT1A/mip/volume/positive atlas/three buffers/full cdata; isolated replay each call; SDK separate';configurations=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 50;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS '}}
try{
 Copy-Item SDK/tools/*.dll $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 foreach($mode in @('Debug','Release')){
  Checked "$mode-managed-build" {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c $mode -o "$work/build-$mode" --nologo}
  Copy-Item "$work/build-$mode/*.dll" $work -Force
  Checked "$mode-public-s3" {& "$PSScriptRoot/BuildTexturePublicS3.ps1" -Destination $work -Configuration $mode}
  $r.publicS3Native=@($r.publicS3Native|Where-Object{$_})+@((Get-Content "$work/PublicS3-native.json" -Raw|ConvertFrom-Json))
  $dll="$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll";$opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  $directSources=@('TextureBmpOs2Oracle.cs','TextureBmpOs2Fixtures.cs','TextureBmpOracle.cs','TexturePngOracle.cs')|ForEach-Object{(Resolve-Path "recovery/tests/$_").Path}
  Checked "$mode-direct-build" {& $csc /nologo /unsafe /platform:x86 $opt /main:TextureBmpOs2Oracle /r:System.Drawing.dll /r:$dll /out:"$work/TextureBmpOs2Oracle.exe" $directSources}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureBmpOs2Oracle.exe.config"
  Checked "$mode-direct" {& "$work/TextureBmpOs2Oracle.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$work/direct-$mode" "$out/$mode-direct.replay"}
  $pipelineSources=@('TextureBmpOs2PipelineOracle.cs','TextureBmpOs2Fixtures.cs','TextureBmpHeaderPipelineOracle.cs','TextureBmpHeaderFixtures.cs','TextureBmpHeaderConsumerFixtures.cs','TextureBmpOracle.cs')|ForEach-Object{(Resolve-Path "recovery/tests/$_").Path}
  Checked "$mode-pipeline-build" {& $csc /nologo /unsafe /platform:x86 $opt /main:TextureBmpOs2PipelineOracle /r:System.Drawing.dll /r:System.Xml.dll /r:$dll /r:"$work/BinaryAssetBuilder.Core.dll" /r:"$work/BinaryAssetBuilder.Utility.dll" /out:"$work/TextureBmpOs2PipelineOracle.exe" $pipelineSources}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureBmpOs2PipelineOracle.exe.config"
  Checked "$mode-pipeline" {& "$work/TextureBmpOs2PipelineOracle.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" $dll "$work/pipeline-$mode" "$out/$mode-pipeline.replay"}
  $isolated=Join-Path $work "isolated-$mode";New-Item -ItemType Directory -Force $isolated|Out-Null
  Get-ChildItem "$work/*.dll"|Where-Object{$_.Name -ne 'BinaryAssetBuilder.TextureCompiler.dll'}|Copy-Item -Destination $isolated
  Copy-Item "$work/TextureBmpOs2Oracle.exe","$work/TextureBmpOs2Oracle.exe.config","$work/TextureBmpOs2PipelineOracle.exe","$work/TextureBmpOs2PipelineOracle.exe.config" $isolated
  Checked "$mode-direct-no-original" {& "$isolated/TextureBmpOs2Oracle.exe" "$out/$mode-direct.replay"}
  Checked "$mode-pipeline-no-original" {& "$isolated/TextureBmpOs2PipelineOracle.exe" "$isolated/Recovered.BinaryAssetBuilder.TextureCompiler.dll" "$isolated/fixtures" "$out/$mode-pipeline.replay"}
  $r.configurations+=@{configuration=$mode;passed=$true;withoutOriginalTextureCompiler=$true;summary=@(Get-Content "$out/$mode-direct.log","$out/$mode-pipeline.log","$out/$mode-direct-no-original.log","$out/$mode-pipeline-no-original.log"|Where-Object{$_ -match '^PASS '})}
 }
 $files=@(Get-ChildItem "$out/*.replay");Compress-Archive -Path $files.FullName -DestinationPath "$out/corpus.zip";$files|Remove-Item
 $r.corpusSha256=(Get-FileHash "$out/corpus.zip").Hash.ToLowerInvariant();$r.status='passed'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"}
