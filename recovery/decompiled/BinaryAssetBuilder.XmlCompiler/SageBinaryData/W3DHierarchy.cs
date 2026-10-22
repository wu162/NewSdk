using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential, Size = 12)]
[NativeCppClass]
[MiscellaneousBits(65)]
[DebugInfoInPDB]
internal struct W3DHierarchy
{
	[StructLayout(LayoutKind.Sequential, Size = 100)]
	[CLSCompliant(false)]
	[MiscellaneousBits(65)]
	[NativeCppClass]
	[DebugInfoInPDB]
	public struct Pivot
	{
		[StructLayout(LayoutKind.Sequential, Size = 64)]
		[CLSCompliant(false)]
		[NativeCppClass]
		[DebugInfoInPDB]
		[MiscellaneousBits(65)]
		public struct FixupMatrix
		{
		}
	}
}
