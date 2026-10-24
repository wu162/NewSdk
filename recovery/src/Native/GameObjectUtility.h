// Complete scalar/string utilities; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const ModuleField utilityStorePurchasedUpgradeBehaviorModuleDataFields[] = {
    { "MaxPlayerUpgrades", 8u, ModuleValue::Unsigned, "9999", nullptr },
};
const ModuleToken utilityStorePurchasedUpgradeBehaviorModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &lifecycleBehaviorModuleData, 0u, 0u },
    { 2122048096u, 1982953380u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout utilityStorePurchasedUpgradeBehaviorModuleData = { utilityStorePurchasedUpgradeBehaviorModuleDataTokens, 2u };
const ModuleField utilityAIGateUpdateModuleDataFields[] = {
    { "TriggerWidthX", 8u, ModuleValue::Float, "0", nullptr },
    { "TriggerWidthY", 12u, ModuleValue::Float, "0", nullptr },
};
const ModuleToken utilityAIGateUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 3580059422u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 3615748891u, 1683514611u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout utilityAIGateUpdateModuleData = { utilityAIGateUpdateModuleDataTokens, 3u };
const ModuleField utilityCombinedInfoModuleDataFields[] = {
    { "DockingBone", 8u, ModuleValue::String, nullptr, nullptr },
};
const ModuleToken utilityCombinedInfoModuleDataTokens[] = {
    { 1u, 816016269u, 0u, &createCreateModuleData, 0u, 0u },
    { 1589100344u, 438907818u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout utilityCombinedInfoModuleData = { utilityCombinedInfoModuleDataTokens, 2u };
const ModuleField utilityTimedOpacityUpdateModuleDataFields[] = {
    { "Opacity", 8u, ModuleValue::Float, "0.5", nullptr },
    { "Duration", 12u, ModuleValue::Time, "5s", nullptr },
};
const ModuleToken utilityTimedOpacityUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 2688997143u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 2384108347u, 2575722984u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout utilityTimedOpacityUpdateModuleData = { utilityTimedOpacityUpdateModuleDataTokens, 3u };
const ModuleFieldSet utilityFieldSets[] = {
    { utilityStorePurchasedUpgradeBehaviorModuleDataFields, 1u },
    { utilityAIGateUpdateModuleDataFields, 2u },
    { utilityCombinedInfoModuleDataFields, 1u },
    { utilityTimedOpacityUpdateModuleDataFields, 2u },
};
const ModuleChoice utilityChoices[] = {
    { 783731100u, &utilityStorePurchasedUpgradeBehaviorModuleData, 12u, 225u },
    { 880602396u, &utilityAIGateUpdateModuleData, 16u, 226u },
    { 2018429368u, &utilityCombinedInfoModuleData, 16u, 227u },
    { 4216826759u, &utilityTimedOpacityUpdateModuleData, 16u, 228u },
};
