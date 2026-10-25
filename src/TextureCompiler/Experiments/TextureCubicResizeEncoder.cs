using System;
using System.Collections.Generic;

namespace Recovered.Compiler.Texture
{
    // EXPERIMENTAL: not included in the production project or plugin route.
    // Cubic base pixels match probes; the post-resize mip boundary is unresolved.
    public static class TextureCubicResizeEncoder
    {
        private sealed class Contribution { public int First; public double[] Weights; }
        private static float[] Resize(float[] original, int sw, int sh, int tw, int th, bool cubic)
        {
            return (long)tw * sh < (long)sw * th
                ? Vertical(Horizontal(original, sw, sh, tw, cubic), tw, sh, th, cubic)
                : Horizontal(Vertical(original, sw, sh, th, cubic), sw, th, tw, cubic);
        }
        public static byte[] Encode(uint width, uint height, uint targetWidth, uint targetHeight,
            byte[] rgba, string format, bool mipmaps)
        {
            // Reuse the original safe input, extent, format and storage contract.
            TextureRawDdsEncoder.Encode(width, height, rgba, format, false);
            if (targetWidth == 0 || targetHeight == 0 ||
                (targetWidth & (targetWidth - 1)) != 0 || (targetHeight & (targetHeight - 1)) != 0)
                throw new ArgumentException("Cubic target dimensions must be positive powers of two.");
            if (targetWidth > width || targetHeight > height ||
                (targetWidth == width && width != 1) || (targetHeight == height && height != 1))
                throw new NotSupportedException("Only strict cubic downscaling (or a unit axis) has been recovered.");
            float[] source = new float[rgba.Length];
            for (int i = 0; i < rgba.Length; i++) source[i] = (float)((double)rgba[i] / 255.0);
            int tw = checked((int)targetWidth), th = checked((int)targetHeight);
            float[] resized = Resize(source, checked((int)width), checked((int)height), tw, th, true);
            byte[] header = TextureRawDdsEncoder.Encode(targetWidth, targetHeight, new byte[checked(tw * th * 4)], format, false);
            int stride = format == "D3DFMT_R5G6B5" ? 2 : 4;
            int levels = mipmaps ? 3 : 1;
            long length = header.Length;
            int w = tw, h = th;
            for (int level = 1; level < levels; level++) { w = Math.Max(1, w / 2); h = Math.Max(1, h / 2); length = checked(length + (long)w * h * stride); }
            byte[] output = new byte[checked((int)length)];
            Buffer.BlockCopy(header, 0, output, 0, 128);
            if (mipmaps) { Put(output, 8, 0x000a1007u); Put(output, 28, Math.Min(TextureVolume.MipCount(targetWidth, targetHeight, 1), 3u)); Put(output, 108, 0x00401008u); }
            int offset = WriteLevel(output, 128, resized, format); w = tw; h = th;
            for (int level = 1; level < levels; level++)
            {
                w = Math.Max(1, w / 2); h = Math.Max(1, h / 2);
                offset = WriteLevel(output, offset, Resize(resized, tw, th, w, h, false), format);
            }
            if (offset != output.Length) throw new InvalidOperationException("Cubic DDS output size mismatch.");
            return output;
        }
        private static Contribution[] Contributions(int source, int target, bool cubic)
        {
            double scale = (double)source / target;
            Contribution[] result = new Contribution[target];
            for (int output = 0; output < target; output++)
            {
                double center = (output + 0.5) * scale - 0.5;
                int first = Math.Max(0, (int)Math.Ceiling(center - (cubic ? 2 * scale : scale)));
                int last = Math.Min(source - 1, (int)Math.Floor(center + (cubic ? 2 * scale : scale)));
                List<double> weights = new List<double>();
                double sum = 0;
                for (int input = first; input <= last; input++)
                {
                    double x = Math.Abs(input - center) / scale;
                    double value = cubic ? (x < 1.0 ? (4.0 + x * x * (-6.0 + 3.0 * x)) / 6.0 : x < 2.0 ? ((2.0 - x) * (2.0 - x) * (2.0 - x)) / 6.0 : 0.0) : 1.0 - x;
                    // Keep zero end taps in contiguous storage. Zero products
                    // do not affect non-negative finite pixel accumulations.
                    weights.Add(value); sum += value;
                }
                double inverse = 1.0 / sum;
                double[] normalized = weights.ToArray();
                // Multiply by one stored reciprocal. Per-item division differs
                // at an observed float32 half-value boundary in the old library.
                for (int i = 0; i < normalized.Length; i++) normalized[i] = normalized[i] * inverse;
                result[output] = new Contribution { First = first, Weights = normalized };
            }
            return result;
        }
        private static float[] Horizontal(float[] source, int width, int height, int targetWidth, bool cubic)
        {
            Contribution[] contributions = Contributions(width, targetWidth, cubic);
            float[] output = new float[checked(targetWidth * height * 4)];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < targetWidth; x++)
                {
                    Contribution contribution = contributions[x];
                    int destination = (y * targetWidth + x) * 4;
                    for (int tap = 0; tap < contribution.Weights.Length; tap++)
                    {
                        int input = (y * width + contribution.First + tap) * 4;
                        double weight = contribution.Weights[tap];
                        for (int channel = 0; channel < 4; channel++)
                            // Array store forces each accumulated tap to float32;
                            // rounding the product before addition is incorrect.
                            output[destination + channel] = (float)((double)output[destination + channel] + (double)source[input + channel] * weight);
                    }
                }
            return output;
        }
        private static float[] Vertical(float[] source, int width, int height, int targetHeight, bool cubic)
        {
            Contribution[] contributions = Contributions(height, targetHeight, cubic);
            float[] output = new float[checked(width * targetHeight * 4)];
            for (int y = 0; y < targetHeight; y++)
                for (int x = 0; x < width; x++)
                {
                    Contribution contribution = contributions[y];
                    int destination = (y * width + x) * 4;
                    for (int tap = 0; tap < contribution.Weights.Length; tap++)
                    {
                        int input = ((contribution.First + tap) * width + x) * 4;
                        double weight = contribution.Weights[tap];
                        for (int channel = 0; channel < 4; channel++)
                            output[destination + channel] = (float)((double)output[destination + channel] + (double)source[input + channel] * weight);
                    }
                }
            return output;
        }
        private static int Quantize(float value, int maximum)
        {
            int result = (int)((double)value * maximum + 0.5);
            if (result < 0) return 0;
            if (result > maximum) return maximum;
            return result;
        }
        private static int WriteLevel(byte[] output, int offset, float[] rgba, string format)
        {
            bool rgb565 = format == "D3DFMT_R5G6B5", alpha = format == "D3DFMT_A8R8G8B8";
            for (int input = 0; input < rgba.Length; input += 4)
            {
                if (rgb565)
                {
                    int packed = (Quantize(rgba[input], 31) << 11) | (Quantize(rgba[input + 1], 63) << 5) | Quantize(rgba[input + 2], 31);
                    output[offset++] = (byte)packed; output[offset++] = (byte)(packed >> 8);
                }
                else
                {
                    output[offset++] = (byte)Quantize(rgba[input + 2], 255);
                    output[offset++] = (byte)Quantize(rgba[input + 1], 255);
                    output[offset++] = (byte)Quantize(rgba[input], 255);
                    output[offset++] = alpha ? (byte)Quantize(rgba[input + 3], 255) : (byte)0;
                }
            }
            return offset;
        }
        private static void Put(byte[] data, int offset, uint value)
        {
            data[offset] = (byte)value; data[offset + 1] = (byte)(value >> 8);
            data[offset + 2] = (byte)(value >> 16); data[offset + 3] = (byte)(value >> 24);
        }
    }
}
