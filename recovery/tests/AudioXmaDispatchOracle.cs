using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Web.Script.Serialization;

// Test-process-only x86 dispatch capture. The three audited two-byte CALLs
// are replaced in memory by INT3. The VEH records the actual register target
// then emulates exactly CALL r32's push(return-address) and EIP change in the
// native CONTEXT. The pinned on-disk DLL is never modified. Complete returned
// bytes are checked against the already accepted uninstrumented raw oracle.
internal static class AudioXmaDispatchOracle
{
    [DllImport("kernel32.dll")] static extern IntPtr AddVectoredExceptionHandler(uint first, Handler handler);
    [DllImport("kernel32.dll")] static extern uint RemoveVectoredExceptionHandler(IntPtr handle);
    [DllImport("kernel32.dll")] static extern bool VirtualProtect(IntPtr address, UIntPtr size, uint protection, out uint old);
    [DllImport("kernel32.dll")] static extern IntPtr VirtualAlloc(IntPtr address, UIntPtr size, uint allocation, uint protection);
    [DllImport("kernel32.dll")] static extern bool VirtualFree(IntPtr address, UIntPtr size, uint kind);
    [DllImport("kernel32.dll")] static extern bool FlushInstructionCache(IntPtr process, IntPtr address, UIntPtr size);
    [DllImport("kernel32.dll")] static extern IntPtr GetCurrentProcess();
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate int Handler(IntPtr pointers);
    struct Hit { public int Case, Site, Target, Object, Vtable, SlotTarget, Esp, Eip, Eflags; }
    static readonly int[] Rvas = new int[] { 0x5a4cd, 0x5a529, 0x5a58d };
    static readonly Hit[] Hits = new Hit[4096];
    static readonly Handler Callback = Handle;
    static int image, count, activeCase, callbackError;
    const BindingFlags StaticPrivate = BindingFlags.NonPublic | BindingFlags.Static;
    static IntPtr At(IntPtr p, int n) { return new IntPtr(unchecked(p.ToInt32() + n)); }
    static void Need(bool value, string message) { if (!value) throw new Exception(message); }
    static string Hex(int p) { return "0x" + unchecked((uint)p).ToString("x8"); }
    static string Hash(byte[] bytes) { using (SHA256 h = SHA256.Create()) return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant(); }
    static byte[] Read(IntPtr p, int n) { byte[] b = new byte[n]; Marshal.Copy(p, b, 0, n); return b; }
    // Windows x86 CONTEXT integer register offsets, from winnt.h:
    // Edi156,Esi160,Ebx164,Edx168,Ecx172,Eax176,Ebp180,Eip184,EFlags192,Esp196.
    static int Handle(IntPtr pointers)
    {
        try
        {
            IntPtr record = Marshal.ReadIntPtr(pointers), context = Marshal.ReadIntPtr(pointers, 4);
            if (unchecked((uint)Marshal.ReadInt32(record)) != 0x80000003u) return 0;
            int address = Marshal.ReadInt32(record, 12), index = -1;
            for (int i = 0; i < Rvas.Length; i++) if (unchecked(address - image) == Rvas[i]) index = i;
            if (index < 0) return 0;
            int eip = Marshal.ReadInt32(context, 184), esp = Marshal.ReadInt32(context, 196);
            int target = Marshal.ReadInt32(context, index == 2 ? 168 : 176), instance = Marshal.ReadInt32(context, 172);
            if (eip != unchecked(address + 1) || target == 0 || instance == 0) { callbackError = 1; return 0; }
            int vtable = Marshal.ReadInt32(new IntPtr(instance));
            int slotTarget = Marshal.ReadInt32(new IntPtr(vtable), index == 0 ? 4 : 0);
            int slot = Interlocked.Increment(ref count) - 1;
            if (slot >= Hits.Length) { callbackError = 2; return 0; }
            Hits[slot] = new Hit { Case = activeCase, Site = Rvas[index], Target = target, Object = instance, Vtable = vtable, SlotTarget = slotTarget, Esp = esp, Eip = eip, Eflags = Marshal.ReadInt32(context, 192) };
            Marshal.WriteInt32(new IntPtr(unchecked(esp - 4)), unchecked(address + 2));
            Marshal.WriteInt32(context, 196, unchecked(esp - 4)); Marshal.WriteInt32(context, 184, target);
            return -1; // EXCEPTION_CONTINUE_EXECUTION; all other CONTEXT fields remain unchanged.
        }
        catch { callbackError = 3; return 0; }
    }
    static void Patch(IntPtr address, byte value)
    {
        uint previous; Need(VirtualProtect(address, new UIntPtr(1), 0x40, out previous), "Cannot change call-site protection");
        Marshal.WriteByte(address, value); uint discarded; Need(VirtualProtect(address, new UIntPtr(1), previous, out discarded), "Cannot restore call-site protection");
        Need(FlushInstructionCache(GetCurrentProcess(), address, new UIntPtr(1)), "Cannot flush call-site instruction cache");
    }
    public sealed class SavedFile { public string path; public int length; public string sha256; }
    public sealed class SavedCase { public string name; public int channels; public int frames; public int sampleRate; public int blockBytes; public SavedFile[] files; }
    static object Field(object instance, string name) { return instance.GetType().GetField(name).GetValue(instance); }
    sealed class Guard : IDisposable
    {
        readonly IntPtr allocation; readonly int length; public readonly IntPtr Pointer;
        static byte[] Sentinel(int n) { byte[] b = new byte[n]; for (int i = 0; i < n; i++) b[i] = 0xa5; return b; }
        public Guard(byte[] bytes) { length = bytes.Length; allocation = Marshal.AllocHGlobal(length + 64); Pointer = At(allocation, 32); Marshal.Copy(Sentinel(length + 64), 0, allocation, length + 64); Marshal.Copy(bytes, 0, Pointer, length); }
        public byte[] Bytes() { return Read(Pointer, length); }
        public void Dispose() { try { Need(Hash(Read(allocation, 32)) == Hash(Sentinel(32)) && Hash(Read(At(Pointer, length), 32)) == Hash(Sentinel(32)), "Multistream native guard changed"); } finally { Marshal.FreeHGlobal(allocation); } }
    }
    sealed class MultiResult { public byte[][] Outputs; public object Evidence; }
    static MultiResult RunMulti(Type oracle, IntPtr module, Delegate encode, Delegate free, Delegate control, Delegate reader, byte[] pcm, byte[] template)
    {
        using (Guard source1 = new Guard(pcm)) using (Guard source2 = new Guard(pcm)) using (Guard descriptors = new Guard(new byte[0x78])) using (Guard outs = new Guard(new byte[24]))
        {
            control.DynamicInvoke(new object[] { 0x10000u, 0x30300u });
            Need(template.Length == 0x3c && pcm.Length == 4096 * 2 * 2, "Multistream template differs");
            Marshal.Copy(template, 0, descriptors.Pointer, template.Length); Marshal.Copy(template, 0, At(descriptors.Pointer, 0x3c), template.Length);
            Marshal.WriteIntPtr(descriptors.Pointer, 0x28, source1.Pointer); Marshal.WriteIntPtr(descriptors.Pointer, 0x3c + 0x28, source2.Pointer);
            byte[] original = descriptors.Bytes();
            object before = oracle.GetMethod("Fpu", StaticPrivate).Invoke(null, new object[] { control, reader });
            try
            {
                object[] arguments = new object[] { 2, descriptors.Pointer, 75, 2, 160, outs.Pointer, At(outs.Pointer, 4), At(outs.Pointer, 8), At(outs.Pointer, 12), At(outs.Pointer, 16), At(outs.Pointer, 20) };
                int hr = (int)encode.DynamicInvoke(arguments); Need(hr >= 0, "Multistream original HRESULT=" + Hex(hr));
                byte[][] outputs = new byte[3][]; object[] allocations = new object[3];
                for (int i = 0; i < 3; i++)
                {
                    IntPtr pointer = Marshal.ReadIntPtr(outs.Pointer, i * 8); int length = Marshal.ReadInt32(outs.Pointer, i * 8 + 4);
                    Need(length >= 0 && length <= 64 * 1024 * 1024 && (length == 0 || pointer != IntPtr.Zero), "Invalid multistream returned allocation");
                    outputs[i] = length == 0 ? new byte[0] : Read(pointer, length); allocations[i] = new { pointer = Hex(pointer.ToInt32()), length = length, sha256 = Hash(outputs[i]) };
                }
                Need(outputs[0].Length > 0 && (outputs[0].Length & 2047) == 0, "Multistream packet shape differs");
                Need(Hash(original) == Hash(descriptors.Bytes()) && Hash(pcm) == Hash(source1.Bytes()) && Hash(pcm) == Hash(source2.Bytes()), "Multistream input changed");
                object after = oracle.GetMethod("Fpu", StaticPrivate).Invoke(null, new object[] { control, reader });
                return new MultiResult { Outputs = outputs, Evidence = new { streams = 2, channelsPerStream = 2, frames = 4096, sampleRate = 22050, quality = 75, flags = 2, blockKiB = 160, hresult = hr, descriptorWithPointersBase64 = Convert.ToBase64String(original), outputSlotsBase64 = Convert.ToBase64String(outs.Bytes()), allocations = allocations, fpuBefore = before, fpuAfter = after } };
            }
            finally
            {
                HashSet<IntPtr> released = new HashSet<IntPtr>();
                for (int i = 0; i < 3; i++) { IntPtr p = Marshal.ReadIntPtr(outs.Pointer, i * 8); if (p != IntPtr.Zero) { Need(released.Add(p), "Multistream aliased allocation ownership"); free.DynamicInvoke(new object[] { p }); } }
            }
        }
    }
    static int Main(string[] args)
    {
        IntPtr handler = IntPtr.Zero, fpuReader = IntPtr.Zero, module = IntPtr.Zero;
        bool[] patched = new bool[3]; Delegate control = null; uint saved = 0; bool restoreControl = false;
        try
        {
            Need(IntPtr.Size == 4 && args.Length == 3, "Usage: x86 AudioXmaDispatchOracle original.dll accepted-raw-directory output-directory");
            Directory.CreateDirectory(args[2]); Type oracle = typeof(AudioXmaRawOracle);
            module = (IntPtr)oracle.GetMethod("Load", StaticPrivate).Invoke(null, new object[] { args[0] }); image = module.ToInt32();
            Delegate encode = Marshal.GetDelegateForFunctionPointer(At(module, 0x589d0), oracle.GetNestedType("Encode", BindingFlags.NonPublic));
            Delegate convert = Marshal.GetDelegateForFunctionPointer(At(module, 0xe9b0), oracle.GetNestedType("ConvertPcm", BindingFlags.NonPublic));
            Delegate free = Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module, 0xab1a8)), oracle.GetNestedType("Free", BindingFlags.NonPublic));
            control = Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module, 0xab210)), oracle.GetNestedType("ControlFp", BindingFlags.NonPublic));
            saved = (uint)control.DynamicInvoke(new object[] { 0u, 0u }); restoreControl = true;
            byte[] readCode = new byte[] { 0x8b,0x44,0x24,0x04,0xd9,0x38,0x0f,0xae,0x58,0x04,0xc3 };
            fpuReader = VirtualAlloc(IntPtr.Zero, new UIntPtr((uint)readCode.Length), 0x3000, 4); Need(fpuReader != IntPtr.Zero, "FPU reader allocation failed"); Marshal.Copy(readCode, 0, fpuReader, readCode.Length);
            uint old; Need(VirtualProtect(fpuReader, new UIntPtr((uint)readCode.Length), 0x20, out old) && FlushInstructionCache(GetCurrentProcess(), fpuReader, new UIntPtr((uint)readCode.Length)), "FPU reader RX failed");
            Delegate reader = Marshal.GetDelegateForFunctionPointer(fpuReader, oracle.GetNestedType("ReadFpu", BindingFlags.NonPublic));
            for (int i = 0; i < Rvas.Length; i++) { byte[] actual = Read(At(module, Rvas[i]), 2); Need(actual[0] == 0xff && actual[1] == (i == 2 ? 0xd2 : 0xd0), "Original CALL opcode differs"); }
            RuntimeHelpers.PrepareDelegate(Callback);
            handler = AddVectoredExceptionHandler(1, Callback); Need(handler != IntPtr.Zero, "VEH installation failed");
            for (int i = 0; i < Rvas.Length; i++) { Patch(At(module, Rvas[i]), 0xcc); patched[i] = true; }
            List<object> cases = new List<object>();
            foreach (int channels in new int[] { 1, 2 })
            {
                string name = "xma-" + channels + "-4096-ram"; string baseline = Path.Combine(args[1], "cases", name);
                SavedCase data = new JavaScriptSerializer().Deserialize<SavedCase>(File.ReadAllText(Path.Combine(baseline, "case.json")));
                Need(data.name == name && data.channels == channels && data.frames == 4096 && data.sampleRate == 22050 && data.blockBytes == 0x28000, "Accepted baseline case differs");
                Dictionary<string, SavedFile> files = new Dictionary<string, SavedFile>(); foreach (SavedFile f in data.files) files.Add(f.path, f);
                byte[] floats = File.ReadAllBytes(Path.Combine(baseline, "source-float32le.bin")); Need(Hash(floats) == files["source-float32le.bin"].sha256, "Accepted source fingerprint differs");
                activeCase = channels; int firstHit = count;
                object result = oracle.GetMethod("Run", StaticPrivate).Invoke(null, new object[] { module, encode, convert, free, control, reader, floats, channels, data.frames, data.sampleRate, data.blockBytes });
                Need(callbackError == 0, "VEH callback failed=" + callbackError);
                byte[][] outputs = (byte[][])Field(result, "Outputs"); byte[] pcm = (byte[])Field(result, "Pcm"), descriptor = (byte[])Field(result, "Descriptor");
                string outDir = Path.Combine(args[2], name); Directory.CreateDirectory(outDir);
                string[] names = new string[] { "raw-packets.bin", "allocation2.bin", "allocation3.bin" };
                for (int i = 0; i < names.Length; i++) { File.WriteAllBytes(Path.Combine(outDir, names[i]), outputs[i]); Need(outputs[i].Length == files[names[i]].length && Hash(outputs[i]) == files[names[i]].sha256, "Instrumented " + names[i] + " differs from accepted uninstrumented original"); }
                Need(Hash(pcm) == files["source-signed16le.bin"].sha256 && Hash(descriptor) == files["descriptor-pointer-zero.bin"].sha256, "Instrumented input differs from accepted baseline");
                HashSet<int> sites = new HashSet<int>(); for (int i = firstHit; i < count; i++) sites.Add(Hits[i].Site);
                foreach (int site in new int[] { 0x5a4cd, 0x5a58d }) Need(sites.Contains(site), "Expected single-stream call site not reached " + Hex(site));
                cases.Add(new { name = name, baseline = "xma-raw-validation/40232525056-1/cases/" + name, hitCount = count - firstHit, originalOutputsEqualAcceptedRaw = true, call = Field(result, "Evidence") });
                Console.WriteLine("PASS dispatch " + name + " hits=" + (count - firstHit) + " all three raw allocations equal accepted uninstrumented original");
            }
            // A single stereo stream deliberately does not own the aggregate
            // object and cannot reach 5a529. Add a distinct two-stream fixture,
            // with the original f150 descriptor layout and accepted stereo PCM
            // duplicated into two read-only stream allocations. This is native
            // codec evidence, not four-channel AudioFile product support.
            for (int i = 0; i < Rvas.Length; i++) { Patch(At(module, Rvas[i]), 0xff); patched[i] = false; }
            string multiBaseline = Path.Combine(args[1], "cases", "xma-2-4096-ram");
            byte[] multiPcm = File.ReadAllBytes(Path.Combine(multiBaseline, "source-signed16le.bin")), multiTemplate = File.ReadAllBytes(Path.Combine(multiBaseline, "descriptor-pointer-zero.bin"));
            MultiResult untraced = RunMulti(oracle, module, encode, free, control, reader, multiPcm, multiTemplate);
            for (int i = 0; i < Rvas.Length; i++) { Patch(At(module, Rvas[i]), 0xcc); patched[i] = true; }
            activeCase = 4; int multiStart = count;
            MultiResult traced = RunMulti(oracle, module, encode, free, control, reader, multiPcm, multiTemplate);
            Need(callbackError == 0, "Multistream VEH callback failed=" + callbackError);
            string multiDir = Path.Combine(args[2], "xma-two-stereo-streams-4096-ram"); Directory.CreateDirectory(multiDir);
            File.WriteAllBytes(Path.Combine(multiDir, "each-stream-signed16le.bin"), multiPcm); File.WriteAllBytes(Path.Combine(multiDir, "each-descriptor-pointer-zero.bin"), multiTemplate);
            string[] multiNames = new string[] { "raw-packets.bin", "allocation2.bin", "allocation3.bin" };
            for (int i = 0; i < 3; i++)
            {
                File.WriteAllBytes(Path.Combine(multiDir, "uninstrumented-" + multiNames[i]), untraced.Outputs[i]); File.WriteAllBytes(Path.Combine(multiDir, "instrumented-" + multiNames[i]), traced.Outputs[i]);
                Need(untraced.Outputs[i].Length == traced.Outputs[i].Length && Hash(untraced.Outputs[i]) == Hash(traced.Outputs[i]), "Multistream instrumentation changed " + multiNames[i]);
            }
            HashSet<int> multiSites = new HashSet<int>(); for (int i = multiStart; i < count; i++) multiSites.Add(Hits[i].Site);
            foreach (int site in Rvas) Need(multiSites.Contains(site), "Expected multistream call site not reached " + Hex(site));
            cases.Add(new { name = "xma-two-stereo-streams-4096-ram", streamCount = 2, channelsPerStream = 2, hitCount = count - multiStart, originalOutputsEqualUninstrumented = true, uninstrumented = untraced.Evidence, instrumented = traced.Evidence });
            Console.WriteLine("PASS dispatch two stereo streams: hits=" + (count - multiStart) + " all three allocations equal uninstrumented original; aggregate destructor site reached");
            List<object> observed = new List<object>();
            for (int i = 0; i < count; i++) { Hit h = Hits[i]; Need(h.Target == h.SlotTarget, "Register target differs from live vtable slot"); observed.Add(new { channels = h.Case, callSiteRva = Hex(h.Site), targetRva = Hex(unchecked(h.Target - image)), targetAddress = Hex(h.Target), objectAddress = Hex(h.Object), vtableRva = Hex(unchecked(h.Vtable - image)), vtableAddress = Hex(h.Vtable), matchedLiveVtableSlot = true, originalEsp = Hex(h.Esp), exceptionEip = Hex(h.Eip), originalEflags = Hex(h.Eflags) }); }
            var evidence = new { status = "original-evidence-only", baselineSourceCommit = "cd66d9a2c821bc41f1f975aac8ac0a5bac6a6499", baselineRunId = "40232525056", originalDllSha256 = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139", backend = "real-windows-x86-cpu-VEH-call-interception", instructionSemantics = "INT3 at FF-D0/FF-D2; CONTEXT ESP-=4; [ESP]=site+2; EIP=original EAX/EDX; other registers and flags unchanged", onDiskOriginalUnmodified = true, numericalEncoderRecovered = false, caseCount = cases.Count, nativeEncodeCalls = 4, hitCount = count, cases = cases, observations = observed };
            File.WriteAllText(Path.Combine(args[2], "dispatch.json"), new JavaScriptSerializer().Serialize(evidence), new UTF8Encoding(false));
            Console.WriteLine("PASS original-only dynamic XMA dispatch: 3 traced encodes (4 native calls including new multistream baseline), " + count + " actual indirect call targets, all returned allocations equal accepted baseline"); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
        finally
        {
            for (int i = 0; i < Rvas.Length; i++) if (patched[i]) Patch(At(module, Rvas[i]), 0xff);
            if (handler != IntPtr.Zero) RemoveVectoredExceptionHandler(handler);
            if (restoreControl) control.DynamicInvoke(new object[] { saved, 0x30300u });
            if (fpuReader != IntPtr.Zero) VirtualFree(fpuReader, UIntPtr.Zero, 0x8000);
            GC.KeepAlive(Callback);
        }
    }
}
