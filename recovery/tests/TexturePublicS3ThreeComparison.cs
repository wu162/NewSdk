using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

internal static unsafe class TexturePublicS3ThreeComparison
{
    [DllImport("kernel32.dll",CharSet=CharSet.Ansi)] private static extern IntPtr LoadLibrary(string path);
    [DllImport("kernel32.dll",CharSet=CharSet.Ansi)] private static extern IntPtr GetProcAddress(IntPtr module,string name);
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)] private delegate void Core(IntPtr owner,IntPtr block,int mode);
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)] private delegate void Auto(IntPtr owner,IntPtr block);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int Recovered(IntPtr block,int length,int mode);
    private static int fixtures,checks,matched;
    private static Core originalCore;
    private static Auto originalAuto;
    private static Recovered candidate;
    private static byte[] owner=new byte[32],options=new byte[1644];
    private static BinaryWriter records;
    private static StreamWriter transcript;
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

    private static void Compare(string label,int mode,byte[] input) {
        byte[] expected=(byte[])input.Clone(),actual=(byte[])input.Clone();
        fixed(byte* o=owner)fixed(byte* opt=options)fixed(byte* e=expected)fixed(byte* a=actual){
            *(int*)opt=71;*(IntPtr*)(o+0x18)=new IntPtr(opt);
            if(mode==0)originalAuto(new IntPtr(o),new IntPtr(e));else originalCore(new IntPtr(o),new IntPtr(e),3);
            Check(candidate(new IntPtr(a),actual.Length,mode)==0,label+" candidate status");
        }
        int first=-1;for(int i=0;i<expected.Length;i++)if(expected[i]!=actual[i]){first=i;break;}
        if(first<0)matched++;
        transcript.WriteLine(label+"\t"+mode+"\t"+Hash(input)+"\t"+Hash(expected)+"\t"+Hash(actual)+"\t"+(first<0?"":first.ToString("x")));
        records.Write(mode);records.Write(input);records.Write(expected);records.Write(actual);fixtures++;
    }
    private static void LoadCandidate(string path){
        IntPtr library=LoadLibrary(Path.GetFullPath(path));Check(library!=IntPtr.Zero,"Candidate native load");
        IntPtr entry=GetProcAddress(library,"TexturePublicS3ThreeColour");Check(entry!=IntPtr.Zero,"Candidate three-colour export");
        candidate=(Recovered)Marshal.GetDelegateForFunctionPointer(entry,typeof(Recovered));
    }
    private static void AssertIsolated(){
        Check(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.TextureCompiler.dll")),"Original absent");
        foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())Check(a.GetName().Name!="BinaryAssetBuilder.TextureCompiler","Original not loaded");
    }
    private static void Replay(string path){
        using(var reader=new BinaryReader(File.OpenRead(path))){
            int count=reader.ReadInt32();
            for(int j=0;j<count;j++){
                AssertIsolated();int mode=reader.ReadInt32();
                byte[] input=reader.ReadBytes(0x3850),expected=reader.ReadBytes(0x3850),previous=reader.ReadBytes(0x3850);
                Check(input.Length==0x3850&&expected.Length==0x3850&&previous.Length==0x3850,"Complete replay record");
                fixed(byte* p=input)Check(candidate(new IntPtr(p),input.Length,mode)==0,"Isolated candidate status");
                bool same=true;for(int i=0;i<input.Length;i++){Check(input[i]==previous[i],"Candidate changed in isolated replay");same&=input[i]==expected[i];}
                if(same)matched++;fixtures++;AssertIsolated();
            }
            Check(reader.BaseStream.Position==reader.BaseStream.Length,"Replay trailing bytes");
        }
        Console.WriteLine("COMPARISON isolated fixtures="+fixtures+" matched="+matched+" differences="+(fixtures-matched)+" checks="+checks);
    }
    private static int Main(string[] args){
        try{
            Check(IntPtr.Size==4,"x86 required");LoadCandidate(args[2]);
            if(args[0]=="--replay"){Replay(args[1]);return 0;}
            string originalPath=Path.GetFullPath(args[0]),dir=Path.GetDirectoryName(originalPath);
            Check(Hash(File.ReadAllBytes(originalPath))=="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e","Original hash");
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object s,ResolveEventArgs e){string p=Path.Combine(dir,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
            Assembly original=Assembly.LoadFrom(originalPath);IntPtr module=Marshal.GetHINSTANCE(original.ManifestModule);
            Check(module!=IntPtr.Zero&&module!=new IntPtr(-1),"Original module");
            originalCore=(Core)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(module,0x7c420),typeof(Core));
            originalAuto=(Auto)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(module,0x83cd0),typeof(Auto));
            using(records=new BinaryWriter(File.Create(args[1]+".records")))
            using(transcript=new StreamWriter(args[1])){
                records.Write(0);transcript.WriteLine("label\tmode\tinputSha256\toriginalSha256\tcandidateSha256\tfirstDifference");
                foreach(int format in new[]{0,1})foreach(int mode in new[]{0,3}){
                    foreach(int count in new[]{1,2,3,4,8,16})for(int pattern=0;pattern<8;pattern++)
                        Compare("f"+format+"-n"+count+"-p"+pattern,mode,Block(format,0,count,pattern));
                    for(int gray=0;gray<256;gray++){
                        byte[] block=Block(format,0,16,0);
                        fixed(byte* b=block){for(int i=0;i<48;i++)((double*)(b+0x10))[i]=(double)(float)(gray/255.0);}
                        Compare("f"+format+"-gray"+gray,mode,block);
                    }
                    for(int seed=0;seed<1024;seed++)
                        Compare("f"+format+"-random"+seed,mode,Block(format,0,16,seed+8));
                }
                records.BaseStream.Position=0;records.Write(fixtures);
            }
            Check(fixtures==5312,"Complete initialized-state comparison corpus");
            Console.WriteLine("COMPARISON explicit-three fixtures="+fixtures+" matched="+matched+" differences="+(fixtures-matched)+" checks="+checks+"; not DDS/plugin/SDK acceptance");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}
