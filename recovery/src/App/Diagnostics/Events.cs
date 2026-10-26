using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using NewModCompiler.Application;

namespace NewModCompiler.Diagnostics
{
    public interface IBuildEventSink : IDisposable { void Write(IDictionary<string, object> value); }
    public sealed class JsonLineSink : IBuildEventSink
    {
        readonly StreamWriter writer; readonly JavaScriptSerializer json = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
        public JsonLineSink(string path) { Directory.CreateDirectory(Path.GetDirectoryName(path)); writer = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read), new UTF8Encoding(false)); }
        public void Write(IDictionary<string, object> value) { writer.WriteLine(json.Serialize(value)); writer.Flush(); }
        public void Dispose() { writer.Dispose(); }
    }
    public sealed class TextLogSink : IBuildEventSink
    {
        readonly StreamWriter writer;
        public TextLogSink(string path) { Directory.CreateDirectory(Path.GetDirectoryName(path)); writer = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read), new UTF8Encoding(false)); }
        public void Write(IDictionary<string, object> value) { writer.WriteLine("{0} [{1}] #{2} {3} {4} {5}", value["timestampUtc"],value["buildId"],value["sequence"],value["level"],value["eventType"],value.ContainsKey("message") ? value["message"] : ""); writer.Flush(); }
        public void Dispose() { writer.Dispose(); }
    }
    public sealed class EventDispatcher : IDisposable
    {
        readonly List<IBuildEventSink> sinks; readonly Stopwatch clock = Stopwatch.StartNew();
        long sequence; bool finished; bool outputFailed;
        public string BuildId { get; private set; }
        public bool OutputFailed { get { return outputFailed; } }
        public long ElapsedMilliseconds { get { return clock.ElapsedMilliseconds; } }
        public EventDispatcher(string buildId, IEnumerable<IBuildEventSink> sinks) { BuildId = buildId; this.sinks = new List<IBuildEventSink>(sinks); }
        public void Emit(string type, string level = "info", object payload = null, string message = null, string code = null, string streamId = null, string stageId = null)
        {
            if (finished) throw new InvalidOperationException("No event may follow BuildFinished.");
            if (type == "BuildFinished") finished = true;
            var value = new Dictionary<string, object> { {"schemaVersion","1.0"}, {"buildId",BuildId}, {"sequence",++sequence}, {"timestampUtc",DateTime.UtcNow.ToString("o")}, {"elapsedMs",clock.ElapsedMilliseconds}, {"eventType",type}, {"level",level} };
            if (payload != null) value["payload"] = payload;
            if (message != null) value["message"] = message;
            if (code != null) value["diagnosticCode"] = code;
            if (streamId != null) value["streamId"] = streamId;
            if (stageId != null) value["stageId"] = stageId;
            Exception failure = null;
            foreach (IBuildEventSink sink in sinks) { try { sink.Write(value); } catch (Exception e) { outputFailed = true; failure = e; } }
            // Console is optional; a disconnected caller must not hide the durable final result.
            try { if (type == "Diagnostic" || type == "BuildFinished") Console.Error.WriteLine("[{0}] {1} {2}",level,code ?? type,message ?? ""); } catch (IOException) { }
            if (failure != null) throw new BuildFailure(ExitCode.Output,"EVENT_WRITE_FAILED",failure.Message);
        }
        public void Dispose() { foreach (var sink in sinks) { try { sink.Dispose(); } catch { outputFailed = true; } } }
    }
    public static class Reports
    {
        public static void RemoveStale(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            // A previous build's terminal report must never masquerade as this run's outcome.
            if (File.Exists(path)) File.Delete(path);
        }
        public static void WriteAtomic(string path, IDictionary<string, object> report)
        {
            string temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try {
                using (var stream = new FileStream(temporary,FileMode.CreateNew,FileAccess.Write,FileShare.None)) {
                    byte[] bytes = new UTF8Encoding(false).GetBytes(new JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Serialize(report));
                    stream.Write(bytes,0,bytes.Length); stream.Flush(true);
                }
                if (File.Exists(path)) File.Replace(temporary,path,null); else File.Move(temporary,path);
            } finally { if (File.Exists(temporary)) File.Delete(temporary); }
        }
    }
}
