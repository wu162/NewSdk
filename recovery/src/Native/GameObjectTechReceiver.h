// Generated complete TechUpgradeReceiver; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const EnumNameValue receiverDistanceNames[] = {
    { "CIRCULAR", 0 },
    { "RECTANGULAR", 1 },
};
const AIEnum receiverDistance = { receiverDistanceNames, 2u, 0, 0u };
const ModuleField receiverFields[] = {
    { "Radius", 36u, ModuleValue::Float, nullptr, nullptr },
    { "RadiusY", 40u, ModuleValue::NullableFloat, nullptr, nullptr },
    { "DistanceType", 44u, ModuleValue::Enum, "CIRCULAR", &receiverDistance },
    { "FXLineBoneName", 48u, ModuleValue::String, nullptr, nullptr },
    { "RadarDrawModule", 56u, ModuleValue::AssetId, nullptr, nullptr },
    { "RadarRotateSpeed", 60u, ModuleValue::Angle, "0.1r", nullptr },
    { "RadarRotateOffset", 64u, ModuleValue::Angle, "0r", nullptr },
};
const ModuleFieldSet receiverFieldSet = { receiverFields, 7u };
const ModuleToken receiverTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 1328662213u, 1683514611u, 36u, nullptr, 0u, 0u },
    { 3519330798u, 1683514611u, 40u, nullptr, 2u, 0u },
    { 236086057u, 873624354u, 44u, nullptr, 0u, 0u },
    { 2718491374u, 438907818u, 48u, nullptr, 0u, 0u },
    { 2110540067u, 3186653541u, 56u, nullptr, 0u, 0u },
    { 2948402585u, 1310124621u, 60u, nullptr, 0u, 0u },
    { 4223502151u, 1310124621u, 64u, nullptr, 0u, 0u },
    { 3832456114u, 3832456114u, 68u, nullptr, 8u, 0u },
};
const ModuleLayout receiverLayout = { receiverTokens, 9u };
const ModuleChoice receiverChoice = { 393460526u, &receiverLayout, 180u, 60u };
