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
public static class CompilerXmlHealthBarsPrepared
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
        // Exercise actual nonzero resolved HealthBarTemplate indices in both
        // root and list import locations. The production fixture closes every
        // name against its real HealthBarTemplate declaration.
        bool template = source.LocalName == "HealthBarTemplate";
        if (!template)
        {
            ((XmlElement)declaration.Node).SetAttribute("DefaultHealthBar", "ParallelHealthTemplate\\9");
            int reference = 7;
            foreach (XmlNode child in declaration.Node.ChildNodes)
                if (child.LocalName == "ObjectMapping") { ((XmlElement)child).SetAttribute("HealthBar", "ParallelHealthTemplate\\" + reference); reference -= 4; }
        }
        AssetBuffer expected = plugin.ProcessInstance(declaration);
        object prepared = Prepare(declaration, plugin);
        Require(prepared != null, "Complete health-bars family must prepare: " + source.LocalName);
        try
        {
            Require(Property<bool>(prepared, "CanCache") && Property<bool>(prepared, "CanRunParallel"), "Complete health-bars family must be independently eligible: " + source.LocalName);
            Require(Property<long>(prepared, "ExternalReadBytes") == 0, "Health-bars metadata must have no leased external reads");
            EqualBuffers(expected, Execute(prepared), source.LocalName + "/" + platform);
            byte[] identity = Identity(prepared);
            InstanceDeclaration frozen = Property<InstanceDeclaration>(prepared, "Declaration");
            Require(!Object.ReferenceEquals(frozen.Node.OwnerDocument, declaration.Node.OwnerDocument), "Health-bars DOM must be private");
            bool big = platform != TargetPlatform.Win32;
            if (template)
            {
                Require(expected.InstanceData.Length == 8 && Word(expected.InstanceData, 4, big) == 117 &&
                    expected.RelocationData.Length == 0 && expected.ImportsData.Length == 0,
                    "Health template must retain its exact inline unsigned Length without pointer/import buffers");
            }
            else
            {
                Require(Word(expected.InstanceData, 4, big) == 10 && Word(expected.InstanceData, 8, big) == 2 &&
                    Word(expected.InstanceData, 12, big) == 16 && Word(expected.InstanceData, 16, big) == 8 && Word(expected.InstanceData, 132, big) == 4,
                    "Health settings must consume the nonzero root and two mapping references at exact 116-byte strides");
                Require(expected.RelocationData.Length > 0 && expected.ImportsData.Length > 0,
                    "Health mapping/filter fixture must exercise all three complete raw buffers");
            }
            // Mutate consumed owner fields and nested filters/weak-name lists
            // after taking the envelope. No worker may consult that owner DOM.
            ((XmlElement)declaration.Node).SetAttribute("OwnerOnlyMutation", "true");
            if (template) ((XmlElement)declaration.Node).SetAttribute("Length", "999");
            else
            {
                ((XmlElement)declaration.Node).SetAttribute("DefaultHealthBar", "Changed\\1");
                foreach (XmlNode child in declaration.Node.ChildNodes)
                    if (child.LocalName == "ObjectMapping")
                    {
                        ((XmlElement)child).SetAttribute("HealthBar", "Changed\\1");
                        foreach (XmlNode filter in child.ChildNodes)
                            if (filter.LocalName == "ObjectFilter")
                            {
                                ((XmlElement)filter).SetAttribute("Include", "AIRCRAFT");
                                foreach (XmlNode thing in filter.ChildNodes) thing.InnerText = "ChangedWeakThing";
                            }
                    }
            }
            namespaces.AddNamespace("ea", "urn:owner-mutated");
            EqualBuffers(expected, Execute(prepared), source.LocalName + "/frozen/" + platform);
            Equal(identity, Identity(prepared), "Prepared health-bars identity after owner mutation");
            evidence.Add(new { type = source.LocalName, platform = (int)platform, instanceBytes = expected.InstanceData.Length,
                relocationBytes = expected.RelocationData.Length, importsBytes = expected.ImportsData.Length, frozenOwnerMutation = true });
        }
        finally { ((IDisposable)prepared).Dispose(); }
    }
    public static int Main(string[] args)
    {
        try
        {
            string path = Path.GetFullPath(Path.Combine(args[4], "../recovery/tests/XmlhealthbarsProbe.xml"));
            var probe = new XmlDocument { XmlResolver = null }; probe.Load(path);
            foreach (TargetPlatform platform in new[] { TargetPlatform.Win32, TargetPlatform.Xbox360, TargetPlatform.PlayStation3, (TargetPlatform)99 })
            {
                int publicRoots = 0;
                foreach (XmlNode source in probe.DocumentElement.ChildNodes)
                {
                    if (source.NodeType != XmlNodeType.Element) continue;
                    CheckFixture(source, platform); publicRoots++;
                }
                Require(publicRoots == 2, "Real SDK health-bars fixture must contain exactly its two global roots");
            }
            string sha;
            using (var hash = SHA256.Create()) sha = BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant();
            File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(new {
                succeeded = true, sourceCommit = args[2], runId = args[3], checks, evidence, fixtureSha256 = sha,
                scope = "Complete Xmlhealthbars two-root prepared certification with resolved HealthBarTemplate root/list references, exact 8/16/116 layouts and nested ObjectFilter weak-name/bitset/list payloads, detached owner mutations, four platforms including 99 and all three raw buffers; actual production reference resolution and worker intervals are checked separately"
            }));
            Console.WriteLine("PASS XmlHealthBarsPrepared: " + checks + " assertions"); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
