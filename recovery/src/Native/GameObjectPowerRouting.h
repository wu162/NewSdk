// Complete Power routing contracts; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const EnumNameValue powerRoutingTriggerNames[] = {
    { "SELF_POSITION", 0 },
    { "TARGET_OBJECT", 2 },
    { "TARGET_POSITION", 1 },
};
const AIEnum powerRoutingTrigger = { powerRoutingTriggerNames, 3u, 0, 0u };
const ModuleField powerRoutingInfoFields[] = {
    { "ModuleId", 0u, ModuleValue::AssetId, nullptr, nullptr },
    { "TriggerAtTargetPosition", 8u, ModuleValue::Boolean, "true", nullptr },
    { "TriggerType", 4u, ModuleValue::Enum, "TARGET_POSITION", &powerRoutingTrigger },
};
const ModuleField powerRoutingActivateFields[] = {
    { "Deactivate", 12u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleField powerRoutingDispatchFields[] = {
    { "SpecialPowerTemplate", 0u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleToken powerRoutingInfoTokens[] = {
    { 3789524821u, 3186653541u, 0u, nullptr, 0u, 0u },
    { 3963059545u, 2011428087u, 8u, nullptr, 0u, 0u },
    { 29598626u, 29598626u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout powerRoutingInfo = { powerRoutingInfoTokens, 3u };
const ModuleToken powerRoutingActivateTokens[] = {
    { 1u, 2913291561u, 0u, &powerRoutingInfo, 0u, 0u },
    { 2294779680u, 2011428087u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout powerRoutingActivate = { powerRoutingActivateTokens, 2u };
const ModuleToken powerRoutingDispatchTokens[] = {
    { 2178440954u, 2934454408u, 0u, nullptr, 0u, 0u },
};
const ModuleLayout powerRoutingDispatch = { powerRoutingDispatchTokens, 1u };
const ModuleField powerRoutingSpecialPowerUpdateModuleDataFields[] = {
    { "SpecialPowerTemplate", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "StartsPaused", 20u, ModuleValue::Boolean, "false", nullptr },
    { "DisabledTypesToIgnore", 12u, ModuleValue::Bits, "HELD", &specialPowerDisabled },
};
const ModuleToken powerRoutingSpecialPowerUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 2178440954u, 2934454408u, 8u, nullptr, 0u, 0u },
    { 1449881964u, 2011428087u, 20u, nullptr, 0u, 0u },
    { 1504369634u, 426078409u, 12u, nullptr, 0u, 0u },
    { 401049261u, 451327671u, 16u, &specialPowerSound, 4u, 0u },
};
const ModuleLayout powerRoutingSpecialPowerUpdateModuleData = { powerRoutingSpecialPowerUpdateModuleDataTokens, 5u };
const ModuleToken powerRoutingActivateModuleSpecialPowerTokens[] = {
    { 1u, 3472757011u, 0u, &specialPowerSpecialPowerModuleData, 0u, 0u },
    { 1816722542u, 1492518492u, 452u, &powerRoutingActivate, 1u, 16u },
};
const ModuleLayout powerRoutingActivateModuleSpecialPower = { powerRoutingActivateModuleSpecialPowerTokens, 2u };
const ModuleField powerRoutingSpecialPowerDispatchSpecialPowerModuleDataFields[] = {
    { "RequireAllPowersToFunction", 460u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleToken powerRoutingSpecialPowerDispatchSpecialPowerModuleDataTokens[] = {
    { 1u, 3472757011u, 0u, &specialPowerSpecialPowerModuleData, 0u, 0u },
    { 326734708u, 2011428087u, 460u, nullptr, 0u, 0u },
    { 3188081606u, 4174279779u, 452u, &powerRoutingDispatch, 1u, 4u },
};
const ModuleLayout powerRoutingSpecialPowerDispatchSpecialPowerModuleData = { powerRoutingSpecialPowerDispatchSpecialPowerModuleDataTokens, 3u };
const ModuleFieldSet powerRoutingFieldSets[] = {
    { powerRoutingSpecialPowerUpdateModuleDataFields, 3u },
    { nullptr, 0u },
    { powerRoutingSpecialPowerDispatchSpecialPowerModuleDataFields, 1u },
};
const ModuleChoice powerRoutingChoices[] = {
    { 3115493811u, &powerRoutingSpecialPowerUpdateModuleData, 24u, 188u },
    { 1213608295u, &powerRoutingActivateModuleSpecialPower, 460u, 189u },
    { 3631725172u, &powerRoutingSpecialPowerDispatchSpecialPowerModuleData, 464u, 190u },
};
