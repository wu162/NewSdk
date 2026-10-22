using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualC;

[StructLayout(LayoutKind.Sequential, Size = 4)]
[DebugInfoInPDB]
[NativeCppClass]
[MiscellaneousBits(65)]
internal struct gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E
{
	[SpecialName]
	public unsafe static void _003CMarshalCopy_003E(gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* P_0, gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* P_1)
	{
		IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)P_1));
		*(int*)P_0 = (int)((IntPtr)GCHandle.Alloc(((GCHandle)intPtr).Target)).ToPointer();
	}

	[SpecialName]
	public unsafe static void _003CMarshalDestroy_003E(gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* P_0)
	{
		IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)P_0));
		((GCHandle)intPtr).Free();
		*(int*)P_0 = 0;
	}
}
[StructLayout(LayoutKind.Sequential, Size = 4)]
[DebugInfoInPDB]
[MiscellaneousBits(65)]
[NativeCppClass]
internal struct gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E
{
	[SpecialName]
	public unsafe static void _003CMarshalCopy_003E(gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E* P_0, gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E* P_1)
	{
		IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)P_1));
		*(int*)P_0 = (int)((IntPtr)GCHandle.Alloc(((GCHandle)intPtr).Target)).ToPointer();
	}

	[SpecialName]
	public unsafe static void _003CMarshalDestroy_003E(gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E* P_0)
	{
		IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)P_0));
		((GCHandle)intPtr).Free();
		*(int*)P_0 = 0;
	}
}
