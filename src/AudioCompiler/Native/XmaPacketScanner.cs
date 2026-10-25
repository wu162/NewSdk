using System;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Integer-only XMA packet helpers recovered from original RVAs 0xeae0
    // and 0xef60. These inspect packet structure; they do not encode XMA or
    // enable the AudioFile XMA route. Native unsafe inputs are bounded here.
    public static class XmaPacketScanner
    {
        private const int PacketBytes = 0x800;
        private const int PayloadBits = 0x3fe0;

        // Original cdecl (packet, frameBitOffset, outFrameLength).
        // The offset is in the logical payload after this packet's 32-bit
        // header. A split 15-bit length skips the next packet's header.
        // Return the frame's final continuation bit, not a length validity bit.
        public static bool ReadFrame(byte[] source, int packetByteOffset,
            int frameBitOffset, out int frameLength)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (packetByteOffset < 0 || packetByteOffset > source.Length)
                throw new ArgumentOutOfRangeException("packetByteOffset");
            if (frameBitOffset < 0 || frameBitOffset >= PayloadBits)
                throw new ArgumentOutOfRangeException("frameBitOffset");
            int remainder = frameBitOffset > 0x3fd1 ? frameBitOffset - 0x3fd1 : 0;
            int first = 15 - remainder;
            uint length = XmaPacketBits.Accumulate(0u, source,
                BitAddress(packetByteOffset, frameBitOffset + 32), first);
            length = XmaPacketBits.Accumulate(length, source,
                BitAddress(packetByteOffset, frameBitOffset + first + 64), remainder);
            frameLength = (int)length;
            int end = frameBitOffset + frameLength;
            long lastByte = (long)packetByteOffset + (end + 31) / 8;
            if (end > PayloadBits) lastByte += 4;
            if (lastByte < 0 || lastByte >= source.Length)
                throw new ArgumentException("Truncated XMA frame continuation bit", "source");
            int shift = (-end) & 7;
            return ((source[(int)lastByte] >> shift) & 1) != 0;
        }

        private static int BitAddress(int packetByteOffset, int packetBitOffset)
        {
            long address = (long)packetByteOffset * 8 + packetBitOffset;
            if (address > Int32.MaxValue)
                throw new ArgumentOutOfRangeException("packetByteOffset");
            return (int)address;
        }

        private static uint BigEndianWord(byte[] source, int offset)
        {
            if (offset < 0 || offset > source.Length - 4)
                throw new ArgumentException("Truncated XMA packet header", "source");
            return ((uint)source[offset] << 24) | ((uint)source[offset + 1] << 16) |
                ((uint)source[offset + 2] << 8) | source[offset + 3];
        }

        private static int FirstFrameOffset(byte[] source, int packetOffset)
        {
            int offset = (int)((BigEndianWord(source, packetOffset) >> 11) & 0x7fffu);
            return offset < PayloadBits ? offset : -1;
        }

        // Original cdecl (block, maximumFrames, outputCounts, outBlockBytes,
        // outCountBytes), returning the total number of frames inspected.
        // The block's first BE32 word stores its byte length in the upper
        // 30 bits. Counts are emitted per packet via the original variable-
        // length count encoding. maximumFrames is tested at terminated packet
        // boundaries, so a packet can take the result above that threshold.
        public static int Scan(byte[] source, int maximumFrames, byte[] destination,
            int destinationOffset, out int blockByteLength, out int bytesWritten)
        {
            blockByteLength = 0;
            bytesWritten = 0;
            if (source == null) throw new ArgumentNullException("source");
            if (destination == null) throw new ArgumentNullException("destination");
            if (maximumFrames <= 0) throw new ArgumentOutOfRangeException("maximumFrames");
            if (destinationOffset < 0 || destinationOffset > destination.Length)
                throw new ArgumentOutOfRangeException("destinationOffset");
            blockByteLength = (int)(BigEndianWord(source, 0) >> 2);
            if (blockByteLength < 8 || blockByteLength > source.Length)
                throw new ArgumentException("XMA block length exceeds the available bounded input", "source");
            int packetOffset = 4;
            int frameOffset = FirstFrameOffset(source, packetOffset);
            int total = 0, packetFrames = 0;
            bool continues = true;
            while (true)
            {
                if (frameOffset < 0)
                {
                    // The native sentinel path does not read the following
                    // packet's first-frame field; it resumes at payload bit0.
                    packetFrames = 0;
                    bytesWritten += XmaPacketBits.WriteCount(0u, destination,
                        checked(destinationOffset + bytesWritten));
                    packetOffset = checked(packetOffset + PacketBytes);
                    frameOffset = 0;
                    if (packetOffset >= blockByteLength) return total;
                    if (continues) continue;
                    return total;
                }
                if (packetOffset >= blockByteLength)
                    throw new ArgumentException("XMA frame continuation exceeds the declared block", "source");
                int length;
                continues = ReadFrame(source, packetOffset, frameOffset, out length);
                // A zero-length continuing frame never advances the original
                // scanner. Reject malformed streams instead of looping forever.
                if (length == 0)
                    throw new ArgumentException("A scanned XMA frame must have positive length", "source");
                frameOffset += length;
                total = checked(total + 1);
                packetFrames = checked(packetFrames + 1);
                if (frameOffset < PayloadBits && continues) continue;

                bytesWritten += XmaPacketBits.WriteCount((uint)packetFrames,
                    destination, checked(destinationOffset + bytesWritten));
                packetOffset = checked(packetOffset + PacketBytes);
                frameOffset -= PayloadBits;
                packetFrames = 0;
                if (continues)
                {
                    // ReadFrame's declared domain is one packet's starting
                    // payload. Multi-packet residual offsets are malformed.
                    if (frameOffset >= PayloadBits)
                        throw new ArgumentException("XMA frame spans unsupported multiple payloads", "source");
                    continue;
                }
                if (packetOffset >= blockByteLength || total >= maximumFrames) return total;
                frameOffset = FirstFrameOffset(source, packetOffset);
                continues = true;
            }
        }
    }
}
