using System;
using System.IO;

namespace BinaryAssetBuilder.AudioCompiler
{
    internal static class MpegPassthrough
    {
        // Recovered from native header parser RVA 0x44D40 and its original
        // bitrate tables at RVA 0xFE170 / 0xFE0BC. Entries are kbit/s.
        private static readonly int[][] Mpeg1 = {
            new int[] { 0,32,64,96,128,160,192,224,256,288,320,352,384,416,448 },
            new int[] { 0,32,48,56,64,80,96,112,128,160,192,224,256,320,384 },
            new int[] { 0,32,40,48,56,64,80,96,112,128,160,192,224,256,320 } };
        private static readonly int[][] Mpeg2 = {
            new int[] { 0,32,48,56,64,80,96,112,128,144,160,176,192,224,256 },
            new int[] { 0,8,16,24,32,40,48,56,64,80,96,112,128,144,160 },
            new int[] { 0,8,16,24,32,40,48,56,64,80,96,112,128,144,160 } };
        private static readonly int[] Frequencies = { 44100, 48000, 32000 };

        // Return payload bytes, excluding the four-byte header, as the original
        // unsigned short arithmetic does. The original mutates channels before
        // rejecting a free-format (bitrate index zero) frame.
        internal static int ParseHeader(byte[] data, int at, ref byte channels)
        {
            if (at < 0 || at > data.Length - 4) return 0;
            uint header = ((uint)data[at] << 24) | ((uint)data[at+1] << 16) | ((uint)data[at+2] << 8) | data[at+3];
            if ((header & 0xffe00000u) != 0xffe00000u) return 0;
            int layer = 4 - (int)((header >> 17) & 3u);
            int bitrate = (int)((header >> 12) & 15u);
            if (layer == 4 || bitrate == 15) return 0;
            int version = (int)((header >> 19) & 3u);
            if (version == 1) return 0;
            int frequency = (int)((header >> 10) & 3u);
            if (frequency == 3) return 0;
            channels = (byte)(((header >> 6) & 3u) == 3u ? 1 : 2);
            int rate = Frequencies[frequency];
            if (version == 2) rate >>= 1;
            else if (version == 0) rate >>= 2;
            if (bitrate == 0) return 0;
            int kbps = (version == 3 ? Mpeg1 : Mpeg2)[layer - 1][bitrate];
            int padding = (int)((header >> 9) & 1u);
            int size;
            if (layer == 1) size = (((12000 * kbps / rate + padding) * 4) - 4) & 65535;
            else
            {
                size = (144000 * kbps / rate) & 65535;
                if (layer == 3 && version != 3) size >>= 1;
                size = (size + padding - 4) & 65535;
            }
            return size;
        }

        // Original MP3 identifier RVA 0x2F6F0 searches byte by byte until seven
        // consecutive parsable MPEG frame headers are found; it does not require
        // the final frame payload to be within the file during identification.
        // Any short header read ends the entire scan (RVA 0x2F7AB), including
        // when an earlier candidate's declared frame span seeks beyond EOF.
        internal static int FindFirstFrame(byte[] data)
        {
            for (int candidate = 0; candidate <= data.Length - 4; ++candidate)
            {
                long position = candidate;
                byte ignored = 0;
                int frames = 0;
                while (frames != 7)
                {
                    if (position > data.Length - 4) return -1;
                    int payload = ParseHeader(data, (int)position, ref ignored);
                    if (payload == 0) break;
                    position += 4L + payload;
                    ++frames;
                }
                if (frames == 7) return candidate;
            }
            return -1;
        }

        internal static bool IsContainer(byte[] data)
        {
            // The original SIMEX registry gives these enclosing formats priority
            // over the score-75 MPEG scan. Do not reinterpret their payload as a
            // raw MPEG file merely because seven headers happen to occur inside.
            if (data.Length < 4) return false;
            return Matches(data, "RIFF") || Matches(data, "FORM") || Matches(data, "OggS") || Matches(data, "fLaC") || Matches(data, "SCHl");
        }
        private static bool Matches(byte[] data, string value)
        {
            for (int i = 0; i < value.Length; ++i) if (data[i] != value[i]) return false;
            return true;
        }

        internal static byte Validate(byte[] data, int first)
        {
            byte channels = 0;
            long position = first;
            while (position <= data.Length - 4)
            {
                int payload = ParseHeader(data, (int)position, ref channels);
                if (payload == 0) break;
                position += 4L + payload;
            }
            // NLayer is a MIT-licensed replacement for the original decoder.
            // Decode all available samples so passthrough is not reduced to a
            // header-only acceptance check. PCM output is intentionally discarded.
            using (MemoryStream stream = new MemoryStream(data, first, data.Length - first, false))
            using (NLayer.MpegFile decoder = new NLayer.MpegFile(stream))
            {
                float[] pcm = new float[8192];
                long samples = 0;
                int count;
                while ((count = decoder.ReadSamples(pcm, 0, pcm.Length)) != 0) samples += count;
                if (samples == 0) throw new InvalidDataException("No MPEG audio samples were decoded.");
            }
            return channels;
        }
    }
}
