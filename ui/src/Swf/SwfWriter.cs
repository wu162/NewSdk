using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace BinaryAssetBuilder.UiCompiler.Swf
{
    internal sealed class SwfWriter
    {
        private readonly MemoryStream output = new MemoryStream();
        private int pending, count;
        internal void bits(long value, int width)
        {
            if (width < 0 || width > 32) throw new InvalidDataException("Invalid SWF bit width.");
            for (int i = width - 1; i >= 0; i--) { pending = pending << 1 | (int)((value >> i) & 1); if (++count == 8) { output.WriteByte((byte)pending); count = pending = 0; } }
        }
        internal void align() { if (count != 0) { output.WriteByte((byte)(pending << (8 - count))); count = pending = 0; } }
        internal void u8(int value) { align(); output.WriteByte((byte)value); }
        internal void u16(int value) { u8(value); u8(value >> 8); }
        internal void u32(long value) { u16((int)value); u16((int)(value >> 16)); }
        internal void bytes(byte[] value) { align(); output.Write(value, 0, value.Length); }
        internal void text(string value) { if (value.IndexOf('\0') >= 0) throw new InvalidDataException("SWF string contains NUL."); bytes(Encoding.UTF8.GetBytes(value)); u8(0); }
        internal byte[] data() { align(); return output.ToArray(); }
        internal void tag(int id, byte[] payload) { u16(id << 6 | Math.Min(payload.Length, 63)); if (payload.Length >= 63) u32(payload.Length); bytes(payload); }
        internal void tag(int id, SwfWriter payload) => tag(id, payload.data());
        internal void tag(int id) => tag(id, Array.Empty<byte>());
        internal static int indexBits(int value) { int width = 0; for (; value != 0; value = (int)((uint)value >> 1)) width++; return width; }
        internal static int signedBits(params int[] values) { int width = 1; foreach (int v in values) width = Math.Max(width, indexBits(v < 0 ? ~v : v) + 1); return width; }
        internal void rect(int left, int right, int top, int bottom) { int n = signedBits(left, right, top, bottom); if (n > 31) throw new InvalidDataException("SWF rectangle overflow."); bits(n, 5); bits(left, n); bits(right, n); bits(top, n); bits(bottom, n); align(); }
        internal void matrix(double[] m)
        {
            int a = Fixed(m[0]), b = Fixed(m[1]), c = Fixed(m[2]), d = Fixed(m[3]);
            bool scale = a != 65536 || d != 65536, skew = b != 0 || c != 0; bits(scale ? 1 : 0, 1);
            void Pair(int x, int y) { int n = signedBits(x, y); if (n > 31) throw new InvalidDataException("SWF matrix overflow."); bits(n, 5); bits(x, n); bits(y, n); }
            if (scale) Pair(a, d); bits(skew ? 1 : 0, 1); if (skew) Pair(b, c); Pair(Round(m[4]), Round(m[5])); align();
        }
        internal static int Round(double value) => checked((int)Math.Floor(value + 0.5));
        private static int Fixed(double value) => checked((int)Math.Round(value * 65536));
        internal static byte[] compress(byte[] bytes)
        {
            using (var result = new MemoryStream())
            {
                result.WriteByte(0x78); result.WriteByte(0x9c);
                using (var deflate = new DeflateStream(result, CompressionLevel.Optimal, true)) deflate.Write(bytes, 0, bytes.Length);
                uint a = 1, b = 0; foreach (byte v in bytes) { a = (a + v) % 65521; b = (b + a) % 65521; }
                uint checksum = b << 16 | a; for (int i = 3; i >= 0; i--) result.WriteByte((byte)(checksum >> (i * 8))); return result.ToArray();
            }
        }
        internal static byte[] movie(byte[] body, bool compressed)
        { var writer = new SwfWriter(); writer.bytes(Encoding.ASCII.GetBytes(compressed ? "CWS" : "FWS")); writer.u8(8); writer.u32(body.Length + 8L); writer.bytes(compressed ? compress(body) : body); return writer.data(); }
    }
}
