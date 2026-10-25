namespace Mvp.Xml.XInclude;

internal class TextUtils
{
	public static void CheckForNonXmlChars(string str)
	{
		int num = 0;
		while (num < str.Length)
		{
			char c = str[num];
			if ((c >= ' ' && c <= '\ud7ff') || (c >= '\ue000' && c <= '\ufffd') || c == '\n' || c == '\r' || c == '\t')
			{
				num++;
				continue;
			}
			if (c >= '\ud800' && c <= '\udbff' && ++num < str.Length)
			{
				if (str[num] >= '\udc00' && str[num] <= '\udfff')
				{
					num++;
				}
				continue;
			}
			throw new NonXmlCharacterException(str[num]);
		}
	}

	public static void CheckAcceptValue(string accept)
	{
		foreach (char c in accept)
		{
			if (c < ' ' || c > '~')
			{
				throw new InvalidAcceptHTTPHeaderValueError(c);
			}
		}
	}
}
