// Complete SlaveWatcher contracts; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const ModuleField crateWatcherSlaveWatcherBehaviorModuleDataFields[] = {
    { "GrantUpgrade", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "RemoveUpgrade", 12u, ModuleValue::Reference, nullptr, nullptr },
    { "ShareUpgrades", 16u, ModuleValue::Boolean, "false", nullptr },
    { "LetSlaveLive", 17u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleFieldSet crateWatcherFieldSets[] = {
    { crateWatcherSlaveWatcherBehaviorModuleDataFields, 4u },
};
const ModuleToken crateWatcherSlaveWatcherBehaviorModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 1289852977u, 1253711657u, 8u, nullptr, 0u, 0u },
    { 2656640899u, 1253711657u, 12u, nullptr, 0u, 0u },
    { 2149919454u, 2011428087u, 16u, nullptr, 0u, 0u },
    { 1292980300u, 2011428087u, 17u, nullptr, 0u, 0u },
};
const ModuleLayout crateWatcherSlaveWatcherBehaviorModuleData = { crateWatcherSlaveWatcherBehaviorModuleDataTokens, 5u };
const ModuleChoice crateWatcherChoices[] = {
    { 3148647452u, &crateWatcherSlaveWatcherBehaviorModuleData, 20u, 127u },
};
