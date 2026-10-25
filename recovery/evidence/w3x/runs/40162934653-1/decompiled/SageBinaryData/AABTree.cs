using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential, Size = 16)]
[MiscellaneousBits(65)]
[DebugInfoInPDB]
[NativeCppClass]
internal struct AABTree
{
	[StructLayout(LayoutKind.Sequential, Size = 8)]
	[DebugInfoInPDB]
	[MiscellaneousBits(65)]
	[NativeCppClass]
	[CLSCompliant(false)]
	public struct PolyIndices
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 32)]
	[NativeCppClass]
	[MiscellaneousBits(65)]
	[DebugInfoInPDB]
	[CLSCompliant(false)]
	public struct Node
	{
		[StructLayout(LayoutKind.Sequential, Size = 8)]
		[NativeCppClass]
		[MiscellaneousBits(65)]
		[CLSCompliant(false)]
		[DebugInfoInPDB]
		public struct Children
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 8)]
		[MiscellaneousBits(65)]
		[CLSCompliant(false)]
		[NativeCppClass]
		[DebugInfoInPDB]
		public struct Polys
		{
		}
	}
}
