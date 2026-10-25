// Generated from original draw-source Marshal/Token bodies; do not edit.
const EnumNameValue drawMainModelLODTypeNames[] = {
    { "HIGH", 2 },
    { "LOW", 0 },
    { "MEDIUM", 1 },
};
const AIEnum drawMainModelLODType = { drawMainModelLODTypeNames, 3u, 2, 0u };
const EnumNameValue drawMainWaypointPortalTypeNames[] = {
    { "BEACON", 5 },
    { "CLIMB_PORTAL", 3 },
    { "FAKE_PATHFIND_PORTAL", 7 },
    { "MINESHAFT_PORTAL", 8 },
    { "NORMAL", 0 },
    { "PORTAL", 1 },
    { "PRE_CLIMB_PORTAL", 4 },
    { "SPLINE_CATMULL_ROM", 6 },
    { "WALK_PORTAL", 2 },
};
const AIEnum drawMainWaypointPortalType = { drawMainWaypointPortalTypeNames, 9u, 5, 0u };
const EnumNameValue drawMainWeaponSlotTypeNames[] = {
    { "NO_WEAPON", 5 },
    { "PRIMARY_WEAPON", 0 },
    { "QUATERNARY_WEAPON", 3 },
    { "QUINARY_WEAPON", 4 },
    { "SECONDARY_WEAPON", 1 },
    { "TERTIARY_WEAPON", 2 },
};
const AIEnum drawMainWeaponSlotType = { drawMainWeaponSlotTypeNames, 6u, 5, 6u };
const ModuleField drawMainAttachModelStructFields[] = {
    { "Object", 0u, ModuleValue::String, nullptr, nullptr },
    { "Probability", 8u, ModuleValue::Signed, "-1", nullptr },
};
const ModuleFieldSet drawMainAttachModelStructFieldSet = { drawMainAttachModelStructFields, 2u };
const ModuleToken drawMainAttachModelStructTokens[] = {
    { 3533348450u, 2153588212u, 0u, nullptr, 0u, 0u },
    { 2652392022u, 602296504u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout drawMainAttachModelStruct = { drawMainAttachModelStructTokens, 2u };
const ModuleField drawMainScriptedModelDrawAttachModelFields[] = {
    { "FlagMask", 0u, ModuleValue::ModelCondition, nullptr, nullptr },
    { "Bone", 60u, ModuleValue::String, nullptr, nullptr },
};
const ModuleFieldSet drawMainScriptedModelDrawAttachModelFieldSet = { drawMainScriptedModelDrawAttachModelFields, 2u };
const ModuleToken drawMainScriptedModelDrawAttachModelTokens[] = {
    { 510621769u, 1742454729u, 0u, nullptr, 0u, 0u },
    { 883283766u, 438907818u, 60u, nullptr, 0u, 0u },
    { 3586538858u, 2246252477u, 68u, &drawMainAttachModelStruct, 0u, 0u },
};
const ModuleLayout drawMainScriptedModelDrawAttachModel = { drawMainScriptedModelDrawAttachModelTokens, 3u };
const ModuleField drawMainScriptedModelDrawEmbedPortalFields[] = {
    { "PortalType", 0u, ModuleValue::Enum, nullptr, &drawMainWaypointPortalType },
    { "BonePrefix", 4u, ModuleValue::String, nullptr, nullptr },
};
const ModuleFieldSet drawMainScriptedModelDrawEmbedPortalFieldSet = { drawMainScriptedModelDrawEmbedPortalFields, 2u };
const ModuleToken drawMainScriptedModelDrawEmbedPortalTokens[] = {
    { 533209324u, 1952008373u, 0u, nullptr, 0u, 0u },
    { 240514807u, 438907818u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout drawMainScriptedModelDrawEmbedPortal = { drawMainScriptedModelDrawEmbedPortalTokens, 2u };
const ModuleField drawMainArticulationBoneFields[] = {
    { "ArticulationBoneName", 0u, ModuleValue::String, nullptr, nullptr },
    { "ArticulationHelperBoneName", 8u, ModuleValue::String, nullptr, nullptr },
};
const ModuleFieldSet drawMainArticulationBoneFieldSet = { drawMainArticulationBoneFields, 2u };
const ModuleToken drawMainArticulationBoneTokens[] = {
    { 2425699643u, 2153588212u, 0u, nullptr, 0u, 0u },
    { 2958838373u, 2153588212u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout drawMainArticulationBone = { drawMainArticulationBoneTokens, 2u };
const ModuleField drawMainTreadScrollingMappingFields[] = {
    { "LeftTrack", 0u, ModuleValue::String, nullptr, nullptr },
    { "RightTrack", 8u, ModuleValue::String, nullptr, nullptr },
    { "SpeedMultiplier", 16u, ModuleValue::Float, "1.0", nullptr },
    { "TreadPivotSpeedFraction", 20u, ModuleValue::Percent, "60%", nullptr },
    { "TreadDriveSpeedFraction", 24u, ModuleValue::Percent, "20%", nullptr },
};
const ModuleFieldSet drawMainTreadScrollingMappingFieldSet = { drawMainTreadScrollingMappingFields, 5u };
const ModuleToken drawMainTreadScrollingMappingTokens[] = {
    { 1182258800u, 438907818u, 0u, nullptr, 0u, 0u },
    { 2120633815u, 438907818u, 8u, nullptr, 0u, 0u },
    { 3508223749u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 2197064543u, 1683514611u, 20u, nullptr, 0u, 0u },
    { 4030341214u, 1683514611u, 24u, nullptr, 0u, 0u },
};
const ModuleLayout drawMainTreadScrollingMapping = { drawMainTreadScrollingMappingTokens, 5u };
const ModuleField drawMainW3DScriptedModelDrawModuleDataFields[] = {
    { "Name", 8u, ModuleValue::String, nullptr, nullptr },
    { "InitialRecoilSpeed", 16u, ModuleValue::FrameSpeed, "2.0", nullptr },
    { "MaxRecoilDistance", 20u, ModuleValue::Float, "3.0", nullptr },
    { "RecoilDamping", 24u, ModuleValue::Float, "0.4", nullptr },
    { "RecoilSettleSpeed", 28u, ModuleValue::FrameSpeed, "0.065", nullptr },
    { "OkToChangeModelColor", 196u, ModuleValue::Boolean, "false", nullptr },
    { "AnimationsRequirePower", 197u, ModuleValue::Boolean, "true", nullptr },
    { "UseYAxisForTurretRotation", 198u, ModuleValue::Boolean, "false", nullptr },
    { "MinLODRequired", 32u, ModuleValue::Enum, nullptr, &drawMainModelLODType },
    { "ProjectileBoneFeedbackEnabledSlots", 36u, ModuleValue::Bits, nullptr, &drawMainWeaponSlotType },
    { "TrackMarks", 40u, ModuleValue::Reference, nullptr, nullptr },
    { "TrackMarksOnlyWhenCorneringQuickly", 199u, ModuleValue::Boolean, "false", nullptr },
    { "ExtraPublicBone", 44u, ModuleValue::StringList, nullptr, nullptr },
    { "AttachToBoneInAnotherModule", 52u, ModuleValue::String, nullptr, nullptr },
    { "DependencySharedModelFlags", 60u, ModuleValue::ModelCondition, nullptr, nullptr },
    { "UseProducerTexture", 200u, ModuleValue::Boolean, "false", nullptr },
    { "NoRotate", 201u, ModuleValue::Boolean, "false", nullptr },
    { "UseFiringArcRotation", 202u, ModuleValue::Boolean, "false", nullptr },
    { "Selectable", 203u, ModuleValue::Boolean, "true", nullptr },
    { "RandomTextureFixedRandomIndex", 204u, ModuleValue::Boolean, "false", nullptr },
    { "TrackMarksLeftBone", 120u, ModuleValue::String, nullptr, nullptr },
    { "TrackMarksRightBone", 128u, ModuleValue::String, nullptr, nullptr },
    { "ParticleBonesCheckDrawable", 205u, ModuleValue::Boolean, "false", nullptr },
    { "ShadowForceDisable", 206u, ModuleValue::Boolean, "false", nullptr },
    { "HighDetailLODThreshold", 136u, ModuleValue::Float, "0.0", nullptr },
    { "LowDetailLODThreshold", 140u, ModuleValue::Float, "0.0", nullptr },
    { "SwitchModelLODMode", 207u, ModuleValue::Boolean, "false", nullptr },
    { "StaticModelLODMode", 208u, ModuleValue::Boolean, "false", nullptr },
    { "ShowShadowWhileContained", 209u, ModuleValue::Boolean, "false", nullptr },
    { "WadingParticleSys", 144u, ModuleValue::Reference, nullptr, nullptr },
    { "AlphaCameraFadeOuterRadius", 148u, ModuleValue::Float, "0.0", nullptr },
    { "AlphaCameraFadeInnerRadius", 152u, ModuleValue::Float, "0.0", nullptr },
    { "AlphaCameraAtInnerRadius", 156u, ModuleValue::Percent, "100", nullptr },
    { "BirthFadeTime", 160u, ModuleValue::Signed, "0", nullptr },
    { "MultiPlayerOnly", 210u, ModuleValue::Boolean, "false", nullptr },
    { "AffectedByStealth", 211u, ModuleValue::Boolean, "true", nullptr },
    { "InvertStealthOpacity", 212u, ModuleValue::Boolean, "false", nullptr },
    { "HighDetailOnly", 213u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleFieldSet drawMainW3DScriptedModelDrawModuleDataFieldSet = { drawMainW3DScriptedModelDrawModuleDataFields, 38u };
const ModuleToken drawMainW3DScriptedModelDrawModuleDataTokens[] = {
    { 1u, 402003752u, 0u, &drawBeamDrawModuleData, 0u, 0u },
    { 1501410244u, 438907818u, 8u, nullptr, 0u, 0u },
    { 3859286001u, 2054604302u, 16u, nullptr, 0u, 0u },
    { 50176494u, 1683514611u, 20u, nullptr, 0u, 0u },
    { 947620222u, 1683514611u, 24u, nullptr, 0u, 0u },
    { 1983335972u, 2054604302u, 28u, nullptr, 0u, 0u },
    { 3864486358u, 2011428087u, 196u, nullptr, 0u, 0u },
    { 2609705733u, 2011428087u, 197u, nullptr, 0u, 0u },
    { 3292472096u, 2011428087u, 198u, nullptr, 0u, 0u },
    { 3780451112u, 2819465040u, 32u, nullptr, 0u, 0u },
    { 3302170755u, 682547989u, 36u, nullptr, 0u, 0u },
    { 673478275u, 1279146296u, 40u, nullptr, 0u, 0u },
    { 1893980271u, 2011428087u, 199u, nullptr, 0u, 0u },
    { 3956304839u, 1835445784u, 44u, nullptr, 6u, 0u },
    { 2894818393u, 438907818u, 52u, nullptr, 0u, 0u },
    { 3620856863u, 1742454729u, 60u, nullptr, 0u, 0u },
    { 1100732515u, 2011428087u, 200u, nullptr, 0u, 0u },
    { 933147286u, 2011428087u, 201u, nullptr, 0u, 0u },
    { 85912111u, 2011428087u, 202u, nullptr, 0u, 0u },
    { 2660231907u, 2011428087u, 203u, nullptr, 0u, 0u },
    { 1150756778u, 2011428087u, 204u, nullptr, 0u, 0u },
    { 270128152u, 438907818u, 120u, nullptr, 0u, 0u },
    { 4132769851u, 438907818u, 128u, nullptr, 0u, 0u },
    { 1246703254u, 2011428087u, 205u, nullptr, 0u, 0u },
    { 849523437u, 2011428087u, 206u, nullptr, 0u, 0u },
    { 888154032u, 1683514611u, 136u, nullptr, 0u, 0u },
    { 11411474u, 1683514611u, 140u, nullptr, 0u, 0u },
    { 1218059286u, 2011428087u, 207u, nullptr, 0u, 0u },
    { 1793628724u, 2011428087u, 208u, nullptr, 0u, 0u },
    { 2971585825u, 2011428087u, 209u, nullptr, 0u, 0u },
    { 33141102u, 1139920438u, 144u, nullptr, 0u, 0u },
    { 2754869796u, 1683514611u, 148u, nullptr, 0u, 0u },
    { 1945285705u, 1683514611u, 152u, nullptr, 0u, 0u },
    { 1731665797u, 1683514611u, 156u, nullptr, 0u, 0u },
    { 3796906818u, 602296504u, 160u, nullptr, 0u, 0u },
    { 3249782202u, 2011428087u, 210u, nullptr, 0u, 0u },
    { 3067976792u, 2011428087u, 211u, nullptr, 0u, 0u },
    { 3616574229u, 2011428087u, 212u, nullptr, 0u, 0u },
    { 4196996111u, 2011428087u, 213u, nullptr, 0u, 0u },
    { 2503798931u, 2503798931u, 164u, &drawModelModelConditionState, 1u, 196u },
    { 2578662084u, 2578662084u, 172u, &drawAnimationState, 1u, 140u },
    { 964289784u, 3034915775u, 180u, &drawMainScriptedModelDrawAttachModel, 1u, 80u },
    { 2694339178u, 2948981731u, 188u, &drawMainScriptedModelDrawEmbedPortal, 1u, 12u },
};
const ModuleLayout drawMainW3DScriptedModelDrawModuleData = { drawMainW3DScriptedModelDrawModuleDataTokens, 43u };
const ModuleField drawMainW3DSpotlightDrawModuleDataFields[] = {
    { "RefreshTime", 216u, ModuleValue::Time, "1s", nullptr },
    { "SweepTime", 220u, ModuleValue::Time, "1s", nullptr },
};
const ModuleFieldSet drawMainW3DSpotlightDrawModuleDataFieldSet = { drawMainW3DSpotlightDrawModuleDataFields, 2u };
const ModuleToken drawMainW3DSpotlightDrawModuleDataTokens[] = {
    { 1u, 1322199627u, 0u, &drawMainW3DScriptedModelDrawModuleData, 0u, 0u },
    { 2282606261u, 2575722984u, 216u, nullptr, 0u, 0u },
    { 409515636u, 2575722984u, 220u, nullptr, 0u, 0u },
};
const ModuleLayout drawMainW3DSpotlightDrawModuleData = { drawMainW3DSpotlightDrawModuleDataTokens, 3u };
const ModuleFieldSet drawMainW3DTankDrawModuleDataFieldSet = { nullptr, 0u };
const ModuleToken drawMainW3DTankDrawModuleDataTokens[] = {
    { 1u, 1322199627u, 0u, &drawMainW3DScriptedModelDrawModuleData, 0u, 0u },
    { 708120020u, 2801747281u, 216u, nullptr, 2u, 0u },
    { 3288563659u, 3288563659u, 220u, &drawMainArticulationBone, 1u, 16u },
};
const ModuleLayout drawMainW3DTankDrawModuleData = { drawMainW3DTankDrawModuleDataTokens, 3u };
const ModuleField drawMainW3DTruckDrawModuleDataFields[] = {
    { "LeftFrontTireBone", 216u, ModuleValue::String, nullptr, nullptr },
    { "RightFrontTireBone", 224u, ModuleValue::String, nullptr, nullptr },
    { "LeftRearTireBone", 232u, ModuleValue::String, nullptr, nullptr },
    { "RightRearTireBone", 240u, ModuleValue::String, nullptr, nullptr },
    { "MidLeftFrontTireBone", 248u, ModuleValue::String, nullptr, nullptr },
    { "MidRightFrontTireBone", 256u, ModuleValue::String, nullptr, nullptr },
    { "MidLeftRearTireBone", 264u, ModuleValue::String, nullptr, nullptr },
    { "MidRightRearTireBone", 272u, ModuleValue::String, nullptr, nullptr },
    { "MidLeftMidTireBone", 280u, ModuleValue::String, nullptr, nullptr },
    { "MidRightMidTireBone", 288u, ModuleValue::String, nullptr, nullptr },
    { "LeftFrontTireBone2", 296u, ModuleValue::String, nullptr, nullptr },
    { "RightFrontTireBone2", 304u, ModuleValue::String, nullptr, nullptr },
    { "LeftRearTireBone2", 312u, ModuleValue::String, nullptr, nullptr },
    { "RightRearTireBone2", 320u, ModuleValue::String, nullptr, nullptr },
    { "MidLeftMidTireBone2", 328u, ModuleValue::String, nullptr, nullptr },
    { "MidRightMidTireBone2", 336u, ModuleValue::String, nullptr, nullptr },
    { "TireRotationMultiplier", 344u, ModuleValue::Float, "1.0", nullptr },
    { "TireRotationMultiplierFront", 348u, ModuleValue::Float, "1.0", nullptr },
    { "PowerslideRotationAddition", 352u, ModuleValue::Float, "0", nullptr },
    { "CabBone", 356u, ModuleValue::String, nullptr, nullptr },
    { "TrailerBone", 364u, ModuleValue::String, nullptr, nullptr },
    { "CabRotationMultiplier", 372u, ModuleValue::Float, "1.0", nullptr },
    { "TrailerRotationMultiplier", 376u, ModuleValue::Float, "1.0", nullptr },
    { "RotationDamping", 380u, ModuleValue::Float, "1.0", nullptr },
};
const ModuleFieldSet drawMainW3DTruckDrawModuleDataFieldSet = { drawMainW3DTruckDrawModuleDataFields, 24u };
const ModuleToken drawMainW3DTruckDrawModuleDataTokens[] = {
    { 1u, 1322199627u, 0u, &drawMainW3DScriptedModelDrawModuleData, 0u, 0u },
    { 4226096469u, 438907818u, 216u, nullptr, 0u, 0u },
    { 3616813880u, 438907818u, 224u, nullptr, 0u, 0u },
    { 3878421130u, 438907818u, 232u, nullptr, 0u, 0u },
    { 936684174u, 438907818u, 240u, nullptr, 0u, 0u },
    { 2490299797u, 438907818u, 248u, nullptr, 0u, 0u },
    { 1432299829u, 438907818u, 256u, nullptr, 0u, 0u },
    { 3248652205u, 438907818u, 264u, nullptr, 0u, 0u },
    { 3453922070u, 438907818u, 272u, nullptr, 0u, 0u },
    { 1760553621u, 438907818u, 280u, nullptr, 0u, 0u },
    { 3681414291u, 438907818u, 288u, nullptr, 0u, 0u },
    { 1758743153u, 438907818u, 296u, nullptr, 0u, 0u },
    { 3824590421u, 438907818u, 304u, nullptr, 0u, 0u },
    { 2784709824u, 438907818u, 312u, nullptr, 0u, 0u },
    { 338196307u, 438907818u, 320u, nullptr, 0u, 0u },
    { 217165409u, 438907818u, 328u, nullptr, 0u, 0u },
    { 158295883u, 438907818u, 336u, nullptr, 0u, 0u },
    { 3600686175u, 1683514611u, 344u, nullptr, 0u, 0u },
    { 2338247415u, 1683514611u, 348u, nullptr, 0u, 0u },
    { 3808089309u, 1683514611u, 352u, nullptr, 0u, 0u },
    { 984760039u, 438907818u, 356u, nullptr, 0u, 0u },
    { 2577083013u, 438907818u, 364u, nullptr, 0u, 0u },
    { 654094207u, 1683514611u, 372u, nullptr, 0u, 0u },
    { 703084920u, 1683514611u, 376u, nullptr, 0u, 0u },
    { 568125036u, 1683514611u, 380u, nullptr, 0u, 0u },
    { 708120020u, 2801747281u, 384u, nullptr, 2u, 0u },
    { 1742554342u, 3520325856u, 388u, &drawMainTreadScrollingMapping, 1u, 28u },
};
const ModuleLayout drawMainW3DTruckDrawModuleData = { drawMainW3DTruckDrawModuleDataTokens, 27u };
const ModuleChoice drawModelChoices[] = {
    {1322199627u,&drawMainW3DScriptedModelDrawModuleData,216u,286u},
    {1121390653u,&drawMainW3DSpotlightDrawModuleData,224u,287u},
    {2323717438u,&drawMainW3DTankDrawModuleData,228u,288u},
    {2615586235u,&drawDynamicsW3DDynamicsDrawModuleData,252u,289u},
    {2756454809u,&drawMainW3DTruckDrawModuleData,396u,290u},
};
