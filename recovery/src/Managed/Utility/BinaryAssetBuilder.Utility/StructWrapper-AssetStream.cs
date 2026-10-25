using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AssetStream;

namespace BinaryAssetBuilder.Utility;

public abstract class StructWrapper_003CAssetStream_003A_003AAssetHeader_003E : IDisposable
{
	protected unsafe AssetStream.AssetHeader* m_Data;

	protected abstract void Swap();

	public unsafe StructWrapper_003CAssetStream_003A_003AAssetHeader_003E()
	{
		AssetStream.AssetHeader* ptr = (AssetStream.AssetHeader*)global::_003CModule_003E.@new(32u);
		AssetStream.AssetHeader* data;
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
		base._002Ector();
	}

	private unsafe void _007EStructWrapper_003CAssetStream_003A_003AAssetHeader_003E()
	{
		AssetStream.AssetHeader* data = m_Data;
		if (data != null)
		{
			global::_003CModule_003E.delete(data);
		}
	}

	public unsafe virtual void LoadFromBuffer(byte[] buffer, [MarshalAs(UnmanagedType.U1)] bool isBigEndian)
	{
		fixed (byte* ptr = &buffer[0])
		{
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(m_Data, ptr, 32);
			if (isBigEndian)
			{
				Swap();
			}
		}
	}

	public virtual void LoadFromStream(Stream input, [MarshalAs(UnmanagedType.U1)] bool isBigEndian)
	{
		byte[] buffer = new byte[32];
		input.Read(buffer, 0, 32);
		LoadFromBuffer(buffer, isBigEndian);
	}

	public unsafe virtual void SaveToStream(Stream output, [MarshalAs(UnmanagedType.U1)] bool isBigEndian)
	{
		if (isBigEndian)
		{
			Swap();
		}
		byte[] buffer = new byte[32];
		new UnmanagedMemoryStream((byte*)m_Data, 32L).Read(buffer, 0, 32);
		output.Write(buffer, 0, 32);
		if (isBigEndian)
		{
			Swap();
		}
	}

	protected unsafe virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool P_0)
	{
		if (P_0)
		{
			AssetStream.AssetHeader* data = m_Data;
			if (data != null)
			{
				global::_003CModule_003E.delete(data);
			}
		}
		else
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
public abstract class StructWrapper_003CAssetStream_003A_003AManifestHeader_003E : IDisposable
{
	protected unsafe AssetStream.ManifestHeader* m_Data;

	protected abstract void Swap();

	public unsafe StructWrapper_003CAssetStream_003A_003AManifestHeader_003E()
	{
		AssetStream.ManifestHeader* ptr = (AssetStream.ManifestHeader*)global::_003CModule_003E.@new(48u);
		AssetStream.ManifestHeader* data;
		if (ptr != null)
		{
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlock(ptr, 0, 48);
			data = ptr;
		}
		else
		{
			data = null;
		}
		m_Data = data;
		base._002Ector();
	}

	private unsafe void _007EStructWrapper_003CAssetStream_003A_003AManifestHeader_003E()
	{
		AssetStream.ManifestHeader* data = m_Data;
		if (data != null)
		{
			global::_003CModule_003E.delete(data);
		}
	}

	public unsafe virtual void LoadFromBuffer(byte[] buffer, [MarshalAs(UnmanagedType.U1)] bool isBigEndian)
	{
		fixed (byte* ptr = &buffer[0])
		{
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(m_Data, ptr, 48);
			if (isBigEndian)
			{
				Swap();
			}
		}
	}

	public virtual void LoadFromStream(Stream input, [MarshalAs(UnmanagedType.U1)] bool isBigEndian)
	{
		byte[] buffer = new byte[48];
		input.Read(buffer, 0, 48);
		LoadFromBuffer(buffer, isBigEndian);
	}

	public unsafe virtual void SaveToStream(Stream output, [MarshalAs(UnmanagedType.U1)] bool isBigEndian)
	{
		if (isBigEndian)
		{
			Swap();
		}
		byte[] buffer = new byte[48];
		new UnmanagedMemoryStream((byte*)m_Data, 48L).Read(buffer, 0, 48);
		output.Write(buffer, 0, 48);
		if (isBigEndian)
		{
			Swap();
		}
	}

	protected unsafe virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool P_0)
	{
		if (P_0)
		{
			AssetStream.ManifestHeader* data = m_Data;
			if (data != null)
			{
				global::_003CModule_003E.delete(data);
			}
		}
		else
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
public abstract class StructWrapper_003CAssetStream_003A_003AAssetEntry_003E : IDisposable
{
	protected unsafe AssetStream.AssetEntry* m_Data;

	protected abstract void Swap();

	public unsafe StructWrapper_003CAssetStream_003A_003AAssetEntry_003E()
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
		base._002Ector();
	}

	private unsafe void _007EStructWrapper_003CAssetStream_003A_003AAssetEntry_003E()
	{
		AssetStream.AssetEntry* data = m_Data;
		if (data != null)
		{
			global::_003CModule_003E.delete(data);
		}
	}

	public unsafe virtual void LoadFromBuffer(byte[] buffer, [MarshalAs(UnmanagedType.U1)] bool isBigEndian)
	{
		fixed (byte* ptr = &buffer[0])
		{
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(m_Data, ptr, 48);
			if (isBigEndian)
			{
				Swap();
			}
		}
	}

	public virtual void LoadFromStream(Stream input, [MarshalAs(UnmanagedType.U1)] bool isBigEndian)
	{
		byte[] buffer = new byte[48];
		input.Read(buffer, 0, 48);
		LoadFromBuffer(buffer, isBigEndian);
	}

	public unsafe virtual void SaveToStream(Stream output, [MarshalAs(UnmanagedType.U1)] bool isBigEndian)
	{
		if (isBigEndian)
		{
			Swap();
		}
		byte[] buffer = new byte[48];
		new UnmanagedMemoryStream((byte*)m_Data, 48L).Read(buffer, 0, 48);
		output.Write(buffer, 0, 48);
		if (isBigEndian)
		{
			Swap();
		}
	}

	protected unsafe virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool P_0)
	{
		if (P_0)
		{
			AssetStream.AssetEntry* data = m_Data;
			if (data != null)
			{
				global::_003CModule_003E.delete(data);
			}
		}
		else
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
