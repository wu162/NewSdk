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
            string incremental = baseline.IncrementalFingerprint;
            Check(incremental.Length == 64 && incremental != baseline.Fingerprint, "Incremental identity must be versioned separately from the whole-input guard.");
            Check(!baseline.CanAuthorizeCacheReuse, "An inventory must never authorize production cache reuse.");
            Check(CacheInputSnapshot.CoverageLimitations.Contains("Root-external"), "Missing inventory scope explanation.");
            Check(baseline.Fingerprint.Length == 64 && baseline.FileCount == 5, "Inventory digest and file count.");
            Check(baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Identical cold/hot inventory differs.");

            // A plugin payload absent from the XML task list still invalidates
            // the root inventory, including same-length/same-timestamp edits.
            DateTime timestamp = File.GetLastWriteTimeUtc(dependency);
            Write(dependency, "BBBB"); File.SetLastWriteTimeUtc(dependency, timestamp);
            Check(!baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Payload content change was missed.");
            Check(CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None).IncrementalFingerprint == incremental, "MOD content edit discarded dependency-aware Core reuse.");
            Write(dependency, "AAAA"); File.SetLastWriteTimeUtc(dependency, timestamp);
            Check(baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Restored input bytes should reproduce the fingerprint.");
            foreach (string file in new[] { entry, schema, config, compiler })
            {
                string previous = File.ReadAllText(file); Write(file, previous + "changed");
                Check(!baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Root input change was missed: " + file);
                Check((CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None).IncrementalFingerprint == incremental) == (file == entry), "Only MOD content edits may retain incremental namespace: " + file);
                Write(file, previous);
            }
            string indirect = Path.Combine(request.ModRoot, "includes", "nested", "indirect.xml");
            Write(indirect, "new dependency");
            Check(!baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Added nested dependency was missed.");
            Check(CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None).IncrementalFingerprint != incremental, "Added resolver candidate retained incremental namespace.");
            File.Delete(indirect); Directory.Delete(Path.GetDirectoryName(indirect)); Directory.Delete(Path.GetDirectoryName(Path.GetDirectoryName(indirect)));
            Check(baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Removed dependency tree did not restore inventory.");
            File.Move(dependency, dependency + ".renamed");
            Check(!baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Renamed payload was missed.");
            Check(CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None).IncrementalFingerprint != incremental, "Renamed resolver candidate retained incremental namespace.");
            File.Move(dependency + ".renamed", dependency);
            request.Options["tp"].Value = "Xbox360";
            Check(!baseline.MatchesCurrent(request, plan, runtime, CancellationToken.None), "Platform change was missed.");
            Check(CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None).IncrementalFingerprint != incremental, "Changed platform retained incremental namespace.");
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
            // A common layout is SDK/Mods/<active mod>. Both inventory roles
            // must agree that editing that MOD is handled by Core dependencies.
            request.ModRoot = Path.Combine(request.SdkRoot, "Mods", "nested");
            plan.Tasks[0].InputPath = Path.Combine(request.ModRoot, "Data", "Mod.xml");
            Write(plan.Tasks[0].InputPath, "<AssetDeclaration/>");
            var nested = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None);
            Write(plan.Tasks[0].InputPath, "<AssetDeclaration />");
            var edited = CacheInputSnapshot.Capture(request, plan, runtime, CancellationToken.None);
            Check(nested.Fingerprint != edited.Fingerprint && nested.IncrementalFingerprint == edited.IncrementalFingerprint, "SDK inventory reintroduced whole-MOD invalidation for nested MODs.");
            return assertions;
        }
    }
}
