param(
    [Parameter(Mandatory)][string]$SdkRoot,
    [Parameter(Mandatory)][string]$PluginDirectory,
    [ValidateSet('Original','Recovered')][string]$Mode = 'Recovered'
)
$ErrorActionPreference = 'Stop'
$cfg = Join-Path $SdkRoot 'tools/BinaryAssetBuilder.exe.config'
[xml]$xml = Get-Content -Raw $cfg
$plugins = $xml.configuration.assetbuilder.plugins
$selected = @('AttributeModifier','FXList','SpecialPowerTemplate','ObjectCreationList','LocomotorTemplate','LogicCommandSet','FXParticleSystemTemplate','UpgradeTemplate','AIPersonalityDefinition','AIStrategicStateDefinition','AIMicroManagerData','AIBudgetStateDefinition','AIStateDefinition','AITargetingHeuristic','LogicCommand','PlayerTech','PlayerTechRank','PlayerTechUpgradeBinding','ExperienceLevelTemplate','PlayerTechStoreTemplate','PurchasePlayerTechButtonTemplate','ButtonSingleStateData','UICommandButton','FontDefaultSetting','FontTranslation','FontSubstitution','Weather','WaterTransparency','WaterSetting','Environment','LightingSettings','WaterTextureList','TerrainParticleMapping','BounceKickTerrainMapping','ShadowMap','OrdersManagerSettings','TerrainVisualSettings','CameraSettings','AIData','UnitAISettings','PlayerTemplate','MultiplayerSettings','AIBuildStateDefinition','AIStrategicStateName','MultiplayerColor','AIPersonalityToAchievementEventMapping','FormationAssistantUnitDefinition','FormationAssistantTemplate','FormationSelectionTemplate','FormationAssistant','GameStatsModeData','SkirmishSettings','DamageFX','DamageFXSettings','GeneralHotKeySlots','DefaultHotKeys','DebugHotKeys','MappableKey','AudioEventLimitGroup','AudioLOD','AudioSettings','Video','VideoEventList','MicrophoneSettings','BootupDisplaySequence','HotKeySlot','MouseCursor','BasicMouseCursors','UIMouseDragSelectionFeedback','UIFloatingTextDisplay','UIMessageDisplay','UIMilitaryCaptionDisplay','UISubtitleDisplay','UITimerDisplay','DynamicGameLOD','GameLODPreset','StaticGameLOD','UICoopTinting','RadiusCursor','AnimationSoundSettings','MiscAudio','LargeGroupAudioMap')
foreach ($plugin in @($plugins.plugin)) {
    if ($plugin.targetType -eq 'Recovered.XmlPriority.Plugin, Recovered.XmlPriority.Plugin') {
        $plugins.RemoveChild($plugin) | Out-Null
        continue
    }
    $types = @($plugin.assetTypes.Split(',') | ForEach-Object { $_.Trim() } | Where-Object { $_ -notin $selected })
    if ($types.Count -eq 0) { $plugins.RemoveChild($plugin) | Out-Null }
    else { $plugin.SetAttribute('assetTypes', ($types -join ',')) }
}
if (@($plugins.plugin | Where-Object { $_.assetTypes -eq '#all' -and $_.targetType -eq 'BinaryAssetBuilder.XmlCompiler.Plugin, BinaryAssetBuilder.XmlCompiler' }).Count -ne 1) {
    throw 'Expected the original XmlCompiler default handler'
}
# Use identical explicit 82-entry registration topology in both disposable SDKs.
# PluginRegistry.AssetBuilderPluginsVersion sums each explicit type's version.
$target = 'BinaryAssetBuilder.XmlCompiler.Plugin, BinaryAssetBuilder.XmlCompiler'
if ($Mode -eq 'Recovered') {
    $target = 'Recovered.XmlPriority.Plugin, Recovered.XmlPriority.Plugin'
    foreach ($name in @('Recovered.XmlPriority.Plugin.dll','Recovered.XmlCompilerPriority.dll')) {
        Copy-Item (Join-Path $PluginDirectory $name) (Join-Path $SdkRoot "tools/$name") -Force
    }
} else {
    foreach ($name in @('Recovered.XmlPriority.Plugin.dll','Recovered.XmlCompilerPriority.dll')) {
        Remove-Item (Join-Path $SdkRoot "tools/$name") -Force -ErrorAction SilentlyContinue
    }
}
$entry = $xml.CreateElement('plugin')
$entry.SetAttribute('assetTypes', ($selected -join ','))
$entry.SetAttribute('enabled','true')
$entry.SetAttribute('useBuildCache','false')
$entry.SetAttribute('targetType',$target)
$plugins.AppendChild($entry) | Out-Null
$xml.Save($cfg)


