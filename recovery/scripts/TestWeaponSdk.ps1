param([int]$TimeoutSeconds = 300)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$work = Join-Path $env:RUNNER_TEMP 'ra3-weapon-sdk'
if (Test-Path $work) { Remove-Item $work -Recurse -Force }
$build = Join-Path $env:RUNNER_TEMP 'ra3-recovered-weapon'
$parentReport = Join-Path $root 'recovery/evidence/weapon'
$report = Join-Path $parentReport 'formal-sdk'
New-Item -ItemType Directory -Force $build, $report | Out-Null
@{ succeeded = $false; sourceCommit = $env:GITHUB_SHA; status = 'started' } | ConvertTo-Json | Set-Content "$report/validation.json"
$component = Get-Content "$parentReport/validation.json" -Raw | ConvertFrom-Json
if (!$component.succeeded -or $component.sourceCommit -ne $env:GITHUB_SHA) { throw 'Current full component matrix must pass first' }
$component.succeeded = $false
$component | Add-Member -Force NoteProperty sdkPluginSucceeded $false
$component | ConvertTo-Json -Depth 8 | Set-Content "$parentReport/validation.json"
Copy-Item "$report/validation.json" "$parentReport/formal-sdk.json" -Force
$probe = Join-Path $root 'SDK/Mods/mymod/Data/RecoveryWeaponNuggetProbe.xml'

$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
& $csc /nologo /platform:x86 /r:"$build/Recovered.Weapon.dll" /r:"$build/BinaryAssetBuilder.Core.dll" /out:"$build/WeaponSdkProbeOracle.exe" (Resolve-Path 'recovery/tests/WeaponSdkProbeOracle.cs') *> "$report/schema-host-build.txt"
if ($LASTEXITCODE -ne 0) { Get-Content "$report/schema-host-build.txt"; throw 'Schema host build failed' }
& $csc /nologo /platform:x86 /r:System.Core.dll /r:"$build/BinaryAssetBuilder.Core.dll" /out:"$build/WeaponSdkOracle.exe" (Resolve-Path 'recovery/tests/WeaponSdkOracle.cs') *> "$report/sdk-oracle-build.txt"
if ($LASTEXITCODE -ne 0) { Get-Content "$report/sdk-oracle-build.txt"; throw 'SDK oracle build failed' }
Copy-Item "$build/WeaponOracle.exe.config" "$build/WeaponSdkOracle.exe.config"
Copy-Item "$build/WeaponOracle.exe.config" "$build/WeaponSdkProbeOracle.exe.config"
& "$build/WeaponSdkProbeOracle.exe" schema (Join-Path $root 'SDK') *> "$report/schema-output.txt"
if ($LASTEXITCODE -ne 0) { Get-Content "$report/schema-output.txt"; throw 'Complete XSD validation failed' }
$trace = Join-Path $report 'trace'
if (Test-Path $trace) { Remove-Item $trace -Recurse -Force }
$results = @()
foreach ($mode in @('Original','Recovered')) {
    $sdk = Join-Path $work $mode
    Copy-Item (Join-Path $root 'SDK') $sdk -Recurse
    Remove-Item (Join-Path $sdk 'builtmods') -Recurse -Force -ErrorAction SilentlyContinue
    # Remove derived caches only in the disposable SDK. Keep shipped reference manifests.
    Get-ChildItem $sdk -Directory -Recurse | Where-Object { $_.Name -in @('cache','buildcache','sessioncache') } | Remove-Item -Recurse -Force
    $output = Join-Path $sdk 'builtmods'
    New-Item -ItemType Directory -Force $output | Out-Null
    # Exercise reversibility and idempotence before the actual build.
    & "$PSScriptRoot/SetWeaponPlugin.ps1" -SdkRoot $sdk -PluginDirectory $build -Mode Recovered
    & "$PSScriptRoot/SetWeaponPlugin.ps1" -SdkRoot $sdk -PluginDirectory $build -Mode Original
    & "$PSScriptRoot/SetWeaponPlugin.ps1" -SdkRoot $sdk -PluginDirectory $build -Mode $mode
    & "$PSScriptRoot/SetWeaponPlugin.ps1" -SdkRoot $sdk -PluginDirectory $build -Mode $mode
    $cfg = Join-Path $sdk 'tools/BinaryAssetBuilder.exe.config'
    [xml]$xml = Get-Content -Raw $cfg
    $handlers = @($xml.configuration.assetbuilder.plugins.plugin | Where-Object { $_.assetTypes.Split(',') -contains 'WeaponTemplate' })
    $expectedHandler = if ($mode -eq 'Recovered') { 'Recovered.Weapon.Plugin, Recovered.WeaponPlugin' } else { 'BinaryAssetBuilder.Tokenizer.Plugin, BinaryAssetBuilder.Tokenizer' }
    if ($handlers.Count -ne 1 -or $handlers[0].targetType -ne $expectedHandler) { throw "$mode Weapon routing is ambiguous or incorrect" }
    if ($mode -eq 'Recovered' -and $handlers[0].useBuildCache -ne 'false') { throw 'Recovered build cache must be disabled' }
    "PASS reversible/idempotent $mode routing: $expectedHandler" | Set-Content "$report/$mode-routing.txt"
    $startup = $xml.CreateElement('startup'); $startup.SetAttribute('useLegacyV2RuntimeActivationPolicy','true')
    $runtime = $xml.CreateElement('supportedRuntime'); $runtime.SetAttribute('version','v4.0')
    $startup.AppendChild($runtime) | Out-Null; $xml.configuration.AppendChild($startup) | Out-Null; $xml.Save($cfg)
    Copy-Item $cfg "$report/$mode.config"
    $env:RA3_RECOVERED_WEAPON_TRACE = if ($mode -eq 'Recovered') { $trace } else { $null }
    $mod = Join-Path $sdk 'Mods/mymod/Data/Mod.xml'
    $arguments = @("`"$mod`"", "/od:`"$output`"", "/iod:`"$output`"", '/csc:false', '/ls:true', '/osh:false', '/pc:true', '/res:true', '/slowclean:true', '/ss:true', '/art:".;.\Mods\mymod\Art;.\Mods;.\Art"', '/audio:".;.\Mods\mymod\Audio;.\Mods;.\Audio"', '/data:".;.\Mods\mymod\Data;.\Mods;.\SageXml"')
    $p = Start-Process (Join-Path $sdk 'tools/BinaryAssetBuilder.exe') -ArgumentList ($arguments -join ' ') -WorkingDirectory $sdk -PassThru -RedirectStandardOutput "$report/$mode-stdout.txt" -RedirectStandardError "$report/$mode-stderr.txt"
    $finished = $p.WaitForExit($TimeoutSeconds * 1000)
    if (!$finished) { Stop-Process -Id $p.Id -Force; throw "$mode timed out" }
    $p.Refresh()
    if ($p.ExitCode -ne 0) { Get-Content "$report/$mode-stdout.txt"; Get-Content "$report/$mode-stderr.txt"; throw "$mode SDK build failed: $($p.ExitCode)" }
    foreach ($ext in @('bin','manifest','imp','relo')) {
        $file = Join-Path $output "mods/mymod/data/mod.$ext"
        if (!(Test-Path $file)) { throw "$mode missing mod.$ext" }
        Copy-Item $file "$report/$mode-mod.$ext"
        $results += @{ mode = $mode; file = "mod.$ext"; bytes = (Get-Item $file).Length; sha256 = (Get-FileHash $file -Algorithm SHA256).Hash }
    }
}
$env:RA3_RECOVERED_WEAPON_TRACE = $null
$results | ConvertTo-Json | Set-Content "$report/files.json"
foreach ($ext in @('bin','manifest','imp','relo')) {
    if ((Get-FileHash "$report/Original-mod.$ext").Hash -ne (Get-FileHash "$report/Recovered-mod.$ext").Hash) { throw "SDK output differs: mod.$ext" }
}
& "$build/WeaponSdkOracle.exe" buffers "$build/BinaryAssetBuilder.Tokenizer.dll" $trace "$report/Recovered-mod.bin" $probe *> "$report/sdk-buffer-oracle.txt"
if ($LASTEXITCODE -ne 0) { Get-Content "$report/sdk-buffer-oracle.txt"; throw 'SDK buffer/routing verification failed' }
Get-Content "$report/sdk-buffer-oracle.txt"
# Prove the recovered plugin can replay the verified SDK buffers with both
# original compiler DLLs physically absent from the process directory.
$isolated = Join-Path $work 'isolated'
New-Item -ItemType Directory -Force $isolated | Out-Null
Get-ChildItem $build -File | Where-Object { $_.Extension -in @('.dll','.exe','.config') -and $_.Name -notin @('BinaryAssetBuilder.Tokenizer.dll','BinaryAssetBuilder.XmlCompiler.dll') } | Copy-Item -Destination $isolated
& "$isolated/WeaponSdkOracle.exe" isolated "$isolated/Recovered.WeaponPlugin.dll" $trace "$report/Recovered-mod.bin" $probe *> "$report/isolated-oracle.txt"
if ($LASTEXITCODE -ne 0) { Get-Content "$report/isolated-oracle.txt"; throw 'Original-free plugin replay failed' }
Get-Content "$report/isolated-oracle.txt"
@{ succeeded = $true; sourceCommit = $env:GITHUB_SHA; runId = $env:GITHUB_RUN_ID; status = 'Formal Weapon plugin Od/O2 differentials; XSD; original/recovered fresh SDK builds; all four outputs byte-identical; traced actual buffers verified; original-free replay; three XSD negative cases; reversible/idempotent routing; 38 probes cover 35 branches, all-35 mixed list and inherited all-35 list'; scope = 'WeaponTemplate only; original plugins retained for other types; SDK Win32 integration, component /Od /O2 on four targets' } | ConvertTo-Json | Set-Content "$report/validation.json"


Copy-Item "$report/validation.json" "$parentReport/formal-sdk.json" -Force
Compress-Archive -Path "$report/*" -DestinationPath "$parentReport/formal-sdk-output.zip" -Force
$component.succeeded = $true
$component.sdkPluginSucceeded = $true
$component | Add-Member -Force NoteProperty sdkPluginSourceCommit $env:GITHUB_SHA
$component | Add-Member -Force NoteProperty sdkPluginScope 'Formal Weapon SDK routing; all four complete outputs byte-identical; actual three-buffer replay and original-free replay; all 35 Nugget branches and inheritance'
$component | ConvertTo-Json -Depth 8 | Set-Content "$parentReport/validation.json"
