namespace Mvp.Xml.XInclude;

public class NonXmlCharacterException : FatalException
{
	public NonXmlCharacterException(char c)
		: base(SR.GetString("NonXmlCharacter", ((int)c).ToString("X2")))
	{
	}
}
