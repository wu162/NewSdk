using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using _003CCppImplementationDetails_003E;
using AudioPhysicalRendererInternal;
using BinaryAssetBuilder.Core;
using EA.Allocator;
using Endian;
using Marshaler;
using Relo;
using SageBinaryData;
using eastl;
using rw.audio.core;
using std;

namespace BinaryAssetBuilder.AudioCompiler;

public class Plugin : IAssetBuilderPlugin, IDisposable
{
	private Tracer m_Tracer = Tracer.GetTracer("AudioCompiler", "Provides Audio processing functionality");

	private uint m_HashCode = 0u;

	private TargetPlatform m_Platform = TargetPlatform.Win32;

	private unsafe rw.audio.core.System* m_sndSystem;

	private unsafe CoreAllocatorWrapper* m_allocator;

	private static string TempFilenameSuffix = "BinaryAssetBuilder.AudioCompiler.tempfile";

	public unsafe Plugin()
	{
		m_sndSystem = null;
		m_allocator = null;
	}

	private void _007EPlugin()
	{
		_0021Plugin();
	}

	private unsafe void _0021Plugin()
	{
		rw.audio.core.System* sndSystem = m_sndSystem;
		if (sndSystem != null)
		{
			global::_003CModule_003E.rw_002Eaudio_002Ecore_002ESystem_002ERelease(sndSystem);
			m_sndSystem = null;
		}
		CoreAllocatorWrapper* allocator = m_allocator;
		if (allocator != null)
		{
			CoreAllocatorWrapper* ptr = allocator;
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, uint, void*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr, 1u);
		}
	}

	public unsafe virtual void Initialize(object configSection, TargetPlatform platform)
	{
		m_HashCode = HashProvider.GetTypeHash(GetType());
		m_Platform = platform;
		if (m_sndSystem == null)
		{
			CoreAllocatorWrapper* ptr = (CoreAllocatorWrapper*)global::_003CModule_003E.@new(1320u);
			CoreAllocatorWrapper* ptr2;
			try
			{
				ptr2 = ((ptr == null) ? null : global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002ECoreAllocatorWrapper_002E_007Bctor_007D(ptr));
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.delete(ptr);
				throw;
			}
			m_allocator = ptr2;
			System.Runtime.CompilerServices.Unsafe.SkipInit(out SNDAutoLock sNDAutoLock);
			global::_003CModule_003E.AudioPhysicalRendererInternal_002ESNDAutoLock_002E_007Bctor_007D(&sNDAutoLock, m_sndSystem = global::_003CModule_003E.rw_002Eaudio_002Ecore_002ESystem_002ECreateInstance((ICoreAllocator*)ptr2, 131072));
			try
			{
				global::_003CModule_003E.rw_002Eaudio_002Ecore_002EEncoderRegistry_002ERegisterAllEncoders(global::_003CModule_003E.rw_002Eaudio_002Ecore_002ESystem_002EGetEncoderRegistry(m_sndSystem));
				global::_003CModule_003E.rw_002Eaudio_002Ecore_002EDecoderRegistry_002ERegisterAllDecoders(global::_003CModule_003E.rw_002Eaudio_002Ecore_002ESystem_002EGetDecoderRegistry(m_sndSystem));
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<SNDAutoLock*, void>)(&global::_003CModule_003E.AudioPhysicalRendererInternal_002ESNDAutoLock_002E_007Bdtor_007D), &sNDAutoLock);
				throw;
			}
			global::_003CModule_003E.AudioPhysicalRendererInternal_002ESNDAutoLock_002E_007Bdtor_007D(&sNDAutoLock);
		}
	}

	public virtual void ReInitialize(object configSection, TargetPlatform platform)
	{
		Initialize(configSection, platform);
	}

	public unsafe string GetLastSIMEXError()
	{
		return new string(global::_003CModule_003E.SIMEX_getlasterr());
	}

	public unsafe void FinalizeTracker(Tracker* tracker, EndianHelper* endian, AssetBuffer buffer)
	{
		System.Runtime.CompilerServices.Unsafe.SkipInit(out Relo.Chunk chunk);
		*(int*)(&chunk) = 0;
		System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4)) = 0;
		System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 8)) = 0;
		System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 12)) = 0;
		System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 16)) = 0;
		System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 20)) = 0;
		try
		{
			global::_003CModule_003E.Relo_002ETracker_002EMakeRelocatable(tracker, &chunk, endian, 0u);
			buffer.InstanceData = new byte[System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4))];
			if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4)) != 0)
			{
				fixed (byte* ptr = &buffer.InstanceData[0])
				{
					try
					{
						// IL cpblk instruction
						System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr, *(int*)(&chunk), System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4)));
					}
					catch
					{
						//try-fault
						ptr = null;
						throw;
					}
				}
			}
			buffer.RelocationData = new byte[System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 12))];
			if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 12)) != 0)
			{
				fixed (byte* ptr2 = &buffer.RelocationData[0])
				{
					try
					{
						// IL cpblk instruction
						System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr2, System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 8)), System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 12)));
					}
					catch
					{
						//try-fault
						ptr2 = null;
						throw;
					}
				}
			}
			buffer.ImportsData = new byte[System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 20))];
			if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 20)) != 0)
			{
				fixed (byte* ptr3 = &buffer.ImportsData[0])
				{
					try
					{
						// IL cpblk instruction
						System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr3, System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 16)), System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 20)));
					}
					catch
					{
						//try-fault
						ptr3 = null;
						throw;
					}
				}
			}
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Relo.Chunk*, void>)(&global::_003CModule_003E.Relo_002EChunk_002E_007Bdtor_007D), &chunk);
			throw;
		}
		if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 8)) != 0)
		{
			global::_003CModule_003E.free((void*)(int)System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 8)));
		}
		if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 16)) != 0)
		{
			global::_003CModule_003E.free((void*)(int)System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 16)));
		}
		if (*(int*)(&chunk) != 0)
		{
			global::_003CModule_003E.free((void*)(int)(*(uint*)(&chunk)));
		}
	}

	public unsafe AssetBuffer ProcessPathMusicMapInstance(InstanceDeclaration declaration)
	{
		AssetBuffer assetBuffer = new AssetBuffer();
		bool flag = ((m_Platform != TargetPlatform.Win32) ? true : false);
		Node* ptr = (Node*)global::_003CModule_003E.@new(16u);
		Node* ptr3;
		try
		{
			if (ptr != null)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E obj);
				gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* pThis = &obj;
				*(int*)(&obj) = (int)((IntPtr)GCHandle.Alloc(declaration.Document.NamespaceManager)).ToPointer();
				System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E obj2);
				try
				{
					gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E* ptr2 = &obj2;
					*(int*)(&obj2) = (int)((IntPtr)GCHandle.Alloc(declaration.Node.CreateNavigator())).ToPointer();
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E_002E_007Bdtor_007D), pThis);
					throw;
				}
				ptr3 = global::_003CModule_003E.Node_002E_007Bctor_007D(ptr, &obj2, &obj);
			}
			else
			{
				ptr3 = null;
			}
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.delete(ptr);
			throw;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
		*(int*)(&nodeHolder) = (int)ptr3;
		try
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker tracker);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out PathMusicMap* ptr4);
			global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003APathMusicMap_003E(&tracker, &ptr4);
			try
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker tracker2);
				System.Runtime.CompilerServices.Unsafe.SkipInit(out PathMusicMapRuntime* ptr5);
				global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003APathMusicMapRuntime_003E(&tracker2, &ptr5);
				try
				{
					System.Runtime.CompilerServices.Unsafe.SkipInit(out EndianHelper endianHelper);
					*(bool*)(&endianHelper) = flag;
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr5, ptr4, 4);
					if (ptr3 != null)
					{
						IValue* ptr6 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)ptr3 + 12)))((nint)ptr3, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_04PBAHLACE_0040File_003F_0024AA_0040), null);
						if (ptr6 != null)
						{
							global::_003CModule_003E.Marshaler_002EMarshal_003Cchar_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr6)))((nint)ptr6), (String_003Cchar_003E*)((byte*)ptr4 + 4), &tracker);
							((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr6 + 4)))((nint)ptr6);
						}
					}
					string value = declaration.XmlNode.Attributes["File"].Value;
					IntPtr hglobal = Marshal.StringToHGlobalAnsi(value);
					_iobuf* ptr7 = global::_003CModule_003E.fopen((sbyte*)hglobal.ToPointer(), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_02JDPG_0040rb_003F_0024AA_0040));
					if (ptr7 != null)
					{
						global::_003CModule_003E.fseek(ptr7, 0, 2);
						PathMusicMapRuntime* ptr8 = (PathMusicMapRuntime*)((byte*)ptr5 + 8);
						*(int*)ptr8 = global::_003CModule_003E.ftell(ptr7);
						global::_003CModule_003E.rewind(ptr7);
						uint objectCount = *(uint*)ptr8;
						PathMusicMapRuntime* ptr9 = (PathMusicMapRuntime*)((byte*)ptr5 + 4);
						sbyte** pointerLocation = (sbyte**)ptr9;
						global::_003CModule_003E.Relo_002ETracker_002EPush(&tracker2, (void**)pointerLocation, 1u, objectCount);
						if (global::_003CModule_003E.fread((void*)(int)(*(uint*)ptr9), 1u, *(uint*)ptr8, ptr7) != (uint)(*(int*)ptr8))
						{
							object[] args = new object[1] { value };
							m_Tracer.TraceWarning("ProcessMusicMapInstance encountered a problem while reading Map data file: {0}", args);
						}
						global::_003CModule_003E.fclose(ptr7);
					}
					else
					{
						object[] args2 = new object[1] { value };
						m_Tracer.TraceWarning("ProcessPathMusicMapInstance could not find the Map Data file: {0}", args2);
					}
					Marshal.FreeHGlobal(hglobal);
					int* ptr10 = (int*)ptr5 + 2;
					if (flag)
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
						*(sbyte*)(&num) = ((sbyte*)ptr10)[3];
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 1)) = ((sbyte*)ptr10)[2];
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 2)) = ((sbyte*)ptr10)[1];
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 3)) = *(sbyte*)ptr10;
						*ptr10 = (int)num;
					}
					FinalizeTracker(&tracker2, &endianHelper, assetBuffer);
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker2);
					throw;
				}
				global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker2);
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker);
				throw;
			}
			global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker);
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&global::_003CModule_003E.Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder);
			throw;
		}
		if (ptr3 != null)
		{
			Node* intPtr = ptr3;
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)intPtr)))((nint)intPtr);
		}
		return assetBuffer;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool ParsePathMusicHeaderLine(sbyte* defineType, sbyte* line, sbyte* name, int* foundID)
	{
		System.Runtime.CompilerServices.Unsafe.SkipInit(out basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E);
		*(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E) = 0;
		System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, 4)) = 0;
		System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, 8)) = 0;
		sbyte* ptr = line;
		if (*line != 0)
		{
			do
			{
				ptr++;
			}
			while (*ptr != 0);
		}
		uint num = (uint)(ptr - (nuint)line);
		global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002ERangeInitialize(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, line, (int)num + line);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3);
		try
		{
			if ((global::_003CModule_003E._003F_003F_B_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_004051 & 1) == 0)
			{
				global::_003CModule_003E._003F_003F_B_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_004051 |= 1u;
				try
				{
					System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref global::_003CModule_003E._003FdefineString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B) = 0;
					System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref global::_003CModule_003E._003FdefineString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B, 4)) = 0;
					System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref global::_003CModule_003E._003FdefineString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B, 8)) = 0;
					sbyte* ptr2 = (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_07MAHLPAJA_0040_003F_0024CDdefine_003F_0024AA_0040);
					do
					{
						ptr2++;
					}
					while (*ptr2 != 0);
					uint num2 = (uint)((ref *(_003F*)ptr2) - (ref System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY07_0024_0024CBD, _003F>(ref global::_003CModule_003E._003F_003F_C_0040_07MAHLPAJA_0040_003F_0024CDdefine_003F_0024AA_0040)));
					global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002ERangeInitialize((basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003FdefineString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_07MAHLPAJA_0040_003F_0024CDdefine_003F_0024AA_0040), (sbyte*)((ref *(_003F*)num2) + (ref System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY07_0024_0024CBD, _003F>(ref global::_003CModule_003E._003F_003F_C_0040_07MAHLPAJA_0040_003F_0024CDdefine_003F_0024AA_0040))));
					global::_003CModule_003E._atexit_m((delegate*<void>)(&global::_003CModule_003E._003F_003F__FdefineString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_0040YMXXZ));
				}
				catch
				{
					//try-fault
					global::_003CModule_003E._003F_003F_B_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_004051 &= 4294967294u;
					throw;
				}
			}
			*(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2) = 0;
			System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2, 4)) = 0;
			System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2, 8)) = 0;
			sbyte* ptr3 = defineType;
			if (*defineType != 0)
			{
				do
				{
					ptr3++;
				}
				while (*ptr3 != 0);
			}
			uint num3 = (uint)(ptr3 - (nuint)defineType);
			global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002ERangeInitialize(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2, defineType, (int)num3 + defineType);
			try
			{
				if ((global::_003CModule_003E._003F_003F_B_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_004051 & 2) == 0)
				{
					global::_003CModule_003E._003F_003F_B_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_004051 |= 2u;
					try
					{
						System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref global::_003CModule_003E._003FspaceString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B) = 0;
						System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref global::_003CModule_003E._003FspaceString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B, 4)) = 0;
						System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref global::_003CModule_003E._003FspaceString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B, 8)) = 0;
						sbyte* ptr4 = (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_01CLKCMJKC_0040_003F5_003F_0024AA_0040);
						do
						{
							ptr4++;
						}
						while (*ptr4 != 0);
						uint num4 = (uint)((ref *(_003F*)ptr4) - (ref System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY01_0024_0024CBD, _003F>(ref global::_003CModule_003E._003F_003F_C_0040_01CLKCMJKC_0040_003F5_003F_0024AA_0040)));
						global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002ERangeInitialize((basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003FspaceString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_01CLKCMJKC_0040_003F5_003F_0024AA_0040), (sbyte*)((ref *(_003F*)num4) + (ref System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY01_0024_0024CBD, _003F>(ref global::_003CModule_003E._003F_003F_C_0040_01CLKCMJKC_0040_003F5_003F_0024AA_0040))));
						global::_003CModule_003E._atexit_m((delegate*<void>)(&global::_003CModule_003E._003F_003F__FspaceString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_0040YMXXZ));
					}
					catch
					{
						//try-fault
						global::_003CModule_003E._003F_003F_B_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_004051 &= 4294967293u;
						throw;
					}
				}
				if ((global::_003CModule_003E._003F_003F_B_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_004051 & 4) == 0)
				{
					global::_003CModule_003E._003F_003F_B_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_004051 |= 4u;
					try
					{
						System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref global::_003CModule_003E._003FhexNumberPrefixString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B) = 0;
						System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref global::_003CModule_003E._003FhexNumberPrefixString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B, 4)) = 0;
						System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref global::_003CModule_003E._003FhexNumberPrefixString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B, 8)) = 0;
						sbyte* ptr5 = (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_02MDDDDAID_00400x_003F_0024AA_0040);
						do
						{
							ptr5++;
						}
						while (*ptr5 != 0);
						uint num5 = (uint)((ref *(_003F*)ptr5) - (ref System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY02_0024_0024CBD, _003F>(ref global::_003CModule_003E._003F_003F_C_0040_02MDDDDAID_00400x_003F_0024AA_0040)));
						global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002ERangeInitialize((basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003FhexNumberPrefixString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_02MDDDDAID_00400x_003F_0024AA_0040), (sbyte*)((ref *(_003F*)num5) + (ref System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY02_0024_0024CBD, _003F>(ref global::_003CModule_003E._003F_003F_C_0040_02MDDDDAID_00400x_003F_0024AA_0040))));
						global::_003CModule_003E._atexit_m((delegate*<void>)(&global::_003CModule_003E._003F_003F__FhexNumberPrefixString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_0040YMXXZ));
					}
					catch
					{
						//try-fault
						global::_003CModule_003E._003F_003F_B_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_004051 &= 4294967291u;
						throw;
					}
				}
				*(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3) = 0;
				System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3, 4)) = 0;
				System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3, 8)) = 0;
				sbyte* ptr6 = name;
				if (*name != 0)
				{
					do
					{
						ptr6++;
					}
					while (*ptr6 != 0);
				}
				uint num6 = (uint)(ptr6 - (nuint)name);
				global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002ERangeInitialize(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3, name, (int)num6 + name);
				try
				{
					if (global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002Efind(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, (basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003FdefineString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B), 0u) == uint.MaxValue)
					{
						goto end_IL_0202;
					}
					goto end_IL_00e3;
					end_IL_0202:;
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3);
					throw;
				}
				if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3) != 0)
				{
					global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3)));
				}
				goto IL_0254;
				end_IL_00e3:;
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2);
				throw;
			}
			goto end_IL_0034;
			IL_0254:
			if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2) != 0)
			{
				global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2)));
			}
			goto IL_0281;
			end_IL_0034:;
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E);
			throw;
		}
		int num7;
		try
		{
			try
			{
				try
				{
					num7 = (int)global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002Efind(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2, 0u);
					if (num7 == -1)
					{
						goto end_IL_029d;
					}
					goto end_IL_029d_2;
					end_IL_029d:;
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3);
					throw;
				}
				if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3) != 0)
				{
					global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3)));
				}
				goto IL_02f0;
				end_IL_029d_2:;
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2);
				throw;
			}
			goto end_IL_029d_3;
			IL_02f0:
			if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2) != 0)
			{
				global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2)));
			}
			goto IL_031d;
			end_IL_029d_3:;
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E);
			throw;
		}
		int num8;
		try
		{
			try
			{
				try
				{
					num8 = (int)global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002Efind(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, (basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003FspaceString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B), (uint)num7);
					if (num8 == -1)
					{
						goto end_IL_0339;
					}
					goto end_IL_0339_2;
					end_IL_0339:;
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3);
					throw;
				}
				if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3) != 0)
				{
					global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3)));
				}
				goto IL_0390;
				end_IL_0339_2:;
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2);
				throw;
			}
			goto end_IL_0339_3;
			IL_0390:
			if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2) != 0)
			{
				global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2)));
			}
			goto IL_03bd;
			end_IL_0339_3:;
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E);
			throw;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E4);
		try
		{
			try
			{
				try
				{
					int num9 = System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2, 4)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2) + num7;
					int n = num8 - num9;
					global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002Esubstr(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E4, (uint)num9, (uint)n);
					try
					{
						if (!global::_003CModule_003E.eastl_002Eoperator_003D_003D_003Cchar_002Cclass_0020eastl_003A_003Aallocator_003E(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E4, &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3))
						{
							goto end_IL_03fc;
						}
						goto end_IL_03d9;
						end_IL_03fc:;
					}
					catch
					{
						//try-fault
						global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E4);
						throw;
					}
					if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E4, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E4) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E4) != 0)
					{
						global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E4)));
					}
					goto IL_044d;
					end_IL_03d9:;
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3);
					throw;
				}
				goto end_IL_03d9_2;
				IL_044d:
				if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3) != 0)
				{
					global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3)));
				}
				goto IL_047a;
				end_IL_03d9_2:;
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2);
				throw;
			}
			goto end_IL_03d9_3;
			IL_047a:
			if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2) != 0)
			{
				global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2)));
			}
			goto IL_04a7;
			end_IL_03d9_3:;
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E);
			throw;
		}
		int num10;
		try
		{
			try
			{
				try
				{
					try
					{
						num10 = (int)global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002Efind(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, (basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003FhexNumberPrefixString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B), 0u);
						if (num10 == -1)
						{
							goto end_IL_04c3;
						}
						goto end_IL_04c3_2;
						end_IL_04c3:;
					}
					catch
					{
						//try-fault
						global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E4);
						throw;
					}
					if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E4, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E4) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E4) != 0)
					{
						global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E4)));
					}
					goto IL_0519;
					end_IL_04c3_2:;
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3);
					throw;
				}
				goto end_IL_04c3_3;
				IL_0519:
				if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3) != 0)
				{
					global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3)));
				}
				goto IL_0546;
				end_IL_04c3_3:;
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2);
				throw;
			}
			goto end_IL_04c3_4;
			IL_0546:
			if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2) != 0)
			{
				global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2)));
			}
			goto IL_0573;
			end_IL_04c3_4:;
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E);
			throw;
		}
		try
		{
			try
			{
				try
				{
					try
					{
						*foundID = global::_003CModule_003E.strtol((sbyte*)(*(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E) + num10), null, 16);
					}
					catch
					{
						//try-fault
						global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E4);
						throw;
					}
					if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E4, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E4) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E4) != 0)
					{
						global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E4)));
					}
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3);
					throw;
				}
				if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3) != 0)
				{
					global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E3)));
				}
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2);
				throw;
			}
			if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2) != 0)
			{
				global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E2)));
			}
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E);
			throw;
		}
		if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E) != 0)
		{
			global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E)));
		}
		return true;
		IL_0573:
		if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E) != 0)
		{
			global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E)));
		}
		return false;
		IL_03bd:
		if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E) != 0)
		{
			global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E)));
		}
		return false;
		IL_0281:
		if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E) != 0)
		{
			global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E)));
		}
		return false;
		IL_031d:
		if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E) != 0)
		{
			global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E)));
		}
		return false;
		IL_04a7:
		if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E) != 0)
		{
			global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E)));
		}
		return false;
	}

	public unsafe AssetBuffer ProcessPathMusicTrackInstance(InstanceDeclaration declaration)
	{
		AssetBuffer assetBuffer = new AssetBuffer();
		bool flag = ((m_Platform != TargetPlatform.Win32) ? true : false);
		Node* ptr = (Node*)global::_003CModule_003E.@new(16u);
		Node* ptr3;
		try
		{
			if (ptr != null)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E obj);
				gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* pThis = &obj;
				*(int*)(&obj) = (int)((IntPtr)GCHandle.Alloc(declaration.Document.NamespaceManager)).ToPointer();
				System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E obj2);
				try
				{
					gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E* ptr2 = &obj2;
					*(int*)(&obj2) = (int)((IntPtr)GCHandle.Alloc(declaration.Node.CreateNavigator())).ToPointer();
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E_002E_007Bdtor_007D), pThis);
					throw;
				}
				ptr3 = global::_003CModule_003E.Node_002E_007Bctor_007D(ptr, &obj2, &obj);
			}
			else
			{
				ptr3 = null;
			}
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.delete(ptr);
			throw;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
		*(int*)(&nodeHolder) = (int)ptr3;
		try
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker tracker);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out PathMusicTrack* ptr4);
			global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003APathMusicTrack_003E(&tracker, &ptr4);
			try
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out EndianHelper endianHelper);
				*(bool*)(&endianHelper) = flag;
				global::_003CModule_003E.Marshaler_002EMarshal((INode*)ptr3, ptr4, &tracker);
				string value = declaration.XmlNode.Attributes["File"].Value;
				if (File.Exists(declaration.CustomDataPath))
				{
					File.Delete(declaration.CustomDataPath);
				}
				if (!File.Exists(value))
				{
					object[] args = new object[1] { value };
					m_Tracer.TraceWarning("ProcessPathMusicTrackInstance could not find the Track Data file: {0}.  Creating an empty dummy file for CDATA.", args);
					File.Create(declaration.CustomDataPath);
				}
				else
				{
					object[] args2 = new object[1] { declaration.CustomDataPath };
					m_Tracer.TraceNote("Creating output file {0}\n", args2);
					File.Copy(value, declaration.CustomDataPath);
					FileAttributes fileAttributes = File.GetAttributes(declaration.CustomDataPath) & ~FileAttributes.ReadOnly;
					File.SetAttributes(declaration.CustomDataPath, fileAttributes);
				}
				int num = 0;
				string value2 = declaration.XmlNode.Attributes["PathfinderTrackHeader"].Value;
				sbyte* ptr5 = (sbyte*)Marshal.StringToHGlobalAnsi(value2).ToPointer();
				string value3 = declaration.XmlNode.Attributes["id"].Value;
				sbyte* name = (sbyte*)Marshal.StringToHGlobalAnsi(value3).ToPointer();
				System.Runtime.CompilerServices.Unsafe.SkipInit(out basic_fstream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E obj3);
				global::_003CModule_003E.std_002Ebasic_fstream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E_002E_007Bctor_007D(&obj3, ptr5, 1, 64, 1);
				try
				{
					if (!global::_003CModule_003E.std_002Eios_base_002Egood((ios_base*)((ref *(_003F*)(*(int*)(*(int*)(&obj3) + 4))) + (ref *(_003F*)(&obj3)))))
					{
						object[] args3 = new object[1] { value2 };
						m_Tracer.TraceWarning("Warning: ProcessPathMusicTrackInstance could not open Event Header file for verfication: {0}", args3);
					}
					else
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0CAA_0040D _0024ArrayType_0024_0024_0024BY0CAA_0040D);
						ios_base* ptr6 = (ios_base*)global::_003CModule_003E.std_002Ebasic_istream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E_002Egetline((basic_istream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E*)(&obj3), (sbyte*)(&_0024ArrayType_0024_0024_0024BY0CAA_0040D), 512);
						if (global::_003CModule_003E.std_002Eios_base_002E_002EPAX((ios_base*)(*(int*)(*(int*)ptr6 + 4) + (byte*)ptr6)) != null)
						{
							while (!ParsePathMusicHeaderLine((sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0M_0040MNCGACDM_0040PATH_TRACK__003F_0024AA_0040), (sbyte*)(&_0024ArrayType_0024_0024_0024BY0CAA_0040D), name, &num))
							{
								ptr6 = (ios_base*)global::_003CModule_003E.std_002Ebasic_istream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E_002Egetline((basic_istream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E*)(&obj3), (sbyte*)(&_0024ArrayType_0024_0024_0024BY0CAA_0040D), 512);
								if (global::_003CModule_003E.std_002Eios_base_002E_002EPAX((ios_base*)(*(int*)(*(int*)ptr6 + 4) + (byte*)ptr6)) == null)
								{
									break;
								}
							}
						}
						global::_003CModule_003E.std_002Ebasic_fstream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E_002Eclose(&obj3);
						if (num != 0)
						{
							goto IL_0282;
						}
					}
					object[] args4 = new object[2] { value3, value2 };
					m_Tracer.TraceWarning("ProcessPathMusicEventInstance could not find a handle for track: {0} in the header file: {1}", args4);
					goto IL_0282;
					IL_0282:
					((int*)ptr4)[6] = num;
					global::_003CModule_003E.Marshaler_002EEndianCorrect(ptr4, &endianHelper);
					FinalizeTracker(&tracker, &endianHelper, assetBuffer);
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_fstream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E*, void>)(&global::_003CModule_003E.std_002Ebasic_fstream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E_002E__vbaseDtor), &obj3);
					throw;
				}
				global::_003CModule_003E.std_002Ebasic_fstream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E_002E__vbaseDtor(&obj3);
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker);
				throw;
			}
			global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker);
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&global::_003CModule_003E.Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder);
			throw;
		}
		if (ptr3 != null)
		{
			Node* intPtr = ptr3;
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)intPtr)))((nint)intPtr);
		}
		return assetBuffer;
	}

	public unsafe AssetBuffer ProcessPathMusicEventInstance(InstanceDeclaration declaration)
	{
		AssetBuffer assetBuffer = new AssetBuffer();
		bool flag = ((m_Platform != TargetPlatform.Win32) ? true : false);
		Node* ptr = (Node*)global::_003CModule_003E.@new(16u);
		Node* ptr3;
		try
		{
			if (ptr != null)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E obj);
				gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* pThis = &obj;
				*(int*)(&obj) = (int)((IntPtr)GCHandle.Alloc(declaration.Document.NamespaceManager)).ToPointer();
				System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E obj2);
				try
				{
					gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E* ptr2 = &obj2;
					*(int*)(&obj2) = (int)((IntPtr)GCHandle.Alloc(declaration.Node.CreateNavigator())).ToPointer();
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E_002E_007Bdtor_007D), pThis);
					throw;
				}
				ptr3 = global::_003CModule_003E.Node_002E_007Bctor_007D(ptr, &obj2, &obj);
			}
			else
			{
				ptr3 = null;
			}
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.delete(ptr);
			throw;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
		*(int*)(&nodeHolder) = (int)ptr3;
		try
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker tracker);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out PathMusicEvent* ptr4);
			global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003APathMusicEvent_003E(&tracker, &ptr4);
			try
			{
				global::_003CModule_003E.Marshaler_002EMarshal((INode*)ptr3, ptr4, &tracker);
				int num = 0;
				string value = declaration.XmlNode.Attributes["PathfinderEventHeader"].Value;
				IntPtr hglobal = Marshal.StringToHGlobalAnsi(value);
				sbyte* ptr5 = (sbyte*)hglobal.ToPointer();
				string value2 = declaration.XmlNode.Attributes["id"].Value;
				IntPtr hglobal2 = Marshal.StringToHGlobalAnsi(value2);
				sbyte* name = (sbyte*)hglobal2.ToPointer();
				System.Runtime.CompilerServices.Unsafe.SkipInit(out basic_fstream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E obj3);
				global::_003CModule_003E.std_002Ebasic_fstream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E_002E_007Bctor_007D(&obj3, ptr5, 1, 64, 1);
				try
				{
					if (!global::_003CModule_003E.std_002Eios_base_002Egood((ios_base*)((ref *(_003F*)(*(int*)(*(int*)(&obj3) + 4))) + (ref *(_003F*)(&obj3)))))
					{
						object[] args = new object[1] { value };
						m_Tracer.TraceWarning("Warning: ProcessPathMusicEventInstance could not open Event Header file for verfication: {0}", args);
					}
					else
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0CAA_0040D _0024ArrayType_0024_0024_0024BY0CAA_0040D);
						ios_base* ptr6 = (ios_base*)global::_003CModule_003E.std_002Ebasic_istream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E_002Egetline((basic_istream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E*)(&obj3), (sbyte*)(&_0024ArrayType_0024_0024_0024BY0CAA_0040D), 512);
						if (global::_003CModule_003E.std_002Eios_base_002E_002EPAX((ios_base*)(*(int*)(*(int*)ptr6 + 4) + (byte*)ptr6)) != null)
						{
							while (!ParsePathMusicHeaderLine((sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0M_0040IHIOENAA_0040PATH_EVENT__003F_0024AA_0040), (sbyte*)(&_0024ArrayType_0024_0024_0024BY0CAA_0040D), name, &num))
							{
								ptr6 = (ios_base*)global::_003CModule_003E.std_002Ebasic_istream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E_002Egetline((basic_istream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E*)(&obj3), (sbyte*)(&_0024ArrayType_0024_0024_0024BY0CAA_0040D), 512);
								if (global::_003CModule_003E.std_002Eios_base_002E_002EPAX((ios_base*)(*(int*)(*(int*)ptr6 + 4) + (byte*)ptr6)) == null)
								{
									break;
								}
							}
						}
						global::_003CModule_003E.std_002Ebasic_fstream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E_002Eclose(&obj3);
						if (num != 0)
						{
							goto IL_01cb;
						}
					}
					object[] args2 = new object[2] { value2, value };
					m_Tracer.TraceWarning("ProcessPathMusicEventInstance could not find an ID for event: {0} in the header file: {1}", args2);
					goto IL_01cb;
					IL_01cb:
					System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker tracker2);
					System.Runtime.CompilerServices.Unsafe.SkipInit(out PathMusicEventRuntime* ptr7);
					global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003APathMusicEventRuntime_003E(&tracker2, &ptr7);
					try
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out EndianHelper endianHelper);
						*(bool*)(&endianHelper) = flag;
						// IL cpblk instruction
						System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr7, ptr4, 4);
						PathMusicEvent* ptr8 = (PathMusicEvent*)((byte*)ptr4 + 12);
						if (*(int*)ptr8 != 0)
						{
							PathMusicEventRuntime* ptr9 = (PathMusicEventRuntime*)((byte*)ptr7 + 8);
							sbyte** pointerLocation = (sbyte**)ptr9;
							global::_003CModule_003E.Relo_002ETracker_002EPush(&tracker2, (void**)pointerLocation, 1u, 4u);
							// IL cpblk instruction
							System.Runtime.CompilerServices.Unsafe.CopyBlock(*(int*)ptr9, *(int*)ptr8, 4);
						}
						((sbyte*)ptr7)[12] = ((sbyte*)ptr4)[16];
						((int*)ptr7)[1] = num;
						Marshal.FreeHGlobal(hglobal);
						Marshal.FreeHGlobal(hglobal2);
						global::_003CModule_003E.Marshaler_002EEndianCorrect(ptr7, &endianHelper);
						FinalizeTracker(&tracker2, &endianHelper, assetBuffer);
					}
					catch
					{
						//try-fault
						global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker2);
						throw;
					}
					global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker2);
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_fstream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E*, void>)(&global::_003CModule_003E.std_002Ebasic_fstream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E_002E__vbaseDtor), &obj3);
					throw;
				}
				global::_003CModule_003E.std_002Ebasic_fstream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E_002E__vbaseDtor(&obj3);
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker);
				throw;
			}
			global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker);
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&global::_003CModule_003E.Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder);
			throw;
		}
		if (ptr3 != null)
		{
			Node* intPtr = ptr3;
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)intPtr)))((nint)intPtr);
		}
		return assetBuffer;
	}

	public unsafe AssetBuffer ProcessAudioFileMP3PassthroughInstance(InstanceDeclaration declaration)
	{
		AssetBuffer assetBuffer = new AssetBuffer();
		bool flag = m_Platform == TargetPlatform.Xbox360;
		Node* ptr = (Node*)global::_003CModule_003E.@new(16u);
		Node* ptr3;
		try
		{
			if (ptr != null)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E obj);
				gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* pThis = &obj;
				*(int*)(&obj) = (int)((IntPtr)GCHandle.Alloc(declaration.Document.NamespaceManager)).ToPointer();
				System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E obj2);
				try
				{
					gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E* ptr2 = &obj2;
					*(int*)(&obj2) = (int)((IntPtr)GCHandle.Alloc(declaration.Node.CreateNavigator())).ToPointer();
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E_002E_007Bdtor_007D), pThis);
					throw;
				}
				ptr3 = global::_003CModule_003E.Node_002E_007Bctor_007D(ptr, &obj2, &obj);
			}
			else
			{
				ptr3 = null;
			}
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.delete(ptr);
			throw;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
		*(int*)(&nodeHolder) = (int)ptr3;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker tracker);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker tracker2);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out AudioFileMP3PassthroughRuntime* ptr5);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out EndianHelper endianHelper);
		string value;
		AudioFileMP3PassthroughRuntime* ptr7;
		sbyte* ptr8;
		AssetBuffer result;
		IntPtr hglobal2;
		try
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out AudioFileMP3Passthrough* ptr4);
			global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AAudioFileMP3Passthrough_003E(&tracker, &ptr4);
			try
			{
				global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AAudioFileMP3PassthroughRuntime_003E(&tracker2, &ptr5);
				try
				{
					*(bool*)(&endianHelper) = flag;
					global::_003CModule_003E.Marshaler_002EMarshal((INode*)ptr3, ptr4, &tracker);
					((sbyte*)ptr5)[13] = ((sbyte*)ptr4)[16];
					value = declaration.XmlNode.Attributes["File"].Value;
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr5, ptr4, 4);
					AudioFileMP3Passthrough* ptr6 = (AudioFileMP3Passthrough*)((byte*)ptr4 + 12);
					if (*(int*)ptr6 != 0)
					{
						ptr7 = (AudioFileMP3PassthroughRuntime*)((byte*)ptr5 + 4);
						global::_003CModule_003E.Marshaler_002EMarshal_003Cchar_003E((sbyte*)(int)(*(uint*)(*(int*)ptr6 + 4)), (String_003Cchar_003E*)ptr7, &tracker2);
					}
					else
					{
						IntPtr hglobal = Marshal.StringToHGlobalAnsi("DIALOGEVENT:" + Path.GetFileNameWithoutExtension(value) + "SubTitle");
						void* text = hglobal.ToPointer();
						ptr7 = (AudioFileMP3PassthroughRuntime*)((byte*)ptr5 + 4);
						global::_003CModule_003E.Marshaler_002EMarshal_003Cchar_003E((sbyte*)text, (String_003Cchar_003E*)ptr7, &tracker2);
						Marshal.FreeHGlobal(hglobal);
					}
					hglobal2 = Marshal.StringToHGlobalAnsi(value);
					ptr8 = (sbyte*)hglobal2.ToPointer();
					if (global::_003CModule_003E.SIMEX_id(ptr8, 0L) != 34)
					{
						Console.WriteLine("Warning: Passthrough is for MP3 format only.  Cannot process \"{0}\"", value);
						result = null;
						goto IL_0183;
					}
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker2);
					throw;
				}
				goto end_IL_00a2;
				IL_0183:
				global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker2);
				goto IL_019a;
				end_IL_00a2:;
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker);
				throw;
			}
			goto end_IL_0098;
			IL_019a:
			global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker);
			goto IL_01b1;
			end_IL_0098:;
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&global::_003CModule_003E.Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder);
			throw;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out AutoSINSTANCECloser autoSINSTANCECloser);
		try
		{
			try
			{
				try
				{
					SINSTANCE* ptr9 = null;
					int num = global::_003CModule_003E.SIMEX_open(ptr8, 0L, 34, &ptr9);
					Marshal.FreeHGlobal(hglobal2);
					hglobal2 = IntPtr.Zero;
					*(int*)(&autoSINSTANCECloser) = (int)ptr9;
					try
					{
						if (num > 0 && ptr9 != null)
						{
							int num2 = 0;
							if (0 < num)
							{
								System.Runtime.CompilerServices.Unsafe.SkipInit(out AutoSINFOFreer autoSINFOFreer);
								do
								{
									SINFO* ptr10 = null;
									if (global::_003CModule_003E.SIMEX_info(ptr9, &ptr10, num2) != 0)
									{
										*(int*)(&autoSINFOFreer) = (int)ptr10;
										try
										{
											if (*(int*)ptr10 != 0)
											{
												byte b = (byte)(*(sbyte*)(*(int*)ptr10 + 502));
												AudioFileMP3PassthroughRuntime* ptr11 = (AudioFileMP3PassthroughRuntime*)((byte*)ptr5 + 12);
												*(byte*)ptr11 = b;
												if (b != 1)
												{
													byte b2 = *(byte*)ptr11;
													if (b2 != 2)
													{
														Console.WriteLine("Warning: MP3 Audio file {0} has {1} channels. The only supported channel counts are 1, 2", declaration, b2);
													}
												}
											}
										}
										catch
										{
											//try-fault
											global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<AutoSINFOFreer*, void>)(&global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINFOFreer_002E_007Bdtor_007D), &autoSINFOFreer);
											throw;
										}
										if (*(int*)(&autoSINFOFreer) != 0)
										{
											global::_003CModule_003E.SIMEX_freesinfo((SINFO*)(int)(*(uint*)(&autoSINFOFreer)));
										}
										num2++;
										continue;
									}
									object[] array = new object[3] { num2, value, null };
									string text2 = new string(global::_003CModule_003E.SIMEX_getlasterr());
									array[2] = text2;
									throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Internal error reading element {0} of \"{1}\" (SIMEX_info(): {2}).", array);
								}
								while (num2 < num);
							}
							if (File.Exists(declaration.CustomDataPath))
							{
								File.Delete(declaration.CustomDataPath);
							}
							File.Copy(value, declaration.CustomDataPath);
							String_003Cchar_003E* ptr12 = (String_003Cchar_003E*)ptr7;
							if (flag)
							{
								System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num3);
								*(sbyte*)(&num3) = ((sbyte*)ptr12)[3];
								System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 1)) = ((sbyte*)ptr12)[2];
								System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 2)) = ((sbyte*)ptr12)[1];
								System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 3)) = *(sbyte*)ptr12;
								*(uint*)ptr12 = num3;
							}
							FinalizeTracker(&tracker2, &endianHelper, assetBuffer);
							goto IL_0327;
						}
					}
					catch
					{
						//try-fault
						global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<AutoSINSTANCECloser*, void>)(&global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINSTANCECloser_002E_007Bdtor_007D), &autoSINSTANCECloser);
						throw;
					}
					goto end_IL_01c1;
					IL_0327:
					if (*(int*)(&autoSINSTANCECloser) != 0)
					{
						global::_003CModule_003E.SIMEX_close((SINSTANCE*)(int)(*(uint*)(&autoSINSTANCECloser)));
					}
					goto IL_0347;
					end_IL_01c1:;
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker2);
					throw;
				}
				goto end_IL_01c1_2;
				IL_0347:
				global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker2);
				goto IL_035e;
				end_IL_01c1_2:;
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker);
				throw;
			}
			goto end_IL_01c1_3;
			IL_035e:
			global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker);
			goto IL_0375;
			end_IL_01c1_3:;
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&global::_003CModule_003E.Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder);
			throw;
		}
		AssetBuffer result2;
		try
		{
			try
			{
				try
				{
					try
					{
						string arg = new string(global::_003CModule_003E.SIMEX_getlasterr());
						Console.WriteLine("Warning: Could not begin audio processing of \"{0}\": {1}.", value, arg);
						result2 = null;
					}
					catch
					{
						//try-fault
						global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<AutoSINSTANCECloser*, void>)(&global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINSTANCECloser_002E_007Bdtor_007D), &autoSINSTANCECloser);
						throw;
					}
					if (*(int*)(&autoSINSTANCECloser) != 0)
					{
						global::_003CModule_003E.SIMEX_close((SINSTANCE*)(int)(*(uint*)(&autoSINSTANCECloser)));
					}
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker2);
					throw;
				}
				global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker2);
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker);
				throw;
			}
			global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker);
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&global::_003CModule_003E.Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder);
			throw;
		}
		if (ptr3 != null)
		{
			Node* intPtr = ptr3;
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)intPtr)))((nint)intPtr);
		}
		return result2;
		IL_01b1:
		if (ptr3 != null)
		{
			Node* intPtr2 = ptr3;
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)intPtr2)))((nint)intPtr2);
		}
		return result;
		IL_0375:
		if (ptr3 != null)
		{
			Node* intPtr3 = ptr3;
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)intPtr3)))((nint)intPtr3);
		}
		return assetBuffer;
	}

	public unsafe AssetBuffer ProcessAudioFileInstance(InstanceDeclaration declaration)
	{
		AutoCleanUpTempFiles autoCleanUpTempFiles = null;
		AssetBuffer assetBuffer = new AssetBuffer();
		bool flag = ((m_Platform != TargetPlatform.Win32) ? true : false);
		Node* ptr = (Node*)global::_003CModule_003E.@new(16u);
		Node* ptr3;
		try
		{
			if (ptr != null)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E obj);
				gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* pThis = &obj;
				gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* namespaceManager = global::_003CModule_003E.gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E_002E_007Bctor_007D(&obj, declaration.Document.NamespaceManager);
				gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E* navigator;
				try
				{
					System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E obj2);
					gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E* ptr2 = &obj2;
					navigator = global::_003CModule_003E.gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E_002E_007Bctor_007D(&obj2, declaration.Node.CreateNavigator());
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E_002E_007Bdtor_007D), pThis);
					throw;
				}
				ptr3 = global::_003CModule_003E.Node_002E_007Bctor_007D(ptr, navigator, namespaceManager);
			}
			else
			{
				ptr3 = null;
			}
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.delete(ptr);
			throw;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
		*(int*)(&nodeHolder) = (int)ptr3;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker tracker);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker tracker2);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out AudioFileRuntime* ptr5);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out EndianHelper endianHelper);
		string value;
		int* ptr7;
		int num;
		int num3;
		bool flag3;
		AudioFileRuntime* ptr10;
		AudioFileRuntime* ptr11;
		AudioFileRuntime* ptr12;
		AudioFileRuntime* ptr13;
		AudioFileRuntime* ptr14;
		sbyte* ptr15;
		AssetBuffer result;
		IntPtr hglobal2;
		try
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out AudioFile* ptr4);
			global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AAudioFile_003E(&tracker, &ptr4);
			try
			{
				global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AAudioFileRuntime_003E(&tracker2, &ptr5);
				try
				{
					*(bool*)(&endianHelper) = flag;
					global::_003CModule_003E.Marshaler_002EMarshal((INode*)ptr3, ptr4, &tracker);
					value = declaration.XmlNode.Attributes["File"].Value;
					bool flag2 = Path.GetDirectoryName(value).ToLower().EndsWith("\\sounds");
					TargetPlatform platform = m_Platform;
					bool* ptr6;
					switch (platform)
					{
					default:
						throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Internal error: unknown platform {0}", platform);
					case TargetPlatform.PlayStation3:
					{
						ptr6 = (bool*)(int)((uint*)ptr4)[14];
						ptr7 = (int*)(int)((uint*)ptr4)[11];
						num = ((int*)ptr4)[13];
						AudioFile* ptr8 = (AudioFile*)((byte*)ptr4 + 48);
						uint num2 = *(uint*)ptr8;
						if (num2 == 0)
						{
							num3 = 29;
							break;
						}
						switch (*(int*)(int)(*(uint*)ptr8))
						{
						default:
							throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Internal error: xml compiler returned bad PS3 audio compression type of {0}", (int)num2);
						case 1:
							if (!global::_003CModule_003E._003Fs_gaveMP3Warning_0040_003FCH_0040_003F_003FProcessAudioFileInstance_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAMP_0024AAVAssetBuffer_0040Core_00404_0040P_0024AAVInstanceDeclaration_004064_0040_0040Z_00404_NA)
							{
								Console.WriteLine("WARNING: MP3-style compression unavailable due to legal restrictions. Files will not be compressed");
								global::_003CModule_003E._003Fs_gaveMP3Warning_0040_003FCH_0040_003F_003FProcessAudioFileInstance_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAMP_0024AAVAssetBuffer_0040Core_00404_0040P_0024AAVInstanceDeclaration_004064_0040_0040Z_00404_NA = true;
							}
							num3 = 29;
							break;
						case 0:
							num3 = 29;
							break;
						}
						break;
					}
					case TargetPlatform.Xbox360:
					{
						ptr6 = (bool*)(int)((uint*)ptr4)[10];
						ptr7 = (int*)(int)((uint*)ptr4)[7];
						num = ((int*)ptr4)[9];
						AudioFile* ptr8 = (AudioFile*)((byte*)ptr4 + 32);
						uint num2 = *(uint*)ptr8;
						if (num2 == 0)
						{
							num3 = 28;
							break;
						}
						switch (*(int*)(int)(*(uint*)ptr8))
						{
						default:
							throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Internal error: xml compiler returned bad 360 audio compression type of {0}", (int)num2);
						case 2:
							if (!global::_003CModule_003E._003Fs_gaveMP3Warning_0040_003FM_0040_003F_003FProcessAudioFileInstance_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAMP_0024AAVAssetBuffer_0040Core_00404_0040P_0024AAVInstanceDeclaration_004064_0040_0040Z_00404_NA)
							{
								Console.WriteLine("WARNING: MP3-style compression unavailable due to legal restrictions. Files will not be compressed");
								global::_003CModule_003E._003Fs_gaveMP3Warning_0040_003FM_0040_003F_003FProcessAudioFileInstance_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAMP_0024AAVAssetBuffer_0040Core_00404_0040P_0024AAVInstanceDeclaration_004064_0040_0040Z_00404_NA = true;
							}
							num3 = 1;
							break;
						case 1:
							num3 = 28;
							break;
						case 0:
							num3 = 1;
							break;
						}
						break;
					}
					case TargetPlatform.Win32:
					{
						ptr6 = (bool*)(int)((uint*)ptr4)[6];
						ptr7 = (int*)(int)((uint*)ptr4)[3];
						num = ((int*)ptr4)[5];
						AudioFile* ptr8 = (AudioFile*)((byte*)ptr4 + 16);
						uint num2 = *(uint*)ptr8;
						if (num2 == 0)
						{
							num3 = ((!flag2) ? 1 : 29);
							break;
						}
						switch (*(int*)(int)(*(uint*)ptr8))
						{
						default:
							throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Internal error: xml compiler returned bad PC audio compression type of {0}", (int)num2);
						case 2:
							if (!global::_003CModule_003E._003Fs_gaveMP3Warning_0040_003FBM_0040_003F_003FProcessAudioFileInstance_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAMP_0024AAVAssetBuffer_0040Core_00404_0040P_0024AAVInstanceDeclaration_004064_0040_0040Z_00404_NA)
							{
								Console.WriteLine("WARNING: MP3-style compression unavailable due to legal restrictions. Files will not be compressed");
								global::_003CModule_003E._003Fs_gaveMP3Warning_0040_003FBM_0040_003F_003FProcessAudioFileInstance_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAMP_0024AAVAssetBuffer_0040Core_00404_0040P_0024AAVInstanceDeclaration_004064_0040_0040Z_00404_NA = true;
							}
							num3 = 1;
							break;
						case 1:
							num3 = 29;
							break;
						case 0:
							num3 = 1;
							break;
						}
						break;
					}
					}
					flag3 = ((ptr6 == null) ? (!flag2) : (*ptr6));
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr5, ptr4, 4);
					AudioFile* ptr9 = (AudioFile*)((byte*)ptr4 + 60);
					if (*(int*)ptr9 != 0)
					{
						global::_003CModule_003E.Marshaler_002EMarshal_003Cchar_003E((sbyte*)(int)(*(uint*)(*(int*)ptr9 + 4)), (String_003Cchar_003E*)((byte*)ptr5 + 4), &tracker2);
					}
					else
					{
						IntPtr hglobal = Marshal.StringToHGlobalAnsi("DIALOGEVENT:" + Path.GetFileNameWithoutExtension(value) + "SubTitle");
						global::_003CModule_003E.Marshaler_002EMarshal_003Cchar_003E((sbyte*)hglobal.ToPointer(), (String_003Cchar_003E*)((byte*)ptr5 + 4), &tracker2);
						Marshal.FreeHGlobal(hglobal);
						hglobal = IntPtr.Zero;
					}
					ptr10 = (AudioFileRuntime*)((byte*)ptr5 + 28);
					*(sbyte*)ptr10 = 0;
					ptr11 = (AudioFileRuntime*)((byte*)ptr5 + 12);
					*(int*)ptr11 = 0;
					ptr12 = (AudioFileRuntime*)((byte*)ptr5 + 16);
					*(int*)ptr12 = 0;
					ptr13 = (AudioFileRuntime*)((byte*)ptr5 + 20);
					*(int*)ptr13 = 0;
					ptr14 = (AudioFileRuntime*)((byte*)ptr5 + 24);
					*(int*)ptr14 = 0;
					hglobal2 = Marshal.StringToHGlobalAnsi(value);
					ptr15 = (sbyte*)hglobal2.ToPointer();
					if (ptr15 == null)
					{
						Console.WriteLine("Warning: Couldn't open audio file \"{0}\".", value);
						result = null;
						goto IL_03a7;
					}
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker2);
					throw;
				}
				goto end_IL_0091;
				IL_03a7:
				global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker2);
				goto IL_03be;
				end_IL_0091:;
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker);
				throw;
			}
			goto end_IL_0087;
			IL_03be:
			global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker);
			goto IL_03d5;
			end_IL_0087:;
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&global::_003CModule_003E.Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder);
			throw;
		}
		int num4;
		try
		{
			try
			{
				try
				{
					num4 = global::_003CModule_003E.SIMEX_id(ptr15, 0L);
					if (num4 < 0)
					{
						Console.WriteLine("Warning: Unable to identify format of \"{0}\"; cannot process. (Error: {1})", value, GetLastSIMEXError());
						result = null;
						goto IL_0417;
					}
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker2);
					throw;
				}
				goto end_IL_03e0;
				IL_0417:
				global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker2);
				goto IL_042e;
				end_IL_03e0:;
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker);
				throw;
			}
			goto end_IL_03e0_2;
			IL_042e:
			global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker);
			goto IL_0445;
			end_IL_03e0_2:;
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&global::_003CModule_003E.Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder);
			throw;
		}
		try
		{
			try
			{
				try
				{
					if (num4 != 1 && num4 != 0)
					{
						Console.WriteLine("Warning: Input files must be WAVE or AIFF format. Cannot process \"{0}\"", value);
						result = null;
						goto IL_047c;
					}
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker2);
					throw;
				}
				goto end_IL_0450;
				IL_047c:
				global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker2);
				goto IL_0493;
				end_IL_0450:;
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker);
				throw;
			}
			goto end_IL_0450_2;
			IL_0493:
			global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker);
			goto IL_04aa;
			end_IL_0450_2:;
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&global::_003CModule_003E.Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder);
			throw;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out AutoSINSTANCECloser autoSINSTANCECloser);
		try
		{
			try
			{
				try
				{
					SINSTANCE* ptr16 = null;
					int num5 = global::_003CModule_003E.SIMEX_open(ptr15, 0L, num4, &ptr16);
					Marshal.FreeHGlobal(hglobal2);
					hglobal2 = IntPtr.Zero;
					global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINSTANCECloser_002E_007Bctor_007D(&autoSINSTANCECloser, ptr16);
					try
					{
						if (num5 > 0 && ptr16 != null)
						{
							string text = declaration.CustomDataPath + TempFilenameSuffix;
							AutoCleanUpTempFiles autoCleanUpTempFiles2 = new AutoCleanUpTempFiles(text);
							try
							{
								autoCleanUpTempFiles = autoCleanUpTempFiles2;
								IntPtr hglobal3 = Marshal.StringToHGlobalAnsi(text);
								void* intPtr = hglobal3.ToPointer();
								SINSTANCE* ptr17 = null;
								if (global::_003CModule_003E.SIMEX_create((sbyte*)intPtr, 39, &ptr17) == 0)
								{
									Marshal.FreeHGlobal(hglobal3);
									hglobal3 = IntPtr.Zero;
									throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Internal error preparing audio output file \"{0}\" (SIMEX_create(): {1}).", hglobal3, GetLastSIMEXError());
								}
								Marshal.FreeHGlobal(hglobal3);
								hglobal3 = IntPtr.Zero;
								System.Runtime.CompilerServices.Unsafe.SkipInit(out AutoSINSTANCEWCloser autoSINSTANCEWCloser);
								global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINSTANCEWCloser_002E_007Bctor_007D(&autoSINSTANCEWCloser, ptr17);
								try
								{
									int num6 = 0;
									System.Runtime.CompilerServices.Unsafe.SkipInit(out AutoSINFOFreer autoSINFOFreer);
									if (0 < num5)
									{
										do
										{
											SINFO* ptr18 = null;
											int num9;
											if (global::_003CModule_003E.SIMEX_info(ptr16, &ptr18, num6) != 0)
											{
												global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINFOFreer_002E_007Bctor_007D(&autoSINFOFreer, ptr18);
												try
												{
													if (ptr18 != null)
													{
														if (global::_003CModule_003E.SIMEX_read(ptr16, ptr18, num6) == 0)
														{
															goto IL_0899;
														}
														object[] args;
														if (flag3)
														{
															*(short*)(*(int*)ptr18 + 518) = 4096;
															args = new object[0];
															m_Tracer.TraceNote("Setting play location to streamed", args);
														}
														else
														{
															*(short*)(*(int*)ptr18 + 518) = 2048;
															args = new object[0];
															m_Tracer.TraceNote("Setting play location to RAM", args);
														}
														*(sbyte*)(*(int*)ptr18 + 501) = (sbyte)num3;
														args = new object[1]
														{
															new string(global::_003CModule_003E.SIMEX_getsamplerepname(num3))
														};
														m_Tracer.TraceNote("Setting compression type to {0}. ", args);
														if (num3 == 31 || num3 == 28)
														{
															if (num < 0 || num > 100)
															{
																goto IL_097e;
															}
															*(int*)(*(int*)ptr18 + 528) = num;
															args = new object[1] { num };
															m_Tracer.TraceNote("Setting compression quality to {0}. ", args);
														}
														if (ptr7 != null)
														{
															int num7 = *(int*)(*(int*)ptr18 + 532);
															int num8 = *ptr7;
															if (num7 != num8)
															{
																num9 = num8;
																if (num9 < 400 || num9 > 96000)
																{
																	goto IL_0912;
																}
																args = new object[2]
																{
																	num7,
																	*ptr7
																};
																m_Tracer.TraceNote("Resampling from {0} to {1} ", args);
																SIMEXFILTERABOUT* ptr19 = global::_003CModule_003E.SIMEX_filterabout(120);
																uint num10 = ((uint*)ptr19)[3];
																SIMEXFILTERPARAM* ptr20 = (SIMEXFILTERPARAM*)global::_003CModule_003E.new_005B_005D((num10 > 76695844) ? uint.MaxValue : (num10 * 56));
																num8 = 0;
																if (0 < ((int*)ptr19)[3])
																{
																	SIMEXFILTERABOUT* ptr21 = (SIMEXFILTERABOUT*)((byte*)ptr19 + 20);
																	SIMEXFILTERPARAM* ptr22 = (SIMEXFILTERPARAM*)((byte*)ptr20 + 16);
																	do
																	{
																		*((int*)ptr22 - 4) = num9;
																		num10 = *(uint*)(*(int*)ptr21 + 32);
																		if (num10 != 0)
																		{
																			int num11 = *(int*)(int)num10;
																			SIMEXFILTERPARAM* ptr23 = ptr22;
																			sbyte b;
																			do
																			{
																				b = (*(sbyte*)ptr23 = *(sbyte*)num11);
																				num11++;
																				ptr23 = (SIMEXFILTERPARAM*)((byte*)ptr23 + 1);
																			}
																			while (b != 0);
																		}
																		else
																		{
																			*(sbyte*)ptr22 = 0;
																		}
																		((int*)ptr22)[7] = 0;
																		num8++;
																		ptr22 = (SIMEXFILTERPARAM*)((byte*)ptr22 + 56);
																	}
																	while (num8 < ((int*)ptr19)[3]);
																}
																if (global::_003CModule_003E.SIMEX_filterssound((SSOUND*)(int)(*(uint*)ptr18), 120, ptr20) <= 0)
																{
																	goto IL_08d6;
																}
																global::_003CModule_003E.delete(ptr20);
																int num12 = *(int*)(*(int*)ptr18 + 532);
																if (num9 != num12)
																{
																	Console.WriteLine("Warning: Downsampling of audio file {0} not completely sucessful. Wanted final sample of {1}Hz but got {2}Hz", declaration, num9, num12);
																}
															}
														}
														if (*(int*)ptr18 != 0)
														{
															*(byte*)ptr10 = (byte)(*(sbyte*)(*(int*)ptr18 + 502));
															*(int*)ptr11 = *(int*)(*(int*)ptr18 + 536);
															*(int*)ptr12 = *(int*)(*(int*)ptr18 + 532);
															if (*(byte*)ptr10 != 1)
															{
																byte b2 = *(byte*)ptr10;
																if (b2 != 2 && b2 != 4 && b2 != 6)
																{
																	Console.WriteLine("Warning: Audio file {0} has {1} channels. The only supported channel counts are 1, 2, 4, and 6; sample will probably use only the first channel in the engine", declaration, b2);
																}
															}
														}
														if (global::_003CModule_003E.SIMEX_write(ptr17, ptr18, num6) == 0)
														{
															goto IL_093c;
														}
													}
												}
												catch
												{
													//try-fault
													global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<AutoSINFOFreer*, void>)(&global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINFOFreer_002E_007Bdtor_007D), &autoSINFOFreer);
													throw;
												}
												global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINFOFreer_002E_007Bdtor_007D(&autoSINFOFreer);
												num6++;
												continue;
											}
											throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Internal error reading element {0} of \"{1}\" (SIMEX_info(): {2}).", num6, value, GetLastSIMEXError());
											IL_097e:
											try
											{
												throw new BinaryAssetBuilderException(ErrorCode.InvalidArgument, "Audio file {0}: Quality parameter must be between 0 and 100", declaration);
											}
											catch
											{
												//try-fault
												global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<AutoSINFOFreer*, void>)(&global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINFOFreer_002E_007Bdtor_007D), &autoSINFOFreer);
												throw;
											}
											IL_0899:
											try
											{
												throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Internal error reading element {0} of \"{1}\" (SIMEX_read(): {2}).", num6, value, GetLastSIMEXError());
											}
											catch
											{
												//try-fault
												global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<AutoSINFOFreer*, void>)(&global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINFOFreer_002E_007Bdtor_007D), &autoSINFOFreer);
												throw;
											}
											IL_08d6:
											try
											{
												throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Internal error resampling sound {0} to {1} Hz. (SIMEX_filterssound(): {2}).", declaration, num9, GetLastSIMEXError());
											}
											catch
											{
												//try-fault
												global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<AutoSINFOFreer*, void>)(&global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINFOFreer_002E_007Bdtor_007D), &autoSINFOFreer);
												throw;
											}
											IL_093c:
											try
											{
												throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Internal error writing element {0} of \"{1}\" (SIMEX_write(): {2}).", num6, hglobal3, GetLastSIMEXError());
											}
											catch
											{
												//try-fault
												global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<AutoSINFOFreer*, void>)(&global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINFOFreer_002E_007Bdtor_007D), &autoSINFOFreer);
												throw;
											}
											IL_0912:
											try
											{
												throw new BinaryAssetBuilderException(ErrorCode.InvalidArgument, "Audio file {0}: Sample rate must be between 400 and 96000", declaration);
											}
											catch
											{
												//try-fault
												global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<AutoSINFOFreer*, void>)(&global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINFOFreer_002E_007Bdtor_007D), &autoSINFOFreer);
												throw;
											}
										}
										while (num6 < num5);
									}
									string text2;
									STDSTREAM* ptr24;
									System.Runtime.CompilerServices.Unsafe.SkipInit(out AutoGSTREAMCloser autoGSTREAMCloser);
									try
									{
										text2 = text + ".snr";
										if (flag3)
										{
											string sourceFileName = text + ".sns";
											if (File.Exists(declaration.CustomDataPath))
											{
												File.Delete(declaration.CustomDataPath);
											}
											object[] args2 = new object[1] { declaration.CustomDataPath };
											m_Tracer.TraceNote("Creating output file {0}\n", args2);
											File.Move(sourceFileName, declaration.CustomDataPath);
											IntPtr hglobal4 = Marshal.StringToHGlobalAnsi(text2);
											ptr24 = global::_003CModule_003E.gopen((sbyte*)hglobal4.ToPointer());
											Marshal.FreeHGlobal(hglobal4);
											hglobal4 = IntPtr.Zero;
											global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoGSTREAMCloser_002E_007Bctor_007D(&autoGSTREAMCloser, ptr24);
											goto IL_0a4f;
										}
									}
									catch
									{
										//try-fault
										global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<AutoSINFOFreer*, void>)(&global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINFOFreer_002E_007Bdtor_007D), &autoSINFOFreer);
										throw;
									}
									try
									{
										if (File.Exists(declaration.CustomDataPath))
										{
											File.Delete(declaration.CustomDataPath);
										}
										object[] args2 = new object[1] { declaration.CustomDataPath };
										m_Tracer.TraceNote("Creating output file {0}\n", args2);
										File.Move(text2, declaration.CustomDataPath);
									}
									catch
									{
										//try-fault
										global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<AutoSINFOFreer*, void>)(&global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINFOFreer_002E_007Bdtor_007D), &autoSINFOFreer);
										throw;
									}
									goto IL_0aef;
									IL_0a4f:
									try
									{
										uint num13 = (uint)global::_003CModule_003E.glen(ptr24);
										global::_003CModule_003E.Relo_002ETracker_002EPush_003Cchar_003E(&tracker2, (sbyte**)ptr13, num13);
										global::_003CModule_003E.gread(ptr24, (void*)(int)(*(uint*)ptr13), (int)num13);
										*(uint*)ptr14 = num13;
										global::_003CModule_003E.Relo_002ETracker_002EPop(&tracker2);
									}
									catch
									{
										//try-fault
										global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<AutoGSTREAMCloser*, void>)(&global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoGSTREAMCloser_002E_007Bdtor_007D), &autoGSTREAMCloser);
										throw;
									}
									global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoGSTREAMCloser_002E_007Bdtor_007D(&autoGSTREAMCloser);
									goto IL_0aef;
									IL_0aef:
									try
									{
										global::_003CModule_003E.Marshaler_002EEndianCorrect(ptr5, &endianHelper);
										FinalizeTracker(&tracker2, &endianHelper, assetBuffer);
									}
									catch
									{
										//try-fault
										global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<AutoSINFOFreer*, void>)(&global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINFOFreer_002E_007Bdtor_007D), &autoSINFOFreer);
										throw;
									}
								}
								catch
								{
									//try-fault
									global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<AutoSINSTANCEWCloser*, void>)(&global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINSTANCEWCloser_002E_007Bdtor_007D), &autoSINSTANCEWCloser);
									throw;
								}
								global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINSTANCEWCloser_002E_007Bdtor_007D(&autoSINSTANCEWCloser);
							}
							catch
							{
								//try-fault
								((IDisposable)autoCleanUpTempFiles).Dispose();
								throw;
							}
							((IDisposable)autoCleanUpTempFiles).Dispose();
							goto IL_0b4c;
						}
					}
					catch
					{
						//try-fault
						global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<AutoSINSTANCECloser*, void>)(&global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINSTANCECloser_002E_007Bdtor_007D), &autoSINSTANCECloser);
						throw;
					}
					goto end_IL_04b5;
					IL_0b4c:
					global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINSTANCECloser_002E_007Bdtor_007D(&autoSINSTANCECloser);
					goto IL_0b63;
					end_IL_04b5:;
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker2);
					throw;
				}
				goto end_IL_04b5_2;
				IL_0b63:
				global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker2);
				goto IL_0b7a;
				end_IL_04b5_2:;
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker);
				throw;
			}
			goto end_IL_04b5_3;
			IL_0b7a:
			global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker);
			goto IL_0b91;
			end_IL_04b5_3:;
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&global::_003CModule_003E.Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder);
			throw;
		}
		AssetBuffer result2;
		try
		{
			try
			{
				try
				{
					try
					{
						Console.WriteLine("Warning: Could not begin audio processing of \"{0}\": {1}.", value, GetLastSIMEXError());
						result2 = null;
					}
					catch
					{
						//try-fault
						global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<AutoSINSTANCECloser*, void>)(&global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINSTANCECloser_002E_007Bdtor_007D), &autoSINSTANCECloser);
						throw;
					}
					global::_003CModule_003E.BinaryAssetBuilder_002EAudioCompiler_002EAutoSINSTANCECloser_002E_007Bdtor_007D(&autoSINSTANCECloser);
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker2);
					throw;
				}
				global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker2);
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker);
				throw;
			}
			global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker);
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&global::_003CModule_003E.Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder);
			throw;
		}
		global::_003CModule_003E.Marshaler_002ENodeHolder_002E_007Bdtor_007D(&nodeHolder);
		return result2;
		IL_04aa:
		global::_003CModule_003E.Marshaler_002ENodeHolder_002E_007Bdtor_007D(&nodeHolder);
		return result;
		IL_0b91:
		global::_003CModule_003E.Marshaler_002ENodeHolder_002E_007Bdtor_007D(&nodeHolder);
		return assetBuffer;
		IL_03d5:
		global::_003CModule_003E.Marshaler_002ENodeHolder_002E_007Bdtor_007D(&nodeHolder);
		return result;
		IL_0445:
		global::_003CModule_003E.Marshaler_002ENodeHolder_002E_007Bdtor_007D(&nodeHolder);
		return result;
	}

	public virtual AssetBuffer ProcessInstance(InstanceDeclaration declaration)
	{
		switch (declaration.Handle.TypeId)
		{
		case 376113229u:
			return ProcessAudioFileInstance(declaration);
		case 364756738u:
			return ProcessAudioFileMP3PassthroughInstance(declaration);
		case 2516466693u:
			return ProcessPathMusicMapInstance(declaration);
		default:
		{
			object[] args = new object[1] { declaration };
			m_Tracer.TraceWarning("Couldn't process {0}. No matching handler found.", args);
			return null;
		}
		case 2802565590u:
			return ProcessPathMusicTrackInstance(declaration);
		case 2590317961u:
			return ProcessPathMusicEventInstance(declaration);
		}
	}

	public virtual uint GetAllTypesHash()
	{
		return 1424942948u;
	}

	public virtual uint GetVersionNumber()
	{
		TargetPlatform platform = m_Platform;
		return platform switch
		{
			TargetPlatform.Xbox360 => 548411652u, 
			TargetPlatform.Win32 => 548405252u, 
			TargetPlatform.PlayStation3 => 548405252u, 
			_ => throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Unknown platform {0}", platform), 
		};
	}

	public unsafe ExtendedTypeInformation GetPathMusicExtendedTypeInformation(uint typeId)
	{
		ExtendedTypeInformation extendedTypeInformation = new ExtendedTypeInformation();
		extendedTypeInformation.TypeId = typeId;
		TargetPlatform platform = m_Platform;
		if (platform != TargetPlatform.Xbox360 && platform != TargetPlatform.Win32 && platform != TargetPlatform.PlayStation3)
		{
			throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Unknown platform {0}", platform);
		}
		switch (typeId)
		{
		case 2802565590u:
			extendedTypeInformation.ProcessingHash = 2668810042u;
			extendedTypeInformation.HasCustomData = true;
			extendedTypeInformation.TypeHash = 2668810035u;
			extendedTypeInformation.TypeName = new string(global::_003CModule_003E._003FTypeName_0040PathMusicTrack_0040SageBinaryData_0040_00402PBDB);
			break;
		case 2590317961u:
			extendedTypeInformation.ProcessingHash = 1993395942u;
			extendedTypeInformation.HasCustomData = false;
			extendedTypeInformation.TypeHash = 1993395951u;
			extendedTypeInformation.TypeName = new string(global::_003CModule_003E._003FTypeName_0040PathMusicEvent_0040SageBinaryData_0040_00402PBDB);
			break;
		case 2516466693u:
			extendedTypeInformation.ProcessingHash = 290548117u;
			extendedTypeInformation.HasCustomData = false;
			extendedTypeInformation.TypeHash = 3122351307u;
			extendedTypeInformation.TypeName = new string(global::_003CModule_003E._003FTypeName_0040PathMusicMap_0040SageBinaryData_0040_00402PBDB);
			break;
		}
		return extendedTypeInformation;
	}

	public unsafe ExtendedTypeInformation GetAudioFileExtendedTypeInformation(uint typeId)
	{
		ExtendedTypeInformation extendedTypeInformation = new ExtendedTypeInformation();
		extendedTypeInformation.TypeId = typeId;
		switch (typeId)
		{
		case 376113229u:
			switch (m_Platform)
			{
			case TargetPlatform.Xbox360:
				extendedTypeInformation.ProcessingHash = 2414316422u;
				break;
			case TargetPlatform.Win32:
				extendedTypeInformation.ProcessingHash = 2414318214u;
				break;
			case TargetPlatform.PlayStation3:
				extendedTypeInformation.ProcessingHash = 2414318214u;
				break;
			}
			extendedTypeInformation.HasCustomData = true;
			extendedTypeInformation.TypeHash = 1405623879u;
			extendedTypeInformation.TypeName = new string(global::_003CModule_003E._003FTypeName_0040AudioFile_0040SageBinaryData_0040_00402PBDB);
			break;
		case 364756738u:
			switch (m_Platform)
			{
			case TargetPlatform.Xbox360:
				extendedTypeInformation.ProcessingHash = 2822643353u;
				break;
			case TargetPlatform.Win32:
				extendedTypeInformation.ProcessingHash = 2822637465u;
				break;
			}
			extendedTypeInformation.HasCustomData = true;
			extendedTypeInformation.TypeHash = 2457571466u;
			extendedTypeInformation.TypeName = new string(global::_003CModule_003E._003FTypeName_0040AudioFileMP3Passthrough_0040SageBinaryData_0040_00402PBDB);
			break;
		}
		return extendedTypeInformation;
	}

	public virtual ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
	{
		switch (typeId)
		{
		default:
			return new ExtendedTypeInformation();
		case 364756738u:
		case 376113229u:
			return GetAudioFileExtendedTypeInformation(typeId);
		case 2516466693u:
		case 2590317961u:
		case 2802565590u:
			return GetPathMusicExtendedTypeInformation(typeId);
		}
	}

	protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool P_0)
	{
		if (P_0)
		{
			_0021Plugin();
			return;
		}
		try
		{
			_0021Plugin();
		}
		finally
		{
			base.Finalize();
		}
	}

	public virtual sealed void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	~Plugin()
	{
		Dispose(false);
	}
}
