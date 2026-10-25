param([switch]$OriginalOnly)
$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if (!$env:GITHUB_RUN_ID -or !$env:GITHUB_RUN_ATTEMPT) { throw 'Input metadata batch requires an identified Actions run' }
$batch = "$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$pluginDirectory = "recovery/evidence/audio/plugin-validation/$batch"
# Original formats are run first and must all return complete buffers/custom
# data. Only then does the unchanged differential harness build both candidates.
if ($OriginalOnly) {
    & "$PSScriptRoot/TestAudioPlugin.ps1" -OriginalOnly -Groups formats
} else {
    & "$PSScriptRoot/TestAudioPlugin.ps1" -Groups formats
}
if (@(Get-Content "$pluginDirectory/formats-original.txt").Count -ne 2400) {
    throw 'Expected 480 existing and 1920 ignored-extension original plugin records'
}
if ($OriginalOnly) { return }
$receipt = Get-Content "$pluginDirectory/validation.json" -Raw | ConvertFrom-Json
if ($receipt.status -ne 'passed' -or @($receipt.configurations).Count -ne 2) { throw 'Missing Debug/Release input metadata differential' }
foreach ($mode in @('Debug','Release')) {
    $configuration = @($receipt.configurations | Where-Object { $_.configuration -eq $mode -and $_.group -eq 'formats' })
    if ($configuration.Count -ne 1 -or !$configuration[0].passed -or $configuration[0].records -ne 2400) {
        throw "Incomplete $mode input metadata matrix"
    }
    if (!(Select-String -Path "$pluginDirectory/$mode-reader-boundaries.log" -SimpleMatch 'PASS Audio ignored-extension sample invariants: 120')) {
        throw "Missing $mode decoded sample-bit invariants"
    }
}
& "$PSScriptRoot/TestAudioSdk.ps1" -InputMetadataOnly
$sdkDirectory = "recovery/evidence/audio/sdk-validation/$batch"
$sdk = Get-Content "$sdkDirectory/validation.json" -Raw | ConvertFrom-Json
if ($sdk.status -ne 'passed' -or !$sdk.succeeded -or $sdk.profile -ne 'create-input-metadata') { throw 'Input metadata SDK validation missing' }
if (@(Get-Content "$sdkDirectory/expected-assets.tsv").Count -ne 120) { throw 'Expected all 120 SDK metadata assets' }
foreach ($side in @('Original','Recovered')) {
    if (@(Get-Content "$sdkDirectory/$side-calls.tsv").Count -ne 120) { throw "Expected all 120 actual $side SDK calls" }
}
Write-Host 'PASS WAVE ignored extension batch: Debug/Release 2400 exact plugin records, 120 sample-bit invariants, SDK 120 calls/four products/120 CustomData'
