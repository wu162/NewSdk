using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

// Calls the immutable original RGBAImage nvDDS::nvDXTcompress native body.
// Inspection mode: original.dll transcript.txt. Differential mode adds recovered.dll.
internal static unsafe class TextureRawDdsOracle
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
    private static void Reject(MethodInfo encode, Type expected, string name, uint width, uint height, byte[] rgba, string format, bool mipmaps)
    {
        try { encode.Invoke(null, new object[] { width, height, rgba, format, mipmaps }); }
        catch (TargetInvocationException e)
        {
            Check(e.InnerException != null && e.InnerException.GetType() == expected, "rejection type " + name);
            return;
        }
        throw new Exception("Expected rejection: " + name);
    }
    private static void Rejections(MethodInfo encode)
    {
        string format = "D3DFMT_A8R8G8B8";
        Reject(encode, typeof(ArgumentNullException), "null pixels", 1, 1, null, format, false);
        Reject(encode, typeof(ArgumentException), "zero width", 0, 1, new byte[0], format, false);
        Reject(encode, typeof(ArgumentException), "zero height", 1, 0, new byte[0], format, false);
        Reject(encode, typeof(ArgumentException), "non power of two", 3, 2, new byte[24], format, false);
        Reject(encode, typeof(ArgumentOutOfRangeException), "64MiB limit", 4096, 4096, new byte[0], format, false);
        Reject(encode, typeof(ArgumentOutOfRangeException), "dimension product overflow", 0x80000000u, 0x80000000u, new byte[0], format, false);
        Reject(encode, typeof(ArgumentException), "short RGBA storage", 1, 1, new byte[3], format, false);
        Reject(encode, typeof(ArgumentException), "excess RGBA storage", 1, 1, new byte[5], format, false);
        Reject(encode, typeof(NotSupportedException), "DXT compression", 1, 1, new byte[4], "D3DFMT_DXT1", false);
        Reject(encode, typeof(NotSupportedException), "unknown format", 1, 1, new byte[4], "unknown", false);
        Reject(encode, typeof(NotSupportedException), "null format", 1, 1, new byte[4], null, false);
        Reject(encode, typeof(NotSupportedException), "unaccepted native mip filter", 1, 1, new byte[4], format, true);
    }
    private static int Main(string[] args)
    {
        try
        {
            Check(IntPtr.Size==4, "x86 host required");
            if(args.Length<2 || args.Length>3) throw new ArgumentException("Usage: TextureRawDdsOracle original.dll transcript.txt [recovered.dll]");
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
            if(args.Length==3)
            {
                Type recovered=Assembly.LoadFrom(Path.GetFullPath(args[2])).GetType("Recovered.Compiler.Texture.TextureRawDdsEncoder",true);
                encode=recovered.GetMethod("Encode",new Type[]{typeof(uint),typeof(uint),typeof(byte[]),typeof(string),typeof(bool)});
                Check(encode!=null,"recovered Encode contract missing");
                Rejections(encode);
            }
            uint[,] dimensions={{1,1},{1,2},{2,1},{2,2},{4,4},{8,4},{4,8},{16,16},{64,4}};
            string[] formats={"D3DFMT_A8R8G8B8","D3DFMT_X8R8G8B8","D3DFMT_R5G6B5"};
            using(StreamWriter transcript=new StreamWriter(args[1],false,new UTF8Encoding(false)))
            {
                foreach(string format in formats)
                    for(int mip=0;mip<(encode==null ? 2 : 1);mip++)
                        for(int d=0;d<dimensions.GetLength(0);d++)
                            for(int pattern=0;pattern<5;pattern++)
                            {
                                uint width=dimensions[d,0],height=dimensions[d,1];
                                byte[] pixels=Pixels(width,height,pattern), source=(byte[])pixels.Clone();
                                byte[] output=Original(compress,plugin,optionsMethod,width,height,pixels,format,mip!=0);
                                string chunkInfo=String.Join(";",calls.ToArray());
                                Check(Hash(pixels)==Hash(source),"original changed borrowed RGBA storage");
                                if(encode!=null && mip==0)
                                {
                                    byte[] recoveredSource=(byte[])source.Clone();
                                    byte[] actual=(byte[])encode.Invoke(null,new object[]{width,height,recoveredSource,format,false});
                                    Check(Hash(recoveredSource)==Hash(source),"recovered changed borrowed RGBA storage");
                                    Check(output.Length==actual.Length,"DDS output length " + format + " " + width+"x"+height);
                                    for(int i=0;i<output.Length;i++) Check(output[i]==actual[i],"DDS byte " + i + " " + format + " " + width+"x"+height + " pattern="+pattern);
                                }
                                transcript.WriteLine("dds|{0}|{1}|{2}|{3}|{4}|{5}|{6}",format,mip,width,height,pattern,chunkInfo,Convert.ToBase64String(output));
                                fixtures++;
                            }
            }
            Console.WriteLine("PASS TextureRawDdsOracle fixtures={0} checks={1} recoveredCompared={2} mipmapsAccepted=false",fixtures,checks,encode!=null);
            return 0;
        }
        catch(Exception e) { Console.Error.WriteLine(e);return 1; }
    }
}
