using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// One explicit SDK milestone for the signed COMM/marker correction and
// MPEG info/passthrough. Synthetic MPEG payloads are never decoded to PCM.
internal static class AudioPcMarkerRangeSdkCases
{
    internal const int MarkerAssetCount = 1728;
    internal const int MpegAssetCount = 44;
    internal const int OffsetCombinationAssetCount = 48;
    internal const int AssetCount = MarkerAssetCount + MpegAssetCount + OffsetCombinationAssetCount;
    internal static IEnumerable<string> WriteSdkCases(string root)
    {
        Directory.CreateDirectory(root);
        var elements = new List<string>();
        var ids = new HashSet<string>(StringComparer.Ordinal);
        int markerBase = 0, markerRates = 0;
        foreach (string format in new string[] { "aiff", "aifc", "adp4" })
        foreach (int bits in new int[] { 8, 16, 24 })
        foreach (int channels in new int[] { 1, 2 })
        foreach (string variant in AudioAiffMarkerRangeCases.Variants)
        {
            string id = "PcMarkerRange" + format + "B" + bits + "C" + channels + variant.Replace("-", "");
            string file = id + (format == "aiff" ? ".aiff" : ".aifc");
            AudioAiffMarkerRangeCases.Write(Path.Combine(root, file), format, bits, channels, variant, false);
            AddAudioFile(elements, ids, id, file, 0); markerBase += 4;
            if (bits == 16 && channels == 1)
                foreach (int rate in new int[] { 11025, 44100 })
                { AddAudioFile(elements, ids, id, file, rate); markerRates += 4; }
        }
        int markerTotal = elements.Count;
        foreach (bool mono in new bool[] { true, false })
        {
            string channel = mono ? "Mono" : "Stereo";
            byte[] real = Convert.FromBase64String(mono ? AudioMp3Fixtures.Mono : AudioMp3Fixtures.Stereo);
            foreach (string prefix in new string[] { "Plain", "RIFF", "FORM", "OggS", "fLaC", "SCHl", "ID3v2" })
            {
                string id = "PcMarkerRangeMpegReal" + channel + prefix;
                byte[] header = prefix == "Plain" ? new byte[0] : prefix == "ID3v2" ?
                    new byte[] { 73, 68, 51, 4, 0, 0, 0, 0, 0, 0 } : Encoding.ASCII.GetBytes(prefix);
                byte[] data = new byte[header.Length + real.Length];
                Buffer.BlockCopy(header, 0, data, 0, header.Length);
                Buffer.BlockCopy(real, 0, data, header.Length, real.Length);
                string file = id + ".mp3"; File.WriteAllBytes(Path.Combine(root, file), data);
                AddPassthrough(elements, ids, id, file);
            }
            foreach (string pattern in new string[] { "Zero", "FF", "AA", "Seed" })
            {
                string id = "PcMarkerRangeMpegSynthetic" + channel + pattern;
                byte[] data = new byte[7 * 104]; uint seed = 0x12345678;
                for (int frame = 0; frame < 7; ++frame)
                {
                    int at = frame * 104;
                    // MPEG1 Layer3, bitrate32kbit/s, 44100Hz, no padding/CRC.
                    data[at] = 255; data[at + 1] = 251; data[at + 2] = 16; data[at + 3] = mono ? (byte)196 : (byte)4;
                    for (int i = at + 4; i < at + 104; ++i)
                    {
                        seed = unchecked(seed * 1664525 + 1013904223);
                        data[i] = pattern == "Zero" ? (byte)0 : pattern == "FF" ? (byte)255 : pattern == "AA" ? (byte)170 : (byte)(seed >> 24);
                    }
                }
                string file = id + ".mp3"; File.WriteAllBytes(Path.Combine(root, file), data);
                AddPassthrough(elements, ids, id, file);
            }
        }
        int mpegTotal = elements.Count - markerTotal;
        int offsetCombinationTotal = 0;
        foreach (string variant in new string[] { "declared223", "duplicate-positive" })
        foreach (int bits in new int[] { 8, 16, 24 })
        foreach (int channels in new int[] { 1, 2 })
        {
            string id = "PcMarkerRangeOffsetAdp4B" + bits + "C" + channels + variant.Replace("-", "");
            string file = id + ".aifc", path = Path.Combine(root, file);
            AudioAiffMarkerRangeCases.Write(path, "adp4", bits, channels, variant, false);
            byte[] data = File.ReadAllBytes(path);
            int sound = AudioAiffOffsetCases.Sound(data);
            AudioAiffOffsetCases.BE(data, sound + 8, 1u);
            File.WriteAllBytes(path, data);
            AddAudioFile(elements, ids, id, file, 0); offsetCombinationTotal += 4;
        }
        if (markerBase != 1296 || markerRates != 432 || markerTotal != MarkerAssetCount ||
            mpegTotal != MpegAssetCount || offsetCombinationTotal != OffsetCombinationAssetCount ||
            elements.Count != AssetCount || ids.Count != AssetCount)
            throw new Exception("Marker/MPEG SDK matrix changed: base=" + markerBase + ", rates=" + markerRates +
                ", marker=" + markerTotal + ", MPEG=" + mpegTotal + ", offsetCombinations=" + offsetCombinationTotal + ", total=" + elements.Count + ", unique=" + ids.Count);
        return elements;
    }
    private static void AddAudioFile(List<string> elements, HashSet<string> ids, string stem, string file, int rate)
    {
        foreach (string codec in new string[] { "NONE", "XAS" })
        foreach (bool streamed in new bool[] { false, true })
        {
            string id = stem + "R" + rate + codec + streamed;
            if (!ids.Add(id)) throw new Exception("Duplicate SDK XML identifier: " + id);
            elements.Add("<AudioFile id='" + id + "' File='../Audio/" + System.Security.SecurityElement.Escape(file) +
                "' SubtitleStringName='DIALOGEVENT:PcMarkerRange' PCCompression='" + codec + "' PCQuality='75' IsStreamedOnPC='" +
                streamed.ToString().ToLowerInvariant() + "'" + (rate == 0 ? "" : " PCSampleRate='" + rate + "'") + "/>");
        }
    }
    private static void AddPassthrough(List<string> elements, HashSet<string> ids, string stem, string file)
    {
        foreach (bool streamed in new bool[] { false, true })
        {
            string id = stem + "Stream" + streamed;
            if (!ids.Add(id)) throw new Exception("Duplicate SDK XML identifier: " + id);
            elements.Add("<AudioFileMP3Passthrough id='" + id + "' File='../Audio/" + System.Security.SecurityElement.Escape(file) +
                "' SubtitleStringName='DIALOGEVENT:PcMarkerRangeMpeg' IsStreamed='" + streamed.ToString().ToLowerInvariant() + "'/>");
        }
    }
}
