using System;
using System.IO;
using System.Collections.Generic;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Original AIFF importer 0x384a6..0x384f6 reads INST sustain IDs;
    // 0x3876d..0x387cc resolves MARK positions and subtracts one at end.
    // Bounded metadata import: the last INST selects sustain IDs, then all
    // MARK groups are resolved in file order, as in the native importer.
    internal static class AudioAiffLoopReader
    {
        internal static bool TryRead(byte[] bytes, out int start, out int end)
        {
            start = end = -1;
            if (bytes == null) throw new ArgumentNullException("bytes");
            if (bytes.Length < 12 || !Tag(bytes, 0, "FORM") || (!Tag(bytes, 8, "AIFF") && !Tag(bytes, 8, "AIFC")))
                return false;
            long limit = 8L + U32(bytes, 4);
            if (limit < 12 || limit > bytes.Length)
                throw new InvalidDataException("Truncated AIFF container");
            int instrument = -1;
            var markerChunks = new List<int>();
            var markerSizes = new List<int>();
            for (long cursor = 12; cursor < limit; )
            {
                if (cursor + 8 > limit) throw new InvalidDataException("Truncated AIFF chunk header");
                int at = checked((int)cursor);
                uint size = U32(bytes, at + 4);
                long next = cursor + 8 + size;
                if (next > limit) throw new InvalidDataException("Truncated AIFF chunk");
                if (Tag(bytes, at, "INST"))
                {
                    if (size != 20) throw new NotSupportedException("Only the standard AIFF instrument chunk has been recovered");
                    instrument = at + 8;
                }
                else if (Tag(bytes, at, "MARK"))
                {
                    if (size < 2) throw new InvalidDataException("Truncated AIFF marker chunk");
                    if (markerChunks.Count >= 200)
                        throw new NotSupportedException("AIFF marker group limit exceeded");
                    markerChunks.Add(at + 8);
                    markerSizes.Add(checked((int)size));
                }
                cursor = next + (size & 1u);
                if (cursor > limit) throw new InvalidDataException("Missing AIFF chunk padding");
            }
            if (instrument < 0 && markerChunks.Count == 0) return false;
            if (instrument < 0 || markerChunks.Count == 0)
                throw new NotSupportedException("AIFF sustain metadata requires both instrument and marker chunks");
            int mode = U16(bytes, instrument + 8);
            int beginId = U16(bytes, instrument + 10), endId = U16(bytes, instrument + 12);
            if (mode == 1 && (beginId == 0 || endId == 0 || beginId == endId))
                throw new NotSupportedException("AIFF sustain loops require distinct positive marker IDs");
            uint first = uint.MaxValue, final = uint.MaxValue;
            for (int group = 0; group < markerChunks.Count; ++group)
            {
                int markers = markerChunks[group], markerBytes = markerSizes[group];
                int count = U16(bytes, markers);
                // Native allocation is 0x324 bytes: a count and 200 pointers.
                // Larger counts would overwrite native memory, not define a format feature.
                if (count > 200) throw new NotSupportedException("AIFF marker record limit exceeded");
                long markerLimit = (long)markers + markerBytes;
                long record = markers + 2;
                for (int index = 0; index < count; ++index)
                {
                    if (record + 7 > markerLimit) throw new InvalidDataException("Truncated AIFF marker record");
                    int at = checked((int)record);
                    int id = U16(bytes, at), nameLength = bytes[at + 6];
                    uint position = U32(bytes, at + 2);
                    if (position > int.MaxValue) throw new NotSupportedException("AIFF marker position is out of range");
                    // Both native comparisons are independent; selected IDs are distinct.
                    // Every later matching record replaces the previous position.
                    if (mode == 1 && id == beginId) first = position;
                    if (mode == 1 && id == endId) final = position;
                    record += 6 + ((nameLength + 2) & ~1);
                    if (record > markerLimit) throw new InvalidDataException("Truncated AIFF marker name");
                }
                if (record != markerLimit) throw new NotSupportedException("Trailing AIFF marker data has not been recovered");
            }
            // Native reads sustain mode only. Release fields are ignored, and
            // other sustain modes do not install a runtime loop.
            if (mode != 1) return false;
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
