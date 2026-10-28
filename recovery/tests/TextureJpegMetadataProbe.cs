using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Recovered.Compiler.Texture;

// Original evidence and current public IJG candidate comparison only. A zero
// exit code means the capture completed, not that a candidate was accepted.
internal static class TextureJpegMetadataProbe
{
    static int checks,fixtures,decoded,rejected,matches,differences;
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static string Hash(byte[] b){using(var s=SHA256.Create())return BitConverter.ToString(s.ComputeHash(b)).Replace("-","").ToLowerInvariant();}
    static string Error(Exception e){if(e==null)return "";while(e is TargetInvocationException&&e.InnerException!=null)e=e.InnerException;var p=e.GetType().GetProperty("ErrorCode");return (e.GetType().FullName+"/"+(p==null?"":Convert.ToString(p.GetValue(e,null)))+"/"+e.Message).Replace('\t',' ').Replace('\r',' ').Replace('\n',' ');}
    static byte[] Bytes(TexturePacker.Image image){byte[] b=new byte[image.Pixels.Length*4];Buffer.BlockCopy(image.Pixels,0,b,0,b.Length);return b;}
    static byte[] Segment(int marker,byte[] payload){Check(payload.Length<65534,"JPEG segment capacity");byte[] b=new byte[payload.Length+4];b[0]=255;b[1]=(byte)marker;int n=payload.Length+2;b[2]=(byte)(n>>8);b[3]=(byte)n;Buffer.BlockCopy(payload,0,b,4,payload.Length);return b;}
    static byte[] Join(params byte[][] rows){return TexturePngMetadataFixtures.Join(rows);}
    static byte[] Insert(byte[] jpg,byte[] segment){byte[] b=new byte[jpg.Length+segment.Length];Buffer.BlockCopy(jpg,0,b,0,2);Buffer.BlockCopy(segment,0,b,2,segment.Length);Buffer.BlockCopy(jpg,2,b,2+segment.Length,jpg.Length-2);return b;}
    static byte[] Adobe(byte[] jpg,int transform,bool remove){
        // Only rewrite the standard Adobe APP14 marker before SOS, preserving
        // all compressed scan bytes and all unrelated markers exactly.
        using(var s=new MemoryStream()){s.Write(jpg,0,2);int at=2;while(at<jpg.Length){
            Check(jpg[at]==255&&at+3<jpg.Length,"JPEG marker framing");int marker=jpg[at+1];if(marker==0xda||marker==0xd9){s.Write(jpg,at,jpg.Length-at);break;}
            int n=jpg[at+2]*256+jpg[at+3];Check(n>=2&&at+n+2<=jpg.Length,"JPEG marker extent");bool adobe=marker==0xee&&n>=14&&Encoding.ASCII.GetString(jpg,at+4,5)=="Adobe";
            if(!adobe||!remove){byte[] part=new byte[n+2];Buffer.BlockCopy(jpg,at,part,0,part.Length);if(adobe)part[15]=(byte)transform;s.Write(part,0,part.Length);}at+=n+2;
        }return s.ToArray();}
    }
    static byte[] Exif(int orientation){return Segment(0xe1,new byte[]{69,120,105,102,0,0,73,73,42,0,8,0,0,0,1,0,18,1,3,0,1,0,0,0,(byte)orientation,0,0,0,0,0,0,0});}
    static void Case(MethodInfo load,StreamWriter transcript,string output,string label,string family,byte[] source){
        string path=Path.Combine(output,label+".jpg");File.WriteAllBytes(path,source);TexturePacker.Image expected=null,again=null,candidate=null;Exception error=null,repeatedError=null,candidateError=null;
        try{expected=(TexturePacker.Image)load.Invoke(null,new object[]{path});}catch(Exception e){error=e;}
        try{again=(TexturePacker.Image)load.Invoke(null,new object[]{path});}catch(Exception e){repeatedError=e;}
        Check(Error(error)==Error(repeatedError),"Repeated original error "+label);byte[] rgba=expected==null?null:Bytes(expected);
        if(expected!=null){Check(again!=null&&again.Width==expected.Width&&again.Height==expected.Height&&again.HasAlpha==expected.HasAlpha,"Repeated original descriptor "+label);byte[] second=Bytes(again);Check(second.Length==rgba.Length,"Repeated original bytes");for(int i=0;i<rgba.Length;i++)Check(rgba[i]==second[i],"Repeated original pixel "+label+"/"+i);File.WriteAllBytes(Path.Combine(output,label+".rgba"),rgba);decoded++;}else{Check(error!=null,"Missing original result");rejected++;}
        try{candidate=TextureImageLoader.Decode(source);}catch(Exception e){candidateError=e;}
        byte[] candidateBytes=candidate==null?null:Bytes(candidate);if(candidateBytes!=null)File.WriteAllBytes(Path.Combine(output,label+".candidate.rgba"),candidateBytes);
        bool equal=Error(error)==Error(candidateError);
        if(expected!=null||candidate!=null){equal=equal&&expected!=null&&candidate!=null&&expected.Width==candidate.Width&&expected.Height==candidate.Height&&expected.HasAlpha==candidate.HasAlpha&&rgba.Length==candidateBytes.Length;if(equal)for(int i=0;i<rgba.Length;i++)if(rgba[i]!=candidateBytes[i]){equal=false;break;}}
        if(equal)matches++;else differences++;
        Check(Hash(source)==Hash(File.ReadAllBytes(path)),"Source unchanged "+label);
        transcript.WriteLine(label+"\t"+family+"\t"+(expected==null?0:expected.Width)+"\t"+(expected==null?0:expected.Height)+"\t"+(expected!=null&&expected.HasAlpha)+"\t"+(rgba==null?0:rgba.Length)+"\t"+(rgba==null?"":Hash(rgba))+"\t"+Error(error)+"\t"+Hash(source)+"\t"+equal+"\t"+Error(candidateError)+"\t"+(candidateBytes==null?"":Hash(candidateBytes)));fixtures++;
    }
    static int Main(string[] args){try{
        Check(IntPtr.Size==4,"x86 required");string input=Path.GetFullPath(args[0]),output=Path.GetFullPath(args[1]),encoder=Path.GetFullPath(args[2]),icc=Path.GetFullPath(args[3]);Directory.CreateDirectory(output);string directory=Path.GetDirectoryName(input);
        AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string p=Path.Combine(directory,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
        typeof(TexturePngOracle).GetMethod("Bind",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{Assembly.LoadFrom(input),input});var load=typeof(TexturePngOracle).GetMethod("Original",BindingFlags.Static|BindingFlags.NonPublic);
        using(var transcript=new StreamWriter(Path.Combine(output,"transcript.tsv"))){transcript.AutoFlush=true;transcript.WriteLine("label\tfamily\twidth\theight\thasAlpha\tbytes\tsha256\terror\tinputSha256\tcandidateEqual\tcandidateError\tcandidateSha256");
            foreach(int[] size in new[]{new[]{1,1},new[]{2,2},new[]{8,4},new[]{16,16},new[]{32,16},new[]{3,2},new[]{7,5},new[]{17,9}})foreach(int space in new[]{0,1,2})foreach(int quality in new[]{30,75,95,100})foreach(int progressive in new[]{0,1})foreach(int pattern in new[]{0,1,2,3}){
                string stem="s"+space+"-"+size[0]+"x"+size[1]+"-q"+quality+"-pr"+progressive+"-p"+pattern,path=Path.Combine(output,"encoded.jpg");var start=new ProcessStartInfo(encoder,"\""+path+"\" "+size[0]+" "+size[1]+" "+space+" "+quality+" "+progressive+" "+pattern);start.UseShellExecute=false;start.CreateNoWindow=true;start.RedirectStandardError=true;using(var process=Process.Start(start)){string message=process.StandardError.ReadToEnd();process.WaitForExit();Check(process.ExitCode==0,"IJG fixture encoder "+message);}byte[] jpg=File.ReadAllBytes(path);
                Case(load,transcript,output,stem+"-native","colour",jpg);
                Case(load,transcript,output,stem+"-no-adobe","colour",Adobe(jpg,0,true));
                foreach(int transform in new[]{0,1,2})Case(load,transcript,output,stem+"-adobe"+transform,"colour",Adobe(jpg,transform,false));
            }
            foreach(string file in Directory.GetFiles(Path.GetFullPath(args[4]),"*.jpg")){
                string stem=Path.GetFileNameWithoutExtension(file);byte[] jpg=File.ReadAllBytes(file);
                foreach(int orientation in new[]{1,3,6,8})Case(load,transcript,output,"metadata-"+stem+"-exif"+orientation,"metadata",Insert(jpg,Exif(orientation)));
                foreach(string name in new[]{"sRGB-v2-micro","sRGB-v4","AdobeCompat-v2","sGrey-v4"}){
                    byte[] profile=Convert.FromBase64String(File.ReadAllText(Path.Combine(icc,name+".icc.base64"))),header=Encoding.ASCII.GetBytes("ICC_PROFILE\0");
                    Case(load,transcript,output,"metadata-"+stem+"-icc-"+name,"metadata",Insert(jpg,Segment(0xe2,Join(header,new byte[]{1,1},profile))));
                }
                Case(load,transcript,output,"metadata-"+stem+"-xmp","metadata",Insert(jpg,Segment(0xe1,Encoding.UTF8.GetBytes("http://ns.adobe.com/xap/1.0/\0<x:xmpmeta xmlns:x='adobe:ns:meta/'/>"))));
                Case(load,transcript,output,"metadata-"+stem+"-comment","metadata",Insert(jpg,Segment(0xfe,Encoding.UTF8.GetBytes("TextureCompiler metadata evidence"))));
            }
        }
        Check(fixtures==4000,"Complete JPEG colour/metadata corpus");Check(matches+differences==fixtures&&decoded+rejected==fixtures,"Complete comparison accounting");Console.WriteLine("PROBE Texture JPEG colour/metadata fixtures="+fixtures+" decoded="+decoded+" rejected="+rejected+" matches="+matches+" differences="+differences+" originalCalls="+(fixtures*2)+" checks="+checks+"; full original/candidate bytes, not recovered/plugin/SDK acceptance");return 0;
    }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{var owner=typeof(TexturePngOracle).GetField("owner",BindingFlags.Static|BindingFlags.NonPublic).GetValue(null) as IDisposable;if(owner!=null)owner.Dispose();}}
}
