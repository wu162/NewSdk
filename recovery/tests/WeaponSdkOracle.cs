using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;

class WeaponSdkOracle
{
    static InstanceDeclaration Declaration(XmlDocument xml)
    {
        var namespaces = new XmlNamespaceManager(xml.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        var document = new AssetDeclarationDocument();
        var stateType = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        var state = FormatterServices.GetUninitializedObject(stateType);
        stateType.GetField("NamespaceManager").SetValue(state, namespaces);
        typeof(AssetDeclarationDocument).GetField("_Current", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(document, state);
        var instance = new InstanceDeclaration(document); instance.XmlNode = xml.DocumentElement;
        return instance;
    }
    static void Equal(byte[] a, byte[] b, string label)
    {
        if (a.Length != b.Length) throw new Exception(label + ": length");
        for (int i = 0; i < a.Length; ++i) if (a[i] != b[i]) throw new Exception(label + ": byte " + i);
    }
    static bool Contains(byte[] container, byte[] value)
    {
        for (int i = 0; i <= container.Length - value.Length; ++i) {
            int j = 0; while (j < value.Length && container[i+j] == value[j]) ++j;
            if (j == value.Length) return true;
        }
        return false;
    }
    static int Main(string[] args)
    {
        try {
            bool isolated = args[0] == "isolated";
            if (isolated) {
                foreach (string name in new[] { "BinaryAssetBuilder.Tokenizer.dll", "BinaryAssetBuilder.XmlCompiler.dll" })
                    if (File.Exists(Path.Combine(Path.GetDirectoryName(args[1]), name))) throw new Exception("Original compiler present in isolated host: " + name);
            }
            var plugin = (IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(args[1]).GetType(isolated ? "Recovered.Weapon.Plugin" : "BinaryAssetBuilder.Tokenizer.Plugin"));
            plugin.Initialize(null, TargetPlatform.Win32);
            var seen = new HashSet<string>();
            byte[] mod = File.ReadAllBytes(args[3]); int calls = 0;
            foreach (string file in Directory.GetFiles(args[2], "*.xml")) {
                var xml = new XmlDocument(); xml.Load(file);
                string id = xml.DocumentElement.GetAttribute("id");
                AssetBuffer original = plugin.ProcessInstance(Declaration(xml));
                string stem = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
                byte[] data = File.ReadAllBytes(stem + ".bin");
                Equal(original.InstanceData, data, id + "/instance");
                Equal(original.RelocationData, File.ReadAllBytes(stem + ".relo"), id + "/relo");
                Equal(original.ImportsData, File.ReadAllBytes(stem + ".imp"), id + "/imp");
                if(xml.DocumentElement.GetAttribute("TypeId")!="2496977262") throw new Exception("BAB root TypeId missing: "+id);
                if(id=="RecoveryNugget_All35" || id=="RecoveryNugget_Inherited") {
                    var namespaces=new XmlNamespaceManager(xml.NameTable);namespaces.AddNamespace("ea","uri:ea.com:eala:asset");
                    var nodes=xml.SelectNodes("/ea:WeaponTemplate/ea:Nuggets/*",namespaces);
                    var tags=new HashSet<string>();
                    foreach(XmlElement nugget in nodes) {string tag=nugget.GetAttribute("TypeId");if(String.IsNullOrEmpty(tag)||tag=="0") throw new Exception("Missing BAB Nugget tag");tags.Add(tag);}
                    if(nodes.Count!=35 || tags.Count!=35 || xml.SelectNodes("//ea:SpecialObjectFilter[@TypeId]",namespaces).Count==0) throw new Exception("All 35 branches or non-polymorphic annotation missing: "+id);
                    if(id=="RecoveryNugget_Inherited" && (xml.DocumentElement.HasAttribute("inheritFrom") || xml.DocumentElement.GetAttribute("AttackRange")!="333")) throw new Exception("BAB inheritance was not resolved");
                }
                if (id.StartsWith("RecoveryNugget_")) {
                    if (!Contains(mod, data)) throw new Exception(id + " not present in final mod.bin");
                    seen.Add(id);
                }
                Console.WriteLine("PASS {0} call {1}: verified buffers equal, bytes={2}", isolated ? "isolated recovered" : "original SDK replay", id, data.Length);
                ++calls;
            }
            var probes=new XmlDocument();probes.Load(args[4]);
            var ns=new XmlNamespaceManager(probes.NameTable);ns.AddNamespace("ea","uri:ea.com:eala:asset");
            var expectedIds=probes.SelectNodes("/ea:AssetDeclaration/ea:WeaponTemplate",ns);
            if(expectedIds.Count!=38) throw new Exception("Expected 38 source Weapon probes");
            foreach(XmlElement probe in expectedIds) {
                string id=probe.GetAttribute("id");
                if(!seen.Contains(id)) throw new Exception("No recovered plugin call for "+id);
            }
            foreach (string id in new[] { "RecoveryNugget_All35", "RecoveryNugget_Inherited" })
                if (!seen.Contains(id)) throw new Exception("No recovered plugin call for " + id);
            if (isolated) foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                string name = assembly.GetName().Name;
                if (name == "BinaryAssetBuilder.Tokenizer" || name == "BinaryAssetBuilder.XmlCompiler") throw new Exception("Original compiler loaded in isolation: " + name);
            }
            Console.WriteLine("PASS {0} verified calls, all 38 probe assets emitted; inherited XML tested after BAB preprocessing; isolated={1}.", calls, isolated);
            return 0;
        } catch (Exception e) { Console.Error.WriteLine(e); return 1; }
    }
}

