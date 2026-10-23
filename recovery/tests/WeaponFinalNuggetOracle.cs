using System;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using Recovered;

unsafe partial class WeaponOracle
{
    static readonly string[] FinalNames={"MetaImpactNugget","TintObjectsNugget","AttributeModifierNugget"};
    static readonly uint[] FinalIds={2940373499u,3665090361u,3821195611u};
    static string FinalNugget(int kind,string attrs,string children) {
        return "<"+FinalNames[kind]+" TypeId='"+FinalIds[kind]+"' "+attrs+">"+children+"</"+FinalNames[kind]+">";
    }
    static AssetBuffer FinalResult(TargetPlatform platform,int kind,string attrs,string children) {
        var xml=new XmlDocument();xml.LoadXml(NuggetXml(FinalNugget(kind,attrs,children),""));
        var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",AssetNamespace);
        return WeaponCompiler.ProcessInstance(Declaration(xml,ns),platform);
    }
    static void FinalLayout(TargetPlatform platform,int kind) {
        var result=FinalResult(platform,kind,"","");bool big=platform!=TargetPlatform.Win32;
        int[] tokens={1680,1596,1548},sizes={96,72,64};int at=tokens[kind]+332;
        if(result.InstanceData.Length!=at+sizes[kind] || Word(result.InstanceData,tokens[kind]+328,big)!=at ||
            Word(result.InstanceData,at,big)!=FinalIds[kind] || result.RelocationData.Length!=12 || result.ImportsData.Length!=0 ||
            Word(result.InstanceData,1300,big)!=3351432196u)
            throw new Exception("Independent final nugget size/type/base/relocation: "+kind);
        uint[][] names={
            new uint[] {1895051624u,3037181377u,3612043902u,4271670327u,668485434u,1762766444u,942695279u,2095072366u,491083703u,123827726u,4063696236u,95831504u,2515231289u,1261291653u,1795125690u,4065019357u,162525091u},
            new uint[] {2873577633u,1562998579u,4118895867u,1416053366u,3095351546u,2654902107u},
            new uint[] {226106323u,701709449u,799690338u,480727292u,352777905u,4240413615u}
        };
        uint[][] types={
            new uint[] {1683514611u,1683514611u,1310124621u,2011428087u,1683514611u,1683514611u,1683514611u,1683514611u,2575722984u,1683514611u,2011428087u,2011428087u,2011428087u,2011428087u,1683514611u,1683514611u,3832456114u},
            new uint[] {2575722984u,2575722984u,2575722984u,1683514611u,1683514611u,2934399653u},
            new uint[] {165531635u,165531635u,799690338u,1310124621u,1173445096u,219975299u}
        };
        int[][] offsets={new[] {40,44,48,88,52,56,60,64,68,72,89,90,91,92,76,80,84},new[] {40,44,48,52,56,60},new[] {40,44,48,52,56,60}};
        for(int i=0;i<names[kind].Length;++i) {
            int entry=1308+12*i;
            uint pointer=kind==0 && i==16?0:kind==1 && i==5?1524u:(uint)(at+offsets[kind][i]);
            if(Word(result.InstanceData,entry,big)!=names[kind][i] || Word(result.InstanceData,entry+4,big)!=types[kind][i] || Word(result.InstanceData,entry+8,big)!=pointer)
                throw new Exception("Independent final nugget key/type/member: "+kind+"/"+i);
        }
        if(kind==0) {
            if(Word(result.InstanceData,at+60,big)!=0x3f800000u || Word(result.InstanceData,at+76,big)!=0x40000000u || Word(result.InstanceData,at+80,big)!=0x42c80000u || Word(result.InstanceData,at+84,big)!=0)
                throw new Exception("MetaImpact defaults/filter");
            for(int i=88;i<96;++i) if(result.InstanceData[at+i]!=0) throw new Exception("MetaImpact Boolean/padding defaults");
        }
        if(kind==1) {
            uint[] rgbNames={3593226867u,4213441201u,3809169639u};
            for(int i=0;i<3;++i) if(Word(result.InstanceData,1524+12*i,big)!=rgbNames[i] || Word(result.InstanceData,1528+12*i,big)!=1683514611u || Word(result.InstanceData,1532+12*i,big)!=at+60+4*i || Word(result.InstanceData,at+60+4*i,big)!=0)
                throw new Exception("Tint inline RGB procedure/default");
            if(Word(result.InstanceData,at+40,big)!=0x40000000u || Word(result.InstanceData,at+44,big)!=0x40000000u || Word(result.InstanceData,at+48,big)!=0x3f800000u || Word(result.InstanceData,at+52,big)!=0x3f800000u || Word(result.InstanceData,at+56,big)!=0x3f800000u)
                throw new Exception("Tint defaults");
        }
        if(kind==2) foreach(int offset in new[] {40,44,48,56,60}) if(Word(result.InstanceData,at+offset,big)!=0) throw new Exception("Modifier absent references/enums/mask");
    }
    static void FinalMask(TargetPlatform platform,string value,uint expected) {
        var result=FinalResult(platform,2,Attribute("AntiCategories",value),"");
        if(Word(result.InstanceData,1548+332+56,platform!=TargetPlatform.Win32)!=expected)
            throw new Exception("Independent AntiCategories: "+value);
    }
    static void FinalNuggets(IAssetBuilderPlugin plugin,TargetPlatform platform) {
        string[][] fields={
            new[] {"ShockWaveAmount","ShockWaveRadius","ShockWaveArc","ShockWaveArcInverted","ShockWaveTaperOff","ShockWaveSpeed","ShockWaveZMult","CyclonicFactor","ShockwaveDelaySeconds","Suppression","InvertShockWave","FlipDirection","OnlyWhenJustDied","ShockWaveClearRadius","ShockWaveClearWaveMult","ShockWaveClearFlingHeight"},
            new[] {"PreColorTime","PostColorTime","SustainedColorTime","Frequency","Amplitude"},
            new[] {"AttributeModifierName","AttributeModifierOwnerName","DamageFXType","DamageArc","AntiCategories","AntiFX"}
        };
        string[] values={"","-0","-17.25","2d1","1e40","1e-40","NaN","garbage","1500ms","-2s","90d","0.5r","true","false","TRUE","1","0","yes","fx","fx\\0","fx\\7","fx\\-1","fx\\2147483648","fx\\-2147483649"};
        for(int kind=0;kind<3;++kind) {
            FinalLayout(platform,kind);
            NuggetFixture(plugin,platform,"final-default-"+kind,FinalNugget(kind,"",""),"");
            foreach(string field in fields[kind]) foreach(string value in values)
                NuggetFixture(plugin,platform,"final-field-"+kind+"/"+field+"/"+value,FinalNugget(kind,Attribute(field,value),""),"");
            for(int mask=0;mask<16;++mask) {
                string attrs=((mask&1)!=0?"ForbiddenTargetObjectStatus='ALL' ":"")+((mask&2)!=0?"ForbiddenTargetModelCondition='ALL' ":"");
                string children=((mask&4)!=0?"<SpecialObjectFilter StatusBitFlags='ALL'><IncludeThing>Base</IncludeThing></SpecialObjectFilter>":"")+((mask&8)!=0?"<RequiredUpgrade>Up</RequiredUpgrade><ForbiddenUpgrade>Down</ForbiddenUpgrade>":"");
                if(kind==0) children+="<KillObjectFilter StatusBitFlags='ALL'><IncludeThing>Kill</IncludeThing><ExcludeThing>Other</ExcludeThing></KillObjectFilter>";
                if(kind==1) children+="<Color R='-0' G='0.25' B='255'/>";
                if(kind==2) attrs+="AttributeModifierName='m\\7' AttributeModifierOwnerName='m\\-1' AntiFX='fx\\9' AntiCategories='ALL'";
                NuggetFixture(plugin,platform,"final-pointers-"+kind+"/"+mask,FinalNugget(kind,attrs,children),"");
            }
            foreach(string child in new[] {"<DamageScalarDetails/>","<AttackOffset/>","<Filter/>","<DamageObjectFilter/>","<SpawnTemplate>Obj</SpawnTemplate>"}) Reject(NuggetXml(FinalNugget(kind,"",child),""));
            Reject(NuggetXml(FinalNugget(kind,"Unknown='1'",""),""));
            Reject(NuggetXml(FinalNugget(kind,"ShockWaveUnknown='1'",""),""));
        }
        foreach(string component in new[] {"R","G","B"}) foreach(string value in values)
            NuggetFixture(plugin,platform,"final-rgb-"+component+"/"+value,FinalNugget(1,"","<Color "+Attribute(component,value)+"/>"),"");
        foreach(string children in new[] {"<Color/>","<Color R='1'/><Color R='2'/>","<Color R='1' G='2' B='3'/>"})
            NuggetFixture(plugin,platform,"final-color-presence/"+children,FinalNugget(1,"",children),"");
        foreach(string children in new[] {"<KillObjectFilter/>","<KillObjectFilter Rule='ANY'/><KillObjectFilter Rule='NONE'/>"})
            NuggetFixture(plugin,platform,"final-kill-presence/"+children,FinalNugget(0,"",children),"");
        foreach(int count in new[] {0,1,2,7,17,33,129,255,256,257}) {
            var filter=new StringBuilder("<KillObjectFilter StatusBitFlags='ALL' StatusBitFlagsExclude='ALL'>");
            for(int i=0;i<count;++i) filter.Append("<IncludeThing>Obj_").Append(i).Append("</IncludeThing><ExcludeThing>Other_").Append(i).Append("</ExcludeThing>");
            filter.Append("</KillObjectFilter>");
            NuggetFixture(plugin,platform,"final-kill-list-"+count,FinalNugget(0,"ForbiddenTargetObjectStatus='ALL' ForbiddenTargetModelCondition='ALL'",filter.ToString()+"<SpecialObjectFilter/><RequiredUpgrade>Up</RequiredUpgrade>"),"");
            var list=new StringBuilder();
            for(int i=0;i<count;++i) list.Append(FinalNugget(i%3,"Radius='"+i+"'","<RequiredUpgrade>Up</RequiredUpgrade><ForbiddenUpgrade>Down</ForbiddenUpgrade>"));
            NuggetFixture(plugin,platform,"final-nugget-list-"+count,list.ToString(),"");
        }
        string[] categories={"BUFF","DEBUFF","FORMATION","INNATE_ARMOR","INNATE_AUTOHEAL","INNATE_DAMAGEMULT","INNATE_HEALTH","INNATE_VISION","LEADERSHIP","LEVEL","NONE","SPELL","STRUCTURE","STUN","WEAPON"};
        int[] bits={7,8,2,10,13,11,14,12,1,6,0,3,5,9,4};
        for(int i=0;i<categories.Length;++i) {
            FinalMask(platform,categories[i],1u<<bits[i]);
            foreach(string sign in new[] {"","+","-"}) foreach(string other in categories)
                NuggetFixture(plugin,platform,"final-category-"+i+sign+other,FinalNugget(2,Attribute("AntiCategories",sign+categories[i]+" "+other),""),"");
        }
        string[] special={"ALL","-ALL NONE","aLl -BUFF","BUFF -BUFF","-BUFF -BUFF","unknown","buff","+","-",""};
        uint[] expected={uint.MaxValue,uint.MaxValue,uint.MaxValue,0,0,128,128,128,128,0};
        for(int i=0;i<special.Length;++i) {
            FinalMask(platform,special[i],expected[i]);
            NuggetFixture(plugin,platform,"final-category-special-"+i,FinalNugget(2,Attribute("AntiCategories",special[i]),""),"");
        }
        var random=new Random(20261024);
        string[] atoms={"BUFF","DEBUFF","FORMATION","INNATE_ARMOR","INNATE_AUTOHEAL","INNATE_DAMAGEMULT","INNATE_HEALTH","INNATE_VISION","LEADERSHIP","LEVEL","NONE","SPELL","STRUCTURE","STUN","WEAPON","unknown","-","+","ALL","-ALL"};
        for(int i=0;i<257;++i) {
            var value=new StringBuilder();for(int j=0;j<1+i%17;++j) value.Append(j%3==0?"\t":" ").Append(random.Next(2)==0?"-":"+").Append(atoms[random.Next(atoms.Length)]);
            NuggetFixture(plugin,platform,"final-category-random-"+i,FinalNugget(2,Attribute("AntiCategories",value.ToString()),""),"");
        }
        foreach(string fx in new[] {"DEFAULT","UNDEFINED","ALIEN_BOMB","SOVIET_LASER","unknown",""}) NuggetFixture(plugin,platform,"final-fxtype-"+fx,FinalNugget(2,Attribute("DamageFXType",fx),""),"");
        for(int i=0;i<32;++i) {
            string list=FinalNugget(0,"ShockWaveAmount='27.5' InvertShockWave='true'","<KillObjectFilter><IncludeThing>Obj</IncludeThing></KillObjectFilter>")+FinalNugget(1,"Frequency='3.5'","<Color R='1' G='0.5' B='0.25'/>")+FinalNugget(2,"AttributeModifierName='m\\3' AntiCategories='ALL' AntiFX='fx\\9'","<SpecialObjectFilter StatusBitFlags='ALL'/>")+Effect(i%2,"","")+ScalarNugget(i%6,"","")+Activation(i%6,"","")+ReferenceNugget(i%9,"","")+Nugget(i%9,"","<DamageScalarDetails><Filter/></DamageScalarDetails>");
            NuggetFixture(plugin,platform,"final-mixed-"+i,list,"FireSound='s\\7' RequiredFiringObjectStatus='ALL'");
        }
        for(int kind=0;kind<3;++kind) {
            if(kind!=0) Reject(NuggetXml(FinalNugget(kind,"","<KillObjectFilter/>"),""));
            if(kind!=1) Reject(NuggetXml(FinalNugget(kind,"","<Color/>"),""));
        }
        foreach(string child in new[] {"<Color Unknown='1'/>","<Color><Unknown/></Color>","<Color xmlns:x='other' x:R='1'/>","<Color/><Color Unknown='1'/>"})
            Reject(NuggetXml(FinalNugget(1,"",child),""));
        foreach(string child in new[] {"<KillObjectFilter Unknown='1'/>","<KillObjectFilter><Unknown/></KillObjectFilter>","<KillObjectFilter><IncludeThing Unknown='1'/></KillObjectFilter>"})
            Reject(NuggetXml(FinalNugget(0,"",child),""));
        foreach(string child in new[] {"<Color/>","<KillObjectFilter/>"}) {
            Reject(NuggetXml(Nugget(0,"",child),""));
            Reject(NuggetXml(Effect(0,"",child),""));
        }
        Reject(NuggetXml(FinalNugget(0,"KillObjectFilter='x'",""),""));
        Reject(NuggetXml(FinalNugget(1,"Color='x'",""),""));
    }
}
