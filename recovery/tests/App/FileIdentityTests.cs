using System;
using System.IO;
using System.Linq;
using System.Threading;
using NewModCompiler.Application;

namespace NewModCompiler.Tests
{
    internal static class FileIdentityTests
    {
        static int count;
        static void Check(bool value,string message) {count++;if(!value)throw new Exception("File identity: "+message);}
        public static int Run(string root)
        {
            count=0; Directory.CreateDirectory(Path.Combine(root,"child"));
            string path=Path.Combine(root,"schema.xsd"), nested=Path.Combine(root,"child","nested.xsd");
            File.WriteAllText(path,"before"); File.WriteAllText(nested,"nested");
            var before=FileIdentity.ReadDirectory(root,true);
            Check(before.Count==2 && before.Any(file=>file.Path==nested),"Recursive metadata omitted an input");
            var first=before.Single(file=>file.Path==path);
            Check(first.Size==6 && first.FileId!=0 && first.ChangeTime!=0,"Directory record layout differs from Win32 file identity");
            var unchanged=FileIdentity.ReadDirectory(root,false).Single();
            Check(unchanged.FileId==first.FileId && unchanged.ChangeTime==first.ChangeTime && unchanged.LastWriteTime==first.LastWriteTime,"Read-only enumeration changed identity");
            DateTime stamp=File.GetLastWriteTimeUtc(path); Thread.Sleep(30);
            File.WriteAllText(path,"after!"); File.SetLastWriteTimeUtc(path,stamp);
            var changed=FileIdentity.ReadDirectory(root,false).Single();
            Check(changed.Size==first.Size && changed.LastWriteTime==first.LastWriteTime && changed.ChangeTime!=first.ChangeTime,"Same-length/restored-mtime write was not invalidated");
            using(var held=new FileStream(path,FileMode.Open,FileAccess.ReadWrite,FileShare.None))
                Check(FileIdentity.ReadDirectory(root,false).Single().Size==6,"Identity attempted to open a locked file body");
            string replacement=Path.Combine(root,"replacement"); File.WriteAllText(replacement,"after!");
            File.SetLastWriteTimeUtc(replacement,stamp); File.Delete(path); File.Move(replacement,path);
            var replaced=FileIdentity.ReadDirectory(root,false).Single();
            Check(replaced.FileId!=changed.FileId,"Replacing an input retained its old identity");
            File.Delete(nested);
            Check(FileIdentity.ReadDirectory(root,true).Count==1,"Removed input remained in metadata closure");
            var hashes=new FileHashCache(Path.Combine(root,"receipts"));
            string published=Path.Combine(root,"published"), item=Path.Combine(published,"child","item.bin");
            Directory.CreateDirectory(Path.GetDirectoryName(item));File.WriteAllText(item,"before");
            var artifacts=new[] {new Artifact {Path=item,Kind="test",Size=6,Sha256=hashes.Hash(item)}};
            hashes.RecordPublished(artifacts,published,false);
            Check(hashes.MatchesPublication(artifacts,published),"Unchanged publication receipt did not match");
            File.WriteAllText(Path.Combine(published,"child","unexpected.bin"),"extra");
            Check(!hashes.MatchesPublication(artifacts,published),"Extra nested output did not invalidate publication receipt");
            File.Delete(Path.Combine(published,"child","unexpected.bin"));hashes.RecordPublished(artifacts,published,false);
            DateTime saved=File.GetLastWriteTimeUtc(item);Thread.Sleep(30);File.WriteAllText(item,"after!");File.SetLastWriteTimeUtc(item,saved);
            Check(!hashes.MatchesPublication(artifacts,published),"Restored-mtime edit did not invalidate publication receipt");
            return count;
        }
    }
}
