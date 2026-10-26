using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using NewModCompiler.Application;

// Real Core input routes under the opt-in build-cache input inventory boundary.
public static class CompilerCacheBoundary
{
    const string Ns = "uri:ea.com:eala:asset";
    static int checks;
    static string sdk, work, inside, outside;
    static readonly List<string> tests = new List<string>();
    static void Require(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    static string Escape(string text) { return text.Replace("&", "&amp;").Replace("'", "&apos;").Replace("<", "&lt;"); }
    static string Write(string directory, string name, string text)
    {
        Directory.CreateDirectory(directory); string path = Path.Combine(directory, name); File.WriteAllText(path, text); return path;
    }
    static Settings Configure(string name)
    {
        var config = new XmlDocument(); config.Load(Path.Combine(sdk, "tools/BinaryAssetBuilder.exe.config"));
        var settings = (Settings)new SerializedSectionHandler().Create(null, null, config.SelectSingleNode("/configuration/assetbuilder"));
        settings.DataRoot = inside; settings.SchemaPath = Path.Combine(sdk, "Schemas/xsd/CnC3Types.xsd");
        settings.InputPath = Path.Combine(inside, name + ".xml");
        settings.OutputDirectory = Path.Combine(inside, name, "output");
        settings.IntermediateOutputDirectory = Path.Combine(inside, name, "intermediate");
        settings.SessionCacheDirectory = Path.Combine(inside, name, "hashes");
        settings.DefaultDataPaths = inside + ";" + outside;
        settings.DefaultArtPaths = inside; settings.DefaultAudioPaths = inside;
        settings.TargetPlatform = TargetPlatform.Win32; settings.BuildCache = settings.SessionCache = false;
        settings.LinkedStreams = true; settings.OutputAssetReport = settings.OutputStringHashes = false;
        settings.ErrorLevel = 1; settings.UsePrecompiled = settings.StreamHints = settings.StableSort = false;
        Settings.Current = settings; SettingsLoader.PostProcessSettings(inside);
        Directory.CreateDirectory(settings.OutputDirectory); Directory.CreateDirectory(settings.IntermediateOutputDirectory); Directory.CreateDirectory(settings.SessionCacheDirectory);
        return settings;
    }
    static DocumentProcessor Processor(BuiltinCompilers plugins)
    {
        var schemas = new SchemaSet(false);
        HashProvider.InitializeStringHashes(Settings.Current.SessionCacheDirectory);
        var cache = new SessionCache(); cache.InitializeCache(new List<string>());
        var verifiers = new VerifierPluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32);
        verifiers.DefaultPlugin = (IAssetBuilderVerifierPlugin)Activator.CreateInstance(typeof(DocumentProcessor).Assembly.GetType("BinaryAssetBuilder.GameDataVerifier.Plugin", true), true);
        verifiers.DefaultPlugin.Initialize(null, TargetPlatform.Win32);
        return new DocumentProcessor(Settings.Current, plugins.Registry, verifiers) { Cache = cache, SchemaSet = schemas };
    }
    static void Compile(BuiltinCompilers plugins, string name, string xml)
    {
        var settings = Configure(name);
        File.WriteAllText(settings.InputPath, xml);
        var processor = Processor(plugins);
        bool successful; processor.ProcessDocument(settings.InputPath, true, false, out successful);
        Require(successful, "Real Core compilation did not succeed: " + name);
        int count = 0;
        foreach (string path in Directory.GetFiles(settings.OutputDirectory, "*", SearchOption.AllDirectories))
            if (Array.IndexOf(new[] { ".manifest", ".bin", ".imp", ".relo" }, Path.GetExtension(path)) >= 0) count++;
        Require(count == 4, "Real Core success must produce the complete four-output stream: " + name);
        Require(processor.BuildCacheHits == 0 && processor.BuildCacheMisses == 0, "Boundary tests with build cache disabled must not invent asset-cache use");
    }
    static bool IsBoundaryFailure(Exception error)
    {
        for (var current = error; current != null; current = current.InnerException)
            if (current is IOException && current.Message.StartsWith("Build-cache input", StringComparison.Ordinal)) return true;
        return false;
    }
    static void Reject(Action action, string label)
    {
        bool rejected = false;
        try { action(); } catch (Exception error) { Require(IsBoundaryFailure(error), label + " failed for another reason: " + error); rejected = true; }
        Require(rejected, label + " was accepted outside the content inventory");
    }
    static string Asset(string body) { return "<AssetDeclaration xmlns='" + Ns + "' xmlns:xi='http://www.w3.org/2001/XInclude'>" + body + "</AssetDeclaration>"; }
    static void ScopeCases()
    {
        string external = Path.Combine(outside, "asset.bin");
        CacheInputBoundary.Validate(external);
        File.WriteAllBytes(external, new byte[] { 7, 8, 9 });
        var existenceCached = new FileHashItem(external, null, TargetPlatform.Win32);
        var hashCached = new FileHashItem(external, null, TargetPlatform.Win32);
        Require(existenceCached.Exists, "Seed existing-file metadata outside the boundary");
        Require(hashCached.Hash != 0, "Seed a completed content hash outside the boundary");
        IDisposable owner = CacheInputBoundary.Begin(inside, sdk);
        try
        {
            CacheInputBoundary.Validate(Path.Combine(inside, "absent.xml"));
            Reject(delegate { CacheInputBoundary.Validate(external); }, "Active scope external path");
            Reject(delegate { existenceCached.IsDirty.ToString(); }, "Previously cached existence must not bypass a new boundary before content reads");
            Reject(delegate { existenceCached.Exists.ToString(); }, "Previously cached existence must not bypass a new boundary");
            Reject(delegate { hashCached.Hash.ToString(); }, "Previously completed hashes must not bypass a new boundary");
            bool nestedRejected = false;
            try { CacheInputBoundary.Begin(inside).Dispose(); } catch (InvalidOperationException) { nestedRejected = true; }
            Require(nestedRejected, "A nested scope must not replace the owner's roots");
            Exception threadError = null;
            var thread = new Thread(delegate()
            {
                try { owner.Dispose(); } catch (Exception error) { threadError = error; }
            });
            thread.Start(); Require(thread.Join(30000), "Wrong-thread disposal test did not finish");
            Require(threadError is InvalidOperationException, "Disposal must require the owning thread");
            Reject(delegate { CacheInputBoundary.Validate(external); }, "Wrong-thread disposal must preserve owner's active scope");
        }
        finally { owner.Dispose(); }
        owner.Dispose(); CacheInputBoundary.Validate(external);
        using (CacheInputBoundary.Begin(outside)) CacheInputBoundary.Validate(external);
        CacheInputBoundary.Validate(external);
        tests.Add("opt-in-scope-nesting-rejected-owning-thread-disposal-enforced-owner-preserved-dispose-restores-and-next-scope-opens");
    }
    static void RealRoutes(BuiltinCompilers plugins)
    {
        string probe = File.ReadAllText(Path.Combine(sdk, "Mods/mymod/Data/RecoveryArmorProbe.xml"));
        string included = Write(outside, "included.xml", probe);
        string definitions = Write(outside, "definitions.xml", "<Defines xmlns='" + Ns + "'><Define name='BOUNDARY_PROBE' value='1'/></Defines>");
        string payload = Path.Combine(outside, "asset.bin"); File.WriteAllBytes(payload, new byte[] { 1, 2, 3, 4 });
        string include = Asset("<Includes><Include type='all' source='" + Escape(included) + "'/></Includes>");
        string xinclude = Asset("<xi:include href='" + Escape(new Uri(definitions).AbsoluteUri) + "'/><ArmorTemplate id='BoundaryArmor'/>");
        string fileReference = Asset("<AptAptData id='BoundaryApt' File='" + Escape(payload) + "'/>");
        // With no opt-in boundary the same external references retain their
        // existing public Core behavior and produce actual linked streams.
        Compile(plugins, "ordinary-include", include);
        Compile(plugins, "ordinary-xinclude", xinclude);
        Compile(plugins, "ordinary-file-reference", fileReference);
        using (CacheInputBoundary.Begin(inside, sdk))
        {
            Compile(plugins, "allowed-input", probe);
            Reject(delegate { Compile(plugins, "guarded-include", include); }, "Real Core Include");
            Reject(delegate { Compile(plugins, "guarded-xinclude", xinclude); }, "Real Core XInclude");
            Reject(delegate { Compile(plugins, "guarded-file-reference", fileReference); }, "Real Core schema-declared FileReference");
            Reject(delegate { new FileHashItem(payload, null, TargetPlatform.Win32).Hash.ToString(); }, "Real FileHashItem");
            Configure("external-schema"); Settings.Current.SchemaPath = Path.Combine(outside, "root.xsd");
            File.Copy(Path.Combine(sdk, "Schemas/xsd/CnC3Types.xsd"), Settings.Current.SchemaPath, true);
            Reject(delegate { new SchemaSet(false); }, "Real SchemaSet root");
            string externalSchema = Settings.Current.SchemaPath;
            Settings.Current.SchemaPath = Path.Combine(inside, "schema-with-external-include.xsd");
            File.WriteAllText(Settings.Current.SchemaPath, "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='" + Ns + "'><xs:include schemaLocation='" + Escape(externalSchema) + "'/></xs:schema>");
            Reject(delegate { new SchemaSet(false); }, "Real SchemaSet include");
        }
        Compile(plugins, "ordinary-after-scope", include);
        tests.Add("real-Core-public-compile-inventory-root-success-external-Include-XInclude-file-reference-rejection-disabled-cache-behavior-preserved-before-and-after");
        tests.Add("real-FileHashItem-and-SchemaSet-external-input-rejection");
    }
    public static int Main(string[] args)
    {
        Settings previous = Settings.Current;
        try
        {
            if (args.Length != 5 || IntPtr.Size != 4) throw new Exception("Expected work result sourceCommit runId sdk (x86)");
            work = Path.GetFullPath(args[0]); sdk = Path.GetFullPath(args[4]); inside = Path.Combine(work, "allowed"); outside = Path.Combine(work, "external");
            Directory.CreateDirectory(inside); Directory.CreateDirectory(outside);
            ScopeCases(); Configure("initialize");
            using (var plugins = BuiltinCompilers.Create(TargetPlatform.Win32)) RealRoutes(plugins);
            File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests }));
            Console.WriteLine("PASS CacheBoundary: " + checks + " assertions"); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
        finally { Settings.Current = previous; }
    }
}
