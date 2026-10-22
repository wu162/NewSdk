$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$out = Join-Path $env:RUNNER_TEMP 'ra3-recovered-weapon'
$report = Join-Path $root 'recovery/evidence/weapon'
$validation = Get-Content "$report/validation.json" -Raw | ConvertFrom-Json
if (!$validation.succeeded -or $validation.sourceCommit -ne $env:GITHUB_SHA) { throw 'Current component validation must pass first' }
$validation.succeeded = $false
$validation | Add-Member -Force NoteProperty componentSucceeded $true
$validation | ConvertTo-Json -Depth 8 | Set-Content "$report/validation.json"
Remove-Item "$report/nugget-sdk-*.txt", "$report/nugget-sdk.json", "$report/nugget-sdk-output.zip" -Force -ErrorAction SilentlyContinue
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
& $csc /nologo /platform:x86 /r:"$out/Recovered.Weapon.dll" /r:"$out/BinaryAssetBuilder.Core.dll" /out:"$out/WeaponSdkProbeOracle.exe" (Resolve-Path 'recovery/tests/WeaponSdkProbeOracle.cs') *> "$report/nugget-sdk-host-build.txt"
if ($LASTEXITCODE -ne 0) { Get-Content "$report/nugget-sdk-host-build.txt"; throw 'SDK probe oracle build failed' }
Copy-Item "$out/WeaponOracle.exe.config" "$out/WeaponSdkProbeOracle.exe.config"
& "$out/WeaponSdkProbeOracle.exe" schema (Join-Path $root 'SDK') *> "$report/nugget-sdk-schema.txt"
if ($LASTEXITCODE -ne 0) { Get-Content "$report/nugget-sdk-schema.txt"; throw 'Nugget mymod probe XSD failed' }
$sdk = Join-Path $env:RUNNER_TEMP 'ra3-nugget-sdk-probe'
if (Test-Path $sdk) { Remove-Item $sdk -Recurse -Force }
Copy-Item (Join-Path $root 'SDK') $sdk -Recurse
Remove-Item (Join-Path $sdk 'builtmods') -Recurse -Force -ErrorAction SilentlyContinue
$output = Join-Path $sdk 'builtmods'
New-Item -ItemType Directory -Force $output | Out-Null
[xml]$config = Get-Content (Join-Path $sdk 'tools/BinaryAssetBuilder.exe.config') -Raw
$tokenizer = @($config.configuration.assetbuilder.plugins.plugin | Where-Object { $_.targetType -eq 'BinaryAssetBuilder.Tokenizer.Plugin, BinaryAssetBuilder.Tokenizer' -and $_.assetTypes.Split(',') -contains 'WeaponTemplate' })
if ($tokenizer.Count -ne 1) { throw 'This probe requires the original SDK WeaponTemplate route' }
$startup = $config.CreateElement('startup');$startup.SetAttribute('useLegacyV2RuntimeActivationPolicy','true')
$runtime = $config.CreateElement('supportedRuntime');$runtime.SetAttribute('version','v4.0')
$startup.AppendChild($runtime) | Out-Null;$config.configuration.AppendChild($startup) | Out-Null
$config.Save((Join-Path $sdk 'tools/BinaryAssetBuilder.exe.config'))
$mod = Join-Path $sdk 'Mods/mymod/Data/Mod.xml'
$arguments = @("`"$mod`"", "/od:`"$output`"", "/iod:`"$output`"", '/csc:false', '/ls:true', '/osh:false', '/pc:true', '/res:true', '/slowclean:true', '/ss:true', '/art:".;.\Mods\mymod\Art;.\Mods;.\Art"', '/audio:".;.\Mods\mymod\Audio;.\Mods;.\Audio"', '/data:".;.\Mods\mymod\Data;.\Mods;.\SageXml"')
$p = Start-Process (Join-Path $sdk 'tools/BinaryAssetBuilder.exe') -ArgumentList ($arguments -join ' ') -WorkingDirectory $sdk -PassThru -RedirectStandardOutput "$report/nugget-sdk-stdout.txt" -RedirectStandardError "$report/nugget-sdk-stderr.txt"
if (!$p.WaitForExit(300000)) { Stop-Process -Id $p.Id -Force; throw 'Original SDK nugget probe timed out' }
$p.Refresh()
if ($p.ExitCode -ne 0) { Get-Content "$report/nugget-sdk-stdout.txt"; Get-Content "$report/nugget-sdk-stderr.txt"; throw 'Original SDK nugget probe build failed' }
$details = Join-Path $out 'sdk-probe-buffers'
if (Test-Path $details) { Remove-Item $details -Recurse -Force }
New-Item -ItemType Directory -Force $details | Out-Null
$files = foreach ($ext in @('bin','manifest','imp','relo')) {
    $file = Join-Path $output "mods/mymod/data/mod.$ext"
    if (!(Test-Path $file)) { throw "Missing original mod.$ext output" }
    Copy-Item $file (Join-Path $details "original-mod.$ext")
    @{ file = "mod.$ext"; bytes = (Get-Item $file).Length; sha256 = (Get-FileHash $file -Algorithm SHA256).Hash }
}
& "$out/WeaponSdkProbeOracle.exe" payloads $sdk (Join-Path $output 'mods/mymod/data/mod.bin') $details *> "$report/nugget-sdk-payloads.txt"
if ($LASTEXITCODE -ne 0) { Get-Content "$report/nugget-sdk-payloads.txt"; throw 'Actual SDK output did not match recovered component' }
Get-Content "$report/nugget-sdk-schema.txt";Get-Content "$report/nugget-sdk-payloads.txt"
@{ sourceCommit = $env:GITHUB_SHA; succeeded = $true; sdkRoute = 'Original Tokenizer for WeaponTemplate'; scope = 'six XSD-valid mymod Nugget assets in real SDK output match recovered component InstanceData; all three buffers separately checked by ProcessInstance differentials'; files = @($files) } | ConvertTo-Json -Depth 8 | Set-Content "$report/nugget-sdk.json"
Compress-Archive -Path "$details/*" -DestinationPath "$report/nugget-sdk-output.zip" -Force
$validation.succeeded = $true
$validation | Add-Member -Force NoteProperty sdkProbeSucceeded $true
$validation | Add-Member -Force NoteProperty sdkProbeScope 'Original SDK build of six schema-valid mymod weapons; recovered InstanceData payload match only; not recovered Weapon routing'
$validation | ConvertTo-Json -Depth 8 | Set-Content "$report/validation.json"
