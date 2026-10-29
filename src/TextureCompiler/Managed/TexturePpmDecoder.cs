using System;
using System.IO;
using System.Runtime.InteropServices;
using BinaryAssetBuilder;
using Recovered.Compiler.Texture;

namespace Recovered.Compiler.Texture
{
    // Pinned, adapted IJG6b P3/P6 reader with PC packing and NPOT semantics.
    public static class TexturePpmDecoder
    {
        [DllImport("Recovered.TexturePpm.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int TextureDecodePpm([In] byte[] input, int count,
            [Out] byte[] output, int capacity, out int width, out int height);
        public static bool IsPpm(byte[] source)
        {
            return source != null && source.Length >= 2 && source[0] == 'P' &&
                (source[1] == '2' || source[1] == '3' || source[1] == '5' || source[1] == '6');
        }
        private static Exception Bad()
        {
            return new BinaryAssetBuilderException(ErrorCode.BadTextureFile,
                "D3DX cannot load the texture from disk.", new object[0]);
        }
        public static TexturePacker.Image Load(string path) { return Decode(File.ReadAllBytes(path)); }
        public static TexturePacker.Image Decode(byte[] source)
        {
            if (source == null) throw new ArgumentNullException("source");
            int width, height;
            if (TextureDecodePpm(source, source.Length, null, 0, out width, out height) == 0) throw Bad();
            byte[] rgba = new byte[checked(width * height * 4)];
            if (TextureDecodePpm(source, source.Length, rgba, rgba.Length, out width, out height) == 0) throw Bad();
            uint[] pixels = new uint[checked(width * height)];
            Buffer.BlockCopy(rgba, 0, pixels, 0, rgba.Length);
            var image = new TexturePacker.Image(width, height, pixels, false);
            return (width & (width - 1)) == 0 && (height & (height - 1)) == 0
                ? image : TextureNpotRgba8Resampler.Resize(image);
        }
    }
}
