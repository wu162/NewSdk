using System;
using System.Runtime.InteropServices;

namespace EALA.Hash
{
    // Public surface and arithmetic recovered from SDK/tools/EALA.Hash.dll IL.
    // In particular, remainder bytes use signed char, and ANSI strings use
    // their UTF-16 Length as the byte count, just like the original wrapper.
    public class FastHash
    {
        public static uint GetHashCode(string text)
        {
            return GetHashCode((uint)text.Length, text);
        }

        public static unsafe uint GetHashCode(uint hash, string text)
        {
            IntPtr memory = Marshal.StringToHGlobalAnsi(text);
            try { return Hash(hash, (byte*)memory.ToPointer(), (uint)text.Length); }
            finally { Marshal.FreeHGlobal(memory); }
        }

        public static uint GetHashCode(byte[] data)
        {
            return GetHashCode((uint)data.Length, data, data.Length);
        }

        public static uint GetHashCode(byte[] data, int length)
        {
            return GetHashCode((uint)data.Length, data, length);
        }

        public static uint GetHashCode(uint hash, byte[] data)
        {
            return GetHashCode(hash, data, data.Length);
        }

        public static unsafe uint GetHashCode(uint hash, byte[] data, int length)
        {
            // The original pins data[0] even for length zero. Retain its null
            // and empty-array exceptions before checking the unsafe range.
            fixed (byte* bytes = &data[0])
            {
                // Original out-of-bounds pointer reads have no defined result.
                if (length < 0 || length > data.Length)
                    throw new ArgumentOutOfRangeException("length");
                return Hash(hash, bytes, (uint)length);
            }
        }

        private static unsafe uint Read16(byte* data)
        {
            return (uint)(data[0] | (data[1] << 8));
        }

        private static unsafe uint Hash(uint hash, byte* data, uint length)
        {
            unchecked
            {
                if (length == 0 || data == null) return 0;
                uint remainder = length & 3;
                for (uint blocks = length >> 2; blocks != 0; --blocks)
                {
                    hash += Read16(data);
                    hash ^= (Read16(data + 2) ^ (hash << 5)) << 11;
                    data += 4;
                    hash += hash >> 11;
                }
                switch (remainder)
                {
                    case 3:
                        hash += Read16(data);
                        hash ^= (((uint)(sbyte)data[2] << 2) ^ hash) << 16;
                        hash += hash >> 11;
                        break;
                    case 2:
                        hash += Read16(data);
                        hash ^= hash << 11;
                        hash += hash >> 17;
                        break;
                    case 1:
                        hash += (uint)(sbyte)data[0];
                        hash ^= hash << 10;
                        hash += hash >> 1;
                        break;
                }
                hash ^= hash << 3;
                hash += hash >> 5;
                hash ^= hash << 2;
                hash += hash >> 15;
                return hash ^ (hash << 10);
            }
        }
    }
}
