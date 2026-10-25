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
            if(args.Length!=2 && !(args.Length==3 && args[2]=="precision"))throw new Exception("Usage: AudioXasBlockOracle recovered.dll original-blocks.tsv [precision]");
            bool precision=args.Length==3;
            MethodInfo precisionSamples=null;
            if(precision)
            {
                Type fixture=typeof(AudioXasBlockOracle).Assembly.GetType("AudioXasPrecisionFixture",false);
                if(fixture==null)throw new Exception("Compile AudioXasPrecisionFixture.cs with the precision host");
                precisionSamples=fixture.GetMethod("Samples",BindingFlags.NonPublic|BindingFlags.Static);
                if(precisionSamples==null)throw new Exception("Precision sample source missing");
            }
            var type=Assembly.LoadFrom(Path.GetFullPath(args[0])).GetType("BinaryAssetBuilder.AudioCompiler.Xas1Encoder",true);
            var encode=type.GetMethod("EncodeBlock128");
            int count=0;var precisionCoverage=new HashSet<int>();var keys=new HashSet<string>();var predictors=new HashSet<int>();var shifts=new HashSet<int>();
            bool fingerprint=false;
            foreach(string line in File.ReadAllLines(args[1]))
            {
                if(line.StartsWith("#"))
                {
                    fingerprint=line.Contains("originalDllSha256=a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139") && line.Contains("nativeRva=0xf700") && line.Contains("fpuControl=0x037f");
                    continue;
                }
                if(!fingerprint)throw new Exception("Original XAS fingerprint/RVA/FPU evidence missing");
                string[] f=line.Split('\t');if(f.Length!=4)throw new Exception("Malformed original block evidence");
                int channels=Int32.Parse(f[1]);
                if(channels!=1&&channels!=2)throw new Exception("Invalid native channel count");
                if(!keys.Add(f[0]+":"+channels))throw new Exception("Duplicate original block fixture");
                byte[] bytes=Convert.FromBase64String(f[2]);
                if(bytes.Length!=128*channels*4)throw new Exception("Bad original sample count");
                float[] samples=new float[bytes.Length/4];Buffer.BlockCopy(bytes,0,samples,0,bytes.Length);
                byte[] expected=Convert.FromBase64String(f[3]);
                if(expected.Length!=76*channels)throw new Exception("Bad original encoded size");
                if(f[0]=="precision-resampled-sine6144")
                {
                    if(!precision)throw new Exception("Precision fixture requires explicit precision mode");
                    float[] declared=(float[])precisionSamples.Invoke(null,new object[]{channels});
                    byte[] declaredBytes=new byte[declared.Length*4];Buffer.BlockCopy(declared,0,declaredBytes,0,declaredBytes.Length);
                    if(declaredBytes.Length!=bytes.Length)throw new Exception("Precision source size differs");
                    for(int i=0;i<bytes.Length;++i)if(bytes[i]!=declaredBytes[i])throw new Exception("Precision signed16 source differs at byte "+i);
                    byte[] groupBytes=new byte[]{0xc2,0xf5,0x36,0xfc,0x46,0x46,0x66,0x57,0x66,0x76,0x76,0x67,0x65,0x66,0x55,0x55,0x34,0x42,0x41};
                    for(int c=0;c<channels;c++)for(int g=0;g<4;g++)for(int i=0;i<19;i++)
                    {
                        int at=c*76+(i<4?g*4+i:16+(i-4)*4+g);
                        if(expected[at]!=groupBytes[i])throw new Exception("Original precision group differs from actual plugin regression");
                    }
                    if(!precisionCoverage.Add(channels))throw new Exception("Duplicate precision channel fixture");
                }
                byte[] actual=(byte[])encode.Invoke(null,new object[]{samples,channels});
                if(actual.Length!=expected.Length)throw new Exception(f[0]+" output size differs");
                for(int i=0;i<actual.Length;++i)if(actual[i]!=expected[i])throw new Exception(f[0]+" byte "+i+" expected "+expected[i]+" actual "+actual[i]);
                for(int c=0;c<channels;c++)for(int g=0;g<4;g++){predictors.Add(expected[c*76+g*4]&15);shifts.Add(expected[c*76+g*4+2]&15);}
                count++;
            }
            if(count!=(precision?264:262))throw new Exception("Expected "+(precision?264:262)+" original native cases, found "+count);
            if(precision && (!precisionCoverage.Contains(1)||!precisionCoverage.Contains(2)))throw new Exception("Missing mono/stereo precision fixtures");
            foreach(int channels in new int[]{1,2})
            {
                foreach(string amplitude in new string[]{"0","0.001","0.1","0.5","0.999"})
                {
                    if(!keys.Contains("constant-"+amplitude+":"+channels))throw new Exception("Missing original constant fixture");
                    foreach(int frequency in new int[]{1,3,8,20,48})if(!keys.Contains("sine-"+amplitude+"-"+frequency+":"+channels))throw new Exception("Missing original sine fixture");
                }
                if(!keys.Contains("ramp:"+channels))throw new Exception("Missing original ramp fixture");
                for(int i=0;i<100;++i)if(!keys.Contains("random-"+i+":"+channels))throw new Exception("Missing original random fixture");
            }
            for(int i=0;i<4;i++)if(!predictors.Contains(i))throw new Exception("Original matrix lacks predictor "+i);
            for(int i=0;i<13;i++)if(!shifts.Contains(i))throw new Exception("Original matrix lacks quantization shift "+i);
            Console.WriteLine("PASS XAS native-block differential: "+count+" cases, "+(precision?"two signed16-derived strict-binary32 regressions, ":"")+"predictors 0/1/2/3, mono/stereo, original DLL machine-code oracle");return 0;
        }
        catch(Exception ex){Console.Error.WriteLine(ex);return 1;}
    }
}
