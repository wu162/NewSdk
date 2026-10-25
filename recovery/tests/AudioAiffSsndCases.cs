using System;
using System.IO;

internal static class AudioAiffSsndCases
{
    internal static readonly uint[] Blocks = { 0, 1, 2, 4, 512, 4096, uint.MaxValue };
    internal static int BlockOffset(int kind) { return kind == 0 || kind == 3 ? 50 : 56; }
    internal static void Write(string path, int kind, int bits, int channels, int frames, uint block)
    {
        if (kind == 0) AudioInputFormatCases.Write(path, true, bits, channels, frames);
        else if (kind == 1) AudioAifcInputCases.Write(path, bits, channels, frames, "empty", "NONE");
        else if (kind == 2) AudioAdp4InputCases.Write(path, bits, channels, frames, 3, frames);
        else AudioAiffLoopCases.Write(path, bits, channels, frames, 129, frames - 2, false);
        byte[] bytes = File.ReadAllBytes(path); int at = BlockOffset(kind);
        for (int i = 0; i < 4; ++i) bytes[at + i] = (byte)(block >> ((3 - i) * 8));
        File.WriteAllBytes(path, bytes);
    }
    internal static void Run(string root, Action<string, int, string, bool> run)
    {
        foreach (int kind in new int[] { 0, 1, 2 }) foreach (int bits in new int[] { 8, 16, 24 })
        foreach (int channels in new int[] { 1, 2 }) foreach (int frames in new int[] { 7, 129, 2041 })
        foreach (uint block in Blocks) {
            string name = "ssnd-" + kind + "-" + bits + "-" + channels + "-" + frames + "-block" + block;
            string path = Path.Combine(root, name + (kind == 0 ? ".aiff" : ".aifc")); Write(path, kind, bits, channels, frames, block);
            foreach (string codec in new string[] { "NONE", "XAS" }) foreach (bool streamed in new bool[] { false, true })
            foreach (int platform in new int[] { 0, 1, 2 }) {
                if ((platform == 1 && codec != "NONE") || (platform == 2 && codec != "XAS")) continue;
                Case(path, name + "-" + codec + "-" + streamed, codec, streamed, platform, run);
            }
        }
        foreach (int bits in new int[] { 8, 16, 24 }) foreach (int channels in new int[] { 1, 2 }) foreach (uint block in Blocks) {
            string name = "ssnd-loop-" + bits + "-" + channels + "-block" + block;
            string path = Path.Combine(root, name + ".aiff"); Write(path, 3, bits, channels, 4096, block);
            foreach (string codec in new string[] { "NONE", "XAS" }) foreach (bool streamed in new bool[] { false, true })
                Case(path, name + "-" + codec + "-" + streamed, codec, streamed, 0, run);
        }
    }
    private static void Case(string path, string name, string codec, bool streamed, int platform, Action<string, int, string, bool> run)
    {
        string flag = streamed.ToString().ToLowerInvariant();
        string xml = "<AudioFile xmlns='uri:ea.com:eala:asset' id='Fixture' SubtitleStringName='SSND:Fixture' File='" +
            System.Security.SecurityElement.Escape(path) + "' PCCompression='" + codec +
            "' XenonCompression='NONE' PS3Compression='XAS' PCQuality='75' XenonQuality='75' PS3Quality='75'" +
            " IsStreamedOnPC='" + flag + "' IsStreamedOnXenon='" + flag + "' IsStreamedOnPS3='" + flag + "'/>";
        run(name, platform, xml, false);
    }
}
