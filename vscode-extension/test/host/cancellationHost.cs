// Test-only controller; it launches the supplied release EXE without loading or building compiler assemblies.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

public static class CancellationHost {
    [StructLayout(LayoutKind.Sequential)] struct Security {
        public int Length; public IntPtr Descriptor; [MarshalAs(UnmanagedType.Bool)] public bool Inherit;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)] struct Startup {
        public int Size; public string Reserved, Desktop, Title;
        public uint X, Y, XSize, YSize, XCount, YCount, Fill, Flags;
        public short Show, ReservedSize; public IntPtr ReservedBytes, Input, Output, Error;
    }
    [StructLayout(LayoutKind.Sequential)] struct ProcessInfo { public IntPtr Process, Thread; public uint Pid, Tid; }
    [StructLayout(LayoutKind.Sequential)] struct BasicLimits {
        public long ProcessTime, JobTime; public uint Flags; public UIntPtr MinWorking, MaxWorking;
        public uint ActiveProcesses; public UIntPtr Affinity; public uint Priority, Scheduling;
    }
    [StructLayout(LayoutKind.Sequential)] struct Io { public ulong ReadOps, WriteOps, OtherOps, ReadBytes, WriteBytes, OtherBytes; }
    [StructLayout(LayoutKind.Sequential)] struct ExtendedLimits {
        public BasicLimits Basic; public Io Io; public UIntPtr ProcessMemory, JobMemory, PeakProcessMemory, PeakJobMemory;
    }
    [DllImport("kernel32.dll", SetLastError = true)] static extern bool AllocConsole();
    [DllImport("kernel32.dll")] static extern bool FreeConsole();
    [DllImport("kernel32.dll")] static extern uint GetConsoleCP();
    [DllImport("kernel32.dll", SetLastError = true)] static extern bool GenerateConsoleCtrlEvent(uint kind, uint group);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)] static extern IntPtr CreateFile(
        string name, uint access, uint share, ref Security security, uint creation, uint flags, IntPtr template);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)] static extern bool CreateProcess(
        string application, StringBuilder command, IntPtr processSecurity, IntPtr threadSecurity, bool inherit,
        uint flags, IntPtr environment, string cwd, ref Startup startup, out ProcessInfo process);
    [DllImport("kernel32.dll", SetLastError = true)] static extern IntPtr CreateJobObject(IntPtr attributes, string name);
    [DllImport("kernel32.dll", SetLastError = true)] static extern bool SetInformationJobObject(
        IntPtr job, int kind, ref ExtendedLimits information, int size);
    [DllImport("kernel32.dll", SetLastError = true)] static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);
    [DllImport("kernel32.dll", SetLastError = true)] static extern bool TerminateJobObject(IntPtr job, uint code);
    [DllImport("kernel32.dll", SetLastError = true)] static extern bool TerminateProcess(IntPtr process, uint code);
    [DllImport("kernel32.dll", SetLastError = true)] static extern uint ResumeThread(IntPtr thread);
    [DllImport("kernel32.dll")] static extern uint WaitForSingleObject(IntPtr handle, uint ms);
    [DllImport("kernel32.dll")] static extern bool GetExitCodeProcess(IntPtr process, out uint code);
    [DllImport("kernel32.dll")] static extern bool CloseHandle(IntPtr handle);

    static void Check(bool condition, string message) {
        if (!condition) throw new Exception(message + " Win32=" + Marshal.GetLastWin32Error());
    }
    static string Quote(string text) {
        var result = new StringBuilder("\""); int slashes = 0;
        foreach (char c in text) {
            if (c == '\\') { slashes++; continue; }
            result.Append('\\', c == '"' ? slashes * 2 + 1 : slashes);
            result.Append(c); slashes = 0;
        }
        return result.Append('\\', slashes * 2).Append('"').ToString();
    }
    static List<Dictionary<string, object>> ReadEvents(string path) {
        var rows = new List<Dictionary<string, object>>();
        try {
            using (var input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            using (var reader = new StreamReader(input, Encoding.UTF8)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    try {
                        var row = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(line);
                        if (row != null) rows.Add(row);
                    } catch (ArgumentException) { } // Retry an incomplete final line on the next poll.
                }
            }
        } catch (IOException) { } // Missing/replaced output is not a build observation yet.
        return rows;
    }
    static Dictionary<string, object> FindCurrentStream(string path, uint pid, HashSet<string> oldIds,
        out Dictionary<string, object> started) {
        started = null;
        foreach (var row in ReadEvents(path)) {
            object type, id;
            if (!row.TryGetValue("eventType", out type) || !row.TryGetValue("buildId", out id) || !(id is string)) continue;
            if ((string)type == "BuildStarted") {
                object payload, processId;
                if (!oldIds.Contains((string)id) && row.TryGetValue("payload", out payload) &&
                    payload is Dictionary<string, object> &&
                    ((Dictionary<string, object>)payload).TryGetValue("processId", out processId) &&
                    Convert.ToUInt32(processId) == pid) started = row;
            } else if ((string)type == "StreamStarted" && started != null &&
                (string)id == (string)started["buildId"] &&
                Convert.ToInt64(row["sequence"]) > Convert.ToInt64(started["sequence"])) return row;
        }
        return null;
    }
    public static int Main(string[] args) {
        var result = new Dictionary<string, object> {
            {"succeeded", false}, {"signalSent", false}, {"signal", "CTRL_BREAK_EVENT"},
            {"controllerPid", Process.GetCurrentProcess().Id}, {"streamStartedObserved", false}
        };
        string resultPath = args.Length == 2 ? args[1] : null;
        IntPtr job = IntPtr.Zero, output = IntPtr.Zero, error = IntPtr.Zero, input = IntPtr.Zero;
        var child = new ProcessInfo(); bool allocated = false, exited = false; int outcome = 1;
        try {
            if (args.Length != 2) throw new Exception("Expected request.json result.json");
            var request = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(File.ReadAllText(args[0]));
            string executable = (string)request["compilerPath"], eventsPath = (string)request["eventsPath"];
            int timeout = Convert.ToInt32(request["timeoutSeconds"]);
            if (timeout < 5 || timeout > 300) throw new Exception("Invalid process timeout");
            var oldIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (var row in ReadEvents(eventsPath)) {
                object id; if (row.TryGetValue("buildId", out id) && id is string) oldIds.Add((string)id);
            }
            if (GetConsoleCP() == 0) { Check(AllocConsole(), "Cannot allocate console"); allocated = true; }
            result["allocatedConsole"] = allocated;
            job = CreateJobObject(IntPtr.Zero, null); Check(job != IntPtr.Zero, "Cannot create process job");
            var limits = new ExtendedLimits(); limits.Basic.Flags = 0x2000; // KILL_ON_JOB_CLOSE for timeout/error cleanup only.
            Check(SetInformationJobObject(job, 9, ref limits, Marshal.SizeOf(typeof(ExtendedLimits))), "Cannot configure process job");
            var security = new Security { Length = Marshal.SizeOf(typeof(Security)), Inherit = true };
            output = CreateFile((string)request["stdoutPath"], 0x40000000, 3, ref security, 2, 0, IntPtr.Zero);
            error = CreateFile((string)request["stderrPath"], 0x40000000, 3, ref security, 2, 0, IntPtr.Zero);
            input = CreateFile("CONIN$", 0x80000000, 3, ref security, 3, 0, IntPtr.Zero);
            Check(output != new IntPtr(-1) && error != new IntPtr(-1) && input != new IntPtr(-1), "Cannot open console/log handles");
            var command = new StringBuilder(Quote(executable));
            foreach (object argument in (IEnumerable)request["arguments"]) command.Append(' ').Append(Quote((string)argument));
            var startup = new Startup { Size = Marshal.SizeOf(typeof(Startup)), Flags = 0x100, Input = input, Output = output, Error = error };
            // CREATE_SUSPENDED | CREATE_NEW_PROCESS_GROUP. The child inherits this console.
            Check(CreateProcess(executable, command, IntPtr.Zero, IntPtr.Zero, true, 0x204, IntPtr.Zero,
                (string)request["cwd"], ref startup, out child), "Cannot create packaged compiler process group");
            result["processId"] = child.Pid; result["processGroupId"] = child.Pid; result["creationFlags"] = 0x204;
            Check(AssignProcessToJobObject(job, child.Process), "Cannot assign compiler to cleanup job");
            Check(ResumeThread(child.Thread) != uint.MaxValue, "Cannot resume packaged compiler");
            var timer = Stopwatch.StartNew();
            while (WaitForSingleObject(child.Process, 0) != 0) {
                if (!(bool)result["signalSent"]) {
                    Dictionary<string, object> started;
                    var stream = FindCurrentStream(eventsPath, child.Pid, oldIds, out started);
                    if (stream != null) {
                        result["buildId"] = started["buildId"]; result["observedBuildStarted"] = started;
                        result["observedStreamStarted"] = stream; result["streamStartedObserved"] = true;
                        result["signalAtMs"] = timer.ElapsedMilliseconds;
                        bool sent = GenerateConsoleCtrlEvent(1, child.Pid);
                        result["signalSent"] = sent; result["signalWin32Error"] = sent ? 0 : Marshal.GetLastWin32Error();
                        Check(sent, "Cannot send CTRL_BREAK_EVENT; safe cancellation remains unverified");
                    }
                }
                if (timer.ElapsedMilliseconds > timeout * 1000L) throw new TimeoutException("Compiler exceeded cancellation timeout; forced cleanup is not cancellation success");
                Thread.Sleep(5);
            }
            exited = true; uint exitCode;
            Check(GetExitCodeProcess(child.Process, out exitCode), "Cannot query packaged compiler exit code");
            result["processExitCode"] = exitCode; result["elapsedMs"] = timer.ElapsedMilliseconds;
            if (!(bool)result["signalSent"] || exitCode != 6) throw new Exception("Expected a delivered Ctrl+Break and normal cancelled exit code 6");
            result["succeeded"] = true; outcome = 0;
        } catch (Exception failure) { result["error"] = failure.ToString(); }
        finally {
            if (!exited && child.Process != IntPtr.Zero) {
                if (job != IntPtr.Zero) TerminateJobObject(job, 252);
                TerminateProcess(child.Process, 252); WaitForSingleObject(child.Process, 5000);
                result["forcedCleanup"] = true;
            }
            foreach (IntPtr handle in new[] { child.Thread, child.Process, job, output, error, input }) {
                if (handle != IntPtr.Zero && handle != new IntPtr(-1)) CloseHandle(handle);
            }
            if (allocated) FreeConsole();
            if (resultPath != null) File.WriteAllText(resultPath, new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue }.Serialize(result));
        }
        return outcome;
    }
}
