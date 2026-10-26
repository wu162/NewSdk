using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Text;
using System.Web.Script.Serialization;
using NewModCompiler.Application;

namespace NewModCompiler.Diagnostics
{
    // Only ordinary directory paths are accepted for writes. Each ancestor is
    // held without FILE_SHARE_WRITE/DELETE until the write finishes, so an existing
    // directory cannot be renamed or replaced with a junction after validation.
    internal sealed class DiagnosticDirectory : IDisposable
    {
        readonly List<SafeFileHandle> handles = new List<SafeFileHandle>();
        const uint BackupSemantics = 0x02000000, OpenReparsePoint = 0x00200000;
        [StructLayout(LayoutKind.Sequential)] struct FileInformation
        {
            public uint Attributes; public System.Runtime.InteropServices.ComTypes.FILETIME Created, Accessed, Written;
            public uint Volume, SizeHigh, SizeLow, Links, IndexHigh, IndexLow;
        }
        [DllImport("kernel32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        static extern SafeFileHandle CreateFile(string path,uint access,uint share,IntPtr security,uint disposition,uint flags,IntPtr template);
        [DllImport("kernel32.dll",EntryPoint="CreateDirectoryW",CharSet=CharSet.Unicode,SetLastError=true)]
        static extern bool CreateDirectoryNative(string path,IntPtr security);
        [DllImport("kernel32.dll", SetLastError=true)]
        static extern bool GetFileInformationByHandle(SafeFileHandle file,out FileInformation information);
        [DllImport("kernel32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        static extern uint GetFinalPathNameByHandle(SafeFileHandle file,StringBuilder path,uint length,uint flags);
        static IOException Error(string path) { return new IOException("Cannot secure diagnostic directory " + path, new Win32Exception(Marshal.GetLastWin32Error())); }
        internal static void ValidateSyntax(string path)
        {
            path=Path.GetFullPath(path);
            if (path.StartsWith(@"\\?\",StringComparison.Ordinal) || path.StartsWith(@"\\.\",StringComparison.Ordinal)) throw new IOException("Device paths are unavailable for diagnostic output: " + path);
            string root=Path.GetPathRoot(path);
            foreach (string part in path.Substring(root.Length).Split(new [] {'\\','/'})) {
                if (part.Length==0) continue;
                string name=part.Split('.')[0];
                if (part.EndsWith(".",StringComparison.Ordinal) || part.EndsWith(" ",StringComparison.Ordinal) || part.IndexOf(':')>=0 ||
                    string.Equals(name,"CON",StringComparison.OrdinalIgnoreCase) || string.Equals(name,"PRN",StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(name,"AUX",StringComparison.OrdinalIgnoreCase) || string.Equals(name,"NUL",StringComparison.OrdinalIgnoreCase) ||
                    (name.Length==4 && (name.StartsWith("COM",StringComparison.OrdinalIgnoreCase) || name.StartsWith("LPT",StringComparison.OrdinalIgnoreCase)) && name[3]>='1' && name[3]<='9'))
                    throw new IOException("Ambiguous or device diagnostic path component: " + part);
            }
        }
        SafeFileHandle Open(string path,bool allowReparse,bool requireDirectory=true)
        {
            var handle=CreateFile(path,0,1,IntPtr.Zero,3,BackupSemantics | (allowReparse ? 0 : OpenReparsePoint),IntPtr.Zero);
            if (handle.IsInvalid) { handle.Dispose(); throw Error(path); }
            FileInformation information;
            if (!GetFileInformationByHandle(handle,out information)) { handle.Dispose(); throw Error(path); }
            if ((requireDirectory && (information.Attributes & 0x10)==0) || (!allowReparse && (information.Attributes & 0x400)!=0)) { handle.Dispose(); throw new IOException("Diagnostic path contains a reparse point or non-directory: " + path); }
            handles.Add(handle); return handle;
        }
        static string Physical(SafeFileHandle handle)
        {
            var value=new StringBuilder(32768); uint count=GetFinalPathNameByHandle(handle,value,(uint)value.Capacity,0);
            if (count==0 || count>=value.Capacity) throw Error("physical path");
            string path=value.ToString();
            if (path.StartsWith(@"\\?\UNC\",StringComparison.OrdinalIgnoreCase)) return @"\\"+path.Substring(8);
            return path.StartsWith(@"\\?\",StringComparison.Ordinal) ? path.Substring(4) : path;
        }
        internal static string ResolveInputDirectory(string path)
        {
            using (var guard=new DiagnosticDirectory()) return Physical(guard.Open(Path.GetFullPath(path),true));
        }
        // Resolve existing directory ancestors, preserving any missing suffix.
        // This also catches DOS short-name aliases during read-only CLI parsing.
        internal static string ResolveOutput(string path)
        {
            ValidateSyntax(path); path=Path.GetFullPath(path);
            if (Directory.Exists(path)) {
                using (var existing=Acquire(path,false)) return Physical(existing.handles[existing.handles.Count-1]);
            }
            string directory=Path.GetDirectoryName(path), suffix=Path.GetFileName(path);
            while (!Directory.Exists(directory)) {
                if (File.Exists(directory)) throw new IOException("Diagnostic ancestor is a file: " + directory);
                suffix=Path.Combine(Path.GetFileName(directory),suffix); directory=Path.GetDirectoryName(directory);
            }
            using (var guard=Acquire(directory,false)) {
                if (File.Exists(path)) return Physical(guard.Open(path,false,false));
                return Path.Combine(Physical(guard.handles[guard.handles.Count-1]),suffix);
            }
        }
        internal static DiagnosticDirectory Acquire(string directory,bool create)
        {
            ValidateSyntax(directory); directory=Path.GetFullPath(directory);
            var guard=new DiagnosticDirectory();
            try {
                string current=Path.GetPathRoot(directory); guard.Open(current,false);
                foreach (string part in directory.Substring(current.Length).Split(new [] {'\\','/'},StringSplitOptions.RemoveEmptyEntries)) {
                    current=Path.Combine(current,part);
                    // Create one component only: recursive managed creation may
                    // traverse a dangling junction before we can inspect it.
                    if (create && !CreateDirectoryNative(current,IntPtr.Zero) && Marshal.GetLastWin32Error()!=183) throw Error(current);
                    guard.Open(current,false);
                }
                return guard;
            } catch { guard.Dispose(); throw; }
        }
        public void Dispose() { for (int i=handles.Count-1;i>=0;--i) handles[i].Dispose(); handles.Clear(); }
    }
    internal static class DiagnosticFiles
    {
        internal static void RejectReparseFile(string path)
        {
            try { if ((File.GetAttributes(path) & FileAttributes.ReparsePoint)!=0) throw new IOException("Diagnostic file is a reparse point: " + path); }
            catch (FileNotFoundException) { } catch (DirectoryNotFoundException) { }
        }
        internal static FileStream CreateFresh(string path,out DiagnosticDirectory guard)
        {
            DiagnosticDirectory.ValidateSyntax(path); guard=DiagnosticDirectory.Acquire(Path.GetDirectoryName(path),true);
            string temporary=path+"."+Guid.NewGuid().ToString("N")+".tmp"; FileStream stream=null;
            try {
                RejectReparseFile(path);
                // Never truncate an existing inode: it may be a hard link to a
                // source file. Keep writing the newly created inode after rename.
                stream=new FileStream(temporary,FileMode.CreateNew,FileAccess.Write,FileShare.Read|FileShare.Delete);
                if (File.Exists(path)) File.Replace(temporary,path,null); else File.Move(temporary,path);
                return stream;
            } catch { if (stream!=null) stream.Dispose(); if (File.Exists(temporary)) File.Delete(temporary); guard.Dispose(); throw; }
        }
    }
    public interface IBuildEventSink : IDisposable { void Write(IDictionary<string, object> value); }
    public sealed class JsonLineSink : IBuildEventSink
    {
        readonly DiagnosticDirectory directory;
        readonly StreamWriter writer; readonly JavaScriptSerializer json = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
        public JsonLineSink(string path) { writer = new StreamWriter(DiagnosticFiles.CreateFresh(path,out directory), new UTF8Encoding(false)); }
        public void Write(IDictionary<string, object> value) { writer.WriteLine(json.Serialize(value)); writer.Flush(); }
        public void Dispose() { try { writer.Dispose(); } finally { directory.Dispose(); } }
    }
    public sealed class TextLogSink : IBuildEventSink
    {
        readonly DiagnosticDirectory directory;
        readonly StreamWriter writer;
        public TextLogSink(string path) { writer = new StreamWriter(DiagnosticFiles.CreateFresh(path,out directory), new UTF8Encoding(false)); }
        public void Write(IDictionary<string, object> value) { writer.WriteLine("{0} [{1}] #{2} {3} {4} {5}", value["timestampUtc"],value["buildId"],value["sequence"],value["level"],value["eventType"],value.ContainsKey("message") ? value["message"] : ""); writer.Flush(); }
        public void Dispose() { try { writer.Dispose(); } finally { directory.Dispose(); } }
    }
    public sealed class EventDispatcher : IDisposable
    {
        readonly List<IBuildEventSink> sinks; readonly Stopwatch clock = Stopwatch.StartNew();
        long sequence; bool finished; bool outputFailed; bool disposed;
        public string BuildId { get; private set; }
        public bool OutputFailed { get { return outputFailed; } }
        public bool CloseFailed { get; private set; }
        public string CloseFailureMessage { get; private set; }
        public long ElapsedMilliseconds { get { return clock.ElapsedMilliseconds; } }
        public EventDispatcher(string buildId, IEnumerable<IBuildEventSink> sinks) { BuildId = buildId; this.sinks = new List<IBuildEventSink>(sinks); }
        public void Emit(string type, string level = "info", object payload = null, string message = null, string code = null, string streamId = null, string stageId = null)
        {
            if (disposed) throw new ObjectDisposedException("EventDispatcher");
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
        public void Dispose()
        {
            if (disposed) return;
            disposed=true;
            foreach (var sink in sinks) {
                try { sink.Dispose(); }
                catch (Exception e) { outputFailed=true; CloseFailed=true; CloseFailureMessage=e.Message; }
            }
        }
    }
    public static class Reports
    {
        public static void RemoveStale(string path)
        {
            DiagnosticDirectory.ValidateSyntax(path);
            using (var guard=DiagnosticDirectory.Acquire(Path.GetDirectoryName(path),true)) {
                DiagnosticFiles.RejectReparseFile(path);
                // Unlinking a hard link does not modify the linked source inode.
                if (File.Exists(path)) File.Delete(path);
            }
        }
        public static void WriteAtomic(string path, IDictionary<string, object> report)
        {
            DiagnosticDirectory.ValidateSyntax(path);
            using (var guard=DiagnosticDirectory.Acquire(Path.GetDirectoryName(path),true)) {
            string temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try {
                DiagnosticFiles.RejectReparseFile(path);
                using (var stream = new FileStream(temporary,FileMode.CreateNew,FileAccess.Write,FileShare.None)) {
                    byte[] bytes = new UTF8Encoding(false).GetBytes(new JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Serialize(report));
                    stream.Write(bytes,0,bytes.Length); stream.Flush(true);
                }
                if (File.Exists(path)) File.Replace(temporary,path,null); else File.Move(temporary,path);
            } finally { if (File.Exists(temporary)) File.Delete(temporary); }
            }
        }
    }
}
