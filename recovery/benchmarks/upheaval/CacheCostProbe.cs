using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Web.Script.Serialization;
using NewModCompiler.Application;

partial class CacheCostProbe {
    static string runtime;
    static readonly List<object> measurements=new List<object>();
    public class Command { public string[] arguments {get;set;} }
    static int Main(string[] args) {
        try {
            runtime=Path.GetFullPath(args[0]);
            return Run(args);
        } catch(Exception e) { Console.Error.WriteLine(e);return 1; }
    }
    static Dictionary<string,object> Time(string kind,string task,int repetition,Action action) {
        var process=Process.GetCurrentProcess();TimeSpan cpu=process.TotalProcessorTime;var watch=Stopwatch.StartNew();
        action();watch.Stop();
        var row=new Dictionary<string,object>{{"kind",kind},{"task",task},{"repetition",repetition},
            {"wallSeconds",watch.Elapsed.TotalSeconds},{"cpuSeconds",(process.TotalProcessorTime-cpu).TotalSeconds}};
        measurements.Add(row);Console.WriteLine("COST "+kind+" "+task+" "+watch.Elapsed.TotalSeconds.ToString("F6",System.Globalization.CultureInfo.InvariantCulture));
        return row;
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    static int Run(string[] args) {
        if(IntPtr.Size!=4)throw new InvalidOperationException("Probe must run as x86 CLR4");
        if(args.Length==2 && args[1]=="--check-runtime") {
            if(typeof(CacheCostProbe).GetMethod("CacheProcessContract",BindingFlags.NonPublic|BindingFlags.Static)==null)
                throw new InvalidOperationException("Accepted runtime contract missing");
            Console.WriteLine("Same-source replay helper loaded: "+typeof(CacheInputSnapshot).Assembly.Location+" CLR "+Environment.Version+" x86");return 0;
        }
        var json=new JavaScriptSerializer{MaxJsonLength=int.MaxValue};
        var input=json.Deserialize<Dictionary<string,object>>(File.ReadAllText(args[1]));
        BuildRequest request=json.Deserialize<BuildRequest>(json.Serialize(input["request"]));
        request.Options.Clear();
        foreach(var pair in (Dictionary<string,object>)input["options"])
            request.Options.Add(pair.Key,json.Deserialize<EffectiveOption>(json.Serialize(pair.Value)));
        BuildPlan plan=new BuildPlan{Tasks=json.Deserialize<BuildTask[]>(json.Serialize(input["tasks"]))};
        if(plan.Tasks.Length!=5)throw new InvalidOperationException("Expected all five original Upheaval streams");
        string work=Path.GetDirectoryName(Path.GetFullPath(args[2]));
        MethodInfo taskName=typeof(BuildAssetCache).GetMethod("TaskName",BindingFlags.NonPublic|BindingFlags.Instance);
        if(taskName==null)throw new InvalidOperationException("Original cache contract changed");
        string processContract=CacheProcessContract();
        var namespaces=new Dictionary<string,string>();var fingerprints=new Dictionary<string,string>();
        int fileCount=0;
        // Warm the exact production method/JIT and derive exact existing cache identities.
        foreach(BuildTask task in plan.Tasks) {
            var one=new BuildPlan{PlanKind="CoreCacheStream.v2",Tasks=new[]{task}};
            var capture=CacheInputSnapshot.Capture(request,one,runtime,CancellationToken.None);
            fileCount=capture.FileCount;fingerprints.Add(task.Id,capture.Fingerprint);
            namespaces.Add(task.Id,CacheNamespace(capture.Fingerprint,processContract));
        }
        for(int repetition=0;repetition<3;repetition++) {
            foreach(BuildTask task in plan.Tasks) {
                var one=new BuildPlan{PlanKind="CoreCacheStream.v2",Tasks=new[]{task}};
                CacheInputSnapshot capture=null;
                var row=Time("input-capture-pair",task.Id,repetition,()=>{
                    capture=CacheInputSnapshot.Capture(request,one,runtime,CancellationToken.None);
                    if(!capture.MatchesCurrent(request,one,runtime,CancellationToken.None))throw new InvalidOperationException("Input changed during capture pair");
                });
                if(capture.Fingerprint!=fingerprints[task.Id])throw new InvalidOperationException("Probe inputs changed between repetitions");
                row.Add("filesHashed",capture.FileCount*2);
            }
        }
        int restored=0;long payloadBytes=0;int payloadFiles=0;
        for(int repetition=0;repetition<3;repetition++) {
            foreach(BuildTask task in plan.Tasks) {
                string stage=Path.Combine(request.OutputRoot,"cost-probe-staging",repetition.ToString(),task.Id.Replace('/','_'));
                string space=namespaces[task.Id];bool hit=false;
                using(var cache=new BuildAssetCache(request.OutputRoot,space)) {
                    string identity=(string)taskName.Invoke(cache,new object[]{task.Id});
                    bool exists=Directory.Exists(Path.Combine(request.OutputRoot,".cache","build-assets-v2",identity));
                    var row=Time("hot-prepare",task.Id,repetition,()=>{hit=cache.PrepareStream(task.Id,stage,CancellationToken.None);});
                    if(exists!=hit)throw new InvalidOperationException("Existing cache entry did not validate");
                    row.Add("restored",hit);
                    if(hit) {
                        restored++;
                        var files=Directory.GetFiles(stage,"*",SearchOption.AllDirectories);
                        long bytes=files.Sum(p=>new FileInfo(p).Length);row.Add("payloadFiles",files.Length);row.Add("payloadBytes",bytes);
                        if(repetition==0){payloadFiles+=files.Length;payloadBytes+=bytes;}
                    }
                    Time("hot-restage",task.Id,repetition,()=>cache.StageStream(task.Id,stage,CancellationToken.None));
                    Time("hot-commit-existing",task.Id,repetition,()=>cache.Commit());
                    Time("hot-discard-candidate",task.Id,repetition,()=>cache.Complete());
                }
                if(hit) {
                    string cold=Path.Combine(work,"cold",repetition.ToString(),task.Id.Replace('/','_'));
                    string source=Path.Combine(cold,"stage");CopyTree(stage,source);
                    using(var cache=new BuildAssetCache(cold,space)) {
                        Time("cold-stage",task.Id,repetition,()=>cache.StageStream(task.Id,source,CancellationToken.None));
                        Time("cold-commit",task.Id,repetition,()=>cache.Commit());
                        Time("cold-complete",task.Id,repetition,()=>cache.Complete());
                    }
                    using(var verify=new BuildAssetCache(cold,space))
                        if(!verify.PrepareStream(task.Id,Path.Combine(cold,"verify"),CancellationToken.None))throw new InvalidOperationException("New cold cache did not restore");
                    Directory.Delete(cold,true);
                }
                if(Directory.Exists(stage))Directory.Delete(stage,true);
            }
        }
        if(restored==0)throw new InvalidOperationException("No real existing cache namespace hit; cannot report cache cost");
        string stagingRoot=Path.Combine(request.OutputRoot,"cost-probe-staging");if(Directory.Exists(stagingRoot))Directory.Delete(stagingRoot,true);
        var result=new{schemaVersion=1,succeeded=true,architecture=IntPtr.Size*8,clrVersion=Environment.Version.ToString(),
            assemblyPath=typeof(CacheInputSnapshot).Assembly.Location,repetitions=3,streamCount=plan.Tasks.Length,
            inputFilesPerInventory=fileCount,restoredStreamObservations=restored,payloadFiles=payloadFiles,payloadBytes=payloadBytes,
            requestArguments=request.Arguments,measurements=measurements,
            methodology="Unmodified production C# method sources compiled opt+ CLR4/x86 into separate replay helper; actual CLI request/tasks and confirmed cache namespace verified; warm body cost, not sampled full-CLI attribution; input/confirmed-cache immutability verified by controller"};
        File.WriteAllText(args[2],json.Serialize(result));return 0;
    }
    static void CopyTree(string source,string target) {
        Directory.CreateDirectory(target);
        foreach(string path in Directory.GetFiles(source,"*",SearchOption.AllDirectories)) {
            string destination=Path.Combine(target,path.Substring(source.TrimEnd(Path.DirectorySeparatorChar).Length+1));
            Directory.CreateDirectory(Path.GetDirectoryName(destination));File.Copy(path,destination,false);
        }
    }
}
