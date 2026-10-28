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
            Require(Value<bool>(prepared, "CanRunParallel"), "Bounded leased APT XML must be certified for independent backend execution");
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
    static void EqualBuffers(AssetBuffer expected, AssetBuffer actual, string type)
    {
        Require(expected != null && actual != null, "Backend buffers must exist for " + type);
        byte[][] left = { expected.InstanceData, expected.RelocationData, expected.ImportsData };
        byte[][] right = { actual.InstanceData, actual.RelocationData, actual.ImportsData };
        for (int array = 0; array < left.Length; array++)
        {
            Require(left[array] != null && right[array] != null && left[array].Length == right[array].Length, "Prepared three-buffer lengths differ for " + type);
            for (int i = 0; i < left[array].Length; i++)
                Require(left[array][i] == right[array][i], "Prepared three-buffer raw byte differs for " + type);
        }
    }
    sealed class UnknownAudioCompiler : BinaryAssetBuilder.AudioCompiler.IAudioFileCompiler
    {
        public AssetBuffer CompileAudioFile(InstanceDeclaration declaration, TargetPlatform platform)
        { throw new InvalidOperationException("Preparation must never execute an untrusted injected compiler"); }
    }
    static void ParallelQualification(string work)
    {
        string input = Path.Combine(work, "qualified-audio.wav");
        using (var stream = File.Create(input))
        using (var writer = new BinaryWriter(stream))
        {
            writer.Write(Encoding.ASCII.GetBytes("RIFF")); writer.Write(52);
            writer.Write(Encoding.ASCII.GetBytes("WAVEfmt ")); writer.Write(16);
            writer.Write((short)1); writer.Write((short)1); writer.Write(22050); writer.Write(44100);
            writer.Write((short)2); writer.Write((short)16); writer.Write(Encoding.ASCII.GetBytes("data")); writer.Write(16);
            for (int i = 0; i < 8; i++) writer.Write((short)(i * 127));
        }
        var audio = new BinaryAssetBuilder.AudioCompiler.Plugin(); audio.Initialize(null, TargetPlatform.Win32);
        var declaration = Declaration("AudioFile", "QualifiedAudio", Namespaces()); Metadata(declaration, audio);
        ((XmlElement)declaration.Node).SetAttribute("File", input);
        ((XmlElement)declaration.Node).SetAttribute("PCCompression", "XAS");
        object prepared = Prepare(declaration, audio);
        try { Require(prepared != null && Value<bool>(prepared, "CanRunParallel"), "Known bounded recovered AudioFile must be parallel eligible"); }
        finally { if (prepared != null) ((IDisposable)prepared).Dispose(); }
        ((XmlElement)declaration.Node).SetAttribute("PCSampleRate", "22050");
        prepared = Prepare(declaration, audio);
        try { Require(prepared != null && !Value<bool>(prepared, "CanRunParallel"), "Explicit sample-rate processing must retain its owner-only budget contract"); }
        finally { if (prepared != null) ((IDisposable)prepared).Dispose(); }
        ((XmlElement)declaration.Node).RemoveAttribute("PCSampleRate");
        var unknown = new BinaryAssetBuilder.AudioCompiler.Plugin(new UnknownAudioCompiler()); unknown.Initialize(null, TargetPlatform.Win32);
        Require(Prepare(declaration, unknown) == null, "Injected AudioFile compiler must not inherit recovered cache or parallel certification");
        using (var stream = new FileStream(input, FileMode.Open, FileAccess.Write)) stream.SetLength(2L * 1024L * 1024L);
        prepared = Prepare(declaration, audio);
        try { Require(prepared != null && Value<bool>(prepared, "CanRunParallel") && Value<long>(prepared, "PreparedBytes") > 64L * 1024L * 1024L, "Large audio stays semantically certified while its decoded/encoded estimate controls resource admission"); }
        finally { if (prepared != null) ((IDisposable)prepared).Dispose(); }
        var xml = new Recovered.XmlPriority.Plugin(); xml.Initialize(null, TargetPlatform.Win32);
        foreach (string type in new[] { "AudioEventLimitGroup", "AudioLOD", "AudioSettings", "Video", "VideoEventList", "MicrophoneSettings", "BootupDisplaySequence" })
        {
            declaration = Declaration(type, "Qualified" + type, Namespaces()); Metadata(declaration, xml);
            prepared = Prepare(declaration, xml);
            try
            {
                Require(prepared != null && Value<bool>(prepared, "CanRunParallel"), "Audited XML metadata root must retain independent backend eligibility: " + type);
                EqualBuffers(xml.ProcessInstance(declaration), Execute(prepared), type);
            }
            finally { if (prepared != null) ((IDisposable)prepared).Dispose(); }
        }
        // File length affects retention even though this fixture never executes
        // invalid large WAV contents. Only the lease/qualification is exercised.
        File.Delete(input);
    }
    static Func<T> TypedSampler<T>(Func<object> sampler) { return delegate { return (T)sampler(); }; }
    static object ResourceSampler(Func<object> sampler)
    {
        return typeof(CompilerPreparedBackend).GetMethod("TypedSampler", BindingFlags.Static | BindingFlags.NonPublic)
            .MakeGenericMethod(resourcePolicy).Invoke(null, new object[] { sampler });
    }
    static readonly Type resourcePolicy = typeof(CoreStreamCompiler).Assembly.GetType("NewModCompiler.Application.BackendResourcePolicy", true);
    static object ResourcePolicy(int jobs, int processors, long physical, long availableVirtual, long totalVirtual, bool queried)
    {
        return resourcePolicy.GetMethod("Calculate").Invoke(null, new object[] { jobs, processors, physical, availableVirtual, totalVirtual, queried });
    }
    static T ResourceValue<T>(object policy, string property)
    {
        return (T)resourcePolicy.GetProperty(property).GetValue(policy, null);
    }
    static void ResourceBudget(string work)
    {
        const long MiB = 1024L * 1024L, GiB = 1024L * MiB;
        object generous = ResourcePolicy(Int32.MaxValue, 8, 16 * GiB, 3 * GiB, 4 * GiB, true);
        Require(ResourceValue<int>(generous, "RequestedJobs") == Int32.MaxValue && ResourceValue<int>(generous, "ProcessorCount") == 8,
            "Resource policy must retain the requested jobs and detected CPU identity");
        Require(ResourceValue<int>(generous, "Capacity") == 8, "A sufficiently provisioned eight-CPU host must not remain hard-capped at two workers");
        Require(ResourceValue<long>(generous, "ByteBudget") == GiB && ResourceValue<long>(generous, "PerTaskBudget") == GiB / 2,
            "Available virtual address space and process reserve must bound aggregate/per-task retention");
        Require(ResourceValue<long>(generous, "PreparationBudget") == GiB / 2, "Private input preparation must follow the observed resource budget");
        Require(ResourceValue<int>(ResourcePolicy(1, 8, 16 * GiB, 3 * GiB, 4 * GiB, true), "Capacity") == 1,
            "Explicit jobs=1 must remain serial on a large host");
        Require(ResourceValue<int>(ResourcePolicy(8, 1, 16 * GiB, 3 * GiB, 4 * GiB, true), "Capacity") == 1,
            "One logical processor must limit dispatch despite large RAM and jobs");
        Require(ResourceValue<int>(ResourcePolicy(Int32.MaxValue, 128, 32 * GiB, 6 * GiB, 8 * GiB, true), "Capacity") == 32,
            "Huge jobs and CPU counts must be bounded independently of requested queue allocation");
        object lowVirtual = ResourcePolicy(8, 8, 32 * GiB, 256 * MiB, 2 * GiB, true);
        Require(ResourceValue<int>(lowVirtual, "Capacity") == 1 && ResourceValue<long>(lowVirtual, "ByteBudget") == 0,
            "A Win32 process with exhausted virtual space must not admit workers merely because host RAM is plentiful");
        object lowPhysical = ResourcePolicy(8, 8, 600 * MiB, 1536 * MiB, 2 * GiB, true);
        Require(ResourceValue<long>(lowPhysical, "ByteBudget") == 44 * MiB && ResourceValue<int>(lowPhysical, "Capacity") == 2,
            "Physical availability must independently constrain the aggregate budget and worker count");
        object failed = ResourcePolicy(Int32.MaxValue, 8, 32 * GiB, 3 * GiB, 4 * GiB, false);
        Require(!ResourceValue<bool>(failed, "MemoryQuerySucceeded") && ResourceValue<int>(failed, "Capacity") == 1 &&
            ResourceValue<long>(failed, "ByteBudget") == 16 * MiB && ResourceValue<long>(failed, "PerTaskBudget") == 8 * MiB,
            "Failed memory observation must explicitly degrade to one worker and a conservative bounded budget");
        object invalid = ResourcePolicy(8, 0, -1, -1, 2 * GiB, true);
        Require(ResourceValue<int>(invalid, "Capacity") == 1 && ResourceValue<long>(invalid, "ByteBudget") == 0,
            "Invalid CPU/availability observations must not overflow into a large worker pool");
        object overflow = ResourcePolicy(Int32.MaxValue, Int32.MaxValue, Int64.MaxValue, Int64.MaxValue, 2 * GiB, true);
        Require(ResourceValue<int>(overflow, "Capacity") <= 32 && ResourceValue<long>(overflow, "ByteBudget") <= 512 * MiB,
            "Extreme observations must remain bounded by the Win32 virtual address-space identity");
        object tightened = resourcePolicy.GetMethod("RecalculateMemory").Invoke(generous,
            new object[] { 600 * MiB, 1536 * MiB, 2 * GiB, true });
        Require(ResourceValue<int>(tightened, "ProcessorCount") == 8 && ResourceValue<int>(tightened, "RequestedJobs") == Int32.MaxValue &&
            ResourceValue<int>(tightened, "Capacity") == 2 && ResourceValue<long>(tightened, "ByteBudget") == 44 * MiB,
            "A new memory observation must tighten capacity without losing CPU/request identity");
        Require(ResourceValue<int>(generous, "Capacity") == 8 && ResourceValue<long>(generous, "ByteBudget") == GiB,
            "Refreshing resources must not silently mutate the prior observation");
        var assembly = typeof(CoreStreamCompiler).Assembly;
        var metrics = Activator.CreateInstance(assembly.GetType("NewModCompiler.Application.AssetExecutionMetrics", true), true);
        var executorType = assembly.GetType("NewModCompiler.Application.AssetTaskExecution", true);
        var executor = (IDisposable)Activator.CreateInstance(executorType,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
            new object[] { null, null, Path.Combine(work, "pure-admission"), "toolchain", "options", 8, metrics,
                           CancellationToken.None, ResourceSampler(delegate { return lowPhysical; }) }, null);
        try
        {
            var retained = executorType.GetField("retainedBytes", BindingFlags.Instance | BindingFlags.NonPublic);
            var fits = executorType.GetMethod("Fits", BindingFlags.Instance | BindingFlags.NonPublic);
            Require((bool)fits.Invoke(executor, new object[] { 44 * MiB }), "The exact aggregate resource boundary must be admissible");
            Require(!(bool)fits.Invoke(executor, new object[] { 44 * MiB + 1 }), "One byte over the observed aggregate budget must be rejected");
            retained.SetValue(executor, 40 * MiB);
            Require((bool)fits.Invoke(executor, new object[] { 4 * MiB }) && !(bool)fits.Invoke(executor, new object[] { 4 * MiB + 1 }),
                "Already retained prepared/worker/result bytes must reduce further admission");
            retained.SetValue(executor, Int64.MaxValue);
            Require(!(bool)fits.Invoke(executor, new object[] { Int64.MaxValue }), "Retained byte arithmetic must reject overflow rather than admit work");
        }
        finally { executorType.GetField("retainedBytes", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(executor, 0L); executor.Dispose(); }

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
        try { Require(Value<bool>(prepared, "CanRunParallel") && Value<long>(prepared, "PreparedBytes") > 2L * 1024L * 1024L * 1024L, "Semantic W3X certification must retain an expansion estimate that excludes huge tasks from any Win32 worker budget"); }
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
        var budgetArguments = new object[] { declaration, plugin, 0L, null };
        Require(!(bool)contract.GetMethod("TryPrepareWithinBudget").Invoke(null, budgetArguments) && budgetArguments[3] == null,
            "A zero observed preparation budget must reject before allocating a detached private representation");

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
    static Dictionary<string, string> RepeatedIncludeRun(string sdk, string mod, string output, int jobs, bool cancelInFlight = false, bool nativeFailure = false, bool resourceFixture = false, bool tightenResources = false, bool limitSingleTask = false)
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
                var executionType = assembly.GetType("NewModCompiler.Application.AssetTaskExecution", true);
                IAssetExecution execution = null;
                bool observedQueuedAtTightening = false;
                int activeAtTightening = -1;
                object sampler = null;
                if (tightenResources)
                {
                    object generous = ResourcePolicy(jobs, 8, 16L * 1024 * 1024 * 1024, 3L * 1024 * 1024 * 1024, 4L * 1024 * 1024 * 1024, true);
                    object pressured = ResourcePolicy(jobs, 8, 0, 0, 2L * 1024 * 1024 * 1024, false);
                    sampler = ResourceSampler(delegate
                    {
                        if (execution != null)
                        {
                            object queue = executionType.GetField("pending", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(execution);
                            if ((int)queue.GetType().GetProperty("Count").GetValue(queue, null) > 0)
                            {
                                observedQueuedAtTightening = true;
                                object gate = metrics.GetType().GetField("gate", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(metrics);
                                lock (gate) activeAtTightening = (int)metrics.GetType().GetField("active", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(metrics);
                                return pressured;
                            }
                        }
                        return generous;
                    });
                }
                if (limitSingleTask)
                {
                    object constrained = ResourcePolicy(jobs, 8, 600L * 1024 * 1024, 1536L * 1024 * 1024, 2L * 1024 * 1024 * 1024, true);
                    sampler = ResourceSampler(delegate { return constrained; });
                }
                object[] executionArguments = new object[] { processor, null, Path.Combine(output, "task-staging"), "no-persistent-cache", "repeat-include-fixture", jobs, metrics, cancellation.Token };
                if (sampler != null)
                {
                    var injected = new object[executionArguments.Length + 1];
                    Array.Copy(executionArguments, injected, executionArguments.Length); injected[injected.Length - 1] = sampler;
                    executionArguments = injected;
                }
                execution = (IAssetExecution)Activator.CreateInstance(executionType,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, executionArguments, null);
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
                    if (!resourceFixture) foreach (var expected in new Dictionary<string, int> { { "Leaf.xml", 2 }, { "A.xml", 2 }, { "B.xml", 1 }, { "Root.xml", 3 } })
                    {
                        AssetDeclarationDocument document;
                        Require(cache.TryGetDocument(Path.Combine(mod, expected.Key), null, TargetPlatform.Win32, false, out document) && document != null,
                            "Actual processed document must remain in the real session cache: " + expected.Key);
                        Require(document.State == DocumentState.Complete && !document.Processing && document.Instances.Count == expected.Value,
                            "Deferred completion must preserve prepared declarations through repeated include and inheritance: " + expected.Key);
                    }
                    if (limitSingleTask)
                    {
                        Require((int)metrics.GetType().GetField("EffectiveJobs").GetValue(metrics) == 2 &&
                            (long)metrics.GetType().GetField("PerTaskMemoryLimitBytes").GetValue(metrics) == 22L * 1024 * 1024,
                            "Single-task admission fixture must retain two-worker/44MiB aggregate/22MiB individual limits");
                        var intervals = (System.Collections.IEnumerable)metrics.GetType().GetField("Intervals").GetValue(metrics);
                        bool checkedLarge = false, checkedSmallWorker = false;
                        foreach (object interval in intervals)
                        {
                            string sourcePath = (string)interval.GetType().GetField("sourcePath").GetValue(interval);
                            if (String.Equals(Path.GetFileName(sourcePath), "One.xml", StringComparison.OrdinalIgnoreCase))
                            {
                                Require((int)interval.GetType().GetField("threadId").GetValue(interval) == Thread.CurrentThread.ManagedThreadId,
                                    "An individually oversized APT task must execute on the owner even when it fits the aggregate pool");
                                checkedLarge = true;
                            }
                            else if ((int)interval.GetType().GetField("threadId").GetValue(interval) != Thread.CurrentThread.ManagedThreadId)
                                checkedSmallWorker = true;
                        }
                        Require(checkedLarge && checkedSmallWorker, "The oversized APT must run on the owner while an admissible independent small APT uses a worker");
                        lifecycleEvidence.Add(new { scenario = "oversized-single-apt-on-owner", effectiveJobs = 2,
                            taskMemoryBudgetBytes = 44L * 1024 * 1024, perTaskMemoryLimitBytes = 22L * 1024 * 1024,
                            inputBytes = 8L * 1024 * 1024, estimatedRetainedBytesAtLeast = 32L * 1024 * 1024 });
                    }
                    if (tightenResources)
                    {
                        Require(observedQueuedAtTightening, "Resource reduction must occur with a real pending backend result");
                        Require((int)executionType.GetField("capacity", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(execution) == 1 &&
                            (long)executionType.GetField("byteBudget", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(execution) == 16L * 1024 * 1024,
                            "A later generous sample must not re-expand this stream after memory pressure");
                        Require((int)metrics.GetType().GetField("MaximumConcurrency").GetValue(metrics) == 1 &&
                            (int)metrics.GetType().GetField("MinimumEffectiveJobs").GetValue(metrics) == 1 &&
                            !(bool)metrics.GetType().GetField("ResourceMemoryQuerySucceeded").GetValue(metrics),
                            "Budget reduction must stop further parallel admission and report conservative fallback");
                        var intervals = (System.Collections.IEnumerable)metrics.GetType().GetField("Intervals").GetValue(metrics);
                        long priorEnd = 0; int count = 0;
                        foreach (object interval in intervals)
                        {
                            long start = (long)interval.GetType().GetField("startTicks").GetValue(interval);
                            long end = (long)interval.GetType().GetField("endTicks").GetValue(interval);
                            Require(start >= priorEnd && end > start, "A reduced resource budget must join prior real work before the next backend starts");
                            priorEnd = end; count++;
                        }
                        Require(count == 3, "Resource pressure must preserve all three actual backend calls");
                        lifecycleEvidence.Add(new { scenario = "memory-pressure-with-pending-work", observedQueuedAtTightening, activeAtTightening, completedIntervals = count,
                            effectiveJobs = 8, minimumEffectiveJobs = 1, minimumTaskMemoryBudgetBytes = 16L * 1024 * 1024 });
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
    static void ResourcePressure(string work, string sdk)
    {
        string mod = Path.Combine(work, "resource-pressure-input");
        string includes = "";
        foreach (string name in new[] { "One", "Two", "Three" })
        {
            string frames = name == "One" ? "4096" : "1024";
            Put(Path.Combine(mod, name + ".xml"), Assets("<W3DAnimation id='Resource" + name + "' NumFrames='" + frames + "' FrameRate='30'>" +
                "<Channels><ChannelScalar Type='XTranslation' Pivot='0' FirstFrame='0'><Frame>0</Frame><Frame>1</Frame></ChannelScalar></Channels></W3DAnimation>"));
            includes += "<Include type='all' source='" + name + ".xml'/>";
        }
        Put(Path.Combine(mod, "Root.xml"), Assets("<Includes>" + includes + "</Includes>"));
        string serialOutput = Path.Combine(work, "resource-pressure-serial"), constrainedOutput = Path.Combine(work, "resource-pressure-constrained");
        var serial = RepeatedIncludeRun(sdk, mod, serialOutput, 1, false, false, true, false);
        var constrained = RepeatedIncludeRun(sdk, mod, constrainedOutput, 8, false, false, true, true);
        Require(serial.Count == constrained.Count, "Resource pressure must preserve the exact linked output layout");
        foreach (string relative in serial.Keys)
        {
            byte[] left = File.ReadAllBytes(Path.Combine(serialOutput, "output") + relative);
            byte[] right = File.ReadAllBytes(Path.Combine(constrainedOutput, "output") + relative);
            Require(left.Length == right.Length, "Resource-constrained output length differs");
            for (int i = 0; i < left.Length; i++) Require(left[i] == right[i], "Resource-constrained output raw byte differs");
        }
    }
    static void SingleTaskAdmission(string work, string sdk)
    {
        string mod = Path.Combine(work, "single-task-admission-input");
        string includes = "";
        foreach (string name in new[] { "One", "Two", "Three" })
        {
            string input = Path.Combine(mod, name + ".bin");
            Directory.CreateDirectory(mod);
            using (var stream = File.Create(input))
            {
                if (name == "One") stream.SetLength(8L * 1024 * 1024);
                else stream.Write(new byte[] { 1, 2, 3, 4 }, 0, 4);
            }
            Put(Path.Combine(mod, name + ".xml"), Assets("<AptAptData id='Admission" + name + "' File='" + name + ".bin'/>"));
            includes += "<Include type='all' source='" + name + ".xml'/>";
        }
        Put(Path.Combine(mod, "Root.xml"), Assets("<Includes>" + includes + "</Includes>"));
        string serialOutput = Path.Combine(work, "single-task-serial"), constrainedOutput = Path.Combine(work, "single-task-constrained");
        var serial = RepeatedIncludeRun(sdk, mod, serialOutput, 1, false, false, true);
        var constrained = RepeatedIncludeRun(sdk, mod, constrainedOutput, 8, false, false, true, false, true);
        Require(serial.Count == constrained.Count, "Single-task owner fallback must preserve linked output layout");
        foreach (string relative in serial.Keys)
        {
            byte[] left = File.ReadAllBytes(Path.Combine(serialOutput, "output") + relative);
            byte[] right = File.ReadAllBytes(Path.Combine(constrainedOutput, "output") + relative);
            Require(left.Length == right.Length, "Single-task owner output length differs");
            for (int i = 0; i < left.Length; i++) Require(left[i] == right[i], "Single-task owner output raw byte differs");
        }
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
            Directory.CreateDirectory(args[0]); AptLeaseAndFreeze(args[0]); ParallelQualification(args[0]); ResourceBudget(args[0]); W3xBudget(); SchemaDefaults(); PreparationBudget(); BinaryAssetAuthorization(args[0]);
            RepeatedIncludeAndInheritance(args[0], args[4]); ResourcePressure(args[0], args[4]); SingleTaskAdmission(args[0], args[4]);
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
