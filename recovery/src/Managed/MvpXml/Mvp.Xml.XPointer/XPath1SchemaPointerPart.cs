using System;
using System.Xml;
using System.Xml.XPath;
using Mvp.Xml.Common.XPath;

namespace Mvp.Xml.XPointer;

internal class XPath1SchemaPointerPart : PointerPart
{
	private string _xpath;

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

	public static XPath1SchemaPointerPart ParseSchemaData(XPointerLexer lexer)
	{
		XPath1SchemaPointerPart xPath1SchemaPointerPart = new XPath1SchemaPointerPart();
		try
		{
			xPath1SchemaPointerPart._xpath = lexer.ParseEscapedData();
			return xPath1SchemaPointerPart;
		}
		catch (Exception ex)
		{
			throw new XPointerSyntaxException(SR.GetString("SyntaxErrorInXPath1SchemeData", ex.Message));
		}
	}
}
