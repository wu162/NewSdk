using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential, Size = 12)]
[MiscellaneousBits(65)]
[NativeCppClass]
[DebugInfoInPDB]
internal struct AIStateLinearCombinationHeuristic
{
	[StructLayout(LayoutKind.Sequential, Size = 8)]
	[NativeCppClass]
	[CLSCompliant(false)]
	[DebugInfoInPDB]
	[MiscellaneousBits(65)]
	public struct WeightedHeuristic
	{
	}
}
