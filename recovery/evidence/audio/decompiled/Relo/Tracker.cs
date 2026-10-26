using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace Relo;

[StructLayout(LayoutKind.Sequential, Size = 88)]
[DebugInfoInPDB]
[NativeCppClass]
[MiscellaneousBits(64)]
internal struct Tracker
{
	[StructLayout(LayoutKind.Sequential, Size = 32)]
	[DebugInfoInPDB]
	[CLSCompliant(false)]
	[MiscellaneousBits(65)]
	[NativeCppClass]
	public struct ExternalReference
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 12)]
	[DebugInfoInPDB]
	[NativeCppClass]
	[MiscellaneousBits(65)]
	internal struct Block
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 12)]
	[NativeCppClass]
	[MiscellaneousBits(65)]
	[DebugInfoInPDB]
	internal struct GenericBookmark_003Cunsigned_0020int_003E
	{
	}
}
