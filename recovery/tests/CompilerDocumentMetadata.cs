using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder.Core;

public static class CompilerDocumentMetadata
{
    const string Disable = "NewSdk.Compiler.DisableDocumentSnapshots";
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    static int checks;
    static readonly List<string> tests = new List<string>();
    static readonly List<object> evidence = new List<object>();
    static void Require(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    static object State(AssetDeclarationDocument document, string name) { return typeof(AssetDeclarationDocument).GetField(name, Hidden).GetValue(document); }
    static void Set(AssetDeclarationDocument document, string name, object value)
    {
        object current = State(document, "_Current"); current.GetType().GetField(name).SetValue(current, value);
    }
    static AssetDeclarationDocument Bare()
    {
        var result = new AssetDeclarationDocument();
        Type type = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object current = Activator.CreateInstance(type, true); type.GetMethod("FromScratch").Invoke(current, new object[0]);
        typeof(AssetDeclarationDocument).GetField("_Current", Hidden).SetValue(result, current);
        return result;
    }
    static string Xml(AssetDeclarationDocument document)
    {
        var result = new StringBuilder();
        using (var writer = XmlWriter.Create(result, new XmlWriterSettings { OmitXmlDeclaration = true })) document.WriteXml(writer);
        return result.ToString();
    }
    static AssetDeclarationDocument Read(string text)
    {
        var result = new AssetDeclarationDocument();
        using (var reader = XmlReader.Create(new StringReader(text))) { reader.MoveToContent(); result.ReadXml(reader); }
        return result;
    }
    static void Populate(AssetDeclarationDocument document)
    {
        document.SelfDefines.Add(new Definition { Name = "Armor", OriginalValue = "=1+2", EvaluatedValue = "3", IsOverride = true, Document = document });
        document.InclusionItems.Add(new InclusionItem("DATA:child.xml", "child.xml", InclusionType.All) { Document = document });
        var handle = new InstanceHandle();
        using (var reader = XmlReader.Create(new StringReader("<ih d='12345;67890;54321;9876;ArmorTemplate;StoredOwner'/>")))
            { reader.MoveToContent(); handle.ReadXml(reader); }
        Set(document, "OutputAssets", new List2<OutputAsset> { new OutputAsset { Handle = handle, InstanceFileSize = 17, RelocationFileSize = 19, ImportsFileSize = 23 } });
        document.UsedDefines["Armor"] = "3";
    }
    static void Mutate(AssetDeclarationDocument document)
    {
        document.SelfDefines[0].EvaluatedValue = "changed";
        document.SelfDefines[0].OriginalValue = "changed-original";
        document.InclusionItems[0].PhysicalPath = "changed-path";
        document.InclusionItems[0].Document = new AssetDeclarationDocument();
        document.LastOutputAssets[0].InstanceFileSize = 97;
        document.LastOutputAssets[0].Handle.TypeHash = 101;
    }
    static void PublicLifecycle()
    {
        foreach (bool disabled in new[] { false, true })
        {
            AppContext.SetSwitch(Disable, disabled);
            foreach (bool persisted in new[] { false, true })
            {
                var source = Bare(); Populate(source); source.MakeCacheable();
                if (persisted) source = Read(Xml(source));
                string before = Xml(source);
                var intermediate = new AssetDeclarationDocument(); intermediate.CacheFromDocument(source);
                var copy = new AssetDeclarationDocument(); copy.CacheFromDocument(intermediate);
                Require(Xml(copy) == before, "Public cache-copy changed serialized metadata");
                source.ResetState(); source.FromLastHack(); copy.FromLastHack();
                Require(Object.ReferenceEquals(source.SelfDefines[0], copy.SelfDefines[0]) == disabled, "Definition ownership differs from switch");
                Require(Object.ReferenceEquals(source.InclusionItems[0], copy.InclusionItems[0]) == disabled, "Include ownership differs from switch");
                Require(Object.ReferenceEquals(source.LastOutputAssets[0], copy.LastOutputAssets[0]) == disabled, "Output ownership differs from switch");
                if (!disabled)
                {
                    Require(Object.ReferenceEquals(source.SelfDefines[0].Document, source) && Object.ReferenceEquals(copy.SelfDefines[0].Document, copy), "Define source document was rebound through another stream");
                    Require(copy.SelfDefines[0].OriginalValue == (persisted ? null : "=1+2"), "OriginalValue in-memory/XML distinction changed");
                    Require(copy.LastOutputAssets[0].Handle.TypeId == 12345 && copy.LastOutputAssets[0].Handle.TypeHash == 67890 &&
                        copy.LastOutputAssets[0].Handle.InstanceId == 54321 && copy.LastOutputAssets[0].Handle.InstanceHash == 9876, "Output snapshot rehashed stored ids");
                }
                Mutate(copy);
                Require((Xml(source) == before) == !disabled && (Xml(intermediate) == before) == !disabled, "Receiver mutations wrote through cache metadata");
                // The receiver's own saved result remains unchanged until a new
                // completed result is captured; no metadata is evaluated again.
                Require((Xml(copy) == before) == !disabled, "Live metadata changed its saved cache result");
                string beforeShape = Xml(source);
                copy.LastOutputAssets.Add(new OutputAsset { Handle = new InstanceHandle("ArmorTemplate", "ReceiverOnly"), InstanceFileSize = 41 });
                Require((Xml(source) == beforeShape) == !disabled, "Legacy output collection shape no longer shares the saved list");
                copy.LastOutputAssets.RemoveAt(copy.LastOutputAssets.Count - 1);
                Require(Xml(source) == beforeShape, "Output collection shape did not restore after removing the same item");
                evidence.Add(new { id = persisted ? "persisted-metadata" : "in-memory-metadata", legacyFallback = disabled,
                    sourceXmlChanged = Xml(source) != before, scope = "Public Core cache/restore metadata ownership; not an observed CLI fault" });
            }
        }
        AppContext.SetSwitch(Disable, false);
        tests.Add("public-document-cache-copy-restore-definition-include-output-ownership-and-raw-id-preservation");
    }
    static void Aliases()
    {
        var source = Bare(); Populate(source);
        source.SelfDefines.Add(source.SelfDefines[0]); source.InclusionItems.Add(source.InclusionItems[0]);
        object current = State(source, "_Current");
        var outputs = (List2<OutputAsset>)current.GetType().GetField("OutputAssets").GetValue(current);
        outputs.Add(outputs[0]); outputs.Add(new OutputAsset { Handle = outputs[0].Handle, InstanceFileSize = 31 });
        var copy = new AssetDeclarationDocument(); copy.CacheFromDocument(source); copy.FromLastHack();
        Require(copy.SelfDefines.Count == 2 && Object.ReferenceEquals(copy.SelfDefines[0], copy.SelfDefines[1]) && !Object.ReferenceEquals(copy.SelfDefines[0], source.SelfDefines[0]), "Define duplicate alias/order lost");
        Require(copy.InclusionItems.Count == 2 && Object.ReferenceEquals(copy.InclusionItems[0], copy.InclusionItems[1]) && !Object.ReferenceEquals(copy.InclusionItems[0], source.InclusionItems[0]), "Include duplicate alias/order lost");
        Require(copy.LastOutputAssets.Count == 3 && Object.ReferenceEquals(copy.LastOutputAssets[0], copy.LastOutputAssets[1]) &&
            !Object.ReferenceEquals(copy.LastOutputAssets[0], copy.LastOutputAssets[2]) && Object.ReferenceEquals(copy.LastOutputAssets[0].Handle, copy.LastOutputAssets[2].Handle) &&
            !Object.ReferenceEquals(copy.LastOutputAssets[0].Handle, outputs[0].Handle), "Output record/Handle identity graph lost");
        tests.Add("metadata-duplicate-record-order-and-distinct-output-records-sharing-one-owned-handle");
    }
    sealed class DerivedDefinition : Definition { }
    sealed class DerivedInclusion : InclusionItem { }
    sealed class DerivedOutput : OutputAsset { }
    sealed class DerivedHandle : InstanceHandle { }
    sealed class DerivedDefinitions : List2<Definition> { }
    static void Fallbacks()
    {
        foreach (string kind in new[] { "definition", "include", "output", "handle", "list" })
        {
            var source = Bare(); Populate(source);
            object current = State(source, "_Current");
            var outputs = (List2<OutputAsset>)current.GetType().GetField("OutputAssets").GetValue(current);
            if (kind == "definition") source.SelfDefines[0] = new DerivedDefinition { Name = "Armor", EvaluatedValue = "3" };
            if (kind == "include") source.InclusionItems[0] = new DerivedInclusion();
            if (kind == "output") outputs[0] = new DerivedOutput { Handle = outputs[0].Handle };
            if (kind == "handle") outputs[0].Handle = new DerivedHandle();
            if (kind == "list") Set(source, "SelfDefines", new DerivedDefinitions { source.SelfDefines[0] });
            source.MakeCacheable();
            var first = new AssetDeclarationDocument(); first.CacheFromDocument(source);
            var copy = new AssetDeclarationDocument(); copy.CacheFromDocument(first); copy.FromLastHack();
            Require(Object.ReferenceEquals(copy.SelfDefines[0], source.SelfDefines[0]) &&
                Object.ReferenceEquals(copy.InclusionItems[0], source.InclusionItems[0]) && Object.ReferenceEquals(copy.LastOutputAssets[0], outputs[0]), "Derived metadata graph was sliced: " + kind);
            copy.MakeComplete(); copy.MakeCacheable();
            var second = new AssetDeclarationDocument(); second.CacheFromDocument(copy); second.FromLastHack();
            Require(Object.ReferenceEquals(second.SelfDefines[0], source.SelfDefines[0]), "Derived graph provenance lost after recapture: " + kind);
        }
        tests.Add("derived-record-handle-or-collection-entire-metadata-graph-legacy-provenance-through-recapture");
    }
    public static int Main(string[] args)
    {
        object result; int code = 0; bool previous; AppContext.TryGetSwitch(Disable, out previous);
        try
        {
            Require(args.Length == 5 && IntPtr.Size == 4 && Path.DirectorySeparatorChar == '\\', "Expected Windows x86 work/result/source/run/SDK");
            AppContext.SetSwitch(Disable, false); PublicLifecycle(); Aliases(); Fallbacks();
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), evidence = evidence.ToArray(),
                scope = "Document cache scalar metadata ownership; existing XML, DOM and document references remain; not immutable semantic results or parallel safety" };
            Console.WriteLine("PASS DocumentMetadata: " + tests.Count + " groups; " + checks + " checks");
        }
        catch (Exception error)
        {
            code = 1; Console.Error.WriteLine(error); result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null,
                runId = args.Length > 3 ? args[3] : null, checks, tests = tests.ToArray(), evidence = evidence.ToArray(), error = error.ToString() };
        }
        finally { AppContext.SetSwitch(Disable, previous); }
        Directory.CreateDirectory(Path.GetDirectoryName(args[1]));
        File.WriteAllText(args[1], new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue }.Serialize(result)); return code;
    }
}
