using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

internal static class AudioXmaRawPacketOracle
{
    private const string Sha = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139";
    private const int TotalCases = 1106;
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr GetModuleHandle(string name);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] private static extern uint GetModuleFileName(IntPtr module, StringBuilder name, int capacity);
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)] private delegate void NativeHeader(IntPtr self, int stream);
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)] private delegate void NativeNext(IntPtr self, int stream, uint offset);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int NativeCopy(IntPtr source, IntPtr destination, int sourceBit, int destinationBit, int count);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate void NativeBit(IntPtr data, int offset);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate void NativePad(IntPtr data, int count);
    private static IntPtr At(IntPtr value, int offset) { return new IntPtr(unchecked(value.ToInt32() + offset)); }
    private static void Require(bool condition, string label) { if (!condition) throw new Exception(label); }
    private static byte[] Sentinel(int size) { byte[] result = new byte[size]; for (int i = 0; i < size; ++i) result[i] = 0xa5; return result; }
    private static void Equal(string label, byte[] expected, byte[] actual)
    {
        Require(expected.Length == actual.Length, label + " length differs");
        for (int i = 0; i < actual.Length; ++i) if (actual[i] != expected[i]) throw new Exception(label + " byte=" + i + " expected=" + expected[i] + " actual=" + actual[i]);
    }
    private sealed class Guarded : IDisposable
    {
        private readonly IntPtr allocation;
        private readonly int size;
        public readonly IntPtr Pointer;
        public Guarded(byte[] data)
        {
            size = data.Length; allocation = Marshal.AllocHGlobal(checked(size + 64)); Pointer = At(allocation, 32);
            Marshal.Copy(Sentinel(size + 64), 0, allocation, size + 64); Marshal.Copy(data, 0, Pointer, size);
        }
        public byte[] Data() { byte[] result = new byte[size]; Marshal.Copy(Pointer, result, 0, size); return result; }
        public void Dispose()
        {
            try { byte[] before = new byte[32], after = new byte[32]; Marshal.Copy(allocation, before, 0, 32); Marshal.Copy(At(Pointer, size), after, 0, 32); Equal("native prefix guard", Sentinel(32), before); Equal("native suffix guard", Sentinel(32), after); }
            finally { Marshal.FreeHGlobal(allocation); }
        }
    }
    private static IntPtr LoadOriginal(string path)
    {
        Require(IntPtr.Size == 4, "Real original rewrite requires an x86 host"); path = Path.GetFullPath(path);
        using (SHA256 hash = SHA256.Create()) Require(BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant() == Sha, "Original DLL fingerprint changed");
        Assembly assembly = Assembly.LoadFrom(path);
        Require(String.Equals(Path.GetFullPath(assembly.Location), path, StringComparison.OrdinalIgnoreCase), "Original managed load path differs");
        IntPtr module = GetModuleHandle(Path.GetFileName(path)); StringBuilder actual = new StringBuilder(32768);
        Require(module != IntPtr.Zero && GetModuleFileName(module, actual, actual.Capacity) != 0 && String.Equals(Path.GetFullPath(actual.ToString()), path, StringComparison.OrdinalIgnoreCase), "Original native load path differs");
        return module;
    }
    private sealed class Fixture { public string Name, Kind; public int A, B, C; public uint Value; public byte[] Source; }
    private static IEnumerable<Fixture> Fixtures()
    {
        foreach(int s in new int[]{0,1,2,3,4,5,6,7})foreach(int d in new int[]{0,1,2,3,4,5,6,7})foreach(int count in new int[]{-1,0,1,2,7,8,9,15,16,17,31,32,33,511,16352})yield return new Fixture{Name="copy-"+s+"-"+d+"-"+count,Kind="copy",A=s,B=d,C=count};
        foreach(int packet in new int[]{0,1,3})foreach(uint frames in new uint[]{0,1,2,63,64,255,0x80000000,0xffffffff})yield return new Fixture{Name="header-"+packet+"-"+frames,Kind="header",A=packet,Value=frames};
        foreach(int old in new int[]{0,1,3})foreach(int next in new int[]{0,1,2,3})foreach(uint offset in new uint[]{0,1,32767,32768,0xffffffff})yield return new Fixture{Name="next-"+old+"-"+next+"-"+offset,Kind="next",A=old,B=next,Value=offset};
        foreach(int packet in new int[]{0,1,3})foreach(int bits in new int[]{0,1,7,8,31,32,33,16352,16383,16384})yield return new Fixture{Name="pad-"+packet+"-"+bits,Kind="pad",A=packet,B=bits};
        foreach(int offset in new int[]{0,1,7,8,31,65535})foreach(int value in new int[]{0,1})yield return new Fixture{Name="bit-"+offset+"-"+value,Kind="bit",A=offset,B=value};
        foreach(uint first in new uint[]{0,7,32767})foreach(int count in new int[]{1,7,8,511})yield return new Fixture{Name="sequence-"+first+"-"+count,Kind="sequence",C=count,Value=first};
    }
    private static IEnumerable<Fixture> RealFixtures()
    {
        foreach(int channels in new int[]{1,2})foreach(int frames in new int[]{257,4096})foreach(string stream in new string[]{"ram","stream"})
        {
            string name="xma-"+channels+"-"+frames+"-"+stream;
            byte[] packet=File.ReadAllBytes(Path.Combine("recovery/evidence/audio/xma-raw-validation/40232525056-1/cases",name,"raw-packets.bin"));
            Require(packet.Length==2048,"Accepted raw packet input must be one complete packet");byte[] source=new byte[2056];Array.Copy(packet,source,packet.Length);
            yield return new Fixture{Name="real-raw-copy-"+name,Kind="copy",A=32,B=35,C=16341,Source=source};
        }
    }
    private static IEnumerable<Fixture> AllFixtures(){foreach(Fixture f in Fixtures())yield return f;foreach(Fixture f in RealFixtures())yield return f;}
    private static byte[] Pattern(int n,int multiplier) { byte[] data=new byte[n];for(int i=0;i<n;++i)data[i]=(byte)((i*multiplier)^(i>>7)^0xa5);return data; }
    private sealed class Backend
    {
        public bool Original; public IntPtr Module;
        public NativeHeader Header;public NativeNext Next;public NativeCopy Copy;public NativeBit Set,Clear;public NativePad Pad;
        public MethodInfo ManagedHeader,ManagedNext,ManagedCopy,ManagedBit,ManagedPad;
        public Backend(bool original,string dll)
        {
            Original=original;
            if(original){Module=LoadOriginal(dll);Header=(NativeHeader)Marshal.GetDelegateForFunctionPointer(At(Module,0x5dc70),typeof(NativeHeader));Next=(NativeNext)Marshal.GetDelegateForFunctionPointer(At(Module,0x5dcf0),typeof(NativeNext));Copy=(NativeCopy)Marshal.GetDelegateForFunctionPointer(At(Module,0x5c270),typeof(NativeCopy));Set=(NativeBit)Marshal.GetDelegateForFunctionPointer(At(Module,0x5c390),typeof(NativeBit));Clear=(NativeBit)Marshal.GetDelegateForFunctionPointer(At(Module,0x5c3b0),typeof(NativeBit));Pad=(NativePad)Marshal.GetDelegateForFunctionPointer(At(Module,0x5ddb0),typeof(NativePad));}
            else{Require(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.AudioCompiler.dll"))&&GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll")==IntPtr.Zero,"Original target present or loaded in candidate host");Type type=Assembly.LoadFrom(Path.GetFullPath(dll)).GetType("BinaryAssetBuilder.AudioCompiler.XmaRawPacketWriter",true);ManagedHeader=type.GetMethod("FinalizeHeader");ManagedNext=type.GetMethod("StartNextPacket");ManagedCopy=type.GetMethod("CopyBits");ManagedBit=type.GetMethod("SetBit");ManagedPad=type.GetMethod("PadPacket");Require(ManagedHeader!=null&&ManagedNext!=null&&ManagedCopy!=null&&ManagedBit!=null&&ManagedPad!=null,"Raw packet API missing");}
        }
    }
    private sealed class Run : IDisposable
    {
        readonly Backend backend;readonly Guarded state,slots,packets,source;readonly byte[] initialState,initialSlots,sourceBytes;
        public byte[] Buffer;public int PacketIndex,NextIndex,BitCount,Returned;public uint Frames;
        public Run(Backend b,Fixture f)
        {
            backend=b;Buffer=Pattern(8192,19);sourceBytes=f.Source==null?Pattern(2056,37):(byte[])f.Source.Clone();PacketIndex=f.Kind=="header"||f.Kind=="next"||f.Kind=="pad"?f.A:0;NextIndex=f.Kind=="next"?f.B:1;Frames=f.Kind=="header"?f.Value:0x89abcdefu;BitCount=123;
            state=new Guarded(Sentinel(0x4050));slots=new Guarded(Sentinel(3*0x818));packets=new Guarded(Buffer);source=new Guarded(sourceBytes);
            Marshal.WriteIntPtr(state.Pointer,0x402c,slots.Pointer);Marshal.WriteIntPtr(state.Pointer,0x4030,packets.Pointer);Marshal.WriteInt32(state.Pointer,0x403c,NextIndex);StoreSlot();initialState=state.Data();initialSlots=slots.Data();
        }
        void StoreSlot(){Marshal.WriteInt32(slots.Pointer,0x818+8,unchecked((int)Frames));Marshal.WriteInt32(slots.Pointer,0x818+12,PacketIndex);Marshal.WriteInt32(slots.Pointer,0x818+16,BitCount);}
        void Pull()
        {
            Buffer=packets.Data();NextIndex=Marshal.ReadInt32(state.Pointer,0x403c);Frames=unchecked((uint)Marshal.ReadInt32(slots.Pointer,0x818+8));PacketIndex=Marshal.ReadInt32(slots.Pointer,0x818+12);BitCount=Marshal.ReadInt32(slots.Pointer,0x818+16);
            byte[] expected=(byte[])initialState.Clone();Array.Copy(BitConverter.GetBytes(NextIndex),0,expected,0x403c,4);Equal("complete native codec state including untouched fields",expected,state.Data());expected=(byte[])initialSlots.Clone();Array.Copy(BitConverter.GetBytes(Frames),0,expected,0x818+8,4);Array.Copy(BitConverter.GetBytes(PacketIndex),0,expected,0x818+12,4);Array.Copy(BitConverter.GetBytes(BitCount),0,expected,0x818+16,4);Equal("complete native stream-slot array including inactive slots",expected,slots.Data());Equal("native coded source immutable",sourceBytes,source.Data());
        }
        public void Start(uint first)
        {
            if(backend.Original){backend.Next(state.Pointer,1,first);Pull();}
            else{object[] a=new object[]{Buffer,PacketIndex,NextIndex,first,0u,0};backend.ManagedNext.Invoke(null,a);PacketIndex=(int)a[1];NextIndex=(int)a[2];Frames=(uint)a[4];BitCount=(int)a[5];}
        }
        public void Header(){if(backend.Original){backend.Header(state.Pointer,1);Pull();}else backend.ManagedHeader.Invoke(null,new object[]{Buffer,PacketIndex,Frames});}
        public void Pad(int bits){if(backend.Original){backend.Pad(At(packets.Pointer,PacketIndex*2048),bits);Pull();}else backend.ManagedPad.Invoke(null,new object[]{Buffer,PacketIndex,bits});}
        public void Bit(int offset,bool value){if(backend.Original){(value?backend.Set:backend.Clear)(packets.Pointer,offset);Pull();}else backend.ManagedBit.Invoke(null,new object[]{Buffer,offset,value});}
        public void Copy(int sourceBit,int destinationBit,int count){if(backend.Original){Returned=backend.Copy(source.Pointer,packets.Pointer,sourceBit,destinationBit,count);Pull();}else{byte[] input=(byte[])sourceBytes.Clone();Returned=(int)backend.ManagedCopy.Invoke(null,new object[]{input,sourceBit,Buffer,destinationBit,count});Equal("candidate coded source immutable",sourceBytes,input);}}
        public void AdvanceFrame(int count,uint frames){BitCount+=count;Frames=frames;if(backend.Original)StoreSlot();}
        public byte[] Snapshot(){byte[] bytes=new byte[20+Buffer.Length];Array.Copy(BitConverter.GetBytes(Returned),0,bytes,0,4);Array.Copy(BitConverter.GetBytes(PacketIndex),0,bytes,4,4);Array.Copy(BitConverter.GetBytes(NextIndex),0,bytes,8,4);Array.Copy(BitConverter.GetBytes(Frames),0,bytes,12,4);Array.Copy(BitConverter.GetBytes(BitCount),0,bytes,16,4);Array.Copy(Buffer,0,bytes,20,Buffer.Length);return bytes;}
        public void Dispose(){source.Dispose();packets.Dispose();slots.Dispose();state.Dispose();}
    }
    private static byte[] Execute(Backend backend,Fixture fixture)
    {
        using(Run run=new Run(backend,fixture))
        {
            if(fixture.Kind=="copy")run.Copy(fixture.A,fixture.B,fixture.C);
            else if(fixture.Kind=="header")run.Header();
            else if(fixture.Kind=="next")run.Start(fixture.Value);
            else if(fixture.Kind=="pad")run.Pad(fixture.B);
            else if(fixture.Kind=="bit")run.Bit(fixture.A,fixture.B!=0);
            else if(fixture.Kind=="sequence")
            {
                run.Start(fixture.Value);run.Copy(0,run.PacketIndex*16384+run.BitCount,fixture.C);run.AdvanceFrame(fixture.C,1);run.Header();run.Bit(run.PacketIndex*16384+run.BitCount-1,false);run.Pad(run.BitCount);
                run.Start(7);run.Copy(3,run.PacketIndex*16384+run.BitCount,fixture.C+1);run.AdvanceFrame(fixture.C+1,2);run.Header();run.Bit(run.PacketIndex*16384+run.BitCount-1,false);run.Pad(run.BitCount);
            }
            else throw new Exception("Unknown raw packet fixture");return run.Snapshot();
        }
    }
    private static void Reject(MethodInfo method,object[] args,Type expected,int bufferArgument)
    {
        byte[] before=args[bufferArgument] as byte[];before=before==null?null:(byte[])before.Clone();
        try{method.Invoke(null,args);}catch(TargetInvocationException e){if(e.InnerException==null||e.InnerException.GetType()!=expected)throw;if(before!=null)Equal("safety rejection leaves packet bytes unchanged",before,(byte[])args[bufferArgument]);return;}throw new Exception("Missing raw packet safety rejection "+method.Name);
    }
    private static void Safety(Backend b)
    {
        byte[] raw=new byte[4096];
        Reject(b.ManagedCopy,new object[]{null,0,raw,0,1},typeof(ArgumentNullException),2);
        Reject(b.ManagedCopy,new object[]{new byte[8],0,null,0,1},typeof(ArgumentNullException),2);
        Reject(b.ManagedCopy,new object[]{new byte[8],-1,raw,0,1},typeof(ArgumentOutOfRangeException),2);
        Reject(b.ManagedCopy,new object[]{new byte[1],1,raw,0,1},typeof(ArgumentOutOfRangeException),2);
        Reject(b.ManagedCopy,new object[]{new byte[8],0,new byte[0],0,1},typeof(ArgumentOutOfRangeException),2);
        Reject(b.ManagedCopy,new object[]{raw,0,raw,0,1},typeof(ArgumentException),2);
        Reject(b.ManagedHeader,new object[]{null,0,0u},typeof(ArgumentNullException),0);
        Reject(b.ManagedHeader,new object[]{raw,2,0u},typeof(ArgumentOutOfRangeException),0);
        Reject(b.ManagedNext,new object[]{raw,-1,1,0u,0u,0},typeof(ArgumentOutOfRangeException),0);
        Reject(b.ManagedNext,new object[]{raw,0,2,0u,0u,0},typeof(ArgumentOutOfRangeException),0);
        Reject(b.ManagedNext,new object[]{raw,0,-1,0u,0u,0},typeof(ArgumentOutOfRangeException),0);
        Reject(b.ManagedPad,new object[]{raw,0,-1},typeof(ArgumentOutOfRangeException),0);
        Reject(b.ManagedPad,new object[]{raw,0,16385},typeof(ArgumentOutOfRangeException),0);
        Reject(b.ManagedBit,new object[]{null,0,true},typeof(ArgumentNullException),0);
        Reject(b.ManagedBit,new object[]{raw,raw.Length*8,false},typeof(ArgumentOutOfRangeException),0);
    }
    private static int Main(string[] args)
    {
        try
        {
            Require(args.Length==3&&(args[0]=="original"||args[0]=="candidate"),"Usage: original|candidate assembly.dll transcript.tsv.gz");bool original=args[0]=="original";Backend backend=new Backend(original,args[1]);int count=0;
            if(original)
            {
                using(FileStream file=File.Create(args[2]))using(GZipStream gzip=new GZipStream(file,CompressionMode.Compress))using(StreamWriter writer=new StreamWriter(gzip))
                {writer.WriteLine("# originalDllSha256="+Sha+" realCpu=true nativeRvas=0x5dc70,0x5dcf0,0x5ddb0,0x5c270,0x5c390,0x5c3b0 syntheticStates=1098 realRawPacketInputs=8 actualEncoderStates=0");foreach(Fixture fixture in AllFixtures()){writer.WriteLine(fixture.Name+"\t"+Convert.ToBase64String(Execute(backend,fixture)));++count;}}
            }
            else
            {
                using(FileStream file=File.OpenRead(args[2]))using(GZipStream gzip=new GZipStream(file,CompressionMode.Decompress))using(StreamReader reader=new StreamReader(gzip))
                {string header=reader.ReadLine();Require(header!=null&&header.Contains("originalDllSha256="+Sha)&&header.Contains("realCpu=true")&&header.Contains("syntheticStates=1098")&&header.Contains("realRawPacketInputs=8")&&header.Contains("actualEncoderStates=0"),"Raw packet original CPU provenance missing");foreach(Fixture fixture in AllFixtures()){string line=reader.ReadLine();Require(line!=null,"Missing raw packet fixture");string[] f=line.Split('\t');Require(f.Length==2&&f[0]==fixture.Name,"Raw packet fixture coverage/order differs");Equal(fixture.Name,Convert.FromBase64String(f[1]),Execute(backend,fixture));++count;}Require(reader.ReadLine()==null,"Unexpected raw packet fixture");}
                Safety(backend);Require(GetModuleHandle("BinaryAssetBuilder.AudioCompiler.dll")==IntPtr.Zero,"Candidate loaded original target");
            }
            Require(count==TotalCases,"Incomplete raw packet matrix");Console.WriteLine("PASS "+args[0]+" raw XMA packet lifecycle and bitsplice: 1106 exact cases (1098 synthetic states:960 copy,24 header,60 next,30 padding,12 set/clear,12 complete two-packet sequences;8 accepted real raw payload inputs); full state/stream-slot/packet bytes, source immutability and guards; actualEncoderStates=0; no encoding support"+(original?"":"; 15 pre-write safety rejections"));return 0;
        }
        catch(Exception error){Console.Error.WriteLine(error);return 1;}
    }
}
