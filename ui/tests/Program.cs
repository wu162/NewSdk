// SPDX-License-Identifier: GPL-3.0-or-later
using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using eaf2apt;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
string fixture = Path.GetFullPath(args[0]);
string report = Path.GetFullPath(args[1]);
byte[] input = File.ReadAllBytes(fixture);
if (Convert.ToHexString(SHA256.HashData(input)).ToLowerInvariant() !=
    "b661542cf2c8b52ef846c348f817f523f5fba63ee4baa53c365aabde30af9fa7")
    throw new Exception("Fixture fingerprint mismatch");
string scratch = Path.Combine(Path.GetTempPath(), "newsdk-ui-" + Guid.NewGuid());
Directory.CreateDirectory(scratch);
string oldCwd = Environment.CurrentDirectory;
try
{
    string baseline = Path.Combine(scratch, "baseline");
    Directory.CreateDirectory(baseline);
    string swf = Path.Combine(baseline, "CoronaUIMain.swf");
    File.WriteAllBytes(swf, input);
    Environment.CurrentDirectory = baseline;
    new AptConverter().ConvertToApt(swf, optimize: false, isra3: true);
    byte[] apt = File.ReadAllBytes(Path.ChangeExtension(swf, ".apt"));
    byte[] constants = File.ReadAllBytes(Path.ChangeExtension(swf, ".const"));
    byte[] dat = File.ReadAllBytes(Path.ChangeExtension(swf, ".dat"));
    string memoryDir = Path.Combine(scratch, "memory");
    Directory.CreateDirectory(memoryDir);
    Environment.CurrentDirectory = memoryDir;
    int comparisons = 0;
    foreach (var bytes in new[] { input, Uncompress(input) })
    foreach (string culture in new[] { "en-US", "zh-CN" })
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
        var actual = new MemoryAptConverter().Convert(bytes, "CoronaUIMain");
        if (!apt.SequenceEqual(actual.Apt) || !constants.SequenceEqual(actual.Constants) || !dat.SequenceEqual(actual.Dat))
            throw new Exception("Byte-for-byte comparison failed: " + culture);
        comparisons++;
        if (Directory.EnumerateFileSystemEntries(memoryDir).Any())
            throw new Exception("Memory converter created filesystem artifacts");
    }
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    int rejected = 0;
    void Reject(byte[] bytes, string name)
    {
        try { new MemoryAptConverter().Convert(bytes, name); }
        catch (Exception e) when (e is ArgumentException || e is InvalidDataException || e is NotSupportedException)
        { rejected++; return; }
        throw new Exception("Invalid input was not explicitly rejected");
    }
    Reject(Array.Empty<byte>(), "Ui");
    Reject(input, "../Ui");
    Reject(input, "");
    var version = (byte[])input.Clone(); version[3] = 9; Reject(version, "Ui");
    var visual = Uncompress(input);
    // Change first SetBackgroundColor to DefineShape (same payload size).
    int rectBits = visual[8] >> 3;
    int firstTag = 8 + (5 + 4 * rectBits + 7) / 8 + 4;
    visual[firstTag] = (byte)((2 << 6) | 3); visual[firstTag + 1] = 0;
    Reject(visual, "Ui");
    if (Directory.EnumerateFileSystemEntries(memoryDir).Any()) throw new Exception("Rejection created files");
    Directory.CreateDirectory(Path.GetDirectoryName(report));
    var evidence = new {
        sourceCommit = Environment.GetEnvironmentVariable("GITHUB_SHA"),
        runId = Environment.GetEnvironmentVariable("GITHUB_RUN_ID"),
        upstreamCommit = "60b15a67a4cbd931f181af8c6c758bd7a8441ebc",
        fixtureSha256 = Convert.ToHexString(SHA256.HashData(input)).ToLowerInvariant(),
        baseline = "pinned lanyizi/swf2apt source, optimize=false, ra3=true; NOT uploaded binary or optimized script",
        comparisons, rejected, noOutputFiles = true,
        aptLength = apt.Length, constLength = constants.Length, datLength = dat.Length,
        aptSha256 = Convert.ToHexString(SHA256.HashData(apt)).ToLowerInvariant(),
        constSha256 = Convert.ToHexString(SHA256.HashData(constants)).ToLowerInvariant(),
        datSha256 = Convert.ToHexString(SHA256.HashData(dat)).ToLowerInvariant(),
        sdkIntegration = false, optimizerParity = false
    };
    string json = JsonSerializer.Serialize(evidence, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(report, json);
    Console.WriteLine(json);
}
finally { Environment.CurrentDirectory = oldCwd; Directory.Delete(scratch, recursive: true); }

static byte[] Uncompress(byte[] swf)
{
    using var source = new MemoryStream(swf, 8, swf.Length - 8);
    using var zlib = new ZLibStream(source, CompressionMode.Decompress);
    using var result = new MemoryStream();
    result.Write(swf, 0, 8); zlib.CopyTo(result);
    byte[] bytes = result.ToArray(); bytes[0] = (byte)'F'; return bytes;
}
