using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder.Core;
using NewModCompiler.Application;

// Executed by the independent host linked from the actual production modules.
// Reflection keeps the prepared representation an internal application detail.
public static class CompilerPreparedBackend
{
    static int checks;
    static readonly Type contract = typeof(CoreStreamCompiler).Assembly.GetType("NewModCompiler.Application.PreparedBackend", true);
    static void Require(bool condition, string message)
    {
        checks++;
        if (!condition) throw new InvalidOperationException(message);
    }
    static object Prepare(InstanceDeclaration declaration, IAssetBuilderPlugin plugin)
    {
        var arguments = new object[] { declaration, plugin, null };
        bool accepted = (bool)contract.GetMethod("TryPrepare", BindingFlags.Public | BindingFlags.Static).Invoke(null, arguments);
        return accepted ? arguments[2] : null;
    }
    static T Value<T>(object prepared, string property)
    {
        return (T)contract.GetProperty(property, BindingFlags.Public | BindingFlags.Instance).GetValue(prepared, null);
    }
    static string Identity(object prepared)
    {
        using (var bytes = new MemoryStream())
        using (var writer = new BinaryWriter(bytes, Encoding.UTF8))
        using (var sha = SHA256.Create())
        {
            contract.GetMethod("WriteIdentity").Invoke(prepared, new object[] { writer });
            writer.Flush();
            return BitConverter.ToString(sha.ComputeHash(bytes.ToArray()));
        }
    }
    static AssetBuffer Execute(object prepared)
    {
        return (AssetBuffer)contract.GetMethod("Execute").Invoke(prepared, null);
    }
    static InstanceDeclaration Declaration(string type, string id, XmlNamespaceManager namespaces)
    {
        var xml = new XmlDocument { XmlResolver = null };
        var root = xml.CreateElement(type, "uri:ea.com:eala:asset");
        root.SetAttribute("id", id); xml.AppendChild(root);
        var declaration = new InstanceDeclaration(AssetDeclarationDocument.CreateBackendView(namespaces));
        declaration.XmlNode = root;
        return declaration;
    }
    static XmlNamespaceManager Namespaces()
    {
        var namespaces = new XmlNamespaceManager(new NameTable());
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        return namespaces;
    }
    static void Metadata(InstanceDeclaration instance, IAssetBuilderPlugin plugin)
    {
        var information = plugin.GetExtendedTypeInformation(instance.Handle.TypeId);
        instance.Handle.TypeHash = information.TypeHash;
        instance.ProcessingHash = information.ProcessingHash;
        instance.HasCustomData = information.HasCustomData;
    }
    static bool Contains(byte[] data, byte[] expected)
    {
        for (int i = 0; i <= data.Length - expected.Length; ++i)
        {
            int match = 0;
            while (match < expected.Length && data[i + match] == expected[match]) match++;
            if (match == expected.Length) return true;
        }
        return false;
    }
    static void Denied(Action action, string operation)
    {
        bool denied = false;
        try { action(); }
        catch (IOException) { denied = true; }
        catch (UnauthorizedAccessException) { denied = true; }
        Require(denied, "Prepared input must reject " + operation + " while its content lease is held");
    }
    static void AptLeaseAndFreeze(string work)
    {
        string path = Path.Combine(work, "prepared-blob.bin");
        string alternate = Path.Combine(work, "alternate-blob.bin");
        byte[] first = Encoding.ASCII.GetBytes("ABCDEFGHIJKLMNOP");
        byte[] second = Encoding.ASCII.GetBytes("QRSTUVWXYZabcdef");
        File.WriteAllBytes(path, first); File.WriteAllBytes(alternate, second);
        var plugin = new Recovered.XmlPriority.Plugin();
        plugin.Initialize(null, TargetPlatform.Win32);
        var namespaces = Namespaces();
        var instance = Declaration("AptAptData", "PreparedApt", namespaces);
        ((XmlElement)instance.Node).SetAttribute("File", path); Metadata(instance, plugin);
        string before;
        object prepared = Prepare(instance, plugin);
        Require(prepared != null && Value<bool>(prepared, "CanCache"), "Existing APT blob input must be cacheable");
        try
        {
            before = Identity(prepared);
            Require(!Value<bool>(prepared, "CanRunParallel"), "XML parallelism must remain separately gated");
            Denied(delegate { using (var writer = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite)) { } }, "a writer");
            Denied(delegate { File.Delete(path); }, "deletion");
            Denied(delegate { File.Move(path, path + ".moved"); }, "rename");
            // Native fopen must still be allowed to read while the owner holds
            // FileShare.Read. Neither a new XML value nor a namespace mutation
            // may alter the actual private representation being executed.
            ((XmlElement)instance.Node).SetAttribute("File", alternate);
            namespaces.AddNamespace("ea", "urn:changed-after-prepare");
            var frozen = Value<InstanceDeclaration>(prepared, "Declaration");
            Require(frozen.Node.Attributes["File"].Value == path, "Prepared XML must be independent of owner mutation");
            Require(frozen.Document.NamespaceManager.LookupNamespace("ea") == "uri:ea.com:eala:asset", "Namespace table must be private");
            Require(!Object.ReferenceEquals(frozen.Node.OwnerDocument, instance.Node.OwnerDocument), "XML document and name table must be private");
            var result = Execute(prepared);
            Require(result != null && Contains(result.InstanceData, first) && !Contains(result.InstanceData, second), "Native APT consumption must match the held and hashed version");
            Require(before == Identity(prepared), "Owner mutations must not change prepared identity");
        }
        finally { ((IDisposable)prepared).Dispose(); }
        ((XmlElement)instance.Node).SetAttribute("File", path);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        DateTime timestamp = File.GetLastWriteTimeUtc(path);
        File.WriteAllBytes(path, second); File.SetLastWriteTimeUtc(path, timestamp);
        Require(new FileInfo(path).Length == first.Length && File.GetLastWriteTimeUtc(path) == timestamp, "Equal-length/mtime fixture must be exact");
        prepared = Prepare(instance, plugin);
        Require(prepared != null, "A released input lease must allow a new version");
        try { Require(before != Identity(prepared), "Equal-length equal-mtime external byte mutation must change prepared identity"); }
        finally { ((IDisposable)prepared).Dispose(); }
        File.Delete(path);
        Require(Prepare(instance, plugin) == null, "A missing APT read must use the compatibility path rather than an incomplete identity");
    }
    static void W3xBudget()
    {
        var plugin = new Recovered.Compiler.W3x.NativePlugin();
        plugin.Initialize(null, TargetPlatform.Win32);
        var instance = Declaration("W3DAnimation", "PreparedAnimation", Namespaces());
        Metadata(instance, plugin);
        ((XmlElement)instance.Node).SetAttribute("NumFrames", "65536");
        var channels = instance.Node.OwnerDocument.CreateElement("Channels", "uri:ea.com:eala:asset");
        var channel = instance.Node.OwnerDocument.CreateElement("W3DAnimationChannelScalar", "uri:ea.com:eala:asset");
        channels.AppendChild(channel); instance.Node.AppendChild(channels);
        object prepared = Prepare(instance, plugin);
        Require(prepared != null, "W3X must have a certified preparation contract");
        try
        {
            Require(Value<bool>(prepared, "CanRunParallel"), "A bounded 65536-frame single channel animation may run in the W3X pool");
            Require(Value<long>(prepared, "PreparedBytes") >= 65536L * 256L, "Declared frame expansion must be included in the lifecycle budget");
        }
        finally { ((IDisposable)prepared).Dispose(); }
        ((XmlElement)instance.Node).SetAttribute("NumFrames", "4294967295");
        prepared = Prepare(instance, plugin);
        try { Require(!Value<bool>(prepared, "CanRunParallel"), "A tiny XML declaring huge expansion must stay on the owner path"); }
        finally { ((IDisposable)prepared).Dispose(); }
    }
    static void SchemaDefaults()
    {
        var xml = new XmlDocument { XmlResolver = null };
        xml.LoadXml("<ArmorTemplate xmlns='uri:ea.com:eala:asset' id='PreparedArmor'/>");
        using (var reader = XmlReader.Create(new StringReader(
            "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='uri:ea.com:eala:asset'>" +
            "<xs:element name='ArmorTemplate'><xs:complexType><xs:attribute name='id' type='xs:string'/>" +
            "<xs:attribute name='Default' type='xs:string' default='100'/></xs:complexType></xs:element></xs:schema>")))
            xml.Schemas.Add(null, reader);
        xml.Validate(null);
        Require(xml.DocumentElement.Attributes["Default"] != null && !xml.DocumentElement.Attributes["Default"].Specified,
            "Fixture must contain a real schema-supplied unspecified default");
        var instance = new InstanceDeclaration(AssetDeclarationDocument.CreateBackendView(Namespaces()));
        instance.XmlNode = xml.DocumentElement;
        var plugin = new Recovered.Compiler.Plugin(); plugin.Initialize(null, TargetPlatform.Win32);
        Metadata(instance, plugin);
        object prepared = Prepare(instance, plugin);
        try
        {
            var frozen = Value<InstanceDeclaration>(prepared, "Declaration");
            Require(frozen.Node.Attributes["Default"] != null && frozen.Node.Attributes["Default"].Value == "100",
                "The actual backend input must preserve schema default values during detachment");
            Require(Execute(prepared) != null, "The detached default-bearing Armor declaration must execute through the real backend");
        }
        finally { ((IDisposable)prepared).Dispose(); }
    }
    public static int Main(string[] args)
    {
        try
        {
            Directory.CreateDirectory(args[0]); AptLeaseAndFreeze(args[0]); W3xBudget(); SchemaDefaults();
            File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(new {
                succeeded = true, sourceCommit = args[2], runId = args[3], checks,
                scope = "Actual prepared input isolation, native APT content lease, exact external invalidation and W3X expansion budget; cross-document overlap is checked separately"
            }));
            Console.WriteLine("PASS PreparedBackend: " + checks + " assertions"); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
