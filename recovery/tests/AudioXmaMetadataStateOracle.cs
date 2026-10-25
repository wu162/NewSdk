using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

// Actual metadata-state capture at 5f170 through a native-only entry trampoline.
// No CLR callback or floating point operation occurs inside the original codec.
internal static class AudioXmaMetadataStateOracle
{
    const string Sha = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139";
    const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;
    [DllImport("kernel32.dll")] static extern bool VirtualProtect(IntPtr p, UIntPtr n, uint protection, out uint previous);
    [DllImport("kernel32.dll")] static extern IntPtr VirtualAlloc(IntPtr p, UIntPtr n, uint allocation, uint protection);
    [DllImport("kernel32.dll")] static extern bool VirtualFree(IntPtr p, UIntPtr n, uint kind);
    [DllImport("kernel32.dll")] static extern IntPtr GetCurrentProcess();
    [DllImport("kernel32.dll")] static extern bool FlushInstructionCache(IntPtr process, IntPtr p, UIntPtr n);
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode)] static extern IntPtr GetModuleHandle(string name);
    static IntPtr At(IntPtr p, int n) { return new IntPtr(unchecked(p.ToInt32() + n)); }
    static void Need(bool b, string text) { if (!b) throw new Exception(text); }
    static byte[] Read(IntPtr p, int n) { byte[] b = new byte[n]; if (n != 0) Marshal.Copy(p, b, 0, n); return b; }
    static string Hash(byte[] b) { using (SHA256 hash = SHA256.Create()) return BitConverter.ToString(hash.ComputeHash(b)).Replace("-", "").ToLowerInvariant(); }
    static void Equal(string name, byte[] a, byte[] b) { Need(a.Length == b.Length, name + " length differs"); for (int i = 0; i < a.Length; ++i) Need(a[i] == b[i], name + " byte differs at " + i); }
    static byte[] Sentinel(int n) { byte[] b = new byte[n]; for (int i = 0; i < n; ++i) b[i] = 0xa5; return b; }
    static object Field(object instance, string name) { return instance.GetType().GetField(name).GetValue(instance); }
    static object Save(string directory, string name, byte[] bytes) { File.WriteAllBytes(Path.Combine(directory, name), bytes); return new { path = name, length = bytes.Length, sha256 = Hash(bytes) }; }
    sealed class Capture
    {
        public int Case, StateAddress, Esp, Flags; public byte[] State; public byte[][] Formats; public uint[] Counts;
        public uint U32(int offset) { return BitConverter.ToUInt32(State, offset); }
    }
    sealed class Journal : IDisposable
    {
        const int Capacity = 16, RecordBytes = 0x6460;
        readonly IntPtr site, allocation, journal, stub;
        readonly byte[] original, bytes;
        readonly int length = 16 + Capacity * RecordBytes;
        bool patched;
        static void Add(List<byte> c, params byte[] b) { c.AddRange(b); }
        static void Dword(List<byte> c, int value) { c.AddRange(BitConverter.GetBytes(value)); }
        static void Fix(List<byte> c, int where, int value) { byte[] b = BitConverter.GetBytes(value); for (int i=0;i<4;++i)c[where+i]=b[i]; }
        public Journal(IntPtr module)
        {
            site=At(module,0x5f170);original=Read(site,6);Equal("metadata entry prologue",new byte[]{0x81,0xec,0x30,0x04,0x00,0x00},original);
            allocation=Marshal.AllocHGlobal(length+64);journal=At(allocation,32);Marshal.Copy(Sentinel(length+64),0,allocation,length+64);Marshal.Copy(new byte[length],0,journal,length);
            List<byte> c=new List<byte>();List<int> errors=new List<int>();
            Add(c,0x9c,0x60,0x8b,0xe9); // pushfd;pushad;mov ebp,ecx(original input state)
            Add(c,0xb8);Dword(c,1);Add(c,0xf0,0x0f,0xc1,0x05);Dword(c,journal.ToInt32()); // lock xadd count,eax
            Add(c,0x3d);Dword(c,Capacity);Add(c,0x0f,0x83);errors.Add(c.Count);Dword(c,0);
            Add(c,0x69,0xc0);Dword(c,RecordBytes);Add(c,0x05);Dword(c,At(journal,16).ToInt32());Add(c,0x8b,0xd8); // ebx=record
            Add(c,0x8b,0x15);Dword(c,At(journal,8).ToInt32());Add(c,0x89,0x10,0x89,0x68,0x04); // case/state
            Add(c,0x8d,0x54,0x24,0x24,0x89,0x50,0x08,0x8b,0x54,0x24,0x20,0x89,0x50,0x0c); // original esp/eflags
            Add(c,0x8d,0x78,0x10,0x8b,0xf5,0xb9);Dword(c,0x1014);Add(c,0xfc,0xf3,0xa5); // all 0x4050 bytes
            Add(c,0x8b,0x55,0x04,0x85,0xd2,0x0f,0x84);errors.Add(c.Count);Dword(c,0);
            Add(c,0x81,0xfa);Dword(c,256);Add(c,0x0f,0x87);errors.Add(c.Count);Dword(c,0);
            Add(c,0x83,0xc5,0x10); // ebp=first source-format pointer slot; edi=record+4060
            int loop=c.Count;Add(c,0x8b,0x75,0x00,0xb9);Dword(c,8);Add(c,0xf3,0xa5,0x83,0xc5,0x38,0x4a,0x0f,0x85);int again=c.Count;Dword(c,loop-(again+4));
            Add(c,0x8b,0x6b,0x04,0x8b,0x8d);Dword(c,0x4038);Add(c,0x81,0xf9);Dword(c,256);Add(c,0x0f,0x87);errors.Add(c.Count);Dword(c,0);
            Add(c,0x8b,0xb5);Dword(c,0x4040);Add(c,0x8d,0xbb);Dword(c,0x6060);Add(c,0xf3,0xa5); // count array
            Add(c,0xe9);int doneJump=c.Count;Dword(c,0);
            int error=c.Count;Add(c,0xc7,0x05);Dword(c,At(journal,4).ToInt32());Dword(c,1);
            int done=c.Count;Add(c,0x61,0x9d);Add(c,original);Add(c,0xe9);int returnJump=c.Count;Dword(c,0);
            foreach(int p in errors)Fix(c,p,error-(p+4));Fix(c,doneJump,done-(doneJump+4));
            stub=VirtualAlloc(IntPtr.Zero,new UIntPtr((uint)c.Count),0x3000,4);Need(stub!=IntPtr.Zero,"Native metadata capture stub allocation");Fix(c,returnJump,unchecked(At(site,6).ToInt32()-(stub.ToInt32()+returnJump+4)));bytes=c.ToArray();Marshal.Copy(bytes,0,stub,bytes.Length);
            uint old;Need(VirtualProtect(stub,new UIntPtr((uint)bytes.Length),0x20,out old)&&FlushInstructionCache(GetCurrentProcess(),stub,new UIntPtr((uint)bytes.Length)),"Native metadata stub RX");
            byte[] patch=new byte[6];patch[0]=0xe9;Buffer.BlockCopy(BitConverter.GetBytes(unchecked(stub.ToInt32()-site.ToInt32()-5)),0,patch,1,4);patch[5]=0x90;Write(patch);patched=true;
        }
        void Write(byte[] b) { uint old, discarded;Need(VirtualProtect(site,new UIntPtr(6),0x40,out old),"Metadata entry write protection");Marshal.Copy(b,0,site,6);Need(VirtualProtect(site,new UIntPtr(6),old,out discarded)&&FlushInstructionCache(GetCurrentProcess(),site,new UIntPtr(6)),"Metadata entry restore protection"); }
        public void SetCase(int n) { Marshal.WriteInt32(journal,8,n); }
        public int Count { get { int n=Marshal.ReadInt32(journal);Need(Marshal.ReadInt32(journal,4)==0&&n>=0&&n<=Capacity,"Native capture domain/capacity exceeded");return n; } }
        public Capture Get(int n)
        {
            Need(n>=0&&n<Count,"Metadata capture index");IntPtr p=At(journal,16+n*RecordBytes);byte[] state=Read(At(p,16),0x4050);int streams=BitConverter.ToInt32(state,4),blocks=BitConverter.ToInt32(state,0x4038);
            Need(streams>=1&&streams<=256&&blocks>=0&&blocks<=256,"Captured metadata bounds");byte[][] formats=new byte[streams][];for(int i=0;i<streams;++i)formats[i]=Read(At(p,0x4060+i*32),32);
            byte[] counts=Read(At(p,0x6060),blocks*4);uint[] values=new uint[blocks];for(int i=0;i<blocks;++i)values[i]=BitConverter.ToUInt32(counts,4*i);
            return new Capture { Case=Marshal.ReadInt32(p),StateAddress=Marshal.ReadInt32(p,4),Esp=Marshal.ReadInt32(p,8),Flags=Marshal.ReadInt32(p,12),State=state,Formats=formats,Counts=values };
        }
        public object Evidence() { return new { entryRva="0x5f170",stolenBytes=Convert.ToBase64String(original),stubAddress="0x"+unchecked((uint)stub.ToInt32()).ToString("x8"),stubBase64=Convert.ToBase64String(bytes),capacity=Capacity,recordBytes=RecordBytes,managedCallbacks=0,usesVeh=false,preservesIntegerRegistersFlagsAndFpu=true }; }
        public void Dispose() { try { if(patched){Write(original);patched=false;} Equal("restored original metadata entry",original,Read(site,6)); Equal("capture prefix guard",Sentinel(32),Read(allocation,32));Equal("capture suffix guard",Sentinel(32),Read(At(journal,length),32)); }finally{if(stub!=IntPtr.Zero)VirtualFree(stub,UIntPtr.Zero,0x8000);Marshal.FreeHGlobal(allocation);} }
    }
    public sealed class SavedFile { public string path; public int length; public string sha256; }
    public sealed class SavedCase { public string name;public int channels,frames,sampleRate,blockBytes;public bool streamed;public SavedFile[] files; }
    sealed class CaseResult { public string Name,Directory;public Capture Capture;public byte[][] Outputs;public object Call; }
    static void Original(string dll,string baselineRoot,string outputRoot)
    {
        Directory.CreateDirectory(outputRoot);Type raw=typeof(AudioXmaRawOracle);IntPtr module=(IntPtr)raw.GetMethod("Load",PrivateStatic).Invoke(null,new object[]{dll});
        Delegate encode=Marshal.GetDelegateForFunctionPointer(At(module,0x589d0),raw.GetNestedType("Encode",BindingFlags.NonPublic)),convert=Marshal.GetDelegateForFunctionPointer(At(module,0xe9b0),raw.GetNestedType("ConvertPcm",BindingFlags.NonPublic)),free=Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module,0xab1a8)),raw.GetNestedType("Free",BindingFlags.NonPublic)),control=Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module,0xab210)),raw.GetNestedType("ControlFp",BindingFlags.NonPublic));
        uint saved=(uint)control.DynamicInvoke(new object[]{0u,0u});IntPtr readerCode=IntPtr.Zero;List<CaseResult> captured=new List<CaseResult>();object instrumentation=null;
        try
        {
            byte[] code=new byte[]{0x8b,0x44,0x24,0x04,0xd9,0x38,0x0f,0xae,0x58,0x04,0xc3};readerCode=VirtualAlloc(IntPtr.Zero,new UIntPtr((uint)code.Length),0x3000,4);Need(readerCode!=IntPtr.Zero,"FPU reader allocation");Marshal.Copy(code,0,readerCode,code.Length);uint old;Need(VirtualProtect(readerCode,new UIntPtr((uint)code.Length),0x20,out old)&&FlushInstructionCache(GetCurrentProcess(),readerCode,new UIntPtr((uint)code.Length)),"FPU reader RX");Delegate reader=Marshal.GetDelegateForFunctionPointer(readerCode,raw.GetNestedType("ReadFpu",BindingFlags.NonPublic));
            using(Journal journal=new Journal(module))
            {
                instrumentation=journal.Evidence();int index=0;
                foreach(int channels in new int[]{1,2})foreach(int frames in new int[]{257,4096})foreach(bool streamed in new bool[]{false,true})
                {
                    string name="xma-"+channels+"-"+frames+"-"+(streamed?"stream":"ram"),baseline=Path.Combine(baselineRoot,"cases",name),directory=Path.Combine(outputRoot,"cases",name);Directory.CreateDirectory(directory);
                    SavedCase original=new JavaScriptSerializer().Deserialize<SavedCase>(File.ReadAllText(Path.Combine(baseline,"case.json")));Need(original.name==name&&original.channels==channels&&original.frames==frames&&original.sampleRate==22050&&original.streamed==streamed,"Accepted raw case metadata differs");Dictionary<string,SavedFile> files=new Dictionary<string,SavedFile>();foreach(SavedFile f in original.files)files.Add(f.path,f);
                    byte[] source=File.ReadAllBytes(Path.Combine(baseline,"source-float32le.bin"));Need(Hash(source)==files["source-float32le.bin"].sha256,"Accepted raw input fingerprint");journal.SetCase(index);int before=journal.Count;
                    object result=raw.GetMethod("Run",PrivateStatic).Invoke(null,new object[]{module,encode,convert,free,control,reader,source,channels,frames,original.sampleRate,original.blockBytes});Need(journal.Count==before+1,"Expected exactly one EA-format builder entry per original encode");Capture capture=journal.Get(before);Need(capture.Case==index&&capture.Formats.Length==1&&capture.U32(0x4028)==(uint)original.blockBytes,"Actual metadata state differs from caller");
                    byte[][] outputs=(byte[][])Field(result,"Outputs");string[] names=new string[]{"raw-packets.bin","allocation2.bin","allocation3.bin"};for(int i=0;i<3;++i)Need(outputs[i].Length==files[names[i]].length&&Hash(outputs[i])==files[names[i]].sha256,"Native capture changed original "+names[i]);Need(Hash((byte[])Field(result,"Pcm"))==files["source-signed16le.bin"].sha256&&Hash((byte[])Field(result,"Descriptor"))==files["descriptor-pointer-zero.bin"].sha256,"Native capture changed original inputs");
                    captured.Add(new CaseResult{Name=name,Directory=directory,Capture=capture,Outputs=outputs,Call=Field(result,"Evidence")});++index;Console.WriteLine("PASS captured actual metadata state "+name+" three original allocations unchanged");
                }
                Need(journal.Count==8,"Metadata capture count differs");
            }
            // The original entry is restored before direct metadata replays;
            // those replays must not be mistaken for fresh wrapper captures.
            Type metadata=typeof(AudioXmaCodecMetadataOracle),fixtureType=metadata.GetNestedType("Fixture",BindingFlags.NonPublic);Delegate metadataFree=Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module,0xab1a8)),metadata.GetNestedType("NativeFree",BindingFlags.NonPublic));List<object> cases=new List<object>();
            string transcript=Path.Combine(outputRoot,"actual-states.tsv.gz");using(FileStream file=File.Create(transcript))using(GZipStream gzip=new GZipStream(file,CompressionMode.Compress))using(StreamWriter writer=new StreamWriter(gzip))
            {
                writer.WriteLine("# originalDllSha256="+Sha+" actualWrapperStates=8 nativeEntryRva=0x5f170 originalOutputsEqualAcceptedBaseline=true");
                foreach(CaseResult result in captured)
                {
                    Capture c=result.Capture;object fixture=Activator.CreateInstance(fixtureType,true);foreach(KeyValuePair<string,object> field in new Dictionary<string,object>{{"Name",result.Name},{"Formats",c.Formats},{"Counts",c.Counts},{"BlockBytes",c.U32(0x4028)},{"PlayLength",c.U32(0x4024)},{"LoopBegin",c.U32(0x401c)},{"LoopEnd",c.U32(0x4020)}})fixtureType.GetField(field.Key).SetValue(fixture,field.Value);
                    byte[][] replay=(byte[][])metadata.GetMethod("RunNative",PrivateStatic).Invoke(null,new object[]{fixture,module,metadataFree});Equal(result.Name+" captured-state native EA to actual raw allocation2",result.Outputs[1],replay[0]);Equal(result.Name+" captured-state native seek to actual raw allocation3",result.Outputs[2],replay[2]);
                    byte[] formats=new byte[c.Formats.Length*32],counts=new byte[c.Counts.Length*4];for(int i=0;i<c.Formats.Length;++i)Array.Copy(c.Formats[i],0,formats,i*32,32);for(int i=0;i<c.Counts.Length;++i)Array.Copy(BitConverter.GetBytes(c.Counts[i]),0,counts,i*4,4);
                    object[] files=new object[]{Save(result.Directory,"actual-state.bin",c.State),Save(result.Directory,"stream-formats.bin",formats),Save(result.Directory,"block-counts.bin",counts),Save(result.Directory,"ea-format.bin",replay[0]),Save(result.Directory,"wave-format.bin",replay[1]),Save(result.Directory,"seek-table.bin",replay[2])};cases.Add(new{name=result.Name,streamCount=c.Formats.Length,blockCount=c.Counts.Length,blockBytes=c.U32(0x4028),playLength=c.U32(0x4024),loopBegin=c.U32(0x401c),loopEnd=c.U32(0x4020),actualStateCapturedBeforeMetadataBuilder=true,originalOutputsEqualAcceptedBaseline=true,eaSeekEqualActualCodecAllocations=true,files=files,encode=result.Call});
                    writer.WriteLine(result.Name+"\t"+c.U32(0x4028)+"\t"+c.U32(0x4024)+"\t"+c.U32(0x401c)+"\t"+c.U32(0x4020)+"\t"+Convert.ToBase64String(formats)+"\t"+Convert.ToBase64String(counts)+"\t"+Convert.ToBase64String(replay[0])+"\t"+Convert.ToBase64String(replay[1])+"\t"+Convert.ToBase64String(replay[2]));
                }
            }
            File.WriteAllText(Path.Combine(outputRoot,"actual-states.json"),new JavaScriptSerializer().Serialize(new{status="original-evidence-only",originalDllSha256=Sha,actualWrapperStates=8,nativeEncodeCalls=8,baselineSourceCommit="cd66d9a2c821bc41f1f975aac8ac0a5bac6a6499",baselineRunId="40232525056",instrumentation=instrumentation,entryRestoredBeforeDirectReplays=true,codecEncodingRecovered=false,cases=cases}),new UTF8Encoding(false));Console.WriteLine("PASS actual original metadata states: 8 captures, 8 original encodes, three allocations unchanged; captured-state EA/seek exactly equal real codec allocations; original entry restored before replay");
        }
        finally{control.DynamicInvoke(new object[]{saved,0x30300u});if(readerCode!=IntPtr.Zero)VirtualFree(readerCode,UIntPtr.Zero,0x8000);}
    }
    static void Candidate(string dll,string transcript)
    {
        Need(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.AudioCompiler.dll"))&&GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll")==IntPtr.Zero,"Original target present or loaded in candidate host");Type type=Assembly.LoadFrom(Path.GetFullPath(dll)).GetType("BinaryAssetBuilder.AudioCompiler.XmaCodecMetadata",true);MethodInfo ea=type.GetMethod("BuildEaFormat"),wave=type.GetMethod("BuildWaveFormat"),seek=type.GetMethod("BuildSeekTable");int count=0;HashSet<string> names=new HashSet<string>();
        using(FileStream file=File.OpenRead(transcript))using(GZipStream gzip=new GZipStream(file,CompressionMode.Decompress))using(StreamReader reader=new StreamReader(gzip))
        {
            string line=reader.ReadLine();Need(line!=null&&line.Contains("originalDllSha256="+Sha)&&line.Contains("actualWrapperStates=8")&&line.Contains("originalOutputsEqualAcceptedBaseline=true"),"Actual state provenance missing");
            while((line=reader.ReadLine())!=null)
            {
                string[] f=line.Split('\t');Need(f.Length==10&&names.Add(f[0]),"Malformed actual state fixture");byte[] formats=Convert.FromBase64String(f[5]),counts=Convert.FromBase64String(f[6]);Need(formats.Length%32==0&&counts.Length%4==0,"Actual state record boundaries");byte[][] formatArray=new byte[formats.Length/32][];for(int i=0;i<formatArray.Length;++i){formatArray[i]=new byte[32];Array.Copy(formats,i*32,formatArray[i],0,32);}uint[] values=new uint[counts.Length/4];for(int i=0;i<values.Length;++i)values[i]=BitConverter.ToUInt32(counts,i*4);
                object[] args=new object[]{formatArray,values,UInt32.Parse(f[1]),UInt32.Parse(f[2]),UInt32.Parse(f[3]),UInt32.Parse(f[4])};Equal(f[0]+" actual EA allocation2",Convert.FromBase64String(f[7]),(byte[])ea.Invoke(null,args));Equal(f[0]+" actual state WAVE52",Convert.FromBase64String(f[8]),(byte[])wave.Invoke(null,args));Equal(f[0]+" actual seek allocation3",Convert.FromBase64String(f[9]),(byte[])seek.Invoke(null,new object[]{values}));for(int i=0;i<formatArray.Length;++i)for(int j=0;j<32;++j)Need(formatArray[i][j]==formats[i*32+j],"Candidate changed native stream format");for(int i=0;i<values.Length;++i)Need(values[i]==BitConverter.ToUInt32(counts,i*4),"Candidate changed native block counts");++count;
            }
        }
        Need(count==8,"Actual metadata state matrix incomplete");foreach(int channels in new int[]{1,2})foreach(int frames in new int[]{257,4096})foreach(string streamed in new string[]{"ram","stream"})Need(names.Contains("xma-"+channels+"-"+frames+"-"+streamed),"Actual metadata case missing");Need(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll")==IntPtr.Zero,"Candidate loaded original codec");Console.WriteLine("PASS candidate actual metadata states: 8 captured wrapper states; EA44 and BEseek4 exactly equal original real codec allocations, WAVE52 equal original captured-state replay; source inputs immutable; no encoding support");
    }
    static int Main(string[] args)
    {
        try{Need(args.Length>=3,"Usage: original original.dll baseline output | candidate recovered.dll transcript");if(args[0]=="original"){Need(args.Length==4,"Original arguments");Original(args[1],args[2],args[3]);}else if(args[0]=="candidate"){Need(args.Length==3,"Candidate arguments");Candidate(args[1],args[2]);}else throw new Exception("Unknown mode");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}
