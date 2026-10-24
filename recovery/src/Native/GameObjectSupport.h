// Generated complete support group; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const EnumNameValue supportSetLocomotorSpeedUpdateTypeNames[] = {
    { "INVALID", 0 },
    { "IN_RANGE_OBJECT_FILTER", 1 },
};
const AIEnum supportSetLocomotorSpeedUpdateType = { supportSetLocomotorSpeedUpdateTypeNames, 2u, 0, 0u };
const EnumNameValue supportSpecialPowerRestrictionTypeNames[] = {
    { "1", 1 },
    { "2", 2 },
    { "UNRESTRICTED", 0 },
};
const AIEnum supportSpecialPowerRestrictionType = { supportSpecialPowerRestrictionTypeNames, 3u, 1, 0u };
const EnumNameValue supportAttributeModifierCategoryTypeNames[] = {
    { "BUFF", 7 },
    { "DEBUFF", 8 },
    { "FORMATION", 2 },
    { "INNATE_ARMOR", 10 },
    { "INNATE_AUTOHEAL", 13 },
    { "INNATE_DAMAGEMULT", 11 },
    { "INNATE_HEALTH", 14 },
    { "INNATE_VISION", 12 },
    { "LEADERSHIP", 1 },
    { "LEVEL", 6 },
    { "NONE", 0 },
    { "SPELL", 3 },
    { "STRUCTURE", 5 },
    { "STUN", 9 },
    { "WEAPON", 4 },
};
const AIEnum supportAttributeModifierCategoryType = { supportAttributeModifierCategoryTypeNames, 15u, 7, 15u };
const EnumNameValue supportAttributeModifierAuraUpdateRequiredNames[] = {
    { "MOUNTED", 0 },
};
const AIEnum supportAttributeModifierAuraUpdateRequired = { supportAttributeModifierAuraUpdateRequiredNames, 1u, 0, 1u };
const EnumNameValue supportDisabledTypeNames[] = {
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
const AIEnum supportDisabledType = { supportDisabledTypeNames, 12u, 0, 12u };
const EnumNameValue supportFXListBehaviorTypeNames[] = {
    { "onBecomingTemporaryOwner", 7 },
    { "onBecomingTemporarySlave", 6 },
    { "onCreate", 2 },
    { "onDamaged", 1 },
    { "onDeath", 0 },
    { "onEndingTemporaryOwner", 9 },
    { "onEndingTemporarySlave", 8 },
    { "onShieldSphereCollision", 11 },
    { "onSubmerging", 13 },
    { "onSurfacing", 12 },
    { "onTerrainCollision", 10 },
    { "onTransitionToDamaged", 3 },
    { "onTransitionToReallyDamaged", 4 },
    { "onTransitionToRubble", 5 },
};
const AIEnum supportFXListBehaviorType = { supportFXListBehaviorTypeNames, 14u, 7, 0u };
const EnumNameValue supportFXListBehaviorDirectionNames[] = {
    { "Default", 4 },
    { "Front", 0 },
    { "Rear", 1 },
    { "Side", 2 },
    { "Top", 3 },
};
const AIEnum supportFXListBehaviorDirection = { supportFXListBehaviorDirectionNames, 5u, 4, 0u };
const ModuleField supportQueryFields[] = {
    { "EMax", 0u, ModuleValue::Signed, "6", nullptr },
    { "Value", 4u, ModuleValue::Signed, "-1", nullptr },
};
const ModuleToken supportQueryTokens[] = {
    { 1341120459u, 602296504u, 0u, nullptr, 0u, 0u },
    { 3893002744u, 602296504u, 4u, nullptr, 0u, 0u },
    { 3832456114u, 3832456114u, 8u, nullptr, 7u, 0u },
};
const ModuleLayout supportQuery = { supportQueryTokens, 3u };
const ModuleField supportFXListBehaviorEventFields[] = {
    { "Index", 0u, ModuleValue::Enum, nullptr, &supportFXListBehaviorType },
    { "FX", 4u, ModuleValue::Reference, nullptr, nullptr },
    { "MinThreshold", 8u, ModuleValue::Float, "0", nullptr },
    { "MaxThreshold", 12u, ModuleValue::Float, "0", nullptr },
    { "OrientToObject", 20u, ModuleValue::Boolean, "true", nullptr },
    { "ForceUseDoFXObj", 21u, ModuleValue::Boolean, "false", nullptr },
    { "Direction", 16u, ModuleValue::Enum, "Default", &supportFXListBehaviorDirection },
};
const ModuleToken supportFXListBehaviorEventTokens[] = {
    { 1034518573u, 61836591u, 0u, nullptr, 0u, 0u },
    { 898024161u, 219975299u, 4u, nullptr, 0u, 0u },
    { 1003967157u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 1967413098u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 3497346223u, 2011428087u, 20u, nullptr, 0u, 0u },
    { 3779533864u, 2011428087u, 21u, nullptr, 0u, 0u },
    { 908073516u, 1578438664u, 16u, nullptr, 0u, 0u },
};
const ModuleLayout supportFXListBehaviorEvent = { supportFXListBehaviorEventTokens, 7u };
const ModuleField supportLocomotorSetModuleDataFields[] = {
    { "KillLocomotorUpgrade", 36u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleToken supportLocomotorSetModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 2908758514u, 2011428087u, 36u, nullptr, 0u, 0u },
};
const ModuleLayout supportLocomotorSetModuleData = { supportLocomotorSetModuleDataTokens, 2u };
const ModuleField supportSetLocomotorSpeedUpdateModuleDataFields[] = {
    { "Type", 8u, ModuleValue::Enum, "INVALID", &supportSetLocomotorSpeedUpdateType },
    { "LocomotorSpeedToSet", 12u, ModuleValue::Float, "0.0", nullptr },
    { "LocomotorDeltaPerUpdate", 16u, ModuleValue::Float, "0.0", nullptr },
    { "OtherScaredScanRangeMultiplier", 20u, ModuleValue::Float, "0.0", nullptr },
    { "ObjectFilterRange", 24u, ModuleValue::Float, "0.0", nullptr },
    { "UpdatePeriod", 28u, ModuleValue::Time, "1.0s", nullptr },
    { "VisionArc", 32u, ModuleValue::Angle, "180d", nullptr },
};
const ModuleToken supportSetLocomotorSpeedUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 2219907648u, 2361384232u, 8u, nullptr, 0u, 0u },
    { 145327958u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 3217246955u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 762780181u, 1683514611u, 20u, nullptr, 0u, 0u },
    { 3098758058u, 1683514611u, 24u, nullptr, 0u, 0u },
    { 3829086399u, 2575722984u, 28u, nullptr, 0u, 0u },
    { 1144468699u, 1310124621u, 32u, nullptr, 0u, 0u },
    { 3832456114u, 3832456114u, 36u, nullptr, 7u, 0u },
};
const ModuleLayout supportSetLocomotorSpeedUpdateModuleData = { supportSetLocomotorSpeedUpdateModuleDataTokens, 9u };
const ModuleField supportRestrictSpecialPowerBehaviorModuleDataFields[] = {
    { "RestrictionType", 8u, ModuleValue::Enum, nullptr, &supportSpecialPowerRestrictionType },
    { "DependentObject", 12u, ModuleValue::AssetId, nullptr, nullptr },
    { "ConsiderSpecialPowerRadius", 24u, ModuleValue::Boolean, "true", nullptr },
};
const ModuleToken supportRestrictSpecialPowerBehaviorModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &lifecycleBehaviorModuleData, 0u, 0u },
    { 384904913u, 729580760u, 8u, nullptr, 0u, 0u },
    { 3318598274u, 344905565u, 12u, nullptr, 0u, 0u },
    { 2723359786u, 2011428087u, 24u, nullptr, 0u, 0u },
    { 2587419965u, 1683514611u, 16u, nullptr, 3u, 0u },
};
const ModuleLayout supportRestrictSpecialPowerBehaviorModuleData = { supportRestrictSpecialPowerBehaviorModuleDataTokens, 5u };
const ModuleField supportPassiveAreaEffectBehaviorModuleDataFields[] = {
    { "EffectRadius", 8u, ModuleValue::Float, "0", nullptr },
    { "MaxBeneficiaries", 12u, ModuleValue::Unsigned, "0", nullptr },
    { "HealingPercentPerSecond", 16u, ModuleValue::Percent, "0", nullptr },
    { "HealingPointsPerSecond", 20u, ModuleValue::Float, "0", nullptr },
    { "SurveyDelayFrames", 24u, ModuleValue::Unsigned, "20", nullptr },
    { "UpgradeRequired", 28u, ModuleValue::Reference, nullptr, nullptr },
    { "NonStackable", 68u, ModuleValue::Boolean, "false", nullptr },
    { "AntiCategoryMask", 32u, ModuleValue::Bits, nullptr, &supportAttributeModifierCategoryType },
    { "AntiFX", 36u, ModuleValue::Reference, nullptr, nullptr },
    { "HealFX", 40u, ModuleValue::Reference, nullptr, nullptr },
    { "AffectAttached", 69u, ModuleValue::Boolean, "false", nullptr },
    { "AffectWoundedOnly", 70u, ModuleValue::Boolean, "false", nullptr },
    { "AffectUnderAttack", 71u, ModuleValue::Boolean, "false", nullptr },
    { "AffectContained", 72u, ModuleValue::Boolean, "false", nullptr },
    { "DisabledTypesToProcess", 44u, ModuleValue::Bits, nullptr, &supportDisabledType },
};
const ModuleToken supportPassiveAreaEffectBehaviorModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 386882139u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 624406373u, 1982953380u, 12u, nullptr, 0u, 0u },
    { 139575931u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 3741384635u, 1683514611u, 20u, nullptr, 0u, 0u },
    { 3822215771u, 1982953380u, 24u, nullptr, 0u, 0u },
    { 2084565028u, 1253711657u, 28u, nullptr, 0u, 0u },
    { 3807635175u, 2011428087u, 68u, nullptr, 0u, 0u },
    { 2546513103u, 1173445096u, 32u, nullptr, 0u, 0u },
    { 4240413615u, 219975299u, 36u, nullptr, 0u, 0u },
    { 1937770881u, 219975299u, 40u, nullptr, 0u, 0u },
    { 3270774743u, 2011428087u, 69u, nullptr, 0u, 0u },
    { 897405705u, 2011428087u, 70u, nullptr, 0u, 0u },
    { 2281253216u, 2011428087u, 71u, nullptr, 0u, 0u },
    { 437681369u, 2011428087u, 72u, nullptr, 0u, 0u },
    { 943614829u, 426078409u, 44u, nullptr, 0u, 0u },
    { 2698420400u, 165531635u, 48u, nullptr, 3u, 0u },
    { 1290071661u, 3832456114u, 56u, nullptr, 7u, 0u },
    { 1885606616u, 3832456114u, 60u, nullptr, 7u, 0u },
    { 3933465695u, 1049596812u, 64u, &weaponObjectStatusValidationDataType, 4u, 0u },
};
const ModuleLayout supportPassiveAreaEffectBehaviorModuleData = { supportPassiveAreaEffectBehaviorModuleDataTokens, 20u };
const ModuleField supportAttributeModifierAuraUpdateModuleDataFields[] = {
    { "AttributeModifierName", 36u, ModuleValue::Reference, nullptr, nullptr },
    { "RefreshDelay", 40u, ModuleValue::Time, nullptr, nullptr },
    { "Range", 44u, ModuleValue::Float, nullptr, nullptr },
    { "AllowPowerWhenAttacking", 96u, ModuleValue::Boolean, "true", nullptr },
    { "InitiallyActive", 97u, ModuleValue::Boolean, "false", nullptr },
    { "RequiredConditions", 48u, ModuleValue::Bits, nullptr, &supportAttributeModifierAuraUpdateRequired },
    { "AntiCategory", 52u, ModuleValue::Bits, nullptr, &supportAttributeModifierCategoryType },
    { "AntiFX", 56u, ModuleValue::Reference, nullptr, nullptr },
    { "RunWhileDead", 98u, ModuleValue::Boolean, nullptr, nullptr },
    { "AllowSelf", 99u, ModuleValue::Boolean, nullptr, nullptr },
    { "AffectContainedOnly", 100u, ModuleValue::Boolean, nullptr, nullptr },
    { "AffectHordeMembersOnly", 101u, ModuleValue::Boolean, nullptr, nullptr },
    { "AffectAttacheeOnly", 102u, ModuleValue::Boolean, nullptr, nullptr },
    { "MaxActiveRank", 60u, ModuleValue::Signed, nullptr, nullptr },
    { "RequiredObjectStatusFlags", 64u, ModuleValue::ObjectStatus, nullptr, nullptr },
    { "ShowDebugCircle", 103u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleToken supportAttributeModifierAuraUpdateModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 226106323u, 165531635u, 36u, nullptr, 0u, 0u },
    { 3742013746u, 2575722984u, 40u, nullptr, 0u, 0u },
    { 3426209446u, 1683514611u, 44u, nullptr, 0u, 0u },
    { 1950954445u, 2011428087u, 96u, nullptr, 0u, 0u },
    { 1454433568u, 2011428087u, 97u, nullptr, 0u, 0u },
    { 1294012426u, 1031726474u, 48u, nullptr, 0u, 0u },
    { 711753708u, 1173445096u, 52u, nullptr, 0u, 0u },
    { 4240413615u, 219975299u, 56u, nullptr, 0u, 0u },
    { 1662924473u, 2011428087u, 98u, nullptr, 0u, 0u },
    { 3435022180u, 2011428087u, 99u, nullptr, 0u, 0u },
    { 2326753257u, 2011428087u, 100u, nullptr, 0u, 0u },
    { 4204774495u, 2011428087u, 101u, nullptr, 0u, 0u },
    { 1047290712u, 2011428087u, 102u, nullptr, 0u, 0u },
    { 1262013020u, 602296504u, 60u, nullptr, 0u, 0u },
    { 2141005593u, 2814214430u, 64u, nullptr, 0u, 0u },
    { 2050255657u, 2011428087u, 103u, nullptr, 0u, 0u },
    { 3832456114u, 3832456114u, 92u, nullptr, 7u, 0u },
};
const ModuleLayout supportAttributeModifierAuraUpdateModuleData = { supportAttributeModifierAuraUpdateModuleDataTokens, 18u };
const ModuleField supportAutoAbilityBehaviorModuleDataFields[] = {
    { "MaxScanRange", 8u, ModuleValue::Float, "1", nullptr },
    { "MinScanRange", 12u, ModuleValue::Float, "0", nullptr },
    { "Radius", 16u, ModuleValue::Float, "0", nullptr },
    { "IdleSeconds", 20u, ModuleValue::Time, "0s", nullptr },
    { "SpecialAbilityName", 24u, ModuleValue::Reference, nullptr, nullptr },
    { "ForbiddenStatus", 28u, ModuleValue::ObjectStatus, nullptr, nullptr },
    { "StartsActive", 64u, ModuleValue::Boolean, "false", nullptr },
    { "UseStartPosition", 65u, ModuleValue::Boolean, "false", nullptr },
    { "AdjustAttackPos", 66u, ModuleValue::Boolean, "false", nullptr },
    { "AllowSelf", 67u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleToken supportAutoAbilityBehaviorModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 1988733171u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 1906490224u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 1328662213u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 3541024273u, 2575722984u, 20u, nullptr, 0u, 0u },
    { 3617336977u, 2934454408u, 24u, nullptr, 0u, 0u },
    { 824345085u, 2814214430u, 28u, nullptr, 0u, 0u },
    { 3455740364u, 2011428087u, 64u, nullptr, 0u, 0u },
    { 1815375446u, 2011428087u, 65u, nullptr, 0u, 0u },
    { 3068870907u, 2011428087u, 66u, nullptr, 0u, 0u },
    { 3435022180u, 2011428087u, 67u, nullptr, 0u, 0u },
    { 2314718166u, 405260724u, 56u, &supportQuery, 1u, 12u },
};
const ModuleLayout supportAutoAbilityBehaviorModuleData = { supportAutoAbilityBehaviorModuleDataTokens, 12u };
const ModuleField supportEmotionTrackerUpdateModuleDataFields[] = {
    { "TauntAndPointDistance", 8u, ModuleValue::Float, "0.0", nullptr },
    { "HeroScanDistance", 12u, ModuleValue::Float, "0.0", nullptr },
    { "TauntAndPointUpdateDelay", 16u, ModuleValue::Time, "2s", nullptr },
    { "FearScanDistance", 20u, ModuleValue::Float, "0.0", nullptr },
    { "IgnoreVeterancy", 52u, ModuleValue::Boolean, "false", nullptr },
    { "ImmuneToFearLevel", 24u, ModuleValue::Signed, "5", nullptr },
};
const ModuleToken supportEmotionTrackerUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 4285554915u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 169727143u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 1878100543u, 2575722984u, 16u, nullptr, 0u, 0u },
    { 569813769u, 1683514611u, 20u, nullptr, 0u, 0u },
    { 2839391183u, 2011428087u, 52u, nullptr, 0u, 0u },
    { 3285241533u, 602296504u, 24u, nullptr, 0u, 0u },
    { 2790838710u, 3832456114u, 28u, nullptr, 7u, 0u },
    { 1356802037u, 3832456114u, 32u, nullptr, 7u, 0u },
    { 607908341u, 3832456114u, 36u, nullptr, 7u, 0u },
    { 2035695822u, 3832456114u, 40u, nullptr, 7u, 0u },
    { 2736334765u, 2633945524u, 44u, nullptr, 3u, 0u },
};
const ModuleLayout supportEmotionTrackerUpdateModuleData = { supportEmotionTrackerUpdateModuleDataTokens, 12u };
const ModuleToken supportFXListBehaviorModuleDataTokens[] = {
    { 1u, 2655988594u, 0u, &dieDieModuleData, 0u, 0u },
    { 4208909635u, 1706983441u, 52u, &supportFXListBehaviorEvent, 1u, 24u },
};
const ModuleLayout supportFXListBehaviorModuleData = { supportFXListBehaviorModuleDataTokens, 2u };
const ModuleFieldSet supportFieldSets[] = {
    { supportLocomotorSetModuleDataFields, 1u },
    { supportSetLocomotorSpeedUpdateModuleDataFields, 7u },
    { supportRestrictSpecialPowerBehaviorModuleDataFields, 3u },
    { supportPassiveAreaEffectBehaviorModuleDataFields, 15u },
    { supportAttributeModifierAuraUpdateModuleDataFields, 16u },
    { supportAutoAbilityBehaviorModuleDataFields, 10u },
    { supportEmotionTrackerUpdateModuleDataFields, 6u },
    { nullptr, 0u },
};
const ModuleChoice supportChoices[] = {
    { 664395762u, &supportLocomotorSetModuleData, 40u, 244u },
    { 793065128u, &supportSetLocomotorSpeedUpdateModuleData, 40u, 245u },
    { 2503769947u, &supportRestrictSpecialPowerBehaviorModuleData, 28u, 246u },
    { 149050769u, &supportPassiveAreaEffectBehaviorModuleData, 76u, 247u },
    { 1578938331u, &supportAttributeModifierAuraUpdateModuleData, 104u, 248u },
    { 1285783358u, &supportAutoAbilityBehaviorModuleData, 68u, 249u },
    { 2842040686u, &supportEmotionTrackerUpdateModuleData, 56u, 250u },
    { 2960202212u, &supportFXListBehaviorModuleData, 60u, 251u },
};
