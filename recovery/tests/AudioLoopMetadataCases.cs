using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// Every altered fixture is paired with an independent canonical source with
// the same PCM/ADP4 payload and expected range. The host checks both original
// semantics and original/recovered full-buffer equivalence.
internal static class AudioLoopMetadataCases
{
    internal static readonly string[] AiffVariants = { "canonical", "extra-before", "extra-after", "duplicate-start", "duplicate-end", "duplicate-both", "multi-mark", "multi-inst", "release", "mode2", "mode0", "reverse-groups" };
    internal static readonly string[] WaveVariants = { "canonical", "mode1", "mode2", "mode-max", "multi-loop", "multi-smpl", "first-zero", "ignored-fields" };
    internal const int Records = 4224;
    internal static void Run(string root, Action<string, int, string, bool> run)
    {
        foreach (string format in new string[] { "wave", "aiff", "aifc", "adp4" })
        foreach (int bits in new int[] { 8, 16, 24 })
        foreach (int channels in new int[] { 1, 2 })
        foreach (string variant in format == "wave" ? WaveVariants : AiffVariants)
        {
            string stem = "loopmeta-" + format + "-" + bits + "-" + channels + "-" + variant;
            string path = Path.Combine(root, stem + "." + (format == "wave" ? "wav" : "aiff"));
            string canonical = path + ".canonical";
            Write(path, format, bits, channels, variant, false);
            Write(canonical, format, bits, channels, variant, true);
            foreach (string codec in new string[] { "NONE", "XAS" })
            foreach (bool streamed in new bool[] { false, true })
            foreach (int rate in new int[] { 0, 11025 })
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
            "' SubtitleStringName='DIALOGEVENT:LoopMetadata'" + (rate == 0 ? "" : " PCSampleRate='" + rate + "'") + "/>";
    }
    internal static void Write(string path, string format, int bits, int channels, string variant, bool canonical)
    {
        if (format == "adp4") AudioAdp4InputCases.Write(path, bits, channels, 257, 3, 257);
        else if (format == "aifc") AudioAifcInputCases.Write(path, bits, channels, 257, "empty", "NONE");
        else AudioInputFormatCases.Write(path, format != "wave", bits, channels, 257);
        byte[] source = File.ReadAllBytes(path);
        bool active = variant != "mode2" || format == "wave";
        if (variant == "mode0" || variant == "first-zero") active = false;
        using (var stream = File.Create(path)) using (var writer = new BinaryWriter(stream))
        {
            writer.Write(source);
            if (format == "wave")
            {
                if (!canonical || active)
                {
                    uint type = variant == "mode1" ? 1u : variant == "mode2" ? 2u : variant == "mode-max" ? uint.MaxValue : 0u;
                    Smpl(writer, canonical ? 1 : variant == "first-zero" ? 0 : variant == "multi-loop" ? 2 : 1, canonical ? 0u : type, !canonical && variant == "ignored-fields");
                    if (!canonical && (variant == "multi-smpl" || variant == "first-zero")) Smpl(writer, 1, 0, false, 1, 30);
                }
            }
            else if (!canonical || active)
            {
                if (canonical) { Inst(writer, 1, false, 7, 11); Mark(writer, new int[] { 7, 17, 11, 223 }); }
                else
                {
                    if (variant == "multi-inst") Inst(writer, 2, true, 99, 100);
                    if (variant == "reverse-groups") Mark(writer, new int[] { 7, 1, 11, 50 });
                    Inst(writer, variant == "mode2" ? 2 : variant == "mode0" ? 0 : 1, variant == "release", 7, 11);
                    if (variant == "multi-mark") Mark(writer, new int[] { 7, 1, 11, 50 });
                    var markers = new List<int>();
                    if (variant == "extra-before") markers.AddRange(new int[] { 99, 3 });
                    if (variant == "duplicate-start" || variant == "duplicate-both") markers.AddRange(new int[] { 7, 1 });
                    if (variant == "duplicate-end" || variant == "duplicate-both") markers.AddRange(new int[] { 11, 50 });
                    markers.AddRange(new int[] { 7, 17, 11, 223 });
                    if (variant == "extra-after") markers.AddRange(new int[] { 100, 250 });
                    Mark(writer, markers.ToArray());
                }
            }
            writer.Flush(); long length = stream.Length; stream.Position = 4;
            if (format == "wave") writer.Write(checked((uint)(length - 8))); else BE(writer, checked((uint)(length - 8)), 4);
        }
    }
    private static void Inst(BinaryWriter writer, int mode, bool release, int first, int last)
    {
        writer.Write(Encoding.ASCII.GetBytes("INST")); BE(writer, 20, 4);
        writer.Write(new byte[] { 60, 0, 0, 127, 1, 127 }); BE(writer, 0, 2);
        BE(writer, (uint)mode, 2); BE(writer, (uint)first, 2); BE(writer, (uint)last, 2);
        BE(writer, release ? 2u : 0u, 2); BE(writer, 199, 2); BE(writer, 200, 2);
    }
    private static void Mark(BinaryWriter writer, int[] records)
    {
        writer.Write(Encoding.ASCII.GetBytes("MARK")); BE(writer, (uint)(2 + records.Length / 2 * 8), 4); BE(writer, (uint)(records.Length / 2), 2);
        for (int i = 0; i < records.Length; i += 2)
        { BE(writer, (uint)records[i], 2); BE(writer, (uint)records[i + 1], 4); writer.Write((byte)1); writer.Write((byte)'m'); }
    }
    private static void Smpl(BinaryWriter writer, int loops, uint type, bool fields) { Smpl(writer, loops, type, fields, 17, 222); }
    private static void Smpl(BinaryWriter writer, int loops, uint type, bool fields, int first, int last)
    {
        writer.Write(Encoding.ASCII.GetBytes("smpl")); writer.Write(36 + loops * 24);
        for (int i = 0; i < 7; ++i) writer.Write(fields ? uint.MaxValue : 0u);
        writer.Write(loops); writer.Write(0);
        for (int i = 0; i < loops; ++i)
        { writer.Write(fields ? uint.MaxValue : 0u); writer.Write(type); writer.Write(i == 0 ? first : 1); writer.Write(i == 0 ? last : 30); writer.Write(fields ? uint.MaxValue : 0u); writer.Write(fields ? uint.MaxValue : 0u); }
    }
    private static void BE(BinaryWriter writer, uint value, int width)
    { for (int i = width - 1; i >= 0; --i) writer.Write((byte)(value >> (i * 8))); }
}
