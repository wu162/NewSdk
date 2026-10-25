using System;
using System.IO;
using System.Reflection;

internal static class AudioAifcReaderOracle
{
    private static MethodInfo read;
    private static int equivalent, refused;
    private static void Same(byte[] plain, byte[] compressed, string name)
    {
        object a = read.Invoke(null, new object[] { plain, false });
        object b = read.Invoke(null, new object[] { compressed, false });
        Type type = a.GetType(); BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        foreach (string field in new string[] { "Channels", "SampleRate" })
            if (!type.GetField(field, flags).GetValue(a).Equals(type.GetField(field, flags).GetValue(b))) throw new Exception("AIFC import changed " + field + ":" + name);
        float[] af = (float[])type.GetField("Samples", flags).GetValue(a), bf = (float[])type.GetField("Samples", flags).GetValue(b);
        if (af.Length != bf.Length) throw new Exception("AIFC import changed frame count:" + name);
        byte[] ab = new byte[af.Length * 4], bb = new byte[bf.Length * 4];
        Buffer.BlockCopy(af, 0, ab, 0, ab.Length); Buffer.BlockCopy(bf, 0, bb, 0, bb.Length);
        for (int i = 0; i < ab.Length; ++i) if (ab[i] != bb[i]) throw new Exception("AIFC sample bits changed:" + name);
        ++equivalent;
    }
    private static void Reject(byte[] bytes, string name)
    {
        try { read.Invoke(null, new object[] { bytes, false }); }
        catch (TargetInvocationException error) {
            if (error.InnerException is NotSupportedException || error.InnerException is InvalidDataException) { ++refused; return; }
            throw;
        }
        throw new Exception("AIFC safety refusal absent:" + name);
    }
    private static int Main(string[] args)
    {
        try {
            read = Assembly.LoadFrom(Path.GetFullPath(args[0])).GetType("BinaryAssetBuilder.AudioCompiler.AudioPcmInputReader", true).GetMethod("Read", BindingFlags.Static | BindingFlags.NonPublic);
            string file = Path.Combine(args[1], "aifc-reader-boundary.aifc");
            foreach (int bits in new int[] { 8, 16, 24 }) foreach (int channels in new int[] { 1, 2 }) {
                AudioInputFormatCases.Write(file, true, bits, channels, 129); byte[] plain = File.ReadAllBytes(file);
                foreach (string variant in AudioAifcInputCases.Variants) {
                    AudioAifcInputCases.Write(file, bits, channels, 129, variant, "NONE");
                    Same(plain, File.ReadAllBytes(file), bits + "-" + channels + "-" + variant);
                }
            }
            foreach (string codec in new string[] { "sowt", "fl32", "????" }) {
                AudioAifcInputCases.Write(file, 16, 1, 129, "empty", codec); Reject(File.ReadAllBytes(file), "codec-" + codec);
            }
            AudioAifcInputCases.Write(file, 16, 1, 1, "empty", "ADP4"); Reject(File.ReadAllBytes(file), "ADP4-undefined-single-frame");
            AudioAifcInputCases.Write(file, 16, 1, 129, "empty", "NONE"); byte[] valid = File.ReadAllBytes(file), altered;
            altered = (byte[])valid.Clone(); altered[19] = 20; Reject(altered, "short-COMM");
            altered = (byte[])valid.Clone(); altered[27] = 32; Reject(altered, "container32");
            altered = (byte[])valid.Clone(); altered[21] = 3; Reject(altered, "three-channel");
            // First SSND follows FORM12 + COMM header8 + body24.
            altered = (byte[])valid.Clone(); altered[55] = 1; Reject(altered, "nonzero-offset");
            altered = (byte[])valid.Clone(); altered[48] = altered[49] = altered[50] = 0; altered[51] = 7; Reject(altered, "truncated-SSND-prefix");
            altered = (byte[])valid.Clone(); altered[44] = (byte)'I'; altered[45] = (byte)'N'; altered[46] = (byte)'S'; altered[47] = (byte)'T'; Reject(altered, "loop-not-enabled");
            if (equivalent != 60 || refused != 10) throw new Exception("Incomplete AIFC reader boundary coverage");
            Console.WriteLine("PASS AIFC reader:60 sample-bit invariants,10 safety refusals"); return 0;
        } catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
