using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Web.Script.Serialization;

// Actual CPU oracle for six integral PCM window/copy leaves. No codec routing.
internal static class AudioXmaScheduleOracle
{
    private const string Sha = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139";
    private const int Cases = 3240;
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode)] private static extern IntPtr GetModuleHandle(string name);
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode)] private static extern uint GetModuleFileName(IntPtr module, System.Text.StringBuilder path, int count);
    [DllImport("kernel32.dll")] private static extern IntPtr VirtualAlloc(IntPtr address, UIntPtr size, uint allocation, uint protection);
    [DllImport("kernel32.dll")] private static extern bool VirtualProtect(IntPtr address, UIntPtr size, uint protection, out uint oldProtection);
    [DllImport("kernel32.dll")] private static extern bool VirtualFree(IntPtr address, UIntPtr size, uint kind);
    [DllImport("kernel32.dll")] private static extern IntPtr GetCurrentProcess();
    [DllImport("kernel32.dll")] private static extern bool FlushInstructionCache(IntPtr process, IntPtr address, UIntPtr size);
    [DllImport("msvcrt.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="_controlfp")] private static extern uint ManagedControl(uint value, uint mask);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint Control(uint value, uint mask);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate void Schedule(IntPtr core,IntPtr newer,IntPtr older,IntPtr channel,int fullHalf,int windowHalf,IntPtr table,int index,int unused,int full,int width,int window,int bytes,int precision,int stride,int leading,int riseEnd,int copyEnd,int fallEnd,float r0,float r1,float r2,float r3,float rf,float f0,float f1,float f2,float f3,float ff,int representation,IntPtr unused2,IntPtr reader);
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
    private static string Hash(byte[] bytes){using(SHA256 h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
    private static int[] Integers(object value){object[] a=(object[])value;int[] b=new int[a.Length];for(int i=0;i<a.Length;++i)b[i]=Convert.ToInt32(a[i]);return b;}
    private static int[][] Coefficients={new int[]{0,0x3f800000,0,0x3f800000,0x3f800000},new int[]{0x3e800000,0x3f000000,unchecked((int)0xbe000000),0x3f400000,0x3f000000},new int[]{0x3f7fffff,0x3f800001,unchecked((int)0xbf7fffff),unchecked((int)0xbf800001),0x3fffffff}};
    private static float[] Floats(int[] bits){float[] a=new float[bits.Length];for(int i=0;i<a.Length;++i)a[i]=BitConverter.ToSingle(BitConverter.GetBytes(bits[i]),0);return a;}
    private static byte[] Source(int bits,int stride,int full,int window,int pattern,bool older)
    {
        int bytes=bits==16?2:3;byte[] data=new byte[(full*3+window*2+16)*stride*bytes];uint random=older?0x12345678u:0x87654321u;
        for(int i=0;i<data.Length/bytes;++i){random=unchecked(random*1664525u+1013904223u);int value=pattern==0?(older?13579:-24680):pattern==1?((i&1)==0?-(1<<(bits-1)):(1<<(bits-1))-1):unchecked((int)(random&((1u<<bits)-1)))-(1<<(bits-1));if(bits==20)value<<=4;for(int b=0;b<bytes;++b)data[i*bytes+b]=(byte)(value>>(b*8));}return data;
    }
    private static byte[] OriginalCase(IntPtr module,Schedule schedule,byte[] older,byte[] newer,int bits,int stride,short block,int full,int window,int[] boundaries,float[] rising,float[] falling)
    {
        int width=window*2;
        using(Guarded a=new Guarded(older))using(Guarded b=new Guarded(newer))using(Guarded core=new Guarded(Bytes(0x18e20)))using(Guarded table=new Guarded(new byte[2]))using(Guarded channel=new Guarded(Bytes(0x4c)))using(Guarded output=new Guarded(Bytes(width*4+24)))
        {
            Marshal.WriteIntPtr(At(core.Pointer,0x18e1c),At(module,0x98010));Marshal.WriteInt16(table.Pointer,block);Marshal.WriteIntPtr(At(channel.Pointer,0x48),At(output.Pointer,12));byte[] coreBefore=core.Read(),channelBefore=channel.Read();
            schedule(core.Pointer,b.Pointer,a.Pointer,channel.Pointer,full/2,window/2,table.Pointer,0,0,full,width,window,bits==16?2:3,bits,stride,boundaries[0],boundaries[1],boundaries[2],boundaries[3],rising[0],rising[1],rising[2],rising[3],rising[4],falling[0],falling[1],falling[2],falling[3],falling[4],bits==16?61:bits==20?78:94,IntPtr.Zero,IntPtr.Zero);
            Equal(older,a.Read(),"older input immutable");Equal(newer,b.Read(),"newer input immutable");Equal(coreBefore,core.Read(),"core immutable");Equal(channelBefore,channel.Read(),"channel immutable");Require(Marshal.ReadInt16(table.Pointer)==block,"table immutable");return output.Read();
        }
    }
    private static byte[] CandidateCase(MethodInfo process,byte[] older,byte[] newer,int bits,int stride,short block,int full,int window,int[] boundaries,float[] rising,float[] falling)
    {
        byte[] a=(byte[])older.Clone(),b=(byte[])newer.Clone();int[] shape=(int[])boundaries.Clone();float[] r=(float[])rising.Clone(),f=(float[])falling.Clone();
        float[] result=(float[])process.Invoke(null,new object[]{older,newer,bits,stride,block,full,window,boundaries,rising,falling});Require(result.Length==window*2,"Output width differs");Equal(a,older,"managed older immutable");Equal(b,newer,"managed newer immutable");
        for(int i=0;i<4;++i)Require(shape[i]==boundaries[i],"geometry mutated");byte[] rb=new byte[20],ra=new byte[20];Buffer.BlockCopy(r,0,rb,0,20);Buffer.BlockCopy(rising,0,ra,0,20);Equal(rb,ra,"rising immutable");Buffer.BlockCopy(f,0,rb,0,20);Buffer.BlockCopy(falling,0,ra,0,20);Equal(rb,ra,"falling immutable");
        byte[] output=Bytes(result.Length*4+24);Buffer.BlockCopy(result,0,output,12,result.Length*4);return output;
    }
    private static void Refusals(MethodInfo process)
    {
        for(int scenario=0;scenario<15;++scenario)
        {
            object[] a={new byte[4096],new byte[4096],16,1,(short)0,128,128,new int[]{0,128,128,256},Floats(Coefficients[0]),Floats(Coefficients[1])};Type expected=typeof(ArgumentOutOfRangeException);
            switch(scenario)
            {
                case 0:a[0]=null;expected=typeof(ArgumentNullException);break;
                case 1:a[1]=null;expected=typeof(ArgumentNullException);break;
                case 2:a[2]=8;break;case 3:a[3]=0;break;case 4:a[5]=127;break;case 5:a[6]=129;break;
                case 6:a[7]=null;expected=typeof(ArgumentNullException);break;
                case 7:a[7]=new int[3];expected=typeof(ArgumentException);break;
                case 8:a[7]=new int[]{-1,128,128,256};break;
                case 9:a[8]=null;expected=typeof(ArgumentNullException);break;
                case 10:a[8]=new float[4];expected=typeof(ArgumentException);break;
                case 11:a[8]=new float[]{0,1,0,1,Single.NaN};break;
                case 12:a[9]=new float[]{0,Single.PositiveInfinity,0,1,1};break;
                case 13:a[4]=Int16.MinValue;break;case 14:a[0]=new byte[0];break;
            }
            byte[][] before=new byte[a.Length][];for(int i=0;i<a.Length;++i)if(a[i] is Array){Array array=(Array)a[i];before[i]=new byte[Buffer.ByteLength(array)];Buffer.BlockCopy(array,0,before[i],0,before[i].Length);}
            bool rejected=false;try{process.Invoke(null,a);}catch(TargetInvocationException error){Require(error.InnerException.GetType()==expected,"Wrong scheduler refusal "+scenario);rejected=true;}Require(rejected,"Missing scheduler refusal "+scenario);
            for(int i=0;i<a.Length;++i)if(before[i]!=null){byte[] after=new byte[before[i].Length];Buffer.BlockCopy((Array)a[i],0,after,0,after.Length);Equal(before[i],after,"Scheduler refusal mutated input "+scenario);}
        }
    }
    private static int Main(string[] args)
    {
        IntPtr thunk=IntPtr.Zero;Control control=null;Word word=null;uint oldControl=0,oldWord=0;bool configured=false;
        try
        {
            Require(IntPtr.Size==4,"x86 host required");Require((args.Length==3||args.Length==4)&&(args[0]=="original"||args[0]=="candidate"),"Usage original|candidate assembly.dll original.tsv.gz [snapshots-directory]");bool original=args[0]=="original";IntPtr module=IntPtr.Zero;Schedule schedule=null;MethodInfo process=null;
            if(original){module=Original(args[1]);schedule=(Schedule)Marshal.GetDelegateForFunctionPointer(At(module,0x98267),typeof(Schedule));control=(Control)Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module,0xab210)),typeof(Control));}
            else{Require(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll")==IntPtr.Zero&&!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.AudioCompiler.dll")),"Original present in candidate host");process=Assembly.LoadFrom(Path.GetFullPath(args[1])).GetType("BinaryAssetBuilder.AudioCompiler.XmaPcmWindowScheduler",true).GetMethod("Process");Require(process!=null,"Candidate scheduler absent");control=ManagedControl;}
            byte[] code={0x83,0xec,0x04,0xd9,0x3c,0x24,0x0f,0xb7,0x04,0x24,0x83,0xc4,0x04,0xc3};thunk=VirtualAlloc(IntPtr.Zero,new UIntPtr((uint)code.Length),0x3000,4);Require(thunk!=IntPtr.Zero,"CW allocation");Marshal.Copy(code,0,thunk,code.Length);uint protection;Require(VirtualProtect(thunk,new UIntPtr((uint)code.Length),0x20,out protection)&&FlushInstructionCache(GetCurrentProcess(),thunk,new UIntPtr((uint)code.Length)),"CW protection");word=(Word)Marshal.GetDelegateForFunctionPointer(thunk,typeof(Word));oldControl=control(0,0);oldWord=word();control(0x10000,0x30300);configured=true;Require(word()==0x27f,"Physical CW differs");
            Console.WriteLine("x87 physicalCW=0x027f originalPhysicalCW=0x"+oldWord.ToString("x4"));StreamWriter writer=null;StreamReader reader=null;int count=0,realCount=0;
            try
            {
                string header="# originalDllSha256="+Sha+" nativeRva=0x98267 representations=61,78,94 physicalCW=0x027f cases="+Cases;if(original){writer=new StreamWriter(new GZipStream(File.Create(args[2]),CompressionMode.Compress));writer.WriteLine(header);}else{reader=new StreamReader(new GZipStream(File.OpenRead(args[2]),CompressionMode.Decompress));Require(reader.ReadLine()==header,"Evidence header differs");}
                foreach(int bits in new int[]{16,20,24})foreach(int stride in new int[]{1,2})foreach(int config in new int[]{0,1,2})
                {
                    int full=config==0?128:256,window=config==2?128:full,half=window/2;
                    short[] offsets={0,(short)(half-full/2),(short)(full/2+half),(short)(full/2+half+7),(short)(full/2+half-7)};
                    int[][] shapes={new int[]{0,window,window,2*window},new int[]{half/2,half,3*half,2*window-half/2},new int[]{half,half,3*half,3*half},new int[]{0,half,3*half,2*window}};
                    for(int at=0;at<offsets.Length;++at)for(int shape=0;shape<shapes.Length;++shape)for(int pattern=0;pattern<3;++pattern)for(int coefficients=0;coefficients<Coefficients.Length;++coefficients)
                    {
                        byte[] older=Source(bits,stride,full,window,pattern,true),newer=Source(bits,stride,full,window,pattern,false);float[] rising=Floats(Coefficients[coefficients]),falling=Floats(Coefficients[(coefficients+1)%3]);
                        byte[] result=original?OriginalCase(module,schedule,older,newer,bits,stride,offsets[at],full,window,shapes[shape],rising,falling):CandidateCase(process,older,newer,bits,stride,offsets[at],full,window,shapes[shape],rising,falling);
                        string label=bits+":"+stride+":"+config+":"+at+":"+shape+":"+pattern+":"+coefficients;if(original)writer.WriteLine(label+"\t"+Convert.ToBase64String(result));else{string line=reader.ReadLine();Require(line!=null,"Missing fixture");string[] f=line.Split('\t');Require(f.Length==2&&f[0]==label,"Fixture order differs");Equal(Convert.FromBase64String(f[1]),result,label+" scheduler exact output/padding");}Require(word()==0x27f,"CW changed");++count;
                    }
                }
                Require(count==Cases,"Incomplete scheduler matrix");
                if(args.Length==4)
                {
                    string[] snapshots=Directory.GetFiles(Path.GetFullPath(args[3]),"case.json",SearchOption.AllDirectories);Array.Sort(snapshots,StringComparer.Ordinal);Require(snapshots.Length>0,"No real window snapshots found");HashSet<string> names=new HashSet<string>();
                    foreach(string path in snapshots)
                    {
                        string root=Path.GetDirectoryName(path);Dictionary<string,object> metadata=(Dictionary<string,object>)new JavaScriptSerializer().DeserializeObject(File.ReadAllText(path));byte[] older=File.ReadAllBytes(Path.Combine(root,"older.bin")),newer=File.ReadAllBytes(Path.Combine(root,"newer.bin"));
                        Require(Hash(older)==(string)metadata["olderSha256"]&&Hash(newer)==(string)metadata["newerSha256"],"Real source snapshot fingerprint differs");
                        int bits=Convert.ToInt32(metadata["bits"]),stride=Convert.ToInt32(metadata["stride"]),full=Convert.ToInt32(metadata["fullFrames"]),window=Convert.ToInt32(metadata["windowFrames"]);short block=Convert.ToInt16(metadata["blockOffset"]);
                        int[] boundaries=Integers(metadata["boundaries"]);float[] rising=Floats(Integers(metadata["risingBits"])),falling=Floats(Integers(metadata["fallingBits"]));
                        Require(boundaries.Length==4&&rising.Length==5&&falling.Length==5,"Malformed real geometry");
                        byte[] nativeArguments=File.ReadAllBytes(Path.Combine(root,"arguments.bin"));Require(nativeArguments.Length==128&&Hash(nativeArguments)==(string)metadata["argumentsSha256"],"Real native argument snapshot fingerprint differs");
                        Require(BitConverter.ToInt32(nativeArguments,4*4)==full/2&&BitConverter.ToInt32(nativeArguments,5*4)==window/2&&BitConverter.ToInt32(nativeArguments,9*4)==full&&BitConverter.ToInt32(nativeArguments,10*4)==window*2&&BitConverter.ToInt32(nativeArguments,11*4)==window&&BitConverter.ToInt32(nativeArguments,12*4)==(bits==16?2:3)&&BitConverter.ToInt32(nativeArguments,13*4)==bits&&BitConverter.ToInt32(nativeArguments,14*4)==stride,"Normalized real geometry differs from native slots");
                        Require(BitConverter.ToInt32(nativeArguments,29*4)==(bits==16?61:bits==20?78:94)&&Convert.ToInt32(metadata["representation"])==BitConverter.ToInt32(nativeArguments,29*4),"Real representation is not the recovered integral path");
                        int[] rbits=Integers(metadata["risingBits"]),fbits=Integers(metadata["fallingBits"]);for(int j=0;j<4;++j)Require(boundaries[j]==BitConverter.ToInt32(nativeArguments,(15+j)*4),"Real boundary slot differs");for(int j=0;j<5;++j)Require(rbits[j]==BitConverter.ToInt32(nativeArguments,(19+j)*4)&&fbits[j]==BitConverter.ToInt32(nativeArguments,(24+j)*4),"Real coefficient bits differ from native slots");
                        byte[] result=original?OriginalCase(module,schedule,older,newer,bits,stride,block,full,window,boundaries,rising,falling):CandidateCase(process,older,newer,bits,stride,block,full,window,boundaries,rising,falling);
                        string label="snapshot:"+Path.GetFileName(root);Require(names.Add(label),"Duplicate snapshot directory name");if(original)writer.WriteLine(label+"\t"+Convert.ToBase64String(result));else{string line=reader.ReadLine();Require(line!=null,"Missing original real snapshot result");string[] fields=line.Split('\t');Require(fields.Length==2&&fields[0]==label,"Real fixture ordering differs");Equal(Convert.FromBase64String(fields[1]),result,label+" complete output/padding");}Require(word()==0x27f,"Real replay changed CW");++realCount;
                    }
                }
                if(!original)Require(reader.ReadLine()==null,"Extra fixtures");
            }
            finally{if(writer!=null)writer.Dispose();if(reader!=null)reader.Dispose();}
            if(!original){Refusals(process);Require(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll")==IntPtr.Zero,"Original DLL loaded after candidate");}
            Console.WriteLine("PASS "+args[0]+" integral XMA window scheduler: 3240 complete output/padding cases; realSnapshots="+realCount+"; three representations, old/new blocks and skip/overlap phases; "+(original?"actual original CPU":"15 separate safety refusals; original DLL absent")+"; no transform or XMA encoder acceptance");return 0;
        }
        catch(Exception e){Console.Error.WriteLine(e);return 1;}
        finally{try{if(configured){control(oldControl,0x30300);Require(word()==oldWord,"CW not restored");}}finally{if(thunk!=IntPtr.Zero)VirtualFree(thunk,UIntPtr.Zero,0x8000);}}
    }
}
