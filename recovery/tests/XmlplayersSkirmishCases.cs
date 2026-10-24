using System;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
partial class XmlplayersOracle {
    static void SkirmishMatrix(IAssetBuilderPlugin o,TargetPlatform p){
        foreach(string type in new[]{"GameStatsModeData","SkirmishSettings"}){
            var d=Test(o,p,"skirmish-default-"+type,Root(type,"",""));Assert(d.InstanceData.Length==RootSize(type),"standalone skirmish root ABI");
            Reject(Root(type,"Bad='1'",""),p);Reject(Root(type,"xmlns:q='urn:other' q:TypeId='1'",""),p);Reject(Root(type,"","<Unknown/>"),p);Reject(Root(type,"","text"),p);
        }
        foreach(string mode in EnumNames("GameStatsMode"))Test(o,p,"standalone-game-mode-"+mode,Root("GameStatsModeData",A("GameStatsMode",mode),"<AvailableMapName>map</AvailableMapName>"));
        foreach(string mode in new[]{"","unknown","lan_game"," LAN_GAME","0","ALL"})Test(o,p,"standalone-game-mode-fallback-"+mode,Root("GameStatsModeData",A("GameStatsMode",mode),""));
        foreach(string value in Strings)Test(o,p,"standalone-map-string-"+value,Root("GameStatsModeData","GameStatsMode='ONLINE_CLAN_2V2_GAME'","<AvailableMapName>"+E(value)+"</AvailableMapName><AvailableMapName/>"));
        foreach(string value in Refs)Test(o,p,"skirmish-load-after-"+value,Root("SkirmishSettings",A("LoadAfter",value),"<HandicapBuildSpeed Speed='2.5'/>"));
        foreach(string value in Strings)Test(o,p,"skirmish-crate-hash-"+value,Root("SkirmishSettings",A("RandomCrateSpawner",value),""));
        foreach(string value in Numbers)Test(o,p,"skirmish-build-speed-"+value,Root("SkirmishSettings","","<HandicapBuildSpeed "+A("Speed",value)+"/>"));
        foreach(int count in new[]{0,1,2,3,27,28,29,64,257}){
            var maps=new StringBuilder();var speeds=new StringBuilder();for(int i=0;i<count;++i){maps.Append(i%2==0?"<AvailableMapName>abcd</AvailableMapName>":"<AvailableMapName/>");speeds.Append("<HandicapBuildSpeed Speed='17.125'/>");}
            Test(o,p,"standalone-map-list-"+count,Root("GameStatsModeData","GameStatsMode='LAN_GAME'",maps.ToString()));
            Test(o,p,"skirmish-speed-list-"+count,Root("SkirmishSettings","LoadAfter='Subsystem\\7' RandomCrateSpawner='Spawner'",speeds.ToString()));
        }
        foreach(string value in new[]{"é","资源","😀","a\0tail","\0leading","a\0b\0c"}){
            var d=Parse(Root("GameStatsModeData","GameStatsMode='LAN_GAME'","<AvailableMapName/>"));d.DocumentElement.FirstChild.InnerText=value;TestDoc(o,p,"standalone-map-ANSI-NUL",d);
            var s=Parse(Root("SkirmishSettings","",""));s.DocumentElement.SetAttribute("RandomCrateSpawner",value);TestDoc(o,p,"skirmish-crate-ANSI-NUL",s);
        }
        bool big=p!=TargetPlatform.Win32;
        var a=Test(o,p,"ABI-StandaloneGameStats",Root("GameStatsModeData","GameStatsMode='LAN_GAME'","<AvailableMapName>abcd</AvailableMapName><AvailableMapName/>"));
        Assert(a.InstanceData.Length==44&&Word(a.InstanceData,4,big)==2&&Word(a.InstanceData,8,big)==2&&Word(a.InstanceData,12,big)==16&&Word(a.InstanceData,20,big)==32&&Word(a.InstanceData,28,big)==40,"GameStats root/list/strings exact ABI");
        var b=Test(o,p,"ABI-SkirmishSettings",Root("SkirmishSettings","LoadAfter='Subsystem\\7' RandomCrateSpawner='Spawner'","<HandicapBuildSpeed Speed='2.5'/><HandicapBuildSpeed Speed='-3.25'/>"));
        Assert(b.InstanceData.Length==28&&Word(b.InstanceData,12,big)==2&&Word(b.InstanceData,16,big)==20&&Word(b.InstanceData,20,big)==0x40200000u&&Word(b.InstanceData,24,big)==0xc0500000u,"Skirmish build-speed contiguous array");Assert(b.ImportsData.Length==8&&Word(b.ImportsData,0,big)==4,"Skirmish LoadAfter imported reference");
        foreach(string xml in new[]{Root("GameStatsModeData","","<AvailableMapName Bad='1'>x</AvailableMapName>"),Root("GameStatsModeData","","<AvailableMapName><Nested/></AvailableMapName>"),Root("SkirmishSettings","","<HandicapBuildSpeed Bad='1'/>"),Root("SkirmishSettings","","<HandicapBuildSpeed><Nested/></HandicapBuildSpeed>"),Root("SkirmishSettings","","<HandicapBuildSpeed xmlns='urn:foreign'/>"),Root("SkirmishSettings","LoadAfter='Ref\\1' Unknown='1'","")})Reject(xml,p);
        foreach(string source in new[]{Root("GameStatsModeData","GameStatsMode='LAN_GAME'","<AvailableMapName>abcd</AvailableMapName>"),Root("SkirmishSettings","LoadAfter='Subsystem\\3'","<HandicapBuildSpeed Speed='2.5'/>")})foreach(string value in new[]{"0","4294967295","metadata",""}){var d=Parse(source);foreach(XmlElement n in d.SelectNodes("//*"))n.SetAttribute("TypeId",value);TestDoc(o,p,"skirmish-BAB-TypeId",d);}
    }
}
