using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder.Core;

// Compile this exact source independently against the original SDK Core and the
// recovered Core. The probe exercises public lifecycle calls; reflection only
// supplies a tiny unprocessed document without requiring the application host.
public static class CompilerDocumentSnapshotOracle
{
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    static AssetDeclarationDocument Document(string work)
    {
        var document = new AssetDeclarationDocument();
        var type = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object current = Activator.CreateInstance(type, true);
        type.GetMethod("FromScratch").Invoke(current, new object[0]);
        type.GetField("SourcePath").SetValue(current, Path.Combine(work, "oracle.xml"));
        type.GetField("SourceDirectory").SetValue(current, work);
        typeof(AssetDeclarationDocument).GetField("_Current", Hidden).SetValue(document, current);
        var xml = new XmlDocument(); xml.LoadXml("<ArmorTemplate id='Owner'/>");
        document.SelfInstances.Add(new InstanceDeclaration(document) { XmlNode = xml.DocumentElement, ProcessingHash = 29 });
        return document;
    }
    static string Xml(AssetDeclarationDocument document)
    {
        var text = new StringBuilder();
        using (var writer = XmlWriter.Create(text, new XmlWriterSettings { OmitXmlDeclaration = true })) document.WriteXml(writer);
        return text.ToString();
    }
    static object Metadata(string work, bool expectedShared)
    {
        var source = Document(work);
        source.SelfDefines.Add(new Definition { Name = "Armor", OriginalValue = "=1+2", EvaluatedValue = "3", Document = source });
        source.InclusionItems.Add(new InclusionItem("DATA:child.xml", "child.xml", InclusionType.All) { Document = source });
        source.UsedDefines["Armor"] = "3";
        var output = new OutputAsset { Handle = new InstanceHandle("ArmorTemplate", "StoredOwner"), InstanceFileSize = 17 };
        object current = typeof(AssetDeclarationDocument).GetField("_Current", Hidden).GetValue(source);
        current.GetType().GetField("OutputAssets").SetValue(current, new List2<OutputAsset> { output });
        source.MakeCacheable(); string before = Xml(source);
        var receiver = new AssetDeclarationDocument(); receiver.CacheFromDocument(source); receiver.FromLastHack();
        bool sameDefinition = Object.ReferenceEquals(source.SelfDefines[0], receiver.SelfDefines[0]);
        bool definitionSourceRebound = Object.ReferenceEquals(source.SelfDefines[0].Document, receiver);
        bool sameInclusion = Object.ReferenceEquals(source.InclusionItems[0], receiver.InclusionItems[0]);
        bool sameOutput = Object.ReferenceEquals(output, receiver.LastOutputAssets[0]);
        bool sameOutputHandle = Object.ReferenceEquals(output.Handle, receiver.LastOutputAssets[0].Handle);
        receiver.SelfDefines[0].EvaluatedValue = "changed";
        receiver.InclusionItems[0].PhysicalPath = "changed-path";
        receiver.LastOutputAssets[0].InstanceFileSize = 97;
        receiver.LastOutputAssets[0].Handle.TypeHash = 101;
        bool sourceXmlChanged = before != Xml(source);
        string sourceBeforeUsedMutation = Xml(source), receiverBeforeUsedMutation = Xml(receiver);
        receiver.UsedDefines["Armor"] = "changed-used";
        bool usedValuesIndependent = source.UsedDefines["Armor"] == "3" &&
            Xml(source) == sourceBeforeUsedMutation && Xml(receiver) == receiverBeforeUsedMutation;
        if (sameDefinition != expectedShared || definitionSourceRebound != expectedShared || sameInclusion != expectedShared ||
            sameOutput != expectedShared || sameOutputHandle != expectedShared || sourceXmlChanged != expectedShared || !usedValuesIndependent)
            throw new Exception("Unexpected public document metadata ownership outcome");
        return new { sameDefinition, definitionSourceRebound, sameInclusion, sameOutput, sameOutputHandle, sourceXmlChanged, usedValuesIndependent,
            scope = "Independent original public Core cache-copy/restore metadata records; reflection supplies Current output fixture only; no production CLI fault claimed" };
    }
    sealed class ChangingText : XmlText
    {
        internal int Reads;
        internal ChangingText(XmlDocument owner) : base("seed", owner) { }
        public override string Value { get { return ++Reads == 1 ? "plain" : Reads == 2 ? "=1+2" : "=3+4"; } set { base.Value = value; } }
    }
    sealed class CountingElement : XmlElement
    {
        internal int ValueReads, ChildLists;
        internal CountingElement(XmlDocument owner) : base("", "Custom", "", owner) { }
        public override string Value { get { ValueReads++; return base.Value; } set { base.Value = value; } }
        public override XmlNodeList ChildNodes { get { ChildLists++; return base.ChildNodes; } }
    }
    static object[] ExpressionTraversal(string work)
    {
        ExpressionEvaluatorWrapper.LoadAssembly();
        var results = new List<object>();
        string[] names = { "mixed", "entities", "attribute-error", "text-error", "custom-attribute", "remove-current", "append-sibling", "move-current", "remove-next", "custom-element" };
        string[] inputs = {
            "<ArmorTemplate id='Tree' Empty='' Literal='x' Sum='=$I+2'><Part Label='=$S+&quot;!&quot;'>=1+2</Part><![CDATA[=$I*2]]><!--=4+5--><?calc =3+4?><Blank>  </Blank></ArmorTemplate>",
            "<!DOCTYPE ArmorTemplate [<!ENTITY plus '1+2'>]><ArmorTemplate id='Tree' Split='=&plus;'><Part>=2+3</Part></ArmorTemplate>",
            "<ArmorTemplate id='Tree' First='=1+2' Broken='=' Later='=9'/>",
            "<ArmorTemplate id='Tree' First='=$I'><Part>=$MISSING</Part><Later Value='=9'/></ArmorTemplate>",
            "<ArmorTemplate id='Tree'/>",
            "<ArmorTemplate id='Tree'><Part Trigger='=1+2'/><Tail Value='=4+5'/></ArmorTemplate>",
            "<ArmorTemplate id='Tree'><Part Trigger='=1+2'/><Tail Value='=4+5'/></ArmorTemplate>",
            "<ArmorTemplate id='Tree'><Part Trigger='=1+2'/><Tail Value='=4+5'/></ArmorTemplate>",
            "<ArmorTemplate id='Tree'><Part Trigger='=1+2'/><Tail Value='=4+5'/></ArmorTemplate>",
            "<ArmorTemplate id='Tree'/>"
        };
        foreach (string culture in new[] { "en-US", "de-DE", "tr-TR", "zh-CN" })
        for (int index = 0; index < names.Length; index++)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);
            string name = names[index]; var document = Document(work); document.SelfInstances.Clear();
            object current = typeof(AssetDeclarationDocument).GetField("_Current", Hidden).GetValue(document);
            current.GetType().GetField("SourcePath").SetValue(current, "expression-tree.xml");
            foreach (string key in new[] { "I", "S" }) document.AllDefines[key] = new Definition { Name = key, EvaluatedValue = key == "I" ? "7" : "hello", Document = document };
            var xml = new XmlDocument { PreserveWhitespace = true }; xml.LoadXml(inputs[index]); ChangingText custom = null; CountingElement customElement = null;
            if (name == "custom-attribute") { var attribute = xml.CreateAttribute("Dynamic"); custom = new ChangingText(xml); attribute.AppendChild(custom); xml.DocumentElement.Attributes.Append(attribute); }
            if (name == "custom-element") { customElement = new CountingElement(xml); customElement.SetAttribute("Value", "=$I+2"); customElement.AppendChild(xml.CreateCDataSection("=3+4")); xml.DocumentElement.AppendChild(customElement); }
            document.SelfInstances.Add(new InstanceDeclaration(document) { XmlNode = xml.DocumentElement });
            bool mutated = false;
            xml.NodeInserted += (sender, change) => {
                if (mutated || change.NewParent == null || change.NewParent.NodeType != XmlNodeType.Attribute || change.NewParent.Name != "Trigger") return;
                mutated = true; XmlNode part = xml.DocumentElement.FirstChild, tail = part.NextSibling;
                if (name == "remove-current") xml.DocumentElement.RemoveChild(part);
                if (name == "move-current") xml.DocumentElement.AppendChild(part);
                if (name == "remove-next") xml.DocumentElement.RemoveChild(tail);
                if (name == "append-sibling") { var added = xml.CreateElement("Added"); added.SetAttribute("Value", "=6+7"); xml.DocumentElement.AppendChild(added); }
            };
            string outcome = "OK"; var prior = Console.Error; var diagnostics = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);
            try { Console.SetError(diagnostics); try { document.ProcessExpressions(); } catch (Exception error) {
                var code = error.GetType().GetProperty("ErrorCode"); outcome = "ERR:" + error.GetType().FullName + ":" + (code == null ? "" : Convert.ToString(code.GetValue(error, null), System.Globalization.CultureInfo.InvariantCulture)) + ":" + error.Message;
            } } finally { Console.SetError(prior); }
            int customReads = custom == null ? 0 : custom.Reads;
            int customElementReads = customElement == null ? 0 : customElement.ValueReads, customChildLists = customElement == null ? 0 : customElement.ChildLists;
            var used = new List<string>(); foreach (var pair in document.UsedDefines) used.Add(pair.Key + "=" + pair.Value);
            results.Add(new { name, culture, inputXml = inputs[index], outcome, resultXml = xml.OuterXml, customReads, customElementReads, customChildLists, mutated, used = String.Join(";", used), diagnostics = diagnostics.ToString() });
        }
        return results.ToArray();
    }
    public static int Main(string[] args)
    {
        var json = new JavaScriptSerializer(); object result; int code = 0;
        try
        {
            if (args.Length != 5 || IntPtr.Size != 4) throw new Exception("Expected x86 work result source run original|fallback|owned");
            string mode = args[4];
            if (mode != "original" && mode != "fallback" && mode != "owned") throw new Exception("Unknown oracle mode");
            AppContext.SetSwitch("NewSdk.Compiler.DisableDocumentSnapshots", mode == "fallback");
            AppContext.SetSwitch("NewSdk.Compiler.DisableDeclarationHandleSnapshots", false);
            Directory.CreateDirectory(args[0]);
            var source = Document(args[0]); source.MakeCacheable(); string before = Xml(source);
            var receiver = new AssetDeclarationDocument(); receiver.CacheFromDocument(source); receiver.FromLastHack();
            bool sameDeclaration = Object.ReferenceEquals(source.SelfInstances[0], receiver.SelfInstances[0]);
            uint savedHash = source.SelfInstances[0].ProcessingHash;
            receiver.MakeComplete(); receiver.SelfInstances[0].ProcessingHash = 47; receiver.MakeCacheable();
            bool sourceXmlChanged = before != Xml(source);
            bool expectedShared = mode != "owned";
            if (sameDeclaration != expectedShared || sourceXmlChanged != expectedShared || savedHash != 29 || receiver.SelfInstances[0].ProcessingHash != 47)
                throw new Exception("Unexpected public document cache lifecycle outcome for " + mode);
            object metadata = Metadata(args[0], expectedShared);
            object[] expressionTraversal = ExpressionTraversal(args[0]);
            string assemblySha;
            using (var sha = SHA256.Create()) assemblySha = Convert.ToBase64String(sha.ComputeHash(File.ReadAllBytes(typeof(AssetDeclarationDocument).Assembly.Location)));
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], mode, sameDeclaration, sourceXmlChanged,
                savedHash, receiverHash = receiver.SelfInstances[0].ProcessingHash, metadata, expressionTraversal,
                assembly = typeof(AssetDeclarationDocument).Assembly.FullName, assemblySha256Base64 = assemblySha,
                scope = "Direct public Core cache-copy/restore/complete/reserialize lifecycle; no production CLI failure is claimed" };
            Console.WriteLine("PASS DocumentSnapshotOracle: " + mode);
        }
        catch (Exception error)
        {
            code = 1; Console.Error.WriteLine(error);
            result = new { succeeded = false, mode = args.Length > 4 ? args[4] : null, error = error.ToString() };
        }
        if (args.Length > 1) File.WriteAllText(args[1], json.Serialize(result));
        return code;
    }
}
