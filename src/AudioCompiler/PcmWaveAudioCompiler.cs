using System;
using System.IO;
using System.Xml;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Fully managed backend for the currently recovered no-loop PCM8/PCM16 WAVE
    // input subset. Unsupported codec/filter/input combinations are explicit.
    public sealed class PcmWaveAudioCompiler : IAudioFileCompiler
    {
        private static readonly bool[] compressionWarnings = new bool[3];
        public AssetBuffer CompileAudioFile(InstanceDeclaration declaration, TargetPlatform platform)
        {
            XmlAttributeCollection attrs = declaration.XmlNode.Attributes;
            string file = attrs["File"].Value;
            string prefix;
            switch (platform)
            {
                case TargetPlatform.Win32: prefix = "PC"; break;
                case TargetPlatform.Xbox360: prefix = "Xenon"; break;
                case TargetPlatform.PlayStation3: prefix = "PS3"; break;
                default: throw new BinaryAssetBuilderException((ErrorCode)1, "Internal error: unknown platform {0}", platform);
            }
            int? compression = Compression(attrs[prefix + "Compression"], platform);
            string streamedKey = "IsStreamedOn" + prefix;
            bool? streamed = attrs[streamedKey] == null ? (bool?)null : Bool(attrs[streamedKey].Value);
            AudioEncodingPolicy policy = AudioEncodingPolicy.Select((int)platform, file, compression, streamed);
            if (policy.SampleRepresentation != 1 && policy.SampleRepresentation != 29)
                throw new NotSupportedException("XMA encoding has not been recovered. Select NONE compression for Xbox360 PCM output.");
            if (policy.CompressionWarning && !compressionWarnings[(int)platform])
            {
                Console.WriteLine("WARNING: MP3-style compression unavailable due to legal restrictions. Files will not be compressed");
                compressionWarnings[(int)platform] = true;
            }
            byte[] wave;
            try { wave = File.ReadAllBytes(file); }
            catch (IOException) { return null; }
            catch (UnauthorizedAccessException) { return null; }
            Pcm16Encoder.PcmWaveInput input = AudioPcmInputReader.Read(wave);
            XmlAttribute requested = attrs[prefix + "SampleRate"];
            if (requested != null)
            {
                int rate = ParseCInt(requested.Value);
                if (rate != input.SampleRate)
                {
                    if (rate < 400 || rate > 96000)
                        throw new BinaryAssetBuilderException(ErrorCode.InvalidArgument,
                            "Audio file {0}: Sample rate must be between 400 and 96000", declaration);
                    throw new NotSupportedException("Audio resampling has not been recovered. Omit SampleRate or use the input rate.");
                }
            }
            // Quality is ignored for representations PCM(1) and XAS(29) by the
            // original. In particular, do not add a new [0,100] rejection here.
            PcmEncodingResult encoded = policy.SampleRepresentation == 1 ?
                Pcm16Encoder.EncodeSamples(input.Samples, input.Channels, input.SampleRate, policy.Streamed) :
                Xas1Encoder.EncodeSamples(input.Samples, input.Channels, input.SampleRate, policy.Streamed);
            PathMusicBuilder builder = new PathMusicBuilder(32, platform != TargetPlatform.Win32);
            string subtitle = attrs["SubtitleStringName"] == null ?
                "DIALOGEVENT:" + Path.GetFileNameWithoutExtension(file) + "SubTitle" : attrs["SubtitleStringName"].Value;
            builder.String(4, subtitle);
            builder.Word(12, (uint)encoded.Frames);
            builder.Word(16, (uint)encoded.SampleRate);
            builder.Byte(28, (byte)encoded.Channels);
            if (policy.Streamed)
            {
                builder.Allocate(20, encoded.HeaderData);
                builder.Word(24, (uint)encoded.HeaderData.Length);
            }
            if (File.Exists(declaration.CustomDataPath)) File.Delete(declaration.CustomDataPath);
            File.WriteAllBytes(declaration.CustomDataPath, encoded.CustomData);
            return builder.Finish();
        }

        private static int? Compression(XmlAttribute attr, TargetPlatform platform)
        {
            if (attr == null) return null;
            string text = attr.Value;
            if (platform == TargetPlatform.PlayStation3)
            {
                if (text == "XAS") return 0;
                if (text == "EALAYER3") return 1;
            }
            else
            {
                if (text == "NONE") return 0;
                if (text == (platform == TargetPlatform.Win32 ? "XAS" : "XMA")) return 1;
                if (text == "EALAYER3") return 2;
            }
            throw new NotSupportedException("Unknown audio compression setting: " + text);
        }

        private static bool Bool(string text)
        {
            return text.Length > 0 && (text[0] == '1' || text[0] == 't' || text[0] == 'T');
        }

        private static int ParseCInt(string text)
        {
            int at = 0;
            while (at < text.Length && (text[at] == ' ' || (text[at] >= '\t' && text[at] <= '\r'))) ++at;
            bool negative = at < text.Length && text[at] == '-';
            if (at < text.Length && (text[at] == '-' || text[at] == '+')) ++at;
            int radix = 10;
            if (at < text.Length && text[at] == '0')
            {
                radix = 8;
                if (at + 1 < text.Length && (text[at + 1] == 'x' || text[at + 1] == 'X')) { radix = 16; at += 2; }
            }
            uint value = 0, limit = negative ? 0x80000000u : 0x7fffffffu;
            for (; at < text.Length; ++at)
            {
                char c = text[at];
                int digit = c >= '0' && c <= '9' ? c - '0' : c >= 'a' && c <= 'f' ? c - 'a' + 10 : c >= 'A' && c <= 'F' ? c - 'A' + 10 : -1;
                if (digit < 0 || digit >= radix) break;
                value = value > (limit - (uint)digit) / (uint)radix ? limit : value * (uint)radix + (uint)digit;
            }
            return unchecked((int)(negative ? 0u - value : value));
        }
    }
}
