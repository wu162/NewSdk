using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

// Complete production DDS API comparison, including the original 2D base-relative
// and volume recursive float mip contracts, with explicit opaque-block refusal.
internal static unsafe class TextureDxt1aDdsDifferential
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate int Writer(IntPtr data,uint size,IntPtr mip,IntPtr user);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate int Compressor(IntPtr image,IntPtr options,Writer writer,IntPtr rect);
    static Compressor original2D,originalVolume;
    static object plugin;
    static MethodInfo optionsMethod,encode2D,encodeVolume;
    static MemoryStream capture;
    static List<string> callbacks;
    static int checks,fixtures,matched,rejected;
    static void Check(bool ok,string message){checks++;if(!ok)throw new Exception(message);}
    static string Hash(byte[] bytes){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
    static bool Equal(byte[] a,byte[] b){if(a.Length!=b.Length)return false;for(int i=0;i<a.Length;i++)if(a[i]!=b[i])return false;return true;}
    static int First(byte[] a,byte[] b){for(int i=0;i<Math.Min(a.Length,b.Length);i++)if(a[i]!=b[i])return i;return a.Length==b.Length?-1:Math.Min(a.Length,b.Length);}
    static void Put(byte[] b,int offset,uint value){Buffer.BlockCopy(BitConverter.GetBytes(value),0,b,offset,4);}
    static int Write(IntPtr data,uint size,IntPtr mip,IntPtr user){
        byte[] bytes=new byte[checked((int)size)];Marshal.Copy(data,bytes,0,bytes.Length);capture.Write(bytes,0,bytes.Length);
        string info="null";if(mip!=IntPtr.Zero){int[] fields=new int[5];Marshal.Copy(mip,fields,0,5);info=String.Join(",",Array.ConvertAll(fields,delegate(int n){return n.ToString();}));}callbacks.Add(size+":"+info);return 0;
    }
    static byte[] Original(byte[] rgba,int width,int height,int depth,bool volume,bool mips){
        byte[] options=new byte[1644];uint* images=stackalloc uint[depth*6];
        fixed(byte* op=options)fixed(byte* pixels=rgba){
            optionsMethod.Invoke(plugin,new object[]{Pointer.Box(op,optionsMethod.GetParameters()[0].ParameterType),"D3DFMT_DXT1A",mips});
            for(int z=0;z<depth;z++){uint* image=images+z*6;image[0]=(uint)width;image[1]=(uint)height;image[2]=(uint)(pixels+z*width*height*4);image[3]=image[4]=(uint)(width*height);image[5]=0;}
            uint* level=stackalloc uint[3];level[0]=(uint)images;level[1]=level[2]=(uint)depth;
            uint* vol=stackalloc uint[3];vol[0]=(uint)level;vol[1]=vol[2]=1;
            if(volume)((uint*)op)[118]=2;
            capture=new MemoryStream();callbacks=new List<string>();Writer writer=new Writer(Write);
            int status=(volume?originalVolume:original2D)(new IntPtr(volume?vol:images),new IntPtr(op),writer,IntPtr.Zero);GC.KeepAlive(writer);
            Check(status==0,"original DDS status");byte[] result=capture.ToArray();capture.Dispose();return result;
        }
    }
    static byte[] Candidate(byte[] rgba,int width,int height,int depth,bool volume,bool mips){
        try{return(byte[])(volume?encodeVolume:encode2D).Invoke(null,volume?new object[]{(uint)width,(uint)height,(uint)depth,rgba,"D3DFMT_DXT1A",mips}:new object[]{(uint)width,(uint)height,rgba,"D3DFMT_DXT1A",mips});}
        catch(TargetInvocationException e){throw e.InnerException;}
    }
    static void CheckLayout(byte[] dds,int width,int height,int depth,bool volume,bool mips){
        Check(dds.Length>=136,"complete DDS minimum size");Check(BitConverter.ToUInt32(dds,0)==0x20534444&&BitConverter.ToUInt32(dds,4)==124,"DDS magic/header size");
        Check(BitConverter.ToUInt32(dds,8)==(volume?(mips?0x8a1007u:0x881007u):(mips?0xa1007u:0x81007u)),"DDS flags");
        Check(BitConverter.ToInt32(dds,12)==height&&BitConverter.ToInt32(dds,16)==width,"DDS dimensions");Check(BitConverter.ToInt32(dds,24)==(volume?depth:0),"DDS depth");
        Check(BitConverter.ToUInt32(dds,84)==0x31545844,"DDS DXT1 FourCC");Check(BitConverter.ToUInt32(dds,112)==(volume?0x200000u:0),"DDS caps2");
        int headerLevels=0,max=Math.Max(width,height);while(max!=0){headerLevels++;max>>=1;}Check(BitConverter.ToInt32(dds,28)==(mips?headerLevels:0),"DDS header XY mip count");
        Check(callbacks.Count>=3&&callbacks[0]=="4:null"&&callbacks[1]=="124:null","original header callbacks");
        int offset=128,frame=2,level=0,w=width,h=height,d=depth;
        do{
            int slice=((w+3)/4)*((h+3)/4)*8;
            for(int z=0;z<d;z++){Check(callbacks.Count>frame&&callbacks[frame]==slice+":"+level+","+w+","+h+","+z+","+d,"original payload callback");frame++;offset+=slice;}
            if(!mips||(w==1&&h==1&&d==1))break;w=Math.Max(1,w/2);h=Math.Max(1,h/2);d=Math.Max(1,d/2);level++;
        }while(true);
        Check(offset==dds.Length&&frame==callbacks.Count,"complete DDS/callback payload");
    }
    static byte[] Pixels(int width,int height,int depth,int pattern){
        byte[] bytes=new byte[width*height*depth*4];Random random=new Random(7919*pattern+width*31+height*17+depth);
        for(int z=0;z<depth;z++)for(int y=0;y<height;y++)for(int x=0;x<width;x++){
            int i=(z*height+y)*width+x;
            for(int c=0;c<3;c++)bytes[i*4+c]=(byte)(pattern<8?pattern*31:pattern<16?(c+1)*63:random.Next(256));
            switch(pattern%8){
                case 0:bytes[i*4+3]=0;break;
                case 1:bytes[i*4+3]=(byte)((x%4==0&&y%4==0)?255:0);break;
                case 2:bytes[i*4+3]=(byte)(((x+y+z)&1)==0?127:128);break;
                case 3:bytes[i*4+3]=(byte)(random.Next(2)==0?0:128);break;
                case 4:bytes[i*4+3]=(byte)((x%4==0&&y%4==0)?0:255);break;
                case 5:bytes[i*4+3]=127;break;
                case 6:bytes[i*4+3]=128;break;
                default:bytes[i*4+3]=(byte)random.Next(256);break;
            }
        }return bytes;
    }
    static void Isolated(){Check(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.TextureCompiler.dll")),"original absent");foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())Check(a.GetName().Name!="BinaryAssetBuilder.TextureCompiler","original not loaded");}
    static void Replay(string path){
        using(var r=new BinaryReader(File.OpenRead(path))){Check(r.ReadInt32()==0x32415844,"replay magic");int total=r.ReadInt32();
            for(int f=0;f<total;f++){
                Isolated();string label=r.ReadString();int w=r.ReadInt32(),h=r.ReadInt32(),d=r.ReadInt32();bool volume=r.ReadBoolean(),mips=r.ReadBoolean(),wasRejected=r.ReadBoolean();int n=r.ReadInt32();Check(n==w*h*d*4,"complete replay source");byte[] rgba=r.ReadBytes(n);Check(rgba.Length==n,"source truncation");string before=Hash(rgba);
                byte[] actual=null;bool nowRejected=false;try{actual=Candidate(rgba,w,h,d,volume,mips);}catch(NotSupportedException){nowRejected=true;}
                Check(nowRejected==wasRejected,"isolated rejection changed: "+label);Check(Hash(rgba)==before,"isolated source changed");
                if(wasRejected)rejected++;else{int size=r.ReadInt32();byte[] expected=r.ReadBytes(size),previous=r.ReadBytes(size);Check(expected.Length==size&&previous.Length==size,"complete replay DDS");Check(Equal(actual,previous),"isolated DDS changed: "+label);if(Equal(actual,expected))matched++;fixtures++;}Isolated();
            }Check(r.BaseStream.Position==r.BaseStream.Length,"replay trailing bytes");
        }Check(matched==fixtures,"complete isolated DDS equality");Console.WriteLine("PASS isolated fullDDS="+fixtures+" matched="+matched+" differences="+(fixtures-matched)+" opaqueBlocksRejected="+rejected+" checks="+checks);
    }
    static void Compare(BinaryWriter record,StreamWriter transcript,string label,int w,int h,int d,bool volume,bool mips,byte[] rgba){
        string before=Hash(rgba);byte[] actual=null;bool unsupported=false;
        try{actual=Candidate(rgba,w,h,d,volume,mips);}catch(NotSupportedException){unsupported=true;}
        record.Write(label);record.Write(w);record.Write(h);record.Write(d);record.Write(volume);record.Write(mips);record.Write(unsupported);record.Write(rgba.Length);record.Write(rgba);
        if(unsupported){rejected++;transcript.WriteLine(label+"\trejected-opaque-block\t\t\t");}
        else{
            byte[] expected=Original(rgba,w,h,d,volume,mips);CheckLayout(expected,w,h,d,volume,mips);if(Equal(actual,expected))matched++;fixtures++;
            record.Write(expected.Length);record.Write(expected);record.Write(actual);
            transcript.WriteLine(label+"\tcompared\t"+Hash(expected)+"\t"+Hash(actual)+"\t"+First(expected,actual));
        }
        Check(Hash(rgba)==before,"source mutated: "+label);
    }
    [DllImport("kernel32.dll",CharSet=CharSet.Ansi)] static extern IntPtr LoadLibrary(string path);
    static void LoadProduction(string path){Assembly a=Assembly.LoadFrom(Path.GetFullPath(path));encode2D=a.GetType("BinaryAssetBuilder.TextureCompiler.TextureDxtDdsEncoder",true).GetMethod("Encode");encodeVolume=a.GetType("BinaryAssetBuilder.TextureCompiler.TextureDxtVolumeDdsEncoder",true).GetMethod("Encode");Check(encode2D!=null&&encodeVolume!=null,"production complete DDS APIs");}
    static int Main(string[] args){IDisposable lifetime=null;try{
        Check(IntPtr.Size==4,"x86 required");Check(LoadLibrary(Path.GetFullPath(args[2]))!=IntPtr.Zero,"public S3 load");LoadProduction(args[3]);
        if(args[0]=="--replay"){Replay(args[1]);return 0;}
        string path=Path.GetFullPath(args[0]),dir=Path.GetDirectoryName(path);Check(Hash(File.ReadAllBytes(path))=="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e","original hash");
        AppDomain.CurrentDomain.AssemblyResolve+=delegate(object s,ResolveEventArgs e){string p=Path.Combine(dir,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
        Assembly original=Assembly.LoadFrom(path);Type type=original.GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true);plugin=Activator.CreateInstance(type);lifetime=(IDisposable)plugin;MethodInfo initialize=type.GetMethod("Initialize");initialize.Invoke(plugin,new object[]{null,Enum.ToObject(initialize.GetParameters()[1].ParameterType,0)});optionsMethod=type.GetMethod("GetNVCompressionOptions",BindingFlags.Instance|BindingFlags.NonPublic);
        IntPtr module=Marshal.GetHINSTANCE(original.ManifestModule);Check(module!=IntPtr.Zero&&module!=new IntPtr(-1),"original module");Check(original.ManifestModule.ResolveMethod(0x060000DC).Name=="nvDDS.nvDXTcompress"&&original.ManifestModule.ResolveMethod(0x060000DB).Name=="nvDDS.nvDXTcompress","original tokens");
        original2D=(Compressor)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(module,0x52b60),typeof(Compressor));originalVolume=(Compressor)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(module,0x52950),typeof(Compressor));
        using(var record=new BinaryWriter(File.Create(args[1]+".records")))using(var transcript=new StreamWriter(args[1])){
            record.Write(0x32415844);record.Write(0);transcript.WriteLine("label\tstatus\toriginalSha256\tcandidateSha256\tfirstDifference");
            foreach(int w in new[]{1,2,4,8,16})foreach(int h in new[]{1,2,4,8,16})foreach(bool mips in new[]{false,true})for(int p=0;p<32;p++)Compare(record,transcript,"2d-"+w+"x"+h+"-m"+mips+"-p"+p,w,h,1,false,mips,Pixels(w,h,1,p));
            foreach(int w in new[]{1,2,4,8})foreach(int h in new[]{1,2,4,8})foreach(int d in new[]{1,2,4,8})foreach(bool mips in new[]{false,true})for(int p=0;p<16;p++)Compare(record,transcript,"volume-"+w+"x"+h+"x"+d+"-m"+mips+"-p"+p,w,h,d,true,mips,Pixels(w,h,d,p+16));
            Check(fixtures+rejected==3648,"complete DDS input corpus");Check(fixtures>0&&rejected>0,"both compared and explicit opaque rejections");record.BaseStream.Position=4;record.Write(fixtures+rejected);
        }
        Check(matched==fixtures,"complete original DDS equality");Console.WriteLine("PASS DXT1A fullDDS="+fixtures+" matched="+matched+" differences="+(fixtures-matched)+" opaqueBlocksRejected="+rejected+" checks="+checks+"; production managed 2D/volume/mip DDS APIs, no SDK claim");return 0;
    }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{if(lifetime!=null)lifetime.Dispose();}}
}
