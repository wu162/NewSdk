using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BinaryAssetBuilder.Core;
using Endian;
using Marshaler;
using Relo;
using SageBinaryData;
using eastl;

namespace BinaryAssetBuilder.W3XCompiler;

public class Plugin : IAssetBuilderPlugin, IDisposable
{
	private uint m_HashCode = 0u;

	private TargetPlatform m_Platform = TargetPlatform.Win32;

	private static uint W3XPluginVersion_PC = 8u;

	private static uint W3XPluginVersion_Xenon = 9u;

	private static uint W3XPluginVersion_PS3 = 11u;

	private void _007EPlugin()
	{
	}

	private void _0021Plugin()
	{
	}

	public virtual void Initialize(object configSection, TargetPlatform platform)
	{
		m_HashCode = HashProvider.GetTypeHash(GetType());
		m_Platform = platform;
	}

	public virtual void ReInitialize(object configSection, TargetPlatform platform)
	{
		Initialize(configSection, platform);
	}

	public unsafe void FinalizeTracker(Tracker* tracker, EndianHelper* helper, AssetBuffer buffer)
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
			global::_003CModule_003E.Relo_002ETracker_002EMakeRelocatable(tracker, &chunk, helper, 0u);
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

	public unsafe AssetBuffer HandleAnimation(InstanceDeclaration declaration)
	{
		System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker tracker);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out W3DAnimation* ptr);
		global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AW3DAnimation_003E(&tracker, &ptr);
		AssetBuffer assetBuffer;
		try
		{
			bool flag = ((m_Platform != TargetPlatform.Win32) ? true : false);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker tracker2);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out W3DAnimationRuntime* ptr2);
			global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AW3DAnimationRuntime_003E(&tracker2, &ptr2);
			try
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out EndianHelper endianHelper);
				*(bool*)(&endianHelper) = flag;
				Node* ptr3 = (Node*)global::_003CModule_003E.@new(16u);
				Node* ptr5;
				try
				{
					if (ptr3 != null)
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E obj);
						gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* pThis = &obj;
						*(int*)(&obj) = (int)((IntPtr)GCHandle.Alloc(declaration.Document.NamespaceManager)).ToPointer();
						System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E obj2);
						try
						{
							gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E* ptr4 = &obj2;
							*(int*)(&obj2) = (int)((IntPtr)GCHandle.Alloc(declaration.Node.CreateNavigator())).ToPointer();
						}
						catch
						{
							//try-fault
							global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E_002E_007Bdtor_007D), pThis);
							throw;
						}
						ptr5 = global::_003CModule_003E.Node_002E_007Bctor_007D(ptr3, &obj2, &obj);
					}
					else
					{
						ptr5 = null;
					}
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.delete(ptr3);
					throw;
				}
				System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
				*(int*)(&nodeHolder) = (int)ptr5;
				try
				{
					global::_003CModule_003E.Marshaler_002EMarshal((INode*)ptr5, ptr, &tracker);
					W3DAnimation* ptr6 = (W3DAnimation*)((byte*)ptr + 16);
					System.Runtime.CompilerServices.Unsafe.SkipInit(out W3DAnimation.CompressionSettings compressionSettings);
					if (*(int*)ptr6 != 0)
					{
						// IL cpblk instruction
						System.Runtime.CompilerServices.Unsafe.CopyBlock(ref compressionSettings, *(int*)ptr6, 24);
						if (System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 21)) == 0 && System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 20)) == 0)
						{
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 20)) = 1;
							*(float*)(&compressionSettings) = 0f;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 4)) = 0f;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 8)) = 0f;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 16)) = 1f;
						}
					}
					else
					{
						TargetPlatform platform = m_Platform;
						switch (platform)
						{
						default:
							throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Internal error: unknown platform {0}", platform);
						case TargetPlatform.PlayStation3:
							*(float*)(&compressionSettings) = 0.002f;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 4)) = 0.003f;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 8)) = 0.01f;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 12)) = 0.001f;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 16)) = 1f;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 20)) = 1;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 21)) = 1;
							break;
						case TargetPlatform.Xbox360:
							*(float*)(&compressionSettings) = 0.002f;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 4)) = 0.003f;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 8)) = 0.01f;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 12)) = 0.001f;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 16)) = 1f;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 20)) = 1;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 21)) = 1;
							break;
						case TargetPlatform.Win32:
							*(float*)(&compressionSettings) = 0.002f;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 4)) = 0.003f;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 8)) = 0.01f;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 12)) = 0.001f;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 16)) = 1f;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 20)) = 1;
							System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 21)) = 1;
							break;
						}
					}
					AssetReference_003CSageBinaryData_003A_003AW3DHierarchy_002CSage_003A_003ARender_003A_003AW3DHierarchyWrapper_003E* ptr7 = (AssetReference_003CSageBinaryData_003A_003AW3DHierarchy_002CSage_003A_003ARender_003A_003AW3DHierarchyWrapper_003E*)((byte*)ptr + 4);
					AssetReference_003CSageBinaryData_003A_003AW3DHierarchy_002CSage_003A_003ARender_003A_003AW3DHierarchyWrapper_003E* location = (AssetReference_003CSageBinaryData_003A_003AW3DHierarchy_002CSage_003A_003ARender_003A_003AW3DHierarchyWrapper_003E*)((byte*)ptr2 + 4);
					global::_003CModule_003E.Relo_002ETracker_002EAddReference(&tracker2, location, *(uint*)ptr7);
					W3DAnimation* ptr8 = (W3DAnimation*)((byte*)ptr + 8);
					((int*)ptr2)[2] = (int)global::_003CModule_003E.Endian_002EEndianHelper_002EEndianToPlatform(&endianHelper, *(uint*)ptr8);
					((int*)ptr2)[3] = (int)global::_003CModule_003E.Endian_002EEndianHelper_002EEndianToPlatform(&endianHelper, ((uint*)ptr)[3]);
					W3DAnimation* ptr9 = (W3DAnimation*)((byte*)ptr + 20);
					uint objectCount = *(uint*)ptr9;
					W3DAnimationRuntime* ptr10 = (W3DAnimationRuntime*)((byte*)ptr2 + 20);
					AnimationChannelBaseRuntime*** pointerLocation = (AnimationChannelBaseRuntime***)ptr10;
					global::_003CModule_003E.Relo_002ETracker_002EPush(&tracker2, (void**)pointerLocation, 4u, objectCount);
					uint num = *(uint*)ptr9;
					if (*(bool*)(&endianHelper))
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
						*(sbyte*)(&num2) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 3));
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 2));
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 1));
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3)) = *(sbyte*)(&num);
						num = num2;
					}
					((int*)ptr2)[4] = (int)num;
					uint num3 = 0u;
					uint num4 = *(uint*)ptr9;
					System.Runtime.CompilerServices.Unsafe.SkipInit(out vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E);
					System.Runtime.CompilerServices.Unsafe.SkipInit(out SrcFrame srcFrame2);
					if (0 < num4)
					{
						W3DAnimation* ptr11 = (W3DAnimation*)((byte*)ptr + 24);
						System.Runtime.CompilerServices.Unsafe.SkipInit(out SrcFrame srcFrame);
						do
						{
							AnimationChannelBase* ptr12 = (AnimationChannelBase*)(int)(*(uint*)(int)(num3 * 4 + (uint)(*(int*)ptr11)));
							*(int*)(&vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E) = 0;
							System.Runtime.CompilerServices.Unsafe.As<vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E, 4)) = 0;
							System.Runtime.CompilerServices.Unsafe.As<vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E, 8)) = 0;
							try
							{
								vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr20;
								switch (*(int*)ptr12)
								{
								default:
									goto end_IL_02f1;
								case 1355976437:
								{
									vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr17 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame2, 4));
									System.Runtime.CompilerServices.Unsafe.As<SrcFrame, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame2, 4)) = 0;
									System.Runtime.CompilerServices.Unsafe.As<SrcFrame, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame2, 8)) = 0;
									System.Runtime.CompilerServices.Unsafe.As<SrcFrame, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame2, 12)) = 0;
									try
									{
										*(int*)(&srcFrame2) = 0;
										switch (((int*)ptr12)[1])
										{
										default:
											goto end_IL_0328;
										case 4:
										{
											float num17 = 1f;
											global::_003CModule_003E.eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002Epush_back((vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame2, 4)), &num17);
											break;
										}
										case 0:
										case 1:
										case 2:
										{
											float num16 = 0f;
											global::_003CModule_003E.eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002Epush_back((vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame2, 4)), &num16);
											break;
										}
										}
										System.Runtime.CompilerServices.Unsafe.As<SrcFrame, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame2, 20)) = 0;
										global::_003CModule_003E.eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002Eresize(&vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E, *(uint*)ptr8, &srcFrame2);
										uint num18 = 0u;
										if (0u < (uint)(*(int*)ptr8))
										{
											int num19 = 0;
											do
											{
												*(uint*)(num19 + *(int*)(&vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E)) = num18;
												num18++;
												num19 += 24;
											}
											while (num18 < (uint)(*(int*)ptr8));
										}
										uint num20 = 0u;
										uint num21 = ((uint*)ptr12)[4];
										if (0 < num21)
										{
											AnimationChannelBase* ptr18 = (AnimationChannelBase*)((byte*)ptr12 + 20);
											do
											{
												AnimationChannelScalar.Frame* ptr19 = (AnimationChannelScalar.Frame*)(int)(num20 * 8 + (uint)(*(int*)ptr18));
												*(float*)(int)(*(uint*)(*(int*)(&vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E) + (((int*)ptr12)[2] + (int)num20) * 24 + 4)) = *(float*)ptr19;
												*(sbyte*)(*(int*)(&vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E) + (((int*)ptr12)[2] + (int)num20) * 24 + 20) = ((sbyte*)ptr19)[4];
												num20++;
												num21 = ((uint*)ptr12)[4];
											}
											while (num20 < num21);
										}
										goto IL_0418;
										end_IL_0328:;
									}
									catch
									{
										//try-fault
										global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<SrcFrame*, void>)(&global::_003CModule_003E.SrcFrame_002E_007Bdtor_007D), &srcFrame2);
										throw;
									}
									goto IL_078e;
								}
								case -163390176:
									{
										vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr13 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 4));
										System.Runtime.CompilerServices.Unsafe.As<SrcFrame, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 4)) = 0;
										System.Runtime.CompilerServices.Unsafe.As<SrcFrame, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 8)) = 0;
										System.Runtime.CompilerServices.Unsafe.As<SrcFrame, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 12)) = 0;
										try
										{
											*(int*)(&srcFrame) = 0;
											float num5 = 0f;
											if ((uint)System.Runtime.CompilerServices.Unsafe.As<SrcFrame, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 8)) < (uint)System.Runtime.CompilerServices.Unsafe.As<SrcFrame, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 12)))
											{
												int num6 = System.Runtime.CompilerServices.Unsafe.As<SrcFrame, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 8));
												System.Runtime.CompilerServices.Unsafe.As<SrcFrame, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 8)) += 4;
												if (num6 != 0)
												{
													*(float*)num6 = 0f;
												}
											}
											else
											{
												global::_003CModule_003E.eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue((vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 4)), (float*)(int)System.Runtime.CompilerServices.Unsafe.As<SrcFrame, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 8)), &num5);
											}
											float num7 = 0f;
											if ((uint)System.Runtime.CompilerServices.Unsafe.As<SrcFrame, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 8)) < (uint)System.Runtime.CompilerServices.Unsafe.As<SrcFrame, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 12)))
											{
												int num8 = System.Runtime.CompilerServices.Unsafe.As<SrcFrame, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 8));
												System.Runtime.CompilerServices.Unsafe.As<SrcFrame, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 8)) += 4;
												if (num8 != 0)
												{
													*(float*)num8 = 0f;
												}
											}
											else
											{
												global::_003CModule_003E.eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue((vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 4)), (float*)(int)System.Runtime.CompilerServices.Unsafe.As<SrcFrame, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 8)), &num7);
											}
											float num9 = 0f;
											global::_003CModule_003E.eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002Epush_back((vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 4)), &num9);
											float num10 = 1f;
											global::_003CModule_003E.eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002Epush_back((vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 4)), &num10);
											System.Runtime.CompilerServices.Unsafe.As<SrcFrame, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 20)) = 0;
											global::_003CModule_003E.eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002Eresize(&vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E, *(uint*)ptr8, &srcFrame);
											uint num11 = 0u;
											if (0u < (uint)(*(int*)ptr8))
											{
												int num12 = 0;
												do
												{
													*(uint*)(num12 + *(int*)(&vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E)) = num11;
													num11++;
													num12 += 24;
												}
												while (num11 < (uint)(*(int*)ptr8));
											}
											uint num13 = 0u;
											uint num14 = ((uint*)ptr12)[4];
											if (0 < num14)
											{
												AnimationChannelBase* ptr14 = (AnimationChannelBase*)((byte*)ptr12 + 20);
												int num15 = 0;
												do
												{
													AnimationChannelQuaternion.Frame* ptr15 = (AnimationChannelQuaternion.Frame*)(num15 + *(int*)ptr14);
													*(float*)(int)(*(uint*)(*(int*)(&vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E) + (((int*)ptr12)[2] + (int)num13) * 24 + 4)) = *(float*)ptr15;
													*(float*)(*(int*)(*(int*)(&vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E) + (((int*)ptr12)[2] + (int)num13) * 24 + 4) + 4) = ((float*)ptr15)[1];
													*(float*)(*(int*)(*(int*)(&vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E) + (((int*)ptr12)[2] + (int)num13) * 24 + 4) + 8) = ((float*)ptr15)[2];
													*(float*)(*(int*)(*(int*)(&vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E) + (((int*)ptr12)[2] + (int)num13) * 24 + 4) + 12) = ((float*)ptr15)[3];
													*(sbyte*)(*(int*)(&vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E) + (((int*)ptr12)[2] + (int)num13) * 24 + 20) = ((sbyte*)ptr15)[16];
													num13++;
													num15 += 20;
													num14 = ((uint*)ptr12)[4];
												}
												while (num13 < num14);
											}
										}
										catch
										{
											//try-fault
											global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<SrcFrame*, void>)(&global::_003CModule_003E.SrcFrame_002E_007Bdtor_007D), &srcFrame);
											throw;
										}
										vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr16 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 4));
										if (System.Runtime.CompilerServices.Unsafe.As<SrcFrame, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 4)) != 0)
										{
											global::_003CModule_003E.delete_005B_005D((void*)(int)System.Runtime.CompilerServices.Unsafe.As<SrcFrame, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 4)));
										}
										break;
									}
									IL_0418:
									ptr20 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame2, 4));
									if (System.Runtime.CompilerServices.Unsafe.As<SrcFrame, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame2, 4)) != 0)
									{
										global::_003CModule_003E.delete_005B_005D((void*)(int)System.Runtime.CompilerServices.Unsafe.As<SrcFrame, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame2, 4)));
									}
									break;
								}
								CompressedTimecoded* ptr21 = null;
								CompressedAdaptiveDelta* ptr22 = null;
								if (System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, bool>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 21)))
								{
									ptr22 = global::_003CModule_003E.CompressedAdaptiveDelta_002ECompress(ptr12, &vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E, &compressionSettings);
									if (ptr22 != null && System.Runtime.CompilerServices.Unsafe.As<W3DAnimation.CompressionSettings, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref compressionSettings, 20)) == 0)
									{
										goto IL_0642;
									}
								}
								ptr21 = global::_003CModule_003E.CompressedTimecoded_002ECompress(ptr12, &vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E, &compressionSettings);
								goto IL_0642;
								IL_0642:
								if (ptr22 != null && (ptr21 == null || global::_003CModule_003E.CompressedTimecoded_002EEstimateSize(ptr21) >= global::_003CModule_003E.CompressedAdaptiveDelta_002EEstimateSize(ptr22)))
								{
									global::_003CModule_003E.CompressedAdaptiveDelta_002EWriteOut(ptr22, &tracker2, &endianHelper, (AnimationChannelDelta**)(int)(num3 * 4 + (uint)(*(int*)ptr10)));
								}
								else
								{
									global::_003CModule_003E.CompressedTimecoded_002EWriteOut(ptr21, &tracker2, &endianHelper, (AnimationChannelTimecoded**)(int)(num3 * 4 + (uint)(*(int*)ptr10)));
								}
								if (ptr21 != null)
								{
									global::_003CModule_003E.CompressedTimecoded_002E__delDtor(ptr21, 1u);
								}
								CompressedAdaptiveDelta* ptr23 = ptr22;
								if (ptr22 != null)
								{
									try
									{
										vector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E* ptr24 = (vector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr22 + 32);
										int num22 = *(int*)ptr24;
										if (num22 != 0)
										{
											global::_003CModule_003E.delete_005B_005D((void*)num22);
										}
									}
									catch
									{
										//try-fault
										global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)ptr23 + 12);
										throw;
									}
									vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr25 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr22 + 12);
									int num23 = *(int*)ptr25;
									if (num23 != 0)
									{
										global::_003CModule_003E.delete_005B_005D((void*)num23);
									}
									global::_003CModule_003E.delete(ptr22);
								}
								goto IL_06ea;
								end_IL_02f1:;
							}
							catch
							{
								//try-fault
								global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E);
								throw;
							}
							try
							{
								throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Internal error: unknown animation channel type {0}", *(int*)ptr12);
							}
							catch
							{
								//try-fault
								global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E);
								throw;
							}
							IL_078e:
							try
							{
								try
								{
									throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Internal error: unknown animation channel usage {0}", ((int*)ptr12)[1]);
								}
								catch
								{
									//try-fault
									global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<SrcFrame*, void>)(&global::_003CModule_003E.SrcFrame_002E_007Bdtor_007D), &srcFrame2);
									throw;
								}
							}
							catch
							{
								//try-fault
								global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E);
								throw;
							}
							IL_06ea:
							try
							{
								SrcFrame* ptr26 = (SrcFrame*)(int)System.Runtime.CompilerServices.Unsafe.As<vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E, 4));
								SrcFrame* ptr27 = (SrcFrame*)(int)(*(uint*)(&vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E));
								if ((uint)(*(int*)(&vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E)) < (uint)System.Runtime.CompilerServices.Unsafe.As<vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E, 4)))
								{
									do
									{
										vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr28 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr27 + 4);
										int num24 = *(int*)ptr28;
										if (num24 != 0)
										{
											global::_003CModule_003E.delete_005B_005D((void*)num24);
										}
										ptr27 = (SrcFrame*)((byte*)ptr27 + 24);
									}
									while (ptr27 < ptr26);
								}
							}
							catch
							{
								//try-fault
								global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<VectorBase_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002EVectorBase_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E);
								throw;
							}
							if (*(int*)(&vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E) != 0)
							{
								global::_003CModule_003E.delete_005B_005D((void*)(int)(*(uint*)(&vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E)));
							}
							num3++;
							num4 = *(uint*)ptr9;
						}
						while (num3 < num4);
					}
					try
					{
						try
						{
							System.Runtime.CompilerServices.Unsafe.As<Tracker, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref tracker2, 8)) -= 4;
							assetBuffer = new AssetBuffer();
							FinalizeTracker(&tracker2, &endianHelper, assetBuffer);
						}
						catch
						{
							//try-fault
							global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<SrcFrame*, void>)(&global::_003CModule_003E.SrcFrame_002E_007Bdtor_007D), &srcFrame2);
							throw;
						}
					}
					catch
					{
						//try-fault
						global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*, void>)(&global::_003CModule_003E.eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E);
						throw;
					}
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&global::_003CModule_003E.Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder);
					throw;
				}
				if (ptr5 != null)
				{
					Node* intPtr = ptr5;
					((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)intPtr)))((nint)intPtr);
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
		return assetBuffer;
	}

	public unsafe AssetBuffer HandleMesh(InstanceDeclaration declaration)
	{
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
		AssetBuffer assetBuffer;
		try
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker tracker);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out W3DMesh* ptr4);
			global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AW3DMesh_003E(&tracker, &ptr4);
			try
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out EndianHelper endianHelper);
				*(bool*)(&endianHelper) = flag;
				if (ptr3 != null)
				{
					W3DMeshPipelineVertexData* ptr5 = null;
					System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker tracker2);
					global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AW3DMeshPipelineVertexData_003E(&tracker2, &ptr5);
					try
					{
						global::_003CModule_003E.Marshaler_002EMarshal((INode*)ptr3, ptr5, &tracker2);
						global::_003CModule_003E.Marshaler_002EMarshal((INode*)ptr3, ptr4, &tracker);
						global::_003CModule_003E.Marshaler_002EEndianCorrect(ptr4, &endianHelper);
						W3DMesh* ptr6 = (W3DMesh*)((byte*)ptr4 + 4);
						W3DMeshVertexData** pointerLocation = (W3DMeshVertexData**)ptr6;
						global::_003CModule_003E.Relo_002ETracker_002EPush(&tracker, (void**)pointerLocation, 28u, 1u);
						global::_003CModule_003E.W3DMeshProcessor_002EBuildVertexData(ptr5, (W3DMeshVertexData*)(int)(*(uint*)ptr6), &tracker, &endianHelper, m_Platform);
						System.Runtime.CompilerServices.Unsafe.As<Tracker, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref tracker, 8)) -= 4;
					}
					catch
					{
						//try-fault
						global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Tracker*, void>)(&global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D), &tracker2);
						throw;
					}
					global::_003CModule_003E.Relo_002ETracker_002E_007Bdtor_007D(&tracker2);
				}
				assetBuffer = new AssetBuffer();
				FinalizeTracker(&tracker, &endianHelper, assetBuffer);
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

	public virtual AssetBuffer ProcessInstance(InstanceDeclaration declaration)
	{
		switch (declaration.Handle.TypeId)
		{
		default:
			Console.WriteLine("Warning: Couldn't process {0}. No matching handler found.", declaration);
			return null;
		case 3266421346u:
			return HandleMesh(declaration);
		case 608742960u:
			return HandleAnimation(declaration);
		}
	}

	public virtual uint GetAllTypesHash()
	{
		return 1424942948u;
	}

	public virtual uint GetVersionNumber()
	{
		uint result = 0u;
		switch (m_Platform)
		{
		case TargetPlatform.Xbox360:
			result = 9u;
			break;
		case TargetPlatform.Win32:
			result = 8u;
			break;
		case TargetPlatform.PlayStation3:
			result = 11u;
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
		case 3266421346u:
			extendedTypeInformation.ProcessingHash = GetVersionNumber() ^ 0xCD8A089Cu;
			extendedTypeInformation.TypeHash = 3448375452u;
			extendedTypeInformation.TypeName = new string(global::_003CModule_003E._003FTypeName_0040W3DMesh_0040SageBinaryData_0040_00402PBDB);
			break;
		case 608742960u:
			extendedTypeInformation.ProcessingHash = GetVersionNumber() ^ 0xD6505CE1u;
			extendedTypeInformation.TypeHash = 1430609896u;
			extendedTypeInformation.TypeName = new string(global::_003CModule_003E._003FTypeName_0040W3DAnimation_0040SageBinaryData_0040_00402PBDB);
			break;
		}
		return extendedTypeInformation;
	}

	protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool P_0)
	{
		if (!P_0)
		{
			try
			{
			}
			finally
			{
				base.Finalize();
			}
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
