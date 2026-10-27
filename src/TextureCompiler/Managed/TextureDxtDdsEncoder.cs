using System;
using Recovered.Compiler.Texture;

namespace BinaryAssetBuilder.TextureCompiler
{
    // Original PC Production DXT3/5 2D DDS and triangle mip chains, including partial power-of-two blocks.
    public static class TextureDxtDdsEncoder
    {
        public static byte[] Encode(uint width, uint height, byte[] rgba, string format, bool generateMipMaps)
        {
            if (rgba == null) throw new ArgumentNullException("rgba");
            if (width < 1 || height < 1 || (width & (width-1)) != 0 || (height & (height-1)) != 0)
                throw new NotSupportedException("Compressed DDS requires positive power-of-two dimensions.");
            ulong pixels = (ulong)width * height;
            if (pixels >= 16777216ul) throw new ArgumentOutOfRangeException("width", "Image exceeds the original safe RGBA range.");
            if ((ulong)rgba.LongLength != pixels * 4) throw new ArgumentException("RGBA storage must match dimensions.", "rgba");
            if (format != "D3DFMT_DXT3" && format != "D3DFMT_DXT5") throw new NotSupportedException("Only PC DXT3/DXT5 DDS is accepted.");
            int payload = checked((int)(((ulong)(width + 3) / 4) * ((height + 3) / 4) * 16));
            int mipCount = 1, levelWidth = (int)width, levelHeight = (int)height;
            long totalPayload = payload;
            if (generateMipMaps) while (levelWidth != 1 || levelHeight != 1)
            {
                levelWidth = Math.Max(1, levelWidth / 2); levelHeight = Math.Max(1, levelHeight / 2);
                totalPayload = checked(totalPayload + (long)((levelWidth + 3) / 4) * ((levelHeight + 3) / 4) * 16);
                mipCount++;
            }
            byte[] result = new byte[checked((int)(128 + totalPayload))];
            Put(result,0,0x20534444);Put(result,4,124);Put(result,8,generateMipMaps ? 0xa1007u : 0x81007u);
            Put(result,12,height);Put(result,16,width);Put(result,20,(uint)payload);
            Put(result,76,32);Put(result,80,4);Put(result,84,format=="D3DFMT_DXT3"?0x33545844u:0x35545844u);Put(result,108,generateMipMaps ? 0x401008u : 0x1000u);
            if (generateMipMaps) Put(result,28,(uint)mipCount);
            if (generateMipMaps)
            {
                WriteMipChain(result, (int)width, (int)height, rgba, format);
                return result;
            }
            int blockWidth=(int)Math.Min(4u,width),blockHeight=(int)Math.Min(4u,height);
            byte[] block = new byte[blockWidth*blockHeight*4];int output=128;
            for(uint y=0;y<height;y+=4)for(uint x=0;x<width;x+=4)
            {
                for(uint row=0;row<blockHeight;row++)Buffer.BlockCopy(rgba,checked((int)(((y+row)*width+x)*4)),block,(int)row*blockWidth*4,blockWidth*4);
                byte[] encoded=blockWidth==4&&blockHeight==4
                    ? TextureDxtBlockEncoder.EncodeBlock(block,format)
                    : TextureDxtSampleBlockEncoder.EncodeBlock(block,blockWidth,blockHeight,format);
                Buffer.BlockCopy(encoded,0,result,output,16);output+=16;
            }
            return result;
        }
        private static void WriteMipChain(byte[] output, int width, int height, byte[] rgba, string format)
        {
            float[] original = TextureTriangleMipEncoder.Normalize(rgba);
            int levelWidth = width, levelHeight = height, offset = 128;
            while (true)
            {
                float[] level = TextureTriangleMipEncoder.ResizeLevel(original, width, height, levelWidth, levelHeight);
                int blockWidth = Math.Min(4, levelWidth), blockHeight = Math.Min(4, levelHeight);
                float[] block = new float[blockWidth * blockHeight * 4];
                for (int y = 0; y < levelHeight; y += 4) for (int x = 0; x < levelWidth; x += 4)
                {
                    for (int row = 0; row < blockHeight; row++)
                        Array.Copy(level, ((y + row) * levelWidth + x) * 4, block, row * blockWidth * 4, blockWidth * 4);
                    // Even a full 4x4 filtered block must consume float samples.
                    byte[] encoded = TextureDxtSampleBlockEncoder.EncodeFloatBlock(block, blockWidth, blockHeight, format);
                    Buffer.BlockCopy(encoded, 0, output, offset, 16); offset += 16;
                }
                if (levelWidth == 1 && levelHeight == 1) break;
                levelWidth = Math.Max(1, levelWidth / 2); levelHeight = Math.Max(1, levelHeight / 2);
            }
            if (offset != output.Length) throw new InvalidOperationException("Compressed mip output size mismatch.");
        }
        private static void Put(byte[] data,int offset,uint value)
        {
            data[offset]=(byte)value;data[offset+1]=(byte)(value>>8);data[offset+2]=(byte)(value>>16);data[offset+3]=(byte)(value>>24);
        }
    }
}

