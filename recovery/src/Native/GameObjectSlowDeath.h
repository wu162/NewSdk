// Complete SlowDeath/destruction group; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const EnumNameValue slowGeometryTypeNames[] = {
    { "BOX", 1 },
    { "CYLINDER", 2 },
    { "RING", 3 },
    { "SPHERE", 0 },
};
const AIEnum slowGeometryType = { slowGeometryTypeNames, 4u, 1, 0u };
const EnumNameValue slowSlowDeathPhaseTypeNames[] = {
    { "FINAL", 2 },
    { "HIT_GROUND", 3 },
    { "INITIAL", 0 },
    { "MIDPOINT", 1 },
};
const AIEnum slowSlowDeathPhaseType = { slowSlowDeathPhaseTypeNames, 4u, 2, 0u };
const ModuleField slowSlowDeathBaseTypeFields[] = {
    { "Type", 0u, ModuleValue::Enum, nullptr, &slowSlowDeathPhaseType },
};
const ModuleToken slowSlowDeathBaseTypeTokens[] = {
    { 2219907648u, 889883045u, 0u, nullptr, 0u, 0u },
};
const ModuleLayout slowSlowDeathBaseType = { slowSlowDeathBaseTypeTokens, 1u };
const ModuleToken slowSlowDeathFXListTypeTokens[] = {
    { 1u, 790409058u, 0u, &slowSlowDeathBaseType, 0u, 0u },
    { 898024161u, 219975299u, 4u, nullptr, 3u, 0u },
};
const ModuleLayout slowSlowDeathFXListType = { slowSlowDeathFXListTypeTokens, 2u };
const ModuleToken slowSlowDeathOCLTypeTokens[] = {
    { 1u, 790409058u, 0u, &slowSlowDeathBaseType, 0u, 0u },
    { 2969981770u, 1122298321u, 4u, nullptr, 3u, 0u },
};
const ModuleLayout slowSlowDeathOCLType = { slowSlowDeathOCLTypeTokens, 2u };
const ModuleToken slowSlowDeathWeaponTypeTokens[] = {
    { 1u, 790409058u, 0u, &slowSlowDeathBaseType, 0u, 0u },
    { 63188358u, 3494305058u, 4u, nullptr, 3u, 0u },
};
const ModuleLayout slowSlowDeathWeaponType = { slowSlowDeathWeaponTypeTokens, 2u };
const ModuleField slowSlowDeathSoundTypeFields[] = {
    { "List", 4u, ModuleValue::StringList, nullptr, nullptr },
    { "Type", 0u, ModuleValue::Enum, nullptr, &slowSlowDeathPhaseType },
};
const ModuleToken slowSlowDeathSoundTypeTokens[] = {
    { 1u, 790409058u, 0u, &slowSlowDeathBaseType, 0u, 0u },
    { 1541700290u, 3776020371u, 4u, nullptr, 6u, 0u },
};
const ModuleLayout slowSlowDeathSoundType = { slowSlowDeathSoundTypeTokens, 2u };
const ModuleField slowBuildingDestructionItemFields[] = {
    { "BonePrefix", 0u, ModuleValue::String, nullptr, nullptr },
    { "FX", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "CreationObject", 12u, ModuleValue::AssetId, nullptr, nullptr },
    { "FlingVelocity", 16u, ModuleValue::Float, "1.0", nullptr },
};
const ModuleToken slowBuildingDestructionItemTokens[] = {
    { 240514807u, 438907818u, 0u, nullptr, 0u, 0u },
    { 898024161u, 219975299u, 8u, nullptr, 0u, 0u },
    { 559052380u, 344905565u, 12u, nullptr, 0u, 0u },
    { 1085665674u, 1683514611u, 16u, nullptr, 0u, 0u },
};
const ModuleLayout slowBuildingDestructionItem = { slowBuildingDestructionItemTokens, 4u };
const ModuleField slowVector2Fields[] = {
    { "x", 0u, ModuleValue::Float, "0.0", nullptr },
    { "y", 4u, ModuleValue::Float, "0.0", nullptr },
};
const ModuleField slowVector3Fields[] = {
    { "x", 0u, ModuleValue::Float, "0.0", nullptr },
    { "y", 4u, ModuleValue::Float, "0.0", nullptr },
    { "z", 8u, ModuleValue::Float, "0.0", nullptr },
};
const ModuleField slowContactPointFields[] = {
    { "Label", 0u, ModuleValue::String, nullptr, nullptr },
};
const ModuleToken slowContactPointTokens[] = {
    { 2494637160u, 438907818u, 0u, nullptr, 0u, 0u },
    { 3166078089u, 691437687u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout slowContactPoint = { slowContactPointTokens, 2u };
const ModuleField slowGeometryShapeFields[] = {
    { "Type", 0u, ModuleValue::Enum, nullptr, &slowGeometryType },
    { "Height", 4u, ModuleValue::Float, "0", nullptr },
    { "MajorRadius", 8u, ModuleValue::Float, "0", nullptr },
    { "MinorRadius", 12u, ModuleValue::Float, "0", nullptr },
    { "Name", 16u, ModuleValue::String, nullptr, nullptr },
    { "BActive", 28u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleToken slowGeometryShapeTokens[] = {
    { 2219907648u, 2966720868u, 0u, nullptr, 0u, 0u },
    { 2875692432u, 1683514611u, 4u, nullptr, 0u, 0u },
    { 2680331028u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 4047609472u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 1501410244u, 438907818u, 16u, nullptr, 0u, 0u },
    { 1468358135u, 2011428087u, 28u, nullptr, 0u, 0u },
    { 2676558395u, 691437687u, 24u, nullptr, 2u, 0u },
};
const ModuleLayout slowGeometryShape = { slowGeometryShapeTokens, 7u };
const ModuleField slowGeometryInfoFields[] = {
    { "IsSmall", 44u, ModuleValue::Boolean, "false", nullptr },
    { "BoundingCircleRadius", 0u, ModuleValue::Float, "0", nullptr },
    { "BoundingSphereRadius", 4u, ModuleValue::Float, "0", nullptr },
    { "XWidth", 8u, ModuleValue::Float, "0", nullptr },
    { "YDepth", 12u, ModuleValue::Float, "0", nullptr },
};
const ModuleToken slowGeometryInfoTokens[] = {
    { 2428332315u, 2011428087u, 44u, nullptr, 0u, 0u },
    { 3064815465u, 1683514611u, 0u, nullptr, 0u, 0u },
    { 445852733u, 1683514611u, 4u, nullptr, 0u, 0u },
    { 2443840251u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 1591122592u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 2572379585u, 2632220834u, 16u, &slowGeometryShape, 1u, 32u },
    { 4047561011u, 1256398628u, 24u, nullptr, 2u, 0u },
    { 1348645848u, 691437687u, 28u, nullptr, 2u, 0u },
    { 1538851375u, 691437687u, 32u, nullptr, 2u, 0u },
    { 1325130660u, 691437687u, 36u, nullptr, 2u, 0u },
    { 1905388306u, 1804527855u, 40u, &slowContactPoint, 4u, 0u },
};
const ModuleLayout slowGeometryInfo = { slowGeometryInfoTokens, 11u };
const ModuleField slowSlowDeathBehaviorModuleDataFields[] = {
    { "SinkRate", 8u, ModuleValue::FrameSpeed, "0", nullptr },
    { "ProbabilityModifier", 12u, ModuleValue::Signed, "10", nullptr },
    { "ModifierBonusPerOverkillPercent", 16u, ModuleValue::Percent, "0", nullptr },
    { "SinkDelay", 20u, ModuleValue::Time, "0s", nullptr },
    { "SinkDelayVariance", 24u, ModuleValue::Time, "0s", nullptr },
    { "DestructionDelay", 28u, ModuleValue::Time, "0s", nullptr },
    { "DestructionDelayVariance", 32u, ModuleValue::Time, "0s", nullptr },
    { "DecayBeginTime", 36u, ModuleValue::Time, "0s", nullptr },
    { "FlingForce", 40u, ModuleValue::Float, "0", nullptr },
    { "FlingForceVariance", 44u, ModuleValue::Float, "0", nullptr },
    { "FlingPitch", 48u, ModuleValue::Angle, nullptr, nullptr },
    { "FlingPitchVariance", 52u, ModuleValue::Angle, nullptr, nullptr },
    { "DeathFlags", 56u, ModuleValue::NullableModelCondition, nullptr, nullptr },
    { "ShadowWhenDead", 112u, ModuleValue::Boolean, "false", nullptr },
    { "Fade", 113u, ModuleValue::Boolean, "false", nullptr },
    { "FadeTime", 60u, ModuleValue::Time, "0s", nullptr },
    { "FadeDelay", 64u, ModuleValue::Time, "0s", nullptr },
    { "DeathTypes", 68u, ModuleValue::NullableModelCondition, nullptr, nullptr },
    { "DeathObjectStatusBits", 72u, ModuleValue::NullableObjectStatus, nullptr, nullptr },
};
const ModuleToken slowSlowDeathBehaviorModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 3780138787u, 2054604302u, 8u, nullptr, 0u, 0u },
    { 39892582u, 602296504u, 12u, nullptr, 0u, 0u },
    { 2905933352u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 494105821u, 2575722984u, 20u, nullptr, 0u, 0u },
    { 48803680u, 2575722984u, 24u, nullptr, 0u, 0u },
    { 2638940236u, 2575722984u, 28u, nullptr, 0u, 0u },
    { 389283231u, 2575722984u, 32u, nullptr, 0u, 0u },
    { 3358662528u, 2575722984u, 36u, nullptr, 0u, 0u },
    { 678303470u, 1683514611u, 40u, nullptr, 0u, 0u },
    { 2817962290u, 1683514611u, 44u, nullptr, 0u, 0u },
    { 1387900568u, 1310124621u, 48u, nullptr, 0u, 0u },
    { 305065786u, 1310124621u, 52u, nullptr, 0u, 0u },
    { 898378959u, 1742454729u, 56u, nullptr, 2u, 0u },
    { 2318370561u, 2011428087u, 112u, nullptr, 0u, 0u },
    { 457187289u, 2011428087u, 113u, nullptr, 0u, 0u },
    { 1730190031u, 2575722984u, 60u, nullptr, 0u, 0u },
    { 2942903753u, 2575722984u, 64u, nullptr, 0u, 0u },
    { 322245936u, 1742454729u, 68u, nullptr, 2u, 0u },
    { 448061836u, 2814214430u, 72u, nullptr, 2u, 0u },
    { 898024161u, 869326766u, 76u, &slowSlowDeathFXListType, 1u, 12u },
    { 2969981770u, 2538029824u, 84u, &slowSlowDeathOCLType, 1u, 12u },
    { 63188358u, 2424375276u, 92u, &slowSlowDeathWeaponType, 1u, 12u },
    { 805758285u, 3102243877u, 100u, &slowSlowDeathSoundType, 1u, 12u },
    { 869730620u, 8650779u, 108u, &dieDieMuxDataType, 4u, 0u },
};
const ModuleLayout slowSlowDeathBehaviorModuleData = { slowSlowDeathBehaviorModuleDataTokens, 25u };
const ModuleField slowClearanceTestingSlowDeathBehaviorModuleDataFields[] = {
    { "ClearanceMaxHeight", 116u, ModuleValue::Float, "0", nullptr },
    { "ClearanceMaxHeightFraction", 120u, ModuleValue::Float, "0", nullptr },
    { "ClearanceMinHeight", 124u, ModuleValue::Float, "0", nullptr },
    { "ClearanceMinHeightFraction", 128u, ModuleValue::Float, "0", nullptr },
};
const ModuleToken slowClearanceTestingSlowDeathBehaviorModuleDataTokens[] = {
    { 1u, 2557908884u, 0u, &slowSlowDeathBehaviorModuleData, 0u, 0u },
    { 4145906403u, 1683514611u, 116u, nullptr, 0u, 0u },
    { 673212695u, 1683514611u, 120u, nullptr, 0u, 0u },
    { 2480348427u, 1683514611u, 124u, nullptr, 0u, 0u },
    { 3777528010u, 1683514611u, 128u, nullptr, 0u, 0u },
    { 4051938204u, 3335913314u, 132u, &slowGeometryInfo, 4u, 0u },
    { 2627223112u, 691437687u, 136u, nullptr, 2u, 0u },
};
const ModuleLayout slowClearanceTestingSlowDeathBehaviorModuleData = { slowClearanceTestingSlowDeathBehaviorModuleDataTokens, 7u };
const ModuleField slowHelicopterSlowDeathBehaviorModuleDataFields[] = {
    { "SpiralOrbitTurnRate", 116u, ModuleValue::Float, nullptr, nullptr },
    { "SpiralOrbitForwardSpeed", 120u, ModuleValue::Float, nullptr, nullptr },
    { "SpiralOrbitForwardSpeedDamping", 124u, ModuleValue::Float, "1.0", nullptr },
    { "MinSelfSpin", 128u, ModuleValue::Float, nullptr, nullptr },
    { "MaxSelfSpin", 132u, ModuleValue::Float, nullptr, nullptr },
    { "SelfSpinUpdateDelay", 136u, ModuleValue::Time, nullptr, nullptr },
    { "SelfSpinUpdateAmount", 140u, ModuleValue::Float, nullptr, nullptr },
    { "FallHowFast", 144u, ModuleValue::Float, nullptr, nullptr },
    { "MinBladeFlyOffDelay", 148u, ModuleValue::Time, nullptr, nullptr },
    { "MaxBladeFlyOffDelay", 152u, ModuleValue::Time, nullptr, nullptr },
    { "BladeBone", 156u, ModuleValue::String, nullptr, nullptr },
    { "OclEjectPilot", 164u, ModuleValue::Reference, nullptr, nullptr },
    { "FxBlade", 168u, ModuleValue::Reference, nullptr, nullptr },
    { "OclBlade", 172u, ModuleValue::Reference, nullptr, nullptr },
    { "FxHitGround", 176u, ModuleValue::Reference, nullptr, nullptr },
    { "OclHitGround", 180u, ModuleValue::Reference, nullptr, nullptr },
    { "FxFinalBlowUp", 184u, ModuleValue::Reference, nullptr, nullptr },
    { "OclFinalBlowUp", 188u, ModuleValue::Reference, nullptr, nullptr },
    { "DelayFromGroundToFinalDeath", 192u, ModuleValue::Float, nullptr, nullptr },
    { "FinalRubbleObject", 196u, ModuleValue::AssetId, nullptr, nullptr },
    { "MaxBraking", 200u, ModuleValue::Float, "99999.9", nullptr },
    { "DeathSound", 204u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleToken slowHelicopterSlowDeathBehaviorModuleDataTokens[] = {
    { 1u, 2557908884u, 0u, &slowSlowDeathBehaviorModuleData, 0u, 0u },
    { 953111212u, 1683514611u, 116u, nullptr, 0u, 0u },
    { 3499133669u, 1683514611u, 120u, nullptr, 0u, 0u },
    { 1538498928u, 1683514611u, 124u, nullptr, 0u, 0u },
    { 2500326849u, 1683514611u, 128u, nullptr, 0u, 0u },
    { 590998064u, 1683514611u, 132u, nullptr, 0u, 0u },
    { 3989262912u, 2575722984u, 136u, nullptr, 0u, 0u },
    { 2912736156u, 1683514611u, 140u, nullptr, 0u, 0u },
    { 965614268u, 1683514611u, 144u, nullptr, 0u, 0u },
    { 4038213477u, 2575722984u, 148u, nullptr, 0u, 0u },
    { 3678740880u, 2575722984u, 152u, nullptr, 0u, 0u },
    { 2883657044u, 438907818u, 156u, nullptr, 0u, 0u },
    { 255480958u, 1122298321u, 164u, nullptr, 0u, 0u },
    { 673945171u, 219975299u, 168u, nullptr, 0u, 0u },
    { 2851789604u, 1122298321u, 172u, nullptr, 0u, 0u },
    { 1784276134u, 219975299u, 176u, nullptr, 0u, 0u },
    { 1950391965u, 1122298321u, 180u, nullptr, 0u, 0u },
    { 1785488986u, 219975299u, 184u, nullptr, 0u, 0u },
    { 2350206844u, 1122298321u, 188u, nullptr, 0u, 0u },
    { 885595425u, 1683514611u, 192u, nullptr, 0u, 0u },
    { 2463630753u, 344905565u, 196u, nullptr, 0u, 0u },
    { 1045384907u, 1683514611u, 200u, nullptr, 0u, 0u },
    { 1498234219u, 1675080808u, 204u, nullptr, 0u, 0u },
};
const ModuleLayout slowHelicopterSlowDeathBehaviorModuleData = { slowHelicopterSlowDeathBehaviorModuleDataTokens, 23u };
const ModuleField slowBuildingDestructionModuleDataFields[] = {
    { "AssociatedModule", 52u, ModuleValue::AssetId, nullptr, nullptr },
    { "MaxDebrisCloudsPerSecond", 56u, ModuleValue::Float, "2.0", nullptr },
    { "FillerMesh", 60u, ModuleValue::AssetId, "NEWSKIN_FILL", nullptr },
};
const ModuleToken slowBuildingDestructionModuleDataTokens[] = {
    { 1u, 2655988594u, 0u, &dieDieModuleData, 0u, 0u },
    { 3694903810u, 3186653541u, 52u, nullptr, 0u, 0u },
    { 2260090415u, 1683514611u, 56u, nullptr, 0u, 0u },
    { 46474131u, 60984384u, 60u, nullptr, 0u, 0u },
    { 2551936840u, 4292734034u, 64u, &slowBuildingDestructionItem, 1u, 20u },
};
const ModuleLayout slowBuildingDestructionModuleData = { slowBuildingDestructionModuleDataTokens, 5u };
const ModuleToken slowShipSlowDeathBehaviorModuleDataTokens[] = { { 1u, 2557908884u, 0u, &slowSlowDeathBehaviorModuleData, 0u, 0u } };
const ModuleLayout slowShipSlowDeathBehaviorModuleData = { slowShipSlowDeathBehaviorModuleDataTokens, 1u };
const ModuleFieldSet slowFieldSets[] = {
    { slowSlowDeathBehaviorModuleDataFields, 19u },
    { slowClearanceTestingSlowDeathBehaviorModuleDataFields, 4u },
    { slowHelicopterSlowDeathBehaviorModuleDataFields, 22u },
    { nullptr, 0u },
    { slowBuildingDestructionModuleDataFields, 3u },
};
const ModuleChoice slowChoices[] = {
    { 2557908884u, &slowSlowDeathBehaviorModuleData, 116u, 265u },
    { 245525561u, &slowClearanceTestingSlowDeathBehaviorModuleData, 140u, 266u },
    { 7192255u, &slowHelicopterSlowDeathBehaviorModuleData, 208u, 267u },
    { 3972432445u, &slowShipSlowDeathBehaviorModuleData, 116u, 268u },
    { 227975924u, &slowBuildingDestructionModuleData, 72u, 269u },
};
