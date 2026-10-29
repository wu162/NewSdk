using System;
using System.IO;
using System.Threading;
using NewModCompiler.Application;

namespace NewModCompiler.Tests
{
    internal static class LegacyCacheStoreTests
    {
        static int count;
        static void Check(bool value, string message) { count++; if (!value) throw new Exception("Legacy cache: " + message); }
        static void Put(string file, string value) { Directory.CreateDirectory(Path.GetDirectoryName(file)); if(File.Exists(file))File.Delete(file); File.WriteAllText(file, value); }
        public static int Run(string root)
        {
            count = 0; string output = Path.Combine(root, "output"), shared = Path.Combine(root, "network", "sessions");
            string hashes = Path.Combine(root, "work", "hashes"), streams = Path.Combine(root, "work", "streams");
            string xml = Path.Combine(hashes, LegacyCacheStore.SessionName), raw = Path.Combine(shared, LegacyCacheStore.SessionName);
            string data = Path.Combine(streams, "mod", "one.cdata");
            Put(xml, "<sc d=\"first\"/>"); Put(data, "cdata first");
            using (var store = new LegacyCacheStore(output, shared, "identity"))
            {
                Check(!store.Prepare(Path.Combine(root,"cold"),Path.Combine(root,"cold-streams"),CancellationToken.None),"Missing original session was a hit");
                store.Stage(hashes, streams, false, CancellationToken.None); store.Commit();
                Check(File.ReadAllText(raw).Contains("first"),"Original session filename or custom directory not honored");
                store.Rollback(); Check(!File.Exists(raw),"Unconfirmed raw session survived rollback");
                Put(xml, "<sc d=\"first\"/>");
                store.Stage(hashes, streams, false, CancellationToken.None); store.ConfirmPartial();
                Check(!File.Exists(xml),"Serialized session was copied instead of moved into its transaction");
                Check(File.Exists(raw),"Partial-session confirmation did not persist useful work");
                string restore = Path.Combine(root,"hot"), restoredStreams=Path.Combine(root,"hot-streams");
                Check(store.Prepare(restore,restoredStreams,CancellationToken.None),"Original-format session did not load");
                Check(File.ReadAllText(Path.Combine(restoredStreams,"mod","one.cdata"))=="cdata first","Session lost its CDATA closure");
                string confirmedData=Path.Combine(shared,".NewModCompiler","cdata","mod","one.cdata");
                DateTime stamp=new DateTime(2001,2,3,4,5,6,DateTimeKind.Utc); File.SetLastWriteTimeUtc(confirmedData,stamp);
                Put(xml,"<sc d=\"first\"/>"); store.Stage(hashes,streams,false,CancellationToken.None); store.Commit(); store.Complete();
                Check(File.GetLastWriteTimeUtc(confirmedData)==stamp,"Identical CDATA was installed again");
            }
            using (var store = new LegacyCacheStore(output, shared, "identity"))
            {
                Put(xml,"<sc d=\"second\"/>"); Put(data,"cdata second");
                store.Stage(hashes, streams, false, CancellationToken.None); store.Commit(); store.Rollback();
                Check(File.ReadAllText(raw).Contains("first"),"Failed replacement did not restore the confirmed original session");
                Check(File.ReadAllText(Path.Combine(shared,".NewModCompiler","cdata","mod","one.cdata"))=="cdata first","Failed replacement changed confirmed CDATA");
            }
            using (var incompatible = new LegacyCacheStore(output, shared, "different-implementation"))
                Check(!incompatible.Prepare(Path.Combine(root,"incompatible"),Path.Combine(root,"incompatible-streams"),CancellationToken.None),"Changed schema or implementation reused a previous session");
            // Format switches use the same original cache directory and replace
            // its active representation, avoiding stale .deflate precedence.
            Put(xml+".deflate","compressed bytes");
            using (var store = new LegacyCacheStore(output, shared, "identity"))
            {
                store.Stage(hashes,streams,true,CancellationToken.None); store.Commit(); store.Complete();
            }
            Check(!File.Exists(raw) && File.ReadAllText(raw+".deflate")=="compressed bytes","Compression selection left a stale active XML representation");
            using (var store = new LegacyCacheStore(output, shared, "identity"))
            {
                string restored=Path.Combine(root,"compressed");
                Check(store.Prepare(restored,Path.Combine(root,"compressed-streams"),CancellationToken.None),"Compressed original representation was not imported");
                Check(File.ReadAllText(Path.Combine(restored,LegacyCacheStore.SessionName)+".deflate")=="compressed bytes","Compressed session bytes changed");
            }
            // Existing BAB directories have no new sidecar. They remain readable.
            string original=Path.Combine(root,"original-sdk"); Put(Path.Combine(original,LegacyCacheStore.SessionName),"<sc d=\"original\"/>");
            Put(Path.Combine(original,"mod","payload.cdata"),"original cdata");
            using (var store=new LegacyCacheStore(output,original,"identity"))
            {
                Check(store.Prepare(Path.Combine(root,"import"),Path.Combine(root,"import-streams"),CancellationToken.None),"Existing old SDK cache directory was rejected");
                Check(File.ReadAllText(Path.Combine(root,"import-streams","mod","payload.cdata"))=="original cdata","Existing old SDK CDATA was not imported");
            }
            return count;
        }
    }
}
