using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using NewModCompiler.Diagnostics;

namespace NewModCompiler.Application
{
    public sealed class PublicationResult
    {
        public string Status { get; internal set; }
        public string Directory { get; internal set; }
        public string RetainedPreviousDirectory { get; internal set; }
        public Artifact[] Artifacts { get; internal set; }
    }

    // BuildSession publishes to OutputRoot/published. Replaces one caller-selected leaf
    // directory; controlled failures roll back, process/power-loss recovery is
    // deliberately not claimed by this two-rename transaction.
    public sealed class ArtifactPublisher
    {
        public ArtifactPublication Prepare(BuildRequest request, string stagingDirectory, string destinationName,
            IList<Artifact> artifacts, CancellationToken cancellation)
        { return Prepare(request, stagingDirectory, destinationName, artifacts, cancellation, false); }

        internal ArtifactPublication PrepareOwned(BuildRequest request, string stagingDirectory, string destinationName,
            IList<Artifact> artifacts, CancellationToken cancellation)
        { return Prepare(request, stagingDirectory, destinationName, artifacts, cancellation, true); }

        ArtifactPublication Prepare(BuildRequest request, string stagingDirectory, string destinationName,
            IList<Artifact> artifacts, CancellationToken cancellation, bool owned)
        {
            if (request == null || artifacts == null || artifacts.Count == 0) throw new ArgumentException("Publication requires a request and a nonempty exact artifact list.");
            if (string.IsNullOrWhiteSpace(destinationName) || destinationName == "." || destinationName == ".." ||
                destinationName.IndexOfAny(new[] { '/', '\\', ':' }) >= 0 || destinationName.StartsWith(".", StringComparison.Ordinal))
                throw new ArgumentException("Publication destination must be an explicit ordinary leaf directory name.");
            DiagnosticDirectory.ValidateSyntax(destinationName);
            string output = Path.GetFullPath(request.OutputRoot), staging = Path.GetFullPath(stagingDirectory);
            string builds = Path.Combine(output, ".build"), destination = Path.Combine(output, destinationName);
            if (!Paths.Contains(builds, staging) || string.Equals(builds, staging, StringComparison.OrdinalIgnoreCase))
                throw new IOException("Publication source must belong to one build below OutputRoot/.build.");
            foreach (string input in new[] { request.ModRoot, request.SdkRoot })
            {
                string physicalInput = DiagnosticDirectory.ResolveInputDirectory(input);
                string physicalOutput = DiagnosticDirectory.ResolveOutput(output);
                if (Paths.Contains(physicalInput, physicalOutput) || Paths.Contains(physicalOutput, physicalInput))
                    throw new IOException("Publication output overlaps an input directory.");
            }
            cancellation.ThrowIfCancellationRequested();
            string prepared = Path.Combine(output, ".publish-" + Guid.NewGuid().ToString("N"));
            string backup = Path.Combine(output, ".previous-" + Guid.NewGuid().ToString("N"));
            bool preparedOwned = false;
            DiagnosticDirectory outputGuard = DiagnosticDirectory.Acquire(output, true);
            PublicationLock targetLock = null;
            try
            {
            targetLock = PublicationLock.Acquire(DiagnosticDirectory.ResolveOutput(destination));
            using (DiagnosticDirectory.Acquire(staging, false))
            {
                if (File.Exists(destination)) throw new IOException("Publication destination is a file.");
                if (Directory.Exists(destination)) RejectReparseTree(destination);
                try
                {
                    var result = new List<Artifact>();
                    var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (Artifact artifact in artifacts)
                    {
                        cancellation.ThrowIfCancellationRequested();
                        if (artifact == null || string.IsNullOrEmpty(artifact.Path) || !Path.IsPathRooted(artifact.Path) || string.IsNullOrEmpty(artifact.Kind))
                            throw new IOException("Publication artifact is incomplete.");
                        string source = Path.GetFullPath(artifact.Path);
                        string sourceRoot=owned && artifact.Published ? destination : staging;
                        if (!Paths.Contains(sourceRoot, source) || string.Equals(sourceRoot, source, StringComparison.OrdinalIgnoreCase))
                            throw new IOException("Publication artifact escaped its build staging directory.");
                        DiagnosticDirectory.ValidateSyntax(source);
                        using (owned ? null : DiagnosticDirectory.Acquire(Path.GetDirectoryName(source), false))
                        {
                            if ((File.GetAttributes(source) & (FileAttributes.ReparsePoint | FileAttributes.Directory)) != 0)
                                throw new IOException("Publication artifact must be an ordinary file.");
                            string relative = source.Substring(sourceRoot.TrimEnd('\\', '/').Length).TrimStart('\\', '/');
                            if (!paths.Add(relative)) throw new IOException("Duplicate publication artifact path.");
                            if (new FileInfo(source).Length != artifact.Size) throw new IOException("Publication source size changed.");
                            result.Add(new Artifact { Path = Path.Combine(destination, relative), Kind = artifact.Kind, Size = artifact.Size, Sha256 = artifact.Sha256 });
                        }
                    }
                    bool unchanged = Matches(destination, result, cancellation,owned ? request.FileIdentities : null);
                    if (unchanged && !owned)
                        foreach (Artifact artifact in artifacts)
                            if (!string.Equals(Hash(artifact.Path), artifact.Sha256, StringComparison.OrdinalIgnoreCase)) throw new IOException("Publication source hash changed.");
                    if (!unchanged)
                    {
                        DiagnosticDirectory.CreateNewDirectory(prepared); preparedOwned = true;
                        for (int index = 0; index < artifacts.Count; index++)
                        {
                            cancellation.ThrowIfCancellationRequested();
                            string copy = Path.Combine(prepared, result[index].Path.Substring(destination.Length + 1));
                            Directory.CreateDirectory(Path.GetDirectoryName(copy));
                            if (owned) File.Copy(artifacts[index].Path, copy);
                            else if (!string.Equals(CopyAndHash(artifacts[index].Path, copy), artifacts[index].Sha256, StringComparison.OrdinalIgnoreCase))
                                throw new IOException("Publication source hash changed.");
                        }
                    }
                    cancellation.ThrowIfCancellationRequested();
                    var transaction = new ArtifactPublication(destination, unchanged ? null : prepared, backup, result.ToArray(), outputGuard, targetLock);
                    preparedOwned = false; outputGuard = null; targetLock = null;
                    return transaction;
                }
                finally
                {
                    // Only our exclusively created preparation directory is
                    // disposable. Never sweep staging, old output or a rollback
                    // backup when commit/rollback did not complete.
                    if (preparedOwned) System.IO.Directory.Delete(prepared, true);
                }
            }
            } finally { if (targetLock != null) targetLock.Dispose(); if (outputGuard != null) outputGuard.Dispose(); }
        }
        static bool Matches(string destination, IList<Artifact> artifacts, CancellationToken cancellation,FileHashCache hashes)
        {
            if (!Directory.Exists(destination) || Directory.GetFiles(destination, "*", SearchOption.AllDirectories).Length != artifacts.Count) return false;
            foreach (Artifact artifact in artifacts)
            {
                cancellation.ThrowIfCancellationRequested();
                if (!File.Exists(artifact.Path) || new FileInfo(artifact.Path).Length != artifact.Size ||
                    !string.Equals(hashes==null ? Hash(artifact.Path) : hashes.Hash(artifact.Path), artifact.Sha256, StringComparison.OrdinalIgnoreCase)) return false;
            }
            return true;
        }
        static string CopyAndHash(string source, string destination)
        {
            using (var input = File.OpenRead(source))
            using (var output = new FileStream(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            using (var hash = SHA256.Create())
            {
                var buffer = new byte[65536]; int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) != 0)
                {
                    output.Write(buffer, 0, read); hash.TransformBlock(buffer, 0, read, buffer, 0);
                }
                hash.TransformFinalBlock(buffer, 0, 0);
                return BitConverter.ToString(hash.Hash).Replace("-", "").ToLowerInvariant();
            }
        }
        static string Hash(string path)
        {
            using (var input = File.OpenRead(path)) using (var hash = SHA256.Create())
                return BitConverter.ToString(hash.ComputeHash(input)).Replace("-", "").ToLowerInvariant();
        }
        internal static void RejectReparseTree(string directory)
        {
            if ((File.GetAttributes(directory) & FileAttributes.ReparsePoint) != 0) throw new IOException("Publication destination contains a reparse point.");
            foreach (string entry in System.IO.Directory.GetFileSystemEntries(directory))
            {
                FileAttributes attributes = File.GetAttributes(entry);
                if ((attributes & FileAttributes.ReparsePoint) != 0) throw new IOException("Publication destination contains a reparse point.");
                if ((attributes & FileAttributes.Directory) != 0) RejectReparseTree(entry);
            }
        }
    }
    // Commit retains the old generation until Complete is explicitly called
    // after required report/log channels have finalized. Dispose rolls back any
    // incomplete transaction; a rollback failure preserves its backup for recovery.
    public sealed class ArtifactPublication : IDisposable
    {
        readonly string destination, prepared, backup;
        readonly Artifact[] artifacts;
        DiagnosticDirectory guard;
        PublicationLock targetLock;
        bool committed, completed, disposed, oldMoved, rollbackFailed;
        internal ArtifactPublication(string destination, string prepared, string backup, Artifact[] artifacts, DiagnosticDirectory guard, PublicationLock targetLock)
        { this.destination = destination; this.prepared = prepared; this.backup = backup; this.artifacts = artifacts; this.guard = guard; this.targetLock = targetLock; }
        public string State { get { return completed ? "Completed" : disposed ? "RolledBack" : rollbackFailed ? "RollbackFailed" : committed ? "CommittedPendingFinalization" : "Prepared"; } }
        public PublicationResult Result { get; private set; }
        public bool Unchanged { get { return prepared == null; } }
        public void Commit()
        {
            Alive(); if (committed) throw new InvalidOperationException("Publication is already committed.");
            if (Unchanged) { committed = true; Result = new PublicationResult { Status = "CommittedPendingFinalization", Directory = destination, Artifacts = artifacts }; return; }
            if (File.Exists(destination)) throw new IOException("Publication destination is a file.");
            if (Directory.Exists(destination)) { ArtifactPublisher.RejectReparseTree(destination); Directory.Move(destination, backup); oldMoved = true; }
            try { Directory.Move(prepared, destination); committed = true; }
            catch (Exception error)
            {
                if (oldMoved)
                {
                    try { Directory.Move(backup, destination); oldMoved = false; }
                    catch (Exception rollback) { error.Data["PublicationBackupDirectory"] = backup; error.Data["PublicationRollbackException"] = rollback; }
                }
                throw;
            }
            Result = new PublicationResult { Status = "CommittedPendingFinalization", Directory = destination, Artifacts = artifacts };
        }
        public void Complete()
        {
            Alive(); if (!committed) throw new InvalidOperationException("Publication must be committed before finalization.");
            // Mark success before best-effort old-generation cleanup: a locked
            // stale backup is reported explicitly, never silently discarded.
            completed = true;
            Result.Status = "PublishedDirectory";
            if (oldMoved)
            {
                try { Directory.Delete(backup, true); oldMoved = false; }
                catch (IOException) { Result.RetainedPreviousDirectory = backup; }
                catch (UnauthorizedAccessException) { Result.RetainedPreviousDirectory = backup; }
            }
            Release();
        }
        public void Rollback()
        {
            Alive();
            try
            {
            if (committed && !Unchanged)
            {
                ArtifactPublisher.RejectReparseTree(destination);
                Directory.Delete(destination, true); committed = false;
            }
            if (oldMoved) { Directory.Move(backup, destination); oldMoved = false; }
            if (prepared != null && Directory.Exists(prepared)) Directory.Delete(prepared, true);
            Result = null; disposed = true; rollbackFailed = false; Release();
            }
            catch (Exception error)
            {
                rollbackFailed = true;
                if (oldMoved) error.Data["PublicationBackupDirectory"] = backup;
                throw;
            }
        }
        void Alive() { if (disposed || completed) throw new ObjectDisposedException("ArtifactPublication"); }
        void Release() { if (guard != null) { guard.Dispose(); guard = null; } if (targetLock != null) { targetLock.Dispose(); targetLock = null; } }
        public void Dispose()
        {
            if (disposed || completed) return;
            try { Rollback(); } finally { Release(); }
        }
    }

    // Named semaphore has no recursive thread ownership: a second transaction
    // on the same thread is rejected just like a separate process. No on-disk
    // lock file can be redirected through a junction/hard link. Local names span
    // processes in the current Windows session; cross-session publication is not
    // an enabled product scenario.
    internal sealed class PublicationLock : IDisposable
    {
        Semaphore semaphore;
        PublicationLock(Semaphore semaphore) { this.semaphore = semaphore; }
        internal static PublicationLock Acquire(string physicalDestination)
        {
            string key;
            using (var sha = SHA256.Create()) key = BitConverter.ToString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(physicalDestination.TrimEnd('\\', '/').ToUpperInvariant()))).Replace("-", "");
            var semaphore = new Semaphore(1, 1, "Local\\NewModCompiler-Publish-" + key);
            try
            {
                if (!semaphore.WaitOne(0)) throw new IOException("Another publication transaction owns this destination.");
                return new PublicationLock(semaphore);
            }
            catch { semaphore.Dispose(); throw; }
        }
        public void Dispose()
        {
            if (semaphore == null) return;
            try { semaphore.Release(); } finally { semaphore.Dispose(); semaphore = null; }
        }
    }

}
