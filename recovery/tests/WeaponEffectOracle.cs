using System;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using Recovered;

unsafe partial class WeaponOracle
{
    static string Effect(int kind,string attrs,string children) {
        string name=kind==0?"ParalyzeNugget":"InformationWarfareNugget";
        return "<"+name+" TypeId='"+(kind==0?169470683u:978137461u)+"' "+attrs+">"+children+"</"+name+">";
    }
    static void EffectLayout(TargetPlatform platform,int kind,string attrs,uint expected) {
        var xml=new XmlDocument();xml.LoadXml(NuggetXml(Effect(kind,attrs,""),""));
        var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",AssetNamespace);
        var result=WeaponCompiler.ProcessInstance(Declaration(xml,ns),platform);bool big=platform!=TargetPlatform.Win32;
        int token=kind==0?1536:1512,size=kind==0?60:52,at=token+332;
        if(result.InstanceData.Length!=at+size || Word(result.InstanceData,token+328,big)!=at ||
            Word(result.InstanceData,at,big)!=(kind==0?169470683u:978137461u) ||
            Word(result.InstanceData,1300,big)!=3351432196u || result.RelocationData.Length!=12)
            throw new Exception("Independent effect size/base/relocation: "+kind);
        uint[] names=kind==0?new uint[] {3738876630u,1835767993u,100864519u,2251207440u,3573896194u}:new uint[] {2973902501u,2984380844u,2374890276u};
        uint[] types=kind==0?new uint[] {1310124621u,2575722984u,121953141u,121953141u,219975299u}:new uint[] {265587170u,1683514611u,2575722984u};
        for(int j=0;j<names.Length;++j) {
            int entry=1308+12*j;
            if(Word(result.InstanceData,entry,big)!=names[j] || Word(result.InstanceData,entry+4,big)!=types[j] || Word(result.InstanceData,entry+8,big)!=at+40+4*j)
                throw new Exception("Independent effect procedure key/type/member: "+kind+"/"+j);
        }
        if(kind==0 && (Word(result.InstanceData,at+48,big)!=expected || Word(result.InstanceData,at+52,big)!=4)) throw new Exception("Paralyze enum/default");
        if(kind==1 && Word(result.InstanceData,at+40,big)!=expected) throw new Exception("InfoWar mask/default/padding");
    }
    static void EffectNuggets(IAssetBuilderPlugin plugin,TargetPlatform platform) {
        for(int kind=0;kind<2;++kind) {
            EffectLayout(platform,kind,"",kind==0?4u:1u);
            NuggetFixture(plugin,platform,"effect-default-"+kind,Effect(kind,"",""),"");
            string[] fields=kind==0?new[] {"EffectArc","DurationSeconds","ParalyzeType","RemoveParalyzeType","ParalyzeFX"}:new[] {"InfoWarType","RadarJamRadius","RadarJamDuration"};
            foreach(string field in fields) foreach(string value in new[] {"", "-0", "-17.25", "2d1", "1e40", "1e-40", "NaN", "garbage", "1500ms", "-2s", "90d", "0.5r", "fx", "fx\\0", "fx\\7", "fx\\-1", "fx\\2147483648", "fx\\-2147483649"})
                NuggetFixture(plugin,platform,"effect-field-"+kind+"/"+field+"/"+value,Effect(kind,Attribute(field,value),""),"");
            for(int mask=0;mask<8;++mask) {
                string attr=((mask&1)!=0?"ForbiddenTargetObjectStatus='ALL' ":"")+((mask&2)!=0?"ForbiddenTargetModelCondition='ALL' ":"");
                string child=(mask&4)!=0?"<SpecialObjectFilter StatusBitFlags='ALL'><IncludeThing>Obj</IncludeThing><ExcludeThing>Other</ExcludeThing></SpecialObjectFilter>":"";
                NuggetFixture(plugin,platform,"effect-pointers-"+kind+"/"+mask,Effect(kind,attr,child+"<RequiredUpgrade>Up</RequiredUpgrade><ForbiddenUpgrade>Down</ForbiddenUpgrade>"),"");
            }
            foreach(string child in new[] {"<DamageScalarDetails/>","<AttackOffset/>","<Filter/>","<DamageObjectFilter/>","<SpawnTemplate>Obj</SpawnTemplate>"}) Reject(NuggetXml(Effect(kind,"",child),""));
            Reject(NuggetXml(Effect(kind,"Damage='1'",""),""));
        }
        string[] paralyze={"EMP","HELD","NONE","UNMANNED","USER_PARALYZE","emp","unknown","", " NONE"};
        uint[] values={0,3,4,2,1,0,0,0,0};
        for(int i=0;i<paralyze.Length;++i) {
            EffectLayout(platform,0,Attribute("ParalyzeType",paralyze[i]),values[i]);
            foreach(string field in new[] {"ParalyzeType","RemoveParalyzeType"}) NuggetFixture(plugin,platform,"effect-paralyze-"+field+"/"+i,Effect(0,Attribute(field,paralyze[i]),""),"");
        }
        string[] names={"NONE","RADAR_JAM","RESET_RADAR_ALL","RESET_RADAR_FREEZE","RESET_RADAR_HACKS","RESET_RADAR_JAM","RESET_RADAR_SPIES"};
        uint[] bits={1,2,64,32,16,4,8};
        for(int i=0;i<names.Length;++i) {
            EffectLayout(platform,1,Attribute("InfoWarType",names[i]),bits[i]);
            foreach(string sign in new[] {"", "+", "-"}) foreach(string other in names)
                NuggetFixture(plugin,platform,"effect-mask-"+i+sign+other,Effect(1,Attribute("InfoWarType",sign+names[i]+" "+other),""),"");
        }
        string[] special={"ALL","-ALL NONE","aLl -RADAR_JAM","RADAR_JAM -RADAR_JAM","-RADAR_JAM -RADAR_JAM","unknown","none","+","-",""};
        uint[] expected={uint.MaxValue,uint.MaxValue,uint.MaxValue,0,0,1,1,1,1,0};
        for(int i=0;i<special.Length;++i) {
            EffectLayout(platform,1,Attribute("InfoWarType",special[i]),expected[i]);
            NuggetFixture(plugin,platform,"effect-mask-special-"+i,Effect(1,Attribute("InfoWarType",special[i]),""),"");
        }
        var random=new Random(20261023);
        string[] atoms={"NONE","RADAR_JAM","RESET_RADAR_ALL","RESET_RADAR_FREEZE","RESET_RADAR_HACKS","RESET_RADAR_JAM","RESET_RADAR_SPIES","unknown","-","+","ALL","-ALL"};
        for(int i=0;i<257;++i) {
            var value=new StringBuilder();for(int j=0;j<1+i%13;++j) value.Append(j%3==0?"\t":" ").Append(random.Next(2)==0?"-":"+").Append(atoms[random.Next(atoms.Length)]);
            NuggetFixture(plugin,platform,"effect-mask-random-"+i,Effect(1,Attribute("InfoWarType",value.ToString()),""),"");
        }
        foreach(int count in new[] {0,1,2,7,17,33,129,255,256,257}) {
            var list=new StringBuilder();for(int i=0;i<count;++i) list.Append(Effect(i%2,"Radius='"+i+"'","<RequiredUpgrade>Up</RequiredUpgrade><ForbiddenUpgrade>Down</ForbiddenUpgrade>"));
            NuggetFixture(plugin,platform,"effect-list-"+count,list.ToString(),"");
        }
        for(int i=0;i<32;++i) {
            string list=Effect(0,"ParalyzeType='HELD' RemoveParalyzeType='EMP' ParalyzeFX='fx\\3'","<SpecialObjectFilter StatusBitFlags='ALL'/>")+Effect(1,"InfoWarType='ALL' RadarJamRadius='7.25' RadarJamDuration='1500ms'","<RequiredUpgrade>Up</RequiredUpgrade>")+ScalarNugget(5,"EventName='event'","")+Activation(i%6,"","")+ReferenceNugget(i%9,"","")+Nugget(i%9,"","<DamageScalarDetails><Filter/></DamageScalarDetails>");
            NuggetFixture(plugin,platform,"effect-mixed-"+i,list,"FireSound='s\\7' RequiredFiringObjectStatus='ALL'");
        }
        Reject(NuggetXml(Effect(0,"InfoWarType='NONE'",""),""));
        Reject(NuggetXml(Effect(1,"ParalyzeType='NONE'",""),""));
    }
}
