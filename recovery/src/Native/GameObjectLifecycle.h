// Generated lifecycle Update contracts; original source blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const EnumNameValue lifecycleDeathNames[] = {
    { "ALL", 33 },
    { "BITTEN", 24 },
    { "BURNED", 3 },
    { "CATALYST", 19 },
    { "CRUSHED", 2 },
    { "DETONATED", 10 },
    { "ELECTROCUTED", 11 },
    { "EXPLODED", 4 },
    { "EXTRA_1", 26 },
    { "EXTRA_2", 27 },
    { "EXTRA_3", 28 },
    { "EXTRA_4", 29 },
    { "EXTRA_5", 30 },
    { "EXTRA_6", 31 },
    { "EXTRA_7", 32 },
    { "FADED", 17 },
    { "FLOODED", 7 },
    { "INTERNAL_DESTRUCTION", 21 },
    { "IRRADIATED", 22 },
    { "KNOCKBACK", 15 },
    { "LASERED", 9 },
    { "MAULED", 23 },
    { "NONE", 0 },
    { "NORMAL", 1 },
    { "POISONED", 5 },
    { "POISONED_BETA", 14 },
    { "PRODUCTION_CANCELLED", 25 },
    { "SHATTERED", 20 },
    { "SLAUGHTERED", 18 },
    { "SPLATTED", 13 },
    { "STABBED", 12 },
    { "SUICIDED", 8 },
    { "SUPERNATURAL", 16 },
    { "TOPPLED", 6 },
};
const AIEnum lifecycleDeath = { lifecycleDeathNames, 34u, 33, 0u };
const ModuleField lifecycleDeletionUpdateModuleDataFields[] = {
    { "MinFrames", 8u, ModuleValue::Unsigned, "0", nullptr },
    { "MaxFrames", 12u, ModuleValue::Unsigned, "1", nullptr },
};
const ModuleField lifecycleDieOnProducerDeathUpdateModuleDataFields[] = {
    { "SleepInterval", 8u, ModuleValue::Time, "0.5s", nullptr },
    { "DieOnProducerDisabled", 12u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleField lifecycleInvalidTargetsDeathUpdateModuleDataFields[] = {
    { "InvalidStatus", 8u, ModuleValue::NullableObjectStatus, nullptr, nullptr },
};
const ModuleField lifecycleHeightDieUpdateModuleDataFields[] = {
    { "TargetHeight", 8u, ModuleValue::Float, nullptr, nullptr },
    { "TargetHeightIncludesStructures", 16u, ModuleValue::Boolean, "false", nullptr },
    { "OnlyWhenMovingDown", 17u, ModuleValue::Boolean, "false", nullptr },
    { "SnapToGroundOnDeath", 18u, ModuleValue::Boolean, "false", nullptr },
    { "InitialDelay", 12u, ModuleValue::Duration, "0", nullptr },
};
const ModuleField lifecycleLoseHealthUpdateModuleDataFields[] = {
    { "HealthTrigger", 8u, ModuleValue::Percent, "10%", nullptr },
    { "DamagePerSecond", 12u, ModuleValue::Float, "100", nullptr },
    { "DeathType", 16u, ModuleValue::Enum, "NONE", &lifecycleDeath },
};
const ModuleField lifecycleLifetimeUpdateModuleDataFields[] = {
    { "MinLifetime", 8u, ModuleValue::Time, "0s", nullptr },
    { "MaxLifetime", 12u, ModuleValue::Time, "0s", nullptr },
    { "WaitForWakeUp", 40u, ModuleValue::Boolean, "false", nullptr },
    { "ScoreKill", 41u, ModuleValue::Boolean, "false", nullptr },
    { "DeathType", 16u, ModuleValue::Enum, nullptr, &lifecycleDeath },
    { "IgnoreProjectileState", 42u, ModuleValue::Boolean, nullptr, nullptr },
    { "CountdownSound", 20u, ModuleValue::Reference, nullptr, nullptr },
    { "CountdownCompleteSound", 24u, ModuleValue::Reference, nullptr, nullptr },
    { "TimeBeforeDeathThatCountdownCompletes", 28u, ModuleValue::Time, "0s", nullptr },
    { "TimeBetweenCountdownSounds", 32u, ModuleValue::Time, "1s", nullptr },
    { "NumberOfCountdownSoundPlays", 36u, ModuleValue::NullableSigned, nullptr, nullptr },
};
const ModuleField lifecycleValidationFields[] = {
    { "ForbiddenStatus", 0u, ModuleValue::ObjectStatus, nullptr, nullptr },
    { "RequiredStatus", 28u, ModuleValue::ObjectStatus, nullptr, nullptr },
};
const ModuleFieldSet lifecycleFieldSets[] = {
    { lifecycleDeletionUpdateModuleDataFields, 2u },
    { lifecycleDieOnProducerDeathUpdateModuleDataFields, 2u },
    { lifecycleInvalidTargetsDeathUpdateModuleDataFields, 1u },
    { lifecycleHeightDieUpdateModuleDataFields, 5u },
    { lifecycleLoseHealthUpdateModuleDataFields, 3u },
    { lifecycleLifetimeUpdateModuleDataFields, 11u },
    { lifecycleValidationFields, 2u },
};
const ModuleToken lifecycleModuleDataTokens[] = {
    { 430981792u, 602296504u, 0u, nullptr, 0u, 0u },
    { 2363267224u, 3186653541u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout lifecycleModuleData = { lifecycleModuleDataTokens, 2u };
const ModuleToken lifecycleBehaviorModuleDataTokens[] = {
    { 1u, 3613234082u, 0u, &lifecycleModuleData, 0u, 0u },
};
const ModuleLayout lifecycleBehaviorModuleData = { lifecycleBehaviorModuleDataTokens, 1u };
const ModuleToken lifecycleContainModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &lifecycleBehaviorModuleData, 0u, 0u },
};
const ModuleLayout lifecycleContainModuleData = { lifecycleContainModuleDataTokens, 1u };
const ModuleToken lifecycleUpdateModuleDataTokens[] = {
    { 1u, 687874333u, 0u, &lifecycleContainModuleData, 0u, 0u },
};
const ModuleLayout lifecycleUpdateModuleData = { lifecycleUpdateModuleDataTokens, 1u };
const ModuleToken lifecycleDeletionUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &lifecycleUpdateModuleData, 0u, 0u },
    { 4224241749u, 1982953380u, 8u, nullptr, 0u, 0u },
    { 2766745806u, 1982953380u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout lifecycleDeletionUpdateModuleData = { lifecycleDeletionUpdateModuleDataTokens, 3u };
const ModuleToken lifecycleDieOnProducerDeathUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &lifecycleUpdateModuleData, 0u, 0u },
    { 245768152u, 2575722984u, 8u, nullptr, 0u, 0u },
    { 4035769945u, 2011428087u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout lifecycleDieOnProducerDeathUpdateModuleData = { lifecycleDieOnProducerDeathUpdateModuleDataTokens, 3u };
const ModuleToken lifecycleInvalidTargetsDeathUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &lifecycleUpdateModuleData, 0u, 0u },
    { 239592987u, 2814214430u, 8u, nullptr, 2u, 0u },
};
const ModuleLayout lifecycleInvalidTargetsDeathUpdateModuleData = { lifecycleInvalidTargetsDeathUpdateModuleDataTokens, 2u };
const ModuleToken lifecycleHeightDieUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &lifecycleUpdateModuleData, 0u, 0u },
    { 2173361783u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 1833223857u, 2011428087u, 16u, nullptr, 0u, 0u },
    { 3416665280u, 2011428087u, 17u, nullptr, 0u, 0u },
    { 2635498556u, 2011428087u, 18u, nullptr, 0u, 0u },
    { 2586086522u, 1869421287u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout lifecycleHeightDieUpdateModuleData = { lifecycleHeightDieUpdateModuleDataTokens, 6u };
const ModuleToken lifecycleObjectStatusValidationDataTypeTokens[] = {
    { 824345085u, 2814214430u, 0u, nullptr, 0u, 0u },
    { 597602817u, 2814214430u, 28u, nullptr, 0u, 0u },
};
const ModuleLayout lifecycleObjectStatusValidationDataType = { lifecycleObjectStatusValidationDataTypeTokens, 2u };
const ModuleToken lifecycleLoseHealthUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &lifecycleUpdateModuleData, 0u, 0u },
    { 4161646524u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 588933898u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 3322473782u, 3322473782u, 16u, nullptr, 0u, 0u },
    { 3933465695u, 1049596812u, 20u, &lifecycleObjectStatusValidationDataType, 4u, 0u },
};
const ModuleLayout lifecycleLoseHealthUpdateModuleData = { lifecycleLoseHealthUpdateModuleDataTokens, 5u };
const ModuleToken lifecycleLifetimeUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &lifecycleUpdateModuleData, 0u, 0u },
    { 3239841135u, 2575722984u, 8u, nullptr, 0u, 0u },
    { 2554841364u, 2575722984u, 12u, nullptr, 0u, 0u },
    { 703137320u, 2011428087u, 40u, nullptr, 0u, 0u },
    { 222778717u, 2011428087u, 41u, nullptr, 0u, 0u },
    { 3322473782u, 3322473782u, 16u, nullptr, 0u, 0u },
    { 14413866u, 2011428087u, 42u, nullptr, 0u, 0u },
    { 3543959693u, 1675080808u, 20u, nullptr, 0u, 0u },
    { 2021792840u, 1675080808u, 24u, nullptr, 0u, 0u },
    { 3424827150u, 2575722984u, 28u, nullptr, 0u, 0u },
    { 3353794796u, 2575722984u, 32u, nullptr, 0u, 0u },
    { 3790356833u, 602296504u, 36u, nullptr, 2u, 0u },
};
const ModuleLayout lifecycleLifetimeUpdateModuleData = { lifecycleLifetimeUpdateModuleDataTokens, 12u };
const ModuleChoice lifecycleChoices[] = {
    { 1111529555u, &lifecycleDeletionUpdateModuleData, 16u, 17u },
    { 474181074u, &lifecycleDieOnProducerDeathUpdateModuleData, 16u, 18u },
    { 1461180892u, &lifecycleInvalidTargetsDeathUpdateModuleData, 12u, 19u },
    { 1964506706u, &lifecycleHeightDieUpdateModuleData, 20u, 20u },
    { 2974418213u, &lifecycleLoseHealthUpdateModuleData, 24u, 21u },
    { 1814576242u, &lifecycleLifetimeUpdateModuleData, 44u, 22u },
};
