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
        int files = 0;
        foreach (string name in new[] { "Mod.xml", "RecoveryArmorProbe.xml", "mytank.xml" }) {
            var settings = new XmlReaderSettings(); settings.Schemas = schemas;
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += delegate(object sender, ValidationEventArgs e) {
                throw new Exception(name + ": " + e.Message);
            };
            using (var reader = XmlReader.Create(Path.Combine(sdk, "Mods/mymod/Data/" + name), settings))
                while (reader.Read()) { }
            ++files;
        }
        File.WriteAllText(report, "PASS " + files + " documents validated against CnC3Types.xsd with relative includes/imports.\n");
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
            var plugin = (IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(args[1]).GetType("BinaryAssetBuilder.Tokenizer.Plugin"));
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
                Console.WriteLine("PASS SDK call {0}: original buffers equal, bytes={1}", id, data.Length);
                ++calls;
            }
            foreach (string id in new[] { "RecoveryArmorProbe_Defaults", "RecoveryArmorProbe_NonDefault", "RecoveryArmorProbe_Inherited" })
                if (!seen.Contains(id)) throw new Exception("No recovered plugin call for " + id);
            Console.WriteLine("PASS {0} actual SDK plugin calls, all three probe assets emitted; inherited XML tested after BAB preprocessing.", calls);
            return 0;
        } catch (Exception e) { Console.Error.WriteLine(e); return 1; }
    }
}
