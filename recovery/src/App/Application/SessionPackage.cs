using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;

namespace NewModCompiler.Application
{
    // A session is reusable only with every intermediate asset and custom-data
    // file that accompanied its XML. Linked output is always rebuilt by Core.
    // This wrapper deliberately knows no Core types, and remains usable by the
    // standalone App transaction tests.
    public sealed class SessionPackage : IDisposable
    {
        readonly BuildAssetCache storage;
        readonly string outputRoot;
        bool disposed;

        public SessionPackage(string outputRoot, string namespaceFingerprint)
        {
            this.outputRoot = Path.GetFullPath(outputRoot);
            storage = new BuildAssetCache(outputRoot, namespaceFingerprint, true);
        }

        public bool PrepareStream(string taskKey, string stagingRoot, CancellationToken cancellation)
        {
            Active(); string staging = Staging(stagingRoot);
            string temporary = Path.Combine(outputRoot, ".session-restore-" + Guid.NewGuid().ToString("N"));
            try
            {
                if (!storage.PrepareStream(taskKey, temporary, cancellation)) return false;
                if (!ValidPackage(temporary, cancellation)) { storage.InvalidateStream(taskKey); return false; }
                cancellation.ThrowIfCancellationRequested();
                string session = Path.Combine(staging, "hashes", "session.xml");
                if (File.Exists(session) || !Empty(Path.Combine(staging,"intermediate")) || !Empty(Path.Combine(staging,"streams")))
                    throw new IOException("The session restore destination must be fresh.");
                // Every byte and the entire XML are checked before touching Core
                // staging. Moves keep the restored trees private to this stream.
                Directory.CreateDirectory(Path.GetDirectoryName(session));
                File.Move(Path.Combine(temporary,"hashes","session.xml"),session);
                foreach (string name in new[] {"intermediate","streams"})
                {
                    string source=Path.Combine(temporary,name), target=Path.Combine(staging,name);
                    if (!Directory.Exists(source)) continue;
                    if (Directory.Exists(target)) Directory.Delete(target);
                    Directory.Move(source,target);
                }
                return true;
            }
            finally { Delete(temporary); }
        }

        public void StageStream(string taskKey, string stagingRoot, CancellationToken cancellation)
        {
            Active(); string staging=Staging(stagingRoot);
            string temporary=Path.Combine(staging,".session-stage-"+Guid.NewGuid().ToString("N"));
            try
            {
                string session=Path.Combine(staging,"hashes","session.xml");
                if (!ValidXml(session)) throw new InvalidDataException("The completed session XML is missing or invalid.");
                Copy(session,Path.Combine(temporary,"hashes","session.xml"),cancellation);
                foreach(string name in new[] {"intermediate","streams"})
                {
                    string source=Path.Combine(staging,name);
                    if (!Directory.Exists(source)) continue;
                    foreach(string path in Files(source,cancellation))
                    {
                        string relative=path.Substring(source.TrimEnd(Path.DirectorySeparatorChar).Length+1);
                        if (Temporary(relative) || name=="streams" && !string.Equals(Path.GetExtension(path),".cdata",StringComparison.OrdinalIgnoreCase)) continue;
                        Copy(path,Path.Combine(temporary,name,relative),cancellation);
                    }
                }
                storage.StageStream(taskKey,temporary,cancellation);
            }
            finally { Delete(temporary); }
        }

        // Core can reject a well-formed XML document whose serialized state is
        // incompatible. Mark that entire package for reversible cold repair.
        public void InvalidateStream(string taskKey) { Active(); storage.InvalidateStream(taskKey); }
        public void Commit() { Active(); storage.Commit(); }
        public void Rollback() { if (!disposed) storage.Rollback(); }
        public void Complete() { if (!disposed) storage.Complete(); }
        public void Dispose() { if (disposed) return; storage.Dispose(); disposed=true; }

        static bool ValidPackage(string root,CancellationToken cancellation)
        {
            bool session=false;
            foreach(string path in Files(root,cancellation))
            {
                string relative=path.Substring(root.TrimEnd(Path.DirectorySeparatorChar).Length+1);
                string normalized=relative.Replace('\\','/');
                if (normalized=="hashes/session.xml") { session=true; continue; }
                if (Temporary(relative)) return false;
                if (normalized.StartsWith("intermediate/",StringComparison.Ordinal)) continue;
                if (normalized.StartsWith("streams/",StringComparison.Ordinal) && string.Equals(Path.GetExtension(path),".cdata",StringComparison.OrdinalIgnoreCase)) continue;
                return false;
            }
            return session && ValidXml(Path.Combine(root,"hashes","session.xml"));
        }
        static bool ValidXml(string path)
        {
            if (!File.Exists(path)) return false;
            try
            {
                var settings=new XmlReaderSettings { DtdProcessing=DtdProcessing.Prohibit, XmlResolver=null };
                using(var reader=XmlReader.Create(path,settings))
                {
                    reader.MoveToContent();
                    if(reader.LocalName!="sc" || reader.NamespaceURI.Length!=0 || reader.GetAttribute("d")==null) return false;
                    while(reader.Read()) { }
                }
                return true;
            }
            catch(XmlException) { return false; }
            catch(IOException) { return false; }
            catch(UnauthorizedAccessException) { return false; }
        }
        static bool Temporary(string relative)
        {
            foreach(string part in relative.Split(new[] {'\\','/'}))
                if(part.EndsWith(".old",StringComparison.OrdinalIgnoreCase) || part.EndsWith(".tmp",StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }
        string Staging(string path)
        {
            if(string.IsNullOrEmpty(path) || !Path.IsPathRooted(path)) throw new ArgumentException("An absolute stream staging directory is required.");
            string full=Path.GetFullPath(path), prefix=outputRoot.TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar;
            if(!full.StartsWith(prefix,StringComparison.OrdinalIgnoreCase) || full.StartsWith(Path.Combine(outputRoot,".cache")+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase) || string.Equals(full,Path.Combine(outputRoot,".cache"),StringComparison.OrdinalIgnoreCase))
                throw new IOException("Session staging must be isolated beneath output.");
            RejectLinks(full); return full;
        }
        static bool Empty(string path) { return !Directory.Exists(path) || Directory.GetFileSystemEntries(path).Length==0; }
        static void Copy(string source,string target,CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested(); RejectLinks(source); Directory.CreateDirectory(Path.GetDirectoryName(target)); File.Copy(source,target,false);
        }
        static IEnumerable<string> Files(string root,CancellationToken cancellation)
        {
            RejectLinks(root);
            foreach(string path in Directory.GetFileSystemEntries(root))
            {
                cancellation.ThrowIfCancellationRequested(); RejectLinks(path);
                if(Directory.Exists(path)) { foreach(string child in Files(path,cancellation)) yield return child; }
                else yield return path;
            }
        }
        static void RejectLinks(string path)
        {
            for(var item=new DirectoryInfo(path);item!=null;item=item.Parent)
                if(item.Exists && (item.Attributes&FileAttributes.ReparsePoint)!=0) throw new IOException("Session package paths cannot use reparse points.");
            if(File.Exists(path) && (File.GetAttributes(path)&FileAttributes.ReparsePoint)!=0) throw new IOException("Session package files cannot use reparse points.");
        }
        static void Delete(string path)
        {
            // Cleanup failure is an operation failure; do not silently leave a
            // partial package that a later stream might interpret as restored.
            RejectLinks(path); if(Directory.Exists(path)) Directory.Delete(path,true);
        }
        void Active() { if(disposed) throw new ObjectDisposedException("SessionPackage"); }
    }
}
