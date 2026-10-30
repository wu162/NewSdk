using System;
using System.IO;
using System.Runtime.InteropServices;

namespace BinaryAssetBuilder.AudioCompiler
{
    // SNR codec 5 (EL31 / EALayer3 V1b), SIMEX representation 31.
    // The original interleaved samples are pinned, not marshalled/copied.
    public static class EaLayer3Encoder
    {
#if NEWSdk_MONOLITHIC
        private const string Library = "NewModCompiler.exe";
#else
        private const string Library = "NewSdk.EaLayer3.dll";
#endif
        [DllImport(Library, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern unsafe IntPtr NewSdkEaEncode(float* samples, int frames, int channels,
            int rate, int quality, int streamed, int loopStart, int loopEnd,
            out IntPtr data, out int size, out IntPtr error, out int loopByteOffset);
        [DllImport(Library, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern void NewSdkEaFree(IntPtr result);

        internal static unsafe PcmEncodingResult EncodeSamples(float[] samples, int channels,
            int rate, bool streamed, int quality, bool hasLoop, int loopStart, int loopEnd)
        {
            if (quality < 0 || quality > 100)
                throw new ArgumentOutOfRangeException("quality", "EALayer3 quality must be between 0 and 100");
            IntPtr data, error; int size, loopByteOffset;
            fixed (float* source = samples)
            {
                IntPtr result = NewSdkEaEncode(source, samples.Length / channels, channels,
                    rate, quality, streamed ? 1 : 0, hasLoop ? loopStart : -1, loopEnd,
                    out data, out size, out error, out loopByteOffset);
                try
                {
                    if (error != IntPtr.Zero) throw new InvalidDataException(Marshal.PtrToStringAnsi(error));
                    byte[] prefix = Pcm16Encoder.CreateHeader(5, channels, rate,
                        hasLoop ? loopEnd + 1 : samples.Length / channels, streamed);
                    byte[] header = new byte[hasLoop ? (streamed ? 16 : 12) : 8];
                    Buffer.BlockCopy(prefix, 0, header, 0, 8);
                    if (hasLoop)
                    {
                        header[4] |= 0x20;
                        Pcm16Encoder.WriteBE(header, 8, (uint)loopStart);
                        if (streamed) Pcm16Encoder.WriteBE(header, 12, (uint)loopByteOffset);
                    }
                    int offset = streamed ? 0 : header.Length;
                    byte[] custom = new byte[checked(offset + size)];
                    if (!streamed) Buffer.BlockCopy(header, 0, custom, 0, header.Length);
                    Marshal.Copy(data, custom, offset, size);
                    return new PcmEncodingResult(streamed ? header : new byte[0], custom,
                        rate, channels, samples.Length / channels);
                }
                finally { NewSdkEaFree(result); }
            }
        }
    }
}
