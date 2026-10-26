using System;
using System.IO;

internal static class AudioAdp4InputCases
{
    internal static void Write(string path, int bits, int channels, int frames, int pattern, int declared)
    { Write(path, bits, channels, frames, pattern, declared, 0); }
    internal static void Write(string path, int bits, int channels, int frames, int pattern, int declared, int payloadDelta)
    {
        AudioAifcInputCases.Write(path, bits, channels, frames, "empty", "ADP4");
        byte[] data = File.ReadAllBytes(path); uint seed = 0x12345678;
        int length = bits / 8 * channels * frames + payloadDelta;
        if (payloadDelta != 0) {
            byte[] resized = new byte[60 + length + (length & 1)]; Array.Copy(data, resized, 60); data = resized;
            for (int i = 0; i < 4; ++i) {
                data[4 + i] = (byte)((uint)(data.Length - 8) >> ((3 - i) * 8));
                data[48 + i] = (byte)((uint)(length + 8) >> ((3 - i) * 8));
            }
        }
        // COMM frame count is deliberately independent of raw SSND length.
        for (int i = 0; i < 4; ++i) data[22 + i] = (byte)((uint)declared >> ((3 - i) * 8));
        for (int i = 0; i < length; ++i) {
            seed = unchecked(seed * 1664525 + 1013904223);
            data[60 + i] = pattern == 0 ? (byte)0 : pattern == 1 ? (byte)255 : pattern == 2 ? (byte)i : (byte)(seed >> 24);
        }
        File.WriteAllBytes(path, data);
    }
    internal static void Run(string root, Action<string, int, string, bool> run)
    {
        foreach (int bits in new int[] { 8, 16, 24 }) foreach (int channels in new int[] { 1, 2 })
        foreach (int frames in new int[] { 2, 7, 129, 1023, 1024, 1025, 2041 })
        foreach (int pattern in new int[] { 0, 1, 2, 3 }) foreach (int variant in new int[] { 0, 1, 2 }) {
            int declared = variant == 0 ? frames : variant == 1 ? 0 : frames + 99;
            string name = "adp4-input-" + bits + "-" + channels + "-" + frames + "-" + pattern + "-declared" + declared;
            string path = Path.Combine(root, name + ".aifc"); Write(path, bits, channels, frames, pattern, declared);
            foreach (string codec in new string[] { "NONE", "XAS" }) foreach (bool streamed in new bool[] { false, true })
            foreach (int platform in new int[] { 0 }) {
                Case(path, name + "-" + codec + "-" + streamed, codec, streamed, platform, 0, run);
            }
        }
        foreach (int bits in new int[] { 8, 16, 24 }) foreach (int channels in new int[] { 1, 2 })
        foreach (int delta in new int[] { -1, 1, bits / 8 * channels + 1 }) {
            string name = "adp4-ssnd-tail-" + bits + "-" + channels + "-" + delta;
            string path = Path.Combine(root, name + ".aifc"); Write(path, bits, channels, 129, 3, 129, delta);
            foreach (string codec in new string[] { "NONE", "XAS" }) foreach (bool streamed in new bool[] { false, true })
            foreach (int platform in new int[] { 0 }) {
                Case(path, name + "-" + codec + "-" + streamed, codec, streamed, platform, 0, run);
            }
        }
        foreach (int bits in new int[] { 8, 16, 24 }) foreach (int channels in new int[] { 1, 2 })
        foreach (int rate in new int[] { 11025, 44100 }) {
            string name = "adp4-resample-" + bits + "-" + channels + "-" + rate;
            string path = Path.Combine(root, name + ".aifc"); Write(path, bits, channels, 129, 3, 228);
            foreach (string codec in new string[] { "NONE", "XAS" }) foreach (bool streamed in new bool[] { false, true })
                Case(path, name + "-" + codec + "-" + streamed, codec, streamed, 0, rate, run);
        }
    }
    private static void Case(string path, string name, string codec, bool streamed, int platform, int rate, Action<string, int, string, bool> run)
    {
        string flag = streamed.ToString().ToLowerInvariant();
        string xml = "<AudioFile xmlns='uri:ea.com:eala:asset' id='Fixture' File='" +
            System.Security.SecurityElement.Escape(path) + "' PCCompression='" + codec +
            "' PCQuality='75'" +
            " IsStreamedOnPC='" + flag + "'" +
            (rate == 0 ? "" : " PCSampleRate='" + rate + "'") + "/>";
        run(name, platform, xml, false);
    }
}
