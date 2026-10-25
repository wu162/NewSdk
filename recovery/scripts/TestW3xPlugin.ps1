param(
    [Parameter(Mandatory=$true)][string]$CaptureDirectory,
    [string]$OutputDirectory
)
$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
$captures = (Resolve-Path $CaptureDirectory).Path
if (!$OutputDirectory) {
    $suffix = if ($env:GITHUB_RUN_ID) { "$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT" } else { 'local-' + [Guid]::NewGuid().ToString('N') }
    $OutputDirectory = "recovery/evidence/w3x/plugin-runs/$suffix"
}
if (Test-Path $OutputDirectory) { throw "Refusing to overwrite plugin validation evidence: $OutputDirectory" }
New-Item -ItemType Directory $OutputDirectory -Force | Out-Null
$out = (Resolve-Path $OutputDirectory).Path
$sourceCommit = (git rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0) { throw 'Cannot determine plugin source commit' }
$original = (Resolve-Path 'SDK/tools/BinaryAssetBuilder.W3XCompiler.dll').Path
$originalHash = (Get-FileHash $original -Algorithm SHA256).Hash.ToLowerInvariant()
if ($originalHash -ne 'bff82309b8e5e9f689112202e3613ad335148934c92dc70fe143f6e60d9ad3f1') { throw 'Original W3X compiler fingerprint changed' }
# Accept only captures produced by the current source's successful complete
# component gate. Plugin validation must not silently consume stale evidence.
$componentValidation = Get-Content (Join-Path $captures 'validation.json') -Raw | ConvertFrom-Json
if (!$componentValidation.succeeded -or $componentValidation.sourceCommit -ne $sourceCommit -or
    !$componentValidation.completeAnimationBuffersValidated -or !$componentValidation.completeMeshBuffersValidated -or
    !$componentValidation.componentWithoutOriginalReplay) { throw 'Current-source complete component validation is required before plugin validation' }
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$config = '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'
$results = @()
$failures = [Collections.Generic.List[string]]::new()
foreach ($mode in @('Od','O2')) {
    $success = $true
    try {
        $animationCapture = Join-Path $captures "$mode-animation.fixtures"
        $meshCapture = Join-Path $captures "$mode-mesh.fixtures"
        foreach ($path in @($animationCapture,$meshCapture)) { if (!(Test-Path $path)) { throw "Missing full component capture: $path" } }
        $work = Join-Path $out $mode
        & "$PSScriptRoot/BuildW3xPlugin.ps1" -OutputDirectory $work -Mode $mode
        if (!(Test-Path "$work/Recovered.W3x.Plugin.dll")) { throw 'Plugin build did not produce its managed assembly' }
        $oracle = Join-Path $work 'W3xPluginOracle.exe'
        $optimize = if ($mode -eq 'O2') { '/optimize+' } else { '/optimize-' }
        & $csc /nologo /platform:x86 $optimize /r:System.Xml.dll /r:"$work/BinaryAssetBuilder.Core.dll" /r:"$work/Recovered.W3x.Plugin.dll" /out:"$oracle" (Resolve-Path 'recovery/tests/W3xPluginOracle.cs').Path *> "$out/$mode-oracle-build.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-oracle-build.log"; throw 'Plugin oracle build failed' }
        $config | Set-Content "$oracle.config"
        & $oracle original $animationCapture $meshCapture $original *> "$out/$mode-original.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-original.log" | Select-Object -Last 50; throw 'Loadable plugin ProcessInstance original three-buffer differential failed' }
        Get-Content "$out/$mode-original.log" | Where-Object { $_ -match '^PASS ' }
        # Separate process: original is never copied into this deployment and
        # the oracle guards both assembly-load events and the loaded inventory.
        & $oracle standalone $animationCapture $meshCapture *> "$out/$mode-standalone.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-standalone.log" | Select-Object -Last 50; throw 'Loadable plugin ProcessInstance replay without original failed' }
        Get-Content "$out/$mode-standalone.log" | Where-Object { $_ -match '^PASS ' }
        $results += @{
            mode=$mode; succeeded=$true; processInstanceOriginalDifferential=$true; withoutOriginalReplay=$true
            animationFixtures=675; meshFixtures=666
            animationCaptureSha256=(Get-FileHash $animationCapture -Algorithm SHA256).Hash.ToLowerInvariant()
            meshCaptureSha256=(Get-FileHash $meshCapture -Algorithm SHA256).Hash.ToLowerInvariant()
            pluginSha256=(Get-FileHash "$work/Recovered.W3x.Plugin.dll" -Algorithm SHA256).Hash.ToLowerInvariant()
            pluginDirectory=$work
        }
    } catch {
        $message = "$mode plugin validation failed: $($_.Exception.Message)"
        $failures.Add($message); Write-Host $message
        $results += @{ mode=$mode; succeeded=$false; processInstanceOriginalDifferential=$false; withoutOriginalReplay=$false; error=$message }
    }
}
$succeeded = $failures.Count -eq 0 -and $results.Count -eq 2
@{
    succeeded=$succeeded; sourceCommit=$sourceCommit; runId=$env:GITHUB_RUN_ID; runAttempt=$env:GITHUB_RUN_ATTEMPT
    pluginType='Recovered.Compiler.W3x.NativePlugin, Recovered.W3x.Plugin'
    originalSha256=$originalHash; componentValidation=(Join-Path $captures 'validation.json')
    modes=$results; failures=$failures.ToArray()
    processInstanceOriginalDifferential=$succeeded; withoutOriginalReplay=$succeeded; sdkValidated=$false
    scope='loadable recovered plugin configuration identity/default constructor, both ProcessInstance backends and public handles, original three-buffer differential, standalone captured fixture replay without original W3X compiler; SDK not run'
} | ConvertTo-Json -Depth 8 | Set-Content "$out/plugin-validation.json"
if (!$succeeded) { throw "Recovered W3X plugin validation failed in $($failures.Count) modes" }
