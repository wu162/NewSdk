using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential, Size = 24)]
[DebugInfoInPDB]
[NativeCppClass]
[MiscellaneousBits(65)]
internal struct ConquerCardUnit
{
	[StructLayout(LayoutKind.Sequential, Size = 4)]
	[DebugInfoInPDB]
	[NativeCppClass]
	[MiscellaneousBits(65)]
	[CLSCompliant(false)]
	public struct Upgrade
	{
	}
}
