using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Recovered.Compiler.Texture;

internal static class TextureBmpHeaderProbe
{
    static int fixtures,decoded,rejected,checks,matches,differences,unstable;
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static string Hash(byte[] b){using(var s=SHA256.Create())return BitConverter.ToString(s.ComputeHash(b)).Replace("-","").ToLowerInvariant();}
    static string Error(Exception e){if(e==null)return "";while(e is TargetInvocationException&&e.InnerException!=null)e=e.InnerException;var p=e.GetType().GetProperty("ErrorCode");return (e.GetType().FullName+"/"+(p==null?"":Convert.ToString(p.GetValue(e,null)))+"/"+e.Message).Replace('\t',' ').Replace('\r',' ').Replace('\n',' ');}
    static byte[] Bytes(TexturePacker.Image image){byte[] b=new byte[image.Pixels.Length*4];Buffer.BlockCopy(image.Pixels,0,b,0,b.Length);return b;}
    static void Case(MethodInfo load,StreamWriter log,string output,string icc,int width,int height,int depth,bool top,int pattern,int dib,int compression,string profile,int gap){
        string label="h"+dib+"-d"+depth+"-"+width+"x"+height+"-top"+top+"-p"+pattern+"-c"+compression+"-g"+gap+"-"+profile,path=Path.Combine(output,label+".bmp");byte[] source=TextureBmpHeaderFixtures.Create(icc,width,height,depth,top,pattern,dib,compression,profile,gap);File.WriteAllBytes(path,source);
        TexturePacker.Image image=null,again=null,candidate=null;Exception error=null,repeatError=null,candidateError=null;
        try{image=(TexturePacker.Image)load.Invoke(null,new object[]{path});}catch(Exception e){error=e;}
        try{again=(TexturePacker.Image)load.Invoke(null,new object[]{path});}catch(Exception e){repeatError=e;}
        byte[] rgba=image==null?null:Bytes(image), repeated=again==null?null:Bytes(again);
        bool stable=Error(error)==Error(repeatError)&&(image==null)==(again==null);
        if(image!=null&&again!=null){stable=stable&&again.Width==image.Width&&again.Height==image.Height&&again.HasAlpha==image.HasAlpha&&repeated.Length==rgba.Length;if(stable)for(int i=0;i<rgba.Length;i++)if(rgba[i]!=repeated[i]){stable=false;break;}}
        if(!stable)unstable++;
        if(repeated!=null)File.WriteAllBytes(Path.Combine(output,label+".repeat.rgba"),repeated);
        if(image!=null){File.WriteAllBytes(Path.Combine(output,label+".rgba"),rgba);decoded++;}else{Check(error!=null,"Missing original result");rejected++;}
        try{candidate=TextureImageLoader.Decode(source);}catch(Exception e){candidateError=e;}
        bool same=Error(error)==Error(candidateError);byte[] actual=candidate==null?null:Bytes(candidate);
        if(image!=null||candidate!=null){same=same&&image!=null&&candidate!=null&&image.Width==candidate.Width&&image.Height==candidate.Height&&image.HasAlpha==candidate.HasAlpha&&rgba.Length==actual.Length;if(same)for(int i=0;i<rgba.Length;i++)if(rgba[i]!=actual[i]){same=false;break;}}
        if(actual!=null)File.WriteAllBytes(Path.Combine(output,label+".candidate.rgba"),actual);if(same)matches++;else differences++;
        Check(Hash(source)==Hash(File.ReadAllBytes(path)),"Source changed "+label);
        log.WriteLine(label+"\t"+profile+"\t"+(image==null?0:image.Width)+"\t"+(image==null?0:image.Height)+"\t"+(image!=null&&image.HasAlpha)+"\t"+(rgba==null?0:rgba.Length)+"\t"+(rgba==null?"":Hash(rgba))+"\t"+Error(error)+"\t"+Hash(source)+"\t"+dib+"\t"+depth+"\t"+compression+"\t"+top+"\t"+gap+"\t"+same+"\t"+Error(candidateError)+"\t"+(actual==null?"":Hash(actual))+"\t"+stable+"\t"+(again==null?0:again.Width)+"\t"+(again==null?0:again.Height)+"\t"+(again!=null&&again.HasAlpha)+"\t"+(repeated==null?0:repeated.Length)+"\t"+(repeated==null?"":Hash(repeated))+"\t"+Error(repeatError));fixtures++;
    }
    static int Main(string[] args){try{
        Check(IntPtr.Size==4,"x86 required");string input=Path.GetFullPath(args[0]),output=Path.GetFullPath(args[1]),icc=Path.GetFullPath(args[2]),directory=Path.GetDirectoryName(input);Directory.CreateDirectory(output);
        AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string p=Path.Combine(directory,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
        typeof(TexturePngOracle).GetMethod("Bind",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{Assembly.LoadFrom(input),input});var load=typeof(TexturePngOracle).GetMethod("Original",BindingFlags.Static|BindingFlags.NonPublic);
        using(var log=new StreamWriter(Path.Combine(output,"transcript.tsv"))){log.AutoFlush=true;log.WriteLine("label\tprofile\twidth\theight\thasAlpha\tbytes\tsha256\terror\tinputSha256\tdib\tdepth\tcompression\ttopDown\tgap\tcandidateEqual\tcandidateError\tcandidateSha256\toriginalStable\trepeatWidth\trepeatHeight\trepeatHasAlpha\trepeatBytes\trepeatSha256\trepeatError");
            foreach(int[] size in new[]{new[]{16,8},new[]{7,5},new[]{1,1},new[]{32,16}})foreach(int pattern in new[]{0,1,2}){
                foreach(int dib in TextureBmpHeaderFixtures.WindowsDibs)foreach(int depth in new[]{16,24,32})foreach(bool top in new[]{false,true})foreach(int compression in new[]{0,3,6})Case(load,log,output,icc,size[0],size[1],depth,top,pattern,dib,compression,"plain",0);
                foreach(int dib in new[]{12,64})Case(load,log,output,icc,size[0],size[1],24,false,pattern,dib,0,"plain",0);
                foreach(int dib in new[]{108,124})foreach(int depth in new[]{16,24,32})foreach(bool top in new[]{false,true})foreach(string profile in new[]{"gamma1","gamma22","srgb","calibrated","embedded-srgb2","embedded-srgb4","embedded-gray4"}){
                    if(dib==108&&profile.StartsWith("embedded-"))continue;Case(load,log,output,icc,size[0],size[1],depth,top,pattern,dib,0,profile,0);
                }
                foreach(int dib in TextureBmpHeaderFixtures.WindowsDibs)foreach(int depth in new[]{16,24,32})foreach(bool top in new[]{false,true})foreach(int gap in new[]{3,17})Case(load,log,output,icc,size[0],size[1],depth,top,pattern,dib,0,"plain",gap);
            }
        }
        Check(fixtures==TextureBmpHeaderFixtures.ProbeCount&&decoded+rejected==fixtures&&matches+differences==fixtures,"Complete BMP header corpus accounting");Console.WriteLine("PROBE Texture BMP headers fixtures="+fixtures+" decoded="+decoded+" rejected="+rejected+" candidateMatches="+matches+" differences="+differences+" unstableOriginals="+unstable+" originalCalls="+(fixtures*2)+" checks="+checks+"; full original/candidate RGBA/errors, DIB/core/V2/V3/V4/V5/OS2, gamma/ICC/bitfields/gaps; no recovered/plugin/SDK acceptance");return 0;
    }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{var owner=typeof(TexturePngOracle).GetField("owner",BindingFlags.Static|BindingFlags.NonPublic).GetValue(null) as IDisposable;if(owner!=null)owner.Dispose();}}
}
