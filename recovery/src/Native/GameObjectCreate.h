// Generated from original source blob 6e40a9ed6c9f961d294d49768d911e6a6f354619; four complete create Create modules.
const ModuleField createGrantUpgradeCreateModuleDataFields[] = {
    { "UpgradeToGrant", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "ExemptStatus", 12u, ModuleValue::ObjectStatus, nullptr, nullptr },
    { "GiveOnBuildComplete", 40u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleField createInheritUpgradeCreateModuleDataFields[] = {
    { "Radius", 8u, ModuleValue::Float, "0", nullptr },
};
const ModuleField createExperienceLevelCreateModuleDataFields[] = {
    { "Level", 8u, ModuleValue::Signed, "0", nullptr },
    { "MPOnly", 12u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleFieldSet createFieldSets[] = {
    { createGrantUpgradeCreateModuleDataFields, 3u },
    { createInheritUpgradeCreateModuleDataFields, 1u },
    { createExperienceLevelCreateModuleDataFields, 2u },
    { nullptr, 0u },
};
const ModuleToken createCreateModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &lifecycleBehaviorModuleData, 0u, 0u },
};
const ModuleLayout createCreateModuleData = { createCreateModuleDataTokens, 1u };
const ModuleToken createGrantUpgradeCreateModuleDataTokens[] = {
    { 1u, 816016269u, 0u, &createCreateModuleData, 0u, 0u },
    { 291847450u, 1253711657u, 8u, nullptr, 0u, 0u },
    { 2894011938u, 2814214430u, 12u, nullptr, 0u, 0u },
    { 3366724162u, 2011428087u, 40u, nullptr, 0u, 0u },
};
const ModuleLayout createGrantUpgradeCreateModuleData = { createGrantUpgradeCreateModuleDataTokens, 4u };
const ModuleToken createInheritUpgradeCreateModuleDataTokens[] = {
    { 1u, 816016269u, 0u, &createCreateModuleData, 0u, 0u },
    { 1328662213u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 3832456114u, 3832456114u, 12u, nullptr, 8u, 0u },
    { 3619147155u, 653971399u, 124u, nullptr, 3u, 4u },
};
const ModuleLayout createInheritUpgradeCreateModuleData = { createInheritUpgradeCreateModuleDataTokens, 4u };
const ModuleToken createExperienceLevelCreateModuleDataTokens[] = {
    { 1u, 816016269u, 0u, &createCreateModuleData, 0u, 0u },
    { 3099873959u, 602296504u, 8u, nullptr, 0u, 0u },
    { 2330193742u, 2011428087u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout createExperienceLevelCreateModuleData = { createExperienceLevelCreateModuleDataTokens, 3u };
const ModuleToken createActivateSpecialPowerOnCreateTokens[] = {
    { 1u, 4043318533u, 0u, &lifecycleUpdateModuleData, 0u, 0u },
    { 2178440954u, 2934454408u, 8u, nullptr, 3u, 4u },
};
const ModuleLayout createActivateSpecialPowerOnCreate = { createActivateSpecialPowerOnCreateTokens, 2u };
const ModuleChoice createChoices[] = {
    { 79217712u, &createGrantUpgradeCreateModuleData, 44u, 55u },
    { 3221293211u, &createInheritUpgradeCreateModuleData, 132u, 56u },
    { 3270227277u, &createExperienceLevelCreateModuleData, 16u, 57u },
    { 3310015739u, &createActivateSpecialPowerOnCreate, 16u, 58u },
};
