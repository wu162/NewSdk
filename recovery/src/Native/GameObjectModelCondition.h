// Complete model condition contracts; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
#include "GameObjectModelConditionEnums.h"
const ModuleField modelConditionMonitorSpecialPowerTimerUpdateModuleDataFields[] = {
    { "SpecialPowerTemplate", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "ReadyCondition", 12u, ModuleValue::ModelCondition, nullptr, nullptr },
};
const ModuleField modelConditionRandomModelConditionBehaviorModuleDataFields[] = {
    { "Conditions", 8u, ModuleValue::StringList, nullptr, nullptr },
};
const ModuleField modelConditionMonitorConditionUpdateModuleDataFields[] = {
    { "ModelConditionFlags", 8u, ModuleValue::ModelCondition, nullptr, nullptr },
    { "OCL", 68u, ModuleValue::Reference, nullptr, nullptr },
    { "PreDelay", 72u, ModuleValue::Time, nullptr, nullptr },
    { "PostDelay", 76u, ModuleValue::Time, nullptr, nullptr },
    { "ActionsOnTriggered", 80u, ModuleValue::Bits, nullptr, &modelConditionActionEnum },
};
const ModuleFieldSet modelConditionFieldSets[] = {
    { modelConditionMonitorSpecialPowerTimerUpdateModuleDataFields, 2u },
    { modelConditionRandomModelConditionBehaviorModuleDataFields, 1u },
    { modelConditionMonitorConditionUpdateModuleDataFields, 5u },
};
const ModuleToken modelConditionMonitorSpecialPowerTimerUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 2178440954u, 2934454408u, 8u, nullptr, 0u, 0u },
    { 4121333752u, 1742454729u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout modelConditionMonitorSpecialPowerTimerUpdateModuleData = { modelConditionMonitorSpecialPowerTimerUpdateModuleDataTokens, 3u };
const ModuleToken modelConditionRandomModelConditionBehaviorModuleDataTokens[] = {
    { 1u, 816016269u, 0u, &queueParkingCreateModuleData, 0u, 0u },
    { 912300027u, 1835445784u, 8u, nullptr, 6u, 8u },
};
const ModuleLayout modelConditionRandomModelConditionBehaviorModuleData = { modelConditionRandomModelConditionBehaviorModuleDataTokens, 2u };
const ModuleToken modelConditionMonitorConditionUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 1668259568u, 1742454729u, 8u, nullptr, 0u, 0u },
    { 2969981770u, 1122298321u, 68u, nullptr, 0u, 0u },
    { 2125237595u, 2575722984u, 72u, nullptr, 0u, 0u },
    { 915678702u, 2575722984u, 76u, nullptr, 0u, 0u },
    { 2852811503u, 2244743705u, 80u, nullptr, 0u, 0u },
};
const ModuleLayout modelConditionMonitorConditionUpdateModuleData = { modelConditionMonitorConditionUpdateModuleDataTokens, 6u };
const ModuleChoice modelConditionChoices[] = {
    { 2376148578u, &modelConditionMonitorSpecialPowerTimerUpdateModuleData, 72u, 156u },
    { 3721928425u, &modelConditionRandomModelConditionBehaviorModuleData, 16u, 157u },
    { 267133387u, &modelConditionMonitorConditionUpdateModuleData, 84u, 158u },
};
