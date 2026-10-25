using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Web.Script.Serialization;

// Actual original CPU oracle for the complete bounded signal-transform group. No codec routing.
internal static class AudioXmaSignalOracle
{
    private const string Sha = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139";
    private const int Cases = 5760;
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode)] private static extern IntPtr GetModuleHandle(string name);
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode)] private static extern uint GetModuleFileName(IntPtr module, System.Text.StringBuilder path, int count);
    [DllImport("kernel32.dll")] private static extern IntPtr VirtualAlloc(IntPtr address, UIntPtr size, uint allocation, uint protection);
    [DllImport("kernel32.dll")] private static extern bool VirtualProtect(IntPtr address, UIntPtr size, uint protection, out uint oldProtection);
    [DllImport("kernel32.dll")] private static extern bool VirtualFree(IntPtr address, UIntPtr size, uint kind);
    [DllImport("kernel32.dll")] private static extern IntPtr GetCurrentProcess();
    [DllImport("kernel32.dll")] private static extern bool FlushInstructionCache(IntPtr process, IntPtr address, UIntPtr size);
    [DllImport("msvcrt.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="_controlfp")] private static extern uint ManagedControl(uint value, uint mask);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint Control(uint value, uint mask);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int Signal(IntPtr core,IntPtr channel,IntPtr outputShift);
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
    {
        float[] values=new float[n*2];uint random=0x12345678;
        for(int i=0;i<values.Length;++i){random=unchecked(random*1664525u+1013904223u);values[i]=pattern==0?((i&1)==0?0f:Float(unchecked((int)0x80000000))):pattern==1?(i==n/3?32767f:0f):(((int)(random&65535))-32768);}return values;
    }
    private static float[] Curve(int mode)
    {if(mode==0)return null;float[] curve=new float[256];for(int i=0;i<256;++i)curve[i]=mode==1?1f:mode==2?i*(1f/256f):(i%7==0?Float(unchecked((int)0x80000000)):(i&1)==0?0.5f:-0.5f);return curve;}
    private static float Fraction(int size,int kind)
    {float half=0.5f/size;int bits=BitConverter.ToInt32(BitConverter.GetBytes(half),0);return kind==0?0f:kind==1?0.5f:kind==2?0.91f:kind==3?1f:kind==4?1.1f:kind==5?half:kind==6?1.5f/size:kind==7?(size-0.5f)/size:kind==8?Float(bits-1):Float(bits+1);}
    private static byte[] Native(Signal signal,IntPtr module,float[] windowed,int leading,int legacy,int version,bool reduce,float fraction,float[] curve,bool absent=false,int? rawTailFlag=null,int? rawClipGainBits=null)
    {
        int n=windowed.Length/2;byte[] input=new byte[windowed.Length*4];Buffer.BlockCopy(windowed,0,input,0,input.Length);byte[] weights=new byte[1024];if(curve!=null)Buffer.BlockCopy(curve,0,weights,0,weights.Length);
        using(Guarded core=new Guarded(new byte[0x18e34]))using(Guarded backend=new Guarded(new byte[0x208]))using(Guarded channel=new Guarded(new byte[0x54]))using(Guarded parameters=new Guarded(new byte[0x98]))using(Guarded source=new Guarded(input))using(Guarded output=new Guarded(Bytes(n*4+24)))using(Guarded table=new Guarded(weights))using(Guarded shift=new Guarded(Bytes(4)))
        {
            Marshal.WriteIntPtr(core.Pointer,backend.Pointer);Marshal.WriteInt32(At(core.Pointer,0x75d0),rawTailFlag.HasValue?rawTailFlag.Value:(reduce?1:0));Marshal.WriteInt32(At(core.Pointer,0x18c64),rawClipGainBits.HasValue?rawClipGainBits.Value:(reduce?0:0x3f800000));Marshal.WriteInt32(At(core.Pointer,0x75d4),BitConverter.ToInt32(BitConverter.GetBytes(fraction),0));Marshal.WriteInt32(At(core.Pointer,0x18e24),curve==null?0:1);Marshal.WriteIntPtr(At(core.Pointer,0x18e30),table.Pointer);
            Marshal.WriteInt32(At(backend.Pointer,0x10c),leading);Marshal.WriteInt32(At(backend.Pointer,0x110),legacy);Marshal.WriteInt32(At(backend.Pointer,0x3c),version);Marshal.WriteIntPtr(At(backend.Pointer,0x1f0),At(module,0xa81db));Marshal.WriteIntPtr(At(backend.Pointer,0x204),At(module,0xa7de3));
            Marshal.WriteIntPtr(channel.Pointer,parameters.Pointer);Marshal.WriteIntPtr(At(channel.Pointer,0x48),source.Pointer);Marshal.WriteIntPtr(At(channel.Pointer,0x50),At(output.Pointer,12));Marshal.WriteInt16(At(parameters.Pointer,0x7c),(short)n);Marshal.WriteInt16(At(parameters.Pointer,0x7e),(short)n);
            byte[] cb=core.Read(),bb=backend.Read(),pb=parameters.Read(),chb=channel.Read();int hr=signal(core.Pointer,channel.Pointer,absent?IntPtr.Zero:shift.Pointer);Require(hr==0,"Original signal HRESULT");if(absent)Equal(Bytes(4),shift.Read(),"Absent shift guard");else Require(BitConverter.ToInt32(shift.Read(),0)==0,"Original signal output shift");
            Equal(input,source.Read(),"original window immutable");Equal(weights,table.Read(),"original curve immutable");Equal(cb,core.Read(),"original core immutable");Equal(bb,backend.Read(),"original backend immutable");Equal(pb,parameters.Read(),"original parameters immutable");Equal(chb,channel.Read(),"original channel immutable");return output.Read();
        }
    }
    private static byte[] Candidate(MethodInfo process,float[] windowed,int leading,int legacy,int version,bool reduce,float fraction,float[] curve)
    {
        byte[] before=new byte[windowed.Length*4];Buffer.BlockCopy(windowed,0,before,0,before.Length);byte[] cb=curve==null?null:new byte[1024];if(cb!=null)Buffer.BlockCopy(curve,0,cb,0,1024);
        object[] args={windowed,leading,legacy,version,reduce,fraction,curve,null,37};Require((int)process.Invoke(null,args)==0&&(int)args[8]==0,"Candidate signal HRESULT/shift");float[] values=(float[])args[7];Require(values.Length==windowed.Length/2,"Candidate spectrum width");byte[] output=Bytes(values.Length*4+24);Buffer.BlockCopy(values,0,output,12,values.Length*4);byte[] after=new byte[before.Length];Buffer.BlockCopy(windowed,0,after,0,after.Length);Equal(before,after,"managed window immutable");if(cb!=null){after=new byte[1024];Buffer.BlockCopy(curve,0,after,0,1024);Equal(cb,after,"managed curve immutable");}return output;
    }
    private static void Refuse(MethodInfo process,object[] args,Type expected)
    {float[] input=(float[])args[0];byte[] before=input==null?null:new byte[input.Length*4];if(before!=null)Buffer.BlockCopy(input,0,before,0,before.Length);try{process.Invoke(null,args);}catch(TargetInvocationException e){Require(e.InnerException.GetType()==expected,"Wrong signal refusal");if(before!=null){byte[] after=new byte[before.Length];Buffer.BlockCopy(input,0,after,0,after.Length);Equal(before,after,"Signal refusal mutated input");}return;}throw new Exception("Missing signal refusal");}
    private static string Hash(byte[] bytes){using(SHA256 h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
    private static int Rva(object value){string text=value as string;return text!=null&&text.StartsWith("0x",StringComparison.OrdinalIgnoreCase)?Convert.ToInt32(text.Substring(2),16):Convert.ToInt32(value);}
    private static int Main(string[] args)
    {
        IntPtr thunk=IntPtr.Zero;Control control=null;Word word=null;uint oldControl=0,oldWord=0;bool configured=false;
        try
        {
            Require(IntPtr.Size==4,"x86 host required");Require(args.Length>=3&&args.Length<=5&&(args[0]=="original"||args[0]=="candidate"),"Usage original|candidate assembly.dll original.tsv.gz [snapshots-directory] [--real-only]");bool realOnly=args.Length==5&&args[4]=="--real-only";Require(args.Length!=5||realOnly,"Unknown signal mode");bool original=args[0]=="original";Signal native=null;IntPtr module=IntPtr.Zero;MethodInfo process=null;
            if(original){module=Original(args[1]);native=(Signal)Marshal.GetDelegateForFunctionPointer(At(module,0x97b00),typeof(Signal));control=(Control)Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module,0xab210)),typeof(Control));}
            else{Require(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll")==IntPtr.Zero&&!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.AudioCompiler.dll")),"Original DLL present in candidate host");process=Assembly.LoadFrom(Path.GetFullPath(args[1])).GetType("BinaryAssetBuilder.AudioCompiler.XmaSignalTransform",true).GetMethod("Process");Require(process!=null,"Candidate signal transform absent");control=ManagedControl;}
            byte[] code={0x83,0xec,0x04,0xd9,0x3c,0x24,0x0f,0xb7,0x04,0x24,0x83,0xc4,0x04,0xc3};thunk=VirtualAlloc(IntPtr.Zero,new UIntPtr((uint)code.Length),0x3000,4);Require(thunk!=IntPtr.Zero,"CW allocation");Marshal.Copy(code,0,thunk,code.Length);uint protection;Require(VirtualProtect(thunk,new UIntPtr((uint)code.Length),0x20,out protection)&&FlushInstructionCache(GetCurrentProcess(),thunk,new UIntPtr((uint)code.Length)),"CW protection");word=(Word)Marshal.GetDelegateForFunctionPointer(thunk,typeof(Word));oldControl=control(0,0);oldWord=word();control(0x10000,0x30300);configured=true;Require(word()==0x27f,"Physical CW differs");Console.WriteLine("x87 physicalCW=0x027f originalPhysicalCW=0x"+oldWord.ToString("x4"));
            StreamWriter writer=null;StreamReader reader=null;int cases=0,realCount=0,expectedSynthetic=realOnly?0:Cases;
            try
            {
                string header="# originalDllSha256="+Sha+" nativeRva=0x97b00 dctRva=0xa81db fftRva=0xa7de3 sizes=64,128,256,512,1024,2048 physicalCW=0x027f cases="+expectedSynthetic;
                if(original){writer=new StreamWriter(new GZipStream(File.Create(args[2]),CompressionMode.Compress));writer.WriteLine(header);}else{reader=new StreamReader(new GZipStream(File.OpenRead(args[2]),CompressionMode.Decompress));Require(reader.ReadLine()==header,"Signal original header mismatch");}
                foreach(int n in realOnly?new int[0]:new int[]{64,128,256,512,1024,2048})foreach(int version in new int[]{-1,2,3,99})for(int ratio=0;ratio<10;++ratio)foreach(bool reduce in new bool[]{false,true})for(int mode=0;mode<4;++mode)for(int pattern=0;pattern<3;++pattern)
                {
                    int leading=new int[]{0,1,n/4,n}[(version+ratio+(reduce?1:0)+mode+4)%4],legacy=version==99?-1:new int[]{0,n/2,n}[(ratio+mode+(version<0?0:version))%3];float fraction=Fraction(n,ratio);float[] input=Input(n,pattern),curve=Curve(mode);
                    byte[] output=original?Native(native,module,input,leading,legacy,version,reduce,fraction,curve):Candidate(process,input,leading,legacy,version,reduce,fraction,curve);
                    string label=n+":"+version+":"+ratio+":"+reduce+":"+mode+":"+pattern;
                    if(original)writer.WriteLine(label+"\t"+Convert.ToBase64String(output));else{string line=reader.ReadLine();Require(line!=null,"Missing original signal");string[] fields=line.Split('\t');Require(fields.Length==2&&fields[0]==label,"Signal fixture ordering differs");Equal(Convert.FromBase64String(fields[1]),output,label+" complete spectrum bits/guards");}Require(word()==0x27f,"Signal transform changed CW");++cases;
                }
                Require(cases==expectedSynthetic,"Incomplete signal matrix");
                if(args.Length>=4)
                {
                    string[] paths=Directory.GetFiles(Path.GetFullPath(args[3]),"case.json",SearchOption.AllDirectories);Array.Sort(paths,StringComparer.Ordinal);Require(paths.Length>0,"No real signal snapshots found");HashSet<string> labels=new HashSet<string>();
                    foreach(string path in paths)
                    {
                        string root=Path.GetDirectoryName(path);Dictionary<string,object> m=(Dictionary<string,object>)new JavaScriptSerializer().DeserializeObject(File.ReadAllText(path));
                        int n=Convert.ToInt32(m["n"]),leading=Convert.ToInt32(m["leadingZeroCount"]),legacy=Convert.ToInt32(m["legacyUpperLimit"]),version=Convert.ToInt32(m["codecVersion"]),tailFlag=Convert.ToInt32(m["tailFlag"]),gainBits=Convert.ToInt32(m["clipGainBits"]),fractionBits=Convert.ToInt32(m["retainedFractionBits"]);bool enabled=Convert.ToBoolean(m["weightsEnabled"]),reduce=tailFlag!=0&&Float(gainBits)<1f;
                        Require(n>=64&&n<=2048&&(n&(n-1))==0&&leading>=0&&leading<=n&&(version>2||legacy>=0),"Unsupported real signal geometry/masks");
                        byte[] input=File.ReadAllBytes(Path.Combine(root,(string)m["inputPath"])),expected=File.ReadAllBytes(Path.Combine(root,(string)m["outputPath"])),nativeArgs=File.ReadAllBytes(Path.Combine(root,(string)m["argumentsPath"]));
                        Require(input.Length==n*8&&expected.Length==n*4&&Hash(input)==(string)m["inputSha256"]&&Hash(expected)==(string)m["outputSha256"],"Real signal input/output fingerprint mismatch");Require(nativeArgs.Length==12&&Hash(nativeArgs)==(string)m["argumentsSha256"],"Real signal argument fingerprint mismatch");
                        byte[] rawFields=File.ReadAllBytes(Path.Combine(root,(string)m["rawFieldsPath"]));Require(rawFields.Length==96&&Hash(rawFields)==(string)m["rawFieldsSha256"],"Real signal raw-fields fingerprint mismatch");int[] f=new int[24];Buffer.BlockCopy(rawFields,0,f,0,96);
                        Require(f[0]==BitConverter.ToInt32(nativeArgs,0)&&f[1]==BitConverter.ToInt32(nativeArgs,4)&&f[5]==BitConverter.ToInt32(nativeArgs,8)&&f[10]==n&&f[11]==n&&f[12]==version&&f[13]==leading&&f[14]==legacy&&f[17]==tailFlag&&f[18]==gainBits&&f[19]==fractionBits&&(f[20]!=0)==enabled&&f[7]==0&&f[8]==0,"Real signal raw options disagree with normalized JSON");Require(unchecked(f[15]-0xa81db)==unchecked(f[16]-0xa7de3),"Real signal callback image bases disagree");
                        Require(Rva(m["targetRva"])==0x97b00&&Rva(m["dctCallbackRva"])==0xa81db&&Rva(m["fftCallbackRva"])==0xa7de3&&Convert.ToInt32(m["hresult"])==0,"Real signal used unrecovered target");bool absent=BitConverter.ToInt32(nativeArgs,8)==0;Require(absent||f[6]==0,"Real signal raw shift mismatch");Require(absent?(m["outputShiftAfter"]==null):(m["outputShiftAfter"]!=null&&Convert.ToInt32(m["outputShiftAfter"])==0),"Real signal captured shift mismatch");
                        float[] windowed=new float[2*n],curve=null;Buffer.BlockCopy(input,0,windowed,0,input.Length);
                        if(enabled){byte[] weightBytes=File.ReadAllBytes(Path.Combine(root,(string)m["weightsPath"]));Require(weightBytes.Length==1024&&Hash(weightBytes)==(string)m["weightsSha256"],"Real signal curve fingerprint mismatch");curve=new float[256];Buffer.BlockCopy(weightBytes,0,curve,0,1024);}else Require(m["weightsPath"]==null&&m["weightsSha256"]==null,"Disabled curve has unexpected file");
                        byte[] full=original?Native(native,module,windowed,leading,legacy,version,reduce,Float(fractionBits),curve,absent,tailFlag,gainBits):Candidate(process,windowed,leading,legacy,version,reduce,Float(fractionBits),curve);byte[] actual=new byte[n*4];Array.Copy(full,12,actual,0,actual.Length);string label="snapshot:"+Path.GetFileName(root);Require(labels.Add(label),"Duplicate real signal name");Equal(expected,actual,label+" exact original capture output");
                        if(original)writer.WriteLine(label+"\t"+Convert.ToBase64String(actual));else{string line=reader.ReadLine();Require(line!=null,"Missing original signal replay");string[] fields=line.Split('\t');Require(fields.Length==2&&fields[0]==label,"Real signal fixture ordering differs");Equal(Convert.FromBase64String(fields[1]),actual,label+" direct CPU replay output");}Require(word()==0x27f,"Signal real replay changed CW");++realCount;
                    }
                }
                if(!original)Require(reader.ReadLine()==null,"Extra original signal records");
            }
            finally{if(writer!=null)writer.Dispose();if(reader!=null)reader.Dispose();}
            if(!original)
            {
                object[][] bad={new object[]{null,0,64,3,false,1f,null,null,37},new object[]{new float[127],0,64,3,false,1f,null,null,37},new object[]{new float[128],-1,64,3,false,1f,null,null,37},new object[]{new float[128],65,64,3,false,1f,null,null,37},new object[]{new float[128],0,-1,2,false,1f,null,null,37},new object[]{new float[128],0,64,3,false,-1f,null,null,37},new object[]{new float[128],0,64,3,false,Single.NaN,null,null,37},new object[]{new float[128],0,64,3,false,Single.MaxValue,null,null,37},new object[]{new float[128],0,64,3,false,1f,new float[255],null,37},new object[]{new float[128],0,64,3,false,1f,new float[256],null,37},new object[]{new float[128],0,64,3,false,1f,null,null,37},new object[]{new float[128],0,64,3,false,1f,null,null,37},new object[]{new float[128],0,64,3,false,1f,null,null,37}};
                ((float[])bad[9][6])[17]=Single.PositiveInfinity;((float[])bad[10][0])[5]=Single.NaN;((float[])bad[11][0])[5]=Single.NegativeInfinity;for(int j=0;j<128;++j)((float[])bad[12][0])[j]=Single.MaxValue;for(int i=0;i<bad.Length;++i)Refuse(process,bad[i],i==0?typeof(ArgumentNullException):i==8?typeof(ArgumentException):typeof(ArgumentOutOfRangeException));Require(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll")==IntPtr.Zero,"Original loaded after candidate");
            }
            Console.WriteLine("PASS "+args[0]+" XMA signal transform: "+cases+" synthetic complete spectrum/HRESULT/shift comparisons; realSnapshots="+realCount+"; six sizes, four codec-version values, ten ratios around half ties, two tail policies, four gain-curve modes, three input patterns; "+(original?"actual original CPU/DCT/FFT":"thirteen separate safety refusals; recovered DCT/FFT; original DLL absent")+"; no quantization/entropy/XMA-route/SDK acceptance");return 0;
        }
        catch(Exception e){Console.Error.WriteLine(e);return 1;}
        finally{try{if(configured){control(oldControl,0x30300);Require(word()==oldWord,"CW not restored");}}finally{if(thunk!=IntPtr.Zero)VirtualFree(thunk,UIntPtr.Zero,0x8000);}}
    }
}
