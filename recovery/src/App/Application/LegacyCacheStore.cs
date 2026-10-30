using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace NewModCompiler.Application
{
    // BAB owns session serialization and its native asset cache. This adapter
    // only validates the implementation identity and imports immutable CDATA.
    public sealed class LegacyCacheStore : IDisposable
    {
        public const string SessionName = "BinaryAssetBuilder.SessionCache.xml";
        readonly string identity, cdataRoot;
        readonly CacheLock sessionLock;
        public string SessionRoot { get; private set; }
        public string SessionPath { get { return Path.Combine(SessionRoot, SessionName); } }
        internal string PrepareMissReason { get; private set; }
        public LegacyCacheStore(string sessionRoot, string identity)
        {
            SessionRoot = Path.GetFullPath(sessionRoot); this.identity = identity;
            cdataRoot = Path.Combine(SessionRoot, ".NewModCompiler", "cdata");
            sessionLock = CacheLock.Acquire(SessionPath);
        }
        public bool CanLoad()
        {
            string stamp = Path.Combine(SessionRoot, ".NewModCompiler", "session.identity");
            PrepareMissReason = null;
            if (File.Exists(stamp) && File.ReadAllText(stamp) != identity) { PrepareMissReason = "cache.session.identityChanged"; return false; }
            if (!File.Exists(SessionPath) && !File.Exists(SessionPath + ".deflate")) { PrepareMissReason = "cache.session.notFound"; return false; }
            return true;
        }
        internal string RestoreData(string relative, string target, CancellationToken cancellation)
        {
            foreach (string root in new[] { cdataRoot, SessionRoot })
            {
                string source = Path.Combine(root, relative);
                if (!File.Exists(source)) continue;
                cancellation.ThrowIfCancellationRequested();
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                // A restored file can be rewritten by Core; keep cache storage independent.
                File.Copy(source, target, true);
                return target;
            }
            return null;
        }
        internal void SaveData(string streams, IEnumerable<string> files, CancellationToken cancellation)
        {
            foreach (string source in files)
            {
                cancellation.ThrowIfCancellationRequested();
                string target = Path.Combine(cdataRoot, source.Substring(streams.Length + 1));
                // BAB's custom-data name contains the native asset identity.
                // Existing immutable entries need no content comparison or copy.
                if (!File.Exists(target)) FileTransfer.Snapshot(source, target);
            }
        }
        public void Saved(bool compressed)
        {
            string alternate = SessionPath + (compressed ? "" : ".deflate");
            if (File.Exists(alternate)) File.Delete(alternate);
            string stamp = Path.Combine(SessionRoot, ".NewModCompiler", "session.identity");
            Directory.CreateDirectory(Path.GetDirectoryName(stamp));
            if (!File.Exists(stamp) || File.ReadAllText(stamp) != identity) File.WriteAllText(stamp, identity);
        }
        public void Dispose() { sessionLock.Dispose(); }
    }
}
