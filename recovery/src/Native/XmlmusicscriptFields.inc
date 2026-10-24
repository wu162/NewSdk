const Field F1[]={{"TimeBetweenConditionChecks",4,Time,"5s"}};
const Field F2[]={{"CountDeadPlayersAsObservers",4,Bool,nullptr}};
const Field F3[]={{"TimeBetweenConditionChecks",4,Time,"5s"},{"MinUnitsToPass",8,Int,nullptr},{"MinDistanceFromBase",12,Real,nullptr},{"Relationship",16,Relationship,nullptr}};
const Field F4[]={{"Timeout",4,Time,nullptr}};
const Field F5[]={{"Track",4,LowerHash,nullptr},{"Count",8,Int,nullptr}};
const Field F6[]={{"TrackType",4,RawHashKind,nullptr}};
const Field F8[]={{"TimeBetweenConditionChecks",4,Time,"5s"},{"Count",8,Int,"1"},{"RequiredModelConditions",12,Model,""},{"ExcludedModelConditions",72,Model,""}};
const Field F9[]={{"EvaEvent",4,Ref,nullptr},{"Timeout",8,Time,"1s"}};
const Field F10[]={{"TimeBetweenConditionChecks",4,Time,"5s"},{"EvaEvent",8,Ref,nullptr},{"Count",12,Int,"1"},{"Distance",16,Real,nullptr}};
const Field F11[]={{"Count",4,Int,nullptr},{"Time",8,Time,nullptr},{"CountOnlyKillsAgainstTheLocalPlayer",16,Bool,nullptr},{"CountOnlyKillsByTheLocalPlayer",17,Bool,nullptr},{"Filter",12,PtrRef,nullptr}};
const Field F15[]={{"TrackedState",4,LowerHash,nullptr},{"TrackedStateChange",8,Change,nullptr},{"TimeSinceLastActive",12,Time,nullptr},{"TimeSicneLastActiveOp",16,Compare,nullptr}};
const Field F16[]={{"TrackedState",4,LowerHash,nullptr}};
const Field F17[]={{"TimeBetweenConditionChecks",4,Time,"5s"},{"Count",8,Int,nullptr},{"Time",12,Time,nullptr},{"CountOnlyEngagementsAgainstTheLocalPlayer",24,Bool,nullptr},{"CountOnlyEngagementsByTheLocalPlayer",25,Bool,nullptr},{"RequiredScreenStatus",16,Screen,nullptr},{"TargetFilter",20,PtrRef,nullptr}};
const Field F18[]={{"TimeBetweenConditionChecks",4,Time,"5s"},{"Time",8,Time,nullptr},{"Ratio",12,Real,nullptr},{"RatioOp",16,Compare,nullptr}};
const Field F19[]={{"TrackedWeapon",4,LowerHash,nullptr},{"Time",8,Time,nullptr},{"Count",12,Int,nullptr},{"CountOnlyHitsAgainstTheLocalPlayer",20,Bool,nullptr},{"TargetFilter",16,PtrRef,nullptr}};
const Field F20[]={{"TrackTypeKey",4,RawHashKind,""},{"Level",8,Int,nullptr},{"Priority",12,Int,nullptr},{"Condition",16,PtrRef,nullptr},{"ConditionsAreLatch",32,Bool,"false"},{"Track",20,Ref,nullptr},{"FadeInTrack",33,Bool,"false"},{"FadeOutLowerLevelTrack",34,Bool,"true"}};
const Layout layouts[]={
{"MusicScriptConditionNuggetBase",4,nullptr,0,0,""},
{"MusicScriptConditionExpensiveNuggetBase",8,F1,1,0,""},
{"MusicScriptConditionNugget_LocalPlayerIsObserver",8,F2,1,0,""},
{"MusicScriptConditionNugget_UnitsFarFromBase",20,F3,4,0,""},
{"MusicScriptConditionNugget_TimeFromStartOfLevel",8,F4,1,0,""},
{"MusicScriptConditionNugget_TrackPlayedCount",12,F5,2,0,""},
{"MusicScriptConditionNugget_SpecificTrackTypePlaying",8,F6,1,0,""},
{"MusicScriptConditionNugget_AnyTrackPlaying",4,nullptr,0,0,""},
{"MusicScriptConditionNugget_ObjectsOfTypeExist",136,F8,4,132,"Filter"},
{"MusicScriptConditionNugget_EvaEventPlayedRecently",12,F9,2,0,""},
{"MusicScriptConditionNugget_ObjectsNearEvaEvent",24,F10,4,20,"Filter"},
{"MusicScriptConditionNugget_ScoredKillCount",20,F11,5,0,""},
{"MusicScriptConditionNugget_Not",8,nullptr,0,0,"Condition"},
{"MusicScriptConditionNugget_Or",12,nullptr,0,0,"Condition"},
{"MusicScriptConditionNugget_And",12,nullptr,0,0,"Condition"},
{"MusicScriptConditionNugget_PathMusicStateLastActive",20,F15,4,0,""},
{"MusicScriptConditionNugget_PathMusicStateCurrentlyActive",8,F16,1,0,""},
{"MusicScriptConditionNugget_EngagementCount",28,F17,7,0,""},
{"MusicScriptConditionNugget_LocalPlayerKillRatio",20,F18,4,0,""},
{"MusicScriptConditionNugget_HitBySuperWeapon",24,F19,5,0,""},
{"MusicScriptTrack",36,F20,8,0,"Timeout"}
};

