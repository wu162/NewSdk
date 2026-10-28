using System;

namespace BinaryAssetBuilder.TextureCompiler
{
    // Complete 4x4 PC Production=71 DXT3/DXT5 and transparent-block DXT1A. No DDS/image/plugin routing.
    public static class TextureDxtBlockEncoder
    {
        internal static readonly object ColourLock = new object();
        public static byte[] EncodeBlock(byte[] rgba, string format)
        {
            if (rgba == null) throw new ArgumentNullException("rgba");
            if (rgba.Length != 64) throw new ArgumentException("A 4x4 RGBA block is required.", "rgba");
            if (format != "D3DFMT_DXT1A" && format != "D3DFMT_DXT3" && format != "D3DFMT_DXT5") throw new NotSupportedException("Only PC Production DXT3/DXT5 or transparent-block DXT1A is accepted.");
            float[] samples=new float[rgba.Length];
            for(int i=0;i<samples.Length;i++)samples[i]=(float)(rgba[i]/255.0);
            return TextureDxtSampleBlockEncoder.EncodeFloatBlock(samples,4,4,format);
        }

    }
}
