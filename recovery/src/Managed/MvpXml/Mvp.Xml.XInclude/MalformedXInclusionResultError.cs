namespace Mvp.Xml.XInclude;

public class MalformedXInclusionResultError : FatalException
{
	public MalformedXInclusionResultError(string message)
		: base(message)
	{
	}
}
