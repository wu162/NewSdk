using BinaryAssetBuilder.UiCompiler;
using System.Security.Cryptography;

internal static class BaselineVerifier
{
    internal static void Capture(ModStreams streams)
    {
        string path = Environment.GetEnvironmentVariable("UI_CAPTURE_BASELINE"); if (string.IsNullOrEmpty(path)) return;
        using var sha = SHA256.Create();
        foreach (var asset in streams.Assets)
        { string hash = BitConverter.ToString(sha.ComputeHash(asset.Buffer.InstanceData)).Replace("-", "").ToLowerInvariant(); File.AppendAllText(path, asset.Source + ":" + asset.TypeName + ":" + asset.Name + "\t" + hash + "\n"); }
    }
}
