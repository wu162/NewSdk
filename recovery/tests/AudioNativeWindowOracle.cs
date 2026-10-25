using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

// Original-instruction CPU evidence, independent of candidate window arithmetic.
// The pinned DLL is loaded for its original constants and MSVCR80 controlfp.
internal static class AudioNativeWindowOracle
{
    private const string OriginalSha = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139";
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string name);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern uint GetModuleFileName(IntPtr module, StringBuilder path, int count);
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr VirtualAlloc(IntPtr address, UIntPtr size, uint allocation, uint protection);
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool VirtualProtect(IntPtr address, UIntPtr size, uint protection, out uint previous);
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool VirtualFree(IntPtr address, UIntPtr size, uint kind);
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FlushInstructionCache(IntPtr process, IntPtr address, UIntPtr size);
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentProcess();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate uint ControlFp(uint value, uint mask);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void Window(IntPtr output, int index, float support);

    private static IntPtr At(IntPtr module, int rva)
    { return new IntPtr(unchecked(module.ToInt32() + rva)); }
    private static string Hash(byte[] bytes)
    {
        using (SHA256 sha = SHA256.Create())
            return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
    }
    private static byte[] Read(IntPtr address, int count)
    { byte[] bytes = new byte[count]; Marshal.Copy(address, bytes, 0, count); return bytes; }
    private static void Expect(IntPtr module, int rva, byte[] expected)
    {
        byte[] actual = Read(At(module, rva), expected.Length);
        for (int i = 0; i < actual.Length; ++i)
            if (actual[i] != expected[i]) throw new Exception("Original instruction/constant differs at RVA " + rva.ToString("x"));
    }
    private static byte[] Extract(Window window, int support, IntPtr output)
    {
        byte[] result = new byte[support * 8];
        for (int i = 1; i <= support; ++i)
        {
            window(output, i, support);
            if ((ushort)Marshal.ReadInt16(output, 8) != 0x027f)
                throw new Exception("Physical x87 control word is not 0x027f");
            Marshal.Copy(output, result, (i - 1) * 8, 8);
        }
        return result;
    }
    private static int Main(string[] args)
    {
        IntPtr executable = IntPtr.Zero, output = IntPtr.Zero;
        try
        {
            if (args.Length != 3) throw new Exception("Usage: AudioNativeWindowOracle original.dll ResamplerCoefficients.cs actual-window.bin");
            if (IntPtr.Size != 4) throw new Exception("Original window evidence requires x86");
            string dll = Path.GetFullPath(args[0]);
            byte[] image = File.ReadAllBytes(dll);
            if (Hash(image) != OriginalSha) throw new Exception("Original DLL fingerprint changed");
            int pe = BitConverter.ToInt32(image, 0x3c);
            if (BitConverter.ToUInt16(image, pe + 4) != 0x014c) throw new Exception("Original DLL is not x86");
            Assembly loaded = Assembly.LoadFrom(dll);
            if (!String.Equals(Path.GetFullPath(loaded.Location), dll, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Managed load resolved a different original DLL");
            IntPtr module = GetModuleHandle(Path.GetFileName(dll));
            if (module == IntPtr.Zero) throw new Exception("Original image not loaded");
            StringBuilder modulePath = new StringBuilder(32768);
            if (GetModuleFileName(module, modulePath, modulePath.Capacity) == 0 ||
                !String.Equals(Path.GetFullPath(modulePath.ToString()), dll, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Native image identity/path differs");
            Expect(module, 0x41d14, new byte[] { 0xd8,0xcc,0x89,0x74,0x24,0x30,0xd8,0xf1,0xd9,0xff,0xd8,0xc3,0xd8,0xca });
            Expect(module, 0xb94dc, new byte[] { 0xdb,0x0f,0x49,0x40 });
            Expect(module, 0xac888, new byte[] { 0,0,0,0x3f });
            ControlFp control = (ControlFp)Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module, 0xab210)), typeof(ControlFp));
            // Build an isolated executable thunk with the original stack setup:
            // FLD pi, FLD1, FLD half, FLD support, FILD index. Then copy the
            // exact original FMUL st4 / FDIV st1 / FCOS / FADD st3 / FMUL st2.
            // Omit only MOV [esp+30],esi, a loop-index bookkeeping store.
            List<byte> code = new List<byte>();
            code.AddRange(new byte[] { 0xd9,0x05 }); code.AddRange(BitConverter.GetBytes(At(module, 0xb94dc).ToInt32()));
            code.AddRange(new byte[] { 0xd9,0xe8,0xd9,0x05 }); code.AddRange(BitConverter.GetBytes(At(module, 0xac888).ToInt32()));
            code.AddRange(new byte[] { 0xd9,0x44,0x24,0x0c,0xdb,0x44,0x24,0x08 });
            code.AddRange(Read(At(module, 0x41d14), 2));
            code.AddRange(Read(At(module, 0x41d1a), 8));
            // Store binary64 weight and physical FNSTCW, pop four setup operands.
            code.AddRange(new byte[] { 0x8b,0x44,0x24,0x04,0xdd,0x18,0xd9,0x78,0x08,
                0xdd,0xd8,0xdd,0xd8,0xdd,0xd8,0xdd,0xd8,0xc3 });
            byte[] thunk = code.ToArray();
            executable = VirtualAlloc(IntPtr.Zero, new UIntPtr((uint)thunk.Length), 0x3000, 0x04);
            if (executable == IntPtr.Zero) throw new Exception("Window thunk allocation failed");
            Marshal.Copy(thunk, 0, executable, thunk.Length);
            uint previous;
            if (!VirtualProtect(executable, new UIntPtr((uint)thunk.Length), 0x20, out previous) ||
                !FlushInstructionCache(GetCurrentProcess(), executable, new UIntPtr((uint)thunk.Length)))
                throw new Exception("Window thunk executable protection failed");
            Window window = (Window)Marshal.GetDelegateForFunctionPointer(executable, typeof(Window));
            output = Marshal.AllocHGlobal(16);
            uint before = control(0, 0);
            byte[] support256, support512;
            try
            {
                uint configured = control(0x10000, 0x30300);
                if ((configured & 0x30300) != 0x10000) throw new Exception("Cannot establish 53-bit nearest rounding");
                support256 = Extract(window, 256, output);
                support512 = Extract(window, 512, output);
            }
            finally { control(before, 0x30300); }
            // Save actual CPU bytes before any candidate comparison can fail.
            File.WriteAllBytes(args[2], support512);
            for (int i = 0; i < 256; ++i)
                for (int b = 0; b < 8; ++b)
                    if (support256[i * 8 + b] != support512[(i * 2 + 1) * 8 + b])
                        throw new Exception("CPU support256 differs from support512 even-index constants");
            string source = File.ReadAllText(args[1]);
            Match declaration = Regex.Match(source, @"WindowBits\s*=\s*Convert\.FromBase64String\((.*?)\);", RegexOptions.Singleline);
            if (!declaration.Success) throw new Exception("Candidate WindowBits declaration missing");
            StringBuilder encoded = new StringBuilder();
            foreach (Match literal in Regex.Matches(declaration.Groups[1].Value, "\"([A-Za-z0-9+/=]+)\"")) encoded.Append(literal.Groups[1].Value);
            byte[] candidate = Convert.FromBase64String(encoded.ToString());
            if (candidate.Length != support512.Length) throw new Exception("Candidate window length differs");
            int different = 0;
            StringBuilder lines = new StringBuilder();
            lines.AppendLine("# originalDllSha256=" + OriginalSha + "; nativeRva=0x41d14..0x41d22; fpuControl=0x027f; backend=real-x86-original-opcodes; binary64Sha256=" + Hash(support512));
            for (int i = 0; i < 512; ++i)
            {
                ulong originalBits = BitConverter.ToUInt64(support512, i * 8), candidateBits = BitConverter.ToUInt64(candidate, i * 8);
                lines.AppendLine((i + 1) + "\t" + originalBits.ToString("x16") + "\t" + candidateBits.ToString("x16"));
                if (originalBits != candidateBits)
                {
                    if (different < 8) Console.WriteLine("Window difference index=" + (i + 1) + " CPU=" + originalBits.ToString("x16") + " source=" + candidateBits.ToString("x16"));
                    ++different;
                }
            }
            File.WriteAllText(args[2] + ".tsv", lines.ToString(), new UTF8Encoding(false));
            Console.WriteLine("Original CPU window: 512 binary64 values; differences=" + different + "; sha256=" + Hash(support512));
            if (different != 0) throw new Exception("Original CPU windows differ from embedded candidate values; evidence saved");
            Console.WriteLine("PASS real original x86 window: 512 binary64 constants, support256 even-index identity, physical FPCW=0x027f");
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
        finally
        {
            if (output != IntPtr.Zero) Marshal.FreeHGlobal(output);
            if (executable != IntPtr.Zero) VirtualFree(executable, UIntPtr.Zero, 0x8000);
        }
    }
}
