using System;
using System.IO;
using System.Text;

// The initialized no-loop domain: explicit INST, absent/empty MARK or neither
// selected ID present. Partial selected pairs and MARK-only stay out of scope.
internal static class AudioLoopMetadataOptionalCases
{
    internal static readonly string[] Variants = { "inst-only0", "inst-only1", "inst-only2", "empty-mark0", "empty-mark1", "empty-mark2", "unmatched", "aux-outside", "aux-negative" };
    internal const int Records = 3888;
    internal static void Run(string root, Action<string, int, string, bool> run)
    {
        foreach (string format in new string[] { "aiff", "aifc", "adp4" })
        foreach (int bits in new int[] { 8, 16, 24 })
        foreach (int channels in new int[] { 1, 2 })
        foreach (string variant in Variants)
        {
            string stem = "loopoptional-" + format + "-" + bits + "-" + channels + "-" + variant;
            string path = Path.Combine(root, stem + ".aiff"), canonical = path + ".canonical";
            Write(path, format, bits, channels, variant, false);
            Write(canonical, format, bits, channels, variant, true);
            foreach (string codec in new string[] { "NONE", "XAS" })
            foreach (bool streamed in new bool[] { false, true })
            foreach (int rate in new int[] { 0, 11025, 44100 })
            {
                string name = stem + "-" + codec + "-" + streamed + "-rate" + rate;
                run(name + "-canonical", 0, Xml(canonical, codec, streamed, rate), false);
                run(name + "-variant", 0, Xml(path, codec, streamed, rate), false);
            }
        }
    }
    private static string Xml(string path, string codec, bool streamed, int rate)
    {
        return "<AudioFile xmlns='uri:ea.com:eala:asset' id='Fixture' File='" + System.Security.SecurityElement.Escape(path) +
            "' PCCompression='" + codec + "' PCQuality='75' IsStreamedOnPC='" + streamed.ToString().ToLowerInvariant() +
            "' SubtitleStringName='DIALOGEVENT:LoopOptional'" + (rate == 0 ? "" : " PCSampleRate='" + rate + "'") + "/>";
    }
    internal static void Write(string path, string format, int bits, int channels, string variant, bool canonical)
    {
        if (format == "adp4") AudioAdp4InputCases.Write(path, bits, channels, 257, 3, 257);
        else if (format == "aifc") AudioAifcInputCases.Write(path, bits, channels, 257, "empty", "NONE");
        else AudioInputFormatCases.Write(path, true, bits, channels, 257);
        byte[] source = File.ReadAllBytes(path);
        bool loop = variant.StartsWith("aux-", StringComparison.Ordinal);
        using (var stream = File.Create(path)) using (var writer = new BinaryWriter(stream))
        {
            writer.Write(source);
            if (!canonical || loop)
            {
                int mode = variant.StartsWith("inst-only", StringComparison.Ordinal) || variant.StartsWith("empty-mark", StringComparison.Ordinal) ? variant[variant.Length - 1] - '0' : 1;
                Inst(writer, mode);
                if (canonical) Mark(writer, new uint[] { 7, 17, 11, 223 });
                else if (variant.StartsWith("empty-mark", StringComparison.Ordinal)) Mark(writer, new uint[0]);
                else if (variant == "unmatched") Mark(writer, new uint[] { 99, 17, 100, 223 });
                else if (loop) Mark(writer, new uint[] { 99, variant == "aux-negative" ? uint.MaxValue : 258u, 7, 17, 11, 223, 100, variant == "aux-negative" ? 0x80000000u : 0x7fffffffu });
            }
            writer.Flush(); long length = stream.Length; stream.Position = 4; BE(writer, checked((uint)(length - 8)), 4);
        }
    }
    private static void Inst(BinaryWriter writer, int mode)
    {
        writer.Write(Encoding.ASCII.GetBytes("INST")); BE(writer, 20, 4);
        writer.Write(new byte[] { 60, 0, 0, 127, 1, 127 }); BE(writer, 0, 2);
        BE(writer, (uint)mode, 2); BE(writer, 7, 2); BE(writer, 11, 2);
        BE(writer, 0, 2); BE(writer, 0, 2); BE(writer, 0, 2);
    }
    private static void Mark(BinaryWriter writer, uint[] records)
    {
        writer.Write(Encoding.ASCII.GetBytes("MARK")); BE(writer, (uint)(2 + records.Length / 2 * 8), 4); BE(writer, (uint)(records.Length / 2), 2);
        for (int i = 0; i < records.Length; i += 2)
        { BE(writer, records[i], 2); BE(writer, records[i + 1], 4); writer.Write((byte)1); writer.Write((byte)'m'); }
    }
    private static void BE(BinaryWriter writer, uint value, int width)
    { for (int i = width - 1; i >= 0; --i) writer.Write((byte)(value >> (i * 8))); }
}
