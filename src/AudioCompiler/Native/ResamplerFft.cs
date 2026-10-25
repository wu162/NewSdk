using System;
using System.Runtime.CompilerServices;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Standalone recovery of original native RVA 0x4eca0. This is a separate
    // numerical component, not a resampling backend or an AudioFile route.
    public static class ResamplerFft
    {
        // Original x87 FSIN stage constants captured at RVA 0x4ef03 with
        // FPCW=0x027f. Each pair is (-2*sin(theta/2)^2, sin(theta));
        // exact float bits, independently recorded in native-fft/twiddles.tsv.
        private static readonly uint[][] StageBits =
        {
            new uint[] { 0xc0000000u, 0xa50d3132u, 0xbf800000u, 0xbf800000u, 0xbe95f61au, 0xbf3504f3u, 0xbd9be50cu, 0xbec3ef15u, 0xbc9d6830u, 0xbe47c5c2u, 0xbb9dc971u, 0xbdc8bd36u, 0xba9de1c8u, 0xbd48fb30u, 0xb99de7dfu, 0xbcc90ab0u, 0xb89de964u, 0xbc490e90u, 0xb79de9c6u, 0xbbc90f88u, 0xb69de9deu, 0xbb490fc6u, 0xb59de9e4u, 0xbac90fd5u, 0xb49de9e6u, 0xba490fd9u },
            new uint[] { 0xc0000000u, 0x250d3132u, 0xbf800000u, 0x3f800000u, 0xbe95f61au, 0x3f3504f3u, 0xbd9be50cu, 0x3ec3ef15u, 0xbc9d6830u, 0x3e47c5c2u, 0xbb9dc971u, 0x3dc8bd36u, 0xba9de1c8u, 0x3d48fb30u, 0xb99de7dfu, 0x3cc90ab0u, 0xb89de964u, 0x3c490e90u, 0xb79de9c6u, 0x3bc90f88u, 0xb69de9deu, 0x3b490fc6u, 0xb59de9e4u, 0x3ac90fd5u, 0xb49de9e6u, 0x3a490fd9u }
        };

        // A non-inlined float parameter is stored as binary32 at the call
        // boundary on the legacy x86 CLR, preventing extended temporaries.
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static float F(float value) { return value; }

        private static float Bits(uint value)
        { return BitConverter.ToSingle(BitConverter.GetBytes(value), 0); }

        // Direction 0: negative-angle, unscaled. Direction 1: positive-angle,
        // normalize both arrays by 1/N. Arrays are transformed in place.
        public static void Transform(float[] real, float[] imaginary, int order, int direction)
        {
            if (order < 1 || order > 13) throw new ArgumentOutOfRangeException("order");
            if (direction != 0 && direction != 1) throw new ArgumentOutOfRangeException("direction");
            if (real == null) throw new ArgumentNullException("real");
            if (imaginary == null) throw new ArgumentNullException("imaginary");
            int n = 1 << order;
            if (real.Length != n || imaginary.Length != n || Object.ReferenceEquals(real, imaginary))
                throw new ArgumentException("Distinct real and imaginary arrays of exactly 2^order floats are required");
            for (int i = 0; i < n; ++i)
                if (Single.IsNaN(real[i]) || Single.IsInfinity(real[i]) ||
                    Single.IsNaN(imaginary[i]) || Single.IsInfinity(imaginary[i]))
                    throw new ArgumentException("Finite FFT input samples are required");

            for (int i = 0; i < n; ++i)
            {
                int reversed = 0, bits = i;
                for (int b = 0; b < order; ++b) { reversed = (reversed << 1) | (bits & 1); bits >>= 1; }
                if (reversed > i)
                {
                    float value = real[i]; real[i] = real[reversed]; real[reversed] = value;
                    value = imaginary[i]; imaginary[i] = imaginary[reversed]; imaginary[reversed] = value;
                }
            }
            for (int stage = 0, width = 2; width <= n; ++stage, width <<= 1)
            {
                int half = width >> 1;
                float a = Bits(StageBits[direction][stage * 2]);
                float b = Bits(StageBits[direction][stage * 2 + 1]);
                for (int first = 0; first < n; first += width)
                {
                    float wr = 1.0f, wi = 0.0f;
                    for (int j = 0; j < half; ++j)
                    {
                        int left = first + j, right = left + half;
                        // RVA 0x4f1e2..0x4f239, including original add order.
                        float imagProduct = F(F(real[right] * wi) + F(imaginary[right] * wr));
                        float realProduct = F(F(real[right] * wr) - F(imaginary[right] * wi));
                        float lr = real[left], li = imaginary[left];
                        real[right] = F(lr - realProduct);
                        imaginary[right] = F(li - imagProduct);
                        real[left] = F(lr + realProduct);
                        imaginary[left] = F(li + imagProduct);
                        // Original two-parameter twiddle recurrence; preserve
                        // its binary32 operation order instead of fresh sin/cos.
                        float nextImaginary = F(F(F(wr * b) + F(wi * a)) + wi);
                        float nextReal = F(F(F(wr * a) - F(wi * b)) + wr);
                        wr = nextReal; wi = nextImaginary;
                    }
                }
            }
            if (direction == 1)
            {
                float scale = F(1.0f / n);
                for (int i = 0; i < n; ++i)
                {
                    real[i] = F(real[i] * scale);
                    imaginary[i] = F(imaginary[i] * scale);
                }
            }
        }
    }
}
