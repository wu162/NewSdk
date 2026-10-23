// Generated from original source blob 6e40a9ed6c9f961d294d49768d911e6a6f354619; eleven complete Upgrade derivations.
const EnumNameValue remainingDistanceCheckTypeNames[] = {
    { "CIRCULAR", 0 },
    { "RECTANGULAR", 1 },
};
const AIEnum remainingDistanceCheckType = { remainingDistanceCheckTypeNames, 2u, 0, 0u };
const EnumNameValue remainingReplaceSelfUpgradeOptionsNames[] = {
    { "CHECK_BUILD_ASSISTANT", 4 },
    { "COPY_TARGET_OBJECT", 0 },
    { "COPY_UPGRADES", 2 },
    { "DISABLE_DURING_UNPACK", 3 },
    { "IGNORE_TERRAIN_RESTRICTIONS", 1 },
};
const AIEnum remainingReplaceSelfUpgradeOptions = { remainingReplaceSelfUpgradeOptionsNames, 5u, 4, 5u };
const ModuleField remainingModelConditionUpgradeModuleDataFields[] = {
    { "AddConditionFlags", 36u, ModuleValue::ModelCondition, nullptr, nullptr },
    { "RemoveConditionFlags", 96u, ModuleValue::ModelCondition, nullptr, nullptr },
    { "AddTempConditionFlag", 156u, ModuleValue::Enum, nullptr, &aiModelConditionFlagType },
    { "TempConditionTime", 160u, ModuleValue::Time, "0.0s", nullptr },
};
const ModuleField remainingReplaceSelfUpgradeModuleDataFields[] = {
    { "NewObjectUnpackTime", 36u, ModuleValue::Time, "0s", nullptr },
    { "Options", 40u, ModuleValue::ReplaceOptions, "DISABLE_DURING_UNPACK", &remainingReplaceSelfUpgradeOptions },
};
const ModuleField remainingObjectCreationUpgradeModuleDataFields[] = {
    { "UpgradeObject", 36u, ModuleValue::Reference, nullptr, nullptr },
    { "Delay", 40u, ModuleValue::Unsigned, "0", nullptr },
    { "RemoveUpgrade", 44u, ModuleValue::Reference, nullptr, nullptr },
    { "GrantUpgrade", 48u, ModuleValue::Reference, nullptr, nullptr },
    { "ThingToSpawn", 52u, ModuleValue::AssetId, nullptr, nullptr },
    { "Angle", 56u, ModuleValue::Angle, nullptr, nullptr },
    { "DestroyWhenSold", 68u, ModuleValue::Boolean, "false", nullptr },
    { "FadeInTime", 60u, ModuleValue::Unsigned, "0", nullptr },
    { "UseBuildingProduction", 69u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleField remainingRemoveUpgradeUpgradeModuleDataFields[] = {
    { "UpradesGroupsToRemove", 36u, ModuleValue::StringList, nullptr, nullptr },
    { "SuppressEvaEventForRemoval", 52u, ModuleValue::Boolean, "false", nullptr },
    { "RemoveFromAllPlayerObjects", 53u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleField remainingStatusBitsUpgradeModuleDataFields[] = {
    { "StatusToSet", 36u, ModuleValue::ObjectStatus, nullptr, nullptr },
    { "StatusToClear", 64u, ModuleValue::ObjectStatus, nullptr, nullptr },
};
const ModuleField remainingUnpauseSpecialPowerUpgradeModuleDataFields[] = {
    { "SpecialPowerTemplate", 36u, ModuleValue::Reference, nullptr, nullptr },
    { "ObeyRechageOnTrigger", 40u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleField remainingCostModifierUpgradeModuleDataFields[] = {
    { "Percentages", 36u, ModuleValue::FloatList, nullptr, nullptr },
    { "IsUpgradeDiscountUpgrade", 64u, ModuleValue::Boolean, "false", nullptr },
    { "StartsActive", 65u, ModuleValue::Boolean, "false", nullptr },
    { "Slaughter", 66u, ModuleValue::Boolean, "false", nullptr },
    { "UILabel", 44u, ModuleValue::String, nullptr, nullptr },
};
const ModuleField remainingTechUpgradeModuleDataFields[] = {
    { "Radius", 36u, ModuleValue::Float, nullptr, nullptr },
    { "RadiusY", 40u, ModuleValue::NullableFloat, nullptr, nullptr },
    { "DistanceType", 44u, ModuleValue::Enum, "CIRCULAR", &remainingDistanceCheckType },
    { "FXLineBoneName", 48u, ModuleValue::String, nullptr, nullptr },
};
const ModuleField remainingGeometryUpgradeModuleDataFields[] = {
    { "ShowGeometry", 36u, ModuleValue::StringList, nullptr, nullptr },
    { "HideGeometry", 44u, ModuleValue::StringList, nullptr, nullptr },
};
const ModuleField remainingSubObjectsUpgradeNuggetFields[] = {
    { "ShowSubObjects", 0u, ModuleValue::StringList, nullptr, nullptr },
    { "HideSubObjects", 8u, ModuleValue::StringList, nullptr, nullptr },
    { "FadeTime", 16u, ModuleValue::Time, "0.5s", nullptr },
    { "WaitBeforeFadeTime", 20u, ModuleValue::Time, "0.0s", nullptr },
    { "SkipFadeOnCreate", 40u, ModuleValue::Boolean, "false", nullptr },
    { "HideSubObjectsOnRemove", 41u, ModuleValue::Boolean, "false", nullptr },
    { "UnHideSubObjectsOnRemove", 42u, ModuleValue::Boolean, "false", nullptr },
    { "Upgrade", 24u, ModuleValue::AssetId, nullptr, nullptr },
};
const ModuleField remainingReplaceTextureFields[] = {
    { "Original", 0u, ModuleValue::Reference, nullptr, nullptr },
    { "New", 4u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleField remainingOffsetInfoFields[] = {
    { "ModelCondition", 0u, ModuleValue::ModelCondition, nullptr, nullptr },
};
const ModuleField remainingVector3Fields[] = {
    { "x", 0u, ModuleValue::Float, "0.0", nullptr },
    { "y", 4u, ModuleValue::Float, "0.0", nullptr },
    { "z", 8u, ModuleValue::Float, "0.0", nullptr },
};
const ModuleFieldSet remainingFieldSets[] = {
    { nullptr, 0u },
    { remainingModelConditionUpgradeModuleDataFields, 4u },
    { remainingReplaceSelfUpgradeModuleDataFields, 2u },
    { nullptr, 0u },
    { remainingObjectCreationUpgradeModuleDataFields, 9u },
    { remainingRemoveUpgradeUpgradeModuleDataFields, 3u },
    { remainingStatusBitsUpgradeModuleDataFields, 2u },
    { remainingUnpauseSpecialPowerUpgradeModuleDataFields, 2u },
    { remainingCostModifierUpgradeModuleDataFields, 5u },
    { remainingTechUpgradeModuleDataFields, 4u },
    { remainingGeometryUpgradeModuleDataFields, 2u },
    { remainingSubObjectsUpgradeNuggetFields, 8u },
    { remainingReplaceTextureFields, 2u },
    { remainingOffsetInfoFields, 1u },
    { nullptr, 0u },
    { remainingVector3Fields, 3u },
};
const ModuleToken remainingAvailableSubStructureTokens[] = {
    { 419385780u, 653971399u, 0u, nullptr, 0u, 0u },
    { 846703367u, 344905565u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout remainingAvailableSubStructure = { remainingAvailableSubStructureTokens, 2u };
const ModuleToken remainingOffsetInfoTokens[] = {
    { 3468811896u, 1742454729u, 0u, nullptr, 0u, 0u },
    { 2676558395u, 691437687u, 60u, nullptr, 0u, 0u },
};
const ModuleLayout remainingOffsetInfo = { remainingOffsetInfoTokens, 2u };
const ModuleToken remainingSubStructureBuildUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 3011984067u, 3011984067u, 36u, &remainingAvailableSubStructure, 1u, 8u },
    { 2843312838u, 1683991075u, 44u, &remainingOffsetInfo, 1u, 72u },
};
const ModuleLayout remainingSubStructureBuildUpgradeModuleData = { remainingSubStructureBuildUpgradeModuleDataTokens, 3u };
const ModuleToken remainingModelConditionUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 517901656u, 1742454729u, 36u, nullptr, 0u, 0u },
    { 2142075441u, 1742454729u, 96u, nullptr, 0u, 0u },
    { 2557032419u, 359808523u, 156u, nullptr, 0u, 0u },
    { 3168806646u, 2575722984u, 160u, nullptr, 0u, 0u },
};
const ModuleLayout remainingModelConditionUpgradeModuleData = { remainingModelConditionUpgradeModuleDataTokens, 5u };
const ModuleToken remainingReplaceSelfUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 4207563660u, 2575722984u, 36u, nullptr, 0u, 0u },
    { 3202187060u, 3497034384u, 40u, nullptr, 0u, 0u },
    { 1160307140u, 344905565u, 44u, nullptr, 3u, 4u },
};
const ModuleLayout remainingReplaceSelfUpgradeModuleData = { remainingReplaceSelfUpgradeModuleDataTokens, 4u };
const ModuleToken remainingReplaceTextureTokens[] = {
    { 1287025308u, 1279146296u, 0u, nullptr, 0u, 0u },
    { 2758584799u, 1279146296u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout remainingReplaceTexture = { remainingReplaceTextureTokens, 2u };
const ModuleToken remainingSubObjectsUpgradeNuggetTokens[] = {
    { 3108602606u, 3885501526u, 0u, nullptr, 6u, 8u },
    { 2255746918u, 3885501526u, 8u, nullptr, 6u, 8u },
    { 1730190031u, 2575722984u, 16u, nullptr, 0u, 0u },
    { 3417723096u, 2575722984u, 20u, nullptr, 0u, 0u },
    { 2598333060u, 2011428087u, 40u, nullptr, 0u, 0u },
    { 747814091u, 2011428087u, 41u, nullptr, 0u, 0u },
    { 4258920447u, 2011428087u, 42u, nullptr, 0u, 0u },
    { 3619147155u, 653971399u, 24u, nullptr, 0u, 0u },
    { 4059987831u, 1851605243u, 28u, &remainingReplaceTexture, 1u, 8u },
    { 2510190459u, 1802791670u, 36u, &upgradeAnimAndDuration, 4u, 0u },
};
const ModuleLayout remainingSubObjectsUpgradeNugget = { remainingSubObjectsUpgradeNuggetTokens, 10u };
const ModuleToken remainingSubObjectsUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 3191956349u, 2137506354u, 36u, &remainingSubObjectsUpgradeNugget, 1u, 44u },
};
const ModuleLayout remainingSubObjectsUpgradeModuleData = { remainingSubObjectsUpgradeModuleDataTokens, 2u };
const ModuleToken remainingObjectCreationUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 3393533913u, 1122298321u, 36u, nullptr, 0u, 0u },
    { 1718388562u, 1982953380u, 40u, nullptr, 0u, 0u },
    { 2656640899u, 1253711657u, 44u, nullptr, 0u, 0u },
    { 1289852977u, 1253711657u, 48u, nullptr, 0u, 0u },
    { 4118313386u, 344905565u, 52u, nullptr, 0u, 0u },
    { 177619443u, 1310124621u, 56u, nullptr, 0u, 0u },
    { 913872450u, 2011428087u, 68u, nullptr, 0u, 0u },
    { 4289579894u, 1982953380u, 60u, nullptr, 0u, 0u },
    { 736567083u, 2011428087u, 69u, nullptr, 0u, 0u },
    { 2676558395u, 691437687u, 64u, nullptr, 2u, 0u },
};
const ModuleLayout remainingObjectCreationUpgradeModuleData = { remainingObjectCreationUpgradeModuleDataTokens, 11u };
const ModuleToken remainingRemoveUpgradeUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 3652354297u, 3885501526u, 36u, nullptr, 6u, 8u },
    { 2081997846u, 2011428087u, 52u, nullptr, 0u, 0u },
    { 3470220284u, 2011428087u, 53u, nullptr, 0u, 0u },
    { 767018680u, 653971399u, 44u, nullptr, 3u, 4u },
};
const ModuleLayout remainingRemoveUpgradeUpgradeModuleData = { remainingRemoveUpgradeUpgradeModuleDataTokens, 5u };
const ModuleToken remainingStatusBitsUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 1097471514u, 2814214430u, 36u, nullptr, 0u, 0u },
    { 1072528565u, 2814214430u, 64u, nullptr, 0u, 0u },
};
const ModuleLayout remainingStatusBitsUpgradeModuleData = { remainingStatusBitsUpgradeModuleDataTokens, 3u };
const ModuleToken remainingUnpauseSpecialPowerUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 2178440954u, 2934454408u, 36u, nullptr, 0u, 0u },
    { 653490582u, 2011428087u, 40u, nullptr, 0u, 0u },
};
const ModuleLayout remainingUnpauseSpecialPowerUpgradeModuleData = { remainingUnpauseSpecialPowerUpgradeModuleDataTokens, 3u };
const ModuleToken remainingCostModifierUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 593036035u, 2771992103u, 36u, nullptr, 3u, 4u },
    { 1192245138u, 2011428087u, 64u, nullptr, 0u, 0u },
    { 3455740364u, 2011428087u, 65u, nullptr, 0u, 0u },
    { 2214479534u, 2011428087u, 66u, nullptr, 0u, 0u },
    { 1800871695u, 438907818u, 44u, nullptr, 0u, 0u },
    { 3832456114u, 3832456114u, 52u, nullptr, 7u, 0u },
    { 3177955424u, 653971399u, 56u, nullptr, 3u, 4u },
};
const ModuleLayout remainingCostModifierUpgradeModuleData = { remainingCostModifierUpgradeModuleDataTokens, 8u };
const ModuleToken remainingTechUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 1328662213u, 1683514611u, 36u, nullptr, 0u, 0u },
    { 3519330798u, 1683514611u, 40u, nullptr, 2u, 0u },
    { 236086057u, 30012736u, 44u, nullptr, 0u, 0u },
    { 2718491374u, 438907818u, 48u, nullptr, 0u, 0u },
    { 3832456114u, 3832456114u, 56u, nullptr, 8u, 0u },
};
const ModuleLayout remainingTechUpgradeModuleData = { remainingTechUpgradeModuleDataTokens, 6u };
const ModuleToken remainingGeometryUpgradeModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 4209525281u, 1835445784u, 36u, nullptr, 6u, 8u },
    { 2435312422u, 1835445784u, 44u, nullptr, 6u, 8u },
};
const ModuleLayout remainingGeometryUpgradeModuleData = { remainingGeometryUpgradeModuleDataTokens, 3u };
const ModuleChoice remainingChoices[] = {
    { 829862040u, &remainingSubStructureBuildUpgradeModuleData, 52u, 41u },
    { 1306352767u, &remainingModelConditionUpgradeModuleData, 164u, 42u },
    { 1409001909u, &remainingReplaceSelfUpgradeModuleData, 52u, 43u },
    { 1594276481u, &remainingSubObjectsUpgradeModuleData, 44u, 44u },
    { 1776471404u, &remainingObjectCreationUpgradeModuleData, 72u, 45u },
    { 1973457077u, &remainingRemoveUpgradeUpgradeModuleData, 56u, 46u },
    { 2020381784u, &remainingStatusBitsUpgradeModuleData, 92u, 47u },
    { 1072475067u, &remainingUnpauseSpecialPowerUpgradeModuleData, 44u, 48u },
    { 3451973623u, &remainingCostModifierUpgradeModuleData, 68u, 49u },
    { 4286303667u, &remainingTechUpgradeModuleData, 168u, 50u },
    { 4286905310u, &remainingGeometryUpgradeModuleData, 52u, 51u },
};
