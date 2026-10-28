using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Recovered.Compiler.Texture;

// The adapter is the existing hash/token/allocator-checked PNG oracle. This
// entry only captures original pixels/errors and repeat determinism.
internal static class TexturePngMetadataProbe
{
    static int checks,fixtures,decoded,rejected;
    static void Check(bool v,string s){checks++;if(!v)throw new Exception(s);}
    static string Hash(byte[] b){using(var s=SHA256.Create())return BitConverter.ToString(s.ComputeHash(b)).Replace("-","").ToLowerInvariant();}
    static byte[] Bytes(TexturePacker.Image image){byte[] b=new byte[image.Pixels.Length*4];Buffer.BlockCopy(image.Pixels,0,b,0,b.Length);return b;}
    static string Error(Exception e){if(e==null)return "";while(e is TargetInvocationException&&e.InnerException!=null)e=e.InnerException;var p=e.GetType().GetProperty("ErrorCode");return (e.GetType().FullName+"/"+(p==null?"":Convert.ToString(p.GetValue(e,null)))+"/"+e.Message).Replace('\t',' ').Replace('\r',' ').Replace('\n',' ');}
    static int Main(string[] args){
        try{
            Check(IntPtr.Size==4,"x86 required");string input=Path.GetFullPath(args[0]),directory=Path.GetDirectoryName(input),output=Path.GetFullPath(args[1]),profiles=Path.GetFullPath(args[2]);Directory.CreateDirectory(output);
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string p=Path.Combine(directory,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
            typeof(TexturePngOracle).GetMethod("Bind",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{Assembly.LoadFrom(input),input});
            MethodInfo load=typeof(TexturePngOracle).GetMethod("Original",BindingFlags.Static|BindingFlags.NonPublic);
            using(var log=new StreamWriter(Path.Combine(output,"transcript.tsv"))){log.AutoFlush=true;log.WriteLine("label\tprofile\twidth\theight\thasAlpha\tbytes\tsha256\terror\tinputSha256");
                foreach(int[] format in TexturePngMetadataFixtures.Formats)foreach(int[] size in new[]{new[]{8,4},new[]{7,5},new[]{16,8}})foreach(int interlace in new[]{0,1})foreach(int pattern in new[]{0,3})foreach(string profile in TexturePngMetadataFixtures.Profiles){
                    int c=format[0],d=format[1],w=size[0],h=size[1];string label="c"+c+"d"+d+"-"+w+"x"+h+"-i"+interlace+"-p"+pattern+"-"+profile,path=Path.Combine(output,label+".png");byte[] source=TexturePngMetadataFixtures.Create(profiles,w,h,c,d,interlace,pattern,profile);File.WriteAllBytes(path,source);
                    TexturePacker.Image image=null,again=null;Exception error=null,repeatError=null;
                    try{image=(TexturePacker.Image)load.Invoke(null,new object[]{path});}catch(Exception e){error=e;}
                    try{again=(TexturePacker.Image)load.Invoke(null,new object[]{path});}catch(Exception e){repeatError=e;}
                    Check(Error(error)==Error(repeatError),"Repeated original error "+label);byte[] rgba=image==null?null:Bytes(image);
                    if(rgba!=null){Check(again!=null&&again.Width==image.Width&&again.Height==image.Height&&again.HasAlpha==image.HasAlpha,"Repeated original descriptor "+label);byte[] repeated=Bytes(again);Check(rgba.Length==repeated.Length,"Repeated original length");for(int i=0;i<rgba.Length;i++)Check(rgba[i]==repeated[i],"Repeated original pixel "+label+"/"+i);File.WriteAllBytes(Path.Combine(output,label+".rgba"),rgba);decoded++;}else{Check(error!=null,"Missing original result");rejected++;}
                    Check(Hash(File.ReadAllBytes(path))==Hash(source),"Source unchanged "+label);
                    log.WriteLine(label+"\t"+profile+"\t"+(image==null?0:image.Width)+"\t"+(image==null?0:image.Height)+"\t"+(image!=null&&image.HasAlpha)+"\t"+(rgba==null?0:rgba.Length)+"\t"+(rgba==null?"":Hash(rgba))+"\t"+Error(error)+"\t"+Hash(source));fixtures++;
                }
            }
            Check(fixtures==4680,"Full metadata probe corpus");Console.WriteLine("PROBE Texture PNG metadata fixtures="+fixtures+" decoded="+decoded+" rejected="+rejected+" originalCalls="+(fixtures*2)+" checks="+checks+"; original complete RGBA/errors, ICC/APNG, POT/NPOT, Adam7 and gamma; not recovered or SDK acceptance");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{var f=typeof(TexturePngOracle).GetField("owner",BindingFlags.Static|BindingFlags.NonPublic);var owner=f.GetValue(null) as IDisposable;if(owner!=null)owner.Dispose();}
    }
}
