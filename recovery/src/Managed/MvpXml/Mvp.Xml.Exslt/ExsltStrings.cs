using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.Exslt;

public class ExsltStrings
{
	private static readonly char[] hexdigit = new char[16]
	{
		'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
		'A', 'B', 'C', 'D', 'E', 'F'
	};

	public XPathNodeIterator tokenize(string str, string delimiters)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml("<tokens/>");
		if (delimiters == string.Empty)
		{
			foreach (char c in str)
			{
				XmlElement xmlElement = xmlDocument.CreateElement("token");
				xmlElement.InnerText = c.ToString();
				xmlDocument.DocumentElement.AppendChild(xmlElement);
			}
		}
		else
		{
			string[] array = str.Split(delimiters.ToCharArray());
			foreach (string innerText in array)
			{
				XmlElement xmlElement2 = xmlDocument.CreateElement("token");
				xmlElement2.InnerText = innerText;
				xmlDocument.DocumentElement.AppendChild(xmlElement2);
			}
		}
		return xmlDocument.CreateNavigator().Select("//token");
	}

	public XPathNodeIterator tokenize(string str)
	{
		Regex regex = new Regex("\\s+");
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml("<tokens/>");
		string[] array = regex.Split(str);
		foreach (string innerText in array)
		{
			XmlElement xmlElement = xmlDocument.CreateElement("token");
			xmlElement.InnerText = innerText;
			xmlDocument.DocumentElement.AppendChild(xmlElement);
		}
		return xmlDocument.CreateNavigator().Select("//token");
	}

	public string replace(string str, string oldValue, string newValue)
	{
		return str.Replace(oldValue, newValue);
	}

	public string padding(int number)
	{
		string empty = string.Empty;
		if (number < 0)
		{
			return empty;
		}
		return empty.PadLeft(number);
	}

	public string padding(int number, string s)
	{
		if (number < 0 || s == string.Empty)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(s);
		while (stringBuilder.Length < number)
		{
			stringBuilder.Append(s);
		}
		if (stringBuilder.Length > number)
		{
			return stringBuilder.Remove(number, stringBuilder.Length - number).ToString();
		}
		return stringBuilder.ToString();
	}

	public XPathNodeIterator split(string str)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml("<tokens/>");
		string[] array = str.Split(' ');
		foreach (string text in array)
		{
			if (!text.Equals(string.Empty))
			{
				XmlElement xmlElement = xmlDocument.CreateElement("token");
				xmlElement.InnerText = text;
				xmlDocument.DocumentElement.AppendChild(xmlElement);
			}
		}
		return xmlDocument.CreateNavigator().Select("//token");
	}

	public XPathNodeIterator split(string str, string delimiter)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml("<tokens/>");
		if (delimiter.Equals(string.Empty))
		{
			foreach (char c in str)
			{
				XmlElement xmlElement = xmlDocument.CreateElement("token");
				xmlElement.InnerText = c.ToString();
				xmlDocument.DocumentElement.AppendChild(xmlElement);
			}
		}
		else
		{
			delimiter = delimiter.Replace("\\", "\\\\").Replace("$", "\\$").Replace("^", "\\^");
			delimiter = delimiter.Replace("{", "\\{").Replace("[", "\\[").Replace("(", "\\(");
			delimiter = delimiter.Replace("*", "\\*").Replace(")", "\\)").Replace("|", "\\|");
			delimiter = delimiter.Replace("+", "\\+").Replace("?", "\\?").Replace(".", "\\.");
			Regex regex = new Regex(delimiter);
			string[] array = regex.Split(str);
			foreach (string text in array)
			{
				if (!text.Equals(string.Empty) && !text.Equals(delimiter))
				{
					XmlElement xmlElement2 = xmlDocument.CreateElement("token");
					xmlElement2.InnerText = text;
					xmlDocument.DocumentElement.AppendChild(xmlElement2);
				}
			}
		}
		return xmlDocument.CreateNavigator().Select("//token");
	}

	public string concat(XPathNodeIterator nodeset)
	{
		StringBuilder stringBuilder = new StringBuilder();
		while (nodeset.MoveNext())
		{
			stringBuilder.Append(nodeset.Current.Value);
		}
		return stringBuilder.ToString();
	}

	public string align(string str, string padding, string alignment)
	{
		if (str.Length > padding.Length)
		{
			return str.Substring(0, padding.Length);
		}
		if (str.Length == padding.Length)
		{
			return str;
		}
		switch (alignment)
		{
		case "right":
			return padding.Substring(0, padding.Length - str.Length) + str;
		case "center":
		{
			int num = (padding.Length - str.Length) / 2;
			return padding.Substring(0, num) + str + padding.Substring(str.Length + num);
		}
		default:
			return str + padding.Substring(str.Length);
		}
	}

	public string align(string str, string padding)
	{
		return align(str, padding, "left");
	}

	public string encode_002Duri(string str, bool encodeReserved)
	{
		return encodeUri(str, encodeReserved);
	}

	public string encode_002Duri(string str, bool encodeReserved, string encoding)
	{
		return encodeUri(str, encodeReserved, encoding);
	}

	public string encodeUri(string str, bool encodeReserved)
	{
		return encodeUriImpl(str, encodeReserved, Encoding.UTF8);
	}

	public string encodeUri(string str, bool encodeReserved, string encoding)
	{
		Encoding encoding2 = null;
		try
		{
			encoding2 = Encoding.GetEncoding(encoding);
		}
		catch
		{
			return string.Empty;
		}
		return encodeUriImpl(str, encodeReserved, encoding2);
	}

	private string encodeUriImpl(string str, bool encodeReserved, Encoding enc)
	{
		if (str == string.Empty)
		{
			return str;
		}
		StringBuilder stringBuilder = new StringBuilder(str.Length);
		char[] array = str.ToCharArray();
		if (encodeReserved)
		{
			for (int i = 0; i < array.Length; i++)
			{
				char c = array[i];
				if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
				{
					stringBuilder.Append(c);
					continue;
				}
				switch (c)
				{
				case '!':
				case '\'':
				case '(':
				case ')':
				case '*':
				case '-':
				case '.':
				case '_':
				case '~':
					stringBuilder.Append(c);
					break;
				case '%':
					if (i < array.Length - 2 && IsHexDigit(array[i + 1]) && IsHexDigit(array[i + 2]))
					{
						stringBuilder.Append(c);
					}
					else
					{
						EncodeChar(stringBuilder, enc, array, i);
					}
					break;
				default:
					EncodeChar(stringBuilder, enc, array, i);
					break;
				}
			}
		}
		else
		{
			for (int j = 0; j < array.Length; j++)
			{
				char c2 = array[j];
				if ((c2 >= 'a' && c2 <= 'z') || (c2 >= 'A' && c2 <= 'Z') || (c2 >= '0' && c2 <= '9'))
				{
					stringBuilder.Append(c2);
					continue;
				}
				switch (c2)
				{
				case '!':
				case '$':
				case '&':
				case '\'':
				case '(':
				case ')':
				case '*':
				case '+':
				case ',':
				case '-':
				case '.':
				case '/':
				case ':':
				case ';':
				case '=':
				case '?':
				case '@':
				case '[':
				case ']':
				case '_':
				case '~':
					stringBuilder.Append(c2);
					break;
				case '%':
					if (j < array.Length - 2 && IsHexDigit(array[j + 1]) && IsHexDigit(array[j + 2]))
					{
						stringBuilder.Append(c2);
					}
					else
					{
						EncodeChar(stringBuilder, enc, array, j);
					}
					break;
				default:
					EncodeChar(stringBuilder, enc, array, j);
					break;
				}
			}
		}
		return stringBuilder.ToString();
	}

	private void EncodeChar(StringBuilder res, Encoding enc, char[] str, int index)
	{
		byte[] bytes = enc.GetBytes(str, index, 1);
		foreach (byte b in bytes)
		{
			res.AppendFormat("%{0}{1}", hexdigit[b >> 4], hexdigit[b & 0xF]);
		}
	}

	private bool IsHexDigit(char c)
	{
		if ((c < '0' || c > '9') && (c < 'a' || c > 'f'))
		{
			if (c >= 'A')
			{
				return c <= 'F';
			}
			return false;
		}
		return true;
	}

	public string decode_002Duri(string str)
	{
		return decodeUri(str);
	}

	public string decode_002Duri(string str, string encoding)
	{
		return decodeUri(str, encoding);
	}

	public string decodeUri(string str)
	{
		return decodeUriImpl(str, Encoding.UTF8);
	}

	public string decodeUri(string str, string encoding)
	{
		if (encoding == string.Empty)
		{
			return string.Empty;
		}
		Encoding encoding2 = null;
		try
		{
			encoding2 = Encoding.GetEncoding(encoding);
		}
		catch
		{
			return string.Empty;
		}
		return decodeUriImpl(str, encoding2);
	}

	private string decodeUriImpl(string str, Encoding enc)
	{
		if (str == string.Empty)
		{
			return str;
		}
		return HttpUtility.UrlDecode(str, enc);
	}
}
