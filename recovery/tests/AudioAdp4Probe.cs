using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;

// Full plugin boundary; failed original import cannot count as a matching pass.
internal static class AudioAdp4Probe
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
        if (buffer.InstanceData.Length < 16 || BitConverter.ToInt32(buffer.InstanceData, 12) <= 0) throw new Exception("ADP4 frame count absent: " + name);
        output.WriteLine(name + ":" + platform + ":BUFFER:" + Data(buffer.InstanceData) + ":" + Data(buffer.RelocationData) + ":" + Data(buffer.ImportsData) + ":CUSTOM:" + Data(File.ReadAllBytes(custom)));
        output.Flush(); ++records;
    }
    private static void Probe()
    {
        foreach (int channels in new int[] { 1, 2 })
        foreach (int frames in new int[] { 7, 129, 2041 })
        foreach (int pattern in new int[] { 0, 1, 2, 3 })
        {
            string name = "adp4-" + channels + "-" + frames + "-" + pattern;
            string path = Path.Combine(root, name + ".aifc");
            AudioAifcInputCases.Write(path, 16, channels, frames, "empty", "ADP4");
            byte[] data = File.ReadAllBytes(path);
            // FORM12 + COMM(8+24) + SSND(8+8) => first payload byte60.
            // Every byte is two legal ADP4 nibbles; no Apple ima4 header is
            // inserted. Use the original native raw reader's measured layout.
            uint state = 0x12345678;
            for (int at = 60; at < data.Length; ++at) {
                state = unchecked(state * 1664525 + 1013904223);
                data[at] = pattern == 0 ? (byte)0 : pattern == 1 ? (byte)0xff : pattern == 2 ? (byte)at : (byte)(state >> 24);
            }
            File.WriteAllBytes(path, data);
            foreach (string codec in new string[] { "NONE", "XAS" })
            foreach (bool streamed in new bool[] { false, true }) {
                string xml = "<AudioFile xmlns='uri:ea.com:eala:asset' id='Fixture' File='" +
                    System.Security.SecurityElement.Escape(path) + "' PCCompression='" + codec +
                    "' PCQuality='75' IsStreamedOnPC='" + streamed.ToString().ToLowerInvariant() + "'/>";
                Run(name + "-" + codec + "-" + streamed, 0, xml, false);
            }
        }
    }

    private static int Main(string[] args)
    {
        try {
            if (args.Length != 4 || IntPtr.Size != 4) throw new ArgumentException("Usage (x86): AudioAdp4Probe assembly fixtures output original|recovered");
            string originalName = "BinaryAssetBuilder.AudioCompiler";
            if (args[3] == "recovered" && File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, originalName + ".dll"))) throw new Exception("Original DLL present in recovered host");
            Assembly library = Assembly.LoadFrom(Path.GetFullPath(args[0]));
            plugin = (IAssetBuilderPlugin)Activator.CreateInstance(library.GetType(originalName + ".Plugin", true));
            root = Path.GetFullPath(args[1]); Directory.CreateDirectory(root);
            using (output = new StreamWriter(args[2], false, new UTF8Encoding(false))) Probe();
            ((IDisposable)plugin).Dispose();
            if (records != 96) throw new Exception("Expected96 complete ADP4 records; got " + records);
            if (args[3] == "recovered") foreach (Assembly loaded in AppDomain.CurrentDomain.GetAssemblies())
                if (loaded.GetName().Name == originalName) throw new Exception("Original DLL loaded in recovered host");
            Console.WriteLine("PASS original-only AIFC/ADP4 records: " + records); return 0;
        } catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
