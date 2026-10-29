using System;
using System.IO;

internal static class TextureBmpHeaderSdkFixtures
{
    static int Main(string[] args)
    {
        try
        {
            string output=Path.GetFullPath(args[0]),icc=Path.GetFullPath(args[1]);
            Directory.CreateDirectory(output);
            int transparent=0,atlas=0;
            foreach(int dib in TextureBmpHeaderFixtures.WindowsDibs)
                foreach(string profile in TextureBmpHeaderConsumerFixtures.Profiles(dib))
                {
                    foreach(int[] size in new[]{new[]{16,8},new[]{7,5}})
                        foreach(bool top in new[]{false,true})
                        {
                            string name="transparent-h"+dib+"-"+size[0]+"x"+size[1]+"-top"+top+"-"+profile+".bmp";
                            File.WriteAllBytes(Path.Combine(output,name),TextureBmpHeaderConsumerFixtures.Transparent(icc,size[0],size[1],top,dib,profile));
                            transparent++;
                        }
                    foreach(int depth in new[]{16,24,32})
                    {
                        string name="atlas-h"+dib+"-d"+depth+"-"+profile+".bmp";
                        File.WriteAllBytes(Path.Combine(output,name),TextureBmpHeaderConsumerFixtures.Atlas(icc,depth,(dib+depth)%2==0,dib,profile));
                        atlas++;
                    }
                }
            foreach(int dib in new[]{12,64})
            {
                File.WriteAllBytes(Path.Combine(output,"atlas-h"+dib+"-d24-plain.bmp"),TextureBmpHeaderConsumerFixtures.Atlas(icc,24,false,dib,"plain"));
                atlas++;
            }
            if(transparent!=64||atlas!=50)throw new Exception("Incomplete BMP header SDK fixture families");
            Console.WriteLine("PASS BMP header SDK fixture generation transparent="+transparent+" atlas="+atlas);
            return 0;
        }
        catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}
