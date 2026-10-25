using System;
using System.IO;
using System.Text;

namespace BinaryAssetBuilder.AudioCompiler
{
    // SIMEX WAVE importer 0x37210 and AIFF importer 0x381c0. This reader
    // deliberately accepts only uncompressed, integral, mono/stereo input.
    internal static class AudioPcmInputReader
    {
        internal static Pcm16Encoder.PcmWaveInput Read(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");
            if (bytes.Length < 12) throw new InvalidDataException("Truncated audio file");
            bool aiff = Tag(bytes, 0) == "FORM" && Tag(bytes, 8) == "AIFF";
            bool wave = Tag(bytes, 0) == "RIFF" && Tag(bytes, 8) == "WAVE";
            if (!aiff && !wave) throw new NotSupportedException("Uncompressed AIFF or PCM WAVE input is required");
            int channels = 0, rate = 0, bits = 0, format = 0, align = 0;
            int dataOffset = -1, dataLength = 0;
            long end = 8L + U32(bytes, 4, aiff);
            if (end < 12 || end > bytes.Length) throw new InvalidDataException("Truncated audio container");
            for (long at = 12; at + 8 <= end; )
            {
                int offset = checked((int)at);
                uint size = U32(bytes, offset + 4, aiff);
                long next = at + 8 + size;
                if (next > end) throw new InvalidDataException("Truncated audio chunk");
                int body = offset + 8;
                string tag = Tag(bytes, offset);
                if (wave && tag == "fmt ")
                {
                    if (size < 16) throw new InvalidDataException("Truncated WAVE format");
                    format = U16(bytes, body, false);
                    channels = U16(bytes, body + 2, false);
                    rate = checked((int)U32(bytes, body + 4, false));
                    align = U16(bytes, body + 12, false);
                    bits = U16(bytes, body + 14, false);
                }
                else if (aiff && tag == "COMM")
                {
                    if (size < 18) throw new InvalidDataException("Truncated AIFF common chunk");
                    channels = U16(bytes, body, true);
                    bits = U16(bytes, body + 6, true);
                    // The original importer uses the high 32 significand bits,
                    // not a general extended-float conversion (0x3837f..383ca).
                    int exponent = U16(bytes, body + 8, true);
                    int shift = 0x401e - exponent;
                    if (shift < 0 || shift > 31) throw new NotSupportedException("Unsupported AIFF sample rate");
                    rate = checked((int)(U32(bytes, body + 10, true) >> shift));
                    format = 1;
                }
                else if (wave && tag == "data") { dataOffset = body; dataLength = checked((int)size); }
                else if (aiff && tag == "SSND")
                {
                    if (size < 8) throw new InvalidDataException("Truncated AIFF sound chunk");
                    if (U32(bytes, body, true) != 0 || U32(bytes, body + 4, true) != 0)
                        throw new NotSupportedException("AIFF sound offsets and block sizes have not been recovered");
                    // Native rounds the chunk size before deriving the sample
                    // count. For odd mono8 data the pad becomes one sample.
                    if ((size & 1) != 0 && next >= end) throw new InvalidDataException("Missing AIFF chunk padding");
                    dataOffset = body + 8; dataLength = checked((int)(size + (size & 1)) - 8);
                }
                else if ((wave && tag == "smpl") || (aiff && (tag == "INST" || tag == "MARK")))
                    throw new NotSupportedException("Audio loop and marker metadata has not been recovered");
                at = next + (size & 1);
            }
            if (format != 1 || (bits != 8 && bits != 16 && bits != 24) || (channels != 1 && channels != 2))
                throw new NotSupportedException("Only mono/stereo integral PCM8/16/24 input is supported");
            int width = bits / 8, frameSize = channels * width;
            if (rate <= 0 || rate > 0x3ffff || (!aiff && align != frameSize) || dataOffset < 0 || (!aiff && dataLength % frameSize != 0))
                throw new InvalidDataException("Invalid PCM audio format");
            int frames = dataLength / frameSize;
            // Native AIFF importer corrects COMM's count from the SSND length
            // (0x388f8..38949). A mismatch therefore does not truncate samples.
            if (frames == 0 || frames > 0x1fffffff) throw new NotSupportedException("Unsupported PCM frame count");
            float[] samples = new float[frames * channels];
            for (int i = 0, at = dataOffset; i < samples.Length; ++i, at += width)
            {
                if (bits == 8) samples[i] = (aiff ? (int)unchecked((sbyte)bytes[at]) : bytes[at] - 128) * 0.0078125f;
                else if (bits == 16) samples[i] = unchecked((short)U16(bytes, at, aiff)) * (1.0f / 32768.0f);
                else
                {
                    // Native P2L0/P2B0 (0x10670/0x10830) place the three
                    // bytes in bits31..8, cvtsi2ss, then multiply by 2^-31.
                    int signed = aiff ? (bytes[at] << 24) | (bytes[at + 1] << 16) | (bytes[at + 2] << 8) :
                        (bytes[at + 2] << 24) | (bytes[at + 1] << 16) | (bytes[at] << 8);
                    samples[i] = signed * (1.0f / 2147483648.0f);
                }
            }
            return new Pcm16Encoder.PcmWaveInput { Samples = samples, Channels = channels, SampleRate = rate };
        }
        private static string Tag(byte[] b, int at) { return Encoding.ASCII.GetString(b, at, 4); }
        private static int U16(byte[] b, int at, bool be) { return be ? (b[at] << 8) | b[at + 1] : b[at] | (b[at + 1] << 8); }
        private static uint U32(byte[] b, int at, bool be)
        {
            return be ? (uint)((b[at] << 24) | (b[at + 1] << 16) | (b[at + 2] << 8) | b[at + 3]) :
                (uint)(b[at] | (b[at + 1] << 8) | (b[at + 2] << 16) | (b[at + 3] << 24));
        }
    }
}
