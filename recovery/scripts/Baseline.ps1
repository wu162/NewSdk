param([int]$TimeoutSeconds = 300)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
$scratch = Join-Path $env:RUNNER_TEMP 'ra3-original-baseline'
if (Test-Path $scratch) { Remove-Item -Recurse -Force $scratch }
Copy-Item (Join-Path $root 'SDK') $scratch -Recurse
# Do not accept previously committed builtmods/cache files as new compiler output.
Remove-Item (Join-Path $scratch 'builtmods') -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force (Join-Path $scratch 'builtmods') | Out-Null
$report = Join-Path $root 'recovery/evidence/baseline'
New-Item -ItemType Directory -Force $report | Out-Null
# CLR2 is not installed by default on hosted Windows. Keep this compatibility
# change isolated from SDK/tools and explicitly record it in the baseline report.
$cfg = Join-Path $scratch 'tools/BinaryAssetBuilder.exe.config'
[xml]$xml = Get-Content $cfg
$startup = $xml.CreateElement('startup')
$startup.SetAttribute('useLegacyV2RuntimeActivationPolicy', 'true')
$runtime = $xml.CreateElement('supportedRuntime')
$runtime.SetAttribute('version', 'v4.0')
$startup.AppendChild($runtime) | Out-Null
$xml.configuration.AppendChild($startup) | Out-Null
$xml.Save($cfg)
$mod = Join-Path $scratch 'Mods/mymod/Data/mod.xml'
$output = Join-Path $scratch 'builtmods'
$arguments = @("`"$mod`"", "/od:`"$output`"", "/iod:`"$output`"", '/csc:false', '/ls:true', '/osh:false', '/pc:true', '/res:true', '/slowclean:true', '/ss:true', '/art:".;.\Mods\mymod\Art;.\Mods;.\Art"', '/audio:".;.\Mods\mymod\Audio;.\Mods;.\Audio"', '/data:".;.\Mods\mymod\Data;.\Mods;.\SageXml"')
$exe = Join-Path $scratch 'tools/BinaryAssetBuilder.exe'
$start = Get-Date
$p = Start-Process $exe -ArgumentList ($arguments -join ' ') -WorkingDirectory $scratch -PassThru -RedirectStandardOutput "$report/stdout.txt" -RedirectStandardError "$report/stderr.txt"
$finished = $p.WaitForExit($TimeoutSeconds * 1000)
if (!$finished) { Stop-Process -Id $p.Id -Force }
$p.Refresh()
$files = @(Get-ChildItem $output -Recurse -File | ForEach-Object {
    @{ path = $_.FullName.Substring($output.Length + 1); size = $_.Length; sha256 = (Get-FileHash $_.FullName -Algorithm SHA256).Hash }
})
$manifest = Join-Path $output 'mods/mymod/data/mod.manifest'
@{
    executable = 'SDK/tools/BinaryAssetBuilder.exe'; arguments = $arguments;
    sourceOfCommand = 'SDK/defaultscript.cs executeModBuildStep case 5';
    compatibilityChange = 'Temporary copy only: CLR4 startup with legacy mixed-mode activation';
    completed = $finished; exitCode = $p.ExitCode;
    elapsedSeconds = ((Get-Date) - $start).TotalSeconds;
    manifestCreated = (Test-Path $manifest); files = $files
} | ConvertTo-Json -Depth 8 | Set-Content "$report/result.json"
if (Test-Path $manifest) {
    Compress-Archive -Path "$output/*" -DestinationPath "$report/original-output.zip" -Force
}
# Failure is evidence, not a successful compiler test. The source extraction job
# must still upload its results when the original tool cannot run on the runner.
Get-Content "$report/result.json"
