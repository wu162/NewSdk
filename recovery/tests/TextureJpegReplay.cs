using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Recovered.Compiler.Texture;
internal static class TextureJpegReplay
{
    static int fixtures,decoded,rejected,checks;static long compared;
    static void Check(bool ok,string label){checks++;if(!ok)throw new Exception(label);}
    static string Hash(byte[] b){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(b)).Replace("-","").ToLowerInvariant();}
    static string Error(Exception e){if(e==null)return "";var code=e.GetType().GetProperty("ErrorCode");return (e.GetType().FullName+"/"+(code==null?"":Convert.ToString(code.GetValue(e,null)))+"/"+e.Message).Replace('\t',' ').Replace('\r',' ').Replace('\n',' ');}
    static void Absent(){Check(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.TextureCompiler.dll")),"Original present");foreach(var a in AppDomain.CurrentDomain.GetAssemblies())Check(a.GetName().Name!="BinaryAssetBuilder.TextureCompiler","Original loaded");}
    static int Main(string[] args){try{Check(IntPtr.Size==4,"x86 required");string dir=Path.GetFullPath(args[0]);foreach(string line in File.ReadAllLines(Path.Combine(dir,"transcript.tsv"))){if(line.StartsWith("label\t"))continue;var row=line.Split('\t');string path=Path.Combine(dir,row[0]+".jpg");byte[] source=File.ReadAllBytes(path);TexturePacker.Image image=null;Exception error=null;Absent();try{image=TextureImageLoader.Load(path);}catch(Exception e){error=e;}Absent();Check(Hash(File.ReadAllBytes(path))==Hash(source),"Source mutation "+row[0]);Check(Error(error)==row[9],row[0]+" expected error="+row[9]+" actual="+Error(error));
      if(error==null){Check(image!=null&&image.Width==int.Parse(row[4])&&image.Height==int.Parse(row[5])&&image.HasAlpha==bool.Parse(row[6]),"Image metadata "+row[0]);byte[] expected=File.ReadAllBytes(Path.Combine(dir,row[0]+".loaded.rgba"));byte[] actual=new byte[image.Pixels.Length*4];Buffer.BlockCopy(image.Pixels,0,actual,0,actual.Length);Check(Hash(expected)==row[8],"Original RGBA hash "+row[0]);Check(actual.Length==expected.Length,"RGBA length "+row[0]);for(int i=0;i<actual.Length;i++)if(actual[i]!=expected[i])throw new Exception(row[0]+" byte="+i+" original="+expected[i]+" recovered="+actual[i]);checks++;compared+=actual.Length;decoded++;}else{Check(image==null,"Failed image result");rejected++;}fixtures++;
    }Check(fixtures==1284,"Full JPEG corpus");Console.WriteLine("PASS Texture JPEG public image loader fixtures="+fixtures+" decoded="+decoded+" rejected="+rejected+" checks="+checks+" comparedBytes="+compared+"; complete immutable original RGBA/metadata/errors, source invariance, per-call original absent and not loaded");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
