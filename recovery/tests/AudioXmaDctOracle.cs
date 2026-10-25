using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Web.Script.Serialization;

// Actual original CPU oracle for the complete table-backed DCT-IV group. No codec routing.
internal static class AudioXmaDctOracle
{
    private const string Sha = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139";
    private const int Cases = 360;
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode)] private static extern IntPtr GetModuleHandle(string name);
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode)] private static extern uint GetModuleFileName(IntPtr module, System.Text.StringBuilder path, int count);
    [DllImport("kernel32.dll")] private static extern IntPtr VirtualAlloc(IntPtr address, UIntPtr size, uint allocation, uint protection);
    [DllImport("kernel32.dll")] private static extern bool VirtualProtect(IntPtr address, UIntPtr size, uint protection, out uint oldProtection);
    [DllImport("kernel32.dll")] private static extern bool VirtualFree(IntPtr address, UIntPtr size, uint kind);
    [DllImport("kernel32.dll")] private static extern IntPtr GetCurrentProcess();
    [DllImport("kernel32.dll")] private static extern bool FlushInstructionCache(IntPtr process, IntPtr address, UIntPtr size);
    [DllImport("msvcrt.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="_controlfp")] private static extern uint ManagedControl(uint value, uint mask);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint Control(uint value, uint mask);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int Dct(IntPtr data,float scale,IntPtr outputShift,int size,IntPtr fft,int unused1,int unused2,int unused3,int unused4);
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
    private static int[] Scales={0,unchecked((int)0x80000000),0x3f800000,unchecked((int)0xbf800000),0x3f000000,unchecked((int)0xbf000000),0x40000000,0x358637bd,1,0x3f800001};
    private static float Float(int bits){return BitConverter.ToSingle(BitConverter.GetBytes(bits),0);}
    private static float[] Input(int size,int pattern)
    {
        float[] values=new float[size];uint random=0x12345678;
        for(int i=0;i<values.Length;++i){random=unchecked(random*1664525u+1013904223u);switch(pattern){case 0:values[i]=(i&1)==0?0f:Float(unchecked((int)0x80000000));break;case 1:values[i]=i==0?1f:0f;break;case 2:values[i]=(((int)(random&65535))-32768)*(1f/8192f);break;case 3:values[i]=(i&1)==0?32767f:-32768f;break;case 4:values[i]=Float(unchecked((int)((random&0x007fffff)|((random&1)<<31))));break;case 5:values[i]=Float(unchecked((int)((0x3f800000u+(random&3)-1)|((random&1)<<31))));break;}}return values;
    }
    private static void Refuse(MethodInfo transform,object[] args,Type expected)
    {
        float[] input=(float[])args[0];byte[] before=input==null?null:new byte[input.Length*4];if(before!=null)Buffer.BlockCopy(input,0,before,0,before.Length);
        try{transform.Invoke(null,args);}catch(TargetInvocationException e){Require(e.InnerException.GetType()==expected,"Wrong DCT safety rejection");if(before!=null){byte[] after=new byte[before.Length];Buffer.BlockCopy(input,0,after,0,after.Length);Equal(before,after,"DCT refusal changed input");}return;}throw new Exception("Missing DCT rejection");
    }
    private static string Hash(byte[] bytes){using(SHA256 h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
    private static int Rva(object value){string text=value as string;return text!=null&&text.StartsWith("0x",StringComparison.OrdinalIgnoreCase)?Convert.ToInt32(text.Substring(2),16):Convert.ToInt32(value);}
    private static byte[] NativeOutput(Dct native,IntPtr fft,float[] data,int scaleBits,bool absent)
    {
        byte[] bytes=new byte[data.Length*4];Buffer.BlockCopy(data,0,bytes,0,bytes.Length);
        using(Guarded buffer=new Guarded(bytes))using(Guarded shift=new Guarded(Bytes(4)))
        {int hr=native(buffer.Pointer,Float(scaleBits),absent?IntPtr.Zero:shift.Pointer,data.Length,fft,0x12345678,unchecked((int)0x87654321),37,-123);Require(hr==0,"Original real replay HRESULT differs");if(absent)Equal(Bytes(4),shift.Read(),"Absent real output shift mutated guard");else Require(BitConverter.ToInt32(shift.Read(),0)==0,"Original real replay output shift differs");return buffer.Read();}
    }
    private static int Main(string[] args)
    {
        IntPtr thunk=IntPtr.Zero;Control control=null;Word word=null;uint oldControl=0,oldWord=0;bool configured=false;
        try
        {
            Require(IntPtr.Size==4,"x86 host required");Require(args.Length>=3&&args.Length<=5&&(args[0]=="original"||args[0]=="candidate"),"Usage original|candidate assembly.dll original.tsv.gz [snapshots-directory] [--real-only]");bool realOnly=args.Length==5&&args[4]=="--real-only";Require(args.Length!=5||realOnly,"Unknown DCT mode");bool original=args[0]=="original";Dct native=null;IntPtr fft=IntPtr.Zero;MethodInfo transform=null;
            if(original){IntPtr module=Original(args[1]);native=(Dct)Marshal.GetDelegateForFunctionPointer(At(module,0xa81db),typeof(Dct));fft=At(module,0xa7de3);control=(Control)Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module,0xab210)),typeof(Control));}
            else{Require(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll")==IntPtr.Zero&&!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.AudioCompiler.dll")),"Original DLL present in candidate host");transform=Assembly.LoadFrom(Path.GetFullPath(args[1])).GetType("BinaryAssetBuilder.AudioCompiler.XmaDct4",true).GetMethod("Transform");Require(transform!=null,"Candidate DCT absent");control=ManagedControl;}
            byte[] code={0x83,0xec,0x04,0xd9,0x3c,0x24,0x0f,0xb7,0x04,0x24,0x83,0xc4,0x04,0xc3};thunk=VirtualAlloc(IntPtr.Zero,new UIntPtr((uint)code.Length),0x3000,4);Require(thunk!=IntPtr.Zero,"CW allocation");Marshal.Copy(code,0,thunk,code.Length);uint protection;Require(VirtualProtect(thunk,new UIntPtr((uint)code.Length),0x20,out protection)&&FlushInstructionCache(GetCurrentProcess(),thunk,new UIntPtr((uint)code.Length)),"CW protection");word=(Word)Marshal.GetDelegateForFunctionPointer(thunk,typeof(Word));oldControl=control(0,0);oldWord=word();control(0x10000,0x30300);configured=true;Require(word()==0x27f,"Physical CW differs");Console.WriteLine("x87 physicalCW=0x027f originalPhysicalCW=0x"+oldWord.ToString("x4"));
            StreamWriter writer=null;StreamReader reader=null;int cases=0,realCount=0,expectedSynthetic=realOnly?0:Cases;
            try
            {
                string header="# originalDllSha256="+Sha+" nativeRva=0xa81db originalFft=0xa7de3 sizes=64,128,256,512,1024,2048 physicalCW=0x027f cases="+expectedSynthetic;
                if(original){writer=new StreamWriter(new GZipStream(File.Create(args[2]),CompressionMode.Compress));writer.WriteLine(header);}else{reader=new StreamReader(new GZipStream(File.OpenRead(args[2]),CompressionMode.Decompress));Require(reader.ReadLine()==header,"DCT original header mismatch");}
                foreach(int size in realOnly?new int[0]:new int[]{64,128,256,512,1024,2048})foreach(int scaleBits in Scales)for(int pattern=0;pattern<6;++pattern)
                {
                    float[] data=Input(size,pattern);byte[] output=new byte[data.Length*4];Buffer.BlockCopy(data,0,output,0,output.Length);
                    if(original)
                    {
                        using(Guarded buffer=new Guarded(output))using(Guarded shift=new Guarded(Bytes(4)))
                        {bool absent=pattern==0;int hr=native(buffer.Pointer,Float(scaleBits),absent?IntPtr.Zero:shift.Pointer,size,fft,0x12345678,unchecked((int)0x87654321),37,-123);Require(hr==0,"Original HRESULT differs");if(absent)Equal(Bytes(4),shift.Read(),"Null output pointer untouched");else Require(BitConverter.ToInt32(shift.Read(),0)==0,"Original output shift differs");output=buffer.Read();Buffer.BlockCopy(output,0,data,0,output.Length);}
                    }
                    else{object[] call={data,Float(scaleBits),37};Require((int)transform.Invoke(null,call)==0&&(int)call[2]==0,"Candidate HRESULT/output shift differs");Buffer.BlockCopy(data,0,output,0,output.Length);}
                    string label=size+":"+scaleBits+":"+pattern;if(original)writer.WriteLine(label+"\t"+Convert.ToBase64String(output));else{string line=reader.ReadLine();Require(line!=null,"Missing original DCT");string[] f=line.Split('\t');Require(f.Length==2&&f[0]==label,"DCT fixture order differs");Equal(Convert.FromBase64String(f[1]),output,label+" complete DCT bits");}if(pattern==0)foreach(float value in data)Require(value==0f,"Zero DCT independent invariant");Require(word()==0x27f,"DCT changed CW");++cases;
                }
                Require(cases==expectedSynthetic,"Incomplete DCT matrix");
                if(args.Length>=4)
                {
                    string[] paths=Directory.GetFiles(Path.GetFullPath(args[3]),"case.json",SearchOption.AllDirectories);Array.Sort(paths,StringComparer.Ordinal);Require(paths.Length>0,"No real DCT snapshots found");HashSet<string> labels=new HashSet<string>();
                    foreach(string path in paths)
                    {
                        string root=Path.GetDirectoryName(path);Dictionary<string,object> metadata=(Dictionary<string,object>)new JavaScriptSerializer().DeserializeObject(File.ReadAllText(path));int n=Convert.ToInt32(metadata["n"]),scaleBits=Convert.ToInt32(metadata["scaleBits"]);byte[] input=File.ReadAllBytes(Path.Combine(root,(string)metadata["inputPath"])),expected=File.ReadAllBytes(Path.Combine(root,(string)metadata["outputPath"])),nativeArgs=File.ReadAllBytes(Path.Combine(root,(string)metadata["argumentsPath"]));
                        Require(input.Length==n*4&&expected.Length==input.Length&&Hash(input)==(string)metadata["inputSha256"]&&Hash(expected)==(string)metadata["outputSha256"],"Real DCT input/output fingerprint mismatch");
                        Require(nativeArgs.Length==36&&Hash(nativeArgs)==(string)metadata["argumentsSha256"]&&BitConverter.ToInt32(nativeArgs,4)==scaleBits&&BitConverter.ToInt32(nativeArgs,12)==n,"Real DCT native argument fingerprint/shape mismatch");
                        Require(Rva(metadata["targetRva"])==0xa81db&&Rva(metadata["fftCallbackRva"])==0xa7de3&&Convert.ToInt32(metadata["hresult"])==0,"Real DCT used an unrecovered dispatch target");bool absent=BitConverter.ToInt32(nativeArgs,8)==0;Require(absent?(metadata["optionalOutAfter"]==null):(metadata["optionalOutAfter"]!=null&&Convert.ToInt32(metadata["optionalOutAfter"])==0),"Real DCT output shift mismatch");
                        float[] data=new float[n];Buffer.BlockCopy(input,0,data,0,input.Length);byte[] actual;
                        if(original)actual=NativeOutput(native,fft,data,scaleBits,absent);else{object[] call={data,Float(scaleBits),37};Require((int)transform.Invoke(null,call)==0&&(int)call[2]==0,"Candidate real DCT HRESULT/shift differs");actual=new byte[input.Length];Buffer.BlockCopy(data,0,actual,0,actual.Length);}
                        string label="snapshot:"+Path.GetFileName(root);Require(labels.Add(label),"Duplicate DCT snapshot name");Equal(expected,actual,label+" exact original capture output");
                        if(original)writer.WriteLine(label+"\t"+Convert.ToBase64String(actual));else{string line=reader.ReadLine();Require(line!=null,"Missing original DCT replay");string[] fields=line.Split('\t');Require(fields.Length==2&&fields[0]==label,"Real DCT fixture order differs");Equal(Convert.FromBase64String(fields[1]),actual,label+" direct CPU replay output");}Require(word()==0x27f,"DCT real replay changed CW");++realCount;
                    }
                }
                if(!original)Require(reader.ReadLine()==null,"Extra DCT rows");
            }
            finally{if(writer!=null)writer.Dispose();if(reader!=null)reader.Dispose();}
            if(!original)
            {
                Refuse(transform,new object[]{null,1f,37},typeof(ArgumentNullException));Refuse(transform,new object[]{new float[32],1f,37},typeof(ArgumentOutOfRangeException));Refuse(transform,new object[]{new float[4096],1f,37},typeof(ArgumentOutOfRangeException));Refuse(transform,new object[]{new float[96],1f,37},typeof(ArgumentOutOfRangeException));Refuse(transform,new object[]{new float[64],Single.NaN,37},typeof(ArgumentOutOfRangeException));Refuse(transform,new object[]{new float[64],Single.PositiveInfinity,37},typeof(ArgumentOutOfRangeException));float[] invalid=new float[64];invalid[19]=Single.NaN;Refuse(transform,new object[]{invalid,1f,37},typeof(ArgumentOutOfRangeException));invalid[19]=Single.NegativeInfinity;Refuse(transform,new object[]{invalid,1f,37},typeof(ArgumentOutOfRangeException));float[] overflow=new float[64];overflow[0]=Single.MaxValue;Refuse(transform,new object[]{overflow,Single.MaxValue,37},typeof(ArgumentOutOfRangeException));Require(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll")==IntPtr.Zero,"Original loaded after candidate");
            }
            Console.WriteLine("PASS "+args[0]+" table-backed XMA DCT-IV: "+cases+" synthetic output-bit/HRESULT/shift comparisons; realSnapshots="+realCount+"; six sizes64..2048 domain; "+(original?"actual original CPU and original FFT":"nine separate safety refusals; recovered FFT; original DLL absent")+"; no wrapper/quantizer/codec/SDK acceptance");return 0;
        }
        catch(Exception e){Console.Error.WriteLine(e);return 1;}
        finally{try{if(configured){control(oldControl,0x30300);Require(word()==oldWord,"CW not restored");}}finally{if(thunk!=IntPtr.Zero)VirtualFree(thunk,UIntPtr.Zero,0x8000);}}
    }
}
