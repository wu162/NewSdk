using System;
using System.IO;
using System.Reflection;

// Candidate safety boundaries are deliberately separate from original-success
// transcripts: rejecting unrecovered input must never count as native parity.
internal static class AudioInputReaderOracle
{
    private static MethodInfo read;
    private static int count;
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
            copy = (byte[])valid.Clone(); copy[11] = (byte)'C'; Reject("AIFC", copy);
            copy = (byte[])valid.Clone(); copy[49] = 1; Reject("AIFF-SSND-offset", copy);
            copy = (byte[])valid.Clone(); copy[53] = 1; Reject("AIFF-SSND-block-size", copy);
            copy = (byte[])valid.Clone(); copy[38] = (byte)'I'; copy[39] = (byte)'N'; copy[40] = (byte)'S'; copy[41] = (byte)'T'; Reject("AIFF-loop", copy);
            AudioInputFormatCases.WriteExtensible(path, 24, 2, 129, true); valid = File.ReadAllBytes(path);
            copy = (byte[])valid.Clone(); copy[36] = 0; Reject("extensible-missing-extension", copy);
            copy = (byte[])valid.Clone(); copy[36] = 24; Reject("extensible-extra-extension", copy);
            copy = (byte[])valid.Clone(); copy[38] = 20; Reject("extensible-partial-validbits", copy);
            copy = (byte[])valid.Clone(); copy[44] = 3; Reject("extensible-float-GUID", copy);
            copy = (byte[])valid.Clone(); copy[59] ^= 1; Reject("extensible-unknown-GUID", copy);
            copy = (byte[])valid.Clone(); copy[40] = 4; Reject("extensible-nonstandard-mask", copy);
            Console.WriteLine("PASS Audio input rejection boundaries: " + count); return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
