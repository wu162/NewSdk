// Complete building-structure contracts; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const ModuleField buildingStructureBuildingBehaviorModuleDataFields[] = {
    { "NightWindowObject", 8u, ModuleValue::String, nullptr, nullptr },
    { "FireWindowObject", 16u, ModuleValue::String, nullptr, nullptr },
    { "GlowWindowObject", 24u, ModuleValue::String, nullptr, nullptr },
    { "NightFireWindowObject", 32u, ModuleValue::String, nullptr, nullptr },
};
const ModuleField buildingStructureStructureUnpackUpdateModuleDataFields[] = {
    { "UnpackTime", 8u, ModuleValue::Time, "0s", nullptr },
    { "UnpackCompleteSound", 12u, ModuleValue::NullableReference, nullptr, nullptr },
    { "OffsetHeightAboveWater", 16u, ModuleValue::Float, "5.0", nullptr },
};
const ModuleField buildingStructureStructureSellUpdateModuleDataFields[] = {
    { "SellTime", 8u, ModuleValue::Time, "0s", nullptr },
    { "ActiveModelCondition", 12u, ModuleValue::Enum, nullptr, &aiModelConditionFlagType },
};
const ModuleFieldSet buildingStructureFieldSets[] = {
    { buildingStructureBuildingBehaviorModuleDataFields, 4u },
    { buildingStructureStructureUnpackUpdateModuleDataFields, 3u },
    { buildingStructureStructureSellUpdateModuleDataFields, 2u },
};
const ModuleToken buildingStructureBuildingBehaviorModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 4230438045u, 2153588212u, 8u, nullptr, 0u, 0u },
    { 2995342761u, 2153588212u, 16u, nullptr, 0u, 0u },
    { 1093389552u, 2153588212u, 24u, nullptr, 0u, 0u },
    { 3042298789u, 2153588212u, 32u, nullptr, 0u, 0u },
};
const ModuleLayout buildingStructureBuildingBehaviorModuleData = { buildingStructureBuildingBehaviorModuleDataTokens, 5u };
const ModuleToken buildingStructureStructureUnpackUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 2661360556u, 2575722984u, 8u, nullptr, 0u, 0u },
    { 3962751035u, 1675080808u, 12u, nullptr, 2u, 0u },
    { 3511020321u, 1683514611u, 16u, nullptr, 0u, 0u },
};
const ModuleLayout buildingStructureStructureUnpackUpdateModuleData = { buildingStructureStructureUnpackUpdateModuleDataTokens, 4u };
const ModuleToken buildingStructureStructureSellUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 3767409419u, 2575722984u, 8u, nullptr, 0u, 0u },
    { 1493854600u, 359808523u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout buildingStructureStructureSellUpdateModuleData = { buildingStructureStructureSellUpdateModuleDataTokens, 3u };
const ModuleChoice buildingStructureChoices[] = {
    { 2170196321u, &buildingStructureBuildingBehaviorModuleData, 40u, 130u },
    { 2541716369u, &buildingStructureStructureUnpackUpdateModuleData, 20u, 131u },
    { 1318893585u, &buildingStructureStructureSellUpdateModuleData, 16u, 132u },
};
