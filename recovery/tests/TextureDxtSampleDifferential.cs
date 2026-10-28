using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

// Calls the immutable original RGBAImage nvDDS::nvDXTcompress native body.
// Inspection mode: original.dll transcript.txt. Differential mode adds recovered.dll.
internal static unsafe class TextureDxtSampleDifferential
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

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate int FloatCompressor(IntPtr owner,IntPtr source,IntPtr header,IntPtr output,int format,uint size);
    private static byte[] OriginalFloat(FloatCompressor compress,object plugin,MethodInfo optionsMethod,int width,int height,float[] rgba,string format)
    {
        byte[] options=new byte[1644],owner=new byte[32],source=new byte[80],header=new byte[124],output=new byte[48];
        for(int i=0;i<output.Length;i++)output[i]=0xa5;
        uint[] rows=new uint[height],rowOwner=new uint[1];
        fixed(byte* op=options,own=owner,src=source,head=header,dst=output)
        fixed(float* pixels=rgba)fixed(uint* row=rows,ro=rowOwner){
            optionsMethod.Invoke(plugin,new object[]{Pointer.Box(op,optionsMethod.GetParameters()[0].ParameterType),format,false});
            *(uint*)(own+0x18)=(uint)op;*(int*)(src+8)=height;*(int*)(src+12)=width;
            for(int y=0;y<height;y++)row[y]=(uint)(pixels+y*width*4);
            ro[0]=(uint)row;*(uint*)(src+0x24)=(uint)ro;
            Check(compress(new IntPtr(own),new IntPtr(src),new IntPtr(head),new IntPtr(dst),format=="D3DFMT_DXT3"?2:3,16)==0,"float dispatch status");
        }
        for(int i=16;i<output.Length;i++)Check(output[i]==0xa5,"native float payload overrun");
        byte[] block=new byte[16];Buffer.BlockCopy(output,0,block,0,16);return block;
    }
    private static int Main(string[] args)
    {
        IDisposable owner=null;
        try{
            Check(IntPtr.Size==4,"x86 required");string input=Path.GetFullPath(args[0]),dir=Path.GetDirectoryName(input),output=Path.GetFullPath(args[1]);
            Check(Hash(File.ReadAllBytes(input))=="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e","Original hash mismatch");
            Assembly recovered=Assembly.LoadFrom(Path.GetFullPath(args[2]));Type api=recovered.GetType("BinaryAssetBuilder.TextureCompiler.TextureDxtSampleBlockEncoder",true);
            MethodInfo encode=api.GetMethod("EncodeFloatBlock"),encodeBytes=api.GetMethod("EncodeBlock");Check(encode!=null&&encodeBytes!=null,"sample block entries");
            foreach(object[] invalid in new[]{new object[]{null,4,4,"D3DFMT_DXT3"},new object[]{new float[0],4,4,"D3DFMT_DXT3"},new object[]{new float[48],3,4,"D3DFMT_DXT3"},new object[]{new float[64],4,4,"D3DFMT_DXT1"},new object[]{new float[64],4,4,null}}){
                bool rejected=false;try{encode.Invoke(null,invalid);}catch(TargetInvocationException e){rejected=e.InnerException is ArgumentException||e.InnerException is NotSupportedException;}Check(rejected,"sample input rejection");
            }
            foreach(float invalid in new[]{Single.NaN,Single.PositiveInfinity,Single.NegativeInfinity,-0.1f,1.1f}){float[] values=new float[64];values[0]=invalid;bool rejected=false;try{encode.Invoke(null,new object[]{values,4,4,"D3DFMT_DXT5"});}catch(TargetInvocationException e){rejected=e.InnerException is ArgumentException;}Check(rejected,"sample range rejection");}
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string p=Path.Combine(dir,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
            Assembly original=Assembly.LoadFrom(input);Type type=original.GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true);object plugin=Activator.CreateInstance(type);owner=(IDisposable)plugin;
            MethodInfo initialize=type.GetMethod("Initialize");initialize.Invoke(plugin,new object[]{null,Enum.ToObject(initialize.GetParameters()[1].ParameterType,0)});
            MethodInfo optionsMethod=type.GetMethod("GetNVCompressionOptions",BindingFlags.Instance|BindingFlags.NonPublic);IntPtr module=Marshal.GetHINSTANCE(original.ManifestModule);
            Check(original.ManifestModule.ResolveMethod(0x060000DC).Name=="nvDDS.nvDXTcompress","Native token");
            Compressor byteCompressor=(Compressor)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(module,0x52b60),typeof(Compressor));
            FloatCompressor floatCompressor=(FloatCompressor)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(module,0x5c790),typeof(FloatCompressor));
            using(var replay=new BinaryWriter(File.Create(output+".replay")))using(var transcript=new StreamWriter(output)){
                replay.Write(4752);transcript.WriteLine("format\twidth\theight\tpattern\tsha256");
                foreach(string format in new[]{"D3DFMT_DXT3","D3DFMT_DXT5"})foreach(int width in new[]{1,2,4})foreach(int height in new[]{1,2,4})for(int pattern=0;pattern<264;pattern++){
                    byte[] bytes=Pixels((uint)width,(uint)height,pattern%8);if(pattern>=8)new Random(pattern*7919+width*31+height).NextBytes(bytes);
                    float[] values=new float[bytes.Length];for(int i=0;i<values.Length;i++)values[i]=(float)(bytes[i]/255.0);
                    if(pattern>=128){var random=new Random(pattern*3571+width*37+height);for(int i=0;i<values.Length;i++)values[i]=random.Next(16777216)/16777215f;
                        if(pattern<158)for(int i=3;i<values.Length;i+=4)values[i]=(float)((pattern-128+0.5)/15.0); // valid half-step alpha cases below
                        for(int i=0;i<values.Length;i++)if(values[i]>1)values[i]=1;
                    }
                    // Near-equal RGB values must still use the original exact
                    // all-same decision, across every partial block shape.
                    if(pattern>=256)for(int i=0;i<values.Length/4;i++)for(int c=0;c<3;c++) {
                        double center=pattern<260?0.5:((c==1?31.5:15.5)/(c==1?63.0:31.0));
                        values[i*4+c]=(float)(center+(pattern%4-1.5)*0.00002+(i-(values.Length/4-1)*0.5)*0.000004);
                    }
                    float[] before=(float[])values.Clone();byte[] expected=OriginalFloat(floatCompressor,plugin,optionsMethod,width,height,values,format);
                    byte[] actual=(byte[])encode.Invoke(null,new object[]{values,width,height,format});string label=format+" "+width+"x"+height+" p"+pattern;
                    Check(actual.Length==16,label+" size");for(int i=0;i<16;i++)Check(expected[i]==actual[i],label+" byte="+i+" expected="+expected[i]+" actual="+actual[i]);
                    for(int i=0;i<values.Length;i++)Check(values[i]==before[i],label+" source changed");
                    if(pattern<128){byte[] dds=Original(byteCompressor,plugin,optionsMethod,(uint)width,(uint)height,bytes,format,false);Check(dds.Length==144,label+" native byte DDS size");
                        byte[] byteActual=(byte[])encodeBytes.Invoke(null,new object[]{bytes,width,height,format});for(int i=0;i<16;i++)Check(dds[128+i]==expected[i]&&byteActual[i]==expected[i],label+" byte/float wrapper mismatch="+i);
                    }
                    replay.Write(format);replay.Write(width);replay.Write(height);replay.Write(values.Length);foreach(float value in values)replay.Write(value);replay.Write(expected);
                    transcript.WriteLine(format+"\t"+width+"\t"+height+"\t"+pattern+"\t"+Hash(actual));fixtures++;
                }
            }
            Check(fixtures==4752,"corpus count");Console.WriteLine("PASS Texture DXT sample blocks fixtures="+fixtures+" checks="+checks+"; normalized float/partial POT shapes; original float dispatch plus 2304 actual byte DDS cross-checks; SDK/plugin not extended");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{if(owner!=null)owner.Dispose();}
    }
}
