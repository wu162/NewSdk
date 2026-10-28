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
    static void ObservedResolverCases()
    {
        Configure("observed-resolver");
        string first = Path.Combine(inside, "first"), second = Path.Combine(inside, "second"), third = Path.Combine(inside, "third");
        Directory.CreateDirectory(first); Directory.CreateDirectory(second); Directory.CreateDirectory(third);
        string missing = Path.Combine(first, "choice.xml"), selected = Write(second, "choice.xml", "AAAA");
        Write(third, "choice.xml", "LATE");
        Settings.Current.DataPaths = new[] { first, second, third };
        var candidates = new List<string>(); var exists = new List<bool>(); var opened = new List<string>();
        Action<string, bool> probe = delegate(string path, bool present) { candidates.Add(Path.GetFullPath(path)); exists.Add(present); };
        Func<string, Stream> read = delegate(string path) { opened.Add(Path.GetFullPath(path)); return File.OpenRead(path); };
        using (CacheInputBoundary.Begin(inside, sdk))
        using (CacheInputBoundary.BeginObservation(probe, read, delegate(string path) { }))
        {
            string result = FileNameResolver.ResolvePath(inside, "DATA:choice.xml");
            Require(string.Equals(result, selected, StringComparison.OrdinalIgnoreCase), "Observed resolver changed root priority");
            int absent = candidates.FindIndex(p => string.Equals(p, missing, StringComparison.OrdinalIgnoreCase));
            int winner = candidates.FindIndex(p => string.Equals(p, selected, StringComparison.OrdinalIgnoreCase));
            Require(absent >= 0 && winner > absent && !exists[absent] && exists[winner], "Resolver must record the earlier absent candidate before its winner");
            Require(!candidates.Exists(p => string.Equals(p, Path.Combine(third, "choice.xml"), StringComparison.OrdinalIgnoreCase)), "Resolver observation must not invent candidates after the selected root");
            DateTime timestamp = File.GetLastWriteTimeUtc(selected);
            using (var input = CacheInputBoundary.OpenRead(selected))
            using (var reader = new StreamReader(input)) Require(reader.ReadToEnd() == "AAAA", "Observed open must consume its delegated stream");
            Write(second, "choice.xml", "BBBB"); File.SetLastWriteTimeUtc(selected, timestamp);
            Require(new FileInfo(selected).Length == 4 && File.GetLastWriteTimeUtc(selected) == timestamp, "Observed input mutation must preserve length and mtime");
            using (var input = CacheInputBoundary.OpenRead(selected))
            using (var reader = new StreamReader(input)) Require(reader.ReadToEnd() == "BBBB", "Observed reads must not reuse content by length and mtime");
            Require(opened.Count == 2 && opened.TrueForAll(p => string.Equals(p, selected, StringComparison.OrdinalIgnoreCase)), "Every delegated content read must identify its actual file");
            Write(first, "choice.xml", "HIGH"); candidates.Clear(); exists.Clear();
            result = FileNameResolver.ResolvePath(inside, "DATA:choice.xml");
            Require(string.Equals(result, missing, StringComparison.OrdinalIgnoreCase) && candidates.Count > 0 && exists[0], "A newly present higher-priority candidate must replace the old winner");
            File.Delete(missing); File.Delete(selected); candidates.Clear(); exists.Clear();
            result = FileNameResolver.ResolvePath(inside, "DATA:choice.xml");
            Require(string.Equals(result, Path.Combine(third, "choice.xml"), StringComparison.OrdinalIgnoreCase) && exists.Count >= 3 && !exists[0] && !exists[1], "Deleting the old winner must preserve ordered negative probes and select the next root");
            Settings.Current.ArtPaths = new[] { first, second }; Settings.Current.Postfix = "hi";
            string artWinner = Write(Path.Combine(second, "ca"), "cache.bin", "ART");
            candidates.Clear(); exists.Clear();
            result = FileNameResolver.ResolvePath(inside, "ART:cache.bin");
            Require(string.Equals(result, artWinner, StringComparison.OrdinalIgnoreCase), "Observed ART fallback changed the selected payload");
            Require(candidates.Count >= 4 && candidates[0] == Path.Combine(first, "ca", "cache_hi.bin") && candidates[1] == Path.Combine(first, "ca", "cache.bin") && candidates[2] == Path.Combine(second, "ca", "cache_hi.bin") && candidates[3] == artWinner,
                "ART postfix and base-name candidates must retain their actual search order");
            Require(!exists[0] && !exists[1] && !exists[2] && exists[3], "ART observation omitted negative postfix or root candidates");
        }
        int after = opened.Count;
        using (var input = CacheInputBoundary.OpenRead(Path.Combine(third, "choice.xml"))) Require(input.ReadByte() == (byte)'L', "Closing observation must restore ordinary file reads");
        Require(opened.Count == after, "An observation callback leaked after scope disposal");
        tests.Add("observed-resolver-ordered-negative-DATA-and-ART-postfix-probes-new-higher-priority-file-deletion-and-same-length-mtime-fresh-consumption");
    }
    sealed class ObservedMemoryStream : MemoryStream
    {
        public bool Closed;
        public ObservedMemoryStream(byte[] bytes) : base(bytes) { }
        protected override void Dispose(bool disposing) { Closed = true; base.Dispose(disposing); }
    }
    static void ObservedStreamOwnership()
    {
        string input = Write(inside, "observed-frozen.xml", "<disk/>");
        var frozen = new ObservedMemoryStream(System.Text.Encoding.UTF8.GetBytes("<frozen/>"));
        using (CacheInputBoundary.BeginObservation(delegate(string path, bool present) { }, delegate(string path) { return frozen; }, delegate(string path) { }))
        using (var reader = CacheInputBoundary.CreateXmlReader(input))
        {
            reader.MoveToContent();
            Require(reader.Name == "frozen", "XML reader reopened the path instead of consuming the identity-bound delegated stream");
        }
        Require(frozen.Closed, "XML reader disposal must release its delegated stream");
        var invalid = new ObservedMemoryStream(new byte[0]); invalid.Dispose();
        bool rejected = false;
        try
        {
            using (CacheInputBoundary.BeginObservation(delegate(string path, bool present) { }, delegate(string path) { return invalid; }, delegate(string path) { }))
            using (CacheInputBoundary.OpenRead(input)) { }
        }
        catch (IOException) { rejected = true; }
        Require(rejected && invalid.Closed && !CacheInputBoundary.IsObserving, "Unreadable delegated input must fail and release the observation scope");
        rejected = false;
        try
        {
            using (CacheInputBoundary.BeginObservation(delegate(string path, bool present) { }, delegate(string path) { return null; }, delegate(string path) { }))
            using (CacheInputBoundary.OpenRead(input)) { }
        }
        catch (IOException) { rejected = true; }
        Require(rejected && !CacheInputBoundary.IsObserving, "A null delegated stream must fail and release observation state");
        tests.Add("observed-XML-consumes-frozen-bytes-instead-of-reopening-path-stream-disposal-and-null-unreadable-failure");
    }
    static void ObservationLifetimeCases()
    {
        string input = Write(inside, "observation-lifetime.bin", "data");
        int reads = 0;
        Func<string, Stream> read = delegate(string path) { reads++; return File.OpenRead(path); };
        IDisposable observation = CacheInputBoundary.BeginObservation(delegate(string path, bool present) { }, read, delegate(string path) { });
        try
        {
            bool rejected = false;
            try { CacheInputBoundary.BeginObservation(delegate(string path, bool present) { }, read, delegate(string path) { }).Dispose(); }
            catch (InvalidOperationException) { rejected = true; }
            Require(rejected && CacheInputBoundary.IsObserving, "Nested observation must not replace the owner callbacks");
            Exception threadError = null;
            var thread = new Thread(delegate()
            {
                try
                {
                    using (var stream = CacheInputBoundary.OpenRead(input)) Require(stream.ReadByte() == (byte)'d', "Unrelated thread must retain ordinary input reads");
                    observation.Dispose();
                }
                catch (Exception error) { threadError = error; }
            });
            thread.Start(); Require(thread.Join(30000), "Observation disposal thread did not finish");
            Require(threadError is InvalidOperationException && reads == 0, "Observation callbacks and disposal must remain owner-thread scoped");
            using (var stream = CacheInputBoundary.OpenRead(input)) Require(stream.ReadByte() == (byte)'d', "Wrong-thread disposal must preserve the owner callback");
            Require(reads == 1, "Owner callback was lost after rejected wrong-thread disposal");
        }
        finally { observation.Dispose(); }
        observation.Dispose();
        Require(!CacheInputBoundary.IsObserving, "Disposed observation must not remain active");
        var unknown = new List<string>();
        using (CacheInputBoundary.BeginObservation(delegate(string path, bool present) { }, read, unknown.Add))
            CacheInputBoundary.ReportUnknownRead("native-or-legacy-reader");
        CacheInputBoundary.ReportUnknownRead("outside-observation");
        Require(unknown.Count == 1 && unknown[0] == "native-or-legacy-reader", "Unknown readers must remain explicit and scope-owned");
        bool failed = false;
        try
        {
            using (CacheInputBoundary.BeginObservation(delegate(string path, bool present) { }, delegate(string path) { throw new IOException("controlled-observer-read-failure"); }, delegate(string path) { }))
            using (CacheInputBoundary.OpenRead(input)) { }
        }
        catch (IOException error) { Require(error.Message == "controlled-observer-read-failure", "Observed read failure changed its cause"); failed = true; }
        Require(failed, "A delegated read failure must propagate");
        using (CacheInputBoundary.BeginObservation(delegate(string path, bool present) { }, read, delegate(string path) { }))
        using (var stream = CacheInputBoundary.OpenRead(input)) Require(stream.ReadByte() == (byte)'d', "An exceptional observation must release its callbacks for the next scope");
        Require(reads == 2, "The next observation did not own its delegated read");
        tests.Add("observation-owner-thread-nesting-rejection-callback-isolation-exception-cleanup-and-next-scope-restoration");
    }
    static void LeasedObservationCases()
    {
        string input = Write(inside, "observed-leased.bin", "leased bytes"), external = Write(outside, "observed-leased.bin", "external");
        var unknown = new List<string>();
        int reads = 0, callbacks = 0;
        Action<string, bool> probe = delegate(string path, bool present) { };
        Func<string, Stream> reopen = delegate(string path) { reads++; throw new Exception("Leased observation must not reopen an already hashed input"); };
        using (CacheInputBoundary.Begin(inside, sdk))
        using (var lease = new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            byte[] digest;
            using (var hash = System.Security.Cryptography.SHA256.Create()) digest = hash.ComputeHash(lease);
            long length = lease.Length;
            using (CacheInputBoundary.BeginObservation(probe, reopen, unknown.Add))
            {
                CacheInputBoundary.ReportLeasedRead(input, length, digest);
                Require(unknown.Count == 1 && unknown[0] == "leased-input-not-recorded:" + Path.GetFullPath(input), "A three-callback observer must explicitly report an unrecorded lease");
                Reject(delegate { CacheInputBoundary.ReportLeasedRead(external, length, digest); }, "Three-callback leased input boundary");
                Require(unknown.Count == 1, "An out-of-boundary lease must not enter the unknown-read callback");
            }
            Action<string, long, byte[]> leased = delegate(string path, long observedLength, byte[] observedDigest)
            {
                callbacks++;
                Require(path == Path.GetFullPath(input) && observedLength == length, "Leased observation changed the physical path or consumed length");
                Require(observedDigest != null && Convert.ToBase64String(observedDigest) == Convert.ToBase64String(digest), "Leased observation changed the existing SHA256 identity");
            };
            using (CacheInputBoundary.BeginObservation(probe, reopen, unknown.Add, leased))
            {
                CacheInputBoundary.ReportLeasedRead(input, length, digest);
                Require(callbacks == 1 && reads == 0 && unknown.Count == 1, "A recorded leased input must reuse its supplied identity without another read or unknown fallback");
                Reject(delegate { CacheInputBoundary.ReportLeasedRead(external, length, digest); }, "Four-callback leased input boundary");
                var invalidLengths = new long[] { -1, length, length, length };
                var invalidDigests = new byte[][] { digest, null, new byte[31], new byte[33] };
                for (int index = 0; index < invalidLengths.Length; index++)
                {
                    bool rejected = false;
                    try { CacheInputBoundary.ReportLeasedRead(input, invalidLengths[index], invalidDigests[index]); }
                    catch (IOException) { rejected = true; }
                    Require(rejected, "Invalid leased length or SHA256 digest reached observation: " + index);
                }
                Require(callbacks == 1 && reads == 0 && unknown.Count == 1, "Rejected lease metadata must not invoke read or identity callbacks");
            }
            // Observation is opt-in even while the independent root guard is
            // active: no observer means there is no leased evidence to record.
            CacheInputBoundary.ReportLeasedRead(external, -1, null);
            Require(callbacks == 1 && unknown.Count == 1 && !CacheInputBoundary.IsObserving, "A disposed leased observer leaked callbacks or validation into an unrelated call");
            var expected = new IOException("controlled-leased-observer-failure");
            bool failed = false;
            try
            {
                using (CacheInputBoundary.BeginObservation(probe, reopen, unknown.Add, delegate(string path, long observedLength, byte[] observedDigest) { throw expected; }))
                    CacheInputBoundary.ReportLeasedRead(input, length, digest);
            }
            catch (IOException error) { Require(Object.ReferenceEquals(error, expected), "Leased callback failure lost its original cause"); failed = true; }
            Require(failed && !CacheInputBoundary.IsObserving, "A failed leased callback must release observation state");
            using (CacheInputBoundary.BeginObservation(probe, reopen, unknown.Add, leased))
                CacheInputBoundary.ReportLeasedRead(input, length, digest);
            Require(callbacks == 2 && reads == 0 && !CacheInputBoundary.IsObserving, "A later leased observer must work after exceptional scope cleanup");
        }
        tests.Add("leased-read-existing-handle-SHA256-transfer-no-reopen-three-callback-unknown-fallback-boundary-invalid-identity-and-exception-scope-cleanup");
    }
    static void ObservedRealRoutes(BuiltinCompilers plugins)
    {
        string nestedDirectory = Path.Combine(inside, "observed # %");
        string nested = Write(nestedDirectory, "nested.xml", "<ArmorTemplate xmlns='" + Ns + "' id='ObservedNestedArmor'/>");
        string included = Write(nestedDirectory, "observed-include.xml", Asset("<ArmorTemplate id='ObservedIncludeArmor'/><xi:include href='nested.xml'/>"));
        string xincluded = Write(inside, "observed-xinclude.xml", "<ArmorTemplate xmlns='" + Ns + "' id='ObservedXIncludeArmor'/>");
        var reads = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        using (CacheInputBoundary.Begin(inside, sdk))
        using (CacheInputBoundary.BeginObservation(delegate(string path, bool present) { }, delegate(string path) { reads.Add(Path.GetFullPath(path)); return File.OpenRead(path); }, delegate(string path) { }))
        {
            Compile(plugins, "observed-routes", Asset("<Includes><Include type='all' source='" + Escape(included) + "'/></Includes><xi:include href='" + Escape(new Uri(xincluded).AbsoluteUri) + "'/><ArmorTemplate id='ObservedRootArmor'/>") );
        }
        Require(reads.Contains(Path.Combine(inside, "observed-routes.xml")) && reads.Contains(included) && reads.Contains(xincluded) && reads.Contains(nested), "Real XML/Include/XInclude consumption must use observed input streams");
        Require(reads.Contains(Path.GetFullPath(Path.Combine(sdk, "Schemas/xsd/CnC3Types.xsd"))), "Real schema root must use observed input bytes");
        Require(reads.Count > 5, "Real schema includes must contribute their consumed byte paths");
        tests.Add("real-Core-XML-Include-nested-XInclude-special-character-base-URI-and-transitive-schema-read-observation-with-four-output-success");
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
            ScopeCases(); ObservedResolverCases(); ObservationLifetimeCases(); ObservedStreamOwnership(); LeasedObservationCases(); Configure("initialize");
            using (var plugins = BuiltinCompilers.Create(TargetPlatform.Win32)) { RealRoutes(plugins); ObservedRealRoutes(plugins); }
            File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests }));
            Console.WriteLine("PASS CacheBoundary: " + checks + " assertions"); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
        finally { Settings.Current = previous; }
    }
}
