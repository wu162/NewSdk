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
            if (args.Length!=5) throw new ArgumentException("Usage: AudioMpegHeaderOracle candidate.dll native.tsv identifier.tsv info.tsv container.tsv");
            Assembly library=Assembly.LoadFrom(Path.GetFullPath(args[0]));
            Type type=library.GetType("BinaryAssetBuilder.AudioCompiler.MpegPassthrough",true);
            MethodInfo parse=type.GetMethod("ParseHeader",BindingFlags.Static|BindingFlags.NonPublic);
            MethodInfo find=type.GetMethod("FindFirstFrame",BindingFlags.Static|BindingFlags.NonPublic);
            MethodInfo info=type.GetMethod("Validate",BindingFlags.Static|BindingFlags.NonPublic);
            MethodInfo container=type.GetMethod("IsContainer",BindingFlags.Static|BindingFlags.NonPublic);
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
            int identifiers=0;
            foreach (string line in File.ReadLines(args[2])) {
                string[] fields=line.Split('\t');
                int actual=(int)find.Invoke(null,new object[] {Convert.FromBase64String(fields[2])});
                if (actual!=int.Parse(fields[1])) throw new Exception("Original MPEG identifier mismatch: "+fields[0]+" expected="+fields[1]+" actual="+actual);
                identifiers++;
            }
            if (identifiers!=441) throw new Exception("Incomplete original identifier matrix: "+identifiers);
            int infos=0;
            foreach (string line in File.ReadLines(args[3])) {
                string[] fields=line.Split('\t');
                byte actual=(byte)info.Invoke(null,new object[] {Convert.FromBase64String(fields[3]),int.Parse(fields[1])});
                if (actual!=byte.Parse(fields[2])) throw new Exception("Original SIMEX_info channel mismatch: "+fields[0]);
                infos++;
            }
            if (infos!=324) throw new Exception("Incomplete original SIMEX_info matrix: "+infos);
            int containers=0;
            foreach (string line in File.ReadLines(args[4])) {
                string[] fields=line.Split('\t');
                bool actual=(bool)container.Invoke(null,new object[] {Convert.FromBase64String(fields[2])});
                if (actual!=(fields[1]=="1")) throw new Exception("Original container priority mismatch: "+fields[0]);
                containers++;
            }
            if (containers!=72) throw new Exception("Incomplete original container matrix: "+containers);
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
            Console.WriteLine("PASS MPEG header: "+count+" native return/channel records; "+identifiers+" native identifier records; "+infos+" native SIMEX_info records; "+containers+" native container records; "+checks+" identification boundaries"); return 0;
        } catch (Exception error) { Console.Error.WriteLine(error);return 1; }
    }
}
