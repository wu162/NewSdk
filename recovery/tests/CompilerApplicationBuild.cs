using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Web.Script.Serialization;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using NewModCompiler.Application;
using NewModCompiler.Cli;
using NewModCompiler.Diagnostics;

// Experimental backend orchestration, entered through the production-object
// validation host. Armor inputs exercise selection and application lifecycle;
// placing one in AptUI does not establish APT compiler or full G3 acceptance.
public static class CompilerApplicationBuild
{
    static readonly JavaScriptSerializer Json = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
    static readonly List<string> Tests = new List<string>();
    static readonly List<object> ProtocolCases = new List<object>();
    static string work, sdk, evidence, probe;
    static int checks;
    static void Require(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static IDictionary<string, object> Map(object value) { return (IDictionary<string, object>)value; }
    static object[] Array(object value) { return ((IEnumerable)value).Cast<object>().ToArray(); }
    static string Text(object value) { return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture); }
    static string Hash(string path)
    {
        using (var algorithm = SHA256.Create()) using (var stream = File.OpenRead(path))
            return BitConverter.ToString(algorithm.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
    }
    sealed class Observe : IBuildEventSink
    {
        internal readonly List<IDictionary<string, object>> Events = new List<IDictionary<string, object>>();
        internal CancellationTokenSource Cancellation;
        internal bool FailArtifact, Failed;
        public void Write(IDictionary<string, object> value)
        {
            Events.Add(Map(Json.DeserializeObject(Json.Serialize(value))));
            string kind = Text(value["eventType"]);
            if (kind == "StreamCompleted" && Cancellation != null) Cancellation.Cancel();
            if (kind == "ArtifactProduced" && FailArtifact && !Failed) { Failed = true; throw new IOException("Injected first artifact sink failure"); }
        }
        public void Dispose() { }
    }
    sealed class RunResult
    {
        internal BuildRequest Request;
        internal BuildPlan Expected;
        internal ExitCode Code;
        internal string BuildId;
        internal Observe Observer;
        internal IDictionary<string, object> Report;
    }
    static string Mod(string name, string[] valid, bool emptyOptional = false)
    {
        string root = Path.Combine(work, "外部 模组", name);
        foreach (string content in new[] { "base", "apt", "maps" })
        {
            if (emptyOptional && content != "base") continue;
            string relative = content == "base" ? "Data/Mod.xml" : content == "apt" ? "Data/AptUI/界面 entry.xml" : "Data/AdditionalMaps/MapMetaData_地图 entry.xml";
            string path = Path.Combine(root, relative); Directory.CreateDirectory(Path.GetDirectoryName(path));
            // An accidental unselected compilation must fail on real XML input,
            // rather than merely disappearing from an event-count assertion.
            File.WriteAllText(path, valid.Contains(content) ? probe : "<unselected-invalid-xml");
        }
        return root;
    }
    static RunResult Build(string name, string mod, string[] content, Observe observer = null)
    {
        string output = Path.Combine(evidence, "protocol", name);
        var request = Options.Parse(new[] { "build", "--mod", mod, "--sdk-root", sdk, "--out", output,
            "--package-content", String.Join(",", content), "/sc:false", "/bc:false", "/pc:false", "/csc:false", "/tl:0" });
        Require(request.ModRoot == Path.GetFullPath(mod) && !Paths.Contains(sdk, request.ModRoot), "Expected external MOD directory from Options.Parse");
        BuildPlan expected = Planner.Resolve(request);
        observer = observer ?? new Observe();
        string id = "application-" + name + "-" + Guid.NewGuid().ToString("N");
        Directory.CreateDirectory(output); string sentinel = Path.Combine(output, "existing-output.keep"); File.WriteAllText(sentinel, "leave existing output alone");
        Settings ambient = Settings.Current;
        var dispatcher = new EventDispatcher(id, new IBuildEventSink[] { new JsonLineSink(request.EventsPath), new TextLogSink(request.LogPath), observer });
        var session = new BuildSession(request, new CoreStreamCompiler(), dispatcher);
        ExitCode code = session.Run(observer.Cancellation == null ? CancellationToken.None : observer.Cancellation.Token);
        Require(Object.ReferenceEquals(ambient, Settings.Current), "BuildSession did not restore ambient Core settings");
        Require(File.ReadAllText(sentinel) == "leave existing output alone", "BuildSession removed pre-existing unrelated output");
        var result = new RunResult { Request = request, Expected = expected, Code = code, BuildId = id, Observer = observer,
            Report = Map(Json.DeserializeObject(File.ReadAllText(request.ReportPath))) };
        Lifecycle(result);
        ProtocolCases.Add(new { name = name, events = request.EventsPath, report = request.ReportPath, buildId = id,
            selectedContent = content, expectedStreamCount = expected.Tasks.Length, exitCode = (int)code });
        return result;
    }
    static IDictionary<string, object>[] Events(RunResult run, string kind)
    {
        return run.Observer.Events.Where(e => Text(e["eventType"]) == kind).ToArray();
    }
    static void Lifecycle(RunResult run)
    {
        var events = run.Observer.Events;
        Require(events.Count > 0 && Text(events[0]["eventType"]) == "BuildStarted" && Text(events[events.Count - 1]["eventType"]) == "BuildFinished", "Application lifecycle endpoints missing");
        for (int i = 0; i < events.Count; i++)
            Require(Text(events[i]["buildId"]) == run.BuildId && Convert.ToInt64(events[i]["sequence"]) == i + 1, "Application event identity or sequence mismatch");
        Require(Events(run, "BuildStarted").Length == 1 && Events(run, "BuildFinished").Length == 1 && Events(run, "PlanResolved").Length == 1, "Application emitted duplicate lifecycle endpoints");
        Require(Convert.ToInt32(Map(events[0]["payload"])["processId"]) == Process.GetCurrentProcess().Id, "BuildSession left the current process");
        Require(Text(run.Report["buildId"]) == run.BuildId && Convert.ToInt32(run.Report["exitCode"]) == (int)run.Code, "Report terminal identity differs from return code");
        var terminal = Map(events[events.Count - 1]["payload"]);
        Require(Convert.ToInt32(terminal["exitCode"]) == (int)run.Code && Text(terminal["status"]) == Text(run.Report["status"]), "Terminal event and report disagree");
        Require(Text(run.Report["executionMode"]) == "InProcess" && Text(run.Report["publicationStatus"]) == "IsolatedStagingOnly" && Array(run.Report["externalTools"]).Length == 0, "Unexpected application execution/publication mode");
        Require(Array(run.Report["loadedProviders"]).Length == 1 && Text(Array(run.Report["loadedProviders"])[0]) == "NewModCompiler.Core.Experimental.Win32.NoCache", "Application did not use the real experimental Core backend");
        string[] expectedIds = run.Expected.Tasks.Select(t => t.Id).ToArray();
        var reportPlan = Map(run.Report["plan"]);
        Require(Array(reportPlan["Tasks"]).Select(t => Text(Map(t)["Id"])).SequenceEqual(expectedIds), "Report plan differs from real Planner");
        Require(Array(Map(Events(run, "PlanResolved")[0]["payload"])["Tasks"]).Select(t => Text(Map(t)["Id"])).SequenceEqual(expectedIds), "Plan event differs from report plan");
        string[] jsonLines = File.ReadAllLines(run.Request.EventsPath);
        Require(jsonLines.Length == events.Count && File.ReadAllBytes(run.Request.EventsPath).Last() == (byte)'\n', "Durable JSONL lost an event or final line");
        for (int i = 0; i < jsonLines.Length; i++) Require(Json.Serialize(Json.DeserializeObject(jsonLines[i])) == Json.Serialize(events[i]), "Durable JSONL differs from observed events");
    }
    static void Success(RunResult run, string[] selected)
    {
        Require(run.Code == ExitCode.Success && Text(run.Report["status"]) == "succeeded", "Real BuildSession failed: " + Json.Serialize(run.Report["diagnostics"]));
        Require(run.Expected.Tasks.Length == selected.Length && run.Expected.Tasks.Select(t => t.Content).SequenceEqual(selected), "Selection compiled an unrequested content group");
        Require(Events(run, "StreamStarted").Select(e => Text(e["streamId"])).SequenceEqual(run.Expected.Tasks.Select(t => t.Id)), "Stream events differ from ordered selection plan");
        Require(Events(run, "StreamCompleted").Length == selected.Length && Events(run, "StreamCompleted").All(e => Text(Map(e["payload"])["status"]) == "succeeded"), "Selected streams did not complete exactly once");
        object[] artifacts = Array(run.Report["artifacts"]);
        Require(artifacts.Length == 4 * selected.Length && Events(run, "ArtifactProduced").Length == artifacts.Length, "Expected four artifacts per selected stream");
        string staging = Path.Combine(run.Request.OutputRoot, ".build", run.BuildId);
        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (object value in artifacts)
        {
            var artifact = Map(value); string path = Text(artifact["Path"]);
            Require(Paths.Contains(staging, path) && paths.Add(path) && File.Exists(path), "Artifact escaped this build's isolated staging");
            Require(new FileInfo(path).Length == Convert.ToInt64(artifact["Size"]) && Hash(path) == Text(artifact["Sha256"]), "Reported artifact identity differs from actual output");
            var emitted = Events(run, "ArtifactProduced").Select(e => Map(e["payload"])).Single(a => Text(a["Path"]) == path);
            Require(Text(emitted["Sha256"]) == Text(artifact["Sha256"]) && Convert.ToInt64(emitted["Size"]) == Convert.ToInt64(artifact["Size"]), "Artifact event and report disagree");
        }
        foreach (BuildTask task in run.Expected.Tasks)
        {
            string streamRoot = Path.Combine(staging, task.Content, Path.GetFileNameWithoutExtension(task.InputPath));
            var streamFiles = paths.Where(p => Paths.Contains(streamRoot, p)).ToArray();
            Require(streamFiles.Length == 4 && new[] { ".manifest", ".bin", ".imp", ".relo" }.All(extension => streamFiles.Count(p => Path.GetExtension(p) == extension) == 1), "Stream did not produce one exact linked four-file set");
        }
    }
    static void Failed(RunResult run, ExitCode expected, string status)
    {
        Require(run.Code == expected && Text(run.Report["status"]) == status, "Controlled application failure returned wrong status");
        Require(Array(run.Report["artifacts"]).Length == 0 && !Directory.Exists(Path.Combine(run.Request.OutputRoot, ".build", run.BuildId)), "Failed build retained its own staging or published artifacts");
    }
    static void Run()
    {
        string[] groups = { "base", "apt", "maps" };
        for (int mask = 1; mask < 8; mask++)
        {
            string[] selected = groups.Where((g, bit) => (mask & (1 << bit)) != 0).ToArray();
            Success(Build("selection-" + mask, Mod("选择 " + mask, selected), selected), selected);
        }
        Tests.Add("seven-nonempty-selections-real-Options-Planner-Core-BuildSession-external-Chinese-space-MODs");
        string full = Mod("取消和日志故障", groups);
        using (var cancellation = new CancellationTokenSource())
        {
            var cancelled = Build("cancel-at-stream-boundary", full, groups, new Observe { Cancellation = cancellation });
            Failed(cancelled, ExitCode.Cancelled, "cancelled");
            Require(Events(cancelled, "StreamStarted").Length == 1 && Events(cancelled, "StreamCompleted").Length == 1 && Events(cancelled, "ArtifactProduced").Length == 4, "Cancellation did not stop before the next stream");
        }
        Success(Build("retry-after-cancel", full, groups), groups);
        Tests.Add("stream-boundary-cancel-cleans-only-build-artifacts-and-next-real-session-succeeds");
        var sink = new Observe { FailArtifact = true };
        var failed = Build("artifact-sink-failure", full, new[] { "base" }, sink);
        Failed(failed, ExitCode.Output, "failed");
        Require(sink.Failed && Events(failed, "ArtifactProduced").Length == 1 && Events(failed, "StreamStarted").Length == 1 && Events(failed, "Diagnostic").Any(e => Text(e["diagnosticCode"]) == "EVENT_WRITE_FAILED"), "First artifact sink failure did not exercise required output handling");
        Success(Build("retry-after-sink-failure", full, new[] { "base" }), new[] { "base" });
        Tests.Add("first-artifact-sink-write-failure-Output-exit-state-release-and-next-real-session-succeeds");
        var optional = Build("empty-optional-groups", Mod("空可选目录", new[] { "base" }, true), new[] { "apt", "maps" });
        Require(optional.Code == ExitCode.Success && optional.Expected.Tasks.Length == 0 && Events(optional, "StreamStarted").Length == 0 && Events(optional, "ArtifactProduced").Length == 0 && Array(optional.Report["artifacts"]).Length == 0, "Empty optional groups unexpectedly compiled assets");
        object[] skipped = Array(Map(optional.Report["plan"])["Skipped"]);
        Require(skipped.Length == 2 && skipped.Select(s => Text(Map(s)["Content"])).SequenceEqual(new[] { "apt", "maps" }) && skipped.All(s => Text(Map(s)["Status"]) == "SkippedNoInputs"), "Empty optional groups lost explicit SkippedNoInputs outcome");
        Tests.Add("empty-selected-optional-groups-explicit-SkippedNoInputs-without-stream-compilation");
    }
    public static int Main(string[] args)
    {
        object result; int code = 0;
        try
        {
            Require(args.Length == 5 && IntPtr.Size == 4, "Expected work result commit run SDK in x86 host");
            work = Path.GetFullPath(args[0]); evidence = Path.GetDirectoryName(Path.GetFullPath(args[1])); sdk = Path.GetFullPath(args[4]);
            Require(typeof(CoreStreamCompiler).Assembly == Assembly.GetEntryAssembly() && typeof(BuildSession).Assembly == typeof(DocumentProcessor).Assembly, "Application fixture must use EXE-owned production implementations");
            probe = File.ReadAllText(Path.Combine(sdk, "Mods/mymod/Data/RecoveryArmorProbe.xml"));
            Run();
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks = checks, tests = Tests.ToArray(), protocolCases = ProtocolCases.ToArray(),
                scope = "Real in-process experimental backend and application orchestration using small Armor inputs; APT business, production CLI, cold-process/G3/SDK and cache acceptance remain separate" };
            Console.WriteLine("PASS ApplicationBuild: " + Tests.Count + " groups; " + checks + " checks");
        }
        catch (Exception error)
        {
            code = 1; Console.Error.WriteLine(error);
            result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null, runId = args.Length > 3 ? args[3] : null, checks = checks, tests = Tests.ToArray(), protocolCases = ProtocolCases.ToArray(), error = error.ToString() };
        }
        File.WriteAllText(args[1], Json.Serialize(result)); return code;
    }
}
