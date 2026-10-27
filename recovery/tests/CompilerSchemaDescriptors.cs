using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;

public static class CompilerSchemaDescriptors
{
    const string Ns = "uri:ea.com:eala:asset";
    const string Switch = "NewSdk.Compiler.DisableSchemaReferenceDescriptors";
    static int checks;
    static readonly List<string> tests = new List<string>();
    static readonly MethodInfo Describe = typeof(SchemaSet).GetMethod("GetReferenceDescriptor", BindingFlags.NonPublic | BindingFlags.Instance);
    static void Require(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static object Field(object owner, string name) { return owner.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(owner); }
    static long Count(SchemaSet schema, string suffix) { return (long)Field(schema, "_ReferenceDescriptor" + suffix + "Count"); }
    static object Descriptor(SchemaSet schema, IXmlSchemaInfo info) { return Describe.Invoke(schema, new object[] { info }); }
    static string Shape(object descriptor) { return Field(descriptor, "Kind") + ":" + (Field(descriptor, "RefType") ?? "<missing>"); }
    static string Schema(string refType)
    {
        return "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:a='" + Ns + "' xmlns:xas='uri:ea.com:eala:asset:schema' xmlns:wrong='urn:wrong' targetNamespace='" + Ns + "' elementFormDefault='qualified'>" +
            "<xs:complexType name='BaseAssetType'/><xs:complexType name='BaseInheritableAsset'><xs:complexContent><xs:extension base='a:BaseAssetType'/></xs:complexContent></xs:complexType>" +
            "<xs:complexType name='TypeA'><xs:complexContent><xs:extension base='a:BaseAssetType'/></xs:complexContent></xs:complexType><xs:complexType name='TypeB'><xs:complexContent><xs:extension base='a:BaseAssetType'/></xs:complexContent></xs:complexType>" +
            "<xs:simpleType name='AssetReference'><xs:restriction base='xs:string'/></xs:simpleType><xs:simpleType name='WeakReference'><xs:restriction base='a:AssetReference'/></xs:simpleType><xs:simpleType name='FileReference'><xs:restriction base='xs:string'/></xs:simpleType><xs:simpleType name='Hashable'><xs:restriction base='xs:string'/></xs:simpleType>" +
            "<xs:simpleType name='TypedReference' xas:refType='TypeA'><xs:restriction base='a:AssetReference'/></xs:simpleType>" +
            "<xs:complexType name='ReferenceExtension'><xs:simpleContent><xs:extension base='a:TypedReference'/></xs:simpleContent></xs:complexType><xs:complexType name='ReferenceTwoLevels'><xs:simpleContent><xs:extension base='a:ReferenceExtension'/></xs:simpleContent></xs:complexType>" +
            "<xs:element name='Doc'><xs:complexType><xs:sequence>" +
            "<xs:element name='Strong' type='a:AssetReference' xas:refType='" + refType + "'/><xs:element name='Other' type='a:AssetReference' xas:refType='TypeB'/><xs:element name='Weak' type='a:WeakReference' xas:refType='TypeA'/><xs:element name='File' type='a:FileReference'/><xs:element name='Hash' type='a:Hashable'/><xs:element name='Plain' type='xs:string'/>" +
            "<xs:element name='Missing' type='a:AssetReference'/><xs:element name='WrongNamespace' type='a:AssetReference' wrong:refType='Wrong'/><xs:element name='Typed' type='a:TypedReference'/><xs:element name='Override' type='a:TypedReference' xas:refType='TypeB'/><xs:element name='Extended' type='a:ReferenceExtension'/><xs:element name='TooDeep' type='a:ReferenceTwoLevels'/>" +
            "</xs:sequence><xs:attribute name='AttrA' type='a:AssetReference' xas:refType='TypeA'/><xs:attribute name='AttrB' type='a:AssetReference' xas:refType='TypeB'/></xs:complexType></xs:element></xs:schema>";
    }
    static XmlDocument Document(SchemaSet schemas, string strong = "target", string missing = "target")
    {
        var document = new XmlDocument();
        document.LoadXml("<Doc xmlns='" + Ns + "' AttrA='target' AttrB='target'><Strong>" + strong + "</Strong><Other>target</Other><Weak>target</Weak><File>file.bin</File><Hash>hash</Hash><Plain>plain</Plain><Missing>" + missing + "</Missing><WrongNamespace>target</WrongNamespace><Typed>target</Typed><Override>target</Override><Extended>target</Extended><TooDeep>target</TooDeep></Doc>");
        document.Schemas = schemas.Schemas; document.Validate(null); return document;
    }
    static XPathNavigator Node(XmlDocument document, string name)
    {
        var navigator = document.CreateNavigator();
        var ns = new XmlNamespaceManager(navigator.NameTable); ns.AddNamespace("a", Ns);
        return navigator.SelectSingleNode(name.StartsWith("@", StringComparison.Ordinal) ? "/a:Doc/" + name : "/a:Doc/a:" + name, ns);
    }
    static SchemaSet Create(bool disabled)
    {
        AppContext.SetSwitch(Switch, disabled); return new SchemaSet(false);
    }
    static void Classification(string root)
    {
        Settings.Current = new Settings { SchemaPath = root, StringHashBinDescriptors = new[] { new StringHashBinDescriptor { SchemaTypeName = "Hashable", BinName = "DescriptorStrings" } } };
        var cached = Create(false); var fallback = Create(true); AppContext.SetSwitch(Switch, false);
        var document = Document(cached); var fallbackDocument = Document(fallback);
        string[] names = { "Strong", "Other", "Weak", "File", "Hash", "Plain", "Missing", "WrongNamespace", "Typed", "Override", "Extended", "TooDeep", "@AttrA", "@AttrB" };
        string[] expected = { "Strong:TypeA", "Strong:TypeB", "Weak:TypeA", "File:<missing>", "Hashable:<missing>", "None:<missing>", "Strong:<missing>", "Strong:<missing>", "Strong:TypeA", "Strong:TypeB", "Strong:TypeA", "Strong:<missing>", "Strong:TypeA", "Strong:TypeB" };
        for (int i = 0; i < names.Length; i++)
        {
            object first = Descriptor(cached, Node(document, names[i]).SchemaInfo);
            Require(Shape(first) == expected[i], names[i] + " classification/annotation precedence mismatch: " + Shape(first));
            for (int repeat = 0; repeat < 8; repeat++)
            {
                object warm = Descriptor(cached, Node(document, names[i]).SchemaInfo);
                object uncached = Descriptor(fallback, Node(fallbackDocument, names[i]).SchemaInfo);
                Require(Object.ReferenceEquals(first, warm), names[i] + " did not reuse an immutable description");
                Require(Shape(warm) == Shape(uncached), names[i] + " cache/fallback result mismatch");
            }
        }
        Require(Count(cached, "Build") == names.Length && Count(cached, "Hit") == names.Length * 8, "Cached descriptions did not eliminate repeated classification");
        Require(Count(fallback, "Build") == names.Length * 8 && Count(fallback, "Hit") == 0, "Fallback did not execute original classification every time");
        Require(((IDictionary)Field(fallback, "_ReferenceDescriptors")).Count == 0, "Disabled descriptions populated a cache");
        var polymorphic = Document(cached);
        polymorphic.DocumentElement.SetAttribute("xmlns:a", Ns);
        var missingElement = (XmlElement)polymorphic.DocumentElement.GetElementsByTagName("Missing", Ns)[0];
        XmlAttribute typeAttribute = polymorphic.CreateAttribute("xsi", "type", XmlSchema.InstanceNamespace); typeAttribute.Value = "a:TypedReference"; missingElement.Attributes.Append(typeAttribute);
        polymorphic.Validate(null);
        Require(Shape(Descriptor(cached, Node(polymorphic, "Missing").SchemaInfo)) == "Strong:TypeA", "Same declaration with xsi:type reused the declared-type description");
        Require(Shape(Descriptor(cached, Node(document, "Missing").SchemaInfo)) == "Strong:<missing>", "Polymorphic description contaminated the original declaration/type context");
        foreach (FieldInfo field in Descriptor(cached, Node(document, "Strong").SchemaInfo).GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            Require(field.IsInitOnly && (field.FieldType == typeof(string) || field.FieldType.IsEnum), "Description retains mutable/runtime schema state: " + field.Name);
        tests.Add("same-schema-generation-cold-warm-fallback-weak-before-strong-file-hashable-declaration-context-annotations-and-one-level-extension");
        var originalSchemas = cached.Schemas;
        cached.ReloadIfChanged(false);
        Require(Object.ReferenceEquals(originalSchemas, cached.Schemas) && Count(cached, "Build") == names.Length + 1, "Unchanged closure discarded its descriptor generation");
        DateTime time = File.GetLastWriteTimeUtc(root); long size = new FileInfo(root).Length;
        File.WriteAllText(root, Schema("TypeB"), new UTF8Encoding(false)); File.SetLastWriteTimeUtc(root, time);
        Require(File.GetLastWriteTimeUtc(root) == time && new FileInfo(root).Length == size, "Reload fixture must preserve exact timestamp and size");
        cached.ReloadIfChanged(false);
        Require(!Object.ReferenceEquals(originalSchemas, cached.Schemas) && Count(cached, "Build") == 0 && Count(cached, "Hit") == 0, "Content change did not establish a fresh descriptor generation");
        Require(Shape(Descriptor(cached, Node(Document(cached), "Strong").SchemaInfo)) == "Strong:TypeB", "New schema content reused the old contextual refType");
        File.WriteAllText(root, "<broken", new UTF8Encoding(false));
        for (int attempt = 0; attempt < 2; attempt++)
        {
            bool rejected = false;
            try { cached.ReloadIfChanged(false); } catch (BinaryAssetBuilderException error) { rejected = error.ErrorCode == ErrorCode.ReadingSchema; }
            Require(rejected && ((IDictionary)Field(cached, "_ReferenceDescriptors")).Count == 0 && Count(cached, "Build") == 0, "Failed/repeated load retained usable stale descriptions");
            bool failedGenerationRejected = false;
            try { Descriptor(cached, Node(document, "Strong").SchemaInfo); }
            catch (TargetInvocationException error) { failedGenerationRejected = error.InnerException is InvalidOperationException; }
            Require(failedGenerationRejected, "Failed generation accepted old schema runtime objects for new descriptions");
        }
        File.WriteAllText(root, Schema("TypeA"), new UTF8Encoding(false)); cached.ReloadIfChanged(false);
        Require(Shape(Descriptor(cached, Node(Document(cached), "Strong").SchemaInfo)) == "Strong:TypeA", "Repair did not load new descriptions");
        tests.Add("full-content-recheck-equal-size-equal-mtime-new-description-generation-failed-reload-clears-and-repair");
    }
    static string Handle(SchemaSet schemas, string field, string value)
    {
        var processor = (DocumentProcessor)FormatterServices.GetUninitializedObject(typeof(DocumentProcessor)); processor.SchemaSet = schemas;
        var document = new AssetDeclarationDocument();
        Type currentType = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object current = Activator.CreateInstance(currentType, true);
        currentType.GetField("DocumentProcessor").SetValue(current, processor);
        typeof(AssetDeclarationDocument).GetField("_Current", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(document, current);
        var instance = new InstanceDeclaration(document);
        var identity = new XmlDocument(); identity.LoadXml("<Asset id='DescriptorProbe'/>"); instance.XmlNode = identity.DocumentElement;
        var input = Document(schemas);
        XPathNavigator node = Node(input, field); node.SetValue(value); input.Validate(null); node = Node(input, field);
        string error = "success";
        using (var bytes = new MemoryStream())
        using (var writer = new BinaryWriter(bytes))
        {
            try { typeof(AssetDeclarationDocument).GetMethod("HandleReferenceType", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(document, new object[] { instance, node, writer }); }
            catch (TargetInvocationException wrapped)
            {
                Exception inner = wrapped.InnerException;
                error = inner.GetType().Name + ":" + (inner is BinaryAssetBuilderException ? ((BinaryAssetBuilderException)inner).ErrorCode.ToString() : "") + ":" + inner.Message;
            }
            return error + "|value=" + node.Value + "|strong=" + instance.ReferencedInstances.Count + "|weak=" + instance.WeakReferencedInstances.Count + "|ids=" + Convert.ToBase64String(bytes.ToArray());
        }
    }
    static void ProductionHandling()
    {
        var cached = Create(false); var fallback = Create(true); AppContext.SetSwitch(Switch, false);
        string[,] cases = { { "Strong", "target" }, { "Strong", "TypeA:target" }, { "Strong", "Unknown:target" }, { "Strong", "TypeB:target" }, { "Weak", "Unknown:target" }, { "Missing", "target" }, { "Missing", " " }, { "Missing", "Too:Many:Colons" }, { "TooDeep", "target" }, { "Other", "target" } };
        for (int i = 0; i < cases.GetLength(0); i++)
        {
            string cold = Handle(cached, cases[i, 0], cases[i, 1]);
            string warm = Handle(cached, cases[i, 0], cases[i, 1]);
            string disabled = Handle(fallback, cases[i, 0], cases[i, 1]);
            Require(cold == warm && warm == disabled, "Production reference handler cold/warm/fallback changed value, reference order, type ids or error: " + cases[i, 0] + "=" + cases[i, 1] + "\n" + cold + "\n" + warm + "\n" + disabled);
            if (cases[i, 0] == "Missing" && cases[i, 1] == " ") Require(cold.StartsWith("success|", StringComparison.Ordinal), "Empty asset reference must retain its early success before missing refType diagnostics");
            if (cases[i, 0] == "Missing" && cases[i, 1] == "Too:Many:Colons") Require(cold.StartsWith("ArgumentException:", StringComparison.Ordinal), "Invalid instance syntax must precede missing refType diagnostics");
            if (cases[i, 0] == "Missing" && cases[i, 1] == "target") Require(cold.Contains("ReferencingError") && cold.Contains("xas:refType missing"), "Missing reference-type diagnostic was swallowed");
        }
        tests.Add("production-HandleReferenceType-cold-warm-fallback-reference-numbering-type-ids-and-original-error-order");
    }
    public static int Main(string[] args)
    {
        int code = 0; object result; Settings previous = Settings.Current;
        bool previousSwitch; AppContext.TryGetSwitch(Switch, out previousSwitch);
        try
        {
            if (args.Length != 5 || IntPtr.Size != 4) throw new Exception("Expected work result sourceCommit runId sdk (x86)");
            Directory.CreateDirectory(args[0]); string root = Path.Combine(args[0], "descriptors.xsd");
            File.WriteAllText(root, Schema("TypeA"), new UTF8Encoding(false));
            Classification(root); ProductionHandling();
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks = checks, tests = tests.ToArray(), scope = "Per-SchemaSet-generation immutable reference descriptions; not shared mutable XmlSchemaSet, cross-stream SchemaSnapshot or parallel acceptance" };
            Console.WriteLine("PASS SchemaDescriptors: " + tests.Count + " groups; " + checks + " checks");
        }
        catch (Exception error)
        {
            code = 1; Console.Error.WriteLine(error); result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null, runId = args.Length > 3 ? args[3] : null, checks = checks, tests = tests.ToArray(), error = error.ToString() };
        }
        finally { Settings.Current = previous; AppContext.SetSwitch(Switch, previousSwitch); }
        if (args.Length > 1) File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(result));
        return code;
    }
}
