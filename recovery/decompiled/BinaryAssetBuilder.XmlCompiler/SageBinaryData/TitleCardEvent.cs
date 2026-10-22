using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential, Size = 24)]
[NativeCppClass]
[DebugInfoInPDB]
[MiscellaneousBits(65)]
internal struct TitleCardEvent
{
	[StructLayout(LayoutKind.Sequential, Size = 44)]
	[CLSCompliant(false)]
	[MiscellaneousBits(65)]
	[DebugInfoInPDB]
	[NativeCppClass]
	public struct TextInstance
	{
	}
}
