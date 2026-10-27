using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using NewModCompiler.Application;

public static class CompilerDeclarationSnapshots
{
    const string Disable = "NewSdk.Compiler.DisableDeclarationHandleSnapshots", Ns = "uri:ea.com:eala:asset";
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    static string work, sdk;
    static int checks;
    static readonly List<string> tests = new List<string>();
    static readonly List<object> evidence = new List<object>();
    static readonly JavaScriptSerializer Json = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
    static void Require(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static object Current(InstanceDeclaration instance) { return typeof(InstanceDeclaration).GetField("_Current", Hidden).GetValue(instance); }
    static void Field(InstanceDeclaration instance, string name, object value) { object current = Current(instance); current.GetType().GetField(name).SetValue(current, value); }
    static AssetDeclarationDocument BareDocument(string name)
    {
        var document = new AssetDeclarationDocument(); var type = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object current = Activator.CreateInstance(type, true); type.GetMethod("FromScratch").Invoke(current, new object[0]);
        type.GetField("SourcePath").SetValue(current, Path.Combine(work, name + ".xml"));
        type.GetField("SourceDirectory").SetValue(current, work); typeof(AssetDeclarationDocument).GetField("_Current", Hidden).SetValue(document, current);
        return document;
    }
    static InstanceDeclaration Instance(AssetDeclarationDocument document)
    {
        var xml = new XmlDocument(); xml.LoadXml("<ArmorTemplate id='Owner' inheritFrom='Base'/>");
        var instance = new InstanceDeclaration(document) { XmlNode = xml.DocumentElement };
        instance.Handle.TypeHash = 17; instance.Handle.InstanceHash = 19; instance.InheritFromHandle.TypeHash = 23;
        instance.ProcessingHash = 29; instance.InheritFromXmlHash = 31; instance.PrevalidationXmlHash = 37;
        var strong = new InstanceHandle("ArmorTemplate", "Strong") { TypeHash = 41, InstanceHash = 43 };
        instance.ReferencedInstances.Add(strong); instance.ReferencedInstances.Add(new InstanceHandle("ArmorTemplate", "Other")); instance.ReferencedInstances.Add(strong);
        instance.WeakReferencedInstances.Add(strong); instance.WeakReferencedInstances.Add(new InstanceHandle("UpgradeTemplate", "Weak"));
        return instance;
    }
    static string Xml(InstanceDeclaration instance)
    {
        var text = new System.Text.StringBuilder();
        using (var writer = XmlWriter.Create(text, new XmlWriterSettings { OmitXmlDeclaration = true })) instance.WriteXml(writer);
        return text.ToString();
    }
    static InstanceDeclaration Read(string xml, AssetDeclarationDocument document)
    {
        var instance = new InstanceDeclaration();
        using (var reader = XmlReader.Create(new StringReader(xml))) { reader.MoveToContent(); instance.ReadXml(reader); }
        instance.Initialize(document); return instance;
    }
    static void Mutate(InstanceDeclaration instance)
    {
        instance.Handle.TypeName = "ChangedOwnerType"; instance.Handle.TypeHash ^= 0x11111111; instance.Handle.InstanceHash ^= 0x22222222;
        instance.ReferencedInstances[0].TypeName = "ChangedStrongType"; instance.ReferencedInstances[0].TypeHash ^= 0x33333333;
        instance.WeakReferencedInstances[1].InstanceHash ^= 0x44444444;
        instance.InheritFromHandle.TypeName = "ChangedBaseType"; instance.InheritFromHandle.TypeHash ^= 0x55555555;
    }
    static void PublicCacheBoundaries()
    {
        foreach (bool disabled in new[] { false, true })
        {
            AppContext.SetSwitch(Disable, disabled);
            foreach (bool copy in new[] { false, true })
            {
                var document = BareDocument("capture"); var source = Instance(document);
                var cached = copy ? new InstanceDeclaration() : source;
                if (copy) cached.CacheFromInstance(source); else source.MakeCacheable();
                string before = Xml(cached); Mutate(source); string after = Xml(cached);
                Require((before == after) == !disabled, "Prepared public Core declaration snapshot changed ownership incorrectly: " + disabled + "/" + copy);
                evidence.Add(new { id = copy ? "CacheFromInstance" : "MakeCacheable", legacyFallback = disabled, cacheXmlChangedAfterCurrentMutation = before != after,
                    scope = "Direct public Core cache-declaration boundary; this is not evidence of a production CLI failure" });
            }
            var restored = Read(Xml(Instance(BareDocument("serialize"))), BareDocument("restored"));
            string serialized = Xml(restored); Mutate(restored);
            Require((Xml(restored) == serialized) == !disabled, "Restored Current wrote through its cached declaration graph");
        }
        AppContext.SetSwitch(Disable, false);
        tests.Add("public-Core-cachefrom-makecacheable-and-XML-restore-mutation-probes-default-value-ownership-legacy-reproduction");
    }
    static void AliasesAndStoredIds()
    {
        var document = BareDocument("aliases"); var source = Instance(document);
        var stored = new InstanceHandle();
        using (var reader = XmlReader.Create(new StringReader("<ih d='12345;67890;54321;9876;ArmorTemplate;StoredOwner'/>") )) { reader.MoveToContent(); stored.ReadXml(reader); }
        Field(source, "Handle", stored); Field(source, "InheritFromHandle", stored);
        source.ReferencedInstances.Clear(); source.WeakReferencedInstances.Clear();
        source.ReferencedInstances.Add(stored); source.ReferencedInstances.Add(null); source.ReferencedInstances.Add(stored); source.ReferencedInstances.Add(new InstanceHandle(stored));
        source.WeakReferencedInstances.Add(stored); source.WeakReferencedInstances.Add(null);
        var cached = new InstanceDeclaration(); cached.CacheFromInstance(source); cached.Initialize(document);
        Require(!Object.ReferenceEquals(cached.Handle, stored) && Object.ReferenceEquals(cached.Handle, cached.InheritFromHandle) &&
            Object.ReferenceEquals(cached.Handle, cached.ReferencedInstances[0]) && Object.ReferenceEquals(cached.ReferencedInstances[0], cached.ReferencedInstances[2]) &&
            Object.ReferenceEquals(cached.Handle, cached.WeakReferencedInstances[0]), "Copying a graph broke its cross-list alias relationships");
        Require(cached.ReferencedInstances.Count == 4 && cached.WeakReferencedInstances.Count == 2 && cached.ReferencedInstances[1] == null && cached.WeakReferencedInstances[1] == null &&
            !Object.ReferenceEquals(cached.ReferencedInstances[0], cached.ReferencedInstances[3]), "Strong duplicates/order/null or equal-valued distinct identities were collapsed");
        Require(cached.Handle.TypeId == 12345 && cached.Handle.TypeHash == 67890 && cached.Handle.InstanceId == 54321 && cached.Handle.InstanceHash == 9876 &&
            cached.Handle.TypeName == "ArmorTemplate" && cached.Handle.InstanceName == "StoredOwner", "Snapshot rehashed names or lost stored legacy identifiers");
        cached.Handle.InstanceHash = 1; stored.TypeHash = 2;
        Require(cached.ReferencedInstances[2].InstanceHash == 1 && stored.InstanceHash == 9876 && cached.Handle.TypeHash == 67890,
            "Copied aliases failed to stay local to their own mutable Current graph");
        var empty = Instance(document); empty.ReferencedInstances.Clear(); empty.WeakReferencedInstances.Clear();
        var emptyCopy = Read(Xml(empty), document);
        Require(emptyCopy.ReferencedInstances.Count == 0 && emptyCopy.WeakReferencedInstances.Count == 0, "XML null collections did not restore legacy empty lists");
        tests.Add("exact-base-handle-graph-reference-aliases-duplicate-order-null-elements-and-raw-32bit-id-preservation");
    }
    sealed class DerivedHandle : InstanceHandle
    {
        internal static int VirtualCalls;
        internal DerivedHandle() : base("ArmorTemplate", "Derived") { }
        public override bool Equals(object other) { VirtualCalls++; return base.Equals(other); }
        public override int GetHashCode() { VirtualCalls++; return base.GetHashCode(); }
        public override string ToString() { VirtualCalls++; return "custom-derived-handle"; }
    }
    sealed class DerivedList : List2<InstanceHandle> { }
    static void UnsupportedFallbacks()
    {
        foreach (string kind in new[] { "own", "inheritance", "strong", "weak", "derived-list" })
        {
            var document = BareDocument("fallback"); var source = Instance(document); var derived = new DerivedHandle();
            if (kind == "own") Field(source, "Handle", derived);
            if (kind == "inheritance") Field(source, "InheritFromHandle", derived);
            if (kind == "strong") source.ReferencedInstances.Add(derived);
            if (kind == "weak") source.WeakReferencedInstances.Add(derived);
            if (kind == "derived-list") { var values = new DerivedList(); values.Add(source.ReferencedInstances[0]); Field(source, "ReferencedInstances", values); }
            DerivedHandle.VirtualCalls = 0;
            var cached = new InstanceDeclaration(); cached.CacheFromInstance(source); cached.Initialize(document);
            Require(Object.ReferenceEquals(cached.Handle, source.Handle) && Object.ReferenceEquals(cached.InheritFromHandle, source.InheritFromHandle) &&
                Object.ReferenceEquals(cached.ReferencedInstances[0], source.ReferencedInstances[0]), "Unsupported graph was partially cloned or sliced: " + kind);
            Require(DerivedHandle.VirtualCalls == 0, "Snapshot classification invoked custom Equals/GetHashCode/ToString: " + kind);
            if (kind == "own") Require(cached.Handle is DerivedHandle && cached.Handle.ToString() == "custom-derived-handle", "Derived custom behavior did not survive legacy fallback");
        }
        tests.Add("any-derived-handle-or-list-keeps-whole-legacy-graph-without-virtual-method-probing-or-slicing");
    }
    static Settings Configure()
    {
        var config = new XmlDocument(); config.Load(Path.Combine(sdk, "tools/BinaryAssetBuilder.exe.config"));
        var settings = (Settings)new SerializedSectionHandler().Create(null, null, config.SelectSingleNode("/configuration/assetbuilder"));
        settings.DataRoot = work; settings.SchemaPath = Path.Combine(sdk, "Schemas/xsd/CnC3Types.xsd"); settings.InputPath = Path.Combine(work, "real", "root.xml");
        settings.OutputDirectory = Path.Combine(work, "real", "output"); settings.IntermediateOutputDirectory = Path.Combine(work, "real", "intermediate");
        settings.SessionCacheDirectory = Path.Combine(work, "real", "cache"); settings.DefaultDataPaths = work; settings.DefaultArtPaths = work; settings.DefaultAudioPaths = work;
        settings.TargetPlatform = TargetPlatform.Win32; settings.BuildCache = false; settings.SessionCache = false; settings.LinkedStreams = true;
        settings.OutputAssetReport = false; settings.OutputStringHashes = false; settings.ErrorLevel = 1; settings.UsePrecompiled = false; settings.StreamHints = false; settings.StableSort = false;
        Settings.Current = settings; SettingsLoader.PostProcessSettings(work); Directory.CreateDirectory(settings.OutputDirectory);
        Directory.CreateDirectory(settings.IntermediateOutputDirectory); Directory.CreateDirectory(settings.SessionCacheDirectory); return settings;
    }
    static DocumentProcessor Processor(BuiltinCompilers plugins, SchemaSet schemas, bool restore)
    {
        HashProvider.InitializeStringHashes(Settings.Current.SessionCacheDirectory);
        var cache = new SessionCache();
        cache.LoadCache(Path.Combine(Settings.Current.SessionCacheDirectory, "declarations.xml"));
        cache.InitializeCache(new List<string>());
        var verifiers = new VerifierPluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32);
        verifiers.DefaultPlugin = (IAssetBuilderVerifierPlugin)Activator.CreateInstance(typeof(DocumentProcessor).Assembly.GetType("BinaryAssetBuilder.GameDataVerifier.Plugin", true), true);
        verifiers.DefaultPlugin.Initialize(null, TargetPlatform.Win32);
        return new DocumentProcessor(Settings.Current, plugins.Registry, verifiers) { Cache = cache, SchemaSet = schemas };
    }
    static void Run(DocumentProcessor processor, string path) { bool success; processor.ProcessDocument(path, true, false, out success); Require(success, "Real Core declaration run failed"); }
    static Dictionary<string, string> Four(string root)
    {
        var result = new Dictionary<string, string>();
        foreach (string path in Directory.GetFiles(root, "*", SearchOption.AllDirectories))
            if (new[] { ".manifest", ".bin", ".imp", ".relo" }.Contains(Path.GetExtension(path)))
                using (var sha = SHA256.Create()) result.Add(Path.GetExtension(path), Convert.ToBase64String(sha.ComputeHash(File.ReadAllBytes(path))));
        Require(result.Count == 4, "Expected four actual Core outputs"); return result;
    }
    static void Equal(Dictionary<string, string> expected, Dictionary<string, string> actual)
    { Require(expected.Count == actual.Count && expected.All(pair => actual.ContainsKey(pair.Key) && actual[pair.Key] == pair.Value), "Declaration ownership changed actual Core four outputs"); }
    static void RealCore()
    {
        var settings = Configure(); Directory.CreateDirectory(Path.GetDirectoryName(settings.InputPath));
        File.Copy(Path.Combine(sdk, "Mods/mymod/Data/RecoveryArmorProbe.xml"), settings.InputPath);
        var schemas = new SchemaSet(false);
        using (var plugins = BuiltinCompilers.Create(TargetPlatform.Win32))
        {
            Dictionary<string, string> expected = null;
            foreach (bool disabled in new[] { false, true })
            {
                AppContext.SetSwitch(Disable, disabled);
                string mode = disabled ? "legacy" : "owned";
                settings.OutputDirectory = Path.Combine(work, "real", mode + "-output"); settings.IntermediateOutputDirectory = Path.Combine(work, "real", mode + "-intermediate");
                settings.SessionCacheDirectory = Path.Combine(work, "real", mode + "-cache"); Directory.CreateDirectory(settings.SessionCacheDirectory);
                var cold = Processor(plugins, schemas, false); Run(cold, settings.InputPath); var output = Four(settings.OutputDirectory);
                if (expected == null) expected = output; else Equal(expected, output);
                ((SessionCache)cold.Cache).SaveCache(false);
                settings.OutputDirectory = Path.Combine(work, "real", mode + "-restored-output"); settings.IntermediateOutputDirectory = Path.Combine(work, "real", mode + "-restored-intermediate");
                var restored = Processor(plugins, schemas, true); Run(restored, settings.InputPath); Equal(expected, Four(settings.OutputDirectory));
                AssetDeclarationDocument document; restored.Cache.TryGetDocument(settings.InputPath, null, TargetPlatform.Win32, false, out document);
                Require(document != null && document.SelfInstances.Count == 3 && document.SelfInstances.Any(instance => instance.InheritFromHandle != null), "Actual persisted inheritance declarations were not restored");
                evidence.Add(new { id = "real-Core-session", legacyFallback = disabled, coldParse = cold.XmlParseCount, restoredParse = restored.XmlParseCount, fourOutputs = output });
            }
        }
        AppContext.SetSwitch(Disable, false);
        tests.Add("real-SDK-Armor-inheritance-cold-persisted-session-restore-and-legacy-fallback-four-output-equality");
    }
    public static int Main(string[] args)
    {
        object result; int code = 0; Settings previous = Settings.Current; bool previousSwitch; AppContext.TryGetSwitch(Disable, out previousSwitch);
        try
        {
            Require(args.Length == 5 && IntPtr.Size == 4 && Path.DirectorySeparatorChar == '\\', "Expected Windows x86 work result source run SDK");
            work = Path.GetFullPath(args[0]); sdk = Path.GetFullPath(args[4]); Directory.CreateDirectory(work); AppContext.SetSwitch(Disable, false);
            PublicCacheBoundaries(); AliasesAndStoredIds(); UnsupportedFallbacks(); RealCore();
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), evidence = evidence.ToArray(),
                scope = "Exact-base-handle cache declaration value snapshots and restore only; derived graphs and opt-out remain legacy; not immutable DOM/Define state or confirmed CLI fault" };
            Console.WriteLine("PASS DeclarationSnapshots: " + tests.Count + " groups; " + checks + " checks");
        }
        catch (Exception error)
        {
            code = 1; Console.Error.WriteLine(error); result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null, runId = args.Length > 3 ? args[3] : null,
                checks, tests = tests.ToArray(), evidence = evidence.ToArray(), error = error.ToString() };
        }
        finally { Settings.Current = previous; AppContext.SetSwitch(Disable, previousSwitch); }
        if (args.Length > 1) File.WriteAllText(args[1], Json.Serialize(result)); return code;
    }
}
