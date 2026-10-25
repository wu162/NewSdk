param([int]$TimeoutSeconds=300)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$out="recovery/evidence/texture/sdk/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-sdk-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work | Out-Null
$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';sdkValidated=$false;scope='EALAModStudio case5 BAB build of dedicated 18-asset real TGA raw/no-mip fixture MOD';modes=@();inputs=(Get-Content recovery/tests/fixtures/texture-main/manifest.json -Raw|ConvertFrom-Json)}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 60;throw "$name failed"}}
try{
 Checked recovered-build {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c Release -o "$work/build" --nologo}
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 $wrapper=(Resolve-Path recovery/tests/TextureSdkTracePlugin.cs).Path
 foreach($mode in @('Original','Recovered','NoOriginal')){
  $sdk="$work/$mode";Copy-Item SDK $sdk -Recurse
  Remove-Item "$sdk/builtmods" -Recurse -Force -ErrorAction SilentlyContinue
  $moddir="$sdk/Mods/RecoveryTexture";New-Item -ItemType Directory -Force "$moddir/Data","$moddir/Art"|Out-Null
  $nodes=@();$index=0
  foreach($f in @($r.inputs.files|Where-Object {$_.fixture.EndsWith('.tga')})){
   if((Get-FileHash $f.fixture -Algorithm SHA256).Hash.ToLowerInvariant() -ne $f.sha256){throw 'Real TGA input hash differs'}
   $name=[IO.Path]::GetFileName($f.fixture);Copy-Item $f.fixture "$moddir/Art/$name"
   foreach($type in @('Texture','OnDemandTexture')){foreach($format in @('A8R8G8B8','X8R8G8B8','R5G6B5')){
    $nodes+="<$type id='RecoveredTexture_$index' File='ART:$name' Type='StandardTexture' OutputFormat='$format' GenerateMipMaps='false' AllowAutomaticResize='false' />";$index++
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
  $env:RA3_TEXTURE_SDK_TRACE=$trace
  $output="$sdk/builtmods";New-Item -ItemType Directory -Force $output|Out-Null
  $mod="$moddir/Data/Mod.xml"
  $arguments=@("`"$mod`"","/od:`"$output`"","/iod:`"$output`"",'/csc:false','/ls:true','/osh:false','/pc:true','/res:true','/slowclean:true','/ss:true','/art:".;.\Mods\RecoveryTexture\Art;.\Mods;.\Art"','/audio:".;.\Mods\RecoveryTexture\Audio;.\Mods;.\Audio"','/data:".;.\Mods\RecoveryTexture\Data;.\Mods;.\SageXml"')
  $p=Start-Process "$sdk/tools/BinaryAssetBuilder.exe" -ArgumentList ($arguments -join ' ') -WorkingDirectory $sdk -PassThru -RedirectStandardOutput "$out/$mode-stdout.log" -RedirectStandardError "$out/$mode-stderr.log"
  if(!$p.WaitForExit($TimeoutSeconds*1000)){Stop-Process -Id $p.Id -Force;throw "$mode SDK timeout"};$p.Refresh()
  if($p.ExitCode -ne 0){Get-Content "$out/$mode-stdout.log" -Tail 70;Get-Content "$out/$mode-stderr.log" -Tail 30;throw "$mode SDK exit=$($p.ExitCode)"}
  $files=@()
  foreach($ext in @('bin','manifest','imp','relo')){
   $f="$output/mods/recoverytexture/data/mod.$ext";if(!(Test-Path $f)){throw "$mode missing $f"}
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
