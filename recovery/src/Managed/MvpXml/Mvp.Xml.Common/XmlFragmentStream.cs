using System;
using System.IO;
using System.Text;

namespace Mvp.Xml.Common;

public class XmlFragmentStream : Stream
{
	private Stream _stream;

	private bool _first = true;

	private bool _done;

	private bool _eof;

	private byte[] _rootstart = Encoding.UTF8.GetBytes("<root>");

	private byte[] _rootend = Encoding.UTF8.GetBytes("</root>");

	private int _endidx = -1;

	public override bool CanRead => _stream.CanRead;

	public override bool CanSeek => _stream.CanSeek;

	public override bool CanWrite => _stream.CanWrite;

	public override long Length => _stream.Length;

	public override long Position
	{
		get
		{
			return _stream.Position;
		}
		set
		{
			_stream.Position = value;
		}
	}

	public XmlFragmentStream(Stream innerStream)
	{
		if (innerStream == null)
		{
			throw new ArgumentNullException("innerStream");
		}
		_stream = innerStream;
	}

	public XmlFragmentStream(Stream innerStream, string rootName)
		: this(innerStream)
	{
		_rootstart = Encoding.UTF8.GetBytes("<" + rootName + ">");
		_rootend = Encoding.UTF8.GetBytes("</" + rootName + ">");
	}

	public XmlFragmentStream(Stream innerStream, string rootName, string ns)
		: this(innerStream)
	{
		_rootstart = Encoding.UTF8.GetBytes("<" + rootName + " xmlns=\"" + ns + "\">");
		_rootend = Encoding.UTF8.GetBytes("</" + rootName + ">");
	}

	public override void Flush()
	{
		_stream.Flush();
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return _stream.Seek(offset, origin);
	}

	public override void SetLength(long value)
	{
		_stream.SetLength(value);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		_stream.Write(buffer, offset, count);
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		if (_done)
		{
			if (!_eof)
			{
				_eof = true;
				return 0;
			}
			throw new EndOfStreamException(SR.GetString(SR.XmlFragmentStream_EOF));
		}
		if (_first)
		{
			_rootstart.CopyTo(buffer, 0);
			_stream.Read(buffer, _rootstart.Length, count - _rootstart.Length);
			_first = false;
			return count;
		}
		if (_endidx != -1)
		{
			for (int i = _endidx; i < _rootend.Length; i++)
			{
				buffer[i] = _rootend[i];
			}
			return _rootend.Length - _endidx;
		}
		int num = _stream.Read(buffer, offset, count);
		if (num < count)
		{
			_rootend.CopyTo(buffer, num);
			if (count - num > _rootend.Length)
			{
				_done = true;
				return num + _rootend.Length;
			}
			_endidx = count - num;
			return count;
		}
		return num;
	}
}
