using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.ExpressionEval;
#if !EXPRESSION_ORIGINAL
using NewModCompiler.Application;
#endif

// One corpus is compiled against original SDK Core/ExpressionEval in an isolated
// process, and against the production validation host. Originals never enter the
// production host's directory or loader context.
public static class CompilerExpressionPrograms
{
    const string Disable = "NewSdk.Compiler.DisableExpressionPrograms", DisableCulture = "NewSdk.Compiler.DisableSharedExpressionCulture";
    const string DisableTraversal = "NewSdk.Compiler.DisableExpressionTraversal";
    const BindingFlags Hidden = BindingFlags.NonPublic | BindingFlags.Instance;
    static readonly JavaScriptSerializer Json = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
    static int checks;
    static readonly List<string> tests = new List<string>();
    static readonly List<object> observations = new List<object>();
    static void Require(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    static DocumentProcessor Owner()
    {
        return new DocumentProcessor(Settings.Current, new PluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32), null);
    }
    static AssetDeclarationDocument Bare(DocumentProcessor owner, string source)
    {
        var document = new AssetDeclarationDocument(); var type = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object state = Activator.CreateInstance(type, true); type.GetMethod("FromScratch").Invoke(state, new object[0]);
        type.GetField("AllDefines").SetValue(state, new DefinitionSet());
        type.GetField("DocumentProcessor").SetValue(state, owner); type.GetField("SourcePath").SetValue(state, source);
        typeof(AssetDeclarationDocument).GetField("_Current", Hidden).SetValue(document, state); return document;
    }
    static AssetDeclarationDocument Input(DocumentProcessor owner, string scenario)
    {
        var document = Bare(owner, "expression-programs.xml"); var external = Bare(owner, "expression-provider-" + scenario + ".xml");
        foreach (string name in new[] { "I", "F", "S", "PENDING", "EMPTY" })
            document.AllDefines[name] = new Definition { Name = name, Document = name == "I" ? document : external,
                EvaluatedValue = name == "I" ? "7" : name == "F" ? "1.5" : name == "S" ? "hello" : name == "EMPTY" ? "" : null };
        if (scenario == "I12-local" || scenario == "I12-external") document.AllDefines["I"].EvaluatedValue = "12";
        if (scenario == "I13-external") document.AllDefines["I"].EvaluatedValue = "13";
        if (scenario.EndsWith("-external", StringComparison.Ordinal)) document.AllDefines["I"].Document = external;
        if (scenario == "I-null") document.AllDefines["I"].EvaluatedValue = null;
        if (scenario == "I-missing") document.AllDefines.Remove("I");
        return document;
    }
    static string Error(Exception error)
    {
        while (error is TargetInvocationException) error = error.InnerException;
        var code = error.GetType().GetProperty("ErrorCode");
        return error.GetType().FullName + ":" + (code == null ? "" : Convert.ToString(code.GetValue(error, null), CultureInfo.InvariantCulture)) + ":" + error.Message;
    }
    static string[] Outcome(string culture, string scenario, string expression, AssetDeclarationDocument document)
    {
        string outcome; var prior = Console.Error; var diagnostics = new StringWriter(CultureInfo.InvariantCulture);
        try
        {
            Console.SetError(diagnostics);
            try { outcome = "OK:" + new ExpressionEvaluator(document).Evaluate(expression); }
            catch (Exception error) { outcome = "ERR:" + Error(error); }
        }
        finally { Console.SetError(prior); }
        string used = String.Join(";", document.UsedDefines.Select(pair => pair.Key + "=" + pair.Value).ToArray());
        return new[] { culture, scenario, expression, outcome, diagnostics.ToString(), used };
    }
    static readonly string[] Corpus = {
        "=1+2*3", "=(1+2)*3", "=7/2", "=8/2", "=5%2", "=-2+3", "=9-3-1", "=16/2/2", "=1=1", "=1!=2", "=1<2", "=2>1", "=1<=1", "=2>=2",
        "=1 or 0 and 0", "=0 and (1/0)", "=1 or (1/0)", "=round(2.5)", "=round(-1.5)", "=round(2)", "=round('x')",
        "='a'+'b'", "=\"a\"+'b'", "='2'+3", "='a'='A'", "='a'<'b'", "=1.5%1", "=1/0", "=1.5/0", "=2147483647+1", "=-2147483648/-1", "=2147483648",
        "=$I+$F", "=$S+'!'", "=$EMPTY", "=$MISSING", "=$PENDING", "=", "", "1+2", "=1+", "=(1", "=1 2", "=foo", "=1@2", "=1e2", "=.5", "=1.", "=1\n+\t2", "=1==1", "=not 0", "=!0", "=1!=1!=0", "='unterminated", "=$I",
        "=$MISSING+$PENDING", "=1/0+$MISSING", "=$I+@", "='left'+$S+'right'", "=$I+$I+$I"
    };
    static IEnumerable<string[]> Specifications()
    {
        foreach (string culture in new[] { "en-US", "de-DE", "tr-TR", "zh-CN" })
        {
            foreach (string expression in Corpus) yield return new[] { culture, "standard", expression };
            foreach (string scenario in new[] { "I7-local", "I12-local", "I12-external", "I-null", "I-missing", "I13-external" })
                yield return new[] { culture, scenario, "=$I+$I+$I" };
        }
    }
#if EXPRESSION_ORIGINAL
    public static int Main(string[] args)
    {
        try
        {
            Settings.Current = new Settings { BuildCache = false, SessionCache = false }; var owner = Owner();
            var rows = new List<string[]>();
            foreach (var spec in Specifications())
            { Thread.CurrentThread.CurrentCulture = new CultureInfo(spec[0]); rows.Add(Outcome(spec[0], spec[1], spec[2], Input(owner, spec[1]))); }
            File.WriteAllText(args[0], Json.Serialize(rows)); Console.WriteLine("PASS ExpressionOriginal: " + rows.Count + " rows"); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
#else
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
    static IDictionary<string, object> TraversalOutcome(IDictionary<string, object> spec, DocumentProcessor owner)
    {
        string name = (string)spec["name"];
        var document = Bare(owner, "expression-tree.xml");
        foreach (string key in new[] { "I", "S" }) document.AllDefines[key] = new Definition { Name = key, EvaluatedValue = key == "I" ? "7" : "hello", Document = document };
        var xml = new XmlDocument { PreserveWhitespace = true }; xml.LoadXml((string)spec["inputXml"]); ChangingText custom = null; CountingElement customElement = null;
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
        string outcome = "OK"; var prior = Console.Error; var diagnostics = new StringWriter(CultureInfo.InvariantCulture);
        try { Console.SetError(diagnostics); try { document.ProcessExpressions(); } catch (Exception error) { outcome = "ERR:" + Error(error); } }
        finally { Console.SetError(prior); }
        Require((name != "remove-current" && name != "append-sibling" && name != "move-current" && name != "remove-next") || mutated,
            "Traversal mutation fixture never reached Trigger: " + name);
        int customReads = custom == null ? 0 : custom.Reads;
        int customElementReads = customElement == null ? 0 : customElement.ValueReads, customChildLists = customElement == null ? 0 : customElement.ChildLists;
        return new Dictionary<string, object> { { "outcome", outcome }, { "resultXml", xml.OuterXml }, { "customReads", customReads }, { "mutated", mutated },
            { "customElementReads", customElementReads }, { "customChildLists", customChildLists },
            { "used", String.Join(";", document.UsedDefines.Select(pair => pair.Key + "=" + pair.Value)) }, { "diagnostics", diagnostics.ToString() } };
    }
    static void TraversalDifferential(string baseline, string sourceCommit, string runId)
    {
        var oracle = Json.Deserialize<Dictionary<string, object>>(File.ReadAllText(baseline));
        Require((bool)oracle["succeeded"] && (string)oracle["mode"] == "original" &&
            (string)oracle["sourceCommit"] == sourceCommit && (string)oracle["runId"] == runId, "Original traversal oracle source/run mismatch");
        var rows = (IEnumerable)oracle["expressionTraversal"]; int count = 0;
        foreach (IDictionary<string, object> row in rows)
        {
            count++; Thread.CurrentThread.CurrentCulture = new CultureInfo((string)row["culture"]);
            foreach (bool fallback in new[] { false, true })
            {
                AppContext.SetSwitch(DisableTraversal, fallback); var owner = Owner(); var actual = TraversalOutcome(row, owner);
                foreach (string field in actual.Keys)
                    Require(Convert.ToString(actual[field], CultureInfo.InvariantCulture) == Convert.ToString(row[field], CultureInfo.InvariantCulture),
                        "Original/" + (fallback ? "legacy" : "optimized") + " traversal mismatch: " + row["name"] + "/" + row["culture"] + "/" + field + ": " + Json.Serialize(actual) + " != " + Json.Serialize(row));
                var metrics = Json.Deserialize<Dictionary<string, object>>(Json.Serialize(owner.GetExpressionTraversalMetrics()));
                Require(Number(metrics, "calls") == 1 && Number(metrics, "nodes") > 0,
                    "Actual public expression traversal metrics missing");
                Require(fallback ? Number(metrics, "siblingTraversals") == 0 && Number(metrics, "singleReadAttributes") == 0 : Number(metrics, "singleReadAttributes") > 0,
                    "Traversal did not use selected child enumeration path");
                Require(Number(metrics, "failedCalls") == (((string)row["outcome"]).StartsWith("ERR:", StringComparison.Ordinal) ? 1 : 0),
                    "Traversal failure metric does not reflect actual failed public call");
                if ((string)row["name"] == "custom-attribute") Require(Number(metrics, "legacyAttributes") > 0,
                    "Exact attribute containing custom XmlText bypassed virtual Value fallback");
                if ((string)row["name"] == "custom-element") Require(Number(metrics, "legacyTraversals") > 0,
                    "Derived element bypassed custom ChildNodes enumeration");
                observations.Add(new { id = "public-expression-traversal-original-differential", name = row["name"], culture = row["culture"], fallback, actual, metrics });
            }
        }
        Require(count == 40, "Expected ten original public traversal cases in four cultures");
        AppContext.SetSwitch(DisableTraversal, false);
        tests.Add("public-expression-traversal-original-DLL-vs-default-fallback-mixed-DOM-errors-entity-custom-text-and-live-sibling-mutations-four-cultures");
    }
    static void TraversalMeasurement()
    {
        const int parts = 256, iterations = 100, rounds = 7;
        string body = String.Concat(Enumerable.Repeat("<Part A='plain-value-123456789' B='plain' C='x' D='' E='literal'><Text>literal-text</Text></Part>", parts));
        string input = "<ArmorTemplate id='Measure'>" + body + "</ArmorTemplate>";
        var owners = new[] { Owner(), Owner() }; var documents = new AssetDeclarationDocument[2];
        for (int mode = 0; mode < 2; mode++)
        {
            documents[mode] = Bare(owners[mode], "traversal-measurement.xml");
            var xml = new XmlDocument(); xml.LoadXml(input);
            documents[mode].SelfInstances.Add(new InstanceDeclaration(documents[mode]) { XmlNode = xml.DocumentElement });
            AppContext.SetSwitch(DisableTraversal, mode == 1);
            for (int warm = 0; warm < 5; warm++) documents[mode].ProcessExpressions();
            Require(xml.OuterXml == input.Replace("'", "\""), "Traversal measurement changed non-expression input");
        }
        var samples = new List<object>(); var optimized = new List<long>(); var legacy = new List<long>();
        for (int round = 0; round < rounds; round++)
        for (int position = 0; position < 2; position++)
        {
            int mode = (round + position) % 2; AppContext.SetSwitch(DisableTraversal, mode == 1);
            var before = Json.Deserialize<Dictionary<string, object>>(Json.Serialize(owners[mode].GetExpressionTraversalMetrics()));
            var timer = Stopwatch.StartNew();
            for (int iteration = 0; iteration < iterations; iteration++) documents[mode].ProcessExpressions();
            timer.Stop();
            var after = Json.Deserialize<Dictionary<string, object>>(Json.Serialize(owners[mode].GetExpressionTraversalMetrics()));
            long nodes = Number(after, "nodes") - Number(before, "nodes"), siblings = Number(after, "siblingTraversals") - Number(before, "siblingTraversals");
            Require(nodes == iterations * (1 + parts * 3) && (mode == 1 ? siblings == 0 : siblings == nodes), "Measured public traversal work/path mismatch");
            (mode == 1 ? legacy : optimized).Add(timer.ElapsedTicks);
            samples.Add(new { round, position, legacyFallback = mode == 1, iterations, elapsedTicks = timer.ElapsedTicks, elapsedMs = timer.Elapsed.TotalMilliseconds, nodes, siblingTraversals = siblings });
        }
        optimized.Sort(); legacy.Sort(); AppContext.SetSwitch(DisableTraversal, false);
        observations.Add(new { id = "bounded-public-expression-traversal-component-measurement", parts, iterations, rounds,
            medianOptimizedMs = optimized[rounds / 2] * 1000.0 / Stopwatch.Frequency, medianLegacyMs = legacy[rounds / 2] * 1000.0 / Stopwatch.Frequency,
            stopwatchFrequency = Stopwatch.Frequency, samples = samples.ToArray(),
            scope = "Same public ProcessExpressions on 769 standard DOM nodes and 1281 attributes with no expressions; two warmed independent contexts, seven alternating-order rounds. Includes evaluator creation and actual metrics recording; excludes DOM creation/reflection/serialization. No forced GC, speed assertion, allocation-byte or full CLI speedup claim" });
        tests.Add("bounded-public-expression-traversal-interleaved-component-timing-with-actual-node-path-counts");
    }
    static IDictionary<string, object> Metrics(DocumentProcessor owner)
    { return Json.Deserialize<Dictionary<string, object>>(Json.Serialize(ExpressionEvaluator.GetProgramMetrics(owner))); }
    static long Number(IDictionary<string, object> value, string key) { return Convert.ToInt64(value[key], CultureInfo.InvariantCulture); }
    static void Differential(string baseline)
    {
        var expected = Json.Deserialize<List<string[]>>(File.ReadAllText(baseline)); var specs = Specifications().ToArray();
        Require(expected.Count == specs.Length && expected.Count > 200, "Original expression corpus identity/count mismatch");
        var owner = Owner();
        for (int index = 0; index < expected.Count; index++)
        {
            var row = expected[index]; Require(row.Take(3).SequenceEqual(specs[index]), "Original expression specification changed");
            Thread.CurrentThread.CurrentCulture = new CultureInfo(row[0]);
            foreach (string mode in new[] { "first", "warm-cross-document", "fallback", "warm-per-value-culture", "legacy-all" })
            {
                AppContext.SetSwitch(Disable, mode == "fallback" || mode == "legacy-all");
                AppContext.SetSwitch(DisableCulture, mode == "warm-per-value-culture" || mode == "legacy-all");
                string[] actual = Outcome(row[0], row[1], row[2], Input(owner, row[1]));
                Require(actual.SequenceEqual(row), "Original/" + mode + " expression mismatch at " + index + ": " + Json.Serialize(row) + " != " + Json.Serialize(actual));
            }
        }
        AppContext.SetSwitch(Disable, false); AppContext.SetSwitch(DisableCulture, false); var metrics = Metrics(owner);
        Require(Number(metrics, "hits") > 0 && Number(metrics, "builds") > 0 && Number(metrics, "parserCalls") > 0, "Real parser/program replay was not exercised");
        observations.Add(new { id = "original-cold-warm-fallback-all-cultures", rows = expected.Count, metrics });
        tests.Add("isolated-original-DLL-vs-first-warm-parser-fallback-per-value-culture-and-legacy-all-full-outcome-and-read-state-differential-in-four-cultures");
    }
    static IDictionary<string, object>[] Reads(DependencyObservationLog log)
    {
        var report = Json.Deserialize<Dictionary<string, object>>(Json.Serialize(log.GetReport()));
        var result = new List<IDictionary<string, object>>();
        foreach (object value in (IEnumerable)report["observations"])
        { var row = (IDictionary<string, object>)value; if (Convert.ToString(row["kind"]) == "defineRead") result.Add(row); }
        return result.ToArray();
    }
    static void ReadAndBudgetCases()
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); AppContext.SetSwitch(Disable, false);
        var owner = Owner(); var expected = new[] { "21", "36", "36", "39" }; int position = 0;
        foreach (string scenario in new[] { "I7-local", "I12-local", "I12-external", "I13-external" })
        {
            var document = Input(owner, scenario); var log = new DependencyObservationLog();
            using (log.BeginScope(scenario)) Require(new ExpressionEvaluator(document).Evaluate("=$I+$I+$I") == expected[position++], "Program cached a Define value/provider");
            var rows = Reads(log); Require(rows.Length == 3 && rows.All(row => Convert.ToString(row["requestedName"]) == "I"), "Warm program skipped/reordered repeated actual LookupDefine reads");
            Require(rows.Select(row => Convert.ToString(row["value"])).Distinct().Single() == document.AllDefines["I"].EvaluatedValue, "Warm read evidence retained an old value");
            Require(rows.All(row => Convert.ToString(row["providerDocument"]) == document.AllDefines["I"].Document.SourcePath), "Warm program retained an old Define provider");
        }
        var metrics = Metrics(owner); Require(Number(metrics, "builds") == 1 && Number(metrics, "hits") == 3, "Exact programs were not shared across documents of one processor");
        var separate = Owner(); Require(Number(Metrics(separate), "entries") == 0, "A processor reused another processor's expression program cache");
        new ExpressionEvaluator(Input(separate, "standard")).Evaluate("=$I+$I+$I"); Require(Number(Metrics(separate), "builds") == 1 && Number(Metrics(separate), "hits") == 0, "New processor skipped its own original parse");
        long builds = Number(Metrics(owner), "builds"), parsers = Number(Metrics(owner), "parserCalls");
        for (int repeat = 0; repeat < 2; repeat++)
            Require(Outcome("en-US", "standard", "=$NEVER_VALID_PROGRAM", Input(owner, "standard"))[3].StartsWith("ERR:", StringComparison.Ordinal), "Missing Define failure was accepted");
        Require(Number(Metrics(owner), "builds") == builds && Number(Metrics(owner), "parserCalls") == parsers + 2, "A failed first parse was cached as a successful program");
        foreach (string expression in new[] { "=" + new string(' ', 17000) + "1", "=1" + String.Concat(Enumerable.Repeat("+1", 180)) })
        {
            var doc = Input(owner, "standard"); string cold = Outcome("en-US", "standard", expression, doc)[3];
            long priorBuilds = Number(Metrics(owner), "builds");
            Require(Outcome("en-US", "standard", expression, Input(owner, "standard"))[3] == cold && Number(Metrics(owner), "builds") == priorBuilds, "Oversize/deep program changed behavior or entered cache");
            AppContext.SetSwitch(Disable, true); Require(Outcome("en-US", "standard", expression, Input(owner, "standard"))[3] == cold, "Bounded fallback differs from original parser"); AppContext.SetSwitch(Disable, false);
        }
        for (int index = 0; index < 150; index++) new ExpressionEvaluator(Input(owner, "standard")).Evaluate("=" + index.ToString(CultureInfo.InvariantCulture) + "+1");
        metrics = Metrics(owner); Require(Number(metrics, "entries") <= 128 && Number(metrics, "retainedTextChars") <= 262144 && Number(metrics, "retainedNodes") <= 32768 && Number(metrics, "evictions") > 0, "Program cache exceeded its explicit bounds");
        observations.Add(new { id = "dynamic-reads-failure-and-bounds", metrics });
        tests.Add("actual-repeated-Define-consumption-value-and-source-changes-failure-not-cached-depth-text-and-entry-bounds-and-processor-isolation");
    }
    static string Balanced(int depth, string value)
    {
        if (depth == 0) return value;
        string child = Balanced(depth - 1, value); return "(" + child + "+" + child + ")";
    }
    static void TotalBudgetCases()
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        foreach (string budget in new[] { "text", "nodes" })
        {
            var owner = Owner();
            for (int index = 0; index < 40; index++)
            {
                string value = index.ToString(CultureInfo.InvariantCulture);
                string expression = budget == "text" ? "='" + index.ToString("D4", CultureInfo.InvariantCulture) + new string('x', 7796) + "'" : "=" + Balanced(10, value);
                AppContext.SetSwitch(Disable, false); string actual = new ExpressionEvaluator(Input(owner, "standard")).Evaluate(expression);
                AppContext.SetSwitch(Disable, true); string fallback = new ExpressionEvaluator(Input(owner, "standard")).Evaluate(expression);
                Require(actual == fallback && (budget == "text" ? actual.Length == 7800 : actual == (1024 * index).ToString(CultureInfo.InvariantCulture)), "Total-budget stress changed parser result: " + budget);
                var metrics = Metrics(owner);
                Require(Number(metrics, "entries") <= 128 && Number(metrics, "retainedTextChars") <= 262144 && Number(metrics, "retainedNodes") <= 32768, "A total program budget was exceeded during insertion: " + budget);
            }
            AppContext.SetSwitch(Disable, false); var final = Metrics(owner);
            Require(Number(final, "builds") == 40 && Number(final, "entries") < 128 && Number(final, "evictions") > 0, "Stress did not exercise its intended total-budget eviction: " + budget);
            if (budget == "text") Require(Number(final, "retainedNodes") < 128, "Text stress unexpectedly depended on node pressure");
            else Require(Number(final, "retainedTextChars") < 262144 / 2, "Balanced node stress unexpectedly depended on text pressure");
            observations.Add(new { id = "actual-total-" + budget + "-budget-eviction", metrics = final });
        }
        tests.Add("independent-total-text-and-total-node-budget-pressure-with-sub-limit-entries-and-balanced-depth10-programs-retain-fallback-results");
    }
    static void WarmFailures()
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); AppContext.SetSwitch(Disable, false);
        var owner = Owner(); var prime = Input(owner, "standard");
        prime.AllDefines["D"] = new Definition { Name = "D", EvaluatedValue = "2", Document = prime };
        const string expression = "=$I/$D+$I";
        Require(new ExpressionEvaluator(prime).Evaluate(expression) == "10.5", "Warm-failure program priming failed");
        foreach (string failure in new[] { "zero", "missing", "pending" })
        {
            string[] expected = null; string[] expectedReads = null;
            foreach (string mode in new[] { "cold", "warm", "fallback" })
            {
                AppContext.SetSwitch(Disable, mode == "fallback"); var document = Input(mode == "cold" ? Owner() : owner, "standard");
                if (failure != "missing") document.AllDefines["D"] = new Definition { Name = "D", EvaluatedValue = failure == "zero" ? "0" : null, Document = document };
                var log = new DependencyObservationLog(); string[] outcome;
                using (log.BeginScope(mode)) outcome = Outcome("en-US", "runtime-" + failure, expression, document);
                string[] reads = Reads(log).Select(row => Convert.ToString(row["requestedName"]) + "=" + Convert.ToString(row["value"]) + ":" + Convert.ToString(row["found"])).ToArray();
                Require(outcome[3].StartsWith("ERR:", StringComparison.Ordinal) && reads.Length == 2 && reads[0].StartsWith("I=", StringComparison.Ordinal) && reads[1].StartsWith("D=", StringComparison.Ordinal), "Runtime failure changed the original evaluated-read prefix");
                if (expected == null) { expected = outcome; expectedReads = reads; }
                else Require(outcome.SequenceEqual(expected) && reads.SequenceEqual(expectedReads), "Warm/fallback runtime failure differs from cold error or LookupDefine prefix");
            }
        }
        AppContext.SetSwitch(Disable, false); Require(Number(Metrics(owner), "replayFailures") >= 3, "Runtime program failures did not exercise actual replay");
        tests.Add("previously-successful-program-replays-current-Define-zero-missing-and-pending-failures-with-identical-first-error-and-read-prefix");
    }
    static void CultureOwnership()
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        var field = typeof(ExpressionValue).GetField("_CultureInfo", Hidden);
        bool prior; AppContext.TryGetSwitch(DisableCulture, out prior);
        try
        {
            AppContext.SetSwitch(DisableCulture, false);
            var sharedA = ExpressionValue.NewFloat("1.25"); var sharedB = ExpressionValue.NewFloat("1.25");
            var shared = (CultureInfo)field.GetValue(sharedA);
            Require(Object.ReferenceEquals(shared, field.GetValue(sharedB)) && shared.IsReadOnly && shared.NumberFormat.IsReadOnly, "Shared expression culture data is not immutable and shared");
            AppContext.SetSwitch(DisableCulture, true);
            var legacyA = ExpressionValue.NewFloat("1.25"); var legacyB = ExpressionValue.NewFloat("1.25");
            Require(!Object.ReferenceEquals(field.GetValue(legacyA), field.GetValue(legacyB)) && !Object.ReferenceEquals(shared, field.GetValue(legacyA)), "Per-value culture fallback shares mutable culture ownership");
            Require(sharedA.AsString() == legacyA.AsString() && sharedB.AsString() == legacyB.AsString(), "Culture ownership changed the original explicit English formatting");
        }
        finally { AppContext.SetSwitch(DisableCulture, prior); }
        tests.Add("shared-culture-readonly-ownership-and-independent-per-value-fallback-preserve-formatting");
    }
    static void InterleavedMicrobenchmark()
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        const string expression = "=round(($I+3)*2/3)+(($I*2)>=10)";
        const int iterations = 500, rounds = 7;
        string[] modes = { "program-shared-culture", "parser-shared-culture", "program-per-value-culture" };
        var owners = modes.ToDictionary(mode => mode, mode => Owner());
        var evaluators = modes.ToDictionary(mode => mode, mode => new ExpressionEvaluator(Input(owners[mode], "standard")));
        AppContext.SetSwitch(Disable, true); AppContext.SetSwitch(DisableCulture, false);
        string expected = evaluators["parser-shared-culture"].Evaluate(expression);
        // Warm every path independently in this process. Initial parses and all
        // metrics serialization are outside the measured intervals.
        for (int warm = 0; warm < 30; warm++)
            foreach (string mode in modes)
            {
                AppContext.SetSwitch(Disable, mode == "parser-shared-culture"); AppContext.SetSwitch(DisableCulture, mode == "program-per-value-culture");
                Require(evaluators[mode].Evaluate(expression) == expected, "Microbenchmark path warmup value changed");
            }
        var samples = new List<object>();
        for (int round = 0; round < rounds; round++)
            for (int position = 0; position < modes.Length; position++)
            {
                string mode = modes[(round + position) % modes.Length]; bool fallback = mode == "parser-shared-culture";
                AppContext.SetSwitch(Disable, fallback); AppContext.SetSwitch(DisableCulture, mode == "program-per-value-culture");
                var before = Metrics(owners[mode]); var timer = Stopwatch.StartNew();
                for (int iteration = 0; iteration < iterations; iteration++) Require(evaluators[mode].Evaluate(expression) == expected, "Timed expression value changed");
                timer.Stop(); var after = Metrics(owners[mode]);
                long parserCalls = Number(after, "parserCalls") - Number(before, "parserCalls"), hits = Number(after, "hits") - Number(before, "hits");
                Require(fallback ? parserCalls == iterations && hits == 0 : parserCalls == 0 && hits == iterations, "Timed batch did not execute the selected parser/program path");
                samples.Add(new { round, position, mode, iterations, elapsedMs = timer.Elapsed.TotalMilliseconds, parserCalls, hits });
            }
        AppContext.SetSwitch(Disable, false); AppContext.SetSwitch(DisableCulture, false);
        observations.Add(new { id = "interleaved-expression-program-parser-and-culture-microbenchmark", expression, expectedValue = expected, rounds, iterationsPerBatch = iterations,
            warmedPaths = true, rotatingOrder = true, samples = samples.ToArray(), modes = modes.Select(mode => new { mode, metrics = Metrics(owners[mode]) }).ToArray(),
            scope = "Three independently warmed same-process exact-expression paths; raw stopwatch samples include actual LookupDefine and result checks and separate parser/program from culture ownership; no OS I/O or complete CLI speedup claim" });
        tests.Add("same-process-seven-round-rotating-three-path-expression-and-culture-microbenchmark-preserves-every-result-and-records-parser-hit-deltas");
    }
    static Dictionary<string, string> Four(string directory)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (string path in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
        {
            string extension = Path.GetExtension(path); if (!new[] { ".manifest", ".bin", ".imp", ".relo" }.Contains(extension)) continue;
            using (var algorithm = SHA256.Create()) result.Add(extension, Convert.ToBase64String(algorithm.ComputeHash(File.ReadAllBytes(path))));
        }
        Require(result.Count == 4, "Expected four actual XML stream output files"); return result;
    }
    static void XmlDifferential(string work, string sdk)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        var config = new XmlDocument(); config.Load(Path.Combine(sdk, "tools/BinaryAssetBuilder.exe.config"));
        var settings = (Settings)new SerializedSectionHandler().Create(null, null, config.SelectSingleNode("/configuration/assetbuilder"));
        settings.DataRoot = work; settings.SchemaPath = Path.Combine(sdk, "Schemas/xsd/CnC3Types.xsd"); settings.InputPath = Path.Combine(work, "expressions.xml");
        settings.DefaultDataPaths = work; settings.DefaultArtPaths = work; settings.DefaultAudioPaths = work;
        settings.TargetPlatform = TargetPlatform.Win32; settings.BuildCache = false; settings.SessionCache = false; settings.LinkedStreams = true;
        settings.OutputAssetReport = false; settings.OutputStringHashes = false; settings.ErrorLevel = 1; settings.UsePrecompiled = false; settings.StreamHints = false; settings.StableSort = false;
        settings.OutputDirectory = Path.Combine(work, "initialize-output"); settings.IntermediateOutputDirectory = Path.Combine(work, "initialize-intermediate"); settings.SessionCacheDirectory = Path.Combine(work, "initialize-cache");
        Settings.Current = settings; SettingsLoader.PostProcessSettings(work);
        Directory.CreateDirectory(settings.OutputDirectory); Directory.CreateDirectory(settings.IntermediateOutputDirectory); Directory.CreateDirectory(settings.SessionCacheDirectory);
        string provider = Path.Combine(work, "provider.xml");
        var extraAssets = new StringBuilder();
        for (int i = 0; i < 64; i++) extraAssets.Append("<ArmorTemplate id='Lookup" + i + "' Default='=$A'/>");
        var extraDefines = new StringBuilder();
        for (int i = 0; i < 128; i++) extraDefines.Append("<Define name='D" + i.ToString("D3") + "' value='1'/>");
        File.WriteAllText(settings.InputPath, "<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Includes><Include type='all' source='provider.xml'/></Includes><Defines><Define name='B' value='=$A+1'/><Define name='C' value='=$A+1'/></Defines><ArmorTemplate id='ExpressionA' Default='=$B'/><ArmorTemplate id='ExpressionB' Default='=$C'/>" + extraAssets + "</AssetDeclaration>");
        using (var plugins = BuiltinCompilers.Create(TargetPlatform.Win32))
        {
            var schemas = new SchemaSet(false);
            foreach (int value in new[] { 10, 20 })
            {
                File.WriteAllText(provider, "<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Defines><Define name='A' value='" + value + "'/>" + extraDefines + "</Defines></AssetDeclaration>");
                Dictionary<string, string> expected = null;
                foreach (bool traversalFallback in new[] { false, true })
                foreach (bool fallback in new[] { false, true })
                foreach (bool defineFallback in new[] { false, true })
                {
                    AppContext.SetSwitch("NewSdk.Compiler.DisableDefineLookupIndex", defineFallback);
                    AppContext.SetSwitch(DisableTraversal, traversalFallback);
                    AppContext.SetSwitch(Disable, fallback); string label = value + (traversalFallback ? "-legacy-traversal" : "-optimized-traversal") + (fallback ? "-fallback" : "-program") + (defineFallback ? "-define-legacy" : "-define-index");
                    settings.OutputDirectory = Path.Combine(work, label, "output"); settings.IntermediateOutputDirectory = Path.Combine(work, label, "intermediate"); settings.SessionCacheDirectory = Path.Combine(work, label, "cache");
                    Directory.CreateDirectory(settings.OutputDirectory); Directory.CreateDirectory(settings.IntermediateOutputDirectory); Directory.CreateDirectory(settings.SessionCacheDirectory);
                    HashProvider.InitializeStringHashes(settings.SessionCacheDirectory); var cache = new SessionCache(); cache.InitializeCache(new List<string>());
                    var verifiers = new VerifierPluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32);
                    verifiers.DefaultPlugin = (IAssetBuilderVerifierPlugin)Activator.CreateInstance(typeof(DocumentProcessor).Assembly.GetType("BinaryAssetBuilder.GameDataVerifier.Plugin", true), true); verifiers.DefaultPlugin.Initialize(null, TargetPlatform.Win32);
                    var processor = new DocumentProcessor(settings, plugins.Registry, verifiers) { Cache = cache, SchemaSet = schemas };
                    bool succeeded; processor.ProcessDocument(settings.InputPath, true, false, out succeeded); Require(succeeded, "Real XML expression stream failed");
                    var outputs = Four(settings.OutputDirectory); var metrics = Metrics(processor);
                    if (expected == null) expected = outputs;
                    Require(expected.All(pair => outputs.ContainsKey(pair.Key) && outputs[pair.Key] == pair.Value), "Real XML expression four outputs differ across traversal/parser paths");
                    Require(fallback ? Number(metrics, "builds") == 0 : Number(metrics, "builds") > 0 && Number(metrics, "hits") > 0, "Actual XML/Define pipeline did not use selected expression program path");
                    var defineRows = (IEnumerable)Json.DeserializeObject(Json.Serialize(processor.GetDefineLookupMetrics()));
                    bool sawRoot = false;
                    foreach (IDictionary<string, object> row in defineRows)
                    {
                        if (!((string)row["sourcePath"]).EndsWith("expressions.xml", StringComparison.OrdinalIgnoreCase)) continue;
                        sawRoot = true; var scope = (IDictionary<string, object>)row["work"];
                        Require((bool)scope["scopeDisposed"] && Number(scope, "retainedIndexEntries") == 0 &&
                            (defineFallback ? Number(scope, "builds") == 0 : Number(scope, "builds") == 1 && Number(scope, "indexQueries") > 0), "Real Core did not use/dispose selected Define lookup scope");
                    }
                    Require(sawRoot, "Real Core Define lookup receipt omitted the root");
                    observations.Add(new { id = "real-XML-" + label, metrics, defineLookup = processor.GetDefineLookupMetrics(), traversal = processor.GetExpressionTraversalMetrics(), outputs });
                }
            }
        }
        AppContext.SetSwitch(Disable, false); AppContext.SetSwitch(DisableTraversal, false); AppContext.SetSwitch("NewSdk.Compiler.DisableDefineLookupIndex", false);
        tests.Add("real-SDK-schema-Armor-XML-imported-Define-change-and-repeat-expression-programs-vs-parser-fallback-four-output-byte-equality");
    }
    static void DefineLookupBudgetsAndMeasurement()
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        AppContext.SetSwitch("NewSdk.Compiler.DisableDefineLookupIndex", false);
        foreach (string mode in new[] { "small", "entries", "text", "warmup" })
        {
            var document = Bare(Owner(), "define-budget.xml");
            int count = mode == "entries" ? 65537 : mode == "small" ? 1 : 17;
            for (int i = 1; i < count; i++)
            {
                string key = mode == "text" ? new string('K', 65536) + i : "D" + i.ToString("D6");
                document.AllDefines[key] = new Definition { Name = key, EvaluatedValue = "1", Document = document };
            }
            document.AllDefines["Target"] = new Definition { Name = "Target", EvaluatedValue = "1", Document = document };
            var xml = new XmlDocument(); xml.LoadXml("<ArmorTemplate id='Budget'/>");
            for (int i = 0; i < (mode == "warmup" ? 1 : 64); i++) xml.DocumentElement.SetAttribute("P" + i, "=$Target");
            document.SelfInstances.Add(new InstanceDeclaration(document) { XmlNode = xml.DocumentElement }); document.ProcessExpressions();
            var metric = (IDictionary<string, object>)Json.DeserializeObject(Json.Serialize(document.GetDefineLookupMetrics()));
            string reason = mode == "text" ? "text-budget" : mode == "warmup" ? "query-warmup" : "entry-budget-or-small-scope";
            Require(Number(metric, "builds") == 0 && Number(metric, "indexQueries") == 0 && (string)metric["scopeReason"] == reason &&
                (bool)metric["scopeDisposed"] && Number(metric, "retainedIndexEntries") == 0, "Define lookup admission fallback changed: " + mode);
            observations.Add(new { id = "Define-lookup-budget-" + mode, metric });
        }
        var content = new System.Text.StringBuilder("<ArmorTemplate id='Measure'>");
        for (int i = 0; i < 512; i++) content.Append("<Part A='=$Target' B='=$Target' C='=$Target' D='=$Target' E='=$Target' F='=$Target' G='=$Target' H='=$Target'/>");
        content.Append("</ArmorTemplate>");
        var defaultTimes = new List<double>(); var fallbackTimes = new List<double>(); string expected = null;
        for (int round = 0; round < 7; round++)
        foreach (bool fallback in round % 2 == 0 ? new[] { false, true } : new[] { true, false })
        {
            AppContext.SetSwitch("NewSdk.Compiler.DisableDefineLookupIndex", fallback);
            var document = Bare(Owner(), "define-measure.xml");
            for (int i = 0; i < 128; i++) document.AllDefines["D" + i.ToString("D3")] = new Definition { Name = "D" + i.ToString("D3"), EvaluatedValue = "1", Document = document };
            document.AllDefines["Target"] = new Definition { Name = "Target", EvaluatedValue = "1", Document = document };
            var xml = new XmlDocument(); xml.LoadXml(content.ToString()); document.SelfInstances.Add(new InstanceDeclaration(document) { XmlNode = xml.DocumentElement });
            var timer = Stopwatch.StartNew(); document.ProcessExpressions(); timer.Stop();
            if (expected == null) expected = xml.OuterXml; Require(expected == xml.OuterXml, "Define lookup measured public traversal changed actual values");
            var metric = (IDictionary<string, object>)Json.DeserializeObject(Json.Serialize(document.GetDefineLookupMetrics()));
            Require(fallback ? Number(metric, "indexQueries") == 0 : Number(metric, "builds") == 1 && Number(metric, "indexQueries") > 4000,
                "Define lookup measurement did not use the requested path");
            (fallback ? fallbackTimes : defaultTimes).Add(timer.Elapsed.TotalMilliseconds);
        }
        AppContext.SetSwitch("NewSdk.Compiler.DisableDefineLookupIndex", false);
        observations.Add(new { id = "Define-lookup-public-expression-traversal", rounds = 7, reads = 4096, defaultMs = defaultTimes, fallbackMs = fallbackTimes,
            scope = "Interleaved same-process complete public ProcessExpressions; DOM construction excluded, parser/program behavior held equal; component measurement only, no complete CLI speedup claim" });
        tests.Add("Define-lookup-entry-text-small-query-budgets-and-seven-interleaved-real-expression-traversal-measurements");
    }
    static void DefineLookupDifferential(string baseline, string work, string sourceCommit, string runId)
    {
        var original = Json.Deserialize<Dictionary<string, object>>(File.ReadAllText(baseline));
        Require((bool)original["succeeded"] && (string)original["mode"] == "original" &&
            (string)original["sourceCommit"] == sourceCommit && (string)original["runId"] == runId, "Original Define lookup identity mismatch");
        var expected = new List<object>(); foreach (object row in (IEnumerable)original["defineLookups"]) expected.Add(row);
        Type helper = typeof(CompilerExpressionPrograms).Assembly.GetType("CompilerDocumentSnapshotOracle", true);
        foreach (bool disabled in new[] { false, true })
        {
            AppContext.SetSwitch("NewSdk.Compiler.DisableDefineLookupIndex", disabled);
            var actual = (object[])helper.GetMethod("DefineLookupCases").Invoke(null, new object[] { work });
            var measured = (object[])helper.GetMethod("GetDefineLookupCaseMetrics").Invoke(null, null);
            Require(actual.Length == 52 && expected.Count == actual.Length && measured.Length == actual.Length, "Expected 52 original Define/culture cases");
            for (int i = 0; i < actual.Length; i++)
            {
                Require(Json.Serialize(expected[i]) == Json.Serialize(actual[i]), "Original Define lookup mismatch: " + i + ": " + Json.Serialize(expected[i]) + " != " + Json.Serialize(actual[i]));
                var row = (IDictionary<string, object>)Json.DeserializeObject(Json.Serialize(actual[i]));
                Require((bool)row["changed"], "Define mutation fixture did not execute its actual DOM event: " + row["culture"] + "/" + row["mode"]);
                var record = (IDictionary<string, object>)Json.DeserializeObject(Json.Serialize(measured[i]));
                var metric = (IDictionary<string, object>)record["work"];
                Require((bool)metric["scopeDisposed"] && Number(metric, "retainedIndexEntries") == 0 && !(bool)metric["canAuthorizeReuse"], "Define scope escaped or claimed cache authority");
                string mode = (string)row["mode"], reason = (string)metric["scopeReason"];
                if (disabled) Require(Number(metric, "builds") == 0 && Number(metric, "indexQueries") == 0 && reason == "disabled", "Define fallback switch was ignored");
                else if (mode == "derived" || mode == "comparer") Require(Number(metric, "builds") == 0 && reason == "custom-definition-table-or-comparer", "Custom Define table/comparer was indexed");
                else if (mode == "prebuilt-culture" && !(bool)row["ordered"]) Require(Number(metric, "builds") == 0 && reason == "definition-key-order-changed", "Old-culture tree lookup was repaired by an index");
                else
                {
                    Require(Number(metric, "builds") == 1 && Number(metric, "indexQueries") > 0, "Actual final-expression Define lookup index was not used: " + mode);
                    if (mode == "replace" || mode == "remove-readd" || mode == "base-indexer" || mode == "failure")
                        Require(reason == "definition-table-version-changed" && Number(metric, "versionChecks") > Number(metric, "entries"), "Define mutation after guard EOF was missed: " + row["culture"] + "/" + mode + ": " + Json.Serialize(metric));
                    if (mode == "current-rebind" || mode == "table-rebind" || mode == "culture-change") Require(reason == "environment-or-culture-changed", "Changed Define environment retained its index: " + mode);
                    if (mode == "reenter") Require(reason == "reentrant-scope", "Reentrant Define traversal retained its index");
                }
            }
            observations.Add(new { id = "original-Define-lookup-scope", fallback = disabled, rows = actual.Length, metrics = measured });
        }
        AppContext.SetSwitch("NewSdk.Compiler.DisableDefineLookupIndex", false);
        tests.Add("original-Define-final-expression-scope-52-four-culture-key-equivalence-live-values-base-mutation-EOF-version-current-table-rebind-reentry-error-and-budget-fallback");
    }
    public static int Main(string[] args)
    {
        object result; int code = 0; var culture = Thread.CurrentThread.CurrentCulture; var settings = Settings.Current; bool disabled, cultureDisabled, traversalDisabled; AppContext.TryGetSwitch(Disable, out disabled); AppContext.TryGetSwitch(DisableCulture, out cultureDisabled); AppContext.TryGetSwitch(DisableTraversal, out traversalDisabled); bool defineDisabled; AppContext.TryGetSwitch("NewSdk.Compiler.DisableDefineLookupIndex", out defineDisabled);
        try
        {
            Require(args.Length == 7, "Expected work result sourceCommit runId sdk originalTranscript originalDocumentTranscript"); Settings.Current = new Settings { BuildCache = false, SessionCache = false };
            Differential(args[5]); TraversalDifferential(args[6], args[2], args[3]); DefineLookupDifferential(args[6], args[0], args[2], args[3]); DefineLookupBudgetsAndMeasurement(); TraversalMeasurement(); ReadAndBudgetCases(); TotalBudgetCases(); WarmFailures(); CultureOwnership(); InterleavedMicrobenchmark(); XmlDifferential(args[0], args[4]);
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), observations = observations.ToArray(), scope = "Exact immutable expression programs, original DLL differential and actual semantic read replay; actual SDK-schema XML four-output differentials; synthetic expression contexts do not replace full MOD/SDK acceptance" };
            Console.WriteLine("PASS ExpressionPrograms: " + tests.Count + " groups; " + checks + " checks");
        }
        catch (Exception error)
        { code = 1; Console.Error.WriteLine(error); result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null, runId = args.Length > 3 ? args[3] : null, checks, tests = tests.ToArray(), observations = observations.ToArray(), error = error.ToString() }; }
        finally { Settings.Current = settings; Thread.CurrentThread.CurrentCulture = culture; AppContext.SetSwitch(Disable, disabled); AppContext.SetSwitch(DisableCulture, cultureDisabled); AppContext.SetSwitch(DisableTraversal, traversalDisabled); AppContext.SetSwitch("NewSdk.Compiler.DisableDefineLookupIndex", defineDisabled); }
        if (args.Length > 1) File.WriteAllText(args[1], Json.Serialize(result)); return code;
    }
#endif
}
