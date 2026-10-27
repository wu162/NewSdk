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
            string assemblySha;
            using (var sha = SHA256.Create()) assemblySha = Convert.ToBase64String(sha.ComputeHash(File.ReadAllBytes(typeof(AssetDeclarationDocument).Assembly.Location)));
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], mode, sameDeclaration, sourceXmlChanged,
                savedHash, receiverHash = receiver.SelfInstances[0].ProcessingHash,
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
