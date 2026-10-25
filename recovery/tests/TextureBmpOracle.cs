using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Recovered.Compiler.Texture;

// x86 CLR4 /unsafe, legacy v2 activation. Original LoadImageA is invoked through
// its managed IL entry; all original image storage uses that module's VC8 CRT.
internal static unsafe class TextureBmpOracle
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate IntPtr MallocFunction(UIntPtr bytes);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void FreeFunction(IntPtr memory);
    private static MallocFunction malloc;
    private static FreeFunction free;
    private static MethodInfo construct, destroy, load;
    private static IDisposable owner;
    private static int fixtures, mismatches, assertions, unsupportedCount, originalFailures, rejectionMatches, decodedMatches;
    private static object PointerArg(MethodInfo method, int index, void* value)
    { return Pointer.Box(value, method.GetParameters()[index].ParameterType); }
    private static void Check(bool condition, string message)
    { assertions++; if (!condition) throw new InvalidOperationException(message); }
    private static void Bind(Assembly original, string input)
    {
        string hash;
        using (SHA256 sha = SHA256.Create())
            hash = BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(input))).Replace("-", "").ToLowerInvariant();
        if (hash != "4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e")
            throw new InvalidDataException("Original TextureCompiler changed; review allocator RVAs and managed method tokens.");
        IntPtr module = Marshal.GetHINSTANCE(original.ManifestModule);
        Check(module != IntPtr.Zero && module != new IntPtr(-1), "Native module handle");
        IntPtr m = IntPtr.Add(module, 0x4e74c), f = IntPtr.Add(module, 0x4e758);
        Check(Marshal.ReadByte(m) == 0xFF && Marshal.ReadByte(m, 1) == 0x25, "Original malloc thunk");
        Check(Marshal.ReadByte(f) == 0xFF && Marshal.ReadByte(f, 1) == 0x25, "Original free thunk");
        malloc = (MallocFunction)Marshal.GetDelegateForFunctionPointer(m, typeof(MallocFunction));
        free = (FreeFunction)Marshal.GetDelegateForFunctionPointer(f, typeof(FreeFunction));
        load = (MethodInfo)original.ManifestModule.ResolveMethod(0x06000017);
        construct = (MethodInfo)original.ManifestModule.ResolveMethod(0x0600002c);
        destroy = (MethodInfo)original.ManifestModule.ResolveMethod(0x0600002d);
        Check(load.Name == "BinaryAssetBuilder.TextureCompiler.LoadImageA", "Original LoadImageA token");
        Check(construct.Name == "nvImage<rgba_t>.{ctor}" && destroy.Name == "nvImage<rgba_t>.{dtor}", "Original nvImage tokens");
        Type pluginType = original.GetType("BinaryAssetBuilder.TextureCompiler.Plugin", true);
        object plugin = Activator.CreateInstance(pluginType);
        MethodInfo initialize = pluginType.GetMethod("Initialize");
        Type platform = initialize.GetParameters()[1].ParameterType;
        initialize.Invoke(plugin, new object[] { null, Enum.ToObject(platform, 0) });
        owner = (IDisposable)plugin;
        byte[] sanity = new byte[34]; sanity[2]=2;sanity[12]=2;sanity[14]=2;sanity[16]=32;sanity[17]=40;
        string sanityPath=Path.Combine(Path.GetDirectoryName(input),"sanity.tga");File.WriteAllBytes(sanityPath,sanity);
        Check(Original(sanityPath).Width==2,"Initialized native TGA positive control");
    }
    private static TexturePacker.Image Original(string path)
    {
        IntPtr image = malloc((UIntPtr)24u);
        if (image == IntPtr.Zero) throw new OutOfMemoryException();
        bool constructed = false;
        try
        {
            for (int i = 0; i < 24; i++) ((byte*)image)[i] = 0xA5;
            construct.Invoke(null, new object[] { PointerArg(construct, 0, image.ToPointer()) });
            constructed = true;
            bool hasAlpha = false;
            byte[] pathBytes = Encoding.Default.GetBytes(path + "\0");
            fixed (byte* pathPointer = pathBytes)
                load.Invoke(null, new object[] { PointerArg(load, 0, image.ToPointer()), PointerArg(load, 1, &hasAlpha), PointerArg(load, 2, pathPointer) });
            int width = ((int*)image)[0], height = ((int*)image)[1];
            Check(width > 0 && height > 0 && (long)width * height < 16777216L, "Original loaded dimensions");
            int count = checked(width * height);
            Check(((int*)image)[3] == count && ((int*)image)[4] >= count, "Original nvVector size/capacity");
            uint* data = (uint*)((IntPtr*)image)[2];
            Check(data != null, "Original loaded pixel pointer");
            uint[] pixels = new uint[count];
            for (int i = 0; i < count; i++) pixels[i] = data[i];
            return new TexturePacker.Image(width, height, pixels, hasAlpha);
        }
        finally
        {
            try
            {
                if (constructed) destroy.Invoke(null, new object[] { PointerArg(destroy, 0, image.ToPointer()) });
            }
            finally { free(image); }
        }
    }
    public static byte[] Fixture(int width,int height,int depth,bool topDown,int pattern,int paletteReserved,int gap)
    {
        int stride=(width*depth+31)/32*4,entries=depth<=8?1<<depth:0,offset=54+entries*4+gap;
        using(var s=new MemoryStream())using(var w=new BinaryWriter(s))
        {
            w.Write((ushort)0x4d42);w.Write((uint)(offset+stride*height));w.Write(0u);w.Write((uint)offset);
            w.Write(40u);w.Write(width);w.Write(topDown?-height:height);w.Write((ushort)1);w.Write((ushort)depth);w.Write(0u);w.Write((uint)(stride*height));w.Write(2835);w.Write(2835);w.Write((uint)entries);w.Write(0u);
            for(int i=0;i<entries;i++){w.Write((byte)(i*47+71));w.Write((byte)(i*13+59));w.Write((byte)(i*23+17));w.Write((byte)paletteReserved);}
            for(int i=0;i<gap;i++)w.Write((byte)(i*17+111));
            for(int fy=0;fy<height;fy++)
            {
                int y=topDown?fy:height-1-fy;byte[] row=new byte[stride];for(int i=0;i<stride;i++)row[i]=0xa7;
                if(depth<8)Array.Clear(row,0,(width*depth+7)/8);
                for(int x=0;x<width;x++)
                {
                    int index=y*width+x;
                    if(depth<=8){int v=pattern==1?0:pattern==2?(1<<depth)-1:(index*71+17)&((1<<depth)-1);if(depth==8)row[x]=(byte)v;else row[x*depth/8]|=(byte)(v<<(8-depth-x*depth%8));}
                    else if(depth==16){int v=pattern==1?0:pattern==2?65535:(index*1271+739)&65535;row[x*2]=(byte)v;row[x*2+1]=(byte)(v>>8);}
                    else{int p=x*(depth/8);row[p]=(byte)(index*31+13);row[p+1]=(byte)(index*47+71);row[p+2]=(byte)(index*17+29);if(depth==32)row[p+3]=(byte)(pattern==1?0:pattern==2?255:index*43+17);}
                }
                w.Write(row);
            }
            return s.ToArray();
        }
    }
    private static string Hash(uint[] pixels)
    {
        byte[] b=new byte[pixels.Length*4];Buffer.BlockCopy(pixels,0,b,0,b.Length);
        using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(b)).Replace("-","");
    }
    private static int Main(string[] args)
    {
        try
        {
            bool probe=args.Length>3&&args[3]=="--probe";string input=Path.GetFullPath(args[0]),dir=Path.GetDirectoryName(input);
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string path=Path.Combine(dir,new AssemblyName(e.Name).Name+".dll");return File.Exists(path)?Assembly.LoadFrom(path):null;};
            Check(IntPtr.Size==4,"x86 required");Bind(Assembly.LoadFrom(input),input);Directory.CreateDirectory(args[2]);
            using(var log=new StreamWriter(args[1]))
            foreach(int depth in new[]{1,4,8,16,24,32})foreach(int width in new[]{1,2,4,8,16,32})foreach(int height in new[]{1,2,4,8,16})foreach(bool top in new[]{false,true})foreach(int pattern in new[]{0,1,2})foreach(int reserved in depth<=8?new[]{0,255}:new[]{0})foreach(int gap in new[]{0,7})
            {
                string label="d"+depth+"-"+width+"x"+height+"-top"+top+"-p"+pattern+"-reserved"+reserved+"-gap"+gap,path=Path.Combine(args[2],"test.bmp");byte[] bytes=Fixture(width,height,depth,top,pattern,reserved,gap);File.WriteAllBytes(path,bytes);fixtures++;
                TexturePacker.Image a=null,b=null;Exception ea=null,eb=null;
                try{a=Original(path);}catch(Exception e){ea=e;}
                try{b=TextureBmpDecoder.Decode(bytes);}catch(Exception e){eb=e;}
                bool equal=ea==null&&eb==null&&a.Width==b.Width&&a.Height==b.Height&&a.HasAlpha==b.HasAlpha&&Hash(a.Pixels)==Hash(b.Pixels);
                int first=-1;if(a!=null&&b!=null)for(int i=0;i<Math.Min(a.Pixels.Length,b.Pixels.Length);i++)if(a.Pixels[i]!=b.Pixels[i]){first=i;break;}
                log.WriteLine(label+"|equal="+equal+"|original="+(a==null?ea.ToString():a.Width+"x"+a.Height+" alpha="+a.HasAlpha+" hash="+Hash(a.Pixels))+"|candidate="+(b==null?eb.ToString():b.Width+"x"+b.Height+" alpha="+b.HasAlpha+" hash="+Hash(b.Pixels))+"|first="+first+(first<0?"":" expected="+a.Pixels[first].ToString("X8")+" actual="+b.Pixels[first].ToString("X8")));
                if(equal)decodedMatches++;else{mismatches++;if(ea!=null)originalFailures++;if(mismatches<=12)Console.WriteLine("DIFFERENCE "+label+" alpha="+(a==null?"error":a.HasAlpha.ToString())+"/"+(b==null?"error":b.HasAlpha.ToString())+" first="+first+(first<0?"":" expected="+a.Pixels[first].ToString("X8")+" actual="+b.Pixels[first].ToString("X8")));if(!probe)throw new Exception("BMP difference "+label);}
            }
            Console.WriteLine((probe?"PROBE":"PASS")+" Texture BMP fixtures="+fixtures+" decodedMatches="+decodedMatches+" originalFailures="+originalFailures+" mismatches="+mismatches+" assertions="+assertions);return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
        finally { if(owner!=null)owner.Dispose(); }
    }
}
