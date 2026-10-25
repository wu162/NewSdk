using System.Runtime.InteropServices;
using AssetStream;

namespace BinaryAssetBuilder.Utility;

public class AssetHeader : StructWrapper_003CAssetStream_003A_003AAssetHeader_003E
{
	public unsafe int RelocationDataSize
	{
		get
		{
			return ((int*)m_Data)[5];
		}
		set
		{
			((int*)m_Data)[5] = value;
		}
	}

	public unsafe int InstanceDataSize
	{
		get
		{
			return ((int*)m_Data)[4];
		}
		set
		{
			((int*)m_Data)[4] = value;
		}
	}

	public unsafe int ImportsDataSize
	{
		get
		{
			return ((int*)m_Data)[6];
		}
		set
		{
			((int*)m_Data)[6] = value;
		}
	}

	public unsafe uint TypeHash
	{
		get
		{
			return ((uint*)m_Data)[2];
		}
		set
		{
			((int*)m_Data)[2] = (int)value;
		}
	}

	public unsafe uint InstanceHash
	{
		get
		{
			return ((uint*)m_Data)[3];
		}
		set
		{
			((int*)m_Data)[3] = (int)value;
		}
	}

	public unsafe uint InstanceId
	{
		get
		{
			return ((uint*)m_Data)[1];
		}
		set
		{
			((int*)m_Data)[1] = (int)value;
		}
	}

	public unsafe uint TypeId
	{
		get
		{
			return *(uint*)m_Data;
		}
		set
		{
			*(uint*)m_Data = value;
		}
	}

	protected unsafe override void Swap()
	{
		AssetStream.AssetHeader* data = m_Data;
		uint num = *(uint*)data;
		*(uint*)data = (((num & 0xFF0000) | (num >> 16)) >> 8) | (((num & 0xFF00) | (num << 16)) << 8);
		AssetStream.AssetHeader* ptr = (AssetStream.AssetHeader*)((byte*)m_Data + 4);
		uint num2 = *(uint*)ptr;
		*(uint*)ptr = (((num2 & 0xFF0000) | (num2 >> 16)) >> 8) | (((num2 & 0xFF00) | (num2 << 16)) << 8);
		AssetStream.AssetHeader* ptr2 = (AssetStream.AssetHeader*)((byte*)m_Data + 24);
		uint num3 = *(uint*)ptr2;
		*(uint*)ptr2 = (((num3 & 0xFF0000) | (num3 >> 16)) >> 8) | (((num3 & 0xFF00) | (num3 << 16)) << 8);
		AssetStream.AssetHeader* ptr3 = (AssetStream.AssetHeader*)((byte*)m_Data + 16);
		uint num4 = *(uint*)ptr3;
		*(uint*)ptr3 = (((num4 & 0xFF0000) | (num4 >> 16)) >> 8) | (((num4 & 0xFF00) | (num4 << 16)) << 8);
		AssetStream.AssetHeader* ptr4 = (AssetStream.AssetHeader*)((byte*)m_Data + 12);
		uint num5 = *(uint*)ptr4;
		*(uint*)ptr4 = (((num5 & 0xFF0000) | (num5 >> 16)) >> 8) | (((num5 & 0xFF00) | (num5 << 16)) << 8);
		AssetStream.AssetHeader* ptr5 = (AssetStream.AssetHeader*)((byte*)m_Data + 20);
		uint num6 = *(uint*)ptr5;
		*(uint*)ptr5 = (((num6 & 0xFF0000) | (num6 >> 16)) >> 8) | (((num6 & 0xFF00) | (num6 << 16)) << 8);
		AssetStream.AssetHeader* ptr6 = (AssetStream.AssetHeader*)((byte*)m_Data + 8);
		uint num7 = *(uint*)ptr6;
		*(uint*)ptr6 = (((num7 & 0xFF0000) | (num7 >> 16)) >> 8) | (((num7 & 0xFF00) | (num7 << 16)) << 8);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool IsValidFileLength(long fileLength)
	{
		AssetStream.AssetHeader* data = m_Data;
		return (byte)(((uint)(((int*)data)[4] + (((int*)data)[6] + ((int*)data)[5]) + 32) == fileLength) ? 1u : 0u) != 0;
	}

	public unsafe AssetHeader()
	{ }

}

