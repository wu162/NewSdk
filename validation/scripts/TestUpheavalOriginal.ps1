param([int]$TimeoutSeconds=2700)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$report=Join-Path $root 'validation/upheaval-original-sdk'
New-Item -ItemType Directory -Force $report|Out-Null
$sdk=Join-Path $env:RUNNER_TEMP 'upheaval-original-sdk'
Copy-Item "$root/SDK" $sdk -Recurse
$mod=Join-Path $sdk 'Mods/Upheaval'
$built=Join-Path $sdk 'builtmods'
# Retain shipped base reference manifests, never reuse generated MOD files.
Get-ChildItem $built -Directory|Where-Object Name -ne 'sagexml'|Remove-Item -Recurse -Force
Get-ChildItem $built -File|Where-Object Extension -ne '.manifest'|Remove-Item -Force
$out=Join-Path $built 'mods/Upheaval'
$data=Join-Path $out 'data'
New-Item -ItemType Directory -Force $data|Out-Null
$result=[ordered]@{
 succeeded=$false;sourceCommit=$env:GITHUB_SHA;runId=$env:GITHUB_RUN_ID
 baseCommit='bacfc69fc3e71c8116d1f22b933b8b6173170ecf'
 mode='Original SDK binaries; full unmodified Upheaval input'
 commandSource='SDK/defaultscript.cs cases 3-9'
 compatibility='Temporary CLR4 startup with legacy mixed-mode activation; original EXE/DLL unchanged'
 stages=@();outputs=@();status='started'
}
$hashes=@{}
Get-ChildItem "$root/SDK/tools" -File|Where-Object Extension -in '.dll','.exe'|ForEach-Object{$hashes[$_.Name]=(Get-FileHash $_.FullName).Hash}
function Q([string]$Value){return '"'+$Value+'"'}
function Invoke-Tool([string]$Label,[string]$Name,[string[]]$ToolArgs){
 $prefix=Join-Path $report ('{0:D3}-{1}' -f $result.stages.Count,$Label)
 Write-Host "START $Label $([DateTime]::UtcNow.ToString('o'))"
 $start=[DateTime]::UtcNow
 $p=Start-Process "$sdk/tools/$Name" -ArgumentList ($ToolArgs -join ' ') -WorkingDirectory $sdk -PassThru -NoNewWindow -RedirectStandardOutput "$prefix.stdout.txt" -RedirectStandardError "$prefix.stderr.txt"
 $done=$p.WaitForExit($TimeoutSeconds*1000)
 if(!$done){Stop-Process -Id $p.Id -Force}
 $p.Refresh()
 $result.stages += [ordered]@{label=$Label;tool=$Name;arguments=$ToolArgs;completed=$done;exitCode=$p.ExitCode;elapsedSeconds=([DateTime]::UtcNow-$start).TotalSeconds}
 $result|ConvertTo-Json -Depth 12|Set-Content "$report/result.json"
 Get-Content "$prefix.stdout.txt" -Tail 30
 Get-Content "$prefix.stderr.txt" -Tail 30
 if(!$done -or $p.ExitCode -ne 0){throw "Failed $Label (completed=$done exit=$($p.ExitCode))"}
}
function Invoke-Bab([string]$Label,[string]$InputXml){
 Invoke-Tool $Label 'BinaryAssetBuilder.exe' @(
 (Q $InputXml),('/od:'+(Q $built)),('/iod:'+(Q $built)),
 '/csc:false','/ls:true','/osh:false','/pc:true','/res:true','/slowclean:true','/ss:true',
 '/art:".;.\Mods\Upheaval\Art;.\Mods;.\Art"',
 '/audio:".;.\Mods\Upheaval\Audio;.\Mods;.\Audio"',
 '/data:".;.\Mods\Upheaval\Data;.\Mods;.\SageXml"')
}
try{
 foreach($exe in Get-ChildItem "$sdk/tools" -Filter '*.exe'){
  $path=$exe.FullName+'.config'
  [xml]$config=if(Test-Path $path){Get-Content $path -Raw}else{'<configuration/>'}
  $startup=$config.CreateElement('startup');$startup.SetAttribute('useLegacyV2RuntimeActivationPolicy','true')
  $runtime=$config.CreateElement('supportedRuntime');$runtime.SetAttribute('version','v4.0')
  $startup.AppendChild($runtime)|Out-Null
  if($config.configuration.startup){$config.configuration.RemoveChild($config.configuration.startup)|Out-Null}
  $config.DocumentElement.AppendChild($startup)|Out-Null
  $config.Save($path)
 }
 foreach($xml in @(Get-ChildItem "$mod/Data/AptUI" -Filter '*.xml' -File|Sort-Object Name)){
  Invoke-Bab ('aptui-'+$xml.BaseName) $xml.FullName
  if(!(Test-Path (Join-Path $data ('aptui/'+$xml.BaseName+'.manifest')))){throw 'Missing AptUI manifest'}
  [IO.File]::WriteAllText((Join-Path $data ('aptui/'+$xml.BaseName+'.version')),'')
 }
 foreach($xml in @(Get-ChildItem "$mod/Data/AdditionalMaps" -Filter 'mapmetadata_*.xml' -File|Sort-Object Name)){
  Invoke-Bab ('global-'+$xml.BaseName) $xml.FullName
  if(!(Test-Path (Join-Path $data ('additionalmaps/'+$xml.BaseName+'.manifest')))){throw 'Missing global manifest'}
 }
 Invoke-Bab 'mod' "$mod/Data/Mod.xml"
 foreach($ext in 'manifest','bin','imp','relo'){if(!(Test-Path "$data/mod.$ext")){throw "Missing fresh mod.$ext"}}
 $result['sourceCompilationSucceeded']=$true
 Compress-Archive "$out/*" "$report/compiled-before-postprocess.zip" -Force
 $assetDirs=@(Get-ChildItem "$mod/Assets" -Filter '*.asset' -Recurse -File|ForEach-Object DirectoryName|Sort-Object -Unique)
 $result['assetDirectories']=$assetDirs.Count
 foreach($dir in $assetDirs){Invoke-Tool 'merge-assets' 'AssetMerge.exe' @((Q "$data/mod"),(Q $dir))}
 Invoke-Tool 'hashfix' 'HashFix.exe' @((Q "$data/mod.manifest"))
 Invoke-Tool 'resolve-assets' 'ModAssetResolver.exe' @((Q "$data/mod.manifest"))
 Invoke-Tool 'lod' 'LoDStreamBuilder.exe' @((Q "$data/mod.manifest"))
 foreach($additional in @("$sdk/additional","$mod/additional")){
  if(Test-Path $additional){Copy-Item "$additional/*" $out -Recurse -Force}
 }
 $big=Join-Path $report 'Upheaval_1.0.big'
 Invoke-Tool 'package' 'MakeBig.exe' @('-f',(Q $out),'-x:*.asset',('-o:'+(Q $big)))
 if(!(Test-Path $big) -or (Get-Item $big).Length -eq 0){throw 'Empty or absent BIG'}
 @('mod-game 1.12','add-big Upheaval_1.0.big')|Set-Content "$report/Upheaval_1.0.skudef"
 $result.succeeded=$true
 $result.status='Original SDK compile, merge, fix, LOD and BIG packaging passed; game runtime not tested'
}catch{
 $result.status='failed';$result['error']=$_.ToString();Write-Host $_
}finally{
 $result['originalBinariesUnchanged']=$true
 foreach($name in $hashes.Keys){if((Get-FileHash "$sdk/tools/$name").Hash -ne $hashes[$name]){$result.originalBinariesUnchanged=$false}}
 if(!$result.originalBinariesUnchanged){$result.succeeded=$false;$result.status='Original binary integrity check failed'}
 $result['binarySha256']=$hashes
 $result['finishedUtc']=[DateTime]::UtcNow.ToString('o')
 $result.outputs=@(Get-ChildItem $out -Recurse -File|Where-Object Extension -ne '.asset'|ForEach-Object{
  @{path=$_.FullName.Substring($out.Length+1);bytes=$_.Length;sha256=(Get-FileHash $_.FullName).Hash}
 })
 $result|ConvertTo-Json -Depth 12|Set-Content "$report/result.json"
}
if(!$result.succeeded){throw "Upheaval original SDK validation failed: $($result.error)"}
