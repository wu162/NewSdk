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
public static class CompilerXmlConditionsPrepared
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
    static XmlNode Component(XmlNode source, string type, bool allChildren)
    {
        var xml = new XmlDocument { XmlResolver = null };
        XmlElement root = xml.CreateElement(type, NS); xml.AppendChild(root);
        root.SetAttribute("id", "ComponentOnly" + type);
        foreach (XmlAttribute attribute in source.Attributes) root.SetAttribute(attribute.LocalName, attribute.Value);
        foreach (XmlNode child in source.ChildNodes)
            if (child.NodeType == XmlNodeType.Element && (allChildren || child.LocalName == "ObjectFilter"))
                root.AppendChild(xml.ImportNode(child, true));
        return root;
    }
    static void MutateOwner(XmlNode node)
    {
        if (node.Attributes != null)
        {
            foreach (XmlAttribute attribute in node.Attributes)
            {
                if (attribute.LocalName == "Count") attribute.Value = "99";
                else if (attribute.LocalName == "TimeReq") attribute.Value = "999s";
                else if (attribute.LocalName == "VeterancyLevels") attribute.Value = "CHANGED";
                else if (attribute.LocalName == "Include") attribute.Value = "AIRCRAFT";
                else if (attribute.LocalName == "MinKillerAngle") attribute.Value = "10r";
            }
        }
        foreach (XmlNode child in node.ChildNodes)
        {
            if (child.LocalName == "UpgradeRequired" || child.LocalName == "IncludeThing" || child.LocalName == "ExcludeThing") child.InnerText = "ChangedWeakName";
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
        Require(prepared != null, "Complete conditions family must prepare: " + source.LocalName);
        try
        {
            Require(Property<bool>(prepared, "CanCache") && Property<bool>(prepared, "CanRunParallel"), "Complete conditions family must be independently eligible: " + source.LocalName);
            Require(Property<long>(prepared, "ExternalReadBytes") == 0, "Conditions metadata must have no leased external reads");
            EqualBuffers(expected, Execute(prepared), source.LocalName + "/" + platform);
            byte[] identity = Identity(prepared);
            InstanceDeclaration frozen = Property<InstanceDeclaration>(prepared, "Declaration");
            Require(!Object.ReferenceEquals(frozen.Node.OwnerDocument, declaration.Node.OwnerDocument), "Conditions DOM must be private");
            Require(expected.RelocationData.Length > 0, "Every complete conditions fixture must allocate its real nested payloads");
            Require(expected.ImportsData.Length == 0, "Conditions filter/upgrade names are weak hashes, not strong imports");
            bool big = platform != TargetPlatform.Win32;
            if (source.LocalName == "GameplayConditionData")
            {
                Require(Word(expected.InstanceData, 4, big) == 5 && Word(expected.InstanceData, 12, big) == 6 && Word(expected.InstanceData, 16, big) == 6,
                    "Gameplay condition/mode/difficulty masks must retain independent nondefault values");
                Require(Word(expected.InstanceData, 8, big) != 0 && Word(expected.InstanceData, 20, big) == 3,
                    "Gameplay time and the inherited inline primary count must be materialized");
            }
            else
            {
                Require(Word(expected.InstanceData, 0, big) == (source.LocalName == "UpgradeConditionData" ? 4u : 3u),
                    "The component-only condition roots store Count directly rather than a BaseAsset reserved word");
                Require(expected.InstanceData[8] == 1, "Unique must retain its one-byte non-endian representation");
                if (source.LocalName == "UpgradeConditionData")
                    Require(Word(expected.InstanceData, 12, big) == 3, "The real typed weak upgrade list must be retained");
            }
            // Mutate actual consumed fields in the owner after detachment,
            // including nested scalar/unit strings, bitsets and weak-name lists.
            ((XmlElement)declaration.Node).SetAttribute("OwnerOnlyMutation", "true");
            MutateOwner(declaration.Node);
            namespaces.AddNamespace("ea", "urn:owner-mutated");
            EqualBuffers(expected, Execute(prepared), source.LocalName + "/frozen/" + platform);
            Equal(identity, Identity(prepared), "Prepared conditions identity after owner mutation");
            evidence.Add(new { type = source.LocalName, platform = (int)platform, instanceBytes = expected.InstanceData.Length,
                relocationBytes = expected.RelocationData.Length, importsBytes = expected.ImportsData.Length, frozenOwnerMutation = true });
        }
        finally { ((IDisposable)prepared).Dispose(); }
    }
    public static int Main(string[] args)
    {
        try
        {
            string path = Path.GetFullPath(Path.Combine(args[4], "../recovery/tests/XmlconditionsProbe.xml"));
            var probe = new XmlDocument { XmlResolver = null }; probe.Load(path);
            XmlNode gameplay = probe.DocumentElement.SelectSingleNode("*[local-name()='GameplayConditionData']");
            Require(gameplay != null, "The real SDK gameplay condition global fixture is required");
            XmlNode primary = gameplay.SelectSingleNode("*[local-name()='PrimaryObject']");
            XmlNode upgrades = gameplay.SelectSingleNode("*[local-name()='RequiredUpgradeCondition']");
            Require(primary != null && upgrades != null, "Component-only condition fixtures must derive from complete real nested data");
            foreach (TargetPlatform platform in new[] { TargetPlatform.Win32, TargetPlatform.Xbox360, TargetPlatform.PlayStation3, (TargetPlatform)99 })
            {
                int publicRoots = 0;
                foreach (XmlNode source in probe.DocumentElement.ChildNodes)
                {
                    if (source.NodeType != XmlNodeType.Element) continue;
                    CheckFixture(source, platform); publicRoots++;
                }
                Require(publicRoots == 1, "Only GameplayConditionData has a real SDK global element");
                CheckFixture(Component(primary, "BaseConditionData", false), platform);
                CheckFixture(Component(primary, "ObjectConditionData", true), platform);
                CheckFixture(Component(upgrades, "UpgradeConditionData", true), platform);
            }
            string sha;
            using (var hash = SHA256.Create()) sha = BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant();
            File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(new {
                succeeded = true, sourceCommit = args[2], runId = args[3], checks, evidence, fixtureSha256 = sha,
                scope = "Complete Xmlconditions four-type prepared certification with nested ObjectFilter, weak upgrades, DieMux status/string/angle payloads and gameplay time/bitsets; detached owner mutation protection and four-platform including 99 three-buffer equality. Only GameplayConditionData is an SDK global root; actual production worker intervals are checked separately"
            }));
            Console.WriteLine("PASS XmlConditionsPrepared: " + checks + " assertions"); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
