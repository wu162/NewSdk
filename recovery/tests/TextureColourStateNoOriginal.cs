using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
internal static unsafe class TextureColourStateNoOriginal
{
    [DllImport("kernel32.dll",CharSet=CharSet.Ansi)]private static extern IntPtr LoadLibrary(string path);
    [DllImport("kernel32.dll",CharSet=CharSet.Ansi)]private static extern IntPtr GetProcAddress(IntPtr module,string name);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]private delegate int Encoder(IntPtr block,int length,int mode);
    private static int Main(string[] args){try{
        if(File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.TextureCompiler.dll")))throw new Exception("Original present");
        IntPtr module=LoadLibrary(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Recovered.TextureDxtColour.dll"));if(module==IntPtr.Zero)throw new Exception("Owned DLL missing");
        Encoder encode=(Encoder)Marshal.GetDelegateForFunctionPointer(GetProcAddress(module,"TextureColorState"),typeof(Encoder));
        int count=0,checks=0;
        using(var reader=new BinaryReader(File.OpenRead(args[0]))){int expectedCount=reader.ReadInt32();if(expectedCount!=2019)throw new Exception("Wrong corpus count");
            while(reader.BaseStream.Position<reader.BaseStream.Length){int mode=reader.ReadInt32();byte[] input=reader.ReadBytes(0x3850),expected=reader.ReadBytes(0x3850);if(input.Length!=0x3850||expected.Length!=0x3850)throw new Exception("Truncated corpus");
                fixed(byte* p=input)if(encode(new IntPtr(p),input.Length,mode)!=0)throw new Exception("Encode status");
                for(int i=0;i<input.Length;i++){checks++;if(input[i]!=expected[i])throw new Exception("NoOriginal mismatch case="+count+" byte="+i);}count++;
            }if(count!=expectedCount)throw new Exception("Replay count mismatch");
        }
        foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())if(a.GetName().Name=="BinaryAssetBuilder.TextureCompiler")throw new Exception("Original loaded");
        Console.WriteLine("PASS Texture Production colour NoOriginal fixtures="+count+" checks="+checks+"; original DLL absent and not loaded");return 0;
    }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
