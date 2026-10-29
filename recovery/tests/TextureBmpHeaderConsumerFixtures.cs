using System;

// Shared generated sources for differential plugin and dedicated SDK validation.
// Unlike the immutable oracle corpus these inputs are compared live to the original.
internal static class TextureBmpHeaderConsumerFixtures
{
    internal static string[] Profiles(int dib)
    {
        if(dib==124)return new[]{"plain","gamma1","gamma22","srgb","calibrated","embedded-srgb2","embedded-srgb4","embedded-gray4"};
        if(dib==108)return new[]{"plain","gamma1","gamma22","srgb","calibrated"};
        return new[]{"plain"};
    }
    internal static byte[] Transparent(string icc,int width,int height,bool top,int dib,string profile)
    {
        byte[] image=TextureBmpHeaderFixtures.Create(icc,width,height,32,top,0,dib,0,profile,0);
        int start=checked((int)BitConverter.ToUInt32(image,10)),stride=width*4;
        // Nonzero alpha avoids the original BMP all-zero-alpha -> opaque rule.
        // Every source pixel remains transparent across actual blocks/mips/slices.
        for(int y=0;y<height;y++)for(int x=0;x<width;x++)image[start+y*stride+x*4+3]=63;
        return image;
    }
    internal static byte[] Atlas(string icc,int depth,bool top,int dib,string profile)
    {return TextureBmpHeaderFixtures.Create(icc,256,256,depth,top,0,dib,0,profile,0);}
    internal static bool RefusedBitfields(string[] row)
    {return row[9]!="40"&&row[11]=="3";}
    internal static bool Selected(string[] row)
    {
        if(RefusedBitfields(row))return false;
        string[] label=row[0].Split('-');int dib=int.Parse(row[9]),depth=int.Parse(row[10]),gap=int.Parse(row[13]),pattern=int.Parse(label[4].Substring(1));
        if(pattern!=(dib+depth+row[1].Length+gap)%3)return false;
        return label[2]=="16x8"||label[2]=="7x5"||
            label[2]=="1x1"&&row[1]=="plain"&&row[11]=="0"&&gap==0||
            label[2]=="32x16"&&dib==124&&depth==32&&row[11]=="0"&&row[1]=="plain"&&gap==0;
    }
}
