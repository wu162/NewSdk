using AssetStream;

namespace BinaryAssetBuilder.Utility;

public class AssetEntry : StructWrapper_003CAssetStream_003A_003AAssetEntry_003E
{
	public unsafe uint Tokenized
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

	public unsafe int RelocationDataSize
	{
		get
		{
			return ((int*)m_Data)[9];
		}
		set
		{
			((int*)m_Data)[9] = value;
		}
	}

	public unsafe int InstanceDataSize
	{
		get
		{
			return ((int*)m_Data)[8];
		}
		set
		{
			((int*)m_Data)[8] = value;
		}
	}

	public unsafe int ImportsDataSize
	{
		get
		{
			return ((int*)m_Data)[10];
		}
		set
		{
			((int*)m_Data)[10] = value;
		}
	}

	public unsafe int SourceFileNameOffset
	{
		get
		{
			return ((int*)m_Data)[7];
		}
		set
		{
			((int*)m_Data)[7] = value;
		}
	}

	public unsafe int NameOffset
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

	public unsafe int AssetReferenceCount
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

	public unsafe int AssetReferenceOffset
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
		AssetStream.AssetEntry* data = m_Data;
		uint num = *(uint*)data;
		*(uint*)data = (((num & 0xFF0000) | (num >> 16)) >> 8) | (((num & 0xFF00) | (num << 16)) << 8);
		AssetStream.AssetEntry* ptr = (AssetStream.AssetEntry*)((byte*)m_Data + 4);
		uint num2 = *(uint*)ptr;
		*(uint*)ptr = (((num2 & 0xFF0000) | (num2 >> 16)) >> 8) | (((num2 & 0xFF00) | (num2 << 16)) << 8);
		AssetStream.AssetEntry* ptr2 = (AssetStream.AssetEntry*)((byte*)m_Data + 12);
		uint num3 = *(uint*)ptr2;
		*(uint*)ptr2 = (((num3 & 0xFF0000) | (num3 >> 16)) >> 8) | (((num3 & 0xFF00) | (num3 << 16)) << 8);
		AssetStream.AssetEntry* ptr3 = (AssetStream.AssetEntry*)((byte*)m_Data + 8);
		uint num4 = *(uint*)ptr3;
		*(uint*)ptr3 = (((num4 & 0xFF0000) | (num4 >> 16)) >> 8) | (((num4 & 0xFF00) | (num4 << 16)) << 8);
		AssetStream.AssetEntry* ptr4 = (AssetStream.AssetEntry*)((byte*)m_Data + 16);
		uint num5 = *(uint*)ptr4;
		*(uint*)ptr4 = (((num5 & 0xFF0000) | (num5 >> 16)) >> 8) | (((num5 & 0xFF00) | (num5 << 16)) << 8);
		AssetStream.AssetEntry* ptr5 = (AssetStream.AssetEntry*)((byte*)m_Data + 20);
		uint num6 = *(uint*)ptr5;
		*(uint*)ptr5 = (((num6 & 0xFF0000) | (num6 >> 16)) >> 8) | (((num6 & 0xFF00) | (num6 << 16)) << 8);
		AssetStream.AssetEntry* ptr6 = (AssetStream.AssetEntry*)((byte*)m_Data + 24);
		uint num7 = *(uint*)ptr6;
		*(uint*)ptr6 = (((num7 & 0xFF0000) | (num7 >> 16)) >> 8) | (((num7 & 0xFF00) | (num7 << 16)) << 8);
		AssetStream.AssetEntry* ptr7 = (AssetStream.AssetEntry*)((byte*)m_Data + 28);
		uint num8 = *(uint*)ptr7;
		*(uint*)ptr7 = (((num8 & 0xFF0000) | (num8 >> 16)) >> 8) | (((num8 & 0xFF00) | (num8 << 16)) << 8);
		AssetStream.AssetEntry* ptr8 = (AssetStream.AssetEntry*)((byte*)m_Data + 32);
		uint num9 = *(uint*)ptr8;
		*(uint*)ptr8 = (((num9 & 0xFF0000) | (num9 >> 16)) >> 8) | (((num9 & 0xFF00) | (num9 << 16)) << 8);
		AssetStream.AssetEntry* ptr9 = (AssetStream.AssetEntry*)((byte*)m_Data + 36);
		uint num10 = *(uint*)ptr9;
		*(uint*)ptr9 = (((num10 & 0xFF0000) | (num10 >> 16)) >> 8) | (((num10 & 0xFF00) | (num10 << 16)) << 8);
		AssetStream.AssetEntry* ptr10 = (AssetStream.AssetEntry*)((byte*)m_Data + 40);
		uint num11 = *(uint*)ptr10;
		*(uint*)ptr10 = (((num11 & 0xFF0000) | (num11 >> 16)) >> 8) | (((num11 & 0xFF00) | (num11 << 16)) << 8);
		AssetStream.AssetEntry* ptr11 = (AssetStream.AssetEntry*)((byte*)m_Data + 44);
		uint num12 = *(uint*)ptr11;
		*(uint*)ptr11 = (((num12 & 0xFF0000) | (num12 >> 16)) >> 8) | (((num12 & 0xFF00) | (num12 << 16)) << 8);
	}

	public unsafe AssetEntry()
	{
		AssetStream.AssetEntry* ptr = (AssetStream.AssetEntry*)global::_003CModule_003E.@new(48u);
		AssetStream.AssetEntry* data;
		try
		{
			data = ptr;
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.delete(ptr);
			throw;
		}
		m_Data = data;
		((object)this)._002Ector();
	}
}
