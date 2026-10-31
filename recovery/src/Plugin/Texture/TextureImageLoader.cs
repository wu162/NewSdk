using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;

namespace Recovered.Compiler.Texture
{
    public static class TextureImageLoader
    {
        public static TexturePacker.Image Load(string path) { return Decode(File.ReadAllBytes(path)); }
        public static TexturePacker.Image Decode(byte[] data)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (data.Length == 0) throw new InvalidDataException("Image file is empty.");
            if (data.Length > 2 && data[0] == 'P' && (data[1] == '3' || data[1] == '6')) return DecodePpm(data);
            try
            {
                if (TexturePngDecoder.IsPng(data)) return TexturePngDecoder.Decode(data);
                if (TextureBmpDecoder.IsBmp(data)) return TextureBmpDecoder.Decode(data);
                if (TextureDibDecoder.IsDib(data)) return TextureDibDecoder.Decode(data);
            }
            catch (NotSupportedException)
            {
                // The recovered byte-compatible subsets are preferred. GDI+
                // handles other standard PNG/BMP variants and common JPEG/GIF/TIFF.
                return DecodeDrawing(data);
            }
            if (IsDrawingFormat(data)) return DecodeDrawing(data);
            return TextureTgaAdditionalFormats.CanDecode(data) ? TextureTgaAdditionalFormats.Decode(data) : TextureTgaDecoder.Decode(data);
        }
        private static bool IsDrawingFormat(byte[] data)
        {
            return data.Length >= 2 && data[0] == 0xff && data[1] == 0xd8 ||
                data.Length >= 3 && data[0] == 'G' && data[1] == 'I' && data[2] == 'F' ||
                data.Length >= 4 && (data[0] == 'I' && data[1] == 'I' && data[2] == 42 && data[3] == 0 ||
                    data[0] == 'M' && data[1] == 'M' && data[2] == 0 && data[3] == 42);
        }
        private static TexturePacker.Image DecodeDrawing(byte[] data)
        {
            try
            {
                using (var stream = new MemoryStream(data, false))
                using (var source = System.Drawing.Image.FromStream(stream, false, true))
                {
                    int width = source.Width, height = source.Height;
                    if ((long)width * height >= 16777216L) throw new InvalidDataException("Image exceeds the 64 MiB RGBA limit.");
                    bool alpha = System.Drawing.Image.IsAlphaPixelFormat(source.PixelFormat) ||
                        (source.Flags & (int)ImageFlags.HasAlpha) != 0 || (source.Flags & (int)ImageFlags.HasTranslucent) != 0;
                    using (var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
                    {
                        using (var graphics = Graphics.FromImage(bitmap))
                        {
                            graphics.CompositingMode = CompositingMode.SourceCopy;
                            graphics.DrawImageUnscaled(source, 0, 0);
                        }
                        var locked = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                        uint[] pixels = new uint[checked(width * height)];
                        try
                        {
                            byte[] row = new byte[checked(width * 4)];
                            for (int y = 0; y < height; y++)
                            {
                                Marshal.Copy(IntPtr.Add(locked.Scan0, y * locked.Stride), row, 0, row.Length);
                                for (int x = 0; x < width; x++)
                                {
                                    int at = x * 4;
                                    pixels[y * width + x] = (uint)(row[at + 2] | row[at + 1] << 8 | row[at] << 16 | row[at + 3] << 24);
                                }
                            }
                        }
                        finally { bitmap.UnlockBits(locked); }
                        return ResizeIfNeeded(new TexturePacker.Image(width, height, pixels, alpha));
                    }
                }
            }
            catch (ArgumentException failure) { throw new InvalidDataException("Cannot decode the image file.", failure); }
            catch (ExternalException failure) { throw new InvalidDataException("Cannot decode the image file.", failure); }
        }
        private static TexturePacker.Image ResizeIfNeeded(TexturePacker.Image image)
        {
            return (image.Width & (image.Width - 1)) != 0 || (image.Height & (image.Height - 1)) != 0
                ? TextureNpotRgba8Resampler.Resize(image) : image;
        }
        private static TexturePacker.Image DecodePpm(byte[] data)
        {
            int at = 0; string magic = PpmToken(data, ref at);
            int width = PpmNumber(data, ref at), height = PpmNumber(data, ref at), maximum = PpmNumber(data, ref at);
            if (width <= 0 || height <= 0 || (long)width * height >= 16777216L || maximum < 1 || maximum > 65535)
                throw new InvalidDataException("Invalid PPM dimensions or sample range.");
            uint[] pixels = new uint[checked(width * height)]; bool binary = magic == "P6";
            if (binary)
            {
                if (at >= data.Length || !PpmSpace(data[at])) throw new InvalidDataException("Missing PPM raster separator.");
                if (data[at++] == 13 && at < data.Length && data[at] == 10) at++;
            }
            for (int i = 0; i < pixels.Length; i++)
            {
                uint pixel = 0xff000000u;
                for (int channel = 0; channel < 3; channel++)
                {
                    int sample;
                    if (!binary) sample = PpmNumber(data, ref at);
                    else
                    {
                        int count = maximum > 255 ? 2 : 1;
                        if (at > data.Length - count) throw new InvalidDataException("Truncated PPM raster.");
                        sample = data[at++]; if (count == 2) sample = sample << 8 | data[at++];
                    }
                    if (sample > maximum) throw new InvalidDataException("PPM sample exceeds the declared range.");
                    pixel |= (uint)((sample * 255 + maximum / 2) / maximum) << (channel * 8);
                }
                pixels[i] = pixel;
            }
            return ResizeIfNeeded(new TexturePacker.Image(width, height, pixels, false));
        }
        private static int PpmNumber(byte[] data, ref int at)
        {
            int value; if (!int.TryParse(PpmToken(data, ref at), out value) || value < 0) throw new InvalidDataException("Invalid PPM numeric field.");
            return value;
        }
        private static bool PpmSpace(byte value) { return value == 32 || value >= 9 && value <= 13; }
        private static string PpmToken(byte[] data, ref int at)
        {
            while (at < data.Length)
            {
                if (PpmSpace(data[at])) { at++; continue; }
                if (data[at] != '#') break;
                while (at < data.Length && data[at] != 10 && data[at] != 13) at++;
            }
            int start = at;
            while (at < data.Length && !PpmSpace(data[at]) && data[at] != '#') at++;
            if (start == at) throw new InvalidDataException("Missing PPM header/sample field.");
            return System.Text.Encoding.ASCII.GetString(data, start, at - start);
        }
    }
}
