using System;
using System.Runtime.CompilerServices;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Recovery of original RVA 0x41a90, limited to filter 120's support=256
    // and integer factors 1/2. This numerical component does not enable a route.
    public static class ResamplerCoefficients
    {
        private const int Size = 8192;
        private static readonly float NegativeZero = BitConverter.ToSingle(new byte[] { 0, 0, 0, 128 }, 0);

        // Original Windows x86 CPU FCOS Hann half-window, support512, FPCW=0x027f.
        // Captured by AudioNativeWindowOracle, run40207591570 (605c6953 SHA256).
        // Host-libm/Unicorn values differ at 195 binary64 positions.
        // Weight[i-1] = ((FCOS(i*binary32(pi)/512)+1)*binary32(0.5)).
        // These binary64 values retain the original x87 53-bit intermediate;
        // factor 1 uses every second value. They must not become float values.
        private static readonly byte[] WindowBits = Convert.FromBase64String(
            "2zLHQuz/7z+1f00Lsf/vP/ECJVpO/+8/qD9BMMT+7z9HHfeOEv7vP07k/Hc5/e8/FTpq7Tj87z+fG7jxEPvvP4LXwIfB+e8/3wbAskr47z9lhVJ2rPbvP2po" +
            "dtbm9O8/DfWK1/ny7z9wlVB+5fDvP/7M6M+p7u8/wSvW0Ubs7z/JQPyJvOnvP6eLn/4K5+8/9GxlNjLk7z/pFVQ4MuHvPxB30gsL3u8/+y2ouLza7z8Tcv1G" +
            "R9fvP3wAW7+q0+8//waqKufP7z8TDjSS/MvvP/Dhov/qx+8/unoAfbLD7z+447YUU7/vP6UhkNHMuu8/Dxi2vh+27z/NbbLnS7HvP4ZwblhRrO8/TfcyHTCn" +
            "7z9TRKhC6KHvP6zl1dV5nO8/JpUi5OSW7z86F1R7KZHvPxIZj6lHi+8/og1XfT+F7z/XCY4FEX/vP+CfdFG8eO8/i7mpcEFy7z+8cSpzoGvvP/fsUWnZZO8/" +
            "BTHZY+xd7z+z+9Zz2VbvP6eYv6qgT+8/TrZkGkJI7z/kOfXUvUDvP5oS/ewTOe8/zwtldUQx7z9rnnKBTynvP07BxyQ1Ie8/5bhic/UY7z/Q5Z2BkBDvP6uS" +
            "L2QGCO8/9cApMFf/7j8U9fn6gvbuP3ABadqJ7e4/utCa5Gvk7j9GLw4wKdvuP4uTnNPB0e4/xOV55jXI7j+uRjSAhb7uP2rVs7iwtO4/f3Q6qLeq7j8DjmNn" +
            "mqDuP+DWIw9Zlu4/QRHJuPOL7j8ozvl9aoHuPxwutXi9du4/CqFSw+xr7j9DpYF4+GDuP6aFSbPgVe4/7hYJj6VK7j8sdHYnRz/uP2K6npjFM+4/UsPl/iAo" +
            "7j9s3wV3WRzuP/OODx5vEO4/PjppEWIE7j806c5uMvjtP+r5UVTg6+0/d9ZY4Gvf7T/rqZ4x1dLtP4MUM2ccxu0/AN95oEG57T82rSr9RKztP8ivUJ0mn+0/" +
            "G1VKoeaR7T9y+cgphYTtP0qW0FcCd+0/33C3TF5p7T/uxyUqmVvtP6WAFRKzTe0/1NLRJqw/7T9J9PaKhDHtP2/DcWE8I+0/HXF/zdMU7T+pKa3ySgbtPyq9" +
            "1/Sh9+w//kYr+Njo7D+L1CIh8NnsPzkLiJTnyuw/r81yd7+77D9M4Ejvd6zsP+GMvSERnew/q0XRNIuN7D+QR9FO5n3sP507V5Yibuw/ytdIMkBe7D/7ftdJ" +
            "P07sP1PgfwQgPuw/wJUJiuIt7D/WwYYChx3sP/OsU5YNDew/pGEWbnb86z9dSL6ywevrP3bCg43v2us/ecTnJwDK6z+3b7Or87jrPy6r90LKp+s/wrsMGISW" +
            "6z+725FVIYXrP6LRbCaic+s/ZIbJtQZi6z/MmhkvT1DrP1b8E757Pus/Tnm0jows6z9NVDvNgRrrPwbXLKZbCOs/deRQRhr26j9YirLavePqPxaSn5BG0eo/" +
            "7BColbS+6j+L950XCKzqPwKhlERBmeo/E2HgSmCG6j/gERZZZXPqP/ygCp5QYOo/4JvSSCJN6j/Au8GI2jnqP8Fwao15Juo/nGydhv8S6j+iLGmkbP/pPyqD" +
            "GRfB6+k/ZSA3D/3X6T+eGoe9IMTpP+R1ClMssOk/Iav9ACCc6T+fLtj4+4fpP/j1S2zAc+k/ef1EjW1f6T/1zOiNA0vpPwj8laCCNuk/1LXj9+oh6T8mPKHG" +
            "PA3pPx5q1T94+Og/RTa+lp3j6D8dNND+rM7oPy8Vtqumueg/iylQ0Yqk6D/L37OjWY/oP5BEK1cTeug/fIE0ILhk6D+tW4EzSE/oP7ux9sXDOeg/MfmrDCsk" +
            "6D+Ru+o8fg7oP9gSLoy9+Oc/iCUiMOni5z86oqNeAc3nP7k6v00Gt+c/ox6xM/ig5z+ZdeRG14rnP/nY8r2jdOc/KM2jz11e5z9jOuyyBUjnPyrl7Z6bMec/" +
            "OOb2yh8b5z8KIoFukgTnPwDAMcHz7eY/D6HY+kPX5j8N1m9Tg8DmP5MVGwOyqeY/djEnQtCS5j/eiwlJ3nvmP/qLX1DcZOY/TBLukMpN5j+X7KBDqTbmP3BJ" +
            "iqF4H+Y/aSvi4zgI5j/l2wVE6vDlP4pdd/uM2eU/Yt7cQyHC5T+YKQBXp6rlP+MYzm4fk+U/mwVWxYl75T91OcmU5mPlP/Beehc2TOU/aPHch3g05T/mrIQg" +
            "rhzlP4/9JBzXBOU/0m6QtfPs5D9DGrgnBNXkPy8Wq60IveQ/3+OVggGl5D+Z3cHh7ozkP1mklAbRdOQ/Po2PLKhc5D+/Dk+PdETkP4wtimo2LOQ/PukR+u0T" +
            "5D++qNB5m/vjP2+mySU/4+M/GVwYOtnK4z+c7u/yabLjP2eZmozxmeM/tRl5Q3CB4z+TGQJU5mjjP66awfpTUOM/9GBYdLk34z/2XHv9Fh/jPyIW89JsBuM/" +
            "yRSbMbvt4j/0S2FWAtXiPw+DRX5CvOI/aL9Y5nuj4j9/rbzLroriPzQKo2vbceI/xQtNAwJZ4j+wygrQIkDiP2SqOg8+J+I/28FI/lMO4j8KRK7aZPXhPzDo" +
            "8OFw3OE/EVKiUXjD4T8Fel9ne6rhP/QU0GB6keE/Nfyle3V44T9XlZz1bF/hP8w5eAxhRuE/hJ4F/lEt4T9xOxkIQBThP/ayjmgr++A/SjlIXRTi4D/D+y0k" +
            "+8jgPxeILfvfr+A/lDM5IMOW4D9AgkfRpH3gP/mNUkyFZOA/jW1Xz2RL4D/Am1WYQzLgP1deTuUhGeA/MVqI6P//3z+aMXQGvM3fP31kZqB4m98/ZOJjMjZp" +
            "3z/XNm849TbfP1tXhy62BN8/g3GmkHnS3j8HucDaP6DeP+81w4gJbt4/yZKSFtc73j/76gkAqQnePyeZ+cB/190/rQUm1Vul3T9HdUa4PXPdP8zXA+YlQd0/" +
            "C5f32RQP3T/aZaoPC93cP0MPkwIJq9w/5EUVLg953D95c4ANHkfcP6CIDhw2Fdw/xMzi1Ffj2z9Trgizg7HbPx6TcjG6f9s//Kj4yvtN2z+2tlf6SBzbPyjt" +
            "Lzqi6to/tbgDBQi52j/+kjbVeofaP+bUCyX7Vdo/54ilbokk2j+7PQMsJvPZP1HZANfRwdk/KGxV6YyQ2T/2BJLcV1/ZP7uEICozLtk/KnNCSx/92D940w+5" +
            "HMzYP5b5dewrm9g/0V82Xk1q2D/afOWGgTnYP0ea6d7ICNg/fat53iPY1z8SJZz9kqfXP6nUJbQWd9c/Q7m4ea9G1z8U3MLFXRbXP9EpfQ8i5tY/ikzqzfy1" +
            "1j8AhtV37oXWP4uK0YP3VdY/h1w3aBgm1j9VKCWbUfbVP+QgfZKjxtU/2lzkww6X1T9ItMGkk2fVP/+ePKoyONU/fxM8SewI1T+HZmX2wNnUP0MrGyaxqtQ/" +
            "IBR8TL171D9N1GHd5UzUP98BYEwrHtQ/o/jCDI7v0z+mvY6RDsHTP2fjfU2tktM/xW4As2pk0z+fvDo0RzbTPzVoBENDCNM/RDLnUF/a0j/d6B3Pm6zSPwZQ" +
            "ky75ftI/HQvh33dR0j8Fh05TGCTSPx3lz/ja9tE//+YEQMDJ0T8b2zeYyJzRPxyKXHD0b9E/IiUPN0RD0T/bNJNauBbRP2+J0khR6tA/UCtcbw++0D/jTGM7" +
            "85HQPxU9vhn9ZdA/ylrldi060D8+CfK+hA7QP5FKO7sGxs8/J/eAfFNvzz93f6GX8BjPP9AIw+Hews4/pGhDLx9tzj+SGLZTshfOP1Ms4iGZws0/sEnAa9Rt" +
            "zT9longCZRnNPxfwYLZLxcw/P3L6VolxzD817u+yHh7MPzqxE5gMy8s/pJRd01N4yz8iBOkw9SXLPx4G83vx08o/REbYfkmCyj80IhMD/jDKP2W4OdEP4Mk/" +
            "M/n7sH+PyT8tuiFpTj/JP53LiL9878g/ThAjeQugyD+fl/RZ+1DIP925ESVNAsg/6DadnAG0xz81V8aBGWbHPyMPx5SVGMc/riTilHbLxj+HV2FAvX7GP4yL" +
            "k1RqMsY/tPXKjX7mxT9eS1un+prFPyb0l1vfT8U/HT7SYy0FxT+QlFd45brEP0W5b1AIccQ/RQBbopYnxD8ojlAjkd7DP/iYfIf4lcM/n6v+gc1Nwz/x6+fE" +
            "EAbDP1FjOQHDvsI/8kni5uR3wj+7VL4kdzHCP9wFlGh668E/AQATX++lwT9EXNKz1mDBP8MCTxExHME//gXqIP/XwD/mAOeKQZTAP7J3avb4UMA/eDt4CSYO" +
            "wD8XoePRkpe/P1OvKXHHE78/7vPyNeuQvj+jryFj/w6+P+30RjkFjr0/FJGf9v0NvT/y+hDX6o68P4JHJhTNELw/QyQN5aWTuz9j15J+dhe7P9dFIRNAnLo/" +
            "S/+70gMiuj8FUP3qwqi5P7pYE4d+MLk/Vyy9zze5uD/X80fr70K4PxUYjP2nzbc/t3HqJ2FZtz8qf0mJHOa2P7ugEj7bc7Y/2VovYJ4Ctj98ngYHZ5K1P78X" +
            "ekc2I7U/t4LjMw21tD9/BhLc7Ee0P5aWR03W27M/elo2kspwsz+kG/6yygazP8+5KbXXnbI/qqWsm/I1sj/jYeBmHM+xP6cKghRWabE/juOvn6AEsT8A7OYA" +
            "/aCwPxZ6AC5sPrA/ALhfNN65rz/f//9rDfmuPzFFot5nOq4/gQitYu99rT/SSCjJpcOsPzUIud2MC6w/rt2cZqZVqz9hlKUk9KGqPzLYNNN38Kk/ve83KDNB" +
            "qT/RgyPUJ5SoP2Z074FX6ac/HbsS18NApz9SW39zbpqmP9RfnvFY9qU/OuZL5oRUpT/3N9Pg87SkPx7x6mqnF6Q/8DSxCKF8oz878Kc44uOiP4wpsXNsTaI/" +
            "R18LLUG5oT+f803SYSehP46mZcvPl6A/wB2ReowKoD8B9bp4Mv+eP3L7Rc/u7Z0/BXQDm1DhnD+op7xyWtmbP6afveEO1po/p97OZ3DXmT8bNi95gd2YPxO4" +
            "jX5E6Jc/rMUD1bv3lj8IOg/O6QuWP+2xjK/QJJU/F/Cxs3JClD9LXggJ0mSTPzSrZ9Lwi5I/G4XwJtG3kT+McQcSdeiQP+nBT5PeHZA//UlNPR+wjj9bqzw4" +
            "FC6NP0HN8s+ftYs/Y1ZMpcVGij/zenRBieGIPzhD3BXuhYc/sQwyfPczhj/YRVm2qOuEP59kYu4ErYM/pxeDNg94gj9nsg6JykyBPzDUbsg5K4A/dJQ4fr8m" +
            "fj+JWTA/fgp8P6puygi1AXo/W7gC4GgMeD8DJr+Znip2PwvGw9paXHQ/r1CnF6Khcj+7KciUePpwP3Oyg8zEzW4/ffbF38bNaz+tTUfK/vRoP4aYOJJzQ2Y/" +
            "FxX63Cu5Yz8H+QrvLVZhP1L181f/NF4/e5KtLE0MWj8U6UifTzJWP8qHmTAQp1I/ZfNRPi/VTj/aZULO2/lIP8a5cYQ0vEM/QwmGnow4Pj/7bN4vOTQ2P+zI" +
            "owkF1y4/35gQtfK8Iz/YLQuNrTQWP8w00DEavQM/cAl78BO94z4AAAAAwDXhPA==");

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static float F(float value) { return value; }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static double D(double value) { return value; }

        // Frequencies are the original normalized breakpoint coordinates, not
        // sample-rate Hz. The final coordinate is 1. Gain is applied before
        // interpolation, preserving each SSE single-precision operation.
        public static float[] Create(int factor, float[] frequencies, float[] amplitudes, float gain, bool invert)
        {
            if (factor != 1 && factor != 2) throw new ArgumentOutOfRangeException("factor");
            if (frequencies == null) throw new ArgumentNullException("frequencies");
            if (amplitudes == null) throw new ArgumentNullException("amplitudes");
            if (frequencies.Length < 2 || frequencies.Length != amplitudes.Length || frequencies.Length > 100)
                throw new ArgumentException("Matching profile arrays with 2..100 breakpoints are required");
            if (Single.IsNaN(gain) || Single.IsInfinity(gain)) throw new ArgumentException("Finite gain required");
            for (int i = 0; i < frequencies.Length; ++i)
                if (Single.IsNaN(frequencies[i]) || Single.IsInfinity(frequencies[i]) ||
                    Single.IsNaN(amplitudes[i]) || Single.IsInfinity(amplitudes[i]) ||
                    frequencies[i] < 0 || frequencies[i] > 1 || (i != 0 && frequencies[i] <= frequencies[i - 1]))
                    throw new ArgumentException("Finite amplitudes and strictly ascending normalized frequencies required");
            if (frequencies[0] != 0 || frequencies[frequencies.Length - 1] != 1)
                throw new ArgumentException("The profile must cover the full normalized interval");

            float[] real = new float[Size], imaginary = new float[Size];
            int half = Size / (2 * factor);
            real[0] = F(amplitudes[0] * gain);
            for (int segment = 1; segment < frequencies.Length; ++segment)
            {
                float first = F(frequencies[segment - 1] * half);
                float last = F(frequencies[segment] * half);
                float a0 = F(amplitudes[segment - 1] * gain);
                float a1 = F(amplitudes[segment] * gain);
                int start = (int)Math.Floor(first) + 1;
                int end = (int)Math.Floor(last);
                for (int index = start; index <= end; ++index)
                {
                    float position = F(F((float)index) - first);
                    float delta = F(a1 - a0);
                    float numerator = F(position * delta);
                    float denominator = F(last - first);
                    real[index] = F(F(numerator / denominator) + a0);
                }
            }
            // The gap remains +0. Original mirrors all high-frequency bins,
            // including Nyquist for factor 1.
            for (int index = Size - half; index < Size; ++index)
                real[index] = real[Size - index];
            ResamplerFft.Transform(real, imaginary, 13, 1);
            int support = 256 * factor;
            for (int index = 1; index <= support; ++index)
            {
                int windowIndex = index * (2 / factor) - 1;
                double weight = BitConverter.ToDouble(WindowBits, windowIndex * 8);
                real[index] = F((float)D((double)real[index] * weight));
                real[Size - index] = F((float)D((double)real[Size - index] * weight));
            }
            for (int index = support + 1; index < Size - support; ++index) real[index] = 0.0f;
            Array.Clear(imaginary, 0, Size);
            if (invert)
            {
                real[0] = F(gain - real[0]);
                for (int index = 1; index <= support; ++index)
                {
                    real[index] = F(NegativeZero - real[index]);
                    real[Size - index] = F(NegativeZero - real[Size - index]);
                }
            }
            ResamplerFft.Transform(real, imaginary, 13, 0);
            return real;
        }
    }
}
