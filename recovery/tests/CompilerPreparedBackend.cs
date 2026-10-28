using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using NewModCompiler.Application;

// Executed by the independent host linked from the actual production modules.
// Reflection keeps the prepared representation an internal application detail.
public static class CompilerPreparedBackend
{
    static int checks;
    static readonly List<object> lifecycleEvidence = new List<object>();
    static readonly Type contract = typeof(CoreStreamCompiler).Assembly.GetType("NewModCompiler.Application.PreparedBackend", true);
    static void Require(bool condition, string message)
    {
        checks++;
        if (!condition) throw new InvalidOperationException(message);
    }
    static object Prepare(InstanceDeclaration declaration, IAssetBuilderPlugin plugin)
    {
        var arguments = new object[] { declaration, plugin, null };
        bool accepted = (bool)contract.GetMethod("TryPrepare", BindingFlags.Public | BindingFlags.Static).Invoke(null, arguments);
        return accepted ? arguments[2] : null;
    }
    static T Value<T>(object prepared, string property)
    {
        return (T)contract.GetProperty(property, BindingFlags.Public | BindingFlags.Instance).GetValue(prepared, null);
    }
    static string Identity(object prepared)
    {
        using (var bytes = new MemoryStream())
        using (var writer = new BinaryWriter(bytes, Encoding.UTF8))
        using (var sha = SHA256.Create())
        {
            contract.GetMethod("WriteIdentity").Invoke(prepared, new object[] { writer });
            writer.Flush();
            return BitConverter.ToString(sha.ComputeHash(bytes.ToArray()));
        }
    }
    static AssetBuffer Execute(object prepared)
    {
        return (AssetBuffer)contract.GetMethod("Execute").Invoke(prepared, null);
    }
    static InstanceDeclaration Declaration(string type, string id, XmlNamespaceManager namespaces)
    {
        var xml = new XmlDocument { XmlResolver = null };
        var root = xml.CreateElement(type, "uri:ea.com:eala:asset");
        root.SetAttribute("id", id); xml.AppendChild(root);
        var declaration = new InstanceDeclaration(AssetDeclarationDocument.CreateBackendView(namespaces));
        declaration.XmlNode = root;
        return declaration;
    }
    static XmlNamespaceManager Namespaces()
    {
        var namespaces = new XmlNamespaceManager(new NameTable());
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        return namespaces;
    }
    static void Metadata(InstanceDeclaration instance, IAssetBuilderPlugin plugin)
    {
        var information = plugin.GetExtendedTypeInformation(instance.Handle.TypeId);
        instance.Handle.TypeHash = information.TypeHash;
        instance.ProcessingHash = information.ProcessingHash;
        instance.HasCustomData = information.HasCustomData;
    }
    static bool Contains(byte[] data, byte[] expected)
    {
        for (int i = 0; i <= data.Length - expected.Length; ++i)
        {
            int match = 0;
            while (match < expected.Length && data[i + match] == expected[match]) match++;
            if (match == expected.Length) return true;
        }
        return false;
    }
    static void Denied(Action action, string operation)
    {
        bool denied = false;
        try { action(); }
        catch (IOException) { denied = true; }
        catch (UnauthorizedAccessException) { denied = true; }
        Require(denied, "Prepared input must reject " + operation + " while its content lease is held");
    }
    static void AptLeaseAndFreeze(string work)
    {
        string inputDirectory = Path.Combine(work, "prepared-input-lease");
        Directory.CreateDirectory(inputDirectory);
        string path = Path.Combine(inputDirectory, "prepared-blob.bin");
        string alternate = Path.Combine(work, "alternate-blob.bin");
        byte[] first = Encoding.ASCII.GetBytes("ABCDEFGHIJKLMNOP");
        byte[] second = Encoding.ASCII.GetBytes("QRSTUVWXYZabcdef");
        File.WriteAllBytes(path, first); File.WriteAllBytes(alternate, second);
        var plugin = new Recovered.XmlPriority.Plugin();
        plugin.Initialize(null, TargetPlatform.Win32);
        var namespaces = Namespaces();
        var instance = Declaration("AptAptData", "PreparedApt", namespaces);
        ((XmlElement)instance.Node).SetAttribute("File", path); Metadata(instance, plugin);
        string before;
        object prepared = Prepare(instance, plugin);
        Require(prepared != null && Value<bool>(prepared, "CanCache"), "Existing APT blob input must be cacheable");
        try
        {
            before = Identity(prepared);
            Require(!Value<bool>(prepared, "CanRunParallel"), "XML parallelism must remain separately gated");
            Denied(delegate { using (var writer = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite)) { } }, "a writer");
            Denied(delegate { File.Delete(path); }, "deletion");
            Denied(delegate { File.Move(path, path + ".moved"); }, "rename");
            Denied(delegate { Directory.Move(inputDirectory, inputDirectory + ".moved"); }, "ancestor directory rename");
            // Native fopen must still be allowed to read while the owner holds
            // FileShare.Read. Neither a new XML value nor a namespace mutation
            // may alter the actual private representation being executed.
            ((XmlElement)instance.Node).SetAttribute("File", alternate);
            namespaces.AddNamespace("ea", "urn:changed-after-prepare");
            var frozen = Value<InstanceDeclaration>(prepared, "Declaration");
            Require(frozen.Node.Attributes["File"].Value == path, "Prepared XML must be independent of owner mutation");
            Require(frozen.Document.NamespaceManager.LookupNamespace("ea") == "uri:ea.com:eala:asset", "Namespace table must be private");
            Require(!Object.ReferenceEquals(frozen.Node.OwnerDocument, instance.Node.OwnerDocument), "XML document and name table must be private");
            var result = Execute(prepared);
            Require(result != null && Contains(result.InstanceData, first) && !Contains(result.InstanceData, second), "Native APT consumption must match the held and hashed version");
            Require(before == Identity(prepared), "Owner mutations must not change prepared identity");
        }
        finally { ((IDisposable)prepared).Dispose(); }
        ((XmlElement)instance.Node).SetAttribute("File", path);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        DateTime timestamp = File.GetLastWriteTimeUtc(path);
        File.WriteAllBytes(path, second); File.SetLastWriteTimeUtc(path, timestamp);
        Require(new FileInfo(path).Length == first.Length && File.GetLastWriteTimeUtc(path) == timestamp, "Equal-length/mtime fixture must be exact");
        prepared = Prepare(instance, plugin);
        Require(prepared != null, "A released input lease must allow a new version");
        try { Require(before != Identity(prepared), "Equal-length equal-mtime external byte mutation must change prepared identity"); }
        finally { ((IDisposable)prepared).Dispose(); }
        File.Delete(path);
        Require(Prepare(instance, plugin) == null, "A missing APT read must use the compatibility path rather than an incomplete identity");
    }
    static void W3xBudget()
    {
        var plugin = new Recovered.Compiler.W3x.NativePlugin();
        plugin.Initialize(null, TargetPlatform.Win32);
        var instance = Declaration("W3DAnimation", "PreparedAnimation", Namespaces());
        Metadata(instance, plugin);
        ((XmlElement)instance.Node).SetAttribute("NumFrames", "65536");
        var channels = instance.Node.OwnerDocument.CreateElement("Channels", "uri:ea.com:eala:asset");
        var channel = instance.Node.OwnerDocument.CreateElement("W3DAnimationChannelScalar", "uri:ea.com:eala:asset");
        channels.AppendChild(channel); instance.Node.AppendChild(channels);
        object prepared = Prepare(instance, plugin);
        Require(prepared != null, "W3X must have a certified preparation contract");
        try
        {
            Require(Value<bool>(prepared, "CanRunParallel"), "A bounded 65536-frame single channel animation may run in the W3X pool");
            Require(Value<long>(prepared, "PreparedBytes") >= 65536L * 256L, "Declared frame expansion must be included in the lifecycle budget");
        }
        finally { ((IDisposable)prepared).Dispose(); }
        ((XmlElement)instance.Node).SetAttribute("NumFrames", "4294967295");
        prepared = Prepare(instance, plugin);
        try { Require(!Value<bool>(prepared, "CanRunParallel"), "A tiny XML declaring huge expansion must stay on the owner path"); }
        finally { ((IDisposable)prepared).Dispose(); }
    }
    static void SchemaDefaults()
    {
        var xml = new XmlDocument { XmlResolver = null };
        xml.LoadXml("<ArmorTemplate xmlns='uri:ea.com:eala:asset' id='PreparedArmor'/>");
        using (var reader = XmlReader.Create(new StringReader(
            "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='uri:ea.com:eala:asset'>" +
            "<xs:element name='ArmorTemplate'><xs:complexType><xs:attribute name='id' type='xs:string'/>" +
            "<xs:attribute name='Default' type='xs:string' default='100'/></xs:complexType></xs:element></xs:schema>")))
            xml.Schemas.Add(null, reader);
        xml.Validate(null);
        Require(xml.DocumentElement.Attributes["Default"] != null && !xml.DocumentElement.Attributes["Default"].Specified,
            "Fixture must contain a real schema-supplied unspecified default");
        var instance = new InstanceDeclaration(AssetDeclarationDocument.CreateBackendView(Namespaces()));
        instance.XmlNode = xml.DocumentElement;
        var plugin = new Recovered.Compiler.Plugin(); plugin.Initialize(null, TargetPlatform.Win32);
        Metadata(instance, plugin);
        object prepared = Prepare(instance, plugin);
        try
        {
            var frozen = Value<InstanceDeclaration>(prepared, "Declaration");
            Require(frozen.Node.Attributes["Default"] != null && frozen.Node.Attributes["Default"].Value == "100",
                "The actual backend input must preserve schema default values during detachment");
            Require(Execute(prepared) != null, "The detached default-bearing Armor declaration must execute through the real backend");
        }
        finally { ((IDisposable)prepared).Dispose(); }
    }
    static long PreparationEstimate(InstanceDeclaration instance)
    {
        return (long)contract.GetMethod("EstimatePreparationBytes", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { instance });
    }
    static void PreparationBudget()
    {
        var plugin = new Recovered.Compiler.Plugin(); plugin.Initialize(null, TargetPlatform.Win32);
        var declaration = Declaration("ArmorTemplate", "PreparationBudget", Namespaces());
        Metadata(declaration, plugin);
        long small = PreparationEstimate(declaration);
        Require(small >= 4096, "Preparation estimate must cover even a small private DOM and identity");
        // Use an otherwise ordinary metadata string. Preparation's size gate
        // precedes backend field validation and must not clone or serialize it.
        ((XmlElement)declaration.Node).SetAttribute("id", new string('x', 3 * 1024 * 1024));
        long large = PreparationEstimate(declaration);
        Require(large >= 3L * 1024L * 1024L * 64L && large > 128L * 1024L * 1024L,
            "A large single attribute must be included without relying on element count or source file size");
        Require(Prepare(declaration, plugin) == null, "Oversized preparation must use the owner compatibility path before cloning");
    }
    static void CommitRejected(BinaryAsset asset, string label)
    {
        bool rejected = false;
        try { asset.Commit(); }
        catch (BinaryAssetBuilderException error) { rejected = error.ErrorCode == ErrorCode.DependencyCacheFailure; }
        Require(rejected, "An unauthenticated " + label + " must not commit through a legacy result location");
    }
    static void BinaryAssetAuthorization(string work)
    {
        Settings previous = Settings.Current;
        bool previousMetrics = EALA.Metrics.MetricManager.Enabled;
        using (DocumentProcessor.BeginStateScope())
        try
        {
            EALA.Metrics.MetricManager.Enabled = false;
            string root = Path.Combine(work, "asset-authorization");
            Settings.Current = new Settings { TargetPlatform = TargetPlatform.Win32, BigEndian = false,
                BuildCache = true, BuildCacheDirectory = Path.Combine(root, "legacy-cache"), CustomPostfix = "" };
            // BinaryAsset's ordinary logging expects a real traversal frame.
            var frames = (Stack<string>)typeof(DocumentProcessor).GetField("_CurrentDocumentStack",
                BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            frames.Push("PreparedBackend authorization fixture:");
            using (var plugins = BuiltinCompilers.Create(TargetPlatform.Win32))
            {
                var processor = new DocumentProcessor(Settings.Current, plugins.Registry,
                    new VerifierPluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32));
                var output = new OutputManager(processor, null, Path.Combine(root, "output"),
                    Path.Combine(root, "intermediate"), null, null, null);
                var declaration = Declaration("W3DAnimation", "GuardAnimation", Namespaces());
                var plugin = plugins.Registry.GetPlugin(declaration.Handle.TypeId);
                Metadata(declaration, plugin);
                ((XmlElement)declaration.Node).SetAttribute("NumFrames", "1");
                ((XmlElement)declaration.Node).SetAttribute("FrameRate", "30");
                var first = plugin.ProcessInstance(declaration);
                Require(first != null && first.InstanceData != null, "Guard fixture must execute the real W3X backend");
                var asset = output.GetBinaryAsset(declaration, true);
                asset.Buffer = first;
                Require(asset.Commit() == AssetLocation.Memory, "Seed must use the real legacy memory commit");
                processor.AddLastWrittenAsset(asset);
                string path = Path.Combine(asset.AssetOutputDirectory, asset.AssetFileName);
                byte[] oldOutput = File.ReadAllBytes(path);
                var oldLocations = asset.GetLocation(AssetLocation.All,
                    AssetLocationOption.ReturnAll | AssetLocationOption.ForceUpdate);
                Require((oldLocations & AssetLocation.Output) != 0 && (oldLocations & AssetLocation.Local) != 0 && (oldLocations & AssetLocation.Cache) != 0,
                    "Guard fixture must contain actual old Output, Local and persistent Cache bytes");

                asset.BeginExecution("action-A");
                Require(asset.GetLocation(AssetLocation.All, AssetLocationOption.ReturnAll | AssetLocationOption.ForceUpdate) == AssetLocation.None,
                    "Beginning an action must mask all three pre-existing legacy result sources");
                CommitRejected(asset, "missing buffer");
                ((XmlElement)declaration.Node).SetAttribute("FrameRate", "60");
                var second = plugin.ProcessInstance(declaration);
                asset.Buffer = second;
                Require(asset.GetLocation(AssetLocation.All, AssetLocationOption.None) == AssetLocation.None,
                    "Installing buffers without source authorization must not make Memory or old Output trusted");
                CommitRejected(asset, "installed buffer");
                bool wrongIdentity = false;
                try { asset.AuthorizeExecutionResult("action-B", new string('b', 64)); }
                catch (InvalidOperationException) { wrongIdentity = true; }
                Require(wrongIdentity, "An action must reject a complete result carrying another action's identity");
                string digest;
                using (var bytes = new MemoryStream())
                using (var sha = SHA256.Create())
                {
                    bytes.Write(second.InstanceData, 0, second.InstanceData.Length);
                    bytes.Write(second.RelocationData, 0, second.RelocationData.Length);
                    bytes.Write(second.ImportsData, 0, second.ImportsData.Length);
                    digest = BitConverter.ToString(sha.ComputeHash(bytes.ToArray())).Replace("-", "").ToLowerInvariant();
                }
                asset.AuthorizeExecutionResult("action-A", digest);
                Require(asset.GetLocation(AssetLocation.All, AssetLocationOption.None) == AssetLocation.Memory,
                    "Authorized Memory must take precedence over stale Output/Local/Cache");
                Require(asset.Commit() == AssetLocation.Memory, "Commit must install the authorized replacement rather than reselect old Output");
                Require(!Convert.ToBase64String(File.ReadAllBytes(path)).Equals(Convert.ToBase64String(oldOutput), StringComparison.Ordinal),
                    "The real output asset must change after the different authorized backend result");
                Require(asset.GetLocation(AssetLocation.All, AssetLocationOption.ReturnAll) == AssetLocation.Output,
                    "Only this action's committed output may remain selectable after Memory is released");
                asset.BeginExecution("action-C");
                Require(asset.GetLocation(AssetLocation.All, AssetLocationOption.ReturnAll) == AssetLocation.None,
                    "Reusing an OutputManager's BinaryAsset must revoke the prior action authorization");
                CommitRejected(asset, "subsequent action");
            }
        }
        finally { Settings.Current = previous; EALA.Metrics.MetricManager.Enabled = previousMetrics; }
    }
    static void Put(string path, string text)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)); File.WriteAllText(path, text);
    }
    static string Assets(string body)
    {
        return "<AssetDeclaration xmlns='uri:ea.com:eala:asset'>" + body + "</AssetDeclaration>";
    }
    static Dictionary<string, string> RepeatedIncludeRun(string sdk, string mod, string output, int jobs, bool cancelInFlight = false, bool nativeFailure = false)
    {
        Settings previous = Settings.Current;
        bool previousMetrics = EALA.Metrics.MetricManager.Enabled;
        using (DocumentProcessor.BeginStateScope())
        try
        {
            EALA.Metrics.MetricManager.Enabled = false;
            var configuration = new XmlDocument { XmlResolver = null };
            configuration.Load(Path.Combine(sdk, "tools/BinaryAssetBuilder.exe.config"));
            var settings = (Settings)new SerializedSectionHandler().Create(null, null, configuration.SelectSingleNode("/configuration/assetbuilder"));
            settings.DataRoot = mod; settings.SchemaPath = Path.Combine(sdk, "Schemas/xsd/CnC3Types.xsd");
            settings.InputPath = Path.Combine(mod, "Root.xml");
            settings.OutputDirectory = Path.Combine(output, "output");
            settings.IntermediateOutputDirectory = Path.Combine(output, "intermediate");
            settings.SessionCacheDirectory = Path.Combine(output, "hashes");
            settings.DefaultDataPaths = mod; settings.DefaultArtPaths = mod; settings.DefaultAudioPaths = mod;
            settings.TargetPlatform = TargetPlatform.Win32; settings.BigEndian = false;
            settings.BuildCache = settings.SessionCache = false; settings.LinkedStreams = true;
            settings.OutputAssetReport = settings.OutputStringHashes = false;
            settings.ErrorLevel = 1; settings.UsePrecompiled = settings.StreamHints = settings.StableSort = false;
            Settings.Current = settings; SettingsLoader.PostProcessSettings(mod);
            Directory.CreateDirectory(settings.OutputDirectory); Directory.CreateDirectory(settings.IntermediateOutputDirectory);
            Directory.CreateDirectory(settings.SessionCacheDirectory);
            var schemas = new SchemaSet(false);
            using (HashProvider.BeginStringHashScope(settings.SessionCacheDirectory))
            using (var plugins = BuiltinCompilers.Create(TargetPlatform.Win32))
            {
                var verifiers = new VerifierPluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32);
                verifiers.DefaultPlugin = (IAssetBuilderVerifierPlugin)Activator.CreateInstance(
                    typeof(DocumentProcessor).Assembly.GetType("BinaryAssetBuilder.GameDataVerifier.Plugin", true), true);
                verifiers.DefaultPlugin.Initialize(null, TargetPlatform.Win32);
                var cache = new SessionCache(); cache.InitializeCache(new List<string>());
                var processor = new DocumentProcessor(settings, plugins.Registry, verifiers) { Cache = cache, SchemaSet = schemas };
                var assembly = typeof(CoreStreamCompiler).Assembly;
                var metrics = Activator.CreateInstance(assembly.GetType("NewModCompiler.Application.AssetExecutionMetrics", true), true);
                using (var cancellation = new CancellationTokenSource())
                using (var observerReady = new ManualResetEventSlim())
                {
                var execution = (IAssetExecution)Activator.CreateInstance(assembly.GetType("NewModCompiler.Application.AssetTaskExecution", true),
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
                    new object[] { processor, null, Path.Combine(output, "task-staging"), "no-persistent-cache", "repeat-include-fixture", jobs, metrics, cancellation.Token }, null);
                Thread observer = null;
                int stopObservation = 0;
                bool observedConcurrentCalls = false;
                object metricGate = metrics.GetType().GetField("gate", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metrics);
                FieldInfo activeCalls = metrics.GetType().GetField("active", BindingFlags.NonPublic | BindingFlags.Instance);
                if (cancelInFlight)
                {
                    observer = new Thread(delegate()
                    {
                        var timeout = Stopwatch.StartNew(); observerReady.Set();
                        while (Volatile.Read(ref stopObservation) == 0 && timeout.Elapsed < TimeSpan.FromSeconds(20))
                        {
                            lock (metricGate)
                            {
                                if ((int)activeCalls.GetValue(metrics) >= 2)
                                {
                                    // Cancel while both actual backend intervals
                                    // remain open under their own metrics lock.
                                    observedConcurrentCalls = true;
                                    cancellation.Cancel(); return;
                                }
                            }
                            Thread.Yield();
                        }
                        // Bound a broken test without retrying or manufacturing
                        // an overlap. The final assertion still rejects it.
                        if (Volatile.Read(ref stopObservation) == 0) cancellation.Cancel();
                    });
                    observer.IsBackground = true; observer.Start(); observerReady.Wait();
                }
                try
                {
                    processor.AssetExecution = execution;
                    bool success;
                    bool cancelled = false;
                    bool failedNative = false;
                    try { processor.ProcessDocument(settings.InputPath, true, false, out success); }
                    catch (OperationCanceledException) { cancelled = true; success = false; }
                    catch (NotSupportedException error)
                    {
                        if (!nativeFailure || error.Message.IndexOf("Animation frame range exceeds NumFrames", StringComparison.Ordinal) < 0) throw;
                        failedNative = true; success = false;
                    }
                    if (cancelInFlight || nativeFailure)
                    {
                        int activeBeforeDispose;
                        lock (metricGate) activeBeforeDispose = (int)activeCalls.GetValue(metrics);
                        ((IDisposable)execution).Dispose();
                        if (cancelInFlight)
                            Require(cancelled && observedConcurrentCalls, "Cancellation must be requested during two real in-flight native backend calls");
                        if (nativeFailure)
                            Require(failedNative && !cancelled, "The schema-legal frame range must fail inside the actual native worker");
                        lock (metricGate)
                        {
                            Require((int)activeCalls.GetValue(metrics) == 0, "Failure/cancellation disposal must join all in-flight native calls before returning");
                            var intervals = (System.Collections.IEnumerable)metrics.GetType().GetField("Intervals").GetValue(metrics);
                            int completedIntervals = 0;
                            foreach (object interval in intervals)
                            {
                                completedIntervals++;
                                Require((long)interval.GetType().GetField("endTicks").GetValue(interval) >= (long)interval.GetType().GetField("startTicks").GetValue(interval),
                                    "Every started native interval must have safely returned after failure/cancellation");
                            }
                            Require(completedIntervals == 2, "Both actual native calls must be present in the failure/cancellation evidence");
                            lifecycleEvidence.Add(new { scenario = nativeFailure ? "native-worker-range-failure" : "cancel-during-two-native-calls",
                                observedConcurrentCalls, activeBeforeDispose, activeAfterDispose = 0, completedIntervals });
                        }
                        foreach (string file in new[] { "One.xml", "Two.xml", "Root.xml" })
                        {
                            AssetDeclarationDocument document;
                            cache.TryGetDocument(Path.Combine(mod, file), null, TargetPlatform.Win32, false, out document);
                            Require(document != null && document.State != DocumentState.Complete && !document.Processing,
                                "Failed/cancelled in-flight work must not publish document completion: " + file);
                        }
                        Require(Directory.GetFiles(settings.OutputDirectory, "*.manifest", SearchOption.AllDirectories).Length == 0,
                            "Failed/cancelled native work must not reach successful final stream linking");
                        return new Dictionary<string, string>();
                    }
                    Require(!cancelled, "The non-cancelled repeated include build must complete");
                    execution.Flush();
                    Require(success && processor.InstanceCompileCount == 3,
                        "Repeated includes must execute the three distinct real assets exactly once with jobs=" + jobs);
                    foreach (var expected in new Dictionary<string, int> { { "Leaf.xml", 2 }, { "A.xml", 2 }, { "B.xml", 1 }, { "Root.xml", 3 } })
                    {
                        AssetDeclarationDocument document;
                        Require(cache.TryGetDocument(Path.Combine(mod, expected.Key), null, TargetPlatform.Win32, false, out document) && document != null,
                            "Actual processed document must remain in the real session cache: " + expected.Key);
                        Require(document.State == DocumentState.Complete && !document.Processing && document.Instances.Count == expected.Value,
                            "Deferred completion must preserve prepared declarations through repeated include and inheritance: " + expected.Key);
                    }
                    object pendingDocuments = typeof(DocumentProcessor).GetField("_PendingExecutionDocuments",
                        BindingFlags.NonPublic | BindingFlags.Instance).GetValue(processor);
                    Require((int)pendingDocuments.GetType().GetProperty("Count").GetValue(pendingDocuments, null) == 0,
                        "Successful physical-result barrier must leave no deferred document completion");
                    var result = new Dictionary<string, string>(StringComparer.Ordinal);
                    foreach (string file in Directory.GetFiles(settings.OutputDirectory, "*", SearchOption.AllDirectories))
                    {
                        string extension = Path.GetExtension(file);
                        if (extension != ".manifest" && extension != ".bin" && extension != ".imp" && extension != ".relo") continue;
                        using (var sha = SHA256.Create()) result.Add(file.Substring(settings.OutputDirectory.Length), BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(file))));
                    }
                    Require(result.Count == 4, "Repeated-include production stream must produce all four raw output files");
                    return result;
                }
                finally
                {
                    Volatile.Write(ref stopObservation, 1);
                    if (observer != null) observer.Join();
                    ((IDisposable)execution).Dispose();
                }
                }
            }
        }
        finally { Settings.Current = previous; EALA.Metrics.MetricManager.Enabled = previousMetrics; }
    }
    static void RepeatedIncludeAndInheritance(string work, string sdk)
    {
        string mod = Path.Combine(work, "repeated-include-input");
        Put(Path.Combine(mod, "Leaf.xml"), Assets(
            "<ArmorTemplate id='PreparedBaseArmor' Default='50%'/><W3DAnimation id='PreparedLeafAnimation' NumFrames='1' FrameRate='30'><Channels/></W3DAnimation>"));
        Put(Path.Combine(mod, "A.xml"), Assets("<Includes><Include type='all' source='Leaf.xml'/></Includes>"));
        Put(Path.Combine(mod, "B.xml"), Assets("<Includes><Include type='instance' source='Leaf.xml'/></Includes>" +
            "<ArmorTemplate id='PreparedDerivedArmor' inheritFrom='PreparedBaseArmor' Default='75%'/>"));
        Put(Path.Combine(mod, "Root.xml"), Assets("<Includes><Include type='all' source='A.xml'/><Include type='all' source='B.xml'/></Includes>"));
        var serial = RepeatedIncludeRun(sdk, mod, Path.Combine(work, "repeated-serial"), 1);
        var parallel = RepeatedIncludeRun(sdk, mod, Path.Combine(work, "repeated-parallel"), 2);
        Require(serial.Count == parallel.Count, "Repeated include serial/parallel outputs must have the same members");
        foreach (var output in serial)
            Require(parallel.ContainsKey(output.Key) && parallel[output.Key] == output.Value,
                "Repeated includes and inherited input must preserve exact serial stream bytes: " + output.Key);
    }
    static void CancelInFlight(string work, string sdk)
    {
        string mod = Path.Combine(work, "native-cancellation-input");
        Put(Path.Combine(mod, "One.xml"), Assets("<W3DAnimation id='CancelNativeOne' NumFrames='4096' FrameRate='30'>" +
            "<Channels><ChannelScalar Type='XTranslation' Pivot='0' FirstFrame='0'><Frame>0</Frame><Frame>1</Frame></ChannelScalar></Channels></W3DAnimation>"));
        Put(Path.Combine(mod, "Two.xml"), Assets("<W3DAnimation id='CancelNativeTwo' NumFrames='8192' FrameRate='30'>" +
            "<Channels><ChannelScalar Type='XTranslation' Pivot='0' FirstFrame='0'><Frame>0</Frame><Frame>2</Frame></ChannelScalar></Channels></W3DAnimation>"));
        Put(Path.Combine(mod, "Root.xml"), Assets("<Includes><Include type='all' source='One.xml'/><Include type='all' source='Two.xml'/></Includes>"));
        RepeatedIncludeRun(sdk, mod, Path.Combine(work, "native-cancelled-output"), 2, true);
    }
    static void NativeWorkerFailure(string work, string sdk)
    {
        string mod = Path.Combine(work, "native-failure-input");
        // xs:unsignedInt permits zero; no schema relation constrains Frame to
        // NumFrames. Native PrepareAnimation must reject this actual overrun.
        Put(Path.Combine(mod, "One.xml"), Assets("<W3DAnimation id='FailNativeOne' NumFrames='0' FrameRate='30'>" +
            "<Channels><ChannelScalar Type='XTranslation' Pivot='0' FirstFrame='0'><Frame>1</Frame></ChannelScalar></Channels></W3DAnimation>"));
        Put(Path.Combine(mod, "Two.xml"), Assets("<W3DAnimation id='FailNativeTwo' NumFrames='8192' FrameRate='30'>" +
            "<Channels><ChannelScalar Type='XTranslation' Pivot='0' FirstFrame='0'><Frame>0</Frame><Frame>2</Frame></ChannelScalar></Channels></W3DAnimation>"));
        Put(Path.Combine(mod, "Root.xml"), Assets("<Includes><Include type='all' source='One.xml'/><Include type='all' source='Two.xml'/></Includes>"));
        RepeatedIncludeRun(sdk, mod, Path.Combine(work, "native-failed-output"), 2, false, true);
    }
    public static int Main(string[] args)
    {
        try
        {
            Directory.CreateDirectory(args[0]); AptLeaseAndFreeze(args[0]); W3xBudget(); SchemaDefaults(); PreparationBudget(); BinaryAssetAuthorization(args[0]);
            RepeatedIncludeAndInheritance(args[0], args[4]);
            CancelInFlight(args[0], args[4]);
            NativeWorkerFailure(args[0], args[4]);
            File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(new {
                succeeded = true, sourceCommit = args[2], runId = args[3], checks, lifecycleEvidence,
                scope = "Actual prepared input isolation, native APT content lease, exact external invalidation, schema defaults and preparation budgets, real BinaryAsset authorization guard, repeated include/inheritance byte equality, cancellation and native worker failure; normal cross-document output overlap is checked separately"
            }));
            Console.WriteLine("PASS PreparedBackend: " + checks + " assertions"); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
