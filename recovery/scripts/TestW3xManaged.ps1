param([string]$OutputDirectory)
$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if (!$env:RUNNER_TEMP) { $env:RUNNER_TEMP = [IO.Path]::GetTempPath() }
if (!$OutputDirectory) {
    $suffix = if ($env:GITHUB_RUN_ID) { "$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT" } else { 'local-' + [Guid]::NewGuid().ToString('N') }
    $OutputDirectory = "recovery/evidence/w3x/managed-runs/$suffix"
}
if (Test-Path $OutputDirectory) { throw "Refusing to overwrite evidence: $OutputDirectory" }
New-Item -ItemType Directory $OutputDirectory -Force | Out-Null
$out = (Resolve-Path $OutputDirectory).Path
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$originalHash = (Get-FileHash SDK/tools/BinaryAssetBuilder.W3XCompiler.dll -Algorithm SHA256).Hash.ToLowerInvariant()
if ($originalHash -ne 'bff82309b8e5e9f689112202e3613ad335148934c92dc70fe143f6e60d9ad3f1') { throw 'Original W3X DLL changed' }
$sourceCommit = (git rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0) { throw 'Cannot identify source commit' }
$results = @()
foreach ($mode in @('unoptimized', 'optimized')) {
    $work = Join-Path $env:RUNNER_TEMP ('w3x-managed-' + $mode + '-' + [Guid]::NewGuid().ToString('N'))
    New-Item -ItemType Directory $work | Out-Null
    Copy-Item SDK/tools/*.dll $work
    $core = Join-Path $work 'BinaryAssetBuilder.Core.dll'
    $original = Join-Path $work 'BinaryAssetBuilder.W3XCompiler.dll'
    $library = Join-Path $work 'Recovered.W3x.Managed.dll'
    $optimize = if ($mode -eq 'optimized') { '/optimize+' } else { '/optimize-' }
    $sources = @(
        (Resolve-Path 'recovery/src/Managed/W3x/W3xCatalog.cs').Path
        (Resolve-Path 'recovery/src/Managed/W3x/IW3xAssetCompiler.cs').Path
        (Resolve-Path 'recovery/src/Plugin/W3x/Plugin.cs').Path
    )
    & $csc /nologo /target:library /platform:x86 $optimize /r:"$core" /out:"$library" $sources *> "$out/$mode-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-build.log"; throw "$mode recovered library build failed" }
    $oracle = Join-Path $work 'W3xManagedOracle.exe'
    & $csc /nologo /platform:x86 $optimize /r:System.Core.dll /r:System.Xml.dll /r:"$core" /r:"$original" /r:"$library" /out:"$oracle" (Resolve-Path 'recovery/tests/W3xManagedOracle.cs').Path *> "$out/$mode-oracle-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-oracle-build.log"; throw "$mode oracle build failed" }
    $config = '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'
    $config | Set-Content "$oracle.config"
    & $oracle *> "$out/$mode-oracle.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-oracle.log"; throw "$mode managed differential failed" }
    Get-Content "$out/$mode-oracle.log"
    # A separate process/directory proves this metadata layer needs no original W3X assembly.
    $standalone = Join-Path $work 'without-original'
    New-Item -ItemType Directory $standalone | Out-Null
    Get-ChildItem $work -Filter '*.dll' | Where-Object Name -ne 'BinaryAssetBuilder.W3XCompiler.dll' | Copy-Item -Destination $standalone
    $hostExe = Join-Path $standalone 'W3xManagedStandalone.exe'
    & $csc /nologo /platform:x86 $optimize /r:System.Core.dll /r:"$core" /r:"$library" /out:"$hostExe" (Resolve-Path 'recovery/tests/W3xManagedStandalone.cs').Path *> "$out/$mode-standalone-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-standalone-build.log"; throw "$mode standalone build failed" }
    $config | Set-Content "$hostExe.config"
    & $hostExe *> "$out/$mode-standalone.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-standalone.log"; throw "$mode standalone test failed" }
    Get-Content "$out/$mode-standalone.log"
    $results += @{ mode = $mode; succeeded = $true; recoveredSha256 = (Get-FileHash $library -Algorithm SHA256).Hash.ToLowerInvariant() }
}
@{
    scope = 'managed plugin metadata, lifecycle and dispatch shell; no native handler or SDK acceptance'
    sourceCommit = $sourceCommit; runId = $env:GITHUB_RUN_ID; runAttempt = $env:GITHUB_RUN_ATTEMPT
    originalSha256 = $originalHash; modes = $results; platforms = @(0, 1, 2, 99, -1, [int]::MinValue, [int]::MaxValue)
    nativeHandlersValidated = $false; sdkValidated = $false
} | ConvertTo-Json -Depth 8 | Set-Content "$out/validation.json"
