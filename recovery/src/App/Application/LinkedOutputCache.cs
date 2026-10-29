using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace NewModCompiler.Application
{
    // Core decides freshness from its original manifest/type/stream checksums.
    // Restore lazily, one requested four-file group at a time: removed includes
    // must never leak stale streams into this build's artifact list.
    internal sealed class LinkedOutputCache : IDisposable
    {
        static readonly string[] Extensions = { ".manifest", ".bin", ".imp", ".relo" };
        readonly string destination, candidate;
        readonly ArtifactPublication publication;
        readonly HashSet<string> requested = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public int RestoredStreams { get; private set; }

        internal LinkedOutputCache(string output)
        {
            destination = Path.Combine(output, ".linked-streams");
            candidate = Path.Combine(output, ".linked-candidate-" + Guid.NewGuid().ToString("N"));
            var targetLock = PublicationLock.Acquire(destination);
            try
            {
                Directory.CreateDirectory(candidate);
                publication = new ArtifactPublication(destination, candidate,
                    Path.Combine(output, ".linked-previous-" + Guid.NewGuid().ToString("N")), new Artifact[0], null, targetLock);
            }
            catch { targetLock.Dispose(); throw; }
        }

        internal void Restore(string scope, string streams, string stem)
        {
            if (!requested.Add(stem)) return;
            string relative = stem.Substring(streams.Length + 1);
            string cached = Path.Combine(destination, scope, relative);
            foreach (string extension in Extensions) if (!File.Exists(cached + extension)) return;
            foreach (string extension in Extensions) FileTransfer.Snapshot(cached + extension, stem + extension);
            RestoredStreams++;
        }

        internal void Stage(string scope, string streams, IList<Artifact> artifacts, CancellationToken cancellation)
        {
            foreach (Artifact artifact in artifacts)
            {
                if (artifact.Kind == "custom-data") continue;
                cancellation.ThrowIfCancellationRequested();
                FileTransfer.Snapshot(artifact.Path, Path.Combine(candidate, scope, artifact.Path.Substring(streams.Length + 1)));
            }
        }
        internal void Commit() { publication.Commit(); }
        internal void Complete() { publication.Complete(); }
        public void Dispose() { publication.Dispose(); }
    }
}
