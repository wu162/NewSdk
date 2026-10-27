using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using NewModCompiler.Application;

public static class CompilerUpheavalSamples
{
    const string Disable = "NewSdk.Compiler.DisableDocumentSnapshots";
    const string MetadataDisable = "NewSdk.Compiler.DisableSharedDocumentMetadata";
    static string work, sdk, mod;
    static int checks;
    static readonly List<string> tests = new List<string>();
    static readonly List<object> evidence = new List<object>();
    static readonly List<object> baselines = new List<object>();
    static readonly JavaScriptSerializer Json = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
    static void Require(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static string Hash(string path)
    {
        using (var sha = SHA256.Create()) return String.Concat(sha.ComputeHash(File.ReadAllBytes(path)).Select(value => value.ToString("x2")));
    }
    static void CopyTree(string source, string target)
    {
        Directory.CreateDirectory(target);
        foreach (string path in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
        {
            string copy = Path.Combine(target, path.Substring(source.Length).TrimStart(Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(copy)); File.Copy(path, copy);
        }
    }
    static object VerifyOriginals()
    {
        var provenance = (Dictionary<string, object>)Json.DeserializeObject(File.ReadAllText(Path.Combine(mod, "provenance.json")));
        Require((string)provenance["sourceCommit"] == "068643219fded09a05fb7b75c6507b769c430316", "Unexpected fixed Upheaval source commit");
        var originals = (object[])provenance["originals"];
        Require(originals.Length == 4, "Expected four whole original Upheaval files");
        foreach (Dictionary<string, object> original in originals)
        {
            string path = Path.Combine(mod, (string)original["path"]);
            Require(Hash(path) == (string)original["sha256"] && new FileInfo(path).Length == Convert.ToInt64(original["bytes"]), "Original Upheaval file bytes changed");
        }
        return provenance;
    }
    static Settings Configure(string mode)
    {
        var config = new System.Xml.XmlDocument(); config.Load(Path.Combine(sdk, "tools/BinaryAssetBuilder.exe.config"));
        var settings = (Settings)new SerializedSectionHandler().Create(null, null, config.SelectSingleNode("/configuration/assetbuilder"));
        settings.InputPath = Path.Combine(mod, "Data/Mod.xml"); settings.SchemaPath = Path.Combine(sdk, "Schemas/xsd/CnC3Types.xsd");
        settings.DataRoot = sdk; settings.DefaultDataPaths = Path.Combine(mod, "Data") + ";" + settings.DefaultDataPaths;
        settings.DefaultArtPaths = Path.Combine(mod, "Art") + ";" + settings.DefaultArtPaths;
        settings.DefaultAudioPaths = Path.Combine(mod, "Audio") + ";" + settings.DefaultAudioPaths;
        string output = Path.Combine(work, mode);
        settings.OutputDirectory = Path.Combine(output, "streams"); settings.IntermediateOutputDirectory = Path.Combine(output, "intermediate");
        settings.SessionCacheDirectory = Path.Combine(output, "cache"); settings.BuildCacheDirectory = Path.Combine(output, "buildcache");
        settings.BuildConfigurations = new BuildConfiguration[0]; settings.BuildConfigurationName = null;
        settings.TargetPlatform = TargetPlatform.Win32; settings.BuildCache = false; settings.SessionCache = false;
        settings.PartialSessionCache = settings.FreezeSessionCache = settings.CompressedSessionCache = settings.AlwaysTouchCache = settings.StreamHints = false;
        settings.UsePrecompiled = settings.StableSort = settings.ForceSlowCleanup = settings.LinkedStreams = true;
        settings.GuiMode = settings.Resident = settings.PauseOnError = settings.MetricsReporting = false;
        settings.OldOutputFormat = settings.VersionFiles = settings.OutputAssetReport = settings.OutputStringHashes = settings.OutputIntermediateXml = settings.SingleFile = false;
        settings.CustomPostfix = settings.StreamPostfix = ""; settings.BasePatchStream = null;
        settings.TraceLevel = 3; settings.ErrorLevel = 0;
        Settings.Current = settings; SettingsLoader.PostProcessSettings(Path.Combine(sdk, "tools"));
        Directory.CreateDirectory(settings.OutputDirectory); Directory.CreateDirectory(settings.IntermediateOutputDirectory); Directory.CreateDirectory(settings.SessionCacheDirectory);
        return settings;
    }
    static DocumentProcessor Processor(BuiltinCompilers plugins, SchemaSet schemas)
    {
        var cache = new SessionCache(); cache.LoadCache(Path.Combine(Settings.Current.SessionCacheDirectory, "session.xml")); cache.InitializeCache(new List<string>());
        var verifiers = new VerifierPluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32);
        verifiers.DefaultPlugin = (IAssetBuilderVerifierPlugin)Activator.CreateInstance(typeof(DocumentProcessor).Assembly.GetType("BinaryAssetBuilder.GameDataVerifier.Plugin", true), true);
        verifiers.DefaultPlugin.Initialize(null, TargetPlatform.Win32);
        return new DocumentProcessor(Settings.Current, plugins.Registry, verifiers) { Cache = cache, SchemaSet = schemas };
    }
    static void Run(DocumentProcessor processor, string path)
    {
        bool success; processor.ProcessDocument(path, true, false, out success); Require(success, "Real Upheaval small Core compilation failed");
    }
    static Dictionary<string, string> Four(string root)
    {
        var result = new Dictionary<string, string>();
        foreach (string path in Directory.GetFiles(root, "*", SearchOption.AllDirectories))
            if (new[] { ".manifest", ".bin", ".imp", ".relo" }.Contains(Path.GetExtension(path))) result.Add(Path.GetExtension(path).Substring(1), path);
        Require(result.Count == 4, "Expected one exact four-stream output set");
        string manifest = Encoding.ASCII.GetString(File.ReadAllBytes(result["manifest"]));
        foreach (string name in new[] { "ArmorTemplate:JapanArcherInfantryArmor", "ArmorTemplate:AlliedFutureTankArmor", "ArmorTemplate:SovietGrinderVehicleArmor",
            "UpgradeTemplate:Upgrade_FortressGeoAir", "UpgradeTemplate:Upgrade_FortressGeoWater", "ArmorTemplate:RecoveryUpheaval_FutureDerived", "ArmorTemplate:RecoveryUpheaval_ArcherDerived" })
            Require(manifest.Contains(name + "\0"), "Missing original or synthetic adapter asset: " + name);
        Require(BitConverter.ToUInt32(File.ReadAllBytes(result["manifest"]), 12) == 7, "Expected exactly seven small-group assets");
        return result;
    }
    static Dictionary<string, byte[]> Bytes(Dictionary<string, string> paths) { return paths.ToDictionary(pair => pair.Key, pair => File.ReadAllBytes(pair.Value)); }
    static void Equal(Dictionary<string, byte[]> expected, Dictionary<string, string> actual, string label)
    {
        Require(expected.Count == actual.Count && expected.All(pair => actual.ContainsKey(pair.Key) && pair.Value.SequenceEqual(File.ReadAllBytes(actual[pair.Key]))), "Upheaval four-stream bytes differ: " + label);
    }
    static void Matrix()
    {
        Dictionary<string, byte[]> expected = null;
        using (var plugins = BuiltinCompilers.Create(TargetPlatform.Win32))
        {
        // Use the same default metadata view as the production backend. The
        // bare explicit registry rejects unknown types and intentionally has a
        // zero global hash; it is not the production manifest metadata view.
        var metadata = typeof(CoreStreamCompiler).GetNestedType("DefaultMetadataPlugin", BindingFlags.NonPublic);
        plugins.Registry.DefaultPlugin = (IAssetBuilderPlugin)Activator.CreateInstance(metadata, new object[] {
            plugins.Registry.DefaultPlugin, plugins.Registry.GetPlugin(InstanceHandle.GetTypeId("AttributeModifier")) });
        Require(plugins.Registry.DefaultPlugin.GetAllTypesHash() == 1424942948u && plugins.Registry.DefaultPlugin.GetVersionNumber() == 4u,
            "Sample registry does not use the actual production XML default metadata contract");
        foreach (bool metadataCopy in new[] { false, true })
        foreach (bool disabled in new[] { false, true })
        {
            AppContext.SetSwitch(Disable, disabled); AppContext.SetSwitch(MetadataDisable, metadataCopy);
            string mode = (metadataCopy ? "metadata-copy-" : "metadata-shared-") + (disabled ? "fallback" : "owned"); var settings = Configure(mode);
            using (HashProvider.BeginStringHashScope(settings.SessionCacheDirectory))
            using (DocumentProcessor.BeginStateScope())
            {
                var schemas = new SchemaSet(settings.StableSort);
                int coldParse, warmParse;
                var processor = Processor(plugins, schemas);
                {
                    Run(processor, settings.InputPath); coldParse = processor.XmlParseCount;
                    var output = Four(settings.OutputDirectory); if (expected == null) expected = Bytes(output); else Equal(expected, output, mode + "-cold");
                    if (!disabled && !metadataCopy) baselines.Add(new { name = "upheaval-small", modRoot = mod, sdkRoot = sdk, inputPath = settings.InputPath,
                        stagingDirectory = Path.Combine(work, mode), outputs = output,
                        scope = "Four whole original Upheaval files (three Armor, two Upgrade assets) plus explicitly synthetic include/Define/inheritance adapter; not complete Upheaval" });
                    // Reinitialize the same in-memory session generation, so this
                    // warm run actually relinks into a fresh stream directory.
                    // A Complete-document no-op would only re-read cold outputs.
                    ((SessionCache)processor.Cache).SaveCache(false);
                    ((SessionCache)processor.Cache).InitializeCache(new List<string>());
                    settings.OutputDirectory = Path.Combine(work, mode + "-warm", "streams"); Directory.CreateDirectory(settings.OutputDirectory);
                    Run(processor, settings.InputPath); warmParse = processor.XmlParseCount; Equal(expected, Four(settings.OutputDirectory), mode + "-warm");
                    ((SessionCache)processor.Cache).SaveCache(false);
                }
                settings.OutputDirectory = Path.Combine(work, mode + "-restored", "streams");
                settings.IntermediateOutputDirectory = Path.Combine(work, mode + "-restored", "intermediate");
                Directory.CreateDirectory(settings.OutputDirectory); Directory.CreateDirectory(settings.IntermediateOutputDirectory);
                var restored = Processor(plugins, schemas);
                {
                    Run(restored, settings.InputPath); Equal(expected, Four(settings.OutputDirectory), mode + "-persisted-session");
                    AssetDeclarationDocument document;
                    string mixedSeparatorPath = Path.Combine(mod, "Data/Derived.xml");
                    // Include discovery stores ShPath-canonical keys. SessionCache
                    // lowercases keys but intentionally does not normalize '/'.
                    // Query the actual resolver key instead of creating a distinct
                    // file-only cache entry with an equivalent filesystem path.
                    string canonicalPath = ShPath.Canonicalize(Path.Combine(mod, "Data", "Derived.xml"));
                    bool fileExists = restored.Cache.TryGetDocument(canonicalPath, null, TargetPlatform.Win32, false, out document);
                    var declarations = document != null && document.State != DocumentState.None
                        ? document.SelfInstances.Select(instance => new { name = instance.Handle.InstanceName,
                            inheritFrom = instance.InheritFromHandle == null ? null : instance.InheritFromHandle.InstanceName,
                            processingHash = instance.ProcessingHash, prevalidationXmlHash = instance.PrevalidationXmlHash,
                            inheritFromXmlHash = instance.InheritFromXmlHash }).ToArray() : null;
                    var lookup = new { id = mode + "-persisted-declaration-lookup", mixedSeparatorPath, canonicalPath,
                        configuration = (string)null, fileExists, documentPresent = document != null,
                        state = document == null ? "missing" : document.State.ToString(), declarations };
                    evidence.Add(lookup);
                    Require(fileExists && document != null && declarations != null && declarations.Length == 2,
                        "Cross-document inherited adapter declarations were not restored: " + Json.Serialize(lookup));
                    Require(declarations.Any(item => item.name == "RecoveryUpheaval_FutureDerived" && item.inheritFrom == "AlliedFutureTankArmor") &&
                        declarations.Any(item => item.name == "RecoveryUpheaval_ArcherDerived" && item.inheritFrom == "JapanArcherInfantryArmor"),
                        "Restored adapter child/base inheritance identities differ: " + Json.Serialize(lookup));
                    Require(coldParse >= 7 && warmParse - coldParse < coldParse && restored.XmlParseCount < coldParse, "Expected cold/warm/persisted-session parse evidence");
                    evidence.Add(new { id = mode, legacyFallback = disabled, metadataDeepCopyFallback = metadataCopy, coldParse, warmParse, warmAdditionalParse = warmParse - coldParse, restoredParse = restored.XmlParseCount,
                        fourOutputs = Four(settings.OutputDirectory).ToDictionary(pair => pair.Key, pair => Hash(pair.Value)), actualAssetCount = 7 });
                }
            }
        }
        }
        tests.Add("real-Upheaval-whole-source-files-and-synthetic-cross-document-Define-inheritance-adapters-owned-fallback-cold-warm-persisted-four-stream-equality");
    }
    public static int Main(string[] args)
    {
        object result; int code = 0; Settings previous = Settings.Current; bool previousSwitch, previousMetadata;
        AppContext.TryGetSwitch(Disable, out previousSwitch); AppContext.TryGetSwitch(MetadataDisable, out previousMetadata);
        try
        {
            Require(args.Length == 5 && IntPtr.Size == 4 && Path.DirectorySeparatorChar == '\\', "Expected Windows x86 work result source run SDK");
            work = Path.GetFullPath(args[0]); sdk = Path.GetFullPath(args[4]); mod = Path.Combine(work, "upheaval-small"); Directory.CreateDirectory(work);
            CopyTree(Path.Combine(Path.GetDirectoryName(sdk), "recovery/tests/fixtures/UpheavalSmall"), mod);
            Directory.CreateDirectory(Path.Combine(mod, "Art")); Directory.CreateDirectory(Path.Combine(mod, "Audio"));
            object provenance = VerifyOriginals(); var inputHashes = Directory.GetFiles(mod, "*", SearchOption.AllDirectories).ToDictionary(path => path, Hash);
            Matrix(); VerifyOriginals(); Require(inputHashes.All(pair => Hash(pair.Key) == pair.Value), "Compilation changed the small-group source inputs");
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), evidence = evidence.ToArray(), provenance,
                baselineCases = baselines.ToArray(), fixtureSource = "recovery/tests/CompilerUpheavalSamples.cs", scope = "Seven-asset bounded Upheaval sample with original byte-preserved source files and explicit synthetic semantic adapters; not full MOD, game or art/resource coverage" };
            Console.WriteLine("PASS UpheavalSamples: " + tests.Count + " groups; " + checks + " checks");
        }
        catch (Exception error)
        {
            code = 1; Console.Error.WriteLine(error); result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null, runId = args.Length > 3 ? args[3] : null,
                checks, tests = tests.ToArray(), evidence = evidence.ToArray(), baselineCases = baselines.ToArray(), error = error.ToString() };
        }
        finally { Settings.Current = previous; AppContext.SetSwitch(Disable, previousSwitch); AppContext.SetSwitch(MetadataDisable, previousMetadata); }
        if (args.Length > 1) File.WriteAllText(args[1], Json.Serialize(result)); return code;
    }
}
