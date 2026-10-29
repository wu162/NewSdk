using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Script.Serialization;

namespace NewModCompiler.Application
{
    // The four files are immutable. Core reads the old manifest/checksums in
    // place, and only changed output creates a new generation.
    internal sealed class LinkedOutputCache : IDisposable
    {
        static readonly string[] Extensions = { ".manifest", ".bin", ".imp", ".relo" };
        readonly string destination, output;
        readonly PublicationLock targetLock;
        readonly Dictionary<string, Artifact> staged = new Dictionary<string, Artifact>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<string, string> previous;
        readonly HashSet<string> requested = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        ArtifactPublication publication;
        bool repairRequired;
        public int RestoredStreams { get; private set; }

        internal LinkedOutputCache(string output)
        {
            this.output = output;
            destination = Path.Combine(output, ".linked-streams");
            targetLock = PublicationLock.Acquire(destination);
            try
            {
                string index = Path.Combine(destination, ".index.json");
                previous = File.Exists(index) ? new JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Deserialize<Dictionary<string,string>>(File.ReadAllText(index)) : null;
            }
            catch { targetLock.Dispose(); throw; }
        }
        internal string Find(string scope, string streams, string stem)
        {
            string cached = Path.Combine(destination, scope, stem.Substring(streams.Length + 1));
            foreach (string extension in Extensions) if (!File.Exists(cached + extension))
            {
                if (previous != null && previous.ContainsKey(Path.Combine(scope, stem.Substring(streams.Length + 1)) + extension)) repairRequired = true;
                return null;
            }
            return cached;
        }
        internal void Reuse(string stem, string cached, IList<Artifact> artifacts, FileHashCache hashes)
        {
            if (!requested.Add(stem)) return;
            foreach (string extension in Extensions)
            {
                string source = cached + extension;
                artifacts.Add(new Artifact { Path = stem + extension, BackingPath = source, Kind = "stream" + extension,
                    Size = new FileInfo(source).Length, Sha256 = hashes.Hash(source, true), Verified = true });
            }
            RestoredStreams++;
        }
        // Retained for standalone import callers. Production uses Find/Reuse.
        internal void Restore(string scope, string streams, string stem)
        {
            if (!requested.Add(stem)) return;
            string cached = Find(scope, streams, stem);
            if (cached == null) return;
            foreach (string extension in Extensions) FileTransfer.Snapshot(cached + extension, stem + extension);
            RestoredStreams++;
        }
        internal void Stage(string scope, string streams, IList<Artifact> artifacts, CancellationToken cancellation)
        {
            foreach (Artifact artifact in artifacts)
            {
                if (artifact.Kind == "custom-data") continue;
                cancellation.ThrowIfCancellationRequested();
                staged.Add(Path.Combine(scope, artifact.Path.Substring(streams.Length + 1)), artifact);
            }
        }
        internal void Commit()
        {
            bool unchanged = !repairRequired && previous != null && previous.Count == staged.Count;
            if (unchanged) foreach (var file in staged)
            {
                string hash;
                if (!previous.TryGetValue(file.Key, out hash) || file.Value.Sha256 == null || hash != file.Value.Sha256) { unchanged = false; break; }
            }
            if (unchanged || staged.Count == 0) return;
            string candidate = Path.Combine(output, ".linked-candidate-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(candidate);
            publication = new ArtifactPublication(destination, candidate,
                Path.Combine(output, ".linked-previous-" + Guid.NewGuid().ToString("N")), new Artifact[0], null, null);
            var index = new Dictionary<string,string>();
            foreach (var file in staged)
            {
                FileTransfer.Snapshot(file.Value.ContentPath, Path.Combine(candidate, file.Key));
                index.Add(file.Key, file.Value.Sha256);
            }
            File.WriteAllText(Path.Combine(candidate, ".index.json"), new JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Serialize(index));
            publication.Commit();
        }
        internal void Complete() { try { if (publication != null) publication.Complete(); } finally { targetLock.Dispose(); } }
        public void Dispose() { try { if (publication != null) publication.Dispose(); } finally { targetLock.Dispose(); } }
    }
}
