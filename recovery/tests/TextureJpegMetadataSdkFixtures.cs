using System;
using System.IO;

internal static class TextureJpegMetadataSdkFixtures
{
    static int Main(string[] args){try{
        string output=Path.GetFullPath(args[0]),encoder=Path.GetFullPath(args[1]),icc=Path.GetFullPath(args[2]);Directory.CreateDirectory(output);int count=0;
        foreach(string path in TextureJpegFixtures.GenerateAtlas(Path.Combine(output,"source"),encoder)){
            string name=Path.GetFileName(path);if(name!="atlas-noise-420-sequential.tga"&&name!="atlas-noise-gray-progressive.tga")continue;
            foreach(string profile in TextureJpegMetadataFixtures.Profiles){File.WriteAllBytes(Path.Combine(output,"metadata-"+Path.GetFileNameWithoutExtension(path)+"-"+profile+".tga"),TextureJpegMetadataFixtures.Create(icc,File.ReadAllBytes(path),profile));count++;}
        }
        if(count!=20)throw new Exception("Incomplete JPEG metadata atlas input set");Console.WriteLine("PASS JPEG metadata atlas fixture generation inputs="+count);return 0;
    }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
