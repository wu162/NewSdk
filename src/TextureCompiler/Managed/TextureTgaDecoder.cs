using System;
using System.IO;

namespace Recovered.Compiler.Texture
{
    // Narrow CPU decoder candidate, deliberately opt-in until its exact Windows
    // original-LoadImageA differential receipt is accepted. No original DLL calls.
    public static class TextureTgaDecoder
    {
        public static TexturePacker.Image Load(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            return Decode(File.ReadAllBytes(path));
        }
        public static TexturePacker.Image Decode(byte[] source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.Length < 18) throw new InvalidDataException("TGA header is truncated.");
            int idLength = source[0], colorMap = source[1], type = source[2];
            if (colorMap != 0 || (type != 2 && type != 10))
                throw new NotSupportedException("Only unmapped true-color raw/RLE TGA images are supported.");
            // Reject color-map declarations, nonzero coordinate metadata and
            // interleaved images instead of silently treating them as this subset.
            for (int i = 3; i < 12; i++)
                if (source[i] != 0) throw new NotSupportedException("TGA palette/origin metadata is outside the recovered subset.");
            int width = source[12] | source[13] << 8;
            int height = source[14] | source[15] << 8;
            int depth = source[16], descriptor = source[17], attributes = descriptor & 15;
            if (depth != 24 && depth != 32)
                throw new NotSupportedException("Only 24-bit and 32-bit true-color TGA images are supported.");
            if ((descriptor & 0xC0) != 0 || (depth == 24 && attributes != 0) ||
                (depth == 32 && attributes != 0 && attributes != 8))
                throw new NotSupportedException("TGA attribute/interleaving metadata is outside the recovered subset.");
            if (width == 0 || height == 0) throw new InvalidDataException("TGA dimensions must be positive.");
            long count64 = (long)width * height;
            if (count64 >= 16777216L)
                throw new NotSupportedException("TGA images at or above the original 64 MiB processing limit are outside this decoder subset.");
            int count = (int)count64, bytesPerPixel = depth / 8, offset = 18 + idLength;
            if (offset > source.Length) throw new InvalidDataException("TGA image ID is truncated.");
            uint[] pixels = new uint[count];
            int position = 0;
            while (position < count)
            {
                int packet = type == 10 ? ReadByte(source, ref offset) : 0;
                int length = type == 10 ? (packet & 127) + 1 : count - position;
                bool repeat = type == 10 && (packet & 128) != 0;
                if (length > count - position) throw new InvalidDataException("TGA RLE packet exceeds the declared image.");
                uint repeated = repeat ? ReadPixel(source, ref offset, bytesPerPixel) : 0;
                for (int i = 0; i < length; i++)
                {
                    uint pixel = repeat ? repeated : ReadPixel(source, ref offset, bytesPerPixel);
                    int fileY = position / width, fileX = position % width;
                    int x = (descriptor & 0x10) != 0 ? width - 1 - fileX : fileX;
                    int y = (descriptor & 0x20) != 0 ? fileY : height - 1 - fileY;
                    pixels[y * width + x] = pixel;
                    position++;
                }
            }
            if (offset != source.Length && !IsEmptyFooter(source, offset))
                throw new NotSupportedException("Only an exact TGA footer with zero extension/developer offsets is supported after the pixel data.");
            return new TexturePacker.Image(width, height, pixels, depth == 32);
        }
        private static bool IsEmptyFooter(byte[] source, int offset)
        {
            if (source.Length - offset != 26) return false;
            for (int i = 0; i < 8; i++) if (source[offset + i] != 0) return false;
            const string signature = "TRUEVISION-XFILE.\0";
            for (int i = 0; i < signature.Length; i++)
                if (source[offset + 8 + i] != (byte)signature[i]) return false;
            return true;
        }
        private static int ReadByte(byte[] source, ref int offset)
        {
            if (offset >= source.Length) throw new InvalidDataException("TGA pixel/RLE data is truncated.");
            return source[offset++];
        }
        private static uint ReadPixel(byte[] source, ref int offset, int bytesPerPixel)
        {
            uint blue = (uint)ReadByte(source, ref offset), green = (uint)ReadByte(source, ref offset);
            uint red = (uint)ReadByte(source, ref offset);
            uint alpha = bytesPerPixel == 4 ? (uint)ReadByte(source, ref offset) : 255u;
            return red | green << 8 | blue << 16 | alpha << 24;
        }
    }
}
