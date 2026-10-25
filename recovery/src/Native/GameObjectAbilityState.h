// Generated complete state/object SpecialAbility derivations; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const EnumNameValue abilityStateReplaceOptionsNames[] = {
    { "CHECK_BUILD_ASSISTANT", 4 },
    { "CLEAR_LOCATION", 6 },
    { "COPY_TARGET_OBJECT", 0 },
    { "COPY_UPGRADES", 2 },
    { "DISABLE_DURING_REPLACE", 3 },
    { "DISABLE_NEW_OBJECT_DURING_UNPACK", 5 },
    { "IGNORE_TERRAIN_RESTRICTIONS", 1 },
};
const AIEnum abilityStateReplaceOptions = { abilityStateReplaceOptionsNames, 7u, 4, 7u };
const ModuleField abilityStateFields0[] = {
    { "CommandButton", 240u, ModuleValue::String, nullptr, nullptr },
    { "UpgradeToGive", 248u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleToken abilityStateGiveOrRestoreUpgradeSpecialPowerModuleDataTokens[] = {
    { 1u, 2891550594u, 0u, &abilitySpecialAbilityUpdateModuleData, 0u, 0u },
    { 1968412924u, 438907818u, 240u, nullptr, 0u, 0u },
    { 3410076785u, 1253711657u, 248u, nullptr, 0u, 0u },
};
const ModuleLayout abilityStateGiveOrRestoreUpgradeSpecialPowerModuleData = { abilityStateGiveOrRestoreUpgradeSpecialPowerModuleDataTokens, 3u };
const ModuleField abilityStateFields1[] = {
    { "UpdateModuleId", 240u, ModuleValue::AssetId, nullptr, nullptr },
    { "TimeStepsBack", 244u, ModuleValue::Time, "0s", nullptr },
    { "LeavePositionFX", 248u, ModuleValue::Reference, nullptr, nullptr },
    { "ReturnPositionFX", 252u, ModuleValue::Reference, nullptr, nullptr },
    { "PostReconstituteAttributeModifier", 256u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleToken abilityStateReconstituteStateSpecialAbilityModuleDataTokens[] = {
    { 1u, 2891550594u, 0u, &abilitySpecialAbilityUpdateModuleData, 0u, 0u },
    { 3665496556u, 3186653541u, 240u, nullptr, 0u, 0u },
    { 1613480919u, 2575722984u, 244u, nullptr, 0u, 0u },
    { 1101181649u, 219975299u, 248u, nullptr, 0u, 0u },
    { 3660416174u, 219975299u, 252u, nullptr, 0u, 0u },
    { 1178455462u, 165531635u, 256u, nullptr, 0u, 0u },
};
const ModuleLayout abilityStateReconstituteStateSpecialAbilityModuleData = { abilityStateReconstituteStateSpecialAbilityModuleDataTokens, 6u };
const ModuleField abilityStateFields2[] = {
    { "DominateRadius", 240u, ModuleValue::Float, nullptr, nullptr },
    { "TriggerFX", 244u, ModuleValue::Reference, nullptr, nullptr },
    { "DominatedFX", 248u, ModuleValue::Reference, nullptr, nullptr },
    { "PermanentlyConvert", 256u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleToken abilityStateDominateEnemySpecialPowerModuleDataTokens[] = {
    { 1u, 2891550594u, 0u, &abilitySpecialAbilityUpdateModuleData, 0u, 0u },
    { 1060381965u, 1683514611u, 240u, nullptr, 0u, 0u },
    { 1761895055u, 219975299u, 244u, nullptr, 0u, 0u },
    { 1867742706u, 219975299u, 248u, nullptr, 0u, 0u },
    { 3256708905u, 2011428087u, 256u, nullptr, 0u, 0u },
    { 2002479074u, 3832456114u, 252u, nullptr, 7u, 0u },
};
const ModuleLayout abilityStateDominateEnemySpecialPowerModuleData = { abilityStateDominateEnemySpecialPowerModuleDataTokens, 6u };
const ModuleField abilityStateFields3[] = {
    { "Experience", 240u, ModuleValue::Signed, "0", nullptr },
    { "RadiusEffect", 244u, ModuleValue::Float, "0", nullptr },
    { "LevelFX", 248u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleToken abilityStateLevelGrantSpecialPowerModuleDataTokens[] = {
    { 1u, 2891550594u, 0u, &abilitySpecialAbilityUpdateModuleData, 0u, 0u },
    { 4023319006u, 602296504u, 240u, nullptr, 0u, 0u },
    { 2749472323u, 1683514611u, 244u, nullptr, 0u, 0u },
    { 2729095677u, 219975299u, 248u, nullptr, 0u, 0u },
    { 1500580722u, 3832456114u, 252u, nullptr, 7u, 0u },
};
const ModuleLayout abilityStateLevelGrantSpecialPowerModuleData = { abilityStateLevelGrantSpecialPowerModuleDataTokens, 5u };
const ModuleField abilityStateFields4[] = {
    { "MinDestinationRadius", 240u, ModuleValue::Float, "0", nullptr },
    { "MaxDestinationRadius", 244u, ModuleValue::Float, "0", nullptr },
    { "TriggerAttributeModifierOnClones", 248u, ModuleValue::Reference, nullptr, nullptr },
    { "TriggerFX", 252u, ModuleValue::Reference, nullptr, nullptr },
    { "TargetFX", 256u, ModuleValue::Reference, nullptr, nullptr },
    { "ClonedObjectLifetime", 260u, ModuleValue::Time, "0s", nullptr },
};
const ModuleToken abilityStateCloneStoredObjectsSpecialPowerUpdateModuleDataTokens[] = {
    { 1u, 2891550594u, 0u, &abilitySpecialAbilityUpdateModuleData, 0u, 0u },
    { 1324986770u, 1683514611u, 240u, nullptr, 0u, 0u },
    { 3385955927u, 1683514611u, 244u, nullptr, 0u, 0u },
    { 1958938660u, 165531635u, 248u, nullptr, 0u, 0u },
    { 1761895055u, 219975299u, 252u, nullptr, 0u, 0u },
    { 1980966568u, 219975299u, 256u, nullptr, 0u, 0u },
    { 35687126u, 2575722984u, 260u, nullptr, 0u, 0u },
};
const ModuleLayout abilityStateCloneStoredObjectsSpecialPowerUpdateModuleData = { abilityStateCloneStoredObjectsSpecialPowerUpdateModuleDataTokens, 7u };
const ModuleField abilityStateFields5[] = {
    { "NewObjectUnpackTime", 240u, ModuleValue::Time, "0s", nullptr },
    { "ReplaceOptions", 244u, ModuleValue::Bits, "DISABLE_DURING_REPLACE", &abilityStateReplaceOptions },
    { "ClearTriggerDistance", 248u, ModuleValue::Float, "120.0", nullptr },
};
const ModuleToken abilityStateReplaceSelfSpecialAbilityModuleDataTokens[] = {
    { 1u, 2891550594u, 0u, &abilitySpecialAbilityUpdateModuleData, 0u, 0u },
    { 4207563660u, 2575722984u, 240u, nullptr, 0u, 0u },
    { 1876817173u, 1666784442u, 244u, nullptr, 0u, 0u },
    { 2520443478u, 1683514611u, 248u, nullptr, 0u, 0u },
    { 1160307140u, 344905565u, 252u, nullptr, 3u, 4u },
};
const ModuleLayout abilityStateReplaceSelfSpecialAbilityModuleData = { abilityStateReplaceSelfSpecialAbilityModuleDataTokens, 5u };
const ModuleField abilityStateFields6[] = {
    { "NewObjectAssemblyTime", 240u, ModuleValue::Time, "0s", nullptr },
    { "BusyDuringAssembly", 256u, ModuleValue::Boolean, "false", nullptr },
    { "ModelConditionsOnAssembly", 244u, ModuleValue::Enum, "INVALID", &aiModelConditionFlagType },
};
const ModuleToken abilityStateAssembleStructureSpecialAbilityUpdateModuleDataTokens[] = {
    { 1u, 2891550594u, 0u, &abilitySpecialAbilityUpdateModuleData, 0u, 0u },
    { 3816502312u, 2575722984u, 240u, nullptr, 0u, 0u },
    { 1029123441u, 2011428087u, 256u, nullptr, 0u, 0u },
    { 3941165731u, 359808523u, 244u, nullptr, 0u, 0u },
    { 948596956u, 344905565u, 248u, nullptr, 3u, 4u },
};
const ModuleLayout abilityStateAssembleStructureSpecialAbilityUpdateModuleData = { abilityStateAssembleStructureSpecialAbilityUpdateModuleDataTokens, 5u };
const ModuleFieldSet abilityStateFieldSets[] = {
    { abilityStateFields0, 2u },
    { abilityStateFields1, 5u },
    { abilityStateFields2, 4u },
    { abilityStateFields3, 3u },
    { abilityStateFields4, 6u },
    { abilityStateFields5, 3u },
    { abilityStateFields6, 3u },
};
const ModuleChoice abilityStateChoices[] = {
    { 72607304u, &abilityStateGiveOrRestoreUpgradeSpecialPowerModuleData, 252u, 204u },
    { 1067163377u, &abilityStateReconstituteStateSpecialAbilityModuleData, 260u, 205u },
    { 2603081264u, &abilityStateDominateEnemySpecialPowerModuleData, 260u, 206u },
    { 2431669529u, &abilityStateLevelGrantSpecialPowerModuleData, 256u, 207u },
    { 3455002493u, &abilityStateCloneStoredObjectsSpecialPowerUpdateModuleData, 264u, 208u },
    { 2434933121u, &abilityStateReplaceSelfSpecialAbilityModuleData, 260u, 209u },
    { 2000137321u, &abilityStateAssembleStructureSpecialAbilityUpdateModuleData, 260u, 210u },
};
