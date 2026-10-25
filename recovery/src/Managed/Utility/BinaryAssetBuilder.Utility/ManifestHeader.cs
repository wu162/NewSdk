using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AssetStream;

namespace BinaryAssetBuilder.Utility;

public class ManifestHeader : StructWrapper_003CAssetStream_003A_003AManifestHeader_003E
{
	public unsafe bool IsBigEndian
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return *(bool*)m_Data;
		}
	}

	public unsafe ushort Version
	{
		get
		{
			return ((ushort*)m_Data)[1];
		}
		set
		{
			((short*)m_Data)[1] = (short)value;
		}
	}

	public unsafe bool IsLinked
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return ((bool*)m_Data)[1];
		}
		[param: MarshalAs(UnmanagedType.U1)]
		set
		{
			((sbyte*)m_Data)[1] = (value ? ((sbyte)1) : ((sbyte)0));
		}
	}

	public unsafe uint TotalInstanceDataSize
	{
		get
		{
			return ((uint*)m_Data)[4];
		}
		set
		{
			((int*)m_Data)[4] = (int)value;
		}
	}

	public unsafe uint AllTypesHash
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

	public unsafe uint StreamChecksum
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

	public unsafe uint SourceFileNameBufferSize
	{
		get
		{
			return ((uint*)m_Data)[11];
		}
		set
		{
			((int*)m_Data)[11] = (int)value;
		}
	}

	public unsafe uint ReferencedManifestNameBufferSize
	{
		get
		{
			return ((uint*)m_Data)[9];
		}
		set
		{
			((int*)m_Data)[9] = (int)value;
		}
	}

	public unsafe uint MaxRelocationChunkSize
	{
		get
		{
			return ((uint*)m_Data)[6];
		}
		set
		{
			((int*)m_Data)[6] = (int)value;
		}
	}

	public unsafe uint MaxInstanceChunkSize
	{
		get
		{
			return ((uint*)m_Data)[5];
		}
		set
		{
			((int*)m_Data)[5] = (int)value;
		}
	}

	public unsafe uint MaxImportsChunkSize
	{
		get
		{
			return ((uint*)m_Data)[7];
		}
		set
		{
			((int*)m_Data)[7] = (int)value;
		}
	}

	public unsafe uint AssetReferenceBufferSize
	{
		get
		{
			return ((uint*)m_Data)[8];
		}
		set
		{
			((int*)m_Data)[8] = (int)value;
		}
	}

	public unsafe uint AssetNameBufferSize
	{
		get
		{
			return ((uint*)m_Data)[10];
		}
		set
		{
			((int*)m_Data)[10] = (int)value;
		}
	}

	public unsafe uint AssetCount
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

	public static ushort LatestVersion => 6;

	protected unsafe override void Swap()
	{
		AssetStream.ManifestHeader* ptr = (AssetStream.ManifestHeader*)((byte*)m_Data + 12);
		uint num = *(uint*)ptr;
		*(uint*)ptr = (((num & 0xFF0000) | (num >> 16)) >> 8) | (((num & 0xFF00) | (num << 16)) << 8);
		AssetStream.ManifestHeader* ptr2 = (AssetStream.ManifestHeader*)((byte*)m_Data + 40);
		uint num2 = *(uint*)ptr2;
		*(uint*)ptr2 = (((num2 & 0xFF0000) | (num2 >> 16)) >> 8) | (((num2 & 0xFF00) | (num2 << 16)) << 8);
		AssetStream.ManifestHeader* ptr3 = (AssetStream.ManifestHeader*)((byte*)m_Data + 32);
		uint num3 = *(uint*)ptr3;
		*(uint*)ptr3 = (((num3 & 0xFF0000) | (num3 >> 16)) >> 8) | (((num3 & 0xFF00) | (num3 << 16)) << 8);
		AssetStream.ManifestHeader* ptr4 = (AssetStream.ManifestHeader*)((byte*)m_Data + 28);
		uint num4 = *(uint*)ptr4;
		*(uint*)ptr4 = (((num4 & 0xFF0000) | (num4 >> 16)) >> 8) | (((num4 & 0xFF00) | (num4 << 16)) << 8);
		AssetStream.ManifestHeader* ptr5 = (AssetStream.ManifestHeader*)((byte*)m_Data + 20);
		uint num5 = *(uint*)ptr5;
		*(uint*)ptr5 = (((num5 & 0xFF0000) | (num5 >> 16)) >> 8) | (((num5 & 0xFF00) | (num5 << 16)) << 8);
		AssetStream.ManifestHeader* ptr6 = (AssetStream.ManifestHeader*)((byte*)m_Data + 24);
		uint num6 = *(uint*)ptr6;
		*(uint*)ptr6 = (((num6 & 0xFF0000) | (num6 >> 16)) >> 8) | (((num6 & 0xFF00) | (num6 << 16)) << 8);
		AssetStream.ManifestHeader* ptr7 = (AssetStream.ManifestHeader*)((byte*)m_Data + 36);
		uint num7 = *(uint*)ptr7;
		*(uint*)ptr7 = (((num7 & 0xFF0000) | (num7 >> 16)) >> 8) | (((num7 & 0xFF00) | (num7 << 16)) << 8);
		AssetStream.ManifestHeader* ptr8 = (AssetStream.ManifestHeader*)((byte*)m_Data + 44);
		uint num8 = *(uint*)ptr8;
		*(uint*)ptr8 = (((num8 & 0xFF0000) | (num8 >> 16)) >> 8) | (((num8 & 0xFF00) | (num8 << 16)) << 8);
		AssetStream.ManifestHeader* ptr9 = (AssetStream.ManifestHeader*)((byte*)m_Data + 4);
		uint num9 = *(uint*)ptr9;
		*(uint*)ptr9 = (((num9 & 0xFF0000) | (num9 >> 16)) >> 8) | (((num9 & 0xFF00) | (num9 << 16)) << 8);
		AssetStream.ManifestHeader* ptr10 = (AssetStream.ManifestHeader*)((byte*)m_Data + 8);
		uint num10 = *(uint*)ptr10;
		*(uint*)ptr10 = (((num10 & 0xFF0000) | (num10 >> 16)) >> 8) | (((num10 & 0xFF00) | (num10 << 16)) << 8);
		AssetStream.ManifestHeader* ptr11 = (AssetStream.ManifestHeader*)((byte*)m_Data + 16);
		uint num11 = *(uint*)ptr11;
		*(uint*)ptr11 = (((num11 & 0xFF0000) | (num11 >> 16)) >> 8) | (((num11 & 0xFF00) | (num11 << 16)) << 8);
		AssetStream.ManifestHeader* ptr12 = (AssetStream.ManifestHeader*)((byte*)m_Data + 2);
		ushort num12 = *(ushort*)ptr12;
		*(short*)ptr12 = (short)((ushort)((num12 << 8) & 0xFF00) | (num12 >> 8));
	}

	public unsafe ManifestHeader()
	{ ((short*)m_Data)[1] = 6; }

	public unsafe override void SaveToStream(Stream output, [MarshalAs(UnmanagedType.U1)] bool isBigEndian)
	{
		*(bool*)m_Data = isBigEndian;
		base.SaveToStream(output, isBigEndian);
	}
}

