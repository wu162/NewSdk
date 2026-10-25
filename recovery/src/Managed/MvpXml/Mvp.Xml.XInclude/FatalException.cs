using System;

namespace Mvp.Xml.XInclude;

public abstract class FatalException : XIncludeException
{
	public FatalException(string message)
		: base(message)
	{
	}

	public FatalException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
