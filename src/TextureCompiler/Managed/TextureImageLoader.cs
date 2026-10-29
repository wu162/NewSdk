using System.IO;
namespace Recovered.Compiler.Texture
{
    // Dispatches the independently validated TGA format groups without changing
    // either decoder's byte/alpha rules or using the original compiler.
    public static class TextureImageLoader
    {
        public static TexturePacker.Image Load(string path) { return Decode(File.ReadAllBytes(path)); }
        public static TexturePacker.Image Decode(byte[] data)
        {
            if(TextureJpegDecoder.IsJpeg(data))return TextureJpegDecoder.Decode(data);
            if(TexturePngDecoder.IsPng(data))return TexturePngDecoder.Decode(data);
            if(TextureBmpDecoder.IsBmp(data))return TextureBmpDecoder.Decode(data);
            if(TextureDibDecoder.IsDib(data))return TextureDibDecoder.Decode(data);
            return TextureTgaAdditionalFormats.CanDecode(data) ? TextureTgaAdditionalFormats.Decode(data) : TextureTgaDecoder.Decode(data);
        }
    }
}

