using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Web.Script.Serialization;

// Original-only longer, non-impulse corpus. Reuses the accepted raw ABI host;
// does not load candidate source or enable a product XMA encoding route.
internal static class AudioXmaCorpusOracle
{
    const BindingFlags PrivateStatic=BindingFlags.NonPublic|BindingFlags.Static;
    [DllImport("kernel32.dll")]static extern IntPtr VirtualAlloc(IntPtr address,UIntPtr size,uint type,uint protection);
    [DllImport("kernel32.dll")]static extern bool VirtualProtect(IntPtr address,UIntPtr size,uint protection,out uint previous);
    [DllImport("kernel32.dll")]static extern bool VirtualFree(IntPtr address,UIntPtr size,uint type);
    [DllImport("kernel32.dll")]static extern bool FlushInstructionCache(IntPtr process,IntPtr address,UIntPtr size);
    [DllImport("kernel32.dll")]static extern IntPtr GetCurrentProcess();
    static IntPtr At(IntPtr p,int n){return new IntPtr(unchecked(p.ToInt32()+n));}
    static void Need(bool yes,string text){if(!yes)throw new Exception(text);}
    static string Hash(byte[] b){using(SHA256 h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(b)).Replace("-","").ToLowerInvariant();}
    static object Field(object x,string name){return x.GetType().GetField(name).GetValue(x);}
    static object Save(string directory,string path,byte[] bytes){File.WriteAllBytes(Path.Combine(directory,path),bytes);return new{path=path,bytes=bytes.Length,sha256=Hash(bytes)};}
    static byte[] Real(string path,string sha,int channels,int frames)
    {
        byte[] b=File.ReadAllBytes(path);Need(Hash(b)==sha,"Real source fingerprint changed");
        Need(System.Text.Encoding.ASCII.GetString(b,8,8)=="WAVEfmt "&&BitConverter.ToUInt16(b,20)==1&&BitConverter.ToUInt16(b,22)==1&&BitConverter.ToInt32(b,24)==22050&&BitConverter.ToUInt16(b,34)==8&&System.Text.Encoding.ASCII.GetString(b,36,4)=="data","Pinned PCM8 source format changed");
        int length=BitConverter.ToInt32(b,40);Need(44L+length<=b.Length&&channels*frames<=length,"Real input fragment exceeds payload");
        float[] f=new float[channels*frames];for(int i=0;i<f.Length;i++)f[i]=(b[44+i]-128)*(1.0f/128.0f);
        byte[] output=new byte[f.Length*4];Buffer.BlockCopy(f,0,output,0,output.Length);return output;
    }
    static int Main(string[] args)
    {
        Delegate control=null;uint saved=0;bool restore=false;IntPtr readerMemory=IntPtr.Zero;
        try
        {
            Need(args.Length==3&&IntPtr.Size==4,"Usage: original.dll repository-root output-directory");Directory.CreateDirectory(args[2]);Type raw=typeof(AudioXmaRawOracle);
            IntPtr module=(IntPtr)raw.GetMethod("Load",PrivateStatic).Invoke(null,new object[]{args[0]});
            Delegate encode=Marshal.GetDelegateForFunctionPointer(At(module,0x589d0),raw.GetNestedType("Encode",BindingFlags.NonPublic));
            Delegate convert=Marshal.GetDelegateForFunctionPointer(At(module,0xe9b0),raw.GetNestedType("ConvertPcm",BindingFlags.NonPublic));
            Delegate free=Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module,0xab1a8)),raw.GetNestedType("Free",BindingFlags.NonPublic));
            control=Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module,0xab210)),raw.GetNestedType("ControlFp",BindingFlags.NonPublic));saved=(uint)control.DynamicInvoke(new object[]{0u,0u});restore=true;
            byte[] code={0x8b,0x44,0x24,0x04,0xd9,0x38,0x0f,0xae,0x58,0x04,0xc3};readerMemory=VirtualAlloc(IntPtr.Zero,new UIntPtr((uint)code.Length),0x3000,4);Need(readerMemory!=IntPtr.Zero,"FPU reader allocation");Marshal.Copy(code,0,readerMemory,code.Length);uint old;
            Need(VirtualProtect(readerMemory,new UIntPtr((uint)code.Length),0x20,out old)&&FlushInstructionCache(GetCurrentProcess(),readerMemory,new UIntPtr((uint)code.Length)),"FPU reader RX");Delegate reader=Marshal.GetDelegateForFunctionPointer(readerMemory,raw.GetNestedType("ReadFpu",BindingFlags.NonPublic));
            int[] channels={1,1,2,2},frames={8877,32768,32768,65536};string[] names={"main-footstep","main-water","main-water-stereo-pairs","seeded-wide-stereo"};
            string[] hashes={"3c3d422ab85c68cb8234bff926e89b13a31e885622e4a76f749b1e45bb3b7b80","951882eebd5b20395a3e8fdfa6ea3d50c5f7e049db9746ff07f3171e92d5fe77"};
            var records=new List<object>();int totalPackets=0,multiPacketCases=0,multiBlockCases=0;
            for(int kind=0;kind<4;kind++)foreach(bool streamed in new bool[]{false,true})
            {
                string name=names[kind]+(streamed?"-stream":"-ram"),directory=Path.Combine(args[2],"cases",name);Directory.CreateDirectory(directory);byte[] source;
                if(kind<3)source=Real(Path.Combine(args[1],"recovery/fixtures/audio/main/real-"+(kind==0?0:1)+".wav"),hashes[kind==0?0:1],channels[kind],frames[kind]);
                else{float[] f=new float[frames[kind]*2];uint seed=0x74119a25;for(int i=0;i<f.Length;i++){seed=unchecked(seed*1664525+1013904223);short value=unchecked((short)(seed>>16));f[i]=value*(1.0f/32768.0f);}source=new byte[f.Length*4];Buffer.BlockCopy(f,0,source,0,source.Length);}
                int block=streamed?2048:0x28000;object[] callArgs={module,encode,convert,free,control,reader,source,channels[kind],frames[kind],22050,block};
                object first=raw.GetMethod("Run",PrivateStatic).Invoke(null,callArgs);byte[][] output=(byte[][])Field(first,"Outputs");var files=new List<object>();
                files.Add(Save(directory,"source-float32le.bin",source));files.Add(Save(directory,"source-signed16le.bin",(byte[])Field(first,"Pcm")));files.Add(Save(directory,"descriptor-pointer-zero.bin",(byte[])Field(first,"Descriptor")));
                string[] outputs={"raw-packets.bin","allocation2.bin","allocation3.bin"};for(int i=0;i<3;i++)files.Add(Save(directory,outputs[i],output[i]));
                // Preserve the first output before any repeat assertion, so a
                // nondeterministic original would retain the evidence of it.
                object second=raw.GetMethod("Run",PrivateStatic).Invoke(null,callArgs);byte[][] repeated=(byte[][])Field(second,"Outputs");
                for(int i=0;i<3;i++){files.Add(Save(directory,"repeat-"+outputs[i],repeated[i]));Need(output[i].Length==repeated[i].Length&&Hash(output[i])==Hash(repeated[i]),"Original corpus nondeterminism "+name+" allocation "+i);}
                Need(Hash((byte[])Field(first,"Pcm"))==Hash((byte[])Field(second,"Pcm"))&&Hash((byte[])Field(first,"Descriptor"))==Hash((byte[])Field(second,"Descriptor")),"Repeat input differs");
                int packets=output[0].Length/2048,blocks=(output[0].Length+block-1)/block;Need(output[0].Length%2048==0,"Incomplete raw packet");totalPackets+=packets;if(packets>1)multiPacketCases++;if(blocks>1)multiBlockCases++;
                object record=new{name=name,channels=channels[kind],frames=frames[kind],sampleRate=22050,quality=75,flags=2,streamCount=1,blockBytes=block,rawBytes=output[0].Length,rawPackets=packets,rawBlocks=blocks,deterministic=true,nativeEncodeCalls=2,sourceKind=kind<3?"pinned-main-PCM8-fragment":"deterministic-seeded-signed16",mainSourceCommit=kind<3?"bacfc69fc3e71c8116d1f22b933b8b6173170ecf":null,mainFileSha256=kind<3?hashes[kind==0?0:1]:null,files=files,calls=new object[]{Field(first,"Evidence"),Field(second,"Evidence")}};
                File.WriteAllText(Path.Combine(directory,"case.json"),new JavaScriptSerializer().Serialize(record));records.Add(record);Console.WriteLine("PASS corpus "+name+" packets="+packets+" blocks="+blocks+" repeated twice exact");
            }
            Need(records.Count==8&&multiPacketCases>0&&multiBlockCases>0,"Long corpus did not exercise multiple packet/block outputs");
            File.WriteAllText(Path.Combine(args[2],"corpus.json"),new JavaScriptSerializer().Serialize(new{status="original-evidence-only",originalDllSha256="a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139",backend="real-original-windows-x86-CPU",caseCount=8,nativeEncodeCalls=16,totalPackets=totalPackets,multiPacketCases=multiPacketCases,multiBlockCases=multiBlockCases,xmaEncoderRecovered=false,sdkValidated=false,cases=records}));
            Console.WriteLine("PASS original XMA long corpus: 8 cases,16 encodes, packets="+totalPackets+", multiPacketCases="+multiPacketCases+", multiBlockCases="+multiBlockCases);return 0;
        }
        catch(Exception error){Console.Error.WriteLine(error);return 1;}
        finally{if(restore)control.DynamicInvoke(new object[]{saved,0x30300u});if(readerMemory!=IntPtr.Zero)VirtualFree(readerMemory,UIntPtr.Zero,0x8000);}
    }
}
