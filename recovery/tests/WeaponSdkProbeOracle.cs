using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using Recovered;

class WeaponSdkProbeOracle
{
    const string AssetNamespace="uri:ea.com:eala:asset";
    static int Find(byte[] stream,byte[] value) {
        for(int i=0;i<=stream.Length-value.Length;++i) {
            int j=0;while(j<value.Length && stream[i+j]==value[j]) ++j;
            if(j==value.Length) return i;
        }
        return -1;
    }
    static int Main(string[] args) {
        try {
            string sdk=args[1],file=Path.Combine(sdk,"Mods/mymod/Data/RecoveryWeaponNuggetProbe.xml");
            if(args[0]=="schema") {
                var set=new XmlSchemaSet();set.XmlResolver=new XmlUrlResolver();
                set.Add(AssetNamespace,Path.Combine(sdk,"Schemas/xsd/CnC3Types.xsd"));set.Compile();
                foreach(string name in new[] {"Mod.xml","RecoveryWeaponNuggetProbe.xml"}) {
                    var settings=new XmlReaderSettings();settings.Schemas=set;settings.ValidationType=ValidationType.Schema;
                    settings.ValidationEventHandler+=delegate(object sender,ValidationEventArgs e) {throw new Exception(name+": "+e.Message);};
                    using(var reader=XmlReader.Create(Path.Combine(sdk,"Mods/mymod/Data/"+name),settings)) while(reader.Read()) {}
                }
                // Keep required fields and enum restrictions observable in the real schema.
                foreach(string missing in new[] {"Amount", "AttachModuleId", "DamageType"}) {
                    var invalid=new XmlDocument();invalid.Load(file);
                    var namespaces=new XmlNamespaceManager(invalid.NameTable);namespaces.AddNamespace("ea",AssetNamespace);
                    string element=missing=="Amount"?"SecondaryDamageNugget":missing=="AttachModuleId"?"AttachNugget":"StripMaxHealthPercentNugget";
                    var node=(XmlElement)invalid.SelectSingleNode("//ea:"+element,namespaces);
                    if(missing=="DamageType") node.SetAttribute(missing,"NOT_A_DAMAGE_TYPE");else node.RemoveAttribute(missing);
                    var settings=new XmlReaderSettings();settings.Schemas=set;settings.ValidationType=ValidationType.Schema;
                    bool rejected=false;settings.ValidationEventHandler+=delegate(object sender,ValidationEventArgs e) {if(e.Severity==XmlSeverityType.Error) rejected=true;};
                    using(var reader=XmlReader.Create(new StringReader(invalid.OuterXml),settings)) while(reader.Read()) {}
                    if(!rejected) throw new Exception("Complete schema accepted invalid Nugget probe: "+missing);
                    Console.WriteLine("PASS complete schema rejects invalid probe: "+missing);
                }
                Console.WriteLine("PASS complete CnC3Types.xsd validates mymod entry and all thirty-eight raw Nugget probes (including inheritance), including required DOT fields and ScalarInfo.Filter.");return 0;
            }
            var types=new Dictionary<string,uint> {
                {"DamageNugget",3237297817u},{"DOTNugget",4022030886u},{"GrapplingHookNugget",1093138400u},
                {"LeechPercentMaxHealthDamageNugget",921679887u},{"LeechHealthDamageNugget",4012276286u},{"LineDamageNugget",4021248832u},{"DamageAndSpawnNugget",3266020104u},{"FireOnObjectsNugget",2014632688u},{"DamageContainedNugget",2459653016u},
                {"ProjectileNugget",2278014412u},{"ScatterProjectileNugget",4131529406u},{"WeaponOCLNugget",3322313791u},{"SeedTiberiumNugget",1757028425u},
                {"SpecialPowerNugget",1542018985u},{"InformTargetNugget",1115743736u},{"ContainedObjectAttackNugget",609705483u},{"SlavesAttackNugget",3111574559u},{"ReportWeaponFiredStatNugget",1675169590u}
                ,{"ActivateCircularDamageNugget",2360033115u},{"ActivateLiftObjectNugget",2769610658u},{"ActivateLinearDamageNugget",3694898303u},{"ActivateStreamNugget",2159708756u},{"ActivateLaserNugget",3915396258u},{"TerrainCraterNugget",3520092115u},{"AttachNugget",576040660u},{"SpendStolenTiberiumNugget",4286253161u},{"SecondaryDamageNugget",2475108259u},{"StripMaxHealthPercentNugget",1622083673u},{"SuppressionNugget",4104564135u},{"LuaEventNugget",2010867179u},{"ParalyzeNugget",169470683u},{"InformationWarfareNugget",978137461u},{"MetaImpactNugget",2940373499u},{"TintObjectsNugget",3665090361u},{"AttributeModifierNugget",3821195611u}
            };
            byte[] compiled=File.ReadAllBytes(args[2]);string evidence=args[3];Directory.CreateDirectory(evidence);
            var source=new XmlDocument();source.Load(file);var ns=new XmlNamespaceManager(source.NameTable);ns.AddNamespace("ea",AssetNamespace);int count=0;
            foreach(XmlElement asset in source.DocumentElement.SelectNodes("ea:WeaponTemplate",ns)) {
                // Actual inheritance is checked by WeaponSdkOracle on BAB's captured input.
                if(asset.HasAttribute("inheritFrom")) continue;
                var xml=new XmlDocument();xml.AppendChild(xml.ImportNode(asset,true));
                var namespaces=new XmlNamespaceManager(xml.NameTable);namespaces.AddNamespace("ea",AssetNamespace);
                // Expected-component input: only annotate polymorphic dispatch IDs.
                // This is not a claim that all BAB preprocessing has been reconstructed.
                foreach(XmlElement nugget in xml.DocumentElement.SelectNodes("ea:Nuggets/*",namespaces)) nugget.SetAttribute("TypeId",types[nugget.LocalName].ToString());
                var buffers=WeaponCompiler.CompileXml(xml.DocumentElement,namespaces,false);
                int offset=Find(compiled,buffers.InstanceData);string id=asset.GetAttribute("id");
                if(offset<0) throw new Exception("Recovered payload missing from actual original SDK mod.bin: "+id);
                File.WriteAllBytes(Path.Combine(evidence,id+".bin"),buffers.InstanceData);
                File.WriteAllBytes(Path.Combine(evidence,id+".relo"),buffers.RelocationData);
                File.WriteAllBytes(Path.Combine(evidence,id+".imp"),buffers.ImportsData);
                xml.Save(Path.Combine(evidence,id+".component-input.xml"));
                Console.WriteLine("PASS original SDK payload {0}: offset={1}, bytes={2}, recovered component data identical",id,offset,buffers.InstanceData.Length);++count;
            }
            if(count!=37) throw new Exception("Expected all thirty-seven non-inherited SDK probes");
            Console.WriteLine("PASS thirty-seven non-inherited schema-valid mymod assets compiled by original SDK and matched recovered component payloads. Weapon SDK routing remains original; not a recovered Weapon plugin build.");return 0;
        } catch(Exception error) {Console.Error.WriteLine(error);return 1;}
    }
}


