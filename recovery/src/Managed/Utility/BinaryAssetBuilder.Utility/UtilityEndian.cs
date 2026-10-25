namespace BinaryAssetBuilder.Utility;
internal static class UtilityEndian {
 internal static uint Swap(uint x) { return ((x&0xff)<<24)|((x&0xff00)<<8)|((x>>8)&0xff00)|(x>>24); }
 internal static ushort Swap(ushort x) { return (ushort)((x<<8)|(x>>8)); }
}
