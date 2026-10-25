using System;
using System.IO;
using System.Text;

internal static class AudioAifcInputCases
{
    internal static readonly string[] Variants = {
        "minimal", "empty", "odd-name", "even-name", "long-name", "trailing-name-data",
        "fver-before", "fver-after", "fver-unknown", "ssnd-first"
    };

    internal static void Run(string root, Action<string, int, string, bool> run)
    {
        foreach (int bits in new int[] { 8, 16, 24 })
        foreach (int channels in new int[] { 1, 2 })
        foreach (int frames in new int[] { 7, 129, 2041 })
        foreach (string variant in Variants)
        {
            string name = "aifc-none-" + bits + "-" + channels + "-" + frames + "-" + variant;
            string path = Path.Combine(root, name + ".aifc");
            Write(path, bits, channels, frames, variant, "NONE");
            foreach (string codec in new string[] { "NONE", "XAS" })
            foreach (bool streamed in new bool[] { false, true })
            foreach (int platform in new int[] { 0, 1, 2 })
            {
                if ((platform == 1 && codec != "NONE") || (platform == 2 && codec != "XAS")) continue;
                string flag = streamed.ToString().ToLowerInvariant();
                string xml = "<AudioFile xmlns='uri:ea.com:eala:asset' id='Fixture' File='" +
                    System.Security.SecurityElement.Escape(path) + "' PCCompression='" + codec +
                    "' XenonCompression='NONE' PS3Compression='XAS' PCQuality='75' XenonQuality='75' PS3Quality='75'" +
                    " IsStreamedOnPC='" + flag + "' IsStreamedOnXenon='" + flag + "' IsStreamedOnPS3='" + flag + "'/>";
                run(name + "-" + codec + "-" + streamed, platform, xml, false);
            }
        }
    }

    internal static void Write(string path, int bits, int channels, int frames, string variant, string compression)
    {
        AudioInputFormatCases.Write(path, true, bits, channels, frames);
        byte[] aiff = File.ReadAllBytes(path);
        byte[] name;
        if (variant == "minimal") name = new byte[0];
        else {
            int length = variant == "odd-name" ? 3 : variant == "even-name" ? 4 : variant == "long-name" ? 255 : 0;
            name = new byte[(length + 2) & ~1]; name[0] = (byte)length;
            for (int i = 0; i < length; ++i) name[i + 1] = (byte)('A' + (i % 26));
            if (variant == "trailing-name-data") Array.Resize(ref name, name.Length + 8);
        }
        using (var file = File.Create(path)) using (var writer = new BinaryWriter(file))
        {
            writer.Write(Encoding.ASCII.GetBytes("FORM")); BE(writer, 0, 4); writer.Write(Encoding.ASCII.GetBytes("AIFC"));
            if (variant.StartsWith("fver", StringComparison.Ordinal) && variant != "fver-after")
                Fver(writer, variant == "fver-unknown" ? 0x12345678u : 0xa2805140u);
            if (variant == "ssnd-first") writer.Write(aiff, 38, aiff.Length - 38);
            writer.Write(Encoding.ASCII.GetBytes("COMM")); BE(writer, (uint)(22 + name.Length), 4);
            writer.Write(aiff, 20, 18); writer.Write(Encoding.ASCII.GetBytes(compression)); writer.Write(name);
            if (variant == "fver-after") Fver(writer, 0xa2805140u);
            if (variant != "ssnd-first") writer.Write(aiff, 38, aiff.Length - 38);
            writer.Flush(); long length = file.Length; file.Position = 4; BE(writer, checked((uint)(length - 8)), 4);
        }
    }

    internal static string[] WriteSdkCases(string audio)
    {
        var result = new System.Collections.Generic.List<string>();
        foreach (int bits in new int[] { 8, 16, 24 })
        foreach (int channels in new int[] { 1, 2 })
        foreach (string variant in new string[] { "minimal", "long-name", "trailing-name-data", "fver-unknown", "ssnd-first" })
        {
            string name = "aifc-" + bits + "-" + channels + "-" + variant + ".aifc";
            Write(Path.Combine(audio, name), bits, channels, 129, variant, "NONE");
            foreach (string codec in new string[] { "NONE", "XAS" })
            foreach (bool streamed in new bool[] { false, true })
                result.Add("<AudioFile id='AifcNone" + bits + "C" + channels + variant.Replace("-", "") + codec + streamed +
                    "' File='../Audio/" + name + "' PCCompression='" + codec + "' IsStreamedOnPC='" + streamed.ToString().ToLowerInvariant() + "'/>");
        }
        return result.ToArray();
    }

    private static void Fver(BinaryWriter writer, uint version)
    { writer.Write(Encoding.ASCII.GetBytes("FVER")); BE(writer, 4, 4); BE(writer, version, 4); }
    private static void BE(BinaryWriter writer, uint value, int width)
    { for (int i = width - 1; i >= 0; --i) writer.Write((byte)(value >> (i * 8))); }
}
