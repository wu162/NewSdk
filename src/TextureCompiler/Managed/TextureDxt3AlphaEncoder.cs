using System;

namespace BinaryAssetBuilder.TextureCompiler
{
    // DXT3 explicit alpha only. Not a complete DXT compressor or plugin route.
    public static class TextureDxt3AlphaEncoder
    {
        public static byte[] EncodeBlock(byte[] rgba)
        {
            if (rgba == null) throw new ArgumentNullException("rgba");
            if (rgba.Length != 64) throw new ArgumentException("A 4x4 RGBA block is required.", "rgba");
            byte[] result = new byte[8];
            for (int i = 0; i < 16; i++)
            {
                int alpha = (rgba[i * 4 + 3] * 15 + 127) / 255;
                result[i / 2] |= (byte)(alpha << ((i & 1) * 4));
            }
            return result;
        }
    }
}
