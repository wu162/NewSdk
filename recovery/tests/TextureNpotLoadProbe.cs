using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Recovered.Compiler.Texture;

// Original-only LoadImageA capture. No recovered NPOT acceptance is implied.
internal static unsafe class TextureNpotLoadProbe
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate IntPtr MallocFunction(UIntPtr bytes);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void FreeFunction(IntPtr memory);
    private static MallocFunction malloc;
    private static FreeFunction free;
    private static MethodInfo construct, destroy, load;
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
    private static int records,loaded,rejected;
    private static byte[] Tga(int width,int height,int depth,int pattern)
    {
        int stride=depth/8;byte[] b=new byte[18+width*height*stride];b[2]=2;b[12]=(byte)width;b[13]=(byte)(width>>8);b[14]=(byte)height;b[15]=(byte)(height>>8);b[16]=(byte)depth;b[17]=(byte)(32+(depth==32?8:0));
        byte[] random=new byte[width*height*4];new Random(7919*pattern+width*31+height).NextBytes(random);
        for(int y=0;y<height;y++)for(int x=0;x<width;x++){
            int i=y*width+x,r=0,g=0,blue=0,alpha=255;
            switch(pattern){
                case 0:break;
                case 1:r=g=blue=255;break;
                case 2:r=g=blue=width==1?(height==1?127:y*255/(height-1)):x*255/(width-1);break;
                case 3:r=((x+y)&1)*255;g=255-r;blue=128;alpha=r;break;
                case 4:r=g=blue=alpha=(x==width/2&&y==height/2)?255:0;break;
                case 5:r=251;g=127;blue=13;alpha=(i*37+7)&255;break;
                case 6:r=(x*71+y*13)&255;g=(x*17+y*37)&255;blue=(x*43+y*97)&255;alpha=255;break;
                default:r=random[i*4];g=random[i*4+1];blue=random[i*4+2];alpha=random[i*4+3];break;
            }
            int at=18+i*stride;b[at]=(byte)blue;b[at+1]=(byte)g;b[at+2]=(byte)r;if(depth==32)b[at+3]=(byte)alpha;
        }return b;
    }
    private static string Hash(byte[] b){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(b)).Replace("-","").ToLowerInvariant();}
    private static void Case(StreamWriter transcript,string directory,int width,int height,string profile,int pattern,string extension,byte[] file)
    {
        string label=profile+"-"+width+"x"+height+"-p"+pattern,path=Path.Combine(directory,label+"."+extension);File.WriteAllBytes(path,file);
        TexturePacker.Image image=null;Exception error=null;
        try{image=Original(path);}catch(TargetInvocationException e){error=e.InnerException;}catch(Exception e){error=e;}
        Check(Hash(File.ReadAllBytes(path))==Hash(file),"Original modified input "+label);
        if(error==null){
            bool control=(width&(width-1))==0&&(height&(height-1))==0;
            if(control)Check(image.Width==width&&image.Height==height,"POT control dimensions "+label);
            byte[] rgba=new byte[image.Pixels.Length*4];Buffer.BlockCopy(image.Pixels,0,rgba,0,rgba.Length);File.WriteAllBytes(Path.Combine(directory,label+".loaded.rgba"),rgba);
            transcript.WriteLine(label+"\t"+profile+"\t"+pattern+"\t"+width+"\t"+height+"\t"+image.Width+"\t"+image.Height+"\t"+image.HasAlpha+"\t"+rgba.Length+"\t"+Hash(rgba)+"\t");loaded++;
        }else{
            if((width&(width-1))==0&&(height&(height-1))==0)throw new Exception("Original rejected POT control "+label,error);
            string signature=(error.GetType().FullName+":"+error.Message).Replace('\t',' ').Replace('\r',' ').Replace('\n',' ');
            transcript.WriteLine(label+"\t"+profile+"\t"+pattern+"\t"+width+"\t"+height+"\t0\t0\tFalse\t0\t\t"+signature);rejected++;
        }records++;
    }
    private static int Main(string[] args)
    {
        IDisposable owner=null;
        try{
            Check(IntPtr.Size==4,"x86 required");string input=Path.GetFullPath(args[0]),directory=Path.GetDirectoryName(input),output=Path.GetFullPath(args[1]);Directory.CreateDirectory(output);
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string path=Path.Combine(directory,new AssemblyName(e.Name).Name+".dll");return File.Exists(path)?Assembly.LoadFrom(path):null;};
            Assembly original=Assembly.LoadFrom(input);Bind(original,input);Type type=original.GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true);object plugin=Activator.CreateInstance(type);owner=(IDisposable)plugin;
            MethodInfo initialize=type.GetMethod("Initialize");initialize.Invoke(plugin,new object[]{null,Enum.ToObject(initialize.GetParameters()[1].ParameterType,0)});
            int[][] sizes={new[]{3,1},new[]{1,3},new[]{3,2},new[]{2,3},new[]{3,3},new[]{5,3},new[]{3,5},new[]{7,5},new[]{5,7},new[]{9,6},new[]{6,9},new[]{15,7},new[]{7,15},new[]{17,9},new[]{9,17},new[]{31,17},new[]{17,31},new[]{63,33},new[]{33,63},new[]{1,1},new[]{2,2},new[]{4,4},new[]{8,4}};
            using(var transcript=new StreamWriter(Path.Combine(output,"transcript.tsv"),false,new UTF8Encoding(false))){
                transcript.WriteLine("label\tprofile\tpattern\tsourceWidth\tsourceHeight\tloadedWidth\tloadedHeight\thasAlpha\tbytes\tsha256\terror");
                foreach(int[] size in sizes){int width=size[0],height=size[1];
                    foreach(int depth in new[]{24,32})for(int pattern=0;pattern<8;pattern++)Case(transcript,output,width,height,"tga-d"+depth,pattern,"tga",Tga(width,height,depth,pattern));
                    foreach(int color in new[]{2,6})foreach(int interlace in new[]{0,1})for(int pattern=0;pattern<4;pattern++)Case(transcript,output,width,height,"png-c"+color+"-i"+interlace,pattern,"png",TexturePngOracle.Fixture(width,height,8,color,interlace,5,0,pattern));
                    foreach(int depth in new[]{24,32})foreach(bool top in new[]{false,true})for(int pattern=0;pattern<3;pattern++)Case(transcript,output,width,height,"bmp-d"+depth+"-top"+top,pattern,"bmp",TextureBmpOracle.Fixture(width,height,depth,top,pattern,0,7));
                }
            }
            Check(records==1012,"Incomplete NPOT load corpus");Console.WriteLine("PROBE Texture original NPOT LoadImageA records="+records+" decoded="+loaded+" rejected="+rejected+" checks="+assertions+"; dimensions/full RGBA capture; original-only; no recovered or SDK acceptance");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{if(owner!=null)owner.Dispose();}
    }
}
