// Complete combat response contracts; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const ModuleField combatResponseFindCoverBehaviorModuleDataFields[] = {
    { "AttributeModifierInCover", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "CoverScanRange", 12u, ModuleValue::Float, nullptr, nullptr },
};
const ModuleField combatResponseNotifyWhenAttackingBehaviorModuleDataFields[] = {
    { "NotifyRadius", 8u, ModuleValue::Float, nullptr, nullptr },
    { "AssistResponseModuleId", 12u, ModuleValue::AssetId, nullptr, nullptr },
    { "SleepTime", 16u, ModuleValue::Time, "0s", nullptr },
    { "HasMobileFriends", 36u, ModuleValue::Boolean, "false", nullptr },
    { "IgnoreDisabledStatus", 37u, ModuleValue::Boolean, "false", nullptr },
    { "MaxFriendsToNotify", 20u, ModuleValue::Signed, "100", nullptr },
    { "WeaponSlotID", 24u, ModuleValue::Unsigned, "1", nullptr },
    { "WeaponSlotType", 28u, ModuleValue::Enum, "PRIMARY_WEAPON", &assistWeaponSlotTypeEnum },
};
const ModuleField combatResponseHitReactionBehaviorModuleDataFields[] = {
    { "LightDamageHitReactionLifeTimer", 8u, ModuleValue::Time, "0s", nullptr },
    { "MediumDamageHitReactionLifeTimer", 12u, ModuleValue::Time, "0s", nullptr },
    { "HeavyDamageHitReactionLifeTimer", 16u, ModuleValue::Time, "0s", nullptr },
    { "LightDamageHitReactionThreshold", 20u, ModuleValue::Float, "0", nullptr },
    { "MediumDamageHitReactionThreshold", 24u, ModuleValue::Float, "0", nullptr },
    { "HeavyDamageHitReactionThreshold", 28u, ModuleValue::Float, "0", nullptr },
    { "RotateToSourceWhenDamaged", 44u, ModuleValue::Boolean, "false", nullptr },
    { "RotateToSourceIgnoreIdleCheck", 45u, ModuleValue::Boolean, "false", nullptr },
    { "GoIdleWhenDamaged", 46u, ModuleValue::Boolean, "false", nullptr },
    { "RequiredDeathTypes", 32u, ModuleValue::DeathBits, nullptr, nullptr },
};
const ModuleFieldSet combatResponseFieldSets[] = {
    { combatResponseFindCoverBehaviorModuleDataFields, 2u },
    { combatResponseNotifyWhenAttackingBehaviorModuleDataFields, 8u },
    { combatResponseHitReactionBehaviorModuleDataFields, 10u },
};
const ModuleToken combatResponseFindCoverBehaviorModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 1996641789u, 165531635u, 8u, nullptr, 0u, 0u },
    { 4157055945u, 1683514611u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout combatResponseFindCoverBehaviorModuleData = { combatResponseFindCoverBehaviorModuleDataTokens, 3u };
const ModuleToken combatResponseNotifyWhenAttackingBehaviorModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 139270466u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 3795982897u, 3186653541u, 12u, nullptr, 0u, 0u },
    { 2918862449u, 2575722984u, 16u, nullptr, 0u, 0u },
    { 1146660936u, 2011428087u, 36u, nullptr, 0u, 0u },
    { 415810163u, 2011428087u, 37u, nullptr, 0u, 0u },
    { 4059274927u, 602296504u, 20u, nullptr, 0u, 0u },
    { 1579523250u, 1982953380u, 24u, nullptr, 0u, 0u },
    { 407422281u, 407422281u, 28u, nullptr, 0u, 0u },
    { 3064842590u, 3832456114u, 32u, nullptr, 7u, 0u },
};
const ModuleLayout combatResponseNotifyWhenAttackingBehaviorModuleData = { combatResponseNotifyWhenAttackingBehaviorModuleDataTokens, 10u };
const ModuleToken combatResponseHitReactionBehaviorModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 3411836437u, 2575722984u, 8u, nullptr, 0u, 0u },
    { 3935634884u, 2575722984u, 12u, nullptr, 0u, 0u },
    { 1263093264u, 2575722984u, 16u, nullptr, 0u, 0u },
    { 3580394746u, 1683514611u, 20u, nullptr, 0u, 0u },
    { 263756345u, 1683514611u, 24u, nullptr, 0u, 0u },
    { 3168928427u, 1683514611u, 28u, nullptr, 0u, 0u },
    { 204123226u, 2011428087u, 44u, nullptr, 0u, 0u },
    { 4260711086u, 2011428087u, 45u, nullptr, 0u, 0u },
    { 2512803067u, 2011428087u, 46u, nullptr, 0u, 0u },
    { 2667496235u, 55439777u, 32u, nullptr, 0u, 0u },
    { 3933465695u, 1049596812u, 40u, &lifecycleObjectStatusValidationDataType, 4u, 0u },
};
const ModuleLayout combatResponseHitReactionBehaviorModuleData = { combatResponseHitReactionBehaviorModuleDataTokens, 12u };
const ModuleChoice combatResponseChoices[] = {
    { 3199719331u, &combatResponseFindCoverBehaviorModuleData, 16u, 140u },
    { 50199012u, &combatResponseNotifyWhenAttackingBehaviorModuleData, 40u, 141u },
    { 1506879888u, &combatResponseHitReactionBehaviorModuleData, 48u, 142u },
};
