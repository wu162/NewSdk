using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder.Core;

public static class CompilerSymbolLookup
{
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;
    const string Disable = "NewSdk.Compiler.DisableSymbolLookupIndex";
    static readonly Type Helper = typeof(DocumentProcessor).Assembly.GetType("BinaryAssetBuilder.Core.CompilerSymbolLookup", true);
    static int checks;
    static readonly List<string> tests = new List<string>();
    static readonly List<object> evidence = new List<object>();
    static readonly JavaScriptSerializer Json = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
    static void Require(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    static object Current(AssetDeclarationDocument document) => typeof(AssetDeclarationDocument).GetField("_Current", Hidden).GetValue(document);
    static void Set(AssetDeclarationDocument document, string field, object value) => Current(document).GetType().GetField(field).SetValue(Current(document), value);
    static AssetDeclarationDocument Bare()
    {
        var document = new AssetDeclarationDocument();
        Type type = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object current = Activator.CreateInstance(type, true); type.GetMethod("FromScratch").Invoke(current, new object[0]);
        typeof(AssetDeclarationDocument).GetField("_Current", Hidden).SetValue(document, current);
        type.GetField("SourcePath").SetValue(current, "symbol-root.xml"); return document;
    }
    static InstanceDeclaration Declaration(AssetDeclarationDocument document, string id, string type = "ArmorTemplate")
    {
        var xml = new XmlDocument(); xml.LoadXml("<" + type + " id='" + id + "'/>");
        return new InstanceDeclaration(document) { XmlNode = xml.DocumentElement };
    }
    static object Scope(AssetDeclarationDocument document, bool safe = true)
    {
        Type workType = Helper.GetNestedType("Work", BindingFlags.NonPublic);
        object work = Activator.CreateInstance(workType, true);
        var sets = new[] { document.Instances, document.SelfInstances, document.AllInstances, document.TentativeInstances, document.ReferenceInstances };
        object scope = Helper.GetMethod("Create", Hidden).Invoke(null, new object[] { safe, sets, work });
        typeof(AssetDeclarationDocument).GetField("_SymbolLookup", Hidden).SetValue(document, scope);
        return work;
    }
    static IDictionary<string, object> Metrics(object work)
    { return Json.Deserialize<Dictionary<string, object>>(Json.Serialize(work.GetType().GetMethod("Snapshot", Hidden).Invoke(work, new object[0]))); }
    static long Number(IDictionary<string, object> metrics, string key) => Convert.ToInt64(metrics[key]);
    static void Populate(AssetDeclarationDocument document, int count = 64)
    {
        for (int index = 0; index < count; index++)
        {
            document.SelfInstances.Add(Declaration(document, "S" + index));
            document.AllInstances.Add(Declaration(document, "A" + index));
            document.TentativeInstances.Add(Declaration(document, "T" + index));
            document.ReferenceInstances.Add(Declaration(document, "R" + index));
        }
    }
    static void Differential()
    {
        var document = Bare(); Populate(document);
        foreach (string id in new[] { "S0", "A63", "T31", "R9", "Missing" })
        foreach (FindLocation skip in new[] { FindLocation.None, FindLocation.Self, FindLocation.All, FindLocation.Tentative, FindLocation.External })
        {
            var handle = new InstanceHandle("ArmorTemplate", id);
            AppContext.SetSwitch(Disable, true); object legacyWork = Scope(document);
            FindLocation expectedLocation; InstanceDeclaration expected = document.FindInstance(handle, skip, out expectedLocation);
            AppContext.SetSwitch(Disable, false); object work = Scope(document);
            FindLocation location; InstanceDeclaration actual = document.FindInstance(handle, skip, out location);
            Require(Object.ReferenceEquals(actual, expected) && location == expectedLocation, "Symbol location/skip/missing differential failed " + id + ":" + skip);
            Require(Number(Metrics(work), "indexQueries") > 0 && Number(Metrics(legacyWork), "indexQueries") == 0, "Indexed/legacy scope not consumed");
        }
        // The legacy lookup table's key remains authoritative when a value's
        // current Handle was replaced after insertion with a different object.
        InstanceDeclaration changed = document.SelfInstances[0]; var old = new InstanceHandle(changed.Handle);
        var xml = new XmlDocument(); xml.LoadXml("<ArmorTemplate id='Renamed'/>"); changed.XmlNode = xml.DocumentElement;
        object currentWork = Scope(document); FindLocation storedLocation;
        Require(Object.ReferenceEquals(document.FindInstance(old, FindLocation.None, out storedLocation), changed) && storedLocation == FindLocation.Self,
            "Index reconstructed dictionary keys from replacement value handles");
        evidence.Add(new { id = "full-key-location-and-stored-key", metrics = Metrics(currentWork) });
        tests.Add("full-type-instance-key-exact-legacy-location-precedence-skip-missing-and-dictionary-key-vs-live-value-handle-differential");
    }
    static void FullKeyCollisions()
    {
        var document = Bare(); Populate(document);
        var items = new List<InstanceDeclaration>();
        foreach (string ids in new[] { "2;0;3;0;ArmorTemplate;CollisionA", "3;0;2;0;ArmorTemplate;CollisionB" })
        {
            var declaration = Declaration(document, "placeholder");
            using (var reader = XmlReader.Create(new StringReader("<ih d='" + ids + "'/>"))) { reader.MoveToContent(); declaration.Handle.ReadXml(reader); }
            document.SelfInstances.Add(declaration); items.Add(declaration);
        }
        object work = Scope(document);
        foreach (var item in items)
        {
            FindLocation location;
            Require(Object.ReferenceEquals(item, document.FindInstance(new InstanceHandle(item.Handle), FindLocation.None, out location)) && location == FindLocation.Self,
                "Full symbol key collapsed an XOR collision");
        }
        Require(Number(Metrics(work), "indexQueries") == 2, "Full key collision fixture did not exercise indexed queries");
        tests.Add("complete-64bit-type-instance-key-keeps-two-equal-XOR-identities-distinct");
    }
    static void Measurement()
    {
        var document = Bare(); Populate(document, 512);
        var queries = Enumerable.Range(0, 512).Reverse().Select(index => new InstanceHandle("ArmorTemplate", "S" + index)).ToArray();
        var samples = new List<object>(); var indexed = new List<long>(); var legacy = new List<long>();
        for (int round = -1; round < 7; round++)
        foreach (bool disabled in (round % 2 == 0 ? new[] { false, true } : new[] { true, false }))
        {
            AppContext.SetSwitch(Disable, disabled);
            long start = System.Diagnostics.Stopwatch.GetTimestamp(); object work = Scope(document);
            int found = 0;
            for (int repeat = 0; repeat < 16; repeat++)
            foreach (InstanceHandle query in queries)
            {
                FindLocation location;
                if (document.FindInstance(query, FindLocation.None, out location) != null && location == FindLocation.Self) found++;
            }
            long ticks = System.Diagnostics.Stopwatch.GetTimestamp() - start;
            Require(found == 8192, "Timed public symbol lookup lost a result");
            if (round < 0) continue;
            (disabled ? legacy : indexed).Add(ticks);
            samples.Add(new { round, disabled, elapsedMs = ticks * 1000.0 / System.Diagnostics.Stopwatch.Frequency, metrics = Metrics(work) });
        }
        AppContext.SetSwitch(Disable, false); indexed.Sort(); legacy.Sort();
        evidence.Add(new { id = "bounded-public-symbol-component-measurement", declarations = 2048, queriesPerSample = 8192, rounds = 7,
            indexedMedianMs = indexed[3] * 1000.0 / System.Diagnostics.Stopwatch.Frequency,
            legacyMedianMs = legacy[3] * 1000.0 / System.Diagnostics.Stopwatch.Frequency, samples = samples.ToArray(),
            scope = "Includes scope admission/index building and public FindInstance calls; excludes fixture setup/reflection result inspection; no CLI speed or speed assertion" });
        tests.Add("interleaved-seven-round-public-FindInstance-timing-including-local-index-preparation-and-no-speed-assertion");
    }
    sealed class DerivedHandle : InstanceHandle { }
    sealed class DerivedSet : InstanceSet { internal static int HashCalls; public override int GetHashCode() { HashCalls++; return 17; } }
    static void Fallbacks()
    {
        foreach (string scenario in new[] { "disabled", "unsafe-owner", "small", "derived-set", "derived-query", "mutated-key-order", "external-boundary" })
        {
            var document = Bare(); Populate(document, scenario == "small" ? 1 : 64);
            if (scenario == "derived-set") { DerivedSet.HashCalls = 0; Set(document, "AllInstances", new DerivedSet()); }
            if (scenario == "mutated-key-order")
                document.SelfInstances.OrderBy(item => item.Handle.InstanceId).ElementAt(32).Handle.TypeName = "ZZChangedType";
            AppContext.SetSwitch(Disable, scenario == "disabled");
            object work = Scope(document, scenario != "unsafe-owner");
            if (scenario == "derived-set") Require(DerivedSet.HashCalls == 0, "Admission invoked custom set hashing before exact-type guard");
            object scope = typeof(AssetDeclarationDocument).GetField("_SymbolLookup", Hidden).GetValue(document);
            if (scenario == "external-boundary") Helper.GetMethod("ExternalBoundary", Hidden).Invoke(scope, new object[0]);
            InstanceHandle query = new InstanceHandle("ArmorTemplate", "S1");
            if (scenario == "derived-query") query = new DerivedHandle();
            FindLocation location; document.FindInstance(query, FindLocation.None, out location);
            var metrics = Metrics(work);
            Require(Number(metrics, "indexQueries") == 0 && Number(metrics, "legacyQueries") > 0, "Unsafe symbol query did not use legacy lookup: " + scenario + " " + Json.Serialize(metrics));
            evidence.Add(new { id = scenario, metrics });
        }
        AppContext.SetSwitch(Disable, false);
        tests.Add("disable-owner-small-derived-set-query-mutated-key-tree-and-external-boundary-retain-legacy-lookup");
    }
    static void CallbackInvalidation()
    {
        var document = Bare(); Populate(document); object work = Scope(document);
        FindLocation location; document.FindInstance(new InstanceHandle("ArmorTemplate", "S0"), FindLocation.None, out location);
        // Invoke the same tracer callback path that external handlers use.
        using (Tracer.BeginScope(delegate(string source, System.Diagnostics.TraceEventType kind, string text) { }, 3))
            Tracer.GetTracer("SymbolFixture", "scope mutation").TraceWarning("symbol callback invalidation");
        document.SelfInstances.Clear();
        Require(document.FindInstance(new InstanceHandle("ArmorTemplate", "S0"), FindLocation.None, out location) == null,
            "Callback-inactivated scope returned stale symbol");
        Require(Number(Metrics(work), "callbackFallbacks") == 1 && Number(Metrics(work), "legacyQueries") > 0, "Callback version did not permanently deactivate symbol scope");
        tests.Add("public-tracer-callback-version-change-permanently-deactivates-local-symbol-index");
    }
    public static int Main(string[] args)
    {
        object result; int code = 0; bool previous; AppContext.TryGetSwitch(Disable, out previous);
        try
        {
            Directory.CreateDirectory(args[0]); Differential(); FullKeyCollisions(); Fallbacks(); CallbackInvalidation(); Measurement();
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), evidence = evidence.ToArray(),
                scope = "Actual production symbol scope and public FindInstance lookup; full Core outputs and independent original BAB remain separate" };
            Console.WriteLine("PASS SymbolLookup: " + checks + " checks");
        }
        catch (Exception error)
        {
            code = 1; Console.Error.WriteLine(error); result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null,
                runId = args.Length > 3 ? args[3] : null, checks, tests = tests.ToArray(), error = error.ToString() };
        }
        finally { AppContext.SetSwitch(Disable, previous); }
        if (args.Length > 1) File.WriteAllText(args[1], Json.Serialize(result)); return code;
    }
}
