// Generated from original source blob 6e40a9ed6c9f961d294d49768d911e6a6f354619; complete Production Update and exit family.
const EnumNameValue productionProductionQueueTypeNames[] = {
    { "AIRCRAFT", 4 },
    { "INFANTRY", 2 },
    { "INVALID", -1 },
    { "MAIN_STRUCTURE", 0 },
    { "OTHER_STRUCTURE", 1 },
    { "UPGRADE", 5 },
    { "VEHICLE", 3 },
    { "WATERCRAFT", 6 },
};
const AIEnum productionProductionQueueType = { productionProductionQueueTypeNames, 8u, 4, 0u };
const EnumNameValue productionDisabledTypeNames[] = {
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
const AIEnum productionDisabledType = { productionDisabledTypeNames, 12u, 0, 12u };
const ModuleField productionProductionUpdateModuleDataFields[] = {
    { "MaxQueueEntries", 8u, ModuleValue::Unsigned, "99", nullptr },
    { "NumDoorAnimations", 12u, ModuleValue::Signed, "0", nullptr },
    { "DoorOpeningTime", 16u, ModuleValue::Time, "0s", nullptr },
    { "DoorWaitOpenTime", 20u, ModuleValue::Time, "0s", nullptr },
    { "DoorCloseTime", 24u, ModuleValue::Time, "0s", nullptr },
    { "ConstructionCompleteDuration", 28u, ModuleValue::Unsigned, "0", nullptr },
    { "DisabledTypesToProcess", 32u, ModuleValue::ReplaceOptions, nullptr, &productionDisabledType },
    { "GiveNoXP", 80u, ModuleValue::Boolean, "false", nullptr },
    { "UnitInvulnerableTime", 36u, ModuleValue::Unsigned, "0", nullptr },
    { "SpecialPrepModelconditionTime", 40u, ModuleValue::Unsigned, "0", nullptr },
    { "VeteranUnitsFromVeteranFactory", 81u, ModuleValue::Boolean, "false", nullptr },
    { "BonusForType", 44u, ModuleValue::AssetId, nullptr, nullptr },
    { "SecondaryQueue", 82u, ModuleValue::Boolean, "false", nullptr },
    { "Type", 48u, ModuleValue::Enum, "INVALID", &productionProductionQueueType },
    { "IgnorePreReqs", 83u, ModuleValue::Boolean, "false", nullptr },
    { "ProductionTimeDelayScalar", 52u, ModuleValue::Time, "0s", nullptr },
    { "NewObjectFadeInTime", 56u, ModuleValue::Time, "0s", nullptr },
};
const ModuleField productionQueueProductionExitUpdateModuleDataFields[] = {
    { "PlacementViewAngle", 8u, ModuleValue::Angle, nullptr, nullptr },
    { "ExitDelaySeconds", 12u, ModuleValue::Time, "0s", nullptr },
    { "AllowAirborneCreation", 44u, ModuleValue::Boolean, "false", nullptr },
    { "InitialBurst", 16u, ModuleValue::Unsigned, "0", nullptr },
    { "NoExitPath", 45u, ModuleValue::Boolean, "false", nullptr },
    { "CanRallyToSlaughter", 46u, ModuleValue::Boolean, "false", nullptr },
    { "ClearAlliesFromDestination", 47u, ModuleValue::Boolean, "true", nullptr },
};
const ModuleField productionQuantityModifierFields[] = {
    { "Name", 0u, ModuleValue::AssetId, nullptr, nullptr },
    { "Count", 4u, ModuleValue::Signed, nullptr, nullptr },
};
const ModuleField productionProductionModifierFields[] = {
    { "RequiredUpgrade", 0u, ModuleValue::Reference, nullptr, nullptr },
    { "CostMultiplier", 4u, ModuleValue::Float, "1.0", nullptr },
    { "TimeMultiplier", 8u, ModuleValue::Float, "1.0", nullptr },
};
const ModuleField productionUpgradeTranslationFields[] = {
    { "RequiredUpgrade", 0u, ModuleValue::Reference, nullptr, nullptr },
    { "ProvidedUpgrade", 4u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleFieldSet productionFieldSets[] = {
    { productionProductionUpdateModuleDataFields, 17u },
    { nullptr, 0u },
    { productionQueueProductionExitUpdateModuleDataFields, 7u },
    { productionQuantityModifierFields, 2u },
    { productionProductionModifierFields, 3u },
    { productionUpgradeTranslationFields, 2u },
};
const ModuleToken productionQuantityModifierTokens[] = {
    { 1501410244u, 344905565u, 0u, nullptr, 0u, 0u },
    { 1820487795u, 602296504u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout productionQuantityModifier = { productionQuantityModifierTokens, 2u };
const ModuleToken productionProductionModifierTokens[] = {
    { 2138150697u, 1253711657u, 0u, nullptr, 0u, 0u },
    { 1377034940u, 1683514611u, 4u, nullptr, 0u, 0u },
    { 3346515955u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 2680068890u, 3832456114u, 12u, nullptr, 8u, 0u },
};
const ModuleLayout productionProductionModifier = { productionProductionModifierTokens, 4u };
const ModuleToken productionUpgradeTranslationTokens[] = {
    { 2138150697u, 1253711657u, 0u, nullptr, 0u, 0u },
    { 406322577u, 1253711657u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout productionUpgradeTranslation = { productionUpgradeTranslationTokens, 2u };
const ModuleToken productionGiveUpgradeToProducedTokens[] = {
    { 3175886613u, 3012211584u, 0u, &productionUpgradeTranslation, 1u, 8u },
};
const ModuleLayout productionGiveUpgradeToProduced = { productionGiveUpgradeToProducedTokens, 1u };
const ModuleToken productionProductionUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &lifecycleUpdateModuleData, 0u, 0u },
    { 1183565060u, 1982953380u, 8u, nullptr, 0u, 0u },
    { 1056711212u, 602296504u, 12u, nullptr, 0u, 0u },
    { 3798358817u, 2575722984u, 16u, nullptr, 0u, 0u },
    { 457371173u, 2575722984u, 20u, nullptr, 0u, 0u },
    { 1105908716u, 2575722984u, 24u, nullptr, 0u, 0u },
    { 929215088u, 1982953380u, 28u, nullptr, 0u, 0u },
    { 943614829u, 426078409u, 32u, nullptr, 0u, 0u },
    { 1909089401u, 2011428087u, 80u, nullptr, 0u, 0u },
    { 2304253894u, 1982953380u, 36u, nullptr, 0u, 0u },
    { 1043517568u, 1982953380u, 40u, nullptr, 0u, 0u },
    { 1013486246u, 2011428087u, 81u, nullptr, 0u, 0u },
    { 2903342219u, 344905565u, 44u, nullptr, 0u, 0u },
    { 1103203100u, 2011428087u, 82u, nullptr, 0u, 0u },
    { 2219907648u, 3163048042u, 48u, nullptr, 0u, 0u },
    { 2502956612u, 2011428087u, 83u, nullptr, 0u, 0u },
    { 1943132298u, 2575722984u, 52u, nullptr, 0u, 0u },
    { 1310266719u, 2575722984u, 56u, nullptr, 0u, 0u },
    { 614333315u, 614333315u, 60u, &productionQuantityModifier, 1u, 8u },
    { 4018374298u, 4018374298u, 68u, &productionProductionModifier, 1u, 124u },
    { 2995837745u, 2995837745u, 76u, &productionGiveUpgradeToProduced, 4u, 0u },
};
const ModuleLayout productionProductionUpdateModuleData = { productionProductionUpdateModuleDataTokens, 21u };
const ModuleToken productionDefaultProductionExitUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &lifecycleUpdateModuleData, 0u, 0u },
    { 1548617554u, 691437687u, 8u, nullptr, 2u, 0u },
    { 991332618u, 691437687u, 12u, nullptr, 2u, 0u },
};
const ModuleLayout productionDefaultProductionExitUpdateModuleData = { productionDefaultProductionExitUpdateModuleDataTokens, 3u };
const ModuleToken productionQueueProductionExitUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &lifecycleUpdateModuleData, 0u, 0u },
    { 3754195860u, 1310124621u, 8u, nullptr, 0u, 0u },
    { 3438310628u, 2575722984u, 12u, nullptr, 0u, 0u },
    { 945976028u, 2011428087u, 44u, nullptr, 0u, 0u },
    { 2318887697u, 1982953380u, 16u, nullptr, 0u, 0u },
    { 9779378u, 2011428087u, 45u, nullptr, 0u, 0u },
    { 323310836u, 2011428087u, 46u, nullptr, 0u, 0u },
    { 3968490200u, 2011428087u, 47u, nullptr, 0u, 0u },
    { 1548617554u, 691437687u, 20u, nullptr, 0u, 0u },
    { 991332618u, 691437687u, 32u, nullptr, 0u, 0u },
};
const ModuleLayout productionQueueProductionExitUpdateModuleData = { productionQueueProductionExitUpdateModuleDataTokens, 10u };
const ModuleChoice productionChoices[] = {
    { 1858556985u, &productionProductionUpdateModuleData, 84u, 52u },
    { 1785063229u, &productionDefaultProductionExitUpdateModuleData, 16u, 53u },
    { 2422471532u, &productionQueueProductionExitUpdateModuleData, 48u, 54u },
};
