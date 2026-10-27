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

public static class CompilerDocumentSnapshots
{
    const string Disable = "NewSdk.Compiler.DisableDocumentSnapshots", Ns = "uri:ea.com:eala:asset";
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
    static InstanceDeclaration Instance(AssetDeclarationDocument document, string name)
    {
        var xml = new XmlDocument(); xml.LoadXml("<ArmorTemplate id='" + name + "' inheritFrom='Base'/>");
        var instance = new InstanceDeclaration(document) { XmlNode = xml.DocumentElement };
        instance.ProcessingHash = 29; instance.InheritFromXmlHash = 31; instance.PrevalidationXmlHash = 37;
        instance.ReferencedInstances.Add(new InstanceHandle("ArmorTemplate", "Strong"));
        document.SelfInstances.Add(instance); return instance;
    }
    static string Xml(AssetDeclarationDocument document)
    {
        var text = new System.Text.StringBuilder();
        using (var writer = XmlWriter.Create(text, new XmlWriterSettings { OmitXmlDeclaration = true })) document.WriteXml(writer);
        return text.ToString();
    }
    static AssetDeclarationDocument Read(string xml)
    {
        var document = new AssetDeclarationDocument();
        using (var reader = XmlReader.Create(new StringReader(xml))) { reader.MoveToContent(); document.ReadXml(reader); }
        return document;
    }
    static void Transfer(AssetDeclarationDocument source, bool disabled, string path)
    {
        string before = Xml(source);
        var first = new AssetDeclarationDocument(); first.CacheFromDocument(source);
        // This second copy has no Current state. It must consume the saved
        // declaration rather than attempting CacheFromInstance(Current=null).
        var copy = new AssetDeclarationDocument(); copy.CacheFromDocument(first);
        Require(Xml(first) == before && Xml(copy) == before, "Copy changed persisted declaration XML: " + path);
        copy.FromLastHack(); source.FromLastHack();
        Require(copy.SelfInstances.Count == source.SelfInstances.Count, "Declaration count/order lost: " + path);
        Require(Object.ReferenceEquals(copy.SelfInstances[0], source.SelfInstances[0]) == disabled, "Unexpected declaration ownership: " + path);
        Require(disabled ? Object.ReferenceEquals(copy.SelfInstances[0].Document, source.SelfInstances[0].Document) : Object.ReferenceEquals(copy.SelfInstances[0].Document, copy), "Restored declaration bound to wrong document: " + path);
        copy.MakeComplete(); copy.SelfInstances[0].ProcessingHash ^= 0x31415926;
        copy.SelfInstances[0].ReferencedInstances[0].TypeHash ^= 0x27182818;
        copy.MakeCacheable();
        string after = Xml(source);
        Require((before == after) == !disabled, "MakeComplete/reserialize cache ownership mismatch: " + path);
        Require(Xml(copy) != before, "Receiver did not serialize its completed mutation: " + path);
        Require((Xml(first) == before) == !disabled, "Intermediate cached-only document was mutated: " + path);
        var roundtrip = Read(Xml(copy)); roundtrip.FromLastHack();
        Require(roundtrip.SelfInstances[0].ProcessingHash == copy.SelfInstances[0].ProcessingHash &&
            roundtrip.SelfInstances[0].InheritFromXmlHash == 31 && roundtrip.SelfInstances[0].PrevalidationXmlHash == 37,
            "Recaptured document lost saved scalar/inheritance metadata: " + path);
        evidence.Add(new { id = path, legacyFallback = disabled, sourceXmlChanged = before != after,
            scope = "Public Core cache-copy/restore/complete/serialize lifecycle; not a confirmed production CLI failure" });
    }
    static void PublicCacheBoundaries()
    {
        foreach (bool disabled in new[] { false, true })
        {
            AppContext.SetSwitch(Disable, disabled);
            var source = BareDocument("public"); Instance(source, "Owner"); Instance(source, "Second"); source.MakeCacheable();
            Transfer(source, disabled, "source-with-Last-and-Current");
            var saved = BareDocument("save-boundary"); Instance(saved, "Owner"); saved.MakeCacheable();
            string savedBefore = Xml(saved);
            saved.SelfInstances[0].MakeComplete(); saved.SelfInstances[0].ProcessingHash ^= 0x12345678;
            Require((savedBefore == Xml(saved)) == !disabled, "MakeCacheable retained mutable live declaration objects");
            saved.ResetState(); saved.FromLastHack();
            Require(saved.SelfInstances[0].Document == saved, "ResetState restore bound declaration to wrong document");
            var persisted = Read(Xml(source));
            Transfer(persisted, disabled, "XML-source-without-Current");
            var current = BareDocument("only-current"); Instance(current, "Owner");
            var captured = new AssetDeclarationDocument(); captured.CacheFromDocument(current);
            Require(Xml(captured).Length > 0, "CacheFromDocument failed with only Current");
            captured.FromLastHack(); Require(!Object.ReferenceEquals(current.SelfInstances[0], captured.SelfInstances[0]), "Current-only source unexpectedly shared declaration object");
            Require(captured.SelfInstances[0].Document == captured, "Current-only declaration did not bind to receiver");
        }
        AppContext.SetSwitch(Disable, false);
        var empty = new AssetDeclarationDocument(); empty.CacheFromDocument(new AssetDeclarationDocument());
        Require(Xml(empty) == "", "Empty source cache copy unexpectedly created state");
        var firstState = BareDocument("guard-one"); Instance(firstState, "Owner"); firstState.MakeCacheable();
        var target = new AssetDeclarationDocument(); target.CacheFromDocument(firstState); string original = Xml(target);
        var secondState = BareDocument("guard-two"); Instance(secondState, "Different"); target.CacheFromDocument(secondState);
        Require(Xml(target) == original, "Existing-target Current-only copy guard changed");
        tests.Add("public-Core-current-and-cached-only-document-copy-complete-serialize-lifecycle-with-legacy-reproduction");
    }
    sealed class DerivedInstance : InstanceDeclaration
    {
        public DerivedInstance(AssetDeclarationDocument document) : base(document) { }
    }
    sealed class DerivedInstanceSet : InstanceSet { }
    static void UnsupportedFallbacks()
    {
        foreach (bool derivedSet in new[] { false, true })
        {
            var source = BareDocument("derived");
            if (derivedSet)
            {
                object state = typeof(AssetDeclarationDocument).GetField("_Current", Hidden).GetValue(source);
                state.GetType().GetField("SelfInstances").SetValue(state, new DerivedInstanceSet());
                Instance(source, "Owner");
            }
            else
            {
                var xml = new XmlDocument(); xml.LoadXml("<ArmorTemplate id='Owner'/>");
                source.SelfInstances.Add(new DerivedInstance(source) { XmlNode = xml.DocumentElement });
            }
            source.MakeCacheable(); var copy = new AssetDeclarationDocument(); copy.CacheFromDocument(source); copy.FromLastHack();
            Require(Object.ReferenceEquals(copy.SelfInstances[0], source.SelfInstances[0]), "Derived declaration graph was sliced");
            copy.MakeComplete(); copy.MakeCacheable();
            var recopy = new AssetDeclarationDocument(); recopy.CacheFromDocument(copy); recopy.FromLastHack();
            Require(Object.ReferenceEquals(recopy.SelfInstances[0], source.SelfInstances[0]), "Derived runtime provenance lost after restore/recapture");
        }
        tests.Add("derived-declaration-and-instance-set-preserve-whole-legacy-graph-through-recapture");
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
        using (var plugins = BuiltinCompilers.Create(TargetPlatform.Win32))
        {
            Dictionary<string, string> expected = null;
            foreach (bool disabled in new[] { false, true })
            {
                AppContext.SetSwitch(Disable, disabled);
                string mode = disabled ? "legacy" : "owned";
                settings.OutputDirectory = Path.Combine(work, "real", mode + "-output"); settings.IntermediateOutputDirectory = Path.Combine(work, "real", mode + "-intermediate");
                settings.SessionCacheDirectory = Path.Combine(work, "real", mode + "-cache"); Directory.CreateDirectory(settings.SessionCacheDirectory);
                var schemas = new SchemaSet(false);
                var cold = Processor(plugins, schemas, false); Run(cold, settings.InputPath); var output = Four(settings.OutputDirectory);
                if (expected == null) expected = output; else Equal(expected, output);
                Run(cold, settings.InputPath); Equal(expected, Four(settings.OutputDirectory));
                ((SessionCache)cold.Cache).SaveCache(false);
                settings.OutputDirectory = Path.Combine(work, "real", mode + "-restored-output"); settings.IntermediateOutputDirectory = Path.Combine(work, "real", mode + "-restored-intermediate");
                var restored = Processor(plugins, schemas, true); Run(restored, settings.InputPath); Equal(expected, Four(settings.OutputDirectory));
                AssetDeclarationDocument document; restored.Cache.TryGetDocument(settings.InputPath, null, TargetPlatform.Win32, false, out document);
                Require(document != null && document.SelfInstances.Count == 3 && document.SelfInstances.Any(instance => instance.InheritFromHandle != null), "Actual persisted inheritance declarations were not restored");
                document.MakeCacheable(); string before = Xml(document);
                var copy = new AssetDeclarationDocument(); copy.CacheFromDocument(document); copy.FromLastHack();
                Require(copy.SelfInstances.Count == 3 && copy.SelfInstances[0].Handle.Equals(document.SelfInstances[0].Handle), "Real Core cache copy lost declaration order");
                copy.MakeComplete(); copy.SelfInstances[0].ProcessingHash ^= 0x12345678; copy.MakeCacheable();
                Require((before == Xml(document)) == !disabled, "Real Core cached document lifecycle did not isolate declarations");
                evidence.Add(new { id = "real-Core-session", legacyFallback = disabled, coldParse = cold.XmlParseCount, restoredParse = restored.XmlParseCount, fourOutputs = output,
                    mutationScope = "After successful compilation, direct public cache-copy/complete/reserialize probe" });
            }
        }
        AppContext.SetSwitch(Disable, false);
        tests.Add("real-SDK-Armor-inheritance-cold-warm-persisted-session-and-legacy-fallback-four-output-equality");
    }
    public static int Main(string[] args)
    {
        object result; int code = 0; Settings previous = Settings.Current; bool previousSwitch; AppContext.TryGetSwitch(Disable, out previousSwitch);
        try
        {
            Require(args.Length == 5 && IntPtr.Size == 4 && Path.DirectorySeparatorChar == '\\', "Expected Windows x86 work result source run SDK");
            work = Path.GetFullPath(args[0]); sdk = Path.GetFullPath(args[4]); Directory.CreateDirectory(work); AppContext.SetSwitch(Disable, false);
            PublicCacheBoundaries(); UnsupportedFallbacks(); RealCore();
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), evidence = evidence.ToArray(),
                scope = "Document declaration cache ownership across save/copy/restore/complete/recapture; derived graphs and opt-out remain legacy; not immutable DOM/SchemaInfo or a confirmed CLI fault" };
            Console.WriteLine("PASS DocumentSnapshots: " + tests.Count + " groups; " + checks + " checks");
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
