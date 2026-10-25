using System.Collections.Generic;

namespace Mvp.Xml.XPointer;

internal class XPointerParser
{
	private static IDictionary<string, XPointerSchema.SchemaType> _schemas = XPointerSchema.Schemas;

	private static XPointerSchema.SchemaType GetSchema(XPointerLexer lexer, IList<PointerPart> parts)
	{
		string text;
		if (lexer.Prefix != string.Empty)
		{
			text = null;
			for (int num = parts.Count - 1; num >= 0; num--)
			{
				PointerPart pointerPart = parts[num];
				if (pointerPart is XmlnsSchemaPointerPart)
				{
					XmlnsSchemaPointerPart xmlnsSchemaPointerPart = (XmlnsSchemaPointerPart)pointerPart;
					if (xmlnsSchemaPointerPart.Prefix == lexer.Prefix)
					{
						text = xmlnsSchemaPointerPart.Uri;
						break;
					}
				}
			}
			if (text == null)
			{
				return XPointerSchema.SchemaType.Unknown;
			}
		}
		else
		{
			text = string.Empty;
		}
		string key = text + ':' + lexer.NCName;
		if (!_schemas.ContainsKey(key))
		{
			return XPointerSchema.SchemaType.Unknown;
		}
		return _schemas[key];
	}

	public static Pointer ParseXPointer(string xpointer)
	{
		XPointerLexer xPointerLexer = new XPointerLexer(xpointer);
		xPointerLexer.NextLexeme();
		if (xPointerLexer.Kind == XPointerLexer.LexKind.NCName && !xPointerLexer.CanBeSchemaName)
		{
			Pointer result = new ShorthandPointer(xPointerLexer.NCName);
			xPointerLexer.NextLexeme();
			if (xPointerLexer.Kind != XPointerLexer.LexKind.Eof)
			{
				throw new XPointerSyntaxException(SR.InvalidTokenAfterShorthandPointer);
			}
			return result;
		}
		IList<PointerPart> list = new List<PointerPart>();
		while (xPointerLexer.Kind != XPointerLexer.LexKind.Eof)
		{
			if ((xPointerLexer.Kind == XPointerLexer.LexKind.NCName || xPointerLexer.Kind == XPointerLexer.LexKind.QName) && xPointerLexer.CanBeSchemaName)
			{
				XPointerSchema.SchemaType schema = GetSchema(xPointerLexer, list);
				xPointerLexer.NextLexeme();
				switch (schema)
				{
				case XPointerSchema.SchemaType.Element:
				{
					ElementSchemaPointerPart elementSchemaPointerPart = ElementSchemaPointerPart.ParseSchemaData(xPointerLexer);
					if (elementSchemaPointerPart != null)
					{
						list.Add(elementSchemaPointerPart);
					}
					break;
				}
				case XPointerSchema.SchemaType.Xmlns:
				{
					XmlnsSchemaPointerPart xmlnsSchemaPointerPart = XmlnsSchemaPointerPart.ParseSchemaData(xPointerLexer);
					if (xmlnsSchemaPointerPart != null)
					{
						list.Add(xmlnsSchemaPointerPart);
					}
					break;
				}
				case XPointerSchema.SchemaType.XPath1:
				{
					XPath1SchemaPointerPart xPath1SchemaPointerPart = XPath1SchemaPointerPart.ParseSchemaData(xPointerLexer);
					if (xPath1SchemaPointerPart != null)
					{
						list.Add(xPath1SchemaPointerPart);
					}
					break;
				}
				case XPointerSchema.SchemaType.XPointer:
				{
					XPointerSchemaPointerPart xPointerSchemaPointerPart = XPointerSchemaPointerPart.ParseSchemaData(xPointerLexer);
					if (xPointerSchemaPointerPart != null)
					{
						list.Add(xPointerSchemaPointerPart);
					}
					break;
				}
				default:
					xPointerLexer.ParseEscapedData();
					break;
				}
				xPointerLexer.NextLexeme();
				if (xPointerLexer.Kind == XPointerLexer.LexKind.Space)
				{
					xPointerLexer.NextLexeme();
				}
				continue;
			}
			throw new XPointerSyntaxException(SR.InvalidToken);
		}
		return new SchemaBasedPointer(list, xpointer);
	}
}
