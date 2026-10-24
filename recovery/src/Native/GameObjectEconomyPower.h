// Complete economy/power contracts; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
#include "GameObjectEconomyPowerEnums.h"
const AIEnum economyPowerDeathTypeEnum = { lifecycleDeathNames, 34u, 33, 0u };
const ModuleField economyPowerPowerUpdateModuleDataFields[] = {
    { "UpdatePeriod", 8u, ModuleValue::Time, "1s", nullptr },
    { "ReloadTime", 12u, ModuleValue::Time, "0s", nullptr },
    { "Duration", 16u, ModuleValue::Time, "0s", nullptr },
    { "Type", 20u, ModuleValue::Enum, "INVALID", &economyPowerTypeEnum },
};
const ModuleField economyPowerPlayerPowerManagerBehaviorModuleDataFields[] = {
    { "SpecialPowerTemplate", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "OnAvailableEvaEvent", 12u, ModuleValue::Reference, "", nullptr },
};
const ModuleField economyPowerAutoDepositUpdateModuleDataFields[] = {
    { "DepositInterval", 8u, ModuleValue::Time, "0.0s", nullptr },
    { "GiveNoXP", 44u, ModuleValue::Boolean, "false", nullptr },
    { "OnlyWhenGarrisoned", 45u, ModuleValue::Boolean, "false", nullptr },
    { "Flags", 12u, ModuleValue::Bits, "NONE", &economyPowerFlagsEnum },
    { "TransactionCategory", 16u, ModuleValue::Enum, "AUTO_DEPOSIT", &economyPowerCategoryEnum },
};
const ModuleField economyPowerDistributedMoneyBehaviorModuleDataFields[] = {
    { "Capacity", 8u, ModuleValue::Unsigned, "1", nullptr },
    { "OnDieSpawnPercentage", 12u, ModuleValue::Percent, "1.0", nullptr },
    { "RedistributeMoneyOnDeathType", 16u, ModuleValue::Enum, "SUICIDED", &economyPowerDeathTypeEnum },
};
const ModuleField economyPowerMoneyTransactionFields[] = {
    { "Account", 0u, ModuleValue::Unsigned, nullptr, nullptr },
    { "Amount", 4u, ModuleValue::Unsigned, nullptr, nullptr },
};
const ModuleFieldSet economyPowerFieldSets[] = {
    { economyPowerPowerUpdateModuleDataFields, 4u },
    { economyPowerPlayerPowerManagerBehaviorModuleDataFields, 2u },
    { economyPowerAutoDepositUpdateModuleDataFields, 5u },
    { economyPowerDistributedMoneyBehaviorModuleDataFields, 3u },
    { economyPowerMoneyTransactionFields, 2u },
};
const ModuleToken economyPowerMoneyTransactionTokens[] = {
    { 78324893u, 1982953380u, 0u, nullptr, 0u, 0u },
    { 1343336868u, 1982953380u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout economyPowerMoneyTransaction = { economyPowerMoneyTransactionTokens, 2u };
const ModuleToken economyPowerPowerUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 3829086399u, 2575722984u, 8u, nullptr, 0u, 0u },
    { 779969265u, 2575722984u, 12u, nullptr, 0u, 0u },
    { 2384108347u, 2575722984u, 16u, nullptr, 0u, 0u },
    { 2219907648u, 3773662082u, 20u, nullptr, 0u, 0u },
};
const ModuleLayout economyPowerPowerUpdateModuleData = { economyPowerPowerUpdateModuleDataTokens, 5u };
const ModuleToken economyPowerPlayerPowerManagerBehaviorModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 2178440954u, 2934454408u, 8u, nullptr, 0u, 0u },
    { 2771095072u, 3303818161u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout economyPowerPlayerPowerManagerBehaviorModuleData = { economyPowerPlayerPowerManagerBehaviorModuleDataTokens, 3u };
const ModuleToken economyPowerAutoDepositUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 1529640319u, 2575722984u, 8u, nullptr, 0u, 0u },
    { 1909089401u, 2011428087u, 44u, nullptr, 0u, 0u },
    { 2170485163u, 2011428087u, 45u, nullptr, 0u, 0u },
    { 4081386890u, 3850537025u, 12u, nullptr, 0u, 0u },
    { 2100892608u, 2656434040u, 16u, nullptr, 0u, 0u },
    { 1552590826u, 3070636877u, 20u, &economyPowerMoneyTransaction, 1u, 8u },
    { 2164515782u, 3070636877u, 28u, &economyPowerMoneyTransaction, 1u, 8u },
    { 2602745531u, 3070636877u, 36u, &economyPowerMoneyTransaction, 1u, 8u },
};
const ModuleLayout economyPowerAutoDepositUpdateModuleData = { economyPowerAutoDepositUpdateModuleDataTokens, 9u };
const ModuleToken economyPowerDistributedMoneyBehaviorModuleDataTokens[] = {
    { 1u, 816016269u, 0u, &queueParkingCreateModuleData, 0u, 0u },
    { 3364929761u, 1982953380u, 8u, nullptr, 0u, 0u },
    { 1441621799u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 2731176034u, 3322473782u, 16u, nullptr, 0u, 0u },
};
const ModuleLayout economyPowerDistributedMoneyBehaviorModuleData = { economyPowerDistributedMoneyBehaviorModuleDataTokens, 4u };
const ModuleChoice economyPowerChoices[] = {
    { 4100895620u, &economyPowerPowerUpdateModuleData, 24u, 133u },
    { 771889415u, &economyPowerPlayerPowerManagerBehaviorModuleData, 16u, 134u },
    { 1300449569u, &economyPowerAutoDepositUpdateModuleData, 48u, 135u },
    { 1023139366u, &economyPowerDistributedMoneyBehaviorModuleData, 20u, 136u },
};
