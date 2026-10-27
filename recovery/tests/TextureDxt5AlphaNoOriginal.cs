using System;
using System.IO;
using System.Reflection;
using BinaryAssetBuilder.TextureCompiler;

internal static class TextureDxt5AlphaNoOriginal
{
    private static int Main(string[] args)
    {
        try {
            string dir=AppDomain.CurrentDomain.BaseDirectory;
            if(File.Exists(Path.Combine(dir,"BinaryAssetBuilder.TextureCompiler.dll"))) throw new Exception("Original DLL present in isolated directory");
            int fixtures=0,checks=0;
            using(var input=File.OpenRead(args[0])) using(var reader=new BinaryReader(input)) {
                while(input.Position<input.Length) {
                    byte[] rgba=reader.ReadBytes(64),expected=reader.ReadBytes(8);
                    if(rgba.Length!=64||expected.Length!=8) throw new Exception("Truncated original replay record");
                    byte[] actual=TextureDxt5AlphaEncoder.EncodeBlock(rgba);
                    if(actual.Length!=8) throw new Exception("Wrong output size");
                    for(int i=0;i<8;i++){checks++;if(actual[i]!=expected[i])throw new Exception("Isolated replay mismatch fixture="+fixtures+" byte="+i);}
                    fixtures++;
                }
            }
            if(fixtures!=Int32.Parse(args[1])) throw new Exception("Replay count mismatch: "+fixtures);
            foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                if(a.GetName().Name=="BinaryAssetBuilder.TextureCompiler")throw new Exception("Original DLL loaded");
            Console.WriteLine("PASS Texture DXT5 alpha NoOriginal fixtures="+fixtures+" checks="+checks+"; original DLL absent and not loaded");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}
