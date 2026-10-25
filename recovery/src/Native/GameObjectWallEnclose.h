// Complete wall hub/enclosure contracts; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const ModuleField wallEncloseWallHubBehaviorModuleDataFields[] = {
    { "HubCapTemplateId", 8u, ModuleValue::AssetId, nullptr, nullptr },
    { "DefaultSegmentTemplateId", 12u, ModuleValue::AssetId, nullptr, nullptr },
    { "CliffCapTemplateId", 16u, ModuleValue::AssetId, nullptr, nullptr },
    { "ShoreCapTemplateId", 20u, ModuleValue::AssetId, nullptr, nullptr },
    { "BorderCapTemplateId", 24u, ModuleValue::AssetId, nullptr, nullptr },
    { "ElevatedSegmentTemplateId", 28u, ModuleValue::AssetId, nullptr, nullptr },
    { "BuilderRadius", 32u, ModuleValue::Float, "0", nullptr },
    { "MaxBuildoutDistance", 36u, ModuleValue::Float, "0", nullptr },
    { "Options", 40u, ModuleValue::Unsigned, "0", nullptr },
    { "StaggeredBuildFactor", 44u, ModuleValue::Signed, "0", nullptr },
};
const ModuleField wallEncloseEncloseBehaviorModuleDataFields[] = {
    { "EnclosureHitFX", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "ScalableDrawModuleId", 12u, ModuleValue::AssetId, nullptr, nullptr },
    { "ScaledBoneName", 16u, ModuleValue::String, nullptr, nullptr },
    { "EnclosureRadius", 24u, ModuleValue::Float, "300", nullptr },
};
const ModuleFieldSet wallEncloseFieldSets[] = {
    { wallEncloseWallHubBehaviorModuleDataFields, 10u },
    { wallEncloseEncloseBehaviorModuleDataFields, 4u },
};
const ModuleToken wallEncloseWallHubBehaviorModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 2846952653u, 344905565u, 8u, nullptr, 0u, 0u },
    { 1913287747u, 344905565u, 12u, nullptr, 0u, 0u },
    { 1027100685u, 344905565u, 16u, nullptr, 0u, 0u },
    { 1539449806u, 344905565u, 20u, nullptr, 0u, 0u },
    { 3808221822u, 344905565u, 24u, nullptr, 0u, 0u },
    { 4142070493u, 344905565u, 28u, nullptr, 0u, 0u },
    { 2829934940u, 1683514611u, 32u, nullptr, 0u, 0u },
    { 1013547413u, 1683514611u, 36u, nullptr, 0u, 0u },
    { 3202187060u, 1982953380u, 40u, nullptr, 0u, 0u },
    { 1266967593u, 602296504u, 44u, nullptr, 0u, 0u },
    { 3229493739u, 344905565u, 48u, nullptr, 3u, 4u },
};
const ModuleLayout wallEncloseWallHubBehaviorModuleData = { wallEncloseWallHubBehaviorModuleDataTokens, 12u };
const ModuleToken wallEncloseEncloseBehaviorModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 2984239183u, 219975299u, 8u, nullptr, 0u, 0u },
    { 3136375604u, 3186653541u, 12u, nullptr, 0u, 0u },
    { 1460382439u, 438907818u, 16u, nullptr, 0u, 0u },
    { 2363172480u, 1683514611u, 24u, nullptr, 0u, 0u },
    { 2239343654u, 3832456114u, 28u, nullptr, 8u, 0u },
};
const ModuleLayout wallEncloseEncloseBehaviorModuleData = { wallEncloseEncloseBehaviorModuleDataTokens, 6u };
const ModuleChoice wallEncloseChoices[] = {
    { 4093160954u, &wallEncloseWallHubBehaviorModuleData, 56u, 150u },
    { 2864072215u, &wallEncloseEncloseBehaviorModuleData, 140u, 151u },
};
