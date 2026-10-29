using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using NewModCompiler.Diagnostics;

namespace NewModCompiler.Application
{
    // Directory records are fetched once per parent. Public files include
    // ChangeTime; immutable private four-stream snapshots survive hard-link
    // creation without rereading their content merely because ChangeTime moved.
    internal sealed class FileHashCache
    {
        internal int Reads { get; private set; }
        internal int Reuses { get; private set; }
        internal long BytesRead { get; private set; }
        readonly string cachePath;
        readonly Dictionary<string, string> previous = new Dictionary<string, string>(StringComparer.Ordinal);
        readonly Dictionary<string, string> used = new Dictionary<string, string>(StringComparer.Ordinal);
        readonly Dictionary<string, Dictionary<string, FileIdentity>> directories =
            new Dictionary<string, Dictionary<string, FileIdentity>>(StringComparer.OrdinalIgnoreCase);

        internal FileHashCache(string outputRoot)
        {
            cachePath = Path.Combine(Path.GetFullPath(outputRoot), ".file-identities.json");
            if (!File.Exists(cachePath)) return;
            var saved = new JavaScriptSerializer { MaxJsonLength = int.MaxValue }
                .Deserialize<Dictionary<string, object>>(File.ReadAllText(cachePath));
            if (Convert.ToInt32(saved["version"], CultureInfo.InvariantCulture) != 1)
                throw new InvalidDataException("Unsupported file identity cache version.");
            foreach (var entry in (Dictionary<string, object>)saved["entries"])
                previous.Add(entry.Key, (string)entry.Value);
        }

        internal string Hash(string path, bool immutable = false)
        {
            FileIdentity file = Identity(Path.GetFullPath(path));
            string key = Key(file, immutable), hash;
            if (used.TryGetValue(key, out hash)) {Reuses++;return hash;}
            if (!previous.TryGetValue(key, out hash))
            {
                Reads++;BytesRead+=file.Size;
                using (var input = File.OpenRead(file.Path))
                using (var algorithm = SHA256.Create())
                    hash = BitConverter.ToString(algorithm.ComputeHash(input)).Replace("-", "").ToLowerInvariant();
            }
            else Reuses++;
            used.Add(key, hash);
            return hash;
        }
        internal bool TryReceipt(string key,out string value)
        {
            if(used.TryGetValue(key,out value))return true;
            if(!previous.TryGetValue(key,out value))return false;
            used[key]=value;return true;
        }
        internal void Remember(string key,string value) {used[key]=value;}

        static string PublicationKey(IList<Artifact> artifacts, string directory)
        {
            using (var bytes = new MemoryStream())
            using (var writer = new BinaryWriter(bytes))
            {
                writer.Write(directory);
                foreach (var artifact in System.Linq.Enumerable.OrderBy(artifacts, value => value.Path, StringComparer.Ordinal))
                { writer.Write(artifact.Path); writer.Write(artifact.Kind); writer.Write(artifact.Size); writer.Write(artifact.Sha256); }
                writer.Flush(); bytes.Position = 0;
                using (var hash = SHA256.Create()) return "publication|" + Convert.ToBase64String(hash.ComputeHash(bytes));
            }
        }
        internal bool MatchesPublication(IList<Artifact> artifacts, string directory)
        {
            string receipt;
            if (!TryReceipt(PublicationKey(artifacts, directory), out receipt)) return false;
            var entries = new JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Deserialize<Dictionary<string,string>>(receipt);
            foreach (var entry in entries) if (FileIdentity.Stamp(entry.Key) != entry.Value) return false;
            return true;
        }
        internal void RecordPublished(IList<Artifact> artifacts, string directory, bool unchanged)
        {
            if (!unchanged)
            {
                string root = Path.GetFullPath(directory).TrimEnd('\\', '/'), prefix = root + Path.DirectorySeparatorChar;
                var stale = new List<string>();
                foreach (string parent in directories.Keys)
                    if (string.Equals(parent, root, StringComparison.OrdinalIgnoreCase) || parent.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                        stale.Add(parent);
                foreach (string parent in stale) directories.Remove(parent);
            }
            foreach (Artifact artifact in artifacts)
                used[Key(Identity(Path.GetFullPath(artifact.Path)), false)] = artifact.Sha256.ToLowerInvariant();
            if (unchanged) return;
            var entries = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
            foreach (var artifact in artifacts)
            {
                entries[artifact.Path] = FileIdentity.Stamp(artifact.Path);
                for (string parent = Path.GetDirectoryName(artifact.Path); Paths.Contains(directory, parent); parent = Path.GetDirectoryName(parent))
                    if (!entries.ContainsKey(parent)) entries[parent] = FileIdentity.Stamp(parent); else break;
            }
            entries[directory] = FileIdentity.Stamp(directory);
            Remember(PublicationKey(artifacts, directory), new JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Serialize(entries));
        }

        internal void Save()
        {
            bool same = previous.Count == used.Count;
            if (same) foreach (var entry in used)
            {
                string hash;
                if (!previous.TryGetValue(entry.Key, out hash) || hash != entry.Value) { same = false; break; }
            }
            if (same) return;
            Reports.WriteAtomic(cachePath, new Dictionary<string, object> { { "version", 1 }, { "entries", used } });
            previous.Clear();
            foreach (var entry in used) previous.Add(entry.Key, entry.Value);
        }

        FileIdentity Identity(string path)
        {
            string parent = Path.GetDirectoryName(path);
            Dictionary<string, FileIdentity> files;
            if (!directories.TryGetValue(parent, out files))
            {
                files = new Dictionary<string, FileIdentity>(StringComparer.OrdinalIgnoreCase);
                foreach (FileIdentity file in FileIdentity.ReadDirectory(parent, false)) files.Add(file.Path, file);
                directories.Add(parent, files);
            }
            return files[path];
        }

        static string Key(FileIdentity file, bool immutable)
        {
            string identity = file.VolumeSerial.ToString("x16", CultureInfo.InvariantCulture) + "|" +
                file.FileId.ToString("x16", CultureInfo.InvariantCulture) + "|" +
                file.LastWriteTime.ToString("x16", CultureInfo.InvariantCulture) + "|" + file.Size.ToString("x16", CultureInfo.InvariantCulture);
            return immutable ? "inode|" + identity : "path|" + file.Path.ToUpperInvariant() + "|" + identity + "|" +
                file.ChangeTime.ToString("x16", CultureInfo.InvariantCulture);
        }
    }
}
