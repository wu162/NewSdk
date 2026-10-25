using System;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Raw XMA2 packet construction helpers used by 5e8f0/5ec90, before the
    // EA output rewriter. These do not perform numerical audio encoding.
    public static class XmaRawPacketWriter
    {
        private const int PacketBytes = 2048;
        private static int Packet(byte[] packets, int index)
        {
            if (packets == null) throw new ArgumentNullException("packets");
            if (index < 0 || index >= packets.Length / PacketBytes)
                throw new ArgumentOutOfRangeException("index");
            return index * PacketBytes;
        }
        private static uint Be32(byte[] b, int p) { return ((uint)b[p] << 24) | ((uint)b[p + 1] << 16) | ((uint)b[p + 2] << 8) | b[p + 3]; }
        private static void Put32(byte[] b, int p, uint x)
        { b[p] = (byte)(x >> 24); b[p + 1] = (byte)(x >> 16); b[p + 2] = (byte)(x >> 8); b[p + 3] = (byte)x; }

        // RVA 5dc70 ORs the existing frame-count bits, rather than replacing
        // them. A zero count writes the first-frame-offset sentinel 0x7fff.
        public static void FinalizeHeader(byte[] packets, int packetIndex, uint frameCount)
        {
            int p = Packet(packets, packetIndex);
            uint header = Be32(packets, p) | unchecked(frameCount << 26) | 0x100u;
            if (frameCount == 0) header = (header & 0xfffff800u) | 0x03fff800u;
            Put32(packets, p, header);
        }

        // RVA 5dcf0 updates stream slot +c/+8/+10 and global next-packet
        // counter +403c. All source/destination indices are validated first.
        public static void StartNextPacket(byte[] packets, ref int packetIndex,
            ref int nextPacketIndex, uint firstFrameOffset, out uint frameCount, out int bitCount)
        {
            int old = Packet(packets, packetIndex), next = Packet(packets, nextPacketIndex);
            int selected = nextPacketIndex;
            Array.Clear(packets, next, PacketBytes);
            Put32(packets, next, (firstFrameOffset & 0x7fffu) << 11);
            packets[old + 3] = unchecked((byte)(selected - packetIndex - 1));
            packetIndex = selected; nextPacketIndex = selected + 1; frameCount = 0; bitCount = 32;
        }

        // RVAs 5c390/5c3b0: MSB-first addressing. The bounded managed policy
        // rejects offsets outside the supplied byte array before any write.
        public static void SetBit(byte[] data, int bitOffset, bool value)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (bitOffset < 0 || (long)bitOffset >= (long)data.Length * 8)
                throw new ArgumentOutOfRangeException("bitOffset");
            int index = bitOffset / 8, mask = 0x80 >> (bitOffset & 7);
            data[index] = (byte)(value ? data[index] | mask : data[index] & ~mask);
        }

        // RVA 5ddb0 sets alignment bits and all remaining packet bytes to ff.
        public static void PadPacket(byte[] packets, int packetIndex, int bitCount)
        {
            int p = Packet(packets, packetIndex);
            if (bitCount < 0 || bitCount > PacketBytes * 8)
                throw new ArgumentOutOfRangeException("bitCount");
            int bit = bitCount;
            while ((bit & 7) != 0) { packets[p + bit / 8] |= (byte)(0x80 >> (bit & 7)); ++bit; }
            for (int i = bit / 8; i < PacketBytes; ++i) packets[p + i] = 0xff;
        }

        // RVA 5c270 is not a general bit-preserving memcpy. Each destination
        // chunk clears the available low bits, including beyond a short final
        // chunk. Nonaligned source chunks always read a second source byte.
        // Counts <=0 return unchanged without dereferencing either pointer.
        public static int CopyBits(byte[] source, int sourceBitOffset, byte[] destination,
            int destinationBitOffset, int count)
        {
            if (count <= 0) return count;
            if (source == null) throw new ArgumentNullException("source");
            if (destination == null) throw new ArgumentNullException("destination");
            if (Object.ReferenceEquals(source, destination))
                throw new ArgumentException("Native forward-overlap semantics are outside the independent-buffer domain", "destination");
            if (sourceBitOffset < 0 || destinationBitOffset < 0)
                throw new ArgumentOutOfRangeException("sourceBitOffset");
            int sourceRemainder = sourceBitOffset & 7, destinationRemainder = destinationBitOffset & 7;
            long chunks = ((long)count + 7) / 8;
            long sourceLast = sourceBitOffset / 8 + chunks - 1 + (sourceRemainder == 0 ? 0 : 1);
            long destinationLast = ((long)destinationBitOffset + count - 1) / 8;
            if (sourceLast >= source.Length || destinationLast >= destination.Length)
                throw new ArgumentOutOfRangeException("count", "Complete native chunk lookahead and write capacity are required");
            int s = sourceBitOffset / 8, d = destinationBitOffset / 8, remaining = count;
            int mask = 0xff >> destinationRemainder;
            while (remaining > 0)
            {
                int value = (source[s] << sourceRemainder) & 255;
                if (sourceRemainder != 0) value |= source[s + 1] >> (8 - sourceRemainder);
                if (remaining < 8) value &= (0xff << (8 - remaining)) & 255;
                destination[d] = (byte)((destination[d] & ~mask) | (value >> destinationRemainder));
                if (destinationRemainder != 0 && remaining > 8 - destinationRemainder)
                {
                    int lowerMask = (0xff << (8 - destinationRemainder)) & 255;
                    destination[d + 1] = (byte)((destination[d + 1] & ~lowerMask) | ((value << (8 - destinationRemainder)) & 255));
                }
                ++s; ++d; remaining -= 8;
            }
            return count;
        }
    }
}
