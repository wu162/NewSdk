using System;

namespace Recovered.Compiler.Texture
{
    // GetNVCompressionOptions: PS3 uses mipMapGeneration=30, specifiedMipMaps=3.
    // Filtering is the same base-relative triangle kernel as the accepted PC
    // encoder. Only the number of serialized levels differs; DDS is little endian.
    public static class TexturePs3MipEncoder
    {
        public static byte[] Encode(uint width, uint height, byte[] rgba, string format)
        {
            byte[] chain = TextureTriangleMipEncoder.Encode(width, height, rgba, format);
            uint count = Read(chain, 28);
            if (count <= 3) return chain;
            int stride = format == "D3DFMT_R5G6B5" ? 2 : 4;
            long length = 128;
            uint w = width, h = height;
            for (int level = 0; level < 3; level++)
            {
                length = checked(length + (long)w * h * stride);
                w = Math.Max(1u, w / 2); h = Math.Max(1u, h / 2);
            }
            byte[] output = new byte[checked((int)length)];
            Buffer.BlockCopy(chain, 0, output, 0, output.Length);
            output[28] = 3; output[29] = 0; output[30] = 0; output[31] = 0;
            return output;
        }
        private static uint Read(byte[] bytes, int offset)
        {
            return (uint)(bytes[offset] | bytes[offset + 1] << 8 |
                bytes[offset + 2] << 16 | bytes[offset + 3] << 24);
        }
    }
}
