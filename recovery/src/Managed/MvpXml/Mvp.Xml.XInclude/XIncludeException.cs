using System;

namespace Mvp.Xml.XInclude;

public abstract class XIncludeException : Exception
{
	public XIncludeException(string message)
		: base(message)
	{
	}

	public XIncludeException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
