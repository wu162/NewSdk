using System;
using System.Xml;
using System.Xml.XPath;
using Mvp.Xml.Common.XPath;

namespace Mvp.Xml.XPointer;

internal class XPointerSchemaPointerPart : PointerPart
{
	private string _xpath;

	public XPointerSchemaPointerPart(string xpath)
	{
		_xpath = xpath;
	}

	public override XPathNodeIterator Evaluate(XPathNavigator doc, XmlNamespaceManager nm)
	{
		try
		{
			return XPathCache.Select(_xpath, doc, nm);
		}
		catch
		{
			return null;
		}
	}

	public static XPointerSchemaPointerPart ParseSchemaData(XPointerLexer lexer)
	{
		try
		{
			return new XPointerSchemaPointerPart(lexer.ParseEscapedData());
		}
		catch (Exception ex)
		{
			throw new XPointerSyntaxException(SR.GetString("SyntaxErrorInXPointerSchemeData", ex.Message));
		}
	}
}
