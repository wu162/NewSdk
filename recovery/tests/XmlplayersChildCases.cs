using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
partial class XmlplayersOracle {
    static string FilterBody="<IncludeThing>ThingA</IncludeThing><IncludeThing>ThingB</IncludeThing><ExcludeThing>ThingC</ExcludeThing>";
    static string FilterContext(string context,string attrs,string children){
        string body="<"+context+" "+attrs+">"+children+"</"+context+">";
        if(context=="ResourceModifierObjectFilter")return Root("PlayerTemplate","",body);
        if(context=="BlockIfThingNearFactory")return Root("AIBuildStateDefinition","","<BuildEntry>"+body+"</BuildEntry>");
        return Root("UnitAISettings","",body);
    }
    static string PlayerCombined(){return Root("PlayerTemplate","DisplayNameToken='abcd' FactionIconImageName='icon' ResourceModifierValueList='1 2 3' LoadScreenMusic='Music\\9'",
        "<RedAlertTemplate>Alert</RedAlertTemplate><PreferredColor R='1' G='2' B='3' A='4'/><InitialUpgrade>Upg\\1</InitialUpgrade><InitialUpgrade>Upg\\2</InitialUpgrade><StartingUnitInfo><Object>Tank\\3</Object><Position x='1' y='2' z='3'/></StartingUnitInfo><ResourceModifierObjectFilter><IncludeThing>ThingA</IncludeThing><ExcludeThing>ThingB</ExcludeThing></ResourceModifierObjectFilter><StartingMoney Account='1' Amount='2'/><MoneyCap Account='3' Amount='4'/><AIBeaconSettings BeaconType='HoldObject' UnattachedTemplate='One' AttachedTemplate='Two'/><TechRank>Rank\\4</TechRank><TechProductionBonuses RequiredTech='Tech\\5' BuildCostBonus='10%'/>");}
    static void ChildMatrix(IAssetBuilderPlugin o,TargetPlatform p){
        foreach(string name in new[]{"InitialCreditsVeryLow","InitialCreditsLow","InitialCreditsMedium","InitialCreditsHigh","InitialCreditsVeryHigh","StartCountdownTimer","MaxBeaconsPerPlayer"})foreach(string value in Numbers)
            Test(o,p,"multiplayer-text-"+name+"-"+value,Root("MultiplayerSettings","","<"+name+">"+E(value)+"</"+name+">"));
        foreach(string name in new[]{"UseShroud","ShowRandomPlayerTemplate","ShowRandomStartPos","ShowRandomColor"})foreach(string value in Bools)
            Test(o,p,"multiplayer-bool-"+name+"-"+value,Root("MultiplayerSettings","","<"+name+">"+E(value)+"</"+name+">"));
        foreach(string value in Strings)Test(o,p,"map-string-"+value,Context("GameStatsModeData","id='InlineStats' GameStatsMode='LAN_GAME'","<AvailableMapName>"+E(value)+"</AvailableMapName><AvailableMapName/>"));
        foreach(string value in Strings)Test(o,p,"red-alert-"+value,Root("PlayerTemplate","","<RedAlertTemplate>"+E(value)+"</RedAlertTemplate>"));
        foreach(string color in new[]{"PreferredColor","PreferredCampaignRadarColor"})foreach(string channel in new[]{"R","G","B","A"})foreach(string value in Numbers)
            Test(o,p,"color-"+color+"-"+channel+"-"+value,Root("PlayerTemplate","","<"+color+" "+A(channel,value)+"/>"));
        foreach(string value in Refs)Test(o,p,"starting-unit-ref-"+value,Root("PlayerTemplate","","<StartingUnitInfo><Object>"+E(value)+"</Object><Position/></StartingUnitInfo>"));
        foreach(string axis in new[]{"x","y","z"})foreach(string value in Numbers)Test(o,p,"starting-position-"+axis+"-"+value,Root("PlayerTemplate","","<StartingUnitInfo><Position "+A(axis,value)+"/></StartingUnitInfo>"));
        foreach(string context in new[]{"VictoryConditionUnitObjectFilter","VictoryConditionStructureObjectFilter","ResourceModifierObjectFilter","BlockIfThingNearFactory"}){
            Test(o,p,"filter-empty-"+context,FilterContext(context,"",""));
            foreach(string attr in new[]{"id","Rule","Relationship","Alignment","Include","Exclude","StatusBitFlags","StatusBitFlagsExclude"})foreach(string value in new[]{"","ANY","ALL","unknown","INFANTRY VEHICLE STRUCTURE","STEALTHED UNDER_CONSTRUCTION","ALLIES SAME_PLAYER","GOOD EVIL","-ALL","ALL -ALL"})
                Test(o,p,"filter-"+context+"-"+attr+"-"+value,FilterContext(context,A(attr,value),FilterBody));
            foreach(int count in new[]{0,1,2,27,28,29,64,257}){
                StringBuilder items=new StringBuilder();for(int i=0;i<count;++i)items.Append("<IncludeThing>Thing").Append(i%11).Append("</IncludeThing><ExcludeThing>Other").Append(i%7).Append("</ExcludeThing>");
                Test(o,p,"filter-lists-"+context+"-"+count,FilterContext(context,"StatusBitFlags='STEALTHED' StatusBitFlagsExclude='UNDER_CONSTRUCTION'",items.ToString()));
            }
            Reject(FilterContext(context,"Bad='1'",""),p);Reject(FilterContext(context,"","<Unknown/>"),p);Reject(FilterContext(context,"","<IncludeThing Bad='1'>Thing</IncludeThing>"),p);Reject(FilterContext(context,"","<ExcludeThing><Nested/></ExcludeThing>"),p);
        }
        foreach(int count in new[]{0,1,2,3,27,28,29,64,257}){
            StringBuilder player=new StringBuilder(),stats=new StringBuilder(),build=new StringBuilder();
            for(int i=0;i<count;++i){
                player.Append("<InitialUpgrade>U\\1</InitialUpgrade><StartingUnitInfo><Object>T\\2</Object><Position x='1' y='2' z='3'/></StartingUnitInfo><StartingMoney Account='1' Amount='2'/><MoneyCap Account='3' Amount='4'/><AIBeaconSettings BeaconType='Assault' UnattachedTemplate='T' AttachedTemplate='U'/><TechRank>R\\3</TechRank><TechProductionBonuses RequiredTech='P\\4' HarvesterBonus='25%'/>");
                stats.Append("<GameStatsModeData GameStatsMode='ONLINE_CLAN_2V2_GAME'><AvailableMapName>abcd</AvailableMapName><AvailableMapName>longer-name</AvailableMapName></GameStatsModeData>");
                build.Append("<BuildEntry Build='Tank' Upgrade='Upgrade' Heuristic='BEST_UNIT_WEIGHTED'><BlockIfThingNearFactory Include='STRUCTURE'>").Append(FilterBody).Append("</BlockIfThingNearFactory></BuildEntry>");
            }
            Test(o,p,"player-all-lists-"+count,Root("PlayerTemplate","",player.ToString()));
            Test(o,p,"stats-nested-lists-"+count,Root("MultiplayerSettings","",stats.ToString()));
            Test(o,p,"build-list-"+count,Root("AIBuildStateDefinition","",build.ToString()));
        }
        foreach(string value in new[]{"é","资源","😀","a\0tail","\0leading","a\0b\0c"}){
            foreach(string attr in new[]{"LowLodTree","LowLodTreeNoGrab","LowLodTreeNoHarvest"}){var d=Parse(Root("AIData","",""));d.DocumentElement.SetAttribute(attr,value);TestDoc(o,p,"ANSI-AIData-"+attr,d);}
            foreach(string attr in new[]{"DisplayNameToken","FactionIconImageName"}){var d=Parse(Root("PlayerTemplate","",""));d.DocumentElement.SetAttribute(attr,value);TestDoc(o,p,"ANSI-player-"+attr,d);}
            {var d=Parse(Root("AIStrategicStateName","",""));d.DocumentElement.SetAttribute("Name",value);TestDoc(o,p,"ANSI-strategic",d);}
            {var d=Parse(Context("GameStatsModeData","","<AvailableMapName/>"));d.SelectSingleNode("//*[local-name()='AvailableMapName']").InnerText=value;TestDoc(o,p,"ANSI-map",d);}
        }
        foreach(string type in new[]{"AIData","UnitAISettings","PlayerTemplate","MultiplayerSettings","AIBuildStateDefinition","AIStrategicStateName","MultiplayerColor","AIPersonalityToAchievementEventMapping"}){
            Reject(Root(type,"","text"),p);Reject("<"+type+" xmlns='urn:foreign' id='Oracle'/>",p);
        }
        foreach(string body in new[]{"<InitialCreditsLow/><InitialCreditsLow/>","<UseShroud Bad='1'/>","<ShowRandomColor><Nested/></ShowRandomColor>"})Reject(Root("MultiplayerSettings","",body),p);
        foreach(string body in new[]{"<RedAlertTemplate/><RedAlertTemplate/>","<PreferredColor Bad='1'/>","<PreferredColor R='1'><Nested/></PreferredColor>","<StartingUnitInfo><Position/><Position/></StartingUnitInfo>","<StartingUnitInfo><Object Bad='1'>T</Object></StartingUnitInfo>","<ResourceModifierObjectFilter/><ResourceModifierObjectFilter/>"})Reject(Root("PlayerTemplate","",body),p);
        Reject(Root("UnitAISettings","","<VictoryConditionUnitObjectFilter/><VictoryConditionUnitObjectFilter/>"),p);
    }
    static void FixedLayouts(IAssetBuilderPlugin o,TargetPlatform p){
        bool big=p!=TargetPlatform.Win32;
        AssetBuffer ai=Test(o,p,"ABI-AIData",Root("AIData","",""));Assert(ai.InstanceData.Length==220&&Word(ai.InstanceData,44,big)==7&&Word(ai.InstanceData,48,big)==15,"AIData exact root/defaults");Assert(ai.InstanceData[207]==1&&ai.InstanceData[208]==1&&ai.InstanceData[216]==8,"AIData bool/byte packing");Assert(ai.InstanceData[217]==0&&ai.InstanceData[218]==0&&ai.InstanceData[219]==0,"AIData padding");
        AssetBuffer unit=Test(o,p,"ABI-UnitAI",Root("UnitAISettings","",""));Assert(unit.InstanceData.Length==448&&Word(unit.InstanceData,4,big)==90&&Word(unit.InstanceData,8,big)==400&&Word(unit.InstanceData,104,big)==2,"UnitAI defaults and enum");Assert(unit.InstanceData[444]==0&&unit.InstanceData[445]==1&&unit.InstanceData[446]==0&&unit.InstanceData[447]==0,"UnitAI packed flags/padding");
        AssetBuffer player=Test(o,p,"ABI-PlayerCombined",PlayerCombined());Assert(player.InstanceData.Length==424,"Player full allocation total");
        int[] offsets={12,20,80,112,128,136,140,148,156,164,172,180};int[] targets={188,196,204,216,220,228,244,364,372,380,392,396};for(int i=0;i<offsets.Length;++i)Assert(Word(player.InstanceData,offsets[i],big)==targets[i],"Player exact pointer/order "+offsets[i]);
        Assert(Word(player.InstanceData,116,big)==0x04010203u,"packed AARRGGBB color");Assert(Word(player.InstanceData,344,big)==356&&Word(player.InstanceData,352,big)==360,"filter inner list order");
        uint[] imports={24,220,224,228,392,396};Assert(player.ImportsData.Length==28,"Player imports exact count");for(int i=0;i<imports.Length;++i)Assert(Word(player.ImportsData,i*4,big)==imports[i],"Player import offset "+i);
        var stats=Test(o,p,"ABI-Multiplayer",Root("MultiplayerSettings","","<InitialCreditsVeryLow>-5</InitialCreditsVeryLow><UseShroud>true</UseShroud><GameStatsModeData GameStatsMode='LAN_GAME'><AvailableMapName>abcd</AvailableMapName></GameStatsModeData>"));
        Assert(stats.InstanceData.Length==76&&Word(stats.InstanceData,4,big)==unchecked((uint)-5)&&stats.InstanceData[40]==1,"Multiplayer root fields/size");Assert(Word(stats.InstanceData,32,big)==1&&Word(stats.InstanceData,36,big)==44&&Word(stats.InstanceData,56,big)==60&&Word(stats.InstanceData,64,big)==68,"Multiplayer nested list/string order");
        var name=Test(o,p,"ABI-StrategicName",Root("AIStrategicStateName","State='S\\7' Name='abcd'",""));Assert(name.InstanceData.Length==24&&Word(name.InstanceData,8,big)==4&&Word(name.InstanceData,12,big)==16,"StrategicName root/string order");Assert(name.ImportsData.Length==8&&Word(name.ImportsData,0,big)==4,"StrategicName imported State");
    }
    static void MetadataMatrix(IAssetBuilderPlugin o,TargetPlatform p){
        foreach(string type in new[]{"AIData","UnitAISettings","PlayerTemplate","MultiplayerSettings","AIBuildStateDefinition","AIStrategicStateName","MultiplayerColor","AIPersonalityToAchievementEventMapping"})foreach(string value in new[]{"0","4294967295","metadata",""}){
            var d=Parse(type=="PlayerTemplate"?PlayerCombined():type=="UnitAISettings"?FilterContext("VictoryConditionUnitObjectFilter","",FilterBody):Root(type,"",""));
            foreach(XmlElement n in d.SelectNodes("//*"))n.SetAttribute("TypeId",value);TestDoc(o,p,"BAB-metadata-"+type,d);
        }
    }
}
