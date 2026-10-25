using System;
using System.Runtime.CompilerServices;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Original 0xa81db table-backed DCT-IV, sizes64..2048 powers of two.
    // Uses recovered 0xa7de3 FFT; not an XMA codec or quantizer.
    public static class XmaDct4
    {
        private static readonly uint[][] TableBits = {
            new uint[] { 0x3c490e90u, 0x3f7ffb11u, 0x3d16c32cu, 0x3f7fd397u, 0x3d48fb30u, 0x3f7fb10fu, 0x3dc8bd36u, 0x3f7ec46du, 0x3cc90ab0u, 0x3f7fec43u, 0x3dc8fb30u },
            new uint[] { 0x3bc90f88u, 0x3f7ffec4u, 0x3c96c9b6u, 0x3f7ff4e6u, 0x3cc90ab0u, 0x3f7fec43u, 0x3d48fb30u, 0x3f7fb10fu, 0x3c490e90u, 0x3f7ffb11u, 0x3d490ab0u },
            new uint[] { 0x3b490fc6u, 0x3f7fffb1u, 0x3c16cb58u, 0x3f7ffd39u, 0x3c490e90u, 0x3f7ffb11u, 0x3cc90ab0u, 0x3f7fec43u, 0x3bc90f88u, 0x3f7ffec4u, 0x3cc90e90u },
            new uint[] { 0x3ac90fd5u, 0x3f7fffecu, 0x3b96cbc1u, 0x3f7fff4eu, 0x3bc90f88u, 0x3f7ffec4u, 0x3c490e90u, 0x3f7ffb11u, 0x3b490fc6u, 0x3f7fffb1u, 0x3c490f88u },
            new uint[] { 0x3a490fd9u, 0x3f7ffffbu, 0x3b16cbdbu, 0x3f7fffd4u, 0x3b490fc6u, 0x3f7fffb1u, 0x3bc90f88u, 0x3f7ffec4u, 0x3ac90fd5u, 0x3f7fffecu, 0x3bc90fc6u },
            new uint[] { 0x39c90fdau, 0x3f7fffffu, 0x3a96cbe2u, 0x3f7ffff5u, 0x3ac90fd5u, 0x3f7fffecu, 0x3b490fc6u, 0x3f7fffb1u, 0x3a490fd9u, 0x3f7ffffbu, 0x3b490fd5u },
        };
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static float Store(double value) { return BitConverter.ToSingle(BitConverter.GetBytes((float)value), 0); }
        private static float Float(uint bits) { return BitConverter.ToSingle(BitConverter.GetBytes(bits), 0); }
        public static int Transform(float[] data, float scale, out int outputShift)
        {
            if (data == null) throw new ArgumentNullException("data");
            int size = data.Length, order = 0;
            if (size < 64 || size > 2048 || (size & (size - 1)) != 0) throw new ArgumentOutOfRangeException("data");
            if (Single.IsNaN(scale) || Single.IsInfinity(scale)) throw new ArgumentOutOfRangeException("scale");
            foreach (float value in data)
                if (Single.IsNaN(value) || Single.IsInfinity(value)) throw new ArgumentOutOfRangeException("data");
            // The recovered FFT accepts finite intermediates. Compute in a
            // private buffer so a safe refusal on overflow cannot partially
            // modify the caller's original input.
            float[] caller = data;
            data = (float[])data.Clone();
            for (int n = size; n > 1; n >>= 1) ++order;
            uint[] bits = TableBits[order - 6];
            float w = Store((double)Float(bits[1]) * scale), q = Store(-((double)Float(bits[0]) * scale));
            float previousW = Store((double)Float(bits[3]) * scale), previousQ = Store((double)Float(bits[2]) * scale);
            float factor = -Float(bits[10]);
            int front = 0, back = size - 1, quarter = size / 4;
            for (int at = 0; at < size / 2; ++at)
            {
                float a = data[front], b = at < quarter ? data[back] : data[front + 1];
                if (at < quarter) { data[back] = data[front + 1]; back -= 2; }
                data[front] = Store((double)a * w - (double)b * q);
                data[front + 1] = Store((double)a * q + (double)b * w);
                front += 2;
                float nextW = Store((double)previousW - (double)factor * q);
                float nextQ = Store((double)factor * w + previousQ);
                previousW = w; previousQ = q; w = nextW; q = nextQ;
            }
            XmaComplexFft.Transform(data, order - 1, false);
            w = 1.0f; q = 0.0f; previousW = Float(bits[5]); previousQ = Float(bits[4]);
            front = 0; back = size - 2;
            for (int at = 0; at < quarter; ++at)
            {
                float a = data[front], b = data[front + 1], c = data[back], d = data[back + 1];
                data[front] = Store((double)a * w - (double)b * q);
                data[back + 1] = Store(-(double)q * a - (double)b * w);
                float nextW = Store((double)previousW - (double)factor * q);
                float nextQ = Store((double)factor * w + previousQ);
                previousW = w; previousQ = q; w = nextW; q = nextQ;
                data[front + 1] = Store((double)c * w + (double)d * q);
                data[back] = Store(-(double)q * c + (double)d * w);
                front += 2; back -= 2;
            }
            Array.Copy(data, caller, size);
            outputShift = 0;
            return 0;
        }
    }
}
