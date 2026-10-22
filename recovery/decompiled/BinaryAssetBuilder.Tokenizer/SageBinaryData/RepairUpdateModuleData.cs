using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential, Size = 44)]
[NativeCppClass]
[DebugInfoInPDB]
[MiscellaneousBits(65)]
internal struct RepairUpdateModuleData
{
	[StructLayout(LayoutKind.Sequential, Size = 16)]
	[NativeCppClass]
	[MiscellaneousBits(65)]
	[DebugInfoInPDB]
	[CLSCompliant(false)]
	public struct PercentOfBuildCostToRebuildInfo
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 8)]
	[DebugInfoInPDB]
	[CLSCompliant(false)]
	[MiscellaneousBits(65)]
	[NativeCppClass]
	public struct SelfAudio
	{
	}
}
