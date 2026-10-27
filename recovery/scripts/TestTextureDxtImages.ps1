$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/dxt-images/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-colour-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$false;sdkValidated=$false;scope='actual Texture/OnDemandTexture DXT3/5 ProcessInstance; PC POT/partial-block and supported byte/5-bit/16-bit storage NPOT standard/volume raw+DXT no-mip/complete float mip TGA/PNG/BMP; three buffers/full cdata and NoOriginal; SDK pending';configurations=@();nativeBinaries=@()}
$r.sourceFiles=@(Get-ChildItem src/TextureCompiler/Colour -File|Sort-Object FullName|ForEach-Object{@{path=[IO.Path]::GetRelativePath($root,$_.FullName).Replace('\','/');sha256=(Get-FileHash $_.FullName).Hash.ToLowerInvariant()}})
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 40;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS '}}
try {
 $npotEvidence='recovery/evidence/texture/npot-load/40528237009-1'
 $npotReceipt=Get-Content "$npotEvidence/validation.json" -Raw|ConvertFrom-Json
 if($npotReceipt.status -ne 'probe-only' -or $npotReceipt.sourceCommit -ne 'a19bb879704cd80ad55202760a160c8af57bbe2d' -or (Get-FileHash "$npotEvidence/samples.zip").Hash.ToLowerInvariant() -ne $npotReceipt.samplesSha256){throw 'NPOT source corpus mismatch'}
 Expand-Archive "$npotEvidence/samples.zip" "$work/npot-source"
 New-Item -ItemType Directory -Force "$work/npot-fixtures"|Out-Null
 foreach($row in Import-Csv "$work/npot-source/transcript.tsv" -Delimiter "`t"){
  if(("$($row.sourceWidth)x$($row.sourceHeight)" -in @('3x1','3x2','5x3','7x5')) -and ($row.pattern -in @('0','2'))){
   $ext=if($row.profile.StartsWith('tga-')){'tga'}elseif($row.profile.StartsWith('png-')){'png'}else{'bmp'}
   Copy-Item "$work/npot-source/$($row.label).$ext" "$work/npot-fixtures"
  }
 }
 if(@(Get-ChildItem "$work/npot-fixtures" -File).Count -ne 80){throw 'Expected 80 focused NPOT inputs'}
 & "$PSScriptRoot/SelectTextureNpotStorageFixtures.ps1" -Destination "$work/npot-storage-fixtures" -Extraction "$work/npot-storage-source"
 Copy-Item SDK/tools/*.dll $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 foreach($mode in @('Debug','Release')) {
  Checked "$mode-asm" {& ml /nologo /c /coff /safeseh /Fo"$work/colour-$mode.obj" (Resolve-Path src/TextureCompiler/Colour/TextureColour.asm).Path}
  $nativeOpt=if($mode -eq 'Release'){'/O2'}else{'/Od'}
  Checked "$mode-native" {& cl /nologo /LD /MT /fp:strict /GS- $nativeOpt /Fo"$work/bridge-$mode.obj" (Resolve-Path src/TextureCompiler/Colour/TextureColourBridge.cpp).Path "$work/colour-$mode.obj" /link /NODEFAULTLIB /NOENTRY /OUT:"$work/Recovered.TextureDxtColour.dll" /IMPLIB:"$work/colour-$mode.lib"}
  $bytes=[IO.File]::ReadAllBytes("$work/Recovered.TextureDxtColour.dll");$pe=[BitConverter]::ToInt32($bytes,0x3c)
  if([BitConverter]::ToUInt16($bytes,$pe+4) -ne 0x14c -or [BitConverter]::ToUInt32($bytes,$pe+40) -ne 0 -or [BitConverter]::ToUInt32($bytes,$pe+128) -ne 0){throw 'Owned colour DLL must be x86 with no entry point or imports'}
  $r.nativeBinaries+=@{configuration=$mode;sha256=(Get-FileHash "$work/Recovered.TextureDxtColour.dll").Hash.ToLowerInvariant();machine='I386';entryPoint=0;importDirectory=0}
  Checked "$mode-managed-build" {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c $mode -o "$work/build-$mode" --nologo}
  Copy-Item "$work/build-$mode/*.dll" $work -Force
  Checked "$mode-alpha-native" {& cl /nologo /LD /MT /fp:strict /GS- $nativeOpt /Fo"$work/alpha-$mode.obj" (Resolve-Path src/TextureCompiler/Dxt/TextureAlphaQuantizer.cpp).Path /link /NODEFAULTLIB /NOENTRY /OUT:"$work/Recovered.TextureDxtAlpha.dll" /IMPLIB:"$work/alpha-$mode.lib"}
  $dll="$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll"
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  Checked "$mode-oracle-build" {& $csc /nologo /unsafe /platform:x86 $opt /main:TextureDxtPipelineOracle /r:System.Drawing.dll /r:System.Xml.dll /r:$dll /r:"$work/BinaryAssetBuilder.Core.dll" /r:"$work/BinaryAssetBuilder.Utility.dll" /out:"$work/TextureDxtPipelineOracle.exe" (Resolve-Path recovery/tests/TextureDxtPipelineOracle.cs).Path (Resolve-Path recovery/tests/TexturePngOracle.cs).Path (Resolve-Path recovery/tests/TextureBmpOracle.cs).Path}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureDxtPipelineOracle.exe.config"
  Checked "$mode-oracle" {& "$work/TextureDxtPipelineOracle.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" $dll "$work/fixtures-$mode" "$out/$mode-corpus.replay"}
  $isolated=Join-Path $work "isolated-$mode";New-Item -ItemType Directory -Force $isolated|Out-Null
  Get-ChildItem "$work/*.dll"|Where-Object{$_.Name -ne 'BinaryAssetBuilder.TextureCompiler.dll'}|Copy-Item -Destination $isolated
  Copy-Item "$work/TextureDxtPipelineOracle.exe","$work/TextureDxtPipelineOracle.exe.config" $isolated
  Checked "$mode-no-original" {& "$isolated/TextureDxtPipelineOracle.exe" "$isolated/Recovered.BinaryAssetBuilder.TextureCompiler.dll" "$isolated/fixtures" "$out/$mode-corpus.replay"}
  $r.configurations+=@{configuration=$mode;passed=$true;withoutOriginalTextureCompiler=$true;summary=@(Get-Content "$out/$mode-oracle.log","$out/$mode-no-original.log"|Where-Object{$_ -match '^PASS '})}
 }
 $files=@(Get-ChildItem "$out/*-corpus.replay");Compress-Archive -Path $files.FullName -DestinationPath "$out/corpus.zip";$files|Remove-Item
 $r.status='passed'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"}

