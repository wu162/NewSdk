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

        // Original FCOS Hann half-window at support 512, FPCW=0x027f.
        // Weight[i-1] = ((FCOS(i*binary32(pi)/512)+1)*binary32(0.5)).
        // These binary64 values retain the original x87 53-bit intermediate;
        // factor 1 uses every second value. They must not become float values.
        private static readonly byte[] WindowBits = Convert.FromBase64String(
            "2zLHQuz/7z+0f00Lsf/vP/ICJVpO/+8/qD9BMMT+7z9GHfeOEv7vP07k/Hc5/e8/Fjpq7Tj87z+fG7jxEPvvP4LXwIfB+e8/3wbAskr47z9mhVJ2rPbvP2po" +
            "dtbm9O8/DPWK1/ny7z9wlVB+5fDvP/7M6M+p7u8/wSvW0Ubs7z/KQPyJvOnvP6iLn/4K5+8/9GxlNjLk7z/pFVQ4MuHvPxB30gsL3u8/+i2ouLza7z8Ucv1G" +
            "R9fvP3wAW7+q0+8//waqKufP7z8SDjSS/MvvP/Dhov/qx+8/unoAfbLD7z+447YUU7/vP6YhkNHMuu8/EBi2vh+27z/NbbLnS7HvP4ZwblhRrO8/TfcyHTCn" +
            "7z9URKhC6KHvP6zl1dV5nO8/JpUi5OSW7z86F1R7KZHvPxIZj6lHi+8/og1XfT+F7z/XCY4FEX/vP+CfdFG8eO8/i7mpcEFy7z+8cSpzoGvvP/bsUWnZZO8/" +
            "BTHZY+xd7z+z+9Zz2VbvP6eYv6qgT+8/TrZkGkJI7z/kOfXUvUDvP5oS/ewTOe8/0AtldUQx7z9rnnKBTynvP07BxyQ1Ie8/5bhic/UY7z/Q5Z2BkBDvP6qS" +
            "L2QGCO8/9sApMFf/7j8U9fn6gvbuP3ABadqJ7e4/utCa5Gvk7j9GLw4wKdvuP4qTnNPB0e4/xOV55jXI7j+uRjSAhb7uP2rVs7iwtO4/f3Q6qLeq7j8DjmNn" +
            "mqDuP+DWIw9Zlu4/QhHJuPOL7j8ozvl9aoHuPxwutXi9du4/CqFSw+xr7j9DpYF4+GDuP6aFSbPgVe4/7hYJj6VK7j8sdHYnRz/uP2K6npjFM+4/UsPl/iAo" +
            "7j9s3wV3WRzuP/OODx5vEO4/PjppEWIE7j806c5uMvjtP+r5UVTg6+0/dtZY4Gvf7T/rqZ4x1dLtP4IUM2ccxu0/AN95oEG57T82rSr9RKztP8ivUJ0mn+0/" +
            "G1VKoeaR7T9y+cgphYTtP0qW0FcCd+0/33C3TF5p7T/uxyUqmVvtP6WAFRKzTe0/1NLRJqw/7T9J9PaKhDHtP27DcWE8I+0/HXF/zdMU7T+pKa3ySgbtPyq9" +
            "1/Sh9+w//kYr+Njo7D+L1CIh8NnsPzkLiJTnyuw/r81yd7+77D9M4Ejvd6zsP+CMvSERnew/qkXRNIuN7D+QR9FO5n3sP547V5Yibuw/ytdIMkBe7D/7ftdJ" +
            "P07sP1PgfwQgPuw/wJUJiuIt7D/WwYYChx3sP/OsU5YNDew/pGEWbnb86z9dSL6ywevrP3bCg43v2us/esTnJwDK6z+3b7Or87jrPy6r90LKp+s/wrsMGISW" +
            "6z+825FVIYXrP6LRbCaic+s/ZIbJtQZi6z/MmhkvT1DrP1b8E757Pus/Tnm0jows6z9NVDvNgRrrPwbXLKZbCOs/dORQRhr26j9YirLavePqPxaSn5BG0eo/" +
            "7BColbS+6j+L950XCKzqPwKhlERBmeo/E2HgSmCG6j/gERZZZXPqP/ygCp5QYOo/4JvSSCJN6j/Au8GI2jnqP8Fwao15Juo/nGydhv8S6j+iLGmkbP/pPyqD" +
            "GRfB6+k/ZiA3D/3X6T+eGoe9IMTpP+R1ClMssOk/Iav9ACCc6T+fLtj4+4fpP/j1S2zAc+k/ef1EjW1f6T/0zOiNA0vpPwj8laCCNuk/1LXj9+oh6T8mPKHG" +
            "PA3pPx5q1T94+Og/RDa+lp3j6D8dNND+rM7oPy4Vtqumueg/iilQ0Yqk6D/L37OjWY/oP5BEK1cTeug/fIE0ILhk6D+tW4EzSE/oP7ux9sXDOeg/MfmrDCsk" +
            "6D+Su+o8fg7oP9gSLoy9+Oc/iCUiMOni5z86oqNeAc3nP7k6v00Gt+c/oh6xM/ig5z+YdeRG14rnP/nY8r2jdOc/KM2jz11e5z9jOuyyBUjnPyrl7Z6bMec/" +
            "OOb2yh8b5z8KIoFukgTnPwDAMcHz7eY/D6HY+kPX5j8O1m9Tg8DmP5QVGwOyqeY/djEnQtCS5j/eiwlJ3nvmP/qLX1DcZOY/TBLukMpN5j+X7KBDqTbmP3BJ" +
            "iqF4H+Y/aivi4zgI5j/k2wVE6vDlP4pdd/uM2eU/Yt7cQyHC5T+YKQBXp6rlP+MYzm4fk+U/mwVWxYl75T92OcmU5mPlP/Beehc2TOU/aPHch3g05T/mrIQg" +
            "rhzlP4/9JBzXBOU/0m6QtfPs5D9DGrgnBNXkPy8Wq60IveQ/3+OVggGl5D+Z3cHh7ozkP1mklAbRdOQ/Po2PLKhc5D+/Dk+PdETkP4wtimo2LOQ/PukR+u0T" +
            "5D++qNB5m/vjP2+mySU/4+M/GVwYOtnK4z+c7u/yabLjP2iZmozxmeM/tRl5Q3CB4z+TGQJU5mjjP66awfpTUOM/9GBYdLk34z/2XHv9Fh/jPyIW89JsBuM/" +
            "yRSbMbvt4j/0S2FWAtXiPw+DRX5CvOI/aL9Y5nuj4j9/rbzLroriPzQKo2vbceI/xQtNAwJZ4j+wygrQIkDiP2SqOg8+J+I/28FI/lMO4j8KRK7aZPXhPzDo" +
            "8OFw3OE/EVKiUXjD4T8Fel9ne6rhP/QU0GB6keE/Nfyle3V44T9XlZz1bF/hP8w5eAxhRuE/hJ4F/lEt4T9xOxkIQBThP/ayjmgr++A/SjlIXRTi4D/D+y0k" +
            "+8jgPxeILfvfr+A/lDM5IMOW4D9AgkfRpH3gP/mNUkyFZOA/jW1Xz2RL4D/Am1WYQzLgP1deTuUhGeA/MVqI6P//3z+aMXQGvM3fP31kZqB4m98/ZOJjMjZp" +
            "3z/XNm849TbfP1tXhy62BN8/g3GmkHnS3j8HucDaP6DeP+81w4gJbt4/yZKSFtc73j/76gkAqQnePyeZ+cB/190/rQUm1Vul3T9HdUa4PXPdP8zXA+YlQd0/" +
            "C5f32RQP3T/aZaoPC93cP0MPkwIJq9w/5EUVLg953D96c4ANHkfcP6CIDhw2Fdw/xMzi1Ffj2z9Trgizg7HbPx6TcjG6f9s//Kj4yvtN2z+2tlf6SBzbPyjt" +
            "Lzqi6to/tbgDBQi52j/+kjbVeofaP+bUCyX7Vdo/6Iilbokk2j+7PQMsJvPZP1HZANfRwdk/KGxV6YyQ2T/2BJLcV1/ZP7yEICozLtk/KnNCSx/92D940w+5" +
            "HMzYP5b5dewrm9g/0V82Xk1q2D/afOWGgTnYP0ea6d7ICNg/fat53iPY1z8SJZz9kqfXP6nUJbQWd9c/Q7m4ea9G1z8U3MLFXRbXP9EpfQ8i5tY/ikzqzfy1" +
            "1j8AhtV37oXWP4uK0YP3VdY/h1w3aBgm1j9UKCWbUfbVP+QgfZKjxtU/2lzkww6X1T9ItMGkk2fVP/6ePKoyONU/fxM8SewI1T+HZmX2wNnUP0IrGyaxqtQ/" +
            "IBR8TL171D9N1GHd5UzUP94BYEwrHtQ/o/jCDI7v0z+mvY6RDsHTP2fjfU2tktM/xG4As2pk0z+evDo0RzbTPzZoBENDCNM/RDLnUF/a0j/d6B3Pm6zSPwZQ" +
            "ky75ftI/HQvh33dR0j8Gh05TGCTSPxzlz/ja9tE//uYEQMDJ0T8b2zeYyJzRPxyKXHD0b9E/IiUPN0RD0T/bNJNauBbRP3CJ0khR6tA/UCtcbw++0D/kTGM7" +
            "85HQPxU9vhn9ZdA/ylrldi060D8+CfK+hA7QP5JKO7sGxs8/JveAfFNvzz94f6GX8BjPP9AIw+Hews4/pGhDLx9tzj+SGLZTshfOP1Qs4iGZws0/sEnAa9Rt" +
            "zT9mongCZRnNPxbwYLZLxcw/QHL6VolxzD827u+yHh7MPzqxE5gMy8s/pJRd01N4yz8iBOkw9SXLPx4G83vx08o/REbYfkmCyj80IhMD/jDKP2a4OdEP4Mk/" +
            "NPn7sH+PyT8uuiFpTj/JP5zLiL9878g/ThAjeQugyD+gl/RZ+1DIP965ESVNAsg/6DadnAG0xz80V8aBGWbHPyIPx5SVGMc/riTilHbLxj+IV2FAvX7GP4yL" +
            "k1RqMsY/tPXKjX7mxT9eS1un+prFPyb0l1vfT8U/HD7SYy0FxT+QlFd45brEP0a5b1AIccQ/RgBbopYnxD8ojlAjkd7DP/iYfIf4lcM/nqv+gc1Nwz/y6+fE" +
            "EAbDP1JjOQHDvsI/8kni5uR3wj+6VL4kdzHCP9wFlGh668E/AgATX++lwT9EXNKz1mDBP8QCTxExHME//gXqIP/XwD/mAOeKQZTAP7J3avb4UMA/eDt4CSYO" +
            "wD8YoePRkpe/P1SvKXHHE78/8PPyNeuQvj+kryFj/w6+P+z0RjkFjr0/FJGf9v0NvT/w+hDX6o68P4RHJhTNELw/RCQN5aWTuz9k15J+dhe7P9hFIRNAnLo/" +
            "TP+70gMiuj8EUP3qwqi5P7hYE4d+MLk/WCy9zze5uD/Y80fr70K4PxQYjP2nzbc/uHHqJ2FZtz8of0mJHOa2P7ygEj7bc7Y/2FovYJ4Ctj98ngYHZ5K1P8AX" +
            "ekc2I7U/uILjMw21tD+ABhLc7Ee0P5SWR03W27M/eFo2kspwsz+kG/6yygazP9C5KbXXnbI/qKWsm/I1sj/kYeBmHM+xP6gKghRWabE/jOOvn6AEsT8A7OYA" +
            "/aCwPxh6AC5sPrA/ALhfNN65rz/g//9rDfmuPzBFot5nOq4/gAitYu99rT/QSCjJpcOsPzgIud2MC6w/sN2cZqZVqz9glKUk9KGqPzDYNNN38Kk/wO83KDNB" +
            "qT/QgyPUJ5SoP2h074FX6ac/ILsS18NApz9QW39zbpqmP9BfnvFY9qU/OOZL5oRUpT/4N9Pg87SkPyDx6mqnF6Q/8DSxCKF8oz848Kc44uOiP5ApsXNsTaI/" +
            "SF8LLUG5oT+g803SYSehP5CmZcvPl6A/wB2ReowKoD8A9bp4Mv+eP3D7Rc/u7Z0/AHQDm1DhnD+wp7xyWtmbP6CfveEO1po/oN7OZ3DXmT8gNi95gd2YPxC4" +
            "jX5E6Jc/sMUD1bv3lj8AOg/O6QuWP/CxjK/QJJU/EPCxs3JClD9QXggJ0mSTPzCrZ9Lwi5I/IIXwJtG3kT+QcQcSdeiQP/DBT5PeHZA/AEpNPR+wjj9gqzw4" +
            "FC6NP0DN8s+ftYs/YFZMpcVGij8Ae3RBieGIP0BD3BXuhYc/wAwyfPczhj/gRVm2qOuEP6BkYu4ErYM/oBeDNg94gj9gsg6JykyBPyDUbsg5K4A/gJQ4fr8m" +
            "fj+AWTA/fgp8P8Buygi1AXo/QLgC4GgMeD8AJr+Znip2PwDGw9paXHQ/wFCnF6Khcj/AKciUePpwP4Cyg8zEzW4/gPbF38bNaz+ATUfK/vRoP4CYOJJzQ2Y/" +
            "ABX63Cu5Yz8A+QrvLVZhPwD181f/NF4/AJKtLE0MWj8A6UifTzJWPwCImTAQp1I/APRRPi/VTj8AZkLO2/lIPwC6cYQ0vEM/AAiGnow4Pj8AbN4vOTQ2PwDI" +
            "owkF1y4/AJgQtfK8Iz8AMAuNrTQWPwBA0DEavQM/AAB78BO94z4AAAAAAADhPA==");

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
