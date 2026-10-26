using System;

// Regression for original 0xf700 binary32 quantization at resampled sine
// output frame6144. The original 42480 kernel produced these signed16 values
// from 8191 input frames at 22050 -> 44100, sin(frame*0.137)*24000 PCM16.
// The native group bytes are c2f536fc464666576676766765665555344241;
// extended temporaries instead produce c2f536fc464666576757577576565555344322.
internal static class AudioXasPrecisionFixture
{
    internal static readonly short[] Signed16 = new short[]
    {
        -2620, -982, 661, 2302, 3932, 5543, 7128, 8680,
        10191, 11655, 13063, 14410, 15690, 16897, 18024, 19066,
        20019, 20878, 21639, 22299, 22854, 23302, 23640, 23868,
        23984, 23988, 23878, 23656, 23324, 22883, 22334, 21680
    };

    // Group histories reset every32 frames. Repeat the isolated original
    // group to form one native128-frame block, in mono or stereo.
    internal static float[] Samples(int channels)
    {
        if (channels != 1 && channels != 2) throw new ArgumentOutOfRangeException("channels");
        float[] result = new float[128 * channels];
        for (int frame = 0; frame < 128; ++frame)
            for (int channel = 0; channel < channels; ++channel)
                result[frame * channels + channel] = Signed16[frame % 32] * (1.0f / 32768.0f);
        return result;
    }
}
