using System;
using System.IO;
using System.Reflection;

// Compares the codec component with original AudioFile output; this is not plugin integration.
internal static class AudioPcmEncoderOracle
{
    private static int Word(byte[] data, int offset, bool big)
    {
        return big ? data[offset] << 24 | data[offset+1] << 16 | data[offset+2] << 8 | data[offset+3] : BitConverter.ToInt32(data, offset);
    }
    private static void Equal(string label, byte[] expected, byte[] actual)
    {
        if (actual == null || expected.Length != actual.Length) throw new Exception(label + " length " + expected.Length + " != " + (actual == null ? -1 : actual.Length));
        for (int i=0; i<expected.Length; i++) if (expected[i]!=actual[i]) throw new Exception(label + " byte " + i + " expected " + expected[i] + " actual " + actual[i]);
    }
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 4 || (args[3] != "PCM" && args[3] != "XAS")) throw new Exception("Usage: AudioPcmEncoderOracle candidate.dll pcm-original.txt original-wav-directory PCM|XAS");
            var assembly = Assembly.LoadFrom(Path.GetFullPath(args[0]));
            bool xas = args[3] == "XAS";
            var type = assembly.GetType("BinaryAssetBuilder.AudioCompiler." + (xas ? "Xas1Encoder" : "Pcm16Encoder"), true);
            var method = type.GetMethod("EncodeWave"); int count=0;
            foreach (string line in File.ReadAllLines(args[1]))
            {
                string[] fields=line.Split(':');
                if (fields.Length!=9 || fields[3]!="BUFFER" || fields[7]!="CUSTOM") throw new Exception("Original PCM evidence contains a failed case: " + fields[0]);
                string[] id=fields[0].Split('-');
                int channels=Int32.Parse(id[1]), frames=Int32.Parse(id[2]); bool streamed=Boolean.Parse(id[3]);
                byte[] wave=File.ReadAllBytes(Path.Combine(args[2], "pcm-"+channels+"-"+frames+".wav"));
                object result=method.Invoke(null,new object[]{wave,streamed});
                Type resultType=result.GetType();
                Func<string,object> get=name=> { var f=resultType.GetField(name); return f!=null?f.GetValue(result):resultType.GetProperty(name).GetValue(result,null); };
                byte[] original=Convert.FromBase64String(fields[4]);
                bool big=fields[1]!="0"; int pointer=Word(original,20,big), size=Word(original,24,big);
                if (size<0 || pointer<0 || pointer+size>original.Length) throw new Exception("Original header bounds are invalid");
                byte[] header=new byte[size]; Buffer.BlockCopy(original,pointer,header,0,size);
                Equal(fields[0]+" header",header,(byte[])get("HeaderData"));
                Equal(fields[0]+" custom",Convert.FromBase64String(fields[8]),(byte[])get("CustomData"));
                if (Convert.ToInt32(get("Channels"))!=channels || Convert.ToInt32(get("Frames"))!=frames || Convert.ToInt32(get("SampleRate"))!=22050) throw new Exception("PCM result metadata differs");
                count++;
            }
            int expectedCount = xas ? 12 : 16;
            if(count!=expectedCount) throw new Exception("Expected " + expectedCount + " original " + args[3] + " cases; found "+count);
            Console.WriteLine("PASS " + args[3] + " encoder component: " + count + " original HeaderData/CustomData comparisons; no SDK or plugin acceptance"); return 0;
        }
        catch(Exception ex){ Console.Error.WriteLine(ex); return 1; }
    }
}
