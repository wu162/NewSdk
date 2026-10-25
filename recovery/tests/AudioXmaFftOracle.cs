using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

// Actual original CPU oracle for the complete table-backed complex FFT. No codec routing.
internal static class AudioXmaFftOracle
{
    private const string Sha = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139";
    private const int Cases = 192;
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode)] private static extern IntPtr GetModuleHandle(string name);
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode)] private static extern uint GetModuleFileName(IntPtr module, System.Text.StringBuilder path, int count);
    [DllImport("kernel32.dll")] private static extern IntPtr VirtualAlloc(IntPtr address, UIntPtr size, uint allocation, uint protection);
    [DllImport("kernel32.dll")] private static extern bool VirtualProtect(IntPtr address, UIntPtr size, uint protection, out uint oldProtection);
    [DllImport("kernel32.dll")] private static extern bool VirtualFree(IntPtr address, UIntPtr size, uint kind);
    [DllImport("kernel32.dll")] private static extern IntPtr GetCurrentProcess();
    [DllImport("kernel32.dll")] private static extern bool FlushInstructionCache(IntPtr process, IntPtr address, UIntPtr size);
    [DllImport("msvcrt.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="_controlfp")] private static extern uint ManagedControl(uint value, uint mask);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint Control(uint value, uint mask);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate void Fft(IntPtr data,int order,int inverse);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint Word();
    private static IntPtr At(IntPtr p, int n) { return new IntPtr(unchecked(p.ToInt32()+n)); }
    private static void Require(bool yes, string label) { if (!yes) throw new Exception(label); }
    private static byte[] Bytes(int size) { byte[] a=new byte[size]; for(int i=0;i<size;++i)a[i]=0xa5;return a; }
    private static void Equal(byte[] a, byte[] b, string label)
    { Require(a.Length==b.Length,label+" length");for(int i=0;i<a.Length;++i)if(a[i]!=b[i])throw new Exception(label+" byte "+i+": "+a[i]+" != "+b[i]); }
    private sealed class Guarded : IDisposable
    {
        private readonly IntPtr memory; public readonly IntPtr Pointer; private readonly int size;
        public Guarded(byte[] bytes) {size=bytes.Length; memory=Marshal.AllocHGlobal(size+64);Pointer=At(memory,32);Marshal.Copy(Bytes(size+64),0,memory,size+64);Marshal.Copy(bytes,0,Pointer,size);}
        public byte[] Read(){byte[] a=new byte[size];Marshal.Copy(Pointer,a,0,size);return a;}
        public void Dispose(){try{byte[] a=new byte[32];Marshal.Copy(memory,a,0,32);Equal(Bytes(32),a,"prefix guard");Marshal.Copy(At(Pointer,size),a,0,32);Equal(Bytes(32),a,"suffix guard");}finally{Marshal.FreeHGlobal(memory);}}
    }
    private static IntPtr Original(string path)
    {
        path=Path.GetFullPath(path); using(SHA256 h=SHA256.Create())Require(BitConverter.ToString(h.ComputeHash(File.ReadAllBytes(path))).Replace("-","").ToLowerInvariant()==Sha,"DLL SHA mismatch");
        Assembly a=Assembly.LoadFrom(path);Require(String.Equals(Path.GetFullPath(a.Location),path,StringComparison.OrdinalIgnoreCase),"Original assembly load path");
        IntPtr p=GetModuleHandle(Path.GetFileName(path));var s=new System.Text.StringBuilder(32768);
        Require(p!=IntPtr.Zero&&GetModuleFileName(p,s,s.Capacity)!=0&&String.Equals(Path.GetFullPath(s.ToString()),path,StringComparison.OrdinalIgnoreCase),"Original native load path");return p;
    }
    private static void Put(byte[] a,int offset,int v){Array.Copy(BitConverter.GetBytes(v),0,a,offset,4);}
    private static float Float(int bits){return BitConverter.ToSingle(BitConverter.GetBytes(bits),0);}
    private static float[] Input(int order,int pattern)
    {
        float[] values=new float[2<<order];uint random=0x12345678;
        for(int i=0;i<values.Length;++i){random=unchecked(random*1664525u+1013904223u);switch(pattern){case 0:values[i]=(i&1)==0?0f:Float(unchecked((int)0x80000000));break;case 1:values[i]=i==0?1f:0f;break;case 2:values[i]=(((int)(random&65535))-32768)*(1f/8192f);break;case 3:values[i]=(i&1)==0?32767f:-32768f;break;case 4:values[i]=Float(unchecked((int)((random&0x007fffff)|((random&1)<<31))));break;case 5:values[i]=Float(unchecked((int)((0x3f800000u+(random&3)-1)|((random&1)<<31))));break;}}return values;
    }
    private static void Refuse(MethodInfo transform,object[] args,Type expected)
    {float[] input=(float[])args[0];byte[] before=input==null?null:new byte[input.Length*4];if(before!=null)Buffer.BlockCopy(input,0,before,0,before.Length);try{transform.Invoke(null,args);}catch(TargetInvocationException e){Require(e.InnerException.GetType()==expected,"Wrong FFT safety rejection");if(before!=null){byte[] after=new byte[before.Length];Buffer.BlockCopy(input,0,after,0,after.Length);Equal(before,after,"FFT refusal changed input");}return;}throw new Exception("Missing FFT rejection");}
    private static int Main(string[] args)
    {
        IntPtr thunk=IntPtr.Zero;Control control=null;Word word=null;uint oldControl=0,oldWord=0;bool configured=false;
        try
        {
            Require(IntPtr.Size==4,"x86 host required");Require(args.Length==3&&(args[0]=="original"||args[0]=="candidate"),"Usage original|candidate assembly.dll original.tsv.gz");bool original=args[0]=="original";Fft native=null;MethodInfo transform=null;
            if(original){IntPtr module=Original(args[1]);native=(Fft)Marshal.GetDelegateForFunctionPointer(At(module,0xa7de3),typeof(Fft));control=(Control)Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module,0xab210)),typeof(Control));}
            else{Require(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll")==IntPtr.Zero&&!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.AudioCompiler.dll")),"Original DLL present in candidate host");transform=Assembly.LoadFrom(Path.GetFullPath(args[1])).GetType("BinaryAssetBuilder.AudioCompiler.XmaComplexFft",true).GetMethod("Transform");Require(transform!=null,"Candidate FFT absent");control=ManagedControl;}
            byte[] code={0x83,0xec,0x04,0xd9,0x3c,0x24,0x0f,0xb7,0x04,0x24,0x83,0xc4,0x04,0xc3};thunk=VirtualAlloc(IntPtr.Zero,new UIntPtr((uint)code.Length),0x3000,4);Require(thunk!=IntPtr.Zero,"CW allocation");Marshal.Copy(code,0,thunk,code.Length);uint protection;Require(VirtualProtect(thunk,new UIntPtr((uint)code.Length),0x20,out protection)&&FlushInstructionCache(GetCurrentProcess(),thunk,new UIntPtr((uint)code.Length)),"CW protection");word=(Word)Marshal.GetDelegateForFunctionPointer(thunk,typeof(Word));oldControl=control(0,0);oldWord=word();control(0x10000,0x30300);configured=true;Require(word()==0x27f,"Physical CW differs");Console.WriteLine("x87 physicalCW=0x027f originalPhysicalCW=0x"+oldWord.ToString("x4"));
            StreamWriter writer=null;StreamReader reader=null;int cases=0;
            try
            {
                string header="# originalDllSha256="+Sha+" nativeRva=0xa7de3 orders=0..15 directions=0,1 physicalCW=0x027f cases="+Cases;
                if(original){writer=new StreamWriter(new GZipStream(File.Create(args[2]),CompressionMode.Compress));writer.WriteLine(header);}else{reader=new StreamReader(new GZipStream(File.OpenRead(args[2]),CompressionMode.Decompress));Require(reader.ReadLine()==header,"FFT original header mismatch");}
                for(int order=0;order<=15;++order)foreach(bool inverse in new bool[]{false,true})for(int pattern=0;pattern<6;++pattern)
                {
                    float[] data=Input(order,pattern);byte[] output=new byte[data.Length*4];Buffer.BlockCopy(data,0,output,0,output.Length);
                    if(original){using(Guarded buffer=new Guarded(output)){native(buffer.Pointer,order,inverse?1:0);output=buffer.Read();Buffer.BlockCopy(output,0,data,0,output.Length);}}
                    else{transform.Invoke(null,new object[]{data,order,inverse});Buffer.BlockCopy(data,0,output,0,output.Length);}
                    string label=order+":"+inverse+":"+pattern;
                    if(original)writer.WriteLine(label+"\t"+Convert.ToBase64String(output));else{string line=reader.ReadLine();Require(line!=null,"Missing original FFT");string[] f=line.Split('\t');Require(f.Length==2&&f[0]==label,"FFT fixture order differs");Equal(Convert.FromBase64String(f[1]),output,label+" complete real/imaginary bits");}
                    if(pattern==0)foreach(float value in data)Require(value==0f,"Zero FFT invariant");if(pattern==1)for(int i=0;i<data.Length;i+=2)Require(data[i]==(inverse?1f/(1<<order):1f)&&data[i+1]==0f,"Impulse FFT independent invariant");Require(word()==0x27f,"FFT changed CW");++cases;
                }
                Require(cases==Cases,"Incomplete FFT matrix");if(!original)Require(reader.ReadLine()==null,"Extra FFT rows");
            }
            finally{if(writer!=null)writer.Dispose();if(reader!=null)reader.Dispose();}
            if(!original)
            {
                Refuse(transform,new object[]{null,0,false},typeof(ArgumentNullException));Refuse(transform,new object[]{new float[2],-1,false},typeof(ArgumentOutOfRangeException));Refuse(transform,new object[]{new float[2],16,false},typeof(ArgumentOutOfRangeException));Refuse(transform,new object[]{new float[1],0,false},typeof(ArgumentException));Refuse(transform,new object[]{new float[]{0,Single.NaN},0,false},typeof(ArgumentOutOfRangeException));Refuse(transform,new object[]{new float[]{Single.PositiveInfinity,0},0,false},typeof(ArgumentOutOfRangeException));Require(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll")==IntPtr.Zero,"Original loaded after candidate");
            }
            Console.WriteLine("PASS "+args[0]+" XMA complex FFT: 192 complete complex-bit comparisons; orders0..15, both directions, six patterns including denormals; "+(original?"actual original CPU":"six separate safety refusals; original DLL absent")+"; no DCT/codec/SDK acceptance");return 0;
        }
        catch(Exception e){Console.Error.WriteLine(e);return 1;}
        finally{try{if(configured){control(oldControl,0x30300);Require(word()==oldWord,"CW not restored");}}finally{if(thunk!=IntPtr.Zero)VirtualFree(thunk,UIntPtr.Zero,0x8000);}}
    }
}
