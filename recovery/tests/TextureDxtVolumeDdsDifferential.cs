using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

// Calls the immutable original RGBAImage nvDDS::nvDXTcompress native body.
// Inspection mode: original.dll transcript.txt. Differential mode adds recovered.dll.
internal static unsafe class TextureDxtVolumeDdsDifferential
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int Writer(IntPtr data, uint size, IntPtr mip, IntPtr user);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int Compressor(IntPtr image, IntPtr options, Writer writer, IntPtr rect);
    private static MemoryStream capture;
    private static List<string> calls;
    private static int checks, fixtures;
    private static void Check(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    private static int Write(IntPtr data, uint size, IntPtr mip, IntPtr user)
    {
        byte[] bytes = new byte[checked((int)size)];
        if (size != 0) Marshal.Copy(data, bytes, 0, bytes.Length);
        capture.Write(bytes, 0, bytes.Length);
        string info = "null";
        if (mip != IntPtr.Zero)
        {
            int[] fields = new int[5]; Marshal.Copy(mip, fields, 0, 5);
            info = String.Join(",", Array.ConvertAll(fields, delegate(int n) { return n.ToString(); }));
        }
        calls.Add(size.ToString() + ":" + info);
        return 0;
    }
    private static string Hash(byte[] bytes)
    {
        using (SHA256 sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
    }
    private static byte[] Pixels(uint width, uint height, int pattern)
    {
        byte[] result = new byte[checked((int)(width * height * 4))];
        for (int i = 0; i < result.Length / 4; i++)
        {
            switch (pattern)
            {
                case 0: result[i*4]=0;result[i*4+1]=0;result[i*4+2]=0;result[i*4+3]=0;break;
                case 1: result[i*4]=255;result[i*4+1]=255;result[i*4+2]=255;result[i*4+3]=255;break;
                case 2: result[i*4]=(byte)(i*37+17);result[i*4+1]=(byte)(i*71+33);result[i*4+2]=(byte)(i*113+129);result[i*4+3]=(byte)(i*19+7);break;
                case 3: result[i*4]=255;result[i*4+1]=1;result[i*4+2]=128;result[i*4+3]=63;break;
                case 5: result[i*4]=(byte)((i%2)*255);result[i*4+1]=(byte)(255-result[i*4]);result[i*4+2]=128;result[i*4+3]=(byte)((i%2)*255);break;
                case 6: result[i*4]=(byte)(i*3);result[i*4+1]=(byte)(i*7);result[i*4+2]=(byte)(i*13);result[i*4+3]=(byte)(8+(i%15)*17);break;
                default: result[i*4]=(byte)(i&255);result[i*4+1]=(byte)((i*3+4)&255);result[i*4+2]=(byte)((i*7+3)&255);result[i*4+3]=(byte)(255-(i&255));break;
            }
        }
        return result;
    }
    private static byte[] Original(Compressor compress, object plugin, MethodInfo optionsMethod,
        uint width, uint height, uint depth, byte[] pixels, string format, bool mipmaps)
    {
        byte[] options = new byte[1644];
        uint* images = stackalloc uint[checked((int)depth * 6)];
        fixed(byte* op=options) fixed(byte* rgba=pixels)
        {
            optionsMethod.Invoke(plugin,new object[]{Pointer.Box(op,optionsMethod.GetParameters()[0].ParameterType),format,mipmaps});
            ((uint*)op)[118]=2;
            for(uint z=0;z<depth;z++)
            {
                uint* image=images+z*6;image[0]=width;image[1]=height;
                image[2]=(uint)(rgba+z*width*height*4);image[3]=width*height;image[4]=image[3];image[5]=0;
            }
            uint* level=stackalloc uint[3];level[0]=(uint)images;level[1]=depth;level[2]=depth;
            uint* volume=stackalloc uint[3];volume[0]=(uint)level;volume[1]=1;volume[2]=1;
            capture=new MemoryStream();calls=new List<string>();Writer writer=new Writer(Write);
            int code=compress(new IntPtr(volume),new IntPtr(op),writer,IntPtr.Zero);GC.KeepAlive(writer);
            Check(code==0,"volume compressor result="+code);byte[] result=capture.ToArray();capture.Dispose();return result;
        }
    }
    private static string Location(int offset,uint width,uint height,uint depth)
    {
        if(offset<128)return "header";
        int start=128,level=0;
        while(true){int slice=checked((int)(((width+3)/4)*((height+3)/4)*16));int size=checked(slice*(int)depth);if(offset<start+size)return "level="+level+" slice="+((offset-start)/slice)+" block="+(((offset-start)%slice)/16)+" byte="+((offset-start)%16);start+=size;level++;width=Math.Max(1u,width/2);height=Math.Max(1u,height/2);depth=Math.Max(1u,depth/2);if(level>32)return "overflow";}
    }

    private static void ValidateLayout(byte[] dds,uint width,uint height,uint depth,string format,bool mips,string label)
    {
        uint w=width,h=height,d=depth;int level=0,total=128,frame=2;
        uint headerLevels=0,max=Math.Max(width,height);while(max!=0){headerLevels++;max>>=1;}
        Check(BitConverter.ToUInt32(dds,8)==(mips?0x8a1007u:0x881007u),label+" independent flags");
        Check(BitConverter.ToUInt32(dds,12)==height&&BitConverter.ToUInt32(dds,16)==width&&BitConverter.ToUInt32(dds,24)==depth,label+" dimensions");
        Check(BitConverter.ToUInt32(dds,20)==((width+3)/4)*((height+3)/4)*16,label+" single-slice level-zero linear size");
        Check(BitConverter.ToUInt32(dds,84)==(format=="D3DFMT_DXT3"?0x33545844u:0x35545844u),label+" FourCC");
        Check(BitConverter.ToUInt32(dds,108)==(mips?0x401008u:0x1000u)&&BitConverter.ToUInt32(dds,112)==0x200000u,label+" caps");
        Check(calls.Count>=3&&calls[0]=="4:null"&&calls[1]=="124:null",label+" header callback");
        do{
            int bytes=checked((int)(((w+3)/4)*((h+3)/4)*16));total=checked(total+bytes*(int)d);
            for(uint z=0;z<d;z++){
                Check(calls.Count>frame&&calls[frame]==bytes+":"+level+","+w+","+h+","+z+","+d,label+" slice callback "+level+"/"+z);frame++;
            }
            level++;
            if(!mips||(w==1&&h==1&&d==1))break;
            w=Math.Max(1u,w/2);h=Math.Max(1u,h/2);d=Math.Max(1u,d/2);
        }while(true);
        Check(dds.Length==total&&calls.Count==frame,label+" full XYZ tail");
        Check(BitConverter.ToUInt32(dds,28)==(mips?headerLevels:0),label+" XY-only header mip count");
    }

    private static int Main(string[] args)
    {
        IDisposable owner=null;
        try{
            Check(IntPtr.Size==4,"x86 required");
            string input=Path.GetFullPath(args[0]),dir=Path.GetDirectoryName(input),output=Path.GetFullPath(args[1]);
            Check(Hash(File.ReadAllBytes(input))=="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e","Original hash mismatch");
            Assembly recovered=Assembly.LoadFrom(Path.GetFullPath(args[2]));
            MethodInfo encode=recovered.GetType("BinaryAssetBuilder.TextureCompiler.TextureDxtVolumeDdsEncoder",true).GetMethod("Encode");
            Check(encode != null, "Complete DDS Encode entry point missing");
            object[][] rejectedInputs={
                new object[]{1u,1u,1u,null,"D3DFMT_DXT3",false},
                new object[]{0u,1u,1u,new byte[0],"D3DFMT_DXT3",false},
                new object[]{1u,0u,1u,new byte[0],"D3DFMT_DXT3",false},
                new object[]{1u,1u,0u,new byte[0],"D3DFMT_DXT3",false},
                new object[]{3u,1u,1u,new byte[12],"D3DFMT_DXT3",false},
                new object[]{1u,3u,1u,new byte[12],"D3DFMT_DXT3",false},
                new object[]{1u,1u,3u,new byte[12],"D3DFMT_DXT3",false},
                new object[]{0x80000000u,0x80000000u,0x80000000u,new byte[0],"D3DFMT_DXT5",false},
                new object[]{256u,256u,256u,new byte[0],"D3DFMT_DXT3",false},
                new object[]{1u,1u,1u,new byte[3],"D3DFMT_DXT3",false},
                new object[]{1u,1u,1u,new byte[5],"D3DFMT_DXT3",false},
                new object[]{1u,1u,1u,new byte[4],"D3DFMT_DXT1",false},
                new object[]{1u,1u,1u,Pixels(1,1,1),"D3DFMT_DXT1A",false},
                new object[]{1u,1u,1u,new byte[4],"unknown",false},
                new object[]{1u,1u,1u,new byte[4],null,false}
            };
            foreach(bool mips in new[]{false,true})foreach(object[] invalid in rejectedInputs){
                invalid[5]=mips;bool rejected=false;
                try{encode.Invoke(null,invalid);}catch(TargetInvocationException e){rejected=e.InnerException is ArgumentException||e.InnerException is NotSupportedException;}
                Check(rejected,"unsupported volume input must reject");
            }
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string p=Path.Combine(dir,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
            Assembly original=Assembly.LoadFrom(input);Type type=original.GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true);
            object plugin=Activator.CreateInstance(type);owner=(IDisposable)plugin;
            MethodInfo initialize=type.GetMethod("Initialize"),reinitialize=type.GetMethod("ReInitialize");
            Type platformType=initialize.GetParameters()[1].ParameterType;
            initialize.Invoke(plugin,new object[]{null,Enum.ToObject(platformType,0)});
            MethodInfo optionsMethod=type.GetMethod("GetNVCompressionOptions",BindingFlags.Instance|BindingFlags.NonPublic);
            Check(original.ManifestModule.ResolveMethod(0x060000DB).Name=="nvDDS.nvDXTcompress","Native token");
            IntPtr module=Marshal.GetHINSTANCE(original.ManifestModule);Check(module!=IntPtr.Zero&&module!=new IntPtr(-1),"Native base");
            Compressor compressor=(Compressor)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(module,0x52950),typeof(Compressor));
            var sizes=new List<uint[]>();
            foreach(uint w in new uint[]{1,2,4,8,16})foreach(uint h in new uint[]{1,2,4,8})foreach(uint d in new uint[]{1,2,4,8})sizes.Add(new[]{w,h,d});
            sizes.AddRange(new[]{new[]{1u,1u,64u},new[]{1u,64u,1u},new[]{64u,1u,1u},new[]{16u,16u,16u},new[]{32u,8u,4u},new[]{8u,32u,4u},new[]{4u,4u,32u}});

            using(var replay=new BinaryWriter(File.Create(output+".replay")))
            using(var transcript=new StreamWriter(output)) {
                replay.Write(5568);
                transcript.WriteLine("format\tmips\twidth\theight\tdepth\tpattern\tbytes\tsha256");
                foreach(string format in new[]{"D3DFMT_DXT3","D3DFMT_DXT5"})foreach(bool mipmaps in new[]{false,true})foreach(uint[] size in sizes)for(int pattern=0;pattern<16;pattern++) {
                    uint width=size[0],height=size[1],depth=size[2];byte[] pixels=Pixels(width,height*depth,pattern%8);
                    if(pattern>=8)new Random(checked((int)(pattern*7919+size[0]*31+size[1]+size[2]*101))).NextBytes(pixels);
                    byte[] before=(byte[])pixels.Clone();
                    byte[] expected=Original(compressor,plugin,optionsMethod,width,height,depth,pixels,format,mipmaps);
                    byte[] actual=(byte[])encode.Invoke(null,new object[]{width,height,depth,pixels,format,mipmaps});
                    string label=format+" "+width+"x"+height+"x"+depth+" pattern="+pattern+" mips="+mipmaps;
                    if(Hash(actual)!=Hash(expected)) {
                        File.WriteAllBytes(output+".expected.dds",expected);File.WriteAllBytes(output+".actual.dds",actual);File.WriteAllBytes(output+".rgba",pixels);
                        File.WriteAllText(output+".failure.txt",label);
                    }

                    ValidateLayout(expected,width,height,depth,format,mipmaps,label);
                    Check(actual.Length==expected.Length,label+" size");
                    for(int i=0;i<expected.Length;i++)Check(actual[i]==expected[i],label+" DDS mismatch "+Location(i,width,height,depth)+" offset="+i+" expected="+expected[i]+" actual="+actual[i]);
                    Check(Hash(pixels)==Hash(before),"Input modified");
                    replay.Write(format);replay.Write(mipmaps);replay.Write(width);replay.Write(height);replay.Write(depth);replay.Write(pixels.Length);replay.Write(pixels);replay.Write(expected.Length);replay.Write(expected);
                    transcript.WriteLine(format+"\t"+mipmaps+"\t"+width+"\t"+height+"\t"+depth+"\t"+pattern+"\t"+actual.Length+"\t"+Hash(actual));fixtures++;
                }
            }
            Console.WriteLine("PASS Texture complete DXT3/DXT5 volume DDS fixtures="+fixtures+" checks="+checks+"; PC Production=71; full header and payload; POT including partial blocks; no-mip and recursive float volume mip chains; no plugin/SDK acceptance");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{if(owner!=null)owner.Dispose();}
    }
}


