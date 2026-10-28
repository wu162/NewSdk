using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Recovered.Compiler.Texture;

internal static class TexturePngMetadataReplay
{
    static int checks,fixtures,decoded,rejected;static long compared;
    static void Check(bool v,string s){checks++;if(!v)throw new Exception(s);}
    static void Isolated(){Check(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.TextureCompiler.dll")),"Original DLL present");foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())Check(a.GetName().Name!="BinaryAssetBuilder.TextureCompiler","Original loaded");}
    static string Hash(byte[] b){using(var s=SHA256.Create())return BitConverter.ToString(s.ComputeHash(b)).Replace("-","").ToLowerInvariant();}
    static string Error(Exception e){if(e==null)return "";var p=e.GetType().GetProperty("ErrorCode");return (e.GetType().FullName+"/"+(p==null?"":Convert.ToString(p.GetValue(e,null)))+"/"+e.Message).Replace('\t',' ').Replace('\r',' ').Replace('\n',' ');}
    static int Main(string[] args){try{
        Check(IntPtr.Size==4,"x86 required");int count=int.Parse(args[1]);Check(count==4680||count==3240,"Known immutable corpus");string directory=Path.GetFullPath(args[0]);string[] lines=File.ReadAllLines(Path.Combine(directory,"transcript.tsv"));Check(lines.Length==count+1,"Complete immutable corpus");var labels=new HashSet<string>();
        foreach(string line in lines){if(line.StartsWith("label\t"))continue;string[] f=line.Split('\t');Check(f.Length==9&&labels.Add(f[0]),"Unique complete row");string label=f[0],path=Path.Combine(directory,label+".png");byte[] source=File.ReadAllBytes(path);Check(Hash(source)==f[8],"Immutable source "+label);
            foreach(bool dispatch in new[]{false,true}){
                TexturePacker.Image actual=null;Exception error=null;Isolated();try{actual=dispatch?TextureImageLoader.Decode(source):TexturePngDecoder.Decode(source);}catch(Exception e){error=e;}Isolated();
                Check(Hash(source)==f[8]&&Hash(File.ReadAllBytes(path))==f[8],"Source changed "+label);
                Check(Error(error)==f[7],"Original error "+label+" public="+dispatch+" expected="+f[7]+" actual="+Error(error));
                if(f[7].Length!=0){Check(actual==null,"Error returned image");rejected++;continue;}
                Check(actual!=null&&actual.Width==int.Parse(f[2])&&actual.Height==int.Parse(f[3])&&actual.HasAlpha==bool.Parse(f[4]),"Dimensions/alpha "+label);
                byte[] expected=File.ReadAllBytes(Path.Combine(directory,label+".rgba"));Check(expected.Length==int.Parse(f[5])&&Hash(expected)==f[6],"Immutable RGBA "+label);Check(actual.Pixels.Length*4==expected.Length,"Full pixel length "+label);
                for(int i=0;i<actual.Pixels.Length;i++)Check(actual.Pixels[i]==BitConverter.ToUInt32(expected,i*4),"Full RGBA "+label+"/"+i+" expected="+BitConverter.ToUInt32(expected,i*4).ToString("X8")+" actual="+actual.Pixels[i].ToString("X8"));compared+=expected.Length;decoded++;
            }fixtures++;
        }
        Check(fixtures==count&&decoded+rejected==count*2,"Complete dual-entry accounting");Console.WriteLine("PASS Texture PNG metadata NoOriginal fixtures="+fixtures+" publicCalls="+(fixtures*2)+" decoded="+decoded+" rejected="+rejected+" checks="+checks+" comparedBytes="+compared+"; full original RGBA/error domains, unchanged inputs, absent/unloaded original per call; plugin/SDK separate");return 0;
    }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
