using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

internal static unsafe class TextureColourStateDifferential
{
    [DllImport("kernel32.dll",CharSet=CharSet.Ansi)] private static extern IntPtr LoadLibrary(string path);
    [DllImport("kernel32.dll",CharSet=CharSet.Ansi)] private static extern IntPtr GetProcAddress(IntPtr module,string name);
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)] private delegate void Core(IntPtr owner,IntPtr block,int mode);
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)] private delegate void Auto(IntPtr owner,IntPtr block);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int Recovered(IntPtr block,int length,int mode);
    private static int fixtures,checks;
    private static void Check(bool ok,string message){checks++;if(!ok)throw new Exception(message);}
    private static string Hash(byte[] bytes){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
    private static byte[] Block(int format,int flag,int count,int pattern)
    {
        byte[] result=new byte[0x3850];Random random=new Random(7919*pattern+count*31+17);
        fixed(byte* p=result) {
            int* header=(int*)p;header[0]=format;header[1]=flag;header[2]=count;
            double* rgb=(double*)(p+0x10);
            for(int i=0;i<count;i++)for(int c=0;c<3;c++) {
                double value;
                switch(pattern){
                    case 0:value=0;break;
                    case 1:value=1;break;
                    case 2:value=0.5;break;
                    case 3:value=(c+1)/4.0;break;
                    case 4:value=((i+c)&1);break;
                    case 5:value=(double)i/Math.Max(1,count-1);break;
                    case 6:value=(double)random.Next(256)/255;break;
                    default:value=random.NextDouble();break;
                }
                rgb[i*3+c]=(double)(float)value;
            }
            *(double*)(p+0x3010)=16.0;
            double* weights=(double*)(p+0x3018);
            weights[0]=(double)0.3086f;weights[1]=(double)0.6094f;weights[2]=(double)0.082f;
            if((pattern&1)!=0)weights[0]=weights[1]=weights[2]=1;
            *(int*)(p+0x3030)=71;
        }
        return result;
    }
    private static int Main(string[] args)
    {
        try {
            Check(IntPtr.Size==4,"x86 required");
            string originalPath=Path.GetFullPath(args[0]),dir=Path.GetDirectoryName(originalPath);
            Check(Hash(File.ReadAllBytes(originalPath))=="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e","Original hash");
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object s,ResolveEventArgs e){string p=Path.Combine(dir,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
            Assembly original=Assembly.LoadFrom(originalPath);IntPtr module=Marshal.GetHINSTANCE(original.ManifestModule);
            Check(module!=IntPtr.Zero&&module!=new IntPtr(-1),"Original module");
            Core core=(Core)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(module,0x7c420),typeof(Core));
            Auto auto=(Auto)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(module,0x83cd0),typeof(Auto));
            IntPtr owned=LoadLibrary(Path.GetFullPath(args[2]));Check(owned!=IntPtr.Zero,"Owned DLL load");
            IntPtr entry=GetProcAddress(owned,"TextureColorState");Check(entry!=IntPtr.Zero,"Owned export");
            Recovered recovered=(Recovered)Marshal.GetDelegateForFunctionPointer(entry,typeof(Recovered));
            byte[] owner=new byte[32],options=new byte[1644];
            using(var replay=new BinaryWriter(File.Create(args[1]+".replay")))
            using(var transcript=new StreamWriter(args[1])) {
                replay.Write(0);transcript.WriteLine("format\tflag\tmode\tcount\tpattern\tstateSha256");
                foreach(int format in new[]{0,1,2,3})foreach(int flag in new[]{0,1,2})foreach(int mode in new[]{0,3,4})
                foreach(int count in new[]{1,2,3,4,8,16,32})for(int pattern=0;pattern<8;pattern++) {
                    byte[] input=Block(format,flag,count,pattern),expected=(byte[])input.Clone(),actual=(byte[])input.Clone();
                    string label="format="+format+" flag="+flag+" mode="+mode+" count="+count+" pattern="+pattern;
                    fixed(byte* o=owner)fixed(byte* opt=options)fixed(byte* e=expected)fixed(byte* a=actual) {
                        *(int*)opt=71;*(IntPtr*)(o+0x18)=new IntPtr(opt);
                        if(mode==0)auto(new IntPtr(o),new IntPtr(e));else core(new IntPtr(o),new IntPtr(e),mode);
                        int status=recovered(new IntPtr(a),actual.Length,mode);Check(status==0,label+" owned status="+status);
                    }
                    replay.Write(mode);replay.Write(input);replay.Write(expected);
                    for(int i=0;i<expected.Length;i++)Check(expected[i]==actual[i],label+" mismatch offset=0x"+i.ToString("X")+" expected="+expected[i]+" actual="+actual[i]);
                    transcript.WriteLine(format+"\t"+flag+"\t"+mode+"\t"+count+"\t"+pattern+"\t"+Hash(actual));fixtures++;
                }
                // Upper storage bound without a costly unrelated high-cardinality exhaustive search.
                foreach(int count in new[]{128,256,512}) {
                    byte[] input=Block(0,0,count,2),expected=(byte[])input.Clone(),actual=(byte[])input.Clone();
                    fixed(byte* o=owner)fixed(byte* opt=options)fixed(byte* e=expected)fixed(byte* a=actual) {
                        *(int*)opt=71;*(IntPtr*)(o+0x18)=new IntPtr(opt);
                        auto(new IntPtr(o),new IntPtr(e));Check(recovered(new IntPtr(a),actual.Length,0)==0,"Capacity status");
                    }
                    for(int i=0;i<expected.Length;i++)Check(expected[i]==actual[i],"Capacity mismatch count="+count+" offset="+i);
                    replay.Write(0);replay.Write(input);replay.Write(expected);fixtures++;
                }
                byte[] invalid=Block(0,0,16,0);
                fixed(byte* p=invalid) {
                    Check(recovered(IntPtr.Zero,invalid.Length,0)!=0,"Null guard");
                    Check(recovered(new IntPtr(p),invalid.Length-1,0)!=0,"Size guard");
                    *(int*)(p+0x3030)=72;Check(recovered(new IntPtr(p),invalid.Length,0)!=0,"Quality72 rejection");
                    *(int*)(p+0x3030)=71;Check(recovered(new IntPtr(p),invalid.Length,2)!=0,"Mode rejection");
                }
                replay.BaseStream.Position=0;replay.Write(fixtures);
            }
            Console.WriteLine("PASS Texture Production colour state fixtures="+fixtures+" checks="+checks+"; complete initialized working blocks; not full DDS/plugin/SDK acceptance");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}
