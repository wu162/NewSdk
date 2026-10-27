using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Recovered.Compiler.Texture;
internal static class TextureNpotStorageReplay
{
    static int fixtures,decoded,rejected,checks;
    static void Check(bool v,string s){checks++;if(!v)throw new Exception(s);}
    static void Isolated(){Check(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.TextureCompiler.dll")),"Original DLL present");foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())Check(a.GetName().Name!="BinaryAssetBuilder.TextureCompiler","Original assembly loaded");}
    static string Hash(byte[] b){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(b)).Replace("-","").ToLowerInvariant();}
    static int Main(string[] args)
    {
        try{
            Check(IntPtr.Size==4,"x86 required");string directory=Path.GetFullPath(args[0]);var rows=File.ReadAllLines(Path.Combine(directory,"transcript.tsv"));Check(rows.Length==4987,"Full immutable storage corpus");
            foreach(string line in rows){if(line.StartsWith("label\t")||line.Length==0)continue;string[] f=line.Split('\t');string label=f[0],extension=f[1].StartsWith("png-")?"png":f[1].StartsWith("bmp-")?"bmp":"tga",path=Path.Combine(directory,label+"."+extension);byte[] source=File.ReadAllBytes(path);string before=Hash(source);TexturePacker.Image actual=null;Exception error=null;
                Isolated();try{actual=TextureImageLoader.Decode(source);}catch(Exception e){error=e;}Isolated();
                Check(before==Hash(source),"Source modified "+label);
                if(f[16].Length!=0){Check(error!=null,"Expected original rejection "+label);var code=error.GetType().GetProperty("ErrorCode");Check(error.GetType().FullName=="BinaryAssetBuilder.BinaryAssetBuilderException"&&code!=null&&Convert.ToString(code.GetValue(error,null))=="BadTextureFile","Original BadTextureFile domain "+label+": "+error);rejected++;}
                else{
                    Check(error==null,"Unexpected error "+label+": "+error);Check(actual!=null,"Null load "+label);Check(actual.Width==int.Parse(f[12])&&actual.Height==int.Parse(f[13])&&actual.HasAlpha==bool.Parse(f[14]),"Dimensions/alpha "+label);
                    byte[] expected=File.ReadAllBytes(Path.Combine(directory,label+".loaded.rgba"));Check(Hash(expected)==f[15],"Immutable RGBA hash "+label);Check(expected.Length==actual.Pixels.Length*4,"Pixel count "+label);
                    for(int i=0;i<actual.Pixels.Length;i++)Check(actual.Pixels[i]==BitConverter.ToUInt32(expected,i*4),label+" pixel="+i+" original="+BitConverter.ToUInt32(expected,i*4).ToString("X8")+" recovered="+actual.Pixels[i].ToString("X8"));decoded++;
                }fixtures++;
            }
            Check(fixtures==4986&&decoded==4446&&rejected==540,"Full storage counts");Console.WriteLine("PASS Texture NPOT/POT storage public decoder fixtures="+fixtures+" decoded="+decoded+" rejected="+rejected+" checks="+checks+"; full original RGBA buffers and original BadTextureFile domains; original absent/unloaded per call; source unchanged; plugin/SDK acceptance separate");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}
