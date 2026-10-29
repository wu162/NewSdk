using System;
using System.IO;

internal static class TextureTgaMetadataSdkFixtures
{
    static int Main(string[] args){try{
        string output=Path.GetFullPath(args[0]);Directory.CreateDirectory(output);int transparent=0,atlas=0;
        foreach(string format in new[]{"rgba32","rgba32-noattr","index32"})foreach(string profile in TextureTgaMetadataFixtures.Profiles)foreach(int[] size in new[]{new[]{16,8},new[]{7,5}}){
            File.WriteAllBytes(Path.Combine(output,"transparent-"+format+"-"+size[0]+"x"+size[1]+"-"+profile+".tga"),TextureTgaMetadataFixtures.Create(size[0],size[1],format,true,32,3,profile));transparent++;
        }
        foreach(string format in new[]{"rgb24","index32"})for(int p=0;p<TextureTgaMetadataFixtures.Profiles.Length;p++){
            string profile=TextureTgaMetadataFixtures.Profiles[p];File.WriteAllBytes(Path.Combine(output,"atlas-"+format+"-"+profile+".tga"),TextureTgaMetadataFixtures.Create(256,256,format,(p&1)==0,32,0,profile));atlas++;
        }
        if(transparent!=156||atlas!=52)throw new Exception("Incomplete TGA metadata SDK fixture families");Console.WriteLine("PASS TGA metadata SDK fixture generation transparent="+transparent+" atlas="+atlas);return 0;
    }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
