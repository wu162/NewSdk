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
        var input=Declaration(attrs,children);AssetBuffer expected,actual;try {expected=original.ProcessInstance(input);actual=Recovered.GameObjectCompiler.ProcessInstance(input,platform);}catch(Exception error){throw new Exception("Fixture "+attrs+children,error);}
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
        // Adjacent representable float values around frame boundaries and powers
        // of two exercise both required single-precision rounding points.
        for(int frame=-32;frame<=32;++frame) {
            float center=frame*200.0f;
            int bits=BitConverter.ToInt32(BitConverter.GetBytes(center),0);
            for(int delta=-2;delta<=2;++delta) {
                float value=BitConverter.ToSingle(BitConverter.GetBytes(bits+delta),0);
                if(!float.IsNaN(value) && !float.IsInfinity(value))Check(original,p,duration+"='"+value.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+"'");
            }
        }
        for(int exponent=-126;exponent<=127;++exponent)foreach(int sign in new[]{-1,1}) {
            float center=(float)Math.Pow(2,exponent)*sign;
            int bits=BitConverter.ToInt32(BitConverter.GetBytes(center),0);
            for(int delta=-1;delta<=1;++delta) {
                float value=BitConverter.ToSingle(BitConverter.GetBytes(bits+delta),0);
                if(!float.IsNaN(value) && !float.IsInfinity(value))Check(original,p,duration+"='"+value.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+"'");
            }
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
    static void ArmorAndLocomotorSets(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        string[] armorNames={"INVALID","VETERAN","ELITE","HERO","PLAYER_UPGRADE","WEAK_VERSUS_BASEDEFENSES","ALTERNATE_FORMATION","MOUNTED","PLAYER_UPGRADE_2","PLAYER_UPGRADE_3","UNBESIEGEABLE","AS_TOWER","AIRBORNE","SECONDARY_DAMAGE","SHRINK_EFFECT","IN_SPIDER_HOLE","UNUSED_05","UNUSED_06","UNUSED_07","UNUSED_08","UNUSED_09","UNUSED_10","SHIELDBODY_ENABLED"};
        for(int i=0;i<armorNames.Length;++i)foreach(string sign in new[]{"","+","-"}) {
            var b=Check(original,p,"","<ArmorSet Conditions='"+sign+armorNames[i]+"'/>");
            int root=(int)Word(b.InstanceData,8,big)-4;int data=(int)Word(b.InstanceData,root+416,big);
            if(Word(b.InstanceData,data+8,big)!=(1u<<i))throw new Exception("Independent ArmorSet bit index "+armorNames[i]+sign);
        }
        foreach(string value in new[]{"", "ALL", "+ALL", "-ALL", "all", "ALL -VETERAN", "VETERAN ALL -VETERAN", "VETERAN -VETERAN", "-VETERAN -VETERAN", "VETERAN VETERAN", "UNKNOWN", "unknown", "veteran", "+UNKNOWN", "-UNKNOWN", "VETERAN,ELITE", " VETERAN  ELITE ", "+", "-"}) {
            var b=Check(original,p,"","<ArmorSet Conditions='"+value+"'/>");
            int root=(int)Word(b.InstanceData,8,big)-4;int data=(int)Word(b.InstanceData,root+416,big);
            if(value.IndexOf("ALL",StringComparison.OrdinalIgnoreCase)>=0 && Word(b.InstanceData,data+8,big)!=0xffffffffu)throw new Exception("ArmorSet ALL padding/stop behavior");
            if(value=="UNKNOWN" && Word(b.InstanceData,data+8,big)!=(1u<<12))throw new Exception("ArmorSet unknown fallback bit");
        }
        string[] locoNames={"NORMAL","NORMAL_UPGRADED","FREEFALL","WANDER","PANIC","TAXIING","SUPERSONIC","MOUNTED","ENRAGED","SCARED","CONTAINED","COMBO","ATTACKING","MOVING_TO_TRANSPORT","WALL_SCALING","CHANGING_FIRINGARC","BURNINGDEATH","EXITING_PRODUCTION_STRUCTURE"};
        for(int i=0;i<locoNames.Length;++i) {
            var b=Check(original,p,"","<LocomotorSet Condition='"+locoNames[i]+"'/>");int root=(int)Word(b.InstanceData,8,big)-4;int data=(int)Word(b.InstanceData,root+424,big);
            if(Word(b.InstanceData,data+4,big)!=(uint)i)throw new Exception("Independent LocomotorSet enum "+locoNames[i]);
        }
        foreach(string value in new[]{"INVALID", "", "UNKNOWN", "normal", " NORMAL", "NORMAL ", "-1", "0"})Check(original,p,"","<LocomotorSet Condition='"+value+"'/>");
        foreach(string value in new[]{"0","-17","23","1.25","2D3","1e40","-1e40","text",""})Check(original,p,"","<LocomotorSet Speed='"+value+"'/>");
        foreach(string value in new[]{"", "Unresolved", "A\\0", "A\\1", "A\\-1", "A\\2147483647", "A\\2147483648", "A\\-2147483649", "A\\23trailing", "A\\0x10"}) {
            foreach(string field in new[]{"Armor","DamageFX"})Check(original,p,"","<ArmorSet "+field+"='"+value+"'/>");
            Check(original,p,"","<LocomotorSet Locomotor='"+value+"'/>");
        }
        var emptyArmor=Check(original,p,"","<ArmorSet/>");var emptyLoco=Check(original,p,"","<LocomotorSet/>");
        foreach(var b in new[]{emptyArmor,emptyLoco}) {
            int root=(int)Word(b.InstanceData,8,big)-4;
            if(root!=1680 || b.InstanceData.Length!=2284)throw new Exception("Independent set allocation size");
            for(int i=0;i<3;++i)if(Word(b.InstanceData,2272+4*i,big)!=0)throw new Exception("Independent omitted set fields");
        }
        var explicitEmpty=Check(original,p,"","<LocomotorSet Condition=''/>");if(Word(explicitEmpty.InstanceData,2276,big)!=12)throw new Exception("Locomotor missing vs empty enum");
        var invalid=Check(original,p,"","<LocomotorSet Condition='INVALID'/>");if(Word(invalid.InstanceData,2276,big)!=0xffffffffu)throw new Exception("Locomotor INVALID enum");
        string pair="<ArmorSet Armor='A\\23' DamageFX='B\\-1' Conditions='VETERAN ELITE'/><LocomotorSet Speed='1.5' Condition='FREEFALL' Locomotor='C\\0'/>";
        var combined=Check(original,p,"",pair);int combinedRoot=(int)Word(combined.InstanceData,8,big)-4;
        if(combinedRoot!=1740 || combined.InstanceData.Length!=2356 || Word(combined.InstanceData,combinedRoot+412,big)!=1 || Word(combined.InstanceData,combinedRoot+416,big)!=2332 || Word(combined.InstanceData,combinedRoot+420,big)!=1 || Word(combined.InstanceData,combinedRoot+424,big)!=2344 || Word(combined.InstanceData,2332,big)!=24 || Word(combined.InstanceData,2336,big)!=0 || Word(combined.InstanceData,2340,big)!=6 || Word(combined.InstanceData,2344,big)!=0x3fc00000u || Word(combined.InstanceData,2348,big)!=2 || Word(combined.InstanceData,2352,big)!=1 || combined.ImportsData.Length!=0)throw new Exception("Independent mixed set layout/references");
        uint[][] memberNames={new uint[]{735930353u,2905958645u,912300027u},new uint[]{2322663157u,45929675u,728601449u}};
        uint[][] memberTypes={new uint[]{887689851u,2593517055u,3985361129u},new uint[]{1683514611u,2220692146u,4275178604u}};
        for(int i=0;i<2;++i) {
            int table=(int)Word(combined.InstanceData,12*(92+i)+8,big);int nested=(int)Word(combined.InstanceData,table+20,big);
            if(Word(combined.InstanceData,table,big)!=1820487795u || Word(combined.InstanceData,table+8,big)!=combinedRoot+412+8*i || Word(combined.InstanceData,table+12,big)!=2974916843u || Word(combined.InstanceData,table+16,big)!=0)throw new Exception("Independent set list token");
            for(int j=0;j<3;++j)if(Word(combined.InstanceData,nested+12*j,big)!=memberNames[i][j] || Word(combined.InstanceData,nested+12*j+4,big)!=memberTypes[i][j] || Word(combined.InstanceData,nested+12*j+8,big)!=2332+12*i+4*j)throw new Exception("Independent nested set token");
            for(int j=36;j<48;++j)if(combined.InstanceData[nested+j]!=0)throw new Exception("Independent set token sentinel");
        }
        Check(original,p,"",pair+pair);
        foreach(int length in new[]{3,7,20}) {
            string many="";for(int i=0;i<length;++i)many+="<ArmorSet Armor='A\\23' Conditions='VETERAN'/><LocomotorSet Speed='"+i+"' Condition='NORMAL'/>";
            Check(original,p,"Browser='a bb'",many);
        }
        Check(original,p,"Browser='one two' Description='text'",pair+"<DisplayName>name</DisplayName><EquivalentTo>a</EquivalentTo><Flammability Fuel='17'/><LiveCameraOffset y='-3'/><UpgradeCameo>abc</UpgradeCameo><DisplayUpgrade>upg</DisplayUpgrade>");
        Check(original,p,"","<ArmorSet TypeId='1' Conditions='ELITE'/><LocomotorSet TypeId='2' Condition='NORMAL'/>");
        foreach(string child in new[]{"<ArmorSet Unknown='1'/>","<ArmorSet Armor='A' xmlns:f='foreign' f:DamageFX='B'/>","<LocomotorSet speed='1'/>","<LocomotorSet><Unknown/></LocomotorSet>","<ArmorSet>text</ArmorSet>","<LocomotorSet>text</LocomotorSet>"})Reject(p,"",child);
    }
    static void VisualLeaves(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        string[] nodes={"FormationPreviewDecal","FormationPreviewItemDecal","ShadowInfo","VisionInfo"};
        string[][] fieldNames={new[]{"Width","Height"},new[]{"Width","Height"},new[]{"SizeX","SizeY","OffsetX","OffsetY","OpacityStart","OpacityPeak","OpacityEnd","MaxHeight","SunAngle"},new[]{"VisionRange","ShroudClearingRange","ShroudClearingHeight","VisionSide","VisionRear","VisionBonusTestRadius","VisionBonusPercentPerFoot","MaxVisionBonusPercent","MinVisionBonusPercent"}};
        for(int n=0;n<nodes.Length;++n) {
            Check(original,p,"","<"+nodes[n]+"/>");
            Check(original,p,"","<"+nodes[n]+" TypeId='123'/>");
            foreach(string field in fieldNames[n])foreach(string value in new[]{"", "text", "0", "-17", "1.25", "0.1", "2D3", "100", "1e38", "-1e38", "1e40", "-1e40", "1e-40"})Check(original,p,"","<"+nodes[n]+" "+field+"='"+value+"'/>");
            Reject(p,"","<"+nodes[n]+"/><"+nodes[n]+"/>");Reject(p,"","<"+nodes[n]+" Unknown='1'/>");Reject(p,"","<"+nodes[n]+"><Unknown/></"+nodes[n]+">");Reject(p,"","<"+nodes[n]+">text</"+nodes[n]+">");Reject(p,"","<"+nodes[n]+" xmlns='foreign'/>");
        }
        foreach(string n in new[]{"FormationPreviewDecal","FormationPreviewItemDecal"})foreach(string value in new[]{"", "a", "abcd", "abcde", "é ÿ 中文", "with spaces", "A&amp;B"})Check(original,p,"","<"+n+" Texture='"+value+"'/>");
        foreach(string field in new[]{"Texture","AdditionalTexture"})foreach(string value in new[]{"", "Unresolved", "T\\0", "T\\23", "T\\-1", "T\\2147483648", "T\\-2147483649", "T\\23trailing"})Check(original,p,"","<ShadowInfo "+field+"='"+value+"'/>");
        foreach(string field in new[]{"OverrideLODVisibility","UseHouseColor","IsRotatingWithObject","LocalPlayerOnly"})foreach(string value in new[]{"true","false","TRUE","False","1","0","t","yes"," true",""})Check(original,p,"","<ShadowInfo "+field+"='"+value+"'/>");
        foreach(string field in new[]{"OpacityFadeInTime","OpacityFadeOutTime"})foreach(string value in new[]{"0s","-17s","23ms","2d1s","1000ms","1.25s","5",""})Check(original,p,"","<ShadowInfo "+field+"='"+value+"'/>");
        string[] shadowEnums={"NONE","DECAL","ALPHA_DECAL","ADDITIVE_DECAL","ALPHA_DECAL_DYNAMIC","ADDITIVE_DECAL_DYNAMIC","MERGE_DECAL"};
        for(int i=0;i<shadowEnums.Length;++i) {var b=Check(original,p,"","<ShadowInfo Type='"+shadowEnums[i]+"'/>");if(Word(b.InstanceData,2440,big)!=(uint)i)throw new Exception("Independent ShadowType enum");}
        foreach(string value in new[]{"", "UNKNOWN", "none", " NONE", "NONE ", "0", "-1"}) {var b=Check(original,p,"","<ShadowInfo Type='"+value+"'/>");if(Word(b.InstanceData,2440,big)!=3)throw new Exception("Independent ShadowType fallback");}
        foreach(string value in new[]{"0","1","-1","0x12345678","4294967295","4294967296","-4294967296","text",""})Check(original,p,"","<VisionInfo VisionBonusTestSegments='"+value+"'/>");
        var decal=Check(original,p,"","<FormationPreviewDecal/>");
        if((int)Word(decal.InstanceData,8,big)-4!=1668 || decal.InstanceData.Length!=2276 || Word(decal.InstanceData,1668+512,big)!=2260 || Word(decal.InstanceData,2260,big)!=0 || Word(decal.InstanceData,2264,big)!=0 || Word(decal.InstanceData,2268,big)!=0x42480000u || Word(decal.InstanceData,2272,big)!=0x42480000u)throw new Exception("Independent decal defaults/layout");
        var texture=Check(original,p,"","<FormationPreviewDecal Texture='abcd'/>");
        if(texture.InstanceData.Length!=2284 || Word(texture.InstanceData,2260,big)!=4 || Word(texture.InstanceData,2264,big)!=2276)throw new Exception("Independent decal texture allocation");
        var vision=Check(original,p,"","<VisionInfo/>");uint[] visionWords={0,0,0x42480000u,0x3f800000u,0x3f800000u,0x42c80000u,8,0,0x3f000000u,0xbf000000u};
        if((int)Word(vision.InstanceData,8,big)-4!=1752 || vision.InstanceData.Length!=2384 || Word(vision.InstanceData,1752+552,big)!=2344)throw new Exception("Independent VisionInfo layout");
        for(int i=0;i<visionWords.Length;++i)if(Word(vision.InstanceData,2344+4*i,big)!=visionWords[i])throw new Exception("Independent VisionInfo defaults "+i);
        var shadow=Check(original,p,"","<ShadowInfo/>");
        if((int)Word(shadow.InstanceData,8,big)-4!=1848 || shadow.InstanceData.Length!=2500 || Word(shadow.InstanceData,1848+544,big)!=2440 || Word(shadow.InstanceData,2440+36,big)!=0x3f800000u || Word(shadow.InstanceData,2440+48,big)!=0x41a00000u)throw new Exception("Independent ShadowInfo layout/default floats");
        for(int i=0;i<4;++i)if(shadow.InstanceData[2440+56+i]!=(i==2?1:0))throw new Exception("Independent ShadowInfo packed booleans");
        Check(original,p,"Browser='a bb' Description='desc'","<DisplayName>name</DisplayName><ArmorSet Conditions='ELITE'/><LocomotorSet Speed='1'/><Flammability Fuel='3'/><FormationPreviewDecal Texture='abcd' Width='23'/><FormationPreviewItemDecal Texture='x' Height='17'/><LiveCameraOffset x='2'/><UpgradeCameo>cameo</UpgradeCameo><ShadowInfo Texture='T\\23' OpacityFadeInTime='250ms' LocalPlayerOnly='true'/><VisionInfo VisionSide='30' ShroudClearingHeight='90'/><DisplayUpgrade>upg</DisplayUpgrade>");
        Reject(p,"","<ShadowInfo xmlns:f='foreign' f:SizeX='1'/>");
    }
    static int Token(byte[] data,int table,uint key,bool big) {
        for(int at=table;Word(data,at,big)!=0;at+=12)if(Word(data,at,big)==key)return at;
        throw new Exception("Missing token "+key);
    }
    static void AutoResolveFamily(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        string[] references={"","Unresolved","Name\\0","Name\\23","Name\\-1","Name\\2147483647","Name\\2147483648","Name\\-2147483648","Name\\-2147483649","Name\\+17","Name\\23trailing","Name\\0x10","Name\\ 23","Name\\text"};
        string[] ids={"","a","AB","abc","abcd","abcde","ABCDEF","AbCdEfG","MiXeDCaSe","with spaces","Asset:Name","é","ÿ","中文"};
        string[] strings={"","x","abc","abcd","abcde","long value with spaces","A&B","中文测试"};
        for(int n=0;n<2;++n) {
            string name=n==0?"AutoResolveArmor":"AutoResolveWeapon",field=n==0?"Armor":"Weapon";
            int rootOffset=504+4*n;uint key=n==0?1972927658u:2393386135u;
            var empty=Check(original,p,"","<"+name+"/>");var bytes=empty.InstanceData;
            int root=(int)Word(bytes,8,big)-4,obj=(int)Word(bytes,root+rootOffset,big),tab=(int)Word(bytes,Token(bytes,0,key,big)+8,big);
            if(root!=1692 || obj!=root+592 || bytes.Length!=2304)throw new Exception("Independent auto-set default allocation");
            if(Word(bytes,tab,big)!=(n==0?735930353u:63188358u) || Word(bytes,tab+4,big)!=(n==0?1636048523u:3494305058u) || Word(bytes,tab+8,big)!=obj)throw new Exception("Independent auto-set reference token");
            for(int group=0;group<2;++group) {
                int record=tab+12*(group+1),listTab=(int)Word(bytes,record+8,big);
                if(Word(bytes,record,big)!=(group==0?2138150697u:4283231007u) || Word(bytes,record+4,big)!=653971399u || Word(bytes,listTab,big)!=1820487795u || Word(bytes,listTab+4,big)!=1564774360u || Word(bytes,listTab+8,big)!=obj+4+8*group)throw new Exception("Independent auto-set list tokens");
            }
            for(int j=0;j<12;++j)if(bytes[tab+36+j]!=0)throw new Exception("Auto-set sentinel");
            foreach(string value in references)Check(original,p,"","<"+name+" "+field+"='"+Escape(value)+"'/>");
            foreach(string tag in new[]{"RequiredUpgrade","ExcludedUpgrade"})foreach(string value in ids)Check(original,p,"","<"+name+"><"+tag+">"+Escape(value)+"</"+tag+"></"+name+">");
            foreach(int a in new[]{0,1,2,5,17})foreach(int b in new[]{0,1,2,5,17}) {
                string content="";for(int j=0;j<a;++j)content+="<RequiredUpgrade>required"+j+"</RequiredUpgrade>";for(int j=0;j<b;++j)content+="<ExcludedUpgrade>excluded"+j+"</ExcludedUpgrade>";
                var result=Check(original,p,"","<"+name+" "+field+"='Ref\\23'>"+content+"</"+name+">");
                var d=result.InstanceData;int rr=(int)Word(d,8,big)-4,oo=(int)Word(d,rr+rootOffset,big),tt=(int)Word(d,Token(d,0,key,big)+8,big);
                if(Word(d,oo,big)!=24 || Word(d,oo+4,big)!=a || Word(d,oo+12,big)!=b || rr!=1692+12*(a+b) || d.Length!=2304+16*(a+b))throw new Exception("Independent auto-set sizes/reference");
                for(int group=0;group<2;++group) {
                    int count=group==0?a:b,lt=(int)Word(d,tt+12*(group+1)+8,big),ptr=(int)Word(d,oo+8+8*group,big);
                    for(int j=0;j<count;++j)if(Word(d,lt+12*(j+1),big)!=2974916843u || Word(d,lt+12*(j+1)+4,big)!=0 || Word(d,lt+12*(j+1)+8,big)!=ptr+4*j)throw new Exception("Independent upgrade item bookmark");
                }
            }
            string ordered="<"+name+"><RequiredUpgrade>A</RequiredUpgrade><RequiredUpgrade>B</RequiredUpgrade><ExcludedUpgrade>C</ExcludedUpgrade><ExcludedUpgrade>D</ExcludedUpgrade></"+name+">";
            string interleaved="<"+name+"><ExcludedUpgrade>C</ExcludedUpgrade><RequiredUpgrade>A</RequiredUpgrade><ExcludedUpgrade>D</ExcludedUpgrade><RequiredUpgrade>B</RequiredUpgrade></"+name+">";
            Equal(Check(original,p,"",ordered).InstanceData,Check(original,p,"",interleaved).InstanceData,"auto-set schema allocation order",false);
            Check(original,p,"","<"+name+" TypeId='17' "+field+"='Ref\\-1'><RequiredUpgrade TypeId='9'>A</RequiredUpgrade><ExcludedUpgrade TypeId='10'>B</ExcludedUpgrade></"+name+">");
            foreach(string invalid in new[]{"<"+name+"/><"+name+"/>","<"+name+" Unknown='1'/>","<"+name+" xmlns='foreign'/>","<"+name+">text</"+name+">","<"+name+"><Unknown/></"+name+">","<"+name+"><RequiredUpgrade Unknown='1'/></"+name+">","<"+name+"><ExcludedUpgrade xmlns='foreign'/></"+name+">","<"+name+"><RequiredUpgrade><Unknown/></RequiredUpgrade></"+name+">","<"+name+"><ExcludedUpgrade xmlns:f='foreign' f:TypeId='1'/></"+name+">","<"+name+" xmlns:f='foreign' f:"+field+"='x'/>","<"+name+"><RequiredUpgrade>A</RequiredUpgrade>text</"+name+">"})Reject(p,"",invalid);
        }
        var info=Check(original,p,"","<AutoResolveInfo/>");var ib=info.InstanceData;int ir=(int)Word(ib,8,big)-4,io=(int)Word(ib,ir+548,big),it=(int)Word(ib,Token(ib,0,2236001359u,big)+8,big);
        if(ir!=1704 || io!=2296 || ib.Length!=2332)throw new Exception("Independent AutoResolveInfo layout");
        uint[] names={928084911u,1310294233u,1972927658u,2393386135u,3105727316u,2746445579u};uint[] types={438907818u,3186653541u,1636048523u,3494305058u,438907818u,438907818u};int[] offs={0,8,12,16,20,28};
        for(int j=0;j<6;++j)if(Word(ib,it+12*j,big)!=names[j] || Word(ib,it+12*j+4,big)!=types[j] || Word(ib,it+12*j+8,big)!=io+offs[j])throw new Exception("Independent AutoResolveInfo member "+j);
        for(int j=0;j<12;++j)if(ib[it+72+j]!=0)throw new Exception("AutoResolveInfo sentinel");
        for(int j=0;j<36;++j)if(ib[io+j]!=0)throw new Exception("AutoResolveInfo zero default");
        foreach(string field in new[]{"AutoResolveUnitType","AutoResolveLeadership","AutoResolveCombatChain"})foreach(string value in strings)Check(original,p,"","<AutoResolveInfo "+field+"='"+Escape(value)+"'/>");
        foreach(string field in new[]{"AutoResolveArmor","AutoResolveWeapon"})foreach(string value in references)Check(original,p,"","<AutoResolveInfo "+field+"='"+Escape(value)+"'/>");
        foreach(string value in ids)Check(original,p,"","<AutoResolveInfo AutoResolveBody='"+Escape(value)+"'/>");
        foreach(string field in new[]{"AutoResolveBody","AutoResolveUnitType","AutoResolveLeadership","AutoResolveCombatChain"}) {
            var a=Check(original,p,"","<AutoResolveInfo "+field+"='AbCd'/>");var b=Check(original,p,"","<AutoResolveInfo "+field+"='abcd'/>");
            if(field=="AutoResolveBody")Equal(a.InstanceData,b.InstanceData,"module id lowercase",false);
        }
        foreach(string invalid in new[]{"<AutoResolveInfo/><AutoResolveInfo/>","<AutoResolveInfo Unknown='1'/>","<AutoResolveInfo><Unknown/></AutoResolveInfo>","<AutoResolveInfo>text</AutoResolveInfo>","<AutoResolveInfo xmlns='foreign'/>","<AutoResolveInfo xmlns:f='foreign' f:AutoResolveBody='A'/>"})Reject(p,"",invalid);
        string armor="<AutoResolveArmor Armor='A\\23'><RequiredUpgrade>upA</RequiredUpgrade><ExcludedUpgrade>upB</ExcludedUpgrade></AutoResolveArmor>",weapon="<AutoResolveWeapon Weapon='W\\-1'><RequiredUpgrade>upC</RequiredUpgrade><RequiredUpgrade>upD</RequiredUpgrade></AutoResolveWeapon>",meta="<AutoResolveInfo TypeId='5' AutoResolveUnitType='tank' AutoResolveBody='ModuleID' AutoResolveArmor='A\\1' AutoResolveWeapon='W\\23' AutoResolveLeadership='leader' AutoResolveCombatChain='chain'/>";
        Equal(Check(original,p,"Browser='a bb' Description='desc'",armor+weapon+meta).InstanceData,Check(original,p,"Browser='a bb' Description='desc'",meta+weapon+armor).InstanceData,"auto family allocation order",false);
        Check(original,p,"Browser='a bb' Description='desc'","<DisplayName>name</DisplayName><ArmorSet Armor='A\\23'/><LocomotorSet Locomotor='L\\1'/><EquivalentTo>tank</EquivalentTo><Flammability Fuel='3'/>"+meta+weapon+armor+"<FormationPreviewDecal Texture='image'/><LiveCameraOffset x='2'/><UpgradeCameo>cameo</UpgradeCameo><ShadowInfo Texture='T\\23'/><VisionInfo VisionSide='30'/><DisplayUpgrade>upg</DisplayUpgrade>");
    }
    static void PointerStringLists(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        string[] names={"BuildFadeInOnCreateList","BuildVariations"};uint[] keys={4079217817u,2160707259u};
        // Reuse the actual SDK StringList declaration, rather than a guessed
        // schema. XmlDocument validation leaves xs:list as element text. BAB
        // ValidateInstances annotates TypeId and passes the same XmlNode onward.
        var baseSchema=new XmlDocument();baseSchema.Load("SDK/Schemas/xsd/Includes/Base.xsd");
        var ns=new XmlNamespaceManager(baseSchema.NameTable);ns.AddNamespace("xs","http://www.w3.org/2001/XMLSchema");
        var listType=baseSchema.SelectSingleNode("/xs:schema/xs:simpleType[@name='StringList']",ns);
        if(listType==null || listType.SelectSingleNode("xs:list[@itemType='xs:token']",ns)==null)throw new Exception("SDK StringList schema changed");
        string schema="<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:ea='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'>"+listType.OuterXml+"<xs:element name='BuildFadeInOnCreateList' type='ea:StringList'/><xs:element name='BuildVariations' type='ea:StringList'/></xs:schema>";
        var schemas=new System.Xml.Schema.XmlSchemaSet();using(var reader=XmlReader.Create(new System.IO.StringReader(schema)))schemas.Add(null,reader);schemas.Compile();
        for(int n=0;n<2;++n) {
            string name=names[n];int offset=436+4*n;
            var empty=Check(original,p,"","<"+name+"/>");var d=empty.InstanceData;
            int root=(int)Word(d,8,big)-4,ptr=(int)Word(d,root+offset,big),tab=(int)Word(d,Token(d,0,keys[n],big)+8,big);
            if(root!=1632 || ptr!=2224 || d.Length!=2232 || Word(d,ptr,big)!=0 || Word(d,ptr+4,big)!=0)throw new Exception("Independent pointer StringList allocation");
            if(Word(d,tab,big)!=1820487795u || Word(d,tab+4,big)!=1564774360u || Word(d,tab+8,big)!=ptr)throw new Exception("Independent pointer StringList count token");
            foreach(string value in new[]{""," ","a","a bb ccc"," a  bb ","a\tb\rc\nd","a,b;c","a a a a","é ÿ 中文","A&B"}) {
                var xml=new XmlDocument();xml.LoadXml("<"+name+" xmlns='uri:ea.com:eala:asset'>"+Escape(value)+"</"+name+">");xml.Schemas=schemas;
                xml.Validate(delegate(object sender,System.Xml.Schema.ValidationEventArgs e){throw new Exception("StringList schema: "+e.Message);});
                if(xml.DocumentElement.SelectNodes("*").Count!=0)throw new Exception("Schema unexpectedly expanded list text");
                xml.DocumentElement.SetAttribute("TypeId","123");
                var result=Check(original,p,"",xml.DocumentElement.OuterXml);
                Equal(empty.InstanceData,result.InstanceData,"Original pointer list ignores valid text",false);
            }
            Check(original,p,"","<"+name+"><![CDATA[a bb]]><!-- retained comment --><?probe ignored?></"+name+">");
            // Characterize the original overload's nonempty path separately.
            // This element-child representation violates StringList XSD and is
            // deliberately refused by the recovered public component.
            string synthetic="<"+name+"><Item>a</Item><Item>bb</Item></"+name+">";
            var observed=original.ProcessInstance(Declaration("",synthetic));var od=observed.InstanceData;
            int oroot=(int)Word(od,8,big)-4,optr=(int)Word(od,oroot+offset,big),items=(int)Word(od,optr+4,big);
            if(Word(od,optr,big)!=2 || Word(od,items,big)!=1 || Word(od,items+8,big)!=2)throw new Exception("Original pointer list must enumerate element children");
            Reject(p,"",synthetic);
            var invalid=new XmlDocument();invalid.LoadXml("<"+name+" xmlns='uri:ea.com:eala:asset'><Item>a</Item></"+name+">");invalid.Schemas=schemas;bool invalidRejected=false;
            invalid.Validate(delegate(object sender,System.Xml.Schema.ValidationEventArgs e){if(e.Severity==System.Xml.Schema.XmlSeverityType.Error)invalidRejected=true;});if(!invalidRejected)throw new Exception("StringList nested elements unexpectedly schema-valid");
            foreach(string bad in new[]{"<"+name+"/><"+name+"/>","<"+name+" Unknown='1'/>","<"+name+" xmlns='foreign'/>","<"+name+" xmlns:f='foreign' f:TypeId='1'/>"})Reject(p,"",bad);
        }
        var both=Check(original,p,"","<BuildFadeInOnCreateList>a bb</BuildFadeInOnCreateList><BuildVariations>c dd</BuildVariations>");var b=both.InstanceData;int br=(int)Word(b,8,big)-4;
        if(br!=1644 || b.Length!=2252 || Word(b,br+436,big)!=2236 || Word(b,br+440,big)!=2244)throw new Exception("Independent pointer list group order");
        Equal(b,Check(original,p,"","<BuildVariations/><BuildFadeInOnCreateList/>").InstanceData,"Pointer list allocation independent of XML order",false);
        Check(original,p,"Browser='a bb' Description='desc'","<BuildVariations>a bb</BuildVariations><DisplayName>name</DisplayName><ArmorSet Armor='A\\1'/><LocomotorSet Locomotor='L\\2'/><BuildFadeInOnCreateList>c dd</BuildFadeInOnCreateList><EquivalentTo>tank</EquivalentTo><Flammability Fuel='3'/><AutoResolveWeapon Weapon='W\\3'><RequiredUpgrade>upgrade</RequiredUpgrade></AutoResolveWeapon><AutoResolveInfo AutoResolveUnitType='tank'/><ShadowInfo/><VisionInfo/><DisplayUpgrade>upg</DisplayUpgrade>");
    }
    static void ObjectResourceLists(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        var empty=Check(original,p,"","<ObjectResourceInfo/>");var d=empty.InstanceData;
        int root=(int)Word(d,8,big)-4,obj=(int)Word(d,root+408,big),tab=(int)Word(d,Token(d,0,1789579963u,big)+8,big),list=(int)Word(d,tab+8,big);
        if(root!=1656 || obj!=2248 || d.Length!=2256 || Word(d,obj,big)!=0 || Word(d,obj+4,big)!=0)throw new Exception("Independent ObjectResourceInfo default allocation");
        if(Word(d,tab,big)!=4043881713u || Word(d,tab+4,big)!=3070636877u || Word(d,list,big)!=1820487795u || Word(d,list+4,big)!=1564774360u || Word(d,list+8,big)!=obj)throw new Exception("Independent ObjectResourceInfo tokens");
        for(int j=0;j<12;++j)if(d[tab+12+j]!=0)throw new Exception("ObjectResourceInfo sentinel");
        string[] values={"0","1","-1","255","256","65535","65536","0x12345678","010","08","2147483647","2147483648","4294967295","4294967296","-4294967296"," 23tail","text",""};
        foreach(string field in new[]{"Account","Amount"})foreach(string value in values)Check(original,p,"","<ObjectResourceInfo><BuildCost "+field+"='"+Escape(value)+"'/></ObjectResourceInfo>");
        foreach(int count in new[]{0,1,2,5,17,64}) {
            string content="";for(int j=0;j<count;++j)content+="<BuildCost Account='"+j+"' Amount='"+(j+100)+"'/>";
            var result=Check(original,p,"","<ObjectResourceInfo>"+content+"</ObjectResourceInfo>");var b=result.InstanceData;
            int r=(int)Word(b,8,big)-4,o=(int)Word(b,r+408,big),t=(int)Word(b,Token(b,0,1789579963u,big)+8,big),l=(int)Word(b,t+8,big),items=(int)Word(b,o+4,big);
            if(r!=1656+48*count || b.Length!=2256+56*count || o!=r+592 || Word(b,o,big)!=count || (count>0 && items!=o+8))throw new Exception("Independent BuildCost list size/order");
            for(int j=0;j<count;++j) {
                int itemToken=(int)Word(b,l+12*(j+1)+8,big);
                if(Word(b,l+12*(j+1),big)!=2974916843u || Word(b,l+12*(j+1)+4,big)!=0 || Word(b,itemToken,big)!=78324893u || Word(b,itemToken+4,big)!=1982953380u || Word(b,itemToken+8,big)!=items+8*j || Word(b,itemToken+12,big)!=1343336868u || Word(b,itemToken+16,big)!=1982953380u || Word(b,itemToken+20,big)!=items+8*j+4 || Word(b,items+8*j,big)!=j || Word(b,items+8*j+4,big)!=j+100)throw new Exception("Independent MoneyTransaction fields/bookmarks");
                for(int k=0;k<12;++k)if(b[itemToken+24+k]!=0)throw new Exception("MoneyTransaction sentinel");
            }
        }
        Check(original,p,"","<ObjectResourceInfo TypeId='1'><BuildCost TypeId='2' Account='0x12345678' Amount='4294967295'/><BuildCost/></ObjectResourceInfo>");
        foreach(string bad in new[]{"<ObjectResourceInfo/><ObjectResourceInfo/>","<ObjectResourceInfo Unknown='1'/>","<ObjectResourceInfo xmlns='foreign'/>","<ObjectResourceInfo>text</ObjectResourceInfo>","<ObjectResourceInfo><Unknown/></ObjectResourceInfo>","<ObjectResourceInfo><BuildCost Unknown='1'/></ObjectResourceInfo>","<ObjectResourceInfo><BuildCost xmlns='foreign'/></ObjectResourceInfo>","<ObjectResourceInfo><BuildCost><Unknown/></BuildCost></ObjectResourceInfo>","<ObjectResourceInfo><BuildCost>text</BuildCost></ObjectResourceInfo>","<ObjectResourceInfo><BuildCost xmlns:f='foreign' f:Amount='1'/></ObjectResourceInfo>","<ObjectResourceInfo><BuildCost/>text</ObjectResourceInfo>"})Reject(p,"",bad);
        string costs="<ObjectResourceInfo><BuildCost Account='1' Amount='2'/><BuildCost Account='3' Amount='4'/></ObjectResourceInfo>";
        string others="<DisplayName>name</DisplayName><BuildFadeInOnCreateList>a bb</BuildFadeInOnCreateList><BuildVariations>c dd</BuildVariations><EquivalentTo>tank</EquivalentTo><ArmorSet Armor='A\\1'/><LocomotorSet Locomotor='L\\2'/><AutoResolveArmor Armor='A\\23'><RequiredUpgrade>upgrade</RequiredUpgrade></AutoResolveArmor><AutoResolveInfo AutoResolveBody='module'/><ShadowInfo/><VisionInfo/>";
        Equal(Check(original,p,"Browser='a bb' Description='desc'",costs+others).InstanceData,Check(original,p,"Browser='a bb' Description='desc'",others+costs).InstanceData,"Resource list schema allocation order",false);
    }
    static void GameDependencyGroup(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        var empty=Check(original,p,"","<GameDependency/>");var d=empty.InstanceData;
        int root=(int)Word(d,8,big)-4,obj=(int)Word(d,root+404,big),tab=(int)Word(d,Token(d,0,1812791604u,big)+8,big);
        if(root!=1752 || obj!=2344 || d.Length!=2384)throw new Exception("Independent GameDependency allocation");
        uint[] keys={618545001u,849164020u,2787973588u,850409515u,725359606u,2925224871u,3832456114u};
        uint[] types={1742454729u,1742454729u,2814214430u,344905565u,653971399u,653971399u,3832456114u};
        for(int j=0;j<7;++j)if(Word(d,tab+12*j,big)!=keys[j] || Word(d,tab+12*j+4,big)!=types[j])throw new Exception("Independent dependency Token key/type");
        for(int j=0;j<3;++j)if(Word(d,tab+12*j+8,big)!=0)throw new Exception("Missing dependency masks must be null");
        if(Word(d,tab+80,big)!=0)throw new Exception("Missing dependency filter must be null");
        for(int j=0;j<3;++j) {
            int lt=(int)Word(d,tab+12*(j+3)+8,big);
            if(Word(d,lt,big)!=1820487795u || Word(d,lt+4,big)!=1564774360u || Word(d,lt+8,big)!=obj+12+8*j)throw new Exception("Independent dependency list count Token");
        }
        for(int j=0;j<12;++j)if(d[tab+84+j]!=0)throw new Exception("Dependency sentinel");
        string[] fields={"RequiredModelConditionsAny","ForbiddenModelConditions","RequiredObjectStatusAny"};
        string modelNames="ABOUT_TO_HIT ACCELERATE ACTIVELY_BEING_CONSTRUCTED ACTIVELY_CONSTRUCTING AFFECTED_BY_EMP AFLAME AIM_FAR AIM_HIGH AIM_LOW AIM_NEAR AIM_STRAIGHT AIR_POWER_EFFECT ALL ALTERNATE_FORMATION ARMORSET_ALTERNATE_FORMATION ARMORSET_ELITE ARMORSET_HERO ARMORSET_MOUNTED ARMORSET_PLAYER_UPGRADE ARMORSET_PLAYER_UPGRADE_2 ARMORSET_PLAYER_UPGRADE_3 ARMORSET_UNBESIEGEABLE ARMORSET_VETERAN ARMORSET_WEAK_VERSUS_BASEDEFENSES ATTACHED ATTACKING ATTACKING_POSITION ATTACKING_STRUCTURE ATTACK_MOVING AWAITING_CONSTRUCTION BACKCRUSHED BACKING_UP BASE_BUILD BEING_MAULED BETWEEN_FIRING_SHOTS_A BETWEEN_FIRING_SHOTS_B BETWEEN_FIRING_SHOTS_C BETWEEN_FIRING_SHOTS_D BETWEEN_FIRING_SHOTS_E BUILD_PLACEMENT_CURSOR BUILD_PLACEMENT_HINT BUILD_VARIATION_ONE BUILD_VARIATION_TWO BURNED BURNINGDEATH BURNT_MODEL BURNT_TEXTURE CANCEL_CAPTURE CAPTURED CAPTURE_0 CAPTURE_100 CAPTURE_25 CAPTURE_50 CAPTURE_75 CAPTURING CARRYING CHARGING CHRONORIFT CLIMBING COLLAPSING COMBINED_CHILD COMBINED_PARENT COMING_OUT_OF_FACTORY CONTINUOUS_FIRE_FAST CONTINUOUS_FIRE_MEAN CONTINUOUS_FIRE_SLOW COVER DAMAGED DEATH_1 DEATH_10 DEATH_11 DEATH_12 DEATH_13 DEATH_14 DEATH_15 DEATH_2 DEATH_3 DEATH_4 DEATH_5 DEATH_6 DEATH_7 DEATH_8 DEATH_9 DEBUG DECAY DECELERATE DEFLECT_SPECIAL_POWER DEPLOYED DESTROYED_BACK DESTROYED_FRONT DESTROYED_LEFT DESTROYED_RIGHT DESTROYED_WEAPON DESTROYED_WHILST_BEING_CONSTRUCTED DISGUISED DIVING DOCKING DOCKING_ACTIVE DOCKING_BEGINNING DOCKING_ENDING DOCKING_EXTRACT DOCKING_FILL DOCKING_MOVEBACK DOCKING_PRE_DOCK DOCKING_RETURN DOOR_1_CLOSING DOOR_1_OPENING DOOR_1_WAITING_OPEN DOOR_1_WAITING_TO_CLOSE DOOR_2_CLOSING DOOR_2_OPENING DOOR_2_WAITING_OPEN DOOR_2_WAITING_TO_CLOSE DOOR_3_CLOSING DOOR_3_OPENING DOOR_3_WAITING_OPEN DOOR_3_WAITING_TO_CLOSE DOOR_4_CLOSING DOOR_4_OPENING DOOR_4_WAITING_OPEN DOOR_4_WAITING_TO_CLOSE DOOR_HELIPAD_CLOSING DOOR_HELIPAD_OPENING DOOR_HELIPAD_WAITING_OPEN DOOR_HELIPAD_WAITING_TO_CLOSE DRAFTED DYING DYING_WASATTACKING DYING_WASMOVING EATING EMOTION_AFRAID EMOTION_ALERT EMOTION_AMUSED EMOTION_BRACE_FOR_BEING_CRUSHED EMOTION_CELEBRATING EMOTION_CHEER_FOR_ABOUT_TO_CRUSH EMOTION_COWER EMOTION_DISSIDENT EMOTION_DOOM EMOTION_GUNG_HO EMOTION_LOOK_TO_SKY EMOTION_MORALE_HIGH EMOTION_MORALE_LOW EMOTION_PANIC EMOTION_POINTING EMOTION_TAUNTING EMOTION_TERROR EMOTION_UNCONTROLLABLY_AFRAID ENGAGED EXPLODED_BOUNCING EXPLODED_FLAILING FIRING_A FIRING_B FIRING_C FIRING_D FIRING_E FIRING_OR_PREATTACK_A FIRING_OR_PREATTACK_B FIRING_OR_PREATTACK_C FIRING_OR_PREATTACK_D FIRING_OR_PREATTACK_E FIRING_OR_RELOADING_A FIRING_OR_RELOADING_B FIRING_OR_RELOADING_C FIRING_OR_RELOADING_D FIRING_OR_RELOADING_E FLOODED FLYING FORMATION_PREVIEW FREEFALL FRONTCRUSHED FROZEN GARRISONED GUARDING HARVEST_ACTION HARVEST_PREPARATION HEALTH_PERCENT_0 HEALTH_PERCENT_100 HEALTH_PERCENT_25 HEALTH_PERCENT_50 HEALTH_PERCENT_75 HIDDEN HIGH_TECH_EFFECT HIT_LEVEL_1 HIT_LEVEL_2 HIT_LEVEL_3 HIT_REACTION HORDE_EMPTY IMPENDING_DOOM INFILTRATED_DISABLE INFILTRATED_ENERGY INFILTRATED_RADAR_FREEZE INFILTRATED_RESET INFILTRATED_STEAL_MONEY INFILTRATED_VISION INFILTRATED_VISION_UNITS INSIDE_GARRISON INVALID INVISIBLE_CAMOUFLAGE INVISIBLE_STEALTH INVULNERABLE IRONCURTAIN JETAFTERBURNER JETEXHAUST JUST_BUILT LANDING LAUNCHED LEASHED_RETURNING LEVELED LOADED LOCOMOTOR_BURNINGDEATH LOCOMOTOR_CHANGING_FIRINGARC LOCOMOTOR_COMBO LOCOMOTOR_COMBO2 LOCOMOTOR_COMBO3 LOCOMOTOR_CONTAINED LOCOMOTOR_ENRAGED LOCOMOTOR_FREEFALL LOCOMOTOR_MOUNTED LOCOMOTOR_NORMAL_UPGRADED LOCOMOTOR_PANIC LOCOMOTOR_SCARED LOCOMOTOR_SUPERSONIC LOCOMOTOR_TAXIING LOCOMOTOR_WALL_SCALING LOCOMOTOR_WANDER MARCHING MARKED_FOR_HUNT_TACTIC MARKED_FOR_NO_SKIRMISH_RECRUIT MONEY_STORED_AMOUNT_1 MONEY_STORED_AMOUNT_2 MONEY_STORED_AMOUNT_3 MONEY_STORED_AMOUNT_4 MOUNTED MOVING MOVING_OUT_OF_THE_WAY NIGHT OVER_WATER PACKING PACKING_TYPE_1 PACKING_TYPE_2 PACKING_TYPE_3 PACKING_TYPE_4 PACKING_TYPE_5 PACKING_TYPE_6 PANICKING PARACHUTE_LAND PARACHUTING PARALYZED PARKINGPLACE_1_DOOR_CLOSED PARKINGPLACE_1_DOOR_CLOSING PARKINGPLACE_1_DOOR_OPEN PARKINGPLACE_1_DOOR_OPENING PARKINGPLACE_2_DOOR_CLOSED PARKINGPLACE_2_DOOR_CLOSING PARKINGPLACE_2_DOOR_OPEN PARKINGPLACE_2_DOOR_OPENING PARKINGPLACE_3_DOOR_CLOSED PARKINGPLACE_3_DOOR_CLOSING PARKINGPLACE_3_DOOR_OPEN PARKINGPLACE_3_DOOR_OPENING PARKINGPLACE_4_DOOR_CLOSED PARKINGPLACE_4_DOOR_CLOSING PARKINGPLACE_4_DOOR_OPEN PARKINGPLACE_4_DOOR_OPENING PARKINGPLACE_HELIPAD_DOOR_CLOSED PARKINGPLACE_HELIPAD_DOOR_CLOSING PARKINGPLACE_HELIPAD_DOOR_OPEN PARKINGPLACE_HELIPAD_DOOR_OPENING PARTIALLY_CONSTRUCTED PASSENGER PASSENGER_VARIATION_1 PASSENGER_VARIATION_2 PASSENGER_VARIATION_3 PASSENGER_VARIATION_4 PASSENGER_VARIATION_5 PHANTOM_STRUCTURE PLAYER_POWER_1 POST_COLLAPSE POST_RUBBLE POWER_PLANT_UPGRADED POWER_PLANT_UPGRADING PREATTACK_A PREATTACK_B PREATTACK_C PREATTACK_D PREATTACK_E PREORDER PREPARING PRIMARY_FORMATION PRONE RADAR_EXTENDING RADAR_UPGRADED RAISING_FLAG RAPPELLING REACT_1 REACT_2 REACT_3 REACT_4 REACT_5 REACT_6 REALLYDAMAGED RELOADING_A RELOADING_B RELOADING_C RELOADING_D RELOADING_E REPAIRING_DISABLED RESURRECTED RIDER1 RIDER2 RIDER3 RIDER4 RIDER5 RIDER6 RIDER7 RIDER8 RIDERLESS RUBBLE RUNNING_OFF_MAP SAIL_BLOWN_LEFT SAIL_BLOWN_RIGHT SAIL_FLAPPING SCALING_WALL SCALING_WALL_HORDE SELECTED SHRINK_EFFECT SIEGE_CONTAIN SMOLDERING SNOW SOLD SPECIALPOWER1_READY SPECIALPOWER2_READY SPECIALPOWER3_READY SPECIALPOWER4_READY SPECIALPOWER5_READY SPECIALPOWER6_READY SPECIALPOWER7_READY SPECIALPOWER8_READY SPECIALPOWER9_READY SPECIAL_CHEERING SPECIAL_DAMAGED SPECIAL_ENEMY_NEAR SPECIAL_POWER_1 SPECIAL_POWER_2 SPECIAL_POWER_3 SPECIAL_WEAPON_FIVE SPECIAL_WEAPON_FOUR SPECIAL_WEAPON_ONE SPECIAL_WEAPON_SIX SPECIAL_WEAPON_THREE SPECIAL_WEAPON_TWO SPLATTED START_CAPTURE STRUCTURE_UNPACKING STUNNED STUNNED_FLAILING STUNNED_STANDING_UP SUCKED_UP_HIGH SUPPRESSED SWAPPING_TO_WEAPONSET_1 SWAPPING_TO_WEAPONSET_2 SWAPPING_TO_WEAPONSET_3 SWIMMING SWOOPING TAKING_OFF THROWN_PROJECTILE TIBERIUM_CRYSTAL_TYPE1 TIBERIUM_CRYSTAL_TYPE2 TIBERIUM_CRYSTAL_TYPE3 TIBERIUM_CRYSTAL_TYPE4 TIBERIUM_CRYSTAL_TYPE5 TIBERIUM_GROWING TOPPLED TRANSPORT_MOVING TRANSPORT_STOPPED TURN_LEFT TURN_LEFT_HIGH_SPEED TURN_RIGHT TURN_RIGHT_HIGH_SPEED TURRET_ANGLE_0 TURRET_ANGLE_180 TURRET_ANGLE_270 TURRET_ANGLE_90 TURRET_ROTATE UNCONTROLLABLE UNDERPOWERED UNIT_ACTIVELY_BEING_CONSTRUCTED UNPACKING UPGRADED_ARMOR USER_1 USER_10 USER_2 USER_3 USER_4 USER_5 USER_6 USER_7 USER_8 USER_9 USING_COMBO_LOCOMOTOR USING_SPECIAL_ABILITY USING_WEAPON_A USING_WEAPON_B USING_WEAPON_C USING_WEAPON_D USING_WEAPON_E VEHICLE_CRUSH_CENTER VEHICLE_CRUSH_LEFT VEHICLE_CRUSH_RIGHT WADING WALKING WALL_SEGMENT WANDER WEAPONLOCK_PRIMARY WEAPONLOCK_QUATERNARY WEAPONLOCK_QUINARY WEAPONLOCK_SECONDARY WEAPONLOCK_TERTIARY WEAPONSET_CLOSE_RANGE WEAPONSET_CONTAINED WEAPONSET_CONTESTING_BUILDING WEAPONSET_ELITE WEAPONSET_GARRISONED WEAPONSET_HERO WEAPONSET_MOUNTED WEAPONSET_PASSENGER_TYPE_ONE WEAPONSET_PASSENGER_TYPE_TWO WEAPONSET_PLAYER_UPGRADE WEAPONSET_RIDER1 WEAPONSET_RIDER2 WEAPONSET_RIDER3 WEAPONSET_RIDER4 WEAPONSET_SPECIAL_ONE WEAPONSET_SPECIAL_TWO WEAPONSET_SPECIAL_UPGRADE WEAPONSET_TOGGLE_1 WEAPONSET_TOGGLE_2 WEAPONSET_TOGGLE_3 WEAPONSET_VETERAN WEAPONSLOTID_01 WEAPONSLOTID_02 WEAPONSLOTID_03 WEAPONSLOTID_04 WEAPONSLOTID_05 WEAPONSLOTID_06 WEAPONSLOTID_07 WEAPONSLOTID_08 WEAPONSLOTID_09 WEAPONSLOTID_10 WEAPONSTATE_CLOSE_RANGE WEAPONSTATE_CONTAINED WEAPONSTATE_ONE WEAPONSTATE_THREE WEAPONSTATE_TWO WEAPON_TOGGLING WORLD_BUILDER",statusNames="ABSORBED_DAMAGE ADVANCED_MISSILE_PACKS AFLAME AIRBORNE_TARGET AIRCRAFT_IGNORE_SAMEPLAYER_HANGAR_RULE AIR_LOCOMOTOR_ACTIVE ANTI_GARRISON ATTACHED ATTACKING_GARRISONED_STRUCTURE BOOBY_TRAPPED BOOBY_TRAP_EXPLODE BOOKENDING BRIDGE_IMPASSABLE BROADCASTING_INVISIBILITY BUILD_BEING_CANCELED BURNED CANNOT_BE_SOLD CANNOT_LAND_AT_AIRFIELD CAN_ATTACK CAN_ATTACK_WHILE_STEALTHED CAN_CONTEST_GARRISON CAN_ENTER_ANYTHING CAN_NOT_WALK_ON CAN_STEALTH CAN_STEALTH_FROM_PRODUCER CARRYING_FLAG CHANTING CHARGING_BASE_DEFENSES CHARGING_WEAPON CLEARED_FOR_LANDING CLONED COMBINED_ATTACHED COMBINED_CHILD COMBINED_PARENT COMMAND_BUTTON_TOGGLED CONTAINER_OCCUPIED CONTESTED CONTESTING_BUILDING DAMAGED DEATH_1 DEATH_2 DEATH_3 DEATH_4 DEATH_5 DEFLECT_INCOMING_FIRE DELAYED_ENTER_STRUCTURE DEPART_FROM_BOARDING DEPLOYED DEPLOYING DESTROYED DETECTED DOCKING DOES_CONTAIN_TIBERIUM DONT_CLEAR_FOR_BUILD DO_NOT_AVOID_TALLBUILDING DO_NOT_PICK_ME DO_NOT_SCORE ENABLE_TRACER_DRAW ENCLOSED END_FIRE_STATE ENRAGED ENTERING_GOAL EXCEEDED_PATH_NEIGHBOR_COUNT EXECUTING_STORED_COMMAND EXITING_COMBINED EXPLOSIVES_ATTACHED EXPLOSIVES_DETONATING FLEE_OFF_MAP FOLLOWING_THROUGH FORCE_ATTACKING FORCE_ATTACK_MOVING GARRISONED GARRISON_CAN_STEALTH GENERIC_TOGGLE_STATE GRAPPLE_UPDATE_ACTIVE GRAPPLE_UPDATE_BEING_MOVED GROUND_LOCOMOTOR_ACTIVE HARVESTING HAS_SECONDARY_DAMAGE HAS_TIBERIUM_GROWTH_MOD HAS_TIBERIUM_UPGRADE HEALTH_PERCENT_0 HEALTH_PERCENT_100 HEALTH_PERCENT_25 HEALTH_PERCENT_50 HEALTH_PERCENT_75 HIDDEN HIJACKED HIT_SHIELD_SPHERE HORDE_MEMBER IGNORE_AI_COMMAND IGNORING_POWER_DOWN IGNORING_STEALTH IMMOBILE IMMOBILE_ALLOW_ROTATE IMMUNE_TO_BARK INAUDIBLE INHERITED_FROM_ALLY_TEAM INSIDE_GARRISON INVISIBLE_DETECTED INVISIBLE_DETECTED_BY_FRIEND IN_COVER IN_FORMATION_TEMPLATE IN_NANOHIVE IN_SHIELD_SPHERE IN_SPIDER_HOLE IN_STASIS IRRADIATED IS_AIMING_WEAPON IS_ATTACKING IS_BEING_DRAGGED IS_BEING_HARVESTED IS_BRAKING IS_ENGAGED IS_FIRING_WEAPON IS_HIDEOUT IS_LEAVING_FACTORY IS_MELEE_ATTACKING IS_MOVING_TO_RALLY_POINT IS_RELOADING_WEAPON LANDING_IN_PROGRESS LEASHED_RETURNING LEECHED_TARGET_ACTIVE LIFTED_INTO_AIR LINE_SEGMENT_DAMAGE_ACTIVE LOADED_FROM_MAP MAGNETIZED MARCH_OF_DEATH MATCH_TARGETS_SPEED MOVING_TO_DISMOUNT NEXT_MOVE_IS_FORCE_ATTACK_MOVE NEXT_MOVE_IS_REVERSE NON_AUTOACQUIRABLE NOT_IN_WORLD NO_ATTACK NO_ATTACK_FROM_AI NO_AUTO_ACQUIRE NO_BRIBE NO_COLLISIONS NO_HERO_PROPERTIES NO_REFUND NO_SHADOW NO_SPECIAL_ABILITY NO_SQUISHCOLLIDE_DELAY OCLMONITOR_COMPLETED_TASK OCLMONITOR_MONITOR_RELEASED OUT_OF_PHASE OVERCHARGING_WEAPON OVER_WATER PARACHUTING PARKED_AT_AIRFIELD PAUSING PENDING_CONSTRUCTION PHANTOM_STRUCTURE PLACED_BY_PLAYER PLAYER_POWER_1 PLAYER_POWER_2 PLAYER_POWER_3 PLAYER_POWER_4 PLAYER_POWER_5 POINT_DEFENSE_DRONE_ATTACHED PORTER_TAGGED POWERED_DOWN_EMP PREEMPT_PRE_ATTACK RAMPAGING REALLYDAMAGED RECONSTRUCTING REPAIR_ALLIES_WHEN_IDLE REPULSOR RESET_GOAL_POSITION RIDER_IS_PILOT RIFT_OCCUPIED RIOT_SHIELDED RUBBLE RUNNING_DOWN_FROM_BEHIND SCARED_CIVILIAN_CAR SCRAMBLED SHIELDBODY_ENABLED SHROUD_REVEAL_TO_ALL SHRUNK SINKING SKIRMISH_AI_DO_NOT_ATTACK SKIRMISH_AI_DO_NOT_TAKE SOLD SPECIALABILITY_ACTIVE SPECIAL_ABILITY_PACKING_UNPACKING_OR_USING SPECIAL_ARMOR_ACTIVE STAND_GROUND STEALTHED STEAL_NEXT_UNIT_TRAPPED SUBMERGED SURFACED SURFACED_IMMOBILE SWITCHED_WEAPONS TAGGED TARGETED_FOR_REPAIR TEMPORARILY_DEFECTED TIBERIUM_VIBRATING TOPPLING TRANSFORMATION_TOGGLE_STATE TRANSPORT_MOVING_TO_CONTAIN UNATTACKABLE UNCONTROLLABLY_SCARED UNDERGOING_REPAIR UNDER_CONSTRUCTION UNDER_FROZEN UNDER_IRON_CURTAIN UNDER_RUSH_ATTACK UNIT_WANTS_TO_REGARRISON UNPACKING UNSELECTABLE UPDATING_AI USER_PARALYZED USER_POWERED_DOWN USING_ABILITY VEHICLE_ATTACHED WAITING_FOR_BOARDING WALL_SEGMENT WATER_LOCOMOTOR_ACTIVE WEAPON_UPGRADED_01 WEAPON_UPGRADED_02 WEAPON_UPGRADED_03 WONT_RIDE_WITH_YOU";
        foreach(string value in modelNames.Split(' ')) {
            Check(original,p,"","<CrusherInfo><ExtraCrushLevels ModelConditionMatch='"+value+"'/></CrusherInfo>");
            Check(original,p,"","<ProjectedBuildabilityInfo ModelConditionsToReject='"+value+"'/>");
            Check(original,p,"","<ProjectedBuildabilityInfo ModelConditionsToReject='-"+value+" -"+value+" +"+value+"'/>");
        }
        foreach(string value in statusNames.Split(' ')) {
            Check(original,p,"","<ProjectedBuildabilityInfo StatusToReject='"+value+"'/>");
            Check(original,p,"","<ProjectedBuildabilityInfo StatusToReject='-"+value+" -"+value+" +"+value+"'/>");
        }
        for(int n=0;n<3;++n) {
            foreach(string value in (n==2?statusNames:modelNames).Split(' ')) {
                Check(original,p,"","<GameDependency "+fields[n]+"='"+value+"'/>");
                Check(original,p,"","<GameDependency "+fields[n]+"='-"+value+" -"+value+" +"+value+"'/>");
            }
            foreach(string value in new[]{""," ","UNKNOWN","unknown","-UNKNOWN -UNKNOWN","+","-","ALL","all","-aLl","ALL -UNKNOWN","UNKNOWN ALL unknown","  UNKNOWN\tUNKNOWN\n"})Check(original,p,"","<GameDependency "+fields[n]+"='"+Escape(value)+"'/>");
            var all=Check(original,p,"","<GameDependency "+fields[n]+"='ALL'/>");var b=all.InstanceData;int r=(int)Word(b,8,big)-4,o=(int)Word(b,r+404,big),mask=(int)Word(b,o+4*n,big),t=(int)Word(b,Token(b,0,1812791604u,big)+8,big);
            if(mask!=o+40 || b.Length!=2384+(n==2?28:60) || Word(b,t+12*n+8,big)!=mask)throw new Exception("Independent allocated mask pointer/bookmark");
            for(int j=0;j<(n==2?7:15);++j)if(Word(b,mask+4*j,big)!=uint.MaxValue)throw new Exception("Independent ALL includes mask padding");
            var unknown=Check(original,p,"","<GameDependency "+fields[n]+"='UNKNOWN'/>");b=unknown.InstanceData;r=(int)Word(b,8,big)-4;o=(int)Word(b,r+404,big);mask=(int)Word(b,o+4*n,big);int bit=n==2?198:233;
            for(int j=0;j<(n==2?7:15);++j)if(Word(b,mask+4*j,big)!=(j==bit/32?1u<<(bit%32):0u))throw new Exception("Independent mask unknown fallback");
        }
        string[] groups={"RequiredObject","ForbiddenUpgrade","NeededUpgrade"};
        foreach(string name in groups) {
            foreach(string value in new[]{"","a","AB","abc","abcd","abcde","MiXeDCaSe","with spaces","é ÿ 中文","A&B"})Check(original,p,"","<GameDependency><"+name+">"+Escape(value)+"</"+name+"></GameDependency>");
            foreach(int count in new[]{0,1,2,5,17}) {
                string content="";for(int j=0;j<count;++j)content+="<"+name+">value"+j+"</"+name+">";var result=Check(original,p,"","<GameDependency>"+content+"</GameDependency>");var b=result.InstanceData;int r=(int)Word(b,8,big)-4,o=(int)Word(b,r+404,big),group=Array.IndexOf(groups,name),t=(int)Word(b,Token(b,0,1812791604u,big)+8,big),lt=(int)Word(b,t+12*(group+3)+8,big),ptr=(int)Word(b,o+16+8*group,big);
                if(r!=1752+12*count || b.Length!=2384+16*count || Word(b,o+12+8*group,big)!=count)throw new Exception("Independent dependency list sizes");
                for(int j=0;j<count;++j)if(Word(b,lt+12*(j+1),big)!=2974916843u || Word(b,lt+12*(j+1)+4,big)!=0 || Word(b,lt+12*(j+1)+8,big)!=ptr+4*j)throw new Exception("Independent dependency item bookmark");
            }
        }
        foreach(string filter in new[]{"<ObjectFilter/>","<ObjectFilter TypeId='3' id='AbCd' Rule='ALL' Relationship='ALLIES ENEMIES' Alignment='GOOD' Include='AIRCRAFT' Exclude='INFANTRY'/>","<ObjectFilter StatusBitFlags='' StatusBitFlagsExclude=''/>","<ObjectFilter StatusBitFlags='ALL' StatusBitFlagsExclude='UNKNOWN'/>","<ObjectFilter StatusBitFlags='-UNKNOWN -UNKNOWN' Include='ALL -AIRCRAFT' Exclude='UNKNOWN'/>","<ObjectFilter><IncludeThing>AbCd</IncludeThing><ExcludeThing>EFGH</ExcludeThing><IncludeThing>other</IncludeThing></ObjectFilter>"})Check(original,p,"","<GameDependency>"+filter+"</GameDependency>");
        foreach(string attr in new[]{"Rule","Relationship","Alignment","Include","Exclude","StatusBitFlags","StatusBitFlagsExclude"})foreach(string value in new[]{"","UNKNOWN","ALL","NONE","-UNKNOWN"})Check(original,p,"","<GameDependency><ObjectFilter "+attr+"='"+value+"'/></GameDependency>");
        string canonical="<RequiredObject>A</RequiredObject><RequiredObject>B</RequiredObject><ForbiddenUpgrade>C</ForbiddenUpgrade><NeededUpgrade>D</NeededUpgrade><ObjectFilter><IncludeThing>E</IncludeThing><ExcludeThing>F</ExcludeThing></ObjectFilter>",interleaved="<NeededUpgrade>D</NeededUpgrade><RequiredObject>A</RequiredObject><ObjectFilter><IncludeThing>E</IncludeThing><ExcludeThing>F</ExcludeThing></ObjectFilter><ForbiddenUpgrade>C</ForbiddenUpgrade><RequiredObject>B</RequiredObject>";
        Equal(Check(original,p,"","<GameDependency RequiredModelConditionsAny='ALL' ForbiddenModelConditions='' RequiredObjectStatusAny='UNKNOWN'>"+canonical+"</GameDependency>").InstanceData,Check(original,p,"","<GameDependency RequiredModelConditionsAny='ALL' ForbiddenModelConditions='' RequiredObjectStatusAny='UNKNOWN'>"+interleaved+"</GameDependency>").InstanceData,"Dependency schema allocation order",false);
        Check(original,p,"TypeId='2486173485'","<GameDependency TypeId='5' RequiredModelConditionsAny='ALL' ForbiddenModelConditions='UNKNOWN' RequiredObjectStatusAny='ALL'><RequiredObject TypeId='6'>tank</RequiredObject><ForbiddenUpgrade TypeId='7'>upgrade</ForbiddenUpgrade><NeededUpgrade TypeId='8'/><ObjectFilter TypeId='9'><IncludeThing TypeId='10'>other</IncludeThing></ObjectFilter></GameDependency>");
        foreach(string bad in new[]{"<GameDependency/><GameDependency/>","<GameDependency Unknown='1'/>","<GameDependency xmlns='foreign'/>","<GameDependency xmlns:f='foreign' f:RequiredObjectStatusAny='ALL'/>","<GameDependency>text</GameDependency>","<GameDependency><Unknown/></GameDependency>","<GameDependency><RequiredObject Unknown='1'/></GameDependency>","<GameDependency><NeededUpgrade xmlns='foreign'/></GameDependency>","<GameDependency><ForbiddenUpgrade><Unknown/></ForbiddenUpgrade></GameDependency>","<GameDependency><ObjectFilter/><ObjectFilter/></GameDependency>","<GameDependency><ObjectFilter Unknown='1'/></GameDependency>","<GameDependency><ObjectFilter xmlns='foreign'/></GameDependency>","<GameDependency><ObjectFilter>text</ObjectFilter></GameDependency>","<GameDependency><ObjectFilter><Unknown/></ObjectFilter></GameDependency>","<GameDependency><ObjectFilter><IncludeThing Unknown='1'/></ObjectFilter></GameDependency>","<GameDependency><ObjectFilter><IncludeThing xmlns='foreign'/></ObjectFilter></GameDependency>","<GameDependency><ObjectFilter><ExcludeThing><Unknown/></ExcludeThing></ObjectFilter></GameDependency>","<GameDependency><RequiredObject/>text</GameDependency>"})Reject(p,"",bad);
        Check(original,p,"Browser='a bb' Description='desc'","<ObjectResourceInfo><BuildCost Account='1' Amount='23'/></ObjectResourceInfo><BuildVariations>a bb</BuildVariations><AutoResolveArmor Armor='A\\23'><RequiredUpgrade>upgrade</RequiredUpgrade></AutoResolveArmor><GameDependency RequiredModelConditionsAny='ALL' ForbiddenModelConditions='UNKNOWN' RequiredObjectStatusAny='ALL'>"+canonical+"</GameDependency><ShadowInfo/><AutoResolveInfo AutoResolveUnitType='tank'/><VisionInfo/>");
    }
    static void CrushAndPlacement(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        var b=Check(original,p,"","<CrusherInfo/>");var d=b.InstanceData;
        int root=(int)Word(d,8,big)-4,obj=(int)Word(d,root+556,big),tab=(int)Word(d,Token(d,0,519758843u,big)+8,big);
        if(root!=1800 || obj!=2392 || d.Length!=2428)throw new Exception("Independent crusher allocation");
        if(Word(d,obj+4,big)!=0x3f800000u || Word(d,obj+8,big)!=BitConverter.ToUInt32(BitConverter.GetBytes(0.01f),0) || d[obj+29]!=127 || d[obj+31]!=1)throw new Exception("Independent crusher defaults");
        if(Word(d,tab+12*12+8,big)==0)throw new Exception("Crusher empty list requires count Token");
        for(int j=0;j<12;++j)if(d[tab+156+j]!=0)throw new Exception("Crusher sentinel");
        string[] numeric={"", "0","-1","127","128","255","256","-128","-129","0x10","010","2147483647","2147483648","-2147483649","4294967296","text","1.25"," 23tail"};
        foreach(string field in new[]{"CrusherLevel","CrushableLevel","CrushKnockback","CrushZFactor","MinCrushVelocityPercent","CrushDecelerationPercent"})foreach(string value in numeric)Check(original,p,"","<CrusherInfo "+field+"='"+value+"'/>");
        foreach(string field in new[]{"CrushEqualLevelProps","UseCrushAttack","CrushOnlyWhileCharging","CrushAllies","CrushWhileStationary"})foreach(string value in new[]{"","true","false","TRUE","False","1","0","t","yes"," true"})Check(original,p,"","<CrusherInfo "+field+"='"+value+"'/>");
        foreach(string value in new[]{"","Unresolved","Name\\0","Name\\23","Name\\-1","Name\\2147483648","Name\\-2147483649","Name\\23tail","Name\\0x10"})Check(original,p,"","<CrusherInfo CrushWeapon='"+value+"'/>");
        foreach(string field in new[]{"CrusherLevel","CrushableLevel"})foreach(string value in numeric)Check(original,p,"","<CrusherInfo><ExtraCrushLevels "+field+"='"+value+"'/></CrusherInfo>");
        foreach(string value in new[]{"","UNKNOWN","unknown","ALL","STRUCTURE_UNPACKING"," STRUCTURE_UNPACKING","STRUCTURE_UNPACKING ","-STRUCTURE_UNPACKING","0"})Check(original,p,"","<CrusherInfo><ExtraCrushLevels ModelConditionMatch='"+value+"'/></CrusherInfo>");
        foreach(int count in new[]{1,2,3,17,64}) {
            string levels="";for(int j=0;j<count;++j)levels+="<ExtraCrushLevels CrusherLevel='"+j+"' CrushableLevel='"+(-j)+"' ModelConditionMatch='STRUCTURE_UNPACKING'/>";
            b=Check(original,p,"","<CrusherInfo>"+levels+"</CrusherInfo>");d=b.InstanceData;root=(int)Word(d,8,big)-4;obj=(int)Word(d,root+556,big);int items=(int)Word(d,obj+24,big);
            if(root!=1800+60*count || d.Length!=2428+68*count || Word(d,obj+20,big)!=count)throw new Exception("Independent crusher list allocation");
            for(int j=0;j<count;++j)if(d[items+8*j+4]!=(byte)j || d[items+8*j+5]!=(byte)(-j))throw new Exception("Independent signed crush levels");
        }
        b=Check(original,p,"","<ProjectedBuildabilityInfo/>");d=b.InstanceData;root=(int)Word(d,8,big)-4;obj=(int)Word(d,root+564,big);
        tab=(int)Word(d,Token(d,0,1388118305u,big)+8,big);int itemTable=(int)Word(d,tab+20,big);
        if(root!=1740 || obj!=2332 || d.Length!=2444 || Word(d,root+560,big)!=1)throw new Exception("Independent projection allocation");
        if(Word(d,obj+4,big)!=0 || Word(d,obj+8,big)!=1 || Word(d,obj+100,big)!=0x42480000u || d[obj+108]!=1 || Word(d,obj+104,big)!=0 || Word(d,itemTable+20,big)!=0 || Word(d,itemTable+92,big)!=0)throw new Exception("Independent projection defaults/null pointers");
        foreach(string field in new[]{"Radius","RadiusY","AllowedBuildabilityHeightVariation"})foreach(string value in numeric)Check(original,p,"","<ProjectedBuildabilityInfo "+field+"='"+value+"'/>");
        foreach(string value in new[]{"","true","false","TRUE","False","1","0","yes"})Check(original,p,"","<ProjectedBuildabilityInfo PrimaryBuidability='"+value+"'/>");
        foreach(string field in new[]{"BuildPlacementTypes","StatusToReject","ModelConditionsToReject"})foreach(string value in new[]{""," ","UNKNOWN","unknown","ALL","all","-ALL","+ALL","-UNKNOWN -UNKNOWN","UNKNOWN ALL unknown","+","-","MAIN_STRUCTURE BLOCKED","INVALID MAIN_STRUCTURE OTHER_STRUCTURE TIBERIUM_FIELD BLOCKED"})Check(original,p,"","<ProjectedBuildabilityInfo "+field+"='"+value+"'/>");
        foreach(string value in new[]{"BLOCKED","INVALID","MAIN_STRUCTURE","OTHER_STRUCTURE","TIBERIUM_FIELD"}) {
            Check(original,p,"","<ProjectedBuildabilityInfo BuildPlacementTypes='"+value+"'/>");
            Check(original,p,"","<ProjectedBuildabilityInfo BuildPlacementTypes='-"+value+" -"+value+" +"+value+"'/>");
        }
        foreach(string filter in new[]{"<AllowedObjectFilter/>","<AllowedObjectFilter TypeId='3' id='AbCd' Rule='ALL' Relationship='ALLIES ENEMIES' Alignment='GOOD' Include='AIRCRAFT' Exclude='INFANTRY'/>","<AllowedObjectFilter StatusBitFlags='' StatusBitFlagsExclude=''/>","<AllowedObjectFilter StatusBitFlags='ALL' StatusBitFlagsExclude='UNKNOWN'/>","<AllowedObjectFilter><IncludeThing>tank</IncludeThing><ExcludeThing>other</ExcludeThing><IncludeThing>third</IncludeThing></AllowedObjectFilter>"})Check(original,p,"","<ProjectedBuildabilityInfo RadiusY='23'>"+filter+"</ProjectedBuildabilityInfo>");
        foreach(int count in new[]{1,2,3,17,64}) {
            string items="";for(int j=0;j<count;++j)items+="<ProjectedBuildabilityInfo Radius='"+j+"'/>";
            b=Check(original,p,"",items);d=b.InstanceData;root=(int)Word(d,8,big)-4;obj=(int)Word(d,root+564,big);
            if(root!=1620+120*count || d.Length!=2212+232*count || Word(d,root+560,big)!=count)throw new Exception("Independent projection list allocation");
            for(int j=0;j<count;++j)if(Word(d,obj+112*j,big)!=BitConverter.ToUInt32(BitConverter.GetBytes((float)j),0))throw new Exception("Independent projection stride");
        }
        string crush="<CrusherInfo TypeId='1' CrusherLevel='-1' CrushableLevel='2' CrushWeapon='W\\23'><ExtraCrushLevels TypeId='2' ModelConditionMatch='STRUCTURE_UNPACKING'/><ExtraCrushLevels/></CrusherInfo>";
        string projection="<ProjectedBuildabilityInfo TypeId='3' Radius='23' RadiusY='0' BuildPlacementTypes='ALL' StatusToReject='ALL' ModelConditionsToReject='ALL' PrimaryBuidability='false'><AllowedObjectFilter TypeId='4'><IncludeThing TypeId='5'>tank</IncludeThing></AllowedObjectFilter></ProjectedBuildabilityInfo>";
        Equal(Check(original,p,"Browser='one two'",crush+projection+"<DisplayUpgrade>upgrade</DisplayUpgrade>").InstanceData,Check(original,p,"Browser='one two'","<DisplayUpgrade>upgrade</DisplayUpgrade>"+projection+crush).InstanceData,"Crusher/projection schema allocation order",false);
        Check(original,p,"Description='mixed'",projection+"<GameDependency RequiredModelConditionsAny='ALL'/><AutoResolveInfo/><ShadowInfo/>"+crush+"<VisionInfo/><ProjectedBuildabilityInfo RadiusY='-17'/>");
        foreach(string bad in new[]{"<CrusherInfo/><CrusherInfo/>","<CrusherInfo Unknown='1'/>","<CrusherInfo xmlns:f='foreign' f:CrusherLevel='1'/>","<CrusherInfo>text</CrusherInfo>","<CrusherInfo><Unknown/></CrusherInfo>","<CrusherInfo><ExtraCrushLevels Unknown='1'/></CrusherInfo>","<CrusherInfo><ExtraCrushLevels xmlns='foreign'/></CrusherInfo>","<CrusherInfo><ExtraCrushLevels>text</ExtraCrushLevels></CrusherInfo>","<CrusherInfo><ExtraCrushLevels><Unknown/></ExtraCrushLevels></CrusherInfo>","<ProjectedBuildabilityInfo PrimaryBuildability='true'/>","<ProjectedBuildabilityInfo Unknown='1'/>","<ProjectedBuildabilityInfo xmlns:f='foreign' f:Radius='1'/>","<ProjectedBuildabilityInfo>text</ProjectedBuildabilityInfo>","<ProjectedBuildabilityInfo><Unknown/></ProjectedBuildabilityInfo>","<ProjectedBuildabilityInfo><AllowedObjectFilter/><AllowedObjectFilter/></ProjectedBuildabilityInfo>","<ProjectedBuildabilityInfo><AllowedObjectFilter Unknown='1'/></ProjectedBuildabilityInfo>","<ProjectedBuildabilityInfo><AllowedObjectFilter xmlns='foreign'/></ProjectedBuildabilityInfo>","<ProjectedBuildabilityInfo><AllowedObjectFilter>text</AllowedObjectFilter></ProjectedBuildabilityInfo>","<ProjectedBuildabilityInfo><AllowedObjectFilter><IncludeThing Unknown='1'/></AllowedObjectFilter></ProjectedBuildabilityInfo>","<ProjectedBuildabilityInfo><AllowedObjectFilter><ExcludeThing><Unknown/></ExcludeThing></AllowedObjectFilter></ProjectedBuildabilityInfo>"})Reject(p,"",bad);
    }
    static void GeometryGroup(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        var b=Check(original,p,"","<Geometry/>");var d=b.InstanceData;
        int root=(int)Word(d,8,big)-4,obj=(int)Word(d,root+492,big);
        if(root!=1716 || obj!=2308 || d.Length!=2336)throw new Exception("Independent empty geometry allocation");
        foreach(int offset in new[]{0,4,8,12,16,20})if(Word(d,obj+offset,big)!=0)throw new Exception("Independent empty geometry state");
        b=Check(original,p,"","<Geometry><Shape/></Geometry>");d=b.InstanceData;root=(int)Word(d,8,big)-4;obj=(int)Word(d,root+492,big);int shape=(int)Word(d,obj+12,big);
        if(root!=1872 || obj!=2464 || shape!=2492 || d.Length!=2540 || Word(d,obj+8,big)!=1)throw new Exception("Independent shape allocation");
        if(Word(d,shape,big)!=0 || Word(d,shape+4,big)!=0 || Word(d,shape+16,big)!=0x3f800000u || Word(d,shape+20,big)!=0x3f800000u || Word(d,shape+32,big)!=0x3f800000u || d[shape+44]!=1 || d[shape+45]!=1)throw new Exception("Independent shape defaults");
        string[] numbers={"","0","-17","23","1.25","0x10","010","1e20","1e-20","text"," 23tail"};
        foreach(string field in new[]{"MajorRadius","MinorRadius","Height"})foreach(string value in numbers)foreach(string name in new[]{"Shape","PlacementShape"})Check(original,p,"","<Geometry><"+name+" "+field+"='"+value+"'/></Geometry>");
        foreach(string value in new[]{"","0r","180d","-90d","1.25r","2d1","5","text","1e20r"})foreach(string name in new[]{"Shape","PlacementShape"})Check(original,p,"","<Geometry><"+name+" FrontAngle='"+value+"'/></Geometry>");
        foreach(string field in new[]{"Active","UsedForHealthBox"})foreach(string value in new[]{"","true","false","TRUE","False","1","0","yes"," true"})Check(original,p,"","<Geometry><Shape "+field+"='"+value+"'/></Geometry>");
        foreach(string value in new[]{"","true","false","TRUE","False","1","0","yes"})Check(original,p,"","<Geometry IsSmall='"+value+"'/>");
        string[] types={"SPHERE","BOX","CYLINDER","RING","","UNKNOWN","sphere"," SPHERE","SPHERE ","-1"};
        foreach(string value in types)foreach(string name in new[]{"Shape","PlacementShape"})Check(original,p,"","<Geometry><"+name+" Type='"+value+"'/></Geometry>");
        foreach(string value in new[]{"NONE","VEHICLE","STRUCTURE","INFANTRY","SQUAD_MEMBER","","UNKNOWN","none"," NONE","NONE "})foreach(string name in new[]{"Shape","PlacementShape"})Check(original,p,"","<Geometry><"+name+" ContactPointGeneration='"+value+"'/></Geometry>");
        foreach(string value in new[]{"","a","abc","abcd","abcde","abcdefgh","with spaces","é ÿ 中文","A&B"}) {
            string text=Escape(value);
            Check(original,p,"","<Geometry><Shape Name='"+text+"' Other='"+text+"'/><PlacementShape Name='"+text+"' Other='"+text+"'/><ContactPoint Label='"+text+"'><Pos/></ContactPoint></Geometry>");
        }
        foreach(string value in numbers)foreach(string axis in new[]{"x","y","z"}) {
            Check(original,p,"","<Geometry><Shape><Offset "+axis+"='"+value+"'/></Shape><PlacementShape><Offset "+axis+"='"+value+"'/></PlacementShape><ContactPoint><Pos "+axis+"='"+value+"'/></ContactPoint></Geometry>");
            if(axis!="z")Check(original,p,"","<Geometry><RotationAnchorOffset "+axis+"='"+value+"'/><Shape/></Geometry>");
        }
        b=Check(original,p,"","<Geometry><RotationAnchorOffset x='17' y='23'/><Shape><Offset x='1' y='2' z='3'/></Shape><ContactPoint><Pos x='4' y='5' z='6'/></ContactPoint></Geometry>");d=b.InstanceData;root=(int)Word(d,8,big)-4;obj=(int)Word(d,root+492,big);int anchor=(int)Word(d,obj,big);shape=(int)Word(d,obj+12,big);int vector=(int)Word(d,shape+40,big),contact=(int)Word(d,obj+20,big);
        if(Word(d,anchor,big)!=0x41880000u || Word(d,anchor+4,big)!=0x41b80000u || Word(d,vector,big)!=0x3f800000u || Word(d,vector+8,big)!=0x40400000u || Word(d,contact+8,big)!=0x40800000u || Word(d,contact+16,big)!=0x40c00000u)throw new Exception("Independent geometry vector bindings");
        foreach(int count in new[]{1,2,3,17,64})foreach(bool contacts in new[]{false,true}) {
            string items="";for(int j=0;j<count;++j)items+=contacts?"<ContactPoint><Pos x='"+j+"'/></ContactPoint>":"<Shape MajorRadius='"+j+"'/>";
            b=Check(original,p,"","<Geometry>"+items+"</Geometry>");d=b.InstanceData;root=(int)Word(d,8,big)-4;obj=(int)Word(d,root+492,big);int list=obj+(contacts?16:8),elements=(int)Word(d,list+4,big);
            if(root!=1716+(contacts?48:156)*count || d.Length!=2336+(contacts?68:204)*count || Word(d,list,big)!=count)throw new Exception("Independent geometry list stride/allocation");
            for(int j=0;j<count;++j)if(Word(d,elements+(contacts?20:48)*j+(contacts?8:16),big)!=BitConverter.ToUInt32(BitConverter.GetBytes((float)j),0))throw new Exception("Independent geometry item values");
        }
        string anchorXml="<RotationAnchorOffset TypeId='2' x='23' y='-17'/>",placement="<PlacementShape TypeId='3' Type='BOX' Name='placement' Other='other'><Offset TypeId='4' x='1' y='2' z='3'/></PlacementShape>",s1="<Shape TypeId='5' Type='CYLINDER' Name='one'/>",s2="<Shape TypeId='6' Type='RING' Other='two'><Offset z='4'/></Shape>",c1="<ContactPoint TypeId='7' Label='first'><Pos TypeId='8' x='5'/></ContactPoint>",c2="<ContactPoint Label='second'><Pos y='6'/></ContactPoint>";
        Equal(Check(original,p,"","<Geometry TypeId='1' IsSmall='true'>"+anchorXml+placement+s1+s2+c1+c2+"</Geometry>").InstanceData,Check(original,p,"","<Geometry IsSmall='true'>"+c1+s1+placement+c2+anchorXml+s2+"</Geometry>").InstanceData,"Geometry schema allocation order",false);
        Check(original,p,"Description='mixed' Browser='a bb'","<VisionInfo/><CrusherInfo/><ProjectedBuildabilityInfo/><Geometry>"+anchorXml+placement+s1+s2+c1+c2+"</Geometry><AutoResolveInfo/><DisplayUpgrade>upgrade</DisplayUpgrade>");
        foreach(string bad in new[]{"<Geometry/><Geometry/>","<Geometry Unknown='1'/>","<Geometry xmlns='foreign'/>","<Geometry xmlns:f='foreign' f:IsSmall='true'/>","<Geometry>text</Geometry>","<Geometry><Unknown/></Geometry>","<Geometry><RotationAnchorOffset/><RotationAnchorOffset/></Geometry>","<Geometry><PlacementShape/><PlacementShape/></Geometry>","<Geometry><RotationAnchorOffset z='1'/></Geometry>","<Geometry><Shape Unknown='1'/></Geometry>","<Geometry><Shape xmlns='foreign'/></Geometry>","<Geometry><Shape><Offset/><Offset/></Shape></Geometry>","<Geometry><Shape><Offset Unknown='1'/></Shape></Geometry>","<Geometry><Shape><Offset><Unknown/></Offset></Shape></Geometry>","<Geometry><PlacementShape>text</PlacementShape></Geometry>","<Geometry><PlacementShape><Unknown/></PlacementShape></Geometry>","<Geometry><ContactPoint Unknown='1'/></Geometry>","<Geometry><ContactPoint><Pos/><Pos/></ContactPoint></Geometry>","<Geometry><ContactPoint><Pos xmlns='foreign'/></ContactPoint></Geometry>","<Geometry><ContactPoint><Pos>text</Pos></ContactPoint></Geometry>","<Geometry><ContactPoint><Offset/></ContactPoint></Geometry>"})Reject(p,"",bad);
    }
    static void AudioGroups(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        string[] arrays={"AudioArraySound","EvaEventArray","AudioArrayVoice"};
        foreach(string array in arrays) {
            var b=Check(original,p,"","<"+array+"/>");var d=b.InstanceData;int r=(int)Word(d,8,big)-4,o=(int)Word(d,r+(array=="AudioArraySound"?528:array=="EvaEventArray"?532:524),big);
            if(r!=(array=="AudioArrayVoice"?1704:1656) || o!=r+592 || d.Length!=o+(array=="AudioArrayVoice"?24:8))throw new Exception("Independent empty audio array allocation");
        }
        foreach(string value in "soundAfterburner soundAmbient soundAmbientBattle soundAmbientDamaged soundAmbientReallyDamaged soundAmbientRubble soundCreated soundCrushing soundDeploy soundDisabled soundDisabledOnCreation soundDisguiseRevealedFailure soundDisguiseRevealedSuccess soundDisguiseStarted soundEnter soundExit soundFalling soundImpact soundImpactCyclonic soundMoveLoop soundMoveLoopDamaged soundMoveStart soundMoveStartDamaged soundOnDamaged soundOnReallyDamaged soundPrepositioned soundPromotedElite soundPromotedHero soundPromotedVeteran soundReenabled soundStealthOff soundStealthOn soundTurretMoveLoop soundUndeploy soundUnderConstruction soundUnderRepairFromDamage soundUnderRepairFromRubble".Split(' '))Check(original,p,"","<AudioArraySound><AudioEntry AudioType='"+value+"'/></AudioArraySound>");
        foreach(string value in "ambushed availableForProduction buildCancelled buildOnHold cannotBuildDueToFullBuildQueue damagedAlly damagedByFireOwner damagedFromShroudedSourceOwner damagedOwner detectedAlly detectedEnemy detectedOwner dieAlly dieEnemy dieOwner enemyObjectSighted enemyObjectSightedAfterRespawn manuallyPoweredOff manuallyPoweredOn onFirstSightingEnemy onFirstSightingNonEnemy placementFailed productionComplete productionStarted promotedOwner repairingOwner secondDamageFarFromFirstOwner secondDamageFarFromFirstScanRange secondDamageFarFromFirstTimeoutMS soldOwner".Split(' '))Check(original,p,"","<EvaEventArray><EvaEntry EvaType='"+value+"'/></EvaEventArray>");
        foreach(string value in "voiceAlert voiceAttack voiceAttackAfterMoving voiceAttackAir voiceAttackCharge voiceAttackGarrisonedStructure voiceAttackMachine voiceAttackStructure voiceBombard voiceBuildResponse voiceCaptureBuildingComplete voiceCombatDrop voiceCombineWithHorde voiceCreated voiceDefectingAway voiceDefectingTowards voiceDeliverRing voiceDesperateAttack voiceDock voiceEnter voiceEnterHostile voiceEnterStateAttack voiceEnterStateAttackAfterMoving voiceEnterStateAttackAir voiceEnterStateAttackCharge voiceEnterStateAttackGarrisonedStructure voiceEnterStateAttackMachine voiceEnterStateAttackStructure voiceEnterStateMove voiceEnterStateMoveLandToWater voiceEnterStateMoveOverWalls voiceEnterStateMoveToCamp voiceEnterStateMoveToHigherGround voiceEnterStateMoveWaterToLand voiceEnterStateMoveWhileAttacking voiceEnterStateRetreatToCastle voiceFear voiceFullyCreated voiceGarrison voiceGetHealed voiceGuard voiceLowFuel voiceMove voiceMoveLandToWater voiceMoveOverWalls voiceMoveToCamp voiceMoveToHigherGround voiceMoveWaterToLand voiceMoveWhileAttacking voiceNoBuild voicePrimaryWeaponMode voiceRapidFire voiceRepair voiceRetreatToCastle voiceSalvage voiceSecondaryWeaponMode voiceSelect voiceSelectBattle voiceSelectIdleWorker voiceSelectUnderConstruction voiceSelectUnderFire voiceSendToSlaughterhouse voiceStartCharging voiceSupply voiceTaskComplete voiceTertiaryWeaponMode voiceUnload".Split(' '))Check(original,p,"","<AudioArrayVoice><AudioEntry AudioType='"+value+"'/></AudioArrayVoice>");
        foreach(string value in "voiceAttackUnit voiceCreatedFrom voiceEnterStateAttackUnit voiceEnterStateEnterUnit voiceEnterUnit voiceFullyCreatedFrom".Split(' '))Check(original,p,"","<AudioArrayVoice><ObjectSpecificEntry AudioType='"+value+"'/></AudioArrayVoice>");
        string[] groups={"AudioArraySound","EvaEventArray","AudioArrayVoice","AudioArrayVoice","AudioArrayVoice"};
        string[] entries={"AudioEntry","EvaEntry","AudioEntry","ObjectSpecificEntry","NamedEntry"};
        int[] rootOffsets={528,532,524,524,524};
        for(int group=0;group<5;++group) {
            string array=groups[group],entry=entries[group];
            foreach(string value in new[]{"","UNKNOWN","unknown","voiceSelect"," voiceSelect","voiceSelect ","-1","0"})if(group!=4)Check(original,p,"","<"+array+"><"+entry+" "+(group==1?"EvaType":"AudioType")+"='"+value+"'/></"+array+">");
            foreach(string value in new[]{"","Unresolved","Name\\0","Name\\1","Name\\23","Name\\-1","Name\\2147483647","Name\\2147483648","Name\\-2147483648","Name\\-2147483649","Name\\23tail","Name\\0x10","Name\\ 23","Name\\text"}) {
                string attr=group==1?"EvaEvent":"Sound";
                Check(original,p,"","<"+array+"><"+entry+" "+attr+"='"+value+"'/></"+array+">");
                if(group>=2)Check(original,p,"","<"+array+"><"+entry+" EvaEvent='"+value+"'/></"+array+">");
            }
            foreach(int count in new[]{1,2,3,17,64}) {
                string items="";for(int j=0;j<count;++j)items+="<"+entry+"/>";
                var b=Check(original,p,"","<"+array+">"+items+"</"+array+">");var d=b.InstanceData;int r=(int)Word(d,8,big)-4,o=(int)Word(d,r+rootOffsets[group],big),list=o+(group==3?8:group==4?16:0);
                int tokenStride=group<2?48:group==3?96:84,dataStride=group<2?8:group==3?16:12;
                if(r!=(group<2?1656:1704)+tokenStride*count || d.Length!=(group<2?2256:2320)+(tokenStride+dataStride)*count || Word(d,list,big)!=count)throw new Exception("Independent audio list allocation "+entry);
            }
        }
        foreach(string value in new[]{"","a","A","ab","AB","abc","AbC","abcd","AbCd","abcde","AbCdE","abcdef","ABCDEF","abcdefg","AbCdEfG","abcdefgh","with spaces","é","ÿ","中文","A&B"}) {
            string text=Escape(value);
            Check(original,p,"","<AudioArrayVoice><NamedEntry Name='"+text+"'/><ObjectSpecificEntry TargetObject='"+text+"'/></AudioArrayVoice>");
        }
        var lower=Check(original,p,"","<AudioArrayVoice><NamedEntry Name='abcd'/><ObjectSpecificEntry TargetObject='abcd'/></AudioArrayVoice>").InstanceData;
        var upper=Check(original,p,"","<AudioArrayVoice><NamedEntry Name='AbCd'/><ObjectSpecificEntry TargetObject='AbCd'/></AudioArrayVoice>").InstanceData;
        int rr=(int)Word(lower,8,big)-4,oo=(int)Word(lower,rr+524,big),named=(int)Word(lower,oo+20,big),specific=(int)Word(lower,oo+12,big);
        if(Word(lower,named+8,big)==Word(upper,named+8,big) || Word(lower,specific+12,big)!=Word(upper,specific+12,big))throw new Exception("Independent named voice case sensitivity / asset case folding");
        var present=Check(original,p,"","<AudioArrayVoice><AudioEntry Sound='' EvaEvent='E\\23'/></AudioArrayVoice>");var bytes=present.InstanceData;rr=(int)Word(bytes,8,big)-4;oo=(int)Word(bytes,rr+524,big);int item=(int)Word(bytes,oo+4,big),sound=(int)Word(bytes,item,big);
        if(sound==0 || Word(bytes,sound,big)!=0 || Word(bytes,item+4,big)!=24)throw new Exception("Independent allocated empty sound reference / EVA index");
        string voice="<AudioEntry TypeId='2' AudioType='voiceSelect' Sound='s\\23' EvaEvent='e\\-1'/><ObjectSpecificEntry TypeId='3' AudioType='voiceEnterUnit' TargetObject='Tank' Sound='o\\4'/><NamedEntry TypeId='4' Name='MiXeD' EvaEvent='e\\5'/>";
        string interleaved="<NamedEntry Name='MiXeD' EvaEvent='e\\5'/><ObjectSpecificEntry AudioType='voiceEnterUnit' TargetObject='Tank' Sound='o\\4'/><AudioEntry AudioType='voiceSelect' Sound='s\\23' EvaEvent='e\\-1'/>";
        Equal(Check(original,p,"","<AudioArrayVoice TypeId='1'>"+voice+"</AudioArrayVoice>").InstanceData,Check(original,p,"","<AudioArrayVoice>"+interleaved+"</AudioArrayVoice>").InstanceData,"Voice list schema allocation order",false);
        Check(original,p,"Browser='a bb' Description='mixed'","<AudioArrayVoice>"+voice+"</AudioArrayVoice><AudioArraySound TypeId='5'><AudioEntry TypeId='6' Sound='s\\23' AudioType='soundAmbient'/><AudioEntry/></AudioArraySound><EvaEventArray TypeId='7'><EvaEntry TypeId='8' EvaEvent='e\\4' EvaType='dieOwner'/></EvaEventArray><Geometry><Shape Name='shape'/></Geometry><CrusherInfo/><ProjectedBuildabilityInfo/><ShadowInfo/>");
        foreach(string array in arrays) {
            Reject(p,"","<"+array+"/><"+array+"/>");Reject(p,"","<"+array+" Unknown='1'/>");Reject(p,"","<"+array+" xmlns='foreign'/>");Reject(p,"","<"+array+">text</"+array+">");Reject(p,"","<"+array+"><Unknown/></"+array+">");
            string entry=array=="EvaEventArray"?"EvaEntry":"AudioEntry";
            foreach(string body in new[]{"<"+entry+" Unknown='1'/>","<"+entry+" xmlns='foreign'/>","<"+entry+" xmlns:f='foreign' f:Sound='x'/>","<"+entry+">text</"+entry+">","<"+entry+"><Unknown/></"+entry+">"})Reject(p,"","<"+array+">"+body+"</"+array+">");
        }
        foreach(string bad in new[]{"<AudioArrayVoice><AudioEntry Name='wrong'/></AudioArrayVoice>","<AudioArrayVoice><NamedEntry AudioType='voiceSelect'/></AudioArrayVoice>","<AudioArrayVoice><NamedEntry TargetObject='x'/></AudioArrayVoice>","<AudioArrayVoice><ObjectSpecificEntry Name='wrong'/></AudioArrayVoice>","<AudioArraySound><AudioEntry EvaEvent='wrong'/></AudioArraySound>","<EvaEventArray><EvaEntry Sound='wrong'/></EvaEventArray>"})Reject(p,"",bad);
    }
    static string AIWrap(int group,string attrs,string body="") {
        string part=group==0?body:group==1?"<AIContextualPower "+attrs+">"+body+"</AIContextualPower>":group==2?"<ConstructionBaseSelectionCriteria "+attrs+">"+body+"</ConstructionBaseSelectionCriteria>":"<AIContextualPower><SpecialPowerWeapon "+attrs+">"+body+"</SpecialPowerWeapon></AIContextualPower>";
        return "<SkirmishAIInformation "+(group==0?attrs:"")+">"+part+"</SkirmishAIInformation>";
    }
    static void SkirmishAIGroup(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        foreach(var f in AIFields) {
            int group=int.Parse(f[0]);string name=f[1],kind=f[2];
            string[] values=kind.IndexOf("Boolean")>=0?new[]{"","true","false","TRUE","False","1","0","yes"," true"}:
                kind.IndexOf("Reference")>=0?new[]{"","Unresolved","Name\\0","Name\\1","Name\\23","Name\\-1","Name\\2147483647","Name\\2147483648","Name\\-2147483648","Name\\-2147483649","Name\\+17","Name\\23trailing","Name\\0x10","Name\\ 23","Name\\text"}:
                kind=="Time"?new[]{"","0s","-17s","23ms","1000ms","1.25s","2d1s","text"}:
                kind=="Angle"?new[]{"","0r","180d","-90d","1.25r","2d1","text"}:
                kind.IndexOf("Bits")>=0?new[]{""," ","UNKNOWN","unknown","ALL","all","-ALL","+ALL","ALL -UNKNOWN","UNKNOWN ALL unknown","-UNKNOWN -UNKNOWN","+","-","A B;C,D"}:
                kind=="Enum"?new[]{"","UNKNOWN","unknown","0","-1","ALL","1"," 1"}:
                new[]{"","0","-0","1.25","-17","0x10","010","2147483647","-2147483648","4294967295","4294967296","-2147483649","1e39","nan","inf"," 23tail","text"};
            foreach(string v in values) {
                string xml=AIWrap(group,name+"='"+Escape(v)+"'");Check(original,p,"",xml);
                if(group==2)Check(original,p,"",xml.Replace("ConstructionBaseSelectionCriteria","ResourceNodeBaseClosenessTestSelectionCriteria"));
            }
        }
        foreach(var e in AIEnumTests) foreach(string value in e[2].Split('|')) {
            int group=int.Parse(e[0]);string field=e[1];
            Check(original,p,"",AIWrap(group,field+"='"+Escape(value)+"'"));
            Check(original,p,"",AIWrap(group,field+"='"+Escape(value.ToLowerInvariant())+"'"));
            Check(original,p,"",AIWrap(group,field+"=' "+Escape(value)+" '"));
            if(e[3]=="bits")Check(original,p,"",AIWrap(group,field+"='-"+Escape(value)+" -"+Escape(value)+" +"+Escape(value)+"'"));
        }
        for(int g=0;g<4;++g) {
            string attrs="";foreach(var f in AIFields)if(int.Parse(f[0])==g)attrs+=f[1]+"='"+(f[2].IndexOf("Boolean")>=0?"true":f[2].IndexOf("Reference")>=0?"Test\\23":f[2].IndexOf("Bits")>=0?"ALL":f[2]=="Angle"?"90d":f[2]=="Time"?"2s":"23")+"' ";
            Check(original,p,"",AIWrap(g,attrs));
        }
        string filter="<ObjectFilter TypeId='4' Include='VEHICLE' Exclude='STRUCTURE' StatusBitFlags='ALL'><IncludeThing TypeId='5'>Tank</IncludeThing><ExcludeThing>Ship</ExcludeThing></ObjectFilter>";
        string power="<AIContextualPower TypeId='2' id='metadata' Heuristic='UNKNOWN' Manager='UNKNOWN' UseOnlyWhenPowerAdvantageIsAbove='0' NextLogicCommand='C\\23' UpdateObject='' MaxUpdatables='-1' UseToCounterGarrison='false'>"+filter+"<SpecialPowerWeapon TypeId='3' Weapon='W\\0' ToggleShortRangeTargetForcesTarget='true'/></AIContextualPower>";
        string base1="<ConstructionBaseSelectionCriteria TypeId='6' RequiredBaseTypes='ALL'/>";
        string base2="<ResourceNodeBaseClosenessTestSelectionCriteria TypeId='7' ExcludedBaseTypes='PRIMARY_TYPE_MAX CAPTURED'/>";
        string dodge=filter.Replace("ObjectFilter","DodgeWithUnits");
        foreach(int count in new[]{0,1,2,7,32}) {
            string body="";for(int i=0;i<count;++i)body+=i%2==0?power:"<AIContextualPower/>";
            string xml=AIWrap(0,"TypeId='1' ConquerMetricsOverrideDPS='0' ConquerMetricsOverrideDefendingAntiMask='' OverrideInnerDistanceToTarget='0' OverrideOuterDistanceToTarget='0'",body+base1+base2+dodge);
            var a=Check(original,p,"Browser='a bb' Description='mixed'","<Flammability Fuel='1'/>"+xml+"<Geometry><Shape/></Geometry><AudioArrayVoice/>");
            var b=Check(original,p,"Description='mixed' Browser='a bb'","<AudioArrayVoice/><Geometry><Shape/></Geometry>"+xml+"<Flammability Fuel='1'/>");
            Equal(a.InstanceData,b.InstanceData,"AI mixed XML order instance",false);Equal(a.RelocationData,b.RelocationData,"AI mixed XML order relocations",false);
        }
        var forward=Check(original,p,"",AIWrap(0,"",power+base1+base2+dodge));
        var reverse=Check(original,p,"",AIWrap(0,"",dodge+base2+base1+power));
        Equal(forward.InstanceData,reverse.InstanceData,"AI schema allocation order",false);Equal(forward.RelocationData,reverse.RelocationData,"AI schema relocation order",false);
        // Independent default layout and table sentinel checks, separate from metadata.
        var empty=Check(original,p,"","<SkirmishAIInformation/>");byte[] bytes=empty.InstanceData;
        int root=(int)Word(bytes,8,big)-4,ai=(int)Word(bytes,root+460,big),table=(int)Word(bytes,Token(bytes,0,465968874u,big)+8,big);
        if(ai!=root+592 || bytes.Length!=ai+88 || Word(bytes,ai,big)!=0 || Word(bytes,ai+8,big)!=3 || Word(bytes,ai+24,big)!=14 || Word(bytes,ai+28,big)!=3 || Word(bytes,ai+40,big)!=0x3f800000u || Word(bytes,ai+48,big)!=0x43960000u || bytes[ai+80]!=1)throw new Exception("Independent AI defaults/layout");
        for(int i=0;i<12;++i)if(bytes[table+25*12+i]!=0)throw new Exception("Independent AI token sentinel");
        var nullable=Check(original,p,"",AIWrap(0,"ConquerMetricsOverrideDPS='0' ConquerMetricsOverrideDefendingAntiMask='ALL' OverrideInnerDistanceToTarget='0' OverrideOuterDistanceToTarget='0'"));
        bytes=nullable.InstanceData;root=(int)Word(bytes,8,big)-4;ai=(int)Word(bytes,root+460,big);
        int allocation=ai+88;foreach(int off in new[]{20,36,52,56}) {if(Word(bytes,ai+off,big)!=allocation)throw new Exception("Independent AI nullable allocation order");allocation+=4;}
        if(Word(bytes,(int)Word(bytes,ai+36,big),big)!=0xffffffffu)throw new Exception("Independent AI bit padding");
        var special=Check(original,p,"",AIWrap(1,"NextLogicCommand='Cmd\\23' UpdateObject='' UseToCounterGarrison='false' MaxUpdatables='0'", "<SpecialPowerWeapon Weapon='W\\1'/>"));
        bytes=special.InstanceData;root=(int)Word(bytes,8,big)-4;ai=(int)Word(bytes,root+460,big);
        int powerAt=(int)Word(bytes,ai+64,big),cmd=(int)Word(bytes,powerAt+128,big),update=(int)Word(bytes,powerAt+136,big),max=(int)Word(bytes,powerAt+148,big),counter=(int)Word(bytes,powerAt+152,big),weapon=(int)Word(bytes,powerAt+160,big);
        if(Word(bytes,ai+60,big)!=1 || cmd==0 || update==0 || max==0 || counter==0 || weapon==0 || Word(bytes,cmd,big)!=24 || Word(bytes,update,big)!=0 || Word(bytes,max,big)!=0 || bytes[counter]!=0 || Word(bytes,weapon,big)!=2 || bytes[powerAt+166]!=1)throw new Exception("Independent AI nullable reference/bool/int and weapon bindings");
        table=(int)Word(bytes,Token(bytes,0,465968874u,big)+8,big);
        int countTable=(int)Word(bytes,Token(bytes,table,2328042926u,big)+8,big),powerTable=(int)Word(bytes,countTable+20,big);
        if(Word(bytes,Token(bytes,powerTable,531684559u,big)+8,big)!=cmd || Word(bytes,Token(bytes,powerTable,587544167u,big)+8,big)!=update || Word(bytes,Token(bytes,powerTable,852392820u,big)+8,big)!=counter)throw new Exception("Independent AI nullable token pointees");
        for(int i=0;i<12;++i)if(bytes[powerTable+30*12+i]!=0)throw new Exception("Independent power token sentinel");
        var unknownDamage=Check(original,p,"",AIWrap(0,"ConquerMetricsOverrideDamageType='UNKNOWN'"));bytes=unknownDamage.InstanceData;root=(int)Word(bytes,8,big)-4;ai=(int)Word(bytes,root+460,big);
        if(Word(bytes,ai+24,big)!=38)throw new Exception("Independent AI damage fallback distinct from UNDEFINED");
        foreach(string node in new[]{"SkirmishAIInformation","AIContextualPower","ConstructionBaseSelectionCriteria","ResourceNodeBaseClosenessTestSelectionCriteria","SpecialPowerWeapon"}) {
            Func<string,string> wrap=delegate(string value){return node=="SkirmishAIInformation"?value:node=="SpecialPowerWeapon"?AIWrap(0,"","<AIContextualPower>"+value+"</AIContextualPower>"):AIWrap(0,"",value);};
            foreach(string bad in new[]{"<"+node+" Unknown='1'/>","<"+node+" xmlns='foreign'/>","<"+node+" xmlns:f='foreign' f:TypeId='1'/>","<"+node+">text</"+node+">","<"+node+"><Unknown/></"+node+">"})Reject(p,"",wrap(bad));
            if(node!="AIContextualPower")Reject(p,"",wrap("<"+node+"/><"+node+"/>"));
        }
        foreach(string bad in new[]{"<ObjectFilter/><ObjectFilter/>","<ObjectFilter Unknown='1'/>","<ObjectFilter>text</ObjectFilter>","<ObjectFilter><IncludeThing Unknown='1'/></ObjectFilter>","<ObjectFilter><ExcludeThing><Unknown/></ExcludeThing></ObjectFilter>","<ObjectFilter><IncludeThing xmlns='foreign'/></ObjectFilter>","<SpecialPowerWeapon/><SpecialPowerWeapon/>"})Reject(p,"",AIWrap(1,"",bad));
        foreach(string bad in new[]{"<DodgeWithUnits/><DodgeWithUnits/>","<DodgeWithUnits Unknown='1'/>","<DodgeWithUnits xmlns='foreign'/>","<DodgeWithUnits>text</DodgeWithUnits>","<DodgeWithUnits><IncludeThing Unknown='1'/></DodgeWithUnits>","<DodgeWithUnits><ExcludeThing><Unknown/></ExcludeThing></DodgeWithUnits>"})Reject(p,"",AIWrap(0,"",bad));
    }

    static void EmptyModuleMarkers(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        foreach(string name in new[]{"ReplaceModule","InheritableModule"}) {
            foreach(string typeId in new[]{"","0","1","4294967295","UNKNOWN"}) {
                string xml="<"+name+" TypeId='"+typeId+"'/>";var b=Check(original,p,"",xml);byte[] bytes=b.InstanceData;
                int root=(int)Word(bytes,8,big)-4,off=name=="ReplaceModule"?496:500;
                if(Word(bytes,root+off,big)!=root+592 || bytes.Length!=root+596 || Word(bytes,root+592,big)!=0 || Word(bytes,Token(bytes,0,name=="ReplaceModule"?1471645405u:3786889584u,big)+8,big)!=0 || b.RelocationData.Length!=8 || Word(b.RelocationData,0,big)!=root+off || Word(b.RelocationData,4,big)!=0xffffffffu)throw new Exception("Independent empty module marker layout/Token/relocation");
            }
            foreach(string bad in new[]{"<"+name+"/><"+name+"/>","<"+name+" id='x'/>","<"+name+" Unknown='1'/>","<"+name+" xmlns='foreign'/>","<"+name+" xmlns:f='foreign' f:TypeId='1'/>","<"+name+">text</"+name+">","<"+name+"><Unknown/></"+name+">"})Reject(p,"",bad);
        }
        string before="<SkirmishAIInformation ConquerMetricsOverrideDPS='0'/><Geometry><Shape/></Geometry>",after="<AutoResolveArmor Armor='A\\23'/><AutoResolveWeapon Weapon='W\\1'/><VisionInfo/>";
        var a=Check(original,p,"",before+"<ReplaceModule/><InheritableModule/>"+after);
        var reverse=Check(original,p,"",after+"<InheritableModule/><ReplaceModule/>"+before);
        Equal(a.InstanceData,reverse.InstanceData,"Marker schema order",false);Equal(a.RelocationData,reverse.RelocationData,"Marker relocation order",false);
        byte[] b2=a.InstanceData;int r=(int)Word(b2,8,big)-4;
        if(Word(b2,r+500,big)!=Word(b2,r+496,big)+4 || Word(b2,r+504,big)!=Word(b2,r+500,big)+4)throw new Exception("Independent marker four-byte padding and following allocation");
    }
    static string ModuleXML(string container,string node,string attrs) {return "<"+container+"><"+node+" "+attrs+"/></"+container+">";}
    static void ModuleFoundation(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        foreach(var container in ModuleContainers) {
            string parent=container[0],node=container[1];Check(original,p,"","<"+parent+"/>");
            foreach(string type in BaseModuleTypes) foreach(string id in new[]{"","a","AB","abc","abcd","abcde","MiXeDCaSe","with spaces","中文","é"})Check(original,p,"",ModuleXML(parent,node,"TypeId='"+type+"' id='"+Escape(id)+"'"));
            foreach(string type in UnknownModuleTypes)Check(original,p,"",ModuleXML(parent,node,"TypeId='"+type+"' id='defaultCase'"));
            foreach(string type in new[]{"","0","1","4294967295","4294967296","-1","-2147483649","0x10","010"," 23tail","UNKNOWN"})Check(original,p,"",ModuleXML(parent,node,"TypeId='"+type+"' id='fallback'"));
            Check(original,p,"",ModuleXML(parent,node,"id='missingTypeId'"));
            foreach(string type in UnsupportedModuleTypes)Reject(p,"",ModuleXML(parent,node,"TypeId='"+type+"'"));
            foreach(string bad in new[]{"<"+parent+"/><"+parent+"/>","<"+parent+" Unknown='1'/>","<"+parent+" xmlns='foreign'/>","<"+parent+" xmlns:f='foreign' f:TypeId='1'/>","<"+parent+">text</"+parent+">","<"+parent+"><Unknown/></"+parent+">","<"+parent+"><"+node+" xmlns='foreign'/></"+parent+">","<"+parent+"><"+node+" Unknown='1'/></"+parent+">","<"+parent+"><"+node+" xmlns:f='foreign' f:id='x'/></"+parent+">","<"+parent+"><"+node+">text</"+node+"></"+parent+">","<"+parent+"><"+node+"><Unknown/></"+node+"></"+parent+">"})Reject(p,"",bad);
            if(parent=="Body") {Reject(p,"","<Body><"+node+"/><"+node+"/></Body>");continue;}
            foreach(int count in new[]{0,1,2,27,28,29,65}) {
                string body="";for(int i=0;i<count;++i)body+="<"+node+" TypeId='"+BaseModuleTypes[i%BaseModuleTypes.Length]+"' id='Item"+i+"'/>";
                Check(original,p,"Browser='a bb'","<"+parent+">"+body+"</"+parent+">");
            }
        }
        string mixed="";foreach(var c in ModuleContainers)mixed+=ModuleXML(c[0],c[1],"TypeId='"+BaseModuleTypes[0]+"' id='mixed'");
        var a=Check(original,p,"Description='mixed'","<SkirmishAIInformation ConquerMetricsOverrideDPS='0'/>"+mixed+"<Geometry/><ReplaceModule/><InheritableModule/>");
        string reverse="";for(int i=ModuleContainers.Length-1;i>=0;--i)reverse+=ModuleXML(ModuleContainers[i][0],ModuleContainers[i][1],"TypeId='"+BaseModuleTypes[0]+"' id='mixed'");
        var b=Check(original,p,"Description='mixed'","<InheritableModule/><ReplaceModule/><Geometry/>"+reverse+"<SkirmishAIInformation ConquerMetricsOverrideDPS='0'/>");
        Equal(a.InstanceData,b.InstanceData,"Module container schema order",false);Equal(a.RelocationData,b.RelocationData,"Module container relocation order",false);
        // Concrete Layout: unknown module is eight bytes, output TypeId=0 and a
        // single 12-byte polymorphic token wrapper with no base token subtable.
        var fallback=Check(original,p,"","<Body><InactiveBody TypeId='1' id='ab'/></Body>");
        byte[] data=fallback.InstanceData;int root=(int)Word(data,8,big)-4,at=(int)Word(data,root+480,big);
        int bodyToken=Token(data,0,1868326721u,big); // original Body name/type hash
        int wrapper=(int)Word(data,bodyToken+8,big);
        if(at!=root+592 || data.Length!=at+8 || Word(data,at,big)!=0 || Word(data,at+4,big)==0 || Word(data,wrapper,big)!=2 || Word(data,wrapper+4,big)!=0 || Word(data,wrapper+8,big)!=0)throw new Exception("Independent unknown module layout/token wrapper");
        var lower=Check(original,p,"","<Body><InactiveBody TypeId='1' id='AB'/></Body>");Equal(fallback.InstanceData,lower.InstanceData,"Module typed id lowercasing",false);
        foreach(string type in BaseModuleTypes) {
            var known=Check(original,p,"",ModuleXML("Body","InactiveBody","TypeId='"+type+"' id='ab'"));data=known.InstanceData;root=(int)Word(data,8,big)-4;at=(int)Word(data,root+480,big);
            wrapper=(int)Word(data,Token(data,0,1868326721u,big)+8,big);
            if(Word(data,at,big)!=uint.Parse(type) || Word(data,wrapper,big)!=2 || Word(data,wrapper+4,big)!=uint.Parse(type) || Word(data,wrapper+8,big)==0)throw new Exception("Independent concrete module type/token binding");
        }
    }

    static string BodyXML(string node,string type,string attrs="",string children="") {
        return "<Body><"+node+" TypeId='"+type+"' id='BodyTest' "+attrs+">"+children+"</"+node+"></Body>";
    }
    static void BodyFamily(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        foreach(var body in BodyTypes) {
            string node=body[0],type=body[1];Check(original,p,"",BodyXML(node,type));
            foreach(var f in BodyFields) {
                if(f[0]=="Shield" && node!="ShieldBody")continue;
                string name=f[1],kind=f[2];
                string[] values=kind=="Boolean"?new[]{"","true","false","TRUE","False","1","0","yes"," true"}:
                    kind=="Reference" || kind=="NullableReference"?new[]{"","Unresolved","A\\0","B\\1","C\\23","D\\-1","E\\2147483647","F\\2147483648","G\\-2147483648","H\\-2147483649","I\\+17","J\\23trailing","K\\0x10","L\\ 23","M\\text"}:
                    kind=="String"?new[]{"","a","ab","abc","abcd","abcde","with spaces","中文测试","é ÿ","A&amp;B"}:
                    kind=="Time"?new[]{"","0s","-17s","23ms","1000ms","1.25s","2d1s","text"}:
                    new[]{"","0","-0","1.25","-17","0x10","010","2147483647","-2147483648","4294967295","4294967296","-2147483649","1e39","nan","inf"," 23tail","text","33.333333","66.666666","200.00002","200.00004"};
                foreach(string value in values)Check(original,p,"",BodyXML(node,type,name+"='"+Escape(value)+"'"));
            }
            foreach(string field in new[]{"Min","Max","AttributeModifier"}) foreach(string value in new[]{"","0","1","-17","33.333333","66.666666","1e39","text","Name\\0","Name\\23","Name\\-1","Name\\2147483648"})Check(original,p,"",BodyXML(node,type,"","<DamageRangeModifier "+field+"='"+Escape(value)+"'/>"));
            foreach(int count in new[]{0,1,2,27,28,29,65}) {
                string items="";for(int i=0;i<count;++i)items+="<DamageRangeModifier TypeId='7' Min='"+i+"' Max='"+(i+1)+"' AttributeModifier='A\\"+i+"'/>";
                string xml=BodyXML(node,type,"GrabObject='body' HealingBuffFX='H\\23' MaxHealth='123.5'",items);
                Check(original,p,"Browser='a bb' Description='mixed'",xml+"<SkirmishAIInformation ConquerMetricsOverrideDPS='0'/><Geometry/><ReplaceModule/>");
            }
            string all="";foreach(var f in BodyFields)if(f[0]!="Shield" || node=="ShieldBody")all+=f[1]+"='"+(f[2]=="Boolean"?"true":f[2]=="String"?"body":f[2].IndexOf("Reference")>=0?"Ref\\23":f[2]=="Time"?"2s":f[2]=="Enum"?"DAMAGED":"23.5")+"' ";
            string range="<DamageRangeModifier Min='12.5' Max='75' AttributeModifier='M\\1'/>";
            Check(original,p,"",BodyXML(node,type,all,range));
            foreach(var container in ModuleContainers) {
                string xml="<"+container[0]+"><"+container[1]+" TypeId='"+type+"' id='viaContainer' "+all+">"+range+"</"+container[1]+"></"+container[0]+">";
                Check(original,p,"",xml);
            }
            foreach(string bad in new[]{"<DamageRangeModifier Unknown='1'/>","<DamageRangeModifier xmlns='foreign'/>","<DamageRangeModifier xmlns:f='foreign' f:Min='1'/>","<DamageRangeModifier>text</DamageRangeModifier>","<DamageRangeModifier><Unknown/></DamageRangeModifier>","<Unknown/>"})Reject(p,"",BodyXML(node,type,"",bad));
            Reject(p,"",BodyXML(node,type,"Unknown='1'"));Reject(p,"",BodyXML(node,type,"xmlns:f='foreign' f:MaxHealth='1'"));
            if(node!="ShieldBody")Reject(p,"",BodyXML(node,type,"ShieldArmor='A'"));
            // Same concrete type and fields, reversed attribute/child ordering.
            var a=Check(original,p,"",BodyXML(node,type,"MaxHealth='123' GrabObject='abc'",range+"<DamageRangeModifier Min='25' Max='50'/>"));
            var b=Check(original,p,"",BodyXML(node,type,"GrabObject='abc' MaxHealth='123'",range+"<DamageRangeModifier Max='50' Min='25'/>"));
            Equal(a.InstanceData,b.InstanceData,"Body attribute order",false);Equal(a.RelocationData,b.RelocationData,"Body relocation order",false);
        }
        foreach(string value in BodyModelConditions) {
            Check(original,p,"",BodyXML("ShieldBody","3545651287","ShieldActiveModelCondition='"+Escape(value)+"'"));
            Check(original,p,"",BodyXML("ShieldBody","3545651287","ShieldActiveModelCondition='"+Escape(value.ToLowerInvariant())+"'"));
        }
        foreach(string value in new[]{"","UNKNOWN","INVALID","INVALID "," INVALID","0","-1","ALL"})Check(original,p,"",BodyXML("ShieldBody","3545651287","ShieldActiveModelCondition='"+value+"'"));
        // Independent explicit offsets and allocation order, not derived from metadata.
        var shield=Check(original,p,"",BodyXML("ShieldBody","3545651287","ShieldArmor='A\\23' GrabObject='xy' DodgePercent='25' MaxHealthDamagedPercentage='25' ShieldAmount='17'","<DamageRangeModifier Min='25' Max='50' AttributeModifier='M\\1'/>"));
        byte[] bytes=shield.InstanceData;int root=(int)Word(bytes,8,big)-4,at=(int)Word(bytes,root+480,big);
        if(Word(bytes,at,big)!=3545651287u || Word(bytes,at+12,big)!=0x3e800000u || Word(bytes,at+24,big)!=0x41c80000u || Word(bytes,at+36,big)!=2 || Word(bytes,at+40,big)!=at+112 || Word(bytes,at+92,big)!=0x41880000u || Word(bytes,at+96,big)!=at+108 || Word(bytes,at+64,big)!=1 || Word(bytes,at+68,big)!=at+116 || Word(bytes,at+108,big)!=24 || Word(bytes,at+116,big)!=0x3e800000u || Word(bytes,at+120,big)!=0x3f000000u || Word(bytes,at+124,big)!=2)throw new Exception("Independent Shield/Active field and allocation order");
        int wrapper=(int)Word(bytes,Token(bytes,0,1868326721u,big)+8,big),table=(int)Word(bytes,wrapper+8,big);
        if(Word(bytes,Token(bytes,table,317447484u,big)+8,big)!=at+108)throw new Exception("Independent nullable ShieldArmor Token pointee");
        for(int i=0;i<12;++i)if(bytes[table+9*12+i]!=0)throw new Exception("Independent Shield Token sentinel");
        var emptyArmor=Check(original,p,"",BodyXML("ShieldBody","3545651287","ShieldArmor=''"));bytes=emptyArmor.InstanceData;root=(int)Word(bytes,8,big)-4;at=(int)Word(bytes,root+480,big);
        if(Word(bytes,at+96,big)!=at+108 || Word(bytes,at+108,big)!=0)throw new Exception("Independent present empty ShieldArmor");
    }

    static string ClientXML(string node,string type,string attrs="",string children="") {
        return "<ClientUpdates><"+node+" TypeId='"+type+"' id='ClientTest' "+attrs+">"+children+"</"+node+"></ClientUpdates>";
    }
    static void ClientUpdateFamily(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;string mixed="";
        foreach(var client in ClientTypes) {
            string node=client[0],type=client[1];Check(original,p,"",ClientXML(node,type));
            string attrs="";
            foreach(var f in ClientFields)if(f[0]==node) {
                string name=f[1],kind=f[2];
                string[] values=kind=="Boolean"?new[]{"","true","false","TRUE","False","1","0","yes"," true"}:
                    kind=="Reference"?new[]{"","Unresolved","A\\0","B\\1","C\\23","D\\-1","E\\2147483647","F\\2147483648","G\\-2147483648","H\\-2147483649","I\\+17","J\\23trailing","K\\0x10","L\\ 23","M\\text"}:
                    kind=="String"?new[]{"","a","ab","abc","abcd","abcde","with spaces","中文测试","é ÿ","A&amp;B"}:
                    new[]{"","0","-0","1.25","-17","0x1234","010","65535","65536","65537","2147483647","-2147483648","4294967295","4294967296","-2147483649","1e39","nan","inf"," 23tail","text"};
                foreach(string value in values)Check(original,p,"",ClientXML(node,type,name+"='"+Escape(value)+"'"));
                attrs+=name+"='"+(kind=="Boolean"?"true":kind=="String"?"client":kind=="Reference"?"R\\23":"23")+"' ";
            }
            string children=node=="LargeGroupAudioClientUpdate"?"<Key TypeId='3'>AB</Key><Key>ab</Key><Key/>":"";
            Check(original,p,"",ClientXML(node,type,attrs,children));
            mixed+="<"+node+" TypeId='"+type+"' id='mix' "+attrs+">"+children+"</"+node+">";
            foreach(var container in ModuleContainers)Check(original,p,"","<"+container[0]+"><"+container[1]+" TypeId='"+type+"' id='viaContainer' "+attrs+">"+children+"</"+container[1]+"></"+container[0]+">");
            foreach(string bad in new[]{"<Unknown/>","<DamageRangeModifier/>","<ObjectFilter/>"})Reject(p,"",ClientXML(node,type,"",bad));
            if(node!="LargeGroupAudioClientUpdate")Reject(p,"",ClientXML(node,type,"","<Key>x</Key>"));
            Reject(p,"",ClientXML(node,type,"Unknown='1'"));Reject(p,"",ClientXML(node,type,"MaxHealth='1'"));
            Reject(p,"",ClientXML(node,type,"xmlns:f='foreign' f:TypeId='1'"));
        }
        foreach(int count in new[]{0,1,2,27,28,29,65}) {
            string items="";for(int i=0;i<count;++i)items+="<Key TypeId='7'>Item"+i+"</Key>";
            Check(original,p,"Browser='a bb'",ClientXML("LargeGroupAudioClientUpdate","2552668823","UnitWeight='0x1234'",items));
        }
        foreach(string value in new[]{""," ","a","A","ab","AB","abc","abcd","abcde","ABCDEF","a b"," a ","a,b;c","é","ÿ","中文","&amp;","line&#10;next"})Check(original,p,"",ClientXML("LargeGroupAudioClientUpdate","2552668823","","<Key>"+value+"</Key><Key>"+value+"</Key>"));
        foreach(string bad in new[]{"<Key Unknown='1'/>","<Key id='wrong'/>","<Key xmlns='foreign'/>","<Key xmlns:f='foreign' f:TypeId='1'/>","<Key><Unknown/></Key>"})Reject(p,"",ClientXML("LargeGroupAudioClientUpdate","2552668823","",bad));
        string list="<ClientUpdates>"+mixed+mixed+mixed+"</ClientUpdates>",body=BodyXML("ShieldBody","3545651287","ShieldArmor='A\\1' GrabObject='body'","<DamageRangeModifier Min='25' Max='50'/>");
        var a=Check(original,p,"Description='mixed'",list+body+"<SkirmishAIInformation/><Geometry/>");
        var b=Check(original,p,"Description='mixed'","<Geometry/><SkirmishAIInformation/>"+body+list);
        Equal(a.InstanceData,b.InstanceData,"Client/Body container order",false);Equal(a.RelocationData,b.RelocationData,"Client/Body relocation order",false);
        // Independent reference words, packed bools and uint overflow.
        var eva=Check(original,p,"",ClientXML("EvaAnnounceClientCreate","3505323485","AnnouncementEventEnemy='A\\23' AnnouncementEventAlly='B\\-1' AnnouncementEventOwner='C\\0' DelayBeforeAnnouncementLogicFrames='-1' OnlyIfVisible='true' CountAsFirstSightedAnnouncement='true' UseObjectsPosition='true' CreateFakeRadarEvent='true'"));
        byte[] bytes=eva.InstanceData;int root=(int)Word(bytes,8,big)-4,header=(int)Word(bytes,root+484,big),slots=(int)Word(bytes,header+4,big),at=(int)Word(bytes,slots,big);
        if(Word(bytes,at+8,big)!=24 || Word(bytes,at+12,big)!=0 || Word(bytes,at+16,big)!=1 || Word(bytes,at+20,big)!=0xffffffffu)throw new Exception("Independent EVA resources/uint");
        for(int i=24;i<28;++i)if(bytes[at+i]!=1)throw new Exception("Independent EVA packed bool");
        var laser=Check(original,p,"",ClientXML("LaserUpdate","3399407677","ParticleSystemId='P\\1' TargetParticleSystemId='T\\23' ParentFireBoneName='ab' ParentFireBoneOnTurret='true' LaserLifetime='2.5'"));
        bytes=laser.InstanceData;root=(int)Word(bytes,8,big)-4;header=(int)Word(bytes,root+484,big);slots=(int)Word(bytes,header+4,big);at=(int)Word(bytes,slots,big);
        if(Word(bytes,at+8,big)!=2 || Word(bytes,at+12,big)!=2 || Word(bytes,at+16,big)!=at+32 || Word(bytes,at+20,big)!=24 || Word(bytes,at+24,big)!=0x40200000u || bytes[at+28]!=1 || bytes.Length!=at+36)throw new Exception("Independent Laser layout/string/allocation");
        var audio=Check(original,p,"",ClientXML("LargeGroupAudioClientUpdate","2552668823","UnitWeight='0x1234' FramesBetweenUpdatesMin='0xfedcba98' FramesBetweenUpdatesVariation='010'","<Key>AB</Key><Key>ab</Key><Key/>"));
        bytes=audio.InstanceData;root=(int)Word(bytes,8,big)-4;header=(int)Word(bytes,root+484,big);slots=(int)Word(bytes,header+4,big);at=(int)Word(bytes,slots,big);int keys=(int)Word(bytes,at+20,big);
        if(Word(bytes,at+8,big)!=0xfedcba98u || Word(bytes,at+12,big)!=8 || Word(bytes,at+16,big)!=3 || keys!=at+28 || bytes[at+24]!=(big?0x12:0x34) || bytes[at+25]!=(big?0x34:0x12) || bytes[at+26]!=0 || bytes[at+27]!=0 || Word(bytes,keys,big)==Word(bytes,keys+4,big) || Word(bytes,keys+8,big)!=0)throw new Exception("Independent LargeGroupAudio narrow field/hash list");
    }

    static string SoundXML(string node,string type,string attrs="",string children="") {return "<ClientBehaviors><"+node+" TypeId='"+type+"' id='SoundTest' "+attrs+">"+children+"</"+node+"></ClientBehaviors>";}
    static void SoundPlaybackFamily(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        foreach(var sound in SoundTypes) {
            string node=sound[0],type=sound[1],item=node=="AnimationSoundClientBehavior"?"Sound":"ModelConditionSound";
            Check(original,p,"",SoundXML(node,type));Check(original,p,"",SoundXML(node,type,"","<"+item+"/>"));
            foreach(int count in new[]{0,1,2,27,28,29,65}) {
                string children="";for(int i=0;i<count;++i)children+=node=="AnimationSoundClientBehavior"?"<Sound TypeId='7' Sound='S\\"+i+"' RequiredMC='DAMAGED' ExcludedMC='' Animation='Walk' Frame='1 -1 010'/>":"<ModelConditionSound TypeId='7' Sound='S\\"+i+"' RequiredFlags='DAMAGED' ExcludedFlags='ALL'/>";
                Check(original,p,"Browser='a bb' Description='mixed'",SoundXML(node,type,"",children)+BodyXML("ShieldBody","3545651287","ShieldArmor='A\\23' GrabObject='body'")+ClientXML("LaserUpdate","3399407677","ParentFireBoneName='laser'"));
            }
            string[] maskNames=node=="AnimationSoundClientBehavior"?new[]{"RequiredMC","ExcludedMC"}:new[]{"RequiredFlags","ExcludedFlags"};
            foreach(string field in maskNames) {
                foreach(string value in BodyModelConditions)Check(original,p,"",SoundXML(node,type,"","<"+item+" "+field+"='"+Escape(value)+"'/>"));
                foreach(string value in new[]{""," ","UNKNOWN","unknown","ALL","all","-ALL","ALL -UNKNOWN","-DAMAGED -DAMAGED +DAMAGED","+","-","DAMAGED RUBBLE"})Check(original,p,"",SoundXML(node,type,"","<"+item+" "+field+"='"+value+"'/>"));
            }
            foreach(string value in new[]{"","Unresolved","A\\0","B\\1","C\\23","D\\-1","E\\2147483647","F\\2147483648","G\\-2147483648","H\\-2147483649","I\\+17","J\\23trailing","K\\0x10","L\\ 23","M\\text"})Check(original,p,"",SoundXML(node,type,"","<"+item+" Sound='"+Escape(value)+"'/>"));
            foreach(var container in ModuleContainers)Check(original,p,"","<"+container[0]+"><"+container[1]+" TypeId='"+type+"' id='viaContainer'><"+item+" Sound='S\\23'/></"+container[1]+"></"+container[0]+">");
            foreach(string bad in new[]{"<"+item+" Unknown='1'/>","<"+item+" id='wrong'/>","<"+item+" xmlns='foreign'/>","<"+item+" xmlns:f='foreign' f:TypeId='1'/>","<"+item+">text</"+item+">","<"+item+"><Unknown/></"+item+">","<Key/>","<DamageRangeModifier/>"})Reject(p,"",SoundXML(node,type,"",bad));
            Reject(p,"",SoundXML(node,type,"Unknown='1'"));Reject(p,"",SoundXML(node,type,"MaxHealth='1'"));
        }
        foreach(string value in new[]{"","0","-0","0.001","17","2147483647","1e39","nan","inf","text"})Check(original,p,"",SoundXML("AnimationSoundClientBehavior","670005533","MaxUpdateRangeCap='"+value+"'"));
        foreach(string value in new[]{"-1","-0.001","-2147483648","-1e39"}) {
            string xml=SoundXML("AnimationSoundClientBehavior","670005533","MaxUpdateRangeCap='"+value+"'");Exception expectedError=null,actualError=null;
            try{original.ProcessInstance(Declaration("",xml));}catch(Exception error){if(error.Message.IndexOf("Value should be >= 0",StringComparison.Ordinal)<0)throw;expectedError=error;}
            try{Recovered.GameObjectCompiler.ProcessInstance(Declaration("",xml),p);}catch(Exception error){actualError=error;}
            if(expectedError==null || actualError==null || expectedError.GetType()!=actualError.GetType() || expectedError.Message!=actualError.Message)throw new Exception("Positive-range exception type/message differs from original");
            foreach(var property in expectedError.GetType().GetProperties(BindingFlags.Public|BindingFlags.Instance))if(property.Name=="ErrorCode" || property.Name=="Code")if(!object.Equals(property.GetValue(expectedError,null),property.GetValue(actualError,null)))throw new Exception("Positive-range error code differs from original");
            ++refusals;
        }
        foreach(string field in new[]{"Animation","Frame"})foreach(string value in new[]{""," ","a","AB","ab","abc","abcd","中文","é","1","1 -1 010","0x10 2147483647 -2147483648 4294967295","1,2;3","text","+1  2   3"})Check(original,p,"",SoundXML("AnimationSoundClientBehavior","670005533","","<Sound "+field+"='"+Escape(value)+"'/>"));
        Check(original,p,"",SoundXML("AnimationSoundClientBehavior","670005533","","<Sound Frame='1&#9;2&#10;3&#13;4'/>"));
        Reject(p,"",SoundXML("ModelConditionAudioLoopClientBehavior","1514068980","MaxUpdateRangeCap='1'"));
        Reject(p,"",SoundXML("AnimationSoundClientBehavior","670005533","","<Sound RequiredFlags='ALL'/>"));
        Reject(p,"",SoundXML("ModelConditionAudioLoopClientBehavior","1514068980","","<ModelConditionSound Animation='A'/>"));
        var animation=Check(original,p,"",SoundXML("AnimationSoundClientBehavior","670005533","MaxUpdateRangeCap='0'","<Sound Sound='S\\23' RequiredMC='' ExcludedMC='ALL' Animation='AB' Frame='1 -1 010'/>"));
        byte[] bytes=animation.InstanceData;int root=(int)Word(bytes,8,big)-4,header=(int)Word(bytes,root+488,big),slots=(int)Word(bytes,header+4,big),at=(int)Word(bytes,slots,big),items=(int)Word(bytes,at+16,big);
        if(Word(bytes,at+8,big)!=at+20 || Word(bytes,at+20,big)!=0 || Word(bytes,at+12,big)!=1 || items!=at+24 || Word(bytes,items,big)!=24 || Word(bytes,items+4,big)!=at+48 || Word(bytes,items+8,big)!=at+108 || Word(bytes,items+16,big)!=3 || Word(bytes,items+20,big)!=at+168 || Word(bytes,at+168,big)!=1 || Word(bytes,at+172,big)!=0xffffffffu || Word(bytes,at+176,big)!=8)throw new Exception("Independent animation sound nullable/list/allocation layout");
        for(int i=0;i<15;++i)if(Word(bytes,at+48+4*i,big)!=0 || Word(bytes,at+108+4*i,big)!=0xffffffffu)throw new Exception("Independent animation sound empty/ALL masks");
        var lower=Check(original,p,"",SoundXML("AnimationSoundClientBehavior","670005533","MaxUpdateRangeCap='0'","<Sound Sound='S\\23' RequiredMC='' ExcludedMC='ALL' Animation='ab' Frame='1 -1 010'/>"));Equal(animation.InstanceData,lower.InstanceData,"Animation typed-id lowercasing",false);
        var loop=Check(original,p,"",SoundXML("ModelConditionAudioLoopClientBehavior","1514068980","","<ModelConditionSound Sound='' RequiredFlags='ALL'/>"));bytes=loop.InstanceData;root=(int)Word(bytes,8,big)-4;header=(int)Word(bytes,root+488,big);slots=(int)Word(bytes,header+4,big);at=(int)Word(bytes,slots,big);items=(int)Word(bytes,at+12,big);
        if(Word(bytes,at+8,big)!=1 || items!=at+16 || Word(bytes,items,big)!=at+140 || Word(bytes,at+140,big)!=0)throw new Exception("Independent model-condition loop nullable sound");
        for(int i=0;i<15;++i)if(Word(bytes,items+4+4*i,big)!=0xffffffffu || Word(bytes,items+64+4*i,big)!=0)throw new Exception("Independent loop inline masks/padding");
    }

    static string SelectorXML(int group,string attrs="",string children="") {
        string[] names={"RandomSoundSelectorClientBehavior","ModelConditionSoundSelectorClientBehavior","UpgradeSoundSelectorClientBehavior","ModelConditionEvaEventSelectorClientBehavior"};
        string[] types={"3030954484","847051638","4178664105","1640547431"};
        return SoundXML(names[group],types[group],attrs,children);
    }
    static void SoundSelectorFamily(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        string voice="<AudioArrayVoice><AudioEntry Sound='S\\23' EvaEvent='E\\-1' AudioType='voiceSelect'/><ObjectSpecificEntry Sound='' EvaEvent='' TargetObject='AbCd' AudioType='voiceAttackUnit'/><NamedEntry Name='AbCd' Sound='N\\0'/></AudioArrayVoice>";
        string sound="<AudioArraySound><AudioEntry Sound='S\\23' AudioType='soundAmbient'/><AudioEntry/></AudioArraySound>";
        string eva="<EvaEventArray><EvaEntry EvaEvent='E\\23' EvaType='dieOwner'/><EvaEntry/></EvaEventArray>";
        for(int group=0;group<4;++group) {
            Check(original,p,"",SelectorXML(group));
            string payload=group==3?eva:voice+sound;
            Check(original,p,"Browser='a bb'",SelectorXML(group,"",group==0?payload:"<Override>"+payload+"</Override>"));
            foreach(int count in new[]{0,1,2,27,28,29,65}) {
                string children="";
                for(int j=0;j<count;++j)children+=group==0?"<AudioEntry Sound='S\\23' AudioType='voiceSelect'/>":"<Override "+(group==3?"":"VoicePriorityOverride='-1'")+">"+payload+(group==2?"<RequiredUpgrade>UpA</RequiredUpgrade><ConflictingUpgrade>UpB</ConflictingUpgrade>":"")+"</Override>";
                if(group==0)children="<AudioArrayVoice>"+children+"</AudioArrayVoice><AudioArraySound/>";
                Check(original,p,"Description='mixed'",SelectorXML(group,"",children)+"<AudioArrayVoice/>"+BodyXML("ShieldBody","3545651287","ShieldArmor='A\\23'"));
            }
            if(group!=0)foreach(string field in group==2?new[]{"RequiredModelConditions","ConflictingModelConditions"}:new[]{"RequiredFlags","ExcludedFlags"}) {
                foreach(string value in BodyModelConditions)Check(original,p,"",SelectorXML(group,"","<Override "+field+"='"+Escape(value)+"'/>"));
                foreach(string value in new[]{""," ","UNKNOWN","unknown","ALL","all","-ALL","ALL -UNKNOWN","-DAMAGED -DAMAGED +DAMAGED","+","-","DAMAGED RUBBLE"})Check(original,p,"",SelectorXML(group,"","<Override "+field+"='"+value+"'/>"));
            }
            if(group<3)foreach(string value in new[]{"","0","-0","1","-1","010","0x10"," 23tail","2147483647","2147483648","-2147483648","-2147483649","4294967295","4294967296","text"})Check(original,p,"",SelectorXML(group,group==0?"VoicePriorityOverride='"+value+"'":"",group==0?"":"<Override VoicePriorityOverride='"+value+"'/>"));
            foreach(var container in ModuleContainers) {
                string type=group==0?"3030954484":group==1?"847051638":group==2?"4178664105":"1640547431";
                Check(original,p,"","<"+container[0]+"><"+container[1]+" TypeId='"+type+"'>"+(group==0?payload:"<Override>"+payload+"</Override>")+"</"+container[1]+"></"+container[0]+">");
            }
            foreach(string bad in new[]{"<Unknown/>","<Override Unknown='1'/>","<Override xmlns='foreign'/>","<Override>text</Override>","<Override><Unknown/></Override>"})Reject(p,"",SelectorXML(group,"",bad));
            Reject(p,"",SelectorXML(group,"Unknown='1'"));
            string entry=group==0?"":"<Override>",end=group==0?"":"</Override>";
            foreach(string bad in group==3?new[]{"<AudioArrayVoice/>","<EvaEventArray/><EvaEventArray/>","<EvaEventArray><EvaEntry Sound='wrong'/></EvaEventArray>"}:new[]{"<EvaEventArray/>","<AudioArrayVoice/><AudioArrayVoice/>","<AudioArraySound/><AudioArraySound/>","<AudioArraySound><AudioEntry><Unknown/></AudioEntry></AudioArraySound>","<AudioArrayVoice><AudioEntry Name='wrong'/></AudioArrayVoice>"})Reject(p,"",SelectorXML(group,"",entry+bad+end));
        }
        foreach(string field in new[]{"Chance","RerollOnEveryFrame"})foreach(string value in new[]{"","-1","0","1","50","100","123.45","1e39","nan","inf","text","true","false","TRUE"," true"})Check(original,p,"",SelectorXML(0,field+"='"+value+"'"));
        foreach(string value in new[]{"","Unresolved","A\\0","B\\1","C\\23","D\\-1","E\\2147483647","F\\2147483648","G\\-2147483648","H\\-2147483649","I\\+17","J\\23trailing","K\\0x10","L\\ 23","M\\text"})for(int group=0;group<4;++group) {
            string payload=group==3?"<EvaEventArray><EvaEntry EvaEvent='"+value+"'/></EvaEventArray>":"<AudioArrayVoice><AudioEntry Sound='"+value+"' EvaEvent='"+value+"'/><ObjectSpecificEntry Sound='"+value+"'/><NamedEntry Sound='"+value+"'/></AudioArrayVoice><AudioArraySound><AudioEntry Sound='"+value+"'/></AudioArraySound>";
            Check(original,p,"",SelectorXML(group,"",group==0?payload:"<Override>"+payload+"</Override>"));
        }
        foreach(string value in new[]{"","a","AB","ab","abc","abcd","abcde","MiXeD","with spaces","中文","é"}) {
            Check(original,p,"",SelectorXML(2,"","<Override><RequiredUpgrade>"+value+"</RequiredUpgrade><RequiredUpgrade>"+value+"</RequiredUpgrade><ConflictingUpgrade>"+value+"</ConflictingUpgrade>"+voice+sound+"</Override>"));
            for(int group=0;group<3;++group) {string payload="<AudioArrayVoice><NamedEntry Name='"+value+"'/><ObjectSpecificEntry TargetObject='"+value+"'/></AudioArrayVoice>";Check(original,p,"",SelectorXML(group,"",group==0?payload:"<Override>"+payload+"</Override>"));}
        }
        foreach(string value in "soundAfterburner soundAmbient soundAmbientBattle soundAmbientDamaged soundAmbientReallyDamaged soundAmbientRubble soundCreated soundCrushing soundDeploy soundDisabled soundDisabledOnCreation soundDisguiseRevealedFailure soundDisguiseRevealedSuccess soundDisguiseStarted soundEnter soundExit soundFalling soundImpact soundImpactCyclonic soundMoveLoop soundMoveLoopDamaged soundMoveStart soundMoveStartDamaged soundOnDamaged soundOnReallyDamaged soundPrepositioned soundPromotedElite soundPromotedHero soundPromotedVeteran soundReenabled soundStealthOff soundStealthOn soundTurretMoveLoop soundUndeploy soundUnderConstruction soundUnderRepairFromDamage soundUnderRepairFromRubble".Split(' '))foreach(int group in new[]{0,1,2}) {
            string payload="<AudioArraySound><AudioEntry AudioType='"+value+"'/></AudioArraySound>";
            Check(original,p,"",SelectorXML(group,"",group==0?payload:"<Override>"+payload+"</Override>"));
        }
        foreach(string value in "ambushed availableForProduction buildCancelled buildOnHold cannotBuildDueToFullBuildQueue damagedAlly damagedByFireOwner damagedFromShroudedSourceOwner damagedOwner detectedAlly detectedEnemy detectedOwner dieAlly dieEnemy dieOwner enemyObjectSighted enemyObjectSightedAfterRespawn manuallyPoweredOff manuallyPoweredOn onFirstSightingEnemy onFirstSightingNonEnemy placementFailed productionComplete productionStarted promotedOwner repairingOwner secondDamageFarFromFirstOwner secondDamageFarFromFirstScanRange secondDamageFarFromFirstTimeoutMS soldOwner".Split(' '))foreach(int group in new[]{3}) {
            string payload="<EvaEventArray><EvaEntry EvaType='"+value+"'/></EvaEventArray>";
            Check(original,p,"",SelectorXML(group,"",group==0?payload:"<Override>"+payload+"</Override>"));
        }
        foreach(string value in "voiceAlert voiceAttack voiceAttackAfterMoving voiceAttackAir voiceAttackCharge voiceAttackGarrisonedStructure voiceAttackMachine voiceAttackStructure voiceBombard voiceBuildResponse voiceCaptureBuildingComplete voiceCombatDrop voiceCombineWithHorde voiceCreated voiceDefectingAway voiceDefectingTowards voiceDeliverRing voiceDesperateAttack voiceDock voiceEnter voiceEnterHostile voiceEnterStateAttack voiceEnterStateAttackAfterMoving voiceEnterStateAttackAir voiceEnterStateAttackCharge voiceEnterStateAttackGarrisonedStructure voiceEnterStateAttackMachine voiceEnterStateAttackStructure voiceEnterStateMove voiceEnterStateMoveLandToWater voiceEnterStateMoveOverWalls voiceEnterStateMoveToCamp voiceEnterStateMoveToHigherGround voiceEnterStateMoveWaterToLand voiceEnterStateMoveWhileAttacking voiceEnterStateRetreatToCastle voiceFear voiceFullyCreated voiceGarrison voiceGetHealed voiceGuard voiceLowFuel voiceMove voiceMoveLandToWater voiceMoveOverWalls voiceMoveToCamp voiceMoveToHigherGround voiceMoveWaterToLand voiceMoveWhileAttacking voiceNoBuild voicePrimaryWeaponMode voiceRapidFire voiceRepair voiceRetreatToCastle voiceSalvage voiceSecondaryWeaponMode voiceSelect voiceSelectBattle voiceSelectIdleWorker voiceSelectUnderConstruction voiceSelectUnderFire voiceSendToSlaughterhouse voiceStartCharging voiceSupply voiceTaskComplete voiceTertiaryWeaponMode voiceUnload".Split(' '))foreach(int group in new[]{0,1,2}) {
            string payload="<AudioArrayVoice><AudioEntry AudioType='"+value+"'/></AudioArrayVoice>";
            Check(original,p,"",SelectorXML(group,"",group==0?payload:"<Override>"+payload+"</Override>"));
        }
        foreach(string value in "voiceAttackUnit voiceCreatedFrom voiceEnterStateAttackUnit voiceEnterStateEnterUnit voiceEnterUnit voiceFullyCreatedFrom".Split(' '))foreach(int group in new[]{0,1,2}) {
            string payload="<AudioArrayVoice><ObjectSpecificEntry AudioType='"+value+"'/></AudioArrayVoice>";
            Check(original,p,"",SelectorXML(group,"",group==0?payload:"<Override>"+payload+"</Override>"));
        }
        foreach(string value in new[]{"","UNKNOWN","unknown","voiceSelect"," voiceSelect","voiceSelect ","-1","0"})for(int group=0;group<4;++group) {
            string payload=group==3?"<EvaEventArray><EvaEntry EvaType='"+value+"'/></EvaEventArray>":"<AudioArrayVoice><AudioEntry AudioType='"+value+"'/><ObjectSpecificEntry AudioType='"+value+"'/></AudioArrayVoice><AudioArraySound><AudioEntry AudioType='"+value+"'/></AudioArraySound>";
            Check(original,p,"",SelectorXML(group,"",group==0?payload:"<Override>"+payload+"</Override>"));
        }
        var b=Check(original,p,"",SelectorXML(0,"VoicePriorityOverride='-1'","<AudioArrayVoice/><AudioArraySound/>"));var bytes=b.InstanceData;
        int root=(int)Word(bytes,8,big)-4,header=(int)Word(bytes,root+488,big),slots=(int)Word(bytes,header+4,big),at=(int)Word(bytes,slots,big);
        if(Word(bytes,at,big)!=3030954484u || Word(bytes,at+8,big)!=0x3f000000u || Word(bytes,at+12,big)!=at+28 || Word(bytes,at+28,big)!=0xffffffffu || Word(bytes,at+16,big)!=at+32 || Word(bytes,at+20,big)!=at+56 || bytes[at+24]!=0)throw new Exception("Independent random selector default/nullable/allocation layout");
        int listToken=(int)Word(bytes,Token(bytes,0,3007506658u,big)+8,big),wrapper=(int)Word(bytes,listToken+20,big),table=(int)Word(bytes,wrapper+8,big);
        if(Word(bytes,wrapper,big)!=2 || Word(bytes,wrapper+4,big)!=3030954484u || Word(bytes,table+72,big)!=0 || Word(bytes,table+76,big)!=0 || Word(bytes,table+80,big)!=0 || Word(bytes,Token(bytes,table,549620986u,big)+8,big)!=at+28)throw new Exception("Independent selector wrapper/nullable Token/sentinel");
        for(int group=1;group<4;++group) {
            b=Check(original,p,"",SelectorXML(group,"","<Override "+(group==2?"RequiredModelConditions":"RequiredFlags")+"='ALL'/><Override/>"));bytes=b.InstanceData;root=(int)Word(bytes,8,big)-4;header=(int)Word(bytes,root+488,big);slots=(int)Word(bytes,header+4,big);at=(int)Word(bytes,slots,big);int items=(int)Word(bytes,at+12,big),stride=group==1?132:group==2?148:124;
            if(Word(bytes,at+8,big)!=2 || items!=at+16 || bytes.Length!=items+2*stride)throw new Exception("Independent selector override count/stride");
            for(int j=0;j<15;++j)if(Word(bytes,items+4*j,big)!=0xffffffffu || Word(bytes,items+60+4*j,big)!=0 || Word(bytes,items+stride+4*j,big)!=0)throw new Exception("Independent selector inline ModelCondition masks");
        }
        string ordered=SelectorXML(2,"","<Override>"+voice+sound+"<RequiredUpgrade>UpA</RequiredUpgrade><ConflictingUpgrade>UpB</ConflictingUpgrade></Override>");
        string reversed=SelectorXML(2,"","<Override><ConflictingUpgrade>UpB</ConflictingUpgrade><RequiredUpgrade>UpA</RequiredUpgrade>"+sound+voice+"</Override>");
        var a=Check(original,p,"",ordered);b=Check(original,p,"",reversed);Equal(a.InstanceData,b.InstanceData,"Selector schema allocation order",false);Equal(a.RelocationData,b.RelocationData,"Selector relocation order",false);
        var upper=Check(original,p,"",SelectorXML(2,"","<Override><AudioArrayVoice><ObjectSpecificEntry TargetObject='AbCd'/></AudioArrayVoice><RequiredUpgrade>AbCd</RequiredUpgrade><ConflictingUpgrade>AbCd</ConflictingUpgrade></Override>"));
        var lower=Check(original,p,"",SelectorXML(2,"","<Override><AudioArrayVoice><ObjectSpecificEntry TargetObject='abcd'/></AudioArrayVoice><RequiredUpgrade>abcd</RequiredUpgrade><ConflictingUpgrade>abcd</ConflictingUpgrade></Override>"));
        Equal(upper.InstanceData,lower.InstanceData,"Selector typed IDs use lowercase",false);
        upper=Check(original,p,"",SelectorXML(0,"","<AudioArrayVoice><NamedEntry Name='AbCd'/></AudioArrayVoice>"));lower=Check(original,p,"",SelectorXML(0,"","<AudioArrayVoice><NamedEntry Name='abcd'/></AudioArrayVoice>"));
        bool same=true;for(int i=0;i<upper.InstanceData.Length;++i)if(upper.InstanceData[i]!=lower.InstanceData[i]){same=false;break;}if(same)throw new Exception("Selector NamedEntry must preserve case");

    }

    static readonly string[] LifecycleNames={"DeletionUpdate","DieOnProducerDeathUpdate","InvalidTargetsDeathUpdate","HeightDieUpdate","LoseHealthUpdate","LifetimeUpdate"};
    static readonly string[] LifecycleTypes={"1111529555","474181074","1461180892","1964506706","2974418213","1814576242"};
    static string LifecycleXML(int group,string attrs="",string children="") {
        return "<Behaviors><"+LifecycleNames[group]+" TypeId='"+LifecycleTypes[group]+"' id='LifeTest' "+attrs+">"+children+"</"+LifecycleNames[group]+"></Behaviors>";
    }
    static int LifecycleAt(byte[] bytes,bool big) {int root=(int)Word(bytes,8,big)-4,header=(int)Word(bytes,root+468,big),slots=(int)Word(bytes,header+4,big);return (int)Word(bytes,slots,big);}
    static void LifecycleFamily(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        string[][] fields={new[]{"MinFrames:Unsigned","MaxFrames:Unsigned"},new[]{"SleepInterval:Time","DieOnProducerDisabled:Boolean"},new[]{"InvalidStatus:Bits"},new[]{"TargetHeight:Float","TargetHeightIncludesStructures:Boolean","OnlyWhenMovingDown:Boolean","SnapToGroundOnDeath:Boolean","InitialDelay:Duration"},new[]{"HealthTrigger:Float","DamagePerSecond:Float","DeathType:Enum"},new[]{"MinLifetime:Time","MaxLifetime:Time","WaitForWakeUp:Boolean","ScoreKill:Boolean","DeathType:Enum","IgnoreProjectileState:Boolean","CountdownSound:Reference","CountdownCompleteSound:Reference","TimeBeforeDeathThatCountdownCompletes:Time","TimeBetweenCountdownSounds:Time","NumberOfCountdownSoundPlays:Signed"}};
        string[] values={"","0","-0","1","-1","23","0x10","010","1.25"," 23tail","2147483647","2147483648","-2147483648","-2147483649","4294967295","4294967296","nan","inf","-inf","1e39","-1e39","text"};
        for(int group=0;group<6;++group) {
            Check(original,p,"",LifecycleXML(group));string mixed="";
            foreach(string field in fields[group]) {
                string[] f=field.Split(':');string[] tests=f[1]=="Boolean"?new[]{"true","false","TRUE","False","1","0","t","yes"," true",""}:f[1]=="Time"?new[]{"0s","-17s","23ms","2d1s","1000ms","1.25s","5","","text","1e39s","nan","inf","-inf"}:f[1]=="Reference"?new[]{"","Unresolved","A\\0","B\\1","C\\23","D\\-1","E\\2147483647","F\\2147483648","G\\-2147483648","H\\-2147483649","I\\+17","J\\23tail","K\\0x10","L\\ 23","M\\text"}:f[1]=="Bits"?new[]{""," ","ALL","all","UNKNOWN","unknown","NONE","UNSELECTABLE","ALL -UNSELECTABLE","-UNSELECTABLE -UNSELECTABLE +UNSELECTABLE","+","-","-ALL","ALL -UNKNOWN"}:values;
                foreach(string value in tests)Check(original,p,"",LifecycleXML(group,f[0]+"='"+Escape(value)+"'"));
                mixed+=f[0]+"='"+(f[1]=="Boolean"?"true":f[1]=="Bits"?"ALL":f[1]=="Enum"?"EXPLODED":f[1]=="Time"?"2s":f[1]=="Reference"?"Ref\\23":"23.5")+"' ";
            }
            Check(original,p,"Browser='a bb' Description='mixed'",LifecycleXML(group,mixed)+SelectorXML(2,"","<Override><AudioArrayVoice/><RequiredUpgrade>UpA</RequiredUpgrade></Override>"));
            foreach(var container in ModuleContainers)Check(original,p,"","<"+container[0]+"><"+container[1]+" TypeId='"+LifecycleTypes[group]+"' "+mixed+"/></"+container[0]+">");
            foreach(string bad in new[]{"Unknown='1'","MaxHealth='1'","xmlns:f='foreign' f:TypeId='1'"})Reject(p,"",LifecycleXML(group,bad));
            foreach(string bad in new[]{"<Unknown/>","<ObjectStatusValidation Unknown='1'/>","<ObjectStatusValidation xmlns='foreign'/>","<ObjectStatusValidation>text</ObjectStatusValidation>","<ObjectStatusValidation><Unknown/></ObjectStatusValidation>"})Reject(p,"",LifecycleXML(group,"",bad));
        }
        // Complete original enum/mask sets are inserted here from previously characterized metadata.
        foreach(string value in "ALL BITTEN BURNED CATALYST CRUSHED DETONATED ELECTROCUTED EXPLODED EXTRA_1 EXTRA_2 EXTRA_3 EXTRA_4 EXTRA_5 EXTRA_6 EXTRA_7 FADED FLOODED INTERNAL_DESTRUCTION IRRADIATED KNOCKBACK LASERED MAULED NONE NORMAL POISONED POISONED_BETA PRODUCTION_CANCELLED SHATTERED SLAUGHTERED SPLATTED STABBED SUICIDED SUPERNATURAL TOPPLED".Split(' '))foreach(int group in new[]{4,5})Check(original,p,"",LifecycleXML(group,"DeathType='"+value+"'"));
        foreach(string value in "ABSORBED_DAMAGE ADVANCED_MISSILE_PACKS AFLAME AIRBORNE_TARGET AIRCRAFT_IGNORE_SAMEPLAYER_HANGAR_RULE AIR_LOCOMOTOR_ACTIVE ANTI_GARRISON ATTACHED ATTACKING_GARRISONED_STRUCTURE BOOBY_TRAPPED BOOBY_TRAP_EXPLODE BOOKENDING BRIDGE_IMPASSABLE BROADCASTING_INVISIBILITY BUILD_BEING_CANCELED BURNED CANNOT_BE_SOLD CANNOT_LAND_AT_AIRFIELD CAN_ATTACK CAN_ATTACK_WHILE_STEALTHED CAN_CONTEST_GARRISON CAN_ENTER_ANYTHING CAN_NOT_WALK_ON CAN_STEALTH CAN_STEALTH_FROM_PRODUCER CARRYING_FLAG CHANTING CHARGING_BASE_DEFENSES CHARGING_WEAPON CLEARED_FOR_LANDING CLONED COMBINED_ATTACHED COMBINED_CHILD COMBINED_PARENT COMMAND_BUTTON_TOGGLED CONTAINER_OCCUPIED CONTESTED CONTESTING_BUILDING DAMAGED DEATH_1 DEATH_2 DEATH_3 DEATH_4 DEATH_5 DEFLECT_INCOMING_FIRE DELAYED_ENTER_STRUCTURE DEPART_FROM_BOARDING DEPLOYED DEPLOYING DESTROYED DETECTED DOCKING DOES_CONTAIN_TIBERIUM DONT_CLEAR_FOR_BUILD DO_NOT_AVOID_TALLBUILDING DO_NOT_PICK_ME DO_NOT_SCORE ENABLE_TRACER_DRAW ENCLOSED END_FIRE_STATE ENRAGED ENTERING_GOAL EXCEEDED_PATH_NEIGHBOR_COUNT EXECUTING_STORED_COMMAND EXITING_COMBINED EXPLOSIVES_ATTACHED EXPLOSIVES_DETONATING FLEE_OFF_MAP FOLLOWING_THROUGH FORCE_ATTACKING FORCE_ATTACK_MOVING GARRISONED GARRISON_CAN_STEALTH GENERIC_TOGGLE_STATE GRAPPLE_UPDATE_ACTIVE GRAPPLE_UPDATE_BEING_MOVED GROUND_LOCOMOTOR_ACTIVE HARVESTING HAS_SECONDARY_DAMAGE HAS_TIBERIUM_GROWTH_MOD HAS_TIBERIUM_UPGRADE HEALTH_PERCENT_0 HEALTH_PERCENT_100 HEALTH_PERCENT_25 HEALTH_PERCENT_50 HEALTH_PERCENT_75 HIDDEN HIJACKED HIT_SHIELD_SPHERE HORDE_MEMBER IGNORE_AI_COMMAND IGNORING_POWER_DOWN IGNORING_STEALTH IMMOBILE IMMOBILE_ALLOW_ROTATE IMMUNE_TO_BARK INAUDIBLE INHERITED_FROM_ALLY_TEAM INSIDE_GARRISON INVISIBLE_DETECTED INVISIBLE_DETECTED_BY_FRIEND IN_COVER IN_FORMATION_TEMPLATE IN_NANOHIVE IN_SHIELD_SPHERE IN_SPIDER_HOLE IN_STASIS IRRADIATED IS_AIMING_WEAPON IS_ATTACKING IS_BEING_DRAGGED IS_BEING_HARVESTED IS_BRAKING IS_ENGAGED IS_FIRING_WEAPON IS_HIDEOUT IS_LEAVING_FACTORY IS_MELEE_ATTACKING IS_MOVING_TO_RALLY_POINT IS_RELOADING_WEAPON LANDING_IN_PROGRESS LEASHED_RETURNING LEECHED_TARGET_ACTIVE LIFTED_INTO_AIR LINE_SEGMENT_DAMAGE_ACTIVE LOADED_FROM_MAP MAGNETIZED MARCH_OF_DEATH MATCH_TARGETS_SPEED MOVING_TO_DISMOUNT NEXT_MOVE_IS_FORCE_ATTACK_MOVE NEXT_MOVE_IS_REVERSE NON_AUTOACQUIRABLE NOT_IN_WORLD NO_ATTACK NO_ATTACK_FROM_AI NO_AUTO_ACQUIRE NO_BRIBE NO_COLLISIONS NO_HERO_PROPERTIES NO_REFUND NO_SHADOW NO_SPECIAL_ABILITY NO_SQUISHCOLLIDE_DELAY OCLMONITOR_COMPLETED_TASK OCLMONITOR_MONITOR_RELEASED OUT_OF_PHASE OVERCHARGING_WEAPON OVER_WATER PARACHUTING PARKED_AT_AIRFIELD PAUSING PENDING_CONSTRUCTION PHANTOM_STRUCTURE PLACED_BY_PLAYER PLAYER_POWER_1 PLAYER_POWER_2 PLAYER_POWER_3 PLAYER_POWER_4 PLAYER_POWER_5 POINT_DEFENSE_DRONE_ATTACHED PORTER_TAGGED POWERED_DOWN_EMP PREEMPT_PRE_ATTACK RAMPAGING REALLYDAMAGED RECONSTRUCTING REPAIR_ALLIES_WHEN_IDLE REPULSOR RESET_GOAL_POSITION RIDER_IS_PILOT RIFT_OCCUPIED RIOT_SHIELDED RUBBLE RUNNING_DOWN_FROM_BEHIND SCARED_CIVILIAN_CAR SCRAMBLED SHIELDBODY_ENABLED SHROUD_REVEAL_TO_ALL SHRUNK SINKING SKIRMISH_AI_DO_NOT_ATTACK SKIRMISH_AI_DO_NOT_TAKE SOLD SPECIALABILITY_ACTIVE SPECIAL_ABILITY_PACKING_UNPACKING_OR_USING SPECIAL_ARMOR_ACTIVE STAND_GROUND STEALTHED STEAL_NEXT_UNIT_TRAPPED SUBMERGED SURFACED SURFACED_IMMOBILE SWITCHED_WEAPONS TAGGED TARGETED_FOR_REPAIR TEMPORARILY_DEFECTED TIBERIUM_VIBRATING TOPPLING TRANSFORMATION_TOGGLE_STATE TRANSPORT_MOVING_TO_CONTAIN UNATTACKABLE UNCONTROLLABLY_SCARED UNDERGOING_REPAIR UNDER_CONSTRUCTION UNDER_FROZEN UNDER_IRON_CURTAIN UNDER_RUSH_ATTACK UNIT_WANTS_TO_REGARRISON UNPACKING UNSELECTABLE UPDATING_AI USER_PARALYZED USER_POWERED_DOWN USING_ABILITY VEHICLE_ATTACHED WAITING_FOR_BOARDING WALL_SEGMENT WATER_LOCOMOTOR_ACTIVE WEAPON_UPGRADED_01 WEAPON_UPGRADED_02 WEAPON_UPGRADED_03 WONT_RIDE_WITH_YOU".Split(' ')) {
            Check(original,p,"",LifecycleXML(2,"InvalidStatus='"+value+"'"));
            foreach(string field in new[]{"ForbiddenStatus","RequiredStatus"}) {
                Check(original,p,"",LifecycleXML(4,"","<ObjectStatusValidation "+field+"='"+value+"'/>"));
                Check(original,p,"",LifecycleXML(4,"","<ObjectStatusValidation "+field+"='-"+value+" -"+value+" +"+value+"'/>"));
            }
        }
        foreach(string value in new[]{""," ","ALL","all","UNKNOWN","unknown","NONE","UNSELECTABLE","ALL -UNSELECTABLE","-UNSELECTABLE -UNSELECTABLE +UNSELECTABLE","+","-","-ALL","ALL -UNKNOWN"})foreach(string field in new[]{"ForbiddenStatus","RequiredStatus"})Check(original,p,"",LifecycleXML(4,"","<ObjectStatusValidation "+field+"='"+value+"'/>"));
        foreach(string value in new[]{"none","all","All"," NONE","NONE ","UNRECOGNIZED"})foreach(int group in new[]{4,5})Check(original,p,"",LifecycleXML(group,"DeathType='"+value+"'"));
        foreach(string value in new[]{"0","0.001","1","199.999","200","200.001","-1","-200","4294967295","1e19","-1e19","1e39","-1e39","nan","inf","-inf"})Check(original,p,"",LifecycleXML(3,"InitialDelay='"+value+"'"));
        foreach(string child in new[]{"<ObjectStatusValidation/>","<ObjectStatusValidation TypeId='7' ForbiddenStatus='ALL' RequiredStatus='UNSELECTABLE'/>","<ObjectStatusValidation RequiredStatus='UNKNOWN'/>","<ObjectStatusValidation RequiredStatus='ALL' ForbiddenStatus='ALL'/>"})Check(original,p,"",LifecycleXML(4,"HealthTrigger='17%' DamagePerSecond='-1' DeathType='EXPLODED'",child));
        foreach(string bad in new[]{"<ObjectStatusValidation/><ObjectStatusValidation/>","<ObjectStatusValidation id='wrong'/>","<ObjectStatusValidation xmlns:f='foreign' f:TypeId='1'/>"})Reject(p,"",LifecycleXML(4,"",bad));
        foreach(int count in new[]{0,1,2,27,28,29,65}) {
            string children="";for(int i=0;i<count;++i) {int group=i%6;children+="<"+LifecycleNames[group]+" TypeId='"+LifecycleTypes[group]+"' id='Item"+i+"' "+(group==2?"InvalidStatus='ALL'":group==5?"NumberOfCountdownSoundPlays='-1' CountdownSound='S\\23'":"")+">"+(group==4?"<ObjectStatusValidation ForbiddenStatus='ALL' RequiredStatus='UNSELECTABLE'/>":"")+"</"+LifecycleNames[group]+">";}
            Check(original,p,"Description='mixed' Browser='a bb'","<Behaviors>"+children+"</Behaviors>"+SelectorXML(0,"VoicePriorityOverride='-1'","<AudioArrayVoice/>"));
        }
        byte[] bytes=Check(original,p,"",LifecycleXML(0)).InstanceData;int at=LifecycleAt(bytes,big);
        if(Word(bytes,at+8,big)!=0 || Word(bytes,at+12,big)!=1 || bytes.Length!=at+16)throw new Exception("Independent Deletion frame defaults/size");
        bytes=Check(original,p,"",LifecycleXML(1)).InstanceData;at=LifecycleAt(bytes,big);if(Word(bytes,at+8,big)!=0x3f000000u || bytes[at+12]!=0 || Word(bytes,at+12,big)!=0)throw new Exception("Independent producer Time/default bool/padding");
        bytes=Check(original,p,"",LifecycleXML(3,"TargetHeight='23.5' InitialDelay='200.001' TargetHeightIncludesStructures='true' OnlyWhenMovingDown='true' SnapToGroundOnDeath='true'")).InstanceData;at=LifecycleAt(bytes,big);if(Word(bytes,at+8,big)!=0x41bc0000u || Word(bytes,at+12,big)!=2 || bytes[at+16]!=1 || bytes[at+17]!=1 || bytes[at+18]!=1 || bytes[at+19]!=0)throw new Exception("Independent height ceil/boolean layout");
        bytes=Check(original,p,"",LifecycleXML(4)).InstanceData;at=LifecycleAt(bytes,big);if(Word(bytes,at+8,big)!=0x3dccccccu || Word(bytes,at+12,big)!=0x42c80000u || Word(bytes,at+16,big)!=0 || Word(bytes,at+20,big)!=0)throw new Exception("Independent LoseHealth percent/default/null structure");
        bytes=Check(original,p,"",LifecycleXML(5,"NumberOfCountdownSoundPlays='-1' DeathType='ALL' CountdownSound='S\\23'")).InstanceData;at=LifecycleAt(bytes,big);int number=(int)Word(bytes,at+36,big);if(Word(bytes,at+16,big)!=33 || Word(bytes,at+20,big)!=24 || Word(bytes,at+32,big)!=0x3f800000u || number!=at+44 || Word(bytes,number,big)!=0xffffffffu || Word(bytes,at+40,big)!=0)throw new Exception("Independent Lifetime nullable signed/allocation/enum/time/bool");
        bytes=Check(original,p,"",LifecycleXML(2,"InvalidStatus='ALL'")).InstanceData;at=LifecycleAt(bytes,big);int mask=(int)Word(bytes,at+8,big);if(mask!=at+12 || bytes.Length!=at+40)throw new Exception("Independent nullable ObjectStatus allocation");for(int i=0;i<7;++i)if(Word(bytes,mask+4*i,big)!=0xffffffffu)throw new Exception("Independent nullable ObjectStatus ALL");
        bytes=Check(original,p,"",LifecycleXML(4,"","<ObjectStatusValidation ForbiddenStatus='ALL' RequiredStatus=''/>")).InstanceData;at=LifecycleAt(bytes,big);int validation=(int)Word(bytes,at+20,big);if(validation!=at+24 || bytes.Length!=at+80)throw new Exception("Independent nullable validation size/allocation");for(int i=0;i<7;++i)if(Word(bytes,validation+4*i,big)!=0xffffffffu || Word(bytes,validation+28+4*i,big)!=0)throw new Exception("Independent validation inline ObjectStatus masks");
        int rootTable=0,behaviors=(int)Word(bytes,Token(bytes,rootTable,3015298429u,big)+8,big); // Root key is checked against the independent original root contract.
        int wrapper=(int)Word(bytes,behaviors+20,big),table=(int)Word(bytes,wrapper+8,big);
        uint[] chain={4043318533u,687874333u,2769121946u,3613234082u};
        for(int i=0;i<chain.Length;++i) {if(Word(bytes,table,big)!=1 || Word(bytes,table+4,big)!=chain[i])throw new Exception("Independent Update Token inheritance chain");table=(int)Word(bytes,table+8,big);}
        if(Word(bytes,table,big)!=430981792u || Word(bytes,table+8,big)!=at || Word(bytes,table+24,big)!=0 || Word(bytes,table+28,big)!=0 || Word(bytes,table+32,big)!=0)throw new Exception("Independent ModuleData Token/sentinel");
        var a=Check(original,p,"",LifecycleXML(4,"HealthTrigger='25%'","<ObjectStatusValidation ForbiddenStatus='ALL' RequiredStatus='UNSELECTABLE'/>"));var b=Check(original,p,"",LifecycleXML(4,"HealthTrigger='25'","<ObjectStatusValidation RequiredStatus='UNSELECTABLE' ForbiddenStatus='ALL'/>"));Equal(a.InstanceData,b.InstanceData,"Lifecycle attribute/schema order and percent text",false);Equal(a.RelocationData,b.RelocationData,"Lifecycle relocation order",false);
    }

    static readonly string[] DieNames={"DestroyDie","DestroyDie","UpgradeDie","KeepObjectDie","CreateObjectDie","CrushDie","RefundDie"};
    static readonly string[] DieTypes={"2655988594","259966977","1359837639","3491013058","996718440","2448240860","3787260944"};
    static string DieXML(int group,string attrs="",string children="") {return "<Behaviors><"+DieNames[group]+" TypeId='"+DieTypes[group]+"' id='DieTest' "+attrs+">"+children+"</"+DieNames[group]+"></Behaviors>";}
    static void DieFamily(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        string[][] fields={new string[0],new string[0],new[]{"UpgradeId:Reference"},new[]{"CollapsingTime:Time","StayOnRadar:Boolean"},new[]{"CreationList:Reference"},new[]{"TotalCrushSound:Reference","BackEndCrushSound:Reference","FrontEndCrushSound:Reference","TotalCrushSoundPercent:Float","BackEndCrushSoundPercent:Float","FrontEndCrushSoundPercent:Float"},new[]{"UpgradeRequired:Reference","BuildingRequired:StringList","RefundPercent:Float"}};
        string[] strings={""," ","a","ab cd"," a  b ","a,b;c","é ÿ 中文","one two three four five six seven eight nine ten","a a a a","a  bb ccc dddd eeeee"};
        string[] numbers={"","-1","0","23","100","17%","0x10","010","1.25"," 23tail","2147483647","2147483648","-2147483648","4294967295","4294967296","nan","inf","-inf","1e39","text"};
        string[] references={"","Unresolved","A\\0","B\\1","C\\23","D\\-1","E\\2147483647","F\\2147483648","G\\-2147483648","H\\-2147483649","I\\+17","J\\23tail","K\\0x10","L\\ 23","M\\text"};
        for(int group=0;group<7;++group) {
            Check(original,p,"",DieXML(group));Check(original,p,"",DieXML(group,"","<DieMuxData/>"));string all="";
            foreach(string field in fields[group]) {
                var f=field.Split(':');var values=f[1]=="Reference"?references:f[1]=="StringList"?strings:f[1]=="Boolean"?new[]{"true","false","TRUE","False","1","0","t","yes"," true",""}:f[1]=="Time"?new[]{"0s","-17s","23ms","2d1s","1000ms","1.25s","5","","nan","inf"}:numbers;
                foreach(string value in values)Check(original,p,"",DieXML(group,f[0]+"='"+Escape(value)+"'"));
                all+=f[0]+"='"+(f[1]=="Boolean"?"true":f[1]=="Time"?"2s":f[1]=="Reference"?"Ref\\23":f[1]=="StringList"?"a bb":"23.5")+"' ";
            }
            foreach(string field in new[]{"DamageAmountRequired","MinKillerAngle","MaxKillerAngle","VeterancyLevels"}) {
                var values=field=="VeterancyLevels"?strings:field.IndexOf("Angle")>=0?new[]{"0r","180d","-90d","1.25r","2d1","","text","nan","inf"}:numbers;
                foreach(string value in values)Check(original,p,"",DieXML(group,"","<DieMuxData "+field+"='"+Escape(value)+"'/>"));
            }
            Check(original,p,"Browser='a bb' Description='mix'",DieXML(group,all,"<DieMuxData VeterancyLevels='VETERAN ELITE HERO' ExemptStatus='ALL' RequiredStatus='UNSELECTABLE' DamageAmountRequired='17' MinKillerAngle='-90d' MaxKillerAngle='180d' DeathTypes='ALL' DeathTypesForbidden='EXPLODED'/>"));
            foreach(var container in ModuleContainers)Check(original,p,"","<"+container[0]+"><"+container[1]+" TypeId='"+DieTypes[group]+"' "+all+"><DieMuxData VeterancyLevels='a bb' ExemptStatus='ALL'/></"+container[1]+"></"+container[0]+">");
            foreach(string bad in new[]{"<Unknown/>","<DieMuxData/><DieMuxData/>","<DieMuxData Unknown='1'/>","<DieMuxData xmlns='foreign'/>","<DieMuxData id='wrong'/>","<DieMuxData>text</DieMuxData>","<DieMuxData><Unknown/></DieMuxData>"})Reject(p,"",DieXML(group,"",bad));
            Reject(p,"",DieXML(group,"Unknown='1'"));
        }
        foreach(string value in "ABSORBED_DAMAGE ADVANCED_MISSILE_PACKS AFLAME AIRBORNE_TARGET AIRCRAFT_IGNORE_SAMEPLAYER_HANGAR_RULE AIR_LOCOMOTOR_ACTIVE ANTI_GARRISON ATTACHED ATTACKING_GARRISONED_STRUCTURE BOOBY_TRAPPED BOOBY_TRAP_EXPLODE BOOKENDING BRIDGE_IMPASSABLE BROADCASTING_INVISIBILITY BUILD_BEING_CANCELED BURNED CANNOT_BE_SOLD CANNOT_LAND_AT_AIRFIELD CAN_ATTACK CAN_ATTACK_WHILE_STEALTHED CAN_CONTEST_GARRISON CAN_ENTER_ANYTHING CAN_NOT_WALK_ON CAN_STEALTH CAN_STEALTH_FROM_PRODUCER CARRYING_FLAG CHANTING CHARGING_BASE_DEFENSES CHARGING_WEAPON CLEARED_FOR_LANDING CLONED COMBINED_ATTACHED COMBINED_CHILD COMBINED_PARENT COMMAND_BUTTON_TOGGLED CONTAINER_OCCUPIED CONTESTED CONTESTING_BUILDING DAMAGED DEATH_1 DEATH_2 DEATH_3 DEATH_4 DEATH_5 DEFLECT_INCOMING_FIRE DELAYED_ENTER_STRUCTURE DEPART_FROM_BOARDING DEPLOYED DEPLOYING DESTROYED DETECTED DOCKING DOES_CONTAIN_TIBERIUM DONT_CLEAR_FOR_BUILD DO_NOT_AVOID_TALLBUILDING DO_NOT_PICK_ME DO_NOT_SCORE ENABLE_TRACER_DRAW ENCLOSED END_FIRE_STATE ENRAGED ENTERING_GOAL EXCEEDED_PATH_NEIGHBOR_COUNT EXECUTING_STORED_COMMAND EXITING_COMBINED EXPLOSIVES_ATTACHED EXPLOSIVES_DETONATING FLEE_OFF_MAP FOLLOWING_THROUGH FORCE_ATTACKING FORCE_ATTACK_MOVING GARRISONED GARRISON_CAN_STEALTH GENERIC_TOGGLE_STATE GRAPPLE_UPDATE_ACTIVE GRAPPLE_UPDATE_BEING_MOVED GROUND_LOCOMOTOR_ACTIVE HARVESTING HAS_SECONDARY_DAMAGE HAS_TIBERIUM_GROWTH_MOD HAS_TIBERIUM_UPGRADE HEALTH_PERCENT_0 HEALTH_PERCENT_100 HEALTH_PERCENT_25 HEALTH_PERCENT_50 HEALTH_PERCENT_75 HIDDEN HIJACKED HIT_SHIELD_SPHERE HORDE_MEMBER IGNORE_AI_COMMAND IGNORING_POWER_DOWN IGNORING_STEALTH IMMOBILE IMMOBILE_ALLOW_ROTATE IMMUNE_TO_BARK INAUDIBLE INHERITED_FROM_ALLY_TEAM INSIDE_GARRISON INVISIBLE_DETECTED INVISIBLE_DETECTED_BY_FRIEND IN_COVER IN_FORMATION_TEMPLATE IN_NANOHIVE IN_SHIELD_SPHERE IN_SPIDER_HOLE IN_STASIS IRRADIATED IS_AIMING_WEAPON IS_ATTACKING IS_BEING_DRAGGED IS_BEING_HARVESTED IS_BRAKING IS_ENGAGED IS_FIRING_WEAPON IS_HIDEOUT IS_LEAVING_FACTORY IS_MELEE_ATTACKING IS_MOVING_TO_RALLY_POINT IS_RELOADING_WEAPON LANDING_IN_PROGRESS LEASHED_RETURNING LEECHED_TARGET_ACTIVE LIFTED_INTO_AIR LINE_SEGMENT_DAMAGE_ACTIVE LOADED_FROM_MAP MAGNETIZED MARCH_OF_DEATH MATCH_TARGETS_SPEED MOVING_TO_DISMOUNT NEXT_MOVE_IS_FORCE_ATTACK_MOVE NEXT_MOVE_IS_REVERSE NON_AUTOACQUIRABLE NOT_IN_WORLD NO_ATTACK NO_ATTACK_FROM_AI NO_AUTO_ACQUIRE NO_BRIBE NO_COLLISIONS NO_HERO_PROPERTIES NO_REFUND NO_SHADOW NO_SPECIAL_ABILITY NO_SQUISHCOLLIDE_DELAY OCLMONITOR_COMPLETED_TASK OCLMONITOR_MONITOR_RELEASED OUT_OF_PHASE OVERCHARGING_WEAPON OVER_WATER PARACHUTING PARKED_AT_AIRFIELD PAUSING PENDING_CONSTRUCTION PHANTOM_STRUCTURE PLACED_BY_PLAYER PLAYER_POWER_1 PLAYER_POWER_2 PLAYER_POWER_3 PLAYER_POWER_4 PLAYER_POWER_5 POINT_DEFENSE_DRONE_ATTACHED PORTER_TAGGED POWERED_DOWN_EMP PREEMPT_PRE_ATTACK RAMPAGING REALLYDAMAGED RECONSTRUCTING REPAIR_ALLIES_WHEN_IDLE REPULSOR RESET_GOAL_POSITION RIDER_IS_PILOT RIFT_OCCUPIED RIOT_SHIELDED RUBBLE RUNNING_DOWN_FROM_BEHIND SCARED_CIVILIAN_CAR SCRAMBLED SHIELDBODY_ENABLED SHROUD_REVEAL_TO_ALL SHRUNK SINKING SKIRMISH_AI_DO_NOT_ATTACK SKIRMISH_AI_DO_NOT_TAKE SOLD SPECIALABILITY_ACTIVE SPECIAL_ABILITY_PACKING_UNPACKING_OR_USING SPECIAL_ARMOR_ACTIVE STAND_GROUND STEALTHED STEAL_NEXT_UNIT_TRAPPED SUBMERGED SURFACED SURFACED_IMMOBILE SWITCHED_WEAPONS TAGGED TARGETED_FOR_REPAIR TEMPORARILY_DEFECTED TIBERIUM_VIBRATING TOPPLING TRANSFORMATION_TOGGLE_STATE TRANSPORT_MOVING_TO_CONTAIN UNATTACKABLE UNCONTROLLABLY_SCARED UNDERGOING_REPAIR UNDER_CONSTRUCTION UNDER_FROZEN UNDER_IRON_CURTAIN UNDER_RUSH_ATTACK UNIT_WANTS_TO_REGARRISON UNPACKING UNSELECTABLE UPDATING_AI USER_PARALYZED USER_POWERED_DOWN USING_ABILITY VEHICLE_ATTACHED WAITING_FOR_BOARDING WALL_SEGMENT WATER_LOCOMOTOR_ACTIVE WEAPON_UPGRADED_01 WEAPON_UPGRADED_02 WEAPON_UPGRADED_03 WONT_RIDE_WITH_YOU".Split(' '))foreach(string field in new[]{"ExemptStatus","RequiredStatus"})for(int group=0;group<7;++group)Check(original,p,"",DieXML(group,"","<DieMuxData "+field+"='"+value+"'/>"));
        foreach(string value in "ALL BITTEN BURNED CATALYST CRUSHED DETONATED ELECTROCUTED EXPLODED EXTRA_1 EXTRA_2 EXTRA_3 EXTRA_4 EXTRA_5 EXTRA_6 EXTRA_7 FADED FLOODED INTERNAL_DESTRUCTION IRRADIATED KNOCKBACK LASERED MAULED NONE NORMAL POISONED POISONED_BETA PRODUCTION_CANCELLED SHATTERED SLAUGHTERED SPLATTED STABBED SUICIDED SUPERNATURAL TOPPLED".Split(' '))foreach(string field in new[]{"DeathTypes","DeathTypesForbidden"})for(int group=0;group<7;++group)Check(original,p,"",DieXML(group,"","<DieMuxData "+field+"='"+value+"'/>"));
        foreach(string value in new[]{""," ","ALL","all","UNKNOWN","unknown","ALL -UNKNOWN","ALL -ALL","-ALL","-EXPLODED -EXPLODED +EXPLODED","UNSELECTABLE","-UNSELECTABLE -UNSELECTABLE +UNSELECTABLE","+","-"})foreach(string field in new[]{"ExemptStatus","RequiredStatus","DeathTypes","DeathTypesForbidden"})for(int group=0;group<7;++group)Check(original,p,"",DieXML(group,"","<DieMuxData "+field+"='"+value+"'/>"));
        foreach(string value in references) {
            Check(original,p,"",DieXML(4,"CreationList='OCL\\23'","<UpgradeForbidden>"+value+"</UpgradeForbidden><UpgradeRequired>"+value+"</UpgradeRequired><DieMuxData VeterancyLevels='a bb' ExemptStatus='ALL'/>"));
        }
        foreach(int count in new[]{0,1,2,27,28,29,65}) {
            string a="",b="",words="";for(int i=0;i<count;++i) {a+="<UpgradeRequired>Up\\23</UpgradeRequired>";b+="<UpgradeForbidden>Up\\-1</UpgradeForbidden>";words+=" word"+i;}
            Check(original,p,"",DieXML(4,"CreationList='OCL\\0'",b+a+"<DieMuxData VeterancyLevels='"+words+"' ExemptStatus='ALL' RequiredStatus='UNSELECTABLE'/>"));
            Check(original,p,"",DieXML(6,"BuildingRequired='"+words+"' RefundPercent='50'","<ObjectFilter Include='VEHICLE' StatusBitFlags='ALL'><IncludeThing>Tank</IncludeThing><ExcludeThing>Other</ExcludeThing></ObjectFilter><DieMuxData VeterancyLevels='"+words+"'/>"));
        }
        foreach(string filter in new[]{"<ObjectFilter/>","<ObjectFilter id='AbCd' Rule='ALL' Relationship='ALLIES ENEMIES' Alignment='GOOD' Include='VEHICLE' Exclude='STRUCTURE' StatusBitFlags='ALL' StatusBitFlagsExclude='UNSELECTABLE'><IncludeThing>Tank</IncludeThing><ExcludeThing>Other</ExcludeThing></ObjectFilter>","<ObjectFilter Rule='UNKNOWN' Relationship='UNKNOWN' Alignment='UNKNOWN' Include='UNKNOWN' StatusBitFlags='UNKNOWN'/>"})Check(original,p,"",DieXML(6,"UpgradeRequired='Up\\23' BuildingRequired='a bb'",filter+"<DieMuxData ExemptStatus='ALL'/>"));
        foreach(string bad in new[]{"<UpgradeRequired Unknown='1'/>","<UpgradeRequired><Unknown/></UpgradeRequired>","<UpgradeForbidden xmlns='foreign'/>","<UpgradeForbidden id='wrong'/>"})Reject(p,"",DieXML(4,"",bad));
        foreach(string bad in new[]{"<ObjectFilter/><ObjectFilter/>","<ObjectFilter Unknown='1'/>","<ObjectFilter xmlns='foreign'/>","<ObjectFilter>text</ObjectFilter>","<ObjectFilter><Unknown/></ObjectFilter>","<ObjectFilter><IncludeThing Unknown='1'/></ObjectFilter>","<ObjectFilter><ExcludeThing><Unknown/></ExcludeThing></ObjectFilter>"})Reject(p,"",DieXML(6,"",bad));
        Reject(p,"",DieXML(0,"","<UpgradeRequired/>"));Reject(p,"",DieXML(4,"","<ObjectFilter/>"));Reject(p,"",DieXML(6,"","<UpgradeRequired/>"));
        byte[] absent=Check(original,p,"",DieXML(1)).InstanceData;int at=LifecycleAt(absent,big);for(int i=8;i<52;++i)if(absent[at+i]!=0)throw new Exception("Independent absent inline DieMux must remain zero");
        var buffer=Check(original,p,"",DieXML(1,"","<DieMuxData/>"));byte[] bytes=buffer.InstanceData;at=LifecycleAt(bytes,big);
        if(Word(bytes,at+24,big)!=0xbf800000u || Word(bytes,at+28,big)!=0x3c8efa35u || Word(bytes,at+32,big)!=0xbc8efa35u || bytes.Length!=at+52)throw new Exception("Independent present DieMux defaults/angles/inline size");
        bytes=Check(original,p,"",DieXML(1,"","<DieMuxData VeterancyLevels='a bb' ExemptStatus='ALL' RequiredStatus='' DeathTypes='ALL' DeathTypesForbidden='ALL'/>")).InstanceData;at=LifecycleAt(bytes,big);int stringsAt=(int)Word(bytes,at+12,big),exempt=(int)Word(bytes,at+16,big),required=(int)Word(bytes,at+20,big);
        if(Word(bytes,at+8,big)!=2 || stringsAt!=at+52 || Word(bytes,stringsAt,big)!=1 || Word(bytes,stringsAt+8,big)!=2 || exempt!=at+76 || required!=at+104 || Word(bytes,at+36,big)!=0xffffffffu || Word(bytes,at+40,big)!=0xffffffffu || Word(bytes,at+44,big)!=0xffffffffu || Word(bytes,at+48,big)!=0xffffffffu)throw new Exception("Independent DieMux strings/nullable masks/34-bit arrays");
        for(int i=0;i<7;++i)if(Word(bytes,exempt+4*i,big)!=0xffffffffu || Word(bytes,required+4*i,big)!=0)throw new Exception("Independent DieMux ObjectStatus contents");
        int bt=(int)Word(bytes,Token(bytes,0,3015298429u,big)+8,big),wr=(int)Word(bytes,bt+20,big),dt=(int)Word(bytes,wr+8,big);
        if(Word(bytes,dt,big)!=1 || Word(bytes,dt+4,big)!=2655988594u || Word(bytes,dt+12,big)!=0 || Word(bytes,dt+16,big)!=0 || Word(bytes,dt+20,big)!=0)throw new Exception("Independent folded DestroyDie Token inheritance/sentinel");
        int baseTable=(int)Word(bytes,dt+8,big),muxTable=(int)Word(bytes,Token(bytes,baseTable,869730620u,big)+8,big),veterancy=(int)Word(bytes,Token(bytes,muxTable,4078390751u,big)+8,big);
        if(Word(bytes,veterancy+8,big)!=at+8 || Word(bytes,veterancy+20,big)!=stringsAt || Word(bytes,veterancy+32,big)!=stringsAt+8 || Word(bytes,Token(bytes,muxTable,2894011938u,big)+8,big)!=exempt || Word(bytes,Token(bytes,muxTable,597602817u,big)+8,big)!=required || Word(bytes,muxTable+96,big)!=0 || Word(bytes,muxTable+100,big)!=0 || Word(bytes,muxTable+104,big)!=0)throw new Exception("Independent inline DieMux StringList/nullable Token/sentinel");
        bytes=Check(original,p,"",DieXML(5)).InstanceData;at=LifecycleAt(bytes,big);for(int i=64;i<76;i+=4)if(Word(bytes,at+i,big)!=0x3f800000u)throw new Exception("Independent CrushDie percentage defaults");
        bytes=Check(original,p,"",DieXML(4,"CreationList='OCL\\23'","<UpgradeForbidden>Up\\-1</UpgradeForbidden><UpgradeRequired>Up\\23</UpgradeRequired><DieMuxData ExemptStatus='ALL'/>")).InstanceData;at=LifecycleAt(bytes,big);int requiredAt=(int)Word(bytes,at+60,big),forbiddenAt=(int)Word(bytes,at+68,big);
        if(Word(bytes,at+52,big)!=24 || Word(bytes,at+56,big)!=1 || requiredAt!=at+72 || Word(bytes,requiredAt,big)!=24 || forbiddenAt!=at+76 || Word(bytes,forbiddenAt,big)!=0 || Word(bytes,at+16,big)!=at+80)throw new Exception("Independent CreateObjectDie strong resource lists/allocation order");
        var aBuffer=Check(original,p,"",DieXML(6,"BuildingRequired='a bb' RefundPercent='50%'","<ObjectFilter/><DieMuxData VeterancyLevels='VETERAN ELITE' ExemptStatus='ALL'/>"));var bBuffer=Check(original,p,"",DieXML(6,"RefundPercent='50' BuildingRequired='a bb'","<DieMuxData ExemptStatus='ALL' VeterancyLevels='VETERAN ELITE'/><ObjectFilter/>"));Equal(aBuffer.InstanceData,bBuffer.InstanceData,"Die allocation/schema order",false);Equal(aBuffer.RelocationData,bBuffer.RelocationData,"Die relocation order",false);
        bytes=aBuffer.InstanceData;at=LifecycleAt(bytes,big);int filterAt=(int)Word(bytes,at+68,big),muxStatus=(int)Word(bytes,at+16,big);if(Word(bytes,at+64,big)!=0x3f000000u || filterAt>=muxStatus || bytes.Length!=muxStatus+28)throw new Exception("Independent Refund filter before DieMux allocation");
    }

    static readonly string[] UpgradeNames={"PowerUpgrade","PowerUpgrade","DelayedUpgrade","AttributeModifierUpgrade","InvisibilityUpgrade","CommandSetUpgrade","ExperienceScalarUpgrade","MaxHealthUpgrade","ArmorUpgrade","LevelUpUpgrade","AudioLoopUpgrade"};
    static readonly string[] UpgradeTypes={"2925571736","3282447464","1518063315","1956793413","1220819118","2125984281","3765090183","2090477367","2717792172","2541561249","585970648"};
    static string UpgradeXML(int group,string attrs="",string children="") {return "<Behaviors><"+UpgradeNames[group]+" TypeId='"+UpgradeTypes[group]+"' id='UpgradeTest' "+attrs+">"+children+"</"+UpgradeNames[group]+"></Behaviors>";}
    static void UpgradeFamily(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        string[] common={"RequiresAllTriggers","RequiresAllConflictingTriggers","Permanent","WaypointMode","WaypointModeTerminal"};
        string[][] fields={new string[0],new string[0],new[]{"DelayTime:Unsigned"},new[]{"AttributeModifier:Reference"},new[]{"InvisibilityTemplate:Reference"},new[]{"CommandSet:AssetId"},new[]{"AddXPScalar:Float"},new[]{"AddMaxHealth:Float","ChangeType:HealthEnum"},new[]{"KillArmorUpgrade:Boolean","IgnoreArmorUpgrade:Boolean","ArmorSetFlag:ArmorEnum"},new[]{"LevelsToGain:Signed","LevelCap:Signed","DoFlash:Boolean"},new[]{"SoundToPlay:Reference","KillAfterMS:Unsigned","KillOnDeath:Boolean"}};
        string[] booleans={"true","false","TRUE","False","1","0","t","yes"," true",""};
        string[] numbers={"","-1","0","23","0x10","010","1.25"," 23tail","2147483647","2147483648","-2147483648","-2147483649","4294967295","4294967296","nan","inf","-inf","1e39","-1e39","text"};
        string[] references={"","Unresolved","A\\0","B\\1","C\\23","D\\-1","E\\2147483647","F\\2147483648","G\\-2147483648","H\\-2147483649","I\\+17","J\\23tail","K\\0x10","L\\ 23","M\\text"};
        string[] ids={"","a","AB","ab","abc","abcd","abcde","ABCDEF","MiXeD","with spaces","é","ÿ","中文","A&B","Up\\23"};
        for(int group=0;group<11;++group) {
            Check(original,p,"",UpgradeXML(group));Check(original,p,"",UpgradeXML(group,"","<CustomAnimAndDuration/>"));
            foreach(string field in common)foreach(string value in booleans)Check(original,p,"",UpgradeXML(group,field+"='"+value+"'"));
            string mixed="RequiresAllTriggers='true' RequiresAllConflictingTriggers='true' Permanent='true' WaypointMode='true' WaypointModeTerminal='false' ";
            foreach(string field in fields[group]) {
                var parts=field.Split(':');string[] values=parts[1]=="Boolean"?booleans:parts[1]=="Reference"?references:parts[1]=="AssetId"?ids:numbers;
                foreach(string value in values)Check(original,p,"",UpgradeXML(group,parts[0]+"='"+Escape(value)+"'"));
                mixed+=parts[0]+"='"+(parts[1]=="Boolean"?"true":parts[1]=="Reference"?"Ref\\23":parts[1]=="AssetId"?"AbCd":"23")+"' ";
            }
            foreach(string field in new[]{"Frames","Trigger"})foreach(string value in numbers)Check(original,p,"",UpgradeXML(group,"","<CustomAnimAndDuration "+field+"='"+value+"'/>"));
            foreach(string value in new[]{"","UNKNOWN","unknown","ALL","all","DAMAGED","damaged"," DAMAGED","DAMAGED ","DAMAGED RUBBLE","-1","0"})Check(original,p,"",UpgradeXML(group,"","<CustomAnimAndDuration AnimState='"+value+"'/>"));
            foreach(string value in BodyModelConditions)Check(original,p,"",UpgradeXML(group,"","<CustomAnimAndDuration AnimState='"+value+"'/>"));
            foreach(string value in ids)foreach(string name in new[]{"TriggeredBy","ConflictsWith"})Check(original,p,"",UpgradeXML(group,"","<"+name+" TypeId='7'>"+Escape(value)+"</"+name+">"));
            string payload="<ConflictsWith>UpB</ConflictsWith><CustomAnimAndDuration TypeId='7' AnimState='DAMAGED' Frames='010' Trigger='0xffffffff'/><TriggeredBy>UpA</TriggeredBy><TriggeredBy>UpB</TriggeredBy>";
            Check(original,p,"Browser='a bb' Description='mixed'",(UpgradeXML(group,mixed,payload)+DieXML(6,"BuildingRequired='a bb'","<ObjectFilter/><DieMuxData VeterancyLevels='a bb'/>")).Replace("</Behaviors><Behaviors>",""));
            foreach(var container in ModuleContainers)Check(original,p,"","<"+container[0]+"><"+container[1]+" TypeId='"+UpgradeTypes[group]+"' "+mixed+">"+payload+"</"+container[1]+"></"+container[0]+">");
            foreach(string bad in new[]{"<Unknown/>","<CustomAnimAndDuration/><CustomAnimAndDuration/>","<CustomAnimAndDuration id='wrong'/>","<CustomAnimAndDuration Unknown='1'/>","<CustomAnimAndDuration xmlns='foreign'/>","<CustomAnimAndDuration>text</CustomAnimAndDuration>","<CustomAnimAndDuration><Unknown/></CustomAnimAndDuration>","<TriggeredBy id='wrong'/>","<TriggeredBy><Unknown/></TriggeredBy>","<ConflictsWith xmlns='foreign'/>","<ConflictsWith xmlns:f='foreign' f:TypeId='1'/>","<DieMuxData/>","<ObjectFilter/>"})Reject(p,"",UpgradeXML(group,"",bad));
            Reject(p,"",UpgradeXML(group,"Unknown='1'"));Reject(p,"",UpgradeXML(group,"MaxHealth='1'"));
        }
        foreach(string value in "AIRBORNE ALTERNATE_FORMATION AS_TOWER ELITE HERO INVALID IN_SPIDER_HOLE MOUNTED PLAYER_UPGRADE PLAYER_UPGRADE_2 PLAYER_UPGRADE_3 SECONDARY_DAMAGE SHIELDBODY_ENABLED SHRINK_EFFECT UNBESIEGEABLE UNUSED_05 UNUSED_06 UNUSED_07 UNUSED_08 UNUSED_09 UNUSED_10 VETERAN WEAK_VERSUS_BASEDEFENSES".Split(' '))Check(original,p,"",UpgradeXML(8,"ArmorSetFlag='"+value+"'"));
        foreach(string value in "ADD_CURRENT_HEALTH_TOO PRESERVE_RATIO SAME_CURRENTHEALTH".Split(' '))Check(original,p,"",UpgradeXML(7,"ChangeType='"+value+"'"));
        foreach(string value in new[]{"","UNKNOWN","unknown","NONE","all"," ALL","ALL ","-1","0"}) {Check(original,p,"",UpgradeXML(7,"ChangeType='"+value+"'"));Check(original,p,"",UpgradeXML(8,"ArmorSetFlag='"+value+"'"));}
        foreach(int count in new[]{0,1,2,27,28,29,65}) {
            string triggers="",conflicts="",modules="";for(int i=0;i<count;++i) {triggers+="<TriggeredBy>Up"+i+"</TriggeredBy>";conflicts+="<ConflictsWith>Up"+i+"</ConflictsWith>";int group=i%11;modules+="<"+UpgradeNames[group]+" TypeId='"+UpgradeTypes[group]+"' id='Item"+i+"'><TriggeredBy>UpA</TriggeredBy><ConflictsWith>UpB</ConflictsWith><CustomAnimAndDuration Frames='23' Trigger='4294967295'/></"+UpgradeNames[group]+">";}
            Check(original,p,"Browser='a bb'",UpgradeXML(1,"",conflicts+"<CustomAnimAndDuration AnimState='ALL'/>"+triggers));
            Check(original,p,"Description='mixed'","<Behaviors>"+modules+"</Behaviors>"+SelectorXML(0,"VoicePriorityOverride='-1'","<AudioArrayVoice/>"));
        }
        byte[] bytes=Check(original,p,"",UpgradeXML(0)).InstanceData;int moduleAt=LifecycleAt(bytes,big);
        if(bytes.Length!=moduleAt+36 || Word(bytes,moduleAt+8,big)!=0 || Word(bytes,moduleAt+12,big)!=0 || Word(bytes,moduleAt+20,big)!=0 || Word(bytes,moduleAt+28,big)!=0 || bytes[moduleAt+32]!=1 || bytes[moduleAt+33]!=0 || bytes[moduleAt+34]!=0 || bytes[moduleAt+35]!=0)throw new Exception("Independent Upgrade base default/null/bool/padding layout");
        bytes=Check(original,p,"",UpgradeXML(1,"","<ConflictsWith>UpB</ConflictsWith><TriggeredBy>UpA</TriggeredBy><CustomAnimAndDuration AnimState='UNKNOWN' Frames='010' Trigger='0xffffffff'/><TriggeredBy>UpB</TriggeredBy>")).InstanceData;moduleAt=LifecycleAt(bytes,big);
        int animAt=(int)Word(bytes,moduleAt+8,big),triggerAt=(int)Word(bytes,moduleAt+16,big),conflictAt=(int)Word(bytes,moduleAt+24,big);
        if(animAt!=moduleAt+36 || triggerAt!=moduleAt+48 || conflictAt!=moduleAt+56 || Word(bytes,moduleAt+12,big)!=2 || Word(bytes,moduleAt+20,big)!=1 || Word(bytes,animAt,big)!=233 || Word(bytes,animAt+4,big)!=8 || Word(bytes,animAt+8,big)!=0xffffffffu)throw new Exception("Independent Upgrade optional struct/weak-list/schema allocation order");
        int behaviors=(int)Word(bytes,Token(bytes,0,3015298429u,big)+8,big),wrapper=(int)Word(bytes,behaviors+20,big),powerTable=(int)Word(bytes,wrapper+8,big);
        if(Word(bytes,powerTable,big)!=1 || Word(bytes,powerTable+4,big)!=2925571736u || Word(bytes,powerTable+12,big)!=0 || Word(bytes,powerTable+16,big)!=0 || Word(bytes,powerTable+20,big)!=0)throw new Exception("Independent folded PowerUpgrade inheritance/sentinel");
        int baseTable=(int)Word(bytes,powerTable+8,big),animTable=(int)Word(bytes,Token(bytes,baseTable,2510190459u,big)+8,big),listTable=(int)Word(bytes,Token(bytes,baseTable,1776173490u,big)+8,big);
        if(Word(bytes,baseTable+108,big)!=0 || Word(bytes,baseTable+112,big)!=0 || Word(bytes,baseTable+116,big)!=0 || Word(bytes,animTable+8,big)!=animAt || Word(bytes,animTable+20,big)!=animAt+4 || Word(bytes,animTable+32,big)!=animAt+8 || Word(bytes,animTable+36,big)!=0 || Word(bytes,animTable+40,big)!=0 || Word(bytes,animTable+44,big)!=0 || Word(bytes,listTable+8,big)!=moduleAt+12 || Word(bytes,listTable+20,big)!=triggerAt || Word(bytes,listTable+32,big)!=triggerAt+4)throw new Exception("Independent Upgrade struct/member/list Token/sentinels");
        bytes=Check(original,p,"",UpgradeXML(7,"ChangeType='UNKNOWN'")).InstanceData;moduleAt=LifecycleAt(bytes,big);if(Word(bytes,moduleAt+40,big)!=2)throw new Exception("Independent MaxHealth enum fallback");
        bytes=Check(original,p,"",UpgradeXML(8,"ArmorSetFlag='UNKNOWN'")).InstanceData;moduleAt=LifecycleAt(bytes,big);if(Word(bytes,moduleAt+36,big)!=12)throw new Exception("Independent ArmorSet scalar fallback");
        bytes=Check(original,p,"",UpgradeXML(9,"LevelsToGain='-1' LevelCap='0x80000000'")).InstanceData;moduleAt=LifecycleAt(bytes,big);if(Word(bytes,moduleAt+36,big)!=0xffffffffu || Word(bytes,moduleAt+40,big)!=0x7fffffffu || bytes[moduleAt+44]!=1 || bytes[moduleAt+45]!=0 || bytes[moduleAt+46]!=0 || bytes[moduleAt+47]!=0)throw new Exception("Independent LevelUp signed/base0 clamp/default bool");
        var upper=Check(original,p,"",UpgradeXML(5,"CommandSet='AbCd'","<TriggeredBy>AbCd</TriggeredBy><ConflictsWith>AbCd</ConflictsWith>"));var lower=Check(original,p,"",UpgradeXML(5,"CommandSet='abcd'","<TriggeredBy>abcd</TriggeredBy><ConflictsWith>abcd</ConflictsWith>"));Equal(upper.InstanceData,lower.InstanceData,"Upgrade weak typed IDs lowercase",false);
        var ordered=Check(original,p,"",UpgradeXML(10,"SoundToPlay='S\\23' KillAfterMS='010' KillOnDeath='true'","<CustomAnimAndDuration Frames='23'/><TriggeredBy>UpA</TriggeredBy><ConflictsWith>UpB</ConflictsWith>"));var reversed=Check(original,p,"",UpgradeXML(10,"KillOnDeath='true' KillAfterMS='010' SoundToPlay='S\\23'","<ConflictsWith>UpB</ConflictsWith><TriggeredBy>UpA</TriggeredBy><CustomAnimAndDuration Frames='23'/>"));Equal(ordered.InstanceData,reversed.InstanceData,"Upgrade allocation order",false);Equal(ordered.RelocationData,reversed.RelocationData,"Upgrade relocation order",false);
        bytes=ordered.InstanceData;moduleAt=LifecycleAt(bytes,big);if(Word(bytes,moduleAt+36,big)!=24 || Word(bytes,moduleAt+40,big)!=8 || bytes[moduleAt+44]!=1 || Word(bytes,moduleAt+8,big)!=moduleAt+48)throw new Exception("Independent AudioLoop strong reference/unsigned/optional allocation");
    }

    static int Main(string[] args) {
        try {
            var original=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(args[0]).GetType("BinaryAssetBuilder.Tokenizer.Plugin"));
            foreach(TargetPlatform p in new[]{TargetPlatform.Win32,TargetPlatform.Xbox360,TargetPlatform.PlayStation3,(TargetPlatform)99}) {
                original.Initialize(null,p);Layout(Check(original,p,""),p!=TargetPlatform.Win32);IndependentBindings(original,p);Enums(original,p);BrowserLayout(original,p);KindOf(original,p);ChildrenAndDuration(original,p);LeafCollections(original,p);ArmorAndLocomotorSets(original,p);VisualLeaves(original,p);AutoResolveFamily(original,p);PointerStringLists(original,p);ObjectResourceLists(original,p);GameDependencyGroup(original,p);CrushAndPlacement(original,p);GeometryGroup(original,p);AudioGroups(original,p);SkirmishAIGroup(original,p);EmptyModuleMarkers(original,p);ModuleFoundation(original,p);BodyFamily(original,p);ClientUpdateFamily(original,p);SoundPlaybackFamily(original,p);SoundSelectorFamily(original,p);LifecycleFamily(original,p);DieFamily(original,p);UpgradeFamily(original,p);
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
                foreach(string child in new[]{"<Foreign xmlns='other'/>","<Unknown/>"})Reject(p,"",child);
            }
            Console.WriteLine("PASS recovered GameObject root: fixtures="+fixtures+" threeBufferComparisons="+comparisons+" refusals="+refusals+" invarianceComparisons="+invarianceComparisons+"; 88 root attributes and thirty-eight child nodes including module containers with 31 base-only types plus complete nine-type Body family, six-type ClientUpdate family, four complete sound/EVA selectors, six lifecycle Updates, complete seven-type shared Die family, complete eleven-type Upgrade base group, animation/model-condition sound playback/default and complete SkirmishAIInformation (pointer StringLists on XSD-valid text surface), four targets including 99, independent defaults/list/sentinel checks; SDK not run");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}



