using System;
using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.XPointer;

internal class XmlnsSchemaPointerPart : PointerPart
{
	private string _prefix;

	private string _uri;

	public string Prefix
	{
		get
		{
			return _prefix;
		}
		set
		{
			_prefix = value;
		}
	}

	public string Uri
	{
		get
		{
			return _uri;
		}
		set
		{
			_uri = value;
		}
	}

	public XmlnsSchemaPointerPart(string prefix, string uri)
	{
		_prefix = prefix;
		_uri = uri;
	}

	public override XPathNodeIterator Evaluate(XPathNavigator doc, XmlNamespaceManager nm)
	{
		nm.AddNamespace(_prefix, _uri);
		return null;
	}

	public static XmlnsSchemaPointerPart ParseSchemaData(XPointerLexer lexer)
	{
		lexer.NextLexeme();
		if (lexer.Kind != XPointerLexer.LexKind.NCName)
		{
			return null;
		}
		string nCName = lexer.NCName;
		lexer.SkipWhiteSpace();
		lexer.NextLexeme();
		if (lexer.Kind != XPointerLexer.LexKind.Eq)
		{
			return null;
		}
		lexer.SkipWhiteSpace();
		string uri;
		try
		{
			uri = lexer.ParseEscapedData();
		}
		catch (Exception ex)
		{
			throw new XPointerSyntaxException(SR.GetString("SyntaxErrorInXmlnsSchemeData", ex.Message));
		}
		return new XmlnsSchemaPointerPart(nCName, uri);
	}
}
