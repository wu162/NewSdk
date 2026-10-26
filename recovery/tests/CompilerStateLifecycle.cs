using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web.Script.Serialization;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;

// Loaded by the production-object validation host: every contract below is the
// real type from its entry EXE, with no substitute Core assembly or static stub.
public static class CompilerStateLifecycle
{
    static object Field(Type type, string name) { return type.GetField(name, BindingFlags.Static | BindingFlags.NonPublic).GetValue(null); }
    static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
    static void Reject(Action action, string message)
    {
        bool rejected = false;
        try { action(); } catch (InvalidOperationException) { rejected = true; }
        Require(rejected, message);
    }
    static void RejectOtherThread(Action action, string message)
    {
        Exception result = null;
        var thread = new Thread(delegate() { try { action(); } catch (Exception error) { result = error; } });
        thread.IsBackground = true; thread.Start();
        Require(thread.Join(5000), message + " (worker blocked)");
        Require(result is InvalidOperationException, message);
    }
    static object[] HashState()
    {
        return new[] { Field(typeof(HashProvider), "_SchemaNameToBinName"), Field(typeof(HashProvider), "_StringHashBins"), (object)HashProvider.GetOutputDirectory(), HashProvider.StringHashesFile };
    }
    static void SameHashState(object[] expected)
    {
        object[] actual = HashState();
        Require(Object.ReferenceEquals(expected[0], actual[0]) && Object.ReferenceEquals(expected[1], actual[1]), "Hash dictionary identity was not restored");
        Require(Object.Equals(expected[2], actual[2]) && Object.Equals(expected[3], actual[3]), "Hash directory or filename was not restored");
    }
    static StringHashBinDescriptor[] Descriptors()
    {
        return new[] { new StringHashBinDescriptor { SchemaTypeName = "Poid", BinName = "POID" } };
    }
    static void HashChecks(string output)
    {
        Settings previous = Settings.Current;
        object[] before = HashState();
        try
        {
            Settings.Current = new Settings { StringHashBinDescriptors = Descriptors(), SessionCacheDirectory = output, OutputAssetReport = false };
            string baseline = Path.Combine(output, "baseline");
            using (HashProvider.BeginStringHashScope(baseline))
            {
                HashProvider.RecordHash("POID", "BaselineOnly");
                HashProvider.StringHashesFile = "baseline-custom.xml";
                object[] expected = HashState();
                for (int round = 0; round < 2; round++)
                {
                    string inner = Path.Combine(output, "sequential-" + round);
                    using (HashProvider.BeginStringHashScope(inner))
                    {
                        Require(HashProvider.StringHashesFile == "StringHashes.xml", "A new hash scope inherited its parent's filename");
                        HashProvider.RecordHash("POID", "InnerOnly" + round);
                        HashProvider.StringHashesFile = "inner.xml";
                    }
                    SameHashState(expected);
                    Require(!Directory.Exists(inner), "Disposing hash scope published output");
                }
                try
                {
                    using (HashProvider.BeginStringHashScope(Path.Combine(output, "failed-stream")))
                    {
                        HashProvider.RecordHash("POID", "FailedStreamOnly");
                        throw new ApplicationException("controlled stream failure");
                    }
                }
                catch (ApplicationException) { }
                SameHashState(expected);
                Require(!Directory.Exists(Path.Combine(output, "failed-stream")), "Failed scope published hash output");

                Settings.Current.StringHashBinDescriptors = new[] { Descriptors()[0], Descriptors()[0] };
                bool rejected = false;
                try { HashProvider.InitializeStringHashes(Path.Combine(output, "invalid-config")); }
                catch (ArgumentException) { rejected = true; }
                Require(rejected, "Duplicate hash configuration was accepted");
                SameHashState(expected);
                Settings.Current.StringHashBinDescriptors = Descriptors();
                string malformed = Path.Combine(output, "malformed"); Directory.CreateDirectory(malformed);
                File.WriteAllText(Path.Combine(malformed, "StringHashes.xml"), "<AssetDeclaration");
                rejected = false;
                try { using (HashProvider.BeginStringHashScope(malformed)) { } }
                catch (System.Xml.XmlException) { rejected = true; }
                Require(rejected, "Malformed hash file was accepted"); SameHashState(expected);

                // Invalid XML content forces a write failure after the file is open.
                // The handle must close immediately, without waiting for a finalizer.
                string writeFailure = Path.Combine(output, "writer-failure");
                using (HashProvider.BeginStringHashScope(writeFailure))
                {
                    HashProvider.RecordHash("POID", "invalid\u0000xml");
                    rejected = false;
                    try { HashProvider.FinalizeStringHashes(); }
                    catch (ArgumentException) { rejected = true; }
                    Require(rejected, "Invalid XML hash text did not exercise a write failure");
                    using (File.Open(Path.Combine(writeFailure, "StringHashes.xml"), FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
                }
                SameHashState(expected);
                File.Delete(Path.Combine(writeFailure, "StringHashes.xml"));
                using (HashProvider.BeginStringHashScope(writeFailure))
                {
                    HashProvider.RecordHash("POID", "RecoveredAfterWriteFailure");
                    HashProvider.FinalizeStringHashes();
                    var valid = new System.Xml.XmlDocument(); valid.Load(Path.Combine(writeFailure, "StringHashes.xml"));
                    Require(valid.OuterXml.Contains("RecoveredAfterWriteFailure"), "Hash output did not recover after write failure");
                }
                SameHashState(expected);

                var outer = HashProvider.BeginStringHashScope(Path.Combine(output, "nested-outer"));
                object[] outerState = HashState();
                var innerScope = HashProvider.BeginStringHashScope(Path.Combine(output, "nested-inner"));
                object[] innerState = HashState();
                Reject(delegate { outer.Dispose(); }, "Out of order hash disposal was accepted"); SameHashState(innerState);
                RejectOtherThread(delegate { innerScope.Dispose(); }, "Foreign thread disposed hash scope"); SameHashState(innerState);
                RejectOtherThread(delegate { using (HashProvider.BeginStringHashScope(output)) { } }, "Concurrent hash scope was accepted");
                innerScope.Dispose(); SameHashState(outerState); innerScope.Dispose(); SameHashState(outerState);
                outer.Dispose(); SameHashState(expected);

                // Explicit finalization remains supported, and proves that the
                // original tables retained their contents without inner assets.
                HashProvider.FinalizeStringHashes();
                string text = File.ReadAllText(Path.Combine(baseline, "StringHashes.xml"));
                Require(text.Contains("BaselineOnly") && !text.Contains("InnerOnly") && !text.Contains("FailedStreamOnly"), "Hash contents leaked between streams");
                int preparingCallbacks = 0;
                using (Tracer.BeginScope(delegate(string source, TraceEventType kind, string message)
                {
                    if (source != "HashProvider") return;
                    preparingCallbacks++;
                    SameHashState(expected);
                    Reject(delegate { HashProvider.InitializeStringHashes(output); }, "Trace callback reentered initialization");
                    Reject(delegate { using (HashProvider.BeginStringHashScope(output)) { } }, "Trace callback reentered scope creation");
                    RejectOtherThread(delegate { HashProvider.InitializeStringHashes(output); }, "Trace callback worker initialized hashes");
                    RejectOtherThread(delegate { using (HashProvider.BeginStringHashScope(output)) { } }, "Trace callback worker entered hash scope");
                }, 9))
                using (HashProvider.BeginStringHashScope(baseline))
                {
                    HashProvider.FinalizeStringHashes();
                    Require(File.ReadAllText(Path.Combine(baseline, "StringHashes.xml")) == text, "Loading prior valid string hashes changed their contents");
                }
                Require(preparingCallbacks == 1, "Expected the prior hash load to exercise one trace callback");
                SameHashState(expected);
            }
            SameHashState(before);
        }
        finally { Settings.Current = previous; }
    }
    static void TraceChecks()
    {
        TraceWriteHandler previous = Tracer.TraceWrite;
        object previousMask = Field(typeof(Tracer), "_TraceMask");
        string environment = Environment.GetEnvironmentVariable("BabEnableTrace");
        try
        {
            Environment.SetEnvironmentVariable("BabEnableTrace", "False");
            Tracer tracer = Tracer.GetTracer("Lifecycle-" + Guid.NewGuid().ToString("N"), "Lifecycle fixture");
            int baselineCalls = 0, firstCalls = 0, secondCalls = 0;
            TraceWriteHandler baseline = delegate { baselineCalls++; };
            Tracer.TraceWrite = baseline;
            // A nonstandard mask ensures restoration preserves the exact mask,
            // rather than guessing its nearest public verbosity level.
            typeof(Tracer).GetField("_TraceMask", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, TraceKind.Warning | TraceKind.Data);
            object baselineMask = Field(typeof(Tracer), "_TraceMask");
            using (Tracer.BeginScope(delegate { firstCalls++; }, 9))
            {
                tracer.Message("first");
                var nested = Tracer.BeginScope(delegate { secondCalls++; }, 0);
                tracer.Message("suppressed");
                nested.Dispose(); nested.Dispose();
                tracer.Message("first again");
            }
            Require(Object.ReferenceEquals(Tracer.TraceWrite, baseline) && baselineMask.Equals(Field(typeof(Tracer), "_TraceMask")), "Trace parent state was not restored exactly");
            try
            {
                using (Tracer.BeginScope(delegate { secondCalls++; }, 9))
                { tracer.Message("second"); throw new ApplicationException("controlled trace failure"); }
            }
            catch (ApplicationException) { }
            Require(Object.ReferenceEquals(Tracer.TraceWrite, baseline) && baselineMask.Equals(Field(typeof(Tracer), "_TraceMask")), "Trace state leaked after failure");
            tracer.TraceWarning("baseline");
            Require(firstCalls == 2 && secondCalls == 1 && baselineCalls == 1, "Trace callback was duplicated or escaped its scope");

            var outer = Tracer.BeginScope(null, 0);
            var inner = Tracer.BeginScope(baseline, 9);
            Reject(delegate { outer.Dispose(); }, "Out of order trace disposal was accepted");
            Require(Object.ReferenceEquals(Tracer.TraceWrite, baseline), "Rejected trace disposal changed current callback");
            RejectOtherThread(delegate { inner.Dispose(); }, "Foreign thread disposed trace scope");
            RejectOtherThread(delegate { using (Tracer.BeginScope(null, 0)) { } }, "Concurrent trace scope was accepted");
            inner.Dispose(); Require(Tracer.TraceWrite == null && Field(typeof(Tracer), "_TraceMask").Equals(TraceKind.None), "Nested trace parent was not restored");
            outer.Dispose();
            Require(Object.ReferenceEquals(Tracer.TraceWrite, baseline) && baselineMask.Equals(Field(typeof(Tracer), "_TraceMask")), "Final trace state differs");
            bool invalid = false;
            try { using (Tracer.BeginScope(null, -1)) { } } catch (IndexOutOfRangeException) { invalid = true; }
            Require(invalid && Object.ReferenceEquals(Tracer.TraceWrite, baseline) && baselineMask.Equals(Field(typeof(Tracer), "_TraceMask")), "Invalid trace level changed state");
        }
        finally
        {
            Tracer.TraceWrite = previous;
            typeof(Tracer).GetField("_TraceMask", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, previousMask);
            Environment.SetEnvironmentVariable("BabEnableTrace", environment);
        }
    }
    public static void Run(string outputDirectory)
    {
        Directory.CreateDirectory(outputDirectory);
        Require(typeof(HashProvider).Assembly == Assembly.GetEntryAssembly() && typeof(Tracer).Assembly == Assembly.GetEntryAssembly(), "State tests must use real production EXE types");
        string environment = Environment.GetEnvironmentVariable("BabEnableTrace");
        try
        {
            Environment.SetEnvironmentVariable("BabEnableTrace", "False");
            HashChecks(outputDirectory); TraceChecks();
        }
        finally { Environment.SetEnvironmentVariable("BabEnableTrace", environment); }
        Console.WriteLine("PASS Hash/Tracer state lifecycle");
    }
    public static int Main(string[] args)
    {
        try
        {
            Run(args[0]);
            File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(new { succeeded = true, sourceCommit = args[2], runId = args[3], checks = new[] { "hash-scope-restoration", "atomic-hash-initialization", "hash-write-failure-releases-file", "hash-callback-worker-no-deadlock", "hash-lifo-and-thread-rejection", "trace-scope-restoration", "trace-no-duplicate-callbacks", "trace-lifo-and-thread-rejection" } }));
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
