using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Collections.Generic;

// candidate mode: recovered managed implementation, no original DLL loaded.
// original mode: independent real Windows x86 CPU cross-check of the native oracle.
internal static class AudioResamplerKernelOracle
{
    private const int N = 8192;
    private static bool Collect;
    private static StreamWriter Collector;
    private static int EmulatorComparisonDifferences;
    private static long EmulatorComparisonDifferingBytes;
    private const string Sha = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139";
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string module);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern uint GetModuleFileName(IntPtr module, System.Text.StringBuilder path, int length);
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr VirtualAlloc(IntPtr address, UIntPtr size, uint allocation, uint protection);
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool VirtualFree(IntPtr address, UIntPtr size, uint freeType);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void Thunk();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint ControlFp(uint value, uint mask);
    private static IntPtr At(IntPtr p, int offset) { return new IntPtr(unchecked(p.ToInt32() + offset)); }
    private static byte[] Bytes(float[] values) { byte[] result = new byte[values.Length * 4]; System.Buffer.BlockCopy(values, 0, result, 0, result.Length); return result; }
    private static float[] Floats(byte[] values) { if (values.Length != N * 4) throw new Exception("Expected 8192 floats"); float[] result = new float[N]; System.Buffer.BlockCopy(values, 0, result, 0, values.Length); return result; }
    private static float[] Sentinel() { float[] result = new float[N]; for (int i = 0; i < N; ++i) result[i] = -123.5f; return result; }
    private static void Equal(string label, byte[] expected, byte[] actual)
    {
        if (expected.Length != actual.Length) throw new Exception(label + " length differs");
        int first = -1; long differingBytes = 0;
        for (int i = 0; i < expected.Length; ++i)
            if (expected[i] != actual[i]) { if (first < 0) first = i; ++differingBytes; }
        if (first >= 0)
        {
            if (!Collect) throw new Exception(label + " float=" + (first / 4) + " byte=" + first + " expected=" + expected[first] + " actual=" + actual[first]);
            ++EmulatorComparisonDifferences; EmulatorComparisonDifferingBytes += differingBytes;
            if (EmulatorComparisonDifferences <= 20) Console.WriteLine("EMULATOR_DIFFERENCE " + label + " buffers=" + EmulatorComparisonDifferences + " firstByte=" + first + " differingBytes=" + differingBytes);
        }
    }
    private static void Emit(List<byte> code, byte opcode, int value)
    { code.Add(opcode); code.AddRange(BitConverter.GetBytes(value)); }
    private static void InvokeNative(IntPtr module, int rva, int[] arguments, int eax, int ebx, int edx, int esi, int edi, int ecx)
    {
        List<byte> code = new List<byte>();
        // Preserve all nonvolatile registers required by the managed interop caller.
        code.AddRange(new byte[] { 0x53, 0x55, 0x56, 0x57 });
        for (int i = arguments.Length - 1; i >= 0; --i) Emit(code, 0x68, arguments[i]);
        Emit(code, 0xb8, eax); Emit(code, 0xbb, ebx); Emit(code, 0xba, edx); Emit(code, 0xbe, esi); Emit(code, 0xbf, edi);
        Emit(code, 0xb9, ecx); Emit(code, 0xbd, At(module, rva).ToInt32()); code.AddRange(new byte[] { 0xff, 0xd5 }); // call ebp; ecx may carry profile
        code.AddRange(new byte[] { 0x83, 0xc4, checked((byte)(arguments.Length * 4)), 0x5f, 0x5e, 0x5d, 0x5b, 0xc3 });
        byte[] raw = code.ToArray();
        IntPtr executable = VirtualAlloc(IntPtr.Zero, new UIntPtr((uint)raw.Length), 0x3000, 0x40);
        if (executable == IntPtr.Zero) throw new Exception("VirtualAlloc thunk failed " + Marshal.GetLastWin32Error());
        try { Marshal.Copy(raw, 0, executable, raw.Length); ((Thunk)Marshal.GetDelegateForFunctionPointer(executable, typeof(Thunk)))(); }
        finally { if (!VirtualFree(executable, UIntPtr.Zero, 0x8000)) throw new Exception("VirtualFree thunk failed"); }
    }
    private static IntPtr Buffer(byte[] value) { IntPtr p = Marshal.AllocHGlobal(value.Length); Marshal.Copy(value, 0, p, value.Length); return p; }
    private static byte[] Read(IntPtr p, int size) { byte[] value = new byte[size]; Marshal.Copy(p, value, 0, size); return value; }
    private static IntPtr LoadOriginal(string dll)
    {
        if (IntPtr.Size != 4) throw new Exception("Original native oracle requires x86");
        string path = Path.GetFullPath(dll);
        using (SHA256 algorithm = SHA256.Create())
            if (BitConverter.ToString(algorithm.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant() != Sha)
                throw new Exception("Original DLL fingerprint differs");
        Assembly assembly = Assembly.LoadFrom(path);
        if (!String.Equals(Path.GetFullPath(assembly.Location), path, StringComparison.OrdinalIgnoreCase)) throw new Exception("Original managed load path differs");
        IntPtr module = GetModuleHandle(Path.GetFileName(path));
        var nativePath = new System.Text.StringBuilder(32768);
        if (module == IntPtr.Zero || GetModuleFileName(module, nativePath, nativePath.Capacity) == 0 ||
            !String.Equals(Path.GetFullPath(nativePath.ToString()), path, StringComparison.OrdinalIgnoreCase)) throw new Exception("Original native module path differs");
        return module;
    }
    private static void CheckSafety(MethodInfo fill, MethodInfo process, MethodInfo derivatives)
    {
        Reject(fill, new object[] { null, 0, 1, new float[N] }, typeof(ArgumentNullException));
        Reject(fill, new object[] { new short[0], 0, 0, new float[N] }, typeof(ArgumentOutOfRangeException));
        Reject(fill, new object[] { new short[0], 0, 1, new float[1] }, typeof(ArgumentException));
        Reject(process, new object[] { new float[N], new float[N], new float[N], 3 }, typeof(ArgumentOutOfRangeException));
        Reject(derivatives, new object[] { new float[N], 0, new float[N] }, typeof(ArgumentOutOfRangeException));
    }
    private static void Reject(MethodInfo method, object[] args, Type expected)
    {
        try { method.Invoke(null, args); }
        catch (TargetInvocationException error) { if (error.InnerException != null && error.InnerException.GetType() == expected) return; throw; }
        throw new Exception("Missing safety rejection " + method.Name + " " + expected.FullName);
    }
    private static float[] ProfileFloats(byte[] raw, int offset, int count)
    { float[] result = new float[count]; System.Buffer.BlockCopy(raw, offset, result, 0, count * 4); return result; }
    private static void CheckKernelFixture(string[] f, bool original, IntPtr module, MethodInfo coefficientsMethod, MethodInfo kernelMethod, HashSet<string> coverage)
    {
        if ((f[0] == "coefficients" && f.Length != 4) || (f[0] == "kernel" && f.Length != 5)) throw new Exception("Malformed kernel fixture");
        if (!coverage.Add(f[0] + ":" + f[1])) throw new Exception("Duplicate kernel fixture");
        byte[] profile = Convert.FromBase64String(f[2]);
        if (profile.Length != 0x338 || BitConverter.ToInt32(profile, 0) != 256) throw new Exception("Bad profile fixture");
        int factor = BitConverter.ToInt32(profile, 8), count = BitConverter.ToInt32(profile, 0x14);
        if ((factor != 1 && factor != 2) || count < 2 || count > 4) throw new Exception("Bad profile factor/count");
        float[] frequencies = ProfileFloats(profile, 0x18, count), amplitudes = ProfileFloats(profile, 0x1a8, count);
        float gain = BitConverter.ToSingle(profile, 0x10), phaseRate = BitConverter.ToSingle(profile, 0xc);
        bool invert = BitConverter.ToInt32(profile, 4) != 0;
        byte[] actual;
        if (f[0] == "coefficients")
        {
            if (original)
            {
                IntPtr p = Buffer(profile), real = Buffer(new byte[N * 4]), imaginary = Buffer(new byte[N * 4]), output = Buffer(new byte[N * 4]), fft = Marshal.AllocHGlobal(12);
                try
                {
                    Marshal.WriteIntPtr(fft, 0, real); Marshal.WriteIntPtr(fft, 4, imaginary); Marshal.WriteInt32(fft, 8, 13);
                    InvokeNative(module, 0x41a90, new int[] { p.ToInt32(), fft.ToInt32(), output.ToInt32() }, 0, 0, 0, 0, 0, 0);
                    actual = Read(output, N * 4);
                }
                finally { Marshal.FreeHGlobal(fft); Marshal.FreeHGlobal(output); Marshal.FreeHGlobal(imaginary); Marshal.FreeHGlobal(real); Marshal.FreeHGlobal(p); }
            }
            else actual = Bytes((float[])coefficientsMethod.Invoke(null, new object[] { factor, frequencies, amplitudes, gain, invert }));
            Equal(f[0] + ":" + f[1], Convert.FromBase64String(f[3]), actual);
            if (Collect) f[3] = Convert.ToBase64String(actual);
        }
        else
        {
            byte[] source = Convert.FromBase64String(f[3]), expected = Convert.FromBase64String(f[4]);
            if (source.Length % 2 != 0 || expected.Length % 2 != 0) throw new Exception("Bad kernel sample lengths");
            if (original)
            {
                IntPtr input = Buffer(source.Length == 0 ? new byte[2] : source), p = Buffer(profile);
                byte[] guarded = new byte[expected.Length + 32]; for (int i = expected.Length; i < guarded.Length; ++i) guarded[i] = 0x5a;
                IntPtr output = Buffer(guarded);
                try
                {
                    InvokeNative(module, 0x42480, new int[] { input.ToInt32(), source.Length / 2, output.ToInt32(), 0 }, 1, 0, 0, 0, 0, p.ToInt32());
                    actual = Read(output, expected.Length);
                    byte[] after = Read(output, guarded.Length);
                    for (int i = expected.Length; i < after.Length; ++i) if (after[i] != 0x5a) throw new Exception("Native kernel overran output");
                }
                finally { Marshal.FreeHGlobal(output); Marshal.FreeHGlobal(p); Marshal.FreeHGlobal(input); }
            }
            else
            {
                short[] samples = new short[source.Length / 2]; System.Buffer.BlockCopy(source, 0, samples, 0, source.Length);
                float[] coefficients = (float[])coefficientsMethod.Invoke(null, new object[] { factor, frequencies, amplitudes, gain, invert });
                short[] result = (short[])kernelMethod.Invoke(null, new object[] { samples, coefficients, factor, phaseRate });
                actual = new byte[result.Length * 2]; System.Buffer.BlockCopy(result, 0, actual, 0, actual.Length);
            }
            Equal(f[0] + ":" + f[1], expected, actual);
            if (Collect) f[4] = Convert.ToBase64String(actual);
        }
    }
    private static void CheckCoverage(HashSet<string> coverage)
    {
        for (int i = 0; i < 18; ++i) if (!coverage.Contains("coefficients:" + i)) throw new Exception("Missing coefficient fixture");
        foreach (string rate in new string[] { "44100-48000", "48000-44100", "44100-22050", "48000-16000" })
            foreach (int length in new int[] { 0, 1, 257, 4096, 8193 })
                foreach (string pattern in new string[] { "impulse", "dc", "extrema", "noise" })
                    if (!coverage.Contains("kernel:" + rate + "-" + length + "-" + pattern)) throw new Exception("Missing whole kernel fixture");
        foreach (string pattern in new string[] { "impulse", "dc", "extrema", "noise" })
            foreach (int factor in new int[] { 1, 2 })
                foreach (int position in new int[] { -513, -1, 0, 4095, 8191, 9000 })
                    if (!coverage.Contains("source:" + pattern + ":" + factor + ":" + position)) throw new Exception("Missing source fixture");
        foreach (int factor in new int[] { 1, 2 })
            foreach (string pattern in new string[] { "impulse", "dc", "alternating", "noise" })
                foreach (string coefficients in new string[] { "identity", "signed-noise" })
                {
                    string fixture = pattern + "-" + coefficients;
                    if (!coverage.Contains("process:" + fixture + ":" + factor + ":0")) throw new Exception("Missing filter fixture");
                    foreach (int support in new int[] { 1, 256, 512 })
                        foreach (string channel in new string[] { "real", "imaginary" })
                            if (!coverage.Contains("derivatives:" + fixture + "-" + channel + "-f" + factor + ":0:" + support)) throw new Exception("Missing derivative fixture");
                }
    }
    private static int Main(string[] args) { return Run("candidate", args); }
    internal static int Run(string mode, string[] args)
    {
        string authoritativePath = null, partialPath = null;
        Collect = false; Collector = null; EmulatorComparisonDifferences = 0; EmulatorComparisonDifferingBytes = 0;
        try
        {
            bool original = mode == "original";
            if (args.Length != 2 && !(original && args.Length == 3)) throw new Exception("Usage: AudioResamplerKernelOracle assembly.dll original.tsv.gz [authoritative.tsv.gz (original CPU only)]");
            Collect = original && args.Length == 3;
            if (Collect)
            {
                authoritativePath = Path.GetFullPath(args[2]); partialPath = authoritativePath + ".partial";
                if (File.Exists(authoritativePath) || File.Exists(partialPath)) throw new Exception("Refusing to overwrite authoritative CPU evidence");
                Collector = new StreamWriter(new GZipStream(new FileStream(partialPath, FileMode.CreateNew, FileAccess.Write), CompressionMode.Compress));
            }
            IntPtr module = IntPtr.Zero;
            ControlFp control = null; uint before = 0;
            MethodInfo fill = null, process = null, derivatives = null, coefficientsMethod = null, kernelMethod = null;
            if (original)
            {
                module = LoadOriginal(args[0]);
                control = (ControlFp)Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module, 0xab210)), typeof(ControlFp));
                before = control(0, 0);
            }
            else
            {
                Assembly candidate = Assembly.LoadFrom(Path.GetFullPath(args[0]));
                Type type = candidate.GetType("BinaryAssetBuilder.AudioCompiler.ResamplerFilter", true);
                fill = type.GetMethod("FillSource", BindingFlags.Public | BindingFlags.Static);
                process = type.GetMethod("Process", BindingFlags.Public | BindingFlags.Static);
                derivatives = type.GetMethod("Derivatives", BindingFlags.Public | BindingFlags.Static);
                if (fill == null || process == null || derivatives == null) throw new Exception("Recovered filter API missing");
                coefficientsMethod = candidate.GetType("BinaryAssetBuilder.AudioCompiler.ResamplerCoefficients", true).GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
                kernelMethod = candidate.GetType("BinaryAssetBuilder.AudioCompiler.ResamplerScheduler", true).GetMethod("Process", BindingFlags.Public | BindingFlags.Static);
                if (coefficientsMethod == null || kernelMethod == null) throw new Exception("Recovered coefficient/scheduler API missing");
            }
            Dictionary<string, int> counts = new Dictionary<string, int>();
            HashSet<string> coverage = new HashSet<string>();
            try
            {
                using (FileStream file = File.OpenRead(args[1]))
                using (GZipStream gzip = new GZipStream(file, CompressionMode.Decompress))
                using (StreamReader reader = new StreamReader(gzip))
                {
                    bool fingerprint = false; string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("#"))
                        {
                            fingerprint = line.Contains("originalDllSha256=" + Sha) && line.Contains("fpuControl=0x027f") && line.Contains("nativeRvas=0x41f40,0x41fa0,0x42250,0x41a90,0x42480");
                            if (!original && !line.Contains("authoritativeOracle=real-windows-x86-cpu")) throw new Exception("Candidate must consume authoritative real CPU evidence");
                            if (Collect) Collector.WriteLine(line + "; authoritativeOracle=real-windows-x86-cpu");
                            continue;
                        }
                        if (!fingerprint) throw new Exception("Evidence fingerprint/FPU missing");
                        if (original && (control(0x10000, 0x30300) & 0x30300) != 0x10000) throw new Exception("Could not set 53-bit nearest rounding");
                        string[] f = line.Split('\t');
                        if (f[0] == "coefficients" || f[0] == "kernel")
                        {
                            CheckKernelFixture(f, original, module, coefficientsMethod, kernelMethod, coverage);
                            if (Collect) Collector.WriteLine(String.Join("\t", f));
                            counts[f[0]] = counts.ContainsKey(f[0]) ? counts[f[0]] + 1 : 1;
                            continue;
                        }
                        if (f.Length < 6) throw new Exception("Malformed fixture");
                        string label = String.Join(":", f, 0, 4);
                        if (!coverage.Add(label)) throw new Exception("Duplicate fixture " + label);
                        int factor = Int32.Parse(f[2]), position = Int32.Parse(f[3]);
                        byte[] a = Convert.FromBase64String(f[4]);
                        byte[] actual;
                        if (f[0] == "source")
                        {
                            if (f.Length != 6 || (factor != 1 && factor != 2) || a.Length % 2 != 0) throw new Exception("Bad source fixture");
                            if (original)
                            {
                                IntPtr input = Buffer(a), output = Buffer(Bytes(Sentinel()));
                                try { InvokeNative(module, 0x41f40, new int[] { input.ToInt32(), a.Length / 2 }, position, output.ToInt32(), 0, 0, factor, 0); actual = Read(output, N * 4); }
                                finally { Marshal.FreeHGlobal(output); Marshal.FreeHGlobal(input); }
                            }
                            else
                            {
                                short[] source = new short[a.Length / 2]; System.Buffer.BlockCopy(a, 0, source, 0, a.Length); float[] output = Sentinel();
                                fill.Invoke(null, new object[] { source, position, factor, output }); actual = Bytes(output);
                            }
                            Equal(label, Convert.FromBase64String(f[5]), actual);
                            if (Collect) f[5] = Convert.ToBase64String(actual);
                        }
                        else if (f[0] == "process")
                        {
                            if (f.Length != 9 || (factor != 1 && factor != 2)) throw new Exception("Bad process fixture");
                            byte[] b = Convert.FromBase64String(f[5]), c = Convert.FromBase64String(f[6]);
                            if (a.Length != N * 4 || b.Length != N * 4 || c.Length != N * 4) throw new Exception("Bad process array lengths");
                            byte[] actualImag;
                            if (original)
                            {
                                IntPtr real = Buffer(a), imaginary = Buffer(b), coeff = Buffer(c), fft = Marshal.AllocHGlobal(12);
                                try
                                {
                                    Marshal.WriteIntPtr(fft, 0, real); Marshal.WriteIntPtr(fft, 4, imaginary); Marshal.WriteInt32(fft, 8, 13);
                                    InvokeNative(module, 0x41fa0, new int[] { coeff.ToInt32() }, factor, 0, 0, fft.ToInt32(), 0, 0);
                                    actual = Read(real, N * 4); actualImag = Read(imaginary, N * 4);
                                }
                                finally { Marshal.FreeHGlobal(fft); Marshal.FreeHGlobal(coeff); Marshal.FreeHGlobal(imaginary); Marshal.FreeHGlobal(real); }
                            }
                            else
                            {
                                float[] real = Floats(a), imaginary = Floats(b), coeff = Floats(c);
                                process.Invoke(null, new object[] { real, imaginary, coeff, factor }); actual = Bytes(real); actualImag = Bytes(imaginary);
                            }
                            Equal(label + " real", Convert.FromBase64String(f[7]), actual); Equal(label + " imaginary", Convert.FromBase64String(f[8]), actualImag);
                            if (Collect) { f[7] = Convert.ToBase64String(actual); f[8] = Convert.ToBase64String(actualImag); }
                        }
                        else if (f[0] == "derivatives")
                        {
                            if (f.Length != 6 || a.Length != N * 4 || (position != 1 && position != 256 && position != 512)) throw new Exception("Bad derivative fixture");
                            if (original)
                            {
                                IntPtr input = Buffer(a), output = Buffer(Bytes(Sentinel()));
                                try { InvokeNative(module, 0x42250, new int[] { input.ToInt32(), output.ToInt32() }, 0, 0, position, 0, 0, 0); actual = Read(output, N * 4); }
                                finally { Marshal.FreeHGlobal(output); Marshal.FreeHGlobal(input); }
                            }
                            else { float[] output = Sentinel(); derivatives.Invoke(null, new object[] { Floats(a), position, output }); actual = Bytes(output); }
                            Equal(label, Convert.FromBase64String(f[5]), actual);
                            if (Collect) f[5] = Convert.ToBase64String(actual);
                        }
                        else throw new Exception("Unknown fixture type");
                        if (Collect) Collector.WriteLine(String.Join("\t", f));
                        counts[f[0]] = counts.ContainsKey(f[0]) ? counts[f[0]] + 1 : 1;
                    }
                }
                if (counts.Count != 5 || counts["source"] != 48 || counts["process"] != 16 || counts["derivatives"] != 96 || counts["coefficients"] != 18 || counts["kernel"] != 80) throw new Exception("Incomplete filter matrix");
                CheckCoverage(coverage);
                if (!original) CheckSafety(fill, process, derivatives);
            }
            finally { if (control != null) control(before, 0x30300); }
            if (Collect)
            {
                Collector.Dispose(); Collector = null;
                File.Move(partialPath, authoritativePath);
                Console.WriteLine("emulatorComparisonDifferences=" + EmulatorComparisonDifferences + " emulatorComparisonDifferingBytes=" + EmulatorComparisonDifferingBytes);
                Console.WriteLine("AUTHORITATIVE_REAL_CPU " + authoritativePath + " fixtures=258 fpuControl=0x027f");
            }
            Console.WriteLine((Collect ? "PASS original CPU oracle collection (emulator differences reported separately): " : "PASS " + mode + " native resampler bit differential: ") + "48 source, 16 complex filter, 96 derivative, 18 coefficient, 80 whole kernel fixtures; " + (original ? "real original x86 CPU; 53-bit nearest" : "five safety rejections; original DLL not loaded"));
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
        finally { if (Collector != null) { Collector.Dispose(); Collector = null; } }
    }
}
