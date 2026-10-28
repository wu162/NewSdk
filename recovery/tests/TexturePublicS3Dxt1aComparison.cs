using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

// Experimental complete DXT1A wrapper. Never called by production APIs.
// Only blocks with at least one actual transparent source sample are accepted.
internal static unsafe class TexturePublicS3Dxt1aComparison
{
    [DllImport("kernel32.dll",CharSet=CharSet.Ansi)] static extern IntPtr LoadLibrary(string path);
    [DllImport("kernel32.dll",CharSet=CharSet.Ansi)] static extern IntPtr GetProcAddress(IntPtr module,string name);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate int Colour(IntPtr state,int length,int mode);
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)] delegate int FloatCompressor(IntPtr owner,IntPtr source,IntPtr header,IntPtr output,int format,uint size);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate int Writer(IntPtr data,uint size,IntPtr mip,IntPtr user);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate int ByteCompressor(IntPtr image,IntPtr options,Writer writer,IntPtr rect);
    static Colour candidate;
    static FloatCompressor originalFloat;
    static ByteCompressor originalBytes;
    static object plugin;
    static MethodInfo optionsMethod;
    static MemoryStream capture;
    static int checks,fixtures,matched,ddsFixtures,ddsMatched;
    static float threshold;
    static void Check(bool ok,string why){checks++;if(!ok)throw new Exception(why);}
    static string Hash(byte[] bytes){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
    static void Put(byte[] bytes,int offset,int value){Buffer.BlockCopy(BitConverter.GetBytes(value),0,bytes,offset,4);}
    static void PutDouble(byte[] bytes,int offset,double value){Buffer.BlockCopy(BitConverter.GetBytes(value),0,bytes,offset,8);}
    static int Write(IntPtr data,uint size,IntPtr mip,IntPtr user){byte[] bytes=new byte[checked((int)size)];Marshal.Copy(data,bytes,0,bytes.Length);capture.Write(bytes,0,bytes.Length);return 0;}
    static byte[] Options(){byte[] result=new byte[1644];fixed(byte* op=result)optionsMethod.Invoke(plugin,new object[]{Pointer.Box(op,optionsMethod.GetParameters()[0].ParameterType),"D3DFMT_DXT1A",false});return result;}
    static byte[] OriginalFloat(float[] rgba,int width,int height){
        byte[] options=Options(),owner=new byte[32],source=new byte[80],header=new byte[124],output=new byte[40];
        for(int i=0;i<output.Length;i++)output[i]=0xa5;
        uint[] rows=new uint[height],rowOwner=new uint[1];
        fixed(byte* op=options,own=owner,src=source,head=header,dst=output)
        fixed(float* pixels=rgba)fixed(uint* row=rows,ro=rowOwner){
            *(uint*)(own+0x18)=(uint)op;*(int*)(src+8)=height;*(int*)(src+12)=width;
            for(int y=0;y<height;y++)row[y]=(uint)(pixels+y*width*4);
            ro[0]=(uint)row;*(uint*)(src+0x24)=(uint)ro;
            Check(originalFloat(new IntPtr(own),new IntPtr(src),new IntPtr(head),new IntPtr(dst),1,8)==0,"original float status");
        }
        for(int i=8;i<output.Length;i++)Check(output[i]==0xa5,"float payload overrun");
        byte[] block=new byte[8];Buffer.BlockCopy(output,0,block,0,8);return block;
    }
    static byte[] OriginalDds(byte[] rgba,int width,int height){
        byte[] options=Options();fixed(byte* p=rgba)fixed(byte* op=options){
            uint* image=stackalloc uint[6];image[0]=(uint)width;image[1]=(uint)height;image[2]=(uint)p;image[3]=image[4]=(uint)(width*height);image[5]=0;
            capture=new MemoryStream();Writer writer=new Writer(Write);
            Check(originalBytes(new IntPtr(image),new IntPtr(op),writer,IntPtr.Zero)==0,"original DDS status");GC.KeepAlive(writer);
            byte[] result=capture.ToArray();capture.Dispose();Check(result.Length==136,"original complete DDS length");return result;
        }
    }
    static byte[] Candidate(float[] rgba,int width,int height){return TexturePublicS3Dxt1aExperimental.EncodeFloatBlock(rgba,width,height);}
    static byte[] Dds(byte[] block,int width,int height){
        byte[] result=new byte[136];Put(result,0,0x20534444);Put(result,4,124);Put(result,8,0x81007);Put(result,12,height);Put(result,16,width);Put(result,20,8);Put(result,76,32);Put(result,80,4);Put(result,84,0x31545844);Put(result,108,0x1000);Buffer.BlockCopy(block,0,result,128,8);return result;
    }
    static bool Equal(byte[] a,byte[] b){if(a.Length!=b.Length)return false;for(int i=0;i<a.Length;i++)if(a[i]!=b[i])return false;return true;}
    static int First(byte[] a,byte[] b){for(int i=0;i<Math.Min(a.Length,b.Length);i++)if(a[i]!=b[i])return i;return a.Length==b.Length?-1:Math.Min(a.Length,b.Length);}
    static void Isolated(){Check(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.TextureCompiler.dll")),"original absent");foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())Check(a.GetName().Name!="BinaryAssetBuilder.TextureCompiler","original not loaded");}
    static void Replay(string path){
        using(var r=new BinaryReader(File.OpenRead(path))){Check(r.ReadInt32()==0x33415844,"replay magic");threshold=r.ReadSingle();int total=r.ReadInt32();
            for(int f=0;f<total;f++){
                Isolated();string label=r.ReadString();int w=r.ReadInt32(),h=r.ReadInt32(),n=r.ReadInt32();Check(n==w*h*4,"complete replay input");float[] rgba=new float[n];for(int i=0;i<n;i++)rgba[i]=r.ReadSingle();
                byte[] expected=r.ReadBytes(8),previous=r.ReadBytes(8);Check(expected.Length==8&&previous.Length==8,"complete replay blocks");byte[] actual=Candidate(rgba,w,h);Check(Equal(actual,previous),"isolated candidate changed: "+label);if(Equal(actual,expected))matched++;fixtures++;
                bool dds=r.ReadBoolean();if(dds){int bytesLength=r.ReadInt32();byte[] source=r.ReadBytes(bytesLength);Check(bytesLength==n&&source.Length==bytesLength,"complete byte source");for(int i=0;i<n;i++)Check((float)(source[i]/255.0)==rgba[i],"byte/float replay input");byte[] e=r.ReadBytes(136),p=r.ReadBytes(136);Check(e.Length==136&&p.Length==136,"complete replay DDS");byte[] a=Dds(actual,w,h);Check(Equal(a,p),"isolated DDS changed");if(Equal(a,e))ddsMatched++;ddsFixtures++;}Isolated();
            }Check(r.BaseStream.Position==r.BaseStream.Length,"replay trailing bytes");
        }
        Console.WriteLine("COMPARISON isolated fixtures="+fixtures+" matched="+matched+" differences="+(fixtures-matched)+" completeDDS="+ddsFixtures+" ddsMatched="+ddsMatched+" checks="+checks);
    }
    static void Compare(BinaryWriter record,StreamWriter transcript,string label,int width,int height,float[] rgba,byte[] bytes){
        float[] before=(float[])rgba.Clone();byte[] bytesBefore=bytes==null?null:(byte[])bytes.Clone();
        byte[] expected=OriginalFloat(rgba,width,height),actual=Candidate(rgba,width,height);
        if(Equal(expected,actual))matched++;fixtures++;
        record.Write(label);record.Write(width);record.Write(height);record.Write(rgba.Length);foreach(float v in rgba)record.Write(v);record.Write(expected);record.Write(actual);record.Write(bytes!=null);
        transcript.WriteLine(label+"\tfloat\t"+width+"\t"+height+"\t"+Hash(expected)+"\t"+Hash(actual)+"\t"+First(expected,actual));
        if(bytes!=null){
            byte[] ddsExpected=OriginalDds(bytes,width,height),ddsActual=Dds(actual,width,height);ddsFixtures++;if(Equal(ddsExpected,ddsActual))ddsMatched++;
            Check(Equal(bytes,bytesBefore),"original bytes source mutated");record.Write(bytes.Length);record.Write(bytes);record.Write(ddsExpected);record.Write(ddsActual);
            transcript.WriteLine(label+"\tdds\t"+width+"\t"+height+"\t"+Hash(ddsExpected)+"\t"+Hash(ddsActual)+"\t"+First(ddsExpected,ddsActual));
        }
        for(int i=0;i<rgba.Length;i++)Check(rgba[i]==before[i],"original/candidate float source mutated");
    }
    static int Main(string[] args){IDisposable lifetime=null;try{
        Check(IntPtr.Size==4,"x86 required");TexturePublicS3Dxt1aExperimental.Initialize(args[2]);
        if(args[0]=="--replay"){Replay(args[1]);return 0;}
        string path=Path.GetFullPath(args[0]),dir=Path.GetDirectoryName(path);Check(Hash(File.ReadAllBytes(path))=="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e","original hash");
        AppDomain.CurrentDomain.AssemblyResolve+=delegate(object s,ResolveEventArgs e){string p=Path.Combine(dir,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
        Assembly original=Assembly.LoadFrom(path);Type type=original.GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true);plugin=Activator.CreateInstance(type);lifetime=(IDisposable)plugin;MethodInfo initialize=type.GetMethod("Initialize");initialize.Invoke(plugin,new object[]{null,Enum.ToObject(initialize.GetParameters()[1].ParameterType,0)});optionsMethod=type.GetMethod("GetNVCompressionOptions",BindingFlags.Instance|BindingFlags.NonPublic);
        IntPtr module=Marshal.GetHINSTANCE(original.ManifestModule);Check(module!=IntPtr.Zero&&module!=new IntPtr(-1),"original module");Check(original.ManifestModule.ResolveMethod(0x060000DC).Name=="nvDDS.nvDXTcompress","original token");
        originalFloat=(FloatCompressor)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(module,0x5c790),typeof(FloatCompressor));originalBytes=(ByteCompressor)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(module,0x52b60),typeof(ByteCompressor));threshold=BitConverter.ToSingle(Options(),0x74);Check(threshold==0.5f,"original default threshold");File.WriteAllBytes(args[1]+".options",Options());
        using(var record=new BinaryWriter(File.Create(args[1]+".records")))using(var transcript=new StreamWriter(args[1])){
            record.Write(0x33415844);record.Write(threshold);record.Write(0);transcript.WriteLine("label\tkind\twidth\theight\toriginalSha256\tcandidateSha256\tfirstDifference");
            foreach(int width in new[]{1,2,4})foreach(int height in new[]{1,2,4})for(int pattern=0;pattern<512;pattern++){
                int count=width*height;byte[] bytes=new byte[count*4];var random=new Random(7919*pattern+count*31+17);float[] rgba=new float[bytes.Length];
                for(int i=0;i<count;i++){
                    for(int c=0;c<3;c++)bytes[i*4+c]=(byte)(pattern<256?pattern:pattern<264?(c+1)*63:random.Next(256));
                    int alphaPattern=pattern%8;bytes[i*4+3]=(byte)(alphaPattern==0?0:alphaPattern==1?(i==0?0:255):alphaPattern==2?(i==count-1?127:128):alphaPattern==3?(i%2==0?127:128):alphaPattern==4?(i<count/2?255:0):random.Next(256));
                    for(int c=0;c<4;c++)rgba[i*4+c]=(float)(bytes[i*4+c]/255.0);
                }
                // Every tested original block must initialize the force-three flag.
                bytes[3]=0;rgba[3]=0;
                if(pattern>=384){
                    for(int i=0;i<count;i++)for(int c=0;c<3;c++)rgba[i*4+c]=(float)random.NextDouble();
                    float[] alphas={0,0.49999997f,0.5f,0.50000006f,1};for(int i=0;i<count;i++)rgba[i*4+3]=alphas[(i+pattern)%alphas.Length];rgba[3]=pattern%2==0?0.5f:0.49999997f;bytes=null;
                }
                Compare(record,transcript,"p"+pattern,width,height,rgba,bytes);
            }
            record.BaseStream.Position=8;record.Write(fixtures);
        }
        Check(fixtures==4608&&ddsFixtures==3456,"complete wrapper corpus");
        Console.WriteLine("COMPARISON DXT1A-transparent fixtures="+fixtures+" matched="+matched+" differences="+(fixtures-matched)+" completeDDS="+ddsFixtures+" ddsMatched="+ddsMatched+" checks="+checks+" threshold="+threshold+"; experimental, no production/SDK acceptance");return 0;
    }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{if(lifetime!=null)lifetime.Dispose();}}
}
