// Complete OCL/Movement SpecialPower derivations; fixed original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const EnumNameValue powerCreationOCLCreateLocTypeNames[] = {
    { "CREATE_ABOVE_LOCATION", 5 },
    { "CREATE_AT_EDGE_ALONG_PATH_LINE", 10 },
    { "CREATE_AT_EDGE_FARTHEST_FROM_TARGET", 6 },
    { "CREATE_AT_EDGE_NEAR_SECONDARY_TARGET", 11 },
    { "CREATE_AT_EDGE_NEAR_SOURCE", 0 },
    { "CREATE_AT_EDGE_NEAR_TARGET", 1 },
    { "CREATE_AT_EDGE_NEAR_TARGET_AND_MOVE_TO_LOCATION", 2 },
    { "CREATE_AT_LOCATION", 3 },
    { "CREATE_AT_OFFSET_FROM_TARGET", 12 },
    { "CREATE_AT_OFFSET_FROM_TARGET_ALONG_SECONDARY_OBJECT_VECTOR_AND_MOVE_TO_TARGET", 14 },
    { "CREATE_AT_OFFSET_FROM_TARGET_AND_MOVE_TO_TARGET", 13 },
    { "CREATE_CLOSEST_TO_SPAWN_POINT", 7 },
    { "USE_MULTIPLE_SECONDARY_OBJECT_LOCATIONS", 9 },
    { "USE_OWNER_OBJECT", 4 },
    { "USE_SECONDARY_OBJECT_LOCATION", 8 },
};
const AIEnum powerCreationOCLCreateLocType = { powerCreationOCLCreateLocTypeNames, 15u, 5, 0u };
const EnumNameValue powerCreationSPMovementTypeNames[] = {
    { "NONE", 0 },
    { "STRAIGHT_AHEAD", 1 },
    { "TARGETED", 2 },
};
const AIEnum powerCreationSPMovementType = { powerCreationSPMovementTypeNames, 3u, 0, 0u };
const ModuleToken powerCreationOCLPairTokens[] = {
    { 2969981770u, 1122298321u, 0u, nullptr, 0u, 0u },
};
const ModuleLayout powerCreationOCLPair = { powerCreationOCLPairTokens, 1u };
const ModuleField powerCreationFields0[] = {
    { "OCL", 452u, ModuleValue::Reference, nullptr, nullptr },
    { "CreateLocation", 456u, ModuleValue::Enum, nullptr, &powerCreationOCLCreateLocType },
    { "CreateLocationOffset", 460u, ModuleValue::Float, nullptr, nullptr },
    { "MaxCreateCount", 464u, ModuleValue::Unsigned, "0", nullptr },
    { "NumberToSpawn", 468u, ModuleValue::Unsigned, "1", nullptr },
    { "DestinationOCL", 472u, ModuleValue::Reference, nullptr, nullptr },
    { "RegisterObjectsWithSpecialAbilityUpdate", 496u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleToken powerCreationOCLSpecialPowerModuleDataTokens[] = {
    { 1u, 3472757011u, 0u, &specialPowerSpecialPowerModuleData, 0u, 0u },
    { 2969981770u, 1122298321u, 452u, nullptr, 0u, 0u },
    { 1862110487u, 3854602880u, 456u, nullptr, 0u, 0u },
    { 3390425208u, 1683514611u, 460u, nullptr, 0u, 0u },
    { 1212546127u, 1982953380u, 464u, nullptr, 0u, 0u },
    { 2968815716u, 1982953380u, 468u, nullptr, 0u, 0u },
    { 4035637479u, 1122298321u, 472u, nullptr, 0u, 0u },
    { 3268689298u, 2011428087u, 496u, nullptr, 0u, 0u },
    { 2754775168u, 2830653170u, 476u, &powerCreationOCLPair, 1u, 4u },
    { 961860796u, 3832456114u, 484u, nullptr, 7u, 0u },
    { 3619147155u, 1253711657u, 488u, nullptr, 3u, 4u },
};
const ModuleLayout powerCreationOCLSpecialPowerModuleData = { powerCreationOCLSpecialPowerModuleDataTokens, 11u };
const ModuleField powerCreationFields1[] = {
    { "MovementType", 452u, ModuleValue::Enum, "NONE", &powerCreationSPMovementType },
    { "Duration", 456u, ModuleValue::Time, "0s", nullptr },
    { "ModelCondition", 460u, ModuleValue::NullableModelCondition, nullptr, nullptr },
    { "ObjectStatus", 464u, ModuleValue::NullableObjectStatus, nullptr, nullptr },
    { "LocomotorSet", 468u, ModuleValue::Enum, "INVALID", &aiUpdateLocomotorSetTypeEnum },
    { "BusyDurationOnExit", 472u, ModuleValue::Time, "0s", nullptr },
    { "CollisionBounceHeight", 476u, ModuleValue::Float, "20.0", nullptr },
    { "CollisionBounceDistance", 480u, ModuleValue::Float, "50.0", nullptr },
    { "CollisionBounceSpeedScalar", 484u, ModuleValue::Float, "0.5", nullptr },
};
const ModuleToken powerCreationMovementSpecialPowerModuleDataTokens[] = {
    { 1u, 3472757011u, 0u, &specialPowerSpecialPowerModuleData, 0u, 0u },
    { 2964156663u, 507759400u, 452u, nullptr, 0u, 0u },
    { 2384108347u, 2575722984u, 456u, nullptr, 0u, 0u },
    { 3468811896u, 1742454729u, 460u, nullptr, 2u, 0u },
    { 3963816762u, 2814214430u, 464u, nullptr, 2u, 0u },
    { 1852568706u, 2220692146u, 468u, nullptr, 0u, 0u },
    { 4208331803u, 2575722984u, 472u, nullptr, 0u, 0u },
    { 4067126440u, 1683514611u, 476u, nullptr, 0u, 0u },
    { 2946403320u, 1683514611u, 480u, nullptr, 0u, 0u },
    { 3629087351u, 1683514611u, 484u, nullptr, 0u, 0u },
};
const ModuleLayout powerCreationMovementSpecialPowerModuleData = { powerCreationMovementSpecialPowerModuleDataTokens, 10u };
const ModuleFieldSet powerCreationFieldSets[] = {
    { powerCreationFields0, 7u },
    { powerCreationFields1, 9u },
};
const ModuleChoice powerCreationChoices[] = {
    { 2308556452u, &powerCreationOCLSpecialPowerModuleData, 500u, 198u },
    { 1729257771u, &powerCreationMovementSpecialPowerModuleData, 488u, 199u },
};
