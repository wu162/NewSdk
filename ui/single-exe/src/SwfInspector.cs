// SPDX-License-Identifier: GPL-3.0-only
using System.Buffers.Binary;
using System.IO.Compression;

namespace NewSdk.Ui;

public sealed record SwfInspection(string Signature, int Version, int UncompressedLength,
    double FrameRate, int FrameCount, int SpriteCount, Dictionary<int, int> TagCounts);

// Structural preflight only. This is not a replacement for SwfLib's tag/AVM1
// semantics. Validating this envelope never implies the backend supports a tag.
public static class SwfInspector
{
    public const int MaxInputBytes = 64 * 1024 * 1024;

    public static SwfInspection Inspect(ReadOnlySpan<byte> input)
    {
        if (input.Length < 12 || input.Length > MaxInputBytes)
            throw new InvalidDataException("Invalid SWF input length.");
        if (input[1] != 'W' || input[2] != 'S') throw new InvalidDataException("Invalid SWF signature.");
        if (input[0] == 'Z') throw new NotSupportedException("ZWS/LZMA preflight is not implemented.");
        if (input[0] != 'F' && input[0] != 'C') throw new InvalidDataException("Expected FWS or CWS.");
        uint length = BinaryPrimitives.ReadUInt32LittleEndian(input[4..]);
        if (length < 12 || length > MaxInputBytes) throw new InvalidDataException("Invalid declared SWF length.");
        byte[] body;
        if (input[0] == 'C')
        {
            using var compressed = new MemoryStream(input[8..].ToArray(), writable: false);
            using var zlib = new ZLibStream(compressed, CompressionMode.Decompress);
            body = new byte[(int)length - 8];
            try { zlib.ReadExactly(body); }
            catch (EndOfStreamException ex) { throw new InvalidDataException("Truncated compressed SWF.", ex); }
            if (zlib.ReadByte() != -1) throw new InvalidDataException("SWF exceeds declared length.");
        }
        else
        {
            if (input.Length != length) throw new InvalidDataException("SWF declared length mismatch.");
            body = input[8..].ToArray();
        }
        int rectBits = body[0] >> 3;
        if (rectBits == 0) throw new InvalidDataException("Invalid SWF rectangle.");
        int header = (5 + 4 * rectBits + 7) / 8;
        if (body.Length < header + 4) throw new InvalidDataException("Truncated SWF movie header.");
        double rate = BinaryPrimitives.ReadUInt16LittleEndian(body.AsSpan(header)) / 256.0;
        int frames = BinaryPrimitives.ReadUInt16LittleEndian(body.AsSpan(header + 2));
        Dictionary<int, int> counts = new();
        ParseTimeline(body.AsSpan(header + 4), counts, 0);
        return new SwfInspection(input[0] == 'C' ? "CWS" : "FWS", input[3], (int)length,
            rate, frames, counts.GetValueOrDefault(39), counts);
    }

    private static void ParseTimeline(ReadOnlySpan<byte> data, Dictionary<int, int> counts, int depth)
    {
        if (depth > 64) throw new InvalidDataException("SWF sprite nesting exceeds limit.");
        int offset = 0;
        while (offset < data.Length)
        {
            if (data.Length - offset < 2) throw new InvalidDataException("Truncated SWF tag header.");
            ushort tag = BinaryPrimitives.ReadUInt16LittleEndian(data[offset..]); offset += 2;
            int code = tag >> 6;
            uint size = (uint)(tag & 63);
            if (size == 63)
            {
                if (data.Length - offset < 4) throw new InvalidDataException("Truncated long SWF tag.");
                size = BinaryPrimitives.ReadUInt32LittleEndian(data[offset..]); offset += 4;
            }
            if (size > data.Length - offset) throw new InvalidDataException("Truncated SWF tag payload.");
            counts[code] = counts.GetValueOrDefault(code) + 1;
            if (code == 39)
            {
                if (size < 4) throw new InvalidDataException("Truncated Sprite header.");
                ParseTimeline(data.Slice(offset + 4, (int)size - 4), counts, depth + 1);
            }
            offset += (int)size;
            if (code == 0)
            {
                if (size != 0 || offset != data.Length) throw new InvalidDataException("Invalid End tag or trailing timeline bytes.");
                return;
            }
        }
        throw new InvalidDataException("Missing SWF End tag.");
    }
}
