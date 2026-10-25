using System;
using BinaryAssetBuilder.Core;
partial class XmlplayersOracle {
    static void AdditionalMatrix(IAssetBuilderPlugin o,TargetPlatform p){
        foreach(string name in new[]{"RGBColor","RGBColorT","RGBNightColor"})foreach(string channel in new[]{"R","G","B","A"})foreach(string value in Numbers)
            Test(o,p,"multiplayer-color-"+name+"-"+channel+"-"+value,Root("MultiplayerColor","","<"+name+" "+A(channel,value)+"/>"));
        foreach(string value in Strings)Test(o,p,"color-tooltip-"+value,Root("MultiplayerColor","","<TooltipName>"+E(value)+"</TooltipName>"));
        foreach(string value in Bools)Test(o,p,"color-available-"+value,Root("MultiplayerColor","","<AvailableInMetaGame>"+E(value)+"</AvailableInMetaGame>"));
        foreach(string value in new[]{"é","资源","😀","a\0tail","\0leading"}){var d=Parse(Root("MultiplayerColor","","<TooltipName/>"));d.DocumentElement.FirstChild.InnerText=value;TestDoc(o,p,"color-tooltip-ANSI",d);}
        var b=Test(o,p,"ABI-MultiplayerColor",Root("MultiplayerColor","","<RGBColor R='1' G='2' B='3' A='4'/><RGBColorT R='5' G='6' B='7' A='8'/><RGBNightColor R='9' G='10' B='11' A='12'/><TooltipName>abcd</TooltipName><AvailableInMetaGame>true</AvailableInMetaGame>"));bool big=p!=TargetPlatform.Win32;
        Assert(b.InstanceData.Length==36&&Word(b.InstanceData,4,big)==0x04010203u&&Word(b.InstanceData,8,big)==0x08050607u&&Word(b.InstanceData,12,big)==0x0c090a0bu,"three packed colors");
        Assert(Word(b.InstanceData,16,big)==4&&Word(b.InstanceData,20,big)==28&&b.InstanceData[24]==1,"MultiplayerColor string allocation/bool");Assert(b.InstanceData[25]==0&&b.InstanceData[26]==0&&b.InstanceData[27]==0,"MultiplayerColor padding");
        var m=Test(o,p,"ABI-PersonalityAchievement",Root("AIPersonalityToAchievementEventMapping","AIPersonalityDef='P\\17' AchievementEventId='0xFEDCBA98'",""));Assert(m.InstanceData.Length==12&&Word(m.InstanceData,8,big)==0xfedcba98u,"achievement unsigned field");Assert(m.ImportsData.Length==8&&Word(m.ImportsData,0,big)==4,"achievement personality import");
        foreach(string child in new[]{"<RGBColor/><RGBColor/>","<RGBColor Bad='1'/>","<TooltipName Bad='1'>text</TooltipName>","<TooltipName><Nested/></TooltipName>","<AvailableInMetaGame Bad='1'/>","<AvailableInMetaGame/><AvailableInMetaGame/>","<RGBColor xmlns='urn:other'/>","<Unknown/>"})Reject(Root("MultiplayerColor","",child),p);
    }
}
