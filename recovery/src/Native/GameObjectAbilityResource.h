// Generated complete resource/creation contracts; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const ModuleField abilityResourceTheftFields[] = {
    { "TiberiumStolenPerUpdate", 240u, ModuleValue::Signed, "1", nullptr },
    { "DelayBetweenThefts", 244u, ModuleValue::Time, "0s", nullptr },
    { "TiberiumCapacity", 248u, ModuleValue::Signed, "1", nullptr },
    { "ObjectStatusWhenNotEmpty", 252u, ModuleValue::ObjectStatus, "", nullptr },
    { "ModelConditionWhenNotEmpty", 280u, ModuleValue::ModelCondition, nullptr, nullptr },
    { "PlayWeaponPreFireFX", 340u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleToken abilityResourceTiberiumThiefSpecialAbilityUpdateModuleDataTokens[] = {
    { 1u, 2891550594u, 0u, &abilitySpecialAbilityUpdateModuleData, 0u, 0u },
    { 3118654258u, 602296504u, 240u, nullptr, 0u, 0u },
    { 127895364u, 2575722984u, 244u, nullptr, 0u, 0u },
    { 3944684671u, 602296504u, 248u, nullptr, 0u, 0u },
    { 2728899577u, 2814214430u, 252u, nullptr, 0u, 0u },
    { 247625548u, 1742454729u, 280u, nullptr, 0u, 0u },
    { 1401519719u, 2011428087u, 340u, nullptr, 0u, 0u },
};
const ModuleLayout abilityResourceTiberiumThiefSpecialAbilityUpdateModuleData = { abilityResourceTiberiumThiefSpecialAbilityUpdateModuleDataTokens, 7u };
const ModuleToken abilityResourceCreateAndEnterObjectSpecialPowerModuleDataTokens[] = {
    { 1u, 2308556452u, 0u, &powerCreationOCLSpecialPowerModuleData, 0u, 0u },
    { 948911813u, 691437687u, 500u, nullptr, 2u, 0u },
};
const ModuleLayout abilityResourceCreateAndEnterObjectSpecialPowerModuleData = { abilityResourceCreateAndEnterObjectSpecialPowerModuleDataTokens, 2u };
const ModuleChoice abilityResourceChoices[] = {
    { 4099311701u, &abilityResourceTiberiumThiefSpecialAbilityUpdateModuleData, 344u, 213u },
    { 952407768u, &abilityResourceCreateAndEnterObjectSpecialPowerModuleData, 504u, 214u },
};
