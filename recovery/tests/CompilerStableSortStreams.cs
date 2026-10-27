using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using NewModCompiler.Application;

// Real native-backed SDK XML routes, with one private binding graph per root.
// The independent original BAB process consumes the same en-US input below.
public static class CompilerStableSortStreams
{
    const string SymbolDisable = "NewSdk.Compiler.DisableSymbolLookupIndex";
    const string Disable = "NewSdk.Compiler.DisableStableSortMembershipIndex";
    static readonly JavaScriptSerializer Json = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
    static readonly List<object> observations = new List<object>(), baselines = new List<object>();
    static readonly List<string> tests = new List<string>();
    static string work, sdk, mod, input, nextInput, shared;
    static int checks;
    static void Require(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static string Asset(string body) { return "<AssetDeclaration xmlns='uri:ea.com:eala:asset'>" + body + "</AssetDeclaration>"; }
    static long Number(IDictionary<string, object> row, string name) { return Convert.ToInt64(row[name]); }
    static IDictionary<string, object> Metrics(AssetDeclarationDocument document)
    {
        object value = document.GetStableSortMembershipMetrics();
        Require(value != null, "StableSort did not publish membership work metrics");
        return Json.Deserialize<Dictionary<string, object>>(Json.Serialize(value));
    }
    static string Hash(string path)
    { using (var sha = SHA256.Create()) return Convert.ToBase64String(sha.ComputeHash(File.ReadAllBytes(path))); }
    static Dictionary<string, string> Four(string output, string name)
    {
        var files = Directory.GetFiles(output, "*", SearchOption.AllDirectories).Where(path => Path.GetFileNameWithoutExtension(path) == name &&
            new[] { ".manifest", ".bin", ".imp", ".relo" }.Contains(Path.GetExtension(path))).ToArray();
        Require(files.Length == 4, "Stable sort stream did not emit exactly four outputs: " + name);
        return files.ToDictionary(path => Path.GetExtension(path).Substring(1), path => path, StringComparer.Ordinal);
    }
    static void Equal(IDictionary<string, string> expected, IDictionary<string, string> actual, string label)
    {
        Require(expected.Count == actual.Count && expected.All(pair => actual.ContainsKey(pair.Key) &&
            File.ReadAllBytes(pair.Value).SequenceEqual(File.ReadAllBytes(actual[pair.Key]))), "Stable sorting changed linked bytes: " + label);
    }
    static void Inputs()
    {
        mod = Path.Combine(work, "stable-sort-chain"); string data = Path.Combine(mod, "Data"); Directory.CreateDirectory(data);
        input = Path.Combine(data, "chain.xml"); nextInput = Path.Combine(data, "chain-next.xml"); shared = Path.Combine(data, "graph.xml");
        var graph = new StringBuilder();
        for (int index = 95; index >= 0; index--)
        {
            graph.Append("<ExperienceLevelTemplate id='StableE").Append(index.ToString("D3", CultureInfo.InvariantCulture)).Append("'");
            if (index != 0) graph.Append(" Prerequisite='StableE").Append((index - 1).ToString("D3", CultureInfo.InvariantCulture)).Append("'");
            graph.Append("><LevelUpTintColor r='1' g='1' b='1'/>");
            if (index == 0 || index == 48) graph.Append("<Upgrade>StableUpgrade").Append(index == 0 ? "A" : "B").Append("</Upgrade>");
            graph.Append("</ExperienceLevelTemplate>");
        }
        File.WriteAllText(shared, Asset(graph.ToString()), new UTF8Encoding(false));
        File.WriteAllText(Path.Combine(data, "upgrades.xml"), Asset("<UpgradeTemplate id='StableUpgradeA'/><UpgradeTemplate id='StableUpgradeB'/>"), new UTF8Encoding(false));
        string root = Asset("<Includes><Include type='all' source='graph.xml'/><Include type='instance' source='upgrades.xml'/></Includes>");
        File.WriteAllText(input, root, new UTF8Encoding(false)); File.WriteAllText(nextInput, root, new UTF8Encoding(false));
    }
    static Settings Configure(string name)
    {
        var config = new XmlDocument(); config.Load(Path.Combine(sdk, "tools/BinaryAssetBuilder.exe.config"));
        var settings = (Settings)new SerializedSectionHandler().Create(null, null, config.SelectSingleNode("/configuration/assetbuilder"));
        settings.DataRoot = work; settings.SchemaPath = Path.Combine(sdk, "Schemas/xsd/CnC3Types.xsd"); settings.InputPath = input;
        settings.OutputDirectory = Path.Combine(work, name, "streams"); settings.IntermediateOutputDirectory = Path.Combine(work, name, "intermediate");
        settings.SessionCacheDirectory = Path.Combine(work, name, "hashes"); settings.DefaultDataPaths = Path.Combine(mod, "Data");
        settings.DefaultArtPaths = work; settings.DefaultAudioPaths = work; settings.TargetPlatform = TargetPlatform.Win32;
        settings.BuildCache = settings.SessionCache = false; settings.LinkedStreams = true;
        settings.OutputAssetReport = settings.OutputStringHashes = settings.OutputIntermediateXml = settings.VersionFiles = settings.OldOutputFormat = false; settings.ErrorLevel = 0; settings.UsePrecompiled = true;
        settings.StreamHints = false; settings.StableSort = true; settings.ForceSlowCleanup = true; settings.CustomPostfix = settings.StreamPostfix = "";
        settings.BuildConfigurations = new BuildConfiguration[0]; settings.BuildConfigurationName = null;
        Settings.Current = settings; SettingsLoader.PostProcessSettings(work);
        Directory.CreateDirectory(settings.OutputDirectory); Directory.CreateDirectory(settings.IntermediateOutputDirectory); Directory.CreateDirectory(settings.SessionCacheDirectory);
        return settings;
    }
    static DocumentProcessor Processor(BuiltinCompilers plugins, SchemaSet schemas)
    {
        var cache = new SessionCache(); cache.InitializeCache(new List<string>());
        var verifiers = new VerifierPluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32);
        verifiers.DefaultPlugin = (IAssetBuilderVerifierPlugin)Activator.CreateInstance(typeof(DocumentProcessor).Assembly.GetType("BinaryAssetBuilder.GameDataVerifier.Plugin", true), true);
        verifiers.DefaultPlugin.Initialize(null, TargetPlatform.Win32);
        return new DocumentProcessor(Settings.Current, plugins.Registry, verifiers) { Cache = cache, SchemaSet = schemas };
    }
    static AssetDeclarationDocument Run(DocumentProcessor processor, string path)
    {
        bool success; processor.ProcessDocument(path, true, false, out success); Require(success, "Stable sort real Core failed: " + path);
        AssetDeclarationDocument document;
        Require(processor.Cache.TryGetDocument(ShPath.Canonicalize(path), null, TargetPlatform.Win32, false, out document) && document != null,
            "Stable sort root was not retained by its session");
        return document;
    }
    static void Work(IDictionary<string, object> metrics, bool disabled)
    {
        Require(Number(metrics, "calls") > 0 && Convert.ToBoolean(metrics["disabled"]) == disabled, "StableSort membership switch/call evidence was not recorded");
        if (disabled) Require(Number(metrics, "builds") == 0 && Number(metrics, "indexQueries") == 0 && Number(metrics, "linearQueries") > 0,
            "Disabled membership fixture did not execute the original linear path");
        else Require(Convert.ToBoolean(metrics["enabled"]) && Number(metrics, "builds") > 0 && Number(metrics, "entries") > 0 &&
            Number(metrics, "indexQueries") > 0 && Number(metrics, "indexMatches") > 0 && Number(metrics, "budgetBypasses") == 0 && Number(metrics, "unsafeScopeBypasses") == 0,
            "Real exact private dependency chain did not execute the bounded membership fast path: " + Json.Serialize(metrics));
    }
    static void ScalarWorkOnly(AssetDeclarationDocument document)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        Type helper = typeof(DocumentProcessor).Assembly.GetType("BinaryAssetBuilder.Core.CompilerStableSortMembership", true);
        foreach (FieldInfo field in typeof(AssetDeclarationDocument).GetFields(flags))
            Require(field.FieldType != helper && (field.GetValue(document) == null || field.GetValue(document).GetType() != helper), "Root retained the temporary membership index helper");
        FieldInfo stored = typeof(AssetDeclarationDocument).GetField("_StableSortMembershipWork", flags);
        Require(stored != null && stored.GetValue(document) != null, "Root membership scalar work owner was not present");
        foreach (FieldInfo field in stored.GetValue(document).GetType().GetFields(flags))
            Require(field.FieldType == typeof(long) || field.FieldType == typeof(bool) || field.FieldType == typeof(string), "Root membership work retained a collection or mutable semantic object");
        object current = typeof(AssetDeclarationDocument).GetField("_Current", flags).GetValue(document);
        foreach (FieldInfo field in current.GetType().GetFields(flags))
            Require(field.FieldType != helper && (field.GetValue(current) == null || field.GetValue(current).GetType() != helper), "Root current state retained the temporary membership index helper");
    }
    static void Cases()
    {
        Inputs();
        foreach (string culture in new[] { "en-US", "tr-TR" })
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture); Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
            Dictionary<string, string> expected = null;
            foreach (bool symbolDisabled in new[] { false, true })
            foreach (bool disabled in new[] { false, true })
            {
                AppContext.SetSwitch(Disable, disabled); AppContext.SetSwitch(SymbolDisable, symbolDisabled);
                string name = culture + (symbolDisabled ? "-symbol-legacy" : "-symbol-indexed") + (disabled ? "-disabled" : "-indexed"); var settings = Configure(name);
                using (DocumentProcessor.BeginStateScope())
                using (var hashes = HashProvider.BeginStringHashScope(settings.SessionCacheDirectory))
                using (var plugins = BuiltinCompilers.Create(TargetPlatform.Win32))
                {
                    var metadata = typeof(CoreStreamCompiler).GetNestedType("DefaultMetadataPlugin", BindingFlags.NonPublic);
                    plugins.Registry.DefaultPlugin = (IAssetBuilderPlugin)Activator.CreateInstance(metadata, new object[] {
                        plugins.Registry.DefaultPlugin, plugins.Registry.GetPlugin(InstanceHandle.GetTypeId("AttributeModifier")) });
                    Require(plugins.Registry.DefaultPlugin.GetAllTypesHash() == 1424942948u && plugins.Registry.DefaultPlugin.GetVersionNumber() == 4u,
                        "StableSort fixture must use the actual production default metadata view");
                    var processor = Processor(plugins, new SchemaSet(true)); var first = Run(processor, input); var firstMetrics = Metrics(first); Work(firstMetrics, disabled); ScalarWorkOnly(first);
                    var symbolMetrics = Json.Deserialize<Dictionary<string, object>>(Json.Serialize(first.GetSymbolLookupMetrics()));
                    Require(Convert.ToBoolean(symbolMetrics["disabled"]) == symbolDisabled, "Real Core symbol lookup switch not recorded");
                    Require(Number(symbolMetrics, symbolDisabled ? "legacyQueries" : "indexQueries") > 0, "Real prerequisite chain did not consume symbol lookup path: " + Json.Serialize(symbolMetrics));
                    Require(typeof(AssetDeclarationDocument).GetField("_SymbolLookup", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(first) == null,
                        "Temporary symbol index escaped root binding scope");
                    var binding = first.GetStreamBindings();
                    Require(binding != null && binding.enabled && binding.assets.Count == 98 && binding.assets.Count(row => row.identity.typeName == "ExperienceLevelTemplate") == 96 &&
                        binding.assets.Count(row => row.identity.typeName == "UpgradeTemplate") == 2, "StableSort fixture did not bind both asset types and pull its two weak Upgrade candidates");
                    string frozenWork = Json.Serialize(firstMetrics), frozenBindings = Json.Serialize(binding);
                    var outputs = Four(settings.OutputDirectory, "chain");
                    if (expected == null) expected = outputs; else Equal(expected, outputs, culture + " default/disabled");
                    AssetDeclarationDocument semantic;
                    Require(processor.Cache.TryGetDocument(ShPath.Canonicalize(shared), null, TargetPlatform.Win32, false, out semantic), "Shared semantic declaration was absent");
                    var firstDeclaration = semantic.SelfInstances[0];
                    var second = Run(processor, nextInput); var secondMetrics = Metrics(second); Work(secondMetrics, disabled); ScalarWorkOnly(second);
                    Require(!Object.ReferenceEquals(first, second) && processor.Cache.TryGetDocument(ShPath.Canonicalize(shared), null, TargetPlatform.Win32, false, out semantic) &&
                        Object.ReferenceEquals(firstDeclaration, semantic.SelfInstances[0]), "Two root sorts did not reuse one actual semantic graph");
                    Require(Json.Serialize(Metrics(first)) == frozenWork && Json.Serialize(first.GetStreamBindings()) == frozenBindings,
                        "A later root sort changed the earlier root membership work/binding snapshot");
                    observations.Add(new { id = name, inputSha256 = Hash(input), firstMetrics, secondMetrics, symbolMetrics,
                        fourOutputs = outputs.ToDictionary(pair => pair.Key, pair => Hash(pair.Value)), firstBindingsRemainImmutable = true });
                    if (culture == "en-US" && !disabled && !symbolDisabled)
                        baselines.Add(new { name = "stable-sort-chain", sdkRoot = sdk, modRoot = mod, inputPath = input,
                            stagingDirectory = settings.OutputDirectory, outputs, expectedTextures = new object[0], customOutputs = new object[0],
                            effectiveInputRoots = new { dataRoot = work, dataPaths = new[] { Path.Combine(mod, "Data") }, artPaths = new[] { work }, audioPaths = new[] { work } },
                            scope = "96 actual ExperienceLevelTemplate prerequisite-chain assets plus two weakly pulled UpgradeTemplate candidates with StableSort; bounded XML fixture, not full SDK" });
                }
            }
        }
        tests.Add("real-Core-96-Experience-prerequisite-chain-two-Upgrade-types-default-and-disabled-cold-four-byte-output-equality-in-en-US-and-tr-TR");
        tests.Add("actual-root-private-membership-fast-path-counters-disabled-linear-work-and-second-root-shared-semantic-graph-does-not-mutate-prior-work-or-bindings");
    }
    public static int Main(string[] args)
    {
        object result; int code = 0; var previous = Settings.Current; var culture = Thread.CurrentThread.CurrentCulture; var ui = Thread.CurrentThread.CurrentUICulture;
        bool disabled, symbolDisabled; AppContext.TryGetSwitch(Disable, out disabled); AppContext.TryGetSwitch(SymbolDisable, out symbolDisabled);
        try
        {
            Require(args.Length == 5 && IntPtr.Size == 4, "Expected work result source run SDK in x86 host");
            work = Path.GetFullPath(args[0]); sdk = Path.GetFullPath(args[4]); Directory.CreateDirectory(work); Cases();
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), observations = observations.ToArray(),
                fixtureSource = "recovery/tests/CompilerStableSortStreams.cs", baselineCases = baselines.ToArray(),
                scope = "Real Core stable-sort membership work and cold default/fallback exact linked streams; small SDK XML routes plus independent original BAB case, no whole-build speed claim" };
            Console.WriteLine("PASS StableSortStreams: " + checks + " checks");
        }
        catch (Exception error)
        { code = 1; Console.Error.WriteLine(error); result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null, runId = args.Length > 3 ? args[3] : null, checks, tests = tests.ToArray(), observations = observations.ToArray(), error = error.ToString() }; }
        finally { Settings.Current = previous; AppContext.SetSwitch(Disable, disabled); AppContext.SetSwitch(SymbolDisable, symbolDisabled); Thread.CurrentThread.CurrentCulture = culture; Thread.CurrentThread.CurrentUICulture = ui; }
        if (args.Length > 1) File.WriteAllText(args[1], Json.Serialize(result)); return code;
    }
}
