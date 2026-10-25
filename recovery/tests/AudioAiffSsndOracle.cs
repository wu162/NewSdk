using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;

// Full plugin boundary; failed original import cannot count as a matching pass.
internal static class AudioAiffSsndOracle
{
    private static IAssetBuilderPlugin plugin;
    private static StreamWriter output;
    private static string root;
    private static int records;
    private static readonly System.Collections.Generic.Dictionary<string, string> canonical = new System.Collections.Generic.Dictionary<string, string>();
    private static string Data(byte[] data) { return data == null ? "<null>" : Convert.ToBase64String(data); }
    private static void Run(string name, int platform, string text, bool unused)
    {
        plugin.Initialize(null, (TargetPlatform)platform);
        var xml = new XmlDocument(); xml.LoadXml(text);
        var ns = new XmlNamespaceManager(xml.NameTable); ns.AddNamespace("ea", "uri:ea.com:eala:asset");
        var document = new AssetDeclarationDocument();
        Type current = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object state = FormatterServices.GetUninitializedObject(current);
        current.GetField("NamespaceManager").SetValue(state, ns);
        typeof(AssetDeclarationDocument).GetField("_Current", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(document, state);
        string custom = Path.Combine(root, "aifc-custom.bin"); if (File.Exists(custom)) File.Delete(custom);
        var declaration = new InstanceDeclaration(document); declaration.XmlNode = xml.DocumentElement; declaration.CustomDataPath = custom;
        AssetBuffer buffer = plugin.ProcessInstance(declaration);
        if (buffer == null || !File.Exists(custom)) throw new Exception("Required AIFC fixture produced no output: " + name);
        string result = "BUFFER:" + Data(buffer.InstanceData) + ":" + Data(buffer.RelocationData) + ":" + Data(buffer.ImportsData) + ":CUSTOM:" + Data(File.ReadAllBytes(custom));
        string key = System.Text.RegularExpressions.Regex.Replace(name, "-block[0-9]+-", "-block0-") + ":" + platform;
        if (name.Contains("-block0-")) canonical.Add(key, result);
        else if (!canonical.ContainsKey(key) || canonical[key] != result) throw new Exception("SSND blockSize changed canonical buffers/custom: " + name + "/" + platform);
        output.WriteLine(name + ":" + platform + ":" + result);
        output.Flush(); ++records;
    }
    private static int Main(string[] args)
    {
        try {
            if (args.Length != 4 || IntPtr.Size != 4) throw new ArgumentException("Usage (x86): AudioAiffSsndOracle assembly fixtures output original|recovered");
            string originalName = "BinaryAssetBuilder.AudioCompiler";
            if (args[3] == "recovered" && File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, originalName + ".dll"))) throw new Exception("Original DLL present in recovered host");
            Assembly library = Assembly.LoadFrom(Path.GetFullPath(args[0]));
            plugin = (IAssetBuilderPlugin)Activator.CreateInstance(library.GetType(originalName + ".Plugin", true));
            root = Path.GetFullPath(args[1]); Directory.CreateDirectory(root);
            using (output = new StreamWriter(args[2], false, new UTF8Encoding(false))) AudioAiffSsndCases.Run(root, Run);
            ((IDisposable)plugin).Dispose();
            if (canonical.Count != 456) throw new Exception("Expected456 canonical SSND records");
            if (records != 3192) throw new Exception("Expected3192 complete AIFC records; got " + records);
            if (args[3] == "recovered") foreach (Assembly loaded in AppDomain.CurrentDomain.GetAssemblies())
                if (loaded.GetName().Name == originalName) throw new Exception("Original DLL loaded in recovered host");
            Console.WriteLine("PASS SSND blockSize complete plugin records: " + records); return 0;
        } catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
