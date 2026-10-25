using System;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Original XMA output metadata group: EA format (5f170), XMA2 WAVE
    // format (5f000) and cumulative big-endian seek table (5eea0).
    // Inputs are the original encoder's per-stream 32-byte format records
    // and block frame counts, not raw PCM. This does not implement encoding.
    public static class XmaCodecMetadata
    {
        private static void Validate(byte[][] streamFormats, uint[] blockFrameCounts)
        {
            if (streamFormats == null) throw new ArgumentNullException("streamFormats");
            if (blockFrameCounts == null) throw new ArgumentNullException("blockFrameCounts");
            // The native EA writer has a fixed 0x428-byte local format area:
            // 40 bytes plus four bytes for each of at most 256 streams.
            if (streamFormats.Length < 1 || streamFormats.Length > 256)
                throw new ArgumentOutOfRangeException("streamFormats");
            foreach (byte[] format in streamFormats)
                if (format == null || format.Length != 32)
                    throw new ArgumentException("Each native stream format must contain exactly 32 bytes", "streamFormats");
        }
        private static uint U16(byte[] data, int offset) { return (uint)(data[offset] | (data[offset + 1] << 8)); }
        private static uint U32(byte[] data, int offset)
        { return (uint)data[offset] | ((uint)data[offset + 1] << 8) | ((uint)data[offset + 2] << 16) | ((uint)data[offset + 3] << 24); }
        private static void Be32(byte[] data, int offset, uint value)
        { data[offset] = (byte)(value >> 24); data[offset + 1] = (byte)(value >> 16); data[offset + 2] = (byte)(value >> 8); data[offset + 3] = (byte)value; }
        private static void Le16(byte[] data, int offset, uint value)
        { data[offset] = (byte)value; data[offset + 1] = (byte)(value >> 8); }
        private static void Le32(byte[] data, int offset, uint value)
        { data[offset] = (byte)value; data[offset + 1] = (byte)(value >> 8); data[offset + 2] = (byte)(value >> 16); data[offset + 3] = (byte)(value >> 24); }
        private static uint TotalSamples(uint[] counts)
        { unchecked { uint sum = 0; foreach (uint count in counts) sum += count; return sum << 9; } }
        private static uint AdjustLoop(uint value) { return value == 0 ? 0 : unchecked(value - 64u); }

        public static byte[] BuildEaFormat(byte[][] streamFormats, uint[] blockFrameCounts,
            uint blockBytes, uint playLength, uint loopBegin, uint loopEnd)
        {
            Validate(streamFormats, blockFrameCounts);
            byte[] first = streamFormats[0], output = new byte[40 + 4 * streamFormats.Length];
            output[0] = 4; output[1] = unchecked((byte)streamFormats.Length); output[3] = first[10];
            Be32(output, 4, AdjustLoop(loopBegin)); Be32(output, 8, AdjustLoop(loopEnd));
            Be32(output, 12, U32(first, 16)); Be32(output, 16, U16(first, 4)); Be32(output, 20, U32(first, 12));
            Be32(output, 24, blockBytes); Be32(output, 28, TotalSamples(blockFrameCounts));
            Be32(output, 32, playLength); Be32(output, 36, (uint)blockFrameCounts.Length);
            for (int i = 0; i < streamFormats.Length; ++i)
            {
                byte[] format = streamFormats[i]; int offset = 40 + 4 * i;
                output[offset] = format[29]; output[offset + 2] = format[31]; output[offset + 3] = format[30];
            }
            return output;
        }

        public static byte[] BuildWaveFormat(byte[][] streamFormats, uint[] blockFrameCounts,
            uint blockBytes, uint playLength, uint loopBegin, uint loopEnd)
        {
            Validate(streamFormats, blockFrameCounts);
            uint channels = 0, sourceMask = 0;
            foreach (byte[] format in streamFormats)
            {
                channels += format[29];
                // The original ORs the two bytes of the native 16-bit mask,
                // then ORs streams. It does not simply use that uint16 value.
                sourceMask |= (uint)(format[30] | format[31]);
            }
            uint channelMask = (sourceMask & 15u) | ((sourceMask & 16u) << 5) |
                ((sourceMask & 32u) << 5) | ((sourceMask & 64u) >> 2) | ((sourceMask & 128u) >> 2);
            byte[] first = streamFormats[0], output = new byte[52];
            Le16(output, 0, 0x166); Le16(output, 2, channels);
            Le32(output, 4, U32(first, 16)); Le32(output, 8, U32(first, 12));
            Le16(output, 12, channels * 2); Le16(output, 14, 16); Le16(output, 16, 34);
            Le16(output, 18, (uint)streamFormats.Length); Le32(output, 20, channelMask);
            Le32(output, 24, TotalSamples(blockFrameCounts)); Le32(output, 28, blockBytes);
            Le32(output, 36, playLength); uint adjusted = AdjustLoop(loopBegin); Le32(output, 40, adjusted);
            Le32(output, 44, loopEnd == 0 ? 0 : unchecked(loopEnd - adjusted - 64u));
            output[48] = first[10]; output[49] = 4; Le16(output, 50, (uint)blockFrameCounts.Length);
            return output;
        }

        public static byte[] BuildSeekTable(uint[] blockFrameCounts)
        {
            if (blockFrameCounts == null) throw new ArgumentNullException("blockFrameCounts");
            byte[] output = new byte[checked(blockFrameCounts.Length * 4)];
            unchecked
            {
                uint cumulative = 0;
                for (int i = 0; i < blockFrameCounts.Length; ++i)
                { cumulative += blockFrameCounts[i] << 9; Be32(output, i * 4, cumulative); }
            }
            return output;
        }
    }
}
