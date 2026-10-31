// New in-process boundary for the GPL-3.0 SWF2APT serializer.
using System.Collections.Generic;
namespace eaf2apt
{
    internal sealed class MemoryTexture
    {
        internal readonly int Width, Height;
        internal readonly byte[] Pixels; // top-down BGRA, straight alpha
        internal MemoryTexture(int width, int height, byte[] pixels) { Width = width; Height = height; Pixels = pixels; }
    }
    internal sealed class MemoryMovie
    {
        internal byte[] Apt, Constants, Dat;
        internal readonly SortedDictionary<int, byte[]> Geometry = new();
        internal readonly SortedDictionary<int, MemoryTexture> Textures = new();
    }
}
