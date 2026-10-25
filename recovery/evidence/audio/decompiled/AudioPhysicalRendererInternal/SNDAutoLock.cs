using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace AudioPhysicalRendererInternal;

[StructLayout(LayoutKind.Sequential, Size = 8)]
[MiscellaneousBits(64)]
[DebugInfoInPDB]
[NativeCppClass]
internal struct SNDAutoLock
{
}
