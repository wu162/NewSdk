using System;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Original 0x48500 plus the nibble order in 0x3b970/0x3bab0. This is
    // EA ADP4's raw nibble decoder, not Apple ima4's packet-header format.
    // Standalone component only; the AudioFile AIFC/ADP4 route stays disabled.
    public static class Adp4Decoder
    {
        private static readonly int[] Steps = {
            7,8,9,10,11,12,13,14,16,17,19,21,23,25,28,31,34,37,41,45,50,55,60,66,73,80,88,97,107,118,
            130,143,157,173,190,209,230,253,279,307,337,371,408,449,494,544,598,658,724,796,876,963,1060,
            1166,1282,1411,1552,1707,1878,2066,2272,2499,2749,3024,3327,3660,4026,4428,4871,5358,5894,
            6484,7132,7845,8630,9493,10442,11487,12635,13899,15289,16818,18500,20350,22385,24623,27086,29794,32767
        };
        private static readonly int[] Adjust = { -1,-1,-1,-1,2,4,6,8 };

        public static int DecodeNibble(int nibble, ref int predictor, ref int stepIndex)
        {
            if (predictor < short.MinValue || predictor > short.MaxValue)
                throw new ArgumentOutOfRangeException("predictor");
            if (stepIndex < 0 || stepIndex >= Steps.Length)
                throw new ArgumentOutOfRangeException("stepIndex");
            int step = Steps[stepIndex], delta = 0;
            if ((nibble & 4) != 0) delta = step;
            step >>= 1;
            if ((nibble & 2) != 0) delta += step;
            step >>= 1;
            if ((nibble & 1) != 0) delta += step;
            step >>= 1;
            delta += step;
            if ((nibble & 8) != 0) delta = -delta;
            int sample = predictor + delta;
            if (sample < short.MinValue) sample = short.MinValue;
            else if (sample > short.MaxValue) sample = short.MaxValue;
            int index = stepIndex + Adjust[nibble & 7];
            if (index < 0) index = 0;
            else if (index > 88) index = 88;
            predictor = sample; stepIndex = index;
            return sample;
        }

        // Mono: high/low nibbles in succession. Stereo: high=left, low=right,
        // each with independent predictor/index state, as the native readers.
        public static short[] Decode(byte[] source, int offset, int channels, int frames)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (offset < 0 || offset > source.Length) throw new ArgumentOutOfRangeException("offset");
            if (channels != 1 && channels != 2) throw new ArgumentOutOfRangeException("channels");
            if (frames < 0 || (long)frames * channels > int.MaxValue) throw new ArgumentOutOfRangeException("frames");
            long bytes = ((long)frames * channels + 1) / 2;
            if (bytes > source.Length - offset) throw new ArgumentException("Truncated ADP4 nibble stream", "source");
            short[] result = new short[frames * channels];
            int left = 0, leftIndex = 0, right = 0, rightIndex = 0;
            for (int frame = 0; frame < frames; ++frame)
            {
                if (channels == 1)
                {
                    int nibble = source[offset + frame / 2];
                    nibble = (frame & 1) == 0 ? nibble >> 4 : nibble & 15;
                    result[frame] = (short)DecodeNibble(nibble, ref left, ref leftIndex);
                }
                else
                {
                    int packed = source[offset + frame];
                    result[frame * 2] = (short)DecodeNibble(packed >> 4, ref left, ref leftIndex);
                    result[frame * 2 + 1] = (short)DecodeNibble(packed & 15, ref right, ref rightIndex);
                }
            }
            return result;
        }
    }
}
