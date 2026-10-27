using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NewModCompiler.Application;
using NewModCompiler.Cli;
using NewModCompiler.Diagnostics;

namespace NewModCompiler.Tests
{
    internal static class InputSnapshotTransactionTests
    {
        static int checks;
        static void Check(bool value, string message) { checks++; if (!value) throw new Exception("Input transaction: " + message); }
        sealed class Backend : IStreamCompiler, IBuildCacheCompiler, IInputSnapshotCompiler
        {
            public bool Retained, Initialized, Disposed, Verified, Released, Closed, CacheCommitted, CacheCompleted;
            public bool FailInitialize, FailVerify, FailCompile;
            public CancellationTokenSource CancelVerify;
            public string ProviderIdentity { get { return "TEST_INPUT_TRANSACTION"; } }
            public object CacheReport { get { return new { status = "Controlled", hits = 0, misses = 0 }; } }
            public object InputSnapshotReport { get { return new { enabled = true, active = Initialized && !Released }; } }
            public void RetainInputsUntilCompletion() { Check(!Initialized && !Disposed, "retention selected after initialization"); Retained = true; }
            public void Initialize(BuildRequest request) { Check(Retained, "backend initialized without extended input ownership"); Initialized = true; if (FailInitialize) throw new BuildFailure(ExitCode.Environment, "TEST_INITIALIZE", "initialize"); }
            public IList<Artifact> Compile(StreamContext context, CancellationToken cancellation)
            {
                Check(!Released, "inputs released before compile");
                if (FailCompile) throw new BuildFailure(ExitCode.Compilation, "TEST_COMPILE", "compile");
                string path = Path.Combine(context.StagingDirectory, "test.bin"); File.WriteAllText(path, "current");
                return new[] { new Artifact { Path = path, Kind = "controlled" } };
            }
            public void Dispose() { Disposed = true; Check(!Released, "provider disposal prematurely released retained inputs"); }
            public void VerifyInputs(CancellationToken cancellation)
            {
                Check(Disposed && !Released, "verification must follow provider disposal with inputs retained");
                if (CancelVerify != null) CancelVerify.Cancel();
                cancellation.ThrowIfCancellationRequested();
                if (FailVerify) throw new BuildFailure(ExitCode.Compilation, "CORE_INPUT_CHANGED", "injected change");
                Verified = true;
            }
            public void CommitCache() { Check(Verified && !Released, "cache committed without verified, retained inputs"); CacheCommitted = true; }
            public void RollbackCache() { }
            public void CompleteCache() { Check(Closed && CacheCommitted && !Released, "inputs released before cache Complete or required output close"); CacheCompleted = true; }
            public void ReleaseInputs() { Released = true; }
        }
        sealed class Sink : IBuildEventSink
        {
            public Backend Backend; public BuildRequest Request; public bool FailClose, FailTerminal, FailReport;
            public void Write(IDictionary<string, object> value)
            {
                string kind = (string)value["eventType"];
                if (kind == "StageStarted" && value.ContainsKey("stageId") && (string)value["stageId"] == "publish")
                    Check(Backend.Disposed && Backend.Verified && !Backend.Released, "publication started without verified retained inputs");
                if (kind == "StageCompleted" && value.ContainsKey("stageId") && (string)value["stageId"] == "publish" && FailReport)
                    Directory.CreateDirectory(Request.ReportPath);
                if (kind == "BuildFinished") { Check(!Backend.Released, "inputs released before terminal output"); if (FailTerminal) throw new IOException("terminal"); }
            }
            public void Dispose() { Check(!Backend.Released, "inputs released before required sink close"); Backend.Closed = true; if (FailClose) throw new IOException("close"); }
        }
        public static int Run(string work, string mod, string sdk)
        {
            for (int mode = 0; mode < 8; mode++)
            {
                using (var cancellation = new CancellationTokenSource())
                {
                    var request = Options.Parse(new[] { "build", "--mod", mod, "--sdk-root", sdk, "--out", Path.Combine(work, "case" + mode), "--package-format", "loose", "--package-content", "base" });
                    string previous = Path.Combine(request.OutputRoot, "published", "previous.bin"); Directory.CreateDirectory(Path.GetDirectoryName(previous)); File.WriteAllText(previous, "previous");
                    var backend = new Backend { FailVerify = mode == 1, FailInitialize = mode == 2, FailCompile = mode == 3, CancelVerify = mode == 7 ? cancellation : null };
                    var sink = new Sink { Backend = backend, Request = request, FailTerminal = mode == 4, FailReport = mode == 5, FailClose = mode == 6 };
                    ExitCode code;
                    using (var events = new EventDispatcher(Guid.NewGuid().ToString("N"), new IBuildEventSink[] { sink }))
                        code = new BuildSession(request, backend, events).Run(cancellation.Token);
                    Check(backend.Released && backend.Disposed, "input owner leaked after build return");
                    if (mode == 0) Check(code == ExitCode.Success && backend.CacheCompleted && !File.Exists(previous), "successful input transaction did not finalize");
                    else
                    {
                        Check(code == (mode == 1 || mode == 3 ? ExitCode.Compilation : mode == 2 ? ExitCode.Environment : mode == 7 ? ExitCode.Cancelled : ExitCode.Output), "failure/cancellation changed its exit code");
                        Check(!backend.CacheCompleted && File.ReadAllText(previous) == "previous", "failed input transaction finalized or replaced previous publication");
                    }
                }
            }
            return checks;
        }
    }
}
