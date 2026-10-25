using System;
using System.IO;
using System.Reflection;

// The expected values come from the original DLL's executed x86 instructions,
// not from a second implementation of the recovered bitrate/header formulas.
internal static class AudioMpegHeaderOracle
{
    private static int Main(string[] args)
    {
        try {
            if (args.Length!=2) throw new ArgumentException("Usage: AudioMpegHeaderOracle candidate.dll native.tsv");
            Assembly library=Assembly.LoadFrom(Path.GetFullPath(args[0]));
            Type type=library.GetType("BinaryAssetBuilder.AudioCompiler.MpegPassthrough",true);
            MethodInfo parse=type.GetMethod("ParseHeader",BindingFlags.Static|BindingFlags.NonPublic);
            MethodInfo find=type.GetMethod("FindFirstFrame",BindingFlags.Static|BindingFlags.NonPublic);
            int count=0;
            foreach (string line in File.ReadLines(args[1])) {
                string[] fields=line.Split('\t');
                uint word=Convert.ToUInt32(fields[0],16);
                byte[] data={ (byte)(word>>24),(byte)(word>>16),(byte)(word>>8),(byte)word };
                object[] call={data,0,(byte)0xa5};
                int result=(int)parse.Invoke(null,call);
                if (result!=int.Parse(fields[1]) || (byte)call[2]!=byte.Parse(fields[2]))
                    throw new Exception("Original MPEG header mismatch: "+fields[0]+" result="+result+" channels="+call[2]);
                count++;
            }
            if (count!=32783) throw new Exception("Incomplete native header matrix: "+count);
            byte[] header={255,251,80,196}; object[] valid={header,0,(byte)0};
            int stride=4+(int)parse.Invoke(null,valid);
            int checks=0;
            foreach (int prefix in new[] {0,1,127}) foreach (int frames in new[] {0,1,6,7,8}) {
                // Identification requires seven headers, including the seventh
                // four-byte header but no bytes from its payload.
                byte[] data=new byte[prefix+(frames==0?0:(frames-1)*stride+4)];
                for (int n=0;n<frames;n++) Array.Copy(header,0,data,prefix+n*stride,4);
                int actual=(int)find.Invoke(null,new object[] {data});
                int expected=frames>=7?prefix:-1;
                if (actual!=expected) throw new Exception("MPEG seven-header boundary mismatch");
                checks++;
            }
            Console.WriteLine("PASS MPEG header: "+count+" native return/channel records; "+checks+" identification boundaries"); return 0;
        } catch (Exception error) { Console.Error.WriteLine(error);return 1; }
    }
}
