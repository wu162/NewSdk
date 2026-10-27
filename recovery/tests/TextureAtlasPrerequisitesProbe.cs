using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
// Original-only native DDS evidence for the PC atlas's exact options overrides.
// Does not call the recovered encoder and does not claim full atlas acceptance.
internal static unsafe class TextureAtlasPrerequisitesProbe
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]delegate int Writer(IntPtr data,uint size,IntPtr mip,IntPtr user);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]delegate int Compressor(IntPtr image,IntPtr options,Writer writer,IntPtr rect);
    static MemoryStream capture;static List<string> callbacks;static int fixtures,checks;
    static void Check(bool v,string s){checks++;if(!v)throw new Exception(s);}
    static string Hash(byte[] b){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(b)).Replace("-","").ToLowerInvariant();}
    static int Write(IntPtr data,uint size,IntPtr mip,IntPtr user){byte[] b=new byte[checked((int)size)];if(b.Length!=0)Marshal.Copy(data,b,0,b.Length);capture.Write(b,0,b.Length);string fields="null";if(mip!=IntPtr.Zero){int[] f=new int[5];Marshal.Copy(mip,f,0,5);fields=String.Join(",",Array.ConvertAll(f,x=>x.ToString()));}callbacks.Add(size+":"+fields);return 0;}
    static void Dump(IntPtr module,string output,int rva,int length)
    {
        byte[] data=new byte[length];Marshal.Copy(IntPtr.Add(module,rva),data,0,length);File.WriteAllBytes(Path.Combine(output,"native-"+rva.ToString("x")+".bin"),data);
        using(var w=new StreamWriter(Path.Combine(output,"native-"+rva.ToString("x")+".tsv"))){w.WriteLine("rva\tbits\tfloat32\tdouble64\tpointerRva");for(int i=0;i<length;i+=4){uint bits=BitConverter.ToUInt32(data,i);w.WriteLine((rva+i).ToString("x")+"\t"+bits.ToString("X8")+"\t"+BitConverter.ToSingle(data,i).ToString("R",System.Globalization.CultureInfo.InvariantCulture)+"\t"+(i%8==0?BitConverter.ToDouble(data,i).ToString("R",System.Globalization.CultureInfo.InvariantCulture):"")+"\t"+(bits>=(uint)module.ToInt32()&&bits<(uint)module.ToInt32()+0xc0000?(bits-(uint)module.ToInt32()).ToString("x"):""));}}
    }
    static byte[] Pixels(int width,int height,int pattern)
    {
        byte[] b=new byte[width*height*4];uint state=0x163ad5u;
        for(int y=0;y<height;y++)for(int x=0;x<width;x++)for(int c=0;c<4;c++){
            state=unchecked(state*1664525u+1013904223u);int v=pattern==0?0:pattern==1?255:pattern==2?(x*37+y*71+c*113+17)&255:pattern==3?((x+y+c)&1)*255:pattern==4?((x==width/2&&y==height/2)?255:0):pattern==5?(c==2?255:128):pattern==6?(c==0?x*255/Math.Max(1,width-1):c==1?y*255/Math.Max(1,height-1):c==2?128:255):(int)(state>>24);b[(y*width+x)*4+c]=(byte)v;
        }return b;
    }
    static byte[] Compress(Compressor compress,object plugin,MethodInfo optionsMethod,int width,int height,byte[] pixels,int format,int filter,out byte[] options,out string calls)
    {
        options=new byte[1644];fixed(byte* op=options)fixed(byte* rgba=pixels){
            optionsMethod.Invoke(plugin,new object[]{Pointer.Box(op,optionsMethod.GetParameters()[0].ParameterType),"D3DFMT_A8R8G8B8",true});
            *(int*)(op+476)=format;*(int*)(op+104)=filter;
            uint* image=stackalloc uint[6];image[0]=(uint)width;image[1]=(uint)height;image[2]=(uint)rgba;image[3]=(uint)(width*height);image[4]=image[3];image[5]=0;
            capture=new MemoryStream();callbacks=new List<string>();Writer writer=Write;int status=compress((IntPtr)image,(IntPtr)op,writer,IntPtr.Zero);GC.KeepAlive(writer);Check(status==0,"Native compression result "+status);byte[] result=capture.ToArray();capture.Dispose();calls=String.Join(";",callbacks.ToArray());Check(result.Length>=128&&BitConverter.ToUInt32(result,0)==0x20534444u,"DDS header");return result;
        }
    }
    static int Main(string[] args)
    {
        IDisposable owner=null;
        try{
            Check(IntPtr.Size==4,"x86 required");string input=Path.GetFullPath(args[0]),directory=Path.GetDirectoryName(input),output=Path.GetFullPath(args[1]);Directory.CreateDirectory(output);
            Check(Hash(File.ReadAllBytes(input))=="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e","Original hash");
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string path=Path.Combine(directory,new AssemblyName(e.Name).Name+".dll");return File.Exists(path)?Assembly.LoadFrom(path):null;};
            var original=Assembly.LoadFrom(input);Type type=original.GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true);object plugin=Activator.CreateInstance(type);owner=(IDisposable)plugin;var init=type.GetMethod("Initialize");init.Invoke(plugin,new object[]{null,Enum.ToObject(init.GetParameters()[1].ParameterType,0)});
            var optionsMethod=type.GetMethod("GetNVCompressionOptions",BindingFlags.Instance|BindingFlags.NonPublic);Check(original.ManifestModule.ResolveMethod(0x060000dc).Name=="nvDDS.nvDXTcompress","Compressor token");IntPtr module=Marshal.GetHINSTANCE(original.ManifestModule);Check(module!=IntPtr.Zero&&module!=new IntPtr(-1)&&Marshal.ReadByte(module,0x52b60)==0x6a,"Native compressor signature");var compress=(Compressor)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(module,0x52b60),typeof(Compressor));
            int[][] sizes={new[]{1,1},new[]{1,8},new[]{8,1},new[]{2,8},new[]{8,2},new[]{4,4},new[]{8,8},new[]{8,16},new[]{16,8},new[]{32,16}};
            using(var transcript=new StreamWriter(Path.Combine(output,"transcript.tsv"))){
                transcript.WriteLine("label\twidth\theight\tpattern\tformatOption\tfilterOption\tbytes\tfourCC\tflags\tpitchOrLinearSize\tmipCount\tpixelFormatFlags\tbitCount\tredMask\tgreenMask\tblueMask\talphaMask\tsha256\tcallbacks");
                foreach(int[] size in sizes)for(int pattern=0;pattern<8;pattern++)foreach(int format in new[]{3,7,9})foreach(int filter in new[]{0,5}){
                    int w=size[0],h=size[1];string label=w+"x"+h+"-p"+pattern+"-format"+format+"-filter"+filter;byte[] pixels=Pixels(w,h,pattern),before=(byte[])pixels.Clone(),options,againOptions;string calls,againCalls;
                    byte[] dds=Compress(compress,plugin,optionsMethod,w,h,pixels,format,filter,out options,out calls),again=Compress(compress,plugin,optionsMethod,w,h,pixels,format,filter,out againOptions,out againCalls);
                    Check(Hash(pixels)==Hash(before),"Source mutation "+label);Check(Hash(dds)==Hash(again)&&calls==againCalls&&Hash(options)==Hash(againOptions),"Repeated determinism "+label);
                    File.WriteAllBytes(Path.Combine(output,label+".rgba"),pixels);File.WriteAllBytes(Path.Combine(output,label+".dds"),dds);File.WriteAllBytes(Path.Combine(output,label+".options.bin"),options);
                    transcript.WriteLine(label+"\t"+w+"\t"+h+"\t"+pattern+"\t"+format+"\t"+filter+"\t"+dds.Length+"\t"+BitConverter.ToUInt32(dds,84).ToString("X8")+"\t"+BitConverter.ToUInt32(dds,8).ToString("X8")+"\t"+BitConverter.ToUInt32(dds,20)+"\t"+BitConverter.ToUInt32(dds,28)+"\t"+BitConverter.ToUInt32(dds,80).ToString("X8")+"\t"+BitConverter.ToUInt32(dds,88)+"\t"+BitConverter.ToUInt32(dds,92).ToString("X8")+"\t"+BitConverter.ToUInt32(dds,96).ToString("X8")+"\t"+BitConverter.ToUInt32(dds,100).ToString("X8")+"\t"+BitConverter.ToUInt32(dds,104).ToString("X8")+"\t"+Hash(dds)+"\t"+calls);fixtures++;
                }
            }
            Dump(module,output,0x56708,0x80);Dump(module,output,0xa2a00,0x900);
            Check(fixtures==480,"Full PC atlas prerequisite corpus");Console.WriteLine("PROBE Texture PC atlas prerequisites fixtures="+fixtures+" nativeCalls="+(fixtures*2)+" checks="+checks+"; exact format3/9 and filter5 overrides plus format7/filter0 controls; full DDS/options/callbacks and repeat determinism; original-only, no full atlas/recovered acceptance");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{if(owner!=null)owner.Dispose();}
    }
}
