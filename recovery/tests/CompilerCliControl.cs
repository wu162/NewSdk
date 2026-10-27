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
    [DllImport("kernel32.dll",SetLastError=true)] static extern IntPtr OpenProcess(uint access,bool inherit,uint pid);
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool GetProcessTimes(IntPtr process,out long created,out long exited,out long kernel,out long user);
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
    static List<Dictionary<string,object>> ReadEvents(string path)
    {
        var rows=new List<Dictionary<string,object>>();
        try {
            // Product sinks replace stale files atomically. Observers must never
            // prevent that replacement while scanning an earlier generation.
            using(var input=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.ReadWrite|FileShare.Delete))
            using(var reader=new StreamReader(input,Encoding.UTF8)) {
                string line; while((line=reader.ReadLine())!=null) {
                    try { var row=new JavaScriptSerializer().Deserialize<Dictionary<string,object>>(line); if(row!=null)rows.Add(row); }
                    catch(ArgumentException) { } // A not-yet-flushed final JSON line is retried.
                }
            }
        } catch(IOException) { } // Missing/replaced file is not a build observation.
        return rows;
    }
    static Dictionary<string,object> StreamStarted(string path,uint expectedPid,HashSet<string> previousBuilds,out Dictionary<string,object> buildStarted)
    {
        buildStarted=null;
        foreach(var row in ReadEvents(path)) {
            object eventType,buildId,sequence;
            if(!row.TryGetValue("eventType",out eventType) || !row.TryGetValue("buildId",out buildId) ||
                !row.TryGetValue("sequence",out sequence) || string.IsNullOrEmpty(buildId as string))continue;
            if((string)eventType=="BuildStarted") {
                object payload,pid;
                if(row.TryGetValue("payload",out payload) && payload is Dictionary<string,object> &&
                    ((Dictionary<string,object>)payload).TryGetValue("processId",out pid) &&
                    Convert.ToUInt32(pid)==expectedPid && !previousBuilds.Contains((string)buildId))buildStarted=row;
            } else if((string)eventType=="StreamStarted" && buildStarted!=null &&
                (string)buildId==(string)buildStarted["buildId"] && Convert.ToInt64(sequence)>Convert.ToInt64(buildStarted["sequence"]))return row;
        }
        return null;
    }
    static long Created(IntPtr process)
    {
        long created,exited,kernel,user;
        Check(GetProcessTimes(process,out created,out exited,out kernel,out user),"Cannot query process creation identity");
        return created;
    }
    static bool ChildIdentity(long parentCreated,long childCreated) { return childCreated>=parentCreated; }
    static int ParentIdentityTests()
    {
        if(ChildIdentity(200,100)) throw new Exception("A recycled parent PID attributed an older process");
        if(!ChildIdentity(100,200)) throw new Exception("A real newer child was excluded");
        if(!ChildIdentity(100,100)) throw new Exception("Equal-resolution creation times must retain a possible child");
        if(ChildIdentity(300,200)) throw new Exception("A recursive recycled parent PID attributed an older grandchild");
        if(!ChildIdentity(200,300)) throw new Exception("A recursive real child was excluded");
        using(var current=Process.GetCurrentProcess()) if(Created(current.Handle)<=0) throw new Exception("Real process creation query failed");
        Console.WriteLine("PASS parent process creation identity: 6 assertions"); return 0;
    }
    static void Audit(uint root,long rootCreated,Dictionary<uint,long> descendants,List<object> observed)
    {
        IntPtr snapshot=CreateToolhelp32Snapshot(2,0);
        if(snapshot==new IntPtr(-1)) throw new Exception("Cannot audit process tree");
        try {
            var rows=new List<Entry>();var row=new Entry {Size=(uint)Marshal.SizeOf(typeof(Entry))};
            if(!Process32First(snapshot,ref row)) return;
            do {rows.Add(row);} while(Process32Next(snapshot,ref row));
            // Toolhelp parent PIDs survive parent exit and can point at a newly
            // reused product PID. Creation identity gates every graph edge.
            var current=new Dictionary<uint,long> {{root,rootCreated}};
            bool added;
            do {
                added=false;
                foreach(var item in rows) {
                    long parentCreated;
                    if(item.Pid==root || current.ContainsKey(item.Pid) || !current.TryGetValue(item.Parent,out parentCreated))continue;
                    IntPtr process=OpenProcess(0x1000,false,item.Pid);
                    if(process==IntPtr.Zero) {
                        // A vanished process is absent; access/query failures
                        // cannot silently authorize a suspected descendant.
                        int error=Marshal.GetLastWin32Error();
                        if(error==87)continue;
                        throw new Exception("Cannot query candidate descendant identity: "+item.Pid+" Win32="+error);
                    }
                    long created;
                    try {created=Created(process);} finally {CloseHandle(process);}
                    if(!ChildIdentity(parentCreated,created))continue;
                    current.Add(item.Pid,created);added=true;
                    long previous;
                    if(!descendants.TryGetValue(item.Pid,out previous) || previous!=created) {
                        descendants[item.Pid]=created;
                        observed.Add(new {pid=item.Pid,parentPid=item.Parent,name=item.Name,createdFileTime=created,parentCreatedFileTime=parentCreated});
                    }
                }
            } while(added);
        } finally {CloseHandle(snapshot);}
    }
    public static int Main(string[] args)
    {
        if(args.Length==1 && args[0]=="--test-parent-identity") return ParentIdentityTests();
        var state=new Dictionary<string,object> { {"succeeded",false},{"controllerPid",Process.GetCurrentProcess().Id},{"singleProcessJobLimit",1},{"signalRequested",false},{"signalSent",false},{"streamStartedObserved",false} };
        IntPtr job=IntPtr.Zero,output=IntPtr.Zero,error=IntPtr.Zero,input=IntPtr.Zero;var child=new ProcessInfo();bool allocated=false,finished=false;
        FileStream readLock=null;FileSystemWatcher eventWatcher=null;int freshEvents=0;
        string resultPath=args.Length>1?args[1]:null;int code=1;
        try {
            if(args.Length!=2) throw new Exception("Expected request.json result.json");
            var json=new JavaScriptSerializer();var request=json.Deserialize<Dictionary<string,object>>(File.ReadAllText(args[0]));
            string exe=(string)request["exe"],cwd=(string)request["cwd"],events=(string)request["events"];
            bool cancel=Convert.ToBoolean(request["cancel"]);int timeout=Convert.ToInt32(request["timeoutSeconds"]);
            if(timeout<5 || timeout>600) throw new Exception("Invalid strict process timeout");
            if(request.ContainsKey("holdReadFile")) {
                string path=(string)request["holdReadFile"];
                readLock=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read);
                state["readLockHeld"]=true;state["readLockPath"]=path;
            }
            var previousBuilds=new HashSet<string>(StringComparer.Ordinal);
            foreach(var row in ReadEvents(events)) {object id;if(row.TryGetValue("buildId",out id) && !string.IsNullOrEmpty(id as string))previousBuilds.Add((string)id);}
            state["previousBuildIds"]=previousBuilds.ToArray();
            bool existingEvents=File.Exists(events);state["freshEventsRequired"]=existingEvents;
            if(existingEvents) {
                // Never open the previous target concurrently with CreateFresh's
                // replacement. Observe its file-name publication first; only then
                // read this process's new writer with delete sharing enabled.
                string expectedEvents=Path.GetFullPath(events);
                eventWatcher=new FileSystemWatcher(Path.GetDirectoryName(expectedEvents),Path.GetFileName(expectedEvents));
                eventWatcher.NotifyFilter=NotifyFilters.FileName;
                FileSystemEventHandler created=(sender,change)=>{if(string.Equals(Path.GetFullPath(change.FullPath),expectedEvents,StringComparison.OrdinalIgnoreCase))Interlocked.Exchange(ref freshEvents,1);};
                RenamedEventHandler renamed=(sender,change)=>{if(string.Equals(Path.GetFullPath(change.FullPath),expectedEvents,StringComparison.OrdinalIgnoreCase))Interlocked.Exchange(ref freshEvents,1);};
                eventWatcher.Created+=created;eventWatcher.Renamed+=renamed;
                eventWatcher.EnableRaisingEvents=true;
            } else Interlocked.Exchange(ref freshEvents,1);
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
            long childCreated=Created(child.Process);state["processCreatedFileTime"]=childCreated;
            state["processId"]=child.Pid;state["processGroupId"]=child.Pid;state["creationFlags"]=0x204;
            Check(AssignProcessToJobObject(job,child.Process),"Cannot assign production process to single-process job");
            Check(ResumeThread(child.Thread)!=uint.MaxValue,"Cannot resume production process");
            var timer=Stopwatch.StartNew();var descendants=new Dictionary<uint,long>();var observed=new List<object>();int polls=0;
            while(WaitForSingleObject(child.Process,0)!=0)
            {
                Audit(child.Pid,childCreated,descendants,observed);polls++;
                if(cancel && !(bool)state["signalSent"] && Interlocked.CompareExchange(ref freshEvents,0,0)!=0) {
                    Dictionary<string,object> buildStarted;
                    var started=StreamStarted(events,child.Pid,previousBuilds,out buildStarted);
                    if(started!=null) {
                        state["freshEventsPathObserved"]=true;state["observedBuildStarted"]=buildStarted;state["streamStartedObserved"]=true;state["observedStreamStarted"]=started;state["signalAtMs"]=timer.ElapsedMilliseconds;
                        bool sent=GenerateConsoleCtrlEvent(1,child.Pid);state["signalSent"]=sent;state["signalWin32Error"]=sent?0:Marshal.GetLastWin32Error();
                        Check(sent,"CTRL_BREAK_EVENT could not be sent; cancellation remains unverified");
                    }
                }
                if(timer.ElapsedMilliseconds>timeout*1000L) {state["timedOut"]=true;throw new TimeoutException("Production CLI exceeded strict timeout; kill is not cancellation success");}
                Thread.Sleep(10);
            }
            finished=true;uint exit;Check(GetExitCodeProcess(child.Process,out exit),"Cannot read production exit code");
            Audit(child.Pid,childCreated,descendants,observed);state["processExitCode"]=exit;state["elapsedMs"]=timer.ElapsedMilliseconds;state["auditPolls"]=polls;state["descendants"]=observed.ToArray();
            long createdTime,exitedTime,kernelTime,userTime;
            Check(GetProcessTimes(child.Process,out createdTime,out exitedTime,out kernelTime,out userTime),"Cannot measure completed production process");
            if(createdTime!=childCreated || exitedTime<createdTime || kernelTime<0 || userTime<0)
                throw new Exception("Invalid completed production process timing identity");
            state["processWallMs"]=(exitedTime-createdTime)/10000.0;
            state["processKernelMs"]=kernelTime/10000.0;state["processUserMs"]=userTime/10000.0;
            state["processCpuMs"]=(kernelTime+userTime)/10000.0;
            if(observed.Count!=0) throw new Exception("Production process created descendants");
            if(cancel && (!(bool)state["signalSent"] || exit!=6)) throw new Exception("Real CtrlBreak cancellation was not verified");
            state["succeeded"]=true;code=0;
        } catch(Exception e) {state["error"]=e.ToString();}
        finally {
            if(!finished && child.Process!=IntPtr.Zero) {if(job!=IntPtr.Zero)TerminateJobObject(job,252);TerminateProcess(child.Process,252);WaitForSingleObject(child.Process,5000);}
            foreach(IntPtr handle in new[] {child.Thread,child.Process,job,output,error,input})if(handle!=IntPtr.Zero && handle!=new IntPtr(-1))CloseHandle(handle);
            if(eventWatcher!=null)eventWatcher.Dispose();
            if(readLock!=null)readLock.Dispose();
            if(allocated)FreeConsole();
            if(resultPath!=null)File.WriteAllText(resultPath,new JavaScriptSerializer {MaxJsonLength=Int32.MaxValue}.Serialize(state));
        }
        return code;
    }
}
