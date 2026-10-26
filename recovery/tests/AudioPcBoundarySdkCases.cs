using System;
using System.Collections.Generic;
using System.IO;

// Additional assets for one explicitly selected PC SDK boundary milestone.
// Reuses the exact component fixture writers, including 257-frame odd WAVE
// payloads. No malformed/refusal fixtures or synthetic canonical outputs.
internal static class AudioPcBoundarySdkCases
{
    internal const int AssetCount = 3624;
    internal static IEnumerable<string> WriteSdkCases(string root)
    {
        Directory.CreateDirectory(root);
        var elements = new List<string>();
        int loopAssets = 0, optionalAssets = 0, offsetAssets = 0;
        int loopRateAssets = 0, optionalRateAssets = 0;
        foreach (string format in new string[] { "wave", "aiff", "aifc", "adp4" })
        foreach (int bits in new int[] { 8, 16, 24 })
        foreach (int channels in new int[] { 1, 2 })
        foreach (string variant in format == "wave" ? AudioLoopMetadataCases.WaveVariants : AudioLoopMetadataCases.AiffVariants)
        {
            string id = "PcInputBoundaryLoop" + format + "B" + bits + "C" + channels + variant.Replace("-", "");
            string file = id + (format == "wave" ? ".wav" : format == "aiff" ? ".aiff" : ".aifc");
            AudioLoopMetadataCases.Write(Path.Combine(root, file), format, bits, channels, variant, false);
            AddAudioFile(elements, id, file, 0); loopAssets += 4;
            if (bits == 16 && channels == 1)
                foreach (int rate in new int[] { 11025, 44100 })
                { AddAudioFile(elements, id, file, rate); loopRateAssets += 4; }
        }
        foreach (string format in new string[] { "aiff", "aifc", "adp4" })
        foreach (int bits in new int[] { 8, 16, 24 })
        foreach (int channels in new int[] { 1, 2 })
        foreach (string variant in AudioLoopMetadataOptionalCases.Variants)
        {
            string id = "PcInputBoundaryOptional" + format + "B" + bits + "C" + channels + variant.Replace("-", "");
            string file = id + (format == "aiff" ? ".aiff" : ".aifc");
            AudioLoopMetadataOptionalCases.Write(Path.Combine(root, file), format, bits, channels, variant, false);
            AddAudioFile(elements, id, file, 0); optionalAssets += 4;
            if (bits == 16 && channels == 1)
                foreach (int rate in new int[] { 11025, 44100 })
                { AddAudioFile(elements, id, file, rate); optionalRateAssets += 4; }
        }
        foreach (int kind in new int[] { 0, 1, 2, 3 })
        foreach (int bits in new int[] { 8, 16, 24 })
        foreach (int channels in new int[] { 1, 2 })
        foreach (int frames in kind == 3 ? new int[] { 4096 } : new int[] { 7, 129, 2041 })
        {
            int size = bits / 8 * channels, tail = size - 1;
            int length = frames * size + tail;
            int consumed = kind == 2 ? (frames * channels + 1) / 2 : frames * size;
            foreach (int offset in AudioAiffOffsetCases.Offsets(kind, length, consumed))
            foreach (uint block in new uint[] { 0, uint.MaxValue })
            {
                string id = "PcInputBoundaryOffsetK" + kind + "B" + bits + "C" + channels + "F" + frames + "O" + offset + "Block" + block;
                string file = id + (kind == 0 || kind == 3 ? ".aiff" : ".aifc");
                AudioAiffOffsetCases.Create(Path.Combine(root, file), kind, bits, channels, frames, tail, (uint)offset, block);
                AddAudioFile(elements, id, file, 0); offsetAssets += 4;
            }
        }
        if (loopAssets != 1056 || optionalAssets != 648 || offsetAssets != 1352 ||
            loopRateAssets != 352 || optionalRateAssets != 216 || elements.Count != AssetCount)
            throw new Exception("PC boundary SDK matrix changed: loop=" + loopAssets + ", optional=" + optionalAssets +
                ", offset=" + offsetAssets + ", loopRates=" + loopRateAssets + ", optionalRates=" + optionalRateAssets + ", total=" + elements.Count);
        return elements;
    }
    private static void AddAudioFile(List<string> elements, string id, string file, int rate)
    {
        foreach (string codec in new string[] { "NONE", "XAS" })
        foreach (bool streamed in new bool[] { false, true })
            elements.Add("<AudioFile id='" + id + "R" + rate + codec + streamed + "' File='../Audio/" +
                System.Security.SecurityElement.Escape(file) + "' SubtitleStringName='DIALOGEVENT:PcBoundary' PCCompression='" + codec +
                "' PCQuality='75' IsStreamedOnPC='" + streamed.ToString().ToLowerInvariant() + "'" +
                (rate == 0 ? "" : " PCSampleRate='" + rate + "'") + "/>");
    }
}
