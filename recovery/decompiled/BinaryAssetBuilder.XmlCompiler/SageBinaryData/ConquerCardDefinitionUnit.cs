using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential, Size = 32)]
[NativeCppClass]
[MiscellaneousBits(65)]
[DebugInfoInPDB]
internal struct ConquerCardDefinitionUnit
{
	[StructLayout(LayoutKind.Sequential, Size = 8)]
	[MiscellaneousBits(65)]
	[NativeCppClass]
	[CLSCompliant(false)]
	[DebugInfoInPDB]
	public struct Upgrade
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 8)]
	[CLSCompliant(false)]
	[NativeCppClass]
	[MiscellaneousBits(65)]
	[DebugInfoInPDB]
	public struct Veterancy
	{
	}
}
