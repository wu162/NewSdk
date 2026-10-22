using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential, Size = 108)]
[NativeCppClass]
[DebugInfoInPDB]
[MiscellaneousBits(65)]
internal struct ExperienceLevelTemplate
{
	[StructLayout(LayoutKind.Sequential, Size = 12)]
	[MiscellaneousBits(65)]
	[CLSCompliant(false)]
	[DebugInfoInPDB]
	[NativeCppClass]
	public struct LevelUpFxList
	{
	}
}
