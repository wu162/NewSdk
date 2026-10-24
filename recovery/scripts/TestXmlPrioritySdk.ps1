param([int]$TimeoutSeconds = 300)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$tempRoot = if ($env:RUNNER_TEMP) { $env:RUNNER_TEMP } else { [IO.Path]::GetTempPath() }
$work = Join-Path $tempRoot ('ra3-xml-priority-sdk-' + [Guid]::NewGuid().ToString('N'))
$build = Join-Path $work 'build'
$report = Join-Path $root 'recovery/evidence/xml-priority-sdk'
New-Item -ItemType Directory -Force $build, $report | Out-Null
$sourceCommit = if ($env:GITHUB_SHA) { $env:GITHUB_SHA } else { (& git rev-parse HEAD).Trim() }
$runId = if ($env:GITHUB_RUN_ID) { $env:GITHUB_RUN_ID } else { 'local-' + [DateTime]::UtcNow.ToString('yyyyMMddTHHmmssZ') }
$summary = [ordered]@{
    succeeded = $false; sourceCommit = $sourceCommit; runId = $runId; status = 'started'
    scope = '38 recovered XmlCompiler types; required probes pinned by the SDK oracle; same O2 compiler tested in Reduced and complete shipped mymod include profiles; not all XmlCompiler types'
    componentModes = @('/Od','/O2'); sdkMode = '/O2'; sdkPlatform = 0
    files = @(); profiles = @(); profileCount = 2; fullMymodGate = $false; originalDllUnchanged = $false
}
$originalHashes = @{}
foreach ($name in @('BinaryAssetBuilder.Tokenizer.dll','BinaryAssetBuilder.XmlCompiler.dll')) {
    $originalHashes[$name] = (Get-FileHash "SDK/tools/$name" -Algorithm SHA256).Hash
}
try {
    Copy-Item SDK/tools/*.dll $build
    $csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    $native = Join-Path $root 'recovery/src/Native'
    $common = @('Relocation','ScalarParsing','DataInitializers','ResourceMarshaling','EnumBitset','ObjectFilter')
    $sources = @($common | ForEach-Object { (Join-Path $native "$_.cpp") })
    $sources += @(Get-ChildItem "$native/Xml*.cpp" -File | Where-Object { $_.Name -notmatch 'Draft' } | Sort-Object Name | ForEach-Object { $_.FullName })
    $sources += @(Get-ChildItem 'recovery/src/Bridge/Xml*Compiler.cpp' -File | Where-Object { $_.Name -notmatch 'Draft' } | Sort-Object Name | ForEach-Object { $_.FullName })
    & cl /nologo /clr /LD /MD /EHa /std:c++17 /O2 /fp:precise "/AI$build" "/Fo$build/" "/Fe$build/Recovered.XmlCompilerPriority.dll" @sources *> "$report/build.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$report/build.txt"; throw 'Combined SDK compiler build failed' }
    & $csc /nologo /target:library /platform:x86 "/r:$build/BinaryAssetBuilder.Core.dll" "/r:$build/Recovered.XmlCompilerPriority.dll" "/out:$build/Recovered.XmlPriority.Plugin.dll" (Resolve-Path 'recovery/src/Plugin/XmlPriorityPlugin.cs').Path *> "$report/plugin-build.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$report/plugin-build.txt"; throw 'SDK plugin build failed' }
    & $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Web.Extensions.dll "/r:$build/BinaryAssetBuilder.Core.dll" "/out:$build/XmlPrioritySdkOracle.exe" (Resolve-Path 'recovery/tests/XmlPrioritySdkOracle.cs').Path *> "$report/oracle-build.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$report/oracle-build.txt"; throw 'SDK oracle build failed' }
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$build/XmlPrioritySdkOracle.exe.config"
    $probe = (Resolve-Path 'recovery/sdk/XmlPriorityAllProbe.xml').Path
    & "$build/XmlPrioritySdkOracle.exe" schema (Join-Path $root 'SDK') $probe "$report/schema.json" *> "$report/schema.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$report/schema.txt"; throw 'Full XSD validation or negative cases failed' }
    & "$build/XmlPrioritySdkOracle.exe" metadata "$build/BinaryAssetBuilder.XmlCompiler.dll" "$build/Recovered.XmlPriority.Plugin.dll" "$report/metadata.json" *> "$report/metadata.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$report/metadata.txt"; throw 'Original XmlCompiler metadata comparison failed' }
    foreach ($profile in @('Reduced','FullMymod')) {
        $profileReport = Join-Path $report $profile
        if (Test-Path $profileReport) { Remove-Item $profileReport -Recurse -Force }
        New-Item -ItemType Directory -Force $profileReport | Out-Null
        $profileSummary = [ordered]@{ name = $profile; succeeded = $false; fullMymod = ($profile -eq 'FullMymod') }
    $trace = Join-Path $profileReport 'trace'
    if (Test-Path $trace) { Remove-Item $trace -Recurse -Force }
    foreach ($mode in @('Original','Recovered')) {
        $sdk = Join-Path $work ($profile + '-' + $mode)
        Copy-Item (Join-Path $root 'SDK') $sdk -Recurse
        Remove-Item (Join-Path $sdk 'builtmods') -Recurse -Force -ErrorAction SilentlyContinue
        Get-ChildItem $sdk -Directory -Recurse | Where-Object { $_.Name -in @('cache','buildcache','sessioncache') } | Remove-Item -Recurse -Force
        Copy-Item $probe (Join-Path $sdk 'Mods/mymod/Data/RecoveryXmlPriorityProbe.xml') -Force
        $mod = Join-Path $sdk 'Mods/mymod/Data/Mod.xml'
        if ($profile -eq 'Reduced') {
            # Identical reduced input keeps shipped reference manifests and the complete probe.
            '<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Tags/><Includes><Include type="reference" source="DATA:static.xml"/><Include type="reference" source="DATA:global.xml"/><Include type="reference" source="DATA:audio.xml"/><Include type="all" source="RecoveryXmlPriorityProbe.xml"/></Includes></AssetDeclaration>' | Set-Content -Encoding UTF8 $mod
        } else {
            # Keep every shipped mymod include and all other Mod.xml content, then append the same probe.
            [xml]$fullMod = Get-Content -Raw $mod
            $manager = New-Object System.Xml.XmlNamespaceManager($fullMod.NameTable)
            $manager.AddNamespace('ea','uri:ea.com:eala:asset')
            $includes = $fullMod.SelectSingleNode('/ea:AssetDeclaration/ea:Includes',$manager)
            if ($null -eq $includes) { throw 'Shipped mymod has no Includes; full-profile preservation cannot be proved' }
            $originalIncludes = @($includes.ChildNodes | Where-Object { $_.NodeType -eq [Xml.XmlNodeType]::Element } | ForEach-Object { $_.OuterXml })
            if ($originalIncludes.Count -eq 0) { throw 'Shipped mymod include set is empty' }
            $append = $fullMod.CreateElement('Include','uri:ea.com:eala:asset')
            $append.SetAttribute('type','all'); $append.SetAttribute('source','RecoveryXmlPriorityProbe.xml')
            $includes.AppendChild($append) | Out-Null
            $retained = @($includes.ChildNodes | Where-Object { $_.NodeType -eq [Xml.XmlNodeType]::Element })
            if ($retained.Count -ne $originalIncludes.Count + 1) { throw 'Full mymod include count changed unexpectedly' }
            for ($i=0; $i -lt $originalIncludes.Count; ++$i) {
                if ($retained[$i].OuterXml -ne $originalIncludes[$i]) { throw 'A shipped mymod include was modified' }
            }
            $fullMod.Save($mod)
            $profileSummary['shippedIncludesPreserved'] = $originalIncludes.Count
        }
        & "$PSScriptRoot/SetXmlPriorityPlugin.ps1" -SdkRoot $sdk -PluginDirectory $build -Mode Recovered
        & "$PSScriptRoot/SetXmlPriorityPlugin.ps1" -SdkRoot $sdk -PluginDirectory $build -Mode Original
        & "$PSScriptRoot/SetXmlPriorityPlugin.ps1" -SdkRoot $sdk -PluginDirectory $build -Mode $mode
        & "$PSScriptRoot/SetXmlPriorityPlugin.ps1" -SdkRoot $sdk -PluginDirectory $build -Mode $mode
        $cfg = Join-Path $sdk 'tools/BinaryAssetBuilder.exe.config'
        [xml]$xml = Get-Content -Raw $cfg
        $expected = if ($mode -eq 'Recovered') { 'Recovered.XmlPriority.Plugin, Recovered.XmlPriority.Plugin' } else { 'BinaryAssetBuilder.XmlCompiler.Plugin, BinaryAssetBuilder.XmlCompiler' }
        foreach ($type in @('AttributeModifier','FXList','SpecialPowerTemplate','ObjectCreationList','LocomotorTemplate','LogicCommandSet','FXParticleSystemTemplate','UpgradeTemplate','AIPersonalityDefinition','AIStrategicStateDefinition','AIMicroManagerData','AIBudgetStateDefinition','AIStateDefinition','AITargetingHeuristic','LogicCommand','PlayerTech','PlayerTechRank','PlayerTechUpgradeBinding','ExperienceLevelTemplate','PlayerTechStoreTemplate','PurchasePlayerTechButtonTemplate','ButtonSingleStateData','Weather','WaterTransparency','WaterSetting','Environment','LightingSettings','WaterTextureList','TerrainParticleMapping','BounceKickTerrainMapping','AIData','UnitAISettings','PlayerTemplate','MultiplayerSettings','AIBuildStateDefinition','AIStrategicStateName','MultiplayerColor','AIPersonalityToAchievementEventMapping')) {
            $handlers = @($xml.configuration.assetbuilder.plugins.plugin | Where-Object { $_.assetTypes.Split(',') -contains $type })
            if ($handlers.Count -ne 1 -or $handlers[0].targetType -ne $expected -or $handlers[0].useBuildCache -ne 'false') { throw "$mode ambiguous or cached $type route" }
        }
        foreach ($startup in @($xml.configuration.SelectNodes('startup'))) { $xml.configuration.RemoveChild($startup) | Out-Null }
        $startup = $xml.CreateElement('startup'); $startup.SetAttribute('useLegacyV2RuntimeActivationPolicy','true')
        $runtime = $xml.CreateElement('supportedRuntime'); $runtime.SetAttribute('version','v4.0')
        $startup.AppendChild($runtime) | Out-Null; $xml.configuration.AppendChild($startup) | Out-Null; $xml.Save($cfg)
        Copy-Item $cfg "$profileReport/$mode.config"
        Copy-Item $mod "$profileReport/$mode-Mod.xml"
        $output = Join-Path $sdk 'builtmods'; New-Item -ItemType Directory -Force $output | Out-Null
        $env:RA3_RECOVERED_XML_PRIORITY_TRACE = if ($mode -eq 'Recovered') { $trace } else { $null }
        $arguments = @("`"$mod`"", "/od:`"$output`"", "/iod:`"$output`"", '/csc:false', '/ls:true', '/osh:false', '/pc:true', '/res:true', '/slowclean:true', '/ss:true', '/art:".;.\Mods\mymod\Art;.\Mods;.\Art"', '/audio:".;.\Mods\mymod\Audio;.\Mods;.\Audio"', '/data:".;.\Mods\mymod\Data;.\Mods;.\SageXml"')
        $p = Start-Process (Join-Path $sdk 'tools/BinaryAssetBuilder.exe') -ArgumentList ($arguments -join ' ') -WorkingDirectory $sdk -PassThru -RedirectStandardOutput "$profileReport/$mode-stdout.txt" -RedirectStandardError "$profileReport/$mode-stderr.txt"
        if (!$p.WaitForExit($TimeoutSeconds * 1000)) { Stop-Process -Id $p.Id -Force; throw "$mode SDK timed out" }
        $p.Refresh()
        if ($p.ExitCode -ne 0) { Get-Content "$profileReport/$mode-stdout.txt"; Get-Content "$profileReport/$mode-stderr.txt"; throw "$mode SDK failed: $($p.ExitCode)" }
        foreach ($ext in @('bin','manifest','imp','relo')) {
            $file = Join-Path $output "mods/mymod/data/mod.$ext"
            if (!(Test-Path $file)) { throw "$mode missing mod.$ext" }
            Copy-Item $file "$profileReport/$mode-mod.$ext"
            $summary.files += [ordered]@{ profile = $profile; mode = $mode; file = "mod.$ext"; bytes = (Get-Item $file).Length; sha256 = (Get-FileHash $file -Algorithm SHA256).Hash }
        }
    }
    $env:RA3_RECOVERED_XML_PRIORITY_TRACE = $null
    if ((Get-FileHash "$profileReport/Original-Mod.xml").Hash -ne (Get-FileHash "$profileReport/Recovered-Mod.xml").Hash) { throw "$profile original/recovered MOD inputs differ" }
    $profileSummary['inputSha256'] = (Get-FileHash "$profileReport/Original-Mod.xml" -Algorithm SHA256).Hash
    if ((Get-Item "$profileReport/Recovered-mod.imp").Length -le 4) { throw 'SDK probes did not exercise nonempty imports' }
    foreach ($ext in @('bin','manifest','imp','relo')) {
        if ((Get-FileHash "$profileReport/Original-mod.$ext").Hash -ne (Get-FileHash "$profileReport/Recovered-mod.$ext").Hash) { throw "SDK output differs: mod.$ext" }
    }
    & "$build/XmlPrioritySdkOracle.exe" replay "$build/BinaryAssetBuilder.XmlCompiler.dll" $trace "$profileReport/Recovered-mod.bin" $probe "$profileReport/replay.json" 0 $profile *> "$profileReport/replay.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$profileReport/replay.txt"; throw 'SDK original three-buffer replay failed' }
    $isolated = Join-Path $work ($profile + '-isolated'); New-Item -ItemType Directory -Force $isolated | Out-Null
    Get-ChildItem $build -File | Where-Object { $_.Extension -in @('.dll','.exe','.config') -and $_.Name -notin @('BinaryAssetBuilder.Tokenizer.dll','BinaryAssetBuilder.XmlCompiler.dll') } | Copy-Item -Destination $isolated
    & "$isolated/XmlPrioritySdkOracle.exe" isolated "$isolated/Recovered.XmlPriority.Plugin.dll" $trace "$profileReport/Recovered-mod.bin" $probe "$profileReport/isolated.json" 0 $profile *> "$profileReport/isolated.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$profileReport/isolated.txt"; throw 'Original-free SDK replay failed' }
    foreach ($name in @('replay','isolated')) {
        $result = Get-Content "$profileReport/$name.json" -Raw | ConvertFrom-Json
        if (!$result.succeeded) { throw "Missing successful $name evidence" }
    }
        $profileSummary.succeeded = $true
        $profileSummary['fourOutputsIdentical'] = $true
        $profileSummary['compilerSha256'] = (Get-FileHash "$build/Recovered.XmlCompilerPriority.dll" -Algorithm SHA256).Hash
        $profileSummary['pluginSha256'] = (Get-FileHash "$build/Recovered.XmlPriority.Plugin.dll" -Algorithm SHA256).Hash
        $profileSummary['originalAndIsolatedReplay'] = $true
        $profileSummary | ConvertTo-Json -Depth 12 | Set-Content "$profileReport/validation.json"
        $summary.profiles += $profileSummary
    }
    foreach ($name in @('schema','metadata')) {
        $result = Get-Content "$report/$name.json" -Raw | ConvertFrom-Json
        if (!$result.succeeded) { throw "Missing successful shared $name evidence" }
    }
    if ($summary.profiles.Count -ne 2 -or @($summary.profiles | Where-Object { !$_.succeeded }).Count -ne 0) { throw 'Both SDK profiles must complete' }
    $summary.fullMymodGate = $true
    $summary['probeSha256'] = (Get-FileHash $probe -Algorithm SHA256).Hash
    $summary['compilerSha256'] = (Get-FileHash "$build/Recovered.XmlCompilerPriority.dll" -Algorithm SHA256).Hash
    $summary['pluginSha256'] = (Get-FileHash "$build/Recovered.XmlPriority.Plugin.dll" -Algorithm SHA256).Hash
    $summary.succeeded = $true
    $summary.status = 'Full XSD and negatives; exact original metadata on four platforms; same compiled DLL/plugin: Reduced and FullMymod original/recovered Win32 SDK; all shipped mymod Includes preserved; four identical outputs per profile; every actual call replayed with and without original DLLs'
} catch {
    $summary.status = 'failed'; $summary['error'] = $_.ToString(); throw
} finally {
    $env:RA3_RECOVERED_XML_PRIORITY_TRACE = $null
    $summary.originalDllUnchanged = $true
    foreach ($name in $originalHashes.Keys) {
        if ((Get-FileHash "SDK/tools/$name" -Algorithm SHA256).Hash -ne $originalHashes[$name]) { $summary.originalDllUnchanged = $false; $summary.succeeded = $false }
    }
    $summary | ConvertTo-Json -Depth 12 | Set-Content "$report/validation.json"
}

