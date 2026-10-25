using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml;
using AssetStream;

namespace BinaryAssetBuilder.Utility;

public class Manifest : IDisposable
{
	private unsafe sbyte* m_pBuffer;

	private unsafe AssetStream.ManifestHeader* m_pHeader;

	private string m_FileName;

	private Asset[] m_Assets;

	private IDictionary<uint, string> m_StringHashes;

	private ReferencedManifest[] m_ReferencedManifests;

	private Manifest m_PatchManifest;

	public unsafe uint MaxInstanceChunkSize
	{
		get
		{
			AssetStream.ManifestHeader* pHeader = m_pHeader;
			if (pHeader == null)
			{
				return 0u;
			}
			return ((uint*)pHeader)[5];
		}
	}

	public unsafe uint MaxRelocationChunkSize
	{
		get
		{
			AssetStream.ManifestHeader* pHeader = m_pHeader;
			if (pHeader == null)
			{
				return 0u;
			}
			return ((uint*)pHeader)[6];
		}
	}

	public unsafe uint MaxImportsChunkSize
	{
		get
		{
			AssetStream.ManifestHeader* pHeader = m_pHeader;
			if (pHeader == null)
			{
				return 0u;
			}
			return ((uint*)pHeader)[7];
		}
	}

	public unsafe uint TotalInstanceDataSize
	{
		get
		{
			AssetStream.ManifestHeader* pHeader = m_pHeader;
			if (pHeader == null)
			{
				return 0u;
			}
			return ((uint*)pHeader)[4];
		}
	}

	public unsafe uint AllTypesHash
	{
		get
		{
			AssetStream.ManifestHeader* pHeader = m_pHeader;
			if (pHeader == null)
			{
				return 0u;
			}
			return ((uint*)pHeader)[2];
		}
	}

	public unsafe uint StreamChecksum
	{
		get
		{
			AssetStream.ManifestHeader* pHeader = m_pHeader;
			if (pHeader == null)
			{
				return 0u;
			}
			return ((uint*)pHeader)[1];
		}
	}

	public unsafe int Version
	{
		get
		{
			AssetStream.ManifestHeader* pHeader = m_pHeader;
			if (pHeader == null)
			{
				return 0;
			}
			return ((ushort*)pHeader)[1];
		}
	}

	public unsafe bool IsBigEndian
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			AssetStream.ManifestHeader* pHeader = m_pHeader;
			if (pHeader == null)
			{
				return false;
			}
			return *(bool*)pHeader;
		}
	}

	public unsafe bool IsLinked
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			AssetStream.ManifestHeader* pHeader = m_pHeader;
			if (pHeader == null)
			{
				return false;
			}
			return ((bool*)pHeader)[1];
		}
	}

	public unsafe int SourceFileNameBufferSize
	{
		get
		{
			AssetStream.ManifestHeader* pHeader = m_pHeader;
			if (pHeader == null)
			{
				return 0;
			}
			return ((int*)pHeader)[11];
		}
	}

	public unsafe int ExternalManifestNameBufferSize
	{
		get
		{
			AssetStream.ManifestHeader* pHeader = m_pHeader;
			if (pHeader == null)
			{
				return 0;
			}
			return ((int*)pHeader)[9];
		}
	}

	public unsafe int ExternalReferenceBufferSize
	{
		get
		{
			AssetStream.ManifestHeader* pHeader = m_pHeader;
			if (pHeader == null)
			{
				return 0;
			}
			return ((int*)pHeader)[8];
		}
	}

	public unsafe int AssetNameBufferSize
	{
		get
		{
			AssetStream.ManifestHeader* pHeader = m_pHeader;
			if (pHeader == null)
			{
				return 0;
			}
			return ((int*)pHeader)[10];
		}
	}

	public unsafe uint AssetCount
	{
		get
		{
			AssetStream.ManifestHeader* pHeader = m_pHeader;
			if (pHeader == null)
			{
				return 0u;
			}
			return ((uint*)pHeader)[3];
		}
	}

	public Asset[] Assets => m_Assets;

	public string FileName => m_FileName;

	public ReferencedManifest[] ReferencedManifests => m_ReferencedManifests;

	public Manifest PatchManifest => m_PatchManifest;

	public unsafe bool IsLoaded
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return (byte)((m_pBuffer != null) ? 1u : 0u) != 0;
		}
	}

	private unsafe void Free()
	{
		m_Assets = null;
		m_FileName = null;
		sbyte* pBuffer = m_pBuffer;
		if (pBuffer != null)
		{
			Marshal.FreeHGlobal((IntPtr)pBuffer);
			m_pBuffer = null;
		}
	}

	public Manifest()
	{
		m_StringHashes = new SortedDictionary<uint, string>();
		m_PatchManifest = null;
	}

	public static Manifest Create(string fileName, string[] patchSearchPaths, [MarshalAs(UnmanagedType.U1)] bool validateAssets)
	{
		Manifest manifest = new Manifest();
		if (manifest.Load(fileName, patchSearchPaths, validateAssets))
		{
			return manifest;
		}
		return null;
	}

	public static Manifest Create(string fileName, string[] patchSearchPaths)
	{
		Manifest manifest = new Manifest();
		return (!manifest.Load(fileName, patchSearchPaths, validateAssets: true)) ? null : manifest;
	}

	public static Manifest Create(string fileName)
	{
		Manifest manifest = new Manifest();
		return (!manifest.Load(fileName, null, validateAssets: true)) ? null : manifest;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool Reload()
	{
		if (string.IsNullOrEmpty(m_FileName))
		{
			return false;
		}
		string fileName = m_FileName;
		return Load(fileName, null, validateAssets: true);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool Load(string fileName, string[] patchSearchPaths, [MarshalAs(UnmanagedType.U1)] bool validateAssets)
	{
		Asset asset = null;
		Free();
		m_FileName = Path.GetFullPath(fileName);
        byte[] bytes;
        try { bytes = File.ReadAllBytes(fileName); }
        catch (IOException) { return false; }
        catch (UnauthorizedAccessException) { return false; }
        m_pBuffer = (sbyte*)Marshal.AllocHGlobal(bytes.Length);
        Marshal.Copy(bytes, 0, (IntPtr)m_pBuffer, bytes.Length);
        {
			string basePath = Path.Combine(Path.GetDirectoryName(m_FileName), Path.GetFileNameWithoutExtension(m_FileName));
			LoadStringHashes(basePath);
			sbyte* ptr3 = (sbyte*)(m_pHeader = (AssetStream.ManifestHeader*)m_pBuffer) + 48;
			AssetStream.ManifestHeader* pHeader = m_pHeader;
			if (*(bool*)pHeader)
			{
				((int*)pHeader)[3] = (int)UtilityEndian.Swap(((uint*)pHeader)[3]);
				byte* num2 = (byte*)m_pHeader + 40;
				*(uint*)num2 = UtilityEndian.Swap(*(uint*)num2);
				byte* num3 = (byte*)m_pHeader + 32;
				*(uint*)num3 = UtilityEndian.Swap(*(uint*)num3);
				byte* num4 = (byte*)m_pHeader + 28;
				*(uint*)num4 = UtilityEndian.Swap(*(uint*)num4);
				byte* num5 = (byte*)m_pHeader + 20;
				*(uint*)num5 = UtilityEndian.Swap(*(uint*)num5);
				byte* num6 = (byte*)m_pHeader + 24;
				*(uint*)num6 = UtilityEndian.Swap(*(uint*)num6);
				byte* num7 = (byte*)m_pHeader + 36;
				*(uint*)num7 = UtilityEndian.Swap(*(uint*)num7);
				byte* num8 = (byte*)m_pHeader + 44;
				*(uint*)num8 = UtilityEndian.Swap(*(uint*)num8);
				byte* num9 = (byte*)m_pHeader + 4;
				*(uint*)num9 = UtilityEndian.Swap(*(uint*)num9);
				byte* num10 = (byte*)m_pHeader + 8;
				*(uint*)num10 = UtilityEndian.Swap(*(uint*)num10);
				byte* num11 = (byte*)m_pHeader + 16;
				*(uint*)num11 = UtilityEndian.Swap(*(uint*)num11);
				byte* num12 = (byte*)m_pHeader + 2;
				*(ushort*)num12 = UtilityEndian.Swap(*(ushort*)num12);
			}
			pHeader = m_pHeader;
			if ((uint)((ushort*)pHeader)[1] < 4u)
			{
				throw new ArgumentException("Can't read manifest. Unsupported file version.");
			}
			m_Assets = new Asset[((int*)pHeader)[3]];
			AssetStream.AssetEntry* ptr4 = (AssetStream.AssetEntry*)ptr3;
			ptr3 = ((int*)pHeader)[3] * 48 + ptr3;
			sbyte* ptr5 = ptr3;
			ptr3 = ((int*)pHeader)[8] + ptr3;
			sbyte* ptr6 = ptr3;
			ptr3 = ((int*)pHeader)[9] + ptr3;
			List<ReferencedManifest> list = new List<ReferencedManifest>();
			if (ptr6 < ptr3)
			{
				do
				{
					bool isPatch = false;
					if ((uint)((ushort*)m_pHeader)[1] >= 4u)
					{
						if (*ptr6 == 2)
						{
							isPatch = true;
						}
						ptr6++;
					}
					string text = new string(ptr6);
					list.Add(new ReferencedManifest(text, isPatch));
					ptr6 = text.Length + ptr6 + 1;
				}
				while (ptr6 < ptr3);
			}
			m_ReferencedManifests = list.ToArray();
			sbyte* ptr7 = ((int*)m_pHeader)[10] + ptr3;
			int num13 = 4;
			int num14 = 4;
			int num15 = 4;
			uint num16 = 0u;
			if (0 < AssetCount)
			{
				AssetStream.AssetEntry* ptr8 = (AssetStream.AssetEntry*)((byte*)ptr4 + 12);
				do
				{
					if (*(bool*)m_pHeader)
					{
						((int*)ptr8)[2] = (int)UtilityEndian.Swap(((uint*)ptr8)[2]);
						((int*)ptr8)[1] = (int)UtilityEndian.Swap(((uint*)ptr8)[1]);
						*((int*)ptr8 - 3) = (int)UtilityEndian.Swap(*((uint*)ptr8 - 3));
						*((int*)ptr8 - 2) = (int)UtilityEndian.Swap(*((uint*)ptr8 - 2));
						((int*)ptr8)[7] = (int)UtilityEndian.Swap(((uint*)ptr8)[7]);
						((int*)ptr8)[5] = (int)UtilityEndian.Swap(((uint*)ptr8)[5]);
						AssetStream.AssetEntry* intPtr = ptr8;
						*(uint*)intPtr = UtilityEndian.Swap(*(uint*)intPtr);
						((int*)ptr8)[3] = (int)UtilityEndian.Swap(((uint*)ptr8)[3]);
						((int*)ptr8)[6] = (int)UtilityEndian.Swap(((uint*)ptr8)[6]);
						((int*)ptr8)[4] = (int)UtilityEndian.Swap(((uint*)ptr8)[4]);
						*((int*)ptr8 - 1) = (int)UtilityEndian.Swap(*((uint*)ptr8 - 1));
					}
					uint num17 = ((uint*)ptr8)[2];
					ReferenceHandle[] array = new ReferenceHandle[num17];
					AssetId* ptr9 = (AssetId*)(((int*)ptr8)[1] + ptr5);
					uint num18 = 0u;
					if (0 < num17)
					{
						do
						{
							if (*(bool*)m_pHeader)
							{
								AssetId* intPtr2 = ptr9;
								*(uint*)intPtr2 = UtilityEndian.Swap(*(uint*)intPtr2);
								((int*)ptr9)[1] = (int)UtilityEndian.Swap(((uint*)ptr9)[1]);
							}
							array[num18] = new ReferenceHandle();
							array[num18].TypeId = *(uint*)ptr9;
							array[num18].InstanceId = ((uint*)ptr9)[1];
							array[num18].TypeName = StringFromHash(*(uint*)ptr9);
							array[num18].InstanceName = StringFromHash(((uint*)ptr9)[1]);
							ReferenceHandle referenceHandle = array[num18];
							array[num18].QualifiedName = $"{referenceHandle.TypeName}:{referenceHandle.InstanceName}";
							num18++;
							ptr9 = (AssetId*)((byte*)ptr9 + 8);
						}
						while (num18 < (uint)((int*)ptr8)[2]);
					}
					Asset[] assets = m_Assets;
					uint num19 = num16;
					assets[num19] = new Asset((int)num19, basePath, (AssetStream.AssetEntry*)((byte*)ptr8 - 12), ((int*)ptr8)[3] + ptr3, ((int*)ptr8)[4] + ptr7, array, this, num13, num14, num15);
					if (validateAssets)
					{
						m_Assets[num16].Update();
					}
					num13 = ((int*)ptr8)[5] + num13;
					num14 = ((int*)ptr8)[6] + num14;
					num15 = ((int*)ptr8)[7] + num15;
					num16++;
					ptr8 = (AssetStream.AssetEntry*)((byte*)ptr8 + 48);
				}
				while (num16 < AssetCount);
			}
			List<string> list2 = new List<string>();
			list2.Add(Path.GetDirectoryName(fileName));
			if (patchSearchPaths != null)
			{
				int num20 = 0;
				if (0 < (nint)patchSearchPaths.LongLength)
				{
					do
					{
						string item = patchSearchPaths[num20];
						list2.Add(item);
						num20++;
					}
					while (num20 < (nint)patchSearchPaths.LongLength);
				}
			}
			Manifest manifest = null;
			ReferencedManifest[] referencedManifests = m_ReferencedManifests;
			int num21 = 0;
			if (0 < (nint)referencedManifests.LongLength)
			{
				do
				{
					ReferencedManifest referencedManifest = referencedManifests[num21];
					if (referencedManifest.IsPatch)
					{
						List<string>.Enumerator enumerator = list2.GetEnumerator();
						if (enumerator.MoveNext())
						{
							do
							{
								string text2 = Path.Combine(enumerator.Current, referencedManifest.Path);
								if (File.Exists(text2))
								{
									manifest = new Manifest();
									manifest.Load(text2, patchSearchPaths);
									break;
								}
							}
							while (enumerator.MoveNext());
						}
						if (manifest == null)
						{
							throw new ArgumentException($"Base patch manifest {referencedManifest.Path} does not exist.");
						}
					}
					num21++;
				}
				while (num21 < (nint)referencedManifests.LongLength);
			}
			if (manifest != null)
			{
				IDictionary<AssetHandle, Asset> dictionary = new SortedDictionary<AssetHandle, Asset>();
				Asset[] assets2 = manifest.m_Assets;
				int num22 = 0;
				if (0 < (nint)assets2.LongLength)
				{
					do
					{
						Asset asset2 = assets2[num22];
						dictionary.Add(new AssetHandle(asset2.TypeId, asset2.InstanceId), asset2);
						num22++;
					}
					while (num22 < (nint)assets2.LongLength);
				}
				int num23 = 0;
				if (0 < (nint)m_Assets.LongLength)
				{
					do
					{
						Asset asset3 = m_Assets[num23];
						if (asset3.InstanceDataSize == 0 && asset3.RelocationDataSize == 0 && asset3.ImportsDataSize == 0)
						{
							asset = null;
							if (dictionary.TryGetValue(new AssetHandle(asset3.TypeId, asset3.InstanceId), out asset))
							{
								m_Assets[num23] = asset;
							}
						}
						num23++;
					}
					while (num23 < (nint)m_Assets.LongLength);
				}
				m_PatchManifest = manifest;
			}
			return true;
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool Load(string fileName, string[] patchSearchPaths)
	{
		return Load(fileName, patchSearchPaths, validateAssets: true);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool Load(string fileName, [MarshalAs(UnmanagedType.U1)] bool validateAssets)
	{
		return Load(fileName, null, validateAssets);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool Load(string fileName)
	{
		return Load(fileName, null, validateAssets: true);
	}

	public void LoadStringHashes(string basePath)
	{
		string text = null;
		m_StringHashes.Clear();
		string text2 = string.Empty;
		if (basePath != null)
		{
			while (basePath.Length > 3)
			{
				string text3 = Path.Combine(basePath, "StringHashes.xml");
				if (!File.Exists(text3))
				{
					basePath = Path.GetDirectoryName(basePath);
					if (basePath == null)
					{
						break;
					}
					continue;
				}
				text2 = text3;
				break;
			}
		}
		if (text2 == string.Empty || !File.Exists(text2))
		{
			return;
		}
		XmlReader xmlReader = XmlReader.Create(text2);
		try
		{
			while (xmlReader.Read())
			{
				if (!(xmlReader.Name != "StringAndHash"))
				{
					uint key = Convert.ToUInt32(xmlReader.GetAttribute("Hash"));
					text = xmlReader.GetAttribute("Text");
					if (!m_StringHashes.ContainsKey(key))
					{
						m_StringHashes.Add(key, text);
					}
				}
			}
		}
		catch (XmlException)
		{
			m_StringHashes.Clear();
		}
		xmlReader.Close();
	}

	public int GetBaseStreamPosition(AssetEntry assetEntry)
	{
		int num = 0;
		Asset[] assets = m_Assets;
		if (0 < (nint)assets.LongLength)
		{
			do
			{
				Asset asset = assets[num];
				if (!IsEquivalent(assetEntry, asset))
				{
					num++;
					assets = m_Assets;
					continue;
				}
				return num;
			}
			while (num < (nint)assets.LongLength);
		}
		return -1;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public static bool IsEquivalent(AssetEntry assetEntry, Asset asset)
	{
		int num = ((assetEntry.InstanceId == asset.InstanceId && assetEntry.InstanceHash == asset.InstanceHash && assetEntry.TypeId == asset.TypeId && (assetEntry.TypeHash == asset.TypeHash || asset.Tokenized)) ? 1 : 0);
		return (byte)num != 0;
	}

	public string StringFromHash(uint hash)
	{
		string value = null;
		if (m_StringHashes.TryGetValue(hash, out value))
		{
			return value;
		}
		uint num = hash;
		return num.ToString("x8");
	}

	private unsafe void _007EManifest()
	{
		m_Assets = null;
		m_FileName = null;
		sbyte* pBuffer = m_pBuffer;
		if (pBuffer != null)
		{
			Marshal.FreeHGlobal((IntPtr)pBuffer);
			m_pBuffer = null;
		}
	}

	protected unsafe virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool P_0)
	{
		if (P_0)
		{
			m_Assets = null;
			m_FileName = null;
			sbyte* pBuffer = m_pBuffer;
			if (pBuffer != null)
			{
				Marshal.FreeHGlobal((IntPtr)pBuffer);
				m_pBuffer = null;
			}
		}
		else
		{
			// Original Object.Finalize has no user-visible effect here.
		}
	}

	public virtual sealed void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}

