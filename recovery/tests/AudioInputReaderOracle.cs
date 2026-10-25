using System;
using System.IO;
using System.Reflection;

// Candidate safety boundaries are deliberately separate from original-success
// transcripts: rejecting unrecovered input must never count as native parity.
internal static class AudioInputReaderOracle
{
    private static MethodInfo read;
    private static int count, accepted;
    private static void Reject(string name, byte[] data)
    {
        try { read.Invoke(null, new object[] { data, false }); }
        catch (TargetInvocationException ex)
        {
            if (ex.InnerException is NotSupportedException || ex.InnerException is InvalidDataException || ex.InnerException is ArgumentNullException)
            { Console.WriteLine("PASS reject " + name); ++count; return; }
            throw;
        }
        throw new Exception("Unsupported input accepted: " + name);
    }
    private static void Equivalent(string name, byte[] canonical, byte[] altered)
    {
        object expected = read.Invoke(null, new object[] { canonical, false });
        object actual = read.Invoke(null, new object[] { altered, false });
        Type type = expected.GetType();
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        foreach (string field in new string[] { "Channels", "SampleRate" })
            if (!type.GetField(field, flags).GetValue(expected).Equals(type.GetField(field, flags).GetValue(actual)))
                throw new Exception("Ignored WAVE metadata changed " + field + ": " + name);
        float[] a = (float[])type.GetField("Samples", flags).GetValue(expected);
        float[] b = (float[])type.GetField("Samples", flags).GetValue(actual);
        if (a.Length != b.Length) throw new Exception("Ignored WAVE metadata changed frame count: " + name);
        byte[] ab = new byte[a.Length * 4], bb = new byte[b.Length * 4];
        Buffer.BlockCopy(a, 0, ab, 0, ab.Length); Buffer.BlockCopy(b, 0, bb, 0, bb.Length);
        for (int i = 0; i < ab.Length; ++i) if (ab[i] != bb[i])
            throw new Exception("Ignored WAVE metadata changed sample bits: " + name);
        Console.WriteLine("PASS ignored extension " + name); ++accepted;
    }

    private static int Main(string[] args)
    {
        try
        {
            Assembly library = Assembly.LoadFrom(Path.GetFullPath(args[0]));
            read = library.GetType("BinaryAssetBuilder.AudioCompiler.AudioPcmInputReader", true).GetMethod("Read", BindingFlags.Static | BindingFlags.NonPublic);
            string path = Path.Combine(args[1], "reader-boundary.wav");
            AudioInputFormatCases.Write(path, false, 24, 1, 129);
            byte[] valid = File.ReadAllBytes(path), copy;
            Reject("null", null); Reject("truncated", new byte[11]);
            copy = (byte[])valid.Clone(); copy[20] = 3; Reject("IEEE-float", copy);
            copy = (byte[])valid.Clone(); copy[34] = 32; Reject("PCM32", copy);
            copy = (byte[])valid.Clone(); copy[20] = 0xfe; copy[21] = 0xff; Reject("truncated-extensible-format", copy);
            copy = (byte[])valid.Clone(); copy[22] = 3; Reject("three-channels", copy);
            copy = (byte[])valid.Clone(); copy[32] = 4; Reject("block-alignment", copy);
            copy = (byte[])valid.Clone(); copy[4] = 0xff; copy[5] = 0xff; Reject("chunk-overrun", copy);
            copy = (byte[])valid.Clone(); copy[36] = (byte)'s'; copy[37] = (byte)'m'; copy[38] = (byte)'p'; copy[39] = (byte)'l'; Reject("WAVE-loop", copy);
            AudioInputFormatCases.Write(path, true, 16, 1, 129); valid = File.ReadAllBytes(path);
            copy = (byte[])valid.Clone(); copy[11] = (byte)'C'; Reject("truncated-AIFC-COMM", copy);
            copy = (byte[])valid.Clone(); copy[49] = 1; Reject("AIFF-SSND-offset", copy);
            copy = (byte[])valid.Clone(); copy[53] = 1; Reject("AIFF-SSND-block-size", copy);
            copy = (byte[])valid.Clone(); copy[38] = (byte)'I'; copy[39] = (byte)'N'; copy[40] = (byte)'S'; copy[41] = (byte)'T'; Reject("AIFF-loop", copy);
            AudioInputFormatCases.WriteExtensible(path, 24, 2, 129, true); valid = File.ReadAllBytes(path);
            copy = (byte[])valid.Clone(); copy[16] = 38; Reject("extensible-short38", copy);
            copy = (byte[])valid.Clone(); copy[16] = 39; Reject("extensible-short39", copy);
            foreach (int bits in new int[] { 8, 16, 24 })
            foreach (int channels in new int[] { 1, 2 })
            {
                AudioInputFormatCases.WriteExtensible(path, bits, channels, 129, true);
                byte[] canonical = File.ReadAllBytes(path);
                foreach (string variant in AudioInputFormatCases.MetadataVariants)
                {
                    AudioInputFormatCases.WriteMetadataExtension(path, bits, channels, 129, variant);
                    Equivalent(bits + "-" + channels + "-" + variant, canonical, File.ReadAllBytes(path));
                }
            }
            Console.WriteLine("PASS Audio ignored-extension sample invariants: " + accepted);
            Console.WriteLine("PASS Audio input rejection boundaries: " + count); return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
