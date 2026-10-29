$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/ppm-images/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-ppm-images-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$false;sdkValidated=$false;scope='PC P3/P6 public loader and ppm plugin; raw3/DXT3/5 standard/volume/full mip, 64 successful atlas, full buffers/cdata/old tails, PGM/header/volume original errors and opaque DXT1/1A refusals; per-call NoOriginal; SDK separate';configurations=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 50;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS '}}
try{
 Copy-Item SDK/tools/*.dll $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 foreach($mode in @('Debug','Release')){
  Checked "$mode-managed-build" {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c $mode -o "$work/build-$mode" --nologo}
  Copy-Item "$work/build-$mode/*.dll" $work -Force
  Checked "$mode-ppm-native" {& "$PSScriptRoot/BuildTexturePpm.ps1" -Destination "$work/ppm-$mode" -Configuration $mode}
  Copy-Item "$work/ppm-$mode/Recovered.TexturePpm.dll" $work -Force
  Checked "$mode-public-s3" {& "$PSScriptRoot/BuildTexturePublicS3.ps1" -Destination $work -Configuration $mode}
  $r.publicS3Native=@($r.publicS3Native|Where-Object{$_})+@((Get-Content "$work/PublicS3-native.json" -Raw|ConvertFrom-Json))
  $dll="$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll";$opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  $pipelineSources=@('TexturePpmPipelineOracle.cs','TexturePpmFixtures.cs','TextureBmpHeaderPipelineOracle.cs','TextureBmpHeaderFixtures.cs','TextureBmpHeaderConsumerFixtures.cs','TextureBmpOracle.cs')|ForEach-Object{(Resolve-Path "recovery/tests/$_").Path}
  Checked "$mode-pipeline-build" {& $csc /nologo /unsafe /platform:x86 $opt /main:TexturePpmPipelineOracle /r:System.Drawing.dll /r:System.Xml.dll /r:$dll /r:"$work/BinaryAssetBuilder.Core.dll" /r:"$work/BinaryAssetBuilder.Utility.dll" /out:"$work/TexturePpmPipelineOracle.exe" $pipelineSources}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TexturePpmPipelineOracle.exe.config"
  Checked "$mode-pipeline" {& "$work/TexturePpmPipelineOracle.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" $dll "$work/pipeline-$mode" "$out/$mode-pipeline.replay"}
  $isolated=Join-Path $work "isolated-$mode";New-Item -ItemType Directory -Force $isolated|Out-Null
  Get-ChildItem "$work/*.dll"|Where-Object{$_.Name -ne 'BinaryAssetBuilder.TextureCompiler.dll'}|Copy-Item -Destination $isolated
  Copy-Item "$work/TexturePpmPipelineOracle.exe","$work/TexturePpmPipelineOracle.exe.config" $isolated
  Checked "$mode-pipeline-no-original" {& "$isolated/TexturePpmPipelineOracle.exe" "$isolated/Recovered.BinaryAssetBuilder.TextureCompiler.dll" "$isolated/fixtures" "$out/$mode-pipeline.replay"}
  $r.configurations+=@{configuration=$mode;passed=$true;withoutOriginalTextureCompiler=$true;summary=@(Get-Content "$out/$mode-pipeline.log","$out/$mode-pipeline-no-original.log"|Where-Object{$_ -match '^PASS '})}
 }
 $files=@(Get-ChildItem "$out/*.replay");Compress-Archive -Path $files.FullName -DestinationPath "$out/corpus.zip";$files|Remove-Item
 $r.corpusSha256=(Get-FileHash "$out/corpus.zip").Hash.ToLowerInvariant();$r.status='passed'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"}
