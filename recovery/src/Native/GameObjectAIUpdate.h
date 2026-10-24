// Generated complete eight-type AI Update contracts; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const EnumNameValue aiUpdateAutoAcquireEnemiesTypeNames[] = {
    { "ATTACK_BUILDINGS", 4 },
    { "NO", 2 },
    { "NOTWHILEATTACKING", 3 },
    { "STEALTHED", 1 },
    { "YES", 0 },
};
const AIEnum aiUpdateAutoAcquireEnemiesTypeEnum = { aiUpdateAutoAcquireEnemiesTypeNames, 5u, 4, 5u };
const EnumNameValue aiUpdateStateMachineTypeNames[] = {
    { "AIStateMachine", 0 },
    { "CombinedHelicopterAIStateMachine", 1 },
    { "DisguiseWhenIdleAIStateMachine", 12 },
    { "DiveBomberJetAIStateMachine", 15 },
    { "HelicopterAIStateMachine", 2 },
    { "HordeAIStateMachine", 3 },
    { "JetAIStateMachine", 4 },
    { "OffensiveStructureAIStateMachine", 5 },
    { "OreMinerOffensiveStateMachine", 10 },
    { "OreMinerStateMachine", 9 },
    { "OreNodeStateMachine", 8 },
    { "OreRefineryStateMachine", 11 },
    { "StrafingRunAIStateMachine", 14 },
    { "StructureAIStateMachine", 6 },
    { "TransportHelicopterAIStateMachine", 13 },
    { "UnitAIStateMachine", 7 },
};
const AIEnum aiUpdateStateMachineTypeEnum = { aiUpdateStateMachineTypeNames, 16u, 0, 0u };
const EnumNameValue aiUpdateLocomotorSetTypeNames[] = {
    { "ATTACKING", 12 },
    { "BURNINGDEATH", 16 },
    { "CHANGING_FIRINGARC", 15 },
    { "COMBO", 11 },
    { "CONTAINED", 10 },
    { "ENRAGED", 8 },
    { "EXITING_PRODUCTION_STRUCTURE", 17 },
    { "FREEFALL", 2 },
    { "INVALID", -1 },
    { "MOUNTED", 7 },
    { "MOVING_TO_TRANSPORT", 13 },
    { "NORMAL", 0 },
    { "NORMAL_UPGRADED", 1 },
    { "PANIC", 4 },
    { "SCARED", 9 },
    { "SUPERSONIC", 6 },
    { "TAXIING", 5 },
    { "WALL_SCALING", 14 },
    { "WANDER", 3 },
};
const AIEnum aiUpdateLocomotorSetTypeEnum = { aiUpdateLocomotorSetTypeNames, 19u, 12, 0u };
const ModuleField aiUpdateAIUpdateModuleDataFields[] = {
    { "AttackPriority", 8u, ModuleValue::String, nullptr, nullptr },
    { "AutoAcquireEnemiesWhenIdle", 16u, ModuleValue::Bits, nullptr, &aiUpdateAutoAcquireEnemiesTypeEnum },
    { "StopChaseDistance", 20u, ModuleValue::Float, "500.0", nullptr },
    { "StandGround", 72u, ModuleValue::Boolean, "false", nullptr },
    { "MinCowerTime", 24u, ModuleValue::Time, "0s", nullptr },
    { "MaxCowerTime", 28u, ModuleValue::Time, "0s", nullptr },
    { "BurningDeathTime", 32u, ModuleValue::Time, nullptr, nullptr },
    { "ComboLocoAttackDistance", 36u, ModuleValue::Float, "80", nullptr },
    { "SpecialContactPoints", 40u, ModuleValue::StringList, nullptr, nullptr },
    { "FadeOnPortals", 73u, ModuleValue::Boolean, "false", nullptr },
    { "IdleTargetTime", 48u, ModuleValue::Time, "0s", nullptr },
    { "MaxCombineChildren", 52u, ModuleValue::Unsigned, "1", nullptr },
    { "SpawnOffsetRadius", 56u, ModuleValue::Float, "0.0", nullptr },
    { "StateMachine", 60u, ModuleValue::Enum, "AIStateMachine", &aiUpdateStateMachineTypeEnum },
    { "DefaultLocomotorSet", 64u, ModuleValue::Enum, "NORMAL", &aiUpdateLocomotorSetTypeEnum },
};
const ModuleField aiUpdateJetAIUpdateModuleDataFields[] = {
    { "OutOfAmmoDamagePerSecond", 76u, ModuleValue::Percent, "0", nullptr },
    { "OutOfAmmoDamageDelay", 80u, ModuleValue::Time, nullptr, nullptr },
    { "NeedsRunway", 96u, ModuleValue::Boolean, "false", nullptr },
    { "ReturnToBaseWhenVictimDead", 97u, ModuleValue::Boolean, "false", nullptr },
    { "KeepsParkingSpaceWhenAirborne", 98u, ModuleValue::Boolean, "false", nullptr },
    { "ParkingOffset", 84u, ModuleValue::Float, "0.0", nullptr },
    { "CirclesForAttack", 99u, ModuleValue::Boolean, "false", nullptr },
    { "UseForOutOfAmmoCheck", 88u, ModuleValue::Reference, nullptr, nullptr },
    { "OutOfAmmoEvaEvent", 92u, ModuleValue::Reference, "", nullptr },
};
const ModuleField aiUpdateSupplyTruckAIUpdateModuleDataFields[] = {
    { "MaxBoxes", 76u, ModuleValue::Unsigned, nullptr, nullptr },
    { "SupplyCenterActionDelay", 80u, ModuleValue::Time, nullptr, nullptr },
    { "SupplyWarehouseActionDelay", 84u, ModuleValue::Time, nullptr, nullptr },
    { "SupplyWarehouseScanDistance", 88u, ModuleValue::Float, nullptr, nullptr },
    { "DepletedAnnounceDistance", 92u, ModuleValue::Float, "2000", nullptr },
    { "HarvestTrees", 108u, ModuleValue::Boolean, nullptr, nullptr },
    { "HarvestTiberium", 109u, ModuleValue::Boolean, nullptr, nullptr },
    { "HarvestActivationRange", 96u, ModuleValue::Float, nullptr, nullptr },
    { "HarvestPreparationTime", 100u, ModuleValue::Time, nullptr, nullptr },
    { "HarvestActionTime", 104u, ModuleValue::Time, nullptr, nullptr },
    { "HarvestsFromRearEnd", 110u, ModuleValue::Boolean, "false", nullptr },
    { "CanPathThroughUnitsWhileDocking", 111u, ModuleValue::Boolean, "true", nullptr },
    { "CanPathThroughUnitsWhileHarvesting", 112u, ModuleValue::Boolean, "true", nullptr },
};
const ModuleField aiUpdateAnimalAIUpdateModuleDataFields[] = {
    { "FleeRadius", 76u, ModuleValue::Signed, "20", nullptr },
    { "FleeDistance", 80u, ModuleValue::Signed, "100", nullptr },
    { "WanderPercentage", 84u, ModuleValue::Signed, "35", nullptr },
    { "MinDistance", 88u, ModuleValue::Signed, "15", nullptr },
    { "MaxDistance", 92u, ModuleValue::Signed, "50", nullptr },
    { "Radius", 96u, ModuleValue::Signed, "25", nullptr },
    { "FleeUpdateTimer", 100u, ModuleValue::Time, "0.8s", nullptr },
    { "WanderUpdateTimer", 104u, ModuleValue::Time, "2.0s", nullptr },
    { "AfraidOfCastles", 112u, ModuleValue::Boolean, "true", nullptr },
    { "InitialFleeBlindlyRadius", 108u, ModuleValue::Float, "50.0", nullptr },
};
const ModuleField aiUpdateWanderAIUpdateModuleDataFields[] = {
    { "AttackAll", 140u, ModuleValue::Boolean, "false", nullptr },
    { "EntryCondition", 76u, ModuleValue::ModelCondition, nullptr, nullptr },
    { "Selectable", 141u, ModuleValue::Boolean, "false", nullptr },
    { "WanderDistance", 136u, ModuleValue::Unsigned, "0", nullptr },
};
const ModuleField aiUpdateDeployStyleAIUpdateModuleDataFields[] = {
    { "UnpackTime", 76u, ModuleValue::Time, "0s", nullptr },
    { "PackTime", 80u, ModuleValue::Time, "0s", nullptr },
    { "ResetTurretBeforePacking", 88u, ModuleValue::Boolean, "false", nullptr },
    { "TurretsFunctionOnlyWhenDeployed", 89u, ModuleValue::Boolean, "false", nullptr },
    { "TurretsMustCenterBeforePacking", 90u, ModuleValue::Boolean, "false", nullptr },
    { "MustDeployToAttack", 91u, ModuleValue::Boolean, "false", nullptr },
    { "DeployedAttributeModifierName", 84u, ModuleValue::Reference, nullptr, nullptr },
};
const ModuleField aiUpdateUnitAITargetChooserDataFields[] = {
    { "TargetingCompareList", 36u, ModuleValue::Reference, "DefaultTargetingCompareList", nullptr },
};
const ModuleFieldSet aiUpdateFieldSets[] = {
    { aiUpdateAIUpdateModuleDataFields, 15u },
    { aiUpdateJetAIUpdateModuleDataFields, 9u },
    { nullptr, 0u },
    { aiUpdateSupplyTruckAIUpdateModuleDataFields, 13u },
    { nullptr, 0u },
    { aiUpdateAnimalAIUpdateModuleDataFields, 10u },
    { aiUpdateWanderAIUpdateModuleDataFields, 4u },
    { aiUpdateDeployStyleAIUpdateModuleDataFields, 7u },
    { aiUpdateUnitAITargetChooserDataFields, 1u },
};
const ModuleToken aiUpdateUnitAITargetChooserDataTokens[] = {
    { 1u, 2856512412u, 0u, &weaponSetBaseAITargetChooserData, 0u, 0u },
    { 3188107749u, 461747305u, 36u, nullptr, 0u, 0u },
};
const ModuleLayout aiUpdateUnitAITargetChooserData = { aiUpdateUnitAITargetChooserDataTokens, 2u };
const ModuleToken aiUpdateAIUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &lifecycleUpdateModuleData, 0u, 0u },
    { 3572451655u, 438907818u, 8u, nullptr, 0u, 0u },
    { 1824896703u, 2333063642u, 16u, nullptr, 0u, 0u },
    { 2007046145u, 1683514611u, 20u, nullptr, 0u, 0u },
    { 124171271u, 2011428087u, 72u, nullptr, 0u, 0u },
    { 41638084u, 2575722984u, 24u, nullptr, 0u, 0u },
    { 3851960838u, 2575722984u, 28u, nullptr, 0u, 0u },
    { 2615314091u, 2575722984u, 32u, nullptr, 0u, 0u },
    { 692768181u, 1683514611u, 36u, nullptr, 0u, 0u },
    { 2890273319u, 1835445784u, 40u, nullptr, 6u, 0u },
    { 489884458u, 2011428087u, 73u, nullptr, 0u, 0u },
    { 3118435559u, 2575722984u, 48u, nullptr, 0u, 0u },
    { 5471934u, 1982953380u, 52u, nullptr, 0u, 0u },
    { 2583364244u, 1683514611u, 56u, nullptr, 0u, 0u },
    { 1939012404u, 2892903904u, 60u, nullptr, 0u, 0u },
    { 4132216731u, 2220692146u, 64u, nullptr, 0u, 0u },
    { 607907585u, 607907585u, 68u, &aiUpdateUnitAITargetChooserData, 4u, 0u },
};
const ModuleLayout aiUpdateAIUpdateModuleData = { aiUpdateAIUpdateModuleDataTokens, 17u };
const ModuleToken aiUpdateJetAIUpdateModuleDataTokens[] = {
    { 1u, 148619278u, 0u, &aiUpdateAIUpdateModuleData, 0u, 0u },
    { 885370369u, 1683514611u, 76u, nullptr, 0u, 0u },
    { 224880835u, 2575722984u, 80u, nullptr, 0u, 0u },
    { 2659224601u, 2011428087u, 96u, nullptr, 0u, 0u },
    { 1476518662u, 2011428087u, 97u, nullptr, 0u, 0u },
    { 1406666406u, 2011428087u, 98u, nullptr, 0u, 0u },
    { 1658340233u, 1683514611u, 84u, nullptr, 0u, 0u },
    { 937298440u, 2011428087u, 99u, nullptr, 0u, 0u },
    { 1319159053u, 3494305058u, 88u, nullptr, 0u, 0u },
    { 1468898432u, 3303818161u, 92u, nullptr, 0u, 0u },
};
const ModuleLayout aiUpdateJetAIUpdateModuleData = { aiUpdateJetAIUpdateModuleDataTokens, 10u };
const ModuleToken aiUpdateHordeAIUpdateModuleDataTokens[] = {
    { 1u, 148619278u, 0u, &aiUpdateAIUpdateModuleData, 0u, 0u },
};
const ModuleLayout aiUpdateHordeAIUpdateModuleData = { aiUpdateHordeAIUpdateModuleDataTokens, 1u };
const ModuleToken aiUpdateSupplyTruckAIUpdateModuleDataTokens[] = {
    { 1u, 148619278u, 0u, &aiUpdateAIUpdateModuleData, 0u, 0u },
    { 1872115417u, 1982953380u, 76u, nullptr, 0u, 0u },
    { 3532671605u, 2575722984u, 80u, nullptr, 0u, 0u },
    { 1448872934u, 2575722984u, 84u, nullptr, 0u, 0u },
    { 1983695139u, 1683514611u, 88u, nullptr, 0u, 0u },
    { 2182341307u, 1683514611u, 92u, nullptr, 0u, 0u },
    { 3776212517u, 2011428087u, 108u, nullptr, 0u, 0u },
    { 2171155491u, 2011428087u, 109u, nullptr, 0u, 0u },
    { 3442227992u, 1683514611u, 96u, nullptr, 0u, 0u },
    { 3219047194u, 2575722984u, 100u, nullptr, 0u, 0u },
    { 2090756146u, 2575722984u, 104u, nullptr, 0u, 0u },
    { 2375921336u, 2011428087u, 110u, nullptr, 0u, 0u },
    { 2845449802u, 2011428087u, 111u, nullptr, 0u, 0u },
    { 2974168566u, 2011428087u, 112u, nullptr, 0u, 0u },
};
const ModuleLayout aiUpdateSupplyTruckAIUpdateModuleData = { aiUpdateSupplyTruckAIUpdateModuleDataTokens, 14u };
const ModuleToken aiUpdateTransportAIUpdateModuleDataTokens[] = {
    { 1u, 148619278u, 0u, &aiUpdateAIUpdateModuleData, 0u, 0u },
};
const ModuleLayout aiUpdateTransportAIUpdateModuleData = { aiUpdateTransportAIUpdateModuleDataTokens, 1u };
const ModuleToken aiUpdateAnimalAIUpdateModuleDataTokens[] = {
    { 1u, 148619278u, 0u, &aiUpdateAIUpdateModuleData, 0u, 0u },
    { 4066379897u, 602296504u, 76u, nullptr, 0u, 0u },
    { 2640593581u, 602296504u, 80u, nullptr, 0u, 0u },
    { 2228523351u, 602296504u, 84u, nullptr, 0u, 0u },
    { 3219517357u, 602296504u, 88u, nullptr, 0u, 0u },
    { 2903588941u, 602296504u, 92u, nullptr, 0u, 0u },
    { 1328662213u, 602296504u, 96u, nullptr, 0u, 0u },
    { 1988340267u, 2575722984u, 100u, nullptr, 0u, 0u },
    { 3080638840u, 2575722984u, 104u, nullptr, 0u, 0u },
    { 2937698500u, 2011428087u, 112u, nullptr, 0u, 0u },
    { 3342372444u, 1683514611u, 108u, nullptr, 0u, 0u },
};
const ModuleLayout aiUpdateAnimalAIUpdateModuleData = { aiUpdateAnimalAIUpdateModuleDataTokens, 11u };
const ModuleToken aiUpdateWanderAIUpdateModuleDataTokens[] = {
    { 1u, 148619278u, 0u, &aiUpdateAIUpdateModuleData, 0u, 0u },
    { 1706207774u, 2011428087u, 140u, nullptr, 0u, 0u },
    { 1213853382u, 1742454729u, 76u, nullptr, 0u, 0u },
    { 2660231907u, 2011428087u, 141u, nullptr, 0u, 0u },
    { 1822909352u, 1982953380u, 136u, nullptr, 0u, 0u },
};
const ModuleLayout aiUpdateWanderAIUpdateModuleData = { aiUpdateWanderAIUpdateModuleDataTokens, 5u };
const ModuleToken aiUpdateDeployStyleAIUpdateModuleDataTokens[] = {
    { 1u, 148619278u, 0u, &aiUpdateAIUpdateModuleData, 0u, 0u },
    { 2661360556u, 2575722984u, 76u, nullptr, 0u, 0u },
    { 2872549302u, 2575722984u, 80u, nullptr, 0u, 0u },
    { 2277848618u, 2011428087u, 88u, nullptr, 0u, 0u },
    { 2172936675u, 2011428087u, 89u, nullptr, 0u, 0u },
    { 2111210692u, 2011428087u, 90u, nullptr, 0u, 0u },
    { 171952216u, 2011428087u, 91u, nullptr, 0u, 0u },
    { 1194538443u, 165531635u, 84u, nullptr, 0u, 0u },
};
const ModuleLayout aiUpdateDeployStyleAIUpdateModuleData = { aiUpdateDeployStyleAIUpdateModuleDataTokens, 8u };
const ModuleChoice aiUpdateChoices[] = {
    { 148619278u, &aiUpdateAIUpdateModuleData, 76u, 104u },
    { 1149685576u, &aiUpdateJetAIUpdateModuleData, 100u, 105u },
    { 1440184067u, &aiUpdateHordeAIUpdateModuleData, 76u, 106u },
    { 2595752269u, &aiUpdateSupplyTruckAIUpdateModuleData, 116u, 107u },
    { 3197949694u, &aiUpdateTransportAIUpdateModuleData, 76u, 108u },
    { 3444991387u, &aiUpdateAnimalAIUpdateModuleData, 116u, 109u },
    { 3625168682u, &aiUpdateWanderAIUpdateModuleData, 144u, 110u },
    { 3996180436u, &aiUpdateDeployStyleAIUpdateModuleData, 92u, 111u },
};
