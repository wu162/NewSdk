using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;

class ArmorSdkOracle
{
    static void Validate(string sdk, string report)
    {
        var schemas = new XmlSchemaSet();
        schemas.XmlResolver = new XmlUrlResolver();
        schemas.Add("uri:ea.com:eala:asset", Path.Combine(sdk, "Schemas/xsd/CnC3Types.xsd"));
        schemas.Compile();
        // Existing mytank.xml contains xai:joinAction. BAB removes that directive
        // before validating the GameObject; use BAB itself for that existing file.
        int files = 0;
        foreach (string name in new[] { "Mod.xml", "RecoveryArmorProbe.xml" }) {
            var settings = new XmlReaderSettings(); settings.Schemas = schemas;
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += delegate(object sender, ValidationEventArgs e) {
                throw new Exception(name + ": " + e.Message);
            };
            using (var reader = XmlReader.Create(Path.Combine(sdk, "Mods/mymod/Data/" + name), settings))
                while (reader.Read()) { }
            ++files;
        }
        int rejected = 0;
        foreach (string attribute in new[] { "Damage", "Percent", "id" }) {
            var invalid = new XmlDocument();
            invalid.Load(Path.Combine(sdk, "Mods/mymod/Data/RecoveryArmorProbe.xml"));
            invalid.Schemas = schemas;
            if (attribute == "id") {
                var manager = new XmlNamespaceManager(invalid.NameTable); manager.AddNamespace("ea", "uri:ea.com:eala:asset");
                ((XmlElement)invalid.SelectSingleNode("//ea:ArmorTemplate", manager)).RemoveAttribute("id");
            }
            else {
                var manager = new XmlNamespaceManager(invalid.NameTable); manager.AddNamespace("ea", "uri:ea.com:eala:asset");
                var armor = (XmlElement)invalid.SelectSingleNode("//ea:Armor", manager);
                armor.SetAttribute(attribute, "INVALID_RECOVERY_PROBE");
            }
            bool failed = false;
            try { invalid.Validate(delegate(object sender, ValidationEventArgs e) { throw new Exception(e.Message); }); }
            catch (Exception) { failed = true; }
            if (!failed) throw new Exception("Schema accepted intentionally invalid " + attribute);
            ++rejected;
        }
        File.WriteAllText(report, "PASS " + files + " documents validated against CnC3Types.xsd with relative includes/imports; " + rejected + " invalid fixtures rejected.\n");
    }
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
            if (args[0] == "schema") { Validate(args[1], args[2]); return 0; }
            bool isolated = args[0] == "isolated";
            if (isolated) {
                foreach (string name in new[] { "BinaryAssetBuilder.Tokenizer.dll", "BinaryAssetBuilder.XmlCompiler.dll" })
                    if (File.Exists(Path.Combine(Path.GetDirectoryName(args[1]), name))) throw new Exception("Original compiler present in isolated host: " + name);
            }
            var plugin = (IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(args[1]).GetType(isolated ? "Recovered.Compiler.Plugin" : "BinaryAssetBuilder.Tokenizer.Plugin"));
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
                if (id.StartsWith("RecoveryArmorProbe_")) {
                    if (!Contains(mod, data)) throw new Exception(id + " not present in final mod.bin");
                    seen.Add(id);
                }
                Console.WriteLine("PASS {0} call {1}: verified buffers equal, bytes={2}", isolated ? "isolated recovered" : "original SDK replay", id, data.Length);
                ++calls;
            }
            foreach (string id in new[] { "RecoveryArmorProbe_Defaults", "RecoveryArmorProbe_NonDefault", "RecoveryArmorProbe_Inherited" })
                if (!seen.Contains(id)) throw new Exception("No recovered plugin call for " + id);
            if (isolated) foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                string name = assembly.GetName().Name;
                if (name == "BinaryAssetBuilder.Tokenizer" || name == "BinaryAssetBuilder.XmlCompiler") throw new Exception("Original compiler loaded in isolation: " + name);
            }
            Console.WriteLine("PASS {0} verified calls, all three probe assets emitted; inherited XML tested after BAB preprocessing; isolated={1}.", calls, isolated);
            return 0;
        } catch (Exception e) { Console.Error.WriteLine(e); return 1; }
    }
}
