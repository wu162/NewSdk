using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace BinaryAssetBuilder.TextureCompiler;

[StructLayout(LayoutKind.Sequential, Size = 28)]
[CLSCompliant(false)]
[MiscellaneousBits(64)]
[NativeCppClass]
[DebugInfoInPDB]
public struct TexturePacker
{
	[StructLayout(LayoutKind.Sequential, Size = 20)]
	[NativeCppClass]
	[MiscellaneousBits(65)]
	[CLSCompliant(false)]
	[DebugInfoInPDB]
	public struct Texture
	{
	}
}
