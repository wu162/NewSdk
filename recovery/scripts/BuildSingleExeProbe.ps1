param(
    [ValidateSet('Debug','Release')][string]$Configuration = 'Release',
    [Parameter(Mandatory=$true)][string]$OutputDirectory,
    [string]$FoundationDirectory,
    [switch]$Minimal
)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
if (Test-Path $OutputDirectory) { throw "Refusing to overwrite probe build: $OutputDirectory" }
New-Item -ItemType Directory -Force $OutputDirectory | Out-Null
$out = (Resolve-Path $OutputDirectory).Path
$work = Join-Path $out 'obj'
$bin = Join-Path $out 'bin'
New-Item -ItemType Directory -Force $work,$bin | Out-Null
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
function Invoke-Checked([string]$Command,[string[]]$Arguments,[string]$Log) {
    & $Command @Arguments *> $Log
    if ($LASTEXITCODE -ne 0) { Get-Content $Log -Tail 100 | ForEach-Object { Write-Host $_ }; throw "$Command failed: $Log" }
}
Push-Location $root
try {
    $managedOpt = if ($Configuration -eq 'Release') { '/optimize+' } else { '/optimize-' }
    if ($Minimal) {
        # Syntax normalization only: same AssetBuffer class body, old framework
        # csc cannot parse its file-scoped namespace. No production source edit.
        $asset = Get-Content 'recovery/src/Managed/Core/BinaryAssetBuilder.Core/AssetBuffer.cs' -Raw
        if (!$asset.Contains('namespace BinaryAssetBuilder.Core;')) { throw 'AssetBuffer namespace changed; review generator' }
        $asset.Replace('namespace BinaryAssetBuilder.Core;',"namespace BinaryAssetBuilder.Core`n{") + "`n}" | Set-Content "$work/AssetBuffer.cs" -Encoding UTF8
        Invoke-Checked $csc @('/nologo','/codepage:65001','/target:module','/platform:anycpu','/unsafe',$managedOpt,"/out:$work/Managed.Foundation.netmodule",(Resolve-Path "$work/AssetBuffer.cs").Path,(Resolve-Path 'recovery/src/Managed/Hash/FastHash.cs').Path) "$out/foundation-build.log"
        $foundation = [IO.Path]::GetFullPath("$work/Managed.Foundation.netmodule")
        $resources = @(@{path=(Join-Path $root 'recovery/src/Managed/Core/BinaryAssetBuilderProject.xsd');logicalName='BinaryAssetBuilderProject.xsd'})
    } else {
        if (!$FoundationDirectory) { throw 'Supply -FoundationDirectory from BuildManagedFoundation.ps1, or explicitly select -Minimal' }
        $FoundationDirectory = (Resolve-Path $FoundationDirectory).Path
        $foundation = (Resolve-Path "$FoundationDirectory/Managed.Foundation.netmodule").Path
        $resources = @(Get-Content "$FoundationDirectory/resources.json" -Raw | ConvertFrom-Json)
    }
    # Derive the bridge from the real source, not a rewritten implementation.
    # Minimal closure only removes the high-level Core ProcessInstance adapter.
    $bridge = (Get-Content 'recovery/src/Bridge/ArmorCompiler.cpp' -Raw).Replace("`r`n","`n")
    $bridge = $bridge.Replace('#using <BinaryAssetBuilder.Core.dll>', '#using "Managed.Foundation.netmodule"')
    $bridge = $bridge.Replace('#include "XmlNodeAdapter.h"', '#include "' + (Join-Path $root 'recovery/src/Bridge/XmlNodeAdapter.h').Replace('\','/') + '"')
    $bridge = $bridge.Replace('#include "../Native/ArmorTemplate.h"', '#include "' + (Join-Path $root 'recovery/src/Native/ArmorTemplate.h').Replace('\','/') + '"')
    if ($Minimal) {
        $method = $bridge.IndexOf('    static AssetBuffer^ ProcessInstance(')
        if ($method -lt 0 -or !$bridge.Substring($method).TrimEnd().EndsWith("};`n}")) { throw 'Armor bridge tail changed; review generator' }
        $bridge = $bridge.Substring(0,$method) + "};`n}`n"
    }
    $bridge | Set-Content "$work/ArmorCompiler.cpp" -Encoding UTF8
    $opt = if ($Configuration -eq 'Debug') { '/Od' } else { '/O2' }
    $objects = @()
    foreach ($source in @('recovery/src/Native/Relocation.cpp','recovery/src/Native/ScalarParsing.cpp','recovery/src/Native/ArmorTemplate.cpp',"$work/ArmorCompiler.cpp")) {
        $source = (Resolve-Path $source).Path
        $object = Join-Path $work ([IO.Path]::GetFileNameWithoutExtension($source) + '.obj')
        Invoke-Checked 'cl.exe' @('/nologo','/c','/clr','/MD','/EHa',$opt,'/fp:precise',"/AI$([IO.Path]::GetDirectoryName($foundation))","/Fo$object",$source) "$out/$([IO.Path]::GetFileNameWithoutExtension($source))-build.log"
        $objects += $object
    }
    $module = Join-Path $work 'Managed.Application.netmodule'
    $modulePlatform = '/platform:anycpu'
    $applicationArgs = @('/nologo','/codepage:65001','/target:module',$modulePlatform,$managedOpt,"/addmodule:$foundation,$($objects[-1])",'/r:System.Xml.dll','/r:System.Core.dll',"/out:$module",(Resolve-Path 'recovery/probes/SingleExe/Probe.cs').Path)
    $applicationCsc = $csc
    if (!$Minimal) {
        $packageLine = (& dotnet nuget locals global-packages --list)
        if($LASTEXITCODE -ne 0){throw 'Cannot locate pinned application compiler'}
        $packageRoot = ($packageLine -replace '^global-packages:\s*','').Trim()
        $applicationCsc = Join-Path $packageRoot 'microsoft.net.compilers.toolset/4.0.1/tasks/net472/csc.exe'
        if(!(Test-Path $applicationCsc)){throw 'Roslyn 4.0.1 compiler is required for full application module'}
        $applicationCsc = (Resolve-Path $applicationCsc).Path
        $framework = Join-Path ${env:ProgramFiles(x86)} 'Reference Assemblies/Microsoft/Framework/.NETFramework/v4.8'
        $applicationArgs = @($applicationArgs | Where-Object { $_ -notin @('/r:System.Xml.dll','/r:System.Core.dll') })
        $applicationArgs += @('/noconfig','/nostdlib+','/langversion:10.0')
        foreach($name in @('mscorlib','System','System.Core','System.Xml','System.Configuration','System.Data','System.Web','System.Configuration.Install','System.Web.Extensions')) {
            $applicationArgs += ('/r:' + (Join-Path $framework ($name+'.dll')))
        }
        $applicationArgs += ('/r:' + (Join-Path $root 'SDK/tools/antlr.runtime.dll'))
        $applicationArgs += @(Get-ChildItem 'recovery/src/App' -Recurse -Filter *.cs -File | Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' } | Sort-Object FullName | ForEach-Object FullName)
        $applicationArgs += @('/define:FULL_FOUNDATION',(Resolve-Path 'recovery/build/ManagedValidation/ManagedFoundationValidation.cs').Path,(Resolve-Path 'recovery/probes/TextureRedirect/TextureProbe.cs').Path)
    }
    Invoke-Checked $applicationCsc $applicationArgs "$out/application-build.log"
    $linkArgs = @('/nologo','/LTCG','/MACHINE:X86','/SUBSYSTEM:CONSOLE','/ENTRY:SingleExeProbe.Main',"/OUT:$bin/NewModCompiler.exe") + $objects + @($foundation,$module)
    foreach ($resource in $resources) {
        $path = [string]$resource.path
        if (![IO.Path]::IsPathRooted($path)) { $path = Join-Path $FoundationDirectory $path }
        $linkArgs += "/ASSEMBLYRESOURCE:$path,$($resource.logicalName)"
    }
    Invoke-Checked 'link.exe' $linkArgs "$out/link.log"
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.8" /></startup></configuration>' | Set-Content "$bin/NewModCompiler.exe.config"
    if (!$Minimal) {
        Copy-Item 'SDK/tools/antlr.runtime.dll' $bin
        Copy-Item (Join-Path $FoundationDirectory 'Foundation.types.txt') $bin
        $metadataTool=Join-Path $root 'recovery/build/ManagedMetadata/bin/Release/net8.0/ManagedMetadata.dll'
        $beforeMetadataAudit = (Get-FileHash "$bin/NewModCompiler.exe" -Algorithm SHA256).Hash
        Invoke-Checked 'dotnet' @($metadataTool,'verify-linked',"$bin/NewModCompiler.exe",(Join-Path $root 'SDK/tools/BinaryAssetBuilder.Utility.dll')) "$out/linked-metadata.log"
        if((Get-FileHash "$bin/NewModCompiler.exe" -Algorithm SHA256).Hash -cne $beforeMetadataAudit){throw 'Read-only final metadata audit changed the mixed executable'}
    }
    $sourcePaths = @('recovery/src/Native/Relocation.cpp','recovery/src/Native/Relocation.h','recovery/src/Native/ScalarParsing.cpp','recovery/src/Native/ScalarParsing.h','recovery/src/Native/ArmorTemplate.cpp','recovery/src/Native/ArmorTemplate.h','recovery/src/Bridge/ArmorCompiler.cpp','recovery/src/Bridge/XmlNodeAdapter.h','recovery/probes/SingleExe/Probe.cs')
    if (!$Minimal) {
        $sourcePaths += @('recovery/build/ManagedValidation/ManagedFoundationValidation.cs','recovery/probes/TextureRedirect/TextureProbe.cs')
        $sourcePaths += @(Get-ChildItem 'recovery/src/App' -Recurse -Filter *.cs -File | Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' } | ForEach-Object { [IO.Path]::GetRelativePath($root,$_.FullName).Replace('\','/') })
    }
    # Foundation.types.txt is test-only inventory, not a product runtime dependency.
    @{
        sourceCommit=(git rev-parse HEAD).Trim(); configuration=$Configuration
        toolchain=@{clPath=(Get-Command cl.exe).Source;clVersion=[Diagnostics.FileVersionInfo]::GetVersionInfo((Get-Command cl.exe).Source).FileVersion;linkPath=(Get-Command link.exe).Source;linkVersion=[Diagnostics.FileVersionInfo]::GetVersionInfo((Get-Command link.exe).Source).FileVersion;applicationCscPath=$applicationCsc;applicationCscVersion=[Diagnostics.FileVersionInfo]::GetVersionInfo($applicationCsc).FileVersion;cscPath=$csc;cscVersion=[Diagnostics.FileVersionInfo]::GetVersionInfo($csc).FileVersion;vcToolsVersion=$env:VCToolsVersion;sdkVersion=$env:WindowsSDKVersion;fp='precise';platform='x86';intermediateModulePlatform='AnyCPU'}
        sourceFiles=@($sourcePaths | Sort-Object -Unique | ForEach-Object { @{path=$_;sha256=(Get-FileHash $_ -Algorithm SHA256).Hash.ToLowerInvariant()} })
        scope=if($Minimal){'minimal link probe: real AssetBuffer, Hash, Armor CompileXml; not full G1'}else{'Foundation plus real Armor bridge unified-link probe'}
        foundationSha256=(Get-FileHash $foundation -Algorithm SHA256).Hash.ToLowerInvariant()
        bridgeSourceSha256=(Get-FileHash 'recovery/src/Bridge/ArmorCompiler.cpp' -Algorithm SHA256).Hash.ToLowerInvariant()
        generatedBridgeSha256=(Get-FileHash "$work/ArmorCompiler.cpp" -Algorithm SHA256).Hash.ToLowerInvariant()
        exeSha256=(Get-FileHash "$bin/NewModCompiler.exe" -Algorithm SHA256).Hash.ToLowerInvariant()
        resources=$resources; minimal=[bool]$Minimal; status='built; runtime validation required'
    } | ConvertTo-Json -Depth 8 | Set-Content "$out/build.json"
    return "$bin/NewModCompiler.exe"
} finally { Pop-Location }
