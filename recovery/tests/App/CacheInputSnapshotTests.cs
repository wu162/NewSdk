using System;
using System.IO;
using System.Threading;
using NewModCompiler.Application;

namespace NewModCompiler.Tests
{
    public static class CacheInputSnapshotTests
    {
        static int assertions;
        static void Check(bool condition, string message) { ++assertions; if (!condition) throw new Exception(message); }
        static void Write(string file, string contents) { Directory.CreateDirectory(Path.GetDirectoryName(file)); File.WriteAllText(file, contents); }
        static void Reject<T>(Action action, string message) where T : Exception
        {
            try { action(); } catch (T) { ++assertions; return; }
            throw new Exception(message);
        }
        static void SharedToolLeases(BuildRequest request, BuildPlan plan, string runtime, string dependency, string schema, string config, string compiler, CacheInputSnapshot baseline)
        {
            long toolBytes = new FileInfo(schema).Length + new FileInfo(config).Length + new FileInfo(compiler).Length;
            var leases = new CacheInputLeaseSet();
            try
            {
                var first = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None, leases);
                Check(first.Fingerprint == baseline.Fingerprint && first.ToolchainFingerprint == baseline.ToolchainFingerprint,
                    "Leased input inventory must preserve the original content and tool identities.");
                Check(first.HashedBytes == baseline.HashedBytes && first.ReusedHashBytes == 0 && leases.RetainedFiles == 3,
                    "The first capture must hash every byte and retain only the three tool files.");
                var second = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None, leases);
                Check(second.Fingerprint == first.Fingerprint && second.ToolchainFingerprint == first.ToolchainFingerprint,
                    "Tool digest reuse changed the actual inventory identity.");
                Check(second.ReusedHashBytes == toolBytes && second.HashedBytes + second.ReusedHashBytes == baseline.HashedBytes,
                    "A repeated capture must reuse exactly the leased tool bytes and still hash all ordinary inputs.");
                Check(leases.ReusedFiles == 3 && leases.ReusedBytes == toolBytes,
                    "Lease counters must describe successful identity-verified reuse, not merely retained handles.");
                string taskId = plan.Tasks[0].Id;
                try
                {
                    plan.Tasks[0].Id = "apt/second-stream";
                    var otherTask = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None, leases);
                    Check(otherTask.Fingerprint != baseline.Fingerprint && otherTask.ToolchainFingerprint == baseline.ToolchainFingerprint && otherTask.ReusedHashBytes == toolBytes,
                        "Cross-stream tool reuse must retain the current task's semantic inventory identity.");
                }
                finally { plan.Tasks[0].Id = taskId; }
                foreach (string path in new[] { schema, config, compiler })
                {
                    Reject<IOException>(() => { using (File.Open(path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite)) { } },
                        "A retained tool's exact bytes must not be writable while its digest is reused: " + path);
                    Reject<IOException>(() => File.Delete(path), "A retained tool must not be deletable while its digest is reused: " + path);
                }
                using (var cancelled = new CancellationTokenSource())
                {
                    cancelled.Cancel();
                    Reject<OperationCanceledException>(() => CacheInputSnapshot.Capture(request, plan, runtime, cancelled.Token, leases),
                        "A cancelled shared capture must not succeed.");
                }
                Check(CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None, leases).Fingerprint == baseline.Fingerprint,
                    "Cancellation must not corrupt a later shared capture.");

                // MOD files deliberately do not acquire persistent tool leases.
                // Equal length and mtime must remain insufficient for reuse.
                DateTime timestamp = File.GetLastWriteTimeUtc(dependency);
                try
                {
                    Write(dependency, "BBBB"); File.SetLastWriteTimeUtc(dependency, timestamp);
                    var changed = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None, leases);
                    Check(changed.Fingerprint != baseline.Fingerprint && changed.ToolchainFingerprint == baseline.ToolchainFingerprint,
                        "Shared tool leases hid a same-length/same-mtime MOD mutation.");
                    Check(changed.ReusedHashBytes == toolBytes && changed.HashedBytes + changed.ReusedHashBytes == baseline.HashedBytes,
                        "MOD mutation must still read ordinary bytes rather than reuse metadata.");
                }
                finally { Write(dependency, "AAAA"); File.SetLastWriteTimeUtc(dependency, timestamp); }

                string added = Path.Combine(request.SdkRoot, "Schemas", "new-schema.xsd");
                try
                {
                    Write(added, "new schema");
                    var changed = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None, leases);
                    Check(changed.Fingerprint != baseline.Fingerprint && changed.ToolchainFingerprint != baseline.ToolchainFingerprint,
                        "Retained tool digests must not bypass root enumeration or hide a new schema file.");
                }
                finally { leases.Dispose(); File.Delete(added); }
            }
            finally { leases.Dispose(); }
            leases.Dispose();

            // A directory entry can be replaced without preserving the file
            // object behind a previously opened handle. Either Windows blocks
            // the rename, or the newly opened file identity must force a read.
            string schemaDirectory = Path.GetDirectoryName(schema), movedDirectory = schemaDirectory + "-moved";
            using (var replacing = new CacheInputLeaseSet())
            {
                CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None, replacing);
                bool moved = false;
                try
                {
                    try { Directory.Move(schemaDirectory, movedDirectory); moved = true; }
                    catch (IOException) { }
                    if (moved)
                    {
                        string original = File.ReadAllText(Path.Combine(movedDirectory, Path.GetFileName(schema)));
                        Write(schema, "Y" + original.Substring(1));
                        File.SetLastWriteTimeUtc(schema, File.GetLastWriteTimeUtc(Path.Combine(movedDirectory, Path.GetFileName(schema))));
                        var replacement = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None, replacing);
                        Check(replacement.Fingerprint != baseline.Fingerprint && replacement.ToolchainFingerprint != baseline.ToolchainFingerprint,
                            "A replaced ancestor directory reused a digest from a different file object.");
                        Check(replacement.HashedBytes >= baseline.HashedBytes - toolBytes + new FileInfo(schema).Length,
                            "A replacement file object must be hashed even when its old path, length and mtime agree.");
                    }
                    else Check(CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None, replacing).Fingerprint == baseline.Fingerprint,
                        "A blocked ancestor replacement must leave the original inventory intact.");
                }
                finally
                {
                    replacing.Dispose();
                    if (moved) { Directory.Delete(schemaDirectory, true); Directory.Move(movedDirectory, schemaDirectory); }
                }
            }

            string previous = File.ReadAllText(schema); DateTime schemaTime = File.GetLastWriteTimeUtc(schema);
            try
            {
                Write(schema, "X" + previous.Substring(1)); File.SetLastWriteTimeUtc(schema, schemaTime);
                using (var next = new CacheInputLeaseSet())
                {
                    var changed = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None, next);
                    Check(changed.Fingerprint != baseline.Fingerprint && changed.ToolchainFingerprint != baseline.ToolchainFingerprint && changed.ReusedHashBytes == 0,
                        "Disposing leases must release writes and a new session must hash changed tool bytes afresh.");
                }
            }
            finally { Write(schema, previous); File.SetLastWriteTimeUtc(schema, schemaTime); }
            using (var limited = new CacheInputLeaseSet(1))
            {
                var first = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None, limited);
                var second = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None, limited);
                Check(limited.RetainedFiles == 1 && first.HashedBytes == baseline.HashedBytes && first.ReusedHashBytes == 0,
                    "The lease cap must bound retained handles without skipping fresh content reads.");
                Check(second.Fingerprint == baseline.Fingerprint && second.ToolchainFingerprint == baseline.ToolchainFingerprint && second.ReusedHashBytes > 0 && second.ReusedHashBytes < toolBytes,
                    "Exceeding the lease cap must fall back to hashing the remaining files with identical identities.");
                Check(second.HashedBytes + second.ReusedHashBytes == baseline.HashedBytes,
                    "Bounded fallback must account for every input byte exactly once.");
            }
            foreach (string path in new[] { schema, config, compiler })
                using (File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
            Check(baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None),
                "Shared inventory tests must release every retained handle and restore the original bytes.");
        }
        static void StableSdkLeases(string root)
        {
            string sdk = Path.Combine(root, "sdk"), runtime = Path.Combine(root, "runtime");
            // Exclusion follows the actual MOD root, even below SDK/Art rather
            // than a conventional SDK/Mods directory.
            string mod = Path.Combine(sdk, "Art", "EditableMod");
            string entry = Path.Combine(mod, "Data", "Mod.xml"), editable = Path.Combine(mod, "Art", "editable.bin");
            string art = Path.Combine(sdk, "Art", "large.bin"), declaration = Path.Combine(sdk, "SageXml", "Stable.xml");
            string additional = Path.Combine(sdk, "Additional", "payload.bin"), compiler = Path.Combine(runtime, "NewModCompiler.exe");
            Write(entry, "<AssetDeclaration/>"); Write(editable, "AAAA"); Write(art, new string('A', 256 * 1024));
            Write(declaration, "<AssetDeclaration id='Stable'/>"); Write(additional, "PACK"); Write(compiler, "compiler");
            var request = new BuildRequest { Command = "build", SdkRoot = sdk, ModRoot = mod, OutputRoot = Path.Combine(root, "output"), Content = new[] { "base" }, PackageFormat = "loose" };
            var plan = new BuildPlan { Tasks = new[] { new BuildTask { Id = "base/mod", Content = "base", InputPath = entry, OutputName = "base/mod", Dependencies = new string[0] } } };
            var fresh = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None);
            long stableBytes = new FileInfo(art).Length + new FileInfo(declaration).Length + new FileInfo(compiler).Length;
            CacheInputSnapshot last;
            using (var leases = new CacheInputLeaseSet())
            {
                var first = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None, leases);
                var second = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None, leases);
                Check(first.Fingerprint == fresh.Fingerprint && first.ToolchainFingerprint == fresh.ToolchainFingerprint && first.HashedBytes == fresh.HashedBytes && first.ReusedHashBytes == 0,
                    "Stable SDK leasing must preserve the original first-capture identities and full byte reads.");
                Check(second.Fingerprint == fresh.Fingerprint && second.ToolchainFingerprint == fresh.ToolchainFingerprint && leases.RetainedFiles == 3,
                    "Only stable SDK Art/SageXml and runtime files may retain leases in the nested-MOD fixture.");
                Check(second.ReusedHashBytes == stableBytes && second.HashedBytes + second.ReusedHashBytes == fresh.HashedBytes,
                    "Repeated SDK capture must reuse exact stable bytes while rehashing Additional and both appearances of nested MOD inputs.");
                foreach (string path in new[] { art, declaration })
                    Reject<IOException>(() => { using (File.Open(path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite)) { } },
                        "An SDK digest may be reused only while its actual file bytes remain under a read lease: " + path);

                DateTime editTime = File.GetLastWriteTimeUtc(editable);
                Write(editable, "BBBB"); File.SetLastWriteTimeUtc(editable, editTime);
                Check(new FileInfo(editable).Length == 4 && File.GetLastWriteTimeUtc(editable) == editTime,
                    "Nested MOD mutation must preserve exact file length and mtime.");
                var changedMod = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None, leases);
                Check(changedMod.Fingerprint != second.Fingerprint && changedMod.ToolchainFingerprint == fresh.ToolchainFingerprint && changedMod.ReusedHashBytes == stableBytes,
                    "An SDK-role lease must not lock or hide an equal-length/equal-time mutation anywhere below the active MOD root.");
                Check(changedMod.HashedBytes + changedMod.ReusedHashBytes == fresh.HashedBytes,
                    "Nested MOD files must be read freshly in both SDK and MOD inventory roles.");

                DateTime additionalTime = File.GetLastWriteTimeUtc(additional);
                Write(additional, "TAPE"); File.SetLastWriteTimeUtc(additional, additionalTime);
                Check(new FileInfo(additional).Length == 4 && File.GetLastWriteTimeUtc(additional) == additionalTime,
                    "SDK Additional mutation must preserve exact file length and mtime.");
                last = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None, leases);
                Check(last.Fingerprint != changedMod.Fingerprint && last.ToolchainFingerprint == fresh.ToolchainFingerprint && last.ReusedHashBytes == stableBytes,
                    "SDK Additional must remain writable and freshly content-hashed without widening tool identity.");
                Check(last.Fingerprint == CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None).Fingerprint && last.HashedBytes + last.ReusedHashBytes == fresh.HashedBytes,
                    "Shared SDK capture must agree with a completely fresh inventory after mutable input changes.");
            }
            DateTime artTime = File.GetLastWriteTimeUtc(art);
            byte[] changed = File.ReadAllBytes(art); changed[0] ^= 0x7f; File.WriteAllBytes(art, changed); File.SetLastWriteTimeUtc(art, artTime);
            using (var next = new CacheInputLeaseSet())
            {
                var changedArt = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None, next);
                Check(changedArt.Fingerprint != last.Fingerprint && changedArt.ToolchainFingerprint == fresh.ToolchainFingerprint && changedArt.ReusedHashBytes == 0,
                    "A new lease set must hash edited SDK Art bytes afresh without treating ordinary SDK assets as compiler tools.");
            }
            using (var limited = new CacheInputLeaseSet(1))
            {
                CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None, limited);
                var again = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None, limited);
                Check(limited.RetainedFiles == 1 && again.ReusedHashBytes == new FileInfo(art).Length && again.HashedBytes + again.ReusedHashBytes == fresh.HashedBytes,
                    "A shared SDK file must obey the existing handle cap and largest-file preference while remaining inputs use fresh hashes.");
            }
        }
        public static int Run(string root)
        {
            assertions = 0;
            Directory.CreateDirectory(root);
            var request = new BuildRequest { Command = "build", SdkRoot = Path.Combine(root, "sdk"), ModRoot = Path.Combine(root, "mod"), OutputRoot = Path.Combine(root, "output"), Content = new[] { "base" }, PackageFormat = "loose" };
            string runtime = Path.Combine(root, "runtime");
            string entry = Path.Combine(request.ModRoot, "Data", "mod.xml");
            string dependency = Path.Combine(request.ModRoot, "textures", "payload.dds");
            string schema = Path.Combine(request.SdkRoot, "Schemas", "types.xsd");
            string config = Path.Combine(request.SdkRoot, "tools", "BinaryAssetBuilder.exe.config");
            string compiler = Path.Combine(runtime, "NewModCompiler.exe");
            Write(entry, "<AssetDeclaration/>"); Write(dependency, "AAAA"); Write(schema, "schema"); Write(config, "config"); Write(compiler, "compiler");
            var plan = new BuildPlan { Tasks = new[] { new BuildTask { Id = "base/mod", Content = "base", InputPath = entry, OutputName = "base/mod", Dependencies = new string[0] } } };
            request.Options["tp"] = new EffectiveOption { Value = "Win32", Source = "commandLine" };
            var baseline = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None);
            Check(!baseline.CanAuthorizeCacheReuse, "An inventory must never authorize production cache reuse.");
            Check(CacheInputSnapshot.CoverageLimitations.Contains("Root-external"), "Missing inventory scope explanation.");
            Check(baseline.Fingerprint.Length == 64 && baseline.FileCount == 5, "Inventory digest and file count.");
            Check(baseline.ToolchainFingerprint.Length == 64, "Tool identity must be a separate SHA-256 digest.");
            long expectedBytes = 0;
            foreach (string file in new[] { entry, dependency, schema, config, compiler }) expectedBytes += new FileInfo(file).Length;
            Check(baseline.HashedBytes > 0 && baseline.HashedBytes == expectedBytes, "Inventory byte accounting must include each actual input read exactly once.");
            var unchanged = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None);
            Check(unchanged.ToolchainFingerprint == baseline.ToolchainFingerprint && unchanged.HashedBytes == baseline.HashedBytes, "Unchanged tool identity or byte accounting is unstable.");
            Check(baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Identical cold/hot inventory differs.");

            // A plugin payload absent from the XML task list still invalidates
            // the root inventory, including same-length/same-timestamp edits.
            DateTime timestamp = File.GetLastWriteTimeUtc(dependency);
            Write(dependency, "BBBB"); File.SetLastWriteTimeUtc(dependency, timestamp);
            Check(!baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Payload content change was missed.");
            var payloadChange = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None);
            Check(payloadChange.Fingerprint != baseline.Fingerprint && payloadChange.ToolchainFingerprint == baseline.ToolchainFingerprint, "Ordinary payload bytes leaked into tool identity or escaped whole-root protection.");
            Check(payloadChange.HashedBytes == baseline.HashedBytes, "Same-size payload mutation changed byte accounting.");
            Write(dependency, "AAAA"); File.SetLastWriteTimeUtc(dependency, timestamp);
            Check(baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Restored input bytes should reproduce the fingerprint.");
            foreach (string file in new[] { entry, schema, config, compiler })
            {
                string previous = File.ReadAllText(file); DateTime previousTime = File.GetLastWriteTimeUtc(file);
                Write(file, (previous[0] == 'X' ? "Y" : "X") + previous.Substring(1)); File.SetLastWriteTimeUtc(file, previousTime);
                Check(!baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Root input change was missed: " + file);
                var changed = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None);
                Check(changed.HashedBytes == baseline.HashedBytes, "Same-size input mutation changed byte accounting: " + file);
                if (file == entry)
                    Check(changed.ToolchainFingerprint == baseline.ToolchainFingerprint, "MOD XML bytes were included in tool identity.");
                else
                    Check(changed.ToolchainFingerprint != baseline.ToolchainFingerprint, "Same-size/same-mtime tool, config or schema mutation did not invalidate tool identity: " + file);
                Write(file, previous); File.SetLastWriteTimeUtc(file, previousTime);
                Check(CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None).ToolchainFingerprint == baseline.ToolchainFingerprint, "Restoring tool bytes did not restore tool identity: " + file);
            }
            // Packaging-only additions and normal SDK declarations remain root
            // protection inputs, without invalidating unrelated action tools.
            foreach (string file in new[] {
                Path.Combine(request.ModRoot, "Additional", "packaging.bin"),
                Path.Combine(request.SdkRoot, "Additional", "packaging.bin"),
                Path.Combine(request.SdkRoot, "SageXml", "Weapon.xml")
            })
            {
                Write(file, "AAAA");
                var added = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None);
                Check(added.Fingerprint != baseline.Fingerprint && added.ToolchainFingerprint == baseline.ToolchainFingerprint, "Packaging/SDK asset addition widened tool identity or escaped root protection: " + file);
                DateTime previousTime = File.GetLastWriteTimeUtc(file); Write(file, "BBBB"); File.SetLastWriteTimeUtc(file, previousTime);
                var changed = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None);
                Check(changed.Fingerprint != added.Fingerprint && changed.ToolchainFingerprint == added.ToolchainFingerprint, "Packaging/SDK asset edit widened tool identity or escaped root protection: " + file);
                Check(changed.HashedBytes == added.HashedBytes && added.HashedBytes > baseline.HashedBytes, "Packaging/SDK asset byte accounting is inconsistent.");
                File.Delete(file); Directory.Delete(Path.GetDirectoryName(file));
            }
            Check(baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Removing packaging/SDK asset probes did not restore the inventory.");
            string backendTool = Path.Combine(request.SdkRoot, "tools", "backend.dll"); Write(backendTool, "tool-implementation");
            Check(CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None).ToolchainFingerprint != baseline.ToolchainFingerprint, "New SDK tool implementation did not invalidate tool identity.");
            File.Delete(backendTool);
            string indirect = Path.Combine(request.ModRoot, "includes", "nested", "indirect.xml");
            Write(indirect, "new dependency");
            Check(!baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Added nested dependency was missed.");
            File.Delete(indirect); Directory.Delete(Path.GetDirectoryName(indirect)); Directory.Delete(Path.GetDirectoryName(Path.GetDirectoryName(indirect)));
            Check(baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Removed dependency tree did not restore inventory.");
            File.Move(dependency, dependency + ".renamed");
            Check(!baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Renamed payload was missed.");
            File.Move(dependency + ".renamed", dependency);
            request.Options["tp"].Value = "Xbox360";
            Check(!baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Platform change was missed.");
            request.Options["tp"].Value = "Win32";
            request.Options["unrecognized-future-option"] = new EffectiveOption { Value = "yes", Source = "commandLine" };
            Check(!baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "New effective option was missed.");
            request.Options.Remove("unrecognized-future-option");
            plan.Tasks[0].OutputName = "base/changed";
            Check(!baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Output semantics change was missed.");
            plan.Tasks[0].OutputName = "base/mod";
            plan.Tasks[0].Dependencies = new[] { "apt/dependency" };
            Check(!baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Task dependencies change was missed.");
            plan.Tasks[0].Dependencies = new string[0];
            request.PackageFormat = "big";
            Check(!baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Packaging change was missed.");
            request.PackageFormat = "loose";
            string previousMod = request.ModRoot;
            request.ModRoot = Path.Combine(root, "second-mod");
            Write(Path.Combine(request.ModRoot, "Data", "mod.xml"), "<AssetDeclaration/>"); Write(Path.Combine(request.ModRoot, "textures", "payload.dds"), "AAAA");
            plan.Tasks[0].InputPath = Path.Combine(request.ModRoot, "Data", "mod.xml");
            Check(!baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Two identical MOD directories were not isolated.");
            request.ModRoot = previousMod; plan.Tasks[0].InputPath = entry;
            using (var cancelled = new CancellationTokenSource())
            {
                cancelled.Cancel(); Reject<OperationCanceledException>(() => CacheInputSnapshot.Capture(request, plan, runtime, cancelled.Token), "Cancelled inventory succeeded.");
            }
            Check(baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Cancellation polluted retry.");
            File.Delete(entry);
            Reject<FileNotFoundException>(() => CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None), "Missing stream input succeeded.");
            Write(entry, "<AssetDeclaration/>");
            Check(baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Failed inventory polluted retry.");
            plan.Tasks[0].InputPath = compiler;
            Reject<IOException>(() => CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None), "Root-external task was silently inventoried.");
            plan.Tasks[0].InputPath = entry;
            string output = request.OutputRoot; request.OutputRoot = Path.Combine(request.ModRoot, "output");
            Reject<IOException>(() => CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None), "Output was allowed inside inputs.");
            request.OutputRoot = output;
            Check(baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Validation failure polluted retry.");
            SharedToolLeases(request, plan, runtime, dependency, schema, config, compiler, baseline);
            StableSdkLeases(Path.Combine(root, "stable-sdk-inputs"));
            return assertions;
        }
    }
}
