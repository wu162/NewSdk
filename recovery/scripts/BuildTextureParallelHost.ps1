[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$OutputDirectory)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$out=(New-Item -ItemType Directory $OutputDirectory).FullName
function Checked([string]$Command,[string[]]$Arguments,[string]$Log) {
    & $Command @Arguments *> $Log
    if($LASTEXITCODE -ne 0){Get-Content $Log -Tail 80;throw "Texture parallel host build failed: $Command"}
}
$packages=((& dotnet nuget locals global-packages --list) -replace '^global-packages:\s*','').Trim()
$csc=Join-Path $packages 'microsoft.net.compilers.toolset/4.0.1/tasks/net472/csc.exe'
$refs=Join-Path ${env:ProgramFiles(x86)} 'Reference Assemblies/Microsoft/Framework/.NETFramework/v4.8'
foreach($mode in @('Debug','Release')) {
    $build=(New-Item -ItemType Directory "$out/$mode").FullName
    $obj=(New-Item -ItemType Directory "$build/obj").FullName
    $bin=(New-Item -ItemType Directory "$build/bin").FullName
    & "$PSScriptRoot/BuildManagedFoundation.ps1" -Configuration $mode -OutputDirectory "$obj/foundation" *> "$build/foundation.log"
    if($LASTEXITCODE -ne 0){Get-Content "$build/foundation.log" -Tail 80;throw 'Foundation build failed'}
    $foundation=(Resolve-Path "$obj/foundation/Managed.Foundation.netmodule").Path
    $module=Join-Path $obj 'Managed.TextureParallelProbe.netmodule'
    $arguments=@('/nologo','/noconfig','/nostdlib+','/codepage:65001','/target:module','/platform:anycpu','/langversion:7.3',"/addmodule:$foundation","/out:$module")
    $arguments+=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
    foreach($reference in @('mscorlib','System','System.Core','System.Xml','System.Configuration','System.Data','System.Web','System.Configuration.Install','System.Web.Extensions')){$arguments+=('/r:'+(Join-Path $refs ($reference+'.dll')))}
    $arguments+=('/r:'+(Join-Path $root 'SDK/tools/antlr.runtime.dll'))
    $arguments+=(Resolve-Path "$root/recovery/tests/TextureParallelProbe.cs").Path
    Checked $csc $arguments "$build/probe-build.log"
    $object=Join-Path $obj 'TextureParallelEntry.obj'
    $optimization=if($mode -eq 'Release'){'/O2'}else{'/Od'}
    Checked 'cl.exe' @('/nologo','/c','/clr','/MD','/EHa','/fp:precise',$optimization,"/AI$obj",('/AI'+(Split-Path $foundation)),"/Fo$object",(Resolve-Path "$root/recovery/probes/TextureRedirect/TextureParallelEntry.cpp").Path) "$build/entry-build.log"
    $exe=Join-Path $bin 'NewModCompiler.exe'
    $link=@('/nologo','/LTCG','/MACHINE:X86','/SUBSYSTEM:CONSOLE','/ENTRY:TextureParallelEntry.Main',"/OUT:$exe",$object,$foundation,$module)
    foreach($resource in @(Get-Content "$obj/foundation/resources.json" -Raw|ConvertFrom-Json)){$link+=('/ASSEMBLYRESOURCE:'+$resource.path+','+$resource.logicalName)}
    Checked 'link.exe' $link "$build/link.log"
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.8"/></startup></configuration>'|Set-Content "$exe.config"
    Copy-Item "$root/SDK/tools/antlr.runtime.dll" $bin
    & "$PSScriptRoot/PrepareLegacyTexture.ps1" -TargetExe $exe *> "$build/prepare-texture.log"
    if($LASTEXITCODE -ne 0){Get-Content "$build/prepare-texture.log" -Tail 80;throw 'Host-bound texture preparation failed'}
    # The final EXE must run after every source/build netmodule has moved away.
    Move-Item $obj "$build/quarantined-obj"
}
