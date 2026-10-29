using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Recovered.Compiler.Texture;

internal static class TexturePpmReplay
{
    static int checks, fixtures, decoded, rejected, boundaries, differences;
    static long compared;
    static void Check(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static string Hash(byte[] b) { using (var s = SHA256.Create()) return BitConverter.ToString(s.ComputeHash(b)).Replace("-", "").ToLowerInvariant(); }
    static void Isolated()
    {
        Check(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BinaryAssetBuilder.TextureCompiler.dll")), "Original file present");
        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) Check(a.GetName().Name != "BinaryAssetBuilder.TextureCompiler", "Original assembly loaded");
    }
    static string Error(Exception e)
    {
        if (e == null) return "";
        var code = e.GetType().GetProperty("ErrorCode");
        return (e.GetType().FullName + "/" + (code == null ? "" : Convert.ToString(code.GetValue(e, null))) + "/" + e.Message).Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');
    }
    static void Case(string directory, string[] row, bool file, bool dispatch)
    {
        string path = Path.Combine(directory, row[0] + ".ppm"); byte[] source = File.ReadAllBytes(path);
        Check(Hash(source) == row[8], "Immutable source " + row[0]);
        TexturePacker.Image image = null; Exception error = null;
        Isolated(); try { image = dispatch ? (file ? TextureImageLoader.Load(path) : TextureImageLoader.Decode(source)) : (file ? TexturePpmDecoder.Load(path) : TexturePpmDecoder.Decode(source)); } catch (Exception e) { error = e; } Isolated();
        Check(Hash(source) == row[8] && Hash(File.ReadAllBytes(path)) == row[8], "Source changed " + row[0]);
        bool equal = Error(error) == row[7];
        if (row[7].Length != 0) { equal = equal && image == null; rejected++; }
        else
        {
            byte[] expected = File.ReadAllBytes(Path.Combine(directory, row[0] + ".rgba"));
            Check(Hash(expected) == row[6] && expected.Length == int.Parse(row[5]), "Immutable RGBA " + row[0]);
            equal = equal && image != null && image.Width == int.Parse(row[2]) && image.Height == int.Parse(row[3]) && image.HasAlpha == bool.Parse(row[4]) && image.Pixels.Length * 4 == expected.Length;
            if (equal) for (int i = 0; i < image.Pixels.Length; i++) if (image.Pixels[i] != BitConverter.ToUInt32(expected, i * 4)) { equal = false; break; }
            compared += expected.Length; decoded++;
        }
        if (!equal) { differences++; if (differences <= 24) Console.WriteLine("DIFF " + row[0] + " file=" + file + " public=" + dispatch + " expectedError=" + row[7] + " actualError=" + Error(error)); }
    }
    static void Boundaries()
    {
        // These potentially unsafe native domains are tested only against our
        // bounded candidate, never sent to the original DLL as gold.
        var sources = new List<byte[]> { new byte[0], new byte[] { 80 }, new byte[] { 80, 54 } };
        foreach (string text in new[] { "P6\n4 4\n255\n", "P3\n0 4\n255\n", "P3\n4 0\n255\n", "P3\n4 4\n0\n", "P3\n4294967295 2\n255\n", "P3\n8192 8192\n255\n", "P3\n1 1\n65536\n0 0 0\n", "P3\n1 1\n255\n0 0 x\n" }) sources.Add(Encoding.ASCII.GetBytes(text));
        byte[] header = Encoding.ASCII.GetBytes("P6\n4 4\n255\n");
        for (int remaining = 0; remaining < 48; remaining++) { byte[] input = new byte[header.Length + remaining]; Buffer.BlockCopy(header, 0, input, 0, header.Length); sources.Add(input); }
        foreach (byte[] source in sources)
        {
            string before = Hash(source); TexturePacker.Image image = null; Exception error = null;
            Isolated(); try { image = TexturePpmDecoder.Decode(source); } catch (Exception e) { error = e; } Isolated();
            Check(image == null && error is BinaryAssetBuilder.BinaryAssetBuilderException, "Bounded decoder refusal " + source.Length + ": " + error);
            Check(Hash(source) == before, "Malformed input changed"); boundaries++;
        }
        Exception nullError = null; Isolated(); try { TexturePpmDecoder.Decode(null); } catch (Exception e) { nullError = e; } Isolated();
        Check(nullError is ArgumentNullException, "Null source contract"); boundaries++;
        Check(boundaries == 60, "Complete malformed binary/overflow boundary matrix");
    }
    static int Main(string[] args)
    {
        try
        {
            Check(IntPtr.Size == 4, "x86 required"); string directory = Path.GetFullPath(args[0]);
            string[] lines = File.ReadAllLines(Path.Combine(directory, "transcript.tsv"));
            Check(lines.Length == 8439, "Complete immutable PPM corpus"); var labels = new HashSet<string>();
            for (int i = 1; i < lines.Length; i++)
            {
                string[] row = lines[i].Split('\t'); Check(row.Length == 15 && labels.Add(row[0]), "Unique complete row");
                Check(row[12] == "True" && row[6] == row[13] && row[7] == row[14], "Stable original required");
                foreach (bool file in new[] { false, true }) foreach (bool dispatch in new[] { false, true }) Case(directory, row, file, dispatch); fixtures++;
            }
            Boundaries(); Check(fixtures == 8438 && decoded + rejected == fixtures * 4, "Complete dual-entry accounting");
            Check(differences == 0, "Full original PPM differences=" + differences);
            Console.WriteLine("PASS public PPM NoOriginal fixtures=" + fixtures + " calls=" + fixtures * 4 + " decoded=" + decoded + " rejected=" + rejected + " boundaries=" + boundaries + " checks=" + checks + " comparedBytes=" + compared + "; complete original bytes/errors, absent/unloaded original per call, immutable inputs; plugin/SDK acceptance separate");
            return 0;
        }
        catch (Exception e) { Console.Error.WriteLine(e); return 1; }
    }
}
