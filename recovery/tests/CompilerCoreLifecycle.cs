using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using NewModCompiler.Application;
using EALA.Metrics;

// Runs inside the independently linked production-object validation host.
// This fixture uses real SDK schemas, Core, recovered Armor and verifier code.
internal static class CompilerCoreLifecycle
{
    const string Ns = "uri:ea.com:eala:asset";
    static readonly List<string> Tests = new List<string>();
    static readonly List<object> OptimizationRisks = new List<object>();
    static readonly List<object> WorkMetrics = new List<object>();
    static string sdk, work, probe;
    static int checks;
    static readonly BindingFlags Private = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    static void Require(bool value, string message) { ++checks; if (!value) throw new Exception(message); }
    static object Field(object target, string name) { return typeof(DocumentProcessor).GetField(name, Private).GetValue(target); }
    static Stack<string> Documents { get { return (Stack<string>)Field(null, "_CurrentDocumentStack"); } }
    static Stack<string> Streams { get { return (Stack<string>)Field(null, "_CurrentStreamStack"); } }
    static IList Inclusions(DocumentProcessor p) { return (IList)Field(p, "_DocumentStack"); }
    static string Write(string relative, string contents)
    {
        string path = Path.GetFullPath(Path.Combine(work, relative)); Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, contents); return path;
    }
    static string Asset(string body) { return "<AssetDeclaration xmlns='" + Ns + "'>" + body + "</AssetDeclaration>"; }
    static string Include(string source) { return "<Includes><Include type='all' source='" + source + "'/></Includes>"; }
    static Settings Configure(string name)
    {
        var config = new XmlDocument(); config.Load(Path.Combine(sdk, "tools/BinaryAssetBuilder.exe.config"));
        var s = (Settings)new SerializedSectionHandler().Create(null, null, config.SelectSingleNode("/configuration/assetbuilder"));
        s.DataRoot = work; s.SchemaPath = Path.Combine(sdk, "Schemas/xsd/CnC3Types.xsd");
        s.InputPath = Path.Combine(work, name, "root.xml");
        s.OutputDirectory = Path.Combine(work, name, "output");
        s.IntermediateOutputDirectory = Path.Combine(work, name, "intermediate");
        s.SessionCacheDirectory = Path.Combine(work, name, "cache");
        s.DefaultDataPaths = work; s.DefaultArtPaths = work; s.DefaultAudioPaths = work;
        s.TargetPlatform = TargetPlatform.Win32; s.BuildCache = false; s.SessionCache = false;
        s.LinkedStreams = true; s.OutputAssetReport = false; s.OutputStringHashes = false;
        s.ErrorLevel = 1; s.UsePrecompiled = false; s.StreamHints = false; s.StableSort = false;
        Settings.Current = s; SettingsLoader.PostProcessSettings(work);
        Directory.CreateDirectory(s.OutputDirectory); Directory.CreateDirectory(s.IntermediateOutputDirectory);
        Directory.CreateDirectory(s.SessionCacheDirectory);
        return s;
    }
    static DocumentProcessor Processor(BuiltinCompilers plugins)
    {
        var schemas = new SchemaSet(Settings.Current.StableSort);
        HashProvider.InitializeStringHashes(Settings.Current.SessionCacheDirectory);
        var cache = new SessionCache(); cache.InitializeCache(new List<string>());
        var verifiers = new VerifierPluginRegistry(new PluginDescriptor[0], TargetPlatform.Win32);
        verifiers.DefaultPlugin = (IAssetBuilderVerifierPlugin)Activator.CreateInstance(
            typeof(DocumentProcessor).Assembly.GetType("BinaryAssetBuilder.GameDataVerifier.Plugin", true), true);
        verifiers.DefaultPlugin.Initialize(null, TargetPlatform.Win32);
        return new DocumentProcessor(Settings.Current, plugins.Registry, verifiers) { Cache = cache, SchemaSet = schemas };
    }
    static void Run(DocumentProcessor p, string input)
    {
        bool success; p.ProcessDocument(input, true, false, out success); Require(success, "Core did not report success: " + input);
    }
    static void Failure(DocumentProcessor p, string input, ErrorCode expected)
    {
        try { Run(p, input); }
        catch (BinaryAssetBuilderException e) { Require(e.ErrorCode == expected, "Expected " + expected + ", got " + e.ErrorCode + ": " + e); return; }
        throw new Exception("Core accepted rejected input: " + input);
    }
    static void Clean(DocumentProcessor p, string configuration, params string[] paths)
    {
        Require(Documents.Count == 0 && Streams.Count == 0 && Inclusions(p).Count == 0, "Core traversal frames leaked");
        Require(!(bool)Field(p, "_CompilingProject"), "Project flag leaked");
        Require(Field(p, "_ProjectDefaultConfigurations") == null, "Project configuration table leaked");
        foreach (string path in paths)
        {
            AssetDeclarationDocument document;
            p.Cache.TryGetDocument(Path.GetFullPath(path), configuration, TargetPlatform.Win32, false, out document);
            if (document != null) Require(!document.Processing, "Processing flag leaked: " + path);
        }
    }
    static Dictionary<string, string> Outputs(string root)
    {
        var result = new Dictionary<string, string>();
        foreach (string path in Directory.GetFiles(root, "*", SearchOption.AllDirectories))
        {
            string extension = Path.GetExtension(path);
            if (extension != ".manifest" && extension != ".bin" && extension != ".imp" && extension != ".relo") continue;
            using (var sha = SHA256.Create()) result.Add(path.Substring(root.Length), BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path))));
        }
        return result;
    }
    static void Equal(Dictionary<string, string> expected, Dictionary<string, string> actual)
    {
        Require(expected.Count == actual.Count && expected.Count == 4, "Expected four linked stream output files");
        foreach (var pair in expected) Require(actual.ContainsKey(pair.Key) && actual[pair.Key] == pair.Value, "Output changed: " + pair.Key);
    }
    static void RetryCase(BuiltinCompilers plugins, string name, bool inheritance)
    {
        var s = Configure(name); var p = Processor(plugins);
        string root = Write(name + "/root.xml", Asset(Include("middle.xml")));
        string middle = Write(name + "/middle.xml", Asset(Include("leaf.xml")));
        string leaf = Path.Combine(work, name, "leaf.xml");
        if (inheritance) Write(name + "/leaf.xml", Asset("<ArmorTemplate id='Broken' inheritFrom='MissingParent'/>"));
        // Seed pre-existing frames to ensure this call never clears a caller's state.
        Documents.Push("parent-document"); Streams.Push("parent-stream"); Inclusions(p).Add("parent-inclusion");
        try
        {
            Failure(p, root, inheritance ? ErrorCode.InheritFromError : ErrorCode.FileNotFound);
            Require(Documents.Count == 1 && Documents.Peek() == "parent-document", "Document parent frame lost");
            Require(Streams.Count == 1 && Streams.Peek() == "parent-stream", "Stream parent frame lost");
            Require(Inclusions(p).Count == 1 && (string)Inclusions(p)[0] == "parent-inclusion", "Inclusion parent frame lost");
        }
        finally { Documents.Pop(); Streams.Pop(); Inclusions(p).RemoveAt(0); }
        Clean(p, null, root, middle, leaf);
        AssetDeclarationDocument failed; p.Cache.TryGetDocument(root, null, TargetPlatform.Win32, false, out failed);
        Require(failed.State != DocumentState.Complete, "Failed root was completed/cached as successful");
        Write(name + "/leaf.xml", probe);
        Run(p, root); Clean(p, null, root, middle, leaf);
        Dictionary<string, string> repaired = Outputs(s.OutputDirectory);
        // Repeat a completed stream: early return must neither leak frames nor change output.
        Run(p, root); Clean(p, null, root, middle, leaf); Equal(repaired, Outputs(s.OutputDirectory));
        // New root includes an already complete document with GenerateOutput=false.
        string reused = Write(name + "/reuse.xml", Asset(Include("leaf.xml")));
        Run(p, reused); Clean(p, null, reused, leaf);
        // Cold execution uses the same source paths, a fresh real cache and clean output roots.
        string oldOutput = s.OutputDirectory, oldIntermediate = s.IntermediateOutputDirectory;
        s.OutputDirectory = Path.Combine(work, name, "cold-output");
        s.IntermediateOutputDirectory = Path.Combine(work, name, "cold-intermediate");
        var cold = Processor(plugins); Run(cold, root); Clean(cold, null, root, middle, leaf);
        Equal(repaired, Outputs(s.OutputDirectory)); s.OutputDirectory = oldOutput; s.IntermediateOutputDirectory = oldIntermediate;
        Tests.Add(name + ":real-core-failure-same-processor-cache-retry-parent-frames-complete-reuse-cold-output-equality");
    }
    static void CycleCase(BuiltinCompilers plugins)
    {
        Configure("cycle"); var p = Processor(plugins);
        string a = Write("cycle/root.xml", Asset(Include("b.xml")));
        string b = Write("cycle/b.xml", Asset(Include("root.xml")));
        Failure(p, a, ErrorCode.CircularDependency); Clean(p, null, a, b);
        Write("cycle/b.xml", probe); Run(p, a); Clean(p, null, a, b);
        Tests.Add("real-core-circular-include-rejected-then-fixed-same-processor-cache");
    }
    static void InheritanceExecutionCase(BuiltinCompilers plugins, int length)
    {
        string name = "inheritance-cycle-" + length;
        var settings = Configure(name); var processor = Processor(plugins);
        var body = new System.Text.StringBuilder();
        for (int i = 0; i < length; i++)
            body.Append("<ArmorTemplate id='Node" + i + "' inheritFrom='Node" + ((i + 1) % length) + "'/>");
        string input = Write(name + "/root.xml", Asset(body.ToString()));
        Failure(processor, input, ErrorCode.CircularDependency);
        Clean(processor, null, input);
        Require(CachedDocument(processor, input).State != DocumentState.Complete,
            "Circular inheritance published a successful document");
        // Repair the same source and reuse the processor after the failed
        // partial DFS. Its visiting state must belong to that invocation only.
        string chain = "<ArmorTemplate id='First' inheritFrom='Middle'/>" +
            "<ArmorTemplate id='Middle' inheritFrom='Last'/>" +
            "<ArmorTemplate id='Last' Default='10'/>";
        Write(name + "/root.xml", Asset(chain)); Run(processor, input); Clean(processor, null, input);
        var inherited = Outputs(settings.OutputDirectory);
        Require(CachedDocument(processor, input).SelfInstances.Count == 3, "Repaired forward inheritance chain lost a declaration");
        // The fully expanded declaration is an independent semantic oracle for
        // these four actual production outputs; no digest is used as readiness.
        settings.OutputDirectory = Path.Combine(work, name, "expanded-output");
        settings.IntermediateOutputDirectory = Path.Combine(work, name, "expanded-intermediate");
        Write(name + "/root.xml", Asset("<ArmorTemplate id='First' Default='10'/>" +
            "<ArmorTemplate id='Middle' Default='10'/><ArmorTemplate id='Last' Default='10'/>"));
        var expanded = Processor(plugins); Run(expanded, input); Clean(expanded, null, input);
        Equal(inherited, Outputs(settings.OutputDirectory));
        Tests.Add(name + ":bounded-cycle-error-no-completion-same-processor-repair-forward-chain-expanded-four-output-equality");
    }
    static void ZeroInheritanceDigestCase(BuiltinCompilers plugins)
    {
        Configure("inheritance-zero-digest"); var processor = Processor(plugins);
        var document = new AssetDeclarationDocument();
        var type = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object current = Activator.CreateInstance(type, true); type.GetMethod("FromScratch").Invoke(current, new object[0]);
        type.GetField("SourcePath").SetValue(current, Path.Combine(work, "inheritance-zero-digest/root.xml"));
        type.GetField("DocumentProcessor").SetValue(current, processor);
        var xml = new XmlDocument(); xml.LoadXml(Asset("<ArmorTemplate id='First' inheritFrom='Last'/>" +
            "<ArmorTemplate id='Second' inheritFrom='Last'/><ArmorTemplate id='Last' Default='10'/>"));
        type.GetField("XmlDocument").SetValue(current, xml);
        typeof(AssetDeclarationDocument).GetField("_Current", Private).SetValue(document, current);
        foreach (XmlNode node in xml.DocumentElement.ChildNodes)
            document.SelfInstances.Add(new InstanceDeclaration(document) { XmlNode = node });
        InstanceDeclaration source = document.SelfInstances[2], consumer = document.SelfInstances[1];
        bool injected = false;
        xml.NodeInserted += (sender, change) => {
            if (change.NewParent != xml.DocumentElement || change.Node.Attributes == null ||
                change.Node.Attributes["id"] == null || change.Node.Attributes["id"].Value != "First") return;
            // Supply a zero-valued completed digest at the actual boundary. This
            // tests readiness independently of the hash algorithm; it does not
            // claim to have found a real XML FastHash collision/preimage.
            source.PrevalidationXmlHash = 0; injected = true;
        };
        document.ProcessOverrides();
        Require(injected && consumer.InheritFromXmlHash == 0,
            "A completed zero digest was treated as an unfinished inheritance source");
        Require(consumer.XmlNode.Attributes["Default"].Value == "10", "Zero digest source payload was not consumed");
        Require(source.PrevalidationXmlHash != 0, "Legacy top-level source hash/event order was skipped");
        Tests.Add("inheritance-zero-digest:actual-merge-boundary-injection-source-ready-with-zero-legacy-hash-root-order-preserved");
    }
    sealed class SaveFaultCache : SessionCache
    {
        public bool Failed;
        public override void SaveDocumentToCache(string path, string configuration, TargetPlatform platform, AssetDeclarationDocument document)
        {
            if (!Failed) { Failed = true; throw new IOException("Injected real-cache save failure"); }
            base.SaveDocumentToCache(path, configuration, platform, document);
        }
    }
    static void SaveFailureCase(BuiltinCompilers plugins)
    {
        Configure("cache-save"); var p = Processor(plugins);
        string input = Write("cache-save/root.xml", probe);
        var fault = new SaveFaultCache(); fault.InitializeCache(new List<string>()); p.Cache = fault;
        bool rejected = false;
        try { Run(p, input); }
        catch (IOException e) { rejected = e.Message == "Injected real-cache save failure"; }
        Require(rejected && fault.Failed, "SaveDocumentToCache fault was not reached/propagated");
        Clean(p, null, input);
        // Cache persistence failure is not acceptance of its half-committed document state.
        // Production must discard this cache; verify the same processor can run with fresh real state.
        var fresh = new SessionCache(); fresh.InitializeCache(new List<string>()); p.Cache = fresh;
        Run(p, input); Clean(p, null, input);
        Tests.Add("real-cache-save-fault-propagates-cleans-frames-then-same-processor-fresh-cache-success");
    }
    sealed class CompletionMetricFault : IMetricsListener
    {
        internal bool FailFinal, FailTimer;
        internal readonly Exception FinalFailure = new IOException("Injected final BuildSuccessful metric failure");
        internal readonly Exception TimerFailure = new IOException("Injected ProcessingTime Dispose metric failure");
        internal int FinalCalls, TimerCalls;
        public void Open() { }
        public void Close() { }
        public void SubmitMetrics(Metric metric)
        {
            if (metric.Name == "BAB.BuildSuccessful")
            {
                FinalCalls++;
                if (FailFinal) throw FinalFailure;
            }
            if (metric.Name == "BAB.ProcessingTime")
            {
                TimerCalls++;
                if (FailTimer) throw TimerFailure;
            }
        }
    }
    static void MetricsFailureCase(BuiltinCompilers plugins, bool failFinal, bool failTimer)
    {
        const string finalKey = "BinaryAssetBuilder.ProcessDocument.FinalMetricsFailure";
        const string timerKey = "BinaryAssetBuilder.ProcessDocument.ProcessingTimeFailure";
        string name = "metrics-failure-" + failFinal + "-" + failTimer;
        Configure(name); var p = Processor(plugins);
        string bad = Write(name + "/bad.xml", Asset("<ArmorTemplate id='BadMetrics' inheritFrom='MissingParent'/>"));
        string good = Write(name + "/good.xml", probe);
        MetricManager.GetDescriptor("BAB.BuildSuccessful", MetricType.Success, "Build successful");
        MetricManager.GetDescriptor("BAB.ProcessingTime", MetricType.Duration, "Processing time");
        var listener = new CompletionMetricFault { FailFinal = failFinal, FailTimer = failTimer };
        bool oldEnabled = MetricManager.Enabled, opened = false;
        MetricManager.AddListener(listener);
        try
        {
            MetricManager.OpenSession(); opened = true;
            BinaryAssetBuilderException primary = null;
            bool success = false;
            try { p.ProcessDocument(bad, true, false, out success); }
            catch (BinaryAssetBuilderException error) { primary = error; }
            Require(primary != null && primary.ErrorCode == ErrorCode.InheritFromError && !success,
                "Completion metrics replaced the real inheritance failure");
            Require(primary.StackTrace != null && primary.StackTrace.Contains("ProcessDocumentInternal"),
                "Original real Core compilation stack was not preserved");
            Require(primary.Data.Contains(finalKey) == failFinal && primary.Data.Contains(timerKey) == failTimer,
                "Secondary metrics error attachments are missing or unexpected");
            if (failFinal) Require(Object.ReferenceEquals(primary.Data[finalKey], listener.FinalFailure),
                "Final metrics secondary exception identity was lost");
            if (failTimer) Require(Object.ReferenceEquals(primary.Data[timerKey], listener.TimerFailure),
                "MetricTimer.Dispose secondary exception identity was lost");
            Clean(p, null, bad);

            // Without a compilation failure, keep the metrics exception as the
            // primary error. A later Dispose error must not replace it either.
            Exception metricFailure = null; success = false;
            try { p.ProcessDocument(good, true, false, out success); }
            catch (Exception error) { metricFailure = error; }
            Require(success && Object.ReferenceEquals(metricFailure, failFinal ? listener.FinalFailure : listener.TimerFailure),
                "Successful compilation swallowed or replaced its metrics failure");
            if (failFinal && failTimer)
                Require(Object.ReferenceEquals(metricFailure.Data[timerKey], listener.TimerFailure),
                    "Timer failure replaced the first successful-build metrics error");
            Require(listener.FinalCalls == 2 && listener.TimerCalls == 2, "Completion metric fault paths were not both reached");
            Clean(p, null, bad, good);
        }
        finally
        {
            try { if (opened) MetricManager.CloseSession(); }
            finally
            {
                Require(MetricManager.RemoveListener(listener), "Fault listener was not removable after completion failure");
                MetricManager.Enabled = oldEnabled;
            }
        }
        string retry = Write(name + "/retry.xml", probe);
        Run(p, retry); Clean(p, null, bad, good, retry);
        Tests.Add(name + ":real-inheritance-primary-and-stack-preserved-secondary-errors-attached-metrics-primary-on-success-listener-removed-retry-success");
    }
    static void VerifyProjectOutput(Settings settings)
    {
        string prefix = Path.Combine(settings.OutputDirectory, "project", "root_G");
        var expected = new HashSet<uint>();
        var source = new XmlDocument(); source.LoadXml(probe);
        foreach (XmlNode child in source.DocumentElement.ChildNodes)
            if (child is XmlElement) expected.Add(InstanceHandle.GetInstanceId(((XmlElement)child).GetAttribute("id")));
        using (var manifest = new BinaryAssetBuilder.Utility.Manifest())
        {
            // Linked payloads are the three stream files, not loose .asset files.
            Require(manifest.Load(prefix + ".manifest", new[] { settings.OutputDirectory }, false), "Good project manifest did not load");
            Require(manifest.Assets.Length == expected.Count && expected.Count == 3, "Good project manifest asset set incomplete");
            int instance = 4, relocation = 4, imports = 4;
            foreach (var asset in manifest.Assets)
            {
                Require(asset.TypeId == InstanceHandle.GetTypeId("ArmorTemplate") && expected.Remove(asset.InstanceId),
                    "Good project manifest contains an unexpected/duplicate asset");
                Require(asset.InstanceDataSize > 0 && asset.LinkedInstanceOffset == instance &&
                    asset.LinkedRelocationOffset == relocation && asset.LinkedImportsOffset == imports,
                    "Good project manifest has invalid linked payload sizes/offsets");
                instance += asset.InstanceDataSize; relocation += asset.RelocationDataSize; imports += asset.ImportsDataSize;
            }
            Require(expected.Count == 0 && manifest.TotalInstanceDataSize == instance - 4, "Good project manifest omitted probe payloads");
            string[] extensions = { ".bin", ".relo", ".imp" }; int[] sizes = { instance, relocation, imports };
            for (int i = 0; i < extensions.Length; i++)
            {
                byte[] bytes = File.ReadAllBytes(prefix + extensions[i]);
                Require(bytes.Length == sizes[i] && BitConverter.ToUInt32(bytes, 0) == manifest.StreamChecksum,
                    "Good project linked stream size/checksum does not match its manifest: " + extensions[i]);
            }
        }
    }
    static readonly string[] ProcessorStateFields = {
        "_MissingReferences", "_CurrentDocumentStack", "_CurrentStreamStack", "_TypeProcessingTime",
        "_TotalProcInstancesTime", "_TotalPrepareOutputTime", "_TotalPrepareSourceTime", "_TotalPostProcTime", "_TotalValidateTime"
    };
    static object[] ProcessorState() { return ProcessorStateFields.Select(name => Field(null, name)).ToArray(); }
    static void SameProcessorState(object[] expected)
    {
        object[] actual = ProcessorState();
        for (int i = 0; i < actual.Length; i++)
            Require(i < 4 ? Object.ReferenceEquals(actual[i], expected[i]) : Object.Equals(actual[i], expected[i]),
                "Processor state did not restore " + ProcessorStateFields[i]);
    }
    static void EmptyProcessorState(object[] parent)
    {
        object[] actual = ProcessorState();
        for (int i = 0; i < 4; i++) Require(!Object.ReferenceEquals(actual[i], parent[i]), "Processor child state reused " + ProcessorStateFields[i]);
        Require(((InstanceHandleSet)actual[0]).Count == 0 && Documents.Count == 0 && Streams.Count == 0 &&
            ((IDictionary)actual[3]).Count == 0, "Processor scope did not isolate reference/traversal/type tables");
        for (int i = 4; i < actual.Length; i++) Require((TimeSpan)actual[i] == TimeSpan.Zero, "Processor scope inherited a parent timer");
    }
    static void RejectProcessorState(Action action, string message)
    {
        bool rejected = false; try { action(); } catch (InvalidOperationException) { rejected = true; }
        Require(rejected, message);
    }
    static void RejectProcessorStateWorker(Action action, string message)
    {
        Exception failure = null;
        var thread = new Thread(delegate() { try { action(); } catch (Exception error) { failure = error; } });
        thread.IsBackground = true; thread.Start();
        Require(thread.Join(5000), message + " (worker blocked)");
        Require(failure is InvalidOperationException, message);
    }
    static void ProcessorStateCase(BuiltinCompilers plugins)
    {
        Settings oldSettings = Settings.Current;
        Configure("processor-state-parent"); var parentProcessor = Processor(plugins);
        object[] original = ProcessorState();
        try
        {
            using (DocumentProcessor.BeginStateScope())
            {
                var missing = new InstanceHandle("ArmorTemplate", "SameMissingReferenceAcrossBuilds");
                Require(parentProcessor.MissingReferences.TryAdd(missing), "Parent missing-reference sentinel duplicate");
                Documents.Push("state-parent-document"); Streams.Push("state-parent-stream");
                for (int i = 4; i < ProcessorStateFields.Length; i++)
                    typeof(DocumentProcessor).GetField(ProcessorStateFields[i], Private).SetValue(null, TimeSpan.FromTicks(100 + i));
                parentProcessor.AddCompileTime(missing, TimeSpan.FromTicks(9876));
                var parentTypes = (IDictionary)Field(null, "_TypeProcessingTime");
                object parentType = parentTypes[missing.TypeId];
                object[] parent = ProcessorState();
                for (int round = 0; round < 2; round++)
                {
                    using (var child = DocumentProcessor.BeginStateScope())
                    {
                        EmptyProcessorState(parent);
                        string name = "processor-state-child-" + round;
                        Configure(name); var processor = Processor(plugins);
                        Require(processor.MissingReferences.TryAdd(new InstanceHandle(missing)) &&
                            !processor.MissingReferences.TryAdd(new InstanceHandle(missing)),
                            "Missing-reference warning deduplication leaked across processor scopes");
                        string bad = Write(name + "/bad.xml", Asset("<ArmorTemplate id='StateBad' inheritFrom='MissingParent'/>"));
                        string good = Write(name + "/good.xml", probe);
                        Failure(processor, bad, ErrorCode.InheritFromError); Clean(processor, null, bad);
                        Run(processor, good); Clean(processor, null, bad, good);
                        object[] childState = ProcessorState();
                        var nested = DocumentProcessor.BeginStateScope();
                        try
                        {
                            EmptyProcessorState(childState);
                            Require(processor.MissingReferences.TryAdd(new InstanceHandle(missing)), "Nested scope retained missing-reference deduplication");
                            RejectProcessorState(child.Dispose, "Out-of-order processor scope disposal accepted");
                            RejectProcessorStateWorker(nested.Dispose, "Foreign thread disposed processor scope");
                            RejectProcessorStateWorker(delegate { using (DocumentProcessor.BeginStateScope()) { } }, "Foreign thread entered processor scope");
                        }
                        finally { nested.Dispose(); }
                        nested.Dispose(); SameProcessorState(childState);
                        Require(processor.MissingReferences.Count == 1 && !processor.MissingReferences.TryAdd(new InstanceHandle(missing)),
                            "Nested processor scope did not restore its parent's warning set");
                    }
                    SameProcessorState(parent);
                    Require(Documents.Count == 1 && Documents.Peek() == "state-parent-document" &&
                        Streams.Count == 1 && Streams.Peek() == "state-parent-stream", "Child processor consumed parent traversal sentinels");
                    Require(parentProcessor.MissingReferences.Count == 1 && !parentProcessor.MissingReferences.TryAdd(new InstanceHandle(missing)),
                        "Child processor changed the parent missing-reference set");
                    Require(parentTypes.Count == 1 && Object.ReferenceEquals(parentTypes[missing.TypeId], parentType) &&
                        (uint)parentType.GetType().GetField("InstancesProcessed").GetValue(parentType) == 1 &&
                        (TimeSpan)parentType.GetType().GetField("TotalProcessTime").GetValue(parentType) == TimeSpan.FromTicks(9876),
                        "Child compile times mutated the parent's existing type entry");
                }
            }
            SameProcessorState(original);
            Tests.Add("processor-scopes:real-core-failure-success-warning-dedup-fresh-per-build-parent-reference-stack-five-timers-type-entries-restored-nested-thread-lifo-guards");
        }
        finally { Settings.Current = oldSettings; }
    }
    static void UnicodePathCase()
    {
        const string ascii = @"C:\Sdk Root\Data\Mod.xml";
        Require(ShPath.Canonicalize(ascii) == ascii, "ASCII canonical path changed");
        Require(ShPath.Canonicalize(@"C:\Sdk Root\Data\.\Child\..\Mod.xml") == ascii,
            "ASCII dot-segment canonicalization changed");
        Require(ShPath.RemoveFileSpec(ascii) == @"C:\Sdk Root\Data", "ASCII parent directory changed");
        const string unicode = @"C:\SDK 中文\Mods\外部 MOD\Data\模组 文档.xml";
        Require(ShPath.Canonicalize(unicode) == unicode, "Unicode canonical path changed under the process ANSI code page");
        string normalized = ShPath.Canonicalize(@"C:\SDK 中文\Mods\外部 MOD\Data\.\贴图\..\模组 文档.xml");
        Require(normalized == unicode, "Unicode/space/dot-segment path was damaged");
        Require(ShPath.RemoveFileSpec(unicode) == @"C:\SDK 中文\Mods\外部 MOD\Data" &&
            ShPath.RemoveFileSpec(normalized) == @"C:\SDK 中文\Mods\外部 MOD\Data",
            "Unicode/space parent directory was damaged");
        Tests.Add("Win32-shell-W-paths:ASCII-unchanged-Unicode-space-dot-segments-and-parent-preserved-existing-MAX_PATH-contract-only");
    }
    static void ProjectCase(BuiltinCompilers plugins)
    {
        Settings s = Configure("project");
        string good = Path.Combine(work, "project/good"), bad = Path.Combine(work, "project/bad");
        Write("project/good/leaf.xml", probe);
        Write("project/bad/leaf.xml", Asset("<ArmorTemplate id='BadConfig' inheritFrom='MissingParent'/>"));
        s.BuildConfigurations = new[] {
            new BuildConfiguration { Name="Good", Postfix="G", DataPaths=work+";"+good },
            new BuildConfiguration { Name="Bad", Postfix="B", DataPaths=work+";"+bad }
        };
        var p = Processor(plugins);
        string root = Write("project/root.xml", Asset(Include("data:leaf.xml")));
        string project = Write("project/test.babproj", "<BinaryAssetBuilderProject xmlns='urn:xmlns:ea.com:babproject'><Stream Source='root.xml'><Configuration Name='Good' Default='true'/><Configuration Name='Bad'/></Stream></BinaryAssetBuilderProject>");
        Failure(p, project, ErrorCode.InheritFromError);
        Require(Object.ReferenceEquals(Settings.Current, s), "Settings.Current not restored after project failure");
        Require(s.BuildConfigurationName == null && s.Postfix == null && s.StreamPostfix == null, "Original Settings values mutated");
        Clean(p, "Good", root, Path.Combine(good, "leaf.xml"));
        Clean(p, "Bad", root, Path.Combine(bad, "leaf.xml"));
        // The project normalizes Source through GetFullPath, while SessionCache
        // keys only lowercase paths. Write returns the same canonical form;
        // a mixed '/' query otherwise misses an existing '\\' Windows cache key.
        AssetDeclarationDocument first; p.Cache.TryGetDocument(root, "Good", TargetPlatform.Win32, false, out first);
        Require(first != null && first.State == DocumentState.Complete,
            "First project configuration cache state incorrect: " + root + " state=" + (first == null ? "missing" : first.State.ToString()));
        VerifyProjectOutput(s);
        using (File.Open(project, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
        string normal = Write("project/normal.xml", probe); Run(p, normal); Clean(p, null, normal);
        Tests.Add("real-project-first-config-complete-three-assets-four-linked-streams-second-inheritance-failure-restores-settings-reader-then-plain-xml-success");
    }
    // W01 records the actual public Core boundary before changing cache or
    // binding semantics. A reproduced candidate is evidence, not acceptance of
    // a production CLI bug: the CLI has its own staging/cache protections.
    static bool SameOutputs(Dictionary<string, string> expected, Dictionary<string, string> actual)
    {
        return expected.Count == actual.Count && expected.All(pair => actual.ContainsKey(pair.Key) && actual[pair.Key] == pair.Value);
    }
    static AssetDeclarationDocument CachedDocument(DocumentProcessor processor, string path)
    {
        AssetDeclarationDocument document;
        Require(processor.Cache.TryGetDocument(path, null, TargetPlatform.Win32, false, out document) && document != null,
            "Probe document missing from real Core cache: " + path);
        return document;
    }
    static DocumentProcessor PersistentProcessor(BuiltinCompilers plugins, string cachePath)
    {
        var processor = Processor(plugins);
        var cache = new SessionCache(); cache.LoadCache(cachePath); cache.InitializeCache(new List<string>());
        processor.Cache = cache;
        return processor;
    }
    static void DefineOverrideProbe(BuiltinCompilers plugins, bool increment)
    {
        string name = "risk-define-" + (increment ? "increment" : "identity");
        var settings = Configure(name);
        string input = Write(name + "/root.xml", Asset(Include("external.xml") +
            "<Defines><Define name='A' value='=$A" + (increment ? "+1" : "") + "' override='true'/></Defines>" +
            "<ArmorTemplate id='OverrideProbe' Default='=$A'/>"));
        string external = Write(name + "/external.xml", Asset("<Defines><Define name='A' value='10'/></Defines>"));
        string cachePath = Path.Combine(settings.SessionCacheDirectory, "probe.xml");
        var first = PersistentProcessor(plugins, cachePath); Run(first, input);
        Require(CachedDocument(first, input).AllDefines.GetEvaluatedValue("A") == (increment ? "11" : "10"),
            "Cold Define override did not read the external pre-override value");
        var expectedInitial = Outputs(settings.OutputDirectory); first.Cache.SaveCache(false);
        var warm = PersistentProcessor(plugins, cachePath); Run(warm, input);
        var unmodified = Outputs(settings.OutputDirectory);
        int unmodifiedParsed = (int)Field(warm, "_FilesParsedCount");
        string unmodifiedValue = CachedDocument(warm, input).AllDefines.GetEvaluatedValue("A");
        // Keep the original persisted generation: the root source stays byte
        // identical, and only the included definition changes from 10 to 20.
        byte[] rootBefore = File.ReadAllBytes(input);
        File.WriteAllText(external, Asset("<Defines><Define name='A' value='20'/></Defines>"));
        var changed = PersistentProcessor(plugins, cachePath); Run(changed, input);
        string restoredValue = CachedDocument(changed, input).AllDefines.GetEvaluatedValue("A");
        var restored = Outputs(settings.OutputDirectory);
        Require(rootBefore.SequenceEqual(File.ReadAllBytes(input)), "Define probe changed the parent source");
        settings.OutputDirectory = Path.Combine(work, name, "cold-output");
        settings.IntermediateOutputDirectory = Path.Combine(work, name, "cold-intermediate");
        var cold = Processor(plugins); Run(cold, input);
        string coldValue = CachedDocument(cold, input).AllDefines.GetEvaluatedValue("A");
        Require(coldValue == (increment ? "21" : "20"), "Changed cold Define override value incorrect");
        OptimizationRisks.Add(new { id = name, boundary = "public-Core-persisted-session", initialValue = increment ? "11" : "10",
            unmodifiedValue, unmodifiedEquivalent = SameOutputs(expectedInitial, unmodified), unmodifiedParsed, unmodifiedWork = warm.GetWorkMetrics(),
            restoredValue, coldValue, changedEquivalent = SameOutputs(restored, Outputs(settings.OutputDirectory)),
            correctnessMismatch = restoredValue != coldValue || !SameOutputs(restored, Outputs(settings.OutputDirectory)),
            repeatedInvalidation = unmodifiedParsed > 0,
            reproduced = restoredValue != coldValue || !SameOutputs(restored, Outputs(settings.OutputDirectory)) || unmodifiedParsed > 0,
            cliFailureEstablished = false });
        Require(SameOutputs(expectedInitial, unmodified) && unmodifiedValue == (increment ? "11" : "10"),
            "Unmodified persisted override must preserve its original value and four outputs");
        Require(unmodifiedParsed == 0, "Unmodified persisted override reparsed a document unnecessarily");
        Require(restoredValue == coldValue && SameOutputs(restored, Outputs(settings.OutputDirectory)),
            "External Define change must match the fresh cold value and four outputs");
        Tests.Add(name + ":real-Core-persisted-cache-unchanged-no-reparse-external-change-cold-warm-equality");
    }
    static string Experience(string id, string prerequisite, bool upgrade)
    {
        return "<ExperienceLevelTemplate id='" + id + "'" + (prerequisite == null ? "" : " Prerequisite='" + prerequisite + "'") +
            "><LevelUpTintColor r='1.0' g='1.0' b='1.0'/>" + (upgrade ? "<Upgrade>BindingUpgrade</Upgrade>" : "") + "</ExperienceLevelTemplate>";
    }
    static void CrossStreamBindingProbe(BuiltinCompilers plugins)
    {
        const string name = "risk-stream-binding";
        var settings = Configure(name); var processor = Processor(plugins);
        string shared = Write(name + "/shared.xml", Asset(Experience("BindingOwner", null, true)));
        Write(name + "/candidate.xml", Asset("<UpgradeTemplate id='BindingUpgrade'/>"));
        string first = Write(name + "/first.xml", Asset(Include("shared.xml")));
        string second = Write(name + "/second.xml", Asset("<Includes><Include type='all' source='shared.xml'/>" +
            "<Include type='instance' source='candidate.xml'/></Includes>"));
        Run(processor, first);
        var snapshot = processor.GetWorkMetrics();
        var snapshotValues = snapshot.ToDictionary(item => item.stage, item => new[] { (double)item.calls, (double)item.failedCalls, item.elapsedMs });
        foreach (string stage in new[] { "rawParse", "definitions", "expressions", "inheritance", "validation", "assetProcessing", "referenceBinding", "manifest", "link" })
            Require(snapshot.Any(item => item.stage == stage && item.calls > 0 && item.failedCalls == 0 && item.elapsedMs >= 0 && !Double.IsInfinity(item.elapsedMs)),
                "Cold Core work metric missing/invalid: " + stage);
        var before = CachedDocument(processor, shared).SelfInstances[0];
        Require(before.WeakReferencedInstances.Count == 1 && CachedDocument(processor, first).GetStreamBindings().assets.Single(row => row.identity.instanceName == "BindingOwner").references.Count == 0,
            "First stream must leave the weak Upgrade reference unresolved");
        Run(processor, second);
        foreach (var item in snapshot)
            Require(item.calls == snapshotValues[item.stage][0] && item.failedCalls == snapshotValues[item.stage][1] && item.elapsedMs == snapshotValues[item.stage][2],
                "A previously returned work-metric snapshot changed after another stream");
        Require(processor.GetWorkMetrics().Single(item => item.stage == "link").calls > snapshot.Single(item => item.stage == "link").calls,
            "The next stream must record its own additional link work");
        var after = CachedDocument(processor, shared).SelfInstances[0];
        var warmDocument = CachedDocument(processor, second);
        var warm = Outputs(settings.OutputDirectory).Where(pair => pair.Key.Contains("second")).ToDictionary(pair => pair.Key, pair => pair.Value);
        Require(warm.Count == 4, "Second binding stream did not produce four files");
        bool warmContainsUpgrade = warmDocument.OutputInstances.Any(instance => instance.Handle.InstanceName == "BindingUpgrade");
        settings.OutputDirectory = Path.Combine(work, name, "cold-output");
        settings.IntermediateOutputDirectory = Path.Combine(work, name, "cold-intermediate");
        var cold = Processor(plugins); Run(cold, second);
        var coldDocument = CachedDocument(cold, second);
        Require(coldDocument.OutputInstances.Any(instance => instance.Handle.InstanceName == "BindingUpgrade"),
            "Cold stream must pull the weakly referenced Tentative upgrade into its closure");
        var expected = Outputs(settings.OutputDirectory);
        OptimizationRisks.Add(new { id = name, boundary = "public-Core-two-streams-one-processor", sharedInstanceRetained = Object.ReferenceEquals(before, after),
            warmContainsUpgrade, warmAssets = warmDocument.OutputInstances.Count, coldAssets = coldDocument.OutputInstances.Count,
            equivalent = SameOutputs(expected, warm), reproduced = !SameOutputs(expected, warm), cliFailureEstablished = false });
        Require(warmContainsUpgrade && warmDocument.GetStreamBindings().assets.Single(row => row.identity.instanceName == "BindingOwner").references.Single().instanceName == "BindingUpgrade" && SameOutputs(expected, warm),
            "Each stream must rebind the declaration's weak references in its own symbol view");
        Tests.Add("risk-stream-binding:real-Core-two-symbol-views-weak-missing-to-Tentative-cold-warm-equality");
    }
    static void WorkMetricsProbe(BuiltinCompilers plugins)
    {
        const string name = "work-metrics";
        var settings = Configure(name); var processor = Processor(plugins); var schemas = processor.SchemaSet;
        var preparation = schemas.GetPreparationMetrics();
        var values = preparation.ToDictionary(item => item.stage, item => new[] { (double)item.calls, (double)item.failedCalls, item.elapsedMs });
        foreach (string stage in new[] { "schemaRead", "schemaCompile", "schemaMetadata" })
            Require(preparation.Any(item => item.stage == stage && item.calls == 1 && item.failedCalls == 0 && item.elapsedMs >= 0 && !Double.IsInfinity(item.elapsedMs)),
                "Schema preparation metric missing/invalid: " + stage);
        long filesRead = schemas.SchemaFileReadCount, bytesRead = schemas.SchemaContentBytesRead;
        Require(filesRead > 0 && bytesRead > 0 && schemas.SchemaContentCheckCount == 0,
            "Initial schema preparation must report its actual input reads");
        schemas.ReloadIfChanged(settings.StableSort);
        var checkedPreparation = schemas.GetPreparationMetrics();
        Require(checkedPreparation.Single(item => item.stage == "schemaContentCheck").calls == 1 &&
            checkedPreparation.Single(item => item.stage == "schemaContentCheck").failedCalls == 0,
            "Unchanged schema reload must record a content-check interval");
        Require(schemas.SchemaFileReadCount == filesRead && schemas.SchemaContentCheckCount == filesRead && schemas.SchemaContentBytesRead == 2 * bytesRead,
            "Unchanged schema reload must recheck the whole closure without reparsing");
        Require(checkedPreparation.Single(item => item.stage == "schemaRead").calls == 1,
            "Unchanged schema reload must not rebuild schema metadata");
        foreach (var item in preparation)
            Require(item.calls == values[item.stage][0] && item.failedCalls == values[item.stage][1] && item.elapsedMs == values[item.stage][2],
                "A previously returned schema-preparation snapshot changed");
        string valid = Write(name + "/valid.xml", Asset(Experience("WorkMetricOwner", null, false)));
        Run(processor, valid);
        var parsed = processor.GetWorkMetrics().Single(item => item.stage == "rawParse");
        Require(parsed.calls == processor.XmlParseCount && parsed.calls == 1 && parsed.failedCalls == 0,
            "Raw parse intervals must count the actual parser invocations");
        long parseCalls = parsed.calls; double parseElapsed = parsed.elapsedMs;
        string invalid = Write(name + "/invalid.xml", "<AssetDeclaration");
        Failure(processor, invalid, ErrorCode.XmlFormattingError);
        var failed = processor.GetWorkMetrics().Single(item => item.stage == "rawParse");
        Require(failed.calls == processor.XmlParseCount && failed.calls == parseCalls + 1 && failed.failedCalls == 1 && failed.elapsedMs >= parseElapsed,
            "Failed parsing must retain its call, failure and elapsed work");
        Require(parsed.calls == parseCalls && parsed.failedCalls == 0 && parsed.elapsedMs == parseElapsed,
            "A previous parse metric snapshot changed after failure");
        string originalSchema = settings.SchemaPath;
        try
        {
            settings.SchemaPath = Path.Combine(work, name, "missing-schema.xsd");
            bool rejected = false;
            try { schemas.ReloadIfChanged(settings.StableSort); }
            catch (BinaryAssetBuilderException error) { rejected = error.ErrorCode == ErrorCode.ReadingSchema; }
            Require(rejected, "Missing schema reload must retain its original reading error");
            var readFailure = schemas.GetPreparationMetrics().Single(item => item.stage == "schemaRead");
            Require(readFailure.calls == 2 && readFailure.failedCalls == 1 && readFailure.elapsedMs >= preparation.Single(item => item.stage == "schemaRead").elapsedMs,
                "Failed schema loading must retain its call, failure and elapsed work");
        }
        finally { settings.SchemaPath = originalSchema; }
        WorkMetrics.Add(new { id = name, preparation, checkedPreparation, finalPreparation = schemas.GetPreparationMetrics(),
            initialFilesRead = filesRead, initialBytesRead = bytesRead, finalBytesRead = schemas.SchemaContentBytesRead,
            contentChecks = schemas.SchemaContentCheckCount, successfulParse = parsed, finalStages = processor.GetWorkMetrics() });
        Tests.Add("work-metrics:schema-closure-content-checks-parse-failure-and-immutable-snapshots");
    }
    static void LinkedSideFileProbe(BuiltinCompilers plugins)
    {
        const string name = "risk-linked-side-files";
        var settings = Configure(name);
        string input = Write(name + "/root.xml", Asset(Experience("ImportParent", null, false) + Experience("ImportChild", "ImportParent", false)));
        var first = Processor(plugins); Run(first, input);
        var expected = Outputs(settings.OutputDirectory);
        var original = Directory.GetFiles(settings.OutputDirectory, "*", SearchOption.AllDirectories)
            .Where(path => new[] { ".manifest", ".bin", ".imp", ".relo" }.Contains(Path.GetExtension(path)))
            .ToDictionary(path => path, File.ReadAllBytes);
        foreach (string extension in new[] { ".imp", ".relo" })
        {
            string side = original.Keys.Single(path => Path.GetExtension(path) == extension);
            Require(original[side].Length > 4, "Side-file recovery probe requires a nonempty payload: " + extension);
            foreach (string damage in new[] { "deleted", "truncated-header", "truncated-payload" })
            {
                foreach (var pair in original) File.WriteAllBytes(pair.Key, pair.Value);
                if (damage == "deleted") File.Delete(side);
                else File.WriteAllBytes(side, original[side].Take(damage == "truncated-header" ? 2 : 4).ToArray());
                var bin = original.Keys.Single(path => Path.GetExtension(path) == ".bin");
                Require(File.ReadAllBytes(bin).SequenceEqual(original[bin]), "Probe must retain valid .bin bytes");
                var repeat = Processor(plugins); Run(repeat, input);
                bool equivalent = SameOutputs(expected, Outputs(settings.OutputDirectory));
                OptimizationRisks.Add(new { id = name, extension, damage, boundary = "public-Core-existing-linked-stream-fresh-processor",
                    equivalent, reproduced = !equivalent, originalPayloadBytes = original[side].Length - 4,
                    restoredPayloadBytes = new FileInfo(side).Length - 4, cliFailureEstablished = false });
                Require(equivalent, "Complete linked stream must recover " + extension + " " + damage + " with a valid retained .bin");
            }
        }
        foreach (var pair in original) File.WriteAllBytes(pair.Key, pair.Value);
        Tests.Add("risk-linked-side-files:real-Core-valid-bin-independent-imp-relo-deletion-header-and-payload-truncation-four-output-equality");
    }
    public static int Main(string[] args)
    {
        object result; int code = 0; Settings original = Settings.Current;
        try
        {
            if (args.Length != 5 || IntPtr.Size != 4) throw new Exception("Expected work result sourceCommit runId sdk (x86)");
            work = Path.GetFullPath(args[0]); sdk = Path.GetFullPath(args[4]); Directory.CreateDirectory(work);
            probe = File.ReadAllText(Path.Combine(sdk, "Mods/mymod/Data/RecoveryArmorProbe.xml"));
            UnicodePathCase();
            Require(typeof(DocumentProcessor).Assembly == typeof(BuiltinCompilers).Assembly, "Core is outside product assembly");
            // Configuration precedes built-in initialization, just as a real build requires.
            Configure("initialize");
            using (var plugins = BuiltinCompilers.Create(TargetPlatform.Win32))
            {
                RetryCase(plugins, "include", false); RetryCase(plugins, "inheritance", true);
                CycleCase(plugins); InheritanceExecutionCase(plugins, 2); InheritanceExecutionCase(plugins, 3); ZeroInheritanceDigestCase(plugins);
                ProjectCase(plugins); SaveFailureCase(plugins);
                MetricsFailureCase(plugins, true, false); MetricsFailureCase(plugins, false, true);
                MetricsFailureCase(plugins, true, true); ProcessorStateCase(plugins);
                DefineOverrideProbe(plugins, false); DefineOverrideProbe(plugins, true);
                CrossStreamBindingProbe(plugins); LinkedSideFileProbe(plugins); WorkMetricsProbe(plugins);
                Console.WriteLine("W01_PROBES " + new JavaScriptSerializer().Serialize(OptimizationRisks));
            }
            result = new { succeeded=true, sourceCommit=args[2], runId=args[3], checks=checks, tests=Tests.ToArray(), optimizationRisks=OptimizationRisks.ToArray(), workMetrics=WorkMetrics.ToArray(), optimizationRisksAccepted=true, longPathsValidated=false, scope="Real Core lifecycle and targeted persisted-session regression probes only; production CLI backend, SDK/G3 and general persistent-cache acceptance are separate" };
            Console.WriteLine("PASS CoreLifecycle: " + Tests.Count + " groups; " + checks + " checks");
        }
        catch (Exception e)
        {
            code=1; result = new { succeeded=false, sourceCommit=args.Length>2?args[2]:null, runId=args.Length>3?args[3]:null, checks=checks, tests=Tests.ToArray(), optimizationRisks=OptimizationRisks.ToArray(), workMetrics=WorkMetrics.ToArray(), optimizationRisksAccepted=false, longPathsValidated=false, error=e.ToString() };
            Console.Error.WriteLine(e);
        }
        finally { Settings.Current = original; }
        if (args.Length>1) File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(result));
        return code;
    }
}
