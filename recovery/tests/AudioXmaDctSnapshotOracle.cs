using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

// Original-only real DCT entry/return pairs. Native helper preserves integer
// registers, flags, and FXSAVE state on both sides; no managed callback runs
// inside the codec. All codec outputs must equal accepted baseline bytes.
internal static class AudioXmaDctSnapshotOracle
{
    const BindingFlags PrivateStatic=BindingFlags.NonPublic|BindingFlags.Static;
    const int Site=0x97bb0;
    [DllImport("kernel32.dll")]static extern bool VirtualProtect(IntPtr address,UIntPtr size,uint protection,out uint previous);
    [DllImport("kernel32.dll")]static extern IntPtr GetCurrentProcess();
    [DllImport("kernel32.dll")]static extern bool FlushInstructionCache(IntPtr process,IntPtr address,UIntPtr size);
    [DllImport("kernel32.dll")]static extern IntPtr VirtualAlloc(IntPtr address,UIntPtr size,uint allocation,uint protection);
    [DllImport("kernel32.dll")]static extern bool VirtualFree(IntPtr address,UIntPtr size,uint kind);
    [DllImport("AudioXmaDctSnapshotNative.dll",CallingConvention=CallingConvention.Cdecl,ExactSpelling=true,EntryPoint="DctConfigure")]static extern void Configure(uint next);
    [DllImport("AudioXmaDctSnapshotNative.dll",CallingConvention=CallingConvention.Cdecl,ExactSpelling=true,EntryPoint="DctHook")]static extern IntPtr Hook();
    [DllImport("AudioXmaDctSnapshotNative.dll",CallingConvention=CallingConvention.Cdecl,ExactSpelling=true,EntryPoint="DctSetCase")]static extern void SetCase(uint channels);
    [DllImport("AudioXmaDctSnapshotNative.dll",CallingConvention=CallingConvention.Cdecl,ExactSpelling=true,EntryPoint="DctCount")]static extern int Count();
    [DllImport("AudioXmaDctSnapshotNative.dll",CallingConvention=CallingConvention.Cdecl,ExactSpelling=true,EntryPoint="DctRecord")]static extern IntPtr Record(int index);
    [DllImport("AudioXmaDctSnapshotNative.dll",CallingConvention=CallingConvention.Cdecl,ExactSpelling=true,EntryPoint="DctSize")]static extern int Size();
    static IntPtr At(IntPtr p,int n){return new IntPtr(unchecked(p.ToInt32()+n));}
    static void Need(bool b,string message){if(!b)throw new Exception(message);}
    static string Hash(byte[] bytes){using(SHA256 h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
    static byte[] Read(IntPtr p,int n){byte[] b=new byte[n];Marshal.Copy(p,b,0,n);return b;}
    static object Field(object p,string name){return p.GetType().GetField(name).GetValue(p);}
    static string Hex(uint n){return "0x"+n.ToString("x8");}
    static void Json(string path,object value){File.WriteAllText(path,new JavaScriptSerializer().Serialize(value),new UTF8Encoding(false));}
    static void Patch(IntPtr site,byte[] bytes){uint old;Need(VirtualProtect(site,new UIntPtr(6),0x40,out old),"Patch protection failed");Marshal.Copy(bytes,0,site,6);uint discard;Need(VirtualProtect(site,new UIntPtr(6),old,out discard)&&FlushInstructionCache(GetCurrentProcess(),site,new UIntPtr(6)),"Patch restore/flush failed");}
    public sealed class SavedFile{public string path;public int length;public string sha256;}
    public sealed class SavedCase{public string name;public int channels,frames,sampleRate,blockBytes;public SavedFile[] files;}
    static int Main(string[] args)
    {
        IntPtr module=IntPtr.Zero,readerCode=IntPtr.Zero;Delegate control=null;uint saved=0;bool restore=false,patched=false;byte[] original=null;
        try{
            Need(args.Length==3&&IntPtr.Size==4,"Usage: x86 AudioXmaDctSnapshotOracle original.dll accepted-raw-dir output-dir");Directory.CreateDirectory(args[2]);Type raw=typeof(AudioXmaRawOracle);
            Console.WriteLine("PHASE load original and native DCT helper");Console.Out.Flush();
            module=(IntPtr)raw.GetMethod("Load",PrivateStatic).Invoke(null,new object[]{args[0]});
            Delegate encode=Marshal.GetDelegateForFunctionPointer(At(module,0x589d0),raw.GetNestedType("Encode",BindingFlags.NonPublic));
            Delegate convert=Marshal.GetDelegateForFunctionPointer(At(module,0xe9b0),raw.GetNestedType("ConvertPcm",BindingFlags.NonPublic));
            Delegate free=Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module,0xab1a8)),raw.GetNestedType("Free",BindingFlags.NonPublic));
            control=Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module,0xab210)),raw.GetNestedType("ControlFp",BindingFlags.NonPublic));saved=(uint)control.DynamicInvoke(new object[]{0u,0u});restore=true;
            byte[] code={0x8b,0x44,0x24,0x04,0xd9,0x38,0x0f,0xae,0x58,0x04,0xc3};readerCode=VirtualAlloc(IntPtr.Zero,new UIntPtr((uint)code.Length),0x3000,4);Need(readerCode!=IntPtr.Zero,"FPU reader allocation");Marshal.Copy(code,0,readerCode,code.Length);uint protection;Need(VirtualProtect(readerCode,new UIntPtr((uint)code.Length),0x20,out protection)&&FlushInstructionCache(GetCurrentProcess(),readerCode,new UIntPtr((uint)code.Length)),"FPU reader RX");
            Delegate reader=Marshal.GetDelegateForFunctionPointer(readerCode,raw.GetNestedType("ReadFpu",BindingFlags.NonPublic));
            Need(Size()==16468,"Native snapshot structure size differs");original=Read(At(module,Site),6);Need(Hash(original)==Hash(new byte[]{0xff,0x90,0xf0,0x01,0x00,0x00}),"Original CALL differs");Configure(unchecked((uint)At(module,Site+6).ToInt32()));IntPtr hook=Hook();Need(hook!=IntPtr.Zero,"Native hook missing");byte[] jump=new byte[6];jump[0]=0xe9;Buffer.BlockCopy(BitConverter.GetBytes(unchecked(hook.ToInt32()-module.ToInt32()-Site-5)),0,jump,1,4);jump[5]=0x90;Patch(At(module,Site),jump);patched=true;
            List<object> cases=new List<object>();
            foreach(int n in new int[]{1,2}){
                string name="xma-"+n+"-4096-ram",baseline=Path.Combine(args[1],"cases",name),directory=Path.Combine(args[2],name);Directory.CreateDirectory(directory);
                SavedCase data=new JavaScriptSerializer().Deserialize<SavedCase>(File.ReadAllText(Path.Combine(baseline,"case.json")));Need(data.name==name&&data.channels==n&&data.frames==4096&&data.sampleRate==22050&&data.blockBytes==0x28000,"Accepted baseline differs");Dictionary<string,SavedFile> files=new Dictionary<string,SavedFile>();foreach(SavedFile f in data.files)files.Add(f.path,f);
                byte[] source=File.ReadAllBytes(Path.Combine(baseline,"source-float32le.bin"));Need(Hash(source)==files["source-float32le.bin"].sha256,"Accepted input hash differs");SetCase((uint)n);int first=Count();Console.WriteLine("PHASE capture "+name+" before="+first);Console.Out.Flush();
                object result=raw.GetMethod("Run",PrivateStatic).Invoke(null,new object[]{module,encode,convert,free,control,reader,source,n,data.frames,data.sampleRate,data.blockBytes});Need(Count()>first&&Count()<=256,"Snapshot count differs");byte[][] outputs=(byte[][])Field(result,"Outputs");string[] names={"raw-packets.bin","allocation2.bin","allocation3.bin"};for(int i=0;i<3;i++){File.WriteAllBytes(Path.Combine(directory,names[i]),outputs[i]);Need(outputs[i].Length==files[names[i]].length&&Hash(outputs[i])==files[names[i]].sha256,"Snapshot changed "+names[i]);}Need(Hash((byte[])Field(result,"Pcm"))==files["source-signed16le.bin"].sha256&&Hash((byte[])Field(result,"Descriptor"))==files["descriptor-pointer-zero.bin"].sha256,"Snapshot changed native input");
                cases.Add(new{name=name,snapshots=Count()-first,rawOutputsEqualAcceptedUninstrumented=true,call=Field(result,"Evidence")});Console.WriteLine("PASS "+name+" snapshots="+(Count()-first)+" full original outputs unchanged");
            }
            int count=Count();Need(count>0&&count<=256,"DCT snapshot capacity exceeded");List<object> snapshots=new List<object>();
            int imageSize=Marshal.ReadInt32(At(module,Marshal.ReadInt32(At(module,0x3c))+24+56));
            for(int i=0;i<count;i++){
                IntPtr p=Record(i);Need(p!=IntPtr.Zero,"Missing native DCT record");byte[] bytes=Read(p,Size());Need(BitConverter.ToUInt32(bytes,0)==0xa55a1234u&&BitConverter.ToUInt32(bytes,16464)==0x5aa54321u,"Native DCT record guard differs");
                string name="dct-"+i.ToString("D5"),directory=Path.Combine(args[2],"transforms",name);Directory.CreateDirectory(directory);byte[] arguments=new byte[36];Array.Copy(bytes,4,arguments,0,36);File.WriteAllBytes(Path.Combine(directory,"arguments.bin"),arguments);
                Need(BitConverter.ToUInt32(bytes,72)==0,"Incomplete native DCT pair status="+BitConverter.ToUInt32(bytes,72)+" record="+i);
                int n=BitConverter.ToInt32(bytes,44);Need(n>=64&&n<=2048&&(n&(n-1))==0,"Unsupported native DCT size");byte[] input=new byte[n*4],output=new byte[n*4];Array.Copy(bytes,80,input,0,input.Length);Array.Copy(bytes,8272,output,0,output.Length);File.WriteAllBytes(Path.Combine(directory,"input.bin"),input);File.WriteAllBytes(Path.Combine(directory,"output.bin"),output);
                uint target=BitConverter.ToUInt32(bytes,52),fft=BitConverter.ToUInt32(bytes,56),hresult=BitConverter.ToUInt32(bytes,68),optional=BitConverter.ToUInt32(bytes,60),optionalAfter=BitConverter.ToUInt32(bytes,64);uint targetRva=unchecked(target-(uint)module.ToInt32()),fftRva=unchecked(fft-(uint)module.ToInt32());
                Need(targetRva==0xa81db&&hresult==0&&(optional==0||optionalAfter==0),"Unexpected actual DCT target/result");
                object entry=new{name=name,channels=BitConverter.ToInt32(bytes,76),n=n,scaleBits=BitConverter.ToInt32(bytes,48),inputPath="input.bin",outputPath="output.bin",argumentsPath="arguments.bin",inputSha256=Hash(input),outputSha256=Hash(output),argumentsSha256=Hash(arguments),targetRva=Hex(targetRva),fftCallbackRva=fftRva<(uint)imageSize?Hex(fftRva):null,fftCallbackAddress=Hex(fft),optionalOutPointer=Hex(optional),optionalOutAfter=optional==0?(int?)null:(int)optionalAfter,hresult=hresult,captureStatus="real-original-entry-and-return",siteRva="0x97bb0",resumeRva="0x97bb6"};Json(Path.Combine(directory,"case.json"),entry);snapshots.Add(entry);
            }
            Json(Path.Combine(args[2],"dct-snapshots.json"),new{status="original-evidence-only",baselineSourceCommit="cd66d9a2c821bc41f1f975aac8ac0a5bac6a6499",baselineRunId="40232525056",backend="real-windows-x86-cpu-native-DCT-entry-return-snapshot",originalDllSha256="a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139",caseCount=cases.Count,nativeEncodeCalls=cases.Count,snapshotCount=count,actualDctTargetRva="0xa81db",numericalEncoderRecovered=false,cases=cases,transforms=snapshots});
            Console.WriteLine("PASS original-only real DCT snapshots: 2 encodes, "+count+" entry/return pairs; all complete outputs equal accepted uninstrumented original");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
        finally{if(patched)Patch(At(module,Site),original);if(restore)control.DynamicInvoke(new object[]{saved,0x30300u});if(readerCode!=IntPtr.Zero)VirtualFree(readerCode,UIntPtr.Zero,0x8000);}
    }
}
