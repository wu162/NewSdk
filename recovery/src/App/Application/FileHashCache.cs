using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using NewModCompiler.Diagnostics;

namespace NewModCompiler.Application
{
    // Input identities are fetched once per parent. ChangeTime preserves
    // invalidation for same-length edits with restored modification times.
    internal sealed class FileHashCache
    {
        internal int Reads { get; private set; }
        internal int Reuses { get; private set; }
        internal long BytesRead { get; private set; }
        readonly string cachePath;
        readonly BuildPerformance performance;
        readonly Dictionary<string, string> previous = new Dictionary<string, string>(StringComparer.Ordinal);
        readonly Dictionary<string, string> used = new Dictionary<string, string>(StringComparer.Ordinal);
        readonly Dictionary<string, Dictionary<string, FileIdentity>> directories =
            new Dictionary<string, Dictionary<string, FileIdentity>>(StringComparer.OrdinalIgnoreCase);

        internal FileHashCache(string outputRoot, BuildPerformance performance = null)
        {
            this.performance=performance;
            cachePath = Path.Combine(Path.GetFullPath(outputRoot), ".file-identities.json");
            if (!File.Exists(cachePath)) return;
            var saved = new JavaScriptSerializer { MaxJsonLength = int.MaxValue }
                .Deserialize<Dictionary<string, object>>(File.ReadAllText(cachePath));
            if (Convert.ToInt32(saved["version"], CultureInfo.InvariantCulture) != 1)
                throw new InvalidDataException("Unsupported file identity cache version.");
            foreach (var entry in (Dictionary<string, object>)saved["entries"])
                previous.Add(entry.Key, (string)entry.Value);
        }

        internal string HashBase64(string path)
        {
            return Base64(Hash(path));
        }
        internal string HashBase64(string path, byte[] contents)
        {
            return Base64(Hash(Identity(Path.GetFullPath(path)), contents));
        }
        static string Base64(string hex)
        {
            var bytes = new byte[32];
            for (int i = 0; i < bytes.Length; i++) bytes[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            return Convert.ToBase64String(bytes);
        }

        internal string Hash(string path)
        {
            return Hash(Identity(Path.GetFullPath(path)));
        }
        string Hash(FileIdentity file, byte[] contents = null)
        {
            string key = Key(file), hash;
            if (used.TryGetValue(key, out hash)) {Reuses++;return hash;}
            if (!previous.TryGetValue(key, out hash))
            {
                Reads++;BytesRead+=file.Size;
                using (performance == null ? null : performance.Measure("identity.sha256",true))
                using (var algorithm = SHA256.Create())
                {
                    byte[] digest;
                    if (contents != null) digest = algorithm.ComputeHash(contents);
                    else using (var input = File.OpenRead(file.Path)) digest = algorithm.ComputeHash(input);
                    hash = BitConverter.ToString(digest).Replace("-", "").ToLowerInvariant();
                }
            }
            else Reuses++;
            used.Add(key, hash);
            return hash;
        }
#if NEWSdk_MONOLITHIC
        internal BinaryAssetBuilder.Core.FileHashInfo CoreIdentity(string path)
        {
            path = Path.GetFullPath(path);
            FileIdentity file;
            if (!DirectoryEntries(Path.GetDirectoryName(path)).TryGetValue(path, out file))
                return new BinaryAssetBuilder.Core.FileHashInfo();
            string key = Key(file), hash;
            if (used.TryGetValue(key, out hash) || previous.TryGetValue(key, out hash))
            { used[key] = hash; Reuses++; }
            return new BinaryAssetBuilder.Core.FileHashInfo { Exists = true,
                LastWriteTime = DateTime.FromFileTimeUtc(file.LastWriteTime).ToLocalTime(),
                Identity = key, ContentHash = hash == null ? null : Base64(hash) };
        }
        internal void RememberCoreHash(string path, string contentHash)
        {
            var file = Identity(Path.GetFullPath(path));
            string key = Key(file);
            if (!used.ContainsKey(key) && !previous.ContainsKey(key)) { Reads++; BytesRead += file.Size; }
            used[key] = BitConverter.ToString(Convert.FromBase64String(contentHash)).Replace("-", "").ToLowerInvariant();
        }
#endif
        internal IDictionary<string, string> SchemaIdentities(IEnumerable<string> paths)
        {
            // A new schema pass refreshes the metadata snapshot before any
            // content-hash receipt is reused, including after a failed load.
            foreach (string path in paths) directories.Remove(Path.GetDirectoryName(path));
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string path in paths)
            {
                FileIdentity file;
                result[path] = DirectoryEntries(Path.GetDirectoryName(path)).TryGetValue(path, out file) ? Key(file) : null;
            }
            return result;
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
            FileIdentity file;
            if (!DirectoryEntries(Path.GetDirectoryName(path)).TryGetValue(path, out file)) throw new FileNotFoundException("Cached file is missing.", path);
            return file;
        }
        Dictionary<string, FileIdentity> DirectoryEntries(string parent)
        {
            Dictionary<string, FileIdentity> files;
            if (!directories.TryGetValue(parent, out files))
            {
                files = new Dictionary<string, FileIdentity>(StringComparer.OrdinalIgnoreCase);
                try { using (performance == null ? null : performance.Measure("identity.directory",true))
                    foreach (FileIdentity file in FileIdentity.ReadDirectory(parent, false)) files.Add(file.Path, file); }
                catch (DirectoryNotFoundException) { }
                directories.Add(parent, files);
            }
            return files;
        }

        internal static string Key(FileIdentity file)
        {
            string identity = file.VolumeSerial.ToString("x16", CultureInfo.InvariantCulture) + "|" +
                file.FileId.ToString("x16", CultureInfo.InvariantCulture) + "|" +
                file.LastWriteTime.ToString("x16", CultureInfo.InvariantCulture) + "|" + file.Size.ToString("x16", CultureInfo.InvariantCulture);
            return "path|" + file.Path.ToUpperInvariant() + "|" + identity + "|" +
                file.ChangeTime.ToString("x16", CultureInfo.InvariantCulture);
        }
    }
}
