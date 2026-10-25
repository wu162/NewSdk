using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

// Calls the immutable original RGBAImage nvDDS::nvDXTcompress native body.
// Inspection mode: original.dll transcript.txt. Differential mode adds recovered.dll.
internal static unsafe class TextureCubicIntermediateProbe
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int Writer(IntPtr data, uint size, IntPtr mip, IntPtr user);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int Compressor(IntPtr image, IntPtr options, Writer writer, IntPtr rect);
    private static MemoryStream capture;
    private static List<string> calls;
    private static int checks, fixtures;
    private static List<string> floatBits;
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
            ((uint*)op)[15]=5;((uint*)op)[16]=4;
            ((float*)op)[17]=Math.Max(width/2,1u);((float*)op)[18]=Math.Max(height/2,1u);
            uint* image = stackalloc uint[6];
            image[0]=width;image[1]=height;image[2]=(uint)rgba;
            image[3]=width*height;image[4]=image[3];image[5]=0;
            capture = new MemoryStream(); calls = new List<string>(); floatBits = new List<string>();
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
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void FloatTrace(IntPtr stack, IntPtr fxsave);
    [DllImport("kernel32.dll", SetLastError=true)]
    private static extern bool VirtualProtect(IntPtr address, UIntPtr size, uint protection, out uint oldProtection);
    [DllImport("kernel32.dll", SetLastError=true)]
    private static extern IntPtr VirtualAlloc(IntPtr address, UIntPtr size, uint allocationType, uint protection);
    [DllImport("kernel32.dll", SetLastError=true)]
    private static extern bool VirtualFree(IntPtr address, UIntPtr size, uint freeType);
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentProcess();
    [DllImport("kernel32.dll", SetLastError=true)]
    private static extern bool FlushInstructionCache(IntPtr process, IntPtr address, UIntPtr size);
    private static FloatTrace traceRoot;
    private static IntPtr hookAddress, trampoline;
    private static byte[] savedBytes;
    private static uint originalProtection;
    private static bool hooked;
    private static bool captureIntermediate;
    private static int tracedCalls;
    private static string traceError;
    private static void Trace(IntPtr stack, IntPtr fxsave)
    {
        if (!captureIntermediate) return;
        try
        {
            int count=Marshal.ReadInt32(stack,4);
            IntPtr table=Marshal.ReadIntPtr(stack,12),source=Marshal.ReadIntPtr(stack,16);
            if(count<1 || count>32) { traceError="unexpected count="+count;return; }
            var row=new StringBuilder();row.Append("count=").Append(count);
            row.Append(" cw=").Append(((ushort)Marshal.ReadInt16(fxsave,0)).ToString("X4"));
            row.Append(" mxcsr=").Append(Marshal.ReadInt32(fxsave,24).ToString("X8"));
            for(int i=0;i<count;i++)
            {
                int first=Marshal.ReadInt32(table,i*12),last=Marshal.ReadInt32(table,i*12+4);
                IntPtr weights=Marshal.ReadIntPtr(table,i*12+8);
                if(first<0 || last<first || last>128){traceError="unsafe contribution bounds";return;}
                row.Append(" row=").Append(first).Append(',').Append(last).Append(" weights=");
                for(int k=0;k<last-first;k++)row.Append(Marshal.ReadInt64(weights,k*8).ToString("X16")).Append(',');
                row.Append(" sourceRGBA=");
                for(int k=first*4;k<last*4;k++)row.Append(Marshal.ReadInt32(source,k*4).ToString("X8")).Append(',');
            }
            floatBits.Add(row.ToString());tracedCalls++;
        }catch(Exception e){traceError=e.GetType().Name+":"+e.Message;}
    }
    private static void Add32(List<byte> code, int value) { code.AddRange(BitConverter.GetBytes(value)); }
    private static void BeginProbe(IntPtr module)
    {
        hookAddress=new IntPtr(module.ToInt32()+0x5e6c0);
        savedBytes=new byte[7];Marshal.Copy(hookAddress,savedBytes,0,7);
        byte[] expected={0x8b,0x44,0x24,0x04,0x83,0xec,0x08};
        for(int i=0;i<expected.Length;i++)Check(savedBytes[i]==expected[i],"float4 horizontal entry opcode guard");
        traceRoot=new FloatTrace(Trace);
        IntPtr callback=Marshal.GetFunctionPointerForDelegate(traceRoot);
        trampoline=VirtualAlloc(IntPtr.Zero,new UIntPtr(1024),0x3000,0x40);
        Check(trampoline!=IntPtr.Zero,"trampoline allocation");
        List<byte> code=new List<byte>();
        // Save EFLAGS/GPRs and all x87/SSE state. FXSAVE needs 16-byte alignment.
        // EBP anchors the saved-register stack, original caller stack is [ebp+36] and the aligned FXSAVE buffer is ESP.
        code.AddRange(new byte[]{0x9c,0x60,0x8b,0xec,0x81,0xec,0x10,0x02,0x00,0x00,0x83,0xe4,0xf0,0x0f,0xae,0x04,0x24,0xdb,0xe3,0x54,0x8d,0x45,0x24,0x50,0xb8});
        Add32(code,callback.ToInt32());
        code.AddRange(new byte[]{0xff,0xd0,0x83,0xc4,0x08,0x0f,0xae,0x0c,0x24,0x8b,0xe5,0x61,0x9d});
        // Run the original two full instructions with its exact restored state,
        // then return to the untouched native body at +7. No quantizer is replaced.
        code.AddRange(savedBytes);code.Add(0xe9);
        Add32(code,unchecked(hookAddress.ToInt32()+7-(trampoline.ToInt32()+code.Count+4)));
        byte[] body=code.ToArray();Marshal.Copy(body,0,trampoline,body.Length);
        Check(FlushInstructionCache(GetCurrentProcess(),trampoline,new UIntPtr((uint)body.Length)),"flush trampoline");
        uint old;
        Check(VirtualProtect(hookAddress,new UIntPtr(5),0x40,out old),"make original entry writable");
        originalProtection=old;
        byte[] jump=new byte[5];jump[0]=0xe9;
        Buffer.BlockCopy(BitConverter.GetBytes(unchecked(trampoline.ToInt32()-hookAddress.ToInt32()-5)),0,jump,1,4);
        Marshal.Copy(jump,0,hookAddress,5);hooked=true;
        Check(FlushInstructionCache(GetCurrentProcess(),hookAddress,new UIntPtr(5)),"flush entry hook");
        Check(VirtualProtect(hookAddress,new UIntPtr(5),old,out old),"restore entry protection");
    }
    private static void EndProbe()
    {
        if(hooked)
        {
            uint old;
            if(!VirtualProtect(hookAddress,new UIntPtr(5),0x40,out old)) throw new Exception("Cannot restore original entry protection");
            Marshal.Copy(savedBytes,0,hookAddress,5);
            if(!FlushInstructionCache(GetCurrentProcess(),hookAddress,new UIntPtr(5))) throw new Exception("Cannot flush restored original entry");
            if(!VirtualProtect(hookAddress,new UIntPtr(5),originalProtection,out old)) throw new Exception("Cannot restore original protection");
            for(int i=0;i<7;i++) if(Marshal.ReadByte(hookAddress,i)!=savedBytes[i]) throw new Exception("Original entry restoration mismatch");
            hooked=false;
        }
        if(trampoline!=IntPtr.Zero)
        {
            if(!VirtualFree(trampoline,UIntPtr.Zero,0x8000)) throw new Exception("Cannot free probe trampoline");
            trampoline=IntPtr.Zero;
        }
        GC.KeepAlive(traceRoot);
    }
    private static int Main(string[] args)
    {
        try
        {
            Check(IntPtr.Size==4, "x86 host required");
            if(args.Length!=2) throw new ArgumentException("Usage: TextureCubicIntermediateProbe original.dll float-transcript.txt original-transcript.txt");
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
            object plugin=Activator.CreateInstance(type);FieldInfo pf=type.GetField("_Platform",BindingFlags.Instance|BindingFlags.NonPublic);pf.SetValue(plugin,Enum.Parse(pf.FieldType,"PlayStation3"));
            MethodInfo optionsMethod=type.GetMethod("GetNVCompressionOptions",BindingFlags.Instance|BindingFlags.NonPublic);
            IntPtr module=Marshal.GetHINSTANCE(original.ManifestModule);
            Check(module!=IntPtr.Zero && module!=new IntPtr(-1),"original native image base unavailable");
            MethodBase native=original.ManifestModule.ResolveMethod(0x060000DC);
            Check(native.Name=="nvDDS.nvDXTcompress", "native RGBAImage metadata token changed");
            Compressor compress=(Compressor)Marshal.GetDelegateForFunctionPointer(new IntPtr(module.ToInt32()+0x52B60),typeof(Compressor));
            MethodInfo encode=null;
            List<string> baseline=new List<string>();
            uint[,] dimensions={{1,1},{1,2},{2,1},{2,2},{4,4},{8,4},{4,8},{16,16},{64,4}};
            string[] formats={"D3DFMT_A8R8G8B8","D3DFMT_X8R8G8B8","D3DFMT_R5G6B5"};
            for(int pass=0;pass<2;pass++)
            {
            fixtures=0;
            if(pass==1){Check(baseline.Count==270,"unmodified native baseline incomplete");BeginProbe(module);}
            using(StreamWriter transcript=new StreamWriter(args[1],false,new UTF8Encoding(false)))
            {
                foreach(string format in formats)
                    for(int mip=0;mip<2;mip++)
                        for(int d=0;d<dimensions.GetLength(0);d++)
                            for(int pattern=0;pattern<5;pattern++)
                            {
                                uint width=dimensions[d,0],height=dimensions[d,1];
                                byte[] pixels=Pixels(width,height,pattern), source=(byte[])pixels.Clone();
                                captureIntermediate=pass==1 && format=="D3DFMT_A8R8G8B8" && mip==1 && width==4 && height==8 && pattern==4;
                                byte[] output=Original(compress,plugin,optionsMethod,width,height,pixels,format,mip!=0);
                                string chunkInfo=String.Join(";",calls.ToArray());
                                Check(Hash(pixels)==Hash(source),"original changed borrowed RGBA storage");
                                if(encode!=null && mip==0)
                                {
                                    byte[] actual=(byte[])encode.Invoke(null,new object[]{width,height,(byte[])source.Clone(),format,false});
                                    Check(output.Length==actual.Length,"DDS output length " + format + " " + width+"x"+height);
                                    for(int i=0;i<output.Length;i++) Check(output[i]==actual[i],"DDS byte " + i + " " + format + " " + width+"x"+height + " pattern="+pattern);
                                }
                                string line=String.Format("dds|{0}|{1}|{2}|{3}|{4}|{5}|{6}",format,mip,width,height,pattern,chunkInfo,Convert.ToBase64String(output));
                                if(pass==0)baseline.Add(line);
                                else{Check(line==baseline[fixtures],"Probe changed original DDS bytes or callback metadata fixture="+fixtures);
                                transcript.WriteLine(line+"|intermediates="+String.Join(";",floatBits.ToArray()));}
                                fixtures++;
                            }
            }
            }
            Check(tracedCalls>0 && traceError==null,"intermediate capture "+traceError);
            Console.WriteLine("PASS TextureCubicIntermediateProbe fixtures={0} checks={1} unchangedOriginalDds=true fullFpStatePreserved=true recoveredAccepted=false intermediateCalls={2}",fixtures,checks,tracedCalls);
            return 0;
        }
        catch(Exception e) { Console.Error.WriteLine(e);return 1; }
        finally { EndProbe(); }
    }
}


