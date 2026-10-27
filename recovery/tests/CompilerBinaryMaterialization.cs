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
            Require(Attempt(metrics, "copyReadHits") == 1 && Attempt(metrics, "captureReadsCompleted") == 2,
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
            Require(Attempt(failed, "captureReadsStarted") == 1 && Attempt(failed, "captureReadsCompleted") == 1 &&
                Attempt(failed, "captureReadBytes") == new FileInfo(source).Length, "Rollback erased actual completed capture-read work");
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
            var capture = typeof(PhysicalInputSnapshot).GetMethod("CaptureMaterializationSource", Hidden);
            bool rejected = false;
            using ((IDisposable)begin.Invoke(null, new object[0]))
            {
                try { capture.Invoke(null, new object[] { replacement, consumed }); }
                catch (TargetInvocationException error) { if (!(error.InnerException is IOException)) throw error.InnerException; rejected = true; }
            }
            Require(rejected, "A replacement path with equal bytes/length was accepted for a different consumed file handle");
            var metrics = Metrics(snapshot);
            Require(Number(metrics, "captures") == 0 && Number(metrics, "activeReadHandles") == 0 && Number(metrics, "activeDirectoryPins") == 0 &&
                Attempt(metrics, "rollbacks") == 1 && Attempt(metrics, "captureReadsCompleted") == 1,
                "Consumed identity mismatch did not roll back its provisional pathname pin");
            Writable(replacement); observations.Add(new { id = "consumed-handle-versus-equal-content-replacement-identity", metrics });
        }
        Writable(original); Writable(replacement);
        tests.Add("post-copy-native-handle-identity-check-rejects-equal-length-equal-content-distinct-file-and-releases-provisional-pins");
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
                            var outputs = Four(settings.OutputDirectory);
                            Require(expected.All(pair => outputs[pair.Key] == pair.Value), "Public Core warm/cold capture/fallback four outputs differ");
                        }
                        var metrics = Metrics(snapshot);
                        Require(Attempt(metrics, "copyReadHits") == (mode == "enabled" ? 2 : 0), "Public Core three-stream cache reads did not use the expected sealed/fallback paths");
                        Require(Attempt(metrics, "captureReadsCompleted") == (mode == "enabled" ? 1 : 0), "Public Core external asset was not captured exactly once in its generation");
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
            SuccessfulSources(); FailedAttemptAndRetry(); ExistingEntryAliasRollback(); ConservativeModes(); ConsumedHandleIdentity(); PublicCoreExternalCache(); PublicCoreMutableLocalOutputs();
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), observations = observations.ToArray(),
                scope = "Actual CopyAsset successful validated sources and failure rollback only; invalid-header/length repair retains separate existing fixture coverage; not all BinaryAsset reads or hidden Texture closure" };
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
