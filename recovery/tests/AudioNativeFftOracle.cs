using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Collections.Generic;

// Executes the original mixed-mode DLL's native FFT on the real x86 CPU.
// Separate from the recovered C# and Unicorn implementations.
internal static class AudioNativeFftOracle
{
    private const string OriginalSha256 = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139";
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string module);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern uint GetModuleFileName(IntPtr module, System.Text.StringBuilder path, int length);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate void NativeFft(IntPtr instance, int order, int direction);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate uint ControlFp(uint value, uint mask);

    private static IntPtr At(IntPtr image, int rva)
    { return new IntPtr(unchecked(image.ToInt32() + rva)); }

    private static void Equal(string label, byte[] expected, IntPtr output)
    {
        byte[] actual = new byte[expected.Length];
        Marshal.Copy(output, actual, 0, actual.Length);
        for (int i = 0; i < actual.Length; ++i)
            if (expected[i] != actual[i])
                throw new Exception(label + " float=" + (i / 4) + " byte=" + i +
                    " expected=" + expected[i] + " actual=" + actual[i]);
    }

    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 2) throw new Exception("Usage: AudioNativeFftOracle original.dll original.tsv.gz");
            if (IntPtr.Size != 4) throw new Exception("Original native FFT requires an x86 host");
            string dll = Path.GetFullPath(args[0]);
            byte[] image = File.ReadAllBytes(dll);
            string sha;
            using (SHA256 algorithm = SHA256.Create())
                sha = BitConverter.ToString(algorithm.ComputeHash(image)).Replace("-", "").ToLowerInvariant();
            if (sha != OriginalSha256) throw new Exception("Original DLL fingerprint changed");
            int pe = BitConverter.ToInt32(image, 0x3c);
            if (BitConverter.ToUInt16(image, pe + 4) != 0x014c)
                throw new Exception("Original PE machine is not x86");
            Assembly loaded = Assembly.LoadFrom(dll);
            if (!String.Equals(Path.GetFullPath(loaded.Location), dll, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Managed load resolved a different original DLL");
            IntPtr module = GetModuleHandle(Path.GetFileName(dll));
            if (module == IntPtr.Zero) throw new Exception("Original native image is not loaded; error=" + Marshal.GetLastWin32Error());
            var path = new System.Text.StringBuilder(32768);
            if (GetModuleFileName(module, path, path.Capacity) == 0 ||
                !String.Equals(Path.GetFullPath(path.ToString()), dll, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Native module identity/path differs");
            NativeFft fft = (NativeFft)Marshal.GetDelegateForFunctionPointer(At(module, 0x4eca0), typeof(NativeFft));
            // Read this original PE's resolved _controlfp IAT slot. This uses
            // exactly its loaded MSVCR80 instance, avoiding CRT side-by-side
            // ambiguity and guessed external CRT search paths.
            IntPtr controlAddress = Marshal.ReadIntPtr(At(module, 0xab210));
            if (controlAddress == IntPtr.Zero) throw new Exception("Original _controlfp import is unresolved");
            ControlFp control = (ControlFp)Marshal.GetDelegateForFunctionPointer(controlAddress, typeof(ControlFp));
            uint before = control(0, 0);
            // MSVC _MCW_PC=0x30000, _PC_53=0x10000;
            // _MCW_RC=0x300, _RC_NEAR=0. Returned flags use CRT encoding,
            // not the physical x87 control-word layout 0x027f.
            uint configured = control(0x10000, 0x30300);
            if ((configured & 0x30300) != 0x10000)
                throw new Exception("Could not establish x87 53-bit nearest rounding");
            Console.WriteLine("Original FFT imageBase=0x" + module.ToInt32().ToString("x8") +
                " _controlfpBefore=0x" + before.ToString("x8") + " configured=0x" + configured.ToString("x8"));
            int count = 0;
            HashSet<string> coverage = new HashSet<string>();
            try
            {
                using (FileStream file = File.OpenRead(args[1]))
                using (GZipStream gzip = new GZipStream(file, CompressionMode.Decompress))
                using (StreamReader reader = new StreamReader(gzip))
                {
                    string line;
                    bool fingerprint = false;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("#"))
                        {
                            fingerprint = line.Contains("originalDllSha256=" + OriginalSha256) &&
                                line.Contains("fpuControl=0x027f") && line.Contains("nativeRva=0x4eca0");
                            continue;
                        }
                        if (!fingerprint) throw new Exception("Native FFT evidence fingerprint/FPU missing");
                        string[] f = line.Split('\t');
                        if (f.Length != 7) throw new Exception("Malformed native FFT fixture");
                        int order = Int32.Parse(f[1]), direction = Int32.Parse(f[2]);
                        if (order < 1 || order > 13 || (direction != 0 && direction != 1))
                            throw new Exception("Native FFT fixture outside audited bounds");
                        int size = (1 << order) * 4;
                        byte[] real = Convert.FromBase64String(f[3]), imaginary = Convert.FromBase64String(f[4]);
                        byte[] expectedReal = Convert.FromBase64String(f[5]), expectedImaginary = Convert.FromBase64String(f[6]);
                        if (real.Length != size || imaginary.Length != size ||
                            expectedReal.Length != size || expectedImaginary.Length != size)
                            throw new Exception("Native FFT fixture array size differs");
                        IntPtr realBuffer = Marshal.AllocHGlobal(size);
                        IntPtr imaginaryBuffer = Marshal.AllocHGlobal(size);
                        IntPtr instance = Marshal.AllocHGlobal(12);
                        try
                        {
                            Marshal.Copy(real, 0, realBuffer, size);
                            Marshal.Copy(imaginary, 0, imaginaryBuffer, size);
                            Marshal.WriteIntPtr(instance, 0, realBuffer);
                            Marshal.WriteIntPtr(instance, 4, imaginaryBuffer);
                            Marshal.WriteInt32(instance, 8, order);
                            uint active = control(0x10000, 0x30300);
                            if ((active & 0x30300) != 0x10000) throw new Exception("x87 rounding control changed");
                            fft(instance, order, direction);
                            string label = f[0] + " order=" + order + " direction=" + direction;
                            Equal(label + " real", expectedReal, realBuffer);
                            Equal(label + " imaginary", expectedImaginary, imaginaryBuffer);
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(instance);
                            Marshal.FreeHGlobal(imaginaryBuffer);
                            Marshal.FreeHGlobal(realBuffer);
                        }
                        if (!coverage.Add(f[0] + ":" + order + ":" + direction)) throw new Exception("Duplicate native FFT fixture");
                        ++count;
                    }
                }
            }
            finally { control(before, 0x30300); }
            for (int order = 1; order <= 13; ++order)
                for (int direction = 0; direction < 2; ++direction)
                    foreach (string pattern in new string[] { "impulse", "dc", "complex-noise" })
                        if (!coverage.Contains(pattern + ":" + order + ":" + direction))
                            throw new Exception("Incomplete native FFT coverage");
            if (count != 78) throw new Exception("Expected 78 original CPU fixtures; found " + count);
            Console.WriteLine("PASS real original x86 CPU FFT versus Unicorn: 78 cases, both directions, orders 1..13, exact float bits, original MSVCR80 controlfp 53-bit nearest");
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
