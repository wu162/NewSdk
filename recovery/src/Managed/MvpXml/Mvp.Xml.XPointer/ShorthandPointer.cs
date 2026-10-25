using System.Xml;
using System.Xml.XPath;
using Mvp.Xml.Common.XPath;

namespace Mvp.Xml.XPointer;

internal class ShorthandPointer : Pointer
{
	private string _NCName;

	public ShorthandPointer(string n)
	{
		_NCName = n;
	}

	public override XPathNodeIterator Evaluate(XPathNavigator nav)
	{
		XPathNodeIterator xPathNodeIterator = XPathCache.Select("id('" + _NCName + "')", nav, (XmlNamespaceManager)null);
		if (xPathNodeIterator != null && xPathNodeIterator.MoveNext())
		{
			return xPathNodeIterator;
		}
		throw new NoSubresourcesIdentifiedException(SR.GetString("NoSubresourcesIdentifiedException", _NCName));
	}
}
