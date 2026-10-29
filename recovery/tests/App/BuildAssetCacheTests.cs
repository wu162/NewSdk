using System;
using System.IO;
using System.Linq;
using System.Threading;
using NewModCompiler.Application;

namespace NewModCompiler.Tests
{
    public static class BuildAssetCacheTests
    {
        static int checks;
        static readonly string Namespace = new string('a',64);
        static void Check(bool value,string message) { ++checks; if (!value) throw new Exception(message); }
        static void Reject<T>(Action action,string message) where T:Exception { try { action(); } catch(T) { ++checks; return; } throw new Exception(message); }
        static void Put(string file,string text) { Directory.CreateDirectory(Path.GetDirectoryName(file)); File.WriteAllText(file,text); }
        static string Cache(string output) { return Path.Combine(output,".cache","build-assets-v2"); }
        static string Staging(string output,string name) { return Path.Combine(output,".build",name,"cache"); }
        static int Entries(string output) { string root=Cache(output); return Directory.Exists(root)?Directory.GetDirectories(root).Length:0; }
        public static int Run(string root)
        {
            checks=0; Directory.CreateDirectory(root);
            string source=Staging(root,"source"), file=Path.Combine(source,"Win32","ArmorTemplate","ab","one.asset");
            Put(file,"asset A"); Put(Path.ChangeExtension(file,".cdata"),"custom data A");
            using(var store=new BuildAssetCache(root,Namespace)) {
                Check(!store.PrepareStream("base/Mod",Staging(root,"cold"),CancellationToken.None),"An uncommitted cache hit");
                store.StageStream("base/Mod",source,CancellationToken.None);
                Check(Entries(root)==0,"Staging published cache bytes");
            }
            Check(!Directory.GetDirectories(root,".cache-candidate-*").Any(),"Disposed candidate survived");
            using(var store=new BuildAssetCache(root,Namespace)) {
                store.StageStream("base/Mod",source,CancellationToken.None); store.Commit();
                Check(Entries(root)==1,"Commit did not install a stream cache");
                Reject<IOException>(()=>{using(var competing=new BuildAssetCache(root,Namespace)) competing.PrepareStream("base/Mod",Staging(root,"competing"),CancellationToken.None);},"Unconfirmed cache transaction allowed a competing reader");
                store.Rollback(); Check(Entries(root)==0,"Rollback retained a new cache generation");
                store.Rollback(); Check(Entries(root)==0,"Rollback was not idempotent");
            }
            using(var store=new BuildAssetCache(root,Namespace)) {
                store.StageStream("base/Mod",source,CancellationToken.None); store.Commit(); store.Complete();
            }
            Check(Entries(root)==1,"Confirmed cache was discarded by Dispose");
            string hot=Staging(root,"hot");
            using(var store=new BuildAssetCache(root,Namespace)) {
                Check(store.PrepareStream("base/Mod",hot,CancellationToken.None),"Identical namespace/task did not restore assets");
                string copied=Path.Combine(hot,"Win32","ArmorTemplate","ab","one.asset");
                Check(File.ReadAllBytes(file).SequenceEqual(File.ReadAllBytes(copied)),"Restored asset bytes differ");
                Check(File.ReadAllText(Path.ChangeExtension(copied,".cdata"))=="custom data A","Custom data was not restored");
                Check(!store.PrepareStream("apt/Mod",Staging(root,"other-task"),CancellationToken.None),"Task cache namespaces leaked");
                Reject<IOException>(()=>store.PrepareStream("base/Mod",hot,CancellationToken.None),"Nonempty staging was overwritten");
                store.StageStream("base/Mod",source,CancellationToken.None); store.Commit(); store.Rollback();
                Check(Entries(root)==1,"Rollback deleted a previously committed immutable entry");
            }
            using(var other=new BuildAssetCache(root,new string('b',64)))
                Check(!other.PrepareStream("base/Mod",Staging(root,"other-namespace"),CancellationToken.None),"Implementation/input namespaces leaked");
            string secondRoot=Path.Combine(root,"second-output");
            using(var other=new BuildAssetCache(secondRoot,Namespace))
                Check(!other.PrepareStream("base/Mod",Staging(secondRoot,"cold"),CancellationToken.None),"Output roots leaked");
            using(var store=new BuildAssetCache(root,Namespace)) {
                string missing=Staging(root,"empty"); Directory.CreateDirectory(missing);
                store.StageStream("empty",missing,CancellationToken.None); store.Commit(); store.Complete();
            }
            Check(Entries(root)==1,"Empty asset tree was cached as a hit");
            using(var store=new BuildAssetCache(root,Namespace)) using(var cancellation=new CancellationTokenSource()) {
                cancellation.Cancel();
                Reject<OperationCanceledException>(()=>store.PrepareStream("base/Mod",Staging(root,"cancelled-read"),cancellation.Token),"Cancelled restore succeeded");
                Reject<OperationCanceledException>(()=>store.StageStream("cancelled",source,cancellation.Token),"Cancelled staging succeeded");
            }
            Check(Entries(root)==1,"Cancelled transaction changed durable entries");
            using(var store=new BuildAssetCache(root,Namespace)) {
                Reject<IOException>(()=>store.StageStream("escape",Path.GetDirectoryName(root),CancellationToken.None),"Source outside output was accepted");
                Reject<IOException>(()=>store.StageStream("output-root",root,CancellationToken.None),"Output root was accepted as staging");
                Reject<IOException>(()=>store.StageStream("inside-cache",Cache(root),CancellationToken.None),"Durable cache was accepted as staging");
            }
            Reject<ArgumentException>(()=>new BuildAssetCache(root,"../bad"),"Namespace traversal was accepted");
            // Reject byte corruption and unexpected files without exposing a partial restore.
            string durable=Directory.GetDirectories(Cache(root)).Single(), asset=Directory.GetFiles(Path.Combine(durable,"files"),"*",SearchOption.AllDirectories).Single(path=>File.ReadAllText(path)=="asset A");
            string pending=Path.Combine(durable,"pending"); Put(pending,"abandoned transaction");
            using(var store=new BuildAssetCache(root,Namespace))
                Check(!store.PrepareStream("base/Mod",Staging(root,"abandoned"),CancellationToken.None),"Unconfirmed durable entry was reused after owner disappeared");
            File.Delete(pending);
            DateTime timestamp=File.GetLastWriteTimeUtc(asset); Put(asset,"asset B"); File.SetLastWriteTimeUtc(asset,timestamp);
            using(var store=new BuildAssetCache(root,Namespace))
                Check(!store.PrepareStream("base/Mod",Staging(root,"corrupt"),CancellationToken.None),"Same-length/same-mtime cache corruption was reused");
            Check(!Directory.Exists(Staging(root,"corrupt")),"Corrupt tree was partly restored");
            Put(asset,"asset A"); string extra=Path.Combine(durable,"files","unexpected.asset"); Put(extra,"extra");
            using(var store=new BuildAssetCache(root,Namespace))
                Check(!store.PrepareStream("base/Mod",Staging(root,"unexpected"),CancellationToken.None),"Unindexed cache data was reused");
            File.Delete(extra);
            using(var store=new BuildAssetCache(root,Namespace))
                Check(store.PrepareStream("base/Mod",Staging(root,"retry"),CancellationToken.None),"Integrity failure poisoned later valid restore");
            string index=Path.Combine(durable,"index.bin"); byte[] valid=File.ReadAllBytes(index); File.WriteAllBytes(index,new byte[] {255});
            using(var store=new BuildAssetCache(root,Namespace))
                Check(!store.PrepareStream("base/Mod",Staging(root,"bad-index"),CancellationToken.None),"Malformed cache index was reused");
            File.WriteAllBytes(index,valid);
            using(var store=new BuildAssetCache(root,Namespace)) {
                store.StageStream("another",source,CancellationToken.None); store.Commit();
                Check(Entries(root)==2,"Second stream did not install independently");
            }
            Check(Entries(root)==1,"Unconfirmed second stream survived Dispose");
            // A Windows sharing violation must leave the transaction retryable
            // and the installed entry unconfirmed until exact cleanup succeeds.
            using(var store=new BuildAssetCache(root,Namespace)) {
                store.StageStream("locked-entry",source,CancellationToken.None); store.Commit();
                string lockedEntry=Directory.GetDirectories(Cache(root)).Single(path=>File.Exists(Path.Combine(path,"pending")));
                string lockedFile=Directory.GetFiles(Path.Combine(lockedEntry,"files"),"*",SearchOption.AllDirectories).Single(path=>File.ReadAllText(path)=="asset A");
                using(var held=new FileStream(lockedFile,FileMode.Open,FileAccess.Read,FileShare.Read)) {
                    bool failed=false;
                    try { store.Rollback(); }
                    catch(IOException error) { failed=true; Check(((string[])error.Data["RetainedCacheEntries"]).Contains(lockedEntry),"Rollback failure lost its retained entry path"); }
                    Check(failed,"Locked cache rollback unexpectedly succeeded");
                    Check(File.Exists(Path.Combine(lockedEntry,"pending")),"Failed rollback made its entry reusable");
                }
                store.Rollback(); Check(!Directory.Exists(lockedEntry),"Rollback could not retry after lock release");
            }
            Check(!Directory.GetDirectories(root,".cache-candidate-*").Any(),"Cache transaction candidates leaked");
            return checks;
        }
    }
}

