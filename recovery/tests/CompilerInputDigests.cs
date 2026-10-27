using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using EALA.Hash;

public static class CompilerInputDigests
{
    static int checks;
    static readonly List<object> measurements = new List<object>();
    static void Require(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    static uint Legacy(string path)
    {
        var input = new AsynchronousFileReader(path);
        uint result = input.FileSize;
        while (input.BeginRead())
        {
            result = FastHash.GetHashCode(result, input.CurrentChunk.Data, input.CurrentChunk.BytesRead);
            input.EndRead();
        }
        return result;
    }
    static string Strong(string path)
    {
        using (var algorithm = SHA256.Create()) using (var input = File.OpenRead(path))
            return Convert.ToBase64String(algorithm.ComputeHash(input));
    }
    static uint HistoricalUpdate(string path, out string strong)
    {
        var input = new AsynchronousFileReader(path);
        uint result = input.FileSize;
        using (var algorithm = SHA256.Create())
        {
            while (input.BeginRead())
            {
                byte[] data = input.CurrentChunk.Data; int count = input.CurrentChunk.BytesRead;
                result = FastHash.GetHashCode(result, data, count);
                algorithm.TransformBlock(data, 0, count, data, 0);
                input.EndRead();
            }
            algorithm.TransformFinalBlock(new byte[0], 0, 0);
            strong = Convert.ToBase64String(algorithm.Hash);
        }
        return result;
    }
    static FileHashItem Roundtrip(FileHashItem item, out string strong)
    {
        var text = new StringBuilder();
        using (var writer = XmlWriter.Create(text, new XmlWriterSettings { OmitXmlDeclaration = true })) item.WriteXml(writer);
        var xml = new XmlDocument(); xml.LoadXml(text.ToString());
        strong = xml.DocumentElement.GetAttribute("sha256");
        var restored = new FileHashItem();
        using (var reader = XmlReader.Create(new StringReader(text.ToString()))) { reader.MoveToContent(); restored.ReadXml(reader); }
        return restored;
    }
    static void ReaderOwnership(string work)
    {
        string path = Path.Combine(work, "reader-ownership.bin");
        File.WriteAllBytes(path, new byte[1048607]);
        foreach (string phase in new[] { "pending", "active", "requeued", "eof", "consumer-fault" })
        {
            Chunk held = null, eof = null;
            using (var reader = new AsynchronousFileReader(path))
            {
                Require(reader.FileSize == 1048607 && reader.CurrentChunk == null, "Reader construction changed its public initial state");
                if (phase == "active" || phase == "requeued" || phase == "consumer-fault")
                {
                    Require(reader.BeginRead(), "Reader did not return its first complete chunk");
                    held = reader.CurrentChunk;
                    Require(held.BytesRead == 1048576 && !reader.BeginRead() && Object.ReferenceEquals(held, reader.CurrentChunk),
                        "Reader changed one-MiB chunks or repeated BeginRead while a chunk is owned");
                    using (var another = new Chunk()) Require(!Object.ReferenceEquals(held.Data, another.Data), "An active reader buffer was returned to the global pool");
                    if (phase == "requeued")
                    {
                        reader.EndRead();
                        Require(reader.CurrentChunk == null && held.BytesRead == 0, "EndRead did not transfer the chunk to its pending job");
                    }
                    if (phase == "consumer-fault")
                    {
                        held.Dispose(); bool rejected = false;
                        try { reader.EndRead(); } catch (NullReferenceException) { rejected = true; }
                        Require(rejected, "Disposing the public chunk before EndRead changed its original primary failure");
                    }
                }
                else if (phase == "eof")
                {
                    long bytes = 0;
                    while (reader.BeginRead()) { bytes += reader.CurrentChunk.BytesRead; reader.EndRead(); }
                    eof = reader.CurrentChunk;
                    Require(bytes == 1048607 && eof != null && eof.Data == null && eof.BytesRead == 0 && reader.FileSize == 0,
                        "Natural EOF changed its disposed CurrentChunk or closed FileSize contract");
                }
                reader.Dispose(); reader.Dispose();
                Require(reader.FileSize == 0 && !reader.BeginRead(), "Disposed reader retained a source or accepted a new read");
                reader.EndRead();
                if (held != null) Require(held.Data == null, "An early exit retained its pending/active buffer");
                if (eof != null) Require(Object.ReferenceEquals(eof, reader.CurrentChunk), "EOF disposal changed the public CurrentChunk object");
                using (File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
            }
            // A duplicate pool return would hand the same array to simultaneous
            // consumers. No GC/finalizer is needed to unlock the source above.
            using (var first = new Chunk()) using (var second = new Chunk())
                Require(!Object.ReferenceEquals(first.Data, second.Data), "Reader cleanup returned its buffer to the pool twice");
        }
        measurements.Add(new { scenario = "async-reader-owned-lifecycle", phases = 5,
            sourceImmediatelyExclusive = true, eofObjectPreserved = true,
            scope = "Public reader/Chunk lifecycle only; Dispose waits for pending APM completion, not cancellation of OS I/O" });
    }
    static readonly IOException DigestFault = new IOException("Injected actual SHA256 HashCore failure");
    public sealed class FailingSha256 : SHA256
    {
        public FailingSha256() { HashSizeValue = 256; }
        public override void Initialize() { }
        protected override void HashCore(byte[] data, int start, int size) { throw DigestFault; }
        protected override byte[] HashFinal() { throw DigestFault; }
    }
    static void FailedDigestOwnership(string work)
    {
        string path = Path.Combine(work, "failed-digest-ownership.bin");
        File.WriteAllBytes(path, new byte[1048607]);
        uint expected = Legacy(path); Type originalProvider;
        using (var original = SHA256.Create()) originalProvider = original.GetType();
        var item = new FileHashItem(path, "failed-digest", TargetPlatform.Win32);
        bool rejected = false;
        try
        {
            // Only this isolated test process changes CryptoConfig. Production
            // calls the real SHA256 factory and consumes the real reader chunk.
            CryptoConfig.AddAlgorithm(typeof(FailingSha256), "System.Security.Cryptography.SHA256");
            try { bool dirty = item.IsDirty; } catch (IOException error) { rejected = Object.ReferenceEquals(error, DigestFault); }
        }
        finally { CryptoConfig.AddAlgorithm(originalProvider, "System.Security.Cryptography.SHA256"); }
        Require(rejected, "Digest cleanup replaced or swallowed the actual consumer's primary exception");
        using (File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
        Require(item.DigestReadCount == 0 && item.DigestBytesRead == 0, "A failed digest published successful read accounting");
        Require(item.IsDirty && item.Hash == expected && item.DigestReadCount == 1 && item.DigestBytesRead == 1048607,
            "The same FileHashItem could not retry after its failed digest");
        string strong; Roundtrip(item, out strong);
        Require(strong == Strong(path), "Retried digest committed a partial strong identity");
        measurements.Add(new { scenario = "actual-file-hash-SHA256-consumer-failure", primaryExceptionPreserved = true,
            sourceImmediatelyExclusive = true, sameItemRetry = true, cryptoProviderRestored = originalProvider.FullName,
            scope = "Actual SHA256.Create/HashCore fault in isolated fixture process; no fabricated digest or native I/O error" });
    }
    static void Cases(string work)
    {
        string path = Path.Combine(work, "digest.bin");
        foreach (int length in new[] { 0, 1, 1048575, 1048576, 1048607 })
        {
            byte[] data = new byte[length];
            for (int index = 0; index < length; index++) data[index] = (byte)((index * 19 + 7) % 251);
            File.WriteAllBytes(path, data);
            var item = new FileHashItem(path, "digest", TargetPlatform.Win32);
            Require(item.IsDirty, "New input must require a digest");
            Require(item.Hash == Legacy(path), "Single-pass legacy digest differs at length " + length);
            Require(item.DigestReadCount == 1 && item.DigestBytesRead == length, "Dirty SHA256 and FastHash must share one logical read");
            string strong;
            var loaded = Roundtrip(item, out strong);
            Require(strong == Strong(path), "Strong digest must describe the same bytes as legacy digest");
            Require(!loaded.IsDirty && loaded.Hash == item.Hash && loaded.DigestReadCount == 1, "Persisted baseline must revalidate content once");
            Require(!item.IsDirty && item.Hash == loaded.Hash && item.DigestReadCount == 1, "Repeated accesses in one observation must not reopen the file");
            item.Reset();
            Require(!item.IsDirty && item.Hash == loaded.Hash && item.DigestReadCount == 2, "Reset must always perform fresh content verification");
            if (length > 0)
            {
                DateTime timestamp = File.GetLastWriteTimeUtc(path);
                data[length - 1] ^= 93;
                File.WriteAllBytes(path, data); File.SetLastWriteTimeUtc(path, timestamp);
                Require(File.GetLastWriteTimeUtc(path) == timestamp && new FileInfo(path).Length == length, "Mutation fixture must preserve time and length");
                item.Reset();
                Require(item.IsDirty && item.Hash == Legacy(path), "Equal-time equal-size mutation must invalidate the observation");
                Require(item.DigestReadCount == 4 && item.DigestBytesRead == 4L * length, "Equal-time mutation must retain the conservative warm check plus full rehash");
                Roundtrip(item, out strong);
                Require(strong == Strong(path), "Updated strong identity must be committed with the current legacy identity");
                File.Delete(path); item.Reset();
                Require(item.IsDirty && !item.Exists && item.Hash == 0 && item.DigestReadCount == 4, "Missing files must not open content or reuse the old legacy hash");
                File.WriteAllBytes(path, data); File.SetLastWriteTimeUtc(path, timestamp); item.Reset();
                Require(item.IsDirty && item.Hash == Legacy(path) && item.DigestReadCount == 5, "Same-time recreation must establish a fresh nonzero identity");
                data[0] ^= 11; File.WriteAllBytes(path, data); File.SetLastWriteTimeUtc(path, timestamp.AddSeconds(2));
                Require(File.GetLastWriteTimeUtc(path) != timestamp, "Changed-date fixture must change its timestamp");
                item.Reset();
                Require(item.IsDirty && item.Hash == Legacy(path) && item.DigestReadCount == 6, "Timestamp-changed content must compute both identities in one read");
            }
        }
        AppContext.SetSwitch("NewSdk.Compiler.DisableSinglePassDigest", true);
        try
        {
            var fallback = new FileHashItem(path, "digest", TargetPlatform.Win32);
            long length = new FileInfo(path).Length;
            Require(fallback.Hash == Legacy(path) && fallback.DigestReadCount == 2 && fallback.DigestBytesRead == 2 * length,
                "Rollback must preserve the historical two-read path and legacy digest");
            string sha; Roundtrip(fallback, out sha);
            Require(sha == Strong(path), "Rollback must preserve the strong identity");
            fallback.Reset();
            Require(!fallback.IsDirty && fallback.Hash == Legacy(path) && fallback.DigestReadCount == 3,
                "Rollback warm check must remain SHA-only");
        }
        finally { AppContext.SetSwitch("NewSdk.Compiler.DisableSinglePassDigest", false); }
        var cache = new SessionCache(); cache.InitializeCache(new List<string>());
        FileHashItem cached;
        Require(cache.TryGetFile(path, "one", TargetPlatform.Win32, out cached), "Digest counter fixture must find its input");
        uint hash = cached.Hash;
        var snapshot = cache.GetDigestReadMetrics();
        Require(snapshot.reads == 1 && snapshot.bytes == new FileInfo(path).Length && snapshot.items == 1, "Session report must include the observed digest work");
        cached.Reset(); Require(cached.Hash == hash, "Session digest reset changed stable legacy bytes");
        Require(cache.GetDigestReadMetrics().reads == 2 && snapshot.reads == 1, "Session digest snapshots must be detached from subsequent work");
    }
    static void Benchmark(string work)
    {
        string path = Path.Combine(work, "sample.bin");
        byte[] data = new byte[4194335];
        for (int i = 0; i < data.Length; i++) data[i] = (byte)(i * 31 + i / 257);
        File.WriteAllBytes(path, data);
        uint expected = Legacy(path); string expectedStrong = Strong(path);
        var oldCold = new List<double>(); var newCold = new List<double>();
        var oldWarm = new List<double>(); var newWarm = new List<double>();
        // Alternating order, same process/machine/input. OS page cache is unknown.
        for (int sample = 0; sample < 5; sample++)
        {
            Action baseline = () => {
                var timer = Stopwatch.StartNew();
                string sha = Strong(path), secondSha; uint legacy = HistoricalUpdate(path, out secondSha); timer.Stop();
                oldCold.Add(timer.Elapsed.TotalMilliseconds);
                Require(sha == expectedStrong && secondSha == expectedStrong && legacy == expected, "Historical two-pass baseline changed");
                timer.Restart(); sha = Strong(path); timer.Stop(); oldWarm.Add(timer.Elapsed.TotalMilliseconds);
                Require(sha == expectedStrong, "Historical warm SHA-only check changed");
            };
            Action candidate = () => {
                var item = new FileHashItem(path, "digest", TargetPlatform.Win32);
                var timer = Stopwatch.StartNew(); uint legacy = item.Hash; timer.Stop(); newCold.Add(timer.Elapsed.TotalMilliseconds);
                Require(legacy == expected && item.DigestReadCount == 1 && item.DigestBytesRead == data.Length, "Candidate must read each input byte once");
                item.Reset(); timer.Restart(); bool dirty = item.IsDirty; timer.Stop(); newWarm.Add(timer.Elapsed.TotalMilliseconds);
                Require(!dirty && item.Hash == expected, "Warm candidate must keep its valid baseline");
            };
            if (sample % 2 == 0) { baseline(); candidate(); } else { candidate(); baseline(); }
        }
        measurements.Add(new { scenario = "one-file-observation", samples = 5, inputBytes = data.Length,
            historicalColdLogicalBytes = 2L * data.Length, candidateColdLogicalBytes = data.Length,
            historicalWarmLogicalBytes = data.Length, candidateWarmLogicalBytes = data.Length,
            oldColdMs = oldCold, newColdMs = newCold, oldWarmMs = oldWarm, newWarmMs = newWarm,
            osPageCache = "unknown", scope = "isolated digest microbenchmark; not end-to-end build speed" });
    }
    public static int Main(string[] args)
    {
        try
        {
            Directory.CreateDirectory(args[0]); Cases(args[0]); ReaderOwnership(args[0]); FailedDigestOwnership(args[0]); Benchmark(args[0]);
            var result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks,
                tests = new[] { "single-pass-legacy-and-SHA256-boundaries-roundtrip-reset-content-invalidation-delete-recreate", "public-async-reader-pending-active-requeued-EOF-consumer-failure-dispose-and-no-double-pool-return", "actual-FileHashItem-SHA256-consumer-fault-immediate-source-release-primary-error-and-same-item-retry", "interleaved-cold-and-warm-digest-cost" }, measurements };
            string json = new JavaScriptSerializer().Serialize(result);
            File.WriteAllText(args[1], json); Console.WriteLine(json);
            Console.WriteLine("PASS InputDigests: " + checks + " assertions"); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
