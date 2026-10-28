using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace NewModCompiler.Application
{
    // Build-local, bounded digest reuse. A path/mtime/size is never a version:
    // the original read lease stays open and a freshly opened current path must
    // resolve to the same Win32 file identity before its digest can be reused.
    public sealed class CacheInputLeaseSet : IDisposable
    {
        sealed class Entry
        {
            public FileStream Input;
            public FileIdentity Identity;
            public byte[] Digest;
            public long Length;
        }
        // FILE_ID_INFO: the 128-bit ID is required for ReFS as well as NTFS.
        // Unsupported filesystems/older OS APIs fall back to fresh SHA256 reads.
        [StructLayout(LayoutKind.Sequential)]
        struct FileIdentity
        {
            public ulong Volume, IndexLow, IndexHigh;
            public bool SameFile(FileIdentity other)
            { return Volume == other.Volume && IndexHigh == other.IndexHigh && IndexLow == other.IndexLow; }
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetFileInformationByHandleEx(SafeFileHandle handle, int informationClass, out FileIdentity information, uint bufferSize);

        readonly Dictionary<string, Entry> entries = new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
        readonly int maximumFiles;
        long minimumRetainedLength;
        bool disposed;
        public CacheInputLeaseSet(int maximumFiles = 512)
        {
            if (maximumFiles < 1 || maximumFiles > 512) throw new ArgumentOutOfRangeException("maximumFiles");
            this.maximumFiles = maximumFiles;
        }
        public int RetainedFiles { get { return entries.Count; } }
        public long ReusedFiles { get; private set; }
        public long ReusedBytes { get; private set; }
        public long IdentityQueries { get; private set; }
        public long IdentitySkippedFiles { get; private set; }

        internal byte[] Read(string path, CancellationToken cancellation, SHA256 hash, out long length, out bool reused)
        {
            if (disposed) throw new ObjectDisposedException("CacheInputLeaseSet");
            cancellation.ThrowIfCancellationRequested();
            FileStream input = null;
            try
            {
                input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                length = input.Length;
                FileIdentity current = new FileIdentity();
                bool identified = false;
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    // Size decides only whether a NEW file can enter the bounded
                    // retained set. It never authorizes digest reuse: every old
                    // entry must still prove the freshly opened path's file ID.
                    bool needsIdentity = entries.ContainsKey(path) || entries.Count < maximumFiles || length > minimumRetainedLength;
                    if (needsIdentity)
                    {
                        IdentityQueries = checked(IdentityQueries + 1);
                        identified = GetFileInformationByHandleEx(input.SafeFileHandle, 18, out current, 24);
                    }
                    else IdentitySkippedFiles = checked(IdentitySkippedFiles + 1);
                }
                Entry entry;
                if (identified && entries.TryGetValue(path, out entry) && entry.Identity.SameFile(current))
                {
                    cancellation.ThrowIfCancellationRequested();
                    length = entry.Length; reused = true;
                    ReusedFiles = checked(ReusedFiles + 1); ReusedBytes = checked(ReusedBytes + length);
                    return entry.Digest;
                }
                // An ancestor can be renamed/replaced on some filesystems.
                // Never reuse an old file's digest at its replacement path.
                if (entries.TryGetValue(path, out entry))
                { entries.Remove(path); entry.Input.Dispose(); }
                // Capture owns this sequential hasher; ComputeHash resets its
                // state after each file. No object is shared with backend work.
                byte[] digest = hash.ComputeHash(input);
                cancellation.ThrowIfCancellationRequested();
                reused = false;
                if (identified)
                {
                    // Spend the bounded handle budget on the largest leased inputs,
                    // rather than filling it with the first tiny schema leaves.
                    if (entries.Count == maximumFiles && length > minimumRetainedLength)
                    {
                        string smallest = null;
                        foreach (var pair in entries)
                            if (pair.Value.Length == minimumRetainedLength) { smallest = pair.Key; break; }
                        Entry removed = entries[smallest]; entries.Remove(smallest); removed.Input.Dispose();
                    }
                    if (entries.Count < maximumFiles)
                    {
                        entries.Add(path, new Entry { Input = input, Identity = current, Digest = digest, Length = length });
                        input = null;
                        minimumRetainedLength = long.MaxValue;
                        foreach (Entry retained in entries.Values) minimumRetainedLength = Math.Min(minimumRetainedLength, retained.Length);
                    }
                }
                return digest;
            }
            finally { if (input != null) input.Dispose(); }
        }
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            var failures = new List<Exception>();
            foreach (Entry entry in entries.Values)
                try { entry.Input.Dispose(); } catch (Exception error) { failures.Add(error); }
            entries.Clear();
            if (failures.Count != 0) throw new AggregateException("Input digest lease cleanup failed.", failures);
        }
    }
}
