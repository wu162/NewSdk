using System;
using System.Runtime.CompilerServices;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Complete optional 0x855ef spectral attenuation stage; no codec routing.
    public static class XmaSpectralGate
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static float Store(double value) { return BitConverter.ToSingle(BitConverter.GetBytes((float)value), 0); }
        public static int Process(float[] spectrum, int control, bool enabled, float[] lowerBands, float[] upperBands)
        {
            if (spectrum == null) throw new ArgumentNullException("spectrum");
            int size = spectrum.Length;
            if (size < 128 || size > 2048 || (size & (size - 1)) != 0) throw new ArgumentOutOfRangeException("spectrum");
            foreach (float value in spectrum)
                if (Single.IsNaN(value) || Single.IsInfinity(value)) throw new ArgumentOutOfRangeException("spectrum");
            if (!enabled || control > 200) return 0;
            if (lowerBands == null || upperBands == null) throw new ArgumentNullException("bands");
            if (lowerBands.Length != 128 || upperBands.Length != 128) throw new ArgumentException("128 band values required", "bands");
            foreach (float value in lowerBands)
                if (Single.IsNaN(value) || Single.IsInfinity(value)) throw new ArgumentOutOfRangeException("lowerBands");
            foreach (float value in upperBands)
                if (Single.IsNaN(value) || Single.IsInfinity(value)) throw new ArgumentOutOfRangeException("upperBands");
            float factor = Store(1.0 - (double)control / 200.0);
            if (factor > 0.5f) factor = 0.5f;
            int binsPerBand = size / 128;
            for (int i = 0; i < size; ++i)
            {
                int band = i / binsPerBand;
                float upper = upperBands[band];
                float lower = Store((double)lowerBands[band] * factor);
                spectrum[i] = lower < upper && upper != 0.0f
                    ? Store((((double)upper - lower) / upper) * spectrum[i]) : 0.0f;
            }
            return 0;
        }
    }
}
