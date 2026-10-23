// Generated from original source blob 6e40a9ed6c9f961d294d49768d911e6a6f354619; shared DieMux and seven complete concrete dispatches.
const ModuleField dieUpgradeDieModuleDataFields[] = {
    { "UpgradeId", 52u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleField dieKeepObjectDieModuleDataFields[] = {
    { "CollapsingTime", 52u, ModuleValue::Time, "0s", nullptr },
    { "StayOnRadar", 56u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleField dieCreateObjectDieModuleDataFields[] = {
    { "CreationList", 52u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleField dieCrushDieModuleDataFields[] = {
    { "TotalCrushSound", 52u, ModuleValue::Reference, nullptr, nullptr },
    { "BackEndCrushSound", 56u, ModuleValue::Reference, nullptr, nullptr },
    { "FrontEndCrushSound", 60u, ModuleValue::Reference, nullptr, nullptr },
    { "TotalCrushSoundPercent", 64u, ModuleValue::Percent, "100", nullptr },
    { "BackEndCrushSoundPercent", 68u, ModuleValue::Percent, "100", nullptr },
    { "FrontEndCrushSoundPercent", 72u, ModuleValue::Percent, "100", nullptr },
};
const ModuleField dieRefundDieModuleDataFields[] = {
    { "UpgradeRequired", 52u, ModuleValue::Reference, nullptr, nullptr },
    { "BuildingRequired", 56u, ModuleValue::StringList, nullptr, nullptr },
    { "RefundPercent", 64u, ModuleValue::Percent, "0", nullptr },
};
const ModuleField dieDieMuxDataTypeFields[] = {
    { "VeterancyLevels", 0u, ModuleValue::StringList, nullptr, nullptr },
    { "ExemptStatus", 8u, ModuleValue::NullableObjectStatus, nullptr, nullptr },
    { "RequiredStatus", 12u, ModuleValue::NullableObjectStatus, nullptr, nullptr },
    { "DamageAmountRequired", 16u, ModuleValue::Float, "-1", nullptr },
    { "MinKillerAngle", 20u, ModuleValue::Angle, "1d", nullptr },
    { "MaxKillerAngle", 24u, ModuleValue::Angle, "-1d", nullptr },
    { "DeathTypes", 28u, ModuleValue::DeathBits, nullptr, nullptr },
    { "DeathTypesForbidden", 36u, ModuleValue::DeathBits, nullptr, nullptr },
};
const ModuleFieldSet dieFieldSets[] = {
    { nullptr, 0u },
    { nullptr, 0u },
    { dieUpgradeDieModuleDataFields, 1u },
    { dieKeepObjectDieModuleDataFields, 2u },
    { dieCreateObjectDieModuleDataFields, 1u },
    { dieCrushDieModuleDataFields, 6u },
    { dieRefundDieModuleDataFields, 3u },
    { dieDieMuxDataTypeFields, 8u },
};
const ModuleToken dieModuleDataTokens[] = {
    { 430981792u, 602296504u, 0u, nullptr, 0u, 0u },
    { 2363267224u, 3186653541u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout dieModuleData = { dieModuleDataTokens, 2u };
const ModuleToken dieBehaviorModuleDataTokens[] = {
    { 1u, 3613234082u, 0u, &dieModuleData, 0u, 0u },
};
const ModuleLayout dieBehaviorModuleData = { dieBehaviorModuleDataTokens, 1u };
const ModuleToken dieDieMuxDataTypeTokens[] = {
    { 4078390751u, 1835445784u, 0u, nullptr, 6u, 8u },
    { 2894011938u, 2814214430u, 8u, nullptr, 2u, 0u },
    { 597602817u, 2814214430u, 12u, nullptr, 2u, 0u },
    { 39636894u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 957598286u, 1310124621u, 20u, nullptr, 0u, 0u },
    { 3756364866u, 1310124621u, 24u, nullptr, 0u, 0u },
    { 322245936u, 55439777u, 28u, nullptr, 0u, 0u },
    { 575960958u, 55439777u, 36u, nullptr, 0u, 0u },
};
const ModuleLayout dieDieMuxDataType = { dieDieMuxDataTypeTokens, 8u };
const ModuleToken dieDieModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &dieBehaviorModuleData, 0u, 0u },
    { 869730620u, 8650779u, 8u, &dieDieMuxDataType, 0u, 0u },
};
const ModuleLayout dieDieModuleData = { dieDieModuleDataTokens, 2u };
const ModuleToken dieDestroyDieModuleDataTokens[] = {
    { 1u, 2655988594u, 0u, &dieDieModuleData, 0u, 0u },
};
const ModuleLayout dieDestroyDieModuleData = { dieDestroyDieModuleDataTokens, 1u };
const ModuleToken dieUpgradeDieModuleDataTokens[] = {
    { 1u, 2655988594u, 0u, &dieDieModuleData, 0u, 0u },
    { 1870752227u, 1253711657u, 52u, nullptr, 0u, 0u },
};
const ModuleLayout dieUpgradeDieModuleData = { dieUpgradeDieModuleDataTokens, 2u };
const ModuleToken dieKeepObjectDieModuleDataTokens[] = {
    { 1u, 2655988594u, 0u, &dieDieModuleData, 0u, 0u },
    { 3270900003u, 2575722984u, 52u, nullptr, 0u, 0u },
    { 194648177u, 2011428087u, 56u, nullptr, 0u, 0u },
};
const ModuleLayout dieKeepObjectDieModuleData = { dieKeepObjectDieModuleDataTokens, 3u };
const ModuleToken dieCreateObjectDieModuleDataTokens[] = {
    { 1u, 2655988594u, 0u, &dieDieModuleData, 0u, 0u },
    { 3130123258u, 1122298321u, 52u, nullptr, 0u, 0u },
    { 2084565028u, 1253711657u, 56u, nullptr, 3u, 4u },
    { 3977401016u, 1253711657u, 64u, nullptr, 3u, 4u },
};
const ModuleLayout dieCreateObjectDieModuleData = { dieCreateObjectDieModuleDataTokens, 4u };
const ModuleToken dieCrushDieModuleDataTokens[] = {
    { 1u, 2655988594u, 0u, &dieDieModuleData, 0u, 0u },
    { 1602190034u, 1675080808u, 52u, nullptr, 0u, 0u },
    { 1302472151u, 1675080808u, 56u, nullptr, 0u, 0u },
    { 2030787144u, 1675080808u, 60u, nullptr, 0u, 0u },
    { 1455148505u, 1683514611u, 64u, nullptr, 0u, 0u },
    { 3446982236u, 1683514611u, 68u, nullptr, 0u, 0u },
    { 2083758373u, 1683514611u, 72u, nullptr, 0u, 0u },
};
const ModuleLayout dieCrushDieModuleData = { dieCrushDieModuleDataTokens, 7u };
const ModuleToken dieRefundDieModuleDataTokens[] = {
    { 1u, 2655988594u, 0u, &dieDieModuleData, 0u, 0u },
    { 2084565028u, 1253711657u, 52u, nullptr, 0u, 0u },
    { 1218661169u, 1835445784u, 56u, nullptr, 6u, 8u },
    { 2436025328u, 1683514611u, 64u, nullptr, 0u, 0u },
    { 3832456114u, 3832456114u, 68u, nullptr, 7u, 0u },
};
const ModuleLayout dieRefundDieModuleData = { dieRefundDieModuleDataTokens, 5u };
const ModuleChoice dieChoices[] = {
    { 2655988594u, &dieDieModuleData, 52u, 23u },
    { 259966977u, &dieDestroyDieModuleData, 52u, 24u },
    { 1359837639u, &dieUpgradeDieModuleData, 56u, 25u },
    { 3491013058u, &dieKeepObjectDieModuleData, 60u, 26u },
    { 996718440u, &dieCreateObjectDieModuleData, 72u, 27u },
    { 2448240860u, &dieCrushDieModuleData, 76u, 28u },
    { 3787260944u, &dieRefundDieModuleData, 72u, 29u },
};
