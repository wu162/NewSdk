#if WEAPON_TRANSCRIPT_ORACLE || WEAPON_TRANSCRIPT_PRODUCT
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;

// Transport only. This file is compiled into isolated fixture hosts, never the product.
static unsafe class WeaponTranscript
{
    static BinaryWriter writer;
    static BinaryReader reader;
    static Stream stream;
    static int buffers, masks, rejections;
    static readonly int[] platforms = new int[4];
    static readonly Dictionary<string, MethodInfo> maskParsers = new Dictionary<string, MethodInfo>(StringComparer.Ordinal);
    static void Require(bool condition, string message) { if (!condition) throw new InvalidDataException(message); }
    public static void VerifyProduct(Assembly foundation, Assembly bridge, Assembly wrapper)
    {
        Require(foundation == bridge && foundation == wrapper, "Weapon bridge, wrapper and Core types belong to different assemblies");
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
            writer.Write("NewSdk.Weapon.FullFixture.v1");
        }
        else
        {
            stream = new GZipStream(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read), CompressionMode.Decompress);
            reader = new BinaryReader(stream, Encoding.UTF8);
            Require(reader.ReadString() == "NewSdk.Weapon.FullFixture.v1", "Unknown Weapon transcript format");
        }
    }
    static void Identity(byte kind, string identity)
    {
        if (writer != null) { writer.Write(kind); writer.Write(identity); }
        else Require(reader.ReadByte() == kind && reader.ReadString() == identity, "Weapon fixture order or identity changed");
    }
    static string Number(uint value) { return value.ToString(System.Globalization.CultureInfo.InvariantCulture); }
    static void WriteBytes(byte[] bytes)
    {
        Require(bytes != null, "Original returned null buffer"); writer.Write(bytes.Length); writer.Write(bytes);
    }
    static byte[] ReadBytes()
    {
        int count = reader.ReadInt32(); Require(count >= 0 && count <= 64 * 1024 * 1024, "Invalid transcript buffer size");
        byte[] bytes = reader.ReadBytes(count); Require(bytes.Length == count, "Truncated transcript"); return bytes;
    }
    public static uint[] ParseMask(Assembly assembly, string name, string text, uint[] seed)
    {
        MethodInfo parser;
        if (!maskParsers.TryGetValue(name, out parser))
        {
            foreach (var method in assembly.GetModules()[0].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                if (method.Name == name) parser = method;
            if (parser == null) throw new InvalidOperationException("Missing original mask parser: " + name);
            maskParsers.Add(name, parser);
        }
        uint[] result = (uint[])seed.Clone();
        IntPtr ansi = Marshal.StringToHGlobalAnsi(text);
        try
        {
            fixed (uint* target = result)
            {
                var types = parser.GetParameters();
                parser.Invoke(null, new object[] { Pointer.Box(ansi.ToPointer(), types[0].ParameterType), Pointer.Box(target, types[1].ParameterType), Pointer.Box(null, types[2].ParameterType) });
            }
        }
        finally { Marshal.FreeHGlobal(ansi); }
        return result;
    }
    public static uint[] ExpectedMask(Assembly assembly, string name, string text, uint[] seed)
    {
        Identity(2, name); Identity(3, text);
        if (writer != null)
        {
            writer.Write(seed.Length); foreach (uint word in seed) writer.Write(word);
            uint[] expected = ParseMask(assembly, name, text, seed);
            foreach (uint word in expected) writer.Write(word);
            ++masks; return expected;
        }
        Require(reader.ReadInt32() == seed.Length, "Mask seed length changed");
        foreach (uint word in seed) Require(reader.ReadUInt32() == word, "Mask seed changed");
        uint[] result = new uint[seed.Length]; for (int i = 0; i < result.Length; ++i) result[i] = reader.ReadUInt32();
        ++masks; return result;
    }
    public static void Rejection(string xml)
    {
        // These are recovered compiler contract rejections. They are not invoked
        // on the original tokenizer, whose unsupported-input behavior can differ.
        Identity(4, xml); ++rejections;
    }
    public sealed class OriginalPlugin : IAssetBuilderPlugin
    {
        readonly IAssetBuilderPlugin original;
        TargetPlatform platform;
        public OriginalPlugin(IAssetBuilderPlugin original) { this.original = original; }
        public void Initialize(object config, TargetPlatform target)
        {
            platform = target; if (original != null) original.Initialize(config, target);
        }
        public void ReInitialize(object config, TargetPlatform target) { Initialize(config, target); }
        public uint GetAllTypesHash()
        {
            Identity(5, ((int)platform).ToString(System.Globalization.CultureInfo.InvariantCulture));
            if (writer == null) return reader.ReadUInt32();
            uint value = original.GetAllTypesHash(); writer.Write(value); return value;
        }
        public uint GetVersionNumber()
        {
            Identity(6, ((int)platform).ToString(System.Globalization.CultureInfo.InvariantCulture));
            if (writer == null) return reader.ReadUInt32();
            uint value = original.GetVersionNumber(); writer.Write(value); return value;
        }
        public ExtendedTypeInformation GetExtendedTypeInformation(uint id)
        {
            Identity(7, ((int)platform).ToString(System.Globalization.CultureInfo.InvariantCulture) + "/" + Number(id));
            if (writer != null)
            {
                var info = original.GetExtendedTypeInformation(id);
                Require(info != null, "Original metadata is null");
                writer.Write(info.TypeId); writer.Write(info.TypeName); writer.Write(info.TypeHash); writer.Write(info.ProcessingHash);
                writer.Write(info.HasCustomData); writer.Write(info.Tokenized); writer.Write(info.UseBuildCache);
                return info;
            }
            return new ExtendedTypeInformation { TypeId = reader.ReadUInt32(), TypeName = reader.ReadString(), TypeHash = reader.ReadUInt32(),
                ProcessingHash = reader.ReadUInt32(), HasCustomData = reader.ReadBoolean(), Tokenized = reader.ReadBoolean(), UseBuildCache = reader.ReadBoolean() };
        }
        public AssetBuffer ProcessInstance(InstanceDeclaration declaration)
        {
            int number = (int)platform;
            int index = number == 0 ? 0 : number == 1 ? 1 : number == 2 ? 2 : number == 99 ? 3 : -1;
            Require(index >= 0, "Unexpected fixture platform"); ++platforms[index];
            Identity(1, number.ToString(System.Globalization.CultureInfo.InvariantCulture) + "/" + declaration.Node.OuterXml);
            ++buffers;
            if (writer != null)
            {
                var value = original.ProcessInstance(declaration);
                Require(value != null, "Original ProcessInstance returned null");
                WriteBytes(value.InstanceData); WriteBytes(value.RelocationData); WriteBytes(value.ImportsData); return value;
            }
            return new AssetBuffer { InstanceData = ReadBytes(), RelocationData = ReadBytes(), ImportsData = ReadBytes() };
        }
    }
    public static void Complete(int fixtures, int comparisons, int rejected, int statusMasks, int modelMasks, int annotated)
    {
        Require(buffers == 2 * fixtures && fixtures > 0 && annotated == fixtures, "Incomplete Weapon fixture/annotation accounting");
        Require(masks == statusMasks + modelMasks && statusMasks > 0 && modelMasks > 0, "Incomplete direct mask matrix");
        Require(rejections == rejected && rejected > 0, "Incomplete recovered rejection matrix");
        foreach (int count in platforms) Require(count > 0 && count == platforms[0], "Incomplete four-platform fixture matrix");
        int[] summary = { fixtures, comparisons, rejected, statusMasks, modelMasks, annotated, buffers, masks, platforms[0], platforms[1], platforms[2], platforms[3] };
        Identity(8, "summary");
        if (writer != null) { foreach (int value in summary) writer.Write(value); writer.Flush(); }
        else
        {
            foreach (int value in summary) Require(reader.ReadInt32() == value, "Original/product accounting differs");
            Require(stream.ReadByte() == -1, "Trailing untested transcript data");
        }
        Console.WriteLine("PASS Weapon full transcript " + (writer != null ? "record" : "replay") + ": fixtures=" + fixtures +
            " bufferComparisons=" + comparisons + " recoveredContractRejections=" + rejected + " directStatusComparisons=" + statusMasks +
            " directModelComparisons=" + modelMasks + " annotatedFixtures=" + annotated + " fourPlatforms=0,1,2,99");
    }
    public static void Close()
    {
        try { if (writer != null) writer.Dispose(); else if (reader != null) reader.Dispose(); else if (stream != null) stream.Dispose(); }
        finally { writer = null; reader = null; stream = null; }
    }
}
#if WEAPON_TRANSCRIPT_ORACLE
// Original-only fixture substitute lets the unchanged independent layout checks
// run in the recording process. It is never built into the replay host or product.
namespace Recovered
{
    static class WeaponCompiler
    {
        static IAssetBuilderPlugin original;
        static Assembly assembly;
        public static void SetOriginal(IAssetBuilderPlugin plugin, Assembly source) { original = plugin; assembly = source; }
        public static AssetBuffer ProcessInstance(InstanceDeclaration declaration, TargetPlatform platform)
        {
            original.Initialize(null, platform); return original.ProcessInstance(declaration);
        }
        public static AssetBuffer CompileXml(XmlElement node, XmlNamespaceManager namespaces, bool bigEndian)
        {
            return WeaponOracle.OriginalCompile(node, namespaces, bigEndian ? TargetPlatform.PlayStation3 : TargetPlatform.Win32);
        }
        public static uint[] MarshalObjectStatus(string text, uint[] seed)
        {
            return WeaponTranscript.ParseMask(assembly, "Marshaler.Marshal<enum ObjectStatusType,223>", text, seed);
        }
        public static uint[] MarshalModelCondition(string text, uint[] seed)
        {
            return WeaponTranscript.ParseMask(assembly, "Marshaler.Marshal<enum ModelConditionFlagType,456>", text, seed);
        }
    }
}
unsafe partial class WeaponOracle
{
    internal static AssetBuffer OriginalCompile(XmlElement node, XmlNamespaceManager namespaces, TargetPlatform platform)
    {
        return Recovered.WeaponCompiler.ProcessInstance(Declaration(node.OwnerDocument, namespaces), platform);
    }
}
#endif
#endif

