param(
    [Parameter(Mandatory)][string]$SdkRoot,
    [Parameter(Mandatory)][string]$PluginDirectory,
    [ValidateSet('Original','Recovered')][string]$Mode = 'Recovered'
)
$ErrorActionPreference = 'Stop'
$cfg = Join-Path $SdkRoot 'tools/BinaryAssetBuilder.exe.config'
[xml]$xml = Get-Content -Raw $cfg
$plugins = $xml.configuration.assetbuilder.plugins
# Remove any previous recovered registration, then remove explicit Armor routing
# from other handlers. PluginRegistry uses Dictionary.Add, not last-one-wins.
foreach ($plugin in @($plugins.plugin)) {
    if ($plugin.targetType -eq 'Recovered.Compiler.Plugin, Recovered.Compiler') {
        $plugins.RemoveChild($plugin) | Out-Null
        continue
    }
    $types = @($plugin.assetTypes.Split(',') | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne 'ArmorTemplate' })
    if ($types.Count -eq 0) { $plugins.RemoveChild($plugin) | Out-Null }
    else { $plugin.SetAttribute('assetTypes', ($types -join ',')) }
}
$tokenizer = @($plugins.plugin | Where-Object { $_.targetType -eq 'BinaryAssetBuilder.Tokenizer.Plugin, BinaryAssetBuilder.Tokenizer' })
if ($tokenizer.Count -ne 1) { throw 'Expected exactly one original Tokenizer registration' }
if ($Mode -eq 'Original') {
    $tokenizer[0].SetAttribute('assetTypes', $tokenizer[0].assetTypes + ',ArmorTemplate')
} else {
    foreach ($name in @('Recovered.Compiler.dll','Recovered.Armor.dll')) {
        Copy-Item (Join-Path $PluginDirectory $name) (Join-Path $SdkRoot "tools/$name") -Force
    }
    $plugin = $xml.CreateElement('plugin')
    $plugin.SetAttribute('assetTypes', 'ArmorTemplate')
    $plugin.SetAttribute('enabled', 'true')
    $plugin.SetAttribute('useBuildCache', 'false')
    $plugin.SetAttribute('targetType', 'Recovered.Compiler.Plugin, Recovered.Compiler')
    $plugins.AppendChild($plugin) | Out-Null
}
$xml.Save($cfg)
