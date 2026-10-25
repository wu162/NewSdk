// Fixed Module blob 2d2f471c8008b7c2e1f78e39752d0726da395cea; exact static literals and offsets.
const Field fieldsAIData[]={
{"StructureSeconds","0.0",4,Kind::Float,""},
{"TeamSeconds","0.0",8,Kind::Float,""},
{"ResourcesWealthy","0",12,Kind::Unsigned,""},
{"ResourcesPoor","0",16,Kind::Unsigned,""},
{"StructuresWealthyMod","0.0",20,Kind::Float,""},
{"TeamWealthyMod","0.0",24,Kind::Float,""},
{"StructuresPoorMod","0.0",28,Kind::Float,""},
{"TeamPoorMod","0.0",32,Kind::Float,""},
{"TeamResourcesToBuild","0.0",36,Kind::Float,""},
{"GuardChaseUnitFrames","0",40,Kind::Unsigned,""},
{"GuardEnemyScanRate","7",44,Kind::Unsigned,""},
{"GuardEnemyReturnScanRate","15",48,Kind::Unsigned,""},
{"WallHeight","0.0",52,Kind::Float,""},
{"SkirmishGroupFudgeValue","0.0",56,Kind::Float,""},
{"MaxRecruitDistance","0.0",60,Kind::Float,""},
{"RepulsedDistance","0.0",64,Kind::Float,""},
{"EnableRepulsors","false",204,Kind::Bool,""},
{"ForceSkirmishAI","false",205,Kind::Bool,""},
{"RotateSkirmishBases","false",206,Kind::Bool,""},
{"AttackUsesLineOfSight","true",207,Kind::Bool,""},
{"MinDistanceForGroup","100.0",68,Kind::Float,""},
{"FormationEnemyDistance","600.0",72,Kind::Float,""},
{"RebuildDelaySeconds","10",76,Kind::Signed,""},
{"SupplyCenterSafeRadius","250.0",80,Kind::Float,""},
{"AIDozerBoredRadiusModifier","2.0",84,Kind::Float,""},
{"AICrushesInfantry","true",208,Kind::Bool,""},
{"MeleeApproachTolerance","20.0",88,Kind::Float,""},
{"MeleeApproachDist","60.0",92,Kind::Float,""},
{"WadeWaterDepth","5.0",96,Kind::Float,""},
{"DeepWaterDepth","50.0",100,Kind::Float,""},
{"FormationColumnWidth","65.0",104,Kind::Float,""},
{"FormationRowDepth","65.0",108,Kind::Float,""},
{"FormationSquadSpacing","30.0",112,Kind::Float,""},
{"NarrowPassageScale","1.0",116,Kind::Float,""},
{"MaxSquadWidth","2",120,Kind::Signed,""},
{"WaitForOthers","false",209,Kind::Bool,""},
{"HordesWaitForHordes","true",210,Kind::Bool,""},
{"AttackMoveUsesFormations","true",211,Kind::Bool,""},
{"AllowForestFires","false",212,Kind::Bool,""},
{"UseFormations","true",213,Kind::Bool,""},
{"AltCameraZoomOverride","1.4",124,Kind::Float,""},
{"AltCameraPitchOverride","0.5",128,Kind::Float,""},
{"MaxRetaliateDistance","200.0",132,Kind::Float,""},
{"RetaliateFriendsRadius","120.0",136,Kind::Float,""},
{"ChaseFromBehindLimit","50.0",140,Kind::Float,""},
{"CastleSiegeStandBackDistance","100.0",144,Kind::Float,""},
{"UseLowLODTrees","false",214,Kind::Bool,""},
{"LowLodTree",nullptr,148,Kind::String,""},
{"LowLodTreeNoGrab",nullptr,156,Kind::String,""},
{"LowLodTreeNoHarvest",nullptr,164,Kind::String,""},
{"LowLodTreeScale","0.55",172,Kind::Float,""},
{"DisableTrees","false",215,Kind::Bool,""},
{"MaximumCraterDepth","8",216,Kind::Byte,""},
{"Optimization_NumTeamsToRecruit","10",176,Kind::Signed,""},
{"Optimization_NumEfficiencyUpdates","2",180,Kind::Signed,""},
{"Optimization_TimeToCacheCanEnterTransport","2s",184,Kind::Time,""},
{"Optimization_TimeToCacheCanReachMobileTarget","1s",188,Kind::Time,""},
{"Optimization_TimeToCacheCanReachImmobileTarget","1s",192,Kind::Time,""},
{"Optimization_ConquerMetricsCacheTimeOut","3s",196,Kind::Time,""},
{"Optimization_TacticsEmergencyKickPeriod","4s",200,Kind::Time,""},
};
const Field fieldsUnitAISettings[]={
{"SupplyBoxesPerTree","90",4,Kind::Signed,""},
{"ValuePerSupplyBox","400",8,Kind::Signed,""},
{"MinDistFromEdgeOfMapForBuild","30.0",12,Kind::Float,""},
{"SupplyBuildBorder","20.0",16,Kind::Float,""},
{"MaxLineBuildObjects","50",20,Kind::Signed,""},
{"MaxTunnelCapacity","3",24,Kind::Signed,""},
{"MinLowEnergyProductionSpeed","0.5",28,Kind::Float,""},
{"MaxLowEnergyProductionSpeed","0.8",32,Kind::Float,""},
{"LowEnergyPenaltyModifier","1.0",36,Kind::Float,""},
{"DamageRadiusMinimumForSplash","0.0",40,Kind::Float,""},
{"SellPercentage","50%",44,Kind::Percent,""},
{"UnlookPersistDuration","10s",48,Kind::Time,""},
{"DefaultStructureRubbleHeight","8.0",52,Kind::Float,""},
{"TimeAfterDamageUntilRepairAllowed","0",56,Kind::Signed,""},
{"SelectionFlashSaturationFactor","0.5",60,Kind::Float,""},
{"SelectionFlashHouseColor","false",444,Kind::Bool,""},
{"EnableHouseColor","true",445,Kind::Bool,""},
{"GarrisonedRangeMultiplier","1.5",64,Kind::Float,""},
{"GroupMoveClickToGatherAreaFactor","0.5",68,Kind::Float,""},
{"FormationMoveClusterRange","200.0",72,Kind::Float,""},
{"InvisibilityOpacityCycleFrames","60",76,Kind::Signed,""},
{"BuilderFadeOutTime","1s",80,Kind::Time,""},
{"BuilderFadeInTime","1s",84,Kind::Time,""},
{"BuilderMoveFromNewStructureDistance","10.0",88,Kind::Float,""},
{"PartitionCellSize","40.0",92,Kind::Float,""},
{"TerrainResourceCellSize","20.0",96,Kind::Float,""},
{"DefaultStartingCash","1500",100,Kind::Signed,""},
{"MovementPenaltyDamageState","REALLY_DAMAGED",104,Kind::Enum,"BodyDamageType"},
{"ParachuteDriftSpeed","2.0",108,Kind::Float,""},
{"ParachuteLandTime","1.5s",112,Kind::Time,""},
{"LinearDragSubmerged","0.85",116,Kind::Float,""},
{"AutomaticReverseAngle","10d",120,Kind::Angle,""},
{"AutomaticReverseDistance","40",124,Kind::Float,""},
{"SkipMoveMinDistance","10.0",128,Kind::Float,""},
{"CompleteTargetingCompareList",nullptr,132,Kind::Reference,""},
{"ScatterDistance","100.0",136,Kind::Float,""},
{"AIIgnoreDuplicateMoveCommandDistance","20.0",140,Kind::Float,""},
{"HumanIgnoreDuplicateMoveCommandDistance","5.0",144,Kind::Float,""},
{"ConsiderHumanDuplicateMoveCommandDistance","100.0",148,Kind::Float,""},
{"CaptureWaitDistance","20.0",152,Kind::Float,""},
{"CaptureWaitTime","2s",156,Kind::Time,""},
{"CaptureWaitModelConditions",nullptr,160,Kind::Model,""},
};
const Child childrenUnitAISettings[]={
{"VictoryConditionUnitObjectFilter","ObjectFilter",220,ChildKind::Inline},
{"VictoryConditionStructureObjectFilter","ObjectFilter",332,ChildKind::Inline},
};
const Field fieldsPlayerTemplate[]={
{"Type","NONPLAYABLE",4,Kind::Enum,"PlayerTemplateType"},
{"DisplayNameToken",nullptr,8,Kind::String,""},
{"FactionIconImageName",nullptr,16,Kind::String,""},
{"LoadScreenMusic",nullptr,24,Kind::Reference,""},
{"SkirmishMPVictoryMusic",nullptr,28,Kind::Reference,""},
{"SkirmishMPDefeatMusic",nullptr,32,Kind::Reference,""},
{"FactionDataPathMusicEvent",nullptr,36,Kind::Reference,""},
{"MatchVictoryPathMusicEvent",nullptr,40,Kind::Reference,""},
{"MatchDefeatPathMusicEvent",nullptr,44,Kind::Reference,""},
{"Side","Neutral",48,Kind::Enum,"FactionType"},
{"StartingBuilding",nullptr,52,Kind::Hash,""},
{"SpellBookSP",nullptr,56,Kind::Hash,""},
{"SpellBookMP",nullptr,60,Kind::Hash,""},
{"UnattachedMpBeaconTemplate",nullptr,64,Kind::Hash,""},
{"AttachedMpBeaconTemplate",nullptr,68,Kind::Hash,""},
{"MaxMpBeacons","0",72,Kind::Unsigned,""},
{"ResourceModifierValueList",nullptr,76,Kind::Tokens,""},
{"NumberOfResourceAccounts","1",84,Kind::Unsigned,""},
{"ScriptedHuntTactic","",88,Kind::Reference,""},
{"ParachuteOCL",nullptr,92,Kind::Reference,""},
{"DefaultTech",nullptr,96,Kind::Reference,""},
{"PlayerTechUpgradeBinding",nullptr,100,Kind::Reference,""},
{"TechRankPointsPerSecond","0",104,Kind::Unsigned,""},
{"InitialTechPoints","0",108,Kind::Unsigned,""},
{"PowerOutageTurnsOffRadar","true",184,Kind::Bool,""},
};
const Child childrenPlayerTemplate[]={
{"RedAlertTemplate","HashText",112,ChildKind::Pointer},
{"PreferredColor","Color",116,ChildKind::Inline},
{"PreferredCampaignRadarColor","Color",120,ChildKind::Inline},
{"InitialUpgrade","ReferenceText",124,ChildKind::List},
{"StartingUnitInfo","StartingUnitInfoType",132,ChildKind::List},
{"ResourceModifierObjectFilter","ObjectFilter",140,ChildKind::Pointer},
{"StartingMoney","MoneyTransaction",144,ChildKind::List},
{"MoneyCap","MoneyTransaction",152,ChildKind::List},
{"AIBeaconSettings","PlayerTemplateAIBeaconSettings",160,ChildKind::List},
{"TechRank","ReferenceText",168,ChildKind::List},
{"TechProductionBonuses","TechProductionBonusInfo",176,ChildKind::List},
};
const Field fieldsTechProductionBonusInfo[]={
{"RequiredTech",nullptr,0,Kind::Reference,""},
{"BuildCostBonus","0",4,Kind::Percent,""},
{"BuildCostBonusAllies","0",8,Kind::Percent,""},
{"BuildTimeBonus","0",12,Kind::Percent,""},
{"BuildTimeBonusAllies","0",16,Kind::Percent,""},
{"HarvesterBonus","0",20,Kind::Percent,""},
{"HarvesterBonusAllies","0",24,Kind::Percent,""},
};
const Field fieldsPlayerTemplateAIBeaconSettings[]={
{"BeaconType",nullptr,0,Kind::Enum,"AIBeaconType"},
{"UnattachedTemplate",nullptr,4,Kind::Hash,""},
{"AttachedTemplate",nullptr,8,Kind::Hash,""},
};
const Field fieldsMoneyTransaction[]={
{"Account",nullptr,0,Kind::Unsigned,""},
{"Amount",nullptr,4,Kind::Unsigned,""},
};
const Field fieldsGameStatsModeData[]={
{"GameStatsMode",nullptr,4,Kind::Enum,"GameStatsMode"},
};
const Child childrenGameStatsModeData[]={
{"AvailableMapName","StringText",8,ChildKind::List},
};
const Field fieldsAIBuildEntry[]={
{"Build",nullptr,0,Kind::Hash,""},
{"Upgrade",nullptr,4,Kind::Hash,""},
{"Heuristic",nullptr,8,Kind::Enum,"AIBuildHeuristics"},
{"PercentLow","0%",12,Kind::Percent,""},
{"PercentHigh","0%",16,Kind::Percent,""},
{"ExpectedEfficiencyWeight","0.4",20,Kind::Float,""},
{"ActualEfficiencyWeight","0.6",24,Kind::Float,""},
{"SampleSizeForFullWeight","10000",28,Kind::Signed,""},
{"MinTime","0s",32,Kind::Time,""},
{"ForceTime","99999s",36,Kind::Time,""},
{"BlockIfThingNearFactoryDistance","150.0",40,Kind::Float,""},
{"CountModifier","1.0",44,Kind::Float,""},
{"Score","0.0",48,Kind::Float,""},
};
const Child childrenAIBuildEntry[]={
{"BlockIfThingNearFactory","ObjectFilter",52,ChildKind::Pointer},
};
const Field fieldsAIStrategicStateName[]={
{"State",nullptr,4,Kind::Reference,""},
{"Name",nullptr,8,Kind::String,""},
};
const Field fieldsAIPersonalityToAchievementEventMapping[]={
{"AIPersonalityDef",nullptr,4,Kind::Reference,""},
{"AchievementEventId",nullptr,8,Kind::Unsigned,""},
};
const Field fieldsVector3[]={
{"x","0.0",0,Kind::Float,""},
{"y","0.0",4,Kind::Float,""},
{"z","0.0",8,Kind::Float,""},
};
const Field fieldsColor[]={
{"B","0",0,Kind::Byte,""},
{"G","0",1,Kind::Byte,""},
{"R","0",2,Kind::Byte,""},
{"A","255",3,Kind::Byte,""},
};
const Child childrenStartingUnitInfoType[]={
{"Object","ReferenceText",0,ChildKind::Inline},
{"Position","Vector3",4,ChildKind::Inline},
};
const Child childrenMultiplayerSettings[]={
{"InitialCreditsVeryLow","SignedText",4,ChildKind::Inline},
{"InitialCreditsLow","SignedText",8,ChildKind::Inline},
{"InitialCreditsMedium","SignedText",12,ChildKind::Inline},
{"InitialCreditsHigh","SignedText",16,ChildKind::Inline},
{"InitialCreditsVeryHigh","SignedText",20,ChildKind::Inline},
{"StartCountdownTimer","SignedText",24,ChildKind::Inline},
{"MaxBeaconsPerPlayer","SignedText",28,ChildKind::Inline},
{"UseShroud","BoolText",40,ChildKind::Inline},
{"ShowRandomPlayerTemplate","BoolText",41,ChildKind::Inline},
{"ShowRandomStartPos","BoolText",42,ChildKind::Inline},
{"ShowRandomColor","BoolText",43,ChildKind::Inline},
{"GameStatsModeData","GameStatsModeData",32,ChildKind::List},
};
const Child childrenAIBuildStateDefinition[]={
{"BuildEntry","AIBuildEntry",12,ChildKind::List},
{"Heuristic","Heuristics",4,ChildKind::Inline},
};
const Child childrenMultiplayerColor[]={
{"RGBColor","Color",4,ChildKind::Inline},
{"RGBColorT","Color",8,ChildKind::Inline},
{"RGBNightColor","Color",12,ChildKind::Inline},
{"TooltipName","StringText",16,ChildKind::Inline},
{"AvailableInMetaGame","BoolText",24,ChildKind::Inline},
};
#include "XmlplayersFormations.inc"
#include "XmlplayersSkirmish.inc"
Layout GetLayout(const std::string& name) {
if(name=="AIData") return {220,3827723072u,fieldsAIData,60,nullptr,0};
if(name=="UnitAISettings") return {448,2964891936u,fieldsUnitAISettings,42,childrenUnitAISettings,2};
if(name=="PlayerTemplate") return {188,4063352333u,fieldsPlayerTemplate,25,childrenPlayerTemplate,11};
if(name=="TechProductionBonusInfo") return {28,0u,fieldsTechProductionBonusInfo,7,nullptr,0};
if(name=="PlayerTemplateAIBeaconSettings") return {12,0u,fieldsPlayerTemplateAIBeaconSettings,3,nullptr,0};
if(name=="MoneyTransaction") return {8,0u,fieldsMoneyTransaction,2,nullptr,0};
if(name=="GameStatsModeData") return {16,3249689110u,fieldsGameStatsModeData,1,childrenGameStatsModeData,1};
if(name=="AIBuildEntry") return {56,0u,fieldsAIBuildEntry,13,childrenAIBuildEntry,1};
if(name=="AIStrategicStateName") return {16,2010562192u,fieldsAIStrategicStateName,2,nullptr,0};
if(name=="AIPersonalityToAchievementEventMapping") return {12,4086175864u,fieldsAIPersonalityToAchievementEventMapping,2,nullptr,0};
if(name=="Vector3") return {12,0u,fieldsVector3,3,nullptr,0};
if(name=="Color") return {4,0u,fieldsColor,4,nullptr,0};
if(name=="ReferenceText") return {4,0u,nullptr,0,nullptr,0};
if(name=="StringText") return {8,0u,nullptr,0,nullptr,0};
if(name=="HashText") return {4,0u,nullptr,0,nullptr,0};
if(name=="StartingUnitInfoType") return {16,0u,nullptr,0,childrenStartingUnitInfoType,2};
if(name=="MultiplayerSettings") return {44,191082224u,nullptr,0,childrenMultiplayerSettings,12};
if(name=="AIBuildStateDefinition") return {20,1644972043u,nullptr,0,childrenAIBuildStateDefinition,2};
if(name=="SignedText") return {4,0u,nullptr,0,nullptr,0};
if(name=="BoolText") return {1,0u,nullptr,0,nullptr,0};
if(name=="MultiplayerColor") return {28,2384988189u,nullptr,0,childrenMultiplayerColor,5};
Layout formation; if(GetFormationLayout(name,formation)||GetSkirmishLayout(name,formation))return formation;
throw std::invalid_argument("Unsupported player type: "+name);
}
