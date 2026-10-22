using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential, Size = 32)]
[NativeCppClass]
[DebugInfoInPDB]
[MiscellaneousBits(65)]
internal struct FireWeaponUpdateModuleData
{
	[StructLayout(LayoutKind.Sequential, Size = 16)]
	[MiscellaneousBits(65)]
	[NativeCppClass]
	[CLSCompliant(false)]
	[DebugInfoInPDB]
	public struct FireWeaponNugget
	{
	}
}
