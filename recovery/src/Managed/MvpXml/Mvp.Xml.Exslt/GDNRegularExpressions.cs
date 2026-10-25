using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.Exslt;

public class GDNRegularExpressions
{
	public XPathNodeIterator tokenize(string str, string regexp)
	{
		RegexOptions options = RegexOptions.ECMAScript;
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml("<matches/>");
		Regex regex = new Regex(regexp, options);
		string[] array = regex.Split(str);
		foreach (string innerText in array)
		{
			XmlElement xmlElement = xmlDocument.CreateElement("match");
			xmlElement.InnerText = innerText;
			xmlDocument.DocumentElement.AppendChild(xmlElement);
		}
		return xmlDocument.CreateNavigator().Select("//match");
	}

	public XPathNodeIterator tokenize(string str, string regexp, string flags)
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
		string[] array = regex.Split(str);
		foreach (string innerText in array)
		{
			XmlElement xmlElement = xmlDocument.CreateElement("match");
			xmlElement.InnerText = innerText;
			xmlDocument.DocumentElement.AppendChild(xmlElement);
		}
		return xmlDocument.CreateNavigator().Select("//match");
	}
}
