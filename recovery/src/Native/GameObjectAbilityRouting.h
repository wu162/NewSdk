// Generated complete routing/radar SpecialAbility contracts; fixed original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const ModuleField abilityRoutingKindOfAndStatusAndModelConditionTypeFields[] = {
    { "ObjectStatus", 0u, ModuleValue::ObjectStatus, nullptr, nullptr },
    { "ModelCondition", 28u, ModuleValue::ModelCondition, nullptr, nullptr },
    { "KindOf", 88u, ModuleValue::Bits, nullptr, &aiKindOfType },
    { "UseLocalPlayerFilter", 124u, ModuleValue::Boolean, "true", nullptr },
};
const ModuleField abilityRoutingSpecialPowerModuleInfoFields[] = {
    { "ModuleId", 0u, ModuleValue::AssetId, nullptr, nullptr },
    { "TriggerAtTargetPosition", 8u, ModuleValue::Boolean, "true", nullptr },
    { "TriggerType", 4u, ModuleValue::Enum, "TARGET_POSITION", &powerRoutingTrigger },
};
const ModuleToken abilityRoutingSpecialPowerModuleInfoTokens[] = {
    { 3789524821u, 3186653541u, 0u, nullptr, 0u, 0u },
    { 3963059545u, 2011428087u, 8u, nullptr, 0u, 0u },
    { 29598626u, 29598626u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout abilityRoutingSpecialPowerModuleInfo = { abilityRoutingSpecialPowerModuleInfoTokens, 3u };
const ModuleToken abilityRoutingKindOfAndStatusAndModelConditionTypeTokens[] = {
    { 3963816762u, 2814214430u, 0u, nullptr, 0u, 0u },
    { 3468811896u, 1742454729u, 28u, nullptr, 0u, 0u },
    { 4185812704u, 2600243603u, 88u, nullptr, 0u, 0u },
    { 1120639023u, 2011428087u, 124u, nullptr, 0u, 0u },
};
const ModuleLayout abilityRoutingKindOfAndStatusAndModelConditionType = { abilityRoutingKindOfAndStatusAndModelConditionTypeTokens, 4u };
const ModuleToken abilityRoutingActivateModuleSpecialAbilityUpdateModuleDataTokens[] = {
    { 1u, 2891550594u, 0u, &abilitySpecialAbilityUpdateModuleData, 0u, 0u },
    { 2206287502u, 2913291561u, 240u, &abilityRoutingSpecialPowerModuleInfo, 1u, 12u },
};
const ModuleLayout abilityRoutingActivateModuleSpecialAbilityUpdateModuleData = { abilityRoutingActivateModuleSpecialAbilityUpdateModuleDataTokens, 2u };
const ModuleToken abilityRoutingRadarScanMapSpecialAbilityUpdateModuleDataTokens[] = {
    { 1u, 2891550594u, 0u, &abilitySpecialAbilityUpdateModuleData, 0u, 0u },
    { 4249583194u, 1766435192u, 240u, &abilityRoutingKindOfAndStatusAndModelConditionType, 1u, 128u },
};
const ModuleLayout abilityRoutingRadarScanMapSpecialAbilityUpdateModuleData = { abilityRoutingRadarScanMapSpecialAbilityUpdateModuleDataTokens, 2u };
const ModuleChoice abilityRoutingChoices[] = {
    { 3442719982u, &abilityRoutingActivateModuleSpecialAbilityUpdateModuleData, 248u, 211u },
    { 3660533477u, &abilityRoutingRadarScanMapSpecialAbilityUpdateModuleData, 248u, 212u },
};
