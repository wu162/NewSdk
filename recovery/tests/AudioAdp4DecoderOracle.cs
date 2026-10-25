using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

internal static class AudioAdp4DecoderOracle
{
    private const string Sha = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139";
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode)] private static extern IntPtr GetModuleHandle(string name);
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode)] private static extern uint GetModuleFileName(IntPtr module, System.Text.StringBuilder text, int capacity);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int Native(int nibble, IntPtr state);
    private static IntPtr Add(IntPtr p, int n) { return new IntPtr(unchecked(p.ToInt32() + n)); }
    private static void Need(bool condition, string message) { if (!condition) throw new Exception(message); }
    private static byte[] Fill(int count) { byte[] b = new byte[count]; for (int i = 0; i < count; ++i) b[i] = 0xa5; return b; }
    private sealed class State : IDisposable
    {
        private IntPtr memory;
        internal readonly IntPtr Pointer;
        internal State(int predictor, int index)
        {
            memory = Marshal.AllocHGlobal(92); Pointer = Add(memory, 32);
            byte[] b = Fill(92); Marshal.Copy(b, 0, memory, b.Length);
            Marshal.WriteInt32(Pointer, predictor); Marshal.WriteInt32(Add(Pointer, 24), index);
        }
        internal int Predictor { get { return Marshal.ReadInt32(Pointer); } }
        internal int Index { get { return Marshal.ReadInt32(Add(Pointer, 24)); } }
        internal void Guard()
        {
            byte[] b = new byte[92]; Marshal.Copy(memory, b, 0, b.Length);
            for (int i = 0; i < b.Length; ++i)
                if (!(i >= 32 && i < 36) && !(i >= 56 && i < 60)) Need(b[i] == 0xa5, "Native state overwrote guard/unrelated field at " + i);
        }
        public void Dispose() { try { Guard(); } finally { Marshal.FreeHGlobal(memory); memory = IntPtr.Zero; } }
    }
    private static int Scalar(Native native, MethodInfo method, int nibble, ref int predictor, ref int index)
    {
        if (native != null) using (State state = new State(predictor, index)) {
            int result = native(nibble, state.Pointer); predictor = state.Predictor; index = state.Index; return result;
        }
        object[] args = { nibble, predictor, index }; int value = (int)method.Invoke(null, args);
        predictor = (int)args[1]; index = (int)args[2]; return value;
    }
    private static byte[] Sequence(Native native, byte[] source, int offset, int channels, int frames)
    {
        short[] output = new short[channels * frames];
        using (State left = new State(0, 0)) using (State right = new State(0, 0)) {
            for (int frame = 0; frame < frames; ++frame) {
                if (channels == 1) {
                    int nibble = source[offset + frame / 2]; nibble = (frame & 1) == 0 ? nibble >> 4 : nibble & 15;
                    output[frame] = (short)native(nibble, left.Pointer);
                } else {
                    int pair = source[offset + frame]; output[frame * 2] = (short)native(pair >> 4, left.Pointer);
                    output[frame * 2 + 1] = (short)native(pair & 15, right.Pointer);
                }
            }
        }
        byte[] result = new byte[output.Length * 2]; Buffer.BlockCopy(output, 0, result, 0, result.Length); return result;
    }
    private static void Reject(MethodInfo method, object[] args, Type expected)
    {
        try { method.Invoke(null, args); }
        catch (TargetInvocationException error) { Need(error.InnerException.GetType() == expected, "Unexpected candidate rejection type"); return; }
        throw new Exception("Expected safety refusal: " + expected.Name);
    }
    private static int Main(string[] args)
    {
        try {
            Need(args.Length == 3 && IntPtr.Size == 4, "Usage (x86): original|candidate assembly fixture.gz");
            bool original = args[0] == "original"; Native native = null; MethodInfo scalar = null, decode = null;
            if (original) {
                string path = Path.GetFullPath(args[1]);
                using (SHA256 hash = SHA256.Create()) Need(BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant() == Sha, "Original SHA mismatch");
                Assembly assembly = Assembly.LoadFrom(path); Need(String.Equals(assembly.Location, path, StringComparison.OrdinalIgnoreCase), "Original assembly path");
                IntPtr module = GetModuleHandle(Path.GetFileName(path)); var text = new System.Text.StringBuilder(32768);
                Need(module != IntPtr.Zero && GetModuleFileName(module, text, text.Capacity) != 0 && String.Equals(Path.GetFullPath(text.ToString()), path, StringComparison.OrdinalIgnoreCase), "Original native path");
                native = (Native)Marshal.GetDelegateForFunctionPointer(Add(module, 0x48500), typeof(Native));
            } else {
                Need(args[0] == "candidate", "Unknown mode");
                Need(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll") == IntPtr.Zero && !File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BinaryAssetBuilder.AudioCompiler.dll")), "Original DLL present in candidate");
                Type type = Assembly.LoadFrom(Path.GetFullPath(args[1])).GetType("BinaryAssetBuilder.AudioCompiler.Adp4Decoder", true);
                scalar = type.GetMethod("DecodeNibble"); decode = type.GetMethod("Decode");
            }
            string header = "# originalSha256=" + Sha + " nativeRva=0x48500 tables=0xfe340,0xfe4a4 rawNibbleDecoderOnly=true";
            StreamWriter writer = null; StreamReader reader = null; int records = 0;
            try {
                if (original) { writer = new StreamWriter(new GZipStream(File.Create(args[2]), CompressionMode.Compress)); writer.WriteLine(header); }
                else { reader = new StreamReader(new GZipStream(File.OpenRead(args[2]), CompressionMode.Decompress)); Need(reader.ReadLine() == header, "Original evidence identity"); }
                int[] predictors = { -32768, -32767, -1, 0, 1, 32766, 32767 };
                int[] nibbles = { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,-8,-7,-6,-5,-4,-3,-2,-1,16,31,255,Int32.MinValue,Int32.MaxValue };
                foreach (int initial in predictors) for (int step = 0; step <= 88; ++step) foreach (int nibble in nibbles) {
                    int predictor = initial, index = step;
                    int sample = Scalar(native, scalar, nibble, ref predictor, ref index);
                    string line = "scalar\t" + initial + "\t" + step + "\t" + nibble + "\t" + sample + "\t" + predictor + "\t" + index;
                    if (original) writer.WriteLine(line); else Need(reader.ReadLine() == line, "ADP4 scalar mismatch: " + line);
                    ++records;
                }
                foreach (int channels in new int[] { 1, 2 }) foreach (int frames in new int[] { 0,1,2,7,63,64,129,1023,1024,1025,2041 })
                foreach (int offset in new int[] { 0, 3 }) foreach (int pattern in new int[] { 0, 1, 2, 3 }) {
                    byte[] source = new byte[offset + (frames * channels + 1) / 2 + 7]; uint seed = 0x12345678;
                    for (int i = 0; i < source.Length; ++i) { seed = unchecked(seed * 1664525 + 1013904223); source[i] = pattern == 0 ? (byte)0 : pattern == 1 ? (byte)255 : pattern == 2 ? (byte)i : (byte)(seed >> 24); }
                    byte[] before = (byte[])source.Clone(), output;
                    if (original) output = Sequence(native, source, offset, channels, frames);
                    else { short[] decoded = (short[])decode.Invoke(null, new object[] { source, offset, channels, frames }); output = new byte[decoded.Length * 2]; Buffer.BlockCopy(decoded, 0, output, 0, output.Length); }
                    for (int i = 0; i < source.Length; ++i) Need(source[i] == before[i], "ADP4 source mutated");
                    string line = "sequence\t" + channels + "\t" + frames + "\t" + offset + "\t" + pattern + "\t" + Convert.ToBase64String(output);
                    if (original) writer.WriteLine(line); else Need(reader.ReadLine() == line, "ADP4 sequence mismatch: " + channels + "/" + frames + "/" + pattern);
                    ++records;
                }
                Need(records == 18243, "Incomplete ADP4 matrix: " + records);
                if (!original) Need(reader.ReadLine() == null, "Extra native records");
            } finally { if (writer != null) writer.Dispose(); if (reader != null) reader.Dispose(); }
            if (!original) {
                foreach (object[] invalid in new object[][] { new object[] { 0, -32769, 0 }, new object[] { 0, 32768, 0 }, new object[] { 0, 0, -1 }, new object[] { 0, 0, 89 } }) {
                    int predictor = (int)invalid[1], index = (int)invalid[2]; Reject(scalar, invalid, typeof(ArgumentOutOfRangeException)); Need((int)invalid[1] == predictor && (int)invalid[2] == index, "Rejected state changed");
                }
                Reject(decode, new object[] { null, 0, 1, 1 }, typeof(ArgumentNullException));
                Reject(decode, new object[] { new byte[1], -1, 1, 1 }, typeof(ArgumentOutOfRangeException));
                Reject(decode, new object[] { new byte[1], 2, 1, 1 }, typeof(ArgumentOutOfRangeException));
                Reject(decode, new object[] { new byte[1], 0, 3, 1 }, typeof(ArgumentOutOfRangeException));
                Reject(decode, new object[] { new byte[1], 0, 1, -1 }, typeof(ArgumentOutOfRangeException));
                Reject(decode, new object[] { new byte[1], 0, 2, Int32.MaxValue }, typeof(ArgumentOutOfRangeException));
                Reject(decode, new object[] { new byte[1], 0, 2, 2 }, typeof(ArgumentException));
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) Need(a.GetName().Name != "BinaryAssetBuilder.AudioCompiler", "Original assembly loaded in candidate");
                Console.WriteLine("PASS ADP4 candidate safety:11 refusals");
            }
            Console.WriteLine("PASS ADP4 standalone decoder:18243 exact records (18067 scalar,176 initialized stream sequences); no AudioFile route"); return 0;
        } catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
