using System;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Original RVA 0xee00: convert one raw codec block to the EA packet block.
    // This is output framing only; it does not encode audio or enable XMA.
    public static class XmaOutputRewriter
    {
        private const int PacketBytes = 0x800;

        // The source block is an integral number of packets. streamCount is
        // outer SIMEX object +0x2c = (channels+1)/2. Native code walks each
        // stream from its first interleaved packet, following header byte 3.
        // The final packet of each stream must contain a non-ff byte, so its
        // backwards padding scan is bounded to that allocated packet.
        private static int Measure(byte[] source, int sourceLength, int streamCount,
            out int maximumCopiedLength)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (sourceLength < 0 || sourceLength > source.Length || sourceLength % PacketBytes != 0)
                throw new ArgumentOutOfRangeException("sourceLength", "A complete bounded raw packet block is required");
            if (streamCount < 0 || streamCount > sourceLength / PacketBytes)
                throw new ArgumentOutOfRangeException("streamCount");
            maximumCopiedLength = 0;
            if (streamCount == 0) return 0;
            long output = 0;
            for (int stream = 0; stream < streamCount; ++stream)
            {
                output += 4;
                long position = (long)stream * PacketBytes;
                int last = 0;
                do
                {
                    last = (int)position;
                    output += PacketBytes;
                    if (output > Int32.MaxValue)
                        throw new ArgumentOutOfRangeException("streamCount", "Packet output exceeds the managed addressable domain");
                    maximumCopiedLength = Math.Max(maximumCopiedLength, (int)output);
                    position += ((long)source[last + 3] + 1) * PacketBytes;
                } while (position < sourceLength);
                int padding = 0;
                while (padding < PacketBytes && source[last + PacketBytes - 1 - padding] == 0xff) ++padding;
                if (padding == PacketBytes)
                    throw new ArgumentException("Native padding scan would leave its final packet", "source");
                output -= padding;
            }
            return (int)output;
        }

        // Returns the native byte count. Full packets are copied before their
        // padding is trimmed, so destination can contain copied ff bytes past
        // the returned logical length. Capacity must cover those writes too.
        // All bounds are checked before any destination byte is modified.
        public static int Rewrite(byte[] source, int sourceLength, int streamCount,
            uint rateFlags, byte[] destination, int destinationOffset)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (Object.ReferenceEquals(source, destination))
                throw new ArgumentException("Source and destination must not alias", "destination");
            int maximum;
            int length = Measure(source, sourceLength, streamCount, out maximum);
            if (destinationOffset < 0 || destinationOffset > destination.Length - maximum)
                throw new ArgumentOutOfRangeException("destinationOffset", "Insufficient capacity for native full-packet writes");
            if (streamCount == 0) return 0;
            int output = 0;
            unchecked
            {
                for (int stream = 0; stream < streamCount; ++stream)
                {
                    int blockStart = output;
                    output += 4;
                    long position = (long)stream * PacketBytes;
                    int last = 0;
                    do
                    {
                        last = (int)position;
                        Array.Copy(source, last, destination, destinationOffset + output, PacketBytes);
                        destination[destinationOffset + output + 3] = 0;
                        output += PacketBytes;
                        position += ((long)source[last + 3] + 1) * PacketBytes;
                    } while (position < sourceLength);
                    int padding = 0;
                    while (source[last + PacketBytes - 1 - padding] == 0xff) ++padding;
                    output -= padding;
                    uint header = ((uint)(output - blockStart) << 2) | rateFlags;
                    destination[destinationOffset + blockStart] = (byte)(header >> 24);
                    destination[destinationOffset + blockStart + 1] = (byte)(header >> 16);
                    destination[destinationOffset + blockStart + 2] = (byte)(header >> 8);
                    destination[destinationOffset + blockStart + 3] = (byte)header;
                }
            }
            return length;
        }

        public static byte[] Rewrite(byte[] source, int streamCount, uint rateFlags)
        {
            int maximum;
            int length = Measure(source, source == null ? 0 : source.Length, streamCount, out maximum);
            byte[] temporary = new byte[maximum];
            Rewrite(source, source.Length, streamCount, rateFlags, temporary, 0);
            byte[] output = new byte[length];
            Array.Copy(temporary, output, length);
            return output;
        }
    }
}
