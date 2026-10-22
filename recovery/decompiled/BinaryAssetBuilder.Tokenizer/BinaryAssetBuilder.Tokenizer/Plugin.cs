using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BinaryAssetBuilder.Core;
using Endian;
using Marshaler;
using Relo;
using SageBinaryData;
using Tokenized;

namespace BinaryAssetBuilder.Tokenizer;

public class Plugin : IAssetBuilderPlugin
{
	private static Tracer _Tracer = Tracer.GetTracer("Tokenizer", "Tokenizes XML data into binary data structures.");

	private static int TokenizerVersion = 4100;

	private static int PlayStation3Version = 2;

	private static int Xbox360Version = 1;

	private static int Win32Version = 0;

	private TargetPlatform _Platform;

	public virtual void Initialize(object configSection, TargetPlatform platform)
	{
		_Platform = platform;
	}

	public virtual void ReInitialize(object configSection, TargetPlatform platform)
	{
		Initialize(configSection, platform);
	}

	public unsafe void HandleType_003CSageBinaryData_003A_003AWeaponTemplate_003E(gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* declaration, gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E* buffer)
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
					System.Runtime.CompilerServices.Unsafe.SkipInit(out WeaponTemplate* ptr4);
					global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AWeaponTemplate_003E(&tracker, &ptr4);
					try
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker tracker2);
						global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D(&tracker2);
						try
						{
							global::_003CModule_003E.Marshaler_002EMarshal((INode*)ptr3, ptr4, &tracker);
							System.Runtime.CompilerServices.Unsafe.SkipInit(out void* ptr5);
							global::_003CModule_003E.Tokenizer_002ETokenize(&ptr5, ptr4, &tracker2);
							System.Runtime.CompilerServices.Unsafe.SkipInit(out EndianHelper endianHelper);
							*(bool*)(&endianHelper) = flag;
							global::_003CModule_003E.Marshaler_002EEndianCorrect(ptr4, &endianHelper);
							System.Runtime.CompilerServices.Unsafe.SkipInit(out Relo.Chunk chunk);
							*(int*)(&chunk) = 0;
							System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4)) = 0;
							System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 8)) = 0;
							System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 12)) = 0;
							System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 16)) = 0;
							System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 20)) = 0;
							try
							{
								global::_003CModule_003E.Relo_002ETracker_002EMakeRelocatable(&tracker2, &chunk, &endianHelper, 0u);
								global::_003CModule_003E.Relo_002ETracker_002EFixupMemberOffsets(&tracker2, &tracker, &chunk, &endianHelper);
								System.Runtime.CompilerServices.Unsafe.SkipInit(out Relo.Chunk chunk2);
								*(int*)(&chunk2) = 0;
								System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 4)) = 0;
								System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 8)) = 0;
								System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 12)) = 0;
								System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 16)) = 0;
								System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 20)) = 0;
								try
								{
									global::_003CModule_003E.Relo_002ETracker_002EMakeRelocatable(&tracker, &chunk2, &endianHelper, System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4)));
									IntPtr intPtr3 = new IntPtr((void*)(int)(*(uint*)buffer));
									((AssetBuffer)((GCHandle)intPtr3).Target).InstanceData = new byte[System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 4)) + System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4))];
									if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4)) != 0)
									{
										ProcTableElement* ptr6 = (ProcTableElement*)(int)((uint)System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4)) / 12u * 12 + (uint)(*(int*)(&chunk)));
										if ((ProcTableElement*)(*(int*)(&chunk)) != ptr6)
										{
											int num = *(int*)(&chunk) + 6;
											System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
											System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num3);
											do
											{
												if (flag)
												{
													*(sbyte*)(&num2) = *(sbyte*)(num + 1);
													System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1)) = *(sbyte*)num;
													System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2)) = *(sbyte*)(num - 1);
													System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3)) = *(sbyte*)(num - 2);
													*(uint*)(num - 2) = num2;
													*(sbyte*)(&num3) = *(sbyte*)(num - 3);
													System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 1)) = *(sbyte*)(num - 4);
													System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 2)) = *(sbyte*)(num - 5);
													System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 3)) = *(sbyte*)(num - 6);
													*(uint*)(num - 6) = num3;
												}
												num += 12;
											}
											while ((ProcTableElement*)(num - 6) != ptr6);
										}
										IntPtr intPtr4 = new IntPtr((void*)(int)(*(uint*)buffer));
										fixed (byte* ptr7 = &((AssetBuffer)((GCHandle)intPtr4).Target).InstanceData[0])
										{
											try
											{
												// IL cpblk instruction
												System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr7, *(int*)(&chunk), System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4)));
											}
											catch
											{
												//try-fault
												ptr7 = null;
												throw;
											}
										}
									}
									if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 4)) != 0)
									{
										IntPtr intPtr5 = new IntPtr((void*)(int)(*(uint*)buffer));
										fixed (byte* ptr8 = &((AssetBuffer)((GCHandle)intPtr5).Target).InstanceData[System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4))])
										{
											try
											{
												// IL cpblk instruction
												System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr8, *(int*)(&chunk2), System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 4)));
											}
											catch
											{
												//try-fault
												ptr8 = null;
												throw;
											}
										}
									}
									IntPtr intPtr6 = new IntPtr((void*)(int)(*(uint*)buffer));
									((AssetBuffer)((GCHandle)intPtr6).Target).RelocationData = new byte[System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 12))];
									if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 12)) != 0)
									{
										IntPtr intPtr7 = new IntPtr((void*)(int)(*(uint*)buffer));
										fixed (byte* ptr9 = &((AssetBuffer)((GCHandle)intPtr7).Target).RelocationData[0])
										{
											try
											{
												// IL cpblk instruction
												System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr9, System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 8)), System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 12)));
											}
											catch
											{
												//try-fault
												ptr9 = null;
												throw;
											}
										}
									}
									IntPtr intPtr8 = new IntPtr((void*)(int)(*(uint*)buffer));
									((AssetBuffer)((GCHandle)intPtr8).Target).ImportsData = new byte[0];
								}
								catch
								{
									//try-fault
									global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Relo.Chunk*, void>)(&global::_003CModule_003E.Relo_002EChunk_002E_007Bdtor_007D), &chunk2);
									throw;
								}
								if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 8)) != 0)
								{
									global::_003CModule_003E.free((void*)(int)System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 8)));
								}
								if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 16)) != 0)
								{
									global::_003CModule_003E.free((void*)(int)System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 16)));
								}
								if (*(int*)(&chunk2) != 0)
								{
									global::_003CModule_003E.free((void*)(int)(*(uint*)(&chunk2)));
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
					Node* intPtr9 = ptr3;
					((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)intPtr9)))((nint)intPtr9);
				}
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D), declaration);
				throw;
			}
			IntPtr intPtr10 = new IntPtr((void*)(int)(*(uint*)declaration));
			((GCHandle)intPtr10).Free();
			*(int*)declaration = 0;
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E_002E_007Bdtor_007D), buffer);
			throw;
		}
		IntPtr intPtr11 = new IntPtr((void*)(int)(*(uint*)buffer));
		((GCHandle)intPtr11).Free();
		*(int*)buffer = 0;
	}

	public unsafe void HandleType_003CSageBinaryData_003A_003AArmorTemplate_003E(gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* declaration, gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E* buffer)
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
					System.Runtime.CompilerServices.Unsafe.SkipInit(out ArmorTemplate* ptr4);
					global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AArmorTemplate_003E(&tracker, &ptr4);
					try
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker tracker2);
						global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D(&tracker2);
						try
						{
							global::_003CModule_003E.Marshaler_002EMarshal((INode*)ptr3, ptr4, &tracker);
							System.Runtime.CompilerServices.Unsafe.SkipInit(out void* ptr5);
							global::_003CModule_003E.Tokenizer_002ETokenize(&ptr5, ptr4, &tracker2);
							System.Runtime.CompilerServices.Unsafe.SkipInit(out EndianHelper endianHelper);
							*(bool*)(&endianHelper) = flag;
							global::_003CModule_003E.Marshaler_002EEndianCorrect(ptr4, &endianHelper);
							System.Runtime.CompilerServices.Unsafe.SkipInit(out Relo.Chunk chunk);
							*(int*)(&chunk) = 0;
							System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4)) = 0;
							System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 8)) = 0;
							System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 12)) = 0;
							System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 16)) = 0;
							System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 20)) = 0;
							try
							{
								global::_003CModule_003E.Relo_002ETracker_002EMakeRelocatable(&tracker2, &chunk, &endianHelper, 0u);
								global::_003CModule_003E.Relo_002ETracker_002EFixupMemberOffsets(&tracker2, &tracker, &chunk, &endianHelper);
								System.Runtime.CompilerServices.Unsafe.SkipInit(out Relo.Chunk chunk2);
								*(int*)(&chunk2) = 0;
								System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 4)) = 0;
								System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 8)) = 0;
								System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 12)) = 0;
								System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 16)) = 0;
								System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 20)) = 0;
								try
								{
									global::_003CModule_003E.Relo_002ETracker_002EMakeRelocatable(&tracker, &chunk2, &endianHelper, System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4)));
									IntPtr intPtr3 = new IntPtr((void*)(int)(*(uint*)buffer));
									((AssetBuffer)((GCHandle)intPtr3).Target).InstanceData = new byte[System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 4)) + System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4))];
									if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4)) != 0)
									{
										ProcTableElement* ptr6 = (ProcTableElement*)(int)((uint)System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4)) / 12u * 12 + (uint)(*(int*)(&chunk)));
										if ((ProcTableElement*)(*(int*)(&chunk)) != ptr6)
										{
											int num = *(int*)(&chunk) + 6;
											System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
											System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num3);
											do
											{
												if (flag)
												{
													*(sbyte*)(&num2) = *(sbyte*)(num + 1);
													System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1)) = *(sbyte*)num;
													System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2)) = *(sbyte*)(num - 1);
													System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3)) = *(sbyte*)(num - 2);
													*(uint*)(num - 2) = num2;
													*(sbyte*)(&num3) = *(sbyte*)(num - 3);
													System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 1)) = *(sbyte*)(num - 4);
													System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 2)) = *(sbyte*)(num - 5);
													System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 3)) = *(sbyte*)(num - 6);
													*(uint*)(num - 6) = num3;
												}
												num += 12;
											}
											while ((ProcTableElement*)(num - 6) != ptr6);
										}
										IntPtr intPtr4 = new IntPtr((void*)(int)(*(uint*)buffer));
										fixed (byte* ptr7 = &((AssetBuffer)((GCHandle)intPtr4).Target).InstanceData[0])
										{
											try
											{
												// IL cpblk instruction
												System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr7, *(int*)(&chunk), System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4)));
											}
											catch
											{
												//try-fault
												ptr7 = null;
												throw;
											}
										}
									}
									if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 4)) != 0)
									{
										IntPtr intPtr5 = new IntPtr((void*)(int)(*(uint*)buffer));
										fixed (byte* ptr8 = &((AssetBuffer)((GCHandle)intPtr5).Target).InstanceData[System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4))])
										{
											try
											{
												// IL cpblk instruction
												System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr8, *(int*)(&chunk2), System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 4)));
											}
											catch
											{
												//try-fault
												ptr8 = null;
												throw;
											}
										}
									}
									IntPtr intPtr6 = new IntPtr((void*)(int)(*(uint*)buffer));
									((AssetBuffer)((GCHandle)intPtr6).Target).RelocationData = new byte[System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 12))];
									if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 12)) != 0)
									{
										IntPtr intPtr7 = new IntPtr((void*)(int)(*(uint*)buffer));
										fixed (byte* ptr9 = &((AssetBuffer)((GCHandle)intPtr7).Target).RelocationData[0])
										{
											try
											{
												// IL cpblk instruction
												System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr9, System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 8)), System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 12)));
											}
											catch
											{
												//try-fault
												ptr9 = null;
												throw;
											}
										}
									}
									IntPtr intPtr8 = new IntPtr((void*)(int)(*(uint*)buffer));
									((AssetBuffer)((GCHandle)intPtr8).Target).ImportsData = new byte[0];
								}
								catch
								{
									//try-fault
									global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Relo.Chunk*, void>)(&global::_003CModule_003E.Relo_002EChunk_002E_007Bdtor_007D), &chunk2);
									throw;
								}
								if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 8)) != 0)
								{
									global::_003CModule_003E.free((void*)(int)System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 8)));
								}
								if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 16)) != 0)
								{
									global::_003CModule_003E.free((void*)(int)System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 16)));
								}
								if (*(int*)(&chunk2) != 0)
								{
									global::_003CModule_003E.free((void*)(int)(*(uint*)(&chunk2)));
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
					Node* intPtr9 = ptr3;
					((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)intPtr9)))((nint)intPtr9);
				}
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D), declaration);
				throw;
			}
			IntPtr intPtr10 = new IntPtr((void*)(int)(*(uint*)declaration));
			((GCHandle)intPtr10).Free();
			*(int*)declaration = 0;
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E_002E_007Bdtor_007D), buffer);
			throw;
		}
		IntPtr intPtr11 = new IntPtr((void*)(int)(*(uint*)buffer));
		((GCHandle)intPtr11).Free();
		*(int*)buffer = 0;
	}

	public unsafe void HandleType_003CSageBinaryData_003A_003AGameObject_003E(gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* declaration, gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E* buffer)
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
					System.Runtime.CompilerServices.Unsafe.SkipInit(out GameObject* ptr4);
					global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AGameObject_003E(&tracker, &ptr4);
					try
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker tracker2);
						global::_003CModule_003E.Relo_002ETracker_002E_007Bctor_007D(&tracker2);
						try
						{
							global::_003CModule_003E.Marshaler_002EMarshal((INode*)ptr3, ptr4, &tracker);
							System.Runtime.CompilerServices.Unsafe.SkipInit(out void* ptr5);
							global::_003CModule_003E.Tokenizer_002ETokenize(&ptr5, ptr4, &tracker2);
							System.Runtime.CompilerServices.Unsafe.SkipInit(out EndianHelper endianHelper);
							*(bool*)(&endianHelper) = flag;
							global::_003CModule_003E.Marshaler_002EEndianCorrect(ptr4, &endianHelper);
							System.Runtime.CompilerServices.Unsafe.SkipInit(out Relo.Chunk chunk);
							*(int*)(&chunk) = 0;
							System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4)) = 0;
							System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 8)) = 0;
							System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 12)) = 0;
							System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 16)) = 0;
							System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 20)) = 0;
							try
							{
								global::_003CModule_003E.Relo_002ETracker_002EMakeRelocatable(&tracker2, &chunk, &endianHelper, 0u);
								global::_003CModule_003E.Relo_002ETracker_002EFixupMemberOffsets(&tracker2, &tracker, &chunk, &endianHelper);
								System.Runtime.CompilerServices.Unsafe.SkipInit(out Relo.Chunk chunk2);
								*(int*)(&chunk2) = 0;
								System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 4)) = 0;
								System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 8)) = 0;
								System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 12)) = 0;
								System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 16)) = 0;
								System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 20)) = 0;
								try
								{
									global::_003CModule_003E.Relo_002ETracker_002EMakeRelocatable(&tracker, &chunk2, &endianHelper, System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4)));
									IntPtr intPtr3 = new IntPtr((void*)(int)(*(uint*)buffer));
									((AssetBuffer)((GCHandle)intPtr3).Target).InstanceData = new byte[System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 4)) + System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4))];
									if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4)) != 0)
									{
										ProcTableElement* ptr6 = (ProcTableElement*)(int)((uint)System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4)) / 12u * 12 + (uint)(*(int*)(&chunk)));
										if ((ProcTableElement*)(*(int*)(&chunk)) != ptr6)
										{
											int num = *(int*)(&chunk) + 6;
											System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
											System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num3);
											do
											{
												if (flag)
												{
													*(sbyte*)(&num2) = *(sbyte*)(num + 1);
													System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1)) = *(sbyte*)num;
													System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2)) = *(sbyte*)(num - 1);
													System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3)) = *(sbyte*)(num - 2);
													*(uint*)(num - 2) = num2;
													*(sbyte*)(&num3) = *(sbyte*)(num - 3);
													System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 1)) = *(sbyte*)(num - 4);
													System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 2)) = *(sbyte*)(num - 5);
													System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 3)) = *(sbyte*)(num - 6);
													*(uint*)(num - 6) = num3;
												}
												num += 12;
											}
											while ((ProcTableElement*)(num - 6) != ptr6);
										}
										IntPtr intPtr4 = new IntPtr((void*)(int)(*(uint*)buffer));
										fixed (byte* ptr7 = &((AssetBuffer)((GCHandle)intPtr4).Target).InstanceData[0])
										{
											try
											{
												// IL cpblk instruction
												System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr7, *(int*)(&chunk), System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4)));
											}
											catch
											{
												//try-fault
												ptr7 = null;
												throw;
											}
										}
									}
									if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 4)) != 0)
									{
										IntPtr intPtr5 = new IntPtr((void*)(int)(*(uint*)buffer));
										fixed (byte* ptr8 = &((AssetBuffer)((GCHandle)intPtr5).Target).InstanceData[System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk, 4))])
										{
											try
											{
												// IL cpblk instruction
												System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr8, *(int*)(&chunk2), System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 4)));
											}
											catch
											{
												//try-fault
												ptr8 = null;
												throw;
											}
										}
									}
									IntPtr intPtr6 = new IntPtr((void*)(int)(*(uint*)buffer));
									((AssetBuffer)((GCHandle)intPtr6).Target).RelocationData = new byte[System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 12))];
									if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 12)) != 0)
									{
										IntPtr intPtr7 = new IntPtr((void*)(int)(*(uint*)buffer));
										fixed (byte* ptr9 = &((AssetBuffer)((GCHandle)intPtr7).Target).RelocationData[0])
										{
											try
											{
												// IL cpblk instruction
												System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr9, System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 8)), System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 12)));
											}
											catch
											{
												//try-fault
												ptr9 = null;
												throw;
											}
										}
									}
									IntPtr intPtr8 = new IntPtr((void*)(int)(*(uint*)buffer));
									((AssetBuffer)((GCHandle)intPtr8).Target).ImportsData = new byte[0];
								}
								catch
								{
									//try-fault
									global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<Relo.Chunk*, void>)(&global::_003CModule_003E.Relo_002EChunk_002E_007Bdtor_007D), &chunk2);
									throw;
								}
								if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 8)) != 0)
								{
									global::_003CModule_003E.free((void*)(int)System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 8)));
								}
								if (System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 16)) != 0)
								{
									global::_003CModule_003E.free((void*)(int)System.Runtime.CompilerServices.Unsafe.As<Relo.Chunk, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref chunk2, 16)));
								}
								if (*(int*)(&chunk2) != 0)
								{
									global::_003CModule_003E.free((void*)(int)(*(uint*)(&chunk2)));
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
					Node* intPtr9 = ptr3;
					((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)intPtr9)))((nint)intPtr9);
				}
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D), declaration);
				throw;
			}
			IntPtr intPtr10 = new IntPtr((void*)(int)(*(uint*)declaration));
			((GCHandle)intPtr10).Free();
			*(int*)declaration = 0;
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E*, void>)(&global::_003CModule_003E.gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E_002E_007Bdtor_007D), buffer);
			throw;
		}
		IntPtr intPtr11 = new IntPtr((void*)(int)(*(uint*)buffer));
		((GCHandle)intPtr11).Free();
		*(int*)buffer = 0;
	}

	public unsafe virtual AssetBuffer ProcessInstance(InstanceDeclaration declaration)
	{
		AssetBuffer assetBuffer = new AssetBuffer();
		switch (declaration.Handle.TypeId)
		{
		default:
			return null;
		case 2496977262u:
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
			HandleType_003CSageBinaryData_003A_003AWeaponTemplate_003E(&obj4, &obj3);
			break;
		}
		case 2486173485u:
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
			HandleType_003CSageBinaryData_003A_003AGameObject_003E(&obj6, &obj5);
			break;
		}
		case 980180622u:
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
			HandleType_003CSageBinaryData_003A_003AArmorTemplate_003E(&obj2, &obj);
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
		uint result = 4100u;
		switch (_Platform)
		{
		case TargetPlatform.PlayStation3:
			result = 4102u;
			break;
		case TargetPlatform.Xbox360:
			result = 4101u;
			break;
		}
		return result;
	}

	public unsafe virtual ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
	{
		ExtendedTypeInformation extendedTypeInformation = new ExtendedTypeInformation();
		extendedTypeInformation.HasCustomData = false;
		extendedTypeInformation.TypeId = typeId;
		extendedTypeInformation.Tokenized = true;
		uint versionNumber = GetVersionNumber();
		switch (typeId)
		{
		case 2496977262u:
			extendedTypeInformation.TypeName = new string(global::_003CModule_003E._003FTypeName_0040WeaponTemplate_0040SageBinaryData_0040_00402PBDB);
			extendedTypeInformation.ProcessingHash = versionNumber + 216405646;
			extendedTypeInformation.TypeHash = 216405646u;
			break;
		case 2486173485u:
			extendedTypeInformation.TypeName = new string(global::_003CModule_003E._003FTypeName_0040GameObject_0040SageBinaryData_0040_00402PBDB);
			extendedTypeInformation.ProcessingHash = versionNumber - 879949440;
			extendedTypeInformation.TypeHash = 3415017856u;
			break;
		case 980180622u:
			extendedTypeInformation.TypeName = new string(global::_003CModule_003E._003FTypeName_0040ArmorTemplate_0040SageBinaryData_0040_00402PBDB);
			extendedTypeInformation.ProcessingHash = versionNumber + 1679479598;
			extendedTypeInformation.TypeHash = 1679479598u;
			break;
		}
		return extendedTypeInformation;
	}
}
