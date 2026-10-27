using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

// Calls the immutable original RGBAImage nvDDS::nvDXTcompress native body.
// Inspection mode: original.dll transcript.txt. Differential mode adds recovered.dll.
internal static unsafe class TextureDxt5AlphaDifferential
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int Writer(IntPtr data, uint size, IntPtr mip, IntPtr user);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int Compressor(IntPtr image, IntPtr options, Writer writer, IntPtr rect);
    private static MemoryStream capture;
    private static List<string> calls;
    private static int checks, fixtures;
    private static void Check(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    private static int Write(IntPtr data, uint size, IntPtr mip, IntPtr user)
    {
        byte[] bytes = new byte[checked((int)size)];
        if (size != 0) Marshal.Copy(data, bytes, 0, bytes.Length);
        capture.Write(bytes, 0, bytes.Length);
        string info = "null";
        if (mip != IntPtr.Zero)
        {
            int[] fields = new int[5]; Marshal.Copy(mip, fields, 0, 5);
            info = String.Join(",", Array.ConvertAll(fields, delegate(int n) { return n.ToString(); }));
        }
        calls.Add(size.ToString() + ":" + info);
        return 0;
    }
    private static string Hash(byte[] bytes)
    {
        using (SHA256 sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
    }
    private static byte[] Pixels(uint width, uint height, int pattern)
    {
        byte[] result = new byte[checked((int)(width * height * 4))];
        for (int i = 0; i < result.Length / 4; i++)
        {
            switch (pattern)
            {
                case 0: result[i*4]=0;result[i*4+1]=0;result[i*4+2]=0;result[i*4+3]=0;break;
                case 1: result[i*4]=255;result[i*4+1]=255;result[i*4+2]=255;result[i*4+3]=255;break;
                case 2: result[i*4]=(byte)(i*37+17);result[i*4+1]=(byte)(i*71+33);result[i*4+2]=(byte)(i*113+129);result[i*4+3]=(byte)(i*19+7);break;
                case 3: result[i*4]=255;result[i*4+1]=1;result[i*4+2]=128;result[i*4+3]=63;break;
                default: result[i*4]=(byte)(i&255);result[i*4+1]=(byte)((i*3+4)&255);result[i*4+2]=(byte)((i*7+3)&255);result[i*4+3]=(byte)(255-(i&255));break;
            }
        }
        return result;
    }
    private static byte[] Original(Compressor compress, object plugin, MethodInfo optionsMethod,
        uint width, uint height, byte[] pixels, string format, bool mipmaps)
    {
        byte[] options = new byte[1644];
        fixed (byte* op = options)
        fixed (byte* rgba = pixels)
        {
            optionsMethod.Invoke(plugin, new object[] { Pointer.Box(op, optionsMethod.GetParameters()[0].ParameterType), format, mipmaps });
            uint* image = stackalloc uint[6];
            image[0]=width;image[1]=height;image[2]=(uint)rgba;
            image[3]=width*height;image[4]=image[3];image[5]=0;
            capture = new MemoryStream(); calls = new List<string>();
            Writer writer = new Writer(Write);
            int result = compress(new IntPtr(image), new IntPtr(op), writer, IntPtr.Zero);
            GC.KeepAlive(writer);
            Check(result == 0, "original nvDXTcompress result=" + result + " " + format + " " + width + "x" + height);
            byte[] output = capture.ToArray();capture.Dispose();
            Check(output.Length >= 128, "DDS header missing");
            Check(output[0]==68 && output[1]==68 && output[2]==83 && output[3]==32, "DDS magic missing");
            return output;
        }
    }

    private static int Main(string[] args)
    {
        IDisposable owner=null;
        try{
            Check(IntPtr.Size==4,"x86 required");
            string input=Path.GetFullPath(args[0]),dir=Path.GetDirectoryName(input),output=Path.GetFullPath(args[1]);
            Check(Hash(File.ReadAllBytes(input))=="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e","Original hash mismatch");
            Assembly recovered=Assembly.LoadFrom(Path.GetFullPath(args[2]));
            MethodInfo encode=recovered.GetType("BinaryAssetBuilder.TextureCompiler.TextureDxt5AlphaEncoder",true).GetMethod("EncodeBlock");
            foreach(byte[] invalid in new byte[][] { null, new byte[0], new byte[63], new byte[65] }) {
                bool rejected=false;
                try { encode.Invoke(null,new object[]{invalid}); }
                catch(TargetInvocationException e) { rejected=e.InnerException is ArgumentException; }
                Check(rejected,"invalid block must be rejected");
            }
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string p=Path.Combine(dir,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
            Assembly original=Assembly.LoadFrom(input);Type type=original.GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true);
            object plugin=Activator.CreateInstance(type);owner=(IDisposable)plugin;
            MethodInfo initialize=type.GetMethod("Initialize"),reinitialize=type.GetMethod("ReInitialize");
            Type platformType=initialize.GetParameters()[1].ParameterType;
            initialize.Invoke(plugin,new object[]{null,Enum.ToObject(platformType,0)});
            MethodInfo optionsMethod=type.GetMethod("GetNVCompressionOptions",BindingFlags.Instance|BindingFlags.NonPublic);
            Check(original.ManifestModule.ResolveMethod(0x060000DC).Name=="nvDDS.nvDXTcompress","Native token");
            IntPtr module=Marshal.GetHINSTANCE(original.ManifestModule);Check(module!=IntPtr.Zero&&module!=new IntPtr(-1),"Native base");
            Compressor compressor=(Compressor)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(module,0x52b60),typeof(Compressor));
            using(var replay=new BinaryWriter(File.Create(output+".replay")))
            using(var transcript=new StreamWriter(output)) {
                transcript.WriteLine("fixture\toriginalAlpha\trecoveredAlpha\tddsSha256");
                // Exhaust every alpha value, then every value at every pixel position
                // against nonuniform colour/alpha neighbours (packing and ordering).
                for(int fixture=0;fixture<78144;fixture++) {
                    byte[] pixels=Pixels(4,4,2);
                    if(fixture<256) {
                        for(int i=0;i<16;i++) pixels[i*4+3]=(byte)fixture;
                    } else if (fixture<4352) {
                        int n=fixture-256, position=n/256, value=n%256;
                        for(int i=0;i<16;i++) pixels[i*4+3]=(byte)(i*17);
                        pixels[position*4+3]=(byte)value;
                    } else if(fixture<12608) {
                        Random random=new Random(fixture*31+17); random.NextBytes(pixels);
                    } else {
                        int pair=fixture-12608, a=pair&255, b=pair>>8;
                        for(int i=0;i<16;i++) pixels[i*4+3]=(byte)((i&1)==0?a:b);
                    }
                    byte[] before=(byte[])pixels.Clone();
                    byte[] expected=Original(compressor,plugin,optionsMethod,4,4,pixels,"D3DFMT_DXT5",false);
                    Check(expected.Length==144,"DXT5 complete DDS size");
                    Check(Hash(before)==Hash(pixels),"original modified input");
                    replay.Write(pixels);
                    replay.Write(expected,128,8);
                    byte[] actual=(byte[])encode.Invoke(null,new object[]{pixels});
                    Check(actual.Length==8,"alpha block length");
                    for(int i=0;i<8;i++) Check(actual[i]==expected[128+i],"alpha mismatch fixture="+fixture+" byte="+i+" expected="+expected[128+i]+" actual="+actual[i]);
                    Check(Hash(before)==Hash(pixels),"recovered modified input");
                    transcript.WriteLine(fixture+"\t"+BitConverter.ToString(expected,128,8)+"\t"+BitConverter.ToString(actual)+"\t"+Hash(expected));
                    fixtures++;
                }
            }
            Console.WriteLine("PASS Texture DXT5 production alpha fixtures="+fixtures+" checks="+checks+"; alpha subcomponent only; colour compression not implemented");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{if(owner!=null)owner.Dispose();}
    }
}

