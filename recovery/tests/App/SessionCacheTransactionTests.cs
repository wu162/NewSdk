using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web.Script.Serialization;
using NewModCompiler.Application;
using NewModCompiler.Cli;
using NewModCompiler.Diagnostics;

namespace NewModCompiler.Tests
{
    internal static class SessionCacheTransactionTests
    {
        static int count;
        static void Check(bool value,string message) { count++; if (!value) throw new Exception("Cache transaction: "+message); }
        sealed class Backend : IStreamCompiler, IBuildCacheCompiler
        {
            public bool Committed, Completed, Disposed, FailCommit, FailCompile, FailDispose;
            public bool RollbackObserved; public string Status="Pending";
            public object CacheReport { get { return new {status=Status,hits=2,misses=1}; } }
            public string ProviderIdentity { get { return "TEST_CACHE_TRANSACTION"; } }
            public void Initialize(BuildRequest request) { }
            public IList<Artifact> Compile(StreamContext context,CancellationToken token)
            {
                if (FailCompile) throw new BuildFailure(ExitCode.Compilation,"TEST_COMPILE","injected compile failure");
                string path=Path.Combine(context.StagingDirectory,"test.bin"); File.WriteAllText(path,"new");
                return new [] {new Artifact {Path=path,Kind="test"}};
            }
            public void Dispose() { Disposed=true; if (FailDispose) throw new IOException("dispose"); }
            public void CommitCache() { Check(Disposed,"cache installed before provider disposal"); Committed=true; Status="Committed"; if (FailCommit) throw new IOException("commit"); }
            public void RollbackCache() { if (Completed) return; RollbackObserved=true; Committed=false; Status="Discarded"; }
            public void CompleteCache() { Check(Committed,"completed uncommitted cache"); Completed=true; }
        }
        sealed class Sink : IBuildEventSink
        {
            public Backend Backend; public bool FailClose, FailTerminal, BlockReport; public BuildRequest Request;
            public void Write(IDictionary<string,object> value)
            {
                string type=(string)value["eventType"];
                if (type=="StageCompleted" && value.ContainsKey("stageId") && (string)value["stageId"]=="publish" && BlockReport)
                    Directory.CreateDirectory(Request.ReportPath);
                if (type=="BuildFinished") { Check(!Backend.Completed,"cache finalized before terminal output"); if (FailTerminal) throw new IOException("terminal"); }
            }
            public void Dispose() { Check(!Backend.Completed,"cache finalized before required sink close"); if (FailClose) throw new IOException("close"); }
        }
        public static int Run(string work,string mod,string sdk)
        {
            for (int mode=0;mode<7;mode++) {
                var request=Options.Parse(new [] {"build","--mod",mod,"--sdk-root",sdk,"--out",Path.Combine(work,"case"+mode),"--package-content","base"});
                string old=Path.Combine(request.OutputRoot,"published","old.txt"); Directory.CreateDirectory(Path.GetDirectoryName(old)); File.WriteAllText(old,"previous");
                var backend=new Backend {FailCommit=mode==1,FailCompile=mode==5,FailDispose=mode==6};
                var sink=new Sink {Backend=backend,Request=request,FailClose=mode==2,FailTerminal=mode==3,BlockReport=mode==4};
                ExitCode result;
                using (var events=new EventDispatcher(Guid.NewGuid().ToString("N"),new IBuildEventSink[] {sink}))
                    result=new BuildSession(request,backend,events).Run(CancellationToken.None);
                if (mode==0) {
                    Check(result==ExitCode.Success && backend.Completed && backend.Committed,"successful build failed to finalize cache");
                    Check(!File.Exists(old),"successful publication retained stale file");
                    var report=new JavaScriptSerializer().Deserialize<Dictionary<string,object>>(File.ReadAllText(request.ReportPath));
                    var cache=(Dictionary<string,object>)report["cache"];
                    Check((string)cache["status"]=="Committed" && Convert.ToInt32(cache["hits"])==2,"cache report lost observed counts");
                } else {
                    Check(result!=ExitCode.Success && !backend.Completed && !backend.Committed && backend.RollbackObserved,"failure retained cache installation");
                    Check(File.ReadAllText(old)=="previous","failure did not restore previous publication");
                }
            }
            return count;
        }
    }
}
