namespace Mvp.Xml.XInclude;

public class InvalidAcceptHTTPHeaderValueError : FatalException
{
	public InvalidAcceptHTTPHeaderValueError(char c)
		: base(SR.GetString("InvalidCharForAccept", ((int)c).ToString("X2")))
	{
	}
}
