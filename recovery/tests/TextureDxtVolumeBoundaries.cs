using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

// Calls the immutable original RGBAImage nvDDS::nvDXTcompress native body.
// Inspection mode: original.dll transcript.txt. Differential mode adds recovered.dll.
internal static unsafe class TextureDxtVolumeBoundaries
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
        uint width, uint height, uint depth, byte[] pixels, string format, bool mipmaps)
    {
        byte[] options = new byte[1644];
        uint* images = stackalloc uint[checked((int)depth * 6)];
        fixed(byte* op=options) fixed(byte* rgba=pixels)
        {
            optionsMethod.Invoke(plugin,new object[]{Pointer.Box(op,optionsMethod.GetParameters()[0].ParameterType),format,mipmaps});
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
    private static uint U32(byte[] bytes,int offset){return BitConverter.ToUInt32(bytes,offset);}
    private static int Main(string[] args)
    {
        IDisposable owner=null;
        try{
            Check(IntPtr.Size==4,"x86 required");string input=Path.GetFullPath(args[0]),dir=Path.GetDirectoryName(input),output=Path.GetFullPath(args[1]);Directory.CreateDirectory(output);
            Check(Hash(File.ReadAllBytes(input))=="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e","Original hash mismatch");
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string p=Path.Combine(dir,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
            Assembly original=Assembly.LoadFrom(input);Type type=original.GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true);object plugin=Activator.CreateInstance(type);owner=(IDisposable)plugin;
            MethodInfo initialize=type.GetMethod("Initialize");initialize.Invoke(plugin,new object[]{null,Enum.ToObject(initialize.GetParameters()[1].ParameterType,0)});
            MethodInfo optionsMethod=type.GetMethod("GetNVCompressionOptions",BindingFlags.Instance|BindingFlags.NonPublic);IntPtr module=Marshal.GetHINSTANCE(original.ManifestModule);
            Check(original.ManifestModule.ResolveMethod(0x060000DB).Name=="nvDDS.nvDXTcompress","Native token");
            Compressor compressor=(Compressor)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(module,0x52950),typeof(Compressor));
            using(var transcript=new StreamWriter(Path.Combine(output,"transcript.tsv"))){
                transcript.WriteLine("format\twidth\theight\tdepth\tmipmaps\tpattern\tbytes\tflags\tlinearSize\tmipCount\tcaps\tcaps2\tcallbackFrames\tsha256");
                int[][] sizes={new[]{1,1,1},new[]{1,1,8},new[]{1,2,4},new[]{2,1,4},new[]{2,2,2},new[]{4,4,4},new[]{4,8,2},new[]{8,4,2},new[]{8,8,8},new[]{16,4,4},new[]{4,4,16},new[]{16,8,4},new[]{8,16,4}};
                foreach(string format in new[]{"D3DFMT_DXT3","D3DFMT_DXT5"})foreach(bool mips in new[]{false,true}){
                    byte[] options=new byte[1644];fixed(byte* op=options)optionsMethod.Invoke(plugin,new object[]{Pointer.Box(op,optionsMethod.GetParameters()[0].ParameterType),format,mips});
                    fixed(byte* op=options)((uint*)op)[118]=2;
                    File.WriteAllBytes(Path.Combine(output,format+"-mips"+mips+".options"),options);
                    foreach(int[] size in sizes)for(int pattern=0;pattern<8;pattern++){
                        uint width=(uint)size[0],height=(uint)size[1],depth=(uint)size[2];byte[] pixels=Pixels(width,height*depth,pattern);if(pattern>=4)new Random(pattern*7919+size[0]*31+size[1]+size[2]*101).NextBytes(pixels);
                        string before=Hash(pixels);byte[] dds=Original(compressor,plugin,optionsMethod,width,height,depth,pixels,format,mips);
                        Check(Hash(pixels)==before,"source modified");Check(U32(dds,12)==height&&U32(dds,16)==width&&U32(dds,24)==depth,"DDS dimensions");Check(U32(dds,84)==(format=="D3DFMT_DXT3"?0x33545844u:0x35545844u),"FourCC");
                        string name=format+"-"+width+"x"+height+"x"+depth+"-mips"+mips+"-p"+pattern;
                        File.WriteAllBytes(Path.Combine(output,name+".rgba"),pixels);File.WriteAllBytes(Path.Combine(output,name+".dds"),dds);
                        transcript.WriteLine(format+"\t"+width+"\t"+height+"\t"+depth+"\t"+mips+"\t"+pattern+"\t"+dds.Length+"\t"+U32(dds,8)+"\t"+U32(dds,20)+"\t"+U32(dds,28)+"\t"+U32(dds,108)+"\t"+U32(dds,112)+"\t"+String.Join(";",calls.ToArray())+"\t"+Hash(dds));fixtures++;
                    }
                }
            }
            Check(fixtures==416,"probe corpus count");Console.WriteLine("PROBE Texture DXT volume boundaries fixtures="+fixtures+" checks="+checks+"; original-only PC DXT3/5 small/aligned volumes and complete mip chains; not recovered acceptance");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{if(owner!=null)owner.Dispose();}
    }
}

