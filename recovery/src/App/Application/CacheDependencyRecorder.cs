using System.IO;
using System.Threading;

namespace NewModCompiler.Application
{
    // Optional evidence must not turn its metadata capacity limit into a build
    // failure. Core keeps the full-root input inventory and boundary throughout
    // this scope, so abandoning observation preserves the conservative path.
    // A partial manifest can never authorize reuse or be published as complete.
    public sealed class CacheDependencyRecorder
    {
        readonly CancellationToken cancellation;
        public CacheDependencyManifest Manifest { get; private set; }
        public bool IsRecording { get; private set; }

        public CacheDependencyRecorder(CancellationToken cancellation = default(CancellationToken))
        {
            this.cancellation = cancellation;
            Manifest = new CacheDependencyManifest(cancellation);
            IsRecording = true;
        }

        public void Probe(string path, bool exists)
        {
            cancellation.ThrowIfCancellationRequested();
            if (!IsRecording) return;
            try { Manifest.Probe(path, exists); }
            catch (CacheDependencyBudgetException) { IsRecording = false; }
        }

        public Stream OpenRead(string path)
        {
            cancellation.ThrowIfCancellationRequested();
            if (IsRecording)
            {
                try { return Manifest.OpenRead(path); }
                catch (CacheDependencyBudgetException) { IsRecording = false; }
            }
            // Manifest.OpenRead closes its attempted read on every exception.
            // Continue with a fresh normal read only for capacity exhaustion;
            // real I/O errors, contradictory observations and cancellation
            // remain errors and are never converted into this fallback.
            cancellation.ThrowIfCancellationRequested();
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public void ReportUnknownRead(string reason)
        {
            cancellation.ThrowIfCancellationRequested();
            if (!IsRecording) return;
            try { Manifest.ReportUnknownRead(reason); }
            catch (CacheDependencyBudgetException) { IsRecording = false; }
        }

        public void RecordLeasedRead(string path, long length, byte[] digest)
        {
            cancellation.ThrowIfCancellationRequested();
            if (!IsRecording) return;
            try { Manifest.RecordLeasedRead(path, length, digest); }
            catch (CacheDependencyBudgetException) { IsRecording = false; }
        }
    }
}
