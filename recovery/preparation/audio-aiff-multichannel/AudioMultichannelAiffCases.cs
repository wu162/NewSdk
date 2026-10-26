using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// NEXT-GROUP PREPARATION ONLY: no candidate or SDK acceptance is implied.
// Independent AIFF/AIFC NONE 4/6-channel group; source order is preserved.
// Original-only source0d744a71/run40251368891 confirms source order on its
// bounded129-frame no-loop inputs. This full matrix still needs execution.
internal static class AudioMultichannelAiffCases
{
    internal const int AssetCount = 1728;
    private sealed class Case { internal string Name, Xml; }
    private static void BE(BinaryWriter writer, uint value, int width)
    { for (int i = width - 1; i >= 0; --i) writer.Write((byte)(value >> (8 * i))); }

    internal static void Write(string path, string container, int bits, int channels, int frames, int loopStart)
    {
        if ((container != "AIFF" && container != "AIFC") ||
            (channels != 4 && channels != 6) || (bits != 8 && bits != 16 && bits != 24) ||
            frames < 2 || frames > 4096 || loopStart < -1 || loopStart >= frames)
            throw new ArgumentException("Bounded AIFF/AIFC NONE4/6 fixture parameters required");
        int payload = checked(frames * channels * (bits / 8));
        using (var file = File.Create(path)) using (var writer = new BinaryWriter(file)) {
            writer.Write(Encoding.ASCII.GetBytes("FORM")); BE(writer, 0, 4);
            writer.Write(Encoding.ASCII.GetBytes(container));
            if (container == "AIFC") {
                writer.Write(Encoding.ASCII.GetBytes("FVER")); BE(writer, 4, 4); BE(writer, 0xa2805140u, 4);
            }
            writer.Write(Encoding.ASCII.GetBytes("COMM")); BE(writer, container == "AIFC" ? 24u : 18u, 4);
            BE(writer, (uint)channels, 2); BE(writer, (uint)frames, 4); BE(writer, (uint)bits, 2);
            writer.Write(new byte[] { 0x40,0x0d,0xac,0x44,0,0,0,0,0,0 }); // extended80:22050Hz
            if (container == "AIFC") {
                writer.Write(Encoding.ASCII.GetBytes("NONE")); writer.Write((byte)0); writer.Write((byte)0);
            } // Empty Pascal compression name, padded to an even byte length.
            writer.Write(Encoding.ASCII.GetBytes("SSND")); BE(writer, (uint)(payload + 8), 4);
            BE(writer, 0, 4); BE(writer, 0, 4); // offset and blockSize
            for (int frame = 0; frame < frames; ++frame) for (int channel = 0; channel < channels; ++channel) {
                // Per-channel DC/ramp/impulses/sign changes expose permutations
                // and crosstalk; the values stay safely inside signed16.
                int sample = (channel + 1) * 1700 + ((frame * (channel * 7 + 13)) % 8191) - 4096;
                if (frame % (29 + channel * 5) == channel) sample = -14000 + channel * 1100;
                if ((frame / 31) % 2 != 0) sample = -sample;
                if (bits == 8) writer.Write(unchecked((byte)(sbyte)(sample >> 8)));
                else if (bits == 16) BE(writer, unchecked((ushort)(short)sample), 2);
                else BE(writer, unchecked((uint)((sample << 8) | ((frame * 17 + channel * 37) & 255))), 3);
            }
            if ((payload & 1) != 0) writer.Write((byte)0);
            if (loopStart >= 0) {
                Instrument(writer);
                // Original AIFF marker end is exclusive; frames becomes
                // inclusive loopEnd=frames-1 after native conversion.
                writer.Write(Encoding.ASCII.GetBytes("MARK")); BE(writer, 20, 4); BE(writer, 2, 2);
                Marker(writer, 7, loopStart, "b"); Marker(writer, 11, frames, "en");
            }
            writer.Flush(); long total = file.Length; file.Position = 4; BE(writer, checked((uint)(total - 8)), 4);
        }
    }
    private static void Instrument(BinaryWriter writer)
    {
        writer.Write(Encoding.ASCII.GetBytes("INST")); BE(writer, 20, 4);
        writer.Write(new byte[] { 60,0,0,127,1,127 }); BE(writer, 0, 2); // gain
        BE(writer, 1, 2); BE(writer, 7, 2); BE(writer, 11, 2); // forward sustain
        BE(writer, 0, 2); BE(writer, 0, 2); BE(writer, 0, 2); // no release loop
    }
    private static void Marker(BinaryWriter writer, int id, int position, string name)
    {
        BE(writer, (uint)id, 2); BE(writer, (uint)position, 4);
        writer.Write((byte)name.Length); writer.Write(Encoding.ASCII.GetBytes(name));
        if (((name.Length + 1) & 1) != 0) writer.Write((byte)0);
    }
    private static IEnumerable<Case> Cases(string root)
    {
        Directory.CreateDirectory(root);
        foreach (string container in new[] { "AIFF", "AIFC" })
        foreach (int channels in new[] { 4,6 }) foreach (int bits in new[] { 8,16,24 })
        foreach (int frames in new[] { 127,128,129,4096 })
        foreach (int start in frames == 4096 ? new[] { -1,0,129 } : new[] { -1,0 }) {
            string stem = container + "C" + channels + "B" + bits + "F" + frames + "L" + (start < 0 ? "None" : start.ToString());
            string path = Path.Combine(root, "multichannel-aiff-" + stem + (container == "AIFF" ? ".aiff" : ".aifc"));
            Write(path, container, bits, channels, frames, start);
            foreach (int rate in new[] { 22050,11025,44100,8000 })
            foreach (string codec in new[] { "NONE", "XAS" }) foreach (bool streamed in new[] { false,true }) {
                string name = "MultichannelAiff" + stem + "R" + rate + codec + streamed;
                string xml = "<AudioFile xmlns='uri:ea.com:eala:asset' id='" + name + "' File='" +
                    System.Security.SecurityElement.Escape(path) + "' PCCompression='" + codec +
                    "' PCQuality='75' PCSampleRate='" + rate + "' IsStreamedOnPC='" + streamed.ToString().ToLowerInvariant() + "'/>";
                yield return new Case { Name = name, Xml = xml };
            }
        }
    }
    internal static void Run(string root, Action<string,int,string,bool> run)
    {
        int count = 0; var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (Case item in Cases(root)) {
            if (!ids.Add(item.Name)) throw new Exception("Duplicate AIFF multichannel case " + item.Name);
            run("multichannel-aiff-" + item.Name, 0, item.Xml, false); ++count;
        }
        if (count != AssetCount) throw new Exception("Incomplete AIFF multichannel matrix: " + count);
    }
    internal static IEnumerable<string> WriteSdkCases(string root)
    {
        int count = 0; var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (Case item in Cases(root)) {
            if (!ids.Add(item.Name)) throw new Exception("Duplicate AIFF multichannel SDK case " + item.Name);
            ++count; yield return item.Xml;
        }
        if (count != AssetCount) throw new Exception("Incomplete AIFF multichannel SDK matrix: " + count);
    }
}
