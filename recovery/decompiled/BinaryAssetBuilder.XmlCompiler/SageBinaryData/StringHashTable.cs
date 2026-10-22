using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential, Size = 16)]
[NativeCppClass]
[MiscellaneousBits(65)]
[DebugInfoInPDB]
internal struct StringHashTable
{
	[StructLayout(LayoutKind.Sequential, Size = 12)]
	[DebugInfoInPDB]
	[CLSCompliant(false)]
	[NativeCppClass]
	[MiscellaneousBits(65)]
	public struct StringAndHash
	{
	}
}
