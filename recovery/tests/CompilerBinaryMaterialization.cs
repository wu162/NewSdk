using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Utility;
using NewModCompiler.Application;
using EALA.Hash;

// Actual CopyAsset sources, not manually substituted streams or fabricated
// output bytes. The full CLI/SDK matrix remains a separate acceptance gate.
public static class CompilerBinaryMaterialization
{
    const BindingFlags Hidden = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    const string Disable = "NewSdk.Compiler.DisableBinaryMaterializationCapture";
    static string work, sdk;
    static int checks;
    static readonly List<string> tests = new List<string>();
    static readonly List<object> observations = new List<object>();
    static readonly JavaScriptSerializer Json = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
    static void Require(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    static IDictionary<string, object> Metrics(PhysicalInputSnapshot snapshot)
    { return Json.Deserialize<Dictionary<string, object>>(Json.Serialize(snapshot.GetMetrics())); }
    static long Number(IDictionary<string, object> metrics, string key) { return Convert.ToInt64(metrics[key]); }
    static long Attempt(IDictionary<string, object> metrics, string key)
    { return Number((IDictionary<string, object>)metrics["binaryMaterialization"], key); }
    static long CacheQuery(IDictionary<string, object> metrics, string key)
    { return Number((IDictionary<string, object>)metrics["binaryCacheCandidates"], key); }
    static void RejectedChange(PhysicalInputSnapshot snapshot, string message)
    {
        bool rejected = false;
        try { snapshot.VerifyUnchanged(); } catch (IOException) { rejected = true; }
        Require(rejected, message);
    }
    static void WithDocument(string name, Action action)
    {
        using (DocumentProcessor.BeginStateScope())
        {
            var documents = (Stack<string>)typeof(DocumentProcessor).GetField("_CurrentDocumentStack", Hidden).GetValue(null);
            documents.Push(name);
            try { action(); }
            finally { Require(documents.Pop() == name && documents.Count == 0, "Cache query fixture changed diagnostic stack ownership"); }
        }
    }
    static BinaryAsset Asset(string name, bool custom)
    {
        var processor = new DocumentProcessor(Settings.Current, new PluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32), null);
        var document = new AssetDeclarationDocument();
        var type = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object current = Activator.CreateInstance(type, true); type.GetMethod("FromScratch").Invoke(current, new object[0]);
        type.GetField("DocumentProcessor").SetValue(current, processor); type.GetField("SourcePath").SetValue(current, Path.Combine(work, name, "root.xml"));
        type.GetField("SourceDirectory").SetValue(current, work); type.GetField("LogicalSourcePath").SetValue(current, "root.xml");
        type.GetField("SourcePathFromRoot").SetValue(current, name);
        typeof(AssetDeclarationDocument).GetField("_Current", Hidden).SetValue(document, current);
        var xml = new XmlDocument(); xml.LoadXml("<ArmorTemplate id='Materialization'/>");
        var instance = new InstanceDeclaration(document) { XmlNode = xml.DocumentElement, HasCustomData = custom };
        instance.Handle.TypeHash = 17; instance.Handle.InstanceHash = 19;
        var manager = new OutputManager(processor, null, Path.Combine(work, name, "output"), Path.Combine(work, name, "intermediate"), null, "", new string[0]);
        return manager.GetBinaryAsset(instance, true);
    }
    static string Source(BinaryAsset asset, string name, int bytes = 1048576)
    {
        string directory = Path.Combine(work, "external", name); Directory.CreateDirectory(directory);
        string path = Path.Combine(directory, asset.AssetFileName);
        var header = new AssetHeader { TypeId = asset.Instance.Handle.TypeId, InstanceId = asset.Instance.Handle.InstanceId,
            TypeHash = asset.Instance.Handle.TypeHash, InstanceHash = asset.Instance.Handle.InstanceHash,
            InstanceDataSize = bytes, RelocationDataSize = 0, ImportsDataSize = 0 };
        byte[] payload = new byte[bytes]; for (int i = 0; i < payload.Length; i++) payload[i] = (byte)(i % 251);
        using (var output = File.Create(path)) { header.SaveToStream(output, false); output.Write(payload, 0, payload.Length); }
        Require(header.IsValidFileLength(new FileInfo(path).Length), "Source header/length preflight failed"); return path;
    }
    static bool Copy(BinaryAsset asset, string source)
    {
        try { return (bool)typeof(BinaryAsset).GetMethod("CopyAsset", Hidden).Invoke(asset, new object[] { source, source + ".cdata" }); }
        catch (TargetInvocationException error) { throw error.InnerException; }
    }
    static string Destination(BinaryAsset asset) { return Path.Combine(asset.AssetOutputDirectory, asset.AssetFileName); }
    static void Writable(string path) { using (var input = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { } }
    static void Blocked(Action action, string message)
    {
        bool blocked = false; try { action(); } catch (IOException) { blocked = true; } catch (UnauthorizedAccessException) { blocked = true; }
        Require(blocked, message);
    }
    static void SuccessfulSources()
    {
        var asset = Asset("success", true); string source = Source(asset, "success");
        File.WriteAllBytes(source + ".cdata", new byte[] { 9, 4, 7 }); byte[] expected = File.ReadAllBytes(source);
        using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, "success-snapshot")))
        using (snapshot.BeginReadScope())
        {
            Require(Copy(asset, source), "Valid asset/cdata copy failed");
            Require(File.ReadAllBytes(Destination(asset)).SequenceEqual(expected), "Valid asset bytes changed");
            string custom = Path.Combine(asset.CustomDataOutputDirectory, asset.Instance.Handle.FileBase + ".cdata");
            Require(File.ReadAllBytes(custom).SequenceEqual(new byte[] { 9, 4, 7 }), "Custom data bytes changed");
            var metrics = Metrics(snapshot);
            Require(Number(metrics, "captures") == 2 && Number(metrics, "activeReadHandles") == 2 && Attempt(metrics, "commits") == 1,
                "Successful materialization did not retain exactly its validated asset and cdata sources");
            Blocked(() => File.Delete(source), "Validated source asset was not pinned");
            Blocked(() => File.WriteAllBytes(source + ".cdata", new byte[] { 8 }), "Consumed custom data was not pinned");
            var second = Asset("success-second", true);
            Require(Copy(second, source), "Repeated source materialization failed");
            Require(File.ReadAllBytes(Destination(second)).SequenceEqual(expected), "Repeated source changed bytes");
            snapshot.VerifyUnchanged(); metrics = Metrics(snapshot);
            Require(Number(metrics, "captures") == 2 && Number(metrics, "hits") >= 2 && Number(metrics, "verificationReads") == 2,
                "Repeated materialization did not share one verified content generation");
            Require(Attempt(metrics, "copyReadHits") == 1 && Attempt(metrics, "captureReadsCompleted") == 1 &&
                Attempt(metrics, "teeCompleted") == 1 && Attempt(metrics, "teeBytes") == expected.Length,
                "Repeated asset copy did not consume its already sealed read view or redundantly rehashed its source");
            observations.Add(new { id = "validated-asset-cdata-and-repeat", metrics });
        }
        Writable(source); Writable(source + ".cdata");
        tests.Add("actual-CopyAsset-valid-identity-and-length-plus-cdata-success-pins-repeat-shares-final-SHA-and-dispose-releases");
    }
    static void FailedAttemptAndRetry()
    {
        var asset = Asset("missing-custom", true); string source = Source(asset, "missing-custom");
        string scratch = Path.Combine(work, "missing-custom-snapshot");
        using (var snapshot = new PhysicalInputSnapshot(scratch))
        using (snapshot.BeginReadScope())
        {
            Require(!Copy(asset, source) && !File.Exists(Destination(asset)), "Missing custom data lost the original false/output-cleanup contract");
            var failed = Metrics(snapshot);
            Require(Number(failed, "captures") == 0 && Number(failed, "activeReadHandles") == 0 && Number(failed, "activeDirectoryPins") == 0 &&
                Number(failed, "diskBytes") == 0 && Attempt(failed, "rollbacks") == 1, "Failed materialization retained provisional file/directory/storage ownership");
            Require(Attempt(failed, "captureReadsStarted") == 0 && Attempt(failed, "captureReadsCompleted") == 0 &&
                Attempt(failed, "teeCompleted") == 1 && Attempt(failed, "teeBytes") == new FileInfo(source).Length,
                "Asset tee performed an extra source pass or rollback erased actual spooled work");
            Writable(source); File.Delete(source); source = Source(asset, "missing-custom");
            File.WriteAllBytes(source + ".cdata", new byte[] { 2, 3 });
            Require(Copy(asset, source), "Repair and retry within the same generation failed");
            snapshot.VerifyUnchanged();
            observations.Add(new { id = "failed-custom-data-rolls-back-before-repair-and-retry", failed, repaired = Metrics(snapshot) });
        }
        Require(!Directory.Exists(scratch), "Provisional disk storage survived snapshot disposal");
        Writable(source); Writable(source + ".cdata");
        tests.Add("missing-cdata-after-valid-large-asset-releases-provisional-content-storage-parents-before-delete-repair-and-retry");
    }
    static void ExistingEntryAliasRollback()
    {
        var asset = Asset("alias-failure", true); string source = Source(asset, "alias-failure", 64);
        using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, "alias-snapshot")))
        using (snapshot.BeginReadScope())
        {
            PhysicalInputSnapshot.Entry entry; Require(PhysicalInputSnapshot.TryCapture(source, out entry), "Alias precondition did not capture source");
            Require(!Copy(asset, source.ToUpperInvariant()), "Missing custom data alias unexpectedly copied");
            var failed = Metrics(snapshot);
            Require(Number(failed, "captures") == 1 && Number(failed, "activeReadHandles") == 1 && Number(failed, "activeAliasPins") == 0,
                "Failed alias attempt released prior content ownership or leaked its new name handle");
            Blocked(() => File.Delete(source), "Rollback released an earlier successful owner's pin");
            snapshot.VerifyUnchanged(); observations.Add(new { id = "new-alias-rollback-preserves-preexisting-generation", metrics = Metrics(snapshot) });
        }
        Writable(source);
        tests.Add("failed-attempt-alias-rollback-preserves-earlier-entry-and-its-original-path-pin");
    }
    static void ConservativeModes()
    {
        foreach (string mode in new[] { "disabled", "budget", "generated" })
        {
            var asset = Asset(mode, true); string source = Source(asset, mode, 64); File.WriteAllBytes(source + ".cdata", new byte[] { 1 });
            AppContext.SetSwitch(Disable, mode == "disabled");
            using (var snapshot = mode == "budget" ? new PhysicalInputSnapshot(Path.Combine(work, mode + "-snapshot"), 0, 0, 0, 0) : new PhysicalInputSnapshot(Path.Combine(work, mode + "-snapshot")))
            {
                if (mode == "generated") snapshot.ExcludeRoot(Path.GetDirectoryName(source));
                using (snapshot.BeginReadScope())
                {
                    Require(Copy(asset, source), "Conservative mode changed the original successful copy: " + mode);
                    Require(Number(Metrics(snapshot), "captures") == 0 && Number(Metrics(snapshot), "activeReadHandles") == 0, "Conservative mode retained source pins: " + mode);
                    Writable(source); Writable(source + ".cdata"); snapshot.VerifyUnchanged();
                    observations.Add(new { id = mode, metrics = Metrics(snapshot) });
                }
            }
        }
        AppContext.SetSwitch(Disable, false);
        var cancelledAsset = Asset("cancelled", false); string cancelledSource = Source(cancelledAsset, "cancelled", 64);
        using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, "cancelled-snapshot")))
        using (snapshot.BeginReadScope(new CancellationToken(true)))
        {
            bool cancelled = false; try { Copy(cancelledAsset, cancelledSource); } catch (OperationCanceledException) { cancelled = true; }
            Require(cancelled && Number(Metrics(snapshot), "activeReadHandles") == 0 && !File.Exists(Destination(cancelledAsset)),
                "Capture swallowed cancellation, leaked provisional ownership or retained cancelled output");
        }
        Writable(cancelledSource);
        tests.Add("independent-disable-zero-budget-generated-root-conservative-fallback-and-capture-cancellation-release");
    }
    static void ConsumedHandleIdentity()
    {
        var asset = Asset("identity-race", false);
        string original = Source(asset, "identity-original", 64), replacement = Source(asset, "identity-replacement", 64);
        // Use exactly equal bytes and lengths so only the open file's native
        // identity can reject a pathname that no longer names the consumed
        // object. Reflection reaches the same production post-copy check;
        // no timing callback is added to CopyAsset.
        File.Copy(original, replacement, true);
        Require(File.ReadAllBytes(original).SequenceEqual(File.ReadAllBytes(replacement)), "Identity fixture must use equal-content distinct files");
        using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, "identity-race-snapshot")))
        using (snapshot.BeginReadScope())
        using (var consumed = File.OpenRead(original))
        {
            var begin = typeof(PhysicalInputSnapshot).GetMethod("BeginMaterializationCapture", Hidden);
            var prepare = typeof(PhysicalInputSnapshot).GetMethod("BeginMaterializationContent", Hidden);
            bool rejected = false;
            using ((IDisposable)begin.Invoke(null, new object[0]))
            {
                object content = prepare.Invoke(null, new object[] { replacement, consumed });
                byte[] bytes = new byte[checked((int)consumed.Length)]; int count = 0, next;
                while (count < bytes.Length && (next = consumed.Read(bytes, count, bytes.Length - count)) != 0) count += next;
                Require(count == bytes.Length, "Small identity fixture did not read its complete source");
                content.GetType().GetMethod("Append", Hidden).Invoke(content, new object[] { bytes, 0, count });
                try { content.GetType().GetMethod("Complete", Hidden).Invoke(content, new object[] { consumed }); }
                catch (TargetInvocationException error) { if (!(error.InnerException is IOException)) throw error.InnerException; rejected = true; }
            }
            Require(rejected, "A replacement path with equal bytes/length was accepted for a different consumed file handle");
            var metrics = Metrics(snapshot);
            Require(Number(metrics, "captures") == 0 && Number(metrics, "activeReadHandles") == 0 && Number(metrics, "activeDirectoryPins") == 0 &&
                Attempt(metrics, "rollbacks") == 1 && Attempt(metrics, "captureReadsCompleted") == 0 && Attempt(metrics, "teeCompleted") == 1,
                "Consumed identity mismatch did not roll back its provisional pathname pin");
            Writable(replacement); observations.Add(new { id = "consumed-handle-versus-equal-content-replacement-identity", metrics });
        }
        Writable(original); Writable(replacement);
        tests.Add("post-copy-native-handle-identity-check-rejects-equal-length-equal-content-distinct-file-and-releases-provisional-pins");
    }
    static void CachedCommitCancellation()
    {
        var asset = Asset("cancelled-cache-commit", false); string source = Source(asset, "cancelled-cache-commit", 64);
        typeof(BinaryAsset).GetField("_CacheDirectory", Hidden).SetValue(asset, Path.GetDirectoryName(source));
        Require(asset.GetLocation(AssetLocation.Cache, AssetLocationOption.ForceUpdate) == AssetLocation.Cache,
            "Cancelled commit fixture did not establish an available external cache source");
        using (DocumentProcessor.BeginStateScope())
        {
            var documents = (Stack<string>)typeof(DocumentProcessor).GetField("_CurrentDocumentStack", Hidden).GetValue(null);
            documents.Push("cancelled-cache-commit:");
            try
            {
                using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, "cancelled-cache-commit-snapshot")))
                using (snapshot.BeginReadScope(new CancellationToken(true)))
                {
                    bool cancelled = false;
                    try { asset.Commit(); } catch (OperationCanceledException) { cancelled = true; }
                    var metrics = Metrics(snapshot);
                    Require(cancelled, "CommitFromCache converted materialization cancellation into a retry/final cache error");
                    Require(Attempt(metrics, "attempts") == 1 && Attempt(metrics, "rollbacks") == 1 && Number(metrics, "activeReadHandles") == 0 &&
                        !File.Exists(Destination(asset)), "Cancelled cached commit retried materialization or retained pins/output");
                    Writable(source); observations.Add(new { id = "public-Commit-external-cache-cancellation-not-retried", metrics });
                }
            }
            finally { Require(documents.Pop() == "cancelled-cache-commit:" && documents.Count == 0, "Cached commit cancellation changed diagnostic stack ownership"); }
        }
        tests.Add("public-BinaryAsset-Commit-real-external-cache-materialization-cancellation-propagates-after-one-attempt-without-sleep-retry-error-conversion-pins-or-output");
    }
    static void CapturedCacheKeepalive()
    {
        var asset = Asset("captured-cache-keepalive", true); string source = Source(asset, "captured-cache-keepalive");
        string custom = Path.Combine(Path.GetDirectoryName(source), asset.Instance.Handle.FileBase + ".cdata");
        File.WriteAllBytes(custom, new byte[] { 5, 8, 13 });
        var oldWrite = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        File.SetLastWriteTimeUtc(source, oldWrite); File.SetLastWriteTimeUtc(custom, oldWrite);
        typeof(BinaryAsset).GetField("_CacheDirectory", Hidden).SetValue(asset, Path.GetDirectoryName(source));
        Require(asset.GetLocation(AssetLocation.Cache, AssetLocationOption.ForceUpdate) == AssetLocation.Cache,
            "Keepalive fixture did not establish an available asset/cdata cache pair");
        using (DocumentProcessor.BeginStateScope())
        {
            var documents = (Stack<string>)typeof(DocumentProcessor).GetField("_CurrentDocumentStack", Hidden).GetValue(null);
            documents.Push("captured-cache-keepalive:");
            try
            {
                using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, "captured-cache-keepalive-snapshot")))
                using (snapshot.BeginReadScope())
                {
                    Require(asset.Commit() == AssetLocation.Cache, "Public asset/cdata commit did not use the real cache materialization path");
                    Require(File.GetLastWriteTimeUtc(source) > oldWrite && File.GetLastWriteTimeUtc(custom) > oldWrite,
                        "Retained source pins blocked the original asset/cdata cache keepalive timestamp updates");
                    Blocked(() => File.WriteAllBytes(source, new byte[] { 1 }), "Cache keepalive relaxed the asset content-write pin");
                    Blocked(() => File.WriteAllBytes(custom, new byte[] { 1 }), "Cache keepalive relaxed the cdata content-write pin");
                    Blocked(() => File.Delete(source), "Cache keepalive relaxed the original source delete pin");
                    var metrics = Metrics(snapshot);
                    Require(Attempt(metrics, "cacheAttributeTouches") == 2 && Attempt(metrics, "cacheAttributeTouchFailures") == 0 && Number(metrics, "captures") == 2,
                        "Captured asset/cdata keepalive did not complete two identity-checked metadata-only updates");
                    snapshot.VerifyUnchanged(); observations.Add(new { id = "public-Commit-captured-asset-cdata-keepalive-retains-content-and-delete-pins", metrics });
                }
            }
            finally { Require(documents.Pop() == "captured-cache-keepalive:" && documents.Count == 0, "Keepalive commit changed diagnostic stack ownership"); }
        }
        Writable(source); Writable(custom);
        tests.Add("public-BinaryAsset-Commit-captured-asset-and-cdata-keepalive-metadata-updates-preserve-content-write-delete-pins-and-final-content-verification");
    }
    static void NetworkCacheQueries()
    {
        foreach (string mode in new[] { "missing-appears", "custom-disappears", "short-circuit", "excluded", "budget" })
        {
            var asset = Asset("cache-query-" + mode, true); string source = Source(asset, "cache-query-" + mode, 64);
            string custom = Path.Combine(Path.GetDirectoryName(source), asset.Instance.Handle.FileBase + ".cdata");
            File.WriteAllBytes(custom, new byte[] { 4, 2 });
            if (mode != "custom-disappears") File.Delete(source);
            typeof(BinaryAsset).GetField("_CacheDirectory", Hidden).SetValue(asset, Path.GetDirectoryName(source));
            using (var snapshot = mode == "budget"
                ? new PhysicalInputSnapshot(Path.Combine(work, "query-snapshot-" + mode), 1048576, 1048576, 10, 10, 0, 0)
                : new PhysicalInputSnapshot(Path.Combine(work, "query-snapshot-" + mode)))
            {
                if (mode == "excluded") snapshot.ExcludeRoot(Path.GetDirectoryName(source));
                using (snapshot.BeginReadScope())
                {
                    var location = asset.GetLocation(AssetLocation.Cache, AssetLocationOption.ForceUpdate);
                    Require((location == AssetLocation.Cache) == (mode == "custom-disappears"), "Cache query changed legacy presence result: " + mode);
                    var before = Metrics(snapshot);
                    long expectedQueries = mode == "custom-disappears" ? 2 : 1;
                    Require(CacheQuery(before, "observations") == expectedQueries && Number(before, "activeReadHandles") == 0,
                        "Cache existence observation added a short-circuited cdata query or pinned an unvalidated source: " + mode);
                    if (mode == "custom-disappears" || mode == "short-circuit") File.Delete(custom);
                    else Source(asset, "cache-query-" + mode, 64);
                    if (mode == "missing-appears" || mode == "custom-disappears")
                        RejectedChange(snapshot, "Changed executed cache candidate was accepted: " + mode);
                    else snapshot.VerifyUnchanged();
                    var metrics = Metrics(snapshot);
                    Require(CacheQuery(metrics, "uniquePaths") == ((mode == "excluded" || mode == "budget") ? 0 : expectedQueries),
                        "Cache query exclusion/budget/short-circuit candidate count changed: " + mode);
                    if (mode == "excluded" || mode == "budget") Require(CacheQuery(metrics, "dropped") == 1, "Cache query fallback did not report incomplete observation");
                    observations.Add(new { id = "network-cache-executed-query-" + mode, metrics });
                }
            }
        }
        tests.Add("network-cache-actual-GetLocation-executed-positive-negative-queries-short-circuit-no-unvalidated-pin-exclusion-budget-and-publication-recheck");
    }
    static void OwnCachePublication()
    {
        foreach (string mode in new[] { "asset", "custom", "conflict", "failed-write" })
        {
            bool previousConflict = mode == "conflict", custom = mode == "custom", failedWrite = mode == "failed-write";
            string name = "own-publication-" + mode;
            var asset = Asset(name, custom); string source = Source(asset, name, 64); File.Delete(source);
            if (failedWrite) { Directory.Delete(Path.GetDirectoryName(source)); File.WriteAllBytes(Path.GetDirectoryName(source), new byte[] { 9 }); }
            typeof(BinaryAsset).GetField("_CacheDirectory", Hidden).SetValue(asset, Path.GetDirectoryName(source));
            using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, name + "-snapshot")))
            using (snapshot.BeginReadScope())
            {
                Require(asset.GetLocation(AssetLocation.Cache, AssetLocationOption.ForceUpdate) == AssetLocation.None, "Own publication did not start from a real cache miss");
                if (previousConflict)
                {
                    Source(asset, name, 64);
                    Require(asset.GetLocation(AssetLocation.Cache, AssetLocationOption.ForceUpdate) == AssetLocation.Cache, "Conflict fixture did not observe an external publication");
                    File.Delete(source);
                }
                asset.Buffer = new AssetBuffer { InstanceData = new byte[64], RelocationData = new byte[0], ImportsData = new byte[0] };
                if (custom)
                {
                    Directory.CreateDirectory(asset.CustomDataOutputDirectory);
                    File.WriteAllBytes(Path.Combine(asset.CustomDataOutputDirectory, asset.Instance.Handle.FileBase + ".cdata"), new byte[] { 6, 3, 1 });
                }
                WithDocument(name, () => Require(asset.Commit() == AssetLocation.Memory, "Real memory commit did not publish its own cache file"));
                Require(File.Exists(source) == !failedWrite && (failedWrite || File.ReadAllBytes(source).SequenceEqual(File.ReadAllBytes(Destination(asset)))),
                    "Own publication changed cache/output bytes or a failed optional cache write changed commit behavior");
                Require(asset.GetLocation(AssetLocation.Cache, AssetLocationOption.ForceUpdate) == (failedWrite ? AssetLocation.None : AssetLocation.Cache),
                    "Own publication result was not visible to the legacy lookup");
                if (custom) Require(File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(source), asset.Instance.Handle.FileBase + ".cdata")).SequenceEqual(new byte[] { 6, 3, 1 }),
                    "Own custom-data cache publication changed bytes");
                if (previousConflict) RejectedChange(snapshot, "Own publication erased an earlier independent conflicting cache observation");
                else snapshot.VerifyUnchanged();
                var metrics = Metrics(snapshot);
                Require(CacheQuery(metrics, "successfulOwnChanges") == (failedWrite ? 0 : custom ? 2 : 1) && CacheQuery(metrics, "conflictingObservations") == (previousConflict ? 1 : 0),
                    "Own cache write did not advance only its expected existence state while retaining prior conflicts");
                Require(Number(metrics, "activeReadHandles") == 0, "Own mutable cache output was pinned as an external input");
                if (!failedWrite) Writable(source); observations.Add(new { id = name, metrics });
            }
        }
        tests.Add("actual-public-Commit-memory-to-network-cache-miss-publication-advances-only-successful-own-write-and-never-clears-prior-external-conflict");
    }
    static void CacheHeaderRecovery()
    {
        foreach (bool invalidCache in new[] { false, true })
        {
            string name = invalidCache ? "invalid-cache-header-delete" : "sealed-cache-header-recovery";
            var first = Asset(name + "-first", false); string source = Source(first, name, 64);
            byte[] expected = File.ReadAllBytes(source);
            if (invalidCache) File.WriteAllBytes(source, expected.Take(32).ToArray());
            using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, name + "-snapshot")))
            using (snapshot.BeginReadScope())
            {
                if (!invalidCache) Require(Copy(first, source), "Header recovery fixture did not seal its first valid cache copy");
                var asset = Asset(name + "-recover", false);
                typeof(BinaryAsset).GetField("_CacheDirectory", Hidden).SetValue(asset, Path.GetDirectoryName(source));
                Directory.CreateDirectory(asset.AssetOutputDirectory); File.WriteAllBytes(Destination(asset), expected.Take(32).ToArray());
                Require(asset.GetLocation(AssetLocation.Output, AssetLocationOption.ForceUpdate) == AssetLocation.Output &&
                    asset.GetLocation(AssetLocation.Cache, AssetLocationOption.ForceUpdate) == AssetLocation.Cache,
                    "Header recovery fixture did not refresh the real output/cache availability after setup");
                bool rejected = false;
                WithDocument(name, () =>
                {
                    try { Require(asset.InstanceFileSize == 64, "Recovered cache header instance size changed"); }
                    catch (BinaryAssetBuilderException error)
                    { if (!invalidCache || error.ErrorCode != ErrorCode.UnexpectedSize) throw; rejected = true; }
                });
                Require(rejected == invalidCache, "Invalid cache header lost its original UnexpectedSize rejection");
                var metrics = Metrics(snapshot);
                if (invalidCache)
                {
                    Require(!File.Exists(source) && Number(metrics, "captures") == 0 && CacheQuery(metrics, "successfulOwnChanges") == 1,
                        "Invalid cache recovery failed to delete its unpinned bad source and record the successful own deletion");
                }
                else
                {
                    Require(File.ReadAllBytes(Destination(asset)).SequenceEqual(expected), "Sealed header recovery changed materialized source bytes");
                    Require(Attempt(metrics, "cacheHeaderReadHits") == 1 && Attempt(metrics, "copyReadHits") == 1 && Number(metrics, "captures") == 1,
                        "Header repair did not reuse the previously sealed source for both header and subsequent copy");
                }
                snapshot.VerifyUnchanged(); observations.Add(new { id = name, metrics });
            }
            if (File.Exists(source)) Writable(source);
        }
        tests.Add("actual-InstanceFileSize-output-header-repair-reuses-sealed-cache-header-and-copy-while-invalid-unsealed-cache-retains-delete-UnexpectedSize-lifecycle");
    }
    static void CacheQueryCancellation()
    {
        bool touch = Settings.Current.AlwaysTouchCache;
        try
        {
            foreach (bool keepalive in new[] { false, true })
            {
                string name = keepalive ? "cancelled-cache-keepalive-query" : "cancelled-cache-publication-query";
                var asset = Asset(name, false); string source = Source(asset, name, 64);
                typeof(BinaryAsset).GetField("_CacheDirectory", Hidden).SetValue(asset, Path.GetDirectoryName(source));
                DateTime oldWrite = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                if (keepalive)
                {
                    Require(Copy(asset, source), "Cancelled keepalive fixture did not establish its real committed output");
                    Require(asset.GetLocation(AssetLocation.Output, AssetLocationOption.ForceUpdate) == AssetLocation.Output,
                        "Cancelled keepalive fixture did not refresh its output availability");
                    File.SetLastWriteTimeUtc(source, oldWrite);
                }
                else
                {
                    File.Delete(source);
                    asset.Buffer = new AssetBuffer { InstanceData = new byte[64], RelocationData = new byte[0], ImportsData = new byte[0] };
                }
                Settings.Current.AlwaysTouchCache = keepalive;
                using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, name + "-snapshot")))
                using (snapshot.BeginReadScope(new CancellationToken(true)))
                {
                    bool cancelled = false;
                    WithDocument(name, () => { try { asset.Commit(); } catch (OperationCanceledException) { cancelled = true; } });
                    Require(cancelled, "Real public Commit swallowed cancellation from an executed optional cache query: " + name);
                    Require(keepalive ? File.GetLastWriteTimeUtc(source) == oldWrite : !File.Exists(source),
                        "Cancelled cache query still performed its keepalive or publication side effect");
                    var metrics = Metrics(snapshot);
                    Require(Number(metrics, "activeReadHandles") == 0 && CacheQuery(metrics, "successfulOwnChanges") == 0,
                        "Cancelled optional cache query retained ownership or advanced the cache write state");
                    observations.Add(new { id = name, metrics });
                }
                if (File.Exists(source)) Writable(source);
            }
        }
        finally { Settings.Current.AlwaysTouchCache = touch; }
        tests.Add("public-Commit-optional-cache-publication-and-output-keepalive-query-cancellation-propagates-without-write-touch-pin-or-own-change");
    }
    static uint Legacy(byte[] bytes)
    {
        uint value = (uint)bytes.Length; var block = new byte[1048576];
        for (int offset = 0; offset < bytes.Length; offset += block.Length)
        {
            int count = Math.Min(block.Length, bytes.Length - offset); System.Buffer.BlockCopy(bytes, offset, block, 0, count);
            value = FastHash.GetHashCode(value, block, count);
        }
        return value;
    }
    static void ArbitraryTeeChunks()
    {
        var begin = typeof(PhysicalInputSnapshot).GetMethod("BeginMaterializationCapture", Hidden);
        var prepare = typeof(PhysicalInputSnapshot).GetMethod("BeginMaterializationContent", Hidden);
        foreach (int length in new[] { 32, 96, 1048576 + 17, 2 * 1048576 + 49 })
        {
            var asset = Asset("tee-chunks-" + length, false); string source = Source(asset, "tee-chunks-" + length, length - 32);
            byte[] expected = File.ReadAllBytes(source); string strong;
            using (var hash = SHA256.Create()) strong = Convert.ToBase64String(hash.ComputeHash(expected));
            using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, "tee-chunks-snapshot-" + length)))
            using (snapshot.BeginReadScope())
            {
                object capture = begin.Invoke(null, new object[0]);
                using ((IDisposable)capture)
                using (var consumed = File.OpenRead(source))
                {
                    object content = prepare.Invoke(null, new object[] { source, consumed });
                    Require(content != null, "Arbitrary chunk source was not eligible for a bounded tee");
                    var append = content.GetType().GetMethod("Append", Hidden);
                    byte[] buffer = new byte[1048576 + 23]; int[] pattern = { 1, 7, 17, 31, 4093, 65521, 1048576 + 3 };
                    int turn = 0; long total = 0;
                    while (total < length)
                    {
                        int read = consumed.Read(buffer, 7, (int)Math.Min(pattern[turn++ % pattern.Length], length - total));
                        Require(read != 0, "Arbitrary chunk source ended early");
                        append.Invoke(content, new object[] { buffer, 7, read }); total += read;
                    }
                    Require(Number(Metrics(snapshot), "captures") == 0 && Number(Metrics(snapshot), "activeDirectoryPins") == 0,
                        "Tee acquired source or parent pins before the complete successful source copy");
                    content.GetType().GetMethod("Complete", Hidden).Invoke(content, new object[] { consumed });
                    PhysicalInputSnapshot.Entry entry;
                    Require(PhysicalInputSnapshot.TryCapture(source, out entry), "Completed tee did not publish a snapshot entry");
                    Require(entry.Length == length && entry.Sha256 == strong && entry.LegacyHash == Legacy(expected),
                        "Arbitrary short chunks changed canonical one-MiB legacy FastHash or SHA256");
                    using (var input = entry.OpenRead()) using (var output = new MemoryStream())
                    { input.CopyTo(output); Require(output.ToArray().SequenceEqual(expected), "Tee immutable memory/disk content differs from actual consumed chunks"); }
                    capture.GetType().GetMethod("Commit", Hidden).Invoke(capture, new object[0]);
                }
                snapshot.VerifyUnchanged(); var metrics = Metrics(snapshot);
                Require(Attempt(metrics, "teeStarted") == 1 && Attempt(metrics, "teeCompleted") == 1 && Attempt(metrics, "teeBytes") == length &&
                    Attempt(metrics, "captureReadsStarted") == 0 && Attempt(metrics, "captureReadBytes") == 0,
                    "Tee performed an extra complete source read or misreported its actual copied bytes");
                observations.Add(new { id = "arbitrary-tee-chunks", length, metrics });
            }
            Writable(source);
        }
        tests.Add("tee-actual-FileStream-arbitrary-one-byte-offset-and-cross-one-MiB-chunks-canonical-FastHash-SHA-memory-disk-no-source-reread-no-early-pins");
    }
    static void ActiveSnapshotCopyTailContracts()
    {
        foreach (int tail in new[] { 0, 1, 15, 16, 17 })
        {
            var first = Asset("tee-tail-enabled-" + tail, false);
            string source = Source(first, "tee-tail-" + tail, 2 * 1048576 + tail - 32);
            string expectedState = null;
            foreach (bool disabled in new[] { false, true })
            {
                var asset = disabled ? Asset("tee-tail-fallback-" + tail, false) : first;
                AppContext.SetSwitch(Disable, disabled);
                using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, "tee-tail-snapshot-" + tail + "-" + disabled)))
                using (snapshot.BeginReadScope())
                {
                    bool copied = Copy(asset, source), expected = tail == 0 || tail >= 16;
                    string destination = Destination(asset), temporary = destination + ".tmp";
                    Require(copied == expected && File.Exists(destination) == expected, "Tee changed the original CopyAsset short-final-block rejection");
                    string state = copied + "|" + File.Exists(temporary) + "|" + (File.Exists(temporary) ? new FileInfo(temporary).Length : -1);
                    if (expectedState == null) expectedState = state; else Require(state == expectedState, "Tee changed the original failed temporary-file state");
                    var metrics = Metrics(snapshot);
                    Require(Number(metrics, "captures") == (!disabled && expected ? 1 : 0), "Short-block failure or disabled mode retained a source entry");
                    Require(Attempt(metrics, "captureReadsStarted") == 0, "Asset copying still reread the original source for capture");
                    if (!expected) { Writable(source); Require(Number(metrics, "activeDirectoryPins") == 0, "Short-block failure retained parent pins"); }
                    else Require(File.ReadAllBytes(destination).SequenceEqual(File.ReadAllBytes(source)), "Tee changed successful copy bytes");
                    snapshot.VerifyUnchanged(); observations.Add(new { id = "active-snapshot-tee-tail", tail, disabled, metrics, state });
                }
            }
        }
        AppContext.SetSwitch(Disable, false);
        var fallback = Asset("tee-parent-budget", false); string fallbackSource = Source(fallback, "tee-parent-budget");
        using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, "tee-parent-budget-snapshot"), 0, 8L * 1048576, 10, 0))
        using (snapshot.BeginReadScope())
        {
            Require(Copy(fallback, fallbackSource), "Parent-pin budget fallback changed successful CopyAsset result");
            var metrics = Metrics(snapshot);
            Require(Number(metrics, "captures") == 0 && Number(metrics, "activeDirectoryPins") == 0 && Number(metrics, "diskBytes") == 0 &&
                Attempt(metrics, "teeCompleted") == 1, "Parent-pin fallback retained provisional source/storage ownership");
            Writable(fallbackSource); snapshot.VerifyUnchanged();
        }
        tests.Add("actual-CopyAsset-active-snapshot-zero-one-fifteen-sixteen-seventeen-tail-contracts-and-post-spool-parent-pin-budget-fallback");
    }
    static void LateFallbackClosesSpool()
    {
        var asset = Asset("late-fallback-spool", false); string source = Source(asset, "late-fallback-spool");
        string scratch = Path.Combine(work, "late-fallback-spool-snapshot");
        using (var snapshot = new PhysicalInputSnapshot(scratch, 0, 8L * 1048576, 10, 0))
        using (snapshot.BeginReadScope())
        {
            var begin = typeof(PhysicalInputSnapshot).GetMethod("BeginMaterializationCapture", Hidden);
            var prepare = typeof(PhysicalInputSnapshot).GetMethod("BeginMaterializationContent", Hidden);
            object capture = begin.Invoke(null, new object[0]);
            using ((IDisposable)capture)
            using (var consumed = File.OpenRead(source))
            {
                object content = prepare.Invoke(null, new object[] { source, consumed });
                var append = content.GetType().GetMethod("Append", Hidden); byte[] buffer = new byte[65521]; int count;
                while ((count = consumed.Read(buffer, 0, buffer.Length)) != 0) append.Invoke(content, new object[] { buffer, 0, count });
                var activeSpool = (FileStream)content.GetType().GetField("storage", Hidden).GetValue(content);
                Require(activeSpool != null && activeSpool.CanRead && activeSpool.CanWrite && activeSpool.Length == consumed.Length,
                    "Disk tee preflight did not establish an active complete private spool");
                content.GetType().GetMethod("Complete", Hidden).Invoke(content, new object[] { consumed });
                Require(!activeSpool.CanRead && Directory.GetFiles(scratch).Length == 0 && content.GetType().GetField("storage", Hidden).GetValue(content) == null,
                    "Late publication fallback kept an unregistered private spool alive until the cdata phase");
                var metrics = Metrics(snapshot);
                Require(Number(metrics, "captures") == 0 && Number(metrics, "activeDirectoryPins") == 0 && Number(metrics, "diskBytes") == 0,
                    "Late fallback retained source/directory/budget ownership");
                capture.GetType().GetMethod("Commit", Hidden).Invoke(capture, new object[0]);
                observations.Add(new { id = "late-fallback-releases-spool-before-following-consumers", metrics });
            }
        }
        Require(!Directory.Exists(scratch), "Late fallback scratch was not removed on snapshot disposal");
        Writable(source);
        tests.Add("late-parent-pin-fallback-immediately-closes-unregistered-disk-spool-before-cdata-can-use-remaining-budget");
    }
    static Settings PublicSettings(string name, string input)
    {
        var config = new XmlDocument(); config.Load(Path.Combine(sdk, "tools/BinaryAssetBuilder.exe.config"));
        var settings = (Settings)new SerializedSectionHandler().Create(null, null, config.SelectSingleNode("/configuration/assetbuilder"));
        settings.DataRoot = Path.GetDirectoryName(input); settings.InputPath = input;
        settings.SchemaPath = Path.Combine(sdk, "Schemas/xsd/CnC3Types.xsd");
        settings.OutputDirectory = Path.Combine(work, "public-generated", name, "output");
        settings.IntermediateOutputDirectory = Path.Combine(work, "public-generated", name, "intermediate");
        settings.SessionCacheDirectory = Path.Combine(work, "public-generated", name, "hashes");
        settings.BuildCacheDirectory = Path.Combine(work, "public-external-cache", name.Split('-')[0]);
        settings.DefaultDataPaths = settings.DefaultArtPaths = settings.DefaultAudioPaths = Path.GetDirectoryName(input);
        settings.TargetPlatform = TargetPlatform.Win32; settings.BuildCache = true; settings.SessionCache = false;
        settings.LinkedStreams = true; settings.OutputAssetReport = settings.OutputStringHashes = false;
        settings.ErrorLevel = 1; settings.UsePrecompiled = settings.StreamHints = settings.StableSort = false;
        Settings.Current = settings; SettingsLoader.PostProcessSettings(settings.DataRoot);
        Directory.CreateDirectory(settings.OutputDirectory); Directory.CreateDirectory(settings.IntermediateOutputDirectory);
        Directory.CreateDirectory(settings.SessionCacheDirectory); Directory.CreateDirectory(settings.BuildCacheDirectory);
        return settings;
    }
    static Dictionary<string, string> Four(string output, string name = "root")
    {
        var result = new Dictionary<string, string>();
        foreach (string path in Directory.GetFiles(output, "*", SearchOption.AllDirectories))
            if (Path.GetFileNameWithoutExtension(path) == name && new[] { ".manifest", ".bin", ".imp", ".relo" }.Contains(Path.GetExtension(path)))
                using (var hash = SHA256.Create()) result.Add(Path.GetExtension(path), Convert.ToBase64String(hash.ComputeHash(File.ReadAllBytes(path))));
        Require(result.Count == 4, "Public Core did not produce exactly four stream outputs"); return result;
    }
    static DocumentProcessor PublicProcessor(BuiltinCompilers plugins, SchemaSet schema)
    {
        HashProvider.InitializeStringHashes(Settings.Current.SessionCacheDirectory);
        var cache = new SessionCache(); cache.InitializeCache(new List<string>());
        var verifiers = new VerifierPluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32);
        verifiers.DefaultPlugin = (IAssetBuilderVerifierPlugin)Activator.CreateInstance(typeof(DocumentProcessor).Assembly.GetType("BinaryAssetBuilder.GameDataVerifier.Plugin", true), true);
        verifiers.DefaultPlugin.Initialize(null, TargetPlatform.Win32);
        return new DocumentProcessor(Settings.Current, plugins.Registry, verifiers) { Cache = cache, SchemaSet = schema };
    }
    static void PublicCoreExternalCache()
    {
        string inputDirectory = Path.Combine(work, "public-input"); Directory.CreateDirectory(inputDirectory);
        string input = Path.Combine(inputDirectory, "root.xml");
        File.WriteAllText(input, "<AssetDeclaration xmlns='uri:ea.com:eala:asset'><AptAptData id='MaterializedBlob' File='payload.bin'/></AssetDeclaration>");
        byte[] payload = new byte[1048576 + 64]; for (int i = 0; i < payload.Length; i++) payload[i] = (byte)(i % 251);
        File.WriteAllBytes(Path.Combine(inputDirectory, "payload.bin"), payload);
        Dictionary<string, string> expected = null;
        PublicSettings("enabled-initialize", input);
        using (var plugins = BuiltinCompilers.Create(TargetPlatform.Win32))
        {
            // Match the public legacy Core contract with cache explicitly
            // enabled for this real native DataBlob route. App /bc copies its
            // own cache into excluded staging and remains a distinct scope.
            plugins.Registry.GetExtendedTypeInformation(InstanceHandle.GetTypeId("AptAptData")).UseBuildCache = true;
            foreach (string mode in new[] { "enabled", "fallback" })
            {
                AppContext.SetSwitch(Disable, mode == "fallback");
                PublicSettings(mode + "-cold", input);
                var schema = new SchemaSet(false); var cold = PublicProcessor(plugins, schema);
                bool success; cold.ProcessDocument(input, true, false, out success); Require(success, "Public Core cold compile failed");
                Require(cold.BuildCacheMisses >= 1 && cold.BuildCacheHits == 0, "Public Core cold stream did not populate its fresh external cache");
                var coldOutputs = Four(Settings.Current.OutputDirectory);
                if (expected == null) expected = coldOutputs;
                Require(expected.All(pair => coldOutputs[pair.Key] == pair.Value), "Cold public Core four outputs differ between capture modes");
                string cacheRoot = Settings.Current.BuildCacheDirectory;
                string[] assets = Directory.GetFiles(cacheRoot, "*.asset", SearchOption.AllDirectories);
                Require(assets.Length == 1, "Real native DataBlob did not populate exactly one legacy cache asset");
                var oldWrite = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                File.SetLastWriteTimeUtc(assets[0], oldWrite);
                Require(File.GetLastWriteTimeUtc(assets[0]) == oldWrite, "Cache timestamp preflight did not establish an old write time");
                using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, mode + "-public-snapshot")))
                {
                    snapshot.ExcludeRoot(Path.Combine(work, "public-generated"));
                    using (snapshot.BeginReadScope())
                    {
                        for (int stream = 0; stream < 3; stream++)
                        {
                            var settings = PublicSettings(mode + "-warm-" + stream, input);
                            var processor = PublicProcessor(plugins, schema);
                            processor.ProcessDocument(input, true, false, out success); Require(success, "Public Core warm stream failed");
                            Require(processor.BuildCacheHits >= 1 && processor.BuildCacheMisses == 0, "Public Core warm stream did not consume the external build cache");
                            Require(File.GetLastWriteTimeUtc(assets[0]) > oldWrite, "Successful cache pin blocked the original TouchCache keepalive timestamp update");
                            var outputs = Four(settings.OutputDirectory);
                            Require(expected.All(pair => outputs[pair.Key] == pair.Value), "Public Core warm/cold capture/fallback four outputs differ");
                        }
                        var metrics = Metrics(snapshot);
                        Require(Attempt(metrics, "copyReadHits") == (mode == "enabled" ? 2 : 0), "Public Core three-stream cache reads did not use the expected sealed/fallback paths");
                        Require(Attempt(metrics, "captureReadsCompleted") == 0 && Attempt(metrics, "teeCompleted") == (mode == "enabled" ? 1 : 0),
                            "Public Core external asset was reread or not spooled exactly once from its original copy stream");
                        Require(Attempt(metrics, "cacheAttributeTouches") == (mode == "enabled" ? 3 : 0) && Attempt(metrics, "cacheAttributeTouchFailures") == 0,
                            "Captured cache keepalive did not use successful metadata-only identity-checked updates");
                        if (mode == "enabled") Blocked(() => File.Delete(assets[0]), "Public Core successfully consumed external cache source was not pinned");
                        else Writable(assets[0]);
                        snapshot.VerifyUnchanged(); observations.Add(new { id = "public-Core-external-build-cache-" + mode, metrics, outputs = expected });
                    }
                }
                Writable(assets[0]);
            }
        }
        AppContext.SetSwitch(Disable, false);
        tests.Add("public-DocumentProcessor-native-DataBlob-external-build-cache-cold-and-three-warm-streams-four-output-fallback-equality-sealed-read-reuse-final-verification-and-generation-release");
    }
    static void PublicCoreMutableLocalOutputs()
    {
        string inputDirectory = Path.Combine(work, "local-input"); Directory.CreateDirectory(inputDirectory);
        string shared = Path.Combine(inputDirectory, "shared.xml");
        File.WriteAllText(shared, "<AssetDeclaration xmlns='uri:ea.com:eala:asset'><AptAptData id='LocalBlob' File='payload.bin'/></AssetDeclaration>");
        File.WriteAllBytes(Path.Combine(inputDirectory, "payload.bin"), new byte[1048576 + 64]);
        var inputs = new List<string>();
        foreach (string name in new[] { "first", "second", "bad", "retry" })
        {
            string path = Path.Combine(inputDirectory, name + ".xml"); inputs.Add(path);
            File.WriteAllText(path, "<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Includes><Include type='all' source='shared.xml'/></Includes></AssetDeclaration>");
        }
        Dictionary<string, string> expected = null;
        PublicSettings("local-initialize", inputs[0]);
        using (var plugins = BuiltinCompilers.Create(TargetPlatform.Win32))
        {
            foreach (bool disabled in new[] { false, true })
            {
                AppContext.SetSwitch(Disable, disabled);
                var settings = PublicSettings(disabled ? "local-fallback" : "local-enabled", inputs[0]); settings.BuildCache = false;
                var processor = PublicProcessor(plugins, new SchemaSet(false));
                using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, disabled ? "local-fallback-snapshot" : "local-enabled-snapshot")))
                using (snapshot.BeginReadScope())
                {
                    // Deliberately register no output exclusions: the Local
                    // source role itself must preserve generated-file repair.
                    bool success; processor.ProcessDocument(inputs[0], true, false, out success); Require(success, "Public Core first Local stream failed");
                    var initial = Four(settings.OutputDirectory, "first");
                    if (expected == null) expected = initial;
                    Require(expected.All(pair => initial[pair.Key] == pair.Value), "Local source baseline differs by capture mode");
                    string[] sources = Directory.GetFiles(settings.IntermediateOutputDirectory, "*.asset", SearchOption.AllDirectories);
                    Require(sources.Length == 1, "Initial Local stream must produce exactly one asset source");
                    string source = sources[0]; byte[] valid = File.ReadAllBytes(source);
                    processor.ProcessDocument(inputs[1], true, false, out success); Require(success, "Public Core second Local stream failed");
                    var second = Four(settings.OutputDirectory, "second");
                    Require(expected.All(pair => second[pair.Key] == pair.Value), "Local source copy changed four outputs");
                    Writable(source); File.Delete(source); File.WriteAllBytes(source, new byte[] { 1 });
                    bool rejected = false;
                    try { processor.ProcessDocument(inputs[2], true, false, out success); }
                    catch (BinaryAssetBuilderException error) { if (error.ErrorCode != ErrorCode.InternalError) throw; rejected = true; }
                    Require(rejected, "Malformed Local source must retain its original failed-copy contract");
                    File.Delete(source); File.WriteAllBytes(source, valid);
                    processor.ProcessDocument(inputs[3], true, false, out success); Require(success, "Local source delete/repair/new-stream retry failed");
                    var retry = Four(settings.OutputDirectory, "retry");
                    Require(expected.All(pair => retry[pair.Key] == pair.Value), "Repaired Local source changed four outputs");
                    var metrics = Metrics(snapshot);
                    Require(Attempt(metrics, "attempts") == 0 && Attempt(metrics, "copyReadHits") == 0,
                        "Mutable Local source was routed through external-cache capture");
                    Require(processor.BuildCacheHits == 0 && processor.BuildCacheMisses == 0, "Local copies were counted as external cache activity");
                    snapshot.VerifyUnchanged(); Writable(source);
                    observations.Add(new { id = disabled ? "public-Core-mutable-Local-fallback" : "public-Core-mutable-Local-enabled", metrics, outputs = retry });
                }
            }
        }
        AppContext.SetSwitch(Disable, false);
        tests.Add("public-DocumentProcessor-same-backend-Local-multistream-without-exclusions-remains-writable-deletable-invalid-copy-rejects-repair-retry-and-four-output-equality");
    }
    public static int Main(string[] args)
    {
        object result; int code = 0; var settings = Settings.Current; bool disabled; AppContext.TryGetSwitch(Disable, out disabled);
        try
        {
            work = Path.GetFullPath(args[0]); sdk = Path.GetFullPath(args[4]); Directory.CreateDirectory(work);
            Settings.Current = new Settings { BuildCache = false, SessionCache = false, LinkedStreams = true, CustomPostfix = "", TargetPlatform = TargetPlatform.Win32 };
            AppContext.SetSwitch(Disable, false);
            SuccessfulSources(); FailedAttemptAndRetry(); ExistingEntryAliasRollback(); ConservativeModes(); ConsumedHandleIdentity(); CachedCommitCancellation(); CapturedCacheKeepalive(); NetworkCacheQueries(); OwnCachePublication(); CacheHeaderRecovery(); CacheQueryCancellation(); ArbitraryTeeChunks(); ActiveSnapshotCopyTailContracts(); LateFallbackClosesSpool(); PublicCoreExternalCache(); PublicCoreMutableLocalOutputs();
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), observations = observations.ToArray(),
                scope = "Actual CopyAsset validated sources, header recovery and network-cache executed existence queries with own publication/deletion; mutable Output/Local and hidden Texture closure remain separate" };
            Console.WriteLine("PASS BinaryMaterialization: " + tests.Count + " groups; " + checks + " checks");
        }
        catch (Exception error)
        {
            code = 1; Console.Error.WriteLine(error); result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null,
                runId = args.Length > 3 ? args[3] : null, checks, tests = tests.ToArray(), observations = observations.ToArray(), error = error.ToString() };
        }
        finally { Settings.Current = settings; AppContext.SetSwitch(Disable, disabled); }
        if (args.Length > 1) File.WriteAllText(args[1], Json.Serialize(result)); return code;
    }
}
