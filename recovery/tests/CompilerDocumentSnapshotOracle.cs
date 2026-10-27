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
using BinaryAssetBuilder;

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
            XmlNodeChangedEventHandler mutation = (sender, change) => {
                XmlNode parent = change.Node.NodeType == XmlNodeType.Attribute ? change.Node : (change.NewParent ?? change.Node.ParentNode);
                if (mutated || parent == null || parent.NodeType != XmlNodeType.Attribute || parent.Name != "Trigger") return;
                mutated = true; XmlNode part = xml.DocumentElement.FirstChild, tail = part.NextSibling;
                if (name == "remove-current") xml.DocumentElement.RemoveChild(part);
                if (name == "move-current") xml.DocumentElement.AppendChild(part);
                if (name == "remove-next") xml.DocumentElement.RemoveChild(tail);
                if (name == "append-sibling") { var added = xml.CreateElement("Added"); added.SetAttribute("Value", "=6+7"); xml.DocumentElement.AppendChild(added); }
            };
            xml.NodeInserted += mutation; xml.NodeChanged += mutation;
            string outcome = "OK"; var prior = Console.Error; var diagnostics = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);
            try { Console.SetError(diagnostics); try { document.ProcessExpressions(); } catch (Exception error) {
                var code = error.GetType().GetProperty("ErrorCode"); outcome = "ERR:" + error.GetType().FullName + ":" + (code == null ? "" : Convert.ToString(code.GetValue(error, null), System.Globalization.CultureInfo.InvariantCulture)) + ":" + error.Message;
            } } finally { Console.SetError(prior); }
            if ((name == "remove-current" || name == "append-sibling" || name == "move-current" || name == "remove-next") && !mutated)
                throw new Exception("Traversal mutation fixture never reached Trigger: " + culture + "/" + name);
            int customReads = custom == null ? 0 : custom.Reads;
            int customElementReads = customElement == null ? 0 : customElement.ValueReads, customChildLists = customElement == null ? 0 : customElement.ChildLists;
            var used = new List<string>(); foreach (var pair in document.UsedDefines) used.Add(pair.Key + "=" + pair.Value);
            results.Add(new { name, culture, inputXml = inputs[index], outcome, resultXml = xml.OuterXml, customReads, customElementReads, customChildLists, mutated, used = String.Join(";", used), diagnostics = diagnostics.ToString() });
        }
        return results.ToArray();
    }
    const string AssetNamespace = "uri:ea.com:eala:asset";
    static AssetDeclarationDocument InheritanceDocument(DocumentProcessor processor, string name, string body)
    {
        var document = Document(Path.GetTempPath()); document.SelfInstances.Clear();
        object current = typeof(AssetDeclarationDocument).GetField("_Current", Hidden).GetValue(document);
        Type type = current.GetType();
        // Stable logical sources keep independent original/recovered processes
        // comparable without erasing provider identity or error ordering.
        type.GetField("SourcePath").SetValue(current, name + ".xml");
        type.GetField("LogicalSourcePath").SetValue(current, name + ".xml");
        type.GetField("DocumentProcessor").SetValue(current, processor);
        var xml = new XmlDocument(); xml.LoadXml("<AssetDeclaration xmlns='" + AssetNamespace + "'>" + body + "</AssetDeclaration>");
        type.GetField("XmlDocument").SetValue(current, xml);
        type.GetField("NodeSourceInfoSet").SetValue(current, Activator.CreateInstance(type.GetField("NodeSourceInfoSet").FieldType));
        foreach (XmlNode node in xml.DocumentElement.ChildNodes)
            document.SelfInstances.Add(new InstanceDeclaration(document) { XmlNode = node });
        return document;
    }
    static object[] InheritanceProjection(AssetDeclarationDocument document)
    {
        var rows = new List<object>();
        foreach (InstanceDeclaration instance in document.SelfInstances)
        {
            XmlNode node = instance.XmlNode; var schema = node == null ? null : node.SchemaInfo;
            XmlAttribute defaults = node == null ? null : node.Attributes["Default"];
            rows.Add(new { id = instance.Handle.InstanceName, xml = node == null ? null : node.OuterXml,
                prevalidationHash = instance.PrevalidationXmlHash, inheritedHash = instance.InheritFromXmlHash,
                schemaType = schema == null || schema.SchemaType == null ? null : schema.SchemaType.QualifiedName.ToString(),
                validity = schema == null ? null : schema.Validity.ToString(),
                defaultValue = defaults == null ? null : defaults.Value,
                defaultSpecified = defaults == null ? (bool?)null : defaults.Specified,
                providerDocument = instance.Document == null ? null : instance.Document.SourcePath });
        }
        return rows.ToArray();
    }
    public static object[] InheritanceCases(string work, string sdk)
    {
        Settings previous = Settings.Current;
        var priorCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
        try
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            var config = new XmlDocument(); config.Load(Path.Combine(sdk, "tools/BinaryAssetBuilder.exe.config"));
            var settings = (Settings)new SerializedSectionHandler().Create(null, null, config.SelectSingleNode("/configuration/assetbuilder"));
            settings.SchemaPath = Path.Combine(sdk, "Schemas/xsd/CnC3Types.xsd"); settings.TargetPlatform = TargetPlatform.Win32;
            settings.DataRoot = work; settings.SessionCacheDirectory = Path.Combine(work, "inheritance-hashes");
            settings.OutputIntermediateXml = false; Settings.Current = settings;
            Directory.CreateDirectory(settings.SessionCacheDirectory); HashProvider.InitializeStringHashes(settings.SessionCacheDirectory);
            var schemas = new SchemaSet(false);
            var processor = new DocumentProcessor(settings, new PluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32), null) { SchemaSet = schemas };
            string[] names = { "forward", "backward", "diamond", "first-depth-error", "first-sibling-error" };
            string[] inputs = {
                "<ArmorTemplate id='First' inheritFrom='Middle'/><ArmorTemplate id='Middle' inheritFrom='Last'/><ArmorTemplate id='Last' Default='10'/>",
                "<ArmorTemplate id='Last' Default='10'/><ArmorTemplate id='Middle' inheritFrom='Last'/><ArmorTemplate id='First' inheritFrom='Middle'/>",
                "<ArmorTemplate id='Left' inheritFrom='Root'/><ArmorTemplate id='Right' inheritFrom='Root'/><ArmorTemplate id='Leaf' inheritFrom='Left'/><ArmorTemplate id='Root' Default='10'/>",
                "<ArmorTemplate id='First' inheritFrom='Middle'/><ArmorTemplate id='Middle' inheritFrom='MissingDeep'/><ArmorTemplate id='Later' inheritFrom='MissingSibling'/>",
                "<ArmorTemplate id='First' inheritFrom='MissingSibling'/><ArmorTemplate id='Later' inheritFrom='MissingDeep'/>"
            };
            var results = new List<object>();
            for (int index = 0; index < names.Length; index++)
            {
                var document = InheritanceDocument(processor, "inheritance-" + names[index], inputs[index]);
                var events = new List<string>();
                document.XmlDocument.NodeInserted += (sender, change) => {
                    if (change.NewParent == document.XmlDocument.DocumentElement && change.Node.Attributes != null && change.Node.Attributes["id"] != null)
                        events.Add(change.Node.Attributes["id"].Value);
                };
                int passes = index < 3 ? 2 : 1;
                for (int pass = 0; pass < passes; pass++)
                {
                    events.Clear(); string outcome = "OK";
                    try { document.ProcessOverrides(); }
                    catch (BinaryAssetBuilderException error) { outcome = "ERR:" + error.ErrorCode + ":" + error.Message; }
                    results.Add(new { name = names[index], pass, inputXml = inputs[index], outcome,
                        documentXml = document.XmlDocument.OuterXml, events = String.Join(",", events), instances = InheritanceProjection(document) });
                }
            }
            // Actual public Validate produces default attributes, SchemaInfo,
            // TypeId and legacy per-instance metadata in the original provider.
            var provider = InheritanceDocument(processor, "inheritance-processed-provider", "<ArmorTemplate id='Provider'/>");
            provider.ProcessOverrides(); provider.Validate();
            object[] providerBefore = InheritanceProjection(provider);
            var consumer = InheritanceDocument(processor, "inheritance-processed-consumer", "<ArmorTemplate id='Consumer' inheritFrom='Provider'/>");
            consumer.TentativeInstances.Add(provider.SelfInstances[0]);
            consumer.InclusionItems.Add(new InclusionItem("provider.xml", "provider.xml", InclusionType.Instance) { Document = provider });
            consumer.ProcessOverrides();
            object[] merged = InheritanceProjection(consumer);
            consumer.XmlDocument.Schemas.Add(schemas.Schemas);
            consumer.XmlDocument.Validate((sender, error) => { throw error.Exception; });
            results.Add(new { name = "included-processed", pass = 0, providerBefore,
                providerAfter = InheritanceProjection(provider), merged, validated = InheritanceProjection(consumer),
                providerType = provider.SelfInstances[0].XmlNode.SchemaInfo.SchemaType.QualifiedName.ToString(),
                providerDocument = provider.SelfInstances[0].Document.SourcePath,
                providerStillTyped = provider.SelfInstances[0].XmlNode.SchemaInfo.SchemaType != null });
            return results.ToArray();
        }
        finally { Settings.Current = previous; System.Threading.Thread.CurrentThread.CurrentCulture = priorCulture; }
    }
    sealed class DerivedDefinitions : DefinitionSet { }
    static object[] defineLookupMetrics;
    public static object[] GetDefineLookupCaseMetrics() { return defineLookupMetrics; }
    static DefinitionSet OrdinalDefinitions()
    {
        // Fixture-only base-state construction: DefinitionSet exposes no custom
        // comparer constructor. Preserve an actual nondefault SortedDictionary
        // comparer without adding any production API for this test.
        var result = new DefinitionSet(); var source = new SortedDictionary<string, Definition>(StringComparer.Ordinal);
        foreach (FieldInfo field in typeof(SortedDictionary<string, Definition>).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            field.SetValue(result, field.GetValue(source));
        if (!Object.ReferenceEquals(result.Comparer, StringComparer.Ordinal)) throw new Exception("Nondefault comparer fixture was not installed");
        return result;
    }
    public static object[] DefineLookupCases(string work)
    {
        ExpressionEvaluatorWrapper.LoadAssembly();
        var previousCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
        var results = new List<object>(); var metrics = new List<object>();
        string[] unicode = { "I", "i", "İ", "ı", "é", "e\u0301", "ab", "a\u00adb", "a\0b", "Å", "A\u030a" };
        try
        {
            foreach (string culture in new[] { "en-US", "de-DE", "tr-TR", "zh-CN" })
            foreach (string mode in new[] { "stable", "value-field", "replace", "remove-readd", "base-indexer", "culture-change", "reenter", "failure", "derived", "comparer", "current-rebind", "table-rebind", "prebuilt-culture" })
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(mode == "prebuilt-culture" ? "en-US" : culture);
                var document = Document(work); document.SelfInstances.Clear();
                object current = typeof(AssetDeclarationDocument).GetField("_Current", Hidden).GetValue(document);
                current.GetType().GetField("SourcePath").SetValue(current, "define-lookup-root.xml");
                var imported = Document(work);
                object importedCurrent = typeof(AssetDeclarationDocument).GetField("_Current", Hidden).GetValue(imported);
                importedCurrent.GetType().GetField("SourcePath").SetValue(importedCurrent, "define-lookup-provider.xml");
                if (mode == "derived") current.GetType().GetField("AllDefines").SetValue(current, new DerivedDefinitions());
                if (mode == "comparer") current.GetType().GetField("AllDefines").SetValue(current, OrdinalDefinitions());
                for (int i = 0; i < 128; i++) document.AllDefines["D" + i.ToString("D3")] = new Definition { Name = "D" + i.ToString("D3"), EvaluatedValue = "1", Document = imported };
                document.AllDefines["Target"] = new Definition { Name = "Target", EvaluatedValue = "1", Document = imported };
                for (int i = 0; i < unicode.Length; i++) document.AllDefines[unicode[i]] = new Definition { Name = unicode[i], EvaluatedValue = (100 + i).ToString(System.Globalization.CultureInfo.InvariantCulture), Document = imported };
                if (mode == "prebuilt-culture") System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("tr-TR");
                bool ordered = true, first = true; string previous = null;
                foreach (var pair in document.AllDefines) { if (!first && document.AllDefines.Comparer.Compare(previous, pair.Key) >= 0) ordered = false; first = false; previous = pair.Key; }
                var xml = new XmlDocument(); xml.LoadXml("<ArmorTemplate id='Lookup'/>");
                for (int i = 0; i < 192; i++) xml.DocumentElement.SetAttribute("P" + i.ToString("D3"), "=$Target");
                document.SelfInstances.Add(new InstanceDeclaration(document) { XmlNode = xml.DocumentElement });
                bool changed = false; var direct = new List<string>();
                XmlNodeChangedEventHandler mutation = (sender, change) => {
                    XmlNode parent = change.Node.NodeType == XmlNodeType.Attribute ? change.Node : (change.NewParent ?? change.Node.ParentNode);
                    if (changed || parent == null || parent.NodeType != XmlNodeType.Attribute || parent.Name != "P180") return;
                    changed = true;
                    var replacement = new Definition { Name = "Target", EvaluatedValue = "2", Document = imported };
                    if (mode == "value-field") document.AllDefines["Target"].EvaluatedValue = "2";
                    if (mode == "replace") document.AllDefines["Target"] = replacement;
                    if (mode == "remove-readd") { document.AllDefines.Remove("Target"); document.AllDefines.Add("Target", replacement); }
                    if (mode == "base-indexer") ((SortedDictionary<string, Definition>)document.AllDefines)["Target"] = replacement;
                    if (mode == "culture-change") System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("tr-TR");
                    if (mode == "reenter") document.ProcessExpressions();
                    if (mode == "failure") document.AllDefines.Remove("Target");
                    if (mode == "current-rebind")
                    {
                        object clone = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(current, null);
                        typeof(AssetDeclarationDocument).GetField("_Current", Hidden).SetValue(document, clone);
                    }
                    if (mode == "table-rebind")
                    {
                        var table = new DefinitionSet(); foreach (var pair in document.AllDefines) table.Add(pair.Key, pair.Value);
                        table["Target"] = replacement; current.GetType().GetField("AllDefines").SetValue(current, table);
                    }
                    var evaluator = new BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator(document);
                    foreach (string key in unicode)
                    {
                        try { direct.Add(key + "=" + evaluator.LookupDefine("$" + key)); }
                        catch (BinaryAssetBuilderException error) { direct.Add(key + "=ERR:" + error.ErrorCode + ":" + error.Message); }
                    }
                };
                xml.NodeInserted += mutation;
                xml.NodeChanged += mutation;
                string outcome = "OK";
                try { document.ProcessExpressions(); }
                catch (BinaryAssetBuilderException error) { outcome = "ERR:" + error.ErrorCode + ":" + error.Message; }
                if (!changed) throw new Exception("Define mutation fixture never reached P180: " + culture + "/" + mode);
                var used = new List<string>(); foreach (var pair in document.UsedDefines) used.Add(pair.Key + "=" + pair.Value);
                results.Add(new { culture, mode, ordered, outcome, xml = xml.OuterXml, changed,
                    direct = direct.ToArray(), used = used.ToArray(), target = document.AllDefines.GetEvaluatedValue("Target") });
                MethodInfo getMetrics = typeof(AssetDeclarationDocument).GetMethod("GetDefineLookupMetrics");
                metrics.Add(new { culture, mode, work = getMetrics == null ? null : getMetrics.Invoke(document, null) });
            }
            defineLookupMetrics = metrics.ToArray(); return results.ToArray();
        }
        finally { System.Threading.Thread.CurrentThread.CurrentCulture = previousCulture; }
    }
    static void SemanticDomProjection(XmlNode node, List<object> result)
    {
        var info = node.SchemaInfo; XmlAttribute attribute = node as XmlAttribute;
        result.Add(new { kind = node.NodeType.ToString(), name = node.Name, prefix = node.Prefix, ns = node.NamespaceURI,
            value = node.Value, baseUri = node.BaseURI, specified = attribute == null ? (bool?)null : attribute.Specified,
            schemaType = info == null || info.SchemaType == null ? null : info.SchemaType.QualifiedName.ToString(),
            schemaSource = info == null || info.SchemaType == null ? null : info.SchemaType.SourceUri,
            memberType = info == null || info.MemberType == null ? null : info.MemberType.QualifiedName.ToString(),
            schemaElement = info == null || info.SchemaElement == null ? null : info.SchemaElement.QualifiedName.ToString(),
            schemaAttribute = info == null || info.SchemaAttribute == null ? null : info.SchemaAttribute.QualifiedName.ToString(),
            validity = info == null ? null : info.Validity.ToString(),
            isDefault = info == null ? (bool?)null : info.IsDefault, isNil = info == null ? (bool?)null : info.IsNil });
        if (node.Attributes != null) foreach (XmlAttribute item in node.Attributes) SemanticDomProjection(item, result);
        foreach (XmlNode child in node.ChildNodes) SemanticDomProjection(child, result);
    }
    public static object[] SemanticDomRestoreCases(string work, string sdk)
    {
        Settings previous = Settings.Current;
        var priorCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
        try
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            var config = new XmlDocument(); config.Load(Path.Combine(sdk, "tools/BinaryAssetBuilder.exe.config"));
            var settings = (Settings)new SerializedSectionHandler().Create(null, null, config.SelectSingleNode("/configuration/assetbuilder"));
            settings.SchemaPath = Path.Combine(sdk, "Schemas/xsd/CnC3Types.xsd"); settings.TargetPlatform = TargetPlatform.Win32;
            settings.DataRoot = work; settings.SessionCacheDirectory = Path.Combine(work, "semantic-restore-hashes");
            settings.OutputIntermediateXml = false; Settings.Current = settings;
            Directory.CreateDirectory(settings.SessionCacheDirectory); HashProvider.InitializeStringHashes(settings.SessionCacheDirectory);
            var schemas = new SchemaSet(false);
            var processor = new DocumentProcessor(settings, new PluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32), null) { SchemaSet = schemas };
            var results = new List<object>();
            foreach (string mode in new[] { "validated-Core-DOM", "processed-Core-DOM", "Framework-prefix-entity" })
            {
                string uri = "file:///semantic-restore/" + mode + ".xml";
                XmlDocument source; XmlElement target;
                if (mode == "Framework-prefix-entity")
                {
                    source = new XmlDocument { PreserveWhitespace = true, XmlResolver = null };
                    string xml = "<!DOCTYPE p:Root [<!ENTITY word 'entity-value'>]><p:Root xmlns:p='urn:semantic-restore' xmlns:q='urn:semantic-aux' q:note='kept'> <p:Child>&word;</p:Child> </p:Root>";
                    using (var reader = new XmlTextReader(uri, new StringReader(xml)))
                    { reader.DtdProcessing = DtdProcessing.Parse; reader.EntityHandling = EntityHandling.ExpandCharEntities; reader.XmlResolver = null; source.Load(reader); }
                    string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='urn:semantic-restore' elementFormDefault='qualified'><xs:element name='Root'><xs:complexType><xs:sequence><xs:element name='Child' type='xs:string'/></xs:sequence><xs:attribute name='Default' type='xs:string' default='default-value'/><xs:anyAttribute processContents='skip'/></xs:complexType></xs:element></xs:schema>";
                    using (var reader = XmlReader.Create(new StringReader(xsd), new XmlReaderSettings(), "file:///semantic-restore/schema.xsd")) source.Schemas.Add(null, reader);
                    source.Validate((sender, error) => { throw error.Exception; }); target = source.DocumentElement;
                }
                else
                {
                    var document = InheritanceDocument(processor, "semantic-restore-provider", "<ArmorTemplate id='Provider'/>");
                    source = document.XmlDocument; string xml = source.OuterXml;
                    using (var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings(), uri)) source.Load(reader);
                    document.SelfInstances.Clear();
                    foreach (XmlNode child in source.DocumentElement.ChildNodes) document.SelfInstances.Add(new InstanceDeclaration(document) { XmlNode = child });
                    document.ProcessOverrides();
                    if (mode == "processed-Core-DOM") document.Validate();
                    else { source.Schemas.Add(schemas.Schemas); source.Validate((sender, error) => { throw error.Exception; }); }
                    target = (XmlElement)source.DocumentElement.FirstChild;
                }
                var sourceProjection = new List<object>(); SemanticDomProjection(source, sourceProjection);
                var restored = new XmlDocument { PreserveWhitespace = source.PreserveWhitespace, XmlResolver = null };
                using (var reader = new XmlNodeReader(source)) restored.Load(reader);
                XmlElement restoredTarget = mode == "Framework-prefix-entity" ? restored.DocumentElement : (XmlElement)restored.DocumentElement.FirstChild;
                var restoredProjection = new List<object>(); SemanticDomProjection(restored, restoredProjection);
                string sourceXml = source.OuterXml, restoredXml = restored.OuterXml;
                XmlNode sourceTarget = target, legacyClone = target.CloneNode(true);
                var cloneProjection = new List<object>(); SemanticDomProjection(legacyClone, cloneProjection);
                string sourceTargetXml = target.OuterXml, cloneXml = legacyClone.OuterXml;
                uint sourceTargetHash = HashProvider.GetXmlHash(ref sourceTarget), cloneHash = HashProvider.GetXmlHash(ref legacyClone);
                bool schemaTypeAliases = Object.ReferenceEquals(target.SchemaInfo.SchemaType, restoredTarget.SchemaInfo.SchemaType);
                bool schemaElementAliases = Object.ReferenceEquals(target.SchemaInfo.SchemaElement, restoredTarget.SchemaInfo.SchemaElement);
                XmlSchemaComplexType mutableType = target.SchemaInfo.SchemaType as XmlSchemaComplexType;
                bool observedSchemaMutation = false;
                if (mutableType != null)
                {
                    bool old = mutableType.IsMixed;
                    try { mutableType.IsMixed = !old; observedSchemaMutation = ((XmlSchemaComplexType)restoredTarget.SchemaInfo.SchemaType).IsMixed == !old; }
                    finally { mutableType.IsMixed = old; }
                }
                target.SetAttribute("probe-source-edit", "source"); bool sourceEditIsolated = restored.OuterXml == restoredXml;
                string sourceAfterEdit = source.OuterXml;
                restoredTarget.SetAttribute("probe-copy-edit", "copy"); bool copyEditIsolated = source.OuterXml == sourceAfterEdit;
                results.Add(new { mode, expectedBaseUri = uri, sourceBaseUri = source.BaseURI, restoredBaseUri = restored.BaseURI,
                    sourceProjection = sourceProjection.ToArray(), restoredProjection = restoredProjection.ToArray(), sourceXml, restoredXml,
                    cloneProjection = cloneProjection.ToArray(), sourceTargetXml, cloneXml, sourceTargetHash, cloneHash,
                    sameXml = sourceXml == restoredXml, schemaTypeAliases, schemaElementAliases, observedSchemaMutation,
                    sourceSchemaCount = source.Schemas.Count, restoredSchemaCount = restored.Schemas.Count,
                    sourceEditIsolated, copyEditIsolated,
                    scope = "Actual Framework XmlNodeReader to XmlDocument.Load; preserves a typed DOM projection but shares mutable schema references and does not establish saved-result reuse or immutable ASR" });
            }
            return results.ToArray();
        }
        finally { Settings.Current = previous; System.Threading.Thread.CurrentThread.CurrentCulture = priorCulture; }
    }
    public static object[] InputReaderCases(string work)
    {
        string directory = Path.Combine(work, "input-reader-corpus"); Directory.CreateDirectory(directory);
        var rows = new List<object>();
        foreach (int length in new[] { 0, 1, 1048575, 1048576, 1048607, 2097183 })
        {
            string path = Path.Combine(directory, "reader-" + length + ".bin");
            byte[] bytes = new byte[length];
            for (int index = 0; index < bytes.Length; index++) bytes[index] = (byte)((index * 19 + 7) % 251);
            File.WriteAllBytes(path, bytes);
            string sha256;
            using (var algorithm = SHA256.Create()) sha256 = Convert.ToBase64String(algorithm.ComputeHash(bytes));
            // Use only the original public API and complete its natural EOF.
            // Dispose is deliberately absent: it did not exist in the SDK API.
            var reader = new AsynchronousFileReader(path);
            uint initialSize = reader.FileSize; bool initiallyNull = reader.CurrentChunk == null;
            var chunks = new List<object>(); Chunk first = null;
            bool reusedChunk = true; long delivered = 0;
            while (reader.BeginRead())
            {
                Chunk chunk = reader.CurrentChunk;
                if (first == null) first = chunk; else reusedChunk &= Object.ReferenceEquals(first, chunk);
                int count = chunk.BytesRead; string content;
                using (var algorithm = SHA256.Create()) content = Convert.ToBase64String(algorithm.ComputeHash(chunk.Data, 0, count));
                bool repeatedBegin = reader.BeginRead();
                bool sameHeldChunk = Object.ReferenceEquals(chunk, reader.CurrentChunk);
                bool repeatPreservedBytes = chunk.BytesRead == count;
                delivered += count; reader.EndRead();
                chunks.Add(new { count, sha256 = content, repeatedBegin, sameHeldChunk, repeatPreservedBytes,
                    endCurrentNull = reader.CurrentChunk == null, endBytesRead = chunk.BytesRead, endRetainsData = chunk.Data != null });
            }
            Chunk eof = reader.CurrentChunk;
            bool eofNonNull = eof != null, eofDataNull = eof != null && eof.Data == null;
            int eofBytes = eof == null ? -1 : eof.BytesRead;
            uint eofSize = reader.FileSize;
            bool eofReusesChunk = first == null || Object.ReferenceEquals(first, eof);
            bool beginAfterEof = reader.BeginRead(); reader.EndRead();
            bool eofObjectPreserved = Object.ReferenceEquals(eof, reader.CurrentChunk);
            uint legacyHash = new FileHashItem(path, "input-reader-corpus", TargetPlatform.Win32).Hash;
            rows.Add(new { length, sha256, initialSize, initiallyNull, chunks = chunks.ToArray(), delivered,
                reusedChunk, eofReusesChunk, eofNonNull, eofDataNull, eofBytes, eofSize, beginAfterEof, eofObjectPreserved, legacyHash });
        }
        return rows.ToArray();
    }
#if !DOCUMENT_ORACLE_SHARED
    public static int Main(string[] args)
    {
        var json = new JavaScriptSerializer(); object result; int code = 0;
        try
        {
            if (args.Length != 6 || IntPtr.Size != 4) throw new Exception("Expected x86 work result source run original|fallback|owned SDK");
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
            object[] inheritance = InheritanceCases(args[0], args[5]);
            object[] inputReaders = InputReaderCases(args[0]);
            object[] defineLookups = DefineLookupCases(args[0]);
            object[] semanticDomRestores = SemanticDomRestoreCases(args[0], args[5]);
            string assemblySha;
            using (var sha = SHA256.Create()) assemblySha = Convert.ToBase64String(sha.ComputeHash(File.ReadAllBytes(typeof(AssetDeclarationDocument).Assembly.Location)));
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], mode, sameDeclaration, sourceXmlChanged,
                savedHash, receiverHash = receiver.SelfInstances[0].ProcessingHash, metadata, expressionTraversal,
                inheritance, inputReaders, defineLookups, semanticDomRestores, assembly = typeof(AssetDeclarationDocument).Assembly.FullName, assemblySha256Base64 = assemblySha,
                scope = "Direct public Core metadata lifecycle, 40 expression-DOM traversal cases and nine real-schema inheritance-stage rows and six original async-reader/digest rows; no production CLI failure is claimed" };
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
#endif
}
