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
	[CLSCompliant(false)]
	[DebugInfoInPDB]
	[MiscellaneousBits(65)]
	[NativeCppClass]
	public struct ExternalReference
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 12)]
	[MiscellaneousBits(65)]
	[NativeCppClass]
	[DebugInfoInPDB]
	internal struct Block
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 12)]
	[DebugInfoInPDB]
	[MiscellaneousBits(65)]
	[NativeCppClass]
	internal struct GenericBookmark_003Cunsigned_0020int_003E
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 12)]
	[DebugInfoInPDB]
	[MiscellaneousBits(65)]
	[NativeCppClass]
	internal struct GenericBookmark_003Cvoid_0020_002A_003E
	{
	}
}
