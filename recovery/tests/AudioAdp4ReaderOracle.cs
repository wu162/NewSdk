using System;
using System.IO;
using System.Reflection;

internal static class AudioAdp4ReaderOracle
{
    private static MethodInfo read, decode;
    private static int valid, refused;
    private static void Check(string path, int bits, int channels, int physicalFrames, int declared, int delta)
    {
        AudioAdp4InputCases.Write(path, bits, channels, physicalFrames, 3, declared, delta); byte[] data = File.ReadAllBytes(path);
        object input = read.Invoke(null, new object[] { data, false }); BindingFlags fields = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        Type type = input.GetType(); float[] samples = (float[])type.GetField("Samples", fields).GetValue(input);
        int frames = (bits / 8 * channels * physicalFrames + delta) / (bits / 8 * channels);
        if ((int)type.GetField("Channels", fields).GetValue(input) != channels || (int)type.GetField("SampleRate", fields).GetValue(input) != 22050 || samples.Length != frames * channels)
            throw new Exception("ADP4 metadata/frame correction differs");
        short[] expected = (short[])decode.Invoke(null, new object[] { data, 60, channels, frames });
        for (int i = 0; i < samples.Length; ++i)
            if (BitConverter.ToInt32(BitConverter.GetBytes(samples[i]), 0) != BitConverter.ToInt32(BitConverter.GetBytes(expected[i] * (1.0f / 32768.0f)), 0))
                throw new Exception("ADP4 normalized sample bits differ");
        ++valid;
    }
    private static void Reject(string path, int bits, int channels, int frames)
    {
        AudioAdp4InputCases.Write(path, bits, channels, frames, 3, frames);
        try { read.Invoke(null, new object[] { File.ReadAllBytes(path), false }); }
        catch (TargetInvocationException error) { if (error.InnerException is NotSupportedException) { ++refused; return; } throw; }
        throw new Exception("Undefined/empty ADP4 input accepted");
    }
    private static int Main(string[] args)
    {
        try {
            Assembly library = Assembly.LoadFrom(Path.GetFullPath(args[0]));
            read = library.GetType("BinaryAssetBuilder.AudioCompiler.AudioPcmInputReader", true).GetMethod("Read", BindingFlags.Static | BindingFlags.NonPublic);
            decode = library.GetType("BinaryAssetBuilder.AudioCompiler.Adp4Decoder", true).GetMethod("Decode");
            string path = Path.Combine(args[1], "adp4-reader.aifc");
            foreach (int bits in new int[] { 8, 16, 24 }) foreach (int channels in new int[] { 1, 2 }) {
                foreach (int frames in new int[] { 2, 7, 129, 1023, 1024, 1025, 2041 })
                foreach (int declared in new int[] { 0, frames, frames + 99 }) Check(path, bits, channels, frames, declared, 0);
                foreach (int delta in new int[] { -1, 1, bits / 8 * channels + 1 }) Check(path, bits, channels, 129, 0, delta);
                Reject(path, bits, channels, 0); Reject(path, bits, channels, 1);
            }
            if (valid != 144 || refused != 12) throw new Exception("Incomplete ADP4 reader checks");
            Console.WriteLine("PASS ADP4 reader:144 exact normalized/frame invariants,12 empty/single-frame safety refusals"); return 0;
        } catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
