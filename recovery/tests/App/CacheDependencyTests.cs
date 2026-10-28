using System;
using System.IO;
using System.Security.Cryptography;
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
        static void RejectInvalidData(Action action, string message)
        {
            try { action(); }
            catch (InvalidDataException) { ++checks; return; }
            catch (EndOfStreamException) { ++checks; return; }
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
        static void RepairChecksum(byte[] bytes)
        {
            using (var hash = SHA256.Create())
                Array.Copy(hash.ComputeHash(bytes, 0, bytes.Length - 32), 0, bytes, bytes.Length - 32, 32);
        }
        static void SignedManifest(string file, Action<BinaryWriter> body)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, true)) body(writer);
                byte[] payload = stream.ToArray(), checksum;
                using (var hash = SHA256.Create()) checksum = hash.ComputeHash(payload);
                using (var output = File.Create(file)) { output.Write(payload, 0, payload.Length); output.Write(checksum, 0, checksum.Length); }
            }
        }
        static void Text(BinaryWriter writer, string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text); writer.Write(bytes.Length); writer.Write(bytes);
        }
        static void EmptyCancellationAndInvalidText(string root)
        {
            Directory.CreateDirectory(root);
            var empty = new CacheDependencyManifest();
            using (var cancelled = new CancellationTokenSource())
            {
                cancelled.Cancel();
                Reject<OperationCanceledException>(() => empty.ValidateCurrent(cancelled.Token), "Empty read closures must still observe cancellation at publication validation");
                var preCancelled = new CacheDependencyManifest(cancelled.Token);
                Reject<OperationCanceledException>(() => preCancelled.ValidateCurrent(CancellationToken.None), "An empty observation must honor its constructor cancellation token");
                Reject<OperationCanceledException>(() => preCancelled.Probe(Path.Combine(root, "missing"), false), "Cancelled observation accepted a resolver decision");
                Reject<OperationCanceledException>(() => preCancelled.Write(Path.Combine(root, "cancelled.bin")), "Cancelled observation created a persisted record");
                Check(!File.Exists(Path.Combine(root, "cancelled.bin")), "Cancelled persistence must not create a partial manifest");
            }
            string file = Path.Combine(root, "invalid-text.bin");
            SignedManifest(file, writer => { writer.Write(0x31434452); writer.Write(1); writer.Write(0); writer.Write(1); writer.Write(2); writer.Write((byte)0xc0); writer.Write((byte)0xaf); });
            RejectInvalidData(() => CacheDependencyManifest.Read(file), "Malformed UTF-8 must reject as invalid cache data rather than leak a decoder implementation exception");
            SignedManifest(file, writer => { writer.Write(0x31434452); writer.Write(1); writer.Write(1); writer.Write(0); Text(writer, Path.Combine(root, "nul") + "\0name"); writer.Write((byte)0); writer.Write(-1L); });
            RejectInvalidData(() => CacheDependencyManifest.Read(file), "An illegal physical path in a checksum-valid table must reject as invalid cache data");
            empty.ValidateCurrent(CancellationToken.None);
            string first = Path.Combine(root, "empty.bin"), second = Path.Combine(root, "empty-restored.bin");
            string key = empty.Write(first);
            Check(CacheDependencyManifest.Read(first).Write(second) == key, "The minimum empty manifest must have a deterministic bounded roundtrip");
        }
        static void CorruptRecords(string root)
        {
            string input = Put(root, "input.bin", "AAAA"), missing = Path.Combine(root, "missing.bin"), file = Path.Combine(root, "manifest.bin");
            Observe(input, missing).Write(file);
            byte[] good = File.ReadAllBytes(file);
            foreach (int length in new[] { 0, 1, good.Length / 2, good.Length - 1 })
            {
                byte[] shortRecord = new byte[length]; Array.Copy(good, shortRecord, length); File.WriteAllBytes(file, shortRecord);
                RejectInvalidData(() => CacheDependencyManifest.Read(file), "Truncated dependency record was accepted: " + length);
            }
            byte[] bad = (byte[])good.Clone(); bad[0] ^= 0x7f; File.WriteAllBytes(file, bad);
            RejectInvalidData(() => CacheDependencyManifest.Read(file), "Unknown binary manifest magic/version was accepted");
            bad = new byte[good.Length + 1]; Array.Copy(good, bad, good.Length); bad[bad.Length - 1] = 0x7f; File.WriteAllBytes(file, bad);
            RejectInvalidData(() => CacheDependencyManifest.Read(file), "Trailing unvalidated bytes were accepted by the binary manifest reader");
            // Explicit little-endian count and path-length bounds reject allocation
            // requests before parsing a forged or truncated table.
            foreach (var malformed in new[] { new[] { 4, 2 }, new[] { 8, -1 }, new[] { 8, 100001 }, new[] { 12, 100001 }, new[] { 16, int.MaxValue } })
            {
                bad = (byte[])good.Clone(); Array.Copy(BitConverter.GetBytes(malformed[1]), 0, bad, malformed[0], 4); RepairChecksum(bad); File.WriteAllBytes(file, bad);
                RejectInvalidData(() => CacheDependencyManifest.Read(file), "Out-of-range dependency table count or text length was accepted");
            }
            // Re-sign malformed records so structural checks, rather than the
            // outer checksum alone, must reject the invalid payload.
            foreach (int corruption in new[] { 0, 1, 2, 3, 4, 5 })
            {
                SignedManifest(file, writer =>
                {
                    writer.Write(0x31434452); writer.Write(1); writer.Write(corruption == 0 ? 2 : corruption == 4 ? 0 : 1); writer.Write(corruption == 4 ? 2 : 0);
                    if (corruption == 4) { Text(writer, "duplicate"); Text(writer, "duplicate"); return; }
                    Text(writer, corruption == 3 ? "relative.xml" : missing);
                    writer.Write((byte)(corruption == 1 ? 2 : 0)); writer.Write(corruption == 2 ? -2L : corruption == 5 ? 4L : -1L);
                    if (corruption == 0) { Text(writer, missing); writer.Write((byte)0); writer.Write(-1L); }
                    if (corruption == 5) writer.Write(new byte[32]);
                });
                RejectInvalidData(() => CacheDependencyManifest.Read(file), "Checksum-valid malformed path/candidate/content-length/duplicate table was accepted: " + corruption);
            }
            SignedManifest(file, writer => { writer.Write(0x31434452); writer.Write(1); writer.Write(1); writer.Write(0); writer.Write(131069); writer.Write(new byte[131069]); });
            RejectInvalidData(() => CacheDependencyManifest.Read(file), "Checksum-valid oversized path escaped the per-record decode budget");
            File.WriteAllBytes(file, good); CacheDependencyManifest.Read(file).ValidateCurrent(CancellationToken.None);
            var bounded = new CacheDependencyManifest();
            RejectInvalidData(() => bounded.ReportUnknownRead(new string('x', 131069)), "Oversized unknown-read metadata escaped the manifest decode budget");
            RejectInvalidData(() => bounded.Probe("relative-path.xml", false), "Relative dependency paths were accepted");
            bounded.Probe(missing, false);
            Reject<IOException>(() => bounded.Probe(missing, true), "Contradictory candidate decisions within one stream were accepted");
        }
        static void LeasedReadIdentities(string root)
        {
            string input = Put(root, "leased.bin", "AAAA");
            byte[] digest;
            using (var file = File.OpenRead(input))
            using (var hash = SHA256.Create()) digest = hash.ComputeHash(file);
            var manifest = new CacheDependencyManifest();
            manifest.RecordLeasedRead(input, 4, digest);
            Check(manifest.FileCount == 1 && manifest.ReadBytes == 0 && manifest.LeasedReadBytes == 4, "An identity from an existing read lease must not invent another file read");
            byte[] retained = (byte[])digest.Clone(); digest[0] ^= 0x7f;
            manifest.ValidateCurrent(CancellationToken.None);
            manifest.RecordLeasedRead(input, 4, retained);
            Check(manifest.FileCount == 1 && manifest.LeasedReadBytes == 8 && manifest.ReadBytes == 0, "Repeated consistent lease identities must preserve one dependency and account separately");
            Reject<IOException>(() => manifest.RecordLeasedRead(input, 4, digest), "Contradictory digests from one stream's leased input were accepted");
            Reject<IOException>(() => manifest.RecordLeasedRead(input, 5, retained), "Contradictory leased lengths were accepted");
            foreach (byte[] invalid in new[] { null, new byte[0], new byte[31], new byte[33] })
                RejectInvalidData(() => manifest.RecordLeasedRead(input, 4, invalid), "A leased input without exactly one SHA256 identity was accepted");
            RejectInvalidData(() => manifest.RecordLeasedRead(input, -1, retained), "A negative leased input length was accepted");
            using (var stream = manifest.OpenRead(input)) Check(stream.ReadByte() == (byte)'A', "Ordinary observed reads must agree with an existing leased identity");
            Check(manifest.ReadBytes == 4 && manifest.LeasedReadBytes == 8 && manifest.FileCount == 1, "Leased and observed read byte counters must remain distinct");
            string record = Path.Combine(root, "leased-manifest.bin"); manifest.Write(record);
            var restored = CacheDependencyManifest.Read(record); restored.ValidateCurrent(CancellationToken.None);
            Check(restored.FileCount == 1 && !restored.CanAuthorizeReuse, "Persisted leased identity must not acquire reuse authority");
            Put(root, "leased.bin", "BBBB");
            Reject<IOException>(() => restored.ValidateCurrent(CancellationToken.None), "Publication validation failed to check the current content of a previously leased input");
            var absent = new CacheDependencyManifest(); absent.Probe(Path.Combine(root, "absent.bin"), false);
            Reject<IOException>(() => absent.RecordLeasedRead(Path.Combine(root, "absent.bin"), 4, retained), "A leased read silently contradicted an earlier negative candidate decision");
        }
        static void MetadataBudgetIsTransactional()
        {
            var bounded = new CacheDependencyManifest();
            string prefix = new string('x', 131000), refused = null;
            int accepted = 0;
            // Reach the 16 MiB metadata cap with fewer than 130 records. This
            // exercises bounded rejection without a large MOD or file payload.
            for (int index = 0; index < 130; index++)
            {
                string reason = prefix + index.ToString("D4", System.Globalization.CultureInfo.InvariantCulture);
                try { bounded.ReportUnknownRead(reason); accepted++; }
                catch (InvalidDataException) { refused = reason; break; }
            }
            Check(refused != null && accepted > 0 && bounded.UnknownReadCount == accepted, "The cumulative metadata budget did not bound accepted unknown-reader records");
            RejectInvalidData(() => bounded.ReportUnknownRead(refused), "A rejected budget record polluted deduplication and silently disappeared on retry");
            Check(bounded.UnknownReadCount == accepted && !bounded.CanAuthorizeReuse, "Budget rejection changed the accepted evidence set or reuse authority");
        }
        public static int Run(string root)
        {
            checks = 0; Directory.CreateDirectory(root);
            RoundTripAndChanges(Path.Combine(root, "roundtrip"));
            CorruptRecords(Path.Combine(root, "malformed"));
            EmptyCancellationAndInvalidText(Path.Combine(root, "empty-and-invalid-text"));
            MetadataBudgetIsTransactional();
            LeasedReadIdentities(Path.Combine(root, "leased-identities"));
            return checks;
        }
    }
}
