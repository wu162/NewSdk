using System;
using System.IO;
using System.Reflection;

internal static class AudioAiffOffsetReaderOracle
{
    private static MethodInfo read;
    private static readonly BindingFlags Fields = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static int invariants, refusals;
    private static void Reject(byte[] data, bool loop, string name)
    {
        try { read.Invoke(null, new object[] { data, loop }); }
        catch (TargetInvocationException error) {
            if (error.InnerException is NotSupportedException) { ++refusals; return; }
            throw new Exception("Expected explicit unsupported offset: " + name, error.InnerException);
        }
        throw new Exception("Unsafe offset accepted: " + name);
    }
    private static int Main(string[] args)
    {
        try {
            Assembly library = Assembly.LoadFrom(Path.GetFullPath(args[0]));
            read = library.GetType("BinaryAssetBuilder.AudioCompiler.AudioPcmInputReader", true).GetMethod("Read", BindingFlags.Static | BindingFlags.NonPublic);
            string file = Path.Combine(args[1], "ssnd-offset-reader.aiff");
            foreach (int kind in new int[] { 0, 1, 2, 3 }) foreach (int bits in new int[] { 8, 16, 24 })
            foreach (int channels in new int[] { 1, 2 }) foreach (int frames in kind == 3 ? new int[] { 4096 } : new int[] { 7, 129, 2041 }) {
                int size = bits / 8 * channels, length = frames * size + size - 1;
                int consumed = kind == 2 ? (frames * channels + 1) / 2 : frames * size;
                foreach (uint block in new uint[] { 0, uint.MaxValue }) {
                    foreach (int offset in AudioAiffOffsetCases.Offsets(kind, length, consumed)) {
                        byte[] input = AudioAiffOffsetCases.Create(file, kind, bits, channels, frames, size - 1, (uint)offset, block);
                        byte[] immutable = (byte[])input.Clone();
                        object actual = read.Invoke(null, new object[] { input, kind == 3 });
                        object canonical = read.Invoke(null, new object[] { AudioAiffOffsetCases.Canonical(input, consumed), kind == 3 });
                        Type type = actual.GetType();
                        float[] samples = (float[])type.GetField("Samples", Fields).GetValue(actual);
                        float[] expected = (float[])type.GetField("Samples", Fields).GetValue(canonical);
                        if (samples.Length != frames * channels || samples.Length != expected.Length ||
                            (int)type.GetField("Channels", Fields).GetValue(actual) != channels ||
                            (int)type.GetField("SampleRate", Fields).GetValue(actual) != 22050)
                            throw new Exception("SSND offset changed raw-length-derived frame metadata");
                        for (int i = 0; i < samples.Length; ++i)
                            if (BitConverter.ToInt32(BitConverter.GetBytes(samples[i]), 0) != BitConverter.ToInt32(BitConverter.GetBytes(expected[i]), 0))
                                throw new Exception("SSND bounded offset sample differs from shifted canonical");
                        for (int i = 0; i < input.Length; ++i) if (input[i] != immutable[i]) throw new Exception("SSND input mutated");
                        ++invariants;
                    }
                    byte[] unsafeInput = AudioAiffOffsetCases.Create(file, kind, bits, channels, frames, size - 1, 0, block);
                    int at = AudioAiffOffsetCases.Sound(unsafeInput);
                    foreach (uint unsafeOffset in new uint[] { (uint)(length - consumed + 1), (uint)length, uint.MaxValue }) {
                        byte[] data = (byte[])unsafeInput.Clone(); AudioAiffOffsetCases.BE(data, at + 8, unsafeOffset);
                        Reject(data, kind == 3, kind + ":" + bits + ":" + channels + ":" + unsafeOffset);
                    }
                    // Even a physically present initialized following chunk must
                    // not become audio merely to emulate the native overread.
                    byte[] extra = new byte[unsafeInput.Length + 72]; Array.Copy(unsafeInput, extra, unsafeInput.Length);
                    int tail = unsafeInput.Length; extra[tail] = (byte)'J'; extra[tail + 1] = (byte)'U'; extra[tail + 2] = (byte)'N'; extra[tail + 3] = (byte)'K';
                    AudioAiffOffsetCases.BE(extra, tail + 4, 64); AudioAiffOffsetCases.BE(extra, 4, (uint)extra.Length - 8);
                    for (int i = tail + 8; i < extra.Length; ++i) extra[i] = 0x7f;
                    AudioAiffOffsetCases.BE(extra, at + 8, (uint)(length - consumed + 1));
                    Reject(extra, kind == 3, "following-chunk-is-not-audio");
                }
            }
            if (invariants != 338 || refusals != 480) throw new Exception("Incomplete SSND offset boundaries: " + invariants + "/" + refusals);
            Console.WriteLine("PASS bounded SSND offset:338 sample/metadata/immutability invariants and480 exact chunk-window refusals"); return 0;
        } catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
