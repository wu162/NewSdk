using System;
using System.IO;
using System.Text;

// Original-DLL evidence collection for the unrecovered loop capability.
// The caller must enforce a non-null buffer and custom file for loop-* cases.
internal static class AudioLoopProbeCases
{
    internal static void Run(string root, Action<string, int, string, bool> run)
    {
        foreach (int channels in new int[] { 1, 2 })
        foreach (int start in new int[] { 0, 1, 100, 127, 128, 129, 1019, 1020 })
        foreach (int end in new int[] { 4094, 4095 })
        {
            Case(root, run, channels, start, end);
        }
        // Short loop tails exercise the native partial XAS flush independently
        // of long source and intro segments; avoid multiplying the full grid.
        foreach (int channels in new int[] { 1, 2 })
        foreach (int start in new int[] { 0, 129 })
        foreach (int frames in new int[] { 1, 127, 128, 129 })
            Case(root, run, channels, start, start + frames - 1);
        foreach (int start in new int[] { 3435, 3436 })
            Case(root, run, 1, start, 4095);
        foreach (int start in new int[] { 509, 510, 1717, 1718 })
            Case(root, run, 2, start, 4095);
    }

    private static void Case(string root, Action<string, int, string, bool> run,
        int channels, int start, int end)
    {
        string name = "loop-" + channels + "-" + start + "-" + end;
        string file = Path.Combine(root, name + ".wav");
        Wave(file, channels, 4096, start, end);
        foreach (string codec in new string[] { "NONE", "XAS" })
        foreach (bool streamed in new bool[] { false, true })
        {
            string xml = "<AudioFile xmlns='uri:ea.com:eala:asset' id='Fixture' File='" +
                System.Security.SecurityElement.Escape(file) + "' PCCompression='" + codec +
                "' PCQuality='75' IsStreamedOnPC='" + streamed.ToString().ToLowerInvariant() + "'/>";
            run(name + "-" + codec + "-" + streamed, 0, xml, false);
        }
    }

    internal static void Wave(string file, int channels, int frames, int start, int end)
    {
        using (var stream = File.Create(file))
        using (var writer = new BinaryWriter(stream))
        {
            int pcmBytes = frames * channels * 2;
            writer.Write(Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + pcmBytes + 68);
            writer.Write(Encoding.ASCII.GetBytes("WAVEfmt "));
            writer.Write(16); writer.Write((short)1); writer.Write((short)channels);
            writer.Write(22050); writer.Write(22050 * channels * 2);
            writer.Write((short)(channels * 2)); writer.Write((short)16);
            writer.Write(Encoding.ASCII.GetBytes("data")); writer.Write(pcmBytes);
            short[] values = { 0, 1, -1, 32767, -32768, 0x1234, -2048, 1024, 4567 };
            for (int sample = 0; sample < frames * channels; sample++)
                writer.Write(values[sample % values.Length]);
            writer.Write(Encoding.ASCII.GetBytes("smpl")); writer.Write(60);
            writer.Write(0); writer.Write(0); writer.Write(1000000000 / 22050);
            writer.Write(60); writer.Write(0); writer.Write(0); writer.Write(0);
            writer.Write(1); writer.Write(0);
            writer.Write(0); writer.Write(0); writer.Write(start); writer.Write(end);
            writer.Write(0); writer.Write(0);
        }
    }
}
