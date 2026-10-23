// Generated from original Marshal/Tokenize and PE enum maps. See GenerateGameObjectFields.py.
const GameObjectField fields[] = {
    { "Description", 48u, GameObjectValue::String, nullptr, 0 }, // original line 50088
    { "DescriptionTransformed", 56u, GameObjectValue::String, nullptr, 0 }, // original line 50089
    { "TypeDescription", 64u, GameObjectValue::String, nullptr, 0 }, // original line 50090
    { "TypeDescriptionTransformed", 72u, GameObjectValue::String, nullptr, 0 }, // original line 50091
    { "RadarPriority", 80u, GameObjectValue::Enum, "INVALID", -1 }, // original line 50092
    { "TransportSlotCount", 580u, GameObjectValue::Byte, "0", 0 }, // original line 50093
    { "FenceWidth", 84u, GameObjectValue::Float, "0", 0 }, // original line 50094
    { "FenceXOffset", 88u, GameObjectValue::Float, "0", 0 }, // original line 50095
    { "RemoveTerrainRadius", 92u, GameObjectValue::Float, "0.0", 0 }, // original line 50096
    { "EmotionRange", 96u, GameObjectValue::Float, "0", 0 }, // original line 50097
    { "PlacementViewAngle", 100u, GameObjectValue::Angle, "0r", 0 }, // original line 50099
    { "JustBuiltTime", 104u, GameObjectValue::Time, "0s", 0 }, // original line 50101
    { "FactoryExitWidth", 108u, GameObjectValue::Float, "0.0", 0 }, // original line 50102
    { "FactoryExtraBibWidth", 112u, GameObjectValue::Float, "0.0", 0 }, // original line 50103
    { "IsTrainable", 581u, GameObjectValue::Boolean, "true", 0 }, // original line 50104
    { "EditorName", 120u, GameObjectValue::String, nullptr, 0 }, // original line 50106
    { "EditorSorting", 128u, GameObjectValue::Enum, "NONE", 0 }, // original line 50107
    { "BountyValue", 132u, GameObjectValue::Signed, "0", 0 }, // original line 50108
    { "BuildTime", 136u, GameObjectValue::Float, "0.0", 0 }, // original line 50109
    { "RefundValue", 576u, GameObjectValue::Short, "0", 0 }, // original line 50110
    { "EnergyProduction", 140u, GameObjectValue::Signed, "0", 0 }, // original line 50111
    { "EnergyBonus", 144u, GameObjectValue::Signed, "0", 0 }, // original line 50112
    { "IsForbidden", 582u, GameObjectValue::Boolean, "false", 0 }, // original line 50113
    { "IsPrerequisite", 583u, GameObjectValue::Boolean, "false", 0 }, // original line 50114
    { "IsGrabbable", 584u, GameObjectValue::Boolean, "false", 0 }, // original line 50115
    { "IsHarvestable", 585u, GameObjectValue::Boolean, "false", 0 }, // original line 50116
    { "VoicePriority", 168u, GameObjectValue::Signed, nullptr, 0 }, // original line 50122
    { "MinZIncreaseForVoiceMoveToHigherGround", 172u, GameObjectValue::Float, "-1", 0 }, // original line 50123
    { "CampnessValue", 180u, GameObjectValue::Unsigned, "0", 0 }, // original line 50125
    { "CampnessValueRadius", 184u, GameObjectValue::Float, "600.0", 0 }, // original line 50126
    { "Scale", 188u, GameObjectValue::Float, "1.0", 0 }, // original line 50127
    { "HealthBoxHeightOffset", 192u, GameObjectValue::Float, "10.0", 0 }, // original line 50128
    { "LiveCameraFXPitch", 196u, GameObjectValue::Float, "0", 0 }, // original line 50129
    { "FormationWidth", 200u, GameObjectValue::Signed, "1", 0 }, // original line 50130
    { "FormationDepth", 204u, GameObjectValue::Signed, "1", 0 }, // original line 50131
    { "InstanceScaleFuzziness", 208u, GameObjectValue::Float, nullptr, 0 }, // original line 50132
    { "StructureRubbleHeight", 212u, GameObjectValue::Float, "0", 0 }, // original line 50133
    { "MaxSimultaneousOfType", 578u, GameObjectValue::Short, nullptr, 0 }, // original line 50134
    { "RamPower", 216u, GameObjectValue::Signed, "0", 0 }, // original line 50135
    { "RamZMult", 220u, GameObjectValue::Float, "1.0", 0 }, // original line 50136
    { "ShockwaveResistance", 224u, GameObjectValue::Float, "0", 0 }, // original line 50137
    { "CommandPoints", 228u, GameObjectValue::Signed, "0", 0 }, // original line 50138
    { "CommandPointBonus", 232u, GameObjectValue::Signed, "0", 0 }, // original line 50139
    { "VoiceAttackChargeTimeout", 236u, GameObjectValue::Unsigned, "150", 0 }, // original line 50140
    { "VoiceMoveLandToWaterTimeout", 240u, GameObjectValue::Time, "5s", 0 }, // original line 50142
    { "VoiceMoveWaterToLandTimeout", 244u, GameObjectValue::Time, "5s", 0 }, // original line 50144
    { "VoiceSelectUnderFireTimeout", 248u, GameObjectValue::Time, "20s", 0 }, // original line 50146
    { "VoiceSelectUnderFireDamageTime", 252u, GameObjectValue::Time, "3s", 0 }, // original line 50148
    { "MaxDistanceForEngaged", 256u, GameObjectValue::Float, "150", 0 }, // original line 50149
    { "EngagedStateTimeout", 260u, GameObjectValue::Unsigned, "30", 0 }, // original line 50150
    { "ThreatLevel", 264u, GameObjectValue::Float, "1.0", 0 }, // original line 50151
    { "ForceLuaRegistration", 586u, GameObjectValue::Boolean, "false", 0 }, // original line 50152
    { "KeepSelectableWhenDead", 587u, GameObjectValue::Boolean, "false", 0 }, // original line 50153
    { "IsAutoBuilt", 588u, GameObjectValue::Boolean, "false", 0 }, // original line 50154
    { "CanPathThroughGates", 589u, GameObjectValue::Boolean, "true", 0 }, // original line 50155
    { "SlopeLimitIndex", 268u, GameObjectValue::Unsigned, "0", 0 }, // original line 50156
    { "PathfindDiameter", 272u, GameObjectValue::Float, "-1", 0 }, // original line 50157
    { "SupplyOverride", 276u, GameObjectValue::Signed, "0", 0 }, // original line 50158
    { "ExperienceScalarTable", 280u, GameObjectValue::String, nullptr, 0 }, // original line 50159
    { "FiringArc", 288u, GameObjectValue::Float, "360", 0 }, // original line 50160
    { "CamouflageDetectorLevel", 292u, GameObjectValue::Signed, "1", 0 }, // original line 50161
    { "SelectionPriority", 296u, GameObjectValue::Signed, "2147483647", 0 }, // original line 50162
    { "PathPriority", 300u, GameObjectValue::Signed, "25", 0 }, // original line 50163
    { "ProductionQueueType", 304u, GameObjectValue::Enum, "INVALID", -1 }, // original line 50164
    { "BuildPlacementTypeFlag", 308u, GameObjectValue::Enum, "INVALID", 0 }, // original line 50165
    { "BuildInProximityToSamePlayerStucture", 590u, GameObjectValue::Boolean, "true", 0 }, // original line 50166
    { "UnitCategory", 348u, GameObjectValue::Enum, "NONE", -1 }, // original line 50168
    { "WeaponCategory", 352u, GameObjectValue::Enum, "NONE", -1 }, // original line 50169
    { "HasFiredRecentlyTime", 356u, GameObjectValue::Time, "2s", 0 }, // original line 50171
    { "ReinvisibilityDelay", 364u, GameObjectValue::Time, "0.5s", 0 }, // original line 50174
    { "InvisibilityOpacityMin", 368u, GameObjectValue::Float, "0.4", 0 }, // original line 50175
    { "InvisibilityOpacityMax", 372u, GameObjectValue::Float, "0.9", 0 }, // original line 50176
    { "SubGroupPriority", 380u, GameObjectValue::Signed, "500", 0 }, // original line 50178
    { "EvaEventSecondDamageFarFromFirstScanRange", 384u, GameObjectValue::Float, "200", 0 }, // original line 50179
    { "EvaEventSecondDamageFarFromFirstTimeoutMS", 388u, GameObjectValue::DurationDefault, "31000", 155 }, // original line 50181
};
const GameObjectEnum enumEntries[] = {
    { "RadarPriority", "COMMANDO", 2 },
    { "RadarPriority", "INVALID", -1 },
    { "RadarPriority", "NOT_ON_RADAR", -2 },
    { "RadarPriority", "STRUCTURE", 0 },
    { "RadarPriority", "UNIT", 1 },
    { "EditorSorting", "AUDIO", 8 },
    { "EditorSorting", "CAMPAIGN_STRUCTURES", 19 },
    { "EditorSorting", "CAMPAIGN_UNITS", 18 },
    { "EditorSorting", "DEBRIS", 6 },
    { "EditorSorting", "DESTRUCTIBLE_PROPS", 16 },
    { "EditorSorting", "EMITTERS", 13 },
    { "EditorSorting", "FOR_REVIEW", 10 },
    { "EditorSorting", "MISC_MAN_MADE", 4 },
    { "EditorSorting", "MISC_NATURAL", 5 },
    { "EditorSorting", "NONE", 0 },
    { "EditorSorting", "OBSOLETE", 15 },
    { "EditorSorting", "OPTIMIZED_PROPS", 17 },
    { "EditorSorting", "ROAD", 11 },
    { "EditorSorting", "SCORCH", 14 },
    { "EditorSorting", "SHRUBBERY", 3 },
    { "EditorSorting", "STRUCTURE", 1 },
    { "EditorSorting", "SYSTEM", 7 },
    { "EditorSorting", "TEST", 9 },
    { "EditorSorting", "UNIT", 2 },
    { "EditorSorting", "WAYPOINT", 12 },
    { "ProductionQueueType", "AIRCRAFT", 4 },
    { "ProductionQueueType", "INFANTRY", 2 },
    { "ProductionQueueType", "INVALID", -1 },
    { "ProductionQueueType", "MAIN_STRUCTURE", 0 },
    { "ProductionQueueType", "OTHER_STRUCTURE", 1 },
    { "ProductionQueueType", "UPGRADE", 5 },
    { "ProductionQueueType", "VEHICLE", 3 },
    { "ProductionQueueType", "WATERCRAFT", 6 },
    { "BuildPlacementTypeFlag", "BLOCKED", 4 },
    { "BuildPlacementTypeFlag", "INVALID", 0 },
    { "BuildPlacementTypeFlag", "MAIN_STRUCTURE", 1 },
    { "BuildPlacementTypeFlag", "OTHER_STRUCTURE", 2 },
    { "BuildPlacementTypeFlag", "TIBERIUM_FIELD", 3 },
    { "UnitCategory", "AIRCRAFT", 3 },
    { "UnitCategory", "INFANTRY", 1 },
    { "UnitCategory", "NONE", -1 },
    { "UnitCategory", "STRUCTURE", 0 },
    { "UnitCategory", "VEHICLE", 2 },
    { "WeaponCategory", "BLAST", 1 },
    { "WeaponCategory", "CANNON", 2 },
    { "WeaponCategory", "GUN", 0 },
    { "WeaponCategory", "LIGHTNING", 4 },
    { "WeaponCategory", "MISSILE", 3 },
    { "WeaponCategory", "NONE", -1 },
};
const GameObjectEnumFallback enumFallbacks[] = {
    { "RadarPriority", 2 },
    { "EditorSorting", 8 },
    { "ProductionQueueType", 4 },
    { "BuildPlacementTypeFlag", 4 },
    { "UnitCategory", 3 },
    { "WeaponCategory", 1 },
};
const GameObjectToken tokens[] = {
    { 4185812704u, 2600243603u, 4u, GameObjectTokenKind::Member },
    { 3067491221u, 1835445784u, 40u, GameObjectTokenKind::List },
    { 2503394572u, 438907818u, 48u, GameObjectTokenKind::Member },
    { 2071352306u, 438907818u, 56u, GameObjectTokenKind::Member },
    { 1405703153u, 438907818u, 64u, GameObjectTokenKind::Member },
    { 2553711004u, 438907818u, 72u, GameObjectTokenKind::Member },
    { 1256201774u, 2635111608u, 80u, GameObjectTokenKind::Member },
    { 2603815667u, 814836630u, 580u, GameObjectTokenKind::Member },
    { 2784876747u, 1683514611u, 84u, GameObjectTokenKind::Member },
    { 4115183252u, 1683514611u, 88u, GameObjectTokenKind::Member },
    { 2374071002u, 1683514611u, 92u, GameObjectTokenKind::Member },
    { 3901548879u, 1683514611u, 96u, GameObjectTokenKind::Member },
    { 3754195860u, 1310124621u, 100u, GameObjectTokenKind::Member },
    { 707785181u, 2575722984u, 104u, GameObjectTokenKind::Member },
    { 3506240728u, 1683514611u, 108u, GameObjectTokenKind::Member },
    { 405691149u, 1683514611u, 112u, GameObjectTokenKind::Member },
    { 3144851442u, 2011428087u, 581u, GameObjectTokenKind::Member },
    { 449781326u, 1873548347u, 116u, GameObjectTokenKind::Member },
    { 444549785u, 438907818u, 120u, GameObjectTokenKind::Member },
    { 847505653u, 3650538541u, 128u, GameObjectTokenKind::Member },
    { 2693990618u, 602296504u, 132u, GameObjectTokenKind::Member },
    { 3394036936u, 1683514611u, 136u, GameObjectTokenKind::Member },
    { 779772782u, 1129320594u, 576u, GameObjectTokenKind::Member },
    { 2792070950u, 602296504u, 140u, GameObjectTokenKind::Member },
    { 2790196718u, 602296504u, 144u, GameObjectTokenKind::Member },
    { 4150983076u, 2011428087u, 582u, GameObjectTokenKind::Member },
    { 1825943781u, 2011428087u, 583u, GameObjectTokenKind::Member },
    { 4162298070u, 2011428087u, 584u, GameObjectTokenKind::Member },
    { 1767410626u, 2011428087u, 585u, GameObjectTokenKind::Member },
    { 2584759047u, 2245282653u, 148u, GameObjectTokenKind::Member },
    { 3330187981u, 1019125366u, 152u, GameObjectTokenKind::Member },
    { 771982898u, 1019125366u, 156u, GameObjectTokenKind::Member },
    { 4171715770u, 1019125366u, 160u, GameObjectTokenKind::Member },
    { 3689831051u, 1019125366u, 164u, GameObjectTokenKind::Member },
    { 2960732144u, 602296504u, 168u, GameObjectTokenKind::Member },
    { 2851957643u, 1683514611u, 172u, GameObjectTokenKind::Member },
    { 400896388u, 2597951987u, 176u, GameObjectTokenKind::Member },
    { 3038829189u, 1982953380u, 180u, GameObjectTokenKind::Member },
    { 248528920u, 1683514611u, 184u, GameObjectTokenKind::Member },
    { 3749463182u, 1683514611u, 188u, GameObjectTokenKind::Member },
    { 1294491905u, 1683514611u, 192u, GameObjectTokenKind::Member },
    { 3539859246u, 1683514611u, 196u, GameObjectTokenKind::Member },
    { 85538830u, 602296504u, 200u, GameObjectTokenKind::Member },
    { 1498319417u, 602296504u, 204u, GameObjectTokenKind::Member },
    { 3687407556u, 1683514611u, 208u, GameObjectTokenKind::Member },
    { 1031951649u, 1683514611u, 212u, GameObjectTokenKind::Member },
    { 1216152939u, 1129320594u, 578u, GameObjectTokenKind::Member },
    { 3185619309u, 602296504u, 216u, GameObjectTokenKind::Member },
    { 1475445830u, 1683514611u, 220u, GameObjectTokenKind::Member },
    { 3971031051u, 1683514611u, 224u, GameObjectTokenKind::Member },
    { 694860650u, 602296504u, 228u, GameObjectTokenKind::Member },
    { 3937849036u, 602296504u, 232u, GameObjectTokenKind::Member },
    { 3808143923u, 1982953380u, 236u, GameObjectTokenKind::Member },
    { 2773906269u, 2575722984u, 240u, GameObjectTokenKind::Member },
    { 3975318301u, 2575722984u, 244u, GameObjectTokenKind::Member },
    { 170801484u, 2575722984u, 248u, GameObjectTokenKind::Member },
    { 3419462363u, 2575722984u, 252u, GameObjectTokenKind::Member },
    { 2816668280u, 1683514611u, 256u, GameObjectTokenKind::Member },
    { 3527745219u, 1982953380u, 260u, GameObjectTokenKind::Member },
    { 2984215616u, 1683514611u, 264u, GameObjectTokenKind::Member },
    { 548801598u, 2011428087u, 586u, GameObjectTokenKind::Member },
    { 3802663823u, 2011428087u, 587u, GameObjectTokenKind::Member },
    { 3434745923u, 2011428087u, 588u, GameObjectTokenKind::Member },
    { 1531640618u, 2011428087u, 589u, GameObjectTokenKind::Member },
    { 3694606693u, 1982953380u, 268u, GameObjectTokenKind::Member },
    { 1311875094u, 1683514611u, 272u, GameObjectTokenKind::Member },
    { 3482443700u, 602296504u, 276u, GameObjectTokenKind::Member },
    { 1143703929u, 438907818u, 280u, GameObjectTokenKind::Member },
    { 2471770621u, 1683514611u, 288u, GameObjectTokenKind::Member },
    { 1575092515u, 602296504u, 292u, GameObjectTokenKind::Member },
    { 2271802027u, 602296504u, 296u, GameObjectTokenKind::Member },
    { 1846208615u, 602296504u, 300u, GameObjectTokenKind::Member },
    { 3163048042u, 3163048042u, 304u, GameObjectTokenKind::Member },
    { 2630826128u, 549004363u, 308u, GameObjectTokenKind::Member },
    { 2521734105u, 2011428087u, 590u, GameObjectTokenKind::Member },
    { 1445533692u, 2600243603u, 312u, GameObjectTokenKind::Member },
    { 2482724672u, 2482724672u, 348u, GameObjectTokenKind::Member },
    { 668229763u, 668229763u, 352u, GameObjectTokenKind::Member },
    { 552856633u, 2575722984u, 356u, GameObjectTokenKind::Member },
    { 3614134471u, 2716967967u, 360u, GameObjectTokenKind::Member },
    { 2154577981u, 2575722984u, 364u, GameObjectTokenKind::Member },
    { 506097697u, 1683514611u, 368u, GameObjectTokenKind::Member },
    { 3598372541u, 1683514611u, 372u, GameObjectTokenKind::Member },
    { 682912317u, 3769189345u, 376u, GameObjectTokenKind::Member },
    { 3635298881u, 602296504u, 380u, GameObjectTokenKind::Member },
    { 2763501760u, 1683514611u, 384u, GameObjectTokenKind::Member },
    { 2853411872u, 1869421287u, 388u, GameObjectTokenKind::Member },
    { 1870180772u, 1786164858u, 392u, GameObjectTokenKind::Member },
    { 926479574u, 438907818u, 396u, GameObjectTokenKind::Null },
    { 797144851u, 438907818u, 400u, GameObjectTokenKind::Null },
    { 1812791604u, 1836672734u, 404u, GameObjectTokenKind::Null },
    { 1789579963u, 1789579963u, 408u, GameObjectTokenKind::Null },
    { 681740580u, 1583019381u, 412u, GameObjectTokenKind::List },
    { 1852568706u, 1852568706u, 420u, GameObjectTokenKind::List },
    { 9963775u, 4118694088u, 428u, GameObjectTokenKind::Member },
    { 1022237882u, 1723492635u, 432u, GameObjectTokenKind::Member },
    { 4079217817u, 1835445784u, 436u, GameObjectTokenKind::Null },
    { 2160707259u, 1835445784u, 440u, GameObjectTokenKind::Null },
    { 3726299163u, 344905565u, 444u, GameObjectTokenKind::List },
    { 601607184u, 2654902107u, 452u, GameObjectTokenKind::Member },
    { 272911724u, 272911724u, 456u, GameObjectTokenKind::Null },
    { 465968874u, 465968874u, 460u, GameObjectTokenKind::Null },
    { 1373506191u, 4022052753u, 464u, GameObjectTokenKind::Null },
    { 3015298429u, 620546786u, 468u, GameObjectTokenKind::Null },
    { 1376227828u, 1608018205u, 472u, GameObjectTokenKind::List },
    { 1868326721u, 2947372788u, 480u, GameObjectTokenKind::Null },
    { 346717295u, 835647844u, 484u, GameObjectTokenKind::Null },
    { 3007506658u, 3664568761u, 488u, GameObjectTokenKind::Null },
    { 4001023936u, 4001023936u, 492u, GameObjectTokenKind::Null },
    { 1471645405u, 1471645405u, 496u, GameObjectTokenKind::Null },
    { 3786889584u, 3786889584u, 500u, GameObjectTokenKind::Null },
    { 1972927658u, 1972927658u, 504u, GameObjectTokenKind::Null },
    { 2393386135u, 2393386135u, 508u, GameObjectTokenKind::Null },
    { 3612602563u, 3612602563u, 512u, GameObjectTokenKind::Null },
    { 2471735507u, 3612602563u, 516u, GameObjectTokenKind::Null },
    { 222595978u, 691437687u, 520u, GameObjectTokenKind::Null },
    { 164725313u, 164725313u, 524u, GameObjectTokenKind::Null },
    { 1501986059u, 1501986059u, 528u, GameObjectTokenKind::Null },
    { 4135037614u, 4135037614u, 532u, GameObjectTokenKind::Null },
    { 754362008u, 438907818u, 536u, GameObjectTokenKind::List },
    { 2197716616u, 2197716616u, 544u, GameObjectTokenKind::Null },
    { 2236001359u, 2236001359u, 548u, GameObjectTokenKind::Null },
    { 1688180689u, 1688180689u, 552u, GameObjectTokenKind::Null },
    { 519758843u, 519758843u, 556u, GameObjectTokenKind::Null },
    { 1388118305u, 1388118305u, 560u, GameObjectTokenKind::List },
    { 2658004210u, 653971399u, 568u, GameObjectTokenKind::List },
};
