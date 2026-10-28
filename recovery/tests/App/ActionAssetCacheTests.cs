using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using NewModCompiler.Application;

namespace NewModCompiler.Tests
{
    public static class ActionAssetCacheTests
    {
        static int checks;
        static readonly string First = new string('a', 64), Second = new string('b', 64);
        static void Check(bool value, string message) { ++checks; if (!value) throw new Exception(message); }
        static void Reject<T>(Action action, string message) where T : Exception { try { action(); } catch (T) { ++checks; return; } throw new Exception(message); }
        static string Cache(string root) { return Path.Combine(root, ".cache", "action-cache-v1"); }
        static string Staging(string root, string name) { return Path.Combine(root, ".build", name); }
        static string Entry(string root, string key) { return Path.Combine(Cache(root), key); }
        static void Put(string path, string content) { Directory.CreateDirectory(Path.GetDirectoryName(path)); File.WriteAllText(path, content); }
        static void Result(string root, string name, string value)
        {
            string path = Staging(root, name);
            Put(Path.Combine(path, "result.bin"), "all-buffers-and-metadata");
            Put(Path.Combine(path, "custom", "one.cdata"), value);
            Put(Path.Combine(path, "custom", "two.cdata"), "second-cdata");
        }
        static void Publish(string root, string key, string source)
        {
            using (var store = new ActionAssetCache(root)) { store.Stage(key, source, CancellationToken.None); store.Commit(); store.Complete(); }
        }
        static string[] RelativeFiles(string root) { return Directory.GetFiles(root, "*", SearchOption.AllDirectories).Select(path => path.Substring(root.Length + 1)).OrderBy(path => path, StringComparer.Ordinal).ToArray(); }

        static void MalformedMembers(string index, string actionKey, string[] names)
        {
            string format, owner;
            using (var reader = new BinaryReader(File.OpenRead(index), Encoding.UTF8)) { format = reader.ReadString(); owner = reader.ReadString(); }
            string hash = new string('c', 64), digest;
            // Keep the aggregate digest valid so path/collision cases cannot
            // pass merely because a generic checksum mismatch was detected.
            using (var bytes = new MemoryStream())
            using (var writer = new BinaryWriter(bytes, Encoding.UTF8))
            {
                writer.Write("NewModCompiler.ActionResultDigest.v1"); writer.Write(names.Length);
                foreach (string name in names) { writer.Write(name); writer.Write(0L); writer.Write(hash); }
                writer.Flush(); using (var algorithm = SHA256.Create()) digest = BitConverter.ToString(algorithm.ComputeHash(bytes.ToArray())).Replace("-", "").ToLowerInvariant();
            }
            using (var writer = new BinaryWriter(File.Open(index, FileMode.Create, FileAccess.Write, FileShare.None), Encoding.UTF8))
            {
                writer.Write(format); writer.Write(owner); writer.Write(actionKey); writer.Write(digest); writer.Write(names.Length);
                foreach (string name in names) { writer.Write(name); writer.Write(0L); writer.Write(hash); }
            }
        }

        public static int Run(string root)
        {
            checks = 0; Directory.CreateDirectory(root); Result(root, "first", "custom-A");
            string source = Staging(root, "first");
            using (var store = new ActionAssetCache(root))
            {
                Check(!store.TryRestore(First, Staging(root, "cold"), CancellationToken.None), "Cold action cache unexpectedly hit");
                store.Stage(First, source, CancellationToken.None);
                Check(!Directory.Exists(Entry(root, First)), "Staging exposed an action cache entry");
                // The building owner may reuse its own complete candidate,
                // including multiple CDATA files, without public visibility.
                string candidate = Staging(root, "candidate");
                Check(store.TryRestore(First, candidate, CancellationToken.None), "Owner could not restore its action candidate");
                Check(RelativeFiles(source).SequenceEqual(RelativeFiles(candidate)), "Action candidate member set changed");
                store.Stage(First, source, CancellationToken.None);
                store.Commit(); Check(File.Exists(Path.Combine(Entry(root, First), "pending")), "Action install omitted its exclusion marker");
                Reject<IOException>(() => { using (var competing = new ActionAssetCache(root)) { } }, "Action cache root allowed a competing transaction");
                store.Rollback(); Check(!Directory.Exists(Entry(root, First)), "Rollback left a new action entry visible");
                store.Rollback();
            }
            Publish(root, First, source);
            byte[] publishedIndex = File.ReadAllBytes(Path.Combine(Entry(root, First), "index.bin"));
            using (var store = new ActionAssetCache(root))
            {
                string hot = Staging(root, "hot"); Check(store.TryRestore(First, hot, CancellationToken.None), "Complete action entry did not restore");
                Check(RelativeFiles(source).SequenceEqual(RelativeFiles(hot)), "Complete action restore lost members");
                foreach (string name in RelativeFiles(source)) Check(File.ReadAllBytes(Path.Combine(source, name)).SequenceEqual(File.ReadAllBytes(Path.Combine(hot, name))), "Action restore changed result bytes: " + name);
                Put(Path.Combine(hot, "custom", "one.cdata"), "mutated published output");
                Check(store.TryRestore(First, Staging(root, "after-output-edit"), CancellationToken.None), "Writable output bytes were shared with durable action payload");
                Check(File.ReadAllText(Path.Combine(Staging(root, "after-output-edit"), "custom", "one.cdata")) == "custom-A", "Output mutation corrupted action store bytes");
                Check(!store.TryRestore(Second, Staging(root, "other-action"), CancellationToken.None), "Different action key reused same legacy result");
                store.Stage(First, source, CancellationToken.None); store.Commit(); store.Rollback();
            }
            Check(File.ReadAllBytes(Path.Combine(Entry(root, First), "index.bin")).SequenceEqual(publishedIndex), "Reusing an identical action result replaced a trusted generation");
            Result(root, "different", "custom-B");
            using (var store = new ActionAssetCache(root))
                Reject<InvalidDataException>(() => store.Stage(First, Staging(root, "different"), CancellationToken.None), "A strong action key accepted different complete results");
            Check(!File.Exists(Path.Combine(Entry(root, First), "pending")), "Nondeterministic result damaged a trusted entry");
            // A byte-valid result rejected by the semantic result reader may be
            // explicitly repaired, preserving the old object on publication
            // failure. This is distinct from executing a valid key differently.
            using (var store = new ActionAssetCache(root))
            {
                store.Invalidate(First);
                Check(!store.TryRestore(First, Staging(root, "semantic-miss"), CancellationToken.None), "Semantically invalidated action was reused");
                store.Stage(First, Staging(root, "different"), CancellationToken.None); store.Commit(); store.Rollback();
            }
            Check(File.ReadAllBytes(Path.Combine(Entry(root, First), "index.bin")).SequenceEqual(publishedIndex), "Semantic repair rollback changed the trusted predecessor");
            using (var store = new ActionAssetCache(root))
            {
                store.Stage(Second, source, CancellationToken.None); store.Invalidate(Second);
                store.Stage(Second, Staging(root, "different"), CancellationToken.None);
                Check(store.TryRestore(Second, Staging(root, "replaced-candidate"), CancellationToken.None), "Repaired private action candidate was not reusable");
                Check(File.ReadAllText(Path.Combine(Staging(root, "replaced-candidate"), "custom", "one.cdata")) == "custom-B", "Invalidated candidate retained stale semantic bytes");
            }

            // A missing member or a same-size/same-mtime edit is a miss. Repair
            // is reversible until all required publication channels complete.
            string[] payloads = Directory.GetFiles(Path.Combine(Entry(root, First), "files"));
            string damaged = payloads.Single(path => File.ReadAllText(path) == "custom-A");
            DateTime timestamp = File.GetLastWriteTimeUtc(damaged); Put(damaged, "custom-X"); File.SetLastWriteTimeUtc(damaged, timestamp);
            byte[] corruptBytes = File.ReadAllBytes(damaged);
            using (var store = new ActionAssetCache(root))
            {
                Check(!store.TryRestore(First, Staging(root, "corrupt"), CancellationToken.None), "Same-size/same-mtime corruption was reused");
                Check(!Directory.Exists(Staging(root, "corrupt")), "Corrupt action entry partly restored");
                store.Stage(First, source, CancellationToken.None); store.Commit();
                Check(File.Exists(Path.Combine(Entry(root, First), "pending")), "Action repair became visible before Complete");
                store.Rollback();
            }
            Check(File.ReadAllBytes(damaged).SequenceEqual(corruptBytes), "Failed repair did not restore the previous cache object");
            Publish(root, First, source);
            using (var store = new ActionAssetCache(root)) Check(store.TryRestore(First, Staging(root, "repaired"), CancellationToken.None), "Transactional action repair failed");
            string missing = Directory.GetFiles(Path.Combine(Entry(root, First), "files")).Single(path => File.ReadAllText(path) == "second-cdata");
            File.Delete(missing);
            using (var store = new ActionAssetCache(root)) Check(!store.TryRestore(First, Staging(root, "missing-cdata"), CancellationToken.None), "Missing CDATA was accepted as a complete result");
            Publish(root, First, source);
            Put(Path.Combine(Entry(root, First), "files", "unexpected"), "unindexed");
            using (var store = new ActionAssetCache(root)) Check(!store.TryRestore(First, Staging(root, "extra"), CancellationToken.None), "Unexpected action payload member was accepted");
            Publish(root, First, source);
            Put(Path.Combine(Entry(root, First), "pending"), "abandoned");
            using (var store = new ActionAssetCache(root)) Check(!store.TryRestore(First, Staging(root, "pending"), CancellationToken.None), "Abandoned pending action became a hit");
            Publish(root, First, source);

            using (var store = new ActionAssetCache(root))
            using (var cancellation = new CancellationTokenSource())
            {
                cancellation.Cancel();
                Reject<OperationCanceledException>(() => store.TryRestore(First, Staging(root, "cancelled"), cancellation.Token), "Cancelled action restore succeeded");
                Reject<OperationCanceledException>(() => store.Stage(Second, source, cancellation.Token), "Cancelled action stage succeeded");
                Reject<ArgumentException>(() => store.Stage("../escape", source, CancellationToken.None), "Action key traversal was accepted");
                Reject<ArgumentException>(() => store.TryRestore(First.ToUpperInvariant(), Staging(root, "uppercase"), CancellationToken.None), "Noncanonical action identity was accepted");
                Reject<IOException>(() => store.Stage(Second, root, CancellationToken.None), "Output root was accepted as action staging");
                Reject<IOException>(() => store.Stage(Second, Entry(root, First), CancellationToken.None), "Durable action bytes were accepted as staging");
                Reject<IOException>(() => store.TryRestore(First, Path.GetDirectoryName(root), CancellationToken.None), "Action restore escaped output");
                Directory.CreateDirectory(Staging(root, "empty"));
                Reject<InvalidDataException>(() => store.Stage(Second, Staging(root, "empty"), CancellationToken.None), "Empty result without metadata became a cache entry");
                Put(Path.Combine(Staging(root, "nonempty"), "existing"), "preserve");
                Reject<IOException>(() => store.TryRestore(First, Staging(root, "nonempty"), CancellationToken.None), "Action restore overwrote nonempty staging");
            }

            // Strictly validate manifest action identity, path boundaries and
            // member uniqueness even when bytes are individually well formed.
            string index = Path.Combine(Entry(root, First), "index.bin");
            byte[] valid = File.ReadAllBytes(index);
            File.WriteAllBytes(index, new byte[] { 255, 255, 255, 255, 127 });
            using (var store = new ActionAssetCache(root)) Check(!store.TryRestore(First, Staging(root, "bad-index"), CancellationToken.None), "Malformed action index was accepted");
            File.WriteAllBytes(index, valid);
            int attack = 0;
            foreach (string[] names in new[] {
                new[] { "..\\escaped" }, new[] { "result.bin:stream" }, new[] { "CON.txt" }, new[] { "trailing. " },
                new[] { "custom/one.cdata", "custom\\one.cdata" }, new[] { "custom", "custom/one.cdata" }, new[] { "DUPLICATE", "duplicate" }
            })
            {
                MalformedMembers(index, First, names);
                using (var store = new ActionAssetCache(root)) Check(!store.TryRestore(First, Staging(root, "path-attack-" + (++attack)), CancellationToken.None), "Unsafe action manifest member set was accepted");
                File.WriteAllBytes(index, valid);
            }
            string swapped = Path.Combine(Cache(root), Second); Directory.Move(Entry(root, First), swapped);
            using (var store = new ActionAssetCache(root)) Check(!store.TryRestore(Second, Staging(root, "swapped"), CancellationToken.None), "Manifest belonging to another action identity was reused");
            Directory.Move(swapped, Entry(root, First));

            // Windows deletion failure retains the exclusion marker and exact
            // ownership evidence; rollback can safely retry after handle close.
            using (var store = new ActionAssetCache(root))
            {
                store.Stage(Second, source, CancellationToken.None); store.Commit();
                string secondEntry = Entry(root, Second), heldPath = Directory.GetFiles(Path.Combine(secondEntry, "files")).First();
                using (var held = new FileStream(heldPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    bool failed = false;
                    try { store.Rollback(); }
                    catch (IOException error) { failed = true; Check(((string[])error.Data["RetainedCacheEntries"]).Contains(secondEntry), "Failed action rollback lost its retained entry path"); }
                    Check(failed, "Action rollback ignored a live payload handle");
                    Check(File.Exists(Path.Combine(secondEntry, "pending")), "Failed action rollback exposed an entry");
                }
                store.Rollback(); Check(!Directory.Exists(secondEntry), "Action rollback did not retry after a sharing failure");
            }
            Check(Directory.Exists(Entry(root, First)), "Rolling back another action removed trusted bytes");

            string bounded = Path.Combine(root, "bounded"); Result(bounded, "source", "bounded-A");
            using (var store = new ActionAssetCache(bounded, 1024, 1)) { store.Stage(First, Staging(bounded, "source"), CancellationToken.None); store.Stage(Second, Staging(bounded, "source"), CancellationToken.None); store.Commit(); store.Complete(); }
            Check(Directory.GetDirectories(Cache(bounded)).Length == 1, "Action entry capacity did not evict a complete entry");
            using (var store = new ActionAssetCache(bounded, 0, 0)) { store.Commit(); store.Complete(); }
            Check(Directory.GetDirectories(Cache(bounded)).Length == 0, "Action byte capacity did not remove an oversized complete entry");
            Publish(bounded, First, Staging(bounded, "source"));
            Put(Path.Combine(Entry(bounded, First), "pending"), "crashed transaction");
            using (var store = new ActionAssetCache(bounded)) { store.Commit(); store.Complete(); }
            Check(!Directory.Exists(Entry(bounded, First)), "Action cache cleanup retained an abandoned pending generation");
            Check(!Directory.GetDirectories(root, ".action-cache-candidate-*").Any(), "Action candidate directories leaked after Dispose");
            Check(!Directory.GetDirectories(root, "*.restore-*", SearchOption.AllDirectories).Any(), "Action restoration scratch leaked");
            return checks;
        }
    }
}
