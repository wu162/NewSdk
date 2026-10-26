using System;
using System.IO;
using System.Linq;
using System.Threading;
using NewModCompiler.Application;

namespace NewModCompiler.Tests
{
    // These are real persisted package tests. Core reuse/output equality belongs
    // to the separate production CLI matrix, not to this storage fixture.
    internal static class SessionPackageTests
    {
        static int checks;
        static readonly string Fingerprint=new string('c',64);
        const string Xml="<sc d=\"2026-10-27\"><fic/></sc>";
        static void Check(bool value,string message) { ++checks; if(!value) throw new Exception("Session package: "+message); }
        static void Reject<T>(Action action,string message) where T:Exception { try { action(); } catch(T) { ++checks; return; } throw new Exception("Session package: "+message); }
        static void Put(string path,string text) { Directory.CreateDirectory(Path.GetDirectoryName(path)); File.WriteAllText(path,text); }
        static string Stage(string output,string name) { return Path.Combine(output,".build",name); }
        static string Root(string output) { return Path.Combine(output,".cache","session-packages-v1"); }
        static string Entry(string output) { return Directory.GetDirectories(Root(output)).Single(); }
        static string Payload(string output,string content) { return Directory.GetFiles(Path.Combine(Entry(output),"files")).Single(path=>File.ReadAllText(path)==content); }
        static void Fixture(string root)
        {
            Put(Path.Combine(root,"hashes","session.xml"),Xml);
            Put(Path.Combine(root,"intermediate","Win32","ArmorTemplate","one.asset"),"asset A");
            Put(Path.Combine(root,"streams","base","Win32","one.cdata"),"custom data A");
            Put(Path.Combine(root,"streams","base","Win32","Mod.manifest"),"linked output must be rebuilt");
            Put(Path.Combine(root,"streams","base","Win32","Mod.bin"),"linked binary must be rebuilt");
            Put(Path.Combine(root,"hashes","session.xml.old"),"obsolete session");
            Put(Path.Combine(root,"intermediate","discard.asset.old"),"old asset");
            Put(Path.Combine(root,"intermediate","discard.tmp","nested.asset"),"temporary subtree");
            Put(Path.Combine(root,"streams","one.cdata.tmp"),"temporary custom data");
        }
        static void Install(string output,string source)
        {
            using(var store=new SessionPackage(output,Fingerprint)) { store.StageStream("base/Mod",source,CancellationToken.None); store.Commit(); store.Complete(); }
        }
        static bool Restore(string output,string name)
        {
            using(var store=new SessionPackage(output,Fingerprint)) return store.PrepareStream("base/Mod",Stage(output,name),CancellationToken.None);
        }
        public static int Run(string output)
        {
            checks=0; Directory.CreateDirectory(output); string source=Stage(output,"source"); Fixture(source);
            using(var store=new SessionPackage(output,Fingerprint)) {
                Check(!store.PrepareStream("base/Mod",Stage(output,"cold"),CancellationToken.None),"Uninstalled package was a hit");
                store.StageStream("base/Mod",source,CancellationToken.None);
                Check(!Directory.Exists(Root(output)),"Candidate package was published before commit");
                store.Commit();
                Check(File.Exists(Path.Combine(Entry(output),"pending")),"Unconfirmed package lacks durable exclusion");
                Reject<IOException>(()=>{using(var competing=new SessionPackage(output,Fingerprint)) { }},"Namespace lock allowed competing access");
                store.Rollback(); Check(!Directory.GetDirectories(Root(output)).Any(),"Rollback retained newly installed package");
                store.Rollback();
            }
            Install(output,source);
            string hot=Stage(output,"hot"); Check(Restore(output,"hot"),"Confirmed package could not restore");
            string[] actual=Directory.GetFiles(hot,"*",SearchOption.AllDirectories).Select(path=>path.Substring(hot.Length+1).Replace('\\','/')).OrderBy(x=>x,StringComparer.Ordinal).ToArray();
            Check(actual.SequenceEqual(new[] {"hashes/session.xml","intermediate/Win32/ArmorTemplate/one.asset","streams/base/Win32/one.cdata"}),"Package restored old/tmp or linked-output files, or lost required assets");
            Check(File.ReadAllText(Path.Combine(hot,"hashes","session.xml"))==Xml,"Session XML bytes changed");
            Check(File.ReadAllText(Path.Combine(hot,"intermediate","Win32","ArmorTemplate","one.asset"))=="asset A","Intermediate asset bytes changed");
            Check(File.ReadAllText(Path.Combine(hot,"streams","base","Win32","one.cdata"))=="custom data A","Custom data bytes changed");
            using(var store=new SessionPackage(output,Fingerprint)) {
                Check(!store.PrepareStream("apt/Mod",Stage(output,"other-task"),CancellationToken.None),"Stream identities leaked");
                Reject<IOException>(()=>store.PrepareStream("base/Mod",hot,CancellationToken.None),"Restore overwrote an active stream");
                store.StageStream("base/Mod",source,CancellationToken.None); store.Commit(); store.Rollback();
            }
            Check(Restore(output,"after-preserved"),"Rollback removed previously confirmed package");
            using(var other=new SessionPackage(output,new string('d',64)))
                Check(!other.PrepareStream("base/Mod",Stage(output,"other-namespace"),CancellationToken.None),"Input/implementation identities leaked");
            using(var assets=new BuildAssetCache(output,Fingerprint))
                Check(!assets.PrepareStream("base/Mod",Path.Combine(Stage(output,"asset-only"),"cache"),CancellationToken.None),"Session package leaked into build-asset cache");
            string durable=Entry(output), payload=Payload(output,"asset A");
            DateTime timestamp=File.GetLastWriteTimeUtc(payload); File.WriteAllText(payload,"asset B"); File.SetLastWriteTimeUtc(payload,timestamp);
            Check(!Restore(output,"corrupt"),"Same-length/same-time corruption was accepted");
            Check(!Directory.Exists(Stage(output,"corrupt")),"Corrupt package partly populated staging");
            File.WriteAllText(payload,"asset A");
            string saved=File.ReadAllText(payload); File.Delete(payload);
            Check(!Restore(output,"missing-asset"),"Missing intermediate asset did not invalidate entire package");
            Check(!Directory.Exists(Stage(output,"missing-asset")),"Missing-asset package partly restored session");
            File.WriteAllText(payload,saved);
            string extra=Path.Combine(durable,"files","unexpected"); Put(extra,"unindexed");
            Check(!Restore(output,"extra-file"),"Unindexed payload was accepted"); File.Delete(extra);
            string cdata=Payload(output,"custom data A"); saved=File.ReadAllText(cdata); File.Delete(cdata);
            Check(!Restore(output,"missing-cdata"),"Missing custom data did not invalidate entire package"); File.WriteAllText(cdata,saved);
            string pending=Path.Combine(durable,"pending"); Put(pending,"owner disappeared");
            Check(!Restore(output,"pending"),"Abandoned unconfirmed generation was reused"); File.Delete(pending);
            string index=Path.Combine(durable,"index.bin"); byte[] original=File.ReadAllBytes(index); File.WriteAllBytes(index,new byte[] {255});
            Check(!Restore(output,"bad-index"),"Malformed index was reused"); File.WriteAllBytes(index,original);
            Check(Restore(output,"retry"),"Rejected package poisoned a subsequent valid restore");
            using(var store=new SessionPackage(output,Fingerprint)) using(var cancel=new CancellationTokenSource()) {
                cancel.Cancel();
                Reject<OperationCanceledException>(()=>store.PrepareStream("base/Mod",Stage(output,"cancelled-read"),cancel.Token),"Cancelled restore succeeded");
                Reject<OperationCanceledException>(()=>store.StageStream("cancelled",source,cancel.Token),"Cancelled stage succeeded");
                Reject<IOException>(()=>store.PrepareStream("base/Mod",output,CancellationToken.None),"Output root was accepted as stream staging");
                Reject<IOException>(()=>store.StageStream("escape",Path.GetDirectoryName(output),CancellationToken.None),"External source was accepted");
                Reject<IOException>(()=>store.StageStream("durable",Root(output),CancellationToken.None),"Durable cache was accepted as source staging");
            }
            foreach(string invalid in new[] {"<sc>","<AssetDeclaration/>","<sc/>","<!DOCTYPE sc [<!ENTITY e SYSTEM 'file:///never'>]><sc d=\"x\">&e;</sc>"}) {
                string bad=Stage(output,"bad-xml-"+Guid.NewGuid().ToString("N")); Put(Path.Combine(bad,"hashes","session.xml"),invalid);
                using(var store=new SessionPackage(output,Fingerprint))
                    Reject<InvalidDataException>(()=>store.StageStream("invalid",bad,CancellationToken.None),"Invalid/external-entity session XML was staged");
            }
            // A validated generic index must still pass session-specific XML and
            // layout validation before any live Core staging is populated.
            string invalidOutput=Path.Combine(output,"invalid-package"), invalidSource=Stage(invalidOutput,"source");
            Put(Path.Combine(invalidSource,"hashes","session.xml"),"<sc d=\"x\">");
            using(var raw=new BuildAssetCache(invalidOutput,Fingerprint,true)) { raw.StageStream("base/Mod",invalidSource,CancellationToken.None); raw.Commit(); raw.Complete(); }
            Check(!Restore(invalidOutput,"bad-xml"),"Well-indexed malformed session XML was reused");
            Check(!Directory.Exists(Stage(invalidOutput,"bad-xml")),"Malformed session XML was partly restored");
            string layoutOutput=Path.Combine(output,"invalid-layout"), layoutSource=Stage(layoutOutput,"source");
            Put(Path.Combine(layoutSource,"hashes","session.xml"),Xml); Put(Path.Combine(layoutSource,"streams","Mod.manifest"),"linked");
            using(var raw=new BuildAssetCache(layoutOutput,Fingerprint,true)) { raw.StageStream("base/Mod",layoutSource,CancellationToken.None); raw.Commit(); raw.Complete(); }
            Check(!Restore(layoutOutput,"bad-layout"),"Well-indexed linked output was accepted in a session package");
            Check(!Directory.Exists(Stage(layoutOutput,"bad-layout")),"Invalid layout partly populated live staging");
            Check(!Directory.GetDirectories(output,".cache-candidate-*").Any(),"Package transaction candidates leaked");
            Check(!Directory.GetDirectories(source,".session-*",SearchOption.TopDirectoryOnly).Any(),"Temporary package staging leaked");
            return checks;
        }
    }
}
