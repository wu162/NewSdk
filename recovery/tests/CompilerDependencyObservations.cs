using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Xml;
using System.Web.Script.Serialization;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using NewModCompiler.Application;

public static class CompilerDependencyObservations
{
    const string Switch = "NewSdk.Compiler.DisableDependencyObservations";
    static int checks;
    static readonly List<string> tests = new List<string>();
    static readonly List<object> observations = new List<object>();
    static readonly JavaScriptSerializer Json = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
    static void Require(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static IDictionary<string, object> Map(object value) { return (IDictionary<string, object>)value; }
    static object[] Array(object value) { return ((IEnumerable)value).Cast<object>().ToArray(); }
    static string Text(object value) { return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture); }
    static long Number(IDictionary<string, object> value, string key) { return Convert.ToInt64(value[key]); }
    static IDictionary<string, object> ReportObject(object value) { return Json.Deserialize<Dictionary<string, object>>(Json.Serialize(value)); }
    static IDictionary<string, object> Report(DependencyObservationLog log) { return ReportObject(log.GetReport()); }
    static void Write(string path) { Directory.CreateDirectory(Path.GetDirectoryName(path)); File.WriteAllText(path, "resolver-positive-input"); }
    sealed class Outcome
    {
        public string selectedPath, errorType, errorCode, message;
    }
    static Outcome Resolve(DependencyObservationLog log, string task, string baseDirectory, string target)
    {
        using (log.BeginScope(task))
        {
            try { return new Outcome { selectedPath = FileNameResolver.ResolvePath(baseDirectory, target) }; }
            catch (BinaryAssetBuilderException error) { return new Outcome { errorType = error.GetType().FullName, errorCode = error.ErrorCode.ToString(), message = error.Message }; }
            catch (IOException error) { return new Outcome { errorType = error.GetType().FullName, errorCode = "IOException", message = error.Message }; }
        }
    }
    static void EqualFallback(Outcome enabled, string baseDirectory, string target)
    {
        AppContext.SetSwitch(Switch, true);
        try
        {
            var disabled = new DependencyObservationLog(); var result = Resolve(disabled, "fallback", baseDirectory, target);
            Require(Json.Serialize(result) == Json.Serialize(enabled), "Shadow observations changed selected path or refusal: " + target);
            var report = Report(disabled);
            Require(!Convert.ToBoolean(report["enabled"]) && Number(report, "recorded") == 0 && Array(report["observations"]).Length == 0,
                "Disabled shadow observations retained candidate events");
        }
        finally { AppContext.SetSwitch(Switch, false); }
    }
    static IDictionary<string, object>[] Events(DependencyObservationLog log)
    {
        var rows = Array(Report(log)["observations"]).Select(Map).ToArray();
        for (int index = 0; index < rows.Length; index++)
            Require(Number(rows[index], "sequence") == index + 1, "Observation order is not actual monotonically increasing execution order");
        return rows;
    }
    static void Trace(DependencyObservationLog log, string task, string baseDirectory, string target, string[] paths, bool[] found, string selected, string error = null)
    {
        var report = Report(log); var rows = Events(log);
        Require(rows.Length == paths.Length + 1 && Number(report, "resolutionCalls") == 1 && Number(report, "candidateChecks") == paths.Length,
            "Resolver observation did not preserve exact original candidate-check count: " + target);
        var result = rows.Single(row => Text(row["kind"]) == "resolve");
        Require(Text(result["taskId"]) == task && Text(result["baseDirectory"]) == baseDirectory && Text(result["targetPath"]) == target,
            "Resolver request lost task/base/target context");
        Require(Text(result["selectedPath"]) == Text(selected) && (error == null ? String.IsNullOrEmpty(Text(result["errorCode"])) : !String.IsNullOrEmpty(Text(result["errorCode"]))),
            "Resolver result/failure was not recorded after execution");
        var candidates = rows.Where(row => Text(row["kind"]) == "candidate").ToArray();
        for (int index = 0; index < paths.Length; index++)
            Require(Text(candidates[index]["candidatePath"]) == paths[index] && candidates[index]["found"] != null && Convert.ToBoolean(candidates[index]["found"]) == found[index] &&
                Text(candidates[index]["queryId"]) == Text(result["queryId"]) && Text(candidates[index]["taskId"]) == task &&
                Text(candidates[index]["baseDirectory"]) == baseDirectory && Text(candidates[index]["targetPath"]) == target,
                "Resolver candidate order, existence result or query correlation changed");
        Require(Number(report, "foundChecks") == found.Count(value => value) && Number(report, "missingChecks") == found.Count(value => !value),
            "Found/missing candidate counters differ from executed File.Exists calls");
        Require(Number(report, error == null ? "resolutionsSucceeded" : "resolutionsFailed") == 1,
            "Resolver terminal counter lost a success or failure");
        observations.Add(new { task, target, report });
    }
    static void SearchCases(string work)
    {
        string root = Path.Combine(work, "resolver"), source = Path.Combine(root, "source"), high = Path.Combine(root, "high"), low = Path.Combine(root, "low");
        Directory.CreateDirectory(source); Directory.CreateDirectory(high); Directory.CreateDirectory(low);
        Settings.Current = new Settings { DataRoot = root, DataPaths = new[] { high, low }, ArtPaths = new[] { high, low }, AudioPaths = new[] { high, low }, Postfix = "low" };
        foreach (string alias in new[] { "data", "audio" })
        {
            string name = alias + ".bin", first = Path.Combine(high, name), second = Path.Combine(low, name); Write(second);
            string target = alias + ":" + name; var log = new DependencyObservationLog(); var result = Resolve(log, alias, source, target);
            Require(result.selectedPath == second && result.errorType == null, "Legacy alias search did not select the lower-priority existing file");
            Trace(log, alias, source, target, new[] { first, second }, new[] { false, true }, second); EqualFallback(result, source, target);
        }
        string artName = "stone.dds", artHighPost = Path.Combine(high, "st", "stone_low.dds"), artHigh = Path.Combine(high, "st", artName), artLowPost = Path.Combine(low, "st", "stone_low.dds");
        Write(artLowPost); Write(Path.Combine(low, "st", artName));
        var art = new DependencyObservationLog(); var artResult = Resolve(art, "art-postfix", source, "art:" + artName);
        Require(artResult.selectedPath == artLowPost, "Legacy art postfix priority changed");
        Trace(art, "art-postfix", source, "art:" + artName, new[] { artHighPost, artHigh, artLowPost }, new[] { false, false, true }, artLowPost); EqualFallback(artResult, source, "art:" + artName);
        string missing = "missing.dds"; var miss = new DependencyObservationLog(); var missed = Resolve(miss, "art-missing", source, "art:" + missing);
        string last = Path.Combine(low, "mi", missing);
        Require(missed.selectedPath == last && !File.Exists(last), "A wholly missing art search must retain the historical final candidate path");
        Trace(miss, "art-missing", source, "art:" + missing,
            new[] { Path.Combine(high, "mi", "missing_low.dds"), Path.Combine(high, "mi", missing), Path.Combine(low, "mi", "missing_low.dds"), last },
            new[] { false, false, false, false }, last); EqualFallback(missed, source, "art:" + missing);
        string nested = Path.Combine(low, "folder", "nested.dds"); Write(nested);
        var nestedLog = new DependencyObservationLog(); var nestedResult = Resolve(nestedLog, "art-nested", source, "art:folder/nested.dds");
        Trace(nestedLog, "art-nested", source, "art:folder/nested.dds", new[] { Path.Combine(high, "folder", "nested.dds"), nested }, new[] { false, true }, nested);
        EqualFallback(nestedResult, source, "art:folder/nested.dds");
        tests.Add("real-data-audio-art-postfix-bucket-nested-missing-candidate-order-and-disabled-path-equivalence");
        Settings.Current.ArtPaths = new[] { "*", high }; Write(Path.Combine(source, "x"));
        var shortLog = new DependencyObservationLog(); var shortResult = Resolve(shortLog, "short-wildcard", source, "art:x");
        Trace(shortLog, "short-wildcard", source, "art:x", new[] { Path.Combine(source, "x_low"), Path.Combine(source, "x") }, new[] { false, true }, Path.Combine(source, "x"));
        EqualFallback(shortResult, source, "art:x");
        File.Delete(Path.Combine(source, "x")); var shortMiss = new DependencyObservationLog(); var shortFailure = Resolve(shortMiss, "short-refusal", source, "art:x");
        Require(shortFailure.errorCode == ErrorCode.IllegalPath.ToString(), "Art short-name refusal changed");
        Trace(shortMiss, "short-refusal", source, "art:x", new[] { Path.Combine(source, "x_low"), Path.Combine(source, "x") }, new[] { false, false }, null, shortFailure.errorCode);
        EqualFallback(shortFailure, source, "art:x");
        foreach (string target in new[] { "art:", "unknown:file", "data:a:b" })
        {
            var log = new DependencyObservationLog(); var result = Resolve(log, "refusal", source, target);
            Require(result.errorType != null, "Invalid resolver input was accepted"); Trace(log, "refusal", source, target, new string[0], new bool[0], null, result.errorCode); EqualFallback(result, source, target);
        }
        var invalidBase = new DependencyObservationLog(); var invalidResult = Resolve(invalidBase, "relative-base", "relative", "data:file");
        Require(invalidResult.errorCode == ErrorCode.InternalError.ToString(), "Relative source-directory refusal changed");
        Trace(invalidBase, "relative-base", "relative", "data:file", new string[0], new bool[0], null, invalidResult.errorCode); EqualFallback(invalidResult, "relative", "data:file");
        tests.Add("wildcard-short-name-success-and-later-refusal-empty-art-unknown-alias-multiple-colons-and-relative-base-equivalence");
        Settings.Current.DataPaths = new[] { low };
        foreach (string target in new[] { "data:single-missing.xml", "relative-missing.xml", Path.Combine(high, "absolute-missing.xml"), "root:root-missing.xml" })
        {
            var log = new DependencyObservationLog(); var result = Resolve(log, "no-probe", source, target);
            Require(result.errorType == null && result.selectedPath != null, "Direct/single-root resolver behavior changed");
            Trace(log, "no-probe", source, target, new string[0], new bool[0], result.selectedPath); EqualFallback(result, source, target);
        }
        Settings.Current.DataPaths = new string[0];
        var empty = new DependencyObservationLog(); var emptyResult = Resolve(empty, "empty-roots", source, "data:missing.xml");
        Require(emptyResult.errorType == null && emptyResult.selectedPath == null, "Empty root list must preserve the old null resolution");
        Trace(empty, "empty-roots", source, "data:missing.xml", new string[0], new bool[0], null); EqualFallback(emptyResult, source, "data:missing.xml");
        tests.Add("single-root-relative-absolute-root-and-empty-search-do-not-invent-extra-existence-probes");
        Settings.Current.DataPaths = new[] { high, low };
        using (CacheInputBoundary.Begin(low))
        {
            var boundary = new DependencyObservationLog(); var refusal = Resolve(boundary, "cache-boundary", source, "data:data.bin");
            Require(refusal.errorType == typeof(IOException).FullName, "Input boundary refusal changed");
            Trace(boundary, "cache-boundary", source, "data:data.bin", new string[0], new bool[0], null, refusal.errorCode);
            EqualFallback(refusal, source, "data:data.bin");
        }
        tests.Add("cache-input-boundary-still-refuses-before-any-outside-file-existence-query");
        var bounded = new DependencyObservationLog(0); var boundedResult = Resolve(bounded, "zero-budget", source, "data:data.bin");
        var boundedReport = Report(bounded);
        Require(boundedResult.selectedPath == Path.Combine(low, "data.bin") && Number(boundedReport, "recorded") == 0 && Number(boundedReport, "dropped") == 3 && Number(boundedReport, "candidateChecks") == 2,
            "Zero observation capacity skipped legacy candidate work or retained events");
        var noText = new DependencyObservationLog(8192, 0); var noTextResult = Resolve(noText, "zero-text-budget", source, "data:data.bin");
        var noTextReport = Report(noText);
        Require(noTextResult.selectedPath == Path.Combine(low, "data.bin") && Number(noTextReport, "recorded") == 0 && Number(noTextReport, "dropped") == 3 &&
            Number(noTextReport, "candidateChecks") == 2 && Number(noTextReport, "recordedTextChars") == 0 && Convert.ToBoolean(noTextReport["truncated"]),
            "Zero text capacity skipped legacy work, truncated a stored field or retained unbounded request text");
        observations.Add(new { id = "zero-text-budget", report = noTextReport });
        var limited = new DependencyObservationLog(2); Resolve(limited, "limited-first", source, "data:data.bin"); Resolve(limited, "limited-second", source, "data:data.bin");
        var limitedReport = Report(limited);
        Require(Number(limitedReport, "recorded") == 2 && Number(limitedReport, "dropped") == 4 && Number(limitedReport, "resolutionCalls") == 2 && Number(limitedReport, "candidateChecks") == 4,
            "Bounded observation storage changed repeated resolver work or grew unbounded");
        observations.Add(new { id = "zero-budget", report = boundedReport }); observations.Add(new { id = "bounded", report = limitedReport });
        tests.Add("finite-and-zero-shadow-storage-budgets-count-dropped-events-without-skipping-any-old-query");
    }
    static void CrossTaskCases(string work)
    {
        string root = Path.Combine(work, "two-task"), source = Path.Combine(root, "source"), high = Path.Combine(root, "high"), low = Path.Combine(root, "low");
        Directory.CreateDirectory(source); Directory.CreateDirectory(high); Directory.CreateDirectory(low); Write(Path.Combine(low, "changing.xml"));
        Settings.Current = new Settings { DataRoot = root, DataPaths = new[] { high, low }, ArtPaths = new[] { high, low }, AudioPaths = new[] { high, low } };
        var log = new DependencyObservationLog(); string target = "data:changing.xml";
        var first = Resolve(log, "flow-one", source, target); object prior = log.GetReport();
        var repeated = Resolve(log, "flow-one-repeat", source, target);
        Require(first.selectedPath == repeated.selectedPath && Number(Report(log), "candidateChecks") == 4, "Repeated shadow query reused a resolution result");
        Write(Path.Combine(high, "changing.xml")); var second = Resolve(log, "flow-two", source, target);
        Require(first.selectedPath == Path.Combine(low, "changing.xml") && second.selectedPath == Path.Combine(high, "changing.xml"), "Legacy resolver did not naturally select a newly added higher-priority candidate");
        var rows = Events(log); var highCandidates = rows.Where(row => Text(row["kind"]) == "candidate" && Text(row["candidatePath"]) == Path.Combine(high, "changing.xml")).ToArray();
        Require(highCandidates.Length == 3 && !Convert.ToBoolean(highCandidates[0]["found"]) && !Convert.ToBoolean(highCandidates[1]["found"]) && Convert.ToBoolean(highCandidates[2]["found"]),
            "Shadow observations lost negative-to-positive candidate changes");
        Require(highCandidates.Select(row => Text(row["taskId"])).SequenceEqual(new[] { "flow-one", "flow-one-repeat", "flow-two" }) && highCandidates.Select(row => Text(row["queryId"])).Distinct().Count() == 3,
            "Repeated/cross-task candidate observations lost request identity");
        Require(Number(ReportObject(prior), "resolutionCalls") == 1 && Array(ReportObject(prior)["observations"]).Length == 3,
            "A previously returned shadow report changed after later tasks");
        EqualFallback(second, source, target); observations.Add(new { id = "cross-task-negative-to-positive", report = Report(log) });
        tests.Add("repeated-and-cross-task-negative-to-positive-candidates-always-requery-and-return-immutable-contextual-observations");
    }
    static AssetDeclarationDocument BareDocument(string source)
    {
        // Exercise actual public Core/evaluator methods with a small in-memory
        // source context; only private state construction avoids an unrelated parse.
        var document = new AssetDeclarationDocument();
        Type currentType = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object current = Activator.CreateInstance(currentType, true);
        currentType.GetMethod("FromScratch").Invoke(current, new object[0]);
        currentType.GetField("SourcePath").SetValue(current, source);
        currentType.GetField("SourceDirectory").SetValue(current, Path.GetDirectoryName(source));
        typeof(AssetDeclarationDocument).GetField("_Current", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(document, current);
        return document;
    }
    static InstanceDeclaration BareInstance(AssetDeclarationDocument document, string type, string id)
    {
        var xml = new XmlDocument(); xml.LoadXml("<" + type + " id='" + id + "'/>");
        return new InstanceDeclaration(document) { XmlNode = xml.DocumentElement };
    }
    static void DefineCases(string work)
    {
        var imported = BareDocument(Path.Combine(work, "define", "external.xml"));
        var document = BareDocument(Path.Combine(work, "define", "root.xml"));
        var provider = new Definition { Name = "A", EvaluatedValue = "10", Document = imported };
        document.AllDefines["A"] = provider;
        document.SelfDefines.Add(new Definition { Name = "A", OriginalValue = "=$A+1", IsOverride = true, Document = document });
        var log = new DependencyObservationLog();
        using (log.BeginScope("actual-define-override"))
        {
            document.EvaluateDefinitions();
            var evaluator = new BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator(document);
            Require(evaluator.Evaluate("=$A") == "11", "Local override did not retain the actual pre-installation imported read");
            foreach (string missing in new[] { "MISSING", "PENDING" })
            {
                if (missing == "PENDING") document.AllDefines[missing] = new Definition { Name = missing, Document = imported };
                try { evaluator.Evaluate("=$" + missing); throw new Exception("Unknown define was accepted"); }
                catch (BinaryAssetBuilderException error) { Require(error.ErrorCode == ErrorCode.ExpressionEvaluationError, "Define read changed legacy error code"); }
            }
            var instance = BareInstance(document, "ArmorTemplate", "DefineOwner");
            ((XmlElement)instance.XmlNode).SetAttribute("Default", "=$A"); document.SelfInstances.Add(instance);
            document.ProcessExpressions(); Require(instance.XmlNode.Attributes["Default"].Value == "11", "Actual per-asset expression changed");
        }
        var reads = Events(log).Where(row => Text(row["kind"]) == "defineRead").ToArray();
        Require(reads.Length == 5 && Number(Report(log), "defineReads") == 5, "Actual LookupDefine consumption did not produce one observation per old lookup");
        Require(Text(reads[0]["providerDocument"]) == imported.SourcePath && Text(reads[0]["value"]) == "10" && Number(reads[0], "scopeEpoch") == 0 &&
            Text(reads[0]["stage"]) == "definitions" && Text(reads[0]["readSite"]) == "Define:A", "Define override observation captured the post-installation binding");
        Require(Text(reads[1]["providerDocument"]) == document.SourcePath && Text(reads[1]["value"]) == "11" && Number(reads[1], "scopeEpoch") == 1,
            "Post-installation Define read lost source/value/scope epoch");
        Require(!Convert.ToBoolean(reads[2]["found"]) && String.IsNullOrEmpty(Text(reads[2]["providerDocument"])) &&
            !Convert.ToBoolean(reads[3]["found"]) && Text(reads[3]["providerDocument"]) == imported.SourcePath,
            "Missing and unevaluated Define bindings were conflated");
        Require(Text(reads[4]["stage"]) == "expressions" && Text(reads[4]["readSite"]).StartsWith("ArmorTemplate:DefineOwner/") && Text(reads[4]["readSite"]).EndsWith("/@Default"), "Asset expression read lost its actual attribute read site");
        object prior = log.GetReport(); string immutable = Json.Serialize(prior); provider.EvaluatedValue = "mutated"; provider.Name = "mutated";
        Require(Json.Serialize(prior) == immutable, "Define observations retained mutable Definition state");
        var equalProvider = BareDocument(Path.Combine(work, "define", "equal-value-other-source.xml"));
        document.AllDefines["A"] = new Definition { Name = "A", EvaluatedValue = "11", Document = equalProvider };
        using (log.BeginScope("same-value-new-provider")) Require(new BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator(document).Evaluate("=$A") == "11", "Same-value source change altered expression");
        var changed = Events(log).Last(); Require(Text(changed["value"]) == "11" && Text(changed["providerDocument"]) == equalProvider.SourcePath,
            "Same-value binding changes must retain their new provider evidence");
        foreach (var budget in new[] { new DependencyObservationLog(0), new DependencyObservationLog(8192, 0) })
        {
            using (budget.BeginScope("bounded-semantic-read")) Require(new BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator(document).Evaluate("=$A+$A") == "22", "Bounded storage changed actual Define evaluation");
            Require(Number(Report(budget), "defineReads") == 2 && Number(Report(budget), "recorded") == 0 && Number(Report(budget), "dropped") == 2,
                "Semantic evidence ignored the existing shared row/text budget");
        }
        AppContext.SetSwitch(Switch, true);
        try
        {
            var disabled = new DependencyObservationLog();
            using (disabled.BeginScope("disabled-define")) Require(new BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator(document).Evaluate("=$A") == "11", "Disabled Define observation changed legacy evaluation");
            Require(Number(Report(disabled), "defineReads") == 0 && Number(Report(disabled), "recorded") == 0, "Disabled semantic observation retained reads");
        }
        finally { AppContext.SetSwitch(Switch, false); }
        observations.Add(new { id = "actual-define-bindings", report = Report(log) });
        tests.Add("actual-evaluator-preoverride-postoverride-missing-unevaluated-attribute-site-same-value-new-provider-immutable-bounded-disabled");
    }
    static void SymbolCases(string work)
    {
        var document = BareDocument(Path.Combine(work, "symbols", "root.xml"));
        var provider = BareDocument(Path.Combine(work, "symbols", "external.xml"));
        var self = BareInstance(document, "ArmorTemplate", "Target");
        var all = BareInstance(provider, "ArmorTemplate", "Target");
        var tentative = BareInstance(provider, "ArmorTemplate", "Tentative");
        var external = BareInstance(provider, "ArmorTemplate", "External");
        document.SelfInstances.Add(self); document.AllInstances.Add(all); document.TentativeInstances.Add(tentative); document.ReferenceInstances.Add(external);
        var log = new DependencyObservationLog();
        using (log.BeginScope("actual-symbol-sets"))
        {
            FindLocation location;
            Require(Object.ReferenceEquals(document.FindInstance(self.Handle, FindLocation.None, out location), self) && location == FindLocation.Self, "Self symbol selection changed");
            Require(Object.ReferenceEquals(document.FindInstance(self.Handle, FindLocation.Self, out location), all) && location == FindLocation.All, "Skipped Self symbol selection changed");
            Require(Object.ReferenceEquals(document.FindInstance(tentative.Handle, FindLocation.None, out location), tentative) && location == FindLocation.Tentative, "Tentative symbol selection changed");
            Require(Object.ReferenceEquals(document.FindInstance(external.Handle, FindLocation.None, out location), external) && location == FindLocation.External, "External symbol selection changed");
            Require(document.FindInstance(new InstanceHandle("ArmorTemplate", "Absent"), FindLocation.None, out location) == null && location == FindLocation.None, "Missing symbol selection changed");
        }
        var rows = Events(log); var candidates = rows.Where(row => Text(row["kind"]) == "symbolCandidate").ToArray();
        Require(Number(Report(log), "symbolLookups") == 5 && Number(Report(log), "symbolCandidateChecks") == 13 && candidates.Length == 13,
            "Symbol lookup added probes, failed to skip a set, or omitted an actual attempted set");
        Require(candidates.Select(row => Text(row["providerLocation"])).SequenceEqual(new[] { "Self", "All", "Self", "All", "Tentative", "Self", "All", "Tentative", "External", "Self", "All", "Tentative", "External" }),
            "Symbol candidate evidence lost original set priority or skip behavior");
        Require(candidates.Count(row => Convert.ToBoolean(row["found"])) == 4 && rows.Last()["found"].Equals(false), "Positive/negative symbol queries were not retained");
        string prior = Json.Serialize(log.GetReport()); self.Handle.TypeName = "Changed";
        Require(Json.Serialize(log.GetReport()) == prior, "Symbol report retained a mutable InstanceHandle");
        observations.Add(new { id = "actual-symbol-set-queries", report = Report(log) });
        tests.Add("actual-Core-symbol-priority-skip-all-negative-set-evidence-and-mutable-handle-snapshot");
    }
    const string Ns = "uri:ea.com:eala:asset";
    static string Asset(string body) { return "<AssetDeclaration xmlns='" + Ns + "'>" + body + "</AssetDeclaration>"; }
    static string WriteXml(string path, string body) { Directory.CreateDirectory(Path.GetDirectoryName(path)); File.WriteAllText(path, Asset(body)); return path; }
    static Settings ConfigureCore(string sdk, string root, string name)
    {
        var config = new XmlDocument(); config.Load(Path.Combine(sdk, "tools/BinaryAssetBuilder.exe.config"));
        var settings = (Settings)new SerializedSectionHandler().Create(null, null, config.SelectSingleNode("/configuration/assetbuilder"));
        settings.DataRoot = root; settings.SchemaPath = Path.Combine(sdk, "Schemas/xsd/CnC3Types.xsd");
        settings.InputPath = Path.Combine(root, name, "root.xml"); settings.OutputDirectory = Path.Combine(root, name, "output");
        settings.IntermediateOutputDirectory = Path.Combine(root, name, "intermediate"); settings.SessionCacheDirectory = Path.Combine(root, name, "cache");
        settings.DefaultDataPaths = root; settings.DefaultArtPaths = root; settings.DefaultAudioPaths = root;
        settings.TargetPlatform = TargetPlatform.Win32; settings.BuildCache = false; settings.SessionCache = false; settings.LinkedStreams = true;
        settings.OutputAssetReport = false; settings.OutputStringHashes = false; settings.ErrorLevel = 1; settings.UsePrecompiled = false; settings.StreamHints = false; settings.StableSort = false;
        Settings.Current = settings; SettingsLoader.PostProcessSettings(root);
        Directory.CreateDirectory(settings.OutputDirectory); Directory.CreateDirectory(settings.IntermediateOutputDirectory); Directory.CreateDirectory(settings.SessionCacheDirectory);
        return settings;
    }
    static DocumentProcessor Processor(BuiltinCompilers plugins)
    {
        var schemas = new SchemaSet(false); HashProvider.InitializeStringHashes(Settings.Current.SessionCacheDirectory);
        var cache = new SessionCache(); cache.InitializeCache(new List<string>());
        var verifiers = new VerifierPluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32);
        verifiers.DefaultPlugin = (IAssetBuilderVerifierPlugin)Activator.CreateInstance(typeof(DocumentProcessor).Assembly.GetType("BinaryAssetBuilder.GameDataVerifier.Plugin", true), true);
        verifiers.DefaultPlugin.Initialize(null, TargetPlatform.Win32);
        return new DocumentProcessor(Settings.Current, plugins.Registry, verifiers) { Cache = cache, SchemaSet = schemas };
    }
    static void Run(DocumentProcessor processor, string input) { bool success; processor.ProcessDocument(input, true, false, out success); Require(success, "Actual Core pipeline failed: " + input); }
    static Dictionary<string, string> Outputs(string root)
    {
        var result = new Dictionary<string, string>();
        foreach (string path in Directory.GetFiles(root, "*", SearchOption.AllDirectories))
            if (new[] { ".manifest", ".bin", ".imp", ".relo" }.Contains(Path.GetExtension(path)))
                using (var sha = SHA256.Create()) result[path.Substring(root.Length)] = BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path)));
        return result;
    }
    static string Experience(string id, string upgrade)
    {
        return "<ExperienceLevelTemplate id='" + id + "'><LevelUpTintColor r='1.0' g='1.0' b='1.0'/>" + (upgrade == null ? "" : "<Upgrade>" + upgrade + "</Upgrade>") + "</ExperienceLevelTemplate>";
    }
    static Outcome ResolveSymbol(AssetDeclarationDocument document, InstanceDeclaration parent, InstanceHandle requested, bool weak)
    {
        var method = typeof(AssetDeclarationDocument).GetMethod("ResolveReference", BindingFlags.NonPublic | BindingFlags.Instance);
        object[] arguments = { parent, requested, FindLocation.None, weak };
        try
        {
            var selected = (InstanceDeclaration)method.Invoke(document, arguments);
            return new Outcome { selectedPath = selected == null ? null : selected.Handle.Name + "@" + arguments[2] };
        }
        catch (TargetInvocationException invocation)
        {
            var error = invocation.InnerException as BinaryAssetBuilderException;
            if (error == null) throw;
            return new Outcome { errorType = error.GetType().FullName, errorCode = error.ErrorCode.ToString(), message = error.Message };
        }
    }
    static void DerivedSymbolCases(string work, DocumentProcessor processor)
    {
        var document = BareDocument(Path.Combine(work, "derived", "root.xml"));
        var current = typeof(AssetDeclarationDocument).GetField("_Current", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(document);
        current.GetType().GetField("DocumentProcessor").SetValue(current, processor);
        var parent = BareInstance(document, "ExperienceLevelTemplate", "Owner");
        var first = BareInstance(document, "ArmorTemplate", "DerivedTarget");
        document.TentativeInstances.Add(first);
        var request = new InstanceHandle("BaseInheritableAsset", "DerivedTarget");
        string[] derivedOrder = processor.SchemaSet.GetDerivedTypes(request.TypeName).Cast<string>().ToArray();
        Require(derivedOrder.Contains("ArmorTemplate") && derivedOrder.Contains("UpgradeTemplate"), "Real schema fixture lacks expected derived candidates");
        var log = new DependencyObservationLog(65536, 8 * 1048576);
        Outcome single;
        using (log.BeginScope("derived-unique")) single = ResolveSymbol(document, parent, request, false);
        Require(single.selectedPath == "ArmorTemplate:DerivedTarget@Tentative" && single.errorCode == null, "Actual derived lookup lost its unique candidate");
        var rows = Events(log);
        var lookups = rows.Where(row => Text(row["kind"]) == "symbolLookup" && Text(row["stage"]) == "strongReference:derived").ToArray();
        Require(lookups.Select(row => Text(row["requestedType"])).SequenceEqual(derivedOrder), "Derived candidates were reordered, deduplicated, or extra candidates were queried");
        long parentQuery = Number(rows.Last(), "queryId");
        Require(lookups.All(row => Number(row, "parentQueryId") == parentQuery), "Derived lookup evidence lost its owning reference-selection query");
        Require(rows.Any(row => Text(row["kind"]) == "symbolLookup" && Text(row["stage"]) == "strongReference:exact" && !Convert.ToBoolean(row["found"])) &&
            lookups.Count(row => Convert.ToBoolean(row["found"])) == 1, "Exact missing and derived successful binding evidence were conflated");
        document.TentativeInstances.Add(BareInstance(document, "UpgradeTemplate", "DerivedTarget"));
        Outcome ambiguous;
        using (log.BeginScope("derived-ambiguous")) ambiguous = ResolveSymbol(document, parent, request, false);
        Require(ambiguous.errorCode == ErrorCode.ReferencingError.ToString(), "Two derived candidates must preserve original ambiguity refusal");
        var failed = Events(log).Last();
        Require(Text(failed["kind"]) == "symbolSelection" && Text(failed["errorCode"]) == ErrorCode.ReferencingError.ToString() && !Convert.ToBoolean(failed["found"]),
            "Derived ambiguity terminal outcome was lost");
        AppContext.SetSwitch(Switch, true);
        try
        {
            var disabled = new DependencyObservationLog(); Outcome fallback;
            using (disabled.BeginScope("derived-ambiguous-disabled")) fallback = ResolveSymbol(document, parent, request, false);
            Require(Json.Serialize(fallback) == Json.Serialize(ambiguous) && Number(Report(disabled), "recorded") == 0, "Symbol observations changed legacy derived ambiguity details");
        }
        finally { AppContext.SetSwitch(Switch, false); }
        observations.Add(new { id = "real-schema-derived-unique-and-ambiguity", report = Report(log) });
        tests.Add("actual-Core-real-schema-exact-negative-derived-order-unique-then-ambiguous-with-legacy-error-equivalence");
    }
    static void PipelineCases(string work, string sdk)
    {
        string root = Path.Combine(work, "semantic-pipeline"); ConfigureCore(sdk, root, "initialize");
        using (var plugins = BuiltinCompilers.Create(TargetPlatform.Win32))
        {
            string[] expectedOutputs = null;
            foreach (bool disabled in new[] { false, true })
            {
                var settings = ConfigureCore(sdk, root, "inheritance");
                if (disabled)
                {
                    settings.OutputDirectory = Path.Combine(root, "inheritance", "disabled-output");
                    settings.IntermediateOutputDirectory = Path.Combine(root, "inheritance", "disabled-intermediate");
                    Directory.CreateDirectory(settings.OutputDirectory); Directory.CreateDirectory(settings.IntermediateOutputDirectory);
                }
                string external = WriteXml(Path.Combine(root, "external.xml"), "<ArmorTemplate id='ExternalBase' Default='72.5%' />");
                string input = WriteXml(settings.InputPath, "<Includes><Include type='instance' source='" + external.Replace('\\', '/') + "'/></Includes>" +
                    "<ArmorTemplate id='LocalBase' Default='61.25%'/>" +
                    "<ArmorTemplate id='LocalChild' inheritFrom='LocalBase'/>" +
                    "<ArmorTemplate id='ExternalChild' inheritFrom='ExternalBase'/>");
                AppContext.SetSwitch(Switch, disabled); var log = new DependencyObservationLog();
                var processor = Processor(plugins); using (log.BeginScope("real-inheritance")) Run(processor, input);
                var outputs = Outputs(settings.OutputDirectory).OrderBy(pair => pair.Key).Select(pair => pair.Value).ToArray();
                Require(outputs.Length == 4 && (expectedOutputs == null || expectedOutputs.SequenceEqual(outputs)), "Semantic observations changed actual four-file Core inheritance output");
                expectedOutputs = outputs;
                if (!disabled)
                {
                    var templates = Events(log).Where(row => Text(row["kind"]) == "inheritanceTemplate").ToArray();
                    Require(templates.Any(row => Text(row["templateStage"]) == "SameDocumentPrevalidation" && Text(row["providerInstance"]) == "LocalBase") &&
                        templates.Any(row => Text(row["templateStage"]) == "IncludedDocumentProcessed" && Text(row["providerInstance"]) == "ExternalBase"),
                        "Actual same/cross-document inherited template consumption lost its stage/source");
                    observations.Add(new { id = "real-Core-inheritance", report = Report(log) });
                }
                else Require(Number(Report(log), "recorded") == 0, "Disabled real-Core semantic observations retained records");
                AppContext.SetSwitch(Switch, false);
            }
            var binding = ConfigureCore(sdk, root, "binding"); var sharedProcessor = Processor(plugins); var bindingLog = new DependencyObservationLog();
            WriteXml(Path.Combine(root, "binding", "shared.xml"), Experience("Owner", "LateUpgrade"));
            WriteXml(Path.Combine(root, "binding", "candidate.xml"), "<UpgradeTemplate id='LateUpgrade'/>");
            string first = WriteXml(Path.Combine(root, "binding", "first.xml"), "<Includes><Include type='all' source='shared.xml'/></Includes>");
            string second = WriteXml(Path.Combine(root, "binding", "second.xml"), "<Includes><Include type='all' source='shared.xml'/><Include type='instance' source='candidate.xml'/></Includes>");
            using (bindingLog.BeginScope("missing-weak")) Run(sharedProcessor, first);
            using (bindingLog.BeginScope("present-weak")) Run(sharedProcessor, second);
            var selected = Events(bindingLog).Where(row => Text(row["kind"]) == "symbolSelection" && Text(row["stage"]) == "weakReference" && Text(row["requestedInstance"]) == "LateUpgrade").ToArray();
            Require(selected.Any(row => Text(row["taskId"]) == "missing-weak" && !Convert.ToBoolean(row["found"])) &&
                selected.Any(row => Text(row["taskId"]) == "present-weak" && Convert.ToBoolean(row["found"]) && Text(row["providerLocation"]) == "Tentative"),
                "Actual cross-stream missing weak reference lost its negative-to-Tentative evidence");
            observations.Add(new { id = "real-Core-cross-stream-weak", report = Report(bindingLog) });
            DerivedSymbolCases(work, sharedProcessor);
        }
        tests.Add("real-Core-inheritance-template-stages-and-fallback-four-output-equivalence-plus-cross-stream-missing-weak-to-Tentative");
    }
    public static int Main(string[] args)
    {
        int code = 0; object result; Settings previous = Settings.Current; bool previousSwitch; AppContext.TryGetSwitch(Switch, out previousSwitch);
        try
        {
            Require(args.Length == 5 && IntPtr.Size == 4 && Path.DirectorySeparatorChar == '\\', "Expected Windows x86 work result source run SDK");
            AppContext.SetSwitch(Switch, false); string work = Path.GetFullPath(args[0]); Directory.CreateDirectory(work);
            SearchCases(work); CrossTaskCases(work); DefineCases(work); SymbolCases(work); PipelineCases(work, Path.GetFullPath(args[4]));
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), observations = observations.ToArray(),
                scope = "Resolver candidates plus actual expression Define reads, inherited template stages and per-set symbol candidate/selection shadow evidence; no dependency-based invalidation, result caching or skipped legacy work" };
            Console.WriteLine("PASS DependencyObservations: " + tests.Count + " groups; " + checks + " checks");
        }
        catch (Exception error)
        {
            code = 1; Console.Error.WriteLine(error); result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null,
                runId = args.Length > 3 ? args[3] : null, checks, tests = tests.ToArray(), observations = observations.ToArray(), error = error.ToString() };
        }
        finally { Settings.Current = previous; AppContext.SetSwitch(Switch, previousSwitch); }
        if (args.Length > 1) File.WriteAllText(args[1], Json.Serialize(result)); return code;
    }
}
