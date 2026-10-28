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
public static class CompilerXmlRenderDevicePrepared
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
    static void MutateOwner(XmlNode node)
    {
        if (node.Attributes != null)
        {
            XmlAttribute include = node.Attributes["Include"], terrain = node.Attributes["TerrainClass"];
            if (include != null) include.Value = "AIRCRAFT";
            if (terrain != null) terrain.Value = "Grass";
        }
        foreach (XmlNode child in node.ChildNodes)
        {
            if (child.LocalName == "StandardPublicBone" || child.LocalName == "IncludeThing" || child.LocalName == "ExcludeThing") child.InnerText = "ChangedWeakName";
            else if (child.LocalName == "Scorch") child.InnerText = "SCORCH_2";
            else MutateOwner(child);
        }
    }
    static void CheckFixture(XmlNode source, TargetPlatform platform)
    {
        var plugin = new Recovered.XmlPriority.Plugin(); plugin.Initialize(null, platform);
        XmlNamespaceManager namespaces;
        var declaration = Declaration(source, plugin, out namespaces);
        AssetBuffer expected = plugin.ProcessInstance(declaration);
        object prepared = Prepare(declaration, plugin);
        Require(prepared != null, "Complete render-device family must prepare: " + source.LocalName);
        try
        {
            Require(Property<bool>(prepared, "CanCache") && Property<bool>(prepared, "CanRunParallel"), "Complete render-device family must be independently eligible: " + source.LocalName);
            Require(Property<long>(prepared, "ExternalReadBytes") == 0, "Render-device metadata must have no leased external reads");
            EqualBuffers(expected, Execute(prepared), source.LocalName + "/" + platform);
            byte[] identity = Identity(prepared);
            InstanceDeclaration frozen = Property<InstanceDeclaration>(prepared, "Declaration");
            Require(!Object.ReferenceEquals(frozen.Node.OwnerDocument, declaration.Node.OwnerDocument), "Render-device DOM must be private");
            Require(expected.RelocationData.Length > 0 && expected.ImportsData.Length == 0,
                "Render-device nested lists/filter/string payloads must use relocations without inventing strong imports");
            bool big = platform != TargetPlatform.Win32;
            if (source.LocalName == "W3DDeviceSettings")
            {
                Require(Word(expected.InstanceData, 4, big) == 117 && Word(expected.InstanceData, 28, big) == 3 && Word(expected.InstanceData, 156, big) == 2,
                    "Device tracks, shroud enum and bone count must retain their nondefault values");
                Require(expected.InstanceData[164] == 1 && expected.InstanceData[165] == 77 && expected.InstanceData[166] == 12 && expected.InstanceData[167] == 201,
                    "Device boolean/alpha byte fields must not endian-swap on non-Win32 platforms");
            }
            else
            {
                Require(Word(expected.InstanceData, 4, big) == 2, "Both real nested scorch rows must be materialized");
                int rows = checked((int)Word(expected.InstanceData, 8, big));
                int first = checked((int)Word(expected.InstanceData, rows + 8, big));
                int second = checked((int)Word(expected.InstanceData, rows + 20, big));
                Require(Word(expected.InstanceData, rows, big) == 8 && Word(expected.InstanceData, rows + 12, big) == 10 &&
                    Word(expected.InstanceData, first, big) == 0 && Word(expected.InstanceData, first + 4, big) == 15 &&
                    Word(expected.InstanceData, second, big) == UInt32.MaxValue && Word(expected.InstanceData, second + 4, big) == 16,
                    "Terrain and scorch enum lists must preserve distinct values and signed RANDOM on every platform");
            }
            // Mutate owner fields that the backend actually consumes, including
            // lists of strings/enums and the inline ObjectFilter subtree.
            ((XmlElement)declaration.Node).SetAttribute("OwnerOnlyMutation", "true");
            if (source.LocalName == "W3DDeviceSettings") ((XmlElement)declaration.Node).SetAttribute("MaxTerrainTracks", "999");
            MutateOwner(declaration.Node);
            namespaces.AddNamespace("ea", "urn:owner-mutated");
            EqualBuffers(expected, Execute(prepared), source.LocalName + "/frozen/" + platform);
            Equal(identity, Identity(prepared), "Prepared render-device identity after owner mutation");
            evidence.Add(new { type = source.LocalName, platform = (int)platform, instanceBytes = expected.InstanceData.Length,
                relocationBytes = expected.RelocationData.Length, importsBytes = expected.ImportsData.Length, frozenOwnerMutation = true });
        }
        finally { ((IDisposable)prepared).Dispose(); }
    }
    public static int Main(string[] args)
    {
        try
        {
            string path = Path.GetFullPath(Path.Combine(args[4], "../recovery/tests/XmlrenderdeviceProbe.xml"));
            var probe = new XmlDocument { XmlResolver = null }; probe.Load(path);
            foreach (TargetPlatform platform in new[] { TargetPlatform.Win32, TargetPlatform.Xbox360, TargetPlatform.PlayStation3, (TargetPlatform)99 })
            {
                int publicRoots = 0;
                foreach (XmlNode source in probe.DocumentElement.ChildNodes)
                {
                    if (source.NodeType != XmlNodeType.Element) continue;
                    CheckFixture(source, platform); publicRoots++;
                }
                Require(publicRoots == 2, "Real SDK render-device fixture must contain exactly both global roots");
            }
            string sha;
            using (var hash = SHA256.Create()) sha = BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant();
            File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(new {
                succeeded = true, sourceCommit = args[2], runId = args[3], checks, evidence, fixtureSha256 = sha,
                scope = "Complete Xmlrenderdevice two-root prepared certification with scalar/byte/color, inline ObjectFilter weak-name/status payloads, bone strings and nested scorch enums, detached owner mutations, four platforms including 99 and all three raw buffers; actual production worker intervals are checked separately"
            }));
            Console.WriteLine("PASS XmlRenderDevicePrepared: " + checks + " assertions"); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
