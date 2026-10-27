using System;

namespace BinaryAssetBuilder.TextureCompiler
{
    // Original nvFastDXT alpha path only (quality 68). Production uses a different
    // endpoint search. This helper is not connected to the plugin's DXT route.
    public static class TextureDxt5FastAlphaEncoder
    {
        public static byte[] EncodeBlock(byte[] rgba)
        {
            if (rgba == null) throw new ArgumentNullException("rgba");
            if (rgba.Length != 64) throw new ArgumentException("A 4x4 RGBA block is required.", "rgba");
            float[] alpha = new float[16];
            float low = float.MaxValue, high = -float.MaxValue;
            for (int i = 0; i < 16; i++)
            {
                alpha[i] = (float)(rgba[i * 4 + 3] / 255.0);
                if (alpha[i] < low) low = alpha[i];
                if (alpha[i] > high) high = alpha[i];
            }
            byte[] result = new byte[8];
            result[0] = Quantize(high);
            result[1] = Quantize(low);
            if (result[0] == result[1])
            {
                // The DDS writer canonicalizes equal endpoints to six-level mode.
                // All pixels keep index zero; the second endpoint increments then clamps.
                result[1] = (byte)Math.Min(255, result[1] + 1);
                return result;
            }
            int[] map = { 0, 2, 3, 4, 5, 6, 7, 1 };
            ulong indices = 0;
            for (int i = 0; i < 16; i++)
            {
                // The original explicitly stores this ratio as float before
                // multiplying by 7 and adding 0.5 in x87 precision.
                float ratio = (float)(((double)alpha[i] - high) / ((double)low - high));
                int index = (int)((double)ratio * 7.0 + 0.5);
                index = Math.Max(0, Math.Min(7, index));
                indices |= (ulong)map[index] << (i * 3);
            }
            for (int i = 0; i < 6; i++) result[i + 2] = (byte)(indices >> (i * 8));
            return result;
        }

        private static byte Quantize(float alpha)
        {
            float biased = (float)((double)alpha * 255.0 + 12582912.0);
            return (byte)(BitConverter.ToInt32(BitConverter.GetBytes(biased), 0) & 0xffff);
        }
    }
}
