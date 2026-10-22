using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace Relo;

[StructLayout(LayoutKind.Sequential, Size = 88)]
[MiscellaneousBits(64)]
[NativeCppClass]
[DebugInfoInPDB]
internal struct Tracker
{
	[StructLayout(LayoutKind.Sequential, Size = 32)]
	[MiscellaneousBits(65)]
	[NativeCppClass]
	[CLSCompliant(false)]
	[DebugInfoInPDB]
	public struct ExternalReference
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 12)]
	[NativeCppClass]
	[DebugInfoInPDB]
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

	[StructLayout(LayoutKind.Sequential, Size = 12)]
	[MiscellaneousBits(65)]
	[DebugInfoInPDB]
	[NativeCppClass]
	internal struct GenericBookmark_003Cvoid_0020_002A_003E
	{
	}
}
