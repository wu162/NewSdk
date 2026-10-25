using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace BinaryAssetBuilder.AudioCompiler;

[StructLayout(LayoutKind.Sequential, Size = 4)]
[NativeCppClass]
[MiscellaneousBits(64)]
[DebugInfoInPDB]
internal struct AutoGSTREAMCloser
{
}
