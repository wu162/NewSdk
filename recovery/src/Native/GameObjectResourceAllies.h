// Complete resource/allies contracts; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const ModuleField resourceAlliesDistributeEnergyToAlliesBehaviorModuleDataFields[] = {
    { "ComradePercentage", 20u, ModuleValue::Percent, "100.0", nullptr },
};
const ModuleField resourceAlliesTerrainResourceBehaviorModuleDataFields[] = {
    { "Radius", 8u, ModuleValue::Float, "0", nullptr },
    { "MaxIncome", 12u, ModuleValue::Signed, "0", nullptr },
    { "IncomeInterval", 16u, ModuleValue::Unsigned, "0xffffffff", nullptr },
    { "HighPriority", 40u, ModuleValue::Boolean, "false", nullptr },
    { "Visible", 41u, ModuleValue::Boolean, "true", nullptr },
    { "Upgrade", 20u, ModuleValue::Reference, nullptr, nullptr },
    { "UpgradeMustBePresent", 24u, ModuleValue::StringList, nullptr, nullptr },
    { "UpgradeBonusPercent", 32u, ModuleValue::Percent, "100", nullptr },
};
const ModuleFieldSet resourceAlliesFieldSets[] = {
    { resourceAlliesDistributeEnergyToAlliesBehaviorModuleDataFields, 1u },
    { resourceAlliesTerrainResourceBehaviorModuleDataFields, 8u },
};
const ModuleToken resourceAlliesDistributeEnergyToAlliesBehaviorModuleDataTokens[] = {
    { 1u, 4173638251u, 0u, &visionCaptureBehaviorModuleData, 0u, 0u },
    { 3095739879u, 1683514611u, 20u, nullptr, 0u, 0u },
};
const ModuleLayout resourceAlliesDistributeEnergyToAlliesBehaviorModuleData = { resourceAlliesDistributeEnergyToAlliesBehaviorModuleDataTokens, 2u };
const ModuleToken resourceAlliesTerrainResourceBehaviorModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 1328662213u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 2618336920u, 602296504u, 12u, nullptr, 0u, 0u },
    { 583847901u, 1982953380u, 16u, nullptr, 0u, 0u },
    { 2816578890u, 2011428087u, 40u, nullptr, 0u, 0u },
    { 1793489874u, 2011428087u, 41u, nullptr, 0u, 0u },
    { 3619147155u, 1253711657u, 20u, nullptr, 0u, 0u },
    { 280543122u, 1835445784u, 24u, nullptr, 6u, 8u },
    { 2147801636u, 1683514611u, 32u, nullptr, 0u, 0u },
    { 3832456114u, 3832456114u, 36u, nullptr, 7u, 0u },
};
const ModuleLayout resourceAlliesTerrainResourceBehaviorModuleData = { resourceAlliesTerrainResourceBehaviorModuleDataTokens, 10u };
const ModuleChoice resourceAlliesChoices[] = {
    { 251140562u, &resourceAlliesDistributeEnergyToAlliesBehaviorModuleData, 24u, 152u },
    { 1510899067u, &resourceAlliesTerrainResourceBehaviorModuleData, 44u, 153u },
};
