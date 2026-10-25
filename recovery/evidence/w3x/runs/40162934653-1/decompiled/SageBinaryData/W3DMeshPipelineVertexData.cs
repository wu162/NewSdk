using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential, Size = 48)]
[MiscellaneousBits(65)]
[DebugInfoInPDB]
[NativeCppClass]
internal struct W3DMeshPipelineVertexData
{
	[StructLayout(LayoutKind.Sequential, Size = 8)]
	[DebugInfoInPDB]
	[CLSCompliant(false)]
	[MiscellaneousBits(65)]
	[NativeCppClass]
	public struct Vertices
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 8)]
	[DebugInfoInPDB]
	[CLSCompliant(false)]
	[MiscellaneousBits(65)]
	[NativeCppClass]
	public struct Normals
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 8)]
	[CLSCompliant(false)]
	[DebugInfoInPDB]
	[MiscellaneousBits(65)]
	[NativeCppClass]
	public struct Tangents
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 8)]
	[DebugInfoInPDB]
	[MiscellaneousBits(65)]
	[NativeCppClass]
	[CLSCompliant(false)]
	public struct Binormals
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 8)]
	[CLSCompliant(false)]
	[MiscellaneousBits(65)]
	[NativeCppClass]
	[DebugInfoInPDB]
	public struct VertexColors
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 8)]
	[NativeCppClass]
	[CLSCompliant(false)]
	[DebugInfoInPDB]
	[MiscellaneousBits(65)]
	public struct TexCoords
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 8)]
	[DebugInfoInPDB]
	[NativeCppClass]
	[CLSCompliant(false)]
	[MiscellaneousBits(65)]
	public struct BoneInfluences
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 8)]
	[DebugInfoInPDB]
	[NativeCppClass]
	[CLSCompliant(false)]
	[MiscellaneousBits(65)]
	public struct ShadeIndices
	{
	}
}
