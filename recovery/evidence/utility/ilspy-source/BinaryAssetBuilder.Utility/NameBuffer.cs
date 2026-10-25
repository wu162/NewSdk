using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using std;

namespace BinaryAssetBuilder.Utility;

public class NameBuffer : IDisposable
{
	private List<byte> m_Data = new List<byte>();

	private unsafe map_003Cint_002Cint_002Cstd_003A_003Aless_003Cint_003E_002Cstd_003A_003Aallocator_003Cstd_003A_003Apair_003Cint_0020const_0020_002Cint_003E_0020_003E_0020_003E* m_Positions;

	public int Length => m_Data.Count;

	public unsafe NameBuffer()
	{
		map_003Cint_002Cint_002Cstd_003A_003Aless_003Cint_003E_002Cstd_003A_003Aallocator_003Cstd_003A_003Apair_003Cint_0020const_0020_002Cint_003E_0020_003E_0020_003E* ptr = (map_003Cint_002Cint_002Cstd_003A_003Aless_003Cint_003E_002Cstd_003A_003Aallocator_003Cstd_003A_003Apair_003Cint_0020const_0020_002Cint_003E_0020_003E_0020_003E*)global::_003CModule_003E.@new(12u);
		map_003Cint_002Cint_002Cstd_003A_003Aless_003Cint_003E_002Cstd_003A_003Aallocator_003Cstd_003A_003Apair_003Cint_0020const_0020_002Cint_003E_0020_003E_0020_003E* positions;
		try
		{
			positions = ((ptr == null) ? null : global::_003CModule_003E.std_002Emap_003Cint_002Cint_002Cstd_003A_003Aless_003Cint_003E_002Cstd_003A_003Aallocator_003Cstd_003A_003Apair_003Cint_0020const_0020_002Cint_003E_0020_003E_0020_003E_002E_007Bctor_007D(ptr));
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.delete(ptr);
			throw;
		}
		m_Positions = positions;
		base._002Ector();
	}

	private void _007ENameBuffer()
	{
	}

	public unsafe int AddName(string name)
	{
		int hashCode = name.GetHashCode();
		map_003Cint_002Cint_002Cstd_003A_003Aless_003Cint_003E_002Cstd_003A_003Aallocator_003Cstd_003A_003Apair_003Cint_0020const_0020_002Cint_003E_0020_003E_0020_003E* positions = m_Positions;
		_Tree_003Cstd_003A_003A_Tmap_traits_003Cint_002Cint_002Cstd_003A_003Aless_003Cint_003E_002Cstd_003A_003Aallocator_003Cstd_003A_003Apair_003Cint_0020const_0020_002Cint_003E_0020_003E_002C0_003E_0020_003E* ptr = (_Tree_003Cstd_003A_003A_Tmap_traits_003Cint_002Cint_002Cstd_003A_003Aless_003Cint_003E_002Cstd_003A_003Aallocator_003Cstd_003A_003Apair_003Cint_0020const_0020_002Cint_003E_0020_003E_002C0_003E_0020_003E*)positions;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _Tree_003Cstd_003A_003A_Tmap_traits_003Cint_002Cint_002Cstd_003A_003Aless_003Cint_003E_002Cstd_003A_003Aallocator_003Cstd_003A_003Apair_003Cint_0020const_0020_002Cint_003E_0020_003E_002C0_003E_0020_003E.iterator iterator);
		*(int*)(&iterator) = (int)global::_003CModule_003E.std_002E_Tree_003Cstd_003A_003A_Tmap_traits_003Cint_002Cint_002Cstd_003A_003Aless_003Cint_003E_002Cstd_003A_003Aallocator_003Cstd_003A_003Apair_003Cint_0020const_0020_002Cint_003E_0020_003E_002C0_003E_0020_003E_002E_Lbound(ptr, &hashCode);
		int num = ((int*)ptr)[1];
		_Tree_003Cstd_003A_003A_Tmap_traits_003Cint_002Cint_002Cstd_003A_003Aless_003Cint_003E_002Cstd_003A_003Aallocator_003Cstd_003A_003Apair_003Cint_0020const_0020_002Cint_003E_0020_003E_002C0_003E_0020_003E.iterator* ptr2;
		if (*(int*)(&iterator) != num && !(hashCode < *(int*)(*(int*)(&iterator) + 12)))
		{
			ptr2 = &iterator;
		}
		else
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _Tree_003Cstd_003A_003A_Tmap_traits_003Cint_002Cint_002Cstd_003A_003Aless_003Cint_003E_002Cstd_003A_003Aallocator_003Cstd_003A_003Apair_003Cint_0020const_0020_002Cint_003E_0020_003E_002C0_003E_0020_003E.iterator iterator2);
			*(int*)(&iterator2) = num;
			ptr2 = &iterator2;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _Tree_003Cstd_003A_003A_Tmap_traits_003Cint_002Cint_002Cstd_003A_003Aless_003Cint_003E_002Cstd_003A_003Aallocator_003Cstd_003A_003Apair_003Cint_0020const_0020_002Cint_003E_0020_003E_002C0_003E_0020_003E.iterator iterator3);
		// IL cpblk instruction
		System.Runtime.CompilerServices.Unsafe.CopyBlock(ref iterator3, ptr2, 4);
		if (*(int*)(&iterator3) != ((int*)positions)[1])
		{
			return *(int*)(*(int*)(&iterator3) + 16);
		}
		IntPtr hglobal = Marshal.StringToHGlobalAnsi(name);
		byte* ptr3 = (byte*)hglobal.ToPointer();
		int count = m_Data.Count;
		byte b = *ptr3;
		if (b != 0)
		{
			do
			{
				byte item = b;
				m_Data.Add(item);
				ptr3++;
				b = *ptr3;
			}
			while (b != 0);
		}
		m_Data.Add(0);
		Marshal.FreeHGlobal(hglobal);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out pair_003Cint_0020const_0020_002Cint_003E obj);
		*(int*)(&obj) = name.GetHashCode();
		System.Runtime.CompilerServices.Unsafe.As<pair_003Cint_0020const_0020_002Cint_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 4)) = count;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out pair_003Cstd_003A_003A_Tree_003Cstd_003A_003A_Tmap_traits_003Cint_002Cint_002Cstd_003A_003Aless_003Cint_003E_002Cstd_003A_003Aallocator_003Cstd_003A_003Apair_003Cint_0020const_0020_002Cint_003E_0020_003E_002C0_003E_0020_003E_003A_003Aiterator_002Cbool_003E obj2);
		global::_003CModule_003E.std_002E_Tree_003Cstd_003A_003A_Tmap_traits_003Cint_002Cint_002Cstd_003A_003Aless_003Cint_003E_002Cstd_003A_003Aallocator_003Cstd_003A_003Apair_003Cint_0020const_0020_002Cint_003E_0020_003E_002C0_003E_0020_003E_002Einsert((_Tree_003Cstd_003A_003A_Tmap_traits_003Cint_002Cint_002Cstd_003A_003Aless_003Cint_003E_002Cstd_003A_003Aallocator_003Cstd_003A_003Apair_003Cint_0020const_0020_002Cint_003E_0020_003E_002C0_003E_0020_003E*)m_Positions, &obj2, &obj);
		return count;
	}

	public void SaveToStream(Stream output)
	{
		output.Write(m_Data.ToArray(), 0, m_Data.Count);
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
