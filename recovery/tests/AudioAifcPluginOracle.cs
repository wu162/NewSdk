using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;

// Full plugin boundary; failed original import cannot count as a matching pass.
internal static class AudioAifcPluginOracle
{
    private static IAssetBuilderPlugin plugin;
    private static StreamWriter output;
    private static string root;
    private static int records;
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
        output.WriteLine(name + ":" + platform + ":BUFFER:" + Data(buffer.InstanceData) + ":" + Data(buffer.RelocationData) + ":" + Data(buffer.ImportsData) + ":CUSTOM:" + Data(File.ReadAllBytes(custom)));
        output.Flush(); ++records;
    }
    private static int Main(string[] args)
    {
        try {
            if (args.Length != 4 || IntPtr.Size != 4) throw new ArgumentException("Usage (x86): AudioAifcPluginOracle assembly fixtures output original|recovered");
            string originalName = "BinaryAssetBuilder.AudioCompiler";
            if (args[3] == "recovered" && File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, originalName + ".dll"))) throw new Exception("Original DLL present in recovered host");
            Assembly library = Assembly.LoadFrom(Path.GetFullPath(args[0]));
            plugin = (IAssetBuilderPlugin)Activator.CreateInstance(library.GetType(originalName + ".Plugin", true));
            root = Path.GetFullPath(args[1]); Directory.CreateDirectory(root);
            using (output = new StreamWriter(args[2], false, new UTF8Encoding(false))) AudioAifcInputCases.Run(root, Run);
            ((IDisposable)plugin).Dispose();
            if (records != 1440) throw new Exception("Expected1440 complete AIFC records; got " + records);
            if (args[3] == "recovered") foreach (Assembly loaded in AppDomain.CurrentDomain.GetAssemblies())
                if (loaded.GetName().Name == originalName) throw new Exception("Original DLL loaded in recovered host");
            Console.WriteLine("PASS AIFC/NONE complete plugin records: " + records); return 0;
        } catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
