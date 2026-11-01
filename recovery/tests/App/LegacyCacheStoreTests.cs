using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NewModCompiler.Application;

namespace NewModCompiler.Tests
{
    internal static class LegacyCacheStoreTests
    {
        static int count;
        static void Check(bool value, string message) { count++; if (!value) throw new Exception("Legacy cache: " + message); }
        static void Put(string file, string value) { Directory.CreateDirectory(Path.GetDirectoryName(file)); File.WriteAllText(file, value); }
        public static int Run(string root)
        {
            count=0;
            string shared=Path.Combine(root,"sessions"), streams=Path.Combine(root,"streams");
            string session=Path.Combine(shared,LegacyCacheStore.SessionName), data=Path.Combine(streams,"mod","cdata","one.cdata");
            using(var store=new LegacyCacheStore(shared)) {
                Check(!store.CanLoad(),"Missing BAB session was a hit");
                Put(session,"<sc d=\"first\"/>"); Put(data,"payload");
                Check(store.CanLoad(),"Original BAB session without a sidecar was rejected");
                store.SaveData(streams,new[]{data},CancellationToken.None); store.Saved(false);
                Check(!File.Exists(Path.Combine(shared,".NewModCompiler","session.identity")),"Session identity sidecar was created");
                string restored=Path.Combine(root,"restored","one.cdata");
                Check(store.RestoreData(Path.Combine("mod","cdata","one.cdata"),restored,CancellationToken.None)==restored && File.ReadAllText(restored)=="payload","CDATA was not restored on demand");
                File.WriteAllText(restored,"modified");
                Check(File.ReadAllText(data)=="payload","Restoring CDATA shared mutable output storage");
            }
            using(var incompatible=new LegacyCacheStore(shared)) Check(incompatible.CanLoad(),"Existing BAB session was rejected");
            Put(session+".deflate","compressed");
            using(var store=new LegacyCacheStore(shared)) store.Saved(true);
            Check(!File.Exists(session) && File.ReadAllText(session+".deflate")=="compressed","Compression switch kept stale active XML");
            string original=Path.Combine(root,"original"); Put(Path.Combine(original,LegacyCacheStore.SessionName),"<sc d=\"old\"/>"); Put(Path.Combine(original,"mod","cdata","old.cdata"),"old payload");
            using(var store=new LegacyCacheStore(original)) {
                Check(store.CanLoad(),"Existing BAB cache was rejected");
                string restored=Path.Combine(root,"old-import.cdata");
                Check(store.RestoreData(Path.Combine("mod","cdata","old.cdata"),restored,CancellationToken.None)==restored && File.ReadAllText(restored)=="old payload","Original BAB CDATA was not imported");
            }
            string partition = Path.Combine(root, "content-partition");
            string oldSession = Path.Combine(partition, "session", LegacyCacheStore.SessionName);
            Put(oldSession, "unpartitioned legacy bytes");
            var baseGroup = new ContentCacheLayout("base", Path.Combine(partition,"session","base"), Path.Combine(partition,"build-cache","base"), Path.Combine(partition,"intermediate","base"));
            var aptGroup = new ContentCacheLayout("apt", Path.Combine(partition,"session","apt"), Path.Combine(partition,"build-cache","apt"), Path.Combine(partition,"intermediate","apt"));
            // Both groups hold their locks together and write identical BAB file
            // names. Neither may load or overwrite the old unpartitioned cache.
            using(var baseStore=new LegacyCacheStore(baseGroup))
            using(var aptStore=new LegacyCacheStore(aptGroup)) {
                Check(!baseStore.CanLoad() && !aptStore.CanLoad(), "New partitions loaded an unpartitioned legacy session");
                Put(baseStore.SessionPath,"base session"); Put(aptStore.SessionPath,"apt session");
                string baseStreams=Path.Combine(root,"base-streams"), aptStreams=Path.Combine(root,"apt-streams");
                string baseData=Path.Combine(baseStreams,"entry","cdata","same.cdata"), aptData=Path.Combine(aptStreams,"entry","cdata","same.cdata");
                Put(baseData,"base payload"); Put(aptData,"apt payload");
                baseStore.SaveData(baseStreams,new[]{baseData},CancellationToken.None);
                aptStore.SaveData(aptStreams,new[]{aptData},CancellationToken.None);
                string relative=Path.Combine("entry","cdata","same.cdata");
                string baseRestored=Path.Combine(root,"base-restored.cdata"), aptRestored=Path.Combine(root,"apt-restored.cdata");
                baseStore.RestoreData(relative,baseRestored,CancellationToken.None); aptStore.RestoreData(relative,aptRestored,CancellationToken.None);
                Check(File.ReadAllText(baseRestored)=="base payload" && File.ReadAllText(aptRestored)=="apt payload", "Identical CDATA names crossed content cache partitions");
                Check(File.ReadAllText(baseStore.SessionPath)=="base session" && File.ReadAllText(aptStore.SessionPath)=="apt session", "Content sessions collided");
            }
            Check(File.ReadAllText(oldSession)=="unpartitioned legacy bytes" && !Directory.Exists(Path.Combine(partition,"session","maps")), "An unused or unpartitioned cache was modified");
            var reports = new[] {
                new KeyValuePair<string,object>("base",new { status="SessionCache",hits=2,xmlParse=3,sessionCache=true,sessionCacheDirectory=baseGroup.Session,
                    parallel=new { backendCalls=2,peakCpuTasks=2,clockFrequency=100L,backendCallsByType=new Dictionary<string,int>{{"Armor",2}} } }),
                new KeyValuePair<string,object>("apt",new { status="SessionCache",hits=5,xmlParse=7,sessionCache=true,sessionCacheDirectory=aptGroup.Session,
                    parallel=new { backendCalls=5,peakCpuTasks=1,clockFrequency=100L,backendCallsByType=new Dictionary<string,int>{{"Armor",1},{"Apt",4}} } })
            };
            var merged=(IDictionary<string,object>)ContentCacheReport.Merge(reports);
            Check((string)merged["status"]=="SessionCache" && (bool)merged["sessionCache"] && Convert.ToInt64(merged["hits"])==7 && Convert.ToInt64(merged["xmlParse"])==10, "Build counters did not aggregate content reports");
            Check((string)merged["sessionCacheDirectory"]==Path.Combine(partition,"session") && ((IDictionary<string,object>)merged["groups"]).Count==2, "Aggregate cache paths lost content attribution");
            var parallelReport=(IDictionary<string,object>)merged["parallel"];
            Check(Convert.ToInt64(parallelReport["backendCalls"])==7 && Convert.ToInt64(parallelReport["peakCpuTasks"])==2 && Convert.ToInt64(parallelReport["clockFrequency"])==100, "Aggregate timings inflated a peak or clock constant");
            Check(Convert.ToInt64(((IDictionary<string,object>)parallelReport["backendCallsByType"])["Armor"])==3, "Per-type backend counts were lost");
            return count;
        }
    }
}
