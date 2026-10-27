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

public static class CompilerTransferBuffers
{
    const string Disable = "NewSdk.Compiler.DisableOwnedTransferBuffers";
    const BindingFlags Hidden = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    static string work, sdk;
    static int checks;
    static readonly List<string> tests = new List<string>();
    static readonly List<object> observations = new List<object>();
    static readonly Dictionary<string, string> copySourceHashes = new Dictionary<string, string>(StringComparer.Ordinal);
    static readonly JavaScriptSerializer Json = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
    static void Require(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static IDictionary<string, object> Metrics(DocumentProcessor processor)
    { return Json.Deserialize<Dictionary<string, object>>(Json.Serialize(processor.GetTransferBufferMetrics())); }
    static long Number(IDictionary<string, object> row, string key) { return Convert.ToInt64(row[key]); }
    static object Buffers(DocumentProcessor processor) { return typeof(DocumentProcessor).GetProperty("TransferBuffers", Hidden).GetValue(processor, null); }
    static byte[] Buffer(DocumentProcessor processor, string name) { object owner = Buffers(processor); return (byte[])owner.GetType().GetField(name, Hidden).GetValue(owner); }
    static object Legacy(Type type, string name) { return type.GetField(name, Hidden).GetValue(null); }
    static DocumentProcessor Owner() { return new DocumentProcessor(Settings.Current, new PluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32), null); }
    static AssetDeclarationDocument Document(DocumentProcessor processor, string name)
    {
        var document = new AssetDeclarationDocument(); var type = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object current = Activator.CreateInstance(type, true); type.GetMethod("FromScratch").Invoke(current, new object[0]);
        type.GetField("DocumentProcessor").SetValue(current, processor); type.GetField("SourcePath").SetValue(current, Path.Combine(work, "transfer.xml"));
        type.GetField("SourceDirectory").SetValue(current, work); type.GetField("LogicalSourcePath").SetValue(current, "transfer.xml"); type.GetField("SourcePathFromRoot").SetValue(current, name);
        typeof(AssetDeclarationDocument).GetField("_Current", Hidden).SetValue(document, current); return document;
    }
    static InstanceDeclaration Instance(AssetDeclarationDocument document)
    {
        var xml = new XmlDocument(); xml.LoadXml("<ArmorTemplate id='TransferAsset'/>");
        var instance = new InstanceDeclaration(document) { XmlNode = xml.DocumentElement }; instance.Handle.TypeHash = 17; instance.Handle.InstanceHash = 19; return instance;
    }
    static byte[] Payload(int length)
    { var result = new byte[length]; for (int i = 0; i < length; i++) result[i] = (byte)((i * 29 + 7) % 251); return result; }
    static string Source(InstanceDeclaration instance, string name, byte[] data, byte[] relocations, byte[] imports, bool wrongIdentity = false)
    {
        string path = Path.Combine(work, name + ".asset");
        var header = new AssetHeader { TypeId = instance.Handle.TypeId, InstanceId = instance.Handle.InstanceId,
            TypeHash = instance.Handle.TypeHash, InstanceHash = wrongIdentity ? 20u : instance.Handle.InstanceHash,
            InstanceDataSize = data.Length, RelocationDataSize = relocations.Length, ImportsDataSize = imports.Length };
        using (var stream = File.Create(path)) { header.SaveToStream(stream, false); stream.Write(data, 0, data.Length); stream.Write(relocations, 0, relocations.Length); stream.Write(imports, 0, imports.Length); }
        Require(header.IsValidFileLength(new FileInfo(path).Length), "Direct source fixture has an invalid real asset header length"); return path;
    }
    static bool Copy(BinaryAsset asset, string path)
    {
        try { return (bool)typeof(BinaryAsset).GetMethod("CopyAsset", Hidden).Invoke(asset, new object[] { path, path + ".cdata" }); }
        catch (TargetInvocationException error) { throw error.InnerException; }
    }
    static OutputManager Manager(DocumentProcessor owner, string name)
    { return new OutputManager(owner, null, Path.Combine(work, name, "output"), Path.Combine(work, name, "intermediate"), null, "", new string[0]); }
    static string Hash(string path)
    { using (var algorithm = SHA256.Create()) return Convert.ToBase64String(algorithm.ComputeHash(File.ReadAllBytes(path))); }
    static string CopyCase(DocumentProcessor owner, string label, int tail, bool wrongIdentity = false, bool blockedTemporary = false)
    {
        var document = Document(owner, label); var instance = Instance(document); var manager = Manager(owner, label); var asset = manager.GetBinaryAsset(instance, true);
        int length = 2 * 1048576 + tail;
        // AssetHeader retains its original four padding bytes. Corresponding
        // owned/fallback cases must consume one identical source, rather than
        // independently allocating headers whose unspecified padding may differ.
        string sourceName = "shared-copy-source-" + tail + (wrongIdentity ? "-wrong" : blockedTemporary ? "-blocked" : "-valid");
        string source = Path.Combine(work, sourceName + ".asset");
        if (!File.Exists(source)) Source(instance, sourceName, Payload(length - 32), new byte[0], new byte[0], wrongIdentity);
        Require(new FileInfo(source).Length == length, "Copy boundary fixture total length drifted");
        string sourceHash = Hash(source), firstSourceHash;
        if (copySourceHashes.TryGetValue(sourceName, out firstSourceHash)) Require(sourceHash == firstSourceHash, "Owned/fallback copy inputs do not have identical complete source bytes");
        else copySourceHashes.Add(sourceName, sourceHash);
        using (var input = File.OpenRead(source))
        {
            var header = new AssetHeader(); header.LoadFromStream(input, false);
            Require(header.TypeId == instance.Handle.TypeId && header.InstanceId == instance.Handle.InstanceId && header.TypeHash == instance.Handle.TypeHash &&
                header.InstanceHash == (wrongIdentity ? 20u : instance.Handle.InstanceHash) && header.InstanceDataSize == length - 32 && header.RelocationDataSize == 0 && header.ImportsDataSize == 0,
                "Shared copy source no longer matches the case's complete semantic header identity");
        }
        string destination = Path.Combine(asset.AssetOutputDirectory, asset.AssetFileName), temporary = destination + ".tmp";
        if (blockedTemporary) Directory.CreateDirectory(temporary);
        bool copied = Copy(asset, source); bool expected = !wrongIdentity && !blockedTemporary && (tail == 0 || tail >= 16);
        Require(copied == expected, "Copy changed the original header/tail rejection contract: " + label);
        Require(File.Exists(destination) == expected, "Failed copy left a final asset or successful copy omitted it");
        if (expected) Require(Hash(source) == Hash(destination) && !File.Exists(temporary), "Copy did not retain exact source bytes and final rename");
        string temporaryState = Directory.Exists(temporary) ? "directory" : File.Exists(temporary) ? new FileInfo(temporary).Length + ":" + Hash(temporary) : "missing";
        // Legacy CopyAsset retains its own .tmp after read/header failure. Do
        // not silently change cleanup behavior while changing buffer ownership.
        if (!expected && !blockedTemporary) Require(File.Exists(temporary), "Copy changed its original failed-temporary behavior");
        Require(Hash(source) == sourceHash, "Copy mutated its original input bytes");
        observations.Add(new { id = "copy-case", label, sourceName, sourceSha256 = sourceHash, sourceLength = length, copied, destinationExists = File.Exists(destination), temporaryState });
        return copied + "|" + File.Exists(destination) + "|" + temporaryState;
    }
    static void LinkCase(DocumentProcessor owner, string name, int instanceSize, int relocationSize = 0, int importSize = 0)
    {
        var document = Document(owner, name); var instance = Instance(document); var manager = Manager(owner, name); var asset = manager.GetBinaryAsset(instance, true);
        byte[] data = Payload(instanceSize), relocations = Payload(relocationSize), imports = Payload(importSize);
        string source = Source(instance, name + "-source", data, relocations, imports);
        Require(Copy(asset, source), "Link boundary asset could not be copied"); asset.GetLocation(AssetLocation.Output, AssetLocationOption.ForceUpdate); document.OutputInstances.Add(instance);
        manager.LinkStream(document);
        foreach (var row in new[] { new { extension = ".bin", bytes = data }, new { extension = ".relo", bytes = relocations }, new { extension = ".imp", bytes = imports } })
        {
            byte[] actual = File.ReadAllBytes(manager.OutputDirectory + row.extension);
            Require(actual.Length == row.bytes.Length + 4 && actual.Skip(4).SequenceEqual(row.bytes), "Linked header/chunk/tail bytes changed for " + row.extension);
        }
    }
    static void OwnershipAndBoundaries()
    {
        Settings.Current = new Settings { BuildCache = false, SessionCache = false, LinkedStreams = true, CustomPostfix = "", TargetPlatform = TargetPlatform.Win32 };
        Require(Legacy(typeof(BinaryAsset), "_CopyBuffer") == null && Legacy(typeof(OutputManager), "_LinkedBuffer") == null, "Legacy transfer arrays allocated before consumption");
        AppContext.SetSwitch(Disable, false); var first = Owner(); var second = Owner(); var unused = Owner(); Manager(unused, "unused-a"); Manager(unused, "unused-b");
        Require(Number(Metrics(unused), "allocatedBytes") == 0 && Number(Metrics(unused), "copyRequests") == 0 && Number(Metrics(unused), "linkRequests") == 0, "Unused processor/managers allocated transfer storage");
        var outcomes = new List<string>();
        foreach (int tail in new[] { 0, 1, 15, 16, 17 }) outcomes.Add(CopyCase(first, "owned-tail-" + tail, tail));
        outcomes.Add(CopyCase(first, "owned-wrong-header", 16, true)); outcomes.Add(CopyCase(first, "owned-blocked-temp", 16, false, true));
        foreach (int length in new[] { 0, 1, 65535, 65536, 65537, 131079 })
        {
            LinkCase(first, "owned-link-" + length, length);
            if (length == 0) Require(Number(Metrics(first), "linkRequests") == 0 && Number(Metrics(first), "linkAllocations") == 0, "Zero-length link body allocated/consumed a link buffer");
        }
        LinkCase(second, "second-owner", 1048576 + 17, 65536 + 3, 65536 + 5);
        var metrics = Metrics(first); Require(Number(metrics, "copyAllocations") == 1 && Number(metrics, "linkAllocations") == 1 && Number(metrics, "allocatedBytes") == 1048576 + 65536, "Multiple managers did not reuse exactly one lazily owned buffer pair");
        Require(!Object.ReferenceEquals(Buffer(first, "_Copy"), Buffer(second, "_Copy")) && !Object.ReferenceEquals(Buffer(first, "_Link"), Buffer(second, "_Link")), "Two processors alias mutable transfer arrays");
        Require(Legacy(typeof(BinaryAsset), "_CopyBuffer") == null && Legacy(typeof(OutputManager), "_LinkedBuffer") == null, "Owned consumers allocated legacy static arrays");
        AppContext.SetSwitch(Disable, true); var legacy = Owner(); var otherLegacy = Owner();
        var expected = new List<string>(); foreach (int tail in new[] { 0, 1, 15, 16, 17 }) expected.Add(CopyCase(legacy, "legacy-tail-" + tail, tail));
        expected.Add(CopyCase(legacy, "legacy-wrong-header", 16, true)); expected.Add(CopyCase(legacy, "legacy-blocked-temp", 16, false, true));
        Require(outcomes.SequenceEqual(expected), "Owned CopyAsset error/final/temporary states differ from legacy static fallback: " + Json.Serialize(new { owned = outcomes.ToArray(), fallback = expected.ToArray(), sourceSha256 = copySourceHashes }));
        LinkCase(legacy, "legacy-link", 1048576 + 17, 65536 + 3, 65536 + 5); object sharedCopy = Legacy(typeof(BinaryAsset), "_CopyBuffer"), sharedLink = Legacy(typeof(OutputManager), "_LinkedBuffer");
        LinkCase(otherLegacy, "other-legacy-link", 65537);
        Require(Object.ReferenceEquals(sharedCopy, Legacy(typeof(BinaryAsset), "_CopyBuffer")) && Object.ReferenceEquals(sharedLink, Legacy(typeof(OutputManager), "_LinkedBuffer")), "Fallback processors stopped sharing the original static transfer arrays");
        Require(Number(Metrics(legacy), "allocatedBytes") == 0 && Number(Metrics(legacy), "legacyCopyRequests") > 0 && Number(Metrics(legacy), "legacyLinkRequests") > 0, "Fallback transfer requests were counted as private allocations");
        // Mode is captured when the processor is created, including failure and
        // subsequent successful consumption; later switches do not redirect it.
        CopyCase(first, "owned-after-fallback-switch-and-failures", 17);
        Require(Number(Metrics(first), "copyAllocations") == 1 && Number(Metrics(first), "legacyCopyRequests") == 0, "Existing owner changed mode or leaked allocation after failure");
        observations.Add(new { id = "real-copy-link-boundaries-and-owner-lifetime", owned = Metrics(first), otherOwned = Metrics(second), unused = Metrics(unused), fallback = Metrics(legacy), outcomes = outcomes.ToArray() });
        AppContext.SetSwitch(Disable, false);
        tests.Add("actual-CopyAsset-and-LinkStream-lazy-per-processor-ownership-no-alias-reuse-captured-fallback-mode-header-tail-and-temporary-contracts");
    }
    static IDictionary<string, object> Inputs(PhysicalInputSnapshot snapshot)
    { return Json.Deserialize<Dictionary<string, object>>(Json.Serialize(snapshot.GetMetrics())); }
    static void Writable(string path) { using (var stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { } }
    static void Blocked(string path)
    {
        bool blocked = false;
        try { Writable(path); } catch (IOException) { blocked = true; } catch (UnauthorizedAccessException) { blocked = true; }
        Require(blocked, "External copy input was not pinned: " + path);
    }
    static void SnapshotCopyInputs()
    {
        Settings.Current = new Settings { BuildCache = false, SessionCache = false, LinkedStreams = true, CustomPostfix = "", TargetPlatform = TargetPlatform.Win32 };
        AppContext.SetSwitch(Disable, false); var owner = Owner();
        var template = Instance(Document(owner, "snapshot-copy-source"));
        string source = Source(template, "snapshot-copy-source", Payload(64), new byte[0], new byte[0]), custom = source + ".cdata";
        File.WriteAllBytes(custom, Payload(129)); File.SetLastWriteTimeUtc(custom, new DateTime(2020, 1, 2, 3, 4, 6, DateTimeKind.Utc));
        string expectedAsset = Hash(source), expectedCustom = Hash(custom);
        foreach (string mode in new[] { "memory", "disk", "zero-budget", "excluded-output", "legacy-no-scope" })
        {
            var instance = Instance(Document(owner, "snapshot-copy-" + mode)); instance.HasCustomData = true;
            var asset = Manager(owner, "snapshot-copy-" + mode).GetBinaryAsset(instance, true);
            PhysicalInputSnapshot snapshot = mode == "legacy-no-scope" ? null : new PhysicalInputSnapshot(Path.Combine(work, "snapshot-" + mode),
                mode == "disk" ? 0 : 1024, 1024, mode == "zero-budget" ? 0 : 8, 64);
            if (mode == "excluded-output") snapshot.ExcludeRoot(work);
            bool captured = mode == "memory" || mode == "disk";
            try
            {
                using (snapshot == null ? null : snapshot.BeginReadScope())
                {
                    Require(Copy(asset, source), "Snapshot-backed .asset/.cdata pair failed: " + mode);
                    string output = Path.Combine(asset.AssetOutputDirectory, asset.AssetFileName), outputCustom = Path.Combine(asset.CustomDataOutputDirectory, instance.Handle.FileBase + ".cdata");
                    Require(Hash(output) == expectedAsset && Hash(outputCustom) == expectedCustom && File.GetLastWriteTimeUtc(outputCustom) == File.GetLastWriteTimeUtc(custom),
                        "Snapshot pair changed bytes or File.Copy custom-data timestamp metadata: " + mode);
                    // Repeat against the exact same source owner to exercise two
                    // independent sealed cursors and original cdata path pins.
                    Require(Copy(asset, source) && Hash(output) == expectedAsset && Hash(outputCustom) == expectedCustom, "Repeated snapshot copy changed output");
                    if (captured) { Blocked(source); Blocked(custom); } else { Writable(source); Writable(custom); }
                    if (snapshot != null)
                    {
                        var metrics = Inputs(snapshot);
                        Require(Number(metrics, "captures") == (captured ? 2 : 0) && Number(metrics, "hits") == (captured ? 2 : 0), "Copy pair did not share exactly two positive inputs: " + mode);
                        if (mode == "disk") Require(Number(metrics, "diskBytes") == new FileInfo(source).Length + new FileInfo(custom).Length, "Disk snapshot omitted one copy input");
                        snapshot.VerifyUnchanged();
                        observations.Add(new { id = "copy-pair-positive-inputs", mode, metrics = Inputs(snapshot), assetSha256 = expectedAsset, cdataSha256 = expectedCustom });
                    }
                }
            }
            finally { if (snapshot != null) snapshot.Dispose(); }
            Writable(source); Writable(custom);
            if (snapshot != null) Require(Number(Inputs(snapshot), "activeReadHandles") == 0, "Copy pair retained pins after snapshot disposal");
        }
        foreach (string mode in new[] { "memory", "disk" })
        {
            using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, "snapshot-copy-errors-" + mode), mode == "disk" ? 0 : 16777216, 16777216, 16, 64))
            using (snapshot.BeginReadScope())
            {
                foreach (int tail in new[] { 0, 1, 15, 16, 17 }) CopyCase(owner, "snapshot-" + mode + "-tail-" + tail, tail);
                CopyCase(owner, "snapshot-" + mode + "-wrong-header", 16, true);
                CopyCase(owner, "snapshot-" + mode + "-blocked-temp", 16, false, true);
                snapshot.VerifyUnchanged();
                observations.Add(new { id = "sealed-copy-header-tail-and-temp-contract", mode, metrics = Inputs(snapshot) });
            }
        }
        foreach (bool sealedInputs in new[] { false, true })
        {
            string label = "missing-copy-cdata-" + sealedInputs;
            var instance = Instance(Document(owner, label)); instance.HasCustomData = true;
            var asset = Manager(owner, label).GetBinaryAsset(instance, true);
            string missingSource = Source(instance, label, Payload(64), new byte[0], new byte[0]);
            PhysicalInputSnapshot snapshot = sealedInputs ? new PhysicalInputSnapshot(Path.Combine(work, label + "-snapshot")) : null;
            try
            {
                using (snapshot == null ? null : snapshot.BeginReadScope())
                {
                    Require(!Copy(asset, missingSource) && !File.Exists(Path.Combine(asset.AssetOutputDirectory, asset.AssetFileName)) &&
                        !File.Exists(Path.Combine(asset.AssetOutputDirectory, asset.AssetFileName) + ".tmp"), "Missing .cdata changed original false/final/temp contract");
                    if (snapshot != null) Require(Number(Inputs(snapshot), "captures") == 1, "Missing .cdata must not become a captured positive input");
                }
            }
            finally { if (snapshot != null) snapshot.Dispose(); }
            Writable(missingSource);
        }
        // Cache availability must preserve each original short circuit, while
        // retaining the real .cdata negative result for final verification.
        foreach (string condition in new[] { "missing-custom", "missing-asset", "no-custom-required" })
        {
            var instance = Instance(Document(owner, "candidate-" + condition)); instance.HasCustomData = condition != "no-custom-required";
            var asset = Manager(owner, "candidate-" + condition).GetBinaryAsset(instance, true);
            string cache = Path.Combine(work, "external-cache-" + condition); Directory.CreateDirectory(cache);
            string cacheAsset = Path.Combine(cache, asset.AssetFileName), cacheCustom = Path.Combine(cache, instance.Handle.FileBase + ".cdata");
            if (condition != "missing-asset") File.Copy(source, cacheAsset);
            typeof(BinaryAsset).GetField("_CacheDirectory", Hidden).SetValue(asset, cache);
            using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, "candidate-snapshot-" + condition)))
            using (snapshot.BeginReadScope())
            {
                var location = asset.GetLocation(AssetLocation.Cache, AssetLocationOption.ForceUpdate);
                Require((location == AssetLocation.Cache) == (condition == "no-custom-required"), "Cache presence selection changed: " + condition);
                var candidates = (IDictionary<string, object>)Inputs(snapshot)["resolverCandidates"];
                Require(Number(candidates, "observations") == (condition == "missing-custom" ? 2 : 1), "Cache availability added/omitted original short-circuit queries");
                if (condition == "missing-custom")
                {
                    File.WriteAllBytes(cacheCustom, Payload(129)); bool rejected = false;
                    try { snapshot.VerifyUnchanged(); } catch (IOException) { rejected = true; }
                    Require(rejected, "A missing cache .cdata appearing after availability query was accepted");
                }
                else snapshot.VerifyUnchanged();
                observations.Add(new { id = "cache-pair-availability-candidates", condition, metrics = Inputs(snapshot) });
            }
        }
        // Pre-cancel after availability is warm, so the actual CopyAsset input
        // capture (not an earlier candidate query) must propagate the same token
        // through CommitFromCache without the legacy retry/sleep loop.
        var cancelledInstance = Instance(Document(owner, "cancelled-cache-pair")); cancelledInstance.HasCustomData = true;
        var cancelledAsset = Manager(owner, "cancelled-cache-pair").GetBinaryAsset(cancelledInstance, true);
        string cancelledCache = Path.Combine(work, "cancelled-external-cache"); Directory.CreateDirectory(cancelledCache);
        File.Copy(source, Path.Combine(cancelledCache, cancelledAsset.AssetFileName));
        File.Copy(custom, Path.Combine(cancelledCache, cancelledInstance.Handle.FileBase + ".cdata"));
        typeof(BinaryAsset).GetField("_CacheDirectory", Hidden).SetValue(cancelledAsset, cancelledCache);
        Require(cancelledAsset.GetLocation(AssetLocation.Cache, AssetLocationOption.ForceUpdate) == AssetLocation.Cache, "Cancellation fixture did not warm cache availability");
        using (var cancellation = new CancellationTokenSource())
        using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, "cancelled-copy-snapshot")))
        {
            cancellation.Cancel(); bool rejected = false; var elapsed = System.Diagnostics.Stopwatch.StartNew();
            using (snapshot.BeginReadScope(cancellation.Token))
                try { typeof(BinaryAsset).GetMethod("CommitFromCache", Hidden).Invoke(cancelledAsset, new object[0]); }
                catch (TargetInvocationException error)
                {
                    var cancelled = error.InnerException as OperationCanceledException;
                    rejected = cancelled != null && cancelled.CancellationToken == cancellation.Token;
                    if (cancelled == null) throw error.InnerException;
                }
            Require(rejected && elapsed.ElapsedMilliseconds < 4000 && Number(Inputs(snapshot), "captures") == 0,
                "Copy/cache retry swallowed or replaced the original snapshot cancellation token");
        }
        tests.Add("external-BinaryAsset-asset-cdata-materialization-pair-sealed-memory-disk-original-copy-metadata-repeated-inputs-budgets-generated-exclusion-and-pin-release");
        tests.Add("real-cache-asset-cdata-existence-short-circuits-final-negative-candidate-refusal-and-original-cancellation-token-without-cache-retry");
    }
    static Settings Configure(string name)
    {
        var config = new XmlDocument(); config.Load(Path.Combine(sdk, "tools/BinaryAssetBuilder.exe.config"));
        var settings = (Settings)new SerializedSectionHandler().Create(null, null, config.SelectSingleNode("/configuration/assetbuilder"));
        settings.DataRoot = work; settings.SchemaPath = Path.Combine(sdk, "Schemas/xsd/CnC3Types.xsd"); settings.InputPath = Path.Combine(work, "first.xml");
        settings.OutputDirectory = Path.Combine(work, name, "output"); settings.IntermediateOutputDirectory = Path.Combine(work, name, "intermediate"); settings.SessionCacheDirectory = Path.Combine(work, name, "cache");
        settings.DefaultDataPaths = work; settings.DefaultArtPaths = work; settings.DefaultAudioPaths = work; settings.TargetPlatform = TargetPlatform.Win32;
        settings.BuildCache = false; settings.SessionCache = false; settings.LinkedStreams = true; settings.OutputAssetReport = false; settings.OutputStringHashes = false;
        settings.ErrorLevel = 1; settings.UsePrecompiled = false; settings.StreamHints = false; settings.StableSort = false; Settings.Current = settings; SettingsLoader.PostProcessSettings(work);
        Directory.CreateDirectory(settings.OutputDirectory); Directory.CreateDirectory(settings.IntermediateOutputDirectory); Directory.CreateDirectory(settings.SessionCacheDirectory); return settings;
    }
    static Dictionary<string, string> Four(string output, string name)
    {
        var result = new Dictionary<string, string>(); foreach (string path in Directory.GetFiles(output, "*", SearchOption.AllDirectories))
            if (Path.GetFileNameWithoutExtension(path) == name && new[] { ".manifest", ".bin", ".imp", ".relo" }.Contains(Path.GetExtension(path))) result.Add(Path.GetExtension(path), Hash(path));
        Require(result.Count == 4, "Real stream did not produce four outputs"); return result;
    }
    static void Run(DocumentProcessor processor, string path)
    { bool success; processor.ProcessDocument(path, true, false, out success); Require(success, "Real Core did not succeed"); }
    static DocumentProcessor Processor(BuiltinCompilers plugins, SchemaSet schemas, string cachePath)
    {
        HashProvider.InitializeStringHashes(Settings.Current.SessionCacheDirectory); var cache = new SessionCache(); cache.LoadCache(cachePath); cache.InitializeCache(new List<string>());
        var verifiers = new VerifierPluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32);
        verifiers.DefaultPlugin = (IAssetBuilderVerifierPlugin)Activator.CreateInstance(typeof(DocumentProcessor).Assembly.GetType("BinaryAssetBuilder.GameDataVerifier.Plugin", true), true); verifiers.DefaultPlugin.Initialize(null, TargetPlatform.Win32);
        return new DocumentProcessor(Settings.Current, plugins.Registry, verifiers) { Cache = cache, SchemaSet = schemas };
    }
    static void RealCore()
    {
        Configure("initialize"); string shared = Path.Combine(work, "shared.xml");
        File.WriteAllBytes(Path.Combine(work, "payload.bin"), Payload(1048576 + 8192 + 37));
        File.WriteAllText(shared, "<AssetDeclaration xmlns='uri:ea.com:eala:asset'><AptAptData id='TransferBlob' File='payload.bin'/></AssetDeclaration>");
        foreach (string name in new[] { "first", "second", "third" }) File.WriteAllText(Path.Combine(work, name + ".xml"), "<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Includes><Include type='all' source='shared.xml'/></Includes></AssetDeclaration>");
        string cancelledInput = Path.Combine(work, "cancelled.xml"); File.WriteAllText(cancelledInput, "<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Includes><Include type='all' source='shared.xml'/></Includes></AssetDeclaration>");
        string bad = Path.Combine(work, "bad.xml"); File.WriteAllText(bad, "<AssetDeclaration xmlns='uri:ea.com:eala:asset'><ArmorTemplate id='Rejected' inheritFrom='MissingParent'/></AssetDeclaration>");
        Dictionary<string, string> expected = null;
        using (var plugins = BuiltinCompilers.Create(TargetPlatform.Win32))
        {
            var schemas = new SchemaSet(false);
            foreach (bool fallback in new[] { false, true })
            {
                AppContext.SetSwitch(Disable, fallback); var settings = Configure(fallback ? "real-legacy" : "real-owned"); string cachePath = Path.Combine(settings.SessionCacheDirectory, "session.xml");
                var processor = Processor(plugins, schemas, cachePath); Run(processor, settings.InputPath); Run(processor, Path.Combine(work, "second.xml")); Run(processor, Path.Combine(work, "third.xml"));
                var outputs = Four(settings.OutputDirectory, "third"); var metrics = Metrics(processor);
                Require(Number(metrics, "copyRequests") >= 2 && Number(metrics, "linkRequests") > 0, "Actual native DataBlob multi-stream build did not exercise transfer consumers");
                if (expected == null) expected = outputs; else Require(expected.All(pair => outputs[pair.Key] == pair.Value), "Owned and fallback real DataBlob four outputs differ");
                processor.Cache.SaveCache(false); var restored = Processor(plugins, schemas, cachePath); Run(restored, Path.Combine(work, "third.xml"));
                Require(expected.All(pair => Four(settings.OutputDirectory, "third")[pair.Key] == pair.Value), "Persisted session restore changed four output bytes");
                bool rejected = false; try { Run(processor, bad); } catch (BinaryAssetBuilderException error) { rejected = error.ErrorCode == ErrorCode.InheritFromError; }
                Require(rejected, "Real failure contract changed");
                byte[] copyBefore = Buffer(processor, "_Copy"), linkBefore = Buffer(processor, "_Link"); bool cancelled = false;
                using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, fallback ? "cancel-legacy-snapshot" : "cancel-owned-snapshot")))
                using (snapshot.BeginReadScope(new CancellationToken(true)))
                {
                    try { Run(processor, cancelledInput); } catch (OperationCanceledException) { cancelled = true; }
                }
                Require(cancelled && Object.ReferenceEquals(copyBefore, Buffer(processor, "_Copy")) && Object.ReferenceEquals(linkBefore, Buffer(processor, "_Link")), "Cancelled Core attempt leaked/replaced its existing owner buffers");
                Run(processor, Path.Combine(work, "second.xml"));
                Require(Number(Metrics(processor), fallback ? "copyAllocations" : "linkAllocations") == (fallback ? 0 : 1), "Failure/retry changed owner allocation mode");
                observations.Add(new { id = fallback ? "real-Core-fallback" : "real-Core-owned", metrics = Metrics(processor), sessionRestored = Metrics(restored), outputs });
            }
        }
        AppContext.SetSwitch(Disable, false);
        tests.Add("real-native-DataBlob-over-one-MiB-three-stream-local-copies-linking-persisted-session-and-failure-cancellation-retry-preserve-four-outputs-with-static-fallback");
    }
    public static int Main(string[] args)
    {
        object result; int code = 0; var settings = Settings.Current; bool disabled; AppContext.TryGetSwitch(Disable, out disabled);
        try
        {
            work = Path.GetFullPath(args[0]); sdk = Path.GetFullPath(args[4]); Directory.CreateDirectory(work);
            OwnershipAndBoundaries(); SnapshotCopyInputs(); RealCore();
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), observations = observations.ToArray(), scope = "Serial per-DocumentProcessor copy/link buffers and external BinaryAsset copy-pair input capture/cache candidates; generated roots and mutable invalid-header recovery remain legacy, Texture closure and full BinaryAsset closure are not established; no parallel execution" };
            Console.WriteLine("PASS TransferBuffers: " + tests.Count + " groups; " + checks + " checks");
        }
        catch (Exception error) { code = 1; Console.Error.WriteLine(error); result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null, runId = args.Length > 3 ? args[3] : null, checks, tests = tests.ToArray(), observations = observations.ToArray(), error = error.ToString() }; }
        finally { Settings.Current = settings; AppContext.SetSwitch(Disable, disabled); }
        if (args.Length > 1) File.WriteAllText(args[1], Json.Serialize(result)); return code;
    }
}
