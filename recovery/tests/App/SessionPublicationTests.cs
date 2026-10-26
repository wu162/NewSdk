using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Web.Script.Serialization;
using NewModCompiler.Application;
using NewModCompiler.Cli;
using NewModCompiler.Diagnostics;

namespace NewModCompiler.Tests
{
    internal static class SessionPublicationTests
    {
        static int assertions;
        static void Check(bool value,string message) { assertions++; if (!value) throw new Exception("Session publication: "+message); }
        static void Put(string path,string value) { Directory.CreateDirectory(Path.GetDirectoryName(path)); File.WriteAllText(path,value); }
        sealed class Compiler : IStreamCompiler
        {
            public string Payload="new generation", BuildId; public bool FailDispose; public Action<StreamContext> Compiling;
            public string ProviderIdentity { get { return "TEST_ONLY_PUBLICATION_EXECUTOR"; } }
            public void Initialize(BuildRequest request) { }
            public IList<Artifact> Compile(StreamContext context,CancellationToken cancellation)
            {
                BuildId=context.BuildId;
                if (Compiling!=null) Compiling(context);
                string path=Path.Combine(context.StagingDirectory,"payload.bin"); Put(path,Payload);
                Put(Path.Combine(context.StagingDirectory,"unlisted.tmp"),"not an artifact");
                return new[] {new Artifact {Path=path,Kind="TestOnly"}};
            }
            public void Dispose() { if (FailDispose) throw new IOException("injected provider close failure"); }
        }
        sealed class Sink : IBuildEventSink
        {
            public readonly List<IDictionary<string,object>> Events=new List<IDictionary<string,object>>();
            public Action<IDictionary<string,object>> Writing; public Action Closing; public bool FailClose;
            public void Write(IDictionary<string,object> value) { Events.Add(value); if (Writing!=null) Writing(value); }
            public void Dispose() { if (Closing!=null) Closing(); if (FailClose) throw new IOException("injected required close failure"); }
        }
        static BuildRequest Request(string work,string mod,string sdk)
        { return Options.Parse(new[] {"build","--mod",mod,"--sdk-root",sdk,"--out",work,"--package-content","base"}); }
        static Dictionary<string,object> Report(BuildRequest request)
        { return new JavaScriptSerializer().Deserialize<Dictionary<string,object>>(File.ReadAllText(request.ReportPath)); }
        static ExitCode Run(BuildRequest request,Compiler compiler,Sink sink,CancellationToken cancellation=default(CancellationToken))
        {
            using (var events=new EventDispatcher(Guid.NewGuid().ToString("N"),new IBuildEventSink[] {sink,new JsonLineSink(request.EventsPath),new TextLogSink(request.LogPath)}))
                return new BuildSession(request,compiler,events).Run(cancellation);
        }
        static void Clean(BuildRequest request,Compiler compiler)
        {
            Check(!Directory.Exists(Path.Combine(request.OutputRoot,".build",compiler.BuildId)),"owned staging survived finalization");
            Check(Directory.GetDirectories(request.OutputRoot,".publish-*").Length==0,"preparation survived finalization");
            Check(Directory.GetDirectories(request.OutputRoot,".previous-*").Length==0,"unlocked backup survived finalization");
        }
        static void FailedReport(BuildRequest request,ExitCode expected,string publicationStatus)
        {
            var body=Report(request);
            Check(Convert.ToInt32(body["exitCode"])==(int)expected,"failure report/process exit mismatch");
            Check((string)body["publicationStatus"]==publicationStatus,"failure publication status mismatch: "+body["publicationStatus"]);
            Check(new JavaScriptSerializer().Serialize(body["artifacts"])=="[]","failed generation retained accepted artifacts");
        }
        public static int Run(string work,string mod,string sdk)
        {
            // A durable, exact allowlist replaces the whole prior generation.
            var request=Request(Path.Combine(work,"success"),mod,sdk); var compiler=new Compiler(); var sink=new Sink();
            Check(Run(request,compiler,sink)==ExitCode.Success,"first publication failed");
            string published=Path.Combine(request.OutputRoot,"published"), payload=Path.Combine(published,"base","Mod","payload.bin");
            var body=Report(request);
            Check((string)body["publicationStatus"]=="PublishedDirectory" && (string)body["publicationDirectory"]==published,"success receipt lacks final publication");
            Check(File.ReadAllText(payload)==compiler.Payload,"published payload differs");
            Check(Directory.GetFiles(published,"*",SearchOption.AllDirectories).Length==1,"unlisted staging file published");
            Check(new JavaScriptSerializer().Serialize(body["artifacts"]).Contains(published.Replace("\\","\\\\")),"receipt paths did not move to publication");
            Check(sink.Events.Count(x=>(string)x["eventType"]=="StageCompleted" && x.ContainsKey("stageId") && (string)x["stageId"]=="publish")==1,"publication stage missing"); Clean(request,compiler);
            var receiptArtifacts=((System.Collections.IEnumerable)body["artifacts"]).Cast<object>().ToArray();
            var produced=sink.Events.Where(x=>(string)x["eventType"]=="ArtifactProduced").ToArray();
            Check(receiptArtifacts.Length==produced.Length,"publication/produced artifact count differs");
            foreach (Dictionary<string,object> artifact in receiptArtifacts) {
                string path=(string)artifact["Path"], sha=(string)artifact["Sha256"];
                using (var hash=SHA256.Create()) using (var input=File.OpenRead(path))
                    Check(BitConverter.ToString(hash.ComputeHash(input)).Replace("-","").ToLowerInvariant()==sha,"published receipt hash differs from bytes");
                string source=Path.Combine(request.OutputRoot,".build",compiler.BuildId,path.Substring(published.Length+1));
                Check(produced.Any(x=>((Artifact)x["payload"]).Path==source && ((Artifact)x["payload"]).Sha256==sha),"ArtifactProduced source lost its corresponding published relative path/hash");
            }

            Put(Path.Combine(published,"obsolete.bin"),"old extra"); compiler=new Compiler {Payload="replacement generation"};
            Check(Run(request,compiler,new Sink())==ExitCode.Success,"replacement publication failed");
            Check(File.ReadAllText(payload)==compiler.Payload && !File.Exists(Path.Combine(published,"obsolete.bin")),"replacement mixed generations"); Clean(request,compiler);

            foreach (string failure in new[] {"report","terminal","close","cancel-publish","cancel-after-commit","provider-close","occupied"})
            {
                request=Request(Path.Combine(work,failure),mod,sdk); published=Path.Combine(request.OutputRoot,"published");
                string sentinel=Path.Combine(published,"previous.bin"); Put(sentinel,"previous durable generation");
                compiler=new Compiler(); sink=new Sink();
                var cancellation=new CancellationTokenSource(); FileStream held=null;
                if (failure=="report") compiler.Compiling=context=>Directory.CreateDirectory(request.ReportPath);
                if (failure=="terminal") sink.Writing=value=>{if ((string)value["eventType"]=="BuildFinished") throw new IOException("injected terminal failure");};
                if (failure=="close") sink.FailClose=true;
                if (failure=="cancel-publish") sink.Writing=value=>{if ((string)value["eventType"]=="StageStarted" && (string)value["stageId"]=="publish") cancellation.Cancel();};
                if (failure=="cancel-after-commit") sink.Writing=value=>{if ((string)value["eventType"]=="StageCompleted" && (string)value["stageId"]=="publish") cancellation.Cancel();};
                if (failure=="provider-close") compiler.FailDispose=true;
                if (failure=="occupied") held=new FileStream(sentinel,FileMode.Open,FileAccess.Read,FileShare.Read);
                ExitCode expected=failure.StartsWith("cancel-",StringComparison.Ordinal) ? ExitCode.Cancelled : failure=="provider-close" ? ExitCode.Internal : ExitCode.Output;
                try { Check(Run(request,compiler,sink,cancellation.Token)==expected,failure+" exit differs"); }
                finally { if (held!=null) held.Dispose(); cancellation.Dispose(); }
                Check(File.ReadAllText(sentinel)=="previous durable generation",failure+" damaged prior generation");
                Check(Directory.GetFiles(published,"*",SearchOption.AllDirectories).Length==1,failure+" leaked a partial generation");
                if (failure!="report") FailedReport(request,expected,failure=="terminal" || failure=="close" || failure=="cancel-after-commit" ? "RolledBack" : "NotPublished");
                Clean(request,compiler);
                if (failure=="report") Directory.Delete(request.ReportPath);
                compiler=new Compiler {Payload="retry after "+failure};
                Check(Run(request,compiler,new Sink())==ExitCode.Success,failure+" retry failed");
                Check(File.ReadAllText(Path.Combine(published,"base","Mod","payload.bin"))==compiler.Payload && !File.Exists(sentinel),failure+" retry retained stale generation"); Clean(request,compiler);
            }
            // Cancellation after the final cancellation boundary does not contradict
            // a successful terminal event. Required sink failures still revoke output.
            foreach (string point in new[] {"terminal","close"}) {
                request=Request(Path.Combine(work,"late-cancel-"+point),mod,sdk); compiler=new Compiler(); sink=new Sink();
                using (var cancellation=new CancellationTokenSource()) {
                    if (point=="terminal") sink.Writing=value=>{if ((string)value["eventType"]=="BuildFinished") cancellation.Cancel();};
                    else sink.Closing=()=>cancellation.Cancel();
                    Check(Run(request,compiler,sink,cancellation.Token)==ExitCode.Success,"late "+point+" cancellation changed committed outcome");
                    Check(cancellation.IsCancellationRequested,"late "+point+" cancellation callback was not executed");
                }
                Check((string)Report(request)["publicationStatus"]=="PublishedDirectory" && (string)Report(request)["status"]=="succeeded","late cancellation contradicted final report");
                Check(File.Exists(Path.Combine(request.OutputRoot,"published","base","Mod","payload.bin")),"late cancellation revoked committed generation"); Clean(request,compiler);
            }
            // With no selected streams, success must preserve the last generation.
            string emptyMod=Path.Combine(work,"empty-mod"); Put(Path.Combine(emptyMod,"Data","Mod.xml"),"<AssetDeclaration/>");
            request=Options.Parse(new[] {"build","--mod",emptyMod,"--sdk-root",sdk,"--out",Path.Combine(work,"empty"),"--package-content","apt,maps"});
            string retained=Path.Combine(request.OutputRoot,"published","previous.bin"); Put(retained,"retain empty selection");
            compiler=new Compiler(); Check(Run(request,compiler,new Sink())==ExitCode.Success,"empty optional selection failed");
            Check(File.ReadAllText(retained)=="retain empty selection" && (string)Report(request)["publicationStatus"]=="NotPublished","empty selection changed publication");
            foreach (string diagnostic in new[] {"--events","--report","--log"}) {
                bool rejected=false;
                try { Options.Parse(new[] {"build","--mod",mod,"--sdk-root",sdk,"--out",Path.Combine(work,"collision"),diagnostic,Path.Combine(work,"collision","published","log.txt")}); }
                catch (BuildFailure error) { rejected=error.Code=="CLI_LOG_COLLISION"; }
                Check(rejected,"diagnostic inside publication was accepted: "+diagnostic);
            }
            // A first generation also disappears when a required terminal sink fails.
            request=Request(Path.Combine(work,"first-late-failure"),mod,sdk); compiler=new Compiler(); sink=new Sink();
            sink.Writing=value=>{if ((string)value["eventType"]=="BuildFinished") throw new IOException("injected first-generation terminal failure");};
            Check(Run(request,compiler,sink)==ExitCode.Output,"first-generation late failure exit differs");
            Check(!Directory.Exists(Path.Combine(request.OutputRoot,"published")),"first-generation late failure left publication");
            FailedReport(request,ExitCode.Output,"RolledBack"); Clean(request,compiler);
            Console.WriteLine("PASS Session publication: "+assertions+" lifecycle assertions; controlled executor, no real compiler acceptance");
            return assertions;
        }
    }
}
