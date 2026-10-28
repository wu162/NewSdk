using System;
using System.IO;

internal static class TexturePngMetadataSdkFixtures
{
    static int Main(string[] args){try{
        string directory=Path.GetFullPath(args[0]),profiles=Path.GetFullPath(args[1]);Directory.CreateDirectory(directory);int transparent=0,atlas=0;
        foreach(string[] family in new[]{TexturePngMetadataFixtures.Profiles,TexturePngMetadataFixtures.OrderProfiles})foreach(string profile in family)foreach(int[] size in new[]{new[]{16,8},new[]{7,5}}){
            File.WriteAllBytes(Path.Combine(directory,"transparent-"+size[0]+"x"+size[1]+"-"+profile+".png"),TexturePngMetadataFixtures.Create(profiles,size[0],size[1],6,8,0,1,profile));transparent++;
        }
        foreach(string profile in new[]{"icc-rgb2","icc-gray4","icc-adobe","apng-first","apng-fallback","apng-icc-gamma","srgb-first","srgb-last"}){
            // The native atlas resolves the declared TGA path, then decodes by
            // signature. This is the same content dispatch as the JPEG fixture.
            File.WriteAllBytes(Path.Combine(directory,"atlas-"+profile+".tga"),TexturePngMetadataFixtures.Create(profiles,256,256,6,8,0,0,profile));atlas++;
        }
        if(transparent!=88||atlas!=8)throw new Exception("Incomplete PNG metadata SDK fixtures");Console.WriteLine("PASS PNG metadata SDK generated transparent="+transparent+" atlas="+atlas);return 0;
    }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
