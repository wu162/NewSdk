using System;
using System.IO;
using System.Diagnostics;
using System.Text;
public static class TextureJpegFixtures
{
    static byte[] Ppm(int w,int h,bool noise){byte[] header=Encoding.ASCII.GetBytes("P6\n"+w+" "+h+"\n255\n");byte[] b=new byte[header.Length+w*h*3];Buffer.BlockCopy(header,0,b,0,header.Length);var random=new Random(7919+w*13+h*31);for(int i=header.Length;i<b.Length;i++)b[i]=(byte)(noise?random.Next(256):new[]{251,127,13}[(i-header.Length)%3]);return b;}
    public static string[] GenerateAtlas(string output,string encoder){Directory.CreateDirectory(output);foreach(bool noise in new[]{false,true})foreach(bool gray in new[]{false,true})foreach(bool progressive in new[]{false,true}){
        string prefix="atlas-"+(noise?"noise":"solid")+(gray?"-gray":"-420")+(progressive?"-progressive":"-sequential"),ppm=Path.Combine(output,prefix+".ppm"),jpg=Path.Combine(output,prefix+".tga");File.WriteAllBytes(ppm,Ppm(256,256,noise));
        string options="-quality 75 "+(gray?"-grayscale ":"-sample 2x2,1x1,1x1 ")+(progressive?"-progressive ":"")+"-outfile \""+jpg+"\" \""+ppm+"\"";
        var start=new ProcessStartInfo(encoder,options);start.UseShellExecute=false;start.CreateNoWindow=true;start.RedirectStandardError=true;using(var p=Process.Start(start)){string error=p.StandardError.ReadToEnd();p.WaitForExit();if(p.ExitCode!=0)throw new Exception("Atlas JPEG fixture failed: "+error);}File.Delete(ppm);
    }string[] files=Directory.GetFiles(output,"*.tga");Array.Sort(files,StringComparer.Ordinal);if(files.Length!=8)throw new Exception("Expected8 JPEG atlas fixtures");return files;}
    static int Main(string[] args){try{GenerateAtlas(Path.GetFullPath(args[0]),Path.GetFullPath(args[1]));return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
