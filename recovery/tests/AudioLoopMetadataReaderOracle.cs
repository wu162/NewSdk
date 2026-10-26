using System;
using System.IO;
using System.Reflection;

// Independent expected parser ranges, plus bounded safety refusals. Full
// original parity is tested by AudioLoopMetadataOracle, not these assertions.
internal static class AudioLoopMetadataReaderOracle
{
    private static int checks;
    private static MethodInfo aiff, wave;
    private static void Read(byte[] bytes, bool isWave, bool expected)
    {
        object[] args = { bytes, -1, -1 };
        bool actual = (bool)(isWave ? wave : aiff).Invoke(null, args);
        if (actual != expected || (actual && ((int)args[1] != 17 || (int)args[2] != 222)))
            throw new Exception("Loop metadata parser differs from independent expected range");
        ++checks;
    }
    private static void Reject(byte[] bytes, Type expected)
    {
        try { aiff.Invoke(null, new object[] { bytes, -1, -1 }); }
        catch (TargetInvocationException error)
        {
            if (error.InnerException != null && error.InnerException.GetType() == expected) { ++checks; return; }
            throw;
        }
        throw new Exception("Required loop metadata safety refusal absent: " + expected.FullName);
    }
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 2) throw new ArgumentException("Usage: AudioLoopMetadataReaderOracle assembly fixtures");
            Assembly library = Assembly.LoadFrom(Path.GetFullPath(args[0]));
            aiff = library.GetType("BinaryAssetBuilder.AudioCompiler.AudioAiffLoopReader", true).GetMethod("TryRead", BindingFlags.NonPublic | BindingFlags.Static);
            wave = library.GetType("BinaryAssetBuilder.AudioCompiler.LoopPcmEncoder", true).GetMethod("TryReadWaveLoop", BindingFlags.NonPublic | BindingFlags.Static);
            foreach (string format in new string[] { "wave", "aiff", "aifc", "adp4" })
            foreach (int bits in new int[] { 8, 16, 24 })
            foreach (int channels in new int[] { 1, 2 })
            foreach (string variant in format == "wave" ? AudioLoopMetadataCases.WaveVariants : AudioLoopMetadataCases.AiffVariants)
            {
                string path = Path.Combine(args[1], "loopmeta-" + format + "-" + bits + "-" + channels + "-" + variant + "." + (format == "wave" ? "wav" : "aiff"));
                bool expected = variant != "mode0" && variant != "first-zero" && (variant != "mode2" || format == "wave");
                // Native smpl lookup does not add RIFF odd-byte padding.
                if (format == "wave" && channels == 1 && bits != 16) expected = false;
                Read(File.ReadAllBytes(path), format == "wave", expected);
                Read(File.ReadAllBytes(path + ".canonical"), format == "wave", expected);
            }
            if (checks != 528) throw new Exception("Missing loop metadata fixture coverage");
            // A smpl placed before the same odd-sized data remains visible:
            // the regression must not disable all mono PCM8/24 WAVE looping.
            foreach (int bits in new int[] { 8, 24 })
            {
                byte[] odd = File.ReadAllBytes(Path.Combine(args[1], "loopmeta-wave-" + bits + "-1-canonical.wav"));
                int sampler = odd.Length - 68;
                byte[] shifted = new byte[odd.Length];
                Array.Copy(odd, 0, shifted, 0, 36);
                Array.Copy(odd, sampler, shifted, 36, 68);
                Array.Copy(odd, 36, shifted, 104, sampler - 36);
                Read(shifted, true, true);
            }
            byte[] valid = File.ReadAllBytes(Path.Combine(args[1], "loopmeta-aiff-16-1-canonical.aiff"));
            // Final MARK has two eight-byte records: count at length-18.
            int count = valid.Length - 18;
            byte[] changed = (byte[])valid.Clone(); changed[count] = 0; changed[count + 1] = 201;
            Reject(changed, typeof(NotSupportedException));
            changed = (byte[])valid.Clone(); changed[count + 1] = 1;
            Reject(changed, typeof(NotSupportedException));
            changed = (byte[])valid.Clone(); changed[count + 8] = 255;
            Reject(changed, typeof(InvalidDataException));
            changed = (byte[])valid.Clone(); changed[count + 1] = 3;
            Reject(changed, typeof(InvalidDataException));
            Console.WriteLine("PASS loop metadata reader: " + checks + " independent range and safety assertions"); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
