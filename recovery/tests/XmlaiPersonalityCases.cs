using System.Collections.Generic;
using System.Text;

// Positive full-root fixtures for the existing original XmlCompiler matrix.
// Side text list and OpeningMove/Heuristic pointer-list are excluded until the
// owner extends Builder; they must continue to be tested as explicit rejection.
internal static class XmlaiPersonalityCases
{
    static string Root(string children) {
        return "<AIPersonalityDefinition xmlns='uri:ea.com:eala:asset' id='Oracle'>"+children+"</AIPersonalityDefinition>";
    }
    static string Item(string name,string attrs) { return "<"+name+(attrs.Length==0?"":" "+attrs)+"/>"; }
    public static IEnumerable<string> Cases() {
        yield return Root("");
        string[] names = {"PersonalityMap","UnitCap","BuildDelay","BuildDelayRange","UnitBuilderUnitChoiceStrategy","UnitBuilderUnitChoiceStrategyAdaptive","UnitBuilderEvaluationDelayRange","ResourceMultiplierCheat","StrategicDirectiveFeedback","SkirmishGameEventFeedback","PowerChoice","OpeningMove","StrategicState","BudgetState","BuildState","SpecificUnitCap","UnitModifier","FeedbackEntry"};
        string[] full = {
            "Side='Resource\\3' Personality='Resource\\3'",
            "Cap='-3' Difficulty='ALL -EASY'",
            "Delay='250ms' Difficulty='ALL -EASY'",
            "MinDelay='250ms' MaxDelay='250ms' MinTTKRatio='1.25' MaxTTKRatio='1.25' Difficulty='ALL -EASY'",
            "Shape='FLAT' StandardDeviation='1.25' Difficulty='ALL -EASY'",
            "MinEffectiveness='1.25' MaxEffectiveness='1.25' MinStandardDeviation='1.25' MaxStandardDeviation='1.25' MinEfficiency='1.25' MaxEfficiency='1.25' Difficulty='ALL -EASY'",
            "UseAllAvailableQueues='false' MinDelay='250ms' MaxDelay='250ms' MinEfficiency='1.25' MaxEfficiency='1.25' Difficulty='ALL -EASY'",
            "Percentage='12.5%' Difficulty='ALL -EASY'",
            "StrategicDirective='unknown' Event='Complete' FeedbackKey='Feedback.MiXeD'",
            "Event='InTrouble' FeedbackKey='Feedback.MiXeD'",
            "PlayerPower='Resource\\3' Weight='12.5%'",
            "Name='Resource\\3' Weight='12.5%' Difficulty='ALL -EASY'",
            "State='Resource\\3' Difficulty='ALL -EASY'",
            "State='Resource\\3' Difficulty='ALL -EASY'",
            "State='Resource\\3' Difficulty='ALL -EASY'",
            "Unit='MixedCaseUnit' Cap='-3' Difficulty='ALL -EASY'",
            "Unit='MixedCaseUnit' OffensiveModifier='1.25' DefensiveModifier='1.25' Difficulty='ALL -EASY'",
            "FeedbackKey='Feedback.MiXeD' EvaEvent='Resource\\3' Sound='Resource\\3' Text='Text &amp; value' Video='Text &amp; value' FeedbackTo='ALLIES -ENEMIES' Timeout='250ms'"
        };
        for(int i=0;i<names.Length;i++) {
            yield return Root(Item(names[i],""));
            yield return Root(Item(names[i],full[i]));
            // Attribute presence is significant for nullable resources/time.
            if(names[i]=="PowerChoice") {
                yield return Root("<PowerChoice PlayerPower=''/>");
                yield return Root("<PowerChoice PlayerPower='unresolved' Weight='0%'/>");
                yield return Root("<PowerChoice PlayerPower='Tech\\0' Weight='100%'/>");
            }
            if(names[i]=="FeedbackEntry") {
                yield return Root("<FeedbackEntry Timeout='' Text='' Video=''/>");
                yield return Root("<FeedbackEntry Timeout='-2s' Text='Abc' Video='VideoId' EvaEvent='Eva\\0' Sound='Audio\\9' FeedbackTo='-ALL'/>");
                yield return Root("<FeedbackEntry FeedbackKey='MiXeD'/><FeedbackEntry FeedbackKey='mixed'/>");
            }
            foreach(int count in new[]{27,28,29}) {
                var list=new StringBuilder();
                for(int n=0;n<count;n++) list.Append(Item(names[i],n%2==0?full[i]:""));
                yield return Root(list.ToString());
            }
        }
        yield return Root("<InTroubleSkirmishFeedbackControl/>");
        yield return Root("<InTroubleSkirmishFeedbackControl OnlyIfLocalPlayerIs='ALLIES -ENEMIES' TimeoutAfterPlaying='250ms' TimeBetweenScans='250ms' TimeThatConditionsMustBeMet='250ms' NumberOfObjectsRecentlyLost='-3' TimeForNumberOfObjectLost='250ms' ObjectLostFilter='Resource\\3' MinPowerAdvantageEnemiesInMyBaseVsAllMyUnits='1.25'/>");
        yield return Root("<InTroubleSkirmishFeedbackControl OnlyIfLocalPlayerIs='' TimeoutAfterPlaying='' TimeBetweenScans='0s' TimeThatConditionsMustBeMet='100ms' NumberOfObjectsRecentlyLost='-1' TimeForNumberOfObjectLost='2s' ObjectLostFilter='' MinPowerAdvantageEnemiesInMyBaseVsAllMyUnits='-0'/>");
        var combined=new StringBuilder();
        for(int i=0;i<names.Length;i++) combined.Append(Item(names[i],full[i]));
        combined.Append("<InTroubleSkirmishFeedbackControl OnlyIfLocalPlayerIs='ALLIES -ENEMIES' TimeoutAfterPlaying='250ms' TimeBetweenScans='250ms' TimeThatConditionsMustBeMet='250ms' NumberOfObjectsRecentlyLost='-3' TimeForNumberOfObjectLost='250ms' ObjectLostFilter='Resource\\3' MinPowerAdvantageEnemiesInMyBaseVsAllMyUnits='1.25'/>");
        combined.Append("<DefaultTacticHomeBaseCriteria/><CountAsStructureForBuildPurposesFilter/>");
        yield return Root(combined.ToString());
        foreach(string difficulty in new[]{"","EASY","MEDIUM HARD","ALL","all -EASY","-BRUTAL -BRUTAL","nonsense","+HARD -HARD"}) {
            yield return Root("<UnitCap Difficulty='"+difficulty+"'/><BuildDelay Difficulty='"+difficulty+"'/><OpeningMove Difficulty='"+difficulty+"'/><SpecificUnitCap Difficulty='"+difficulty+"'/>");
        }
        foreach(string eventName in new[]{"Begin","Cancel","Complete","unknown","begin",""}) {
            yield return Root("<StrategicDirectiveFeedback Event='"+eventName+"' FeedbackKey='Case.Sensitive'/>");
        }
        foreach(string eventName in new[]{"DefeatedByLocalPlayer","DefeatedLocalPlayer","FirstSeen","InTrouble","Lost","StartGame","unknown",""}) {
            yield return Root("<SkirmishGameEventFeedback Event='"+eventName+"' FeedbackKey='Case.Sensitive'/>");
        }
        foreach(string numeric in new[]{"0","-1","2147483648","0x10","010","1.25D2","junk"}) {
            yield return Root("<UnitCap Cap='"+numeric+"'/><SpecificUnitCap Cap='"+numeric+"'/><UnitModifier OffensiveModifier='"+numeric+"' DefensiveModifier='"+numeric+"'/><InTroubleSkirmishFeedbackControl NumberOfObjectsRecentlyLost='"+numeric+"'/>");
        }
    }
}
