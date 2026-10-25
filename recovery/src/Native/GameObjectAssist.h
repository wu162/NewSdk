// Generated complete four-type assistance/placement contracts; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const EnumNameValue assistAssistanceHintTypeNames[] = {
    { "ALLOW_SHROUDED", 3 },
    { "FACING_AWAY", 2 },
    { "FACING_TOWARDS", 1 },
    { "NONE", 0 },
};
const AIEnum assistAssistanceHintTypeEnum = { assistAssistanceHintTypeNames, 4u, 3, 4u };
const EnumNameValue assistWeaponSlotTypeNames[] = {
    { "NO_WEAPON", 5 },
    { "PRIMARY_WEAPON", 0 },
    { "QUATERNARY_WEAPON", 3 },
    { "QUINARY_WEAPON", 4 },
    { "SECONDARY_WEAPON", 1 },
    { "TERTIARY_WEAPON", 2 },
};
const AIEnum assistWeaponSlotTypeEnum = { assistWeaponSlotTypeNames, 6u, 5, 0u };
const ModuleField assistAssistedPlacementInfoBehaviorModuleDataFields[] = {
    { "AllowShrouded", 32u, ModuleValue::Boolean, "true", nullptr },
};
const ModuleField assistAssistedPlacementBehaviorModuleDataFields[] = {
    { "PlacementHint", 8u, ModuleValue::Bits, "NONE", &assistAssistanceHintTypeEnum },
    { "SnappingDistance", 12u, ModuleValue::Float, "0", nullptr },
};
const ModuleField assistAttackAssistResponseBehaviorModuleDataFields[] = {
    { "ObjectStatus", 8u, ModuleValue::ObjectStatus, "", nullptr },
    { "WeaponSlotID", 36u, ModuleValue::Unsigned, nullptr, nullptr },
    { "WeaponSlotType", 40u, ModuleValue::Enum, nullptr, &assistWeaponSlotTypeEnum },
};
const ModuleField assistSpecialPowerAssistResponseBehaviorModuleDataFields[] = {
    { "SpecialPowerTemplate", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "ObjectStatus", 12u, ModuleValue::ObjectStatus, "", nullptr },
};
const ModuleField assistHintObjectMappingFields[] = {
    { "PlayerTemplate", 0u, ModuleValue::NullableAssetId, nullptr, nullptr },
    { "HintDisplayObject", 4u, ModuleValue::AssetId, nullptr, nullptr },
    { "HintOpacity", 8u, ModuleValue::Float, "1.0", nullptr },
    { "HintUsesShaders", 16u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleField assistPlacementRestrictionFields[] = {
    { "FilterRadius", 0u, ModuleValue::Float, "0.0", nullptr },
};
const ModuleField assistVector3Fields[] = {
    { "x", 0u, ModuleValue::Float, "0.0", nullptr },
    { "y", 4u, ModuleValue::Float, "0.0", nullptr },
    { "z", 8u, ModuleValue::Float, "0.0", nullptr },
};
const ModuleFieldSet assistFieldSets[] = {
    { assistAssistedPlacementInfoBehaviorModuleDataFields, 1u },
    { assistAssistedPlacementBehaviorModuleDataFields, 2u },
    { assistAttackAssistResponseBehaviorModuleDataFields, 3u },
    { assistSpecialPowerAssistResponseBehaviorModuleDataFields, 2u },
    { assistHintObjectMappingFields, 4u },
    { assistPlacementRestrictionFields, 1u },
    { assistVector3Fields, 3u },
};
const ModuleToken assistHintObjectMappingTokens[] = {
    { 4063352333u, 1873548347u, 0u, nullptr, 2u, 0u },
    { 940843326u, 344905565u, 4u, nullptr, 0u, 0u },
    { 1310225065u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 2319038693u, 2011428087u, 16u, nullptr, 0u, 0u },
    { 3832456114u, 3832456114u, 12u, nullptr, 7u, 0u },
};
const ModuleLayout assistHintObjectMapping = { assistHintObjectMappingTokens, 5u };
const ModuleToken assistPlacementRestrictionTokens[] = {
    { 1065525139u, 1683514611u, 0u, nullptr, 0u, 0u },
    { 3583409053u, 3832456114u, 4u, nullptr, 7u, 0u },
};
const ModuleLayout assistPlacementRestriction = { assistPlacementRestrictionTokens, 2u };
const ModuleToken assistAssistedPlacementInfoBehaviorModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &reactiveBehaviorModuleData, 0u, 0u },
    { 1756596634u, 2011428087u, 32u, nullptr, 0u, 0u },
    { 3214471090u, 2731727911u, 8u, &assistHintObjectMapping, 1u, 20u },
    { 664930705u, 664930705u, 16u, &assistPlacementRestriction, 4u, 0u },
    { 223948434u, 691437687u, 20u, nullptr, 0u, 0u },
};
const ModuleLayout assistAssistedPlacementInfoBehaviorModuleData = { assistAssistedPlacementInfoBehaviorModuleDataTokens, 5u };
const ModuleToken assistAssistedPlacementBehaviorModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &reactiveBehaviorModuleData, 0u, 0u },
    { 2386547037u, 4204322381u, 8u, nullptr, 0u, 0u },
    { 1807410905u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 2549286788u, 3832456114u, 16u, nullptr, 7u, 0u },
};
const ModuleLayout assistAssistedPlacementBehaviorModuleData = { assistAssistedPlacementBehaviorModuleDataTokens, 4u };
const ModuleToken assistAttackAssistResponseBehaviorModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &reactiveBehaviorModuleData, 0u, 0u },
    { 3963816762u, 2814214430u, 8u, nullptr, 0u, 0u },
    { 1579523250u, 1982953380u, 36u, nullptr, 0u, 0u },
    { 407422281u, 407422281u, 40u, nullptr, 0u, 0u },
};
const ModuleLayout assistAttackAssistResponseBehaviorModuleData = { assistAttackAssistResponseBehaviorModuleDataTokens, 4u };
const ModuleToken assistSpecialPowerAssistResponseBehaviorModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &reactiveBehaviorModuleData, 0u, 0u },
    { 2178440954u, 2934454408u, 8u, nullptr, 0u, 0u },
    { 3963816762u, 2814214430u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout assistSpecialPowerAssistResponseBehaviorModuleData = { assistSpecialPowerAssistResponseBehaviorModuleDataTokens, 3u };
const ModuleChoice assistChoices[] = {
    { 2328709842u, &assistAssistedPlacementInfoBehaviorModuleData, 36u, 119u },
    { 2920212896u, &assistAssistedPlacementBehaviorModuleData, 20u, 120u },
    { 3962605070u, &assistAttackAssistResponseBehaviorModuleData, 44u, 121u },
    { 3358372044u, &assistSpecialPowerAssistResponseBehaviorModuleData, 40u, 122u },
};
