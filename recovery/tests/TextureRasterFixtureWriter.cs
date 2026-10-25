using System;
using System.IO;
internal static class TextureRasterFixtureWriter
{
    static int Main(string[] args)
    {
        try{
            string dir=Path.GetFullPath(args[0]);Directory.CreateDirectory(dir);int count=0;
            foreach(int color in new[]{0,2,4,6})foreach(int depth in new[]{1,2,4,8,16}){
                if((color!=0&&depth<8)||(color==4&&depth==16))continue;
                foreach(int interlace in new[]{0,1})foreach(int meta in new[]{0,1}){
                    // Gray16+tRNS is an original rejection, not an SDK success input.
                    int metadata=color==0&&depth==16&&meta==1?5:meta;
                    string name="png-c"+color+"-d"+depth+"-i"+interlace+"-m"+metadata+".png";
                    File.WriteAllBytes(Path.Combine(dir,name),TexturePngOracle.Fixture(8,4,depth,color,interlace,5,metadata,0));count++;
                }
            }
            foreach(int depth in new[]{16,24,32})foreach(bool top in new[]{false,true})foreach(int pattern in depth==32?new[]{0,1,2}:new[]{0}){
                string name="bmp-d"+depth+"-top"+top+"-p"+pattern+".bmp";
                File.WriteAllBytes(Path.Combine(dir,name),TextureBmpOracle.Fixture(8,4,depth,top,pattern,0,0));count++;
            }
            foreach(int profile in new[]{0,1,2})foreach(bool top in new[]{false,true})foreach(int pattern in new[]{0,1}){
                File.WriteAllBytes(Path.Combine(dir,"fields-"+profile+"-top"+top+"-p"+pattern+".bmp"),TextureBmpOracle.BitfieldsFixture(8,4,profile,top,pattern,0));count++;
            }
            Console.WriteLine("PASS Texture raster SDK fixture generation images="+count);return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}
