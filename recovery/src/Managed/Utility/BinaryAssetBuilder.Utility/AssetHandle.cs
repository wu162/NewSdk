using System;
using System.Runtime.InteropServices;

namespace BinaryAssetBuilder.Utility;

internal class AssetHandle : IComparable<AssetHandle>
{
	private uint _InstanceId;

	private uint _TypeId;

	public uint TypeId => _TypeId;

	public uint InstanceId => _InstanceId;

	public AssetHandle(uint typeId, uint instanceId)
	{
		_TypeId = typeId;
		_InstanceId = instanceId;
	}

	public AssetHandle(AssetHandle src)
	{
		_InstanceId = src._InstanceId;
		_TypeId = src._TypeId;
	}

	public AssetHandle()
	{
	}

	public override int GetHashCode()
	{
		return (int)(_TypeId ^ _InstanceId);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		AssetHandle assetHandle = (AssetHandle)obj;
		int num = ((_InstanceId == assetHandle._InstanceId && _TypeId == assetHandle._TypeId) ? 1 : 0);
		return (byte)num != 0;
	}

	public virtual int CompareTo(AssetHandle other)
	{
		uint typeId = other._TypeId;
		uint typeId2 = _TypeId;
		if (typeId2 != typeId)
		{
			int num = 1;
			if (typeId2 <= typeId)
			{
				num = -num;
			}
			return num;
		}
		uint instanceId = other._InstanceId;
		uint instanceId2 = _InstanceId;
		if (instanceId2 != instanceId)
		{
			int num2 = 1;
			if (instanceId2 <= instanceId)
			{
				num2 = -num2;
			}
			return num2;
		}
		return 0;
	}
}
