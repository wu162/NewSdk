using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

// Calls the immutable original RGBAImage nvDDS::nvDXTcompress native body.
// Inspection mode: original.dll transcript.txt. Differential mode adds recovered.dll.
internal static unsafe class TextureCubicResizeProbe
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
            optionsMethod.Invoke(plugin, new object[] { Pointer.Box(op, optionsMethod.GetParameters()[0].ParameterType), format, false });
            ((uint*)op)[15]=5; ((uint*)op)[16]=4;
            ((float*)op)[17]=Math.Max(width/2,1u); ((float*)op)[18]=Math.Max(height/2,1u);
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
    private static void Case(Compressor compress,object plugin,MethodInfo optionsMethod,MethodInfo encode,StreamWriter transcript,uint width,uint height,string format,string name,byte[] source)
    {
        byte[] originalSource=(byte[])source.Clone();
        byte[] expected=Original(compress,plugin,optionsMethod,width,height,originalSource,format,true);
        string chunkInfo=String.Join(";",calls.ToArray());
        Check(Hash(originalSource)==Hash(source),"original source preservation "+name);
        transcript.WriteLine("{0}|{1}|{2}|{3}|{4}",format,width,height,name,Convert.ToBase64String(expected));
        fixtures++;
    }
    private static int Main(string[] args)
    {
        try
        {
            Check(IntPtr.Size==4, "x86 host required");
            if(args.Length!=2) throw new ArgumentException("Usage: TextureCubicResizeProbe original.dll transcript.txt recovered.dll");
            string input = Path.GetFullPath(args[0]);
            Check(Hash(File.ReadAllBytes(input))=="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e", "original SHA256 changed");
            string directory = Path.GetDirectoryName(input);
            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e)
            {
                string path=Path.Combine(directory,new AssemblyName(e.Name).Name+".dll");
                return File.Exists(path)?Assembly.LoadFrom(path):null;
            };
            Assembly original=Assembly.LoadFrom(input);
            Type type=original.GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true);
            object plugin=Activator.CreateInstance(type);
            MethodInfo optionsMethod=type.GetMethod("GetNVCompressionOptions",BindingFlags.Instance|BindingFlags.NonPublic);
            IntPtr module=Marshal.GetHINSTANCE(original.ManifestModule);
            Check(module!=IntPtr.Zero && module!=new IntPtr(-1),"original native image base unavailable");
            MethodBase native=original.ManifestModule.ResolveMethod(0x060000DC);
            Check(native.Name=="nvDDS.nvDXTcompress", "native RGBAImage metadata token changed");
            Compressor compress=(Compressor)Marshal.GetDelegateForFunctionPointer(new IntPtr(module.ToInt32()+0x52B60),typeof(Compressor));
            MethodInfo encode=null;
            uint[,] dimensions={{1,1},{1,2},{2,1},{2,2},{4,4},{8,4},{4,8},{16,16},{64,4}};
            string[] formats={"D3DFMT_A8R8G8B8","D3DFMT_X8R8G8B8","D3DFMT_R5G6B5"};
            using(StreamWriter transcript=new StreamWriter(args[1],false,new UTF8Encoding(false)))
            {
                foreach(string format in formats)
                    for(int d=0;d<dimensions.GetLength(0);d++)
                        for(int pattern=0;pattern<5;pattern++)
                            Case(compress,plugin,optionsMethod,encode,transcript,dimensions[d,0],dimensions[d,1],format,pattern.ToString(),Pixels(dimensions[d,0],dimensions[d,1],pattern));
                uint[,] extra={{1,128},{128,1},{1,256},{256,1},{2,256},{256,2},{4,128},{128,4},{64,256},{256,64},{128,128},{256,256},{128,256},{256,128}};
                foreach(string format in formats)
                    for(int d=0;d<extra.GetLength(0);d++)
                        for(int variant=0;variant<2;variant++)
                        {
                            uint width=extra[d,0],height=extra[d,1];
                            byte[] pixels=new byte[checked((int)(width*height*4))];
                            new Random(17381+d*719+variant*19001).NextBytes(pixels);
                            Case(compress,plugin,optionsMethod,encode,transcript,width,height,format,"random-"+d+"-"+variant,pixels);
                        }
                uint[,] impulseDimensions={{8,8},{16,8},{64,4},{256,1},{1,256}};
                foreach(string format in formats)
                    for(int d=0;d<impulseDimensions.GetLength(0);d++)
                        for(int variant=0;variant<4;variant++)
                        {
                            uint width=impulseDimensions[d,0],height=impulseDimensions[d,1];
                            byte[] pixels=new byte[checked((int)(width*height*4))];
                            int count=pixels.Length/4;
                            int at=variant==0?0:variant==1?count-1:variant==2?count/2:count/3;
                            pixels[at*4]=255;pixels[at*4+1]=127;pixels[at*4+2]=63;pixels[at*4+3]=251;
                            Case(compress,plugin,optionsMethod,encode,transcript,width,height,format,"impulse-"+d+"-"+variant,pixels);
                        }
            }
            Console.WriteLine("PASS TextureCubicResizeProbe fixtures={0} checks={1} originalOnly=true filter=Cubic4 rescale=PreScale5 target=HalfAxesAtLeast1",fixtures,checks);
            return 0;
        }
        catch(Exception e) { Console.Error.WriteLine(e);return 1; }
    }
}

