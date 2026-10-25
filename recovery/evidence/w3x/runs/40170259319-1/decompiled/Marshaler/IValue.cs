using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace Marshaler;

[StructLayout(LayoutKind.Sequential, Size = 4)]
[NativeCppClass]
[MiscellaneousBits(64)]
[DebugInfoInPDB]
internal static struct IValue
{
}
