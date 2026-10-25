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
    }
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
