using System;
using System.IO;
using System.Text;

internal static class AudioAiffLoopCases
{
    internal static void Run(string root, Action<string, int, string, bool> run)
    {
        foreach (int bits in new int[] { 8, 16, 24 })
        foreach (int channels in new int[] { 1, 2 })
        foreach (int start in new int[] { 0, 129 })
        foreach (int frames in new int[] { 1, 127, 129, 2041 })
            Case(root, run, bits, channels, start, start + frames - 1, false);
        // Chunk order and marker order need not match the sustain ID order.
        Case(root, run, 16, 1, 1, 4094, true);
        Case(root, run, 24, 2, 1019, 4095, true);
    }

    private static void Case(string root, Action<string, int, string, bool> run,
        int bits, int channels, int start, int end, bool reverse)
    {
        string name = "loop-aiff" + bits + "-" + channels + "-" + start + "-" + end + "-" + reverse;
        string path = Path.Combine(root, name + ".aiff");
        Write(path, bits, channels, 4096, start, end, reverse);
        foreach (string codec in new string[] { "NONE", "XAS" })
        foreach (bool streamed in new bool[] { false, true })
        {
            string xml = "<AudioFile xmlns='uri:ea.com:eala:asset' id='Fixture' File='" +
                System.Security.SecurityElement.Escape(path) + "' PCCompression='" + codec +
                "' PCQuality='75' IsStreamedOnPC='" + streamed.ToString().ToLowerInvariant() + "'/>";
            run(name + "-" + codec + "-" + streamed, 0, xml, false);
        }
    }

    internal static void Write(string path, int bits, int channels, int frames,
        int start, int end, bool reverse)
    {
        AudioInputFormatCases.Write(path, true, bits, channels, frames);
        byte[] baseFile = File.ReadAllBytes(path);
        using (var file = File.Create(path))
        using (var writer = new BinaryWriter(file))
        {
            writer.Write(baseFile);
            if (reverse) { Mark(writer, start, end, true); Instrument(writer); }
            else { Instrument(writer); Mark(writer, start, end, false); }
            writer.Flush();
            long size = file.Length - 8;
            file.Position = 4;
            BE(writer, checked((uint)size), 4);
        }
    }

    private static void Instrument(BinaryWriter writer)
    {
        writer.Write(Encoding.ASCII.GetBytes("INST")); BE(writer, 20, 4);
        writer.Write(new byte[] { 60, 0, 0, 127, 1, 127 }); BE(writer, 0, 2);
        BE(writer, 1, 2); BE(writer, 7, 2); BE(writer, 11, 2);
        BE(writer, 0, 2); BE(writer, 0, 2); BE(writer, 0, 2);
    }

    private static void Mark(BinaryWriter writer, int start, int end, bool reverse)
    {
        writer.Write(Encoding.ASCII.GetBytes("MARK")); BE(writer, 20, 4); BE(writer, 2, 2);
        if (reverse)
        {
            Marker(writer, 11, end + 1, "en"); Marker(writer, 7, start, "b");
        }
        else
        {
            Marker(writer, 7, start, "b"); Marker(writer, 11, end + 1, "en");
        }
    }

    private static void Marker(BinaryWriter writer, int id, int position, string name)
    {
        BE(writer, (uint)id, 2); BE(writer, (uint)position, 4);
        writer.Write((byte)name.Length); writer.Write(Encoding.ASCII.GetBytes(name));
        if (((name.Length + 1) & 1) != 0) writer.Write((byte)0);
    }

    private static void BE(BinaryWriter writer, uint value, int bytes)
    { for (int at = bytes - 1; at >= 0; --at) writer.Write((byte)(value >> (8 * at))); }
}
