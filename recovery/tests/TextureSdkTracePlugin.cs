using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
#if ORIGINAL
using TraceBase = BinaryAssetBuilder.TextureCompiler.Plugin;
#else
using TraceBase = Recovered.Compiler.Texture.Plugin;
#endif

// Loaded solely through BAB's configured plugin type. There is no Main or
// synthetic ProcessInstance caller: only the real Core dispatch reaches this
// override. Compile both variants as TextureSdkTrace.dll with this same type.
public class TextureSdkTracePlugin : TraceBase
{
    private static readonly object TraceLock = new object();
    private const string Header = "Stage\tTypeId\tInstanceId\tTypeName\tInstanceName\tFileSHA256\tOutputFormat\tGenerateMipMaps\tType\tInstanceLength\tRelocationLength\tImportsLength\tHasCData\tCDataSHA256\tExceptionType";
    private static readonly Encoding Utf8 = new UTF8Encoding(false);

    private static string Attribute(InstanceDeclaration declaration, string name)
    {
        XmlNode node = declaration.XmlNode;
        XmlAttribute attribute = node == null || node.Attributes == null ? null : node.Attributes[name];
        return attribute == null ? "" : attribute.Value;
    }
    private static string HashFile(string path)
    {
        if (String.IsNullOrEmpty(path) || !File.Exists(path)) return "";
        using (SHA256 sha = SHA256.Create())
        using (FileStream stream = File.OpenRead(path))
            return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
    }
    private static string Tsv(string value)
    {
        return (value ?? "").Replace("\\", "\\\\").Replace("\t", "\\t").Replace("\r", "\\r").Replace("\n", "\\n");
    }
    private static string FileComponent(string value)
    {
        StringBuilder encoded = new StringBuilder();
        foreach (char character in value ?? "")
        {
            // Preserve readable ordinary SDK IDs, but encode reserved Windows
            // characters and '%' without collapsing different IDs to '_'.
            if (character < 32 || character == '%' || Array.IndexOf(Path.GetInvalidFileNameChars(), character) >= 0 ||
                character == '/' || character == '\\' || character == ':')
                encoded.Append('%').Append(((int)character).ToString("X4", CultureInfo.InvariantCulture));
            else encoded.Append(character);
        }
        return encoded.ToString();
    }
    private static string Length(byte[] data)
    {
        return (data == null ? -1 : data.Length).ToString(CultureInfo.InvariantCulture);
    }
    private static void WriteBuffer(string path, byte[] buffer)
    {
        // Null remains distinguishable from empty through the TSV length -1.
        File.WriteAllBytes(path, buffer ?? new byte[0]);
    }
    private static void AppendCall(string directory, string[] identity, string stage, AssetBuffer result,
        bool hasCData, string cdataHash, string exceptionType)
    {
        string[] fields = new string[] {
            stage, identity[0], identity[1], identity[2], identity[3], identity[4], identity[5], identity[6], identity[7],
            result == null ? "-1" : Length(result.InstanceData),
            result == null ? "-1" : Length(result.RelocationData),
            result == null ? "-1" : Length(result.ImportsData),
            hasCData ? "true" : "false", cdataHash, exceptionType
        };
        for (int i = 0; i < fields.Length; ++i) fields[i] = Tsv(fields[i]);
        string path = Path.Combine(directory, "texture.calls.tsv");
        if (!File.Exists(path)) File.WriteAllText(path, Header + "\n", Utf8);
        File.AppendAllText(path, String.Join("\t", fields) + "\n", Utf8);
    }
#if NO_ORIGINAL
    private static void VerifyOriginalAbsent()
    {
        if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BinaryAssetBuilder.TextureCompiler.dll")))
            throw new InvalidOperationException("Original TextureCompiler DLL present in isolated SDK.");
        foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            if (assembly.GetName().Name == "BinaryAssetBuilder.TextureCompiler")
                throw new InvalidOperationException("Original TextureCompiler assembly loaded in isolated SDK.");
    }
#endif
    public override AssetBuffer ProcessInstance(InstanceDeclaration declaration)
    {
#if NO_ORIGINAL
        VerifyOriginalAbsent();
#endif
        string directory = Environment.GetEnvironmentVariable("RA3_TEXTURE_SDK_TRACE");
        if (String.IsNullOrEmpty(directory)) return base.ProcessInstance(declaration);
        if (declaration == null || declaration.Handle == null)
            throw new ArgumentNullException("declaration", "SDK tracing requires a real Core instance declaration.");
        directory = Path.GetFullPath(directory);
        string[] identity = new string[] {
            declaration.Handle.TypeId.ToString("x8", CultureInfo.InvariantCulture),
            declaration.Handle.InstanceId.ToString("x8", CultureInfo.InvariantCulture),
            declaration.Handle.TypeName, declaration.Handle.InstanceName,
            HashFile(Attribute(declaration, "File")), Attribute(declaration, "OutputFormat"),
            Attribute(declaration, "GenerateMipMaps"), Attribute(declaration, "Type")
        };
        string prefix = FileComponent(identity[2]) + "_" + FileComponent(identity[3]);
        lock (TraceLock)
        {
            Directory.CreateDirectory(directory);
            AppendCall(directory, identity, "entry", null, false, "", "");
        }
        AssetBuffer result;
        try
        {
            result = base.ProcessInstance(declaration);
#if NO_ORIGINAL
            VerifyOriginalAbsent();
#endif
        }
        catch (Exception error)
        {
            lock (TraceLock) AppendCall(directory, identity, "failure", null, false, "", error.GetType().FullName);
            throw;
        }
        lock (TraceLock)
        {
            bool hasCData = !String.IsNullOrEmpty(declaration.CustomDataPath) && File.Exists(declaration.CustomDataPath);
            string cdataHash = hasCData ? HashFile(declaration.CustomDataPath) : "";
            WriteBuffer(Path.Combine(directory, prefix + ".instance"), result == null ? null : result.InstanceData);
            WriteBuffer(Path.Combine(directory, prefix + ".relo"), result == null ? null : result.RelocationData);
            WriteBuffer(Path.Combine(directory, prefix + ".imp"), result == null ? null : result.ImportsData);
            string cdataTrace = Path.Combine(directory, prefix + ".cdata");
            if (hasCData) File.Copy(declaration.CustomDataPath, cdataTrace, true);
            else if (File.Exists(cdataTrace)) File.Delete(cdataTrace);
            AppendCall(directory, identity, "success", result, hasCData, cdataHash, "");
        }
        return result;
    }
}
