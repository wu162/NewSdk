using System;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using Recovered;

unsafe partial class WeaponOracle
{
    static readonly string[] ScalarNuggetNames={"AttachNugget","SpendStolenTiberiumNugget","SecondaryDamageNugget","StripMaxHealthPercentNugget","SuppressionNugget","LuaEventNugget"};
    static readonly uint[] ScalarNuggetIds={576040660u,4286253161u,2475108259u,1622083673u,4104564135u,2010867179u};
    static string ScalarNugget(int kind,string attrs,string children) {
        return "<"+ScalarNuggetNames[kind]+" TypeId='"+ScalarNuggetIds[kind]+"' "+attrs+">"+children+"</"+ScalarNuggetNames[kind]+">";
    }
    static void ScalarNuggetLayout(TargetPlatform platform,int kind) {
        int[] tokens={1488,1488,1512,1524,1536,1524},sizes={44,44,52,56,60,52};
        var xml=new XmlDocument();xml.LoadXml(NuggetXml(ScalarNugget(kind,"",""),""));
        var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",AssetNamespace);
        var result=WeaponCompiler.ProcessInstance(Declaration(xml,ns),platform);bool big=platform!=TargetPlatform.Win32;
        int token=tokens[kind],at=token+332;
        if(result.InstanceData.Length!=at+sizes[kind] || Word(result.InstanceData,token+328,big)!=at ||
            Word(result.InstanceData,at,big)!=ScalarNuggetIds[kind] || Word(result.InstanceData,1284,big)!=2 ||
            Word(result.InstanceData,1288,big)!=ScalarNuggetIds[kind] || Word(result.InstanceData,1300,big)!=3351432196u ||
            Word(result.InstanceData,1316,big)!=at+40 || result.RelocationData.Length!=12)
            throw new Exception("Independent scalar nugget size/token/relocation: "+kind);
        if(kind==1 && Word(result.InstanceData,at+40,big)!=1) throw new Exception("AmountToSpend default");
        if(kind==2 && (Word(result.InstanceData,at+40,big)!=0 || Word(result.InstanceData,at+44,big)!=0xbf800000u || Word(result.InstanceData,at+48,big)!=0)) throw new Exception("Secondary defaults");
        // Original Tokenizer writes signed -261607243, i.e. 4033360053u.
        if(kind==2 && (Word(result.InstanceData,1332,big)!=4033360053u || Word(result.InstanceData,1336,big)!=1683514611u || Word(result.InstanceData,1340,big)!=at+48)) throw new Exception("Secondary PercentDamageToContained procedure key/type/member");
        if(kind==3 && (Word(result.InstanceData,at+40,big)!=0x3f000000u || Word(result.InstanceData,at+44,big)!=14 || Word(result.InstanceData,at+48,big)!=1 || Word(result.InstanceData,at+52,big)!=112)) throw new Exception("Strip defaults");
        if(kind==4 && (Word(result.InstanceData,at+40,big)!=0 || Word(result.InstanceData,at+44,big)!=0 || Word(result.InstanceData,at+48,big)!=0xbf800000u || Word(result.InstanceData,at+56,big)!=0)) throw new Exception("Suppression defaults and bool padding");
        if(kind==5) for(int j=40;j<52;j+=4) if(Word(result.InstanceData,at+j,big)!=0) throw new Exception("Lua absent string, flags and padding");
    }
    static void ScalarNuggets(IAssetBuilderPlugin plugin,TargetPlatform platform) {
        string[][] fields={new[] {"AttachModuleId"},new[] {"AmountToSpend"},new[] {"Amount","DamageTaperOff","PercentDamageToContained"},new[] {"AmountToStrip","DamageType","DeathType","DamageFXType"},new[] {"Suppression","DurationSeconds","SuppressionTaperOff","SuppressionArc","SuppressionArcInverted"},new[] {"EventName","SendToEnemies","SendToAllies","SendToNeutral"}};
        for(int kind=0;kind<6;++kind) {
            ScalarNuggetLayout(platform,kind);
            NuggetFixture(plugin,platform,"scalar-nugget-default-"+kind,ScalarNugget(kind,"",""),"");
            foreach(string field in fields[kind]) foreach(string value in new[] {"", "0", "-0", "-17.25", "2d1", "1e-40", "1e40", "1500ms", "-2s", "90d", "0.5r", "NaN", "garbage", "25%", "true", "false", "TRUE", "yes", "010", "0x7f", "2147483648", "-2147483649", "UNDEFINED", "NORMAL", "CANNON", "ALL", "fx\\7"})
                NuggetFixture(plugin,platform,"scalar-nugget-"+kind+"/"+field+"/"+value,ScalarNugget(kind,Attribute(field,value),""),"");
            for(int mask=0;mask<8;++mask) {
                string attrs=((mask&1)!=0?"ForbiddenTargetObjectStatus='ALL' ":"")+((mask&2)!=0?"ForbiddenTargetModelCondition='ALL' ":"");
                string children=(mask&4)!=0?"<SpecialObjectFilter StatusBitFlags='ALL'><IncludeThing>Obj</IncludeThing><ExcludeThing>Other</ExcludeThing></SpecialObjectFilter>":"";
                children+="<RequiredUpgrade>Up</RequiredUpgrade><ForbiddenUpgrade>Down</ForbiddenUpgrade>";
                NuggetFixture(plugin,platform,"scalar-nugget-base-"+kind+"/"+mask,ScalarNugget(kind,attrs,children),"");
            }
            foreach(string child in new[] {"<DamageScalarDetails/>","<AttackOffset/>","<VeterancyProjectiles/>","<SpecificBarrelOverride/>","<Filter/>"}) Reject(NuggetXml(ScalarNugget(kind,"",child),""));
            Reject(NuggetXml(ScalarNugget(kind,"Unknown='1'",""),""));
        }
        foreach(string value in new[] {"", "Obj", "OBJ", "aé", "中文", "a & <test>", " space", new string('x',257)}) {
            NuggetFixture(plugin,platform,"attach-hash-"+value,ScalarNugget(0,Attribute("AttachModuleId",value),""),"");
            NuggetFixture(plugin,platform,"lua-string-"+value,ScalarNugget(5,Attribute("EventName",value),"<SpecialObjectFilter StatusBitFlags='ALL'/><RequiredUpgrade>Up</RequiredUpgrade>"),"");
        }
        foreach(string value in new[] {"","x","abc","1234",new string('x',257)}) {
            var xml=new XmlDocument();xml.LoadXml(NuggetXml(ScalarNugget(5,Attribute("EventName",value),""),""));
            var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",AssetNamespace);
            var result=WeaponCompiler.ProcessInstance(Declaration(xml,ns),platform);bool big=platform!=TargetPlatform.Win32;
            const int at=1856,chars=1908;
            int padded=(value.Length+4)&~3;
            if(result.InstanceData.Length!=chars+padded || Word(result.InstanceData,at+40,big)!=value.Length || Word(result.InstanceData,at+44,big)!=chars || result.InstanceData[chars+value.Length]!=0)
                throw new Exception("Lua explicit string allocation/length/padding");
        }
        foreach(int count in new[] {0,1,2,7,17,33,129,255,256,257}) {
            var list=new StringBuilder();for(int j=0;j<count;++j) list.Append(ScalarNugget(j%6,"Radius='"+j+"'",""));
            NuggetFixture(plugin,platform,"scalar-nugget-list-"+count,list.ToString(),"");
            var child=new StringBuilder();for(int j=0;j<count;++j) child.Append("<RequiredUpgrade>Up_"+(j%7)+"</RequiredUpgrade><ForbiddenUpgrade>Down_"+j+"</ForbiddenUpgrade>");
            NuggetFixture(plugin,platform,"lua-allocation-order-"+count,ScalarNugget(5,"EventName='event' ForbiddenTargetObjectStatus='ALL' ForbiddenTargetModelCondition='ALL'",child.ToString()),"");
        }
        for(int mask=0;mask<32;++mask) {
            var list=new StringBuilder(Nugget(mask%9,"Damage='27.5'","<DamageScalarDetails><Filter/></DamageScalarDetails>"));
            for(int kind=0;kind<6;++kind) {
                list.Append(Activation(kind,"Radius='3'","<RequiredUpgrade>Up</RequiredUpgrade>"));
                list.Append(ScalarNugget(kind,"ForbiddenTargetObjectStatus='ALL' ForbiddenTargetModelCondition='ALL'","<SpecialObjectFilter StatusBitFlags='ALL'/>"));
            }
            NuggetFixture(plugin,platform,"scalar-activation-damage-mixed-"+mask,list.ToString(),"FireSound='s\\2' RequiredFiringObjectStatus='ALL'");
        }
        Reject(NuggetXml(ScalarNugget(0,"Lifetime='1s'",""),""));
        Reject(NuggetXml(ScalarNugget(1,"EventName='x'",""),""));
        Reject(NuggetXml(ScalarNugget(4,"Damage='1'",""),""));
    }
}

