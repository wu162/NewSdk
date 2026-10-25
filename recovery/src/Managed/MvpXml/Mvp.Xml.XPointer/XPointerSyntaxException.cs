using System;

namespace Mvp.Xml.XPointer;

public class XPointerSyntaxException : XPointerException
{
	public XPointerSyntaxException(string message)
		: base(message)
	{
	}

	public XPointerSyntaxException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
