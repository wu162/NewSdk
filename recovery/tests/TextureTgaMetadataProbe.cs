using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Recovered.Compiler.Texture;

internal static class TextureTgaMetadataProbe
{
    static int checks,fixtures,decoded,rejected;
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static string Hash(byte[] b){using(var s=SHA256.Create())return BitConverter.ToString(s.ComputeHash(b)).Replace("-","").ToLowerInvariant();}
    static string Error(Exception e){if(e==null)return "";while(e is TargetInvocationException&&e.InnerException!=null)e=e.InnerException;var p=e.GetType().GetProperty("ErrorCode");return (e.GetType().FullName+"/"+(p==null?"":Convert.ToString(p.GetValue(e,null)))+"/"+e.Message).Replace('\t',' ').Replace('\r',' ').Replace('\n',' ');}
    static byte[] Bytes(TexturePacker.Image image){byte[] b=new byte[image.Pixels.Length*4];Buffer.BlockCopy(image.Pixels,0,b,0,b.Length);return b;}
    static int Main(string[] args){try{
        Check(IntPtr.Size==4,"x86 required");string input=Path.GetFullPath(args[0]),directory=Path.GetDirectoryName(input),output=Path.GetFullPath(args[1]);Directory.CreateDirectory(output);
        AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string p=Path.Combine(directory,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
        typeof(TexturePngOracle).GetMethod("Bind",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{Assembly.LoadFrom(input),input});var load=typeof(TexturePngOracle).GetMethod("Original",BindingFlags.Static|BindingFlags.NonPublic);
        using(var log=new StreamWriter(Path.Combine(output,"transcript.tsv"))){log.AutoFlush=true;log.WriteLine("label\tprofile\twidth\theight\thasAlpha\tbytes\tsha256\terror\tinputSha256\tformat\tsourceWidth\tsourceHeight\trle\tdirection\tpattern");
            foreach(string format in TextureTgaMetadataFixtures.Formats)foreach(int[] size in new[]{new[]{8,4},new[]{16,8},new[]{7,5},new[]{1,1}})foreach(bool rle in new[]{false,true})foreach(int direction in new[]{16,32})foreach(int pattern in new[]{0,1,2})foreach(string profile in TextureTgaMetadataFixtures.Profiles){
                string label=format+"-"+size[0]+"x"+size[1]+"-r"+rle+"-d"+direction+"-p"+pattern+"-"+profile,path=Path.Combine(output,label+".tga");byte[] source=TextureTgaMetadataFixtures.Create(size[0],size[1],format,rle,direction,pattern,profile);File.WriteAllBytes(path,source);
                TexturePacker.Image image=null,again=null;Exception error=null,repeatedError=null;
                try{image=(TexturePacker.Image)load.Invoke(null,new object[]{path});}catch(Exception e){error=e;}
                try{again=(TexturePacker.Image)load.Invoke(null,new object[]{path});}catch(Exception e){repeatedError=e;}
                Check(Error(error)==Error(repeatedError),"Repeated original error "+label);byte[] rgba=image==null?null:Bytes(image);
                if(image!=null){Check(again!=null&&again.Width==image.Width&&again.Height==image.Height&&again.HasAlpha==image.HasAlpha,"Repeated original descriptor "+label);byte[] second=Bytes(again);Check(second.Length==rgba.Length,"Repeated original extent");for(int i=0;i<rgba.Length;i++)Check(rgba[i]==second[i],"Repeated original pixel "+label+"/"+i);File.WriteAllBytes(Path.Combine(output,label+".rgba"),rgba);decoded++;}else{Check(error!=null,"Missing original result");rejected++;}
                Check(Hash(source)==Hash(File.ReadAllBytes(path)),"Source changed "+label);
                log.WriteLine(label+"\t"+profile+"\t"+(image==null?0:image.Width)+"\t"+(image==null?0:image.Height)+"\t"+(image!=null&&image.HasAlpha)+"\t"+(rgba==null?0:rgba.Length)+"\t"+(rgba==null?"":Hash(rgba))+"\t"+Error(error)+"\t"+Hash(source)+"\t"+format+"\t"+size[0]+"\t"+size[1]+"\t"+rle+"\t"+direction+"\t"+pattern);fixtures++;
            }
        }
        Check(fixtures==TextureTgaMetadataFixtures.ProbeCount,"Complete TGA metadata corpus");Console.WriteLine("PROBE Texture TGA metadata fixtures="+fixtures+" decoded="+decoded+" rejected="+rejected+" originalCalls="+(fixtures*2)+" checks="+checks+"; complete original RGBA/errors, extension/gamma/alpha/developer/tables/origins, POT/NPOT/raw/RLE; no recovered/plugin/SDK acceptance");return 0;
    }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{var owner=typeof(TexturePngOracle).GetField("owner",BindingFlags.Static|BindingFlags.NonPublic).GetValue(null) as IDisposable;if(owner!=null)owner.Dispose();}}
}
