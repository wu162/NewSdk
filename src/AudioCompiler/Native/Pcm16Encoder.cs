using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BinaryAssetBuilder.AudioCompiler
{
    public sealed class PcmEncodingResult
    {
        public readonly byte[] HeaderData;
        public readonly byte[] CustomData;
        public readonly int SampleRate;
        public readonly int Channels;
        public readonly int Frames;
        internal PcmEncodingResult(byte[] header, byte[] data, int rate, int channels, int frames)
        { HeaderData = header; CustomData = data; SampleRate = rate; Channels = channels; Frames = frames; }
    }

    // Managed replacement for the native P6B0 encoder and SIMEX format 39
    // container writer. This narrow entry point accepts PCM16 RIFF/WAVE only;
    // AIFF, loop metadata, channel remapping, and resampling are not inferred.
    public static class Pcm16Encoder
    {
        public static PcmEncodingResult EncodeWave(byte[] wave, bool streamed)
        {
            PcmWaveInput input = ReadWave(wave);
            return EncodeSamples(input.Samples, input.Channels, input.SampleRate, streamed);
        }

        internal sealed class PcmWaveInput
        {
            internal float[] Samples;
            internal int Channels;
            internal int SampleRate;
        }

        internal static PcmWaveInput ReadWave(byte[] wave)
        {
            if (wave == null) throw new ArgumentNullException("wave");
            if (wave.Length < 12 || Tag(wave, 0) != "RIFF" || Tag(wave, 8) != "WAVE")
                throw new NotSupportedException("PCM16 RIFF/WAVE input is required");
            int channels = 0, rate = 0, bits = 0, format = 0, align = 0;
            int dataOffset = -1, dataLength = 0;
            long end = 8L + U32(wave, 4);
            if (end > wave.Length) throw new InvalidDataException("Truncated RIFF data");
            for (long at = 12; at + 8 <= end; )
            {
                int offset = checked((int)at);
                uint size = U32(wave, offset + 4);
                long next = at + 8 + size;
                if (next > end) throw new InvalidDataException("Truncated RIFF chunk");
                string tag = Tag(wave, offset);
                if (tag == "fmt ")
                {
                    if (size < 16) throw new InvalidDataException("Truncated WAVE format");
                    format = U16(wave, offset + 8);
                    channels = U16(wave, offset + 10);
                    rate = checked((int)U32(wave, offset + 12));
                    align = U16(wave, offset + 20);
                    bits = U16(wave, offset + 22);
                }
                else if (tag == "data") { dataOffset = offset + 8; dataLength = checked((int)size); }
                else if (tag == "smpl") throw new NotSupportedException("WAVE loop metadata is not supported by this entry point");
                at = next + (size & 1);
            }
            if (format != 1 || bits != 16 || (channels != 1 && channels != 2))
                throw new NotSupportedException("Only mono/stereo PCM16 WAVE is supported by this entry point");
            if (rate <= 0 || rate > 0x3ffff || align != channels * 2 || dataOffset < 0 || dataLength % align != 0)
                throw new InvalidDataException("Invalid PCM WAVE format");
            int frames = dataLength / align;
            if (frames == 0 || frames > 0x1fffffff) throw new NotSupportedException("Unsupported PCM frame count");
            float[] samples = new float[dataLength / 2];
            for (int i = 0; i < samples.Length; ++i)
            {
                short sample = unchecked((short)U16(wave, dataOffset + i * 2));
                samples[i] = sample / 32768.0f;
            }
            PcmWaveInput input = new PcmWaveInput();
            input.Samples = samples; input.Channels = channels; input.SampleRate = rate;
            return input;
        }

        public static PcmEncodingResult EncodeSamples(float[] interleaved, int channels, int sampleRate, bool streamed)
        {
            if (interleaved == null) throw new ArgumentNullException("interleaved");
            if ((channels != 1 && channels != 2) || interleaved.Length % channels != 0)
                throw new ArgumentOutOfRangeException("channels");
            if (sampleRate <= 0 || sampleRate > 0x3ffff) throw new ArgumentOutOfRangeException("sampleRate");
            int frames = interleaved.Length / channels;
            if (frames == 0 || frames > 0x1fffffff) throw new ArgumentOutOfRangeException("interleaved");
            byte[] header = CreateHeader(channels, sampleRate, frames, streamed);
            List<byte> data = new List<byte>();
            int maxFrames = streamed ? NativeBlockFrames(sampleRate * channels * 2.0f, sampleRate) : frames;
            for (int frame = 0; frame < frames; )
            {
                int count = Math.Min(maxFrames, frames - frame);
                bool last = frame + count == frames;
                int bytes = checked(8 + count * channels * 2);
                int padding = streamed && last ? 64 - ((data.Count + bytes) % 64) : 0;
                AddBE(data, (uint)(bytes + padding) | (last && streamed ? 0x80000000u : 0u));
                AddBE(data, (uint)count);
                for (int i = frame * channels; i < (frame + count) * channels; ++i)
                {
                    float sample = interleaved[i];
                    if (Single.IsNaN(sample) || Single.IsInfinity(sample) || sample < -1.0f || sample > 1.0f)
                        throw new ArgumentOutOfRangeException("interleaved", "Normalized finite samples are required");
                    short encoded = unchecked((short)(int)(sample * 32767.0f));
                    data.Add((byte)(encoded >> 8)); data.Add((byte)encoded);
                }
                for (int i = 0; i < padding; ++i) data.Add(0);
                frame += count;
            }
            byte[] custom;
            if (streamed) custom = data.ToArray();
            else
            {
                custom = new byte[header.Length + data.Count];
                Buffer.BlockCopy(header, 0, custom, 0, header.Length);
                data.CopyTo(custom, header.Length);
            }
            return new PcmEncodingResult(streamed ? header : new byte[0], custom, sampleRate, channels, frames);
        }

        // RVA 0x2cb50 uses the MSB-first writer at 0x2c6a0. The first
        // 32 bits are version:4, codec:4, channels-minus-one:6, rate:18.
        public static byte[] CreateHeader(int channels, int rate, int frames, bool streamed)
        { return CreateHeader(2, channels, rate, frames, streamed); }

        internal static byte[] CreateHeader(int codec, int channels, int rate, int frames, bool streamed)
        {
            if (channels < 1 || channels > 64 || rate < 0 || rate > 0x3ffff || frames < 0 || frames > 0x1fffffff)
                throw new ArgumentOutOfRangeException();
            List<byte> result = new List<byte>(8);
            AddBE(result, ((uint)codec << 24) | ((uint)(channels - 1) << 18) | (uint)rate);
            AddBE(result, (streamed ? 0x40000000u : 0u) | (uint)frames);
            return result.ToArray();
        }

        // RVA 0x2dac5: x87 2040 / bytesPerSecond * sampleRate, then
        // truncation to integer. Extended-precision rounding matters: 22050 Hz
        // mono PCM yields 1019 frames rather than the algebraic 1020.
        internal static int NativeBlockFrames(float bytesPerSecond, int rate)
        {
            decimal scale = 1m;
            // A direct float-to-decimal cast keeps only seven decimal digits
            // on the CLR; reconstruct the exact binary value instead.
            int bits = BitConverter.ToInt32(BitConverter.GetBytes(bytesPerSecond), 0);
            decimal denominator = (bits & 0x7fffff) | 0x800000;
            int exponent = ((bits >> 23) & 255) - 150;
            while (exponent > 0) { denominator *= 2m; --exponent; }
            while (exponent < 0) { denominator /= 2m; ++exponent; }
            const decimal Two63 = 9223372036854775808m;
            const decimal Two64 = 18446744073709551616m;
            while (2040m * scale / denominator < Two63) scale *= 2m;
            decimal quotient = Decimal.Round(2040m * scale / denominator, 0, MidpointRounding.ToEven);
            decimal product = quotient * rate;
            decimal roundingScale = 1m;
            while (product / roundingScale >= Two64) roundingScale *= 2m;
            product = Decimal.Round(product / roundingScale, 0, MidpointRounding.ToEven) * roundingScale;
            return (int)Decimal.Truncate(product / scale);
        }

        private static void AddBE(List<byte> output, uint value)
        { output.Add((byte)(value >> 24)); output.Add((byte)(value >> 16)); output.Add((byte)(value >> 8)); output.Add((byte)value); }
        private static int U16(byte[] bytes, int at) { return bytes[at] | (bytes[at + 1] << 8); }
        private static uint U32(byte[] bytes, int at)
        { return (uint)(bytes[at] | (bytes[at + 1] << 8) | (bytes[at + 2] << 16) | (bytes[at + 3] << 24)); }
        private static string Tag(byte[] bytes, int at) { return Encoding.ASCII.GetString(bytes, at, 4); }
    }
}
