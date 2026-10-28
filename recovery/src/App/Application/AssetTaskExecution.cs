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
        public int BackendCalls, MaximumConcurrency, Hits, Misses;
        public long RestoredBytes, WrittenBytes;
        public readonly List<BackendInterval> Intervals = new List<BackendInterval>();
        public AssetBuffer Execute(string type, string source, Func<AssetBuffer> action)
        {
            var interval = new BackendInterval { typeName = type, sourcePath = source,
                threadId = Thread.CurrentThread.ManagedThreadId };
            lock (gate) { BackendCalls++; active++; MaximumConcurrency = Math.Max(MaximumConcurrency, active); interval.startTicks = Stopwatch.GetTimestamp(); }
            try { return action(); }
            finally { lock (gate) { interval.endTicks = Stopwatch.GetTimestamp(); active--; if (Intervals.Count < 512) Intervals.Add(interval); } }
        }
    }

    // One owner, bounded independent backend work, deterministic owner commit.
    // Frontend validation and linking remain in DocumentProcessor.
    internal sealed class AssetTaskExecution : IAssetExecution, IDisposable
    {
        const string ResultFormat = "NewModCompiler.ActionResult.v1";
        const long ByteBudget = 256L * 1024 * 1024;
        readonly DocumentProcessor processor;
        readonly ActionAssetCache cache;
        readonly AssetExecutionMetrics metrics;
        readonly string staging, toolchain, effectiveOptions;
        readonly CancellationToken cancellation;
        readonly int capacity;
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
        {
            this.processor = processor; this.cache = cache; this.staging = staging;
            this.toolchain = toolchain; this.effectiveOptions = effectiveOptions;
            this.metrics = metrics; this.cancellation = cancellation;
            // W3X expansion is expensive. Never size a thread/queue from raw N.
            capacity = Math.Min(requestedJobs, 2);
        }

        public void Enqueue(BinaryAsset asset, InstanceDeclaration declaration, IAssetBuilderPlugin plugin,
            Action compileCompatibility, Action verify, Action<AssetLocation> committed)
        {
            cancellation.ThrowIfCancellationRequested();
            if (asset.GetLocation(AssetLocation.BasePatchStream, AssetLocationOption.None) == AssetLocation.BasePatchStream)
            {
                Flush(); verify(); committed(asset.Commit()); return;
            }
            while (pending.Count >= capacity) CommitOne();
            var task = new Pending { Asset = asset, Declaration = declaration, Verify = verify, Committed = committed,
                Identity = "execution:" + Guid.NewGuid().ToString("N"),
                Directory = Path.Combine(staging, "asset-tasks", (++sequence).ToString("D8")) };
            task.Sequence = sequence;
            Directory.CreateDirectory(task.Directory);
            declaration.CustomDataPath = Path.Combine(task.Directory, "result.cdata");
            PreparedBackend prepared;
            if (!PreparedBackend.TryPrepare(declaration, plugin, out prepared))
            {
                // Owner-only is also a barrier against unknown shared state.
                Flush(); asset.BeginExecution(task.Identity);
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
            try
            {
                if (prepared.PreparedBytes > ByteBudget) Flush();
                else while (pending.Count > 0 && retainedBytes + prepared.PreparedBytes > ByteBudget) CommitOne();
                task.Key = prepared.CanCache ? Key(prepared) : null;
                if (task.Key != null) task.Identity = task.Key;
                asset.BeginExecution(task.Identity);
                if (cache != null && task.Key != null && cache.TryRestore(task.Key, Path.Combine(task.Directory, "restored"), cancellation))
                {
                    string restored = Path.Combine(task.Directory, "restored");
                    if (ReadResult(task, restored))
                    {
                        File.Copy(Path.Combine(restored, "result.bin"), Path.Combine(task.Directory, "result.bin"));
                        if (File.Exists(Path.Combine(restored, "result.cdata"))) File.Copy(Path.Combine(restored, "result.cdata"), Path.Combine(task.Directory, "result.cdata"));
                        task.Hit = true; metrics.Hits++;
                        metrics.RestoredBytes += new DirectoryInfo(restored).EnumerateFiles().Sum(x => x.Length);
                    }
                    else cache.Invalidate(task.Key);
                    Directory.Delete(restored, true);
                }
                if (!task.Hit)
                {
                    if (cache != null && task.Key != null) metrics.Misses++;
                    processor.RecordExecutionCompile();
                    if (prepared.CanRunParallel && capacity > 1 && prepared.PreparedBytes <= ByteBudget / 2)
                    {
                        task.Worker = Task.Factory.StartNew(() => metrics.Execute(declaration.Handle.TypeName,
                            declaration.Document.SourcePath, prepared.Execute), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
                    }
                    else
                    {
                        Flush();
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
                if (task.Worker != null) task.Buffer = task.Worker.GetAwaiter().GetResult();
                cancellation.ThrowIfCancellationRequested();
                if (!task.Hit) WriteResult(task);
                Finish(task);
            }
            finally { retainedBytes -= task.Prepared.PreparedBytes; task.Prepared.Dispose(); }
        }

        void Finish(Pending task)
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

        static void WriteResult(Pending task)
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
