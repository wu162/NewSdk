using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using EALA.Hash;
using Microsoft.Win32.SafeHandles;

namespace BinaryAssetBuilder.Core;

// A bounded, build-owned positive-input snapshot. Pins prove that captured files
// and their path ancestors cannot be written/replaced while consumers use their
// sealed bytes or the original native filename. This is not a query/closure cache.
public sealed class PhysicalInputSnapshot : IDisposable
{
    [ThreadStatic] private static PhysicalInputSnapshot _Current;
    private readonly int _Thread = Thread.CurrentThread.ManagedThreadId;
    private readonly string _Scratch;
    private readonly string _SnapshotId = Guid.NewGuid().ToString("N");
    private readonly long _MemoryBudget, _DiskBudget;
    private readonly int _FileLimit, _DirectoryLimit;
    private readonly Dictionary<string, Entry> _Paths = new Dictionary<string, Entry>(StringComparer.Ordinal);
    private readonly Dictionary<string, Entry> _Identities = new Dictionary<string, Entry>(StringComparer.Ordinal);
    private readonly Dictionary<string, SafeFileHandle> _Directories = new Dictionary<string, SafeFileHandle>(StringComparer.Ordinal);
    private long _Captures, _Hits, _Bypasses, _CapturedBytes, _MemoryBytes, _DiskBytes, _VerificationReads, _VerificationBytes, _CleanupFailures;
    private bool _Disposed, _OwnsScratch, _ReadStarted;
    private readonly List<string> _ExcludedRoots = new List<string>();
    private readonly Dictionary<string, long> _BypassReasons = new Dictionary<string, long>(StringComparer.Ordinal);
    private string _RetainedScratch;
    private CancellationToken _Cancellation;

    private const int BlockSize = 1048576;
    private const uint BackupSemantics = 0x02000000, OpenReparsePoint = 0x00200000;
    [StructLayout(LayoutKind.Sequential)] private struct FileInformation
    {
        public uint Attributes; public System.Runtime.InteropServices.ComTypes.FILETIME Created, Accessed, Written;
        public uint Volume, SizeHigh, SizeLow, Links, IndexHigh, IndexLow;
    }
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern SafeFileHandle CreateFile(string path, uint access, uint share, IntPtr security, uint disposition, uint flags, IntPtr template);
    [DllImport("kernel32.dll", EntryPoint = "CreateDirectoryW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool CreateDirectoryNative(string path, IntPtr security);
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetFileInformationByHandle(SafeFileHandle handle, out FileInformation information);

    public PhysicalInputSnapshot(string privateScratchDirectory) : this(privateScratchDirectory, 32L * 1024 * 1024, 512L * 1024 * 1024, 4096, 4096) { }
    public PhysicalInputSnapshot(string privateScratchDirectory, long memoryBudget, long diskBudget, int fileLimit, int directoryLimit)
    {
        if (string.IsNullOrEmpty(privateScratchDirectory)) throw new ArgumentException("A private snapshot scratch directory is required.");
        if (memoryBudget < 0 || diskBudget < 0 || fileLimit < 0 || directoryLimit < 0) throw new ArgumentOutOfRangeException("Snapshot budgets cannot be negative.");
        _Scratch = Path.GetFullPath(privateScratchDirectory); _MemoryBudget = memoryBudget; _DiskBudget = diskBudget;
        _FileLimit = fileLimit; _DirectoryLimit = directoryLimit;
    }
    public void ExcludeRoot(string path)
    {
        Check();
        if (_ReadStarted) throw new InvalidOperationException("Input exclusions must be registered before the first capture.");
        _ExcludedRoots.Add(Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
    }
    private void Check()
    {
        if (_Disposed) throw new ObjectDisposedException("PhysicalInputSnapshot");
        if (_Thread != Thread.CurrentThread.ManagedThreadId) throw new InvalidOperationException("Input snapshots belong to their creating thread.");
    }
    public IDisposable BeginReadScope(CancellationToken cancellation = default(CancellationToken))
    {
        Check();
        if (_Current != null) throw new InvalidOperationException("An input snapshot read scope is already active.");
        _Cancellation = cancellation; _Current = this; return new ReadScope(this);
    }
    private sealed class ReadScope : IDisposable
    {
        private PhysicalInputSnapshot owner;
        internal ReadScope(PhysicalInputSnapshot owner) { this.owner = owner; }
        public void Dispose()
        {
            if (owner == null) return;
            if (owner._Thread != Thread.CurrentThread.ManagedThreadId || (!owner._Disposed && !Object.ReferenceEquals(_Current, owner)))
                throw new InvalidOperationException("Input snapshot read scopes must close on their owning thread.");
            if (Object.ReferenceEquals(_Current, owner)) _Current = null;
            owner._Cancellation = default(CancellationToken); owner = null;
        }
    }
    public sealed class Entry
    {
        private readonly byte[] memory;
        private readonly FileStream storage;
        private readonly object gate = new object();
        internal readonly FileStream Pin;
        internal readonly string Identity;
        internal readonly List<string> Paths = new List<string>();
        public long Length { get; private set; }
        public string Sha256 { get; private set; }
        public uint LegacyHash { get; private set; }
        public DateTime LastWriteTime { get; private set; }
        internal Entry(string path, FileStream pin, string identity, long length, string strong, uint legacy, DateTime date, byte[] memory, FileStream storage)
        {
            Paths.Add(path); Pin = pin; Identity = identity; Length = length; Sha256 = strong; LegacyHash = legacy;
            LastWriteTime = date; this.memory = memory; this.storage = storage;
        }
        public Stream OpenRead() { return memory != null ? (Stream)new MemoryStream(memory, false) : new ReadView(this); }
        internal void CloseStorage() { if (storage != null) storage.Dispose(); }
        private sealed class ReadView : Stream
        {
            private Entry entry; private long position;
            internal ReadView(Entry entry) { this.entry = entry; }
            private Entry Active { get { if (entry == null) throw new ObjectDisposedException("Snapshot read view"); return entry; } }
            public override bool CanRead { get { return entry != null; } }
            public override bool CanSeek { get { return entry != null; } }
            public override bool CanWrite { get { return false; } }
            public override long Length { get { return Active.Length; } }
            public override long Position { get { return position; } set { if (value < 0) throw new ArgumentOutOfRangeException("value"); position = value; } }
            public override int Read(byte[] buffer, int offset, int count)
            {
                Entry value = Active;
                lock (value.gate) { value.storage.Position = position; int read = value.storage.Read(buffer, offset, count); position += read; return read; }
            }
            public override long Seek(long offset, SeekOrigin origin)
            {
                long target = origin == SeekOrigin.Begin ? offset : origin == SeekOrigin.Current ? position + offset : origin == SeekOrigin.End ? Length + offset : -1;
                if (target < 0) throw new IOException("Invalid snapshot seek."); position = target; return position;
            }
            public override void Flush() { }
            public override void SetLength(long value) { throw new NotSupportedException(); }
            public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
            protected override void Dispose(bool disposing) { entry = null; base.Dispose(disposing); }
        }
    }
    public static bool TryCapture(string path, out Entry entry)
    {
        bool captured; return TryCapture(path, out entry, out captured);
    }
    public static bool TryCapture(string path, out Entry entry, out bool captured)
    {
        entry = null; captured = false;
        return _Current != null && _Current.Capture(path, out entry, out captured);
    }
    public static Stream OpenRead(string path)
    {
        Entry entry; return TryCapture(path, out entry) ? entry.OpenRead() : File.OpenRead(path);
    }
    public static byte[] ReadAllBytes(string path)
    {
        Entry entry;
        if (!TryCapture(path, out entry)) return File.ReadAllBytes(path);
        using (var input = entry.OpenRead()) using (var output = new MemoryStream()) { input.CopyTo(output); return output.ToArray(); }
    }
    public void RegisterBypass(string reason)
    {
        Check();
        switch (reason)
        {
            case "unsupported-path": case "generated-output-root": case "snapshot-storage": case "file-budget":
            case "missing-input": case "parent-pin": case "reparse-input": case "storage-budget": case "opaque-texture-closure": break;
            default: reason = "other-consumer"; break;
        }
        long count; _BypassReasons.TryGetValue(reason, out count); _BypassReasons[reason] = count + 1; _Bypasses++;
    }
    private bool Bypass(out Entry entry, out bool captured, string reason = "unsupported-path")
    { RegisterBypass(reason); entry = null; captured = false; return false; }
    private static string Identity(FileStream file, string path)
    {
        FileInformation value;
        if (!GetFileInformationByHandle(file.SafeFileHandle, out value)) throw new IOException("Cannot identify an input file.", new Win32Exception(Marshal.GetLastWin32Error()));
        if ((value.Attributes & 0x400) != 0) throw new IOException("Captured input is a reparse point.");
        if (value.IndexHigh == 0 && value.IndexLow == 0) return null;
        return Path.GetPathRoot(Path.GetFullPath(path)).ToUpperInvariant() + ":" + value.Volume.ToString("x8") + value.IndexHigh.ToString("x8") + value.IndexLow.ToString("x8");
    }
    private static bool Normal(string path, out string full)
    {
        full = null;
        if (Environment.OSVersion.Platform != PlatformID.Win32NT || string.IsNullOrEmpty(path)) return false;
        string syntax = path.Replace('/', '\\');
        if (syntax.StartsWith(@"\\?\", StringComparison.Ordinal) || syntax.StartsWith(@"\\.\", StringComparison.Ordinal)) return false;
        int start = syntax.Length >= 2 && syntax[1] == ':' ? 2 : 0;
        if (!Path.IsPathRooted(syntax) || syntax.IndexOf(':', start) >= 0 ||
            (start == 2 && (syntax.Length == 2 || syntax[2] != '\\'))) return false;
        foreach (string part in syntax.Substring(start).Split('\\'))
            if (part.EndsWith(".", StringComparison.Ordinal) || part.EndsWith(" ", StringComparison.Ordinal)) return false;
        try { full = Path.GetFullPath(path); return true; }
        catch (ArgumentException) { return false; }
        catch (NotSupportedException) { return false; }
        catch (PathTooLongException) { return false; }
    }
    private bool PinParents(string path)
    {
        var pending = new Stack<string>();
        for (var directory = new DirectoryInfo(Path.GetDirectoryName(path)); directory != null && directory.Parent != null; directory = directory.Parent)
            pending.Push(directory.FullName);
        foreach (string directory in pending)
        {
            if (_Directories.ContainsKey(directory)) continue;
            if (_Directories.Count >= _DirectoryLimit) return false;
            SafeFileHandle handle = CreateFile(directory, 0x80, 3, IntPtr.Zero, 3, BackupSemantics | OpenReparsePoint, IntPtr.Zero);
            if (handle.IsInvalid) { handle.Dispose(); return false; }
            FileInformation information;
            if (!GetFileInformationByHandle(handle, out information) || (information.Attributes & 0x410) != 0x10)
            { handle.Dispose(); return false; }
            _Directories.Add(directory, handle);
        }
        return true;
    }
    private bool Capture(string path, out Entry entry, out bool captured)
    {
        Check(); _Cancellation.ThrowIfCancellationRequested(); _ReadStarted = true;
        entry = null; captured = false;
        string full;
        if (!Normal(path, out full)) return Bypass(out entry, out captured);
        foreach (string excluded in _ExcludedRoots)
            if (string.Equals(full, excluded, StringComparison.OrdinalIgnoreCase) || full.StartsWith(excluded + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                return Bypass(out entry, out captured, "generated-output-root");
        if (string.Equals(full, _Scratch, StringComparison.OrdinalIgnoreCase) || full.StartsWith(_Scratch.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            return Bypass(out entry, out captured, "snapshot-storage");
        if (_Paths.TryGetValue(full, out entry)) { _Hits++; return true; }
        if (_Paths.Count >= _FileLimit) return Bypass(out entry, out captured, "file-budget");
        if (!File.Exists(full)) return Bypass(out entry, out captured, "missing-input");
        var existingDirectories = new HashSet<string>(_Directories.Keys, StringComparer.Ordinal);
        bool keepDirectories = false;
        FileStream pin = null, storage = null; string scratchFile = null;
        try
        {
            if (!PinParents(full)) return Bypass(out entry, out captured, "parent-pin");
            if ((File.GetAttributes(full) & FileAttributes.ReparsePoint) != 0) return Bypass(out entry, out captured, "reparse-input");
            SafeFileHandle handle = CreateFile(full, 0x80000000, 1, IntPtr.Zero, 3, OpenReparsePoint | 0x08000000, IntPtr.Zero);
            if (handle.IsInvalid)
            {
                int error = Marshal.GetLastWin32Error(); handle.Dispose();
                if (error == 2 || error == 3) throw new FileNotFoundException("Input file is unavailable.", full);
                if (error == 5) throw new UnauthorizedAccessException("Input file cannot be opened: " + full);
                throw new IOException("Cannot open captured input: " + full, new Win32Exception(error));
            }
            FileInformation information;
            if (!GetFileInformationByHandle(handle, out information)) { handle.Dispose(); throw new IOException("Cannot identify captured input: " + full); }
            if ((information.Attributes & 0x400) != 0) { handle.Dispose(); return Bypass(out entry, out captured, "reparse-input"); }
            try { pin = new FileStream(handle, FileAccess.Read, 4096, false); }
            catch { handle.Dispose(); throw; }
            string identity = Identity(pin, full);
            if (identity == null) return Bypass(out entry, out captured);
            if (_Identities.TryGetValue(identity, out entry))
            {
                entry.Paths.Add(full); _Paths.Add(full, entry); _Hits++; keepDirectories = true; pin.Dispose(); pin = null; return true;
            }
            long length = pin.Length;
            bool inMemory = length <= BlockSize && length <= _MemoryBudget - _MemoryBytes;
            if (!inMemory && length > _DiskBudget - _DiskBytes) return Bypass(out entry, out captured, "storage-budget");
            MemoryStream memory = inMemory ? new MemoryStream((int)length) : null;
            if (!inMemory)
            {
                if (!_OwnsScratch)
                {
                    if (!CreateDirectoryNative(_Scratch, IntPtr.Zero))
                        throw new IOException("Cannot create exclusively owned snapshot scratch: " + _Scratch, new Win32Exception(Marshal.GetLastWin32Error()));
                    _OwnsScratch = true;
                }
                scratchFile = Path.Combine(_Scratch, Guid.NewGuid().ToString("N") + ".input");
                storage = new FileStream(scratchFile, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
            }
            string strong; uint legacy = unchecked((uint)length); long readBytes = 0;
            using (var algorithm = SHA256.Create())
            {
                byte[] buffer = new byte[(int)Math.Min(Math.Max(length, 1), BlockSize)];
                while (true)
                {
                    _Cancellation.ThrowIfCancellationRequested();
                    int count = 0, next;
                    while (count < buffer.Length && (next = pin.Read(buffer, count, buffer.Length - count)) != 0) count += next;
                    if (count == 0) break;
                    legacy = FastHash.GetHashCode(legacy, buffer, count);
                    algorithm.TransformBlock(buffer, 0, count, buffer, 0);
                    if (inMemory) memory.Write(buffer, 0, count); else storage.Write(buffer, 0, count);
                    readBytes += count;
                }
                algorithm.TransformFinalBlock(new byte[0], 0, 0); strong = Convert.ToBase64String(algorithm.Hash);
            }
            if (readBytes != length) throw new IOException("Input length changed while sealing: " + full);
            if (storage != null) storage.Flush();
            entry = new Entry(full, pin, identity, length, strong, legacy, File.GetLastWriteTime(full), inMemory ? memory.ToArray() : null, storage);
            pin = null; storage = null;
            _Paths.Add(full, entry); _Identities.Add(identity, entry);
            _Captures++; _CapturedBytes += length; if (inMemory) _MemoryBytes += length; else _DiskBytes += length;
            keepDirectories = true; captured = true; return true;
        }
        finally
        {
            if (pin != null) try { pin.Dispose(); } catch { _CleanupFailures++; }
            if (storage != null)
            {
                try { storage.Dispose(); } catch { _CleanupFailures++; }
            }
            if (!keepDirectories)
            {
                var release = new List<string>();
                foreach (string directory in _Directories.Keys) if (!existingDirectories.Contains(directory)) release.Add(directory);
                foreach (string directory in release)
                {
                    try { _Directories[directory].Dispose(); } catch { _CleanupFailures++; }
                    _Directories.Remove(directory);
                }
            }
        }
    }
    public void VerifyUnchanged(CancellationToken cancellation = default(CancellationToken))
    {
        Check();
        foreach (Entry entry in _Identities.Values)
        {
            bool first = true;
            foreach (string path in entry.Paths)
            {
                cancellation.ThrowIfCancellationRequested();
                using (var input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (Identity(input, path) != entry.Identity || input.Length != entry.Length) throw new IOException("Captured input identity changed: " + path);
                    if (first)
                    {
                        using (var algorithm = SHA256.Create())
                            if (!string.Equals(Convert.ToBase64String(algorithm.ComputeHash(input)), entry.Sha256, StringComparison.Ordinal)) throw new IOException("Captured input contents changed: " + path);
                        _VerificationReads++; _VerificationBytes += input.Position; first = false;
                    }
                }
            }
        }
    }
    public object GetMetrics()
    {
        return new { enabled = true, snapshotId = _SnapshotId, positiveInputsOnly = true, captures = _Captures, hits = _Hits, bypasses = _Bypasses, capturedBytes = _CapturedBytes,
            memoryBytes = _MemoryBytes, diskBytes = _DiskBytes, verificationReads = _VerificationReads, verificationBytes = _VerificationBytes,
            activeFilePins = _Disposed ? 0 : _Identities.Count, activeDirectoryPins = _Disposed ? 0 : _Directories.Count,
            bypassReasons = new Dictionary<string, long>(_BypassReasons, StringComparer.Ordinal), cleanupFailures = _CleanupFailures, retainedScratch = _RetainedScratch };
    }
    public void Dispose()
    {
        if (_Disposed) return;
        _Disposed = true;
        if (Object.ReferenceEquals(_Current, this)) _Current = null;
        foreach (Entry entry in _Identities.Values)
        {
            try { entry.CloseStorage(); } catch { _CleanupFailures++; }
            try { entry.Pin.Dispose(); } catch { _CleanupFailures++; }
        }
        foreach (SafeFileHandle directory in _Directories.Values)
            try { directory.Dispose(); } catch { _CleanupFailures++; }
        _Directories.Clear(); _Identities.Clear(); _Paths.Clear();
        if (_OwnsScratch)
            try { if (Directory.Exists(_Scratch)) Directory.Delete(_Scratch, false); }
            catch { _CleanupFailures++; _RetainedScratch = _Scratch; }
    }
}
