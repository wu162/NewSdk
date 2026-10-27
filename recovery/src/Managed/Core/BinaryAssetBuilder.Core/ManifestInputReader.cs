using System.IO;

namespace BinaryAssetBuilder.Core;

// Utility owns manifest parsing and recursive path discovery. Core owns the
// build snapshot, so supply only positive sealed content across that boundary.
// A null stream preserves Utility's legacy sharing and fallback behavior.
internal static class ManifestInputReader
{
    internal static bool Exists(string path)
    {
        bool found = File.Exists(path);
        PhysicalInputSnapshot.ObserveCandidate(path, found);
        return found;
    }
    internal static Stream OpenRead(string path)
    {
        PhysicalInputSnapshot.Entry entry;
        return PhysicalInputSnapshot.TryCapture(Path.GetFullPath(path), out entry) ? entry.OpenRead() : null;
    }
}
