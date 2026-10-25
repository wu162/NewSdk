using System.Text;
using System.Xml;
using System.Xml.XPath;
using Mvp.Xml.Common.XPath;

namespace Mvp.Xml.XPointer;

internal class ElementSchemaPointerPart : PointerPart
{
	private string _xpath;

	public string XPath
	{
		get
		{
			return _xpath;
		}
		set
		{
			_xpath = value;
		}
	}

	public override XPathNodeIterator Evaluate(XPathNavigator doc, XmlNamespaceManager nm)
	{
		return XPathCache.Select(_xpath, doc, nm);
	}

	public static ElementSchemaPointerPart ParseSchemaData(XPointerLexer lexer)
	{
		StringBuilder stringBuilder = new StringBuilder();
		ElementSchemaPointerPart elementSchemaPointerPart = new ElementSchemaPointerPart();
		lexer.NextLexeme();
		if (lexer.Kind == XPointerLexer.LexKind.NCName)
		{
			stringBuilder.Append("id('");
			stringBuilder.Append(lexer.NCName);
			stringBuilder.Append("')");
			lexer.NextLexeme();
		}
		int num = 0;
		while (lexer.Kind == XPointerLexer.LexKind.Slash)
		{
			lexer.NextLexeme();
			if (lexer.Kind != XPointerLexer.LexKind.Number)
			{
				return null;
			}
			if (lexer.Number == 0)
			{
				return null;
			}
			num++;
			stringBuilder.Append("/*[");
			stringBuilder.Append(lexer.Number);
			stringBuilder.Append("]");
			lexer.NextLexeme();
		}
		if (lexer.Kind != XPointerLexer.LexKind.RRBracket)
		{
			throw new XPointerSyntaxException(SR.InvalidTokenInElementSchemeWhileClosingRoundBracketExpected);
		}
		if (stringBuilder.Length == 0 && num == 0)
		{
			return null;
		}
		elementSchemaPointerPart.XPath = stringBuilder.ToString();
		return elementSchemaPointerPart;
	}
}
