param([string]$OutputDirectory = '')
$ErrorActionPreference = 'Stop'
$ui = Split-Path -Parent $PSScriptRoot
if (-not $OutputDirectory) { $OutputDirectory = Join-Path $ui '.work/single-exe-publish' }
$msbuild = Get-Command msbuild -ErrorAction SilentlyContinue
if ($msbuild) { $builder = $msbuild.Source }
else {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
    $builder = & $vswhere -latest -products '*' -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -find 'MSBuild/**/Bin/MSBuild.exe' | Select-Object -First 1
    if (-not $builder) { throw 'Visual Studio C++ build tools and MSBuild are required.' }
}
& $builder "$PSScriptRoot/native/SwfCompiler.Native.vcxproj" /m /p:Configuration=Release /p:Platform=x64
if ($LASTEXITCODE -ne 0) { throw 'Native static bridge build failed.' }
dotnet publish "$PSScriptRoot/src/SwfCompiler.csproj" -c Release -r win-x64 -o $OutputDirectory
if ($LASTEXITCODE -ne 0) { throw 'Native AOT publish failed.' }
if (-not (Test-Path "$OutputDirectory/SwfCompiler.exe")) { throw 'Published EXE missing.' }
Write-Host "Published single-EXE framework: $OutputDirectory/SwfCompiler.exe"
