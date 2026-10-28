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
public static class CompilerXmlUnitOverlayPrepared
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
        // Real resolved Image/ImageSequence spellings; every typed root slot
        // receives its own nonzero index, including nested faction/rank imports.
        string[] references = { "AmmoPipImage", "AmmoPipFrameImage", "ContainPipImage", "ContainPipFrameImage", "GarrisonPipImage",
            "UnitTypeIconBackgroundImage", "TargetIndicatorImage", "TargetIndicatorGlowImage", "PowerIconImageSequence",
            "RepairIconImageSequence", "InfiltratedIconImageSequence", "HealUnitIconImageSequence", "RepairVehicleIconImageSequence" };
        for (int i = 0; i < references.Length; i++) ((XmlElement)declaration.Node).SetAttribute(references[i], "ResolvedRoot\\" + (9 + i));
        int[] ranks = { 7, 3, 2 }; int rank = 0;
        foreach (XmlNode faction in declaration.Node.ChildNodes)
            if (faction.LocalName == "FactionVeterancy")
                foreach (XmlNode icon in faction.ChildNodes)
                    if (icon.LocalName == "RankIcon") ((XmlElement)icon).SetAttribute("Image", "ResolvedRank\\" + ranks[rank++]);
        Require(rank == 3, "Unit overlay fixture must contain all three typed rank references");
        AssetBuffer expected = plugin.ProcessInstance(declaration);
        object prepared = Prepare(declaration, plugin);
        Require(prepared != null, "Complete unit overlay family must prepare: " + source.LocalName);
        try
        {
            Require(Property<bool>(prepared, "CanCache") && Property<bool>(prepared, "CanRunParallel"), "Complete unit overlay family must be independently eligible: " + source.LocalName);
            Require(Property<long>(prepared, "ExternalReadBytes") == 0, "Unit overlay metadata must have no leased external reads");
            EqualBuffers(expected, Execute(prepared), source.LocalName + "/" + platform);
            byte[] identity = Identity(prepared);
            InstanceDeclaration frozen = Property<InstanceDeclaration>(prepared, "Declaration");
            Require(!Object.ReferenceEquals(frozen.Node.OwnerDocument, declaration.Node.OwnerDocument), "Unit overlay DOM must be private");
            // Mutate the owner after preparation, including namespaces and a
            // consumed scalar/list. An invalid owner attribute also proves the
            // backend does not accidentally walk the original tree.
            ((XmlElement)declaration.Node).SetAttribute("OwnerOnlyMutation", "true");
            foreach (string reference in references) ((XmlElement)declaration.Node).SetAttribute(reference, "Changed\\1");
            ((XmlElement)declaration.Node).SetAttribute("AmmoPipRange", "99");
            ((XmlElement)declaration.Node).SetAttribute("AmmoPipScale", "99%");
            ((XmlElement)declaration.Node).SetAttribute("HealingIconDisplayTime", "99s");
            foreach (XmlNode faction in declaration.Node.ChildNodes)
                if (faction.LocalName == "FactionVeterancy")
                {
                    ((XmlElement)faction).SetAttribute("Faction", "Neutral");
                    foreach (XmlNode icon in faction.ChildNodes)
                        if (icon.LocalName == "RankIcon")
                        {
                            ((XmlElement)icon).SetAttribute("Rank", "99");
                            ((XmlElement)icon).SetAttribute("Image", "Changed\\1");
                        }
                }
            namespaces.AddNamespace("ea", "urn:owner-mutated");
            EqualBuffers(expected, Execute(prepared), source.LocalName + "/frozen/" + platform);
            Equal(identity, Identity(prepared), "Prepared unit overlay identity after owner mutation");
            bool big = platform != TargetPlatform.Win32;
            Require(expected.InstanceData.Length == 152 && expected.RelocationData.Length > 0 && expected.ImportsData.Length > 0,
                "Complete unit overlay fixture must retain its root104/faction12/rank8 layout and all three buffers");
            for (int i = 0; i < references.Length; i++)
                Require(Word(expected.InstanceData, 4 + 4 * i, big) == (uint)(10 + i), "Typed root Image/ImageSequence import slot differs: " + references[i]);
            Require(Word(expected.InstanceData, 56, big) == 0x3f000000u && Word(expected.InstanceData, 60, big) == unchecked((uint)-17),
                "Unit overlay must preserve percentage50 and signed range fields");
            Require(Word(expected.InstanceData, 92, big) == 0x3fc00001u,
                "Unit overlay 1500ms must retain original binary32 unit-scale rounding");
            Require(Word(expected.InstanceData, 96, big) == 2 && Word(expected.InstanceData, 100, big) == 104,
                "Unit overlay must preserve faction count and pointer");
            Require(Word(expected.InstanceData, 104, big) == 1 && Word(expected.InstanceData, 108, big) == 2 && Word(expected.InstanceData, 112, big) == 128,
                "Soviet faction enum/count/rank pointer differs");
            Require(Word(expected.InstanceData, 116, big) == 2 && Word(expected.InstanceData, 120, big) == 1 && Word(expected.InstanceData, 124, big) == 144,
                "Japan faction enum/count/rank pointer differs");
            Require(Word(expected.InstanceData, 128, big) == 1 && Word(expected.InstanceData, 136, big) == uint.MaxValue && Word(expected.InstanceData, 144, big) == 4,
                "Nested ranks must retain signed int values and eight-byte stride");
            Require(Word(expected.InstanceData, 132, big) == 8 && Word(expected.InstanceData, 140, big) == 4 && Word(expected.InstanceData, 148, big) == 3,
                "Nested rank Image imports must retain independent resolved indices");
            evidence.Add(new { type = source.LocalName, platform = (int)platform, instanceBytes = expected.InstanceData.Length,
                relocationBytes = expected.RelocationData.Length, importsBytes = expected.ImportsData.Length, frozenOwnerMutation = true });
        }
        finally { ((IDisposable)prepared).Dispose(); }
    }
    public static int Main(string[] args)
    {
        try
        {
            string path = Path.GetFullPath(Path.Combine(args[4], "../recovery/tests/XmlunitoverlayProbe.xml"));
            var probe = new XmlDocument { XmlResolver = null }; probe.Load(path);
            foreach (TargetPlatform platform in new[] { TargetPlatform.Win32, TargetPlatform.Xbox360, TargetPlatform.PlayStation3, (TargetPlatform)99 })
            {
                int publicRoots = 0;
                foreach (XmlNode source in probe.DocumentElement.ChildNodes)
                {
                    if (source.NodeType != XmlNodeType.Element) continue;
                    CheckFixture(source, platform); publicRoots++;
                }
                Require(publicRoots == 1, "Real SDK unit overlay fixture must contain exactly its one global root");
            }
            string sha;
            using (var hash = SHA256.Create()) sha = BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant();
            File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(new {
                succeeded = true, sourceCommit = args[2], runId = args[3], checks, evidence, fixtureSha256 = sha,
                scope = "Complete Xmlunitoverlay one-root prepared certification with thirteen typed root Image/ImageSequence imports, nested faction/rank arrays and imports, percentage/time/signed-value ABI, detached owner mutations and four-platform including 99 three-buffer equality; actual production reference resolution and worker intervals are checked separately"
            }));
            Console.WriteLine("PASS XmlUnitOverlayPrepared: " + checks + " assertions"); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
