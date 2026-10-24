// Generated original radar/visibility/Capture contracts; blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const EnumNameValue visionUnifiedControlTypeNames[] = {
    { "ALLIED_PLAYERS", 1 },
    { "SAME_PLAYER", 0 },
};
const AIEnum visionUnifiedControlType = { visionUnifiedControlTypeNames, 2u, 1, 0u };
const EnumNameValue visionRelationshipTypeNames[] = {
    { "ALLIES", 2 },
    { "ENEMIES", 0 },
    { "NEUTRAL", 1 },
};
const AIEnum visionRelationshipType = { visionRelationshipTypeNames, 3u, 2, 3u };
const ModuleField visionCaptureBehaviorModuleDataFields[] = {
    { "GrantMoney", 8u, ModuleValue::Unsigned, "0", nullptr },
    { "GrantMoneyOneTime", 16u, ModuleValue::Boolean, "false", nullptr },
    { "SlavedCaptureObject", 12u, ModuleValue::AssetId, nullptr, nullptr },
};
const ModuleField visionUnifiedControlBehaviorModuleDataFields[] = {
    { "UnificationType", 20u, ModuleValue::Enum, "SAME_PLAYER", &visionUnifiedControlType },
    { "GrantMoney", 8u, ModuleValue::Unsigned, "0", nullptr },
    { "GrantMoneyOneTime", 16u, ModuleValue::Boolean, "false", nullptr },
    { "SlavedCaptureObject", 12u, ModuleValue::AssetId, nullptr, nullptr },
};
const ModuleField visionRadarUpdateModuleDataFields[] = {
    { "RadarExtendTime", 8u, ModuleValue::Float, "0", nullptr },
};
const ModuleField visionUnitVisionBehaviorModuleDataFields[] = {
    { "UpdateRate", 8u, ModuleValue::Signed, "1", nullptr },
};
const ModuleField visionSelectiveVisibilityBehaviorModuleDataFields[] = {
    { "UpdateRate", 8u, ModuleValue::NullableTime, nullptr, nullptr },
    { "AllowedPlayers", 12u, ModuleValue::NullableBits, nullptr, &visionRelationshipType },
};
const ModuleField visionInvisibilityBehaviorModuleDataFields[] = {
    { "InvisibilityTemplate", 8u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleField visionInvisibilityBroadcastUpdateModuleDataFields[] = {
    { "InvisibilityTemplate", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "AffectsSelf", 40u, ModuleValue::Boolean, "true", nullptr },
    { "UpdatePeriod", 12u, ModuleValue::Time, "1s", nullptr },
    { "BroadcastRange", 16u, ModuleValue::Float, "0", nullptr },
    { "ScalableDrawModuleId", 20u, ModuleValue::AssetId, nullptr, nullptr },
    { "ScaledBoneName", 24u, ModuleValue::String, nullptr, nullptr },
};
const ModuleField visionUnifiedShroudRevealBehaviorModuleDataFields[] = {
    { "UnificationType", 20u, ModuleValue::Enum, "SAME_PLAYER", &visionUnifiedControlType },
    { "GrantMoney", 8u, ModuleValue::Unsigned, "0", nullptr },
    { "GrantMoneyOneTime", 16u, ModuleValue::Boolean, "false", nullptr },
    { "SlavedCaptureObject", 12u, ModuleValue::AssetId, nullptr, nullptr },
};
const ModuleFieldSet visionFieldSets[] = {
    { visionCaptureBehaviorModuleDataFields, 3u },
    { visionUnifiedControlBehaviorModuleDataFields, 4u },
    { visionRadarUpdateModuleDataFields, 1u },
    { visionUnitVisionBehaviorModuleDataFields, 1u },
    { visionSelectiveVisibilityBehaviorModuleDataFields, 2u },
    { visionInvisibilityBehaviorModuleDataFields, 1u },
    { visionInvisibilityBroadcastUpdateModuleDataFields, 6u },
    { visionUnifiedShroudRevealBehaviorModuleDataFields, 4u },
};
const ModuleToken visionModuleDataTokens[] = {
    { 430981792u, 602296504u, 0u, nullptr, 0u, 0u },
    { 2363267224u, 3186653541u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout visionModuleData = { visionModuleDataTokens, 2u };
const ModuleToken visionBehaviorModuleDataTokens[] = {
    { 1u, 3613234082u, 0u, &visionModuleData, 0u, 0u },
};
const ModuleLayout visionBehaviorModuleData = { visionBehaviorModuleDataTokens, 1u };
const ModuleToken visionCaptureBehaviorModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &visionBehaviorModuleData, 0u, 0u },
    { 1876756090u, 1982953380u, 8u, nullptr, 0u, 0u },
    { 660992753u, 2011428087u, 16u, nullptr, 0u, 0u },
    { 1691630949u, 344905565u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout visionCaptureBehaviorModuleData = { visionCaptureBehaviorModuleDataTokens, 4u };
const ModuleToken visionUnifiedControlBehaviorModuleDataTokens[] = {
    { 1u, 4173638251u, 0u, &visionCaptureBehaviorModuleData, 0u, 0u },
    { 799470543u, 431396024u, 20u, nullptr, 0u, 0u },
};
const ModuleLayout visionUnifiedControlBehaviorModuleData = { visionUnifiedControlBehaviorModuleDataTokens, 2u };
const ModuleToken visionContainModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &visionBehaviorModuleData, 0u, 0u },
};
const ModuleLayout visionContainModuleData = { visionContainModuleDataTokens, 1u };
const ModuleToken visionUpdateModuleDataTokens[] = {
    { 1u, 687874333u, 0u, &visionContainModuleData, 0u, 0u },
};
const ModuleLayout visionUpdateModuleData = { visionUpdateModuleDataTokens, 1u };
const ModuleToken visionRadarUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &visionUpdateModuleData, 0u, 0u },
    { 2977561024u, 1683514611u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout visionRadarUpdateModuleData = { visionRadarUpdateModuleDataTokens, 2u };
const ModuleToken visionUnitVisionBehaviorModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &visionBehaviorModuleData, 0u, 0u },
    { 4247121054u, 602296504u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout visionUnitVisionBehaviorModuleData = { visionUnitVisionBehaviorModuleDataTokens, 2u };
const ModuleToken visionSelectiveVisibilityBehaviorModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &visionBehaviorModuleData, 0u, 0u },
    { 4247121054u, 2575722984u, 8u, nullptr, 2u, 0u },
    { 1846086947u, 1896218355u, 12u, nullptr, 2u, 0u },
};
const ModuleLayout visionSelectiveVisibilityBehaviorModuleData = { visionSelectiveVisibilityBehaviorModuleDataTokens, 3u };
const ModuleToken visionInvisibilityBehaviorModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &visionBehaviorModuleData, 0u, 0u },
    { 1431290504u, 2437719909u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout visionInvisibilityBehaviorModuleData = { visionInvisibilityBehaviorModuleDataTokens, 2u };
const ModuleToken visionObjectStatusValidationDataTypeTokens[] = {
    { 824345085u, 2814214430u, 0u, nullptr, 0u, 0u },
    { 597602817u, 2814214430u, 28u, nullptr, 0u, 0u },
};
const ModuleLayout visionObjectStatusValidationDataType = { visionObjectStatusValidationDataTypeTokens, 2u };
const ModuleToken visionInvisibilityBroadcastUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &visionUpdateModuleData, 0u, 0u },
    { 1431290504u, 2437719909u, 8u, nullptr, 0u, 0u },
    { 4063025140u, 2011428087u, 40u, nullptr, 0u, 0u },
    { 3829086399u, 2575722984u, 12u, nullptr, 0u, 0u },
    { 2597499286u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 3136375604u, 3186653541u, 20u, nullptr, 0u, 0u },
    { 1460382439u, 438907818u, 24u, nullptr, 0u, 0u },
    { 4220471451u, 3832456114u, 32u, nullptr, 7u, 0u },
    { 3933465695u, 1049596812u, 36u, &visionObjectStatusValidationDataType, 4u, 0u },
};
const ModuleLayout visionInvisibilityBroadcastUpdateModuleData = { visionInvisibilityBroadcastUpdateModuleDataTokens, 9u };
const ModuleToken visionUnifiedShroudRevealBehaviorModuleDataTokens[] = {
    { 1u, 3179117790u, 0u, &visionUnifiedControlBehaviorModuleData, 0u, 0u },
};
const ModuleLayout visionUnifiedShroudRevealBehaviorModuleData = { visionUnifiedShroudRevealBehaviorModuleDataTokens, 1u };
const ModuleChoice visionChoices[] = {
    { 4173638251u, &visionCaptureBehaviorModuleData, 20u, 61u },
    { 3179117790u, &visionUnifiedControlBehaviorModuleData, 24u, 62u },
    { 1768048089u, &visionRadarUpdateModuleData, 12u, 63u },
    { 4070192848u, &visionUnitVisionBehaviorModuleData, 12u, 64u },
    { 1632936356u, &visionSelectiveVisibilityBehaviorModuleData, 16u, 65u },
    { 88803780u, &visionInvisibilityBehaviorModuleData, 12u, 66u },
    { 2984624090u, &visionInvisibilityBroadcastUpdateModuleData, 44u, 67u },
    { 2339238473u, &visionUnifiedShroudRevealBehaviorModuleData, 24u, 68u },
};
