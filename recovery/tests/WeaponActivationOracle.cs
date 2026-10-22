using System;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using Recovered;

unsafe partial class WeaponOracle
{
    static readonly string[] ActivationNames={"ActivateCircularDamageNugget","ActivateLiftObjectNugget","ActivateLinearDamageNugget","ActivateStreamNugget","ActivateLaserNugget","TerrainCraterNugget"};
    static readonly uint[] ActivationIds={2360033115u,2769610658u,3694898303u,2159708756u,3915396258u,3520092115u};
    static string Activation(int kind,string attrs,string children) {
        return "<"+ActivationNames[kind]+" TypeId='"+ActivationIds[kind]+"' "+attrs+">"+children+"</"+ActivationNames[kind]+">";
    }
    static void ActivationLayout(TargetPlatform platform,int kind) {
        var xml=new XmlDocument();xml.LoadXml(NuggetXml(Activation(kind,"",""),""));
        var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",AssetNamespace);
        var result=WeaponCompiler.ProcessInstance(Declaration(xml,ns),platform);bool big=platform!=TargetPlatform.Win32;
        int token=kind<3?1488:kind==4?1524:1500,size=kind<3?44:kind==4?56:48,at=token+332;
        if(result.InstanceData.Length!=at+size || Word(result.InstanceData,token+328,big)!=at || Word(result.InstanceData,at,big)!=ActivationIds[kind] ||
            Word(result.InstanceData,1284,big)!=2 || Word(result.InstanceData,1288,big)!=ActivationIds[kind] ||
            Word(result.InstanceData,1300,big)!=3351432196u || Word(result.InstanceData,1316,big)!=at+40 ||
            result.RelocationData.Length!=12 || Word(result.RelocationData,0,big)!=token+260 || Word(result.RelocationData,8,big)!=uint.MaxValue)
            throw new Exception("Independent activation size/token/relocation: "+kind);
        for(int offset=40;offset<size;offset+=4) if(Word(result.InstanceData,at+offset,big)!=(kind<5 && offset==40?0x3f800000u:0u)) throw new Exception("Independent activation defaults: "+kind+"/"+offset);
    }
    static void ActivationNuggets(IAssetBuilderPlugin plugin,TargetPlatform platform) {
        for(int kind=0;kind<6;++kind) {
            ActivationLayout(platform,kind);
            NuggetFixture(plugin,platform,"activation-default-"+kind,Activation(kind,"",""),"");
            string[] fields=kind<3?new[] {"Lifetime"}:kind==3?new[] {"Lifetime","StreamId"}:kind==4?new[] {"Lifetime","LaserId","HitGroundFX","OverShootDistance"}:new[] {"Depth","Lift"};
            foreach(string field in fields) foreach(string value in new[] {"", "0", "-0", "-17.25", "2d1", "1e-40", "1e40", "1500ms", "-2s", "2d1s", "NaN", "garbage", "25%", "true", "010", "0x7f", "2147483648", "-2147483649", "fx\\0", "fx\\7", "fx\\-1", "fx\\2147483648", "fx\\-2147483649"})
                NuggetFixture(plugin,platform,"activation-"+kind+"/"+field+"/"+value,Activation(kind,Attribute(field,value),""),"");
            foreach(int mask in new[] {0,1,2,3,4,5,6,7}) {
                string attr="Radius='12.5' "+((mask&1)!=0?"ForbiddenTargetObjectStatus='ALL' ":"")+((mask&2)!=0?"ForbiddenTargetModelCondition='ALL' ":"");
                string child=(mask&4)!=0?"<SpecialObjectFilter Rule='ANY' StatusBitFlags='ALL'><IncludeThing>Obj</IncludeThing></SpecialObjectFilter>":"";
                child+="<RequiredUpgrade>Up</RequiredUpgrade><ForbiddenUpgrade>Down</ForbiddenUpgrade>";
                NuggetFixture(plugin,platform,"activation-pointers-"+kind+"/"+mask,Activation(kind,attr,child),"");
            }
            foreach(int count in new[] {0,1,2,7,33,129,257}) {
                var children=new StringBuilder("<SpecialObjectFilter><ExcludeThing>Other</ExcludeThing></SpecialObjectFilter>");
                for(int j=0;j<count;++j) children.Append("<RequiredUpgrade>Up_"+(j%5)+"</RequiredUpgrade><ForbiddenUpgrade>Down_"+j+"</ForbiddenUpgrade>");
                NuggetFixture(plugin,platform,"activation-upgrades-"+kind+"/"+count,Activation(kind,"",children.ToString()),"");
            }
            foreach(string child in new[] {"<DamageScalarDetails/>","<AttackOffset/>","<VeterancyProjectiles/>","<SpecificBarrelOverride/>","<Filter/>","<DamageObjectFilter/>"}) Reject(NuggetXml(Activation(kind,"",child),""));
            Reject(NuggetXml(Activation(kind,"Damage='1'",""),""));
            Reject(NuggetXml(Activation(kind,"Unknown='1'",""),""));
        }
        foreach(int count in new[] {0,1,2,7,17,33,129,255,256,257}) {
            var list=new StringBuilder();for(int j=0;j<count;++j) list.Append(Activation(j%6,"Radius='"+j+"'",""));
            NuggetFixture(plugin,platform,"activation-list-"+count,list.ToString(),"");
        }
        for(int mask=0;mask<32;++mask) {
            var list=new StringBuilder(ReferenceNugget(mask%9,"Radius='1'","<RequiredUpgrade>Up</RequiredUpgrade>"));
            list.Append(Nugget(mask%9,"Damage='12.5'","<DamageScalarDetails><Filter/></DamageScalarDetails>"));
            for(int kind=0;kind<6;++kind) list.Append(Activation(kind,"ForbiddenTargetObjectStatus='ALL' ForbiddenTargetModelCondition='ALL'","<SpecialObjectFilter Include='AIRCRAFT'/><RequiredUpgrade>Up</RequiredUpgrade>"));
            NuggetFixture(plugin,platform,"activation-mixed-"+mask,list.ToString(),"FireSound='s\\3' PreferredTargetBone='bone' RequiredFiringObjectStatus='ALL'");
        }
        Reject(NuggetXml(Activation(0,"LaserId='1'",""),""));
        Reject(NuggetXml(Activation(3,"OverShootDistance='1'",""),""));
        Reject(NuggetXml(Activation(5,"Lifetime='1s'",""),""));
        Reject(NuggetXml(ReferenceNugget(0,"Lifetime='1s'",""),""));
    }
}
