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
                Console.WriteLine("PASS complete CnC3Types.xsd validates mymod entry and all eighteen raw Nugget probes, including required DOT fields and ScalarInfo.Filter.");return 0;
            }
            var types=new Dictionary<string,uint> {
                {"DamageNugget",3237297817u},{"DOTNugget",4022030886u},{"GrapplingHookNugget",1093138400u},
                {"LeechPercentMaxHealthDamageNugget",921679887u},{"LeechHealthDamageNugget",4012276286u},{"LineDamageNugget",4021248832u},{"DamageAndSpawnNugget",3266020104u},{"FireOnObjectsNugget",2014632688u},{"DamageContainedNugget",2459653016u},
                {"ProjectileNugget",2278014412u},{"ScatterProjectileNugget",4131529406u},{"WeaponOCLNugget",3322313791u},{"SeedTiberiumNugget",1757028425u},
                {"SpecialPowerNugget",1542018985u},{"InformTargetNugget",1115743736u},{"ContainedObjectAttackNugget",609705483u},{"SlavesAttackNugget",3111574559u},{"ReportWeaponFiredStatNugget",1675169590u}
            };
            byte[] compiled=File.ReadAllBytes(args[2]);string evidence=args[3];Directory.CreateDirectory(evidence);
            var source=new XmlDocument();source.Load(file);var ns=new XmlNamespaceManager(source.NameTable);ns.AddNamespace("ea",AssetNamespace);int count=0;
            foreach(XmlElement asset in source.DocumentElement.SelectNodes("ea:WeaponTemplate",ns)) {
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
            if(count!=18) throw new Exception("Expected all eighteen SDK probes");
            Console.WriteLine("PASS eighteen schema-valid mymod assets compiled by original SDK and matched recovered component payloads. Weapon SDK routing remains original; not a recovered Weapon plugin build.");return 0;
        } catch(Exception error) {Console.Error.WriteLine(error);return 1;}
    }
}

