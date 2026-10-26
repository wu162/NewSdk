using System;
using System.IO;
using System.Reflection;

internal static class AudioLoopMetadataOptionalReaderOracle
{
    private static MethodInfo read;
    private static int checks;
    private static void Accept(byte[] bytes, bool expected)
    {
        object[] args = { bytes, -1, -1 };
        bool actual = (bool)read.Invoke(null, args);
        if (actual != expected || (actual && ((int)args[1] != 17 || (int)args[2] != 222)))
            throw new Exception("Optional loop metadata range differs");
        ++checks;
    }
    private static void Reject(byte[] bytes)
    {
        try { read.Invoke(null, new object[] { bytes, -1, -1 }); }
        catch (TargetInvocationException error)
        {
            if (error.InnerException is NotSupportedException) { ++checks; return; }
            throw;
        }
        throw new Exception("Required partial/uninitialized loop metadata refusal absent");
    }
    private static void BE(byte[] bytes, int at, uint value, int width)
    { for (int i = width - 1; i >= 0; --i) { bytes[at + i] = (byte)value; value >>= 8; } }
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 2) throw new ArgumentException("Usage: AudioLoopMetadataOptionalReaderOracle assembly fixtures");
            read = Assembly.LoadFrom(Path.GetFullPath(args[0])).GetType("BinaryAssetBuilder.AudioCompiler.AudioAiffLoopReader", true).GetMethod("TryRead", BindingFlags.Static | BindingFlags.NonPublic);
            foreach (string format in new string[] { "aiff", "aifc", "adp4" })
            foreach (int bits in new int[] { 8, 16, 24 })
            foreach (int channels in new int[] { 1, 2 })
            foreach (string variant in AudioLoopMetadataOptionalCases.Variants)
            {
                string path = Path.Combine(args[1], "loopoptional-" + format + "-" + bits + "-" + channels + "-" + variant + ".aiff");
                bool expected = variant.StartsWith("aux-", StringComparison.Ordinal);
                Accept(File.ReadAllBytes(path), expected);
                Accept(File.ReadAllBytes(path + ".canonical"), expected);
            }
            if (checks != 324) throw new Exception("Missing optional metadata coverage");
            byte[] unmatched = File.ReadAllBytes(Path.Combine(args[1], "loopoptional-aiff-16-1-unmatched.aiff"));
            int count = unmatched.Length - 18;
            byte[] changed = (byte[])unmatched.Clone(); BE(changed, count + 2, 7, 2); Reject(changed);
            changed = (byte[])unmatched.Clone(); BE(changed, count + 10, 11, 2); Reject(changed);
            // Keep the bounded MARK but remove the preceding 28-byte INST.
            int prefix = unmatched.Length - 54;
            changed = new byte[unmatched.Length - 28];
            Array.Copy(unmatched, 0, changed, 0, prefix);
            Array.Copy(unmatched, prefix + 28, changed, prefix, 26);
            BE(changed, 4, (uint)(changed.Length - 8), 4); Reject(changed);
            byte[] valid = File.ReadAllBytes(Path.Combine(args[1], "loopoptional-aiff-16-1-aux-outside.aiff.canonical"));
            count = valid.Length - 18;
            changed = (byte[])valid.Clone(); BE(changed, count + 4, uint.MaxValue, 4); Reject(changed);
            Console.WriteLine("PASS optional loop metadata reader: " + checks + " independent range and safety assertions"); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
