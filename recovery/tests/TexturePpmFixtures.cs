using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

internal static class TexturePpmFixtures
{
    internal sealed class Fixture
    {
        internal string Name, Family;
        internal byte[] Data;
        internal Fixture(string name, string family, byte[] data) { Name = name; Family = family; Data = data; }
    }
    internal static readonly int[][] Sizes = { new[] { 1, 1 }, new[] { 2, 2 }, new[] { 8, 4 },
        new[] { 16, 16 }, new[] { 3, 2 }, new[] { 7, 5 }, new[] { 17, 9 } };
    internal const int Count = 8438;
    static int Sample(int x, int y, int c, int max, int pattern)
    {
        if (pattern == 1) return 0;
        if (pattern == 2) return max;
        if (pattern == 3) return ((x + y + c) & 1) * max;
        if (pattern == 4) return c == 0 ? max : c == 1 ? max / 3 : max / 7;
        return (x * 1271 + y * 395 + c * 739 + 17) % (max + 1);
    }
    internal static byte[] Create(int width, int height, int max, int format, int pattern, int profile)
    {
        string whitespace = profile == 1 ? "\r\n" : profile == 2 ? "\t" : profile == 3 ? "\v\f" : "\n";
        string first = profile == 6 ? "" : whitespace;
        string w = profile == 7 ? width.ToString("D5") : width.ToString();
        string h = profile == 7 ? height.ToString("D5") : height.ToString();
        string m = profile == 7 ? max.ToString("D5") : max.ToString();
        string between = profile == 4 ? "\n# bounded comment\r\n" : whitespace;
        string header = "P" + format + first + w + (profile == 5 ? "#inline\n" : between) + h + between + m + whitespace;
        if (profile == 8 && format == 6) header += " ";
        if (profile == 9 && format == 6) header += "# binary bytes are raster, not a comment\n";
        using (var stream = new MemoryStream())
        {
            byte[] head = Encoding.ASCII.GetBytes(header); stream.Write(head, 0, head.Length);
            for (int y = 0; y < height; y++) for (int x = 0; x < width; x++) for (int c = 0; c < 3; c++)
            {
                int value = Sample(x, y, c, max, pattern);
                if (format == 6 || format == 5) stream.WriteByte((byte)value);
                else
                {
                    string delimiter = profile == 8 && (x + y + c) % 3 == 0 ? "\n# sample comment\n" : whitespace;
                    byte[] token = Encoding.ASCII.GetBytes(value + delimiter); stream.Write(token, 0, token.Length);
                }
            }
            // Every original binary call has a complete raster even when the
            // separator profile shifts its first byte. Never probe truncation.
            byte[] tail = Encoding.ASCII.GetBytes(pattern % 3 == 0 ? "\nP3\n1 1\n255\n1 2 3\n" : " \ntrailing ignored data\n");
            stream.Write(tail, 0, tail.Length);
            return stream.ToArray();
        }
    }
    internal static IEnumerable<Fixture> All()
    {
        foreach (int[] size in Sizes) foreach (int max in new[] { 1, 2, 3, 7, 15, 100, 127, 254, 255, 256, 1023, 65535 })
        foreach (int format in new[] { 3, 6 }) foreach (int pattern in new[] { 0, 1, 2, 3, 4 })
        for (int profile = 0; profile < 10; profile++)
            yield return new Fixture("p" + format + "-" + size[0] + "x" + size[1] + "-m" + max + "-v" + pattern + "-h" + profile,
                "syntax-scale", Create(size[0], size[1], max, format, pattern, profile));
        foreach (int[] size in Sizes) foreach (int format in new[] { 2, 5 }) foreach (int pattern in new[] { 0, 1 })
            yield return new Fixture("pgm-p" + format + "-" + size[0] + "x" + size[1] + "-v" + pattern,
                "pgm-rejection", Create(size[0], size[1], 255, format, pattern, 0));
        string[] malformed = {
            "P3\n0 2\n255\n0 0 0\n", "P3\n2 0\n255\n0 0 0\n", "P3\n2 2\n0\n0 0 0\n",
            "P3\n-1 2\n255\n0 0 0\n", "P3\n2 -2\n255\n0 0 0\n", "P3\n2 2\n-1\n0 0 0\n",
            "P3\n1 1\n255\nx 0 0\n", "P3\n1 1\n255\n0 x 0\n", "P3\n1 1\n255\n0 0 x\n",
            "P6\n1 1\n256\n0 0 0\n"
        };
        for (int i = 0; i < malformed.Length; i++)
            yield return new Fixture("invalid-" + i, "bounded-rejection", Encoding.ASCII.GetBytes(malformed[i] + new string(' ', 32)));
    }
}
