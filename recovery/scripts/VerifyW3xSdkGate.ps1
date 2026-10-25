param([string]$GateFile = 'recovery/w3x/SDK_READY.json', [string]$ReportFile)
$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if (!(Test-Path $GateFile)) { throw 'SDK is blocked: explicit W3X closure gate does not exist' }
$gate = Get-Content $GateFile -Raw | ConvertFrom-Json
if ($gate.ready -isnot [bool] -or !$gate.ready) { throw 'SDK is blocked: closure ready must be boolean true' }
if ($gate.unrecoveredDefinedBehaviorCount -ne 0 -or $null -eq $gate.PSObject.Properties['unrecoveredDefinedBehaviorCount']) { throw 'SDK is blocked: defined behavior remains unrecovered' }
$types = @($gate.supportedTypes)
if ($types.Count -ne 2 -or !($types -ccontains 'W3DAnimation') -or !($types -ccontains 'W3DMesh')) { throw 'SDK is blocked: closure must cover exactly Animation and Mesh' }
if ($gate.validatedSourceCommit -notmatch '^[0-9a-fA-F]{40}$' -or !$gate.validatedRunId -or !$gate.validatedRunAttempt) { throw 'SDK is blocked: missing prior validation provenance' }
if ($null -eq $gate.PSObject.Properties['explicitUndefinedBehaviorExclusions']) { throw 'SDK is blocked: missing explicit undefined-behavior boundary' }
$head = (git rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0) { throw 'Cannot identify source commit' }
$closureFile='recovery/w3x/W3xTypeClosure.json'
if (!(Test-Path $closureFile)) { throw 'SDK is blocked: type closure audit does not exist' }
# Git's Windows checkout may expand LF to CRLF. Bind the audit's UTF-8/LF
# content rather than the checkout line-ending representation.
$closureText=[IO.File]::ReadAllText((Resolve-Path $closureFile).Path).Replace("`r`n","`n")
$closureHasher=[Security.Cryptography.SHA256]::Create()
try { $closureHash=[BitConverter]::ToString($closureHasher.ComputeHash([Text.Encoding]::UTF8.GetBytes($closureText))).Replace('-','').ToLowerInvariant() }
finally { $closureHasher.Dispose() }
if ($gate.closureSha256 -ne $closureHash) { throw 'SDK is blocked: type closure audit changed' }
$closure=Get-Content $closureFile -Raw | ConvertFrom-Json
if (!$closure.sdkReady -or !$closure.allNamedXsdTypesAccountedFor -or !$closure.noOtherUnmappedDefinedBusinessFamilyFound) { throw 'SDK is blocked: type closure audit is not ready' }
$originalHash=(Get-FileHash 'SDK/tools/BinaryAssetBuilder.W3XCompiler.dll' -Algorithm SHA256).Hash.ToLowerInvariant()
if ($closure.original.sha256 -ne $originalHash) { throw 'SDK is blocked: type closure original DLL fingerprint differs' }
$paths = @{ srcTreeHash='recovery/src'; testsTreeHash='recovery/tests'; modsTreeHash='SDK/Mods'; scriptsTreeHash='recovery/scripts'; workflowsTreeHash='.github/workflows'; sdkToolsTreeHash='SDK/tools'; schemasTreeHash='SDK/Schemas' }
$checked = @{}
foreach ($field in $paths.Keys) {
    $path = $paths[$field]
    $current = (git rev-parse "HEAD:$path").Trim()
    if ($LASTEXITCODE -ne 0) { throw "Cannot identify current $path tree" }
    $validated = (git rev-parse "$($gate.validatedSourceCommit):$path").Trim()
    if ($LASTEXITCODE -ne 0) { throw "Cannot identify previously validated $path tree; full history is required" }
    if ($gate.$field -ne $current -or $gate.$field -ne $validated) { throw "SDK is blocked: $field differs from current or validated source" }
    $checked[$field] = $current
}
$result = @{ succeeded=$true; sourceCommit=$head; validatedSourceCommit=$gate.validatedSourceCommit; validatedRunId=$gate.validatedRunId; validatedRunAttempt=$gate.validatedRunAttempt; trees=$checked; gateSha256=(Get-FileHash $GateFile -Algorithm SHA256).Hash.ToLowerInvariant(); closureSha256=$closureHash; originalSha256=$originalHash; sdkStarted=$false }
if ($ReportFile) { $result | ConvertTo-Json -Depth 6 | Set-Content $ReportFile }
Write-Host "PASS W3X explicit closure gate for $head; SDK has not been started"
