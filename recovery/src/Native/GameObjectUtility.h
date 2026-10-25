// Complete scalar/string utilities; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const EnumNameValue utilitySpecialPowerRestrictionTypeNames[] = {
    { "1", 1 },
    { "2", 2 },
    { "UNRESTRICTED", 0 },
};
const AIEnum utilitySpecialPowerRestrictionType = { utilitySpecialPowerRestrictionTypeNames, 3u, 1, 0u };
const EnumNameValue utilityObjectFilterRelationshipNames[] = {
    { "ALLIES", 0 },
    { "ENEMIES", 1 },
    { "NEUTRAL", 2 },
    { "SAME_PLAYER", 3 },
};
const AIEnum utilityObjectFilterRelationship = { utilityObjectFilterRelationshipNames, 4u, 0, 4u };
const ModuleField utilityStorePurchasedUpgradeBehaviorModuleDataFields[] = {
    { "MaxPlayerUpgrades", 8u, ModuleValue::Unsigned, "9999", nullptr },
};
const ModuleToken utilityStorePurchasedUpgradeBehaviorModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &lifecycleBehaviorModuleData, 0u, 0u },
    { 2122048096u, 1982953380u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout utilityStorePurchasedUpgradeBehaviorModuleData = { utilityStorePurchasedUpgradeBehaviorModuleDataTokens, 2u };
const ModuleField utilityAIGateUpdateModuleDataFields[] = {
    { "TriggerWidthX", 8u, ModuleValue::Float, "0", nullptr },
    { "TriggerWidthY", 12u, ModuleValue::Float, "0", nullptr },
};
const ModuleToken utilityAIGateUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 3580059422u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 3615748891u, 1683514611u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout utilityAIGateUpdateModuleData = { utilityAIGateUpdateModuleDataTokens, 3u };
const ModuleField utilityCombinedInfoModuleDataFields[] = {
    { "DockingBone", 8u, ModuleValue::String, nullptr, nullptr },
};
const ModuleToken utilityCombinedInfoModuleDataTokens[] = {
    { 1u, 816016269u, 0u, &createCreateModuleData, 0u, 0u },
    { 1589100344u, 438907818u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout utilityCombinedInfoModuleData = { utilityCombinedInfoModuleDataTokens, 2u };
const ModuleField utilityTimedOpacityUpdateModuleDataFields[] = {
    { "Opacity", 8u, ModuleValue::Float, "0.5", nullptr },
    { "Duration", 12u, ModuleValue::Time, "5s", nullptr },
};
const ModuleToken utilityTimedOpacityUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 2688997143u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 2384108347u, 2575722984u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout utilityTimedOpacityUpdateModuleData = { utilityTimedOpacityUpdateModuleDataTokens, 3u };
const ModuleField utilityW3DPropDrawModuleDataFields[] = {
    { "Model", 8u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleToken utilityW3DPropDrawModuleDataTokens[] = {
    { 1u, 402003752u, 0u, &moduleTable13, 0u, 0u },
    { 3586538858u, 4239460053u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout utilityW3DPropDrawModuleData = { utilityW3DPropDrawModuleDataTokens, 2u };
const ModuleField utilityAttributeModifierNavalModuleDataFields[] = {
    { "AttributeModifier", 8u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleToken utilityAttributeModifierNavalModuleDataTokens[] = {
    { 1u, 816016269u, 0u, &createCreateModuleData, 0u, 0u },
    { 3319822471u, 165531635u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout utilityAttributeModifierNavalModuleData = { utilityAttributeModifierNavalModuleDataTokens, 2u };
const ModuleField utilityReplaceDrawModuleBehaviorModuleDataFields[] = {
    { "ConsumeItem", 8u, ModuleValue::Boolean, "true", nullptr },
};
const ModuleToken utilityReplaceDrawModuleBehaviorModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &lifecycleBehaviorModuleData, 0u, 0u },
    { 3562606791u, 2011428087u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout utilityReplaceDrawModuleBehaviorModuleData = { utilityReplaceDrawModuleBehaviorModuleDataTokens, 2u };
const ModuleField utilityStancesBehaviorModuleDataFields[] = {
    { "StanceTemplate", 8u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleToken utilityStancesBehaviorModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 2525284163u, 226629883u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout utilityStancesBehaviorModuleData = { utilityStancesBehaviorModuleDataTokens, 2u };
const ModuleField utilitySquishCollideModuleDataFields[] = {
    { "CrushAnimTime", 8u, ModuleValue::Time, "1s", nullptr },
    { "CrushKillDelay", 12u, ModuleValue::Time, ".5s", nullptr },
    { "UseDirectionCheck", 16u, ModuleValue::Boolean, "true", nullptr },
    { "UseVehicleStyleCrush", 17u, ModuleValue::Boolean, "false", nullptr },
    { "AllowAllyCrush", 18u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleToken utilitySquishCollideModuleDataTokens[] = {
    { 1u, 1894105347u, 0u, &motionCollideModuleData, 0u, 0u },
    { 2284035418u, 2575722984u, 8u, nullptr, 0u, 0u },
    { 3672504870u, 2575722984u, 12u, nullptr, 0u, 0u },
    { 2871119420u, 2011428087u, 16u, nullptr, 0u, 0u },
    { 1530631961u, 2011428087u, 17u, nullptr, 0u, 0u },
    { 205651807u, 2011428087u, 18u, nullptr, 0u, 0u },
};
const ModuleLayout utilitySquishCollideModuleData = { utilitySquishCollideModuleDataTokens, 6u };
const ModuleField utilityAreaRestrictSpecialPowerBehaviorModuleDataFields[] = {
    { "RestrictionType", 8u, ModuleValue::Enum, nullptr, &utilitySpecialPowerRestrictionType },
    { "AreaName", 12u, ModuleValue::String, nullptr, nullptr },
};
const ModuleToken utilityAreaRestrictSpecialPowerBehaviorModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &lifecycleBehaviorModuleData, 0u, 0u },
    { 384904913u, 729580760u, 8u, nullptr, 0u, 0u },
    { 3246016559u, 438907818u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout utilityAreaRestrictSpecialPowerBehaviorModuleData = { utilityAreaRestrictSpecialPowerBehaviorModuleDataTokens, 3u };
const ModuleField utilityMinefieldBehaviorModuleDataFields[] = {
    { "DetonationWeapon", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "DetonatedBy", 12u, ModuleValue::Bits, nullptr, &utilityObjectFilterRelationship },
    { "DetonationFX", 16u, ModuleValue::Reference, nullptr, nullptr },
    { "NumVirtualMines", 20u, ModuleValue::Unsigned, nullptr, nullptr },
    { "RepeatDetonateMoveThresh", 24u, ModuleValue::Float, "5.0", nullptr },
};
const ModuleToken utilityMinefieldBehaviorModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 3218355687u, 3494305058u, 8u, nullptr, 0u, 0u },
    { 932610101u, 1552515497u, 12u, nullptr, 0u, 0u },
    { 3340962800u, 219975299u, 16u, nullptr, 0u, 0u },
    { 726211492u, 1982953380u, 20u, nullptr, 0u, 0u },
    { 3196713972u, 1683514611u, 24u, nullptr, 0u, 0u },
};
const ModuleLayout utilityMinefieldBehaviorModuleData = { utilityMinefieldBehaviorModuleDataTokens, 6u };
const ModuleField utilityLiftObjectUpdateModuleDataFields[] = {
    { "CrusherModifiesVelocity", 36u, ModuleValue::Boolean, nullptr, nullptr },
    { "LiftVelocity", 8u, ModuleValue::Float, nullptr, nullptr },
    { "MaxElevationFromGround", 12u, ModuleValue::Float, nullptr, nullptr },
    { "TimeIncrement", 16u, ModuleValue::Time, "0s", nullptr },
    { "MaxTimeLifted", 20u, ModuleValue::Time, "1s", nullptr },
    { "MaxStructureShakeVelocity", 24u, ModuleValue::Float, "0", nullptr },
    { "AirplaneCrashWeapon", 28u, ModuleValue::Reference, nullptr, nullptr },
    { "SoftLandingWeapon", 32u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleToken utilityLiftObjectUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 2505916104u, 2011428087u, 36u, nullptr, 0u, 0u },
    { 939681351u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 13888185u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 352194316u, 2575722984u, 16u, nullptr, 0u, 0u },
    { 2393042570u, 2575722984u, 20u, nullptr, 0u, 0u },
    { 2306052161u, 1683514611u, 24u, nullptr, 0u, 0u },
    { 4070628995u, 3494305058u, 28u, nullptr, 0u, 0u },
    { 2793371144u, 3494305058u, 32u, nullptr, 0u, 0u },
};
const ModuleLayout utilityLiftObjectUpdateModuleData = { utilityLiftObjectUpdateModuleDataTokens, 9u };
const ModuleField utilityClusterBombUpdateModuleDataFields[] = {
    { "WeaponName", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "InitialWeaponName", 12u, ModuleValue::NullableReference, nullptr, nullptr },
    { "Radius", 16u, ModuleValue::Float, "0.0", nullptr },
    { "NumBomblets", 20u, ModuleValue::Signed, "0", nullptr },
    { "MinDelay", 24u, ModuleValue::Time, "0s", nullptr },
    { "MaxDelay", 28u, ModuleValue::Time, "0s", nullptr },
    { "BombletFX", 32u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleToken utilityClusterBombUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 271546182u, 3494305058u, 8u, nullptr, 0u, 0u },
    { 95434915u, 3494305058u, 12u, nullptr, 2u, 0u },
    { 1328662213u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 3662769867u, 602296504u, 20u, nullptr, 0u, 0u },
    { 162863551u, 2575722984u, 24u, nullptr, 0u, 0u },
    { 1148254404u, 2575722984u, 28u, nullptr, 0u, 0u },
    { 3223426280u, 219975299u, 32u, nullptr, 0u, 0u },
};
const ModuleLayout utilityClusterBombUpdateModuleData = { utilityClusterBombUpdateModuleDataTokens, 8u };
const ModuleFieldSet utilityFieldSets[] = {
    { utilityStorePurchasedUpgradeBehaviorModuleDataFields, 1u },
    { utilityAIGateUpdateModuleDataFields, 2u },
    { utilityCombinedInfoModuleDataFields, 1u },
    { utilityTimedOpacityUpdateModuleDataFields, 2u },
    { utilityW3DPropDrawModuleDataFields, 1u },
    { utilityAttributeModifierNavalModuleDataFields, 1u },
    { utilityReplaceDrawModuleBehaviorModuleDataFields, 1u },
    { utilityStancesBehaviorModuleDataFields, 1u },
    { utilitySquishCollideModuleDataFields, 5u },
    { utilityAreaRestrictSpecialPowerBehaviorModuleDataFields, 2u },
    { utilityMinefieldBehaviorModuleDataFields, 5u },
    { utilityLiftObjectUpdateModuleDataFields, 8u },
    { utilityClusterBombUpdateModuleDataFields, 7u },
};
const ModuleChoice utilityChoices[] = {
    { 783731100u, &utilityStorePurchasedUpgradeBehaviorModuleData, 12u, 225u },
    { 880602396u, &utilityAIGateUpdateModuleData, 16u, 226u },
    { 2018429368u, &utilityCombinedInfoModuleData, 16u, 227u },
    { 4216826759u, &utilityTimedOpacityUpdateModuleData, 16u, 228u },
    { 344630372u, &utilityW3DPropDrawModuleData, 12u, 229u },
    { 2739727708u, &utilityAttributeModifierNavalModuleData, 12u, 230u },
    { 3032642002u, &utilityReplaceDrawModuleBehaviorModuleData, 12u, 231u },
    { 3231835197u, &utilityStancesBehaviorModuleData, 12u, 232u },
    { 3052093933u, &utilitySquishCollideModuleData, 20u, 233u },
    { 1206381257u, &utilityAreaRestrictSpecialPowerBehaviorModuleData, 20u, 234u },
    { 3040483501u, &utilityMinefieldBehaviorModuleData, 28u, 235u },
    { 3214000077u, &utilityLiftObjectUpdateModuleData, 40u, 236u },
    { 3833989627u, &utilityClusterBombUpdateModuleData, 36u, 237u },
};
