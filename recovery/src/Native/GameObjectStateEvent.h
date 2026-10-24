// Complete state/event contracts; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const ModuleField stateEventStoreCurrentStateUpdateModuleDataFields[] = {
    { "SleepTime", 8u, ModuleValue::Time, "0s", nullptr },
    { "MaxTime", 12u, ModuleValue::Time, "5s", nullptr },
};
const ModuleField stateEventLUAEventListBehaviorModuleDataFields[] = {
    { "EventListName", 8u, ModuleValue::String, nullptr, nullptr },
};
const ModuleFieldSet stateEventFieldSets[] = {
    { stateEventStoreCurrentStateUpdateModuleDataFields, 2u },
    { stateEventLUAEventListBehaviorModuleDataFields, 1u },
};
const ModuleToken stateEventStoreCurrentStateUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 2918862449u, 2575722984u, 8u, nullptr, 0u, 0u },
    { 4104451904u, 2575722984u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout stateEventStoreCurrentStateUpdateModuleData = { stateEventStoreCurrentStateUpdateModuleDataTokens, 3u };
const ModuleToken stateEventLUAEventListBehaviorModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &dieBehaviorModuleData, 0u, 0u },
    { 97057483u, 438907818u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout stateEventLUAEventListBehaviorModuleData = { stateEventLUAEventListBehaviorModuleDataTokens, 2u };
const ModuleChoice stateEventChoices[] = {
    { 3201389610u, &stateEventStoreCurrentStateUpdateModuleData, 16u, 154u },
    { 4201302198u, &stateEventLUAEventListBehaviorModuleData, 16u, 155u },
};
