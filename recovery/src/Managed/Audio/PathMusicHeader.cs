using System;
using System.IO;
using System.Text;

namespace BinaryAssetBuilder.AudioCompiler
{
    internal static class PathMusicHeader
    {
        private static string CString(string value)
        {
            int end = value.IndexOf('\0');
            return end < 0 ? value : value.Substring(0, end);
        }

        public static bool Parse(string defineType, string line, string name, ref int foundID)
        {
            // Original EASTL searches are ordinal and intentionally accept text
            // before #define. Only a literal ASCII space delimits the symbol.
            line = CString(line); defineType = CString(defineType); name = CString(name);
            if (line.IndexOf("#define", StringComparison.Ordinal) < 0) return false;
            int prefix = line.IndexOf(defineType, StringComparison.Ordinal);
            if (prefix < 0) return false;
            int space = line.IndexOf(' ', prefix);
            if (space < 0) return false;
            int start = prefix + defineType.Length;
            if (space < start || line.Substring(start, space - start) != name) return false;
            int hex = line.IndexOf("0x", StringComparison.Ordinal);
            if (hex < 0) return false;
            // MSVCRT strtol on Windows is signed 32-bit, and saturates on
            // positive overflow. Its end pointer is discarded by the original.
            uint value = 0;
            for (int i = hex + 2; i < line.Length; ++i)
            {
                char c = line[i];
                int digit = c >= '0' && c <= '9' ? c - '0' :
                    c >= 'a' && c <= 'f' ? c - 'a' + 10 :
                    c >= 'A' && c <= 'F' ? c - 'A' + 10 : -1;
                if (digit < 0) break;
                value = value > (0x7fffffffu - (uint)digit) / 16u ? 0x7fffffffu : value * 16u + (uint)digit;
            }
            foundID = (int)value;
            return true;
        }

        public static int Find(string file, string prefix, string name, out bool opened)
        {
            opened = false;
            int result = 0;
            // std::fstream fails to open rather than throwing for nonexistent
            // files; getline's 512-char array stops the scan on an overlong line.
            if (!File.Exists(file)) return result;
            try
            {
                using (StreamReader reader = new StreamReader(file, Encoding.Default, false))
                {
                    opened = true;
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Length >= 511) break;
                        if (Parse(prefix, line, name, ref result)) break;
                    }
                }
            }
            catch (UnauthorizedAccessException) { opened = false; }
            catch (IOException) { opened = false; }
            return result;
        }

        public static uint WeakHash(string text)
        {
            byte[] data = Encoding.Default.GetBytes(CString(text));
            for (int i = 0; i < data.Length; ++i)
                if (data[i] >= 'A' && data[i] <= 'Z') data[i] += 32;
            uint hash = (uint)data.Length;
            if (hash == 0) return 0;
            int at = 0;
            unchecked
            {
                for (int i = data.Length / 4; i > 0; --i, at += 4)
                {
                    hash += (uint)(data[at] | data[at + 1] << 8);
                    hash ^= ((uint)(data[at + 2] | data[at + 3] << 8) ^ (hash << 5)) << 11;
                    hash += hash >> 11;
                }
                switch (data.Length & 3)
                {
                    case 3: hash += (uint)(data[at] | data[at + 1] << 8); hash ^= (((uint)(sbyte)data[at + 2] << 2) ^ hash) << 16; hash += hash >> 11; break;
                    case 2: hash += (uint)(data[at] | data[at + 1] << 8); hash ^= hash << 11; hash += hash >> 17; break;
                    case 1: hash += (uint)(sbyte)data[at]; hash ^= hash << 10; hash += hash >> 1; break;
                }
                hash ^= hash << 3; hash += hash >> 5; hash ^= hash << 2; hash += hash >> 15;
                return (hash << 10) ^ hash;
            }
        }
    }
}
