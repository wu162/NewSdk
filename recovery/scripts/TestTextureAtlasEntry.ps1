$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/atlas-entry/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-atlas-entry-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$false;sdkValidated=$false;scope='PC TerrainTextureAtlas full plugin entry: immutable 25-case original corpus plus 23 patterned/file/strtoul cases; full runtime/DDS/relo/imports/error/unchanged input and cdata, per-call isolated replay; SDK separate';configurations=@();nativeBinaries=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 40;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS '}}
try{
 $baseline='recovery/evidence/texture/atlas-load/40599993769-1'
 $receipt=Get-Content "$baseline/validation.json" -Raw|ConvertFrom-Json
 if($receipt.status -ne 'probe-only' -or $receipt.sourceCommit -ne '056937b13fa39431ec9171b0d990e6a5c15a013c' -or $receipt.fixtureCount -ne 25 -or $receipt.originalCalls -ne 50 -or $receipt.originalSha256 -ne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e' -or (Get-FileHash "$baseline/samples.zip").Hash.ToLowerInvariant() -ne $receipt.samplesSha256){throw 'Wrong immutable original PC atlas entry corpus'}
 git merge-base --is-ancestor $receipt.sourceCommit HEAD;if($LASTEXITCODE -ne 0){throw 'Atlas entry baseline is not an ancestor'}
 $r.originalBaseline=@{sourceCommit=$receipt.sourceCommit;runId=$receipt.runId;samplesSha256=$receipt.samplesSha256}
 Expand-Archive "$baseline/samples.zip" "$work/samples"
 $original=Join-Path $work 'original';New-Item -ItemType Directory -Force $original|Out-Null
 Copy-Item SDK/tools/*.dll $original
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 Checked oracle-build {& $csc /nologo /platform:x86 /optimize+ /r:System.Xml.dll /r:"$original/BinaryAssetBuilder.Core.dll" /r:"$original/BinaryAssetBuilder.Utility.dll" /out:"$original/TextureAtlasEntryOracle.exe" (Resolve-Path recovery/tests/TextureAtlasEntryOracle.cs).Path}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$original/TextureAtlasEntryOracle.exe.config"
 Checked oracle {& "$original/TextureAtlasEntryOracle.exe" "$original/BinaryAssetBuilder.TextureCompiler.dll" "$work/additional"}
 Copy-Item "$work/additional/transcript.tsv" "$out/additional-transcript.tsv"
 Compress-Archive -Path "$work/additional/*" -DestinationPath "$out/additional-samples.zip"
 $r.additionalSamplesSha256=(Get-FileHash "$out/additional-samples.zip").Hash.ToLowerInvariant()
 Get-ChildItem SDK/tools/*.dll|Where-Object Name -ne 'BinaryAssetBuilder.TextureCompiler.dll'|Copy-Item -Destination $work
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
  $alphaBytes=[IO.File]::ReadAllBytes("$work/Recovered.TextureDxtAlpha.dll");$alphaPe=[BitConverter]::ToInt32($alphaBytes,0x3c)
  if([BitConverter]::ToUInt16($alphaBytes,$alphaPe+4) -ne 0x14c -or [BitConverter]::ToUInt32($alphaBytes,$alphaPe+40) -ne 0 -or [BitConverter]::ToUInt32($alphaBytes,$alphaPe+128) -ne 0){throw 'Owned alpha DLL must be x86 with no entry point or imports'}
  $r.nativeBinaries+=@{configuration=$mode;name='Recovered.TextureDxtAlpha.dll';sha256=(Get-FileHash "$work/Recovered.TextureDxtAlpha.dll").Hash.ToLowerInvariant();machine='I386';entryPoint=0;importDirectory=0}
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  Checked "$mode-replay-build" {& $csc /nologo /platform:x86 $opt /r:System.Xml.dll /r:"$work/BinaryAssetBuilder.Core.dll" /r:"$work/BinaryAssetBuilder.Utility.dll" /r:"$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /out:"$work/TextureAtlasEntryReplay.exe" (Resolve-Path recovery/tests/TextureAtlasEntryReplay.cs).Path}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureAtlasEntryReplay.exe.config"
  Checked "$mode-replay" {& "$work/TextureAtlasEntryReplay.exe" "$work/samples" "$work/additional"}
  $r.configurations+=@{configuration=$mode;passed=$true;withoutOriginalTextureCompiler=$true;summary=@(Get-Content "$out/$mode-replay.log"|Where-Object{$_ -match '^PASS '})}
 }
 $r.status='passed'
}catch{if(Test-Path "$work/additional/current.txt"){Copy-Item "$work/additional/current.txt" "$out/current.txt"};$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"}
