using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.Common.Xsl;

public class XmlInput
{
	internal object source;

	internal XmlResolver resolver;

	public XmlInput(XmlReader reader, XmlResolver resolver)
	{
		source = reader;
		this.resolver = resolver;
	}

	public XmlInput(TextReader reader, XmlResolver resolver)
	{
		source = reader;
		this.resolver = resolver;
	}

	public XmlInput(Stream stream, XmlResolver resolver)
	{
		source = stream;
		this.resolver = resolver;
	}

	public XmlInput(string uri, XmlResolver resolver)
	{
		source = uri;
		this.resolver = resolver;
	}

	public XmlInput(IXPathNavigable nav, XmlResolver resolver)
	{
		source = nav;
		this.resolver = resolver;
	}

	public XmlInput(XmlReader reader)
		: this(reader, new DefaultXmlResolver())
	{
	}

	public XmlInput(TextReader reader)
		: this(reader, new DefaultXmlResolver())
	{
	}

	public XmlInput(Stream stream)
		: this(stream, new DefaultXmlResolver())
	{
	}

	public XmlInput(string uri)
		: this(uri, new DefaultXmlResolver())
	{
	}

	public XmlInput(IXPathNavigable nav)
		: this(nav, new DefaultXmlResolver())
	{
	}
}
