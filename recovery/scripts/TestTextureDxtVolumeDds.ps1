$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/dxt-volume-dds/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-colour-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$false;sdkValidated=$false;scope='PC quality71 DXT3/DXT5 POT/partial-block volume no-mip/recursive float mip DDS; no plugin/SDK acceptance';configurations=@();nativeBinaries=@()}
$r.sourceFiles=@(Get-ChildItem src/TextureCompiler/Colour -File|Sort-Object FullName|ForEach-Object{@{path=[IO.Path]::GetRelativePath($root,$_.FullName).Replace('\','/');sha256=(Get-FileHash $_.FullName).Hash.ToLowerInvariant()}})
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 40;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS '}}
try {
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
  Checked "$mode-oracle-build" {& $csc /nologo /unsafe /platform:x86 $opt /r:$dll /out:"$work/TextureDxtVolumeDdsDifferential.exe" (Resolve-Path recovery/tests/TextureDxtVolumeDdsDifferential.cs).Path}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureDxtVolumeDdsDifferential.exe.config"
  Checked "$mode-oracle" {& "$work/TextureDxtVolumeDdsDifferential.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$out/$mode-transcript.txt" $dll}
  $isolated=Join-Path $work "isolated-$mode";New-Item -ItemType Directory -Force $isolated|Out-Null
  Get-ChildItem "$work/*.dll"|Where-Object{$_.Name -ne 'BinaryAssetBuilder.TextureCompiler.dll'}|Copy-Item -Destination $isolated
  Checked "$mode-no-original-build" {& $csc /nologo /unsafe /platform:x86 $opt /r:$dll /out:"$isolated/TextureDxtVolumeDdsNoOriginal.exe" (Resolve-Path recovery/tests/TextureDxtVolumeDdsNoOriginal.cs).Path}
  Checked "$mode-no-original" {& "$isolated/TextureDxtVolumeDdsNoOriginal.exe" "$out/$mode-transcript.txt.replay"}
  Remove-Item "$out/$mode-transcript.txt.replay"
  $r.configurations+=@{configuration=$mode;passed=$true;withoutOriginalTextureCompiler=$true;summary=@(Get-Content "$out/$mode-oracle.log","$out/$mode-no-original.log"|Where-Object{$_ -match '^PASS '})}
 }
 $files=@(Get-ChildItem "$out/*-transcript.txt");Compress-Archive -Path $files.FullName -DestinationPath "$out/transcripts.zip";$files|Remove-Item
 $r.status='passed'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"}

