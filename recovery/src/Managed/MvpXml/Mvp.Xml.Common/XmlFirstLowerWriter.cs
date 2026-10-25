using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Mvp.Xml.Common;

public class XmlFirstLowerWriter : XmlTextWriter
{
	public XmlFirstLowerWriter(TextWriter w)
		: base(w)
	{
	}

	public XmlFirstLowerWriter(Stream w, Encoding encoding)
		: base(w, encoding)
	{
	}

	public XmlFirstLowerWriter(string filename, Encoding encoding)
		: base(filename, encoding)
	{
	}

	internal static string MakeFirstLower(string name)
	{
		if (name.Length == 0)
		{
			return name;
		}
		if (char.IsLower(name[0]))
		{
			return name;
		}
		if (name.Length == 1)
		{
			return name.ToLower(CultureInfo.CurrentCulture);
		}
		char[] array = name.ToCharArray();
		array[0] = char.ToLower(array[0], CultureInfo.CurrentCulture);
		return new string(array);
	}

	public override void WriteQualifiedName(string localName, string ns)
	{
		base.WriteQualifiedName(MakeFirstLower(localName), ns);
	}

	public override void WriteStartAttribute(string prefix, string localName, string ns)
	{
		base.WriteStartAttribute(prefix, MakeFirstLower(localName), ns);
	}

	public override void WriteStartElement(string prefix, string localName, string ns)
	{
		base.WriteStartElement(prefix, MakeFirstLower(localName), ns);
	}
}
