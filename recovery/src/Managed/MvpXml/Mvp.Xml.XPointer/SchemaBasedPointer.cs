using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.XPointer;

internal class SchemaBasedPointer : Pointer
{
	private IList<PointerPart> _parts;

	private string _xpointer;

	public SchemaBasedPointer(IList<PointerPart> parts, string xpointer)
	{
		_parts = parts;
		_xpointer = xpointer;
	}

	public override XPathNodeIterator Evaluate(XPathNavigator nav)
	{
		XmlNamespaceManager nm = new XmlNamespaceManager(nav.NameTable);
		for (int i = 0; i < _parts.Count; i++)
		{
			PointerPart pointerPart = _parts[i];
			XPathNodeIterator xPathNodeIterator = pointerPart.Evaluate(nav, nm);
			if (xPathNodeIterator != null && xPathNodeIterator.MoveNext())
			{
				return xPathNodeIterator;
			}
		}
		throw new NoSubresourcesIdentifiedException(SR.GetString("NoSubresourcesIdentifiedException", _xpointer));
	}
}
