using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;

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
    public static int Main(string[] args)
    {
        int code = 0; object result; Settings previous = Settings.Current; bool previousSwitch; AppContext.TryGetSwitch(Switch, out previousSwitch);
        try
        {
            Require(args.Length == 5 && IntPtr.Size == 4 && Path.DirectorySeparatorChar == '\\', "Expected Windows x86 work result source run SDK");
            AppContext.SetSwitch(Switch, false); string work = Path.GetFullPath(args[0]); Directory.CreateDirectory(work);
            SearchCases(work); CrossTaskCases(work);
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), observations = observations.ToArray(),
                scope = "Resolver request/candidate shadow observations only: ordered actual existence checks, misses and task outcomes; no dependency-based invalidation, result caching or skipped legacy work" };
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
