using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

namespace EA.Allocator;

[StructLayout(LayoutKind.Sequential, Size = 1312)]
[NativeCppClass]
[MiscellaneousBits(64)]
[DebugInfoInPDB]
internal struct GeneralAllocator
{
	[CLSCompliant(false)]
	[MiscellaneousBits(64)]
	[NativeCppClass]
	[DebugInfoInPDB]
	public enum AllocationFlags
	{

	}

	[CLSCompliant(false)]
	[DebugInfoInPDB]
	[MiscellaneousBits(64)]
	[NativeCppClass]
	public enum Alignment
	{

	}

	[StructLayout(LayoutKind.Sequential, Size = 24)]
	[DebugInfoInPDB]
	[CLSCompliant(false)]
	[MiscellaneousBits(65)]
	[NativeCppClass]
	public struct AssertionFailureInfo
	{
	}

	[MiscellaneousBits(64)]
	[DebugInfoInPDB]
	[CLSCompliant(false)]
	[NativeCppClass]
	public enum HookType
	{

	}

	[CLSCompliant(false)]
	[MiscellaneousBits(64)]
	[NativeCppClass]
	[DebugInfoInPDB]
	public enum HookSubType
	{

	}

	[StructLayout(LayoutKind.Sequential, Size = 56)]
	[DebugInfoInPDB]
	[CLSCompliant(false)]
	[NativeCppClass]
	[MiscellaneousBits(65)]
	public struct HookInfo
	{
	}

	[DebugInfoInPDB]
	[NativeCppClass]
	[CLSCompliant(false)]
	[MiscellaneousBits(64)]
	public enum Option
	{

	}

	[StructLayout(LayoutKind.Sequential, Size = 12)]
	[DebugInfoInPDB]
	[NativeCppClass]
	[CLSCompliant(false)]
	[MiscellaneousBits(65)]
	public struct InitCallbackNode
	{
	}

	[CLSCompliant(false)]
	[MiscellaneousBits(64)]
	[DebugInfoInPDB]
	[NativeCppClass]
	public enum HeapValidationLevel
	{

	}

	[NativeCppClass]
	[MiscellaneousBits(64)]
	[DebugInfoInPDB]
	[CLSCompliant(false)]
	public enum AddressType
	{

	}

	[MiscellaneousBits(64)]
	[NativeCppClass]
	[CLSCompliant(false)]
	[DebugInfoInPDB]
	public enum BlockType
	{

	}

	[StructLayout(LayoutKind.Sequential, Size = 24)]
	[MiscellaneousBits(65)]
	[DebugInfoInPDB]
	[CLSCompliant(false)]
	[NativeCppClass]
	public struct BlockInfo
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 256)]
	[MiscellaneousBits(65)]
	[NativeCppClass]
	[CLSCompliant(false)]
	[DebugInfoInPDB]
	public struct SnapshotImage
	{
	}
}
