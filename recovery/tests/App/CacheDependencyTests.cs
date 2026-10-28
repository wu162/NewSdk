using System;
using System.IO;
using System.Text;
using System.Threading;
using NewModCompiler.Application;

namespace NewModCompiler.Tests
{
    public static class CacheDependencyTests
    {
        static int checks;
        static void Check(bool value, string message) { ++checks; if (!value) throw new Exception(message); }
        static void Reject<T>(Action action, string message) where T : Exception
        {
            try { action(); } catch (T) { ++checks; return; }
            throw new Exception(message);
        }
        static string Put(string root, string name, string text)
        {
            string path = Path.Combine(root, name); Directory.CreateDirectory(Path.GetDirectoryName(path)); File.WriteAllText(path, text); return path;
        }
        static CacheDependencyManifest Observe(string input, string missing)
        {
            var result = new CacheDependencyManifest();
            result.Probe(missing, false); result.Probe(input, true);
            using (var stream = result.OpenRead(input))
            using (var reader = new StreamReader(stream)) Check(reader.ReadToEnd() == "AAAA", "Dependency identity must describe the exact bytes supplied to its consumer");
            return result;
        }
        static void RoundTripAndChanges(string root)
        {
            string input = Put(root, "selected.xml", "AAAA"), missing = Path.Combine(root, "preferred.xml");
            var manifest = Observe(input, missing);
            manifest.ReportUnknownRead("native-reader-unproven"); manifest.ReportUnknownRead("native-reader-unproven");
            Check(!manifest.CanAuthorizeReuse && manifest.UnknownReadCount == 1, "Partial observations must retain incomplete-reader evidence without authorizing cache reuse");
            Check(manifest.ProbeCount >= 2 && manifest.FileCount == 1 && manifest.ReadBytes >= 4, "Read closure must retain actual negative decisions and consumed content");
            manifest.ValidateCurrent(CancellationToken.None);
            Check(manifest.ValidationBytes >= 4, "Publication validation must check current consumed bytes");
            string first = Path.Combine(root, "first.bin"), second = Path.Combine(root, "second.bin");
            string firstKey = manifest.Write(first), secondKey = manifest.Write(second);
            Check(firstKey.Length == 64 && firstKey == secondKey, "Unchanged observations must serialize with a deterministic complete content key");
            Check(BitConverter.ToString(File.ReadAllBytes(first)) == BitConverter.ToString(File.ReadAllBytes(second)), "The binary dependency manifest must have deterministic bytes");
            Check(manifest.ManifestBytes == new FileInfo(second).Length && manifest.ManifestBytes > 0, "Manifest accounting must describe its complete persisted length");
            var restored = CacheDependencyManifest.Read(first);
            Check(restored.ProbeCount == manifest.ProbeCount && restored.FileCount == manifest.FileCount && restored.UnknownReadCount == 1 && !restored.CanAuthorizeReuse, "Binary manifest roundtrip lost dependencies");
            restored.ValidateCurrent(CancellationToken.None);
            string restoredPath = Path.Combine(root, "restored.bin");
            Check(restored.Write(restoredPath) == firstKey, "Read then write must preserve the complete binary manifest content identity");
            Put(root, "irrelevant.bin", "unconsumed root member");
            restored.ValidateCurrent(CancellationToken.None);
            // This only proves the manifest's recorded closure. Production
            // still retains whole-root fallback until all readers are certified.
            Put(root, "preferred.xml", "HIGH");
            Reject<IOException>(() => restored.ValidateCurrent(CancellationToken.None), "A previously absent preferred candidate appeared without invalidating its recorded decision");
            File.Delete(missing); restored.ValidateCurrent(CancellationToken.None);
            DateTime timestamp = File.GetLastWriteTimeUtc(input);
            Put(root, "selected.xml", "BBBB"); File.SetLastWriteTimeUtc(input, timestamp);
            Check(new FileInfo(input).Length == 4 && File.GetLastWriteTimeUtc(input) == timestamp, "Closure mutation must preserve exact input length and mtime");
            Reject<IOException>(() => restored.ValidateCurrent(CancellationToken.None), "Same-length/same-mtime consumed bytes escaped dependency validation");
            Put(root, "selected.xml", "AAAA"); File.SetLastWriteTimeUtc(input, timestamp);
            restored.ValidateCurrent(CancellationToken.None);
            File.Delete(input);
            Reject<IOException>(() => restored.ValidateCurrent(CancellationToken.None), "Deleting the consumed file escaped dependency validation");
            Put(root, "selected.xml", "AAAA"); restored.ValidateCurrent(CancellationToken.None);
            using (var cancelled = new CancellationTokenSource())
            {
                cancelled.Cancel();
                Reject<OperationCanceledException>(() => restored.ValidateCurrent(cancelled.Token), "Cancelled publication dependency validation succeeded");
            }
            restored.ValidateCurrent(CancellationToken.None);
            using (var writable = File.Open(input, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) Check(writable.Length == 4, "Closure readers and validation must release their file handles");
            // Contradictory observations within one build are a real race;
            // neither the first nor last observation may silently win.
            var raced = Observe(input, missing);
            Put(root, "selected.xml", "BBBB");
            Reject<IOException>(() => { using (raced.OpenRead(input)) { } }, "A second observed version of one input was accepted within the same stream");
        }
        static void CorruptRecords(string root)
        {
            string input = Put(root, "input.bin", "AAAA"), missing = Path.Combine(root, "missing.bin"), file = Path.Combine(root, "manifest.bin");
            Observe(input, missing).Write(file);
            byte[] good = File.ReadAllBytes(file);
            foreach (int length in new[] { 0, 1, good.Length / 2, good.Length - 1 })
            {
                byte[] shortRecord = new byte[length]; Array.Copy(good, shortRecord, length); File.WriteAllBytes(file, shortRecord);
                Reject<IOException>(() => CacheDependencyManifest.Read(file), "Truncated dependency record was accepted: " + length);
            }
            byte[] bad = (byte[])good.Clone(); bad[0] ^= 0x7f; File.WriteAllBytes(file, bad);
            Reject<IOException>(() => CacheDependencyManifest.Read(file), "Unknown binary manifest magic/version was accepted");
            bad = new byte[good.Length + 1]; Array.Copy(good, bad, good.Length); bad[bad.Length - 1] = 0x7f; File.WriteAllBytes(file, bad);
            Reject<IOException>(() => CacheDependencyManifest.Read(file), "Trailing unvalidated bytes were accepted by the binary manifest reader");
            // Explicit little-endian count and path-length bounds reject allocation
            // requests before parsing a forged or truncated table.
            foreach (var malformed in new[] { new[] { 8, -1 }, new[] { 8, 100001 }, new[] { 12, 100001 }, new[] { 16, int.MaxValue } })
            {
                bad = (byte[])good.Clone(); Array.Copy(BitConverter.GetBytes(malformed[1]), 0, bad, malformed[0], 4); File.WriteAllBytes(file, bad);
                Reject<IOException>(() => CacheDependencyManifest.Read(file), "Out-of-range dependency table count or text length was accepted");
            }
            File.WriteAllBytes(file, good); CacheDependencyManifest.Read(file).ValidateCurrent(CancellationToken.None);
            var bounded = new CacheDependencyManifest();
            Reject<IOException>(() => bounded.ReportUnknownRead(new string('x', 131069)), "Oversized unknown-read metadata escaped the manifest decode budget");
            Reject<IOException>(() => bounded.Probe("relative-path.xml", false), "Relative dependency paths were accepted");
            bounded.Probe(missing, false);
            Reject<IOException>(() => bounded.Probe(missing, true), "Contradictory candidate decisions within one stream were accepted");
        }
        public static int Run(string root)
        {
            checks = 0; Directory.CreateDirectory(root);
            RoundTripAndChanges(Path.Combine(root, "roundtrip"));
            CorruptRecords(Path.Combine(root, "malformed"));
            return checks;
        }
    }
}
