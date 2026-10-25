using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

internal static class AudioXmaPcmOracle
{
    const string Sha="a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139";
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode)] static extern IntPtr GetModuleHandle(string name);
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode)] static extern uint GetModuleFileName(IntPtr module,System.Text.StringBuilder path,int size);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate void Native(IntPtr output,IntPtr source,int channels,int stride,int frames);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate uint Control(uint value,uint mask);
    static IntPtr At(IntPtr p,int offset){return new IntPtr(unchecked(p.ToInt32()+offset));}
    static void Need(bool value,string message){if(!value)throw new Exception(message);}
    static byte[] Fill(int n){byte[] b=new byte[n];for(int i=0;i<n;i++)b[i]=0xa5;return b;}
    static void Equal(string label,byte[] a,byte[] b){Need(a.Length==b.Length,label+" length");for(int i=0;i<a.Length;i++)Need(a[i]==b[i],label+" byte "+i);}
    sealed class Guard:IDisposable
    {
        readonly IntPtr memory;readonly int length;public readonly IntPtr Pointer;
        public Guard(byte[] b){length=b.Length;memory=Marshal.AllocHGlobal(length+64);Pointer=At(memory,32);Marshal.Copy(Fill(length+64),0,memory,length+64);Marshal.Copy(b,0,Pointer,length);}
        public byte[] Bytes(){byte[] b=new byte[length];Marshal.Copy(Pointer,b,0,length);return b;}
        public void Dispose(){try{byte[] b=new byte[32];Marshal.Copy(memory,b,0,32);Equal("prefix",Fill(32),b);Marshal.Copy(At(Pointer,length),b,0,32);Equal("suffix",Fill(32),b);}finally{Marshal.FreeHGlobal(memory);}}
    }
    static IntPtr Load(string path)
    {
        path=Path.GetFullPath(path);Need(IntPtr.Size==4,"x86 required");
        using(SHA256 hash=SHA256.Create())Need(BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-","").ToLowerInvariant()==Sha,"DLL SHA");
        Assembly a=Assembly.LoadFrom(path);Need(String.Equals(Path.GetFullPath(a.Location),path,StringComparison.OrdinalIgnoreCase),"Managed DLL path");
        IntPtr m=GetModuleHandle(Path.GetFileName(path));var name=new System.Text.StringBuilder(32768);
        Need(m!=IntPtr.Zero&&GetModuleFileName(m,name,name.Capacity)!=0&&String.Equals(Path.GetFullPath(name.ToString()),path,StringComparison.OrdinalIgnoreCase),"Native DLL path");return m;
    }
    sealed class Case
    {public string Name;public int Offset,Channels,Stride,Frames;public byte[] Source;}
    static Case Make(string name,int[] bits,int offset,int channels,int stride,int frames)
    {byte[] b=new byte[bits.Length*4];Buffer.BlockCopy(bits,0,b,0,b.Length);return new Case{Name=name,Source=b,Offset=offset,Channels=channels,Stride=stride,Frames=frames};}
    static IEnumerable<Case> Cases()
    {
        // Exhaust all normalized signed16 samples, the actual SIMEX input domain.
        float[] pcm=new float[65536];for(int i=0;i<pcm.Length;i++)pcm[i]=(short)(i-32768)*(1.0f/32768.0f);
        int[] pb=new int[pcm.Length];Buffer.BlockCopy(pcm,0,pb,0,pb.Length*4);
        yield return Make("all-signed16-mono",pb,0,1,1,65536);
        yield return Make("all-signed16-stereo",pb,0,2,2,32768);
        int[] special={0,unchecked((int)0x80000000),1,-2147483647,0x007fffff,unchecked((int)0x807fffff),0x00800000,0x3f800000,unchecked((int)0xbf800000),0x40000000,unchecked((int)0xc0000000),0x477fffff,0x47800000,unchecked((int)0xc7800000),0x7f7fffff,unchecked((int)0xff7fffff),0x7f800000,unchecked((int)0xff800000),0x7fc00001,unchecked((int)0xffc00001),0x7f800001};
        yield return Make("all-special",special,0,1,1,special.Length);
        var bits=new List<int>(special);
        foreach(int n in new int[]{-65537,-32769,-32768,-3,-2,-1,0,1,2,3,32767,32768,65535})
        {
            float x=(float)((n+0.5)/32767.0);int v=BitConverter.ToInt32(BitConverter.GetBytes(x),0);
            for(int d=-3;d<=3;d++)bits.Add(unchecked(v+d));
        }
        uint state=0x12345678;for(int i=0;i<2048;i++){state=unchecked(state*1664525+1013904223);bits.Add(unchecked((int)state));}
        int[] source=bits.ToArray();
        foreach(int offset in new int[]{0,3})foreach(int channels in new int[]{1,2})foreach(int stride in new int[]{0,1,2,3,7})foreach(int frames in new int[]{0,1,2,7,31,257})
            yield return Make("stride-"+offset+"-"+channels+"-"+stride+"-"+frames,source,offset,channels,stride,frames);
        yield return Make("random-all",source,0,1,1,source.Length);
    }
    static byte[] Execute(Native native,Case c)
    {
        using(Guard input=new Guard(c.Source))using(Guard output=new Guard(Fill(c.Frames*c.Channels*2+32)))
        {
            native(output.Pointer,At(input.Pointer,c.Offset*4),c.Channels,c.Stride,c.Frames);
            Equal("source immutable",c.Source,input.Bytes());byte[] written=output.Bytes();byte[] tail=new byte[32];Array.Copy(written,written.Length-32,tail,0,32);Equal("untouched output tail",Fill(32),tail);
            byte[] result=new byte[written.Length-32];Array.Copy(written,result,result.Length);return result;
        }
    }
    static void Reject(MethodInfo method,object[] args,Type type)
    {try{method.Invoke(null,args);}catch(TargetInvocationException ex){Need(ex.InnerException.GetType()==type,"Unexpected rejection");return;}throw new Exception("Missing rejection");}
    static int Main(string[] args)
    {
        Control control=null;uint saved=0;bool restore=false;
        try
        {
            Need(args.Length==3,"original|candidate assembly fixture.tsv.gz");int records=0,samples=0;
            if(args[0]=="original")
            {
                IntPtr module=Load(args[1]);Native native=(Native)Marshal.GetDelegateForFunctionPointer(At(module,0xe9b0),typeof(Native));
                control=(Control)Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(At(module,0xab210)),typeof(Control));saved=control(0,0);restore=true;
                Need((control(0x10000,0x30300)&0x30300)==0x10000,"Native rounding/precision");
                using(FileStream file=File.Create(args[2]))using(GZipStream gz=new GZipStream(file,CompressionMode.Compress))using(StreamWriter writer=new StreamWriter(gz))
                {
                    writer.WriteLine("# originalDllSha256="+Sha+" nativeRva=0xe9b0 rounding=nearest-even");
                    foreach(Case c in Cases())
                    {
                        byte[] output=Execute(native,c);writer.WriteLine(c.Name+"\t"+c.Offset+"\t"+c.Channels+"\t"+c.Stride+"\t"+c.Frames+"\t"+Convert.ToBase64String(c.Source)+"\t"+Convert.ToBase64String(output));records++;samples+=output.Length/2;
                    }
                }
                control(saved,0x30300);restore=false;
            }
            else
            {
                Need(args[0]=="candidate","mode");Need(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.AudioCompiler.dll")),"Original DLL in candidate directory");
                Assembly assembly=Assembly.LoadFrom(Path.GetFullPath(args[1]));MethodInfo method=assembly.GetType("BinaryAssetBuilder.AudioCompiler.XmaPcmConverter",true).GetMethod("Convert");
                using(FileStream file=File.OpenRead(args[2]))using(GZipStream gz=new GZipStream(file,CompressionMode.Decompress))using(StreamReader reader=new StreamReader(gz))
                {
                    string header=reader.ReadLine();Need(header.Contains(Sha)&&header.Contains("nativeRva=0xe9b0"),"Fixture identity");string line;
                    while((line=reader.ReadLine())!=null)
                    {
                        string[] f=line.Split('\t');Need(f.Length==7,"Fixture columns");byte[] bytes=Convert.FromBase64String(f[5]);float[] source=new float[bytes.Length/4];Buffer.BlockCopy(bytes,0,source,0,bytes.Length);
                        short[] result=(short[])method.Invoke(null,new object[]{source,Int32.Parse(f[1]),Int32.Parse(f[2]),Int32.Parse(f[3]),Int32.Parse(f[4])});byte[] actual=new byte[result.Length*2];Buffer.BlockCopy(result,0,actual,0,actual.Length);Equal(f[0],Convert.FromBase64String(f[6]),actual);
                        byte[] unchanged=new byte[bytes.Length];Buffer.BlockCopy(source,0,unchanged,0,unchanged.Length);Equal("candidate source immutable",bytes,unchanged);records++;samples+=result.Length;
                    }
                }
                Reject(method,new object[]{null,0,1,1,1},typeof(ArgumentNullException));
                Reject(method,new object[]{new float[2],0,0,1,1},typeof(ArgumentOutOfRangeException));
                Reject(method,new object[]{new float[2],0,3,1,1},typeof(ArgumentOutOfRangeException));
                Reject(method,new object[]{new float[2],-1,1,1,1},typeof(ArgumentOutOfRangeException));
                Reject(method,new object[]{new float[2],3,1,1,1},typeof(ArgumentOutOfRangeException));
                Reject(method,new object[]{new float[2],0,1,-1,1},typeof(ArgumentOutOfRangeException));
                Reject(method,new object[]{new float[2],0,1,1,-1},typeof(ArgumentOutOfRangeException));
                Reject(method,new object[]{new float[2],0,2,2,Int32.MaxValue},typeof(ArgumentOutOfRangeException));
                Reject(method,new object[]{new float[2],0,2,2,2},typeof(ArgumentException));
                foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())Need(a.GetName().Name!="BinaryAssetBuilder.AudioCompiler","Original loaded in candidate");
                Console.WriteLine("PASS candidate safety refusals: 9");
            }
            Need(records==124,"Expected124 cases");Console.WriteLine("PASS XMA PCM preconversion: "+records+" cases, "+samples+" exact signed16 samples");return 0;
        }
        catch(Exception ex){Console.Error.WriteLine(ex);return 1;}
        finally{if(restore)control(saved,0x30300);}
    }
}
