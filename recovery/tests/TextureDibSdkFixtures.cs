using System;
using System.IO;

internal static class TextureDibSdkFixtures
{
    static int Main(string[] args)
    {
        try{
            string output=Path.GetFullPath(args[0]);Directory.CreateDirectory(output);
            int raw=0,transparent=0,atlas=0;
            foreach(var source in TextureDibConsumerFixtures.All(Path.GetFullPath(args[1]))){
                // Indexed original errors and invalid volume strips remain in
                // the component matrix, outside this positive SDK input set.
                if(source.Indexed||source.Label.Contains("-8x16-"))continue;
                foreach(string ext in new[]{"bmp","dib"}){
                    string name=source.Label+"-"+ext+"."+ext;
                    File.WriteAllBytes(Path.Combine(output,name),ext=="bmp"?source.Bmp:TextureDibConsumerFixtures.Bare(source.Bmp));
                    if(source.Atlas)atlas++;else if(source.Transparent)transparent++;else raw++;
                }
            }
            if(raw!=498||transparent!=166||atlas!=108)throw new Exception("Complete DIB SDK fixture families "+raw+"/"+transparent+"/"+atlas);
            Console.WriteLine("PASS DIB/BMP table SDK fixture generation raw="+raw+" transparent="+transparent+" atlas="+atlas);return 0;
        }catch(Exception error){Console.Error.WriteLine(error);return 1;}
    }
}
