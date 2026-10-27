using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder.Core;

// Compile this exact source independently against the original SDK Core and the
// recovered Core. The probe exercises public lifecycle calls; reflection only
// supplies a tiny unprocessed document without requiring the application host.
public static class CompilerDocumentSnapshotOracle
{
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    static AssetDeclarationDocument Document(string work)
    {
        var document = new AssetDeclarationDocument();
        var type = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object current = Activator.CreateInstance(type, true);
        type.GetMethod("FromScratch").Invoke(current, new object[0]);
        type.GetField("SourcePath").SetValue(current, Path.Combine(work, "oracle.xml"));
        type.GetField("SourceDirectory").SetValue(current, work);
        typeof(AssetDeclarationDocument).GetField("_Current", Hidden).SetValue(document, current);
        var xml = new XmlDocument(); xml.LoadXml("<ArmorTemplate id='Owner'/>");
        document.SelfInstances.Add(new InstanceDeclaration(document) { XmlNode = xml.DocumentElement, ProcessingHash = 29 });
        return document;
    }
    static string Xml(AssetDeclarationDocument document)
    {
        var text = new StringBuilder();
        using (var writer = XmlWriter.Create(text, new XmlWriterSettings { OmitXmlDeclaration = true })) document.WriteXml(writer);
        return text.ToString();
    }
    static object Metadata(string work, bool expectedShared)
    {
        var source = Document(work);
        source.SelfDefines.Add(new Definition { Name = "Armor", OriginalValue = "=1+2", EvaluatedValue = "3", Document = source });
        source.InclusionItems.Add(new InclusionItem("DATA:child.xml", "child.xml", InclusionType.All) { Document = source });
        source.UsedDefines["Armor"] = "3";
        var output = new OutputAsset { Handle = new InstanceHandle("ArmorTemplate", "StoredOwner"), InstanceFileSize = 17 };
        object current = typeof(AssetDeclarationDocument).GetField("_Current", Hidden).GetValue(source);
        current.GetType().GetField("OutputAssets").SetValue(current, new List2<OutputAsset> { output });
        source.MakeCacheable(); string before = Xml(source);
        var receiver = new AssetDeclarationDocument(); receiver.CacheFromDocument(source); receiver.FromLastHack();
        bool sameDefinition = Object.ReferenceEquals(source.SelfDefines[0], receiver.SelfDefines[0]);
        bool definitionSourceRebound = Object.ReferenceEquals(source.SelfDefines[0].Document, receiver);
        bool sameInclusion = Object.ReferenceEquals(source.InclusionItems[0], receiver.InclusionItems[0]);
        bool sameOutput = Object.ReferenceEquals(output, receiver.LastOutputAssets[0]);
        bool sameOutputHandle = Object.ReferenceEquals(output.Handle, receiver.LastOutputAssets[0].Handle);
        receiver.SelfDefines[0].EvaluatedValue = "changed";
        receiver.InclusionItems[0].PhysicalPath = "changed-path";
        receiver.LastOutputAssets[0].InstanceFileSize = 97;
        receiver.LastOutputAssets[0].Handle.TypeHash = 101;
        bool sourceXmlChanged = before != Xml(source);
        string sourceBeforeUsedMutation = Xml(source), receiverBeforeUsedMutation = Xml(receiver);
        receiver.UsedDefines["Armor"] = "changed-used";
        bool usedValuesIndependent = source.UsedDefines["Armor"] == "3" &&
            Xml(source) == sourceBeforeUsedMutation && Xml(receiver) == receiverBeforeUsedMutation;
        if (sameDefinition != expectedShared || definitionSourceRebound != expectedShared || sameInclusion != expectedShared ||
            sameOutput != expectedShared || sameOutputHandle != expectedShared || sourceXmlChanged != expectedShared || !usedValuesIndependent)
            throw new Exception("Unexpected public document metadata ownership outcome");
        return new { sameDefinition, definitionSourceRebound, sameInclusion, sameOutput, sameOutputHandle, sourceXmlChanged, usedValuesIndependent,
            scope = "Independent original public Core cache-copy/restore metadata records; reflection supplies Current output fixture only; no production CLI fault claimed" };
    }
    public static int Main(string[] args)
    {
        var json = new JavaScriptSerializer(); object result; int code = 0;
        try
        {
            if (args.Length != 5 || IntPtr.Size != 4) throw new Exception("Expected x86 work result source run original|fallback|owned");
            string mode = args[4];
            if (mode != "original" && mode != "fallback" && mode != "owned") throw new Exception("Unknown oracle mode");
            AppContext.SetSwitch("NewSdk.Compiler.DisableDocumentSnapshots", mode == "fallback");
            AppContext.SetSwitch("NewSdk.Compiler.DisableDeclarationHandleSnapshots", false);
            Directory.CreateDirectory(args[0]);
            var source = Document(args[0]); source.MakeCacheable(); string before = Xml(source);
            var receiver = new AssetDeclarationDocument(); receiver.CacheFromDocument(source); receiver.FromLastHack();
            bool sameDeclaration = Object.ReferenceEquals(source.SelfInstances[0], receiver.SelfInstances[0]);
            uint savedHash = source.SelfInstances[0].ProcessingHash;
            receiver.MakeComplete(); receiver.SelfInstances[0].ProcessingHash = 47; receiver.MakeCacheable();
            bool sourceXmlChanged = before != Xml(source);
            bool expectedShared = mode != "owned";
            if (sameDeclaration != expectedShared || sourceXmlChanged != expectedShared || savedHash != 29 || receiver.SelfInstances[0].ProcessingHash != 47)
                throw new Exception("Unexpected public document cache lifecycle outcome for " + mode);
            object metadata = Metadata(args[0], expectedShared);
            string assemblySha;
            using (var sha = SHA256.Create()) assemblySha = Convert.ToBase64String(sha.ComputeHash(File.ReadAllBytes(typeof(AssetDeclarationDocument).Assembly.Location)));
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], mode, sameDeclaration, sourceXmlChanged,
                savedHash, receiverHash = receiver.SelfInstances[0].ProcessingHash, metadata,
                assembly = typeof(AssetDeclarationDocument).Assembly.FullName, assemblySha256Base64 = assemblySha,
                scope = "Direct public Core cache-copy/restore/complete/reserialize lifecycle; no production CLI failure is claimed" };
            Console.WriteLine("PASS DocumentSnapshotOracle: " + mode);
        }
        catch (Exception error)
        {
            code = 1; Console.Error.WriteLine(error);
            result = new { succeeded = false, mode = args.Length > 4 ? args[4] : null, error = error.ToString() };
        }
        if (args.Length > 1) File.WriteAllText(args[1], json.Serialize(result));
        return code;
    }
}
