using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AssetStream;
using BinaryAssetBuilder.Core;
using Endian;
using Marshaler;
using Relo;
using SageBinaryData;
using eastl;

namespace BinaryAssetBuilder.TextureCompiler;

public class Plugin : IAssetBuilderPlugin, IDisposable
{
	private uint _HashCode;

	private TargetPlatform _Platform;

	private static uint TextureAtlasPluginVersion_PC = 1802u;

	private static uint TextureAtlasPluginVersion_Xenon = 1852u;

	private static uint TextureAtlasPluginVersion_PS3 = 1907u;

	private static uint TexturePluginVersion_PC = 4202u;

	private static uint TexturePluginVersion_Xenon = 4605u;

	private static uint TexturePluginVersion_PS3 = 4710u;

	private static Tracer _Tracer = Tracer.GetTracer("TextureCompiler", "Provides Texture Processing");

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool IsValidTexture(gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* declaration, nvImage_003Crgba_t_003E* image)
	{
		uint num;
		uint num2;
		uint num3;
		try
		{
			num = ((uint*)image)[1];
			num2 = *(uint*)image;
			num3 = num2 * num;
			if (num3 == 0)
			{
				object[] array = new object[1];
				IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)declaration));
				InstanceDeclaration target = (InstanceDeclaration)((GCHandle)intPtr).Target;
				array[0] = target.Handle.InstanceName;
				_Tracer.TraceWarning("The loaded image contains no pixel data. {0} will be pink.", array);
				goto IL_006b;
			}
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D), declaration);
			throw;
		}
		try
		{
			if (num3 << 2 >= 67108864)
			{
				object[] array2 = new object[1];
				IntPtr intPtr2 = new IntPtr((void*)(int)(*(uint*)declaration));
				InstanceDeclaration target2 = (InstanceDeclaration)((GCHandle)intPtr2).Target;
				array2[0] = target2.Handle.InstanceName;
				_Tracer.TraceWarning("The loaded image {0} is larger than the compression buffer, which is unsafe. {0} will be pink.", array2);
				goto IL_00e7;
			}
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D), declaration);
			throw;
		}
		try
		{
			uint num4 = num2;
			if (num4 != 0 && ((~num4 + 1) & num4) == num4)
			{
				uint num5 = num;
				if (num5 != 0 && ((~num5 + 1) & num5) == num5)
				{
					goto IL_0143;
				}
			}
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D), declaration);
			throw;
		}
		try
		{
			object[] array3 = new object[1];
			IntPtr intPtr3 = new IntPtr((void*)(int)(*(uint*)declaration));
			InstanceDeclaration target3 = (InstanceDeclaration)((GCHandle)intPtr3).Target;
			array3[0] = target3.Handle.InstanceName;
			_Tracer.TraceWarning("Non power of two sized textures not supported. {0} will be pink.", array3);
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D), declaration);
			throw;
		}
		IntPtr intPtr4 = new IntPtr((void*)(int)(*(uint*)declaration));
		((GCHandle)intPtr4).Free();
		*(int*)declaration = 0;
		return false;
		IL_006b:
		IntPtr intPtr5 = new IntPtr((void*)(int)(*(uint*)declaration));
		((GCHandle)intPtr5).Free();
		*(int*)declaration = 0;
		return false;
		IL_0143:
		global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D(declaration);
		return true;
		IL_00e7:
		IntPtr intPtr6 = new IntPtr((void*)(int)(*(uint*)declaration));
		((GCHandle)intPtr6).Free();
		*(int*)declaration = 0;
		return false;
	}

	private unsafe string GuessProperFormat(gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* declaration, [MarshalAs(UnmanagedType.U1)] bool hasAlpha, nvImage_003Crgba_t_003E* image, string inFormat)
	{
		try
		{
			if (inFormat == "D3DFMT_DXT1" && hasAlpha)
			{
				if (!global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002EHasConstantAlphaChannel(image))
				{
					goto IL_00b2;
				}
				if (*(byte*)(((int*)image)[2] + 3) != byte.MaxValue)
				{
					goto IL_0051;
				}
				goto IL_0047;
			}
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D), declaration);
			throw;
		}
		IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)declaration));
		((GCHandle)intPtr).Free();
		*(int*)declaration = 0;
		return inFormat;
		IL_0047:
		global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D(declaration);
		return inFormat;
		IL_0051:
		string result;
		try
		{
			object[] array = new object[1];
			IntPtr intPtr2 = new IntPtr((void*)(int)(*(uint*)declaration));
			InstanceDeclaration target = (InstanceDeclaration)((GCHandle)intPtr2).Target;
			array[0] = target.Handle.InstanceName;
			_Tracer.TraceInfo("Converting to DXT3 because \"{0}\" contains a constant color alpha channel.  DXT3 should be specified in the XML, or the alpha channel should be removed.", array);
			result = "D3DFMT_DXT3";
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D), declaration);
			throw;
		}
		global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D(declaration);
		return result;
		IL_0113:
		global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D(declaration);
		string result2;
		return result2;
		IL_00b2:
		try
		{
			if (global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002EHas1BitAlphaChannel(image))
			{
				object[] array2 = new object[1];
				IntPtr intPtr3 = new IntPtr((void*)(int)(*(uint*)declaration));
				InstanceDeclaration target2 = (InstanceDeclaration)((GCHandle)intPtr3).Target;
				array2[0] = target2.Handle.InstanceName;
				_Tracer.TraceInfo("Converting to DXT3 because \"{0}\" has a 1 bit alpha channel.  DXT3 should be specified in the XML, or the alpha channel should be removed.", array2);
				result2 = "D3DFMT_DXT3";
				goto IL_0113;
			}
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D), declaration);
			throw;
		}
		string result3;
		try
		{
			object[] array3 = new object[1];
			IntPtr intPtr4 = new IntPtr((void*)(int)(*(uint*)declaration));
			InstanceDeclaration target3 = (InstanceDeclaration)((GCHandle)intPtr4).Target;
			array3[0] = target3.Handle.InstanceName;
			_Tracer.TraceInfo("Converting to DXT5 because \"{0}\" does not have a 1-bit alpha channel.  DXT5 should be specified in the XML, or the alpha channel should be removed.", array3);
			result3 = "D3DFMT_DXT5";
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D), declaration);
			throw;
		}
		global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D(declaration);
		return result3;
	}

	private unsafe nvCompressionOptions* GetNVCompressionOptions(nvCompressionOptions* P_0, string format, [MarshalAs(UnmanagedType.U1)] bool generateMipMaps)
	{
		global::_003CModule_003E.nvCompressionOptions_002E_007Bctor_007D(P_0);
		switch (format)
		{
		case "D3DFMT_A8R8G8B8":
			((int*)P_0)[119] = 7;
			break;
		case "D3DFMT_DXT5":
			((int*)P_0)[119] = 3;
			break;
		case "D3DFMT_DXT3":
			((int*)P_0)[119] = 2;
			break;
		case "D3DFMT_DXT1A":
			((int*)P_0)[119] = 1;
			break;
		case "D3DFMT_X8R8G8B8":
			((int*)P_0)[119] = 25;
			break;
		case "D3DFMT_R5G6B5":
			((int*)P_0)[119] = 6;
			break;
		default:
			((int*)P_0)[119] = 0;
			break;
		}
		if (generateMipMaps)
		{
			if (_Platform == TargetPlatform.PlayStation3)
			{
				((int*)P_0)[25] = 30;
				((int*)P_0)[26] = 3;
			}
			else
			{
				((int*)P_0)[25] = 30;
				((int*)P_0)[26] = 0;
			}
		}
		else
		{
			((int*)P_0)[25] = 32;
		}
		return P_0;
	}

	private void _007EPlugin()
	{
		_0021Plugin();
	}

	private unsafe void _0021Plugin()
	{
		if (global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_pD3DDevice != null)
		{
			global::_003CModule_003E.D3DDevice_002ERelease(global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_pD3DDevice);
		}
		if (global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_pD3D != null)
		{
			global::_003CModule_003E.Direct3D_002ERelease();
		}
		if (global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Bundler != null)
		{
			global::_003CModule_003E.XenonBundler_002E__delDtor(global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Bundler, 1u);
		}
		if (global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Buffer != null)
		{
			global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002ESimpleBuffer_002E__delDtor(global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Buffer, 1u);
		}
		global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_pD3DDevice = null;
		global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_pD3D = null;
	}

	public unsafe virtual void Initialize(object configSection, TargetPlatform platform)
	{
		_HashCode = HashProvider.GetTypeHash(GetType());
		_Platform = platform;
		global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_pD3D = global::_003CModule_003E._003FA0x749b4e39_002EDirect3DCreate9(0u);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _D3DPRESENT_PARAMETERS_ d3DPRESENT_PARAMETERS_);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlock(ref d3DPRESENT_PARAMETERS_, 0, 124);
		*(int*)(&d3DPRESENT_PARAMETERS_) = 640;
		System.Runtime.CompilerServices.Unsafe.As<_D3DPRESENT_PARAMETERS_, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref d3DPRESENT_PARAMETERS_, 4)) = 480;
		System.Runtime.CompilerServices.Unsafe.As<_D3DPRESENT_PARAMETERS_, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref d3DPRESENT_PARAMETERS_, 8)) = 405275014;
		System.Runtime.CompilerServices.Unsafe.As<_D3DPRESENT_PARAMETERS_, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref d3DPRESENT_PARAMETERS_, 16)) = 0;
		System.Runtime.CompilerServices.Unsafe.As<_D3DPRESENT_PARAMETERS_, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref d3DPRESENT_PARAMETERS_, 20)) = 0;
		System.Runtime.CompilerServices.Unsafe.As<_D3DPRESENT_PARAMETERS_, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref d3DPRESENT_PARAMETERS_, 12)) = 1;
		System.Runtime.CompilerServices.Unsafe.As<_D3DPRESENT_PARAMETERS_, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref d3DPRESENT_PARAMETERS_, 36)) = 1;
		System.Runtime.CompilerServices.Unsafe.As<_D3DPRESENT_PARAMETERS_, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref d3DPRESENT_PARAMETERS_, 40)) = 757072278;
		System.Runtime.CompilerServices.Unsafe.As<_D3DPRESENT_PARAMETERS_, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref d3DPRESENT_PARAMETERS_, 24)) = 1;
		System.Runtime.CompilerServices.Unsafe.As<_D3DPRESENT_PARAMETERS_, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref d3DPRESENT_PARAMETERS_, 52)) = 1;
		global::_003CModule_003E.Direct3D_002ECreateDevice(0u, (_D3DDEVTYPE)1, null, 0u, &d3DPRESENT_PARAMETERS_, (D3DDevice**)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_pD3DDevice));
		XenonBundler* ptr = (XenonBundler*)global::_003CModule_003E.@new(24u);
		XenonBundler* binaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Bundler;
		try
		{
			binaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Bundler = ((ptr == null) ? null : global::_003CModule_003E.XenonBundler_002E_007Bctor_007D(ptr, 102400u, 1048576u));
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.delete(ptr);
			throw;
		}
		global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Bundler = binaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Bundler;
		SimpleBuffer* ptr2 = (SimpleBuffer*)global::_003CModule_003E.@new(12u);
		SimpleBuffer* binaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Buffer;
		try
		{
			binaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Buffer = ((ptr2 == null) ? null : global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002ESimpleBuffer_002E_007Bctor_007D(ptr2, 67108864u));
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.delete(ptr2);
			throw;
		}
		global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Buffer = binaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Buffer;
	}

	public virtual void ReInitialize(object configSection, TargetPlatform platform)
	{
		_HashCode = HashProvider.GetTypeHash(GetType());
		_Platform = platform;
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
			fixed (byte* ptr = &buffer.InstanceData[0])
			{
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr, *(int*)(&chunk), System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4)));
				if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 12)) != 0)
				{
					buffer.RelocationData = new byte[System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 12))];
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
				else
				{
					buffer.RelocationData = new byte[0];
				}
				buffer.ImportsData = new byte[0];
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

	public unsafe void CompressAndEmbed(Tracker* tracker, TextureFileData* textureData, nvImage_003Crgba_t_003E* image, nvCompressionOptions* options)
	{
		global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002ESimpleBuffer_002EReset(global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Buffer);
		((int*)options)[410] = (int)global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Buffer;
		NV_ERROR_CODE nV_ERROR_CODE = global::_003CModule_003E.nvDDS_002EnvDXTcompress(image, options, (delegate* unmanaged[Cdecl, Cdecl]<void*, uint, MIPMapData*, void*, NV_ERROR_CODE>)global::_003CModule_003E.__unep_0040_003FnvDXTWriteCallback_0040TextureCompiler_0040BinaryAssetBuilder_0040_0040_0024_0024FYA_003FAW4NV_ERROR_CODE_0040_0040PBXIPBUMIPMapData_0040_0040PAX_0040Z, null);
		if (nV_ERROR_CODE != 0)
		{
			IntPtr ptr = new IntPtr(global::_003CModule_003E.getErrorString(nV_ERROR_CODE));
			string text = Marshal.PtrToStringAnsi(ptr);
			object[] args = new object[0];
			throw new BinaryAssetBuilderException(null, ErrorCode.InternalError, "nvDXT Internal Error : " + text, args);
		}
		if (((int*)global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Buffer)[1] == 0)
		{
			object[] args2 = new object[0];
			throw new BinaryAssetBuilderException(null, ErrorCode.BadTextureFile, "Texture data has length 0", args2);
		}
		if (_Platform == TargetPlatform.PlayStation3)
		{
			EmbedPS3Texture(tracker, textureData, global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Buffer);
		}
		else
		{
			EmbedPCTexture(tracker, textureData, global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Buffer);
		}
	}

	public unsafe void CompressAndEmbed3D(Tracker* tracker, TextureFileData* textureData, nvImage_003Crgba_t_003E* image, nvCompressionOptions* options)
	{
		global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002ESimpleBuffer_002EReset(global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Buffer);
		((int*)options)[410] = (int)global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Buffer;
		((int*)options)[118] = 2;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out RGBAMipMappedVolumeMap rGBAMipMappedVolumeMap);
		*(int*)(&rGBAMipMappedVolumeMap) = 0;
		System.Runtime.CompilerServices.Unsafe.As<RGBAMipMappedVolumeMap, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rGBAMipMappedVolumeMap, 4)) = 0;
		System.Runtime.CompilerServices.Unsafe.As<RGBAMipMappedVolumeMap, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rGBAMipMappedVolumeMap, 8)) = 0;
		try
		{
			global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002ECreateVolumeRGBAImage(&rGBAMipMappedVolumeMap, image);
			NV_ERROR_CODE nV_ERROR_CODE = global::_003CModule_003E.nvDDS_002EnvDXTcompress(&rGBAMipMappedVolumeMap, options, (delegate* unmanaged[Cdecl, Cdecl]<void*, uint, MIPMapData*, void*, NV_ERROR_CODE>)global::_003CModule_003E.__unep_0040_003FnvDXTWriteCallback_0040TextureCompiler_0040BinaryAssetBuilder_0040_0040_0024_0024FYA_003FAW4NV_ERROR_CODE_0040_0040PBXIPBUMIPMapData_0040_0040PAX_0040Z, null);
			if (nV_ERROR_CODE != 0)
			{
				IntPtr ptr = new IntPtr(global::_003CModule_003E.getErrorString(nV_ERROR_CODE));
				string text = Marshal.PtrToStringAnsi(ptr);
				object[] args = new object[0];
				throw new BinaryAssetBuilderException(null, ErrorCode.InternalError, "nvDXT Internal Error : " + text, args);
			}
			if (((int*)global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Buffer)[1] == 0)
			{
				object[] args2 = new object[0];
				throw new BinaryAssetBuilderException(null, ErrorCode.BadTextureFile, "Texture data has length 0", args2);
			}
			if (_Platform == TargetPlatform.PlayStation3)
			{
				EmbedPS3Texture(tracker, textureData, global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Buffer);
			}
			else
			{
				EmbedPCTexture(tracker, textureData, global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Buffer);
			}
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<RGBAMipMappedVolumeMap*, void>)(&global::_003CModule_003E.RGBAMipMappedVolumeMap_002E_007Bdtor_007D), &rGBAMipMappedVolumeMap);
			throw;
		}
		global::_003CModule_003E.nvVector_003CnvVector_003CnvImage_003Crgba_t_003E_0020_003E_0020_003E_002Eresize((nvVector_003CnvVector_003CnvImage_003Crgba_t_003E_0020_003E_0020_003E*)(&rGBAMipMappedVolumeMap), 0u);
		System.Runtime.CompilerServices.Unsafe.As<RGBAMipMappedVolumeMap, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rGBAMipMappedVolumeMap, 8)) = 0;
		if (*(int*)(&rGBAMipMappedVolumeMap) != 0)
		{
			global::_003CModule_003E.free((void*)(int)(*(uint*)(&rGBAMipMappedVolumeMap)));
		}
	}

	public unsafe void EmbedPCTexture(Tracker* tracker, TextureFileData* textureData, SimpleBuffer* buffer)
	{
		global::_003CModule_003E.Relo_002ETracker_002EPush(tracker, (void**)textureData, 1u, ((uint*)buffer)[1]);
		((int*)textureData)[1] = ((int*)buffer)[1];
		// IL cpblk instruction
		System.Runtime.CompilerServices.Unsafe.CopyBlock(*(int*)textureData, *(int*)buffer, ((int*)buffer)[1]);
		((int*)((byte*)tracker + 4))[1] += -4;
	}

	public unsafe void EmbedPS3Texture(Tracker* tracker, TextureFileData* textureData, SimpleBuffer* buffer)
	{
		global::_003CModule_003E.Relo_002ETracker_002EPush(tracker, (void**)textureData, 1u, ((uint*)buffer)[1]);
		((int*)textureData)[1] = (int)global::_003CModule_003E.ReverseEndianness(((uint*)buffer)[1]);
		Console.WriteLine("EmbedPS3Texture reverse endian");
		// IL cpblk instruction
		System.Runtime.CompilerServices.Unsafe.CopyBlock(*(int*)textureData, *(int*)buffer, ((int*)buffer)[1]);
		((int*)((byte*)tracker + 4))[1] += -4;
	}

	public unsafe void EmbedXenonTexture(Tracker* tracker, TextureFileData* textureData, XenonBundler* theBundler)
	{
		((int*)textureData)[1] = (int)global::_003CModule_003E.ReverseEndianness(16u);
		global::_003CModule_003E.Relo_002ETracker_002EPush(tracker, (void**)textureData, 1u, 16u);
		XenonTextureData* ptr = (XenonTextureData*)(int)(*(uint*)textureData);
		((int*)ptr)[1] = (int)global::_003CModule_003E.ReverseEndianness(((uint*)theBundler)[1]);
		((int*)ptr)[3] = (int)global::_003CModule_003E.ReverseEndianness(((uint*)theBundler)[4]);
		global::_003CModule_003E.Relo_002ETracker_002EPush(tracker, (void**)ptr, 1u, ((uint*)theBundler)[1]);
		// IL cpblk instruction
		System.Runtime.CompilerServices.Unsafe.CopyBlock(*(int*)ptr, *(int*)theBundler, ((int*)theBundler)[1]);
		Tracker* ptr2 = (Tracker*)((byte*)tracker + 4);
		stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr3 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)ptr2;
		((int*)ptr3)[1] += -4;
		XenonTextureData* ptr4 = (XenonTextureData*)((byte*)ptr + 8);
		global::_003CModule_003E.Relo_002ETracker_002EPush(tracker, (void**)ptr4, 1u, ((uint*)theBundler)[4]);
		// IL cpblk instruction
		System.Runtime.CompilerServices.Unsafe.CopyBlock(*(int*)ptr4, ((int*)theBundler)[3], ((int*)theBundler)[4]);
		stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr5 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)ptr2;
		((int*)ptr5)[1] += -4;
		stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr6 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)ptr2;
		((int*)ptr6)[1] += -4;
	}

	public unsafe void ProcessTexture(gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* declaration, TextureFileData* textureData, Tracker* tracker)
	{
		string text = null;
		try
		{
			IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)declaration));
			string value = ((InstanceDeclaration)((GCHandle)intPtr).Target).XmlNode.Attributes["File"].Value;
			bool flag = value.EndsWith(".dds");
			if (!File.Exists(value))
			{
				object[] args = new object[0];
				throw new BinaryAssetBuilderException(null, ErrorCode.BadTextureFile, "The texture file referenced from the XML does not exist on disk!", args);
			}
			((int*)textureData)[1] = 0;
			*(int*)textureData = 0;
			IntPtr intPtr2 = new IntPtr((void*)(int)(*(uint*)declaration));
			switch (((InstanceDeclaration)((GCHandle)intPtr2).Target).Node.Attributes["OutputFormat"].Value)
			{
			case "A8R8G8B8":
				text = "D3DFMT_A8R8G8B8";
				break;
			case "DXT1":
				text = "D3DFMT_DXT1";
				break;
			case "DXT1A":
				text = "D3DFMT_DXT1A";
				break;
			case "DXT3":
				text = "D3DFMT_DXT3";
				break;
			case "DXT5":
				text = "D3DFMT_DXT5";
				break;
			case "X8R8G8B8":
				text = "D3DFMT_X8R8G8B8";
				break;
			case "R5G6B5":
				text = "D3DFMT_R5G6B5";
				break;
			}
			if (_Platform == TargetPlatform.PlayStation3 && text != "D3DFMT_DXT1" && text != "D3DFMT_DXT1A" && text != "D3DFMT_DXT5" && text != "D3DFMT_DXT3")
			{
				Console.WriteLine("ProcessTexture PS3 Texture {0} forceformat {1}.", global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_002EP_0024AAVInstanceDeclaration_0040Core_0040BinaryAssetBuilder_0040_0040(declaration), text);
			}
			sbyte* ptr = (sbyte*)Marshal.StringToHGlobalAnsi(value).ToPointer();
			IntPtr intPtr3 = new IntPtr((void*)(int)(*(uint*)declaration));
			bool flag2 = bool.Parse(((InstanceDeclaration)((GCHandle)intPtr3).Target).Node.Attributes["GenerateMipMaps"].Value);
			IntPtr intPtr4 = new IntPtr((void*)(int)(*(uint*)declaration));
			string value2 = ((InstanceDeclaration)((GCHandle)intPtr4).Target).Node.Attributes["Type"].Value;
			IntPtr intPtr5 = new IntPtr((void*)(int)(*(uint*)declaration));
			bool flag3 = bool.Parse(((InstanceDeclaration)((GCHandle)intPtr5).Target).Node.Attributes["AllowAutomaticResize"].Value);
			if (_Platform == TargetPlatform.Xbox360)
			{
				int num = ((!flag2) ? 1 : 6);
				if (text == "D3DFMT_DXT1A")
				{
					text = "D3DFMT_DXT1";
				}
				if (flag)
				{
					global::_003CModule_003E.XenonBundler_002EProcessDDS(global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Bundler, ptr, global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_pD3DDevice);
				}
				else
				{
					System.Runtime.CompilerServices.Unsafe.SkipInit(out nvImage_003Crgba_t_003E nvImage_003Crgba_t_003E);
					global::_003CModule_003E.nvImage_003Crgba_t_003E_002E_007Bctor_007D(&nvImage_003Crgba_t_003E);
					try
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out bool hasAlpha);
						global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002ELoadImageA(&nvImage_003Crgba_t_003E, &hasAlpha, ptr);
						System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E obj);
						gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* ptr2 = &obj;
						gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* declaration2 = global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bctor_007D(&obj, declaration);
						if (IsValidTexture(declaration2, &nvImage_003Crgba_t_003E))
						{
							System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E obj2);
							gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* ptr3 = &obj2;
							gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* declaration3 = global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bctor_007D(&obj2, declaration);
							text = GuessProperFormat(declaration3, hasAlpha, &nvImage_003Crgba_t_003E, text);
							sbyte* ptr4 = (sbyte*)Marshal.StringToHGlobalAnsi(text).ToPointer();
							switch (value2)
							{
							case "VolumeTexture":
								global::_003CModule_003E.XenonBundler_002EProcess3DTexture(global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Bundler, &nvImage_003Crgba_t_003E, ptr4, (uint)num);
								break;
							case "CubeTexture":
							{
								object[] args3 = new object[0];
								throw new BinaryAssetBuilderException(null, ErrorCode.InvalidArgument, "CubeTexture is not currently supported.  Create the cube texture as a DDS with the D3D Texture tool in the meantime.", args3);
							}
							case "StandardTexture":
								if (flag3)
								{
									uint num2 = *(uint*)(&nvImage_003Crgba_t_003E);
									uint num3 = System.Runtime.CompilerServices.Unsafe.As<nvImage_003Crgba_t_003E, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nvImage_003Crgba_t_003E, 4));
									if ((uint)(*(int*)(&nvImage_003Crgba_t_003E)) > 256u || (uint)System.Runtime.CompilerServices.Unsafe.As<nvImage_003Crgba_t_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nvImage_003Crgba_t_003E, 4)) > 256u)
									{
										while (num2 > 256 || num3 > 256)
										{
											num2 >>= 1;
											num3 >>= 1;
										}
										object[] args2 = new object[4]
										{
											*(uint*)(&nvImage_003Crgba_t_003E),
											System.Runtime.CompilerServices.Unsafe.As<nvImage_003Crgba_t_003E, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nvImage_003Crgba_t_003E, 4)),
											num2,
											num3
										};
										_Tracer.TraceInfo("Resizing the image from {0}x{1} to {2}x{3} on the Xenon.  Specify the 'AllowAutomaticResize' field in the texture XML to override this behavior.", args2);
									}
								}
								global::_003CModule_003E.XenonBundler_002EProcess2DTexture(global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Bundler, &nvImage_003Crgba_t_003E, ptr4, flag3, (uint)num);
								break;
							}
						}
					}
					catch
					{
						//try-fault
						global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<nvImage_003Crgba_t_003E*, void>)(&global::_003CModule_003E.nvImage_003Crgba_t_003E_002E_007Bdtor_007D), &nvImage_003Crgba_t_003E);
						throw;
					}
					global::_003CModule_003E.nvImage_003Crgba_t_003E_002E_007Bdtor_007D(&nvImage_003Crgba_t_003E);
				}
				EmbedXenonTexture(tracker, textureData, global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Bundler);
			}
			else if (flag)
			{
				FileStream fileStream = File.OpenRead(value);
				if (fileStream.Length == 0)
				{
					object[] args4 = new object[0];
					throw new BinaryAssetBuilderException(null, ErrorCode.BadTextureFile, "Texture file has length 0", args4);
				}
				global::_003CModule_003E.Relo_002ETracker_002EPush_003Cchar_003E(tracker, (sbyte**)textureData, (uint)(((int*)textureData)[1] = (int)fileStream.Length));
				uint num4 = ((uint*)textureData)[1];
				byte[] array = new byte[num4];
				fileStream.Read(array, 0, (int)num4);
				fixed (byte* ptr5 = &array[0])
				{
					try
					{
						// IL cpblk instruction
						System.Runtime.CompilerServices.Unsafe.CopyBlock(*(int*)textureData, ptr5, ((int*)textureData)[1]);
						if (_Platform == TargetPlatform.PlayStation3)
						{
							((int*)textureData)[1] = (int)global::_003CModule_003E.ReverseEndianness(((uint*)textureData)[1]);
							Console.WriteLine("is DDS PS3 reverse endian");
						}
						fileStream.Close();
						global::_003CModule_003E.Relo_002ETracker_002EPop(tracker);
					}
					catch
					{
						//try-fault
						ptr5 = null;
						throw;
					}
				}
			}
			else
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out nvImage_003Crgba_t_003E nvImage_003Crgba_t_003E2);
				global::_003CModule_003E.nvImage_003Crgba_t_003E_002E_007Bctor_007D(&nvImage_003Crgba_t_003E2);
				try
				{
					System.Runtime.CompilerServices.Unsafe.SkipInit(out bool hasAlpha2);
					global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002ELoadImageA(&nvImage_003Crgba_t_003E2, &hasAlpha2, ptr);
					System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E obj3);
					gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* ptr6 = &obj3;
					gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* declaration4 = global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bctor_007D(&obj3, declaration);
					if (IsValidTexture(declaration4, &nvImage_003Crgba_t_003E2))
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E obj4);
						gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* ptr7 = &obj4;
						gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* declaration5 = global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bctor_007D(&obj4, declaration);
						text = GuessProperFormat(declaration5, hasAlpha2, &nvImage_003Crgba_t_003E2, text);
						System.Runtime.CompilerServices.Unsafe.SkipInit(out nvCompressionOptions nvCompressionOptions);
						GetNVCompressionOptions(&nvCompressionOptions, text, flag2);
						switch (value2)
						{
						case "VolumeTexture":
							CompressAndEmbed3D(tracker, textureData, &nvImage_003Crgba_t_003E2, &nvCompressionOptions);
							if (_Platform == TargetPlatform.PlayStation3)
							{
								Console.WriteLine("VolumeTexture CompressAndEmbed3D");
							}
							break;
						case "CubeTexture":
						{
							if (_Platform == TargetPlatform.PlayStation3)
							{
								Console.WriteLine("CubeTexture unhandled");
							}
							object[] args6 = new object[0];
							throw new BinaryAssetBuilderException(null, ErrorCode.InvalidArgument, "CubeTexture is not currently supported.  Create the cube texture as a DDS with the D3D Texture tool in the meantime.", args6);
						}
						case "StandardTexture":
							if (_Platform == TargetPlatform.PlayStation3)
							{
								if (flag3)
								{
									uint num5 = *(uint*)(&nvImage_003Crgba_t_003E2);
									uint num6 = System.Runtime.CompilerServices.Unsafe.As<nvImage_003Crgba_t_003E, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nvImage_003Crgba_t_003E2, 4));
									if ((uint)(*(int*)(&nvImage_003Crgba_t_003E2)) > 256u || (uint)System.Runtime.CompilerServices.Unsafe.As<nvImage_003Crgba_t_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nvImage_003Crgba_t_003E2, 4)) > 256u)
									{
										while (num5 > 256 || num6 > 256)
										{
											num5 >>= 1;
											num6 >>= 1;
										}
										System.Runtime.CompilerServices.Unsafe.As<nvCompressionOptions, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nvCompressionOptions, 60)) = 5;
										System.Runtime.CompilerServices.Unsafe.As<nvCompressionOptions, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nvCompressionOptions, 68)) = num5;
										System.Runtime.CompilerServices.Unsafe.As<nvCompressionOptions, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nvCompressionOptions, 72)) = num6;
										System.Runtime.CompilerServices.Unsafe.As<nvCompressionOptions, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nvCompressionOptions, 64)) = 4;
										object[] args5 = new object[4]
										{
											*(uint*)(&nvImage_003Crgba_t_003E2),
											System.Runtime.CompilerServices.Unsafe.As<nvImage_003Crgba_t_003E, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nvImage_003Crgba_t_003E2, 4)),
											num5,
											num6
										};
										_Tracer.TraceInfo("Resizing the image from {0}x{1} to {2}x{3} on the PS3.  Specify the 'AllowAutomaticResize' field in the texture XML to override this behavior.", args5);
									}
								}
								Console.WriteLine("PS3 StandardTexture, Resize, CompressAndEmbed");
							}
							CompressAndEmbed(tracker, textureData, &nvImage_003Crgba_t_003E2, &nvCompressionOptions);
							break;
						}
					}
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<nvImage_003Crgba_t_003E*, void>)(&global::_003CModule_003E.nvImage_003Crgba_t_003E_002E_007Bdtor_007D), &nvImage_003Crgba_t_003E2);
					throw;
				}
				global::_003CModule_003E.nvImage_003Crgba_t_003E_002E_007Bdtor_007D(&nvImage_003Crgba_t_003E2);
			}
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D), declaration);
			throw;
		}
		IntPtr intPtr6 = new IntPtr((void*)(int)(*(uint*)declaration));
		((GCHandle)intPtr6).Free();
		*(int*)declaration = 0;
	}

	public unsafe void HandleTextureType(gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* declaration, gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E* buffer)
	{
		try
		{
			try
			{
				bool flag = ((_Platform != TargetPlatform.Win32) ? true : false);
				Node* ptr = (Node*)global::_003CModule_003E.@new(16u);
				Node* ptr3;
				try
				{
					if (ptr != null)
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E obj);
						gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* pThis = &obj;
						IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)declaration));
						*(int*)(&obj) = (int)((IntPtr)GCHandle.Alloc(((InstanceDeclaration)((GCHandle)intPtr).Target).Document.NamespaceManager)).ToPointer();
						System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E obj2);
						try
						{
							gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E* ptr2 = &obj2;
							IntPtr intPtr2 = new IntPtr((void*)(int)(*(uint*)declaration));
							*(int*)(&obj2) = (int)((IntPtr)GCHandle.Alloc(((InstanceDeclaration)((GCHandle)intPtr2).Target).Node.CreateNavigator())).ToPointer();
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
					System.Runtime.CompilerServices.Unsafe.SkipInit(out Texture* ptr4);
					global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003ATexture_003E(&tracker, &ptr4);
					try
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out EndianHelper endianHelper);
						*(bool*)(&endianHelper) = flag;
						uint* ptr5 = (uint*)ptr4 + 2;
						if (flag)
						{
							System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
							*(sbyte*)(&num) = ((sbyte*)ptr5)[3];
							System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 1)) = ((sbyte*)ptr5)[2];
							System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 2)) = ((sbyte*)ptr5)[1];
							System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 3)) = *(sbyte*)ptr5;
							*ptr5 = num;
						}
						System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E obj3);
						gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* pThis2 = &obj3;
						IntPtr intPtr3 = new IntPtr((void*)(int)(*(uint*)declaration));
						*(int*)(&obj3) = (int)((IntPtr)GCHandle.Alloc(((GCHandle)intPtr3).Target)).ToPointer();
						Texture* textureData;
						try
						{
							textureData = (Texture*)((byte*)ptr4 + 4);
						}
						catch
						{
							//try-fault
							global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D), pThis2);
							throw;
						}
						ProcessTexture(&obj3, (TextureFileData*)textureData, &tracker);
						IntPtr intPtr4 = new IntPtr((void*)(int)(*(uint*)buffer));
						FinalizeTracker(&tracker, &endianHelper, (AssetBuffer)((GCHandle)intPtr4).Target);
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
					Node* intPtr5 = ptr3;
					((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)intPtr5)))((nint)intPtr5);
				}
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D), declaration);
				throw;
			}
			IntPtr intPtr6 = new IntPtr((void*)(int)(*(uint*)declaration));
			((GCHandle)intPtr6).Free();
			*(int*)declaration = 0;
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E_002E_007Bdtor_007D), buffer);
			throw;
		}
		IntPtr intPtr7 = new IntPtr((void*)(int)(*(uint*)buffer));
		((GCHandle)intPtr7).Free();
		*(int*)buffer = 0;
	}

	public unsafe void HandleOnDemandTextureType(gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* declaration, gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E* buffer)
	{
		try
		{
			try
			{
				bool flag = ((_Platform != TargetPlatform.Win32) ? true : false);
				Node* ptr = (Node*)global::_003CModule_003E.@new(16u);
				Node* ptr3;
				try
				{
					if (ptr != null)
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E obj);
						gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* pThis = &obj;
						IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)declaration));
						*(int*)(&obj) = (int)((IntPtr)GCHandle.Alloc(((InstanceDeclaration)((GCHandle)intPtr).Target).Document.NamespaceManager)).ToPointer();
						System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E obj2);
						try
						{
							gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E* ptr2 = &obj2;
							IntPtr intPtr2 = new IntPtr((void*)(int)(*(uint*)declaration));
							*(int*)(&obj2) = (int)((IntPtr)GCHandle.Alloc(((InstanceDeclaration)((GCHandle)intPtr2).Target).Node.CreateNavigator())).ToPointer();
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
					System.Runtime.CompilerServices.Unsafe.SkipInit(out OnDemandTexture* ptr4);
					global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AOnDemandTexture_003E(&tracker, &ptr4);
					try
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out EndianHelper endianHelper);
						*(bool*)(&endianHelper) = flag;
						if (ptr3 != null)
						{
							IValue* ptr5 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)ptr3 + 12)))((nint)ptr3, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_04KOACHJEN_0040Type_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BA_0040CCNAHAGP_0040StandardTexture_003F_0024AA_0040));
							if (ptr5 != null)
							{
								EnumNameGetter_003Cint_003E.EnumPair* ptr6 = global::_003CModule_003E.EnumLookup_002EFindNVPair_003Cstruct_0020EnumNameGetter_003Cint_003E_003A_003AEnumPair_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr5)))((nint)ptr5), (EnumNameGetter_003Cint_003E.EnumPair*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003FA0x749b4e39_002E_003FenumMap_0040_003F1_003F_003FGetValue_0040_003F_0024EnumNameGetter_0040W4TextureType_0040SageBinaryData_0040_0040_0040_0040SA_003FBW4TextureType_0040SageBinaryData_0040_0040PBDPA_N_0040Z_00404QBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040B), 3u);
								int num = ((ptr6 == null) ? 2 : ((int*)ptr6)[1]);
								((int*)ptr4)[1] = num;
								((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr5 + 4)))((nint)ptr5);
							}
						}
						System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker tracker2);
						System.Runtime.CompilerServices.Unsafe.SkipInit(out TextureFileData* textureData);
						global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020TextureFileData_003E(&tracker2, &textureData);
						try
						{
							System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E obj3);
							gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* ptr7 = &obj3;
							IntPtr intPtr3 = new IntPtr((void*)(int)(*(uint*)declaration));
							*(int*)(&obj3) = (int)((IntPtr)GCHandle.Alloc(((GCHandle)intPtr3).Target)).ToPointer();
							ProcessTexture(&obj3, textureData, &tracker2);
							System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E obj4);
							*(int*)(&obj4) = (int)((IntPtr)GCHandle.Alloc(new AssetBuffer())).ToPointer();
							try
							{
								IntPtr intPtr4 = new IntPtr((void*)(int)(*(uint*)(&obj4)));
								FinalizeTracker(&tracker2, &endianHelper, (AssetBuffer)((GCHandle)intPtr4).Target);
								IntPtr intPtr5 = new IntPtr((void*)(int)(*(uint*)declaration));
								FileStream fileStream = File.OpenWrite(((InstanceDeclaration)((GCHandle)intPtr5).Target).CustomDataPath);
								IntPtr intPtr6 = new IntPtr((void*)(int)(*(uint*)(&obj4)));
								int num2 = ((AssetBuffer)((GCHandle)intPtr6).Target).RelocationData.Length;
								if (flag)
								{
									System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num3);
									*(sbyte*)(&num3) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 1)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 2)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 3)) = *(sbyte*)(&num2);
									num2 = (int)num3;
								}
								IntPtr intPtr7 = new IntPtr((void*)(int)(*(uint*)declaration));
								uint num4 = ((InstanceDeclaration)((GCHandle)intPtr7).Target).Handle.InstanceId;
								if (flag)
								{
									System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num5);
									*(sbyte*)(&num5) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 3));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num5, 1)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 2));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num5, 2)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 1));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num5, 3)) = *(sbyte*)(&num4);
									num4 = num5;
								}
								System.Runtime.CompilerServices.Unsafe.SkipInit(out AssetHeader assetHeader);
								System.Runtime.CompilerServices.Unsafe.As<AssetHeader, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref assetHeader, 4)) = num4;
								IntPtr intPtr8 = new IntPtr((void*)(int)(*(uint*)declaration));
								uint num6 = ((InstanceDeclaration)((GCHandle)intPtr8).Target).Handle.TypeId;
								if (flag)
								{
									System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num7);
									*(sbyte*)(&num7) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num6, 3));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num7, 1)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num6, 2));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num7, 2)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num6, 1));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num7, 3)) = *(sbyte*)(&num6);
									num6 = num7;
								}
								*(uint*)(&assetHeader) = num6;
								IntPtr intPtr9 = new IntPtr((void*)(int)(*(uint*)declaration));
								uint num8 = ((InstanceDeclaration)((GCHandle)intPtr9).Target).Handle.TypeHash;
								if (flag)
								{
									System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num9);
									*(sbyte*)(&num9) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num8, 3));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num9, 1)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num8, 2));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num9, 2)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num8, 1));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num9, 3)) = *(sbyte*)(&num8);
									num8 = num9;
								}
								System.Runtime.CompilerServices.Unsafe.As<AssetHeader, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref assetHeader, 8)) = num8;
								IntPtr intPtr10 = new IntPtr((void*)(int)(*(uint*)declaration));
								uint num10 = ((InstanceDeclaration)((GCHandle)intPtr10).Target).Handle.InstanceHash;
								if (flag)
								{
									System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num11);
									*(sbyte*)(&num11) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num10, 3));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num11, 1)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num10, 2));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num11, 2)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num10, 1));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num11, 3)) = *(sbyte*)(&num10);
									num10 = num11;
								}
								System.Runtime.CompilerServices.Unsafe.As<AssetHeader, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref assetHeader, 12)) = num10;
								IntPtr intPtr11 = new IntPtr((void*)(int)(*(uint*)(&obj4)));
								int num12 = ((AssetBuffer)((GCHandle)intPtr11).Target).InstanceData.Length;
								if (flag)
								{
									System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num13);
									*(sbyte*)(&num13) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num12, 3));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num13, 1)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num12, 2));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num13, 2)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num12, 1));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num13, 3)) = *(sbyte*)(&num12);
									num12 = (int)num13;
								}
								System.Runtime.CompilerServices.Unsafe.As<AssetHeader, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref assetHeader, 16)) = num12;
								IntPtr intPtr12 = new IntPtr((void*)(int)(*(uint*)(&obj4)));
								int num14 = ((AssetBuffer)((GCHandle)intPtr12).Target).RelocationData.Length;
								if (flag)
								{
									System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num15);
									*(sbyte*)(&num15) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num14, 3));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num15, 1)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num14, 2));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num15, 2)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num14, 1));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num15, 3)) = *(sbyte*)(&num14);
									num14 = (int)num15;
								}
								System.Runtime.CompilerServices.Unsafe.As<AssetHeader, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref assetHeader, 20)) = num14;
								IntPtr intPtr13 = new IntPtr((void*)(int)(*(uint*)(&obj4)));
								int num16 = ((AssetBuffer)((GCHandle)intPtr13).Target).ImportsData.Length;
								if (flag)
								{
									System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num17);
									*(sbyte*)(&num17) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num16, 3));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num17, 1)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num16, 2));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num17, 2)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num16, 1));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num17, 3)) = *(sbyte*)(&num16);
									num16 = (int)num17;
								}
								System.Runtime.CompilerServices.Unsafe.As<AssetHeader, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref assetHeader, 24)) = num16;
								byte[] array = new byte[32];
								fixed (byte* ptr8 = &array[0])
								{
									// IL cpblk instruction
									System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr8, ref assetHeader, 32);
									fileStream.Write(array, 0, array.Length);
									IntPtr intPtr14 = new IntPtr((void*)(int)(*(uint*)(&obj4)));
									AssetBuffer target = (AssetBuffer)((GCHandle)intPtr14).Target;
									IntPtr intPtr15 = new IntPtr((void*)(int)(*(uint*)(&obj4)));
									AssetBuffer target2 = (AssetBuffer)((GCHandle)intPtr15).Target;
									fileStream.Write(target2.InstanceData, 0, target.InstanceData.Length);
									IntPtr intPtr16 = new IntPtr((void*)(int)(*(uint*)(&obj4)));
									AssetBuffer target3 = (AssetBuffer)((GCHandle)intPtr16).Target;
									IntPtr intPtr17 = new IntPtr((void*)(int)(*(uint*)(&obj4)));
									AssetBuffer target4 = (AssetBuffer)((GCHandle)intPtr17).Target;
									fileStream.Write(target4.RelocationData, 0, target3.RelocationData.Length);
									fileStream.Close();
									IntPtr intPtr18 = new IntPtr((void*)(int)(*(uint*)buffer));
									FinalizeTracker(&tracker, &endianHelper, (AssetBuffer)((GCHandle)intPtr18).Target);
								}
							}
							catch
							{
								//try-fault
								global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E_002E_007Bdtor_007D), &obj4);
								throw;
							}
							IntPtr intPtr19 = new IntPtr((void*)(int)(*(uint*)(&obj4)));
							((GCHandle)intPtr19).Free();
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
					Node* intPtr20 = ptr3;
					((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)intPtr20)))((nint)intPtr20);
				}
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D), declaration);
				throw;
			}
			IntPtr intPtr21 = new IntPtr((void*)(int)(*(uint*)declaration));
			((GCHandle)intPtr21).Free();
			*(int*)declaration = 0;
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E_002E_007Bdtor_007D), buffer);
			throw;
		}
		IntPtr intPtr22 = new IntPtr((void*)(int)(*(uint*)buffer));
		((GCHandle)intPtr22).Free();
		*(int*)buffer = 0;
	}

	public unsafe void HandleTerrainTextureAtlas(gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* declaration, gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E* buffer)
	{
		int num = (int)stackalloc byte[global::_003CModule_003E.__CxxQueryExceptionSize()];
		bool flag;
		Node* ptr3;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker tracker);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out TerrainTextureAtlas* ptr4);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker tracker2);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out TerrainTextureAtlasRuntime* ptr5);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out EndianHelper endianHelper);
		try
		{
			try
			{
				flag = ((_Platform != TargetPlatform.Win32) ? true : false);
				Node* ptr = (Node*)global::_003CModule_003E.@new(16u);
				try
				{
					if (ptr != null)
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E obj);
						gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* pThis = &obj;
						IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)declaration));
						*(int*)(&obj) = (int)((IntPtr)GCHandle.Alloc(((InstanceDeclaration)((GCHandle)intPtr).Target).Document.NamespaceManager)).ToPointer();
						System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E obj2);
						try
						{
							gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E* ptr2 = &obj2;
							IntPtr intPtr2 = new IntPtr((void*)(int)(*(uint*)declaration));
							*(int*)(&obj2) = (int)((IntPtr)GCHandle.Alloc(((InstanceDeclaration)((GCHandle)intPtr2).Target).Node.CreateNavigator())).ToPointer();
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
				*(int*)(&nodeHolder) = (int)ptr3;
				try
				{
					global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003ATerrainTextureAtlas_003E(&tracker, &ptr4);
					try
					{
						global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003ATerrainTextureAtlasRuntime_003E(&tracker2, &ptr5);
						try
						{
							*(bool*)(&endianHelper) = flag;
							global::_003CModule_003E.Marshaler_002EMarshal((INode*)ptr3, ptr4, &tracker);
							if (((int*)ptr4)[1] == 0)
							{
								goto end_IL_00e4;
							}
							goto end_IL_00da;
							end_IL_00e4:;
						}
						catch
						{
							//try-fault
							global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker2);
							throw;
						}
						global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker2);
						goto IL_0126;
						end_IL_00da:;
					}
					catch
					{
						//try-fault
						global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker);
						throw;
					}
					goto end_IL_00d0;
					IL_0126:
					global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker);
					goto IL_013d;
					end_IL_00d0:;
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&global::_003CModule_003E.Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder);
					throw;
				}
				goto end_IL_0009;
				IL_013d:
				if (ptr3 != null)
				{
					Node* intPtr3 = ptr3;
					((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)intPtr3)))((nint)intPtr3);
				}
				goto IL_015a;
				end_IL_0009:;
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D), declaration);
				throw;
			}
			goto end_IL_0009_2;
			IL_015a:
			IntPtr intPtr4 = new IntPtr((void*)(int)(*(uint*)declaration));
			((GCHandle)intPtr4).Free();
			*(int*)declaration = 0;
			goto IL_0185;
			end_IL_0009_2:;
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E_002E_007Bdtor_007D), buffer);
			throw;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out TexturePacker texturePacker);
		try
		{
			try
			{
				try
				{
					try
					{
						try
						{
							global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002E_007Bctor_007D(&texturePacker, ptr4);
							try
							{
								if (!global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002EPackTextures(&texturePacker))
								{
									object[] args = new object[0];
									throw new BinaryAssetBuilderException(ErrorCode.InternalError, "TexturePacker::PackTextures failed for some unknown reason.", args);
								}
								switch (_Platform)
								{
								case TargetPlatform.Xbox360:
								{
									System.Runtime.CompilerServices.Unsafe.SkipInit(out sbyte* value);
									try
									{
										if (*(int*)(&texturePacker) != 0)
										{
											global::_003CModule_003E.XenonBundler_002EProcess2DTexture(global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Bundler, (nvImage_003Crgba_t_003E*)(int)(*(uint*)(&texturePacker)), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0M_0040MOIKHIPB_0040D3DFMT_DXT5_003F_0024AA_0040), false, 3u);
											EmbedXenonTexture(&tracker2, (TextureFileData*)((byte*)ptr5 + 12), global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Bundler);
										}
										nvImage_003Crgba_t_003E* ptr6 = (nvImage_003Crgba_t_003E*)(int)System.Runtime.CompilerServices.Unsafe.As<TexturePacker, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref texturePacker, 4));
										if (System.Runtime.CompilerServices.Unsafe.As<TexturePacker, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref texturePacker, 4)) != 0)
										{
											nvImage_003Crgba_t_003E* ptr7 = (nvImage_003Crgba_t_003E*)(int)System.Runtime.CompilerServices.Unsafe.As<TexturePacker, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref texturePacker, 4));
											global::_003CModule_003E.XenonBundler_002EProcess2DTexture(global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Bundler, (nvImage_003Crgba_t_003E*)(int)System.Runtime.CompilerServices.Unsafe.As<TexturePacker, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref texturePacker, 4)), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0M_0040KKOGLNPF_0040D3DFMT_DXT1_003F_0024AA_0040), false, 3u);
											EmbedXenonTexture(&tracker2, (TextureFileData*)((byte*)ptr5 + 20), global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Bundler);
										}
									}
									catch when (global::_003CModule_003E.__CxxExceptionFilter((void*)Marshal.GetExceptionPointers(), System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_R0PAD_00408), 1, &value) != 0)
									{
										uint num2 = 0u;
										global::_003CModule_003E.__CxxRegisterExceptionObject((void*)Marshal.GetExceptionPointers(), (void*)num);
										try
										{
											try
											{
												IntPtr ptr8 = new IntPtr(value);
												string text = Marshal.PtrToStringAnsi(ptr8);
												object[] args2 = new object[0];
												throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Xenon error: " + text, args2);
											}
											catch when (((Func<bool>)delegate
											{
												// Could not convert BlockContainer to single expression
												num2 = (uint)global::_003CModule_003E.__CxxDetectRethrow((void*)Marshal.GetExceptionPointers());
												return (byte)num2 != 0;
											}).Invoke())
											{
											}
											if (num2 != 0)
											{
												throw;
											}
										}
										finally
										{
											global::_003CModule_003E.__CxxUnregisterExceptionObject((void*)num, (int)num2);
										}
									}
									break;
								}
								case TargetPlatform.PlayStation3:
								{
									System.Runtime.CompilerServices.Unsafe.SkipInit(out nvCompressionOptions nvCompressionOptions3);
									global::_003CModule_003E.nvCompressionOptions_002E_007Bctor_007D(&nvCompressionOptions3);
									System.Runtime.CompilerServices.Unsafe.As<nvCompressionOptions, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nvCompressionOptions3, 476)) = 3;
									System.Runtime.CompilerServices.Unsafe.As<nvCompressionOptions, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nvCompressionOptions3, 104)) = 2;
									CompressAndEmbed(&tracker2, (TextureFileData*)((byte*)ptr5 + 12), (nvImage_003Crgba_t_003E*)(int)(*(uint*)(&texturePacker)), &nvCompressionOptions3);
									if (System.Runtime.CompilerServices.Unsafe.As<TexturePacker, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref texturePacker, 4)) != 0)
									{
										System.Runtime.CompilerServices.Unsafe.SkipInit(out nvCompressionOptions nvCompressionOptions4);
										global::_003CModule_003E.nvCompressionOptions_002E_007Bctor_007D(&nvCompressionOptions4);
										System.Runtime.CompilerServices.Unsafe.As<nvCompressionOptions, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nvCompressionOptions4, 476)) = 0;
										System.Runtime.CompilerServices.Unsafe.As<nvCompressionOptions, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nvCompressionOptions4, 104)) = 2;
										CompressAndEmbed(&tracker2, (TextureFileData*)((byte*)ptr5 + 20), (nvImage_003Crgba_t_003E*)(int)System.Runtime.CompilerServices.Unsafe.As<TexturePacker, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref texturePacker, 4)), &nvCompressionOptions4);
									}
									break;
								}
								default:
								{
									System.Runtime.CompilerServices.Unsafe.SkipInit(out nvCompressionOptions nvCompressionOptions);
									global::_003CModule_003E.nvCompressionOptions_002E_007Bctor_007D(&nvCompressionOptions);
									System.Runtime.CompilerServices.Unsafe.As<nvCompressionOptions, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nvCompressionOptions, 476)) = 3;
									System.Runtime.CompilerServices.Unsafe.As<nvCompressionOptions, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nvCompressionOptions, 104)) = 5;
									CompressAndEmbed(&tracker2, (TextureFileData*)((byte*)ptr5 + 12), (nvImage_003Crgba_t_003E*)(int)(*(uint*)(&texturePacker)), &nvCompressionOptions);
									if (System.Runtime.CompilerServices.Unsafe.As<TexturePacker, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref texturePacker, 4)) != 0)
									{
										System.Runtime.CompilerServices.Unsafe.SkipInit(out nvCompressionOptions nvCompressionOptions2);
										global::_003CModule_003E.nvCompressionOptions_002E_007Bctor_007D(&nvCompressionOptions2);
										System.Runtime.CompilerServices.Unsafe.As<nvCompressionOptions, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nvCompressionOptions2, 476)) = 9;
										System.Runtime.CompilerServices.Unsafe.As<nvCompressionOptions, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nvCompressionOptions2, 104)) = 5;
										CompressAndEmbed(&tracker2, (TextureFileData*)((byte*)ptr5 + 20), (nvImage_003Crgba_t_003E*)(int)System.Runtime.CompilerServices.Unsafe.As<TexturePacker, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref texturePacker, 4)), &nvCompressionOptions2);
									}
									break;
								}
								}
								uint objectCount = (uint)(System.Runtime.CompilerServices.Unsafe.As<TexturePacker, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref texturePacker, 12)) - System.Runtime.CompilerServices.Unsafe.As<TexturePacker, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref texturePacker, 8)) >> 2);
								TerrainTextureAtlasRuntime* ptr9 = (TerrainTextureAtlasRuntime*)((byte*)ptr5 + 8);
								TerrainTextureTileRuntime** pointerLocation = (TerrainTextureTileRuntime**)ptr9;
								global::_003CModule_003E.Relo_002ETracker_002EPush(&tracker2, (void**)pointerLocation, 12u, objectCount);
								uint num3 = (uint)(System.Runtime.CompilerServices.Unsafe.As<TexturePacker, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref texturePacker, 12)) - System.Runtime.CompilerServices.Unsafe.As<TexturePacker, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref texturePacker, 8)) >> 2);
								if (flag)
								{
									System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num4);
									*(sbyte*)(&num4) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 3));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 1)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 2));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 2)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 1));
									System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 3)) = *(sbyte*)(&num3);
									num3 = num4;
								}
								((int*)ptr5)[1] = (int)num3;
								uint num5 = 0u;
								if (0u < (uint)(System.Runtime.CompilerServices.Unsafe.As<TexturePacker, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref texturePacker, 12)) - System.Runtime.CompilerServices.Unsafe.As<TexturePacker, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref texturePacker, 8)) >> 2))
								{
									int num6 = 0;
									do
									{
										TexturePacker.Texture* ptr10 = (TexturePacker.Texture*)(int)(*(uint*)(int)(num5 * 4 + (uint)System.Runtime.CompilerServices.Unsafe.As<TexturePacker, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref texturePacker, 8))));
										TerrainTextureTileRuntime* ptr11 = (TerrainTextureTileRuntime*)(num6 + *(int*)ptr9);
										*(int*)ptr11 = ((int*)ptr10)[2];
										((short*)ptr11)[2] = ((short*)ptr10)[6];
										((short*)ptr11)[3] = ((short*)ptr10)[7];
										((short*)ptr11)[4] = ((short*)ptr10)[8];
										((short*)ptr11)[5] = ((short*)ptr10)[9];
										global::_003CModule_003E.Marshaler_002EEndianCorrect(ptr11, &endianHelper);
										num5++;
										num6 += 12;
									}
									while (num5 < (uint)(System.Runtime.CompilerServices.Unsafe.As<TexturePacker, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref texturePacker, 12)) - System.Runtime.CompilerServices.Unsafe.As<TexturePacker, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref texturePacker, 8)) >> 2));
								}
								System.Runtime.CompilerServices.Unsafe.As<Tracker, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref tracker2, 8)) -= 4;
								IntPtr intPtr5 = new IntPtr((void*)(int)(*(uint*)buffer));
								FinalizeTracker(&tracker2, &endianHelper, (AssetBuffer)((GCHandle)intPtr5).Target);
							}
							catch
							{
								//try-fault
								global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<TexturePacker*, void>)(&global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002E_007Bdtor_007D), &texturePacker);
								throw;
							}
							global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002E_007Bdtor_007D(&texturePacker);
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
					Node* intPtr6 = ptr3;
					((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)intPtr6)))((nint)intPtr6);
				}
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D), declaration);
				throw;
			}
			IntPtr intPtr7 = new IntPtr((void*)(int)(*(uint*)declaration));
			((GCHandle)intPtr7).Free();
			*(int*)declaration = 0;
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E_002E_007Bdtor_007D), buffer);
			throw;
		}
		IntPtr intPtr8 = new IntPtr((void*)(int)(*(uint*)buffer));
		((GCHandle)intPtr8).Free();
		*(int*)buffer = 0;
		goto IL_0564;
		IL_0185:
		IntPtr intPtr9 = new IntPtr((void*)(int)(*(uint*)buffer));
		((GCHandle)intPtr9).Free();
		*(int*)buffer = 0;
		goto IL_0564;
		IL_0564:
		try
		{
			try
			{
				try
				{
					try
					{
						try
						{
							try
							{
								return;
							}
							catch
							{
								//try-fault
								global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<TexturePacker*, void>)(&global::_003CModule_003E.BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002E_007Bdtor_007D), &texturePacker);
								throw;
							}
						}
						catch
						{
							//try-fault
							global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker2);
							throw;
						}
					}
					catch
					{
						//try-fault
						global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker);
						throw;
					}
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&global::_003CModule_003E.Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder);
					throw;
				}
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D), declaration);
				throw;
			}
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E_002E_007Bdtor_007D), buffer);
			throw;
		}
	}

	public unsafe virtual AssetBuffer ProcessInstance(InstanceDeclaration declaration)
	{
		AssetBuffer assetBuffer = new AssetBuffer();
		switch (declaration.Handle.TypeId)
		{
		default:
			Console.WriteLine("Warning: Couldn't process {0}. No matching handler found.", declaration);
			return null;
		case 2467477932u:
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E obj3);
			gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E* pThis2 = &obj3;
			*(int*)(&obj3) = (int)((IntPtr)GCHandle.Alloc(assetBuffer)).ToPointer();
			System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E obj4);
			try
			{
				gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* ptr2 = &obj4;
				*(int*)(&obj4) = (int)((IntPtr)GCHandle.Alloc(declaration)).ToPointer();
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E_002E_007Bdtor_007D), pThis2);
				throw;
			}
			HandleOnDemandTextureType(&obj4, &obj3);
			break;
		}
		case 1601459053u:
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E obj5);
			gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E* pThis3 = &obj5;
			*(int*)(&obj5) = (int)((IntPtr)GCHandle.Alloc(assetBuffer)).ToPointer();
			System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E obj6);
			try
			{
				gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* ptr3 = &obj6;
				*(int*)(&obj6) = (int)((IntPtr)GCHandle.Alloc(declaration)).ToPointer();
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E_002E_007Bdtor_007D), pThis3);
				throw;
			}
			HandleTerrainTextureAtlas(&obj6, &obj5);
			break;
		}
		case 568797146u:
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E obj);
			gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E* pThis = &obj;
			*(int*)(&obj) = (int)((IntPtr)GCHandle.Alloc(assetBuffer)).ToPointer();
			System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E obj2);
			try
			{
				gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* ptr = &obj2;
				*(int*)(&obj2) = (int)((IntPtr)GCHandle.Alloc(declaration)).ToPointer();
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E_002E_007Bdtor_007D), pThis);
				throw;
			}
			HandleTextureType(&obj2, &obj);
			break;
		}
		}
		return assetBuffer;
	}

	public virtual uint GetAllTypesHash()
	{
		return 1424942948u;
	}

	public virtual uint GetVersionNumber()
	{
		uint result = 0u;
		switch (_Platform)
		{
		case TargetPlatform.Xbox360:
			result = 4605u;
			break;
		case TargetPlatform.Win32:
			result = 4202u;
			break;
		}
		return result;
	}

	public unsafe virtual ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
	{
		ExtendedTypeInformation extendedTypeInformation = new ExtendedTypeInformation();
		extendedTypeInformation.HasCustomData = false;
		extendedTypeInformation.TypeId = typeId;
		switch (typeId)
		{
		case 2467477932u:
			switch (_Platform)
			{
			case TargetPlatform.Xbox360:
				extendedTypeInformation.ProcessingHash = 4165508899u;
				break;
			case TargetPlatform.PlayStation3:
				extendedTypeInformation.ProcessingHash = 4165508280u;
				break;
			default:
				extendedTypeInformation.ProcessingHash = 4165508788u;
				break;
			}
			extendedTypeInformation.HasCustomData = true;
			extendedTypeInformation.TypeHash = 4165504734u;
			extendedTypeInformation.TypeName = new string(global::_003CModule_003E._003FTypeName_0040OnDemandTexture_0040SageBinaryData_0040_00402PBDB);
			break;
		case 1601459053u:
			switch (_Platform)
			{
			case TargetPlatform.Xbox360:
				extendedTypeInformation.ProcessingHash = 1182597248u;
				break;
			case TargetPlatform.PlayStation3:
				extendedTypeInformation.ProcessingHash = 1182597327u;
				break;
			default:
				extendedTypeInformation.ProcessingHash = 1182597302u;
				break;
			}
			extendedTypeInformation.TypeHash = 417928170u;
			extendedTypeInformation.TypeName = new string(global::_003CModule_003E._003FTypeName_0040TerrainTextureAtlas_0040SageBinaryData_0040_00402PBDB);
			break;
		case 568797146u:
			switch (_Platform)
			{
			case TargetPlatform.Xbox360:
				extendedTypeInformation.ProcessingHash = 2673862951u;
				break;
			case TargetPlatform.PlayStation3:
				extendedTypeInformation.ProcessingHash = 2673863356u;
				break;
			default:
				extendedTypeInformation.ProcessingHash = 2673862832u;
				break;
			}
			extendedTypeInformation.TypeHash = 2673866970u;
			extendedTypeInformation.TypeName = new string(global::_003CModule_003E._003FTypeName_0040Texture_0040SageBinaryData_0040_00402PBDB);
			break;
		}
		return extendedTypeInformation;
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
