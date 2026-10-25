using System;
using System.IO;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Original AIFF importer 0x384a6..0x384f6 reads INST sustain IDs;
    // 0x3876d..0x387cc resolves MARK positions and subtracts one at end.
    // Deliberately restricted to one forward sustain loop and its two markers.
    internal static class AudioAiffLoopReader
    {
        internal static bool TryRead(byte[] bytes, out int start, out int end)
        {
            start = end = -1;
            if (bytes == null) throw new ArgumentNullException("bytes");
            if (bytes.Length < 12 || !Tag(bytes, 0, "FORM") || !Tag(bytes, 8, "AIFF"))
                return false;
            long limit = 8L + U32(bytes, 4);
            if (limit < 12 || limit > bytes.Length)
                throw new InvalidDataException("Truncated AIFF container");
            int instrument = -1, markers = -1, markerBytes = 0;
            for (long cursor = 12; cursor < limit; )
            {
                if (cursor + 8 > limit) throw new InvalidDataException("Truncated AIFF chunk header");
                int at = checked((int)cursor);
                uint size = U32(bytes, at + 4);
                long next = cursor + 8 + size;
                if (next > limit) throw new InvalidDataException("Truncated AIFF chunk");
                if (Tag(bytes, at, "INST"))
                {
                    if (instrument >= 0) throw new NotSupportedException("Multiple AIFF instrument chunks have not been recovered");
                    if (size != 20) throw new NotSupportedException("Only the standard AIFF instrument chunk has been recovered");
                    instrument = at + 8;
                }
                else if (Tag(bytes, at, "MARK"))
                {
                    if (markers >= 0) throw new NotSupportedException("Multiple AIFF marker chunks have not been recovered");
                    if (size < 2) throw new InvalidDataException("Truncated AIFF marker chunk");
                    markers = at + 8;
                    markerBytes = checked((int)size);
                }
                cursor = next + (size & 1u);
                if (cursor > limit) throw new InvalidDataException("Missing AIFF chunk padding");
            }
            if (instrument < 0 && markers < 0) return false;
            if (instrument < 0 || markers < 0)
                throw new NotSupportedException("AIFF sustain loops require both instrument and marker chunks");
            if (U16(bytes, instrument + 8) != 1 || U16(bytes, instrument + 14) != 0)
                throw new NotSupportedException("Only forward AIFF sustain loops without release loops have been recovered");
            int beginId = U16(bytes, instrument + 10), endId = U16(bytes, instrument + 12);
            if (beginId == 0 || endId == 0 || beginId == endId)
                throw new NotSupportedException("AIFF sustain loops require distinct positive marker IDs");
            if (U16(bytes, markers) != 2)
                throw new NotSupportedException("Only the two AIFF sustain markers have been recovered");
            long markerLimit = (long)markers + markerBytes;
            long record = markers + 2;
            uint first = uint.MaxValue, final = uint.MaxValue;
            for (int index = 0; index < 2; ++index)
            {
                if (record + 7 > markerLimit) throw new InvalidDataException("Truncated AIFF marker record");
                int at = checked((int)record);
                int id = U16(bytes, at), nameLength = bytes[at + 6];
                uint position = U32(bytes, at + 2);
                if (position > int.MaxValue) throw new NotSupportedException("AIFF marker position is out of range");
                if (id == beginId && first == uint.MaxValue) first = position;
                else if (id == endId && final == uint.MaxValue) final = position;
                else throw new NotSupportedException("AIFF loop markers must be unique and match the sustain IDs");
                record += 6 + ((nameLength + 2) & ~1);
                if (record > markerLimit) throw new InvalidDataException("Truncated AIFF marker name");
            }
            if (record != markerLimit) throw new NotSupportedException("Trailing AIFF marker data has not been recovered");
            if (first == uint.MaxValue || final == uint.MaxValue || final <= first)
                throw new NotSupportedException("AIFF sustain loop markers must form a nonempty range");
            start = (int)first;
            end = (int)final - 1;
            return true;
        }

        private static bool Tag(byte[] bytes, int at, string value)
        { return bytes[at] == value[0] && bytes[at + 1] == value[1] && bytes[at + 2] == value[2] && bytes[at + 3] == value[3]; }
        private static int U16(byte[] bytes, int at) { return (bytes[at] << 8) | bytes[at + 1]; }
        private static uint U32(byte[] bytes, int at)
        { return (uint)((bytes[at] << 24) | (bytes[at + 1] << 16) | (bytes[at + 2] << 8) | bytes[at + 3]); }
    }
}
