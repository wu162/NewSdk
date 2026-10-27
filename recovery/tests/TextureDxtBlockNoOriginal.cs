using System;
using System.IO;
using System.Reflection;
using BinaryAssetBuilder.TextureCompiler;
internal static class TextureDxtBlockNoOriginal {
    private static int Main(string[] args){try{
        if(File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.TextureCompiler.dll")))throw new Exception("Original present");
        int fixtures=0,checks=0;
        using(var r=new BinaryReader(File.OpenRead(args[0]))){int count=r.ReadInt32();if(count!=10240)throw new Exception("Wrong corpus count");
            while(r.BaseStream.Position<r.BaseStream.Length){string format=r.ReadString();byte[] rgba=r.ReadBytes(64),expected=r.ReadBytes(16);if(rgba.Length!=64||expected.Length!=16)throw new Exception("Truncated replay");
                byte[] actual=TextureDxtBlockEncoder.EncodeBlock(rgba,format);
                for(int i=0;i<16;i++){checks++;if(actual[i]!=expected[i])throw new Exception("Replay mismatch "+fixtures+" byte="+i);}fixtures++;
            }if(fixtures!=count)throw new Exception("Replay count mismatch");
        }
        foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())if(a.GetName().Name=="BinaryAssetBuilder.TextureCompiler")throw new Exception("Original loaded");
        Console.WriteLine("PASS Texture complete blocks NoOriginal fixtures="+fixtures+" checks="+checks+"; original DLL absent and not loaded");return 0;
    }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
