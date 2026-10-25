using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

// Original-only real whole signal-wrapper entry/return pairs. Native helper preserves integer
// registers, flags, and FXSAVE state on both sides; no managed callback runs
// inside the codec. All codec outputs must equal accepted baseline bytes.
internal static class AudioXmaSignalSnapshotOracle
{
    const BindingFlags PrivateStatic=BindingFlags.NonPublic|BindingFlags.Static;
    const int Site=0x8c0af;
    [DllImport("kernel32.dll")]static extern bool VirtualProtect(IntPtr address,UIntPtr size,uint protection,out uint previous);
    [DllImport("kernel32.dll")]static extern IntPtr GetCurrentProcess();
    [DllImport("kernel32.dll")]static extern bool FlushInstructionCache(IntPtr process,IntPtr address,UIntPtr size);
    [DllImport("kernel32.dll")]static extern IntPtr VirtualAlloc(IntPtr address,UIntPtr size,uint allocation,uint protection);
    [DllImport("kernel32.dll")]static extern bool VirtualFree(IntPtr address,UIntPtr size,uint kind);
    [DllImport("AudioXmaSignalSnapshotNative.dll",CallingConvention=CallingConvention.Cdecl,ExactSpelling=true,EntryPoint="SignalConfigure")]static extern void Configure(uint next,uint target);
    [DllImport("AudioXmaSignalSnapshotNative.dll",CallingConvention=CallingConvention.Cdecl,ExactSpelling=true,EntryPoint="SignalHook")]static extern IntPtr Hook();
    [DllImport("AudioXmaSignalSnapshotNative.dll",CallingConvention=CallingConvention.Cdecl,ExactSpelling=true,EntryPoint="SignalSetCase")]static extern void SetCase(uint channels);
    [DllImport("AudioXmaSignalSnapshotNative.dll",CallingConvention=CallingConvention.Cdecl,ExactSpelling=true,EntryPoint="SignalCount")]static extern int Count();
    [DllImport("AudioXmaSignalSnapshotNative.dll",CallingConvention=CallingConvention.Cdecl,ExactSpelling=true,EntryPoint="SignalRecord")]static extern IntPtr Record(int index);
    [DllImport("AudioXmaSignalSnapshotNative.dll",CallingConvention=CallingConvention.Cdecl,ExactSpelling=true,EntryPoint="SignalSize")]static extern int Size();
    static IntPtr At(IntPtr p,int n){return new IntPtr(unchecked(p.ToInt32()+n));}
    static void Need(bool b,string message){if(!b)throw new Exception(message);}
    static string Hash(byte[] bytes){using(SHA256 h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
    static byte[] Read(IntPtr p,int n){byte[] b=new byte[n];Marshal.Copy(p,b,0,n);return b;}
    static object Field(object p,string name){return p.GetType().GetField(name).GetValue(p);}
    static string Hex(uint n){return "0x"+n.ToString("x8");}
    static void Json(string path,object value){File.WriteAllText(path,new JavaScriptSerializer().Serialize(value),new UTF8Encoding(false));}
    static void Patch(IntPtr site,byte[] bytes){uint old;Need(VirtualProtect(site,new UIntPtr(5),0x40,out old),"Patch protection failed");Marshal.Copy(bytes,0,site,5);uint discard;Need(VirtualProtect(site,new UIntPtr(5),old,out discard)&&FlushInstructionCache(GetCurrentProcess(),site,new UIntPtr(5)),"Patch restore/flush failed");}
    public sealed class SavedFile{public string path;public int length;public string sha256;}
    public sealed class SavedCase{public string name;public int channels,frames,sampleRate,blockBytes;public SavedFile[] files;}
    static int Main(string[] args)
    {
        IntPtr module=IntPtr.Zero,readerCode=IntPtr.Zero;Delegate control=null;uint saved=0;bool restore=false,patched=false;byte[] original=null;
        try{
            Need(args.Length==3&&IntPtr.Size==4,"Usage: x86 AudioXmaSignalSnapshotOracle original.dll accepted-raw-dir output-dir");Directory.CreateDirectory(args[2]);Type raw=typeof(AudioXmaRawOracle);
            Console.WriteLine("PHASE load original and native signal helper");Console.Out.Flush();
            module=(IntPtr)raw.GetMethod("Load",PrivateStatic).Invoke(null,new object[]{args[0]});
            Delegate encode=Marshal.GetDelegateForFunctionPointer(At(module,0x589d0),raw.GetNestedType("Encode",BindingFlags.NonPublic));
            Delegate convert=Marshal.GetDelegateForFunctionPointer(At(module,0xe9b0),raw.GetNestedType("ConvertPcm",BindingFlags.NonPublic));
            Delegate free=Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module,0xab1a8)),raw.GetNestedType("Free",BindingFlags.NonPublic));
            control=Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module,0xab210)),raw.GetNestedType("ControlFp",BindingFlags.NonPublic));saved=(uint)control.DynamicInvoke(new object[]{0u,0u});restore=true;
            byte[] code={0x8b,0x44,0x24,0x04,0xd9,0x38,0x0f,0xae,0x58,0x04,0xc3};readerCode=VirtualAlloc(IntPtr.Zero,new UIntPtr((uint)code.Length),0x3000,4);Need(readerCode!=IntPtr.Zero,"FPU reader allocation");Marshal.Copy(code,0,readerCode,code.Length);uint protection;Need(VirtualProtect(readerCode,new UIntPtr((uint)code.Length),0x20,out protection)&&FlushInstructionCache(GetCurrentProcess(),readerCode,new UIntPtr((uint)code.Length)),"FPU reader RX");
            Delegate reader=Marshal.GetDelegateForFunctionPointer(readerCode,raw.GetNestedType("ReadFpu",BindingFlags.NonPublic));
            Need(Size()==25716,"Native snapshot structure size differs");original=Read(At(module,Site),5);Need(Hash(original)==Hash(new byte[]{0xe8,0x4c,0xba,0x00,0x00}),"Original CALL differs");Configure(unchecked((uint)At(module,Site+5).ToInt32()),unchecked((uint)At(module,0x97b00).ToInt32()));IntPtr hook=Hook();Need(hook!=IntPtr.Zero,"Native hook missing");byte[] jump=new byte[5];jump[0]=0xe9;Buffer.BlockCopy(BitConverter.GetBytes(unchecked(hook.ToInt32()-module.ToInt32()-Site-5)),0,jump,1,4);Patch(At(module,Site),jump);patched=true;
            List<object> cases=new List<object>();
            foreach(int n in new int[]{1,2}){
                string name="xma-"+n+"-4096-ram",baseline=Path.Combine(args[1],"cases",name),directory=Path.Combine(args[2],name);Directory.CreateDirectory(directory);
                SavedCase data=new JavaScriptSerializer().Deserialize<SavedCase>(File.ReadAllText(Path.Combine(baseline,"case.json")));Need(data.name==name&&data.channels==n&&data.frames==4096&&data.sampleRate==22050&&data.blockBytes==0x28000,"Accepted baseline differs");Dictionary<string,SavedFile> files=new Dictionary<string,SavedFile>();foreach(SavedFile f in data.files)files.Add(f.path,f);
                byte[] source=File.ReadAllBytes(Path.Combine(baseline,"source-float32le.bin"));Need(Hash(source)==files["source-float32le.bin"].sha256,"Accepted input hash differs");SetCase((uint)n);int first=Count();Console.WriteLine("PHASE capture "+name+" before="+first);Console.Out.Flush();
                object result=raw.GetMethod("Run",PrivateStatic).Invoke(null,new object[]{module,encode,convert,free,control,reader,source,n,data.frames,data.sampleRate,data.blockBytes});Need(Count()>first&&Count()<=256,"Snapshot count differs");byte[][] outputs=(byte[][])Field(result,"Outputs");string[] names={"raw-packets.bin","allocation2.bin","allocation3.bin"};for(int i=0;i<3;i++){File.WriteAllBytes(Path.Combine(directory,names[i]),outputs[i]);Need(outputs[i].Length==files[names[i]].length&&Hash(outputs[i])==files[names[i]].sha256,"Snapshot changed "+names[i]);}Need(Hash((byte[])Field(result,"Pcm"))==files["source-signed16le.bin"].sha256&&Hash((byte[])Field(result,"Descriptor"))==files["descriptor-pointer-zero.bin"].sha256,"Snapshot changed native input");
                cases.Add(new{name=name,snapshots=Count()-first,rawOutputsEqualAcceptedUninstrumented=true,call=Field(result,"Evidence")});Console.WriteLine("PASS "+name+" snapshots="+(Count()-first)+" full original outputs unchanged");
            }
            int count=Count();Need(count>0&&count<=256,"Signal snapshot capacity exceeded");List<object> snapshots=new List<object>();
            for(int i=0;i<count;i++){
                IntPtr p=Record(i);Need(p!=IntPtr.Zero,"Missing native signal record");byte[] bytes=Read(p,Size());Need(BitConverter.ToUInt32(bytes,0)==0xa55a1234u&&BitConverter.ToUInt32(bytes,25712)==0x5aa54321u,"Native signal record guard differs");
                string name="signal-"+i.ToString("D5"),directory=Path.Combine(args[2],"signals",name);Directory.CreateDirectory(directory);byte[] arguments=new byte[12],rawFields=new byte[96];Array.Copy(bytes,4,arguments,0,12);Array.Copy(bytes,16,rawFields,0,96);File.WriteAllBytes(Path.Combine(directory,"arguments.bin"),arguments);File.WriteAllBytes(Path.Combine(directory,"fields.bin"),rawFields);uint[] f=new uint[24];Buffer.BlockCopy(rawFields,0,f,0,96);
                Need(f[8]==0,"Incomplete native signal pair status="+f[8]+" record="+i);int n=(int)f[10];Need(n>=64&&n<=2048&&(n&(n-1))==0&&f[11]==n,"Unsupported signal shape");
                byte[] input=new byte[n*8],output=new byte[n*4],weights=f[20]!=0?new byte[1024]:new byte[0];Array.Copy(bytes,112,input,0,input.Length);Array.Copy(bytes,16496,output,0,output.Length);if(weights.Length!=0)Array.Copy(bytes,24688,weights,0,weights.Length);File.WriteAllBytes(Path.Combine(directory,"input.bin"),input);File.WriteAllBytes(Path.Combine(directory,"output.bin"),output);if(weights.Length!=0)File.WriteAllBytes(Path.Combine(directory,"weights.bin"),weights);
                uint dctRva=unchecked(f[15]-(uint)module.ToInt32()),fftRva=unchecked(f[16]-(uint)module.ToInt32());Need(dctRva==0xa81db&&fftRva==0xa7de3&&f[7]==0&&(f[5]==0||f[6]==0),"Unexpected signal callback/result");
                object entry=new{name=name,channels=(int)f[9],n=n,leadingZeroCount=unchecked((int)f[13]),legacyUpperLimit=unchecked((int)f[14]),codecVersion=unchecked((int)f[12]),tailFlag=unchecked((int)f[17]),clipGainBits=unchecked((int)f[18]),retainedFractionBits=unchecked((int)f[19]),weightsEnabled=f[20]!=0,inputPath="input.bin",outputPath="output.bin",weightsPath=weights.Length!=0?"weights.bin":null,argumentsPath="arguments.bin",rawFieldsPath="fields.bin",inputSha256=Hash(input),outputSha256=Hash(output),weightsSha256=weights.Length!=0?Hash(weights):null,argumentsSha256=Hash(arguments),rawFieldsSha256=Hash(rawFields),targetRva="0x00097b00",dctCallbackRva=Hex(dctRva),fftCallbackRva=Hex(fftRva),outputShiftAfter=f[5]==0?(int?)null:unchecked((int)f[6]),hresult=f[7],capturedInputOptionsWeightsImmutable=true,captureStatus="real-original-entry-and-return",siteRva="0x8c0af",resumeRva="0x8c0b4"};Json(Path.Combine(directory,"case.json"),entry);snapshots.Add(entry);
            }
            Json(Path.Combine(args[2],"signal-snapshots.json"),new{status="original-evidence-only",baselineSourceCommit="cd66d9a2c821bc41f1f975aac8ac0a5bac6a6499",baselineRunId="40232525056",backend="real-windows-x86-cpu-native-signal-entry-return-snapshot",originalDllSha256="a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139",caseCount=cases.Count,nativeEncodeCalls=cases.Count,snapshotCount=count,actualTargetRva="0x97b00",numericalEncoderRecovered=false,cases=cases,signals=snapshots});
            Console.WriteLine("PASS original-only real whole-signal snapshots: 2 encodes, "+count+" entry/return pairs; all complete outputs equal accepted uninstrumented original");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
        finally{if(patched)Patch(At(module,Site),original);if(restore)control.DynamicInvoke(new object[]{saved,0x30300u});if(readerCode!=IntPtr.Zero)VirtualFree(readerCode,UIntPtr.Zero,0x8000);}
    }
}
