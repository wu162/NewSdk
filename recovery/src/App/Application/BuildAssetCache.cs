using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace NewModCompiler.Application
{
    // Stores Core /bc asset files, never XML documents or linked output streams.
    // The caller owns dependency/implementation namespace validity. Integrity of
    // stored bytes is necessary but cannot itself authorize an input cache hit.
    public sealed class BuildAssetCache : IDisposable
    {
        readonly string format;
        readonly bool replaceInvalidEntries;
        readonly bool refreshExistingEntries;
        readonly HashSet<string> invalidEntries = new HashSet<string>(StringComparer.Ordinal);
        readonly Dictionary<string,string> replaced = new Dictionary<string,string>(StringComparer.Ordinal);
        const string IndexName = "index.bin";
        const string PendingName = "pending";
        readonly string outputRoot, cacheRoot, candidateRoot, namespaceFingerprint, owner = Guid.NewGuid().ToString("N");
        readonly Dictionary<string,string> staged = new Dictionary<string,string>(StringComparer.Ordinal);
        readonly List<string> installed = new List<string>();
        bool committed, completed, disposed;
        PublicationLock cacheLock;
        sealed class Entry { public string Name, Storage; public long Size; public string Hash; }

        public BuildAssetCache(string outputRoot, string namespaceFingerprint) : this(outputRoot, namespaceFingerprint, false) { }
        // Session packages use the same reversible integrity store, but never
        // share its directory, format, locks or task entries with /bc assets.
        internal BuildAssetCache(string outputRoot, string namespaceFingerprint, bool sessionPackage) : this(outputRoot, namespaceFingerprint, sessionPackage, false) { }
        internal BuildAssetCache(string outputRoot, string namespaceFingerprint, bool sessionPackage, bool refreshExistingEntries)
        {
            if (string.IsNullOrWhiteSpace(outputRoot)) throw new ArgumentException("An output root is required.");
            if (namespaceFingerprint == null || namespaceFingerprint.Length != 64) throw new ArgumentException("A SHA-256 namespace is required.");
            foreach (char c in namespaceFingerprint) if (!(c >= '0' && c <= '9') && !(c >= 'a' && c <= 'f')) throw new ArgumentException("A lowercase SHA-256 namespace is required.");
            this.outputRoot = Path.GetFullPath(outputRoot);
            RejectAncestors(this.outputRoot);
            this.namespaceFingerprint = namespaceFingerprint;
            replaceInvalidEntries = sessionPackage;
            this.refreshExistingEntries = refreshExistingEntries;
            format = sessionPackage ? "NewModCompiler.SessionPackage.v1" : "NewModCompiler.BuildAssets.v2";
            cacheRoot = Path.Combine(this.outputRoot, ".cache", sessionPackage ? "session-packages-v1" : "build-assets-v2");
            candidateRoot = Path.Combine(this.outputRoot, ".cache-candidate-" + owner);
            // Same namespace is held through the required-channel outcome.
            // Semaphore contention is an explicit failure, never a retry loop.
            cacheLock = PublicationLock.Acquire(Path.Combine(cacheRoot,"namespace-"+namespaceFingerprint));
        }
        public bool PrepareStream(string taskKey, string stagingCache, CancellationToken cancellation)
        {
            Active(); EnsureLock(); if (committed) throw new InvalidOperationException("The cache transaction is already committed.");
            string destination = Destination(stagingCache), source = Path.Combine(cacheRoot, TaskName(taskKey));
            cancellation.ThrowIfCancellationRequested(); RejectAncestors(source);
            List<Entry> entries;
            if (!TryRead(source, cancellation, out entries))
            {
                if (replaceInvalidEntries && Directory.Exists(source)) invalidEntries.Add(TaskName(taskKey));
                return false;
            }
            if (Directory.Exists(destination) && Directory.GetFileSystemEntries(destination).Length != 0) throw new IOException("The staging asset cache must be empty.");
            Directory.CreateDirectory(destination);
            foreach (Entry entry in entries)
            {
                cancellation.ThrowIfCancellationRequested();
                string target = Path.Combine(destination, entry.Name); Directory.CreateDirectory(Path.GetDirectoryName(target));
                File.Copy(Path.Combine(source, "files", entry.Storage), target, false);
                if (new FileInfo(target).Length != entry.Size || Hash(target) != entry.Hash) throw new IOException("Cached asset changed while copying.");
            }
            return true;
        }
        public void StageStream(string taskKey, string stagingCache, CancellationToken cancellation)
        {
            Active(); EnsureLock(); if (committed) throw new InvalidOperationException("The cache transaction is already committed.");
            string source = Destination(stagingCache), name = TaskName(taskKey);
            if (staged.ContainsKey(name)) throw new InvalidOperationException("A stream cache was staged twice.");
            var files = Files(source, cancellation);
            if (files.Count == 0) return; // Never advertise an empty tree as a hit.
            string target = Path.Combine(candidateRoot, name), data = Path.Combine(target, "files");
            RejectAncestors(target); Directory.CreateDirectory(data);
            var entries = new List<Entry>();
            foreach (string relative in files)
            {
                cancellation.ThrowIfCancellationRequested();
                string storage = entries.Count.ToString("x8",System.Globalization.CultureInfo.InvariantCulture);
                string input = Path.Combine(source, relative), copied = Path.Combine(data, storage);
                File.Copy(input, copied, false);
                entries.Add(new Entry { Name = relative, Storage = storage, Size = new FileInfo(copied).Length, Hash = Hash(copied) });
            }
            using (var writer = new BinaryWriter(File.Open(Path.Combine(target, IndexName), FileMode.CreateNew, FileAccess.Write, FileShare.None), Encoding.UTF8))
            {
                writer.Write(format); writer.Write(owner); writer.Write(entries.Count);
                foreach (Entry entry in entries) { writer.Write(entry.Name); writer.Write(entry.Size); writer.Write(entry.Hash); }
            }
            File.WriteAllText(Path.Combine(target,PendingName),owner);
            staged.Add(name,target);
        }
        // Session-specific XML/layout validation is performed by its wrapper
        // after the generic byte manifest. Only a proven invalid session may be
        // replaced, and its old directory remains recoverable until Complete.
        internal void InvalidateStream(string taskKey)
        {
            Active(); EnsureLock();
            if (!replaceInvalidEntries || committed) throw new InvalidOperationException("Only an uncommitted session package may be invalidated.");
            invalidEntries.Add(TaskName(taskKey));
        }
        // Incremental namespaces advance to the latest successful generation.
        // Its predecessor remains recoverable until publication is confirmed.
        public void Commit()
        {
            Active(); EnsureLock(); if (committed) return;
            RejectAncestors(cacheRoot); Directory.CreateDirectory(cacheRoot);
            try
            {
                foreach (var pair in staged)
                {
                    string destination = Path.Combine(cacheRoot, pair.Key); RejectAncestors(destination);
                    if (Directory.Exists(destination) || File.Exists(destination))
                    {
                        if ((!refreshExistingEntries && (!replaceInvalidEntries || !invalidEntries.Contains(pair.Key))) || !Directory.Exists(destination)) continue;
                        string backup = Path.Combine(candidateRoot,"replaced",pair.Key);
                        RejectAncestors(backup); Directory.CreateDirectory(Path.GetDirectoryName(backup));
                        Directory.Move(destination,backup); replaced.Add(destination,backup);
                    }
                    Directory.Move(pair.Value, destination); installed.Add(destination);
                }
                committed = true;
            }
            catch (Exception primary) {
                try { Rollback(); }
                catch (Exception cleanup) { throw new AggregateException("Build-cache commit and rollback both failed.",primary,cleanup); }
                throw;
            }
        }
        public void Rollback()
        {
            if (completed || disposed) return;
            try { RollbackCore(); }
            catch (Exception error) { error.Data["RetainedCacheEntries"] = RetainedEntries(); throw; }
        }
        void RollbackCore()
        {
            if (installed.Count != 0 || replaced.Count != 0) EnsureLock();
            foreach (string path in installed)
            {
                RejectAncestors(path);
                if (!Directory.Exists(path)) continue;
                using (var reader = new BinaryReader(File.OpenRead(Path.Combine(path, IndexName)), Encoding.UTF8))
                    if (reader.ReadString() != format || reader.ReadString() != owner) throw new IOException("Cache rollback no longer owns its installed entry.");
                // Delete payload before its exclusion marker. A sharing violation
                // must not expose an otherwise valid failed entry to readers.
                string data = Path.Combine(path,"files");
                if (Directory.Exists(data)) Directory.Delete(data,true);
                File.Delete(Path.Combine(path,IndexName));
                File.Delete(Path.Combine(path,PendingName));
                Directory.Delete(path,false);
            }
            installed.Clear();
            foreach (var pair in new List<KeyValuePair<string,string>>(replaced))
            {
                RejectAncestors(pair.Key); RejectAncestors(pair.Value);
                Directory.Move(pair.Value,pair.Key); replaced.Remove(pair.Key);
            }
            committed = false; ReleaseLock();
        }
        // Called only after required output channels have closed successfully.
        // There is no fallible installation here and cleanup cannot revoke success.
        public void Complete()
        {
            if (completed || disposed) return;
            if (!committed) return;
            // Pending markers are durable exclusion records. If this best-effort
            // cleanup fails, readers keep treating the entry as a miss, including
            // after a process crash or an unsuccessful rollback.
            foreach (string path in installed)
                try { File.Delete(Path.Combine(path,PendingName)); }
                catch (IOException) { } catch (UnauthorizedAccessException) { }
            completed = true; installed.Clear(); replaced.Clear(); TryDeleteCandidate(); ReleaseLock();
        }
        public void Dispose()
        {
            if (disposed) return;
            // A failed rollback remains retryable. Keep exact installed paths in
            // the error and preserve durable pending markers against reuse.
            try { Rollback(); }
            catch (Exception error) { error.Data["RetainedCacheEntries"] = RetainedEntries(); ReleaseLock(); throw; }
            TryDeleteCandidate(); disposed = true; ReleaseLock();
        }
        string[] RetainedEntries()
        {
            var paths = new List<string>(installed); paths.AddRange(replaced.Values); return paths.ToArray();
        }
        void ReleaseLock() { if (cacheLock != null) { cacheLock.Dispose(); cacheLock = null; } }
        void TryDeleteCandidate() { try { RejectAncestors(candidateRoot); if (Directory.Exists(candidateRoot)) Directory.Delete(candidateRoot,true); } catch (IOException) { } catch (UnauthorizedAccessException) { } }
        void EnsureLock() { if (cacheLock == null) cacheLock = PublicationLock.Acquire(Path.Combine(cacheRoot,"namespace-"+namespaceFingerprint)); }
        void Active() { if (disposed || completed) throw new ObjectDisposedException("BuildAssetCache"); }
        string Destination(string path)
        {
            if (string.IsNullOrEmpty(path) || !Path.IsPathRooted(path)) throw new ArgumentException("An absolute staging cache is required.");
            string full = Path.GetFullPath(path);
            if (string.Equals(outputRoot,full,StringComparison.OrdinalIgnoreCase) || !Contains(outputRoot,full) || Contains(Path.Combine(outputRoot,".cache"),full) || Contains(candidateRoot,full)) throw new IOException("The staging cache must be isolated beneath output.");
            RejectAncestors(full); return full;
        }
        static bool Contains(string root,string path) { return string.Equals(root,path,StringComparison.OrdinalIgnoreCase) || path.StartsWith(root.TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase); }
        string TaskName(string taskKey)
        {
            if (string.IsNullOrEmpty(taskKey)) throw new ArgumentException("A stream cache identity is required.");
            using (var bytes = new MemoryStream())
            using (var writer = new BinaryWriter(bytes,Encoding.UTF8))
            {
                writer.Write(namespaceFingerprint); writer.Write(taskKey); writer.Flush();
                using (var hash = SHA256.Create()) return Hex(hash.ComputeHash(bytes.ToArray()));
            }
        }
        static string Hex(byte[] bytes) { return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant(); }
        static string Hash(string path) { using (var hash = SHA256.Create()) using (var stream = File.OpenRead(path)) return Hex(hash.ComputeHash(stream)); }
        static void RejectAncestors(string path)
        {
            for (var item = new DirectoryInfo(path); item != null; item = item.Parent)
                if (item.Exists && (item.Attributes & FileAttributes.ReparsePoint) != 0) throw new IOException("Asset cache paths cannot use reparse points.");
            if (File.Exists(path) && (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0) throw new IOException("Asset cache files cannot use reparse points.");
        }
        static List<string> Files(string root, CancellationToken cancellation)
        {
            var result = new List<string>(); if (!Directory.Exists(root)) return result;
            RejectAncestors(root); Walk(root,root,result,cancellation); result.Sort(StringComparer.Ordinal); return result;
        }
        static void Walk(string root,string directory,List<string> files,CancellationToken cancellation)
        {
            foreach (string path in Directory.GetFileSystemEntries(directory))
            {
                cancellation.ThrowIfCancellationRequested(); FileAttributes attributes = File.GetAttributes(path);
                if ((attributes & FileAttributes.ReparsePoint) != 0) throw new IOException("Asset cache entries cannot use reparse points.");
                if ((attributes & FileAttributes.Directory) != 0) Walk(root,path,files,cancellation);
                else files.Add(path.Substring(root.TrimEnd(Path.DirectorySeparatorChar).Length+1));
            }
        }
        bool TryRead(string root,CancellationToken cancellation,out List<Entry> entries)
        {
            entries = new List<Entry>(); if (!Directory.Exists(root) || File.Exists(Path.Combine(root,PendingName))) return false;
            try
            {
                RejectAncestors(root); string index = Path.Combine(root,IndexName); RejectAncestors(index);
                foreach (string item in Directory.GetFileSystemEntries(root))
                {
                    string name = Path.GetFileName(item);
                    if (name != IndexName && name != "files") return false;
                }
                using (var reader = new BinaryReader(File.OpenRead(index),Encoding.UTF8))
                {
                    if (reader.ReadString() != format) return false; reader.ReadString();
                    int count = reader.ReadInt32(); if (count <= 0 || count > 1000000) return false;
                    var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    for (int i=0;i<count;i++)
                    {
                        cancellation.ThrowIfCancellationRequested();
                        var entry = new Entry {Name=reader.ReadString(),Size=reader.ReadInt64(),Hash=reader.ReadString(),Storage=i.ToString("x8",System.Globalization.CultureInfo.InvariantCulture)};
                        if (string.IsNullOrEmpty(entry.Name) || Path.IsPathRooted(entry.Name) || entry.Name.IndexOf(':') >= 0 || entry.Size < 0 || entry.Hash.Length != 64 || !names.Add(entry.Name)) return false;
                        foreach (string part in entry.Name.Split(new[] {'\\','/'})) if (part.Length == 0 || part == "." || part == "..") return false;
                        string path = Path.Combine(root,"files",entry.Storage); RejectAncestors(path);
                        if (!File.Exists(path) || new FileInfo(path).Length != entry.Size || Hash(path) != entry.Hash) return false;
                        entries.Add(entry);
                    }
                    if (reader.BaseStream.Position != reader.BaseStream.Length) return false;
                }
                var actual = Files(Path.Combine(root,"files"),cancellation);
                if (actual.Count != entries.Count) return false;
                return true;
            }
            catch (IOException) { entries.Clear(); return false; }
            catch (UnauthorizedAccessException) { entries.Clear(); return false; }
            catch (ArgumentException) { entries.Clear(); return false; }
        }
    }
}
