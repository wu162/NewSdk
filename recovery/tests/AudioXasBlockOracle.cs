using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

// Expected blocks were produced by executing original DLL native RVA 0xf700.
internal static class AudioXasBlockOracle
{
    private static int Main(string[] args)
    {
        try
        {
            if(args.Length!=2)throw new Exception("Usage: AudioXasBlockOracle recovered.dll original-blocks.tsv");
            var type=Assembly.LoadFrom(Path.GetFullPath(args[0])).GetType("BinaryAssetBuilder.AudioCompiler.Xas1Encoder",true);
            var encode=type.GetMethod("EncodeBlock128");
            int count=0;var predictors=new HashSet<int>();var shifts=new HashSet<int>();
            foreach(string line in File.ReadAllLines(args[1]))
            {
                if(line.StartsWith("#"))continue;
                string[] f=line.Split('\t');if(f.Length!=4)throw new Exception("Malformed original block evidence");
                int channels=Int32.Parse(f[1]);byte[] bytes=Convert.FromBase64String(f[2]);
                if(bytes.Length!=128*channels*4)throw new Exception("Bad original sample count");
                float[] samples=new float[bytes.Length/4];Buffer.BlockCopy(bytes,0,samples,0,bytes.Length);
                byte[] expected=Convert.FromBase64String(f[3]),actual=(byte[])encode.Invoke(null,new object[]{samples,channels});
                if(actual.Length!=expected.Length)throw new Exception(f[0]+" output size differs");
                for(int i=0;i<actual.Length;++i)if(actual[i]!=expected[i])throw new Exception(f[0]+" byte "+i+" expected "+expected[i]+" actual "+actual[i]);
                for(int c=0;c<channels;c++)for(int g=0;g<4;g++){predictors.Add(expected[c*76+g*4]&15);shifts.Add(expected[c*76+g*4+2]&15);}
                count++;
            }
            if(count!=262)throw new Exception("Expected 262 original native cases, found "+count);
            for(int i=0;i<4;i++)if(!predictors.Contains(i))throw new Exception("Original matrix lacks predictor "+i);
            for(int i=0;i<13;i++)if(!shifts.Contains(i))throw new Exception("Original matrix lacks quantization shift "+i);
            Console.WriteLine("PASS XAS native-block differential: 262 cases, predictors 0/1/2/3, mono/stereo, original DLL machine-code oracle");return 0;
        }
        catch(Exception ex){Console.Error.WriteLine(ex);return 1;}
    }
}
