using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;

// The original SDK DLL runs only in a disposable AppDomain. No original business
// assembly enters the production validation host's domain; comparisons exchange
// XML/error strings, counts and child positions, never original runtime objects.
public static class CompilerNodeJoiner
{
    const string Ns = "uri:ea.com:eala:asset";
    const string InstanceNs = "uri:ea.com:eala:asset:instance";
    static int checks, cases;
    static long originalAttributeReads, indexedAttributeReads, disabledAttributeReads;
    static readonly List<string> tests = new List<string>();
    static readonly List<object> observations = new List<object>();
    static LegacyNodeJoinerDomain original;
    internal static XmlSchemaSet schemas;
    static void Require(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }

    internal sealed class CountingDocument : XmlDocument
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
    internal static XmlSchemaSet Schema()
    {
        string text = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:a='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'>" +
            "<xs:complexType name='ItemType'><xs:sequence><xs:element name='Value' type='xs:string' minOccurs='0' maxOccurs='unbounded'/></xs:sequence><xs:attribute name='id' type='xs:string'/><xs:attribute name='mark' type='xs:string'/><xs:anyAttribute processContents='skip'/></xs:complexType>" +
            "<xs:complexType name='Asset'><xs:sequence><xs:element name='Item' type='a:ItemType' minOccurs='0' maxOccurs='unbounded'/><xs:element name='Other' type='a:ItemType' minOccurs='0' maxOccurs='unbounded'/><xs:element name='Tail' type='xs:string' minOccurs='0'/></xs:sequence><xs:attribute name='id' type='xs:string'/><xs:anyAttribute processContents='skip'/></xs:complexType>" +
            "<xs:complexType name='Choice'><xs:choice minOccurs='0'><xs:element name='Item' type='a:ItemType'/><xs:element name='Other' type='a:ItemType'/></xs:choice><xs:anyAttribute processContents='skip'/></xs:complexType>" +
            "<xs:complexType name='Many'><xs:choice minOccurs='0' maxOccurs='unbounded'><xs:element name='Item' type='a:ItemType'/><xs:element name='Other' type='a:ItemType'/></xs:choice><xs:anyAttribute processContents='skip'/></xs:complexType>" +
            "<xs:complexType name='Singles'><xs:sequence>" + SingletonElements() + "</xs:sequence><xs:anyAttribute processContents='skip'/></xs:complexType>" +
            "<xs:complexType name='Wide'><xs:sequence>" + WideElements() + "</xs:sequence><xs:anyAttribute processContents='skip'/></xs:complexType>" +
            "<xs:element name='Wide' type='a:Wide'/><xs:element name='Asset' type='a:Asset'/><xs:element name='Choice' type='a:Choice'/><xs:element name='Many' type='a:Many'/><xs:element name='Singles' type='a:Singles'/></xs:schema>";
        var result = new XmlSchemaSet();
        using (var reader = XmlReader.Create(new StringReader(text))) result.Add(Ns, reader);
        result.Compile(); return result;
    }
    static string WideElements()
    {
        var result = new StringBuilder();
        for (int index = 0; index < 64; index++) result.Append("<xs:element name='W").Append(index).Append("' type='a:ItemType' minOccurs='0' maxOccurs='unbounded'/>");
        return result.ToString();
    }
    static string SingletonElements()
    {
        var result = new StringBuilder();
        for (int index = 0; index < 12; index++) result.Append("<xs:element name='A").Append(index).Append("' type='a:ItemType'/>");
        return result.ToString();
    }
    internal static XmlDocument Document(string root, string body, string rootAttributes = "")
    {
        var document = new XmlDocument();
        document.LoadXml("<" + root + " xmlns='" + Ns + "' xmlns:p='" + Ns + "' xmlns:i='" + InstanceNs + "' " + rootAttributes + ">" + body + "</" + root + ">");
        document.Schemas = schemas;
        // NodeJoiner also operates on pre-validation input. Populate available
        // type annotations without hiding its own rejection behavior.
        document.Validate(delegate(object sender, ValidationEventArgs args) { });
        return document;
    }
    static string Outcome(XmlDocument baseDocument, XmlDocument overrideDocument, CountingDocument destination, bool disableIndex = false)
    {
        bool previousSwitch;
        AppContext.TryGetSwitch("NewSdk.Compiler.DisableNodeJoinerIndex", out previousSwitch);
        AppContext.SetSwitch("NewSdk.Compiler.DisableNodeJoinerIndex", disableIndex);
        try
        {
            XmlNode result = NodeJoiner.Override(schemas, destination, baseDocument.DocumentElement, overrideDocument.DocumentElement);
            return "XML:" + result.OuterXml;
        }
        catch (Exception error)
        {
            var invoke = error as TargetInvocationException;
            if (invoke != null) error = invoke.InnerException;
            PropertyInfo code = error.GetType().GetProperty("ErrorCode");
            return "ERROR:" + error.GetType().Name + ":" + (code == null ? "" : Convert.ToString(code.GetValue(error, null))) + ":" + error.Message;
        }
        finally { AppContext.SetSwitch("NewSdk.Compiler.DisableNodeJoinerIndex", previousSwitch); }
    }
    static string Compare(string label, string root, string before, string after, bool failure, string overrideAttributes = "")
    {
        var newDestination = new CountingDocument(); var disabledDestination = new CountingDocument();
        LegacyNodeJoinerResult baseline = original.Run(root, before, after, overrideAttributes);
        string expected = baseline.Outcome;
        string actual = Outcome(Document(root, before), Document(root, after, overrideAttributes), newDestination);
        Require(expected == actual, label + " original DLL differential failed\nexpected: " + expected + "\nactual: " + actual);
        string disabled = Outcome(Document(root, before), Document(root, after, overrideAttributes), disabledDestination, true);
        Require(expected == disabled, label + " disabled-index original DLL differential failed\nexpected: " + expected + "\nactual: " + disabled);
        Require(expected.StartsWith(failure ? "ERROR:" : "XML:", StringComparison.Ordinal), label + " did not exercise its intended outcome: " + expected);
        cases++; originalAttributeReads += baseline.AttributeReads; indexedAttributeReads += newDestination.AttributeReads; disabledAttributeReads += disabledDestination.AttributeReads;
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
        MethodInfo newSelect = typeof(NodeJoiner).GetMethod("SelectSame", BindingFlags.Instance | BindingFlags.NonPublic);
        var destination = new XmlDocument();
        destination.LoadXml("<Parent xmlns:p='urn:p' xmlns:q='urn:q'><A id='dup' mark='first'/><B id='dup' mark='second'/><A/><A id=''/><p:A id='p'/><q:A id='q'/><A id='last'/><A id='end'/></Parent>");
        XmlNode parent = destination.DocumentElement;
        var newJoiner = new NodeJoiner(schemas, destination);
        object index = null;
        Action<string, bool, bool> select = delegate(string xml, bool byId, bool replaceAny)
        {
            var source = new XmlDocument(); source.LoadXml(xml);
            int expected = original.Select(parent.OuterXml, xml, byId, replaceAny);
            object[] arguments = { parent, source.DocumentElement, byId, replaceAny, index };
            object actual = newSelect.Invoke(newJoiner, arguments); index = arguments[4];
            int position = -1, offset = 0;
            foreach (XmlNode child in parent.ChildNodes) { if (Object.ReferenceEquals(child, actual)) { position = offset; break; } offset++; }
            Require(expected == position, "Private original selector differential: " + xml + " byId=" + byId + " replaceAny=" + replaceAny);
        };
        select("<B id='dup'/>", true, false);
        Require(index != null, "Wide duplicate-parent fixture did not instantiate an index");
        select("<A/>", false, false);
        select("<A id=''/>", true, false);
        select("<A/>", true, false);
        select("<p:A xmlns:p='urn:other'/>", false, false);
        select("<r:A xmlns:r='urn:p'/>", false, false);
        XmlNode prefixed = parent.ChildNodes[4];
        Require(prefixed.Name == "p:A", "Prefix fixture selected the wrong node");
        int prefixEvents = 0; XmlNodeChangedEventHandler prefixEvent = (sender, change) => { prefixEvents++; };
        destination.NodeInserted += prefixEvent; destination.NodeRemoved += prefixEvent; destination.NodeChanged += prefixEvent;
        prefixed.Prefix = "q";
        destination.NodeInserted -= prefixEvent; destination.NodeRemoved -= prefixEvent; destination.NodeChanged -= prefixEvent;
        select("<p:A xmlns:p='urn:p'/>", false, false); select("<q:A xmlns:q='urn:q'/>", false, false);
        observations.Add(new { id = "private-selector-prefix-mutation-with-existing-id-index", prefixEvents, resultingName = prefixed.Name });
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
        ((IDisposable)index).Dispose();
        tests.Add("original-private-selector-duplicate-first-key-name-prefix-insert-remove-key-refresh-and-shrink-to-single-choice");
    }
    internal sealed class DomMutation : IDisposable
    {
        readonly XmlDocument document;
        readonly string scenario;
        internal int Calls;
        internal bool IndexedWhenCalled;
        internal int PrefixEvents;
        bool changingPrefix;
        internal DomMutation(XmlDocument document, string scenario)
        {
            this.document = document; this.scenario = scenario;
            document.NodeInserted += Changed; document.NodeRemoved += Changed; document.NodeChanged += Changed;
        }
        void Changed(object sender, XmlNodeChangedEventArgs change)
        {
            if (changingPrefix) PrefixEvents++;
            if (Calls != 0) return;
            XmlNode parent = null;
            if (scenario.StartsWith("inserted:", StringComparison.Ordinal))
            {
                if (change.Action == XmlNodeChangedAction.Insert && change.Node is XmlText && change.Node.Value == "fire" && change.NewParent.Name == "Value")
                    parent = change.NewParent.ParentNode.ParentNode;
            }
            else if (scenario.StartsWith("removed:", StringComparison.Ordinal))
            {
                var element = change.Node as XmlElement;
                if (change.Action == XmlNodeChangedAction.Remove && element != null && element.GetAttribute("id") == "k0") parent = change.OldParent;
            }
            else
            {
                var attribute = change.NewParent as XmlAttribute;
                if (change.Action == XmlNodeChangedAction.Change && attribute != null && attribute.Name == "mark" && attribute.Value == "trigger")
                    parent = attribute.OwnerElement.ParentNode;
            }
            if (parent == null || (parent.Name != "Asset" && parent.Name != "Singles") || parent.ChildNodes.Count < 8) return;
            Calls++; IndexedWhenCalled = ChildIndexSubscriptions(document) > 0;
            string action = scenario.Substring(scenario.IndexOf(':') + 1);
            XmlElement first = (XmlElement)parent.SelectSingleNode("*[@id='target']"), last = (XmlElement)parent.LastChild;
            XmlElement earlier = (XmlElement)parent.SelectSingleNode("*[@id='k10']");
            if (action == "prefix")
            {
                changingPrefix = true;
                try { earlier.Prefix = "p"; }
                finally { changingPrefix = false; }
            }
            else if (action == "changed-id" || action == "throw") earlier.SetAttribute("id", "target");
            else if (action == "added-id") { earlier.RemoveAttribute("id"); earlier.SetAttribute("id", "target"); }
            else if (action == "removed-id") first.RemoveAttribute("id");
            else if (action == "reorder") parent.PrependChild(last);
            else if (action == "remove-child") parent.RemoveChild(first);
            else if (action == "move-out") document.CreateElement("Elsewhere").AppendChild(first);
            else if (action == "insert-child" || action == "move-in")
            {
                XmlElement inserted = document.CreateElement("Item", Ns); inserted.SetAttribute("id", "target"); inserted.SetAttribute("mark", "callback");
                if (action == "move-in") document.CreateElement("Elsewhere").AppendChild(inserted);
                parent.PrependChild(inserted);
            }
            if (action == "throw") throw new InvalidOperationException("Injected DOM callback failure");
        }
        public void Dispose() { document.NodeInserted -= Changed; document.NodeRemoved -= Changed; document.NodeChanged -= Changed; }
    }
    internal static int ChildIndexSubscriptions(XmlDocument document)
    {
        int count = 0;
        foreach (FieldInfo field in typeof(XmlDocument).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
        {
            if (!typeof(Delegate).IsAssignableFrom(field.FieldType)) continue;
            var callbacks = field.GetValue(document) as Delegate;
            if (callbacks == null) continue;
            foreach (Delegate callback in callbacks.GetInvocationList())
                if (callback.Target != null && callback.Target.GetType().FullName == "BinaryAssetBuilder.Core.NodeJoiner+ChildIndex") count++;
        }
        return count;
    }
    static void DynamicCallbacks()
    {
        string before = Items(12) + "<Item id='target' mark='first'/><Item id='target' mark='second'/>";
        foreach (string scenario in new[] { "changed:changed-id", "changed:added-id", "changed:removed-id", "changed:reorder", "changed:insert-child", "changed:remove-child",
            "changed:move-out", "changed:move-in", "inserted:changed-id", "removed:changed-id", "changed:throw" })
        {
            string after = scenario.StartsWith("inserted:", StringComparison.Ordinal) ? "<Item id='k0'><Value>fire</Value></Item>" :
                scenario.StartsWith("removed:", StringComparison.Ordinal) ? "<Item id='k0' i:joinAction='Remove'/>" : "<Item id='k0' mark='trigger'/>";
            after += "<Item id='target' mark='selected'/><Item id='target' mark='selected-again'/>";
            LegacyNodeJoinerResult baseline = original.Run("Asset", before, after, "", scenario);
            Require(baseline.MutationCalls == 1, "Original DLL dynamic callback did not execute above the index threshold: " + scenario);
            foreach (bool disabled in new[] { false, true })
            {
                var destination = new CountingDocument();
                using (var mutation = new DomMutation(destination, scenario))
                {
                    string actual = Outcome(Document("Asset", before), Document("Asset", after), destination, disabled);
                    Require(mutation.Calls == 1 && actual == baseline.Outcome, "Dynamic DOM callback original DLL differential failed: " + scenario + " disabled=" + disabled + "\nexpected: " + baseline.Outcome + "\nactual: " + actual);
                    Require(mutation.IndexedWhenCalled == !disabled, "Dynamic callback did not run with the expected live index: " + scenario + " disabled=" + disabled);
                    Require(ChildIndexSubscriptions(destination) == 0, "NodeJoiner retained document subscriptions after merge/failure: " + scenario);
                }
            }
            Require(baseline.Outcome.StartsWith(scenario.EndsWith(":throw", StringComparison.Ordinal) ? "ERROR:" : "XML:", StringComparison.Ordinal), "Dynamic callback did not exercise its intended success/failure");
            cases++;
        }
        // Confirm the lifecycle inspection sees the real three subscriptions,
        // rather than passing because the runtime hides its delegate storage.
        var document = new XmlDocument(); document.LoadXml("<Parent>" + Items(12) + "</Parent>");
        var source = new XmlDocument(); source.LoadXml("<Item id='k0'/>");
        object[] arguments = { document.DocumentElement, source.DocumentElement, true, false, null };
        typeof(NodeJoiner).GetMethod("SelectSame", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(new NodeJoiner(schemas, document), arguments);
        Require(arguments[4] != null && ChildIndexSubscriptions(document) == 3, "Wide selector did not expose its three live DOM event subscriptions");
        ((IDisposable)arguments[4]).Dispose();
        Require(ChildIndexSubscriptions(document) == 0, "ChildIndex.Dispose did not detach all document events");
        tests.Add("original-DLL-dynamic-DOM-callback-sibling-id-add-remove-value-duplicate-order-structural-move-and-exception-differential-with-verified-event-unsubscription");
    }
    static void PrefixNameMatching()
    {
        var before = new StringBuilder();
        for (int index = 0; index < 12; index++) before.Append("<A").Append(index).Append(" id='k").Append(index).Append("' mark='base'/>");
        const string after = "<A0 mark='trigger'/><A10 mark='selected'/><A10 mark='selected-again'/>";
        LegacyNodeJoinerResult baseline = original.Run("Singles", before.ToString(), after, "", "changed:prefix");
        Require(baseline.MutationCalls == 1 && baseline.Outcome.StartsWith("XML:", StringComparison.Ordinal), "Original DLL singleton prefix callback did not execute above the index threshold");
        foreach (bool disabled in new[] { false, true })
        {
            var destination = new CountingDocument();
            using (var mutation = new DomMutation(destination, "changed:prefix"))
            {
                string actual = Outcome(Document("Singles", before.ToString()), Document("Singles", after), destination, disabled);
                Require(mutation.Calls == 1 && actual == baseline.Outcome, "Wide singleton name matching did not observe a callback Prefix mutation: disabled=" + disabled + "\nexpected: " + baseline.Outcome + "\nactual: " + actual);
                Require(!mutation.IndexedWhenCalled && ChildIndexSubscriptions(destination) == 0, "Name-only matching created or retained an unsafe Prefix-sensitive index");
                observations.Add(new { id = "original-DLL-wide-singleton-prefix-callback", disabled, originalPrefixEvents = baseline.PrefixEvents, prefixEvents = mutation.PrefixEvents });
            }
        }
        cases++;
        tests.Add("original-DLL-wide-maxOccurs-one-name-matching-Prefix-callback-keeps-linear-selection-without-DOM-event-assumptions");
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
    static string PredecessorOutcome(string before, string after, bool disabled, bool derived, out long builds, out long entries, out long queries)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        const string name = "NewSdk.Compiler.DisableNodeJoinerPredecessors";
        bool previous; AppContext.TryGetSwitch(name, out previous); AppContext.SetSwitch(name, disabled);
        var destination = derived ? (XmlDocument)new CountingDocument() : new XmlDocument();
        var joiner = new NodeJoiner(schemas, destination);
        try
        {
            XmlNode src = Document("Wide", before).DocumentElement;
            XmlNode result = destination.CreateNode(src.NodeType, src.Name, src.NamespaceURI);
            MethodInfo replace = typeof(NodeJoiner).GetMethod("ReplaceXmlNode", flags);
            object append = Enum.ToObject(replace.GetParameters()[4].ParameterType, 0);
            replace.Invoke(joiner, new object[] { result, src, null, false, append });
            replace.Invoke(joiner, new object[] { result, Document("Wide", after).DocumentElement, null, false, append });
            return "XML:" + result.OuterXml;
        }
        finally
        {
            builds = (long)typeof(NodeJoiner).GetField("m_predecessorBuilds", flags).GetValue(joiner);
            entries = (long)typeof(NodeJoiner).GetField("m_predecessorEntries", flags).GetValue(joiner);
            queries = (long)typeof(NodeJoiner).GetField("m_predecessorQueries", flags).GetValue(joiner);
            AppContext.SetSwitch(name, previous);
        }
    }
    static void PredecessorPlans()
    {
        var before = new StringBuilder();
        for (int index = 0; index < 64; index++) before.Append("<W").Append(index).Append(" id='base").Append(index).Append("'/>");
        foreach (string after in new[] {
            "<W0 id='new0'/><W31 id='new31'/><W63 id='new63'/>",
            "<W32 id='top' i:insertPosition='Top'/><W32 id='bottom'/><W16 id='middle'/>",
            "<W33 id='base33' i:joinAction='Remove'/><W33 id='replacement'/><W0 id='base0' i:joinAction='Replace'/>" })
        {
            string expected = original.Run("Wide", before.ToString(), after, "").Outcome;
            long builds, entries, queries;
            string actual = PredecessorOutcome(before.ToString(), after, false, false, out builds, out entries, out queries);
            Require(actual == expected && expected.StartsWith("XML:", StringComparison.Ordinal), "Wide predecessor original DLL XML differential");
            Require(builds > 0 && entries > 0 && queries > 0, "Wide exact-element insertion did not consume predecessor plans");
            observations.Add(new { id = "wide-predecessor-plan", builds, entries, queries });
            string disabled = PredecessorOutcome(before.ToString(), after, true, false, out builds, out entries, out queries);
            Require(disabled == expected && builds == 0 && queries == 0, "Predecessor switch did not restore legacy traversal");
            string derived = PredecessorOutcome(before.ToString(), after, false, true, out builds, out entries, out queries);
            Require(derived == expected && builds == 0 && queries == 0, "Virtual destination element did not conservatively retain legacy traversal");
            cases++;
        }
        // The private scalar set must exactly match the original base-first
        // helper for every candidate, including own target and unknown names.
        var type = (XmlSchemaComplexType)schemas.GlobalTypes[new XmlQualifiedName("Wide", Ns)];
        var sequence = (XmlSchemaSequence)type.ContentTypeParticle;
        var parent = new XmlDocument(); parent.LoadXml("<Parent>" + Items(12) + "</Parent>");
        var owner = new NodeJoiner(schemas, parent);
        MethodInfo plan = typeof(NodeJoiner).GetMethod("PredecessorNames", BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (int targetIndex in new[] { 0, 1, 31, 63 })
        {
            var target = (XmlSchemaElement)sequence.Items[targetIndex];
            var names = (HashSet<string>)plan.Invoke(owner, new object[] { parent.DocumentElement, type, target });
            Require(names != null, "Predecessor plan unexpectedly absent");
            foreach (string candidate in new[] { "W0", "W1", "W31", "W32", "W63", "Unknown", "p:W0" })
                Require(names.Contains(candidate) == original.Predecessor(targetIndex, candidate), "Original predecessor helper mismatch " + targetIndex + ":" + candidate);
        }
        tests.Add("original-DLL-wide-schema-insertion-and-predecessor-helper-differential-local-bounded-set-with-switch-and-virtual-node-fallback");
    }
    static void PredecessorMeasurement()
    {
        var before = new StringBuilder(); var after = new StringBuilder();
        for (int index = 0; index < 64; index++)
        {
            for (int item = 0; item < 4; item++) before.Append("<W").Append(index).Append(" id='b").Append(index).Append('_').Append(item).Append("'/>");
            after.Append("<W").Append(index).Append(" id='new").Append(index).Append("'/>");
        }
        string expected = original.Run("Wide", before.ToString(), after.ToString(), "").Outcome;
        Require(expected.StartsWith("XML:", StringComparison.Ordinal), "Predecessor timing corpus original outcome failed");
        var baseDocument = Document("Wide", before.ToString()); var overrideDocument = Document("Wide", after.ToString());
        var samples = new List<object>(); var indexed = new List<long>(); var linear = new List<long>();
        const string name = "NewSdk.Compiler.DisableNodeJoinerPredecessors";
        bool previous; AppContext.TryGetSwitch(name, out previous);
        try
        {
            for (int round = -1; round < 7; round++)
            {
                bool[] order = round % 2 == 0 ? new[] { false, true } : new[] { true, false };
                foreach (bool disabled in order)
                {
                    AppContext.SetSwitch(name, disabled);
                    var destination = new XmlDocument();
                    long start = System.Diagnostics.Stopwatch.GetTimestamp();
                    XmlNode merged = NodeJoiner.Override(schemas, destination, baseDocument.DocumentElement, overrideDocument.DocumentElement);
                    long ticks = System.Diagnostics.Stopwatch.GetTimestamp() - start;
                    Require("XML:" + merged.OuterXml == expected, "Predecessor timed merge changed original output");
                    if (round < 0) continue;
                    (disabled ? linear : indexed).Add(ticks);
                    samples.Add(new { round, disabled, elapsedMs = ticks * 1000.0 / System.Diagnostics.Stopwatch.Frequency });
                }
            }
        }
        finally { AppContext.SetSwitch(name, previous); }
        indexed.Sort(); linear.Sort();
        observations.Add(new { id = "wide-predecessor-component-measurement", rounds = 7, beforeChildren = 256, insertedChildren = 64,
            indexedMedianMs = indexed[3] * 1000.0 / System.Diagnostics.Stopwatch.Frequency,
            legacyMedianMs = linear[3] * 1000.0 / System.Diagnostics.Stopwatch.Frequency,
            samples = samples.ToArray(), scope = "Complete public NodeJoiner merge only; excludes source parse/validation/XML comparison, no CLI speed or significance claim" });
        tests.Add("interleaved-seven-round-wide-predecessor-public-merge-timing-with-exact-original-XML-and-no-speed-assertion");
    }
    static void PredecessorOwnerMetrics()
    {
        var before = new StringBuilder();
        for (int index = 0; index < 64; index++) before.Append("<W").Append(index).Append(" id='b").Append(index).Append("'/>");
        const string after = "<W32 id='new'/><W0 id='first'/>";
        string expected = original.Run("Wide", before.ToString(), after, "").Outcome;
        bool previous; AppContext.TryGetSwitch("NewSdk.Compiler.DisableNodeJoinerPredecessors", out previous);
        try
        {
            foreach (bool disabled in new[] { false, true })
            {
                AppContext.SetSwitch("NewSdk.Compiler.DisableNodeJoinerPredecessors", disabled);
                var owner = new DocumentProcessor(new Settings(), new PluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32), null);
                XmlNode result = (XmlNode)typeof(NodeJoiner).GetMethod("OverrideWithWork", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null,
                    new object[] { schemas, new XmlDocument(), Document("Wide", before.ToString()).DocumentElement, Document("Wide", after).DocumentElement, owner });
                var serializer = new JavaScriptSerializer();
                var metrics = serializer.Deserialize<Dictionary<string, object>>(serializer.Serialize(owner.GetNodeJoinerPredecessorMetrics()));
                Require("XML:" + result.OuterXml == expected && Convert.ToInt64(metrics["calls"]) == 1 &&
                    Convert.ToInt64(metrics["optimizationEnabledCalls"]) == (disabled ? 0 : 1), "Owned predecessor call/output diagnostics mismatch");
                Require(Convert.ToInt64(metrics[disabled ? "linearQueries" : "indexQueries"]) > 0 &&
                    (!disabled || Convert.ToInt64(metrics["indexQueries"]) == 0), "Owned predecessor actual query work missing");
                observations.Add(new { id = "owner-predecessor-actual-query-work", disabled, metrics });
            }
        }
        finally { AppContext.SetSwitch("NewSdk.Compiler.DisableNodeJoinerPredecessors", previous); }
        tests.Add("actual-owner-NodeJoiner-merge-scalar-report-counter-and-disabled-legacy-query-path");
    }
    public static int Main(string[] args)
    {
        int code = 0; object result; AppDomain baselineDomain = null;
        try
        {
            if (args.Length != 5 || IntPtr.Size != 4) throw new Exception("Expected work result sourceCommit runId sdk (x86)");
            Directory.CreateDirectory(args[0]);
            string library = Path.Combine(args[4], "tools/BinaryAssetBuilder.Core.dll");
            baselineDomain = AppDomain.CreateDomain("NodeJoiner-original-" + Guid.NewGuid().ToString("N"), null, new AppDomainSetup {
                ApplicationBase = Path.GetDirectoryName(typeof(CompilerNodeJoiner).Assembly.Location),
                ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
            });
            original = (LegacyNodeJoinerDomain)baselineDomain.CreateInstanceFromAndUnwrap(typeof(LegacyNodeJoinerDomain).Assembly.Location, typeof(LegacyNodeJoinerDomain).FullName);
            string baselineAssembly = original.Initialize(library);
            Require(baselineAssembly.StartsWith("BinaryAssetBuilder.Core,", StringComparison.Ordinal), "Isolated original SDK assembly identity mismatch");
            schemas = Schema(); Semantics(); DuplicateIndexSemantics(); DynamicCallbacks(); PrefixNameMatching(); WorkReduction(); PredecessorPlans(); PredecessorMeasurement(); PredecessorOwnerMetrics();
            AppDomain.Unload(baselineDomain); baselineDomain = null; original = null;
            foreach (Assembly loaded in AppDomain.CurrentDomain.GetAssemblies())
                Require(!loaded.GetName().Name.Equals("BinaryAssetBuilder.Core", StringComparison.Ordinal), "Original Core escaped the disposable baseline AppDomain");
            tests.Add("original-Core-isolated-in-unloaded-AppDomain-production-host-ownership-preserved");
            string sha256;
            using (var sha = SHA256.Create()) sha256 = BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(library))).Replace("-", "").ToLowerInvariant();
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks = checks, tests = tests.ToArray(), observations = observations.ToArray(), cases = cases,
                originalDllSha256 = sha256, originalDllAssembly = baselineAssembly, originalDestinationAttributeReads = originalAttributeReads,
                indexedDestinationAttributeReads = indexedAttributeReads, disabledDestinationAttributeReads = disabledAttributeReads, scope = "Real original DLL NodeJoiner XML/rejection differential and matching work; no end-to-end performance claim" };
            Console.WriteLine("PASS NodeJoiner: " + cases + " original DLL differential cases; destination attribute reads original=" + originalAttributeReads + " indexed=" + indexedAttributeReads);
        }
        catch (Exception error)
        {
            code = 1; Console.Error.WriteLine(error);
            result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null, runId = args.Length > 3 ? args[3] : null,
                checks = checks, tests = tests.ToArray(), observations = observations.ToArray(), cases = cases, error = error.ToString() };
        }
        finally { if (baselineDomain != null) AppDomain.Unload(baselineDomain); }
        if (args.Length > 1) File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(result));
        return code;
    }
}

// MarshalByRefObject is the only cross-domain service. Every public input and
// output uses framework primitives or our serializable scalar result.
[Serializable]
public sealed class LegacyNodeJoinerResult
{
    public string Outcome;
    public long AttributeReads;
    public int MutationCalls;
    public int PrefixEvents;
}

public sealed class LegacyNodeJoinerDomain : MarshalByRefObject
{
    MethodInfo original;
    public override object InitializeLifetimeService() { return null; }
    public string Initialize(string library)
    {
        Assembly baseline = Assembly.LoadFrom(library);
        if (!String.Equals(Path.GetFullPath(baseline.Location), Path.GetFullPath(library), StringComparison.OrdinalIgnoreCase))
            throw new Exception("Isolated baseline resolved outside the pinned SDK library path");
        original = baseline.GetType("BinaryAssetBuilder.Core.NodeJoiner", true).GetMethod("Override", BindingFlags.Public | BindingFlags.Static);
        if (original == null) throw new Exception("Original SDK NodeJoiner.Override missing");
        CompilerNodeJoiner.schemas = CompilerNodeJoiner.Schema();
        return baseline.FullName;
    }
    public LegacyNodeJoinerResult Run(string root, string before, string after, string attributes, string mutationScenario = null)
    {
        var destination = new CompilerNodeJoiner.CountingDocument();
        string outcome;
        CompilerNodeJoiner.DomMutation mutation = mutationScenario == null ? null : new CompilerNodeJoiner.DomMutation(destination, mutationScenario);
        try
        {
            XmlNode result = (XmlNode)original.Invoke(null, new object[] {
                CompilerNodeJoiner.schemas, destination,
                CompilerNodeJoiner.Document(root, before).DocumentElement,
                CompilerNodeJoiner.Document(root, after, attributes).DocumentElement
            });
            outcome = "XML:" + result.OuterXml;
        }
        catch (Exception error)
        {
            var wrapped = error as TargetInvocationException;
            if (wrapped != null) error = wrapped.InnerException;
            PropertyInfo code = error.GetType().GetProperty("ErrorCode");
            outcome = "ERROR:" + error.GetType().Name + ":" + (code == null ? "" : Convert.ToString(code.GetValue(error, null))) + ":" + error.Message;
        }
        if (mutation != null) mutation.Dispose();
        return new LegacyNodeJoinerResult { Outcome = outcome, AttributeReads = destination.AttributeReads, MutationCalls = mutation == null ? 0 : mutation.Calls, PrefixEvents = mutation == null ? 0 : mutation.PrefixEvents };
    }
    public bool Predecessor(int targetIndex, string candidate)
    {
        var type = (XmlSchemaComplexType)CompilerNodeJoiner.schemas.GlobalTypes[new XmlQualifiedName("Wide", "uri:ea.com:eala:asset")];
        var sequence = (XmlSchemaSequence)type.ContentTypeParticle;
        var document = new XmlDocument(); document.LoadXml("<" + candidate + (candidate.StartsWith("p:", StringComparison.Ordinal) ? " xmlns:p='urn:p'" : "") + "/>");
        object joiner = Activator.CreateInstance(original.DeclaringType, new object[] { CompilerNodeJoiner.schemas, document });
        return (bool)original.DeclaringType.GetMethod("FindPrevNode", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(joiner, new object[] { type, document.DocumentElement, sequence.Items[targetIndex] });
    }
    public int Select(string parentXml, string sourceXml, bool byId, bool replaceAny)
    {
        var destination = new XmlDocument(); destination.LoadXml(parentXml);
        var source = new XmlDocument(); source.LoadXml(sourceXml);
        object joiner = Activator.CreateInstance(original.DeclaringType, new object[] { CompilerNodeJoiner.schemas, destination });
        MethodInfo method = original.DeclaringType.GetMethod("SelectSame", BindingFlags.Instance | BindingFlags.NonPublic);
        var selected = (XmlNode)method.Invoke(joiner, new object[] { destination.DocumentElement, source.DocumentElement, byId, replaceAny });
        int position = 0;
        foreach (XmlNode child in destination.DocumentElement.ChildNodes) { if (Object.ReferenceEquals(child, selected)) return position; position++; }
        return -1;
    }
}
