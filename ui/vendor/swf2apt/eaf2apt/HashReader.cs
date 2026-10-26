using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Text.Json;

namespace eaf2apt
{
    static class HashReader
    {
        public record Hashes(string FileName, string SwfHash);

        public static byte[] ReadSwf(string filename, string basename, out bool isChanged)
        {
            var swfTask = File.ReadAllBytesAsync(filename);
            try
            {
                if (!File.Exists(basename + ".xml"))
                {
                    isChanged = true;
                }
                else
                {
                    var swfHashTask = swfTask.ContinueWith(t => Convert.ToBase64String(SHA1.HashData(t.Result)));
                    using var hashFile = File.OpenRead(basename + ".hash");
                    var oldHashTask = JsonSerializer.DeserializeAsync<Hashes>(hashFile);

                    var hash = new Hashes(filename, swfHashTask.Result);
                    isChanged = oldHashTask.Result != hash;
                }
            }
            catch
            {
                Console.WriteLine("Cannot retrieve hash, will not use cached data.");
                isChanged = true;
            }

            try
            {
                return swfTask.Result;
            }
            catch (AggregateException e) when (e.InnerExceptions.Count == 1)
            {
                ExceptionDispatchInfo.Capture(e.InnerExceptions[0]).Throw();
                throw null; // make compiler happy
            }
        }

        public static void SaveHashes(string filename, string basename, byte[] data)
        {
            var hash = new Hashes(filename, Convert.ToBase64String(SHA1.HashData(data)));
            File.WriteAllBytes(basename + ".hash", JsonSerializer.SerializeToUtf8Bytes(hash));
        }
    }
}
