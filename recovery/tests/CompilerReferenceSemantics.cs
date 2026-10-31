using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using EALA.Hash;

// Small CLR/PSVI and isolated production-reference probes in the production-object host.
internal static class CompilerReferenceSemantics
{
    const string Ns = "uri:ea.com:eala:asset";
    const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    static int checks;
    static readonly List<string> tests = new List<string>();
    static SchemaSet schemas;
    static DocumentProcessor processor;
    static void Need(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    static string Schema()
    {
        return @"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' xmlns:xas='uri:ea.com:eala:asset:schema' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'>
<xs:complexType name='BaseAssetType'><xs:attribute name='id' type='xs:string' use='required'/></xs:complexType>
<xs:complexType name='BaseInheritableAsset'><xs:complexContent><xs:extension base='BaseAssetType'/></xs:complexContent></xs:complexType>
<xs:simpleType name='AssetReference'><xs:restriction base='xs:string'/></xs:simpleType>
<xs:simpleType name='WeakReference'><xs:restriction base='xs:string'/></xs:simpleType>
<xs:simpleType name='FileReference'><xs:restriction base='xs:string'/></xs:simpleType>
<xs:complexType name='BaseThing'><xs:complexContent><xs:extension base='BaseInheritableAsset'/></xs:complexContent></xs:complexType>
<xs:complexType name='DerivedThing'><xs:complexContent><xs:extension base='BaseThing'/></xs:complexContent></xs:complexType>
<xs:complexType name='OtherThing'><xs:complexContent><xs:extension base='BaseInheritableAsset'/></xs:complexContent></xs:complexType>
<xs:simpleType name='Strong' xas:refType='BaseThing'><xs:restriction base='AssetReference'/></xs:simpleType>
<xs:simpleType name='OtherStrong' xas:refType='OtherThing'><xs:restriction base='Strong'/></xs:simpleType>
<xs:complexType name='StrongBox'><xs:simpleContent><xs:extension base='Strong'><xs:attribute name='Tag' type='xs:string' default='kept'/></xs:extension></xs:simpleContent></xs:complexType>
<xs:simpleType name='Weak' xas:refType='BaseThing'><xs:restriction base='WeakReference'/></xs:simpleType>
<xs:simpleType name='Missing'><xs:restriction base='AssetReference'/></xs:simpleType>
<xs:simpleType name='UnderlyingMissing' xas:refType='Gone'><xs:restriction base='AssetReference'/></xs:simpleType>
<xs:complexType name='Container'><xs:complexContent><xs:extension base='BaseAssetType'><xs:choice minOccurs='0' maxOccurs='unbounded'>
<xs:element name='Strong' type='Strong'/><xs:element name='ByElement' type='AssetReference' xas:refType='BaseThing'/><xs:element name='Box' type='StrongBox'/>
<xs:element name='Dynamic' type='Strong'/><xs:element name='Weak' type='Weak'/><xs:element name='Missing' type='Missing'/><xs:element name='UnderlyingMissing' type='UnderlyingMissing'/>
</xs:choice><xs:attribute name='AttributeStrong' type='Strong'/><xs:attribute name='AttributeDefault' type='Strong' default='BaseThing:Default'/></xs:extension></xs:complexContent></xs:complexType>
<xs:element name='Container' type='Container'/></xs:schema>";
    }
    static XmlDocument Document(string body, string attributes = "")
    {
        var xml = new XmlDocument { PreserveWhitespace = true, Schemas = schemas.Schemas };
        xml.LoadXml("<Container xmlns='" + Ns + "' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' id='Probe' " + attributes + ">" + body + "</Container>");
        xml.Validate(null);
        return xml;
    }
    static void SameInfo(IXmlSchemaInfo left, IXmlSchemaInfo right, string label)
    {
        Need(left != null && right != null && Object.ReferenceEquals(left.SchemaType, right.SchemaType) &&
            Object.ReferenceEquals(left.SchemaElement, right.SchemaElement) && Object.ReferenceEquals(left.SchemaAttribute, right.SchemaAttribute) &&
            left.Validity == right.Validity && left.IsDefault == right.IsDefault && left.IsNil == right.IsNil, "PSVI differs: " + label);
    }
    static void DomEquivalence()
    {
        foreach (string value in new[] {" BaseThing:One ", "<![CDATA[ BaseThing:One ]]>", "BaseThing:<!--comment-->One", "BaseThing:O&#110;e"})
        {
            var old = Document("<Box>" + value + "</Box>", "AttributeStrong=' BaseThing:Attribute '");
            var current = Document("<Box>" + value + "</Box>", "AttributeStrong=' BaseThing:Attribute '");
            var oldBox = (XmlElement)old.DocumentElement.FirstChild;
            var currentBox = (XmlElement)current.DocumentElement.FirstChild;
            var oldNav = oldBox.CreateNavigator();
            SameInfo(oldNav.SchemaInfo, oldBox.SchemaInfo, "element XPath/DOM before rewrite");
            SameInfo(oldNav.SchemaInfo, currentBox.SchemaInfo, "independent PSVI before rewrite");
            XmlSchemaType boxType = currentBox.SchemaInfo.SchemaType;
            Need(currentBox.GetAttribute("Tag") == "kept" && !currentBox.GetAttributeNode("Tag").Specified, "XSD default attribute absent");
            oldNav.SetValue("BaseThing:One\\0"); currentBox.InnerText = "BaseThing:One\\0";
            SameInfo(oldNav.SchemaInfo, currentBox.SchemaInfo, "element after SetValue/InnerText");
            Need(Object.ReferenceEquals(boxType, currentBox.SchemaInfo.SchemaType), "Element rewrite lost its type information");
            oldNav.CreateAttribute("", "TypeId", "", "123"); currentBox.SetAttribute("TypeId", "123");
            SameInfo(oldNav.SchemaInfo, currentBox.SchemaInfo, "element after TypeId insertion");
            SameInfo(oldBox.GetAttributeNode("Tag").SchemaInfo, currentBox.GetAttributeNode("Tag").SchemaInfo, "retained XSD default attribute");
            foreach (string name in new[] { "AttributeStrong", "AttributeDefault" })
            {
                XmlAttribute before = old.DocumentElement.GetAttributeNode(name), after = current.DocumentElement.GetAttributeNode(name);
                var nav = before.CreateNavigator();
                SameInfo(nav.SchemaInfo, before.SchemaInfo, "attribute XPath/DOM");
                nav.SetValue("BaseThing:Attribute\\1"); after.InnerText = "BaseThing:Attribute\\1";
                SameInfo(nav.SchemaInfo, after.SchemaInfo, "attribute SetValue/InnerText");
                Need(before.Specified == after.Specified, "Default-attribute Specified semantics changed");
            }
            Need(old.OuterXml == current.OuterXml, "XPath/DOM rewriting changed CDATA/comment/entity/default-attribute output");
        }
        tests.Add("validated-CLR-XPath-DOM-schema-info-value-rewrite-CDATA-comment-entity-default-attribute-TypeId-equivalence");
    }
    static InstanceDeclaration References(XmlDocument xml, out uint hash)
    {
        var ns = new XmlNamespaceManager(xml.NameTable); ns.AddNamespace("ea", Ns);
        AssetDeclarationDocument owner = AssetDeclarationDocument.CreateBackendView(ns);
        object state = typeof(AssetDeclarationDocument).GetField("_Current", Private).GetValue(owner);
        state.GetType().GetField("DocumentProcessor").SetValue(state, processor);
        var instance = new InstanceDeclaration(owner) { XmlNode = xml.DocumentElement };
        MethodInfo method = typeof(AssetDeclarationDocument).GetMethod("ProcessInstanceReferences", Private);
        Type bufferType = method.GetParameters()[1].ParameterType.GetElementType();
        object[] arguments = { instance, Activator.CreateInstance(bufferType), new Dictionary<string, FileHashItem>(StringComparer.Ordinal) };
        try { method.Invoke(owner, arguments); }
        catch (TargetInvocationException error) { throw error.InnerException; }
        hash = (uint)bufferType.GetMethod("GetHash", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(arguments[1], null);
        return instance;
    }
    static uint ExpectedHash(params string[] types)
    {
        using (var bytes = new MemoryStream())
        using (var writer = new BinaryWriter(bytes))
        {
            foreach (string type in types) writer.Write(HashProvider.GetCaseSensitiveSymbolHash(type));
            return types.Length == 0 ? 0u : FastHash.GetHashCode(bytes.GetBuffer());
        }
    }
    static void ReferenceHashCapacity()
    {
        Type bufferType = typeof(AssetDeclarationDocument).GetNestedType("ReferenceHashBuffer", BindingFlags.NonPublic);
        MethodInfo write = bufferType.GetMethod("Write", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo final = bufferType.GetMethod("GetHash", BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (int count in new[] { 0, 1, 64, 65, 128, 129 })
        {
            object buffer = Activator.CreateInstance(bufferType);
            using (var bytes = new MemoryStream())
            using (var writer = new BinaryWriter(bytes))
            {
                for (int i = 0; i < count; i++)
                {
                    uint value = unchecked((uint)(i * 9187 + 153));
                    write.Invoke(buffer, new object[] { value }); writer.Write(value);
                }
                uint expected = count == 0 ? 0u : FastHash.GetHashCode(bytes.GetBuffer());
                Need((uint)final.Invoke(buffer, null) == expected, "Reference hash changed capacity growth at " + count);
            }
        }
        tests.Add("production-reference-buffer-zero-writes-and-original-capacity-growth-boundaries");
    }
    static void ReferenceCases()
    {
        string body = "<Strong> DerivedThing:One </Strong><ByElement>Two</ByElement><Box><![CDATA[ BaseThing:Three ]]><!--tail--></Box>" +
            "<Dynamic xsi:type='OtherStrong'>OtherThing:Four</Dynamic><Weak>Gone:Weak</Weak>";
        uint plainHash;
        InstanceDeclaration plain = References(Document("<Dynamic>BaseThing:Plain</Dynamic>"), out plainHash);
        Need(plain.ReferencedInstances[1].TypeName == "BaseThing", "Initial declaration metadata changed");
        for (int repeat = 0; repeat < 2; repeat++)
        {
            XmlDocument xml = Document(body, "AttributeStrong='BaseThing:Attribute'");
            uint hash; InstanceDeclaration instance = References(xml, out hash);
            Need(instance.ReferencedInstances.Count == 6 && instance.WeakReferencedInstances.Count == 1, "Strong/default/weak reference counts changed");
            string[] expected = { "BaseThing:Attribute", "BaseThing:Default", "DerivedThing:One", "BaseThing:Two", "BaseThing:Three", "OtherThing:Four" };
            for (int i = 0; i < expected.Length; i++) Need(instance.ReferencedInstances[i].Name == expected[i], "Reference order/type changed at " + i);
            Need(instance.WeakReferencedInstances[0].Name == "Gone:Weak", "Weak reference gained a strong-type check");
            Need(xml.DocumentElement.GetAttribute("AttributeStrong") == "BaseThing:Attribute\\0" &&
                xml.DocumentElement.GetAttribute("AttributeDefault") == "BaseThing:Default\\1", "Attribute reference indexes changed");
            string[] text = { "DerivedThing:One\\2", "Two\\3", "BaseThing:Three\\4", "OtherThing:Four\\5", "Gone:Weak" };
            for (int i = 0; i < text.Length; i++) Need(xml.DocumentElement.ChildNodes[i].InnerText == text[i], "Element reference indexes changed");
            Need(hash == ExpectedHash("BaseThing", "BaseThing", "BaseThing", "BaseThing", "BaseThing", "OtherThing", "BaseThing"),
                "Reference hash changed its original ordered writes or unused MemoryStream capacity");
            Need(xml.DocumentElement.ChildNodes[2].Attributes["Tag"].Value == "kept", "Reference rewrite dropped a schema default");
        }
        InstanceDeclaration plainAgain = References(Document("<Dynamic>BaseThing:Again</Dynamic><Missing> </Missing>"), out plainHash);
        Need(plainAgain.ReferencedInstances.Count == 2 && plainAgain.ReferencedInstances[1].TypeName == "BaseThing",
            "xsi:type reference metadata polluted the declared type, or empty references gained type checks");
        foreach (string bodyCase in new[] { "<Strong>OtherThing:Bad</Strong>", "<Strong>Gone:Bad</Strong>", "<Missing>Target</Missing>", "<UnderlyingMissing>DerivedThing:Bad</UnderlyingMissing>" })
        {
            bool rejected = false;
            try { uint hash; References(Document(bodyCase), out hash); }
            catch (BinaryAssetBuilderException error) { rejected = error.ErrorCode == ErrorCode.ReferencingError; }
            Need(rejected, "Reference rejection changed: " + bodyCase);
        }
        XmlDocument missing = Document("");
        XmlElement untyped = missing.CreateElement("Strong", Ns); untyped.InnerText = "BaseThing:One"; missing.DocumentElement.AppendChild(untyped);
        bool noType = false;
        try { uint hash; References(missing, out hash); }
        catch (BinaryAssetBuilderException error) { noType = error.ErrorCode == ErrorCode.InternalError; }
        Need(noType, "Untyped EA node was silently accepted by reference traversal");
        XmlDocument foreign = Document("<Strong>BaseThing:ThroughForeign</Strong>");
        XmlNode strong = foreign.DocumentElement.FirstChild;
        XmlElement wrapper = foreign.CreateElement("foreign", "Wrapper", "urn:foreign");
        wrapper.AppendChild(strong); foreign.DocumentElement.AppendChild(wrapper);
        uint foreignHash; InstanceDeclaration crossed = References(foreign, out foreignHash);
        Need(crossed.ReferencedInstances.Count == 2 && crossed.ReferencedInstances[1].Name == "BaseThing:ThroughForeign", "Traversal did not cross a non-EA ancestor");
        tests.Add("production-reference-traversal-strong-derived-explicit-inferred-weak-default-attribute-xsi-type-cache-ordered-hash-rejections-foreign-ancestor");
    }
    public static int Main(string[] args)
    {
        object result; int code = 0; Settings previous = Settings.Current;
        try
        {
            Need(args.Length == 5 && IntPtr.Size == 4, "Expected work result sourceCommit runId SDK in x86 validation host");
            Directory.CreateDirectory(args[0]);
            string path = Path.Combine(args[0], "reference.xsd"); File.WriteAllText(path, Schema(), new UTF8Encoding(false));
            Settings.Current = new Settings { SchemaPath = path, StringHashBinDescriptors = new StringHashBinDescriptor[0], BuildCache = false };
            schemas = new SchemaSet(false); processor = new DocumentProcessor(Settings.Current, null, null) { SchemaSet = schemas };
            DomEquivalence(); ReferenceHashCapacity(); ReferenceCases();
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), scope = "CLR PSVI and production Core reference semantics; complete SDK remains a separate gate" };
            Console.WriteLine("PASS ReferenceSemantics: " + checks + " assertions");
        }
        catch (Exception error)
        {
            code = 1; result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null, runId = args.Length > 3 ? args[3] : null, checks, tests = tests.ToArray(), error = error.ToString() };
            Console.Error.WriteLine(error);
        }
        finally { Settings.Current = previous; }
        if (args.Length > 1) File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(result));
        return code;
    }
}
