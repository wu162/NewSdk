using System;
using System.Runtime.CompilerServices;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Original native RVAs 0x41f40, 0x41fa0 and 0x42250. Both FFT lanes
    // contain real, consecutive source windows; the complex transform filters
    // them together. No AudioFile routing is enabled by this component.
    public static class ResamplerFilter
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static float F(float value) { return value; }

        private static void CheckFactor(int factor)
        {
            if (factor != 1 && factor != 2)
                throw new ArgumentOutOfRangeException("factor");
        }

        private static void CheckWindow(float[] window, string name)
        {
            if (window == null) throw new ArgumentNullException(name);
            if (window.Length != 8192) throw new ArgumentException("8192 floats are required", name);
        }

        public static void FillSource(short[] source, int position, int factor, float[] destination)
        {
            if (source == null) throw new ArgumentNullException("source");
            CheckFactor(factor);
            CheckWindow(destination, "destination");
            // x86 IDIV truncates toward zero, including negative positions.
            int index = position / factor;
            int count = 8192 / factor;
            for (int i = 0; i < count; ++i)
            {
                long at = (long)index + i;
                destination[i] = at < 0 || at >= source.Length ? 0.0f : F(source[(int)at] * (float)factor);
            }
            // Native code leaves the unused suffix untouched. The filter
            // overwrites it with repeated frequency bins after the short FFT.
        }

        public static void Process(float[] real, float[] imaginary, float[] coefficients, int factor)
        {
            CheckFactor(factor);
            CheckWindow(real, "real");
            CheckWindow(imaginary, "imaginary");
            CheckWindow(coefficients, "coefficients");
            if (Object.ReferenceEquals(real, imaginary) || Object.ReferenceEquals(real, coefficients) ||
                Object.ReferenceEquals(imaginary, coefficients))
                throw new ArgumentException("Distinct buffers are required");
            int count = 8192 / factor;
            if (factor == 1)
                ResamplerFft.Transform(real, imaginary, 13, 0);
            else
            {
                float[] shorterReal = new float[count];
                float[] shorterImaginary = new float[count];
                Array.Copy(real, shorterReal, count);
                Array.Copy(imaginary, shorterImaginary, count);
                ResamplerFft.Transform(shorterReal, shorterImaginary, 12, 0);
                Array.Copy(shorterReal, real, count);
                Array.Copy(shorterImaginary, imaginary, count);
                Array.Copy(shorterReal, 0, real, count, count);
                Array.Copy(shorterImaginary, 0, imaginary, count, count);
            }
            for (int i = 0; i < 8192; ++i)
            {
                real[i] = F(real[i] * coefficients[i]);
                imaginary[i] = F(imaginary[i] * coefficients[i]);
            }
            ResamplerFft.Transform(real, imaginary, 13, 1);
        }

        public static void Derivatives(float[] filtered, int support, float[] destination)
        {
            CheckWindow(filtered, "filtered");
            CheckWindow(destination, "destination");
            if (Object.ReferenceEquals(filtered, destination))
                throw new ArgumentException("Distinct buffers are required");
            if (support < 1 || support >= 4096) throw new ArgumentOutOfRangeException("support");
            for (int i = support; i < 8192 - support; ++i)
                destination[i] = F(F(filtered[i + 1] - filtered[i - 1]) * 0.5f);
        }
    }
}
