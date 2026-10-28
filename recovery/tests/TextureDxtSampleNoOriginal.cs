using System;
using System.IO;
using System.Reflection;
using BinaryAssetBuilder.TextureCompiler;
internal static class TextureDxtSampleNoOriginal {
    private static int Main(string[] args){try{
        if(File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.TextureCompiler.dll")))throw new Exception("Original present");
        int fixtures=0,checks=0;
        using(var r=new BinaryReader(File.OpenRead(args[0]))){int count=r.ReadInt32();if(count!=4752)throw new Exception("Wrong corpus count");
            while(r.BaseStream.Position<r.BaseStream.Length){string format=r.ReadString();int width=r.ReadInt32(),height=r.ReadInt32(),length=r.ReadInt32();if(length!=width*height*4||length>64)throw new Exception("Wrong sample count");float[] rgba=new float[length];for(int i=0;i<length;i++)rgba[i]=r.ReadSingle();byte[] expected=r.ReadBytes(16);if(expected.Length!=16)throw new Exception("Truncated replay");
                byte[] actual=TextureDxtSampleBlockEncoder.EncodeFloatBlock(rgba,width,height,format);
                if(actual.Length!=16)throw new Exception("Block size");
                for(int i=0;i<16;i++){checks++;if(actual[i]!=expected[i])throw new Exception("Sample replay mismatch "+fixtures+" byte="+i);}fixtures++;
            }if(fixtures!=count)throw new Exception("Replay count mismatch");
        }
        foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())if(a.GetName().Name=="BinaryAssetBuilder.TextureCompiler")throw new Exception("Original loaded");
        Console.WriteLine("PASS Texture DXT sample block NoOriginal fixtures="+fixtures+" checks="+checks+"; original DLL absent and not loaded");return 0;
    }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
