using System;
using System.Runtime.CompilerServices;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Complete integral PCM leaf group used by native window dispatch 0x98267:
    // 0x98010/0x97da4/0x97edd and 0x980da/0x97e7d/0x97fb3.
    // The outer old/new source-block scheduler and transform remain unrecovered.
    public static class XmaPcmWindow
    {
        // A physical binary32 store is required at the original FSTP points.
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static float Store(double value)
        { return BitConverter.ToSingle(BitConverter.GetBytes((float)value), 0); }

        private static int Check(byte[] source, int sourceOffset, int bits, int stride,
            float[] destination, int destinationOffset, int position, int end)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (destination == null) throw new ArgumentNullException("destination");
            if (bits != 16 && bits != 20 && bits != 24) throw new ArgumentOutOfRangeException("bits");
            if (stride < 1) throw new ArgumentOutOfRangeException("stride");
            if (position < 0 || end < position) throw new ArgumentOutOfRangeException("position/end");
            int count = end - position, size = bits == 16 ? 2 : 3;
            if (sourceOffset < 0 || sourceOffset > source.Length ||
                (count > 0 && (long)sourceOffset + (long)(count - 1) * stride * size + size > source.Length) ||
                (long)sourceOffset + (long)count * stride * size > Int32.MaxValue)
                throw new ArgumentOutOfRangeException("sourceOffset/count");
            if (destinationOffset < 0 || destinationOffset > destination.Length - count)
                throw new ArgumentOutOfRangeException("destinationOffset/count");
            return count;
        }
        private static int Read(byte[] source, int offset, int bits)
        {
            if (bits == 16) return unchecked((short)(source[offset] | source[offset + 1] << 8));
            int value = source[offset] | source[offset + 1] << 8 | unchecked((sbyte)source[offset + 2]) << 16;
            return bits == 20 ? value >> 4 : value;
        }
        public static void Copy(byte[] source, int bits, int stride, ref int sourceOffset,
            float[] destination, ref int destinationOffset, ref int position, int end)
        {
            int count = Check(source, sourceOffset, bits, stride, destination, destinationOffset, position, end);
            int step = count == 0 ? 0 : checked(stride * (bits == 16 ? 2 : 3));
            for (int i = 0; i < count; ++i)
            { destination[destinationOffset++] = Read(source, sourceOffset, bits); sourceOffset += step; }
            position = end;
        }
        public static void Apply(byte[] source, int bits, int stride, ref int sourceOffset,
            float[] destination, ref int destinationOffset, ref int position, int end,
            float factor, float[] state)
        {
            int count = Check(source, sourceOffset, bits, stride, destination, destinationOffset, position, end);
            if (state == null) throw new ArgumentNullException("state");
            if (state.Length != 4 || Object.ReferenceEquals(destination, state)) throw new ArgumentException("Four distinct recurrence state values required", "state");
            if (Single.IsNaN(factor) || Single.IsInfinity(factor)) throw new ArgumentOutOfRangeException("factor");
            for (int i = 0; i < 4; ++i)
                if (Single.IsNaN(state[i]) || Single.IsInfinity(state[i])) throw new ArgumentOutOfRangeException("state");
            int step = count == 0 ? 0 : checked(stride * (bits == 16 ? 2 : 3));
            float w = state[0], q = state[1], previousW = state[2], previousQ = state[3];
            for (int i = 0; i < count; ++i)
            {
                destination[destinationOffset++] = Store((double)Read(source, sourceOffset, bits) * w);
                float nextW = Store((double)q * factor + previousW);
                float nextQ = Store((double)previousQ - (double)w * factor);
                previousW = w; previousQ = q; w = nextW; q = nextQ;
                sourceOffset += step;
            }
            state[0] = w; state[1] = q; state[2] = previousW; state[3] = previousQ;
            position = end;
        }
    }
}
