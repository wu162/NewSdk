using System;
using System.Runtime.CompilerServices;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Complete 0x97b00 signal-transform wrapper for the recovered table-backed
    // DCT domain. Folding, band masks and the optional 256-entry gain curve
    // are included. Quantization, entropy coding and XMA routing are not.
    public static class XmaSignalTransform
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static float Store(double value) { return BitConverter.ToSingle(BitConverter.GetBytes((float)value), 0); }
        private static int Size(float[] windowed)
        {
            if (windowed == null) throw new ArgumentNullException("windowed");
            if ((windowed.Length & 1) != 0) throw new ArgumentOutOfRangeException("windowed");
            int size = windowed.Length / 2;
            if (size < 64 || size > 2048 || (size & (size - 1)) != 0) throw new ArgumentOutOfRangeException("windowed");
            foreach (float value in windowed)
                if (Single.IsNaN(value) || Single.IsInfinity(value)) throw new ArgumentOutOfRangeException("windowed");
            return size;
        }
        public static float[] Fold(float[] windowed)
        {
            int size = Size(windowed), half = size / 2;
            float[] folded = new float[size];
            for (int i = 0; i < half; ++i)
            {
                folded[i] = Store((double)windowed[3 * half - 1 - i] + windowed[3 * half + i]);
                folded[half + i] = Store((double)windowed[size - 1 - i] - windowed[i]);
            }
            return folded;
        }
        public static int Process(float[] windowed, int leadingZeroCount, int legacyUpperLimit,
            int codecVersion, bool reduceTail, float retainedFraction, float[] weights,
            out float[] spectrum, out int outputShift)
        {
            int size = Size(windowed);
            if (leadingZeroCount < 0 || leadingZeroCount > size) throw new ArgumentOutOfRangeException("leadingZeroCount");
            if (legacyUpperLimit < 0) throw new ArgumentOutOfRangeException("legacyUpperLimit");
            if (Single.IsNaN(retainedFraction) || Single.IsInfinity(retainedFraction) || retainedFraction < 0.0f)
                throw new ArgumentOutOfRangeException("retainedFraction");
            float scaled = Store((double)retainedFraction * size);
            // Native 0x79490 adds .5 for nonnegative values and calls the
            // truncating 0x586a0 conversion. Reject masked-invalid conversion,
            // which would otherwise turn into a negative native array index.
            double rounded = Math.Truncate((double)scaled + 0.5);
            if (Double.IsInfinity(rounded) || rounded > Int32.MaxValue) throw new ArgumentOutOfRangeException("retainedFraction");
            if (weights != null)
            {
                if (weights.Length != 256) throw new ArgumentException("256 weights required", "weights");
                foreach (float value in weights)
                    if (Single.IsNaN(value) || Single.IsInfinity(value)) throw new ArgumentOutOfRangeException("weights");
            }
            float[] result = Fold(windowed);
            int shift;
            int hresult = XmaDct4.Transform(result, 1.0f, out shift);
            for (int i = 0; i < leadingZeroCount; ++i) result[i] = 0.0f;
            int upper = codecVersion > 2 ? size : legacyUpperLimit;
            if (codecVersion > 2 && reduceTail) upper += (-9 * size) / 100;
            int firstZero = Math.Min((int)rounded, upper);
            for (int i = firstZero; i < size; ++i) result[i] = 0.0f;
            if (weights != null)
                for (int i = 0; i < size; ++i) result[i] = Store((double)weights[(i * 256) / size] * result[i]);
            spectrum = result; outputShift = shift;
            return hresult;
        }
    }
}
