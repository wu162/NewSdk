#if NEWSdk_MONOLITHIC
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BinaryAssetBuilder.Core;
using NewModCompiler.Diagnostics;

namespace NewModCompiler.Application
{
    internal sealed class BackendInterval
    {
        public string typeName;
        public long startTicks, endTicks;
        public int threadId;
    }

    // Workers record only private timing data. The owner merges Core timings.
    internal sealed class ParallelBackendMetrics
    {
        readonly object gate = new object();
        readonly bool enabled, detailed;
        int active;
        long nextSample;
        internal int BackendCalls, MaximumConcurrency, EffectiveJobs, MinimumEffectiveJobs;
        internal long PreparationTicks, WaitTicks, WorkTicks, CommitTicks, PeakPrivateBytes;
        internal bool ResourceQuerySucceeded = true;
        internal readonly Dictionary<string,int> CallsByType;
        internal readonly Dictionary<string,int> FallbackByReason;
        internal readonly List<BackendInterval> Intervals;
        internal ParallelBackendMetrics(BuildPerformance performance)
        {
            enabled = performance.Enabled; detailed = performance.Detailed;
            if (enabled) { CallsByType = new Dictionary<string,int>(StringComparer.Ordinal); FallbackByReason = new Dictionary<string,int>(StringComparer.Ordinal); }
            if (detailed) Intervals = new List<BackendInterval>();
        }
        internal bool Enabled { get { return enabled; } }
        internal void Fallback(string reason)
        { if (!enabled) return; int count; FallbackByReason.TryGetValue(reason,out count); FallbackByReason[reason]=count+1; }
        internal AssetBuffer Execute(string type, Func<AssetBuffer> execute, out long elapsed)
        {
            elapsed = 0;
            if (!enabled) return execute();
            long start;
            lock (gate) { start=Stopwatch.GetTimestamp(); BackendCalls++; active++; MaximumConcurrency=Math.Max(MaximumConcurrency,active); int count; CallsByType.TryGetValue(type,out count); CallsByType[type]=count+1; }
            try { return execute(); }
            finally {
                lock (gate) {
                    long end=Stopwatch.GetTimestamp(); elapsed=end-start; WorkTicks+=elapsed; active--;
                    if (detailed && Intervals.Count<512) Intervals.Add(new BackendInterval {typeName=type,startTicks=start,endTicks=end,threadId=Thread.CurrentThread.ManagedThreadId});
                }
            }
        }
        internal void SampleMemory()
        {
            if (!enabled) return;
            long now=Stopwatch.GetTimestamp(); if (now<nextSample) return;
            nextSample=now+Math.Max(1,Stopwatch.Frequency/10);
            try { using (var process=Process.GetCurrentProcess()) PeakPrivateBytes=Math.Max(PeakPrivateBytes,process.PrivateMemorySize64); }
            catch (System.ComponentModel.Win32Exception) { }
            catch (InvalidOperationException) { }
        }
        static double Ms(long ticks) { return ticks*(1000.0/Stopwatch.Frequency); }
        internal object Snapshot(BuildRequest request)
        {
            if (!enabled) return null;
            return new { protocol="NewModCompiler.ParallelBackend.v1", requestedJobs=request.JobsAuto ? "auto" : request.RequestedJobs.ToString(System.Globalization.CultureInfo.InvariantCulture),
                effectiveJobs=EffectiveJobs,minimumEffectiveJobs=MinimumEffectiveJobs,backendCalls=BackendCalls,backendCallsByType=CallsByType,maxBackendConcurrency=MaximumConcurrency,
                prepareMs=Ms(PreparationTicks),workerWaitMs=Ms(WaitTicks),workerComputeMs=Ms(WorkTicks),ownerCommitMs=Ms(CommitTicks),fallbackByReason=FallbackByReason,
                sampledPeakPrivateBytes=PeakPrivateBytes,resourceQuerySucceeded=ResourceQuerySucceeded,backendIntervals=Intervals,intervalSampleLimit=512,clockFrequency=Stopwatch.Frequency };
        }
    }

    internal sealed class ParallelBackendScheduler : IAssetExecution, IDisposable
    {
        readonly DocumentProcessor processor;
        readonly string staging;
        readonly CancellationToken cancellation;
        readonly ParallelBackendMetrics metrics;
        readonly BuildPerformance performance;
        readonly Queue<Pending> pending=new Queue<Pending>();
        readonly Queue<KeyValuePair<int,Action>> completions=new Queue<KeyValuePair<int,Action>>();
        readonly Func<BackendResourcePolicy> sampler;
        BackendResourcePolicy resources;
        int capacity, sequence, committedSequence;
        long budget, retained, nextSample;
        bool disposed, failed;
        sealed class Pending
        {
            internal int Sequence;
            internal BinaryAsset Asset;
            internal InstanceDeclaration Declaration;
            internal PreparedBackendWork Prepared;
            internal Task<AssetBuffer> Worker;
            internal AssetBuffer Buffer;
            internal Action Verify, Cached;
            internal Action<AssetLocation> Committed;
            internal string Directory;
            internal long WorkTicks;
        }
        internal ParallelBackendScheduler(DocumentProcessor processor,string staging,int requestedJobs,
            ParallelBackendMetrics metrics,BuildPerformance performance,CancellationToken cancellation,Func<BackendResourcePolicy> sampler=null)
        {
            this.processor=processor; this.staging=staging; this.metrics=metrics; this.performance=performance; this.cancellation=cancellation; this.sampler=sampler;
            resources=sampler==null ? BackendResourcePolicy.Capture(requestedJobs) : sampler();
            capacity=resources.Capacity; budget=resources.ByteBudget;
            metrics.EffectiveJobs=Math.Max(metrics.EffectiveJobs,capacity);
            metrics.MinimumEffectiveJobs=metrics.MinimumEffectiveJobs==0 ? capacity : Math.Min(metrics.MinimumEffectiveJobs,capacity);
            metrics.ResourceQuerySucceeded &= resources.MemoryQuerySucceeded;
        }
        public bool HasPending { get { return pending.Count!=0; } }
        void Check() { if(disposed || failed) throw new InvalidOperationException("Backend scheduler is closed."); cancellation.ThrowIfCancellationRequested(); }
        void Refresh()
        {
            long now=Stopwatch.GetTimestamp(); if(sampler==null && now<nextSample) return;
            nextSample=now+Math.Max(1,Stopwatch.Frequency/10);
            resources=sampler==null ? resources.RefreshMemoryBudget() : sampler();
            capacity=Math.Min(capacity,resources.Capacity); budget=Math.Min(budget,resources.ByteBudget);
            metrics.MinimumEffectiveJobs=Math.Min(metrics.MinimumEffectiveJobs,capacity); metrics.ResourceQuerySucceeded &= resources.MemoryQuerySucceeded;
            metrics.SampleMemory();
        }
        bool Fits(long bytes) { return bytes>=0 && retained<=budget && bytes<=budget-retained; }
        long Start() { return metrics.Enabled ? Stopwatch.GetTimestamp() : 0; }
        long Elapsed(long start) { return metrics.Enabled ? Stopwatch.GetTimestamp()-start : 0; }
        public void EnqueueCached(Action commit)
        {
            Check();
            // No cloning, file access or resource sampling for cache hits.
            while(pending.Count>=capacity) CommitOne();
            pending.Enqueue(new Pending {Sequence=++sequence,Cached=commit});
        }
        public void Enqueue(BinaryAsset asset,InstanceDeclaration declaration,IAssetBuilderPlugin plugin,
            Action compileCompatibility,Action verify,Action<AssetLocation> committed)
        {
            Check(); Refresh();
            if(capacity<=1) { Compatibility(declaration,compileCompatibility,verify,committed,asset,"resource-capacity"); return; }
            long estimate=PreparedBackendWork.EstimatePreparationBytes(declaration);
            while(pending.Count>0 && (pending.Count>=capacity || !Fits(estimate))) CommitOne();
            PreparedBackendWork prepared;
            string reason;
            long start=Start(); bool accepted;
            try { accepted=PreparedBackendWork.TryPrepareWithReason(declaration,plugin,budget/2,out prepared,out reason); }
            finally { metrics.PreparationTicks+=Elapsed(start); }
            if(!accepted) { Compatibility(declaration,compileCompatibility,verify,committed,asset,reason); return; }
            var task=new Pending {Asset=asset,Declaration=declaration,Prepared=prepared,Verify=verify,Committed=committed,Sequence=++sequence};
            bool enqueued=false;
            try {
                Refresh();
                while(pending.Count>0 && (pending.Count>=capacity || !Fits(prepared.PreparedBytes))) CommitOne();
                if(!prepared.CanRunParallel || capacity<=1 || prepared.PreparedBytes>budget/2 || !Fits(prepared.PreparedBytes)) {
                    prepared.Dispose(); task.Prepared=null;
                    Compatibility(declaration,compileCompatibility,verify,committed,asset,"resource-budget"); return;
                }
                if(declaration.HasCustomData) {
                    task.Directory=Path.Combine(staging,"backend-work",task.Sequence.ToString("D8")); Directory.CreateDirectory(task.Directory);
                    prepared.Declaration.CustomDataPath=Path.Combine(task.Directory,"payload.cdata");
                }
                cancellation.ThrowIfCancellationRequested();
                processor.RecordExecutionCompile();
                string typeName=declaration.Handle.TypeName;
                task.Worker=Task.Factory.StartNew(() => metrics.Execute(typeName,prepared.Execute,out task.WorkTicks),CancellationToken.None,TaskCreationOptions.None,TaskScheduler.Default);
                pending.Enqueue(task); retained+=prepared.PreparedBytes; enqueued=true;
            }
            finally { if(!enqueued && task.Prepared!=null) task.Prepared.Dispose(); }
        }
        void Compatibility(InstanceDeclaration declaration,Action compile,Action verify,Action<AssetLocation> committed,BinaryAsset asset,string reason)
        {
            metrics.Fallback(reason); Flush(); cancellation.ThrowIfCancellationRequested();
            long ticks;
            metrics.Execute(declaration.Handle.TypeName,() => { compile(); return asset.Buffer; },out ticks);
            long start=Start(); try { verify(); committed(asset.Commit(AssetLocation.Memory)); } finally { metrics.CommitTicks+=Elapsed(start); }
            committedSequence=sequence;
        }
        public void DeferCompletion(Action complete)
        {
            Check(); completions.Enqueue(new KeyValuePair<int,Action>(sequence,complete)); CompleteReady();
        }
        void CompleteReady()
        { while(completions.Count>0 && completions.Peek().Key<=committedSequence) completions.Dequeue().Value(); }
        public void Flush()
        { Check(); while(pending.Count>0) CommitOne(); CompleteReady(); }
        void CommitOne()
        {
            Pending task=pending.Dequeue();
            try {
                if(task.Worker!=null) {
                    long wait=Start(); try {task.Buffer=task.Worker.GetAwaiter().GetResult();} finally {metrics.WaitTicks+=Elapsed(wait);}
                }
                cancellation.ThrowIfCancellationRequested();
                long start=Start();
                try {
                    if(task.Cached!=null) task.Cached();
                    else {
                        if(task.Declaration.HasCustomData) {
                            string target=Path.Combine(task.Asset.CustomDataOutputDirectory,task.Asset.FileBase+".cdata");
                            Directory.CreateDirectory(task.Asset.CustomDataOutputDirectory);
                            if (File.Exists(task.Prepared.Declaration.CustomDataPath)) {
                                if(File.Exists(target)) File.Delete(target);
                                File.Move(task.Prepared.Declaration.CustomDataPath,target);
                            }
                            task.Declaration.CustomDataPath=target;
                        }
                        task.Asset.Buffer=task.Buffer;
                        if(performance.Detailed) {
                            performance.RecordBackend("core.asset.compile",task.WorkTicks);
                            performance.RecordBackend("core.compiler."+task.Declaration.Handle.TypeName,task.WorkTicks);
                        }
                        task.Verify(); task.Committed(task.Asset.Commit(AssetLocation.Memory));
                    }
                    committedSequence=task.Sequence;
                } finally {metrics.CommitTicks+=Elapsed(start);}
                CompleteReady();
            }
            catch { failed=true; throw; }
            finally { Release(task); }
        }
        void Release(Pending task)
        {
            if(task.Prepared!=null) {retained-=task.Prepared.PreparedBytes; task.Prepared.Dispose();}
            if(task.Directory!=null && Directory.Exists(task.Directory)) Directory.Delete(task.Directory,true);
        }
        // Failure/cancellation joins every native call without committing unverified
        // work or completing its document. Core state and leases stay alive here.
        public void Dispose()
        {
            if(disposed) return; disposed=true;
            Exception failure=null;
            while(pending.Count>0) {
                var task=pending.Dequeue();
                try {if(task.Worker!=null) task.Worker.GetAwaiter().GetResult();}
                catch(Exception error) {if(failure==null) failure=error;else failure.Data["BackendJoin."+task.Sequence]=error;}
                finally {try {Release(task);} catch(Exception error) {if(failure==null) failure=error;else failure.Data["BackendRelease."+task.Sequence]=error;}}
            }
            completions.Clear();
            if(failure!=null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
        }
    }
}
#endif
