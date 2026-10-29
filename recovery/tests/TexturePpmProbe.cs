using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Recovered.Compiler.Texture;
using Recovered.Compiler.Texture.Candidates;

// Capture only. Differences remain evidence; a completed probe is not acceptance.
internal static class TexturePpmProbe
{
    static int count, decoded, rejected, matches, differences, unstable, checks;
    static void Check(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static string Hash(byte[] data) { if (data == null) return ""; using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(data)).Replace("-", "").ToLowerInvariant(); }
    internal static string Error(Exception error)
    {
        if (error == null) return "";
        while (error is TargetInvocationException && error.InnerException != null) error = error.InnerException;
        var code = error.GetType().GetProperty("ErrorCode");
        return (error.GetType().FullName + "/" + (code == null ? "" : Convert.ToString(code.GetValue(error, null))) + "/" + error.Message).Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');
    }
    static byte[] Bytes(TexturePacker.Image image) { if (image == null) return null; byte[] b = new byte[image.Pixels.Length * 4]; Buffer.BlockCopy(image.Pixels, 0, b, 0, b.Length); return b; }
    static bool Same(TexturePacker.Image a, Exception ae, TexturePacker.Image b, Exception be)
    {
        if (Error(ae) != Error(be) || (a == null) != (b == null)) return false;
        if (a == null) return true;
        if (a.Width != b.Width || a.Height != b.Height || a.HasAlpha != b.HasAlpha || a.Pixels.Length != b.Pixels.Length) return false;
        for (int i = 0; i < a.Pixels.Length; i++) if (a.Pixels[i] != b.Pixels[i]) return false;
        return true;
    }
    static void Case(MethodInfo load, StreamWriter log, string output, TexturePpmFixtures.Fixture fixture)
    {
        string path = Path.Combine(output, fixture.Name + ".ppm"); File.WriteAllBytes(path, fixture.Data);
        TexturePacker.Image original = null, repeat = null, candidate = null, direct = null;
        Exception error = null, repeatError = null, candidateError = null, directError = null;
        try { original = (TexturePacker.Image)load.Invoke(null, new object[] { path }); } catch (Exception e) { error = e; }
        try { repeat = (TexturePacker.Image)load.Invoke(null, new object[] { path }); } catch (Exception e) { repeatError = e; }
        try { candidate = TexturePpmDecoder.Load(path); } catch (Exception e) { candidateError = e; }
        try { direct = TexturePpmDecoder.Decode(fixture.Data); } catch (Exception e) { directError = e; }
        Check(Same(candidate, candidateError, direct, directError), "Candidate Decode/Load disagree " + fixture.Name);
        bool stable = Same(original, error, repeat, repeatError), equal = Same(original, error, candidate, candidateError);
        if (!stable) unstable++;
        byte[] rgba = Bytes(original), repeated = Bytes(repeat), actual = Bytes(candidate);
        if (rgba != null) { File.WriteAllBytes(Path.Combine(output, fixture.Name + ".rgba"), rgba); decoded++; } else { Check(error != null, "Missing original outcome"); rejected++; }
        if (repeated != null) File.WriteAllBytes(Path.Combine(output, fixture.Name + ".repeat.rgba"), repeated);
        if (actual != null) File.WriteAllBytes(Path.Combine(output, fixture.Name + ".candidate.rgba"), actual);
        if (equal) matches++; else differences++;
        Check(Hash(fixture.Data) == Hash(File.ReadAllBytes(path)), "Source changed " + fixture.Name);
        log.WriteLine(fixture.Name + "\t" + fixture.Family + "\t" + (original == null ? 0 : original.Width) + "\t" + (original == null ? 0 : original.Height) + "\t" + (original != null && original.HasAlpha) + "\t" + (rgba == null ? 0 : rgba.Length) + "\t" + Hash(rgba) + "\t" + Error(error) + "\t" + Hash(fixture.Data) + "\t" + equal + "\t" + Error(candidateError) + "\t" + Hash(actual) + "\t" + stable + "\t" + Hash(repeated) + "\t" + Error(repeatError));
        count++;
    }
    static int Main(string[] args)
    {
        try
        {
            Check(IntPtr.Size == 4, "x86 required");
            string input = Path.GetFullPath(args[0]), output = Path.GetFullPath(args[1]), directory = Path.GetDirectoryName(input);
            Directory.CreateDirectory(output);
            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e) { string p = Path.Combine(directory, new AssemblyName(e.Name).Name + ".dll"); return File.Exists(p) ? Assembly.LoadFrom(p) : null; };
            typeof(TexturePngOracle).GetMethod("Bind", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { Assembly.LoadFrom(input), input });
            var load = typeof(TexturePngOracle).GetMethod("Original", BindingFlags.Static | BindingFlags.NonPublic);
            using (var log = new StreamWriter(Path.Combine(output, "transcript.tsv")))
            {
                log.AutoFlush = true;
                log.WriteLine("label\tfamily\twidth\theight\thasAlpha\tbytes\tsha256\terror\tinputSha256\tcandidateEqual\tcandidateError\tcandidateSha256\toriginalStable\trepeatSha256\trepeatError");
                foreach (var fixture in TexturePpmFixtures.All()) Case(load, log, output, fixture);
            }
            Check(count == TexturePpmFixtures.Count && decoded + rejected == count && matches + differences == count, "Complete PPM corpus accounting");
            Console.WriteLine("PROBE Texture PPM fixtures=" + count + " decoded=" + decoded + " rejected=" + rejected + " candidateMatches=" + matches + " differences=" + differences + " unstableOriginals=" + unstable + " originalCalls=" + count * 2 + " checks=" + checks + "; full source/repeated original/candidate RGBA and errors; not production or SDK acceptance");
            return 0;
        }
        catch (Exception e) { Console.Error.WriteLine(e); return 1; }
        finally { var owner = typeof(TexturePngOracle).GetField("owner", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as IDisposable; if (owner != null) owner.Dispose(); }
    }
}
