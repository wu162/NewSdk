// Generated original beam state contracts; blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const ModuleField beamLaserStateModuleDataFields[] = {
    { "LaserId", 8u, ModuleValue::Signed, "0", nullptr },
    { "OriginBoneName", 12u, ModuleValue::String, nullptr, nullptr },
};
const ModuleField beamSweepingLaserStateModuleDataFields[] = {
    { "Radius", 44u, ModuleValue::Float, "10.0", nullptr },
    { "SweepFXList", 48u, ModuleValue::Reference, nullptr, nullptr },
    { "VeteranSweepFXList", 52u, ModuleValue::Reference, nullptr, nullptr },
    { "SweepFXTimeout", 56u, ModuleValue::Time, "0s", nullptr },
    { "SweepWeapon", 60u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleField beamStreamStateModuleDataFields[] = {
    { "StreamId", 8u, ModuleValue::Signed, "0", nullptr },
};
const ModuleFieldSet beamFieldSets[] = {
    { beamLaserStateModuleDataFields, 2u },
    { beamSweepingLaserStateModuleDataFields, 5u },
    { beamStreamStateModuleDataFields, 1u },
};
const ModuleToken beamModuleDataTokens[] = {
    { 430981792u, 602296504u, 0u, nullptr, 0u, 0u },
    { 2363267224u, 3186653541u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout beamModuleData = { beamModuleDataTokens, 2u };
const ModuleToken beamBehaviorModuleDataTokens[] = {
    { 1u, 3613234082u, 0u, &beamModuleData, 0u, 0u },
};
const ModuleLayout beamBehaviorModuleData = { beamBehaviorModuleDataTokens, 1u };
const ModuleToken beamContainModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &beamBehaviorModuleData, 0u, 0u },
};
const ModuleLayout beamContainModuleData = { beamContainModuleDataTokens, 1u };
const ModuleToken beamUpdateModuleDataTokens[] = {
    { 1u, 687874333u, 0u, &beamContainModuleData, 0u, 0u },
};
const ModuleLayout beamUpdateModuleData = { beamUpdateModuleDataTokens, 1u };
const ModuleToken beamObjectStatusValidationDataTypeTokens[] = {
    { 824345085u, 2814214430u, 0u, nullptr, 0u, 0u },
    { 597602817u, 2814214430u, 28u, nullptr, 0u, 0u },
};
const ModuleLayout beamObjectStatusValidationDataType = { beamObjectStatusValidationDataTypeTokens, 2u };
const ModuleToken beamLaserStateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &beamUpdateModuleData, 0u, 0u },
    { 1118104655u, 602296504u, 8u, nullptr, 0u, 0u },
    { 3297432462u, 438907818u, 12u, nullptr, 0u, 0u },
    { 1897971278u, 1139920438u, 20u, nullptr, 3u, 0u },
    { 2120060954u, 1139920438u, 28u, nullptr, 3u, 0u },
    { 3314459928u, 691437687u, 36u, nullptr, 2u, 0u },
    { 3933465695u, 1049596812u, 40u, &beamObjectStatusValidationDataType, 4u, 0u },
};
const ModuleLayout beamLaserStateModuleData = { beamLaserStateModuleDataTokens, 7u };
const ModuleToken beamSweepingLaserStateModuleDataTokens[] = {
    { 1u, 3887509261u, 0u, &beamLaserStateModuleData, 0u, 0u },
    { 1328662213u, 1683514611u, 44u, nullptr, 0u, 0u },
    { 732717553u, 219975299u, 48u, nullptr, 0u, 0u },
    { 214280096u, 219975299u, 52u, nullptr, 0u, 0u },
    { 4107750991u, 2575722984u, 56u, nullptr, 0u, 0u },
    { 1093366695u, 3494305058u, 60u, nullptr, 0u, 0u },
};
const ModuleLayout beamSweepingLaserStateModuleData = { beamSweepingLaserStateModuleDataTokens, 6u };
const ModuleToken beamStreamStateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &beamUpdateModuleData, 0u, 0u },
    { 2251403163u, 602296504u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout beamStreamStateModuleData = { beamStreamStateModuleDataTokens, 2u };
const ModuleChoice beamChoices[] = {
    { 3887509261u, &beamLaserStateModuleData, 44u, 77u },
    { 1016285011u, &beamSweepingLaserStateModuleData, 64u, 78u },
    { 4277690261u, &beamStreamStateModuleData, 12u, 79u },
};
