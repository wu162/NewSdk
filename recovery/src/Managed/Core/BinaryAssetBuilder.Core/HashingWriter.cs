using System;
using System.IO;
using System.Text;

namespace BinaryAssetBuilder.Core;

internal class HashingWriter : TextWriter
{
	private uint m_runningHash;

	private string m_leftoverChars = "";

	private static readonly int HashingSize = 512;

    private readonly bool m_bufferedWrites;
    private char[] m_pending;
    private int m_pendingCount;

	public override Encoding Encoding => Encoding.Default;

	public HashingWriter(uint runningHash)
	{
		m_runningHash = runningHash;
        bool disabled; AppContext.TryGetSwitch("NewSdk.Compiler.DisableBufferedHashingWriter", out disabled);
        // An internal derived writer can still override Write(string). Keep
        // the old virtual overload routing and state algorithm for that case.
        m_bufferedWrites = !disabled && GetType() == typeof(HashingWriter);
	}

	public override void Write(char c)
	{
        if (!m_bufferedWrites || (m_pending == null && m_leftoverChars.Length == 0))
            { Write(new string(c, 1)); return; }
        PreparePending();
        m_pending[m_pendingCount++] = c;
        if (m_pendingCount == HashingSize) CommitPending();
	}

	public override void Write(char[] buffer, int index, int count)
	{
        // Let the original String constructor own every invalid-argument
        // exception (including parameter names/order) before touching state.
        if (!m_bufferedWrites || buffer == null || index < 0 || count < 0 || index > buffer.Length - count)
            { Write(new string(buffer, index, count)); return; }
        Append(null, buffer, index, count);
	}

	public override void Write(string str)
	{
        if (!m_bufferedWrites) { WriteLegacy(str); return; }
        if (str == null)
        {
            // Legacy concatenation treats null as empty only with pending text.
            // With no pending text its Length access throws NullReference.
            if (m_pendingCount == 0 && m_leftoverChars.Length == 0) WriteLegacy(str);
            return;
        }
        Append(str, null, 0, str.Length);
    }

    private void PreparePending()
    {
        if (m_pending == null) m_pending = new char[HashingSize];
        if (m_leftoverChars.Length == 0) return;
        m_leftoverChars.CopyTo(0, m_pending, 0, m_leftoverChars.Length);
        m_pendingCount = m_leftoverChars.Length;
        m_leftoverChars = "";
    }

    private void CommitPending()
    {
        m_runningHash = HashProvider.GetTextHash(m_runningHash, new string(m_pending, 0, HashingSize));
        m_pendingCount = 0;
    }

    private static string Slice(string text, char[] array, int index, int count)
    {
        return text == null ? new string(array, index, count) : text.Substring(index, count);
    }

    private void Copy(string text, char[] array, int index, int count)
    {
        if (text == null) Array.Copy(array, index, m_pending, m_pendingCount, count);
        else text.CopyTo(index, m_pending, m_pendingCount, count);
        m_pendingCount += count;
    }

    private void Append(string text, char[] array, int index, int count)
    {
        if (count == 0) return;
        if (m_pendingCount != 0 || m_leftoverChars.Length != 0)
        {
            PreparePending();
            int take = Math.Min(HashingSize - m_pendingCount, count);
            Copy(text, array, index, take); index += take; count -= take;
            if (m_pendingCount == HashingSize) CommitPending();
        }
        // Hash exactly the old 512 UTF-16 character strings. In particular do
        // not combine, encode, normalize or realign a split surrogate pair.
        while (count >= HashingSize)
        {
            m_runningHash = HashProvider.GetTextHash(m_runningHash, Slice(text, array, index, HashingSize));
            index += HashingSize; count -= HashingSize;
        }
        if (count == 0) return;
        if (m_pending == null) m_leftoverChars = Slice(text, array, index, count);
        else Copy(text, array, index, count);
    }

    private void WriteLegacy(string str)
    {
		string text = str;
		if (m_leftoverChars.Length > 0)
		{
			text = m_leftoverChars + str;
		}
		if (text.Length < HashingSize)
		{
			m_leftoverChars = text;
			return;
		}
		for (int i = 0; i + HashingSize <= text.Length; i += HashingSize)
		{
			string text2 = text.Substring(i, HashingSize);
			m_runningHash = HashProvider.GetTextHash(m_runningHash, text2);
		}
		int num = text.Length % HashingSize;
		if (num == 0)
		{
			m_leftoverChars = "";
		}
		else
		{
			m_leftoverChars = text.Substring(text.Length - num, num);
		}
	}

	public uint GetFinalHash()
	{
        if (m_pendingCount > 0)
            return HashProvider.GetTextHash(m_runningHash, new string(m_pending, 0, m_pendingCount));
		if (m_leftoverChars.Length > 0)
		{
			return HashProvider.GetTextHash(m_runningHash, m_leftoverChars);
		}
		return m_runningHash;
	}
}
