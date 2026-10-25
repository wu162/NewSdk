using System;
using System.Runtime.CompilerServices;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Original 0xa7de3 complete table-backed interleaved FFT, orders0..15.
    // Used as backend transform callback; no XMA codec route is enabled.
    public static class XmaComplexFft
    {
        private static readonly uint[] Cosine = { 0xbf800000u, 0xb33bbd2eu, 0x3f3504f3u, 0x3f6c835eu, 0x3f7b14beu, 0x3f7ec46du, 0x3f7fb10fu, 0x3f7fec43u, 0x3f7ffb11u, 0x3f7ffec4u, 0x3f7fffb1u, 0x3f7fffecu, 0x3f7ffffbu, 0x3f7fffffu, 0x3f800000u, 0x3f800000u };
        private static readonly uint[] Sine = { 0x33bbbd2eu, 0xbf800000u, 0xbf3504f3u, 0xbec3ef16u, 0xbe47c5c2u, 0xbdc8bd36u, 0xbd48fb30u, 0xbcc90ab0u, 0xbc490e90u, 0xbbc90f88u, 0xbb490fc6u, 0xbac90fd6u, 0xba490fdau, 0xb9c90fdbu, 0xb9490fdbu, 0xb8c90fdbu };
        private static float Float(uint bits) { return BitConverter.ToSingle(BitConverter.GetBytes(bits), 0); }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static float Store(double value) { return BitConverter.ToSingle(BitConverter.GetBytes((float)value), 0); }
        private static void Butterfly(float[] data, int a, int b, float real, float imaginary, bool plain)
        {
            float r = Store((double)data[a] - data[b]), i = Store((double)data[a + 1] - data[b + 1]);
            data[a] = Store((double)data[a] + data[b]); data[a + 1] = Store((double)data[a + 1] + data[b + 1]);
            data[b] = plain ? r : Store((double)r * real - (double)i * imaginary);
            data[b + 1] = plain ? i : Store((double)r * imaginary + (double)i * real);
        }
        public static void Transform(float[] data, int order, bool inverse)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (order < 0 || order > 15) throw new ArgumentOutOfRangeException("order");
            int count = 1 << order, length = count * 2;
            if (data.Length != length) throw new ArgumentException("Interleaved real/imaginary array length must equal 2 << order", "data");
            foreach (float value in data)
                if (Single.IsNaN(value) || Single.IsInfinity(value)) throw new ArgumentOutOfRangeException("data");
            float rootCos = Float(Cosine[order]), rootSin = Store(2.0 * Float(Sine[order]));
            if (inverse) rootSin = Store(-rootSin);
            float halfSin = Store(0.5 * rootSin);
            int width = length;
            while (width > 4)
            {
                int half = width / 2;
                float cosine = Store(1.0 - (double)halfSin * rootSin);
                float sine = Store((double)rootSin * rootCos);
                halfSin = sine; rootCos = cosine; rootSin = Store(2.0 * sine);
                for (int block = 0; block < length; block += width)
                {
                    Butterfly(data, block, block + half, 1.0f, 0.0f, true);
                    Butterfly(data, block + 2, block + half + 2, cosine, sine, false);
                }
                float real = 1.0f, imaginary = 0.0f;
                for (int offset = 4; offset < half; offset += 4)
                {
                    real = Store((double)real - (double)rootSin * sine);
                    imaginary = Store((double)rootSin * cosine + imaginary);
                    cosine = Store((double)cosine - (double)rootSin * imaginary);
                    sine = Store((double)sine + (double)rootSin * real);
                    for (int block = 0; block < length; block += width)
                    {
                        Butterfly(data, block + offset, block + offset + half, real, imaginary, false);
                        Butterfly(data, block + offset + 2, block + offset + half + 2, cosine, sine, false);
                    }
                }
                width = half;
            }
            if (width > 2)
                for (int block = 0; block < length; block += 4) Butterfly(data, block, block + 2, 1.0f, 0.0f, true);
            // Same bit-reversal permutation as the original four-at-a-time swap.
            for (int index = 0, reverse = 0; index < count; ++index)
            {
                if (reverse > index)
                {
                    float real = data[index * 2], imaginary = data[index * 2 + 1];
                    data[index * 2] = data[reverse * 2]; data[index * 2 + 1] = data[reverse * 2 + 1];
                    data[reverse * 2] = real; data[reverse * 2 + 1] = imaginary;
                }
                int bit = count >> 1;
                while (bit > 0 && (reverse & bit) != 0) { reverse ^= bit; bit >>= 1; }
                reverse ^= bit;
            }
            if (inverse)
                for (int index = 0; index < length; ++index) data[index] = Store((double)data[index] / count);
        }
    }
}
