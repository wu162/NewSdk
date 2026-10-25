using System.Collections.Generic;

namespace Mvp.Xml.XPointer;

internal class XPointerSchema
{
	public enum SchemaType
	{
		Element,
		Xmlns,
		XPath1,
		XPointer,
		Unknown
	}

	public static IDictionary<string, SchemaType> Schemas = CreateSchemasTable();

	private static IDictionary<string, SchemaType> CreateSchemasTable()
	{
		IDictionary<string, SchemaType> dictionary = new Dictionary<string, SchemaType>(4);
		dictionary.Add(":element", SchemaType.Element);
		dictionary.Add(":xmlns", SchemaType.Xmlns);
		dictionary.Add(":xpath1", SchemaType.XPath1);
		dictionary.Add(":xpointer", SchemaType.XPointer);
		return dictionary;
	}
}
