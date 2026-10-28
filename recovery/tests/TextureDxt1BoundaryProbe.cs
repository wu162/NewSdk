using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

// Original-only state-boundary probe. A successful collection is not recovery
// acceptance. All files remain original outputs, including any unstable ones.
internal static unsafe class TextureDxt1BoundaryProbe
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int Writer(IntPtr data,uint size,IntPtr mip,IntPtr user);
    [DllImport("TextureDxt1BoundaryNative.dll",CallingConvention=CallingConvention.Cdecl)]
    private static extern int TextureBoundaryCompress(IntPtr image,IntPtr options,IntPtr compressor,Writer writer,uint fill);
    private static MemoryStream capture;
    private static List<string> callbacks;
    private static int checks,fixtures;
    private static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    private static string Hash(byte[] bytes){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
    private static int Write(IntPtr data,uint size,IntPtr mip,IntPtr user){
        byte[] bytes=new byte[checked((int)size)];Marshal.Copy(data,bytes,0,bytes.Length);capture.Write(bytes,0,bytes.Length);
        string info="null";
        if(mip!=IntPtr.Zero){int[] fields=new int[5];Marshal.Copy(mip,fields,0,5);info=String.Join(",",Array.ConvertAll(fields,delegate(int x){return x.ToString();}));}
        callbacks.Add(size+":"+info);return 0;
    }
    private static byte[] Pixels(int pattern,int alpha){
        byte[] result=new byte[64];Random random=new Random(7711+pattern*37);
        for(int i=0;i<16;i++){
            for(int c=0;c<3;c++){
                int value;
                switch(pattern){
                    case 0:value=0;break;case 1:value=255;break;case 2:value=128;break;
                    case 3:value=(c+1)*63;break;case 4:value=((i+c)&1)*255;break;
                    case 5:value=i*17;break;default:value=random.Next(256);break;
                }
                result[i*4+c]=(byte)value;
            }
            result[i*4+3]=(byte)(alpha==0?0:alpha==1?255:alpha==2?((i&1)==0?127:128):i*17);
        }
        return result;
    }
    private static int Main(string[] args){
        IDisposable owner=null;
        try{
            Check(IntPtr.Size==4,"x86 required");
            string path=Path.GetFullPath(args[0]),directory=Path.GetDirectoryName(path),output=Path.GetFullPath(args[1]);
            Check(Hash(File.ReadAllBytes(path))=="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e","Original hash");
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object s,ResolveEventArgs e){string p=Path.Combine(directory,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
            Assembly assembly=Assembly.LoadFrom(path);Type type=assembly.GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true);
            object plugin=Activator.CreateInstance(type);owner=(IDisposable)plugin;
            MethodInfo initialize=type.GetMethod("Initialize");initialize.Invoke(plugin,new object[]{null,Enum.ToObject(initialize.GetParameters()[1].ParameterType,0)});
            MethodInfo optionsMethod=type.GetMethod("GetNVCompressionOptions",BindingFlags.Instance|BindingFlags.NonPublic);
            Check(assembly.ManifestModule.ResolveMethod(0x060000DC).Name=="nvDDS.nvDXTcompress","Native token");
            IntPtr module=Marshal.GetHINSTANCE(assembly.ManifestModule);Check(module!=IntPtr.Zero&&module!=new IntPtr(-1),"Module base");
            IntPtr compress=IntPtr.Add(module,0x52b60);
            Directory.CreateDirectory(output);Directory.CreateDirectory(Path.Combine(output,"dds"));Directory.CreateDirectory(Path.Combine(output,"inputs"));
            var variations=new Dictionary<string,HashSet<string>>();
            using(var transcript=new StreamWriter(Path.Combine(output,"transcript.tsv"))){
                transcript.WriteLine("format\tpattern\talpha\tstackFill\trepeat\tbytes\tsha256\tinputSha256\toptionsSha256\tcallbacks");
                foreach(string format in new[]{"D3DFMT_DXT1","D3DFMT_DXT1A","D3DFMT_DXT3","D3DFMT_DXT5"})
                for(int pattern=0;pattern<8;pattern++)for(int alpha=0;alpha<4;alpha++){
                    byte[] pixels=Pixels(pattern,alpha),options=new byte[1644];string before=Hash(pixels),key=format+"-p"+pattern+"-a"+alpha;
                    File.WriteAllBytes(Path.Combine(output,"inputs",key+".rgba"),pixels);
                    fixed(byte* p=options)optionsMethod.Invoke(plugin,new object[]{Pointer.Box(p,optionsMethod.GetParameters()[0].ParameterType),format,false});
                    string optionsHash=Hash(options);File.WriteAllBytes(Path.Combine(output,"inputs",key+".options"),options);
                    variations[key]=new HashSet<string>();
                    foreach(uint fill in new uint[]{0,1,2,0x7fffffff,0xffffffff})for(int repeat=0;repeat<2;repeat++){
                        fixed(byte* rgba=pixels)fixed(byte* opt=options){
                            uint* image=stackalloc uint[6];image[0]=4;image[1]=4;image[2]=(uint)rgba;image[3]=image[4]=16;image[5]=0;
                            capture=new MemoryStream();callbacks=new List<string>();Writer writer=new Writer(Write);
                            int status=TextureBoundaryCompress(new IntPtr(image),new IntPtr(opt),compress,writer,fill);GC.KeepAlive(writer);
                            Check(status==0,key+" original status="+status);
                            byte[] dds=capture.ToArray();capture.Dispose();
                            Check(dds.Length==(format=="D3DFMT_DXT1"||format=="D3DFMT_DXT1A"?136:144),key+" DDS length");
                            Check(dds[0]==68&&dds[1]==68&&dds[2]==83&&dds[3]==32,key+" DDS magic");
                            Check(Hash(pixels)==before,key+" original source changed");Check(Hash(options)==optionsHash,key+" original options changed");
                            string hash=Hash(dds);variations[key].Add(hash);
                            string label=key+"-f"+fill.ToString("x8")+"-r"+repeat;
                            File.WriteAllBytes(Path.Combine(output,"dds",label+".dds"),dds);
                            transcript.WriteLine(format+"\t"+pattern+"\t"+alpha+"\t"+fill.ToString("x8")+"\t"+repeat+"\t"+dds.Length+"\t"+hash+"\t"+before+"\t"+optionsHash+"\t"+String.Join(";",callbacks));
                            fixtures++;
                        }
                    }
                }
            }
            int unstable=0;
            using(var summary=new StreamWriter(Path.Combine(output,"variations.tsv"))){
                summary.WriteLine("fixture\tdistinctOutputs");
                foreach(var pair in variations){summary.WriteLine(pair.Key+"\t"+pair.Value.Count);if(pair.Value.Count>1)unstable++;
                    if(pair.Key.StartsWith("D3DFMT_DXT3-")||pair.Key.StartsWith("D3DFMT_DXT5-"))Check(pair.Value.Count==1,"Initialized DXT3/5 control must be stack independent: "+pair.Key);
                }
            }
            Check(fixtures==1280,"Full boundary corpus count");
            Console.WriteLine("PROBE Texture DXT1 stack boundary fixtures="+fixtures+" checks="+checks+" varyingInputs="+unstable+"; original-only; no production acceptance");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{if(owner!=null)owner.Dispose();}
    }
}
