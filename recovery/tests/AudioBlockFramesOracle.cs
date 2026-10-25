using System;
using System.IO;
using System.Reflection;

// Expected scalar values execute the original fdivr/fimul/_ftol machine code.
internal static class AudioBlockFramesOracle
{
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 3) throw new Exception("Usage: AudioBlockFramesOracle recovered.dll original.tsv PCM|XAS");
            Type type = Assembly.LoadFrom(Path.GetFullPath(args[0])).GetType("BinaryAssetBuilder.AudioCompiler.Pcm16Encoder", true);
            MethodInfo method = type.GetMethod("NativeBlockFrames", BindingFlags.NonPublic | BindingFlags.Static);
            int count = 0;
            bool rate400 = false, rate431 = false;
            foreach (string line in File.ReadAllLines(args[1]))
            {
                if (line.StartsWith("#")) continue;
                string[] f = line.Split('\t');
                if (f.Length != 5) throw new Exception("Malformed original block-size evidence");
                if (f[0] != args[2]) continue;
                byte[] bits = new byte[4];
                for (int i = 0; i < bits.Length; ++i) bits[i] = Convert.ToByte(f[3].Substring(i * 2, 2), 16);
                int rate = Int32.Parse(f[2]), expected = Int32.Parse(f[4]);
                int actual = (int)method.Invoke(null, new object[] { BitConverter.ToSingle(bits, 0), rate });
                if (actual != expected) throw new Exception(f[0] + " channels=" + f[1] + " rate=" + rate + " frames expected=" + expected + " actual=" + actual);
                if (rate == 400) rate400 = true;
                if (rate == 431) rate431 = true;
                ++count;
            }
            if (count != 504 || !rate400 || !rate431) throw new Exception("Incomplete block-size matrix: " + count);
            Console.WriteLine("PASS " + args[2] + " native block-size differential: " + count + " original x87 53-bit cases including 400/431 Hz");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
