// Generated pinned original Animation60, AnimationState140, LuaEvent16 contracts.
const EnumNameValue drawAnimAnimationModeNames[] = {
    { "LOOP", 1 },
    { "LOOP_BACKWARDS", 3 },
    { "LOOP_PINGPONG", 5 },
    { "MANUAL", 0 },
    { "MATCH_UNPACKING", 7 },
    { "ONCE", 2 },
    { "ONCE_BACKWARDS", 4 },
    { "PLAY_TO_FRAME", 6 },
};
const AIEnum drawAnimAnimationMode = { drawAnimAnimationModeNames, 8u, 1, 0u };
const EnumNameValue drawAnimParseCondStateTypeNames[] = {
    { "PARSE_DEFAULT", 1 },
    { "PARSE_NORMAL", 0 },
    { "PARSE_TRANSITION", 2 },
};
const AIEnum drawAnimParseCondStateType = { drawAnimParseCondStateTypeNames, 3u, 1, 0u };
const EnumNameValue drawAnimAnimationStateFlagNames[] = {
    { "ADJUST_HEIGHT_BY_CONSTRUCTION_PERCENT", 10 },
    { "DO_NOT_PLAY_WHEN_UNPOWERED", 9 },
    { "IGNORE_MOVEMENT_SPEED", 11 },
    { "MAINTAIN_FRAME_ACROSS_STATES", 5 },
    { "MAINTAIN_FRAME_ACROSS_STATES2", 6 },
    { "MAINTAIN_FRAME_ACROSS_STATES3", 7 },
    { "MAINTAIN_FRAME_ACROSS_STATES4", 8 },
    { "NONE", 0 },
    { "RANDOMSTART", 1 },
    { "RESTART_ANIM_WHEN_COMPLETE", 4 },
    { "START_FRAME_FIRST", 2 },
    { "START_FRAME_LAST", 3 },
};
const AIEnum drawAnimAnimationStateFlag = { drawAnimAnimationStateFlagNames, 12u, 10, 12u };
const EnumNameValue drawAnimLuaEventTriggerTypeNames[] = {
    { "TRIGGER_ONFRAME", 0 },
    { "TRIGGER_ONSTATEENTER", 1 },
    { "TRIGGER_ONSTATELEAVE", 2 },
};
const AIEnum drawAnimLuaEventTriggerType = { drawAnimLuaEventTriggerTypeNames, 3u, 0, 0u };
const EnumNameValue drawAnimWeaponSlotTypeNames[] = {
    { "NO_WEAPON", 5 },
    { "PRIMARY_WEAPON", 0 },
    { "QUATERNARY_WEAPON", 3 },
    { "QUINARY_WEAPON", 4 },
    { "SECONDARY_WEAPON", 1 },
    { "TERTIARY_WEAPON", 2 },
};
const AIEnum drawAnimWeaponSlotType = { drawAnimWeaponSlotTypeNames, 6u, 5, 0u };
const ModuleField drawAnimationFields[] = {
    { "Flags", 0u, ModuleValue::String, nullptr, nullptr },
    { "AnimationName", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "AnimationMode", 12u, ModuleValue::Enum, nullptr, &drawAnimAnimationMode },
    { "AnimNickName", 16u, ModuleValue::String, nullptr, nullptr },
    { "Distance", 24u, ModuleValue::Float, "0.0", nullptr },
    { "AnimationBlendTime", 28u, ModuleValue::Float, "5.0", nullptr },
    { "AnimationMustCompleteBlend", 56u, ModuleValue::Boolean, "false", nullptr },
    { "AnimationSpeedFactorMin", 32u, ModuleValue::Float, "1.0", nullptr },
    { "AnimationSpeedFactorMax", 36u, ModuleValue::Float, "1.0", nullptr },
    { "AnimationAbsoluteTime", 40u, ModuleValue::Time, "0.0s", nullptr },
    { "UseWeaponTiming", 57u, ModuleValue::Boolean, "false", nullptr },
    { "WeaponTimingOrdering", 44u, ModuleValue::Enum, nullptr, &drawAnimWeaponSlotType },
    { "WeaponTimingSlotID", 48u, ModuleValue::Signed, "1", nullptr },
    { "AnimationPriority", 52u, ModuleValue::Signed, "1", nullptr },
};
const ModuleFieldSet drawAnimationFieldSet = { drawAnimationFields, 14u };
const ModuleToken drawAnimationTokens[] = {
    { 4081386890u, 438907818u, 0u, nullptr, 0u, 0u },
    { 1215954844u, 4264414083u, 8u, nullptr, 0u, 0u },
    { 382203096u, 3821913990u, 12u, nullptr, 0u, 0u },
    { 2511072322u, 438907818u, 16u, nullptr, 0u, 0u },
    { 32598927u, 1683514611u, 24u, nullptr, 0u, 0u },
    { 2782472672u, 1683514611u, 28u, nullptr, 0u, 0u },
    { 3468862718u, 2011428087u, 56u, nullptr, 0u, 0u },
    { 67625028u, 1683514611u, 32u, nullptr, 0u, 0u },
    { 3449140074u, 1683514611u, 36u, nullptr, 0u, 0u },
    { 993036170u, 2575722984u, 40u, nullptr, 0u, 0u },
    { 2337654849u, 2011428087u, 57u, nullptr, 0u, 0u },
    { 1568531037u, 407422281u, 44u, nullptr, 0u, 0u },
    { 3287345543u, 602296504u, 48u, nullptr, 0u, 0u },
    { 1771418849u, 602296504u, 52u, nullptr, 0u, 0u },
};
const ModuleLayout drawAnimation = { drawAnimationTokens, 14u };
const ModuleField drawLuaEventFields[] = {
    { "Frame", 0u, ModuleValue::Signed, nullptr, nullptr },
    { "Data", 4u, ModuleValue::String, nullptr, nullptr },
    { "Trigger", 12u, ModuleValue::Enum, "TRIGGER_ONFRAME", &drawAnimLuaEventTriggerType },
};
const ModuleFieldSet drawLuaEventFieldSet = { drawLuaEventFields, 3u };
const ModuleToken drawLuaEventTokens[] = {
    { 1798861906u, 602296504u, 0u, nullptr, 0u, 0u },
    { 2582625313u, 438907818u, 4u, nullptr, 0u, 0u },
    { 2187291630u, 3243606648u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout drawLuaEvent = { drawLuaEventTokens, 3u };
const ModuleField drawAnimationStateFields[] = {
    { "ParseCondStateType", 0u, ModuleValue::Enum, nullptr, &drawAnimParseCondStateType },
    { "AnimNickName", 4u, ModuleValue::String, nullptr, nullptr },
    { "ConditionsYes", 12u, ModuleValue::ModelCondition, nullptr, nullptr },
    { "Name", 72u, ModuleValue::String, nullptr, nullptr },
    { "StateName", 80u, ModuleValue::String, nullptr, nullptr },
    { "Flags", 88u, ModuleValue::Bits, "NONE", &drawAnimAnimationStateFlag },
    { "ShareAnimation", 136u, ModuleValue::Boolean, "false", nullptr },
    { "EnteringStateFX", 92u, ModuleValue::Reference, nullptr, nullptr },
    { "FrameForPristineBonePositions", 96u, ModuleValue::Signed, "0", nullptr },
    { "AllowRepeatInRandomPick", 137u, ModuleValue::Boolean, "false", nullptr },
    { "SimilarRestart", 138u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleFieldSet drawAnimationStateFieldSet = { drawAnimationStateFields, 11u };
const ModuleToken drawAnimationStateTokens[] = {
    { 321108080u, 321108080u, 0u, nullptr, 0u, 0u },
    { 2511072322u, 438907818u, 4u, nullptr, 0u, 0u },
    { 3741888440u, 1742454729u, 12u, nullptr, 0u, 0u },
    { 1501410244u, 438907818u, 72u, nullptr, 0u, 0u },
    { 1358786456u, 438907818u, 80u, nullptr, 0u, 0u },
    { 4081386890u, 1589818194u, 88u, nullptr, 0u, 0u },
    { 1221602444u, 2011428087u, 136u, nullptr, 0u, 0u },
    { 897337300u, 219975299u, 92u, nullptr, 0u, 0u },
    { 1813126156u, 602296504u, 96u, nullptr, 0u, 0u },
    { 1455075458u, 2011428087u, 137u, nullptr, 0u, 0u },
    { 2364691681u, 2011428087u, 138u, nullptr, 0u, 0u },
    { 1486976473u, 1486976473u, 100u, &drawAnimation, 1u, 60u },
    { 407356560u, 407356560u, 108u, nullptr, 2u, 0u },
    { 1972968147u, 1972968147u, 112u, &drawModelFXEvent, 1u, 28u },
    { 3657821042u, 3657821042u, 120u, &drawLuaEvent, 1u, 16u },
    { 2563063782u, 2563063782u, 128u, &drawModelParticleSysBone, 1u, 36u },
};
const ModuleLayout drawAnimationState = { drawAnimationStateTokens, 16u };
