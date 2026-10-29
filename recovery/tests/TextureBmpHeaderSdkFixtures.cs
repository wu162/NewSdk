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
            int transparent=0,atlas=0,os2Raw=0,os2Transparent=0,os2Atlas=0;
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
            foreach(int dib in new[]{12,64})foreach(bool top in dib==12?new[]{false}:new[]{false,true})
            {
                foreach(int depth in new[]{16,24,32})
                {
                    if(depth==24&&!top)continue; // Already declared legacy OS2 profile.
                    foreach(int[] size in new[]{new[]{16,8},new[]{7,5},new[]{1,1}})foreach(int pattern in new[]{0,1,2})foreach(int gap in new[]{0,3,17})
                    {
                        string name="os2-h"+dib+"-d"+depth+"-"+size[0]+"x"+size[1]+"-top"+top+"-p"+pattern+"-g"+gap+".bmp";
                        File.WriteAllBytes(Path.Combine(output,name),TextureBmpOs2Fixtures.Create(size[0],size[1],depth,top,pattern,dib,gap,0));os2Raw++;
                    }
                    foreach(int gap in new[]{0,3,17})
                    {
                        string name="atlas-os2-h"+dib+"-d"+depth+"-top"+top+"-g"+gap+".bmp";
                        File.WriteAllBytes(Path.Combine(output,name),TextureBmpOs2Fixtures.Create(256,256,depth,top,0,dib,gap,0));os2Atlas++;
                    }
                }
                foreach(int[] size in new[]{new[]{16,8},new[]{7,5},new[]{1,1}})foreach(int gap in new[]{0,3,17})
                {
                    string name="transparent-os2-h"+dib+"-"+size[0]+"x"+size[1]+"-top"+top+"-g"+gap+".bmp";
                    File.WriteAllBytes(Path.Combine(output,name),TextureBmpOs2Fixtures.Transparent(size[0],size[1],top,dib,gap));os2Transparent++;
                }
            }
            if(os2Raw!=189||os2Transparent!=27||os2Atlas!=21)throw new Exception("Incomplete new OS2 SDK fixture families");
            Console.WriteLine("PASS BMP header SDK fixture generation transparent="+transparent+" atlas="+atlas+" os2Raw="+os2Raw+" os2Transparent="+os2Transparent+" os2Atlas="+os2Atlas);
            return 0;
        }
        catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}
