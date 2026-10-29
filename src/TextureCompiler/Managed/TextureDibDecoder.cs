using System;
using System.IO;

namespace Recovered.Compiler.Texture
{
    // Bare DIB storage, without the 14-byte BMP file header. The original image
    // dispatcher calls the same DIB parser for this representation and BMP.
    // This independent API does not add a Plugin/TextureImageLoader route.
    public static class TextureDibDecoder
    {
        private static uint U32(byte[] b,int at){return (uint)b[at]|(uint)b[at+1]<<8|(uint)b[at+2]<<16|(uint)b[at+3]<<24;}
        private static void Put(byte[] b,int at,uint value){for(int i=0;i<4;i++)b[at+i]=(byte)(value>>(i*8));}
        public static bool IsDib(byte[] source)
        {
            if(source==null||source.Length<4)return false;
            uint size=U32(source,0);
            return size==12||size==40||size==52||size==56||size==64||size==108||size==124;
        }
        public static TexturePacker.Image Load(string path)
        {
            if(path==null)throw new ArgumentNullException("path");
            return Decode(File.ReadAllBytes(path));
        }
        public static TexturePacker.Image Decode(byte[] source)
        {
            if(source==null)throw new ArgumentNullException("source");
            if(source.Length<4)throw new InvalidDataException("DIB header is truncated.");
            if(!IsDib(source))throw new NotSupportedException("This raw DIB revision has not been recovered.");
            uint size=U32(source,0);
            if(source.Length<size)throw new InvalidDataException("DIB header is truncated.");
            if(size!=12)
            {
                int depth=source[14]|source[15]<<8;
                if((depth==16||depth==24||depth==32)&&U32(source,32)!=0)
                    throw new NotSupportedException("True-colour DIB color tables have not been recovered.");
            }
            byte[] bmp=new byte[checked(source.Length+14)];
            bmp[0]=66;bmp[1]=77;Put(bmp,2,(uint)bmp.Length);Put(bmp,10,14+size);
            Buffer.BlockCopy(source,0,bmp,14,source.Length);
            // V5 profile offsets are relative to the DIB and therefore survive
            // the private envelope unchanged. Raster/gap/alpha/NPOT semantics,
            // deterministic original errors and bitfields quarantine stay shared.
            return TextureBmpDecoder.Decode(bmp);
        }
    }
}
