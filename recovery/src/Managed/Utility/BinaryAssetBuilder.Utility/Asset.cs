using System;
using System.IO;
using System.Runtime.InteropServices;
using AssetStream;

namespace BinaryAssetBuilder.Utility;

public class Asset
{
	private unsafe AssetStream.AssetEntry* m_AssetEntry;

	private ReferenceHandle[] m_ExternalReferences;

	private string[] m_Id;

	private string m_QualifiedName;

	private bool m_HasErrors;

	private string m_Message;

	private string m_Source;

	private string m_FileBasePath;

	private string m_CdataPath;

	private int m_Index;

	private int m_LinkedInstanceOffset;

	private int m_LinkedRelocationOffset;

	private int m_LinkedImportsOffset;

	private Manifest m_SourceManifest;

	public Manifest SourceManifest => m_SourceManifest;

	public unsafe bool Tokenized
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return (byte)((((int*)m_AssetEntry)[11] != 0) ? 1u : 0u) != 0;
		}
	}

	public int LinkedImportsOffset => m_LinkedImportsOffset;

	public int LinkedRelocationOffset => m_LinkedRelocationOffset;

	public int LinkedInstanceOffset => m_LinkedInstanceOffset;

	public unsafe uint InstanceHash => ((uint*)m_AssetEntry)[3];

	public unsafe uint InstanceId => ((uint*)m_AssetEntry)[1];

	public unsafe uint TypeHash => ((uint*)m_AssetEntry)[2];

	public unsafe uint TypeId => *(uint*)m_AssetEntry;

	public unsafe int RelocationDataSize => ((int*)m_AssetEntry)[9];

	public unsafe int ImportsDataSize => ((int*)m_AssetEntry)[10];

	public unsafe int InstanceDataSize => ((int*)m_AssetEntry)[8];

	public int Index => m_Index;

	public string TypeName => m_Id[0];

	public string InstanceName => m_Id[1];

	public string QualifiedName => m_QualifiedName;

	public string Message => m_Message;

	public string Source => m_Source;

	public bool HasErrors
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_HasErrors;
		}
	}

	public string CdataPath => m_CdataPath;

	public string FileBasePath => m_FileBasePath;

	public ReferenceHandle[] ExternalReferences => m_ExternalReferences;

	public unsafe Asset(int index, string basePath, AssetStream.AssetEntry* assetEntry, sbyte* assetName, sbyte* assetSource, ReferenceHandle[] externalReferences, Manifest sourceManifest, int linkedInstanceOffset, int linkedRelocationOffset, int linkedImportsOffset)
	{
		m_AssetEntry = assetEntry;
		m_ExternalReferences = externalReferences;
		m_HasErrors = false;
		m_Message = "";
		m_Source = new string(assetSource);
		m_LinkedInstanceOffset = linkedInstanceOffset;
		m_LinkedRelocationOffset = linkedRelocationOffset;
		m_LinkedImportsOffset = linkedImportsOffset;
		m_SourceManifest = sourceManifest;

		IntPtr ptr = new IntPtr(assetName);
		m_Id = (m_QualifiedName = Marshal.PtrToStringAnsi(ptr)).Split(':');
		m_Index = index;
		object[] array = new object[4];
		AssetStream.AssetEntry* assetEntry2 = m_AssetEntry;
		array[0] = *(uint*)assetEntry2;
		array[1] = ((uint*)assetEntry2)[2];
		array[2] = ((uint*)assetEntry2)[1];
		array[3] = ((uint*)assetEntry2)[3];
		string text = string.Format("{0:x8}.{1:x8}.{2:x8}.{3:x8}", array);
		m_FileBasePath = Path.Combine(Path.Combine(basePath, "assets"), text);
		m_CdataPath = Path.Combine(Path.Combine(basePath, "cdata"), text + ".cdata");
	}

	public void Delete()
	{
		if (File.Exists(m_FileBasePath + ".asset"))
		{
			File.Delete(m_FileBasePath + ".asset");
		}
		Update();
	}

	public void Update()
	{
		if (!File.Exists(m_FileBasePath + ".asset"))
		{
			m_HasErrors |= true;
			m_Message += "Asset file missing. ";
		}
	}
}

