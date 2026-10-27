using System;

namespace BinaryAssetBuilder.TextureCompiler
{
    // Original PC Production DXT3/5 2D no-mip DDS, complete aligned blocks only.
    public static class TextureDxtDdsEncoder
    {
        public static byte[] Encode(uint width, uint height, byte[] rgba, string format, bool generateMipMaps)
        {
            if (rgba == null) throw new ArgumentNullException("rgba");
            if (width < 4 || height < 4 || (width & (width-1)) != 0 || (height & (height-1)) != 0)
                throw new NotSupportedException("Compressed DDS requires power-of-two dimensions of at least 4.");
            ulong pixels = (ulong)width * height;
            if (pixels >= 16777216ul) throw new ArgumentOutOfRangeException("width", "Image exceeds the original safe RGBA range.");
            if ((ulong)rgba.LongLength != pixels * 4) throw new ArgumentException("RGBA storage must match dimensions.", "rgba");
            if (format != "D3DFMT_DXT3" && format != "D3DFMT_DXT5") throw new NotSupportedException("Only PC DXT3/DXT5 DDS is accepted.");
            if (generateMipMaps) throw new NotSupportedException("Compressed mip/partial-block output is not yet accepted.");
            byte[] result = new byte[checked(128 + (int)pixels)];
            Put(result,0,0x20534444);Put(result,4,124);Put(result,8,0x81007);
            Put(result,12,height);Put(result,16,width);Put(result,20,(uint)pixels);
            Put(result,76,32);Put(result,80,4);Put(result,84,format=="D3DFMT_DXT3"?0x33545844u:0x35545844u);Put(result,108,0x1000);
            byte[] block = new byte[64];int output=128;
            for(uint y=0;y<height;y+=4)for(uint x=0;x<width;x+=4)
            {
                for(uint row=0;row<4;row++)Buffer.BlockCopy(rgba,checked((int)(((y+row)*width+x)*4)),block,(int)row*16,16);
                byte[] encoded=TextureDxtBlockEncoder.EncodeBlock(block,format);
                Buffer.BlockCopy(encoded,0,result,output,16);output+=16;
            }
            return result;
        }
        private static void Put(byte[] data,int offset,uint value)
        {
            data[offset]=(byte)value;data[offset+1]=(byte)(value>>8);data[offset+2]=(byte)(value>>16);data[offset+3]=(byte)(value>>24);
        }
    }
}
