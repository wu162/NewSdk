using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace Marshaler;

[StructLayout(LayoutKind.Sequential, Size = 4)]
[DebugInfoInPDB]
[NativeCppClass]
[MiscellaneousBits(64)]
internal static struct IValue
{
}
