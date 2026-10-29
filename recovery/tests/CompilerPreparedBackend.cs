using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
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
    static readonly Type contract = typeof(CoreStreamCompiler).Assembly.GetType("NewModCompiler.Application.PreparedBackendWork", true);
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
    static object PrepareWithReason(InstanceDeclaration declaration, IAssetBuilderPlugin plugin, long budget, out string reason)
    {
        var arguments = new object[] { declaration, plugin, budget, null, null };
        bool accepted = (bool)contract.GetMethod("TryPrepareWithReason", BindingFlags.Public | BindingFlags.Static).Invoke(null, arguments);
        reason = (string)arguments[4];
        Require(accepted == (arguments[3] != null), "Preparation acceptance must match retained envelope presence");
        Require(accepted ? reason == null : !string.IsNullOrEmpty(reason), "Exactly refused preparations must have a reason");
        return arguments[3];
    }
    static void RejectedPreparation(InstanceDeclaration declaration, IAssetBuilderPlugin plugin, long budget, string expected)
    {
        string reason;
        object prepared = PrepareWithReason(declaration, plugin, budget, out reason);
        try { Require(prepared == null && reason == expected, "Expected exact preparation refusal " + expected + ", actual " + reason); }
        finally { if (prepared != null) ((IDisposable)prepared).Dispose(); }
    }
    static T Value<T>(object prepared, string property)
    {
        return (T)contract.GetProperty(property, BindingFlags.Public | BindingFlags.Instance).GetValue(prepared, null);
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
        object prepared = Prepare(instance, plugin);
        Require(prepared != null && Value<bool>(prepared, "CanRunParallel"), "Existing APT blob input must be parallel eligible");
        try
        {
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
            Require(result != null && Contains(result.InstanceData, first) && !Contains(result.InstanceData, second), "Native APT consumption must match the held version");
        }
        finally { ((IDisposable)prepared).Dispose(); }
        ((XmlElement)instance.Node).SetAttribute("File", path);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        DateTime timestamp = File.GetLastWriteTimeUtc(path);
        File.WriteAllBytes(path, second); File.SetLastWriteTimeUtc(path, timestamp);
        Require(new FileInfo(path).Length == first.Length && File.GetLastWriteTimeUtc(path) == timestamp, "Equal-length/mtime fixture must be exact");
        prepared = Prepare(instance, plugin);
        Require(prepared != null, "A released input lease must allow a new version");
        try { Require(Contains(Execute(prepared).InstanceData, second), "A fresh lease must consume equal-length equal-mtime changed bytes"); }
        finally { ((IDisposable)prepared).Dispose(); }
        File.Delete(path);
        Require(Prepare(instance, plugin) == null, "A missing APT read must use the compatibility path rather than a stale input lease");
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
        try { Require(prepared == null, "Explicit sample-rate processing must retain its original owner-only input"); }
        finally { if (prepared != null) ((IDisposable)prepared).Dispose(); }
        ((XmlElement)declaration.Node).RemoveAttribute("PCSampleRate");
        var unknown = new BinaryAssetBuilder.AudioCompiler.Plugin(new UnknownAudioCompiler()); unknown.Initialize(null, TargetPlatform.Win32);
        Require(Prepare(declaration, unknown) == null, "Injected AudioFile compiler must not inherit recovered cache or parallel certification");
        using (var stream = new FileStream(input, FileMode.Open, FileAccess.Write)) stream.SetLength(2L * 1024L * 1024L);
        prepared = PrepareWithReason(declaration, audio, 256L * 1024L * 1024L, out string largeReason);
        try { Require(prepared != null && Value<bool>(prepared, "CanRunParallel") && Value<long>(prepared, "PreparedBytes") > 64L * 1024L * 1024L, "Large audio stays semantically certified within an adequate preparation budget"); }
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
    static void EnvironmentQualification(string sdk)
    {
        // Use the existing source-derived, XSD-covered component fixtures rather
        // than empty roots that omit required fields and pointer/list payloads.
        var fixtures = new XmlDocument { XmlResolver = null };
        fixtures.Load(Path.Combine(sdk, "../recovery/tests/XmlenvironmentProbe.xml"));
        string[] roots = { "Weather", "WaterTransparency", "WaterSetting", "Environment", "LightingSettings",
            "WaterTextureList", "TerrainParticleMapping", "BounceKickTerrainMapping", "ShadowMap",
            "OrdersManagerSettings", "TerrainVisualSettings", "CameraSettings" };
        foreach (TargetPlatform platform in new[] { TargetPlatform.Win32, TargetPlatform.Xbox360, TargetPlatform.PlayStation3, (TargetPlatform)99 })
        {
            var plugin = new Recovered.XmlPriority.Plugin(); plugin.Initialize(null, platform);
            foreach (string type in roots)
            {
                int fixtureCount = 0;
                foreach (XmlNode source in fixtures.DocumentElement.ChildNodes)
                {
                    if (source.LocalName != type) continue;
                    fixtureCount++;
                    var declaration = Declaration(type, "Prepared" + type, Namespaces());
                    var document = new XmlDocument { XmlResolver = null };
                    declaration.XmlNode = document.ImportNode(source, true); document.AppendChild(declaration.Node);
                    Metadata(declaration, plugin);
                    object prepared = Prepare(declaration, plugin);
                    try
                    {
                        Require(prepared != null && Value<bool>(prepared, "CanRunParallel"), "Complete environment family must be independently eligible: " + type);
                        EqualBuffers(plugin.ProcessInstance(declaration), Execute(prepared), type + "/" + platform);
                    }
                    finally { if (prepared != null) ((IDisposable)prepared).Dispose(); }
                }
                Require(fixtureCount > 0, "Environment qualification fixture missing: " + type);
            }
            var owner = Declaration("UncertifiedFutureXmlBackend", "StillOwner", Namespaces());
            object unqualified = Prepare(owner, plugin);
            try { Require(unqualified == null, "Unaudited XML roots must retain original owner input without cloning"); }
            finally { if (unqualified != null) ((IDisposable)unqualified).Dispose(); }
        }
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
        try { Require(prepared == null, "Huge frame expansion must use owner compatibility before cloning"); }
        finally { if (prepared != null) ((IDisposable)prepared).Dispose(); }
    }
    static void SchemaDefaults()
    {
        var xml = new XmlDocument { XmlResolver = null };
        xml.LoadXml("<AudioEventLimitGroup xmlns='uri:ea.com:eala:asset' id='PreparedArmor'/>");
        using (var reader = XmlReader.Create(new StringReader(
            "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='uri:ea.com:eala:asset'>" +
            "<xs:element name='AudioEventLimitGroup'><xs:complexType><xs:attribute name='id' type='xs:string'/>" +
            "<xs:attribute name='Limit' type='xs:string' default='100'/></xs:complexType></xs:element></xs:schema>")))
            xml.Schemas.Add(null, reader);
        xml.Validate(null);
        Require(xml.DocumentElement.Attributes["Limit"] != null && !xml.DocumentElement.Attributes["Limit"].Specified,
            "Fixture must contain a real schema-supplied unspecified default");
        var instance = new InstanceDeclaration(AssetDeclarationDocument.CreateBackendView(Namespaces()));
        instance.XmlNode = xml.DocumentElement;
        var plugin = new Recovered.XmlPriority.Plugin(); plugin.Initialize(null, TargetPlatform.Win32);
        Metadata(instance, plugin);
        object prepared = Prepare(instance, plugin);
        try
        {
            var frozen = Value<InstanceDeclaration>(prepared, "Declaration");
            Require(frozen.Node.Attributes["Limit"] != null && frozen.Node.Attributes["Limit"].Value == "100",
                "The actual backend input must preserve schema default values during detachment");
            Require(Execute(prepared) != null, "The detached default-bearing XML declaration must execute through the real backend");
        }
        finally { ((IDisposable)prepared).Dispose(); }
    }
    static long PreparationEstimate(InstanceDeclaration instance)
    {
        return (long)contract.GetMethod("EstimatePreparationBytes", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { instance });
    }
    static void PreparationBudget()
    {
        var plugin = new Recovered.XmlPriority.Plugin(); plugin.Initialize(null, TargetPlatform.Win32);
        var declaration = Declaration("AudioEventLimitGroup", "PreparationBudget", Namespaces());
        Metadata(declaration, plugin);
        long small = PreparationEstimate(declaration);
        var budgetArguments = new object[] { declaration, plugin, 0L, null };
        Require(!(bool)contract.GetMethod("TryPrepareWithinBudget").Invoke(null, budgetArguments) && budgetArguments[3] == null,
            "A zero observed preparation budget must reject before allocating a detached private representation");

        Require(small >= 4096, "Preparation estimate must cover even a small private DOM");
        // Use an otherwise ordinary metadata string. Preparation's size gate
        // precedes backend field validation and must not clone or serialize it.
        ((XmlElement)declaration.Node).SetAttribute("id", new string('x', 20 * 1024 * 1024));
        long large = PreparationEstimate(declaration);
        Require(large >= 20L * 1024L * 1024L * 8L && large > 128L * 1024L * 1024L,
            "A large single attribute must be included without relying on element count or source file size");
        Require(Prepare(declaration, plugin) == null, "Oversized preparation must use the owner compatibility path before cloning");
    }
    sealed class UncertifiedAudioPlugin : BinaryAssetBuilder.AudioCompiler.Plugin { }
    static void PreparationRejectionReasons(string work)
    {
        const long budget = 128L * 1024L * 1024L;
        var xml = new Recovered.XmlPriority.Plugin(); xml.Initialize(null, TargetPlatform.Win32);
        var audio = new BinaryAssetBuilder.AudioCompiler.Plugin(); audio.Initialize(null, TargetPlatform.Win32);
        var apt = Declaration("AptAptData", "RejectedApt", Namespaces()); Metadata(apt, xml);
        RejectedPreparation(null, xml, budget, "invalid-input");
        RejectedPreparation(apt, null, budget, "invalid-input");
        RejectedPreparation(apt, new UncertifiedAudioPlugin(), budget, "uncertified-plugin");
        RejectedPreparation(apt, xml, 0, "preparation-budget-exceeded");
        ((XmlElement)apt.Node).SetAttribute("File", "");
        RejectedPreparation(apt, xml, budget, "external-input-path-missing");
        ((XmlElement)apt.Node).SetAttribute("File", Path.Combine(work, "absent-prepared.bin"));
        RejectedPreparation(apt, xml, budget, "external-input-unavailable");
        ((XmlElement)apt.Node).SetAttribute("File", Path.Combine(work, "non-ascii-\u4e2d.bin"));
        RejectedPreparation(apt, xml, budget, "native-path-non-ascii");

        string path = Path.Combine(work, "reason-input.bin"); File.WriteAllBytes(path, new byte[] { 1, 2, 3, 4 });
        ((XmlElement)apt.Node).SetAttribute("File", path);
        using (var denied = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.None))
            RejectedPreparation(apt, xml, budget, "external-input-unavailable");
        string reason;
        object accepted = PrepareWithReason(apt, xml, budget, out reason);
        try
        {
            Require(accepted != null && reason == null, "A successful retry must clear the previous refusal");
            Denied(delegate { File.Delete(path); }, "deletion after a successful reason-aware preparation");
        }
        finally { if (accepted != null) ((IDisposable)accepted).Dispose(); }

        var texture = Declaration("TestTexture", "RejectedTexture", Namespaces()); Metadata(texture, xml);
        RejectedPreparation(texture, xml, budget, "uncertified-xml-type");
        using (var released = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.None))
            Require(released.CanWrite, "Refused unreviewed XML must never retain external leases");

        string target = Path.Combine(work, "reason-junction-target"); Directory.CreateDirectory(target);
        File.WriteAllBytes(Path.Combine(target, "input.bin"), new byte[] { 5, 6 });
        string junction = Path.Combine(work, "reason-junction");
        var start = new ProcessStartInfo("cmd.exe", "/d /c mklink /J \"" + junction + "\" \"" + target + "\"") {
            UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
        using (var process = Process.Start(start))
        {
            string stdout = process.StandardOutput.ReadToEnd(), stderr = process.StandardError.ReadToEnd();
            process.WaitForExit(); Require(process.ExitCode == 0, "Preparation-reason junction fixture: " + stdout + stderr);
        }
        try
        {
            ((XmlElement)apt.Node).SetAttribute("File", Path.Combine(junction, "input.bin"));
            RejectedPreparation(apt, xml, budget, "external-input-reparse-point");
        }
        finally { Directory.Delete(junction); }

        var audioFile = Declaration("AudioFile", "RejectedAudio", Namespaces()); Metadata(audioFile, audio);
        ((XmlElement)audioFile.Node).SetAttribute("File", path);
        var unknown = new BinaryAssetBuilder.AudioCompiler.Plugin(new UnknownAudioCompiler()); unknown.Initialize(null, TargetPlatform.Win32);
        RejectedPreparation(audioFile, unknown, budget, "uncertified-audio-compiler");
        ((XmlElement)audioFile.Node).SetAttribute("PCCompression", "EALAYER3");
        RejectedPreparation(audioFile, audio, budget, "audio-compression-diagnostic");
        ((XmlElement)audioFile.Node).SetAttribute("PCCompression", "NONE");
        ((XmlElement)audioFile.Node).RemoveAttribute("File");
        RejectedPreparation(audioFile, audio, budget, "external-input-path-missing");
        RejectedPreparation(apt, audio, budget, "uncertified-audio-type");

        var music = Declaration("PathMusicEvent", "RejectedMusic", Namespaces()); Metadata(music, audio);
        RejectedPreparation(music, audio, budget, "uncertified-audio-type");
        File.Delete(path);
        lifecycleEvidence.Add(new { kind = "preparation-rejected-reasons", firstActualGate = true,
            reasons = new[] { "invalid-input", "uncertified-plugin", "preparation-budget-exceeded", "external-input-path-missing",
                "external-input-unavailable", "native-path-non-ascii", "external-input-reparse-point", "uncertified-audio-compiler",
                "audio-compression-diagnostic", "uncertified-audio-type" },
            rejectedUnknownXmlWithoutLeases = true, successfulRetryReasonCleared = true,
            scope = "Windows production host; unsupported-platform remains a source-certified gate" });
    }
    sealed class BufferWork
    {
        public string Type;
        public IAssetBuilderPlugin Plugin;
        public InstanceDeclaration Owner;
        public object Prepared;
        public AssetBuffer Expected,Actual;
        public long Start,End;
        public int ThreadId;
    }
    static void WorkerWave(List<BufferWork> items,string label)
    {
        int next=-1;
        int count=Math.Max(1,Math.Min(4,Environment.ProcessorCount));
        var tasks=new List<Task>();
        var intervals=new List<object>();
        using(var ready=new CountdownEvent(count))
        using(var begin=new ManualResetEventSlim())
        {
            try
            {
                for(int i=0;i<count;i++)
                    tasks.Add(Task.Factory.StartNew(delegate {
                        ready.Signal(); begin.Wait();
                        for(int index=Interlocked.Increment(ref next);index<items.Count;index=Interlocked.Increment(ref next))
                        {
                            var item=items[index]; item.ThreadId=Thread.CurrentThread.ManagedThreadId;
                            item.Start=Stopwatch.GetTimestamp();
                            try { item.Actual=Execute(item.Prepared); }
                            finally { item.End=Stopwatch.GetTimestamp(); }
                        }
                    },CancellationToken.None,TaskCreationOptions.LongRunning,TaskScheduler.Default));
                ready.Wait(); begin.Set(); Task.WaitAll(tasks.ToArray());
                int maximum=0;
                foreach(var item in items)
                {
                    EqualBuffers(item.Expected,item.Actual,label+"/"+item.Type);
                    Require(item.ThreadId!=Thread.CurrentThread.ManagedThreadId,"Backend invocation must run on a real worker");
                    if(item.Owner.HasCustomData)
                    {
                        string path=Value<InstanceDeclaration>(item.Prepared,"Declaration").CustomDataPath;
                        byte[] left=File.ReadAllBytes(item.Owner.CustomDataPath),right=File.ReadAllBytes(path);
                        Require(left.Length==right.Length,"CDATA length differs: "+item.Type);
                        for(int i=0;i<left.Length;i++) if(left[i]!=right[i]) throw new InvalidOperationException("CDATA raw byte differs: "+item.Type+"/"+i);
                        checks++;
                    }
                    int overlap=0;
                    foreach(var other in items) if(other.Start<=item.Start && other.End>item.Start) overlap++;
                    maximum=Math.Max(maximum,overlap);
                    intervals.Add(new {type=item.Type,startTicks=item.Start,endTicks=item.End,threadId=item.ThreadId});
                }
                if(label=="audio" && count>1) Require(maximum>1,"Four substantial PCM encodes must show actual worker interval overlap on a multi-CPU host");
                lifecycleEvidence.Add(new {kind="prepared-worker-wave",family=label,workerCount=count,maximumConcurrency=maximum,intervals,clockFrequency=Stopwatch.Frequency});
            }
            finally
            {
                begin.Set();
                try { Task.WaitAll(tasks.ToArray()); }
                finally { foreach(var item in items) ((IDisposable)item.Prepared).Dispose(); }
            }
        }
    }
    static BufferWork BufferItem(string work,string label,IAssetBuilderPlugin plugin,InstanceDeclaration owner)
    {
        Metadata(owner,plugin);
        owner.CustomDataPath=Path.Combine(work,label+"-owner.cdata");
        object prepared=Prepare(owner,plugin);
        Require(prepared!=null,"Worker fixture must be certified: "+label);
        Value<InstanceDeclaration>(prepared,"Declaration").CustomDataPath=Path.Combine(work,label+"-worker.cdata");
        return new BufferWork {Type=owner.Handle.TypeName,Plugin=plugin,Owner=owner,Prepared=prepared,Expected=plugin.ProcessInstance(owner)};
    }
    static void WorkerBuffers(string work,string sdk)
    {
        var xml=new Recovered.XmlPriority.Plugin(); xml.Initialize(null,TargetPlatform.Win32);
        var apt=new List<BufferWork>();
        string blob=Path.Combine(work,"all-apt-workers.bin");
        byte[] data=new byte[1024*1024];for(int i=0;i<data.Length;i++)data[i]=(byte)(i*73+19); File.WriteAllBytes(blob,data);
        foreach(string type in new[]{"AptAptData","AptConstData","AptDatData","AptGeometryData"})
        {
            var owner=Declaration(type,"Worker"+type,Namespaces()); ((XmlElement)owner.Node).SetAttribute("File",blob);
            apt.Add(BufferItem(work,type,xml,owner));
        }
        WorkerWave(apt,"apt");
        var w3xPlugin=new Recovered.Compiler.W3x.NativePlugin();w3xPlugin.Initialize(null,TargetPlatform.Win32);
        var w3x=new List<BufferWork>();
        for(int i=0;i<4;i++)
        {
            string type=i<2?"W3DAnimation":"W3DMesh";
            var owner=Declaration(type,"WorkerW3x"+i,Namespaces());
            if(i<2)
            {
                ((XmlElement)owner.Node).SetAttribute("NumFrames","4096"); ((XmlElement)owner.Node).SetAttribute("FrameRate","30");
                owner.Node.InnerXml="<Channels xmlns='uri:ea.com:eala:asset'><ChannelScalar Type='XTranslation' TypeId='1355976437' Pivot='0' FirstFrame='0'><Frame>0</Frame><Frame>1</Frame></ChannelScalar></Channels>";
            }
            w3x.Add(BufferItem(work,"w3x"+i,w3xPlugin,owner));
        }
        WorkerWave(w3x,"w3x");
        var audioPlugin=new BinaryAssetBuilder.AudioCompiler.Plugin();audioPlugin.Initialize(null,TargetPlatform.Win32);
        var audio=new List<BufferWork>();
        for(int i=0;i<4;i++)
        {
            string wave=Path.Combine(work,"worker"+i+".wav");
            using(var writer=new BinaryWriter(File.Create(wave)))
            {
                const int samples=262144;
                writer.Write(Encoding.ASCII.GetBytes("RIFF"));writer.Write(36+samples*2);
                writer.Write(Encoding.ASCII.GetBytes("WAVEfmt "));writer.Write(16);writer.Write((short)1);writer.Write((short)1);
                writer.Write(22050);writer.Write(44100);writer.Write((short)2);writer.Write((short)16);
                writer.Write(Encoding.ASCII.GetBytes("data"));writer.Write(samples*2);
                for(int n=0;n<samples;n++)writer.Write((short)(n*127+i));
            }
            var owner=Declaration("AudioFile","WorkerAudio"+i,Namespaces());
            ((XmlElement)owner.Node).SetAttribute("File",wave);((XmlElement)owner.Node).SetAttribute("PCCompression",i==0?"NONE":"XAS");
            audio.Add(BufferItem(work,"audio"+i,audioPlugin,owner));
        }
        WorkerWave(audio,"audio");
        string fixture=File.ReadAllText(Path.Combine(sdk,"../recovery/tests/AudioMp3Fixtures.cs"));
        var match=Regex.Match(fixture,"public const string Mono = \\\"([^\\\"]+)\\\"");
        Require(match.Success,"Existing real MP3 fixture must be present");
        string mp3=Path.Combine(work,"worker-passthrough.mp3");File.WriteAllBytes(mp3,Convert.FromBase64String(match.Groups[1].Value));
        var mp3Owner=Declaration("AudioFileMP3Passthrough","WorkerMp3",Namespaces());((XmlElement)mp3Owner.Node).SetAttribute("File",mp3);
        WorkerWave(new List<BufferWork>{BufferItem(work,"mp3",audioPlugin,mp3Owner)},"mp3");
    }
    public static int Main(string[] args)
    {
        try
        {
            Directory.CreateDirectory(args[0]);
            AptLeaseAndFreeze(args[0]); ParallelQualification(args[0]); EnvironmentQualification(args[4]);
            ResourceBudget(args[0]); W3xBudget(); SchemaDefaults(); PreparationBudget(); PreparationRejectionReasons(args[0]);
            WorkerBuffers(args[0],args[4]);
            File.WriteAllText(args[1],new JavaScriptSerializer().Serialize(new {
                succeeded=true,sourceCommit=args[2],runId=args[3],checks,lifecycleEvidence,
                scope="Actual isolated XML/defaults/namespaces, Windows stable file leases, all four APT roots and W3X/Audio worker three-buffer equality, Audio CDATA, closed route qualification, resource policy and pre-clone budgets; component evidence only"
            }));
            Console.WriteLine("PASS PreparedBackendWork: "+checks+" assertions"); return 0;
        }
        catch(Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
