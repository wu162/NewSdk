using System;
using System.IO;
using System.Reflection;

internal static class AudioAiffMarkerRangeReaderOracle
{
    private static MethodInfo read;
    private static int checks;
    private static void Accept(byte[] bytes, bool expected, int first, int last)
    {
        object[] args = { bytes, -1, -1 }; bool actual = (bool)read.Invoke(null, args);
        if (actual != expected || (actual && ((int)args[1] != first || (int)args[2] != last))) throw new Exception("COMM/marker range differs from independent expectation");
        ++checks;
    }
    private static void Reject(byte[] bytes)
    {
        try { read.Invoke(null, new object[] { bytes, -1, -1 }); }
        catch (TargetInvocationException error) { if (error.InnerException is NotSupportedException) { ++checks; return; } throw; }
        throw new Exception("Unsafe partial marker range was accepted");
    }
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 2) throw new ArgumentException("Usage: AudioAiffMarkerRangeReaderOracle assembly fixtures");
            read = Assembly.LoadFrom(Path.GetFullPath(args[0])).GetType("BinaryAssetBuilder.AudioCompiler.AudioAiffLoopReader", true).GetMethod("TryRead", BindingFlags.Static | BindingFlags.NonPublic);
            foreach (string format in new string[] { "aiff", "aifc", "adp4" })
            foreach (int bits in new int[] { 8, 16, 24 }) foreach (int channels in new int[] { 1, 2 })
            {
                foreach (string variant in AudioAiffMarkerRangeCases.Variants)
                {
                    string path = Path.Combine(args[1], "markrange-" + format + "-" + bits + "-" + channels + "-" + variant + ".aiff");
                    bool expected = AudioAiffMarkerRangeCases.HasLoop(variant);
                    Accept(File.ReadAllBytes(path), expected, AudioAiffMarkerRangeCases.First(variant), AudioAiffMarkerRangeCases.Last(variant));
                    Accept(File.ReadAllBytes(path + ".canonical"), expected, AudioAiffMarkerRangeCases.First(variant), AudioAiffMarkerRangeCases.Last(variant));
                }
                byte[] valid = File.ReadAllBytes(Path.Combine(args[1], "markrange-" + format + "-" + bits + "-" + channels + "-declared257.aiff"));
                // A complete valid pair becomes a partial native pair after
                // signed COMM filtering. These cases never run in the native host.
                foreach (uint declared in new uint[] { 17, 222 })
                { byte[] changed = (byte[])valid.Clone(); AudioAiffMarkerRangeCases.Set32(changed, 22, declared); Reject(changed); }
                int markers = valid.Length - 18;
                byte[] firstInvalid = (byte[])valid.Clone(); AudioAiffMarkerRangeCases.Set32(firstInvalid, markers + 4, uint.MaxValue); Reject(firstInvalid);
                byte[] endInvalid = (byte[])valid.Clone(); AudioAiffMarkerRangeCases.Set32(endInvalid, markers + 12, 0x80000000u); Reject(endInvalid);
            }
            if (checks != 720) throw new Exception("Missing COMM/marker reader coverage: " + checks);
            Console.WriteLine("PASS COMM/marker reader: " + checks + " independent range and partial-pair safety assertions"); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
