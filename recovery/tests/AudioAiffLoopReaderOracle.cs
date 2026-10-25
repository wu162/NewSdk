using System;
using System.IO;
using System.Reflection;

// Candidate boundary checks; original behavioral equivalence is established
// separately by AudioAiffLoopCases through the actual plugin entry point.
internal static class AudioAiffLoopReaderOracle
{
    private static MethodInfo read;
    private static int count;

    private static void BE(byte[] bytes, int at, uint value, int width)
    { for (int i = width - 1; i >= 0; --i) { bytes[at + i] = (byte)value; value >>= 8; } }

    private static void Accept(byte[] bytes, bool expected, int start, int end)
    {
        object[] arguments = { bytes, -1, -1 };
        bool result = (bool)read.Invoke(null, arguments);
        if (result != expected || (result && ((int)arguments[1] != start || (int)arguments[2] != end)))
            throw new Exception("AIFF loop reader result differs");
        ++count;
    }

    private static void Reject(byte[] bytes, Type expected)
    {
        try { read.Invoke(null, new object[] { bytes, -1, -1 }); }
        catch (TargetInvocationException error)
        {
            if (error.InnerException != null && error.InnerException.GetType() == expected)
            { ++count; return; }
            throw;
        }
        throw new Exception("Required AIFF loop rejection absent: " + expected.FullName);
    }

    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 1) throw new ArgumentException("Usage: AudioAiffLoopReaderOracle assembly.dll");
            Type type = Assembly.LoadFrom(Path.GetFullPath(args[0])).GetType(
                "BinaryAssetBuilder.AudioCompiler.AudioAiffLoopReader", true);
            read = type.GetMethod("TryRead", BindingFlags.Static | BindingFlags.NonPublic);
            string file = Path.GetTempFileName();
            byte[] valid, reverse;
            try
            {
                AudioAiffLoopCases.Write(file, 16, 2, 4096, 129, 4094, false);
                valid = File.ReadAllBytes(file);
                AudioAiffLoopCases.Write(file, 16, 2, 4096, 129, 4094, true);
                reverse = File.ReadAllBytes(file);
            }
            finally { File.Delete(file); }
            Accept(valid, true, 129, 4094);
            Accept(reverse, true, 129, 4094);
            Accept(new byte[0], false, -1, -1);
            byte[] plain = new byte[valid.Length - 56];
            Array.Copy(valid, plain, plain.Length); BE(plain, 4, (uint)(plain.Length - 8), 4);
            Accept(plain, false, -1, -1);
            int instrument = valid.Length - 48, markers = valid.Length - 20;
            byte[] altered = (byte[])valid.Clone(); BE(altered, instrument + 8, 2, 2);
            Accept(altered, false, -1, -1);
            altered = (byte[])valid.Clone(); BE(altered, instrument + 14, 1, 2);
            Accept(altered, true, 129, 4094);
            altered = (byte[])valid.Clone(); BE(altered, instrument + 10, 0, 2);
            Reject(altered, typeof(NotSupportedException));
            altered = (byte[])valid.Clone(); BE(altered, markers + 2, 11, 2);
            Reject(altered, typeof(NotSupportedException));
            altered = (byte[])valid.Clone(); BE(altered, markers + 10, 99, 2);
            Reject(altered, typeof(NotSupportedException));
            altered = (byte[])valid.Clone(); altered[markers + 8] = 255;
            Reject(altered, typeof(InvalidDataException));
            altered = (byte[])valid.Clone(); BE(altered, markers + 12, 129, 4);
            Reject(altered, typeof(NotSupportedException));
            altered = (byte[])valid.Clone(); BE(altered, markers + 4, uint.MaxValue, 4);
            Reject(altered, typeof(NotSupportedException));
            altered = new byte[valid.Length - 28];
            Array.Copy(valid, altered, altered.Length); BE(altered, 4, (uint)(altered.Length - 8), 4);
            Reject(altered, typeof(NotSupportedException));
            altered = new byte[valid.Length - 28];
            Array.Copy(valid, altered, plain.Length);
            Array.Copy(valid, valid.Length - 28, altered, plain.Length, 28);
            BE(altered, 4, (uint)(altered.Length - 8), 4);
            Reject(altered, typeof(NotSupportedException));
            altered = (byte[])valid.Clone(); BE(altered, markers, 3, 2);
            Reject(altered, typeof(InvalidDataException));
            altered = (byte[])valid.Clone(); BE(altered, 4, uint.MaxValue, 4);
            Reject(altered, typeof(InvalidDataException));
            Console.WriteLine("PASS AIFF loop candidate safety: " + count + " boundary checks; separate from original equivalence");
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
