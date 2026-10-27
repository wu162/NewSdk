using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

internal static unsafe class TextureDxt5FastAlphaDifferential
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void FastAlpha(IntPtr block);
    private static int fixtures,checks;
    private static void Check(bool ok,string message) { checks++; if(!ok) throw new Exception(message); }
    private static string Hash(byte[] bytes) { using(var h=SHA256.Create()) return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant(); }
    private static int Main(string[] args)
    {
        try {
            Check(IntPtr.Size==4,"x86 required");
            string input=Path.GetFullPath(args[0]), dir=Path.GetDirectoryName(input);
            Check(Hash(File.ReadAllBytes(input))=="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e","Original hash");
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object s,ResolveEventArgs e){string p=Path.Combine(dir,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
            Assembly original=Assembly.LoadFrom(input), recovered=Assembly.LoadFrom(Path.GetFullPath(args[2]));
            IntPtr module=Marshal.GetHINSTANCE(original.ManifestModule);
            Check(module!=IntPtr.Zero&&module!=new IntPtr(-1),"Module base");
            FastAlpha native=(FastAlpha)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(module,0x8b570),typeof(FastAlpha));
            MethodInfo encode=recovered.GetType("BinaryAssetBuilder.TextureCompiler.TextureDxt5FastAlphaEncoder",true).GetMethod("EncodeBlock");
            foreach(byte[] invalid in new byte[][]{null,new byte[0],new byte[63],new byte[65]}) {
                bool rejected=false;try{encode.Invoke(null,new object[]{invalid});}catch(TargetInvocationException e){rejected=e.InnerException is ArgumentException;}
                Check(rejected,"Invalid input accepted");
            }
            using(var output=new StreamWriter(args[1])) {
                output.WriteLine("fixture\talphaBlock");
                Random random=new Random(17027);
                for(int fixture=0;fixture<71168;fixture++) {
                    byte[] rgba=new byte[64];random.NextBytes(rgba);
                    if(fixture<65536) {
                        int a=fixture&255,b=fixture>>8;
                        for(int i=0;i<16;i++) rgba[i*4+3]=(byte)((i&1)==0?a:b);
                    } else if(fixture<69632) {
                        int n=fixture-65536;
                        for(int i=0;i<16;i++) rgba[i*4+3]=(byte)(i*17);
                        rgba[(n/256)*4+3]=(byte)n;
                    } else if(fixture<69888) {
                        for(int i=0;i<16;i++) rgba[i*4+3]=(byte)((fixture+i*17)&255);
                    }
                    byte[] before=(byte[])rgba.Clone(), state=new byte[0x85c];
                    byte[] expected=new byte[8];
                    fixed(byte* p=state) {
                        *(int*)p=16;
                        for(int i=0;i<16;i++) ((float*)(p+4))[i]=(float)(rgba[i*4+3]/255.0);
                        native(new IntPtr(p));
                        int high=*(int*)(p+0x810), low=*(int*)(p+0x814);
                        Check(high>=0&&high<=255&&low>=0&&low<=255,"Endpoint range");
                        expected[0]=(byte)high;expected[1]=(byte)low;
                        ulong bits=0;
                        for(int i=0;i<16;i++) {int index=((int*)(p+0x81c))[i];Check(index>=0&&index<8,"Index range");bits|=(ulong)index<<(3*i);}
                        for(int i=0;i<6;i++) expected[i+2]=(byte)(bits>>(8*i));
                    }
                    byte[] actual=(byte[])encode.Invoke(null,new object[]{rgba});
                    Check(actual.Length==8,"Output size");
                    for(int i=0;i<8;i++) Check(actual[i]==expected[i],"Mismatch fixture="+fixture+" byte="+i+" expected="+expected[i]+" actual="+actual[i]);
                    Check(Hash(before)==Hash(rgba),"Source modified");
                    output.WriteLine(fixture+"\t"+BitConverter.ToString(actual)); fixtures++;
                }
            }
            Console.WriteLine("PASS Texture DXT5 fast alpha fixtures="+fixtures+" checks="+checks+"; quality68 native subcomponent only; no production endpoint search acceptance");return 0;
        } catch(Exception e) { Console.Error.WriteLine(e);return 1; }
    }
}
