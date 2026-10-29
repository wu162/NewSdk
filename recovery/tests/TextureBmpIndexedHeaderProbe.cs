using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Recovered.Compiler.Texture;

// Original-only BI_RGB indexed-header boundaries. Unknown decode/reject/stability
// outcomes are recorded without imposing the current recovered decoder's policy.
internal static class TextureBmpIndexedHeaderProbe
{
    sealed class Loaded
    {
        internal int Width,Height;
        internal bool HasAlpha;
        internal byte[] Rgba;
        internal string Error="";
    }
    static int fixtures,controls,decoded,rejected,unstable,checks;
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static string Hash(byte[] bytes){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
    static string Error(Exception error)
    {
        if(error==null)return "";while(error is TargetInvocationException&&error.InnerException!=null)error=error.InnerException;
        var property=error.GetType().GetProperty("ErrorCode");
        return (error.GetType().FullName+"/"+(property==null?"":Convert.ToString(property.GetValue(error,null)))+"/"+error.Message).Replace('\t',' ').Replace('\r',' ').Replace('\n',' ');
    }
    static void Word(byte[] bytes,int at,int value){bytes[at]=(byte)value;bytes[at+1]=(byte)(value>>8);}
    static void Dword(byte[] bytes,int at,uint value){for(int i=0;i<4;i++)bytes[at+i]=(byte)(value>>(i*8));}
    static byte[] Fixture(int width,int height,int depth,bool top,int pattern,int dib,int gap,int reserved)
    {
        bool core=dib==12;int entries=depth<=8?1<<depth:0,paletteEntryBytes=core?3:4;
        byte[] basic=TextureBmpOracle.Fixture(width,height,depth,top,pattern,reserved,0);
        int sourcePixels=54+entries*4,headerEnd=14+dib,offset=headerEnd+entries*paletteEntryBytes+gap;
        byte[] image=new byte[offset+basic.Length-sourcePixels];Buffer.BlockCopy(basic,0,image,0,14);
        Dword(image,2,(uint)image.Length);Dword(image,10,(uint)offset);
        if(core){Dword(image,14,12);Word(image,18,width);Word(image,20,height);Word(image,22,1);Word(image,24,depth);}
        else{Buffer.BlockCopy(basic,14,image,14,40);Dword(image,14,(uint)dib);if(dib==124)Dword(image,122,4);}
        for(int index=0;index<entries;index++)Buffer.BlockCopy(basic,54+index*4,image,headerEnd+index*paletteEntryBytes,paletteEntryBytes);
        for(int index=0;index<gap;index++)image[headerEnd+entries*paletteEntryBytes+index]=(byte)(index*17+111);
        Buffer.BlockCopy(basic,sourcePixels,image,offset,basic.Length-sourcePixels);
        int stride=(width*depth+31)/32*4;
        Check(image.Length==offset+stride*height&&image.Length>=headerEnd+entries*paletteEntryBytes,"Proper bounded indexed BMP storage");
        Check(!core||!top,"Core fixture has unsigned bottom-up height");
        return image;
    }
    static Loaded Load(MethodInfo load,string path)
    {
        Loaded result=new Loaded();
        try{
            var image=(TexturePacker.Image)load.Invoke(null,new object[]{path});result.Width=image.Width;result.Height=image.Height;result.HasAlpha=image.HasAlpha;
            result.Rgba=new byte[image.Pixels.Length*4];Buffer.BlockCopy(image.Pixels,0,result.Rgba,0,result.Rgba.Length);
        }catch(Exception error){result.Error=Error(error);}return result;
    }
    static bool Same(Loaded first,Loaded second)
    {
        if(first.Error!=second.Error||first.Width!=second.Width||first.Height!=second.Height||first.HasAlpha!=second.HasAlpha)return false;
        if(first.Rgba==null||second.Rgba==null)return first.Rgba==second.Rgba;
        if(first.Rgba.Length!=second.Rgba.Length)return false;
        for(int i=0;i<first.Rgba.Length;i++)if(first.Rgba[i]!=second.Rgba[i])return false;return true;
    }
    static string Columns(Loaded image)
    {return image.Width+"\t"+image.Height+"\t"+image.HasAlpha+"\t"+(image.Rgba==null?0:image.Rgba.Length)+"\t"+(image.Rgba==null?"":Hash(image.Rgba))+"\t"+image.Error;}
    static int Pot(int value){int result=1;while(result<value)result*=2;return result;}
    static void Progress(string output,string label,string phase)
    {File.WriteAllText(Path.Combine(output,"progress.tsv"),"label\tphase\n"+label+"\t"+phase+"\n");}
    static void Case(MethodInfo load,StreamWriter log,string output,int width,int height,int depth,bool top,int pattern,int dib,int gap,int reserved,bool control)
    {
        string label=(control?"control":"index")+"-h"+dib+"-d"+depth+"-"+width+"x"+height+"-top"+top+"-p"+pattern+"-g"+gap+"-a"+reserved,path=Path.Combine(output,label+".bmp");
        byte[] bytes=Fixture(width,height,depth,top,pattern,dib,gap,reserved);File.WriteAllBytes(path,bytes);
        Progress(output,label,"load1");Loaded first=Load(load,path);Progress(output,label,"load2");Loaded second=Load(load,path);
        bool stable=Same(first,second);
        if(first.Rgba!=null)File.WriteAllBytes(Path.Combine(output,label+".rgba"),first.Rgba);
        if(second.Rgba!=null)File.WriteAllBytes(Path.Combine(output,label+".repeat.rgba"),second.Rgba);
        Check(Hash(bytes)==Hash(File.ReadAllBytes(path)),"Original changed source "+label);
        Check((first.Rgba!=null)==(first.Error=="")&&(second.Rgba!=null)==(second.Error==""),"Complete result/error records "+label);
        if(control){
            controls++;Check(stable&&first.Error==""&&first.Width==Pot(width)&&first.Height==Pot(height),"Known true-color BI_RGB control "+label+": "+first.Error+" / "+second.Error);
        }else{fixtures++;if(first.Error=="")decoded++;else rejected++;if(!stable)unstable++;}
        log.WriteLine((control?"control":"indexed")+"\t"+label+"\t"+dib+"\t"+depth+"\t"+width+"\t"+height+"\t"+top+"\t"+pattern+"\t"+gap+"\t"+reserved+"\t"+(dib==12?3:4)+"\t"+(depth<=8?1<<depth:0)+"\t"+Hash(bytes)+"\t"+Columns(first)+"\t"+Columns(second)+"\t"+stable);
        Progress(output,label,"case-completed");
    }
    static int Main(string[] args)
    {
        try{
            Check(IntPtr.Size==4,"x86 required");string input=Path.GetFullPath(args[0]),output=Path.GetFullPath(args[1]),directory=Path.GetDirectoryName(input);Directory.CreateDirectory(output);
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string path=Path.Combine(directory,new AssemblyName(e.Name).Name+".dll");return File.Exists(path)?Assembly.LoadFrom(path):null;};
            typeof(TexturePngOracle).GetMethod("Bind",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{Assembly.LoadFrom(input),input});var load=typeof(TexturePngOracle).GetMethod("Original",BindingFlags.Static|BindingFlags.NonPublic);
            using(var log=new StreamWriter(Path.Combine(output,"transcript.tsv"))){
                log.AutoFlush=true;log.WriteLine("kind\tlabel\tdib\tdepth\tsourceWidth\tsourceHeight\ttopDown\tpattern\tgap\tpaletteReserved\tpaletteEntryBytes\tpaletteEntries\tinputSha256\twidth\theight\thasAlpha\tbytes\tsha256\terror\trepeatWidth\trepeatHeight\trepeatHasAlpha\trepeatBytes\trepeatSha256\trepeatError\toriginalStable");
                foreach(int[] size in new[]{new[]{16,8},new[]{7,5},new[]{1,1}}){
                    foreach(int dib in new[]{40,52,56,108,124})foreach(int depth in new[]{16,24,32})foreach(bool top in new[]{false,true})Case(load,log,output,size[0],size[1],depth,top,0,dib,0,0,true);
                    foreach(int dib in new[]{12,64})Case(load,log,output,size[0],size[1],24,false,0,dib,0,0,true);
                }
                foreach(int dib in new[]{12,40,52,56,64,108,124})foreach(int depth in new[]{1,4,8})foreach(int[] size in new[]{new[]{16,8},new[]{7,5},new[]{1,1}})foreach(bool top in dib==12?new[]{false}:new[]{false,true})foreach(int pattern in new[]{0,1,2})foreach(int gap in new[]{0,3,17})foreach(int reserved in dib==12?new[]{0}:new[]{0,255})Case(load,log,output,size[0],size[1],depth,top,pattern,dib,gap,reserved,false);
            }
            Check(fixtures==2025&&controls==96&&decoded+rejected==fixtures,"Complete original indexed-header corpus accounting");Progress(output,"all","completed");
            Console.WriteLine("PROBE Texture BMP indexed headers fixtures="+fixtures+" controls="+controls+" decoded="+decoded+" rejected="+rejected+" unstableOriginals="+unstable+" fixtureOriginalCalls="+((fixtures+controls)*2)+" initializationControlCalls=1 originalCalls="+((fixtures+controls)*2+1)+" checks="+checks+"; DIB/core/OS2 1/4/8-bit BI_RGB proper full palette, gap/reserved-alpha/POT/NPOT/orientation; full repeated original RGBA/errors; no recovered/plugin/SDK acceptance");return 0;
        }catch(Exception error){Console.Error.WriteLine(error);return 1;}
        finally{var owner=typeof(TexturePngOracle).GetField("owner",BindingFlags.Static|BindingFlags.NonPublic).GetValue(null) as IDisposable;if(owner!=null)owner.Dispose();}
    }
}
