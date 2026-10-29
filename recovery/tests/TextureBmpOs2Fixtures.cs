using System;
using System.IO;

internal static class TextureBmpOs2Fixtures
{
    internal const int DirectCount=1944;
    static void Word(byte[] b,int at,int value){b[at]=(byte)value;b[at+1]=(byte)(value>>8);}
    static void Dword(byte[] b,int at,uint value){for(int i=0;i<4;i++)b[at+i]=(byte)(value>>(i*8));}
    internal static byte[] Create(int width,int height,int depth,bool top,int pattern,int dib,int gap,int reserved)
    {
        if(dib!=12&&dib!=64)throw new ArgumentException("OS/2 DIB required");
        if(dib==12&&top)throw new ArgumentException("Core height is unsigned");
        byte[] basic=TextureBmpOracle.Fixture(width,height,depth,top,pattern,reserved,0);
        int entries=depth<=8?1<<depth:0,paletteBytes=dib==12?3:4;
        int sourcePixels=54+entries*4,offset=14+dib+entries*paletteBytes+gap;
        byte[] data=new byte[offset+basic.Length-sourcePixels];Buffer.BlockCopy(basic,0,data,0,14);
        Dword(data,2,(uint)data.Length);Dword(data,10,(uint)offset);
        if(dib==12){Dword(data,14,12);Word(data,18,width);Word(data,20,height);Word(data,22,1);Word(data,24,depth);}
        else{Buffer.BlockCopy(basic,14,data,14,40);Dword(data,14,64);}
        for(int i=0;i<entries;i++)Buffer.BlockCopy(basic,54+i*4,data,14+dib+i*paletteBytes,paletteBytes);
        for(int i=0;i<gap;i++)data[14+dib+entries*paletteBytes+i]=(byte)(i*17+111);
        Buffer.BlockCopy(basic,sourcePixels,data,offset,basic.Length-sourcePixels);
        return data;
    }
    internal static byte[] Transparent(int width,int height,bool top,int dib,int gap)
    {
        byte[] data=Create(width,height,32,top,0,dib,gap,0);
        // Every source alpha <=.5, with at least one nonzero byte, including
        // partial blocks/1x1. Original all-zero-alpha normalization cannot turn
        // this into an opaque DXT1A block; every mip stays in the declared domain.
        for(int y=0;y<height;y++)for(int x=0;x<width;x++)data[14+dib+y*width*4+x*4+3]=(byte)((x+y)%2==0?64:0);
        return data;
    }
    internal static readonly int[][] Sizes={new[]{1,1},new[]{2,1},new[]{4,2},new[]{8,4},new[]{16,8},new[]{7,5},new[]{5,7},new[]{32,16}};
}
