using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;

// The original SDK DLL is loaded alongside the production-object EXE; the
// comparison does not use a copied/reimplemented baseline algorithm.
public static class CompilerNodeJoiner
{
    const string Ns = "uri:ea.com:eala:asset";
    const string InstanceNs = "uri:ea.com:eala:asset:instance";
    static int checks, cases;
    static long originalAttributeReads, indexedAttributeReads, disabledAttributeReads;
    static readonly List<string> tests = new List<string>();
    static MethodInfo original;
    static XmlSchemaSet schemas;
    static void Require(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }

    sealed class CountingDocument : XmlDocument
    {
        internal long AttributeReads;
        public override XmlElement CreateElement(string prefix, string localName, string namespaceUri)
        {
            return new CountingElement(prefix, localName, namespaceUri, this);
        }
    }
    sealed class CountingElement : XmlElement
    {
        readonly CountingDocument owner;
        internal CountingElement(string prefix, string localName, string namespaceUri, CountingDocument document)
            : base(prefix, localName, namespaceUri, document) { owner = document; }
        public override XmlAttributeCollection Attributes { get { owner.AttributeReads++; return base.Attributes; } }
    }
    static XmlSchemaSet Schema()
    {
        const string text = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:a='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'>" +
            "<xs:complexType name='ItemType'><xs:sequence><xs:element name='Value' type='xs:string' minOccurs='0' maxOccurs='unbounded'/></xs:sequence><xs:attribute name='id' type='xs:string'/><xs:attribute name='mark' type='xs:string'/><xs:anyAttribute processContents='skip'/></xs:complexType>" +
            "<xs:complexType name='Asset'><xs:sequence><xs:element name='Item' type='a:ItemType' minOccurs='0' maxOccurs='unbounded'/><xs:element name='Other' type='a:ItemType' minOccurs='0' maxOccurs='unbounded'/><xs:element name='Tail' type='xs:string' minOccurs='0'/></xs:sequence><xs:attribute name='id' type='xs:string'/><xs:anyAttribute processContents='skip'/></xs:complexType>" +
            "<xs:complexType name='Choice'><xs:choice minOccurs='0'><xs:element name='Item' type='a:ItemType'/><xs:element name='Other' type='a:ItemType'/></xs:choice><xs:anyAttribute processContents='skip'/></xs:complexType>" +
            "<xs:complexType name='Many'><xs:choice minOccurs='0' maxOccurs='unbounded'><xs:element name='Item' type='a:ItemType'/><xs:element name='Other' type='a:ItemType'/></xs:choice><xs:anyAttribute processContents='skip'/></xs:complexType>" +
            "<xs:element name='Asset' type='a:Asset'/><xs:element name='Choice' type='a:Choice'/><xs:element name='Many' type='a:Many'/></xs:schema>";
        var result = new XmlSchemaSet();
        using (var reader = XmlReader.Create(new StringReader(text))) result.Add(Ns, reader);
        result.Compile(); return result;
    }
    static XmlDocument Document(string root, string body, string rootAttributes = "")
    {
        var document = new XmlDocument();
        document.LoadXml("<" + root + " xmlns='" + Ns + "' xmlns:p='" + Ns + "' xmlns:i='" + InstanceNs + "' " + rootAttributes + ">" + body + "</" + root + ">");
        document.Schemas = schemas;
        // NodeJoiner also operates on pre-validation input. Populate available
        // type annotations without hiding its own rejection behavior.
        document.Validate(delegate(object sender, ValidationEventArgs args) { });
        return document;
    }
    static string Outcome(bool legacy, XmlDocument baseDocument, XmlDocument overrideDocument, CountingDocument destination, bool disableIndex = false)
    {
        bool previousSwitch;
        AppContext.TryGetSwitch("NewSdk.Compiler.DisableNodeJoinerIndex", out previousSwitch);
        if (!legacy) AppContext.SetSwitch("NewSdk.Compiler.DisableNodeJoinerIndex", disableIndex);
        try
        {
            XmlNode result = legacy
                ? (XmlNode)original.Invoke(null, new object[] { schemas, destination, baseDocument.DocumentElement, overrideDocument.DocumentElement })
                : NodeJoiner.Override(schemas, destination, baseDocument.DocumentElement, overrideDocument.DocumentElement);
            return "XML:" + result.OuterXml;
        }
        catch (Exception error)
        {
            var invoke = error as TargetInvocationException;
            if (invoke != null) error = invoke.InnerException;
            PropertyInfo code = error.GetType().GetProperty("ErrorCode");
            return "ERROR:" + error.GetType().Name + ":" + (code == null ? "" : Convert.ToString(code.GetValue(error, null))) + ":" + error.Message;
        }
        finally { if (!legacy) AppContext.SetSwitch("NewSdk.Compiler.DisableNodeJoinerIndex", previousSwitch); }
    }
    static string Compare(string label, string root, string before, string after, bool failure, string overrideAttributes = "")
    {
        var oldDestination = new CountingDocument(); var newDestination = new CountingDocument(); var disabledDestination = new CountingDocument();
        string expected = Outcome(true, Document(root, before), Document(root, after, overrideAttributes), oldDestination);
        string actual = Outcome(false, Document(root, before), Document(root, after, overrideAttributes), newDestination);
        Require(expected == actual, label + " original DLL differential failed\nexpected: " + expected + "\nactual: " + actual);
        string disabled = Outcome(false, Document(root, before), Document(root, after, overrideAttributes), disabledDestination, true);
        Require(expected == disabled, label + " disabled-index original DLL differential failed\nexpected: " + expected + "\nactual: " + disabled);
        Require(expected.StartsWith(failure ? "ERROR:" : "XML:", StringComparison.Ordinal), label + " did not exercise its intended outcome: " + expected);
        cases++; originalAttributeReads += oldDestination.AttributeReads; indexedAttributeReads += newDestination.AttributeReads; disabledAttributeReads += disabledDestination.AttributeReads;
        return actual;
    }
    static string Items(int count)
    {
        var text = new StringBuilder();
        for (int i = 0; i < count; i++) text.Append("<Item id='k").Append(i).Append("' mark='base'><Value>base</Value></Item>");
        return text.ToString();
    }
    static void Semantics()
    {
        foreach (int count in new[] { 0, 1, 7, 8, 9, 32 })
        {
            string before = Items(count);
            Compare("append-" + count, "Asset", before, "<Item id='new' mark='tail'/>", false);
            Compare("top-" + count, "Asset", before, "<Item id='new' i:insertPosition='Top'/><Item id='next' i:insertPosition='Top'/>", false);
            Compare("duplicate-update-" + count, "Asset", before + "<Item id='dup' mark='first'/><Item id='dup' mark='second'/>", "<Item id='dup' mark='override'/>", false);
            Compare("remove-then-reselect-" + count, "Asset", before + "<Item id='dup' mark='first'/><Item id='dup' mark='second'/>", "<Item id='dup' i:joinAction='Remove'/><Item id='dup' mark='remaining'/>", false);
            Compare("replace-top-and-reselect-" + count, "Asset", before + "<Item id='dup' mark='first'/><Item id='dup' mark='second'/>", "<Item id='dup' i:joinAction='Replace' i:insertPosition='Top'/><Item id='dup' mark='new-first'/>", false);
            Compare("empty-id-" + count, "Asset", before + "<Item id='' mark='first'/><Item id='' mark='second'/>", "<Item id='' i:joinAction='Remove'/><Item id='' mark='remaining'/>", false);
            Compare("missing-id-append-" + count, "Asset", before, "<Item/><Item/>", false);
            Compare("missing-removal-" + count, "Asset", before, "<Item id='absent' i:joinAction='Remove'/>", true);
            Compare("invalid-action-" + count, "Asset", before, "<Item id='new' i:joinAction='Invalid'/>", true);
            Compare("invalid-position-" + count, "Asset", before, "<Item id='new' i:insertPosition='Invalid'/>", true);
            Compare("case-insensitive-action-" + count, "Asset", before + "<Item id='dup'/>", "<Item id='dup' i:joinAction='rEmOvE'/>", false);
            Compare("nested-text-and-comments-" + count, "Asset", before + "<Item id='dup'><Value>old</Value></Item>", "<!--ignored--><Item id='dup'><Value>new</Value></Item>", false);
        }
        string duplicate = Items(12) + "<Item id='shared' mark='first'/><Other id='shared' mark='other'/>";
        Compare("id-matches-across-node-names", "Many", duplicate, "<Other id='shared' mark='changed'/>", false);
        Compare("prefixed-sequence-child-retains-original-rejection", "Asset", Items(12) + "<p:Item id='shared' mark='prefixed'/>", "<Item id='shared' mark='unprefixed'/>", true);
        Compare("schema-order-append", "Asset", Items(12) + "<Other id='o'/><Tail>end</Tail>", "<Item id='new'/><Other id='p'/>", false);
        Compare("single-choice-replace-any", "Choice", "<Item id='old'/>", "<Other id='new'/>", false);
        Compare("single-choice-id-attribute-change", "Choice", "<Item id='old'/>", "<Item id='new'/>", false);
        Compare("top-level-replace", "Asset", Items(12), "<Item id='new'/>", false, "i:joinAction='Replace'");
        Compare("top-level-remove-rejected", "Asset", Items(12), "", true, "i:joinAction='Remove'");
        tests.Add("original-SDK-NodeJoiner-DLL-exact-XML-and-rejection-differential-small-threshold-large-duplicate-order-id-prefix-choice-insertion-removal");
    }
    static void DuplicateIndexSemantics()
    {
        MethodInfo oldSelect = original.DeclaringType.GetMethod("SelectSame", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo newSelect = typeof(NodeJoiner).GetMethod("SelectSame", BindingFlags.Instance | BindingFlags.NonPublic);
        var destination = new XmlDocument();
        destination.LoadXml("<Parent xmlns:p='urn:p' xmlns:q='urn:q'><A id='dup' mark='first'/><B id='dup' mark='second'/><A/><A id=''/><p:A id='p'/><q:A id='q'/><A id='last'/><A id='end'/></Parent>");
        XmlNode parent = destination.DocumentElement;
        object oldJoiner = Activator.CreateInstance(original.DeclaringType, new object[] { schemas, destination });
        var newJoiner = new NodeJoiner(schemas, destination);
        object index = null;
        Action<string, bool, bool> select = delegate(string xml, bool byId, bool replaceAny)
        {
            var source = new XmlDocument(); source.LoadXml(xml);
            object expected = oldSelect.Invoke(oldJoiner, new object[] { parent, source.DocumentElement, byId, replaceAny });
            object[] arguments = { parent, source.DocumentElement, byId, replaceAny, index };
            object actual = newSelect.Invoke(newJoiner, arguments); index = arguments[4];
            Require(Object.ReferenceEquals(expected, actual), "Private original selector differential: " + xml + " byId=" + byId + " replaceAny=" + replaceAny);
        };
        select("<B id='dup'/>", true, false);
        Require(index != null, "Wide duplicate-parent fixture did not instantiate an index");
        select("<A/>", false, false);
        select("<A id=''/>", true, false);
        select("<A/>", true, false);
        select("<p:A xmlns:p='urn:other'/>", false, false);
        select("<r:A xmlns:r='urn:p'/>", false, false);
        Type indexType = index.GetType();
        MethodInfo remove = indexType.GetMethod("Remove", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo refresh = indexType.GetMethod("Refresh", BindingFlags.Instance | BindingFlags.NonPublic);
        XmlNode first = parent.FirstChild;
        remove.Invoke(index, new object[] { first }); parent.RemoveChild(first);
        select("<A id='dup'/>", true, false);
        var inserted = destination.CreateElement("A"); inserted.SetAttribute("id", "dup");
        parent.PrependChild(inserted); refresh.Invoke(index, new object[] { inserted });
        select("<B id='dup'/>", true, false);
        inserted.SetAttribute("id", "renamed"); refresh.Invoke(index, new object[] { inserted });
        select("<A id='dup'/>", true, false); select("<A id='renamed'/>", true, false);
        inserted.RemoveAttribute("id"); refresh.Invoke(index, new object[] { inserted });
        select("<A id='renamed'/>", true, false); select("<A/>", false, false);
        while (parent.ChildNodes.Count > 1)
        {
            XmlNode last = parent.LastChild; remove.Invoke(index, new object[] { last }); parent.RemoveChild(last);
        }
        select("<Unrelated/>", false, true);
        tests.Add("original-private-selector-duplicate-first-key-name-prefix-insert-remove-key-refresh-and-shrink-to-single-choice");
    }
    static void WorkReduction()
    {
        const int count = 512;
        var after = new StringBuilder();
        for (int i = count - 1; i >= 0; i--) after.Append("<Item id='k").Append(i).Append("' mark='changed'/>");
        long oldBefore = originalAttributeReads, newBefore = indexedAttributeReads;
        Compare("wide-reversed-keyed-overrides", "Asset", Items(count), after.ToString(), false);
        long oldReads = originalAttributeReads - oldBefore, newReads = indexedAttributeReads - newBefore;
        // Count actual destination XML attribute accesses, not elapsed CI time.
        // This bounds repeated matching work while exact XML equality remains
        // the correctness gate. No production global counter is introduced.
        Require(newReads < count * 40L, "Indexed matching still performs quadratic destination attribute reads: " + newReads);
        Require(oldReads > newReads * 4L, "Fixture did not demonstrate reduced matching work: original=" + oldReads + " indexed=" + newReads);
        Require(disabledAttributeReads >= originalAttributeReads, "Disabled index did not preserve legacy destination attribute access work");
        tests.Add("512-reversed-overrides-original-output-equality-and-linear-destination-attribute-access-bound-with-legacy-fallback");
    }
    public static int Main(string[] args)
    {
        int code = 0; object result;
        try
        {
            if (args.Length != 5 || IntPtr.Size != 4) throw new Exception("Expected work result sourceCommit runId sdk (x86)");
            Directory.CreateDirectory(args[0]);
            string library = Path.Combine(args[4], "tools/BinaryAssetBuilder.Core.dll");
            Assembly baseline = Assembly.LoadFrom(library);
            Require(baseline != typeof(NodeJoiner).Assembly, "Original SDK assembly resolved to the production EXE");
            original = baseline.GetType("BinaryAssetBuilder.Core.NodeJoiner", true).GetMethod("Override", BindingFlags.Public | BindingFlags.Static);
            Require(original != null, "Original SDK NodeJoiner.Override missing");
            schemas = Schema(); Semantics(); DuplicateIndexSemantics(); WorkReduction();
            string sha256;
            using (var sha = SHA256.Create()) sha256 = BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(library))).Replace("-", "").ToLowerInvariant();
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks = checks, tests = tests.ToArray(), cases = cases,
                originalDllSha256 = sha256, originalDllAssembly = baseline.FullName, originalDestinationAttributeReads = originalAttributeReads,
                indexedDestinationAttributeReads = indexedAttributeReads, disabledDestinationAttributeReads = disabledAttributeReads, scope = "Real original DLL NodeJoiner XML/rejection differential and matching work; no end-to-end performance claim" };
            Console.WriteLine("PASS NodeJoiner: " + cases + " original DLL differential cases; destination attribute reads original=" + originalAttributeReads + " indexed=" + indexedAttributeReads);
        }
        catch (Exception error)
        {
            code = 1; Console.Error.WriteLine(error);
            result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null, runId = args.Length > 3 ? args[3] : null,
                checks = checks, tests = tests.ToArray(), cases = cases, error = error.ToString() };
        }
        if (args.Length > 1) File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(result));
        return code;
    }
}
