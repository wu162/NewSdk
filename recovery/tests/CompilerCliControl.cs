using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

// Test-only external controller. It never loads a product assembly. The real
// production EXE gets a console process group and a one-process Windows job.
public static class CompilerCliControl
{
    [StructLayout(LayoutKind.Sequential)] struct Security { public int Length; public IntPtr Descriptor; [MarshalAs(UnmanagedType.Bool)] public bool Inherit; }
    [StructLayout(LayoutKind.Sequential,CharSet=CharSet.Unicode)] struct Startup { public int Size; public string Reserved,Desktop,Title; public uint X,Y,XSize,YSize,XCount,YCount,Fill,Flags; public short Show,ReservedSize; public IntPtr ReservedBytes,Input,Output,Error; }
    [StructLayout(LayoutKind.Sequential)] struct ProcessInfo { public IntPtr Process,Thread; public uint Pid,Tid; }
    [StructLayout(LayoutKind.Sequential)] struct BasicLimits { public long ProcessTime,JobTime; public uint Flags; public UIntPtr MinWorking,MaxWorking; public uint ActiveProcesses; public UIntPtr Affinity; public uint Priority,Scheduling; }
    [StructLayout(LayoutKind.Sequential)] struct Io { public ulong ReadOps,WriteOps,OtherOps,ReadBytes,WriteBytes,OtherBytes; }
    [StructLayout(LayoutKind.Sequential)] struct ExtendedLimits { public BasicLimits Basic; public Io Io; public UIntPtr ProcessMemory,JobMemory,PeakProcessMemory,PeakJobMemory; }
    [StructLayout(LayoutKind.Sequential,CharSet=CharSet.Unicode)] struct Entry { public uint Size,Usage,Pid; public UIntPtr Heap; public uint Module,Threads,Parent; public int Priority; public uint Flags; [MarshalAs(UnmanagedType.ByValTStr,SizeConst=260)] public string Name; }
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool AllocConsole();
    [DllImport("kernel32.dll")] static extern bool FreeConsole();
    [DllImport("kernel32.dll")] static extern uint GetConsoleCP();
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool GenerateConsoleCtrlEvent(uint kind,uint group);
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode,SetLastError=true)] static extern IntPtr CreateFile(string name,uint access,uint share,ref Security security,uint creation,uint flags,IntPtr template);
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode,SetLastError=true)] static extern bool CreateProcess(string application,StringBuilder command,IntPtr processSecurity,IntPtr threadSecurity,bool inherit,uint flags,IntPtr environment,string cwd,ref Startup startup,out ProcessInfo process);
    [DllImport("kernel32.dll",SetLastError=true)] static extern IntPtr CreateJobObject(IntPtr attributes,string name);
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool SetInformationJobObject(IntPtr job,int kind,ref ExtendedLimits information,int size);
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool AssignProcessToJobObject(IntPtr job,IntPtr process);
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool TerminateJobObject(IntPtr job,uint code);
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool TerminateProcess(IntPtr process,uint code);
    [DllImport("kernel32.dll",SetLastError=true)] static extern uint ResumeThread(IntPtr thread);
    [DllImport("kernel32.dll")] static extern uint WaitForSingleObject(IntPtr handle,uint ms);
    [DllImport("kernel32.dll")] static extern bool GetExitCodeProcess(IntPtr process,out uint code);
    [DllImport("kernel32.dll")] static extern bool CloseHandle(IntPtr handle);
    [DllImport("kernel32.dll",SetLastError=true)] static extern IntPtr CreateToolhelp32Snapshot(uint flags,uint pid);
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode)] static extern bool Process32First(IntPtr snapshot,ref Entry entry);
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode)] static extern bool Process32Next(IntPtr snapshot,ref Entry entry);
    static void Check(bool value,string message) { if(!value) throw new Exception(message+" Win32="+Marshal.GetLastWin32Error()); }
    static string Quote(string text)
    {
        var result=new StringBuilder("\""); int slashes=0;
        foreach(char c in text) { if(c=='\\') { slashes++; continue; } if(c=='\"') result.Append('\\',slashes*2+1); else result.Append('\\',slashes); result.Append(c); slashes=0; }
        return result.Append('\\',slashes*2).Append('"').ToString();
    }
    static Dictionary<string,object> StreamStarted(string path)
    {
        if(!File.Exists(path)) return null;
        using(var input=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.ReadWrite))
        using(var reader=new StreamReader(input,Encoding.UTF8))
        {
            string line; while((line=reader.ReadLine())!=null)
            {
                try { var row=new JavaScriptSerializer().Deserialize<Dictionary<string,object>>(line); if((string)row["eventType"]=="StreamStarted") return row; }
                catch(ArgumentException) { }
            }
        }
        return null;
    }
    static void Audit(uint parent,HashSet<uint> descendants,List<object> observed)
    {
        IntPtr snapshot=CreateToolhelp32Snapshot(2,0);
        if(snapshot==new IntPtr(-1)) throw new Exception("Cannot audit process tree");
        try {
            var row=new Entry {Size=(uint)Marshal.SizeOf(typeof(Entry))};
            if(!Process32First(snapshot,ref row)) return;
            do { if((row.Parent==parent || descendants.Contains(row.Parent)) && descendants.Add(row.Pid)) observed.Add(new {pid=row.Pid,parentPid=row.Parent,name=row.Name}); } while(Process32Next(snapshot,ref row));
        } finally {CloseHandle(snapshot);}
    }
    public static int Main(string[] args)
    {
        var state=new Dictionary<string,object> { {"succeeded",false},{"controllerPid",Process.GetCurrentProcess().Id},{"singleProcessJobLimit",1},{"signalRequested",false},{"signalSent",false},{"streamStartedObserved",false} };
        IntPtr job=IntPtr.Zero,output=IntPtr.Zero,error=IntPtr.Zero,input=IntPtr.Zero;var child=new ProcessInfo();bool allocated=false,finished=false;
        string resultPath=args.Length>1?args[1]:null;int code=1;
        try {
            if(args.Length!=2) throw new Exception("Expected request.json result.json");
            var json=new JavaScriptSerializer();var request=json.Deserialize<Dictionary<string,object>>(File.ReadAllText(args[0]));
            string exe=(string)request["exe"],cwd=(string)request["cwd"],events=(string)request["events"];
            bool cancel=Convert.ToBoolean(request["cancel"]);int timeout=Convert.ToInt32(request["timeoutSeconds"]);
            if(timeout<5 || timeout>600) throw new Exception("Invalid strict process timeout");
            state["exe"]=exe;state["signalRequested"]=cancel;state["consoleInitiallyAttached"]=GetConsoleCP()!=0;
            if(GetConsoleCP()==0) { Check(AllocConsole(),"Cannot allocate a console; CtrlBreak remains unverified"); allocated=true; }
            state["allocatedConsole"]=allocated;
            job=CreateJobObject(IntPtr.Zero,null);Check(job!=IntPtr.Zero,"CreateJobObject failed");
            var limits=new ExtendedLimits();limits.Basic.Flags=0x2008;limits.Basic.ActiveProcesses=1;
            Check(SetInformationJobObject(job,9,ref limits,Marshal.SizeOf(typeof(ExtendedLimits))),"Cannot enforce single production process");
            state["singleProcessJobEnforced"]=true;
            var security=new Security {Length=Marshal.SizeOf(typeof(Security)),Inherit=true};
            output=CreateFile((string)request["stdout"],0x40000000,3,ref security,2,0,IntPtr.Zero);
            error=CreateFile((string)request["stderr"],0x40000000,3,ref security,2,0,IntPtr.Zero);
            input=CreateFile("CONIN$",0x80000000,3,ref security,3,0,IntPtr.Zero);
            Check(output!=new IntPtr(-1) && error!=new IntPtr(-1) && input!=new IntPtr(-1),"Cannot open child console/log handles");
            var command=new StringBuilder(Quote(exe));foreach(object argument in (System.Collections.IEnumerable)request["arguments"])command.Append(' ').Append(Quote((string)argument));
            var startup=new Startup {Size=Marshal.SizeOf(typeof(Startup)),Flags=0x100,Input=input,Output=output,Error=error};
            Check(CreateProcess(exe,command,IntPtr.Zero,IntPtr.Zero,true,0x204,IntPtr.Zero,cwd,ref startup,out child),"Cannot create real production process group");
            state["processId"]=child.Pid;state["processGroupId"]=child.Pid;state["creationFlags"]=0x204;
            Check(AssignProcessToJobObject(job,child.Process),"Cannot assign production process to single-process job");
            Check(ResumeThread(child.Thread)!=uint.MaxValue,"Cannot resume production process");
            var timer=Stopwatch.StartNew();var descendants=new HashSet<uint>();var observed=new List<object>();int polls=0;
            while(WaitForSingleObject(child.Process,0)!=0)
            {
                Audit(child.Pid,descendants,observed);polls++;
                if(cancel && !(bool)state["signalSent"]) {
                    var started=StreamStarted(events);
                    if(started!=null) {
                        state["streamStartedObserved"]=true;state["observedStreamStarted"]=started;state["signalAtMs"]=timer.ElapsedMilliseconds;
                        bool sent=GenerateConsoleCtrlEvent(1,child.Pid);state["signalSent"]=sent;state["signalWin32Error"]=sent?0:Marshal.GetLastWin32Error();
                        Check(sent,"CTRL_BREAK_EVENT could not be sent; cancellation remains unverified");
                    }
                }
                if(timer.ElapsedMilliseconds>timeout*1000L) {state["timedOut"]=true;throw new TimeoutException("Production CLI exceeded strict timeout; kill is not cancellation success");}
                Thread.Sleep(10);
            }
            finished=true;uint exit;Check(GetExitCodeProcess(child.Process,out exit),"Cannot read production exit code");
            Audit(child.Pid,descendants,observed);state["processExitCode"]=exit;state["elapsedMs"]=timer.ElapsedMilliseconds;state["auditPolls"]=polls;state["descendants"]=observed.ToArray();
            if(observed.Count!=0) throw new Exception("Production process created descendants");
            if(cancel && (!(bool)state["signalSent"] || exit!=6)) throw new Exception("Real CtrlBreak cancellation was not verified");
            state["succeeded"]=true;code=0;
        } catch(Exception e) {state["error"]=e.ToString();}
        finally {
            if(!finished && child.Process!=IntPtr.Zero) {if(job!=IntPtr.Zero)TerminateJobObject(job,252);TerminateProcess(child.Process,252);WaitForSingleObject(child.Process,5000);}
            foreach(IntPtr handle in new[] {child.Thread,child.Process,job,output,error,input})if(handle!=IntPtr.Zero && handle!=new IntPtr(-1))CloseHandle(handle);
            if(allocated)FreeConsole();
            if(resultPath!=null)File.WriteAllText(resultPath,new JavaScriptSerializer {MaxJsonLength=Int32.MaxValue}.Serialize(state));
        }
        return code;
    }
}
