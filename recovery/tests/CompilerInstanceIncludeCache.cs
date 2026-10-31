using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using NewModCompiler.Application;

// Real Core and persisted sessions, using the current production-object host.
// Each edited generation is compared with a new no-session Core execution.
internal static class CompilerInstanceIncludeCache
{
    const string Ns = "uri:ea.com:eala:asset";
    static string sdk, work;
    static int checks;
    static readonly List<string> tests = new List<string>();
    static void Need(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    static string Asset(string body) { return "<AssetDeclaration xmlns='" + Ns + "' xmlns:xi='http://www.w3.org/2001/XInclude'>" + body + "</AssetDeclaration>"; }
    static string Include(string source, string type = "all") { return "<Includes><Include type='" + type + "' source='" + source + "'/></Includes>"; }
    static string Put(string relative, string value)
    {
        string path = Path.GetFullPath(Path.Combine(work, relative)); Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, value, new UTF8Encoding(false)); return path;
    }
    static void Edit(string path, string before, string after)
    {
        string text = File.ReadAllText(path); Need(text.Contains(before), "Missing mutation token: " + before);
        byte[] value = Encoding.UTF8.GetBytes(text.Replace(before, after));
        long length = new FileInfo(path).Length; DateTime stamp = File.GetLastWriteTimeUtc(path);
        Need(value.Length == length, "Mutation must have the same byte length");
        File.WriteAllBytes(path, value); File.SetLastWriteTimeUtc(path, stamp);
        Need(File.GetLastWriteTimeUtc(path) == stamp && new FileInfo(path).Length == length, "Mutation changed time or length");
    }
    static Settings Configure(string name)
    {
        var config = new XmlDocument(); config.Load(Path.Combine(sdk, "tools/BinaryAssetBuilder.exe.config"));
        var settings = (Settings)new SerializedSectionHandler().Create(null, null, config.SelectSingleNode("/configuration/assetbuilder"));
        settings.DataRoot = work; settings.SchemaPath = Path.Combine(sdk, "Schemas/xsd/CnC3Types.xsd");
        settings.InputPath = Path.Combine(work, name, "root.xml");
        settings.OutputDirectory = Path.Combine(work, name, "output");
        settings.IntermediateOutputDirectory = Path.Combine(work, name, "intermediate");
        settings.SessionCacheDirectory = Path.Combine(work, name, "cache");
        settings.DefaultDataPaths = work; settings.DefaultArtPaths = work; settings.DefaultAudioPaths = work;
        settings.TargetPlatform = TargetPlatform.Win32; settings.BuildCache = false; settings.SessionCache = true;
        settings.LinkedStreams = true; settings.OutputAssetReport = false; settings.OutputStringHashes = false;
        settings.ErrorLevel = 1; settings.UsePrecompiled = false; settings.StreamHints = false; settings.StableSort = false;
        Settings.Current = settings; SettingsLoader.PostProcessSettings(work);
        Directory.CreateDirectory(settings.OutputDirectory); Directory.CreateDirectory(settings.IntermediateOutputDirectory);
        Directory.CreateDirectory(settings.SessionCacheDirectory); return settings;
    }
    sealed class RequestState
    {
        internal BuiltinCompilers Plugins;
        internal SchemaSet SchemaSet;
        internal VerifierPluginRegistry Verifiers;
        internal SessionCache Cache;
        internal int XmlParseCount, InstanceCompileCount;
    }
    static RequestState Processor(BuiltinCompilers plugins, SchemaSet schemas = null, SessionCache cache = null)
    {
        schemas = schemas ?? new SchemaSet(false);
        HashProvider.InitializeStringHashes(Settings.Current.SessionCacheDirectory);
        if (cache == null) { cache = new SessionCache(); cache.InitializeCache(new List<string>(), schemas.ContentIdentity); }
        var verifiers = new VerifierPluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32);
        verifiers.DefaultPlugin = (IAssetBuilderVerifierPlugin)Activator.CreateInstance(
            typeof(DocumentProcessor).Assembly.GetType("BinaryAssetBuilder.GameDataVerifier.Plugin", true), true);
        verifiers.DefaultPlugin.Initialize(null, TargetPlatform.Win32);
        return new RequestState { Plugins = plugins, Cache = cache, SchemaSet = schemas, Verifiers = verifiers };
    }
    static void Run(RequestState processor, string root)
    {
        // Each call is a new request: reset input observations after any edit.
        processor.Cache.InitializeCache(new List<string>(), processor.SchemaSet.ContentIdentity);
        // CoreStreamCompiler creates a processor for every stream/request. Its
        // traversal requirements and output lookup tables are request-local.
        var current = new DocumentProcessor(Settings.Current, processor.Plugins.Registry, processor.Verifiers) { Cache = processor.Cache, SchemaSet = processor.SchemaSet };
        try { bool success; current.ProcessDocument(root, true, false, out success); Need(success, "Core did not complete " + root); }
        finally { processor.XmlParseCount += current.XmlParseCount; processor.InstanceCompileCount += current.InstanceCompileCount; }
    }
    static Dictionary<string, string> Outputs(string root)
    {
        var result = new Dictionary<string, string>();
        foreach (string path in Directory.GetFiles(root, "*", SearchOption.AllDirectories))
        {
            string extension = Path.GetExtension(path);
            if (extension != ".manifest" && extension != ".bin" && extension != ".imp" && extension != ".relo") continue;
            using (var sha = SHA256.Create()) result.Add(path.Substring(root.Length), Convert.ToBase64String(sha.ComputeHash(File.ReadAllBytes(path))));
        }
        Need(result.Count == 4, "Expected four linked stream files"); return result;
    }
    static void Equal(Dictionary<string, string> expected, Dictionary<string, string> actual, string label)
    {
        Need(expected.Count == actual.Count && expected.All(pair => actual.ContainsKey(pair.Key) && actual[pair.Key] == pair.Value), label + ": publication bytes differ");
    }
    static Dictionary<string, string> ColdControl(BuiltinCompilers plugins, RequestState current, string root, string generation)
    {
        Settings settings = Settings.Current; string output = settings.OutputDirectory, intermediate = settings.IntermediateOutputDirectory;
        var actual = Outputs(output);
        try
        {
            settings.OutputDirectory = output + "-control-" + generation;
            settings.IntermediateOutputDirectory = intermediate + "-control-" + generation;
            Directory.CreateDirectory(settings.OutputDirectory); Directory.CreateDirectory(settings.IntermediateOutputDirectory);
            Run(Processor(plugins, current.SchemaSet), root); Equal(actual, Outputs(settings.OutputDirectory), generation + " cached/cold");
        }
        finally { settings.OutputDirectory = output; settings.IntermediateOutputDirectory = intermediate; }
        return actual;
    }
    static void Next(SessionCache cache, RequestState processor)
    {
        cache.SaveCache(false);
    }
    static void Hot(SessionCache cache, RequestState processor, string root)
    {
        Next(cache, processor); int parsed = processor.XmlParseCount, compiled = processor.InstanceCompileCount;
        Run(processor, root);
        Need(processor.XmlParseCount == parsed && processor.InstanceCompileCount == compiled, "Completed generation did not become hot: parse=" + (processor.XmlParseCount - parsed) + " compile=" + (processor.InstanceCompileCount - compiled));
    }
    static SessionCache Cache(string name, SchemaSet schemas)
    {
        var cache = new SessionCache(); cache.LoadCacheFrom(null, Path.Combine(Settings.Current.SessionCacheDirectory, "session.xml"));
        cache.InitializeCache(new List<string>(), schemas.ContentIdentity); return cache;
    }
    static void Reject(RequestState processor, string root, ErrorCode expected)
    {
        int compiled = processor.InstanceCompileCount;
        try { Run(processor, root); }
        catch (BinaryAssetBuilderException error)
        {
            Need(error.ErrorCode == expected, "Unexpected rejection: " + error);
            Need(processor.InstanceCompileCount == compiled, "Rejected parent reached asset compilation"); return;
        }
        throw new Exception("Invalid parent input was accepted: " + expected);
    }
    static void Chain(BuiltinCompilers plugins)
    {
        Configure("chain"); var processor = Processor(plugins); var cache = Cache("chain", processor.SchemaSet); processor.Cache = cache;
        string defs = Put("chain/defines.xml", Asset("<Defines><Define name='CHAIN_SCALE' value='83.25'/><Define name='CHAIN_UNUSED' value='95.25'/></Defines>"));
        string xi = Put("chain/xi-defines.xml", "<Defines xmlns='" + Ns + "'><Define name='CHAIN_X' value='63.25'/></Defines>");
        string parent = Put("chain/base.xml", Asset(Include("defines.xml") + "<xi:include href='xi-defines.xml'/><ArmorTemplate id='ChainBase' Default='72.5%' DamageScalar='=$CHAIN_SCALE' SideDamageScalar='=$CHAIN_X'/><ArmorTemplate id='ChainPeer' Default='64.5%'/>"));
        string child = Put("chain/child.xml", Asset(Include("base.xml", "instance") + "<ArmorTemplate id='ChainSelf' inheritFrom='ChainBase'/><ArmorTemplate id='ChainChild' inheritFrom='ChainSelf'/>"));
        string grand = Put("chain/grand.xml", Asset(Include("child.xml", "instance") + "<ArmorTemplate id='ChainGrand' inheritFrom='ChainChild'/>"));
        string root = Put("chain/root.xml", Asset(Include("grand.xml")));
        Run(processor, root); var original = ColdControl(plugins, processor, root, "original"); Next(cache, processor);
        int parsed = processor.XmlParseCount;
        Edit(parent, "64.5%", "54.5%"); Run(processor, root);
        Need(processor.XmlParseCount - parsed == 1, "Unrelated parent sibling expected 1 parse; actual=" + (processor.XmlParseCount - parsed));
        Equal(original, ColdControl(plugins, processor, root, "sibling"), "Unrelated parent sibling"); Hot(cache, processor, root);
        Next(cache, processor); parsed = processor.XmlParseCount;
        Edit(parent, "72.5%", "62.5%"); Run(processor, root);
        Need(processor.XmlParseCount - parsed == 3, "Effective parent expected 3 parses; actual=" + (processor.XmlParseCount - parsed));
        var changed = ColdControl(plugins, processor, root, "effective"); Need(original.Any(pair => changed[pair.Key] != pair.Value), "Effective inherited edit changed no output"); Hot(cache, processor, root);
        Next(cache, processor); Edit(defs, "83.25", "73.25"); Run(processor, root);
        ColdControl(plugins, processor, root, "define-used"); Hot(cache, processor, root);
        Next(cache, processor); parsed = processor.XmlParseCount; var beforeUnused = Outputs(Settings.Current.OutputDirectory);
        Edit(defs, "95.25", "85.25"); Run(processor, root);
        Need(processor.XmlParseCount - parsed == 1, "Unused Define expected 1 parse; actual=" + (processor.XmlParseCount - parsed));
        Equal(beforeUnused, ColdControl(plugins, processor, root, "define-unused"), "Unused Define"); Hot(cache, processor, root);
        Next(cache, processor); Edit(xi, "63.25", "53.25"); Run(processor, root);
        ColdControl(plugins, processor, root, "xinclude"); Hot(cache, processor, root);
        // A cached child must not suppress invalid unrelated parent data.
        Next(cache, processor); Edit(parent, "Default='54.5%'", "Unknown='54.5%'"); Reject(processor, root, ErrorCode.SchemaValidation);
        Edit(parent, "Unknown='54.5%'", "Default='54.5%'"); cache.InitializeCache(new List<string>(), processor.SchemaSet.ContentIdentity);
        Run(processor, root); ColdControl(plugins, processor, root, "repaired"); Hot(cache, processor, root);
        Next(cache, processor); Edit(parent, "id='ChainPeer'", "id='ChainBase'"); Reject(processor, root, ErrorCode.DuplicateInstance);
        Edit(parent, "id='ChainBase' Default='54.5%'", "id='ChainPeer' Default='54.5%'");
        cache.InitializeCache(new List<string>(), processor.SchemaSet.ContentIdentity);
        Run(processor, root); ColdControl(plugins, processor, root, "duplicate-repaired"); Hot(cache, processor, root);
        // New metadata must not be inferred from an older session's ambiguous file list.
        cache.SaveCache(false); var legacy = new XmlDocument(); legacy.Load(cache.CacheFileName);
        string[] cacheIdentity = legacy.DocumentElement.GetAttribute("d").Split(';');
        Need(cacheIdentity[1] == "19", "New narrowed dependency sessions must write format v19");
        cacheIdentity[1] = "18"; legacy.DocumentElement.SetAttribute("d", String.Join(";", cacheIdentity));
        foreach (XmlElement node in legacy.SelectNodes("//*[@iid or @ii]")) { node.RemoveAttribute("iid"); node.RemoveAttribute("ii"); }
        using (var writer = XmlWriter.Create(cache.CacheFileName, new XmlWriterSettings { Indent = false, OmitXmlDeclaration = true })) legacy.WriteTo(writer);
        var imported = new SessionCache(); imported.LoadCache(cache.CacheFileName); imported.InitializeCache(new List<string>(), processor.SchemaSet.ContentIdentity);
        var migrated = Processor(plugins, processor.SchemaSet, imported); Run(migrated, root);
        Need(migrated.XmlParseCount >= 2, "Legacy instance dependency metadata was treated as proven safe");
        ColdControl(plugins, migrated, root, "legacy-upgrade"); Hot(imported, migrated, root);
        tests.Add("parent-sibling-1-parse-effective-3-parses-self-self-external-ancestor-chain-Define-XInclude-invalid-parent-repair-legacy-metadata-upgrade-cold-bytes-hot-zero");
    }
    static void DualRole(BuiltinCompilers plugins, bool fileBinding)
    {
        string name = fileBinding ? "dual-file" : "dual-xinclude"; Configure(name);
        var processor = Processor(plugins); var cache = Cache(name, processor.SchemaSet); processor.Cache = cache;
        string parentBody = fileBinding
            ? "<ArmorTemplate id='DualBase' Default='72.5%'/><ArmorTemplate id='DualSibling' Default='64.5%'/>"
            : "<Defines><Define name='DUAL_UNUSED' value='95.25' override='true'/></Defines><ArmorTemplate id='DualBase' Default='72.5%'/><ArmorTemplate id='DualSibling' Default='64.5%'/>";
        string parent = Put(name + "/base.xml", Asset(parentBody));
        string body = Include("base.xml", "instance") + (fileBinding ? "<ArmorTemplate id='DualChild' inheritFrom='DualBase'/><AptAptData id='DualPayload' File='base.xml'/>"
            : "<xi:include href='base.xml' xpointer='element(/1/1)'/><ArmorTemplate id='DualChild' inheritFrom='DualBase'/>");
        Put(name + "/child.xml", Asset(body)); string root = Put(name + "/root.xml", Asset(Include("child.xml")));
        Run(processor, root); var before = Outputs(Settings.Current.OutputDirectory); Next(cache, processor);
        int parsed = processor.XmlParseCount; Edit(parent, "64.5%", "54.5%"); Run(processor, root);
        Need(processor.XmlParseCount - parsed >= 2, "Same physical path's real " + name + " dependency was discarded with instance Include");
        var after = ColdControl(plugins, processor, root, name);
        if (fileBinding) Need(before.Any(pair => after[pair.Key] != pair.Value), "Same-path external file bytes did not change output");
        else Equal(before, after, "Unused XInclude payload");
        Hot(cache, processor, root); tests.Add(name + "-same-physical-instance-and-real-file-dependency-retained-cold-bytes-hot-zero");
    }
    static void SearchRoot(BuiltinCompilers plugins)
    {
        var settings = Configure("roots");
        string a = Path.Combine(work, "roots/a"), b = Path.Combine(work, "roots/b");
        string parent = Asset("<ArmorTemplate id='RootBase' Default='72.5%'/>");
        Put("roots/a/base.xml", parent); Put("roots/b/base.xml", parent);
        Put("roots/child.xml", Asset(Include("data:base.xml", "instance") + "<ArmorTemplate id='RootSelf' inheritFrom='RootBase'/><ArmorTemplate id='RootChild' inheritFrom='RootSelf'/>"));
        Put("roots/grand.xml", Asset(Include("child.xml", "instance") + "<ArmorTemplate id='RootGrand' inheritFrom='RootChild'/>"));
        string root = Put("roots/root.xml", Asset(Include("grand.xml")));
        settings.DataPaths = new[] { a, b }; var processor = Processor(plugins); var cache = Cache("roots", processor.SchemaSet); processor.Cache = cache;
        Run(processor, root); Next(cache, processor); int parsed = processor.XmlParseCount;
        settings.DataPaths = new[] { b, a }; Run(processor, root);
        Need(processor.XmlParseCount - parsed >= 3, "Search-root physical parent switch did not reload binding with identical parent bytes");
        ColdControl(plugins, processor, root, "root-switch"); Hot(cache, processor, root);
        tests.Add("search-root-physical-parent-switch-identical-input-reloads-binding-cold-bytes-hot-zero");
    }
    public static int Main(string[] args)
    {
        Settings original = Settings.Current; object result; int code = 0;
        try
        {
            Need(args.Length == 5 && IntPtr.Size == 4, "Expected work result sourceCommit runId sdk (x86)");
            work = Path.GetFullPath(args[0]); sdk = Path.GetFullPath(args[4]); Directory.CreateDirectory(work); Configure("initialize");
            var failures = new List<string>();
            Action<string, Action> scenario = delegate(string name, Action action) {
                try { action(); }
                catch (Exception error) { failures.Add(name + ": " + error); Console.Error.WriteLine("FAIL " + name + ": " + error); }
            };
            using (var plugins = BuiltinCompilers.Create(TargetPlatform.Win32))
            {
                scenario("chain", () => Chain(plugins));
                scenario("dual-xinclude", () => DualRole(plugins, false));
                scenario("dual-file", () => DualRole(plugins, true));
                scenario("search-root", () => SearchRoot(plugins));
            }
            Need(failures.Count == 0, String.Join("\n", failures));
            result = new { succeeded=true, sourceCommit=args[2], runId=args[3], checks, tests=tests.ToArray(), scope="Real Core instance Include invalidation and persisted session boundaries; complete SDK remains separate" };
            Console.WriteLine("PASS InstanceIncludeCache: " + checks + " checks");
        }
        catch (Exception error)
        { code = 1; result = new { succeeded=false, sourceCommit=args.Length>2?args[2]:null, runId=args.Length>3?args[3]:null, checks, tests=tests.ToArray(), error=error.ToString() }; Console.Error.WriteLine(error); }
        finally { Settings.Current = original; }
        if (args.Length > 1) File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(result)); return code;
    }
}
