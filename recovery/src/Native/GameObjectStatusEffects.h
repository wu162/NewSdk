// Complete EMP/suppression contracts; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const ModuleField statusEffectsEMPUpdateModuleDataFields[] = {
    { "Lifetime", 8u, ModuleValue::Time, "0s", nullptr },
    { "StartFadeDelay", 12u, ModuleValue::Time, "0s", nullptr },
    { "DisabledDuration", 16u, ModuleValue::Time, "0s", nullptr },
    { "StartScale", 20u, ModuleValue::Float, "1", nullptr },
    { "TargetScaleMin", 24u, ModuleValue::Float, "1", nullptr },
    { "TargetScaleMax", 28u, ModuleValue::Float, "1", nullptr },
};
const ModuleField statusEffectsSuppressionUpdateModuleDataFields[] = {
    { "UpdateDelay", 8u, ModuleValue::Time, "1s", nullptr },
    { "Suppressability", 12u, ModuleValue::Float, "0", nullptr },
    { "SuppressionDuration", 16u, ModuleValue::Time, "3s", nullptr },
    { "AttributeModifierSuppressed", 20u, ModuleValue::Reference, nullptr, nullptr },
    { "AttributeModifierForceMove", 24u, ModuleValue::Reference, nullptr, nullptr },
    { "IgnoreSuppressionObjectStatus", 28u, ModuleValue::ObjectStatus, nullptr, nullptr },
};
const ModuleField statusEffectsColor3fFields[] = {
    { "r", 0u, ModuleValue::Float, nullptr, nullptr },
    { "g", 4u, ModuleValue::Float, nullptr, nullptr },
    { "b", 8u, ModuleValue::Float, nullptr, nullptr },
};
const ModuleFieldSet statusEffectsFieldSets[] = {
    { statusEffectsEMPUpdateModuleDataFields, 6u },
    { statusEffectsSuppressionUpdateModuleDataFields, 6u },
    { statusEffectsColor3fFields, 3u },
};
const ModuleToken statusEffectsColor3fTokens[] = {
    { 568241586u, 1683514611u, 0u, nullptr, 0u, 0u },
    { 1645004052u, 1683514611u, 4u, nullptr, 0u, 0u },
    { 1370364569u, 1683514611u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout statusEffectsColor3f = { statusEffectsColor3fTokens, 3u };
const ModuleToken statusEffectsEMPUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 1411793767u, 2575722984u, 8u, nullptr, 0u, 0u },
    { 791321340u, 2575722984u, 12u, nullptr, 0u, 0u },
    { 4173892532u, 2575722984u, 16u, nullptr, 0u, 0u },
    { 3919868129u, 1683514611u, 20u, nullptr, 0u, 0u },
    { 536476526u, 1683514611u, 24u, nullptr, 0u, 0u },
    { 1551106046u, 1683514611u, 28u, nullptr, 0u, 0u },
    { 2296272929u, 42215075u, 32u, &statusEffectsColor3f, 4u, 0u },
    { 1141339426u, 42215075u, 36u, &statusEffectsColor3f, 4u, 0u },
};
const ModuleLayout statusEffectsEMPUpdateModuleData = { statusEffectsEMPUpdateModuleDataTokens, 9u };
const ModuleToken statusEffectsSuppressionUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 3668658417u, 2575722984u, 8u, nullptr, 0u, 0u },
    { 3668870767u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 400304358u, 2575722984u, 16u, nullptr, 0u, 0u },
    { 1440689783u, 165531635u, 20u, nullptr, 0u, 0u },
    { 343239839u, 165531635u, 24u, nullptr, 0u, 0u },
    { 3649685640u, 2814214430u, 28u, nullptr, 0u, 0u },
};
const ModuleLayout statusEffectsSuppressionUpdateModuleData = { statusEffectsSuppressionUpdateModuleDataTokens, 7u };
const ModuleChoice statusEffectsChoices[] = {
    { 3543149950u, &statusEffectsEMPUpdateModuleData, 40u, 143u },
    { 3170589623u, &statusEffectsSuppressionUpdateModuleData, 56u, 144u },
};
