using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using NewModCompiler.Diagnostics;

namespace NewModCompiler.Application
{
    public sealed class BuildSession
    {
        readonly BuildRequest request; readonly IStreamCompiler compiler; readonly EventDispatcher events; readonly IDependencyPlanner dependencyPlanner;
        readonly List<Artifact> artifacts = new List<Artifact>(); readonly List<object> stages = new List<object>();
        readonly List<object> diagnostics = new List<object>();
        BuildPlan plan; bool executed; string activeStage; long stageStarted; string providerIdentity;
        public BuildSession(BuildRequest request, IStreamCompiler compiler, EventDispatcher events, IDependencyPlanner dependencyPlanner = null) { this.request = request; this.compiler = compiler; this.events = events; this.dependencyPlanner = dependencyPlanner; }
        public ExitCode Run(CancellationToken cancellation)
        {
            if (executed) throw new InvalidOperationException("BuildSession can only be executed once."); executed = true;
            ExitCode code = ExitCode.Success; string status = "succeeded"; string staging = Path.Combine(request.OutputRoot,".build",events.BuildId);
            try {
                Reports.RemoveStale(request.ReportPath);
                events.Emit("BuildStarted",payload:new { request = request, toolVersion = "0.1-p2", processId = System.Diagnostics.Process.GetCurrentProcess().Id });
                cancellation.ThrowIfCancellationRequested();
                StartStage("plan");
                plan = Planner.Resolve(request,dependencyPlanner); events.Emit("PlanResolved",payload:plan);
                CompleteStage("succeeded",0);
                if (request.Command == "build") {
                    if (compiler == null) throw new BuildFailure(ExitCode.Environment,"BACKEND_NOT_INTEGRATED","This P2 application skeleton has no integrated compilation backend. No stream was compiled.");
                    cancellation.ThrowIfCancellationRequested(); StartStage("compile"); Directory.CreateDirectory(staging);
                    compiler.Initialize(request); providerIdentity=compiler.ProviderIdentity;
                    int completed = 0;
                    foreach (BuildTask task in plan.Tasks) {
                        cancellation.ThrowIfCancellationRequested();
                        string streamDirectory = Path.Combine(staging,task.Content,Path.GetFileNameWithoutExtension(task.InputPath));
                        Directory.CreateDirectory(streamDirectory);
                        events.Emit("StreamStarted",payload:new {input=task.InputPath,effectiveOptions=request.Options},streamId:task.Id,stageId:"compile");
                        try {
                            var produced = compiler.Compile(new StreamContext {BuildId=events.BuildId,Task=task,Request=request,StagingDirectory=streamDirectory},cancellation);
                            cancellation.ThrowIfCancellationRequested();
                            if (produced == null) throw new InvalidOperationException("Backend returned no artifact list.");
                            foreach (Artifact artifact in produced) {
                                ValidateArtifact(artifact,streamDirectory); artifacts.Add(artifact); events.Emit("ArtifactProduced",payload:artifact,streamId:task.Id,stageId:"compile");
                            }
                            
                        } catch (Exception e) {
                            events.Emit("StreamCompleted","error",new {status = e is OperationCanceledException ? "cancelled" : "failed",artifactCount=0},streamId:task.Id,stageId:"compile"); throw;
                        }
                        events.Emit("StreamCompleted",payload:new {status="succeeded",artifactCount=artifacts.FindAll(a => Paths.Contains(streamDirectory,a.Path)).Count},streamId:task.Id,stageId:"compile");
                        events.Emit("Progress",payload:new {completed=++completed,total=plan.Tasks.Length,unit="streams"},stageId:"compile");
                    }
                    CompleteStage("succeeded",artifacts.Count);
                }
            } catch (OperationCanceledException) { code = ExitCode.Cancelled; status = "cancelled"; CompleteFailedStage(status); Diagnostic("BUILD_CANCELLED","Build cancelled at a safe boundary."); }
            catch (BuildFailure e) { code = e.ExitCode; status="failed"; CompleteFailedStage(status); Diagnostic(e.Code,e.Message); }
            catch (Exception e) { code = ExitCode.Internal; status="failed"; CompleteFailedStage(status); Diagnostic("INTERNAL_ERROR",e.ToString()); }
            finally {
                if (compiler != null) { try { compiler.Dispose(); } catch (Exception e) { code=ExitCode.Internal; status="failed"; Diagnostic("BACKEND_DISPOSE_FAILED",e.Message); } }
                if (code != ExitCode.Success) {
                    // Only this build's isolated workspace may be removed. Never sweep existing output.
                    try { if (Directory.Exists(staging)) Directory.Delete(staging,true); artifacts.Clear(); }
                    catch (Exception e) { code=ExitCode.Output; status="failed"; Diagnostic("STAGING_CLEANUP_FAILED",e.Message); }
                }
            }
            if (events.OutputFailed) { code=ExitCode.Output; status="failed"; }
            var report = new Dictionary<string, object> {
                {"schemaVersion","1.0"},{"buildId",events.BuildId},{"status",status},{"exitCode",(int)code},
                {"requestArguments",request.Arguments},{"effectiveOptions",request.Options},{"selectedContent",request.Content},
                {"plan",plan},{"stages",stages},{"diagnostics",diagnostics},{"diagnosticCounts",new {error=diagnostics.Count}},
                {"loadedProviders",providerIdentity == null ? new string[0] : new [] {providerIdentity}},
                {"externalTools",new string[0]},{"cache",new {status="NotImplemented",hits=0,misses=0}},
                {"elapsedMs",events.ElapsedMilliseconds},{"artifacts",artifacts},{"logPaths",new {events=request.EventsPath,report=request.ReportPath,text=request.LogPath}},
                {"executionMode",request.Command == "plan" ? "PlanOnly" : "InProcess"},
                {"publicationStatus",request.Command == "plan" ? "NoCompilationRequested" : "IsolatedStagingOnly"}
            };
            try { Reports.WriteAtomic(request.ReportPath,report); }
            catch (Exception e) { code=ExitCode.Output; status="failed"; Diagnostic("REPORT_WRITE_FAILED",e.Message); }
            try { events.Emit("BuildFinished",code == ExitCode.Success ? "info" : "error",new {status=status,exitCode=(int)code,reportPath=request.ReportPath},"Build " + status); }
            catch { code=ExitCode.Output; status="failed"; }
            // Closing required sinks is part of the controlled outcome, before the caller's
            // return value is evaluated. A late failure cannot emit a second terminal event.
            events.Dispose();
            if (events.OutputFailed) { code=ExitCode.Output; status="failed"; }
            if (events.CloseFailed) {
                diagnostics.Add(new {code="EVENT_CLOSE_FAILED",message=events.CloseFailureMessage,level="error"});
                try { Console.Error.WriteLine("EVENT_CLOSE_FAILED: " + events.CloseFailureMessage); } catch (Exception) { }
            }
            // The exit code and durable report must fail even if an earlier terminal event
            // was written successfully before an unavoidable close/flush failure.
            if (events.CloseFailed || (int)code != Convert.ToInt32(report["exitCode"])) {
                report["status"]=status; report["exitCode"]=(int)code;
                report["diagnosticCounts"]=new {error=diagnostics.Count}; report["elapsedMs"]=events.ElapsedMilliseconds;
                try { Reports.WriteAtomic(request.ReportPath,report); } catch { code=ExitCode.Output; }
            }
            return code;
        }
        void StartStage(string id)
        {
            activeStage=id; stageStarted=events.ElapsedMilliseconds;
            events.Emit("StageStarted",stageId:id);
        }
        void CompleteStage(string status,int count)
        {
            string id=activeStage; activeStage=null;
            long elapsed=events.ElapsedMilliseconds-stageStarted;
            stages.Add(new {id=id,status=status,elapsedMs=elapsed,artifactCount=count});
            events.Emit("StageCompleted",payload:new {status=status,elapsedMs=elapsed,artifactCount=count},stageId:id);
        }
        void CompleteFailedStage(string status)
        {
            if (activeStage!=null) { try { CompleteStage(status,0); } catch { } }
        }
        void Diagnostic(string code, string message)
        {
            diagnostics.Add(new {code=code,message=message,level="error"});
            try { events.Emit("Diagnostic","error",message:message,code:code); } catch { }
        }
        static void ValidateArtifact(Artifact artifact,string streamDirectory)
        {
            if (artifact == null || string.IsNullOrEmpty(artifact.Path) || !Paths.Contains(streamDirectory,artifact.Path) || !File.Exists(artifact.Path)) throw new BuildFailure(ExitCode.Output,"ARTIFACT_INVALID","Backend artifacts must be existing files within the current stream workspace.");
            artifact.Path = Path.GetFullPath(artifact.Path); artifact.Size=new FileInfo(artifact.Path).Length;
            using (var hash=SHA256.Create()) using (var input=File.OpenRead(artifact.Path)) artifact.Sha256=BitConverter.ToString(hash.ComputeHash(input)).Replace("-","").ToLowerInvariant();
        }
    }
}
