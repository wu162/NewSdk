// Managed Flash 8 zlib backend. SWF 13+ LZMA is outside this plugin.
using System;
using System.IO;
using System.IO.Compression;
using BinaryAssetBuilder.UiCompiler.Swf;
namespace SwfLib
{
    public static class SwfZip
    {
        public static byte[] CompressZlib(byte[] data) => SwfWriter.compress(data);
        public static byte[] DecompressZlib(byte[] bytes)
        {
            if (bytes.Length < 6 || (bytes[0] & 15) != 8 || ((bytes[0] << 8) + bytes[1]) % 31 != 0 || (bytes[1] & 32) != 0) throw new InvalidDataException("Invalid zlib header.");
            using (var input = new MemoryStream(bytes, 2, bytes.Length - 6)) using (var z = new DeflateStream(input, CompressionMode.Decompress)) using (var output = new MemoryStream())
            {
                var buffer = new byte[8192]; int count;
                while ((count = z.Read(buffer, 0, buffer.Length)) != 0) { if (output.Length + count > 256 * 1024 * 1024) throw new InvalidDataException("Inflated SWF data exceeds 256 MiB."); output.Write(buffer, 0, count); }
                byte[] data = output.ToArray(); uint a = 1, b = 0;
                foreach (byte v in data) { a = (a + v) % 65521; b = (b + a) % 65521; }
                uint actual = b << 16 | a; int p = bytes.Length - 4; uint expected = (uint)bytes[p] << 24 | (uint)bytes[p + 1] << 16 | (uint)bytes[p + 2] << 8 | bytes[p + 3];
                if (actual != expected) throw new InvalidDataException("Zlib checksum mismatch."); return data;
            }
        }
        public static void Compress(Stream input, Stream output, SwfFormat format) { if (format != SwfFormat.CWS) throw new NotSupportedException("Flash 8 only supports zlib."); using (var m = new MemoryStream()) { input.CopyTo(m); byte[] data = CompressZlib(m.ToArray()); output.Write(data, 0, data.Length); } }
        public static void Decompress(Stream input, Stream output, SwfFormat format) { if (format == SwfFormat.FWS) { input.CopyTo(output); return; } if (format != SwfFormat.CWS) throw new NotSupportedException("LZMA requires SWF 13 or later."); using (var m = new MemoryStream()) { input.CopyTo(m); byte[] data = DecompressZlib(m.ToArray()); output.Write(data, 0, data.Length); } }
        public static byte[] Decompress(byte[] input, SwfFormat format) { using (var output = new MemoryStream()) { Decompress(new MemoryStream(input), output, format); return output.ToArray(); } }
    }
}
