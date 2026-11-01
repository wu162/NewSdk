// Protocol fixture only. This is not a build of the real NewModCompiler.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

public static class CompilerFixture {
    static readonly JavaScriptSerializer Json = new JavaScriptSerializer();
    static string id;
    static int sequence;
    static StreamWriter events;
    static void Emit(string type, object payload, string level = "info", string message = null, string code = null) {
        var row = new Dictionary<string, object> { {"schemaVersion", "1.0"}, {"buildId", id},
            {"sequence", ++sequence}, {"timestampUtc", DateTime.UtcNow.ToString("o")},
            {"elapsedMs", sequence * 10}, {"eventType", type}, {"level", level} };
        if (payload != null) row["payload"] = payload;
        if (message != null) row["message"] = message;
        if (code != null) row["diagnosticCode"] = code;
        events.WriteLine(Json.Serialize(row)); events.Flush();
        Console.WriteLine(type + " " + message);
    }
    public static int Main(string[] args) {
        var options = new Dictionary<string, string>();
        for (int i = 1; i < args.Length; ++i) {
            string key = args[i]; int separator = key.IndexOf('=');
            if (separator >= 0) options[key.Substring(0, separator)] = key.Substring(separator + 1);
            else options[key] = args[++i];
        }
        if (args[0] == "doctor") { Console.WriteLine("FIXTURE_ENVIRONMENT_OK"); return 0; }
        string output = options["--out"], mod = options["--mod"], name = Path.GetFileName(mod);
        Directory.CreateDirectory(output);
        id = Guid.NewGuid().ToString("N"); sequence = 0;
        bool failed = name == "MockFailure", missing = name == "MockMissing", slow = name == "MockSlow";
        bool planOnly = args[0] == "plan";
        var tasks = new object[] { new { Id = "base/Mod", Content = "base", InputPath = Path.Combine(mod, "Data", "Mod.xml"),
            OutputName = "base/Mod", Reason = "AutomaticContentDiscovery", Dependencies = new string[0] } };
        var plan = new { Tasks = tasks, Skipped = new object[0], PlanKind = "StreamDiscovery" };
        using (events = new StreamWriter(options["--events"], false, new UTF8Encoding(false))) {
            Emit("BuildStarted", new { command = args[0] }); Emit("PlanResolved", plan);
            Emit("StageStarted", null);
            if (slow) Thread.Sleep(45000);
            if (failed) Emit("Diagnostic", null, "error", "Fixture error without source coordinates", "FIXTURE_ERROR");
            if (!planOnly) { Emit("StreamStarted", null); Emit("StreamCompleted", null); Emit("Progress", new { completed = 1, total = 1, unit = "streams" }); }
            Emit("StageCompleted", null);
            Emit("BuildFinished", new { status = failed ? "failed" : "succeeded", exitCode = failed ? 4 : 0 });
        }
        File.WriteAllText(options["--log"], "Raw protocol fixture log");
        if (!missing) {
            string publication = planOnly ? null : Path.Combine(output, "published");
            object[] artifacts = new object[0];
            if (!planOnly && !failed) {
                Directory.CreateDirectory(publication);
                string artifact = Path.Combine(publication, "MockBasic_1.0.big");
                File.WriteAllText(artifact, "fixture artifact");
                artifacts = new object[] { new { Path = artifact, Kind = "big", Size = new FileInfo(artifact).Length } };
            }
            var report = new { schemaVersion = "1.0", buildId = id, status = failed ? "failed" : "succeeded",
                exitCode = failed ? 4 : 0, executionMode = planOnly ? "PlanOnly" : "InProcess", plan = plan,
                diagnostics = failed ? new object[] { new { code = "FIXTURE_ERROR", message = "Fixture error without source coordinates", level = "error" } } : new object[0],
                artifacts = artifacts, publicationDirectory = publication };
            File.WriteAllText(options["--report"], Json.Serialize(report));
        }
        return failed ? 4 : 0;
    }
}
