using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace NewModCompiler.Application
{
    public enum ActionCacheMissReason
    {
        None, EntryNotFound, EntryInvalidated, EntryPending,
        InvalidManifest, InvalidPayload, EntryUnavailable
    }

    // An action key authorizes reuse; neither the legacy FileBase nor the input
    // root inventory is a store identity. The owner supplies the complete result
    // directory, including metadata and the explicit CDATA presence contract.
    public sealed class ActionAssetCache : IDisposable
    {
        const string Format = "NewModCompiler.ActionAssets.v1";
        const string IndexName = "index.bin", PendingName = "pending", FilesName = "files";
        const int MaximumMembers = 1000000;
        readonly string outputRoot, cacheRoot, candidateRoot, owner = Guid.NewGuid().ToString("N");
        readonly long maximumBytes;
        readonly int maximumEntries;
        readonly Dictionary<string,string> staged = new Dictionary<string,string>(StringComparer.Ordinal);
        readonly Dictionary<string,string> replaced = new Dictionary<string,string>(StringComparer.Ordinal);
        readonly HashSet<string> invalidated = new HashSet<string>(StringComparer.Ordinal);
        readonly List<string> installed = new List<string>();
        PublicationLock cacheLock;
        int candidateSequence;
        bool committed, completed, disposed;
        public long HashReadBytes { get; private set; }
        public long CopyBytes { get; private set; }
        // Inclusive Stopwatch ticks. Validation is nested in lookup/stage/commit;
        // these diagnostic counters never participate in cache identity.
        public long LookupTicks { get; private set; }
        public long ValidationTicks { get; private set; }
        public long MaterializationTicks { get; private set; }
        public long StageTicks { get; private set; }

        sealed class Member { public string Name, Storage, Hash; public long Size; }
        sealed class Manifest { public string Owner, Digest; public List<Member> Members; public long Bytes; }
        sealed class Eviction { public string Path; public long Bytes; public DateTime Created; }

        public ActionAssetCache(string outputRoot, long maximumBytes = 4294967296L, int maximumEntries = 4096)
        {
            if (string.IsNullOrWhiteSpace(outputRoot)) throw new ArgumentException("An output root is required.");
            if (maximumBytes < 0 || maximumEntries < 0) throw new ArgumentOutOfRangeException("maximumBytes", "Action cache limits cannot be negative.");
            this.outputRoot = Path.GetFullPath(outputRoot);
            this.maximumBytes = maximumBytes; this.maximumEntries = maximumEntries;
            RejectAncestors(this.outputRoot);
            cacheRoot = Path.Combine(this.outputRoot, ".cache", "action-cache-v1");
            candidateRoot = Path.Combine(this.outputRoot, ".action-cache-candidate-" + owner);
            // One writer/reader owner for the whole store, held through required
            // report and sink completion. Contention is explicit, never a retry.
            EnsureLock();
        }

        public bool TryRestore(string actionKey, string destination, CancellationToken cancellation)
        {
            ActionCacheMissReason ignored;
            return TryRestore(actionKey, destination, cancellation, out ignored);
        }

        // Reports the rejection of this lookup only. Installation validation,
        // capacity scans and Stage do not manufacture cache lookup misses.
        public bool TryRestore(string actionKey, string destination, CancellationToken cancellation, out ActionCacheMissReason missReason)
        {
            long started = Stopwatch.GetTimestamp();
            try { return TryRestoreCore(actionKey, destination, cancellation, out missReason); }
            finally { LookupTicks += Stopwatch.GetTimestamp() - started; }
        }

        bool TryRestoreCore(string actionKey, string destination, CancellationToken cancellation, out ActionCacheMissReason missReason)
        {
            missReason = ActionCacheMissReason.None;
            Active(); EnsureLock(); Uncommitted(); ValidateKey(actionKey);
            cancellation.ThrowIfCancellationRequested();
            string target = Staging(destination), source;
            if (!staged.TryGetValue(actionKey, out source))
            {
                if (invalidated.Contains(actionKey)) { missReason = ActionCacheMissReason.EntryInvalidated; return false; }
                source = Path.Combine(cacheRoot, actionKey);
            }
            cancellation.ThrowIfCancellationRequested();
            Manifest manifest;
            if (!TryRead(source, actionKey, staged.ContainsKey(actionKey), true, cancellation, out manifest, out missReason)) return false;
            if (File.Exists(target) || (Directory.Exists(target) && Directory.GetFileSystemEntries(target).Length != 0))
                throw new IOException("An action restore destination must be absent or empty.");
            string temporary = target + ".restore-" + owner;
            RejectAncestors(temporary);
            if (Directory.Exists(temporary) || File.Exists(temporary)) throw new IOException("An action restore temporary path already exists.");
            long materializationStarted = Stopwatch.GetTimestamp();
            Directory.CreateDirectory(temporary);
            try
            {
                foreach (Member member in manifest.Members)
                {
                    cancellation.ThrowIfCancellationRequested();
                    string path = Path.Combine(temporary, member.Name);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    Copy(Path.Combine(source, FilesName, member.Storage), path, cancellation);
                    if (new FileInfo(path).Length != member.Size || Hash(path, cancellation) != member.Hash)
                        throw new IOException("An action payload changed during restoration.");
                }
                cancellation.ThrowIfCancellationRequested();
                RejectAncestors(target);
                if (Directory.Exists(target)) Directory.Delete(target, false);
                Directory.Move(temporary, target);
                return true;
            }
            finally
            {
                try { DeletePrivateTree(temporary); }
                finally { MaterializationTicks += Stopwatch.GetTimestamp() - materializationStarted; }
            }
        }

        public void Stage(string actionKey, string source, CancellationToken cancellation)
        {
            long started = Stopwatch.GetTimestamp();
            try { StageCore(actionKey, source, cancellation); }
            finally { StageTicks += Stopwatch.GetTimestamp() - started; }
        }

        void StageCore(string actionKey, string source, CancellationToken cancellation)
        {
            Active(); EnsureLock(); Uncommitted(); ValidateKey(actionKey);
            source = Staging(source);
            cancellation.ThrowIfCancellationRequested();
            if (!Directory.Exists(source)) throw new DirectoryNotFoundException("The complete action result directory is missing.");
            List<string> names = Files(source, cancellation);
            // Even a zero-buffer result needs the caller's result metadata. An
            // empty tree cannot prove which optional files were legally absent.
            if (names.Count == 0) throw new InvalidDataException("An action result needs a complete nonempty result contract.");
            // The manifest carries the full action identity. Repeating a
            // 64-byte key plus a GUID in this private filename needlessly
            // exceeded Win32 MAX_PATH under normal Actions output roots.
            string target = Path.Combine(candidateRoot, checked(++candidateSequence).ToString("x8", CultureInfo.InvariantCulture));
            string data = Path.Combine(target, FilesName);
            RejectAncestors(target); Directory.CreateDirectory(data);
            bool retained = false;
            try
            {
                var manifest = new Manifest { Owner = owner, Members = new List<Member>() };
                foreach (string name in names)
                {
                    cancellation.ThrowIfCancellationRequested(); ValidateName(name);
                    string storage = manifest.Members.Count.ToString("x8", CultureInfo.InvariantCulture);
                    string copied = Path.Combine(data, storage);
                    Copy(Path.Combine(source, name), copied, cancellation);
                    long size = new FileInfo(copied).Length;
                    manifest.Bytes = checked(manifest.Bytes + size);
                    manifest.Members.Add(new Member { Name = name, Storage = storage, Size = size, Hash = Hash(copied, cancellation) });
                }
                manifest.Digest = Digest(manifest.Members);
                WriteManifest(target, actionKey, manifest);
                File.WriteAllText(Path.Combine(target, PendingName), owner);
                // A different result for one strong action identity indicates a
                // missing dependency or nondeterminism. Never overwrite trust.
                string existing;
                bool isCandidate = staged.TryGetValue(actionKey, out existing);
                if (!isCandidate) existing = Path.Combine(cacheRoot, actionKey);
                Manifest previous;
                if ((isCandidate || !invalidated.Contains(actionKey)) && TryRead(existing, actionKey, isCandidate, true, cancellation, out previous))
                {
                    if (previous.Digest != manifest.Digest) throw new InvalidDataException("One action key produced different complete results: " + actionKey);
                    return;
                }
                if (isCandidate) throw new InvalidDataException("A staged action result lost its integrity: " + actionKey);
                cancellation.ThrowIfCancellationRequested();
                staged.Add(actionKey, target); retained = true;
            }
            finally { if (!retained) DeletePrivateTree(target); }
        }

        // Byte integrity alone cannot establish the caller's semantic contract.
        // A payload which fails result/schema/CDATA validation is repairable,
        // with the previous object retained for transaction rollback.
        public void Invalidate(string actionKey)
        {
            Active(); EnsureLock(); Uncommitted(); ValidateKey(actionKey);
            string candidate;
            if (staged.TryGetValue(actionKey, out candidate))
            {
                DeletePrivateTree(candidate); staged.Remove(actionKey);
            }
            invalidated.Add(actionKey);
        }

        public void Commit()
        {
            Active(); EnsureLock(); if (committed) return;
            RejectAncestors(cacheRoot); Directory.CreateDirectory(cacheRoot);
            try
            {
                foreach (var pair in staged)
                {
                    Manifest candidate;
                    if (!TryRead(pair.Value, pair.Key, true, true, CancellationToken.None, out candidate))
                        throw new IOException("A candidate action result failed installation validation.");
                    string destination = Path.Combine(cacheRoot, pair.Key);
                    RejectAncestors(destination);
                    if (Directory.Exists(destination) || File.Exists(destination))
                    {
                        Manifest previous;
                        if (!invalidated.Contains(pair.Key) && TryRead(destination, pair.Key, false, true, CancellationToken.None, out previous))
                        {
                            if (previous.Digest != candidate.Digest) throw new InvalidDataException("One action key produced different complete results: " + pair.Key);
                            continue;
                        }
                        // Only invalid/unconfirmed entries are replaceable. The
                        // old object remains recoverable until required success.
                        string backup = Path.Combine(candidateRoot, "replaced", pair.Key);
                        RejectAncestors(backup); Directory.CreateDirectory(Path.GetDirectoryName(backup));
                        if (Directory.Exists(destination)) Directory.Move(destination, backup);
                        else File.Move(destination, backup);
                        replaced.Add(destination, backup);
                    }
                    Directory.Move(pair.Value, destination);
                    installed.Add(destination);
                }
                committed = true;
            }
            catch (Exception primary)
            {
                try { Rollback(); }
                catch (Exception cleanup) { throw new AggregateException("Action cache commit and rollback both failed.", primary, cleanup); }
                throw;
            }
        }

        public void Rollback()
        {
            if (completed || disposed) return;
            try
            {
                if (installed.Count != 0 || replaced.Count != 0) EnsureLock();
                foreach (string path in new List<string>(installed))
                {
                    RejectAncestors(path);
                    if (Directory.Exists(path))
                    {
                        using (var reader = new BinaryReader(File.OpenRead(Path.Combine(path, IndexName)), Encoding.UTF8))
                            if (ReadText(reader, 128) != Format || ReadText(reader, 32) != owner)
                                throw new IOException("Action cache rollback no longer owns its entry.");
                        // Keep the exclusion marker until all payload bytes and
                        // the manifest have gone, including sharing failures.
                        string data = Path.Combine(path, FilesName);
                        if (Directory.Exists(data)) Directory.Delete(data, true);
                        File.Delete(Path.Combine(path, IndexName));
                        File.Delete(Path.Combine(path, PendingName));
                        Directory.Delete(path, false);
                    }
                    installed.Remove(path);
                }
                foreach (var pair in new Dictionary<string,string>(replaced))
                {
                    RejectAncestors(pair.Key); RejectAncestors(pair.Value);
                    if (Directory.Exists(pair.Value)) Directory.Move(pair.Value, pair.Key);
                    else File.Move(pair.Value, pair.Key);
                    replaced.Remove(pair.Key);
                }
                committed = false;
                ReleaseLock();
            }
            catch (Exception error) { error.Data["RetainedCacheEntries"] = RetainedEntries(); throw; }
        }

        // Called after all required outputs and sinks have succeeded. Pending
        // cleanup and capacity maintenance can only turn a future hit into miss.
        public void Complete()
        {
            if (completed || disposed || !committed) return;
            foreach (string path in installed)
                try { File.Delete(Path.Combine(path, PendingName)); }
                catch (IOException) { } catch (UnauthorizedAccessException) { }
            completed = true;
            try { Prune(); } catch (InvalidDataException) { } catch (IOException) { } catch (UnauthorizedAccessException) { } catch (OverflowException) { }
            installed.Clear(); replaced.Clear(); DeletePrivateTree(candidateRoot); ReleaseLock();
        }

        public void Dispose()
        {
            if (disposed) return;
            try { Rollback(); }
            catch (Exception error) { error.Data["RetainedCacheEntries"] = RetainedEntries(); ReleaseLock(); throw; }
            DeletePrivateTree(candidateRoot); disposed = true; ReleaseLock();
        }

        void Prune()
        {
            if (!Directory.Exists(cacheRoot)) return;
            var entries = new List<Eviction>(); long bytes = 0;
            foreach (string path in Directory.GetDirectories(cacheRoot))
            {
                string key = Path.GetFileName(path); if (!IsKey(key)) continue;
                Manifest manifest;
                // Read only manifests for capacity accounting. Payload hashing
                // stays on demand; size/time never authorize an action hit.
                if (!TryRead(path, key, false, false, CancellationToken.None, out manifest))
                {
                    // The root lock excludes a live owning build. Unknown or
                    // crash-pending complete key directories are safe to drop,
                    // but never follow a reparse point during cleanup.
                    DeleteEntry(path); continue;
                }
                bytes = checked(bytes + manifest.Bytes);
                entries.Add(new Eviction { Path = path, Bytes = manifest.Bytes, Created = Directory.GetCreationTimeUtc(path) });
            }
            entries.Sort((left, right) => { int order = left.Created.CompareTo(right.Created); return order != 0 ? order : StringComparer.Ordinal.Compare(left.Path, right.Path); });
            int count = entries.Count;
            foreach (Eviction entry in entries)
            {
                if (count <= maximumEntries && bytes <= maximumBytes) break;
                DeleteEntry(entry.Path);
                bytes -= entry.Bytes; --count;
            }
        }

        void DeleteEntry(string path)
        {
            RejectAncestors(path);
            var ignored = new List<string>();
            Walk(path, path, ignored, CancellationToken.None);
            // Exclude first. Interrupted cleanup stays a miss rather than
            // advertising a partially deleted complete result.
            File.WriteAllText(Path.Combine(path, PendingName), owner);
            Directory.Delete(path, true);
        }

        string[] RetainedEntries() { var paths = new List<string>(installed); paths.AddRange(replaced.Values); return paths.ToArray(); }
        void EnsureLock() { if (cacheLock == null) { RejectAncestors(cacheRoot); cacheLock = PublicationLock.Acquire(cacheRoot); } }
        void ReleaseLock() { if (cacheLock != null) { cacheLock.Dispose(); cacheLock = null; } }
        void Active() { if (disposed || completed) throw new ObjectDisposedException("ActionAssetCache"); }
        void Uncommitted() { if (committed) throw new InvalidOperationException("The action cache is already committed."); }
        static bool Contains(string root, string path) { return string.Equals(root, path, StringComparison.OrdinalIgnoreCase) || path.StartsWith(root.TrimEnd('\\', '/') + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase); }
        string Staging(string path)
        {
            if (string.IsNullOrEmpty(path) || !Path.IsPathRooted(path)) throw new ArgumentException("An absolute private action staging path is required.");
            string full = Path.GetFullPath(path);
            if (string.Equals(full, outputRoot, StringComparison.OrdinalIgnoreCase) || !Contains(outputRoot, full) || Contains(Path.Combine(outputRoot, ".cache"), full) || Contains(candidateRoot, full))
                throw new IOException("Action staging must be isolated beneath output and outside durable cache.");
            RejectAncestors(full); return full;
        }
        static bool IsKey(string key)
        {
            if (key == null || key.Length != 64) return false;
            foreach (char value in key) if (!(value >= '0' && value <= '9') && !(value >= 'a' && value <= 'f')) return false;
            return true;
        }
        static void ValidateKey(string key) { if (!IsKey(key)) throw new ArgumentException("An action identity must be a lowercase SHA-256 digest."); }
        static void ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length > 32767 || Path.IsPathRooted(name) || name.IndexOf(':') >= 0 || name.IndexOf('\0') >= 0) throw new InvalidDataException("Invalid action result member path.");
            foreach (string part in name.Split(new[] { '\\', '/' }))
            {
                if (part.Length == 0 || part == "." || part == ".." || part.TrimEnd(' ', '.') != part || part.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                    throw new InvalidDataException("Invalid action result member path.");
                string device = part.Split('.')[0].ToUpperInvariant();
                if (device == "CON" || device == "PRN" || device == "AUX" || device == "NUL" || (device.Length == 4 && (device.StartsWith("COM", StringComparison.Ordinal) || device.StartsWith("LPT", StringComparison.Ordinal)) && device[3] >= '1' && device[3] <= '9'))
                    throw new InvalidDataException("Action result paths cannot refer to device files.");
            }
        }
        static void RejectAncestors(string path)
        {
            for (var item = new DirectoryInfo(path); item != null; item = item.Parent)
                if (item.Exists && (item.Attributes & FileAttributes.ReparsePoint) != 0) throw new IOException("Action cache paths cannot use reparse points.");
            if (File.Exists(path) && (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0) throw new IOException("Action cache files cannot use reparse points.");
        }
        static List<string> Files(string root, CancellationToken cancellation)
        {
            var names = new List<string>(); RejectAncestors(root); Walk(root, root, names, cancellation); names.Sort(StringComparer.Ordinal);
            var distinct = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string name in names) { ValidateName(name); if (!distinct.Add(name)) throw new InvalidDataException("Duplicate action result member."); }
            return names;
        }
        static void Walk(string root, string directory, List<string> names, CancellationToken cancellation)
        {
            foreach (string path in Directory.GetFileSystemEntries(directory))
            {
                cancellation.ThrowIfCancellationRequested();
                FileAttributes attributes = File.GetAttributes(path);
                if ((attributes & FileAttributes.ReparsePoint) != 0) throw new IOException("Action result members cannot use reparse points.");
                if ((attributes & FileAttributes.Directory) != 0) Walk(root, path, names, cancellation);
                else { if (names.Count == MaximumMembers) throw new InvalidDataException("Too many action result members."); names.Add(path.Substring(root.TrimEnd('\\', '/').Length + 1)); }
            }
        }
        void Copy(string source, string destination, CancellationToken cancellation)
        {
            RejectAncestors(source); RejectAncestors(destination);
            using (var input = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var output = new FileStream(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                byte[] buffer = new byte[81920]; int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) != 0) { cancellation.ThrowIfCancellationRequested(); output.Write(buffer, 0, read); CopyBytes += read; }
                cancellation.ThrowIfCancellationRequested(); output.Flush(true);
            }
        }
        static string Hex(byte[] value) { return BitConverter.ToString(value).Replace("-", "").ToLowerInvariant(); }
        string Hash(string path, CancellationToken cancellation)
        {
            using (var hash = SHA256.Create())
            using (var input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] buffer = new byte[81920]; int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) != 0) { cancellation.ThrowIfCancellationRequested(); hash.TransformBlock(buffer, 0, read, buffer, 0); HashReadBytes += read; }
                cancellation.ThrowIfCancellationRequested(); hash.TransformFinalBlock(new byte[0], 0, 0); return Hex(hash.Hash);
            }
        }
        static string Digest(List<Member> members)
        {
            using (var bytes = new MemoryStream())
            using (var writer = new BinaryWriter(bytes, Encoding.UTF8))
            {
                writer.Write("NewModCompiler.ActionResultDigest.v1"); writer.Write(members.Count);
                foreach (Member member in members) { writer.Write(member.Name); writer.Write(member.Size); writer.Write(member.Hash); }
                writer.Flush(); using (var hash = SHA256.Create()) return Hex(hash.ComputeHash(bytes.ToArray()));
            }
        }
        static void WriteManifest(string root, string key, Manifest manifest)
        {
            using (var stream = new FileStream(Path.Combine(root, IndexName), FileMode.CreateNew, FileAccess.Write, FileShare.None))
            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                writer.Write(Format); writer.Write(manifest.Owner); writer.Write(key); writer.Write(manifest.Digest); writer.Write(manifest.Members.Count);
                foreach (Member member in manifest.Members) { writer.Write(member.Name); writer.Write(member.Size); writer.Write(member.Hash); }
                writer.Flush(); stream.Flush(true);
            }
        }
        // BinaryReader.ReadString accepts an attacker-controlled allocation
        // length. Bound each field before allocating and reject invalid UTF-8.
        static string ReadText(BinaryReader reader, int maximumBytes)
        {
            int count = 0, shift = 0;
            for (int i = 0; i < 5; ++i)
            {
                byte current = reader.ReadByte();
                if (i == 4 && (current & 0xf8) != 0) throw new InvalidDataException("Malformed action manifest string length.");
                count |= (current & 0x7f) << shift;
                if ((current & 0x80) == 0)
                {
                    if (count < 0 || count > maximumBytes || count > reader.BaseStream.Length - reader.BaseStream.Position) throw new InvalidDataException("Action manifest string exceeds its field boundary.");
                    byte[] bytes = reader.ReadBytes(count);
                    if (bytes.Length != count) throw new EndOfStreamException();
                    return new UTF8Encoding(false, true).GetString(bytes);
                }
                shift += 7;
            }
            throw new InvalidDataException("Malformed action manifest string length.");
        }
        bool TryRead(string root, string key, bool ownCandidate, bool verifyPayload, CancellationToken cancellation, out Manifest manifest)
        {
            ActionCacheMissReason ignored;
            return TryRead(root, key, ownCandidate, verifyPayload, cancellation, out manifest, out ignored);
        }
        bool TryRead(string root, string key, bool ownCandidate, bool verifyPayload, CancellationToken cancellation,
            out Manifest manifest, out ActionCacheMissReason missReason)
        {
            long started = Stopwatch.GetTimestamp();
            try { return TryReadCore(root, key, ownCandidate, verifyPayload, cancellation, out manifest, out missReason); }
            finally { ValidationTicks += Stopwatch.GetTimestamp() - started; }
        }

        bool TryReadCore(string root, string key, bool ownCandidate, bool verifyPayload, CancellationToken cancellation,
            out Manifest manifest, out ActionCacheMissReason missReason)
        {
            manifest = null;
            missReason = ActionCacheMissReason.InvalidManifest;
            try
            {
                cancellation.ThrowIfCancellationRequested(); RejectAncestors(root);
                FileAttributes attributes;
                try { attributes = File.GetAttributes(root); }
                catch (FileNotFoundException) { missReason = ActionCacheMissReason.EntryNotFound; return false; }
                catch (DirectoryNotFoundException) { missReason = ActionCacheMissReason.EntryNotFound; return false; }
                if ((attributes & FileAttributes.Directory) == 0) return false;
                string pending = Path.Combine(root, PendingName); RejectAncestors(pending);
                if (ownCandidate) { if (!File.Exists(pending) || File.ReadAllText(pending) != owner) { missReason = ActionCacheMissReason.EntryPending; return false; } }
                else if (File.Exists(pending) || Directory.Exists(pending)) { missReason = ActionCacheMissReason.EntryPending; return false; }
                foreach (string item in Directory.GetFileSystemEntries(root))
                {
                    string name = Path.GetFileName(item);
                    if (name != IndexName && name != FilesName && !(ownCandidate && name == PendingName)) return false;
                }
                var result = new Manifest { Members = new List<Member>() };
                string index = Path.Combine(root, IndexName); RejectAncestors(index);
                using (var reader = new BinaryReader(File.OpenRead(index), Encoding.UTF8))
                {
                    if (ReadText(reader, 128) != Format) return false;
                    result.Owner = ReadText(reader, 32);
                    Guid ownerId; if (!Guid.TryParseExact(result.Owner, "N", out ownerId) || (ownCandidate && result.Owner != owner)) return false;
                    if (ReadText(reader, 64) != key) return false;
                    result.Digest = ReadText(reader, 64); if (!IsKey(result.Digest)) return false;
                    int count = reader.ReadInt32(); if (count <= 0 || count > MaximumMembers) return false;
                    var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    string previous = null;
                    for (int i = 0; i < count; ++i)
                    {
                        cancellation.ThrowIfCancellationRequested();
                        var member = new Member { Name = ReadText(reader, 131068), Size = reader.ReadInt64(), Hash = ReadText(reader, 64), Storage = i.ToString("x8", CultureInfo.InvariantCulture) };
                        ValidateName(member.Name);
                        if (member.Size < 0 || !IsKey(member.Hash) || !names.Add(member.Name.Replace('\\', '/')) || (previous != null && StringComparer.Ordinal.Compare(previous, member.Name) >= 0)) return false;
                        previous = member.Name; result.Bytes = checked(result.Bytes + member.Size); result.Members.Add(member);
                    }
                    if (reader.BaseStream.Position != reader.BaseStream.Length || Digest(result.Members) != result.Digest) return false;
                    foreach (Member member in result.Members)
                    {
                        string canonical = member.Name.Replace('\\', '/');
                        for (int slash = canonical.IndexOf('/'); slash >= 0; slash = canonical.IndexOf('/', slash + 1))
                            if (names.Contains(canonical.Substring(0, slash))) return false;
                    }
                }
                if (verifyPayload)
                {
                    missReason = ActionCacheMissReason.InvalidPayload;
                    string data = Path.Combine(root, FilesName); RejectAncestors(data);
                    if (!Directory.Exists(data)) return false;
                    var actual = Directory.GetFileSystemEntries(data); if (actual.Length != result.Members.Count) return false;
                    foreach (Member member in result.Members)
                    {
                        string path = Path.Combine(data, member.Storage); RejectAncestors(path);
                        if (!File.Exists(path) || new FileInfo(path).Length != member.Size || Hash(path, cancellation) != member.Hash) return false;
                    }
                }
                manifest = result; missReason = ActionCacheMissReason.None; return true;
            }
            catch (InvalidDataException) { return false; }
            catch (EndOfStreamException) { return false; }
            catch (FileNotFoundException) { return false; }
            catch (DirectoryNotFoundException) { return false; }
            catch (IOException) { missReason = ActionCacheMissReason.EntryUnavailable; return false; }
            catch (UnauthorizedAccessException) { missReason = ActionCacheMissReason.EntryUnavailable; return false; }
            catch (ArgumentException) { return false; }
            catch (OverflowException) { return false; }
        }
        static void DeletePrivateTree(string root)
        {
            try { RejectAncestors(root); if (Directory.Exists(root)) Directory.Delete(root, true); }
            catch (IOException) { } catch (UnauthorizedAccessException) { }
        }
    }
}
