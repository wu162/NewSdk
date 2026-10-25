using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

// Actual original CPU oracle for complete 0x855ef spectral attenuation; no codec routing.
internal static class AudioXmaSpectralGateOracle
{
    private const string Sha = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139";
    private const int Cases = 2400;
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode)] private static extern IntPtr GetModuleHandle(string name);
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode)] private static extern uint GetModuleFileName(IntPtr module, System.Text.StringBuilder path, int count);
    [DllImport("kernel32.dll")] private static extern IntPtr VirtualAlloc(IntPtr address, UIntPtr size, uint allocation, uint protection);
    [DllImport("kernel32.dll")] private static extern bool VirtualProtect(IntPtr address, UIntPtr size, uint protection, out uint oldProtection);
    [DllImport("kernel32.dll")] private static extern bool VirtualFree(IntPtr address, UIntPtr size, uint kind);
    [DllImport("kernel32.dll")] private static extern IntPtr GetCurrentProcess();
    [DllImport("kernel32.dll")] private static extern bool FlushInstructionCache(IntPtr process, IntPtr address, UIntPtr size);
    [DllImport("msvcrt.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="_controlfp")] private static extern uint ManagedControl(uint value, uint mask);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint Control(uint value, uint mask);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int Gate(IntPtr core,IntPtr state,IntPtr channel,int unused);
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
    private static float[] Input(int n,int pattern)
    {float[] a=new float[n];uint random=0x12345678;for(int i=0;i<n;++i){random=unchecked(random*1664525u+1013904223u);a[i]=pattern==0?((i&1)==0?0f:Float(unchecked((int)0x80000000))):pattern==1?(i==n/3?32767f:0f):pattern==2?(((int)(random&65535))-32768):Float(unchecked((int)((random&0x7fffff)|((random&1)<<31))));}return a;}
    private static float[] Bands(int mode,bool upper)
    {
        float[] a=new float[128];for(int i=0;i<128;++i){switch(mode){case 0:a[i]=upper?1f:0.5f;break;case 1:a[i]=upper?0f:1f;break;case 2:a[i]=upper?1f:2f;break;case 3:a[i]=upper?((i&1)==0?2f:-2f):(i%3==0?-1f:1f);break;case 4:a[i]=upper?Float((i%7)+1):Float((i%5)+1);break;default:a[i]=upper?((i%3)==0?Float(unchecked((int)0x80000000)):1f):((i&1)==0?0f:Float(unchecked((int)0x80000000)));break;}}return a;
    }
    private static byte[] Native(Gate gate,float[] spectrum,int control,bool enabled,float[] lower,float[] upper)
    {
        byte[] input=new byte[spectrum.Length*4],lb=new byte[512],ub=new byte[512];Buffer.BlockCopy(spectrum,0,input,0,input.Length);if(lower!=null)Buffer.BlockCopy(lower,0,lb,0,512);if(upper!=null)Buffer.BlockCopy(upper,0,ub,0,512);
        using(Guarded core=new Guarded(new byte[0x18ae0]))using(Guarded state=new Guarded(new byte[0x218]))using(Guarded channel=new Guarded(new byte[0x54]))using(Guarded parameters=new Guarded(new byte[0x80]))using(Guarded output=new Guarded(input))using(Guarded l=new Guarded(lb))using(Guarded u=new Guarded(ub))
        {
            Marshal.WriteInt32(At(core.Pointer,0x18adc),enabled?1:0);Marshal.WriteInt32(At(state.Pointer,0x214),control);Marshal.WriteIntPtr(channel.Pointer,parameters.Pointer);Marshal.WriteIntPtr(At(channel.Pointer,0x3c),lower==null?IntPtr.Zero:l.Pointer);Marshal.WriteIntPtr(At(channel.Pointer,0x40),upper==null?IntPtr.Zero:u.Pointer);Marshal.WriteIntPtr(At(channel.Pointer,0x50),output.Pointer);Marshal.WriteInt16(At(parameters.Pointer,0x7c),(short)spectrum.Length);
            byte[] cb=core.Read(),sb=state.Read(),chb=channel.Read(),pb=parameters.Read();Require(gate(core.Pointer,state.Pointer,channel.Pointer,unchecked((int)0xdeadbeef))==0,"Original gate HRESULT");Equal(cb,core.Read(),"Gate core immutable");Equal(sb,state.Read(),"Gate state immutable");Equal(chb,channel.Read(),"Gate channel immutable");Equal(pb,parameters.Read(),"Gate parameters immutable");Equal(lb,l.Read(),"Gate lower bands immutable");Equal(ub,u.Read(),"Gate upper bands immutable");return output.Read();
        }
    }
    private static byte[] Candidate(MethodInfo process,float[] spectrum,int control,bool enabled,float[] lower,float[] upper)
    {byte[] lb=lower==null?null:new byte[512],ub=upper==null?null:new byte[512];if(lb!=null)Buffer.BlockCopy(lower,0,lb,0,512);if(ub!=null)Buffer.BlockCopy(upper,0,ub,0,512);Require((int)process.Invoke(null,new object[]{spectrum,control,enabled,lower,upper})==0,"Candidate gate HRESULT");byte[] a=new byte[spectrum.Length*4];Buffer.BlockCopy(spectrum,0,a,0,a.Length);if(lb!=null){byte[] b=new byte[512];Buffer.BlockCopy(lower,0,b,0,512);Equal(lb,b,"Candidate lower bands immutable");}if(ub!=null){byte[] b=new byte[512];Buffer.BlockCopy(upper,0,b,0,512);Equal(ub,b,"Candidate upper bands immutable");}return a;}
    private static void Refuse(MethodInfo process,object[] args,Type expected)
    {float[] input=(float[])args[0];byte[] before=input==null?null:new byte[input.Length*4];if(before!=null)Buffer.BlockCopy(input,0,before,0,before.Length);try{process.Invoke(null,args);}catch(TargetInvocationException e){Require(e.InnerException.GetType()==expected,"Wrong spectral gate refusal");if(before!=null){byte[] after=new byte[before.Length];Buffer.BlockCopy(input,0,after,0,after.Length);Equal(before,after,"Spectral gate refusal mutated input");}return;}throw new Exception("Missing spectral gate refusal");}
    private static int Main(string[] args)
    {
        IntPtr thunk=IntPtr.Zero;Control control=null;Word word=null;uint oldControl=0,oldWord=0;bool configured=false;
        try
        {
            Require(IntPtr.Size==4,"x86 host required");Require(args.Length==3&&(args[0]=="original"||args[0]=="candidate"),"Usage original|candidate assembly.dll original.tsv.gz");bool original=args[0]=="original";Gate native=null;IntPtr module=IntPtr.Zero;MethodInfo process=null;
            if(original){module=Original(args[1]);native=(Gate)Marshal.GetDelegateForFunctionPointer(At(module,0x855ef),typeof(Gate));control=(Control)Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module,0xab210)),typeof(Control));}
            else{Require(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll")==IntPtr.Zero&&!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.AudioCompiler.dll")),"Original DLL present in candidate host");process=Assembly.LoadFrom(Path.GetFullPath(args[1])).GetType("BinaryAssetBuilder.AudioCompiler.XmaSpectralGate",true).GetMethod("Process");Require(process!=null,"Candidate spectral gate absent");control=ManagedControl;}
            byte[] code={0x83,0xec,0x04,0xd9,0x3c,0x24,0x0f,0xb7,0x04,0x24,0x83,0xc4,0x04,0xc3};thunk=VirtualAlloc(IntPtr.Zero,new UIntPtr((uint)code.Length),0x3000,4);Require(thunk!=IntPtr.Zero,"CW allocation");Marshal.Copy(code,0,thunk,code.Length);uint protection;Require(VirtualProtect(thunk,new UIntPtr((uint)code.Length),0x20,out protection)&&FlushInstructionCache(GetCurrentProcess(),thunk,new UIntPtr((uint)code.Length)),"CW protection");word=(Word)Marshal.GetDelegateForFunctionPointer(thunk,typeof(Word));oldControl=control(0,0);oldWord=word();control(0x10000,0x30300);configured=true;Require(word()==0x27f,"Physical CW differs");Console.WriteLine("x87 physicalCW=0x027f originalPhysicalCW=0x"+oldWord.ToString("x4"));
            StreamWriter writer=null;StreamReader reader=null;int cases=0;
            try
            {
                string header="# originalDllSha256="+Sha+" nativeRva=0x855ef sizes=128,256,512,1024,2048 physicalCW=0x027f cases="+Cases;
                if(original){writer=new StreamWriter(new GZipStream(File.Create(args[2]),CompressionMode.Compress));writer.WriteLine(header);}else{reader=new StreamReader(new GZipStream(File.OpenRead(args[2]),CompressionMode.Decompress));Require(reader.ReadLine()==header,"Gate original header mismatch");}
                foreach(int n in new int[]{128,256,512,1024,2048})foreach(int controlValue in new int[]{Int32.MinValue,-1,0,99,100,101,199,200,201,Int32.MaxValue})foreach(bool enabled in new bool[]{false,true})for(int mode=0;mode<6;++mode)for(int pattern=0;pattern<4;++pattern)
                {
                    bool absent=!enabled||controlValue>200;float[] input=Input(n,pattern),lower=absent?null:Bands(mode,false),upper=absent?null:Bands(mode,true);byte[] output=original?Native(native,input,controlValue,enabled,lower,upper):Candidate(process,input,controlValue,enabled,lower,upper);
                    string label=n+":"+controlValue+":"+enabled+":"+mode+":"+pattern;
                    if(original)writer.WriteLine(label+"\t"+Convert.ToBase64String(output));else{string line=reader.ReadLine();Require(line!=null,"Missing original gate row");string[] f=line.Split('\t');Require(f.Length==2&&f[0]==label,"Gate fixture order differs");Equal(Convert.FromBase64String(f[1]),output,label+" complete spectrum bits");}Require(word()==0x27f,"Gate changed CW");++cases;
                }
                Require(cases==Cases,"Incomplete spectral gate matrix");if(!original)Require(reader.ReadLine()==null,"Extra original gate rows");
            }
            finally{if(writer!=null)writer.Dispose();if(reader!=null)reader.Dispose();}
            if(!original)
            {
                object[][] bad={new object[]{null,0,true,new float[128],new float[128]},new object[]{new float[64],0,true,new float[128],new float[128]},new object[]{new float[129],0,true,new float[128],new float[128]},new object[]{new float[4096],0,true,new float[128],new float[128]},new object[]{new float[128],0,true,null,new float[128]},new object[]{new float[128],0,true,new float[128],null},new object[]{new float[128],0,true,new float[127],new float[128]},new object[]{new float[128],0,true,new float[128],new float[129]},new object[]{new float[128],0,true,new float[128],new float[128]},new object[]{new float[128],0,true,new float[128],new float[128]},new object[]{new float[128],0,true,new float[128],new float[128]}};
                ((float[])bad[8][0])[5]=Single.NaN;((float[])bad[9][3])[11]=Single.PositiveInfinity;((float[])bad[10][4])[17]=Single.NegativeInfinity;
                for(int i=0;i<bad.Length;++i)Refuse(process,bad[i],i==0||i==4||i==5?typeof(ArgumentNullException):i==6||i==7?typeof(ArgumentException):typeof(ArgumentOutOfRangeException));Require(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll")==IntPtr.Zero,"Original loaded after candidate");
            }
            Console.WriteLine("PASS "+args[0]+" XMA spectral gate: "+cases+" complete spectrum/HRESULT/state comparisons; five sizes, ten controls including200 boundary, both enable states, six band patterns, four inputs; "+(original?"actual original CPU":"eleven separate safety refusals; original DLL absent")+"; no quantization/entropy/XMA-route/SDK acceptance");return 0;
        }
        catch(Exception e){Console.Error.WriteLine(e);return 1;}
        finally{try{if(configured){control(oldControl,0x30300);Require(word()==oldWord,"CW not restored");}}finally{if(thunk!=IntPtr.Zero)VirtualFree(thunk,UIntPtr.Zero,0x8000);}}
    }
}
