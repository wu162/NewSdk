using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Recovered.Compiler.Texture;

// Original-only LoadImageA capture. No recovered NPOT acceptance is implied.
internal static unsafe class TextureJpegLoadProbe
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
    [DllImport("TextureJpegCandidate.dll",CallingConvention=CallingConvention.Cdecl)]
    static extern int JpegCandidate(byte[] input,int length,int mode,[Out] byte[] output,int capacity,out int width,out int height);
    static int records,loaded,rejected;static int[] exact=new int[6];
    static string Hash(byte[] bytes){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
    static string Error(Exception e){if(e==null)return "";while(e is TargetInvocationException&&e.InnerException!=null)e=e.InnerException;var code=e.GetType().GetProperty("ErrorCode");return (e.GetType().FullName+"/"+(code==null?"":Convert.ToString(code.GetValue(e,null)))+"/"+e.Message).Replace('\t',' ').Replace('\r',' ').Replace('\n',' ');}
    static byte[] Bytes(TexturePacker.Image image){byte[] result=new byte[image.Pixels.Length*4];Buffer.BlockCopy(image.Pixels,0,result,0,result.Length);return result;}
    static byte[] Ppm(int w,int h,int pattern){byte[] prefix=Encoding.ASCII.GetBytes("P6\n"+w+" "+h+"\n255\n");byte[] b=new byte[prefix.Length+w*h*3];Buffer.BlockCopy(prefix,0,b,0,prefix.Length);var random=new Random(1979+pattern*71+w*13+h*19);for(int y=0;y<h;y++)for(int x=0;x<w;x++){int at=prefix.Length+(y*w+x)*3;for(int c=0;c<3;c++)b[at+c]=(byte)(pattern==0?new[]{251,127,13}[c]:pattern==1?(x+y)*19:pattern==2?((x+y+c)&1)*255:random.Next(256));}return b;}
    static void Case(StreamWriter transcript,string output,string label,int sw,int sh,string profile,byte[] file){
      string path=Path.Combine(output,label+".jpg");File.WriteAllBytes(path,file);TexturePacker.Image image=null;Exception error=null;try{image=Original(path);}catch(Exception e){error=e;}
      TexturePacker.Image again=null;Exception repeated=null;try{again=Original(path);}catch(Exception e){repeated=e;}Check(Error(error)==Error(repeated),"Original JPEG repeated exception "+label);
      byte[] rgba=image==null?null:Bytes(image);if(rgba!=null){Check(again!=null&&again.Width==image.Width&&again.Height==image.Height&&again.HasAlpha==image.HasAlpha&&Hash(Bytes(again))==Hash(rgba),"Original JPEG repeated pixels "+label);File.WriteAllBytes(Path.Combine(output,label+".loaded.rgba"),rgba);loaded++;}else rejected++;
      Check(Hash(File.ReadAllBytes(path))==Hash(file),"Original changed JPEG input "+label);
      string[] candidates=new string[6];
      for(int mode=0;mode<6;mode++){
        int w,h;int ok=JpegCandidate(file,file.Length,mode,null,0,out w,out h);byte[] result=null;
        if(ok!=0){result=new byte[checked(w*h*4)];ok=JpegCandidate(file,file.Length,mode,result,result.Length,out w,out h);}
        if(ok!=0){uint[] pixels=new uint[w*h];Buffer.BlockCopy(result,0,pixels,0,result.Length);var candidate=new TexturePacker.Image(w,h,pixels,false);if((w&(w-1))!=0||(h&(h-1))!=0)candidate=TextureNpotRgba8Resampler.Resize(candidate);result=Bytes(candidate);File.WriteAllBytes(Path.Combine(output,label+".candidate-"+mode+".rgba"),result);bool same=rgba!=null&&candidate.Width==image.Width&&candidate.Height==image.Height&&candidate.HasAlpha==image.HasAlpha&&Hash(result)==Hash(rgba);if(same)exact[mode]++;candidates[mode]=(same?"same:":"different:")+Hash(result);}
        else candidates[mode]=rgba==null?"both-error":"candidate-error";
      }
      transcript.WriteLine(label+"\t"+profile+"\t"+sw+"\t"+sh+"\t"+(image==null?0:image.Width)+"\t"+(image==null?0:image.Height)+"\t"+(image!=null&&image.HasAlpha)+"\t"+(rgba==null?0:rgba.Length)+"\t"+(rgba==null?"":Hash(rgba))+"\t"+Error(error)+"\t"+String.Join("\t",candidates));records++;
    }
    static int Main(string[] args){IDisposable owner=null;try{
      Check(IntPtr.Size==4,"x86 required");string input=Path.GetFullPath(args[0]),directory=Path.GetDirectoryName(input),output=Path.GetFullPath(args[1]),encoder=Path.GetFullPath(args[2]);Directory.CreateDirectory(output);
      AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string path=Path.Combine(directory,new AssemblyName(e.Name).Name+".dll");return File.Exists(path)?Assembly.LoadFrom(path):null;};var original=Assembly.LoadFrom(input);Bind(original,input);Type type=original.GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true);object plugin=Activator.CreateInstance(type);owner=(IDisposable)plugin;var initialize=type.GetMethod("Initialize");initialize.Invoke(plugin,new object[]{null,Enum.ToObject(initialize.GetParameters()[1].ParameterType,0)});
      using(var transcript=new StreamWriter(Path.Combine(output,"transcript.tsv"))){transcript.AutoFlush=true;transcript.WriteLine("label\tprofile\tsourceWidth\tsourceHeight\tloadedWidth\tloadedHeight\thasAlpha\tbytes\tsha256\terror\tislow-box\tifast-box\tfloat-box\tislow-fancy\tifast-fancy\tfloat-fancy");
        foreach(int[] size in new[]{new[]{1,1},new[]{2,2},new[]{8,4},new[]{16,16},new[]{32,16},new[]{3,2},new[]{7,5},new[]{17,9}})
        for(int pattern=0;pattern<4;pattern++)foreach(int quality in new[]{30,75,95,100})foreach(string sample in new[]{"gray","1x1","2x1","2x2","4x1"})foreach(bool progressive in new[]{false,true}){
          int w=size[0],h=size[1];string profile="p"+pattern+"-q"+quality+"-s"+sample+"-progressive"+progressive,label="jpeg-"+w+"x"+h+"-"+profile,ppm=Path.Combine(output,"source.ppm"),jpg=Path.Combine(output,"encoded.jpg");File.WriteAllBytes(ppm,Ppm(w,h,pattern));
          string options="-quality "+quality+(sample=="gray"?" -grayscale":" -sample "+sample+",1x1,1x1")+(progressive?" -progressive":"")+" -outfile \""+jpg+"\" \""+ppm+"\"";
          var start=new System.Diagnostics.ProcessStartInfo(encoder,options);start.UseShellExecute=false;start.CreateNoWindow=true;start.RedirectStandardError=true;using(var p=System.Diagnostics.Process.Start(start)){string stderr=p.StandardError.ReadToEnd();p.WaitForExit();Check(p.ExitCode==0,"JPEG fixture encoder "+label+": "+stderr);}
          Case(transcript,output,label,w,h,profile,File.ReadAllBytes(jpg));
        }
        byte[] source=File.ReadAllBytes(Path.Combine(output,"jpeg-16x16-p3-q75-s2x2-progressiveFalse.jpg"));
        foreach(int length in new[]{source.Length-2,source.Length-32,source.Length/2,32}){byte[] truncated=new byte[length];Buffer.BlockCopy(source,0,truncated,0,length);Case(transcript,output,"truncated-"+length,16,16,"truncated",truncated);}
      }
      Check(records==1284,"Full JPEG option corpus");Console.WriteLine("PROBE Texture PC JPEG original records="+records+" decoded="+loaded+" rejected="+rejected+" originalCalls="+(records*2)+" checks="+assertions+" exactByMode="+String.Join(",",Array.ConvertAll(exact,x=>x.ToString()))+"; modes ISLOW/IFAST/FLOAT box then fancy; original pixel/exception evidence only, no plugin or SDK acceptance");return 0;
    }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{if(owner!=null)owner.Dispose();}}
}
