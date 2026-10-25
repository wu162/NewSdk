using System;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Closed integer helpers from original AudioCompiler RVAs 0xe890 and
    // 0xea80. These do not encode XMA audio or enable representation 28.
    public static class XmaPacketBits
    {
        // Original thresholds are c0/30c0/c30c0/30c30c0. The 2/3/4-byte
        // forms subtract the previous threshold and add their prefix; the
        // five-byte escape retains the complete original uint32. This is
        // neither LEB128 nor a general UTF-8 integer representation.
        private static int CountSize(uint value)
        {
            if (value < 0xc0u) return 1;
            if (value < 0x30c0u) return 2;
            if (value < 0xc30c0u) return 3;
            if (value < 0x30c30c0u) return 4;
            return 5;
        }

        public static byte[] WriteCount(uint value)
        {
            byte[] destination = new byte[CountSize(value)];
            WriteCount(value, destination, 0);
            return destination;
        }

        // Returns the exact native byte count. The bounded managed policy
        // validates the entire destination before modifying any byte; the
        // native routine has no capacity parameter or defined error return.
        public static int WriteCount(uint value, byte[] destination, int offset)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            int size = CountSize(value);
            if (offset < 0 || offset > destination.Length - size)
                throw new ArgumentOutOfRangeException("offset", "Insufficient count output capacity");
            uint encoded;
            unchecked
            {
                if (size == 1)
                {
                    destination[offset] = (byte)value;
                }
                else if (size == 2)
                {
                    encoded = unchecked(value - 0xc0u) | 0xc000u;
                    destination[offset] = (byte)(encoded >> 8);
                    destination[offset + 1] = (byte)encoded;
                }
                else if (size == 3)
                {
                    encoded = unchecked(value - 0x30c0u) | 0xf00000u;
                    destination[offset] = (byte)(encoded >> 16);
                    destination[offset + 1] = (byte)(encoded >> 8);
                    destination[offset + 2] = (byte)encoded;
                }
                else if (size == 4)
                {
                    encoded = unchecked(value - 0xc30c0u) | 0xfc000000u;
                    destination[offset] = (byte)(encoded >> 24);
                    destination[offset + 1] = (byte)(encoded >> 16);
                    destination[offset + 2] = (byte)(encoded >> 8);
                    destination[offset + 3] = (byte)encoded;
                }
                else
                {
                    destination[offset] = 0xff;
                    destination[offset + 1] = (byte)(value >> 24);
                    destination[offset + 2] = (byte)(value >> 16);
                    destination[offset + 3] = (byte)(value >> 8);
                    destination[offset + 4] = (byte)value;
                }
            }
            return size;
        }

        // Exact RVA 0xea80 accumulator, including its three-byte lookahead
        // and x86 masked 32-bit shifts. The source bit offset is not advanced.
        // For positive count, source[bitOffset/8 .. bitOffset/8+2] must exist
        // regardless of how few bits are requested. Negative offsets and
        // truncated lookahead are outside the recovered safe native domain.
        // Counts above 24-(bitOffset&7) can shift in zeros; counts >=32 wrap
        // the shift counts. Preserve this native behavior rather than reading
        // additional source bytes as a generic bit-stream reader would.
        public static uint Accumulate(uint initial, byte[] source, int bitOffset, int count)
        {
            // The original exits before dereferencing either pointer here.
            if (count <= 0) return initial;
            if (source == null) throw new ArgumentNullException("source");
            if (bitOffset < 0) throw new ArgumentOutOfRangeException("bitOffset");
            int index = bitOffset / 8;
            if (index > source.Length - 3)
                throw new ArgumentOutOfRangeException("bitOffset", "Three source bytes are required for native lookahead");
            unchecked
            {
                uint lookahead = ((uint)source[index] << 24) |
                    ((uint)source[index + 1] << 16) | ((uint)source[index + 2] << 8);
                lookahead <<= bitOffset & 7;
                lookahead >>= (32 - count) & 31;
                return (initial << (count & 31)) | lookahead;
            }
        }
    }
}
