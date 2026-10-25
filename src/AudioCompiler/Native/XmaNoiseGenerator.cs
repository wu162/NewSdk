using System;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Closed numeric helper group: native 0x64cd5 (state transition) and
    // 0x65a0b (float noise fill). This is NOT an XMA encoder or route.
    // The original x87 caller uses 53-bit precision, nearest rounding.
    public static class XmaNoiseGenerator
    {
        public static int Next(ref uint seed, ref int previous)
        {
            unchecked
            {
                seed = seed * 1664525u + 1013904223u;
                int value = ((int)seed) >> 2;
                value += value >> 2;
                int result = value - previous;
                previous = value;
                return result;
            }
        }

        public static void Fill(ref uint seed, ref int previous, float gain,
            float[] destination, int offset, int count)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (offset < 0 || count < 0 || offset > destination.Length - count)
                throw new ArgumentOutOfRangeException("offset/count");
            if (Single.IsNaN(gain) || Single.IsInfinity(gain))
                throw new ArgumentOutOfRangeException("gain");
            for (int i = 0; i < count; ++i)
            {
                // Native FILD int32, FMUL binary32 gain, FMUL exact binary64
                // 2^-29, then FSTP binary32. Do not round the first product
                // to binary32 or replace the signed state arithmetic.
                double value = (double)Next(ref seed, ref previous) * (double)gain;
                destination[offset + i] = (float)(value * 1.86264514923095703125E-09);
            }
        }
    }
}
