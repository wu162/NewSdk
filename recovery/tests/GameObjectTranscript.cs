#if GO_TRANSCRIPT_ORACLE || GO_TRANSCRIPT_PRODUCT
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Reflection;
using System.Globalization;
using BinaryAssetBuilder.Core;

// Shared fixture transport between separate original-Core and production-Core
// processes. Never linked into the production executable.
static class GameObjectTranscript
{
    static BinaryWriter writer;
    static BinaryReader reader;
    static Stream stream;
    static int successes, rejections, observations;
    static readonly int[] platforms = new int[4];
    static void Require(bool condition, string message) { if (!condition) throw new InvalidDataException(message); }
    public static void VerifyProduct(Assembly foundation, Assembly bridge)
    {
        Require(foundation == bridge, "Bridge and Core types belong to different assemblies");
        Require(foundation == Assembly.GetEntryAssembly(), "Business implementation is not the process entry assembly");
        Require(foundation.GetName().Name == "NewModCompiler" && foundation.GetModules().Length == 1,
            "Expected one physical production EXE module");
        string expected = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NewModCompiler.exe"));
        Require(String.Equals(Path.GetFullPath(foundation.Location), expected, StringComparison.OrdinalIgnoreCase),
            "Product was resolved outside the isolated replay directory");
        foreach (AssemblyName reference in foundation.GetReferencedAssemblies())
            Require(!IsOldBusiness(reference.Name), "Production retains an old business AssemblyRef: " + reference.FullName);
        foreach (Assembly loaded in AppDomain.CurrentDomain.GetAssemblies())
            Require(!IsOldBusiness(loaded.GetName().Name), "Old business assembly was loaded: " + loaded.FullName);
    }
    static bool IsOldBusiness(string name)
    {
        return name.StartsWith("BinaryAssetBuilder.", StringComparison.Ordinal) || name.StartsWith("Recovered.", StringComparison.Ordinal) ||
            name.StartsWith("EALA.", StringComparison.Ordinal) || name == "Mvp.Xml" || name.StartsWith("Managed.Foundation", StringComparison.Ordinal) ||
            name.StartsWith("Managed.Application", StringComparison.Ordinal);
    }
    public static void Open(string path, bool recording)
    {
        Require(stream == null, "Transcript already open");
        if (recording)
        {
            stream = new GZipStream(new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None), CompressionMode.Compress);
            writer = new BinaryWriter(stream, Encoding.UTF8);
            writer.Write("NewSdk.GameObject.FullFixture.v1");
        }
        else
        {
            stream = new GZipStream(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read), CompressionMode.Decompress);
            reader = new BinaryReader(stream, Encoding.UTF8);
            Require(reader.ReadString() == "NewSdk.GameObject.FullFixture.v1", "Unknown GameObject transcript format");
        }
    }
    static void Identity(byte tag, TargetPlatform platform, string attributes, string children)
    {
        int number = (int)platform;
        int index = number == 0 ? 0 : number == 1 ? 1 : number == 2 ? 2 : number == 99 ? 3 : -1;
        Require(index >= 0, "Unexpected fixture platform"); ++platforms[index];
        if (writer != null)
        {
            writer.Write(tag); writer.Write(number); writer.Write(attributes); writer.Write(children);
        }
        else
        {
            Require(reader.ReadByte() == tag && reader.ReadInt32() == number && reader.ReadString() == attributes && reader.ReadString() == children,
                "Fixture order/input/platform differs from original transcript at success " + successes + " / rejection " + rejections);
        }
    }
    static void WriteBytes(byte[] bytes)
    {
        Require(bytes != null, "Original returned null buffer"); writer.Write(bytes.Length); writer.Write(bytes);
    }
    static byte[] ReadBytes()
    {
        int size = reader.ReadInt32(); Require(size >= 0 && size <= 64 * 1024 * 1024, "Invalid transcript buffer length");
        byte[] value = reader.ReadBytes(size); Require(value.Length == size, "Truncated transcript buffer"); return value;
    }
    public static void Write(TargetPlatform platform, string attributes, string children, AssetBuffer value)
    {
        Require(writer != null, "Transcript is not recording"); Identity(1, platform, attributes, children);
        WriteBytes(value.InstanceData); WriteBytes(value.RelocationData); WriteBytes(value.ImportsData); ++successes;
    }
    public static AssetBuffer Read(TargetPlatform platform, string attributes, string children)
    {
        Require(reader != null, "Transcript is not replaying"); Identity(1, platform, attributes, children);
        var value = new AssetBuffer { InstanceData = ReadBytes(), RelocationData = ReadBytes(), ImportsData = ReadBytes() }; ++successes; return value;
    }
    public static void Rejection(TargetPlatform platform, string attributes, string children)
    {
        Identity(2, platform, attributes, children); ++rejections;
    }
    public static void WriteObservation(TargetPlatform platform, string attributes, string children, AssetBuffer value)
    {
        Identity(5, platform, attributes, children); WriteBytes(value.InstanceData); WriteBytes(value.RelocationData); WriteBytes(value.ImportsData); ++observations;
    }
    public static AssetBuffer ReadObservation(TargetPlatform platform, string attributes, string children)
    {
        Identity(5, platform, attributes, children); ++observations;
        return new AssetBuffer { InstanceData = ReadBytes(), RelocationData = ReadBytes(), ImportsData = ReadBytes() };
    }
    static string ExceptionSignature(Exception error)
    {
        Require(error != null, "Expected original/product exception was not raised");
        var result = new StringBuilder();
        result.Append(error.GetType().FullName).Append('\n').Append(error.Message);
        foreach (string name in new[] { "ErrorCode", "Code" })
        {
            PropertyInfo property = error.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (property == null) continue;
            object value = property.GetValue(error, null);
            result.Append('\n').Append(name).Append(':').Append(property.PropertyType.FullName).Append(':');
            result.Append(value == null ? "<null>" : Convert.ToString(value, CultureInfo.InvariantCulture));
        }
        return result.ToString();
    }
    public static void CheckException(TargetPlatform platform, string attributes, string children, Exception error)
    {
        Identity(4, platform, attributes, children);
        string signature = ExceptionSignature(error);
        if (writer != null) writer.Write(signature);
        else Require(reader.ReadString() == signature, "Original/product exception type, message or error code differs");
        ++rejections;
    }
    public static void Complete(int fixtureCount, int rejectionCount, int comparisonCount, int invarianceCount)
    {
        Require(successes == fixtureCount && rejections == rejectionCount && successes > 0 && rejections > 0, "Incomplete fixture accounting");
        foreach (int count in platforms) Require(count > 0, "Incomplete four-platform fixture matrix");
        int[] summary = { successes, rejections, observations, comparisonCount, invarianceCount, platforms[0], platforms[1], platforms[2], platforms[3] };
        if (writer != null) { writer.Write((byte)3); foreach (int value in summary) writer.Write(value); writer.Flush(); }
        else
        {
            Require(reader.ReadByte() == 3, "Transcript contains additional untested fixtures");
            foreach (int value in summary) Require(reader.ReadInt32() == value, "Original/product fixture accounting differs");
            Require(stream.ReadByte() == -1, "Trailing transcript data");
        }
        Console.WriteLine("PASS GameObject full transcript " + (writer != null ? "record" : "replay") +
            ": fixtures=" + successes + " productContractRejections=" + rejections + " fourPlatforms=0,1,2,99" +
            " threeBufferComparisons=" + comparisonCount + " invarianceComparisons=" + invarianceCount);
    }
    public static void Close()
    {
        try { if (writer != null) writer.Dispose(); else if (reader != null) reader.Dispose(); else if (stream != null) stream.Dispose(); }
        finally { writer = null; reader = null; stream = null; }
    }
}
#endif

