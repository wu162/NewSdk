#if NEWSdk_MONOLITHIC
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BinaryAssetBuilder.Core;

namespace NewModCompiler.Application
{
    internal sealed class BackendInterval
    {
        public string typeName, sourcePath;
        public long startTicks, endTicks;
        public int threadId;
    }

    internal sealed class AssetExecutionMetrics
    {
        readonly object gate = new object();
        int active;
        long nextMemorySample;
        public int BackendCalls, MaximumConcurrency, Hits, Misses;
        public long RestoredBytes, WrittenBytes, ExternalHashBytes;
        public long PeakSampledPrivateBytes;
        // Owner-only phase counters except BackendWorkTicks, which is updated
        // under gate with the real backend intervals. Inclusive, overlapping.
        public long PreparationTicks, ActionKeyTicks, ResultRestoreTicks, ResultWriteTicks,
            ResultCommitTicks, WorkerWaitTicks, BackendWorkTicks;
        public int EffectiveJobs, MinimumEffectiveJobs, LogicalProcessorCount;
        public long TaskMemoryBudgetBytes, MinimumTaskMemoryBudgetBytes, PerTaskMemoryLimitBytes;
        public bool ResourceMemoryQuerySucceeded;
        public readonly Dictionary<string,int> CallsByType = new Dictionary<string,int>(StringComparer.Ordinal);
        // Owner-only classifications. Lookup misses partition Misses; bypasses
        // never enter the existing action hit/miss totals.
        public readonly Dictionary<string,int> ActionMissReasons = new Dictionary<string,int>(StringComparer.Ordinal);
        public readonly Dictionary<string,int> CacheBypassReasons = new Dictionary<string,int>(StringComparer.Ordinal);
        public readonly Dictionary<string,int> PreparationRejectedReasons = new Dictionary<string,int>(StringComparer.Ordinal);
        public void RecordPreparationRejected(string reason) { Increment(PreparationRejectedReasons, reason); }
        public void RecordMiss(string reason) { Misses++; Increment(ActionMissReasons, reason); }
        public void RecordBypass(string reason) { Increment(CacheBypassReasons, reason); }
        static void Increment(Dictionary<string,int> counts, string reason)
        { int count; counts.TryGetValue(reason, out count); counts[reason] = count + 1; }
        public readonly List<BackendInterval> Intervals = new List<BackendInterval>();
        public void SampleMemory()
        {
            // Process memory queries are much more expensive than many XML
            // backends. Bound telemetry to ten samples/second for the whole
            // executor instead of querying three times for every tiny asset.
            long now = Stopwatch.GetTimestamp();
            lock (gate)
            {
                if (now < nextMemorySample) return;
                nextMemorySample = now + Math.Max(1L, Stopwatch.Frequency / 10);
            }
            try { using (var process = Process.GetCurrentProcess()) { long bytes = process.PrivateMemorySize64; lock (gate) PeakSampledPrivateBytes = Math.Max(PeakSampledPrivateBytes, bytes); } }
            catch (System.ComponentModel.Win32Exception) { }
            catch (InvalidOperationException) { }
        }
        public AssetBuffer Execute(string type, string source, Func<AssetBuffer> action)
        {
            var interval = new BackendInterval { typeName = type, sourcePath = source,
                threadId = Thread.CurrentThread.ManagedThreadId };
            SampleMemory();
            lock (gate) { BackendCalls++; active++; MaximumConcurrency = Math.Max(MaximumConcurrency, active); interval.startTicks = Stopwatch.GetTimestamp(); int count; CallsByType.TryGetValue(type, out count); CallsByType[type] = count + 1; }
            try { return action(); }
            finally { lock (gate) { interval.endTicks = Stopwatch.GetTimestamp(); BackendWorkTicks += interval.endTicks - interval.startTicks; active--; if (Intervals.Count < 512) Intervals.Add(interval); } SampleMemory(); }
        }
    }

    // One owner, bounded independent backend work, deterministic owner commit.
    // Frontend validation and linking remain in DocumentProcessor.
    internal sealed class AssetTaskExecution : IAssetExecution, IDisposable
    {
        const string ResultFormat = "NewModCompiler.ActionResult.v1";
        readonly DocumentProcessor processor;
        readonly ActionAssetCache cache;
        readonly AssetExecutionMetrics metrics;
        readonly string staging, toolchain, effectiveOptions;
        readonly CancellationToken cancellation;
        int capacity;
        long byteBudget, nextResourceSample;
        BackendResourcePolicy resources;
        readonly Func<BackendResourcePolicy> resourceSampler;
        readonly Queue<Pending> pending = new Queue<Pending>();
        readonly Queue<KeyValuePair<int,Action>> completions = new Queue<KeyValuePair<int,Action>>();
        readonly Dictionary<string,string> outputDigests = new Dictionary<string,string>(StringComparer.Ordinal);
        long retainedBytes;
        int sequence, committedSequence;
        bool disposed;

        sealed class Pending
        {
            public BinaryAsset Asset;
            public InstanceDeclaration Declaration;
            public PreparedBackend Prepared;
            public Task<AssetBuffer> Worker;
            public AssetBuffer Buffer;
            public string Identity, Key, Directory;
            public bool Hit;
            public int Sequence;
            public Action Verify;
            public Action<AssetLocation> Committed;
        }

        public AssetTaskExecution(DocumentProcessor processor, ActionAssetCache cache, string staging,
            string toolchain, string effectiveOptions, int requestedJobs, AssetExecutionMetrics metrics, CancellationToken cancellation)
            : this(processor, cache, staging, toolchain, effectiveOptions, requestedJobs, metrics, cancellation, null)
        { }

        internal AssetTaskExecution(DocumentProcessor processor, ActionAssetCache cache, string staging,
            string toolchain, string effectiveOptions, int requestedJobs, AssetExecutionMetrics metrics, CancellationToken cancellation,
            Func<BackendResourcePolicy> resourceSampler)
        {
            this.processor = processor; this.cache = cache; this.staging = staging;
            this.toolchain = toolchain; this.effectiveOptions = effectiveOptions;
            this.metrics = metrics; this.cancellation = cancellation;
            this.resourceSampler = resourceSampler;
            resources = resourceSampler == null ? BackendResourcePolicy.Capture(requestedJobs) : resourceSampler();
            capacity = resources.Capacity; byteBudget = resources.ByteBudget;
            bool previouslyObserved = metrics.LogicalProcessorCount != 0;
            metrics.EffectiveJobs = Math.Max(metrics.EffectiveJobs, capacity);
            metrics.MinimumEffectiveJobs = metrics.MinimumEffectiveJobs == 0 ? capacity : Math.Min(metrics.MinimumEffectiveJobs, capacity);
            metrics.LogicalProcessorCount = resources.ProcessorCount;
            metrics.TaskMemoryBudgetBytes = Math.Max(metrics.TaskMemoryBudgetBytes, byteBudget);
            metrics.MinimumTaskMemoryBudgetBytes = previouslyObserved ? Math.Min(metrics.MinimumTaskMemoryBudgetBytes, byteBudget) : byteBudget;
            metrics.PerTaskMemoryLimitBytes = metrics.MinimumTaskMemoryBudgetBytes / 2;
            metrics.ResourceMemoryQuerySucceeded = (!previouslyObserved || metrics.ResourceMemoryQuerySucceeded) && resources.MemoryQuerySucceeded;
        }

        void RefreshResources()
        {
            long now = Stopwatch.GetTimestamp();
            if (resourceSampler == null && now < nextResourceSample) return;
            nextResourceSample = now + Math.Max(1L, Stopwatch.Frequency / 10);
            resources = resourceSampler == null ? resources.RefreshMemoryBudget() : resourceSampler();
            // Pressure may reduce admission during a stream. Retain that lower
            // bound until its workers and results have all been joined; this
            // avoids repeatedly filling the pool as transient pressure changes.
            capacity = Math.Min(capacity, resources.Capacity);
            byteBudget = Math.Min(byteBudget, resources.ByteBudget);
            metrics.MinimumEffectiveJobs = Math.Min(metrics.MinimumEffectiveJobs, capacity);
            metrics.MinimumTaskMemoryBudgetBytes = Math.Min(metrics.MinimumTaskMemoryBudgetBytes, byteBudget);
            metrics.PerTaskMemoryLimitBytes = metrics.MinimumTaskMemoryBudgetBytes / 2;
            metrics.ResourceMemoryQuerySucceeded &= resources.MemoryQuerySucceeded;
        }

        bool Fits(long bytes) { return bytes >= 0 && retainedBytes <= byteBudget && bytes <= byteBudget - retainedBytes; }

        public void Enqueue(BinaryAsset asset, InstanceDeclaration declaration, IAssetBuilderPlugin plugin,
            Action compileCompatibility, Action verify, Action<AssetLocation> committed)
        {
            cancellation.ThrowIfCancellationRequested();
            RefreshResources();
            if (asset.GetLocation(AssetLocation.BasePatchStream, AssetLocationOption.None) == AssetLocation.BasePatchStream)
            {
                metrics.RecordBypass("base-patch-source");
                Flush(); verify(); committed(asset.Commit()); return;
            }
            while (pending.Count >= capacity) CommitOne();
            var task = new Pending { Asset = asset, Declaration = declaration, Verify = verify, Committed = committed,
                Identity = "execution:" + Guid.NewGuid().ToString("N"),
                Directory = Path.Combine(staging, "asset-tasks", (++sequence).ToString("D8")) };
            task.Sequence = sequence;
            Directory.CreateDirectory(task.Directory);
            declaration.CustomDataPath = Path.Combine(task.Directory, "result.cdata");
            long preparationBytes = PreparedBackend.EstimatePreparationBytes(declaration);
            while (pending.Count > 0 && !Fits(preparationBytes)) CommitOne();
            PreparedBackend prepared;
            string preparationReason;
            bool preparationAccepted;
            long preparationStarted = Stopwatch.GetTimestamp();
            try { preparationAccepted = PreparedBackend.TryPrepareWithReason(declaration, plugin, byteBudget / 2, out prepared, out preparationReason); }
            finally { metrics.PreparationTicks += Stopwatch.GetTimestamp() - preparationStarted; }
            if (!preparationAccepted)
            {
                // The preparer reports the actual rejecting predicate. This
                // independent partition also applies with persistent cache off.
                metrics.RecordPreparationRejected(preparationReason);
                metrics.RecordBypass(cache == null ? "cache-disabled" : "not-prepared");
                // Owner-only is also a barrier against unknown shared state.
                Flush(); cancellation.ThrowIfCancellationRequested(); asset.BeginExecution(task.Identity);
                metrics.Execute(declaration.Handle.TypeName, declaration.Document.SourcePath, () => { compileCompatibility(); return asset.Buffer; });
                task.Buffer = asset.Buffer;
                // Compatibility compilation uses Core's established CDATA path.
                if (declaration.HasCustomData && File.Exists(declaration.CustomDataPath))
                    File.Copy(declaration.CustomDataPath, Path.Combine(task.Directory, "result.cdata"), true);
                WriteResult(task);
                Finish(task);
                return;
            }
            task.Prepared = prepared;
            metrics.ExternalHashBytes += prepared.ExternalReadBytes;
            string typeName = declaration.Handle.TypeName, sourcePath = declaration.Document.SourcePath;
            try
            {
                RefreshResources();
                while (pending.Count >= capacity) CommitOne();
                while (pending.Count > 0 && !Fits(prepared.PreparedBytes)) CommitOne();
                long keyStarted = Stopwatch.GetTimestamp();
                try { task.Key = prepared.CanCache ? Key(prepared) : null; }
                finally { metrics.ActionKeyTicks += Stopwatch.GetTimestamp() - keyStarted; }
                if (task.Key != null) task.Identity = task.Key;
                asset.BeginExecution(task.Identity);
                ActionCacheMissReason diskMiss = ActionCacheMissReason.None;
                string missReason = null;
                if (cache == null) metrics.RecordBypass("cache-disabled");
                else if (task.Key == null) metrics.RecordBypass("not-prepared");
                else if (cache.TryRestore(task.Key, Path.Combine(task.Directory, "restored"), cancellation, out diskMiss))
                {
                    long restoreStarted = Stopwatch.GetTimestamp();
                    try
                    {
                        string restored = Path.Combine(task.Directory, "restored");
                        if (ReadResult(task, restored))
                        {
                            File.Copy(Path.Combine(restored, "result.bin"), Path.Combine(task.Directory, "result.bin"));
                            if (File.Exists(Path.Combine(restored, "result.cdata"))) File.Copy(Path.Combine(restored, "result.cdata"), Path.Combine(task.Directory, "result.cdata"));
                            task.Hit = true; metrics.Hits++;
                            metrics.RestoredBytes += new DirectoryInfo(restored).EnumerateFiles().Sum(x => x.Length);
                        }
                        else { cache.Invalidate(task.Key); missReason = "invalid-result-contract"; }
                        Directory.Delete(restored, true);
                    }
                    finally { metrics.ResultRestoreTicks += Stopwatch.GetTimestamp() - restoreStarted; }
                }
                else missReason = MissReasonCode(diskMiss);
                if (!task.Hit)
                {
                    // Preparing an external input may spend time hashing under
                    // a read lease. Cancellation arriving during that work must
                    // stop dispatch before another backend is started.
                    cancellation.ThrowIfCancellationRequested();
                    if (cache != null && task.Key != null) metrics.RecordMiss(missReason);
                    processor.RecordExecutionCompile();
                    if (prepared.CanRunParallel && capacity > 1 && prepared.PreparedBytes <= byteBudget / 2)
                    {
                        task.Worker = Task.Factory.StartNew(() => metrics.Execute(typeName,
                            sourcePath, prepared.Execute), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
                    }
                    else
                    {
                        Flush();
                        cancellation.ThrowIfCancellationRequested();
                        task.Buffer = metrics.Execute(declaration.Handle.TypeName, declaration.Document.SourcePath, prepared.Execute);
                    }
                }
                pending.Enqueue(task); retainedBytes += prepared.PreparedBytes;
                // jobs=1 is the same engine and commit path, without workers.
                if (capacity == 1 || (!task.Hit && task.Worker == null)) CommitOne();
            }
            catch { prepared.Dispose(); throw; }
        }

        public void DeferCompletion(Action complete)
        {
            completions.Enqueue(new KeyValuePair<int,Action>(sequence, complete));
            CompleteReadyDocuments();
        }
        void CompleteReadyDocuments()
        {
            while (completions.Count != 0 && completions.Peek().Key <= committedSequence) completions.Dequeue().Value();
        }
        public void Flush()
        {
            while (pending.Count != 0) CommitOne();
            // A completion may release XML used by an inheriting declaration.
            // Run only at an actual physical-result barrier, after all workers.
            CompleteReadyDocuments();
        }

        void CommitOne()
        {
            Pending task = pending.Dequeue();
            try
            {
                if (task.Worker != null)
                {
                    long waitStarted = Stopwatch.GetTimestamp();
                    try { task.Buffer = task.Worker.GetAwaiter().GetResult(); }
                    finally { metrics.WorkerWaitTicks += Stopwatch.GetTimestamp() - waitStarted; }
                }
                cancellation.ThrowIfCancellationRequested();
                if (!task.Hit) WriteResult(task);
                Finish(task);
            }
            finally { retainedBytes -= task.Prepared.PreparedBytes; task.Prepared.Dispose(); }
        }

        void Finish(Pending task)
        {
            long started = Stopwatch.GetTimestamp();
            try { FinishCore(task); }
            finally { metrics.ResultCommitTicks += Stopwatch.GetTimestamp() - started; }
        }

        void FinishCore(Pending task)
        {
            cancellation.ThrowIfCancellationRequested();
            string digest = Digest(task.Directory);
            string previous;
            if (outputDigests.TryGetValue(task.Asset.FileBase, out previous) && previous != digest)
                throw new InvalidDataException("Different action results collide in the legacy asset namespace: " + task.Asset.FileBase);
            outputDigests[task.Asset.FileBase] = digest;
            string custom = Path.Combine(task.Directory, "result.cdata");
            string output = Path.Combine(task.Asset.CustomDataOutputDirectory, task.Asset.FileBase + ".cdata");
            if (File.Exists(custom)) { Directory.CreateDirectory(task.Asset.CustomDataOutputDirectory); File.Copy(custom, output, true); }
            else if (task.Declaration.HasCustomData) throw new InvalidDataException("A custom-data result omitted its CDATA payload.");
            else if (File.Exists(output)) File.Delete(output);
            task.Declaration.CustomDataPath = output;
            task.Asset.Buffer = task.Buffer;
            task.Asset.AuthorizeExecutionResult(task.Identity, digest);
            metrics.SampleMemory();
            task.Verify();
            AssetLocation location = task.Asset.Commit();
            task.Committed(location);
            if (!task.Hit && cache != null && task.Key != null)
            {
                cache.Stage(task.Key, task.Directory, cancellation);
                metrics.WrittenBytes += new DirectoryInfo(task.Directory).EnumerateFiles().Sum(x => x.Length);
            }
            Directory.Delete(task.Directory, true);
            committedSequence = task.Sequence;
            CompleteReadyDocuments();
        }

        void WriteResult(Pending task)
        {
            long started = Stopwatch.GetTimestamp();
            try { WriteResultCore(task); }
            finally { metrics.ResultWriteTicks += Stopwatch.GetTimestamp() - started; }
        }

        static void WriteResultCore(Pending task)
        {
            AssetBuffer result = task.Buffer;
            if (result == null || result.InstanceData == null || result.RelocationData == null || result.ImportsData == null)
                throw new InvalidDataException("Backend did not return all three asset buffers.");
            using (var writer = new BinaryWriter(File.Open(Path.Combine(task.Directory, "result.bin"), FileMode.Create, FileAccess.Write, FileShare.None), Encoding.UTF8))
            {
                writer.Write(ResultFormat); writer.Write(task.Key ?? "");
                writer.Write(task.Declaration.HasCustomData);
                writer.Write(File.Exists(Path.Combine(task.Directory, "result.cdata")));
                foreach (byte[] bytes in new[] { result.InstanceData, result.RelocationData, result.ImportsData }) { writer.Write(bytes.Length); writer.Write(bytes); }
            }
        }

        static bool ReadResult(Pending task, string directory)
        {
            try
            {
                string[] files = Directory.GetFiles(directory);
                if (Directory.GetDirectories(directory).Length != 0 || files.Any(x => Path.GetFileName(x) != "result.bin" && Path.GetFileName(x) != "result.cdata")) return false;
                using (var reader = new BinaryReader(File.OpenRead(Path.Combine(directory, "result.bin")), Encoding.UTF8))
                {
                    if (ReadBoundedString(reader, 64) != ResultFormat || ReadBoundedString(reader, 64) != task.Key || reader.ReadBoolean() != task.Declaration.HasCustomData) return false;
                    bool hasCdata = reader.ReadBoolean();
                    if (hasCdata != File.Exists(Path.Combine(directory, "result.cdata")) || (task.Declaration.HasCustomData && !hasCdata)) return false;
                    var buffers = new byte[3][];
                    for (int i = 0; i < buffers.Length; i++)
                    {
                        int length = reader.ReadInt32();
                        if (length < 0 || length > reader.BaseStream.Length - reader.BaseStream.Position) return false;
                        buffers[i] = reader.ReadBytes(length);
                        if (buffers[i].Length != length) return false;
                    }
                    if (reader.BaseStream.Position != reader.BaseStream.Length) return false;
                    task.Buffer = new AssetBuffer { InstanceData = buffers[0], RelocationData = buffers[1], ImportsData = buffers[2] };
                    return true;
                }
            }
            catch (IOException) { return false; }
            catch (ArgumentException) { return false; }
            catch (FormatException) { return false; }
            catch (InvalidDataException) { return false; }
        }

        static string MissReasonCode(ActionCacheMissReason reason)
        {
            switch (reason)
            {
                case ActionCacheMissReason.EntryNotFound: return "entry-not-found";
                case ActionCacheMissReason.EntryInvalidated: return "entry-invalidated";
                case ActionCacheMissReason.EntryPending: return "entry-pending";
                case ActionCacheMissReason.InvalidManifest: return "invalid-manifest";
                case ActionCacheMissReason.InvalidPayload: return "invalid-payload";
                case ActionCacheMissReason.EntryUnavailable: return "entry-unavailable";
                default: throw new InvalidOperationException("A rejected action lookup must provide its miss reason.");
            }
        }

        static string ReadBoundedString(BinaryReader reader, int maximumBytes)
        {
            int length = 0, shift = 0;
            for (int i = 0; i < 5; i++)
            {
                byte value = reader.ReadByte();
                if (i == 4 && value > 7) throw new InvalidDataException("Invalid result string length.");
                length |= (value & 127) << shift;
                if ((value & 128) == 0)
                {
                    if (length < 0 || length > maximumBytes || length > reader.BaseStream.Length - reader.BaseStream.Position)
                        throw new InvalidDataException("Result string exceeds its contract.");
                    byte[] bytes = reader.ReadBytes(length);
                    if (bytes.Length != length) throw new EndOfStreamException();
                    return new UTF8Encoding(false, true).GetString(bytes);
                }
                shift += 7;
            }
            throw new InvalidDataException("Invalid result string length.");
        }

        string Key(PreparedBackend prepared)
        {
            using (var bytes = new MemoryStream()) using (var writer = new BinaryWriter(bytes, Encoding.UTF8))
            {
                writer.Write("NewModCompiler.AssetAction.v1"); writer.Write(toolchain); writer.Write(effectiveOptions);
                prepared.WriteIdentity(writer); writer.Flush(); return Hash(bytes.ToArray());
            }
        }

        static string Digest(string directory)
        {
            using (var bytes = new MemoryStream()) using (var writer = new BinaryWriter(bytes, Encoding.UTF8))
            {
                // Action identity itself is not part of semantic output identity.
                using (var reader = new BinaryReader(File.OpenRead(Path.Combine(directory, "result.bin")), Encoding.UTF8))
                {
                    ReadBoundedString(reader, 64); ReadBoundedString(reader, 64);
                    writer.Write(reader.ReadBytes(checked((int)(reader.BaseStream.Length - reader.BaseStream.Position))));
                }
                string cdata = Path.Combine(directory, "result.cdata");
                if (File.Exists(cdata)) using (var input = File.OpenRead(cdata)) using (var hash = SHA256.Create()) { writer.Write(input.Length); writer.Write(hash.ComputeHash(input)); }
                writer.Flush(); return Hash(bytes.ToArray());
            }
        }
        static string Hash(byte[] bytes) { using (var hash = SHA256.Create()) return BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant(); }

        public void Dispose()
        {
            if (disposed) return;
            // Failure/cancellation never abandons native code using input memory.
            while (pending.Count != 0)
            {
                Pending task = pending.Dequeue();
                try { if (task.Worker != null) task.Worker.GetAwaiter().GetResult(); }
                catch (Exception) { /* The primary failure remains authoritative. */ }
                finally { task.Prepared.Dispose(); }
            }
            completions.Clear(); disposed = true;
        }
    }
}
#endif
