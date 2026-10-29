using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace NewModCompiler.Application
{
    // The original BAB layout and native asset keys are the public cache
    // contract. Session serialization stays in Core, including .deflate and
    // partial/frozen sessions. Only final artifact publication is transactional.
    public sealed class LegacyCacheStore : IDisposable
    {
        public const string SessionName = "BinaryAssetBuilder.SessionCache.xml";
        readonly string output, sessionRoot, identity, candidate, cdataRoot;
        readonly Dictionary<string, string> backups = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        readonly List<string> installed = new List<string>();
        PublicationLock sessionLock;
        string stagedSession;
        bool committed, disposed;
        public string SessionRoot { get { return sessionRoot; } }
        public string SessionPath { get { return Path.Combine(sessionRoot, SessionName); } }
        public LegacyCacheStore(string outputRoot, string sessionRoot, string identity)
        {
            output = Path.GetFullPath(outputRoot); this.sessionRoot = Path.GetFullPath(sessionRoot); this.identity = identity;
            candidate = Path.Combine(output, ".legacy-cache-candidate-" + Guid.NewGuid().ToString("N"));
            cdataRoot = Path.Combine(this.sessionRoot, ".NewModCompiler", "cdata");
            RejectLinks(this.sessionRoot);
            sessionLock = PublicationLock.Acquire(SessionPath);
        }
        public bool Prepare(string hashes, string streams, CancellationToken cancellation, string relativeStream = null)
        {
            Active(); cancellation.ThrowIfCancellationRequested();
            string source = stagedSession;
            if (source == null)
            {
                string stamp = Path.Combine(sessionRoot, ".NewModCompiler", "session.identity");
                if (File.Exists(stamp) && File.ReadAllText(stamp) != identity) return false;
                source = File.Exists(SessionPath + ".deflate") ? SessionPath + ".deflate" : SessionPath;
                if (!File.Exists(source)) return false;
            }
            string target = Path.Combine(hashes, SessionName) + (source.EndsWith(".deflate", StringComparison.OrdinalIgnoreCase) ? ".deflate" : "");
            Copy(source, target, cancellation);
            string stagedData = Path.Combine(candidate, "cdata");
            if (Directory.Exists(stagedData)) CopyData(stagedData, streams, cancellation, false, relativeStream);
            else if (Directory.Exists(cdataRoot)) CopyData(cdataRoot, streams, cancellation, false, relativeStream);
            else if (Directory.Exists(sessionRoot)) CopyData(sessionRoot, streams, cancellation, true, relativeStream);
            return true;
        }
        public void Stage(string hashes, string streams, bool compressed, CancellationToken cancellation)
        {
            Active(); if (committed) throw new InvalidOperationException("A legacy session is already committed.");
            cancellation.ThrowIfCancellationRequested();
            string name = SessionName + (compressed ? ".deflate" : "");
            string source = Path.Combine(hashes, name), target = Path.Combine(candidate, name);
            Copy(source, target, cancellation); stagedSession = target;
            if (File.Exists(source + ".old")) Copy(source + ".old", target + ".old", cancellation);
            CopyData(streams, Path.Combine(candidate, "cdata"), cancellation);
            string alternate = Path.Combine(candidate, SessionName + (compressed ? "" : ".deflate"));
            if (File.Exists(alternate)) File.Delete(alternate);
        }
        public void Commit()
        {
            Active(); if (committed || stagedSession == null) return;
            try
            {
                Install(stagedSession, Path.Combine(sessionRoot, Path.GetFileName(stagedSession)));
                if (File.Exists(stagedSession + ".old")) Install(stagedSession + ".old", Path.Combine(sessionRoot, Path.GetFileName(stagedSession)) + ".old");
                string alternate = SessionPath + (stagedSession.EndsWith(".deflate", StringComparison.OrdinalIgnoreCase) ? "" : ".deflate");
                Backup(alternate);
                string stamp = Path.Combine(candidate, "session.identity");
                File.WriteAllText(stamp, identity);
                Install(stamp, Path.Combine(sessionRoot, ".NewModCompiler", "session.identity"));
                string data = Path.Combine(candidate, "cdata");
                if (Directory.Exists(data)) foreach (string file in Files(data))
                    Install(file, Path.Combine(cdataRoot, file.Substring(data.Length + 1)));
                committed = true;
            }
            catch { Rollback(); throw; }
        }
        // /psc:true deliberately keeps completed cache work even if the build
        // later aborts. This never publishes a partial MOD or changes its exit code.
        public void ConfirmPartial() { Commit(); Confirm(); }
        public void Complete()
        {
            if (disposed) return;
            try { Confirm(); }
            catch (IOException) { } catch (UnauthorizedAccessException) { }
            finally { if (sessionLock != null) sessionLock.Dispose(); sessionLock = null; disposed = true; }
        }
        void Confirm()
        {
            if (!committed) return;
            backups.Clear(); installed.Clear(); committed = false; stagedSession = null;
            DeleteCandidate();
        }
        public void Rollback()
        {
            if (disposed) return;
            foreach (string target in installed.AsEnumerable().Reverse()) if (File.Exists(target)) File.Delete(target);
            installed.Clear();
            foreach (var pair in backups) { Directory.CreateDirectory(Path.GetDirectoryName(pair.Key)); File.Copy(pair.Value, pair.Key, true); }
            backups.Clear(); committed = false; stagedSession = null; DeleteCandidate();
        }
        public void Dispose()
        {
            if (disposed) return;
            try { Rollback(); }
            finally { if (sessionLock != null) sessionLock.Dispose(); sessionLock = null; disposed = true; }
        }
        void Install(string source, string target)
        {
            RejectLinks(target); Backup(target); Directory.CreateDirectory(Path.GetDirectoryName(target));
            installed.Add(target); File.Copy(source, target, true);
        }
        void Backup(string target)
        {
            RejectLinks(target); if (!File.Exists(target) || backups.ContainsKey(target)) return;
            string backup = Path.Combine(candidate, "previous", backups.Count.ToString("x8"));
            Directory.CreateDirectory(Path.GetDirectoryName(backup)); File.Copy(target, backup, false);
            backups.Add(target, backup); File.Delete(target);
        }
        void CopyData(string root, string target, CancellationToken cancellation, bool legacyRoot = false, string relativeStream = null)
        {
            if (!Directory.Exists(root)) return;
            foreach (string path in DataFiles(root, legacyRoot))
            {
                string relative = path.Substring(root.Length + 1);
                if (relativeStream != null && !relative.StartsWith(relativeStream + Path.DirectorySeparatorChar + "cdata" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) continue;
                Copy(path, Path.Combine(target, relative), cancellation);
            }
        }
        IEnumerable<string> DataFiles(string root, bool legacyRoot)
        {
            RejectLinks(root);
            foreach (string path in Directory.GetFileSystemEntries(root))
            {
                if (Directory.Exists(path))
                {
                    string name = Path.GetFileName(path);
                    if (legacyRoot && (name == ".build" || name == "published" || name == "cache" || name == "assets" || name == ".NewModCompiler" || name.StartsWith(".legacy-cache-candidate-", StringComparison.Ordinal))) continue;
                    foreach (string child in DataFiles(path, legacyRoot)) yield return child;
                }
                else if (string.Equals(Path.GetExtension(path), ".cdata", StringComparison.OrdinalIgnoreCase)) yield return path;
            }
        }
        static IEnumerable<string> Files(string root)
        {
            RejectLinks(root);
            foreach (string path in Directory.GetFileSystemEntries(root))
                if (Directory.Exists(path)) { foreach (string child in Files(path)) yield return child; }
                else { RejectLinks(path); yield return path; }
        }
        static void Copy(string source, string target, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested(); RejectLinks(source); RejectLinks(target);
            Directory.CreateDirectory(Path.GetDirectoryName(target)); File.Copy(source, target, true);
        }
        static void RejectLinks(string path)
        {
            if (File.Exists(path) && (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0) throw new IOException("Legacy cache files cannot use reparse points.");
            for (var directory = new DirectoryInfo(path); directory != null; directory = directory.Parent)
                if (directory.Exists && (directory.Attributes & FileAttributes.ReparsePoint) != 0) throw new IOException("Legacy cache directories cannot use reparse points.");
        }
        void DeleteCandidate() { if (Directory.Exists(candidate)) Directory.Delete(candidate, true); }
        void Active() { if (disposed) throw new ObjectDisposedException("LegacyCacheStore"); }
    }
}
