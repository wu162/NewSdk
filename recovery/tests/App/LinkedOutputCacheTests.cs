using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NewModCompiler.Application;

namespace NewModCompiler.Tests
{
    internal static class LinkedOutputCacheTests
    {
        static int count;
        static void Check(bool value,string message) { count++; if(!value)throw new Exception("Linked output: "+message); }
        static Artifact[] Group(string streams,string name,string payload)
        {
            Directory.CreateDirectory(streams); var result=new List<Artifact>();
            foreach(string extension in new[] {".manifest",".bin",".imp",".relo"}) {
                string path=Path.Combine(streams,name+extension); File.WriteAllText(path,payload+extension);
                result.Add(new Artifact {Path=path,Kind="stream"+extension});
            }
            return result.ToArray();
        }
        public static int Run(string root)
        {
            count=0; string output=Path.Combine(root,"out"), first=Path.Combine(root,"first"), next=Path.Combine(root,"next");
            string scope=Path.Combine("base","Mod");
            Artifact[] files=Group(first,"Mod","first"); Group(first,"RemovedInclude","stale");
            using(var cache=new LinkedOutputCache(output)) {
                cache.Restore(scope,first,Path.Combine(first,"Missing"));
                Check(cache.RestoredStreams==0,"Cold output was a hit");
                var all=new List<Artifact>(files); all.AddRange(Group(first,"RemovedInclude","stale"));
                cache.Stage(scope,first,all,CancellationToken.None); cache.Commit(); cache.Complete();
            }
            string cached=Path.Combine(output,".linked-streams",scope,"Mod.bin");
            using(var cache=new LinkedOutputCache(output)) {
                cache.Restore(scope,next,Path.Combine(next,"Mod"));
                Check(cache.RestoredStreams==1 && File.ReadAllText(Path.Combine(next,"Mod.bin"))=="first.bin","Hot four-file group was not restored");
                Check(!File.Exists(Path.Combine(next,"RemovedInclude.manifest")),"Unrequested old stream leaked into output");
                // Replacing a linked file must leave the previous generation intact.
                string binary=Path.Combine(next,"Mod.bin"); File.Delete(binary); File.WriteAllText(binary,"changed.bin");
                cache.Restore(scope,next,Path.Combine(next,"Mod"));
                Check(File.ReadAllText(binary)=="changed.bin","Repeated output request overwrote current work");
                Check(File.ReadAllText(cached)=="first.bin","Relinking changed the previous snapshot");
                var staged=new List<Artifact>();
                foreach(string extension in new[] {".manifest",".bin",".imp",".relo"}) staged.Add(new Artifact {Path=Path.Combine(next,"Mod"+extension),Kind="stream"+extension});
                cache.Stage(scope,next,staged,CancellationToken.None); cache.Commit();
                Check(File.ReadAllText(cached)=="changed.bin","New linked generation was not committed");
                // Dispose without Complete models a late report/sink failure.
            }
            Check(File.ReadAllText(cached)=="first.bin","Rollback lost previous linked output");
            File.Delete(Path.ChangeExtension(cached,".imp"));
            string repair=Path.Combine(root,"repair");
            using(var cache=new LinkedOutputCache(output)) {
                cache.Restore(scope,repair,Path.Combine(repair,"Mod"));
                Check(cache.RestoredStreams==0 && !Directory.Exists(repair),"Incomplete group was partially restored");
            }
            Check(Directory.GetDirectories(output,".linked-candidate-*").Length==0 && Directory.GetDirectories(output,".linked-previous-*").Length==0,"Linked transaction leaked temporary generations");
            return count;
        }
    }
}
