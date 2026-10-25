using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.Exslt;

public class ExsltRegularExpressions
{
	public bool test(string str, string regexp)
	{
		RegexOptions options = RegexOptions.ECMAScript;
		Regex regex = new Regex(regexp, options);
		return regex.IsMatch(str);
	}

	public bool test(string str, string regexp, string flags)
	{
		RegexOptions regexOptions = RegexOptions.ECMAScript;
		if (flags.IndexOf("m") != -1)
		{
			regexOptions |= RegexOptions.Multiline;
		}
		if (flags.IndexOf("i") != -1)
		{
			regexOptions |= RegexOptions.IgnoreCase;
		}
		Regex regex = new Regex(regexp, regexOptions);
		return regex.IsMatch(str);
	}

	public XPathNodeIterator match(string str, string regexp)
	{
		RegexOptions options = RegexOptions.ECMAScript;
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml("<matches/>");
		Regex regex = new Regex(regexp, options);
		foreach (Group group in regex.Match(str).Groups)
		{
			XmlElement xmlElement = xmlDocument.CreateElement("match");
			xmlElement.InnerText = group.Value;
			xmlDocument.DocumentElement.AppendChild(xmlElement);
		}
		return xmlDocument.CreateNavigator().Select("//match");
	}

	public XPathNodeIterator match(string str, string regexp, string flags)
	{
		RegexOptions regexOptions = RegexOptions.ECMAScript;
		if (flags.IndexOf("m") != -1)
		{
			regexOptions |= RegexOptions.Multiline;
		}
		if (flags.IndexOf("i") != -1)
		{
			regexOptions |= RegexOptions.IgnoreCase;
		}
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml("<matches/>");
		Regex regex = new Regex(regexp, regexOptions);
		if (flags.IndexOf("g") == -1)
		{
			foreach (Group group in regex.Match(str).Groups)
			{
				XmlElement xmlElement = xmlDocument.CreateElement("match");
				xmlElement.InnerText = group.Value;
				xmlDocument.DocumentElement.AppendChild(xmlElement);
			}
		}
		else
		{
			foreach (Match item in regex.Matches(str))
			{
				XmlElement xmlElement2 = xmlDocument.CreateElement("match");
				xmlElement2.InnerText = item.Value;
				xmlDocument.DocumentElement.AppendChild(xmlElement2);
			}
		}
		return xmlDocument.CreateNavigator().Select("//match");
	}

	public string replace(string input, string regexp, string flags, string replacement)
	{
		RegexOptions regexOptions = RegexOptions.ECMAScript;
		if (flags.IndexOf("i") != -1)
		{
			regexOptions |= RegexOptions.IgnoreCase;
		}
		Regex regex = new Regex(regexp, regexOptions);
		if (flags.IndexOf("g") != -1)
		{
			return regex.Replace(input, replacement);
		}
		return regex.Replace(input, replacement, 1);
	}
}
