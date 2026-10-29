using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NewModCompiler.Application
{
    internal static class FileTransfer
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool CreateHardLink(string fileName, string existingFileName, IntPtr securityAttributes);

        // Only private, immutable cache snapshots use links. Published files and
        // restored CDATA have independent storage because callers may edit them.
        internal static void Snapshot(string source, string target)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(target));
            if (File.Exists(target)) File.Delete(target);
            if (!CreateHardLink(target, source, IntPtr.Zero)) File.Copy(source, target);
        }

        internal static bool SameContents(string left, string right)
        {
            if (!File.Exists(right)) return false;
            using (var a = File.OpenRead(left))
            using (var b = File.OpenRead(right))
            {
                if (a.Length != b.Length) return false;
                var first = new byte[65536]; var second = new byte[first.Length];
                int count;
                while ((count = a.Read(first, 0, first.Length)) != 0)
                {
                    int read = 0;
                    while (read < count)
                    {
                        int next = b.Read(second, read, count - read);
                        if (next == 0) return false;
                        read += next;
                    }
                    for (int index = 0; index < count; index++) if (first[index] != second[index]) return false;
                }
                return true;
            }
        }
    }
}
