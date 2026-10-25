using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using BinaryAssetBuilder.Core;
using Recovered.Compiler.W3x;

// Exercises the loadable plugin's real ProcessInstance backend. The same host
// also runs in a separate directory/process with no original W3X assembly.
internal static class W3xPluginOracle
{
    const string Ns = "uri:ea.com:eala:asset";
    const string OriginalName = "BinaryAssetBuilder.W3XCompiler";
    static long fixtures, comparisons;
    static string label = "startup";
    static bool standalone;
    static IAssetBuilderPlugin original;

    static void NoOriginal()
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            if (String.Equals(assembly.GetName().Name,OriginalName,StringComparison.OrdinalIgnoreCase))
                throw new Exception("Original W3X compiler loaded during standalone plugin validation: " + assembly.FullName);
    }
    static void Loaded(object sender, AssemblyLoadEventArgs args)
    {
        if (String.Equals(args.LoadedAssembly.GetName().Name,OriginalName,StringComparison.OrdinalIgnoreCase))
            throw new Exception("Original W3X compiler assembly load is forbidden in standalone plugin validation.");
    }
    static InstanceDeclaration Declaration(XmlDocument xml, XmlNamespaceManager namespaces)
    {
        AssetDeclarationDocument document = new AssetDeclarationDocument();
        Type stateType = typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic);
        object state = FormatterServices.GetUninitializedObject(stateType);
        stateType.GetField("NamespaceManager").SetValue(state,namespaces);
        typeof(AssetDeclarationDocument).GetField("_Current",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(document,state);
        InstanceDeclaration declaration = new InstanceDeclaration(document);
        declaration.XmlNode = xml.DocumentElement;
        return declaration;
    }
    static byte[] Buffer(BinaryReader reader)
    {
        int length = reader.ReadInt32();
        if (length < 0 || length > reader.BaseStream.Length-reader.BaseStream.Position)
            throw new InvalidDataException(label + " invalid capture buffer length");
        byte[] bytes = reader.ReadBytes(length);
        if (bytes.Length != length) throw new EndOfStreamException(label + " truncated capture buffer");
        return bytes;
    }
    static void Equal(string name, byte[] expected, byte[] actual)
    {
        if (actual == null || expected.Length != actual.Length)
            throw new Exception(label + " " + name + " length expected=" + expected.Length + " actual=" + (actual == null ? -1 : actual.Length));
        for (int i = 0; i < expected.Length; ++i)
            if (expected[i] != actual[i]) throw new Exception(label + " " + name + " byte=" + i + " expected=" + expected[i] + " actual=" + actual[i]);
        ++comparisons;
    }
    static void Equal(string name, byte[][] expected, AssetBuffer actual)
    {
        if (actual == null) throw new Exception(label + " " + name + " returned null");
        Equal(name+"/instance",expected[0],actual.InstanceData);
        Equal(name+"/relocations",expected[1],actual.RelocationData);
        Equal(name+"/imports",expected[2],actual.ImportsData);
    }
    static string Replay(string path)
    {
        if (!Path.IsPathRooted(path)) throw new Exception("Capture paths must be absolute");
        using (FileStream stream = File.OpenRead(path))
        using (BinaryReader reader = new BinaryReader(stream))
        {
            if (reader.ReadString() != "W3X-FIXTURE-1") throw new InvalidDataException("Unsupported capture format");
            string route = reader.ReadString();
            if (route != "animation" && route != "mesh") throw new InvalidDataException("Unknown capture route");
            long count = 0;
            while (stream.Position < stream.Length)
            {
                int platform = reader.ReadInt32();
                label = route + " fixture=" + count + " platform=" + platform;
                string xmlText = reader.ReadString();
                byte[][] expected = new byte[][] {Buffer(reader),Buffer(reader),Buffer(reader)};
                XmlDocument xml = new XmlDocument(); xml.LoadXml(xmlText);
                string rootName = route == "animation" ? "W3DAnimation" : "W3DMesh";
                if (xml.DocumentElement == null || xml.DocumentElement.LocalName != rootName || xml.DocumentElement.NamespaceURI != Ns)
                    throw new InvalidDataException(label + " wrong XML root for capture route");
                XmlNamespaceManager namespaces = new XmlNamespaceManager(xml.NameTable); namespaces.AddNamespace("ea",Ns);
                InstanceDeclaration declaration = Declaration(xml,namespaces);
                // Resolve the exact configuration identity and its public ctor.
                Type pluginType = Type.GetType("Recovered.Compiler.W3x.NativePlugin, Recovered.W3x.Plugin",true);
                NativePlugin plugin = (NativePlugin)Activator.CreateInstance(pluginType);
                IAssetBuilderPlugin dispatch = plugin;
                dispatch.Initialize(null,(TargetPlatform)platform);
                Equal("plugin ProcessInstance",expected,dispatch.ProcessInstance(declaration));
                // Direct public handler adapters exercise the same backend.
                if (count == 0)
                    Equal("public Handle",expected,route == "animation" ? plugin.HandleAnimation(declaration) : plugin.HandleMesh(declaration));
                if (!standalone)
                {
                    original.Initialize(null,(TargetPlatform)platform);
                    Equal("original ProcessInstance",expected,original.ProcessInstance(declaration));
                    if (plugin.GetVersionNumber() != original.GetVersionNumber() || plugin.GetAllTypesHash() != original.GetAllTypesHash())
                        throw new Exception(label + " plugin metadata differs from original");
                }
                plugin.Dispose();
                ++fixtures; ++count;
            }
            long expectedCount = route == "animation" ? 675 : 666;
            if (count != expectedCount) throw new Exception(route + " fixture count expected=" + expectedCount + " actual=" + count);
            Console.WriteLine("PASS W3X native plugin route={0} fixtures={1} standalone={2}",route,count,standalone);
            return route;
        }
    }
    static int Main(string[] args)
    {
        try
        {
            if (IntPtr.Size != 4) throw new Exception("x86 CLR required");
            if (args.Length < 3 || (args[0] != "original" && args[0] != "standalone"))
                throw new Exception("Usage: W3xPluginOracle original|standalone animationCapture meshCapture [originalDll]");
            standalone = args[0] == "standalone";
            if (args.Length != (standalone ? 3 : 4)) throw new Exception("Wrong plugin oracle argument count");
            // Verification must not create trace artifacts or depend on the
            // invoking shell's SDK tracing preference.
            Environment.SetEnvironmentVariable("RECOVERED_W3X_TRACE_DIRECTORY",null);
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,OriginalName+".dll")))
                throw new Exception("Plugin host directory must not contain original W3X compiler DLL");
            if (standalone)
            {
                AppDomain.CurrentDomain.AssemblyLoad += Loaded;
                NoOriginal();
            }
            else
            {
                if (!Path.IsPathRooted(args[3])) throw new Exception("Original DLL path must be absolute");
                Assembly assembly = Assembly.LoadFrom(args[3]);
                original = (IAssetBuilderPlugin)Activator.CreateInstance(assembly.GetType("BinaryAssetBuilder.W3XCompiler.Plugin",true));
            }
            string first = Replay(args[1]); string second = Replay(args[2]);
            if (first == second) throw new Exception("Both animation and mesh routes are required");
            if (standalone) NoOriginal();
            Console.WriteLine("PASS W3X native plugin ProcessInstance fixtures={0} threeBufferComparisons={1}; standalone={2}; routes=animation,mesh; SDK not run",fixtures,comparisons,standalone);
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(label); Console.Error.WriteLine(error); return 1; }
    }
}
