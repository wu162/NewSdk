using System;
using System.IO;

internal static class TexturePpmSdkFixtures
{
    static int Main(string[] args)
    {
        try
        {
            string output = Path.GetFullPath(args[0]); Directory.CreateDirectory(output);
            int raw = 0, atlas = 0;
            foreach (int[] size in new[] { new[] { 8, 4 }, new[] { 7, 5 }, new[] { 1, 1 }, new[] { 16, 16 } })
            foreach (int max in new[] { 1, 7, 255, 65535 }) foreach (int format in new[] { 3, 6 })
            foreach (int profile in new[] { 0, 1, 2, 3, 4, 8, 9 }) foreach (int pattern in new[] { 0, 4 })
            {
                // Original errors stay in the component matrix. All selected
                // rounded strips have width a direct multiple of height.
                if (format == 6 && max > 255) continue;
                string label = "raw-p" + format + "-" + size[0] + "x" + size[1] + "-m" + max + "-h" + profile + "-v" + pattern + ".ppm";
                File.WriteAllBytes(Path.Combine(output, label), TexturePpmFixtures.Create(size[0], size[1], max, format, pattern, profile)); raw++;
            }
            foreach (int max in new[] { 7, 255 }) foreach (int format in new[] { 3, 6 })
            foreach (int profile in new[] { 0, 4 }) foreach (int pattern in new[] { 0, 4 })
            {
                string label = "atlas-p" + format + "-m" + max + "-h" + profile + "-v" + pattern + ".ppm";
                File.WriteAllBytes(Path.Combine(output, label), TexturePpmFixtures.Create(256, 256, max, format, pattern, profile)); atlas++;
            }
            if (raw != 392 || atlas != 16) throw new Exception("Complete positive PPM SDK input set");
            Console.WriteLine("PASS PPM SDK fixture generation raw=" + raw + " atlas=" + atlas); return 0;
        }
        catch (Exception e) { Console.Error.WriteLine(e); return 1; }
    }
}
