// Complete detector/invisibility contracts; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
// Invisibility voice hashes are nullable exact FastHashFn values; the family marshals them explicitly.
const char* stealthVisionVoiceNames[] = {
    "NamedVoiceNameToUseAsVoiceMoveToStealthyArea",
    "NamedVoiceNameToUseAsVoiceEnterStateMoveToStealthyArea",
};
const ModuleField stealthVisionStealthDetectorUpdateModuleDataFields[] = {
    { "DetectionRate", 36u, ModuleValue::Time, "0.4s", nullptr },
    { "DetectionRange", 40u, ModuleValue::Float, "0.0", nullptr },
    { "InitiallyDisabled", 152u, ModuleValue::Boolean, "false", nullptr },
    { "PingSound", 44u, ModuleValue::Reference, nullptr, nullptr },
    { "LoudPingSound", 48u, ModuleValue::Reference, nullptr, nullptr },
    { "IRBeaconParticleSys", 52u, ModuleValue::Reference, nullptr, nullptr },
    { "IRParticleSys", 56u, ModuleValue::Reference, nullptr, nullptr },
    { "IRBrightParticleSys", 60u, ModuleValue::Reference, nullptr, nullptr },
    { "IRGridParticleSys", 64u, ModuleValue::Reference, nullptr, nullptr },
    { "IRParticleSysBone", 68u, ModuleValue::String, nullptr, nullptr },
    { "ExtraRequiredKindOf", 76u, ModuleValue::Bits, nullptr, &aiKindOfType },
    { "ExtraForbiddenKindOf", 112u, ModuleValue::Bits, nullptr, &aiKindOfType },
    { "CanDetectWhileGarrisoned", 153u, ModuleValue::Boolean, "false", nullptr },
    { "CanDetectWhileContained", 154u, ModuleValue::Boolean, "false", nullptr },
    { "RequiredUpgrade", 148u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleField stealthVisionInvisibilityUpdateModuleDataFields[] = {
    { "InvisibilityTemplate", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "UpdatePeriod", 12u, ModuleValue::Time, "1s", nullptr },
    { "RequiredNearbyObjectRange", 16u, ModuleValue::Float, "0", nullptr },
};
const ModuleFieldSet stealthVisionFieldSets[] = {
    { stealthVisionStealthDetectorUpdateModuleDataFields, 15u },
    { stealthVisionInvisibilityUpdateModuleDataFields, 3u },
};
const ModuleToken stealthVisionStealthDetectorUpdateModuleDataTokens[] = {
    { 1u, 2925571736u, 0u, &upgradeUpgradeModuleData, 0u, 0u },
    { 1855692622u, 2575722984u, 36u, nullptr, 0u, 0u },
    { 2663082180u, 1683514611u, 40u, nullptr, 0u, 0u },
    { 844729359u, 2011428087u, 152u, nullptr, 0u, 0u },
    { 2559475244u, 1675080808u, 44u, nullptr, 0u, 0u },
    { 1906925028u, 1675080808u, 48u, nullptr, 0u, 0u },
    { 2021699195u, 1139920438u, 52u, nullptr, 0u, 0u },
    { 2157092663u, 1139920438u, 56u, nullptr, 0u, 0u },
    { 2833845303u, 1139920438u, 60u, nullptr, 0u, 0u },
    { 2794003738u, 1139920438u, 64u, nullptr, 0u, 0u },
    { 3890781486u, 438907818u, 68u, nullptr, 0u, 0u },
    { 802851208u, 2600243603u, 76u, nullptr, 0u, 0u },
    { 1102212596u, 2600243603u, 112u, nullptr, 0u, 0u },
    { 1737709357u, 2011428087u, 153u, nullptr, 0u, 0u },
    { 252904624u, 2011428087u, 154u, nullptr, 0u, 0u },
    { 2138150697u, 1253711657u, 148u, nullptr, 0u, 0u },
};
const ModuleLayout stealthVisionStealthDetectorUpdateModuleData = { stealthVisionStealthDetectorUpdateModuleDataTokens, 16u };
const ModuleToken stealthVisionInvisibilityUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 1431290504u, 2437719909u, 8u, nullptr, 0u, 0u },
    { 3829086399u, 2575722984u, 12u, nullptr, 0u, 0u },
    { 677781922u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 2855850189u, 1488897831u, 20u, nullptr, 2u, 0u },
    { 1482702507u, 1488897831u, 24u, nullptr, 2u, 0u },
    { 3791641903u, 3832456114u, 28u, nullptr, 8u, 0u },
};
const ModuleLayout stealthVisionInvisibilityUpdateModuleData = { stealthVisionInvisibilityUpdateModuleDataTokens, 7u };
const ModuleChoice stealthVisionChoices[] = {
    { 1951832746u, &stealthVisionStealthDetectorUpdateModuleData, 156u, 145u },
    { 895696920u, &stealthVisionInvisibilityUpdateModuleData, 140u, 146u },
};
