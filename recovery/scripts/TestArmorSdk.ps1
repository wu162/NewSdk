param([int]$TimeoutSeconds = 300)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$work = Join-Path $env:RUNNER_TEMP 'ra3-armor-sdk'
if (Test-Path $work) { Remove-Item $work -Recurse -Force }
$build = Join-Path $work 'build'
$report = Join-Path $root 'recovery/evidence/armor-sdk'
New-Item -ItemType Directory -Force $build, $report | Out-Null
@{ succeeded = $false; sourceCommit = $env:GITHUB_SHA; status = 'started' } | ConvertTo-Json | Set-Content "$report/validation.json"
Copy-Item SDK/tools/*.dll $build
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
foreach ($optimization in @('Od','O2')) {
    cl /nologo /clr /LD /MD /EHa "/$optimization" /fp:precise /AI"$build" /Fo"$build/" /Fe"$build/Recovered.Armor.dll" recovery/src/Native/Relocation.cpp recovery/src/Native/ScalarParsing.cpp recovery/src/Native/ArmorTemplate.cpp recovery/src/Bridge/ArmorCompiler.cpp *> "$report/$optimization-build.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$report/$optimization-build.txt"; throw 'Component build failed' }
    & $csc /nologo /target:library /platform:x86 /r:"$build/BinaryAssetBuilder.Core.dll" /r:"$build/Recovered.Armor.dll" /out:"$build/Recovered.Compiler.dll" (Resolve-Path 'recovery/src/Plugin/ArmorPlugin.cs') *> "$report/$optimization-plugin-build.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$report/$optimization-plugin-build.txt"; throw 'Plugin build failed' }
    & $csc /nologo /platform:x86 /r:"$build/BinaryAssetBuilder.Core.dll" /r:"$build/Recovered.Armor.dll" /out:"$build/ArmorOracle.exe" (Resolve-Path 'recovery/tests/ArmorOracle.cs') *> "$report/$optimization-oracle-build.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$report/$optimization-oracle-build.txt"; throw 'Oracle build failed' }
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$build/ArmorOracle.exe.config"
    & "$build/ArmorOracle.exe" "$build/BinaryAssetBuilder.Tokenizer.dll" "$build/Recovered.Compiler.dll" *> "$report/$optimization-oracle.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$report/$optimization-oracle.txt"; throw 'Formal plugin differential failed' }
}
& $csc /nologo /platform:x86 /r:System.Core.dll /r:"$build/BinaryAssetBuilder.Core.dll" /out:"$build/ArmorSdkOracle.exe" (Resolve-Path 'recovery/tests/ArmorSdkOracle.cs') *> "$report/sdk-oracle-build.txt"
if ($LASTEXITCODE -ne 0) { Get-Content "$report/sdk-oracle-build.txt"; throw 'SDK oracle build failed' }
Copy-Item "$build/ArmorOracle.exe.config" "$build/ArmorSdkOracle.exe.config"
& "$build/ArmorSdkOracle.exe" schema (Join-Path $root 'SDK') "$report/schema.txt" *> "$report/schema-output.txt"
if ($LASTEXITCODE -ne 0) { Get-Content "$report/schema-output.txt"; throw 'XSD validation failed' }
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
    & "$PSScriptRoot/SetArmorPlugin.ps1" -SdkRoot $sdk -PluginDirectory $build -Mode $mode
    $cfg = Join-Path $sdk 'tools/BinaryAssetBuilder.exe.config'
    [xml]$xml = Get-Content -Raw $cfg
    $startup = $xml.CreateElement('startup'); $startup.SetAttribute('useLegacyV2RuntimeActivationPolicy','true')
    $runtime = $xml.CreateElement('supportedRuntime'); $runtime.SetAttribute('version','v4.0')
    $startup.AppendChild($runtime) | Out-Null; $xml.configuration.AppendChild($startup) | Out-Null; $xml.Save($cfg)
    Copy-Item $cfg "$report/$mode.config"
    $env:RA3_RECOVERED_ARMOR_TRACE = if ($mode -eq 'Recovered') { $trace } else { $null }
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
$env:RA3_RECOVERED_ARMOR_TRACE = $null
$results | ConvertTo-Json | Set-Content "$report/files.json"
foreach ($ext in @('bin','manifest','imp','relo')) {
    if ((Get-FileHash "$report/Original-mod.$ext").Hash -ne (Get-FileHash "$report/Recovered-mod.$ext").Hash) { throw "SDK output differs: mod.$ext" }
}
& "$build/ArmorSdkOracle.exe" buffers "$build/BinaryAssetBuilder.Tokenizer.dll" $trace "$report/Recovered-mod.bin" *> "$report/sdk-buffer-oracle.txt"
if ($LASTEXITCODE -ne 0) { Get-Content "$report/sdk-buffer-oracle.txt"; throw 'SDK buffer/routing verification failed' }
Get-Content "$report/sdk-buffer-oracle.txt"
@{ succeeded = $true; sourceCommit = $env:GITHUB_SHA; runId = $env:GITHUB_RUN_ID; status = 'Formal Armor plugin Od/O2 differentials; XSD; original/recovered fresh SDK builds; all four outputs byte-identical; traced actual buffers verified'; scope = 'ArmorTemplate only; original plugins retained for other types' } | ConvertTo-Json | Set-Content "$report/validation.json"
