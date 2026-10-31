using System.Security.Cryptography;
using BinaryAssetBuilder.UiCompiler.Swf;

internal static class AptOracle
{
    // Golden digests from unchanged attachment-matched assemblies, run under
    // .NET 8 x86: feature/swf-direct-compiler, ui/evidence/source-consistency.json,
    // pinned at 148681ad7bfecf571b2ba5c49c000594539bc17a, run 40288873546.
    internal static void Verify(Action<bool, string> check)
    {
        string fixture = Path.Combine(AppContext.BaseDirectory, "fixtures", "CoronaUIMain.swf"); byte[] compressed = File.ReadAllBytes(fixture);
        string Hash(byte[] bytes) { using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant(); }
        check(Hash(compressed) == "b661542cf2c8b52ef846c348f817f523f5fba63ee4baa53c365aabde30af9fa7", "original-tool golden fixture identity");
        var body = SwfLib.SwfZip.DecompressZlib(compressed.Skip(8).ToArray()); var uncompressed = new byte[body.Length + 8]; Array.Copy(compressed, uncompressed, 8); uncompressed[0] = (byte)'F'; Array.Copy(body, 0, uncompressed, 8, body.Length);
        foreach (byte[] swf in new[] { compressed, uncompressed })
        {
            var movie = new eaf2apt.AptConverter().Convert(swf);
            check(movie.Apt.Length == 25861 && Hash(movie.Apt) == "2be204ef627584545ffef09407eff2eba3b70b20e636c5dab0b8a65eb2d14c5f", "unchanged original-tool APT golden");
            check(movie.Constants.Length == 25179 && Hash(movie.Constants) == "497a05a12aef430b3ba644c676e19929ce270387804c32cbe91b8838bb5886c0", "unchanged original-tool constants golden");
            check(movie.Dat.Length == 48 && Hash(movie.Dat) == "20ec61c0bc15b513d846d01e54dba5f726742c647054a43ef20a59181d222707", "unchanged original-tool DAT golden");
        }
    }
}
