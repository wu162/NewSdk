using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder.Core;

public static class CompilerDocumentMetadata
{
    const string Disable = "NewSdk.Compiler.DisableDocumentSnapshots";
    const string ShareDisable = "NewSdk.Compiler.DisableSharedDocumentMetadata";
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
        source.MakeCacheable();
        var intermediate = new AssetDeclarationDocument(); intermediate.CacheFromDocument(source);
        var copy = new AssetDeclarationDocument(); copy.CacheFromDocument(intermediate); copy.FromLastHack();
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
    static object SavedField(AssetDeclarationDocument document, string name)
    {
        object saved = State(document, "_Last");
        return saved.GetType().GetField(name).GetValue(saved);
    }
    static void SavedOwnership()
    {
        foreach (bool deepCopy in new[] { false, true })
        foreach (bool persisted in new[] { false, true })
        {
            AppContext.SetSwitch(ShareDisable, deepCopy);
            var source = Bare(); Populate(source); source.MakeCacheable();
            if (persisted) source = Read(Xml(source));
            string before = Xml(source);
            var first = new AssetDeclarationDocument(); first.CacheFromDocument(source);
            var second = new AssetDeclarationDocument(); second.CacheFromDocument(first);
            foreach (string field in new[] { "InclusionItems", "OutputAssets", "SelfDefines", "UsedDefines" })
            {
                Require(Object.ReferenceEquals(SavedField(source, field), SavedField(first, field)) == !deepCopy,
                    "Private saved metadata sharing/deep-copy mismatch: " + field);
                Require(Object.ReferenceEquals(SavedField(first, field), SavedField(second, field)) == !deepCopy,
                    "Transitive private saved metadata sharing/deep-copy mismatch: " + field);
            }
            first.FromLastHack(); second.FromLastHack();
            Require(!Object.ReferenceEquals(first.SelfDefines[0], second.SelfDefines[0]) &&
                !Object.ReferenceEquals(first.InclusionItems[0], second.InclusionItems[0]) &&
                !Object.ReferenceEquals(first.LastOutputAssets[0], second.LastOutputAssets[0]), "Shared private values escaped into mutable Current");
            Require(first.SelfDefines[0].Document == first && second.SelfDefines[0].Document == second,
                "Restoring private metadata rebound another document's Define source");
            Mutate(first);
            Require(Xml(source) == before && Xml(first) == before && Xml(second) == before,
                "Current mutation wrote through a shared saved result");
            first.MakeComplete(); first.MakeCacheable();
            Require(Xml(first) != before && Xml(source) == before && Xml(second) == before,
                "Receiver recapture changed a prior saved owner");
            evidence.Add(new { id = "private-saved-metadata-ownership", deepCopyFallback = deepCopy, persisted,
                privateSavedListsShared = !deepCopy, liveRecordsIndependent = true });
        }
        AppContext.SetSwitch(ShareDisable, false);
        foreach (bool transferAfterDisable in new[] { false, true })
        {
            AppContext.SetSwitch(Disable, false);
            var source = Bare(); Populate(source); source.MakeCacheable(); string before = Xml(source);
            var first = new AssetDeclarationDocument(); first.CacheFromDocument(source);
            var second = new AssetDeclarationDocument(); second.CacheFromDocument(source);
            AppContext.SetSwitch(Disable, true);
            var receiver = first;
            if (transferAfterDisable) { receiver = new AssetDeclarationDocument(); receiver.CacheFromDocument(first); }
            receiver.FromLastHack(); Mutate(receiver);
            Require(Xml(source) == before && Xml(second) == before,
                "Dynamic legacy metadata restore/transfer wrote through a formerly independent sibling");
            Require(Xml(receiver) != before, "Dynamic legacy metadata restore lost per-owner saved/current aliases");
            if (transferAfterDisable) Require(Xml(first) == Xml(receiver), "Legacy transfer no longer shares the detached owner's saved metadata");
            AppContext.SetSwitch(Disable, false);
            var afterLegacy = new AssetDeclarationDocument(); afterLegacy.CacheFromDocument(receiver);
            Require(!Object.ReferenceEquals(SavedField(receiver, "SelfDefines"), SavedField(afterLegacy, "SelfDefines")),
                "Legacy-exposed saved values authorized subsequent private sharing");
            string legacyBefore = Xml(receiver); afterLegacy.FromLastHack(); afterLegacy.SelfDefines[0].EvaluatedValue = "after-legacy";
            Require(Xml(receiver) == legacyBefore, "Re-enabled snapshot path mutated legacy-exposed source");
        }
        AppContext.SetSwitch(Disable, false);
        var binarySource = Bare(); Populate(binarySource); binarySource.MakeCacheable();
        AssetDeclarationDocument unknown;
        using (var memory = new MemoryStream())
        {
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            formatter.Serialize(memory, binarySource); memory.Position = 0;
            unknown = (AssetDeclarationDocument)formatter.Deserialize(memory);
        }
        var fromUnknown = new AssetDeclarationDocument(); fromUnknown.CacheFromDocument(unknown);
        foreach (string field in new[] { "InclusionItems", "OutputAssets", "SelfDefines", "UsedDefines" })
            Require(!Object.ReferenceEquals(SavedField(unknown, field), SavedField(fromUnknown, field)),
                "Unknown BinaryFormatter provenance authorized private metadata sharing: " + field);
        string unknownBefore = Xml(unknown); fromUnknown.FromLastHack(); Mutate(fromUnknown);
        Require(Xml(unknown) == unknownBefore, "Unknown serialized metadata source was mutated through receiver");
        tests.Add("private-saved-metadata-sharing-XML-and-runtime-provenance-current-recapture-dynamic-legacy-detach-and-unknown-binary-fallback");
    }
    static List<AssetDeclarationDocument> TransferChain(AssetDeclarationDocument source, int count)
    {
        var result = new List<AssetDeclarationDocument>(count); var previous = source;
        for (int index = 0; index < count; index++)
        {
            var next = new AssetDeclarationDocument(); next.CacheFromDocument(previous);
            result.Add(next); previous = next;
        }
        return result;
    }
    static int DistinctSavedRecords(IEnumerable<AssetDeclarationDocument> documents)
    {
        var records = new HashSet<object>();
        foreach (var document in documents)
        foreach (string field in new[] { "InclusionItems", "OutputAssets", "SelfDefines", "UsedDefines" })
        {
            var values = SavedField(document, field) as System.Collections.IEnumerable;
            if (values != null) foreach (object value in values) if (value != null) records.Add(value);
        }
        return records.Count;
    }
    static void TransferMeasurement()
    {
        const int count = 32, transfers = 20, rounds = 7;
        var source = Bare();
        var outputs = new List2<OutputAsset>(); Set(source, "OutputAssets", outputs);
        for (int index = 0; index < count; index++)
        {
            string name = "Meta" + index;
            source.SelfDefines.Add(new Definition { Name = name, OriginalValue = "=1+2", EvaluatedValue = "3", Document = source });
            source.InclusionItems.Add(new InclusionItem("DATA:" + name + ".xml", name + ".xml", InclusionType.All) { Document = source });
            source.UsedDefines[name] = "3";
            outputs.Add(new OutputAsset { Handle = new InstanceHandle("ArmorTemplate", name), InstanceFileSize = index + 1 });
        }
        source.MakeCacheable(); string before = Xml(source);
        // XML, mutable restoration, identity enumeration and checks remain out
        // of the timed public cache-copy operation; no speed assertion.
        foreach (bool deepCopy in new[] { false, true })
        {
            AppContext.SetSwitch(ShareDisable, deepCopy); var precheck = TransferChain(source, 3);
            Require(precheck.All(document => Xml(document) == before), "Measurement precheck changed saved XML");
            precheck[2].FromLastHack(); precheck[2].SelfDefines[0].EvaluatedValue = "changed";
            Require(Xml(source) == before && Xml(precheck[0]) == before, "Measurement precheck mutated saved source");
        }
        var samples = new List<object>(); var shared = new List<long>(); var copied = new List<long>();
        for (int round = 0; round < rounds; round++)
        {
            bool[] order = round % 2 == 0 ? new[] { false, true } : new[] { true, false };
            for (int position = 0; position < order.Length; position++)
            {
                bool deepCopy = order[position]; AppContext.SetSwitch(ShareDisable, deepCopy);
                var timer = System.Diagnostics.Stopwatch.StartNew(); var chain = TransferChain(source, transfers); timer.Stop();
                int records = DistinctSavedRecords(new[] { source }.Concat(chain));
                Require(records == count * 4 * (deepCopy ? transfers + 1 : 1), "Observed private saved record count disagrees with selected path");
                Require(Xml(source) == before && Xml(chain[transfers - 1]) == before, "Measured transfer changed XML");
                (deepCopy ? copied : shared).Add(timer.ElapsedTicks);
                samples.Add(new { round, position, deepCopyFallback = deepCopy, elapsedTicks = timer.ElapsedTicks,
                    elapsedMs = timer.ElapsedTicks * 1000.0 / System.Diagnostics.Stopwatch.Frequency,
                    retainedDocuments = transfers + 1, observedPrivateSavedRecords = records });
            }
        }
        shared.Sort(); copied.Sort(); AppContext.SetSwitch(ShareDisable, false);
        evidence.Add(new { id = "bounded-public-metadata-cache-transfer-component-measurement", recordsPerKind = count, kinds = 4, transfersPerSample = transfers, rounds,
            stopwatchFrequency = System.Diagnostics.Stopwatch.Frequency, medianSharedTicks = shared[rounds / 2], medianDeepCopyTicks = copied[rounds / 2],
            medianSharedMs = shared[rounds / 2] * 1000.0 / System.Diagnostics.Stopwatch.Frequency,
            medianDeepCopyMs = copied[rounds / 2] * 1000.0 / System.Diagnostics.Stopwatch.Frequency, samples = samples.ToArray(),
            scope = "32 records of each of four metadata kinds; 20 public saved-cache transfers; seven alternating-order rounds. Timings include wrapper/container copies, exclude XML/Current/identity enumeration. No forced GC, speed assertion, CLI speedup or memory-peak claim" });
        tests.Add("bounded-public-metadata-transfer-measurement-with-saved-record-work-count");
    }
    public static int Main(string[] args)
    {
        object result; int code = 0; bool previous, previousShare; AppContext.TryGetSwitch(Disable, out previous); AppContext.TryGetSwitch(ShareDisable, out previousShare);
        try
        {
            Require(args.Length == 5 && IntPtr.Size == 4 && Path.DirectorySeparatorChar == '\\', "Expected Windows x86 work/result/source/run/SDK");
            AppContext.SetSwitch(Disable, false);
            foreach (bool deepCopy in new[] { false, true }) { AppContext.SetSwitch(ShareDisable, deepCopy); PublicLifecycle(); Aliases(); Fallbacks(); }
            SavedOwnership(); TransferMeasurement();
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), evidence = evidence.ToArray(),
                scope = "Document cache scalar metadata ownership; existing XML, DOM and document references remain; not immutable semantic results or parallel safety" };
            Console.WriteLine("PASS DocumentMetadata: " + tests.Count + " groups; " + checks + " checks");
        }
        catch (Exception error)
        {
            code = 1; Console.Error.WriteLine(error); result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null,
                runId = args.Length > 3 ? args[3] : null, checks, tests = tests.ToArray(), evidence = evidence.ToArray(), error = error.ToString() };
        }
        finally { AppContext.SetSwitch(Disable, previous); AppContext.SetSwitch(ShareDisable, previousShare); }
        Directory.CreateDirectory(Path.GetDirectoryName(args[1]));
        File.WriteAllText(args[1], new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue }.Serialize(result)); return code;
    }
}
