using System;
using System.Collections.Generic;
using System.Text;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.AudioCompiler
{
    // The three PathMusic handlers use a single root and flat allocations. This
    // reproduces that subset of Relo::Tracker, including four-byte block padding.
    internal sealed class PathMusicBuilder
    {
        private readonly List<byte> bytes;
        private readonly List<int> pointers = new List<int>();

        public PathMusicBuilder(int size)
        {
            bytes = new List<byte>(new byte[size]);
        }

        public void Word(int at, uint value)
        {
            for (int i = 0; i != 4; ++i)
                bytes[at + i] = (byte)(value >> (i * 8));
        }

        public void Byte(int at, byte value) { bytes[at] = value; }

        public int Allocate(int pointerAt, byte[] data)
        {
            int offset = bytes.Count;
            bytes.AddRange(data);
            while ((bytes.Count & 3) != 0) bytes.Add(0);
            Word(pointerAt, (uint)offset);
            pointers.Add(pointerAt);
            return offset;
        }

        public void String(int at, string text)
        {
            byte[] data = Encoding.Default.GetBytes(text);
            int length = Array.IndexOf<byte>(data, 0);
            if (length < 0) length = data.Length;
            Word(at, (uint)length);
            byte[] terminated = new byte[length + 1];
            Buffer.BlockCopy(data, 0, terminated, 0, length);
            Allocate(at + 4, terminated);
        }

        public AssetBuffer Finish()
        {
            byte[] relocation = new byte[pointers.Count == 0 ? 0 : (pointers.Count + 1) * 4];
            // These flat handlers allocate each pointed block in declaration
            // order, which is also the original bookmark target-block order.
            for (int j = 0; j < pointers.Count; ++j)
                for (int i = 0; i != 4; ++i)
                    relocation[j * 4 + i] = (byte)((uint)pointers[j] >> (i * 8));
            if (pointers.Count != 0)
                for (int i = 0; i != 4; ++i) relocation[relocation.Length - 4 + i] = 255;
            return new AssetBuffer { InstanceData = bytes.ToArray(), RelocationData = relocation, ImportsData = new byte[0] };
        }
    }
}
