using System;

namespace Mvp.Xml.XInclude;

public class FatalResourceException : FatalException
{
	public FatalResourceException(Exception re)
		: base(SR.GetString("FatalResourceException", re.Message), re)
	{
	}
}
