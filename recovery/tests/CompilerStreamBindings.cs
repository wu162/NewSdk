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

public static class CompilerStreamBindings
{
    const string Ns = "uri:ea.com:eala:asset", Disable = "NewSdk.Compiler.DisableStreamBindingResults";
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    static string work, sdk;
    static int checks;
    static readonly List<string> tests = new List<string>();
    static readonly List<object> observations = new List<object>();
    static readonly JavaScriptSerializer Json = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
    static void Require(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static string Asset(string body) { return "<AssetDeclaration xmlns='" + Ns + "'>" + body + "</AssetDeclaration>"; }
    static string Write(string path, string text)
    { path = Path.GetFullPath(Path.Combine(work, path)); Directory.CreateDirectory(Path.GetDirectoryName(path)); File.WriteAllText(path, text); return path; }
    static Settings Configure(string name)
    {
        var config = new XmlDocument(); config.Load(Path.Combine(sdk, "tools/BinaryAssetBuilder.exe.config"));
        var settings = (Settings)new SerializedSectionHandler().Create(null, null, config.SelectSingleNode("/configuration/assetbuilder"));
        settings.DataRoot = work; settings.SchemaPath = Path.Combine(sdk, "Schemas/xsd/CnC3Types.xsd");
        settings.InputPath = Path.Combine(work, name, "root.xml"); settings.OutputDirectory = Path.Combine(work, name, "output");
        settings.IntermediateOutputDirectory = Path.Combine(work, name, "intermediate"); settings.SessionCacheDirectory = Path.Combine(work, name, "cache");
        settings.DefaultDataPaths = work; settings.DefaultArtPaths = work; settings.DefaultAudioPaths = work;
        settings.TargetPlatform = TargetPlatform.Win32; settings.BuildCache = false; settings.SessionCache = false;
        settings.LinkedStreams = true; settings.OutputAssetReport = false; settings.OutputStringHashes = false;
        settings.ErrorLevel = 1; settings.UsePrecompiled = false; settings.StreamHints = false; settings.StableSort = false;
        Settings.Current = settings; SettingsLoader.PostProcessSettings(work);
        Directory.CreateDirectory(settings.OutputDirectory); Directory.CreateDirectory(settings.IntermediateOutputDirectory); Directory.CreateDirectory(settings.SessionCacheDirectory);
        return settings;
    }
    static DocumentProcessor Processor(BuiltinCompilers plugins, SchemaSet schemas, string cachePath = null)
    {
        HashProvider.InitializeStringHashes(Settings.Current.SessionCacheDirectory);
        var cache = new SessionCache(); if (cachePath != null) cache.LoadCache(cachePath); cache.InitializeCache(new List<string>());
        var verifiers = new VerifierPluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32);
        verifiers.DefaultPlugin = (IAssetBuilderVerifierPlugin)Activator.CreateInstance(typeof(DocumentProcessor).Assembly.GetType("BinaryAssetBuilder.GameDataVerifier.Plugin", true), true);
        verifiers.DefaultPlugin.Initialize(null, TargetPlatform.Win32);
        return new DocumentProcessor(Settings.Current, plugins.Registry, verifiers) { Cache = cache, SchemaSet = schemas };
    }
    static void Run(DocumentProcessor processor, string path)
    { bool success; processor.ProcessDocument(path, true, false, out success); Require(success, "Real Core failed: " + path); }
    static AssetDeclarationDocument Document(DocumentProcessor processor, string path)
    {
        AssetDeclarationDocument document;
        Require(processor.Cache.TryGetDocument(path, null, TargetPlatform.Win32, false, out document) && document != null, "Real Core cache document missing: " + path);
        return document;
    }
    static Dictionary<string, string> Four(string directory, string name)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (string path in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
        {
            string extension = Path.GetExtension(path);
            if (Path.GetFileNameWithoutExtension(path) != name || !new[] { ".manifest", ".bin", ".imp", ".relo" }.Contains(extension)) continue;
            using (var sha = SHA256.Create()) result.Add(extension, Convert.ToBase64String(sha.ComputeHash(File.ReadAllBytes(path))));
        }
        Require(result.Count == 4, "Expected exactly four stream outputs for " + name); return result;
    }
    static void Equal(Dictionary<string, string> expected, Dictionary<string, string> actual, string message)
    { Require(expected.Count == actual.Count && expected.All(pair => actual.ContainsKey(pair.Key) && actual[pair.Key] == pair.Value), message); }
    static string Experience(string id, string prerequisite, bool upgrade)
    { return "<ExperienceLevelTemplate id='" + id + "'" + (prerequisite == null ? "" : " Prerequisite='" + prerequisite + "'") + "><LevelUpTintColor r='1' g='1' b='1'/>" + (upgrade ? "<Upgrade>StreamUpgrade</Upgrade>" : "") + "</ExperienceLevelTemplate>"; }
    static void OutputRoot(Settings settings, string name)
    { settings.OutputDirectory = Path.Combine(work, name, "output"); settings.IntermediateOutputDirectory = Path.Combine(work, name, "intermediate"); Directory.CreateDirectory(settings.OutputDirectory); Directory.CreateDirectory(settings.IntermediateOutputDirectory); }
    static void WeakStreams(BuiltinCompilers plugins, SchemaSet schemas)
    {
        var settings = Configure("weak");
        string shared = Write("weak/shared.xml", Asset(Experience("StreamOwner", null, true)));
        Write("weak/candidate.xml", Asset("<UpgradeTemplate id='StreamUpgrade'/>"));
        string first = Write("weak/first.xml", Asset("<Includes><Include type='all' source='shared.xml'/></Includes>"));
        string second = Write("weak/second.xml", Asset("<Includes><Include type='all' source='shared.xml'/><Include type='instance' source='candidate.xml'/></Includes>"));
        string cachePath = Path.Combine(settings.SessionCacheDirectory, "bindings.xml");
        AppContext.SetSwitch(Disable, false); var processor = Processor(plugins, schemas, cachePath); Run(processor, first);
        var firstBindings = Document(processor, first).GetStreamBindings();
        Require(firstBindings != null && firstBindings.enabled && firstBindings.assets.Single(row => row.identity.instanceName == "StreamOwner").references.Count == 0,
            "First stream must retain an unresolved weak reference");
        string frozen = Json.Serialize(firstBindings); var sharedAsset = Document(processor, shared).SelfInstances[0];
        Run(processor, second); var secondBindings = Document(processor, second).GetStreamBindings();
        Require(Object.ReferenceEquals(sharedAsset, Document(processor, shared).SelfInstances[0]), "Two streams did not actually share their semantic declaration");
        Require(secondBindings.assets.Single(row => row.identity.instanceName == "StreamOwner").references.Single().instanceName == "StreamUpgrade", "Second symbol view did not pull the weak Tentative target");
        Require(Json.Serialize(firstBindings) == frozen, "Second stream mutated the first binding snapshot");
        var expected = Four(settings.OutputDirectory, "second"); processor.Cache.SaveCache(false);
        var restored = Processor(plugins, schemas, cachePath); Run(restored, second);
        Equal(expected, Four(settings.OutputDirectory, "second"), "Persisted session restore changed binding outputs");
        Require(Document(restored, second).GetStreamBindings().assets.Single(row => row.identity.instanceName == "StreamOwner").references.Single().instanceName == "StreamUpgrade", "Session restore reused stale binding results");
        OutputRoot(settings, "weak-cold"); var cold = Processor(plugins, schemas); Run(cold, second); Equal(expected, Four(settings.OutputDirectory, "second"), "Cold stream differs from shared/restored four outputs");
        AppContext.SetSwitch(Disable, true); OutputRoot(settings, "weak-fallback"); var legacy = Processor(plugins, schemas); Run(legacy, first); Run(legacy, second);
        Equal(expected, Four(settings.OutputDirectory, "second"), "Legacy fallback differs from private stream binding four outputs");
        Require(Json.Serialize(firstBindings) == frozen, "Fallback binding changed a prior private snapshot");
        AppContext.SetSwitch(Disable, false);
        observations.Add(new { id = "weak-missing-to-tentative", first = firstBindings, second = secondBindings, fourOutputs = expected });
        tests.Add("actual-two-Core-streams-share-semantic-asset-but-own-weak-binding-results-session-restore-cold-and-fallback-four-output-equality");
    }
    static AssetDeclarationDocument BareDocument(DocumentProcessor processor, string name)
    {
        var document = new AssetDeclarationDocument(); var type = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object current = Activator.CreateInstance(type, true); type.GetMethod("FromScratch").Invoke(current, new object[0]);
        type.GetField("SourcePath").SetValue(current, Path.Combine(work, name + ".xml")); type.GetField("SourceDirectory").SetValue(current, work);
        type.GetField("SourcePathFromRoot").SetValue(current, name); type.GetField("LogicalSourcePath").SetValue(current, name + ".xml"); type.GetField("DocumentProcessor").SetValue(current, processor);
        typeof(AssetDeclarationDocument).GetField("_Current", Hidden).SetValue(document, current); return document;
    }
    static InstanceDeclaration BareInstance(AssetDeclarationDocument document, string type, string name, uint hash)
    {
        var xml = new XmlDocument(); xml.LoadXml("<" + type + " id='" + name + "'/>");
        var instance = new InstanceDeclaration(document) { XmlNode = xml.DocumentElement }; instance.Handle.TypeHash = 17; instance.Handle.InstanceHash = hash; return instance;
    }
    static StreamBindingResult Materialize(DocumentProcessor processor, AssetDeclarationDocument document, string name, params InstanceDeclaration[] assets)
    {
        var settings = Settings.Current; string output = Path.Combine(settings.OutputDirectory, name), intermediate = Path.Combine(settings.IntermediateOutputDirectory, name);
        var manager = new OutputManager(processor, null, output, intermediate, null, "", new string[0]);
        foreach (var instance in assets) manager.GetBinaryAsset(instance, true).Buffer = new AssetBuffer { InstanceData = BitConverter.GetBytes(instance.Handle.InstanceHash), RelocationData = new byte[0], ImportsData = new byte[0] };
        document.PrepareOutputInstances(manager); manager.CommitManifest(document); manager.LinkStream(document);
        Require(document.GetStreamBindings() != null, "Successful Prepare did not publish immutable bindings"); return document.GetStreamBindings();
    }
    static StreamBindingResult BindGraph(AssetDeclarationDocument document)
    {
        // The abstract exact candidate deliberately has no production encoder.
        // Exercise only the same binding routines called by Prepare; real weak
        // streams and concrete Armor graphs cover Prepare/manifest/link outputs.
        object current = typeof(AssetDeclarationDocument).GetField("_Current", Hidden).GetValue(document);
        var type = current.GetType();
        var instances = new SortedDictionary<InstanceHandle, InstanceDeclaration>();
        type.GetField("OutputInstanceSet").SetValue(current, instances);
        typeof(AssetDeclarationDocument).GetMethod("BeginStreamBindings", Hidden).Invoke(document, new object[0]);
        foreach (var instance in document.Instances) typeof(AssetDeclarationDocument).GetMethod("AddOutputInstance", Hidden).Invoke(document, new object[] { instance });
        type.GetField("OutputInstances").SetValue(current, new List2<InstanceDeclaration>(instances.Values));
        typeof(AssetDeclarationDocument).GetMethod("PublishStreamBindings", Hidden).Invoke(document, new object[0]);
        return document.GetStreamBindings();
    }
    static void BoundGraphs(BuiltinCompilers plugins, SchemaSet schemas)
    {
        var settings = Configure("graphs"); var processor = Processor(plugins, schemas);
        var first = BareDocument(processor, "exact"); var second = BareDocument(processor, "derived");
        var owner = BareInstance(first, "ExperienceLevelTemplate", "GraphOwner", 10);
        var request = new InstanceHandle("BaseInheritableAsset", "GraphTarget"); owner.ReferencedInstances.Add(request); owner.ReferencedInstances.Add(new InstanceHandle("ArmorTemplate", "GraphOther")); owner.ReferencedInstances.Add(new InstanceHandle(request));
        var other = BareInstance(first, "ArmorTemplate", "GraphOther", 40);
        var exact = BareInstance(first, "BaseInheritableAsset", "GraphTarget", 20); var derived = BareInstance(second, "ArmorTemplate", "GraphTarget", 30);
        first.SelfInstances.Add(owner); first.Instances.Add(owner); first.TentativeInstances.Add(exact); first.TentativeInstances.Add(other);
        second.AllInstances.Add(owner); second.Instances.Add(owner); second.TentativeInstances.Add(derived); second.TentativeInstances.Add(other);
        var firstResult = BindGraph(first); string frozen = Json.Serialize(firstResult);
        var secondResult = BindGraph(second);
        var exactOwner = firstResult.assets.Single(row => row.identity.instanceName == "GraphOwner"); var derivedOwner = secondResult.assets.Single(row => row.identity.instanceName == "GraphOwner");
        Require(exactOwner.strongReferenceCount == 3 && exactOwner.references.Select(row => row.instanceName).SequenceEqual(new[] { "GraphTarget", "GraphOther", "GraphTarget" }) && exactOwner.references.Where(row => row.instanceName == "GraphTarget").All(row => row.typeName == "BaseInheritableAsset" && row.instanceHash == 20), "Exact binding lost ordered duplicate strong import positions");
        Require(derivedOwner.references.Select(row => row.instanceName).SequenceEqual(new[] { "GraphTarget", "GraphOther", "GraphTarget" }) && derivedOwner.references.Where(row => row.instanceName == "GraphTarget").All(row => row.typeName == "ArmorTemplate" && row.instanceHash == 30), "Derived rebinding retained an exact target or reordered duplicate imports");
        Require(Json.Serialize(firstResult) == frozen, "Derived stream rewrote prior exact binding identity/hash");
        exact.Handle.InstanceHash = 200; owner.Handle.InstanceHash = 100; request.TypeName = "MutatedRequest";
        Require(Json.Serialize(firstResult) == frozen, "Snapshot retained mutable declaration/handle objects");
        bool readOnly = false; try { firstResult.assets.Clear(); } catch (NotSupportedException) { readOnly = true; }
        Require(readOnly, "Binding snapshot exposes a writable asset list");
        AppContext.SetSwitch(Disable, true);
        // Repeat the original derived graph, with original identities, through the historical algorithm.
        owner.Handle.InstanceHash = 10; request.TypeName = "BaseInheritableAsset";
        var legacy = BareDocument(processor, "derived-legacy"); legacy.AllInstances.Add(owner); legacy.Instances.Add(owner); legacy.TentativeInstances.Add(derived); legacy.TentativeInstances.Add(other);
        var fallback = BindGraph(legacy);
        Require(Json.Serialize(secondResult.assets) == Json.Serialize(fallback.assets), "Synthetic ordered duplicate bindings differ from legacy fallback");
        AppContext.SetSwitch(Disable, false);
        observations.Add(new { id = "exact-derived-hash-and-duplicate-imports", first = firstResult, second = secondResult });
        tests.Add("actual-Core-binding-graph-exact-to-derived-target-hash-change-ordered-duplicate-strong-references-deep-snapshot-and-legacy-equality");
    }
    static void CycleAndFailure(BuiltinCompilers plugins, SchemaSet schemas)
    {
        var settings = Configure("cycle"); var processor = Processor(plugins, schemas);
        var graph = BareDocument(processor, "weak-cycle");
        var a = BareInstance(graph, "ArmorTemplate", "CycleA", 1); var b = BareInstance(graph, "ArmorTemplate", "CycleB", 2);
        a.WeakReferencedInstances.Add(new InstanceHandle("ArmorTemplate", "CycleB")); b.WeakReferencedInstances.Add(new InstanceHandle("ArmorTemplate", "CycleA"));
        graph.SelfInstances.Add(a); graph.Instances.Add(a); graph.TentativeInstances.Add(b);
        var snapshot = Materialize(processor, graph, "weak-cycle", a, b); string frozen = Json.Serialize(snapshot);
        Require(snapshot.assets.Count == 2 && snapshot.assets.Single(row => row.identity.instanceName == "CycleA").references.Single().instanceName == "CycleB", "Legal weak cycle did not pull its Tentative member");
        Require(snapshot.assets.Single(row => row.identity.instanceName == "CycleB").references.Count == 0, "Weak Self target changed the original Tentative-only inclusion rule");
        var expected = Four(settings.OutputDirectory, "weak-cycle");
        AppContext.SetSwitch(Disable, true); Materialize(processor, graph, "weak-cycle-fallback", a, b);
        Equal(expected, Four(settings.OutputDirectory, "weak-cycle-fallback"), "Legal weak-reference cycle differs from legacy four outputs");
        AppContext.SetSwitch(Disable, false); string failure = null;
        foreach (bool legacy in new[] { false, true })
        {
            AppContext.SetSwitch(Disable, legacy); a.ReferencedInstances.Clear(); a.ReferencedInstances.Add(new InstanceHandle("ArmorTemplate", "MissingStrongTarget"));
            bool rejected = false;
            try { Materialize(processor, graph, legacy ? "failure-legacy" : "failure-private", a, b); }
            catch (BinaryAssetBuilderException error)
            {
                rejected = error.ErrorCode == ErrorCode.UnknownReference;
                if (failure == null) failure = error.Message; else Require(error.Message == failure, "New binding changed the missing-strong diagnostic");
            }
            Require(rejected && graph.GetStreamBindings() == null, "Failure published a partial/stale stream binding snapshot");
            Require(Json.Serialize(snapshot) == frozen, "Failed/fallback binding mutated a prior successful snapshot");
        }
        AppContext.SetSwitch(Disable, false);
        observations.Add(new { id = "legal-weak-cycle-and-strong-failure", snapshot, fourOutputs = expected, failure });
        tests.Add("legal-weak-reference-cycle-retains-Tentative-only-rule-and-four-outputs-strong-failure-keeps-exact-legacy-diagnostic-and-publishes-no-snapshot");
    }
    public static int Main(string[] args)
    {
        object result; int code = 0; Settings previous = Settings.Current; bool disabled; AppContext.TryGetSwitch(Disable, out disabled);
        try
        {
            work = Path.GetFullPath(args[0]); sdk = Path.GetFullPath(args[4]); Configure("initialize");
            using (var plugins = BuiltinCompilers.Create(TargetPlatform.Win32))
            {
                var schemas = new SchemaSet(false); WeakStreams(plugins, schemas); BoundGraphs(plugins, schemas); CycleAndFailure(plugins, schemas);
            }
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), observations = observations.ToArray(), scope = "Real Core private per-stream binding snapshots, targeted session restore and four-output fallback differentials; synthetic graphs supplement real SDK-XML streams" };
            Console.WriteLine("PASS StreamBindings: " + tests.Count + " groups; " + checks + " checks");
        }
        catch (Exception error)
        { code = 1; Console.Error.WriteLine(error); result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null, runId = args.Length > 3 ? args[3] : null, checks, tests = tests.ToArray(), error = error.ToString() }; }
        finally { Settings.Current = previous; AppContext.SetSwitch(Disable, disabled); }
        if (args.Length > 1) File.WriteAllText(args[1], Json.Serialize(result)); return code;
    }
}
