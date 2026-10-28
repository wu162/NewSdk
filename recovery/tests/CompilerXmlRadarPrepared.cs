using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder.Core;
using NewModCompiler.Application;

// Linked production-object validation host; no replacement compiler or mocks.
public static class CompilerXmlRadarPrepared
{
    const string NS = "uri:ea.com:eala:asset";
    static readonly Type Contract = typeof(CoreStreamCompiler).Assembly.GetType("NewModCompiler.Application.PreparedBackend", true);
    static int checks;
    static readonly List<object> evidence = new List<object>();
    static void Require(bool condition, string message)
    {
        checks++;
        if (!condition) throw new InvalidOperationException(message);
    }
    static object Prepare(InstanceDeclaration declaration, IAssetBuilderPlugin plugin)
    {
        var parameters = new object[] { declaration, plugin, null };
        bool accepted = (bool)Contract.GetMethod("TryPrepare", new[] { typeof(InstanceDeclaration), typeof(IAssetBuilderPlugin), Contract.MakeByRefType() }).Invoke(null, parameters);
        return accepted ? parameters[2] : null;
    }
    static T Property<T>(object prepared, string name)
    {
        return (T)Contract.GetProperty(name).GetValue(prepared, null);
    }
    static AssetBuffer Execute(object prepared)
    {
        return (AssetBuffer)Contract.GetMethod("Execute").Invoke(prepared, null);
    }
    static byte[] Identity(object prepared)
    {
        using (var bytes = new MemoryStream())
        using (var writer = new BinaryWriter(bytes, Encoding.UTF8))
        {
            Contract.GetMethod("WriteIdentity").Invoke(prepared, new object[] { writer });
            writer.Flush(); return bytes.ToArray();
        }
    }
    static void Equal(byte[] expected, byte[] actual, string label)
    {
        Require(expected != null && actual != null && expected.Length == actual.Length, label + " lengths differ");
        for (int i = 0; i < expected.Length; i++)
            if (expected[i] != actual[i]) throw new InvalidOperationException(label + " raw byte differs at " + i);
        checks++;
    }
    static void EqualBuffers(AssetBuffer expected, AssetBuffer actual, string label)
    {
        Equal(expected.InstanceData, actual.InstanceData, label + "/instance");
        Equal(expected.RelocationData, actual.RelocationData, label + "/relocation");
        Equal(expected.ImportsData, actual.ImportsData, label + "/imports");
    }
    static InstanceDeclaration Declaration(XmlNode source, IAssetBuilderPlugin plugin, out XmlNamespaceManager namespaces)
    {
        var xml = new XmlDocument { XmlResolver = null };
        xml.AppendChild(xml.ImportNode(source, true));
        namespaces = new XmlNamespaceManager(xml.NameTable); namespaces.AddNamespace("ea", NS);
        var declaration = new InstanceDeclaration(AssetDeclarationDocument.CreateBackendView(namespaces));
        declaration.XmlNode = xml.DocumentElement;
        var metadata = plugin.GetExtendedTypeInformation(declaration.Handle.TypeId);
        declaration.Handle.TypeHash = metadata.TypeHash;
        declaration.ProcessingHash = metadata.ProcessingHash;
        declaration.HasCustomData = metadata.HasCustomData;
        return declaration;
    }
    static uint Word(byte[] bytes, int offset, bool big)
    {
        return big ? ((uint)bytes[offset] << 24) | ((uint)bytes[offset + 1] << 16) | ((uint)bytes[offset + 2] << 8) | bytes[offset + 3] : BitConverter.ToUInt32(bytes, offset);
    }
    static void CheckFixture(XmlNode source, TargetPlatform platform)
    {
        var plugin = new Recovered.XmlPriority.Plugin(); plugin.Initialize(null, platform);
        XmlNamespaceManager namespaces;
        var declaration = Declaration(source, plugin, out namespaces);
        // Model the actual resolved resource indices consumed by MarshalReference;
        // the production fixture resolves these names against real Texture/Image
        // declarations in a separate document before reaching this backend.
        bool terrain = source.LocalName == "RadarTerrainImage";
        if (terrain) ((XmlElement)declaration.Node).SetAttribute("Texture", "ParallelRadarTexture\\11");
        else
        {
            int reference = 9;
            foreach (XmlNode child in declaration.Node.ChildNodes)
                if (child.LocalName == "SpecialIcon") { ((XmlElement)child).SetAttribute("Image", "ParallelRadarImage\\" + reference); reference -= 6; }
        }
        AssetBuffer expected = plugin.ProcessInstance(declaration);
        object prepared = Prepare(declaration, plugin);
        Require(prepared != null, "Complete radar family must prepare: " + source.LocalName);
        try
        {
            Require(Property<bool>(prepared, "CanCache") && Property<bool>(prepared, "CanRunParallel"), "Complete radar family must be independently eligible: " + source.LocalName);
            Require(Property<long>(prepared, "ExternalReadBytes") == 0, "Radar metadata must have no leased external reads");
            EqualBuffers(expected, Execute(prepared), source.LocalName + "/" + platform);
            byte[] identity = Identity(prepared);
            InstanceDeclaration frozen = Property<InstanceDeclaration>(prepared, "Declaration");
            Require(!Object.ReferenceEquals(frozen.Node.OwnerDocument, declaration.Node.OwnerDocument), "Radar DOM must be private");
            bool big = platform != TargetPlatform.Win32;
            if (terrain)
            {
                Require(expected.InstanceData.Length == 40 && Word(expected.InstanceData, 8, big) == 12,
                    "Radar terrain must consume the real nonzero resolved Texture reference");
                Require(expected.ImportsData.Length > 0 && expected.RelocationData.Length == 0,
                    "Radar terrain must retain its import-only inline contract");
            }
            else
            {
                Require(Word(expected.InstanceData, 16, big) == 2 && Word(expected.InstanceData, 24, big) == 10 && Word(expected.InstanceData, 148, big) == 4,
                    "Radar icons must consume both nonzero resolved Image references with independent values");
                Require(expected.RelocationData.Length > 0 && expected.ImportsData.Length > 0,
                    "Radar filters/list fixture must exercise all three raw buffers");
            }
            // Mutate consumed owner scalars, all icon refs, filter bitsets and
            // weak-name lists plus namespace bindings after taking the envelope.
            // Detached execution must retain both results and identity bytes.
            ((XmlElement)declaration.Node).SetAttribute(terrain ? "Rotated" : "MinUnitDotSize", terrain ? "false" : "99");
            if (terrain) ((XmlElement)declaration.Node).SetAttribute("Texture", "Changed\\1");
            else foreach (XmlNode child in declaration.Node.ChildNodes)
                if (child.LocalName == "SpecialIcon")
                {
                    ((XmlElement)child).SetAttribute("Image", "Changed\\1");
                    foreach (XmlNode filter in child.ChildNodes)
                        if (filter.LocalName == "ObjectFilter")
                        {
                            ((XmlElement)filter).SetAttribute("Include", "AIRCRAFT");
                            foreach (XmlNode thing in filter.ChildNodes) thing.InnerText = "ChangedWeakThing";
                        }
                }
            namespaces.AddNamespace("ea", "urn:owner-mutated");
            EqualBuffers(expected, Execute(prepared), source.LocalName + "/frozen/" + platform);
            Equal(identity, Identity(prepared), "Prepared radar identity after owner mutation");
            evidence.Add(new { type = source.LocalName, platform = (int)platform, instanceBytes = expected.InstanceData.Length,
                relocationBytes = expected.RelocationData.Length, importsBytes = expected.ImportsData.Length, frozenOwnerMutation = true });
        }
        finally { ((IDisposable)prepared).Dispose(); }
    }
    public static int Main(string[] args)
    {
        try
        {
            string path = Path.GetFullPath(Path.Combine(args[4], "../recovery/tests/XmlradarProbe.xml"));
            var probe = new XmlDocument { XmlResolver = null }; probe.Load(path);
            foreach (TargetPlatform platform in new[] { TargetPlatform.Win32, TargetPlatform.Xbox360, TargetPlatform.PlayStation3, (TargetPlatform)99 })
            {
                int publicRoots = 0;
                foreach (XmlNode source in probe.DocumentElement.ChildNodes)
                {
                    if (source.NodeType != XmlNodeType.Element) continue;
                    CheckFixture(source, platform); publicRoots++;
                }
                Require(publicRoots == 2, "Real SDK radar fixture must contain exactly its two global roots");
            }
            string sha;
            using (var hash = SHA256.Create()) sha = BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant();
            File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(new {
                succeeded = true, sourceCommit = args[2], runId = args[3], checks, evidence, fixtureSha256 = sha,
                scope = "Complete Xmlradar two-root prepared certification with real resolved Texture/Image references, nested ObjectFilter weak-name/bitset/list payloads, detached owner mutations, four platforms including 99 and all three raw buffers; actual production reference resolution and worker intervals are checked separately"
            }));
            Console.WriteLine("PASS XmlRadarPrepared: " + checks + " assertions"); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
