using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using EALA.Hash;

public static class CompilerPhysicalInputs
{
    const string Ns = "uri:ea.com:eala:asset";
    static int checks;
    static readonly List<string> tests = new List<string>();
    static readonly List<object> observations = new List<object>();
    static readonly JavaScriptSerializer Json = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
    static void Require(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static IDictionary<string, object> MetricObject(object value) { return Json.Deserialize<Dictionary<string, object>>(Json.Serialize(value)); }
    static IDictionary<string, object> Metrics(PhysicalInputSnapshot snapshot) { return MetricObject(snapshot.GetMetrics()); }
    static long Number(IDictionary<string, object> metrics, string key) { return Convert.ToInt64(metrics[key]); }
    static uint Legacy(string path)
    {
        var reader = new AsynchronousFileReader(path); uint hash = reader.FileSize;
        while (reader.BeginRead()) { hash = FastHash.GetHashCode(hash, reader.CurrentChunk.Data, reader.CurrentChunk.BytesRead); reader.EndRead(); }
        return hash;
    }
    static string Strong(byte[] bytes) { using (var algorithm = SHA256.Create()) return Convert.ToBase64String(algorithm.ComputeHash(bytes)); }
    static void Blocked(Action operation, string message)
    {
        bool blocked = false;
        try { operation(); }
        catch (IOException) { blocked = true; }
        catch (UnauthorizedAccessException) { blocked = true; }
        Require(blocked, message);
    }
    static void Writable(string path)
    {
        using (var input = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
    }
    static void DigestCases(string work)
    {
        foreach (int length in new[] { 0, 1, 1048575, 1048576, 1048607 })
        {
            string root = Path.Combine(work, "size-" + length); Directory.CreateDirectory(root);
            string path = Path.Combine(root, "input.bin"), moved = root + "-moved";
            byte[] bytes = new byte[length]; for (int index = 0; index < length; index++) bytes[index] = (byte)((index * 19 + 7) % 251);
            File.WriteAllBytes(path, bytes); DateTime time = File.GetLastWriteTimeUtc(path);
            uint legacy = Legacy(path); string sha = Strong(bytes);
            var snapshot = new PhysicalInputSnapshot(Path.Combine(root, "scratch"));
            IDictionary<string, object> frozen; object priorMetrics;
            var first = new FileHashItem(path, "first", TargetPlatform.Win32);
            using (snapshot.BeginReadScope())
            {
                Require(first.IsDirty && first.Hash == legacy, "Sealed legacy hash differs at size " + length);
                var second = new FileHashItem(path, "second", (TargetPlatform)99);
                Require(second.IsDirty && second.Hash == legacy, "Configuration/platform changed physical hash");
                PhysicalInputSnapshot.Entry entry;
                Require(PhysicalInputSnapshot.TryCapture(path, out entry) && entry.Length == length && entry.Sha256 == sha && entry.LegacyHash == legacy,
                    "Sealed entry did not retain exact SHA256/legacy identity");
                Require(PhysicalInputSnapshot.ReadAllBytes(path).SequenceEqual(bytes), "Consumer did not receive original sealed bytes");
                first.Reset(); Require(!first.IsDirty && first.Hash == legacy, "Reset changed an immutable same-build observation");
                priorMetrics = snapshot.GetMetrics(); frozen = MetricObject(priorMetrics);
                Require(Number(frozen, "captures") == 1 && Number(frozen, "capturedBytes") == length && Number(frozen, "hits") >= 3,
                    "One physical input was captured more than once across consumers/configurations");
                Blocked(() => File.WriteAllBytes(path, bytes), "An active sealed file accepted a writer");
                Blocked(() => File.Delete(path), "An active sealed file accepted deletion");
                Blocked(() => Directory.Move(root, moved), "An active sealed ancestor accepted rename/replace");
                snapshot.VerifyUnchanged(); var verified = Metrics(snapshot);
                Require(Number(verified, "verificationReads") == 1 && Number(verified, "verificationBytes") == length,
                    "Final content verification did not reread exact captured bytes");
                Require(Number(MetricObject(priorMetrics), "verificationReads") == 0, "A previous snapshot metric object changed after verification");
                observations.Add(new { length, sealedMetrics = frozen, verifiedMetrics = verified });
            }
            snapshot.Dispose(); snapshot.Dispose(); Writable(path);
            Directory.Move(root, moved); Directory.Move(moved, root);
            Require(!Directory.Exists(Path.Combine(root, "scratch")), "Disposal retained private snapshot scratch");
            if (length > 0)
            {
                bytes[0] ^= 37; File.WriteAllBytes(path, bytes); File.SetLastWriteTimeUtc(path, time);
                Require(File.GetLastWriteTimeUtc(path) == time && new FileInfo(path).Length == length, "Changed generation must preserve time and length");
                using (var next = new PhysicalInputSnapshot(Path.Combine(root, "scratch-next")))
                using (next.BeginReadScope())
                {
                    first.Reset(); Require(first.IsDirty && first.Hash == Legacy(path),
                        "New build reused stale equal-time equal-length content");
                    next.VerifyUnchanged();
                }
                bytes[0] ^= 83; File.WriteAllBytes(path, bytes); File.SetLastWriteTimeUtc(path, time);
                first.Reset(); Require(first.IsDirty && first.Hash == Legacy(path), "Reset outside a snapshot stopped strong content verification");
            }
        }
        tests.Add("same-build-cross-configuration-platform-digest-sharing-legacy-one-MiB-boundaries-reset-and-new-generation-content-proof");
        tests.Add("actual-file-and-parent-directory-pins-writer-delete-rename-denial-final-content-check-and-disposal-release");
    }
    static string Types()
    {
        return "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:a='" + Ns + "' targetNamespace='" + Ns + "'>" +
            "<xs:complexType name='BaseAssetType'/><xs:complexType name='BaseInheritableAsset'><xs:complexContent><xs:extension base='a:BaseAssetType'/></xs:complexContent></xs:complexType>" +
            "<xs:simpleType name='AssetReference'><xs:restriction base='xs:string'/></xs:simpleType><xs:simpleType name='WeakReference'><xs:restriction base='a:AssetReference'/></xs:simpleType><xs:simpleType name='FileReference'><xs:restriction base='xs:string'/></xs:simpleType></xs:schema>";
    }
    static XmlDocument Included(string path)
    {
        using (var input = PhysicalInputSnapshot.OpenRead(path))
        using (var reader = XmlReader.Create(input, new XmlReaderSettings(), new Uri(path, UriKind.Absolute).AbsoluteUri))
        {
            Require(new Uri(reader.BaseURI).LocalPath == path, "Snapshot reader lost logical base URI");
            var resolver = new FileNameXmlResolver(Path.GetDirectoryName(path));
            var including = new Mvp.Xml.XInclude.XIncludingReader(reader) { XmlResolver = resolver };
            var document = new XmlDocument(); document.Load(including); return document;
        }
    }
    static void ReaderCases(string work)
    {
        string root = Path.Combine(work, "source #25% sealed"); Directory.CreateDirectory(root);
        string path = Path.Combine(root, "document.xml"), fragment = Path.Combine(root, "fragment.xml");
        File.WriteAllText(path, "<root xmlns:xi='http://www.w3.org/2001/XInclude'><xi:include href='fragment.xml'/></root>", new UTF8Encoding(false));
        File.WriteAllText(fragment, "<value>sealed-content</value>", new UTF8Encoding(false));
        using (var snapshot = new PhysicalInputSnapshot(Path.Combine(root, "scratch")))
        using (snapshot.BeginReadScope())
        {
            Require(Included(path).DocumentElement.InnerText == "sealed-content" && Included(path).DocumentElement.InnerText == "sealed-content",
                "XInclude did not consume the original source and relative include content");
            var metrics = Metrics(snapshot);
            Require(Number(metrics, "captures") == 2 && Number(metrics, "hits") >= 2, "Root/XInclude consumers did not share exact physical snapshots");
            Blocked(() => File.WriteAllText(fragment, "<value>mutate-content</value>"), "XInclude payload was not pinned");
            snapshot.VerifyUnchanged(); observations.Add(new { id = "xinclude", metrics = Metrics(snapshot) });
        }
        Settings previous = Settings.Current;
        string schema = Path.Combine(root, "root.xsd"), types = Path.Combine(root, "types.xsd");
        File.WriteAllText(schema, "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='" + Ns + "'><xs:include schemaLocation='types.xsd'/></xs:schema>", new UTF8Encoding(false));
        File.WriteAllText(types, Types(), new UTF8Encoding(false));
        try
        {
            Settings.Current = new Settings { SchemaPath = schema, StringHashBinDescriptors = new StringHashBinDescriptor[0] };
            using (var snapshot = new PhysicalInputSnapshot(Path.Combine(root, "schema-scratch")))
            using (snapshot.BeginReadScope())
            {
                var first = new SchemaSet(false); var second = new SchemaSet(false);
                Require(!Object.ReferenceEquals(first.Schemas, second.Schemas), "Physical sharing leaked Framework schema instances");
                Require(first.GetDerivedTypes("BaseAssetType").Cast<string>().SequenceEqual(second.GetDerivedTypes("BaseAssetType").Cast<string>()),
                    "Schema snapshot consumers changed inheritance metadata");
                var metrics = Metrics(snapshot);
                Require(Number(metrics, "captures") == 2 && Number(metrics, "hits") >= 2, "Two schema consumers reread the same positive closure");
                first.ReloadIfChanged(false); snapshot.VerifyUnchanged();
                observations.Add(new { id = "schema", metrics = Metrics(snapshot) });
            }
        }
        finally { Settings.Current = previous; }
        tests.Add("real-XInclude-relative-escaped-base-URI-and-schema-include-consumers-share-bytes-with-private-Framework-state");
    }
    sealed class TrackedXmlInput : MemoryStream
    {
        readonly string name;
        readonly IList<string> closes;
        internal Exception CloseFailure;
        internal bool Closed;
        internal TrackedXmlInput(string name, string xml, IList<string> closes) : base(Encoding.UTF8.GetBytes(xml))
        { this.name = name; this.closes = closes; }
        protected override void Dispose(bool disposing)
        {
            bool first = !Closed; Closed = true;
            base.Dispose(disposing);
            if (disposing && first)
            {
                closes.Add(name);
                if (CloseFailure != null) throw CloseFailure;
            }
        }
    }
    sealed class TrackedXmlResolver : XmlUrlResolver
    {
        internal readonly Dictionary<string, TrackedXmlInput> Inputs = new Dictionary<string, TrackedXmlInput>(StringComparer.Ordinal);
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        { return Inputs[Path.GetFileName(absoluteUri.LocalPath)]; }
    }
    sealed class UnreadableResolverStream : MemoryStream
    {
        internal readonly IOException Primary = new IOException("Resolver stream CanRead failure");
        internal readonly IOException Cleanup = new IOException("Resolver stream close failure");
        internal int CloseCalls;
        public override bool CanRead { get { throw Primary; } }
        protected override void Dispose(bool disposing)
        {
            if (disposing) CloseCalls++;
            base.Dispose(disposing);
            if (disposing && CloseCalls == 1) throw Cleanup;
        }
    }
    sealed class AcquiredTextReader : StringReader
    {
        internal int CloseCalls;
        internal AcquiredTextReader() : base("<included/>") { }
        public override void Close() { CloseCalls++; base.Close(); }
    }
    sealed class UnreadableBaseReader : XmlTextReader
    {
        internal readonly IOException Primary = new IOException("Resolver reader BaseURI failure");
        internal int CloseCalls;
        internal UnreadableBaseReader() : base(new StringReader("<included/>")) { }
        public override string BaseURI { get { throw Primary; } }
        public override void Close() { CloseCalls++; base.Close(); }
    }
    sealed class CountingRootReader : XmlTextReader
    {
        internal int CloseCalls;
        internal CountingRootReader(string uri, string xml) : base(uri, new StringReader(xml)) { }
        public override void Close() { CloseCalls++; base.Close(); }
    }
    sealed class ObjectXmlResolver : XmlUrlResolver
    {
        internal object Input;
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn) { return Input; }
    }
    static bool CausedBy(Exception observed, Exception expected)
    {
        for (Exception current = observed; current != null; current = current.InnerException)
            if (Object.ReferenceEquals(current, expected)) return true;
        return false;
    }
    static void AcquiredReaderFailureCases(string work)
    {
        foreach (string mode in new[] { "stream", "stream-fallback", "reader", "xpointer" })
        {
            object input; Exception primary = null;
            if (mode.StartsWith("stream", StringComparison.Ordinal))
            { var stream = new UnreadableResolverStream(); input = stream; primary = stream.Primary; }
            else if (mode == "reader") { var reader = new UnreadableBaseReader(); input = reader; primary = reader.Primary; }
            else input = new AcquiredTextReader();
            bool fallback = mode.EndsWith("fallback", StringComparison.Ordinal), pointer = mode == "reader" || mode == "xpointer";
            string xml = "<root xmlns:xi='http://www.w3.org/2001/XInclude'><xi:include href='acquired.xml'" +
                (pointer ? " xpointer='xpointer('" : "") + ">" +
                (fallback ? "<xi:fallback><fallback-selected/></xi:fallback>" : "") + "</xi:include></root>";
            var root = new CountingRootReader(new Uri(Path.Combine(work, "acquire-" + mode + ".xml"), UriKind.Absolute).AbsoluteUri, xml);
            var including = new Mvp.Xml.XInclude.XIncludingReader(root) { XmlResolver = new ObjectXmlResolver { Input = input } };
            Exception observed = null; bool selected = false;
            try
            {
                try { while (including.Read()) if (including.LocalName == "fallback-selected") selected = true; }
                catch (Exception error) { observed = error; }
                Require(fallback ? observed == null && selected : observed != null && !selected,
                    "Acquisition failure changed the real xi:fallback versus fatal-resource boundary: " + mode);
                if (primary != null)
                {
                    if (!fallback) Require(CausedBy(observed, primary), "Acquired-input cleanup replaced the original constructor/property error");
                    if (input is UnreadableResolverStream)
                    {
                        var stream = (UnreadableResolverStream)input;
                        Require(stream.CloseCalls == 1 && Object.ReferenceEquals(primary.Data["MvpXml.AcquisitionCleanupFailure"], stream.Cleanup),
                            "Failed StreamReader construction leaked its acquired source or replaced its primary error with close failure");
                    }
                    else Require(((UnreadableBaseReader)input).CloseCalls == 1, "A failed BaseURI getter leaked its untransferred acquired XmlReader");
                }
                else
                {
                    Require(((AcquiredTextReader)input).CloseCalls == 1 && observed.ToString().Contains("XPointer"),
                        "Bad XPointer construction did not preserve its error or closed the already-consumed infoset source twice");
                }
                including.Close();
                Require(root.CloseCalls == 1 && including.NameTable != null, "Failed acquired-input transfer left its parent duplicated on the reader stack");
                observations.Add(new { id = "XInclude-acquisition-transfer-" + mode, primaryPreserved = primary == null || fallback || CausedBy(observed, primary),
                    fallbackSelected = selected, acquiredCloseCalls = input is UnreadableResolverStream ? ((UnreadableResolverStream)input).CloseCalls :
                        input is UnreadableBaseReader ? ((UnreadableBaseReader)input).CloseCalls : ((AcquiredTextReader)input).CloseCalls,
                    rootCloseCalls = root.CloseCalls,
                    scope = "Actual custom resolver result and StreamReader/BaseURI/XPointer construction failure; no fabricated reader stack" });
            }
            finally { if (root.ReadState != ReadState.Closed) including.Close(); }
        }
        tests.Add("real-custom-XInclude-resolver-acquired-stream-reader-and-text-reader-construction-failures-close-before-transfer-preserve-primary-fallback-parent-state-and-no-double-infoset-close");
    }
    static void NestedReaderCloseCases(string work)
    {
        var closes = new List<string>();
        var root = new TrackedXmlInput("root", "<root xmlns:xi='http://www.w3.org/2001/XInclude'><xi:include href='outer.xml'/></root>", closes);
        var outer = new TrackedXmlInput("outer", "<outer xmlns:xi='http://www.w3.org/2001/XInclude'><xi:include href='inner.xml'/></outer>", closes);
        var inner = new TrackedXmlInput("inner", "<inner/>", closes);
        IOException first = new IOException("First nested reader close failure"), second = new IOException("Second nested reader close failure");
        inner.CloseFailure = first; outer.CloseFailure = second;
        var resolver = new TrackedXmlResolver(); resolver.Inputs.Add("outer.xml", outer); resolver.Inputs.Add("inner.xml", inner);
        var source = new XmlTextReader(new Uri(Path.Combine(work, "close-root.xml"), UriKind.Absolute).AbsoluteUri, root);
        var including = new Mvp.Xml.XInclude.XIncludingReader(source) { XmlResolver = resolver };
        bool entered = false, primary = false;
        try
        {
            while (including.Read())
                if (including.NodeType == XmlNodeType.Element && including.LocalName == "inner") { entered = true; break; }
            Require(entered && !root.Closed && !outer.Closed && !inner.Closed, "Real XInclude did not hold all three active nested input readers");
            try { including.Close(); } catch (IOException error) { primary = Object.ReferenceEquals(error, first); }
            Require(primary && root.Closed && outer.Closed && inner.Closed && String.Join(",", closes) == "inner,outer,root",
                "Nested Close stopped at an error, reordered owner cleanup or replaced the first close error");
            Require(including.NameTable != null && source.ReadState == ReadState.Closed, "Nested Close lost the legacy closed root NameTable");
            including.Close();
            Require(String.Join(",", closes) == "inner,outer,root", "Repeated closed-reader cleanup reopened or reclosed an input stream");
        }
        finally
        {
            inner.CloseFailure = outer.CloseFailure = null;
            including.Close(); root.Dispose(); outer.Dispose(); inner.Dispose();
        }
        observations.Add(new { id = "nested-XInclude-close-error-order", primaryPreserved = primary, closeOrder = closes.ToArray(),
            scope = "Actual nested XIncludingReader with tracked owned streams; deterministic Close failures after each underlying stream was closed" });
        tests.Add("real-XInclude-early-close-drains-current-and-parent-readers-in-original-order-retains-first-error-and-closed-root-name-table");
    }
    static void CoreXmlReaderFailureCases(string work)
    {
        string root = Path.Combine(work, "core-xml-reader-failure"); Directory.CreateDirectory(root);
        string source = Path.Combine(root, "root.xml"), outer = Path.Combine(root, "outer.xml"), inner = Path.Combine(root, "inner.xml");
        File.WriteAllText(source, "<AssetDeclaration xmlns='" + Ns + "' xmlns:xi='http://www.w3.org/2001/XInclude'><xi:include href='outer.xml'/></AssetDeclaration>");
        File.WriteAllText(outer, "<container xmlns='" + Ns + "' xmlns:xi='http://www.w3.org/2001/XInclude'><xi:include href='inner.xml'/></container>");
        File.WriteAllText(inner, "<ArmorTemplate xmlns='" + Ns + "' id='Broken'>" + new string(' ', 8192) + "<Unclosed></ArmorTemplate>");
        Settings previous = Settings.Current;
        try
        {
            Settings.Current = new Settings { BuildCache = false, SessionCache = false, TargetPlatform = TargetPlatform.Win32 };
            var processor = new DocumentProcessor(Settings.Current, new PluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32), null);
            var document = new AssetDeclarationDocument();
            var stateType = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
            object state = Activator.CreateInstance(stateType, true); stateType.GetMethod("FromScratch").Invoke(state, new object[0]);
            stateType.GetField("SourcePath").SetValue(state, source); stateType.GetField("SourceDirectory").SetValue(state, root);
            stateType.GetField("LogicalSourcePath").SetValue(state, "root.xml"); stateType.GetField("DocumentProcessor").SetValue(state, processor);
            typeof(AssetDeclarationDocument).GetField("_Current", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(document, state);
            bool rejected = false;
            try { document.LoadXml(false); }
            catch (BinaryAssetBuilderException error) { rejected = error.ErrorCode == ErrorCode.XmlFormattingError && error.InnerException is XmlException; }
            Require(rejected, "Malformed nested include changed Core's original XmlFormattingError/XmlException boundary");
            XmlDocument failedDom = document.XmlDocument;
            // This scope deliberately has no PhysicalInputSnapshot: the
            // supported direct/budget fallback must release its real file
            // readers immediately, without a GC/finalizer or an input pin.
            Writable(source); Writable(outer); Writable(inner);
            File.WriteAllText(inner, "<ArmorTemplate xmlns='" + Ns + "' id='Repaired' Default='10'/>");
            document.Reset(); document.LoadXml(false);
            Require(document.XmlDocument.GetElementsByTagName("ArmorTemplate", Ns).Count == 1 &&
                ((XmlElement)document.XmlDocument.GetElementsByTagName("ArmorTemplate", Ns)[0]).GetAttribute("id") == "Repaired",
                "The same actual Core document could not reload a repaired nested include");
            var sourceInfo = (System.Collections.ICollection)stateType.GetField("NodeSourceInfoSet").GetValue(state); int before = sourceInfo.Count;
            failedDom.DocumentElement.AppendChild(failedDom.CreateElement("detached-failure-dom"));
            Require(sourceInfo.Count == before, "A failed DOM retained the Core NodeInserted subscription and wrote into the next load's source metadata");
            Writable(source); Writable(outer); Writable(inner);
            observations.Add(new { id = "actual-Core-nested-parse-failure-repair", originalErrorCode = ErrorCode.XmlFormattingError.ToString(),
                immediateExclusiveSources = 3, sameDocumentReload = true, failedDomSubscriptionDetached = true,
                scope = "Actual public Core LoadXml with initialized parse state and real file fallback; not an asset compiler or SDK output oracle" });
        }
        finally { Settings.Current = previous; }
        tests.Add("actual-Core-LoadXml-nested-malformed-XInclude-preserves-primary-closes-three-file-readers-before-repair-detaches-failed-DOM-handler-and-same-document-reload");
    }
    sealed class CancellingResolver : FileNameXmlResolver
    {
        readonly CancellationTokenSource cancellation;
        public CancellingResolver(string directory, CancellationTokenSource cancellation) : base(directory) { this.cancellation = cancellation; }
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            cancellation.Cancel();
            return base.GetEntity(absoluteUri, role, ofObjectToReturn);
        }
    }
    static void CancellationCases(string work)
    {
        string root = Path.Combine(work, "cancellation"); Directory.CreateDirectory(root);
        string schema = Path.Combine(root, "types.xsd"); File.WriteAllText(schema, Types(), new UTF8Encoding(false));
        Settings previous = Settings.Current;
        try
        {
            Settings.Current = new Settings { SchemaPath = schema, StringHashBinDescriptors = new StringHashBinDescriptor[0] };
            var snapshot = new PhysicalInputSnapshot(Path.Combine(root, "schema-scratch"));
            using (var cancellation = new CancellationTokenSource())
            {
                cancellation.Cancel(); bool cancelled = false;
                try
                {
                    using (snapshot.BeginReadScope(cancellation.Token))
                    {
                        try { new SchemaSet(false); }
                        catch (OperationCanceledException error) { cancelled = true; Require(error.CancellationToken == cancellation.Token, "Schema cancellation changed its token"); }
                    }
                    Require(cancelled, "Cancelled schema input was swallowed or converted to ReadingSchema");
                }
                finally { snapshot.Dispose(); }
                Require(Number(Metrics(snapshot), "activeFilePins") == 0 && Number(Metrics(snapshot), "activeDirectoryPins") == 0,
                    "Cancelled schema preparation retained input pins");
            }
        }
        finally { Settings.Current = previous; }
        string source = Path.Combine(root, "source.xml"), child = Path.Combine(root, "cancel-fragment.xml");
        File.WriteAllText(source, "<root xmlns:xi='http://www.w3.org/2001/XInclude'><xi:include href='cancel-fragment.xml'><xi:fallback><fallback-selected/></xi:fallback></xi:include></root>", new UTF8Encoding(false));
        File.WriteAllText(child, "<value>must-not-consume</value>", new UTF8Encoding(false));
        var includeSnapshot = new PhysicalInputSnapshot(Path.Combine(root, "include-scratch"));
        using (var cancellation = new CancellationTokenSource())
        {
            bool cancelled = false; var document = new XmlDocument();
            try
            {
                using (includeSnapshot.BeginReadScope(cancellation.Token))
                using (var input = PhysicalInputSnapshot.OpenRead(source))
                using (var reader = XmlReader.Create(input, new XmlReaderSettings { CloseInput = true }, new Uri(source, UriKind.Absolute).AbsoluteUri))
                using (var including = new Mvp.Xml.XInclude.XIncludingReader(reader) { XmlResolver = new CancellingResolver(root, cancellation) })
                {
                    try { document.Load(including); }
                    catch (OperationCanceledException error) { cancelled = true; Require(error.CancellationToken == cancellation.Token, "XInclude cancellation changed its token"); }
                }
                Require(cancelled && document.GetElementsByTagName("fallback-selected").Count == 0,
                    "XInclude converted cancellation into a resource error or selected xi:fallback");
            }
            finally { includeSnapshot.Dispose(); }
            Require(Number(Metrics(includeSnapshot), "activeFilePins") == 0 && Number(Metrics(includeSnapshot), "activeDirectoryPins") == 0,
                "Cancelled XInclude retained its previously captured root pin");
            Writable(source); Writable(child);
        }
        tests.Add("schema-and-real-XInclude-cancellation-retain-token-and-bypass-resource-fallback-wrappers-without-pin-leaks");
    }
    sealed class VerificationStream : Stream
    {
        readonly MemoryStream source;
        readonly byte[] expectedBuffer;
        readonly int chunk, cancelRead;
        readonly bool cancelEof;
        readonly CancellationTokenSource cancellation;
        public int Reads { get; private set; }
        public bool ReusedBuffer { get; private set; }
        public VerificationStream(byte[] data, byte[] buffer, int chunk, CancellationTokenSource cancellation = null, int cancelRead = 0, bool cancelEof = false)
        {
            source = new MemoryStream(data, false); expectedBuffer = buffer; this.chunk = chunk; this.cancellation = cancellation;
            this.cancelRead = cancelRead; this.cancelEof = cancelEof; ReusedBuffer = true;
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            Reads++; ReusedBuffer &= Object.ReferenceEquals(buffer, expectedBuffer);
            int read = source.Read(buffer, offset, Math.Min(count, chunk));
            if (cancellation != null && (Reads == cancelRead || (cancelEof && read == 0))) cancellation.Cancel();
            return read;
        }
        public override bool CanRead { get { return true; } }
        public override bool CanWrite { get { return false; } }
        public override bool CanSeek { get { return true; } }
        public override long Length { get { return source.Length; } }
        public override long Position { get { return source.Position; } set { source.Position = value; } }
        public override long Seek(long offset, SeekOrigin origin) { return source.Seek(offset, origin); }
        public override void Flush() { }
        public override void SetLength(long value) { throw new NotSupportedException(); }
        public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
        protected override void Dispose(bool disposing) { if (disposing) source.Dispose(); base.Dispose(disposing); }
    }
    static string VerificationHash(Stream input, byte[] buffer, CancellationToken cancellation)
    {
        var method = typeof(PhysicalInputSnapshot).GetMethod("ComputeVerificationHash", BindingFlags.NonPublic | BindingFlags.Static);
        Require(method != null, "Physical input verification hash helper is missing");
        try { return (string)method.Invoke(null, new object[] { input, buffer, cancellation }); }
        catch (TargetInvocationException error) { throw error.InnerException; }
    }
    static byte[] VerificationBytes(int length)
    { byte[] bytes = new byte[length]; for (int i = 0; i < length; i++) bytes[i] = (byte)((i * 23 + 11) % 251); return bytes; }
    static void VerificationCancellationCases(string work)
    {
        byte[] buffer = new byte[65536];
        int[] lengths = { 0, 1, 65535, 65536, 65537, 1048607, 0 };
        int[] chunks = { 65536, 1, 17, 65535, 37, 16381, 65536 };
        for (int i = 0; i < lengths.Length; i++)
        {
            byte[] bytes = VerificationBytes(lengths[i]);
            using (var input = new VerificationStream(bytes, buffer, chunks[i]))
            {
                Require(VerificationHash(input, buffer, CancellationToken.None) == Strong(bytes), "Chunked verification SHA differs from SHA256.ComputeHash: " + lengths[i]);
                Require(input.Position == bytes.Length && input.ReusedBuffer && input.Reads >= 1, "Verification truncated a short read or replaced its provided buffer");
            }
        }
        foreach (int read in new[] { 1, 2, 5 })
            using (var cancellation = new CancellationTokenSource())
            using (var input = new VerificationStream(VerificationBytes(1048607), buffer, 65536, cancellation, read))
            {
                bool cancelled = false;
                try { VerificationHash(input, buffer, cancellation.Token); }
                catch (OperationCanceledException error) { cancelled = true; Require(error.CancellationToken == cancellation.Token, "Read-triggered verification cancellation changed its token"); }
                Require(cancelled && input.Reads == read && input.ReusedBuffer, "Verification did not stop immediately after the deterministic cancelling read");
            }
        foreach (int length in new[] { 0, 1, 65537 })
            using (var cancellation = new CancellationTokenSource())
            using (var input = new VerificationStream(VerificationBytes(length), buffer, 65536, cancellation, 0, true))
            {
                bool cancelled = false;
                try { VerificationHash(input, buffer, cancellation.Token); }
                catch (OperationCanceledException error) { cancelled = true; Require(error.CancellationToken == cancellation.Token, "EOF verification cancellation changed its token"); }
                Require(cancelled && input.Position == length && input.Reads == (length + 65535) / 65536 + 1, "EOF cancellation was accepted as a successful final digest");
            }
        using (var cancellation = new CancellationTokenSource())
        using (var input = new VerificationStream(new byte[1], buffer, 1))
        {
            cancellation.Cancel(); bool cancelled = false;
            try { VerificationHash(input, buffer, cancellation.Token); }
            catch (OperationCanceledException error) { cancelled = true; Require(error.CancellationToken == cancellation.Token, "Pre-read cancellation changed its token"); }
            Require(cancelled && input.Reads == 0, "Already cancelled verification consumed input bytes");
        }
        string root = Path.Combine(work, "verification-cancellation"); Directory.CreateDirectory(root);
        string first = Path.Combine(root, "empty.bin"), second = Path.Combine(root, "payload.bin");
        File.WriteAllBytes(first, new byte[0]); File.WriteAllBytes(second, VerificationBytes(1048607));
        var snapshot = new PhysicalInputSnapshot(Path.Combine(root, "scratch"));
        try
        {
            using (snapshot.BeginReadScope())
            {
                Require(PhysicalInputSnapshot.ReadAllBytes(first).Length == 0 && PhysicalInputSnapshot.ReadAllBytes(second).Length == 1048607, "Verification integration did not seal its original inputs");
                using (var cancellation = new CancellationTokenSource())
                {
                    cancellation.Cancel(); bool cancelled = false;
                    try { snapshot.VerifyUnchanged(cancellation.Token); }
                    catch (OperationCanceledException error) { cancelled = true; Require(error.CancellationToken == cancellation.Token, "Snapshot verification cancellation changed its token"); }
                    Require(cancelled && Number(Metrics(snapshot), "verificationReads") == 0 && Number(Metrics(snapshot), "verificationBytes") == 0,
                        "Cancelled verification was reported as successful logical reads/bytes");
                }
                Blocked(() => File.Delete(second), "Cancelled verification released an input pin before the generation ended");
                snapshot.VerifyUnchanged(CancellationToken.None); var metrics = Metrics(snapshot);
                Require(Number(metrics, "verificationReads") == 2 && Number(metrics, "verificationBytes") == 1048607, "Valid-token retry skipped or double-counted incomplete verification");
                observations.Add(new { id = "cancelled-final-verification-and-valid-token-retry", metrics });
            }
        }
        finally { snapshot.Dispose(); }
        var final = Metrics(snapshot);
        Require(Number(final, "activeFilePins") == 0 && Number(final, "activeAliasPins") == 0 && Number(final, "activeReadHandles") == 0 && Number(final, "activeDirectoryPins") == 0,
            "Final verification cancellation/retry left file, alias or directory pins");
        Writable(first); Writable(second);
        tests.Add("actual-verification-hash-helper-reuses-buffer-preserves-SHA-on-short-read-boundaries-and-propagates-deterministic-read-and-EOF-cancellation-tokens");
        tests.Add("actual-snapshot-cancelled-final-verification-does-not-claim-success-keeps-pins-valid-token-retry-completes-and-disposal-releases-pins");
    }
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "CreateHardLinkW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool CreateHardLink(string link, string existing, IntPtr security);
    static void AliasCases(string work)
    {
        string root = Path.Combine(work, "physical-alias"), other = Path.Combine(work, "distinct-identity");
        Directory.CreateDirectory(root); Directory.CreateDirectory(other);
        string path = Path.Combine(root, "mixed-case.bin"), link = Path.Combine(root, "same-volume-hardlink.bin"), copy = Path.Combine(other, "same-content.bin");
        byte[] bytes = Enumerable.Range(0, 64).Select(index => (byte)(index * 17 + 3)).ToArray(); File.WriteAllBytes(path, bytes); File.WriteAllBytes(copy, bytes);
        Require(Path.GetPathRoot(path) == Path.GetPathRoot(link) && CreateHardLink(link, path, IntPtr.Zero),
            "Unable to create a same-volume hard link: Win32 " + Marshal.GetLastWin32Error());
        using (var snapshot = new PhysicalInputSnapshot(Path.Combine(root, "scratch")))
        using (snapshot.BeginReadScope())
        {
            PhysicalInputSnapshot.Entry first = null, caseAlias = null, hardAlias = null, distinct = null;
            Require(PhysicalInputSnapshot.TryCapture(path, out first) && PhysicalInputSnapshot.TryCapture(path.ToUpperInvariant(), out caseAlias) && PhysicalInputSnapshot.TryCapture(link, out hardAlias),
                "Positive aliases could not establish their actual file identities");
            Require(Object.ReferenceEquals(first, caseAlias) && Object.ReferenceEquals(first, hardAlias) &&
                PhysicalInputSnapshot.ReadAllBytes(link).SequenceEqual(bytes), "Case/hard-link aliases did not share the same sealed entry");
            var aliases = Metrics(snapshot);
            Require(Number(aliases, "captures") == 1 && Number(aliases, "hits") >= 3 && Number(aliases, "capturedBytes") == bytes.Length && Number(aliases, "activeFilePins") == 1,
                "Same-volume aliases duplicated physical capture or file pins");
            Blocked(() => File.WriteAllBytes(path, bytes), "Original hard-linked pathname accepted a writer");
            Blocked(() => File.WriteAllBytes(link, bytes), "Second hard-linked pathname bypassed the physical file pin");
            Blocked(() => File.Delete(link), "A hard-linked alias could be removed while its content was sealed");
            Require(PhysicalInputSnapshot.TryCapture(copy, out distinct) && !Object.ReferenceEquals(first, distinct) && distinct.Sha256 == first.Sha256,
                "Distinct same-content files were conflated instead of respecting physical file identity");
            Require(Number(Metrics(snapshot), "captures") == 2 && Number(Metrics(snapshot), "activeFilePins") == 2,
                "Physical identity deduplication crossed distinct file identities");
            snapshot.VerifyUnchanged();
            Require(Number(Metrics(snapshot), "verificationReads") == 2 && Number(Metrics(snapshot), "verificationBytes") == 2 * bytes.Length,
                "Strong verification did not separate unique identities from their aliases");
            observations.Add(new { id = "same-volume-case-and-hard-link-aliases", aliases, verified = Metrics(snapshot) });
        }
        Writable(path); Writable(link); Writable(copy); File.Delete(link);
        Require(File.Exists(path) && File.ReadAllBytes(path).SequenceEqual(bytes), "Released hard-link cleanup changed original content");
        tests.Add("same-volume-hard-link-and-case-aliases-share-one-file-identity-distinct-same-bytes-remain-separate-and-pins-release");
    }
    static void BudgetCases(string work)
    {
        string root = Path.Combine(work, "budgets"); Directory.CreateDirectory(root);
        string first = Path.Combine(root, "first.bin"), second = Path.Combine(root, "second.bin");
        byte[] bytes = Enumerable.Range(0, 32).Select(index => (byte)index).ToArray(); File.WriteAllBytes(first, bytes); File.WriteAllBytes(second, bytes);
        using (var snapshot = new PhysicalInputSnapshot(Path.Combine(root, "disk"), 0, 128, 4, 64))
        using (snapshot.BeginReadScope())
        {
            Require(PhysicalInputSnapshot.ReadAllBytes(first).SequenceEqual(bytes), "Disk-backed snapshot changed bytes");
            var metrics = Metrics(snapshot);
            Require(Number(metrics, "captures") == 1 && Number(metrics, "diskBytes") == bytes.Length && Number(metrics, "memoryBytes") == 0,
                "A zero-memory budget did not use the bounded disk path");
            snapshot.VerifyUnchanged();
        }
        using (var snapshot = new PhysicalInputSnapshot(Path.Combine(root, "limited"), 128, 0, 1, 64))
        using (snapshot.BeginReadScope())
        {
            PhysicalInputSnapshot.Entry entry;
            Require(PhysicalInputSnapshot.TryCapture(first, out entry), "First file in finite budget did not capture");
            Require(!PhysicalInputSnapshot.TryCapture(second, out entry), "File limit did not conservatively bypass");
            Require(PhysicalInputSnapshot.ReadAllBytes(second).SequenceEqual(bytes), "Budget bypass changed original read behavior");
            Require(Number(Metrics(snapshot), "bypasses") > 0 && Number(Metrics(snapshot), "captures") == 1, "Budget bypass did not record separate coverage");
            Writable(second); snapshot.VerifyUnchanged();
        }
        using (var snapshot = new PhysicalInputSnapshot(Path.Combine(root, "empty"), 0, 0, 4, 64))
        using (snapshot.BeginReadScope())
        {
            Require(PhysicalInputSnapshot.ReadAllBytes(first).SequenceEqual(bytes), "No-capacity fallback changed original bytes");
            Require(Number(Metrics(snapshot), "captures") == 0 && Number(Metrics(snapshot), "activeFilePins") == 0 && Number(Metrics(snapshot), "activeDirectoryPins") == 0,
                "No-capacity bypass leaked a pin or authorized a capture");
            Writable(first); string moved = root + "-bypass-moved";
            Directory.Move(root, moved); Directory.Move(moved, root);
        }
        string target = Path.Combine(work, "reparse-target"), link = Path.Combine(work, "reparse-link");
        Directory.CreateDirectory(target); string targetFile = Path.Combine(target, "input.bin"); File.WriteAllBytes(targetFile, bytes);
        var start = new ProcessStartInfo("cmd.exe", "/d /c mklink /J \"" + link + "\" \"" + target + "\"")
            { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
        using (var process = Process.Start(start))
        {
            string log = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd(); process.WaitForExit();
            Require(process.ExitCode == 0, "Unable to create input fallback junction: " + log);
        }
        try
        {
            using (var snapshot = new PhysicalInputSnapshot(Path.Combine(work, "reparse-scratch")))
            using (snapshot.BeginReadScope())
            {
                PhysicalInputSnapshot.Entry entry;
                Require(!PhysicalInputSnapshot.TryCapture(Path.Combine(link, "input.bin"), out entry), "Reparse path authorized a physical content capture");
                Require(PhysicalInputSnapshot.ReadAllBytes(Path.Combine(link, "input.bin")).SequenceEqual(bytes), "Reparse fallback changed legacy bytes");
                Require(!PhysicalInputSnapshot.TryCapture(@"\\?\" + first, out entry) && !PhysicalInputSnapshot.TryCapture(first + ":untracked-ads", out entry),
                    "Device/ADS spelling authorized a physical content capture");
                Require(Number(Metrics(snapshot), "captures") == 0 && Number(Metrics(snapshot), "activeDirectoryPins") == 0,
                    "Unsupported path fallback retained an unneeded ancestor pin");
                Writable(targetFile); string moved = target + "-moved"; Directory.Move(target, moved); Directory.Move(moved, target);
            }
        }
        finally { Directory.Delete(link); }
        tests.Add("finite-memory-disk-file-budgets-and-reparse-device-ADS-retain-legacy-bypass-without-sharing-or-pin-leaks");
    }
    static IDictionary<string, object> CandidateMetrics(PhysicalInputSnapshot snapshot)
    { return (IDictionary<string, object>)Metrics(snapshot)["resolverCandidates"]; }
    static void Changed(PhysicalInputSnapshot snapshot, string message)
    {
        bool rejected = false;
        try { snapshot.VerifyUnchanged(); }
        catch (IOException error) { rejected = error.Message.Contains("Resolver candidate existence changed"); }
        Require(rejected, message);
    }
    static void CandidateCases(string work)
    {
        string root = Path.Combine(work, "resolver-consistency"), high = Path.Combine(root, "high"), low = Path.Combine(root, "low");
        Directory.CreateDirectory(high); Directory.CreateDirectory(low);
        string name = "candidate.bin", first = Path.Combine(high, name), second = Path.Combine(low, name);
        File.WriteAllText(second, "lower-priority");
        Settings previous = Settings.Current;
        try
        {
            Settings.Current = new Settings { DataRoot = root, DataPaths = new[] { high, low }, ArtPaths = new[] { high, low }, AudioPaths = new[] { high, low } };
            using (var snapshot = new PhysicalInputSnapshot(Path.Combine(root, "scratch")))
            using (snapshot.BeginReadScope())
            {
                Require(FileNameResolver.ResolvePath(root, "data:" + name) == second, "Initial resolver choice changed");
                Require(FileNameResolver.ResolvePath(root, "data:" + name) == second, "Repeated resolver choice changed");
                var before = CandidateMetrics(snapshot);
                Require(Number(before, "observations") == 4 && Number(before, "uniquePaths") == 2 && Number(before, "verificationChecks") == 0,
                    "Candidate recording added probes or retained duplicate paths");
                Require(Number(Metrics(snapshot), "captures") == 0 && Number(Metrics(snapshot), "activeReadHandles") == 0,
                    "Candidate observations unexpectedly read or pinned payloads");
                snapshot.VerifyUnchanged();
                Require(Number(CandidateMetrics(snapshot), "verificationChecks") == 2 && Number(before, "verificationChecks") == 0,
                    "Verification omitted a negative/positive candidate or mutated an earlier report");
                File.WriteAllText(first, "higher-priority");
                Changed(snapshot, "New higher-priority candidate did not refuse publication");
                Require(FileNameResolver.ResolvePath(root, "data:" + name) == first, "Guard replaced the resolver's next actual answer");
                File.Delete(first); // End state alone must not erase an already observed conflict.
                Changed(snapshot, "A within-generation resolver conflict was forgotten after restoring the filesystem");
                Require(Number(CandidateMetrics(snapshot), "conflictingObservations") == 1,
                    "Changed repeated existence result was not retained");
                observations.Add(new { id = "resolver-candidate-conflict", metrics = Metrics(snapshot) });
            }
            using (var snapshot = new PhysicalInputSnapshot(Path.Combine(root, "removed")))
            using (snapshot.BeginReadScope())
            {
                Require(FileNameResolver.ResolvePath(root, "audio:" + name) == second, "Audio search changed");
                File.Delete(second); Changed(snapshot, "Removed positive candidate did not refuse publication");
            }
            File.WriteAllText(second, "restored");
            using (var snapshot = new PhysicalInputSnapshot(Path.Combine(root, "new-generation")))
            using (snapshot.BeginReadScope())
            {
                Require(FileNameResolver.ResolvePath(root, "data:" + name) == second, "New generation retained old resolver conflict");
                Settings.Current.DataPaths = new[] { low };
                Require(FileNameResolver.ResolvePath(root, "data:unprobed.bin") == Path.Combine(low, "unprobed.bin"), "Single-root unprobed path changed");
                Require(Number(CandidateMetrics(snapshot), "observations") == 2, "A single-root path added an existence check absent from legacy");
                snapshot.VerifyUnchanged();
            }
            Settings.Current.DataPaths = new[] { high, low };
            foreach (bool countBudget in new[] { true, false })
            {
                using (var snapshot = new PhysicalInputSnapshot(Path.Combine(root, countBudget ? "count-limit" : "text-limit"), 128, 0, 2, 64,
                    countBudget ? 0 : 8192, countBudget ? 1048576 : 0))
                using (snapshot.BeginReadScope())
                {
                    Require(FileNameResolver.ResolvePath(root, "data:" + name) == second, "Candidate budget changed legacy resolution");
                    var metrics = CandidateMetrics(snapshot);
                    Require(Number(metrics, "uniquePaths") == 0 && Number(metrics, "dropped") == 2 && Convert.ToBoolean(metrics["incomplete"]),
                        "Candidate budget did not report incomplete coverage");
                    snapshot.VerifyUnchanged();
                }
            }
            using (var snapshot = new PhysicalInputSnapshot(Path.Combine(root, "excluded")))
            {
                snapshot.ExcludeRoot(high);
                using (snapshot.BeginReadScope())
                {
                    Require(FileNameResolver.ResolvePath(root, "data:" + name) == second, "Exclusion changed resolution");
                    Require(Number(CandidateMetrics(snapshot), "dropped") == 1 && Number(CandidateMetrics(snapshot), "uniquePaths") == 1,
                        "Generated-root candidate was retained as immutable input");
                    snapshot.VerifyUnchanged();
                }
            }
        }
        finally { Settings.Current = previous; }
        tests.Add("actual-resolver-positive-negative-candidate-publication-check-priority-addition-removal-and-transient-conflict-without-query-reuse");
        tests.Add("resolver-single-root-no-extra-probe-bounded-count-text-excluded-roots-and-independent-next-generation");
    }
    public static int Main(string[] args)
    {
        int code = 0; object result;
        try
        {
            Require(args.Length == 5 && IntPtr.Size == 4 && Path.DirectorySeparatorChar == '\\', "Expected Windows x86 work result source run SDK");
            string work = Path.GetFullPath(args[0]); Directory.CreateDirectory(work);
            DigestCases(work); ReaderCases(work); NestedReaderCloseCases(work); AcquiredReaderFailureCases(work); CoreXmlReaderFailureCases(work); CancellationCases(work); VerificationCancellationCases(work); AliasCases(work); BudgetCases(work); CandidateCases(work);
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), observations = observations.ToArray(),
                scope = "Positive physical content snapshot, exact legacy digests, XML/XInclude/schema readers, finite budgets, Windows pin lifecycle and executed resolver existence consistency at verification; hidden plugin reads and complete query closure remain separate" };
            Console.WriteLine("PASS PhysicalInputs: " + tests.Count + " groups; " + checks + " checks");
        }
        catch (Exception error)
        {
            code = 1; Console.Error.WriteLine(error); result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null,
                runId = args.Length > 3 ? args[3] : null, checks, tests = tests.ToArray(), observations = observations.ToArray(), error = error.ToString() };
        }
        if (args.Length > 1) File.WriteAllText(args[1], Json.Serialize(result)); return code;
    }
}
