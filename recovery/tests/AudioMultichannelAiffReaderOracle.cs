using System;
using System.IO;
using System.Reflection;

// Independent source ordering, exact SSND windows, COMM filtering and safety.
internal static class AudioMultichannelAiffReaderOracle
{
    private static MethodInfo pcm, loop, resample;
    private static int ordering, windows, markers, refusals;
    private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static float[] Samples(object input) { return (float[])input.GetType().GetField("Samples", Flags).GetValue(input); }
    private static void Reject(MethodInfo method, object[] args)
    {
        try { method.Invoke(null, args); }
        catch (TargetInvocationException error) { if (error.InnerException is NotSupportedException) { ++refusals; return; } throw; }
        throw new Exception("Unsafe AIFF multichannel input accepted");
    }
    private static int Main(string[] args)
    {
        try {
            Assembly library = Assembly.LoadFrom(Path.GetFullPath(args[0]));
            pcm = library.GetType("BinaryAssetBuilder.AudioCompiler.AudioPcmInputReader", true).GetMethod("Read", BindingFlags.Static | BindingFlags.NonPublic);
            loop = library.GetType("BinaryAssetBuilder.AudioCompiler.AudioAiffLoopReader", true).GetMethod("TryRead", BindingFlags.Static | BindingFlags.NonPublic);
            resample = library.GetType("BinaryAssetBuilder.AudioCompiler.AudioResampling", true).GetMethod("Apply", BindingFlags.Static | BindingFlags.NonPublic);
            Directory.CreateDirectory(args[1]); string file = Path.Combine(args[1], "aiff-reader.aiff");
            foreach (string format in new[] { "aiff","aifc" }) foreach (int channels in new[] { 4,6 }) foreach (int bits in new[] { 8,16,24 }) {
                AudioMultichannelAiffCases.Write(file, format.ToUpperInvariant(), bits, channels, 129, -1);
                byte[] bytes = File.ReadAllBytes(file);
                object input = pcm.Invoke(null, new object[] { bytes, false }); float[] actual = Samples(input);
                int at = AudioAiffOffsetCases.Sound(bytes) + 16, width = bits / 8;
                if (actual.Length != 129 * channels || (int)input.GetType().GetField("Channels", Flags).GetValue(input) != channels)
                    throw new Exception("Source metadata differs");
                for (int i = 0; i < actual.Length; ++i, at += width) {
                    float expected = bits == 8 ? unchecked((sbyte)bytes[at]) / 128f : unchecked((short)((bytes[at] << 8) | bytes[at + 1])) / 32768f;
                    if (actual[i] != expected) throw new Exception("AIFF source channel order or integer conversion differs at sample " + i);
                    ++ordering;
                }
                int frameSize = channels * width;
                foreach (uint block in new[] { 0u,uint.MaxValue }) {
                    foreach (int offset in new[] { 0,1,frameSize - 1 }) {
                        bytes = AudioAiffOffsetCases.Create(file, format == "aiff" ? 0 : 1, bits, channels, 129, frameSize - 1, (uint)offset, block);
                        byte[] unchanged = (byte[])bytes.Clone();
                        float[] a = Samples(pcm.Invoke(null, new object[] { bytes,false }));
                        float[] b = Samples(pcm.Invoke(null, new object[] { AudioAiffOffsetCases.Canonical(bytes,129 * frameSize),false }));
                        if (a.Length != 129 * channels || a.Length != b.Length) throw new Exception("Raw SSND frame count differs");
                        for (int i = 0; i < a.Length; ++i) if (a[i] != b[i]) throw new Exception("SSND canonical sample differs");
                        for (int i = 0; i < bytes.Length; ++i) if (bytes[i] != unchanged[i]) throw new Exception("Reader mutated input");
                        ++windows;
                    }
                    foreach (uint offset in new[] { (uint)frameSize, (uint)(129 * frameSize + frameSize - 1), uint.MaxValue }) {
                        bytes = AudioAiffOffsetCases.Create(file, format == "aiff" ? 0 : 1, bits, channels, 129, frameSize - 1, offset, block);
                        Reject(pcm, new object[] { bytes,false });
                    }
                }
                foreach (string variant in AudioAiffMarkerRangeCases.Variants) foreach (bool canonical in new[] { false,true }) {
                    AudioAiffMarkerRangeCases.Write(file, format,bits,channels,variant,canonical);
                    object[] values = { File.ReadAllBytes(file),-1,-1 };
                    bool result = (bool)loop.Invoke(null, values), expected = AudioAiffMarkerRangeCases.HasLoop(variant);
                    if (result != expected || (result && ((int)values[1] != AudioAiffMarkerRangeCases.First(variant) || (int)values[2] != AudioAiffMarkerRangeCases.Last(variant))))
                        throw new Exception("COMM independent marker expectation differs: " + variant);
                    ++markers;
                }
                AudioAiffMarkerRangeCases.Write(file,format,bits,channels,"declared257",false); bytes = File.ReadAllBytes(file);
                foreach (uint declared in new[] { 17u,222u }) {
                    byte[] partial = (byte[])bytes.Clone(); AudioAiffMarkerRangeCases.Set32(partial,22,declared);
                    Reject(loop,new object[] { partial,-1,-1 });
                }
                // Single-frame native import remains undefined during resampling.
                if (format == "aiff") AudioInputFormatCases.Write(file,true,bits,channels,1);
                else AudioAifcInputCases.Write(file,bits,channels,1,"empty","NONE");
                input = pcm.Invoke(null,new object[] { File.ReadAllBytes(file),false });
                Reject(resample,new object[] { input,11025,false,-1,-1 });
            }
            if (ordering != 7740 || windows != 72 || markers != 432 || refusals != 108)
                throw new Exception("Incomplete independent checks " + ordering + "/" + windows + "/" + markers + "/" + refusals);
            Console.WriteLine("PASS AIFF4/6:7740 source-order samples,72 exact-window invariants,432 marker expectations,108 safety refusals"); return 0;
        } catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
