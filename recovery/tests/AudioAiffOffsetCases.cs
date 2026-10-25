using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

// The original counts frames from raw SSND length without subtracting offset.
// Only fixtures whose entire native sample window stays inside SSND are used.
internal static class AudioAiffOffsetCases
{
    internal static uint BE(byte[] data, int at)
    { return ((uint)data[at] << 24) | ((uint)data[at + 1] << 16) | ((uint)data[at + 2] << 8) | data[at + 3]; }
    internal static void BE(byte[] data, int at, uint value)
    { for (int i = 0; i < 4; ++i) data[at + i] = (byte)(value >> ((3 - i) * 8)); }
    internal static int Sound(byte[] data)
    {
        for (int at = 12; at + 8 <= data.Length; at += checked(8 + (int)BE(data, at + 4) + ((int)BE(data, at + 4) & 1)))
            if (Encoding.ASCII.GetString(data, at, 4) == "SSND") return at;
        throw new Exception("Fixture SSND absent");
    }
    internal static byte[] Create(string path, int kind, int bits, int channels, int frames, int tail, uint offset, uint block)
    {
        if (kind == 0) AudioInputFormatCases.Write(path, true, bits, channels, frames);
        else if (kind == 1) AudioAifcInputCases.Write(path, bits, channels, frames, "empty", "NONE");
        else if (kind == 2) AudioAdp4InputCases.Write(path, bits, channels, frames, 3, frames);
        else AudioAiffLoopCases.Write(path, bits, channels, frames, 129, frames - 2, false);
        byte[] original = File.ReadAllBytes(path); int at = Sound(original), start = at + 16;
        int oldLength = checked((int)BE(original, at + 4)) - 8;
        int length = checked(oldLength + tail), end = start + oldLength + (oldLength & 1);
        byte[] data = new byte[original.Length + length + (length & 1) - oldLength - (oldLength & 1)];
        Array.Copy(original, data, start);
        Array.Copy(original, end, data, start + length + (length & 1), original.Length - end);
        BE(data, 4, (uint)data.Length - 8); BE(data, at + 4, (uint)length + 8);
        BE(data, at + 8, offset); BE(data, at + 12, block);
        uint random = 0x76543210;
        for (int i = 0; i < length; ++i) { random = unchecked(random * 1664525 + 1013904223); data[start + i] = (byte)(random >> 24); }
        File.WriteAllBytes(path, data); return data;
    }
    internal static byte[] Canonical(byte[] source, int bytesNeeded)
    {
        byte[] data = (byte[])source.Clone(); int at = Sound(data), start = at + 16;
        uint offset = BE(data, at + 8); BE(data, at + 8, 0);
        Array.Copy(source, start + (long)offset, data, start, bytesNeeded);
        return data;
    }
    internal static IEnumerable<int> Offsets(int kind, int length, int consumed)
    {
        var seen = new HashSet<int>(); int maximum = length - consumed;
        foreach (int offset in new int[] { 0, 1, kind == 2 ? maximum / 2 : maximum, maximum })
            if (offset >= 0 && offset <= maximum && seen.Add(offset)) yield return offset;
    }
    internal static int Run(string root, Action<string, int, string, bool> run)
    {
        int records = 0;
        foreach (int kind in new int[] { 0, 1, 2, 3 }) foreach (int bits in new int[] { 8, 16, 24 })
        foreach (int channels in new int[] { 1, 2 }) foreach (int frames in kind == 3 ? new int[] { 4096 } : new int[] { 7, 129, 2041 }) {
            int size = bits / 8 * channels, tail = size - 1, length = frames * size + tail;
            int consumed = kind == 2 ? (frames * channels + 1) / 2 : frames * size;
            foreach (int offset in Offsets(kind, length, consumed)) foreach (uint block in new uint[] { 0, uint.MaxValue }) {
                string name = "offset-" + kind + "-" + bits + "-" + channels + "-" + frames + "-" + offset + "-block" + block;
                string file = Path.Combine(root, name + (kind == 0 || kind == 3 ? ".aiff" : ".aifc"));
                byte[] data = Create(file, kind, bits, channels, frames, tail, (uint)offset, block);
                string canonical = Path.Combine(root, name + "-canonical" + Path.GetExtension(file));
                File.WriteAllBytes(canonical, Canonical(data, consumed));
                foreach (string codec in new string[] { "NONE", "XAS" }) foreach (bool streamed in new bool[] { false, true }) {
                    string key = name + "-" + codec + "-" + streamed;
                    Case(canonical, key + "-canonical", codec, streamed, run);
                    Case(file, key + "-actual", codec, streamed, run); records += 2;
                }
            }
        }
        return records;
    }
    private static void Case(string path, string name, string codec, bool streamed, Action<string, int, string, bool> run)
    {
        string xml = "<AudioFile xmlns='uri:ea.com:eala:asset' id='Fixture' SubtitleStringName='SSND:OffsetFixture' File='" +
            System.Security.SecurityElement.Escape(path) + "' PCCompression='" + codec + "' PCQuality='75' IsStreamedOnPC='" + streamed.ToString().ToLowerInvariant() + "'/>";
        run(name, 0, xml, false);
    }
}
