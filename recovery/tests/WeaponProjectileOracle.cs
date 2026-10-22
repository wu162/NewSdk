using System;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using Recovered;

unsafe partial class WeaponOracle
{
    static readonly uint[] ReferenceIds={2278014412u,4131529406u,3322313791u,1757028425u,1542018985u,1115743736u,609705483u,3111574559u,1675169590u};
    static string ReferenceNugget(int kind,string attrs,string children) {
        string name=new[] {"ProjectileNugget","ScatterProjectileNugget","WeaponOCLNugget","SeedTiberiumNugget","SpecialPowerNugget","InformTargetNugget","ContainedObjectAttackNugget","SlavesAttackNugget","ReportWeaponFiredStatNugget"}[kind];
        return "<"+name+" TypeId='"+ReferenceIds[kind]+"' "+attrs+">"+children+"</"+name+">";
    }
    static void ReferenceLayout(TargetPlatform platform,int kind) {
        var xml=new XmlDocument();xml.LoadXml(NuggetXml(ReferenceNugget(kind,"",""),""));
        var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",AssetNamespace);
        var result=WeaponCompiler.ProcessInstance(Declaration(xml,ns),platform);bool big=platform!=TargetPlatform.Win32;
        int tokens=kind==0?1560:kind==3?1548:kind==4?1488:kind>=6?1476:1500,size=kind==0?68:kind==3?56:kind==4?44:kind>=6?40:48,at=tokens+332;
        if(result.InstanceData.Length!=at+size || Word(result.InstanceData,tokens+256,big)!=1 ||
            Word(result.InstanceData,tokens+260,big)!=tokens+328 || Word(result.InstanceData,tokens+328,big)!=at ||
            Word(result.InstanceData,at,big)!=ReferenceIds[kind] || Word(result.InstanceData,at+4,big)!=4 ||
            Word(result.InstanceData,1284,big)!=2 || Word(result.InstanceData,1288,big)!=ReferenceIds[kind] ||
            Word(result.InstanceData,1296,big)!=1 || Word(result.InstanceData,1300,big)!=(kind==3?3322313791u:3351432196u) ||
            result.RelocationData.Length!=12 || Word(result.RelocationData,0,big)!=tokens+260 ||
            Word(result.RelocationData,4,big)!=tokens+328 || Word(result.RelocationData,8,big)!=uint.MaxValue || result.ImportsData.Length!=0)
            throw new Exception("Independent reference nugget layout: "+kind);
        for(int j=40;j<size;j+=4) if(Word(result.InstanceData,at+j,big)!=(kind==0 && j==48?5u:kind==5 && j==40?0x3f59999au:kind==5 && j==44?0x3f933333u:0u)) throw new Exception("Reference defaults/padding: "+kind+"/"+j);
        if(kind==0 && (Word(result.InstanceData,1304,big)!=1392 || Word(result.InstanceData,1352,big)!=0 || Word(result.InstanceData,1376,big)!=0)) throw new Exception("Projectile optional pointer token defaults");
    }
    static void ProjectileNestedLayout(TargetPlatform platform) {
        var xml=new XmlDocument();xml.LoadXml(NuggetXml(ReferenceNugget(0,"WarheadTemplate='w\\2' ProjectileTemplate='p\\3'", "<AttackOffset x='1' y='-2' z='3'/><VeterancyProjectiles VeterancyLevel='HEROIC' ProjectileTemplate='p\\7'/><SpecificBarrelOverride>-7</SpecificBarrelOverride>"),""));
        var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",AssetNamespace);
        var result=WeaponCompiler.ProcessInstance(Declaration(xml,ns),platform);bool big=platform!=TargetPlatform.Win32;
        const int tokens=1608,at=1940;
        if(result.InstanceData.Length!=2032 || Word(result.InstanceData,tokens+328,big)!=at ||
            Word(result.InstanceData,at+52,big)!=2008 || Word(result.InstanceData,at+56,big)!=1 ||
            Word(result.InstanceData,at+60,big)!=2020 || Word(result.InstanceData,at+64,big)!=2028 ||
            Word(result.InstanceData,2008,big)!=0x3f800000u || Word(result.InstanceData,2012,big)!=0xc0000000u ||
            Word(result.InstanceData,2016,big)!=0x40400000u || Word(result.InstanceData,2020,big)!=3 ||
            Word(result.InstanceData,2028,big)!=0xfffffff9u || Word(result.InstanceData,1352,big)!=2008 ||
            Word(result.InstanceData,1376,big)!=2028)
            throw new Exception("Independent projectile nested allocation/count/token/endian layout");
    }
    static void ProjectileNuggets(IAssetBuilderPlugin plugin,TargetPlatform platform) {
        ProjectileNestedLayout(platform);
        for(int kind=0;kind<9;++kind) {
            ReferenceLayout(platform,kind);
            NuggetFixture(plugin,platform,"reference-default-"+kind,ReferenceNugget(kind,"",""),"");
            foreach(string field in new[] {"PartitionFilterTestType","Radius","ForbiddenTargetObjectStatus","ForbiddenTargetModelCondition"})
                foreach(string value in new[] {"", "0", "-0", "-17.25", "1e40", "2d1", "ALL", "-ALL TOPPLED", "AFLAME", "TOPPLED", "EXTENTS_3D", "SPHERE", "unknown"})
                    NuggetFixture(plugin,platform,"reference-base-"+kind+"/"+field+"/"+value,ReferenceNugget(kind,Attribute(field,value),""),"");
            for(int mask=0;mask<8;++mask) {
                string attrs=((mask&1)!=0?"ForbiddenTargetObjectStatus='ALL' ":"")+((mask&2)!=0?"ForbiddenTargetModelCondition='ALL' ":"");
                string child=(mask&4)!=0?"<SpecialObjectFilter StatusBitFlags='ALL'><IncludeThing>Obj</IncludeThing><ExcludeThing>Other</ExcludeThing></SpecialObjectFilter>":"";
                child+="<RequiredUpgrade>Up</RequiredUpgrade><ForbiddenUpgrade>Down</ForbiddenUpgrade>";
                NuggetFixture(plugin,platform,"reference-base-pointers-"+kind+"/"+mask,ReferenceNugget(kind,attrs,child),"");
            }
            foreach(string child in new[] {"<DamageScalarDetails/>","<DamageObjectFilter/>","<Filter/>","<SpawnTemplate>Obj</SpawnTemplate>"}) Reject(NuggetXml(ReferenceNugget(kind,"",child),""));
            Reject(NuggetXml(ReferenceNugget(kind,"Damage='1'",""),""));
            Reject(NuggetXml(ReferenceNugget(kind,"InvalidTargetStatus='ALL'",""),""));
        }
        foreach(string field in new[] {"MinTimeToImpactFudgeFactor","MaxTimeToImpactFudgeFactor"}) foreach(string value in new[] {"", "-0", "-17.25", "2d1", "1e-40", "1e40", "NaN", "garbage"})
            NuggetFixture(plugin,platform,"inform-"+field+"/"+value,ReferenceNugget(5,Attribute(field,value),""),"");
        foreach(string value in new[] {"", "resource", "r\\0", "r\\7", "r\\-1", "r\\2147483648", "r\\-2147483649"})
            NuggetFixture(plugin,platform,"special-power-"+value,ReferenceNugget(4,Attribute("SpecialPowerTemplate",value),""),"");
        foreach(string field in new[] {"WarheadTemplate","ProjectileTemplate","WeaponOCL"}) {
            int kind=field=="WeaponOCL"?2:0;
            foreach(string value in new[] {"", "resource", "r\\0", "r\\7", "r\\-1", "r\\2147483648", "r\\-2147483649", "r\\ 12tail", "a\\9\\2"})
                NuggetFixture(plugin,platform,"reference-"+field+"/"+value,ReferenceNugget(kind,Attribute(field,value),""),"");
        }
        foreach(string field in new[] {"FieldAmount","SpawnedInFieldBonus"}) foreach(string value in new[] {"", "-0", "-17.25", "2d1", "1e-40", "1e40", "NaN", "garbage"})
            NuggetFixture(plugin,platform,"seed-"+field+"/"+value,ReferenceNugget(3,Attribute(field,value)+" WeaponOCL='ocl\\7' TargetAsPrimaryObject='true'", ""),"");
        foreach(string value in new[] {"", "false", "true", "1", "0", "ocl\\0", "ocl\\-1", "ocl\\2147483648"}) {
            NuggetFixture(plugin,platform,"seed-ocl-"+value,ReferenceNugget(3,Attribute("WeaponOCL",value),""),"");
            NuggetFixture(plugin,platform,"seed-bool-"+value,ReferenceNugget(3,Attribute("TargetAsPrimaryObject",value),""),"");
        }
        foreach(string value in new[] {"NO_WEAPON","PRIMARY_WEAPON","QUATERNARY_WEAPON","QUINARY_WEAPON","SECONDARY_WEAPON","TERTIARY_WEAPON","primary_weapon","", "unknown"})
            NuggetFixture(plugin,platform,"slot-"+value,ReferenceNugget(0,Attribute("WeaponLaunchBoneSlotOverride",value),""),"");
        foreach(string field in new[] {"ScatterMin","ScatterMax"}) foreach(string value in new[] {"", "-0", "-17.25", "2d1", "1e-40", "1e40", "NaN", "garbage"})
            NuggetFixture(plugin,platform,"scatter-projectile-"+field+"/"+value,ReferenceNugget(1,Attribute(field,value),""),"");
        foreach(string value in new[] {"", "false", "true", "TRUE", "1", "0", "t", "yes", " true"})
            NuggetFixture(plugin,platform,"ocl-bool-"+value,ReferenceNugget(2,Attribute("TargetAsPrimaryObject",value),""),"");
        foreach(string axis in new[] {"x","y","z"}) foreach(string value in new[] {"", "-0", "-17.25", "2d1", "1e-40", "1e40", "NaN", "garbage"})
            NuggetFixture(plugin,platform,"offset-"+axis+"/"+value,ReferenceNugget(0,"","<AttackOffset "+Attribute(axis,value)+"/>"),"");
        foreach(string value in new[] {"", "0", "-1", "010", "0x7f", "2147483648", "-2147483649", "garbage"})
            NuggetFixture(plugin,platform,"barrel-"+value,ReferenceNugget(0,"","<SpecificBarrelOverride>"+value+"</SpecificBarrelOverride>"),"");
        foreach(string level in new[] {"ELITE","HEROIC","INVALID","REGULAR","VETERAN","elite","", "unknown"})
            foreach(string reference in new[] {"", "Obj", "o\\0", "o\\7", "o\\-1", "o\\2147483648"})
                NuggetFixture(plugin,platform,"veteran-"+level+"/"+reference,ReferenceNugget(0,"","<VeterancyProjectiles "+Attribute("VeterancyLevel",level)+" "+Attribute("ProjectileTemplate",reference)+"/>"),"");
        foreach(int count in new[] {0,1,2,3,7,17,33,129,255,256,257}) {
            var child=new StringBuilder("<AttackOffset x='1' y='-2' z='3'/><SpecificBarrelOverride>-7</SpecificBarrelOverride>");
            for(int j=0;j<count;++j) child.Append("<VeterancyProjectiles VeterancyLevel='"+new[] {"REGULAR","VETERAN","ELITE","HEROIC","INVALID"}[j%5]+"' ProjectileTemplate='Obj\\"+(j%7)+"'/>");
            child.Append("<SpecialObjectFilter StatusBitFlags='ALL'/><RequiredUpgrade>Base</RequiredUpgrade><ForbiddenUpgrade>Down</ForbiddenUpgrade>");
            NuggetFixture(plugin,platform,"veteran-list-"+count,ReferenceNugget(0,"WarheadTemplate='w\\4' ProjectileTemplate='p\\2' ForbiddenTargetObjectStatus='ALL' ForbiddenTargetModelCondition='ALL'",child.ToString()),"");
            var list=new StringBuilder();for(int j=0;j<count;++j) list.Append(ReferenceNugget(j%9,"Radius='"+j+"'",""));
            NuggetFixture(plugin,platform,"reference-list-"+count,list.ToString(),"");
        }
        for(int mask=0;mask<8;++mask) {
            string child=((mask&1)!=0?"<AttackOffset/>":"")+((mask&2)!=0?"<SpecificBarrelOverride/>":"")+((mask&4)!=0?"<VeterancyProjectiles/>":"");
            NuggetFixture(plugin,platform,"projectile-optionals-"+mask,ReferenceNugget(0,"",child),"");
        }
        NuggetFixture(plugin,platform,"projectile-first-child",ReferenceNugget(0,"","<AttackOffset x='1'/><AttackOffset x='2'/><SpecificBarrelOverride>3</SpecificBarrelOverride><SpecificBarrelOverride>4</SpecificBarrelOverride>"),"");
        NuggetFixture(plugin,platform,"projectile-child-order",ReferenceNugget(0,"","<SpecificBarrelOverride>3</SpecificBarrelOverride><VeterancyProjectiles/><AttackOffset x='1'/><VeterancyProjectiles ProjectileTemplate='Obj\\2'/>"),"");
        for(int j=0;j<32;++j) {
            string content=ReferenceNugget(2,"WeaponOCL='ocl\\3' TargetAsPrimaryObject='true'","<RequiredUpgrade>Up</RequiredUpgrade>")+Nugget(j%9,"Damage='12.5'","<SpecialObjectFilter StatusBitFlags='ALL'/>")+ReferenceNugget(0,"WarheadTemplate='w\\2' ProjectileTemplate='p\\4'","<AttackOffset x='1' y='2'/><VeterancyProjectiles ProjectileTemplate='p\\7'/><SpecificBarrelOverride>-2</SpecificBarrelOverride>")+ReferenceNugget(1,"ScatterMin='-2' ScatterMax='3'","");
            NuggetFixture(plugin,platform,"reference-damage-mixed-"+j,content,"FireSound='snd\\1' RequiredFiringObjectStatus='ALL'");
        }
        foreach(int kind in new[] {1,2,3,4,5,6,7,8}) foreach(string child in new[] {"<AttackOffset/>","<VeterancyProjectiles/>","<SpecificBarrelOverride/>"}) Reject(NuggetXml(ReferenceNugget(kind,"",child),""));
        foreach(string child in new[] {"<AttackOffset Unknown='1'/>","<AttackOffset><Nested/></AttackOffset>","<VeterancyProjectiles Unknown='1'/>","<SpecificBarrelOverride Unknown='1'/>"}) Reject(NuggetXml(ReferenceNugget(0,"",child),""));
        Reject(NuggetXml(Nugget(0,"","<AttackOffset/>"),""));
    }
}
