using System.Xml;

namespace Mvp.Xml.Common;

public class XmlPrefix
{
	private string _prefix;

	private string _ns;

	public string Prefix => _prefix;

	public string NamespaceURI => _ns;

	public XmlPrefix(string prefix, string ns)
	{
		_prefix = prefix;
		_ns = ns;
	}

	public XmlPrefix(string prefix, string ns, XmlNameTable nameTable)
	{
		_prefix = nameTable.Add(prefix);
		_ns = nameTable.Add(ns);
	}
}
