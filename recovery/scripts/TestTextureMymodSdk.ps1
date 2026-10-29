param([int]$TimeoutSeconds=1200)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$out="recovery/evidence/texture/sdk-mymod/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-mymod-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null
$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';sdkValidated=$false;profile='PcMymod';targetPlatform='Win32';scope='Complete unmodified mymod, then same complete mymod plus all repository DDS/TGA images through Texture/OnDemandTexture; original/recovered/isolated, four full outputs and every buffer/cdata; temporary SDK only, not production installation';modes=@();baselineModes=@()}
function Checked([string]$name,[scriptblock]$code){
 & $code *> "$out/$name.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 60;throw "$name failed"}
}
function Hash([string]$path){(Get-FileHash $path -Algorithm SHA256).Hash.ToLowerInvariant()}
function SameFile([string]$expected,[string]$actual){
 if(!(Test-Path $actual)){throw "Missing comparison file: $actual"}
 # Hashes are recorded for review; acceptance compares all bytes as well.
 $a=[IO.File]::ReadAllBytes($expected);$b=[IO.File]::ReadAllBytes($actual)
 if($a.Length -ne $b.Length -or ![System.Linq.Enumerable]::SequenceEqual[byte]($a,$b)){throw "Full bytes differ: $actual"}
}
function Inventory([string]$directory){
 @(Get-ChildItem $directory -File -Recurse|Sort-Object FullName|ForEach-Object{@{path=$_.FullName.Substring($directory.Length+1).Replace('\','/');bytes=$_.Length;sha256=(Hash $_.FullName)}})
}
try{
 # Formal integration consumes the exact six-DLL package, not a hand-copied
 # subset. When the PPM SDK ran first, reuse its fully tested package bytes.
 $ppm=Join-Path $root "recovery/evidence/texture/sdk-ppm/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
 $packagePath=Join-Path $work 'package'
 if(Test-Path "$ppm/validation.json"){
  $previous=Get-Content "$ppm/validation.json" -Raw|ConvertFrom-Json
  if($previous.status -ne 'passed' -or !$previous.sdkValidated -or $previous.sourceCommit -ne $r.sourceCommit){throw 'Current PPM SDK package has not passed'}
  if((Hash "$ppm/pc-plugin-package.zip") -ne $previous.packageArchiveSha256){throw 'PPM tested package archive changed'}
  Expand-Archive "$ppm/pc-plugin-package.zip" $packagePath
  Copy-Item "$ppm/pc-plugin-package.zip" "$out/pc-plugin-package.zip"
  $r.reusedPpmPackage=$true
 }else{
  Checked package-build {& "$PSScriptRoot/BuildTextureCompiler.ps1" -Destination $packagePath -Configuration Release}
  Compress-Archive -Path "$packagePath/*" -DestinationPath "$out/pc-plugin-package.zip"
 }
 $package=Get-Content "$packagePath/manifest.json" -Raw|ConvertFrom-Json
 if($package.status -ne 'built' -or $package.dirtySource -or $package.runtimeFiles.Count -ne 6 -or $package.sourceCommit -ne $r.sourceCommit -or $package.originalTextureCompilerDistributed){throw 'Wrong complete PC package'}
 foreach($file in $package.runtimeFiles){if((Hash "$packagePath/$($file.file)") -ne $file.sha256){throw 'Package fingerprint mismatch'}}
 Copy-Item "$packagePath/manifest.json" "$out/package-manifest.json"
 $r.packageArchiveSha256=Hash "$out/pc-plugin-package.zip"
 $r.publicS3Native=@($package.publicS3);$r.nativeBinaries=$package.nativeBinaries
 $r.inputMymod=Inventory (Join-Path $root 'SDK/Mods/mymod')
 $r.defaultscriptSha256=Hash 'SDK/defaultscript.cs'
 $sources=@(Get-ChildItem "$root/SDK/Art" -File -Recurse|Where-Object {$_.Extension.ToLowerInvariant() -in @('.dds','.tga')}|Sort-Object FullName)
 if($sources.Count -ne 157){throw 'Expected all150 DDS and7 TGA repository art sources; review corpus changes explicitly'}
 $r.artInputs=@($sources|ForEach-Object{@{path=$_.FullName.Substring($root.Length+1).Replace('\','/');bytes=$_.Length;sha256=(Hash $_.FullName)}})
 $r.ddsInputs=@($sources|Where-Object Extension -eq '.dds').Count
 $r.tgaInputs=@($sources|Where-Object {$_.Extension.ToLowerInvariant() -eq '.tga'}).Count
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 $wrapper=(Resolve-Path recovery/tests/TextureDxtSdkTracePlugin.cs).Path
 foreach($profile in @('Baseline','ArtOverlay')){
  foreach($mode in @('Original','Recovered','NoOriginal')){
   $label="$profile-$mode";$sdk=Join-Path $work $label
   Copy-Item SDK $sdk -Recurse
   Remove-Item "$sdk/builtmods" -Recurse -Force -ErrorAction SilentlyContinue
   $moddir=Join-Path $sdk 'Mods/mymod'
   # Baseline is byte-for-byte unmodified. Overlay preserves every original
   # include and declaration, and adds one generated include in this copy only.
   foreach($input in $r.inputMymod){if((Hash "$moddir/$($input.path)") -ne $input.sha256){throw 'mymod input differs before overlay'}}
   $expectedAssets=0
   if($profile -eq 'ArtOverlay'){
    New-Item -ItemType Directory -Force "$moddir/Art/TextureAudit"|Out-Null
    $nodes=@();$index=0
    foreach($source in $sources){
     # Normalize extension case because the original DDS passthrough checks it
     # literally; preserve all original file bytes.
     $name=('{0:d3}' -f $index)+$source.Extension.ToLowerInvariant()
     Copy-Item $source.FullName "$moddir/Art/TextureAudit/$name"
     $formats=if($source.Extension.ToLowerInvariant() -eq '.dds'){@('DXT1')}else{@('A8R8G8B8','DXT3','DXT5')}
     foreach($type in @('Texture','OnDemandTexture')){foreach($format in $formats){
      $mips=if($source.Extension.ToLowerInvariant() -eq '.dds'){@('false')}else{@('false','true')}
      foreach($mip in $mips){
       $nodes+="<$type id='TextureAudit_$($nodes.Count)' File='..\Art\TextureAudit\$name' Type='StandardTexture' OutputFormat='$format' GenerateMipMaps='$mip' AllowAutomaticResize='false' />"
      }
     }}
     $index++
    }
    $expectedAssets=$nodes.Count
    if($expectedAssets -ne 384){throw "Expected384 real-art assets; got $expectedAssets"}
    "<?xml version='1.0'?><AssetDeclaration xmlns='uri:ea.com:eala:asset'><Tags />$($nodes -join "`n")</AssetDeclaration>"|Set-Content "$moddir/Data/TextureAudit.xml" -Encoding utf8
    [xml]$modxml=Get-Content "$moddir/Data/Mod.xml" -Raw
    $includes=$modxml.DocumentElement.SelectSingleNode('*[local-name()="Includes"]')
    if(!$includes){throw 'mymod Includes missing'}
    $include=$modxml.CreateElement('Include',$modxml.DocumentElement.NamespaceURI)
    $include.SetAttribute('type','all');$include.SetAttribute('source','TextureAudit.xml');[void]$includes.AppendChild($include)
    $modxml.Save("$moddir/Data/Mod.xml")
    if($mode -eq 'Original'){
     Copy-Item "$moddir/Data/Mod.xml" "$out/overlay-Mod.xml"
     Copy-Item "$moddir/Data/TextureAudit.xml" "$out/overlay-TextureAudit.xml"
    }
   }
   $defines=@();$implementation="$sdk/tools/BinaryAssetBuilder.TextureCompiler.dll"
   if($mode -eq 'Original'){$defines=@('/define:ORIGINAL')}else{
    Checked "$label-package-install" {& "$packagePath/InstallTextureCompiler.ps1" -PackageDirectory $packagePath -SdkDirectory $sdk -ReceiptPath "$out/$label-installation.json"}
    if($mode -eq 'Recovered'){
     # Exercise rollback after relocating the SDK, from a different directory.
     $portable=Join-Path $sdk 'RecoveryTextureInstallation';New-Item -ItemType Directory -Force $portable|Out-Null
     $installation=Get-Content "$out/$label-installation.json" -Raw|ConvertFrom-Json
     $installation.backupDirectory=[IO.Path]::GetRelativePath($portable,$installation.backupDirectory)
     $installation.sdkDirectory='..';$installation|ConvertTo-Json -Depth 8|Set-Content "$portable/installation.json"
     $moved=$sdk+'-relocated';Move-Item $sdk $moved;$sdk=$moved;$moddir=Join-Path $sdk 'Mods/mymod'
     Checked "$label-package-restore" {& "$packagePath/RestoreTextureCompiler.ps1" -ReceiptPath "$sdk/RecoveryTextureInstallation/installation.json"}
     foreach($file in $installation.files){
      $path=Join-Path "$sdk/tools" $file.file
      if($file.existed){if((Hash $path) -ne $file.beforeSha256){throw 'Relocated SDK rollback changed prior bytes'}}
      elseif(Test-Path $path){throw 'Relocated SDK rollback retained a runtime DLL'}
     }
     Checked "$label-package-reinstall" {& "$packagePath/InstallTextureCompiler.ps1" -PackageDirectory $packagePath -SdkDirectory $sdk -ReceiptPath "$out/$label-installation.json"}
     $r.packageRestoreValidated=$true;$r.portableRestoreValidated=$true
    }
    $implementation="$sdk/tools/Recovered.BinaryAssetBuilder.TextureCompiler.dll"
    if($mode -eq 'NoOriginal'){$defines=@('/define:NO_ORIGINAL');Remove-Item "$sdk/tools/BinaryAssetBuilder.TextureCompiler.dll" -Force}
   }
   $trace=Join-Path $work "$label-trace";New-Item -ItemType Directory -Force $trace|Out-Null
   $cfg="$sdk/tools/BinaryAssetBuilder.exe.config";[xml]$xml=Get-Content $cfg -Raw
   $plugin=@($xml.configuration.assetbuilder.plugins.plugin|Where-Object {$_.assetTypes.Split(',') -contains 'Texture'})
   if($plugin.Count -ne 1){throw 'Texture plugin route is ambiguous'}
   if($mode -ne 'Original' -and $plugin[0].targetType -ne $package.pluginType){throw 'Actual installed Texture route differs'}
   Checked "$label-wrapper-build" {& $csc /nologo /target:library /platform:x86 $defines /r:"$sdk/tools/BinaryAssetBuilder.Core.dll" /r:"$sdk/tools/BinaryAssetBuilder.Utility.dll" /r:$implementation /out:"$sdk/tools/TextureSdkTrace.dll" $wrapper}
   $plugin[0].SetAttribute('targetType','TextureSdkTracePlugin, TextureSdkTrace');$plugin[0].SetAttribute('useBuildCache','false')
   $startup=$xml.configuration.startup;if($null -eq $startup){$startup=$xml.CreateElement('startup');[void]$xml.configuration.AppendChild($startup)};$startup.SetAttribute('useLegacyV2RuntimeActivationPolicy','true')
   $runtime=$startup.supportedRuntime;if($null -eq $runtime){$runtime=$xml.CreateElement('supportedRuntime');[void]$startup.AppendChild($runtime)};$runtime.SetAttribute('version','v4.0');$xml.Save($cfg)
   Copy-Item $cfg "$out/$label.config"
   $env:RA3_TEXTURE_SDK_TRACE=$trace
   $output=Join-Path $sdk 'builtmods';New-Item -ItemType Directory -Force $output|Out-Null
   $mod=Join-Path $moddir 'Data/Mod.xml'
   $arguments=@("`"$mod`"","/od:`"$output`"","/iod:`"$output`"",'/tp:Win32','/csc:false','/ls:true','/osh:false','/pc:true','/res:true','/slowclean:true','/ss:true','/art:".;.\Mods\mymod\Art;.\Mods;.\Art"','/audio:".;.\Mods\mymod\Audio;.\Mods;.\Audio"','/data:".;.\Mods\mymod\Data;.\Mods;.\SageXml"')
   $p=Start-Process "$sdk/tools/BinaryAssetBuilder.exe" -ArgumentList ($arguments -join ' ') -WorkingDirectory $sdk -PassThru -RedirectStandardOutput "$out/$label-stdout.log" -RedirectStandardError "$out/$label-stderr.log"
   if(!$p.WaitForExit($TimeoutSeconds*1000)){Stop-Process -Id $p.Id -Force;throw "$label SDK timeout"};$p.Refresh()
   if($p.ExitCode -ne 0){Get-Content "$out/$label-stdout.log" -Tail 70;Get-Content "$out/$label-stderr.log" -Tail 30;throw "$label SDK exit=$($p.ExitCode)"}
   if($mode -eq 'NoOriginal' -and (Test-Path "$sdk/tools/BinaryAssetBuilder.TextureCompiler.dll")){throw 'Original TextureCompiler present in isolated SDK'}
   $files=@()
   foreach($ext in @('bin','manifest','imp','relo')){
    $file="$output/mods/mymod/data/mod.$ext"
    if(!(Test-Path $file)){throw "$label missing mod.$ext"}
    $files+=@{file="mod.$ext";bytes=(Get-Item $file).Length;sha256=(Hash $file)}
    if($mode -ne 'Original'){SameFile "$work/$profile-Original/builtmods/mods/mymod/data/mod.$ext" $file}
   }
   $calls=@();if(Test-Path "$trace/texture.calls.tsv"){$calls=@(Import-Csv "$trace/texture.calls.tsv" -Delimiter "`t")}
   if(@($calls|Where-Object Stage -eq 'failure').Count -ne 0){throw "$label plugin failure trace"}
   $success=@($calls|Where-Object Stage -eq 'success');$unique=@($success|ForEach-Object {"$($_.TypeId):$($_.InstanceId)"}|Sort-Object -Unique)
   if($unique.Count -ne $expectedAssets -or $success.Count -ne $expectedAssets){throw "$label expected $expectedAssets unique assets and actual calls; got $($unique.Count)/$($success.Count)"}
   # Compare both file sets, TSV and complete instance/relo/imp/cdata. Hash
   # manifests remain after equality; large repeated DDS traces aren't committed.
   $traceFiles=Inventory $trace
   if($mode -ne 'Original'){
    $original=Join-Path $work "$profile-Original-trace"
    $expectedFiles=@(Get-ChildItem $original -File|ForEach-Object Name|Sort-Object)
    $actualFiles=@(Get-ChildItem $trace -File|ForEach-Object Name|Sort-Object)
    if(($expectedFiles -join "`n") -cne ($actualFiles -join "`n")){throw "$label trace file set differs"}
    foreach($name in $expectedFiles){SameFile "$original/$name" "$trace/$name"}
   }
   $traceFiles|ConvertTo-Json -Depth 6|Set-Content "$out/$label-trace-files.json"
   if(Test-Path "$trace/texture.calls.tsv"){Copy-Item "$trace/texture.calls.tsv" "$out/$label-calls.tsv"}
   $result=@{mode=$mode;passed=$true;uniqueAssets=$unique.Count;actualCalls=$success.Count;withoutOriginalTextureCompiler=($mode -eq 'NoOriginal');assertedOriginalNotLoadedDuringEveryCall=($mode -eq 'NoOriginal');files=$files;traceFiles=$traceFiles.Count;fullByteComparison=($mode -ne 'Original')}
   if($profile -eq 'Baseline'){$r.baselineModes+=$result}else{$r.modes+=$result}
   Write-Output "PASS $label complete mymod: assets=$($unique.Count) calls=$($success.Count)"
  }
 }
 $r.expectedAssets=384;$r.status='passed';$r.sdkValidated=$true
 $r.productionSdkUnmodified=$true;$r.baselineInputUnmodified=$true
 Write-Output 'PASS Texture complete mymod and384-asset real-art overlay: three modes, full byte equality, no original fallback'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{
 $env:RA3_TEXTURE_SDK_TRACE=$null
 # Full originals are retained as workflow artifacts, once per profile; the
 # committed receipt/manifests contain every compared path, length and hash.
 foreach($profile in @('Baseline','ArtOverlay')){
  $trace=Join-Path $work "$profile-Original-trace"
  if((Test-Path $trace) -and @(Get-ChildItem $trace -File).Count -gt 0){
   Compress-Archive -Path "$trace/*" -DestinationPath "$out/$profile-original-traces.zip"
  }
  $outputs=Join-Path $work "$profile-Original/builtmods/mods/mymod/data"
  if(Test-Path "$outputs/mod.bin"){
   Compress-Archive -Path "$outputs/mod.bin","$outputs/mod.manifest","$outputs/mod.imp","$outputs/mod.relo" -DestinationPath "$out/$profile-original-outputs.zip"
  }
 }
 $r|ConvertTo-Json -Depth 14|Set-Content "$out/validation.json"
}
