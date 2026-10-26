using System;
using System.Collections;
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

// Runs inside the independently linked production-object validation host.
// This fixture uses real SDK schemas, Core, recovered Armor and verifier code.
internal static class CompilerCoreLifecycle
{
    const string Ns = "uri:ea.com:eala:asset";
    static readonly List<string> Tests = new List<string>();
    static string sdk, work, probe;
    static int checks;
    static readonly BindingFlags Private = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    static void Require(bool value, string message) { ++checks; if (!value) throw new Exception(message); }
    static object Field(object target, string name) { return typeof(DocumentProcessor).GetField(name, Private).GetValue(target); }
    static Stack<string> Documents { get { return (Stack<string>)Field(null, "_CurrentDocumentStack"); } }
    static Stack<string> Streams { get { return (Stack<string>)Field(null, "_CurrentStreamStack"); } }
    static IList Inclusions(DocumentProcessor p) { return (IList)Field(p, "_DocumentStack"); }
    static string Write(string relative, string contents)
    {
        string path = Path.Combine(work, relative); Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, contents); return path;
    }
    static string Asset(string body) { return "<AssetDeclaration xmlns='" + Ns + "'>" + body + "</AssetDeclaration>"; }
    static string Include(string source) { return "<Includes><Include type='all' source='" + source + "'/></Includes>"; }
    static Settings Configure(string name)
    {
        var config = new XmlDocument(); config.Load(Path.Combine(sdk, "tools/BinaryAssetBuilder.exe.config"));
        var s = (Settings)new SerializedSectionHandler().Create(null, null, config.SelectSingleNode("/configuration/assetbuilder"));
        s.DataRoot = work; s.SchemaPath = Path.Combine(sdk, "Schemas/xsd/CnC3Types.xsd");
        s.InputPath = Path.Combine(work, name, "root.xml");
        s.OutputDirectory = Path.Combine(work, name, "output");
        s.IntermediateOutputDirectory = Path.Combine(work, name, "intermediate");
        s.SessionCacheDirectory = Path.Combine(work, name, "cache");
        s.DefaultDataPaths = work; s.DefaultArtPaths = work; s.DefaultAudioPaths = work;
        s.TargetPlatform = TargetPlatform.Win32; s.BuildCache = false; s.SessionCache = false;
        s.LinkedStreams = true; s.OutputAssetReport = false; s.OutputStringHashes = false;
        s.ErrorLevel = 1; s.UsePrecompiled = false; s.StreamHints = false; s.StableSort = false;
        Settings.Current = s; SettingsLoader.PostProcessSettings(work);
        Directory.CreateDirectory(s.OutputDirectory); Directory.CreateDirectory(s.IntermediateOutputDirectory);
        Directory.CreateDirectory(s.SessionCacheDirectory);
        return s;
    }
    static DocumentProcessor Processor(BuiltinCompilers plugins)
    {
        var schemas = new SchemaSet(Settings.Current.StableSort);
        HashProvider.InitializeStringHashes(Settings.Current.SessionCacheDirectory);
        var cache = new SessionCache(); cache.InitializeCache(new List<string>());
        var verifiers = new VerifierPluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32);
        verifiers.DefaultPlugin = (IAssetBuilderVerifierPlugin)Activator.CreateInstance(
            typeof(DocumentProcessor).Assembly.GetType("BinaryAssetBuilder.GameDataVerifier.Plugin", true), true);
        verifiers.DefaultPlugin.Initialize(null, TargetPlatform.Win32);
        return new DocumentProcessor(Settings.Current, plugins.Registry, verifiers) { Cache = cache, SchemaSet = schemas };
    }
    static void Run(DocumentProcessor p, string input)
    {
        bool success; p.ProcessDocument(input, true, false, out success); Require(success, "Core did not report success: " + input);
    }
    static void Failure(DocumentProcessor p, string input, ErrorCode expected)
    {
        try { Run(p, input); }
        catch (BinaryAssetBuilderException e) { Require(e.ErrorCode == expected, "Expected " + expected + ", got " + e.ErrorCode + ": " + e); return; }
        throw new Exception("Core accepted rejected input: " + input);
    }
    static void Clean(DocumentProcessor p, string configuration, params string[] paths)
    {
        Require(Documents.Count == 0 && Streams.Count == 0 && Inclusions(p).Count == 0, "Core traversal frames leaked");
        Require(!(bool)Field(p, "_CompilingProject"), "Project flag leaked");
        Require(Field(p, "_ProjectDefaultConfigurations") == null, "Project configuration table leaked");
        foreach (string path in paths)
        {
            AssetDeclarationDocument document;
            p.Cache.TryGetDocument(path, configuration, TargetPlatform.Win32, false, out document);
            if (document != null) Require(!document.Processing, "Processing flag leaked: " + path);
        }
    }
    static Dictionary<string, string> Outputs(string root)
    {
        var result = new Dictionary<string, string>();
        foreach (string path in Directory.GetFiles(root, "*", SearchOption.AllDirectories))
        {
            string extension = Path.GetExtension(path);
            if (extension != ".manifest" && extension != ".bin" && extension != ".imp" && extension != ".relo") continue;
            using (var sha = SHA256.Create()) result.Add(path.Substring(root.Length), BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path))));
        }
        return result;
    }
    static void Equal(Dictionary<string, string> expected, Dictionary<string, string> actual)
    {
        Require(expected.Count == actual.Count && expected.Count == 4, "Expected four linked stream output files");
        foreach (var pair in expected) Require(actual.ContainsKey(pair.Key) && actual[pair.Key] == pair.Value, "Output changed: " + pair.Key);
    }
    static void RetryCase(BuiltinCompilers plugins, string name, bool inheritance)
    {
        var s = Configure(name); var p = Processor(plugins);
        string root = Write(name + "/root.xml", Asset(Include("middle.xml")));
        string middle = Write(name + "/middle.xml", Asset(Include("leaf.xml")));
        string leaf = Path.Combine(work, name, "leaf.xml");
        if (inheritance) Write(name + "/leaf.xml", Asset("<ArmorTemplate id='Broken' inheritFrom='MissingParent'/>"));
        // Seed pre-existing frames to ensure this call never clears a caller's state.
        Documents.Push("parent-document"); Streams.Push("parent-stream"); Inclusions(p).Add("parent-inclusion");
        try
        {
            Failure(p, root, inheritance ? ErrorCode.InheritFromError : ErrorCode.FileNotFound);
            Require(Documents.Count == 1 && Documents.Peek() == "parent-document", "Document parent frame lost");
            Require(Streams.Count == 1 && Streams.Peek() == "parent-stream", "Stream parent frame lost");
            Require(Inclusions(p).Count == 1 && (string)Inclusions(p)[0] == "parent-inclusion", "Inclusion parent frame lost");
        }
        finally { Documents.Pop(); Streams.Pop(); Inclusions(p).RemoveAt(0); }
        Clean(p, null, root, middle, leaf);
        AssetDeclarationDocument failed; p.Cache.TryGetDocument(root, null, TargetPlatform.Win32, false, out failed);
        Require(failed.State != DocumentState.Complete, "Failed root was completed/cached as successful");
        Write(name + "/leaf.xml", probe);
        Run(p, root); Clean(p, null, root, middle, leaf);
        Dictionary<string, string> repaired = Outputs(s.OutputDirectory);
        // Repeat a completed stream: early return must neither leak frames nor change output.
        Run(p, root); Clean(p, null, root, middle, leaf); Equal(repaired, Outputs(s.OutputDirectory));
        // New root includes an already complete document with GenerateOutput=false.
        string reused = Write(name + "/reuse.xml", Asset(Include("leaf.xml")));
        Run(p, reused); Clean(p, null, reused, leaf);
        // Cold execution uses the same source paths, a fresh real cache and clean output roots.
        string oldOutput = s.OutputDirectory, oldIntermediate = s.IntermediateOutputDirectory;
        s.OutputDirectory = Path.Combine(work, name, "cold-output");
        s.IntermediateOutputDirectory = Path.Combine(work, name, "cold-intermediate");
        var cold = Processor(plugins); Run(cold, root); Clean(cold, null, root, middle, leaf);
        Equal(repaired, Outputs(s.OutputDirectory)); s.OutputDirectory = oldOutput; s.IntermediateOutputDirectory = oldIntermediate;
        Tests.Add(name + ":real-core-failure-same-processor-cache-retry-parent-frames-complete-reuse-cold-output-equality");
    }
    static void CycleCase(BuiltinCompilers plugins)
    {
        Configure("cycle"); var p = Processor(plugins);
        string a = Write("cycle/root.xml", Asset(Include("b.xml")));
        string b = Write("cycle/b.xml", Asset(Include("root.xml")));
        Failure(p, a, ErrorCode.CircularDependency); Clean(p, null, a, b);
        Write("cycle/b.xml", probe); Run(p, a); Clean(p, null, a, b);
        Tests.Add("real-core-circular-include-rejected-then-fixed-same-processor-cache");
    }
    sealed class SaveFaultCache : SessionCache
    {
        public bool Failed;
        public override void SaveDocumentToCache(string path, string configuration, TargetPlatform platform, AssetDeclarationDocument document)
        {
            if (!Failed) { Failed = true; throw new IOException("Injected real-cache save failure"); }
            base.SaveDocumentToCache(path, configuration, platform, document);
        }
    }
    static void SaveFailureCase(BuiltinCompilers plugins)
    {
        Configure("cache-save"); var p = Processor(plugins);
        string input = Write("cache-save/root.xml", probe);
        var fault = new SaveFaultCache(); fault.InitializeCache(new List<string>()); p.Cache = fault;
        bool rejected = false;
        try { Run(p, input); }
        catch (IOException e) { rejected = e.Message == "Injected real-cache save failure"; }
        Require(rejected && fault.Failed, "SaveDocumentToCache fault was not reached/propagated");
        Clean(p, null, input);
        // Cache persistence failure is not acceptance of its half-committed document state.
        // Production must discard this cache; verify the same processor can run with fresh real state.
        var fresh = new SessionCache(); fresh.InitializeCache(new List<string>()); p.Cache = fresh;
        Run(p, input); Clean(p, null, input);
        Tests.Add("real-cache-save-fault-propagates-cleans-frames-then-same-processor-fresh-cache-success");
    }
    static void ProjectCase(BuiltinCompilers plugins)
    {
        Settings s = Configure("project");
        string good = Path.Combine(work, "project/good"), bad = Path.Combine(work, "project/bad");
        Write("project/good/leaf.xml", probe);
        Write("project/bad/leaf.xml", Asset("<ArmorTemplate id='BadConfig' inheritFrom='MissingParent'/>"));
        s.BuildConfigurations = new[] {
            new BuildConfiguration { Name="Good", Postfix="G", DataPaths=work+";"+good },
            new BuildConfiguration { Name="Bad", Postfix="B", DataPaths=work+";"+bad }
        };
        var p = Processor(plugins);
        string root = Write("project/root.xml", Asset(Include("data:leaf.xml")));
        string project = Write("project/test.babproj", "<BinaryAssetBuilderProject xmlns='urn:xmlns:ea.com:babproject'><Stream Source='root.xml'><Configuration Name='Good' Default='true'/><Configuration Name='Bad'/></Stream></BinaryAssetBuilderProject>");
        Failure(p, project, ErrorCode.InheritFromError);
        Require(Object.ReferenceEquals(Settings.Current, s), "Settings.Current not restored after project failure");
        Require(s.BuildConfigurationName == null && s.Postfix == null && s.StreamPostfix == null, "Original Settings values mutated");
        Clean(p, "Good", root, Path.Combine(good, "leaf.xml"));
        Clean(p, "Bad", root, Path.Combine(bad, "leaf.xml"));
        AssetDeclarationDocument first; p.Cache.TryGetDocument(root, "Good", TargetPlatform.Win32, false, out first);
        Require(first != null && first.State == DocumentState.Complete, "First project configuration did not actually compile");
        using (File.Open(project, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
        string normal = Write("project/normal.xml", probe); Run(p, normal); Clean(p, null, normal);
        Tests.Add("real-project-first-config-success-second-inheritance-failure-restores-settings-reader-then-plain-xml-success");
    }
    public static int Main(string[] args)
    {
        object result; int code = 0; Settings original = Settings.Current;
        try
        {
            if (args.Length != 5 || IntPtr.Size != 4) throw new Exception("Expected work result sourceCommit runId sdk (x86)");
            work = Path.GetFullPath(args[0]); sdk = Path.GetFullPath(args[4]); Directory.CreateDirectory(work);
            probe = File.ReadAllText(Path.Combine(sdk, "Mods/mymod/Data/RecoveryArmorProbe.xml"));
            Require(typeof(DocumentProcessor).Assembly == typeof(BuiltinCompilers).Assembly, "Core is outside product assembly");
            // Configuration precedes built-in initialization, just as a real build requires.
            Configure("initialize");
            using (var plugins = BuiltinCompilers.Create(TargetPlatform.Win32))
            {
                RetryCase(plugins, "include", false); RetryCase(plugins, "inheritance", true);
                CycleCase(plugins); ProjectCase(plugins); SaveFailureCase(plugins);
            }
            result = new { succeeded=true, sourceCommit=args[2], runId=args[3], checks=checks, tests=Tests.ToArray(), scope="Real Core lifecycle only; production CLI backend, other global scopes, SDK/G3 and persistent cache not accepted" };
            Console.WriteLine("PASS CoreLifecycle: " + Tests.Count + " groups; " + checks + " checks");
        }
        catch (Exception e)
        {
            code=1; result = new { succeeded=false, sourceCommit=args.Length>2?args[2]:null, runId=args.Length>3?args[3]:null, checks=checks, tests=Tests.ToArray(), error=e.ToString() };
            Console.Error.WriteLine(e);
        }
        finally { Settings.Current = original; }
        if (args.Length>1) File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(result));
        return code;
    }
}
