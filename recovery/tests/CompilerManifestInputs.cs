using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Utility;

// Exercise Utility's real unsafe manifest parser through both Core consumers.
// The existing full CLI/SDK matrix remains the end-to-end acceptance gate.
public static class CompilerManifestInputs
{
    static int checks;
    static readonly List<string> tests = new List<string>();
    static readonly List<object> observations = new List<object>();
    static readonly JavaScriptSerializer Json = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
    const BindingFlags Hidden = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    static void Require(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    static IDictionary<string, object> Metrics(PhysicalInputSnapshot snapshot)
    { return Json.Deserialize<Dictionary<string, object>>(Json.Serialize(snapshot.GetMetrics())); }
    static long Number(IDictionary<string, object> metrics, string name) { return Convert.ToInt64(metrics[name]); }
    static Func<string, Stream> Reader()
    {
        var type = typeof(DocumentProcessor).Assembly.GetType("BinaryAssetBuilder.Core.ManifestInputReader", true);
        return (Func<string, Stream>)Delegate.CreateDelegate(typeof(Func<string, Stream>), type.GetMethod("OpenRead", Hidden));
    }
    static Func<string, bool> Exists()
    {
        var type = typeof(DocumentProcessor).Assembly.GetType("BinaryAssetBuilder.Core.ManifestInputReader", true);
        return (Func<string, bool>)Delegate.CreateDelegate(typeof(Func<string, bool>), type.GetMethod("Exists", Hidden));
    }
    static void Blocked(Action action, string message)
    {
        bool blocked = false;
        try { action(); } catch (IOException) { blocked = true; } catch (UnauthorizedAccessException) { blocked = true; }
        Require(blocked, message);
    }
    static void ManifestFile(string path, string patch, string name = "Armor:Sealed")
    {
        byte[] references = patch == null ? new byte[0] : new byte[] { 2 }.Concat(Encoding.ASCII.GetBytes(patch + "\0")).ToArray();
        byte[] names = Encoding.ASCII.GetBytes(name + "\0"), source = Encoding.ASCII.GetBytes("source.xml\0");
        using (var output = new BinaryWriter(File.Create(path)))
        {
            output.Write((byte)0); output.Write((byte)1); output.Write((ushort)6);
            foreach (uint value in new uint[] { 1234, 0, 1, 0, 0, 0, 0, 8, (uint)references.Length, (uint)names.Length, (uint)source.Length }) output.Write(value);
            foreach (uint value in new uint[] { 99, 77, 11, 22, 0, 1, 0, 0, 0, 0, 0, 0 }) output.Write(value);
            output.Write((uint)99); output.Write((uint)77);
            output.Write(references); output.Write(names); output.Write(source);
        }
    }
    static AssetDeclarationDocument Parent()
    {
        var document = new AssetDeclarationDocument();
        var current = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        typeof(AssetDeclarationDocument).GetField("_Current", Hidden).SetValue(document, Activator.CreateInstance(current, true));
        return document;
    }
    static object Invoke(object target, string method, params object[] arguments)
    {
        try { return target.GetType().GetMethod(method, Hidden).Invoke(target, arguments); }
        catch (TargetInvocationException error) { throw error.InnerException; }
    }
    static void Cases(string work)
    {
        string root = Path.Combine(work, "external #25% manifests"), search = Path.Combine(root, "search"), later = Path.Combine(root, "later");
        Directory.CreateDirectory(root); Directory.CreateDirectory(search); Directory.CreateDirectory(later);
        string main = Path.Combine(root, "main.manifest"), patch = Path.Combine(search, "base.manifest"), shadow = Path.Combine(later, "base.manifest");
        string hashes = Path.Combine(root, "StringHashes.xml");
        ManifestFile(main, "base.manifest"); ManifestFile(patch, null); ManifestFile(shadow, null, "Armor:Shadow");
        File.WriteAllText(hashes, "<StringHashes><StringAndHash Hash='99' Text='Armor'/><StringAndHash Hash='77' Text='Sealed'/></StringHashes>", new UTF8Encoding(false));
        var searchPaths = new[] { search, later };
        var open = Reader(); var exists = Exists();
        using (var legacy = new Manifest())
        {
            Require(legacy.Load(main, searchPaths), "Legacy manifest load failed");
            Require(legacy.Assets.Length == 1 && legacy.Assets[0].QualifiedName == "Armor:Sealed", "Legacy patch fixture did not resolve asset");
            Require(legacy.PatchManifest.FileName == patch && legacy.Assets[0].ExternalReferences[0].QualifiedName == "Armor:Sealed", "Legacy patch order/hash mapping precondition failed");
        }
        Require(open(main) == null, "Reader outside a snapshot changed Utility fallback");
        var previous = Settings.Current;
        Settings.Current = new Settings { OutputDirectory = Path.Combine(work, "generated"), TargetPlatform = TargetPlatform.Win32, LinkedStreams = true, CustomPostfix = "", BuildCache = false, SessionCache = false };
        try
        {
            var processor = new DocumentProcessor(Settings.Current, new PluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32), null);
            using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, "snapshot")))
            using (snapshot.BeginReadScope())
            {
                var parent = Parent();
                Require((bool)Invoke(processor, "LoadPrecompiledReference", parent, main, searchPaths), "Core precompiled reference load failed");
                Require(parent.ReferenceInstances.Count == 1 && parent.ReferenceInstances.First().Handle.InstanceName == "Sealed", "Core reference symbols changed");
                var manager = new OutputManager(processor, null, Path.Combine(work, "generated", "stream"), Path.Combine(work, "intermediate"), main, "", searchPaths);
                Require(manager.BasePatchStreamManifest.PatchManifest.FileName == patch, "Core base patch selection changed");
                Require(manager.BasePatchStreamAssets.Count == 1, "Core base patch metadata was not loaded");
                using (var first = new Manifest()) using (var second = new Manifest())
                {
                    Require(first.Load(main, searchPaths, true, open, exists) && second.Load(main, searchPaths, true, open, exists), "Repeated sealed Utility reads failed");
                    Require(first.StringFromHash(99) == "Armor" && first.Assets[0].ExternalReferences[0].QualifiedName == "Armor:Sealed", "Sealed hash table/ref names changed");
                    Require(first.PatchManifest.FileName == patch && second.Assets[0].QualifiedName == "Armor:Sealed", "Repeated parse mixed stream cursors/patches");
                }
                var metrics = Metrics(snapshot);
                Require(Number(metrics, "captures") == 3 && Number(metrics, "hits") >= 9, "Root, recursive patch and StringHashes did not share exactly three positive inputs");
                Blocked(() => File.Delete(main), "Root manifest was not pinned");
                Blocked(() => File.WriteAllText(patch, "mutated"), "Recursive patch manifest was not pinned");
                Blocked(() => File.Delete(hashes), "Discovered StringHashes was not pinned");
                using (var writer = new FileStream(shadow, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
                snapshot.VerifyUnchanged(); metrics = Metrics(snapshot);
                Require(Number(metrics, "verificationReads") == 3, "Publication verification omitted a manifest/hash input");
                observations.Add(new { id = "core-reference-base-patch-and-recursive-content", metrics });
            }
            foreach (string path in new[] { main, patch, hashes }) using (var writer = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
            tests.Add("real-Core-precompiled-and-base-patch-consumers-recursive-manifest-and-StringHashes-positive-content-sharing-pins-and-release");
            using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, "fallback"), 0, 0, 0, 0))
            using (snapshot.BeginReadScope())
            using (var writer = new FileStream(main, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
            using (var manifest = new Manifest())
            {
                Require(manifest.Load(main, searchPaths, true, open), "Budget fallback lost legacy FileShare.ReadWrite manifest behavior");
                Require(Number(Metrics(snapshot), "captures") == 0, "Zero-budget fallback unexpectedly captured inputs");
            }
            tests.Add("bounded-fallback-retains-original-readwrite-sharing-and-old-public-overloads");
            using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, "cancelled")))
            using (snapshot.BeginReadScope(new CancellationToken(true)))
            {
                bool cancelled = false;
                try { Invoke(processor, "LoadPrecompiledReference", Parent(), main, searchPaths); } catch (OperationCanceledException) { cancelled = true; }
                Require(cancelled, "Core precompiled reader swallowed cancellation as a missing manifest");
                cancelled = false;
                try { new OutputManager(processor, null, Path.Combine(work, "cancel-output"), Path.Combine(work, "cancel-intermediate"), main, "", searchPaths); } catch (OperationCanceledException) { cancelled = true; }
                Require(cancelled, "Core base patch reader swallowed cancellation as a missing manifest");
            }
            tests.Add("Core-precompiled-read-cancellation-is-not-converted-to-compatibility-fallback");
            CandidateCases(work, open, exists);
            string missing = Path.Combine(root, "missing.manifest");
            using (var manifest = new Manifest()) Require(!manifest.Load(missing, searchPaths, true, open), "Missing manifest changed legacy false result");
            ManifestFile(main, "absent.manifest");
            using (var manifest = new Manifest())
            {
                bool rejected = false;
                try { manifest.Load(main, searchPaths, true, open); } catch (ArgumentException error) { rejected = error.Message.Contains("Base patch manifest absent.manifest does not exist."); }
                Require(rejected, "Missing recursive patch changed the legacy error");
            }
            tests.Add("missing-root-and-recursive-patch-retain-original-results-and-diagnostics");
        }
        finally { Settings.Current = previous; }
    }
    static void CandidateCases(string work, Func<string, Stream> open, Func<string, bool> exists)
    {
        foreach (string change in new[] { "earlier-patch", "nearer-hashes", "asset-presence" })
        {
            string root = Path.Combine(work, change), search = Path.Combine(root, "search");
            Directory.CreateDirectory(root); Directory.CreateDirectory(search);
            string main = Path.Combine(root, "main.manifest"), patch = Path.Combine(search, "base.manifest");
            ManifestFile(main, "base.manifest"); ManifestFile(patch, null);
            File.WriteAllText(Path.Combine(root, "StringHashes.xml"), "<StringHashes/>");
            using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, change + "-snapshot")))
            using (snapshot.BeginReadScope())
            using (var manifest = new Manifest())
            {
                Require(manifest.Load(main, new[] { search }, true, open, exists), "Candidate fixture could not load baseline");
                if (change == "earlier-patch") ManifestFile(Path.Combine(root, "base.manifest"), null, "Armor:Earlier");
                else if (change == "nearer-hashes")
                {
                    string nearer = Path.Combine(root, "main"); Directory.CreateDirectory(nearer);
                    File.WriteAllText(Path.Combine(nearer, "StringHashes.xml"), "<StringHashes/>");
                }
                else
                {
                    string asset = manifest.Assets[0].FileBasePath + ".asset"; Directory.CreateDirectory(Path.GetDirectoryName(asset)); File.WriteAllBytes(asset, new byte[0]);
                }
                bool rejected = false;
                try { snapshot.VerifyUnchanged(); } catch (IOException) { rejected = true; }
                Require(rejected, "Publication accepted changed manifest discovery: " + change);
                observations.Add(new { id = change, metrics = Metrics(snapshot) });
            }
        }
        tests.Add("original-patch-StringHashes-and-asset-presence-candidate-observations-reject-new-priorities-before-publication");
    }
    public static int Main(string[] args)
    {
        object result; int code = 0;
        try
        {
            Cases(args[0]);
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), observations = observations.ToArray(),
                scope = "Actual Core precompiled/base patch manifest consumers, Utility recursive patch/hash reads, executed discovery queries and bounded fallback; generated outputs and hidden plugin closures remain separate" };
            Console.WriteLine("PASS ManifestInputs: " + tests.Count + " groups; " + checks + " checks");
        }
        catch (Exception error)
        {
            code = 1; Console.Error.WriteLine(error); result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null,
                runId = args.Length > 3 ? args[3] : null, checks, tests = tests.ToArray(), observations = observations.ToArray(), error = error.ToString() };
        }
        if (args.Length > 1) File.WriteAllText(args[1], Json.Serialize(result)); return code;
    }
}
