using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace NewModCompiler.Application
{
    // Observed evidence, never a cache authorization by itself. Unknown native
    // reads and directory decisions still require the conservative inventory.
    public sealed class CacheDependencyManifest
    {
        public const string Format = "NewModCompiler.ReadClosure.v1";
        const int Magic = 0x31434452, MaximumRecords = 100000, MaximumPathBytes = 131068;
        const long MaximumManifestBytes = 16777216;
        static readonly Encoding Utf8 = new UTF8Encoding(false, true);
        sealed class Record { public string Path; public bool Exists; public long Length = -1; public byte[] Digest; }
        readonly List<Record> records = new List<Record>();
        readonly Dictionary<string, Record> paths = new Dictionary<string, Record>(StringComparer.OrdinalIgnoreCase);
        readonly List<string> unknown = new List<string>();
        readonly HashSet<string> unknownSet = new HashSet<string>(StringComparer.Ordinal);
        readonly CancellationToken cancellation;
        long pathBytes;
        public CacheDependencyManifest(CancellationToken cancellation = default(CancellationToken)) { this.cancellation = cancellation; }
        public int ProbeCount { get { return records.Count; } }
        public int FileCount { get; private set; }
        public int UnknownReadCount { get { return unknown.Count; } }
        public long ReadBytes { get; private set; }
        public long LeasedReadBytes { get; private set; }
        public long ValidationBytes { get; private set; }
        public long ManifestBytes { get; private set; }
        public long ReadTicks { get; private set; }
        public long ValidationTicks { get; private set; }
        public long SerializationTicks { get; private set; }
        public bool CanAuthorizeReuse { get { return false; } }
        public void ReportUnknownRead(string reason)
        {
            cancellation.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(reason)) reason = "unspecified";
            if (Utf8.GetByteCount(reason) > MaximumPathBytes) throw new InvalidDataException("Read-closure reason is too long.");
            if (!unknownSet.Contains(reason)) { Account(reason); unknownSet.Add(reason); unknown.Add(reason); }
        }
        public void Probe(string path, bool exists)
        {
            cancellation.ThrowIfCancellationRequested();
            string full = PhysicalPath(path);
            Record previous;
            if (paths.TryGetValue(full, out previous))
            {
                if (previous.Exists != exists) throw new IOException("Read-closure candidate changed during compilation: " + full);
                return;
            }
            Account(full);
            var record = new Record { Path = full, Exists = exists };
            records.Add(record); paths.Add(full, record);
        }
        void Account(string value)
        {
            int bytes = Utf8.GetByteCount(value);
            if (bytes == 0 || bytes > MaximumPathBytes || records.Count + unknown.Count >= MaximumRecords ||
                checked(pathBytes + bytes + 64L * (records.Count + unknown.Count + 1)) > MaximumManifestBytes)
                throw new InvalidDataException("Read-closure metadata budget exceeded.");
            pathBytes = checked(pathBytes + bytes);
        }
        // Called only by the audited owner while the original FileShare.Read
        // handle remains leased through native consumption. The digest belongs
        // to that handle; this API never authorizes a later unleased path read.
        public void RecordLeasedRead(string path, long length, byte[] digest)
        {
            cancellation.ThrowIfCancellationRequested();
            if (length < 0 || digest == null || digest.Length != 32)
                throw new InvalidDataException("Invalid leased read identity.");
            string full = PhysicalPath(path);
            Probe(full, true);
            Record record = paths[full];
            if (record.Digest != null && (record.Length != length || !Same(record.Digest, digest)))
                throw new IOException("Read-closure content changed between reads: " + full);
            if (record.Digest == null) { record.Digest = (byte[])digest.Clone(); record.Length = length; FileCount++; }
            LeasedReadBytes = checked(LeasedReadBytes + length);
        }
        public Stream OpenRead(string path)
        {
            long started = Stopwatch.GetTimestamp();
            try { return OpenReadCore(path); }
            finally { ReadTicks += Stopwatch.GetTimestamp() - started; }
        }
        Stream OpenReadCore(string path)
        {
            cancellation.ThrowIfCancellationRequested();
            string full = PhysicalPath(path);
            RejectReparse(full);
            FileStream input = new FileStream(full, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                // Hash and consumer use the same open read lease, not a later
                // path reopen or a timestamp/size identity. No payload copy.
                byte[] digest = Digest(input, cancellation);
                long length = input.Length;
                Probe(full, true);
                Record record = paths[full];
                if (record.Digest != null && (record.Length != length || !Same(record.Digest, digest)))
                    throw new IOException("Read-closure content changed between reads: " + full);
                if (record.Digest == null) { record.Digest = digest; record.Length = length; FileCount++; }
                ReadBytes = checked(ReadBytes + length);
                input.Position = 0;
                return input;
            }
            catch { input.Dispose(); throw; }
        }
        public void ValidateCurrent(CancellationToken token)
        {
            long started = Stopwatch.GetTimestamp();
            try { ValidateCurrentCore(token); }
            finally { ValidationTicks += Stopwatch.GetTimestamp() - started; }
        }
        void ValidateCurrentCore(CancellationToken token)
        {
            cancellation.ThrowIfCancellationRequested(); token.ThrowIfCancellationRequested();
            foreach (Record record in records)
            {
                cancellation.ThrowIfCancellationRequested(); token.ThrowIfCancellationRequested();
                RejectReparse(record.Path);
                if (File.Exists(record.Path) != record.Exists) throw new IOException("Read-closure candidate changed: " + record.Path);
                if (record.Digest == null) continue;
                using (var input = new FileStream(record.Path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] digest = Digest(input, token);
                    ValidationBytes = checked(ValidationBytes + input.Length);
                    if (input.Length != record.Length || !Same(digest, record.Digest))
                        throw new IOException("Read-closure input changed: " + record.Path);
                }
            }
        }
        public string Write(string path)
        {
            long started = Stopwatch.GetTimestamp();
            try { return WriteCore(path); }
            finally { SerializationTicks += Stopwatch.GetTimestamp() - started; }
        }
        string WriteCore(string path)
        {
            cancellation.ThrowIfCancellationRequested();
            using (var output = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
            {
                using (var writer = new BinaryWriter(output, Utf8, true))
                {
                    writer.Write(Magic); writer.Write(1); // little endian, explicit version
                    writer.Write(records.Count); writer.Write(unknown.Count);
                    foreach (Record record in records)
                    {
                        Text(writer, record.Path); writer.Write(record.Exists); writer.Write(record.Length);
                        if (record.Digest != null) writer.Write(record.Digest);
                    }
                    foreach (string reason in unknown) Text(writer, reason);
                    writer.Flush();
                }
                long contentLength = output.Length;
                output.Position = 0; byte[] checksum = Digest(output, cancellation);
                output.Position = contentLength; output.Write(checksum, 0, checksum.Length);
                ManifestBytes = output.Length;
                if (ManifestBytes > MaximumManifestBytes) throw new InvalidDataException("Read-closure manifest budget exceeded.");
                output.Position = 0;
                return BitConverter.ToString(Digest(output, cancellation)).Replace("-", "").ToLowerInvariant();
            }
        }
        public static CacheDependencyManifest Read(string path)
        {
            try { return ReadCore(path); }
            catch (ArgumentException error) { throw new InvalidDataException("Invalid read-closure text or physical path.", error); }
            catch (NotSupportedException error) { throw new InvalidDataException("Unsupported read-closure physical path.", error); }
        }
        static CacheDependencyManifest ReadCore(string path)
        {
            using (var input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new BinaryReader(input, Utf8))
            {
                if (input.Length < 48 || input.Length > MaximumManifestBytes) throw new InvalidDataException("Invalid read-closure manifest length.");
                long contentLength = input.Length - 32;
                byte[] checksum = Digest(input, CancellationToken.None, contentLength);
                if (!Same(checksum, reader.ReadBytes(32))) throw new InvalidDataException("Read-closure checksum mismatch.");
                input.Position = 0;
                if (reader.ReadInt32() != Magic || reader.ReadInt32() != 1) throw new InvalidDataException("Unknown read-closure format.");
                int count = reader.ReadInt32(), reasons = reader.ReadInt32();
                if (count < 0 || reasons < 0 || count > MaximumRecords || reasons > MaximumRecords - count)
                    throw new InvalidDataException("Invalid read-closure record count.");
                var result = new CacheDependencyManifest();
                for (int i = 0; i < count; i++)
                {
                    string full = ReadText(reader, contentLength);
                    if (PhysicalPath(full) != full || result.paths.ContainsKey(full)) throw new InvalidDataException("Invalid read-closure path table.");
                    if (contentLength - input.Position < 9) throw new InvalidDataException("Truncated read-closure record.");
                    byte present = reader.ReadByte(); if (present > 1) throw new InvalidDataException("Invalid read-closure candidate state.");
                    long length = reader.ReadInt64();
                    if (length < -1 || (length >= 0 && present == 0)) throw new InvalidDataException("Invalid read-closure content length.");
                    result.Probe(full, present != 0);
                    if (length >= 0)
                    {
                        if (contentLength - input.Position < 32) throw new InvalidDataException("Truncated read-closure content digest.");
                        var record = result.paths[full]; record.Length = length; record.Digest = reader.ReadBytes(32);
                        if (record.Digest.Length != 32) throw new EndOfStreamException();
                        result.FileCount++;
                    }
                }
                for (int i = 0; i < reasons; i++)
                {
                    string reason = ReadText(reader, contentLength);
                    if (string.IsNullOrWhiteSpace(reason) || result.unknownSet.Contains(reason)) throw new InvalidDataException("Duplicate read-closure reason.");
                    result.ReportUnknownRead(reason);
                }
                if (input.Position != contentLength) throw new InvalidDataException("Trailing read-closure bytes.");
                result.ManifestBytes = input.Length; return result;
            }
        }
        static void Text(BinaryWriter writer, string value) { byte[] bytes = Utf8.GetBytes(value); writer.Write(bytes.Length); writer.Write(bytes); }
        static string ReadText(BinaryReader reader, long contentLength)
        {
            if (contentLength - reader.BaseStream.Position < 4) throw new InvalidDataException("Truncated read-closure string.");
            int count = reader.ReadInt32();
            if (count <= 0 || count > MaximumPathBytes || count > contentLength - reader.BaseStream.Position)
                throw new InvalidDataException("Invalid read-closure string length.");
            return Utf8.GetString(reader.ReadBytes(count));
        }
        static string PhysicalPath(string path)
        {
            if (string.IsNullOrEmpty(path) || !Path.IsPathRooted(path)) throw new InvalidDataException("Read-closure requires an absolute path.");
            string full = Path.GetFullPath(path);
            if (full.StartsWith(@"\\?\", StringComparison.Ordinal) || full.StartsWith(@"\\.\", StringComparison.Ordinal) ||
                full.IndexOf(':', full.Length >= 2 && full[1] == ':' ? 2 : 0) >= 0)
                throw new InvalidDataException("Invalid read-closure physical path.");
            return full;
        }
        static void RejectReparse(string path)
        {
            if (File.Exists(path) && (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0) throw new IOException("Read-closure rejects reparse points.");
            for (var directory = new DirectoryInfo(path); directory != null; directory = directory.Parent)
                if (directory.Exists && (directory.Attributes & FileAttributes.ReparsePoint) != 0) throw new IOException("Read-closure rejects reparse points.");
        }
        static byte[] Digest(Stream input, CancellationToken cancellation)
        { return Digest(input, cancellation, input.Length - input.Position); }
        static byte[] Digest(Stream input, CancellationToken cancellation, long remaining)
        {
            using (var hash = SHA256.Create())
            {
                byte[] buffer = new byte[65536];
                while (remaining > 0)
                {
                    cancellation.ThrowIfCancellationRequested();
                    int read = input.Read(buffer, 0, (int)Math.Min(buffer.Length, remaining));
                    if (read == 0) throw new EndOfStreamException();
                    hash.TransformBlock(buffer, 0, read, buffer, 0); remaining -= read;
                }
                cancellation.ThrowIfCancellationRequested(); hash.TransformFinalBlock(buffer, 0, 0); return hash.Hash;
            }
        }
        static bool Same(byte[] left, byte[] right)
        { if (left.Length != right.Length) return false; for (int i = 0; i < left.Length; i++) if (left[i] != right[i]) return false; return true; }
    }
}
