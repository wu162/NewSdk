param([ValidateSet('VolumeMipPc','VolumeMipPs3')][string]$Profile='VolumeMipPc',[int]$TimeoutSeconds=300)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$out="recovery/evidence/texture/sdk-volume-mips/$Profile/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-sdk-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work | Out-Null
$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';sdkValidated=$false;scope='EALAModStudio case5 BAB build of dedicated 18-asset raw volume mip fixture MOD';profile=$Profile;targetPlatform=$(if($Profile -eq 'VolumeMipPc'){'Win32'}else{'PlayStation3'});modes=@();inputs=(Get-Content recovery/tests/fixtures/texture-main/manifest.json -Raw|ConvertFrom-Json)}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 60;throw "$name failed"}}
try{
 Checked recovered-build {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c Release -o "$work/build" --nologo}
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 $wrapper=(Resolve-Path recovery/tests/TextureSdkTracePlugin.cs).Path
 foreach($mode in @('Original','Recovered','NoOriginal')){
  $sdk=Join-Path $work $mode;Copy-Item SDK $sdk -Recurse
  Remove-Item "$sdk/builtmods" -Recurse -Force -ErrorAction SilentlyContinue
  $moddir=Join-Path $sdk 'Mods\RecoveryTexture';New-Item -ItemType Directory -Force "$moddir/Data","$moddir/Art"|Out-Null
  $nodes=@();$index=0
   foreach($depth in @(1,4,16)){
    $width=16*$depth;$height=16;$name="volume-$depth.tga";$pixels=$width*$height
    $tga=New-Object byte[] (18+$pixels*4);$tga[2]=2;$tga[12]=[byte]($width-band 255);$tga[13]=[byte]($width-shr 8);$tga[14]=16;$tga[16]=32;$tga[17]=40
    for($i=0;$i -lt $pixels;$i++){$at=18+$i*4;$tga[$at]=[byte](($i*31)-band 255);$tga[$at+1]=[byte](($i*43+11)-band 255);$tga[$at+2]=[byte](($i*7+171)-band 255);$tga[$at+3]=[byte](($i*17)-band 255)}
    [IO.File]::WriteAllBytes("$moddir/Art/$name",$tga)
    foreach($type in @('Texture','OnDemandTexture')){foreach($format in @('A8R8G8B8','X8R8G8B8','R5G6B5')){
     $nodes+="<$type id='RecoveredTexture_$index' File='..\Art\$name' Type='VolumeTexture' OutputFormat='$format' GenerateMipMaps='true' AllowAutomaticResize='false' />";$index++
    }}
   }

  "<?xml version='1.0'?><AssetDeclaration xmlns='uri:ea.com:eala:asset'><Tags />$($nodes -join "`n")</AssetDeclaration>"|Set-Content "$moddir/Data/Mod.xml" -Encoding utf8
  $trace="$out/$mode-trace";New-Item -ItemType Directory -Force $trace|Out-Null
  $cfg="$sdk/tools/BinaryAssetBuilder.exe.config";[xml]$xml=Get-Content $cfg -Raw
  $plugin=@($xml.configuration.assetbuilder.plugins.plugin|Where-Object {$_.assetTypes.Split(',') -contains 'Texture'})
  if($plugin.Count -ne 1){throw 'Texture plugin route is ambiguous'}
  if($mode -eq 'Original'){
   Checked "$mode-wrapper-build" {& $csc /nologo /target:library /platform:x86 /define:ORIGINAL /r:"$sdk/tools/BinaryAssetBuilder.Core.dll" /r:"$sdk/tools/BinaryAssetBuilder.Utility.dll" /r:"$sdk/tools/BinaryAssetBuilder.TextureCompiler.dll" /out:"$sdk/tools/TextureSdkTrace.dll" $wrapper}
  }else{
   Copy-Item "$work/build/Recovered.BinaryAssetBuilder.TextureCompiler.dll" "$sdk/tools"
   Checked "$mode-wrapper-build" {& $csc /nologo /target:library /platform:x86 /r:"$sdk/tools/BinaryAssetBuilder.Core.dll" /r:"$sdk/tools/BinaryAssetBuilder.Utility.dll" /r:"$sdk/tools/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /out:"$sdk/tools/TextureSdkTrace.dll" $wrapper}
   if($mode -eq 'NoOriginal'){Remove-Item "$sdk/tools/BinaryAssetBuilder.TextureCompiler.dll" -Force}
  }
  $plugin[0].SetAttribute('targetType','TextureSdkTracePlugin, TextureSdkTrace');$plugin[0].SetAttribute('useBuildCache','false')
  $startup=$xml.CreateElement('startup');$startup.SetAttribute('useLegacyV2RuntimeActivationPolicy','true')
  $runtime=$xml.CreateElement('supportedRuntime');$runtime.SetAttribute('version','v4.0');[void]$startup.AppendChild($runtime);[void]$xml.configuration.AppendChild($startup);$xml.Save($cfg)
  Copy-Item $cfg "$out/$mode.config"
  if($mode -eq 'Original'){Copy-Item "$moddir/Data/Mod.xml" "$out/input-Mod.xml";$r.fixtureFiles=@(Get-ChildItem "$moddir/Art" -File|Sort-Object Name|ForEach-Object {@{name=$_.Name;bytes=$_.Length;sha256=(Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()}})}
  $env:RA3_TEXTURE_SDK_TRACE=$trace
  $output=Join-Path $sdk 'builtmods';New-Item -ItemType Directory -Force $output|Out-Null
  $mod=Join-Path $moddir 'Data\Mod.xml'
  $arguments=@("`"$mod`"","/od:`"$output`"","/iod:`"$output`"","/tp:$($r.targetPlatform)",'/csc:false','/ls:true','/osh:false','/pc:true','/res:true','/slowclean:true','/ss:true','/art:".;.\Mods\RecoveryTexture\Art;.\Mods;.\Art"','/audio:".;.\Mods\RecoveryTexture\Audio;.\Mods;.\Audio"','/data:".;.\Mods\RecoveryTexture\Data;.\Mods;.\SageXml"')
  $p=Start-Process "$sdk/tools/BinaryAssetBuilder.exe" -ArgumentList ($arguments -join ' ') -WorkingDirectory $sdk -PassThru -RedirectStandardOutput "$out/$mode-stdout.log" -RedirectStandardError "$out/$mode-stderr.log"
  if(!$p.WaitForExit($TimeoutSeconds*1000)){Stop-Process -Id $p.Id -Force;throw "$mode SDK timeout"};$p.Refresh()
  if($p.ExitCode -ne 0){Get-Content "$out/$mode-stdout.log" -Tail 70;Get-Content "$out/$mode-stderr.log" -Tail 30;throw "$mode SDK exit=$($p.ExitCode)"}
  $files=@()
  foreach($ext in @('bin','manifest','imp','relo')){
   $f="$output/mods/recoverytexture/data/mod.$ext";if(!(Test-Path $f)){Get-ChildItem $output -File -Recurse | Select-Object -First 25 FullName | Out-String | Write-Output;throw "$mode missing $f"}
   Copy-Item $f "$out/$mode-mod.$ext";$files+=@{file="mod.$ext";bytes=(Get-Item $f).Length;sha256=(Get-FileHash $f -Algorithm SHA256).Hash.ToLowerInvariant()}
  }
  $calls=Import-Csv "$trace/texture.calls.tsv" -Delimiter "`t"
  if(@($calls|Where-Object Stage -eq 'failure').Count -ne 0){throw "$mode plugin failure trace"}
  $success=@($calls|Where-Object Stage -eq 'success');$unique=@($success|ForEach-Object {"$($_.TypeId):$($_.InstanceId)"}|Sort-Object -Unique)
  if($unique.Count -ne 18 -or $success.Count -lt 18){throw "$mode expected 18 unique actual Core texture calls; found $($unique.Count)"}
  $r.modes+=@{mode=$mode;passed=$true;uniqueAssets=$unique.Count;actualCalls=$success.Count;withoutOriginalTextureCompiler=($mode -eq 'NoOriginal');files=$files}
  Write-Output "PASS $mode real SDK: assets=$($unique.Count) calls=$($success.Count)"
 }
 foreach($mode in @('Recovered','NoOriginal')){
  foreach($ext in @('bin','manifest','imp','relo')){if((Get-FileHash "$out/Original-mod.$ext").Hash -ne (Get-FileHash "$out/$mode-mod.$ext").Hash){throw "$mode SDK mod.$ext differs"}}
  foreach($file in Get-ChildItem "$out/Original-trace" -File){
   $other="$out/$mode-trace/$($file.Name)";if(!(Test-Path $other)){throw "$mode missing trace $($file.Name)"}
   if((Get-FileHash $file.FullName).Hash -ne (Get-FileHash $other).Hash){throw "$mode actual plugin trace differs: $($file.Name)"}
  }
 }
 $r.status='passed';$r.sdkValidated=$true
 Write-Output 'PASS Texture SDK original/recovered/no-original: four stream files, actual Core calls, every buffer and full cdata identical'
}finally{
 $env:RA3_TEXTURE_SDK_TRACE=$null
 $r|ConvertTo-Json -Depth 14|Set-Content "$out/validation.json"
}

