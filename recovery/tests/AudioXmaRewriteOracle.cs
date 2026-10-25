using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

internal static class AudioXmaRewriteOracle
{
    private const string Sha = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139";
    private const int SyntheticCases = 793;
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr GetModuleHandle(string name);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] private static extern uint GetModuleFileName(IntPtr module, StringBuilder name, int capacity);
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)] private delegate int NativeRewrite(IntPtr self, IntPtr destination, IntPtr source, int length, uint flags);
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
    private static NativeRewrite LoadOriginal(string path)
    {
        Require(IntPtr.Size == 4, "Real original rewrite requires an x86 host"); path = Path.GetFullPath(path);
        using (SHA256 hash = SHA256.Create()) Require(BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant() == Sha, "Original DLL fingerprint changed");
        Assembly assembly = Assembly.LoadFrom(path);
        Require(String.Equals(Path.GetFullPath(assembly.Location), path, StringComparison.OrdinalIgnoreCase), "Original managed load path differs");
        IntPtr module = GetModuleHandle(Path.GetFileName(path)); StringBuilder actual = new StringBuilder(32768);
        Require(module != IntPtr.Zero && GetModuleFileName(module, actual, actual.Capacity) != 0 && String.Equals(Path.GetFullPath(actual.ToString()), path, StringComparison.OrdinalIgnoreCase), "Original native load path differs");
        return (NativeRewrite)Marshal.GetDelegateForFunctionPointer(At(module, 0xee00), typeof(NativeRewrite));
    }
    private sealed class Fixture
    {
        public string Name; public byte[] Source; public int Streams; public uint Flags; public byte[] HistoricalEa;
        public Fixture(string name, byte[] source, int streams, uint flags, byte[] historicalEa = null) { Name = name; Source = source; Streams = streams; Flags = flags; HistoricalEa = historicalEa; }
    }
    private static byte[] Source(int streams, int packets, int skips, int trim, int seed)
    {
        byte[] source = new byte[packets * 2048];
        for (int i = 0; i < source.Length; ++i) source[i] = (byte)((i * 37) ^ (i >> 8) ^ seed);
        for (int packet = 0; packet < packets; ++packet)
        {
            int start = packet * 2048;
            source[start] = (byte)(0x80 | (seed & 0x7f)); source[start + 1] = (byte)(packet * 19); source[start + 2] = 0x5a;
            source[start + 3] = (byte)(skips == 0 ? 0 : skips == 1 ? streams - 1 : (packet * 5 + seed) % 5);
            if (trim < 2044) source[start + 2047 - trim] = 0x7e;
            for (int i = 0; i < trim; ++i) source[start + 2047 - i] = 0xff;
        }
        return source;
    }
    private static IEnumerable<Fixture> Fixtures(string rawInputs)
    {
        int index = 0;
        foreach (int streams in new int[] { 1, 2, 3, 4 })
            foreach (int packets in new int[] { streams, streams * 2, streams * 3 + 1 })
                foreach (int skips in new int[] { 0, 1, 2 })
                    foreach (int trim in new int[] { 0, 1, 7, 2043, 2044 })
                        foreach (uint flags in new uint[] { 0, 1, 2, 3 })
                        {
                            string name = "matrix-" + streams + "-" + packets + "-" + skips + "-" + trim + "-" + flags;
                            yield return new Fixture(name, Source(streams, packets, skips, trim, ++index), streams, flags);
                        }
        Random rng = new Random(0xee00);
        for (int i = 0; i < 64; ++i)
        {
            int streams = 1 + rng.Next(8), packets = streams + rng.Next(20), trim = rng.Next(2045);
            byte[] source = Source(streams, packets, 2, trim, rng.Next(255));
            for (int p = 0; p < packets; ++p) source[p * 2048 + 3] = (byte)rng.Next(256);
            yield return new Fixture("random-" + i, source, streams, (uint)rng.Next(4));
        }
        foreach (uint flags in new uint[] { 4, 7, 255, 256, 0x7fffffff, 0x80000000, 0xfffffffc, 0xffffffff })
            yield return new Fixture("flag-" + flags, Source(2, 5, 1, 17, 7), 2, flags);
        yield return new Fixture("zero-streams", new byte[0], 0, 0xffffffff);
        if (!String.IsNullOrEmpty(rawInputs))
            foreach (string line in File.ReadAllLines(rawInputs))
            {
                if (line.StartsWith("#") || line.Length == 0) continue;
                string[] f = line.Split('\t'); Require(f.Length == 5, "Bad raw input manifest or missing historical plugin output");
                yield return new Fixture("raw-" + f[0], Convert.FromBase64String(f[3]), Int32.Parse(f[1]), UInt32.Parse(f[2]), Convert.FromBase64String(f[4]));
            }
    }
    private static int Original(string dll, string transcript, string rawInputs)
    {
        NativeRewrite native = LoadOriginal(dll); int count = 0, real = 0;
        using (FileStream file = File.Create(transcript)) using (GZipStream gzip = new GZipStream(file, CompressionMode.Compress)) using (StreamWriter writer = new StreamWriter(gzip))
        {
            writer.WriteLine("# originalDllSha256=" + Sha + " nativeRva=0xee00 realCpu=true syntheticCases=" + SyntheticCases);
            foreach (Fixture f in Fixtures(rawInputs))
            {
                int capacity = checked((4 + f.Source.Length) * Math.Max(f.Streams, 1));
                byte[] state = Sentinel(0x3c); Array.Copy(BitConverter.GetBytes(f.Streams), 0, state, 0x2c, 4);
                int length; byte[] output;
                using (Guarded obj = new Guarded(state)) using (Guarded source = new Guarded(f.Source)) using (Guarded destination = new Guarded(Sentinel(capacity)))
                {
                    length = native(obj.Pointer, destination.Pointer, source.Pointer, f.Source.Length, f.Flags);
                    Require(length >= 0 && length <= capacity, "Original output length outside bounded fixture");
                    output = destination.Data(); Equal(f.Name + " source immutable", f.Source, source.Data()); Equal(f.Name + " object immutable", state, obj.Data());
                }
                if (f.HistoricalEa != null)
                {
                    Require(length == f.HistoricalEa.Length, f.Name + " historical plugin EA length differs");
                    byte[] logical = new byte[length]; Array.Copy(output, logical, length); Equal(f.Name + " raw-to-original-rewriter-to-historical-plugin closed loop", f.HistoricalEa, logical);
                }
                writer.WriteLine(f.Name + "\t" + f.Streams + "\t" + f.Flags + "\t" + length + "\t" + Convert.ToBase64String(f.Source) + "\t" + Convert.ToBase64String(output) + "\t" + (f.HistoricalEa == null ? "-" : Convert.ToBase64String(f.HistoricalEa)));
                if (f.Name.StartsWith("raw-")) ++real; ++count;
            }
        }
        Require(count == SyntheticCases + real, "Original fixture count differs");
        Console.WriteLine("PASS original XMA output rewrite: " + count + " exact real-CPU cases; synthetic=" + SyntheticCases + "; rawBlocks=" + real + "; return length, complete writes, source/state immutability and guards; encoding not recovered");
        return count;
    }
    private static void Reject(MethodInfo method, object[] args, Type expected)
    {
        byte[] destination = (byte[])args[4], before = destination == null ? null : (byte[])destination.Clone();
        try { method.Invoke(null, args); }
        catch (TargetInvocationException error)
        {
            if (error.InnerException == null || error.InnerException.GetType() != expected) throw;
            if (before != null) Equal("safe rejection leaves destination unchanged", before, destination);
            return;
        }
        throw new Exception("Missing safety rejection " + expected.Name);
    }
    private static int Candidate(string dll, string transcript)
    {
        Require(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BinaryAssetBuilder.AudioCompiler.dll")), "Original target DLL exists in candidate host");
        Require(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll") == IntPtr.Zero, "Original target module is loaded in candidate host");
        Assembly assembly = Assembly.LoadFrom(Path.GetFullPath(dll)); Type type = assembly.GetType("BinaryAssetBuilder.AudioCompiler.XmaOutputRewriter", true);
        MethodInfo rewrite = type.GetMethod("Rewrite", new Type[] { typeof(byte[]), typeof(int), typeof(int), typeof(uint), typeof(byte[]), typeof(int) });
        MethodInfo compact = type.GetMethod("Rewrite", new Type[] { typeof(byte[]), typeof(int), typeof(uint) });
        Require(rewrite != null && compact != null, "Candidate rewrite API missing");
        int count = 0, real = 0; HashSet<string> names = new HashSet<string>();
        using (FileStream file = File.OpenRead(transcript)) using (GZipStream gzip = new GZipStream(file, CompressionMode.Decompress)) using (StreamReader reader = new StreamReader(gzip))
        {
            string line = reader.ReadLine(); Require(line != null && line.Contains("originalDllSha256=" + Sha) && line.Contains("nativeRva=0xee00") && line.Contains("realCpu=true") && line.Contains("syntheticCases=" + SyntheticCases), "Original CPU transcript provenance missing");
            while ((line = reader.ReadLine()) != null)
            {
                string[] f = line.Split('\t'); Require(f.Length == 7 && names.Add(f[0]), "Invalid or duplicate fixture");
                int streams = Int32.Parse(f[1]), expectedLength = Int32.Parse(f[3]); uint flags = UInt32.Parse(f[2]);
                byte[] source = Convert.FromBase64String(f[4]), expected = Convert.FromBase64String(f[5]), output = Sentinel(expected.Length + 34);
                int length = (int)rewrite.Invoke(null, new object[] { source, source.Length, streams, flags, output, 17 });
                Require(length == expectedLength, f[0] + " return length differs");
                byte[] whole = Sentinel(output.Length); Array.Copy(expected, 0, whole, 17, expected.Length); Equal(f[0] + " full writes and untouched guards", whole, output);
                Equal(f[0] + " source immutable", Convert.FromBase64String(f[4]), source);
                byte[] logical = (byte[])compact.Invoke(null, new object[] { source, streams, flags }), expectedLogical = new byte[length]; Array.Copy(expected, expectedLogical, length); Equal(f[0] + " compact output", expectedLogical, logical);
                if (f[0].StartsWith("raw-"))
                {
                    Require(f[6] != "-", "Historical plugin EA output is missing");
                    Equal(f[0] + " candidate to historical plugin closed loop", Convert.FromBase64String(f[6]), logical); ++real;
                }
                else Require(f[6] == "-", "Unexpected historical output on synthetic fixture");
                ++count;
            }
        }
        Require(count == SyntheticCases + real, "Candidate matrix count differs");
        foreach (Fixture fixture in Fixtures(null)) Require(names.Contains(fixture.Name), "Required fixture missing: " + fixture.Name);
        byte[] valid = Source(1, 1, 0, 0, 1), dst = Sentinel(4096);
        Reject(rewrite, new object[] { null, 0, 0, 0u, dst, 0 }, typeof(ArgumentNullException));
        Reject(rewrite, new object[] { valid, 2048, 1, 0u, null, 0 }, typeof(ArgumentNullException));
        Reject(rewrite, new object[] { valid, -1, 1, 0u, dst, 0 }, typeof(ArgumentOutOfRangeException));
        Reject(rewrite, new object[] { valid, 2047, 1, 0u, dst, 0 }, typeof(ArgumentOutOfRangeException));
        Reject(rewrite, new object[] { valid, 4096, 1, 0u, dst, 0 }, typeof(ArgumentOutOfRangeException));
        Reject(rewrite, new object[] { valid, 2048, -1, 0u, dst, 0 }, typeof(ArgumentOutOfRangeException));
        Reject(rewrite, new object[] { valid, 2048, 2, 0u, dst, 0 }, typeof(ArgumentOutOfRangeException));
        Reject(rewrite, new object[] { valid, 2048, 1, 0u, dst, -1 }, typeof(ArgumentOutOfRangeException));
        Reject(rewrite, new object[] { valid, 2048, 1, 0u, Sentinel(2051), 0 }, typeof(ArgumentOutOfRangeException));
        byte[] allff = new byte[2048]; for (int i = 0; i < allff.Length; ++i) allff[i] = 0xff;
        Reject(rewrite, new object[] { allff, 2048, 1, 0u, dst, 0 }, typeof(ArgumentException));
        Reject(rewrite, new object[] { valid, 2048, 1, 0u, valid, 0 }, typeof(ArgumentException));
        Reject(rewrite, new object[] { new byte[0], 0, 1, 0u, dst, 0 }, typeof(ArgumentOutOfRangeException));
        Require(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll") == IntPtr.Zero, "Candidate loaded original target module");
        Console.WriteLine("PASS candidate XMA output rewrite: " + count + " exact real-CPU cases; synthetic=" + SyntheticCases + "; rawBlocks=" + real + "; full writes, guards, both APIs and source immutability; 12 pre-write safety rejections; encoding not recovered");
        return count;
    }
    private static int Main(string[] args)
    {
        try
        {
            Require(args.Length >= 3 && args.Length <= 4, "Usage: original|candidate assembly.dll transcript.tsv.gz [raw-inputs.tsv]");
            if (args[0] == "original") Original(args[1], args[2], args.Length == 4 ? args[3] : null);
            else if (args[0] == "candidate") Candidate(args[1], args[2]);
            else throw new Exception("Unknown mode");
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
