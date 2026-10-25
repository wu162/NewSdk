// Complete wall/bridge/placement contracts; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const EnumNameValue structureWallGeometryShapeTypeNames[] = {
    { "BOX", 1 },
    { "CYLINDER", 2 },
    { "RING", 3 },
    { "SPHERE", 0 },
};
const AIEnum structureWallGeometryShapeTypeEnum = { structureWallGeometryShapeTypeNames, 4u, 1, 0u };
const EnumNameValue structureWallContactPointGenerationNames[] = {
    { "INFANTRY", 3 },
    { "NONE", 0 },
    { "SQUAD_MEMBER", 4 },
    { "STRUCTURE", 2 },
    { "VEHICLE", 1 },
};
const AIEnum structureWallContactPointGenerationEnum = { structureWallContactPointGenerationNames, 5u, 3, 0u };
const ModuleField structureWallWallBuilderBehaviorModuleDataFields[] = {
    { "MaxNumSegments", 8u, ModuleValue::Signed, "5", nullptr },
    { "SecondaryModelConditions", 12u, ModuleValue::ModelCondition, "WALL_SEGMENT", nullptr },
    { "SecondaryObjectStatus", 72u, ModuleValue::ObjectStatus, "WALL_SEGMENT", nullptr },
    { "StartInSecondaryModel", 212u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleField structureWallBridgeBuilderModuleDataFields[] = {
    { "EndCap", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "EndCap2", 12u, ModuleValue::NullableReference, nullptr, nullptr },
    { "EndCapLen", 16u, ModuleValue::Float, nullptr, nullptr },
    { "CenterPiece", 20u, ModuleValue::Reference, nullptr, nullptr },
    { "CenterPieceLen", 24u, ModuleValue::Float, nullptr, nullptr },
    { "CenterPieceB", 28u, ModuleValue::NullableReference, nullptr, nullptr },
    { "CenterPieceBLen", 32u, ModuleValue::NullableFloat, nullptr, nullptr },
    { "Width", 36u, ModuleValue::Float, nullptr, nullptr },
    { "GateHouse", 40u, ModuleValue::Reference, nullptr, nullptr },
    { "GateHouse2", 44u, ModuleValue::NullableReference, nullptr, nullptr },
    { "DamageNeighborSegementsAtHealthPercent", 48u, ModuleValue::Percent, "66.66666", nullptr },
};
const ModuleField structureWallShapeFields[] = {
    { "Type", 0u, ModuleValue::Enum, "SPHERE", &structureWallGeometryShapeTypeEnum },
    { "ContactPointGeneration", 4u, ModuleValue::Enum, "NONE", &structureWallContactPointGenerationEnum },
    { "Name", 8u, ModuleValue::String, nullptr, nullptr },
    { "Active", 44u, ModuleValue::Boolean, "true", nullptr },
    { "UsedForHealthBox", 45u, ModuleValue::Boolean, "true", nullptr },
    { "MajorRadius", 16u, ModuleValue::Float, "1.0", nullptr },
    { "MinorRadius", 20u, ModuleValue::Float, "1.0", nullptr },
    { "Other", 24u, ModuleValue::String, nullptr, nullptr },
    { "Height", 32u, ModuleValue::Float, "1.0", nullptr },
    { "FrontAngle", 36u, ModuleValue::Angle, nullptr, nullptr },
};
const ModuleFieldSet structureWallFieldSets[] = {
    { structureWallWallBuilderBehaviorModuleDataFields, 4u },
    { structureWallBridgeBuilderModuleDataFields, 11u },
    { nullptr, 0u },
    { structureWallShapeFields, 10u },
};
const ModuleToken structureWallShapeTokens[] = {
    { 2219907648u, 2966720868u, 0u, nullptr, 0u, 0u },
    { 1254570213u, 1179977934u, 4u, nullptr, 0u, 0u },
    { 1501410244u, 438907818u, 8u, nullptr, 0u, 0u },
    { 429813291u, 2011428087u, 44u, nullptr, 0u, 0u },
    { 2749344737u, 2011428087u, 45u, nullptr, 0u, 0u },
    { 2680331028u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 4047609472u, 1683514611u, 20u, nullptr, 0u, 0u },
    { 1440024122u, 438907818u, 24u, nullptr, 0u, 0u },
    { 2875692432u, 1683514611u, 32u, nullptr, 0u, 0u },
    { 2317216107u, 1310124621u, 36u, nullptr, 0u, 0u },
    { 2676558395u, 691437687u, 40u, nullptr, 2u, 0u },
};
const ModuleLayout structureWallShape = { structureWallShapeTokens, 11u };
const ModuleToken structureWallWallBuilderBehaviorModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &dieBehaviorModuleData, 0u, 0u },
    { 1319092233u, 602296504u, 8u, nullptr, 0u, 0u },
    { 1557874011u, 1742454729u, 12u, nullptr, 0u, 0u },
    { 1637317267u, 2814214430u, 72u, nullptr, 0u, 0u },
    { 4058880781u, 2011428087u, 212u, nullptr, 0u, 0u },
    { 1293291820u, 3832456114u, 100u, nullptr, 8u, 0u },
};
const ModuleLayout structureWallWallBuilderBehaviorModuleData = { structureWallWallBuilderBehaviorModuleDataTokens, 6u };
const ModuleToken structureWallBridgeBuilderModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 918014243u, 3215691893u, 8u, nullptr, 0u, 0u },
    { 969870274u, 3215691893u, 12u, nullptr, 2u, 0u },
    { 2924069234u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 1649286247u, 3215691893u, 20u, nullptr, 0u, 0u },
    { 4048118050u, 1683514611u, 24u, nullptr, 0u, 0u },
    { 1678238039u, 3215691893u, 28u, nullptr, 2u, 0u },
    { 2561373667u, 1683514611u, 32u, nullptr, 2u, 0u },
    { 2801412506u, 1683514611u, 36u, nullptr, 0u, 0u },
    { 54018642u, 3215691893u, 40u, nullptr, 0u, 0u },
    { 4099521469u, 3215691893u, 44u, nullptr, 2u, 0u },
    { 34622828u, 1683514611u, 48u, nullptr, 0u, 0u },
};
const ModuleLayout structureWallBridgeBuilderModuleData = { structureWallBridgeBuilderModuleDataTokens, 12u };
const ModuleToken structureWallStructurePlacementBehaviorModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &dieBehaviorModuleData, 0u, 0u },
    { 3405627400u, 3877700949u, 8u, &structureWallShape, 0u, 0u },
};
const ModuleLayout structureWallStructurePlacementBehaviorModuleData = { structureWallStructurePlacementBehaviorModuleDataTokens, 2u };
const ModuleChoice structureWallChoices[] = {
    { 2509263754u, &structureWallWallBuilderBehaviorModuleData, 216u, 137u },
    { 2734068631u, &structureWallBridgeBuilderModuleData, 52u, 138u },
    { 3922835435u, &structureWallStructurePlacementBehaviorModuleData, 56u, 139u },
};
