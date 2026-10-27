using System;
using System.IO;
namespace Recovered.Compiler.Texture
{
    // Windows BITMAPINFOHEADER / BI_RGB. Other DIB revisions/compressions are
    // separate profiles and cannot silently fall through to this decoder.
    public static class TextureBmpDecoder
    {
        public static bool IsBmp(byte[] data) { return data != null && data.Length >= 2 && data[0] == 66 && data[1] == 77; }
        private static int U16(byte[] b,int p){return b[p]|b[p+1]<<8;}
        private static uint U32(byte[] b,int p){return (uint)b[p]|(uint)b[p+1]<<8|(uint)b[p+2]<<16|(uint)b[p+3]<<24;}
        private static InvalidDataException Invalid(){return new InvalidDataException("Invalid BMP image.");}
        public static TexturePacker.Image Decode(byte[] data)
        {
            if(!IsBmp(data)||data.Length<54)throw Invalid();
            if(U32(data,14)!=40)throw new NotSupportedException("Only BMP BITMAPINFOHEADER has been recovered.");
            int width=unchecked((int)U32(data,18)),signedHeight=unchecked((int)U32(data,22));
            int depth=U16(data,28);uint compression=U32(data,30),offset=U32(data,10),colors=U32(data,46);
            if(compression==3)throw new BinaryAssetBuilder.BinaryAssetBuilderException(null,BinaryAssetBuilder.ErrorCode.BadTextureFile,"D3DX cannot load the texture from disk.",new object[0]);
            if(compression!=0)throw new NotSupportedException("Compressed BMP has not been recovered.");
            if(U16(data,26)!=1||width<=0||signedHeight==0||signedHeight==int.MinValue||width>16384||Math.Abs(signedHeight)>16384)throw Invalid();
            int height=Math.Abs(signedHeight);
            if((long)width*height>16777216)throw Invalid();
            if(depth!=1&&depth!=4&&depth!=8&&depth!=16&&depth!=24&&depth!=32)throw Invalid();
            bool npot=(width&(width-1))!=0||(height&(height-1))!=0;
            if(depth<=8)throw new BinaryAssetBuilder.BinaryAssetBuilderException(null,BinaryAssetBuilder.ErrorCode.BadTextureFile,"D3DX cannot load the texture from disk.",new object[0]);
            int stride=checked(((width*depth+31)/32)*4);
            int paletteEntries=depth<=8?(colors==0?1<<depth:checked((int)colors)):0;
            if(paletteEntries>(1<<Math.Min(depth,8))||offset<54+paletteEntries*4||offset>data.Length||(long)stride*height>data.Length-offset)throw Invalid();
            // Legacy D3DX BI_RGB consumes pixels directly after the DIB, ignoring
            // the BMP file header's bfOffBits (including an extra file gap).
            offset=54;
            bool hasAlpha=depth==24;
            if(depth==32)for(int y=0;y<height;y++)for(int x=0;x<width;x++)if(data[(int)offset+y*stride+x*4+3]!=0)hasAlpha=true;
            uint[] pixels=new uint[width*height];
            for(int y=0;y<height;y++)for(int x=0;x<width;x++)
            {
                int row=checked((int)offset+(signedHeight<0?y:height-1-y)*stride);
                uint r,g,b,a=255;
                if(depth<=8)
                {
                    int index=(data[row+x*depth/8]>>(8-depth-x*depth%8))&((1<<depth)-1);
                    if(index>=paletteEntries)throw Invalid();int p=54+index*4;b=data[p];g=data[p+1];r=data[p+2];
                }
                else if(depth==16)
                {
                    int v=U16(data,row+x*2),rv=(v>>10)&31,gv=(v>>5)&31,bv=v&31;
                    r=(uint)((rv*255+15)/31);g=(uint)((gv*255+15)/31);b=(uint)((bv*255+15)/31);
                }
                else
                {
                    int p=row+x*(depth/8);b=data[p];g=data[p+1];r=data[p+2];if(depth==32&&hasAlpha)a=data[p+3];
                }
                pixels[y*width+x]=r|g<<8|b<<16|a<<24;
            }
            TexturePacker.Image image=new TexturePacker.Image(width,height,pixels,hasAlpha);
            return !npot?image:depth==16?TextureNpotStorageResampler.Resize(image,TextureNpotStorageResampler.Format.Rgb555):TextureNpotRgba8Resampler.Resize(image);
        }
    }
}


