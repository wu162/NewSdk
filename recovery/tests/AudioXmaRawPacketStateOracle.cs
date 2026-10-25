using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

// Original helper entry/return journals execute native integer code only.
// Return slots point to per-record native continuations, never CLR callbacks.
internal static class AudioXmaRawPacketStateOracle
{
    const string Sha="a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139";
    const BindingFlags PrivateStatic=BindingFlags.NonPublic|BindingFlags.Static;
    [DllImport("kernel32.dll")]static extern bool VirtualProtect(IntPtr p,UIntPtr n,uint protection,out uint old);
    [DllImport("kernel32.dll")]static extern IntPtr VirtualAlloc(IntPtr p,UIntPtr n,uint kind,uint protection);
    [DllImport("kernel32.dll")]static extern bool VirtualFree(IntPtr p,UIntPtr n,uint kind);
    [DllImport("kernel32.dll")]static extern bool FlushInstructionCache(IntPtr process,IntPtr p,UIntPtr n);
    [DllImport("kernel32.dll")]static extern IntPtr GetCurrentProcess();
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode)]static extern IntPtr GetModuleHandle(string name);
    static IntPtr At(IntPtr p,int n){return new IntPtr(unchecked(p.ToInt32()+n));}
    static void Need(bool value,string name){if(!value)throw new Exception(name);}
    static byte[] Read(IntPtr p,int n){byte[] b=new byte[n];if(n!=0)Marshal.Copy(p,b,0,n);return b;}
    static byte[] Fill(int n){byte[] b=new byte[n];for(int i=0;i<n;i++)b[i]=0xa5;return b;}
    static string Hash(byte[] b){using(SHA256 h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(b)).Replace("-","").ToLowerInvariant();}
    static void Equal(string n,byte[] a,byte[] b){Need(a.Length==b.Length,n+" length differs");for(int i=0;i<a.Length;i++)Need(a[i]==b[i],n+" byte differs "+i);}
    static object Field(object o,string n){return o.GetType().GetField(n).GetValue(o);}
    sealed class Patch{public IntPtr Site;public byte[] Original;public IntPtr Stub;public byte[] Code;public int Type;public bool Applied;}
    sealed class Journal:IDisposable
    {
        public const int Capacity=1024,RecordBytes=0x2050,ExitStride=256;
        IntPtr allocation,journal,exitCode;readonly List<Patch> patches=new List<Patch>();readonly int length=16+Capacity*RecordBytes;
        static void A(List<byte> c,params byte[] b){c.AddRange(b);}static void D(List<byte> c,int n){c.AddRange(BitConverter.GetBytes(n));}static void F(List<byte> c,int at,int n){byte[] b=BitConverter.GetBytes(n);for(int i=0;i<4;i++)c[at+i]=b[i];}
        static void Load(List<byte> c,int offset){A(c,0x8b,0x86);D(c,offset);} // eax=[esi+offset]
        static void Store(List<byte> c,int offset){A(c,0x89,0x43,(byte)offset);} // [ebx+offset]=eax
        static void Copy(List<byte> c,int pointerField,int destination){A(c,0x8b,0x73,(byte)pointerField,0x8d,0xbb);D(c,destination);A(c,0xb9);D(c,512);A(c,0xfc,0xf3,0xa5);}
        static void IfError(List<byte> c,List<int> errors,byte condition){A(c,0x0f,condition);errors.Add(c.Count);D(c,0);}
        byte[] Exit(int record)
        {
            List<byte> c=new List<byte>();A(c,0x9c,0x60,0xbb);D(c,At(journal,16+record*RecordBytes).ToInt32());A(c,0x83,0x3b,0x03,0x0f,0x84);int pad=c.Count;D(c,0);
            A(c,0x8b,0x73,0x08,0x8b,0x83);D(c,0x0c);A(c,0x69,0xc0);D(c,0x818);A(c,0x03,0x86);D(c,0x402c);A(c,0x8b,0xe8); // ebp=slot
            A(c,0x8b,0x45,0x0c);Store(c,0x2c);Load(c,0x403c);Store(c,0x30);A(c,0x8b,0x45,0x08);Store(c,0x34);A(c,0x8b,0x45,0x10);Store(c,0x38);
            F(c,pad,c.Count-(pad+4));Copy(c,0x24,0x1050);A(c,0x83,0x7b,0x28,0x00,0x0f,0x84);int one=c.Count;D(c,0);Copy(c,0x28,0x1850);F(c,one,c.Count-(one+4));
            A(c,0xc7,0x43,0x44);D(c,1);A(c,0x61,0x9d,0xff,0x25);D(c,At(journal,16+record*RecordBytes+4).ToInt32());Need(c.Count<=ExitStride,"Native exit stride too small");return c.ToArray();
        }
        byte[] Entry(Patch p)
        {
            List<byte> c=new List<byte>();List<int> errors=new List<int>();A(c,0x9c,0x60,0xb8);D(c,1);A(c,0xf0,0x0f,0xc1,0x05);D(c,journal.ToInt32());A(c,0x3d);D(c,Capacity);IfError(c,errors,0x83);
            A(c,0x8b,0xd0,0x69,0xd2);D(c,ExitStride);A(c,0x81,0xc2);D(c,exitCode.ToInt32());A(c,0x69,0xc0);D(c,RecordBytes);A(c,0x05);D(c,At(journal,16).ToInt32());A(c,0x8b,0xd8,0x89,0x53,0x3c,0xc7,0x03);D(c,p.Type);
            A(c,0x8b,0x44,0x24,0x24);Store(c,4);A(c,0x8b,0x15);D(c,At(journal,8).ToInt32());A(c,0x89,0x53,0x40,0x8d,0x44,0x24,0x24);Store(c,0x48);A(c,0x8b,0x44,0x24,0x20);Store(c,0x4c);
            if(p.Type==3)
            {
                A(c,0x8b,0x44,0x24,0x28);Store(c,0x24);A(c,0x8b,0x44,0x24,0x2c);Store(c,0x1c);A(c,0x3d);D(c,16384);IfError(c,errors,0x87);
            }
            else
            {
                A(c,0x89,0x4b,0x08,0x8b,0x44,0x24,0x28);Store(c,0x0c);A(c,0x3b,0x41,0x04);IfError(c,errors,0x83);A(c,0x69,0xc0);D(c,0x818);A(c,0x03,0x81);D(c,0x402c);A(c,0x8b,0xe8,0x8b,0xf1); // ebp slot,esi state
                A(c,0x8b,0x45,0x0c);Store(c,0x10);Load(c,0x403c);Store(c,0x14);A(c,0x8b,0x45,0x08);Store(c,0x18);A(c,0x8b,0x45,0x10);Store(c,0x1c);Load(c,0x4028);A(c,0x85,0xc0);IfError(c,errors,0x84);A(c,0xa9);D(c,2047);IfError(c,errors,0x85);A(c,0xc1,0xe8,0x0b,0x8b,0xd0,0x3b,0x53,0x10);IfError(c,errors,0x86); // capacity > old
                A(c,0x8b,0x43,0x10,0xc1,0xe0,0x0b,0x03,0x86);D(c,0x4030);Store(c,0x24);
                if(p.Type==2){A(c,0x3b,0x53,0x14);IfError(c,errors,0x86);A(c,0x8b,0x43,0x14,0xc1,0xe0,0x0b,0x03,0x86);D(c,0x4030);Store(c,0x28);A(c,0x8b,0x44,0x24,0x2c);Store(c,0x20);}
            }
            Copy(c,0x24,0x50);A(c,0x83,0x7b,0x28,0x00,0x0f,0x84);int one=c.Count;D(c,0);Copy(c,0x28,0x850);F(c,one,c.Count-(one+4));A(c,0x8b,0x43,0x3c,0x89,0x44,0x24,0x24,0xe9);int doneJump=c.Count;D(c,0);
            int error=c.Count;A(c,0xc7,0x05);D(c,At(journal,4).ToInt32());D(c,1);int done=c.Count;A(c,0x61,0x9d);A(c,p.Original);A(c,0xe9);int resume=c.Count;D(c,0);foreach(int e in errors)F(c,e,error-(e+4));F(c,doneJump,done-(doneJump+4));
            p.Stub=VirtualAlloc(IntPtr.Zero,new UIntPtr((uint)c.Count),0x3000,4);Need(p.Stub!=IntPtr.Zero,"Native entry allocation");F(c,resume,unchecked(At(p.Site,p.Original.Length).ToInt32()-(p.Stub.ToInt32()+resume+4)));return c.ToArray();
        }
        static void Write(Patch p,byte[] bytes){uint old,discard;Need(VirtualProtect(p.Site,new UIntPtr((uint)bytes.Length),0x40,out old),"Packet helper write protection");Marshal.Copy(bytes,0,p.Site,bytes.Length);Need(VirtualProtect(p.Site,new UIntPtr((uint)bytes.Length),old,out discard)&&FlushInstructionCache(GetCurrentProcess(),p.Site,new UIntPtr((uint)bytes.Length)),"Packet helper protection restore");}
        public Journal(IntPtr module)
        {
            try
            {
                allocation=Marshal.AllocHGlobal(length+64);journal=At(allocation,32);Marshal.Copy(Fill(length+64),0,allocation,length+64);Marshal.Copy(new byte[length],0,journal,length);
                exitCode=VirtualAlloc(IntPtr.Zero,new UIntPtr(Capacity*ExitStride),0x3000,4);Need(exitCode!=IntPtr.Zero,"Native exit allocation");
                for(int i=0;i<Capacity;i++){byte[] e=Exit(i);Marshal.Copy(e,0,At(exitCode,i*ExitStride),e.Length);}uint old;Need(VirtualProtect(exitCode,new UIntPtr(Capacity*ExitStride),0x20,out old)&&FlushInstructionCache(GetCurrentProcess(),exitCode,new UIntPtr(Capacity*ExitStride)),"Native exit RX");
                int[] sites={0x5dc70,0x5dcf0,0x5ddb0};byte[][] expected={new byte[]{0x8b,0x44,0x24,0x04,0x8b,0x91,0x2c,0x40,0x00,0x00},new byte[]{0x53,0x55,0x8b,0x6c,0x24,0x0c},new byte[]{0x53,0x8b,0x5c,0x24,0x0c}};
                for(int i=0;i<3;i++){Patch p=new Patch{Site=At(module,sites[i]),Original=Read(At(module,sites[i]),expected[i].Length),Type=i+1};Equal("Original raw packet helper prologue",expected[i],p.Original);patches.Add(p);p.Code=Entry(p);Marshal.Copy(p.Code,0,p.Stub,p.Code.Length);Need(VirtualProtect(p.Stub,new UIntPtr((uint)p.Code.Length),0x20,out old)&&FlushInstructionCache(GetCurrentProcess(),p.Stub,new UIntPtr((uint)p.Code.Length)),"Native entry RX");}
                foreach(Patch p in patches){byte[] b=new byte[p.Original.Length];for(int i=0;i<b.Length;i++)b[i]=0x90;b[0]=0xe9;Buffer.BlockCopy(BitConverter.GetBytes(unchecked(p.Stub.ToInt32()-p.Site.ToInt32()-5)),0,b,1,4);Write(p,b);p.Applied=true;}
            }catch{Dispose();throw;}
        }
        public int Count{get{int n=Marshal.ReadInt32(journal);Need(Marshal.ReadInt32(journal,4)==0&&n>=0&&n<=Capacity,"Native packet journal domain/capacity exceeded");return n;}}
        public void SetCase(int n){Marshal.WriteInt32(journal,8,n);}
        public byte[] Get(int i){Need(i>=0&&i<Count,"Native packet journal index");byte[] b=Read(At(journal,16+i*RecordBytes),RecordBytes);Need(BitConverter.ToInt32(b,0x44)==1,"Native packet helper did not complete paired return");return b;}
        public object Evidence(){List<object> e=new List<object>();foreach(Patch p in patches)e.Add(new{type=p.Type,site="0x"+unchecked((uint)p.Site.ToInt32()).ToString("x8"),original=Convert.ToBase64String(p.Original),nativeEntry=Convert.ToBase64String(p.Code)});return new{mechanism="native entry and per-record return continuation",managedCallbacks=0,usesVeh=false,changesHelperReturnAddressToNativeContinuation=true,capacity=Capacity,recordBytes=RecordBytes,entryStubs=e,exit0=Convert.ToBase64String(Read(exitCode,ExitStride))};}
        public void Dispose(){try{foreach(Patch p in patches)if(p.Applied){Write(p,p.Original);p.Applied=false;Equal("restored packet helper",p.Original,Read(p.Site,p.Original.Length));}if(allocation!=IntPtr.Zero){Equal("journal prefix",Fill(32),Read(allocation,32));Equal("journal suffix",Fill(32),Read(At(journal,length),32));}}finally{foreach(Patch p in patches)if(p.Stub!=IntPtr.Zero){VirtualFree(p.Stub,UIntPtr.Zero,0x8000);p.Stub=IntPtr.Zero;}if(exitCode!=IntPtr.Zero){VirtualFree(exitCode,UIntPtr.Zero,0x8000);exitCode=IntPtr.Zero;}if(allocation!=IntPtr.Zero){Marshal.FreeHGlobal(allocation);allocation=IntPtr.Zero;}}}
    }
    public sealed class SavedFile{public string path;public int bytes;public string sha256;}
    public sealed class SavedCase{public string name;public int channels,frames,sampleRate,blockBytes;public SavedFile[] files;}
    static uint U(byte[] b,int p){return BitConverter.ToUInt32(b,p);}static int I(byte[] b,int p){return BitConverter.ToInt32(b,p);}static byte[] Part(byte[] b,int p,int n){byte[] a=new byte[n];Array.Copy(b,p,a,0,n);return a;}
    static void Original(string dll,string baselineRoot,string outputRoot)
    {
        Directory.CreateDirectory(outputRoot);Type raw=typeof(AudioXmaRawOracle);IntPtr module=(IntPtr)raw.GetMethod("Load",PrivateStatic).Invoke(null,new object[]{dll});Delegate encode=Marshal.GetDelegateForFunctionPointer(At(module,0x589d0),raw.GetNestedType("Encode",BindingFlags.NonPublic)),convert=Marshal.GetDelegateForFunctionPointer(At(module,0xe9b0),raw.GetNestedType("ConvertPcm",BindingFlags.NonPublic)),free=Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module,0xab1a8)),raw.GetNestedType("Free",BindingFlags.NonPublic)),control=Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module,0xab210)),raw.GetNestedType("ControlFp",BindingFlags.NonPublic));uint saved=(uint)control.DynamicInvoke(new object[]{0u,0u});IntPtr readerCode=IntPtr.Zero;
        try
        {
            byte[] code={0x8b,0x44,0x24,0x04,0xd9,0x38,0x0f,0xae,0x58,0x04,0xc3};readerCode=VirtualAlloc(IntPtr.Zero,new UIntPtr((uint)code.Length),0x3000,4);Need(readerCode!=IntPtr.Zero,"FPU reader allocation");Marshal.Copy(code,0,readerCode,code.Length);uint old;Need(VirtualProtect(readerCode,new UIntPtr((uint)code.Length),0x20,out old)&&FlushInstructionCache(GetCurrentProcess(),readerCode,new UIntPtr((uint)code.Length)),"FPU reader RX");Delegate reader=Marshal.GetDelegateForFunctionPointer(readerCode,raw.GetNestedType("ReadFpu",BindingFlags.NonPublic));List<object> cases=new List<object>();List<byte[]> records=new List<byte[]>();object instrumentation=null;
            using(Journal journal=new Journal(module))
            {
                instrumentation=journal.Evidence();int caseIndex=0;
                foreach(string mode in new string[]{"ram","stream"})
                {
                    string name="seeded-wide-stereo-"+mode,baseline=Path.Combine(baselineRoot,"cases",name);SavedCase c=new JavaScriptSerializer().Deserialize<SavedCase>(File.ReadAllText(Path.Combine(baseline,"case.json")));Need(c.name==name&&c.channels==2&&c.frames==65536&&c.sampleRate==22050,"Accepted long corpus metadata differs");Dictionary<string,SavedFile> files=new Dictionary<string,SavedFile>();foreach(SavedFile f in c.files)files.Add(f.path,f);byte[] source=File.ReadAllBytes(Path.Combine(baseline,"source-float32le.bin"));Need(Hash(source)==files["source-float32le.bin"].sha256,"Long corpus input differs");journal.SetCase(caseIndex);int before=journal.Count;
                    object result=raw.GetMethod("Run",PrivateStatic).Invoke(null,new object[]{module,encode,convert,free,control,reader,source,c.channels,c.frames,c.sampleRate,c.blockBytes});byte[][] outputs=(byte[][])Field(result,"Outputs");string[] names={"raw-packets.bin","allocation2.bin","allocation3.bin"};for(int i=0;i<3;i++)Need(outputs[i].Length==files[names[i]].bytes&&Hash(outputs[i])==files[names[i]].sha256,"Native packet interception changed "+names[i]);Need(Hash((byte[])Field(result,"Pcm"))==files["source-signed16le.bin"].sha256&&Hash((byte[])Field(result,"Descriptor"))==files["descriptor-pointer-zero.bin"].sha256,"Native packet interception changed input");int after=journal.Count;Need(after>before,"No real packet lifecycle calls captured");cases.Add(new{name=name,observations=after-before,threeAllocationsEqualAcceptedBaseline=true,inputEqualAcceptedBaseline=true,call=Field(result,"Evidence")});Console.WriteLine("PASS native packet state capture "+name+" observations="+(after-before)+" all3 allocations unchanged");caseIndex++;
                }
                for(int i=0;i<journal.Count;i++)records.Add(journal.Get(i));
            }
            int[] kinds=new int[4];using(FileStream file=File.Create(Path.Combine(outputRoot,"actual-packet-states.tsv.gz")))using(GZipStream gzip=new GZipStream(file,CompressionMode.Compress))using(StreamWriter writer=new StreamWriter(gzip)){writer.WriteLine("# originalDllSha256="+Sha+" realCpu=true actualEncoderStates=true baselineRunId=40233712043 nativeEntryReturnJournal=true");for(int i=0;i<records.Count;i++){int kind=I(records[i],0);Need(kind>=1&&kind<=3,"Native packet helper type");kinds[kind]++;writer.WriteLine(i+"\t"+Convert.ToBase64String(records[i]));}}
            Need(kinds[1]>0&&kinds[2]>0,"Long corpus must capture both header and next-packet transitions");File.WriteAllText(Path.Combine(outputRoot,"packet-state.json"),new JavaScriptSerializer().Serialize(new{status="original-evidence-only",originalDllSha256=Sha,baselineRunId="40233712043",baselineSourceCommit="579c95661d90f067ab414c4133f033c2571b96e1",actualEncoderStates=records.Count,headerStates=kinds[1],nextPacketStates=kinds[2],paddingStates=kinds[3],nativeEncodeCalls=2,entryPointsRestored=true,instrumentation=instrumentation,xmaEncoderRecovered=false,cases=cases}));Console.WriteLine("PASS actual raw packet lifecycle states: "+records.Count+" (header="+kinds[1]+",next="+kinds[2]+",padding="+kinds[3]+");2 long original encodes; all3 allocations unchanged");
        }finally{control.DynamicInvoke(new object[]{saved,0x30300u});if(readerCode!=IntPtr.Zero)VirtualFree(readerCode,UIntPtr.Zero,0x8000);}
    }
    static void Candidate(string dll,string transcript)
    {
        Need(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.AudioCompiler.dll"))&&GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll")==IntPtr.Zero,"Original target present/loaded in candidate");Type type=Assembly.LoadFrom(Path.GetFullPath(dll)).GetType("BinaryAssetBuilder.AudioCompiler.XmaRawPacketWriter",true);MethodInfo header=type.GetMethod("FinalizeHeader"),next=type.GetMethod("StartNextPacket"),pad=type.GetMethod("PadPacket");int count=0;int[] kinds=new int[4];
        using(FileStream file=File.OpenRead(transcript))using(GZipStream gzip=new GZipStream(file,CompressionMode.Decompress))using(StreamReader reader=new StreamReader(gzip))
        {
            string line=reader.ReadLine();Need(line!=null&&line.Contains("originalDllSha256="+Sha)&&line.Contains("actualEncoderStates=true")&&line.Contains("baselineRunId=40233712043"),"Actual packet state provenance missing");while((line=reader.ReadLine())!=null){string[] f=line.Split('\t');Need(f.Length==2&&Int32.Parse(f[0])==count,"Actual packet record index");byte[] b=Convert.FromBase64String(f[1]);Need(b.Length==Journal.RecordBytes&&I(b,0x44)==1,"Actual packet record incomplete");int kind=I(b,0),old=I(b,0x10),n=I(b,0x14),bits=I(b,0x1c);Need(kind>=1&&kind<=3,"Actual helper type");byte[] packets;
                if(kind==3){packets=Part(b,0x50,2048);pad.Invoke(null,new object[]{packets,0,bits});Equal("actual pad packet",Part(b,0x1050,2048),packets);}
                else{Need(old>=0&&old<8192&&(kind!=2||(n>=0&&n<8192)),"Actual packet index outside bounded replay");packets=new byte[(Math.Max(old,kind==2?n:old)+1)*2048];Array.Copy(b,0x50,packets,old*2048,2048);if(kind==2)Array.Copy(b,0x850,packets,n*2048,2048);
                    if(kind==1){header.Invoke(null,new object[]{packets,old,U(b,0x18)});Need(I(b,0x2c)==old&&I(b,0x30)==n&&U(b,0x34)==U(b,0x18)&&I(b,0x38)==bits,"Original header changed unexpected scalar fields");}
                    else{object[] a={packets,old,n,U(b,0x20),0u,0};next.Invoke(null,a);Need((int)a[1]==I(b,0x2c)&&(int)a[2]==I(b,0x30)&&(uint)a[4]==U(b,0x34)&&(int)a[5]==I(b,0x38),"Actual next-packet scalar result differs");Equal("actual new packet",Part(b,0x1850,2048),Part(packets,n*2048,2048));}
                    Equal("actual current/previous packet",Part(b,0x1050,2048),Part(packets,old*2048,2048));
                }kinds[kind]++;count++;}
        }
        Need(kinds[1]>0&&kinds[2]>0,"Actual packet coverage incomplete");Need(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll")==IntPtr.Zero,"Candidate loaded original target");Console.WriteLine("PASS candidate actual raw packet states: "+count+" exact observations (header="+kinds[1]+",next="+kinds[2]+",padding="+kinds[3]+"); raw packets/scalars match original entry-return snapshots; no bitsplice-state or full encoder acceptance");
    }
    static int Main(string[] args){try{Need(args.Length>=3,"Usage: original original.dll corpus output | candidate recovered.dll transcript");if(args[0]=="original"){Need(args.Length==4,"Original args");Original(args[1],args[2],args[3]);}else if(args[0]=="candidate"){Need(args.Length==3,"Candidate args");Candidate(args[1],args[2]);}else throw new Exception("Unknown mode");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
