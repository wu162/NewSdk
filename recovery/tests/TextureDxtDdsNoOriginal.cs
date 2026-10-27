using System;
using System.IO;
using System.Reflection;
using BinaryAssetBuilder.TextureCompiler;
internal static class TextureDxtDdsNoOriginal {
    private static int Main(string[] args){try{
        if(File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.TextureCompiler.dll")))throw new Exception("Original present");
        int fixtures=0,checks=0;
        using(var r=new BinaryReader(File.OpenRead(args[0]))){int count=r.ReadInt32();if(count!=3072)throw new Exception("Wrong corpus count");
            while(r.BaseStream.Position<r.BaseStream.Length){string format=r.ReadString();bool mipmaps=r.ReadBoolean();uint width=r.ReadUInt32(),height=r.ReadUInt32();int bytes=r.ReadInt32();byte[] rgba=r.ReadBytes(bytes);int outputBytes=r.ReadInt32();byte[] expected=r.ReadBytes(outputBytes);if(rgba.Length!=bytes||expected.Length!=outputBytes)throw new Exception("Truncated replay");
                byte[] actual=TextureDxtDdsEncoder.Encode(width,height,rgba,format,mipmaps);
                if(actual.Length!=expected.Length)throw new Exception("DDS size");
                for(int i=0;i<expected.Length;i++){checks++;if(actual[i]!=expected[i])throw new Exception("DDS replay mismatch "+fixtures+" byte="+i);}fixtures++;
            }if(fixtures!=count)throw new Exception("Replay count mismatch");
        }
        foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())if(a.GetName().Name=="BinaryAssetBuilder.TextureCompiler")throw new Exception("Original loaded");
        Console.WriteLine("PASS Texture 2D DDS NoOriginal fixtures="+fixtures+" checks="+checks+"; original DLL absent and not loaded");return 0;
    }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}

