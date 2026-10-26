using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// Full AudioFile rate-conversion fixtures. The pcm- prefix makes the caller
// require successful buffers and CustomData for defined inputs. The eight
// original uninitialized single-frame imports retain full original evidence
// and require an exact candidate safety rejection; see SingleFrameImportBoundary. Every fixture traverses the real plugin entry point.
internal static class AudioResampleCases
{
    internal static void Run(string root, Action<string, int, string, bool> run)
    {
        // Preserve all 112 original evidence cases, including impulses, DC,
        // sine, 8191/8192/8193 frames, all three filter profile branches and
        // rate-only short inputs, with PCM/XAS and RAM/streamed output.
        AudioUnrecoveredProbeCases.Run(root, run);
        foreach (bool aiff in new bool[] { false, true })
        foreach (int bits in new int[] { 8, 16, 24 })
        foreach (int channels in new int[] { 1, 2 })
        foreach (int target in new int[] { 44100, 11025 })
        {
            string name = "resample-format-" + (aiff ? "aiff" : "wave") + bits + "-" + channels + "-" + target;
            string path = Path.Combine(root, name + (aiff ? ".aiff" : ".wav"));
            AudioInputFormatCases.Write(path, aiff, bits, channels, 257);
            Outputs(run, name, path, target);
        }
        foreach (bool aiff in new bool[] { false, true })
        foreach (int bits in new int[] { 8, 16, 24 })
        foreach (int channels in new int[] { 1, 2 })
        foreach (int target in new int[] { 44100, 11025, 8000 })
        {
            // Downsampling by exactly two rounds end4095 to2048 and clamps
            // it to2047. Other profiles exercise nonzero start rescaling.
            int start = target == 44100 ? 0 : 129;
            string name = "resample-loop-" + (aiff ? "aiff" : "wave") + bits + "-" + channels + "-" + target;
            string path = Path.Combine(root, name + (aiff ? ".aiff" : ".wav"));
            WriteLoop(path, aiff, bits, channels, 4096, start, 4095);
            Outputs(run, name, path, target);
        }
        foreach (bool aiff in new bool[] { false, true })
        foreach (int channels in new int[] { 1, 2 })
        for (int edge = 0; edge < 3; ++edge)
        {
            // frames*ratio ==1 uses the rate-only branch; 1.5 uses the
            // actual kernel and truncates to1. Unchanged rates bypass it.
            int frames = edge == 0 ? 2 : edge == 1 ? 3 : 129;
            int target = edge == 2 ? 22050 : 11025;
            int bits = aiff ? 24 : 8;
            string name = "resample-edge-" + (aiff ? "aiff" : "wave") + bits + "-" + channels + "-" + edge;
            string path = Path.Combine(root, name + (aiff ? ".aiff" : ".wav"));
            AudioInputFormatCases.Write(path, aiff, bits, channels, frames);
            Outputs(run, name, path, target);
        }
        foreach (bool aiff in new bool[] { false, true })
        foreach (int start in new int[] { 0, 129 })
        foreach (int target in new int[] { 44100, 11025 })
        {
            // end0 deliberately remains0 in the original wrapper; a positive
            // one-frame loop exercises normal start/end rounding together.
            int channels = aiff ? 2 : 1;
            string name = "resample-single-loop-" + (aiff ? "aiff" : "wave") + "-" + start + "-" + target;
            string path = Path.Combine(root, name + (aiff ? ".aiff" : ".wav"));
            WriteLoop(path, aiff, 16, channels, 257, start, start);
            Outputs(run, name, path, target);
        }
        // 112 existing +96 format +144 loop +48 boundary +32 single-frame
        // loop =432 PC records, including8 original undefined imports.
    }

    private static void Outputs(Action<string, int, string, bool> run,
        string name, string path, int target)
    {
        foreach (string codec in new string[] { "NONE", "XAS" })
        foreach (bool streamed in new bool[] { false, true })
            run("pcm-" + name + "-" + codec + "-" + streamed, 0,
                Element(path, target, codec, streamed, "Fixture"), false);
    }

    private static string Element(string path, int target, string codec, bool streamed, string id)
    {
        return "<AudioFile xmlns='uri:ea.com:eala:asset' id='" + id + "' File='" +
            System.Security.SecurityElement.Escape(path) + "' PCCompression='" + codec +
            "' PCQuality='75' PCSampleRate='" + target + "' IsStreamedOnPC='" +
            streamed.ToString().ToLowerInvariant() + "'/>";
    }

    // Reusable six-asset SDK subset: all input bit depths and containers,
    // all filter profile branches, mono/stereo, PCM/XAS, RAM/stream and loops.
    // Returns complete uniquely named AudioFile elements for a caller's MOD.
    internal static string[] WriteSdkCases(string root)
    {
        Directory.CreateDirectory(root);
        var elements = new List<string>();
        for (int fixture = 0; fixture < 6; ++fixture)
        {
            bool aiff = fixture >= 3;
            int bits = new int[] { 8, 16, 24 }[fixture % 3];
            int channels = (fixture & 1) == 0 ? 1 : 2;
            int target = new int[] { 44100, 11025, 8000 }[fixture % 3];
            bool loop = fixture % 3 != 0;
            string path = Path.Combine(root, "sdk-resample-" + fixture + (aiff ? ".aiff" : ".wav"));
            if (loop) WriteLoop(path, aiff, bits, channels, 4096, 129, 4095);
            else AudioInputFormatCases.Write(path, aiff, bits, channels, 257);
            elements.Add(Element(path, target, (fixture & 1) == 0 ? "NONE" : "XAS",
                fixture % 3 == 1, "AudioResampleSdk" + fixture));
        }
        return elements.ToArray();
    }

    private static void WriteLoop(string path, bool aiff, int bits, int channels,
        int frames, int start, int end)
    {
        if (aiff)
        {
            AudioAiffLoopCases.Write(path, bits, channels, frames, start, end, false);
            return;
        }
        AudioInputFormatCases.Write(path, false, bits, channels, frames);
        using (var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
        using (var writer = new BinaryWriter(stream))
        {
            stream.Position = stream.Length;
            writer.Write(Encoding.ASCII.GetBytes("smpl")); writer.Write(60);
            writer.Write(0); writer.Write(0); writer.Write(1000000000 / 22050);
            writer.Write(60); writer.Write(0); writer.Write(0); writer.Write(0);
            writer.Write(1); writer.Write(0);
            writer.Write(0); writer.Write(0); writer.Write(start); writer.Write(end);
            writer.Write(0); writer.Write(0);
            writer.Flush();
            long size = stream.Length - 8;
            stream.Position = 4;
            writer.Write(checked((int)size));
        }
    }
}
