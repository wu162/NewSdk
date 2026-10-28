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
public static class CompilerXmlObjectSetsPrepared
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
        XmlNode filter = declaration.Node.FirstChild;
        Require(filter != null && (filter.LocalName == "Filter" || filter.LocalName == "ObjectFilter"), "Objectsets source fixture must have its real inline filter");
        AssetBuffer expected = plugin.ProcessInstance(declaration);
        object prepared = Prepare(declaration, plugin);
        Require(prepared != null, "Complete objectsets family must prepare: " + source.LocalName);
        try
        {
            Require(Property<bool>(prepared, "CanCache") && Property<bool>(prepared, "CanRunParallel"), "Complete objectsets family must be independently eligible: " + source.LocalName);
            Require(Property<long>(prepared, "ExternalReadBytes") == 0, "Objectsets metadata must have no leased external reads");
            EqualBuffers(expected, Execute(prepared), source.LocalName + "/" + platform);
            byte[] identity = Identity(prepared);
            InstanceDeclaration frozen = Property<InstanceDeclaration>(prepared, "Declaration");
            Require(!Object.ReferenceEquals(frozen.Node.OwnerDocument, declaration.Node.OwnerDocument), "Objectsets DOM must be private");
            // Mutate the owner after preparation, including namespaces and a
            // consumed scalar/list. An invalid owner attribute also proves the
            // backend does not accidentally walk the original tree.
            ((XmlElement)declaration.Node).SetAttribute("OwnerOnlyMutation", "true");
            if (source.LocalName == "RegisteredObjectSet") ((XmlElement)declaration.Node).SetAttribute("Type", "GLOBAL");
            if (source.LocalName == "ObjectVisionRangeItem") ((XmlElement)declaration.Node).SetAttribute("Range", "99%");
            ((XmlElement)filter).SetAttribute("Include", "DOZER");
            ((XmlElement)filter).SetAttribute("StatusBitFlags", "UNDER_CONSTRUCTION");
            filter.FirstChild.InnerText = "ChangedWeakObject";
            namespaces.AddNamespace("ea", "urn:owner-mutated");
            EqualBuffers(expected, Execute(prepared), source.LocalName + "/frozen/" + platform);
            Equal(identity, Identity(prepared), "Prepared objectsets identity after owner mutation");
            Require(expected.RelocationData.Length > 0 && expected.ImportsData.Length == 0,
                "Objectsets uses inline filters, actual suballocation relocations and weak hashes, never strong imports");
            bool big = platform != TargetPlatform.Win32;
            int offset = source.LocalName == "RegisteredObjectSet" ? 12 : source.LocalName == "ObjectVisionRangeItem" ? 8 : 4;
            if (source.LocalName == "RegisteredObjectSet")
                Require(Word(expected.InstanceData, 4, big) == 26 && Word(expected.InstanceData, 8, big) == 1,
                    "Registered AIRCRAFT/PLAYER enum values must remain in their real root fields");
            if (source.LocalName == "ObjectVisionRangeItem")
                Require(Word(expected.InstanceData, 4, big) == 0x3e800000u,
                    "Nested ObjectVisionRangeItem 25% must retain binary32 0.25 at root offset4");
            Require(Word(expected.InstanceData, offset + 96, big) == 2 && Word(expected.InstanceData, offset + 104, big) == 1,
                "Objectsets fixture must exercise two weak includes and one exclusion");
            int includeOffset = checked((int)Word(expected.InstanceData, offset + 100, big));
            Require(Word(expected.InstanceData, includeOffset, big) == Word(expected.InstanceData, includeOffset + 4, big),
                "Weak ObjectFilter names must preserve pure case-folded hash behavior");
            evidence.Add(new { type = source.LocalName, platform = (int)platform, instanceBytes = expected.InstanceData.Length,
                relocationBytes = expected.RelocationData.Length, importsBytes = expected.ImportsData.Length, frozenOwnerMutation = true });
        }
        finally { ((IDisposable)prepared).Dispose(); }
    }
    public static int Main(string[] args)
    {
        try
        {
            string path = Path.GetFullPath(Path.Combine(args[4], "../recovery/tests/XmlobjectsetsProbe.xml"));
            var probe = new XmlDocument { XmlResolver = null }; probe.Load(path);
            var nested = new XmlDocument { XmlResolver = null };
            nested.LoadXml("<ObjectVisionRangeItem xmlns='" + NS + "' id='ComponentOnlyObjectVision' Range='25%'>" +
                "<Filter id='NestedVisionFilter' Rule='ANY' Relationship='ALLIES' Alignment='GOOD' Include='INFANTRY' Exclude='STRUCTURE' " +
                "StatusBitFlags='IS_ATTACKING' StatusBitFlagsExclude='STEALTHED'>" +
                "<IncludeThing>NestedWeakOne</IncludeThing><IncludeThing>NESTEDWEAKONE</IncludeThing><ExcludeThing>NestedWeakExcluded</ExcludeThing>" +
                "</Filter></ObjectVisionRangeItem>");
            foreach (TargetPlatform platform in new[] { TargetPlatform.Win32, TargetPlatform.Xbox360, TargetPlatform.PlayStation3, (TargetPlatform)99 })
            {
                CheckFixture(nested.DocumentElement, platform);
                int publicRoots = 0;
                foreach (XmlNode source in probe.DocumentElement.ChildNodes)
                {
                    if (source.NodeType != XmlNodeType.Element) continue;
                    CheckFixture(source, platform); publicRoots++;
                }
                Require(publicRoots == 2, "Real SDK objectsets fixture must contain exactly its two global roots");
            }
            string sha;
            using (var hash = SHA256.Create()) sha = BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant();
            File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(new {
                succeeded = true, sourceCommit = args[2], runId = args[3], checks, evidence, fixtureSha256 = sha,
                scope = "Complete Xmlobjectsets three-type prepared certification and detached owner-mutation protection, four platforms including 99, all three raw buffers, inline ObjectFilter/weak lists and RegisteredObjectSet/percentage ABI; ObjectVisionRangeItem is nested-only in the SDK; actual production worker intervals are checked separately"
            }));
            Console.WriteLine("PASS XmlObjectSetsPrepared: " + checks + " assertions"); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
