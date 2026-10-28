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
public static class CompilerXmlUiImagesPrepared
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
        // Model the actual post-resolution indices consumed by the bridges.
        if (source.LocalName == "UnitTypeIcon") ((XmlElement)declaration.Node).SetAttribute("Image", "ParallelRadarImage\\6");
        if (source.LocalName == "ImageSequence")
        {
            int[] indices = { 9, 3, 7 }; int index = 0;
            foreach (XmlNode child in declaration.Node.ChildNodes)
                if (child.LocalName == "Image") child.InnerText = "ParallelRadarImage\\" + indices[index++];
        }
        AssetBuffer expected = plugin.ProcessInstance(declaration);
        object prepared = Prepare(declaration, plugin);
        Require(prepared != null, "Complete UI images family must prepare: " + source.LocalName);
        try
        {
            Require(Property<bool>(prepared, "CanCache") && Property<bool>(prepared, "CanRunParallel"), "Complete UI images family must be independently eligible: " + source.LocalName);
            Require(Property<long>(prepared, "ExternalReadBytes") == 0, "UI image metadata must have no leased external reads");
            EqualBuffers(expected, Execute(prepared), source.LocalName + "/" + platform);
            byte[] identity = Identity(prepared);
            InstanceDeclaration frozen = Property<InstanceDeclaration>(prepared, "Declaration");
            Require(!Object.ReferenceEquals(frozen.Node.OwnerDocument, declaration.Node.OwnerDocument), "UI images DOM must be private");
            // Mutate the owner after preparation, including namespaces and a
            // consumed scalar/list. An invalid owner attribute also proves the
            // backend does not accidentally walk the original tree.
            ((XmlElement)declaration.Node).SetAttribute("OwnerOnlyMutation", "true");
            if (source.LocalName == "UnitTypeIcon")
            {
                ((XmlElement)declaration.Node).SetAttribute("Image", "Changed\\1");
                ((XmlElement)declaration.Node.FirstChild).SetAttribute("x", "99");
            }
            else if (source.LocalName == "ImageSequence")
            {
                foreach (XmlNode child in declaration.Node.ChildNodes) if (child.LocalName == "Image") child.InnerText = "Changed\\1";
            }
            else
            {
                ((XmlElement)declaration.Node).SetAttribute("FontName", "ChangedFont");
                ((XmlElement)declaration.Node).SetAttribute("FontSize", "99");
                ((XmlElement)declaration.Node.FirstChild).SetAttribute("R", "99");
            }
            namespaces.AddNamespace("ea", "urn:owner-mutated");
            EqualBuffers(expected, Execute(prepared), source.LocalName + "/frozen/" + platform);
            Equal(identity, Identity(prepared), "Prepared UI images identity after owner mutation");
            bool big = platform != TargetPlatform.Win32;
            Require(expected.RelocationData.Length > 0, "Every UI images fixture must exercise its real list/vector/string relocation");
            if (source.LocalName == "ImageSequence")
            {
                Require(Word(expected.InstanceData, 4, big) == 3 && Word(expected.InstanceData, 8, big) == 12,
                    "ImageSequence must preserve the real list count and pointer");
                Require(Word(expected.InstanceData, 12, big) == 10 && Word(expected.InstanceData, 16, big) == 4 && Word(expected.InstanceData, 20, big) == 8,
                    "ImageSequence must consume independent resolved Image import indices");
                Require(expected.ImportsData.Length > 0, "ImageSequence must retain its imports");
            }
            else if (source.LocalName == "UnitTypeIcon")
            {
                Require(Word(expected.InstanceData, 4, big) == 7 && Word(expected.InstanceData, 8, big) == 12 &&
                    Word(expected.InstanceData, 12, big) == 0x3f000000u && Word(expected.InstanceData, 16, big) == 0xc0000000u,
                    "UnitTypeIcon must retain its resolved Image and pointed vector2");
                Require(expected.ImportsData.Length > 0, "UnitTypeIcon must retain its import");
            }
            else
            {
                Require(Word(expected.InstanceData, 4, big) == 13 && Word(expected.InstanceData, 8, big) == 52 && Word(expected.InstanceData, 12, big) == 17,
                    "DrawGroupInfo must serialize the complete font string and independent font-size field");
                Require(Word(expected.InstanceData, 40, big) == 0x44112233u && expected.InstanceData[48] == 1 && expected.InstanceData[49] == 0 && expected.InstanceData[50] == 1,
                    "DrawGroupInfo must preserve packed color and one-byte booleans on every platform");
                Require(expected.ImportsData.Length == 0, "DrawGroupInfo font text is serialized data rather than a strong import or external file");
            }
            evidence.Add(new { type = source.LocalName, platform = (int)platform, instanceBytes = expected.InstanceData.Length,
                relocationBytes = expected.RelocationData.Length, importsBytes = expected.ImportsData.Length, frozenOwnerMutation = true });
        }
        finally { ((IDisposable)prepared).Dispose(); }
    }
    public static int Main(string[] args)
    {
        try
        {
            string path = Path.GetFullPath(Path.Combine(args[4], "../recovery/tests/XmluiimagesProbe.xml"));
            var probe = new XmlDocument { XmlResolver = null }; probe.Load(path);
            foreach (TargetPlatform platform in new[] { TargetPlatform.Win32, TargetPlatform.Xbox360, TargetPlatform.PlayStation3, (TargetPlatform)99 })
            {
                int publicRoots = 0;
                foreach (XmlNode source in probe.DocumentElement.ChildNodes)
                {
                    if (source.NodeType != XmlNodeType.Element) continue;
                    CheckFixture(source, platform); publicRoots++;
                }
                Require(publicRoots == 3, "Real SDK UI images fixture must contain exactly its three global roots");
            }
            string sha;
            using (var hash = SHA256.Create()) sha = BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant();
            File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(new {
                succeeded = true, sourceCommit = args[2], runId = args[3], checks, evidence, fixtureSha256 = sha,
                scope = "Complete Xmluiimages three-root prepared certification with actual resolved Image import indices, list/vector/string relocations, packed color and boolean ABI, detached owner mutations, four platforms including 99 and all three raw buffers; actual production reference resolution and worker intervals are checked separately"
            }));
            Console.WriteLine("PASS XmlUiImagesPrepared: " + checks + " assertions"); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
