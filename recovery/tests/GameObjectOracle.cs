using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using BinaryAssetBuilder.Core;
partial class GameObjectOracle {
    class Field {public string Name,Kind;public Field(string n,string k){Name=n;Kind=k;}}
    static int fixtures,comparisons,refusals,invarianceComparisons;
    static InstanceDeclaration Declaration(string attrs,string children) {
        var xml=new XmlDocument();xml.LoadXml("<GameObject xmlns='uri:ea.com:eala:asset' id='RecoveryRoot' "+attrs+">"+children+"</GameObject>");
        var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea","uri:ea.com:eala:asset");
        var doc=new AssetDeclarationDocument();var st=typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic);
        var state=FormatterServices.GetUninitializedObject(st);st.GetField("NamespaceManager").SetValue(state,ns);
        typeof(AssetDeclarationDocument).GetField("_Current",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(doc,state);
        var d=new InstanceDeclaration(doc);d.XmlNode=xml.DocumentElement;return d;
    }
    static uint Word(byte[] b,int at,bool big) {return big?((uint)b[at]<<24)|((uint)b[at+1]<<16)|((uint)b[at+2]<<8)|b[at+3]:BitConverter.ToUInt32(b,at);}
    static void Equal(byte[] a,byte[] b,string label,bool bufferComparison=true) {if(bufferComparison)++comparisons;else ++invarianceComparisons;if(a.Length!=b.Length)throw new Exception(label+" length "+a.Length+" != "+b.Length);for(int i=0;i<a.Length;++i)if(a[i]!=b[i])throw new Exception(label+" byte "+i+": "+a[i]+" != "+b[i]);}
    static AssetBuffer Check(IAssetBuilderPlugin original,TargetPlatform platform,string attrs,string children="") {
        var input=Declaration(attrs,children);var expected=original.ProcessInstance(input);var actual=Recovered.GameObjectCompiler.ProcessInstance(input,platform);
        Equal(expected.InstanceData,actual.InstanceData,attrs+children+" instance");Equal(expected.RelocationData,actual.RelocationData,attrs+children+" relocation");Equal(expected.ImportsData,actual.ImportsData,attrs+children+" imports");++fixtures;return actual;
    }
    static void Reject(TargetPlatform p,string attrs,string children) {
        try {Recovered.GameObjectCompiler.ProcessInstance(Declaration(attrs,children),p);}catch(NotSupportedException){++refusals;return;}
        throw new Exception("Unsupported GameObject accepted: "+attrs+children);
    }
    static string Escape(string value) {return value.Replace("&","&amp;").Replace("'","&apos;").Replace("<","&lt;").Replace("\"","&quot;");}
    static void Layout(AssetBuffer b,bool big) {
        int root=checked((int)Word(b.InstanceData,8,big))-4;
        if(root!=1620 || b.InstanceData.Length!=2212 || b.RelocationData.Length!=0 || b.ImportsData.Length!=0)throw new Exception("Independent default layout");
        // Independent explicit checks, deliberately outside generated metadata.
        int[] negative={80,304,348,352};foreach(int off in negative)if(Word(b.InstanceData,root+off,big)!=0xffffffffu)throw new Exception("Default enum "+off);
        if(Word(b.InstanceData,root+388,big)!=155u)throw new Exception("Duration default frame conversion");
        if(Word(b.InstanceData,root+188,big)!=0x3f800000u || Word(b.InstanceData,root+184,big)!=0x44160000u)throw new Exception("Default float");
        int[] offsets={40,412,420,444,472,536,560,568};
        for(int i=0;i<8;++i)if(Word(b.InstanceData,1524+12*i,big)!=1820487795u || Word(b.InstanceData,1528+12*i,big)!=1564774360u || Word(b.InstanceData,1532+12*i,big)!=root+offsets[i])throw new Exception("Independent empty-list count table");
        for(int i=1512;i<1524;++i)if(b.InstanceData[i]!=0)throw new Exception("Token sentinel");
    }
    static void IndependentBindings(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        string[] names={"FenceWidth","FenceXOffset","RemoveTerrainRadius","EmotionRange","FactoryExitWidth","FactoryExtraBibWidth","BountyValue","BuildTime","EnergyProduction","EnergyBonus","VoicePriority","Scale","HealthBoxHeightOffset","LiveCameraFXPitch","FormationWidth","CampnessValueRadius"};
        int[] offsets={84,88,92,96,108,112,132,136,140,144,168,188,192,196,200,184};
        bool[] signed={false,false,false,false,false,false,true,false,true,true,true,false,false,false,true,false};
        for(int i=0;i<names.Length;++i) {
            var b=Check(original,p,names[i]+"='-17'");int root=(int)Word(b.InstanceData,8,big)-4;
            uint expected=signed[i]?unchecked((uint)-17):0xc1880000u;
            if(Word(b.InstanceData,root+offsets[i],big)!=expected)throw new Exception("Independent scalar offset: "+names[i]);
        }
        var text=Check(original,p,"Description='abcd'");
        if(text.InstanceData.Length!=2220 || Word(text.InstanceData,1620+48,big)!=4 || Word(text.InstanceData,1620+52,big)!=2212 || text.RelocationData.Length!=8 || Word(text.RelocationData,0,big)!=1672 || Word(text.RelocationData,4,big)!=0xffffffffu)throw new Exception("Independent string relocation/length/padding");
        for(int i=0;i<8;++i)if(text.InstanceData[2212+i]!=(i<4?97+i:0))throw new Exception("Independent string bytes/padding");
        var refs=Check(original,p,"CrowdResponse='A\\23' UnitTypeIcon='B\\0' UnitIntro='C\\-1'");
        if(Word(refs.InstanceData,1620+176,big)!=24 || Word(refs.InstanceData,1620+360,big)!=1 || Word(refs.InstanceData,1620+392,big)!=0 || refs.ImportsData.Length!=0)throw new Exception("Independent tokenized resource indices/import output");
        var narrow=Check(original,p,"RefundValue='0x1234' MaxSimultaneousOfType='0x5678' TransportSlotCount='0xab' IsTrainable='false' IsForbidden='true'");
        int a=1620+576;
        byte[] expectedNarrow=big?new byte[]{0x12,0x34,0x56,0x78,0xab,0,1}:new byte[]{0x34,0x12,0x78,0x56,0xab,0,1};
        for(int i=0;i<expectedNarrow.Length;++i)if(narrow.InstanceData[a+i]!=expectedNarrow[i])throw new Exception("Independent packed short/byte/bool layout");
    }
    static void BrowserLayout(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        var b=Check(original,p,"Browser='a bb' Description='abcd'");
        int root=(int)Word(b.InstanceData,8,big)-4;
        // Two item tokens shift every subsequent list table and the native root.
        if(root!=1644 || Word(b.InstanceData,root+40,big)!=2 || Word(b.InstanceData,root+44,big)!=root+592)throw new Exception("Independent Browser root/list binding");
        int elements=root+592;
        if(Word(b.InstanceData,elements,big)!=1 || Word(b.InstanceData,elements+8,big)!=2 || Word(b.InstanceData,elements+4,big)!=elements+16 || Word(b.InstanceData,elements+12,big)!=elements+20 || Word(b.InstanceData,root+52,big)!=elements+24)throw new Exception("Independent Browser allocation order");
        for(int i=0;i<2;++i)if(Word(b.InstanceData,1536+12*i,big)!=2974916843u || Word(b.InstanceData,1540+12*i,big)!=0 || Word(b.InstanceData,1544+12*i,big)!=elements+8*i)throw new Exception("Independent Browser item tokens");
        Check(original,p,"Browser='a&#9;b&#10;c&#13;d'");
    }
    static void Enums(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        int[] offsets={80,128,304,308,348,352};uint[] valid={0xfffffffeu,2,6,3,0xffffffffu,4};uint[] fallback={2,8,4,4,3,1};
        var known=Check(original,p,"RadarPriority='NOT_ON_RADAR' EditorSorting='UNIT' ProductionQueueType='WATERCRAFT' BuildPlacementTypeFlag='TIBERIUM_FIELD' UnitCategory='NONE' WeaponCategory='LIGHTNING'");
        var unknown=Check(original,p,"RadarPriority='UNKNOWN' EditorSorting='UNKNOWN' ProductionQueueType='UNKNOWN' BuildPlacementTypeFlag='UNKNOWN' UnitCategory='UNKNOWN' WeaponCategory='UNKNOWN'");
        for(int i=0;i<offsets.Length;++i)if(Word(known.InstanceData,1620+offsets[i],big)!=valid[i] || Word(unknown.InstanceData,1620+offsets[i],big)!=fallback[i])throw new Exception("Independent enum value/fallback "+offsets[i]);
        foreach(string value in new[]{"COMMANDO","INVALID","NOT_ON_RADAR","STRUCTURE","UNIT","commando"," COMMANDO","COMMANDO ","","-1","0","UNKNOWN"}) Check(original,p,"RadarPriority='"+Escape(value)+"'" );
        foreach(string value in new[]{"AUDIO","CAMPAIGN_STRUCTURES","CAMPAIGN_UNITS","DEBRIS","DESTRUCTIBLE_PROPS","EMITTERS","FOR_REVIEW","MISC_MAN_MADE","MISC_NATURAL","NONE","OBSOLETE","OPTIMIZED_PROPS","ROAD","SCORCH","SHRUBBERY","STRUCTURE","SYSTEM","TEST","UNIT","WAYPOINT","audio"," AUDIO","AUDIO ","","-1","0","UNKNOWN"}) Check(original,p,"EditorSorting='"+Escape(value)+"'" );
        foreach(string value in new[]{"AIRCRAFT","INFANTRY","INVALID","MAIN_STRUCTURE","OTHER_STRUCTURE","UPGRADE","VEHICLE","WATERCRAFT","aircraft"," AIRCRAFT","AIRCRAFT ","","-1","0","UNKNOWN"}) Check(original,p,"ProductionQueueType='"+Escape(value)+"'" );
        foreach(string value in new[]{"BLOCKED","INVALID","MAIN_STRUCTURE","OTHER_STRUCTURE","TIBERIUM_FIELD","blocked"," BLOCKED","BLOCKED ","","-1","0","UNKNOWN"}) Check(original,p,"BuildPlacementTypeFlag='"+Escape(value)+"'" );
        foreach(string value in new[]{"AIRCRAFT","INFANTRY","NONE","STRUCTURE","VEHICLE","aircraft"," AIRCRAFT","AIRCRAFT ","","-1","0","UNKNOWN"}) Check(original,p,"UnitCategory='"+Escape(value)+"'" );
        foreach(string value in new[]{"BLAST","CANNON","GUN","LIGHTNING","MISSILE","NONE","blast"," BLAST","BLAST ","","-1","0","UNKNOWN"}) Check(original,p,"WeaponCategory='"+Escape(value)+"'" );
    }
    static void ChildrenAndDuration(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        const string duration="EvaEventSecondDamageFarFromFirstTimeoutMS";
        foreach(string value in new[]{"", "text", "0", "-0", "0.000001", "199.99998", "199.99999", "200", "200.00001", "200.00002", "201", "399.99997", "400", "400.00003", "31000", "-0.0001", "-199.99998", "-200", "-200.00002", "-201", "-400", "1.25e4", "2D3", "0x10", "nan", "inf", "1e40", "-1e40", "1e1000", "-1e1000", "4294967295", "4294967296", "858993459100", "858993459200", "858993459300", "-858993459200", "1.8446744e21", "1.8446745e21", "-1.8446744e21", "-1.8446745e21", "1e-50", "  +23tail"})
            Check(original,p,duration+"='"+Escape(value)+"'");
        foreach(string value in new[]{"0","1","199.99998","200","200.00002","200.00004","-200","-201","1e40","-1e40"}) {
            uint expected=value=="0"||value=="1e40"||value=="-1e40"?0u:value=="200.00004"?2u:value=="-200"||value=="-201"?0xffffffffu:1u;
            var b=Check(original,p,duration+"='"+value+"'");
            if(Word(b.InstanceData,1620+388,big)!=expected)throw new Exception("Independent duration "+value);
        }
        string[] buildable={"IGNORE_PREREQUISITES","NO","ONLY_BY_AI","YES"};
        string[] thing={"BUFF","BUSH","CAVALRY_UNIT","CHARACTER_UNIT","CIV_BUILDING","FACTION_BUILDING","GROUND_COVER","HORDE_UNIT","LANDMARK_BUILDING","LARGE_MONSTER","MACHINE","MEDIUM_MONSTER","PROP","TREE","UNSPECIFIED","WALL_PIECE"};
        foreach(string name in new[]{"Buildable","ThingClass"}) {
            foreach(string value in name=="Buildable"?buildable:thing)Check(original,p,"","<"+name+">"+value+"</"+name+">");
            foreach(string value in new[]{"", "UNKNOWN", "yes", " YES", "YES ", "0", "-1", " BUFF", "buff"})Check(original,p,"","<"+name+">"+value+"</"+name+">");
        }
        foreach(string color in new[]{"", "R='1'", "G='2'", "B='3'", "A='4'", "R='17' G='34' B='51' A='68'", "R='-1' G='256' B='0x123' A='010'", "R='4294967296' G='-4294967296' B='text' A=''"})Check(original,p,"","<DisplayColor "+color+"/>");
        var inline=Check(original,p,"","<ThingClass>BUFF</ThingClass><DisplayColor R='17' G='34' B='51' A='68'/><Buildable>NO</Buildable>");
        if(inline.InstanceData.Length!=2212 || inline.RelocationData.Length!=0 || Word(inline.InstanceData,1620+428,big)!=2 || Word(inline.InstanceData,1620+432,big)!=15 || Word(inline.InstanceData,1620+452,big)!=0x44112233u)throw new Exception("Independent inline child layout");
        var emptyColor=Check(original,p,"","<DisplayColor/>");
        if(Word(emptyColor.InstanceData,1620+452,big)!=0xff000000u)throw new Exception("Color omitted channel defaults");
        foreach(string name in new[]{"DisplayName","DisplayNameTransformed"})foreach(string value in new[]{"", "a", "abcd", "abcde", "with spaces", "é ÿ 中文", "A&amp;B"})Check(original,p,"","<"+name+">"+value+"</"+name+">");
        string names="<DisplayName>abcd</DisplayName><DisplayNameTransformed>x</DisplayNameTransformed>";
        var strings=Check(original,p,"",names);
        if(Word(strings.InstanceData,1620+396,big)!=2212 || Word(strings.InstanceData,1620+400,big)!=2228 || Word(strings.InstanceData,2212,big)!=4 || Word(strings.InstanceData,2216,big)!=2220 || Word(strings.InstanceData,2228,big)!=1 || Word(strings.InstanceData,2232,big)!=2236 || strings.InstanceData.Length!=2240)throw new Exception("Independent optional string allocation");
        var reversed=Check(original,p,"","<DisplayNameTransformed>x</DisplayNameTransformed><DisplayName>abcd</DisplayName>");
        Equal(strings.InstanceData,reversed.InstanceData,"XML order must not change native allocation order",false);
        Check(original,p,"Browser='a bb' Description='desc' KindOf='INFANTRY' CrowdResponse='A\\23' "+duration+"='201'",names+"<Buildable>ONLY_BY_AI</Buildable><ThingClass>MACHINE</ThingClass><DisplayColor R='10' G='20' B='30'/>");
        Check(original,p,"","<DisplayName TypeId='1'>name</DisplayName><Buildable TypeId='2'>YES</Buildable><DisplayColor TypeId='3' R='1'/>");
        foreach(string child in new[]{"<Buildable/><Buildable/>","<ThingClass/><ThingClass/>","<DisplayName/><DisplayName/>","<DisplayColor/><DisplayColor/>","<Buildable R='1'/>","<ThingClass Unknown='1'/>","<DisplayColor X='1'/>","<DisplayColor R='1' xmlns:f='foreign' f:G='2'/>","<DisplayName xmlns='foreign'/>","<DisplayName><Unknown/></DisplayName>","<DisplayColor>123</DisplayColor>"})Reject(p,"",child);
    }
    static void LeafCollections(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        string[] values={"", "a", "AB", "abc", "abcd", "abcde", "MiXeD", "with spaces", "é ÿ 中文", "-1", "4294967296", "A&amp;B"};
        foreach(string name in new[]{"EquivalentTo","UpgradeCameo","DisplayUpgrade"}) {
            foreach(string value in values)Check(original,p,"","<"+name+">"+value+"</"+name+">");
            Check(original,p,"","<"+name+">a</"+name+"><"+name+">bb</"+name+">");
            Check(original,p,"","<"+name+" TypeId='123'>a</"+name+"><"+name+"/>");
            if(name!="UpgradeCameo") {
                var lower=Check(original,p,"","<"+name+">mixed</"+name+">");
                var upper=Check(original,p,"","<"+name+">MIXED</"+name+">");
                Equal(lower.InstanceData,upper.InstanceData,"Case-insensitive typed list IDs",false);
            }
        }
        string cameo="";for(int i=0;i<5;++i)cameo+="<UpgradeCameo>item"+i+"</UpgradeCameo>";Check(original,p,"",cameo);Reject(p,"",cameo+"<UpgradeCameo/>");
        Reject(p,"","<DisplayUpgrade/><DisplayUpgrade/><DisplayUpgrade/>");
        foreach(string name in new[]{"Fuel","MaxBurnRate","Decay","Resistance"})foreach(string value in new[]{"0","1","-1","255","256","0x12345678","010","2147483647","4294967295","4294967296","-4294967296"," 23tail","text",""})Check(original,p,"","<Flammability "+name+"='"+value+"'/>");
        foreach(string axis in new[]{"x","y","z"})foreach(string value in new[]{"0","1","-17","1.25","2D3","1e40","-1e40","1e-50","text",""})Check(original,p,"","<LiveCameraOffset "+axis+"='"+value+"'/>");
        var flame=Check(original,p,"","<Flammability Fuel='1' MaxBurnRate='2' Decay='3' Resistance='4'/>");
        int root=(int)Word(flame.InstanceData,8,big)-4;
        if(root!=1680 || flame.InstanceData.Length!=2288 || Word(flame.InstanceData,root+456,big)!=2272 || Word(flame.InstanceData,1208,big)!=1572)throw new Exception("Independent Flammability root/pointer/token layout");
        uint[] tokenNames={2194468989u,1555868979u,1413518551u,2097559226u};
        for(int i=0;i<4;++i)if(Word(flame.InstanceData,2272+4*i,big)!=(uint)i+1 || Word(flame.InstanceData,1572+12*i,big)!=tokenNames[i] || Word(flame.InstanceData,1576+12*i,big)!=1982953380u || Word(flame.InstanceData,1580+12*i,big)!=2272+4*i)throw new Exception("Independent Flammability data/token members");
        for(int i=1620;i<1632;++i)if(flame.InstanceData[i]!=0)throw new Exception("Flammability token sentinel");
        var emptyFlame=Check(original,p,"","<Flammability/>");for(int i=0;i<4;++i)if(Word(emptyFlame.InstanceData,2272+4*i,big)!=0)throw new Exception("Flammability zero defaults");
        var camera=Check(original,p,"","<LiveCameraOffset x='1' y='-2' z='0.5'/>");
        if(camera.InstanceData.Length!=2224 || Word(camera.InstanceData,1620+520,big)!=2212 || Word(camera.InstanceData,1388,big)!=2212 || Word(camera.InstanceData,2212,big)!=0x3f800000u || Word(camera.InstanceData,2216,big)!=0xc0000000u || Word(camera.InstanceData,2220,big)!=0x3f000000u)throw new Exception("Independent Vector3 allocation/token/layout");
        var emptyCamera=Check(original,p,"","<LiveCameraOffset/>");for(int i=0;i<3;++i)if(Word(emptyCamera.InstanceData,2212+4*i,big)!=0)throw new Exception("Vector3 zero defaults");
        string lists="<EquivalentTo>a</EquivalentTo><EquivalentTo>bb</EquivalentTo><UpgradeCameo>a</UpgradeCameo><UpgradeCameo>bb</UpgradeCameo><DisplayUpgrade>a</DisplayUpgrade><DisplayUpgrade>bb</DisplayUpgrade>";
        var combined=Check(original,p,"",lists);root=(int)Word(combined.InstanceData,8,big)-4;
        if(root!=1692 || combined.InstanceData.Length!=2324 || Word(combined.InstanceData,root+444,big)!=2 || Word(combined.InstanceData,root+448,big)!=2284 || Word(combined.InstanceData,root+536,big)!=2 || Word(combined.InstanceData,root+540,big)!=2292 || Word(combined.InstanceData,root+568,big)!=2 || Word(combined.InstanceData,root+572,big)!=2316)throw new Exception("Independent collection counts/pointers/allocation");
        if(Word(combined.InstanceData,2292,big)!=1 || Word(combined.InstanceData,2296,big)!=2308 || Word(combined.InstanceData,2300,big)!=2 || Word(combined.InstanceData,2304,big)!=2312)throw new Exception("Independent collection string entries");
        int[] fieldTokens={98,119,125};int[] offsets={444,536,568};int[] elements={2284,2292,2316};int[] strides={4,8,4};
        for(int i=0;i<3;++i) {
            int t=(int)Word(combined.InstanceData,12*fieldTokens[i]+8,big);
            if(Word(combined.InstanceData,t,big)!=1820487795u || Word(combined.InstanceData,t+4,big)!=1564774360u || Word(combined.InstanceData,t+8,big)!=root+offsets[i])throw new Exception("Independent collection count token");
            for(int j=0;j<2;++j)if(Word(combined.InstanceData,t+12+12*j,big)!=2974916843u || Word(combined.InstanceData,t+16+12*j,big)!=0 || Word(combined.InstanceData,t+20+12*j,big)!=elements[i]+strides[i]*j)throw new Exception("Independent collection element token");
        }
        string extra="<Flammability Fuel='17'/><LiveCameraOffset y='-3'/><DisplayName>name</DisplayName><DisplayColor R='255'/><Buildable>NO</Buildable>";
        Check(original,p,"Browser='one two' Description='text' EvaEventSecondDamageFarFromFirstTimeoutMS='200.00004'",lists+extra);
        var canonical=Check(original,p,"",lists+extra);var reversed=Check(original,p,"",extra+lists);Equal(canonical.InstanceData,reversed.InstanceData,"Native allocation independent of child group order",false);
        foreach(string child in new[]{"<EquivalentTo Unknown='1'/>","<UpgradeCameo xmlns='foreign'/>","<DisplayUpgrade><Unknown/></DisplayUpgrade>","<Flammability/><Flammability/>","<LiveCameraOffset/><LiveCameraOffset/>","<Flammability fuel='1'/>","<LiveCameraOffset X='1'/>","<Flammability>text</Flammability>","<LiveCameraOffset xmlns:f='foreign' f:x='1'/>"})Reject(p,"",child);
    }
    static int Main(string[] args) {
        try {
            var original=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(args[0]).GetType("BinaryAssetBuilder.Tokenizer.Plugin"));
            foreach(TargetPlatform p in new[]{TargetPlatform.Win32,TargetPlatform.Xbox360,TargetPlatform.PlayStation3,(TargetPlatform)99}) {
                original.Initialize(null,p);Layout(Check(original,p,""),p!=TargetPlatform.Win32);IndependentBindings(original,p);Enums(original,p);BrowserLayout(original,p);KindOf(original,p);ChildrenAndDuration(original,p);LeafCollections(original,p);
                string mixed="";
                foreach(var f in Fields) {
                    string[] values=f.Kind=="StringList"?new[]{""," ","a","ab cd"," a  b ","a,b;c","a\tb\rc\nd","é ÿ 中文","one two three four five six seven eight nine ten","a a a a","a  bb ccc dddd eeeee"}:
                        f.Kind=="AssetId"?new[]{"","a","AB","abc","abcd","abcde","ABCDEF","AbCdEfG","MiXeDCaSe","with spaces","Asset:Name","1","_Test-Name","é","ÿ","中文"}:
                        f.Kind=="Reference"?new[]{"","Unresolved","Name\\0","Name\\1","Name\\23","Name\\-1","Name\\2147483647","Name\\2147483648","Name\\-2147483648","Name\\-2147483649","Name\\+17","Name\\23trailing","Name\\0x10","Name\\ 23","Name\\text"}:
                        f.Kind=="String"?new[]{"","x","abc","abcd","abcde","long value with spaces","A&amp;B","中文测试"}:
                        f.Kind=="Boolean"?new[]{"true","false","TRUE","False","1","0","t","yes"," true",""}:
                        f.Kind=="Time"?new[]{"0s","-17s","23ms","2d1s","1000ms","1.25s","5",""}:
                        f.Kind=="Angle"?new[]{"0r","180d","-90d","1.25r","2d1","","text"}:
                        new[]{"-17","0","23","-1","255","256","65535","65536","0x10","010","1.25","2d1"," 23tail","text","","2147483647","-2147483648","4294967295","4294967296"};
                    foreach(string value in values)Check(original,p,f.Name+"='"+Escape(value)+"'");
                    mixed+=f.Name+"='"+(f.Kind=="String"?"mixed-string":f.Kind=="Boolean"?"true":"23")+"' ";
                }
                Check(original,p,mixed);Check(original,p,"TypeId='2486173485' FenceWidth='23'");
                foreach(string a in new[]{"Unknown='1'","foreign:FenceWidth='1' xmlns:foreign='foreign'"})Reject(p,a,"");
                foreach(string child in new[]{"<Geometry/>","<ArmorSet/>","<ReplaceModule/>","<InheritableModule/>","<Foreign xmlns='other'/>","<Unknown/>"})Reject(p,"",child);
            }
            Console.WriteLine("PASS recovered GameObject root: fixtures="+fixtures+" threeBufferComparisons="+comparisons+" refusals="+refusals+" invarianceComparisons="+invarianceComparisons+"; 88 root attributes and ten child nodes, four targets including 99, independent defaults/list/sentinel checks; SDK not run");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}

