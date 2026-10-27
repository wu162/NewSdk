using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
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
        File.WriteAllText(settings.InputPath, "<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Includes><Include type='all' source='provider.xml'/></Includes><Defines><Define name='B' value='=$A+1'/><Define name='C' value='=$A+1'/></Defines><ArmorTemplate id='ExpressionA' Default='=$B'/><ArmorTemplate id='ExpressionB' Default='=$C'/></AssetDeclaration>");
        using (var plugins = BuiltinCompilers.Create(TargetPlatform.Win32))
        {
            var schemas = new SchemaSet(false);
            foreach (int value in new[] { 10, 20 })
            {
                File.WriteAllText(provider, "<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Defines><Define name='A' value='" + value + "'/></Defines></AssetDeclaration>");
                Dictionary<string, string> expected = null;
                foreach (bool fallback in new[] { false, true })
                {
                    AppContext.SetSwitch(Disable, fallback); string label = value + (fallback ? "-fallback" : "-program");
                    settings.OutputDirectory = Path.Combine(work, label, "output"); settings.IntermediateOutputDirectory = Path.Combine(work, label, "intermediate"); settings.SessionCacheDirectory = Path.Combine(work, label, "cache");
                    Directory.CreateDirectory(settings.OutputDirectory); Directory.CreateDirectory(settings.IntermediateOutputDirectory); Directory.CreateDirectory(settings.SessionCacheDirectory);
                    HashProvider.InitializeStringHashes(settings.SessionCacheDirectory); var cache = new SessionCache(); cache.InitializeCache(new List<string>());
                    var verifiers = new VerifierPluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32);
                    verifiers.DefaultPlugin = (IAssetBuilderVerifierPlugin)Activator.CreateInstance(typeof(DocumentProcessor).Assembly.GetType("BinaryAssetBuilder.GameDataVerifier.Plugin", true), true); verifiers.DefaultPlugin.Initialize(null, TargetPlatform.Win32);
                    var processor = new DocumentProcessor(settings, plugins.Registry, verifiers) { Cache = cache, SchemaSet = schemas };
                    bool succeeded; processor.ProcessDocument(settings.InputPath, true, false, out succeeded); Require(succeeded, "Real XML expression stream failed");
                    var outputs = Four(settings.OutputDirectory); var metrics = Metrics(processor);
                    if (!fallback) { expected = outputs; Require(Number(metrics, "builds") > 0 && Number(metrics, "hits") > 0, "Actual XML/Define pipeline did not use exact expression programs"); }
                    else Require(expected.All(pair => outputs.ContainsKey(pair.Key) && outputs[pair.Key] == pair.Value) && Number(metrics, "builds") == 0, "Real XML expression four outputs differ from parser fallback");
                    observations.Add(new { id = "real-XML-" + label, metrics, outputs });
                }
            }
        }
        AppContext.SetSwitch(Disable, false);
        tests.Add("real-SDK-schema-Armor-XML-imported-Define-change-and-repeat-expression-programs-vs-parser-fallback-four-output-byte-equality");
    }
    public static int Main(string[] args)
    {
        object result; int code = 0; var culture = Thread.CurrentThread.CurrentCulture; var settings = Settings.Current; bool disabled, cultureDisabled; AppContext.TryGetSwitch(Disable, out disabled); AppContext.TryGetSwitch(DisableCulture, out cultureDisabled);
        try
        {
            Require(args.Length == 6, "Expected work result sourceCommit runId sdk originalTranscript"); Settings.Current = new Settings { BuildCache = false, SessionCache = false };
            Differential(args[5]); ReadAndBudgetCases(); TotalBudgetCases(); WarmFailures(); CultureOwnership(); InterleavedMicrobenchmark(); XmlDifferential(args[0], args[4]);
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), observations = observations.ToArray(), scope = "Exact immutable expression programs, original DLL differential and actual semantic read replay; actual SDK-schema XML four-output differentials; synthetic expression contexts do not replace full MOD/SDK acceptance" };
            Console.WriteLine("PASS ExpressionPrograms: " + tests.Count + " groups; " + checks + " checks");
        }
        catch (Exception error)
        { code = 1; Console.Error.WriteLine(error); result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null, runId = args.Length > 3 ? args[3] : null, checks, tests = tests.ToArray(), observations = observations.ToArray(), error = error.ToString() }; }
        finally { Settings.Current = settings; Thread.CurrentThread.CurrentCulture = culture; AppContext.SetSwitch(Disable, disabled); AppContext.SetSwitch(DisableCulture, cultureDisabled); }
        if (args.Length > 1) File.WriteAllText(args[1], Json.Serialize(result)); return code;
    }
#endif
}
