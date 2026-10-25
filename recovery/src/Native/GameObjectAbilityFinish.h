// Generated complete control/shield group; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const EnumNameValue abilityFinishShieldSphereUpdateOptionNames[] = {
    { "ALLOW_ALLIES_PROJECTTILE_GOTHROUGH", 0 },
};
const AIEnum abilityFinishShieldSphereUpdateOption = { abilityFinishShieldSphereUpdateOptionNames, 1u, 0, 1u };
const EnumNameValue abilityFinishTerrainClassTypeNames[] = {
    { "Cliff", 3 },
    { "Dirt", 2 },
    { "Grass", 4 },
    { "Misc", 1 },
    { "Mud", 7 },
    { "Road", 6 },
    { "Rock", 5 },
    { "Sand", 8 },
    { "Shrub", 9 },
    { "Snow", 10 },
    { "UNSPECIFIED", 0 },
};
const AIEnum abilityFinishTerrainClassType = { abilityFinishTerrainClassTypeNames, 11u, 3, 0u };
const EnumNameValue abilityFinishDamageTypeNames[] = {
    { "ALL", 38 },
    { "AUTO_CANNON", 29 },
    { "BLAST", 10 },
    { "CANNON", 22 },
    { "CONCUSSIVE", 32 },
    { "CRUSH", 1 },
    { "EXPLOSIVE", 31 },
    { "FALLING", 9 },
    { "FIRE", 19 },
    { "FLAK", 33 },
    { "FLAME", 4 },
    { "FORCE", 0 },
    { "GRENADE", 23 },
    { "GUN", 21 },
    { "HEALING", 5 },
    { "IMPACT", 30 },
    { "KI", 36 },
    { "LASER", 17 },
    { "LOGICAL_FIRE", 15 },
    { "MAGIC", 13 },
    { "MELEE", 28 },
    { "MINE", 25 },
    { "PASSENGER", 12 },
    { "PENALTY", 8 },
    { "PIERCE", 3 },
    { "PLASMA", 18 },
    { "POISON", 16 },
    { "PRISM", 34 },
    { "RADIATION", 37 },
    { "REFLECTED", 11 },
    { "ROCKET", 20 },
    { "SLASH", 2 },
    { "SNIPER", 24 },
    { "STORM_SHIELD", 27 },
    { "TESLA", 35 },
    { "TIBERIUM", 26 },
    { "UNDEFINED", 14 },
    { "UNRESISTABLE", 6 },
    { "WATER", 7 },
};
const AIEnum abilityFinishDamageType = { abilityFinishDamageTypeNames, 39u, 38, 39u };
const ModuleField abilityFinishSpecialDisguiseTerrainDataFields[] = {
    { "TerrainClass", 0u, ModuleValue::Enum, nullptr, &abilityFinishTerrainClassType },
    { "ObjectTemplate", 4u, ModuleValue::AssetId, nullptr, nullptr },
};
const ModuleToken abilityFinishSpecialDisguiseTerrainDataTokens[] = {
    { 3777418260u, 3454218194u, 0u, nullptr, 0u, 0u },
    { 846703367u, 344905565u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout abilityFinishSpecialDisguiseTerrainData = { abilityFinishSpecialDisguiseTerrainDataTokens, 2u };
const ModuleToken abilityFinishReplaceObjectNuggetTokens[] = {
    { 376819750u, 3832456114u, 0u, nullptr, 7u, 0u },
    { 3053726141u, 344905565u, 4u, nullptr, 3u, 0u },
};
const ModuleLayout abilityFinishReplaceObjectNugget = { abilityFinishReplaceObjectNuggetTokens, 2u };
const ModuleField abilityFinishGiveUpgradeUpdateModuleDataFields[] = {
    { "GiveUpgradeEffect", 240u, ModuleValue::Reference, nullptr, nullptr },
    { "SpawnOutFX", 244u, ModuleValue::Reference, nullptr, nullptr },
    { "FadeOutSpeed", 248u, ModuleValue::Float, "0.025", nullptr },
    { "DeliverUpgrade", 252u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleToken abilityFinishGiveUpgradeUpdateModuleDataTokens[] = {
    { 1u, 2891550594u, 0u, &abilitySpecialAbilityUpdateModuleData, 0u, 0u },
    { 1996389071u, 219975299u, 240u, nullptr, 0u, 0u },
    { 559101512u, 219975299u, 244u, nullptr, 0u, 0u },
    { 22217949u, 1683514611u, 248u, nullptr, 0u, 0u },
    { 185361499u, 2011428087u, 252u, nullptr, 0u, 0u },
};
const ModuleLayout abilityFinishGiveUpgradeUpdateModuleData = { abilityFinishGiveUpgradeUpdateModuleDataTokens, 5u };
const ModuleField abilityFinishReplaceObjectUpdateModuleDataFields[] = {
    { "ReplaceRadius", 240u, ModuleValue::Float, nullptr, nullptr },
    { "ReplaceFX", 244u, ModuleValue::Reference, nullptr, nullptr },
    { "Scatter", 256u, ModuleValue::Boolean, nullptr, nullptr },
};
const ModuleToken abilityFinishReplaceObjectUpdateModuleDataTokens[] = {
    { 1u, 2891550594u, 0u, &abilitySpecialAbilityUpdateModuleData, 0u, 0u },
    { 2683200202u, 1683514611u, 240u, nullptr, 0u, 0u },
    { 1872955543u, 219975299u, 244u, nullptr, 0u, 0u },
    { 3097292883u, 2011428087u, 256u, nullptr, 0u, 0u },
    { 2950708851u, 2950708851u, 248u, &abilityFinishReplaceObjectNugget, 1u, 12u },
};
const ModuleLayout abilityFinishReplaceObjectUpdateModuleData = { abilityFinishReplaceObjectUpdateModuleDataTokens, 5u };
const ModuleField abilityFinishShieldSphereUpdateModuleDataFields[] = {
    { "RadiusMax", 8u, ModuleValue::Float, "40", nullptr },
    { "RadiusMin", 12u, ModuleValue::Float, "10", nullptr },
    { "DamageTypesNotToAbsorb", 16u, ModuleValue::Bits, nullptr, &abilityFinishDamageType },
    { "ScanFrequency", 24u, ModuleValue::Time, "1s", nullptr },
    { "Duration", 28u, ModuleValue::Time, "10s", nullptr },
    { "MaxDamage", 32u, ModuleValue::Float, "0", nullptr },
    { "ObjectStatus", 36u, ModuleValue::ObjectStatus, "", nullptr },
    { "ModelCondition", 64u, ModuleValue::ModelCondition, "", nullptr },
    { "InitiallyActive", 260u, ModuleValue::Boolean, "false", nullptr },
    { "ShieldBoneName", 124u, ModuleValue::String, nullptr, nullptr },
    { "ShieldSizeMultiplier", 132u, ModuleValue::Float, "1.0", nullptr },
    { "Options", 136u, ModuleValue::Bits, nullptr, &abilityFinishShieldSphereUpdateOption },
};
const ModuleToken abilityFinishShieldSphereUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 3800915515u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 2534493354u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 1992436293u, 133869220u, 16u, nullptr, 0u, 0u },
    { 1328103125u, 2575722984u, 24u, nullptr, 0u, 0u },
    { 2384108347u, 2575722984u, 28u, nullptr, 0u, 0u },
    { 561127447u, 1683514611u, 32u, nullptr, 0u, 0u },
    { 3963816762u, 2814214430u, 36u, nullptr, 0u, 0u },
    { 3468811896u, 1742454729u, 64u, nullptr, 0u, 0u },
    { 1454433568u, 2011428087u, 260u, nullptr, 0u, 0u },
    { 718672663u, 438907818u, 124u, nullptr, 0u, 0u },
    { 2660289546u, 1683514611u, 132u, nullptr, 0u, 0u },
    { 3202187060u, 3938274318u, 136u, nullptr, 0u, 0u },
    { 4208789467u, 691437687u, 140u, nullptr, 2u, 0u },
    { 3832456114u, 3832456114u, 144u, nullptr, 8u, 0u },
    { 632492410u, 3832456114u, 256u, nullptr, 7u, 0u },
};
const ModuleLayout abilityFinishShieldSphereUpdateModuleData = { abilityFinishShieldSphereUpdateModuleDataTokens, 16u };
const ModuleField abilityFinishSpecialDisguiseUpdateModuleDataFields[] = {
    { "TriggerInstantlyOnCreate", 260u, ModuleValue::Boolean, "false", nullptr },
    { "OpacityTarget", 240u, ModuleValue::Float, "0", nullptr },
    { "DisguiseFX", 244u, ModuleValue::Reference, nullptr, nullptr },
    { "ScanRadiusForPropDisguise", 248u, ModuleValue::Float, "0.0", nullptr },
};
const ModuleToken abilityFinishSpecialDisguiseUpdateModuleDataTokens[] = {
    { 1u, 2891550594u, 0u, &abilitySpecialAbilityUpdateModuleData, 0u, 0u },
    { 2997621972u, 2011428087u, 260u, nullptr, 0u, 0u },
    { 2874932308u, 1683514611u, 240u, nullptr, 0u, 0u },
    { 4219948537u, 219975299u, 244u, nullptr, 0u, 0u },
    { 1913325433u, 1683514611u, 248u, nullptr, 0u, 0u },
    { 1413499044u, 962146009u, 252u, &abilityFinishSpecialDisguiseTerrainData, 1u, 8u },
};
const ModuleLayout abilityFinishSpecialDisguiseUpdateModuleData = { abilityFinishSpecialDisguiseUpdateModuleDataTokens, 6u };
const ModuleFieldSet abilityFinishFieldSets[] = {
    { abilityFinishGiveUpgradeUpdateModuleDataFields, 4u },
    { abilityFinishReplaceObjectUpdateModuleDataFields, 3u },
    { abilityFinishShieldSphereUpdateModuleDataFields, 12u },
    { abilityFinishSpecialDisguiseUpdateModuleDataFields, 4u },
};
const ModuleChoice abilityFinishChoices[] = {
    { 769868147u, &abilityFinishGiveUpgradeUpdateModuleData, 256u, 216u },
    { 1442440941u, &abilityFinishReplaceObjectUpdateModuleData, 260u, 217u },
    { 1813100279u, &abilityFinishShieldSphereUpdateModuleData, 264u, 218u },
    { 2784675807u, &abilityFinishSpecialDisguiseUpdateModuleData, 264u, 219u },
};
