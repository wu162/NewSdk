using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

internal static class AudioXmaCodecMetadataOracle
{
    private const string Sha = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139";
    private const int SyntheticCases = 384;
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr GetModuleHandle(string name);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] private static extern uint GetModuleFileName(IntPtr module, StringBuilder name, int capacity);
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)] private delegate int NativeBuild(IntPtr self, IntPtr outputObject);
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)] private delegate int NativeExport(IntPtr self, IntPtr pointer, IntPtr length);
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)] private delegate int NativeSize(IntPtr self);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void NativeFree(IntPtr memory);
    private static IntPtr At(IntPtr value, int offset) { return new IntPtr(unchecked(value.ToInt32() + offset)); }
    private static void Require(bool condition, string label) { if (!condition) throw new Exception(label); }
    private static byte[] Sentinel(int size) { byte[] result = new byte[size]; for (int i = 0; i < size; ++i) result[i] = 0xa5; return result; }
    private static void Equal(string label, byte[] expected, byte[] actual)
    {
        Require(expected.Length == actual.Length, label + " length differs");
        for (int i = 0; i < actual.Length; ++i) if (actual[i] != expected[i]) throw new Exception(label + " byte=" + i + " expected=" + expected[i] + " actual=" + actual[i]);
    }
    private sealed class Guarded : IDisposable
    {
        private readonly IntPtr allocation;
        private readonly int size;
        public readonly IntPtr Pointer;
        public Guarded(byte[] data)
        {
            size = data.Length; allocation = Marshal.AllocHGlobal(checked(size + 64)); Pointer = At(allocation, 32);
            Marshal.Copy(Sentinel(size + 64), 0, allocation, size + 64); Marshal.Copy(data, 0, Pointer, size);
        }
        public byte[] Data() { byte[] result = new byte[size]; Marshal.Copy(Pointer, result, 0, size); return result; }
        public void Dispose()
        {
            try { byte[] before = new byte[32], after = new byte[32]; Marshal.Copy(allocation, before, 0, 32); Marshal.Copy(At(Pointer, size), after, 0, 32); Equal("native prefix guard", Sentinel(32), before); Equal("native suffix guard", Sentinel(32), after); }
            finally { Marshal.FreeHGlobal(allocation); }
        }
    }
    private static IntPtr LoadOriginal(string path)
    {
        Require(IntPtr.Size == 4, "Real original metadata requires an x86 host"); path = Path.GetFullPath(path);
        using (SHA256 hash = SHA256.Create()) Require(BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant() == Sha, "Original DLL fingerprint changed");
        Assembly assembly = Assembly.LoadFrom(path);
        Require(String.Equals(Path.GetFullPath(assembly.Location), path, StringComparison.OrdinalIgnoreCase), "Original managed load path differs");
        IntPtr module = GetModuleHandle(Path.GetFileName(path)); StringBuilder actual = new StringBuilder(32768);
        Require(module != IntPtr.Zero && GetModuleFileName(module, actual, actual.Capacity) != 0 && String.Equals(Path.GetFullPath(actual.ToString()), path, StringComparison.OrdinalIgnoreCase), "Original native load path differs");
        return module;
    }
    private sealed class Fixture
    {
        public string Name; public byte[][] Formats; public uint[] Counts; public uint BlockBytes, PlayLength, LoopBegin, LoopEnd;
    }
    private static IEnumerable<Fixture> Fixtures()
    {
        uint[] values = new uint[] { 0, 1, 63, 64, 65, 0x7fffffff, 0x80000000, 0xffffffff };
        foreach (int streams in new int[] { 1, 2, 3, 8, 16, 64, 255, 256 })
            foreach (int blocks in new int[] { 0, 1, 2, 3, 17, 256 })
                for (int pattern = 0; pattern < 8; ++pattern)
                {
                    byte[][] formats = new byte[streams][];
                    for (int stream = 0; stream < streams; ++stream)
                    {
                        byte[] f = new byte[32];
                        for (int i = 0; i < f.Length; ++i) f[i] = (byte)((i * 37 + stream * 19) ^ (pattern * 31));
                        // Across eight patterns independently cover all eight
                        // mask bits, both native bytes and cross-stream OR.
                        f[30] = (byte)(1 << pattern); f[31] = (byte)(stream == 0 ? 0 : 1 << ((pattern + stream) & 7));
                        formats[stream] = f;
                    }
                    uint[] counts = new uint[blocks];
                    for (int i = 0; i < blocks; ++i) counts[i] = values[(i + pattern) & 7];
                    yield return new Fixture { Name = streams + "-" + blocks + "-" + pattern, Formats = formats, Counts = counts, BlockBytes = pattern == 0 ? 2048u : pattern == 1 ? 163840u : values[pattern], PlayLength = values[(pattern + 2) & 7], LoopBegin = values[pattern], LoopEnd = values[(pattern + 1) & 7] };
                }
    }
    private static byte[] JoinFormats(byte[][] formats)
    { byte[] bytes = new byte[formats.Length * 32]; for (int i = 0; i < formats.Length; ++i) Array.Copy(formats[i], 0, bytes, i * 32, 32); return bytes; }
    private static byte[] CountBytes(uint[] counts)
    { byte[] bytes = new byte[counts.Length * 4]; for (int i = 0; i < counts.Length; ++i) Array.Copy(BitConverter.GetBytes(counts[i]), 0, bytes, i * 4, 4); return bytes; }
    private static byte[] Read(IntPtr pointer, int length)
    { Require(length >= 0 && (length == 0 || pointer != IntPtr.Zero), "Bad original allocation"); byte[] data = new byte[length]; if (length != 0) Marshal.Copy(pointer, data, 0, length); return data; }
    private static byte[] Export(IntPtr obj, NativeExport export, NativeFree free, int expectedLength, IntPtr originalPointer)
    {
        using (Guarded pointer = new Guarded(new byte[4])) using (Guarded length = new Guarded(Sentinel(4)))
        {
            Require(export(obj, pointer.Pointer, length.Pointer) == 0, "Original metadata export HRESULT differs");
            IntPtr result = new IntPtr(BitConverter.ToInt32(pointer.Data(), 0));
            try
            {
                Require(result != IntPtr.Zero && result != originalPointer, "Original metadata export must own a fresh allocation");
                Require(BitConverter.ToInt32(length.Data(), 0) == expectedLength, "Original metadata export size differs");
                return Read(result, expectedLength);
            }
            finally { if (result != IntPtr.Zero) free(result); }
        }
    }
    private static byte[][] RunNative(Fixture fixture, IntPtr module, NativeFree free)
    {
        NativeBuild ea = (NativeBuild)Marshal.GetDelegateForFunctionPointer(At(module, 0x5f170), typeof(NativeBuild));
        NativeBuild wave = (NativeBuild)Marshal.GetDelegateForFunctionPointer(At(module, 0x5f000), typeof(NativeBuild));
        NativeBuild seek = (NativeBuild)Marshal.GetDelegateForFunctionPointer(At(module, 0x5eea0), typeof(NativeBuild));
        NativeExport exportEa = (NativeExport)Marshal.GetDelegateForFunctionPointer(At(module, 0x5af00), typeof(NativeExport));
        NativeExport exportWave = (NativeExport)Marshal.GetDelegateForFunctionPointer(At(module, 0x5afd0), typeof(NativeExport));
        NativeExport exportSeek = (NativeExport)Marshal.GetDelegateForFunctionPointer(At(module, 0x5b570), typeof(NativeExport));
        NativeSize seekSize = (NativeSize)Marshal.GetDelegateForFunctionPointer(At(module, 0x5b5d0), typeof(NativeSize));
        List<Guarded> formats = new List<Guarded>();
        using (Guarded state = new Guarded(new byte[0x4050])) using (Guarded target = new Guarded(new byte[0x30])) using (Guarded seekPair = new Guarded(new byte[8])) using (Guarded counts = new Guarded(CountBytes(fixture.Counts)))
        {
            IntPtr eaPointer = IntPtr.Zero, wavePointer = IntPtr.Zero, seekPointer = IntPtr.Zero;
            try
            {
                for (int i = 0; i < fixture.Formats.Length; ++i)
                { Guarded f = new Guarded(fixture.Formats[i]); formats.Add(f); Marshal.WriteIntPtr(state.Pointer, 0x10 + 0x38 * i, f.Pointer); }
                Marshal.WriteInt32(state.Pointer, 4, fixture.Formats.Length);
                Marshal.WriteInt32(state.Pointer, 0x401c, unchecked((int)fixture.LoopBegin)); Marshal.WriteInt32(state.Pointer, 0x4020, unchecked((int)fixture.LoopEnd));
                Marshal.WriteInt32(state.Pointer, 0x4024, unchecked((int)fixture.PlayLength)); Marshal.WriteInt32(state.Pointer, 0x4028, unchecked((int)fixture.BlockBytes));
                Marshal.WriteInt32(state.Pointer, 0x4038, fixture.Counts.Length); Marshal.WriteIntPtr(state.Pointer, 0x4040, counts.Pointer);
                Marshal.WriteIntPtr(target.Pointer, At(module, 0xc5408)); // concrete flat-seek vtable, whose +8 is 5b5d0
                Marshal.WriteIntPtr(target.Pointer, 0x24, seekPair.Pointer);
                byte[] immutable = state.Data();
                Require(ea(state.Pointer, target.Pointer) == 0, "EA metadata builder HRESULT differs"); eaPointer = Marshal.ReadIntPtr(target.Pointer, 8);
                Require(wave(state.Pointer, target.Pointer) == 0, "WAVE metadata builder HRESULT differs"); wavePointer = Marshal.ReadIntPtr(target.Pointer, 0x10);
                Require(seek(state.Pointer, target.Pointer) == 0, "Seek metadata builder HRESULT differs"); seekPointer = Marshal.ReadIntPtr(seekPair.Pointer, 4);
                int eaLength = 40 + 4 * fixture.Formats.Length, seekLength = fixture.Counts.Length * 4;
                Require(Marshal.ReadInt32(target.Pointer, 0xc) == eaLength && Marshal.ReadInt32(target.Pointer, 0x14) == 52 && Marshal.ReadInt32(seekPair.Pointer) == fixture.Counts.Length && seekSize(target.Pointer) == seekLength, "Native output metadata length differs");
                byte[] eaBytes = Export(target.Pointer, exportEa, free, eaLength, eaPointer), waveBytes = Export(target.Pointer, exportWave, free, 52, wavePointer), seekBytes = Export(target.Pointer, exportSeek, free, seekLength, seekPointer);
                Equal("native EA export clone", Read(eaPointer, eaLength), eaBytes); Equal("native WAVE export clone", Read(wavePointer, 52), waveBytes); Equal("native seek export clone", Read(seekPointer, seekLength), seekBytes);
                Equal("native source state immutable", immutable, state.Data()); Equal("native frame counts immutable", CountBytes(fixture.Counts), counts.Data());
                for (int i = 0; i < formats.Count; ++i) Equal("native stream format immutable", fixture.Formats[i], formats[i].Data());
                return new byte[][] { eaBytes, waveBytes, seekBytes };
            }
            finally
            {
                if (eaPointer != IntPtr.Zero) free(eaPointer); if (wavePointer != IntPtr.Zero) free(wavePointer); if (seekPointer != IntPtr.Zero) free(seekPointer);
                foreach (Guarded format in formats) format.Dispose();
            }
        }
    }
    private static void Original(string dll, string transcript)
    {
        IntPtr module = LoadOriginal(dll); NativeFree free = (NativeFree)Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module, 0xab1a8)), typeof(NativeFree)); int count = 0;
        using (FileStream file = File.Create(transcript)) using (GZipStream gzip = new GZipStream(file, CompressionMode.Compress)) using (StreamWriter writer = new StreamWriter(gzip))
        {
            writer.WriteLine("# originalDllSha256=" + Sha + " realCpu=true nativeRvas=0x5f170,0x5f000,0x5eea0 syntheticStateCases=" + SyntheticCases);
            foreach (Fixture fixture in Fixtures())
            {
                byte[][] result = RunNative(fixture, module, free);
                writer.WriteLine(fixture.Name + "\t" + fixture.BlockBytes + "\t" + fixture.PlayLength + "\t" + fixture.LoopBegin + "\t" + fixture.LoopEnd + "\t" + Convert.ToBase64String(JoinFormats(fixture.Formats)) + "\t" + Convert.ToBase64String(CountBytes(fixture.Counts)) + "\t" + Convert.ToBase64String(result[0]) + "\t" + Convert.ToBase64String(result[1]) + "\t" + Convert.ToBase64String(result[2])); ++count;
            }
        }
        Require(count == SyntheticCases, "Native metadata matrix incomplete");
        Console.WriteLine("PASS original CPU XMA output metadata: " + count + " synthetic-state cases; EA/WAVE/BE cumulative seek, original allocation/export/size routines, immutable input states and guarded buffers; encoding not recovered");
    }
    private static void Reject(MethodInfo method, object[] args, Type expected)
    {
        try { method.Invoke(null, args); }
        catch (TargetInvocationException error) { if (error.InnerException != null && error.InnerException.GetType() == expected) return; throw; }
        throw new Exception("Missing metadata safety rejection " + expected.Name);
    }
    private static void Candidate(string dll, string transcript)
    {
        Require(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BinaryAssetBuilder.AudioCompiler.dll")) && GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll") == IntPtr.Zero, "Original DLL present or loaded in candidate host");
        Type type = Assembly.LoadFrom(Path.GetFullPath(dll)).GetType("BinaryAssetBuilder.AudioCompiler.XmaCodecMetadata", true);
        MethodInfo ea = type.GetMethod("BuildEaFormat"), wave = type.GetMethod("BuildWaveFormat"), seek = type.GetMethod("BuildSeekTable"); Require(ea != null && wave != null && seek != null, "Metadata API missing");
        int count = 0; HashSet<string> names = new HashSet<string>();
        using (FileStream file = File.OpenRead(transcript)) using (GZipStream gzip = new GZipStream(file, CompressionMode.Decompress)) using (StreamReader reader = new StreamReader(gzip))
        {
            string line = reader.ReadLine(); Require(line != null && line.Contains("originalDllSha256=" + Sha) && line.Contains("realCpu=true") && line.Contains("nativeRvas=0x5f170,0x5f000,0x5eea0"), "Original metadata provenance missing");
            while ((line = reader.ReadLine()) != null)
            {
                string[] f = line.Split('\t'); Require(f.Length == 10 && names.Add(f[0]), "Invalid metadata fixture"); byte[] formats = Convert.FromBase64String(f[5]), counts = Convert.FromBase64String(f[6]);
                Require(formats.Length % 32 == 0 && counts.Length % 4 == 0, "Fixture record boundaries differ"); byte[][] inputFormats = new byte[formats.Length / 32][];
                for (int i = 0; i < inputFormats.Length; ++i) { inputFormats[i] = new byte[32]; Array.Copy(formats, i * 32, inputFormats[i], 0, 32); }
                uint[] inputCounts = new uint[counts.Length / 4]; for (int i = 0; i < inputCounts.Length; ++i) inputCounts[i] = BitConverter.ToUInt32(counts, i * 4);
                object[] args = new object[] { inputFormats, inputCounts, UInt32.Parse(f[1]), UInt32.Parse(f[2]), UInt32.Parse(f[3]), UInt32.Parse(f[4]) };
                Equal(f[0] + " EA", Convert.FromBase64String(f[7]), (byte[])ea.Invoke(null, args)); Equal(f[0] + " WAVE", Convert.FromBase64String(f[8]), (byte[])wave.Invoke(null, args)); Equal(f[0] + " seek", Convert.FromBase64String(f[9]), (byte[])seek.Invoke(null, new object[] { inputCounts }));
                Equal("candidate format source immutable", formats, JoinFormats(inputFormats)); Equal("candidate count source immutable", counts, CountBytes(inputCounts)); ++count;
            }
        }
        Require(count == SyntheticCases, "Candidate metadata matrix incomplete"); foreach (Fixture f in Fixtures()) Require(names.Contains(f.Name), "Required metadata fixture missing");
        foreach (MethodInfo formatMethod in new MethodInfo[] { ea, wave })
        {
            Reject(formatMethod, new object[] { null, new uint[0], 0u, 0u, 0u, 0u }, typeof(ArgumentNullException));
            Reject(formatMethod, new object[] { new byte[][] { new byte[32] }, null, 0u, 0u, 0u, 0u }, typeof(ArgumentNullException));
            Reject(formatMethod, new object[] { new byte[0][], new uint[0], 0u, 0u, 0u, 0u }, typeof(ArgumentOutOfRangeException));
            Reject(formatMethod, new object[] { new byte[257][], new uint[0], 0u, 0u, 0u, 0u }, typeof(ArgumentOutOfRangeException));
            Reject(formatMethod, new object[] { new byte[][] { null }, new uint[0], 0u, 0u, 0u, 0u }, typeof(ArgumentException));
            Reject(formatMethod, new object[] { new byte[][] { new byte[31] }, new uint[0], 0u, 0u, 0u, 0u }, typeof(ArgumentException));
            Reject(formatMethod, new object[] { new byte[][] { new byte[33] }, new uint[0], 0u, 0u, 0u, 0u }, typeof(ArgumentException));
        }
        Reject(seek, new object[] { null }, typeof(ArgumentNullException));
        Require(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll") == IntPtr.Zero, "Candidate loaded original target DLL");
        Console.WriteLine("PASS candidate XMA output metadata: " + count + " exact real-CPU synthetic-state cases, EA/WAVE/BE cumulative seek and source immutability; 15 safety rejections; no codec encoding or actual wrapper-state acceptance");
    }
    private static int Main(string[] args)
    {
        try { Require(args.Length == 3, "Usage: original|candidate assembly.dll transcript.tsv.gz"); if (args[0] == "original") Original(args[1], args[2]); else if (args[0] == "candidate") Candidate(args[1], args[2]); else throw new Exception("Unknown mode"); return 0; }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
