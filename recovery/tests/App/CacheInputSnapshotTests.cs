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
            return assertions;
        }
    }
}
