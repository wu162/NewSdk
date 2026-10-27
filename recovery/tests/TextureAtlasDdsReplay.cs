using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Recovered.Compiler.Texture;
internal static class TextureAtlasDdsReplay
{
    static int fixtures,checks,rejected;static long compared;
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static string Hash(byte[] b){using(var s=SHA256.Create())return BitConverter.ToString(s.ComputeHash(b)).Replace("-","").ToLowerInvariant();}
    static void Isolated(){Check(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.TextureCompiler.dll")),"Original DLL present");foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())Check(a.GetName().Name!="BinaryAssetBuilder.TextureCompiler","Original assembly loaded");}
    static void Reject(uint width,uint height,byte[] rgba,bool normal){Isolated();bool failed=false;try{TextureAtlasDdsEncoder.Encode(width,height,rgba,normal);}catch(ArgumentException){failed=true;}Isolated();Check(failed,"Unsafe atlas DDS input must reject");rejected++;}
    static int Main(string[] args)
    {
        try{
            Check(IntPtr.Size==4,"x86 required");string directory=Path.GetFullPath(args[0]);var rows=File.ReadAllLines(Path.Combine(directory,"transcript.tsv"));Check(rows.Length==481,"Immutable prerequisite corpus size");
            foreach(string row in rows){if(row.StartsWith("label\t")||row.Length==0)continue;string[] f=row.Split('\t');if(f[5]!="5"||(f[4]!="3"&&f[4]!="9"))continue;
                string label=f[0];byte[] rgba=File.ReadAllBytes(Path.Combine(directory,label+".rgba")),before=(byte[])rgba.Clone(),expected=File.ReadAllBytes(Path.Combine(directory,label+".dds"));Check(Hash(expected)==f[17],"Original DDS hash "+label);
                Isolated();byte[] actual=TextureAtlasDdsEncoder.Encode(uint.Parse(f[1]),uint.Parse(f[2]),rgba,f[4]=="9");Isolated();Check(actual.Length==expected.Length,"Full DDS length "+label+" original="+expected.Length+" recovered="+actual.Length);
                for(int i=0;i<actual.Length;i++)Check(actual[i]==expected[i],label+" byte="+i+" original="+expected[i]+" recovered="+actual[i]);compared+=actual.Length;Check(Hash(rgba)==Hash(before),"Source mutated "+label);
                int w=int.Parse(f[1]),h=int.Parse(f[2]),offset=128,levels=0;string[] calls=f[18].Split(';');Check(calls.Length==7,"Exactly five payload callbacks "+label);
                do{int size=f[4]=="9"?w*h*2:((w+3)/4)*((h+3)/4)*16;Check(calls[levels+2]==size+":"+levels+","+w+","+h+",0,1","Original callback/layout "+label+"/"+levels);offset+=size;w=Math.Max(1,w/2);h=Math.Max(1,h/2);levels++;}while(levels<5);
                Check(offset==actual.Length,"Full fixed-count layout "+label);fixtures++;
            }
            Check(fixtures==160,"Full format3/9 fixed-five-level original corpus");foreach(bool normal in new[]{false,true}){Reject(0,1,new byte[0],normal);Reject(3,2,new byte[24],normal);Reject(4096,4096,new byte[0],normal);Reject(1,1,null,normal);Reject(1,1,new byte[3],normal);Reject(1,1,new byte[5],normal);}Check(rejected==12,"Full unsafe input domain");Console.WriteLine("PASS Texture PC atlas DDS prerequisite fixtures="+fixtures+" rejected="+rejected+" checks="+checks+" comparedBytes="+compared+"; complete original DXT5/RGB555 DDS with exactly five levels and repeated 1x1 tails; original absent/unloaded per call; source unchanged; full atlas/plugin/SDK separate");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}
