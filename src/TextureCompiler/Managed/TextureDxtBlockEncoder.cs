using System;
using System.Runtime.InteropServices;

namespace BinaryAssetBuilder.TextureCompiler
{
    // Complete 4x4 PC Production=71 DXT3/DXT5 blocks. No DDS/image/plugin routing.
    public static class TextureDxtBlockEncoder
    {
        internal static readonly object ColourLock = new object();
        [DllImport("Recovered.TextureDxtColour.dll", CallingConvention = CallingConvention.StdCall,
            EntryPoint = "TextureColorState")]
        private static extern int EncodeColourState([In, Out] byte[] block, int bytes, int mode);

        public static byte[] EncodeBlock(byte[] rgba, string format)
        {
            if (rgba == null) throw new ArgumentNullException("rgba");
            if (rgba.Length != 64) throw new ArgumentException("A 4x4 RGBA block is required.", "rgba");
            int nativeFormat;
            if (format == "D3DFMT_DXT3") nativeFormat = 2;
            else if (format == "D3DFMT_DXT5") nativeFormat = 3;
            else throw new NotSupportedException("Only PC Production DXT3/DXT5 blocks are accepted.");
            byte[] state = new byte[0x3850];
            PutInt(state, 0, nativeFormat);
            PutInt(state, 4, 1); // The original forces four-colour mode for DXT3/5.
            PutInt(state, 8, 16);
            for (int i = 0; i < 16; i++) for (int c = 0; c < 3; c++)
                PutDouble(state, 0x10 + (i * 3 + c) * 8, (double)(float)(rgba[i * 4 + c] / 255.0));
            PutDouble(state, 0x3010, 16);
            PutDouble(state, 0x3018, (double)0.3086f);
            PutDouble(state, 0x3020, (double)0.6094f);
            PutDouble(state, 0x3028, (double)0.082f);
            PutInt(state, 0x3030, 71);
            // AllSame lazily initializes its owned lookup, as in the native library.
            lock (ColourLock)
            {
                int status = EncodeColourState(state, state.Length, 0);
                if (status != 0) throw new InvalidOperationException("Colour state rejected: " + status);
            }
            int levels = BitConverter.ToInt32(state, 0x3034);
            if (levels != 4) throw new InvalidOperationException("DXT3/5 requires four colour levels.");
            ushort first = Pack565(state, 0x3038), second = Pack565(state, 0x3044);
            int[] indices = new int[16];
            for (int i = 0; i < 16; i++) indices[i] = BitConverter.ToInt32(state, 0x3050 + i * 4);
            if (first == second)
            {
                first = unchecked((ushort)(first + 1));
                for (int i = 0; i < 16; i++) indices[i] = 1;
            }
            bool swap = (second < first) == (levels == 3);
            if (swap) { ushort temp = first; first = second; second = temp; }
            byte[] result = new byte[16];
            byte[] alpha = nativeFormat == 2 ? TextureDxt3AlphaEncoder.EncodeBlock(rgba) : TextureDxt5AlphaEncoder.EncodeBlock(rgba);
            Buffer.BlockCopy(alpha, 0, result, 0, 8);
            result[8] = (byte)first; result[9] = (byte)(first >> 8);
            result[10] = (byte)second; result[11] = (byte)(second >> 8);
            uint bits = 0;
            for (int i = 0; i < 16; i++)
            {
                int index = indices[i];
                if (index < 0 || index > 3) throw new InvalidOperationException("Invalid colour index.");
                if (swap) index ^= 1;
                bits |= (uint)index << (i * 2);
            }
            for (int i = 0; i < 4; i++) result[12 + i] = (byte)(bits >> (8 * i));
            return result;
        }
        private static ushort Pack565(byte[] state, int offset)
        {
            int r = BitConverter.ToInt32(state, offset), g = BitConverter.ToInt32(state, offset + 4), b = BitConverter.ToInt32(state, offset + 8);
            if (r < 0 || r > 31 || g < 0 || g > 63 || b < 0 || b > 31) throw new InvalidOperationException("Invalid 565 endpoint.");
            return (ushort)((r << 11) | (g << 5) | b);
        }
        private static void PutInt(byte[] target, int offset, int value) { Buffer.BlockCopy(BitConverter.GetBytes(value), 0, target, offset, 4); }
        private static void PutDouble(byte[] target, int offset, double value) { Buffer.BlockCopy(BitConverter.GetBytes(value), 0, target, offset, 8); }
    }
}
