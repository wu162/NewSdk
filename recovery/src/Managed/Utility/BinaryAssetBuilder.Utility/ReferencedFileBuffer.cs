using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace BinaryAssetBuilder.Utility;

public class ReferencedFileBuffer : IDisposable
{
    private List<byte> m_Data = new List<byte>();
    // The native map is keyed by String.GetHashCode(), rather than the string.
    // In particular, hash collisions and an altered isPatch flag reuse the first entry.
    private Dictionary<int, int> m_Positions = new Dictionary<int, int>();

    public int Length => m_Data.Count;

    public unsafe int AddReference(string name, [MarshalAs(UnmanagedType.U1)] bool isPatch)
    {
        int hashCode = name.GetHashCode();
        int position;
        if (m_Positions.TryGetValue(hashCode, out position)) return position;
        IntPtr hglobal = Marshal.StringToHGlobalAnsi(name);
        byte* data = (byte*)hglobal.ToPointer();
        int count = m_Data.Count;
        if (isPatch) m_Data.Add(2);
        else m_Data.Add(1);
        while (*data != 0)
        {
            m_Data.Add(*data);
            data++;
        }
        m_Data.Add(0);
        Marshal.FreeHGlobal(hglobal);
        m_Positions.Add(name.GetHashCode(), count);
        return count;
    }

    public void SaveToStream(Stream output)
    {
        output.Write(m_Data.ToArray(), 0, m_Data.Count);
    }

    protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool disposing)
    {
        // Original destructor is empty; Dispose(false) only calls Object.Finalize.
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
