using System;
using System.IO;
using System.Text;

// Complete initialized COMM/marker filtering group. Partial selected pairs
// are intentionally confined to candidate safety assertions, never native runs.
internal static class AudioAiffMarkerRangeCases
{
    internal static readonly string[] Variants = {
        "declared0", "declared1", "declared16", "declared223", "declared256", "declared257", "declared300",
        "declared-max", "declared-highbit", "declared-ones", "duplicate-positive", "duplicate-negative",
        "invalid-before-valid", "zero-boundary", "actual-boundary", "last-comm-zero", "last-comm-valid", "last-comm-negative"
    };
    internal const int Records = 7776;
    internal static bool HasLoop(string variant)
    { return variant != "declared0" && variant != "declared1" && variant != "declared16" && variant != "declared-highbit" && variant != "declared-ones" && variant != "last-comm-zero" && variant != "last-comm-negative"; }
    internal static int First(string variant) { return variant == "zero-boundary" ? 0 : 17; }
    internal static int Last(string variant) { return variant == "zero-boundary" ? 0 : variant == "actual-boundary" ? 256 : 222; }
    internal static void Run(string root, Action<string, int, string, bool> run)
    {
        foreach (string format in new string[] { "aiff", "aifc", "adp4" })
        foreach (int bits in new int[] { 8, 16, 24 }) foreach (int channels in new int[] { 1, 2 })
        foreach (string variant in Variants)
        {
            string stem = "markrange-" + format + "-" + bits + "-" + channels + "-" + variant;
            string actual = Path.Combine(root, stem + ".aiff"), canonical = actual + ".canonical";
            Write(actual, format, bits, channels, variant, false); Write(canonical, format, bits, channels, variant, true);
            foreach (string codec in new string[] { "NONE", "XAS" }) foreach (bool streamed in new bool[] { false, true })
            foreach (int rate in new int[] { 0, 11025, 44100 })
            {
                string name = stem + "-" + codec + "-" + streamed + "-rate" + rate;
                run(name + "-canonical", 0, Xml(canonical, codec, streamed, rate), false);
                run(name + "-variant", 0, Xml(actual, codec, streamed, rate), false);
            }
        }
    }
    private static string Xml(string path, string codec, bool streamed, int rate)
    { return "<AudioFile xmlns='uri:ea.com:eala:asset' id='Fixture' File='" + System.Security.SecurityElement.Escape(path) + "' SubtitleStringName='DIALOGEVENT:MarkerRange' PCCompression='" + codec + "' PCQuality='75' IsStreamedOnPC='" + streamed.ToString().ToLowerInvariant() + "'" + (rate == 0 ? "" : " PCSampleRate='" + rate + "'") + "/>"; }
    internal static void Write(string path, string format, int bits, int channels, string variant, bool canonical)
    {
        if (format == "adp4") AudioAdp4InputCases.Write(path, bits, channels, 257, 3, 257);
        else if (format == "aifc") AudioAifcInputCases.Write(path, bits, channels, 257, "empty", "NONE");
        else AudioInputFormatCases.Write(path, true, bits, channels, 257);
        byte[] source = File.ReadAllBytes(path);
        uint declared = 257;
        if (!canonical)
        {
            if (variant == "declared-max") declared = 0x7fffffffu;
            else if (variant == "declared-highbit") declared = 0x80000000u;
            else if (variant == "declared-ones") declared = uint.MaxValue;
            else if (variant.StartsWith("declared", StringComparison.Ordinal)) declared = uint.Parse(variant.Substring(8));
            else if (variant == "zero-boundary") declared = 1;
            else if (variant == "last-comm-valid") declared = 0;
        }
        Set32(source, 22, declared);
        using (var stream = File.Create(path)) using (var writer = new BinaryWriter(stream))
        {
            writer.Write(source);
            if (!canonical || HasLoop(variant))
            {
                Inst(writer);
                if (!canonical && variant == "invalid-before-valid") Mark(writer, new uint[] { 7, 258, 11, uint.MaxValue });
                Mark(writer, new uint[] { 7, (uint)First(variant), 11, (uint)(Last(variant) + 1) });
                if (!canonical && variant == "duplicate-positive") Mark(writer, new uint[] { 7, 258, 11, 259 });
                if (!canonical && variant == "duplicate-negative") Mark(writer, new uint[] { 7, 0x80000000u, 11, uint.MaxValue });
            }
            if (!canonical && variant.StartsWith("last-comm-", StringComparison.Ordinal))
            {
                byte[] last = (byte[])source.Clone(); Set32(last, 22, variant == "last-comm-valid" ? 257u : variant == "last-comm-zero" ? 0u : 0x80000000u);
                // The fixture writers put COMM first, at offset12. Preserve its
                // full format-specific bounded payload, including AIFC codec.
                int size = checked((int)(((uint)last[16] << 24) | ((uint)last[17] << 16) | ((uint)last[18] << 8) | last[19]));
                writer.Write(last, 12, 8 + size + (size & 1));
            }
            writer.Flush(); long length = stream.Length; stream.Position = 4; BE(writer, checked((uint)(length - 8)), 4);
        }
    }
    private static void Inst(BinaryWriter writer)
    {
        writer.Write(Encoding.ASCII.GetBytes("INST")); BE(writer, 20, 4); writer.Write(new byte[] { 60, 0, 0, 127, 1, 127 }); BE(writer, 0, 2);
        BE(writer, 1, 2); BE(writer, 7, 2); BE(writer, 11, 2); BE(writer, 0, 2); BE(writer, 0, 2); BE(writer, 0, 2);
    }
    private static void Mark(BinaryWriter writer, uint[] records)
    {
        writer.Write(Encoding.ASCII.GetBytes("MARK")); BE(writer, (uint)(2 + records.Length / 2 * 8), 4); BE(writer, (uint)(records.Length / 2), 2);
        for (int i = 0; i < records.Length; i += 2) { BE(writer, records[i], 2); BE(writer, records[i + 1], 4); writer.Write((byte)1); writer.Write((byte)'m'); }
    }
    internal static void Set32(byte[] bytes, int at, uint value)
    { for (int i = 0; i < 4; ++i) bytes[at + i] = (byte)(value >> ((3 - i) * 8)); }
    private static void BE(BinaryWriter writer, uint value, int width)
    { for (int i = width - 1; i >= 0; --i) writer.Write((byte)(value >> (i * 8))); }
}
