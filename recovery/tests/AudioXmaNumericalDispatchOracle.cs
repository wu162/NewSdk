using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

// Original-only numerical dispatch evidence through an entirely native
// trampoline. No VEH or managed callback runs inside the codec. The existing
// peripheral dispatch group is not rerun; complete outputs must equal the
// accepted uninstrumented mono/stereo 4096-frame raw-codec fixtures.
internal static class AudioXmaNumericalDispatchOracle
{
    const int Site = 0x97d97;
    const int TargetOffset = 0x18e08;
    const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;
    [DllImport("kernel32.dll")] static extern bool VirtualProtect(IntPtr address, UIntPtr size, uint protection, out uint previous);
    [DllImport("kernel32.dll")] static extern IntPtr VirtualAlloc(IntPtr address, UIntPtr size, uint allocation, uint protection);
    [DllImport("kernel32.dll")] static extern bool VirtualFree(IntPtr address, UIntPtr size, uint kind);
    [DllImport("kernel32.dll")] static extern IntPtr GetCurrentProcess();
    [DllImport("kernel32.dll")] static extern bool FlushInstructionCache(IntPtr process, IntPtr address, UIntPtr size);
    struct Hit { public int Channels, Target, State, TargetSlot, Esp, Eflags; }
    static IntPtr At(IntPtr p, int offset) { return new IntPtr(unchecked(p.ToInt32() + offset)); }
    static void Need(bool value, string message) { if (!value) throw new Exception(message); }
    static string Hex(int value) { return "0x" + unchecked((uint)value).ToString("x8"); }
    static string Hash(byte[] value) { using (SHA256 sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(value)).Replace("-", "").ToLowerInvariant(); }
    static byte[] Read(IntPtr pointer, int length) { byte[] b = new byte[length]; Marshal.Copy(pointer, b, 0, length); return b; }
    // All recording executes as native integer instructions. No CLR reverse
    // P/Invoke, exceptions, debugger, managed callback, or FP operation occurs
    // while the original codec runs. The callee sees the ORIGINAL return IP.
    sealed class NativeTrace : IDisposable
    {
        const int Capacity = 8192, RecordBytes = 24;
        readonly IntPtr site, allocation, journal, stub;
        readonly byte[] original;
        readonly byte[] stubBytes;
        readonly int journalBytes = 16 + Capacity * RecordBytes;
        bool patched;
        static byte[] Sentinel(int n) { byte[] b = new byte[n]; for (int i = 0; i < n; i++) b[i] = 0xa5; return b; }
        static void Add(List<byte> code, params byte[] bytes) { code.AddRange(bytes); }
        static void Dword(List<byte> code, int value) { code.AddRange(BitConverter.GetBytes(value)); }
        static void Set(List<byte> code, int at, int value) { byte[] bytes = BitConverter.GetBytes(value); for (int i = 0; i < 4; i++) code[at + i] = bytes[i]; }
        public NativeTrace(IntPtr module)
        {
            site = At(module, Site); original = Read(site, 6);
            Need(Hash(original) == Hash(new byte[] { 0xff,0x96,0x08,0x8e,0x01,0x00 }), "Original numerical CALL instruction differs");
            allocation = Marshal.AllocHGlobal(journalBytes + 64); journal = At(allocation, 32);
            Marshal.Copy(Sentinel(journalBytes + 64), 0, allocation, journalBytes + 64); Marshal.Copy(new byte[journalBytes], 0, journal, journalBytes);
            List<byte> code = new List<byte>();
            Add(code, 0x9c,0x60);                         // pushfd; pushad
            Add(code, 0xb8); Dword(code, 1);              // mov eax,1
            Add(code, 0xf0,0x0f,0xc1,0x05); Dword(code, journal.ToInt32()); // lock xadd [count],eax
            Add(code, 0x3d); Dword(code, Capacity);       // cmp eax,capacity
            Add(code, 0x0f,0x83); int overflowJump = code.Count; Dword(code, 0);
            Add(code, 0x6b,0xc0,RecordBytes,0x05); Dword(code, At(journal, 16).ToInt32()); // eax=record
            Add(code, 0x89,0x30);                        // [eax]=esi
            Add(code, 0x8b,0x96); Dword(code, TargetOffset); Add(code, 0x89,0x50,0x04);
            Add(code, 0x8d,0x96); Dword(code, TargetOffset); Add(code, 0x89,0x50,0x08);
            Add(code, 0x8d,0x54,0x24,0x24,0x89,0x50,0x0c); // original esp = current esp+36
            Add(code, 0x8b,0x15); Dword(code, At(journal, 8).ToInt32()); Add(code, 0x89,0x50,0x10);
            Add(code, 0x8b,0x54,0x24,0x20,0x89,0x50,0x14); // saved original flags at [esp+32]
            Add(code, 0xe9); int doneJump = code.Count; Dword(code, 0);
            int overflow = code.Count; Add(code, 0xc7,0x05); Dword(code, At(journal, 4).ToInt32()); Dword(code, 1);
            int done = code.Count; Add(code, 0x61,0x9d,0x68); Dword(code, At(site, 6).ToInt32()); // popad;popfd;push ORIGINAL return
            Add(code, 0xff,0xa6); Dword(code, TargetOffset); // jmp [original esi+18e08]
            Set(code, overflowJump, overflow - (overflowJump + 4)); Set(code, doneJump, done - (doneJump + 4));
            byte[] bytes = code.ToArray(); stubBytes = bytes; stub = VirtualAlloc(IntPtr.Zero, new UIntPtr((uint)bytes.Length), 0x3000, 4); Need(stub != IntPtr.Zero, "Native journal stub allocation failed"); Marshal.Copy(bytes, 0, stub, bytes.Length);
            uint previous; Need(VirtualProtect(stub, new UIntPtr((uint)bytes.Length), 0x20, out previous) && FlushInstructionCache(GetCurrentProcess(), stub, new UIntPtr((uint)bytes.Length)), "Native journal stub RX failed");
            byte[] jump = new byte[6]; jump[0] = 0xe9; Buffer.BlockCopy(BitConverter.GetBytes(unchecked(stub.ToInt32() - site.ToInt32() - 5)), 0, jump, 1, 4); jump[5] = 0x90;
            WriteSite(jump); patched = true;
        }
        void WriteSite(byte[] bytes)
        {
            uint previous; Need(VirtualProtect(site, new UIntPtr(6), 0x40, out previous), "Numerical call-site protection failed"); Marshal.Copy(bytes, 0, site, 6);
            uint discarded; Need(VirtualProtect(site, new UIntPtr(6), previous, out discarded) && FlushInstructionCache(GetCurrentProcess(), site, new UIntPtr(6)), "Numerical call-site restore/flush failed");
        }
        public void SetCase(int value) { Marshal.WriteInt32(journal, 8, value); }
        public int Count { get { int value = Marshal.ReadInt32(journal); Need(Marshal.ReadInt32(journal, 4) == 0 && value >= 0 && value <= Capacity, "Native journal capacity exceeded"); return value; } }
        public Hit Get(int index) { Need(index >= 0 && index < Count, "Native journal index differs"); IntPtr p = At(journal, 16 + index * RecordBytes); return new Hit { State = Marshal.ReadInt32(p), Target = Marshal.ReadInt32(p, 4), TargetSlot = Marshal.ReadInt32(p, 8), Esp = Marshal.ReadInt32(p, 12), Channels = Marshal.ReadInt32(p, 16), Eflags = Marshal.ReadInt32(p, 20) }; }
        public object Evidence() { return new { patch = "E9 rel32 + NOP", originalCallRva = "0x97d97", originalReturnRva = "0x97d9d", nativeStubAddress = Hex(stub.ToInt32()), journalAddress = Hex(journal.ToInt32()), nativeStubBase64 = Convert.ToBase64String(stubBytes), capacity = Capacity, recordBytes = RecordBytes, preservesAllIntegerRegistersFlagsAndFpu = true, managedCallbacks = 0 }; }
        public void Dispose()
        {
            try { if (patched) { WriteSite(original); patched = false; } Need(Hash(Read(allocation, 32)) == Hash(Sentinel(32)) && Hash(Read(At(journal, journalBytes), 32)) == Hash(Sentinel(32)), "Native journal guard changed"); }
            finally { if (stub != IntPtr.Zero) VirtualFree(stub, UIntPtr.Zero, 0x8000); Marshal.FreeHGlobal(allocation); }
        }
    }
    public sealed class SavedFile { public string path; public int length; public string sha256; }
    public sealed class SavedCase { public string name; public int channels; public int frames; public int sampleRate; public int blockBytes; public SavedFile[] files; }
    static object Field(object instance, string name) { return instance.GetType().GetField(name).GetValue(instance); }
    static int Main(string[] args)
    {
        IntPtr module = IntPtr.Zero, readerCode = IntPtr.Zero; Delegate control = null; uint savedControl = 0; bool restore = false; NativeTrace trace = null;
        try
        {
            Need(args.Length == 3 && IntPtr.Size == 4, "Usage: x86 AudioXmaNumericalDispatchOracle original.dll accepted-raw-directory output-directory");
            Directory.CreateDirectory(args[2]); Type raw = typeof(AudioXmaRawOracle);
            Console.WriteLine("PHASE load pinned original"); Console.Out.Flush();
            module = (IntPtr)raw.GetMethod("Load", PrivateStatic).Invoke(null, new object[] { args[0] }); int image = module.ToInt32();
            Delegate encode = Marshal.GetDelegateForFunctionPointer(At(module, 0x589d0), raw.GetNestedType("Encode", BindingFlags.NonPublic));
            Delegate convert = Marshal.GetDelegateForFunctionPointer(At(module, 0xe9b0), raw.GetNestedType("ConvertPcm", BindingFlags.NonPublic));
            Delegate free = Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module, 0xab1a8)), raw.GetNestedType("Free", BindingFlags.NonPublic));
            control = Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module, 0xab210)), raw.GetNestedType("ControlFp", BindingFlags.NonPublic));
            savedControl = (uint)control.DynamicInvoke(new object[] { 0u, 0u }); restore = true;
            byte[] code = new byte[] { 0x8b,0x44,0x24,0x04,0xd9,0x38,0x0f,0xae,0x58,0x04,0xc3 };
            readerCode = VirtualAlloc(IntPtr.Zero, new UIntPtr((uint)code.Length), 0x3000, 4); Need(readerCode != IntPtr.Zero, "FPU reader allocation failed"); Marshal.Copy(code, 0, readerCode, code.Length);
            uint old; Need(VirtualProtect(readerCode, new UIntPtr((uint)code.Length), 0x20, out old) && FlushInstructionCache(GetCurrentProcess(), readerCode, new UIntPtr((uint)code.Length)), "FPU reader RX failed");
            Delegate reader = Marshal.GetDelegateForFunctionPointer(readerCode, raw.GetNestedType("ReadFpu", BindingFlags.NonPublic));
            byte[] expected = new byte[] { 0xff,0x96,0x08,0x8e,0x01,0x00 }, actual = Read(At(module, Site), 6);
            Need(Hash(expected) == Hash(actual), "Original numerical CALL [ESI+18e08] instruction differs");
            Console.WriteLine("PHASE install native journal trampoline"); Console.Out.Flush();
            trace = new NativeTrace(module);
            Console.WriteLine("PHASE native journal ready"); Console.Out.Flush();
            List<object> cases = new List<object>();
            foreach (int n in new int[] { 1, 2 })
            {
                string name = "xma-" + n + "-4096-ram", baseline = Path.Combine(args[1], "cases", name), directory = Path.Combine(args[2], name); Directory.CreateDirectory(directory);
                SavedCase original = new JavaScriptSerializer().Deserialize<SavedCase>(File.ReadAllText(Path.Combine(baseline, "case.json")));
                Need(original.name == name && original.channels == n && original.frames == 4096 && original.sampleRate == 22050 && original.blockBytes == 0x28000, "Accepted numerical baseline differs");
                Dictionary<string, SavedFile> files = new Dictionary<string, SavedFile>(); foreach (SavedFile file in original.files) files.Add(file.path, file);
                byte[] source = File.ReadAllBytes(Path.Combine(baseline, "source-float32le.bin")); Need(Hash(source) == files["source-float32le.bin"].sha256, "Accepted float input hash differs");
                trace.SetCase(n); int start = trace.Count;
                Console.WriteLine("PHASE encode " + name + " journalBefore=" + trace.Count); Console.Out.Flush();
                object result = raw.GetMethod("Run", PrivateStatic).Invoke(null, new object[] { module, encode, convert, free, control, reader, source, n, original.frames, original.sampleRate, original.blockBytes });
                Need(trace.Count > start, "Numerical CALL site was not reached");
                byte[][] outputs = (byte[][])Field(result, "Outputs"); string[] names = new string[] { "raw-packets.bin", "allocation2.bin", "allocation3.bin" };
                for (int i = 0; i < 3; i++) { File.WriteAllBytes(Path.Combine(directory, names[i]), outputs[i]); Need(outputs[i].Length == files[names[i]].length && Hash(outputs[i]) == files[names[i]].sha256, "Numerical interception changed " + names[i]); }
                Need(Hash((byte[])Field(result, "Pcm")) == files["source-signed16le.bin"].sha256 && Hash((byte[])Field(result, "Descriptor")) == files["descriptor-pointer-zero.bin"].sha256, "Numerical interception changed input");
                cases.Add(new { name = name, hitCount = trace.Count - start, rawOutputsEqualAcceptedUninstrumented = true, call = Field(result, "Evidence") });
                Console.WriteLine("PASS numerical dispatch " + name + " hits=" + (trace.Count - start) + " three allocations equal accepted uninstrumented original");
            }
            int peOffset = Marshal.ReadInt32(At(module, 0x3c));
            int imageSize = Marshal.ReadInt32(At(module, peOffset + 24 + 56));
            Need(imageSize > 0, "Loaded PE image size differs");
            List<object> observations = new List<object>();
            int count = trace.Count;
            for (int i = 0; i < count; i++) { Hit hit = trace.Get(i); observations.Add(new { channels = hit.Channels, callSiteRva = Hex(Site), targetRva = unchecked((uint)(hit.Target - image)) < (uint)imageSize ? Hex(unchecked(hit.Target - image)) : null, targetInsideOriginalImage = unchecked((uint)(hit.Target - image)) < (uint)imageSize, targetAddress = Hex(hit.Target), stateEsi = Hex(hit.State), targetSlotAddress = Hex(hit.TargetSlot), targetOffset = Hex(TargetOffset), originalEsp = Hex(hit.Esp), originalEflags = Hex(hit.Eflags) }); }
            object receipt = new { status = "original-evidence-only", baselineSourceCommit = "cd66d9a2c821bc41f1f975aac8ac0a5bac6a6499", baselineRunId = "40232525056", originalDllSha256 = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139", backend = "real-windows-x86-cpu-native-journal-trampoline", originalInstruction = "FF 96 08 8E 01 00", callSiteRva = "0x00097d97", callTarget = "dword ptr [ESI+0x18e08]", instrumentationSemantics = "E9 rel32 + NOP; native PUSHFD/PUSHAD journal; POPAD/POPFD; PUSH original site+6; JMP [original ESI+18e08]", instrumentation = trace.Evidence(), caseCount = cases.Count, nativeEncodeCalls = cases.Count, hitCount = count, numericalEncoderRecovered = false, productionRouteEnabled = false, cases = cases, observations = observations };
            File.WriteAllText(Path.Combine(args[2], "numerical-dispatch.json"), new JavaScriptSerializer().Serialize(receipt), new UTF8Encoding(false));
            Console.WriteLine("PASS original-only numerical XMA dispatch: 2 native encodes, " + count + " memory-indirect targets observed, three allocations per case equal accepted baseline"); return 0;
        }
        catch (Exception failure) { Console.Error.WriteLine(failure); return 1; }
        finally
        {
            if (trace != null) trace.Dispose();
            if (restore) control.DynamicInvoke(new object[] { savedControl, 0x30300u }); if (readerCode != IntPtr.Zero) VirtualFree(readerCode, UIntPtr.Zero, 0x8000);
        }
    }
}
