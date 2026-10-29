using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace NewModCompiler.Application
{
    // Inventory SDK/MOD/runtime roots once for a full mutation guard and a
    // separate incremental namespace. Neither is a cache lookup or an enablement
    // gate by itself: Core must validate document and asset dependencies inside
    // the input boundary, with audited providers and process resolution identity.
    public sealed class CacheInputSnapshot
    {
        public const string Format = "NewModCompiler.InputInventory.v1";
        public const string CoverageLimitations = "Only SDK, MOD and compiler-runtime roots are inventoried. Root-external references, environment-dependent resolution and toolchain identity still require a verified resolver contract before this fingerprint can authorize cache reuse. This component neither stores nor loads cached results.";
        readonly string fingerprint;
        readonly string incrementalFingerprint;
        readonly int fileCount;
        private CacheInputSnapshot(string fingerprint, string incrementalFingerprint, int fileCount) { this.fingerprint = fingerprint; this.incrementalFingerprint = incrementalFingerprint; this.fileCount = fileCount; }
        public string Fingerprint { get { return fingerprint; } }
        // Core owns MOD document/file dependency invalidation. Keep compiler,
        // SDK and resolver topology identity stable across MOD content edits,
        // so the previous session can actually reach those dependency checks.
        // Like Fingerprint, this value alone never authorizes an asset hit.
        public string IncrementalFingerprint { get { return incrementalFingerprint; } }
        public int FileCount { get { return fileCount; } }
        public bool CanAuthorizeCacheReuse { get { return false; } }

        public static CacheInputSnapshot Capture(BuildRequest request, BuildPlan plan, string runtimeDirectory, CancellationToken cancellation)
        {
            if (request == null || plan == null || plan.Tasks == null) throw new ArgumentException("A resolved request and plan are required.");
            string[] roots = { Root(request.SdkRoot), Root(request.ModRoot), Root(runtimeDirectory) };
            if (string.IsNullOrWhiteSpace(request.OutputRoot)) throw new ArgumentException("An output root is required.");
            string output = Path.GetFullPath(request.OutputRoot);
            foreach (string root in roots)
                if (Paths.Contains(root, output) || Paths.Contains(output, root)) throw new IOException("Input inventory roots must be separate from output.");
            cancellation.ThrowIfCancellationRequested();
            int count = 0;
            using (var bytes = new MemoryStream())
            using (var writer = new BinaryWriter(bytes, new UTF8Encoding(false, true)))
            using (var incrementalBytes = new MemoryStream())
            using (var incremental = new BinaryWriter(incrementalBytes, new UTF8Encoding(false, true)))
            {
                // BinaryWriter's length-prefixed strings prevent delimiter and
                // concatenation collisions. Enumeration uses ordinal sorting.
                writer.Write(Format);
                Value(writer, request.Command); Value(writer, request.PackageFormat); Value(writer, plan.PlanKind);
                var options = new List<string>(request.Options.Keys); options.Sort(StringComparer.Ordinal);
                writer.Write(options.Count);
                foreach (string name in options)
                {
                    EffectiveOption option = request.Options[name];
                    if (option == null) throw new ArgumentException("An effective option is missing.");
                    Value(writer, name); Value(writer, option.Value); Value(writer, option.Source);
                }
                string[] content = request.Content;
                writer.Write(content == null ? -1 : content.Length);
                if (content != null) foreach (string value in content) Value(writer, value);
                writer.Write(plan.Tasks.Length);
                foreach (BuildTask task in plan.Tasks)
                {
                    if (task == null || string.IsNullOrEmpty(task.InputPath)) throw new ArgumentException("A resolved task is incomplete.");
                    string input = Path.GetFullPath(task.InputPath);
                    if (!Paths.Contains(roots[0], input) && !Paths.Contains(roots[1], input))
                        throw new IOException("A task input is outside the inventoried SDK/MOD roots.");
                    if (!File.Exists(input)) throw new FileNotFoundException("A resolved task input is missing.", input);
                    Value(writer, task.Id); Value(writer, task.Content); Value(writer, input);
                    Value(writer, task.OutputName); Value(writer, task.Reason);
                    writer.Write(task.Dependencies == null ? -1 : task.Dependencies.Length);
                    if (task.Dependencies != null) foreach (string dependency in task.Dependencies) Value(writer, dependency);
                }
                // Full root identities intentionally isolate two different MODs
                // even when their current bytes happen to be identical.
                writer.Flush();
                incremental.Write("NewModCompiler.LegacyIncremental.v1");
                incremental.Write(bytes.ToArray());
                for (int role = 0; role < roots.Length; ++role)
                {
                    writer.Write(role); writer.Write(roots[role]);
                    incremental.Write(role); incremental.Write(roots[role]);
                    Inventory(writer, incremental, roots[role], roots[role], roots[1], role == 2, cancellation, ref count);
                }
                cancellation.ThrowIfCancellationRequested(); writer.Flush(); incremental.Flush(); bytes.Position = incrementalBytes.Position = 0;
                using (var hash = SHA256.Create()) return new CacheInputSnapshot(Hex(hash.ComputeHash(bytes)), Hex(hash.ComputeHash(incrementalBytes)), count);
            }
        }

        // Useful to reject input changes across a prospective compilation. It
        // is not a file-lock transaction and does not make cache reuse safe by
        // itself; production integration must also enforce the resolver scope.
        public bool MatchesCurrent(BuildRequest request, BuildPlan plan, string runtimeDirectory, CancellationToken cancellation)
        {
            return string.Equals(fingerprint, Capture(request, plan, runtimeDirectory, cancellation).Fingerprint, StringComparison.Ordinal);
        }

        static string Root(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory)) throw new ArgumentException("All three inventory roots are required.");
            string root = Path.GetFullPath(directory);
            if (!Directory.Exists(root)) throw new DirectoryNotFoundException(root);
            // Check ancestors as well: a root may be below a linked directory.
            for (var parent = new DirectoryInfo(root); parent != null; parent = parent.Parent)
                if ((parent.Attributes & FileAttributes.ReparsePoint) != 0) throw new IOException("Input inventory does not follow reparse points.");
            return root;
        }
        static void Value(BinaryWriter writer, string value)
        {
            writer.Write(value != null); if (value != null) writer.Write(value);
        }
        static void Inventory(BinaryWriter writer, BinaryWriter incremental, string root, string directory, string modRoot, bool runtime, CancellationToken cancellation, ref int count)
        {
            cancellation.ThrowIfCancellationRequested();
            if ((File.GetAttributes(directory) & FileAttributes.ReparsePoint) != 0) throw new IOException("Input inventory does not follow reparse points.");
            string[] entries = Directory.GetFileSystemEntries(directory); Array.Sort(entries, StringComparer.Ordinal);
            writer.Write(entries.Length);
            incremental.Write(entries.Length);
            foreach (string path in entries)
            {
                cancellation.ThrowIfCancellationRequested();
                FileAttributes attributes = File.GetAttributes(path);
                if ((attributes & FileAttributes.ReparsePoint) != 0) throw new IOException("Input inventory does not follow reparse points.");
                bool isDirectory = (attributes & FileAttributes.Directory) != 0;
                string relative = path.Substring(root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Length);
                writer.Write(isDirectory); writer.Write(relative);
                incremental.Write(isDirectory); incremental.Write(relative);
                if (isDirectory) Inventory(writer, incremental, root, path, modRoot, runtime, cancellation, ref count);
                else
                {
                    using (var input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (var hash = SHA256.Create())
                    {
                        byte[] contentHash = hash.ComputeHash(input);
                        writer.Write(input.Length); writer.Write(contentHash);
                        // SDK may contain the active MOD. Do not accidentally
                        // reintroduce whole-MOD invalidation via that root.
                        bool pinned = runtime || !Paths.Contains(modRoot, path);
                        incremental.Write(pinned);
                        if (pinned) { incremental.Write(input.Length); incremental.Write(contentHash); }
                    }
                    checked { ++count; }
                }
            }
        }
        static string Hex(byte[] hash) { return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant(); }
    }
}
