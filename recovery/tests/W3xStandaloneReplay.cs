using System;
using System.IO;
using System.Reflection;
using System.Xml;
using BinaryAssetBuilder.Core;
using Recovered.W3x;

// Replays original component captures using only the recovered bridges and
// Core. No original W3X compiler assembly or SDK executable is permitted.
internal static class W3xStandaloneReplay
{
    const string OriginalName = "BinaryAssetBuilder.W3XCompiler";
    const string AssetNamespace = "uri:ea.com:eala:asset";
    static long fixtures, comparisons, comparedBytes;
    static string label = "startup";

    static bool IsOriginal(Assembly assembly)
    {
        return String.Equals(assembly.GetName().Name, OriginalName,
            StringComparison.OrdinalIgnoreCase);
    }

    static void CheckLoadedAssemblies()
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            if (IsOriginal(assembly))
                throw new InvalidOperationException("Original W3X compiler assembly was loaded: " + assembly.FullName);
    }

    static void AssemblyLoaded(object sender, AssemblyLoadEventArgs args)
    {
        if (IsOriginal(args.LoadedAssembly))
            throw new InvalidOperationException("Original W3X compiler assembly load is forbidden during standalone replay.");
    }

    static byte[] Buffer(BinaryReader reader, string name)
    {
        int length = reader.ReadInt32();
        if (length < 0 || length > reader.BaseStream.Length - reader.BaseStream.Position)
            throw new InvalidDataException(label + " invalid or truncated " + name + " length " + length);
        byte[] bytes = reader.ReadBytes(length);
        if (bytes.Length != length)
            throw new EndOfStreamException(label + " truncated " + name);
        return bytes;
    }

    static void Equal(string name, byte[] expected, byte[] actual)
    {
        if (actual == null || expected.Length != actual.Length)
            throw new InvalidOperationException(label + " " + name + " length expected=" + expected.Length +
                " actual=" + (actual == null ? -1 : actual.Length));
        for (int i = 0; i < expected.Length; ++i)
            if (expected[i] != actual[i])
                throw new InvalidOperationException(label + " " + name + " byte=" + i +
                    " expected=" + expected[i] + " actual=" + actual[i]);
        ++comparisons;
        comparedBytes += expected.Length;
    }

    static string Replay(string path)
    {
        if (!Path.IsPathRooted(path))
            throw new ArgumentException("Capture paths must be absolute: " + path);
        using (FileStream stream = File.OpenRead(path))
        using (BinaryReader reader = new BinaryReader(stream))
        {
            if (reader.ReadString() != "W3X-FIXTURE-1")
                throw new InvalidDataException("Unsupported W3X capture format: " + path);
            string route = reader.ReadString();
            if (route != "animation" && route != "mesh")
                throw new InvalidDataException("Unsupported W3X capture route: " + route);
            long routeFixtures = 0;
            while (stream.Position < stream.Length)
            {
                label = route + " fixture=" + routeFixtures + " file=" + path;
                int platform = reader.ReadInt32();
                label += " platform=" + platform;
                string xmlText = reader.ReadString();
                byte[] instance = Buffer(reader, "instance");
                byte[] relocations = Buffer(reader, "relocations");
                byte[] imports = Buffer(reader, "imports");
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(xmlText);
                if (xml.DocumentElement == null)
                    throw new InvalidDataException(label + " has no XML root");
                string rootName = route == "animation" ? "W3DAnimation" : "W3DMesh";
                if (xml.DocumentElement.LocalName != rootName || xml.DocumentElement.NamespaceURI != AssetNamespace)
                    throw new InvalidDataException(label + " XML root does not match its capture route");
                XmlNamespaceManager namespaces = new XmlNamespaceManager(xml.NameTable);
                namespaces.AddNamespace("ea", AssetNamespace);
                AssetBuffer actual = route == "animation"
                    ? AnimationCompiler.CompileXml(xml.DocumentElement, namespaces, (TargetPlatform)platform)
                    : MeshCompiler.CompileXml(xml.DocumentElement, namespaces, (TargetPlatform)platform);
                if (actual == null)
                    throw new InvalidOperationException(label + " returned null AssetBuffer");
                Equal("instance", instance, actual.InstanceData);
                Equal("relocations", relocations, actual.RelocationData);
                Equal("imports", imports, actual.ImportsData);
                ++routeFixtures;
                ++fixtures;
            }
            // This acceptance gate covers the complete current oracle sets.
            // Update these counts deliberately when either source set changes;
            // a capture ending cleanly between records must not silently pass.
            long expectedFixtures = route == "animation" ? 675 : 666;
            if (routeFixtures != expectedFixtures)
                throw new InvalidDataException(route + " capture fixture count expected=" + expectedFixtures +
                    " actual=" + routeFixtures + " file=" + path);
            Console.WriteLine("PASS W3X standalone route={0} fixtures={1}", route, routeFixtures);
            return route;
        }
    }

    static int Main(string[] args)
    {
        try
        {
            if (IntPtr.Size != 4)
                throw new InvalidOperationException("Standalone W3X replay requires an x86 CLR host.");
            if (args.Length != 2)
                throw new ArgumentException("Usage: W3xStandaloneReplay animation-capture-absolute-path mesh-capture-absolute-path");
            AppDomain.CurrentDomain.AssemblyLoad += AssemblyLoaded;
            CheckLoadedAssemblies();
            string originalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, OriginalName + ".dll");
            if (File.Exists(originalPath))
                throw new InvalidOperationException("Original W3X compiler DLL must be absent from the replay program directory: " + originalPath);
            string firstRoute = Replay(args[0]);
            string secondRoute = Replay(args[1]);
            if (firstRoute == secondRoute)
                throw new InvalidDataException("Duplicate capture route; both animation and mesh are required.");
            CheckLoadedAssemblies();
            Console.WriteLine("PASS W3X standalone replay fixtures={0} threeBufferComparisons={1} comparedBytes={2}; routes=animation,mesh; originalW3XLoaded=false; SDK not run",
                fixtures, comparisons, comparedBytes);
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(label);
            Console.Error.WriteLine(error);
            return 1;
        }
    }
}
