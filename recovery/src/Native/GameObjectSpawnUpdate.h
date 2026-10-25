// Complete Spawn/OCL update contracts; original Tokenizer blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
// OCLMonitor names must come from the original PE table; no schema-derived ordering.
#include "GameObjectSpawnEnums.h"
const AIEnum spawnDamageTypeEnum = { aiDamageTypeNames, 39u, 38, 39u };
const ModuleField spawnSpawnBehaviorFields[] = {
    { "SpawnNumberData", 36u, ModuleValue::Signed, "0", nullptr },
    { "SpawnReplaceDelayData", 40u, ModuleValue::Time, "0s", nullptr },
    { "InitialBurst", 44u, ModuleValue::Signed, "0", nullptr },
    { "IsOneShotData", 76u, ModuleValue::Boolean, "false", nullptr },
    { "CanReclaimOrphans", 77u, ModuleValue::Boolean, "false", nullptr },
    { "AggregateHealth", 78u, ModuleValue::Boolean, "false", nullptr },
    { "ExitByBudding", 79u, ModuleValue::Boolean, "false", nullptr },
    { "SpawnedRequireSpawner", 80u, ModuleValue::Boolean, "false", nullptr },
    { "RespectCommandLimit", 81u, ModuleValue::Boolean, "false", nullptr },
    { "DamageTypesToPropagateToSlaves", 48u, ModuleValue::Bits, "ALL", &spawnDamageTypeEnum },
    { "FadeInTime", 56u, ModuleValue::Time, "0s", nullptr },
    { "KillSpawnBasedOnModelConditionState", 82u, ModuleValue::Boolean, "false", nullptr },
    { "ShareUpgrades", 83u, ModuleValue::Boolean, "false", nullptr },
    { "SpawnInsideBuilding", 84u, ModuleValue::Boolean, "false", nullptr },
    { "InitialDelay", 60u, ModuleValue::Time, "0s", nullptr },
    { "CombineOnCreate", 85u, ModuleValue::Boolean, "false", nullptr },
    { "KillSpawnsOnDisabled", 86u, ModuleValue::Boolean, "false", nullptr },
    { "PassExperienceToSpawned", 87u, ModuleValue::Boolean, "false", nullptr },
    { "KillSpawnsOnCaptured", 88u, ModuleValue::Boolean, "false", nullptr },
    { "SpawnOnUnpack", 89u, ModuleValue::Boolean, "false", nullptr },
    { "SetPurchasePrice", 90u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleField spawnOCLUpdateFields[] = {
    { "OCL", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "MinDelay", 12u, ModuleValue::Duration, "0", nullptr },
    { "MaxDelay", 16u, ModuleValue::Duration, "0", nullptr },
    { "CreateAtEdge", 24u, ModuleValue::Boolean, "false", nullptr },
    { "Amount", 20u, ModuleValue::Signed, "-1", nullptr },
};
const ModuleField spawnOCLMonitorUpdateFields[] = {
    { "Options", 8u, ModuleValue::Bits, nullptr, &spawnMonitorOptionEnum },
    { "RequiredMasterObjectStatus", 12u, ModuleValue::ObjectStatus, nullptr, nullptr },
};
const ModuleFieldSet spawnFieldSets[] = {
    { spawnSpawnBehaviorFields, 21u },
    { spawnOCLUpdateFields, 5u },
    { spawnOCLMonitorUpdateFields, 2u },
};
const ModuleToken spawnContainModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &dieBehaviorModuleData, 0u, 0u },
};
const ModuleLayout spawnContainModuleData = { spawnContainModuleDataTokens, 1u };
const ModuleToken spawnUpdateModuleDataTokens[] = {
    { 1u, 687874333u, 0u, &spawnContainModuleData, 0u, 0u },
};
const ModuleLayout spawnUpdateModuleData = { spawnUpdateModuleDataTokens, 1u };
const ModuleToken spawnSpawnBehaviorTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 1071469225u, 602296504u, 36u, nullptr, 0u, 0u },
    { 196055136u, 2575722984u, 40u, nullptr, 0u, 0u },
    { 2318887697u, 602296504u, 44u, nullptr, 0u, 0u },
    { 2080927055u, 2011428087u, 76u, nullptr, 0u, 0u },
    { 678498620u, 2011428087u, 77u, nullptr, 0u, 0u },
    { 2180741996u, 2011428087u, 78u, nullptr, 0u, 0u },
    { 562085248u, 2011428087u, 79u, nullptr, 0u, 0u },
    { 1500265397u, 2011428087u, 80u, nullptr, 0u, 0u },
    { 3030608410u, 2011428087u, 81u, nullptr, 0u, 0u },
    { 2468572256u, 133869220u, 48u, nullptr, 0u, 0u },
    { 4289579894u, 2575722984u, 56u, nullptr, 0u, 0u },
    { 93596315u, 2011428087u, 82u, nullptr, 0u, 0u },
    { 2149919454u, 2011428087u, 83u, nullptr, 0u, 0u },
    { 1284888005u, 2011428087u, 84u, nullptr, 0u, 0u },
    { 2586086522u, 2575722984u, 60u, nullptr, 0u, 0u },
    { 2682316434u, 2011428087u, 85u, nullptr, 0u, 0u },
    { 2648361451u, 2011428087u, 86u, nullptr, 0u, 0u },
    { 1640802174u, 2011428087u, 87u, nullptr, 0u, 0u },
    { 4006929090u, 2011428087u, 88u, nullptr, 0u, 0u },
    { 118088827u, 2011428087u, 89u, nullptr, 0u, 0u },
    { 2975313353u, 2011428087u, 90u, nullptr, 0u, 0u },
    { 1309478125u, 8650779u, 64u, &dieDieMuxDataType, 4u, 0u },
    { 1528957511u, 344905565u, 68u, nullptr, 3u, 4u },
};
const ModuleLayout spawnSpawnBehavior = { spawnSpawnBehaviorTokens, 24u };
const ModuleToken spawnOCLUpdateTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 2969981770u, 1122298321u, 8u, nullptr, 0u, 0u },
    { 162863551u, 1869421287u, 12u, nullptr, 0u, 0u },
    { 1148254404u, 1869421287u, 16u, nullptr, 0u, 0u },
    { 796369701u, 2011428087u, 24u, nullptr, 0u, 0u },
    { 1343336868u, 602296504u, 20u, nullptr, 0u, 0u },
};
const ModuleLayout spawnOCLUpdate = { spawnOCLUpdateTokens, 6u };
const ModuleToken spawnOCLMonitorUpdateTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 3202187060u, 132232580u, 8u, nullptr, 0u, 0u },
    { 1774733260u, 2814214430u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout spawnOCLMonitorUpdate = { spawnOCLMonitorUpdateTokens, 3u };
const ModuleChoice spawnChoices[] = {
    { 227633887u, &spawnSpawnBehavior, 92u, 123u },
    { 3098903621u, &spawnOCLUpdate, 28u, 124u },
    { 4047873639u, &spawnOCLMonitorUpdate, 40u, 125u },
};
