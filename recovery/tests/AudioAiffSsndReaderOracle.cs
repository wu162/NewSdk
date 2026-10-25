using System;
using System.IO;
using System.Reflection;

internal static class AudioAiffSsndReaderOracle
{
    private static int Main(string[] args)
    {
        try {
            Assembly library = Assembly.LoadFrom(Path.GetFullPath(args[0]));
            MethodInfo read = library.GetType("BinaryAssetBuilder.AudioCompiler.AudioPcmInputReader", true).GetMethod("Read", BindingFlags.Static | BindingFlags.NonPublic);
            string file = Path.Combine(args[1], "ssnd-reader.aiff"); int records = 0;
            foreach (int kind in new int[] { 0, 1, 2, 3 }) foreach (int bits in new int[] { 8, 16, 24 }) foreach (int channels in new int[] { 1, 2 }) {
                AudioAiffSsndCases.Write(file, kind, bits, channels, kind == 3 ? 4096 : 129, 0);
                object canonical = read.Invoke(null, new object[] { File.ReadAllBytes(file), kind == 3 });
                BindingFlags fields = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                Type type = canonical.GetType(); float[] first = (float[])type.GetField("Samples", fields).GetValue(canonical);
                foreach (uint block in AudioAiffSsndCases.Blocks) {
                    AudioAiffSsndCases.Write(file, kind, bits, channels, kind == 3 ? 4096 : 129, block);
                    byte[] input = File.ReadAllBytes(file); object actual = read.Invoke(null, new object[] { input, kind == 3 });
                    float[] second = (float[])type.GetField("Samples", fields).GetValue(actual);
                    if (first.Length != second.Length || (int)type.GetField("Channels", fields).GetValue(actual) != channels || (int)type.GetField("SampleRate", fields).GetValue(actual) != 22050)
                        throw new Exception("SSND blockSize changed metadata");
                    for (int i = 0; i < first.Length; ++i)
                        if (BitConverter.ToInt32(BitConverter.GetBytes(first[i]), 0) != BitConverter.ToInt32(BitConverter.GetBytes(second[i]), 0)) throw new Exception("SSND blockSize changed sample bits");
                    // Offset remains separately refused, including when blockSize is accepted.
                    int offsetAt = AudioAiffSsndCases.BlockOffset(kind) - 4; input[offsetAt + 3] = 1;
                    try { read.Invoke(null, new object[] { input, kind == 3 }); }
                    catch (TargetInvocationException error) { if (error.InnerException is NotSupportedException) { ++records; continue; } throw; }
                    throw new Exception("Nonzero SSND offset was silently accepted");
                }
            }
            if (records != 168) throw new Exception("Incomplete SSND metadata checks");
            Console.WriteLine("PASS SSND blockSize:168 exact sample/metadata invariants and168 nonzero-offset refusals"); return 0;
        } catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
