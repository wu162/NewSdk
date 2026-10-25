param(
 [Parameter(Mandatory)][string]$ReportDirectory,
 [Parameter(Mandatory)][object[]]$Builds,
 [Parameter(Mandatory)][string]$RecoveredSdkRoot
)
$ErrorActionPreference='Stop'
$assemblies=@(Get-ChildItem "$ReportDirectory/production" -Recurse -Filter assemblies.tsv | ForEach-Object {Import-Csv $_.FullName -Delimiter "`t"})
$calls=@(Get-ChildItem "$ReportDirectory/production" -Recurse -Filter calls.tsv | ForEach-Object {Import-Csv $_.FullName -Delimiter "`t"})
$xml=@(Get-ChildItem "$ReportDirectory/production" -Recurse -Filter xml.tsv | ForEach-Object {Import-Csv $_.FullName -Delimiter "`t"})
$xinclude=@(Get-ChildItem "$ReportDirectory/production" -Recurse -Filter xinclude.tsv | ForEach-Object {Import-Csv $_.FullName -Delimiter "`t"})
if(!$calls.Count -or !$xml.Count -or !$xinclude.Count){throw 'Missing actual Core plugin, XML loader, or XInclude execution evidence'}
$loads=@()
foreach($build in $Builds){
 $matches=@($assemblies|Where-Object name -ceq $build.assembly)
 # EALABuild/Remote are installed for inventory; the extracted BAB flow does not start their old hosts.
 $required=$build.assembly -notin @('EALABuild','BinaryAssetBuilder.Remote')
 if($required -and !$matches.Count){throw "Recovered component did not load in BAB: $($build.assembly)"}
 foreach($match in $matches){
  $expected=[IO.Path]::GetFullPath("$RecoveredSdkRoot/tools/$($build.assembly).dll")
  if(![string]::Equals($expected,$match.location,[StringComparison]::OrdinalIgnoreCase) -or $match.sha256.ToLowerInvariant() -cne $build.sha256.ToLowerInvariant()){
   throw "Wrong recovered assembly loaded: $($match.fullname) at $($match.location)"
  }
 }
 $loads+=@{assembly=$build.assembly;required=$required;actualLoads=$matches.Count;sha256=$build.sha256;identity=$build.fullName}
}
$routes=@(
 @{type='Texture';assembly='BinaryAssetBuilder.TextureCompiler'},
 @{type='AudioFile';assembly='BinaryAssetBuilder.AudioCompiler'},
 @{type='GameObject';assembly='Recovered.GameObjectPlugin'},
 @{type='WeaponTemplate';assembly='Recovered.WeaponPlugin'},
 @{type='ArmorTemplate';assembly='Recovered.Compiler'},
 @{type='W3DMesh';assembly='Recovered.W3x.Plugin'},
 @{type='W3DAnimation';assembly='Recovered.W3x.Plugin'}
)
$coverage=@()
foreach($route in $routes){
 $matches=@($calls|Where-Object {$_.type -ceq $route.type -and $_.pluginAssembly.Split(',')[0] -ceq $route.assembly})
 if(!$matches.Count){throw "No actual $($route.type) calls to $($route.assembly)"}
 $coverage+=@{type=$route.type;assembly=$route.assembly;calls=$matches.Count}
}
if(!@($calls|Where-Object {$_.pluginAssembly.Split(',')[0] -ceq 'Recovered.XmlPriority.Plugin'}).Count){throw 'Recovered XML route never executed'}
foreach($row in $xml){if($row.reader -notmatch '^Mvp.Xml.XInclude.XIncludingReader, Mvp.Xml,'){throw "Unexpected XML reader: $($row.reader)"}}
$byRoute=@($calls|Group-Object type,pluginAssembly|ForEach-Object {@{route=$_.Name;count=$_.Count}})
# Preserve precise actual dispatch, including remaining original XML default handlers.
$byRoute|ConvertTo-Json -Depth 6|Set-Content "$ReportDirectory/actual-dispatch.json"
@{succeeded=$true;loads=$loads;requiredRoutes=$coverage;totalCalls=$calls.Count;xmlLoads=$xml.Count;xincludeResolutions=$xinclude.Count;actualDispatch='actual-dispatch.json';coldCache=$true}
