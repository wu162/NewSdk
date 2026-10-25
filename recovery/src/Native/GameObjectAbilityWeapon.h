// Generated complete weapon/beam/status SpecialAbility derivations; fixed original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const ModuleField abilityWeaponFields0[] = {
    { "SpecialWeapon", 240u, ModuleValue::Reference, nullptr, nullptr },
    { "WhichSpecialWeapon", 244u, ModuleValue::Signed, "0", nullptr },
    { "SkipContinue", 264u, ModuleValue::Boolean, "false", nullptr },
    { "BusyForDuration", 248u, ModuleValue::Time, "0s", nullptr },
    { "NeedLivingTargets", 265u, ModuleValue::Boolean, "false", nullptr },
    { "PlayWeaponPreFireFX", 266u, ModuleValue::Boolean, "false", nullptr },
    { "OffsetTargetLocationFromObject", 267u, ModuleValue::Boolean, "false", nullptr },
    { "WeaponSlotID", 252u, ModuleValue::Signed, "1", nullptr },
    { "WeaponSlotType", 256u, ModuleValue::Enum, "PRIMARY_WEAPON", &weaponSetWeaponSlotTypeEnum },
    { "DestealthClosestObjectMatchingFilter", 268u, ModuleValue::Boolean, "false", nullptr },
    { "ModelConditionsOnUnpack", 260u, ModuleValue::Enum, "INVALID", &aiModelConditionFlagType },
};
const ModuleToken abilityWeaponWeaponFireSpecialAbilityUpdateModuleDataTokens[] = {
    { 1u, 2891550594u, 0u, &abilitySpecialAbilityUpdateModuleData, 0u, 0u },
    { 1696919238u, 3494305058u, 240u, nullptr, 0u, 0u },
    { 1445051076u, 602296504u, 244u, nullptr, 0u, 0u },
    { 2102441447u, 2011428087u, 264u, nullptr, 0u, 0u },
    { 3051957251u, 2575722984u, 248u, nullptr, 0u, 0u },
    { 4267032782u, 2011428087u, 265u, nullptr, 0u, 0u },
    { 1401519719u, 2011428087u, 266u, nullptr, 0u, 0u },
    { 3902409127u, 2011428087u, 267u, nullptr, 0u, 0u },
    { 1579523250u, 602296504u, 252u, nullptr, 0u, 0u },
    { 407422281u, 407422281u, 256u, nullptr, 0u, 0u },
    { 3838544164u, 2011428087u, 268u, nullptr, 0u, 0u },
    { 245062075u, 359808523u, 260u, nullptr, 0u, 0u },
};
const ModuleLayout abilityWeaponWeaponFireSpecialAbilityUpdateModuleData = { abilityWeaponWeaponFireSpecialAbilityUpdateModuleDataTokens, 12u };
const ModuleField abilityWeaponFields1[] = {
    { "FireDelay", 272u, ModuleValue::Time, "0s", nullptr },
};
const ModuleToken abilityWeaponDelayedWeaponFireSpecialAbilityUpdateModuleDataTokens[] = {
    { 1u, 507880339u, 0u, &abilityWeaponWeaponFireSpecialAbilityUpdateModuleData, 0u, 0u },
    { 310886179u, 2575722984u, 272u, nullptr, 0u, 0u },
};
const ModuleLayout abilityWeaponDelayedWeaponFireSpecialAbilityUpdateModuleData = { abilityWeaponDelayedWeaponFireSpecialAbilityUpdateModuleDataTokens, 2u };
const ModuleField abilityWeaponFields2[] = {
    { "DamagePerSecond", 240u, ModuleValue::Float, nullptr, nullptr },
    { "JoinWithOtherBeams", 268u, ModuleValue::Boolean, "false", nullptr },
    { "DissapateWhenTargetDead", 269u, ModuleValue::Boolean, "true", nullptr },
    { "LineType", 270u, ModuleValue::Boolean, "false", nullptr },
    { "TargetAttributeModifierAdd", 244u, ModuleValue::Reference, nullptr, nullptr },
    { "TargetObjectStatusAdd", 248u, ModuleValue::NullableObjectStatus, nullptr, nullptr },
    { "SweepFX", 252u, ModuleValue::Reference, nullptr, nullptr },
    { "ReflectorExtendDistance", 256u, ModuleValue::Float, "0", nullptr },
    { "TargetSamePlayerOnly", 271u, ModuleValue::Boolean, "false", nullptr },
    { "PreferredTargetBone", 260u, ModuleValue::String, nullptr, nullptr },
};
const ModuleToken abilityWeaponBeamSpecialAbilityUpdateModuleDataTokens[] = {
    { 1u, 2891550594u, 0u, &abilitySpecialAbilityUpdateModuleData, 0u, 0u },
    { 588933898u, 1683514611u, 240u, nullptr, 0u, 0u },
    { 3792978182u, 2011428087u, 268u, nullptr, 0u, 0u },
    { 2343499938u, 2011428087u, 269u, nullptr, 0u, 0u },
    { 2886112425u, 2011428087u, 270u, nullptr, 0u, 0u },
    { 2382798884u, 165531635u, 244u, nullptr, 0u, 0u },
    { 3948109203u, 2814214430u, 248u, nullptr, 2u, 0u },
    { 2929450270u, 219975299u, 252u, nullptr, 0u, 0u },
    { 1505633815u, 1683514611u, 256u, nullptr, 0u, 0u },
    { 3649857539u, 2011428087u, 271u, nullptr, 0u, 0u },
    { 1284714290u, 438907818u, 260u, nullptr, 0u, 0u },
};
const ModuleLayout abilityWeaponBeamSpecialAbilityUpdateModuleData = { abilityWeaponBeamSpecialAbilityUpdateModuleDataTokens, 11u };
const ModuleField abilityWeaponFields3[] = {
    { "Status1", 240u, ModuleValue::ObjectStatus, nullptr, nullptr },
    { "Status2", 268u, ModuleValue::ObjectStatus, nullptr, nullptr },
};
const ModuleToken abilityWeaponObjectStatusSpecialAbilityUpdateModuleDataTokens[] = {
    { 1u, 2891550594u, 0u, &abilitySpecialAbilityUpdateModuleData, 0u, 0u },
    { 311101648u, 2814214430u, 240u, nullptr, 0u, 0u },
    { 2507823137u, 2814214430u, 268u, nullptr, 0u, 0u },
};
const ModuleLayout abilityWeaponObjectStatusSpecialAbilityUpdateModuleData = { abilityWeaponObjectStatusSpecialAbilityUpdateModuleDataTokens, 3u };
const ModuleFieldSet abilityWeaponFieldSets[] = {
    { abilityWeaponFields0, 11u },
    { abilityWeaponFields1, 1u },
    { abilityWeaponFields2, 10u },
    { abilityWeaponFields3, 2u },
};
const ModuleChoice abilityWeaponChoices[] = {
    { 507880339u, &abilityWeaponWeaponFireSpecialAbilityUpdateModuleData, 272u, 194u },
    { 915350037u, &abilityWeaponDelayedWeaponFireSpecialAbilityUpdateModuleData, 276u, 195u },
    { 2695497697u, &abilityWeaponBeamSpecialAbilityUpdateModuleData, 272u, 196u },
    { 127626925u, &abilityWeaponObjectStatusSpecialAbilityUpdateModuleData, 296u, 197u },
};
