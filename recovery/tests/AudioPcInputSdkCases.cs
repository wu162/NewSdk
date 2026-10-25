using System;
using System.Collections.Generic;
using System.IO;

// Dedicated PC SDK milestone: previously component-validated raw ADP4 and
// ignored SSND blockSize inputs. Every asset passes the original full XSD.
internal static class AudioPcInputSdkCases
{
    internal const int AssetCount = 864;
    internal static IEnumerable<string> WriteSdkCases(string root)
    {
        var elements = new List<string>();
        foreach (int bits in new int[] { 8, 16, 24 })
        foreach (int channels in new int[] { 1, 2 })
        foreach (int frames in new int[] { 7, 129, 2041 })
        foreach (int pattern in new int[] { 0, 3 })
        foreach (int delta in new int[] { -1, 0, 1 }) {
            string id = "PcInputAdp4B" + bits + "C" + channels + "F" + frames + "P" + pattern + "D" + (delta + 1);
            string name = id + ".aifc";
            AudioAdp4InputCases.Write(Path.Combine(root, name), bits, channels, frames, pattern, frames + 99, delta);
            Add(elements, id, name, 0);
        }
        foreach (int bits in new int[] { 8, 16, 24 })
        foreach (int channels in new int[] { 1, 2 })
        foreach (int rate in new int[] { 11025, 44100 }) {
            string id = "PcInputAdp4RateB" + bits + "C" + channels + "R" + rate;
            string name = id + ".aifc";
            AudioAdp4InputCases.Write(Path.Combine(root, name), bits, channels, 129, 3, 228);
            Add(elements, id, name, rate);
        }
        foreach (int kind in new int[] { 0, 1, 2, 3 })
        foreach (int bits in new int[] { 8, 16, 24 })
        foreach (int channels in new int[] { 1, 2 })
        foreach (uint block in new uint[] { 0, 1, 512, uint.MaxValue }) {
            string id = "PcInputSsndK" + kind + "B" + bits + "C" + channels + "Block" + block;
            string name = id + (kind == 0 || kind == 3 ? ".aiff" : ".aifc");
            AudioAiffSsndCases.Write(Path.Combine(root, name), kind, bits, channels, kind == 3 ? 4096 : 129, block);
            Add(elements, id, name, 0);
        }
        if (elements.Count != AssetCount) throw new Exception("PC input SDK matrix count changed: " + elements.Count);
        return elements;
    }
    private static void Add(List<string> elements, string id, string file, int rate)
    {
        foreach (string codec in new string[] { "NONE", "XAS" })
        foreach (bool streamed in new bool[] { false, true })
            elements.Add("<AudioFile id='" + id + codec + streamed + "' File='../Audio/" + file + "' PCCompression='" + codec +
                "' PCQuality='75' IsStreamedOnPC='" + streamed.ToString().ToLowerInvariant() + "'" +
                (rate == 0 ? "" : " PCSampleRate='" + rate + "'") + "/>");
    }
}
