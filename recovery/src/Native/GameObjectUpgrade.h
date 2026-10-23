// Generated original Upgrade contracts; source blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const EnumNameValue upgradeArmorSetTypeNames[] = {
    { "AIRBORNE", 12 },
    { "ALTERNATE_FORMATION", 6 },
    { "AS_TOWER", 11 },
    { "ELITE", 2 },
    { "HERO", 3 },
    { "INVALID", 0 },
    { "IN_SPIDER_HOLE", 15 },
    { "MOUNTED", 7 },
    { "PLAYER_UPGRADE", 4 },
    { "PLAYER_UPGRADE_2", 8 },
    { "PLAYER_UPGRADE_3", 9 },
    { "SECONDARY_DAMAGE", 13 },
    { "SHIELDBODY_ENABLED", 22 },
    { "SHRINK_EFFECT", 14 },
    { "UNBESIEGEABLE", 10 },
    { "UNUSED_05", 16 },
    { "UNUSED_06", 17 },
    { "UNUSED_07", 18 },
    { "UNUSED_08", 19 },
    { "UNUSED_09", 20 },
    { "UNUSED_10", 21 },
    { "VETERAN", 1 },
    { "WEAK_VERSUS_BASEDEFENSES", 5 },
};
const AIEnum upgradeArmorSetType = { upgradeArmorSetTypeNames, 23u, 12, 0u };
const EnumNameValue upgradeMaxHealthChangeTypeNames[] = {
    { "ADD_CURRENT_HEALTH_TOO", 2 },
    { "PRESERVE_RATIO", 1 },
    { "SAME_CURRENTHEALTH", 0 },
};
const AIEnum upgradeMaxHealthChangeType = { upgradeMaxHealthChangeTypeNames, 3u, 2, 0u };
const ModuleField upgradeUpgradeModuleDataFields[] = {
    { "RequiresAllTriggers", 28u, ModuleValue::Boolean, "false", nullptr },
    { "RequiresAllConflictingTriggers", 29u, ModuleValue::Boolean, "false", nullptr },
    { "Permanent", 30u, ModuleValue::Boolean, "false", nullptr },
    { "WaypointMode", 31u, ModuleValue::Boolean, "false", nullptr },
    { "WaypointModeTerminal", 32u, ModuleValue::Boolean, "true", nullptr },
};
const ModuleField upgradeDelayedUpgradeModuleDataFields[] = {
    { "DelayTime", 36u, ModuleValue::Unsigned, "0", nullptr },
};
const ModuleField upgradeAttributeModifierUpgradeModuleDataFields[] = {
    { "AttributeModifier", 36u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleField upgradeInvisibilityUpgradeModuleDataFields[] = {
    { "InvisibilityTemplate", 36u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleField upgradeCommandSetUpgradeModuleDataFields[] = {
    { "CommandSet", 36u, ModuleValue::AssetId, nullptr, nullptr },
};
const ModuleField upgradeExperienceScalarUpgradeModuleDataFields[] = {
    { "AddXPScalar", 36u, ModuleValue::Float, "0", nullptr },
};
const ModuleField upgradeMaxHealthUpgradeModuleDataFields[] = {
    { "AddMaxHealth", 36u, ModuleValue::Float, "0.0", nullptr },
    { "ChangeType", 40u, ModuleValue::Enum, nullptr, &upgradeMaxHealthChangeType },
};
const ModuleField upgradeArmorUpgradeModuleDataFields[] = {
    { "KillArmorUpgrade", 40u, ModuleValue::Boolean, "false", nullptr },
    { "IgnoreArmorUpgrade", 41u, ModuleValue::Boolean, "false", nullptr },
    { "ArmorSetFlag", 36u, ModuleValue::Enum, nullptr, &upgradeArmorSetType },
};
const ModuleField upgradeLevelUpUpgradeModuleDataFields[] = {
    { "LevelsToGain", 36u, ModuleValue::Signed, "0", nullptr },
    { "LevelCap", 40u, ModuleValue::Signed, "0", nullptr },
    { "DoFlash", 44u, ModuleValue::Boolean, "true", nullptr },
};
const ModuleField upgradeAudioLoopUpgradeModuleDataFields[] = {
    { "SoundToPlay", 36u, ModuleValue::Reference, nullptr, nullptr },
    { "KillAfterMS", 40u, ModuleValue::Unsigned, nullptr, nullptr },
    { "KillOnDeath", 44u, ModuleValue::Boolean, nullptr, nullptr },
};
const ModuleField upgradeAnimAndDurationFields[] = {
    { "AnimState", 0u, ModuleValue::Enum, nullptr, &aiModelConditionFlagType },
    { "Frames", 4u, ModuleValue::Unsigned, "0", nullptr },
    { "Trigger", 8u, ModuleValue::Unsigned, "0", nullptr },
};
const ModuleFieldSet upgradeFieldSets[] = {
    { upgradeUpgradeModuleDataFields, 5u },
    { nullptr, 0u },
    { upgradeDelayedUpgradeModuleDataFields, 1u },
    { upgradeAttributeModifierUpgradeModuleDataFields, 1u },
    { upgradeInvisibilityUpgradeModuleDataFields, 1u },
    { upgradeCommandSetUpgradeModuleDataFields, 1u },
    { upgradeExperienceScalarUpgradeModuleDataFields, 1u },
    { upgradeMaxHealthUpgradeModuleDataFields, 2u },
    { upgradeArmorUpgradeModuleDataFields, 3u },
    { upgradeLevelUpUpgradeModuleDataFields, 3u },
    { upgradeAudioLoopUpgradeModuleDataFields, 3u },
    { upgradeAnimAndDurationFields, 3u },
};
const ModuleToken upgradeModuleDataTokens[] = {
    { 430981792u, 602296504u, 0u, nullptr, 0u, 0u },
    { 2363267224u, 3186653541u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout upgradeModuleData = { upgradeModuleDataTokens, 2u };
const ModuleToken upgradeBehaviorModuleDataTokens[] = {
    { 1u, 3613234082u, 0u, &upgradeModuleData, 0u, 0u },
};
const ModuleLayout upgradeBehaviorModuleData = { upgradeBehaviorModuleDataTokens, 1u };
const ModuleToken upgradeAnimAndDurationTokens[] = {
    { 2823664884u, 359808523u, 0u, nullptr, 0u, 0u },
    { 161845256u, 1982953380u, 4u, nullptr, 0u, 0u },
    { 2187291630u, 1982953380u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout upgradeAnimAndDuration = { upgradeAnimAndDurationTokens, 3u };
const ModuleToken upgradeUpgradeModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &upgradeBehaviorModuleData, 0u, 0u },
    { 3634213996u, 2011428087u, 28u, nullptr, 0u, 0u },
    { 4011489361u, 2011428087u, 29u, nullptr, 0u, 0u },
    { 3434725126u, 2011428087u, 30u, nullptr, 0u, 0u },
    { 460323776u, 2011428087u, 31u, nullptr, 0u, 0u },
    { 1687664392u, 2011428087u, 32u, nullptr, 0u, 0u },
    { 2510190459u, 1802791670u, 8u, &upgradeAnimAndDuration, 4u, 0u },
    { 1776173490u, 653971399u, 12u, nullptr, 3u, 4u },
    { 4294320002u, 653971399u, 20u, nullptr, 3u, 4u },
};
const ModuleLayout upgradeUpgradeModuleData = { upgradeUpgradeModuleDataTokens, 9u };
const ModuleToken upgradePowerUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
};
const ModuleLayout upgradePowerUpgradeModuleData = { upgradePowerUpgradeModuleDataTokens, 1u };
const ModuleToken upgradeDelayedUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 3811566927u, 1982953380u, 36u, nullptr, 0u, 0u },
};
const ModuleLayout upgradeDelayedUpgradeModuleData = { upgradeDelayedUpgradeModuleDataTokens, 2u };
const ModuleToken upgradeAttributeModifierUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 3319822471u, 165531635u, 36u, nullptr, 0u, 0u },
};
const ModuleLayout upgradeAttributeModifierUpgradeModuleData = { upgradeAttributeModifierUpgradeModuleDataTokens, 2u };
const ModuleToken upgradeInvisibilityUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 1431290504u, 2437719909u, 36u, nullptr, 0u, 0u },
};
const ModuleLayout upgradeInvisibilityUpgradeModuleData = { upgradeInvisibilityUpgradeModuleDataTokens, 2u };
const ModuleToken upgradeCommandSetUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 2584759047u, 2245282653u, 36u, nullptr, 0u, 0u },
};
const ModuleLayout upgradeCommandSetUpgradeModuleData = { upgradeCommandSetUpgradeModuleDataTokens, 2u };
const ModuleToken upgradeExperienceScalarUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 4006572887u, 1683514611u, 36u, nullptr, 0u, 0u },
};
const ModuleLayout upgradeExperienceScalarUpgradeModuleData = { upgradeExperienceScalarUpgradeModuleDataTokens, 2u };
const ModuleToken upgradeMaxHealthUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 3032348824u, 1683514611u, 36u, nullptr, 0u, 0u },
    { 1272345052u, 3101050986u, 40u, nullptr, 0u, 0u },
};
const ModuleLayout upgradeMaxHealthUpgradeModuleData = { upgradeMaxHealthUpgradeModuleDataTokens, 3u };
const ModuleToken upgradeArmorUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 3932257339u, 2011428087u, 40u, nullptr, 0u, 0u },
    { 1683401354u, 2011428087u, 41u, nullptr, 0u, 0u },
    { 1652355685u, 540476559u, 36u, nullptr, 0u, 0u },
};
const ModuleLayout upgradeArmorUpgradeModuleData = { upgradeArmorUpgradeModuleDataTokens, 4u };
const ModuleToken upgradeLevelUpUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 3158960348u, 602296504u, 36u, nullptr, 0u, 0u },
    { 2579596682u, 602296504u, 40u, nullptr, 0u, 0u },
    { 1084140215u, 2011428087u, 44u, nullptr, 0u, 0u },
};
const ModuleLayout upgradeLevelUpUpgradeModuleData = { upgradeLevelUpUpgradeModuleDataTokens, 4u };
const ModuleToken upgradeAudioLoopUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 3110107903u, 1675080808u, 36u, nullptr, 0u, 0u },
    { 2625281214u, 1982953380u, 40u, nullptr, 0u, 0u },
    { 664167800u, 2011428087u, 44u, nullptr, 0u, 0u },
};
const ModuleLayout upgradeAudioLoopUpgradeModuleData = { upgradeAudioLoopUpgradeModuleDataTokens, 4u };
const ModuleChoice upgradeChoices[] = {
    { 2925571736u, &upgradeUpgradeModuleData, 36u, 30u },
    { 3282447464u, &upgradePowerUpgradeModuleData, 36u, 31u },
    { 1518063315u, &upgradeDelayedUpgradeModuleData, 40u, 32u },
    { 1956793413u, &upgradeAttributeModifierUpgradeModuleData, 40u, 33u },
    { 1220819118u, &upgradeInvisibilityUpgradeModuleData, 40u, 34u },
    { 2125984281u, &upgradeCommandSetUpgradeModuleData, 40u, 35u },
    { 3765090183u, &upgradeExperienceScalarUpgradeModuleData, 40u, 36u },
    { 2090477367u, &upgradeMaxHealthUpgradeModuleData, 44u, 37u },
    { 2717792172u, &upgradeArmorUpgradeModuleData, 44u, 38u },
    { 2541561249u, &upgradeLevelUpUpgradeModuleData, 48u, 39u },
    { 585970648u, &upgradeAudioLoopUpgradeModuleData, 48u, 40u },
};
