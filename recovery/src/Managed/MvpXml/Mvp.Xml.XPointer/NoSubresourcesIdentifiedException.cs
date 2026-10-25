using System;

namespace Mvp.Xml.XPointer;

public class NoSubresourcesIdentifiedException : XPointerException
{
	public NoSubresourcesIdentifiedException(string message)
		: base(message)
	{
	}

	public NoSubresourcesIdentifiedException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
