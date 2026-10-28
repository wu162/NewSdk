using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace NewModCompiler.Application
{
    // A component of the new build-asset namespace, deliberately not a cache
    // lookup or an enablement gate by itself. Production additionally requires
    // the opt-in Core input boundary, audited cacheable providers and process
    // resolution identity. Inventory whole SDK/MOD/runtime roots instead of
    // trusting a caller-maintained list of XML includes.
    public sealed class CacheInputSnapshot
    {
        public const string Format = "NewModCompiler.InputInventory.v1";
        public const string CoverageLimitations = "Only SDK, MOD and compiler-runtime roots are inventoried. Root-external references, environment-dependent resolution and toolchain identity still require a verified resolver contract before this fingerprint can authorize cache reuse. This component neither stores nor loads cached results.";
        readonly string fingerprint;
        readonly int fileCount;
        private CacheInputSnapshot(string fingerprint, int fileCount, string toolchainFingerprint, long hashedBytes, long reusedHashBytes)
        { this.fingerprint = fingerprint; this.fileCount = fileCount; ToolchainFingerprint = toolchainFingerprint; HashedBytes = hashedBytes; ReusedHashBytes = reusedHashBytes; }
        public string Fingerprint { get { return fingerprint; } }
        public int FileCount { get { return fileCount; } }
        public string ToolchainFingerprint { get; private set; }
        public long HashedBytes { get; private set; }
        public long ReusedHashBytes { get; private set; }
        public bool CanAuthorizeCacheReuse { get { return false; } }

        public static CacheInputSnapshot Capture(BuildRequest request, BuildPlan plan, string runtimeDirectory, CancellationToken cancellation)
        { return Capture(request, plan, runtimeDirectory, cancellation, null); }

        public static CacheInputSnapshot Capture(BuildRequest request, BuildPlan plan, string runtimeDirectory, CancellationToken cancellation, CacheInputLeaseSet leases)
        {
            if (request == null || plan == null || plan.Tasks == null) throw new ArgumentException("A resolved request and plan are required.");
            string[] roots = { Root(request.SdkRoot), Root(request.ModRoot), Root(runtimeDirectory) };
            if (string.IsNullOrWhiteSpace(request.OutputRoot)) throw new ArgumentException("An output root is required.");
            string output = Path.GetFullPath(request.OutputRoot);
            foreach (string root in roots)
                if (Paths.Contains(root, output) || Paths.Contains(output, root)) throw new IOException("Input inventory roots must be separate from output.");
            cancellation.ThrowIfCancellationRequested();
            int count = 0;
            long hashedBytes = 0, reusedHashBytes = 0;
            // One owner-local file hasher per capture avoids CryptoConfig/CSP
            // creation for every tiny leaf. ComputeHash initializes between
            // files; recursive inventory never nests or shares a hash operation.
            using (var fileHash = SHA256.Create())
            using (var bytes = new MemoryStream())
            using (var writer = new BinaryWriter(bytes, new UTF8Encoding(false, true)))
            using (var tools = new MemoryStream())
            using (var toolWriter = new BinaryWriter(tools, new UTF8Encoding(false, true)))
            {
                toolWriter.Write("NewModCompiler.ToolInputs.v1");
                foreach (string root in roots) toolWriter.Write(root);
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
                for (int role = 0; role < roots.Length; ++role)
                {
                    writer.Write(role); writer.Write(roots[role]);
                    int currentRole = role;
                    // SDK inputs may share byte digests only while their exact
                    // file objects remain read-leased. The user's current MOD
                    // can be beneath SDK/Mods: exclude it in the SDK role too.
                    // Packaging-only Additional remains writable and rehashed.
                    // This changes neither inventory bytes nor tool identity.
                    Inventory(writer, roots[role], roots[role], cancellation, ref count, leases, fileHash, path => currentRole == 2 || (currentRole == 0 &&
                        !Paths.Contains(roots[1], path) && !Paths.Contains(Path.Combine(roots[0], "Additional"), path)), (path, length, digest, reused) => {
                        if (reused) reusedHashBytes = checked(reusedHashBytes + length);
                        else hashedBytes = checked(hashedBytes + length);
                        if (currentRole == 2 || (currentRole == 0 &&
                            (Paths.Contains(Path.Combine(roots[0], "tools"), path) || Paths.Contains(Path.Combine(roots[0], "Schemas"), path))))
                        { toolWriter.Write(currentRole); toolWriter.Write(path); toolWriter.Write(length); toolWriter.Write(digest); }
                    });
                }
                cancellation.ThrowIfCancellationRequested(); writer.Flush(); bytes.Position = 0;
                toolWriter.Flush(); tools.Position = 0;
                using (var hash = SHA256.Create())
                    return new CacheInputSnapshot(Hex(hash.ComputeHash(bytes)), count, Hex(hash.ComputeHash(tools)), hashedBytes, reusedHashBytes);
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
        static void Inventory(BinaryWriter writer, string root, string directory, CancellationToken cancellation, ref int count, CacheInputLeaseSet leases, SHA256 fileHash, Func<string,bool> eligible, Action<string,long,byte[],bool> record)
        {
            cancellation.ThrowIfCancellationRequested();
            if ((File.GetAttributes(directory) & FileAttributes.ReparsePoint) != 0) throw new IOException("Input inventory does not follow reparse points.");
            string[] entries = Directory.GetFileSystemEntries(directory); Array.Sort(entries, StringComparer.Ordinal);
            writer.Write(entries.Length);
            foreach (string path in entries)
            {
                cancellation.ThrowIfCancellationRequested();
                FileAttributes attributes = File.GetAttributes(path);
                if ((attributes & FileAttributes.ReparsePoint) != 0) throw new IOException("Input inventory does not follow reparse points.");
                bool isDirectory = (attributes & FileAttributes.Directory) != 0;
                writer.Write(isDirectory); writer.Write(path.Substring(root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Length));
                if (isDirectory) Inventory(writer, root, path, cancellation, ref count, leases, fileHash, eligible, record);
                else
                {
                    long length; byte[] digest; bool reused = false;
                    if (leases != null && eligible(path)) digest = leases.Read(path, cancellation, fileHash, out length, out reused);
                    else using (var input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    { digest = fileHash.ComputeHash(input); length = input.Length; }
                    writer.Write(length); writer.Write(digest); record(path, length, digest, reused);
                    checked { ++count; }
                }
            }
        }
        static string Hex(byte[] hash) { return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant(); }
    }
}
