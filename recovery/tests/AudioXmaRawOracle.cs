using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

// Original-only native codec evidence. This never loads a recovered XMA encoder.
// ABI is the pinned PE's f234..f275 -> 589d0 (ret 2c), with the six output
// slots mapped through 58a0a..58a4d. Input conversion uses original e9b0.
internal static class AudioXmaRawOracle
{
    const string Sha = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139";
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] static extern IntPtr GetModuleHandle(string name);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] static extern uint GetModuleFileName(IntPtr module, StringBuilder path, int count);
    [DllImport("kernel32.dll")] static extern IntPtr VirtualAlloc(IntPtr address, UIntPtr size, uint allocation, uint protection);
    [DllImport("kernel32.dll")] static extern bool VirtualProtect(IntPtr address, UIntPtr size, uint protection, out uint previous);
    [DllImport("kernel32.dll")] static extern bool VirtualFree(IntPtr address, UIntPtr size, uint kind);
    [DllImport("kernel32.dll")] static extern bool FlushInstructionCache(IntPtr process, IntPtr address, UIntPtr size);
    [DllImport("kernel32.dll")] static extern IntPtr GetCurrentProcess();
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate int Encode(int streams, IntPtr descriptors, int quality, int flags, int blockKiB, IntPtr rawPointer, IntPtr rawLength, IntPtr allocation2Pointer, IntPtr allocation2Length, IntPtr allocation3Pointer, IntPtr allocation3Length);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate void ConvertPcm(IntPtr destination, IntPtr source, int channels, int stride, int frames);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void Free(IntPtr memory);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate uint ControlFp(uint value, uint mask);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void ReadFpu(IntPtr output);
    static IntPtr At(IntPtr p, int n) { return new IntPtr(unchecked(p.ToInt32() + n)); }
    static void Need(bool value, string text) { if (!value) throw new Exception(text); }
    static byte[] Read(IntPtr p, int n) { byte[] b = new byte[n]; if (n != 0) Marshal.Copy(p, b, 0, n); return b; }
    static string Hash(byte[] bytes) { using (SHA256 s = SHA256.Create()) return BitConverter.ToString(s.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant(); }
    static string Hex(IntPtr p) { return "0x" + unchecked((uint)p.ToInt32()).ToString("x8"); }
    static void Equal(string name, byte[] a, byte[] b) { Need(a.Length == b.Length, name + " length differs"); for (int i = 0; i < a.Length; i++) Need(a[i] == b[i], name + " byte differs at " + i); }
    static byte[] Fill(int n) { byte[] b = new byte[n]; for (int i = 0; i < n; i++) b[i] = 0xa5; return b; }
    sealed class Guard : IDisposable
    {
        IntPtr allocation; readonly int length; public readonly IntPtr Pointer;
        public Guard(byte[] bytes) { length = bytes.Length; allocation = Marshal.AllocHGlobal(length + 64); Pointer = At(allocation, 32); Marshal.Copy(Fill(length + 64), 0, allocation, length + 64); Marshal.Copy(bytes, 0, Pointer, length); }
        public byte[] Bytes() { return Read(Pointer, length); }
        public void Dispose() { try { Equal("prefix guard", Fill(32), Read(allocation, 32)); Equal("suffix guard", Fill(32), Read(At(Pointer, length), 32)); } finally { Marshal.FreeHGlobal(allocation); } }
    }
    static void Json(string path, object value) { File.WriteAllText(path, new JavaScriptSerializer().Serialize(value), new UTF8Encoding(false)); }
    static object Save(string directory, string name, byte[] bytes) { File.WriteAllBytes(Path.Combine(directory, name), bytes); return new { path = name, length = bytes.Length, sha256 = Hash(bytes) }; }
    static Dictionary<string, object> Fpu(ControlFp control, ReadFpu reader)
    {
        using (Guard b = new Guard(new byte[8])) { reader(b.Pointer); byte[] value = b.Bytes(); return new Dictionary<string, object> { { "crtControl", "0x" + control(0, 0).ToString("x8") }, { "x87Control", "0x" + BitConverter.ToUInt16(value, 0).ToString("x4") }, { "mxcsr", "0x" + BitConverter.ToUInt32(value, 4).ToString("x8") } }; }
    }
    static IntPtr Load(string path)
    {
        Need(IntPtr.Size == 4, "Original native codec requires x86"); path = Path.GetFullPath(path); byte[] pe = File.ReadAllBytes(path);
        Need(Hash(pe) == Sha, "Original DLL fingerprint changed"); Need(BitConverter.ToUInt16(pe, BitConverter.ToInt32(pe, 0x3c) + 4) == 0x14c, "Original PE is not x86");
        Assembly assembly = Assembly.LoadFrom(path); Need(String.Equals(Path.GetFullPath(assembly.Location), path, StringComparison.OrdinalIgnoreCase), "Managed load path differs");
        IntPtr module = GetModuleHandle(Path.GetFileName(path)); StringBuilder found = new StringBuilder(32768);
        Need(module != IntPtr.Zero && GetModuleFileName(module, found, found.Capacity) != 0 && String.Equals(Path.GetFullPath(found.ToString()), path, StringComparison.OrdinalIgnoreCase), "Native module identity/path differs"); return module;
    }
    sealed class Result
    {
        public byte[][] Outputs;
        public byte[] Descriptor;
        public byte[] Pcm;
        public int Hresult;
        public object Evidence;
    }
    static Result Run(IntPtr module, Encode encode, ConvertPcm convert, Free free, ControlFp control, ReadFpu reader, byte[] floatBytes, int channels, int frames, int rate, int blockBytes)
    {
        using (Guard input = new Guard(floatBytes)) using (Guard pcm = new Guard(new byte[frames * channels * 2])) using (Guard descriptor = new Guard(new byte[0x3c])) using (Guard outs = new Guard(new byte[24]))
        {
            uint active = control(0x10000, 0x30300); Need((active & 0x30300) == 0x10000, "Could not establish x87 53-bit nearest");
            Dictionary<string, object> before = Fpu(control, reader); Need((string)before["x87Control"] == "0x027f", "Unexpected physical x87 control word");
            convert(pcm.Pointer, input.Pointer, channels, channels, frames); Equal("float source immutable", floatBytes, input.Bytes());
            byte[] signed16 = pcm.Bytes();
            // Original f150..f1e5 writes the same fixed fields even for stereo
            // (in particular dwChannelMask=4, not a guessed stereo mask=3).
            Marshal.WriteInt16(descriptor.Pointer, 0, 1); Marshal.WriteInt16(descriptor.Pointer, 2, (short)channels);
            Marshal.WriteInt32(descriptor.Pointer, 4, rate); Marshal.WriteInt32(descriptor.Pointer, 8, rate * channels * 2);
            Marshal.WriteInt16(descriptor.Pointer, 0xc, (short)(channels * 2)); Marshal.WriteInt16(descriptor.Pointer, 0xe, 16);
            Marshal.WriteInt16(descriptor.Pointer, 0x10, 0x16); Marshal.WriteInt16(descriptor.Pointer, 0x12, 16);
            Marshal.WriteInt32(descriptor.Pointer, 0x14, 4); Marshal.Copy(Read(At(module, 0xac890), 16), 0, At(descriptor.Pointer, 0x18), 16);
            Marshal.WriteIntPtr(descriptor.Pointer, 0x28, pcm.Pointer); Marshal.WriteInt32(descriptor.Pointer, 0x2c, signed16.Length);
            byte[] descriptorBefore = descriptor.Bytes(); int hr;
            // f30b..f320 frees exactly these three returned pointers through
            // resolved original MSVCR80 free IAT ab1a8; HGlobal is never used.
            try
            {
                hr = encode(1, descriptor.Pointer, 75, 2, blockBytes >> 10, outs.Pointer, At(outs.Pointer, 4), At(outs.Pointer, 8), At(outs.Pointer, 12), At(outs.Pointer, 16), At(outs.Pointer, 20));
                Need(hr >= 0, "Original codec HRESULT=" + unchecked((uint)hr).ToString("x8"));
                byte[][] output = new byte[3][]; object[] allocations = new object[3];
                for (int i = 0; i < 3; i++)
                {
                    IntPtr pointer = Marshal.ReadIntPtr(outs.Pointer, i * 8); int length = Marshal.ReadInt32(outs.Pointer, i * 8 + 4);
                    Need(length >= 0 && length <= 64 * 1024 * 1024, "Unbounded codec output length"); Need(length == 0 || pointer != IntPtr.Zero, "Nonempty codec output has null pointer");
                    output[i] = Read(pointer, length); allocations[i] = new { pointer = Hex(pointer), length = length, sha256 = Hash(output[i]) };
                }
                Need(output[0].Length > 0 && (output[0].Length & 0x7ff) == 0, "Raw codec output is not complete 2048-byte packets");
                Equal("descriptor immutable", descriptorBefore, descriptor.Bytes()); Equal("signed16 input immutable", signed16, pcm.Bytes());
                Dictionary<string, object> after = Fpu(control, reader);
                Need(((control(0, 0) & 0x30300) == 0x10000), "Codec changed configured rounding/precision");
                // Pointer fields are retained in the per-call evidence but zeroed
                // in the stable descriptor file, so fresh-process comparison is meaningful.
                byte[] stableDescriptor = (byte[])descriptorBefore.Clone(); Array.Clear(stableDescriptor, 0x28, 4);
                return new Result { Outputs = output, Descriptor = stableDescriptor, Pcm = signed16, Hresult = hr,
                    Evidence = new { hresult = hr, arguments = new object[] { 1, Hex(descriptor.Pointer), 75, 2, blockBytes >> 10, Hex(outs.Pointer), Hex(At(outs.Pointer, 4)), Hex(At(outs.Pointer, 8)), Hex(At(outs.Pointer, 12)), Hex(At(outs.Pointer, 16)), Hex(At(outs.Pointer, 20)) }, descriptorWithPointersBase64 = Convert.ToBase64String(descriptorBefore), returnedSlotsBase64 = Convert.ToBase64String(outs.Bytes()), allocations = allocations, fpuBefore = before, fpuAfter = after } };
            }
            finally
            {
                HashSet<IntPtr> released = new HashSet<IntPtr>();
                for (int i = 0; i < 3; i++) { IntPtr p = Marshal.ReadIntPtr(outs.Pointer, i * 8); if (p != IntPtr.Zero) { Need(released.Add(p), "Codec returned aliased allocation ownership"); free(p); } }
            }
        }
    }
    static int Main(string[] args)
    {
        IntPtr thunk = IntPtr.Zero; ControlFp control = null; uint saved = 0; bool savedControl = false;
        try
        {
            Need(args.Length == 3, "Usage: AudioXmaRawOracle original.dll existing-fixtures-directory output-directory"); string outputRoot = Path.GetFullPath(args[2]); Directory.CreateDirectory(outputRoot);
            IntPtr module = Load(args[0]);
            Encode encode = (Encode)Marshal.GetDelegateForFunctionPointer(At(module, 0x589d0), typeof(Encode));
            ConvertPcm convert = (ConvertPcm)Marshal.GetDelegateForFunctionPointer(At(module, 0xe9b0), typeof(ConvertPcm));
            Free free = (Free)Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module, 0xab1a8)), typeof(Free));
            control = (ControlFp)Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module, 0xab210)), typeof(ControlFp));
            // Diagnostic-only x86 thunk: mov eax,[esp+4]; fnstcw [eax];
            // stmxcsr [eax+4]; ret. Does not modify the original image.
            byte[] code = new byte[] { 0x8b,0x44,0x24,0x04,0xd9,0x38,0x0f,0xae,0x58,0x04,0xc3 };
            thunk = VirtualAlloc(IntPtr.Zero, new UIntPtr((uint)code.Length), 0x3000, 0x04); Need(thunk != IntPtr.Zero, "FPU reader allocation failed"); Marshal.Copy(code, 0, thunk, code.Length);
            uint oldProtection; Need(VirtualProtect(thunk, new UIntPtr((uint)code.Length), 0x20, out oldProtection) && FlushInstructionCache(GetCurrentProcess(), thunk, new UIntPtr((uint)code.Length)), "FPU reader RX protection failed");
            ReadFpu reader = (ReadFpu)Marshal.GetDelegateForFunctionPointer(thunk, typeof(ReadFpu)); saved = control(0, 0); savedControl = true;
            List<object> cases = new List<object>(); int count = 0;
            foreach (int channels in new int[] { 1, 2 }) foreach (int frames in new int[] { 257, 4096 }) foreach (bool streamed in new bool[] { false, true })
            {
                string name = "xma-" + channels + "-" + frames + "-" + (streamed ? "stream" : "ram"); string directory = Path.Combine(outputRoot, "cases", name); Directory.CreateDirectory(directory);
                string wave = Path.Combine(args[1], "xma-original-" + channels + "-" + frames + ".wav"); byte[] bytes = File.ReadAllBytes(wave);
                Need(bytes.Length == 44 + frames * channels * 2 && Encoding.ASCII.GetString(bytes, 0, 4) == "RIFF" && Encoding.ASCII.GetString(bytes, 8, 8) == "WAVEfmt " && Encoding.ASCII.GetString(bytes, 36, 4) == "data", "Fixture layout differs");
                Need(BitConverter.ToInt16(bytes, 20) == 1 && BitConverter.ToInt16(bytes, 22) == channels && BitConverter.ToInt32(bytes, 24) == 22050 && BitConverter.ToInt16(bytes, 34) == 16, "Fixture format differs");
                byte[] floats = new byte[frames * channels * 4];
                for (int i = 0; i < frames * channels; i++) { float value = BitConverter.ToInt16(bytes, 44 + i * 2) * (1.0f / 32768.0f); Buffer.BlockCopy(BitConverter.GetBytes(value), 0, floats, i * 4, 4); }
                int blockBytes = streamed ? 2048 : 0x28000;
                Result first = Run(module, encode, convert, free, control, reader, floats, channels, frames, 22050, blockBytes);
                Result second = Run(module, encode, convert, free, control, reader, floats, channels, frames, 22050, blockBytes);
                Need(first.Hresult == second.Hresult, "Repeated HRESULT differs"); Equal("repeated descriptor", first.Descriptor, second.Descriptor); Equal("repeated PCM", first.Pcm, second.Pcm);
                for (int i = 0; i < 3; i++) Equal(name + " repeated allocation" + i, first.Outputs[i], second.Outputs[i]);
                object[] files = new object[] { Save(directory, "source.wav", bytes), Save(directory, "source-float32le.bin", floats), Save(directory, "source-signed16le.bin", first.Pcm), Save(directory, "descriptor-pointer-zero.bin", first.Descriptor), Save(directory, "raw-packets.bin", first.Outputs[0]), Save(directory, "allocation2.bin", first.Outputs[1]), Save(directory, "allocation3.bin", first.Outputs[2]) };
                object entry = new { name = name, channels = channels, streamCount = 1, frames = frames, sampleRate = 22050, streamed = streamed, quality = 75, flags = 2, blockBytes = blockBytes, rawLength = first.Outputs[0].Length, allocation2Length = first.Outputs[1].Length, allocation3Length = first.Outputs[2].Length, repeatCount = 2, deterministic = true, files = files, calls = new object[] { first.Evidence, second.Evidence } };
                Json(Path.Combine(directory, "case.json"), entry); cases.Add(entry); count++;
                Console.WriteLine("PASS " + name + " raw=" + first.Outputs[0].Length + " allocation2=" + first.Outputs[1].Length + " allocation3=" + first.Outputs[2].Length + " repeated twice exact");
            }
            Need(count == 8, "Missing native codec cases");
            control(saved, 0x30300); savedControl = false;
            Json(Path.Combine(outputRoot, "raw-codec.json"), new { status = "original-evidence-only", originalDllSha256 = Sha, nativeRva = "0x589d0", converterRva = "0xe9b0", originalFreeIatRva = "0xab1a8", originalControlFpIatRva = "0xab210", backend = "real-original-windows-x86-cpu", originalFpuControlBefore = "0x" + saved.ToString("x8"), fpuAfterRestore = Fpu(control, reader), rawBeforeOuterRewriter = true, xmaEncodingRecovered = false, sdkValidated = false, caseCount = count, nativeEncodeCalls = count * 2, cases = cases });
            Console.WriteLine("PASS original-only raw XMA codec: 8 cases, 16 native encodes, three complete returned allocations and deterministic repeated outputs; no candidate encoder support"); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
        finally { if (savedControl) control(saved, 0x30300); if (thunk != IntPtr.Zero) VirtualFree(thunk, UIntPtr.Zero, 0x8000); }
    }
}
