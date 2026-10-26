// SPDX-License-Identifier: GPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Buffers.Binary;
using SwfLib;
using SwfLib.Tags;

namespace eaf2apt
{
    // This is a conversion component, not a BAB plugin. The caller owns these arrays.
    public sealed record MemoryAssets(string LogicalName, byte[] Apt, byte[] Constants, byte[] Dat);

    internal static class ScriptOnlyGuard
    {
        // Supported tags: End, ShowFrame, SetBackgroundColor, DoAction,
        // DefineSprite (EMPTY ONLY), ExportAssets, DoInitAction.
        // Restrict to the actual fixture's capabilities until more are verified.
        private static readonly HashSet<int> Allowed = new() { 0, 1, 9, 12, 39, 56, 59 };

        public static void Validate(byte[] bytes, string name)
        {
            if (bytes == null || bytes.Length < 8) throw new InvalidDataException("Truncated SWF header");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Empty logical name");
            foreach (char c in name)
                if (!(char.IsAsciiLetterOrDigit(c) || c == '_'))
                    throw new ArgumentException("Logical name must contain only ASCII letters, digits or underscore");
            if (bytes[3] != 7) throw new NotSupportedException("Only SWF 7 is currently verified");
            ValidateEnvelope(bytes);
            using var input = new MemoryStream(bytes, writable: false);
            var movie = SwfFile.ReadFrom(input);
            foreach (var tag in movie.Tags)
            {
                if (!Allowed.Contains((int)tag.TagType))
                    throw new NotSupportedException("Unsupported tag: " + tag.TagType);
                if (tag.RestData != null && tag.RestData.Length != 0)
                    throw new InvalidDataException("Unparsed tag data: " + tag.TagType);
                if (tag is DefineSpriteTag sprite)
                {
                    // Nonempty sprites need a separate timeline/frame validation milestone.
                    if (sprite.FramesCount != 0 || sprite.Tags.Count != 1 || (int)sprite.Tags[0].TagType != 0)
                        throw new NotSupportedException("Only empty sprites are currently supported");
                }
            }
        }

        private static void ValidateEnvelope(byte[] input)
        {
            const int limit = 32 * 1024 * 1024;
            uint size = BinaryPrimitives.ReadUInt32LittleEndian(input.AsSpan(4));
            if (size < 12 || size > limit) throw new InvalidDataException("Invalid or oversized SWF length");
            if (input[1] != 'W' || input[2] != 'S' || (input[0] != 'F' && input[0] != 'C'))
                throw new InvalidDataException("Only FWS and CWS are accepted");
            byte[] body;
            if (input[0] == 'C')
            {
                using var compressed = new MemoryStream(input, 8, input.Length - 8);
                using var zlib = new ZLibStream(compressed, CompressionMode.Decompress);
                body = new byte[(int)size - 8];
                zlib.ReadExactly(body);
                if (zlib.ReadByte() != -1) throw new InvalidDataException("SWF exceeds declared length");
            }
            else
            {
                if (input.Length != size) throw new InvalidDataException("SWF declared length mismatch");
                body = input.AsSpan(8).ToArray();
            }
            int rect = (5 + 4 * (body[0] >> 3) + 7) / 8;
            int pos = rect + 4;
            if (pos > body.Length) throw new InvalidDataException("Truncated movie header");
            bool ended = false;
            while (pos < body.Length)
            {
                if (body.Length - pos < 2) throw new InvalidDataException("Truncated tag header");
                ushort header = BinaryPrimitives.ReadUInt16LittleEndian(body.AsSpan(pos)); pos += 2;
                int code = header >> 6;
                uint length = (uint)(header & 63);
                if (length == 63)
                {
                    if (body.Length - pos < 4) throw new InvalidDataException("Truncated long tag");
                    length = BinaryPrimitives.ReadUInt32LittleEndian(body.AsSpan(pos)); pos += 4;
                }
                if (length > body.Length - pos) throw new InvalidDataException("Truncated tag payload");
                if (!Allowed.Contains(code)) throw new NotSupportedException("Unsupported tag code: " + code);
                if (code == 39 && (length != 6 ||
                    BinaryPrimitives.ReadUInt16LittleEndian(body.AsSpan(pos + 2)) != 0 ||
                    BinaryPrimitives.ReadUInt16LittleEndian(body.AsSpan(pos + 4)) != 0))
                    throw new NotSupportedException("Only empty sprites are accepted");
                pos += (int)length;
                if (code == 0)
                {
                    if (length != 0 || pos != body.Length) throw new InvalidDataException("Invalid End tag");
                    ended = true; break;
                }
            }
            if (!ended) throw new InvalidDataException("Missing End tag");
        }
    }
}
