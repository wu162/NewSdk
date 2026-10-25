using System;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Native 0x98267 integral representations 61/78/94 only. This schedules
    // the complete old/new block and padding sequence, not the transform.
    public static class XmaPcmWindowScheduler
    {
        private static void Coefficients(float[] values, string name)
        {
            if (values == null) throw new ArgumentNullException(name);
            if (values.Length != 5) throw new ArgumentException("Four recurrence values and one factor required", name);
            for (int i = 0; i < values.Length; ++i)
                if (Single.IsNaN(values[i]) || Single.IsInfinity(values[i])) throw new ArgumentOutOfRangeException(name);
        }
        public static float[] Process(byte[] older, byte[] newer, int bits, int stride,
            short blockOffset, int fullFrames, int windowFrames, int[] boundaries,
            float[] rising, float[] falling)
        {
            if (older == null) throw new ArgumentNullException("older");
            if (newer == null) throw new ArgumentNullException("newer");
            if (bits != 16 && bits != 20 && bits != 24) throw new ArgumentOutOfRangeException("bits");
            if (stride < 1) throw new ArgumentOutOfRangeException("stride");
            if (fullFrames < 2 || windowFrames < 2 || (fullFrames & 1) != 0 ||
                (windowFrames & 1) != 0 || windowFrames > fullFrames || windowFrames > Int32.MaxValue / 2)
                throw new ArgumentOutOfRangeException("fullFrames/windowFrames");
            if (boundaries == null) throw new ArgumentNullException("boundaries");
            if (boundaries.Length != 4) throw new ArgumentException("Four phase boundaries required", "boundaries");
            int width = windowFrames * 2;
            foreach (int boundary in boundaries)
                if (boundary < 0 || boundary > width) throw new ArgumentOutOfRangeException("boundaries");
            Coefficients(rising, "rising"); Coefficients(falling, "falling");
            int half = windowFrames / 2;
            long initial = (long)blockOffset - half + fullFrames / 2;
            if (initial < 0 || initial > Int32.MaxValue) throw new ArgumentOutOfRangeException("blockOffset");
            int bytes = bits == 16 ? 2 : 3;
            long step = (long)stride * bytes;
            if (step > Int32.MaxValue) throw new ArgumentOutOfRangeException("stride");
            int previousAvailable = initial < fullFrames ? (int)Math.Min(width, fullFrames - initial) : 0;
            bool current = previousAvailable <= 0;
            long initialFrame = current ? initial - fullFrames : initial;
            long initialByte = initialFrame * step;
            if (initialByte > Int32.MaxValue) throw new ArgumentOutOfRangeException("blockOffset");
            int sourceOffset = (int)initialByte;
            long afterPadding = (long)sourceOffset + boundaries[0] * step;
            if (afterPadding > Int32.MaxValue) throw new ArgumentOutOfRangeException("boundaries");
            sourceOffset = (int)afterPadding;
            int position = boundaries[0], destinationOffset = position;
            float[] output = new float[width], state = new float[4];
            Array.Copy(rising, state, 4);
            Apply(older, newer, current, bits, stride, ref sourceOffset, output, ref destinationOffset, ref position, half, rising[4], state);
            Switch(previousAvailable, position, ref current, ref sourceOffset);
            Apply(older, newer, current, bits, stride, ref sourceOffset, output, ref destinationOffset, ref position, boundaries[1], rising[4], state);
            Copy(older, newer, current, bits, stride, ref sourceOffset, output, ref destinationOffset, ref position, windowFrames);
            Switch(previousAvailable, position, ref current, ref sourceOffset);
            Copy(older, newer, current, bits, stride, ref sourceOffset, output, ref destinationOffset, ref position, boundaries[2]);
            Array.Copy(falling, state, 4);
            Apply(older, newer, current, bits, stride, ref sourceOffset, output, ref destinationOffset, ref position, 3 * half, falling[4], state);
            Switch(previousAvailable, position, ref current, ref sourceOffset);
            Apply(older, newer, current, bits, stride, ref sourceOffset, output, ref destinationOffset, ref position, boundaries[3], falling[4], state);
            // Both native leading/trailing padding use positive zero. New array
            // initialization supplies the same bits, including skipped phases.
            return output;
        }
        private static void Switch(int available, int position, ref bool current, ref int offset)
        { if (!current && position >= available) { current = true; offset = 0; } }
        private static void Apply(byte[] older, byte[] newer, bool current, int bits, int stride,
            ref int source, float[] output, ref int destination, ref int position, int end, float factor, float[] state)
        { if (position < end) XmaPcmWindow.Apply(current ? newer : older, bits, stride, ref source, output, ref destination, ref position, end, factor, state); }
        private static void Copy(byte[] older, byte[] newer, bool current, int bits, int stride,
            ref int source, float[] output, ref int destination, ref int position, int end)
        { if (position < end) XmaPcmWindow.Copy(current ? newer : older, bits, stride, ref source, output, ref destination, ref position, end); }
    }
}
