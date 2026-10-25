using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Collections.Generic;

// Exact float bits from original DLL machine-code execution; no tolerance.
internal static class AudioResamplerFftOracle
{
    private static byte[] Raw(float[] values)
    {
        byte[] raw = new byte[checked(values.Length * 4)];
        Buffer.BlockCopy(values, 0, raw, 0, raw.Length);
        return raw;
    }

    private static float[] Floats(byte[] raw, int count)
    {
        if (raw.Length != count * 4) throw new Exception("Bad source float count");
        float[] result = new float[count];
        Buffer.BlockCopy(raw, 0, result, 0, raw.Length);
        return result;
    }

    private static void Equal(string label, byte[] expected, float[] values)
    {
        byte[] actual = Raw(values);
        if (expected.Length != actual.Length) throw new Exception(label + " output length differs");
        for (int i = 0; i < expected.Length; ++i)
            if (expected[i] != actual[i])
                throw new Exception(label + " float=" + (i / 4) + " byte=" + i +
                    " expected=" + expected[i] + " actual=" + actual[i]);
    }

    private static void Reject(MethodInfo method, object[] args, Type expected)
    {
        try { method.Invoke(null, args); }
        catch (TargetInvocationException error)
        {
            if (error.InnerException != null && error.InnerException.GetType() == expected) return;
            throw;
        }
        throw new Exception("Missing candidate safety rejection: " + expected.FullName);
    }

    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 2) throw new Exception("Usage: AudioResamplerFftOracle candidate.dll original.tsv.gz");
            Assembly assembly = Assembly.LoadFrom(Path.GetFullPath(args[0]));
            Type type = assembly.GetType("BinaryAssetBuilder.AudioCompiler.ResamplerFft", true);
            MethodInfo transform = type.GetMethod("Transform", BindingFlags.Public | BindingFlags.Static);
            int count = 0;
            HashSet<string> coverage = new HashSet<string>();
            using (FileStream file = File.OpenRead(args[1]))
            using (GZipStream gzip = new GZipStream(file, CompressionMode.Decompress))
            using (StreamReader reader = new StreamReader(gzip))
            {
                string line;
                bool fingerprint = false;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("#"))
                    {
                        fingerprint = line.Contains("originalDllSha256=a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139") &&
                            line.Contains("fpuControl=0x027f") && line.Contains("nativeRva=0x4eca0");
                        continue;
                    }
                    if (!fingerprint) throw new Exception("Original FFT fingerprint/FPU evidence missing");
                    string[] f = line.Split('\t');
                    if (f.Length != 7) throw new Exception("Malformed native FFT evidence");
                    int order = Int32.Parse(f[1]), direction = Int32.Parse(f[2]), n = 1 << order;
                    float[] real = Floats(Convert.FromBase64String(f[3]), n);
                    float[] imaginary = Floats(Convert.FromBase64String(f[4]), n);
                    transform.Invoke(null, new object[] { real, imaginary, order, direction });
                    string label = f[0] + " order=" + order + " direction=" + direction;
                    Equal(label + " real", Convert.FromBase64String(f[5]), real);
                    Equal(label + " imaginary", Convert.FromBase64String(f[6]), imaginary);
                    if (!coverage.Add(f[0] + ":" + order + ":" + direction)) throw new Exception("Duplicate FFT fixture");
                    ++count;
                }
            }
            for (int order = 1; order <= 13; ++order)
                for (int direction = 0; direction < 2; ++direction)
                    foreach (string pattern in new string[] { "impulse", "dc", "complex-noise" })
                        if (!coverage.Contains(pattern + ":" + order + ":" + direction))
                            throw new Exception("Incomplete FFT order/direction/pattern coverage");
            if (count != 78) throw new Exception("Expected 78 FFT fixtures; found " + count);
            Reject(transform, new object[] { new float[2], new float[2], 0, 0 }, typeof(ArgumentOutOfRangeException));
            Reject(transform, new object[] { new float[2], new float[2], 14, 0 }, typeof(ArgumentOutOfRangeException));
            Reject(transform, new object[] { new float[2], new float[2], 1, 2 }, typeof(ArgumentOutOfRangeException));
            Reject(transform, new object[] { null, new float[2], 1, 0 }, typeof(ArgumentNullException));
            Reject(transform, new object[] { new float[2], null, 1, 0 }, typeof(ArgumentNullException));
            Reject(transform, new object[] { new float[3], new float[2], 1, 0 }, typeof(ArgumentException));
            float[] aliased = new float[2];
            Reject(transform, new object[] { aliased, aliased, 1, 0 }, typeof(ArgumentException));
            Reject(transform, new object[] { new float[] { Single.NaN, 0 }, new float[2], 1, 0 }, typeof(ArgumentException));
            Reject(transform, new object[] { new float[2], new float[] { Single.PositiveInfinity, 0 }, 1, 0 }, typeof(ArgumentException));
            Console.WriteLine("PASS original native FFT bit differential: 78 cases, both directions, orders 1..13, impulse/DC/complex noise; 9 candidate safety checks; no resampling/SDK acceptance");
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
