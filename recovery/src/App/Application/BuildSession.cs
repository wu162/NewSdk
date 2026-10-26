using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        BuildPlan plan; bool executed, acceptingCoreTrace; string activeStage, activeStream; long stageStarted; string providerIdentity;
        int errorCount, warningCount;
        public BuildSession(BuildRequest request, IStreamCompiler compiler, EventDispatcher events, IDependencyPlanner dependencyPlanner = null) { this.request = request; this.compiler = compiler; this.events = events; this.dependencyPlanner = dependencyPlanner; }
        public ExitCode Run(CancellationToken cancellation)
        {
            if (executed) throw new InvalidOperationException("BuildSession can only be executed once."); executed = true; acceptingCoreTrace = true;
            DiagnosticDirectory stagingParent = null, stagingGuard = null; bool stagingOwned = false;
            try {
            ExitCode code = ExitCode.Success; string status = "succeeded"; string staging = Path.Combine(request.OutputRoot,".build",events.BuildId);
            try {
                Reports.RemoveStale(request.ReportPath);
                events.Emit("BuildStarted",payload:new { request = request, toolVersion = typeof(BuildSession).Assembly.GetName().Version.ToString(), processId = System.Diagnostics.Process.GetCurrentProcess().Id });
                cancellation.ThrowIfCancellationRequested();
                StartStage("plan");
                plan = Planner.Resolve(request,dependencyPlanner); events.Emit("PlanResolved",payload:plan);
                CompleteStage("succeeded",0);
                if (request.Command == "build") {
                    if (compiler == null) throw new BuildFailure(ExitCode.Environment,"BACKEND_NOT_INTEGRATED","This P2 application skeleton has no integrated compilation backend. No stream was compiled.");
                    cancellation.ThrowIfCancellationRequested(); StartStage("compile");
                    try {
                        // Secure the actual parent before any write or cleanup.
                        // Never take ownership of a pre-existing build directory.
                        stagingParent=DiagnosticDirectory.Acquire(Path.GetDirectoryName(staging),true);
                        DiagnosticDirectory.CreateNewDirectory(staging);
                        stagingGuard=DiagnosticDirectory.Acquire(staging,false);
                        stagingOwned=true;
                    } catch (IOException e) { throw new BuildFailure(ExitCode.Output,"STAGING_PATH",e.Message); }
                    catch (UnauthorizedAccessException e) { throw new BuildFailure(ExitCode.Output,"STAGING_PATH",e.Message); }
                    compiler.Initialize(request); providerIdentity=compiler.ProviderIdentity;
                    int completed = 0;
                    foreach (BuildTask task in plan.Tasks) {
                        cancellation.ThrowIfCancellationRequested();
                        string streamDirectory = Path.Combine(staging,task.Content,Path.GetFileNameWithoutExtension(task.InputPath));
                        Directory.CreateDirectory(streamDirectory);
                        activeStream = task.Id;
                        events.Emit("StreamStarted",payload:new {input=task.InputPath,effectiveOptions=request.Options},streamId:task.Id,stageId:"compile");
                        try {
                            var produced = compiler.Compile(new StreamContext {BuildId=events.BuildId,Task=task,Request=request,StagingDirectory=streamDirectory},cancellation);
                            cancellation.ThrowIfCancellationRequested();
                            if (produced == null) throw new InvalidOperationException("Backend returned no artifact list.");
                            foreach (Artifact artifact in produced) {
                                ValidateArtifact(artifact,streamDirectory); artifacts.Add(artifact); events.Emit("ArtifactProduced",payload:artifact,streamId:task.Id,stageId:"compile");
                            }
                            
                        } catch (Exception e) {
                            try { events.Emit("StreamCompleted","error",new {status = e is OperationCanceledException ? "cancelled" : "failed",artifactCount=0},streamId:task.Id,stageId:"compile"); }
                            catch (Exception outputFailure) { AttachCleanup(e, outputFailure); }
                            throw;
                        }
                        events.Emit("StreamCompleted",payload:new {status="succeeded",artifactCount=artifacts.FindAll(a => Paths.Contains(streamDirectory,a.Path)).Count},streamId:task.Id,stageId:"compile");
                        activeStream = null;
                        events.Emit("Progress",payload:new {completed=++completed,total=plan.Tasks.Length,unit="streams"},stageId:"compile");
                    }
                    CompleteStage("succeeded",artifacts.Count);
                }
            } catch (OperationCanceledException e) { code = ExitCode.Cancelled; status = "cancelled"; Fail(e,status,"BUILD_CANCELLED","Build cancelled at a safe boundary."); }
            catch (BuildFailure e) { code = e.ExitCode; status="failed"; Fail(e,status,e.Code,e.Message); }
            catch (Exception e) { code = ExitCode.Internal; status="failed"; Fail(e,status,"INTERNAL_ERROR",e.ToString()); }
            finally {
                activeStream = null;
                if (compiler != null) {
                    try { compiler.Dispose(); }
                    catch (Exception e) {
                        // Cleanup must not hide the original compilation failure
                        // or cancellation. A previously successful build must fail.
                        if (code == ExitCode.Success) { code=ExitCode.Internal; status="failed"; }
                        Diagnostic("BACKEND_DISPOSE_FAILED",e.ToString()); ExportExceptionDetails(e);
                    }
                }
                if (stagingGuard != null) { stagingGuard.Dispose(); stagingGuard=null; }
                if (code != ExitCode.Success && stagingOwned) {
                    // Only this build's isolated workspace may be removed. Never sweep existing output.
                    try { if (Directory.Exists(staging)) Directory.Delete(staging,true); }
                    catch (Exception e) { Diagnostic("STAGING_CLEANUP_FAILED",e.ToString()); ExportExceptionDetails(e); }
                    finally { artifacts.Clear(); }
                }
                acceptingCoreTrace = false;
            }
            bool retainedStaging = code == ExitCode.Success && stagingOwned;
            if (events.OutputFailed) { code=ExitCode.Output; status="failed"; }
            var report = new Dictionary<string, object> {
                {"schemaVersion","1.0"},{"buildId",events.BuildId},{"status",status},{"exitCode",(int)code},
                {"requestArguments",request.Arguments},{"effectiveOptions",request.Options},{"selectedContent",request.Content},
                {"plan",plan},{"stages",stages},{"diagnostics",diagnostics},{"diagnosticCounts",new {error=errorCount,warning=warningCount}},
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
                AddReportDiagnostic("EVENT_CLOSE_FAILED",events.CloseFailureMessage,"error",null,null,null,null);
                try { Console.Error.WriteLine("EVENT_CLOSE_FAILED: " + events.CloseFailureMessage); } catch (Exception) { }
            }
            // Output channels finalize after compilation cleanup. A late failure
            // must revoke the successfully compiled artifacts as well as exit7.
            bool revokedArtifacts = retainedStaging && code != ExitCode.Success;
            if (revokedArtifacts) {
                try { if (Directory.Exists(staging)) Directory.Delete(staging,true); }
                catch (Exception e) { Diagnostic("STAGING_CLEANUP_FAILED",e.ToString()); ExportExceptionDetails(e); }
                finally { artifacts.Clear(); }
            }
            // The exit code and durable report must fail even if an earlier terminal event
            // was written successfully before an unavoidable close/flush failure.
            if (revokedArtifacts || events.CloseFailed || (int)code != Convert.ToInt32(report["exitCode"])) {
                report["status"]=status; report["exitCode"]=(int)code;
                report["diagnosticCounts"]=new {error=errorCount,warning=warningCount}; report["elapsedMs"]=events.ElapsedMilliseconds;
                try { Reports.WriteAtomic(request.ReportPath,report); } catch { code=ExitCode.Output; }
            }
            return code;
            } finally {
                if (stagingGuard != null) stagingGuard.Dispose();
                if (stagingParent != null) stagingParent.Dispose();
            }
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
        // Core's severity is diagnostic information, not a second compilation
        // success policy. In particular, a warning/error line cannot by itself
        // change the result returned by Core or suppress a required sink failure.
        public void RecordCoreTrace(string source, TraceEventType eventType, string message)
        {
            if (!acceptingCoreTrace) throw new InvalidOperationException("Core traces require an active build session.");
            string level = eventType == TraceEventType.Critical || eventType == TraceEventType.Error ? "error" :
                eventType == TraceEventType.Warning ? "warning" : eventType == TraceEventType.Verbose ? "debug" : "info";
            string code = level == "error" ? "CORE_TRACE_ERROR" : level == "warning" ? "CORE_TRACE_WARNING" : "CORE_TRACE";
            string type = eventType.ToString();
            if (level == "error" || level == "warning") AddReportDiagnostic(code,message,level,activeStream,activeStage,source,type);
            // Deliberately no catch: EventDispatcher marks the required output
            // failure and its Output exception must reach the build outcome.
            events.Emit("Diagnostic",level,payload:new {source=source,traceEventType=type},message:message,code:code,streamId:activeStream,stageId:activeStage);
        }
        void AddReportDiagnostic(string code,string message,string level,string streamId,string stageId,string source,string traceEventType)
        {
            var diagnostic = new Dictionary<string,object> {{"code",code},{"message",message},{"level",level}};
            if (streamId != null) diagnostic["streamId"] = streamId;
            if (stageId != null) diagnostic["stageId"] = stageId;
            if (source != null) diagnostic["source"] = source;
            if (traceEventType != null) diagnostic["traceEventType"] = traceEventType;
            diagnostics.Add(diagnostic);
            if (level == "error") errorCount++;
            if (level == "warning") warningCount++;
        }
        void Diagnostic(string code, string message,string level = "error",string streamId = null,string stageId = null)
        {
            AddReportDiagnostic(code,message,level,streamId,stageId,null,null);
            // These diagnostics are emitted while unwinding an existing failure.
            // EventDispatcher.OutputFailed still forces Output at finalization.
            try { events.Emit("Diagnostic",level,message:message,code:code,streamId:streamId,stageId:stageId); } catch { }
        }
        void Fail(Exception error,string status,string code,string message)
        {
            string stage = activeStage;
            CompleteFailedStage(status);
            Diagnostic(code,message,streamId:activeStream,stageId:stage);
            ExportExceptionDetails(error,activeStream,stage);
        }
        static void AttachCleanup(Exception primary,Exception cleanup)
        {
            var failures = primary.Data["CoreCleanupExceptions"] as List<Exception>;
            if (failures == null) { failures = new List<Exception>(); primary.Data["CoreCleanupExceptions"] = failures; }
            failures.Add(cleanup);
        }
        void ExportExceptionDetails(Exception error,string streamId = null,string stageId = null)
        {
            // Only our known Core keys are exported; arbitrary user Data is not
            // traversed or serialized. Bound both graph depth and report growth.
            var seen = new List<Exception> {error};
            var pending = new Queue<Tuple<Exception,int>>();
            pending.Enqueue(Tuple.Create(error,0));
            int count = 0; bool truncated = false;
            while (pending.Count != 0)
            {
                var item = pending.Dequeue();
                if (item.Item2 >= 8) { truncated = true; continue; }
                foreach (string key in new[] {"OriginalException","BinaryAssetBuilder.ProcessDocument.FinalMetricsFailure","BinaryAssetBuilder.ProcessDocument.ProcessingTimeFailure"})
                {
                    Exception detail = item.Item1.Data[key] as Exception;
                    if (detail == null) continue;
                    bool duplicate = false;
                    foreach (Exception previous in seen) if (Object.ReferenceEquals(previous,detail)) { duplicate = true; break; }
                    if (duplicate) continue;
                    if (++count > 16) { truncated = true; break; }
                    seen.Add(detail);
                    Diagnostic(key == "OriginalException" ? "CORE_ORIGINAL_EXCEPTION" : "CORE_METRICS_CLEANUP_FAILED",detail.ToString(),key == "OriginalException" ? "info" : "error",streamId,stageId);
                    pending.Enqueue(Tuple.Create(detail,item.Item2+1));
                }
                var cleanup = item.Item1.Data["CoreCleanupExceptions"] as IEnumerable<Exception>;
                int examined = 0;
                if (cleanup != null) foreach (Exception detail in cleanup)
                {
                    if (++examined > 16) { truncated = true; break; }
                    if (detail == null) continue;
                    bool duplicate = false;
                    foreach (Exception previous in seen) if (Object.ReferenceEquals(previous,detail)) { duplicate = true; break; }
                    if (duplicate) continue;
                    if (++count > 16) { truncated = true; break; }
                    seen.Add(detail);
                    Diagnostic("CORE_CLEANUP_FAILED",detail.ToString(),"error",streamId,stageId);
                    pending.Enqueue(Tuple.Create(detail,item.Item2+1));
                }
                if (count > 16) break;
            }
            if (truncated) Diagnostic("CORE_EXCEPTION_DETAILS_TRUNCATED","Additional Core exception details exceeded the bounded diagnostic limit.","warning",streamId,stageId);
        }
        static void ValidateArtifact(Artifact artifact,string streamDirectory)
        {
            if (artifact == null || string.IsNullOrEmpty(artifact.Path) || !Paths.Contains(streamDirectory,artifact.Path) || !File.Exists(artifact.Path)) throw new BuildFailure(ExitCode.Output,"ARTIFACT_INVALID","Backend artifacts must be existing files within the current stream workspace.");
            artifact.Path = Path.GetFullPath(artifact.Path); artifact.Size=new FileInfo(artifact.Path).Length;
            using (var hash=SHA256.Create()) using (var input=File.OpenRead(artifact.Path)) artifact.Sha256=BitConverter.ToString(hash.ComputeHash(input)).Replace("-","").ToLowerInvariant();
        }
    }
}
