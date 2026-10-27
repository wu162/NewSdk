using System;
using System.Collections.Generic;
using System.Threading;

namespace BinaryAssetBuilder.Core;

public class Chunk : IDisposable
{
	private const int BufferSize = 1048576;

    // Bound idle retention only. Live readers still own their arrays and this
    // does not establish a process-wide memory budget or enable parallel Core.
    private const int MaxRetainedBuffers = 8;

	private static Stack<byte[]> _BufferStack = new Stack<byte[]>();

	private byte[] _Data = GetBuffer();

	private int _BytesRead;

	public byte[] Data => _Data;

	public int BytesRead
	{
		get
		{
			return _BytesRead;
		}
		set
		{
			_BytesRead = value;
		}
	}

	private static byte[] GetBuffer()
	{
		lock (_BufferStack)
		{
			if (_BufferStack.Count == 0)
			{
				return new byte[1048576];
			}
			return _BufferStack.Pop();
		}
	}

	private static void ReleaseBuffer(byte[] buff)
	{
		lock (_BufferStack)
		{
            if (_BufferStack.Count < MaxRetainedBuffers) _BufferStack.Push(buff);
		}
	}

	public void Dispose()
	{
        byte[] data = Interlocked.Exchange(ref _Data, null);
        if (data != null) ReleaseBuffer(data);
	}

    // An uncertain APM read may still own this array. Drop this owner's
    // reference without returning that array to the shared pool. This method
    // never establishes read completion; the reader must prove it separately.
    internal void AbandonBuffer()
    {
        Interlocked.Exchange(ref _Data, null);
    }
}
