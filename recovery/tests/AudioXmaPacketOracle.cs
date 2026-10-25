using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Collections.Generic;

// Independent real original x86 CPU/candidate exact integer packet helper oracle.
// No XMA encoder is called and no AudioFile XMA support is implied.
internal static class AudioXmaPacketOracle
{
    private const string Sha = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139";
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr GetModuleHandle(string name);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] private static extern uint GetModuleFileName(IntPtr module, System.Text.StringBuilder path, int size);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int NativeCount(IntPtr destination, uint value);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void NativeBits(IntPtr value, IntPtr source, int offset, int count);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate byte NativeFrame(IntPtr source, int offset, IntPtr length);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int NativeScan(IntPtr source, int maximum, IntPtr destination, IntPtr block, IntPtr written);
    private static IntPtr At(IntPtr value, int offset) { return new IntPtr(unchecked(value.ToInt32() + offset)); }
    private static void Equal(string label, byte[] expected, byte[] actual)
    {
        if (expected.Length != actual.Length) throw new Exception(label + " length differs");
        for (int i = 0; i < actual.Length; ++i) if (actual[i] != expected[i]) throw new Exception(label + " byte=" + i + " expected=" + expected[i] + " actual=" + actual[i]);
    }
    private static byte[] Sentinel(int count) { byte[] result = new byte[count]; for (int i = 0; i < count; ++i) result[i] = 0xa5; return result; }
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
        public void CheckGuards()
        {
            byte[] before = new byte[32], after = new byte[32]; Marshal.Copy(allocation, before, 0, 32); Marshal.Copy(At(Pointer, size), after, 0, 32);
            Equal("native prefix guard", Sentinel(32), before); Equal("native suffix guard", Sentinel(32), after);
        }
        public void Dispose() { try { CheckGuards(); } finally { Marshal.FreeHGlobal(allocation); } }
    }
    private static IntPtr LoadOriginal(string path)
    {
        if (IntPtr.Size != 4) throw new Exception("Original packet helpers require an x86 host");
        path = Path.GetFullPath(path);
        using (SHA256 hash = SHA256.Create()) if (BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant() != Sha) throw new Exception("Original DLL fingerprint changed");
        Assembly assembly = Assembly.LoadFrom(path);
        if (!String.Equals(Path.GetFullPath(assembly.Location), path, StringComparison.OrdinalIgnoreCase)) throw new Exception("Original managed load path differs");
        IntPtr module = GetModuleHandle(Path.GetFileName(path)); var actual = new System.Text.StringBuilder(32768);
        if (module == IntPtr.Zero || GetModuleFileName(module, actual, actual.Capacity) == 0 || !String.Equals(Path.GetFullPath(actual.ToString()), path, StringComparison.OrdinalIgnoreCase)) throw new Exception("Original native module path differs");
        return module;
    }
    private static void Require(bool condition, string label) { if (!condition) throw new Exception(label); }
    private static void Reject(MethodInfo method, object[] args, Type expected)
    {
        try { method.Invoke(null, args); }
        catch (TargetInvocationException error) { if (error.InnerException != null && error.InnerException.GetType() == expected) return; throw; }
        throw new Exception("Missing safe rejection " + method.Name + " " + expected.Name);
    }
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 3 || (args[0] != "original" && args[0] != "candidate")) throw new Exception("Usage: AudioXmaPacketOracle original|candidate assembly.dll original.tsv.gz");
            bool original = args[0] == "original";
            NativeCount nativeCount = null; NativeBits nativeBits = null; NativeFrame nativeFrame = null; NativeScan nativeScan = null;
            MethodInfo countMethod = null, bitsMethod = null, frameMethod = null, scanMethod = null;
            if (original)
            {
                IntPtr module = LoadOriginal(args[1]);
                nativeCount = (NativeCount)Marshal.GetDelegateForFunctionPointer(At(module, 0xe890), typeof(NativeCount));
                nativeBits = (NativeBits)Marshal.GetDelegateForFunctionPointer(At(module, 0xea80), typeof(NativeBits));
                nativeFrame = (NativeFrame)Marshal.GetDelegateForFunctionPointer(At(module, 0xeae0), typeof(NativeFrame));
                nativeScan = (NativeScan)Marshal.GetDelegateForFunctionPointer(At(module, 0xef60), typeof(NativeScan));
            }
            else
            {
                Assembly assembly = Assembly.LoadFrom(Path.GetFullPath(args[1]));
                Type bits = assembly.GetType("BinaryAssetBuilder.AudioCompiler.XmaPacketBits", true), scan = assembly.GetType("BinaryAssetBuilder.AudioCompiler.XmaPacketScanner", true);
                countMethod = bits.GetMethod("WriteCount", new Type[] { typeof(uint) }); bitsMethod = bits.GetMethod("Accumulate"); frameMethod = scan.GetMethod("ReadFrame"); scanMethod = scan.GetMethod("Scan");
                Require(countMethod != null && bitsMethod != null && frameMethod != null && scanMethod != null, "Candidate packet API missing");
            }
            var coverage = new HashSet<string>(); var counts = new Dictionary<string, int>();
            using (FileStream file = File.OpenRead(args[2]))
            using (GZipStream gzip = new GZipStream(file, CompressionMode.Decompress))
            using (StreamReader reader = new StreamReader(gzip))
            {
                bool fingerprint = false; string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("#")) { fingerprint = line.Contains("originalDllSha256=" + Sha) && line.Contains("nativeRvas=0xe890,0xea80,0xeae0,0xef60") && line.Contains("realEaBlocks=8"); continue; }
                    Require(fingerprint, "Original packet evidence fingerprint missing");
                    string[] f = line.Split('\t'); Require(f.Length >= 3, "Malformed fixture");
                    string label = f[0] + ":" + f[1]; Require(coverage.Add(label), "Duplicate fixture " + label);
                    if (f[0] == "count")
                    {
                        Require(f.Length == 3, "Bad count fixture"); uint value = UInt32.Parse(f[1]); byte[] expected = Convert.FromBase64String(f[2]); byte[] actual;
                        if (original)
                        {
                            using (Guarded output = new Guarded(Sentinel(16)))
                            {
                                int written = nativeCount(output.Pointer, value); Require(written == expected.Length, label + " return length differs");
                                byte[] guarded = Sentinel(16); Array.Copy(expected, guarded, expected.Length); Equal(label + " output and untouched suffix", guarded, output.Data()); actual = expected;
                            }
                        }
                        else actual = (byte[])countMethod.Invoke(null, new object[] { value });
                        Equal(label, expected, actual);
                    }
                    else if (f[0] == "bits")
                    {
                        Require(f.Length == 7, "Bad bits fixture"); uint initial = UInt32.Parse(f[2]), expected = UInt32.Parse(f[6]); int offset = Int32.Parse(f[3]), count = Int32.Parse(f[4]); byte[] source = Convert.FromBase64String(f[5]); uint actual;
                        if (original)
                        {
                            using (Guarded input = new Guarded(source)) using (Guarded value = new Guarded(BitConverter.GetBytes(initial)))
                            { nativeBits(value.Pointer, input.Pointer, offset, count); actual = BitConverter.ToUInt32(value.Data(), 0); Equal(label + " source immutable", source, input.Data()); }
                        }
                        else { actual = (uint)bitsMethod.Invoke(null, new object[] { initial, source, offset, count }); Equal(label + " source immutable", Convert.FromBase64String(f[5]), source); }
                        Require(actual == expected, label + " accumulation differs");
                    }
                    else if (f[0] == "frame")
                    {
                        Require(f.Length == 6, "Bad frame fixture"); int offset = Int32.Parse(f[2]), expectedLength = Int32.Parse(f[4]); bool expectedFlag = f[5] == "1"; byte[] source = Convert.FromBase64String(f[3]); int actualLength; bool actualFlag;
                        if (original)
                        {
                            using (Guarded input = new Guarded(source)) using (Guarded length = new Guarded(Sentinel(4)))
                            { byte flag = nativeFrame(input.Pointer, offset, length.Pointer); Require(flag <= 1, "Bad native boolean"); actualFlag = flag != 0; actualLength = BitConverter.ToInt32(length.Data(), 0); Equal(label + " source immutable", source, input.Data()); }
                        }
                        else
                        { object[] invoke = new object[] { source, 0, offset, 0 }; actualFlag = (bool)frameMethod.Invoke(null, invoke); actualLength = (int)invoke[3]; Equal(label + " source immutable", Convert.FromBase64String(f[3]), source); }
                        Require(actualLength == expectedLength && actualFlag == expectedFlag, label + " length/continuation differs");
                    }
                    else if (f[0] == "scan")
                    {
                        Require(f.Length == 7, "Bad scan fixture"); int maximum = Int32.Parse(f[2]), expectedFrames = Int32.Parse(f[4]), expectedBlock = Int32.Parse(f[5]); byte[] source = Convert.FromBase64String(f[3]), expected = Convert.FromBase64String(f[6]);
                        int actualFrames, actualBlock, actualWritten; byte[] actualOutput;
                        if (original)
                        {
                            using (Guarded input = new Guarded(source)) using (Guarded output = new Guarded(Sentinel(4096))) using (Guarded block = new Guarded(Sentinel(4))) using (Guarded written = new Guarded(Sentinel(4)))
                            {
                                actualFrames = nativeScan(input.Pointer, maximum, output.Pointer, block.Pointer, written.Pointer); actualBlock = BitConverter.ToInt32(block.Data(), 0); actualWritten = BitConverter.ToInt32(written.Data(), 0);
                                actualOutput = output.Data(); Equal(label + " source immutable", source, input.Data());
                            }
                        }
                        else
                        {
                            byte[] output = Sentinel(4096 + 34); object[] invoke = new object[] { source, maximum, output, 17, 0, 0 };
                            actualFrames = (int)scanMethod.Invoke(null, invoke); actualBlock = (int)invoke[4]; actualWritten = (int)invoke[5];
                            byte[] prefix = new byte[17], suffix = new byte[17]; Array.Copy(output, prefix, 17); Array.Copy(output, output.Length - 17, suffix, 0, 17);
                            Equal(label + " managed prefix", Sentinel(17), prefix); Equal(label + " managed suffix", Sentinel(17), suffix);
                            actualOutput = new byte[4096]; Array.Copy(output, 17, actualOutput, 0, 4096); Equal(label + " source immutable", Convert.FromBase64String(f[3]), source);
                        }
                        Require(actualFrames == expectedFrames && actualBlock == expectedBlock && actualWritten == expected.Length, label + " return/out parameters differ");
                        byte[] complete = Sentinel(4096); Array.Copy(expected, complete, expected.Length); Equal(label + " output/untouched suffix", complete, actualOutput);
                    }
                    else throw new Exception("Unknown fixture group");
                    counts[f[0]] = counts.ContainsKey(f[0]) ? counts[f[0]] + 1 : 1;
                }
            }
            Require(counts.Count == 4 && counts["count"] == 143 && counts["bits"] == 680 && counts["frame"] == 104 && counts["scan"] == 98, "Incomplete packet matrix");
            foreach (int pattern in new int[] { 0, 1, 2, 3 })
                foreach (int offset in new int[] { 0, 1, 7, 8, 15, 16, 23, 24, 31, 63 })
                    foreach (int bits in new int[] { Int32.MinValue, -1, 0, 1, 7, 8, 15, 16, 23, 24, 25, 31, 32, 33, 63, 64, Int32.MaxValue })
                        Require(coverage.Contains("bits:" + pattern + "-" + offset + "-" + bits), "Missing accumulator fixture");
            foreach (int offset in new int[] { 0, 1, 7, 8, 0x3fd0, 0x3fd1, 0x3fd2, 0x3fdf })
                foreach (int length in new int[] { 0, 16, 31, 511, 16352, 32767 })
                    foreach (int continuation in new int[] { 0, 1 })
                        Require(coverage.Contains("frame:" + offset + "-" + length + "-" + continuation), "Missing frame fixture");
            foreach (int initial in new int[] { 0, 7, 0x3fd0, 0x3fdf })
                foreach (int count in new int[] { 1, 2, 7, 20 })
                    foreach (int flags in new int[] { 0, 3 })
                        foreach (int maximum in new int[] { 1, count, count + 1 })
                        {
                            int length = count == 1 ? 16 : count == 2 ? 31 : count == 7 ? 511 : 1024;
                            Require(coverage.Contains("scan:" + initial + "-" + count + "-" + length + "-" + flags + "-max" + maximum), "Missing scanner fixture");
                        }
            Require(coverage.Contains("scan:sentinel-max1") && coverage.Contains("scan:sentinel-max2"), "Missing sentinel fixture");
            foreach (uint boundary in new uint[] { 0xbf, 0xc0, 0xc1, 0x30bf, 0x30c0, 0x30c1, 0xc30bf, 0xc30c0, 0xc30c1, 0x30c30bf, 0x30c30c0, 0x30c30c1, 0xffffffff }) Require(coverage.Contains("count:" + boundary), "Missing count threshold");
            foreach (int channels in new int[] { 1, 2 }) foreach (int frames in new int[] { 257, 4096 }) foreach (string stream in new string[] { "False", "True" })
            { string name = "real-xma-evidence-" + channels + "-" + frames + "-" + stream; Require(coverage.Contains("frame:" + name) && coverage.Contains("scan:" + name), "Missing real EA block fixture"); }
            if (!original)
            {
                Reject(bitsMethod, new object[] { 0u, null, 0, 1 }, typeof(ArgumentNullException));
                Reject(bitsMethod, new object[] { 0u, new byte[3], -1, 1 }, typeof(ArgumentOutOfRangeException));
                Reject(bitsMethod, new object[] { 0u, new byte[2], 0, 1 }, typeof(ArgumentOutOfRangeException));
                Reject(frameMethod, new object[] { new byte[8192], 0, 0x3fe0, 0 }, typeof(ArgumentOutOfRangeException));
                Reject(scanMethod, new object[] { new byte[8], 0, new byte[32], 0, 0, 0 }, typeof(ArgumentOutOfRangeException));
                byte[] zeroFrame = new byte[2055]; zeroFrame[2] = 0x20; zeroFrame[3] = 0x10;
                Reject(scanMethod, new object[] { zeroFrame, 1, new byte[32], 0, 0, 0 }, typeof(ArgumentException));
            }
            Console.WriteLine("PASS " + args[0] + " XMA packet integer helpers: 1025 exact fixtures (143 count, 680 accumulator, 104 frame, 98 scan); 8 real EA blocks; all return/out parameters, source immutability and guards; no codec encoding acceptance" + (original ? "" : "; six safety rejections"));
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
