using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace BinaryAssetBuilder.Utility;

public class UInt32Buffer : IDisposable
{
	private List<uint> m_Data = new List<uint>();

	public int Length => m_Data.Count << 2;

	private void _007EUInt32Buffer()
	{
	}

	public int AddValue(uint value)
	{
		int result = m_Data.Count << 2;
		m_Data.Add(value);
		return result;
	}

	public void SaveToStream(Stream output, [MarshalAs(UnmanagedType.U1)] bool isBigEndian)
	{
		BinaryWriter binaryWriter = new BinaryWriter(output);
		if (isBigEndian)
		{
			int num = 0;
			if (0 < m_Data.Count)
			{
				do
				{
					uint num2 = m_Data[num];
					binaryWriter.Write((((num2 & 0xFF0000) | (num2 >> 16)) >> 8) | (((num2 & 0xFF00) | (num2 << 16)) << 8));
					num++;
				}
				while (num < m_Data.Count);
			}
			return;
		}
		int num3 = 0;
		if (0 < m_Data.Count)
		{
			do
			{
				binaryWriter.Write(m_Data[num3]);
				num3++;
			}
			while (num3 < m_Data.Count);
		}
	}

	protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool P_0)
	{
		if (!P_0)
		{
			Finalize();
		}
	}

	public virtual sealed void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}
