// Managed replacement for the original jpeg.dll and disk-backed TGA bridge.
// Serialization derived from lanyizi/swf2apt, GPL-3.0.
using System;
using System.IO;
using SwfLib.Tags.BitmapTags;
using StbImageSharp;
namespace eaf2apt.Characters
{
    class AptCharacterBitmap : AptCharacter
    {
        public MemoryTexture Texture;
        public bool NoPack;
        public FloatVector PackOffset;
        public override void OutputCharacter(int i) { base.OutputCharacter(i); GlobalData.output.Write(3, (int)AptCharacterType.Bitmap); GlobalData.output.Write(3, 0x09876543); GlobalData.output.Write(3, ID); }
        // The upstream gradient generators supply bottom-up BGRA pixels.
        public AptCharacterBitmap(byte[] pixels, ushort width, ushort height, Globals globals, int id) : base(globals, id)
        { var output = new byte[pixels.Length]; for (int y = 0; y < height; y++) Buffer.BlockCopy(pixels, (height - y - 1) * width * 4, output, y * width * 4, width * 4); Texture = new MemoryTexture(width, height, output); }
        public AptCharacterBitmap(DefineBitsLossless2Tag tag, Globals globals) : base(globals, tag.CharacterID) => Lossless(tag.BitmapFormat, tag.BitmapWidth, tag.BitmapHeight, tag.BitmapColorTableSize, tag.ZlibBitmapData, true);
        public AptCharacterBitmap(DefineBitsLosslessTag tag, Globals globals) : base(globals, tag.CharacterID) => Lossless(tag.BitmapFormat, tag.BitmapWidth, tag.BitmapHeight, tag.BitmapColorTableSize, tag.ZlibBitmapData, false);
        private void Lossless(int format, int width, int height, int tableSize, byte[] compressed, bool alpha)
        {
            if (width <= 0 || height <= 0 || (long)width * height > 16777216) throw new InvalidDataException("Invalid SWF bitmap dimensions.");
            byte[] raw = SwfLib.SwfZip.DecompressZlib(compressed), output = new byte[checked(width * height * 4)]; int stride = format == 3 ? (width + 3) & ~3 : width * 4;
            int paletteBytes = format == 3 ? (tableSize + 1) * (alpha ? 4 : 3) : 0;
            if (format != 3 && format != 5 || raw.Length != paletteBytes + stride * height) throw new InvalidDataException("Unsupported SWF lossless bitmap layout.");
            for (int y = 0; y < height; y++) for (int x = 0; x < width; x++)
            {
                int from = paletteBytes + y * stride + (format == 3 ? x : x * 4), to = (y * width + x) * 4; int a, r, g, b;
                if (format == 3) { int index = raw[from]; if (index > tableSize) throw new InvalidDataException("SWF palette index."); from = index * (alpha ? 4 : 3); r = raw[from]; g = raw[from + 1]; b = raw[from + 2]; a = alpha ? raw[from + 3] : 255; }
                else { a = alpha ? raw[from] : 255; r = raw[from + 1]; g = raw[from + 2]; b = raw[from + 3]; }
                int aa = Math.Max(1, a); output[to] = (byte)Math.Min(255, b * 255 / aa); output[to + 1] = (byte)Math.Min(255, g * 255 / aa); output[to + 2] = (byte)Math.Min(255, r * 255 / aa); output[to + 3] = (byte)a;
            }
            Texture = new MemoryTexture(width, height, output);
        }
        public AptCharacterBitmap(DefineBitsJPEG2Tag tag, Globals globals) : base(globals, tag.CharacterID) => Jpeg(tag.ImageData, null);
        public AptCharacterBitmap(DefineBitsJPEG3Tag tag, Globals globals) : base(globals, tag.CharacterID) => Jpeg(tag.ImageData, SwfLib.SwfZip.DecompressZlib(tag.BitmapAlphaData));
        public AptCharacterBitmap(DefineBitsTag tag, Globals globals) : base(globals, tag.CharacterID)
        { if (globals.JPEGTables == null) throw new InvalidDataException("Missing JPEG tables."); using (var m = new MemoryStream()) { m.Write(globals.JPEGTables, 0, globals.JPEGTables.Length - 2); m.Write(tag.JPEGData, 2, tag.JPEGData.Length - 2); Jpeg(m.ToArray(), null); } }
        private void Jpeg(byte[] bytes, byte[] alpha)
        {
            if (bytes.Length > 4 && bytes[0] == 255 && bytes[1] == 217 && bytes[2] == 255 && bytes[3] == 216) { var fixedBytes = new byte[bytes.Length - 4]; Buffer.BlockCopy(bytes, 4, fixedBytes, 0, fixedBytes.Length); bytes = fixedBytes; }
            var image = ImageResult.FromMemory(bytes, ColorComponents.RedGreenBlueAlpha); if (alpha != null && alpha.Length != image.Width * image.Height) throw new InvalidDataException("JPEG alpha size mismatch.");
            byte[] bgra = image.Data; for (int i = 0; i < bgra.Length; i += 4) { byte red = bgra[i]; bgra[i] = bgra[i + 2]; bgra[i + 2] = red; if (alpha != null) bgra[i + 3] = alpha[i / 4]; } Texture = new MemoryTexture(image.Width, image.Height, bgra);
        }
    }
}
