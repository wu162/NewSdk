using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential, Size = 120)]
[MiscellaneousBits(65)]
[NativeCppClass]
[DebugInfoInPDB]
internal struct GiftManagerSettings
{
	[StructLayout(LayoutKind.Sequential, Size = 116)]
	[MiscellaneousBits(65)]
	[CLSCompliant(false)]
	[NativeCppClass]
	[DebugInfoInPDB]
	public struct ObjectGiftSettings
	{
	}
}
