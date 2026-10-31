// Stable dictionary hashing on both .NET Framework 4.7.2 and .NET 8.
namespace SwfLib
{
    internal static class HashCode
    {
        internal static int Combine(params object[] values)
        { unchecked { int hash = 17; foreach (object value in values) hash = hash * 31 + (value?.GetHashCode() ?? 0); return hash; } }
    }
}
