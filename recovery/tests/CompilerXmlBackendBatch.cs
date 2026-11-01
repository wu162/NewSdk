using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder.Core;
using NewModCompiler.Application;

// One shared certification matrix for all reviewed metadata modules. It is a
// component reentry/isolation test, not SDK-global or complex-input acceptance.
public static class CompilerXmlBackendBatch
{
    const string NS = "uri:ea.com:eala:asset";
    static readonly Type Prepared = typeof(CoreStreamCompiler).Assembly.GetType("NewModCompiler.Application.PreparedBackendWork", true);
    static readonly Type Policy = typeof(CoreStreamCompiler).Assembly.GetType("NewModCompiler.Application.XmlBackendCertification", true);
    static int checks, active, maximum;
    sealed class Outcome
    {
        public AssetBuffer Buffer;
        public string ErrorType, ErrorMessage;
        public int ThreadId;
        public long Start, End;
    }
    sealed class Work
    {
        public string TypeName, Family;
        public object Envelope;
        public InstanceDeclaration Control;
        public Outcome Worker;
    }
    static void Need(bool value, string message)
    { checks++; if (!value) throw new InvalidOperationException(message); }
    static InstanceDeclaration Declaration(string type, IAssetBuilderPlugin plugin, out XmlNamespaceManager namespaces)
    {
        var document = new XmlDocument { XmlResolver = null };
        var root = document.CreateElement(type, NS); root.SetAttribute("id", "Batch" + type); document.AppendChild(root);
        namespaces = new XmlNamespaceManager(document.NameTable); namespaces.AddNamespace("ea", NS);
        var declaration = new InstanceDeclaration(AssetDeclarationDocument.CreateBackendView(namespaces));
        declaration.XmlNode = root;
        var metadata = plugin.GetExtendedTypeInformation(declaration.Handle.TypeId);
        declaration.Handle.TypeHash = metadata.TypeHash; declaration.ProcessingHash = metadata.ProcessingHash;
        declaration.HasCustomData = metadata.HasCustomData;
        return declaration;
    }
    static object Prepare(InstanceDeclaration declaration, IAssetBuilderPlugin plugin)
    {
        var arguments = new object[] { declaration, plugin, null };
        bool accepted = (bool)Prepared.GetMethod("TryPrepare").Invoke(null, arguments);
        Need(accepted == (arguments[2] != null), "Prepared result presence differs");
        return arguments[2];
    }
    static Outcome Call(Func<AssetBuffer> action)
    {
        var result = new Outcome { ThreadId = Thread.CurrentThread.ManagedThreadId, Start = Stopwatch.GetTimestamp() };
        try { result.Buffer = action(); }
        catch (Exception error)
        {
            while (error is TargetInvocationException && error.InnerException != null) error = error.InnerException;
            result.ErrorType = error.GetType().FullName; result.ErrorMessage = error.Message;
        }
        finally { result.End = Stopwatch.GetTimestamp(); }
        return result;
    }
    static void Bytes(byte[] expected, byte[] actual, string label)
    {
        Need((expected == null) == (actual == null), label + " null contract differs");
        if (expected == null) return;
        Need(expected.Length == actual.Length, label + " length differs");
        for (int i = 0; i < expected.Length; i++)
            if (expected[i] != actual[i]) throw new InvalidOperationException(label + " raw byte differs at " + i);
        checks++;
    }
    static void Equal(Outcome owner, Outcome worker, string label)
    {
        Need(owner.ErrorType == worker.ErrorType && owner.ErrorMessage == worker.ErrorMessage, label + " exception/rejection contract differs");
        if (owner.ErrorType != null) return;
        Need(owner.Buffer != null && worker.Buffer != null, label + " successful calls must both return complete buffers");
        Bytes(owner.Buffer.InstanceData, worker.Buffer.InstanceData, label + "/instance");
        Bytes(owner.Buffer.RelocationData, worker.Buffer.RelocationData, label + "/relocation");
        Bytes(owner.Buffer.ImportsData, worker.Buffer.ImportsData, label + "/imports");
    }
    static object Matrix(TargetPlatform platform)
    {
        var plugin = new Recovered.XmlPriority.Plugin(); plugin.Initialize(null, platform);
        int ownerThread = Thread.CurrentThread.ManagedThreadId;
        var work = new List<Work>(); var retainedOwners = new List<string>();
        var launched = new List<Task>(4);
        try
        {
            foreach (var route in BuiltinCompilers.ExpectedRoutes)
            {
                // The UI source provider also preserves the original binary
                // AptAptData backend, covered by this unchanged XML matrix.
                if (route.Family != "xml" && route.TypeName != "AptAptData") continue;
                bool certified = (bool)Policy.GetMethod("IsCertified").Invoke(null, new object[] { route.TypeId });
                XmlNamespaceManager namespaces;
                var declaration = Declaration(route.TypeName, plugin, out namespaces);
                object envelope = Prepare(declaration, plugin);
                if (!certified)
                {
                    if (envelope != null)
                    {
                        // Read-dependent APT keeps its separate real leased-file
                        // matrix. All other unreviewed XML remains owner-only.
                        bool apt = route.TypeName == "AptAptData" || route.TypeName == "AptConstData" || route.TypeName == "AptDatData" || route.TypeName == "AptGeometryData";
                        Need(apt || envelope == null, "Unreviewed metadata route became parallel: " + route.TypeName);
                        ((IDisposable)envelope).Dispose();
                    }
                    retainedOwners.Add(route.TypeName); continue;
                }
                Need(envelope != null, "Reviewed module route was not prepared for workers: " + route.TypeName);
                XmlNamespaceManager ignored;
                var control = Declaration(route.TypeName, plugin, out ignored);
                var frozen = (InstanceDeclaration)Prepared.GetProperty("Declaration").GetValue(envelope, null);
                Need(!Object.ReferenceEquals(frozen, declaration) && !Object.ReferenceEquals(frozen.Document, declaration.Document) &&
                    Object.ReferenceEquals(frozen.Node, declaration.Node) && Object.ReferenceEquals(frozen.Handle, declaration.Handle) &&
                    Object.ReferenceEquals(frozen.ReferencedInstances, declaration.ReferencedInstances) &&
                    Object.ReferenceEquals(frozen.Node.OwnerDocument, declaration.Node.OwnerDocument) &&
                    Object.ReferenceEquals(frozen.Document.NamespaceManager, namespaces),
                    "Metadata workers must own stable declaration/document views while borrowing finalized XML and identity: " + route.TypeName);
                // Worker execution comes first, including cold native module
                // initialization. Core leaves borrowed input unchanged until join.
                work.Add(new Work { TypeName = route.TypeName, Family = (string)Policy.GetMethod("FamilyFor").Invoke(null, new object[] { route.TypeId }), Envelope = envelope, Control = control });
            }
            var expectedOwners = new HashSet<string>(new[] { "AptAptData", "AptConstData", "AptDatData", "AptGeometryData", "TestTexture" }, StringComparer.Ordinal);
            Need(retainedOwners.Count == 5 && expectedOwners.SetEquals(retainedOwners), "Only the four explicitly closed-read APT routes and TestTexture may remain outside the metadata matrix");
            Need(work.Count == (int)Policy.GetProperty("TypeCount").GetValue(null, null), "Batch must execute every reviewed registry route present in the built-in XML set");
            Need(work.Count == 295, "The current reviewed metadata source closure, including eleven Joypad roots, must contain exactly 295 registered routes");
            Need(!(bool)Policy.GetMethod("IsCertified").Invoke(null, new object[] { UInt32.MaxValue }), "An unknown root ID must remain outside the closed metadata registry");
            int next = -1; active = 0; maximum = 0;
            int workerCount = Math.Max(1, Math.Min(4, Environment.ProcessorCount));
            var tasks = new Task[workerCount];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Factory.StartNew(delegate {
                    if (Thread.CurrentThread.ManagedThreadId == ownerThread) throw new InvalidOperationException("Component work ran on its owner");
                    for (int index = Interlocked.Increment(ref next); index < work.Count; index = Interlocked.Increment(ref next))
                    {
                        var item = work[index];
                        int now = Interlocked.Increment(ref active), seen;
                        do { seen = maximum; if (seen >= now) break; } while (Interlocked.CompareExchange(ref maximum, now, seen) != seen);
                        try { item.Worker = Call(delegate { return (AssetBuffer)Prepared.GetMethod("Execute").Invoke(item.Envelope, null); }); }
                        finally { Interlocked.Decrement(ref active); }
                    }
                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                launched.Add(tasks[i]);
            }
            Task.WaitAll(tasks);
            var rows = new List<object>(); int successful = 0, rejected = 0;
            foreach (var item in work)
            {
                Need(item.Worker != null && item.Worker.ThreadId != ownerThread && item.Worker.End > item.Worker.Start, "No completed real worker invocation: " + item.TypeName);
                var owner = Call(delegate { return plugin.ProcessInstance(item.Control); });
                Equal(owner, item.Worker, item.TypeName + "/" + platform);
                if (owner.ErrorType == null) successful++; else rejected++;
                rows.Add(new { type = item.TypeName, family = item.Family, workerThreadId = item.Worker.ThreadId,
                    startTicks = item.Worker.Start, endTicks = item.Worker.End, ownerThreadId = ownerThread,
                    outcome = owner.ErrorType == null ? "three-buffer-equal" : "same-minimal-input-rejection", exceptionType = owner.ErrorType,
                    instanceBytes = owner.Buffer == null || owner.Buffer.InstanceData == null ? 0 : owner.Buffer.InstanceData.Length,
                    relocationBytes = owner.Buffer == null || owner.Buffer.RelocationData == null ? 0 : owner.Buffer.RelocationData.Length,
                    importsBytes = owner.Buffer == null || owner.Buffer.ImportsData == null ? 0 : owner.Buffer.ImportsData.Length });
            }
            Need(active == 0 && maximum <= workerCount, "Component worker wave did not join within its bound");
            return new { platform = (int)platform, certifiedRoutes = work.Count, successfulMinimalInputs = successful,
                rejectedMinimalInputs = rejected, workerCount, maximumConcurrency = maximum, retainedOwners, rows };
        }
        finally
        {
            // A later worker creation may fail after earlier workers started.
            // Join every successfully launched task before releasing any DOM.
            try { Task.WaitAll(launched.ToArray()); }
            finally { foreach (var item in work) ((IDisposable)item.Envelope).Dispose(); }
        }
    }
    public static int Main(string[] args)
    {
        try
        {
            var matrices = new List<object>();
            foreach (var platform in new[] { TargetPlatform.Win32, TargetPlatform.Xbox360, TargetPlatform.PlayStation3, (TargetPlatform)99 }) matrices.Add(Matrix(platform));
            File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, matrices,
                scope = "Reviewed XML metadata modules: borrowed finalized minimal DOMs, cold bounded worker wave followed by serial owner raw three-buffer or exact rejection parity on four platforms. Minimal invalid inputs are explicitly recorded as rejection checks. This does not replace historical complex original-DLL matrices or prove every type is SDK-global or simultaneously overlaps in production." }));
            Console.WriteLine("PASS XmlBackendBatch: " + checks + " assertions"); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
