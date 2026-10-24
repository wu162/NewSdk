// Generated original weapon firing/tracking contracts; blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const float weaponSecondsPerLogicFrame = 0.2f;
const EnumNameValue weaponDisabledTypeNames[] = {
    { "DEFAULT", 0 },
    { "EMP", 2 },
    { "FREEFALL", 7 },
    { "HELD", 3 },
    { "INFILTRATED", 11 },
    { "PARALYZED", 4 },
    { "SCRIPT_DISABLED", 9 },
    { "SCRIPT_UNDERPOWERED", 10 },
    { "TEMPORARILY_BUSY", 8 },
    { "UNDERPOWERED", 6 },
    { "UNMANNED", 5 },
    { "USER_PARALYZED", 1 },
};
const AIEnum weaponDisabledType = { weaponDisabledTypeNames, 12u, 0, 12u };
const EnumNameValue weaponEffectInfoFlagTypeNames[] = {
    { "DIE_ON_ENTER", 0 },
};
const AIEnum weaponEffectInfoFlagType = { weaponEffectInfoFlagTypeNames, 1u, 0, 1u };
const ModuleField weaponChargeWeaponUpdateModuleDataFields[] = {
    { "ChargeTime", 8u, ModuleValue::Time, "1.0s", nullptr },
    { "OverChargeTime", 12u, ModuleValue::Time, "0.0s", nullptr },
    { "OverChargePercent", 16u, ModuleValue::Percent, "0%", nullptr },
};
const ModuleField weaponFireWeaponUpdateModuleDataFields[] = {
    { "FireInterval", 8u, ModuleValue::Time, nullptr, nullptr },
    { "AliveOnly", 28u, ModuleValue::Boolean, "false", nullptr },
    { "ActiveWhenDisabled", 12u, ModuleValue::ReplaceOptions, nullptr, &weaponDisabledType },
};
const ModuleField weaponAimWeaponBehaviorModuleDataFields[] = {
    { "AimLowThreshold", 8u, ModuleValue::Float, "-0.4", nullptr },
    { "AimHighThreshold", 12u, ModuleValue::Float, "0.4", nullptr },
    { "AimNearDistance", 16u, ModuleValue::Float, "0", nullptr },
    { "AimFarDistance", 20u, ModuleValue::Float, "0", nullptr },
};
const ModuleField weaponLaserUpdateModuleDataFields[] = {
    { "ParticleSystemId", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "ParentFireBoneName", 12u, ModuleValue::String, nullptr, nullptr },
    { "ParentFireBoneOnTurret", 28u, ModuleValue::Boolean, "false", nullptr },
    { "TargetParticleSystemId", 20u, ModuleValue::Reference, nullptr, nullptr },
    { "LaserLifetime", 24u, ModuleValue::Float, "0", nullptr },
};
const ModuleField weaponLaserScatterUpdateModuleDataFields[] = {
    { "LaserLifetime", 8u, ModuleValue::Time, "1s", nullptr },
    { "LaserHitGroundWidthMultiplier", 12u, ModuleValue::Percent, "10%", nullptr },
};
const ModuleField weaponLinearDamageUpdateModuleDataFields[] = {
    { "StartDistanceOffset", 8u, ModuleValue::Float, "0", nullptr },
    { "Radius", 12u, ModuleValue::Float, "10.0", nullptr },
    { "Length", 16u, ModuleValue::Float, nullptr, nullptr },
    { "SweepFXList", 20u, ModuleValue::Reference, nullptr, nullptr },
    { "SweepWeapon", 24u, ModuleValue::Reference, nullptr, nullptr },
    { "Speed", 28u, ModuleValue::FrameSpeed, "1.0", nullptr },
    { "FollowTerrain", 36u, ModuleValue::Boolean, "true", nullptr },
};
const ModuleField weaponSecondaryDamageTrackerModuleDataFields[] = {
    { "UpdateDuration", 8u, ModuleValue::Time, "1s", nullptr },
    { "DissipateAmount", 12u, ModuleValue::Unsigned, "0", nullptr },
    { "MaxDamageClampScaler", 16u, ModuleValue::Percent, "125%", nullptr },
};
const ModuleField weaponTrackDamageModuleDataFields[] = {
    { "TimeToTrack", 8u, ModuleValue::Time, nullptr, nullptr },
    { "InstantWeaponsOnly", 12u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleField weaponRGBColorFields[] = {
    { "R", 0u, ModuleValue::Float, nullptr, nullptr },
    { "G", 4u, ModuleValue::Float, nullptr, nullptr },
    { "B", 8u, ModuleValue::Float, nullptr, nullptr },
};
const ModuleField weaponEffectInfoTypeFields[] = {
    { "Fraction", 0u, ModuleValue::Float, nullptr, nullptr },
    { "AttributeModifier", 4u, ModuleValue::Reference, nullptr, nullptr },
    { "OnEnterDisable", 12u, ModuleValue::Boolean, "false", nullptr },
    { "OnEnterCancelSpecialAbilities", 13u, ModuleValue::Boolean, "false", nullptr },
    { "SetUpgradedArmor", 14u, ModuleValue::Boolean, "false", nullptr },
    { "Options", 8u, ModuleValue::ReplaceOptions, "", &weaponEffectInfoFlagType },
};
const ModuleField weaponFireWeaponUpdateModuleData_FireWeaponNuggetFields[] = {
    { "WeaponName", 0u, ModuleValue::Reference, nullptr, nullptr },
    { "FireDelay", 4u, ModuleValue::Time, nullptr, nullptr },
    { "OneShot", 12u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleField weaponVector3Fields[] = {
    { "x", 0u, ModuleValue::Float, "0.0", nullptr },
    { "y", 4u, ModuleValue::Float, "0.0", nullptr },
    { "z", 8u, ModuleValue::Float, "0.0", nullptr },
};
const ModuleFieldSet weaponFieldSets[] = {
    { weaponChargeWeaponUpdateModuleDataFields, 3u },
    { weaponFireWeaponUpdateModuleDataFields, 3u },
    { weaponAimWeaponBehaviorModuleDataFields, 4u },
    { weaponLaserUpdateModuleDataFields, 5u },
    { weaponLaserScatterUpdateModuleDataFields, 2u },
    { weaponLinearDamageUpdateModuleDataFields, 7u },
    { weaponSecondaryDamageTrackerModuleDataFields, 3u },
    { weaponTrackDamageModuleDataFields, 2u },
    { weaponRGBColorFields, 3u },
    { weaponEffectInfoTypeFields, 6u },
    { weaponFireWeaponUpdateModuleData_FireWeaponNuggetFields, 3u },
    { weaponVector3Fields, 3u },
};
const ModuleToken weaponModuleDataTokens[] = {
    { 430981792u, 602296504u, 0u, nullptr, 0u, 0u },
    { 2363267224u, 3186653541u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout weaponModuleData = { weaponModuleDataTokens, 2u };
const ModuleToken weaponBehaviorModuleDataTokens[] = {
    { 1u, 3613234082u, 0u, &weaponModuleData, 0u, 0u },
};
const ModuleLayout weaponBehaviorModuleData = { weaponBehaviorModuleDataTokens, 1u };
const ModuleToken weaponContainModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &weaponBehaviorModuleData, 0u, 0u },
};
const ModuleLayout weaponContainModuleData = { weaponContainModuleDataTokens, 1u };
const ModuleToken weaponUpdateModuleDataTokens[] = {
    { 1u, 687874333u, 0u, &weaponContainModuleData, 0u, 0u },
};
const ModuleLayout weaponUpdateModuleData = { weaponUpdateModuleDataTokens, 1u };
const ModuleToken weaponObjectStatusValidationDataTypeTokens[] = {
    { 824345085u, 2814214430u, 0u, nullptr, 0u, 0u },
    { 597602817u, 2814214430u, 28u, nullptr, 0u, 0u },
};
const ModuleLayout weaponObjectStatusValidationDataType = { weaponObjectStatusValidationDataTypeTokens, 2u };
const ModuleToken weaponChargeWeaponUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &weaponUpdateModuleData, 0u, 0u },
    { 1065715096u, 2575722984u, 8u, nullptr, 0u, 0u },
    { 1273088161u, 2575722984u, 12u, nullptr, 0u, 0u },
    { 3027945563u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 3933465695u, 1049596812u, 20u, &weaponObjectStatusValidationDataType, 4u, 0u },
};
const ModuleLayout weaponChargeWeaponUpdateModuleData = { weaponChargeWeaponUpdateModuleDataTokens, 5u };
const ModuleToken weaponFireWeaponUpdateModuleData_FireWeaponNuggetTokens[] = {
    { 271546182u, 3494305058u, 0u, nullptr, 0u, 0u },
    { 310886179u, 2575722984u, 4u, nullptr, 0u, 0u },
    { 373198107u, 2011428087u, 12u, nullptr, 0u, 0u },
    { 2676558395u, 691437687u, 8u, nullptr, 2u, 0u },
};
const ModuleLayout weaponFireWeaponUpdateModuleData_FireWeaponNugget = { weaponFireWeaponUpdateModuleData_FireWeaponNuggetTokens, 4u };
const ModuleToken weaponFireWeaponUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &weaponUpdateModuleData, 0u, 0u },
    { 3266714871u, 2575722984u, 8u, nullptr, 0u, 0u },
    { 1775687299u, 2011428087u, 28u, nullptr, 0u, 0u },
    { 445571954u, 426078409u, 12u, nullptr, 0u, 0u },
    { 3222992782u, 2372208250u, 16u, &weaponFireWeaponUpdateModuleData_FireWeaponNugget, 1u, 16u },
    { 3933465695u, 1049596812u, 24u, &weaponObjectStatusValidationDataType, 4u, 0u },
};
const ModuleLayout weaponFireWeaponUpdateModuleData = { weaponFireWeaponUpdateModuleDataTokens, 6u };
const ModuleToken weaponAimWeaponBehaviorModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &weaponUpdateModuleData, 0u, 0u },
    { 1787736291u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 981824646u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 425435166u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 1600290480u, 1683514611u, 20u, nullptr, 0u, 0u },
};
const ModuleLayout weaponAimWeaponBehaviorModuleData = { weaponAimWeaponBehaviorModuleDataTokens, 5u };
const ModuleToken weaponClientUpdateModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &weaponBehaviorModuleData, 0u, 0u },
};
const ModuleLayout weaponClientUpdateModuleData = { weaponClientUpdateModuleDataTokens, 1u };
const ModuleToken weaponLaserUpdateModuleDataTokens[] = {
    { 1u, 1286434524u, 0u, &weaponClientUpdateModuleData, 0u, 0u },
    { 3288131190u, 1139920438u, 8u, nullptr, 0u, 0u },
    { 372443599u, 438907818u, 12u, nullptr, 0u, 0u },
    { 1025257192u, 2011428087u, 28u, nullptr, 0u, 0u },
    { 3651255743u, 1139920438u, 20u, nullptr, 0u, 0u },
    { 2850671775u, 1683514611u, 24u, nullptr, 0u, 0u },
};
const ModuleLayout weaponLaserUpdateModuleData = { weaponLaserUpdateModuleDataTokens, 6u };
const ModuleToken weaponLaserScatterUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &weaponUpdateModuleData, 0u, 0u },
    { 2850671775u, 2575722984u, 8u, nullptr, 0u, 0u },
    { 528812145u, 1683514611u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout weaponLaserScatterUpdateModuleData = { weaponLaserScatterUpdateModuleDataTokens, 3u };
const ModuleToken weaponLinearDamageUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &weaponUpdateModuleData, 0u, 0u },
    { 957500112u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 1328662213u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 2358492574u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 732717553u, 219975299u, 20u, nullptr, 0u, 0u },
    { 1093366695u, 3494305058u, 24u, nullptr, 0u, 0u },
    { 2322663157u, 2054604302u, 28u, nullptr, 0u, 0u },
    { 1237653014u, 2011428087u, 36u, nullptr, 0u, 0u },
    { 3933465695u, 1049596812u, 32u, &weaponObjectStatusValidationDataType, 4u, 0u },
};
const ModuleLayout weaponLinearDamageUpdateModuleData = { weaponLinearDamageUpdateModuleDataTokens, 9u };
const ModuleToken weaponRGBColorTokens[] = {
    { 3593226867u, 1683514611u, 0u, nullptr, 0u, 0u },
    { 4213441201u, 1683514611u, 4u, nullptr, 0u, 0u },
    { 3809169639u, 1683514611u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout weaponRGBColor = { weaponRGBColorTokens, 3u };
const ModuleToken weaponEffectInfoTypeTokens[] = {
    { 1619237267u, 1683514611u, 0u, nullptr, 0u, 0u },
    { 3319822471u, 165531635u, 4u, nullptr, 0u, 0u },
    { 821162299u, 2011428087u, 12u, nullptr, 0u, 0u },
    { 4120563629u, 2011428087u, 13u, nullptr, 0u, 0u },
    { 2859802133u, 2011428087u, 14u, nullptr, 0u, 0u },
    { 3202187060u, 2733865554u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout weaponEffectInfoType = { weaponEffectInfoTypeTokens, 6u };
const ModuleToken weaponSecondaryDamageTrackerModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &weaponUpdateModuleData, 0u, 0u },
    { 1108031397u, 2575722984u, 8u, nullptr, 0u, 0u },
    { 2854104076u, 1982953380u, 12u, nullptr, 0u, 0u },
    { 921055669u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 2654902107u, 2934399653u, 20u, &weaponRGBColor, 4u, 0u },
    { 2723343677u, 636827551u, 24u, &weaponEffectInfoType, 1u, 16u },
    { 3933465695u, 1049596812u, 32u, &weaponObjectStatusValidationDataType, 4u, 0u },
};
const ModuleLayout weaponSecondaryDamageTrackerModuleData = { weaponSecondaryDamageTrackerModuleDataTokens, 7u };
const ModuleToken weaponDamageModuleDataTokens[] = {
    { 1u, 687874333u, 0u, &weaponContainModuleData, 0u, 0u },
};
const ModuleLayout weaponDamageModuleData = { weaponDamageModuleDataTokens, 1u };
const ModuleToken weaponTrackDamageModuleDataTokens[] = {
    { 1u, 39586775u, 0u, &weaponDamageModuleData, 0u, 0u },
    { 2535529986u, 2575722984u, 8u, nullptr, 0u, 0u },
    { 2820141804u, 2011428087u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout weaponTrackDamageModuleData = { weaponTrackDamageModuleDataTokens, 3u };
const ModuleChoice weaponChoices[] = {
    { 602268606u, &weaponChargeWeaponUpdateModuleData, 24u, 69u },
    { 2995535186u, &weaponFireWeaponUpdateModuleData, 32u, 70u },
    { 3828176408u, &weaponAimWeaponBehaviorModuleData, 24u, 71u },
    { 4198440042u, &weaponLaserScatterUpdateModuleData, 16u, 73u },
    { 509375610u, &weaponLinearDamageUpdateModuleData, 40u, 74u },
    { 2988211815u, &weaponSecondaryDamageTrackerModuleData, 36u, 75u },
    { 3579087005u, &weaponTrackDamageModuleData, 16u, 76u },
};
