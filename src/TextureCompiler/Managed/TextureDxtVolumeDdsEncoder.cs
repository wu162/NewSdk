using System;
using Recovered.Compiler.Texture;

namespace BinaryAssetBuilder.TextureCompiler
{
    // PC DXT3/5 volume DDS, tightly packed RGBA slices in z/y/x order.
    // Header/slice layout: original probe 6d065a9e / 40525480986.
    // Header mip count covers XY; payload includes the full XYZ tail.
    public static class TextureDxtVolumeDdsEncoder
    {
        public static byte[] Encode(uint width, uint height, uint depth, byte[] rgba, string format, bool generateMipMaps)
        {
            if (rgba == null) throw new ArgumentNullException("rgba");
            if (width == 0 || height == 0 || depth == 0 ||
                (width & (width - 1)) != 0 || (height & (height - 1)) != 0 || (depth & (depth - 1)) != 0)
                throw new ArgumentException("Compressed volume DDS requires positive power-of-two dimensions.");
            ulong slicePixels = (ulong)width * height;
            if (slicePixels >= 16777216ul || depth >= 16777216ul || slicePixels * depth >= 16777216ul)
                throw new ArgumentOutOfRangeException("width", "Volume exceeds the original safe RGBA range.");
            if ((ulong)rgba.LongLength != slicePixels * depth * 4)
                throw new ArgumentException("RGBA storage must exactly match volume dimensions.", "rgba");
            if (format != "D3DFMT_DXT3" && format != "D3DFMT_DXT5")
                throw new NotSupportedException("Only PC Production DXT3/DXT5 volumes are accepted.");
            int cw = (int)width, ch = (int)height, cd = (int)depth;
            long total = 128;
            do
            {
                total = checked(total + (long)((cw + 3) / 4) * ((ch + 3) / 4) * cd * 16);
                if (!generateMipMaps || (cw == 1 && ch == 1 && cd == 1)) break;
                cw = Math.Max(1, cw / 2); ch = Math.Max(1, ch / 2); cd = Math.Max(1, cd / 2);
            } while (true);
            byte[] output = new byte[checked((int)total)];
            Put(output, 0, 0x20534444); Put(output, 4, 124);
            Put(output, 8, generateMipMaps ? 0x8a1007u : 0x881007u);
            Put(output, 12, height); Put(output, 16, width);
            Put(output, 20, checked(((width + 3) / 4) * ((height + 3) / 4) * 16));
            Put(output, 24, depth);
            if (generateMipMaps) Put(output, 28, TextureVolume.MipCount(width, height, 1));
            Put(output, 76, 32); Put(output, 80, 4);
            Put(output, 84, format == "D3DFMT_DXT3" ? 0x33545844u : 0x35545844u);
            Put(output, 108, generateMipMaps ? 0x401008u : 0x1000u); Put(output, 112, 0x200000u);
            float[] current = new float[rgba.Length];
            for (int i = 0; i < rgba.Length; i++) current[i] = (float)((double)rgba[i] / 255.0);
            cw = (int)width; ch = (int)height; cd = (int)depth;
            int offset = 128;
            do
            {
                int bw = Math.Min(4, cw), bh = Math.Min(4, ch);
                float[] block = new float[bw * bh * 4];
                for (int z = 0; z < cd; z++) for (int y = 0; y < ch; y += 4) for (int x = 0; x < cw; x += 4)
                {
                    for (int row = 0; row < bh; row++)
                        Array.Copy(current, ((z * ch + y + row) * cw + x) * 4, block, row * bw * 4, bw * 4);
                    byte[] encoded = TextureDxtSampleBlockEncoder.EncodeFloatBlock(block, bw, bh, format);
                    Buffer.BlockCopy(encoded, 0, output, offset, 16); offset += 16;
                }
                if (!generateMipMaps || (cw == 1 && ch == 1 && cd == 1)) break;
                current = TextureVolumeMipEncoder.NextLevel(current, cw, ch, cd);
                cw = Math.Max(1, cw / 2); ch = Math.Max(1, ch / 2); cd = Math.Max(1, cd / 2);
            } while (true);
            if (offset != output.Length) throw new InvalidOperationException("Compressed volume output size mismatch.");
            return output;
        }
        public static byte[] EncodeStrip(uint width, uint height, byte[] rgba, string format, bool generateMipMaps)
        {
            if (rgba == null) throw new ArgumentNullException("rgba");
            if (width == 0 || height == 0 || (width & (width - 1)) != 0 || (height & (height - 1)) != 0)
                throw new ArgumentException("Compressed volume strips require positive power-of-two dimensions.");
            ulong count = (ulong)width * height;
            if (count >= 16777216ul) throw new ArgumentOutOfRangeException("width", "Volume exceeds the original safe RGBA range.");
            if ((ulong)rgba.LongLength != count * 4) throw new ArgumentException("RGBA storage must exactly match strip dimensions.", "rgba");
            uint[] pixels = new uint[rgba.Length / 4]; Buffer.BlockCopy(rgba, 0, pixels, 0, rgba.Length);
            TextureVolume volume = new TextureVolume(); volume.CreateVolumeRGBAImage(width, height, pixels);
            byte[] ordered = new byte[rgba.Length]; int offset = 0;
            foreach (TextureVolume.Slice slice in volume.Levels[0].Slices)
            {
                int bytes = checked((int)slice.PixelCount * 4);
                Buffer.BlockCopy(slice.Pixels, 0, ordered, offset, bytes); offset += bytes;
            }
            return Encode(height, height, checked(width / height), ordered, format, generateMipMaps);
        }
        private static void Put(byte[] output, int offset, uint value)
        {
            output[offset] = (byte)value; output[offset + 1] = (byte)(value >> 8);
            output[offset + 2] = (byte)(value >> 16); output[offset + 3] = (byte)(value >> 24);
        }
    }
}
