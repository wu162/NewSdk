using System;
using System.IO;
internal static class TextureDxtSdkFixtures
{
    static byte[] Tga(int width,int height,int pattern)
    {
        byte[] b=new byte[18+width*height*4];b[2]=2;b[12]=(byte)width;b[13]=(byte)(width>>8);b[14]=(byte)height;b[15]=(byte)(height>>8);b[16]=32;b[17]=40;
        for(int i=0;i<width*height;i++){int at=18+i*4;b[at]=(byte)(pattern==0?0:pattern==1?255:i*31);b[at+1]=(byte)(pattern==0?0:pattern==1?255:i*43+11);b[at+2]=(byte)(pattern==0?0:pattern==1?255:i*7+171);b[at+3]=(byte)(pattern==0?0:pattern==1?255:pattern==2?63:pattern==3?(i%2)*255:i*17);}
        if(pattern>=5){byte[] pixels=new byte[width*height*4];new Random(pattern*7919+width*31+height).NextBytes(pixels);Buffer.BlockCopy(pixels,0,b,18,pixels.Length);}return b;
    }
    static int Main(string[] args)
    {
        try{
            string dir=Path.GetFullPath(args[0]);Directory.CreateDirectory(dir);int count=0;
            foreach(int[] size in new[]{new[]{4,4},new[]{4,8},new[]{8,4},new[]{8,8},new[]{8,16},new[]{16,8},new[]{32,32}})foreach(int pattern in new[]{0,1,2,3,4,5,6,7}){
                File.WriteAllBytes(Path.Combine(dir,"tga-"+size[0]+"x"+size[1]+"-p"+pattern+".tga"),Tga(size[0],size[1],pattern));count++;
            }
            foreach(int color in new[]{0,2,4,6})foreach(int depth in new[]{1,2,4,8,16}){
                if((color==3?depth==16:color!=0&&depth<8)||(color==4&&depth==16))continue;
                foreach(int interlace in new[]{0,1})foreach(int meta in new[]{0,1}){
                    int metadata=color==0&&depth==16&&meta==1?5:meta;
                    File.WriteAllBytes(Path.Combine(dir,"png-c"+color+"-d"+depth+"-i"+interlace+"-m"+metadata+".png"),TexturePngOracle.Fixture(8,4,depth,color,interlace,5,metadata,0));count++;
                }
            }
            foreach(int depth in new[]{16,24,32})foreach(bool top in new[]{false,true})foreach(int pattern in new[]{0,1,2}){
                File.WriteAllBytes(Path.Combine(dir,"bmp-d"+depth+"-top"+top+"-p"+pattern+".bmp"),TextureBmpOracle.Fixture(8,4,depth,top,pattern,0,7));count++;
            }
            foreach(int profile in new[]{0,1,2})foreach(bool top in new[]{false,true}){
                File.WriteAllBytes(Path.Combine(dir,"bmp-fields"+profile+"-top"+top+".bmp"),TextureBmpOracle.BitfieldsFixture(8,4,profile,top,0,7));count++;
            }
            if(count!=120)throw new Exception("Fixture count mismatch "+count);
            Console.WriteLine("PASS Texture DXT SDK fixtures="+count+"; deterministic aligned PC TGA/PNG/BMP");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}
