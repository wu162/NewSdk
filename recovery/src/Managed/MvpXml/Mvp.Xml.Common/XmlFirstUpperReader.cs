using System.Globalization;
using System.IO;
using System.Xml;

namespace Mvp.Xml.Common;

public class XmlFirstUpperReader : XmlTextReader
{
	public override string this[string name, string namespaceURI] => base[NameTable.Add(XmlFirstLowerWriter.MakeFirstLower(name)), namespaceURI];

	public override string this[string name] => this[name, string.Empty];

	public override string LocalName
	{
		get
		{
			if (base.NodeType == XmlNodeType.Element || base.NodeType == XmlNodeType.EndElement || base.NodeType == XmlNodeType.Attribute)
			{
				if (!(base.NamespaceURI == "http://www.w3.org/2000/xmlns/"))
				{
					return MakeFirstUpper(base.LocalName);
				}
				return base.LocalName;
			}
			return base.LocalName;
		}
	}

	public override string Name
	{
		get
		{
			if (base.NamespaceURI == "http://www.w3.org/2000/xmlns/")
			{
				return base.Name;
			}
			if (base.Name.IndexOf(":") == -1)
			{
				return MakeFirstUpper(base.Name);
			}
			string text = base.Name.Substring(0, base.Name.IndexOf(":") + 1);
			text += MakeFirstUpper(base.Name.Substring(base.Name.IndexOf(":") + 1));
			return NameTable.Add(text);
		}
	}

	public XmlFirstUpperReader(Stream input)
		: base(input)
	{
	}

	public XmlFirstUpperReader(TextReader input)
		: base(input)
	{
	}

	public XmlFirstUpperReader(string url)
		: base(url)
	{
	}

	public XmlFirstUpperReader(Stream input, XmlNameTable nt)
		: base(input, nt)
	{
	}

	public XmlFirstUpperReader(TextReader input, XmlNameTable nt)
		: base(input, nt)
	{
	}

	public XmlFirstUpperReader(string url, Stream input)
		: base(url, input)
	{
	}

	public XmlFirstUpperReader(string url, TextReader input)
		: base(url, input)
	{
	}

	public XmlFirstUpperReader(string url, XmlNameTable nt)
		: base(url, nt)
	{
	}

	public XmlFirstUpperReader(Stream xmlFragment, XmlNodeType fragType, XmlParserContext context)
		: base(xmlFragment, fragType, context)
	{
	}

	public XmlFirstUpperReader(string url, Stream input, XmlNameTable nt)
		: base(url, input, nt)
	{
	}

	public XmlFirstUpperReader(string url, TextReader input, XmlNameTable nt)
		: base(url, input, nt)
	{
	}

	public XmlFirstUpperReader(string xmlFragment, XmlNodeType fragType, XmlParserContext context)
		: base(xmlFragment, fragType, context)
	{
	}

	private string MakeFirstUpper(string name)
	{
		if (name.Length == 0)
		{
			return name;
		}
		if (char.IsUpper(name[0]))
		{
			return name;
		}
		if (name.Length == 1)
		{
			return name.ToUpper(CultureInfo.CurrentCulture);
		}
		char[] array = name.ToCharArray();
		array[0] = char.ToUpper(array[0], CultureInfo.CurrentUICulture);
		return NameTable.Add(new string(array));
	}

	public override bool MoveToAttribute(string name, string ns)
	{
		return base.MoveToAttribute(NameTable.Add(XmlFirstLowerWriter.MakeFirstLower(name)), ns);
	}
}
