using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

// Original-only real window inputs. Native helper preserves integer/flags and
// FXSAVE state; no managed callback executes inside the codec. Full codec
// outputs must match accepted uninstrumented original bytes.
internal static class AudioXmaWindowSnapshotOracle
{
    const BindingFlags PrivateStatic=BindingFlags.NonPublic|BindingFlags.Static;
    const int Site=0x97d97;
    [DllImport("kernel32.dll")]static extern bool VirtualProtect(IntPtr address,UIntPtr size,uint protection,out uint previous);
    [DllImport("kernel32.dll")]static extern IntPtr GetCurrentProcess();
    [DllImport("kernel32.dll")]static extern bool FlushInstructionCache(IntPtr process,IntPtr address,UIntPtr size);
    [DllImport("kernel32.dll")]static extern IntPtr VirtualAlloc(IntPtr address,UIntPtr size,uint allocation,uint protection);
    [DllImport("kernel32.dll")]static extern bool VirtualFree(IntPtr address,UIntPtr size,uint kind);
    [DllImport("AudioXmaSnapshotNative.dll",CallingConvention=CallingConvention.Cdecl,ExactSpelling=true,EntryPoint="SnapshotConfigure")]static extern void Configure(uint next);
    [DllImport("AudioXmaSnapshotNative.dll",CallingConvention=CallingConvention.Cdecl,ExactSpelling=true,EntryPoint="SnapshotHook")]static extern IntPtr Hook();
    [DllImport("AudioXmaSnapshotNative.dll",CallingConvention=CallingConvention.Cdecl,ExactSpelling=true,EntryPoint="SnapshotSetCase")]static extern void SetCase(uint channels);
    [DllImport("AudioXmaSnapshotNative.dll",CallingConvention=CallingConvention.Cdecl,ExactSpelling=true,EntryPoint="SnapshotCount")]static extern int Count();
    [DllImport("AudioXmaSnapshotNative.dll",CallingConvention=CallingConvention.Cdecl,ExactSpelling=true,EntryPoint="SnapshotRecord")]static extern IntPtr Record(int index);
    [DllImport("AudioXmaSnapshotNative.dll",CallingConvention=CallingConvention.Cdecl,ExactSpelling=true,EntryPoint="SnapshotSize")]static extern int Size();
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
            Need(args.Length==3&&IntPtr.Size==4,"Usage: x86 AudioXmaWindowSnapshotOracle original.dll accepted-raw-dir output-dir");Directory.CreateDirectory(args[2]);Type raw=typeof(AudioXmaRawOracle);
            Console.WriteLine("PHASE load original and native helper");Console.Out.Flush();
            module=(IntPtr)raw.GetMethod("Load",PrivateStatic).Invoke(null,new object[]{args[0]});
            Delegate encode=Marshal.GetDelegateForFunctionPointer(At(module,0x589d0),raw.GetNestedType("Encode",BindingFlags.NonPublic));
            Delegate convert=Marshal.GetDelegateForFunctionPointer(At(module,0xe9b0),raw.GetNestedType("ConvertPcm",BindingFlags.NonPublic));
            Delegate free=Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module,0xab1a8)),raw.GetNestedType("Free",BindingFlags.NonPublic));
            control=Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module,0xab210)),raw.GetNestedType("ControlFp",BindingFlags.NonPublic));saved=(uint)control.DynamicInvoke(new object[]{0u,0u});restore=true;
            byte[] code={0x8b,0x44,0x24,0x04,0xd9,0x38,0x0f,0xae,0x58,0x04,0xc3};readerCode=VirtualAlloc(IntPtr.Zero,new UIntPtr((uint)code.Length),0x3000,4);Need(readerCode!=IntPtr.Zero,"FPU reader allocation");Marshal.Copy(code,0,readerCode,code.Length);uint protection;Need(VirtualProtect(readerCode,new UIntPtr((uint)code.Length),0x20,out protection)&&FlushInstructionCache(GetCurrentProcess(),readerCode,new UIntPtr((uint)code.Length)),"FPU reader RX");
            Delegate reader=Marshal.GetDelegateForFunctionPointer(readerCode,raw.GetNestedType("ReadFpu",BindingFlags.NonPublic));
            Need(Size()==131232,"Native snapshot structure size differs");original=Read(At(module,Site),6);Need(Hash(original)==Hash(new byte[]{0xff,0x96,0x08,0x8e,0x01,0x00}),"Original CALL differs");Configure(unchecked((uint)At(module,Site+6).ToInt32()));IntPtr hook=Hook();Need(hook!=IntPtr.Zero,"Native hook missing");byte[] jump=new byte[6];jump[0]=0xe9;Buffer.BlockCopy(BitConverter.GetBytes(unchecked(hook.ToInt32()-module.ToInt32()-Site-5)),0,jump,1,4);jump[5]=0x90;Patch(At(module,Site),jump);patched=true;
            List<object> cases=new List<object>();
            foreach(int n in new int[]{1,2}){
                string name="xma-"+n+"-4096-ram",baseline=Path.Combine(args[1],"cases",name),directory=Path.Combine(args[2],name);Directory.CreateDirectory(directory);
                SavedCase data=new JavaScriptSerializer().Deserialize<SavedCase>(File.ReadAllText(Path.Combine(baseline,"case.json")));Need(data.name==name&&data.channels==n&&data.frames==4096&&data.sampleRate==22050&&data.blockBytes==0x28000,"Accepted baseline differs");Dictionary<string,SavedFile> files=new Dictionary<string,SavedFile>();foreach(SavedFile f in data.files)files.Add(f.path,f);
                byte[] source=File.ReadAllBytes(Path.Combine(baseline,"source-float32le.bin"));Need(Hash(source)==files["source-float32le.bin"].sha256,"Accepted input hash differs");SetCase((uint)n);int first=Count();Console.WriteLine("PHASE capture "+name+" before="+first);Console.Out.Flush();
                object result=raw.GetMethod("Run",PrivateStatic).Invoke(null,new object[]{module,encode,convert,free,control,reader,source,n,data.frames,data.sampleRate,data.blockBytes});Need(Count()>first&&Count()<=256,"Snapshot count differs");byte[][] outputs=(byte[][])Field(result,"Outputs");string[] names={"raw-packets.bin","allocation2.bin","allocation3.bin"};for(int i=0;i<3;i++){File.WriteAllBytes(Path.Combine(directory,names[i]),outputs[i]);Need(outputs[i].Length==files[names[i]].length&&Hash(outputs[i])==files[names[i]].sha256,"Snapshot changed "+names[i]);}Need(Hash((byte[])Field(result,"Pcm"))==files["source-signed16le.bin"].sha256&&Hash((byte[])Field(result,"Descriptor"))==files["descriptor-pointer-zero.bin"].sha256,"Snapshot changed native input");
                cases.Add(new{name=name,snapshots=Count()-first,rawOutputsEqualAcceptedUninstrumented=true,call=Field(result,"Evidence")});Console.WriteLine("PASS "+name+" snapshots="+(Count()-first)+" full original outputs unchanged");
            }
            int count=Count();Need(count>0&&count<=256,"Snapshot capacity exceeded");List<object> windows=new List<object>();
            for(int i=0;i<count;i++){
                IntPtr p=Record(i);Need(p!=IntPtr.Zero,"Missing native snapshot");byte[] bytes=Read(p,Size());Need(BitConverter.ToUInt32(bytes,0)==0xa55a1234u&&BitConverter.ToUInt32(bytes,131228)==0x5aa54321u,"Native snapshot guard differs");uint[] a=new uint[32];Buffer.BlockCopy(bytes,4,a,0,128);string name="window-"+i.ToString("D5"),directory=Path.Combine(args[2],"windows",name);Directory.CreateDirectory(directory);byte[] arguments=new byte[128];Array.Copy(bytes,4,arguments,0,128);File.WriteAllBytes(Path.Combine(directory,"arguments.bin"),arguments);int status=BitConverter.ToInt32(bytes,144);Need(status==0,"Bounded snapshot rejected status="+status+" record="+i);
                int newerLength=BitConverter.ToInt32(bytes,136),olderLength=BitConverter.ToInt32(bytes,140);Need(newerLength>=0&&newerLength<=65536&&olderLength>=0&&olderLength<=65536,"Snapshot source span differs");byte[] newer=new byte[newerLength],older=new byte[olderLength];Array.Copy(bytes,156,newer,0,newerLength);Array.Copy(bytes,65692,older,0,olderLength);File.WriteAllBytes(Path.Combine(directory,"newer.bin"),newer);File.WriteAllBytes(Path.Combine(directory,"older.bin"),older);
                int[] boundaries=new int[4],rising=new int[5],falling=new int[5];for(int j=0;j<4;j++)boundaries[j]=unchecked((int)a[15+j]);for(int j=0;j<5;j++){rising[j]=unchecked((int)a[19+j]);falling[j]=unchecked((int)a[24+j]);}
                object entry=new{name=name,channels=BitConverter.ToInt32(bytes,148),bits=(int)a[13],stride=(int)a[14],blockOffset=BitConverter.ToInt32(bytes,132),fullFrames=(int)a[9],windowFrames=(int)a[11],width=(int)a[10],boundaries=boundaries,risingBits=rising,fallingBits=falling,olderBytes=olderLength,newerBytes=newerLength,olderSha256=Hash(older),newerSha256=Hash(newer),argumentsSha256=Hash(arguments),representation=(int)a[29],readerFlag=(int)a[30],readerCallback=Hex(a[31]),originalOutputPointer=Hex(BitConverter.ToUInt32(bytes,152)),olderPath="older.bin",newerPath="newer.bin",argumentsPath="arguments.bin",captureStatus="bounded-real-original-input",sourceSpanPolicy="six actual phases; prefix through final read byte; span <= fullFrames*sampleBytes*stride and 65536"};Json(Path.Combine(directory,"case.json"),entry);windows.Add(entry);
            }
            Json(Path.Combine(args[2],"window-snapshots.json"),new{status="original-evidence-only",baselineSourceCommit="cd66d9a2c821bc41f1f975aac8ac0a5bac6a6499",baselineRunId="40232525056",backend="real-windows-x86-cpu-native-bounded-snapshot",originalDllSha256="a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139",caseCount=cases.Count,nativeEncodeCalls=cases.Count,snapshotCount=count,numericalEncoderRecovered=false,cases=cases,windows=windows});
            Console.WriteLine("PASS original-only bounded window snapshots: 2 encodes, "+count+" real windows; all complete outputs equal uninstrumented accepted original");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
        finally{if(patched)Patch(At(module,Site),original);if(restore)control.DynamicInvoke(new object[]{saved,0x30300u});if(readerCode!=IntPtr.Zero)VirtualFree(readerCode,UIntPtr.Zero,0x8000);}
    }
}
