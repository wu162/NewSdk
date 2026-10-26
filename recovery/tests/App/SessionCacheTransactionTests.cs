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
            public bool Committed, Completed, Disposed, FailCommit, FailCompile, FailDispose, FailRollback;
            public bool RollbackObserved; public string Status="Pending";
            public CancellationTokenSource CancelAfterStage;
            SessionPackage package;
            public object CacheReport { get { return new {status=Status,hits=2,misses=1}; } }
            public string ProviderIdentity { get { return "TEST_CACHE_TRANSACTION"; } }
            public void Initialize(BuildRequest request) { package=new SessionPackage(request.OutputRoot,new string('e',64)); }
            public IList<Artifact> Compile(StreamContext context,CancellationToken token)
            {
                if (FailCompile) throw new BuildFailure(ExitCode.Compilation,"TEST_COMPILE","injected compile failure");
                string path=Path.Combine(context.StagingDirectory,"test.bin"); File.WriteAllText(path,"new");
                string session=Path.Combine(context.StagingDirectory,"hashes","session.xml"); Directory.CreateDirectory(Path.GetDirectoryName(session)); File.WriteAllText(session,"<sc d=\"2026-10-27\"><fic/></sc>");
                string asset=Path.Combine(context.StagingDirectory,"intermediate","one.asset"); Directory.CreateDirectory(Path.GetDirectoryName(asset)); File.WriteAllText(asset,"asset");
                package.StageStream(context.Task.Id,context.StagingDirectory,token);
                if(CancelAfterStage!=null) CancelAfterStage.Cancel();
                return new [] {new Artifact {Path=path,Kind="test"}};
            }
            public void Dispose() { Disposed=true; if (FailDispose) throw new IOException("dispose"); }
            public void CommitCache() { Check(Disposed,"cache installed before provider disposal"); package.Commit(); Committed=true; Status="Committed"; if (FailCommit) throw new IOException("commit"); }
            public void RollbackCache() { if (Completed) return; RollbackObserved=true; Committed=false; Status="Discarded"; if(package!=null) { package.Rollback(); package.Dispose(); package=null; } if (FailRollback) throw new IOException("persistent rollback failure"); }
            public void CompleteCache() { Check(Committed,"completed uncommitted cache"); package.Complete(); package.Dispose(); package=null; Completed=true; }
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
            for (int mode=0;mode<9;mode++) {
                var cancellation=new CancellationTokenSource();
                var request=Options.Parse(new [] {"build","--mod",mod,"--sdk-root",sdk,"--out",Path.Combine(work,"case"+mode),"--package-content","base"});
                string old=Path.Combine(request.OutputRoot,"published","old.txt"); Directory.CreateDirectory(Path.GetDirectoryName(old)); File.WriteAllText(old,"previous");
                var backend=new Backend {FailCommit=mode==1 || mode==7,FailCompile=mode==5,FailDispose=mode==6,FailRollback=mode==7,CancelAfterStage=mode==8 ? cancellation : null};
                var sink=new Sink {Backend=backend,Request=request,FailClose=mode==2,FailTerminal=mode==3,BlockReport=mode==4};
                ExitCode result;
                using (var events=new EventDispatcher(Guid.NewGuid().ToString("N"),new IBuildEventSink[] {sink}))
                    result=new BuildSession(request,backend,events).Run(cancellation.Token);
                if (mode==0) {
                    Check(result==ExitCode.Success && backend.Completed && backend.Committed,"successful build failed to finalize cache");
                    Check(!File.Exists(old),"successful publication retained stale file");
                    var report=new JavaScriptSerializer().Deserialize<Dictionary<string,object>>(File.ReadAllText(request.ReportPath));
                    var cache=(Dictionary<string,object>)report["cache"];
                    Check((string)cache["status"]=="Committed" && Convert.ToInt32(cache["hits"])==2,"cache report lost observed counts");
                } else {
                    Check((mode!=7 || result==ExitCode.Output) && result!=ExitCode.Success && !backend.Completed && !backend.Committed && backend.RollbackObserved,"failure retained cache installation");
                    Check(File.ReadAllText(old)=="previous","failure did not restore previous publication");
                    if(mode==8) Check(result==ExitCode.Cancelled,"Staged session package cancellation lost its exit code");
                }
                string cacheRoot=Path.Combine(request.OutputRoot,".cache","session-packages-v1");
                Check(!Directory.Exists(cacheRoot) || Directory.GetDirectories(cacheRoot).Length==(mode==0 ? 1 : 0),"Failed session retained a durable package or successful session lost its package");
                using(var retry=new SessionPackage(request.OutputRoot,new string('e',64))) {
                    string retryRoot=Path.Combine(request.OutputRoot,".build","retry");
                    Check(retry.PrepareStream("base/Mod",retryRoot,CancellationToken.None)==(mode==0),"Same-path retry reused an unconfirmed/failed session package");
                }
                cancellation.Dispose();
            }
            return count;
        }
    }
}
