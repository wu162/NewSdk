// Generated from pinned original draw-source methods; complete model conditions.
const EnumNameValue drawModelParseCondStateTypeNames[] = {
    { "PARSE_DEFAULT", 1 },
    { "PARSE_NORMAL", 0 },
    { "PARSE_TRANSITION", 2 },
};
const AIEnum drawModelParseCondStateType = { drawModelParseCondStateTypeNames, 3u, 1, 0u };
const EnumNameValue drawModelFXTriggerTypeNames[] = {
    { "CATAPULT_ROCK", 1 },
    { "NONE", 0 },
    { "TREBUCHET_ROCK", 2 },
};
const AIEnum drawModelFXTriggerType = { drawModelFXTriggerTypeNames, 3u, 1, 0u };
const EnumNameValue drawModelFXActionTypeNames[] = {
    { "HOLD", 1 },
    { "KILL", 2 },
    { "NONE", 0 },
    { "SPAWN", 3 },
};
const AIEnum drawModelFXActionType = { drawModelFXActionTypeNames, 4u, 1, 0u };
const EnumNameValue drawModelShadowTypeNames[] = {
    { "ADDITIVE_DECAL", 3 },
    { "ADDITIVE_DECAL_DYNAMIC", 5 },
    { "ALPHA_DECAL", 2 },
    { "ALPHA_DECAL_DYNAMIC", 4 },
    { "DECAL", 1 },
    { "MERGE_DECAL", 6 },
    { "NONE", 0 },
};
const AIEnum drawModelShadowType = { drawModelShadowTypeNames, 7u, 3, 0u };
const EnumNameValue drawModelWeaponSlotTypeNames[] = {
    { "NO_WEAPON", 5 },
    { "PRIMARY_WEAPON", 0 },
    { "QUATERNARY_WEAPON", 3 },
    { "QUINARY_WEAPON", 4 },
    { "SECONDARY_WEAPON", 1 },
    { "TERTIARY_WEAPON", 2 },
};
const AIEnum drawModelWeaponSlotType = { drawModelWeaponSlotTypeNames, 6u, 5, 0u };
const ModuleField drawModelScriptedModelDrawModelFields[] = {
    { "Name", 0u, ModuleValue::Reference, nullptr, nullptr },
    { "ExtraMesh", 4u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleToken drawModelScriptedModelDrawModelTokens[] = {
    { 1501410244u, 4239460053u, 0u, nullptr, 0u, 0u },
    { 3757562318u, 2011428087u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout drawModelScriptedModelDrawModel = { drawModelScriptedModelDrawModelTokens, 2u };
const ModuleField drawModelReplaceTextureFields[] = {
    { "Original", 0u, ModuleValue::Reference, nullptr, nullptr },
    { "New", 4u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleToken drawModelReplaceTextureTokens[] = {
    { 1287025308u, 1279146296u, 0u, nullptr, 0u, 0u },
    { 2758584799u, 1279146296u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout drawModelReplaceTexture = { drawModelReplaceTextureTokens, 2u };
const ModuleField drawModelBoneAttachPointFields[] = {
    { "WeaponSlotID", 0u, ModuleValue::Signed, "1", nullptr },
    { "WeaponSlotType", 4u, ModuleValue::Enum, "PRIMARY_WEAPON", &drawModelWeaponSlotType },
    { "BoneName", 8u, ModuleValue::String, nullptr, nullptr },
};
const ModuleToken drawModelBoneAttachPointTokens[] = {
    { 1579523250u, 602296504u, 0u, nullptr, 0u, 0u },
    { 407422281u, 407422281u, 4u, nullptr, 0u, 0u },
    { 4048210012u, 438907818u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout drawModelBoneAttachPoint = { drawModelBoneAttachPointTokens, 3u };
const ModuleField drawModelParticleSysBoneFields[] = {
    { "BoneName", 0u, ModuleValue::String, nullptr, nullptr },
    { "FXParticleSystemTemplate", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "FollowBone", 32u, ModuleValue::Boolean, "false", nullptr },
    { "FXTrigger", 12u, ModuleValue::Enum, "NONE", &drawModelFXTriggerType },
    { "FXAction", 16u, ModuleValue::Enum, "NONE", &drawModelFXActionType },
    { "PersistID", 20u, ModuleValue::Signed, "0", nullptr },
    { "OnlyIfOnWater", 33u, ModuleValue::Boolean, "false", nullptr },
    { "OnlyIfOnLand", 34u, ModuleValue::Boolean, "false", nullptr },
    { "id", 24u, ModuleValue::String, nullptr, nullptr },
};
const ModuleToken drawModelParticleSysBoneTokens[] = {
    { 4048210012u, 438907818u, 0u, nullptr, 0u, 0u },
    { 686292351u, 1139920438u, 8u, nullptr, 0u, 0u },
    { 2498831791u, 2011428087u, 32u, nullptr, 0u, 0u },
    { 3290909714u, 2156627390u, 12u, nullptr, 0u, 0u },
    { 1141223022u, 2085207091u, 16u, nullptr, 0u, 0u },
    { 2646787814u, 602296504u, 20u, nullptr, 0u, 0u },
    { 2971070198u, 2011428087u, 33u, nullptr, 0u, 0u },
    { 159295449u, 2011428087u, 34u, nullptr, 0u, 0u },
    { 2363267224u, 2153588212u, 24u, nullptr, 0u, 0u },
};
const ModuleLayout drawModelParticleSysBone = { drawModelParticleSysBoneTokens, 9u };
const ModuleField drawModelFXEventFields[] = {
    { "Frame", 0u, ModuleValue::Signed, nullptr, nullptr },
    { "FrameStep", 4u, ModuleValue::Signed, nullptr, nullptr },
    { "FrameStop", 8u, ModuleValue::Signed, nullptr, nullptr },
    { "FireWhenSkipped", 24u, ModuleValue::Boolean, "false", nullptr },
    { "Effect", 12u, ModuleValue::Reference, nullptr, nullptr },
    { "Bone", 16u, ModuleValue::String, nullptr, nullptr },
};
const ModuleToken drawModelFXEventTokens[] = {
    { 1798861906u, 602296504u, 0u, nullptr, 0u, 0u },
    { 3063727352u, 602296504u, 4u, nullptr, 0u, 0u },
    { 2310731213u, 602296504u, 8u, nullptr, 0u, 0u },
    { 648584851u, 2011428087u, 24u, nullptr, 0u, 0u },
    { 884957657u, 219975299u, 12u, nullptr, 0u, 0u },
    { 883283766u, 438907818u, 16u, nullptr, 0u, 0u },
};
const ModuleLayout drawModelFXEvent = { drawModelFXEventTokens, 6u };
const ModuleField drawModelModelConditionStateTurretFields[] = {
    { "TurretNameKey", 0u, ModuleValue::String, nullptr, nullptr },
    { "TurretArtAngle", 8u, ModuleValue::Angle, nullptr, nullptr },
    { "TurretPitch", 12u, ModuleValue::String, nullptr, nullptr },
    { "TurretArtPitch", 20u, ModuleValue::Angle, nullptr, nullptr },
    { "TurretID", 24u, ModuleValue::Signed, "-1", nullptr },
};
const ModuleToken drawModelModelConditionStateTurretTokens[] = {
    { 2647352561u, 438907818u, 0u, nullptr, 0u, 0u },
    { 14090048u, 1310124621u, 8u, nullptr, 0u, 0u },
    { 196191532u, 438907818u, 12u, nullptr, 0u, 0u },
    { 681536604u, 1310124621u, 20u, nullptr, 0u, 0u },
    { 2218272382u, 602296504u, 24u, nullptr, 0u, 0u },
};
const ModuleLayout drawModelModelConditionStateTurret = { drawModelModelConditionStateTurretTokens, 5u };
const ModuleField drawModelShadowInfoFields[] = {
    { "Type", 0u, ModuleValue::Enum, nullptr, &drawModelShadowType },
    { "Texture", 4u, ModuleValue::Reference, nullptr, nullptr },
    { "AdditionalTexture", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "SizeX", 12u, ModuleValue::Float, "0.0", nullptr },
    { "SizeY", 16u, ModuleValue::Float, "0.0", nullptr },
    { "OffsetX", 20u, ModuleValue::Float, "0.0", nullptr },
    { "OffsetY", 24u, ModuleValue::Float, "0.0", nullptr },
    { "OpacityStart", 28u, ModuleValue::Float, "0", nullptr },
    { "OpacityFadeInTime", 32u, ModuleValue::Time, "0s", nullptr },
    { "OpacityPeak", 36u, ModuleValue::Float, "1", nullptr },
    { "OpacityFadeOutTime", 40u, ModuleValue::Time, "0s", nullptr },
    { "OpacityEnd", 44u, ModuleValue::Float, "0", nullptr },
    { "MaxHeight", 48u, ModuleValue::Float, "20", nullptr },
    { "OverrideLODVisibility", 56u, ModuleValue::Boolean, "false", nullptr },
    { "UseHouseColor", 57u, ModuleValue::Boolean, "false", nullptr },
    { "IsRotatingWithObject", 58u, ModuleValue::Boolean, "true", nullptr },
    { "LocalPlayerOnly", 59u, ModuleValue::Boolean, "false", nullptr },
    { "SunAngle", 52u, ModuleValue::Float, "0.0", nullptr },
};
const ModuleToken drawModelShadowInfoTokens[] = {
    { 2219907648u, 995629393u, 0u, nullptr, 0u, 0u },
    { 568797146u, 1279146296u, 4u, nullptr, 0u, 0u },
    { 3810401382u, 1279146296u, 8u, nullptr, 0u, 0u },
    { 1331287495u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 701354670u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 3535499909u, 1683514611u, 20u, nullptr, 0u, 0u },
    { 458018766u, 1683514611u, 24u, nullptr, 0u, 0u },
    { 980905958u, 1683514611u, 28u, nullptr, 0u, 0u },
    { 1591653543u, 2575722984u, 32u, nullptr, 0u, 0u },
    { 2256853385u, 1683514611u, 36u, nullptr, 0u, 0u },
    { 1338199621u, 2575722984u, 40u, nullptr, 0u, 0u },
    { 403917391u, 1683514611u, 44u, nullptr, 0u, 0u },
    { 2442743418u, 1683514611u, 48u, nullptr, 0u, 0u },
    { 1465894513u, 2011428087u, 56u, nullptr, 0u, 0u },
    { 1377712240u, 2011428087u, 57u, nullptr, 0u, 0u },
    { 2723964047u, 2011428087u, 58u, nullptr, 0u, 0u },
    { 3633590885u, 2011428087u, 59u, nullptr, 0u, 0u },
    { 1902020174u, 1683514611u, 52u, nullptr, 0u, 0u },
};
const ModuleLayout drawModelShadowInfo = { drawModelShadowInfoTokens, 18u };
const ModuleField drawModelModelConditionStateFields[] = {
    { "ParseCondStateType", 0u, ModuleValue::Enum, nullptr, &drawModelParseCondStateType },
    { "Name", 4u, ModuleValue::String, nullptr, nullptr },
    { "ConditionsYes", 12u, ModuleValue::ModelCondition, nullptr, nullptr },
    { "Skeleton", 72u, ModuleValue::String, nullptr, nullptr },
    { "PortraitImage", 80u, ModuleValue::Reference, nullptr, nullptr },
    { "ButtonImage", 84u, ModuleValue::Reference, nullptr, nullptr },
    { "OverrideTooltip", 88u, ModuleValue::String, nullptr, nullptr },
    { "RetainSubObjects", 192u, ModuleValue::Boolean, "false", nullptr },
    { "id", 96u, ModuleValue::String, nullptr, nullptr },
};
const ModuleToken drawModelModelConditionStateTokens[] = {
    { 321108080u, 321108080u, 0u, nullptr, 0u, 0u },
    { 1501410244u, 438907818u, 4u, nullptr, 0u, 0u },
    { 3741888440u, 1742454729u, 12u, nullptr, 0u, 0u },
    { 1066826308u, 2153588212u, 72u, nullptr, 0u, 0u },
    { 763037527u, 17349918u, 80u, nullptr, 0u, 0u },
    { 4171715770u, 17349918u, 84u, nullptr, 0u, 0u },
    { 2397097160u, 438907818u, 88u, nullptr, 0u, 0u },
    { 3983572700u, 2011428087u, 192u, nullptr, 0u, 0u },
    { 2363267224u, 2153588212u, 96u, nullptr, 0u, 0u },
    { 3586538858u, 3448685579u, 104u, &drawModelScriptedModelDrawModel, 1u, 8u },
    { 603709696u, 3354017012u, 112u, &drawBeamFXShaderMaterial, 4u, 0u },
    { 568797146u, 1851605243u, 116u, &drawModelReplaceTexture, 1u, 8u },
    { 97697089u, 3174683041u, 124u, &drawModelBoneAttachPoint, 1u, 16u },
    { 4124333999u, 3174683041u, 132u, &drawModelBoneAttachPoint, 1u, 16u },
    { 1893074230u, 3174683041u, 140u, &drawModelBoneAttachPoint, 1u, 16u },
    { 2833692462u, 3174683041u, 148u, &drawModelBoneAttachPoint, 1u, 16u },
    { 2563063782u, 2563063782u, 156u, &drawModelParticleSysBone, 1u, 36u },
    { 1972968147u, 1972968147u, 164u, &drawModelFXEvent, 1u, 28u },
    { 2197716616u, 2197716616u, 172u, &drawModelShadowInfo, 4u, 0u },
    { 3227510366u, 4077476099u, 176u, &drawModelModelConditionStateTurret, 1u, 28u },
    { 4202137261u, 60984384u, 184u, nullptr, 3u, 0u },
};
const ModuleLayout drawModelModelConditionState = { drawModelModelConditionStateTokens, 21u };
