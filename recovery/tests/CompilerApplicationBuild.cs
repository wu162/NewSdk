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
using System.Xml;
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
    static string work, sdk, evidence, probe, shortOutputRoot;
    static int checks, buildNumber;
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
        internal Action<IDictionary<string, object>> InputProbe;
        internal bool FailArtifact, FailCoreTrace, Failed, InjectBridgeMessages, Injected;
        public void Write(IDictionary<string, object> value)
        {
            Events.Add(Map(Json.DeserializeObject(Json.Serialize(value))));
            string kind = Text(value["eventType"]);
            if (InputProbe != null) InputProbe(value);
            object diagnostic;
            bool coreTrace = kind == "Diagnostic" && value.TryGetValue("diagnosticCode", out diagnostic) && Text(diagnostic).StartsWith("CORE_TRACE", StringComparison.Ordinal);
            if (coreTrace && InjectBridgeMessages && !Injected)
            {
                Injected = true;
                // Contract instrumentation, invoked only after an actual Core
                // trace reaches the installed bridge. These are not claims of
                // business warning/error coverage for the Armor fixture.
                var tracer = Tracer.GetTracer("ApplicationBridgeContractFixture", "Fixture bridge contract injection");
                tracer.TraceWarning("[bridge contract injection] warning");
                tracer.TraceError("[bridge contract injection] error");
            }
            if (coreTrace && FailCoreTrace && !Failed) { Failed = true; throw new IOException("Injected first Core trace sink failure"); }
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
        internal IDictionary<string, object> ReleasedInputs;
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
    static RunResult Build(string name, string mod, string[] content, Observe observer = null, string[] extraArguments = null, int traceLevel = 3)
    {
        int number = ++buildNumber;
        // Core still uses legacy MAX_PATH APIs for intermediate asset names.
        // Keep the real compile workspace short, then archive unmodified logs;
        // do not weaken Chinese/space MOD-path or production path validation.
        string output = Path.Combine(shortOutputRoot, "b" + number);
        var arguments = new List<string> { "build", "--mod", mod, "--sdk-root", sdk, "--out", output,
            "--package-format", "loose", "--package-content", String.Join(",", content), "/sc:false", "/bc:false", "/pc:false", "/csc:false", "/tl:" + traceLevel.ToString(System.Globalization.CultureInfo.InvariantCulture) };
        if (extraArguments != null) arguments.AddRange(extraArguments);
        var request = Options.Parse(arguments.ToArray());
        Require(request.ModRoot == Path.GetFullPath(mod) && !Paths.Contains(sdk, request.ModRoot), "Expected external MOD directory from Options.Parse");
        BuildPlan expected = Planner.Resolve(request);
        observer = observer ?? new Observe();
        string id = "app-" + number + "-" + Guid.NewGuid().ToString("N").Substring(0, 8);
        Directory.CreateDirectory(output); string sentinel = Path.Combine(output, "existing-output.keep"); File.WriteAllText(sentinel, "leave existing output alone");
        Settings ambient = Settings.Current;
        var dispatcher = new EventDispatcher(id, new IBuildEventSink[] { new JsonLineSink(request.EventsPath), new TextLogSink(request.LogPath), observer });
        BuildSession session = null;
        var backend = new CoreStreamCompiler(delegate(string source, TraceEventType kind, string message) { session.RecordCoreTrace(source, kind, message); });
        session = new BuildSession(request, backend, dispatcher);
        ExitCode code;
        string archive = Path.Combine(evidence, "protocol", name);
        try { code = session.Run(observer.Cancellation == null ? CancellationToken.None : observer.Cancellation.Token); }
        finally
        {
            Directory.CreateDirectory(archive);
            foreach (string file in new[] { request.EventsPath, request.ReportPath, request.LogPath })
                if (File.Exists(file)) File.Copy(file, Path.Combine(archive, Path.GetFileName(file)), true);
        }
        Require(Object.ReferenceEquals(ambient, Settings.Current), "BuildSession did not restore ambient Core settings");
        Require(File.ReadAllText(sentinel) == "leave existing output alone", "BuildSession removed pre-existing unrelated output");
        var result = new RunResult { Request = request, Expected = expected, Code = code, BuildId = id, Observer = observer,
            Report = Map(Json.DeserializeObject(File.ReadAllText(request.ReportPath))),
            ReleasedInputs = Map(Json.DeserializeObject(Json.Serialize(backend.InputSnapshotReport))) };
        Lifecycle(result);
        var copies = new List<object>(); int artifactNumber = 0;
        foreach (var artifact in Array(result.Report["artifacts"]).Select(Map))
        {
            string source = Text(artifact["Path"]);
            string destination = Path.Combine(evidence, "compiled", "b" + number, (++artifactNumber) + Path.GetExtension(source));
            Directory.CreateDirectory(Path.GetDirectoryName(destination)); File.Copy(source, destination, true);
            Require(Hash(destination) == Text(artifact["Sha256"]), "Archived application artifact differs from actual output");
            copies.Add(new { originalPath = source, archivedPath = destination, sha256 = Text(artifact["Sha256"]) });
        }
        ProtocolCases.Add(new { name = name, events = Path.Combine(archive, "build.events.jsonl"), report = Path.Combine(archive, "build-report.json"),
            originalEventsPath = request.EventsPath, originalReportPath = request.ReportPath, originalOutputRoot = output, buildId = id,
            selectedContent = content, expectedStreamCount = expected.Tasks.Length, exitCode = (int)code, artifactCopies = copies.ToArray(), releasedPhysicalInputs = result.ReleasedInputs });
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
        Require(Text(run.Report["executionMode"]) == "InProcess" && Text(run.Report["publicationStatus"]) == (run.Code == ExitCode.Success && run.Expected.Tasks.Length != 0 ? "PublishedDirectory" : "NotPublished") && Array(run.Report["externalTools"]).Length == 0, "Unexpected application execution/publication mode");
        Require(Array(run.Report["loadedProviders"]).Length == 1 && Text(Array(run.Report["loadedProviders"])[0]) == "NewModCompiler.Core.Experimental.Win32.NoCache", "Application did not use the real experimental Core backend");
        string[] expectedIds = run.Expected.Tasks.Select(t => t.Id).ToArray();
        var reportPlan = Map(run.Report["plan"]);
        Require(Array(reportPlan["Tasks"]).Select(t => Text(Map(t)["Id"])).SequenceEqual(expectedIds), "Report plan differs from real Planner");
        Require(Array(Map(Events(run, "PlanResolved")[0]["payload"])["Tasks"]).Select(t => Text(Map(t)["Id"])).SequenceEqual(expectedIds), "Plan event differs from report plan");
        string[] jsonLines = File.ReadAllLines(run.Request.EventsPath);
        Require(jsonLines.Length == events.Count && File.ReadAllBytes(run.Request.EventsPath).Last() == (byte)'\n', "Durable JSONL lost an event or final line");
        for (int i = 0; i < jsonLines.Length; i++) Require(Json.Serialize(Json.DeserializeObject(jsonLines[i])) == Json.Serialize(events[i]), "Durable JSONL differs from observed events");
        var applicationWork = Map(run.Report["applicationWork"]);
        Require(Text(applicationWork["captureBoundary"]) == "beforeInitialReport", "Application work lost its capture boundary");
        var measured = Array(applicationWork["stages"]).Select(Map).ToArray();
        foreach (var item in measured)
            Require(Convert.ToInt64(item["calls"]) > 0 && Convert.ToInt64(item["failedCalls"]) >= 0 &&
                    Convert.ToInt64(item["failedCalls"]) <= Convert.ToInt64(item["calls"]) && Convert.ToDouble(item["elapsedMs"]) >= 0,
                    "Application operation work counters invalid");
        var compileWork = measured.Where(item => Text(item["stage"]) == "backendCompile").ToArray();
        Require(compileWork.Length <= 1 && compileWork.Sum(item => Convert.ToInt64(item["calls"])) == Events(run, "StreamStarted").Length,
            "Application work did not count actual stream compile attempts, including zero-input optional groups");
        Require(Convert.ToInt64(measured.Single(item => Text(item["stage"]) == "backendDispose")["calls"]) == 1,
            "Application work did not count backend disposal");
        CoreDiagnostics(run);
    }
    static bool CoreDiagnostic(IDictionary<string, object> value)
    {
        object code; return value.TryGetValue("diagnosticCode", out code) && Text(code).StartsWith("CORE_TRACE", StringComparison.Ordinal);
    }
    static void CoreDiagnostics(RunResult run)
    {
        var traced = Events(run, "Diagnostic").Where(CoreDiagnostic).ToArray();
        var streamIds = new HashSet<string>(Events(run, "StreamStarted").Select(e => Text(e["streamId"])), StringComparer.Ordinal);
        foreach (var value in traced)
        {
            Require(value.ContainsKey("streamId") && streamIds.Contains(Text(value["streamId"])) && Text(value["stageId"]) == "compile", "Core trace lost its active stream/stage correlation");
            var payload = Map(value["payload"]);
            Require(!String.IsNullOrEmpty(Text(payload["source"])) && !String.IsNullOrEmpty(Text(payload["traceEventType"])), "Core trace lost source/type metadata");
            string traceType = Text(payload["traceEventType"]), level = Text(value["level"]);
            Require((traceType == "Error" || traceType == "Critical") ? level == "error" : traceType == "Warning" ? level == "warning" : traceType == "Verbose" ? level == "debug" : level == "info", "Core trace severity mapping differs from bridge contract");
        }
        object[] diagnostics = Array(run.Report["diagnostics"]);
        var counts = Map(run.Report["diagnosticCounts"]);
        Require(Convert.ToInt32(counts["error"]) == diagnostics.Count(d => Text(Map(d)["level"]) == "error") &&
                Convert.ToInt32(counts["warning"]) == diagnostics.Count(d => Text(Map(d)["level"]) == "warning"), "Report diagnostic severity counts disagree with records");
        Require(!diagnostics.Any(d => Text(Map(d)["code"]) == "CORE_TRACE"), "Info/debug Core trace incorrectly accumulated as a report diagnostic");
        foreach (var diagnostic in diagnostics.Select(Map).Where(d => Text(d["code"]).StartsWith("CORE_TRACE", StringComparison.Ordinal)))
        {
            Require(Text(diagnostic["level"]) == "warning" || Text(diagnostic["level"]) == "error", "Report retained a non-actionable Core trace");
            Require(traced.Any(e => Text(e["diagnosticCode"]) == Text(diagnostic["code"]) && Text(e["message"]) == Text(diagnostic["message"]) &&
                    Text(e["streamId"]) == Text(diagnostic["streamId"]) && Text(e["stageId"]) == Text(diagnostic["stageId"]) &&
                    Text(Map(e["payload"])["source"]) == Text(diagnostic["source"])), "Reported Core warning/error lost its matching contextual event");
        }
    }
    static void Success(RunResult run, string[] selected)
    {
        Require(run.Code == ExitCode.Success && Text(run.Report["status"]) == "succeeded", "Real BuildSession failed: " + Json.Serialize(run.Report["diagnostics"]));
        Require(run.Expected.Tasks.Length == selected.Length && run.Expected.Tasks.Select(t => t.Content).SequenceEqual(selected), "Selection compiled an unrequested content group");
        Require(Events(run, "StreamStarted").Select(e => Text(e["streamId"])).SequenceEqual(run.Expected.Tasks.Select(t => t.Id)), "Stream events differ from ordered selection plan");
        Require(Events(run, "StreamCompleted").Length == selected.Length && Events(run, "StreamCompleted").All(e => Text(Map(e["payload"])["status"]) == "succeeded"), "Selected streams did not complete exactly once");
        foreach (BuildTask task in run.Expected.Tasks)
            Require(Events(run, "Diagnostic").Any(e => CoreDiagnostic(e) && Text(e["streamId"]) == task.Id && Text(Map(e["payload"])["source"]) != "ApplicationBridgeContractFixture"), "Selected real Core stream produced no bridged diagnostics");
        object[] artifacts = Array(run.Report["artifacts"]);
        Require(artifacts.Length == 4 * selected.Length && Events(run, "ArtifactProduced").Length == artifacts.Length, "Expected four artifacts per selected stream");
        string staging = Path.Combine(run.Request.OutputRoot, ".build", run.BuildId);
        string published = Path.Combine(run.Request.OutputRoot, "published");
        Require(!Directory.Exists(staging) && Text(run.Report["publicationDirectory"]) == published, "Successful publication retained staging or reported the wrong destination");
        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (object value in artifacts)
        {
            var artifact = Map(value); string path = Text(artifact["Path"]);
            Require(Paths.Contains(published, path) && paths.Add(path) && File.Exists(path), "Artifact escaped this build's published directory");
            Require(new FileInfo(path).Length == Convert.ToInt64(artifact["Size"]) && Hash(path) == Text(artifact["Sha256"]), "Reported artifact identity differs from actual output");
            var emitted = Events(run, "ArtifactProduced").Select(e => Map(e["payload"])).Single(a => Text(a["Path"]) == Path.Combine(staging, path.Substring(published.Length).TrimStart('\\', '/')));
            Require(Text(emitted["Sha256"]) == Text(artifact["Sha256"]) && Convert.ToInt64(emitted["Size"]) == Convert.ToInt64(artifact["Size"]), "Artifact event and report disagree");
        }
        foreach (BuildTask task in run.Expected.Tasks)
        {
            string streamRoot = Path.Combine(published, task.Content, Path.GetFileNameWithoutExtension(task.InputPath));
            var streamFiles = paths.Where(p => Paths.Contains(streamRoot, p)).ToArray();
            Require(streamFiles.Length == 4 && new[] { ".manifest", ".bin", ".imp", ".relo" }.All(extension => streamFiles.Count(p => Path.GetExtension(p) == extension) == 1), "Stream did not produce one exact linked four-file set");
        }
    }
    static void Failed(RunResult run, ExitCode expected, string status)
    {
        Require(run.Code == expected && Text(run.Report["status"]) == status,
            "Controlled application failure returned wrong status: expected=" + expected + "/" + status +
            " actual=" + run.Code + "/" + Text(run.Report["status"]) + " diagnostics=" + Json.Serialize(run.Report["diagnostics"]));
        Require(Array(run.Report["artifacts"]).Length == 0 && !Directory.Exists(Path.Combine(run.Request.OutputRoot, ".build", run.BuildId)), "Failed build retained its own staging or published artifacts");
    }
    static void RequireBlocked(Action operation, string message)
    {
        bool blocked = false;
        try { operation(); }
        catch (IOException) { blocked = true; }
        catch (UnauthorizedAccessException) { blocked = true; }
        Require(blocked, message);
    }
    static void RequireWritable(string path)
    {
        using (var input = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
    }
    static string[] ArtifactHashes(RunResult run)
    {
        return Array(run.Report["artifacts"]).Select(Map).OrderBy(item => Text(item["Path"]), StringComparer.Ordinal)
            .Select(item => Text(item["Sha256"])).ToArray();
    }
    static bool ContainsBytes(byte[] bytes, byte[] expected)
    {
        for (int offset = 0; offset <= bytes.Length - expected.Length; offset++)
        {
            int index = 0; while (index < expected.Length && bytes[offset + index] == expected[index]) index++;
            if (index == expected.Length) return true;
        }
        return false;
    }
    static void NativePhysicalPayloadCase()
    {
        const string disable = "NewSdk.Compiler.DisablePhysicalInputSnapshot";
        string mod = Path.Combine(shortOutputRoot, "native-positive-input"), data = Path.Combine(mod, "Data"); Directory.CreateDirectory(data);
        string payload = Path.Combine(data, "payload.bin"); byte[] bytes = Enumerable.Range(0, 64).Select(index => (byte)(index * 37 + 11)).ToArray();
        File.WriteAllBytes(payload, bytes);
        File.WriteAllText(Path.Combine(data, "Mod.xml"), "<AssetDeclaration xmlns='uri:ea.com:eala:asset'><AptAptData id='SealedNativeApt' File='payload.bin'/></AssetDeclaration>");
        bool beforeNative = false;
        var observed = new Observe { InputProbe = row => {
            object message;
            if (!beforeNative && Text(row["eventType"]) == "Diagnostic" && row.TryGetValue("message", out message) && Text(message).Contains("Compiling AptAptData"))
            {
                beforeNative = true;
                RequireBlocked(() => File.WriteAllBytes(payload, bytes), "Declared DataBlob was not pinned before native ProcessInstance");
            }
        } };
        var sealedRun = Build("physical-native-payload", mod, new[] { "base" }, observed); Success(sealedRun, new[] { "base" });
        Require(beforeNative, "Native DataBlob pin probe did not observe actual Core compile trace");
        var bin = Array(sealedRun.Report["artifacts"]).Select(Map).Single(item => Path.GetExtension(Text(item["Path"])) == ".bin");
        Require(ContainsBytes(File.ReadAllBytes(Text(bin["Path"])), bytes), "Native DataBlob did not consume the declared positive input bytes");
        RequireWritable(payload);
        RunResult fallback; AppContext.SetSwitch(disable, true);
        try { fallback = Build("physical-native-fallback", mod, new[] { "base" }); Success(fallback, new[] { "base" }); }
        finally { AppContext.SetSwitch(disable, false); }
        Require(ArtifactHashes(sealedRun).SequenceEqual(ArtifactHashes(fallback)), "Positive input pins changed native DataBlob four-file output");
        RequireWritable(payload);
        Tests.Add("actual-native-APT-DataBlob-positive-input-pinned-before-plugin-and-fallback-four-output-equivalence");
    }
    static void PhysicalInputCases()
    {
        const string disable = "NewSdk.Compiler.DisablePhysicalInputSnapshot";
        string[] groups = { "base", "apt", "maps" };
        string mod = Mod("封存 成功", groups), data = Path.Combine(mod, "Data");
        string source = Path.Combine(data, "Mod.xml"), child = Path.Combine(data, "sealed-child.xml"), moved = data + "-moved";
        File.WriteAllText(child, probe, new System.Text.UTF8Encoding(false));
        File.WriteAllText(source, "<xi:include xmlns:xi='http://www.w3.org/2001/XInclude' href='sealed-child.xml'/>", new System.Text.UTF8Encoding(false));
        byte[] original = File.ReadAllBytes(child); bool atStream = false, atPublish = false;
        var observer = new Observe { InputProbe = row => {
            string kind = Text(row["eventType"]); object stage;
            if (kind == "StreamCompleted" && !atStream)
            {
                atStream = true;
                RequireBlocked(() => File.WriteAllBytes(child, original), "A stream-completed input accepted an external writer");
                RequireBlocked(() => Directory.Move(data, moved), "A stream-completed parent accepted replacement/rename");
            }
            if (kind == "StageStarted" && row.TryGetValue("stageId", out stage) && Text(stage) == "publish")
            {
                atPublish = true;
                RequireBlocked(() => File.WriteAllBytes(child, original), "Provider disposal released an input before publication");
            }
        } };
        var sealedRun = Build("physical-inputs-shared", mod, groups, observer); Success(sealedRun, groups);
        Require(atStream && atPublish && File.ReadAllBytes(child).SequenceEqual(original), "Input pin probes did not exercise real stream and publication boundaries");
        var physical = Map(Map(sealedRun.Report["cache"])["physicalInputs"]);
        Require(Convert.ToBoolean(physical["enabled"]) && Convert.ToInt64(physical["captures"]) > 0 && Convert.ToInt64(physical["hits"]) > 0 &&
            Convert.ToInt64(physical["verificationReads"]) > 0 && Convert.ToInt64(physical["activeFilePins"]) > 0,
            "Successful application report lost positive-input sharing/final verification or its beforeInitialReport pin boundary");
        Require(Array(Map(sealedRun.Report["applicationWork"])["stages"]).Select(Map).Any(item => Text(item["stage"]) == "inputVerification" && Convert.ToInt64(item["calls"]) == 1),
            "Application did not record its final input-verification operation");
        RequireWritable(child); Directory.Move(data, moved); Directory.Move(moved, data);
        Require(!Directory.EnumerateDirectories(sealedRun.Request.OutputRoot, ".input-snapshot-*").Any(), "Successful finalization retained input-snapshot scratch");
        AppContext.SetSwitch(disable, true);
        RunResult fallback;
        try { fallback = Build("physical-inputs-fallback", mod, groups); Success(fallback, groups); }
        finally { AppContext.SetSwitch(disable, false); }
        Require(!Convert.ToBoolean(Map(Map(fallback.Report["cache"])["physicalInputs"])["enabled"]), "Physical snapshot fallback remained enabled");
        Require(ArtifactHashes(sealedRun).SequenceEqual(ArtifactHashes(fallback)), "Snapshot default/fallback changed real four-file output bytes");
        RequireWritable(child);
        DateTime timestamp = File.GetLastWriteTimeUtc(child);
        var document = new XmlDocument(); document.LoadXml(probe);
        var instance = document.DocumentElement.ChildNodes.Cast<XmlNode>().OfType<XmlElement>().First(item => item.HasAttribute("id"));
        string id = instance.GetAttribute("id"); Require(id.Length > 0, "Physical change probe needs a real asset id");
        string changedId = (id[0] == 'Q' ? "R" : "Q") + id.Substring(1);
        string changed = probe.Replace("id=\"" + id + "\"", "id=\"" + changedId + "\"");
        Require(changed != probe && System.Text.Encoding.UTF8.GetByteCount(changed) == original.Length, "Real equal-size mutation must alter one serialized asset id");
        File.WriteAllText(child, changed, new System.Text.UTF8Encoding(false)); File.SetLastWriteTimeUtc(child, timestamp);
        Require(new FileInfo(child).Length == original.Length && File.GetLastWriteTimeUtc(child) == timestamp, "Changed build input did not preserve exact size/time");
        var changedRun = Build("physical-inputs-new-generation", mod, new[] { "base" }); Success(changedRun, new[] { "base" });
        string[] originalBase = Array(sealedRun.Report["artifacts"]).Select(Map).Where(item => Text(item["Path"]).IndexOf(Path.DirectorySeparatorChar + "base" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) >= 0)
            .OrderBy(item => Text(item["Path"]), StringComparer.Ordinal).Select(item => Text(item["Sha256"])).ToArray();
        Require(originalBase.Length == 4 && !originalBase.SequenceEqual(ArtifactHashes(changedRun)), "New build reused an equal-time equal-size stale XML generation");
        using (var cancellation = new CancellationTokenSource())
        {
            var cancelled = Build("physical-inputs-cancel", mod, groups, new Observe { Cancellation = cancellation });
            Failed(cancelled, ExitCode.Cancelled, "cancelled"); RequireWritable(child);
            Require(!Directory.EnumerateDirectories(cancelled.Request.OutputRoot, ".input-snapshot-*").Any(), "Cancellation retained input-snapshot scratch");
        }
        var failed = Build("physical-inputs-output-failure", mod, new[] { "base" }, new Observe { FailArtifact = true });
        Failed(failed, ExitCode.Output, "failed"); RequireWritable(child);
        Require(!Directory.EnumerateDirectories(failed.Request.OutputRoot, ".input-snapshot-*").Any(), "Output failure retained input-snapshot scratch");
        Success(Build("physical-inputs-retry", mod, new[] { "base" }), new[] { "base" }); RequireWritable(child);
        Tests.Add("positive-input-sharing-default-fallback-exact-output-XInclude-pins-through-provider-disposal-and-publication");
        Tests.Add("new-build-equal-mtime-size-change-cancel-output-failure-final-release-and-retry");
        NativePhysicalPayloadCase();
    }
    static void CandidatePublicationCase(bool combined = false)
    {
        string suffix = combined ? "-combined" : "";
        string[] groups = combined ? new[] { "base", "apt", "maps" } : new[] { "base" };
        string mod = Mod("候选 一致性" + suffix, groups), data = Path.Combine(mod, "Data");
        string high = Path.Combine(work, "resolver-high" + suffix), low = Path.Combine(work, "resolver-low" + suffix);
        Directory.CreateDirectory(high); Directory.CreateDirectory(low);
        string candidate = "candidate-input.xml", first = Path.Combine(high, candidate), second = Path.Combine(low, candidate);
        File.WriteAllText(second, probe);
        File.WriteAllText(Path.Combine(data, "Mod.xml"), "<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Includes><Include type='all' source='data:" + candidate + "'/></Includes></AssetDeclaration>");
        string[] dataArguments = { "/data:" + high + ";" + low };
        bool injected = false;
        var observer = new Observe { InputProbe = row => {
            if (!injected && Text(row["eventType"]) == "StreamCompleted")
            {
                injected = true; File.WriteAllText(first, probe);
            }
        } };
        var failed = Build("resolver-candidate-changed-before-publish" + suffix, mod, groups, observer, dataArguments);
        Failed(failed, ExitCode.Compilation, "failed");
        Require(Events(failed, "StreamCompleted").Length == groups.Length && Events(failed, "StreamCompleted").All(row => Text(Map(row["payload"])["status"]) == "succeeded"),
            "Candidate publication refusal did not follow all selected successful stream compilations");
        Require(injected && Events(failed, "Diagnostic").Any(row => Text(row["diagnosticCode"]) == "CORE_INPUT_CHANGED"),
            "New higher-priority candidate did not reach final application input refusal");
        Require(Events(failed, "StageStarted").All(row => !row.ContainsKey("stageId") || Text(row["stageId"]) != "publish"),
            "Changed resolver generation entered publication");
        RequireWritable(first); RequireWritable(second);
        var retry = Build("resolver-candidate-next-generation" + suffix, mod, groups, null, dataArguments);
        Require(retry.Code == ExitCode.Success && Array(retry.Report["artifacts"]).Length == 4 * groups.Length,
            "New generation did not compile the newly selected input after refusal");
        var candidates = Map(Map(Map(retry.Report["cache"])["physicalInputs"])["resolverCandidates"]);
        Require(Convert.ToInt64(candidates["observations"]) > 0 && Convert.ToInt64(candidates["verificationChecks"]) > 0 &&
            !Convert.ToBoolean(candidates["canAuthorizeReuse"]), "Application did not expose final candidate verification boundaries");
        RequireWritable(first); RequireWritable(second);
        Tests.Add("actual-no-cache-application-higher-priority-resolver-candidate-added-after-stream-refuses-before-publication-and-new-generation-retries" + suffix);
    }
    // Fixture-only Core configuration instrumentation. The supported CLI still
    // clears BasePatchStream/configuration overrides. Each real application
    // stream consumes the patch via its ordinary OutputManager; the additional
    // precompiled consumer runs inside that same live physical-input scope.
    // This tests BuildSession acceptance, not a newly supported CLI option.
    static void ManifestFixture(string path, string patch)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        byte[] references = patch == null ? new byte[0] : new byte[] { 2 }.Concat(System.Text.Encoding.ASCII.GetBytes(patch + "\0")).ToArray();
        byte[] names = System.Text.Encoding.ASCII.GetBytes("Armor:Sealed\0"), source = System.Text.Encoding.ASCII.GetBytes("source.xml\0");
        using (var output = new BinaryWriter(File.Create(path)))
        {
            output.Write((byte)0); output.Write((byte)1); output.Write((ushort)6);
            foreach (uint value in new uint[] { 1234, 0, 1, 0, 0, 0, 0, 8, (uint)references.Length, (uint)names.Length, (uint)source.Length }) output.Write(value);
            foreach (uint value in new uint[] { 99, 77, 11, 22, 0, 1, 0, 0, 0, 0, 0, 0 }) output.Write(value);
            output.Write((uint)99); output.Write((uint)77);
            output.Write(references); output.Write(names); output.Write(source);
        }
    }
    static void PrecompiledFixtureConsumer(string manifest, string[] search)
    {
        const BindingFlags hidden = BindingFlags.NonPublic | BindingFlags.Instance;
        var parent = new AssetDeclarationDocument();
        var current = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        typeof(AssetDeclarationDocument).GetField("_Current", hidden).SetValue(parent, Activator.CreateInstance(current, true));
        var processor = new DocumentProcessor(Settings.Current, new PluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32), null);
        try
        {
            Require((bool)typeof(DocumentProcessor).GetMethod("LoadPrecompiledReference", hidden).Invoke(processor, new object[] { parent, manifest, search }),
                "Fixture precompiled consumer did not accept the external manifest");
        }
        catch (TargetInvocationException error) { throw error.InnerException; }
        Require(parent.ReferenceInstances.Count == 1 && parent.ReferenceInstances.First().Handle.InstanceName == "Sealed",
            "Fixture precompiled consumer did not restore the sealed symbols");
    }
    static void ManifestPublicationCases()
    {
        string[] groups = { "base", "apt", "maps" };
        foreach (string change in new[] { "earlier-patch", "nearer-hashes", "patch-asset-presence" })
        {
            string mod = Mod("清单 事务 " + change, groups);
            string root = Path.Combine(shortOutputRoot, "manifest-" + change), referenceRoot = Path.Combine(root, "reference"), search = Path.Combine(root, "search");
            string patch = Path.Combine(search, "base.manifest"), reference = Path.Combine(referenceRoot, "main.manifest"), basePatch = Path.Combine(root, "patch.manifest");
            ManifestFixture(patch, null); ManifestFixture(reference, "base.manifest"); ManifestFixture(basePatch, patch);
            string hashes = Path.Combine(root, "StringHashes.xml");
            File.WriteAllText(hashes, "<StringHashes><StringAndHash Hash='99' Text='Armor'/><StringAndHash Hash='77' Text='Sealed'/></StringHashes>");
            string added = change == "earlier-patch" ? Path.Combine(referenceRoot, "base.manifest") :
                change == "nearer-hashes" ? Path.Combine(referenceRoot, "main", "StringHashes.xml") : null;
            if (added == null)
                using (var manifest = new BinaryAssetBuilder.Utility.Manifest())
                {
                    Require(manifest.Load(patch, new[] { search }), "Patch asset candidate precondition failed");
                    added = manifest.Assets[0].FileBasePath + ".asset";
                }
            var configured = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            bool inFixture = false, injected = false; int completed = 0;
            Func<bool, Observe> observer = mutate => new Observe { InputProbe = row => {
                string kind = Text(row["eventType"]);
                if (!inFixture && kind == "Diagnostic" && row.ContainsKey("diagnosticCode") && Text(row["diagnosticCode"]).StartsWith("CORE_TRACE", StringComparison.Ordinal) &&
                    Text(Map(Json.DeserializeObject(Json.Serialize(row["payload"])))["source"]) == "DocumentProcessor" && Text(row["message"]).StartsWith("Build Cache Status:", StringComparison.Ordinal))
                {
                    string input = Settings.Current.InputPath;
                    Require(configured.Add(input), "Manifest instrumentation ran more than once for an application stream");
                    // The real processor has not yet copied these settings into
                    // ProcessOptions. Keep input/output paths untouched.
                    Settings.Current.BasePatchStream = basePatch;
                    inFixture = true;
                    try { PrecompiledFixtureConsumer(reference, new[] { search }); }
                    finally { inFixture = false; }
                }
                if (kind != "StreamCompleted") return;
                completed++;
                foreach (string path in new[] { reference, basePatch, patch, hashes })
                {
                    RequireBlocked(() => File.Delete(path), "Manifest input was not pinned through completed application stream: " + path);
                    RequireBlocked(() => File.WriteAllText(path, "changed"), "Manifest input allowed a write through completed application stream: " + path);
                }
                if (!mutate || completed != groups.Length) return;
                Require(!injected && !File.Exists(added), "Manifest candidate injection was not a single negative-to-positive transition");
                injected = true; Directory.CreateDirectory(Path.GetDirectoryName(added));
                if (change == "earlier-patch") ManifestFixture(added, null);
                else if (change == "nearer-hashes") File.WriteAllText(added, File.ReadAllText(hashes));
                else File.WriteAllBytes(added, new byte[0]);
            } };
            var failed = Build("manifest-publication-" + change, mod, groups, observer(true), null, 9);
            Failed(failed, ExitCode.Compilation, "failed");
            Require(injected && configured.Count == groups.Length && completed == groups.Length && Events(failed, "StreamCompleted").All(row => Text(Map(row["payload"])["status"]) == "succeeded"),
                "Manifest candidate refusal did not follow three successful configured real Core streams");
            Require(Events(failed, "Diagnostic").Any(row => Text(row["diagnosticCode"]) == "CORE_INPUT_CHANGED") &&
                !Events(failed, "StageStarted").Any(row => row.ContainsKey("stageId") && Text(row["stageId"]) == "publish"),
                "Manifest candidate change was not refused before whole-build publication");
            Require(!Directory.Exists(Path.Combine(failed.Request.OutputRoot, "published")) &&
                !Directory.EnumerateDirectories(failed.Request.OutputRoot, ".input-snapshot-*").Any(), "Refused manifest generation retained published output or snapshot scratch");
            foreach (string path in new[] { reference, basePatch, patch, hashes, added }) RequireWritable(path);
            var failedInputs = failed.ReleasedInputs;
            Require(Convert.ToInt64(failedInputs["activeFilePins"]) == 0 && Convert.ToInt64(failedInputs["activeReadHandles"]) == 0,
                "Refused manifest generation retained input pins");
            configured.Clear(); completed = 0;
            var retry = Build("manifest-new-generation-" + change, mod, groups, observer(false), null, 9);
            Success(retry, groups);
            Require(configured.Count == groups.Length && completed == groups.Length, "Manifest retry omitted one configured application stream");
            var retryInputs = retry.ReleasedInputs;
            var candidates = Map(retryInputs["resolverCandidates"]);
            Require(Text(failedInputs["snapshotId"]) != Text(retryInputs["snapshotId"]) && Convert.ToInt64(candidates["verificationChecks"]) > 0,
                "Manifest retry reused the rejected snapshot generation or omitted final query verification");
            Require(Convert.ToInt64(retryInputs["activeFilePins"]) == 0 && Convert.ToInt64(retryInputs["activeReadHandles"]) == 0, "Successful manifest retry retained input pins");
            foreach (string path in new[] { reference, basePatch, patch, hashes, added }) RequireWritable(path);
            Tests.Add("fixture-only-Core-patch-setting-and-precompiled-consumer-three-real-application-streams-" + change + "-whole-build-refusal-release-and-new-generation-success");
        }
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
        var bridge = new Observe { InjectBridgeMessages = true };
        var bridgeRun = Build("core-diagnostic-bridge-contract", full, new[] { "base" }, bridge);
        Success(bridgeRun, new[] { "base" });
        Require(bridge.Injected, "Actual Core trace did not trigger bridge contract instrumentation");
        var injected = Array(bridgeRun.Report["diagnostics"]).Select(Map).Where(d => d.ContainsKey("source") && Text(d["source"]) == "ApplicationBridgeContractFixture").ToArray();
        Require(injected.Length == 2 && injected.Count(d => Text(d["level"]) == "warning") == 1 && injected.Count(d => Text(d["level"]) == "error") == 1, "Bridge contract injection did not retain one warning and one error with severity counts");
        Tests.Add("real-Core-trace-bridge-stream-stage-correlation-plus-explicit-warning-error-contract-instrumentation");
        var traceSink = new Observe { FailCoreTrace = true };
        var traceFailed = Build("core-trace-sink-failure", full, new[] { "base" }, traceSink);
        Failed(traceFailed, ExitCode.Output, "failed");
        Require(traceSink.Failed && Events(traceFailed, "Diagnostic").Any(CoreDiagnostic) && Events(traceFailed, "Diagnostic").Any(e => Text(e["diagnosticCode"]) == "EVENT_WRITE_FAILED"), "Core trace sink failure did not retain Output semantics");
        Success(Build("retry-after-core-trace-sink-failure", full, new[] { "base" }), new[] { "base" });
        Tests.Add("real-Core-trace-sink-failure-Output-exit-clean-staging-and-next-session-success");
        var optional = Build("empty-optional-groups", Mod("空可选目录", new[] { "base" }, true), new[] { "apt", "maps" });
        Require(optional.Code == ExitCode.Success && optional.Expected.Tasks.Length == 0 && Events(optional, "StreamStarted").Length == 0 && Events(optional, "ArtifactProduced").Length == 0 && Array(optional.Report["artifacts"]).Length == 0, "Empty optional groups unexpectedly compiled assets");
        object[] skipped = Array(Map(optional.Report["plan"])["Skipped"]);
        Require(skipped.Length == 2 && skipped.Select(s => Text(Map(s)["Content"])).SequenceEqual(new[] { "apt", "maps" }) && skipped.All(s => Text(Map(s)["Status"]) == "SkippedNoInputs"), "Empty optional groups lost explicit SkippedNoInputs outcome");
        Tests.Add("empty-selected-optional-groups-explicit-SkippedNoInputs-without-stream-compilation");
        PhysicalInputCases();
        CandidatePublicationCase();
        CandidatePublicationCase(true);
        ManifestPublicationCases();
    }
    public static int Main(string[] args)
    {
        object result; int code = 0;
        string priorTraceEnvironment = Environment.GetEnvironmentVariable("BabEnableTrace");
        try
        {
            Require(args.Length == 5 && IntPtr.Size == 4, "Expected work result commit run SDK in x86 host");
            work = Path.GetFullPath(args[0]); evidence = Path.GetDirectoryName(Path.GetFullPath(args[1])); sdk = Path.GetFullPath(args[4]);
            shortOutputRoot = Path.Combine(Path.GetPathRoot(work), "nsa-" + Guid.NewGuid().ToString("N").Substring(0, 10));
            Directory.CreateDirectory(shortOutputRoot);
            Require(typeof(CoreStreamCompiler).Assembly == Assembly.GetEntryAssembly() && typeof(BuildSession).Assembly == typeof(DocumentProcessor).Assembly, "Application fixture must use EXE-owned production implementations");
            probe = File.ReadAllText(Path.Combine(sdk, "Mods/mymod/Data/RecoveryArmorProbe.xml"));
            Environment.SetEnvironmentVariable("BabEnableTrace", "False");
            Run();
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks = checks, tests = Tests.ToArray(), protocolCases = ProtocolCases.ToArray(),
                scope = "Real in-process backend and application orchestration using small Armor inputs, plus explicitly fixture-only patch settings/precompiled consumer injection for three-stream final-input transaction acceptance; no new CLI patch/precompiled option support, APT business, cold-process/G3/SDK or cache acceptance" };
            Console.WriteLine("PASS ApplicationBuild: " + Tests.Count + " groups; " + checks + " checks");
        }
        catch (Exception error)
        {
            code = 1; Console.Error.WriteLine(error);
            result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null, runId = args.Length > 3 ? args[3] : null, checks = checks, tests = Tests.ToArray(), protocolCases = ProtocolCases.ToArray(), error = error.ToString() };
        }
        Environment.SetEnvironmentVariable("BabEnableTrace", priorTraceEnvironment);
        File.WriteAllText(args[1], Json.Serialize(result)); return code;
    }
}
