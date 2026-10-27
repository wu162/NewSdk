using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

internal static unsafe class TextureDxt5FloatAlphaDifferential
{
    // CodeAlphaBlock's this pointer is not read by its production body or QuantizeAlpha.
    // It consumes one stack argument and returns with ret 4.
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate void Alpha(IntPtr block);
    private static int checks,fixtures;
    private static void Check(bool ok,string message){checks++;if(!ok)throw new Exception(message);}
    private static string Hash(byte[] bytes){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
    private static int Main(string[] args)
    {
        try {
            Check(IntPtr.Size==4,"x86 required");
            string input=Path.GetFullPath(args[0]),dir=Path.GetDirectoryName(input);
            Check(Hash(File.ReadAllBytes(input))=="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e","Original hash");
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object s,ResolveEventArgs e){string p=Path.Combine(dir,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
            Assembly original=Assembly.LoadFrom(input),recovered=Assembly.LoadFrom(Path.GetFullPath(args[2]));
            IntPtr module=Marshal.GetHINSTANCE(original.ManifestModule);Check(module!=IntPtr.Zero&&module!=new IntPtr(-1),"Native module");
            Alpha native=(Alpha)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(module,0x7e8f0),typeof(Alpha));
            MethodInfo encode=recovered.GetType("BinaryAssetBuilder.TextureCompiler.TextureDxt5AlphaEncoder",true).GetMethod("EncodeAlphaBlock");
            float[] boundary={0, float.Epsilon, 1.0e-20f, 1.0e-8f, 1.0f/255, 0.25f, 0.5f, 0.75f, 254.0f/255, 0.99999994f, 1};
            foreach(float[] invalid in new float[][]{null,new float[0],new float[15],new float[17]}) {
                bool rejected=false;try{encode.Invoke(null,new object[]{invalid});}catch(TargetInvocationException e){rejected=e.InnerException is ArgumentException;}Check(rejected,"Invalid length accepted");
            }
            foreach(float invalid in new float[]{Single.NaN,Single.PositiveInfinity,Single.NegativeInfinity,-0.001f,1.001f}) {
                float[] values=new float[16];values[7]=invalid;
                bool rejected=false;try{encode.Invoke(null,new object[]{values});}catch(TargetInvocationException e){rejected=e.InnerException is ArgumentException;}Check(rejected,"Invalid alpha accepted");
            }
            using(var output=new StreamWriter(args[1])) {
                output.WriteLine("fixture\tlevels\tendpoints\talphaPayload");
                for(int fixture=0;fixture<12288;fixture++) {
                    float[] values=new float[16];Random random=new Random(fixture*7919+31);
                    for(int i=0;i<16;i++) {
                        if(fixture<boundary.Length) values[i]=boundary[fixture];
                        else if(fixture<512) values[i]=boundary[(fixture+i)%boundary.Length];
                        else if(fixture<1024) values[i]=(i%3)==0?0:(i%3)==1?1:boundary[fixture%boundary.Length];
                        else if(fixture<4096) values[i]=(float)((i+fixture%16)/31.0);
                        else values[i]=(float)random.NextDouble();
                    }
                    float[] before=(float[])values.Clone();
                    byte[] state=new byte[0x85c],expected=new byte[8];
                    int levels,a,b;
                    fixed(byte* p=state) {
                        *(int*)p=16;*(int*)(p+0x818)=71;
                        for(int i=0;i<16;i++) ((float*)(p+4))[i]=values[i];
                        native(new IntPtr(p));
                        levels=*(int*)(p+0x80c);a=*(int*)(p+0x810);b=*(int*)(p+0x814);
                        Check(levels==6||levels==8,"Native levels");
                        int[] indices=new int[16];for(int i=0;i<16;i++)indices[i]=((int*)(p+0x81c))[i];
                        // Literal PC DDS writer rules, cross-checked separately through actual DDS.
                        if(a==b){b++;levels=6;Array.Clear(indices,0,indices.Length);}
                        bool swap=(a>b)==(levels==6);if(swap){int t=a;a=b;b=t;}
                        expected[0]=(byte)Math.Max(0,Math.Min(255,a));expected[1]=(byte)Math.Max(0,Math.Min(255,b));
                        ulong bits=0;
                        for(int i=0;i<16;i++) {
                            int index=indices[i];Check(index>=0&&index<8,"Native index");
                            if(swap){if(levels==8)index=index>1?9-index:1-index;else if(index<6)index=index>1?7-index:1-index;}
                            bits|=(ulong)index<<(3*i);
                        }
                        for(int i=0;i<6;i++)expected[i+2]=(byte)(bits>>(8*i));
                    }
                    byte[] actual=(byte[])encode.Invoke(null,new object[]{values});Check(actual.Length==8,"Payload size");
                    for(int i=0;i<8;i++)Check(actual[i]==expected[i],"Float alpha mismatch fixture="+fixture+" byte="+i+" expected="+expected[i]+" actual="+actual[i]);
                    for(int i=0;i<16;i++)Check(values[i]==before[i],"Input changed");
                    output.WriteLine(fixture+"\t"+levels+"\t"+a+","+b+"\t"+BitConverter.ToString(actual));fixtures++;
                }
            }
            Console.WriteLine("PASS Texture DXT5 float alpha fixtures="+fixtures+" checks="+checks+"; native production state and PC payload rules; normalized float input only");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}
