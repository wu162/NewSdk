using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace Relo;

[StructLayout(LayoutKind.Sequential, Size = 88)]
[DebugInfoInPDB]
[MiscellaneousBits(64)]
[NativeCppClass]
internal struct Tracker
{
	[StructLayout(LayoutKind.Sequential, Size = 32)]
	[MiscellaneousBits(65)]
	[CLSCompliant(false)]
	[NativeCppClass]
	[DebugInfoInPDB]
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
	[MiscellaneousBits(65)]
	[DebugInfoInPDB]
	[NativeCppClass]
	internal struct GenericBookmark_003Cunsigned_0020int_003E
	{
	}
}
