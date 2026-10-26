using System;
using System.Runtime.CompilerServices;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Original SIMEX filter 120 profile and RVA 0x42480 window scheduler.
    // Standalone numerical candidate. AudioFile rate conversion is not enabled
    // merely by compiling this component; its plugin/SDK acceptance is separate.
    public static class ResamplerScheduler
    {
        public sealed class Profile
        {
            public readonly int Factor;
            public readonly float Ratio;
            public readonly float PhaseRate;
            public readonly float[] Frequencies;
            public readonly float[] Amplitudes;
            internal Profile(int factor, float ratio, float phaseRate, float[] frequencies, float[] amplitudes)
            { Factor = factor; Ratio = ratio; PhaseRate = phaseRate; Frequencies = frequencies; Amplitudes = amplitudes; }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static float F(float value) { return value; }

        public static Profile CreateProfile(int sourceRate, int destinationRate)
        {
            if (sourceRate <= 0) throw new ArgumentOutOfRangeException("sourceRate");
            if (destinationRate < 400 || destinationRate > 96000)
                throw new ArgumentOutOfRangeException("destinationRate");
            float ratio = F(F((float)destinationRate) / F((float)sourceRate));
            const float rolloff = 0.949999988079071044921875f;
            if (destinationRate > sourceRate)
                return new Profile(2, ratio, F(ratio * 0.5f),
                    new float[] { 0.0f, rolloff, 1.0f }, new float[] { 1.0f, 1.0f, 0.0f });
            int factor = (long)destinationRate * 2 < sourceRate ? 1 : 2;
            return new Profile(factor, ratio, factor == 1 ? ratio : F(ratio * 0.5f),
                new float[] { 0.0f, F(ratio * rolloff), ratio, 1.0f },
                new float[] { 1.0f, 1.0f, 0.0f, 0.0f });
        }

        public static int OutputFrameCount(int sourceFrames, float ratio)
        {
            if (sourceFrames < 0 || !(ratio > 0.0f) || Single.IsInfinity(ratio))
                throw new ArgumentOutOfRangeException("sourceFrames");
            float value = F(F((float)sourceFrames) * ratio);
            // cvttss2si's overflow sentinel is not a usable allocation length.
            if (value >= 2147483648.0f) throw new ArgumentOutOfRangeException("sourceFrames");
            return (int)value;
        }

        // The original short-input branch changes only the rate. It retains
        // source frames, loop points and marker positions unchanged.
        public static bool IsRateOnly(int sourceFrames, Profile profile)
        {
            if (profile == null) throw new ArgumentNullException("profile");
            if (sourceFrames < 0) throw new ArgumentOutOfRangeException("sourceFrames");
            return F(F((float)sourceFrames) * profile.Ratio) <= 1.0f;
        }

        public static int RescalePosition(int position, float ratio)
        {
            float value = F(F(F((float)position) * ratio) + 0.5f);
            if (value >= 2147483648.0f || value < -2147483648.0f)
                throw new ArgumentOutOfRangeException("position");
            return (int)value;
        }

        public static void RescaleLoop(int destinationFrames, float ratio, ref int start, ref int end)
        {
            if (end > 0)
            {
                start = RescalePosition(start, ratio);
                end = RescalePosition(end, ratio);
            }
            if (end >= destinationFrames) end = destinationFrames - 1;
        }

        public static short[] Resample(short[] source, int sourceRate, int destinationRate)
        {
            if (source == null) throw new ArgumentNullException("source");
            Profile profile = CreateProfile(sourceRate, destinationRate);
            if (sourceRate == destinationRate || IsRateOnly(source.Length, profile)) return source;
            float[] coefficients = ResamplerCoefficients.Create(profile.Factor,
                profile.Frequencies, profile.Amplitudes, 1.0f, false);
            return Process(source, coefficients, profile.Factor, profile.PhaseRate);
        }

        // The two FFT arrays pack consecutive real source windows into the
        // real and imaginary lanes. Filtering both together produces two
        // independent real convolutions. Consume them in alternating order.
        public static short[] Process(short[] source, float[] coefficients, int factor, float phaseRate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (coefficients == null || coefficients.Length != 8192)
                throw new ArgumentException("8192 filter coefficients are required", "coefficients");
            if (factor != 1 && factor != 2) throw new ArgumentOutOfRangeException("factor");
            if (!(phaseRate > 0.0f) || Single.IsInfinity(phaseRate))
                throw new ArgumentOutOfRangeException("phaseRate");
            if (source.Length > Int32.MaxValue / factor) throw new ArgumentOutOfRangeException("source");
            int count = OutputFrameCount(source.Length * factor, phaseRate);
            short[] output = new short[count];
            if (count == 0) return output;
            int support = 256 * factor;
            int span = 8192 - 2 * support - factor;
            float[] real = new float[8192], imaginary = new float[8192], derivative = new float[8192];
            float[] current = imaginary;
            int sourcePosition = -support;
            float phase = F((float)span), increment = F(1.0f / phaseRate);
            for (int i = 0; i < count; ++i)
            {
                if (phase >= F((float)span))
                {
                    if (Object.ReferenceEquals(current, real)) current = imaginary;
                    else
                    {
                        ResamplerFilter.FillSource(source, sourcePosition, factor, real);
                        sourcePosition = checked(sourcePosition + span);
                        ResamplerFilter.FillSource(source, sourcePosition, factor, imaginary);
                        sourcePosition = checked(sourcePosition + span);
                        ResamplerFilter.Process(real, imaginary, coefficients, factor);
                        current = real;
                    }
                    ResamplerFilter.Derivatives(current, support, derivative);
                    phase = F(phase - F((float)span));
                }
                float position = F(F((float)support) + phase);
                int index = (int)Math.Floor((double)position);
                // The original single-refill scheduler is meaningful only
                // while the phase step stays within the usable window.
                if (index < support || index + 1 >= 8192 - support)
                    throw new ArgumentOutOfRangeException("phaseRate", "Phase exceeds the original interpolation window");
                float t = F(position - F((float)index));
                float p0 = current[index], p1 = current[index + 1];
                float m0 = derivative[index], m1 = derivative[index + 1];
                // RVA 0x4267d..0x426b6, preserving every SSE add/mul order.
                float a = F(F(F(p0 - p1) * 2.0f) + m1);
                a = F(a + m0);
                float b = F(F(p1 - p0) - m0);
                b = F(b - a);
                float value = F(F(F(F(F(F(a * t) + b) * t) + m0) * t) + p0);
                output[i] = RoundSigned16(value);
                phase = F(increment + phase);
            }
            return output;
        }

        public static short RoundSigned16(float value)
        {
            if (Single.IsNaN(value) || Single.IsInfinity(value))
                throw new ArgumentOutOfRangeException("value");
            double rounded = Math.Round((double)value, MidpointRounding.ToEven);
            // x87 FISTP returns 0x8000 on signed16 overflow, then native code
            // selects the clipping limit from the original float sign.
            if (rounded > 32767.0) return 32767;
            if (rounded < -32768.0) return -32768;
            return (short)rounded;
        }
    }
}
