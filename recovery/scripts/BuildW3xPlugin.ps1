param(
    [Parameter(Mandatory=$true)][string]$OutputDirectory,
    [ValidateSet('Od','O2')][string]$Mode = 'O2',
    [string]$DependencyDirectory
)
$ErrorActionPreference = 'Stop'
$repositoryRoot = (Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $repositoryRoot
if ($env:VSCMD_ARG_TGT_ARCH -and $env:VSCMD_ARG_TGT_ARCH -ne 'x86') { throw 'W3X plugin requires the MSVC x86 target environment' }
if (Test-Path $OutputDirectory) {
    if (@(Get-ChildItem -Force $OutputDirectory).Count) { throw "Refusing to overwrite a nonempty plugin output directory: $OutputDirectory" }
} else { New-Item -ItemType Directory $OutputDirectory -Force | Out-Null }
$out = (Resolve-Path $OutputDirectory).Path
$objectsDirectory = Join-Path $out 'objects'
$logsDirectory = Join-Path $out 'build-logs'
New-Item -ItemType Directory $objectsDirectory,$logsDirectory | Out-Null
# Copy runtime dependencies while excluding the original compiler absolutely.
Get-ChildItem SDK/tools -Filter '*.dll' | Where-Object Name -ne 'BinaryAssetBuilder.W3XCompiler.dll' | Copy-Item -Destination $out
$dependencyCoreHash = $null
if ($DependencyDirectory) {
    $dependencies = (Resolve-Path $DependencyDirectory).Path
    $dependencyCore = Join-Path $dependencies 'BinaryAssetBuilder.Core.dll'
    if (!(Test-Path $dependencyCore -PathType Leaf)) { throw "Dependency directory has no BinaryAssetBuilder.Core.dll: $dependencies" }
    $dependencyCoreHash = (Get-FileHash $dependencyCore -Algorithm SHA256).Hash
    # Joint SDK validation must compile both bridges and the managed plugin
    # against the same recovered Core installed in the disposable SDK.
    Get-ChildItem $dependencies -Filter '*.dll' -File | Where-Object Name -ne 'BinaryAssetBuilder.W3XCompiler.dll' | Copy-Item -Destination $out -Force
    if ((Get-FileHash "$out/BinaryAssetBuilder.Core.dll" -Algorithm SHA256).Hash -ne $dependencyCoreHash) {
        throw 'W3X dependency Core hash mismatch before compilation'
    }
}
$architecture = Join-Path $objectsDirectory 'Architecture.cpp'
'static_assert(sizeof(void*) == 4, "W3X plugin requires x86");' | Set-Content $architecture
$architectureObject = Join-Path $objectsDirectory 'Architecture.obj'
& cl.exe /nologo /c /EHsc /std:c++14 /MD "/Fo$architectureObject" $architecture *> "$logsDirectory/architecture.log"
if ($LASTEXITCODE -ne 0) { Get-Content "$logsDirectory/architecture.log"; throw 'W3X plugin architecture check failed' }
$sources = @(Get-ChildItem recovery/src/Native/W3x -Filter '*.cpp' | Sort-Object Name | ForEach-Object FullName)
$sources += (Resolve-Path 'recovery/src/Native/Relocation.cpp').Path
$sources += (Resolve-Path 'recovery/src/Native/ScalarParsing.cpp').Path
$sources += (Resolve-Path 'recovery/src/Native/ResourceMarshaling.cpp').Path
$objects = @()
foreach ($source in $sources) {
    $name = [IO.Path]::GetFileNameWithoutExtension($source)
    $object = Join-Path $objectsDirectory "$name.obj"
    & cl.exe /nologo /c /EHsc /std:c++14 /fp:precise "/$Mode" /MD "/Fo$object" $source *> "$logsDirectory/$name.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$logsDirectory/$name.log"; throw "W3X plugin native build failed: $name" }
    $objects += $object
}
foreach ($bridge in @(@{ Source='AnimationCompiler'; Name='Recovered.W3x.Native' }, @{ Source='MeshCompiler'; Name='Recovered.W3x.Mesh' })) {
    $source = (Resolve-Path "recovery/src/Bridge/W3x/$($bridge.Source).cpp").Path
    $library = Join-Path $out "$($bridge.Name).dll"
    $object = Join-Path $objectsDirectory "$($bridge.Source)Bridge.obj"
    & cl.exe /nologo /clr /LD /EHa /fp:precise "/$Mode" /MD "/AI$out" "/Fe$library" "/Fo$object" $source $objects *> "$logsDirectory/$($bridge.Name).log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$logsDirectory/$($bridge.Name).log"; throw "W3X plugin bridge build failed: $($bridge.Name)" }
}
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$optimize = if ($Mode -eq 'O2') { '/optimize+' } else { '/optimize-' }
$plugin = Join-Path $out 'Recovered.W3x.Plugin.dll'
$managedSources = @(
    (Resolve-Path 'recovery/src/Managed/W3x/W3xCatalog.cs').Path
    (Resolve-Path 'recovery/src/Managed/W3x/IW3xAssetCompiler.cs').Path
    (Resolve-Path 'recovery/src/Plugin/W3x/Plugin.cs').Path
    (Resolve-Path 'recovery/src/Plugin/W3x/NativePlugin.cs').Path
)
& $csc /nologo /target:library /platform:x86 $optimize /r:System.Xml.dll /r:"$out/BinaryAssetBuilder.Core.dll" /r:"$out/Recovered.W3x.Native.dll" /r:"$out/Recovered.W3x.Mesh.dll" /out:"$plugin" $managedSources *> "$logsDirectory/managed-plugin.log"
if ($LASTEXITCODE -ne 0) { Get-Content "$logsDirectory/managed-plugin.log"; throw 'W3X plugin managed build failed' }
if (Test-Path "$out/BinaryAssetBuilder.W3XCompiler.dll") { throw 'Original W3X DLL unexpectedly copied into plugin output' }
if ($dependencyCoreHash -and (Get-FileHash "$out/BinaryAssetBuilder.Core.dll" -Algorithm SHA256).Hash -ne $dependencyCoreHash) {
    throw 'W3X dependency Core changed during compilation'
}
@{
    sourceCommit=(git rev-parse HEAD).Trim(); mode=$Mode
    pluginType='Recovered.Compiler.W3x.NativePlugin, Recovered.W3x.Plugin'
    pluginSha256=(Get-FileHash $plugin -Algorithm SHA256).Hash.ToLowerInvariant()
    bridgeAnimationSha256=(Get-FileHash "$out/Recovered.W3x.Native.dll" -Algorithm SHA256).Hash.ToLowerInvariant()
    bridgeMeshSha256=(Get-FileHash "$out/Recovered.W3x.Mesh.dll" -Algorithm SHA256).Hash.ToLowerInvariant()
    coreSha256=(Get-FileHash "$out/BinaryAssetBuilder.Core.dll" -Algorithm SHA256).Hash.ToLowerInvariant()
    originalW3xPresent=$false; sdkValidated=$false
} | ConvertTo-Json | Set-Content "$out/plugin-build.json"
Write-Host "Built recovered W3X plugin ($Mode): $plugin"
