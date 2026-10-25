using System;
using System.IO;
using System.Text;

// Called by the process-isolated AudioPluginOracle in original and recovered
// hosts. Every case uses the full plugin route and compares all three buffers
// plus the complete custom data, rather than merely accepting a parsed header.
internal static class AudioInputFormatCases
{
    internal static void Run(string root, Action<string, int, string, bool> run)
    {
        foreach (bool aiff in new bool[] { false, true })
        foreach (int bits in (aiff ? new int[] { 8, 16, 24 } : new int[] { 24 }))
        foreach (int channels in new int[] { 1, 2 })
        foreach (int frames in new int[] { 7, 129, 2041 })
        {
            string name = (aiff ? "aiff" : "wave") + bits + "-" + channels + "-" + frames;
            string path = Path.Combine(root, name + (aiff ? ".aiff" : ".wav"));
            Write(path, aiff, bits, channels, frames);
            foreach (string codec in new string[] { "NONE", "XAS" })
            foreach (bool streamed in new bool[] { false, true })
            foreach (int platform in new int[] { 0, 1, 2 })
            {
                // Xenon NONE and PS3 XAS are the supported original codecs.
                if ((platform == 1 && codec != "NONE") || (platform == 2 && codec != "XAS")) continue;
                string attrs = "File='" + System.Security.SecurityElement.Escape(path) + "' PCCompression='" + codec +
                    "' XenonCompression='NONE' PS3Compression='XAS' PCQuality='75' XenonQuality='75' PS3Quality='75'" +
                    " IsStreamedOnPC='" + streamed.ToString().ToLowerInvariant() + "' IsStreamedOnXenon='" +
                    streamed.ToString().ToLowerInvariant() + "' IsStreamedOnPS3='" + streamed.ToString().ToLowerInvariant() + "'";
                // pcm- prefix enforces successful buffer and custom-data output
                // in the existing oracle; errors cannot compare equal as PASS.
                run("pcm-format-" + name + "-" + codec + "-" + streamed, platform,
                    "<AudioFile xmlns='uri:ea.com:eala:asset' id='Fixture' " + attrs + "/>", false);
            }
        }
        foreach (int bits in new int[] { 8, 16, 24 })
        foreach (int channels in new int[] { 1, 2 })
        foreach (int frames in new int[] { 7, 129, 2041 })
        foreach (bool mask in new bool[] { false, true })
        {
            string name = "extensible" + bits + "-" + channels + "-" + frames + "-mask" + mask;
            string path = Path.Combine(root, name + ".wav"); WriteExtensible(path, bits, channels, frames, mask);
            foreach (string codec in new string[] { "NONE", "XAS" })
            foreach (bool streamed in new bool[] { false, true })
            foreach (int platform in new int[] { 0, 1, 2 })
            {
                if ((platform == 1 && codec != "NONE") || (platform == 2 && codec != "XAS")) continue;
                string flag = streamed.ToString().ToLowerInvariant();
                string attrs = "File='" + System.Security.SecurityElement.Escape(path) + "' PCCompression='" + codec +
                    "' XenonCompression='NONE' PS3Compression='XAS' PCQuality='75' XenonQuality='75' PS3Quality='75'" +
                    " IsStreamedOnPC='" + flag + "' IsStreamedOnXenon='" + flag + "' IsStreamedOnPS3='" + flag + "'";
                run("pcm-format-" + name + "-" + codec + "-" + streamed, platform,
                    "<AudioFile xmlns='uri:ea.com:eala:asset' id='Fixture' " + attrs + "/>", false);
            }
        }
        RunMetadata(root, run);
    }
    // Complete ignored-extension group, separate from the already accepted
    // canonical format fixtures above. Every source still contains integral
    // PCM8/16/24 bytes regardless of its extension's declared GUID/valid bits.
    internal static readonly string[] MetadataVariants = {
        "cb0", "cb1", "cb21", "cb23", "cb65535",
        "valid0", "valid1", "valid-minus1", "valid-plus1", "valid65535",
        "mask1", "mask4", "maskffffffff",
        "guid-float", "guid-zero", "guid-ones", "guid-tail",
        "all-garbage", "fmt42", "fmt64"
    };

    internal static void RunMetadata(string root, Action<string, int, string, bool> run)
    {
        foreach (int bits in new int[] { 8, 16, 24 })
        foreach (int channels in new int[] { 1, 2 })
        foreach (int frames in new int[] { 7, 129 })
        foreach (string variant in MetadataVariants)
        {
            string name = "metadata-" + bits + "-" + channels + "-" + frames + "-" + variant;
            string path = Path.Combine(root, name + ".wav");
            WriteMetadataExtension(path, bits, channels, frames, variant);
            foreach (string codec in new string[] { "NONE", "XAS" })
            foreach (bool streamed in new bool[] { false, true })
            foreach (int platform in new int[] { 0, 1, 2 })
            {
                if ((platform == 1 && codec != "NONE") || (platform == 2 && codec != "XAS")) continue;
                string flag = streamed.ToString().ToLowerInvariant();
                string attrs = "File='" + System.Security.SecurityElement.Escape(path) + "' PCCompression='" + codec +
                    "' XenonCompression='NONE' PS3Compression='XAS' PCQuality='75' XenonQuality='75' PS3Quality='75'" +
                    " IsStreamedOnPC='" + flag + "' IsStreamedOnXenon='" + flag + "' IsStreamedOnPS3='" + flag + "'";
                run("pcm-" + name + "-" + codec + "-" + streamed, platform,
                    "<AudioFile xmlns='uri:ea.com:eala:asset' id='Fixture' " + attrs + "/>", false);
            }
        }
    }

    internal static void WriteMetadataExtension(string path, int bits, int channels, int frames, string variant)
    {
        WriteExtensible(path, bits, channels, frames, true);
        byte[] data = File.ReadAllBytes(path);
        if (variant.StartsWith("cb", StringComparison.Ordinal)) Set16(data, 36, int.Parse(variant.Substring(2)));
        else if (variant.StartsWith("valid", StringComparison.Ordinal))
        {
            int value = variant == "valid-minus1" ? bits - 1 : variant == "valid-plus1" ? bits + 1 : int.Parse(variant.Substring(5));
            Set16(data, 38, value);
        }
        else if (variant.StartsWith("mask", StringComparison.Ordinal))
        {
            uint mask = variant == "maskffffffff" ? uint.MaxValue : uint.Parse(variant.Substring(4));
            for (int i = 0; i < 4; ++i) data[40 + i] = (byte)(mask >> (8 * i));
        }
        else if (variant == "guid-float") data[44] = 3;
        else if (variant == "guid-zero" || variant == "guid-ones")
            for (int i = 44; i < 60; ++i) data[i] = variant == "guid-zero" ? (byte)0 : (byte)255;
        else if (variant == "guid-tail") data[59] ^= 1;
        else if (variant == "all-garbage")
            for (int i = 36; i < 60; ++i) data[i] = (byte)(0xa5 ^ (i * 7));
        else if (variant == "fmt42" || variant == "fmt64")
        {
            int extra = variant == "fmt42" ? 2 : 24;
            byte[] extended = new byte[data.Length + extra];
            Array.Copy(data, 0, extended, 0, 60);
            for (int i = 0; i < extra; ++i) extended[60 + i] = (byte)(0x70 + i);
            Array.Copy(data, 60, extended, 60 + extra, data.Length - 60);
            data = extended;
            Set32(data, 4, (uint)(data.Length - 8)); Set32(data, 16, (uint)(40 + extra));
        }
        else throw new ArgumentException("Unknown WAVE metadata fixture: " + variant);
        File.WriteAllBytes(path, data);
    }

    private static void Set16(byte[] bytes, int at, int value)
    { bytes[at] = (byte)value; bytes[at + 1] = (byte)(value >> 8); }
    private static void Set32(byte[] bytes, int at, uint value)
    { for (int i = 0; i < 4; ++i) bytes[at + i] = (byte)(value >> (8 * i)); }

    private static void BE(BinaryWriter b, uint value, int bytes)
    { for (int i = bytes - 1; i >= 0; --i) b.Write((byte)(value >> (i * 8))); }
    internal static void Write(string path, bool aiff, int bits, int channels, int frames)
    {
        int length = bits / 8 * channels * frames;
        using (var file = File.Create(path)) using (var b = new BinaryWriter(file))
        {
            if (aiff)
            {
                b.Write(Encoding.ASCII.GetBytes("FORM")); BE(b, (uint)(46 + length + (length & 1)), 4);
                b.Write(Encoding.ASCII.GetBytes("AIFFCOMM")); BE(b, 18, 4); BE(b, (uint)channels, 2);
                BE(b, (uint)frames, 4); BE(b, (uint)bits, 2);
                // 22050 as IEEE 80-bit extended: 0x400d ac44000000000000.
                b.Write(new byte[] { 0x40, 0x0d, 0xac, 0x44, 0, 0, 0, 0, 0, 0 });
                b.Write(Encoding.ASCII.GetBytes("SSND")); BE(b, (uint)(length + 8), 4); BE(b, 0, 4); BE(b, 0, 4);
            }
            else
            {
                b.Write(Encoding.ASCII.GetBytes("RIFF")); b.Write(36 + length + (length & 1)); b.Write(Encoding.ASCII.GetBytes("WAVEfmt "));
                b.Write(16); b.Write((short)1); b.Write((short)channels); b.Write(22050); b.Write(22050 * channels * bits / 8);
                b.Write((short)(channels * bits / 8)); b.Write((short)bits); b.Write(Encoding.ASCII.GetBytes("data")); b.Write(length);
            }
            int[] values = { 0, 1, -1, 8388607, -8388608, 0x123456, -0x123456, 255, -256, 65535, -65536 };
            for (int i = 0; i < channels * frames; ++i)
            {
                int value = values[i % values.Length] >> (24 - bits);
                if (!aiff && bits == 8) value += 128;
                if (aiff) BE(b, unchecked((uint)value), bits / 8);
                else for (int j = 0; j < bits / 8; ++j) b.Write((byte)(value >> (8 * j)));
            }
            if ((length & 1) != 0) b.Write((byte)0);
        }
    }
    internal static void WriteExtensible(string path, int bits, int channels, int frames, bool channelMask)
    {
        Write(path, false, bits, channels, frames);
        byte[] pcm = File.ReadAllBytes(path);
        using (var file = File.Create(path)) using (var b = new BinaryWriter(file))
        {
            b.Write(pcm, 0, 4); b.Write(pcm.Length + 24 - 8); b.Write(pcm, 8, 8); b.Write(40);
            b.Write((ushort)0xfffe); b.Write(pcm, 22, 14); b.Write((ushort)22); b.Write((ushort)bits);
            b.Write(channelMask ? (channels == 1 ? 4u : 3u) : 0u);
            b.Write(new byte[] { 1, 0, 0, 0, 0, 0, 0x10, 0, 0x80, 0, 0, 0xaa, 0, 0x38, 0x9b, 0x71 });
            b.Write(pcm, 36, pcm.Length - 36);
        }
    }
}
