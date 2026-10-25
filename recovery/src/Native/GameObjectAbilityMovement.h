// Generated complete movement/teleport SpecialAbility derivations; fixed original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const EnumNameValue abilityMovementLocomotorNames[] = {
    { "ATTACKING", 12 },
    { "BURNINGDEATH", 16 },
    { "CHANGING_FIRINGARC", 15 },
    { "COMBO", 11 },
    { "CONTAINED", 10 },
    { "ENRAGED", 8 },
    { "EXITING_PRODUCTION_STRUCTURE", 17 },
    { "FREEFALL", 2 },
    { "INVALID", -1 },
    { "MOUNTED", 7 },
    { "MOVING_TO_TRANSPORT", 13 },
    { "NORMAL", 0 },
    { "NORMAL_UPGRADED", 1 },
    { "PANIC", 4 },
    { "SCARED", 9 },
    { "SUPERSONIC", 6 },
    { "TAXIING", 5 },
    { "WALL_SCALING", 14 },
    { "WANDER", 3 },
};
const AIEnum abilityMovementLocomotorEnum = { abilityMovementLocomotorNames, 19u, 12, 0u };
const ModuleField abilityMovementFields0[] = {
    { "UseUpgradedLocomotor", 256u, ModuleValue::Boolean, nullptr, nullptr },
    { "TriggersLanding", 257u, ModuleValue::Boolean, "false", nullptr },
    { "TriggersFlight", 258u, ModuleValue::Boolean, "false", nullptr },
    { "BusyForDuration", 240u, ModuleValue::Time, "0s", nullptr },
    { "LandingRange", 244u, ModuleValue::Float, "0", nullptr },
    { "HordeMembersSpecialPowerTemplate", 248u, ModuleValue::Reference, nullptr, nullptr },
    { "LocomotorToSet", 252u, ModuleValue::Enum, nullptr, &abilityMovementLocomotorEnum },
};
const ModuleToken abilityMovementSwitchLocomotorsSpecialAbilityUpdateModuleDataTokens[] = {
    { 1u, 2891550594u, 0u, &abilitySpecialAbilityUpdateModuleData, 0u, 0u },
    { 162124602u, 2011428087u, 256u, nullptr, 0u, 0u },
    { 3885477424u, 2011428087u, 257u, nullptr, 0u, 0u },
    { 1695455310u, 2011428087u, 258u, nullptr, 0u, 0u },
    { 3051957251u, 2575722984u, 240u, nullptr, 0u, 0u },
    { 315190379u, 1683514611u, 244u, nullptr, 0u, 0u },
    { 4067676603u, 2934454408u, 248u, nullptr, 0u, 0u },
    { 969198254u, 2220692146u, 252u, nullptr, 0u, 0u },
};
const ModuleLayout abilityMovementSwitchLocomotorsSpecialAbilityUpdateModuleData = { abilityMovementSwitchLocomotorsSpecialAbilityUpdateModuleDataTokens, 8u };
const ModuleField abilityMovementFields1[] = {
    { "TimePerPassenger", 240u, ModuleValue::Time, "1s", nullptr },
    { "Speed", 244u, ModuleValue::Float, "10.0", nullptr },
    { "Radius", 248u, ModuleValue::Float, "20.0", nullptr },
    { "Height", 252u, ModuleValue::Float, "100.0", nullptr },
    { "ApexHeight", 256u, ModuleValue::Float, "0.0", nullptr },
    { "EjectionFX", 260u, ModuleValue::Reference, nullptr, nullptr },
    { "EjectionBoneName", 264u, ModuleValue::String, nullptr, nullptr },
    { "RunOffMapWhenDone", 276u, ModuleValue::Boolean, "false", nullptr },
    { "IgnoreTargetLocation", 277u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleToken abilityMovementEjectPassengersSpecialAbilityUpdateModuleDataTokens[] = {
    { 1u, 2891550594u, 0u, &abilitySpecialAbilityUpdateModuleData, 0u, 0u },
    { 1987415666u, 2575722984u, 240u, nullptr, 0u, 0u },
    { 2322663157u, 1683514611u, 244u, nullptr, 0u, 0u },
    { 1328662213u, 1683514611u, 248u, nullptr, 0u, 0u },
    { 2875692432u, 1683514611u, 252u, nullptr, 0u, 0u },
    { 3673259227u, 1683514611u, 256u, nullptr, 0u, 0u },
    { 513568279u, 219975299u, 260u, nullptr, 0u, 0u },
    { 2985002141u, 438907818u, 264u, nullptr, 0u, 0u },
    { 2427181176u, 2011428087u, 276u, nullptr, 0u, 0u },
    { 185836098u, 2011428087u, 277u, nullptr, 0u, 0u },
    { 2459417890u, 691437687u, 272u, nullptr, 2u, 0u },
};
const ModuleLayout abilityMovementEjectPassengersSpecialAbilityUpdateModuleData = { abilityMovementEjectPassengersSpecialAbilityUpdateModuleDataTokens, 11u };
const ModuleField abilityMovementFields2[] = {
    { "BusyDuration", 240u, ModuleValue::Time, "0s", nullptr },
    { "DestinationWeapon", 244u, ModuleValue::Reference, nullptr, nullptr },
    { "SourceWeapon", 248u, ModuleValue::Reference, nullptr, nullptr },
    { "MaxDistance", 252u, ModuleValue::Float, "0", nullptr },
    { "HeightToTeleportTo", 256u, ModuleValue::Float, "0", nullptr },
    { "FadeInTime", 260u, ModuleValue::Time, "0s", nullptr },
};
const ModuleToken abilityMovementTeleportSpecialAbilityUpdateModuleDataTokens[] = {
    { 1u, 2891550594u, 0u, &abilitySpecialAbilityUpdateModuleData, 0u, 0u },
    { 2813668214u, 2575722984u, 240u, nullptr, 0u, 0u },
    { 454958037u, 3494305058u, 244u, nullptr, 0u, 0u },
    { 771945036u, 3494305058u, 248u, nullptr, 0u, 0u },
    { 2903588941u, 1683514611u, 252u, nullptr, 0u, 0u },
    { 3653544759u, 1683514611u, 256u, nullptr, 0u, 0u },
    { 4289579894u, 2575722984u, 260u, nullptr, 0u, 0u },
};
const ModuleLayout abilityMovementTeleportSpecialAbilityUpdateModuleData = { abilityMovementTeleportSpecialAbilityUpdateModuleDataTokens, 7u };
const ModuleField abilityMovementFields3[] = {
    { "Radius", 240u, ModuleValue::Float, "0", nullptr },
    { "MinDestinationRadius", 244u, ModuleValue::Float, "0", nullptr },
    { "MaxDestinationRadius", 248u, ModuleValue::Float, "0", nullptr },
    { "MaxRangeFromCasterSP", 252u, ModuleValue::Float, "0", nullptr },
    { "MaxRangeFromCasterMP", 256u, ModuleValue::Float, "0", nullptr },
    { "TriggerFX", 260u, ModuleValue::Reference, nullptr, nullptr },
    { "TargetFX", 264u, ModuleValue::Reference, nullptr, nullptr },
    { "RequireStoredObjects", 268u, ModuleValue::Boolean, "false", nullptr },
    { "KillObjectsIfPlacedInBadSpot", 269u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleToken abilityMovementTeleportToCasterSpecialPowerModuleDataTokens[] = {
    { 1u, 2891550594u, 0u, &abilitySpecialAbilityUpdateModuleData, 0u, 0u },
    { 1328662213u, 1683514611u, 240u, nullptr, 0u, 0u },
    { 1324986770u, 1683514611u, 244u, nullptr, 0u, 0u },
    { 3385955927u, 1683514611u, 248u, nullptr, 0u, 0u },
    { 188498541u, 1683514611u, 252u, nullptr, 0u, 0u },
    { 2753519709u, 1683514611u, 256u, nullptr, 0u, 0u },
    { 1761895055u, 219975299u, 260u, nullptr, 0u, 0u },
    { 1980966568u, 219975299u, 264u, nullptr, 0u, 0u },
    { 3074765388u, 2011428087u, 268u, nullptr, 0u, 0u },
    { 3277141815u, 2011428087u, 269u, nullptr, 0u, 0u },
};
const ModuleLayout abilityMovementTeleportToCasterSpecialPowerModuleData = { abilityMovementTeleportToCasterSpecialPowerModuleDataTokens, 10u };
const ModuleFieldSet abilityMovementFieldSets[] = {
    { abilityMovementFields0, 7u },
    { abilityMovementFields1, 9u },
    { abilityMovementFields2, 6u },
    { abilityMovementFields3, 9u },
};
const ModuleChoice abilityMovementChoices[] = {
    { 350237090u, &abilityMovementSwitchLocomotorsSpecialAbilityUpdateModuleData, 260u, 200u },
    { 887477685u, &abilityMovementEjectPassengersSpecialAbilityUpdateModuleData, 280u, 201u },
    { 1313225606u, &abilityMovementTeleportSpecialAbilityUpdateModuleData, 264u, 202u },
    { 2474849635u, &abilityMovementTeleportToCasterSpecialPowerModuleData, 272u, 203u },
};
