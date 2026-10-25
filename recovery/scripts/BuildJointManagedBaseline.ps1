param(
    [Parameter(Mandatory = $true)][string]$OutputDirectory,
    [string]$BuildDirectory,
    [string]$LogDirectory
)

# B0 preparation only: never install dependencies copied by MSBuild, since they
# may be original SDK assemblies referenced by HintPath rather than rebuilt code.
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
$output = [IO.Path]::GetFullPath($OutputDirectory)
if (!$BuildDirectory) {
    $temporary = if ($env:RUNNER_TEMP) { $env:RUNNER_TEMP } else { [IO.Path]::GetTempPath() }
    $BuildDirectory = Join-Path $temporary ('joint-managed-' + [Guid]::NewGuid().ToString('N'))
}
$build = [IO.Path]::GetFullPath($BuildDirectory)
if (!$LogDirectory) { $LogDirectory = Join-Path $build 'logs' }
$logs = [IO.Path]::GetFullPath($LogDirectory)
$sdk = [IO.Path]::GetFullPath((Join-Path $root 'SDK'))
foreach ($directory in @($output, $build, $logs)) {
    if ($directory.Equals($sdk, [StringComparison]::OrdinalIgnoreCase) -or
        $directory.StartsWith($sdk + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Joint baseline preparation must not write into the original SDK: $directory"
    }
}
if ($build.Equals($output, [StringComparison]::OrdinalIgnoreCase)) {
    throw 'BuildDirectory and OutputDirectory must differ'
}
New-Item -ItemType Directory -Force $output, $build, $logs | Out-Null

$projects = @(
    @{ name = 'Utility'; assembly = 'BinaryAssetBuilder.Utility' },
    @{ name = 'Metrics'; assembly = 'EALA.Metrics' },
    @{ name = 'ExpressionEval'; assembly = 'BinaryAssetBuilder.ExpressionEval' },
    @{ name = 'GameDataVerifier'; assembly = 'BinaryAssetBuilder.GameDataVerifier' },
    @{ name = 'MvpXml'; assembly = 'Mvp.Xml' },
    @{ name = 'Core'; assembly = 'BinaryAssetBuilder.Core' },
    @{ name = 'Remote'; assembly = 'BinaryAssetBuilder.Remote' },
    @{ name = 'EALABuild'; assembly = 'EALABuild' }
)
$rows = @()
Push-Location $root
try {
    foreach ($entry in $projects) {
        $project = "recovery/src/Managed/$($entry.name)/$($entry.assembly).csproj"
        $projectPath = (Resolve-Path $project).Path
        $projectOutput = Join-Path $build $entry.name
        $log = Join-Path $logs "$($entry.name)-build.log"
        & dotnet build $projectPath -c Release -p:PlatformTarget=x86 -o $projectOutput --nologo *> $log
        if ($LASTEXITCODE -ne 0) {
            Get-Content $log -Tail 80 | Write-Host
            throw "Joint managed baseline build failed: $project; log: $log"
        }
        $source = Join-Path $projectOutput "$($entry.assembly).dll"
        if (!(Test-Path $source -PathType Leaf)) { throw "Missing built target: $source" }
        $destination = Join-Path $output "$($entry.assembly).dll"
        $hash = (Get-FileHash $source -Algorithm SHA256).Hash.ToLowerInvariant()
        Copy-Item -LiteralPath $source -Destination $destination -Force
        if ((Get-FileHash $destination -Algorithm SHA256).Hash.ToLowerInvariant() -cne $hash) {
            throw "Joint managed installation mismatch: $destination"
        }
        $identity = [Reflection.AssemblyName]::GetAssemblyName($destination)
        if ($identity.Name -cne $entry.assembly) { throw "Unexpected assembly identity: $($identity.FullName)" }
        $rows += [pscustomobject]@{
            assembly = $entry.assembly
            file = $destination
            sha256 = $hash
            sourceProject = $project
            fullName = $identity.FullName
            configuration = 'Release'
            platform = 'x86'
            buildLog = $log
        }
    }
    # All files have now been copied, including unsigned Mvp.Xml. Recheck the
    # explicit target set to catch accidental overwrites during preparation.
    foreach ($row in $rows) {
        if ((Get-FileHash $row.file -Algorithm SHA256).Hash.ToLowerInvariant() -cne $row.sha256) {
            throw "Joint managed target changed during preparation: $($row.file)"
        }
    }
} finally {
    Pop-Location
}
$rows
