using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Native encoder reconstruction: RVA 0xf700 (predictor/quantizer),
    // 0xf690 (four-subblock layout), 0xfa60/0xfba0 (128-frame buffering).
    // Kept separate from Plugin routing until original-DLL differential passes.
    public static class Xas1Encoder
    {
        private static readonly float[] Previous = { 0.0f, -0.9375f, -1.796875f, -1.53125f };
        private static readonly float[] Older = { 0.0f, 0.0f, 0.8125f, 0.859375f };

        // Original 0xf700 uses SSE binary32 after every mulss/addss. Legacy
        // x86 Release JIT otherwise retains extended x87 temporaries, which
        // can cross an XAS quantization threshold even for exact signed16
        // imported/resampled samples. Preserve each original float boundary.
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static float F(float value) { return value; }

        public static byte[] EncodeBlock128(float[] samples, int channels)
        {
            if (samples == null) throw new ArgumentNullException("samples");
            if ((channels != 1 && channels != 2 && channels != 4 && channels != 6) || samples.Length != 128 * channels)
                throw new ArgumentException("A 1/2/4/6-channel block of exactly 128 frames is required");
            byte[] output = new byte[76 * channels];
            for (int channel = 0; channel < channels; ++channel)
            {
                byte[][] groups = new byte[4][];
                for (int group = 0; group < 4; ++group)
                    groups[group] = EncodeGroup(samples, channels, channel + group * 32 * channels);
                int at = channel * 76;
                for (int group = 0; group < 4; ++group)
                    Buffer.BlockCopy(groups[group], 0, output, at + group * 4, 4);
                for (int i = 0; i < 15; ++i)
                    for (int group = 0; group < 4; ++group)
                        output[at + 16 + i * 4 + group] = groups[group][4 + i];
            }
            return output;
        }

        private static byte[] EncodeGroup(float[] samples, int channels, int at)
        {
            int first = Math.Min(32752, ((int)F(F(samples[at] * 32767.0f) + 8.0f)) & ~15);
            int second = Math.Min(32752, ((int)F(F(samples[at + channels] * 32767.0f) + 8.0f)) & ~15);
            float best = 1000000000.0f;
            int filter = 0;
            float[] source = new float[30];
            for (int f = 0; f < 4; ++f)
            {
                int old = first, previous = second;
                float maximum = 0.0f;
                for (int i = 0; i < 30; ++i)
                {
                    float value = F(samples[at + (i + 2) * channels] * 32767.0f);
                    source[i] = value;
                    float prediction = F(previous * Previous[f]);
                    prediction = F(prediction + F(old * Older[f]));
                    float residual = F(prediction + value);
                    maximum = Math.Max(maximum, residual > 0 ? residual : -residual);
                    old = previous;
                    previous = (int)value;
                }
                if (maximum < best) { best = maximum; filter = f; }
                if (f == 0 && maximum <= 7.0f) { filter = 0; break; }
            }
            int magnitude = Math.Max(-32768, Math.Min(32767, (int)best));
            int shift = 0, mask = 0x4000;
            while (shift < 12 && ((magnitude + (mask >> 3)) & mask) == 0)
            { ++shift; mask >>= 1; }
            byte[] output = new byte[19];
            int firstHeader = first | filter, secondHeader = second | shift;
            output[0] = (byte)firstHeader; output[1] = (byte)(firstHeader >> 8);
            output[2] = (byte)secondHeader; output[3] = (byte)(secondHeader >> 8);
            float h1 = second, h2 = first;
            float gain = 1 << shift;
            for (int i = 0; i < 30; ++i)
            {
                float p2 = F(Older[filter] * h2);
                float p1 = F(Previous[filter] * h1);
                float prediction = F(p2 + p1);
                float residual = F(prediction + source[i]);
                residual = F(residual * gain);
                int quantized = ((int)residual + 0x800) & ~0xfff;
                quantized = Math.Max(-32768, Math.Min(32767, quantized));
                if ((i & 1) == 0) output[4 + i / 2] = (byte)((quantized >> 8) & 0xf0);
                else output[4 + i / 2] |= (byte)((quantized >> 12) & 15);
                float reconstructed = F((quantized >> shift) - prediction);
                h2 = h1; h1 = reconstructed;
            }
            return output;
        }

        public static PcmEncodingResult EncodeWave(byte[] wave, bool streamed)
        {
            Pcm16Encoder.PcmWaveInput input = Pcm16Encoder.ReadWave(wave);
            return EncodeSamples(input.Samples, input.Channels, input.SampleRate, streamed);
        }

        public static PcmEncodingResult EncodeSamples(float[] samples, int channels, int sampleRate, bool streamed)
        {
            if (samples == null) throw new ArgumentNullException("samples");
            if ((channels != 1 && channels != 2 && channels != 4 && channels != 6) || samples.Length % channels != 0)
                throw new ArgumentOutOfRangeException("channels");
            int frames = samples.Length / channels;
            if (frames == 0 || frames > 0x1fffffff) throw new ArgumentOutOfRangeException("samples");
            for (int i = 0; i < samples.Length; ++i)
                if (Single.IsNaN(samples[i]) || Single.IsInfinity(samples[i]) || samples[i] < -1 || samples[i] > 1)
                    throw new ArgumentOutOfRangeException("samples");
            byte[] header = Pcm16Encoder.CreateHeader(4, channels, sampleRate, frames, streamed);
            List<byte> data = new List<byte>();
            List<float> pending = new List<float>();
            int frameOffset = 0;
            int maximum = streamed ? Pcm16Encoder.NativeBlockFrames(sampleRate * channels * 0.59375f, sampleRate) : frames;
            while (frameOffset < frames)
            {
                int count = Math.Min(maximum, frames - frameOffset);
                bool last = frameOffset + count == frames;
                for (int i = frameOffset * channels; i < (frameOffset + count) * channels; ++i) pending.Add(samples[i]);
                int encodedFrames = (pending.Count / (channels * 128)) * 128;
                List<byte> payload = new List<byte>();
                while (pending.Count >= channels * 128)
                {
                    float[] block = pending.GetRange(0, channels * 128).ToArray();
                    pending.RemoveRange(0, channels * 128);
                    payload.AddRange(EncodeBlock128(block, channels));
                }
                if (last && pending.Count > 0)
                {
                    int remaining = pending.Count / channels;
                    float[] block = new float[128 * channels];
                    pending.CopyTo(block);
                    for (int i = remaining; i < 128; ++i)
                        for (int c = 0; c < channels; ++c) block[i * channels + c] = block[(remaining - 1) * channels + c];
                    payload.AddRange(EncodeBlock128(block, channels));
                    encodedFrames += remaining;
                    pending.Clear();
                }
                int bytes = payload.Count + 8;
                int padding = streamed && last ? 64 - ((data.Count + bytes) % 64) : 0;
                WriteBE(data, (uint)(bytes + padding) | (last && streamed ? 0x80000000u : 0u));
                WriteBE(data, (uint)encodedFrames);
                data.AddRange(payload);
                for (int i = 0; i < padding; ++i) data.Add(0);
                frameOffset += count;
            }
            byte[] custom = data.ToArray();
            if (!streamed)
            {
                custom = new byte[header.Length + data.Count];
                Buffer.BlockCopy(header, 0, custom, 0, header.Length); data.CopyTo(custom, header.Length);
            }
            return new PcmEncodingResult(streamed ? header : new byte[0], custom, sampleRate, channels, frames);
        }

        private static void WriteBE(List<byte> output, uint value)
        { output.Add((byte)(value >> 24)); output.Add((byte)(value >> 16)); output.Add((byte)(value >> 8)); output.Add((byte)value); }
    }
}
