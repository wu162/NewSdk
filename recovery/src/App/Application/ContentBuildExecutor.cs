using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using NewModCompiler.Diagnostics;

namespace NewModCompiler.Application
{
    // The coordinator owns all externally visible events and report mutations.
    // Each nonempty content group owns exactly one thread and one Core lifetime.
    internal sealed class ContentBuildExecutor
    {
        readonly Func<string,IStreamCompiler> factory;
        readonly BlockingCollection<Message> messages=new BlockingCollection<Message>();
        readonly List<Owner> owners=new List<Owner>();
        static readonly AsyncLocal<string> currentStream=new AsyncLocal<string>();
        static readonly AsyncLocal<ContentBuildExecutor> currentExecutor=new AsyncLocal<ContentBuildExecutor>();
        internal readonly List<KeyValuePair<string,object>> CacheReports=new List<KeyValuePair<string,object>>();
        internal object InputRootsReport;
        internal string ProviderIdentity;
        internal object ResourceReport;
        long origin;
        internal object Report { get { return new {groupCount=owners.Count,groups=owners.Select(owner=>new {content=owner.Content,ownerThreadId=owner.Thread.ManagedThreadId,streams=owner.Timings.ToArray()}).ToArray(),resources=ResourceReport}; } }
        internal ContentBuildExecutor(Func<string,IStreamCompiler> factory) { this.factory=factory; }
        internal bool QueueTrace(string source,TraceEventType type,string message)
        {
            string stream=currentStream.Value;
            if(!Object.ReferenceEquals(currentExecutor.Value,this)) return false;
            messages.Add(new Message {Kind="trace",StreamId=stream,Source=source,TraceType=type,Text=message});
            return true;
        }
        internal IList<Artifact> Run(BuildRequest request,BuildPlan plan,CacheLayout layout,string buildId,
            CancellationToken cancellation,Action<BuildTask> started,Action<BuildTask,IList<Artifact>> completed,
            Action<string,string,TraceEventType,string> trace,Action<BuildTask,Exception> failed)
        {
            origin=Stopwatch.GetTimestamp();
            int jobs=request.JobsAuto ? Math.Max(1,Environment.ProcessorCount) : Math.Max(1,request.RequestedJobs);
            var governor=new BuildResourceGovernor(jobs);
            var results=new Dictionary<string,IList<Artifact>>(StringComparer.OrdinalIgnoreCase);
            var successful=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Exception failure=null;
            using(var stop=CancellationTokenSource.CreateLinkedTokenSource(cancellation)) {
                foreach(var group in plan.Tasks.GroupBy(task=>task.Content)) {
                    var owner=new Owner {Content=group.Key,Tasks=group.ToArray(),Request=request.ForContent(group.Key,governor)};
                    owner.Thread=new Thread(()=>RunOwner(owner,layout,buildId,governor,stop)) {Name="NewSdk "+group.Key+" Owner",IsBackground=false};
                    owners.Add(owner);
                }
                int exited=0;
                foreach(var owner in owners) owner.Thread.Start();
                try {
                    while(exited<owners.Count) {
                        if(failure==null && !stop.IsCancellationRequested) {
                            int running=owners.Count(owner=>owner.Busy);
                            foreach(var owner in owners) {
                                if(owner.Busy || owner.Closed || running>=jobs) continue;
                                if(owner.Next==owner.Tasks.Length) { owner.Work.CompleteAdding(); owner.Closed=true; continue; }
                                BuildTask task=owner.Tasks[owner.Next];
                                if(!task.Dependencies.All(successful.Contains)) continue;
                                layout.PrepareDirectory(layout.StreamDirectory(task));
                                owner.Busy=true; owner.Next++; running++; owner.Work.Add(task);
                            }
                        }
                        Message message;
                        if(!messages.TryTake(out message,100)) continue;
                        if(message.Kind=="exit") { exited++; continue; }
                        try {
                            if(message.Kind=="trace") trace(message.StreamId,message.Source,message.TraceType,message.Text);
                            else if(message.Kind=="start") {
                                try { started(message.Task); }
                                catch { stop.Cancel(); throw; }
                                finally { message.Owner.StartedAck.Set(); }
                            }
                            else if(message.Kind=="success") {
                                completed(message.Task,message.Artifacts);
                                results.Add(message.Task.Id,message.Artifacts); successful.Add(message.Task.Id);
                                message.Owner.Busy=false;
                            } else if(message.Kind=="failure") {
                                failed(message.Task,message.Error);
                                if(failure==null || (failure is OperationCanceledException && !(message.Error is OperationCanceledException))) failure=message.Error;
                                stop.Cancel();
                            }
                        } catch(Exception error) {
                            if(failure==null) failure=error;
                            stop.Cancel();
                        }
                    }
                } catch(Exception error) { failure=error; stop.Cancel(); }
                finally {
                    stop.Cancel();
                    foreach(var owner in owners) owner.Thread.Join();
                    // A coordinator/output error can bypass the normal drain;
                    // every native worker has joined before returning the error.
                    foreach(var owner in owners) {
                        request.Performance.Merge(owner.Request.Performance);
                        if(owner.CacheReport!=null) CacheReports.Add(new KeyValuePair<string,object>(owner.Content,owner.CacheReport));
                        if(InputRootsReport==null) InputRootsReport=owner.InputRoots;
                        if(ProviderIdentity==null) ProviderIdentity=owner.Provider;
                        owner.StartedAck.Dispose();
                        owner.Work.Dispose();
                    }
                    ResourceReport=governor.Snapshot();
                    messages.Dispose();
                }
            }
            if(failure!=null) ExceptionDispatchInfo.Capture(failure).Throw();
            cancellation.ThrowIfCancellationRequested();
            return plan.Tasks.SelectMany(task=>results[task.Id]).ToList();
        }
        void RunOwner(Owner owner,CacheLayout layout,string buildId,BuildResourceGovernor governor,CancellationTokenSource stop)
        {
            IStreamCompiler compiler=null; BuildTask current=null; bool began=false,reportFailure=false;
            Exception failure=null;
            currentExecutor.Value=this;
            try {
                foreach(BuildTask task in owner.Work.GetConsumingEnumerable(stop.Token)) {
                    current=task; began=false; currentStream.Value=task.Id;
                    owner.Request.Performance.StreamId=task.Id;
                    using(governor.EnterOwner(stop.Token)) {
                        stop.Token.ThrowIfCancellationRequested();
                        began=true;
                        owner.StartedAck.Reset();
                        messages.Add(new Message {Kind="start",Owner=owner,Task=task});
                        // Core's first phase/work callback must observe the
                        // stream already registered by the coordinator. Short
                        // or hot streams can otherwise finish before it reads
                        // the start message and lose their real work counters.
                        owner.StartedAck.Wait(stop.Token);
                        stop.Token.ThrowIfCancellationRequested();
                        if(compiler==null) {
                            compiler=factory(owner.Content);
                            if(compiler==null) throw new InvalidOperationException("Content compiler factory returned null.");
                            using(owner.Request.Performance.Measure("backend.initialize")) compiler.Initialize(owner.Request);
                            owner.Provider=compiler.ProviderIdentity;
                        }
                        IList<Artifact> produced;
                        double compileStarted=ElapsedMilliseconds();
                        using(owner.Request.Performance.Measure("stream.compile"))
                            produced=compiler.Compile(new StreamContext {BuildId=buildId,Task=task,Request=owner.Request,OutputDirectory=layout.StreamDirectory(task)},stop.Token);
                        owner.Timings.Add(new {streamId=task.Id,startedMs=compileStarted,completedMs=ElapsedMilliseconds()});
                        stop.Token.ThrowIfCancellationRequested();
                        if(produced==null) throw new InvalidOperationException("Backend returned no artifact list.");
                        messages.Add(new Message {Kind="success",Owner=owner,Task=task,Artifacts=produced});
                    }
                    current=null; currentStream.Value=null; owner.Request.Performance.StreamId=null;
                }
            } catch(OperationCanceledException error) {
                failure=error; reportFailure=current!=null;
                stop.Cancel();
            } catch(Exception error) {
                failure=error; reportFailure=true;
                stop.Cancel();
            } finally {
                try {
                    if(compiler!=null) {
                        using(governor.EnterOwner(CancellationToken.None))
                        using(owner.Request.Performance.Measure("backend.dispose")) compiler.Dispose();
                    }
                } catch(Exception error) {
                    AddCleanupFailure(ref failure,error,"BACKEND_DISPOSE_FAILED"); reportFailure=true; stop.Cancel();
                }
                finally {
                    try {
                        if(compiler is IBuildCacheCompiler) owner.CacheReport=((IBuildCacheCompiler)compiler).CacheReport;
                        if(compiler is IInputRootsCompiler) owner.InputRoots=((IInputRootsCompiler)compiler).InputRootsReport;
                    } catch(Exception error) {
                        AddCleanupFailure(ref failure,error,"BACKEND_REPORT_FAILED"); reportFailure=true; stop.Cancel();
                    }
                    finally {
                        currentStream.Value=null; currentExecutor.Value=null;
                        // Publish the primary exception only after teardown has
                        // finished adding details. The coordinator may inspect
                        // its Data immediately, and each started stream has one
                        // terminal message even if both compilation and cleanup fail.
                        if(reportFailure) messages.Add(new Message {Kind="failure",Owner=owner,Task=began ? current : null,Error=failure});
                        messages.Add(new Message {Kind="exit",Owner=owner});
                    }
                }
            }
        }
        static void AddCleanupFailure(ref Exception failure,Exception cleanup,string code)
        {
            if(failure==null) failure=new BuildFailure(ExitCode.Internal,code,cleanup.Message);
            var details=failure.Data["CoreCleanupExceptions"] as List<Exception>;
            if(details==null) { details=new List<Exception>(); failure.Data["CoreCleanupExceptions"]=details; }
            details.Add(cleanup);
        }
        double ElapsedMilliseconds() { return (Stopwatch.GetTimestamp()-origin)*1000.0/Stopwatch.Frequency; }
        sealed class Owner
        {
            internal string Content,Provider; internal BuildTask[] Tasks; internal BuildRequest Request;
            internal readonly BlockingCollection<BuildTask> Work=new BlockingCollection<BuildTask>();
            internal readonly ManualResetEventSlim StartedAck=new ManualResetEventSlim();
            internal Thread Thread; internal int Next; internal bool Busy,Closed; internal object CacheReport,InputRoots;
            internal readonly List<object> Timings=new List<object>();
        }
        sealed class Message
        {
            internal string Kind,StreamId,Source,Text; internal TraceEventType TraceType;
            internal Owner Owner; internal BuildTask Task; internal IList<Artifact> Artifacts; internal Exception Error;
        }
    }
}
