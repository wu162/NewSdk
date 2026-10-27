using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            PhysicalInputSnapshot.Entry first, caseAlias, hardAlias, distinct;
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
    public static int Main(string[] args)
    {
        int code = 0; object result;
        try
        {
            Require(args.Length == 5 && IntPtr.Size == 4 && Path.DirectorySeparatorChar == '\\', "Expected Windows x86 work result source run SDK");
            string work = Path.GetFullPath(args[0]); Directory.CreateDirectory(work);
            DigestCases(work); ReaderCases(work); CancellationCases(work); AliasCases(work); BudgetCases(work);
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), observations = observations.ToArray(),
                scope = "Positive physical content snapshot, exact legacy digests, XML/XInclude/schema readers, finite budgets and Windows pin lifecycle only; negative queries and hidden plugin reads remain separate" };
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
