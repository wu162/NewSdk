using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

// Actual original CPU oracle for 0x64cd5 / 0x65a0b only. No codec routing.
internal static class AudioXmaNoiseOracle
{
    private const string Sha = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139";
    private const int Cases = 6360;
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode)] private static extern IntPtr GetModuleHandle(string name);
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode)] private static extern uint GetModuleFileName(IntPtr module, System.Text.StringBuilder path, int count);
    [DllImport("kernel32.dll")] private static extern IntPtr VirtualAlloc(IntPtr address, UIntPtr size, uint allocation, uint protection);
    [DllImport("kernel32.dll")] private static extern bool VirtualProtect(IntPtr address, UIntPtr size, uint protection, out uint oldProtection);
    [DllImport("kernel32.dll")] private static extern bool VirtualFree(IntPtr address, UIntPtr size, uint kind);
    [DllImport("kernel32.dll")] private static extern IntPtr GetCurrentProcess();
    [DllImport("kernel32.dll")] private static extern bool FlushInstructionCache(IntPtr process, IntPtr address, UIntPtr size);
    [DllImport("msvcrt.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="_controlfp")] private static extern uint ManagedControl(uint value, uint mask);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint Control(uint value, uint mask);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int Next(IntPtr state);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate void Fill(IntPtr state, IntPtr destination);
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
    private static uint[] Seeds={0,1,0xffffffff,0x80000000,0x7fffffff,0x19660d,0x3c6ef35f,0x12345678};
    private static int[] Previous={0,-1,Int32.MinValue,Int32.MaxValue,0x12345678};
    private static int[] Gains={0,unchecked((int)0x80000000),0x3f800000,unchecked((int)0xbf800000),0x3f000000,unchecked((int)0xbf000000),1,unchecked((int)0x80000001),0x007fffff,unchecked((int)0x807fffff),0x00800000,unchecked((int)0x80800000),0x7f7fffff,unchecked((int)0xff7fffff),0x358637bd,unchecked((int)0xb58637bd),0x3c23d70a,0x3f7fffff,0x3f800001,0x4b000001,0x12345678,0x3d23d70a};
    private static int[] NextCounts={1,2,31,257,4096};
    private static int[] FillCounts={0,1,2,3,31,257,1024};
    private static float Float(int bits){return BitConverter.ToSingle(BitConverter.GetBytes(bits),0);}
    private static void NativeCase(bool fill,uint seed,int previous,int gain,int count,Next next,Fill noise,out uint endSeed,out int endPrevious,out byte[] output)
    {
        using(Guarded state=new Guarded(Bytes(4))) using(Guarded config=new Guarded(Bytes(0x218))) using(Guarded dest=new Guarded(Bytes(checked(count*4+24))))
        {
            Marshal.WriteIntPtr(state.Pointer,config.Pointer);Marshal.WriteInt32(At(config.Pointer,0x210),previous);Marshal.WriteInt32(At(config.Pointer,0x214),unchecked((int)seed));
            Marshal.WriteInt32(At(config.Pointer,0x104),count);Marshal.WriteInt32(At(config.Pointer,0x128),gain);
            byte[] before=state.Read(),configBefore=config.Read();
            if(fill)noise(state.Pointer,At(dest.Pointer,12));
            else for(int i=0;i<count;++i)Marshal.WriteInt32(At(dest.Pointer,12+i*4),next(At(config.Pointer,0x210)));
            endPrevious=Marshal.ReadInt32(At(config.Pointer,0x210));endSeed=unchecked((uint)Marshal.ReadInt32(At(config.Pointer,0x214)));
            Put(configBefore,0x210,endPrevious);Put(configBefore,0x214,unchecked((int)endSeed));Equal(before,state.Read(),"native outer state immutable");Equal(configBefore,config.Read(),"native unrelated configuration immutable");
            output=dest.Read();
        }
    }
    private static void CandidateCase(bool fill,uint seed,int previous,int gain,int count,MethodInfo next,MethodInfo noise,out uint endSeed,out int endPrevious,out byte[] output)
    {
        output=Bytes(count*4+24);object[] invoke;
        if(fill)
        {
            float[] dest=new float[count+6];for(int i=0;i<dest.Length;++i)dest[i]=Float(unchecked((int)0xa5a5a5a5));
            invoke=new object[]{seed,previous,Float(gain),dest,3,count};noise.Invoke(null,invoke);Buffer.BlockCopy(dest,0,output,0,output.Length);
        }
        else
        {
            invoke=new object[]{seed,previous};for(int i=0;i<count;++i)Put(output,12+i*4,(int)next.Invoke(null,invoke));
        }
        endSeed=(uint)invoke[0];endPrevious=(int)invoke[1];
    }
    private static void Refuse(MethodInfo method,object[] args,Type expected)
    {
        try{method.Invoke(null,args);}catch(TargetInvocationException e){Require(e.InnerException.GetType()==expected,"Wrong rejection");Require((uint)args[0]==1u&&(int)args[1]==2,"Rejection mutated state");return;}throw new Exception("Missing rejection");
    }
    private static int Main(string[] args)
    {
        IntPtr thunk=IntPtr.Zero;Control control=null;Word word=null;uint oldControl=0,oldWord=0;bool configured=false;
        try
        {
            Require(IntPtr.Size==4,"x86 host required");Require(args.Length==3&&(args[0]=="original"||args[0]=="candidate"),"Usage: original|candidate assembly.dll original.tsv.gz");
            bool original=args[0]=="original";Next next=null;Fill fill=null;MethodInfo nextMethod=null,fillMethod=null;
            if(original)
            {
                IntPtr p=Original(args[1]);next=(Next)Marshal.GetDelegateForFunctionPointer(At(p,0x64cd5),typeof(Next));fill=(Fill)Marshal.GetDelegateForFunctionPointer(At(p,0x65a0b),typeof(Fill));
                control=(Control)Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(p,0xab210)),typeof(Control));
            }
            else
            {
                Require(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll")==IntPtr.Zero,"Original DLL loaded in recovered process");
                Require(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.AudioCompiler.dll")),"Original DLL file present in recovered host");
                Assembly a=Assembly.LoadFrom(Path.GetFullPath(args[1]));Type t=a.GetType("BinaryAssetBuilder.AudioCompiler.XmaNoiseGenerator",true);nextMethod=t.GetMethod("Next");fillMethod=t.GetMethod("Fill");Require(nextMethod!=null&&fillMethod!=null,"Candidate API absent");control=ManagedControl;
            }
            byte[] code={0x83,0xec,0x04,0xd9,0x3c,0x24,0x0f,0xb7,0x04,0x24,0x83,0xc4,0x04,0xc3};
            thunk=VirtualAlloc(IntPtr.Zero,new UIntPtr((uint)code.Length),0x3000,4);Require(thunk!=IntPtr.Zero,"CW thunk allocation");Marshal.Copy(code,0,thunk,code.Length);uint oldProtection;
            Require(VirtualProtect(thunk,new UIntPtr((uint)code.Length),0x20,out oldProtection)&&FlushInstructionCache(GetCurrentProcess(),thunk,new UIntPtr((uint)code.Length)),"CW thunk protection");word=(Word)Marshal.GetDelegateForFunctionPointer(thunk,typeof(Word));
            oldControl=control(0,0);oldWord=word();control(0x10000,0x30300);configured=true;Require(word()==0x027f,"Physical x87 CW must be 0x027f");
            Console.WriteLine("x87 physicalCW=0x"+word().ToString("x4")+" originalPhysicalCW=0x"+oldWord.ToString("x4")+" CRTcontrolBefore=0x"+oldControl.ToString("x8")+"; 53-bit nearest");
            int records=0;StreamWriter writer=null;StreamReader reader=null;
            try
            {
                if(original){writer=new StreamWriter(new GZipStream(File.Create(args[2]),CompressionMode.Compress));writer.WriteLine("# originalDllSha256="+Sha+" nativeRvas=0x64cd5,0x65a0b physicalCW=0x027f cases="+Cases);}
                else {reader=new StreamReader(new GZipStream(File.OpenRead(args[2]),CompressionMode.Decompress));Require(reader.ReadLine()=="# originalDllSha256="+Sha+" nativeRvas=0x64cd5,0x65a0b physicalCW=0x027f cases="+Cases,"Original evidence header mismatch");}
                foreach(bool isFill in new bool[]{false,true})foreach(uint seed in Seeds)foreach(int previous in Previous)foreach(int gain in isFill?Gains:new int[]{0})foreach(int count in isFill?FillCounts:NextCounts)
                {
                    uint endSeed;int endPrevious;byte[] output;
                    if(original)NativeCase(isFill,seed,previous,gain,count,next,fill,out endSeed,out endPrevious,out output);
                    else CandidateCase(isFill,seed,previous,gain,count,nextMethod,fillMethod,out endSeed,out endPrevious,out output);
                    string label=(isFill?"fill":"next")+"\t"+seed+"\t"+previous+"\t"+gain+"\t"+count;
                    if(original)writer.WriteLine(label+"\t"+endSeed+"\t"+endPrevious+"\t"+Convert.ToBase64String(output));
                    else
                    {
                        string line=reader.ReadLine();Require(line!=null,"Missing fixture "+label);string[] f=line.Split('\t');Require(f.Length==8&&String.Join("\t",f,0,5)==label,"Unexpected fixture ordering "+label);
                        Require(endSeed==UInt32.Parse(f[5])&&endPrevious==Int32.Parse(f[6]),"State mismatch "+label);Equal(Convert.FromBase64String(f[7]),output,label+" exact output/guards");
                    }
                    Require(word()==0x027f,"Native/candidate changed physical CW");++records;
                }
                Require(records==Cases,"Incomplete noise/state matrix");if(!original)Require(reader.ReadLine()==null,"Extra fixture rows");
            }
            finally {if(writer!=null)writer.Dispose();if(reader!=null)reader.Dispose();}
            if(!original)
            {
                Refuse(fillMethod,new object[]{1u,2,1f,null,0,0},typeof(ArgumentNullException));
                Refuse(fillMethod,new object[]{1u,2,1f,new float[2],-1,1},typeof(ArgumentOutOfRangeException));
                Refuse(fillMethod,new object[]{1u,2,1f,new float[2],0,-1},typeof(ArgumentOutOfRangeException));
                Refuse(fillMethod,new object[]{1u,2,1f,new float[2],1,2},typeof(ArgumentOutOfRangeException));
                Refuse(fillMethod,new object[]{1u,2,1f,new float[2],Int32.MaxValue,1},typeof(ArgumentOutOfRangeException));
                Refuse(fillMethod,new object[]{1u,2,Single.NaN,new float[2],0,1},typeof(ArgumentOutOfRangeException));
                Refuse(fillMethod,new object[]{1u,2,Single.PositiveInfinity,new float[2],0,1},typeof(ArgumentOutOfRangeException));
                Refuse(fillMethod,new object[]{1u,2,Single.NegativeInfinity,new float[2],0,1},typeof(ArgumentOutOfRangeException));
                Require(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll")==IntPtr.Zero,"Original DLL loaded after candidate run");
            }
            Console.WriteLine("PASS "+args[0]+" XMA noise numeric helpers: "+records+" exact state/output cases (200 PRNG sequences, 6160 fills); 22 finite gain patterns; x87 CW=0x027f; "+(original?"actual original CPU":"8 separate safety refusals; original DLL absent")+"; no XMA encoding acceptance");return 0;
        }
        catch(Exception e){Console.Error.WriteLine(e);return 1;}
        finally{try{if(configured){control(oldControl,0x30300);Require(word()==oldWord,"Physical x87 CW was not restored");}}finally{if(thunk!=IntPtr.Zero)VirtualFree(thunk,UIntPtr.Zero,0x8000);}}
    }
}
