using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

// Original D3DX format factory/vtables only. Caller owns data/descriptor/sample
// storage; converter objects and their internal buffers use D3DX's process heap.
internal static class TextureNpotFormatProbe
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]delegate IntPtr Factory(IntPtr descriptor);
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]delegate void Row(IntPtr self,int y,int z,IntPtr samples);
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]delegate void Delete(IntPtr self,int flags);
    static int checks,cases;
    static void Check(bool v,string s){checks++;if(!v)throw new Exception(s);}
    static void Put(IntPtr p,int offset,int v){Marshal.WriteInt32(p,offset,v);}
    static byte[] Read(IntPtr p,int count){byte[] result=new byte[count];Marshal.Copy(p,result,0,count);return result;}
    static void Dump(IntPtr module,string output,int rva,int size)
    {
        byte[] b=Read(IntPtr.Add(module,rva),size);File.WriteAllBytes(Path.Combine(output,"constants-"+rva.ToString("x")+".bin"),b);
        using(var w=new StreamWriter(Path.Combine(output,"constants-"+rva.ToString("x")+".tsv"))){w.WriteLine("rva\tbits\tfloat32\tdouble64");for(int i=0;i<size;i+=4)w.WriteLine((rva+i).ToString("x")+"\t"+BitConverter.ToUInt32(b,i).ToString("X8")+"\t"+BitConverter.ToSingle(b,i).ToString("R",System.Globalization.CultureInfo.InvariantCulture)+"\t"+(i%8==0?BitConverter.ToDouble(b,i).ToString("R",System.Globalization.CultureInfo.InvariantCulture):""));}
    }
    static void Case(Factory factory,IntPtr module,StreamWriter transcript,string directory,uint format,int bytesPerPixel,int dither)
    {
        const int width=4,height=4;int rowBytes=width*bytesPerPixel;byte[] input=new byte[rowBytes*height];
        for(int i=0;i<input.Length;i++)input[i]=(byte)(i*37+11);
        IntPtr data=Marshal.AllocHGlobal(input.Length),descriptor=Marshal.AllocHGlobal(0x50),samples=Marshal.AllocHGlobal(width*16),converter=IntPtr.Zero;
        string label=format.ToString("X8")+"-dither"+dither;
        try{
            Marshal.Copy(input,0,data,input.Length);for(int i=0;i<0x50;i++)Marshal.WriteByte(descriptor,i,0);
            Put(descriptor,0,data.ToInt32());Put(descriptor,4,unchecked((int)format));Put(descriptor,8,rowBytes);Put(descriptor,12,rowBytes*height);
            Put(descriptor,0x30,width);Put(descriptor,0x34,height);Put(descriptor,0x3c,1);Put(descriptor,0x40,dither==0?0:0x80000);
            converter=factory(descriptor);Check(converter!=IntPtr.Zero,"Converter factory "+label);
            Check(Marshal.ReadInt32(converter,0x68)==width&&Marshal.ReadInt32(converter,0x6c)==height&&Marshal.ReadInt32(converter,0x70)==1,"Converter extents "+label);
            Check(Marshal.ReadInt32(converter,0x78)==bytesPerPixel,"Converter byte size "+label);
            IntPtr vtable=Marshal.ReadIntPtr(converter),getAddress=Marshal.ReadIntPtr(vtable,4),setAddress=Marshal.ReadIntPtr(vtable,8);
            Row getter=(Row)Marshal.GetDelegateForFunctionPointer(getAddress,typeof(Row)),setter=(Row)Marshal.GetDelegateForFunctionPointer(setAddress,typeof(Row));
            getter(converter,0,0,samples);byte[] decoded=Read(samples,width*16);File.WriteAllBytes(Path.Combine(directory,label+".getter.float"),decoded);
            string bits=String.Join(",",Array.ConvertAll(ToUInt32(decoded),x=>x.ToString("X8")));
            transcript.WriteLine(label+"\tgetter\t0\t"+(getAddress.ToInt32()-module.ToInt32()).ToString("x")+"\t"+(setAddress.ToInt32()-module.ToInt32()).ToString("x")+"\t"+BitConverter.ToString(input,0,rowBytes).Replace("-","")+"\t"+bits);
            float[] values={0f,0f,0f,0f, 1f,1f,1f,1f, .5f,.25f,.75f,.5f, .12345679f,.9876543f,.001f,.49999997f};byte[] source=new byte[width*16];Buffer.BlockCopy(values,0,source,0,source.Length);
            for(int y=0;y<2;y++){
                for(int i=0;i<input.Length;i++)Marshal.WriteByte(data,i,0xa5);Marshal.Copy(source,0,samples,source.Length);setter(converter,y,0,samples);byte[] encoded=Read(IntPtr.Add(data,y*rowBytes),rowBytes);File.WriteAllBytes(Path.Combine(directory,label+".setter-y"+y+".bin"),encoded);
                transcript.WriteLine(label+"\tsetter\t"+y+"\t\t\t"+BitConverter.ToString(encoded).Replace("-","")+"\t"+String.Join(",",Array.ConvertAll(ToUInt32(source),x=>x.ToString("X8"))));
            }cases++;
        }finally{
            if(converter!=IntPtr.Zero){IntPtr vtable=Marshal.ReadIntPtr(converter);var destroy=(Delete)Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(vtable),typeof(Delete));destroy(converter,1);}
            Marshal.FreeHGlobal(samples);Marshal.FreeHGlobal(descriptor);Marshal.FreeHGlobal(data);
        }
    }
    static uint[] ToUInt32(byte[] bytes){uint[] result=new uint[bytes.Length/4];Buffer.BlockCopy(bytes,0,result,0,bytes.Length);return result;}
    static int Main(string[] args)
    {
        IDisposable owner=null;
        try{
            Check(IntPtr.Size==4,"x86 required");string input=Path.GetFullPath(args[0]),directory=Path.GetDirectoryName(input),output=Path.GetFullPath(args[1]);Directory.CreateDirectory(output);
            using(var sha=SHA256.Create())Check(BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(input))).Replace("-","").ToLowerInvariant()=="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e","Original hash");
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string p=Path.Combine(directory,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
            var assembly=Assembly.LoadFrom(input);Type type=assembly.GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true);object plugin=Activator.CreateInstance(type);owner=(IDisposable)plugin;MethodInfo init=type.GetMethod("Initialize");init.Invoke(plugin,new object[]{null,Enum.ToObject(init.GetParameters()[1].ParameterType,0)});
            IntPtr module=Marshal.GetHINSTANCE(assembly.ManifestModule);Check(module!=IntPtr.Zero&&module!=new IntPtr(-1),"Module base");Check(Marshal.ReadByte(IntPtr.Add(module,0x232b9))==0x56,"Format factory prologue");
            var factory=(Factory)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(module,0x232b9),typeof(Factory));
            uint[] formats={0x28000002u,0x0800004au,0x28000058u,0x28280043u,0x18280043u,0x18280086u,0x28280086u,0x1a20005au};int[] strides={1,2,2,2,2,4,4,8};
            using(var transcript=new StreamWriter(Path.Combine(output,"transcript.tsv"))){transcript.WriteLine("label\tstage\ty\tgetterRva\tsetterRva\tstorageHex\tfloatBits");for(int i=0;i<formats.Length;i++)foreach(int dither in new[]{0,1})Case(factory,module,transcript,output,formats[i],strides[i],dither);}
            Dump(module,output,0x98000,0x400);Dump(module,output,0x99a00,0x600);
            Check(cases==16,"Format case count");Console.WriteLine("PROBE Texture NPOT D3DX format cases="+cases+" checks="+checks+"; getter float bits/setter stored bytes with dither and row parity; original-only, no recovered acceptance");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{if(owner!=null)owner.Dispose();}
    }
}
