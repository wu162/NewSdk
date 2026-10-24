// Original XmlCompiler blob 2d2f471c8008b7c2e1f78e39752d0726da395cea.
// Include inside MakeLayouts(), immediately before return m.
// Child Marshal/Endian source: parts0113-0116, original lines56927-57964.
// Sizes: original SageBinaryData StructLayout; field order is Marshal order.
// Requires GetEnum fallback to GetXmlaiPersonalityEnum (XmlaiPersonalityEnums.inc).
// Explicitly unsupported: Side text-reference list; OpeningMove/Heuristic
// nullable pointer-to-list. Do not substitute an inline Heuristics child at @12.
// Original Marshal line 57069
m["AIPersonalityDefMap"] = Layout{8,0u,{
    {"Side",nullptr,0,Kind::Reference,false,"",0,0},
    {"Personality",nullptr,4,Kind::Reference,false,"",0,0}
},{}};
// Original Marshal line 57227
m["AIUnitCap"] = Layout{8,0u,{
    {"Cap","100",0,Kind::Signed,false,"",0,0},
    {"Difficulty","EASY MEDIUM HARD BRUTAL",4,Kind::Flags,false,"AIDifficulty",4,3}
},{}};
// Original Marshal line 57443
m["AIBuildDelay"] = Layout{8,0u,{
    {"Delay","0s",0,Kind::Time,false,"",0,0},
    {"Difficulty","EASY MEDIUM HARD BRUTAL",4,Kind::Flags,false,"AIDifficulty",4,3}
},{}};
// Original Marshal line 57648
m["AIBuildDelayRange"] = Layout{20,0u,{
    {"MinDelay","0s",0,Kind::Time,false,"",0,0},
    {"MaxDelay","0s",4,Kind::Time,false,"",0,0},
    {"MinTTKRatio","1",8,Kind::Float,false,"",0,0},
    {"MaxTTKRatio","1",12,Kind::Float,false,"",0,0},
    {"Difficulty","EASY MEDIUM HARD BRUTAL",16,Kind::Flags,false,"AIDifficulty",4,3}
},{}};
// Original Marshal line 57464
m["AIUnitBuilderUnitChoiceStrategy"] = Layout{12,0u,{
    {"Shape","FLAT",0,Kind::Enum,false,"AIUnitBuilderUnitChoiceShape",0,0},
    {"StandardDeviation","1",4,Kind::Float,false,"",0,0},
    {"Difficulty","EASY MEDIUM HARD BRUTAL",8,Kind::Flags,false,"AIDifficulty",4,3}
},{}};
// Original Marshal line 57105
m["AIUnitBuilderUnitChoiceStrategyAdaptive"] = Layout{28,0u,{
    {"MinEffectiveness","0.5",0,Kind::Float,false,"",0,0},
    {"MaxEffectiveness","0.5",4,Kind::Float,false,"",0,0},
    {"MinStandardDeviation","0.5",8,Kind::Float,false,"",0,0},
    {"MaxStandardDeviation","0.5",12,Kind::Float,false,"",0,0},
    {"MinEfficiency","1",16,Kind::Float,false,"",0,0},
    {"MaxEfficiency","1",20,Kind::Float,false,"",0,0},
    {"Difficulty","EASY MEDIUM HARD BRUTAL",24,Kind::Flags,false,"AIDifficulty",4,3}
},{}};
// Original Marshal line 57247
m["AIUnitBuilderEvaluationDelayRange"] = Layout{24,0u,{
    {"UseAllAvailableQueues","true",20,Kind::Bool,false,"",0,0},
    {"MinDelay","0s",0,Kind::Time,false,"",0,0},
    {"MaxDelay","0s",4,Kind::Time,false,"",0,0},
    {"MinEfficiency","1",8,Kind::Float,false,"",0,0},
    {"MaxEfficiency","1",12,Kind::Float,false,"",0,0},
    {"Difficulty","EASY MEDIUM HARD BRUTAL",16,Kind::Flags,false,"AIDifficulty",4,3}
},{}};
// Original Marshal line 57423
m["AIResourceMultiplierCheat"] = Layout{8,0u,{
    {"Percentage","100%",0,Kind::Percent,false,"",0,0},
    {"Difficulty","EASY MEDIUM HARD BRUTAL",4,Kind::Flags,false,"AIDifficulty",4,3}
},{}};
// Original Marshal line 57360
m["AIStrategicDirectiveFeedbackMap"] = Layout{12,0u,{
    {"StrategicDirective",nullptr,0,Kind::Enum,false,"AIStrategicDirective",0,1},
    {"Event",nullptr,4,Kind::Enum,false,"AIStrategicDirectiveEvent",0,0},
    {"FeedbackKey",nullptr,8,Kind::CaseHash,false,"",0,0}
},{}};
// Original Marshal line 57078
m["AISkirmishGameEventFeedbackMap"] = Layout{8,0u,{
    {"Event",nullptr,0,Kind::Enum,false,"AIFeedbackWorthySkirmishGameEvent",0,4},
    {"FeedbackKey",nullptr,4,Kind::CaseHash,false,"",0,0}
},{}};
// Original Marshal line 57340
m["AIPlayerPowerChoice"] = Layout{8,0u,{
    {"PlayerPower",nullptr,0,Kind::Reference,true,"",0,0},
    {"Weight","20%",4,Kind::Percent,false,"",0,0}
},{}};
// Original Marshal line 57740
m["AIWeightedOpeningMove"] = Layout{16,0u,{
    {"Name",nullptr,0,Kind::Reference,false,"",0,0},
    {"Weight","100%",4,Kind::Percent,false,"",0,0},
    {"Difficulty","EASY MEDIUM HARD BRUTAL",8,Kind::Flags,false,"AIDifficulty",4,3}
},{}};
// Original Marshal line 57813
m["AIStrategicState"] = Layout{12,0u,{
    {"State",nullptr,4,Kind::Reference,false,"",0,0},
    {"Difficulty","EASY MEDIUM HARD BRUTAL",8,Kind::Flags,false,"AIDifficulty",4,3}
},{}};
// Original Marshal line 57951
m["AIBudgetState"] = Layout{12,0u,{
    {"State",nullptr,4,Kind::Reference,false,"",0,0},
    {"Difficulty","EASY MEDIUM HARD BRUTAL",8,Kind::Flags,false,"AIDifficulty",4,3}
},{}};
// Original Marshal line 57937
m["AIBuildState"] = Layout{12,0u,{
    {"State",nullptr,4,Kind::Reference,false,"",0,0},
    {"Difficulty","EASY MEDIUM HARD BRUTAL",8,Kind::Flags,false,"AIDifficulty",4,3}
},{}};
// Original Marshal line 57591
m["AISpecificUnitCap"] = Layout{12,0u,{
    {"Unit",nullptr,0,Kind::Hash,false,"",0,0},
    {"Cap","1",4,Kind::Signed,false,"",0,0},
    {"Difficulty","EASY MEDIUM HARD BRUTAL",8,Kind::Flags,false,"AIDifficulty",4,3}
},{}};
// Original Marshal line 57518
m["AIUnitBuilderModifier"] = Layout{16,0u,{
    {"Unit",nullptr,0,Kind::Hash,false,"",0,0},
    {"OffensiveModifier","1.0",4,Kind::Float,false,"",0,0},
    {"DefensiveModifier","1.0",8,Kind::Float,false,"",0,0},
    {"Difficulty","EASY MEDIUM HARD BRUTAL",12,Kind::Flags,false,"AIDifficulty",4,3}
},{}};
// Original Marshal line 57827
m["AIStateFeedbackEntry"] = Layout{36,0u,{
    {"FeedbackKey",nullptr,0,Kind::CaseHash,false,"",0,0},
    {"EvaEvent","",4,Kind::Reference,false,"",0,0},
    {"Sound","",8,Kind::Reference,false,"",0,0},
    {"Text","",12,Kind::String,false,"",0,0},
    {"Video","",20,Kind::String,false,"",0,0},
    {"FeedbackTo","ALLIES ENEMIES NEUTRAL",28,Kind::Flags,false,"ObjectFilterRelationship",4,0},
    {"Timeout",nullptr,32,Kind::Time,true,"",0,0}
},{}};
// Original Marshal line 56927
m["AIInTroubleSkirmishFeedbackControl"] = Layout{32,0u,{
    {"OnlyIfLocalPlayerIs",nullptr,0,Kind::Flags,false,"ObjectFilterRelationship",4,0},
    {"TimeoutAfterPlaying",nullptr,4,Kind::Time,false,"",0,0},
    {"TimeBetweenScans",nullptr,8,Kind::Time,false,"",0,0},
    {"TimeThatConditionsMustBeMet",nullptr,12,Kind::Time,false,"",0,0},
    {"NumberOfObjectsRecentlyLost",nullptr,16,Kind::Signed,false,"",0,0},
    {"TimeForNumberOfObjectLost",nullptr,20,Kind::Time,false,"",0,0},
    {"ObjectLostFilter",nullptr,24,Kind::Reference,false,"",0,0},
    {"MinPowerAdvantageEnemiesInMyBaseVsAllMyUnits",nullptr,28,Kind::Float,false,"",0,0}
},{}};
// Restore original child allocation order: lists @236..372, criteria @380,
// inline filter @384, trouble pointer @496, feedback list @500.
// Existing criteria/filter entries are preserved, moved to their original place.
m["AIPersonalityDefinition"].children = {
    {"PersonalityMap","AIPersonalityDefMap",244,ChildKind::List},
    {"UnitCap","AIUnitCap",252,ChildKind::List},
    {"BuildDelay","AIBuildDelay",260,ChildKind::List},
    {"BuildDelayRange","AIBuildDelayRange",268,ChildKind::List},
    {"UnitBuilderUnitChoiceStrategy","AIUnitBuilderUnitChoiceStrategy",276,ChildKind::List},
    {"UnitBuilderUnitChoiceStrategyAdaptive","AIUnitBuilderUnitChoiceStrategyAdaptive",284,ChildKind::List},
    {"UnitBuilderEvaluationDelayRange","AIUnitBuilderEvaluationDelayRange",292,ChildKind::List},
    {"ResourceMultiplierCheat","AIResourceMultiplierCheat",300,ChildKind::List},
    {"StrategicDirectiveFeedback","AIStrategicDirectiveFeedbackMap",308,ChildKind::List},
    {"SkirmishGameEventFeedback","AISkirmishGameEventFeedbackMap",316,ChildKind::List},
    {"PowerChoice","AIPlayerPowerChoice",324,ChildKind::List},
    {"OpeningMove","AIWeightedOpeningMove",332,ChildKind::List},
    {"StrategicState","AIStrategicState",340,ChildKind::List},
    {"BudgetState","AIBudgetState",348,ChildKind::List},
    {"BuildState","AIBuildState",356,ChildKind::List},
    {"SpecificUnitCap","AISpecificUnitCap",364,ChildKind::List},
    {"UnitModifier","AIUnitBuilderModifier",372,ChildKind::List},
    {"DefaultTacticHomeBaseCriteria","AIBaseSelectionCriteria",380,ChildKind::Pointer},
    {"CountAsStructureForBuildPurposesFilter","ObjectFilter",384,ChildKind::Inline},
    {"InTroubleSkirmishFeedbackControl","AIInTroubleSkirmishFeedbackControl",496,ChildKind::Pointer},
    {"FeedbackEntry","AIStateFeedbackEntry",500,ChildKind::List}
};
