// Runs through the dedicated validation entry in the production-object EXE.
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using EALA.Metrics;

static class CompilerResourceLifecycle
{
    static readonly List<string> Checks = new List<string>();
    static void Check(bool value, string name) { if (!value) throw new Exception(name); Checks.Add(name); }
    static void SameFailure(Action action, Exception expected, string name)
    {
        Exception actual = null; try { action(); } catch (Exception e) { actual = e; }
        Check(Object.ReferenceEquals(actual, expected), name);
    }
    static void Rejected(Action action, string name)
    {
        bool rejected = false; try { action(); } catch (ArgumentException) { rejected = true; }
        Check(rejected, name);
    }
    sealed class Listener : IMetricsListener
    {
        internal string Name;
        internal List<string> Log;
        internal Exception OpenFailure, CloseFailure, SubmitFailure;
        internal Action OnOpen, OnClose, OnSubmit;
        internal bool ResourceAcquired;
        internal int Opens, Closes, Submits;
        public void Open() { Opens++; ResourceAcquired = true; Log.Add("open:" + Name); if (OnOpen != null) OnOpen(); if (OpenFailure != null) throw OpenFailure; }
        public void Close() { Closes++; ResourceAcquired = false; Log.Add("close:" + Name); if (OnClose != null) OnClose(); if (CloseFailure != null) throw CloseFailure; }
        public void SubmitMetrics(Metric value) { Submits++; if (OnSubmit != null) OnSubmit(); if (SubmitFailure != null) throw SubmitFailure; }
        public override bool Equals(object value) { throw new Exception("Listener equality must not run while removing a registration"); }
        public override int GetHashCode() { return 0; }
    }
    static Listener NewListener(string name, List<string> log) { return new Listener { Name = name, Log = log }; }
    static void WorkerMutation(Action action, string name)
    {
        Exception failure = null;
        var worker = new Thread(delegate() { try { action(); } catch (Exception e) { failure = e; } });
        worker.IsBackground = true; worker.Start();
        Check(worker.Join(5000), name + " callback can join mutation worker");
        Check(failure is ArgumentException, name + " worker mutation rejected");
    }
    static void TestMetrics()
    {
        var log = new List<string>();
        var a = NewListener("a", log); var b = NewListener("b", log); var c = NewListener("c", log);
        var primary = new IOException("open failure"); var secondary = new IOException("rollback failure");
        c.OpenFailure = primary; b.CloseFailure = secondary;
        MetricManager.AddListener(a); MetricManager.AddListener(b); MetricManager.AddListener(c);
        SameFailure(MetricManager.OpenSession, primary, "metrics Open preserves first error");
        Check(String.Join(",", log.ToArray()) == "open:a,open:b,open:c,close:c,close:b,close:a", "metrics failed Open reverses attempted listeners");
        Check(a.Closes == 1 && b.Closes == 1 && c.Closes == 1 && !a.ResourceAcquired && !b.ResourceAcquired && !c.ResourceAcquired, "metrics rollback releases failing and completed listeners");
        Check(MetricManager.RemoveListener(c), "metrics remove after failed Open");
        b.CloseFailure = null; log.Clear();
        a.OnOpen = delegate {
            Rejected(delegate { MetricManager.AddListener(c); }, "metrics add rejected during Open");
            Rejected(delegate { MetricManager.RemoveListener(b); }, "metrics remove rejected during Open");
            Rejected(MetricManager.OpenSession, "metrics recursive Open rejected");
            Rejected(MetricManager.CloseSession, "metrics Close during Open rejected");
            WorkerMutation(delegate { MetricManager.AddListener(c); }, "metrics Open");
        };
        a.OnClose = delegate {
            Rejected(delegate { MetricManager.RemoveListener(a); }, "metrics remove rejected during Close");
            Rejected(MetricManager.OpenSession, "metrics Open during Close rejected");
            Rejected(MetricManager.CloseSession, "metrics recursive Close rejected");
            WorkerMutation(delegate { MetricManager.RemoveListener(a); }, "metrics Close");
        };
        MetricManager.Enabled = false; MetricManager.OpenSession();
        Check(MetricManager.Enabled, "metrics successful Open enables metrics");
        Rejected(delegate { MetricManager.RemoveListener(a); }, "metrics remove rejected while open");
        a.CloseFailure = primary; b.CloseFailure = secondary;
        SameFailure(MetricManager.CloseSession, primary, "metrics Close preserves first error");
        Check(String.Join(",", log.ToArray()) == "open:a,open:b,close:a,close:b", "metrics Close preserves successful order and attempts all listeners");
        a.OnOpen = a.OnClose = null; a.CloseFailure = b.CloseFailure = null;
        MetricManager.OpenSession();
        var descriptor = MetricManager.GetDescriptor("resource-lifecycle", MetricType.Count, "resource fixture");
        a.OnSubmit = delegate { WorkerMutation(delegate { MetricManager.RemoveListener(a); }, "metrics Submit"); };
        a.SubmitFailure = primary;
        SameFailure(delegate { MetricManager.Submit(descriptor, 1); }, primary, "metrics Submit preserves callback error");
        a.SubmitFailure = null; MetricManager.Submit(descriptor, 2);
        Check(a.Submits == 2 && b.Submits == 1, "metrics Submit can retry after callback failure");
        MetricManager.CloseSession();
        Check(MetricManager.RemoveListener(a) && MetricManager.RemoveListener(b), "metrics listeners removable after retry");
        Check(!MetricManager.RemoveListener(a), "metrics missing listener removal is false");
        var equalByReferenceOnly = NewListener("identity", log);
        MetricManager.AddListener(equalByReferenceOnly);
        Check(!MetricManager.RemoveListener(NewListener("other-instance", log)), "metrics removal does not invoke listener equality");
        Check(MetricManager.RemoveListener(equalByReferenceOnly), "metrics removal finds registered reference without equality");
        for (int i = 0; i < 3; i++) { MetricManager.AddListener(c); c.OpenFailure = null; MetricManager.OpenSession(); MetricManager.CloseSession(); Check(MetricManager.RemoveListener(c), "metrics repeated registration released " + i); }
        Check(c.Opens == 4 && c.Closes == 4, "metrics registrations do not accumulate");

        // Concurrent mutation is rejected while Open is in a callback; the
        // listener list cannot be changed in a partially opened session.
        using (var entered = new ManualResetEvent(false))
        using (var release = new ManualResetEvent(false))
        using (var attempted = new ManualResetEvent(false))
        {
            var blocker = NewListener("blocker", log);
            blocker.OnOpen = delegate { entered.Set(); if (!release.WaitOne(5000)) throw new TimeoutException("release Open"); };
            MetricManager.AddListener(blocker);
            Exception openError = null, mutationError = null;
            var opener = new Thread(delegate() { try { MetricManager.OpenSession(); } catch (Exception e) { openError = e; } });
            var changer = new Thread(delegate() { attempted.Set(); try { MetricManager.AddListener(c); } catch (Exception e) { mutationError = e; } });
            opener.IsBackground = changer.IsBackground = true;
            opener.Start(); Check(entered.WaitOne(5000), "metrics concurrent Open entered");
            changer.Start(); Check(attempted.WaitOne(5000), "metrics concurrent mutation attempted");
            release.Set();
            Check(opener.Join(5000) && changer.Join(5000), "metrics concurrent transition completes");
            Check(openError == null && mutationError is ArgumentException, "metrics concurrent mutation sees completed open session");
            MetricManager.CloseSession(); Check(MetricManager.RemoveListener(blocker), "metrics concurrent listener released");
        }
        var listeners = (ICollection)typeof(MetricManager).GetField("m_Listeners", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        Check(listeners.Count == 0, "metrics no retained listeners");
    }

    static object ReportValue(string name)
    {
        var flags = BindingFlags.Static | BindingFlags.NonPublic;
        var field = typeof(AssetReport).GetField(name, flags);
        return field != null ? field.GetValue(null) : typeof(AssetReport).GetProperty(name, flags).GetValue(null, null);
    }
    static void SetReportValue(string name, object value)
    {
        var flags = BindingFlags.Static | BindingFlags.NonPublic;
        var field = typeof(AssetReport).GetField(name, flags);
        if (field != null) field.SetValue(null, value);
        else typeof(AssetReport).GetProperty(name, flags).SetValue(null, value, null);
    }
    static void InitReport()
    {
        try { typeof(AssetReport).GetMethod("InitAssetRecordWriter", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null); }
        catch (TargetInvocationException e) { System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e.InnerException).Throw(); throw; }
    }
    static void ReportCleared(string name)
    {
        Check(ReportValue("AssetRecordWriter") == null && ReportValue("AssetRecordStream") == null &&
              ReportValue("RecordedAssets") == null && AssetReport.FileNameFullyQualified == null, name);
    }
    sealed class FaultWriter : XmlWriter
    {
        readonly XmlWriter inner;
        internal Exception EndFailure, FlushFailure, CloseFailure;
        internal Action OnClose;
        internal bool CloseCalled;
        internal FaultWriter(XmlWriter value) { inner = value; }
        public override WriteState WriteState { get { return inner.WriteState; } }
        public override string LookupPrefix(string ns) { return inner.LookupPrefix(ns); }
        public override void Flush() { if (FlushFailure != null) throw FlushFailure; inner.Flush(); }
        public override void Close() { CloseCalled = true; if (OnClose != null) OnClose(); if (CloseFailure != null) throw CloseFailure; inner.Close(); }
        public override void WriteEndDocument() { if (EndFailure != null) throw EndFailure; inner.WriteEndDocument(); }
        public override void WriteStartDocument() { inner.WriteStartDocument(); }
        public override void WriteStartDocument(bool standalone) { inner.WriteStartDocument(standalone); }
        public override void WriteDocType(string name, string pubid, string sysid, string subset) { inner.WriteDocType(name, pubid, sysid, subset); }
        public override void WriteStartElement(string prefix, string localName, string ns) { inner.WriteStartElement(prefix, localName, ns); }
        public override void WriteEndElement() { inner.WriteEndElement(); }
        public override void WriteFullEndElement() { inner.WriteFullEndElement(); }
        public override void WriteStartAttribute(string prefix, string localName, string ns) { inner.WriteStartAttribute(prefix, localName, ns); }
        public override void WriteEndAttribute() { inner.WriteEndAttribute(); }
        public override void WriteCData(string text) { inner.WriteCData(text); }
        public override void WriteComment(string text) { inner.WriteComment(text); }
        public override void WriteProcessingInstruction(string name, string text) { inner.WriteProcessingInstruction(name, text); }
        public override void WriteEntityRef(string name) { inner.WriteEntityRef(name); }
        public override void WriteCharEntity(char ch) { inner.WriteCharEntity(ch); }
        public override void WriteWhitespace(string ws) { inner.WriteWhitespace(ws); }
        public override void WriteString(string text) { inner.WriteString(text); }
        public override void WriteSurrogateCharEntity(char lowChar, char highChar) { inner.WriteSurrogateCharEntity(lowChar, highChar); }
        public override void WriteChars(char[] buffer, int index, int count) { inner.WriteChars(buffer, index, count); }
        public override void WriteRaw(char[] buffer, int index, int count) { inner.WriteRaw(buffer, index, count); }
        public override void WriteRaw(string data) { inner.WriteRaw(data); }
        public override void WriteBase64(byte[] buffer, int index, int count) { inner.WriteBase64(buffer, index, count); }
    }
    static void TestReports(string root)
    {
        Settings old = Settings.Current; string oldName = AssetReport.FileName;
        try
        {
            AssetReport.Close(); Directory.CreateDirectory(root);
            Settings.Current = new Settings { SessionCacheDirectory = root };
            string path = Path.Combine(root, AssetReport.FileName);
            using (var held = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                bool failed = false; try { InitReport(); } catch (IOException) { failed = true; }
                Check(failed, "asset report Init rejects locked output");
                ReportCleared("asset report failed Init clears state");
            }
            for (int i = 0; i < 3; i++)
            {
                InitReport(); var original = (XmlWriter)ReportValue("AssetRecordWriter");
                var fault = new FaultWriter(original); var primary = new IOException("writer primary " + i); var secondary = new IOException("writer Close secondary");
                if (i == 0) { fault.EndFailure = primary; fault.CloseFailure = secondary; }
                if (i == 1) { fault.FlushFailure = primary; fault.CloseFailure = secondary; }
                if (i == 2) fault.CloseFailure = primary;
                SetReportValue("AssetRecordWriter", fault);
                SameFailure(AssetReport.Close, primary, "asset report preserves writer failure " + i);
                Check(fault.CloseCalled, "asset report always attempts writer Close " + i);
                ReportCleared("asset report failed Close clears state " + i);
                using (var exclusive = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { Check(exclusive.CanWrite, "asset report releases file despite writer failure " + i); }
                AssetReport.Close();
                InitReport(); AssetReport.Close();
                var doc = new XmlDocument(); doc.Load(path);
                Check(doc.DocumentElement.LocalName == "AssetDeclaration" && doc.DocumentElement.NamespaceURI == "uri:ea.com:eala:asset" &&
                      doc.DocumentElement.FirstChild.LocalName == AssetReport.TypeName && doc.DocumentElement.FirstChild.Attributes["id"].Value == "TheAssetReportTable", "asset report retry preserves successful document " + i);
                ReportCleared("asset report successful retry clears state " + i);
            }
        }
        finally { AssetReport.Close(); Settings.Current = old; AssetReport.FileName = oldName; }
    }
    static void ScopeRejected(Action action, string name)
    {
        bool rejected = false; try { action(); } catch (InvalidOperationException) { rejected = true; }
        Check(rejected, name);
    }
    static void ScopeWorkerRejected(Action action, string name)
    {
        Exception failure = null;
        var worker = new Thread(delegate() { try { action(); } catch (Exception error) { failure = error; } });
        worker.IsBackground = true; worker.Start();
        Check(worker.Join(5000), name + " worker completes without callback lock deadlock");
        Check(failure is InvalidOperationException, name + " worker rejected");
    }
    static object[] ReportState()
    {
        return new[] { ReportValue("AssetRecordWriter"), ReportValue("AssetRecordStream"),
            ReportValue("RecordedAssets"), AssetReport.FileName, AssetReport.FileNameFullyQualified };
    }
    static void SameReportState(object[] expected, string name)
    {
        object[] actual = ReportState();
        Check(Object.ReferenceEquals(expected[0], actual[0]) && Object.ReferenceEquals(expected[1], actual[1]) && Object.ReferenceEquals(expected[2], actual[2]) &&
              Object.Equals(expected[3], actual[3]) && Object.Equals(expected[4], actual[4]), name);
    }
    static void TestReportScopes(string root)
    {
        Settings previous = Settings.Current; string previousName = AssetReport.FileName;
        try
        {
            AssetReport.Close();
            Settings parentSettings = new Settings { SessionCacheDirectory = Path.Combine(root, "parent") };
            Settings.Current = parentSettings; AssetReport.FileName = "parent-custom.xml"; InitReport();
            object[] parent = ReportState();
            var parentWriter = (XmlWriter)parent[0]; parentWriter.WriteComment("parent-before");
            Settings.Current = new Settings { SessionCacheDirectory = Path.Combine(root, "child-success") };
            using (AssetReport.BeginScope())
            {
                Check(AssetReport.FileName == "AssetReport.xml", "asset report child gets default filename");
                ReportCleared("asset report child begins with empty state");
                AssetReport.FileName = "child-custom.xml"; InitReport();
                ((XmlWriter)ReportValue("AssetRecordWriter")).WriteComment("child-success");
            }
            SameReportState(parent, "asset report successful child restores exact parent state");
            Check(File.ReadAllText(Path.Combine(root, "child-success/child-custom.xml")).Contains("child-success"), "asset report child output completed");
            parentWriter.WriteComment("parent-after-success");

            Settings.Current = new Settings { SessionCacheDirectory = Path.Combine(root, "child-failure") };
            var failedScope = AssetReport.BeginScope(); InitReport();
            var child = (XmlWriter)ReportValue("AssetRecordWriter");
            var primary = new IOException("child EndDocument failure");
            var fault = new FaultWriter(child) { EndFailure = primary, CloseFailure = new IOException("child Close secondary") };
            fault.OnClose = delegate {
                ScopeRejected(delegate { AssetReport.BeginScope(); }, "asset report scope creation rejected during writer Close");
                ScopeRejected(failedScope.Dispose, "asset report scope disposal reentry rejected during writer Close");
                ScopeRejected(AssetReport.Close, "asset report Close reentry rejected");
                ScopeWorkerRejected(delegate { AssetReport.BeginScope(); }, "asset report callback scope creation");
                ScopeWorkerRejected(failedScope.Dispose, "asset report callback scope disposal");
            };
            SetReportValue("AssetRecordWriter", fault);
            SameFailure(failedScope.Dispose, primary, "asset report child Dispose preserves original Close error");
            SameReportState(parent, "asset report failed child restores exact parent state");
            failedScope.Dispose(); SameReportState(parent, "asset report child Dispose is idempotent");
            using (File.Open(Path.Combine(root, "child-failure/AssetReport.xml"), FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
            Check(fault.CloseCalled, "asset report failed child closes writer and releases file");
            parentWriter.WriteComment("parent-after-failure");

            var outer = AssetReport.BeginScope(); object[] outerState = ReportState();
            var inner = AssetReport.BeginScope(); object[] innerState = ReportState();
            ScopeRejected(outer.Dispose, "asset report out-of-order Dispose rejected"); SameReportState(innerState, "asset report rejected Dispose preserves child");
            ScopeWorkerRejected(inner.Dispose, "asset report foreign Dispose");
            ScopeWorkerRejected(delegate { AssetReport.BeginScope(); }, "asset report concurrent BeginScope");
            inner.Dispose(); SameReportState(outerState, "asset report nested child restores outer");
            outer.Dispose(); SameReportState(parent, "asset report nested scopes restore live parent");
            Settings.Current = parentSettings; parentWriter.WriteComment("parent-after-nested"); AssetReport.Close();
            string parentText = File.ReadAllText(Path.Combine(root, "parent/parent-custom.xml"));
            Check(parentText.Contains("parent-before") && parentText.Contains("parent-after-success") && parentText.Contains("parent-after-failure") &&
                  parentText.Contains("parent-after-nested") && !parentText.Contains("child-success"), "asset report live parent remains writable after child success failure and nesting");
            var document = new XmlDocument(); document.Load(Path.Combine(root, "parent/parent-custom.xml"));
            ReportCleared("asset report scoped parent closes successfully");
        }
        finally { AssetReport.Close(); Settings.Current = previous; AssetReport.FileName = previousName; }
    }
    static int Main(string[] args)
    {
        string root = args.Length > 0 ? args[0] : Path.Combine(Path.GetTempPath(), "compiler-resource-" + Guid.NewGuid().ToString("N"));
        var receipt = new Dictionary<string, object>(); receipt["succeeded"] = false;
        receipt["sourceCommit"] = args.Length > 2 ? args[2] : "local"; receipt["runId"] = args.Length > 3 ? args[3] : "local";
        receipt["checks"] = Checks;
        try
        {
            Assembly own = Assembly.GetEntryAssembly();
            Check(own.GetName().Name == "NewModCompiler" && typeof(MetricManager).Assembly == own && typeof(AssetReport).Assembly == own, "resource fixture uses EXE-owned production types");
            TestMetrics(); TestReports(Path.Combine(root, "reports")); TestReportScopes(Path.Combine(root, "report-scopes"));
            receipt["succeeded"] = true;
            Console.WriteLine("PASS resource lifecycle checks=" + Checks.Count);
            return 0;
        }
        catch (Exception e) { receipt["error"] = e.ToString(); Console.Error.WriteLine(e); return 1; }
        finally { if (args.Length > 1) File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(receipt)); }
    }
}
