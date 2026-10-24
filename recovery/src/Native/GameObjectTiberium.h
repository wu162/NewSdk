// Complete Tiberium contracts; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
// RadarColor uses unsigned parse -> byte truncation -> ARGB packed word; no color node leaves zero.
const char* tiberiumColorNames[] = { "R", "G", "B", "A" };
const char* tiberiumColorDefaults[] = { "0", "0", "0", "255" };
const ModuleField tiberiumTiberiumGrowthModBehaviorModuleDataFields[] = {
    { "GreenTiberiumMultiplier", 52u, ModuleValue::Float, "1.0", nullptr },
    { "BlueTiberiumMultiplier", 56u, ModuleValue::Float, "1.0", nullptr },
};
const ModuleField tiberiumTiberiumCrystalBehaviorModuleDataFields[] = {
    { "NumBoxes", 8u, ModuleValue::Unsigned, "0", nullptr },
    { "ValuePerBox", 12u, ModuleValue::Unsigned, "0", nullptr },
    { "GrowthStages", 16u, ModuleValue::Unsigned, "1", nullptr },
    { "TimeBetweenGrowthStages", 20u, ModuleValue::Time, "5s", nullptr },
    { "GrowthTimePerStage", 24u, ModuleValue::Time, "3s", nullptr },
};
const ModuleField tiberiumTiberiumFieldBehaviorModuleDataFields[] = {
    { "DamageUpdateFrequencySeconds", 8u, ModuleValue::Time, "0.67s", nullptr },
    { "HealUpdateFrequencySeconds", 12u, ModuleValue::Time, "0.60s", nullptr },
    { "Radius", 16u, ModuleValue::Float, "0", nullptr },
    { "DamageRadiusAdd", 20u, ModuleValue::Float, "20", nullptr },
    { "HealRadiusAdd", 24u, ModuleValue::Float, "20", nullptr },
    { "FieldResolution", 28u, ModuleValue::Signed, "40", nullptr },
    { "MaxFieldMoney", 32u, ModuleValue::Signed, "0", nullptr },
    { "StartingFieldMoney", 36u, ModuleValue::Signed, "0", nullptr },
    { "ThingToSpawn", 40u, ModuleValue::AssetId, nullptr, nullptr },
    { "SpawnOffset", 44u, ModuleValue::Float, "0.0", nullptr },
    { "ClusterScaleFactor", 48u, ModuleValue::Float, "1.0", nullptr },
    { "ClusterPowerFactor", 52u, ModuleValue::Float, "1.0", nullptr },
    { "CrystalGrowthRate", 56u, ModuleValue::Signed, "0", nullptr },
    { "LifetimeFieldMoney", 60u, ModuleValue::Unsigned, "0", nullptr },
    { "KillWhenEmptyAndFinishedSpawning", 72u, ModuleValue::Boolean, "false", nullptr },
    { "AllowStartingFieldCrystalsToGrow", 73u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleFieldSet tiberiumFieldSets[] = {
    { tiberiumTiberiumGrowthModBehaviorModuleDataFields, 2u },
    { tiberiumTiberiumCrystalBehaviorModuleDataFields, 5u },
    { tiberiumTiberiumFieldBehaviorModuleDataFields, 16u },
};
const ModuleToken tiberiumTiberiumGrowthModBehaviorModuleDataTokens[] = {
    { 1u, 2655988594u, 0u, &dieDieModuleData, 0u, 0u },
    { 731730542u, 1683514611u, 52u, nullptr, 0u, 0u },
    { 389245546u, 1683514611u, 56u, nullptr, 0u, 0u },
};
const ModuleLayout tiberiumTiberiumGrowthModBehaviorModuleData = { tiberiumTiberiumGrowthModBehaviorModuleDataTokens, 3u };
const ModuleToken tiberiumTiberiumCrystalBehaviorModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 2206315201u, 1982953380u, 8u, nullptr, 0u, 0u },
    { 3399082896u, 1982953380u, 12u, nullptr, 0u, 0u },
    { 2371217592u, 1982953380u, 16u, nullptr, 0u, 0u },
    { 297525730u, 2575722984u, 20u, nullptr, 0u, 0u },
    { 3688057992u, 2575722984u, 24u, nullptr, 0u, 0u },
    { 859172812u, 2654902107u, 28u, nullptr, 0u, 0u },
};
const ModuleLayout tiberiumTiberiumCrystalBehaviorModuleData = { tiberiumTiberiumCrystalBehaviorModuleDataTokens, 7u };
const ModuleToken tiberiumTiberiumFieldBehaviorModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 632573300u, 2575722984u, 8u, nullptr, 0u, 0u },
    { 1200563872u, 2575722984u, 12u, nullptr, 0u, 0u },
    { 1328662213u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 978231127u, 1683514611u, 20u, nullptr, 0u, 0u },
    { 809100689u, 1683514611u, 24u, nullptr, 0u, 0u },
    { 3915859749u, 602296504u, 28u, nullptr, 0u, 0u },
    { 1370731839u, 602296504u, 32u, nullptr, 0u, 0u },
    { 2765557538u, 602296504u, 36u, nullptr, 0u, 0u },
    { 4118313386u, 344905565u, 40u, nullptr, 0u, 0u },
    { 1152202613u, 1683514611u, 44u, nullptr, 0u, 0u },
    { 1765720748u, 1683514611u, 48u, nullptr, 0u, 0u },
    { 3969728535u, 1683514611u, 52u, nullptr, 0u, 0u },
    { 549669266u, 602296504u, 56u, nullptr, 0u, 0u },
    { 3149682598u, 1982953380u, 60u, nullptr, 0u, 0u },
    { 1563689956u, 2011428087u, 72u, nullptr, 0u, 0u },
    { 2873653892u, 2011428087u, 73u, nullptr, 0u, 0u },
    { 234483600u, 3832456114u, 64u, nullptr, 7u, 0u },
    { 3247156842u, 3832456114u, 68u, nullptr, 7u, 0u },
};
const ModuleLayout tiberiumTiberiumFieldBehaviorModuleData = { tiberiumTiberiumFieldBehaviorModuleDataTokens, 19u };
const ModuleChoice tiberiumChoices[] = {
    { 2923073644u, &tiberiumTiberiumGrowthModBehaviorModuleData, 60u, 147u },
    { 254628242u, &tiberiumTiberiumCrystalBehaviorModuleData, 32u, 148u },
    { 1808399630u, &tiberiumTiberiumFieldBehaviorModuleData, 76u, 149u },
};
