using System;
using System.IO;
using System.Reflection;

// Candidate safety contract, separate from original-DLL byte equivalence.
internal static class AudioLoopReaderOracle
{
    private static MethodInfo read, encode;
    private static int count;

    private static void LE(byte[] bytes, int at, uint value)
    {
        for (int i = 0; i < 4; ++i) bytes[at + i] = (byte)(value >> (8 * i));
    }

    private static void Accept(byte[] bytes, bool expected, int first, int last)
    {
        object[] args = { bytes, -1, -1 };
        bool result = (bool)read.Invoke(null, args);
        if (result != expected || (result && ((int)args[1] != first || (int)args[2] != last)))
            throw new Exception("Loop reader result differs");
        ++count;
    }

    private static void Reject(Action action, Type exception)
    {
        try { action(); }
        catch (TargetInvocationException error)
        {
            if (error.InnerException != null && error.InnerException.GetType() == exception)
            { ++count; return; }
            throw;
        }
        throw new Exception("Required loop rejection was absent: " + exception.FullName);
    }

    private static void RejectRead(byte[] bytes, Type exception)
    {
        Reject(delegate { read.Invoke(null, new object[] { bytes, -1, -1 }); }, exception);
    }

    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 1) throw new ArgumentException("Usage: AudioLoopReaderOracle assembly.dll");
            Type loop = Assembly.LoadFrom(Path.GetFullPath(args[0])).GetType(
                "BinaryAssetBuilder.AudioCompiler.LoopPcmEncoder", true);
            read = loop.GetMethod("TryReadWaveLoop", BindingFlags.Static | BindingFlags.NonPublic);
            encode = loop.GetMethod("EncodeSamples", BindingFlags.Static | BindingFlags.NonPublic);
            string file = Path.GetTempFileName();
            byte[] valid;
            try
            {
                AudioLoopProbeCases.Wave(file, 1, 4096, 129, 4094);
                valid = File.ReadAllBytes(file);
            }
            finally { File.Delete(file); }
            Accept(valid, true, 129, 4094);
            Accept(new byte[0], false, -1, -1);
            byte[] noSampler = new byte[valid.Length - 68];
            Array.Copy(valid, noSampler, noSampler.Length);
            LE(noSampler, 4, (uint)(noSampler.Length - 8));
            Accept(noSampler, false, -1, -1);
            int sampler = valid.Length - 68;
            byte[] changed = (byte[])valid.Clone(); LE(changed, sampler + 36, 0);
            Accept(changed, false, -1, -1);
            changed = (byte[])valid.Clone(); LE(changed, sampler + 36, 2);
            Accept(changed, true, 129, 4094);
            changed = (byte[])valid.Clone(); LE(changed, sampler + 48, 1);
            Accept(changed, true, 129, 4094);
            changed = (byte[])valid.Clone(); LE(changed, sampler + 52, 5000);
            RejectRead(changed, typeof(NotSupportedException));
            changed = (byte[])valid.Clone(); LE(changed, sampler + 52, uint.MaxValue);
            RejectRead(changed, typeof(NotSupportedException));
            changed = (byte[])valid.Clone(); LE(changed, sampler + 4, 35);
            RejectRead(changed, typeof(InvalidDataException));
            changed = (byte[])valid.Clone(); LE(changed, sampler + 4, 59);
            RejectRead(changed, typeof(InvalidDataException));
            changed = (byte[])valid.Clone(); LE(changed, sampler + 4, uint.MaxValue);
            RejectRead(changed, typeof(InvalidDataException));
            changed = new byte[valid.Length + 68];
            Array.Copy(valid, changed, valid.Length);
            Array.Copy(valid, sampler, changed, valid.Length, 68);
            LE(changed, 4, (uint)(changed.Length - 8));
            Accept(changed, true, 129, 4094);
            Reject(delegate { encode.Invoke(null, new object[] {
                new float[4], 1, 22050, false, 0, 4, false }); }, typeof(NotSupportedException));
            Reject(delegate { encode.Invoke(null, new object[] {
                new float[4], 1, 22050, false, 3, 2, false }); }, typeof(NotSupportedException));
            Console.WriteLine("PASS loop candidate safety: " + count + " structural/unsupported/point-boundary checks; separate from original equivalence");
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
