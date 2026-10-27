using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Utility;

// Actual CopyAsset sources, not manually substituted streams or fabricated
// output bytes. The full CLI/SDK matrix remains a separate acceptance gate.
public static class CompilerBinaryMaterialization
{
    const BindingFlags Hidden = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    const string Disable = "NewSdk.Compiler.DisableBinaryMaterializationCapture";
    static string work;
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
    public static int Main(string[] args)
    {
        object result; int code = 0; var settings = Settings.Current; bool disabled; AppContext.TryGetSwitch(Disable, out disabled);
        try
        {
            work = Path.GetFullPath(args[0]); Directory.CreateDirectory(work);
            Settings.Current = new Settings { BuildCache = false, SessionCache = false, LinkedStreams = true, CustomPostfix = "", TargetPlatform = TargetPlatform.Win32 };
            AppContext.SetSwitch(Disable, false);
            SuccessfulSources(); FailedAttemptAndRetry(); ExistingEntryAliasRollback(); ConservativeModes();
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
