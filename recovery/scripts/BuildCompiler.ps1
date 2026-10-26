[CmdletBinding()]
param(
    [ValidateSet('Debug','Release')][string]$Configuration='Release',
    [ValidateSet('Win32')][string]$Platform='Win32',
    [string]$OutputDirectory,
    [string]$FoundationDirectory
)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
if(!$OutputDirectory){$OutputDirectory=Join-Path $root "recovery/artifacts/compiler/$Configuration"}
if(Test-Path $OutputDirectory){throw "Refusing to reuse product output: $OutputDirectory"}
New-Item -ItemType Directory -Force $OutputDirectory|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$obj=Join-Path $out 'obj';$bin=Join-Path $out 'bin'
New-Item -ItemType Directory -Force $obj,$bin|Out-Null
function Invoke-CompilerTool([string]$Command,[string[]]$Arguments,[string]$Log) {
    & $Command @Arguments *> $Log
    if($LASTEXITCODE -ne 0){Get-Content $Log -Tail 100|ForEach-Object{Write-Host $_};throw "Compiler tool failed: $Command ($Log)"}
}
Push-Location $root
try {
    if(!$FoundationDirectory) {
        $FoundationDirectory=Join-Path $obj 'foundation'
        & "$PSScriptRoot/BuildManagedFoundation.ps1" -Configuration $Configuration -OutputDirectory $FoundationDirectory|Out-Host
    } elseif(Test-Path (Join-Path $FoundationDirectory "$Configuration/Managed.Foundation.netmodule")) {
        $FoundationDirectory=Join-Path $FoundationDirectory $Configuration
    }
    $FoundationDirectory=(Resolve-Path $FoundationDirectory).Path
    $foundation=(Resolve-Path (Join-Path $FoundationDirectory 'Managed.Foundation.netmodule')).Path
    $foundationReceipt=Get-Content (Join-Path $FoundationDirectory 'build.json') -Raw|ConvertFrom-Json
    if($foundationReceipt.configuration -cne $Configuration){throw 'Foundation configuration differs from requested product configuration'}
    if((Get-FileHash $foundation -Algorithm SHA256).Hash.ToLowerInvariant() -cne $foundationReceipt.moduleSha256){throw 'Foundation module differs from its build receipt'}
    $packageLine=(& dotnet nuget locals global-packages --list)
    if($LASTEXITCODE -ne 0){throw 'Cannot locate pinned compiler package'}
    $packageRoot=($packageLine -replace '^global-packages:\s*','').Trim()
    $csc=(Resolve-Path (Join-Path $packageRoot 'microsoft.net.compilers.toolset/4.0.1/tasks/net472/csc.exe')).Path
    $framework=(Resolve-Path (Join-Path ${env:ProgramFiles(x86)} 'Reference Assemblies/Microsoft/Framework/.NETFramework/v4.8')).Path
    $application=Join-Path $obj 'Managed.Application.netmodule'
    $sources=@(Get-ChildItem 'recovery/src/App' -Recurse -Filter *.cs -File|Where-Object{$_.FullName -notmatch '[\\/](obj|bin)[\\/]'}|Sort-Object FullName)
    $arguments=@('/nologo','/noconfig','/nostdlib+','/codepage:65001','/target:module','/platform:anycpu','/langversion:10.0','/deterministic+',('/out:"'+$application+'"'),('/addmodule:"'+$foundation+'"'))
    $arguments+=if($Configuration -eq 'Release'){'/optimize+'}else{'/optimize-'}
    foreach($reference in @('mscorlib','System','System.Core','System.Xml','System.Configuration','System.Data','System.Web','System.Configuration.Install','System.Web.Extensions')){$arguments+=('/reference:"'+(Join-Path $framework ($reference+'.dll'))+'"')}
    $arguments+=('/reference:"'+(Resolve-Path 'SDK/tools/antlr.runtime.dll').Path+'"')
    $arguments+=@($sources|ForEach-Object{'"'+$_.FullName+'"'})
    $rsp=Join-Path $obj 'Application.rsp'
    [IO.File]::WriteAllLines($rsp,[string[]]$arguments,[Text.UTF8Encoding]::new($false))
    Invoke-CompilerTool $csc @('@'+$rsp) (Join-Path $out 'application-build.log')
    $hostingSource=(Resolve-Path 'recovery/src/Hosting/CompilerEntry.cpp').Path
    $hostObject=Join-Path $obj 'CompilerEntry.obj'
    $optimization=if($Configuration -eq 'Release'){'/O2'}else{'/Od'}
    Invoke-CompilerTool 'cl.exe' @('/nologo','/c','/clr','/MD','/EHa','/fp:precise',$optimization,('/AI'+$obj),('/AI'+$FoundationDirectory),('/Fo'+$hostObject),$hostingSource) (Join-Path $out 'host-build.log')
    $exe=Join-Path $bin 'NewModCompiler.exe'
    $linkArguments=@('/nologo','/LTCG','/MACHINE:X86','/SUBSYSTEM:CONSOLE','/ENTRY:NewModCompiler.Hosting.CompilerEntry.Main',('/OUT:'+$exe),$hostObject,$foundation,$application)
    $resources=@(Get-Content (Join-Path $FoundationDirectory 'resources.json') -Raw|ConvertFrom-Json)
    foreach($resource in $resources){$linkArguments+=('/ASSEMBLYRESOURCE:'+$resource.path+','+$resource.logicalName)}
    Invoke-CompilerTool 'link.exe' $linkArguments (Join-Path $out 'link.log')
    Copy-Item 'recovery/src/App/App.config' "$exe.config"
    Copy-Item 'SDK/tools/antlr.runtime.dll' $bin
    $before=(Get-FileHash $exe -Algorithm SHA256).Hash
    $metadataTool=(Resolve-Path 'recovery/build/ManagedMetadata/bin/Release/net8.0/ManagedMetadata.dll').Path
    Invoke-CompilerTool 'dotnet' @($metadataTool,'verify-linked',$exe,(Resolve-Path 'SDK/tools/BinaryAssetBuilder.Utility.dll').Path) (Join-Path $out 'metadata.log')
    if((Get-FileHash $exe -Algorithm SHA256).Hash -cne $before){throw 'Metadata verification mutated the mixed executable'}
    @{
        schemaVersion=1;sourceCommit=(git rev-parse HEAD).Trim();configuration=$Configuration;runtime='net48/Win32';intermediateModules='AnyCPU'
        entryPoint='NewModCompiler.Hosting.CompilerEntry.Main';foundationSha256=$foundationReceipt.moduleSha256
        applicationSha256=(Get-FileHash $application -Algorithm SHA256).Hash.ToLowerInvariant();exeSha256=$before.ToLowerInvariant()
        compiler='Microsoft.Net.Compilers.Toolset/4.0.1';vcToolsVersion=$env:VCToolsVersion;windowsSdkVersion=$env:WindowsSDKVersion
        sources=@($sources|ForEach-Object{@{path=[IO.Path]::GetRelativePath($root,$_.FullName).Replace('\','/');sha256=(Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()}})
        hostingSource=@{path='recovery/src/Hosting/CompilerEntry.cpp';sha256=(Get-FileHash $hostingSource -Algorithm SHA256).Hash.ToLowerInvariant()}
        resources=$resources;runtimeFiles=@('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll')
        scope='P3 production application and all managed Foundation code in one physical EXE; Native asset routes and real MOD compilation remain P4/P5'
        containsProbeOrTestSources=$false;modCompilationSupported=$false;status='built; clean deployment validation required'
    }|ConvertTo-Json -Depth 8|Set-Content (Join-Path $out 'build.json')
    Write-Output $exe
} finally {Pop-Location}
