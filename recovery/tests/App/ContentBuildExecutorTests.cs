using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using NewModCompiler.Application;
using NewModCompiler.Cli;
using NewModCompiler.Diagnostics;

namespace NewModCompiler.Tests
{
    internal static class ContentBuildExecutorTests
    {
        static int checks, serial;
        static void Check(bool condition, string message)
        {
            Interlocked.Increment(ref checks);
            if (!condition) throw new Exception("Content execution: " + message);
        }
        static void Put(string path, string text = "<AssetDeclaration/>")
        { Directory.CreateDirectory(Path.GetDirectoryName(path)); File.WriteAllText(path, text); }
        static void Wait(ManualResetEventSlim signal, string message)
        { Check(signal.Wait(TimeSpan.FromSeconds(8)), message); }

        sealed class Recorder : IBuildEventSink
        {
            readonly int coordinator = Thread.CurrentThread.ManagedThreadId;
            internal readonly List<IDictionary<string, object>> Values = new List<IDictionary<string, object>>();
            public void Write(IDictionary<string, object> value)
            { Check(Thread.CurrentThread.ManagedThreadId == coordinator, "An Owner wrote the event sink directly"); Values.Add(value); }
            public void Dispose() { }
        }
        sealed class Harness : IDisposable
        {
            internal readonly BuildRequest Request;
            internal readonly Recorder Events = new Recorder();
            internal readonly List<Compiler> Compilers = new List<Compiler>();
            internal readonly List<string> Started = new List<string>();
            internal readonly object Gate = new object();
            internal readonly ManualResetEventSlim Overlap = new ManualResetEventSlim();
            internal BuildSession Session;
            internal Action<StreamContext, CancellationToken> CompileAction;
            internal int Active, Peak, ExpectedOverlap = 1;
            internal bool Trace;
            internal string CleanupFailure;
            internal Harness(string root, string content, int jobs)
            {
                string work = Path.Combine(root, (++serial).ToString()), sdk = Path.Combine(work, "sdk"), mod = Path.Combine(work, "mod");
                Directory.CreateDirectory(sdk);
                foreach (string name in new[] {"Mod.xml", "AptUI/a.xml", "AptUI/b.xml", "AdditionalMaps/MapMetaData_a.xml", "AdditionalMaps/MapMetaData_b.xml"})
                    Put(Path.Combine(mod, "Data", name));
                Request = Options.Parse(new[] {"build", "--mod", mod, "--sdk-root", sdk, "--out", Path.Combine(work,"out"),
                    "--package-format", "loose", "--package-content", content, "--jobs", jobs.ToString(), "--performance", "summary"});
            }
            internal ExitCode Run(CancellationToken cancellation = default(CancellationToken), IDependencyPlanner dependencies = null)
            {
                using (var dispatcher = new EventDispatcher(Guid.NewGuid().ToString("N"), new IBuildEventSink[] {Events})) {
                    Session = new BuildSession(Request, null, dispatcher, dependencies, compilerFactory: content => {
                        var compiler = new Compiler(this, content);
                        lock (Gate) Compilers.Add(compiler);
                        return compiler;
                    });
                    return Session.Run(cancellation);
                }
            }
            internal Dictionary<string, object> Report()
            { return new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(File.ReadAllText(Request.ReportPath)); }
            internal void Joined()
            {
                Check(Active == 0, "Build returned while compilation was active");
                foreach (var compiler in Compilers)
                    Check(compiler.Disposed && !compiler.Owner.IsAlive, "Build returned before an Owner completed Dispose and joined");
            }
            public void Dispose() { Overlap.Dispose(); }
        }
        sealed class Compiler : IStreamCompiler, IBuildCacheCompiler
        {
            readonly Harness harness;
            internal readonly string Content;
            internal readonly Thread Owner = Thread.CurrentThread;
            internal readonly List<string> Calls = new List<string>();
            internal bool Initialized, Disposed;
            BuildRequest request;
            internal Compiler(Harness harness, string content) { this.harness = harness; Content = content; }
            void Affinity() { Check(Thread.CurrentThread == Owner, "Core lifecycle moved away from its Owner thread"); }
            public string ProviderIdentity { get { return "CONTROLLED_CONTENT_COMPILER"; } }
            public object CacheReport {
                get {
                    if(Content=="base" && harness.CleanupFailure=="report") throw new InvalidOperationException("CONTROLLED_CACHE_REPORT_CLEANUP");
                    return new {status="Disabled",hits=0,misses=0,sessionCacheDirectory=request.CacheLayout.ForContent(Content).Session};
                }
            }
            public void Initialize(BuildRequest value)
            {
                Affinity(); Check(!Initialized && !Disposed, "Invalid Initialize lifecycle");
                Check(value.ContentGroup == Content && value.Options.IsReadOnly, "Owner request or options were not isolated");
                request=value; Initialized=true;
            }
            public IList<Artifact> Compile(StreamContext context, CancellationToken cancellation)
            {
                Affinity(); Check(Initialized && !Disposed && context.Task.Content == Content, "Compilation crossed content ownership");
                lock (harness.Gate) {
                    Calls.Add(context.Task.Id); harness.Started.Add(context.Task.Id);
                    harness.Active++; harness.Peak=Math.Max(harness.Peak,harness.Active);
                    if (harness.Active >= harness.ExpectedOverlap) harness.Overlap.Set();
                }
                try {
                    Wait(harness.Overlap, "Independent content groups never overlapped");
                    if (harness.Trace) {
                        harness.Session.RecordCoreTrace("controlled",TraceEventType.Warning,"owner:"+context.Task.Id);
                        Task.Run(() => harness.Session.RecordCoreTrace("controlled",TraceEventType.Warning,"task:"+context.Task.Id)).GetAwaiter().GetResult();
                    }
                    if (harness.CompileAction != null) harness.CompileAction(context,cancellation);
                    cancellation.ThrowIfCancellationRequested();
                    var artifacts = new List<Artifact>();
                    foreach (string extension in new[] {"manifest", "bin", "imp", "relo"}) {
                        string path=Path.Combine(context.OutputDirectory,"stream."+extension);
                        Put(path,context.Task.Id+":"+extension);
                        artifacts.Add(new Artifact {Path=path,Kind=extension,Size=new FileInfo(path).Length});
                    }
                    return artifacts;
                } finally { lock (harness.Gate) harness.Active--; }
            }
            public void Dispose()
            {
                Affinity(); Check(!Disposed, "Owner disposed twice"); Disposed=true;
                if(Content=="base" && harness.CleanupFailure=="dispose") throw new InvalidOperationException("CONTROLLED_DISPOSE_CLEANUP");
            }
        }
        sealed class AptDependency : IDependencyPlanner
        {
            public IEnumerable<BuildTask> Expand(BuildRequest request, IList<BuildTask> roots)
            {
                roots.Single(task=>task.Content=="base").Dependencies=new[] {"apt/a"};
                return roots;
            }
        }
        sealed class ProgressCompiler : IStreamCompiler
        {
            readonly BuildProgressState progress;
            readonly IDictionary<string,ManualResetEventSlim> entered;
            internal ProgressCompiler(BuildProgressState progress,IDictionary<string,ManualResetEventSlim> entered)
            { this.progress=progress; this.entered=entered; }
            public string ProviderIdentity { get { return "CONTROLLED_EARLY_PROGRESS"; } }
            public void Initialize(BuildRequest request) { }
            public IList<Artifact> Compile(StreamContext context,CancellationToken cancellation)
            {
                // Core can report work synchronously at the first line of
                // Compile. A queued StreamStarted alone must not lose it.
                progress.RecordWork(context.Task.Id,1,2,3);
                entered[context.Task.Id].Set();
                return new Artifact[0];
            }
            public void Dispose() { }
        }
        static string[] ArtifactIdentities(Harness harness)
        {
            var report=harness.Report();
            return ((IEnumerable)report["artifacts"]).Cast<Dictionary<string,object>>()
                .Select(artifact=>File.ReadAllText((string)artifact["Path"])).ToArray();
        }
        static void Combinations(string root)
        {
            foreach (string content in new[] {"base","apt","maps","base,apt","base,maps","apt,maps","base,apt,maps"}) {
                string[] serialOutput=null;
                foreach (int jobs in new[] {1,4}) using (var harness=new Harness(root,content,jobs)) {
                    int groupCount=content.Split(',').Length;
                    harness.ExpectedOverlap=Math.Min(groupCount,Math.Min(jobs,Environment.ProcessorCount));
                    harness.Trace=true;
                    var plan=Planner.Resolve(harness.Request);
                    Check(harness.Run()==ExitCode.Success,"Combination failed: "+content+" jobs="+jobs);
                    harness.Joined();
                    Check(harness.Compilers.Select(compiler=>compiler.Content).OrderBy(value=>value)
                        .SequenceEqual(content.Split(',').OrderBy(value=>value)),"Created a compiler for an absent group");
                    Check(harness.Compilers.Select(compiler=>compiler.Owner.ManagedThreadId).Distinct().Count()==groupCount,"Groups shared an Owner thread");
                    Check(harness.Peak==harness.ExpectedOverlap,"jobs did not constrain actual overlapping Owner work");
                    foreach (var compiler in harness.Compilers)
                        Check(compiler.Calls.SequenceEqual(plan.Tasks.Where(task=>task.Content==compiler.Content).Select(task=>task.Id)),"Group entry order changed");
                    string[] output=ArtifactIdentities(harness);
                    Check(output.SequenceEqual(plan.Tasks.SelectMany(task=>new[] {"manifest","bin","imp","relo"}.Select(extension=>task.Id+":"+extension))),"Artifacts did not retain plan order");
                    if (serialOutput==null) serialOutput=output;
                    else Check(output.SequenceEqual(serialOutput),"jobs changed raw output bytes");
                    var traces=harness.Events.Values.Where(value=>(string)value["eventType"]=="Diagnostic" && value.ContainsKey("message") &&
                        (((string)value["message"]).StartsWith("owner:") || ((string)value["message"]).StartsWith("task:"))).ToArray();
                    Check(traces.Length==plan.Tasks.Length*2,"Owner or managed Task traces were lost");
                    foreach (var trace in traces)
                        Check((string)trace["streamId"]==((string)trace["message"]).Substring(((string)trace["message"]).IndexOf(':')+1),"A trace was attributed to another stream");
                    var progress=harness.Events.Values.Where(value=>(string)value["eventType"]=="Progress").ToArray();
                    Check(progress.Length==plan.Tasks.Length,"Progress emitted per-asset or missed a successful stream");
                }
            }
        }
        static void ReadyDependencies(string root)
        {
            if (Environment.ProcessorCount<2) return;
            using (var firstDone=new ManualResetEventSlim())
            using (var baseStarted=new ManualResetEventSlim())
            using (var harness=new Harness(root,"base,apt",2)) {
                harness.CompileAction=(context,cancellation)=>{
                    if(context.Task.Id=="apt/a") firstDone.Set();
                    else if(context.Task.Id=="apt/b") Wait(baseStarted,"Ready Base waited for the entire APT group");
                    else { Check(firstDone.IsSet,"Base ran before its explicit APT dependency");baseStarted.Set(); }
                };
                Check(harness.Run(dependencies:new AptDependency())==ExitCode.Success,"Task-level dependency execution failed");
                harness.Joined();
                Check(baseStarted.IsSet,"A ready dependent task never started");
            }
        }
        static void ProgressStartAcknowledgement(string root)
        {
            using(var harness=new Harness(root,"base,apt",2)) {
                var plan=Planner.Resolve(harness.Request);
                var entered=plan.Tasks.ToDictionary(task=>task.Id,task=>new ManualResetEventSlim());
                var progress=new BuildProgressState(()=>0);
                progress.SetPlan(plan.Tasks.Length); progress.StartStage("compile");
                int completed=0; bool compiledBeforeStart=false;
                var executor=new ContentBuildExecutor(content=>new ProgressCompiler(progress,entered));
                try {
                    executor.Run(harness.Request,plan,harness.Request.CacheLayout,"progress-start-race",CancellationToken.None,
                        task=>{
                            // Hold the coordinator before creating the stream's
                            // counters. The old fire-and-forget start lets its
                            // Owner finish Compile during this bounded wait.
                            compiledBeforeStart|=entered[task.Id].Wait(TimeSpan.FromMilliseconds(200));
                            progress.StartStream(task.Id);
                        },
                        (task,artifacts)=>{
                            var stream=progress.Snapshot().ActiveStreams.Single(value=>value.Stream==task.Id);
                            Check(stream.AssetsCompleted==1 && stream.XmlParsed==2 && stream.DocumentsReused==3,
                                "Compile reported work before the coordinator created its stream counters: "+task.Id);
                            progress.CompleteStream(task.Id,"succeeded"); completed++;
                        },
                        (stream,source,type,message)=>{},
                        (task,error)=>{throw new Exception("Progress acknowledgement build failed",error);});
                    Check(!compiledBeforeStart && completed==plan.Tasks.Length && progress.Snapshot().StreamsCompleted==plan.Tasks.Length,
                        "Owner did not wait for its coordinator's StreamStarted callback");
                } finally { foreach(var signal in entered.Values) signal.Dispose(); }
            }
        }
        static void FailureAndCancellation(string root)
        {
            if (Environment.ProcessorCount<2) return;
            foreach (bool cancel in new[] {false,true})
            foreach (string cleanup in new[] {"none","dispose","report"})
            using (var source=new CancellationTokenSource())
            using (var peerStarted=new ManualResetEventSlim())
            using (var harness=new Harness(root,"base,apt,maps",2)) {
                harness.ExpectedOverlap=2; harness.Trace=true; harness.CleanupFailure=cleanup;
                harness.CompileAction=(context,cancellation)=>{
                    if(context.Task.Content=="base") {
                        Wait(peerStarted,"Failure was injected before the peer began native work");
                        if(cancel) source.Cancel();
                        else throw new BuildFailure(ExitCode.Compilation,"CONTROLLED_GROUP_FAILURE","Controlled content failure");
                    } else {
                        peerStarted.Set();
                        Check(cancellation.WaitHandle.WaitOne(TimeSpan.FromSeconds(8)),"Peer did not receive build stop notification");
                    }
                };
                Check(harness.Run(source.Token)==(cancel ? ExitCode.Cancelled : ExitCode.Compilation),"Content failure/cancellation exit changed");
                harness.Joined();
                Check(harness.Started.Count==2 && !harness.Started.Contains("apt/b") && !harness.Started.Any(value=>value.StartsWith("maps/")),"Failure accepted new content work");
                var started=harness.Events.Values.Where(value=>(string)value["eventType"]=="StreamStarted").Select(value=>(string)value["streamId"]).ToArray();
                var completed=harness.Events.Values.Where(value=>(string)value["eventType"]=="StreamCompleted").Select(value=>(string)value["streamId"]).ToArray();
                Check(started.Length==harness.Started.Count && completed.Length==started.Length,"Failure/cleanup emitted an unmatched stream terminal event");
                foreach(string stream in started) Check(completed.Count(value=>value==stream)==1,"Failure/cleanup repeated or omitted a stream terminal event: "+stream);
                if(cleanup!="none") {
                    string marker=cleanup=="dispose" ? "CONTROLLED_DISPOSE_CLEANUP" : "CONTROLLED_CACHE_REPORT_CLEANUP";
                    Check(harness.Events.Values.Any(value=>(string)value["eventType"]=="Diagnostic" && value.ContainsKey("message") &&
                        ((string)value["message"]).Contains(marker)),"Secondary Owner cleanup failure was not diagnosed");
                }
                Check(!harness.Events.Values.Any(value=>(string)value["eventType"]=="ArtifactProduced" ||
                    (string)value["eventType"]=="StageStarted" && ((string)value["stageId"]=="publish" || (string)value["stageId"]=="package")),"Failed content was packaged or published");
                var report=harness.Report();
                Check(((ICollection)report["artifacts"]).Count==0 && (string)report["publicationStatus"]=="NotPublished","Failure accepted partial artifacts");
            }
        }
        internal static int Run(string root)
        {
            checks=0; serial=0;
            Combinations(root); ReadyDependencies(root); ProgressStartAcknowledgement(root); FailureAndCancellation(root);
            return checks;
        }
    }
}
