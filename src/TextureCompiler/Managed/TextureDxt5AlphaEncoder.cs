using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BinaryAssetBuilder.TextureCompiler
{
    // PC production DXT5 alpha search, reconstructed from original native RVAs
    // 0x7D500/0x7E8F0. Colour compression and the plugin DXT route remain separate.
    public static class TextureDxt5AlphaEncoder
    {
        private sealed class Fit
        {
            public double Low, High, ErrorNumerator, ErrorDenominator;
        }

        private static Fit Solve(int levels, int count, double meanAlpha,
            double meanIndex, double variance, double covariance, bool trial)
        {
            double n = count, steps = levels - 1;
            double left = -meanIndex, right = steps - meanIndex;
            // The original local-move solver uses a different multiplication order
            // from the main fit; preserving it matters for strict error ties.
            double a = trial ? left * n * left + variance : left * left * n + variance;
            double b = trial ? left * n * right + variance : right * left * n + variance;
            double c = right * n * right + variance;
            double ac = c * a;
            double scale = ac * variance * ac * steps * steps;
            double lowerLeft = -(variance * meanAlpha) - left * covariance;
            double upperLeft = (1.0 - meanAlpha) * variance - left * covariance;
            double lowerRight = (-(variance * meanAlpha) - right * covariance) * a;
            double upperRight = ((1.0 - meanAlpha) * variance - right * covariance) * a;
            double t = ((upperLeft - Math.Abs(upperLeft)) + Math.Abs(lowerLeft) + lowerLeft) * 0.5 * b;
            t = (Math.Abs(lowerRight - t) + lowerRight + (upperRight - Math.Abs(upperRight - t))) * 0.5;
            double highScaled = t * c;
            double lowScaled = (Math.Abs(lowerLeft * ac - b * t) + lowerLeft * ac +
                (upperLeft * ac - Math.Abs(upperLeft * ac - b * t))) * 0.5;
            double residual = highScaled * left - lowScaled * right;
            return new Fit {
                Low = (lowScaled / ac + left * covariance) / variance,
                High = (highScaled / ac + right * covariance) / variance,
                ErrorNumerator = ((lowScaled - highScaled) * variance * (lowScaled - highScaled) +
                    residual * n * residual) - covariance * covariance * scale,
                ErrorDenominator = scale * variance
            };
        }

        [DllImport("Recovered.TextureDxtAlpha.dll", CallingConvention = CallingConvention.StdCall,
            EntryPoint = "TextureQuantizeAlpha")]
        private static extern double NativeQuantize(int levels, int count,
            [In] double[] values, [In, Out] int[] endpoints, [In, Out] int[] indices);

        public static double Quantize(int levels, double[] values, int[] endpoints, int[] indices)
        {
            if (levels != 6 && levels != 8) throw new ArgumentOutOfRangeException("levels");
            if (values == null) throw new ArgumentNullException("values");
            if (values.Length > 512) throw new ArgumentException("At most 512 alpha values are supported.", "values");
            if (endpoints == null || endpoints.Length != 2) throw new ArgumentException("Two endpoints are required.", "endpoints");
            if (indices == null || indices.Length != values.Length) throw new ArgumentException("One index per alpha is required.", "indices");
            for (int i = 0; i < values.Length; i++)
                if (Double.IsNaN(values[i]) || values[i] < 0 || values[i] > 1)
                    throw new ArgumentException("Finite alpha values in [0,1] are required.", "values");
            if (values.Length == 0) return 0;
            Array.Clear(indices, 0, indices.Length);
            return NativeQuantize(levels, values.Length, values, endpoints, indices);
        }

        // Readable algorithm reference only. Not used by the production path:
        // managed rounding currently differs at the symmetric five-point tie.
        // Returns the native error in byte-alpha units, not normalized units.
        // Endpoints and indices follow QuantizeAlpha's internal ordering.
        private static double QuantizeManagedReference(int levels, double[] values, int[] endpoints, int[] indices)
        {
            if (levels != 6 && levels != 8) throw new ArgumentOutOfRangeException("levels");
            if (values == null) throw new ArgumentNullException("values");
            if (values.Length > 512) throw new ArgumentException("At most 512 alpha values are supported.", "values");
            if (endpoints == null || endpoints.Length != 2) throw new ArgumentException("Two endpoints are required.", "endpoints");
            if (indices == null || indices.Length != values.Length) throw new ArgumentException("One index per alpha is required.", "indices");
            for (int i = 0; i < values.Length; i++)
                if (Double.IsNaN(values[i]) || values[i] < 0 || values[i] > 1)
                    throw new ArgumentException("Finite alpha values in [0,1] are required.", "values");
            int count = values.Length;
            if (count == 0) return 0;
            double min = values[0], max = values[0], mean = values[0];
            for (int i = 1; i < count; i++) {
                min = Math.Min(min, values[i]); max = Math.Max(max, values[i]); mean += values[i];
            }
            double invCount = 1.0 / count;
            mean *= invCount;
            double[] centered = new double[count];
            for (int i = 0; i < count; i++) centered[i] = values[i] - mean;
            double low = min - mean, high = max - mean;
            int steps = levels - 1, remaining = 64;
            int[] populations = new int[levels];
            // The native caller zero-initializes these arrays before searching.
            Array.Clear(indices, 0, indices.Length);
            bool done = false;
            while (!done) {
                double[] thresholds = new double[steps];
                for (int i = 0; i < steps; i++) thresholds[i] = (i + 0.5) * ((high - low) / steps) + low;
                Array.Clear(populations, 0, populations.Length);
                bool unchanged = true;
                for (int i = 0; i < count; i++) {
                    int index = 0;
                    while (index < steps && thresholds[index] <= centered[i]) index++;
                    populations[index]++;
                    unchanged &= indices[i] == index;
                    indices[i] = index;
                }
                int occupied = 0;
                for (int i = 0; i < levels; i++) if (populations[i] != 0) occupied++;
                if (occupied == 1) break;
                double covariance = 0, sumIndex = 0, sumSquare = 0;
                int pos = 0;
                // Preserve the original four-at-a-time accumulation order.
                for (; pos < count - 3; pos += 4) {
                    double i0 = indices[pos], i1 = indices[pos+1], i2 = indices[pos+2], i3 = indices[pos+3];
                    covariance = i2 * centered[pos+2] + centered[pos+1] * i1 + centered[pos] * i0 + covariance + centered[pos+3] * i3;
                    sumIndex = i0 + sumIndex + i1 + i2 + i3;
                    sumSquare = i2 * i2 + i1 * i1 + i0 * i0 + sumSquare + i3 * i3;
                }
                for (; pos < count; pos++) {
                    double index = indices[pos];
                    covariance = centered[pos] * index + covariance;
                    sumSquare = index * index + sumSquare;
                    sumIndex = index + sumIndex;
                }
                double meanIndex = invCount * sumIndex;
                double variance = sumSquare - count * meanIndex * meanIndex;
                Fit best = Solve(levels, count, mean, meanIndex, variance, covariance, false);
                low = best.Low; high = best.High;
                if (remaining == 0) break;
                remaining--;
                if (!unchanged) continue;
                int[] first = new int[levels], last = new int[levels];
                for (int i = 0; i < levels; i++) first[i] = -1;
                for (int i = 0; i < count; i++) {
                    int bin = indices[i];
                    if (first[bin] == -1) { first[bin] = last[bin] = i; }
                    else {
                        if (centered[i] < centered[first[bin]]) first[bin] = i;
                        if (centered[last[bin]] < centered[i]) last[bin] = i;
                    }
                }
                done = true;
                for (int direction = 0; direction < 2 && done; direction++) {
                    double delta = direction == 0 ? 1.0 : -1.0;
                    double movedMean = invCount * delta + meanIndex;
                    double twice = delta + delta;
                    double correction = ((variance + 1.0) - twice * meanIndex) - invCount;
                    for (int bin = direction; bin < steps + direction; bin++) {
                        if (populations[bin] == 0 || (populations[bin] == 1 && occupied <= 2)) continue;
                        int extreme = direction == 0 ? last[bin] : first[bin];
                        double movedCovariance = centered[extreme] * delta + covariance;
                        double movedVariance = twice * bin + correction;
                        Fit trial = Solve(levels, count, mean, movedMean, movedVariance, movedCovariance, true);
                        if (trial.ErrorNumerator * best.ErrorDenominator < trial.ErrorDenominator * best.ErrorNumerator) {
                            low = trial.Low; high = trial.High; done = false; break;
                        }
                    }
                }
            }
            int[] palette = new int[levels];
            palette[0] = Math.Max(0, Math.Min(255, (int)Math.Floor((low + mean) * 255.0 + 0.5)));
            palette[1] = Math.Max(0, Math.Min(255, (int)Math.Floor((high + mean) * 255.0 + 0.5)));
            for (int i = 2; i < levels; i++)
                palette[i] = (palette[0] * (levels-i) + (i-1) * palette[1] + (levels-2)/2) / steps;
            endpoints[0] = palette[0]; endpoints[1] = palette[1];
            double error = 0;
            for (int i = 0; i < count; i++) {
                double alpha = values[i] * 255.0;
                int nearest = 0;
                for (int j = 1; j < levels; j++)
                    if (Math.Abs(palette[j] - alpha) < Math.Abs(palette[nearest] - alpha)) nearest = j;
                indices[i] = nearest;
                double diff = palette[nearest] - alpha;
                error = diff * diff + error;
            }
            return error;
        }

        public static byte[] EncodeBlock(byte[] rgba)
        {
            if (rgba == null) throw new ArgumentNullException("rgba");
            if (rgba.Length != 64) throw new ArgumentException("A 4x4 RGBA block is required.", "rgba");
            float[] alpha = new float[16];
            for (int i = 0; i < 16; i++) alpha[i] = (float)(rgba[i*4+3] / 255.0);
            return EncodeAlphaBlock(alpha);
        }

        // Preserve filtered/mip alpha floats without a byte round-trip.
        public static byte[] EncodeAlphaBlock(float[] values)
        {
            if (values == null) throw new ArgumentNullException("values");
            if (values.Length != 16) throw new ArgumentException("A 4x4 alpha block is required.", "values");
            double[] alpha = new double[16];
            for (int i = 0; i < 16; i++) {
                if (Single.IsNaN(values[i]) || values[i] < 0 || values[i] > 1)
                    throw new ArgumentException("Finite normalized alpha values are required.", "values");
                alpha[i] = values[i];
            }
            int[] levels = {8,6,6,6,6};
            bool[] excludeZero = {false,false,true,false,true};
            bool[] excludeOne = {false,false,false,true,true};
            double bestError = Double.MaxValue;
            int best = -1;
            int[] chosenEndpoints = null, chosenIndices = null;
            for (int mode = 0; mode < 5; mode++) {
                List<double> filtered = new List<double>();
                foreach (double value in alpha)
                    if (!(value == 0 && excludeZero[mode]) && !(value == 1 && excludeOne[mode])) filtered.Add(value);
                if (mode > 1 && filtered.Count == alpha.Length) continue;
                int[] endpoints = new int[2], indices = new int[filtered.Count];
                double error = Quantize(levels[mode], filtered.ToArray(), endpoints, indices);
                if (best == -1 || error < bestError) {
                    best = mode; bestError = error; chosenEndpoints = endpoints; chosenIndices = indices;
                }
            }
            int low = chosenEndpoints[0], high = chosenEndpoints[1], numLevels = levels[best];
            int[] outputIndices = new int[16];
            int position = 0;
            for (int i = 0; i < 16; i++) {
                if (alpha[i] == 0 && excludeZero[best]) outputIndices[i] = 6;
                else if (alpha[i] == 1 && excludeOne[best]) outputIndices[i] = 7;
                else outputIndices[i] = chosenIndices[position++];
            }
            if (low == high) {
                high++; numLevels = 6;
                Array.Clear(outputIndices, 0, outputIndices.Length);
            }
            bool swap = (low > high) == (numLevels == 6);
            if (swap) { int temp = low; low = high; high = temp; }
            byte[] result = new byte[8];
            result[0] = (byte)Math.Max(0, Math.Min(255, low));
            result[1] = (byte)Math.Max(0, Math.Min(255, high));
            ulong bits = 0;
            for (int i = 0; i < 16; i++) {
                int index = outputIndices[i];
                if (swap) {
                    if (numLevels == 8) index = index > 1 ? 9-index : 1-index;
                    else if (index < 6) index = index > 1 ? 7-index : 1-index;
                }
                bits |= (ulong)index << (i*3);
            }
            for (int i = 0; i < 6; i++) result[i+2] = (byte)(bits >> (i*8));
            return result;
        }
    }
}
