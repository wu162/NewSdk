using System;

namespace Mvp.Xml.XPointer;

public abstract class XPointerException : Exception
{
	public XPointerException(string message)
		: base(message)
	{
	}

	public XPointerException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
