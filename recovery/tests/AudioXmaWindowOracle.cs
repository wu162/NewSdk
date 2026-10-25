using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

// Actual CPU oracle for six integral PCM window/copy leaves. No codec routing.
internal static class AudioXmaWindowOracle
{
    private const string Sha = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139";
    private const int Cases = 6888;
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode)] private static extern IntPtr GetModuleHandle(string name);
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode)] private static extern uint GetModuleFileName(IntPtr module, System.Text.StringBuilder path, int count);
    [DllImport("kernel32.dll")] private static extern IntPtr VirtualAlloc(IntPtr address, UIntPtr size, uint allocation, uint protection);
    [DllImport("kernel32.dll")] private static extern bool VirtualProtect(IntPtr address, UIntPtr size, uint protection, out uint oldProtection);
    [DllImport("kernel32.dll")] private static extern bool VirtualFree(IntPtr address, UIntPtr size, uint kind);
    [DllImport("kernel32.dll")] private static extern IntPtr GetCurrentProcess();
    [DllImport("kernel32.dll")] private static extern bool FlushInstructionCache(IntPtr process, IntPtr address, UIntPtr size);
    [DllImport("msvcrt.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="_controlfp")] private static extern uint ManagedControl(uint value, uint mask);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint Control(uint value, uint mask);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate void Window(IntPtr position,int end,int unused,IntPtr w,IntPtr q,float factor,IntPtr previousW,IntPtr previousQ,IntPtr source,IntPtr output,int bytes,int precision,int stride,IntPtr reader,IntPtr unused2);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate void Copy(IntPtr position,int end,int unused,IntPtr source,IntPtr output,int bytes,int precision,int stride,IntPtr reader,IntPtr unused2);
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
    private static int[] Factors={0,unchecked((int)0x80000000),0x3f000000,unchecked((int)0xbf000000),0x3f800000,unchecked((int)0xbf800000),0x40000000,unchecked((int)0xc0000000),0x3fffffff,0x3fffff58};
    private static int[][] States={new int[]{0,0x3f800000,0,0x3f800000},new int[]{0x3e800000,0x3f000000,unchecked((int)0xbe000000),0x3f400000},new int[]{0x3f800000,0,0x3f800000,0},new int[]{0x3f7fffff,0x3f800001,unchecked((int)0xbf7fffff),unchecked((int)0xbf800001)}};
    private static int[] Counts={0,1,2,3,31,128,257};
    private static float Float(int bits){return BitConverter.ToSingle(BitConverter.GetBytes(bits),0);}
    private static byte[] Source(int bits,int stride,int count,int pattern)
    {
        int bytes=bits==16?2:3;byte[] data=Bytes((count*stride+9)*bytes);uint random=0x12345678;
        for(int i=0;i<count*stride+9;++i)
        {
            random=unchecked(random*1664525u+1013904223u);int minimum=-(1<<(bits-1)),maximum=(1<<(bits-1))-1;
            int value=pattern==0?0:pattern==1?((i&1)==0?minimum:maximum):pattern==2?(i==3?maximum:0):unchecked((int)(random&((1u<<bits)-1)))-(1<<(bits-1));
            if(bits==20)value<<=4;for(int b=0;b<bytes;++b)data[i*bytes+b]=(byte)(value>>(b*8));
        }
        return data;
    }
    private static void Native(bool window,Window nativeWindow,Copy nativeCopy,byte[] source,int bits,int stride,int count,int factor,int[] state,out byte[] output,out byte[] finalState,out int position,out int sourceOffset,out int outputOffset)
    {
        using(Guarded input=new Guarded(source))using(Guarded destination=new Guarded(Bytes(count*4+24)))using(Guarded slots=new Guarded(Bytes(28)))
        {
            for(int i=0;i<4;++i)Marshal.WriteInt32(At(slots.Pointer,i*4),state[i]);Marshal.WriteInt32(At(slots.Pointer,16),17);
            int initialSource=(bits==16?2:3)*2;Marshal.WriteIntPtr(At(slots.Pointer,20),At(input.Pointer,initialSource));Marshal.WriteIntPtr(At(slots.Pointer,24),At(destination.Pointer,12));
            if(window)nativeWindow(At(slots.Pointer,16),17+count,0,slots.Pointer,At(slots.Pointer,4),Float(factor),At(slots.Pointer,8),At(slots.Pointer,12),At(slots.Pointer,20),At(slots.Pointer,24),bits==16?2:3,bits,stride,IntPtr.Zero,IntPtr.Zero);
            else nativeCopy(At(slots.Pointer,16),17+count,0,At(slots.Pointer,20),At(slots.Pointer,24),bits==16?2:3,bits,stride,IntPtr.Zero,IntPtr.Zero);
            finalState=new byte[16];Marshal.Copy(slots.Pointer,finalState,0,16);position=Marshal.ReadInt32(At(slots.Pointer,16));sourceOffset=unchecked(Marshal.ReadIntPtr(At(slots.Pointer,20)).ToInt32()-input.Pointer.ToInt32());outputOffset=unchecked(Marshal.ReadIntPtr(At(slots.Pointer,24)).ToInt32()-destination.Pointer.ToInt32())/4;
            output=destination.Read();Equal(source,input.Read(),"native source immutable");
        }
    }
    private static void Candidate(bool window,MethodInfo method,byte[] source,int bits,int stride,int count,int factor,int[] state,out byte[] output,out byte[] finalState,out int position,out int sourceOffset,out int outputOffset)
    {
        float[] destination=new float[count+6],values=new float[4];for(int i=0;i<destination.Length;++i)destination[i]=Float(unchecked((int)0xa5a5a5a5));for(int i=0;i<4;++i)values[i]=Float(state[i]);
        object[] args=window?new object[]{source,bits,stride,(bits==16?2:3)*2,destination,3,17,17+count,Float(factor),values}:new object[]{source,bits,stride,(bits==16?2:3)*2,destination,3,17,17+count};
        byte[] before=(byte[])source.Clone();method.Invoke(null,args);Equal(before,source,"managed source immutable");sourceOffset=(int)args[3];outputOffset=(int)args[5];position=(int)args[6];
        output=new byte[destination.Length*4];Buffer.BlockCopy(destination,0,output,0,output.Length);finalState=new byte[16];Buffer.BlockCopy(values,0,finalState,0,16);
    }
    private static void Reject(MethodInfo apply,object[] args,Type expected)
    {
        int source=(int)args[3],output=(int)args[5],position=(int)args[6];float[] state=(float[])args[9];byte[] before=state==null?null:new byte[state.Length*4];if(before!=null)Buffer.BlockCopy(state,0,before,0,before.Length);
        try{apply.Invoke(null,args);}catch(TargetInvocationException e){Require(e.InnerException.GetType()==expected,"Wrong safe rejection");Require((int)args[3]==source&&(int)args[5]==output&&(int)args[6]==position,"Rejection mutated cursor");if(state!=null){byte[] after=new byte[before.Length];Buffer.BlockCopy(state,0,after,0,after.Length);Equal(before,after,"Rejection mutated recurrence");}return;}throw new Exception("Missing safe rejection");
    }
    private static int Main(string[] args)
    {
        IntPtr thunk=IntPtr.Zero;Control control=null;Word word=null;uint oldControl=0,oldWord=0;bool configured=false;
        try
        {
            Require(IntPtr.Size==4,"x86 host required");Require(args.Length==3&&(args[0]=="original"||args[0]=="candidate"),"Usage: original|candidate assembly.dll original.tsv.gz");bool original=args[0]=="original";
            Window[] windows=new Window[3];Copy[] copies=new Copy[3];MethodInfo apply=null,copy=null;
            if(original)
            {
                IntPtr p=Original(args[1]);int[] wrvas={0x98010,0x97da4,0x97edd},crvas={0x980da,0x97e7d,0x97fb3};for(int i=0;i<3;++i){windows[i]=(Window)Marshal.GetDelegateForFunctionPointer(At(p,wrvas[i]),typeof(Window));copies[i]=(Copy)Marshal.GetDelegateForFunctionPointer(At(p,crvas[i]),typeof(Copy));}control=(Control)Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(p,0xab210)),typeof(Control));
            }
            else
            {
                Require(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll")==IntPtr.Zero&&!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.AudioCompiler.dll")),"Original DLL present in recovered host");
                Assembly a=Assembly.LoadFrom(Path.GetFullPath(args[1]));Type t=a.GetType("BinaryAssetBuilder.AudioCompiler.XmaPcmWindow",true);apply=t.GetMethod("Apply");copy=t.GetMethod("Copy");Require(apply!=null&&copy!=null,"Candidate window API missing");control=ManagedControl;
            }
            byte[] code={0x83,0xec,0x04,0xd9,0x3c,0x24,0x0f,0xb7,0x04,0x24,0x83,0xc4,0x04,0xc3};thunk=VirtualAlloc(IntPtr.Zero,new UIntPtr((uint)code.Length),0x3000,4);Require(thunk!=IntPtr.Zero,"CW thunk allocation");Marshal.Copy(code,0,thunk,code.Length);uint oldProtection;
            Require(VirtualProtect(thunk,new UIntPtr((uint)code.Length),0x20,out oldProtection)&&FlushInstructionCache(GetCurrentProcess(),thunk,new UIntPtr((uint)code.Length)),"CW thunk protection");word=(Word)Marshal.GetDelegateForFunctionPointer(thunk,typeof(Word));oldControl=control(0,0);oldWord=word();control(0x10000,0x30300);configured=true;Require(word()==0x027f,"Physical x87 CW must be 027f");
            Console.WriteLine("x87 physicalCW=0x027f originalPhysicalCW=0x"+oldWord.ToString("x4")+" CRTcontrolBefore=0x"+oldControl.ToString("x8"));
            StreamWriter writer=null;StreamReader reader=null;int records=0;
            try
            {
                string header="# originalDllSha256="+Sha+" nativeRvas=0x98010,0x97da4,0x97edd,0x980da,0x97e7d,0x97fb3 physicalCW=0x027f cases="+Cases;
                if(original){writer=new StreamWriter(new GZipStream(File.Create(args[2]),CompressionMode.Compress));writer.WriteLine(header);}else{reader=new StreamReader(new GZipStream(File.OpenRead(args[2]),CompressionMode.Decompress));Require(reader.ReadLine()==header,"Original window header differs");}
                foreach(bool window in new bool[]{false,true})foreach(int bits in new int[]{16,20,24})foreach(int stride in new int[]{1,2})foreach(int pattern in new int[]{0,1,2,3})foreach(int count in Counts)foreach(int stateIndex in window?new int[]{0,1,2,3}:new int[]{0})foreach(int factor in window?Factors:new int[]{0})
                {
                    byte[] source=Source(bits,stride,count,pattern),output,state;int position,sourceOffset,outputOffset;int index=bits==16?0:bits==20?1:2;
                    if(original)Native(window,windows[index],copies[index],source,bits,stride,count,factor,States[stateIndex],out output,out state,out position,out sourceOffset,out outputOffset);else Candidate(window,window?apply:copy,source,bits,stride,count,factor,States[stateIndex],out output,out state,out position,out sourceOffset,out outputOffset);
                    string label=(window?"window":"copy")+":"+bits+":"+stride+":"+pattern+":"+count+":"+stateIndex+":"+factor;
                    if(original)writer.WriteLine(label+"\t"+position+"\t"+sourceOffset+"\t"+outputOffset+"\t"+Convert.ToBase64String(state)+"\t"+Convert.ToBase64String(output));
                    else{string line=reader.ReadLine();Require(line!=null,"Missing original fixture");string[] f=line.Split('\t');Require(f.Length==6&&f[0]==label,"Fixture order differs");Require(position==Int32.Parse(f[1])&&sourceOffset==Int32.Parse(f[2])&&outputOffset==Int32.Parse(f[3]),label+" cursor differs");Equal(Convert.FromBase64String(f[4]),state,label+" all recurrence bits");Equal(Convert.FromBase64String(f[5]),output,label+" output and guards");}
                    Require(position==17+count&&sourceOffset==(bits==16?2:3)*(2+count*stride)&&outputOffset==3+count,"Independent cursor layout");Require(word()==0x027f,"CW changed");++records;
                }
                Require(records==Cases,"Incomplete six-function window matrix");if(!original)Require(reader.ReadLine()==null,"Extra original rows");
            }
            finally{if(writer!=null)writer.Dispose();if(reader!=null)reader.Dispose();}
            if(!original)
            {
                object[][] refusals={new object[]{null,16,1,0,new float[4],0,0,1,1f,new float[4]},new object[]{new byte[8],16,1,0,null,0,0,1,1f,new float[4]},new object[]{new byte[8],32,1,0,new float[4],0,0,1,1f,new float[4]},new object[]{new byte[8],16,0,0,new float[4],0,0,1,1f,new float[4]},new object[]{new byte[8],16,1,-1,new float[4],0,0,1,1f,new float[4]},new object[]{new byte[1],16,1,0,new float[4],0,0,1,1f,new float[4]},new object[]{new byte[8],16,1,0,new float[1],1,0,1,1f,new float[4]},new object[]{new byte[8],16,1,0,new float[4],0,2,1,1f,new float[4]},new object[]{new byte[8],16,1,0,new float[4],0,0,1,1f,null},new object[]{new byte[8],16,1,0,new float[4],0,0,1,1f,new float[3]},new object[]{new byte[8],16,1,0,new float[4],0,0,1,Single.NaN,new float[4]},new object[]{new byte[8],16,1,0,new float[4],0,0,1,1f,new float[]{0,Single.PositiveInfinity,0,0}}};
                for(int i=0;i<refusals.Length;++i)Reject(apply,refusals[i],i<2||i==8?typeof(ArgumentNullException):i==9?typeof(ArgumentException):typeof(ArgumentOutOfRangeException));
                Require(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll")==IntPtr.Zero,"Original DLL loaded after candidate");
            }
            Console.WriteLine("PASS "+args[0]+" XMA integral PCM window/copy leaves: 6888 exact cases (6720 recurrence windows, 168 copies), 16/20/24-bit, all states/cursors/guards; "+(original?"actual original CPU":"12 separate safe refusals; original DLL absent")+"; no outer window scheduler or XMA codec acceptance");return 0;
        }
        catch(Exception e){Console.Error.WriteLine(e);return 1;}
        finally{try{if(configured){control(oldControl,0x30300);Require(word()==oldWord,"Physical x87 CW not restored");}}finally{if(thunk!=IntPtr.Zero)VirtualFree(thunk,UIntPtr.Zero,0x8000);}}
    }
}
