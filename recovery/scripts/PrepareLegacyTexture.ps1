[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$TargetExe)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$target=(Resolve-Path $TargetExe).Path
$directory=Split-Path $target
$output=Join-Path $directory 'BinaryAssetBuilder.TextureCompiler.dll'
$manifest="$output.patch.json"
if((Test-Path $output) -or (Test-Path $manifest)){throw 'Texture deployment must be created from a clean output directory'}
$original=Join-Path $root 'SDK/tools/BinaryAssetBuilder.TextureCompiler.dll'
if([IO.Path]::GetFullPath($directory).TrimEnd('\','/') -ieq [IO.Path]::GetFullPath((Split-Path $original)).TrimEnd('\','/')){throw 'Refusing to prepare texture in the original SDK directory'}
$work=Join-Path ([IO.Path]::GetTempPath()) ('texture-deployment-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $work|Out-Null
Push-Location $root
try {
    & dotnet build recovery/probes/TextureRedirect/Writer/TextureReferencePatch.csproj -c Release -p:Platform=AnyCPU -p:PlatformTarget=AnyCPU -o "$work/writer" *> "$work/writer-build.log"
    if($LASTEXITCODE -ne 0){Get-Content "$work/writer-build.log" -Tail 80|Write-Host;throw 'Texture patch tool build failed'}
    $candidate=Join-Path $work 'BinaryAssetBuilder.TextureCompiler.dll'
    & dotnet "$work/writer/TextureReferencePatch.dll" redirect $original $candidate $target *> "$work/patch.log"
    if($LASTEXITCODE -ne 0){Get-Content "$work/patch.log" -Tail 80|Write-Host;throw 'Texture deployment patch failed'}
    & python recovery/probes/TextureRedirect/AuditPe.py $original $candidate "$work/audit.json" --mode redirect
    if($LASTEXITCODE -ne 0){throw 'Texture protected PE audit failed'}
    $patch=Get-Content "$candidate.patch.json" -Raw|ConvertFrom-Json
    $audit=Get-Content "$work/audit.json" -Raw|ConvertFrom-Json
    if($audit.status -cne 'passed' -or $audit.outputSha256 -cne $patch.patchedSha256 -or $audit.originalSha256 -cne $patch.originalSha256){throw 'Texture audit does not describe the deployment candidate'}
    $patch|Add-Member -NotePropertyName protectedPeAudit -NotePropertyValue $audit
    $patch.status='static-audited-runtime-validation-required'
    $patch|ConvertTo-Json -Depth 12|Set-Content "$candidate.patch.json" -Encoding utf8
    # Publish only after validation; preserve the original SDK and retain work logs on failure.
    Copy-Item $candidate $output
    try {Copy-Item "$candidate.patch.json" $manifest} catch {Remove-Item $output -Force;throw}
    Write-Host 'Prepared EXE-bound texture deployment; component/runtime validation remains required.'
} finally {Pop-Location}
