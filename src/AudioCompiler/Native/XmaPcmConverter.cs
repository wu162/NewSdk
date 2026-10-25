using System;
using System.Runtime.CompilerServices;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Original 0xe9b0: SSE binary32 scale, then masked x87 FISTP with
    // round-to-nearest/even, storing only the low 16 bits. No clipping.
    // This is a preprocessing component; no XMA encoder route is enabled.
    public static class XmaPcmConverter
    {
        public static short[] Convert(float[] source, int sourceOffset, int channels, int sourceStride, int frames)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (channels != 1 && channels != 2) throw new ArgumentOutOfRangeException("channels");
            if (sourceOffset < 0 || sourceOffset > source.Length) throw new ArgumentOutOfRangeException("sourceOffset");
            if (sourceStride < 0) throw new ArgumentOutOfRangeException("sourceStride");
            if (frames < 0 || frames > int.MaxValue / channels) throw new ArgumentOutOfRangeException("frames");
            long end = frames == 0 ? sourceOffset : (long)sourceOffset + (long)(frames - 1) * sourceStride + channels;
            if (end > source.Length) throw new ArgumentException("Source does not contain the requested strided frames", "source");
            short[] result = new short[frames * channels];
            for (int frame = 0, output = 0; frame < frames; ++frame)
            for (int channel = 0; channel < channels; ++channel)
            {
                float scaled = Scale(source[sourceOffset + frame * sourceStride + channel]);
                // Masked invalid FISTP stores integer indefinite (80000000),
                // whose low word is zero. Preserve it for NaN/Inf/overflow.
                int value = float.IsNaN(scaled) || scaled >= 2147483648.0 || scaled < -2147483648.0 ?
                    int.MinValue : (int)Math.Round((double)scaled, MidpointRounding.ToEven);
                result[output++] = unchecked((short)value);
            }
            return result;
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static float Scale(float value)
        {
            // A physical float argument/store enforces MULSS rounding even
            // under the legacy x86 JIT's extended temporary precision.
            return BitConverter.ToSingle(BitConverter.GetBytes(value * 32767.0f), 0);
        }
    }
}
