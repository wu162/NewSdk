// Complete SpecialPower parent and Radar contracts; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const EnumNameValue specialPowerWeatherNames[] = {
    { "CLOUDY", 1 },
    { "CLOUDYRAINY", 3 },
    { "INVALID", 6 },
    { "NORMAL", 0 },
    { "RAINY", 2 },
    { "SNOWY", 5 },
    { "SUNNY", 4 },
};
const AIEnum specialPowerWeather = { specialPowerWeatherNames, 7u, 1, 0u };
const EnumNameValue specialPowerDisabledNames[] = {
    { "DEFAULT", 0 },
    { "EMP", 2 },
    { "FREEFALL", 7 },
    { "HELD", 3 },
    { "INFILTRATED", 11 },
    { "PARALYZED", 4 },
    { "SCRIPT_DISABLED", 9 },
    { "SCRIPT_UNDERPOWERED", 10 },
    { "TEMPORARILY_BUSY", 8 },
    { "UNDERPOWERED", 6 },
    { "UNMANNED", 5 },
    { "USER_PARALYZED", 1 },
};
const AIEnum specialPowerDisabled = { specialPowerDisabledNames, 12u, 0, 12u };
const EnumNameValue specialPowerDistanceNames[] = {
    { "CENTER_2D", 0 },
    { "CENTER_3D", 2 },
    { "CIRCLE", 1 },
    { "EXTENTS_3D", 4 },
    { "SPHERE", 3 },
};
const AIEnum specialPowerDistance = { specialPowerDistanceNames, 5u, 0, 0u };
const ModuleField specialPowerSpecialPowerModuleDataFields[] = {
    { "SpecialPowerTemplate", 8u, ModuleValue::Reference, nullptr, nullptr },
    { "UpdateModuleStartsAttack", 436u, ModuleValue::Boolean, "false", nullptr },
    { "StartsPaused", 437u, ModuleValue::Boolean, "false", nullptr },
    { "AntiFX", 12u, ModuleValue::Reference, nullptr, nullptr },
    { "AttributeModifier", 16u, ModuleValue::Reference, nullptr, nullptr },
    { "AttributeModifierRange", 20u, ModuleValue::Float, "0.0", nullptr },
    { "AttributeModifierAffectsSelf", 438u, ModuleValue::Boolean, "false", nullptr },
    { "AttributeModifierFX", 24u, ModuleValue::Reference, nullptr, nullptr },
    { "AttributeModifierWeatherBased", 439u, ModuleValue::Boolean, "false", nullptr },
    { "WeatherDuration", 28u, ModuleValue::Duration, "0", nullptr },
    { "RequirementsFilterMPSkirmish", 32u, ModuleValue::StringList, nullptr, nullptr },
    { "InitiateFX", 40u, ModuleValue::Reference, nullptr, nullptr },
    { "TriggerFX", 44u, ModuleValue::Reference, nullptr, nullptr },
    { "SetModelCondition", 48u, ModuleValue::Enum, nullptr, &aiModelConditionFlagType },
    { "SetModelConditionTime", 52u, ModuleValue::Time, "0s", nullptr },
    { "GiveLevels", 56u, ModuleValue::Signed, "0", nullptr },
    { "DisableDuringAnimDuration", 440u, ModuleValue::Boolean, "false", nullptr },
    { "IdleWhenStartingPower", 441u, ModuleValue::Boolean, "false", nullptr },
    { "AffectGood", 442u, ModuleValue::Boolean, "false", nullptr },
    { "AffectEvil", 443u, ModuleValue::Boolean, "false", nullptr },
    { "AffectAllies", 444u, ModuleValue::Boolean, "true", nullptr },
    { "AvailableAtStart", 445u, ModuleValue::Boolean, "true", nullptr },
    { "ChangeWeather", 60u, ModuleValue::Enum, nullptr, &specialPowerWeather },
    { "AdjustVictim", 446u, ModuleValue::Boolean, "false", nullptr },
    { "OnTriggerRechargeSpecialPower", 64u, ModuleValue::AssetId, nullptr, nullptr },
    { "BurnDecayModifier", 68u, ModuleValue::Unsigned, "0", nullptr },
    { "UseDistanceFromCommandCenter", 447u, ModuleValue::Boolean, "false", nullptr },
    { "MaxDistanceFromCommandCenter", 72u, ModuleValue::Float, "0", nullptr },
    { "ChildModuleHandlesFX", 448u, ModuleValue::Boolean, "false", nullptr },
    { "LeechRechargePercentPlayerTechTrigger", 76u, ModuleValue::Reference, nullptr, nullptr },
    { "LeechRechargePercentFromSpecialPower", 80u, ModuleValue::Reference, nullptr, nullptr },
    { "DisabledTypesToIgnore", 84u, ModuleValue::Bits, "HELD", &specialPowerDisabled },
    { "CanAffectObjectFilter", 88u, ModuleValue::Reference, "GenericSpecialPowerObjectFilter", nullptr },
    { "ObjectFilterDistType", 92u, ModuleValue::Enum, "CENTER_2D", &specialPowerDistance },
};
const ModuleField specialPowerRadarHackSpecialPowerModuleDataFields[] = {
    { "NumFalseReturns", 452u, ModuleValue::Unsigned, "0", nullptr },
    { "Radius", 456u, ModuleValue::Float, "0.0", nullptr },
    { "HackDuration", 460u, ModuleValue::Time, "0s", nullptr },
};
const ModuleField specialPowerRadarJamSpecialPowerModuleDataFields[] = {
    { "JamRadius", 452u, ModuleValue::Float, "0.0", nullptr },
    { "JamDuration", 456u, ModuleValue::Time, "0s", nullptr },
};
const ModuleField specialPowerSoundFields[] = {
    { "Sound", 0u, ModuleValue::NullableReference, nullptr, nullptr },
    { "EvaEvent", 4u, ModuleValue::Reference, "", nullptr },
};
const ModuleToken specialPowerSoundTokens[] = {
    { 805758285u, 1675080808u, 0u, nullptr, 2u, 0u },
    { 357806494u, 3303818161u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout specialPowerSound = { specialPowerSoundTokens, 2u };
const ModuleFieldSet specialPowerFieldSets[] = {
    { specialPowerSpecialPowerModuleDataFields, 34u },
    { specialPowerRadarHackSpecialPowerModuleDataFields, 3u },
    { specialPowerRadarJamSpecialPowerModuleDataFields, 2u },
};
const ModuleToken specialPowerSpecialPowerModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &dieBehaviorModuleData, 0u, 0u },
    { 2178440954u, 2934454408u, 8u, nullptr, 0u, 0u },
    { 2079685184u, 2011428087u, 436u, nullptr, 0u, 0u },
    { 1449881964u, 2011428087u, 437u, nullptr, 0u, 0u },
    { 4240413615u, 219975299u, 12u, nullptr, 0u, 0u },
    { 3319822471u, 165531635u, 16u, nullptr, 0u, 0u },
    { 2174126497u, 1683514611u, 20u, nullptr, 0u, 0u },
    { 2128714776u, 2011428087u, 438u, nullptr, 0u, 0u },
    { 4031576094u, 219975299u, 24u, nullptr, 0u, 0u },
    { 2579398722u, 2011428087u, 439u, nullptr, 0u, 0u },
    { 1226739657u, 1869421287u, 28u, nullptr, 0u, 0u },
    { 1923332876u, 1835445784u, 32u, nullptr, 6u, 0u },
    { 2508471948u, 219975299u, 40u, nullptr, 0u, 0u },
    { 1761895055u, 219975299u, 44u, nullptr, 0u, 0u },
    { 4116740590u, 359808523u, 48u, nullptr, 0u, 0u },
    { 1230450932u, 2575722984u, 52u, nullptr, 0u, 0u },
    { 1470726639u, 602296504u, 56u, nullptr, 0u, 0u },
    { 3782992435u, 2011428087u, 440u, nullptr, 0u, 0u },
    { 3142449905u, 2011428087u, 441u, nullptr, 0u, 0u },
    { 3469363109u, 2011428087u, 442u, nullptr, 0u, 0u },
    { 155413259u, 2011428087u, 443u, nullptr, 0u, 0u },
    { 2829779650u, 2011428087u, 444u, nullptr, 0u, 0u },
    { 3732452932u, 2011428087u, 445u, nullptr, 0u, 0u },
    { 1558150140u, 3305139215u, 60u, nullptr, 0u, 0u },
    { 2156621259u, 2011428087u, 446u, nullptr, 0u, 0u },
    { 3804329280u, 1086062514u, 64u, nullptr, 0u, 0u },
    { 3282380122u, 1982953380u, 68u, nullptr, 0u, 0u },
    { 1510762607u, 2011428087u, 447u, nullptr, 0u, 0u },
    { 269885700u, 1683514611u, 72u, nullptr, 0u, 0u },
    { 1982418990u, 2011428087u, 448u, nullptr, 0u, 0u },
    { 1132537419u, 3686731743u, 76u, nullptr, 0u, 0u },
    { 3629429738u, 2519800697u, 80u, nullptr, 0u, 0u },
    { 1504369634u, 426078409u, 84u, nullptr, 0u, 0u },
    { 3870913200u, 509160310u, 88u, nullptr, 0u, 0u },
    { 3111862848u, 2668471815u, 92u, nullptr, 0u, 0u },
    { 2002479074u, 3832456114u, 96u, nullptr, 8u, 0u },
    { 2614671147u, 3832456114u, 208u, nullptr, 8u, 0u },
    { 1635776510u, 3832456114u, 320u, nullptr, 8u, 0u },
    { 401049261u, 451327671u, 432u, &specialPowerSound, 4u, 0u },
};
const ModuleLayout specialPowerSpecialPowerModuleData = { specialPowerSpecialPowerModuleDataTokens, 39u };
const ModuleToken specialPowerRadarHackSpecialPowerModuleDataTokens[] = {
    { 1u, 3472757011u, 0u, &specialPowerSpecialPowerModuleData, 0u, 0u },
    { 1166670575u, 1982953380u, 452u, nullptr, 0u, 0u },
    { 1328662213u, 1683514611u, 456u, nullptr, 0u, 0u },
    { 2197593123u, 2575722984u, 460u, nullptr, 0u, 0u },
};
const ModuleLayout specialPowerRadarHackSpecialPowerModuleData = { specialPowerRadarHackSpecialPowerModuleDataTokens, 4u };
const ModuleToken specialPowerRadarJamSpecialPowerModuleDataTokens[] = {
    { 1u, 3472757011u, 0u, &specialPowerSpecialPowerModuleData, 0u, 0u },
    { 3616721498u, 1683514611u, 452u, nullptr, 0u, 0u },
    { 392286290u, 2575722984u, 456u, nullptr, 0u, 0u },
};
const ModuleLayout specialPowerRadarJamSpecialPowerModuleData = { specialPowerRadarJamSpecialPowerModuleDataTokens, 3u };
const ModuleChoice specialPowerChoices[] = {
    { 3472757011u, &specialPowerSpecialPowerModuleData, 452u, 162u },
    { 3201673786u, &specialPowerRadarHackSpecialPowerModuleData, 464u, 163u },
    { 3870511801u, &specialPowerRadarJamSpecialPowerModuleData, 460u, 164u },
};
