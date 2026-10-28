using System;
using System.IO;

internal static class TextureDxt1aSdkFixtures
{
    internal static byte[] Tga(int width,int height,int pattern){
        byte[] result=new byte[18+width*height*4];result[2]=2;result[12]=(byte)width;result[13]=(byte)(width>>8);result[14]=(byte)height;result[15]=(byte)(height>>8);result[16]=32;result[17]=40;
        var random=new Random(pattern*7919+width*31+height);
        for(int y=0;y<height;y++)for(int x=0;x<width;x++){
            int at=18+(y*width+x)*4;
            for(int c=0;c<3;c++)result[at+c]=(byte)(pattern==0?0:pattern==1?255:pattern==2?128:pattern==3?(c+1)*63:random.Next(256));
            // Nonzero transparent alpha preserves TGA's declared alpha channel.
            // Sparse opaque samples exercise colour search while every generated
            // 4x4 block and complete mip chain retains actual transparency.
            result[at+3]=(byte)(pattern%8==0?63:(x+y)%4==0?255:63);
        }return result;
    }
    static int Main(string[] args){try{
        string dir=Path.GetFullPath(args[0]);Directory.CreateDirectory(dir);int count=0;
        foreach(int[] size in new[]{new[]{8,8},new[]{16,8},new[]{8,16},new[]{16,16}})for(int pattern=0;pattern<16;pattern++){
            File.WriteAllBytes(Path.Combine(dir,"std-"+size[0]+"x"+size[1]+"-p"+pattern+".tga"),Tga(size[0],size[1],pattern));count++;
        }
        for(int pattern=0;pattern<16;pattern++){File.WriteAllBytes(Path.Combine(dir,"vol-16x8-p"+pattern+".tga"),Tga(16,8,pattern));count++;}
        if(count!=80)throw new Exception("Incomplete transparent DXT1A SDK corpus");Console.WriteLine("PASS DXT1A SDK fixtures="+count);return 0;
    }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
