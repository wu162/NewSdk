param([string]$OutputDirectory='recovery/evidence/audio/ealayer3', [Parameter(Mandatory=$true)][string]$UpstreamDirectory)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$out=[IO.Path]::GetFullPath($OutputDirectory)
$upstream=(Resolve-Path $UpstreamDirectory).Path
$bin=Join-Path $out 'oracle/bin';New-Item -ItemType Directory -Force $bin|Out-Null
$source=Join-Path $upstream 'BinaryAssetBuilder.EALayer3AudioCompiler'
$project=Join-Path $out 'oracle/Oracle.csproj'
@"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><TargetFramework>net472</TargetFramework><AllowUnsafeBlocks>true</AllowUnsafeBlocks><GenerateAssemblyInfo>false</GenerateAssemblyInfo><EnableDefaultCompileItems>false</EnableDefaultCompileItems><AssemblyName>BinaryAssetBuilder.EALayer3AudioCompiler</AssemblyName><LangVersion>7.3</LangVersion><PlatformTarget>x86</PlatformTarget></PropertyGroup>
  <ItemGroup><Compile Include="$source/**/*.cs" Exclude="$source/obj/**;$source/bin/**" />
  <Reference Include="BinaryAssetBuilder.Core"><HintPath>$root/SDK/tools/BinaryAssetBuilder.Core.dll</HintPath></Reference>
  <Reference Include="EALA.Hash"><HintPath>$root/SDK/tools/EALA.Hash.dll</HintPath></Reference>
  <Reference Include="EALA.Metrics"><HintPath>$root/SDK/tools/EALA.Metrics.dll</HintPath></Reference>
  </ItemGroup>
</Project>
"@|Set-Content $project
& dotnet build $project -c Release -o $bin *> "$out/oracle/build.log"
if($LASTEXITCODE -ne 0){Get-Content "$out/oracle/build.log" -Tail 50;throw 'Jonwil RA3 plugin oracle build failed'}
Copy-Item "$upstream/audio.dll" $bin
Copy-Item "$out/Release/bin/AudioEaLayer3Host.exe*" $bin
Copy-Item SDK/tools/*.dll $bin -Exclude BinaryAssetBuilder.AudioCompiler.dll
[xml]$xml=Get-Content "$out/fixtures/assets.xml" -Raw
foreach($node in @($xml.DocumentElement.ChildNodes)) {
    if($node.id -like 'short*' -or $node.File -like '*.aiff'){$xml.DocumentElement.RemoveChild($node)|Out-Null}
}
$xml.Save("$out/oracle/assets.xml")
& "$bin/AudioEaLayer3Host.exe" "$bin/BinaryAssetBuilder.EALayer3AudioCompiler.dll" BinaryAssetBuilder.EALayer3AudioCompiler.Plugin "$out/oracle/assets.xml" "$out/oracle/assets" 1 *> "$out/oracle/encode.log"
if($LASTEXITCODE -ne 0){Get-Content "$out/oracle/encode.log" -Tail 70;throw 'Jonwil RA3 reference encode failed'}
$decoder=(Get-ChildItem (Join-Path $out 'vgmstream') -Recurse -Filter vgmstream-cli.exe|Select-Object -First 1).FullName
python recovery/scripts/CompareAudioEaLayer3Oracle.py --root $out --decoder $decoder
if($LASTEXITCODE -ne 0){throw 'Jonwil RA3 format/decoded behavior comparison failed'}
