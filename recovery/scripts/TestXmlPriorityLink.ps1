param()
$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
$repo = (Get-Location).Path
$evidence = Join-Path $repo 'recovery/evidence/xml-priority-link'
$tempRoot = if ($env:RUNNER_TEMP) { $env:RUNNER_TEMP } else { [IO.Path]::GetTempPath() }
$out = Join-Path $tempRoot 'ra3-recovered-xml-priority-link'
New-Item -ItemType Directory -Force $evidence, $out | Out-Null
# This is a combined-TU/CLI metadata/link smoke, not the six groups' full matrices.
$expectedRootTypeIds = [ordered]@{
    AttributeModifier = [uint32]3319822471
    FXList = [uint32]2254974584
    SpecialPowerTemplate = [uint32]2178440954
    ObjectCreationList = [uint32]3899542881
    LocomotorTemplate = [uint32]3972178387
    LogicCommandSet = [uint32]3959844197
    FXParticleSystemTemplate = [uint32]686292351
    UpgradeTemplate = [uint32]3786401627
    AIPersonalityDefinition = [uint32]3604279694
    AIStrategicStateDefinition = [uint32]607123156
    AIMicroManagerData = [uint32]2423780215
    AIBudgetStateDefinition = [uint32]2893598307
    AIStateDefinition = [uint32]3239935619
    AITargetingHeuristic = [uint32]926814458
}
$sourceCommit = $env:GITHUB_SHA
if (-not $sourceCommit) { $sourceCommit = (& git rev-parse HEAD).Trim(); if ($LASTEXITCODE -ne 0) { throw 'Cannot resolve sourceCommit' } }
$runId = $env:GITHUB_RUN_ID
if (-not $runId) { $runId = 'local-' + [DateTime]::UtcNow.ToString('yyyyMMddTHHmmssZ') }
$validation = [ordered]@{
    succeeded = $false; sourceCommit = $sourceCommit; runId = $runId
    scope = 'Combined XmlCompiler priority link/default-root smoke only; group full matrices and SDK acceptance remain separate'
    expectedRootTypeIds = $expectedRootTypeIds; expectedTypes = 14
    expectedFixturesPerMode = 56; expectedComparisonsPerMode = 168
    platforms = @(0,1,2,99); modes = @(); processExits = @(); status = 'started'
}
$validation | ConvertTo-Json -Depth 12 | Set-Content "$evidence/validation.json"
$originalPath = (Resolve-Path 'SDK/tools/BinaryAssetBuilder.XmlCompiler.dll').Path
$originalHash = (Get-FileHash $originalPath -Algorithm SHA256).Hash.ToLowerInvariant()
$validation['originalDllSha256'] = $originalHash
try {
    if ($env:VSCMD_ARG_TGT_ARCH -and $env:VSCMD_ARG_TGT_ARCH -ne 'x86') { throw 'Run in an x86 VS2022 devshell' }
    if ($expectedRootTypeIds.Count -ne 14) { throw 'Expected root map must contain all 14 priority types' }
    if ($originalHash -ne 'd4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6') { throw 'Unexpected original XmlCompiler DLL fingerprint' }
    $native = Join-Path $repo 'recovery/src/Native'
    $bridge = Join-Path $repo 'recovery/src/Bridge'
    $commonNames = @('Relocation','ScalarParsing','DataInitializers','ResourceMarshaling','EnumBitset','ObjectFilter')
    $common = @($commonNames | ForEach-Object { (Resolve-Path (Join-Path $native "$_.cpp")).Path })
    $xmlNative = @(Get-ChildItem -LiteralPath $native -Filter 'Xml*.cpp' -File)
    $xmlBridge = @(Get-ChildItem -LiteralPath $bridge -Filter 'Xml*Compiler.cpp' -File)
    $candidates = @($xmlNative) + @($xmlBridge)
    $excluded = @($candidates | Where-Object { $_.Name -match 'Draft' })
    $formal = @($candidates | Where-Object { $_.Name -notmatch 'Draft' } | Sort-Object FullName)
    if ($formal.Count -eq 0) { throw 'No formal Xml sources found' }
    # A #if 0 snapshot does not satisfy the 14-class reflection preflight below.
    $sources = @($common) + @($formal | ForEach-Object { $_.FullName })
    $sources = @($sources | Select-Object -Unique)
    $validation['sources'] = @($sources | ForEach-Object {
        [ordered]@{ path = $_.Substring($repo.Length + 1).Replace('\','/'); sha256 = (Get-FileHash $_ -Algorithm SHA256).Hash.ToLowerInvariant() }
    })
    $validation['excludedDraftSources'] = @($excluded | ForEach-Object { $_.FullName.Substring($repo.Length + 1).Replace('\','/') })
    $validation | ConvertTo-Json -Depth 12 | Set-Content "$evidence/validation.json"
    $manifestPath = Join-Path $out 'expected-root-types.tsv'
    @($expectedRootTypeIds.GetEnumerator() | ForEach-Object { "$($_.Key)`t$($_.Value)" }) | Set-Content -Encoding ASCII $manifestPath
    Copy-Item $manifestPath "$evidence/expected-root-types.tsv" -Force
    $csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    $oracleSource = (Resolve-Path 'recovery/tests/XmlPriorityLinkOracle.cs').Path
    foreach ($mode in @('Od','O2')) {
        $modeOut = Join-Path $out $mode
        if (Test-Path $modeOut) { Remove-Item $modeOut -Recurse -Force }
        New-Item -ItemType Directory -Force $modeOut | Out-Null
        Copy-Item SDK/tools/*.dll $modeOut -Force
        & cl /nologo /clr /LD /MD /EHa "/$mode" /fp:precise "/AI$modeOut" "/Fo$modeOut/" "/Fe$modeOut/Recovered.XmlCompilerPriority.dll" @sources *> "$evidence/$mode-build.txt"
        if ($LASTEXITCODE -ne 0) { Get-Content "$evidence/$mode-build.txt"; throw "$mode combined XmlCompiler C++/CLI link failed" }
        & $csc /nologo /platform:x86 /r:System.Core.dll "/r:$modeOut/BinaryAssetBuilder.Core.dll" "/out:$modeOut/XmlPriorityLinkOracle.exe" $oracleSource *> "$evidence/$mode-host-build.txt"
        if ($LASTEXITCODE -ne 0) { Get-Content "$evidence/$mode-host-build.txt"; throw "$mode priority link oracle build failed" }
        '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$modeOut/XmlPriorityLinkOracle.exe.config"
        $dllPath = Join-Path $modeOut 'Recovered.XmlCompilerPriority.dll'
        # Preserve each linked DLL even if the later oracle rejects an absent class.
        $artifactDirectory = Join-Path $evidence $mode
        New-Item -ItemType Directory -Force $artifactDirectory | Out-Null
        Copy-Item $dllPath (Join-Path $artifactDirectory 'Recovered.XmlCompilerPriority.dll') -Force
        & "$modeOut/XmlPriorityLinkOracle.exe" $originalPath $dllPath "$evidence/$mode-result.json" $manifestPath $sourceCommit $runId *> "$evidence/$mode-oracle.txt"
        $code = $LASTEXITCODE
        # Preserve the real child-process status before any later command or parsing.
        $exitCodeHex = '0x' + [BitConverter]::ToUInt32([BitConverter]::GetBytes([int32]$code), 0).ToString('X8')
        $exitRecord = [ordered]@{
            mode = $mode; exitCode = [int32]$code; exitCodeHex = $exitCodeHex
            sourceCommit = $sourceCommit; runId = $runId
            processSucceeded = ($code -eq 0)
        }
        $validation.processExits += $exitRecord
        $exitRecord | ConvertTo-Json -Depth 4 | Set-Content "$evidence/$mode-exit.json"
        "PROCESS EXIT mode=$mode exitCode=$code exitCodeHex=$exitCodeHex" | Add-Content "$evidence/$mode-oracle.txt"
        $validation | ConvertTo-Json -Depth 12 | Set-Content "$evidence/validation.json"
        Get-Content "$evidence/$mode-oracle.txt"
        if ($code -ne 0) { throw "$mode combined priority default-root smoke failed: exitCode=$code ($exitCodeHex); all 14 types and a clean process exit are required" }
        $result = Get-Content "$evidence/$mode-result.json" -Raw | ConvertFrom-Json
        if (-not $result.succeeded -or $result.sourceCommit -ne $sourceCommit -or $result.runId -ne $runId -or $result.types -ne 14 -or $result.fixtures -ne 56 -or $result.comparisons -ne 168 -or ($result.platforms -join ',') -ne '0,1,2,99') { throw "$mode incomplete or mismatched priority smoke result" }
        if (@($result.typeCoverage.PSObject.Properties).Count -ne 14) { throw "$mode incomplete per-type coverage" }
        foreach ($entry in $expectedRootTypeIds.GetEnumerator()) {
            if ($result.typeCoverage.($entry.Key) -ne 4 -or $result.rootTypeIds.($entry.Key) -ne $entry.Value) { throw "$mode missing type coverage or incorrect TypeId: $($entry.Key)" }
        }
        $validation.modes += [ordered]@{
            mode = $mode; succeeded = $true; exitCode = [int32]$code; exitCodeHex = $exitCodeHex; types = $result.types
            fixtures = $result.fixtures; comparisons = $result.comparisons
            platforms = $result.platforms; typeCoverage = $result.typeCoverage
            compilerClasses = $result.compilerClasses
            linkedAssembly = "$mode/Recovered.XmlCompilerPriority.dll"
            linkedAssemblySha256 = (Get-FileHash $dllPath -Algorithm SHA256).Hash.ToLowerInvariant()
            log = "$mode-oracle.txt"
        }
        $validation | ConvertTo-Json -Depth 12 | Set-Content "$evidence/validation.json"
    }
    $validation.succeeded = $true
    $validation.status = 'All 14 exact compiler routes linked and passed default-root three-array smoke on Od/O2, platforms 0/1/2/99; not full group or SDK acceptance'
} catch {
    $validation.status = $_.Exception.Message
    throw
} finally {
    $afterHash = (Get-FileHash $originalPath -Algorithm SHA256).Hash.ToLowerInvariant()
    $validation['originalDllUnchanged'] = ($afterHash -eq $originalHash)
    if ($afterHash -ne $originalHash) { $validation.succeeded = $false; $validation.status = 'Original XmlCompiler DLL changed during smoke' }
    $validation | ConvertTo-Json -Depth 12 | Set-Content "$evidence/validation.json"
    if ($afterHash -ne $originalHash) { throw 'Original XmlCompiler DLL changed during smoke' }
}
