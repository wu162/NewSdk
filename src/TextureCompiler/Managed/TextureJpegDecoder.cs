using System;
using System.Runtime.InteropServices;
using BinaryAssetBuilder;

namespace Recovered.Compiler.Texture
{
    // Pinned IJG6b IFAST/fancy decoding. PC grayscale retains the native L8
    // destination store during NPOT filtering; RGB uses the RGBA8 store.
    public static class TextureJpegDecoder
    {
        [DllImport("Recovered.TextureJpeg.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int TextureDecodeJpeg([In] byte[] input, int count,
            [Out] byte[] output, int capacity, out int width, out int height, out int grayscale);
        public static bool IsJpeg(byte[] source)
        {
            return source != null && source.Length >= 2 && source[0] == 255 && source[1] == 216;
        }
        private static Exception Bad()
        {
            return new BinaryAssetBuilderException(ErrorCode.BadTextureFile,
                "D3DX cannot load the texture from disk.", new object[0]);
        }
        public static TexturePacker.Image Decode(byte[] source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (!IsJpeg(source)) throw Bad();
            int width, height, gray;
            if (TextureDecodeJpeg(source, source.Length, null, 0, out width, out height, out gray) == 0) throw Bad();
            byte[] rgba = new byte[checked(width * height * 4)];
            if (TextureDecodeJpeg(source, source.Length, rgba, rgba.Length, out width, out height, out gray) == 0) throw Bad();
            uint[] pixels = new uint[checked(width * height)];
            Buffer.BlockCopy(rgba, 0, pixels, 0, rgba.Length);
            var image = new TexturePacker.Image(width, height, pixels, false);
            if ((width & (width - 1)) == 0 && (height & (height - 1)) == 0) return image;
            return gray != 0 ? TextureNpotStorageResampler.Resize(image, TextureNpotStorageResampler.Format.L8)
                : TextureNpotRgba8Resampler.Resize(image);
        }
    }
}
