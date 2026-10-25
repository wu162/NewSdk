using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

// Calls the immutable original RGBAImage nvDDS::nvDXTcompress native body.
// Inspection mode: original.dll transcript.txt. Differential mode adds recovered.dll.
internal static unsafe class TextureVolumeMipProbe
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
        uint width, uint height, uint depth, byte[] pixels, string format)
    {
        byte[] options = new byte[1644];
        uint* images = stackalloc uint[checked((int)depth * 6)];
        fixed(byte* op=options) fixed(byte* rgba=pixels)
        {
            optionsMethod.Invoke(plugin,new object[]{Pointer.Box(op,optionsMethod.GetParameters()[0].ParameterType),format,true});
            ((uint*)op)[118]=2;
            for(uint z=0;z<depth;z++)
            {
                uint* image=images+z*6;image[0]=width;image[1]=height;
                image[2]=(uint)(rgba+z*width*height*4);image[3]=width*height;image[4]=image[3];image[5]=0;
            }
            uint* level=stackalloc uint[3];level[0]=(uint)images;level[1]=depth;level[2]=depth;
            uint* volume=stackalloc uint[3];volume[0]=(uint)level;volume[1]=1;volume[2]=1;
            capture=new MemoryStream();calls=new List<string>();Writer writer=new Writer(Write);
            int code=compress(new IntPtr(volume),new IntPtr(op),writer,IntPtr.Zero);GC.KeepAlive(writer);
            Check(code==0,"volume compressor result="+code);byte[] result=capture.ToArray();capture.Dispose();return result;
        }
    }
    private static void Case(Compressor compress,object plugin,MethodInfo optionsMethod,MethodInfo encode,StreamWriter transcript,uint width,uint height,uint depth,string format,int pattern)
    {
        byte[] source=Pixels(width,height*depth,pattern),borrowed=(byte[])source.Clone();
        byte[] expected=Original(compress,plugin,optionsMethod,width,height,depth,borrowed,format);
        Check(Hash(borrowed)==Hash(source),"original source changed");
        string label=format+" "+width+"x"+height+"x"+depth+" pattern="+pattern;
        fixtures++;transcript.WriteLine(label+"|"+String.Join(";",calls.ToArray())+"|"+Convert.ToBase64String(expected));
        Console.WriteLine("PROBE "+label+" bytes="+expected.Length+" header="+Convert.ToBase64String(expected,0,128)+" chunks="+String.Join(";",calls.ToArray()));
    }
    private static int Main(string[] args)
    {
        try
        {
            Check(IntPtr.Size==4, "x86 host required");
            if(args.Length!=2) throw new ArgumentException("Usage: TextureVolumeMipProbe original.dll transcript.txt recovered.dll");
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
            MethodBase native=original.ManifestModule.ResolveMethod(0x060000DB);
            Check(native.Name=="nvDDS.nvDXTcompress", "native Volume metadata token changed");
            Compressor compress=(Compressor)Marshal.GetDelegateForFunctionPointer(new IntPtr(module.ToInt32()+0x52950),typeof(Compressor));
            MethodInfo encode=null;
            string[] formats={"D3DFMT_A8R8G8B8","D3DFMT_X8R8G8B8","D3DFMT_R5G6B5"};
            using(StreamWriter transcript=new StreamWriter(args[1],false,new UTF8Encoding(false)))
            foreach(int platform in new int[]{0})
            {
                FieldInfo field=type.GetField("_Platform",BindingFlags.Instance|BindingFlags.NonPublic);field.SetValue(plugin,Enum.ToObject(field.FieldType,platform));
                Console.WriteLine("PROBE platform="+platform);
                foreach(string format in formats) foreach(uint size in new uint[]{1,2,4,8,16})
                foreach(uint depth in new uint[]{1,2,4,8})Case(compress,plugin,optionsMethod,encode,transcript,size,size,depth,format,2);
            }
            Console.WriteLine("PROBE TextureVolumeMipProbe fixtures={0} checks={1} originalOnly=true",fixtures,checks);
            return 0;
        }
        catch(Exception e) { Console.Error.WriteLine(e);return 1; }
    }
}


