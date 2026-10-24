// Generated complete WeaponSetUpdate contracts; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const EnumNameValue weaponSetWeaponSlotTypeNames[] = {
    { "NO_WEAPON", 5 },
    { "PRIMARY_WEAPON", 0 },
    { "QUATERNARY_WEAPON", 3 },
    { "QUINARY_WEAPON", 4 },
    { "SECONDARY_WEAPON", 1 },
    { "TERTIARY_WEAPON", 2 },
};
const AIEnum weaponSetWeaponSlotTypeEnum = { weaponSetWeaponSlotTypeNames, 6u, 5, 6u };
const EnumNameValue weaponSetWeaponSlotInterleavedStyleTypeNames[] = {
    { "INTERLEAVE_FIRST_AVAILABLE", 0 },
    { "INTERLEAVE_RANDOM", 1 },
};
const AIEnum weaponSetWeaponSlotInterleavedStyleTypeEnum = { weaponSetWeaponSlotInterleavedStyleTypeNames, 2u, 0, 0u };
const EnumNameValue weaponSetWeaponChoiceCriteriaNames[] = {
    { "PREFER_GRAB_OVER_DAMAGE", 2 },
    { "PREFER_LEAST_MOVEMENT", 3 },
    { "PREFER_LONGEST_RANGE", 1 },
    { "PREFER_MOST_DAMAGE", 0 },
    { "SELECT_AT_RANDOM", 4 },
    { "USE_WEAPONSET_DEFAULT_CRITERIA", 5 },
};
const AIEnum weaponSetWeaponChoiceCriteriaEnum = { weaponSetWeaponChoiceCriteriaNames, 6u, 2, 0u };
const EnumNameValue weaponSetTargetChooserDynamicTargetFlagNames[] = {
    { "AGGRESSIVE_STANCE", 2 },
    { "ALWAYS", 3 },
    { "ATTACK_MOVE", 1 },
    { "NEVER", 0 },
};
const AIEnum weaponSetTargetChooserDynamicTargetFlagEnum = { weaponSetTargetChooserDynamicTargetFlagNames, 4u, 2, 4u };
const ModuleField weaponSetWeaponSlot_HardpointFields[] = {
    { "ID", 4u, ModuleValue::Unsigned, nullptr, nullptr },
    { "AllowInterleavedFiring", 24u, ModuleValue::Boolean, "false", nullptr },
    { "InterleavedStyle", 8u, ModuleValue::Enum, "INTERLEAVE_RANDOM", &weaponSetWeaponSlotInterleavedStyleTypeEnum },
    { "WeaponChoiceCriteria", 12u, ModuleValue::Enum, "PREFER_MOST_DAMAGE", &weaponSetWeaponChoiceCriteriaEnum },
    { "AutoChooseWeapon", 25u, ModuleValue::Boolean, "true", nullptr },
};
const ModuleField weaponSetWeaponSlot_HierarchicalTurretFields[] = {
    { "ParentID", 124u, ModuleValue::Unsigned, "0", nullptr },
};
const ModuleField weaponSetWeaponSlot_WeaponDataFields[] = {
    { "Ordering", 0u, ModuleValue::Enum, nullptr, &weaponSetWeaponSlotTypeEnum },
    { "Template", 4u, ModuleValue::Reference, nullptr, nullptr },
    { "Upgrade", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "ObjectStatus", 12u, ModuleValue::ObjectStatus, "", nullptr },
    { "ForbiddenObjectStatus", 40u, ModuleValue::ObjectStatus, "", nullptr },
    { "IsPlayerUpgradePermanent", 68u, ModuleValue::Boolean, "false", nullptr },
    { "ConquerMetricsAlwaysCount", 69u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleField weaponSetTurretAIDataFields[] = {
    { "TurretTurnRate", 0u, ModuleValue::Float, "0.0", nullptr },
    { "TurretPitchRate", 4u, ModuleValue::Float, "0.0", nullptr },
    { "AllowsPitch", 92u, ModuleValue::Boolean, "false", nullptr },
    { "MinimumPitch", 8u, ModuleValue::Angle, "0d", nullptr },
    { "PitchHeight", 12u, ModuleValue::Percent, "50%", nullptr },
    { "ControlledWeaponSlots", 16u, ModuleValue::Bits, nullptr, &weaponSetWeaponSlotTypeEnum },
    { "InvalidIdleScanObjectStatus", 20u, ModuleValue::ObjectStatus, nullptr, nullptr },
    { "IdleTurnRateModifier", 48u, ModuleValue::Percent, "50%", nullptr },
    { "MinIdleScanTime", 52u, ModuleValue::Time, "0.0s", nullptr },
    { "MaxIdleScanTime", 56u, ModuleValue::Time, "0.0s", nullptr },
    { "MinIdleScanAngle", 60u, ModuleValue::Float, "0.0", nullptr },
    { "MaxIdleScanAngle", 64u, ModuleValue::Float, "0.0", nullptr },
    { "MaxDeflectionClockwise", 68u, ModuleValue::Angle, "180d", nullptr },
    { "MaxDeflectionAntiClockwise", 72u, ModuleValue::Angle, "180d", nullptr },
    { "NaturalTurretAngle", 76u, ModuleValue::Angle, "0d", nullptr },
    { "RecenterWhenOutOfTurnRange", 93u, ModuleValue::Boolean, "true", nullptr },
    { "BlindFireDistance", 80u, ModuleValue::Float, "0", nullptr },
    { "ComeToHaltJiggle", 84u, ModuleValue::Angle, "0d", nullptr },
    { "SetAngleModelStates", 94u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleField weaponSetTurretAITargetChooserDataFields[] = {
    { "TargetingCompareList", 36u, ModuleValue::Reference, "DefaultTurretTargetingCompareList", nullptr },
    { "ActiveWhenPerformingSpecialAbility", 40u, ModuleValue::Boolean, "false", nullptr },
    { "CanAcquireDynamicIfAssignedOutOfRange", 41u, ModuleValue::Boolean, "false", nullptr },
    { "CanPickTargetsOutOfTurretAngle", 42u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleField weaponSetBaseAITargetChooserDataFields[] = {
    { "IdleScanDelay", 0u, ModuleValue::Time, "0.8s", nullptr },
    { "ReacquireDelay", 4u, ModuleValue::Time, "60s", nullptr },
    { "StartupDelay", 8u, ModuleValue::Time, "0s", nullptr },
    { "SympathyRange", 12u, ModuleValue::Float, "25.0", nullptr },
    { "CheckVisionRange", 28u, ModuleValue::Boolean, "false", nullptr },
    { "CanPickDynamicTargets", 29u, ModuleValue::Boolean, "true", nullptr },
    { "CanPickDynamicTargetType", 16u, ModuleValue::Bits, "ALWAYS", &weaponSetTargetChooserDynamicTargetFlagEnum },
    { "RotateToTargetWhenAiming", 30u, ModuleValue::Boolean, "false", nullptr },
    { "CanAutoAcquireNonAutoAcquirable", 31u, ModuleValue::Boolean, "false", nullptr },
    { "CanChaseAttack", 32u, ModuleValue::Boolean, "true", nullptr },
    { "CanOnlyAttackTargetsInFront", 33u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleField weaponSetObjectVisionRangeItemFields[] = {
    { "Range", 4u, ModuleValue::Percent, "100", nullptr },
};
const ModuleFieldSet weaponSetFieldSets[] = {
    { nullptr, 0u },
    { weaponSetWeaponSlot_HardpointFields, 5u },
    { nullptr, 0u },
    { weaponSetWeaponSlot_HierarchicalTurretFields, 1u },
    { nullptr, 0u },
    { weaponSetWeaponSlot_WeaponDataFields, 7u },
    { weaponSetTurretAIDataFields, 19u },
    { weaponSetTurretAITargetChooserDataFields, 4u },
    { weaponSetBaseAITargetChooserDataFields, 11u },
    { weaponSetObjectVisionRangeItemFields, 1u },
};
const ModuleToken weaponSetWeaponSlot_WeaponDataTokens[] = {
    { 3791981744u, 407422281u, 0u, nullptr, 0u, 0u },
    { 1650367787u, 3494305058u, 4u, nullptr, 0u, 0u },
    { 3619147155u, 1253711657u, 8u, nullptr, 0u, 0u },
    { 3963816762u, 2814214430u, 12u, nullptr, 0u, 0u },
    { 2365265496u, 2814214430u, 40u, nullptr, 0u, 0u },
    { 688512685u, 2011428087u, 68u, nullptr, 0u, 0u },
    { 4174931028u, 2011428087u, 69u, nullptr, 0u, 0u },
};
const ModuleLayout weaponSetWeaponSlot_WeaponData = { weaponSetWeaponSlot_WeaponDataTokens, 7u };
const ModuleToken weaponSetWeaponSlot_HardpointTokens[] = {
    { 430981792u, 602296504u, 0u, nullptr, 0u, 0u },
    { 2706179226u, 1982953380u, 4u, nullptr, 0u, 0u },
    { 637519812u, 2011428087u, 24u, nullptr, 0u, 0u },
    { 1518226902u, 3194394665u, 8u, nullptr, 0u, 0u },
    { 487655314u, 487655314u, 12u, nullptr, 0u, 0u },
    { 3525166389u, 2011428087u, 25u, nullptr, 0u, 0u },
    { 63188358u, 2006535755u, 16u, &weaponSetWeaponSlot_WeaponData, 1u, 72u },
};
const ModuleLayout weaponSetWeaponSlot_Hardpoint = { weaponSetWeaponSlot_HardpointTokens, 7u };
const ModuleToken weaponSetObjectVisionRangeItemTokens[] = {
    { 3426209446u, 1683514611u, 4u, nullptr, 0u, 0u },
    { 2239343654u, 3832456114u, 8u, nullptr, 8u, 0u },
};
const ModuleLayout weaponSetObjectVisionRangeItem = { weaponSetObjectVisionRangeItemTokens, 2u };
const ModuleToken weaponSetBaseAITargetChooserDataTokens[] = {
    { 4202375177u, 2575722984u, 0u, nullptr, 0u, 0u },
    { 1971854864u, 2575722984u, 4u, nullptr, 0u, 0u },
    { 3999791613u, 2575722984u, 8u, nullptr, 0u, 0u },
    { 2436625818u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 4276714359u, 2011428087u, 28u, nullptr, 0u, 0u },
    { 2663767317u, 2011428087u, 29u, nullptr, 0u, 0u },
    { 1158934690u, 877530936u, 16u, nullptr, 0u, 0u },
    { 1473219925u, 2011428087u, 30u, nullptr, 0u, 0u },
    { 2278107960u, 2011428087u, 31u, nullptr, 0u, 0u },
    { 1698488146u, 2011428087u, 32u, nullptr, 0u, 0u },
    { 3825347779u, 2011428087u, 33u, nullptr, 0u, 0u },
    { 2040109034u, 772930644u, 20u, &weaponSetObjectVisionRangeItem, 1u, 120u },
};
const ModuleLayout weaponSetBaseAITargetChooserData = { weaponSetBaseAITargetChooserDataTokens, 12u };
const ModuleToken weaponSetTurretAITargetChooserDataTokens[] = {
    { 1u, 2856512412u, 0u, &weaponSetBaseAITargetChooserData, 0u, 0u },
    { 3188107749u, 461747305u, 36u, nullptr, 0u, 0u },
    { 385161536u, 2011428087u, 40u, nullptr, 0u, 0u },
    { 772643074u, 2011428087u, 41u, nullptr, 0u, 0u },
    { 3629683364u, 2011428087u, 42u, nullptr, 0u, 0u },
};
const ModuleLayout weaponSetTurretAITargetChooserData = { weaponSetTurretAITargetChooserDataTokens, 5u };
const ModuleToken weaponSetTurretAIDataTokens[] = {
    { 2379067778u, 1683514611u, 0u, nullptr, 0u, 0u },
    { 2201199974u, 1683514611u, 4u, nullptr, 0u, 0u },
    { 1198657302u, 2011428087u, 92u, nullptr, 0u, 0u },
    { 2249467414u, 1310124621u, 8u, nullptr, 0u, 0u },
    { 323796386u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 51148849u, 682547989u, 16u, nullptr, 0u, 0u },
    { 2727160208u, 2814214430u, 20u, nullptr, 0u, 0u },
    { 2683993879u, 1683514611u, 48u, nullptr, 0u, 0u },
    { 4184927910u, 2575722984u, 52u, nullptr, 0u, 0u },
    { 469080566u, 2575722984u, 56u, nullptr, 0u, 0u },
    { 3895826996u, 1683514611u, 60u, nullptr, 0u, 0u },
    { 1773783680u, 1683514611u, 64u, nullptr, 0u, 0u },
    { 1067368045u, 1310124621u, 68u, nullptr, 0u, 0u },
    { 3646494640u, 1310124621u, 72u, nullptr, 0u, 0u },
    { 2113132439u, 1310124621u, 76u, nullptr, 0u, 0u },
    { 3980510527u, 2011428087u, 93u, nullptr, 0u, 0u },
    { 3713635497u, 1683514611u, 80u, nullptr, 0u, 0u },
    { 204923461u, 1310124621u, 84u, nullptr, 0u, 0u },
    { 3916033543u, 2011428087u, 94u, nullptr, 0u, 0u },
    { 147797168u, 147797168u, 88u, &weaponSetTurretAITargetChooserData, 4u, 0u },
};
const ModuleLayout weaponSetTurretAIData = { weaponSetTurretAIDataTokens, 20u };
const ModuleToken weaponSetWeaponSlot_TurretTokens[] = {
    { 1u, 3461895127u, 0u, &weaponSetWeaponSlot_Hardpoint, 0u, 0u },
    { 2460431979u, 4134875235u, 28u, &weaponSetTurretAIData, 0u, 0u },
};
const ModuleLayout weaponSetWeaponSlot_Turret = { weaponSetWeaponSlot_TurretTokens, 2u };
const ModuleToken weaponSetWeaponSlot_HierarchicalTurretTokens[] = {
    { 1u, 1326752513u, 0u, &weaponSetWeaponSlot_Turret, 0u, 0u },
    { 310255250u, 1982953380u, 124u, nullptr, 0u, 0u },
};
const ModuleLayout weaponSetWeaponSlot_HierarchicalTurret = { weaponSetWeaponSlot_HierarchicalTurretTokens, 2u };
const ModuleToken weaponSetWeaponSlot_ContainedTokens[] = {
    { 1u, 3461895127u, 0u, &weaponSetWeaponSlot_Hardpoint, 0u, 0u },
};
const ModuleLayout weaponSetWeaponSlot_Contained = { weaponSetWeaponSlot_ContainedTokens, 1u };
const ModuleToken weaponSetWeaponSetUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &lifecycleUpdateModuleData, 0u, 0u },
    { 1151544716u, 3461895127u, 8u, &weaponSetWeaponSlot_Hardpoint, 1u, 28u },
    { 2527635580u, 1326752513u, 16u, &weaponSetWeaponSlot_Turret, 1u, 124u },
    { 2014210431u, 3297263697u, 24u, &weaponSetWeaponSlot_HierarchicalTurret, 1u, 128u },
    { 1187408286u, 3123540706u, 32u, &weaponSetWeaponSlot_Contained, 1u, 28u },
};
const ModuleLayout weaponSetWeaponSetUpdateModuleData = { weaponSetWeaponSetUpdateModuleDataTokens, 5u };
const ModuleChoice weaponSetChoice = { 1125589673u, &weaponSetWeaponSetUpdateModuleData, 40u, 91u };
